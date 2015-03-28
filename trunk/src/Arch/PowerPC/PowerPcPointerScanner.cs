#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcPointerScanner : PointerScanner<uint>
    {
        public PowerPcPointerScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override uint GetLinearAddress(Address address)
        {
            return (uint)address.ToLinear();
        }

        public override bool TryPeekOpcode(ImageReader rdr, out uint opcode)
        {
            return rdr.TryPeekBeUInt32(0, out opcode);
        }

        public override int PointerAlignment
        {
            get { return 4; }
        }

        public override bool MatchCall(ImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xFC000003u) == 0x48000001u)
            {
                var uOffset = opcode & 0x03FFFFFC;
                if ((uOffset & 0x02000000) != 0)
                    uOffset |= 0xFF000000;
                target = unchecked((uint)rdr.Address.ToLinear() + uOffset);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(ImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xFC000003u) == 0x48000000u)
            {
                var uOffset = opcode & 0x03FFFFFC;
                if ((uOffset & 0x02000000) != 0)
                    uOffset |= 0xFF000000;
                target = unchecked((uint)rdr.Address.ToLinear() + uOffset);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool PeekPointer(ImageReader rdr, out uint target)
        {
            return rdr.TryPeekBeUInt32(0, out target);
        }
    }
}
