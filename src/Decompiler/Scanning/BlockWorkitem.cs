﻿#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcedureCharacteristics = Reko.Core.Serialization.ProcedureCharacteristics;
using Reko.Analysis;
using Reko.Core.Services;

namespace Reko.Scanning
{
    /// <summary>
    /// Scanner work item for processing a basic block.
    /// </summary>
    /// <remarks>
    /// The block work item will disassemble and rewrite instructions linearly
    /// until it reaches:
    /// <list type="">
    /// <item>An condition or unconditional branch</item>
    /// <item>A call to a procedure that is known to terminate.</item>
    /// </list></remarks>
    public class BlockWorkitem : WorkItem, RtlInstructionVisitor<bool>
    {
        private IScanner scanner;
        private Program program;
        private IProcessorArchitecture arch;
        private Address addrStart;
        private Block blockCur;
        private Frame frame;
        private RtlInstruction ri;
        private RtlInstructionCluster ric;
        private IEnumerator<RtlInstructionCluster> rtlStream;
        private ProcessorState state;
        private ExpressionSimplifier eval;
        private int extraLabels;
        private Identifier stackReg;
        private VarargsFormatScanner vaScanner;

        public BlockWorkitem(
            IScanner scanner,
            Program program,
            ProcessorState state,
            Address addr) : base(addr)
        {
            this.scanner = scanner;
            this.program = program;
            this.arch = program.Architecture;   // cached since it's used heavily.
            this.state = state;
            this.eval = new ExpressionSimplifier(
                state,
                scanner.Services.RequireService<DecompilerEventListener>());
            this.addrStart = addr;
            this.blockCur = null;
        }

        /// <summary>
        /// Processes the statements of a basic block by using the architecture-specific
        /// Rewriter to obtain a stream of low-level RTL instructions. RTL assignments are 
        /// simply added to the instruction list of the basic block. Jumps, returns, and 
        /// calls to procedures that terminate the thread of executationresult in the 
        /// termination of processing.
        /// </summary>
        public override void Process()
        {
            state.ErrorListener = (message) => { scanner.Warn(ric.Address, message); };
            blockCur = scanner.FindContainingBlock(addrStart);
            if (blockCur == null || BlockHasBeenScanned(blockCur))
                return;

            frame = blockCur.Procedure.Frame;
            this.stackReg = frame.EnsureRegister(arch.StackRegister);
            this.vaScanner = new VarargsFormatScanner(program, frame, state, scanner.Services);
            rtlStream = scanner.GetTrace(addrStart, state, frame)
                .GetEnumerator();

            while (rtlStream.MoveNext())
            {
                this.ric = rtlStream.Current;
                if (blockCur != scanner.FindContainingBlock(ric.Address))
                    break;  // Fell off the end of this block.
                if (!ProcessRtlCluster(ric))
                    break;
                var addrInstrEnd = ric.Address + ric.Length;
                var blNext = FallenThroughNextProcedure(ric.Address, addrInstrEnd);
                if (blNext != null)
                {
                    EnsureEdge(blockCur.Procedure, blockCur, blNext);
                    return;
                }
                blNext = FallenThroughNextBlock(addrInstrEnd);
                if (blNext != null)
                {
                    EnsureEdge(blockCur.Procedure, blockCur, blNext);
                    return;
                }
            }
        }

