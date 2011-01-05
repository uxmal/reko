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
using System;
using System.Collections.Generic;
using System.Text;

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
        private Rewriter2 rewriter;
        private ProcessorState state;
        private IEnumerator<RtlInstruction> rtlStream;

        public BlockWorkitem(
            IScanner scanner,
            Rewriter2 rewriter,
            ProcessorState state,
            Frame frame,
            Address addr)
        {
            this.scanner = scanner;
            this.arch = scanner.Architecture;
            this.rewriter = rewriter;
            this.state = state;
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
                ri = rtlStream.Current;
                if (blockCur != scanner.FindContainingBlock(ri.Address))
                    break;
                state.SetInstructionPointer(ri.Address);
                if (!ri.Accept(this))
                    break;
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
            ApplicationBuilder ab = new ApplicationBuilder(
                frame,
                site,
                fn,
                sig);
            //state.ShrinkStack(sig.StackDelta);
            //state.ShrinkFpuStack(sig.FpuStackDelta);
            blockCur.Statements.Add(ri.Address.Linear, ab.CreateInstruction());
        }

        public Constant GetValue(Expression op)
        {
            var eval = new ScannerEvaluator(state);
            return eval.GetValue(op);
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

        private void ProcessVector(Address addrSwitch, Address addrVector, ushort segBase, int stride)
        {
            VectorBuilder builder = new VectorBuilder(arch, null, new Decompiler.Core.Lib.DirectedGraphImpl<object>());
            Address[] vector = builder.Build(addrVector, addrSwitch, segBase, stride);
            if (vector == null)
            {
                throw new NotImplementedException();
                //Address addrNext = addrVector + wi.stride.Size;
                //if (scanner.isprogram.Image.IsValidAddress(addrNext))
                //{
                //    // Can't determine the size of the table, but surely it has one entry?
                //    map.AddItem(addrNext, new ImageMapItem());
                //}
                //return;
            }

            throw new NotImplementedException();
            //item.Addresses.AddRange(vector);
            //for (int i = 0; i < vector.Length; ++i)
            //{
            //    ProcessorState st = wi.state.Clone();
            //    if (wi.table.IsCallTable)
            //    {
            //        EnqueueProcedure(wiCur, vector[i], null, st);
            //    }
            //    else
            //    {
            //        jumpGraph.AddEdge(wi.addrFrom - 1, vector[i]);
            //        EnqueueJumpTarget(vector[i], st, wi.proc);
            //    }
            //}
            //vectorUses[wi.addrFrom] = new VectorUse(wi.Address, builder.IndexRegister);
            //map.AddItem(wi.Address + builder.TableByteSize, new ImageMapItem());
        }

        #region InstructionVisitor Members

        public bool VisitAssignment(RtlAssignment a)
        {
            SetValue(a.Dst, GetValue(a.Src));
            var idDst = a.Dst as Identifier;
            var inst = (idDst != null)
                ? new Assignment(idDst, a.Src)
                : (Instruction) new Store(a.Dst, a.Src);
            blockCur.Statements.Add(ri.Address.Linear, inst);
            return true;
        }


        public bool VisitBranch(RtlBranch b)
        {
            // The following statements may chop up the blockCur, so hang on to the essentials.
            var proc = blockCur.Procedure;
            var fallthruAddress = ri.Address + ri.Length;
            var blockThen = scanner.EnqueueJumpTarget(b.Target, proc, state.Clone());
            var blockElse = FallthroughBlock(proc, fallthruAddress);
            var branchingBlock = scanner.FindContainingBlock(ri.Address);
            branchingBlock.Statements.Add(
                ri.Address.Linear,
                new Branch(b.Condition, blockThen));
            proc.ControlGraph.AddEdge(branchingBlock, blockElse);
            proc.ControlGraph.AddEdge(branchingBlock, blockThen);

            // Now, switch to the fallthru block.
            blockCur = blockElse;
            return true;
        }

        private Block FallthroughBlock(Procedure proc, Address fallthruAddress)
        {
            if (ri.NextStatementRequiresLabel)
            {
                // Some machine instructions, like the X86 'rep' prefix, force the need to generate 
                // a label where there wouldn't be one normally.

                var fallthru = new Block(proc, blockCur.Name + "x");
                proc.ControlGraph.Nodes.Add(fallthru);
                return fallthru;
            }
            else
            {
                return scanner.EnqueueJumpTarget(fallthruAddress, proc, state);
            }
        }

        public bool VisitGoto(RtlGoto g)
        {
            var addrTarget = g.Target as Address;
            if (addrTarget != null)
            {
                var blockDest = scanner.EnqueueJumpTarget(addrTarget, blockCur.Procedure, state);
                var blockSource = scanner.FindContainingBlock(ri.Address);
                blockCur.Procedure.ControlGraph.AddEdge(blockSource, blockDest);
                return false;
            }

            blockCur.Statements.Add(ri.Address.Linear, new GotoInstruction(g.Target));
            var mem = g.Target as MemoryAccess;
            if (mem == null)
                return false;
            var ea = mem.EffectiveAddress as BinaryExpression;
            if (ea != null)
                return false;
            if (ea.op != Operator.Add)
                return false;
            var cTableOffset = ea.Right as Constant;
            if (cTableOffset == null || cTableOffset == Constant.Invalid)
                return false;
            ea = ea.Left as BinaryExpression;
            if (ea == null)
                return false;
            if (ea.op != Operator.Muls && ea.op != Operator.Mul && ea.op != Operator.Mulu)
                return false;
            var cStride = ea.Right as Constant;
            if (cStride == null || cStride == Constant.Invalid)
                return false;

            ProcessVector(ri.Address, OffsetToAddress(mem, cTableOffset), 0, cStride.ToInt32());
            return false;
        }

        private Address OffsetToAddress(MemoryAccess mem, Constant cTableOffset)
        {
            throw new NotImplementedException();
        }


        public bool VisitCall(RtlCall call)
        {
            var site = state.OnBeforeCall();

            Address addr = call.Target as Address;
            if (addr != null)
            {
                var callee = scanner.EnqueueProcedure(this, addr, null, state);
                blockCur.Statements.Add(
                    ri.Address.Linear, 
                    new CallInstruction(
                        new ProcedureConstant(PrimitiveType.Pointer32, callee),
                        site,
                        call.ReturnAddressSize));
                scanner.CallGraph.AddEdge(blockCur.Statements.Last, callee);
                state.OnAfterCall(callee.Signature);
                return true;
            }

            var sig = scanner.GetCallSignatureAtAddress(ri.Address);
            if (sig != null)
            {
                BuildApplication(call.Target, sig, site);
                state.OnAfterCall(sig);
                return true;
            }

            var imp = ImportedProcedureName(call.Target);
            if (imp != null)
            {
                BuildApplication(new ProcedureConstant(arch.PointerType, imp), imp.Signature, site);
                state.OnAfterCall(imp.Signature);
                return true;
            }

            blockCur.Statements.Add(
                    ri.Address.Linear,
                    new IndirectCall(
                        call.Target,
                        site));
            state.OnAfterCall(sig);
            return true;        //$BUGBUG: but may call exit(), or ExitThread(), which should return false.
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
            return (PseudoProcedure)scanner.GetImportedProcedure(new Address(offset.ToUInt32()));
        }


        public bool VisitReturn(RtlReturn ret)
        {
            var proc = blockCur.Procedure;
            blockCur.Statements.Add(ri.Address.Linear, new ReturnInstruction());
            blockCur.Procedure.ControlGraph.AddEdge(blockCur, proc.ExitBlock);

            if (frame.ReturnAddressSize != 0)
            {
                if (frame.ReturnAddressSize != ret.ReturnAddressBytes)
                {
                    scanner.AddDiagnostic(
                        ri.Address,
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
                    ri.Address,
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
            scanner.TerminateBlock(blockCur, ri.Address + ri.Length);
            return false;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            SystemService svc = MatchSyscallToService(side);
            if (svc != null)
            {
                ExternalProcedure ep = svc.CreateExternalProcedure(arch);
                ProcedureConstant fn = new ProcedureConstant(arch.PointerType, ep);
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
                blockCur.Statements.Add(ri.Address.Linear, new SideEffect(side.Expression));
            }
            return true;
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

        #endregion

    }
}
