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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Structure;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Structure
{
    [Obsolete("", true)]
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
    [Obsolete("", true)]
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
