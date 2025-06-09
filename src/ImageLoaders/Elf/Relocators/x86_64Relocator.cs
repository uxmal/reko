#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class x86_64Relocator : ElfRelocator64
    {
        private Dictionary<ElfSymbol, Address> plt;

        public x86_64Relocator(
            ElfLoader64 loader,
            SortedList<Address, ImageSymbol> imageSymbols,
            Dictionary<ElfSymbol, Address> plt) : base(loader, imageSymbols)
        {
            this.loader = loader;
            this.plt = plt;
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
        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            base.Relocate(program, addrBase, pltEntries);
            LoadImportReferencesFromRelaPlt(program.ImportReferences);
        }

        private void LoadImportReferencesFromRelaPlt(Dictionary<Address, ImportReference> importReferences)
        {
            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            var plt = loader.GetSectionInfoByName(".plt");
            if (rela_plt is null || plt is null)
                return;
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            for (ulong i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                var rela = Elf64_Rela.Read(relaRdr);

                ulong sym = rela.r_info >> 32;
                var fileOff = rela_plt.LinkedSection!.FileOffset;
                if (fileOff == 0)
                    continue;
                var symStr = loader.BinaryImage.SymbolsByFileOffset[fileOff][(int)sym];

                var addr = plt.VirtualAddress! + (uint)(i + 1) * plt.EntrySize;
                var st = ElfLoader.GetSymbolType(symStr);
                if (st.HasValue)
                {
                    importReferences[addr] = new NamedImportReference(addr, null, symStr.Name, st.Value);
                }
            }
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = Address.Ptr64(ctx.P);
            var rt = (x86_64Rt) (rela.Info & 0xFF);
            switch (rt)
            {
            case x86_64Rt.R_X86_64_GLOB_DAT:    // S
            case x86_64Rt.R_X86_64_JUMP_SLOT:   // S
                var addrPfn = addr;
                ctx.AddImportReference(symbol, addr);
                ctx.AddGotSymbol(symbol, addrPfn);
                return (addrPfn, null);
            case x86_64Rt.R_X86_64_PC32:    // S + A - P
                ctx.WriteUInt32(addr, ctx.S + ctx.A - ctx.P);
                break;
            case x86_64Rt.R_X86_64_PLT32:   // L + A - P
                if (ctx.L == 0)
                {
                    Debug.Print("ELF external symbol {0} not present in PLT", symbol);
                    return (null, null);
                }
                ctx.WriteUInt32(addr, ctx.L + ctx.A - ctx.P);
                break;
            default:
                Debug.Print("x86_64 ELF relocation type {0} not implemented yet.",
                    rt);
                break;
            }
            return (addr, symbol);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((x86_64Rt)type).ToString();
        }
        
        public override StartPattern[] GetStartPatterns()
        {
            return new[]
            {
                new StartPattern    // Found in position dependent executables.
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
                },
                new StartPattern // Found in PIC executables
                {
                    SearchPattern =
                        "31 ED" +                  // 00: xor ebp,ebp
                        "49 89 D1" +               // 02: mov r9,rdx
                        "5E" +                     // 05: pop rsi
                        "48 89 E2" +               // 06: mov rdx,rsp
                        "48 83 E4 F0" +            // 09: and rsp,0F0h
                        "50" +                     // 0D: push rax
                        "54" +                     // 0E: push rsp
                        "45 31 C0" +               // 0F: xor r8d,r8d
                        "31 C9" +                  // 12: xor ecx,ecx
                        "48 8D 3D ?? ?? ?? ??" +   // 14: lea rdi,[main]; [rip+????????]
                        "FF 15 ?? ?? ?? ??" +      // 1B: call [__libc_start_main@got] ; [rip+????????]
                        "F4",                      // 21: hlt
                    MainPcRelativeOffset = 0x17,
                }
            };
        }

        protected override Address? GetMainFunctionAddress(IProcessorArchitecture arch, ByteMemoryArea mem, int offset, StartPattern sPattern)
        {
            if (sPattern.MainAddressOffset > 0)
            {
                var uAddr = mem.ReadLeUInt32((uint) (offset + sPattern.MainAddressOffset));
                return Address.Ptr64(uAddr);
            }
            else
            {
                Debug.Assert(sPattern.MainPcRelativeOffset > 0);
                var pcOffset = offset + sPattern.MainPcRelativeOffset;
                var uPcRelative = mem.ReadLeInt32((uint)pcOffset);
                var addr = mem.BaseAddress + (pcOffset + 4 + uPcRelative);
                return addr;
            }
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
