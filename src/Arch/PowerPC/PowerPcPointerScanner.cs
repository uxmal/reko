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

namespace Reko.Arch.PowerPC
{
    public abstract class PowerPcPointerScanner<T> : PointerScanner<T>
    {
        public PowerPcPointerScanner(EndianImageReader rdr, HashSet<T> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            return rdr.TryPeekBeUInt32(0, out opcode);
        }

        protected bool TryGetDisplacement(uint opcode, uint opcodeExp, out int offset)
        {
            if ((opcode & 0xFC000003u) == opcodeExp)
            {
                var uOffset = opcode & 0x03FFFFFC;
                if ((uOffset & 0x02000000) != 0)
                    uOffset |= 0xFC000000;
                offset = (int)uOffset;
                return true;
            }
            offset = 0;
            return false;
        }

    }

    public class PowerPcPointerScanner32 : PowerPcPointerScanner<uint>
    {
        public PowerPcPointerScanner32(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment
        {
            get { return 4; }
        }

        public override uint GetLinearAddress(Address address)
        {
            return address.ToUInt32();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (TryGetDisplacement(opcode, 0x48000001u, out int offset))
            {
                target = (uint)(rdr.Address.ToUInt32() + offset);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (TryGetDisplacement(opcode, 0x48000000u, out int offset))
            {
                target = (uint)(rdr.Address.ToUInt32() + offset);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            return rdr.TryPeekBeUInt32(0, out target);
        }
    }

    public class PowerPcPointerScanner64 : PowerPcPointerScanner<ulong>
    {
        public PowerPcPointerScanner64(EndianImageReader rdr, HashSet<ulong> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment
        {
            get { return 4; }
        }

        public override ulong GetLinearAddress(Address address)
        {
            return address.ToLinear();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out ulong target)
        {
            if (TryGetDisplacement(opcode, 0x48000001u, out int offset))
            {
                target = (ulong)((long)rdr.Address.ToLinear() + (long)offset);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out ulong target)
        {
            if (TryGetDisplacement(opcode, 0x48000000u, out int offset))
            {
                target = (ulong)((long)rdr.Address.ToLinear() + (long)offset);
                return true;
            }
            target = 0;
            return false;
        }
        public override bool TryPeekPointer(EndianImageReader rdr, out ulong target)
        {
            return rdr.TryPeekBeUInt64(0, out target);
        }
    }
}
