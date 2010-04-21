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
using System;
using System.Collections.Generic;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Contains dataflow information for each procedure and each block of the program.
	/// </summary>
	public class ProgramDataFlow
	{
		private Dictionary<Procedure,ProcedureFlow> procFlow;
        private Dictionary<Block, BlockFlow> blockFlow;

		public ProgramDataFlow()
		{
			procFlow = new Dictionary<Procedure,ProcedureFlow>();
            blockFlow = new Dictionary<Block,BlockFlow>();
		}

		public ProgramDataFlow(Program prog) : this()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				procFlow[proc] = new ProcedureFlow(proc, prog.Architecture);
				foreach (Block block in proc.RpoBlocks)
				{
					blockFlow[block] = new BlockFlow(block, prog.Architecture.CreateRegisterBitset());
				}
			}
		}

		public ProcedureFlow this[Procedure proc]
		{
			get { return procFlow[proc]; }
			set { procFlow[proc] = value; }
		}

		public BlockFlow this[Block block]
		{
			get { return (BlockFlow) blockFlow[block]; }
            set { blockFlow[block] = value; }
		}

		public ICollection<BlockFlow> BlockFlows
		{
			get { return blockFlow.Values; }
		}

        public ICollection<ProcedureFlow> ProcedureFlows
        {
            get { return procFlow.Values; }
        }
	}
}
