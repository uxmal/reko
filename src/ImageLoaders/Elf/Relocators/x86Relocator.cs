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

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class x86Relocator : ElfRelocator32
    {
        public x86Relocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void Relocate(Program program)
        {
            base.Relocate(program);

            LoadImportReferencesFromRelPlt(program.ImportReferences);
        }

        private void LoadImportReferencesFromRelPlt(Dictionary<Address,ImportReference> importReferences)
        {
            var rel_plt = loader.GetSectionInfoByName(".rel.plt");
            if (rel_plt == null)
                return;
            var symtab = rel_plt.LinkedSection;
            var relRdr = loader.CreateReader(rel_plt.FileOffset);

            uint entries = rel_plt.EntryCount();
            for (uint i = 0; i < entries; ++i)
            {
                if (!relRdr.TryReadUInt32(out uint offset))
                    return;
                if (!relRdr.TryReadUInt32(out uint info))
                    return;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(symtab, sym);

                //$REFACTOR: use common code here. 
                var addr = Address.Ptr32(offset);
                importReferences[addr] = new NamedImportReference(
                    addr, "", symStr, SymbolType.ExternalProcedure);
            }
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol sym, ElfSection referringSection, ElfRelocation rela)
        {
            if (loader.Sections.Count <= sym.SectionIndex)
                return sym;
            uint S = (uint)sym.Value;
            int A = 0;
            int sh = 0;
            uint mask = ~0u;
            var addr = referringSection != null
                ? referringSection.Address + rela.Offset
                : loader.CreateAddress(rela.Offset);
            uint P = (uint)addr.ToLinear();
            uint PP = P;
            uint B = 0;
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);
            var rt = (i386Rt)(rela.Info & 0xFF);
            switch (rt)
            {
            case i386Rt.R_386_NONE: //  just ignore (common)
                break;
            case i386Rt.R_386_COPY:
                break;
            case i386Rt.R_386_RELATIVE: // B + A
                A = (int)rela.Addend;
                B = program.SegmentMap.BaseAddress.ToUInt32();
                break;
            case i386Rt.R_386_JMP_SLOT:
                if (sym.Value == 0)
                {
                    // Broken GCC compilers generate relocations referring to symbols 
                    // whose value is 0 instead of the expected address of the PLT stub.
                    var gotEntry = relR.PeekLeUInt32(0);
                    return CreatePltStubSymbolFromRelocation(sym, gotEntry, 6);
                }
                break;
            case i386Rt.R_386_32: // S + A
                                  // Read the symTabIndex'th symbol.
                A = (int)rela.Addend;
                P = 0;
                break;
            case i386Rt.R_386_PC32: // S + A - P
                if (sym.Value == 0)
                {
                    // This means that the symbol doesn't exist in this module, and is not accessed
                    // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                    // name of the symbol right here in the symbol table entry, but the only way
                    // to communicate with the loader is through the target address of the call.
                    // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                    // in the symbol table
                    //S = nextFakeLibAddr--; // Allocate a new fake address
                    //loader.AddSymbol(S, sym.Name);
                    //}
                }
                A = (int)rela.Addend;
                P = ~P + 1;
                break;
            case i386Rt.R_386_GLOB_DAT:
                // This relocation type is used to set a global offset table entry to the address of the
                // specified symbol. The special relocation type allows one to determine the
                // correspondence between symbols and global offset table entries.
                P = 0;
                break;
            default:
                throw new NotImplementedException(string.Format(
                    "i386 ELF relocation type {0} not implemented yet.",
                    rt));
            }
            var w = relR.ReadLeUInt32();
            w += ((uint)(B + S + A + P) >> sh) & mask;
            relW.WriteLeUInt32(w);
            return sym;
        }



        public override string RelocationTypeToString(uint type)
        {
            return ((i386Rt)type).ToString();
        }
    }

    public enum i386Rt
    {
        R_386_NONE,             // just ignore (common)
        R_386_32 = 1,           // S + A
        R_386_PC32 = 2,         // S + A - P
        R_386_COPY = 5,         // seems to do nothing according to ELF spec.
        R_386_GLOB_DAT = 6,
        R_386_JMP_SLOT = 7,     // S
        R_386_RELATIVE = 8
    }
}
