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
            foreach (Block b in proc.RpoBlocks)
            {
                StructureNode cfgNode = new StructureNode(b, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(b, cfgNode);
            }
            if (!blockNodes.ContainsKey(proc.ExitBlock))
            {
                StructureNode cfgNode = new StructureNode(proc.ExitBlock, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(proc.ExitBlock, cfgNode);
            }
        }

        private void DebugBuildNodes()
        {
            foreach (StructureNode newNode in nodeList)
            {
                List<Statement> ins = newNode.Instructions;
                Debug.Write("Block #" + newNode.Ident() + " is of type " + newNode.BlockType);
                Debug.WriteLine(" and contains:");

                for (int i = 0; i < ins.Count; i++)
                {
                    Console.Error.WriteLine("\t");
                    Console.Error.WriteLine(ins[i].ToString());
                }
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

            ProcedureStructure newProc = new ProcedureStructure(proc.Name, nodes);
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
            curProc.EntryNode.SetRevLoopStamps(ref time, new HashedSet<StructureNode>());

            // do the ordering of nodes within the reverse graph 
            List<StructureNode> rorder = curProc.ReverseOrdering;
            Debug.Assert(curProc.ExitNode != null);
            curProc.ExitNode.SetRevOrder(rorder, new HashedSet<StructureNode>());
        }

        public void AnalyzeGraph()
        {
            StructureGraphAdapter graph = new StructureGraphAdapter(curProc.Nodes);
            ICollection<StructureNode> infiniteLoops = FindInfiniteLoops(graph, curProc.EntryNode);
            AddPseudoEdgeFromInfiniteLoopsToExitNode(graph, infiniteLoops, curProc.ExitNode);

            // (re)set the reverse ordering of the nodes.
            curProc.ReverseOrdering.Clear();
            curProc.ExitNode.SetRevOrder(curProc.ReverseOrdering, new HashedSet<StructureNode>());

            curProc.Dump();

            PostDominatorGraph g = new PostDominatorGraph(curProc);
            g.FindImmediatePostDominators();

            RemovePseudoEdgeFromInfiniteLoopsToExitNode(graph, infiniteLoops, curProc.ExitNode);
        }

        private void AddPseudoEdgeFromInfiniteLoopsToExitNode(DirectedGraph<StructureNode> graph, ICollection<StructureNode> infiniteLoops, StructureNode exitNode)
        {
            foreach (StructureNode infLoopHeader in infiniteLoops)
            {
                Debug.Assert(!infLoopHeader.OutEdges.Contains(exitNode));
                graph.AddEdge(infLoopHeader, exitNode);
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
            SccFinder<StructureNode> finder = new SccFinder<StructureNode>(graph, delegate(ICollection<StructureNode> scc)
            {
                if (IsInfiniteLoop(graph, scc))
                {
                    StructureNode header = FindNodeWithHighestPostOrderNumber(scc);
                    foreach (StructureNode tail in graph.Predecessors(header))
                    {
                        if (scc.Contains(tail))
                        {
                            infiniteLoopHeaders.Add(tail);
                        }
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
            HashedSet<StructureNode> members = new HashedSet<StructureNode>();
            members.AddRange(scc);
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