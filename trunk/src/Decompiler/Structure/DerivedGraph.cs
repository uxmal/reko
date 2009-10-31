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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Structure
{
    public class DerivedGraph
    {
        public StructureNode cfg;				// head of derived graph
        public int Count;			    // number of nodes in this graph
        private List<IntNode> intervals;
        private DirectedGraph<StructureNode> graph;

        public DerivedGraph()
        {
            this.intervals = new List<IntNode>();
            this.graph = new DirectedGraphImpl<StructureNode>();
        }

        public DerivedGraph(DirectedGraph<StructureNode> graph, StructureNode entry, List<IntNode> intervals)
        {
            this.graph = graph;
            this.cfg = entry;
            this.intervals = intervals;
        }
        
        public List<IntNode> Intervals
        {
            get { return intervals; }
        }


        public void Dump()
        {
            Write(Console.Out);
        }

        public void Write(TextWriter writer)
        {
            foreach (IntNode curInt in intervals)
            {
                writer.WriteLine("   Interval #{0}:", curInt.Ident());

                for (int j = 0; j < curInt.Nodes.Count; j++)
                {
                    StructureNode curNode = curInt.Nodes[j];
                    writer.Write("      ");
                    writer.Write((curNode.BlockType != bbType.intNode) ? "BB node #" : "IntNode #");
                    writer.WriteLine((curNode.BlockType != bbType.intNode) ? curNode.Order : curNode.Ident());
                }
            }
        }

        public DirectedGraph<StructureNode> Graph
        {
            get { return graph; }
        }
    }
}
