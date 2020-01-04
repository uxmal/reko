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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class x86_64Relocator : ElfRelocator64
    {
        private Dictionary<Address, ImportReference> importReferences;

        public x86_64Relocator(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
            this.loader = loader;
        }

        /// <remarks>
        /// According to the ELF x86_64 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public override void Relocate(Program program)
        {
            this.importReferences = program.ImportReferences;
            base.Relocate(program);
            LoadImportReferencesFromRelaPlt();
        }

        private void LoadImportReferencesFromRelaPlt()
        {
            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            var plt = loader.GetSectionInfoByName(".plt");
            if (rela_plt == null || plt == null)
                return;
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            for (ulong i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                var rela = Elf64_Rela.Read(relaRdr);

                ulong sym = rela.r_info >> 32;
                var symStr = loader.Symbols[rela_plt.LinkedSection.FileOffset][(int)sym];

                var addr = plt.Address + (uint)(i + 1) * plt.EntrySize;
                var st = ElfLoader.GetSymbolType(symStr);
                if (st.HasValue)
                {
                    importReferences[addr] = new NamedImportReference(addr, null, symStr.Name, st.Value);
                }
            }
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol sym, ElfSection referringSection, ElfRelocation rela)
        {
            var rt = (x86_64Rt)(rela.Info & 0xFF);
            if (loader.Sections.Count <= sym.SectionIndex)
                return sym;
            if (rt == x86_64Rt.R_X86_64_GLOB_DAT ||
                rt == x86_64Rt.R_X86_64_JUMP_SLOT)
            {
                var addrPfn = Address.Ptr64(rela.Offset);

                var st = ElfLoader.GetSymbolType(sym);
                if (!st.HasValue)
                    return sym;
                importReferences.Add(addrPfn, new NamedImportReference(addrPfn, null, sym.Name, st.Value));
                var gotSym = loader.CreateGotSymbol(addrPfn, sym.Name);
                imageSymbols.Add(addrPfn, gotSym);
                return sym;
            }
            if (sym.SectionIndex == 0)
                return sym;
            var symSection = loader.Sections[(int)sym.SectionIndex];
            ulong S = (ulong)sym.Value + symSection.Address.ToLinear();
            long A = 0;
            int sh = 0;
            uint mask = ~0u;
            Address addr;
            ulong P;
            if (referringSection?.Address != null)
            {
                addr = referringSection.Address + rela.Offset;
                P = addr.ToLinear();
            }
            else
            {
                addr = Address.Ptr64(rela.Offset);
                P = 0;
            }
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);
            ulong PP = P;
            switch (rt)
            {
            case x86_64Rt.R_X86_64_NONE: //  just ignore (common)
                break;
            case x86_64Rt.R_X86_64_COPY:
                break;
            default:
                Debug.Print("x86_64 ELF relocation type {0} not implemented yet.",
                    rt);
                break;
                //throw new NotImplementedException(string.Format(
                //    "x86_64 ELF relocation type {0} not implemented yet.",
                //    rt));
            }
            if (relR != null)
            {
                var w = relR.ReadUInt64();
                w += ((ulong)(S + (ulong)A + P) >> sh) & mask;
                relW.WriteUInt64(w);
            }
            return sym;
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((x86_64Rt)type).ToString();
        }
        
        public override StartPattern[] GetStartPatterns()
        {
            return new[]
            {
                new StartPattern
                {
                     SearchPattern =
                        "31 ED" +                   // 00: xor ebp, ebp
                        "49 89 D1" +                // 02: mov r9, rdx
                        "5E" +                      // 05: pop rsi
                        "48 89 E2" +                // 06: mov rdx, rsp
                        "48 83 E4 F0" +             // 09: and rsp,F0
                        "50" +                      // 0D: push rax
                        "54" +                      // 0E: push rsp
                        "49 C7 C0 ?? ?? ?? ??" +    // 0F: mov r8,+004005B0
                        "48 C7 C1 ?? ?? ?? ??" +    // 16: mov rcx,+00400540
                        "48 C7 C7 ?? ?? ?? ??" +    // 1D: mov rdi,offset main
                        "E8 ?? ?? ?? ??",           // 24: call __libc_start_main
                     MainAddressOffset = 0x20
                }
            };
        }

        protected override Address GetMainFunctionAddress(IProcessorArchitecture arch, MemoryArea mem, int offset, StartPattern sPattern)
        {
            var uAddr = mem.ReadLeUInt32((uint)(offset + sPattern.MainAddressOffset));
            return Address.Ptr64(uAddr);
        }
    }

    public enum x86_64Rt
    {
        R_X86_64_NONE = 0,              // none none
        R_X86_64_64 = 1,                // word64 S + A
        R_X86_64_PC32 = 2,              // word32 S + A - P
        R_X86_64_GOT32 = 3,             // word32 G + A
        R_X86_64_PLT32 = 4,             // word32 L + A - P
        R_X86_64_COPY = 5,              // none none
        R_X86_64_GLOB_DAT = 6,          // word64 S
        R_X86_64_JUMP_SLOT = 7,         // word64 S
        R_X86_64_RELATIVE = 8,          // word64 B + A
        R_X86_64_GOTPCREL = 9,          // word32 G + GOT + A - P
        R_X86_64_32 = 10,               // word32 S + A
        R_X86_64_32S = 11,              // word32 S + A
        R_X86_64_16 = 12,               // word16 S + A
        R_X86_64_PC16 = 13,             // word16 S + A - P
        R_X86_64_8 = 14,                // word8 S + A
        R_X86_64_PC8 = 15,              // word8 S + A - P
        R_X86_64_DTPMOD64 = 16,         // word64
        R_X86_64_DTPOFF64 = 17,         // word64
        R_X86_64_TPOFF64 = 18,          // word64
        R_X86_64_TLSGD = 19,            // word32
        R_X86_64_TLSLD = 20,            // word32
        R_X86_64_DTPOFF32 = 21,         // word32
        R_X86_64_GOTTPOFF = 22,         // word32
        R_X86_64_TPOFF32 = 23,          // word32
        R_X86_64_PC64 = 24,             // word64 S + A - P
        R_X86_64_GOTOFF64 = 25,         // word64 S + A - GOT
        R_X86_64_GOTPC32 = 26,          // word32 GOT + A - P
        R_X86_64_SIZE32 = 32,           // word32 Z + A
        R_X86_64_SIZE64 = 33,           // word64 Z + A
        R_X86_64_GOTPC32_TLSDESC = 34,  // word32
        R_X86_64_TLSDESC_CALL = 35,     // none
        R_X86_64_TLSDESC = 36,          // word642
        R_X86_64_IRELATIVE = 37,        // word64 indirect(B + A)
    }
}
