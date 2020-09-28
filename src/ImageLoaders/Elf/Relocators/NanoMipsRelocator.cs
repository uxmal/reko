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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class NanoMipsRelocator : ElfRelocator32
    {
        public NanoMipsRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rel)
        {
            if (symbol == null)
                return symbol;
            if (loader.Sections.Count <= symbol.SectionIndex)
                return symbol;
            if (symbol.SectionIndex == 0)
                return symbol;
            var symSection = loader.Sections[(int) symbol.SectionIndex];

            Address addr;
            uint P;
            if (referringSection?.Address != null)
            {
                addr = referringSection.Address + rel.Offset;
                P = (uint) addr.ToLinear();
            }
            else
            {
                addr = Address.Ptr64(rel.Offset);
                P = 0;
            }
            //var S = symbol.Value;
            //uint PP = P;
            //var relR = program.CreateImageReader(program.Architecture, addr);
            //var relW = program.CreateImageWriter(program.Architecture, addr);
            //int sh = 0;
            //uint mask = 0;
            //uint A = 0;
            //var rt = (MIPSrt) (rel.Info & 0xFF);
            //switch (rt)
            //{

            //}
            return symbol;
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((NanoRt) type).ToString();
        }

        public enum NanoRt
        {
            R_NANOMIPS_NONE = 0,
            R_NANOMIPS_32 = 1,
            R_NANOMIPS_64 = 2,
            R_NANOMIPS_NEG = 3,
            R_NANOMIPS_ASHIFTR_1 = 4,
            R_NANOMIPS_UNSIGNED_8 = 5,
            R_NANOMIPS_SIGNED_8 = 6,
            R_NANOMIPS_UNSIGNED_16 = 7,
            R_NANOMIPS_SIGNED_16 = 8,
            R_NANOMIPS_RELATIVE = 9,
            R_NANOMIPS_GLOBAL = 10,
            R_NANOMIPS_JUMP_SLOT = 11,
            R_NANOMIPS_IRELATIVE = 12,

            R_NANOMIPS_PC25_S1 = 13,
            R_NANOMIPS_PC21_S1 = 14,
            R_NANOMIPS_PC14_S1 = 15,
            R_NANOMIPS_PC11_S1 = 16,
            R_NANOMIPS_PC10_S1 = 17,
            R_NANOMIPS_PC7_S1 = 18,
            R_NANOMIPS_PC4_S1 = 19,

            R_NANOMIPS_GPREL19_S2 = 20,
            R_NANOMIPS_GPREL18_S3 = 21,
            R_NANOMIPS_GPREL18 = 22,
            R_NANOMIPS_GPREL17_S1 = 23,
            R_NANOMIPS_GPREL16_S2 = 24,
            R_NANOMIPS_GPREL7_S2 = 25,
            R_NANOMIPS_GPREL_HI20 = 26,
            R_NANOMIPS_PCHI20 = 27,

            R_NANOMIPS_HI20 = 28,
            R_NANOMIPS_LO12 = 29,
            R_NANOMIPS_GPREL_I32 = 30,
            R_NANOMIPS_PC_I32 = 31,
            R_NANOMIPS_I32 = 32,
            R_NANOMIPS_GOT_DISP = 33,
            R_NANOMIPS_GOTPC_I32 = 34,
            R_NANOMIPS_GOTPC_HI20 = 35,
            R_NANOMIPS_GOT_LO12 = 36,
            R_NANOMIPS_GOT_CALL = 37,
            R_NANOMIPS_GOT_PAGE = 38,
            R_NANOMIPS_GOT_OFST = 39,
            R_NANOMIPS_LO4_S2 = 40,
            /* Reserved for 64-bit ABI. */
            R_NANOMIPS_RESERVED1 = 41,
            R_NANOMIPS_GPREL_LO12 = 42,
            R_NANOMIPS_SCN_DISP = 43,
            R_NANOMIPS_COPY = 44,

            R_NANOMIPS_ALIGN = 64,
            R_NANOMIPS_FILL = 65,
            R_NANOMIPS_MAX = 66,
            R_NANOMIPS_INSN32 = 67,
            R_NANOMIPS_FIXED = 68,
            R_NANOMIPS_NORELAX = 69,
            R_NANOMIPS_RELAX = 70,
            R_NANOMIPS_SAVERESTORE = 71,
            R_NANOMIPS_INSN16 = 72,
            R_NANOMIPS_JALR32 = 73,
            R_NANOMIPS_JALR16 = 74,

            /* TLS relocations.  */
            R_NANOMIPS_TLS_DTPMOD = 80,
            R_NANOMIPS_TLS_DTPREL = 81,
            R_NANOMIPS_TLS_TPREL = 82,
            R_NANOMIPS_TLS_GD = 83,
            R_NANOMIPS_TLS_GD_I32 = 84,
            R_NANOMIPS_TLS_LD = 85,
            R_NANOMIPS_TLS_LD_I32 = 86,
            R_NANOMIPS_TLS_DTPREL12 = 87,
            R_NANOMIPS_TLS_DTPREL16 = 88,
            R_NANOMIPS_TLS_DTPREL_I32 = 89,
            R_NANOMIPS_TLS_GOTTPREL = 90,
            R_NANOMIPS_TLS_GOTTPREL_PC_I32 = 91,
            R_NANOMIPS_TLS_TPREL12 = 92,
            R_NANOMIPS_TLS_TPREL16 = 93,
            R_NANOMIPS_TLS_TPREL_I32 = 94,
        }
    }
}
