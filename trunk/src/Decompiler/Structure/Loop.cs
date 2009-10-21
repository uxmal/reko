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
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    /// <summary>
    /// A loop is described by its header, its latch node, its members, and a follow node.
    /// </summary>
    public abstract class Loop
    {
        private StructureNode header;
        private StructureNode latch;
        private StructureNode follow;
        private HashSet<StructureNode> loopNodes;

        public Loop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes)
        {
            this.header = header;
            this.latch = latch;
            this.loopNodes = loopNodes;
        }

        public Loop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow)
        {
            this.header = header;
            this.latch = latch;
            this.loopNodes = loopNodes;
            this.follow = follow;
        }

        public StructureNode Header
        {
            get { return header; }
        }

        public StructureNode Latch
        {
            get { return latch; }
        }

        public HashSet<StructureNode> Nodes
        {
            get { return loopNodes; }
        }

        public StructureNode Follow
        {
            get { return follow; }
        }
    }


    public class PreTestedLoop : Loop
    {
        public PreTestedLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow) : base(header, latch, loopNodes, follow)
        {
        }
    }

    public class PostTestedLoop : Loop
    {
        public PostTestedLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow) : base(header, latch, loopNodes, follow)
        {
        }
    }

    public class EndLessLoop : Loop
    {
        public EndLessLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow) : base(header, latch, loopNodes, follow)
        {
        }
    }


	/// <summary>
	/// Describes a loop.
	/// </summary>
    [Obsolete]
	public abstract class LoopObsolete
	{
		private BitSet blocks;
		private Block header;
		private Block end;
		protected Block follow;

		public LoopObsolete(Block header, Block end, BitSet blocks)
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

    [Obsolete]
	public class WhileLoopObsolete : LoopObsolete
	{
		public WhileLoopObsolete(Block head, Block end, BitSet blocks) : base(head, end, blocks)
		{
			follow = GetFollowBlock(head);
		}

	}

    [Obsolete]
	public class RepeatLoopObsolete : LoopObsolete
	{
		public RepeatLoopObsolete(Block head, Block end, BitSet blocks) : base(head, end, blocks)
		{
			follow = GetFollowBlock(end);
		}
	}
}
