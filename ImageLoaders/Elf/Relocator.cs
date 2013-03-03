using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    using ADDRESS = UInt32;

    public class Relocator
    {
        ElfImageLoader loader;
        private uint nextFakeLibAddr;
        int machine;
        int e_type;

        public Relocator(ElfImageLoader loader)
        {
            this.loader = loader;
            machine = loader.ElfHeader.e_machine;
            e_type = loader.ElfHeader.e_type;
        }

        public void ApplyRelocations()
        {
            nextFakeLibAddr = ~2U; // See R_386_PC32 below; -1 sometimes used for main
            switch (machine)
            {
            default:
                throw new NotImplementedException();
            case Elf32_Ehdr.EM_386:
                int i = 0;
                foreach (SectionInfo ps in loader.Sections)
                {
                    if (ps.uType == Elf32_Shdr.SHT_REL)
                    {
                        nextFakeLibAddr = Relocate386Section(ps, i);
                    }
                }
                break;
            }
        }

        private uint Relocate386Section(SectionInfo ps, int i)
        {
            // A section such as .rel.dyn or .rel.plt (without an addend field).
            // Each entry has 2 words: r_offet and r_info. The r_offset is just the offset from the beginning
            // of the section (section given by the section header's sh_info) to the word to be modified.
            // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
            // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
            // the section header's sh_link field.
            var pReloc = loader.CreateReader(ps.uHostAddr);
            uint size = ps.uSectionSize;
            // NOTE: the r_offset is different for .o files (E_REL in the e_type header field) than for exe's
            // and shared objects!
            ADDRESS destNatOrigin = 0, destHostOrigin = 0;
            if (e_type == Elf32_Ehdr.E_REL)
            {
                int destSection = loader.m_sh_info[i];
                destNatOrigin = loader.Sections[destSection].uNativeAddr;
                destHostOrigin = loader.Sections[destSection].uHostAddr;
            }
            int symSection = loader.m_sh_link[i]; // Section index for the associated symbol table
            int strSection = loader.m_sh_link[symSection]; // Section index for the string section assoc with this
            uint pStrSection = loader.Sections[strSection].uHostAddr;
            Elf32_Sym[] symOrigin = loader.LoadElf32Symbols(loader.Sections[symSection]);     // Elf32_sym's
            var rdr = loader.CreateReader(0);
            var wrt = loader.CreateWriter();
            for (uint u = 0; u < size; u += 2 * sizeof(uint))
            {
                uint r_offset = pReloc.ReadUInt32();
                uint info = pReloc.ReadUInt32();
                byte relType = (byte)info;
                uint symTabIndex = info >> 8;
                uint pRelWord; // Pointer to the word to be relocated
                if (e_type == Elf32_Ehdr.E_REL)
                    pRelWord = (destHostOrigin + r_offset);
                else
                {
                    if (r_offset == 0) continue;
                    SectionInfo destSec = loader.GetSectionInfoByAddr(r_offset);
                    pRelWord = (destSec.uHostAddr - destSec.uNativeAddr + r_offset);
                    destNatOrigin = 0;
                }
                ADDRESS A, S = 0, P;
                int nsec;
                switch (relType)
                {
                case 0: // R_386_NONE: just ignore (common)
                    break;
                case 1: // R_386_32: S + A
                    S = symOrigin[symTabIndex].st_value;
                    if (e_type == Elf32_Ehdr.E_REL)
                    {
                        nsec = symOrigin[symTabIndex].st_shndx;
                        if (0 <= nsec && nsec < loader.Sections.Length)
                            S += loader.GetSectionInfo(nsec).uNativeAddr;
                    }
                    A = rdr.ReadUInt32(pRelWord);
                    wrt.WriteUInt32(pRelWord, S + A);
                    break;
                case 2: // R_386_PC32: S + A - P
                    if (symOrigin[symTabIndex].ELF32_ST_TYPE == ElfImageLoader.STT_SECTION)
                    {
                        nsec = symOrigin[symTabIndex].st_shndx;
                        if (nsec >= 0 && nsec < loader.Sections.Length)
                            S = loader.GetSectionInfo(nsec).uNativeAddr;
                    }
                    else
                    {
                        S = symOrigin[symTabIndex].st_value;
                        if (S == 0)
                        {
                            // This means that the symbol doesn't exist in this module, and is not accessed
                            // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                            // name of the symbol right here in the symbol table entry, but the only way
                            // to communicate with the loader is through the target address of the call.
                            // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                            // in the symbol table
                            uint nameOffset = symOrigin[symTabIndex].st_name;
                            uint pName = pStrSection + nameOffset;
                            // this is too slow, I'm just going to assume it is 0
                            //S = GetAddressByName(pName);
                            //if (S == (e_type == E_REL ? 0x8000000 : 0)) {
                            S = nextFakeLibAddr--; // Allocate a new fake address
                            loader.AddSymbol(S, loader.ReadAsciizString(pName));
                            //}
                        }
                        else if (e_type == Elf32_Ehdr.E_REL)
                        {
                            nsec = symOrigin[symTabIndex].st_shndx;
                            if (nsec >= 0 && nsec < loader.Sections.Length)
                                S += loader.GetSectionInfo(nsec).uNativeAddr;
                        }
                    }
                    A = rdr.ReadUInt32(pRelWord);
                    P = destNatOrigin + r_offset;
                    wrt.WriteUInt32(pRelWord, S + A - P);
                    break;
                case 7:
                case 8: // R_386_RELATIVE
                    break; // No need to do anything with these, if a shared object
                default:
                    throw new NotImplementedException("Relocation type " + (int)relType + " not handled yet");
                }
            }
            return nextFakeLibAddr;
        }
    }
}
