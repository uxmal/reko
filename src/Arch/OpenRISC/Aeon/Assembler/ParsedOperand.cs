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

using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public class ParsedOperand
    {
        public Constant? Immediate;
        public string? Identifier;

        private ParsedOperand() { }

        public static ParsedOperand UInt32(uint value)
        {
            return new ParsedOperand
            {
                Immediate = Constant.UInt32(value)
            };
        }

        public static ParsedOperand Int32(int value)
        {
            return new ParsedOperand
            {
                Immediate = Constant.Int32(value)
            };
        }

        public static ParsedOperand Id(string id)
        {
            return new ParsedOperand
            {
                Identifier = id
            };
        }
    }
}