#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

using Reko.Libraries.Microchip;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// Scans an image looking for uses of pointer values.
    /// </summary>
    public class PIC18PointerScanner : PICPointerScanner
    {
        public PIC18PointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment => 2;

        public override uint GetLinearAddress(Address address) => address.ToUInt32();

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            bool ret = rdr.TryReadUInt16(out ushort bOpcode);
            opcode = bOpcode;
            return ret;
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            target = 0;
            ushort sopcode = (ushort)opcode;
            var offset = rdr.Offset;
            if (((sopcode & 0xFE00) == 0xEC00) // CALL n,[s]
                &&
                rdr.IsValidOffset(rdr.Offset + 2u))
            {
                ushort word2 = rdr.ReadLeUInt16();
                if ((word2 & 0xF000U) == 0xF000U)
                {
                    target = (uint)(sopcode.Extract(0, 8) | (word2.Extract(0, 12) << 8));
                    return true;
                }
            }
            if ((sopcode & 0xF800) == 0xD800) // RCALL n
            {
                var off = (int)sopcode.ExtractSignExtend(0, 11);
                target = (uint)((long)rdr.Address.ToLinear() + 2 + (off * 2));
                return true;
            }
            rdr.Offset = offset;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            target = 0;
            ushort sopcode = (ushort)opcode;
            var offset = rdr.Offset;
            if (((sopcode & 0xFF00) == 0xEF00) // GOTO n
                &&
                rdr.IsValidOffset(rdr.Offset + 2u))
            {
                ushort word2 = rdr.ReadLeUInt16();
                if ((word2 & 0xF000U) == 0xF000U)
                {
                    target = (uint)(sopcode.Extract(0, 8) | (word2.Extract(0, 12) << 8));
                    return true;
                }
            }
            if ((sopcode & 0xF800) == 0xD000) // BRA n
            {
                var off = (int)sopcode.ExtractSignExtend(0, 11);
                target = (uint)((long)rdr.Address.ToLinear() + 2 + (off * 2));
                return true;
            }
            if ((sopcode & 0xF800) == 0xE000) // Bcond n
            {
                var off = (sbyte)(sopcode.ExtractSignExtend(0, 8));
                target = (uint)((long)rdr.Address.ToLinear() + 2 + (off * 2));
                return true;
            }
            rdr.Offset = offset;
            return false;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            target = 0;
            if (rdr.IsValidOffset(rdr.Offset + 4 - 1))
            {
                target = rdr.PeekLeUInt32(0);
                return true;
            }
            return false;
        }

    }

}
