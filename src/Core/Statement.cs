#region License
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
#endregion

using System;
using Decompiler.Core.Code;

namespace Decompiler.Core
{
	public class Statement
	{
        [Obsolete("Use the other constructor")]
		public Statement(Instruction instr, Block block)
		{
            this.LinearAddress = 0;
			this.Instruction = instr;
			this.Block = block;
		}

        public Statement(uint linearAddress, Instruction instr, Block block)
        {
            this.LinearAddress = linearAddress;
            this.Instruction = instr;
        }

        public uint LinearAddress { get; private set; }

        public Block Block {get; private set; }

        public Instruction Instruction { get; set; }

		public override string ToString()
		{
			return Instruction.ToString();
		}
	}
}
