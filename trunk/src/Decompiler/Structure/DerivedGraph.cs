/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public class DerivedGraph
    {
        private List<IntNode> intervals = new List<IntNode>();	// intervals of derived graph

        public StructureNode cfg;				// head of derived graph
        public int Count;			    // number of nodes in this graph

        public List<IntNode> Intervals
        {
            get { return intervals; }
        }

        public void DisplayIntervals()
        {
            foreach (IntNode curInt in intervals)
            {
                Console.Out.WriteLine("   Interval #{0}:", curInt.Ident());

                for (int j = 0; j < curInt.Nodes.Count; j++)
                {
                    StructureNode curNode = curInt.Nodes[j];
                    Console.Out.Write("      ");
                    Console.Out.Write((curNode.BlockType != bbType.intNode) ? "BB node #" : "IntNode #");
                    Console.Out.WriteLine((curNode.BlockType != bbType.intNode) ? curNode.Order : curNode.Ident());
                }
            }
        }

    }
}
