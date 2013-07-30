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
using Decompiler.Core.Lib;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Promotes a block to a procedure by moving all of the blocks that are reachable from the block into a new
    /// procedure.
    /// </summary>
    public class BlockPromoter
    {
        private Program program;
        private Block blockToPromote;
        private Procedure procOld;
        private Procedure procNew;
        private IdentifierReplacer replacer;

        public BlockPromoter(Program program)
        {
            this.program = program;
        }

        public Block MaybePromote(Block block, Address addrStart, Procedure proc)
        {
            if (BlockInDifferentProcedure(block, proc))
            {
                if (!BlockIsEntryBlock(block))
                {
                    Debug.Print("Block {0} (proc {1}) is not entry block", block, block.Procedure);
                    block = PromoteBlock(block, addrStart);
                }
            }
            return block;
        }

        public bool BlockInDifferentProcedure(Block block, Procedure proc)
        {
            if (block.Procedure == null)
                throw new InvalidOperationException("Blocks must always be associated with a procedure.");
            Debug.Print("{0} should be promoted: {1}", block.Name, block.Procedure != proc);
            return (block.Procedure != proc);
        }

        private bool BlockIsEntryBlock(Block block)
        {
            return
                block.Pred.Count == 1 &&
                block.Pred[0] == block.Procedure.EntryBlock;
        }

        /// <summary>
        /// Promotes a block to being the entry of a procedure.
        /// </summary>
        /// <param name="block">Block to promote</param>
        /// <param name="addrStart">Address at which the block starts</param>
        /// <param name="proc">The procedure from which the block is called.</param>
        /// <returns></returns>
        public Block PromoteBlock(Block block, Address addrStart)
        {
            this.blockToPromote = block ;
            this.procOld = blockToPromote.Procedure;
            if (!program.Procedures.TryGetValue(addrStart, out procNew))
            {
                procNew = Procedure.Create(addrStart, program.Architecture.CreateFrame());
                procNew.Frame.ReturnAddressSize = procOld.Frame.ReturnAddressSize;
                procNew.Characteristics = new ProcedureCharacteristics(procOld.Characteristics);
                program.Procedures.Add(addrStart, procNew);
                program.CallGraph.AddProcedure(procNew);
            }
            Promote();
            return block;
        }

        public void Promote()
        {
            FixInboundEdges();

            var movedBlocks = new HashSet<Block>(
                new DfsIterator<Block>(procOld.ControlGraph)
                .PreOrder(blockToPromote)
                .Where(b => b.Procedure == blockToPromote.Procedure));
            movedBlocks.Remove(procOld.ExitBlock);

            this.replacer = new IdentifierReplacer(procNew.Frame);
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

            ReplaceJumpsToExitBlock(movedBlocks);
            foreach (var b in movedBlocks)
            {
                FixOutboundEdges(b);
            }
            procNew.ControlGraph.AddEdge(procNew.EntryBlock, blockToPromote);
        }

        private void ReplaceJumpsToExitBlock(HashSet<Block> movedBlocks)
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

        /// <summary>
        /// Patches all inbound edges, replacing jumps with call/return sequences, and 
        /// calls
        /// </summary>
        public void FixInboundEdges()
        {
            CallRetThunkBlock = FixInboundEdges(blockToPromote, procOld, procNew);
        }

        public Block FixInboundEdges(Block blockToPromote, Procedure procOld, Procedure procNew)
        {
            var callRetThunkBlock = CreateCallRetThunk(blockToPromote, procOld, procNew);
            Block.ReplaceJumpsTo(blockToPromote, callRetThunkBlock);
            procOld.ControlGraph.AddEdge(callRetThunkBlock, procOld.ExitBlock);
            return callRetThunkBlock;
        }

        private void FixOutboundEdges(Block b)
        {
            for (int i = 0; i < b.Succ.Count; ++i)
            {
                var s= b.Succ[i];
                if (b.Procedure != s.Procedure)
                {
                    var thunk = CreateCallRetThunk(b, b.Procedure, s.Procedure);
                    Block.ReplaceJumpsTo(s, thunk);
                    procOld.ControlGraph.AddEdge(thunk, b.Procedure.ExitBlock);
                }
            }
        }

        private Block CreateCallRetThunk(Block block, Procedure procOld, Procedure procNew)
        {
            var callRetThunkBlock = procOld.AddBlock(block.Name + Scanner.CallRetThunkSuffix);
                callRetThunkBlock.Statements.Add(0, new CallInstruction(
                        new ProcedureConstant(program.Architecture.PointerType, procNew),
                        new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
                    program.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
                    callRetThunkBlock.Statements.Add(0, new ReturnInstruction());
            return callRetThunkBlock;
        }

        /*
            if (BlockInDifferentProcedure(block, proc))
            {
                if (!BlockIsEntryBlock(block))
                {
                    Debug.Print("Block {0} (proc {1}) is not entry block", block, block.Procedure);
                    if (IsLinearReturning(block))
                    {
                        Debug.Print("Cloning {0} to {1}", block.Name, proc);
                        block = new BlockCloner(block, proc, program.CallGraph).Execute();
                    }
                    else
                    {
                        block = PromoteBlock(block, addrStart, proc);
                    }
                }
            }
         
         */

        public Block CallRetThunkBlock { get; private set; }

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
                return frame.EnsureOutArgument((Identifier)VisitIdentifier(ost.OriginalIdentifier), idOut.DataType);
            }
        }
    }
}
