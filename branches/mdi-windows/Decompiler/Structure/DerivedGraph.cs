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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Structure
{
    public class DerivedGraph
    {
        private StructureNode entry;
        private List<Interval> intervals;
        private DirectedGraph<StructureNode> graph;

        public StructureNode Entry				// head of derived graph
        {
            get { return entry; }
        }

        public DerivedGraph(DirectedGraph<StructureNode> graph, StructureNode entry, List<Interval> intervals)
        {
            this.graph = graph;
            this.entry = entry;
            this.intervals = intervals;
        }
        
        public List<Interval> Intervals
        {
            get { return intervals; }
        }


        public void Dump()
        {
            Write(Console.Out);
        }

        public void Write(TextWriter writer)
        {
            foreach (StructureNode node in new DfsIterator<StructureNode>(graph).PreOrder(entry))
            {
                writer.WriteLine("   Node {0}", node);
                foreach (StructureNode succ in graph.Successors(node))
                {
                    writer.Write("     Succ: ");
                    writer.Write(" ");
                    writer.Write(succ.Name);
                    writer.WriteLine();
                }
            }
            foreach (Interval interval in intervals)
            {
                writer.WriteLine("   Interval #{0}: {1}", interval.Ident(), interval.ToString());
            }
        }

        public DirectedGraph<StructureNode> Graph
        {
            get { return graph; }
        }
    }
}
