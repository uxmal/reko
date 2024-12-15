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

using Reko.Core;
using Reko.Core.Expressions;
using System;

namespace Reko.Arch.Pdp.Memory
{
    public class Address18 : Address
    {
        private const uint Mask = (1 << 18) - 1;
        
        public Address18(uint uAddr) : base(PdpTypes.Ptr18)
        {
            this.Offset = uAddr;
        }

        public override bool IsNull => Offset == 0;

        public override sealed ulong Offset { get; }

        public override ushort? Selector => null;

        public override Address Add(long offset)
        {
            return new Address18((uint) ((long) Offset + offset) & Mask);
        }

        public override Address Align(int alignment)
        {
            return new Address18((uint)(alignment * (((long)Offset + alignment - 1) / alignment)));
        }

        public override Expression CloneExpression()
        {
            return new Address18((uint)Offset);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            var octal = Convert.ToString((uint)Offset, 8);
            return $"{prefix}{octal}{suffix}";
        }

        public override Address NewOffset(ulong offset)
        {
            return new Address18((uint)offset & Mask);
        }

        public override Constant ToConstant()
        {
            return Constant.Create(this.DataType, Offset);
        }

        public override ulong ToLinear()
        {
            return Offset;
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return (uint)Offset;
        }

        protected override string ConvertToString()
        {
            return Convert.ToString((uint) Offset, 8).PadLeft(6, '0');
        }
    }
}
