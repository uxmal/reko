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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Pe
{
    public class Arm64Relocator : Relocator
    {
        private DecompilerEventListener eventListener;

        public Arm64Relocator(IServiceProvider services, Program program) : base(program)
        {
            this.eventListener = services.RequireService<DecompilerEventListener>();
        }

        // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format
        public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations)
        {
            ushort fixup = rdr.ReadLeUInt16();
            var rt = (Arm64Rt) (fixup >> 12);
            Address offset = baseOfImage + page + (fixup & 0x0FFFu);
            DebugEx.Verbose(PeImageLoader.trace, "  {0:X4} {1}", fixup, rt);
            var imgR = program.CreateImageReader(program.Architecture, offset);
            var imgW = program.CreateImageWriter(program.Architecture, offset);
            switch (rt)
            {
            case Arm64Rt.IMAGE_REL_ARM64_ABSOLUTE:
                break;
            case Arm64Rt.IMAGE_REL_ARM64_SECREL_HIGH12A:
                var uInstr = imgR.ReadLeUInt32();
                break;
            default:
                eventListener.Warn(
                    eventListener.CreateAddressNavigator(program, offset),
                    string.Format(
                        "Unsupported AArch64 PE fixup type: {0:X}",
                        fixup >> 12));
                break;


            }
        }

        public enum Arm64Rt : short
        {
            IMAGE_REL_ARM64_ABSOLUTE = 0x0000,      // The relocation is ignored.
            IMAGE_REL_ARM64_ADDR32 = 0x0001,        // The 32-bit VA of the target.
            IMAGE_REL_ARM64_ADDR32NB = 0x0002,      // The 32-bit RVA of the target.
            IMAGE_REL_ARM64_BRANCH26 = 0x0003,      // The 26-bit relative displacement to the target, for B and BL instructions.
            IMAGE_REL_ARM64_PAGEBASE_REL21 = 0x0004, // The page base of the target, for ADRP instruction.
            IMAGE_REL_ARM64_REL21 = 0x0005,         // The 12-bit relative displacement to the target, for instruction ADR
            IMAGE_REL_ARM64_PAGEOFFSET_12A = 0x0006, // The 12-bit page offset of the target, for instructions ADD/ADDS (immediate) with zero shift.
            IMAGE_REL_ARM64_PAGEOFFSET_12L = 0x0007, // The 12-bit page offset of the target, for instruction LDR (indexed, unsigned immediate).
            IMAGE_REL_ARM64_SECREL = 0x0008,        // The 32-bit offset of the target from the beginning of its section.This is used to support debugging information and static thread local storage.
            IMAGE_REL_ARM64_SECREL_LOW12A = 0x0009, // Bit 0:11 of section offset of the target, for instructions ADD/ADDS(immediate) with zero shift.
            IMAGE_REL_ARM64_SECREL_HIGH12A = 0x000A, // Bit 12:23 of section offset of the target, for instructions ADD/ADDS(immediate) with zero shift.
            IMAGE_REL_ARM64_SECREL_LOW12L = 0x000B, // Bit 0:11 of section offset of the target, for instruction LDR(indexed, unsigned immediate).
            IMAGE_REL_ARM64_TOKEN = 0x000C,         // CLR token.
            IMAGE_REL_ARM64_SECTION = 0x000D,       // The 16-bit section index of the section that contains the target. This is used to support debugging information.
            IMAGE_REL_ARM64_ADDR64 = 0x000E,        // The 64-bit VA of the relocation target.
            IMAGE_REL_ARM64_BRANCH19 = 0x000F,      // The 19-bit offset to the relocation target, for conditional B instruction.
            IMAGE_REL_ARM64_BRANCH14 = 0x0010,      // The 14-bit offset to the relocation target, for instructions TBZ and TBNZ.
            IMAGE_REL_ARM64_REL32 = 0x0011,         // The 32-bit relative address from the byte following the relocation.
        }
    }
}
