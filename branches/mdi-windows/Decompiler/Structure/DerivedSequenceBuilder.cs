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
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    /// <summary>
    /// Builds the derived graph sequence. At each step, the intervals of the graph are found. A new graph is then
    /// built, where the vertices are the intervals and the edges consist of transitions between different intervals.
    /// </summary>
    public class DerivedSequenceBuilder
    {
        private List<DerivedGraph> graphs;
        private IntervalBuilder ib;

        public DerivedSequenceBuilder(ProcedureStructure proc)
        {
            graphs = proc.DerivedGraphs;
            ib = new IntervalBuilder();
            DerivedGraph gr = BuildDerivedGraph(proc.Nodes, proc.EntryNode);
            graphs.Add(gr);
            while (gr.Graph.Nodes.Count > 1)
            {
                DerivedGraph newGr = BuildNextOrderGraph(gr);
                if (newGr.Graph.Nodes.Count == gr.Graph.Nodes.Count)
                    return;
                graphs.Add(newGr);
                gr = newGr;
            }
        }

        private DerivedGraph BuildNextOrderGraph(DerivedGraph gr)
        {
            DirectedGraph<StructureNode> newGraph = new DirectedGraphImpl<StructureNode>();
            StructureNode newEntry = gr.Intervals[0];

            foreach (Interval interval in gr.Intervals)
            {
                newGraph.Nodes.Add(interval);
            }

            foreach (Interval interval in gr.Intervals)
            {
                foreach (StructureNode node in interval.Nodes)
                {
                    foreach (StructureNode succ in gr.Graph.Successors(node))
                    {
                        if (succ.Interval != interval && !newGraph.ContainsEdge(interval, succ.Interval))
                        {
                            newGraph.AddEdge(interval, succ.Interval);
                        }
                    }
                }
            }
            return new DerivedGraph(newGraph, newEntry, ib.BuildIntervals(newGraph, newEntry));
        }

        private DerivedGraph BuildDerivedGraph(ICollection<StructureNode> nodes, StructureNode entryNode)
        {
            DirectedGraph<StructureNode> graph = new StructureGraphAdapter(nodes);
            return new DerivedGraph(graph, entryNode, ib.BuildIntervals(graph, entryNode));
        }

        public List<DerivedGraph> Graphs
        {
            get { return graphs; }
        }
    }

    /// <summary>
    /// Wraps the graph of structurenodes in an interface so generic algorithms can use it.
    /// </summary>
        public class StructureGraphAdapter : DirectedGraph<StructureNode>
        {
            private ICollection<StructureNode> nodes;

            public StructureGraphAdapter(ICollection<StructureNode> nodes)
            {
                this.nodes = nodes;
            }

            public ICollection<StructureNode> Predecessors(StructureNode node)
            {
                return node.InEdges;
            }

            public ICollection<StructureNode> Successors(StructureNode node)
            {
                return node.OutEdges;
            }

            public ICollection<StructureNode> Nodes
            {
                get { return nodes; }
            }

            public void AddEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                nodeFrom.OutEdges.Add(nodeTo);
                nodeTo.InEdges.Add(nodeFrom);
            }

            public void RemoveEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                nodeFrom.OutEdges.Remove(nodeTo);
                nodeTo.InEdges.Remove(nodeFrom);
            }

            public bool ContainsEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                return nodeFrom.OutEdges.Contains(nodeTo);
            }
    }
}
