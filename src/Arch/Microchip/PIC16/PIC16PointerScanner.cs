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

using Reko.Core;
using Reko.Libraries.Microchip;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// Scans an image looking for uses of pointer values.
    /// </summary>
    public class PIC16PointerScanner : PICPointerScanner
    {
        public PIC16PointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override uint GetLinearAddress(Address address) =>  address.ToUInt32();

        public override int PointerAlignment => 2;

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            bool ret = rdr.TryReadUInt16(out ushort bOpcode);
            opcode = bOpcode;
            return ret;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            target = 0;
            if (rdr.IsValidOffset(rdr.Offset + 2 - 1))
            {
                target = rdr.PeekLeUInt16(0);
                return true;
            }
            return false;
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            target = 0;
            ushort sopcode = (ushort)opcode;
            var offset = rdr.Offset;
            if ((sopcode & 0x3800) == 0x2000) // CALL k
            {
                target = sopcode.Extract(0, 11);
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
            if ((sopcode & 0x3800) == 0x2800) // GOTO k
            {
                target = sopcode.Extract(0, 11);
                return true;
            }
            rdr.Offset = offset;
            return false;
        }

    }
}
