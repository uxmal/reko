#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
    public class PromoteBlockWorkItem : WorkItem
    {
        public Address Address;
        public Block Block;
        public Procedure ProcNew;
        public IScanner Scanner;

        public override void Process()
        {
            var movedBlocks = new HashSet<Block>();
            var stack = new Stack<IEnumerator<Block>>();
            stack.Push(new Block[] { Block }.Cast<Block>().GetEnumerator());
            while (stack.Count != 0)
            {
                var e = stack.Pop();
                if (!e.MoveNext())
                    continue;
                var b = e.Current;
                if (b.Procedure == ProcNew || b == b.Procedure.ExitBlock || b.Procedure.EntryBlock.Succ[0] == b)
                    continue;

                b.Procedure = ProcNew;
                movedBlocks.Add(b);
                stack.Push(b.Succ.GetEnumerator());
            }
            FixInboundEdges(movedBlocks);
        }

        private void FixInboundEdges(HashSet<Block> movedBlocks)
        {
            foreach (var block in movedBlocks)
            {
                FixInboundEdges(block);
                if (block == Block)
                {
                    // First block.
                }
                else
                {
                }
            }
        }

        public void FixInboundEdges(Block blockToPromote)
        {
            // Get all blocks that are from "outside" blocks.
            var inboundBlocks = blockToPromote.Pred.Where(p => p.Procedure != ProcNew).ToArray();
            foreach (var inboundBlock in inboundBlocks)
            {
                var callRetThunkBlock = Scanner.CreateCallRetThunk(inboundBlock.Procedure, ProcNew);
                ReplaceSuccessorsWith(inboundBlock, blockToPromote, callRetThunkBlock);
            }
            foreach (var p in inboundBlocks)
            {
                blockToPromote.Pred.Remove(p);
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
    }
}
