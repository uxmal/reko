﻿#region License
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
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public abstract class ElfLoader
    {
        public const int ELFOSABI_NONE = 0x00;  // No specific ABI specified.
        public const int ELFOSABI_HPUX = 1;     // Hewlett-Packard HP-UX 
        public const int ELFOSABI_NETBSD = 2;   // NetBSD 
        public const int ELFOSABI_GNU = 3;      // GNU 
        public const int ELFOSABI_LINUX = 3;    // Linux  historical - alias for ELFOSABI_GNU 
        public const int ELFOSABI_SOLARIS = 6;  // Sun Solaris 
        public const int ELFOSABI_AIX = 7;      // AIX 
        public const int ELFOSABI_IRIX = 8;     // IRIX 
        public const int ELFOSABI_FREEBSD = 9;  // FreeBSD 
        public const int ELFOSABI_TRU64 = 10;   // Compaq TRU64 UNIX 
        public const int ELFOSABI_MODESTO = 11; // Novell Modesto 
        public const int ELFOSABI_OPENBSD = 12; // Open BSD 
        public const int ELFOSABI_OPENVMS = 13; // Open VMS 
        public const int ELFOSABI_NSK = 14;     // Hewlett-Packard Non-Stop Kernel 
        public const int ELFOSABI_AROS = 15;    // Amiga Research OS 
        public const int ELFOSABI_FENIXOS = 16; // The FenixOS highly scalable multi-core OS 
        
        public const byte ELFDATA2LSB = 1;
        public const byte ELFDATA2MSB = 2;

        public const int ELFOSABI_CELL_LV2 = 0x66;     // PS/3 has this in its files
        public const uint SHF_WRITE = 0x1;
        public const uint SHF_ALLOC = 0x2;
        public const uint SHF_EXECINSTR = 0x4;
        public const uint SHF_REKOCOMMON = 0x08000000;  // A hack until we determine what should happen with SHN_COMMON symbols
        public const int DT_NULL = 0;
        public const int DT_NEEDED = 1;
        public const int DT_STRTAB = 5;
        public const int STT_NOTYPE = 0;			// Symbol table type: none
        public const int STT_FUNC = 2;				// Symbol table type: function
        public const int STT_SECTION = 3;
        public const int STT_FILE = 4;
        public const int STB_GLOBAL = 1;
        public const int STB_WEAK = 2;

        public const uint PF_R = 4;
        public const uint PF_W = 2;
        public const uint PF_X = 1;

        protected ElfImageLoader imgLoader;
        protected Address m_uPltMin;
        protected Address m_uPltMax;
        protected IPlatform platform;
        protected byte[] rawImage;

        protected ElfLoader(ElfImageLoader imgLoader, ushort machine, byte endianness)
        {
            this.imgLoader = imgLoader;
            this.Architecture = CreateArchitecture(machine, endianness);
            this.Symbols = new Dictionary<ElfSection, List<ElfSymbol>>();
            this.Sections = new List<ElfSection>();
            this.ExternalProcedures = new Dictionary<Address, ExternalProcedure>();
        }

        public IProcessorArchitecture Architecture { get; private set; }
        public IServiceProvider Services { get { return imgLoader.Services; } }
        public ElfRelocator Relocator { get; protected set; }
        public abstract Address DefaultAddress { get; }
        public List<ElfSection> Sections { get; private set; }
        public Dictionary<ElfSection, List<ElfSymbol>> Symbols { get; }
        public Dictionary<Address, ExternalProcedure> ExternalProcedures { get; }

        public static AccessMode AccessModeOf(ulong sh_flags)
        {
            AccessMode mode = AccessMode.Read;
            if ((sh_flags & SHF_WRITE) != 0)
                mode |= AccessMode.Write;
            if ((sh_flags & SHF_EXECINSTR) != 0)
                mode |= AccessMode.Execute;
            return mode;
        }

        public static SortedList<Address, MemoryArea> AllocateMemoryAreas(IEnumerable<Tuple<Address, uint>> segments)
        {
            var mems = new SortedList<Address, MemoryArea>();
            Address addr = null;
            Address addrEnd = null;
            foreach (var pair in segments)
            {
                if (addr == null)
                {
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else if (addrEnd < pair.Item1)
                {
                    var size = (uint)(addrEnd - addr);
                    mems.Add(addr, new MemoryArea(addr, new byte[size]));
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else
                {
                    addrEnd = Address.Max(addrEnd, pair.Item1 + pair.Item2);
                }
            }
            if (addr != null)
            {
                var size = (uint)(addrEnd - addr);
                mems.Add(addr, new MemoryArea(addr, new byte[size]));
            }
            return mems;
        }

        protected virtual IProcessorArchitecture CreateArchitecture(ushort machineType, byte endianness)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            string arch;
            switch ((ElfMachine)machineType)
            {
            case ElfMachine.EM_NONE: return null; // No machine
            case ElfMachine.EM_SPARC: arch = "sparc32"; break;
            case ElfMachine.EM_386: arch = "x86-protected-32"; break;
            case ElfMachine.EM_X86_64: arch = "x86-protected-64"; break;
            case ElfMachine.EM_68K: arch = "m68k"; break;
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-32";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = "mips-be-32";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The MIPS architecture does not support ELF endianness value {0}", endianness));
                }
                break;
            case ElfMachine.EM_PPC: arch = "ppc32"; break;
            case ElfMachine.EM_PPC64: arch = "ppc64"; break;
            case ElfMachine.EM_ARM: arch = "arm"; break;
            case ElfMachine.EM_XTENSA: arch = "xtensa"; break;
            default:
                throw new NotSupportedException(string.Format("Processor format {0} is not supported.", machineType));
            }
            return cfgSvc.GetArchitecture(arch);
        }

        private static Dictionary<ElfSymbolType, SymbolType> mpSymbolType = new Dictionary<ElfSymbolType, SymbolType>
        {
            { ElfSymbolType.STT_FUNC, SymbolType.Procedure },
            { ElfSymbolType.STT_OBJECT, SymbolType.Data },
        };

        protected ImageSymbol CreateImageSymbol(ElfSymbol sym, uint headerType)
        {
            SymbolType st;
            if (sym.SectionIndex == 0 || sym.SectionIndex >= Sections.Count)
                return null;
            if (!mpSymbolType.TryGetValue(sym.Type, out st))
                return null;
            if (sym.SectionIndex == 0)
            {
                if (st != SymbolType.Procedure)
                    return null;
                st = SymbolType.ExternalProcedure;
            }
            var symSection = Sections[(int)sym.SectionIndex];
            // If this is a relocatable file, the symbol value is 
            // an offset from the section's virtual address. 
            // If this is an executable file, the symbol value is
            // the virtual address.
            var addr = headerType == ElfImageLoader.ET_REL
                ? symSection.Address + sym.Value
                : platform.MakeAddressFromLinear(sym.Value);

            return new ImageSymbol(addr)
            {
                Type = st,
                Name = sym.Name,
                Size = (uint)sym.Size,     //$REVIEW: is int32 a problem? Could such large objects (like arrays) exist?
                ProcessorState = Architecture.CreateProcessorState()
            };
        }

        public IPlatform LoadPlatform(byte osAbi, IProcessorArchitecture arch)
        {
            string envName;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            switch (osAbi)
            {
            case ELFOSABI_NONE: // Unspecified ABI
                envName = "elf-neutral";
                break;
            case ELFOSABI_CELL_LV2: // PS/3
                envName = "elf-cell-lv2";
                break;
            default:
                throw new NotSupportedException(string.Format("Unsupported ELF ABI 0x{0:X2}.", osAbi));
            }
            var env = cfgSvc.GetEnvironment(envName);
            this.platform = env.Load(Services, arch);
            return platform;
        }

        public virtual ElfRelocator CreateRelocator(ElfMachine machine)
        {
            throw new NotSupportedException(
                string.Format("Relocator for architecture {0} not implemented yet.",
                machine));
        }

        public Program LoadImage(IPlatform platform, byte[] rawImage)
        {
            Debug.Assert(platform != null);
            this.platform = platform;
            this.rawImage = rawImage;
            GetPltLimits();
            var addrPreferred = ComputeBaseAddress(platform);
            Dump();
            var segmentMap = LoadImageBytes(platform, rawImage, addrPreferred);
            var program = new Program(segmentMap, platform.Architecture, platform);
            return program;
        }

        public abstract ElfObjectLinker CreateLinker();

        public abstract void GetPltLimits();

        public abstract SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred);

        public ImageReader CreateReader(ulong fileOffset)
        {
            return imgLoader.CreateReader(fileOffset);
        }

        public ImageWriter CreateWriter(uint fileOffset)
        {
            return imgLoader.CreateWriter(fileOffset);
        }

        public abstract Address ComputeBaseAddress(IPlatform platform);
        public abstract int LoadProgramHeaderTable();
        public abstract void LoadSectionHeaders();
        public abstract List<ElfSymbol> LoadSymbolsSection(ElfSection symSection);
        public abstract void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols);

        public IEnumerable<ElfSymbol> GetAllSymbols()
        {
            return Symbols.Values.SelectMany(s => s);
        }

        public ElfSection GetSectionByIndex(uint shidx)
        {
            if (0 <= shidx && shidx < Sections.Count)
            {
                return Sections[(int)shidx];
            }
            else
            {
                return null;
            }
        }

        public ElfSection GetSectionInfoByName(string sectionName)
        {
            return Sections.FirstOrDefault(s => s.Name == sectionName);
        }

        protected string ReadSectionName(uint idxString)
        {
            ulong offset = (ulong)GetSectionNameOffset(idxString);
            return imgLoader.ReadAsciiString(offset);
        }

        public string GetSectionName(ushort st_shndx)
        {
            Debug.Assert(Sections != null);
            if (st_shndx < 0xFF00)
            {
                return Sections[st_shndx].Name;
            }
            else
            {
                switch (st_shndx)
                {
                case 0xFFF1: return "SHN_ABS";
                case 0xFFF2: return "SHN_COMMON";
                default: return st_shndx.ToString("X4");
                }
            }
        }
        protected abstract int GetSectionNameOffset(uint idxString);

        public string GetStrPtr(ElfSection section, ulong offset)
        {
            if (section == null)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentNullException("section");
            }
            // Get a pointer to the start of the string table and add the offset
            return imgLoader.ReadAsciiString(section.FileOffset + offset);
        }

        public void Dump()
        {
            var sw = new StringWriter();
            Dump(sw);
            Debug.Print(sw.ToString());
        }

        public abstract void Dump(TextWriter writer);

        protected string DumpShFlags(ulong shf)
        {
            return string.Format("{0}{1}{2}",
                ((shf & SHF_EXECINSTR) != 0) ? "x" : " ",
                ((shf & SHF_ALLOC) != 0) ? "a" : " ",
                ((shf & SHF_WRITE) != 0) ? "w" : " ");
        }

        public abstract Address GetEntryPointAddress(Address addrBase);

        // A map for extra symbols, those not in the usual Elf symbol tables

        public void AddSymbol(uint uNative, string pName)
        {
            //m_SymTab[uNative] = pName;
        }

        public IEnumerable<Elf64_Dyn> GetDynEntries64(ulong offset)
        {
            var rdr = imgLoader.CreateReader(offset);
            for (;;)
            {
                var dyn = new Elf64_Dyn();
                if (!rdr.TryReadInt64(out dyn.d_tag))
                    break;
                if (dyn.d_tag == DT_NULL)
                    break;
                long val;
                if (!rdr.TryReadInt64(out val))
                    break;
                dyn.d_val = val;
                yield return dyn;
            }
        }

        public IEnumerable<Elf32_Dyn> GetDynEntries(ulong offset)
        {
            var rdr = imgLoader.CreateReader(offset);
            for (;;)
            {
                var dyn = new Elf32_Dyn();
                if (!rdr.TryReadInt32(out dyn.d_tag))
                    break;
                if (dyn.d_tag == DT_NULL)
                    break;
                int val;
                if (!rdr.TryReadInt32(out val))
                    break;
                dyn.d_val = val;
                yield return dyn;
            }
        }

        /// <summary>
        /// Find the names of all shared objects this image depends on.
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetDependencyList(byte[] rawImage);

        /*==============================================================================
         * FUNCTION:	  ElfBinaryFile::GetImportStubs
         * OVERVIEW:	  Get an array of addresses of imported function stubs
         *					This function relies on the fact that the symbols are sorted by address, and that Elf PLT
         *					entries have successive addresses beginning soon after m_PltMin
         * PARAMETERS:	  numImports - reference to integer set to the number of these
         * RETURNS:		  An array of native ADDRESSes
         *============================================================================*/
        uint GetImportStubs(out int numImports)
        {
            int n = 0;
#if NYI
            ADDRESS a = m_uPltMin;
            std_map<ADDRESS, string>.iterator aa = m_SymTab.find(a);
            std_map<ADDRESS, string>.iterator ff = aa;
            bool delDummy = false;
            if (aa == m_SymTab.end())
            {
                // Need to insert a dummy entry at m_uPltMin
                delDummy = true;
                m_SymTab[a] = "";
                ff = m_SymTab.find(a);
                aa = ff;
                aa++;
            }
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                n++;
                a = aa.first;
                aa++;
            }
            // Allocate an array of ADDRESSESes
            m_pImportStubs = new ADDRESS[n];
            aa = ff; // Start at first
            a = aa.first;
            int i = 0;
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                m_pImportStubs[i++] = a;
                a = aa.first;
                aa++;
            }
            if (delDummy)
                m_SymTab.erase(ff); // Delete dummy entry
