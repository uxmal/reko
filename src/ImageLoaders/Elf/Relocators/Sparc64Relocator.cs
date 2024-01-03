#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class Sparc64Relocator : ElfRelocator64
    {
        private Dictionary<Address, ImportReference> importReferences;

        public Sparc64Relocator(ElfLoader64 elfLoader, SortedList<Address, ImageSymbol> symbols) : 
            base(elfLoader, symbols)
        {
            importReferences = null!;
        }

        public override void Relocate(Program program)
        {
            this.importReferences = program.ImportReferences;
            base.Relocate(program);
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol sym, ElfSection? referringSection, ElfRelocation rela)
        {
            if (loader.Sections.Count <= sym.SectionIndex)
                return (null, null);
            var rt = (SparcRt) (rela.Info & 0xFF);

            var addr = referringSection != null
                 ? referringSection.Address! + rela.Offset
                 : loader.CreateAddress(rela.Offset);
            if (sym.SectionIndex == 0)
            {
                if (rt == SparcRt.R_SPARC_GLOB_DAT ||
                    rt == SparcRt.R_SPARC_JMP_SLOT)
                {
                    var addrPfn = Address.Ptr32((uint) rela.Offset);
                    ElfImageLoader.trace.Verbose("Import reference {0} - {1}", addrPfn, sym.Name);
                    var st = ElfLoader.GetSymbolType(sym);
                    if (st.HasValue)
                    {
                        importReferences[addrPfn] = new NamedImportReference(addrPfn, null, sym.Name, st.Value);
                    }
                    return (addrPfn, null);
                }
            }

            var symSection = loader.Sections[(int) sym.SectionIndex];
            ulong S = 0;
            int A = 0;
            int sh = 0;
            uint mask = ~0u;
            if (referringSection != null)
            {
                addr = referringSection.Address! + rela.Offset;
            }
            else
            {
                addr = Address.Ptr64(rela.Offset);
            }
            ulong P = addr.ToLinear();
            ulong PP = P;
            ulong B = 0;

            ElfImageLoader.trace.Verbose("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2} {5}",
                rela.Offset,
                (SparcRt) (rela.Info & 0xFF),
                sym.Name,
                rela.Addend.HasValue ? rela.Addend.Value : 0,
                (int) (rela.Info >> 8),
                "section?");

            switch (rt)
            {
            case 0:
                return (null, null);
            case SparcRt.R_SPARC_HI22:
                A = (int) rela.Addend!.Value;
                sh = 10;
                P = 0;
                mask = 0x3FFFFF;
                return Relocate32(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_LM22:
                A = (int) rela.Addend!.Value;
                S = sym.Value;
                sh = 10;
                P = 0;
                mask = 0x3FFFFF;
                return Relocate32(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_LO10:
                A = (int) rela.Addend!.Value;
                S = sym.Value;
                mask = 0x3FF;
                P = 0;
                return Relocate32(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_32:
                A = (int) rela.Addend!.Value;
                S = sym.Value;
                mask = 0xFFFFFFFF;
                P = 0;
                return Relocate32(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_WDISP30:
                A = (int) rela.Addend!.Value;
                P = ~P + 1;
                sh = 2;
                return Relocate32(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_RELATIVE:
                A = (int) rela.Addend!.Value;
                B = program.SegmentMap.BaseAddress.ToLinear();
                P = 0;
                return Relocate64(program, sym, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_COPY:
                ElfImageLoader.trace.Warn("Relocation type {0} not handled yet.", rt);
                return (addr, null);
            default:
                ElfImageLoader.trace.Error(
                    "SPARC ELF relocation type {0} not implemented yet.",
                    rt);
                return (addr, null);
            }
        }

        private (Address?, ElfSymbol?) Relocate32(Program program, ElfSymbol sym, Address addr, ulong S, int A, int sh, uint mask, ulong P, ulong B)
        {
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            var w = relR.ReadBeUInt32();
            var wOld = w;
            w += ((uint) (B + S + (uint) A + P) >> sh) & mask;
            relW.WriteBeUInt32(w);
            ElfImageLoader.trace.Verbose($"Relocated value at {addr} from {wOld:X8} to {w:X8}.");
            return (addr, null);
        }

        private (Address?, ElfSymbol?) Relocate64(Program program, ElfSymbol sym, Address addr, ulong S, int A, int sh, uint mask, ulong P, ulong B)
        {
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            var w = relR.ReadBeUInt64();
            var wOld = w;
            w += ((ulong) (B + S + (ulong) (long) A + P) >> sh) & mask;
            relW.WriteBeUInt64(w);
            ElfImageLoader.trace.Verbose($"Relocated value at {addr} from {wOld:X8} to {w:X8}.");
            return (addr, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return type.ToString();
        }
    }
}
