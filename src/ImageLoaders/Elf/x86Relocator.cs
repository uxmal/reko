#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

namespace Reko.ImageLoaders.Elf
{
    class x86Relocator : ElfRelocator
    {
        private ElfLoader32 loader;
        private Dictionary<Address, ImportReference> importReferences;

        public x86Relocator(ElfLoader32 loader, Dictionary<Address, ImportReference> importReferences)
        {
            this.loader = loader;
            this.importReferences = importReferences;
        }

        public override void Relocate()
        {
            uint nextFakeLibAddr = ~1u; // See R_386_PC32 below; -1 sometimes used for main
            for (int i = 1; i < loader.SectionHeaders.Count; ++i)
            {
                var ps = loader.SectionHeaders[i];
                if (ps.sh_type == SectionHeaderType.SHT_REL)
                {
                    // A section such as .rel.dyn or .rel.plt (without an addend field).
                    // Each entry has 2 words: r_offset and r_info. The r_offset is just the offset from the beginning
                    // of the section (section given by the section header's sh_info) to the word to be modified.
                    // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                    // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                    // the section header's sh_link field.
                    var pReloc = loader.CreateReader(ps.sh_offset);
                    uint size = ps.sh_size;
                    // NOTE: the r_offset is different for .o files (ET_REL in the e_type header field) than for exe's
                    // and shared objects!
                    uint destNatOrigin = 0;
                    uint destHostOrigin = 0;
                    if ( loader.Header.e_type == ElfImageLoader.ET_REL)
                    {
                        int destSection = (int)loader.SectionHeaders[i].sh_info;
                        destNatOrigin = loader.SectionHeaders[destSection].sh_addr;
                        destHostOrigin = loader.SectionHeaders[destSection].sh_offset;
                    }
                    int symSection = (int)loader.SectionHeaders[i].sh_link; // Section index for the associated symbol table
                    int strSection = (int)loader.SectionHeaders[symSection].sh_link; // Section index for the string section assoc with this
                    uint pStrSection = loader. SectionHeaders[strSection].sh_offset;
                    var symOrigin = loader.SectionHeaders[symSection].sh_offset;
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
                                if (nsec >= 0 && nsec < loader.SectionHeaders.Count)
                                    S += loader.SectionHeaders[nsec].sh_addr;
                            }
                            A = relocR.ReadUInt32(pRelWord);
                            relocW.WriteUInt32(pRelWord, S + A);
                            break;
                        case 2: // R_386_PC32: S + A - P
                            if (ElfLoader32.ELF32_ST_TYPE(sym.st_info) == ElfLoader.STT_SECTION)
                            {
                                nsec = sym.st_shndx;
                                if (nsec >= 0 && nsec < loader.SectionHeaders.Count)
                                    S = loader.SectionHeaders[nsec].sh_addr;
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
                                    if (nsec >= 0 && nsec < loader.SectionHeaders.Count)
                                        S += loader.SectionHeaders[nsec].sh_addr;
                                }
                            }
                            A = relocR.ReadUInt32(pRelWord);
                            P = destNatOrigin + r_offset;
                            relocW.WriteUInt32(pRelWord, S + A - P);
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
}
