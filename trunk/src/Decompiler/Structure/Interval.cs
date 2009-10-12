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
    public class IntNode : StructureNode
    {
        private List<StructureNode> nodes = new List<StructureNode>();		// nodes of the interval
        private List<StructureNode> cfgNodes = new List<StructureNode>();   // list of CFG nodes;

        // Define a global variable to be used when assigning unique id's to new interval nodes
        static int NewIntId = 0;

        public IntNode(StructureNode node)
            : base(NewIntId++, bbType.intNode)
        {
            AddNode(node);
        }

        public IntNode(int intervalID, StructureNode headerNode)
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
            cfgNodes.Add(node);
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

        [Obsolete]
        public void FindNodesInInt(bool[] cfgNodes, int level)
        {
            if (level == 0)
                for (int i = 0; i < nodes.Count; i++)
                    cfgNodes[nodes[i].Order] = true;
            else
                for (int i = 0; i < nodes.Count; i++)
                    ((IntNode) nodes[i]).FindNodesInInt(cfgNodes, level - 1);    //$CAST
        }

        public HashSet<StructureNode> FindIntervalNodes(int level)
        {
            HashSet<StructureNode> nodes = new HashSet<StructureNode>();
            FindIntervalNodes(level, nodes);
            return nodes;
        }

        private void FindIntervalNodes(int level, HashSet<StructureNode> intervalMembers)
        {
            if (level == 0)
                for (int i = 0; i < nodes.Count; ++i)
                    intervalMembers.Add(nodes[i]);
            else
                for (int i = 0; i < nodes.Count; ++i)
                    ((IntNode) nodes[i]).FindIntervalNodes(level - 1, intervalMembers);
        }

        public StructureNode Header
        {
            get { return nodes[0]; }
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("Interval {0}: [", Ident());
            string sep = "";
            StructureNode [] ns = nodes.ToArray();
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

        [Obsolete("Investigate getting rid of this property")]
        public Block HeaderBlock
        {
            get { throw new NotImplementedException(); }
        }

        [Obsolete("Investigate getting rid of this property")]
        internal BitSet Blocks
        {
            get { throw new NotImplementedException(); }
        }

    }

#if DELETE_ME
	public class Interval : StructureNode2
	{
		private Block header;
		private List<StructureNode2> nodes;	// nodes in the interval

		public Interval(int id, Block header) : base(id, header)
		{
			this.header = header;
			nodes = new List<StructureNode2>();
		}
		
		public StructureNode2 HeaderNode
		{ 
			get { return Nodes[0]; }
		}

		public void AddNode(StructureNode2 n)
		{
			Nodes.Add(n);
			n.Interval = this;
		}

        public BitSet Blocks
        {
            get { return null; }
        }

        /// <summary>
        /// A list of the nodes that are part of this interval.
        /// </summary>
        public List<StructureNode2> Nodes
        {
            get { return nodes; }
        }

        [Obsolete]
        public void FindNodesInInterval(bool[] cfgNodes)
        {
            foreach (StructureNode2 node in this.Nodes)
            {
                Interval innerInt = node as Interval;
                if (innerInt == null)
                {
                    cfgNodes[node.Order] = true;
                }
                else
                {
                    innerInt.FindNodesInInterval(cfgNodes);
                }
            }
        }

        public void FindNodesInInterval(IDictionary<int, StructureNode2> nodes)
        {
            foreach (StructureNode2 node in this.Nodes)
            {
                Interval innerInt = node as Interval;
                if (innerInt == null)
                {
                    nodes.Add(node.Order, node);
                }
                else
                {
                    innerInt.FindNodesInInterval(nodes);
                }
            }
        }


        public Block HeaderBlock
		{
			get { return header; }
		}

		public override void Write(TextWriter writer)
		{
            writer.Write("Interval {0}: [", this.Ident);
            SortedDictionary<int, StructureNode2> nodes = new SortedDictionary<int, StructureNode2>();
            FindNodesInInterval(nodes);
            string sep = "";
			foreach (StructureNode2 sn in nodes.Values)
			{
                writer.Write(sep);
				writer.Write(sn.EntryBlock.Name);
                sep = ",";
			}
            writer.Write("]");
		}

		public override string ToString()
		{
			StringWriter text = new StringWriter();
			Write(text);
			return text.ToString();
		}
	}
#endif
}
