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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Text;

namespace Decompiler.Structure
{
	/// <summary>
	/// Describes a loop.
	/// </summary>
	public abstract class Loop
	{
		private BitSet blocks;
		private Block header;
		private Block end;
		protected Block follow;

		public Loop(Block header, Block end, BitSet blocks)
		{
			this.header = header;
			this.end = end;
			this.blocks = blocks;
		}

		public BitSet Blocks
		{
			get { return blocks; }
		}


		public Block EndBlock
		{
			get { return end; }
		}
		
		public Block FollowBlock
		{
			get { return follow; }
		}

		protected static Branch GetBranch(Block block)
		{
			if (block.Statements.Count == 0)
				return null;
			return block.Statements.Last.Instruction as Branch;
		}

		public Block GetFollowBlock(Block b)
		{
			if (Blocks[b.ElseBlock.RpoNumber])
			{
				return b.ThenBlock;
			}
			else
			{
				return b.ElseBlock;
			}
		}

		public Block HeaderBlock
		{
			get { return header; }
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("hdr: {0}, end: {1} [", header.RpoNumber, end.RpoNumber);
			string fmt = "{0}";
			foreach (int i in blocks)
			{
				sb.AppendFormat(fmt, i);
				fmt = ", {0}";
			}
			sb.Append("]");
			return sb.ToString();
		}
	}

	public class WhileLoop : Loop
	{
		public WhileLoop(Block head, Block end, BitSet blocks) : base(head, end, blocks)
		{
			follow = GetFollowBlock(head);
		}

	}

	public class RepeatLoop : Loop
	{
		public RepeatLoop(Block head, Block end, BitSet blocks) : base(head, end, blocks)
		{
			follow = GetFollowBlock(end);
		}
	}
}
