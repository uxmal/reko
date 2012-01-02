#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
        Block blockToPromote;
        Procedure procOld;
        Procedure procNew;
        private IProcessorArchitecture arch;

        public BlockPromoter(Block blockToPromote, Procedure procNew, IProcessorArchitecture arch)
        {
            this.blockToPromote = blockToPromote;
            this.procOld = blockToPromote.Procedure;
            this.procNew = procNew;
            this.arch = arch;
        }

        public void Promote()
        {
            var movedBlocks = new HashSet<Block>();
            foreach (var b in new DfsIterator<Block>(procOld.ControlGraph).PreOrder(blockToPromote))
            {
                movedBlocks.Add(b);
            }
            movedBlocks.Remove(procOld.ExitBlock);

            foreach (var b in movedBlocks)
            {
                procOld.RemoveBlock(b);
                procNew.AddBlock(b);
                b.Procedure = procNew;
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

        private void FixInboundEdges()
        {
            CallRetThunkBlock = procOld.AddBlock(blockToPromote+ "_tmp");
            CallRetThunkBlock.Statements.Add(0, new CallInstruction(
                new ProcedureConstant(arch.PointerType, procNew),
                new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
            CallRetThunkBlock.Statements.Add(0, new ReturnInstruction());

            Block.ReplaceJumpsTo(blockToPromote, CallRetThunkBlock);

            procOld.ControlGraph.AddEdge(CallRetThunkBlock, procOld.ExitBlock);
        }

        public Block CallRetThunkBlock { get; private set; }
    }
}
