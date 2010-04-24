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

using System;
using Decompiler.Core.Code;

namespace Decompiler.Core
{
	public class Statement
	{
		private Instruction instruction;
        private Block block;

		public Statement(Instruction instr, Block block)
		{
			this.instruction = instr;
			this.block = block;
		}

        public Block Block
        {
            get { return block; }
        }

		public Instruction Instruction
		{
			get { return instruction; }
			set { instruction = value; }
		}

		public override string ToString()
		{
			return instruction.ToString();
		}
	}
}
