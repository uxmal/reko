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
	public class Interval : StructureNode
	{
		private Block header;
		private List<StructureNode> nodes;	// nodes in the interval

		public Interval(int id, Block header) : base(id, header)
		{
			this.header = header;
			nodes = new List<StructureNode>();
		}
		
		public StructureNode HeaderNode
		{ 
			get { return Nodes[0]; }
		}

		public void AddNode(StructureNode n)
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
        public List<StructureNode> Nodes
        {
            get { return nodes; }
        }

        [Obsolete]
        public void FindNodesInInterval(bool[] cfgNodes)
        {
            foreach (StructureNode node in this.Nodes)
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

        public void FindNodesInInterval(IDictionary<int, StructureNode> nodes)
        {
            foreach (StructureNode node in this.Nodes)
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
            SortedDictionary<int, StructureNode> nodes = new SortedDictionary<int, StructureNode>();
            FindNodesInInterval(nodes);
            string sep = "";
			foreach (StructureNode sn in nodes.Values)
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
}
