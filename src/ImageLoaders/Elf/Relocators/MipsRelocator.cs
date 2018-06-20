﻿#region License
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

using System;
using System.Collections.Generic;
using Reko.Core;
using System.Linq;

namespace Reko.ImageLoaders.Elf.Relocators
{
    // https://gcc.gnu.org/ml/gcc/2008-07/txt00000.txt - MIPS non-PIC ABI specification
    public class MipsRelocator : ElfRelocator32
    {
        public MipsRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void Relocate(Program program)
        {
            base.Relocate(program);

            var dynsect = loader.GetSectionInfoByName(".dynamic");
            if (dynsect == null)
                return;

            // $TODO: Fix the relocation code (it's crashing right now)
            /*
            var dynentries = loader.GetDynEntries(dynsect.FileOffset).ToDictionary(k => k.d_tag);
            var symtab = dynentries[DynamicSectionRenderer.DT_SYMTAB];
            var pltgot = dynentries[DynamicSectionRenderer.DT_PLTGOT].d_val;
            var mips_local_gotno = dynentries[DynamicSectionRenderer.Mips.DT_MIPS_LOCAL_GOTNO].d_val;
            var mips_gotsym = dynentries[DynamicSectionRenderer.Mips.DT_MIPS_GOTSYM].d_val;
            var dyngot = pltgot + (mips_local_gotno - mips_gotsym) * 4;
            //loader.CreateReader()
            for (int i = mips_gotsym; i < loader.Symbols.Count; ++i)
            {
            }
            */

            // https://www.cr0.org/paper/mips.elf.external.resolution.txt
            /*Linux MIPS ELF reverse engineering tips
            ---------------------------------------

            Julien TINNES <julien at cr0.org>

            You may have been surprised that while reverse engineering MIPS ELF executables with IDA, you don't get any XREF for local procedure calls, nor for external procedure calls.
            This is due to the way the functions are called, using a classic GOT/PLT mecanism.

            The ABI states that every function must be called through jalr $t9. This means that at the begining of a given function, $t9 holds the current virtual address. The very first instructions of the function will be something like:

            lui	$gp, 0xFC0
            addiu	$gp, 0x76E0
            addu	$gp, t9

            which IDA will simplify as:

            li	$gp, 0xFC076E0
            addu	$gp, $t9

            the value 0xFC076E0 is calculated by the compiler in such a way that $gp will hold a "global pointer" value which is constant in the program.
            This value is always OFFSET_GP_GOT bytes after the program's GOT (global offset table), OFFSET_GP_GOT is always 0x7ff0.

            So, if you need to calculate $gp's value, you can add OFFSET_GP_GOT to the address of the GOT (which you can retrieve through the ELF dynamic table, under the entry: MIPS_RLD_VERSION). Or you can look at the begining of the function and add the function's virtual address and the magic value (see li).

            Here on my executable, GOT is at 0x10000030, so general $gp's value is 0x10008020

            Now if we need to call an internal function (in the same ELF file), we'll do something like:
            lw	$t9, -0x7Fb4($gp)
            nop
            jalr $t9

            in my GOT, at address 0x1000006c I have a pointer to my local function.

            Now how does it work if we call an external function ? Well it's pretty similar to the way it works on Intel with PLT/GOT. The pointer in the GOT will point to a 4 instructions stub in .text section which is:

            lw	$t9, -0x7FF0($gp)
            move	$t7, $ra
            jalr	$t9
            li	$t8, SYM_INDEX

            where SYM_INDEX depends on the external function you want to call. What it does is load the first entry of the GOT (always) into $t9, save the return address in $t7, load an index in $t8 (remember the delay slot ;), and call $t9 which now points to dl_linux_resolve (in ld.so) (also called _dl_runtime_resolve). This first entry in the GOT is initialized at runtime by the dynamic loader.
            dl_linux_resolve will call __dl_runtime_resolve with $t8 and (the program's) $gp as arguments. dl_runtime_resolve will patch the corresonding (to $t8, which is an index) GOT entry, then it'll put $t7 (the saved return address, remember) in $ra and jump to the now resolved function (returned by __dl_runtime_resolve in $v0).

            What is really interesting is to know how dl_runtime_resolve works, this would allow us to write an IDA plugin:

            Here's an algorithm you can use:

            Search DT_SYMTAB in the dynamic section (you can try with readelf -d)

            Read this symbol table (readelf -s)

            Read DT_PLTGOT, DT_MIPS_LOCAL_GOTNO et DT_MIPS_GOTSYM in dynamic section.

            now, dyngot=DT_PLTGOT + (DT_MIPS_LOCAL_GOTNO - DT_MIPS_GOTSYM)*(POINTER_SIZE (4 on 32 bits))

            Now in the dynamic symbol table (found with DT_SYMTAB) (his name is probably
            .dynsym anyway), from index DT_MIPS_GOTSYM, the names are corresponding to the
            functions' PLT stubs pointed to by dyngot[index-DT_MIPS_GOTSYM] entries. Make
            each dyngot entry an offset and change his name to the name found in the
            dynamic symbol table and you're done.
            */

        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rel)
        {
            if (loader.Sections.Count <= symbol.SectionIndex)
                return;
            if (symbol.SectionIndex == 0)
                return;
            var symSection = loader.Sections[(int)symbol.SectionIndex];

            var addr = referringSection.Address + rel.Offset;
            var S = symbol.Value;
            uint P = (uint)addr.ToLinear();
            uint PP = P;
            var relR = program.CreateImageReader(addr);
            var relW = program.CreateImageWriter(addr);
            int sh = 0;
            uint mask = 0;
            uint A = 0;

            switch ((MIPSrt)(rel.Info & 0xFF))
            {
            case MIPSrt.R_MIPS_NONE: return;
            case MIPSrt.R_MIPS_REL32:
                break;
                /*
                R_MIPS_NONE      0  none     local    none
                R_MIPS_16        1  V–half16 external S + sign–extend(A)
                                 1  V–half16 local    S + sign–extend(A)
                R_MIPS_32        2  T–word32 external S + A
                                 2  T–word32 local    S + A
                R_MIPS_REL32     3  T–word32 external A – EA + S
                R_MIPS_REL32     3  T–word32 local    A – EA + S
                R_MIPS_26        4  T–targ26 local    (((A << 2) | \
                                                      (P & 0xf0000000) + S) >> 2
                                 4  T–targ26 external (sign–extend(A < 2) + S) >> 2
                R_MIPS_HI16      5  T–hi16   external ((AHL + S) – \
                                                        (short)(AHL + S)) >> 16
                                 5  T–hi16   local    ((AHL + S) – \
                                                        (short)(AHL + S)) >> 16
                                 5  V–hi16   _gp_disp (AHL + GP – P) – (short) \
                                                        (AHL + GP – P)) >> 16
                R_MIPS_LO16      6  T–lo16   external AHL + S
                                 6  T–lo16   local    AHL + S
                                 6  V–lo16   _gp_disp AHL + GP – P + 4
                R_MIPS_GPREL16   7  V–rel16  external sign–extend(A) + S + GP
                                 7  V–rel16  local    sign–extend(A) + S + GP0 – GP
                R_MIPS_LITERAL   8  V–lit16  local    sign–extend(A) + L
                R_MIPS_GOT16     9  V–rel16  external G
                                 9  V–rel16  local    see below
                R_MIPS_PC16      10 V–pc16   external sign–extend(A) + S – P
                R_MIPS_CALL16    11 V–rel16  external G
                R_MIPS_GPREL32   12 T–word32 local    A + S + GP0 – GP
                R_MIPS_GOTHI16   21 T-hi16   external (G - (short)G) >> 16 + A
                R_MIPS_GOTLO16   22 T-lo16   external G & 0xffff
                R_MIPS_CALLHI16  30 T-hi16   external (G - (short)G) >> 16 + A
                R_MIPS_CALLLO16  31 T-lo16   external G & 0xffff */
            }
            var w = relR.ReadUInt32();
            w += ((uint)(S + A + P) >> sh) & mask;
            relW.WriteUInt32(w);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((MIPSrt)type).ToString();
        }
    }

