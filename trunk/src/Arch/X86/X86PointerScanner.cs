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

namespace Decompiler.Arch.X86
{
    /// <summary>
    /// Scans an image looking for uses of pointer values.
    /// </summary>
    public class X86PointerScanner : PointerScanner
    {
        public X86PointerScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags) : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 1; } }

        public override uint ReadOpcode(ImageReader rdr)
        {
            return rdr.PeekByte(0);
        }

        public override bool MatchCall(ImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE8 // CALL NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = (uint) (callOffset + rdr.Address.Linear + 5);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(ImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE9 // JMP NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 5u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = (uint) (callOffset + rdr.Address.Linear + 5);
                return true;
            }
            if (0x70 <= opcode && opcode <= 0x7F &&       // short branch.
                rdr.IsValidOffset(rdr.Offset + 1u))
            {
                sbyte callOffset = rdr.PeekSByte(1);
                target = (uint) (rdr.Address.Linear + callOffset + 2);
                return true;
            }
            if (opcode == 0x0F && rdr.IsValidOffset(rdr.Offset + 5u))
            {
                opcode = rdr.PeekByte(1);
                int callOffset = rdr.PeekLeInt32(2);
                uint linAddr = rdr.Address.Linear;
                if (0x80 <= opcode && opcode <= 0x8F)   // long branch
                {
                    target = (uint) (callOffset + linAddr + 6);
                    return true;
                }
            }
            target = 0;
            return false;
        }

        public override bool PeekPointer(ImageReader rdr, out uint target)
        {
            if (!rdr.IsValidOffset(rdr.Offset + 4 - 1))
            {
                target = 0;
                return false;
            }
            else
            {
                target = rdr.PeekLeUInt32(0);
                return true;
            }
        }
    }
}