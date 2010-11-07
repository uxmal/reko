#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
    public class BlockWorkitem2 : WorkItem2, RtlInstructionVisitor<bool>
    {
        private IScanner scanner;
        private IProcessorArchitecture arch;
        private Block blockCur;
        private Frame frame;
        private RtlInstruction ri;
        private Rewriter2 rewriter;
        private ProcessorState state;

        public BlockWorkitem2(
            IScanner scanner,
            Rewriter2 rewriter,
            ProcessorState state,
            Frame frame,
            Block block)
        {
            this.scanner = scanner;
            this.arch = scanner.Architecture;
            this.rewriter = rewriter;
            this.state = state;
            this.frame = frame;
            this.blockCur = block;
        }

        public override void Process()
        {
            for (var e = rewriter.GetEnumerator(); e.MoveNext(); )
            {
                ri = e.Current;
                state.SetInstructionPointer(ri.Address);
                if (!ri.Accept(this))
                    break;
            }
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

        private Block AddBlock(Block block)
        {
            throw new NotImplementedException();
        }

        private void BuildApplication(Expression fn, ProcedureSignature sig)
        {
            ApplicationBuilder ab = new ApplicationBuilder(
                frame,
                new CallSite(0, 0),
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

        private void ProcessVector(VectorWorkItemOld wi)
        {
            throw new NotImplementedException();
            ////$TODO: pass imagemapvectortable in workitem.
            //ImageMapItem q;
            //map.TryFindItem(wi.Address, out q);
            //ImageMapVectorTable item = (ImageMapVectorTable)q;
            //VectorBuilder builder = new VectorBuilder(program, map, jumpGraph);
            //Address[] vector = builder.Build(wi.Address, wi.addrFrom, wi.segBase, wi.stride);
            //if (vector == null)
            //{
            //    Address addrNext = wi.Address + wi.stride.Size;
            //    if (program.Image.IsValidAddress(addrNext))
            //    {
            //        // Can't determine the size of the table, but surely it has one entry?
            //        map.AddItem(addrNext, new ImageMapItem());
            //    }
            //    return;
            //}

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
            var blockThen = scanner.EnqueueJumpTarget(b.Target, blockCur.Procedure, state.Clone());
            var blockElse = scanner.EnqueueJumpTarget(ri.Address + ri.Length, blockCur.Procedure, state);
            this.blockCur.Statements.Add(
                ri.Address.Linear,
                new Branch(b.Condition, blockThen));
            return false;
        }

        public bool VisitGoto(RtlGoto g)
        {
            var addrTarget = g.Target as Address;
            if (addrTarget != null)
            {
                var blockTarget = scanner.EnqueueJumpTarget(addrTarget, blockCur.Procedure, state);
                blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockTarget);
                return false;
            }

            blockCur.Statements.Add(ri.Address.Linear, new GotoInstruction(g.Target));
            var mem = g.Target as MemoryAccess;
            if (mem == null)
                return false;
            var ea = mem.EffectiveAddress as BinaryExpression;
            if (ea != null)
                return false;
            scanner.EnqueueVectorTable(ri.Address, null, null, 0, false, blockCur.Procedure, state);
            return false;
        }

        public bool VisitCall(RtlCall call)
        {
            Address addr = call.Target as Address;
            if (addr != null)
            {
                var callee = scanner.EnqueueProcedure(this, addr, null, null);
                blockCur.Statements.Add(
                    ri.Address.Linear, 
                    new CallInstruction(
                        new ProcedureConstant(PrimitiveType.Pointer32, callee),
                        new CallSite(0, 0)));
                scanner.CallGraph.AddEdge(blockCur.Statements.Last, callee);
                return true;
            }

            var sig = scanner.GetCallSignatureAtAddress(ri.Address);
            if (sig != null)
            {
                BuildApplication(call.Target, sig);
            }
            else
            {
                blockCur.Statements.Add(
                    ri.Address.Linear,
                    new IndirectCall(
                        call.Target,
                        new CallSite(0, 0)));
            }
            return true;        //$BUGBUG: but may call exit(), or ExitThread(), which should return false.
        }

        public bool VisitReturn(RtlReturn ret)
        {
            var proc = blockCur.Procedure;
            blockCur.Statements.Add(ri.Address.Linear, new ReturnInstruction(null));
            blockCur.Procedure.ControlGraph.AddEdge(blockCur, proc.ExitBlock);

            if (frame.ReturnAddressSize != ret.ReturnAddressBytes)
            {
                scanner.AddDiagnostic(
                    ri.Address,
                    new WarningDiagnostic(string.Format(
                    "Caller expects a return address of {0} bytes, but procedure {1} was previously called with a return address of {2} bytes.",
                    ret.ReturnAddressBytes, proc.Name, frame.ReturnAddressSize)));
            }
            if (proc.Signature.StackDelta != 0 && proc.Signature.StackDelta != ret.ExtraBytesPopped)
            {
                scanner.AddDiagnostic(
                    ri.Address,
                    new WarningDiagnostic(string.Format(
                    "Multiple differing values of stack delta in procedure {0} when processing RET instruction; was {1} previously.", proc.Name, proc.Signature.StackDelta)));
            }
            else
            {
                proc.Signature.StackDelta = ret.ExtraBytesPopped;
            }
            //proc.Signature.FpuStackDelta = state.FpuStackItems;       //$REDO
            //proc.Signature.FpuStackArgumentMax = maxFpuStackRead;     //$REDO
            //proc.Signature.FpuStackOutArgumentMax = maxFpuStackWrite;       //$REDO
            return false;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            SystemService svc = MatchSyscallToService(side);
            if (svc != null)
            {
                ExternalProcedure ep = svc.CreateExternalProcedure(arch);
                ProcedureConstant fn = new ProcedureConstant(arch.PointerType, ep);
                BuildApplication(fn, ep.Signature);
                if (svc.Characteristics.Terminates)
                {
                    blockCur.Procedure.ControlGraph.AddEdge(blockCur, blockCur.Procedure.ExitBlock);
                    return false;
                }
                AffectProcessorState(svc.Signature);
            }
            return true;
        }

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
