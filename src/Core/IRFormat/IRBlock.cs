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

namespace Reko.Core.IRFormat
{
    /// <summary>
    /// Represents a block of intermediate representation (IR) code.
    /// </summary>
    public class IRBlock
    {
        private Address addrCur;
        private string id;
        private string? name;
        private readonly List<(Address, Instruction)> stmts;

        public IRBlock(Address addrCur, string id, string? name)
        {
            this.addrCur = addrCur;
            this.id = id;
            this.name = name;
            this.stmts = new List<(Address, Instruction)>(); 
        }

        public IReadOnlyList<(Address, Instruction)> Statements => stmts;

        public Instruction AddStatement(Address addrCur, Instruction instr)
        {
            stmts.Add((addrCur, instr));
            return instr;
        }
    }
}