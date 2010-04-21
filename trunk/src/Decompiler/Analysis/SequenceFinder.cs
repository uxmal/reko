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
using Decompiler.Core.Code;
using System;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Finds register sequences used for long additions &c in code
	/// </summary>
	public class SequenceFinder
	{
		private Procedure proc;
		private SsaState ssa;

		public SequenceFinder(Procedure proc, SsaState ssa)
		{
			this.proc = proc;
			this.ssa = ssa;
		}

		public void Find()
		{
			foreach (Block block in proc.RpoBlocks)
			{
				for (int i = 0; i < block.Statements.Count; ++i)
				{
					if (IsSequenceCandidate(block, i))
					{
						ReplaceSequenceCandidate(block, i);
					}
				}
			}
		}

		private bool IsAddSequence(Block block, int i)
		{
			Assignment ass = block.Statements[i].Instruction as Assignment;
			if (ass == null)
				return false;
			
			return false;
		}

		private bool IsSequenceCandidate(Block block, int i)
		{
			return IsAddSequence(block, i);
		}

		private void ReplaceSequenceCandidate(Block block, int i)
		{
		}
	}
}
