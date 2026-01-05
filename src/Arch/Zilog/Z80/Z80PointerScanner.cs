#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.Zilog.Z80
{
    public class Z80PointerScanner : PointerScanner<ushort>
    {
        private readonly SegmentMap map;
        private readonly EndianImageReader rdr;
        private readonly HashSet<ushort> knownLinAddresses;
        private readonly PointerScannerFlags flags;

        public Z80PointerScanner(SegmentMap map, EndianImageReader rdr, HashSet<ushort> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
            this.map = map;
            this.rdr = rdr;
            this.knownLinAddresses = knownLinAddresses;
            this.flags = flags;
        }

        public override int PointerAlignment => 1;

        public override ushort GetLinearAddress(Address address)
        {
            return address.ToUInt16();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out ushort target)
        {
            switch (opcode)
            {
            case 0xC2:
            case 0xC3:
            case 0xCA:
            case 0xD2:
            case 0xDA:
            case 0xE2:
            case 0xEA:
            case 0xF2:
            case 0xFA:
                return rdr.TryPeekLeUInt16(1, out target);
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out ushort target)
        {
            switch (opcode)
            {
            case 0xC4:
            case 0xCC:
            case 0xCD:
            case 0xD4:
            case 0xDC:
            case 0xE4:
            case 0xEC:
            case 0xF4:
            case 0xFC:
                return rdr.TryPeekLeUInt16(1, out target);
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