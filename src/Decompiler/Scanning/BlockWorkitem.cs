#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        private readonly IScanner scanner;
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly Address addrStart;
        private Block blockCur;
        private Frame frame;
        private RtlInstruction ri;
        private RtlInstructionCluster ric;
        private IEnumerator<RtlInstructionCluster> rtlStream;
        private readonly ProcessorState state;
        private readonly ExpressionSimplifier eval;
        private int extraLabels;
        private Identifier stackReg;
        private VarargsFormatScanner vaScanner;

        public BlockWorkitem(
            IScanner scanner,
            Program program,
            IProcessorArchitecture arch,
            ProcessorState state,
            Address addr) : base(addr)
        {
            this.scanner = scanner;
            this.program = program;
            this.arch = arch;
            this.state = state;
            this.eval = new ExpressionSimplifier(
                program.SegmentMap,
                state,
                scanner.Services.RequireService<DecompilerEventListener>());
            this.addrStart = addr;
            this.blockCur = null;
        }

        /// <summary>
        /// Processes the statements of a basic block by using the architecture-specific
        /// Rewriter to obtain a stream of low-level RTL instructions. RTL assignments are 
        /// simply added to the instruction list of the basic block. Jumps, returns, and 
        /// calls to procedures that terminate the thread of execution result in the 
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
            rtlStream = scanner.GetTrace(arch, addrStart, state, frame)
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
                    break;
                }
                blNext = FallenThroughNextBlock(addrInstrEnd);
                if (blNext != null)
                {
                    EnsureEdge(blockCur.Procedure, blockCur, blNext);
                    break;
                }
            }
            rtlStream.Dispose();
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
            if (!program.User.RegisterValues.TryGetValue(addr, out var regValues))
                return;
            foreach (var rv in regValues)
            {
                var reg = frame.EnsureRegister(rv.Register);
                new RtlAssignment(reg, rv.Value).Accept(this);
            }
        }

        private Block FallenThroughNextProcedure(Address addrInstr, Address addrTo)
        {
            if (program.Procedures.TryGetValue(addrTo, out var procOther) &&
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

        private Instruction BuildApplication(Expression fn, FunctionType sig, ProcedureCharacteristics c, CallSite site)
        {
            var ab = arch.CreateFrameApplicationBuilder(frame, site, fn);
            return ab.CreateInstruction(sig, c);
        }

        public Expression GetValue(Expression op)
        {
            return op.Accept<Expression>(eval);
        }

        public void SetValue(Expression dst, Expression value)
        {
            switch (dst)
            {
            case Identifier id:
                state.SetValue(id, value);
                return;
            case SegmentedAccess smem:
                state.SetValueEa(smem.BasePointer, GetValue(smem.EffectiveAddress), value);
                return;
            case MemoryAccess mem:
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
            var val = GetValue(a.Src);
            SetValue(a.Dst, val);
            var inst = (a.Dst is Identifier idDst)
                ? new Assignment(idDst, a.Src)
                : (Instruction)new Store(a.Dst, a.Src);
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
            if ((b.Class & InstrClass.Delay) != 0)
            {
                rtlStream.MoveNext();
                ricDelayed = rtlStream.Current;
                ric = ricDelayed;
            }
            var fallthruAddress = ric.Address + ric.Length;

            Block blockThen;
            if (!program.SegmentMap.IsValidAddress((Address)b.Target))
            {
                var label = program.NamingPolicy.BlockName(ric.Address) + "_then";
                blockThen = proc.AddBlock(label);
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

            if ((b.Class & InstrClass.Delay) != 0 &&
                ricDelayed.Instructions.Length > 0)
            {
                // Introduce stubs for the delay slot, but only
                // if the delay slot isn't empty.

                if ((b.Class & InstrClass.Annul) != 0)
                {
                    EnsureEdge(proc, branchingBlock, blockElse);
                }
                else
                {
                    var blockDsF = proc.AddSyntheticBlock(
                        ricDelayed.Address,
                        branchingBlock.Name + "_ds_f");
                    blockCur = blockDsF;
                    ProcessRtlCluster(ricDelayed);
                    EnsureEdge(proc, blockDsF, blockElse);
                    EnsureEdge(proc, branchingBlock, blockDsF);
                }

                var blockDsT = proc.AddSyntheticBlock(
                    ricDelayed.Address,
                    branchingBlock.Name + "_ds_t");
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
            if (BlockHasBeenScanned(blockElse))
            {
                return false;
            }
            else
            {
                blockCur = blockElse;
                return true;
            }
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
        /// <returns>Always returns false, indicating linear control
        /// flow stops here.</returns>
        public bool VisitGoto(RtlGoto g)
        {
            var blockFrom = blockCur;
            if ((g.Class & InstrClass.Delay) != 0)
            {
                // Get next instruction cluster, which should be the delay slot.
                //$TODO: some architectures, curse it, have more than one delay slot...
                rtlStream.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            CallSite site;
            scanner.TerminateBlock(blockCur, rtlStream.Current.Address + ric.Length);
            if (g.Target is Address addrTarget)
            {
                var impProc = scanner.GetImportedProcedure(this.arch, addrTarget, this.ric.Address);
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
                var trampoline = scanner.GetTrampoline(blockCur.Procedure.Architecture, addrTarget);
                if (trampoline != null)
                {
                    var jmpSite = state.OnBeforeCall(stackReg, arch.PointerType.Size);
                    if (trampoline is DispatchProcedure disp)
                    {
                        trampoline = ResolveDispatchProcedureCall(disp, state);
                    }
                    var sig = trampoline.Signature;
                    var chr = trampoline.Characteristics;
                    // Adjust stack to "hide" any pushed return value since
                    // currently Reko treats the return value as an implicit detail
                    // of the calling convention. Had the x86 rewriter explicity
                    // generated code to predecrement the stack pointer
                    // when encountering CALL instructions this would 
                    // not be necessary.
                    if (sig != null && sig.ReturnAddressOnStack != 0)
                    {
                        Emit(new Assignment(stackReg, new BinaryExpression(
                            Operator.IAdd,
                            stackReg.DataType,
                            stackReg,
                            Constant.Word(stackReg.DataType.BitSize, sig.ReturnAddressOnStack))));
                    }
                    EmitCall(CreateProcedureConstant(trampoline), sig, chr, jmpSite);
                    if (sig != null && sig.ReturnAddressOnStack != 0)
                    {
                        //$TODO: make x86 calls' implicit storage explicit
                        // to avoid this hacky dance,
                        Emit(new Assignment(stackReg, new BinaryExpression(
                            Operator.ISub,
                            stackReg.DataType,
                            stackReg,
                            Constant.Word(stackReg.DataType.BitSize, sig.ReturnAddressOnStack))));
                    }
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

                // Always emit goto statements to avoid error during block splitting
                //$REVIEW: we insert a statement into empty blocks to satisfy the BlockHasBeenScanned
                // predicate. This should be done in a better way; perhaps by keeping track
                // of scanned blocks in the Scanner class?
                // The recursive scanning of basic blocks does need improvement;
                // consider using a similar technique to Shingle scanner, where reachable
                // statements are collected first, and basic blocks reconstructed afterwards.

                Emit(new GotoInstruction(addrTarget));
                return false;
            }
            if (g.Target is MemoryAccess mem && mem.EffectiveAddress is Constant)
            {
                // jmp [address]
                site = state.OnBeforeCall(this.stackReg, mem.DataType.Size);
                Emit(new CallInstruction(g.Target, site));
                Emit(new ReturnInstruction());
                blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                return false;
            }
            if (ProcessIndirectControlTransfer(ric.Address, g))
            {
                return false;
            }

            // We've encountered JMP <exp> and we can't determine the limits of <exp>.
            // We emit a call-return pair and call it a day.

            site = state.OnBeforeCall(this.stackReg, g.Target.DataType.Size);
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
            if ((call.Class & InstrClass.Delay) != 0)
            {
                // Get delay slot instruction cluster.
                rtlStream.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            var site = OnBeforeCall(stackReg, call.ReturnAddressSize);
            FunctionType sig;
            ProcedureCharacteristics chr = null;
            Address addr = CallTargetAsAddress(call);
            if (addr != null)
            {
                // Some image loaders generate import symbols at addresses
                // outside of the program image. 
                var impProc = scanner.GetImportedProcedure(this.arch, addr, this.ric.Address);
                if (impProc != null)
                {
                    sig = impProc.Signature;
                    chr = impProc.Characteristics;
                    if (chr != null && chr.IsAlloca)
                    {
                        return ProcessAlloca(site, impProc);
                    }
                    EmitCall(CreateProcedureConstant(impProc), sig, chr, site);
                    return OnAfterCall(sig, chr);
                }

                if (!program.SegmentMap.IsValidAddress(addr))
                {
                    return GenerateCallToOutsideProcedure(site, addr);
                }

                if (InlineCall(addr))
                {
                    return true;
                }

                var arch = call.Architecture ?? blockCur.Procedure.Architecture;
                var callee = scanner.ScanProcedure(arch, addr, null, state);
                if (callee is DispatchProcedure disp)
                {
                    callee = ResolveDispatchProcedureCall(disp, state);
                }
                var pcCallee = CreateProcedureConstant(callee);
                sig = callee.Signature;
                chr = callee.Characteristics;
                EmitCall(pcCallee, sig, chr, site);
                if (callee is Procedure pCallee)
                {
                    program.CallGraph.AddEdge(blockCur.Statements.Last, pCallee);
                }
                return OnAfterCall(sig, chr);
            }

            if (call.Target is ProcedureConstant procCallee)
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
                return OnAfterCall(sig, chr);
            }

            if (call.Target is Identifier id)
            {
                //$REVIEW: this is a hack. Were we in SSA form,
                // we could quickly determine if `id` is assigned
                // to constant.
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


        /// <summary>
        /// If the architecture can inline this call, do so.
        /// </summary>
        /// <param name="addCallee">Address to which the call is made.</param>
        /// <returns>True if the call was successfully inlined, false if not.</returns>
        private bool InlineCall(Address addCallee)
        {
            var rdr = program.CreateImageReader(this.arch, addCallee);
            List<RtlInstruction> inlinedInstructions = arch.InlineCall(addCallee, ric.Address + ric.Length, rdr, frame);
            if (inlinedInstructions == null)
                return false;
            foreach (var instr in inlinedInstructions)
            {
                instr.Accept(this);
            }
            return true;
        }

        private Address CallTargetAsAddress(RtlCall call)
        {
            var callTarget = call.Target.Accept(eval);
            if (callTarget is Constant c)
            {
                if (c.IsValid)
                {
                    return arch.MakeAddressFromConstant(c, true);
                }
                else
                {
                    return null;
                }
            }
            var addr = call.Target as Address;
            if (addr == null)
            {
               addr = program.Platform.ResolveIndirectCall(call);
            }
            return addr;
        }

        private bool GenerateCallToOutsideProcedure(CallSite site, Address addr)
        {
            scanner.Warn(ric.Address, "Call target address {0} is invalid.", addr);
            var sig = new FunctionType();
            ProcedureCharacteristics chr = null;
            var name = NamingPolicy.Instance.ProcedureName(addr);
            var pc = CreateProcedureConstant(new ExternalProcedure(name, sig));
            EmitCall(pc, sig, chr, site);
            return OnAfterCall(sig, chr);
        }

        private void EmitCall(
            Expression callee,
            FunctionType sig,
            ProcedureCharacteristics chr,
            CallSite site)
        {
            if (vaScanner.TryScan(ric.Address, callee, sig, chr))
            {
                Emit(vaScanner.BuildInstruction(callee, site, chr));
            }
            else if (sig != null && sig.ParametersValid)
            {
                Emit(BuildApplication(callee, sig, chr, site));
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
                            PrimitiveType.CreateWord(sizeOfRetAddrOnStack * DataType.BitsPerByte),
                            sizeOfRetAddrOnStack));
                newVal = newVal.Accept(eval);
                SetValue(stackReg, newVal);
            }
            return state.OnBeforeCall(stackReg, sizeOfRetAddrOnStack);
        }

        private bool OnAfterCall(FunctionType sigCallee, ProcedureCharacteristics characteristics)
        {
            if (program.User.Calls.TryGetUpperBound(ric.Address, out var userCall))
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

            if (sigCallee != null && sigCallee.StackDelta != 0)
            {
                // Generate explicit stack adjustment expression
                // SP = SP + stackDelta
                // after the call.
                Expression newVal = new BinaryExpression(
                    Operator.IAdd,
                    stackReg.DataType,
                    stackReg,
                    Constant.Create(
                        PrimitiveType.CreateWord(stackReg.DataType.BitSize),
                        sigCallee.StackDelta));
                newVal = newVal.Accept(eval);
                SetValue(stackReg, newVal);
            }
            state.OnAfterCall(sigCallee);

            // Adjust stack after call 
            //$REVIEW: looks like common code; consider refactoring this.
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
                if (sigCallee.FpuStackDelta != 0)
                {
                    BinaryOperator op;
                    int dd = sigCallee.FpuStackDelta;
                    if (sigCallee.FpuStackDelta > 0)
                    {
                        op = Operator.ISub;
                    }
                    else
                    {
                        op = Operator.IAdd;
                        dd = -sigCallee.FpuStackDelta;
                    }
                    var fpuStackReg = frame.EnsureRegister(arch.FpuStackRegister);
                    var d = Constant.Create(fpuStackReg.DataType, dd);
                    this.Emit(new Assignment(
                        fpuStackReg,
                        new BinaryExpression(op, fpuStackReg.DataType, fpuStackReg, d)));
                }
            }
            TrashRegistersAfterCall();
            return true;
        }

        private void TrashRegistersAfterCall()
        {
            foreach (var reg in program.Platform.CreateTrashedRegisters())
            {
                // $REVIEW: do not trash stack register. It gives regression
                // on some MSDOS binaries
                if (reg != arch.StackRegister)
                    state.SetValue(reg, Constant.Invalid);
            }
        }

        private FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
        {
            if (!program.User.Calls.TryGetValue(addrCallInstruction, out var call))
                return null;
            return call.Signature;
        }

        public bool VisitReturn(RtlReturn ret)
        {
            if ((ret.Class & InstrClass.Delay) != 0)
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
                if (side.Expression is Application appl &&
                    appl.Procedure is ProcedureConstant fn &&
                    fn.Procedure.Characteristics.Terminates)
                {
                    scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                    return false;
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
                Emit(BuildApplication(fn, ep.Signature, ep.Characteristics, site));
                if (svc.Characteristics.Terminates)
                {
                    scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                    //blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return true;
                }
                AffectProcessorState(svc.Signature);
                OnAfterCall(svc.Signature, svc.Characteristics);
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
            var ab = arch.CreateFrameApplicationBuilder(
                frame,
                site,
                new ProcedureConstant(program.Platform.PointerType, impProc));
            if (impProc.Signature.Parameters.Length != 1)
                throw new ApplicationException(string.Format("An alloca function must have exactly one parameter, but {0} has {1}.", impProc.Name, impProc.Signature.Parameters.Length));
            var target = ab.Bind(impProc.Signature.Parameters[0]);
            if (!(target is Identifier id))
                throw new ApplicationException(string.Format("The parameter of {0} wasn't a register.", impProc.Name));
            if (state.GetValue(id) is Constant c && c.IsValid)
            {
                Emit(new Assignment(stackReg, new BinaryExpression(Operator.ISub, stackReg.DataType, stackReg, c)));
            }
            else
            {
                Emit(ab.CreateInstruction(impProc.Signature, impProc.Characteristics));
            }
            return true;
        }

#endregion

        /// <summary>
        /// Attempts to trace an indirect control transfer. If successful, emits a 
        /// switch RTL instruction (for jumps) or an indirect call instruction (for 
        /// calls).
        /// </summary>
        /// <param name="addrSwitch"></param>
        /// <param name="xfer"></param>
        /// <returns>True if an instruction was emitted, false if not.</returns>
        public bool ProcessIndirectControlTransfer(Address addrSwitch, RtlTransfer xfer)
        {
            List<Address> vector;
            ImageMapVectorTable imgVector;
            Expression switchExp;

            if (program.User.IndirectJumps.TryGetValue(addrSwitch, out var indJump))
            {
                // Trust the user knows what they're doing.
                vector = indJump.Table.Addresses;
                switchExp = this.frame.EnsureIdentifier(indJump.IndexRegister);
                imgVector = indJump.Table;
            }
            else
            {
                if (!DiscoverTableExtent(addrSwitch, xfer, out vector, out imgVector, out switchExp))
                    return false;
            }

            if (xfer is RtlCall)
            {
                ScanCallVectorTargets(vector);
            }
            else
            {
                var jumpDests = ScanJumpVectorTargets(vector);
                var blockSource = scanner.FindContainingBlock(ric.Address);
                blockCur = blockSource;
                foreach (var dest in jumpDests)
                {
                    blockSource.Procedure.ControlGraph.AddEdge(blockSource, dest);
                }
              
                if (switchExp == null)
                {
                    scanner.Warn(addrSwitch, "Unable to determine index variable for indirect jump.");
                    return false;
                }
                var sw = new SwitchInstruction(switchExp, jumpDests.ToArray());
                Emit(sw);
            }
            if (imgVector.Address != null)
            {
                if (imgVector.Size > 0)
                {
                    program.ImageMap.AddItemWithSize(imgVector.Address, imgVector);
                }
                else
                {
                    program.ImageMap.AddItem(imgVector.Address, imgVector);
                }
            }
            return true;
        }

        /// <summary>
        /// Discovers the extent of a jump/call table by walking backwards from the 
        /// jump/call until some gating condition (index < value, index & bitmask etc)
        /// can be found.
        /// </summary>
        /// <param name="addrSwitch">Address of the indirect transfer instruction</param>
        /// <param name="xfer">Expression that computes the transfer destination.
        /// It is never a constant value</param>
        /// <param name="vector">If successful, returns the list of addresses
        /// jumped/called to</param>
        /// <param name="imgVector"></param>
        /// <param name="switchExp">The expression to use in the resulting switch / call.</param>
        /// <returns></returns>
        private bool DiscoverTableExtent(
            Address addrSwitch,
            RtlTransfer xfer,
            out List<Address> vector,
            out ImageMapVectorTable imgVector,
            out Expression switchExp)
        {
            Debug.Assert(!(xfer.Target is Address || xfer.Target is Constant), $"This should not be a constant {xfer}.");
            var listener = scanner.Services.RequireService<DecompilerEventListener>();
            vector = null;
            imgVector = null;
            switchExp = null;

            var bwsHost = new BackwardSlicerHost(program, this.arch);
            var rtlBlock = bwsHost.GetRtlBlock(blockCur);
            var bws = new BackwardSlicer(bwsHost, rtlBlock, state);
            var te = bws.DiscoverTableExtent(addrSwitch, xfer, listener);
            if (te == null)
                return false;
            foreach (var de in te.Accesses)
            {
                var item = new ImageMapItem((uint)de.Value.Size)
                {
                    Address = de.Key,
                    DataType = de.Value
                };
                program.ImageMap.AddItemWithSize(de.Key, item);
            }
            imgVector = new ImageMapVectorTable(
                null, // bw.VectorAddress,
                te.Targets.ToArray(),
                4); // builder.TableByteSize);
            vector = te.Targets;
            switchExp = te.Index;
            return true;
        }


        private void ScanCallVectorTargets(List<Address> vector)
        {
            foreach (Address addr in vector)
            {
                if (!program.SegmentMap.IsValidAddress(addr))
                    continue;
                var st = state.Clone();
                var pbase = scanner.ScanProcedure(blockCur.Procedure.Architecture, addr, null, st);
                if (pbase is Procedure pcallee)
                {
                    program.CallGraph.AddEdge(blockCur.Statements.Last, pcallee);
                }
            }
        }

        private List<Block> ScanJumpVectorTargets(List<Address> vector)
        {
            var blocks = new List<Block>();
            foreach (Address addr in vector)
            {
                if (!program.SegmentMap.IsValidAddress(addr))
                    break;
                var st = state.Clone();
                blocks.Add(BlockFromAddress(ric.Address, addr, blockCur.Procedure, state));
            }
            return blocks;
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
            ++extraLabels;
            var label = program.NamingPolicy.BlockName(ric.Address);
            return proc.AddSyntheticBlock(ric.Address, $"{label}_{extraLabels}");
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
                    if (!(block.Statements[i].Instruction is Assignment ass))
                        continue;
                    var idAss = ass.Dst ;
                    if (idAss != null && idAss == id)
                    {
                        if (ass.Src is ProcedureConstant pc)
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
            if (!(callTarget is MemoryAccess mem))
                return null;
            if (mem.EffectiveAddress.DataType.Size != this.program.Platform.PointerType.Size)
                return null;
            Address addrTarget = mem.EffectiveAddress as Address;
            if (addrTarget == null)
            {
                if (!(mem.EffectiveAddress is Constant offset))
                    return null;
                addrTarget = program.Platform.MakeAddressFromConstant(offset, true);
            }
            var impEp = scanner.GetImportedProcedure(this.arch, addrTarget, ric.Address);
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
                if (sig.Parameters[i].Storage is OutArgumentStorage os)
                {
                    TrashVariable(os.OriginalIdentifier.Storage);
                }
            }
        }

        public void TrashVariable(Storage stg)
        {
            if (stg == null)
                return;
            switch (stg)
            {
            case RegisterStorage reg:
                state.SetValue(reg, Constant.Invalid);
                break;
            case SequenceStorage seq:
                foreach (var e in seq.Elements)
                {
                    TrashVariable(e);
                }
                break;
            }
        }

        /// <summary>
        /// Attempt to resolve call to to dispatch procedure
        /// into one of its sub-services. We do this by looking at 
        /// the procedure state at the time of the call. If we can't 
        /// resolve it, fall back on the dispatch procedure 
        /// directly.
        /// </summary>
        private ProcedureBase ResolveDispatchProcedureCall(DispatchProcedure disp, ProcessorState state)
        {
            var callable = disp.FindService(state);
            if (callable is null)
                return disp;
            else
                return callable;
        }

        private SystemService MatchSyscallToService(RtlSideEffect side)
        {
            if (!(side.Expression is Application fn))
                return null;
            if (!(fn.Procedure is ProcedureConstant pc))
                return null;
            if (!(pc.Procedure is PseudoProcedure ppp))
                return null;
            if (ppp.Name != PseudoProcedure.Syscall || fn.Arguments.Length == 0)
                return null;

            if (!(fn.Arguments[0] is Constant vector))
                return null;
            var svc = program.Platform.FindService(vector.ToInt32(), state);
            //$TODO if SVC uis null (and not-speculating) report the error.
            if (svc != null && svc.Signature == null)
            {
                scanner.Error(ric.Address, string.Format("System service '{0}' didn't specify a signature.", svc.Name));
            }
            return svc;
        }

        public override string ToString()
        {
            return $"{nameof(BlockWorkitem)}: {base.Address}";
        }
    }
}
