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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    // https://github.com/yasm/yasm/blob/master/modules/dbgfmts/codeview/cv8.txt
    // https://github.com/microsoft/microsoft-pdb/blob/805655a28bd8198004be2ac27e6e0290121a5e89/include/cvinfo.h#L3724
    // https://learn.microsoft.com/en-us/windows/win32/debug/pe-format
    // https://www.delorie.com/djgpp/doc/coff/
    public class CoffLoader : ProgramImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(CoffLoader), "Trace loading of COFF files")
        {
            Level = TraceLevel.Warning
        };

        private const ushort IMAGE_FILE_RELOCS_STRIPPED = 0x0001;
        private const ushort IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002;
        private const ushort IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004;
        private const ushort IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008;
        private const ushort IMAGE_FILE_AGGRESSIVE_WS_TRIM = 0x0010;
        private const ushort IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020;
        private const ushort IMAGE_FILE_BYTES_REVERSED_LO = 0x0080;
        // Little endian: the least significant bit (LSB) precedes the most
        // significant bit (MSB) in memory. This flag is deprecated and should be zero.
        private const ushort IMAGE_FILE_32BIT_MACHINE = 0x0100;
        private const ushort IMAGE_FILE_DEBUG_STRIPPED = 0x0200;
        private const ushort IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400;
        private const ushort IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800;
        private const ushort IMAGE_FILE_SYSTEM = 0x1000;
        private const ushort IMAGE_FILE_DLL = 0x2000;
        private const ushort IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000;
        //The file should be run only on a uniprocessor machine.
        private const ushort IMAGE_FILE_BYTES_REVERSED_HI = 0x8000;

        private IProcessorArchitecture arch;
        private Address addrPreferred;
        private FileHeader header;
        private uint entry;
        private List<(string, CoffSectionHeader)> coffSections;
        private IEventListener eventListener;

        public CoffLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawBytes)
            : base(services, imageLocation, rawBytes)
        {
            this.eventListener = services.GetService<IEventListener>() ?? new NullEventListener();
            (this.arch, this.header, coffSections, entry) = LoadHeader();
            addrPreferred = default!;
        }

        public override Address PreferredBaseAddress
        {
            get { return this.addrPreferred; }
            set { throw new NotImplementedException(); }
        }

        public override Program LoadProgram(Address? addrLoad, string? platformOverride)
        {
            var platform = Platform.Load(Services, (string?)null, platformOverride, arch);
            Program program;
            if ((header.f_flags & IMAGE_FILE_EXECUTABLE_IMAGE) == 0)
            {
                program  = LinkObjectFile(platform);
            }
            else
            {
                program = LoadExecutable(platform);
            }
            var syms = ReadSymbols();
            foreach (var sym in syms)
            {
                program.ImageSymbols[sym.Address] = sym;
            }
            return program;
        }

        private Program LinkObjectFile(IPlatform platform)
        {
            var segs = ComputeSegmentLayout(this.coffSections);
            var segmentMap = LinkSegments(segs, this.coffSections);
            return new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform);
        }

        private Program LoadExecutable(IPlatform platform)
        {
            var segs = LoadImageSegments(this.coffSections);
            var program = new Program(
                new ByteProgramMemory(new SegmentMap(segs.ToArray())),
                arch,
                platform);
            if (this.entry != ~0u)
            {
                var ep = ImageSymbol.Procedure(arch, Address.Ptr32(this.entry), "_entry");
                program.EntryPoints.Add(ep.Address, ep);
            }
            return program;
        }

        private SegmentMap LinkSegments(List<ImageSegment> segs, List<(string, CoffSectionHeader)> coffSections)
        {
            return new SegmentMap(segs.ToArray());
        }

        private List<ImageSegment> ComputeSegmentLayout(List<(string, CoffSectionHeader)> coffSections)
        {
            var addr = Address.Ptr32(0x4000);
            var imgSegments = new List<ImageSegment>();
            foreach (var (n, hdr) in coffSections)
            {
                if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_LNK_REMOVE))
                    continue;
                addr = Align(addr, hdr.Characteristics);
                var mem = new ByteMemoryArea(addr, new byte[hdr.SizeOfRawData]);
                if (!hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_UNINITIALIZED_DATA))
                {
                    Array.Copy(RawImage, hdr.PointerToRawData, mem.Bytes, 0, mem.Bytes.Length);
                }
                var seg = new ImageSegment(n, mem, AccessFromSegmentHeader(hdr));
                imgSegments.Add(seg);
                addr = addr + mem.Bytes.Length;
            }
            return imgSegments;
        }

        private List<ImageSegment> LoadImageSegments(List<(string, CoffSectionHeader)> coffSections)
        {
            var imgSegments = new List<ImageSegment>();
            foreach (var (n, hdr) in coffSections)
            {
                var addr = Address.Ptr32(hdr.VirtualAddress);
                var mem = new ByteMemoryArea(addr, new byte[Math.Max(hdr.VirtualSize, hdr.SizeOfRawData)]);
                if (!hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_UNINITIALIZED_DATA))
                {
                    Array.Copy(RawImage, hdr.PointerToRawData, mem.Bytes, 0, Math.Min(hdr.SizeOfRawData, mem.Bytes.Length));
                }
                var seg = new ImageSegment(n, mem, AccessFromSegmentHeader(hdr));
                imgSegments.Add(seg);
            }
            return imgSegments;
        }

        private Address Align(Address address, CoffSectionCharacteristics characteristics)
        {
            int alignment;
            switch (characteristics & CoffSectionCharacteristics.IMAGE_SCN_ALIGN_MASK)
            {
            default:
                return address;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_1BYTES: return address;

            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_2BYTES: alignment = 2; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_4BYTES: alignment = 4; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_8BYTES: alignment = 8; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_16BYTES: alignment = 0x10; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_32BYTES: alignment = 0x20; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_64BYTES: alignment = 0x40; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_128BYTES: alignment = 0x80; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_256BYTES: alignment = 0x100; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_512BYTES: alignment = 0x200; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_1024BYTES: alignment = 0x400; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_2048BYTES: alignment = 0x800; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_4096BYTES: alignment = 0x1000; break;
            case CoffSectionCharacteristics.IMAGE_SCN_ALIGN_8192BYTES: alignment = 0x2000; break;
            }
            return address.Align(alignment);
        }

        private AccessMode AccessFromSegmentHeader(in CoffSectionHeader hdr)
        {
            AccessMode mode = 0;
            if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_CODE))
                return AccessMode.ReadExecute;
            if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_INITIALIZED_DATA) ||
                hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_UNINITIALIZED_DATA))
                return AccessMode.ReadWrite;
            if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_MEM_READ))
                mode |= AccessMode.Read;
            if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_MEM_WRITE))
                mode |= AccessMode.Write;
            if (hdr.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_MEM_EXECUTE))
                mode |= AccessMode.Execute;
            return mode;
        }

        private (IProcessorArchitecture, FileHeader, List<(string, CoffSectionHeader)>, uint entry) LoadHeader()
        {
            var rdr = new LeImageReader(RawImage, 0);
            var magic = rdr.ReadLeUInt16();
            var cfgSvc = Services.RequireService<IConfigurationService>();
            uint entry = ~0u;
            IProcessorArchitecture? arch = null;
            switch (magic)
            {
            case 0x014C: arch = cfgSvc.GetArchitecture("x86-protected-32"); break;
            case 0x8664: arch = cfgSvc.GetArchitecture("x86-protected-64"); break;
            case MagicNumbers.NS32ROMAGIC: arch = cfgSvc.GetArchitecture("ns32k"); break;
            case 0xAA64: arch = cfgSvc.GetArchitecture("arm64"); break;
            }
            if (arch is null)
                throw new NotSupportedException($"COFF loader for architecture {magic:X4} not supported yet.");
            // https://github.com/LADSoft/OrangeC/issues/252
            var fileHeader = new FileHeader
            {
                f_magic = magic,
                f_nscns = rdr.ReadUInt16(),
                f_timdat = rdr.ReadUInt32(),
                f_symptr = rdr.ReadUInt32(),
                f_nsyms = rdr.ReadUInt32(),
                f_opthdr = rdr.ReadUInt16(),
                f_flags = rdr.ReadUInt16(),
            };
            // Section header follow immediately after the optional file header.
            if (fileHeader.f_opthdr > 0)
            {
                if (magic == MagicNumbers.NS32ROMAGIC)
                {
                    Ns32kOptionalHeader ns32kopt = new();
                    ns32kopt.magic = rdr.ReadInt16();
                    ns32kopt.vstamp = rdr.ReadInt16();
                    ns32kopt.tsize = rdr.ReadInt32();
                    ns32kopt.dsize = rdr.ReadInt32();
                    ns32kopt.bsize = rdr.ReadInt32();
                    ns32kopt.msize = rdr.ReadInt32();
                    ns32kopt.mod_start = rdr.ReadInt32();
                    ns32kopt.entry = rdr.ReadUInt32();
                    ns32kopt.text_start = rdr.ReadUInt32();
                    ns32kopt.data_start = rdr.ReadUInt32();
                    ns32kopt.entry_mod = rdr.ReadInt16();
                    ns32kopt.flags = rdr.ReadUInt16();
                    entry = ns32kopt.entry;
                }
                else
                {
                    rdr.Offset += fileHeader.f_opthdr;
                }
            }
            var coffSections = new List<(string,CoffSectionHeader)>();
            trace.Verbose("## COFF Sections");
            for (int i = 0; i < fileHeader.f_nscns; ++i)
            {
                var hdr = rdr.ReadStruct<CoffSectionHeader>();
                if (fileHeader.f_magic == MagicNumbers.NS32ROMAGIC) // ns32k
                {
                    rdr.ReadStruct<Ns32kSectionHeader>();
                }
                string name;
                unsafe
                {
                    name = Encoding.ASCII.GetString(hdr.Name, 8).TrimEnd('\0');
                }
                trace.Verbose($"  {i,4} {name,-8} {hdr.PointerToRawData:X8} {hdr.SizeOfRawData:X8} r:{hdr.PointerToRelocations:X8} ({hdr.NumberOfRelocations:X8})");
                coffSections.Add((name,hdr));
                if (hdr.NumberOfRelocations != 0 && hdr.PointerToRelocations != 0)
                {
                    LoadSectionRelocations(name, hdr);
                }
            }
            return (arch, fileHeader, coffSections, entry);
        }

        private void LoadSectionRelocations(string name, in CoffSectionHeader hdr)
        {
            trace.Verbose($"## COFF relocations for section {name}");
            var rdr = new LeImageReader(RawImage, hdr.PointerToRelocations);
            for (int i = 0; i < hdr.NumberOfRelocations; ++i)
            {
                var reloc = rdr.ReadStruct<CoffRelocation>();
                trace.Verbose($"  {reloc.VirtualAddress:X8} {reloc.SymbolTableIndex,8} {reloc.Type:X4}");
            }
        }

        private List<ImageSymbol> ReadSymbols()
        {
            List<ImageSymbol> result = [];
            List<CoffSymbol> syms = [];
            if (header.f_nsyms == 0)
                return result;
            var rdr = new ByteImageReader(RawImage, this.header.f_symptr);
            for (int i = 0; i < header.f_nsyms; ++i)
            {
                syms.Add(rdr.ReadStruct<CoffSymbol>());
            }
            // rdr is now positioned at the start of the string table. 
            long strtabOffset = rdr.Offset;
            // Generate symbol names.
            trace.Verbose($"{"COFF symbols",-18} {"sec",-3} {"value",-8} {"type",-4} {"cls",-4} {"aux",-4}");
            for (int i = 0; i < syms.Count; ++i)
            {
                var sym = syms[i];
                string name;
                if (sym.e_zeroes != 0) unsafe
                {
                    name = Encoding.ASCII.GetString(sym.e_name, 8).TrimEnd('\0');
                }
                else
                {
                    var ab = new List<byte>();
                    rdr.Offset = strtabOffset + sym.e_offset;
                    while (rdr.TryReadByte(out byte b) && b != 0)
                    {
                        ab.Add(b);
                    }
                    name = Encoding.ASCII.GetString(ab.ToArray());
                }
                trace.Verbose($"  {name,-16} {sym.e_scnum,3} {sym.e_value:X8} {sym.e_type:X4} {sym.e_sclass:X4} {sym.e_numaux:X2}");
                if (sym.e_numaux != 0)
                {
                    LoadAuxSymbolRecords(syms, i, sym);
                    i += sym.e_numaux;
                }
                AddSymbolToResult(sym, name, result);
            }
            return result;
        }

        private void AddSymbolToResult(in CoffSymbol sym, string name, List<ImageSymbol> result)
        {
            if (sym.e_sclass == SymbolStorageClass.IMAGE_SYM_CLASS_EXTERNAL &&
                (sym.e_scnum > 0 && sym.e_scnum <= coffSections.Count))
            {
                ImageSymbol imgSym;
                var (sectName, sect) = coffSections[sym.e_scnum - 1];
                if (sect.Characteristics.HasFlag(CoffSectionCharacteristics.IMAGE_SCN_CNT_CODE))
                {
                    imgSym = ImageSymbol.Procedure(
                        arch,
                        Address.Ptr32(sym.e_value),
                        name);
                }
                else
                {
                    imgSym = ImageSymbol.Create(
                        SymbolType.Unknown,
                        arch,
                        Address.Ptr32(sym.e_value),
                        name: name);
                }
                result.Add(imgSym);
            }
        }

        private bool LoadAuxSymbolRecords(List<CoffSymbol> syms, int i, in CoffSymbol sym)
        {
            switch (sym.e_sclass)
            {
            case SymbolStorageClass.IMAGE_SYM_CLASS_STATIC:
                var a = syms[i + 1];
                ref var auxSect = ref Unsafe.As<CoffSymbol, AuxSectionDefinition>(ref a);
                trace.Verbose($"    {auxSect.Length:X8} {auxSect.NumberOfRelocations} {auxSect.NumberOfLinenumbers,-4} {auxSect.CheckSum:X8} {auxSect.Number,-4} {auxSect.Selection:X2}; ");
                return true;
            case SymbolStorageClass.IMAGE_SYM_CLASS_FILE:
                a = syms[i + 1];
                var abFilename = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(ref a), 1));
                string filename = Encoding.ASCII.GetString(abFilename).TrimEnd('\0');
                trace.Verbose($"    {filename}");
                return true;
            default:
                this.eventListener.Warn($"Unknown symbol class {sym.e_sclass}.");
                return false;
            }
        }
    }
}
