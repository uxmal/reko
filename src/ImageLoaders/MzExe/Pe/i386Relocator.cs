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

namespace Reko.ImageLoaders.MzExe.Pe
{
    public class i386Relocator : Relocator
    {
        private const ushort RelocationAbsolute = 0;
        private const ushort RelocationHigh = 1;
        private const ushort RelocationLow = 2;
        private const ushort RelocationHighLow = 3;

        private DecompilerEventListener dcSvc;

        public i386Relocator(IServiceProvider services, Program program)
         : base(program)
        {
            this.dcSvc = services.RequireService<DecompilerEventListener>();
        }

        public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations)
		{
			ushort fixup = rdr.ReadLeUInt16();
			Address offset = baseOfImage + page + (fixup & 0x0FFFu);
            var imgR = program.CreateImageReader(program.Architecture, offset);
            var imgW = program.CreateImageWriter(program.Architecture, offset);
            switch (fixup >> 12)
			{
			case RelocationAbsolute:
				// Used for padding to 4-byte boundary, ignore.
				break;
			case RelocationHighLow:
			{
				uint n = (uint) (imgR.ReadUInt32() + (baseOfImage - program.ImageMap.BaseAddress));
				imgW.WriteUInt32(n);
				relocations.AddPointerReference(offset.ToLinear(), n);
				break;
			}
            case 0xA:
            break;
			default:
                dcSvc.Warn(
                    dcSvc.CreateAddressNavigator(program, offset),
                    string.Format(
                        "Unsupported i386 PE fixup type: {0:X}",
                        fixup >> 12));
                break;
			}
        }


        /*
IMAGE_REL_I386_ABSOLUTE     0x0000  The relocation is ignored.
IMAGE_REL_I386_DIR16        0x0001  Not supported.
IMAGE_REL_I386_REL16        0x0002  Not supported.
IMAGE_REL_I386_DIR32        0x0006  The target's 32-bit VA.
IMAGE_REL_I386_DIR32NB      0x0007  The target's 32-bit RVA.
IMAGE_REL_I386_SEG12        0x0009  Not supported.
IMAGE_REL_I386_SECTION      0x000A  The 16-bit section index of the section that contains the target. This is used to support debugging information.
IMAGE_REL_I386_SECREL       0x000B  The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage.
IMAGE_REL_I386_TOKEN        0x000C  The CLR token.
IMAGE_REL_I386_SECREL7      0x000D  A 7-bit offset from the base of the section that contains the target.
IMAGE_REL_I386_REL32        0x0014  The 32-bit relative displacement to the target. This supports the x86 relative branch and call instructions.
*/
    }
}
 