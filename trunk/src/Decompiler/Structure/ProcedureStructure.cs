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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Structure;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private List<StructureNode> ordering;
        private List<DerivedGraph> derivedGraphs;	// the derived graphs for this procedure

        public ProcedureStructure(string name, List<StructureNode> nodes)
        {
            this.Name = name;
            this.Nodes = nodes;
            this.derivedGraphs = new List<DerivedGraph>();
            this.ordering = new List<StructureNode>();
        }

        public StructureNode EntryNode { get; set; }
        public StructureNode ExitNode { get; set; }
        public string Name { get; set; }
        public List<StructureNode> Nodes { get; private set; }

        public StructureNode FindNodeByName(string nodeName)
        {
            return Nodes.FirstOrDefault(node => node.Name == nodeName);
        }

        public List<DerivedGraph> DerivedGraphs
        {
            get { return derivedGraphs; }
        }

        public List<StructureNode> Ordering
        {
            get { return ordering; }
        }

        [Conditional("DEBUG")]
        public void Dump()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            Debug.WriteLine(sw.ToString());
        }

        public void Write(TextWriter writer)
        {
            WriteNode(this.EntryNode, new HashSet<StructureNode>(), writer);
        }

        [Conditional("VERBOSE_DEBUG")]
        private void WriteNode(StructureNode node, HashSet<StructureNode> visited, TextWriter writer)
        {
            if (visited.Contains(node))
                return;
            visited.Add(node);
            writer.WriteLine("Node {0}: Block: {1}",
                node.Number,
                node.Block != null ? node.Block.Name : "<none>");

            writer.WriteLine("    Order: {0}", node.Order);
            writer.WriteLine("    Interval: {0}", node.Interval != null ? (object) node.Interval.Number : "<none>");
            writer.WriteLine("    iPDom: {0}", node.ImmPDom != null ? node.ImmPDom.Name : "<none>");
            writer.Write("    Structure type:");
            if (node.Loop != null)
                writer.Write(" {0}", node.Loop.GetType().Name);
            if (node.Conditional != null)
                writer.Write(" {0}", node.Conditional.GetType().Name);
            writer.WriteLine();

            if (node.Loop != null && node.Loop.Header == node)
            {
                writer.WriteLine("    Loop Latch: {0}", node.Loop.Latch.Name);
                writer.WriteLine("    Loop Follow: {0}", node.Loop.Follow);
            }
            if (node.Conditional != null)
            {
                if (node.Conditional.Follow != null)
                    writer.WriteLine("    Cond follow: {0}", node.Conditional.Follow.Block.Name);
            }
            if (node.CaseHead != null)
            {
                writer.WriteLine("    Case header: {0}", node.CaseHead.Name);
            }
            writer.WriteLine("    Unstructured type: {0}", node.UnstructType);
            foreach (Statement stm in node.Block.Statements)
            {
                writer.Write("\t");
                writer.WriteLine(stm);
            }
            writer.Write("    Succ: ");
            string sep = "";
            foreach (StructureNode s in node.OutEdges)
            {
                writer.Write(sep);
                writer.Write(s.Block.Name);
                sep = ",";
            }
            writer.WriteLine();

            foreach (StructureNode s in node.OutEdges)
            {
                WriteNode(s, visited, writer);
            }
        }

        [Conditional("DEBUG")]
        public void DumpDerivedSequence(TextWriter writer)
        {
            WriteDerivedSequence(Console.Out);
        }

        [Conditional("DEBUG")]
        public void WriteDerivedSequence(TextWriter writer)
        {
            for (int i = 0; i < this.derivedGraphs.Count; ++i)
            {
                writer.WriteLine("Graph level {0}", i);
                derivedGraphs[i].Write(writer);
            }
        }
    }
}
