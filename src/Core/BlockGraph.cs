#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    public class BlockGraph : DirectedGraph<Block>
    {
        private IList<Block> blocks;

        public BlockGraph(IList<Block> blocks)
        {
            this.blocks = blocks;
        }

        #region DirectedGraph<Block> Members

        public ICollection<Block> Predecessors(Block node)
        {
            return node.Pred;
        }

        public ICollection<Block> Successors(Block node)
        {
            return node.Succ;
        }

        public IList<Block> Blocks { get { return blocks; } }
        ICollection<Block> DirectedGraph<Block>.Nodes { get { return blocks; } }

        public void AddEdge(Block nodeFrom, Block nodeTo)
        {
            nodeFrom.Succ.Add(nodeTo);
            nodeTo.Pred.Add(nodeFrom);
        }

        public void RemoveEdge(Block nodeFrom, Block nodeTo)
        {
            if (nodeFrom.Succ.Contains(nodeTo) && nodeTo.Pred.Contains(nodeFrom))
            {
                nodeFrom.Succ.Remove(nodeTo);
                nodeTo.Pred.Remove(nodeFrom);
            }
        }

        public bool ContainsEdge(Block from, Block to)
        {
            return from.Succ.Contains(to);
        }

        #endregion
    }
}
