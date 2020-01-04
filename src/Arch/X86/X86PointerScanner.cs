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

namespace Reko.Arch.X86
{
    /// <summary>
    /// Scans an image looking for uses of pointer values.
    /// </summary>
    public class X86PointerScanner32 : PointerScanner<uint>
    {
        public X86PointerScanner32(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags) : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 1; } }

        public override uint GetLinearAddress(Address address)
        {
            return address.ToUInt32();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE8 // CALL NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = (uint) (callOffset + (uint)rdr.Address.ToLinear() + 5);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE9 // JMP NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 5u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = (uint) (callOffset + (int)rdr.Address.ToLinear() + 5);
                return true;
            }
            if (0x70 <= opcode && opcode <= 0x7F &&       // short branch.
                rdr.IsValidOffset(rdr.Offset + 1u))
            {
                sbyte callOffset = rdr.PeekSByte(1);
                target = (uint) ((int)rdr.Address.ToLinear() + callOffset + 2);
                return true;
            }
            if (opcode == 0x0F && rdr.IsValidOffset(rdr.Offset + 5u))
            {
                opcode = rdr.PeekByte(1);
                int callOffset = rdr.PeekLeInt32(2);
                uint linAddr = (uint)rdr.Address.ToLinear();
                if (0x80 <= opcode && opcode <= 0x8F)   // long branch
                {
                    target = (uint) (callOffset + linAddr + 6);
                    return true;
                }
            }
            target = 0;
            return false;
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            byte bOpcode;
            bool ret = rdr.TryPeekByte(0, out bOpcode);
            opcode = bOpcode;
            return ret;
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
                target = rdr.PeekLeUInt32(0);
                return true;
            }
        }
    }

    public class X86PointerScanner64 : PointerScanner<ulong>
    {
        public X86PointerScanner64(EndianImageReader rdr, HashSet<ulong> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 1; } }

        public override ulong GetLinearAddress(Address address)
        {
            return address.ToLinear();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out ulong target)
        {
            if (opcode == 0xE8 // CALL NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 4u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = ((ulong)(long)callOffset + rdr.Address.ToLinear() + 5);
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out ulong target)
        {
            if (opcode == 0xE9 // JMP NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 5u))
            {
                int callOffset = rdr.PeekLeInt32(1);
                target = ((ulong)(long)callOffset + rdr.Address.ToLinear() + 5);
                return true;
            }
            if (0x70 <= opcode && opcode <= 0x7F &&       // short branch.
                rdr.IsValidOffset(rdr.Offset + 1u))
            {
                sbyte callOffset = rdr.PeekSByte(1);
                target = (rdr.Address.ToLinear() + (ulong)(long)callOffset + 2);
                return true;
            }
            if (opcode == 0x0F && rdr.IsValidOffset(rdr.Offset + 5u))
            {
                opcode = rdr.PeekByte(1);
                int callOffset = rdr.PeekLeInt32(2);
                uint linAddr = (uint)rdr.Address.ToLinear();
                if (0x80 <= opcode && opcode <= 0x8F)   // long branch
                {
                    target = (ulong)(callOffset + linAddr + 6);
                    return true;
                }
            }
            target = 0;
            return false;
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            byte bOpcode;
            bool ret = rdr.TryPeekByte(0, out bOpcode);
            opcode = bOpcode;
            return ret;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out ulong target)
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

    public class X86RealModePointerScanner : PointerScanner<uint>
    {
        public X86RealModePointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override int PointerAlignment { get { return 1; } }

        public override uint GetLinearAddress(Address address)
        {
            return (uint)address.ToLinear();
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            byte bOpcode;
            bool ret = rdr.TryPeekByte(0, out bOpcode);
            opcode = bOpcode;
            return ret;
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE8 // CALL NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 2u))
            {
                int callOffset = rdr.PeekLeInt16(1);
                target = (uint)(callOffset + (uint)rdr.Address.ToLinear() + 3);
                return true;
            }
            if (opcode == 0x9A // CALL FAR
                 &&
                rdr.IsValidOffset(rdr.Offset + 5u))
            {
                ushort callOff = rdr.PeekLeUInt16(1);
                ushort callSeg = rdr.PeekLeUInt16(3);
                target = ((uint)callSeg << 4) + callOff;
                return true;
            }
            target = 0;
            return false;
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if (opcode == 0xE9 // JMP NEAR
                &&
                rdr.IsValidOffset(rdr.Offset + 3u))
            {
                int callOffset = rdr.PeekLeInt16(1);
                target = (uint)(callOffset + (uint)rdr.Address.ToLinear() + 3);
                return true;
            }
            if (0x70 <= opcode && opcode <= 0x7F &&       // short branch.
                rdr.IsValidOffset(rdr.Offset + 1u))
            {
                sbyte callOffset = rdr.PeekSByte(1);
                target = (uint)((uint)rdr.Address.ToLinear() + callOffset + 2);
                return true;
            }
            if (opcode == 0x0F && rdr.IsValidOffset(rdr.Offset + 4u))
            {
                opcode = rdr.PeekByte(1);
                int callOffset = rdr.PeekLeInt16(2);
                uint linAddr = (uint)rdr.Address.ToLinear();
                if (0x80 <= opcode && opcode <= 0x8F)   // long branch
                {
                    target = (uint)(callOffset + linAddr + 4);
                    return true;
                }
            }
            if (opcode == 0xEA // JMP FAR
                &&
                rdr.IsValidOffset(rdr.Offset + 5u))
            {
                ushort jmpOffset = rdr.PeekLeUInt16(1);
                ushort jmpSeg = rdr.PeekLeUInt16(3);
                target = ((uint)jmpSeg << 4) + jmpOffset;
                return true;
            }
            target = 0;
            return false;
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            if (!rdr.IsValidOffset(rdr.Offset + 2 - 1))
            {
                target = 0;
                return false;
            }
            else
            {
                target = rdr.PeekLeUInt16(0);
                return true;
            }
        }
    }
}