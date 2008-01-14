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

namespace Decompiler.Structure
{
	/// <summary>
	/// Follows a chain of successor nodes, halting when either no successors exist,
	/// or more than one exists.
	/// </summary>
	public class StraightPathWalker
	{
		private Block block;
		private int pathLength;
		private int rpoPrev = -1;

		public StraightPathWalker(Block block)
		{
			this.block = block;
			this.pathLength = 0;
		}

		public bool Advance()
		{
			if (block.Succ.Count == 1)
			{
				rpoPrev = block.RpoNumber;
				block = block.Succ[0];
				++pathLength;
				return true;
			}
			else
				return false;
		}

		public Block Current
		{
			get { return block; }
		}

		public int PathLength
		{
			get { return pathLength; }
		}

		public bool IsBlocked(BitSet loopBody)
		{
			if (!loopBody[block.RpoNumber])
				return true;
			if (block.RpoNumber <= rpoPrev)
				return true;
			return block.Succ.Count != 1;
		}
	}
}
