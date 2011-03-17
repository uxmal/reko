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
using Decompiler.Core.Rtl;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using System;
using System.Collections.Generic;
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
        private ProcessorState state;
        private ExpressionSimplifier eval;
        private IEnumerator<RtlInstructionCluster> rtlStream;
        private int extraLabels;

        public BlockWorkitem(
            IScanner scanner,
            Rewriter rewriter,
            ProcessorState state,
            Frame frame,
            Address addr)
        {
            this.scanner = scanner;
            this.arch = scanner.Architecture;
            this.rewriter = rewriter;
            this.state = state;
            this.eval = new ExpressionSimplifier(new ScannerEvaluator(state));
            this.frame = frame;
            this.addr = addr;
            this.blockCur = null;
        }

        public ProcessorState State { get { return state; } }

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
                state.SetInstructionPointer(ric.Address);
                for (var e = ric.Instructions.GetEnumerator(); e.MoveNext();)
                {
                    ri = e.Current;
                    if (!ri.Accept(this))
                        return;
                }
            }
        }

        private bool BlockHasBeenScanned(Block block)
        {
            return block.Statements.Count > 0;
        }

        /// <summary>
        /// Rewrites instructions until the current address is exactly on an instruction boundary.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="blockCur"></param>
        /// <param name="addrStart"></param>
        private void ResyncBlocks(Block oldBlock, Address addrStart)
        {
            uint linAddr = (uint) addrStart.Linear;
        }

        private void BuildApplication(Expression fn, ProcedureSignature sig, CallSite site)
        {
            var ab = new ApplicationBuilder(
                arch,
                frame,
                site,
                fn,
                sig);
            //state.ShrinkStack(sig.StackDelta);
            //state.ShrinkFpuStack(sig.FpuStackDelta);
            blockCur.Statements.Add(ric.Address.Linear, ab.CreateInstruction());
        }

        public Constant GetValue(Expression op)
        {
            return op.Accept<Expression>(eval) as Constant;
        }

        public void SetValue(Expression op, Constant c)
        {
            var id = op as Identifier;
            if (id == null)
                return;
            var reg = id.Storage as RegisterStorage;
            if (reg != null)
            {
                state.Set(reg.Register, c);
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
            blockCur.Statements.Add(ric.Address.Linear, inst);
            return true;
        }


        public bool VisitBranch(RtlBranch b)
        {
            // We don't know the 'then' block yet, as the following statements may chop up the block
            // we're presently in. Back-patch in when the block target is obtained.
            var branch = new Branch(b.Condition, null);
            blockCur.Statements.Add(
                ric.Address.Linear,
                branch);
            // The following statements may chop up the blockCur, so hang on to the essentials.
            var proc = blockCur.Procedure;
            var fallthruAddress = ric.Address + ric.Length;

            var blockThen = scanner.EnqueueJumpTarget(b.Target, proc, state.Clone());

            var blockElse = FallthroughBlock(proc, fallthruAddress);
            var branchingBlock = scanner.FindContainingBlock(ric.Address);
            branch.Target = blockThen;
            proc.ControlGraph.AddEdge(branchingBlock, blockElse);
            proc.ControlGraph.AddEdge(branchingBlock, blockThen);

            // Now, switch to the fallthru block.
            blockCur = blockElse;
            return true;
        }

        public bool VisitGoto(RtlGoto g)
        {
            var addrTarget = g.Target as Address;
            if (addrTarget != null)
            {
                scanner.TerminateBlock(blockCur, ric.Address + ric.Length);
                var blockDest = scanner.EnqueueJumpTarget(addrTarget, blockCur.Procedure, state);
                var blockSource = scanner.FindContainingBlock(ric.Address);
                blockSource.Procedure.ControlGraph.AddEdge(blockSource, blockDest);

                return false;
            }

            ProcessVector(ric.Address, g);
            return false;
        }


        public bool VisitCall(RtlCall call)
        {
            var site = state.OnBeforeCall();

            Address addr = call.Target as Address;
            if (addr != null)
            {
                var callee = scanner.ScanProcedure(addr, null, state);
                blockCur.Statements.Add(
                    ric.Address.Linear, 
                    new CallInstruction(
                        new ProcedureConstant(PrimitiveType.Pointer32, callee),
                        site,
                        call.ReturnAddressSize));
                scanner.CallGraph.AddEdge(blockCur.Statements.Last, callee);
                state.OnAfterCall(callee.Signature);
                return true;
            }

            var procCallee = call.Target as ProcedureConstant;
            if (procCallee != null)
            {
                var ppp = procCallee.Procedure as PseudoProcedure;
                if (ppp != null)
                {
                    BuildApplication(procCallee, ppp.Signature, site);
                    state.OnAfterCall(ppp.Signature);
                    return !ppp.Characteristics.Terminates;
                }
            }
            var sig = scanner.GetCallSignatureAtAddress(ric.Address);
            if (sig != null)
            {
                BuildApplication(call.Target, sig, site);
                state.OnAfterCall(sig);
                return true;
            }

            var id = call.Target as Identifier;
            if (id != null)
            {
                var ppp = SearchBackForProcedureConstant(id);
                if (ppp != null)
                {
                    var e = new ProcedureConstant(PrimitiveType.Create(Domain.Pointer, arch.WordWidth.Size), ppp);
                    BuildApplication(e, ppp.Signature, site);
                    return true;
                }
            }

            var imp = ImportedProcedureName(call.Target);
            if (imp != null)
            {
                BuildApplication(new ProcedureConstant(arch.PointerType, imp), imp.Signature, site);
                state.OnAfterCall(imp.Signature);
                return true;
            }

            blockCur.Statements.Add(
                    ric.Address.Linear,
                    new IndirectCall(
                        call.Target,
                        site));
            state.OnAfterCall(sig);
            return true;        //$BUGBUG: The called procedure could be exit(), or ExitThread(), in which case we should return false.
        }

        public bool VisitReturn(RtlReturn ret)
        {
            var proc = blockCur.Procedure;
            blockCur.Statements.Add(ric.Address.Linear, new ReturnInstruction());
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
            state.OnProcedureLeft(proc.Signature);
            //proc.Signature.FpuStackArgumentMax = maxFpuStackRead;     //$REDO
            //proc.Signature.FpuStackOutArgumentMax = maxFpuStackWrite;       //$REDO
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
                var site = state.OnBeforeCall();
                BuildApplication(fn, ep.Signature, site);
                if (svc.Characteristics.Terminates)
                {
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                AffectProcessorState(svc.Signature);
            }
            else
            {
                blockCur.Statements.Add(ric.Address.Linear, new SideEffect(side.Expression));
            }
            return true;
        }

        #endregion

        private void ProcessVector(Address addrSwitch, RtlTransfer xfer)
        {
            var bw = new Backwalker(xfer, eval);
            if (!bw.CanBackwalk())
                throw new NotImplementedException(" scanner.AddDiagnostic(...");
            var bwos = bw.BackWalk(blockCur, null);
            if (bwos.Count == 0)
                return;     //$REVIEW: warn?

            VectorBuilder builder = new VectorBuilder(arch, scanner, new Decompiler.Core.Lib.DirectedGraphImpl<object>());
            List<Address> vector = builder.BuildAux(bw, addrSwitch, state);
            if (vector.Count == 0)
            {
                var addrNext = bw.VectorAddress + bw.Stride;
                var rdr = scanner.CreateReader(bw.VectorAddress);
                if (!rdr.IsValid)
                    return;
                // Can't determine the size of the table, but surely it has one entry?
                vector.Add(arch.ReadCodeAddress(xfer.Target.DataType.Size, rdr, state));
            }

            foreach (Address addr in vector)
            {
                var st = state.Clone();
                if (xfer is RtlCall)
                {
                    scanner.ScanProcedure(addr, null, st);
                }
                else
                {
                    var blockDest = scanner.EnqueueJumpTarget(addr, blockCur.Procedure, state);
                    var blockSource = scanner.FindContainingBlock(ric.Address);
                    blockSource.Procedure.ControlGraph.AddEdge(blockSource, blockDest);
                }
            }
            //vectorUses[wi.addrFrom] = new VectorUse(wi.Address, builder.IndexRegister);
            //map.AddItem(wi.Address + builder.TableByteSize, new ImageMapItem());
        }

        private Block FallthroughBlock(Procedure proc, Address fallthruAddress)
        {
            if (ri.NextStatementRequiresLabel)
            {
                // Some machine instructions, like the X86 'rep cmps' instruction, force the need to generate 
                // a label where there wouldn't be one normally, in the middle of the rtl sequence corresponding to
                // the machine instruction.

                var fallthru = new Block(proc, ric.Address.GenerateName("l", string.Format("_{0}", ++extraLabels)));
                proc.ControlGraph.Nodes.Add(fallthru);
                return fallthru;
            }
            else
            {
                return scanner.EnqueueJumpTarget(fallthruAddress, proc, state);
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
        // Because the scanner constant propagation is doing its propagation by bits (see IntelProcessorstate)
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
            foreach (Block block in blockCur.Procedure.ControlGraph.Nodes)
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
                state.Set(reg.Register, Constant.Invalid);
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
            return scanner.Platform.FindService(vector.ToInt32(), state);
        }


    }
}
