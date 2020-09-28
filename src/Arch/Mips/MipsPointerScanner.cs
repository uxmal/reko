#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    /// <summary>
    /// Scans an image looking for uses of pointer values.
    /// </summary>
    public class MipsPointerScanner32 : PointerScanner<uint>
    {
        public MipsPointerScanner32(
            EndianImageReader rdr, 
            HashSet<uint> knownLinAddresses,
            PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 4; } }

        public override uint GetLinearAddress(Address address)
        {
            return address.ToUInt32();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xFC000000) == 0x0C000000 // JAL
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                var off = (opcode & 0x03FFFFFF) << 2;
                target = (((uint)rdr.Address.ToLinear() + 4) & 0xF0000000u) | off;
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xFC000000) == 0x08000000  // J - far jump
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                var off = (opcode & 0x03FFFFFF) << 2;
                target = (((uint)rdr.Address.ToLinear() + 4) & 0xF0000000u) | off;
                return true;
            }

            if ((opcode & 0xF0000000) == 0x10000000  // b** - far jump
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                int off = (short) opcode;
                off <<= 2;
                target = (uint)(((int)rdr.Address.ToLinear() + 4) + off);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            return rdr.TryPeekUInt32(0, out opcode);
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            return rdr.TryPeekUInt32(0, out target);
        }
    }
}