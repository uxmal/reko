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
using System.IO;

namespace Decompiler.Structure
{
    /// <summary>
    /// Represents an interval.
    /// </summary>
    public class Interval : StructureNode
    {
        private List<StructureNode> nodes = new List<StructureNode>();		// nodes of the interval

        public Interval(int intervalID, StructureNode headerNode)
            : base(intervalID, bbType.intNode)
        {
            AddNode(headerNode);
        }

        public bool Contains(StructureNode node)
        {
            return nodes.Contains(node);
        }

        public void AddNode(StructureNode node)
        {
            nodes.Add(node);
            node.Interval = this;
        }

        public override string Name
        {
            get { return base.Ident().ToString(); }
        }

        public List<StructureNode> Nodes
        {
            get { return nodes; }
        }

        public HashedSet<StructureNode> FindIntervalNodes(int level)
        {
            HashedSet<StructureNode> nodes = new HashedSet<StructureNode>();
            FindIntervalNodes(level, nodes);
            return nodes;
        }

        private void FindIntervalNodes(int level, HashedSet<StructureNode> intervalMembers)
        {
            if (level == 0)
                foreach (StructureNode node in nodes)
                    intervalMembers.Add(node);
            else
                foreach (Interval i in nodes)
                    i.FindIntervalNodes(level - 1, intervalMembers);
        }

        public StructureNode Header
        {
            get { return nodes[0]; }
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("Interval {0}: [", Ident());
            string sep = "";
            StructureNode[] ns = nodes.ToArray();
            Array.Sort(ns, delegate(StructureNode a, StructureNode b)
            {
                return string.Compare(a.Name, b.Name);
            });
            foreach (StructureNode node in ns)
            {
                writer.Write(sep);
                sep = ",";
                writer.Write(node.Name);
            }
            writer.Write("]");
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }
    }
}