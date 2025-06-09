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
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    public class XCoff32Loader : ProgramImageLoader
    {
        private ImageSegment? textSection;
        private ImageSegment? dataSection;
        private ImageSegment? bssSection;
        private ImageSegment? loaderSection;
        AuxFileHeader? aux = null;

        public XCoff32Loader(IServiceProvider services, ImageLocation imageLocation, byte[] rawImage) :
            base(services, imageLocation, rawImage)
        {
            this.PreferredBaseAddress = Address.Ptr32(0x0010_0000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            BeImageReader rdr = new BeImageReader(this.RawImage, 0);
            FileHeader str = rdr.ReadStruct<FileHeader>();
            if (str.f_opthdr != 0)
            {
                aux = rdr.ReadStruct<AuxFileHeader>();
            }
            if (str.f_nscns == 0)
                throw new BadImageFormatException("Expected at least one XCoff32 section.");
            var segments = new ImageSegment[str.f_nscns+1];
            for (int i = 1; i <= str.f_nscns; ++i)
            {
                segments[i] = LoadSegment(rdr);
            }
            var mem = new ByteMemoryArea(addrLoad ?? PreferredBaseAddress, RawImage);
            var seg = new ImageSegment("foo", mem, AccessMode.ReadWriteExecute);
            var map = new SegmentMap(segments.Skip(1).ToArray());
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc-be-32")!;
            var platform = new DefaultPlatform(Services, arch);
            return new Program(new ByteProgramMemory(map), arch, platform);
        }

        public void Relocate(Program program, Address addrLoad)
        {
            var (entries, symbols) = LoadLoaderSection(program.Architecture);
            symbols ??= new List<ImageSymbol>();
            var symDict = symbols
                .Where(s => s.Address.ToLinear() != 0)
                .ToSortedList(s => s.Address);
            entries ??= new List<ImageSymbol>();
            var ep = LoadProgramEntryPoint(program, symDict);
            if (ep is not null)
                entries.Add(ep);
            foreach (var e in entries)
            {
                program.EntryPoints[e.Address] = e;
            }
            foreach (var s in symDict)
            {
                program.ImageSymbols[s.Key] = s.Value;
            }
        }

        private ImageSymbol? LoadProgramEntryPoint(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            if (!aux.HasValue)
                return null;
            var entry = aux.Value.o_entry;
            if (entry == 0 || entry == ~0u)
                return null;
            var arch = program.Architecture;
            if (!program.TryCreateImageReader(arch, Address.Ptr32(entry), out var rdr))
                return null;
            // o_entry actually points to a function descriptor (think of it as a closure)
            // consisting of a pointer to the actual code and a pointer to the TOC.
            if (TryReadFunctionDescriptor(rdr, out Address addrEntry, out Constant ptrToc))
            {
                if (!symbols.TryGetValue(addrEntry, out var symEntry))
                {
                    symEntry = ImageSymbol.Procedure(arch, addrEntry);
                    symbols.Add(addrEntry, symEntry);
                }
                symEntry.ProcessorState = arch.CreateProcessorState();
                symEntry.ProcessorState.SetRegister(arch.GetRegister("r2")!, ptrToc);
                return symEntry;
            }
            return null;
        }

        private bool TryReadFunctionDescriptor(EndianImageReader rdr, out Address addrEntry, out Constant ptrToc)
        {
            if (rdr.TryReadUInt32(out uint uAddrEntry) &&
                rdr.TryReadUInt32(out uint uEntryTocPtr))
            {
                addrEntry = Address.Ptr32(uAddrEntry);
                ptrToc = Constant.Word32(uEntryTocPtr);
                return true;
            }
            addrEntry = default;
            ptrToc = null!;
            return false;
        }

        private ImageSegment LoadSegment(BeImageReader rdr)
        {
            var section = rdr.ReadStruct<Section>();
            var name = Utf8StringFromFixedArray(section.s_name);
            var addr = Address.Ptr32(section.v_paddr);
            var bytes = new byte[section.s_size];
            if (section.s_scnptr != 0)
            {
                Array.Copy(RawImage, section.s_scnptr, bytes, 0, bytes.Length);
            }
            var mem = new ByteMemoryArea(addr, bytes);
            var access = SectionAccessMode(section.s_flags);
            var seg = new ImageSegment(name, mem, access);
            Debug.Print("   {0} {1,-10} {2}", seg.Address, section.s_flags, seg.Name);
            SetWellKnownSection(section, SectionFlags.STYP_TEXT , seg, ref textSection);
            SetWellKnownSection(section, SectionFlags.STYP_DATA  , seg, ref dataSection);
            SetWellKnownSection(section, SectionFlags.STYP_BSS, seg, ref bssSection);
            SetWellKnownSection(section, SectionFlags.STYP_LOADER, seg, ref loaderSection);
            return seg;
        }

        private void SetWellKnownSection(Section section, SectionFlags flag, ImageSegment  seg, ref ImageSegment? segToSet)
        {
            if (section.s_flags.HasFlag(flag))
            {
                if (segToSet is not null)
                {
                    Services.RequireService<IEventListener>().Warn("Multiple XCoff {0} sections found, ignoring.", flag);
                    Debug.WriteLine("  ?Dup?");
                }
                else
                {
                    segToSet = seg;
                }
            }
        }

        private AccessMode SectionAccessMode(SectionFlags s_flags)
        {
            if (s_flags.HasFlag(SectionFlags.STYP_TEXT))
                return AccessMode.ReadExecute;
            if (s_flags.HasFlag(SectionFlags.STYP_DATA) ||
                s_flags.HasFlag( SectionFlags.STYP_BSS))
                return AccessMode.ReadWrite;
            return AccessMode.Read;
        }

        private static string Utf8StringFromFixedArray(byte[] abName)
        {
            int i = Array.IndexOf<byte>(abName, 0);
            if (i < 0)
                i = abName.Length;
            return Encoding.UTF8.GetString(abName, 0, i);
        }

        private (List<ImageSymbol>?, List<ImageSymbol>?) LoadLoaderSection(IProcessorArchitecture arch)
        {
            if (this.loaderSection is null)
                return (null, null);
            var rdr = (BeImageReader) loaderSection.MemoryArea.CreateBeReader(0);
            var hdr = rdr.ReadStruct<LoaderSectionHeader>();

            // In XCoff32 files, the symbols appear immediately after the header.
            var symbols = ReadSymbols(rdr, hdr.l_nsyms);
            
            // ...followed by the relocations
            var relocs = ReadRelocations(rdr, hdr.l_nreloc, symbols);
            return (
                null, 
                symbols.Select((sym, i) => MakeImageSymbol(sym, arch, (ByteMemoryArea) loaderSection.MemoryArea, hdr.l_stoff))
                    .ToList());
        }

        private List<Symbol> ReadSymbols(BeImageReader rdr, uint nsyms)
        {
            var syms = new List<Symbol>((int)nsyms);
            for (uint i = 0; i < nsyms; ++i)
            {
                var xcoffSym = rdr.ReadStruct<Symbol>();
                syms.Add(xcoffSym);
            }
            return syms;
        }

        private ImageSymbol MakeImageSymbol(in Symbol xcoffSym, IProcessorArchitecture arch, ByteMemoryArea loaderArea, uint strTableBase)
        {
            var name = ReadSymbolName(xcoffSym, loaderArea, strTableBase);
            var stype = GetSymbolType(xcoffSym.l_smclas);
            Debug.Print("    {0:X4} {1} {2}", xcoffSym.l_value, xcoffSym.l_smclas, name);
            return ImageSymbol.Create(stype, arch, Address.Ptr32(xcoffSym.l_value), name);
        }

        private static Core.Loading.SymbolType GetSymbolType(SymbolClass l_smclas)
        {
            return l_smclas switch
            {
                SymbolClass.XMC_PR => Core.Loading.SymbolType.Procedure,
                SymbolClass.XMC_RW => Core.Loading.SymbolType.Data,
                _ => Core.Loading.SymbolType.Unknown
            };
        }

        private string ReadSymbolName(in Symbol xcoffSym, ByteMemoryArea loaderArea, uint strTableBase)
        {
            if (ByteMemoryArea.ReadBeUInt32(xcoffSym.l_name, 0) == 0)
            {
                var strOffset = strTableBase + ByteMemoryArea.ReadBeUInt32(xcoffSym.l_name, 4);
                // The length of the string is two bytes back, and includes the null terminator
                int strlen = loaderArea.ReadBeUInt16(strOffset - 2) - 1;
                return Encoding.UTF8.GetString(loaderArea.Bytes, (int) strOffset, strlen);
            }
            else
                return Utf8StringFromFixedArray(xcoffSym.l_name);
        }

        private object ReadRelocations(BeImageReader rdr, uint nreloc, List<Symbol> symbols)
        {
            // Compute the loader section symbol table index (n-th entry) of the symbol that is being referenced. Values 0, 1, and 2 are implicit references to .text, .data, and .bss sections, respectively. Symbol index 3 is the index for the first symbol actually contained in the loader section symbol table.

            var badAddr = Address.Ptr32(0);
            var symsToAddresses = new List<Address>
            {
                this.textSection?.Address ?? badAddr,
                this.dataSection?.Address ?? badAddr,
                this.bssSection?.Address ?? badAddr
            };
            symsToAddresses.AddRange(symbols.Select(s => Address.Ptr32(s.l_value)));
            Debug.WriteLine($"== Relocations {nreloc} ========================");
            for (uint i = 0; i < nreloc; ++i)
            {
                var reloc = rdr.ReadStruct<Relocation>();
                Debug.Print("    {0:X8} {1:X8}:{2} {3:X2} {4} {5:X4}",
                    reloc.l_vaddr,
                    reloc.l_symndx,
                    symsToAddresses[(int)reloc.l_symndx],
                    reloc.l_rsize,
                    reloc.l_rtype,
                    reloc.l_rsecnm);
            }
            return nreloc;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
	    [Endian(Endianness.BigEndian)]
        private struct FileHeader
        {
            public ushort f_magic;
            public ushort f_nscns;
            public uint f_timdat;
            public uint f_symptr;
            public uint f_nsyms;
            public ushort f_opthdr;
            public ushort f_flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        private struct AuxFileHeader
        {
            public ushort o_mflag; // Flags, offset 0
            public ushort o_vstamp; // Version, offset 2
            public uint o_tsize; // Text size in bytes, offset 4
            public uint o_dsize; // Initialized data size in bytes, offset 8
            public uint o_bsize; // Uninitialized data size in bytes, offset 12
            public uint o_entry; // Entry point descriptor(virtual address), offset 16
            public uint o_text_start; // Base address of text(virtual address), offset 20
            public uint o_data_start; // Base address of data(virtual address), offset 24
            public uint o_toc; // Address of TOC anchor, offset 28
            public ushort o_snentry; // Section number for entry point, offset 32
            public ushort o_sntext; // Section number for .text, offset 34
            public ushort o_sndata; // Section number for .data, offset 36
            public ushort o_sntoc; // Section number for TOC, offset 38
            public ushort o_snloader; // Section number for loader data, offset 40
            public ushort o_snbss; // Section number for .bss, offset 42
            public ushort o_algntext; // Maximum alignment for .text, offset 44
            public ushort o_algndata; // Maximum alignment for .data, offset 46
            public ushort o_modtype; // Module type field, offset 48
            public byte o_cpuflag; // Bit flags - cpu types of objects, offset 50
            public byte o_cputype; // Reserved for CPU type, offset 51
            public uint o_maxstack; // Maximum stack size allowed(bytes), offset 52
            public uint o_maxdata; // Maximum data size allowed(bytes), offset 56
            public uint o_debugger; // Reserved for debuggers., offset 60
            public byte o_textpsize; // Requested text page size., offset 64
            public byte o_datapsize; // Requested data page size., offset 65
            public byte o_stackpsize; // Requested stack page size., offset 66
            public byte o_flags; // Flags and thread-local storage alignment, offset 67
            public ushort o_sntdata; // Section number for .tdata, offset 68
            public ushort o_sntbss; // Section number for .tbss, offset 70
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        public struct Section
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] s_name;   // Section name
            public uint s_paddr;    // Physical address, offset 8
            public uint v_paddr;    // Physical address, offset 12
            public uint s_size;     // Section size, offset 16
            public uint s_scnptr;   // Offset in file to raw data for section, offset 20
            public uint s_relptr;   // Offset in file to relocation entries for section, offset 24
            public uint s_lnnoptr;  // Offset in file to line number entries for section, offset 28
            public ushort s_nreloc; // Number of relocation entries, offset 32
            public ushort s_nlnno;  // Number of line number entries, offset 34
            public SectionFlags s_flags;  // Flags to define the section type, offset 36
        }

        [Flags]
        public enum SectionFlags : uint
        { 
            STYP_PAD =     0x0008,
            STYP_DWARF =   0x0010,
            STYP_TEXT =    0x0020,
            STYP_DATA =    0x0040,
            STYP_BSS =     0x0080,
            STYP_EXCEPT =  0x0100,
            STYP_LOADER =  0x1000,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        public struct LoaderSectionHeader
        {
            public uint l_version;      // Loader section version number, Offset: 0
            public uint l_nsyms;        // Number of symbol table entries, Offset: 4
            public uint l_nreloc;       // Number of relocation table entries, Offset: 8
            public uint l_istlen;       // Length of import file ID string table, Offset: 12
            public uint l_nimpid;       // Number of import file IDs, Offset: 16
            public uint l_impoff;       // Offset to start of import file IDs, Offset: 20
            public uint l_stlen;        // Length of string table, Offset: 24
            public uint l_stoff;        // Offset to start of string table, Offset: 28
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        public struct Symbol
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte [] l_name;          // Symbol name or byte offset into string table, Offset: 0
                                            // or Byte offset into string table of symbol name
            public uint l_value;            // Address field, Offset: 8
            public ushort l_scnum;          // Section number containing symbol, Offset: 12
            public SymbolType l_smtype;     // Symbol type, export, import flags, Offset: 14
            public SymbolClass l_smclas;    // Symbol storage class, Offset: 15
            public uint l_ifile;            // Import file ID; ordinal of import file IDs, Offset: 16
            public uint l_parm;             // Parameter type-check field, Offset: 20
        }

        // https://www.ibm.com/support/knowledgecenter/ssw_aix_72/filesreference/XCOFF.html#XCOFF__sua3i125jbau
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Endian(Endianness.BigEndian)]
        public struct Relocation
        {

            public uint l_vaddr;            // Address field, Offset: 0
            public uint l_symndx;           // Loader section symbol table index of referenced item, Offset: 4
            public byte l_rsize;            // Relocation size
            public RelocationType l_rtype;  // Relocation type
            public ushort l_rsecnm;         // File section number being relocated
        }

        [Flags]
        public enum SymbolType : byte
        {
            Reserved = 0x80,
            Import = 0x40,  // Specifies an imported symbol.
            Entry =  0x20,  // Specifies an entry point descriptor symbol.
            Export = 0x10,  // Specifies an exported symbol.
            Weak = 0x08,    // Specifies a weak symbol.

            XTY_ER = 0, // Specifies an external reference providing a symbol table entry for an external (global) symbol contained in another XCOFF object file.
            XTY_SD = 1, // Specifies the csect section definition, providing the definition of the smallest initialized unit within an XCOFF object file.
            XTY_LD = 2, // Specifies the label definition, providing the definition of the global entry points for initialized csects. An uninitialized csect of type XTY_CM may not contain a label definition.
            XTY_CM = 3, // Specifies a common (BSS uninitialized data) csect definition, providing the definition of the smallest uninitialized unit within an XCOFF object file.
        }

        public enum SymbolClass : byte
        {
            // The following storage-mapping classes are read-only and normally mapped to the .text section:
            XMC_PR = 0,     // Specifies program code. The csect contains the executable instructions of the program.
            XMC_RO = 1,     // Specifies a read-only constant. The csect contains data that is constant and will not change during execution of the program.
            XMC_DB = 2,     // Specifies the debug dictionary table. The csect contains symbolic-debugging data or exception-processing data. This storage mapping class was defined to permit compilers with special symbolic-debugging or exception-processing requirements to place data in csects that are loaded at execution time but that can be collected separately from the executable code of the program.
            XMC_GL  = 6,    // Specifies global linkage. The csect provides the interface code necessary to handle csect relative calls to a target symbol that can be out-of-module.This global linkage csect has the same name as the target symbol and becomes the local target of the relative calls.As a result, the csect maintains position-independent code within the .text section of the executable XCOFF object file.
            XMC_XO = 7,     // Specifies extended operation. A csect of this type has no dependency on (references through) the TOC.It is intended to reside at a fixed address in memory such that it can be the target of a branch - absolute instruction.
            XMC_TI = 12,    //     Reserved.
            XMC_TB = 13,    //      Reserved.

            // The following storage - mapping classes are read / write and normally mapped to the.data or.bss section:

            XMC_RW = 5,       // Specifies read / write data.A csect of this type contains initialized or uninitialized data that is permitted to be modified during program execution.If the x_smtyp value is XTY_SD, the csect contains initialized data and is mapped into the.data section.If the x_smtyp value is XTY_CM, the csect is uninitialized and is mapped into the.bss section.Typically, all the initialized static data from a C source file is contained in a single csect of this type.The csect would have a storage class value of C_HIDEXT.An initialized definition for a global data scalar or structure from a C source file is contained in its own csect of this type.The csect would have a storage class value of C_EXT.A csect of this type is accessible by name references from other object files.
            XMC_TC0 = 15, // Specifies TOC anchor for TOC addressability. This is a zero-length csect whose n_value address provides the base address for TOC relative addressability.Only one csect of type XMC_TC0 is permitted per section of an XCOFF object file. In implementations that permit compilers and assemblers to generate multiple .data sections, there must be a csect of type XMC_TC0 in each section that contains data that is referenced (by way of a relocation entry) as a TOC-relative data item.Some hardware architectures limit the value that a relative displacement field within a load instruction may contain.This limit then becomes an inherent limit on the size of a TOC for an executable XCOFF object. For RS/6000®, this limit is 65,536 bytes, or 16,384 4-byte TOC entries.

            // Specifies general TOC entries.Csects of this type are the same size as a pointer and contain the address of other csects or global symbols.These csects provide addressability to other csects or symbols. The symbols might be in either the local executable XCOFF object or another executable XCOFF object. The binder uses special processing semantics to eliminate duplicate TOC entries as follows:
            // Symbols that have a storage class value of C_EXT are global symbols and must have names(a non-null n_name field). These symbols require no special TOC processing logic to combine duplicate entries.Duplicate entries with the same n_name value are combined into a single entry.
            // Symbols that have a storage class value of C_HIDEXT are not global symbols, and duplicate entries are resolved by context.Any two such symbols are defined as duplicates and combined into a single entry whenever the following conditions are met:
            // The n_name fields are the same. That is, they have either a null name or the same name string.
            // Each is the same size as a pointer.
            // Each has a single RLD entry that references external symbols with the same name.
            // To minimize the number of duplicate TOC entries that the binder cannot combine, compilers and assemblers should adhere to a common naming convention for TOC entries. By convention, compilers and assemblers produce TOC entries that have a storage class value of C_HIDEXT and an n_name string that is the same as the n_name value for the symbol that the TOC entry addresses.

            //Storage-mapping classes XMC_TC and XMC_TE are equivalent, except that the binder should map XMC_TE symbols after XMC_TC and XMC_TD symbols.

            XMC_TC = 3,
            XMC_TE = 22,

            //Specifies scalar data entry in the TOC.A csect that is a special form of an XMC_RW csect that is directly accessed from the TOC by compiler generated code.This lets some frequently used globol symbols be accessed directly from the TOC rather than indirectly through an address pointer csect contained in the TOC. A csect of type XMC_TD has the following characteristics:
            //The compiler generates code that is TOC relative to directly access the data contained in the csect of type XMC_TD.
            //It is 4-bytes long or less.
            //It has initialized data that can be modified as the program runs.
            //If a same named csect of type XMC_RW or XMC_UA exist, it is replaced by the XMC_TD csect.
            //For the cases where TOC scalar cannot reside in the TOC, the binder must be capable of transforming the compiler generated TOC relative instruction into a conventional indirect addressing instruction sequence. This transformation is necessary if the TOC scalar is contained in a shared object.
            XMC_TD = 16,

            // Specifies a csect containing a function descriptor, which contains the following three values:
            // The address of the executable code for a function.
            // The address of the TOC anchor (TOC base address) of the module that contains the function.
            // The environment pointer(used by languages such as Pascal and PL/I).
            // There is only one function descriptor csect for a function, and it must be contained within the same executable as the function itself is contained.The function descriptor has a storage class value of C_EXT and has an n_name value that is the same as the name of the function in the source file.The addresses of function descriptors are imported to and exported from an executable XCOFF file.
            XMC_DS = 10,

            XMC_SV = 8,    // Specifies 32-bit supervisor call descriptor csect.The supervisor call descriptors are contained within the operating system kernel.To an application program, the reference to a supervisor call descriptor is treated the same as a reference to a regular function descriptor.It is through the import/export mechanism that a function descriptor is treated as a supervisor call descriptor.These symbols are only available to 32-bit programs.
            XMC_SV64 = 17, // Specifies 64-bit supervisor call descriptor csect.See XMV_SV for supervisor call information.These symbols are only available to 64-bit programs.
            XMC_SV3264 = 18, // Specifies supervisor call descriptor csect for both 32-bit and 64-bit.See XMV_SV for supervisor call information.These symbols are available to both 32-bit and 64-bit programs.
            XMC_UA = 4, // Unclassified. This csect is treated as read/write.This csect is frequently produced by an assembler or object file translator program that cannot determine the true classification of the resultant csect.
            XMC_BS = 9, // Specifies BSS class (uninitialized static internal). A csect of this type is uninitialized, and is intended to be mapped into the .bss section. This type of csect must have a x_smtyp value of XTY_CM.
            XMC_UC  = 11, // Specifies unnamed FORTRAN common.A csect of this type is intended for an unnamed and uninitialized FORTRAN common. It is intended to be mapped into the .bss section. This type of csect must have a x_smtyp value of XTY_CM.


            //The following storage mapping class is read-write and is mapped to the.tdata section:

            XMC_TL = 20 ,// Specifies read/write thread-local data.A csect of this type contains initialized data that is local to every thread in a process. When a new thread is created, a csect with type XMC_TL is used to initialize the thread-local data for the thread.

            // The following storage mapping class is read-write and is mapped to the.tbss section:

            XMC_UL = 21, // Specifies read/write thread-local data.A csect of this type contains uninitialized data that is local to every thread in a process. When a new thread is created, the thread-local storage for a csect of this type is initialized to zero.
        }

        public enum RelocationType : byte
        {
            R_POS = 0x00,   // Specifies positive relocation. Provides the address of the symbol specified by the r_symndx field.

            R_NEG = 0x01,   // Specifies negative relocation. Provides the negative of the address of the symbol specified by the r_symndx field.

            R_REL = 0x02 ,      // Specifies relative-to-self relocation. Provides a displacement value between the address of the symbol specified by the r_symndx field and the address of the csect to be modified.

            R_TOC = 0x03,   // Specifies a relocation that is relative to TOC. Provides a displacement value that is the difference between the address value in the symbol specified by the r_symndx field and the address of the TOC anchor csect. The TOC anchor csect is a symbol with storage-mapping class defined as XMC_TC0 and with a length of 0. At most, one TOC anchor csect is allowed per XCOFF section.
                            // A link editor is allowed to transform the instruction referenced by the r_vaddr field. The symbol specified by the r_symndxfield is a TOC symbol if its storage-mapping class is XMC_TC, and the TOC symbol contains the address of another symbol that is within 32,768 bites of the TOC anchor or the thread-local storage base. Therefore, if the referenced instruction is a load, and the symbol specified by the r_symndx field is a TOC symbol, the load can be converted in to an add-immediate instruction.This transformation eliminates a storage reference during execution.If the instruction is transformed, the R_TOC relocation type is replaced by a R_TRLA relocation type when the output file is written.This enables a reverse transformation if the object is re-linked.

            R_TRL = 0x04,   // Specifies a relocation that is relative to TOC.This relocation entry is treated the same as an R_TOC relocation entry, except that link editors are not allowed to convert the instruction from a load to an add-immediate instruction.

            R_TRLA = 0x13,  // Specifies a relocation that is either relative to TOC or relative to the thread-local storage base. The instruction specified by the r_vaddr field is an add-immediate instruction, and the symbol specified by the r_symndx field must be a TOC symbol, which means that its storage-mapping class is XMC_TC.This instruction is previously transformed by a link editor from a load instruction into an add-immediate instruction.The link editor transforms the instruction back into a load instruction, and changes the relocation type from R_TRLA to R_TOC.The instruction can be transformed again as described for the R_TOC relocation entry.
                            // Compilers are not permitted to generate this relocation type.


            R_GL = 0x05,   // Specifies Global Linkage-External TOC address relocation. Provides the address of the TOC associated with a defined external symbol. The external symbol with the required TOC address is specified by the r_symndx field of the relocation entry.This relocation entry provides a method of accessing the address of the TOC contained within the same executable where the r_symndx external symbol is defined.

            R_TCL = 0x06,   // Specifies local object TOC address relocation. Provides the address of the TOC associated with a defined external symbol. The external symbol for which the TOC address is required is specified by the r_symndx field of the relocation entry.The external symbol is defined locally within the resultant executable. This relocation entry provides a method of accessing the address of the TOC contained within the same executable where the r_symndx external symbol is defined.

            R_RL = 0x0C,    // Treated the same as the R_POS relocation type.

            R_RLA = 0x0D,   // Treated the same as the R_POS relocation type.

            R_REF = 0x0F,   // Specifies a nonrelocating reference to prevent garbage collection (by the binder) of a symbol.This relocation type is intended to provide compilers and assemblers a method to specify that a given csect has a dependency upon another csect without using any space in the actual csect.The reason for making the dependency reference is to prevent the binder from garbage-collecting(eliminating) a csect for which another csect has an implicit dependency.

            R_BA = 0x08,    // Treated the same as the R_RBA relocation type.

            R_RBA = 0x18,   // Specifies branch absolute relocation.Provides the address of the symbol specified by the r_symndx field as the target address of a branch instruction.The instruction can be modified to a (relative) branch instruction if the target address is relocatable.

            R_BR = 0x0A,    // Treated the same as the R_RBR relocation type.

            R_RBR = 0x1A,   // Specifies (relative) branch relocation.Provides a displacement value between the address of the symbol specified by the r_symndx field and the address of the csect containing the branch instruction to be modified. The instruction can be modified to an absolute branch instruction if the target address is not relocatable.
                            // The R_RBR relocation type is the standard branch relocation type used by compilers and assemblers for the.This relocation type along with glink code allows an executable object file to have a text section that is position-independent.

            R_TLS = 0x20,   // Specifies thread-local storage relocation, using the general-dynamic model.Provides an offset into the thread-local storage for the module.

            R_TLS_IE = 0x21, // Same as R_TLS, except that the initial-exec model is used.That is, the referenced symbol must be exported by the main program or a module that is loaded at exec time.

            R_TLS_LD = 0x22, // Same as R_TLS, except that the local-dynamic model is used.That is, the referenced symbol must be in the referencing module.

            R_TLS_LE = 0x23,    // Same as R_TLS, except that local-exec model is used.That is, both the reference and the referenced symbol must be in the main program.
            R_TLSM = 0x24,  // Specifies thread-local storage relocation.Provides a handle for the thread-local storage of the referenced variable. The handle is used by the pthread runtime to locate the thread-local storage.
            R_TLSML = 0x25, // Specifies thread-local storage relocation.Provides a handle for the module containing the reference.The r_symndx field must specify the symbol table index of the csect symbol containing the reference.
            R_TOCU = 0x30,  // Specifies the high-order 16 bits of a TOC-relative relocation. Similar to the R_TOC relocation, a displacement value is computed.The displacement value is the difference between the address value in the symbol that the r_symndx field specifies and the address of the TOC anchor csect.The high-order 16 bits of the displacement are used to update the instruction. This relocation can overflow if the TOC size is larger than 231 bytes.
            R_TOCL = 0x31,  // Specifies the low-order 16 bits of a TOC-relative relocation. Similar to the R_TOC relocation, a displacement value is computed.The displacement value is the difference between the address value in the symbol that the r_symndx field specifies and the address of the TOC anchor csect.The low-order 16 bits of the displacement are used to update the instruction.
        }
    }
}
