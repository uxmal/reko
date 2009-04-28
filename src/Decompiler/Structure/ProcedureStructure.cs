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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Structure;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    /// <summary>
    /// Collects structure-related data associated with a Procedure.
    /// </summary>
    public class ProcedureStructure
    {
        private Procedure proc;
        private Dictionary<Block, StructureNode> nodes;               // The nodes of the procedure.
        private StructureNode entryNode;					// The node at the head of the graph
        private StructureNode exitNode;			// the node at the bottom of the graph
        private List<StructureNode> ordering;
        private List<StructureNode> revOrdering;
        private List<ast.DerivedGraph> derivedGraphs;	// the derived graphs for this procedure
        private List<StructureNode> nodeList;			// head of the linked list of nodes

        public ProcedureStructure(Procedure proc)
        {
            Dictionary<Block, StructureNode> blocks = new Dictionary<Block, StructureNode>();
            BuildNodes(proc.EntryBlock, blocks);
            BuildEdges(blocks);
            this.ordering = new List<StructureNode>();
            this.revOrdering = new List<StructureNode>();
            this.derivedGraphs = new List<ast.DerivedGraph>();
            this.entryNode = blocks[proc.EntryBlock];
            this.exitNode = blocks[proc.ExitBlock];
            SetTimeStamps();
        }

        [Obsolete]
        public ProcedureStructure(Procedure proc, Dictionary<Block, StructureNode> blocks)
        {
            this.proc = proc;
            this.nodes = blocks;
            this.ordering = new List<StructureNode>();
            this.revOrdering = new List<StructureNode>();
            this.derivedGraphs = new List<ast.DerivedGraph>();
            this.entryNode = blocks[proc.EntryBlock];
            this.exitNode = blocks[proc.ExitBlock];
        }

        private void BuildNodes(Block block, Dictionary<Block, StructureNode> blockNodes)
        {
            StructureNode node = new StructureNode(blockNodes.Count, block);
            blockNodes.Add(block, node);
            foreach (Block succ in block.Succ)
            {
                if (!blockNodes.ContainsKey(succ))
                {
                    BuildNodes(succ, blockNodes);
                }
            }
        }

        public void BuildEdges(Dictionary<Block, StructureNode> blockNodes)
        {
            foreach (KeyValuePair<Block, StructureNode> de in blockNodes)
            {
                foreach (Block succ in de.Key.Succ)
                {
                    de.Value.AddEdgeTo(blockNodes[succ]);
                }
            }
        }

        public Procedure Procedure
        {
            get { return proc; }
        }

        public string Name
        {
            get { return proc.Name; }
        }

        public StructureNode EntryNode
        {
            get { return entryNode; }
        }

        public StructureNode ExitNode
        {
            get { return exitNode; }
        }

        public List<ast.DerivedGraph> DerivedGraphs
        {
            get { return derivedGraphs; }
        }

        public ICollection<StructureNode> Nodes
        {
            get { return nodes.Values; }
        }

        /// <summary>
        /// The nodes of the procedure in post-order. Nodes "further down" in the graph
        /// have lower numbers.
        /// </summary>
        public List<StructureNode> Ordering
        {
            get { return ordering; }
        }

        // an array of pointers to the nodes
        // within this procedure such that the nodes lower in
        // reverse graph are earlier in the array
        public List<StructureNode> RevOrdering
        {
            get { return revOrdering; }
        }

        // Performs a DF traversal on the procedure, starting with the entry node.
        // This gives each node its time stamp tuple
        // that will be used for loop structuring as well as building the structure that will
        // be used for traversing the nodes in linear time. The inedges are also built during
        // this traversal.
        public void SetLoopStamps()
        {
            int time = 0;
            HashSet<StructureNode> visited = new HashSet<StructureNode>();
            SetLoopStamps(EntryNode, ref time, visited);
        }

        private void SetLoopStamps(StructureNode node, ref int time, HashSet<StructureNode> visited)
        {
            //timestamp the current node with the current time and set its traversed flag
            visited.Add(node);
            node.EntryStamp = time;

            //recurse on unvisited children and set inedges for all children
            foreach (StructureNode outEdge in node.Succ)
            {
                // set the in edge from this child to its parent (the current node)
                outEdge.Pred.Add(node);

                // recurse on this child if it hasn't already been visited
                if (!visited.Contains(outEdge))
                {
                    ++time;
                    SetLoopStamps(outEdge, ref time, visited);
                }
            }

            //set the the second loopStamp value
            node.ExitStamp = ++time;

            //add this node to the ordering structure as well as recording its position within the ordering
            node.Order = this.Ordering.Count;
            Ordering.Add(node);
        }

        private void SetTimeStamps()
        {
            // the post-order ordering between the nodes
            SetLoopStamps();

            // set the reverse parenthesis for the nodes
            int time = 1;
            SetRevLoopStamps(EntryNode, ref time, new HashSet<StructureNode>());

            Debug.Assert(ExitNode != null);
            SetRevOrder(ExitNode, RevOrdering, new HashSet<StructureNode>());
        }

        // This sets the reverse loop stamps for each node. The children are traversed in
        // reverse order.
        private void SetRevLoopStamps(StructureNode node, ref int time, HashSet<StructureNode> traversed)
        {
            //timestamp the current node with the current time and set its traversed flag
            traversed.Add(node);
            node.RevEntryStamp = time;

            //recurse on the unvisited children in reverse order
            for (int i = node.Succ.Count - 1; i >= 0; i--)
            {
                // recurse on this child if it hasn't already been visited
                if (!traversed.Contains(node.Succ[i]))
                {
                    ++time;
                    SetRevLoopStamps(node.Succ[i], ref time, traversed);
                }
            }

            //set the the second loopStamp value
            node.RevExitStamp = ++time;
        }


        // Build the ordering of the nodes in the reverse graph that will be used to
        // determine the immediate post dominators for each node
        public void SetRevOrder(StructureNode node, List<StructureNode> rvOrder, HashSet<StructureNode> traversed)
        {
            // Set this node as having been traversed during the post domimator 
            // DFS ordering traversal
            traversed.Add(node);

            // recurse on unvisited children 
            foreach (StructureNode p in node.Pred)
            {
                if (!traversed.Contains(p))
                    SetRevOrder(p, rvOrder, traversed);
            }

            // add this node to the ordering structure and record the post dom. order
            // of this node as its index within this ordering structure
            node.RevOrder = rvOrder.Count;
            rvOrder.Add(node);
        }


    }
}
