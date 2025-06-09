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
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf.Relocators
{
    // https://gcc.gnu.org/ml/gcc/2008-07/txt00000.txt - MIPS non-PIC ABI 
    // https://dmz-portal.mips.com/wiki/MIPS_Multi_GOT - Understanding the layout of the MIPS got table
    public class MipsRelocator : ElfRelocator32
    {
        const int PointerByteSize = 4;
        const int OFFSET_GP_GOT = 0x7FF0;

        private readonly IProcessorArchitecture arch;
        private IProcessorArchitecture? archMips16e;
        private Address? addrHi;
        private Address? addrHiPrev;

        public MipsRelocator(ElfLoader32 loader, IProcessorArchitecture arch, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
            this.arch = arch;
            archMips16e = null;
        }

        public override ImageSymbol AdjustImageSymbol(ImageSymbol sym)
        {
            if (sym.Type != SymbolType.Code &&
                sym.Type != SymbolType.ExternalProcedure &&
                sym.Type != SymbolType.Procedure)
                return sym;
            if ((sym.Address!.ToLinear() & 1) == 0)
                return sym;
            if (archMips16e is null)
            {
                var cfgSvc = loader.Services.RequireService<IConfigurationService>();
                this.archMips16e = cfgSvc.GetArchitecture(
                    arch.Name, 
                    new Dictionary<string, object>
                    {
                        { "decoder", "mips16e" }
                    })!;
            }
            var addrNew = sym.Address - 1;
            var symNew = ImageSymbol.Create(
                sym.Type,
                archMips16e,
                addrNew,
                sym.Name,
                sym.DataType,
                !sym.NoDecompile);
            symNew.ProcessorState = sym.ProcessorState;
            return symNew;
        }

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            // MIPS relocation is really confusing, and doesn't follow
            // the pattern of most other architectures. The code below follows
            // the specification in "SYSTEM V APPLICATION BINARY INTERFACE, MIPS
            // RISC Processor Supplement, 3rd Edition", figure 5-9.

            foreach (var dynSeg in EnumerateDynamicSegments())
            {
                DumpDynamicSegment(dynSeg);

                var dynent = Loader.BinaryImage.DynamicEntries;
                if (!dynent.TryGetValue(ElfDynamicEntry.Mips.DT_MIPS_BASE_ADDRESS, out var baseAddr) ||
                    !dynent.TryGetValue(ElfDynamicEntry.DT_SYMTAB, out var dynsymtab) ||
                    !dynent.TryGetValue(ElfDynamicEntry.DT_STRTAB, out var strtab) ||
                    !dynent.TryGetValue(ElfDynamicEntry.DT_SYMENT, out var syment) ||
                    !dynent.TryGetValue(ElfDynamicEntry.Mips.DT_MIPS_LOCAL_GOTNO, out var got_local_num) ||
                    !dynent.TryGetValue(ElfDynamicEntry.Mips.DT_MIPS_GOTSYM, out var symtab_got_idx) ||
                    !dynent.TryGetValue(ElfDynamicEntry.Mips.DT_MIPS_SYMTABNO, out var symbol_count) ||
                    !dynent.TryGetValue(ElfDynamicEntry.DT_PLTGOT, out var gotaddr))
                {
                    var listener = loader.Services.RequireService<IEventListener>();
                    listener.Warn("Required MIPS .dynamic information is missing from this binary. This will degrade decompilation output.");
                    continue;
                }
                program.GlobalRegisterValue = Constant.Word32((uint)gotaddr.UValue + OFFSET_GP_GOT);
                ElfImageLoader.trace.Inform("Global register value: {0:X}", program.GlobalRegisterValue);
                var wordsize = (uint) program.Architecture.WordWidth.Size;

                // "Local entries reside in the first part of the global offset table. The value of
                // the dynamic tag DT_MIPS_LOCAL_GOTNO holds the number of local global offset
                // table entries.These entries only require relocation if they occur in a shared object
                // and the shared object memory load address differs from the virtual address of the
                // loadable segments of the shared object. As with defined external entries in the
                // //global offset table, these local entries contain actual addresses".

                for (uint i = 2; i < got_local_num.UValue; ++i)
                {
                    //$TODO: not sure how to deal with these entries yet; they don't appear
                    // to have symbols associated with them
                    // gotaddr.UValue + i * wordsize;
                }

                // "External entries reside in the second part of the global offset table.
                // Each entry in the external section corresponds to an entry in the global
                // offset table mapped part of the .dynsym section The first symbol in the
                // .dynsym section corresponds to the first word of the global offset table;
                // the second symbol corresponds to the second word, and so on. Each word
                // in the external entry part of the global offset table contains the
                // actual address for its corresponding symbol."

                var nGlobalSyms = symbol_count.UValue - symtab_got_idx.UValue;
                for (uint i = 0; i < nGlobalSyms; ++i)
                {
                    var offStrtab = Loader.AddressToFileOffset(strtab.UValue);
                    var offSymtab = Loader.AddressToFileOffset(dynsymtab.UValue);
                    var symbol = this.loader.EnsureSymbol(offSymtab, (int)(i + symtab_got_idx.UValue), syment.UValue, offStrtab);
                    ElfImageLoader.trace.Verbose("Mips Dynsym: {0:X8} - {1} {2:X4} {3}", symbol!.Value, symbol!.SectionIndex, symbol.Size, symbol!.Name);
                    var addrGotSlot = loader.CreateAddress(gotaddr.UValue + (i + got_local_num.UValue) * wordsize);
                    if (symbol.Value != 0)
                    {
                        base.GenerateImageSymbol(program, addrGotSlot, symbol, null);
                    }
                }
            }
            base.Relocate(program, addrBase, pltEntries);
        }

        #region Long tirade about global pointer in MIPS ELF

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
        #endregion

        //$TODO: this will likely not work at all for MIPS64 
        public override void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            LocateLocalGotEntries(program, symbols);
            LocateGlobalGotEntries(program, symbols);
        }

        /// <summary>
        /// Generate GOT entries for local symbols of the ELF binary.
        /// </summary>
        /// <remarks>
        /// The first DT_MIPS_LOCAL_GOTNO pointers in the GOT have their pointer values
        /// already filled in, so we do a reverse lookup to find their corresponding symbols.
        /// </remarks>
        private void LocateLocalGotEntries(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            var dynamic = loader.BinaryImage.DynamicEntries;
            if (!dynamic.TryGetValue(ElfDynamicEntry.Mips.DT_MIPS_LOCAL_GOTNO, out var dynEntry))
                return;
            var numberoflocalPointers = (int) dynEntry.SValue;
            var uAddrBeginningOfGot = (uint)dynamic[ElfDynamicEntry.DT_PLTGOT].UValue;

            var addrGot = Address.Ptr32(uAddrBeginningOfGot);
            var addrLocalGotEnd = addrGot + numberoflocalPointers * PointerByteSize;
            loader.ConstructGotEntries(program, symbols, addrGot, addrLocalGotEnd, true);
        }

        /// <summary>
        /// Generate GOT entries for the global (imported) symbols of the MIPS ELF binary
        /// </summary>
        /// <remarks>
        /// The Elf MIPS ABI specifies that the GOT entries for import symbols are located after
        /// the GOT entries for the local functions. In addition they are orderd in the same order
        /// as the corresponding symbols in the symbol table.
        /// </remarks>
        private void LocateGlobalGotEntries(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            var dynamic = loader.BinaryImage.DynamicEntries;
            if (!dynamic.TryGetValue(ElfDynamicEntry.DT_SYMTAB, out var dynEntry))
                return;
            var uAddrSymtab = (uint) dynEntry.UValue;
            var allSymbols = loader.BinaryImage.DynamicSymbols;

            var cLocalSymbols = (int)dynamic[ElfDynamicEntry.Mips.DT_MIPS_GOTSYM].SValue;
            var cTotalSymbols = (int)dynamic[ElfDynamicEntry.Mips.DT_MIPS_SYMTABNO].SValue;
            var cGlobalSymbols = cTotalSymbols - cLocalSymbols;

            var numberoflocalPointers = (int)dynamic[ElfDynamicEntry.Mips.DT_MIPS_LOCAL_GOTNO].SValue;
            var uAddrBeginningOfGot = (uint)dynamic[ElfDynamicEntry.DT_PLTGOT].UValue;
            var uAddrBeginningOfGlobalPointers = uAddrBeginningOfGot + (uint)numberoflocalPointers * PointerByteSize;

            for (int i = 0; i < cGlobalSymbols; ++i)
            {
                var addrGot = Address.Ptr32(uAddrBeginningOfGlobalPointers + PointerByteSize * (uint)i);
                var iSymbol = cLocalSymbols + i;
                if (allSymbols.TryGetValue(iSymbol, out var isymbol) &&
                    isymbol is ElfSymbol symbol &&
                    symbol.Type == ElfSymbolType.STT_FUNC)
                {
                    // This GOT entry is a known symbol!
                    ImageSymbol symGotEntry = loader.CreateGotSymbol(arch, addrGot, symbol.Name);
                    symbols[addrGot] = symGotEntry;
                    Debug.Print("Found GOT entry at {0}, changing symbol at {1}", symGotEntry, addrGot);
                    var st = ElfLoader.GetSymbolType(symbol);
                    if (st.HasValue)
                    {
                        program.ImportReferences[addrGot] = new NamedImportReference(addrGot, null, symbol.Name, st.Value);
                    }
                }
            }
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = Address.Ptr32((uint) ctx.P);
            var rt = (MIPSrt) (rela.Info & 0xFF);
            switch (rt)
            {
            case MIPSrt.R_MIPS_NONE:
            case MIPSrt.R_MIPS_REL32:
                return (addr, null);
            case MIPSrt.R_MIPS_JUMP_SLOT:
                // Non-PIC binary will have these.
                if (!ctx.TryReadUInt32(addr, out var uAddrInSlot))
                    return default;
                if (symbol.Value == 0 && ctx.IsExecutableAddress(ctx.CreateAddress(uAddrInSlot)))
                {
                    var newSym = CreatePltStubSymbolFromRelocation(symbol, uAddrInSlot, 0);
                    return (addr, newSym);
                }
                return (addr, null);
            case MIPSrt.R_MIPS_32:
                ctx.WriteUInt32(addr, ctx.S + ctx.A);
                return (addr, null);
            case MIPSrt.R_MIPS_HI16:
                // Wait for the following R_MIPS_LO16 relocation.
                addrHi = addr;
                addrHiPrev = null;
                return (addr, null);
            case MIPSrt.R_MIPS_LO16:
                if (addrHi is { })
                {
                    // This LO16 relocation had a HI16 just before.
                    if (!ctx.TryReadUInt32(addrHi.Value, out uint valueHi) ||
                        !ctx.TryReadUInt32(addr, out uint valueLo))
                        return default;

                    uint ahl = (valueHi << 16) | (valueLo & 0xFFFF);
                    ahl += (uint)ctx.S;

                    valueHi = (valueHi & 0xFFFF0000u) | (ahl >> 16);
                    valueLo = (valueLo & 0xFFFF0000u) | (ahl & 0xFFFF);
                    ctx.WriteUInt32(addrHi.Value, valueHi);
                    ctx.WriteUInt32(addr, valueLo);

                    // If there is another LO16 without a HI16 in-between use stash
                    // the current HI16 address. 
                    addrHiPrev = addrHi;
                    addrHi = null;
                    return (addr, null);
                }
                else
                {
                    // This LO16 relocation is "orphaned"; reuse the last HI16 if there is one.
                    if (addrHiPrev is null)
                        return (null, null);
                    if (!ctx.TryReadUInt32(addr, out uint valueLo))
                        return default;
                    uint ahl = (valueLo & 0xFFFF) + (uint) ctx.S;
                    valueLo = (valueLo & 0xFFFF0000u) | (ahl & 0xFFFF);
                    ctx.WriteUInt32(addr, valueLo);
                    return (addr, null);
                }
            case MIPSrt.R_MIPS_26:
                if (!ctx.TryReadUInt32(addr, out uint w))
                    return default;
                uint value26 = (uint) ((ctx.A << 2) + (ctx.P & 0xF0000000) + ctx.S) >> 2;
                ctx.WriteUInt32(addr, w & ~0x3FFFFFFu | value26 & 0x3FFFFFFu);
                return (addr, null);
            default:
                Debug.Print("ELF RiscV: unhandled 32-bit relocation {0}: {1}", rt, rela);
                return (addr, null);
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
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((MIPSrt)type).ToString();
        }
    }

    [Flags]
    public enum MIPSflags
    {
        EF_MIPS_ARCH = unchecked((int)(0xF0000000)), /* MIPS architecture level mask  */

        EF_MIPS_ARCH_1 = 0x00000000, /* -mips1 code.  */
        EF_MIPS_ARCH_2 = 0x10000000, /* -mips2 code.  */
        EF_MIPS_ARCH_3 = 0x20000000, /* -mips3 code.  */
        EF_MIPS_ARCH_4 = 0x30000000, /* -mips4 code.  */
        EF_MIPS_ARCH_5 = 0x40000000, /* -mips5 code.  */
        EF_MIPS_ARCH_32 = 0x50000000, /* MIPS32 code.  */
        //EF_MIPS_ARCH_6 = 0x50000000,
        EF_MIPS_ARCH_64 = 0x60000000, /* MIPS64 code.  */
        EF_MIPS_ARCH_32R2 = 0x70000000, /* MIPS32r2 code.  */
        EF_MIPS_ARCH_64R2 = unchecked((int)0x80000000), /* MIPS64r2 code.  */
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

        R_MIPS_SCN_DISP        = 32,
        R_MIPS_REL16           = 33,
        R_MIPS_ADD_IMMEDIATE   = 34,
        R_MIPS_PJUMP           = 35,
        R_MIPS_RELGOT          = 36,
        R_MIPS_JALR            = 37,
        R_MIPS_TLS_DTPMOD32    = 38,    // Module number 32 bit
        R_MIPS_TLS_DTPREL32    = 39,    // Module-relative offset 32 bit
        R_MIPS_TLS_DTPMOD64    = 40,    // Module number 64 bit
        R_MIPS_TLS_DTPREL64    = 41,    // Module-relative offset 64 bit
        R_MIPS_TLS_GD          = 42,    // 16 bit GOT offset for GD
        R_MIPS_TLS_LDM         = 43,    // 16 bit GOT offset for LDM
        R_MIPS_TLS_DTPREL_HI16 = 44,    // Module-relative offset, high 16 bits
        R_MIPS_TLS_DTPREL_LO16 = 45,    // Module-relative offset, low 16 bits
        R_MIPS_TLS_GOTTPREL    = 46,    // 16 bit GOT offset for IE
        R_MIPS_TLS_TPREL32     = 47,    // TP-relative offset, 32 bit
        R_MIPS_TLS_TPREL64     = 48,    // TP-relative offset, 64 bit
        R_MIPS_TLS_TPREL_HI16  = 49,    // TP-relative offset, high 16 bits
        R_MIPS_TLS_TPREL_LO16  = 50,    // TP-relative offset, low 16 bits
        R_MIPS_GLOB_DAT        = 51,
        /*
         * Introduced for MIPSr6.
         */
        R_MIPS_PC21_S2 = 60,
        R_MIPS_PC26_S2 = 61,

        R_MIPS_COPY            = 126,
        R_MIPS_JUMP_SLOT       = 127,
    }

    public class MipsRelocator64 : ElfRelocator64
    {
        private readonly Bitfield targ26 = new Bitfield(0, 26);

        public MipsRelocator64(ElfLoader64 elfLoader, SortedList<Address, ImageSymbol> imageSymbols) : base(elfLoader, imageSymbols)
        {
        }

        protected override ElfRelocation AdjustRelocation(ElfRelocation elfRelocation)
        {
            var elfNew = new ElfRelocation
            {
                Offset = elfRelocation.Offset,
                Info = elfRelocation.Info,
                Addend = elfRelocation.Addend,
                SymbolIndex = (int)(elfRelocation.Info >> 32),
            };
            return elfNew;
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rel, ElfSymbol symbol)
        {
            var addr = ctx.CreateAddress(ctx.P);
            if (ctx.P == 0)
                return default;

            switch ((Mips64Rt)rel.Info)
            {
            case Mips64Rt.R_MIPS_NONE:
                return (null, null);
            case Mips64Rt.R_MIPS_64:
                ulong A64;
                if (rel.Addend.HasValue)
                    A64 = (ulong) rel.Addend.Value;
                else if (!ctx.TryReadUInt64(addr, out A64))
                    return (null, null);
                    ctx.WriteUInt64(addr, ctx.S + A64);
                    return (addr, symbol);
            case Mips64Rt.R_MIPS_26:
                if (!ctx.TryReadUInt32(addr, out uint w))
                    return default;
                long sA = targ26.ReadSigned(ctx.A) << 2;
                uint n = (uint) ((ctx.S + (ulong) sA) >> 2);
                ctx.WriteUInt32(addr, (w & 0xFC000000u) | n);
                return (addr, null);
            default:
                ctx.Warn(addr, $"Unimplemented MIPS64 relocation type: {RelocationTypeToString((uint) rel.Info)}.");
                return (addr, symbol);
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Mips64Rt) type).ToString();
        }
    }

    public enum Mips64Rt
    {
        R_MIPS_NONE = 0, // none n/a none
        R_MIPS_16 = 1, // V-half16 any S + sign_extend(A)
                       //R_MIPS_32
        R_MIPS_ADD = 2, // T-word32 any S + A
                        //R_MIPS_REL
        R_MIPS_REL32 = 3, // T-word32 any S + A - EA
        R_MIPS_26 = 4, // T-targ26 local a ( ( (A << 2) | (P&0xf0000000) ) + S ) >> 2
                       // external a ( sign_extend(A<<2) + S ) >> 2
        R_MIPS_HI16 = 5, // T-hi16 any %high (AHL + S) d
        R_MIPS_LO16 = 6,  // T-lo16 any AHL + S
        //R_MIPS_GPREL
        R_MIPS_GPREL16 = 7, // 7 V-rel16
                    //external sign_extend(A) + S - GP
                    //local sign_extend(A) + S + GP0 - GP
        R_MIPS_LITERAL = 8, // V-lit16 local sign_extend(A) + L
                            //R_MIPS_GOT
        R_MIPS_GOT16 = 9,   // V-rel16
                            //external G
                            //local f
        R_MIPS_PC16  = 10,          // V-pc16 external sign_extend(A) + S - P
        R_MIPS_CALL16 = 11,         // e, m
        R_MIPS_CALL = 11,           // V-rel16 external G
        R_MIPS_GPREL32 = 12,        // T-word32 local A + S + GP0 - GP
        R_MIPS_SHIFT5 = 16,         // V-sh5 any S
        R_MIPS_SHIFT6 = 17,         // V-sh6 any S
        R_MIPS_64 = 18,             // T-word64 any S + A
        R_MIPS_GOT_DISP = 19,       // V-rel16 any G
        R_MIPS_GOT_PAGE = 20,       // V-rel16 any
        R_MIPS_GOT_OFST = 21,       // V-rel16 any
        R_MIPS_GOT_HI16 = 22,       // T-hi16 any %high(G)d
        R_MIPS_GOT_LO16 = 23,       // T-lo16 any G
        R_MIPS_SUB  = 24,           // T-word64 any S - A
        R_MIPS_INSERT_A = 25,       // T-word32 any Insert addend as instruction immediately prior to addressed location. 
        R_MIPS_INSERT_B = 26,       // T-word32 any
        R_MIPS_DELETE = 27,         // T-word32 any Remove the addressed 32-bit object (normally an instruction). j
        R_MIPS_HIGHER = 28,         // T-hi16 any %higher(A+S)k
        R_MIPS_HIGHEST = 29,        // T-hi16 any %highest(A+S)l
        R_MIPS_CALL_HI16 = 30,      // T-hi16 any %high(G)d
        R_MIPS_CALL_LO16 = 31,      // T-lo16 any G
        R_MIPS_SCN_DISP = 32,       // T-word32 any S+A-scn_addr (Section displacement)
        R_MIPS_REL16 = 33,          // V-hw16 any S + A
        R_MIPS_ADD_IMMEDIATE = 34,  // V-half16 any oS + sign_extend(A)
        R_MIPS_PJUMP = 35,          // T-word32 any Deprecated (protected jump)
        R_MIPS_RELGOT = 36,         // T-word32 any qS + A - EA
        R_MIPS_JALR = 37,           // T-word32 any pProtected jump conversion
    }
}