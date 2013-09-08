#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Serialization;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Classifies blocks in a program as being either:
    /// * nothing special
    /// * blocks that need to be moved to another, possibly new, procedure.
    /// * blocks that need to be cloned into other procedures.
    /// </summary>
    public class CrossProcedureAnalyzer
    {
        private Program prog;
        private Block block;

        public CrossProcedureAnalyzer(Program prog)
        {
            this.prog = prog;
            this.BlocksNeedingCloning = new HashSet<Block>();
            this.BlocksNeedingPromotion = new HashSet<Block>();
        }

        public HashSet<Block> BlocksNeedingCloning { get; private set; }
        public HashSet<Block> BlocksNeedingPromotion { get; private set; }

        /// <summary>
        /// Analyzes the blocks in a procedure to determine if they are being reached from
        /// procedures outside of the procedure proc.
        /// </summary>
        /// <param name="proc"></param>
        public void Analyze(Procedure proc)
        {
            foreach (var block in new DfsIterator<Block>(proc.ControlGraph).PreOrder())
            {
                if (block.Statements.Count == 0)
                    continue;
                this.block = block;
                if (IsBlockEnteredFromOtherProcedures(block, proc))
                {
                    if (IsBlockLinearProcedureExit(block))
                    {
                        BlocksNeedingCloning.Add(block);
                    }
                    else
                    {
                        BlocksNeedingPromotion.Add(block);
                        //var procNew = PromoteBlockToProcedureEntry(block);
                        //CloneBlockIntoOtherProcedures(block, proc);
                        //ReplaceInboundEdgesWithCalls(block);
                    }
                }
            }
        }

        public void ReplaceInboundEdgesWithCalls(Block block)
        {
            throw new NotImplementedException();
        }

        public bool IsBlockLinearProcedureExit(Block block)
        {
            if (block.Statements.Count == 0)
                return false;
            return block.Statements.Last.Instruction is ReturnInstruction;
        }

        public bool IsBlockEnteredFromOtherProcedures(Block block, Procedure proc)
        {
            return block.Pred.Any(p => p.Procedure != proc);
        }

        /// <summary>
        /// Promotes a block to being the entry of a procedure.
        /// </summary>
        /// <param name="block">Block to promote</param>
        /// <param name="addrStart">Address at which the block starts</param>
        /// <param name="proc">The procedure from which the block is called.</param>
        /// <returns></returns>
        public Procedure PromoteBlockToProcedureEntry(Block block, Procedure procOld)
        {
            Address addrStart = prog.ImageMap.MapLinearAddressToAddress(block.Statements[0].LinearAddress);

            Procedure procNew;
            if (!prog.Procedures.TryGetValue(addrStart, out procNew))
            {
                procNew = Procedure.Create(addrStart, prog.Architecture.CreateFrame());
                procNew.Frame.ReturnAddressSize = procOld.Frame.ReturnAddressSize;
                procNew.Characteristics = new ProcedureCharacteristics(procOld.Characteristics);
                prog.Procedures.Add(addrStart, procNew);
                prog.CallGraph.AddProcedure(procNew);
            }
            return procNew;
        }

        private Block CreateCallRetThunk(Block block, Procedure procOld, Procedure procNew)
        {
            var callRetThunkBlock = procOld.AddBlock(block.Name + Scanner.CallRetThunkSuffix);
            callRetThunkBlock.Statements.Add(0, new CallInstruction(
                    new ProcedureConstant(prog.Architecture.PointerType, procNew),
                    new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
            prog.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
            callRetThunkBlock.Statements.Add(0, new ReturnInstruction());
            return callRetThunkBlock;
        }

        public void Analyze(Program prog)
        {
            Analyze(prog.Procedures.Values);
            PromoteBlocksToProcedures(this.BlocksNeedingPromotion);
            CloneBlocksIntoOtherProcedures(this.BlocksNeedingCloning);
        }

        public void Analyze(IEnumerable<Procedure> procedures)
        {
            foreach (var proc in procedures)
                Analyze(proc);
        }

        public void PromoteBlocksToProcedures(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                PromoteBlockToProcedure(block);
            }
        }

        public void CloneBlocksIntoOtherProcedures(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
                CloneBlockIntoOtherProcedures(block);
        }

        private void PromoteBlockToProcedure(Block block)
        {
            var procOld = block.Procedure;
            var procNew = PromoteBlockToProcedureEntry(block, procOld);
            Promote(block, procOld, procNew);
        }

        public void CloneBlockIntoOtherProcedures(Block block)
        {
            foreach (Block pred in block.Pred.Where(p => p.Procedure != block.Procedure))
            {
                Debug.Print("Cloning {0} to {1}", block.Name, pred.Procedure);
                var clonedBlock = new BlockCloner(block, pred.Procedure, prog.CallGraph).Execute();
                ReplaceSuccessorsWith(pred, block, clonedBlock);
            }
        }

        private void PromoteInboundEdges(Block block)
        {
            foreach (var p in block.Pred)
            {
                if (p.Procedure == block.Procedure)
                    continue;
            }
        }

        public void FixInboundEdges(Block blockToPromote, Procedure procOld, Procedure procNew)
        {
            // Get all blocks that are from "outside" blocks.
            var inboundBlocks = blockToPromote.Pred.Where(p => p.Procedure != procOld && p.Procedure != procNew);
            foreach (var inboundBlock in inboundBlocks)
            {
                var thunkName = inboundBlock.Name + Scanner.CallRetThunkSuffix;
                var callRetThunkBlock = CreateCallRetThunk(thunkName,inboundBlock.Procedure, procNew);
                ReplaceSuccessorsWith(inboundBlock, blockToPromote, callRetThunkBlock);
            }
        }

        private void ReplaceSuccessorsWith(Block block, Block blockOld, Block blockNew)
        {
            for (int s = 0; s < block.Succ.Count; ++s)
            {
                if (block.Succ[s] == blockOld)
                    block.Succ[s] = blockNew;
            }
        }

        private Block CreateCallRetThunk(string blockName, Procedure proc, Procedure procNew)
        {
            var callRetThunkBlock = proc.AddBlock(blockName);
            callRetThunkBlock.Statements.Add(0, new CallInstruction(
                    new ProcedureConstant(prog.Architecture.PointerType, procNew),
                    new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
            prog.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
            callRetThunkBlock.Statements.Add(0, new ReturnInstruction());
            proc.ControlGraph.AddEdge(callRetThunkBlock, proc.ExitBlock);
            return callRetThunkBlock;
        }

        public void Promote(Block blockToPromote, Procedure procOld, Procedure procNew)
        {
            var movedBlocks = new HashSet<Block>(
                new DfsIterator<Block>(procOld.ControlGraph)
                .PreOrder(blockToPromote)
                .Where(b => b.Procedure == blockToPromote.Procedure));
            movedBlocks.Remove(procOld.ExitBlock);

            ReplaceJumpsToExitBlock(movedBlocks, procOld, procNew);

            var replacer = new IdentifierReplacer(procNew.Frame);
            foreach (var b in movedBlocks)
            {
                procOld.RemoveBlock(b);
                procNew.AddBlock(b);
                b.Procedure = procNew;
                foreach (var stm in b.Statements)
                {
                    stm.Instruction = replacer.ReplaceIdentifiers(stm.Instruction);
                }
            }
            procNew.ControlGraph.AddEdge(procNew.EntryBlock, blockToPromote);

            FixInboundEdges(blockToPromote, procOld, procNew);
        }

        private void FixOutboundEdges(Block b, HashSet<Block> movedBlocks, Procedure procOld)
        {
            for (int i = 0; i < b.Succ.Count; ++i)
            {
                var s = b.Succ[i];
                if (!movedBlocks.Contains(s) && s.Procedure.ExitBlock != s)
                {
                    var thunk = CreateCallRetThunk(b, b.Procedure, s.Procedure);
                    Block.ReplaceJumpsTo(s, thunk);
                    procOld.ControlGraph.AddEdge(thunk, b.Procedure.ExitBlock);
                }
            }
        }

        private void ReplaceJumpsToExitBlock(HashSet<Block> movedBlocks, Procedure procOld, Procedure procNew)
        {
            foreach (var movedBlock in movedBlocks)
            {
                for (int i = 0; i < movedBlock.Succ.Count; ++i)
                {
                    if (movedBlock.Succ[i] == procOld.ExitBlock)
                    {
                        movedBlock.Succ[i] = procNew.ExitBlock;
                        procOld.ExitBlock.Pred.Remove(movedBlock);
                        procNew.ExitBlock.Pred.Add(movedBlock);
                    }
                }
            }
        }

        public class IdentifierReplacer : InstructionTransformer, StorageVisitor<Identifier>
        {
            private Frame frame;
            private Dictionary<Identifier, Identifier> mapIds;
            private Identifier id;

            public IdentifierReplacer(Frame frame)
            {
                this.frame = frame;
                this.mapIds = new Dictionary<Identifier, Identifier>();
            }

            public Instruction ReplaceIdentifiers(Instruction instr)
            {
                return instr.Accept(this);
            }

            public override Expression VisitIdentifier(Identifier id)
            {
                this.id = id;
                Identifier idNew;
                if (!mapIds.TryGetValue(id, out idNew))
                {
                    idNew = id.Storage.Accept(this);
                    mapIds.Add(id, idNew);
                }
                return idNew;
            }

            public Identifier VisitFlagGroupStorage(FlagGroupStorage flags)
            {
                return frame.EnsureFlagGroup(flags.FlagGroupBits, flags.Name, id.DataType);
            }

            public Identifier VisitFpuStackStorage(FpuStackStorage fpu)
            {
                return frame.EnsureFpuStackVariable(fpu.FpuStackOffset, id.DataType);
            }

            public Identifier VisitRegisterStorage(RegisterStorage reg)
            {
                return frame.EnsureRegister(reg);
            }

            public Identifier VisitMemoryStorage(MemoryStorage mem)
            {
                return frame.Memory;
            }

            public Identifier VisitStackArgumentStorage(StackArgumentStorage arg)
            {
                return frame.EnsureStackArgument(arg.StackOffset, arg.DataType);
            }

            public Identifier VisitStackLocalStorage(StackLocalStorage loc)
            {
                return frame.EnsureStackLocal(loc.StackOffset, loc.DataType);
            }

            public Identifier VisitTemporaryStorage(TemporaryStorage tmp)
            {
                return frame.CreateTemporary(id.DataType);
            }

            public Identifier VisitSequenceStorage(SequenceStorage seq)
            {
                var idSeq = id;
                var newHead = (Identifier) VisitIdentifier(seq.Head);
                var newTail = (Identifier) VisitIdentifier(seq.Tail);
                return frame.EnsureSequence(newHead, newTail, idSeq.DataType);
            }

            public Identifier VisitOutArgumentStorage(OutArgumentStorage ost)
            {
                var idOut = id;
                return frame.EnsureOutArgument((Identifier) VisitIdentifier(ost.OriginalIdentifier), idOut.DataType);
            }
        }

    }
}
