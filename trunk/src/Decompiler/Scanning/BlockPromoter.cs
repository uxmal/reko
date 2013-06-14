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

        public BlockPromoter(Program program, Block blockToPromote, Procedure procNew)
        {
            this.program = program;
            this.blockToPromote = blockToPromote;
            this.procOld = blockToPromote.Procedure;
            this.procNew = procNew;
            this.replacer = new IdentifierReplacer(procNew.Frame);
        }

        public void Promote()
        {
            var movedBlocks = new HashSet<Block>(
                new DfsIterator<Block>(procOld.ControlGraph).PreOrder(blockToPromote));
            movedBlocks.Remove(procOld.ExitBlock);

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
            FixInboundEdges();

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
            var callRetThunkBlock = procOld.AddBlock(blockToPromote + "_tmp");
            callRetThunkBlock.Statements.Add(0, new CallInstruction(
                new ProcedureConstant(program.Architecture.PointerType, procNew),
                new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
            program.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
            callRetThunkBlock.Statements.Add(0, new ReturnInstruction());

            Block.ReplaceJumpsTo(blockToPromote, callRetThunkBlock);

            procOld.ControlGraph.AddEdge(callRetThunkBlock, procOld.ExitBlock);
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

        private bool BlockInDifferentProcedure(Block block, Procedure proc)
        {
            if (block.Procedure == null)
                throw new InvalidOperationException("Blocks must always be associated with a procedure.");
            Debug.Print("{0} should be promoted: {1}", block.Name, block.Procedure != proc);
            return (block.Procedure != proc);
        }

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
