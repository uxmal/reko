#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Structure
{
    public class DerivedGraph
    {
        public DerivedGraph(DirectedGraph<StructureNode> graph, StructureNode entry, List<Interval> intervals)
        {
            this.Graph = graph;
            this.Entry = entry;
            this.Intervals = intervals;
        }

        public StructureNode Entry { get; private set; }			// head of derived graph
        public List<Interval> Intervals { get; private set; }
        public DirectedGraph<StructureNode> Graph { get; private set; }
        
        public void Dump()
        {
            Write(Console.Out);
        }

        public void Write(TextWriter writer)
        {
            foreach (StructureNode node in new DfsIterator<StructureNode>(Graph).PreOrder(Entry))
            {
                writer.WriteLine("   Node {0}", node);
                foreach (StructureNode succ in Graph.Successors(node))
                {
                    writer.Write("     Succ: ");
                    writer.Write(" ");
                    writer.Write(succ.Name);
                    writer.WriteLine();
                }
            }
            foreach (Interval interval in Intervals)
            {
                writer.WriteLine("   Interval #{0}: {1}", interval.Number, interval.ToString());
            }
        }
    }
}
