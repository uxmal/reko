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
    public class MipsRelocator : Relocator
    {
        private DecompilerEventListener dcSvc;

        public MipsRelocator(IServiceProvider services, Program program) : base(program)
        {
            dcSvc = services.RequireService<DecompilerEventListener>();
        }

public const short 	IMAGE_REL_MIPS_ABSOLUTE    	=  0x0000; // This relocation is ignored. 
public const short 	IMAGE_REL_MIPS_REFHALF    	=  0x0001; // The high 16 bits of the target's 32-bit virtual address. 
public const short 	IMAGE_REL_MIPS_REFWORD    	=  0x0002; // The target's 32-bit virtual address. 
public const short 	IMAGE_REL_MIPS_JMPADDR    	=  0x0003; // The low 26 bits of the target's virtual address. This supports the MIPS J and JAL instructions. 
public const short 	IMAGE_REL_MIPS_REFHI    	=  0x0004; // The high 16 bits of the target's 32-bit virtual address. Used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocations whose SymbolTableIndex contains a signed 16-bit displacement which is added to the upper 16 bits taken from the location being relocated. 
public const short 	IMAGE_REL_MIPS_REFLO    	=  0x0005; // The low 16 bits of the target's virtual address. 
public const short 	IMAGE_REL_MIPS_GPREL    	=  0x0006; // 16-bit signed displacement of the target relative to the Global Pointer (GP) register. 
public const short 	IMAGE_REL_MIPS_LITERAL    	=  0x0007; // Same as IMAGE_REL_MIPS_GPREL. 
public const short 	IMAGE_REL_MIPS_SECTION    	=  0x000A; // The 16-bit section index of the section containing the target. This is used to support debugging information. 
public const short 	IMAGE_REL_MIPS_SECREL    	=  0x000B; // The 32-bit offset of the target from the beginning of its section. This is used to support debugging information as well as static thread local storage. 
public const short 	IMAGE_REL_MIPS_SECRELLO    	=  0x000C; // The low 16 bits of the 32-bit offset of the target from the beginning of its section. 
public const short 	IMAGE_REL_MIPS_SECRELHI    	=  0x000D; // The high 16 bits of the 32-bit offset of the target from the beginning of its section. A PAIR relocation must immediately follow this on. The SymbolTableIndex of the PAIR relocation contains a signed 16-bit displacement, which is added to the upper 16 bits taken from the location being relocated. 
public const short  IMAGE_REL_MIPS_TOKEN        =  0x000E; // clr token
public const short 	IMAGE_REL_MIPS_JMPADDR16    =  0x0010; // The low 26 bits of the target's virtual address. This supports the MIPS16 JAL instruction. 
public const short 	IMAGE_REL_MIPS_REFWORDNB    =  0x0022; // The target's 32-bit relative virtual address. 
public const short 	IMAGE_REL_MIPS_PAIR    	    =  0x0025; // This relocation is only valid when it immediately follows a REFHI or SECRELHI relocation. Its SymbolTableIndex contains a displacement and not an index into the symbol table. 

        public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations)
        {
            ushort fixup = rdr.ReadUInt16();
            Address offset = baseOfImage + page + (fixup & 0x0FFFu);
            var arch = program.Architecture;
            var imgR = program.CreateImageReader(arch, offset);
            var imgW = program.CreateImageWriter(arch, offset);
            uint w = imgR.ReadUInt32();
            int s;
            switch (fixup >> 12)
            {
            case IMAGE_REL_MIPS_ABSOLUTE:
                // Used for padding to 4-byte boundary, ignore.
                break;
            case IMAGE_REL_MIPS_REFWORD:
                break;
            case IMAGE_REL_MIPS_JMPADDR:
                break;
            case IMAGE_REL_MIPS_REFHI:
                w = imgR.ReadUInt32();
                //w += (fixup & 0x0FFFu);
                //imgW.WriteUInt32(w);
                s = rdr.ReadInt16();
                w = (uint)(w + s);
                // w points to something.
                break;
            case IMAGE_REL_MIPS_REFLO:
                // w points to something.
                break;
            default:
                dcSvc.Warn(
                    dcSvc.CreateAddressNavigator(program, offset),
                    string.Format(
                        "Unsupported MIPS PE fixup type: {0:X}",
                        fixup >> 12));
                break;
            }
        }
    }
}