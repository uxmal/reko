#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Code;

namespace Reko.Core
{
    /// <summary>
    /// Statements constitute the contents of basic blocks. They associate an
    /// <see cref="Reko.Core.Code.Instruction"/> with an <see cref="Reko.Core.Address"/> and 
    /// the <see cref="Reko.Core.Block"/> in which the instrution is located.
    /// </summary>
	public class Statement
	{
        public Statement(Address address, Instruction instr, Block block)
        {
            this.Address = address;
            this.Instruction = instr;
            this.Block = block;
        }

        public Block Block { get; set; }
        public Instruction Instruction { get; set; }
        public Address Address { get; }

		public override string ToString()
		{
			return Instruction.ToString();
		}
	}
}
