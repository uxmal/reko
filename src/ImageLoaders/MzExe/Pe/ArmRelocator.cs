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

namespace Reko.ImageLoaders.MzExe.Pe
{
    public class ArmRelocator : Relocator
    {
        public ArmRelocator(Program program) : base(program)
        {
        }

        // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format
        public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations)
        {
            ushort fixup = rdr.ReadLeUInt16();
            var rt = (ArmRt) (fixup >> 12);
            DebugEx.Verbose(PeImageLoader.trace, "  {0:X4} {1}", fixup, rt);
            switch (fixup)
            {

            }
        }

        public enum ArmRt
        {
            IMAGE_REL_ARM_ABSOLUTE = 0x0000,    // The relocation is ignored.
            IMAGE_REL_ARM_ADDR32 = 0x0001,      // The 32-bit VA of the target.
            IMAGE_REL_ARM_ADDR32NB = 0x0002,    // The 32-bit RVA of the target.
            IMAGE_REL_ARM_BRANCH24 = 0x0003,    // The 24-bit relative displacement to the target.
            IMAGE_REL_ARM_BRANCH11 = 0x0004,    // The reference to a subroutine call. The reference consists of two 16-bit instructions with 11-bit offsets.
            IMAGE_REL_ARM_REL32 = 0x000A,       // The 32-bit relative address from the byte following the relocation.
            IMAGE_REL_ARM_SECTION = 0x000E,     // The 16-bit section index of the section that contains the target. This is used to support debugging information.
            IMAGE_REL_ARM_SECREL = 0x000F,      // The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage.
            IMAGE_REL_ARM_MOV32 = 0x0010,       // The 32-bit VA of the target. This relocation is applied using a MOVW instruction for the low 16 bits followed by a MOVT for the high 16 bits.
            IMAGE_REL_THUMB_MOV32 = 0x0011,     // The 32-bit VA of the target. This relocation is applied using a MOVW instruction for the low 16 bits followed by a MOVT for the high 16 bits.
            IMAGE_REL_THUMB_BRANCH20 = 0x0012,  // The instruction is fixed up with the 21 - bit relative displacement to the 2 - byte aligned target.The least significant bit of the displacement is always zero and is not stored. This relocation corresponds to a Thumb - 2 32 - bit conditional B instruction.
            Unused = 0x0013,
            IMAGE_REL_THUMB_BRANCH24 = 0x0014,  // The instruction is fixed up with the 25 - bit relative displacement to the 2 - byte aligned target.The least significant bit of the displacement is zero and is not stored.This relocation corresponds to a Thumb - 2 B instruction.
            IMAGE_REL_THUMB_BLX23 = 0x0015,     //The instruction is fixed up with the 25-bit relative displacement to the 4-byte aligned target. The low 2 bits of the displacement are zero and are not stored.
                                                //This relocation corresponds to a Thumb - 2 BLX instruction.
            IMAGE_REL_ARM_PAIR = 0x0016,        // The relocation is valid only when it immediately follows a ARM_REFHI or THUMB_REFHI. Its SymbolTableIndex contains a displacement and not an index into the symbol table.             */
        }

    }
}