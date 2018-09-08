#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
    class x86Relocator : ElfRelocator32
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

                var addr = Address.Ptr32(offset);
                importReferences[addr] = new NamedImportReference(
                    addr, "", symStr);
            }
        }

        public override void RelocateEntry(Program program, ElfSymbol sym, ElfSection referringSection, ElfRelocation rela)
        {
            if (loader.Sections.Count <= sym.SectionIndex)
                return;
            if (sym.SectionIndex == 0)
                return;
            var symSection = loader.Sections[(int)sym.SectionIndex];
            uint S = (uint)sym.Value;
            int A = 0;
            int sh = 0;
            uint mask = ~0u;
            var addr = referringSection != null
                ? referringSection.Address + rela.Offset
                : loader.CreateAddress(rela.Offset);
            uint P = (uint)addr.ToLinear();
            uint PP = P;
            var relR = program.CreateImageReader(addr);
            var relW = program.CreateImageWriter(addr);
            var rt = (i386Rt)(rela.Info & 0xFF);
            switch (rt)
            {
            case i386Rt.R_386_NONE: //  just ignore (common)
                break;
            case i386Rt.R_386_COPY:
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
            var w = relR.ReadBeUInt32();
            w += ((uint)(S + A + P) >> sh) & mask;
            relW.WriteBeUInt32(w);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((i386Rt)type).ToString();
        }

        public void RelocateOld(Program program)
        {
            uint nextFakeLibAddr = ~1u; // See R_386_PC32 below; -1 sometimes used for main
            for (int i = 1; i < loader.Sections.Count; ++i)
            {
                var ps = loader.Sections[i];
                if (ps.Type == SectionHeaderType.SHT_REL)
                {
                    // A section such as .rel.dyn or .rel.plt (without an addend field).
                    // Each entry has 2 words: r_offset and r_info. The r_offset is just the offset from the beginning
                    // of the section (section given by the section header's sh_info) to the word to be modified.
                    // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                    // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                    // the section header's sh_link field.
                    var pReloc = loader.CreateReader(ps.FileOffset);
                    ulong  size = ps.Size;
                    // NOTE: the r_offset is different for .o files (ET_REL in the e_type header field) than for exe's
                    // and shared objects!
                    uint destNatOrigin = 0;
                    uint destHostOrigin = 0;
                    if ( loader.Header.e_type == ElfImageLoader.ET_REL)
                    {
                        var destSection = loader.Sections[i].RelocatedSection;
                        destNatOrigin = destSection.Address.ToUInt32();
                        destHostOrigin = (uint) destSection.FileOffset;
                    }
                    var symSection = loader.Sections[i].LinkedSection; // associated symbol table
                    var strSection = symSection.LinkedSection; // Section index for the string section assoc with this
                    var pStrSection = strSection.FileOffset;
                    var symOrigin = symSection.FileOffset;
                    var relocR = loader.CreateReader(0);
                    var relocW = loader.CreateWriter(0);
                    for (uint u = 0; u < size; u += 2 * sizeof(uint))
                    {
                        uint r_offset = pReloc.ReadUInt32();
                        uint info = pReloc.ReadUInt32();

                        byte relType = (byte)info;
                        uint symTabIndex = info >> 8;
                        uint pRelWord; // Pointer to the word to be relocated
                        if (loader.Header.e_type == ElfImageLoader.ET_REL)
                        {
                            pRelWord = destHostOrigin + r_offset;
                        }
                        else
                        {
                            if (r_offset == 0)
                                continue;
                            var destSec = loader.GetSectionInfoByAddr(r_offset);
                            pRelWord = ~0u; // destSec.uHostAddr - destSec.uNativeAddr + r_offset;
                            destNatOrigin = 0;
                        }
                        uint A, S = 0, P;
                        int nsec;
                        var sym = Elf32_Sym.Load(loader.CreateReader(symOrigin + symTabIndex * Elf32_Sym.Size));
                        switch (relType)
                        {
                        case 0: // R_386_NONE: just ignore (common)
                            break;
                        case 1: // R_386_32: S + A
                            // Read the symTabIndex'th symbol.
                            S = sym.st_value;
                            if (loader.Header.e_type == ElfImageLoader.ET_REL)
                            {
                                nsec = sym.st_shndx;
                                if (nsec >= 0 && nsec < loader.Sections.Count)
                                    S += loader.Sections[nsec].Address.ToUInt32();
                            }
                            A = relocR.ReadUInt32((int)pRelWord);
                            relocW.WriteUInt32(pRelWord, S + A);
                            break;
                        case 2: // R_386_PC32: S + A - P
                            if (ElfLoader32.ELF32_ST_TYPE(sym.st_info) == ElfLoader.STT_SECTION)
                            {
                                nsec = sym.st_shndx;
                                if (nsec >= 0 && nsec < loader.Sections.Count)
                                    S = loader.Sections[nsec].Address.ToUInt32();
                            }
                            else
                            {
                                S = sym.st_value;
                                if (S == 0)
                                {
                                    // This means that the symbol doesn't exist in this module, and is not accessed
                                    // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                                    // name of the symbol right here in the symbol table entry, but the only way
                                    // to communicate with the loader is through the target address of the call.
                                    // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                                    // in the symbol table
                                    uint nameOffset = sym.st_name;
                                    string pName = loader.ReadAsciiString(pStrSection + nameOffset);
                                    // this is too slow, I'm just going to assume it is 0
                                    //S = GetAddressByName(pName);
                                    //if (S == (e_type == E_REL ? 0x8000000 : 0)) {
                                    S = nextFakeLibAddr--; // Allocate a new fake address
                                    loader.AddSymbol(S, pName);
                                    //}
                                }
                                else if (loader.Header.e_type == ElfImageLoader.ET_REL)
                                {
                                    nsec = sym.st_shndx;
                                    if (nsec >= 0 && nsec < loader.Sections.Count)
                                        S += loader.Sections[nsec].Address.ToUInt32();
                                }
                            }
                            A = relocR.ReadUInt32((int)pRelWord);
                            P = destNatOrigin + r_offset;
                            relocW.WriteUInt32(pRelWord, S + A - P);
                            break;
                        case 6: // R_386_GLOB_DAT
                            // This relocation type is used to set a global offset table entry to the address of the
                            // specified symbol. The special relocation type allows one to determine the
                            // correspondence between symbols and global offset table entries.
                            S = sym.st_value;
                            relocW.WriteUInt32(pRelWord, S);
                            break;
                        case 7:
                        case 8: // R_386_RELATIVE
                            break; // No need to do anything with these, if a shared object
                        default:
                            throw new NotSupportedException("Relocation type " + (int)relType + " not handled yet");
                        }
                    }
                }
            }
        }
    }

    public enum i386Rt
    {

        R_386_NONE, // just ignore (common)
        R_386_32 = 1, // S + A
        R_386_PC32 = 2, // S + A - P
        R_386_COPY = 5,         // seems to do nothing according to ELF spec.
        R_386_GLOB_DAT = 6,
        R_386_RELATIVE = 8
    }
}