#endif
            numImports = n;
            return 0; //m_pImportStubs[];
        }



        public string GetStrPtr(Elf32_SHdr sect, uint offset)
        {
            if (sect == null)
            {
                // Most commonly, this will be an null, because a call to GetSectionByName() failed
                throw new ArgumentException("GetStrPtr passed null section.");
            }
            // Get a pointer to the start of the string table and add the offset
            return imgLoader.ReadAsciiString(sect.sh_offset + offset);
        }

        public string GetStrPtr64(Elf64_SHdr sect, uint offset)
        {
            if (sect == null)
            {
                // Most commonly, this will be an null, because a call to GetSectionByName() failed
                throw new ArgumentException("GetStrPtr passed null section.");
            }
            // Get a pointer to the start of the string table and add the offset
            return imgLoader.ReadAsciiString(sect.sh_offset + offset);
        }

        protected bool IsLoadable(ulong p_vaddr, ProgramHeaderType p_type)
        {
            if (p_vaddr == 0)
                return false;
            return (p_type == ProgramHeaderType.PT_LOAD ||
                    p_type == ProgramHeaderType.PT_DYNAMIC);
        }

        public void LoadSymbols()
        {
            foreach (var section in Sections.Where(s =>
                s.Type == SectionHeaderType.SHT_SYMTAB ||
                s.Type == SectionHeaderType.SHT_DYNSYM))
            {
                Symbols[section] = LoadSymbolsSection(section);
            }
        }

        public string ReadAsciiString(ulong v)
        {
            return imgLoader.ReadAsciiString(v);
        }

        public abstract RelocationResults Relocate(Program program, Address addrLoad);
    }

    public class ElfLoader64 : ElfLoader
    {
        private byte osAbi;

        public ElfLoader64(ElfImageLoader imgLoader, Elf64_EHdr elfHeader, byte[] rawImage, byte osAbi, byte endianness)
            : base(imgLoader, elfHeader.e_machine, endianness)
        {
            this.Header64 = elfHeader;
            this.osAbi = osAbi;
            base.rawImage = rawImage;
            this.ProgramHeaders64 = new List<Elf64_PHdr>();
            this.Relocator = CreateRelocator((ElfMachine)elfHeader.e_machine);
        }

        public Elf64_EHdr Header64 { get; set; }
        public List<Elf64_PHdr> ProgramHeaders64 { get; private set; }
        public override Address DefaultAddress { get { return Address.Ptr64(0x8048000); } }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            ulong uBaseAddr = ProgramHeaders64
                .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                .Min(ph => ph.p_vaddr);
            return platform.MakeAddressFromLinear(uBaseAddr);
        }

        protected override IProcessorArchitecture CreateArchitecture(ushort machineType, byte endianness)
        {
            string arch;
            switch ((ElfMachine)machineType)
            {
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-64";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = "mips-be-64";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The MIPS architecture does not support ELF endianness value {0}", endianness));
                }
                break;
            default:
                return base.CreateArchitecture(machineType, endianness);
            }
            var cfgSvc = Services.RequireService<IConfigurationService>();
            return cfgSvc.GetArchitecture(arch);
        }

        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker64(this, Architecture, rawImage);
        }

        private ImageSegmentRenderer CreateRenderer64(ElfSection shdr)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer64(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer64(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer64(this, shdr);
            default: return null;
            }
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine)
        {
            switch (machine)
            {
            case ElfMachine.EM_X86_64: return new x86_64Relocator(this);
            case ElfMachine.EM_PPC64: return new PpcRelocator64(this);
            case ElfMachine.EM_MIPS: return new MipsRelocator64(this);
            }
            return base.CreateRelocator(machine);
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header64.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.Address,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection != null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection != null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in ProgramHeaders64)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6:X8} p_align:{7:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress(platform));
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList(imgLoader.RawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                // DumpRela(sh);
            }
        }

        public override List<string> GetDependencyList(byte[] rawImage)
        {
            var result = new List<string>();
            var dynsect = GetSectionInfoByName(".dynamic");
            if (dynsect == null)
                return result; // no dynamic section = statically linked 

            var dynStrtab = GetDynEntries64(dynsect.FileOffset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
            if (dynStrtab == null)
                return result;
            var section = GetSectionInfoByAddr64(dynStrtab.d_ptr);
            foreach (var dynEntry in GetDynEntries64(dynsect.FileOffset).Where(d => d.d_tag == DT_NEEDED))
            {
                result.Add(imgLoader.ReadAsciiString(section.FileOffset + dynEntry.d_ptr));
            }
            return result;
        }

        public override Address GetEntryPointAddress(Address addrBase)
        {
            Address addr = null;
            //$REVIEW: should really have a subclassed "Ps3ElfLoader"
            if (osAbi == ElfLoader.ELFOSABI_CELL_LV2)
            {
                // The Header64.e_entry field actually points to a 
                // "function descriptor" consisiting of two 32-bit 
                // pointers.
                var rdr = imgLoader.CreateReader(Header64.e_entry - addrBase.ToLinear());
                uint uAddr;
                if (rdr.TryReadUInt32(out uAddr))
                    addr = Address.Ptr32(uAddr);
            }
            else
            {
                addr = Address.Ptr64(Header64.e_entry);
            }
            return addr;
        }

        // Find the PLT limits. Required for IsDynamicLinkedProc(), e.g.
        public override void GetPltLimits()
        {
            var pPlt = GetSectionInfoByName(".plt");
            if (pPlt != null)
            {
                m_uPltMin = pPlt.Address;
                m_uPltMax = pPlt.Address + pPlt.Size;
            }
        }

        internal ElfSection GetSectionInfoByAddr64(ulong r_offset)
        {
            return
                (from sh in this.Sections
                 let addr = sh.Address != null ? sh.Address.ToLinear() : 0
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(uint idxString)
        {
            return (int)(Sections[Header64.e_shstrndx].FileOffset + idxString);
        }

        public string GetStrPtr64(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return imgLoader.ReadAsciiString(Sections[idx].FileOffset + offset);
        }

        public string GetSymbol64(int iSymbolSection, ulong symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbol64(iSymbolSection, symbolNo);
        }

        public string GetSymbol64(ElfSection symSection, ulong symbolNo)
        {
            var strSection = symSection.LinkedSection;
            ulong offset = symSection.FileOffset + symbolNo * symSection.EntrySize;
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt64(out offset);
            return GetStrPtr(symSection.LinkedSection, (uint)offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                ProgramHeaders64
                    .Where(p => IsLoadable(p.p_vaddr, p.p_type))
                    .Select(p => Tuple.Create(
                        platform.MakeAddressFromLinear(p.p_vaddr),
                        (uint)p.p_pmemsz)));
            foreach (var ph in ProgramHeaders64)
            {
                Debug.Print("ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_vaddr, ph.p_type))
                    continue;
                var vaddr = platform.MakeAddressFromLinear(ph.p_vaddr);
                MemoryArea mem;
                segMap.TryGetLowerBound(vaddr, out mem);
                if (ph.p_filesz > 0)
                    Array.Copy(
                        rawImage,
                        (long)ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long)ph.p_filesz);
            }
            var segmentMap = new SegmentMap(addrPreferred);
            foreach (var section in Sections)
            {
                if (section.Name == null || section.Address == null)
                    continue;
                MemoryArea mem;
                if (segMap.TryGetLowerBound(section.Address, out mem) &&
                    section.Address < mem.EndAddress)
                {
                    AccessMode mode = AccessModeOf(section.Flags);
                    var seg = segmentMap.AddSegment(new ImageSegment(
                        section.Name,
                        section.Address,
                        mem, mode)
                    {
                        Size = (uint)section.Size
                    });
                    seg.Designer = CreateRenderer64(section);
                }
                else
                {
                    //$TODO: warn
                }
            }
            segmentMap.DumpSections();
            return segmentMap;

        }

        public override int LoadProgramHeaderTable()
        {
            var rdr = imgLoader.CreateReader(Header64.e_phoff);
            for (int i = 0; i < Header64.e_phnum; ++i)
            {
                ProgramHeaders64.Add(Elf64_PHdr.Load(rdr));
            }
            return ProgramHeaders64.Count;
        }

        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header64.e_shoff);
            for (uint i = 0; i < Header64.e_shnum; ++i)
            {
                var shdr = Elf64_SHdr.Load(rdr);
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = shdr.sh_addr != 0
                        ? platform.MakeAddressFromLinear(shdr.sh_addr)
                        : null,
                    FileOffset = shdr.sh_offset,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                Sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < Sections.Count; ++i)
            {
                var section = Sections[i];
                section.Name = ReadSectionName(inames[i]);

                ElfSection linkSection = null;
                ElfSection relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(links[i]);
                    relSection = GetSectionByIndex(infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
        }

        public override List<ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            //Debug.Print("Symbols");
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new List<ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf64_Sym.Load(rdr);
                //Debug.Print("  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                //    i,
                //    ReadAsciiString(stringtableSection.FileOffset + sym.st_name),
                //    (ElfSymbolType)(sym.st_info & 0xF),
                //    sym.st_shndx,
                //    GetSectionName(sym.st_shndx),
                //    sym.st_value,
                //    sym.st_size);
                symbols.Add(new ElfSymbol
                {
                    Name = ReadAsciiString(stringtableSection.FileOffset + sym.st_name),
                    Type = (ElfSymbolType)(sym.st_info & 0xF),
                    SectionIndex = sym.st_shndx,
                    Value = sym.st_value,
                    Size = sym.st_size,
                });
            }
            return symbols;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            Relocator.Relocate(program);

            var entryPoints = new List<ImageSymbol>();
            var symbols = new SortedList<Address, ImageSymbol>();
            foreach (var sym in Symbols.Values.SelectMany(seg => seg))
            {
                var imgSym = CreateImageSymbol(sym, Header64.e_type);
                if (imgSym != null)
                {
                    symbols[imgSym.Address] = imgSym;
                }
            }

            var addrEntry = GetEntryPointAddress(addrLoad);
            if (addrEntry != null)
            {
                ImageSymbol entrySymbol;
                if (symbols.TryGetValue(addrEntry, out entrySymbol))
                {
                    entryPoints.Add(entrySymbol);
                }
                else
                {
                    var ep = new ImageSymbol(addrEntry)
                    {
                        ProcessorState = Architecture.CreateProcessorState()
                    };
                    entryPoints.Add(ep);
                }
            }
            return new RelocationResults(entryPoints, symbols);
        }

        public override void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            // Locate the GOT
            //$REVIEW: there doesn't seem to be a reliable way to get that
            // information.
            var got = program.SegmentMap.Segments.Values.FirstOrDefault(s => s.Name == ".got");
            if (got == null)
                return;

            var rdr = program.CreateImageReader(got.Address);
            while (rdr.Address < got.EndAddress)
            {
                var addrGot = rdr.Address;
                ulong uAddrSym;
                if (!rdr.TryReadUInt64(out uAddrSym))
                    break;

                var addrSym = Address.Ptr64(uAddrSym);
                ImageSymbol symbol;
                if (symbols.TryGetValue(addrSym, out symbol))
                {
                    // This GOT entry is a known symbol!
                    if (symbol.Type == SymbolType.Procedure)
                    {
                        //$TODO: look up function signature.
                        var gotSym = new ImageSymbol(addrGot, symbol.Name + "_GOT", new Pointer(new CodeType(), 4));
                        symbols[addrGot] = gotSym;
                        Debug.Print("Found GOT entry at {0}, changing symbol at {1}", gotSym, symbol);
                    }
                }
            }
        }
    }

    public class ElfLoader32 : ElfLoader
    {
        public ElfLoader32(ElfImageLoader imgLoader, Elf32_EHdr header32, byte[] rawImage, byte endianness)
            : base(imgLoader, header32.e_machine, endianness)
        {
            if (header32 == null)
                throw new ArgumentNullException("header32");
            this.Header = header32;
            this.ProgramHeaders = new List<Elf32_PHdr>();
            this.Relocator = CreateRelocator((ElfMachine)header32.e_machine);
            this.rawImage = rawImage;
        }

        public Elf32_EHdr Header { get; private set; }
        public List<Elf32_PHdr> ProgramHeaders { get; private set; }
        public override Address DefaultAddress { get { return Address.Ptr32(0x8048000); } }

        public static int ELF32_R_SYM(int info) { return ((info) >> 8); }
        public static int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        public static int ELF32_ST_TYPE(int i) { return ((i) & 0x0F); }
        public static byte ELF32_ST_INFO(int b, ElfSymbolType t) { return (byte)(((b) << 4) + ((byte)t & 0xF)); }

        // Add appropriate symbols to the symbol table.  secIndex is the section index of the symbol table.
        private void AddSyms(Elf32_SHdr pSect)
        {
            int e_type = this.Header.e_type;
            // Calc number of symbols
            uint nSyms = pSect.sh_size / pSect.sh_entsize;
            uint offSym = pSect.sh_offset;
            //m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            uint strIdx = pSect.sh_link; // sh_link points to the string table

            var siPlt = GetSectionInfoByName(".plt");
            Address addrPlt = siPlt != null ? siPlt.Address : null;
            var siRelPlt = GetSectionInfoByName(".rel.plt");
            uint sizeRelPlt = 8; // Size of each entry in the .rel.plt table
            if (siRelPlt == null)
            {
                siRelPlt = GetSectionInfoByName(".rela.plt");
                sizeRelPlt = 12; // Size of each entry in the .rela.plt table is 12 bytes
            }
            Address addrRelPlt = null;
            ulong numRelPlt = 0;
            if (siRelPlt != null)
            {
                addrRelPlt = siRelPlt.Address;
                numRelPlt = sizeRelPlt != 0 ? siRelPlt.Size / sizeRelPlt : 0u;
            }
            // Number of entries in the PLT:
            // int max_i_for_hack = siPlt ? (int)siPlt.uSectionSize / 0x10 : 0;
            // Index 0 is a dummy entry
            var symRdr = imgLoader.CreateReader(offSym);
            for (int i = 1; i < nSyms; i++)
            {
                uint name;
                if (!symRdr.TryReadUInt32(out name))
                    break;
                uint val;
                if (!symRdr.TryReadUInt32(out val)) //= (ADDRESS)elfRead4((int)m_pSym[i].st_value);
                    break;
                uint size;
                if (!symRdr.TryReadUInt32(out size))
                    break;
                byte info;
                if (!symRdr.TryReadByte(out info))
                    break;
                byte other;
                if (!symRdr.TryReadByte(out other))
                    break;
                ushort shndx;
                if (!symRdr.TryReadLeUInt16(out shndx))
                    break;

                if (name == 0)
                    continue; // Weird: symbol w no name.

                if (shndx >= Sections.Count)
                {

                }
                else
                {
                    var otherSection = Sections[shndx];
                }
                string str = GetStrPtr((int)strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Ensure no overwriting (except functions)
#if Nilx
                if (@m_SymTab.ContainsKey(val) || ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC)
                {
                    if (val == 0 && siPlt != null)
                    { //&& i < max_i_for_hack) {
                        throw new NotImplementedException();
                        // Special hack for gcc circa 3.3.3: (e.g. test/pentium/settest).  The value in the dynamic symbol table
                        // is zero!  I was assuming that index i in the dynamic symbol table would always correspond to index i
                        // in the .plt section, but for fedora2_true, this doesn't work. So we have to look in the .rel[a].plt
                        // section. Thanks, gcc!  Note that this hack can cause strange symbol names to appear
                        //val = findRelPltOffset(i, addrRelPlt, sizeRelPlt, numRelPlt, addrPlt);
                    }
                    else if (e_type == ET_REL)
                    {
                        throw new NotImplementedException();
#if NYI
                        int nsec = elfRead2(m_pSym[i].st_shndx);
                        if (nsec >= 0 && nsec < m_iNumSections)
                            val += GetSectionInfo(nsec)->uNativeAddr;
#endif
                    }

                    m_SymTab[val] = str;
                }
#endif
                Debug.Print("  Symbol {0} ({0:X}) with address {1:X} (segment {2} {2:X}): {3}", i, val, shndx, str);
            }
#if NYI
            ADDRESS uMain = GetMainEntryPoint();
            if (uMain != NO_ADDRESS && m_SymTab.find(uMain) == m_SymTab.end())
            {
                // Ugh - main mustn't have the STT_FUNC attribute. Add it
                string sMain = "main";
                m_SymTab[uMain] = sMain;
            }
            return;
#endif
        }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            if (ProgramHeaders.Count == 0)
                return Address.Ptr32(0);

            return Address.Ptr32(
                ProgramHeaders
                .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                .Min(ph => ph.p_vaddr));
        }

        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker32(this, Architecture, rawImage);
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine)
        {
            switch (machine)
            {
            case ElfMachine.EM_386: return new x86Relocator(this);
            case ElfMachine.EM_ARM: return new ArmRelocator(this);
            case ElfMachine.EM_MIPS: return new MipsRelocator(this);
            case ElfMachine.EM_PPC: return new PpcRelocator(this);
            case ElfMachine.EM_SPARC: return new SparcRelocator(this);
            case ElfMachine.EM_XTENSA: return new XtensaRelocator(this);
            case ElfMachine.EM_68K: return new M68kRelocator(this);
            }
            return base.CreateRelocator(machine);
        }

        public ImageSegmentRenderer CreateRenderer(ElfSection shdr)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer32(this, shdr);
            case SectionHeaderType.SHT_REL:
                return new RelSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer32(this, shdr);
            default: return null;
            }
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.Address,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection != null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection != null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in ProgramHeaders)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6:X8} p_align:{7:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress(platform));
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList(rawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                DumpRela(sh);
            }
        }

        private void DumpRela(ElfSection sh)
        {
            var entries = sh.Size / sh.EntrySize;
            var symtab = sh.LinkedSection;
            var rdr = imgLoader.CreateReader(sh.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                uint offset;
                if (!rdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!rdr.TryReadUInt32(out info))
                    return;
                int addend;
                if (!rdr.TryReadInt32(out addend))
                    return;

                uint sym = info >> 8;
                string symStr = GetStrPtr(symtab, sym);
                Debug.Print("  RELA {0:X8} {1,3} {2:X8} {3:X8} {4}", offset, info & 0xFF, sym, addend, symStr);
            }
        }

        public override List<string> GetDependencyList(byte[] rawImage)
        {
            var result = new List<string>();
            var dynsect = GetSectionInfoByName(".dynamic");
            if (dynsect == null)
                return result; // no dynamic section = statically linked 

            var dynStrtab = GetDynEntries(dynsect.FileOffset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
            if (dynStrtab == null)
                return result;
            var section = GetSectionInfoByAddr(dynStrtab.d_ptr);
            foreach (var dynEntry in GetDynEntries(dynsect.FileOffset).Where(d => d.d_tag == DT_NEEDED))
            {
                result.Add(imgLoader.ReadAsciiString(section.FileOffset + dynEntry.d_ptr));
            }
            return result;
        }

        public override Address GetEntryPointAddress(Address addrBase)
        {
            if (Header.e_entry == 0)
                return null;
            else
                return Address.Ptr32(Header.e_entry);
        }

        public override void GetPltLimits()
        {
            var pPlt = GetSectionInfoByName(".plt");
            if (pPlt != null)
            {
                m_uPltMin = pPlt.Address;
                m_uPltMax = pPlt.Address + pPlt.Size; ;
            }
        }

        internal ElfSection GetSectionInfoByAddr(uint r_offset)
        {
            return
                (from sh in this.Sections
                 let addr = sh.Address != null ? sh.Address.ToLinear() : 0
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(uint idxString)
        {
            return (int)(Sections[Header.e_shstrndx].FileOffset + idxString);
        }

        public string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return imgLoader.ReadAsciiString(Sections[idx].FileOffset + offset);
        }

        public string GetSymbolName(int iSymbolSection, uint symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbolName(symSection, symbolNo);
        }

        public string GetSymbolName(ElfSection symSection, uint symbolNo)
        {
            var strSection = symSection.LinkedSection;
            uint offset = (uint)(symSection.FileOffset + symbolNo * symSection.EntrySize);
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt32(out offset);
            return GetStrPtr(strSection, offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                ProgramHeaders
                    .Where(p => IsLoadable(p.p_vaddr, p.p_type))
                    .Select(p => Tuple.Create(
                        Address.Ptr32(p.p_vaddr),
                        p.p_pmemsz)));

            foreach (var ph in ProgramHeaders)
            {
                Debug.Print("ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_vaddr, ph.p_type))
                    continue;
                var vaddr = Address.Ptr32(ph.p_vaddr);
                MemoryArea mem;
                segMap.TryGetLowerBound(vaddr, out mem);
                if (ph.p_filesz > 0)
                    Array.Copy(
                        rawImage,
                        (long)ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long)ph.p_filesz);
            }
            var segmentMap = new SegmentMap(addrPreferred);
            foreach (var section in Sections)
            {
                if (section.Name == null || section.Address == null)
                    continue;

                MemoryArea mem;
                if (segMap.TryGetLowerBound(section.Address, out mem) &&
                    section.Address < mem.EndAddress)
                {
                    AccessMode mode = AccessModeOf(section.Flags);
                    var seg = segmentMap.AddSegment(new ImageSegment(
                        section.Name,
                        section.Address,
                        mem, mode)
                    {
                        Size = (uint)section.Size
                    });
                    seg.Designer = CreateRenderer(section);
                } else
                {
                    //$TODO: warn
                }
            }
            segmentMap.DumpSections();
            return segmentMap;
        }

        public override int LoadProgramHeaderTable()
        {
            var rdr = imgLoader.CreateReader(Header.e_phoff);
            for (int i = 0; i < Header.e_phnum; ++i)
            {
                ProgramHeaders.Add(Elf32_PHdr.Load(rdr));
            }
            return ProgramHeaders.Count;
        }

        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header.e_shoff);
            for (uint i = 0; i < Header.e_shnum; ++i)
            {
                var shdr = Elf32_SHdr.Load(rdr);
                if (shdr == null)
                    break;
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = shdr.sh_addr != 0
                        ? Address.Ptr32(shdr.sh_addr)
                        : null,
                    FileOffset = shdr.sh_offset,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                Sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < Sections.Count; ++i)
            {
                var section = Sections[i];
                section.Name = ReadSectionName(inames[i]);

                ElfSection linkSection = null;
                ElfSection relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(links[i]);
                    relSection = GetSectionByIndex(infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
        }

        public override List<ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            //Debug.Print("Symbols");
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new List<ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf32_Sym.Load(rdr);
                //Debug.Print("  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                //    i,
                //    ReadAsciiString(stringtableSection.FileOffset + sym.st_name),
                //    (ElfSymbolType)(sym.st_info & 0xF),
                //    sym.st_shndx,
                //    GetSectionName(sym.st_shndx),
                //    sym.st_value,
                //    sym.st_size);
                symbols.Add(new ElfSymbol
                {
                    Name = ReadAsciiString(stringtableSection.FileOffset + sym.st_name),
                    Type = (ElfSymbolType)(sym.st_info & 0xF),
                    SectionIndex = sym.st_shndx,
                    Value = sym.st_value,
                    Size = sym.st_size,
                });
            }
            return symbols;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            Relocator.Relocate(program);

            var entryPoints = new List<ImageSymbol>();
            var symbols = new SortedList<Address, ImageSymbol>();
            foreach (var sym in Symbols.Values.SelectMany(seg => seg))
            {
                var imgSym = CreateImageSymbol(sym, Header.e_type);
                if (imgSym != null)
                {
                    symbols[imgSym.Address] = imgSym;
                }
            }

            var addrEntry = GetEntryPointAddress(addrLoad);
            if (addrEntry != null)
            {
                ImageSymbol entrySymbol;
                if (symbols.TryGetValue(addrEntry, out entrySymbol))
                {
                    entryPoints.Add(entrySymbol);
                }
                else
                {
                    var ep = new ImageSymbol(addrEntry)
                    {
                        ProcessorState = Architecture.CreateProcessorState()
                    };
                    entryPoints.Add(ep);
                }
            }
            return new RelocationResults(entryPoints, symbols);
        }

        /// <summary>
        /// The GOT table contains an array of pointers. Some of these
        /// pointers may be pointing to the symbols in the symbol table(s).
        /// </summary>
        public override void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            // Locate the GOT
            //$REVIEW: there doesn't seem to be a reliable way to get that
            // information.
            var got = program.SegmentMap.Segments.Values.FirstOrDefault(s => s.Name == ".got");
            if (got == null)
                return;

            var rdr = program.CreateImageReader(got.Address);
            while (rdr.Address < got.EndAddress)
            {
                var addrGot = rdr.Address;
                uint uAddrSym;
                if (!rdr.TryReadUInt32(out uAddrSym))
                    break;

                var addrSym = Address.Ptr32(uAddrSym);
                ImageSymbol symbol;
                if (symbols.TryGetValue(addrSym, out symbol))
                {
                    // This GOT entry is a known symbol!
                    if (symbol.Type == SymbolType.Procedure)
                    {
                        //$TODO: look up function signature.
                        var gotSym = new ImageSymbol(addrGot, symbol.Name + "_GOT",  new Pointer(new CodeType(), 4));
                        symbols[addrGot] = gotSym;
                        Debug.Print("Found GOT entry at {0}, changing symbol at {1}", gotSym, symbol);
                    }
                }
            }
        }
    }
}
