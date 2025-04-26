#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Represents a list of <see cref="Statement"/>s in a <see cref="Block"/>.
    /// </summary>
    public class StatementList : List<Statement>
    {
        private readonly Block block;

        /// <summary>
        /// Constructs a <see cref="StatementList"/>.
        /// </summary>
        /// <param name="block">The <see cref="Block"/> in which the statements
        /// are located.
        /// </param>
        public StatementList(Block block)
        {
            this.block = block;
        }

        /// <summary>
        /// Adds an instruction to the end of the statement list.
        /// </summary>
        /// <param name="address">Address from which the <see cref="Instruction"/>
        /// was lifted.</param>
        /// <param name="instr">Instruction to add.
        /// </param>
        /// <returns>The new created <see cref="Statement"/>.</returns>
        public Statement Add(Address address, Instruction instr)
        {
            var stm = new Statement(address, instr, block);
            Add(stm);
            return stm;
        }

        /// <summary>
        /// Inserts an instruction at a given position in the statement list.
        /// </summary>
        /// <param name="position">Index at which to insert the statement.</param>
        /// <param name="address">Address from which the <see cref="Instruction"/>
        /// was lifted.</param>
        /// <param name="instr">Instruction to insert.
        /// </param>
        /// <returns>The new created <see cref="Statement"/>.</returns>
        public Statement Insert(int position, Address address, Instruction instr)
        {
            var stm = new Statement(address, instr, block);
            Insert(position, stm);
            return stm;
        }
    }
}
