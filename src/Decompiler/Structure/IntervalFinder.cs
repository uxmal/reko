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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
    public class IntervalBuilder
    {
        private int intervalID = 0;

        public void BuildIntervals(DerivedGraph derGraph)
        {
            if (derGraph == null)
                throw new ArgumentNullException("derGraph");
            if (derGraph.cfg == null)
                throw new ArgumentException("cfg graph must be non-null", "derGraph");

            List<IntNode> intSeq = derGraph.Intervals;	// The sequence of intervals in this graph
            WorkList<StructureNode> headerSeq = new WorkList<StructureNode>();	// The sequence of interval header nodes
            List<StructureNode> beenInH = new List<StructureNode>();	// The set of nodes that have been in the above sequence at some stage

            headerSeq.Add(derGraph.cfg);

            beenInH.Add(derGraph.cfg);

            // Keep processing the header sequence until it is empty
            StructureNode header;
            while (headerSeq.GetWorkItem(out header))
            {
                // Remove the head of the headers sequence and set it to be the head of a new interval
                IntNode newInt = new IntNode(intervalID++, header);

                // Process each succesive node in the interval until no more nodes can be added to the interval.
                for (int i = 0; i < newInt.Nodes.Count; i++)
                {
                    StructureNode curNode = newInt.Nodes[i];

                    // Process each child of the current node
                    for (int j = 0; j < curNode.OutEdges.Count; j++)
                    {
                        StructureNode succ = curNode.OutEdges[j];

                        // Only further consider the current child if it isn't already in the interval
                        if (!newInt.Nodes.Contains(succ))
                        {
                            // If the current child has all its parents
                            // inside the interval, then add it to the interval. Remove it from the header
                            // sequence if it is on it.
                            if (SubSetOf(succ.InEdges, newInt))
                            {
                                newInt.AddNode(succ);
                                headerSeq.Remove(succ);
                            }

                            // Otherwise, add it to the header sequence if it hasn't already been in it.
                            else if (!beenInH.Contains(succ))
                            {
                                headerSeq.Add(succ);
                                beenInH.Add(succ);
                            }
                        }
                    }
                }

                // Add the new interval to the sequence of intervals
                intSeq.Add(/*static_cast<CFGNode>*/(newInt));
            }
        }

        private bool SubSetOf(List<StructureNode> iEdges, IntNode newInt)
        {
            for (int i = 0; i < iEdges.Count; i++)
                if (iEdges[i].Interval != newInt)
                    return false;
            return true;
        }
    }
}

