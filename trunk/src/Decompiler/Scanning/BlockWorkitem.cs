#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Rtl;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Scanner work item for processing basic blocks.
    /// </summary>
    public class BlockWorkitem : WorkItem, RtlInstructionVisitor<bool>
    {
        private IScanner scanner;
        private IProcessorArchitecture arch;
        private Address addr;
        private Block blockCur;
        private Frame frame;
        private RtlInstruction ri;
        private RtlInstructionCluster ric;
        private Rewriter rewriter;
        private ScannerEvaluationContext scEval;
        private ExpressionSimplifier eval;
        private IEnumerator<RtlInstructionCluster> rtlStream;
        private int extraLabels;

        public BlockWorkitem(
            IScanner scanner,
            Rewriter rewriter,
            ScannerEvaluationContext state,
            Frame frame,
            Address addr)
        {
            this.scanner = scanner;
            this.arch = scanner.Architecture;
            this.rewriter = rewriter;
            this.scEval = state;
            this.eval = new ExpressionSimplifier(scEval);
            this.frame = frame;
            this.addr = addr;
            this.blockCur = null;
        }

        public Block Block { get { return blockCur; } } 
        public ScannerEvaluationContext Context { get { return scEval; } }       //$TODO: can we get rid of this? apparently only test use it.

        public override void Process()
        {
            rtlStream = rewriter.GetEnumerator();
            blockCur = scanner.FindContainingBlock(addr);
            if (BlockHasBeenScanned(blockCur))
                return;
            while (rtlStream.MoveNext())
            {
                ric = rtlStream.Current;
                if (blockCur != scanner.FindContainingBlock(ric.Address))
                    break;
                scEval.State.SetInstructionPointer(ric.Address);
                for (var e = ric.Instructions.GetEnumerator(); e.MoveNext();)
                {
                    ri = e.Current;
                    if (!ri.Accept(this))
                        return;
                }
                var blNext = FallenThroughNextBlock(ric.Address + ric.Length);
                if (blNext != null)
                {
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blNext);
                    return;
                }
            }
        }

        private Block FallenThroughNextBlock(Address addr)
        {
            var cont = scanner.FindContainingBlock(addr);
            if (cont == null || cont == blockCur)
                return null;
            return cont;
        }

        private bool BlockHasBeenScanned(Block block)
        {
            return block.Statements.Count > 0;
        }

        private Instruction BuildApplication(Expression fn, ProcedureSignature sig, CallSite site)
        {
            var ab = CreateApplicationBuilder(fn, sig, site);
            return ab.CreateInstruction();
        }

        private ApplicationBuilder CreateApplicationBuilder(Expression callee, ProcedureSignature sig, CallSite site)
        {
            var ab = new ApplicationBuilder(
                arch,
                frame,
                site,
                callee,
                sig);
            return ab;
        }

        public Expression GetValue(Expression op)
        {
            return op.Accept<Expression>(eval);
        }

        public void SetValue(Expression dst, Expression  value)
        {
            var id = dst as Identifier;
            if (id != null)
            {
                scEval.SetValue(id, value);
                return;
            }
            var smem = dst as SegmentedAccess;
            if (smem != null)
            {
                scEval.SetValueEa(smem.BasePointer, GetValue(smem.EffectiveAddress), value);
            }
            var mem = dst as MemoryAccess;
            if (mem != null)
            {
                scEval.SetValueEa(GetValue(mem.EffectiveAddress), value);
                return;
            }
        }

        #region InstructionVisitor Members

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


        public bool VisitBranch(RtlBranch b)
        {
            // We don't know the 'then' block yet, as the following statements may chop up the block
            // we're presently in. Back-patch in when the block target is obtained.
            var branch = new Branch(b.Condition, null);
            Emit(branch);

            // The following statements may chop up the blockCur, so hang on to the essentials.
            var proc = blockCur.Procedure;
            var fallthruAddress = ric.Address + ric.Length;

            var blockThen = scanner.EnqueueJumpTarget(b.Target, proc, scEval.Clone());

            var blockElse = FallthroughBlock(proc, fallthruAddress);
            var branchingBlock = scanner.FindContainingBlock(ric.Address);
            branch.Target = blockThen;      // The back-patch referred to above.
            proc.ControlGraph.AddEdge(branchingBlock, blockElse);
            proc.ControlGraph.AddEdge(branchingBlock, blockThen);

            // Now, switch to the fallthru block and keep rewriting.
            blockCur = blockElse;
            return true;
        }

        public bool VisitGoto(RtlGoto g)
        {
            var addrTarget = g.Target as Address;
            if (addrTarget != null)
            {
                scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                var blockDest = scanner.EnqueueJumpTarget(addrTarget, blockCur.Procedure, scEval);
                var blockSource = scanner.FindContainingBlock(ric.Address);
                blockSource.Procedure.ControlGraph.AddEdge(blockSource, blockDest);

                return false;
            }
            ProcessVector(ric.Address, g);
            return false;
        }

        private ProcedureConstant CreateProcedureConstant(ProcedureBase callee)
        {
            return new ProcedureConstant(arch.PointerType, callee);
        }

        public bool VisitCall(RtlCall call)
        {
            var site = scEval.State.OnBeforeCall(call.ReturnAddressSize);

            Address addr = call.Target as Address;
            if (addr != null)
            {
                var impProc = scanner.GetImportedProcedure(addr.Linear);
                if (impProc != null && impProc.Characteristics.IsAlloca)
                    return ProcessAlloca(site, impProc);

                var callee = scanner.ScanProcedure(addr, null, scEval);
                Emit(new CallInstruction(CreateProcedureConstant(callee), site));
                var pCallee = callee as Procedure;
                if (pCallee != null)
                {
                    scanner.CallGraph.AddEdge(blockCur.Statements.Last, pCallee);
                }
                scEval.State.OnAfterCall(callee.Signature);
                return true;
            }

            var procCallee = call.Target as ProcedureConstant;
            if (procCallee != null)
            {
                var ppp = procCallee.Procedure as PseudoProcedure;
                if (ppp != null)
                {
                    Emit(BuildApplication(procCallee, ppp.Signature, site));
                    scEval.State.OnAfterCall(ppp.Signature);
                    return !ppp.Characteristics.Terminates;
                }
            }
            var sig = scanner.GetCallSignatureAtAddress(ric.Address);
            if (sig != null)
            {
                Emit(BuildApplication(call.Target, sig, site));
                scEval.State.OnAfterCall(sig);
                return true;
            }

            var id = call.Target as Identifier;
            if (id != null)
            {
                var ppp = SearchBackForProcedureConstant(id);
                if (ppp != null)
                {
                    var e = CreateProcedureConstant(ppp);
                    Emit(BuildApplication(e, ppp.Signature, site));
                    return !ppp.Characteristics.Terminates;
                }
            }

            var imp = ImportedProcedureName(call.Target);
            if (imp != null)
            {
                Emit(BuildApplication(CreateProcedureConstant(imp), imp.Signature, site));
                scEval.State.OnAfterCall(imp.Signature);
                return !imp.Characteristics.Terminates;
            }

            var ic = new IndirectCall(call.Target, site);
            Emit(ic);
            sig = GuessProcedureSignature(ic);
            scEval.State.OnAfterCall(sig);
            return true;        //$BUGBUG: The called procedure could be exit(), or ExitThread(), in which case we should return false.
        }

        public bool VisitReturn(RtlReturn ret)
        {
            var proc = blockCur.Procedure;
            Emit(new ReturnInstruction());
            blockCur.Procedure.ControlGraph.AddEdge(blockCur, proc.ExitBlock);

            if (frame.ReturnAddressSize != 0)
            {
                if (frame.ReturnAddressSize != ret.ReturnAddressBytes)
                {
                    scanner.AddDiagnostic(
                        ric.Address,
                        new WarningDiagnostic(string.Format(
                        "Procedure {1} previously had a return address of {2} bytes on the stack, but now seems to have a return address of {0} bytes on the stack.",
                        ret.ReturnAddressBytes, proc.Name, frame.ReturnAddressSize)));
                }
            }
            else
            {
                frame.ReturnAddressSize = ret.ReturnAddressBytes;
            }

            int stackDelta = ret.ReturnAddressBytes + ret.ExtraBytesPopped;
            if (proc.Signature.StackDelta != 0 && proc.Signature.StackDelta != stackDelta)
            {
                scanner.AddDiagnostic(
                    ric.Address,
                    new WarningDiagnostic(string.Format(
                    "Multiple different values of stack delta in procedure {0} when processing RET instruction; was {1} previously.", proc.Name, proc.Signature.StackDelta)));
            }
            else
            {
                proc.Signature.StackDelta = stackDelta;
            }
            scEval.State.OnProcedureLeft(proc.Signature);
            scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
            return false;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            var svc = MatchSyscallToService(side);
            if (svc != null)
            {
                var ep = svc.CreateExternalProcedure(arch);
                var fn = new ProcedureConstant(arch.PointerType, ep);
                var site = scEval.State.OnBeforeCall(svc.Signature.ReturnAddressOnStack);
                Emit(BuildApplication(fn, ep.Signature, site));
                if (svc.Characteristics.Terminates)
                {
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                AffectProcessorState(svc.Signature);
            }
            else
            {
                Emit(new SideEffect(side.Expression));
            }
            return true;
        }

        private ProcedureSignature GuessProcedureSignature(IndirectCall ic)
        {
            return new ProcedureSignature(); //$TODO: attempt to detect parameters of procedure?
            // This would have to be arch-dependent + platform-dependent as some arch pass
            // on stack, while others pass in registers, or a combination or both
            // (" xthiscall" in x86 µsoft world).
        }

        public bool ProcessAlloca(CallSite site, PseudoProcedure impProc)
        {
            if (impProc.Signature == null)
                throw new ApplicationException(string.Format("You must specify a procedure signature for {0} since it has been marked as 'alloca'.", impProc.Name));
            var ab = CreateApplicationBuilder(new ProcedureConstant(arch.PointerType, impProc), impProc.Signature, site);
            if (impProc.Signature.FormalArguments.Length != 1)
                throw new ApplicationException(string.Format("An alloca function must have exactly one parameter, but {0} has {1}.", impProc.Name, impProc.Signature.FormalArguments.Length));
            var target = ab.Bind(impProc.Signature.FormalArguments[0]);
            var id = target as Identifier;
            if (id == null)
                throw new ApplicationException(string.Format("The parameter of {0} wasn't a register.", impProc.Name));
            Constant c = scEval.GetValue(id) as Constant;
            if (c != null && c.IsValid)
            {
                var sp = frame.EnsureRegister(arch.StackRegister);
                Emit(new Assignment(sp, new BinaryExpression(Operator.Sub, sp.DataType, sp, c)));
            }
            else
            {
                Emit(ab.CreateInstruction());
            }
            return true;
        }

        #endregion

        private void ProcessVector(Address addrSwitch, RtlTransfer xfer)
        {
            var bw = new Backwalker(new BackwalkerHost(), xfer, eval);
            if (!bw.CanBackwalk())
            {
                scanner.AddDiagnostic(addrSwitch, new WarningDiagnostic(
                    string.Format("Unable to determine register used in transfer instruction {0}.", xfer)));
                return;
            }
            var bwops = bw.BackWalk(blockCur);
            if (bwops.Count == 0)
                return;     //$REVIEW: warn?
            var idIndex = blockCur.Procedure.Frame.EnsureRegister(bw.Index);

            VectorBuilder builder = new VectorBuilder(arch, scanner, new DirectedGraphImpl<object>());
            List<Address> vector = builder.BuildAux(bw, addrSwitch, scEval.State);
            if (vector.Count == 0)
            {
                var addrNext = bw.VectorAddress + bw.Stride;
                var rdr = scanner.CreateReader(bw.VectorAddress);
                if (!rdr.IsValid)
                    return;
                // Can't determine the size of the table, but surely it has one entry?
                vector.Add(arch.ReadCodeAddress(xfer.Target.DataType.Size, rdr, scEval.State));
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
                Emit(new SwitchInstruction(idIndex, blockCur.Procedure.ControlGraph.Successors(blockCur).ToArray()));
            }
            //vectorUses[wi.addrFrom] = new VectorUse(wi.Address, builder.IndexRegister);
            //map.AddItem(wi.Address + builder.TableByteSize, new ImageMapItem());
        }

        private void ScanVectorTargets(RtlTransfer xfer, List<Address> vector)
        {
            foreach (Address addr in vector)
            {
                var st = scEval.Clone();
                if (xfer is RtlCall)
                {
                    scanner.ScanProcedure(addr, null, st);
                }
                else
                {
                    scanner.EnqueueJumpTarget(addr, blockCur.Procedure, scEval);
                }
            }
        }

        private void Emit(Instruction instruction)
        {
            blockCur.Statements.Add(ric.Address.Linear, instruction);
        }

        private Block FallthroughBlock(Procedure proc, Address fallthruAddress)
        {
            if (ri.NextStatementRequiresLabel)
            {
                // Some machine instructions, like the X86 'rep cmps' instruction, force the need to generate 
                // a label where there wouldn't be one normally, in the middle of the rtl sequence corresponding to
                // the machine instruction.

                var fallthru = new Block(proc, ric.Address.GenerateName("l", string.Format("_{0}", ++extraLabels)));
                proc.ControlGraph.Blocks.Add(fallthru);
                return fallthru;
            }
            else
            {
                return scanner.EnqueueJumpTarget(fallthruAddress, proc, scEval);
            }
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
        private PseudoProcedure SearchBackForProcedureConstant(Identifier id)
        {
            var visited = new HashSet<Block>();
            Block block = blockCur;
            while (block != null && !visited.Contains(block))
            {
                visited.Add(block);
                for (int i = block.Statements.Count - 1; i >= 0; --i)
                {
                    Assignment ass = block.Statements[i].Instruction as Assignment;
                    if (ass != null)
                    {
                        Identifier idAss = ass.Dst as Identifier;
                        if (idAss != null && idAss == id)
                        {
                            ProcedureConstant pc = ass.Src as ProcedureConstant;
                            if (pc != null)
                            {
                                return (PseudoProcedure)pc.Procedure;
                            }
                            else
                                return null;
                        }
                    }
                }
                var pred = block.Procedure.ControlGraph.Predecessors(block).ToArray();
                if (pred.Length != 1)
                    return null;
                block = pred[0];
            }
            return null;
        }

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

        public PseudoProcedure ImportedProcedureName(Expression callTarget)
        {
            var mem = callTarget as MemoryAccess;
            if (mem == null)
                return null;
            if (mem.EffectiveAddress.DataType.Size != PrimitiveType.Word32.Size)
                return null;
            var offset = mem.EffectiveAddress as Constant;
            if (offset == null)
                return null;
            return (PseudoProcedure)scanner.GetImportedProcedure(offset.ToUInt32());
        }

        //$TODO: merge these?
        private void AffectProcessorState(ProcedureSignature sig)
        {
            TrashVariable(sig.ReturnValue);
            for (int i = 0; i < sig.FormalArguments.Length; ++i)
            {
                var os = sig.FormalArguments[i].Storage as OutArgumentStorage;
                if (os != null)
                {
                    TrashVariable(os.OriginalIdentifier);
                }
            }
        }

        public void TrashVariable(Identifier id)
        {
            if (id == null)
                return;
            var reg = id.Storage as RegisterStorage;
            if (reg != null)
            {
                scEval.SetValue(id, Constant.Invalid);
            }
            SequenceStorage seq = id.Storage as SequenceStorage;
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
            if (ppp.Name != "__syscall")
                return null;

            var vector = fn.Arguments[0] as Constant;
            if (vector == null)
                return null;
            return scanner.Platform.FindService(vector.ToInt32(), scEval.State);
        }

        private class BackwalkerHost : IBackWalkHost
        {

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
        }


    }
}
