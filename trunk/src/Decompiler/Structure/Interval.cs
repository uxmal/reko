/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
	public class Interval
	{
		private Block		header;
		public int			OutEdges;
		public ArrayList	Nodes;			// nodes in the interval
		public BitSet		Blocks;			// blocks of the interval

		public Interval(Block header, int cBlocks) 
		{
			this.header = header;
			Nodes = new ArrayList();
			Blocks = new BitSet(cBlocks);
		}
		
		public StructureNode HeadNode
		{ 
			get { return (StructureNode) Nodes[0]; }
		}

		public void AddBlock(Block block)
		{
			Blocks[block.RpoNumber] = true;
		}

		public void AddNode(StructureNode n)
		{
			Nodes.Add(n);
			n.Interval = this;
			Blocks |= n.Blocks;
		}

		public Block Header
		{
			get { return header; }
		}

		public void Write(TextWriter tw)
		{
			tw.Write("Header {0}, blocks:", (Header != null ? Header.RpoNumber.ToString() : "<none>"));
			foreach (int i in Blocks)
			{
				tw.Write(" {0}", i);
			}
		}

		public override string ToString()
		{
			StringWriter text = new StringWriter();
			Write(text);
			return text.ToString();
		}

	}
}
