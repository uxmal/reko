#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcedureCharacteristics = Reko.Core.Serialization.ProcedureCharacteristics;

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
        private readonly IScannerServices scanner;
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly Address addrStart;
        private Block? blockCur;
        private Frame frame;
        private RtlInstruction? ri;
        private RtlInstructionCluster? ric;
        private IEnumerator<RtlInstructionCluster>? rtlStream;
        private readonly ProcessorState state;
        private readonly ExpressionSimplifier eval;
        private int extraLabels;
        private Identifier? stackReg;
        private VarargsFormatScanner? vaScanner;
        private readonly InstrClass rejectMask;

        public BlockWorkitem(
            IScannerServices scanner,
            Program program,
            IProcessorArchitecture arch,
            ProcessorState state,
            Address addr) : base(addr)
        {
            this.scanner = scanner;
            this.program = program;
            this.arch = arch;
            this.state = state;
            this.frame = default!;
            this.eval = new ExpressionSimplifier(
                program.Memory,
                state,
                scanner.Services.RequireService<IDecompilerEventListener>());
            this.addrStart = addr;
            this.blockCur = null;
            this.rejectMask = program.User.Heuristics.Contains(ScannerHeuristics.Unlikely)
                ? InstrClass.Unlikely
                : 0;
            this.rejectMask |= program.User.Heuristics.Contains(ScannerHeuristics.UserMode)
                ? InstrClass.Privileged
                : 0;
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
            state.ErrorListener = (message) => { scanner.Warn(ric!.Address, message); };
            blockCur = scanner.FindContainingBlock(addrStart);
            if (blockCur is null || BlockHasBeenScanned(blockCur))
                return;

            frame = blockCur.Procedure.Frame;
            this.stackReg = frame.EnsureRegister(arch.StackRegister);
            var pluginSvc = scanner.Services.RequireService<IPluginLoaderService>();
            var listener = scanner.Services.RequireService<IEventListener>();
            this.vaScanner = new VarargsFormatScanner(program, arch, state, scanner.Services, listener);
            rtlStream = scanner.GetTrace(arch, addrStart, state, frame)
                .GetEnumerator();

            while (rtlStream.MoveNext())
            {
                this.ric = rtlStream.Current;
                if (!blockCur.IsSynthesized && blockCur != scanner.FindContainingBlock(ric.Address))
                    break;  // Fell off the end of this block.
                if (program.User.Patches.TryGetValue(ric.Address, out var patch))
                {
                    Debug.Print("BWI: Applying patch at address {0}", ric.Address);
                    ric = patch.Code;
                }
                if (!ProcessRtlCluster(ric))
                    break;
                var addrInstrEnd = ric.Address + ric.Length;
                var blNext = FallenThroughNextProcedure(ric.Address, addrInstrEnd);
                if (blNext is not null)
                {
                    EnsureEdge(blockCur.Procedure, blockCur, blNext);
                    break;
                }
                blNext = FallenThroughNextBlock(addrInstrEnd);
                if (blNext is not null)
                {
                    EnsureEdge(blockCur.Procedure, blockCur, blNext);
                    break;
                }
            }
            rtlStream.Dispose();
        }

        private bool ProcessRtlCluster(RtlInstructionCluster ric)
        {
            state.InstructionPointer = ric.Address;
            SetAssumedRegisterValues(ric.Address);
            if ((ric.Class & this.rejectMask) != 0)
                return false;
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
                if (rv.Register is not null && rv.Value is not null)
                {
                    var reg = frame!.EnsureIdentifier(rv.Register);
                    new RtlAssignment(reg, rv.Value).Accept(this);
                }
            }
        }

        private Block? FallenThroughNextProcedure(Address addrInstr, Address addrTo)
        {
            if (program.Procedures.TryGetValue(addrTo, out var procOther) &&
                procOther != blockCur!.Procedure)
            {
                // Fell into another procedure. 
                var block = scanner.CreateCallRetThunk(addrInstr, blockCur.Procedure, state, procOther);
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
        private Block? FallenThroughNextBlock(Address addr)
        {
            var cont = scanner.FindContainingBlock(addr);
            if (cont is null || cont == blockCur)
                return null;
            return BlockFromAddress(ric!.Address, addr, blockCur!.Procedure, state);
        }

        private static bool BlockHasBeenScanned(Block block)
        {
            return block.Statements.Count > 0;
        }

        public Expression GetValue(Expression op)
        {
            var (e, _) = op.Accept(eval);
            return e;
        }

        public void SetValue(Expression dst, Expression value)
        {
            switch (dst)
            {
            case Identifier id:
                state.SetValue(id, value);
                return;
            case MemoryAccess mem:
                if (mem.EffectiveAddress is SegmentedPointer segptr)
                {
                    state.SetValueEa(segptr.BasePointer, GetValue(segptr.Offset), value);
                }
                else
                {
                    state.SetValueEa(GetValue(mem.EffectiveAddress), value);
                }
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
            var branch = new Branch(b.Condition, new Block(blockCur!.Procedure, blockCur.Address, "TMP!"));
            Emit(branch, blockCur);

            // The following statements may chop up the blockCur, so hang on to the essentials.
            var proc = blockCur.Procedure;
            RtlInstructionCluster? ricDelayed = null;
            if ((b.Class & InstrClass.Delay) != 0)
            {
                rtlStream!.MoveNext();
                ricDelayed = rtlStream.Current;
                ric = ricDelayed;
            }
            var fallthruAddress = ric!.Address + ric.Length;
            if (!blockCur.IsSynthesized)
                scanner.TerminateBlock(blockCur, fallthruAddress);

            Block blockThen;
            if (!program.Memory.IsValidAddress((Address) b.Target))
            {
                var label = program.NamingPolicy.BlockName(ric.Address) + "_then";
                blockThen = proc.AddBlock((Address) b.Target, label);
                if (program.User.BlockLabels.TryGetValue(label, out var userLabel))
                    blockThen.UserLabel = userLabel;
                var jmpSite = state.OnBeforeCall(stackReg!, arch.PointerType.Size);
                GenerateCallToOutsideProcedure(jmpSite, (Address) b.Target);
                Emit(new ReturnInstruction());
                blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
            }
            else
            {
                blockThen = BlockFromAddress(ric.Address, (Address) b.Target, proc, state.Clone());
            }

            var blockElse = FallthroughBlock(ric.Address, proc, fallthruAddress);
            var branchingBlock = blockCur.IsSynthesized
                ? blockCur
                : scanner.FindContainingBlock(ric.Address)!;

            if (HasNonEmptyDelaySlot(b, ricDelayed))
            {
                // Introduce stubs for the delay slot, but only
                // if the delay slot isn't empty.

                var delaySlotName = program.NamingPolicy.BlockName(ricDelayed!.Address);
                if (b.Class.HasFlag(InstrClass.Annul))
                {
                    EnsureEdge(proc, branchingBlock, blockElse);
                }
                else
                {
                    AddSyntheticDelayBlock(proc, delaySlotName + "_ds_f", ricDelayed!, blockElse, branchingBlock);
                }
                blockThen = AddSyntheticDelayBlock(proc, delaySlotName + "_ds_t", ricDelayed!, blockThen, branchingBlock);
            }
            else
            {
                EnsureEdge(proc, branchingBlock, blockElse);
                if (blockElse != blockThen)
                    EnsureEdge(proc, branchingBlock, blockThen);
                else
                    proc.ControlGraph.AddEdge(branchingBlock, blockThen);
            }
            branch.Target = blockThen;      // The back-patch referred to above.

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

        private Block AddSyntheticDelayBlock(Procedure proc, string name, RtlInstructionCluster ricDelayed, Block blockSucc, Block blockPred)
        {
            var blockDelay = proc.AddSyntheticBlock(
                ricDelayed.Address,
                name);
            blockCur = blockDelay;
            ProcessRtlCluster(ricDelayed);
            EnsureEdge(proc, blockDelay, blockSucc);
            EnsureEdge(proc, blockPred, blockDelay);
            return blockDelay;
        }

        private static bool HasNonEmptyDelaySlot(RtlBranch b, RtlInstructionCluster? ricDelayed)
        {
            if (!b.Class.HasFlag(InstrClass.Delay))
                return false;
            if (ricDelayed!.Instructions.Length <= 0)
                return false;
            return ricDelayed.Instructions[0] is not RtlNop;
        }

        /// <summary>
        /// Conditional instructions basic blocks to host them.
        /// </summary>
        /// <param name="rtlIf"></param>
        /// <returns></returns>
        public bool VisitIf(RtlIf rtlIf)
        {
            var branch = new Branch(rtlIf.Condition.Invert(), null!);
            Emit(branch);

            var proc = blockCur?.Procedure!;
            var fallthruAddress = ric!.Address + ric.Length;

            var blockInstr = AddIntraStatementBlock(proc);
            var blockFollow = BlockFromAddress(ric.Address, fallthruAddress, proc, state);

            blockCur = blockInstr;
            rtlIf.Instruction.Accept(this);

            var branchingBlock = scanner.FindContainingBlock(ric.Address)!;
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
            return scanner.EnqueueJumpTarget(addrSrc, addrDst, proc, state)!;
        }

        private static void EnsureEdge(Procedure proc, Block blockFrom, Block blockTo)
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
                rtlStream!.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            CallSite site;
            scanner.TerminateBlock(blockCur!, rtlStream!.Current.Address + ric!.Length);
            if (g.Target is Address addrTarget)
            {
                var impProc = scanner.GetImportedProcedure(this.arch, addrTarget, this.ric.Address);
                if (impProc is not null)
                {
                    // Since we are tail jumping to another procedure, if there is a return
                    // address (continuation) pushed on the stack, we reuse that 
                    // continuation.
                    site = state.OnBeforeCall(stackReg!, 0);
                    var sig = impProc.Signature;
                    var chr = impProc.Characteristics;
                    if (ProcessAlloca(site, impProc, chr))
                        return true;
                    EmitCall(CreateProcedureConstant(impProc), sig, chr, site);
                    Emit(new ReturnInstruction());
                    blockCur!.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                var platformProc = program.Platform.LookupProcedureByAddress(addrTarget);
                if (platformProc is not null)
                {
                    site = state.OnBeforeCall(stackReg!, 0);
                    EmitCall(CreateProcedureConstant(platformProc), platformProc.Signature, platformProc.Characteristics, site);
                    Emit(new ReturnInstruction());
                    blockCur!.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }

                if (!program.Memory.IsValidAddress(addrTarget))
                {
                    var jmpSite = state.OnBeforeCall(stackReg!, 0);
                    GenerateCallToOutsideProcedure(jmpSite, addrTarget);
                    Emit(new ReturnInstruction());
                    blockCur!.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                if (program.Memory.IsExecutableAddress(addrTarget))
                {
                    var trampoline = scanner.GetTrampoline(blockCur!.Procedure.Architecture, addrTarget);
                    if (trampoline is not null)
                    {
                        var jmpSite = state.OnBeforeCall(stackReg!, 0);
                        trampoline = ResolveDispatchProcedureCall(trampoline, state);
                        var sig = trampoline.Signature;
                        var chr = trampoline.Characteristics;
                        EmitCall(CreateProcedureConstant(trampoline), sig, chr, jmpSite);
                        Emit(new ReturnInstruction());
                        blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                        return false;
                    }
                    var blockTarget = BlockFromAddress(ric.Address, addrTarget, blockCur.Procedure, state);
                    var blockSource = blockCur.IsSynthesized
                        ? blockCur
                        : scanner.FindContainingBlock(ric.Address)!;
                    EnsureEdge(blockSource.Procedure, blockSource, blockTarget);
                    if (ric.Address == addrTarget)
                    {
                        var bt = BlockFromAddress(ric.Address, addrTarget, blockCur.Procedure, state);
                        EnsureEdge(blockSource.Procedure, blockFrom!, bt);
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
            }
            else
            {
                if (g.Target is MemoryAccess mem && mem.EffectiveAddress is Constant)
                {
                    // jmp [address]
                    site = state.OnBeforeCall(this.stackReg!, 0);
                    Emit(new CallInstruction(g.Target, site));
                    Emit(new ReturnInstruction());
                    blockCur!.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                if (ProcessIndirectControlTransfer(ric.Address, g))
                {
                    return false;
                }
            }

            // We've encountered JMP <exp> and we can't determine the limits of <exp>.
            // We emit a call-return pair and call it a day.

            site = state.OnBeforeCall(this.stackReg!, 0);
            Emit(new CallInstruction(g.Target, site));
            Emit(new ReturnInstruction());
            blockCur!.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
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
                rtlStream!.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            var site = OnBeforeCall(stackReg!, call.ReturnAddressSize);
            FunctionType? sig;
            ProcedureCharacteristics? chr = null;
            Address? a = CallTargetAsAddress(call);
            if (a is not null)
            {
                Address addr = a.Value;
                // Some image loaders generate import symbols at addresses
                // outside of the program image. 
                var impProc = scanner.GetImportedProcedure(this.arch, addr, this.ric!.Address);
                if (impProc is not null)
                {
                    sig = impProc.Signature;
                    chr = impProc.Characteristics;
                    if (ProcessAlloca(site, impProc, chr))
                        return true;
                    EmitCall(CreateProcedureConstant(impProc), sig, chr, site);
                    return OnAfterCall(sig, chr);
                }

                var platformProc = program.Platform.LookupProcedureByAddress(addr);
                if (platformProc is not null)
                {
                    platformProc = ResolveDispatchProcedureCall(platformProc, state);
                    EmitCall(CreateProcedureConstant(platformProc), platformProc.Signature, platformProc.Characteristics, site);
                    return OnAfterCall(platformProc.Signature, chr);
                }
                if (!program.Memory.IsValidAddress(addr))
                {
                    return GenerateCallToOutsideProcedure(site, addr);
                }

                if (InlineCall(addr))
                {
                    return true;
                }
                if (program.Procedures.TryGetValue(addr, out var maybeAlloca) && 
                    maybeAlloca.Characteristics.IsAlloca)
                {
                    if (ProcessAlloca(site, maybeAlloca, maybeAlloca.Characteristics))
                        return true;
                }

                var arch = call.Architecture ?? blockCur!.Procedure.Architecture;
                var callee = scanner.ScanProcedure(arch, addr, null, state);
                callee = ResolveDispatchProcedureCall(callee, state);
                var pcCallee = CreateProcedureConstant(callee);
                sig = callee.Signature;
                chr = callee.Characteristics;
                EmitCall(pcCallee, sig, chr, site);
                program.CallGraph.AddEdge(blockCur!.Statements[^1], callee);
                return OnAfterCall(sig, chr);
            }

            if (call.Target is ProcedureConstant procCallee)
            {
                sig = procCallee.Signature;
                chr = procCallee.Procedure.Characteristics;
                EmitCall(procCallee, sig, chr, site);
                return OnAfterCall(sig, chr);
            }
            sig = GetCallSignatureAtAddress(ric!.Address);
            if (sig is not null)
            {
                EmitCall(call.Target, sig, chr, site);
                return OnAfterCall(sig, chr);
            }

            if (call.Target is Identifier id)
            {
                //$REVIEW: this is a hack. Were we in SSA form,
                // we could quickly determine if `id` is assigned
                // to constant.
                var intrinsic = SearchBackForProcedureConstant(id);
                if (intrinsic is not null)
                {
                    var e = CreateProcedureConstant(intrinsic);
                    sig = intrinsic.Signature;
                    chr = intrinsic.Characteristics;
                    EmitCall(e, sig, chr, site);
                    return OnAfterCall(sig, chr);
                }
            }

            var imp = ImportedProcedureName(call.Target);
            if (imp is not null)
            {
                sig = imp.Signature;
                chr = imp.Characteristics;
                EmitCall(CreateProcedureConstant(imp), sig, chr, site);
                return OnAfterCall(sig, chr);
            }

            var syscall = program.Platform.FindService(call, state, program.Memory);
            if (syscall is not null)
            {
                return EmitSystemServiceCall(syscall);
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
            if (!program.TryCreateImageReader(this.arch, addCallee, out var rdr))
                return false;
            List<RtlInstruction>? inlinedInstructions = arch.InlineCall(addCallee, ric!.Address + ric.Length, rdr, frame!);
            if (inlinedInstructions is null)
                return false;
            foreach (var instr in inlinedInstructions)
            {
                instr.Accept(this);
            }
            return true;
        }

        private Address? CallTargetAsAddress(RtlCall call)
        {
            var (callTarget, _) = call.Target.Accept(eval);

            if (call.Target is Address addr)
                return addr;

            if (callTarget is Constant c)
            {
                if (!c.IsValid)
                    return null;
                return arch.MakeAddressFromConstant(c, true);
            }
            return program.Platform.ResolveIndirectCall(call);
        }

        private bool GenerateCallToOutsideProcedure(CallSite site, Address addr)
        {
            scanner.Warn(ric!.Address, "Call target address {0} is invalid.", addr);
            var sig = new FunctionType();
            ProcedureCharacteristics? chr = null;
            var name = NamingPolicy.Instance.ProcedureName(addr);
            var pc = CreateProcedureConstant(new ExternalProcedure(name, sig));
            EmitCall(pc, sig, chr, site);
            return OnAfterCall(sig, chr);
        }

        private void EmitCall(
            Expression callee,
            FunctionType sig,
            ProcedureCharacteristics? chr,
            CallSite site)
        {
            var ab = arch.CreateFrameApplicationBuilder(frame!, site);
            if (VarargsFormatScanner.IsVariadicParserKnown(sig, chr))
            {
                if (vaScanner!.TryScan(ric!.Address, callee, sig, chr, ab, out var varargs))
                {
                    Emit(vaScanner.BuildInstruction(callee, sig, varargs.Signature, chr, ab));
                }
                else
                {
                    // We're unable to create the varargs application at this point,
                    // try later during the SSA and ValuePropagation pass.
                    Emit(new CallInstruction(callee, site));
                }
            }
            else if (sig is not null && sig.ParametersValid)
            {
                Emit(ab.CreateInstruction(callee, sig, chr));
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
                            PrimitiveType.Create(Domain.SignedInt, stackReg.DataType.BitSize),
                            sizeOfRetAddrOnStack));
                (newVal, _) = newVal.Accept(eval);
                SetValue(stackReg, newVal);
            }
            return state.OnBeforeCall(stackReg, sizeOfRetAddrOnStack);
        }

        private bool OnAfterCall(FunctionType sigCallee, ProcedureCharacteristics? characteristics)
        {
            if (program.User.Calls.TryGetUpperBound(ric!.Address, out var userCall))
            {
                var linStart = ric.Address.ToLinear();
                var linEnd = linStart + (uint)ric.Length;
                var linUserCall = userCall.Address!.ToLinear();
                if (linStart > linUserCall || linUserCall >= linEnd)
                    userCall = null!;
            }
            if ((characteristics is not null && characteristics.Terminates) ||
                (userCall is not null && userCall.NoReturn))
            {
                scanner.TerminateBlock(blockCur!, ric.Address + ric.Length);
                return false;
            }

            if (sigCallee is not null && sigCallee.StackDelta != 0)
            {
                // Generate explicit stack adjustment expression
                // SP = SP + stackDelta
                // after the call.
                Expression newVal = new BinaryExpression(
                    Operator.IAdd,
                    stackReg!.DataType,
                    stackReg,
                    Constant.Create(
                        PrimitiveType.CreateWord(stackReg.DataType.BitSize),
                        sigCallee.StackDelta));
                (newVal, _) = newVal.Accept(eval);
                SetValue(stackReg, newVal);
            }
            state.OnAfterCall(sigCallee);

            // Adjust stack after call 
            //$REVIEW: looks like common code; consider refactoring this.
            if (sigCallee is not null)
            {
                int delta = sigCallee.StackDelta - sigCallee.ReturnAddressOnStack;
                if (delta != 0)
                {
                    var d = Constant.Create(stackReg!.DataType, delta);
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
                    var fpuStackReg = frame!.EnsureRegister(arch.FpuStackRegister!);
                    var d = Constant.Create(fpuStackReg.DataType, dd);
                    this.Emit(new Assignment(
                        fpuStackReg,
                        new BinaryExpression(op, fpuStackReg.DataType, fpuStackReg, d)));
                }
            }

            TrashRegistersAfterCall();
            if (characteristics is not null && characteristics.ReturnAddressAdjustment > 0)
            {
                var addrNext = ric.Address + characteristics.ReturnAddressAdjustment;
                scanner.TerminateBlock(blockCur!, addrNext);
                var blockNext = scanner.EnqueueJumpTarget(ric.Address, addrNext, blockCur!.Procedure!, state)!;
                Emit(new GotoInstruction(addrNext));
                blockCur?.Procedure.ControlGraph.AddEdge(blockCur, blockNext);
                return false;
            }
            return true;
        }

        private void TrashRegistersAfterCall()
        {
            foreach (var reg in program.Platform.TrashedRegisters)
            {
                // $REVIEW: do not trash stack register. It gives regression
                // on some MSDOS binaries
                if (reg != arch.StackRegister)
                    state.SetValue(reg, InvalidConstant.Create(reg.DataType));
            }
        }

        private FunctionType? GetCallSignatureAtAddress(Address addrCallInstruction)
        {
            if (!program.User.Calls.TryGetValue(addrCallInstruction, out var call))
                return null;
            return call.Signature;
        }

        public bool VisitMicroGoto(RtlMicroGoto uGoto)
        {
            throw new NotImplementedException();
        }

        public bool VisitReturn(RtlReturn ret)
        {
            if ((ret.Class & InstrClass.Delay) != 0)
            {
                // Get next instruction cluster from the delay slot.
                rtlStream!.MoveNext();
                ProcessRtlCluster(rtlStream.Current);
            }
            var proc = blockCur!.Procedure;
            Emit(new ReturnInstruction());
            proc.ControlGraph.AddEdge(blockCur, proc.ExitBlock);

            int returnAddressBytes = ret.ReturnAddressBytes;
            var address = ric!.Address;
            scanner.SetProcedureReturnAddressBytes(proc, returnAddressBytes, address);
            int stackDelta = ret.ReturnAddressBytes + ret.ExtraBytesPopped;
            scanner.SetProcedureStackDelta(proc, stackDelta, address);
            state.OnProcedureLeft(proc.Signature);
            scanner.TerminateBlock(blockCur, rtlStream!.Current.Address + ric.Length);
            return false;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            var svc = MatchSyscallToService(side);
            if (svc is not null)
            {
                return EmitSystemServiceCall(svc);
            }
            else
            {
                Emit(new SideEffect(side.Expression));
                if (side.Expression is Application appl &&
                    appl.Procedure is ProcedureConstant fn &&
                    fn.Procedure.Characteristics.Terminates)
                {
                    scanner.TerminateBlock(blockCur!, ric!.Address + ric.Length);
                    return false;
                }
            }
            return true;
        }

        public bool VisitSwitch(RtlSwitch rtlSwitch)
        {
            return false;
        }
        /// <summary>
        /// Takes a system service description and generates a system call from it.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True if the system service does not terminate, false if it does
        /// and scanning should stop.</returns>
        private bool EmitSystemServiceCall(SystemService svc)
        {
            var ep = svc.CreateExternalProcedure(arch);
            var fn = new ProcedureConstant(program.Platform.PointerType, ep);
            if (svc.Signature is not null)
            {
                var site = state.OnBeforeCall(stackReg!, svc.Signature.ReturnAddressOnStack);
                var ab = arch.CreateFrameApplicationBuilder(frame!, site);
                Emit(ab.CreateInstruction(fn, ep.Signature, ep.Characteristics));
                if (svc.Characteristics is not null && svc.Characteristics.Terminates)
                {
                    scanner.TerminateBlock(blockCur!, ric!.Address + ric.Length);
                    return false;
                }
                AffectProcessorState(svc.Signature);
                return OnAfterCall(svc.Signature, svc.Characteristics);
            }
            else
            {
                var site = state.OnBeforeCall(stackReg!, 0);
                Emit(new CallInstruction(fn, site));
            }
            return true;
        }

        private static FunctionType GuessProcedureSignature(CallInstruction call)
        {
            return new FunctionType(); //$TODO: attempt to detect parameters of procedure?
            // This would have to be arch-dependent + platform-dependent as some arch pass
            // on stack, while others pass in registers, or a combination or both
            // ("thiscall" in x86 µsoft world).
        }

        public bool ProcessAlloca(CallSite site, ProcedureBase impProc, ProcedureCharacteristics chr)
        {
            if (chr is null || !chr.IsAlloca)
                return false;
            if (impProc.Signature is null || !impProc.Signature.ParametersValid)
            {
                this.scanner.Warn(ric!.Address, $"You must specify a procedure signature for {impProc.Name} since it has been marked as 'alloca'.");
                return false;
            }
            var sig = impProc.Signature;
            if (sig.Parameters!.Length != 1)
            {
                this.scanner.Warn(ric!.Address, $"An alloca function must have exactly one parameter, but {impProc.Name} has {sig.Parameters.Length}.");
            }
            var callee = new ProcedureConstant(program.Platform.PointerType, impProc);
            var ab = arch.CreateFrameApplicationBuilder(
                frame!,
                site);
            var target = ab.BindInArg(sig.Parameters[0].Storage);
            if (target is not Identifier id)
            {
                this.scanner.Warn(ric!.Address, $"The parameter of {impProc.Name} wasn't a register.");
                return false;
            }
            if (state.GetValue(id) is Constant c && c.IsValid)
            {
                // Replace constant call to alloca with a stack adjustment
                //$TODO: should grow to higher addresses on PA-RISC.
                Emit(new Assignment(stackReg!, new BinaryExpression(Operator.ISub, stackReg!.DataType, stackReg, c)));
            }
            else
            {
                Emit(ab.CreateInstruction(callee, impProc.Signature, impProc.Characteristics));
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
            ImageMapVectorTable? imgVector;
            Expression switchExp;
            var eventListener = this.scanner.Services.RequireService<IDecompilerEventListener>();
            if (program.User.IndirectJumps.TryGetValue(addrSwitch, out var indJump))
            {
                // Trust the user knows what they're doing.
                vector = indJump.Table!.Addresses;
                switchExp = this.frame!.EnsureIdentifier(indJump.IndexRegister!);
                imgVector = indJump.Table;
            }
            else
            {
                var te = DiscoverTableExtent(addrSwitch, xfer);
                if (te is null)
                {
                    var navigator = eventListener.CreateJumpTableNavigator(program, this.arch, addrSwitch, null, 0);
                    return false;
                }

                foreach (var de in te.Accesses!)
                {
                    var item = new ImageMapItem(de.Key, (uint) de.Value.Size)
                    {
                        DataType = de.Value
                    };
                    program.ImageMap.AddItemWithSize(de.Key, item);
                }

                var idIndex = this.frame!.CreateTemporary(te.Index!.DataType);
                InsertSwitchIndexPreservingStatement(te, idIndex);

                imgVector = null;
                vector = te.Targets!;
                switchExp = idIndex;
            }

            if (xfer is RtlCall)
            {
                ScanCallVectorTargets(vector);
            }
            else
            {
                var jumpDests = ScanJumpVectorTargets(vector);
                var blockSource = blockCur!.IsSynthesized
                    ? blockCur
                    : scanner.FindContainingBlock(ric!.Address)!;
                blockCur = blockSource;
                foreach (var dest in jumpDests)
                {
                    blockSource.Procedure.ControlGraph.AddEdge(blockSource, dest);
                }
              
                if (switchExp is null)
                {
                    scanner.Warn(addrSwitch, "Unable to determine index variable for indirect jump.");
                    return false;
                }
                var sw = new SwitchInstruction(switchExp, jumpDests.ToArray());
                Emit(sw);
            }
            if (imgVector is not null)
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
        /// Injects an assignment instruction to keep the conditional 
        /// expression of a switch statement alive.
        /// </summary>
        public void InsertSwitchIndexPreservingStatement(TableExtent te, Identifier idIndex)
        {
            var a = te.GuardInstrAddress!;
            if (a is null)
                return;
            var addr = a.Value;
            var block = scanner.FindContainingBlock(addr);
            if (block is null)
            {
                // Weird, it should exist.
                this.scanner.Warn(addr, "Unable to find basic block containing address {0}. " +
                    "Switch statement reconstruction may be incorrect.",
                    addr);
                return;
            }
            var iStm = block.Statements.FindIndex(s => s.Address == te.GuardInstrAddress);
            if (iStm < 0)
            {
                this.scanner.Warn(addr, "Unable to find instruction at address {0}. " +
                   "Switch statement reconstruction may be incorrect.",
                   addr);
                return;
            }
            var ass = new Assignment(idIndex, te.Index!);
            block.Statements.Insert(iStm, addr, ass);
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
        private TableExtent? DiscoverTableExtent(
            Address addrSwitch,
            RtlTransfer xfer)
        {
            Debug.Assert(!(xfer.Target is Address || xfer.Target is Constant), $"This should not be a constant {xfer}.");
            var listener = scanner.Services.RequireService<IDecompilerEventListener>();

            var bwsHost = new BackwardSlicerHost(program, this.arch);
            var rtlBlock = bwsHost.GetRtlBlock(blockCur!);
            var bws = new BackwardSlicer(bwsHost, rtlBlock, state);
            return bws.DiscoverTableExtent(addrSwitch, xfer, listener);
        }


        private void ScanCallVectorTargets(List<Address> vector)
        {
            foreach (Address addr in vector)
            {
                if (!program.Memory.IsValidAddress(addr))
                    continue;
                var st = state.Clone();
                var pbase = scanner.ScanProcedure(blockCur!.Procedure.Architecture, addr, null, st);
                if (pbase is Procedure pcallee)
                {
                    program.CallGraph.AddEdge(blockCur.Statements[^1], pcallee);
                }
            }
        }

        private List<Block> ScanJumpVectorTargets(List<Address> vector)
        {
            var blocks = new List<Block>();
            foreach (Address addr in vector)
            {
                if (!program.Memory.IsValidAddress(addr))
                    break;
                var st = state.Clone();
                blocks.Add(BlockFromAddress(ric!.Address, addr, blockCur!.Procedure, st));
            }
            return blocks;
        }

        private void Emit(Instruction instruction)
        {
            blockCur!.Statements.Add(ric!.Address, instruction);
        }

        private void Emit(Instruction instruction, Block block)
        {
            block.Statements.Add(ric!.Address, instruction);
        }

        private Block FallthroughBlock(Address addrSrc, Procedure proc, Address fallthruAddress)
        {
            if (ri!.NextStatementRequiresLabel)
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
            var label = program.NamingPolicy.BlockName(ric!.Address);
            var block = proc.AddSyntheticBlock(ric.Address, $"{label}_{extraLabels}");
            if (program.User.BlockLabels.TryGetValue(label, out var userLabel))
                block.UserLabel = userLabel;
            return block;
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
        private ProcedureBase? SearchBackForProcedureConstant(Identifier id)
        {
            var visited = new HashSet<Block>();
            Block? block = blockCur;
            while (block is not null && !visited.Contains(block))
            {
                visited.Add(block);
                for (int i = block.Statements.Count - 1; i >= 0; --i)
                {
                    if (block.Statements[i].Instruction is not Assignment ass)
                        continue;
                    var idAss = ass.Dst ;
                    if (idAss is not null && idAss == id)
                    {
                        if (ass.Src is ProcedureConstant pc)
                        {
                            return pc.Procedure;
                        }
                        var imp = ImportedProcedureName(ass.Src);
                        if (imp is not null)
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
            foreach (Block block in blockCur!.Procedure.ControlGraph.Blocks)
            {
                Console.WriteLine("block: {0}", block.Id);
                Console.Write("\tpred:");
                foreach (var p in block.Procedure.ControlGraph.Predecessors(block))
                {
                    Console.Write(" {0}", p.Id);
                }
                Console.WriteLine();
                Console.Write("\tsucc:");
                foreach (var s in block.Procedure.ControlGraph.Successors(block))
                {
                    Console.Write(" {0}", s.Id);
                }
                Console.WriteLine();
            }
        }

        public ExternalProcedure? ImportedProcedureName(Expression callTarget)
        {
            if (callTarget is not MemoryAccess mem)
                return null;
            if (mem.EffectiveAddress.DataType.Size != this.program.Platform.PointerType.Size)
                return null;
            if (!program.TryInterpretAsAddress(mem.EffectiveAddress, true, out Address addrTarget))
                return null;
            var impEp = scanner.GetImportedProcedure(this.arch, addrTarget, ric!.Address);
            //if (impEp is not null)
                return impEp;
            //return scanner.GetInterceptedCall(addrTarget);
        }

        //$TODO: merge the followng two procedures?
        private void AffectProcessorState(FunctionType sig)
        {
            if (sig is null)
                return;
            for (int i = 0; i < sig.Outputs.Length; ++i)
            {
                var id = sig.Outputs[i];
                if (id.DataType is VoidType)
                    continue;
                TrashVariable(id.Storage);
            }
        }

        public void TrashVariable(Storage stg)
        {
            if (stg is null)
                return;
            switch (stg)
            {
            case RegisterStorage reg:
                state.SetValue(reg, InvalidConstant.Create(stg.DataType));
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
        private static ProcedureBase ResolveDispatchProcedureCall(ProcedureBase proc, ProcessorState state)
        {
            if (proc is not DispatchProcedure disp)
                return proc;
            var callable = disp.FindService(state);
            if (callable is null)
                return disp;
            else
                return callable;
        }

        private SystemService? MatchSyscallToService(RtlSideEffect side)
        {
            if (side.Expression is not Application fn)
                return null;
            if (fn.Procedure is not ProcedureConstant pc)
                return null;
            if (pc.Procedure is not IntrinsicProcedure intrinsic)
                return null;
            if (intrinsic.Name != IntrinsicProcedure.Syscall || fn.Arguments.Length == 0)
                return null;

            if (fn.Arguments[0] is not Constant vector)
                return null;
            var svc = program.Platform.FindService((int)vector.ToUInt32(), state, program.Memory);
            //$TODO if SVC is null (and not-speculating) report the error.
            if (svc is not null && svc.Signature is null)
            {
                scanner.Error(ric!.Address, $"System service '{svc.Name}' didn't specify a signature.");
            }
            return svc;
        }

        public override string ToString()
        {
            return $"{nameof(BlockWorkitem)}: {base.Address}";
        }
    }
}
