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
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    public class DerivedSequenceBuilder
    {
        // Build the derived sequence of graphs within a CFG
        public void BuildDerivedSequence(ProcedureStructure proc)
        {
            // Initialise the first graph in the sequence to be the CFG for the procedure
            DerivedGraph curGraph = new DerivedGraph();
            curGraph.cfg = proc.EntryNode;
            curGraph.Count = proc.Nodes.Count;
            proc.DerivedGraphs.Add(curGraph);
            IntervalBuilder i = new IntervalBuilder();

            // Continually process the current graph until it is the trivial graph (i.e. it has only one node)
            while (curGraph.Count != 1)
            {
                i.BuildIntervals(curGraph);

                // If the number of intervals found is equal to the number of nodes in the graph, then this
                // graph is irreducible and the generation of the derived sequence must be terminated here
                // to prevent infinite recursion.
                if (curGraph.Count == curGraph.Intervals.Count)
                    break;


                DerivedGraph nextGraph = new DerivedGraph();
                nextGraph.Count = curGraph.Intervals.Count;
                BuildNextOrderGraph(curGraph, nextGraph);

                proc.DerivedGraphs.Add(nextGraph);

                curGraph = nextGraph;
            }

            // If the generation of the derived sequence resulted in the trivial graph, then this trivial
            // graph still needs its (trivial) singleton set of intervals built
            Debug.Assert(curGraph == proc.DerivedGraphs[proc.DerivedGraphs.Count - 1]);
            if (curGraph.Count == 1)
            {
                i.BuildIntervals(curGraph);
            }
        }

        // Pre: the number of nodes in the current graph must be greater than the number of intervals 
        //		  determined for the current graph
        // Build the next order graph from the current derived graph
        private void BuildNextOrderGraph(DerivedGraph curGraph, DerivedGraph nextGraph)
        // Pre: the number of intervals in the current graph is greater than the number of nodes
        //		  in the current graph. 
        // Post: the next order graph has been built
        {
            Debug.Assert(curGraph.Count > curGraph.Intervals.Count);

            // Set the first interval in the list of intervals for the current graph to be the head
            // of the next order graph
            nextGraph.cfg = curGraph.Intervals[0];

            // Process the intervals of the current graph (which are the nodes in the next order graph)
            // to define the edges between them
            foreach (IntNode curInt in curGraph.Intervals)
            {
                // Process each node in the current interval to see if if has an outedge to another interval
                foreach (StructureNode curNode in curInt.Nodes)
                {
                    // Examine each out edge of the current node within the interval
                    foreach (StructureNode child in curNode.OutEdges)
                    {
                        // If the outedge under examination leads outside the current interval, then add it as
                        // an edge to this interval. Also add the corresponding in edges to the child
                        if (child.Interval != curInt)
                        {
                            curInt.AddEdgeTo(child.Interval);
                            child.Interval.AddEdgeFrom(curInt);
                        }
                    }
                }
            }
        }
    }

    public class DerivedSequenceBuilder2
    {
        private List<DerivedGraph> graphs;
        private IntervalBuilder ib;

        public DerivedSequenceBuilder2(ProcedureStructure proc)
        {
            graphs = proc.DerivedGraphs;
            ib = new IntervalBuilder();
            DerivedGraph gr = BuildDerivedGraph(proc.Nodes, proc.EntryNode);
            graphs.Add(gr);
            while (gr.Graph.Nodes.Count > 1)
            {
                DerivedGraph newGr = BuildNextOrderGraph2(gr);
                if (newGr.Graph.Nodes.Count == gr.Graph.Nodes.Count)
                    return;
                graphs.Add(newGr);
                gr = newGr;
            }
        }

        private DerivedGraph BuildNextOrderGraph2(DerivedGraph gr)
        {
            DirectedGraph<StructureNode> newGraph = new DirectedGraphImpl<StructureNode>();
            StructureNode newEntry = gr.Intervals[0];

            foreach (IntNode interval in gr.Intervals)
            {
                newGraph.Nodes.Add(interval);
            }

            foreach (IntNode interval in gr.Intervals)
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

        private class StructureGraphAdapter : DirectedGraph<StructureNode>
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
}
