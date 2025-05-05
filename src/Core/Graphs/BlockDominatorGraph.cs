#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Maintains the dominator graph of the blocks in a procedure, along with
    /// an extramethod for determining the dominance relation between two 
    /// statements, which must always live in blocks.
    /// </summary>
   public class BlockDominatorGraph : DominatorGraph<Block>
   {
        /// <summary>
        /// Constructs a dominator graph for the basic  blocks in a procedure.
        /// </summary>
        /// <param name="graph">Control graph for the basic blocks.</param>
        /// <param name="entry">The entry basic block.</param>
        public BlockDominatorGraph(DirectedGraph<Block> graph, Block entry) : base(graph, entry)
        {
        }

        /// <summary>
        /// Determines whether the statement <paramref name="stmDom"/> dominates
        /// <paramref name="stm"/> strictly.
        /// </summary>
        /// <param name="stmDom">Statement that might be dominating.</param>
        /// <param name="stm">Statement that might be dominated.</param>
        /// <returns>True if <paramref name="stmDom"/> dominates <paramref name="stm"/>
        /// strictly.
        /// </returns>
        public bool DominatesStrictly(Statement stmDom, Statement stm)
       {
           if (stmDom == stm)
               return false;
           if (stmDom.Block == stm.Block)
           {
               foreach (Statement s in stmDom.Block.Statements)
               {
                   if (stmDom == s)
                       return true;
                   if (stm == s)
                       return false;
               }
               throw new ApplicationException("Impossible: both stmDom and stm should be in stmDom's block!");
           }
           else
           {
               return DominatesStrictly(stmDom.Block, stm.Block);
           }
       }
   }
}