        private bool ProcessRtlCluster(RtlInstructionCluster ric)
        {
            state.SetInstructionPointer(ric.Address);
            SetAssumedRegisterValues(ric.Address);
            foreach (var rtlInstr in ric.Instructions)
            {
                ri = rtlInstr;
                if (!ri.Accept(this))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If a user has requested register values to be hard wired at a
        /// particular address, do so by emitting assignments that set
        /// the relevant registers to the right values.
        /// </summary>
        /// <param name="addr"></param>
        public void SetAssumedRegisterValues(Address addr)
        {
            List<UserRegisterValue> regValues;
            if (!program.User.RegisterValues.TryGetValue(addr, out regValues))
                return;
            foreach (var rv in regValues)
            {
                var reg = frame.EnsureRegister(rv.Register);
                new RtlAssignment(reg, rv.Value).Accept(this);
            }
        }

        private Block FallenThroughNextProcedure(Address addrInstr, Address addrTo)
        {
            Procedure procOther;
            if (program.Procedures.TryGetValue(addrTo, out procOther) &&
                procOther != blockCur.Procedure)
            {
                // Fell into another procedure. 
                var block = scanner.CreateCallRetThunk(addrInstr, blockCur.Procedure, procOther);
                return block;
            }
            return null;
        }

        /// <summary>
        /// Checks to see if the scanning process has wandered off
        /// into another, previously existing basic block or procedure.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>The block we fell into or null if we remained in the 
        /// same block.</returns>
        private Block FallenThroughNextBlock(Address addr)
        {
            var cont = scanner.FindContainingBlock(addr);
            if (cont == null || cont == blockCur)
                return null;
            return BlockFromAddress(ric.Address, addr, blockCur.Procedure, state);
        }

        private bool BlockHasBeenScanned(Block block)
        {
            return block.Statements.Count > 0;
        }

        private Instruction BuildApplication(Expression fn, FunctionType sig, CallSite site)
        {
            var ab = CreateApplicationBuilder(fn, sig, site);
            return ab.CreateInstruction();
        }

        private ApplicationBuilder CreateApplicationBuilder(Expression callee, FunctionType sig, CallSite site)
        {
            var ab = new ApplicationBuilder(
                arch,
                frame,
                site,
                callee,
                sig, 
                false);
            return ab;
        }

        public Expression GetValue(Expression op)
        {
            return op.Accept<Expression>(eval);
        }

        public void SetValue(Expression dst, Expression value)
        {
            var id = dst as Identifier;
            if (id != null)
            {
                state.SetValue(id, value);
                return;
            }
            var smem = dst as SegmentedAccess;
            if (smem != null)
            {
                state.SetValueEa(smem.BasePointer, GetValue(smem.EffectiveAddress), value);
            }
            var mem = dst as MemoryAccess;
            if (mem != null)
            {
                state.SetValueEa(GetValue(mem.EffectiveAddress), value);
                return;
            }
        }

        #region RtlInstructionVisitor Members

        /// <summary>
        /// Assignments are simulated on the processor state.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool VisitAssignment(RtlAssignment a)
        {
            SetValue(a.Dst, GetValue(a.Src));
            var idDst = a.Dst as Identifier;
            var inst = (idDst != null)
                ? new Assignment(idDst, a.Src)
                : (Instruction) new Store(a.Dst, a.Src);
            Emit(inst);
            return true;
        }

        /// <summary>
        /// Branches need to terminate the current basic block and make links
        /// to the 'true' and 'false' destinations.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool VisitBranch(RtlBranch b)
        {
            // We don't know the 'then' block yet, as the following statements may chop up the block
            // we're presently in. Back-patch in when the block target is obtained.
            var branch = new Branch(b.Condition, new Block(blockCur.Procedure, "TMP!"));
            Emit(branch, blockCur);

            // The following statements may chop up the blockCur, so hang on to the essentials.
            var proc = blockCur.Procedure;
            RtlInstructionCluster ricDelayed = null;
            if ((b.Class & RtlClass.Delay) != 0)
            {
                rtlStream.MoveNext();
                ricDelayed = rtlStream.Current;
                ric = ricDelayed;
            }
            var fallthruAddress = ric.Address + ric.Length;

            Block blockThen;
            if (!program.SegmentMap.IsValidAddress((Address)b.Target))
            {
                blockThen = proc.AddBlock(this.ric.Address.GenerateName("l", "_then"));
                var jmpSite = state.OnBeforeCall(stackReg, arch.PointerType.Size);
                GenerateCallToOutsideProcedure(jmpSite, (Address)b.Target);
                Emit(new ReturnInstruction());
                blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
            }
            else
            {
                blockThen = BlockFromAddress(ric.Address, (Address)b.Target, proc, state.Clone());
            }

            var blockElse = FallthroughBlock(ric.Address, proc, fallthruAddress);
            var branchingBlock = blockCur.IsSynthesized
                ? blockCur
                : scanner.FindContainingBlock(ric.Address);

            if ((b.Class & RtlClass.Delay) != 0 &&
                ricDelayed.Instructions.Length > 0)
            {
                // Introduce stubs for the delay slot, but only
                // if the delay slot isn't empty.

                if ((b.Class & RtlClass.Annul) != 0)
                {
                    EnsureEdge(proc, branchingBlock, blockElse);
                }
                else
                {
                    Block blockDsF = null;
                    blockDsF = proc.AddBlock(branchingBlock.Name + "_ds_f");
                    blockDsF.IsSynthesized = true;
                    blockCur = blockDsF;
                    ProcessRtlCluster(ricDelayed);
                    EnsureEdge(proc, blockDsF, blockElse);
                    EnsureEdge(proc, branchingBlock, blockDsF);
                }

                Block blockDsT = proc.AddBlock(branchingBlock.Name + "_ds_t");
                blockDsT.IsSynthesized = true;
                blockCur = blockDsT;
                ProcessRtlCluster(ricDelayed);
                EnsureEdge(proc, blockDsT, blockThen);
                branch.Target = blockDsT;
                EnsureEdge(proc, branchingBlock, blockDsT);
            }
            else
            {
                branch.Target = blockThen;      // The back-patch referred to above.
                EnsureEdge(proc, branchingBlock, blockElse);
                if (blockElse != blockThen)
                    EnsureEdge(proc, branchingBlock, blockThen);
                else
                    proc.ControlGraph.AddEdge(branchingBlock, blockThen);
            }
            if (!BlockHasBeenScanned(blockElse))
            {
                blockCur = blockElse;
            }
            return true;
        }

        /// <summary>
        /// Conditional instructions basic blocks to host them.
        /// </summary>
        /// <param name="rtlIf"></param>
        /// <returns></returns>
        public bool VisitIf(RtlIf rtlIf)
        {
            var branch = new Branch(rtlIf.Condition.Invert(), null);
            Emit(branch);

            var proc = blockCur.Procedure;
            var fallthruAddress = ric.Address + ric.Length;

            var blockInstr = AddIntraStatementBlock(proc);
            var blockFollow = BlockFromAddress(ric.Address, fallthruAddress, proc, state);

            blockCur = blockInstr;
            rtlIf.Instruction.Accept(this);

            var branchingBlock = scanner.FindContainingBlock(ric.Address);
            branch.Target = blockFollow;
            EnsureEdge(proc, branchingBlock, blockInstr);
            EnsureEdge(proc, branchingBlock, blockFollow);
            EnsureEdge(proc, blockInstr, blockFollow);

            blockCur = blockFollow;
            return true;
        }

        /// <summary>
        /// Encountering invalid instructions is unexpected.
        /// </summary>
        /// <param name="invalid"></param>
        /// <returns></returns>
        public bool VisitInvalid(RtlInvalid invalid)
        {
            return false;
        }

        public bool VisitNop(RtlNop nop)
        {
            return true;
        }
       
        private Block BlockFromAddress(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state)
        {
            return scanner.EnqueueJumpTarget(addrSrc, addrDst, proc, state);
        }

        private void EnsureEdge(Procedure proc, Block blockFrom, Block blockTo)
        {
            if (!proc.ControlGraph.ContainsEdge(blockFrom, blockTo))
                proc.ControlGraph.AddEdge(blockFrom, blockTo);
        }

        /// <summary>
        /// RtlGoto transfers control to either a constant destination or 
        /// a variable destination computed at run-time.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public bool VisitGoto(RtlGoto g)
        {
            var blockFrom = blockCur;
            if ((g.Class & RtlClass.Delay) != 0)
            {
                // Get next instruction cluster.
                rtlStream.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            CallSite site;
            scanner.TerminateBlock(blockCur, rtlStream.Current.Address + ric.Length);
            var addrTarget = g.Target as Address;
            if (addrTarget != null)
            {
                var impProc = scanner.GetImportedProcedure(addrTarget, this.ric.Address);
                if (impProc != null)
                {
                    site = state.OnBeforeCall(stackReg, arch.PointerType.Size);
                    var sig = impProc.Signature;
                    var chr = impProc.Characteristics;
                    if (chr != null && chr.IsAlloca)
                        return ProcessAlloca(site, impProc);
                    EmitCall(CreateProcedureConstant(impProc), sig, chr, site);
                    Emit(new ReturnInstruction());
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                if (!program.SegmentMap.IsValidAddress(addrTarget))
                {
                    var jmpSite = state.OnBeforeCall(stackReg, arch.PointerType.Size);
                    GenerateCallToOutsideProcedure(jmpSite, addrTarget);
                    Emit(new ReturnInstruction());
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                var blockTarget = BlockFromAddress(ric.Address, addrTarget, blockCur.Procedure, state);
                var blockSource = blockCur.IsSynthesized
                    ? blockCur
                    : scanner.FindContainingBlock(ric.Address);
                EnsureEdge(blockSource.Procedure, blockSource, blockTarget);
                if (ric.Address == addrTarget)
                {
                    var bt = BlockFromAddress(ric.Address, addrTarget, blockCur.Procedure, state);
                    EnsureEdge(blockSource.Procedure, blockFrom, bt);
                }
                return false;
            }
            var mem = g.Target as MemoryAccess;
            if (mem != null)
            {
                if (mem.EffectiveAddress is Constant)
                {
                    // jmp [address]
                    site = state.OnBeforeCall(this.stackReg, 4);            //$BUGBUG: hard coded.
                    Emit(new CallInstruction(g.Target, site));
                    Emit(new ReturnInstruction());
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
            }
            if (ProcessIndirectControlTransfer(ric.Address, g))
                return false;
            site = state.OnBeforeCall(this.stackReg, 4);    //$BUGBUG: hard coded
            Emit(new CallInstruction(g.Target, site));
            Emit(new ReturnInstruction());
            blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
            return false;
        }

        private ProcedureConstant CreateProcedureConstant(ProcedureBase callee)
        {
            return new ProcedureConstant(program.Platform.PointerType, callee);
        }

        public bool VisitCall(RtlCall call)
        {
            if ((call.Class & RtlClass.Delay) != 0)
            {
                // Get delay slot instruction cluster.
                rtlStream.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            var site = OnBeforeCall(stackReg, call.ReturnAddressSize);
            FunctionType sig;
            ProcedureCharacteristics chr = null;
            Address addr = call.Target as Address;
            if (addr != null)
            {
                var impProc = scanner.GetImportedProcedure(addr, this.ric.Address);
                if (impProc != null)
                {
                    sig = impProc.Signature;
                    chr = impProc.Characteristics;
                    if (chr != null && chr.IsAlloca)
                        return ProcessAlloca(site, impProc);
                    EmitCall(CreateProcedureConstant(impProc), sig, chr, site);
                    return OnAfterCall(sig, chr);
                }
                if (!program.SegmentMap.IsValidAddress(addr))
                {
                    return GenerateCallToOutsideProcedure(site, addr);
                }
                var callee = scanner.ScanProcedure(addr, null, state);
                var pcCallee = CreateProcedureConstant(callee);
                sig = callee.Signature;
                chr = callee.Characteristics;
                EmitCall(pcCallee, sig, chr, site);
                var pCallee = callee as Procedure;
                if (pCallee != null)
                {
                    program.CallGraph.AddEdge(blockCur.Statements.Last, pCallee);
                }
                return OnAfterCall(sig, chr);
            }

            var procCallee = call.Target as ProcedureConstant;
            if (procCallee != null)
            {
                sig = procCallee.Procedure.Signature;
                chr = procCallee.Procedure.Characteristics;
                EmitCall(procCallee, sig, chr, site);
                return OnAfterCall(sig, chr);
            }
            sig = GetCallSignatureAtAddress(ric.Address);
            if (sig != null)
            {
                EmitCall(call.Target, sig, chr, site);
                return OnAfterCall(sig, chr);  //$TODO: make characteristics available
            }

            Identifier id; 
            if (call.Target.As<Identifier>(out id))
            {
                var ppp = SearchBackForProcedureConstant(id);
                if (ppp != null)
                {
                    var e = CreateProcedureConstant(ppp);
                    sig = ppp.Signature;
                    chr = ppp.Characteristics;
                    EmitCall(e, sig, chr, site);
                    return OnAfterCall(sig, chr);
                }
            }

            var imp = ImportedProcedureName(call.Target);
            if (imp != null)
            {
                sig = imp.Signature;
                chr = imp.Characteristics;
                EmitCall(CreateProcedureConstant(imp), sig, chr, site);
                return OnAfterCall(sig, chr);
            }

            var syscall = program.Platform.FindService(call, state);
            if (syscall != null)
            {
                return !EmitSystemServiceCall(syscall);
            }

            ProcessIndirectControlTransfer(ric.Address, call);

            var ic = new CallInstruction(call.Target, site);
            Emit(ic);
            sig = GuessProcedureSignature(ic);
            return OnAfterCall(sig, chr);
        }

        private bool GenerateCallToOutsideProcedure(CallSite site, Address addr)
        {
            scanner.Warn(ric.Address, "Call target address {0} is invalid.", addr);
            var sig = new FunctionType();
            ProcedureCharacteristics chr = null;
            EmitCall(
                CreateProcedureConstant(
                    new ExternalProcedure(Procedure.GenerateName(addr), sig)),
                sig,
                chr,
                site);
            return OnAfterCall(sig, chr);
        }

        private void EmitCall(
            Expression callee,
            FunctionType sig,
            ProcedureCharacteristics chr,
            CallSite site)
        {
            if (vaScanner.TryScan(ric.Address, sig, chr))
            {
                Emit(vaScanner.BuildInstruction(callee, site));
            }
            else if (sig != null && sig.ParametersValid)
            {
                Emit(BuildApplication(callee, sig, site));
            }
            else
            {
                Emit(new CallInstruction(callee, site));
            }
        }

        private CallSite OnBeforeCall(Identifier stackReg, int sizeOfRetAddrOnStack)
        {
            if (sizeOfRetAddrOnStack > 0)
            {
                //$BUG: stack grows negative here; some stacks might grow
                // positive?
                Expression newVal = new BinaryExpression(
                        Operator.ISub,
                        stackReg.DataType,
                        stackReg,
                        Constant.Create(
                            PrimitiveType.CreateWord(sizeOfRetAddrOnStack),
                            sizeOfRetAddrOnStack));
                newVal = newVal.Accept(eval);
                SetValue(stackReg, newVal);
            }
            return state.OnBeforeCall(stackReg, sizeOfRetAddrOnStack);
        }

        private bool OnAfterCall(FunctionType sigCallee, ProcedureCharacteristics characteristics)
        {
            UserCallData userCall = null;
            if (program.User.Calls.TryGetUpperBound(ric.Address, out userCall))
            {
                var linStart = ric.Address.ToLinear();
                var linEnd = linStart + ric.Length;
                var linUserCall = userCall.Address.ToLinear();
                if (linStart > linUserCall || linUserCall >= linEnd)
                    userCall = null;
            }
            if ((characteristics != null && characteristics.Terminates) ||
                (userCall != null && userCall.NoReturn))
            {
                scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                return false;
            }

            if (sigCallee != null)
            {
                if (sigCallee.StackDelta != 0)
                {
                    Expression newVal = new BinaryExpression(
                            Operator.IAdd,
                            stackReg.DataType,
                            stackReg,
                            Constant.Create(
                                PrimitiveType.CreateWord(stackReg.DataType.Size),
                                sigCallee.StackDelta));
                    newVal = newVal.Accept(eval);
                    SetValue(stackReg, newVal);
                }
            }
            state.OnAfterCall(sigCallee);

            // Adjust stack after call
            if (sigCallee != null)
            {
                int delta = sigCallee.StackDelta - sigCallee.ReturnAddressOnStack;
                if (delta != 0)
                {
                    var d = Constant.Create(stackReg.DataType, delta);
                    this.Emit(new Assignment(
                        stackReg,
                        new BinaryExpression(Operator.IAdd, stackReg.DataType, stackReg, d)));
                }
            }
            return true;
        }

        private FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
        {
            UserCallData call = null;
            if (!program.User.Calls.TryGetValue(addrCallInstruction, out call))
                return null;
            return call.Signature;
        }

        public bool VisitReturn(RtlReturn ret)
        {
            if ((ret.Class & RtlClass.Delay) != 0)
            {
                // Get next instruction cluster from the delay slot.
                rtlStream.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            var proc = blockCur.Procedure;
            Emit(new ReturnInstruction());
            proc.ControlGraph.AddEdge(blockCur, proc.ExitBlock);

            int returnAddressBytes = ret.ReturnAddressBytes;
            var address = ric.Address;
            scanner.SetProcedureReturnAddressBytes(proc, returnAddressBytes, address);

            int stackDelta = ret.ReturnAddressBytes + ret.ExtraBytesPopped;
            if (proc.Signature.StackDelta != 0 && proc.Signature.StackDelta != stackDelta)
            {
                scanner.Warn(
                    ric.Address,
                    "Multiple different values of stack delta in procedure {0} when processing RET instruction; was {1} previously.", 
                    proc.Name,
                    proc.Signature.StackDelta);
            }
            else
            {
                proc.Signature.StackDelta = stackDelta;
            }
            state.OnProcedureLeft(proc.Signature);
            scanner.TerminateBlock(blockCur, rtlStream.Current.Address + ric.Length);
            return false;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            var svc = MatchSyscallToService(side);
            if (svc != null)
            {
                return !EmitSystemServiceCall(svc);
            }
            else
            {
                Emit(new SideEffect(side.Expression));
                var appl = side.Expression as Application;
                if (appl != null)
                {
                    var fn = appl.Procedure as ProcedureConstant;
                    if (fn != null)
                    {
                        if (fn.Procedure.Characteristics.Terminates)
                        {
                            scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Takes a system service description and generates a system call from it.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True if the system service terminates.</returns>
        private bool EmitSystemServiceCall(SystemService svc)
        {
            var ep = svc.CreateExternalProcedure(arch);
            var fn = new ProcedureConstant(program.Platform.PointerType, ep);
            if (svc.Signature != null)
            {
                var site = state.OnBeforeCall(stackReg, svc.Signature.ReturnAddressOnStack);
                Emit(BuildApplication(fn, ep.Signature, site));
                if (svc.Characteristics.Terminates)
                {
                    scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                    //blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return true;
                }
                AffectProcessorState(svc.Signature);
            }
            else
            {
                var site = state.OnBeforeCall(stackReg, 0);
                Emit(new CallInstruction(fn, site));
            }
            return false;
        }

        private FunctionType GuessProcedureSignature(CallInstruction call)
        {
            return new FunctionType(); //$TODO: attempt to detect parameters of procedure?
            // This would have to be arch-dependent + platform-dependent as some arch pass
            // on stack, while others pass in registers, or a combination or both
            // ("thiscall" in x86 µsoft world).
        }

        public bool ProcessAlloca(CallSite site, ExternalProcedure impProc)
        {
            if (impProc.Signature == null)
                throw new ApplicationException(string.Format("You must specify a procedure signature for {0} since it has been marked as 'alloca'.", impProc.Name));
            var ab = CreateApplicationBuilder(new ProcedureConstant(program.Platform.PointerType, impProc), impProc.Signature, site);
            if (impProc.Signature.Parameters.Length != 1)
                throw new ApplicationException(string.Format("An alloca function must have exactly one parameter, but {0} has {1}.", impProc.Name, impProc.Signature.Parameters.Length));
            var target = ab.Bind(impProc.Signature.Parameters[0]);
            var id = target as Identifier;
            if (id == null)
                throw new ApplicationException(string.Format("The parameter of {0} wasn't a register.", impProc.Name));
            Constant c = state.GetValue(id) as Constant;
            if (c != null && c.IsValid)
            {
                Emit(new Assignment(stackReg, new BinaryExpression(Operator.ISub, stackReg.DataType, stackReg, c)));
            }
            else
            {
                Emit(ab.CreateInstruction());
            }
            return true;
        }

#endregion

        public bool ProcessIndirectControlTransfer(Address addrSwitch, RtlTransfer xfer)
        {
            List<Address> vector;
            ImageMapVectorTable imgVector;
            Expression swExp;
            UserIndirectJump indJump;

            var listener = scanner.Services.RequireService<DecompilerEventListener>();
            if (program.User.IndirectJumps.TryGetValue(addrSwitch, out indJump))
            {
                vector = indJump.Table.Addresses;
                swExp = this.frame.EnsureIdentifier(indJump.IndexRegister);
                imgVector = indJump.Table;
            }
            else
            {
                var bw = new Backwalker<Block,Instruction>(new BackwalkerHost(this), xfer, eval);
                if (!bw.CanBackwalk())
                    return false;
                var bwops = bw.BackWalk(blockCur);
                if (bwops == null || bwops.Count == 0)
                    return false;     //$REVIEW: warn?
                Identifier idIndex = bw.Index != null
                    ? blockCur.Procedure.Frame.EnsureRegister(bw.Index)
                    : null;

                VectorBuilder builder = new VectorBuilder(scanner.Services, program, new DirectedGraphImpl<object>());
                if (bw.VectorAddress == null)
                    return false;

                vector = builder.BuildAux(bw, addrSwitch, state);
                if (vector.Count == 0)
                {
                    var rdr = program.CreateImageReader(bw.VectorAddress);
                    if (!rdr.IsValid)
                        return false;
                    // Can't determine the size of the table, but surely it has one entry?
                    var addrEntry = arch.ReadCodeAddress(bw.Stride, rdr, state);
                    string msg;
                    if (this.program.SegmentMap.IsValidAddress(addrEntry))
                    {
                        vector.Add(addrEntry);
                        msg = "Can't determine size of jump vector; probing only one entry.";
                    }
                    else
                    {
                        // Nope, not even that.
                        msg = "No valid entries could be found in jump vector.";
                    }
                    var nav = listener.CreateJumpTableNavigator(program, addrSwitch, bw.VectorAddress, bw.Stride);
                    listener.Warn(nav, msg);
                    if (vector.Count == 0)
                        return false;

                }
                imgVector = new ImageMapVectorTable(
                    bw.VectorAddress,
                    vector.ToArray(),
                    builder.TableByteSize);
                swExp = idIndex;
                if (idIndex == null || idIndex.Name == "None")
                    swExp = bw.IndexExpression;
            }
            ScanVectorTargets(xfer, vector);

            if (xfer is RtlGoto)
            {
                var blockSource = scanner.FindContainingBlock(ric.Address);
                blockCur = blockSource;
                foreach (Address addr in vector)
                {
                    var dest = scanner.FindContainingBlock(addr);
                    Debug.Assert(dest != null, "The block at address " + addr + "should have been enqueued.");
                    blockSource.Procedure.ControlGraph.AddEdge(blockSource, dest);
                }
              
                if (swExp == null)
                {
                    scanner.Warn(addrSwitch, "Unable to determine index variable for indirect jump.");
                    Emit(new ReturnInstruction());
                    blockSource.Procedure.ControlGraph.AddEdge(
                        blockSource, 
                        blockSource.Procedure.ExitBlock);
                }
                else
                {
                    Emit(new SwitchInstruction(swExp, blockCur.Procedure.ControlGraph.Successors(blockCur).ToArray()));
                }
            }
            if (imgVector.Size > 0)
            {
                program.ImageMap.AddItemWithSize(imgVector.Address, imgVector);
            }
            else
            {
                program.ImageMap.AddItem(imgVector.Address, imgVector);
            }
            return true;
        }

        private void ScanVectorTargets(RtlTransfer xfer, List<Address> vector)
        {
            foreach (Address addr in vector)
            {
                var st = state.Clone();
                if (xfer is RtlCall)
                {
                    var pbase = scanner.ScanProcedure(addr, null, st);
                    var pcallee = pbase as Procedure;
                    if (pcallee != null)
                    {
                        program.CallGraph.AddEdge(blockCur.Statements.Last, pcallee);
                    }
                }
                else
                {
                    if (!program.SegmentMap.IsValidAddress(addr))
                        break;
                    BlockFromAddress(ric.Address, addr, blockCur.Procedure, state);
                }
            }
        }

        private void Emit(Instruction instruction)
        {
            blockCur.Statements.Add(ric.Address.ToLinear(), instruction);
        }

        private void Emit(Instruction instruction, Block block)
        {
            block.Statements.Add(ric.Address.ToLinear(), instruction);
        }

        private Block FallthroughBlock(Address addrSrc, Procedure proc, Address fallthruAddress)
        {
            if (ri.NextStatementRequiresLabel)
            {
                // Some machine instructions, like the X86 'rep cmps' 
                // instruction, force the need to generate a label where
                // there wouldn't be one normally, in the middle of the rtl
                // sequence corresponding to the machine instruction.
                return AddIntraStatementBlock(proc);
            }
            else
            {
                return BlockFromAddress(addrSrc, fallthruAddress, proc, state);
            }
        }

        private Block AddIntraStatementBlock(Procedure proc)
        {
            var fallthru = new Block(proc, ric.Address.GenerateName("l", string.Format("_{0}", ++extraLabels)));
            fallthru.IsSynthesized = true;
            proc.ControlGraph.Blocks.Add(fallthru);
            return fallthru;
        }

        /// <summary>
        /// Searches backwards to find a ProcedureConstant that is assigned to the identifier id.
        /// </summary>
        /// <remarks>
        /// This is a sleazy hack since we pay absolutely no attention to register liveness &c. However,
        /// the code is written in the spirit of "innocent until proven guilty". If this turns out to be buggy,
        /// and false positives occur, it will have to be canned and a better solution will have to be invented.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        //$REVIEW: we're effectively doing constant propagation during scanning, why not use that information?
        // Because the scanner constant propagation is doing its propagation by bits (see X86Processorstate)
        // but we want to propagate procedure constants. For the future: change processor state to handle
        // not only numeric constants, but all constants.
        private ProcedureBase SearchBackForProcedureConstant(Identifier id)
        {
            var visited = new HashSet<Block>();
            Block block = blockCur;
            while (block != null && !visited.Contains(block))
            {
                visited.Add(block);
                for (int i = block.Statements.Count - 1; i >= 0; --i)
                {
                    var ass = block.Statements[i].Instruction as Assignment;
                    if (ass == null)
                        continue;
                    var idAss = ass.Dst ;
                    if (idAss != null && idAss == id)
                    {
                        ProcedureConstant pc = ass.Src as ProcedureConstant;
                        if (pc != null)
                        {
                            return pc.Procedure;
                        }
                        var imp = ImportedProcedureName(ass.Src);
                        if (imp != null)
                            return new ExternalProcedure(imp.Name, imp.Signature, imp.Characteristics);
                        else
                            return null;
                    }
                }
                var pred = block.Procedure.ControlGraph.Predecessors(block).ToArray();
                if (pred.Length != 1)
                    return null;
                block = pred[0];
            }
            return null;
        }

        [Conditional("DEBUG")]
        private void DumpCfg()
        {
            foreach (Block block in blockCur.Procedure.ControlGraph.Blocks)
            {
                Console.WriteLine("block: {0}", block.Name);
                Console.Write("\tpred:");
                foreach (var p in block.Procedure.ControlGraph.Predecessors(block))
                {
                    Console.Write(" {0}", p.Name);
                }
                Console.WriteLine();
                Console.Write("\tsucc:");
                foreach (var s in block.Procedure.ControlGraph.Successors(block))
                {
                    Console.Write(" {0}", s.Name);
                }
                Console.WriteLine();
            }
        }

        public ExternalProcedure ImportedProcedureName(Expression callTarget)
        {
            var mem = callTarget as MemoryAccess;
            if (mem == null)
                return null;
            if (mem.EffectiveAddress.DataType.Size != this.program.Platform.PointerType.Size)
                return null;
            Address addrTarget = mem.EffectiveAddress as Address;
            if (addrTarget == null)
            {
                var offset = mem.EffectiveAddress as Constant;
                if (offset == null)
                    return null;
                addrTarget = program.Platform.MakeAddressFromConstant(offset);
            }
            var impEp = scanner.GetImportedProcedure(addrTarget, ric.Address);
            //if (impEp != null)
                return impEp;
            //return scanner.GetInterceptedCall(addrTarget);
        }

        //$TODO: merge the followng two procedures?
        private void AffectProcessorState(FunctionType sig)
        {
            if (sig == null)
                return;
            if (!sig.HasVoidReturn)
                TrashVariable(sig.ReturnValue.Storage);
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                var os = sig.Parameters[i].Storage as OutArgumentStorage;
                if (os != null)
                {
                    TrashVariable(os.OriginalIdentifier.Storage);
                }
            }
        }

        public void TrashVariable(Storage id)
        {
            if (id == null)
                return;
            var reg = id as RegisterStorage;
            if (reg != null)
            {
                state.SetValue(reg, Constant.Invalid);
            }
            SequenceStorage seq = id as SequenceStorage;
            if (seq != null)
            {
                TrashVariable(seq.Head);
                TrashVariable(seq.Tail);
            }
        }

        private SystemService MatchSyscallToService(RtlSideEffect side)
        {
            var fn = side.Expression as Application;
            if (fn == null)
                return null;
            var pc = fn.Procedure as ProcedureConstant;
            if (pc == null)
                return null;
            var ppp = pc.Procedure as PseudoProcedure;
            if (ppp == null)
                return null;
            if (ppp.Name != PseudoProcedure.Syscall || fn.Arguments.Length == 0)
                return null;

            var vector = fn.Arguments[0] as Constant;
            if (vector == null)
                return null;
            var svc = program.Platform.FindService(vector.ToInt32(), state);
            if (svc != null && svc.Signature == null)
            {
                scanner.Error(ric.Address, string.Format("System service '{0}' didn't specify a signature.", svc.Name));
            }
            return svc;
        }

        private class BackwalkerHost : IBackWalkHost<Block, Instruction>
        {
            private IScannerQueue scanner;
            private SegmentMap segmentMap;
            private IPlatform platform;
            private IProcessorArchitecture arch;

            public BackwalkerHost(BlockWorkitem item)
            {
                this.scanner = item.scanner;
                this.segmentMap = item.program.SegmentMap;
                this.arch = item.program.Architecture;
                this.platform = item.program.Platform;
            }

            public Tuple<Expression,Expression> AsAssignment(Instruction instr)
            {
                var ass = instr as Assignment;
                if (ass == null)
                    return null;
                return Tuple.Create((Expression)ass.Dst, ass.Src);
            }

            public Expression AsBranch(Instruction instr)
            {
                var bra = instr as Branch;
                if (bra == null)
                    return null;
                return bra.Condition;
            }

            public bool IsFallthrough(Instruction instr, Block block)
            {
                var bra = instr as Branch;
                if (bra == null)
                    return false;
                return bra.Target != block;
            }

            public AddressRange GetSinglePredecessorAddressRange(Address block)
            {
                throw new NotImplementedException();
            }

            public Address GetBlockStartAddress(Address addr)
            {
                throw new NotImplementedException();
            }

            public Block GetSinglePredecessor(Block block)
            {
                return block.Procedure.ControlGraph.Predecessors(block).FirstOrDefault();
            }

            public RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
            {
                return arch.GetSubregister(reg, offset, width);
            }

            public bool IsStackRegister(Storage stg)
            {
                return stg == arch.StackRegister;
            }

            public bool IsValidAddress(Address addr)
            {
                return segmentMap.IsValidAddress(addr);
            }

            public Address MakeAddressFromConstant(Constant c)
            {
                return platform.MakeAddressFromConstant(c);
            }

            public Address MakeSegmentedAddress(Constant seg, Constant off)
            {
                return arch.MakeSegmentedAddress(seg, off);
            }

            public IEnumerable<Instruction> GetReversedBlockInstructions(Block block)
            {
                return block.Statements.Select(s => s.Instruction).Reverse();
            }
        }
    }
}
