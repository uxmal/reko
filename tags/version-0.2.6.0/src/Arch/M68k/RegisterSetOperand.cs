#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class RegisterSetOperand : M68kOperandImpl
    {
        public RegisterSetOperand(uint bitset)
            : base(PrimitiveType.Word16)
        {
            this.BitSet = bitset;
        }

        public uint BitSet { get; set; }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public static MachineOperand CreateReversed(ushort vv)
        {
            int v;
            // Swap odd and even bits
            v = ((vv >> 1) & 0x5555) | ((vv & 0x5555) << 1);
            // swap consecutive pairs
            v = ((v >> 2) & 0x3333) | ((v & 0x3333) << 2);
            // swap nibbles
            v = ((v >> 4) & 0x0F0F) | ((v & 0x0F0F) << 4);
            // swap bytes
            v = ((v >> 8) & 0x00FF) | ((v & 0x00FF) << 8);
            return new RegisterSetOperand((ushort) v);
        }
    }
}