    public enum MIPSrt
    {
        R_MIPS_NONE = 0,
        R_MIPS_16 = 1,
        R_MIPS_32 = 2,
        R_MIPS_REL32 = 3,
        R_MIPS_26 = 4,
        R_MIPS_HI16 = 5,
        R_MIPS_LO16 = 6,
        R_MIPS_GPREL16 = 7,
        R_MIPS_LITERAL = 8,
        R_MIPS_GOT16 = 9,
        R_MIPS_PC16 = 10,
        R_MIPS_CALL16 = 11,
        R_MIPS_GPREL32 = 12,
        /* The remaining relocs are defined on Irix, although they are not
           in the MIPS ELF ABI.	 */
        R_MIPS_UNUSED1 = 13,
        R_MIPS_UNUSED2 = 14,
        R_MIPS_UNUSED3 = 15,
        R_MIPS_SHIFT5 = 16,
        R_MIPS_SHIFT6 = 17,
        R_MIPS_64 = 18,
        R_MIPS_GOT_DISP = 19,
        R_MIPS_GOT_PAGE = 20,
        R_MIPS_GOT_OFST = 21,
        /*
         * The following two relocation types are specified in the MIPS ABI
         * conformance guide version 1.2 but not yet in the psABI.
         */
        R_MIPS_GOTHI16 = 22,
        R_MIPS_GOTLO16 = 23,
        R_MIPS_SUB = 24,
        R_MIPS_INSERT_A = 25,
        R_MIPS_INSERT_B = 26,
        R_MIPS_DELETE = 27,
        R_MIPS_HIGHER = 28,
        R_MIPS_HIGHEST = 29,
        /*
         * The following two relocation types are specified in the MIPS ABI
         * conformance guide version 1.2 but not yet in the psABI.
         */
        R_MIPS_CALLHI16 = 30,
        R_MIPS_CALLLO16 = 31,
        /*
         * Introduced for MIPSr6.
         */
        R_MIPS_PC21_S2 = 60,
        R_MIPS_PC26_S2 = 61,
    }

    public class MipsRelocator64 : ElfRelocator64
    {
        private ElfLoader64 elfLoader;

        public MipsRelocator64(ElfLoader64 elfLoader, SortedList<Address, ImageSymbol> imageSymbols) : base(elfLoader, imageSymbols)
        {
            this.elfLoader = elfLoader;
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela)
        {
            throw new NotImplementedException();
        }

        public override string RelocationTypeToString(uint type)
        {
            throw new NotImplementedException();
        }
    }
}