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
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    // http://www.vxdev.com/docs/vx55man/diab5.0ppc/x-elf_fo.htm

    public class PpcRelocator : ElfRelocator32
    {
        private ElfRelocation? prevPpcHi16;
        private Address? prevRelR;
        private readonly Dictionary<PpcRt, int> missedRelocations = new Dictionary<PpcRt, int>();

        public PpcRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .rela.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            base.Relocate(program, addrBase, pltEntries);
            return;
            /*
            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            if (rela_plt is null)
                return;
            var plt = loader.GetSectionInfoByName(".plt");
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            var pltRdr = loader.CreateReader(plt.FileOffset);
            for (int i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                if (!relaRdr.TryReadUInt32(out uint offset))
                    return;
                if (!relaRdr.TryReadUInt32(out uint info))
                    return;
                if (!relaRdr.TryReadInt32(out int addend))
                    return;

                // Read the .plt entry. We don't care about its contents,
                // only its address. Anyone accessing that address is
                // trying to access the symbol.

                if (!pltRdr.TryReadUInt32(out uint thunkAddress))
                    break;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(rela_plt.LinkedSection, sym);

                var addr = plt.Address + (uint)i * 4;
                //$TODO: why is this relocator not like the others? This code needs 
                // to be changed to us RelocateEntry like all other subclasses.
                program.ImportReferences.Add(
                    addr,
                    //$BUG: ExternalProcedure below should be using the symbol type. When
                    // changing to use RelocateEntry this will go away.
                    new NamedImportReference(addr, null, symStr, SymbolType.ExternalProcedure));
            }
            */
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = ctx.CreateAddress(ctx.P);
            var rt = (PpcRt)(rela.Info & 0xFF);
            switch (rt)
            {
            case PpcRt.R_PPC_GLOB_DAT:
            case PpcRt.R_PPC_COPY:
            case PpcRt.R_PPC_JMP_SLOT:
                return (addr, null);
            case PpcRt.R_PPC_ADDR32:
                ctx.WriteUInt32(addr, ctx.S + ctx.A);
                break;
            case PpcRt.R_PPC_REL24:
                if (!ctx.TryReadUInt32(addr, out uint wInstr))
                    return default;
                // 24 bit relocation where bits 3-29 are used for relocations
                uint value = (uint)(ctx.S + ctx.A - ctx.P);
                wInstr = (wInstr & 0xFC000003) | (value & 0x03FFFFFC);
                ctx.WriteUInt32(addr, wInstr);
                break;
            case PpcRt.R_PPC_ADDR16_HI:
            case PpcRt.R_PPC_ADDR16_HA:
                // Wait for the following R_PPC_ADDR16_LO relocation.
                prevPpcHi16 = rela;
                prevRelR = addr;
                break;
            case PpcRt.R_PPC_ADDR16_LO:
                if (prevPpcHi16 is null || prevRelR is null)
                    return (null, null);

                if (!ctx.TryReadUInt16(prevRelR.Value, out ushort valueHi))
                    return default;
                if (!ctx.TryReadUInt16(addr, out ushort valueLo))
                    return default;

                if ((PpcRt)(prevPpcHi16.Info & 0xFF) == PpcRt.R_PPC_ADDR16_HA)
                {
                    valueHi = (ushort)(valueHi + (valueHi & 0x8000u) != 0 ? 1u : 0u);
                }

                value = ((uint)valueHi << 16) | valueLo;
                value += (uint)ctx.S;

                valueHi = (ushort)(value >> 16);
                valueLo = (ushort) value;
                ctx.WriteUInt16(prevRelR.Value, valueHi);
                ctx.WriteUInt16(addr, valueLo);
                break;
            default:
                if (!missedRelocations.TryGetValue(rt, out var count))
                    count = 0;
                missedRelocations[rt] = count + 1;
                break;
            }
            return (addr, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((PpcRt)type).ToString();
        }
    }

    public class PpcRelocator64 : ElfRelocator64
    {
        public PpcRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
            this.loader = loader;
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = ctx.CreateAddress(ctx.P);
            var rt = (Ppc64Rt) (rela.Info & 0xFF);
            switch (rt)
            {
            case Ppc64Rt.R_PPC64_RELATIVE: // B + A
                ctx.WriteUInt64(addr, ctx.B + ctx.A);
                return (addr, null);
            case Ppc64Rt.R_PPC64_ADDR64:    // S + A
                ctx.WriteUInt64(addr, ctx.S + ctx.A);
                return (addr, null);
            case Ppc64Rt.R_PPC64_JMP_SLOT:
                return (addr, null);
            default:
                ctx.Warn(addr, $"Unimplemented PowerPC64 relocation type {rt}.");
                return (addr, null);
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Ppc64Rt)type).ToString();
        }
    }

    public enum PpcRt
    {
        R_PPC_NONE = 0,                 // none none
        R_PPC_ADDR32 = 1,               // word32 S + A
        R_PPC_ADDR24 = 2,               // low24* (S + A) >> 2
        R_PPC_ADDR16 = 3,               // half16* S + A
        R_PPC_ADDR16_LO = 4,            // half16 #lo(S + A)
        R_PPC_ADDR16_HI = 5,            // half16 #hi(S + A)
        R_PPC_ADDR16_HA = 6,            // half16 #ha(S + A)
        R_PPC_ADDR14 = 7,               // low14* (S + A) >> 2
        R_PPC_ADDR14_BRTAKEN = 8,       // low14* (S + A) >> 2
        R_PPC_ADDR14_BRNTAKEN = 9,      // low14* (S + A) >> 2
        R_PPC_REL24 = 10,               // low24* (S + A - P) >> 2
        R_PPC_REL14 = 11,               // low14* (S + A - P) >> 2
        R_PPC_REL14_BRTAKEN = 12,       // low14* (S + A - P) >> 2
        R_PPC_REL14_BRNTAKEN = 13,      // low14* (S + A - P) >> 2
        R_PPC_GOT16 = 14,               // half16* G + A
        R_PPC_GOT16_LO = 15,            // half16 #lo(G + A)
        R_PPC_GOT16_HI = 16,            // half16 #hi(G + A)
        R_PPC_GOT16_HA = 17,            // half16 #ha(G + A)
        R_PPC_PLTREL24 = 18,            // low24* (L + A - P) >> 2
        R_PPC_COPY = 19,                // none none
        R_PPC_GLOB_DAT = 20,            // word32 S + A
        R_PPC_JMP_SLOT = 21,            // none see below
        R_PPC_RELATIVE = 22,            // word32 B + A
        R_PPC_LOCAL24PC = 23,           // low24* see below
        R_PPC_UADDR32 = 24,             // word32 S + A
        R_PPC_UADDR16 = 25,             // half16* S + A
        R_PPC_REL32 = 26,               // word32 S + A - P
        R_PPC_PLT32 = 27,               // word32 L + A


        //An instruction with the lower 16 bits being the offset into a 64KB big Small Data Area (SDA). There are three SDAs:

        //The absolute SDA (address 0) pointed to by r0
        //The constant SDA2 (typically the .sdata2 section) pointed to by r2
        //The normal SDA (typically the .sdata and .sbss sections) pointed to by r13
        //Depending on which of these three SDA's the identifier is defined in, the linker will patch the instruction to use the correct register and offset:

        //lwz    r3, var@sdarx(r0)  

        R_PPC_EMB_SDA21 = 109,
    }

    // https://refspecs.linuxfoundation.org/ELF/ppc64/PPC-elf64abi.html#RELOC-TYPE
    public enum Ppc64Rt
    {
        R_PPC64_NONE               = 0,     // none          none
        R_PPC64_ADDR32             = 1,     // word32*       S + A
        R_PPC64_ADDR24             = 2,     // low24*        (S + A) >> 2
        R_PPC64_ADDR16             = 3,     // half16*       S + A
        R_PPC64_ADDR16_LO          = 4,     // half16        #lo(S + A)
        R_PPC64_ADDR16_HI          = 5,     // half16        #hi(S + A)
        R_PPC64_ADDR16_HA          = 6,     // half16        #ha(S + A)
        R_PPC64_ADDR14             = 7,     // low14*        (S + A) >> 2
        R_PPC64_ADDR14_BRTAKEN     = 8,     // low14*        (S + A) >> 2
        R_PPC64_ADDR14_BRNTAKEN    = 9,     // low14*        (S + A) >> 2
        R_PPC64_REL24              = 10,     // low24*        (S + A - P) >> 2
        R_PPC64_REL14              = 11,     // low14*        (S + A - P) >> 2
        R_PPC64_REL14_BRTAKEN      = 12,     // low14*        (S + A - P) >> 2
        R_PPC64_REL14_BRNTAKEN     = 13,     // low14*        (S + A - P) >> 2
        R_PPC64_GOT16              = 14,     // 	 half16*       G
        R_PPC64_GOT16_LO           = 15,     // 	 half16        #lo(G)
        R_PPC64_GOT16_HI           = 16,     // 	 half16        #hi(G)
        R_PPC64_GOT16_HA           = 17,     // 	 half16        #ha(G)
        R_PPC64_COPY               = 19,     // none          none
        R_PPC64_GLOB_DAT           = 20,     // doubleword64  S + A
        R_PPC64_JMP_SLOT           = 21,     // none          see below
        R_PPC64_RELATIVE           = 22,     // doubleword64  B + A
        R_PPC64_UADDR32            = 24,     // word32*       S + A
        R_PPC64_UADDR16            = 25,     // half16*       S + A
        R_PPC64_REL32              = 26,     // word32*       S + A - P
        R_PPC64_PLT32              = 27,     // 	 word32*       L
        R_PPC64_PLTREL32           = 28,     // 	 word32*       L - P
        R_PPC64_PLT16_LO           = 29,     // 	 half16        #lo(L)
        R_PPC64_PLT16_HI           = 30,     // 	 half16        #hi(L)
        R_PPC64_PLT16_HA           = 31,     // 	 half16        #ha(L)
        R_PPC64_SECTOFF            = 33,     // half16*       R + A
        R_PPC64_SECTOFF_LO         = 34,     // half16        #lo(R + A)
        R_PPC64_SECTOFF_HI         = 35,     // half16        #hi(R + A)
        R_PPC64_SECTOFF_HA         = 36,     // half16        #ha(R + A)
        R_PPC64_ADDR30             = 37,     // word30        (S + A - P) >> 2
        R_PPC64_ADDR64             = 38,     // doubleword64  S + A
        R_PPC64_ADDR16_HIGHER      = 39,     // half16        #higher(S + A)
        R_PPC64_ADDR16_HIGHERA     = 40,     // half16        #highera(S + A)
        R_PPC64_ADDR16_HIGHEST     = 41,     // half16        #highest(S + A)
        R_PPC64_ADDR16_HIGHESTA    = 42,     // half16        #highesta(S + A)
        R_PPC64_UADDR64            = 43,     // doubleword64  S + A
        R_PPC64_REL64              = 44,     // doubleword64  S + A - P
        R_PPC64_PLT64              = 45,     // 	 doubleword64  L
        R_PPC64_PLTREL64           = 46,     // 	 doubleword64  L - P
        R_PPC64_TOC16              = 47,     // half16*       S + A - .TOC.
        R_PPC64_TOC16_LO           = 48,     // half16        #lo(S + A - .TOC.)
        R_PPC64_TOC16_HI           = 49,     // half16        #hi(S + A - .TOC.)
        R_PPC64_TOC16_HA           = 50,     // half16        #ha(S + A - .TOC.)
        R_PPC64_TOC                = 51,     // doubleword64  .TOC.
        R_PPC64_PLTGOT16           = 52,     // 	 half16*       M
        R_PPC64_PLTGOT16_LO        = 53,     // 	 half16        #lo(M)
        R_PPC64_PLTGOT16_HI        = 54,     // 	 half16        #hi(M)
        R_PPC64_PLTGOT16_HA        = 55,     // 	 half16        #ha(M)
        R_PPC64_ADDR16_DS          = 56,     // half16ds*     (S + A) >> 2
        R_PPC64_ADDR16_LO_DS       = 57,     // half16ds      #lo(S + A) >> 2
        R_PPC64_GOT16_DS           = 58,     // 	 half16ds*     G >> 2
        R_PPC64_GOT16_LO_DS        = 59,     // 	 half16ds      #lo(G) >> 2
        R_PPC64_PLT16_LO_DS        = 60,     // 	 half16ds      #lo(L) >> 2
        R_PPC64_SECTOFF_DS         = 61,     // half16ds*     (R + A) >> 2
        R_PPC64_SECTOFF_LO_DS      = 62,     // half16ds      #lo(R + A) >> 2
        R_PPC64_TOC16_DS           = 63,     // half16ds*     (S + A - .TOC.) >> 2
        R_PPC64_TOC16_LO_DS        = 64,     // half16ds      #lo(S + A - .TOC.) >> 2
        R_PPC64_PLTGOT16_DS        = 65,     // 	 half16ds*     M >> 2
        R_PPC64_PLTGOT16_LO_DS     = 66,     // 	 half16ds      #lo(M) >> 2
        R_PPC64_TLS                = 67,     // 	 none          none
        R_PPC64_DTPMOD64           = 68,     // 	 doubleword64  @dtpmod
        R_PPC64_TPREL16            = 69,     // 	 half16*       @tprel
        R_PPC64_TPREL16_LO         = 60,     // 	 half16        #lo(@tprel)
        R_PPC64_TPREL16_HI         = 71,     // 	 half16        #hi(@tprel)
        R_PPC64_TPREL16_HA         = 72,     // 	 half16        #ha(@tprel)
        R_PPC64_TPREL64            = 73,     // 	 doubleword64  @tprel
        R_PPC64_DTPREL16           = 74,     // 	 half16*       @dtprel
        R_PPC64_DTPREL16_LO        = 75,     // 	 half16        #lo(@dtprel)
        R_PPC64_DTPREL16_HI        = 76,     // 	 half16        #hi(@dtprel)
        R_PPC64_DTPREL16_HA        = 77,     // 	 half16        #ha(@dtprel)
        R_PPC64_DTPREL64           = 78,     // 	 doubleword64  @dtprel
        R_PPC64_GOT_TLSGD16        = 79,     // 	 half16*       @got@tlsgd
        R_PPC64_GOT_TLSGD16_LO     = 80,     // 	 half16        #lo(@got@tlsgd)
        R_PPC64_GOT_TLSGD16_HI     = 81,     // 	 half16        #hi(@got@tlsgd)
        R_PPC64_GOT_TLSGD16_HA     = 82,     // 	 half16        #ha(@got@tlsgd)
        R_PPC64_GOT_TLSLD16        = 83,     // 	 half16*       @got@tlsld
        R_PPC64_GOT_TLSLD16_LO     = 84,     // 	 half16        #lo(@got@tlsld)
        R_PPC64_GOT_TLSLD16_HI     = 85,     // 	 half16        #hi(@got@tlsld)
        R_PPC64_GOT_TLSLD16_HA     = 86,     // 	 half16        #ha(@got@tlsld)
        R_PPC64_GOT_TPREL16_DS     = 87,     // 	 half16ds*     @got@tprel
        R_PPC64_GOT_TPREL16_LO_DS  = 88,     // 	 half16ds      #lo(@got@tprel)
        R_PPC64_GOT_TPREL16_HI     = 89,     // 	 half16        #hi(@got@tprel)
        R_PPC64_GOT_TPREL16_HA     = 90,     // 	 half16        #ha(@got@tprel)
        R_PPC64_GOT_DTPREL16_DS    = 91,     // 	 half16ds*     @got@dtprel
        R_PPC64_GOT_DTPREL16_LO_DS = 92,     // half16ds,     // #lo(@got@dtprel)
        R_PPC64_GOT_DTPREL16_HI    = 93,     // 	 half16        #hi(@got@dtprel)
        R_PPC64_GOT_DTPREL16_HA    = 94,     // 	 half16        #ha(@got@dtprel)
        R_PPC64_TPREL16_DS         = 95,     // 	 half16ds*     @tprel
        R_PPC64_TPREL16_LO_DS      = 96,     // 	 half16ds      #lo(@tprel)
        R_PPC64_TPREL16_HIGHER     = 97,     // 	 half16        #higher(@tprel)
        R_PPC64_TPREL16_HIGHERA    = 98,     // 	 half16        #highera(@tprel)
        R_PPC64_TPREL16_HIGHEST    = 99,     // 	 half16        #highest(@tprel)
        R_PPC64_TPREL16_HIGHESTA   = 100,     // half16        #highesta(@tprel)
        R_PPC64_DTPREL16_DS        = 101,     // half16ds*     @dtprel
        R_PPC64_DTPREL16_LO_DS     = 102,     // half16ds      #lo(@dtprel)
        R_PPC64_DTPREL16_HIGHER    = 103,     // half16        #higher(@dtprel)
        R_PPC64_DTPREL16_HIGHERA   = 104,     // half16        #highera(@dtprel)
        R_PPC64_DTPREL16_HIGHEST   = 105,     // half16        #highest(@dtprel)
        R_PPC64_DTPREL16_HIGHESTA  = 106,     // half16        #highesta(@dtprel)
    }
}
