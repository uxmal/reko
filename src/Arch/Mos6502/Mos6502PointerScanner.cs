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

using Reko.Core;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.Arch.Mos6502
{
    public class Mos6502PointerScanner : PointerScanner<ushort>
    {
        public Mos6502PointerScanner(EndianImageReader rdr, HashSet<ushort> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {

        }

        public override int PointerAlignment => 1;

        public override ushort GetLinearAddress(Address address)
        {
            return address.ToUInt16();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out ushort target)
        {
            if (opcode == 0x20 // JSR
                && rdr.TryPeekLeUInt16(1, out target))
            {
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out ushort target)
        {
            if (opcode == 0x4C // JMP
                && rdr.TryPeekLeUInt16(1, out target))
            {
                return true;
            }
            target = 0;
            return false;
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            if (!rdr.TryPeekByte(0, out byte bOpcode))
            {
                opcode = 0;
                return false;
            }
            opcode = bOpcode;
            return true;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out ushort target)
        {
            return rdr.TryPeekLeUInt16(0, out target);
        }
    }
}