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

namespace Reko.Arch.M68k
{
    public class M68kPointerScanner : PointerScanner<uint>
    {
        public M68kPointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 2; } }

        public override uint GetLinearAddress(Address address)
        {
            return address.ToUInt32();
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            ushort wOpcode;
            if (rdr.TryPeekBeUInt16(0, out wOpcode))
            {
                opcode = wOpcode;
                return true;
            }
            else
            {
                opcode = 0;
                return false;
            }
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xFF00) == 0x6100)
            {
                return RelativeBranchCall(rdr, opcode, out target);
            }
            else if ((opcode & 0xFFFE) == 0x4EB8)        // jsr.w
            {
                return AbsoluteJumpCall(rdr, opcode, out target);
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0xF000) == 0x6000)
            {
                return RelativeBranchCall(rdr, opcode, out target);
            }
            else if ((opcode & 0xFFFE) == 0x4EF8)        // jsr.w
            {
                return AbsoluteJumpCall(rdr, opcode, out target);
            }
            target = 0;
            return false;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            if (!rdr.IsValidOffset(rdr.Offset + 4 - 1))
            {
                target = 0;
                return false;
            }
            else
            {
                target = rdr.PeekBeUInt32(0);
                return true;
            }
        }

        private bool AbsoluteJumpCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 1) == 0)
            {
                if (rdr.IsValidOffset(rdr.Offset + 2u))
                {
                    target = (uint) (int) rdr.PeekBeInt16(2);
                    return true;
                }
            }
            else
            {
                if (rdr.IsValidOffset(rdr.Offset + 4u))
                {
                    target = rdr.PeekBeUInt32(2);
                    return true;
                }
            }
            target = 0;
            return false;
        }

        private bool RelativeBranchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            int callOffset = (sbyte) opcode;
            if (callOffset == -1// bsr.l
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                callOffset = rdr.PeekBeInt32(2);
                target = (uint) (callOffset + (long)rdr.Address.ToLinear() + 2);
                return true;
            }
            if (callOffset == 0x00 // bsr.w)
                &&
                rdr.IsValidOffset(rdr.Offset + 2u))
            {
                callOffset = rdr.PeekBeInt16(2);
                target = (uint) (callOffset + (long)rdr.Address.ToLinear() + 2);
                return true;
            }
            target = (uint) (callOffset + (long)rdr.Address.ToLinear() + 2);
            return true;
        }
    }
}