#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class ArmRelocator : ElfRelocator32
    {
        IProcessorArchitecture archThumb;

        public ArmRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override ImageSymbol AdjustImageSymbol(ImageSymbol sym)
        {
            if (sym.Type != SymbolType.Code &&
                sym.Type != SymbolType.ExternalProcedure &&
                sym.Type != SymbolType.Procedure)
                return sym;
            if ((sym.Address.ToLinear() & 1) == 0)
                return sym;
            if (archThumb == null)
            {
                var cfgSvc = loader.Services.RequireService<IConfigurationService>();
                this.archThumb = cfgSvc.GetArchitecture("arm-thumb");
            }
            var addrNew = sym.Address - 1;
            var symNew = ImageSymbol.Create(
                sym.Type,
                archThumb,
                addrNew,
                sym.Name,
                sym.DataType,
                !sym.NoDecompile);
            symNew.ProcessorState = sym.ProcessorState;
            return symNew;
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela)
        {
            /*
            S (when used on its own) is the address of the symbol
            A is the addend for the relocation.
            P is the address of the place being relocated (derived from r_offset).
            * Pa is the adjusted address of the place being reloc
            ated, defined as (P & 0xFFFFFFFC). 
            * T is 1 if the target symbol S has type STT_FUNC and the symbol addresses a Thumb instruction; it is 0 
            otherwise.
            * B(S) is the addressing origin of the output segment defining the symbol S. The origin is not required to be 
            the base address of the segment. This value must always be word-aligned.

            * GOT_ORG is the addressing origin of the Global Offset Table (the indirection table for imported data 
            addresses).  This value must always be word-aligned.  See §4.6.1.8, Proxy generating relocations. 
            * GOT(S) is the address of the GOT entry for the symbol S.
            Table 
            0   R_ARM_NONE      Static      Miscellaneous
            1   R_ARM_PC24      Deprecated  ARM             ((S + A) | T) - P
            2   R_ARM_ABS32     Static      Data            ((S + A) | T)
            3   R_ARM_REL32     Static      Data            ((S + A) | T) – P
            4   R_ARM_LDR_PC_G0 Static      ARM             S + A – P
            5   R_ARM_ABS16     Static      Data            S + A
            6   R_ARM_ABS12     Static      ARM             S + A
            7   R_ARM_THM_ABS5  Static      Thumb16         S + A
            8   R_ARM_ABS8      Static      Data            S + A
            9   R_ARM_SBREL32   Static      Data            ((S + A) | T) – B(S)
            20  R_ARM_COPY                                  S
            21  R_ARM_GLOB_DAT Dynamic      Data            (S + A) | T 
            22  R_ARM_JUMP_SLOT Dynamic     Data            (S + A) | T 
            23  R_ARM_RELATIVE Dynamic      Data            B(S) + A  [Note: see Table 4-18]
            */
            var addr = referringSection != null
                ? referringSection.Address + rela.Offset
                : loader.CreateAddress(rela.Offset);

            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            var A = rela.Addend;
            uint S = (uint) symbol.Value;
            uint mask = ~0u;
            int sh = 0;
            var rt = (Arm32Rt)(rela.Info & 0xFF);
            switch (rt)
            {
            case Arm32Rt.R_ARM_COPY:
                A = S = 0;
                break;
            case Arm32Rt.R_ARM_ABS32:
                break;
            case Arm32Rt.R_ARM_GLOB_DAT:
            case Arm32Rt.R_ARM_JUMP_SLOT:
                A = S = 0;
                break;
            case Arm32Rt.R_ARM_RELATIVE:
                // From the docs:
                //
                // (S ≠ 0) B(S) resolves to the difference between the address
                // at which the segment defining the symbol S was loaded and
                // the address at which it was linked. 
                // (S = 0) B(S) resolves to the difference between the address
                // at which the segment being relocated was loaded and the
                // address at which it was linked.
                //
                // Reko always loads objects at their specified physical address, 
                // so this relocation is a no-op;
                A = S = 0;
                break;
            default:
                throw new NotImplementedException($"AArch32 relocation type {rt} is not implemented yet.");
            }
            var w = relR.ReadLeUInt32();
            w += ((uint) (S + A) >> sh) & mask;
            relW.WriteLeUInt32(w);

            return symbol;
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Arm32Rt)type).ToString();
        }
    }

    // http://infocenter.arm.com/help/topic/com.arm.doc.ihi0044f/IHI0044F_aaelf.pdf
    public enum Arm32Rt
    {
        R_ARM_NONE = 0,
        R_ARM_PC24 = 1,
        R_ARM_ABS32 = 2,
        R_ARM_REL32 = 3,
        R_ARM_LDR_PC_G0 = 4,
        R_ARM_ABS16 = 5,
        R_ARM_ABS12 = 6,
        R_ARM_THM_ABS5 = 7,
        R_ARM_ABS8 = 8,
        R_ARM_SBREL32 = 9,

        R_ARM_COPY = 20,
        R_ARM_GLOB_DAT = 21,
        R_ARM_JUMP_SLOT = 22,
        R_ARM_RELATIVE = 23,
    }
}
