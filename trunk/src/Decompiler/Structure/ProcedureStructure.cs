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
using System.IO;
using System.Text;

namespace Decompiler.Structure
{
    public abstract class GraphNode<T>
    {
        private List<T> inEdges;				// pointers to the nodes on an in edge to this node
        private List<T> outEdges;				// pointers to the nodes on an out edge from this node

        public GraphNode()
        {
            inEdges = new List<T>();
            outEdges = new List<T>();
        }

        public List<T> InEdges { get { return inEdges; } }
        public List<T> OutEdges { get { return outEdges; } }
    }

    /// <summary>
    /// Collects structure-related data associated with a Procedure.
    /// </summary>
    public class ProcedureStructure
    {
        private Procedure proc;
        private string name;
        private List<StructureNode> nodes;
        private StructureNode exitNode;
        private StructureNode entryNode;
        private List<StructureNode> ordering;
        private List<StructureNode> revOrdering;
        private List<Decompiler.Structure.DerivedGraph> derivedGraphs;	// the derived graphs for this procedure

        public ProcedureStructure(Procedure proc, List<StructureNode> nodes)
        {
            this.proc = proc;
            this.name = proc.Name;
            this.nodes = nodes;
            this.derivedGraphs = new List<DerivedGraph>();
            this.ordering = new List<StructureNode>();
            this.revOrdering = new List<StructureNode>();
        }


        public List<StructureNode> Nodes
        {
            get { return nodes; }
        }

        public StructureNode EntryNode
        {
            get { return entryNode; }
            set { entryNode = value; }
        }

        public StructureNode ExitNode
        {
            get { return exitNode; }
            set { exitNode = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Decompiler.Structure.DerivedGraph> DerivedGraphs
        {
            get { return derivedGraphs; }
        }

        public List<StructureNode> Ordering
        {
            get { return ordering; }
        }

        // within this procedure such that the nodes lower in
        // graph are earlier in the array

        public List<StructureNode> ReverseOrdering
        {
            get { return revOrdering; }
        }


        [Conditional("DEBUG")]
        public void Dump()
        {
            DumpNode(this.entryNode, new HashSet<StructureNode>());
        }

        [Conditional("DEBUG")]
        private void DumpNode(StructureNode node, HashSet<StructureNode> visited)
        {
            if (visited.Contains(node))
                return;
            visited.Add(node);
            Debug.WriteLine(string.Format("Node {0}: Block: {1}",
                node.Ident(),
                node.Block != null ? node.Block.Name : "<none>"));
            Debug.Indent();

            Debug.WriteLine(string.Format("Order: {0}, RevOrder {1}", node.Order, node.RevOrder));
            if (node.LoopHead != null)
                Debug.WriteLine("Loop header:" + node.LoopHead.Block.Name);
            if (node.LatchNode != null)
                Debug.WriteLine("latch:" + node.LatchNode.Block.Name);
            if (node.CondFollow != null)
                Debug.WriteLine("Cond follow:" + node.CondFollow.Block.Name);

            Debug.Write("Succ: ");
            string sep = "";
            foreach (StructureNode s in node.OutEdges)
            {
                Debug.Write(sep);
                Debug.Write(s.Block.Name);
                sep = ",";
            }
            Debug.WriteLine("");
            Debug.Unindent();

            foreach (StructureNode s in node.OutEdges)
            {
                DumpNode(s, visited);
            }
        }

    }
}
