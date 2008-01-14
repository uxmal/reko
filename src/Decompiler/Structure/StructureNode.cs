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
using System.IO;

namespace Decompiler.Structure
{
	// class StructureNode ///////////////////////

	public class StructureNode
	{
		public int Number;
		public Block EntryBlock;
		public int LinksFollowed;
		public Interval Interval;
		public BitSet Blocks;
		public ArrayList Pred;
		public ArrayList Succ;

		public StructureNode(int i, Block entry, BitSet blocks)
		{
			Number = i;
			EntryBlock = entry;
			LinksFollowed = 0;
			Blocks = blocks;
			Pred = new ArrayList();
			Succ = new ArrayList();
		}

		public static void AddEdge(StructureNode from, StructureNode to)
		{
			if (from.Succ.Contains(to) && to.Pred.Contains(from))
			{
				return;
			}
			from.Succ.Add(to);
			to.Pred.Add(from);
		}

		public StructureNode IntervalHead
		{
			get { return Interval.HeadNode; }
		}

		public void Write(TextWriter tw)
		{
			tw.Write("node {0}: entry {1}, blocks: [", Number, EntryBlock.RpoNumber);
			foreach (int i in Blocks)
			{
				tw.Write(" {0}", i);
			}
			tw.WriteLine(" ]");
			tw.Write("  pred:");
			foreach (StructureNode p in Pred)
			{
				tw.Write(" {0}", p.Number);
			}
			tw.WriteLine();
			tw.Write("  succ: ");
			foreach (StructureNode s in Succ)
			{
				tw.Write(" {0}", s.Number);
			}

			tw.WriteLine();


		}

	}
}
