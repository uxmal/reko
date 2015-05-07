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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    public class ProcedureStructureBuilder
    {
        private Procedure proc;
        private List<StructureNode> nodeList;
        private Dictionary<Block, StructureNode> blockNodes;
        private ProcedureStructure curProc;

        public ProcedureStructureBuilder(Procedure proc)
        {
            this.proc = proc;
            this.nodeList = new List<StructureNode>();
            this.blockNodes = new Dictionary<Block, StructureNode>();
        }

        public ProcedureStructure Build()
        {
            BuildNodes();
            DefineEdges();

            curProc = CreateProcedureStructure();
            SetTimeStamps();
            BuildDerivedSequences(curProc);
            return curProc;
        }

        public void BuildNodes()
        {
            int bId = 0;
            var iterator = new DfsIterator<Block>(proc.ControlGraph);
            foreach (Block b in iterator.PreOrder(proc.EntryBlock))
            {
                var cfgNode = new StructureNode(b, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(b, cfgNode);
            }
            if (!blockNodes.ContainsKey(proc.ExitBlock))
            {
                var cfgNode = new StructureNode(proc.ExitBlock, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(proc.ExitBlock, cfgNode);
            }
        }

        public void DefineEdges()
        {
            foreach (StructureNode curNode in nodeList)
            {
                foreach (Block s in curNode.Block.Succ)
                {
                    curNode.AddEdgeTo(blockNodes[s]);
                }
            }
        }

        public ProcedureStructure CreateProcedureStructure()
        {
            List<StructureNode> nodes = new List<StructureNode>();
            foreach (StructureNode node in blockNodes.Values)
            {
                nodes.Add(node);
            }

            var newProc = new ProcedureStructure(proc.Name, nodes);
            newProc.EntryNode = blockNodes[proc.EntryBlock];
            newProc.ExitNode = blockNodes[proc.ExitBlock];
            this.curProc = newProc;
            return newProc;
        }


        // Build the sequence of derived graphs for each procedure
        public void BuildDerivedSequences(ProcedureStructure curProc)
        {
            DerivedSequenceBuilder d = new DerivedSequenceBuilder(curProc);
        }



        /// <summary>
        /// Assign nodes their PostOrder and reverse PostOrder numbers.
        /// </summary>
        /// <param name="curProc"></param>
        public void SetTimeStamps()
        {
            //do the time stamping used for loop structuring 
            int time = 1;
            List<StructureNode> order = curProc.Ordering;

            // set the parenthesis for the nodes as well as setting
            // the post-order ordering between the nodes
            curProc.EntryNode.SetLoopStamps(ref time, order);

            // set the reverse parenthesis for the nodes
            time = 1;
            curProc.EntryNode.SetRevLoopStamps(ref time, new HashSet<StructureNode>());
            Debug.Assert(curProc.ExitNode != null);
        }

        public DominatorGraph<StructureNode> AnalyzeGraph()
        {
            var graph = new StructureGraphAdapter(curProc.Nodes);
            var reverseGraph = new ReverseGraph(curProc.Nodes);
            var infiniteLoops = FindInfiniteLoops(graph, curProc.EntryNode);
            AddPseudoEdgeFromInfiniteLoopsToExitNode(graph, infiniteLoops, curProc.ExitNode, reverseGraph);

            var pdg = new DominatorGraph<StructureNode>(reverseGraph, curProc.ExitNode);
            SetImmediatePostDominators(pdg);

            RemovePseudoEdgeFromInfiniteLoopsToExitNode(graph, infiniteLoops, curProc.ExitNode);
            return pdg;
        }

        private void SetImmediatePostDominators(DominatorGraph<StructureNode> reverseDomGraph)
        {
            foreach (StructureNode node in curProc.Nodes)
            {
                node.ImmPDom = reverseDomGraph.ImmediateDominator(node);
            }
        }

        public class ReverseGraph : DirectedGraph<StructureNode>
        {
            private ICollection<StructureNode> nodes;
            private List<Edge> pseudoEdges;
            
            private struct Edge
            {
                public Edge(StructureNode from, StructureNode to) { this.From = from; this.To = to; }

                public StructureNode From;
                public StructureNode To;
            }
            public ReverseGraph(ICollection<StructureNode> nodes)
            {
                this.nodes = nodes;
                this.pseudoEdges = new List<Edge>();
            }

            #region DirectedGraph<StructureNode> Members

            public ICollection<StructureNode> Predecessors(StructureNode node)
            {
                var preds = new List<StructureNode>(node.OutEdges);
                foreach (Edge e in pseudoEdges)
                {
                    if (e.To == node)
                        preds.Add(e.From);
                }
                return preds;
            }

            public ICollection<StructureNode> Successors(StructureNode node)
            {
                var succs = new List<StructureNode>(node.InEdges);
                foreach (Edge e in pseudoEdges)
                {
                    if (e.From == node)
                        succs.Add(e.To);
                }
                return succs;
            }

            public ICollection<StructureNode> Nodes
            {
                get { return nodes; }
            }

            public void AddEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                pseudoEdges.Add(new Edge(nodeFrom, nodeTo));
            }

            public void RemoveEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
        private void AddPseudoEdgeFromInfiniteLoopsToExitNode(
            DirectedGraph<StructureNode> graph,
            ICollection<StructureNode> infiniteLoops,
            StructureNode exitNode,
            DirectedGraph<StructureNode> reverseGraph)
        {
            foreach (StructureNode infLoopHeader in infiniteLoops)
            {
                Debug.Assert(!infLoopHeader.OutEdges.Contains(exitNode));
                reverseGraph.AddEdge(exitNode, infLoopHeader);
            }
        }

        private void RemovePseudoEdgeFromInfiniteLoopsToExitNode(DirectedGraph<StructureNode> graph, ICollection<StructureNode> infiniteLoops, StructureNode exitNode)
        {
            foreach (StructureNode infLoopHeader in infiniteLoops)
            {
                graph.RemoveEdge(infLoopHeader, exitNode);
            }
        }

        public ICollection<StructureNode> FindInfiniteLoops(DirectedGraph<StructureNode> graph, StructureNode entry)
        {
            List<StructureNode> infiniteLoopHeaders = new List<StructureNode>();
            SccFinder<StructureNode> finder = new SccFinder<StructureNode>(graph, delegate(IList<StructureNode> scc)
            {
                if (!IsInfiniteLoop(graph, scc))
                    return;

                var header = FindNodeWithHighestPostOrderNumber(scc);
                foreach (StructureNode tail in graph.Predecessors(header))
                {
                    if (scc.Contains(tail))
                    {
                        infiniteLoopHeaders.Add(tail);
                    }
                }
            });
            finder.Find(entry);
            return infiniteLoopHeaders;
        }

        private StructureNode FindNodeWithHighestPostOrderNumber(ICollection<StructureNode> scc)
        {
            StructureNode nodeHi = null;
            foreach (StructureNode node in scc)
            {
                if (nodeHi == null)
                    nodeHi = node;
                else if (node.Order > nodeHi.Order)
                    nodeHi = node;

            }
            return nodeHi;
        }

        private bool IsInfiniteLoop(DirectedGraph<StructureNode> graph, ICollection<StructureNode> scc)
        {
            var members = new HashSet<StructureNode>(scc);
            foreach (StructureNode node in scc)
            {
                foreach (StructureNode succ in graph.Successors(node))
                {
                    if (!members.Contains(succ))
                        return false;
                }
            }
            return true;
        }
    }
}