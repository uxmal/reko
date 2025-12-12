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

using K4os.Compression.LZ4;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.ImageLoaders.Elf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Environments.Switch
{
    // https://switchbrew.org/wiki/NSO
    public class NSO0Loader : ProgramImageLoader
    {
        public NSO0Loader(IServiceProvider services, ImageLocation imageUri, byte[] rawBytes)
            : base(services, imageUri, rawBytes)
        {
        }
        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x00010000);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Program LoadProgram(Address? addrLoad, string? sPlatformOverride)
        {
            var @in = new LeImageReader(RawImage);
            NSOHeader nsoHeader = @in.ReadStruct<NSOHeader>();
            byte[] dcmpText = new byte[nsoHeader.dcmpSize_text];
            LZ4Codec.Decode(
                RawImage, (int)nsoHeader.fileOffset_text, (int)nsoHeader.cmpSize_text,
                dcmpText, 0, (int) nsoHeader.dcmpSize_text);
 
            byte[] dcmpRodata = new byte[nsoHeader.dcmpSize_rodata];
            LZ4Codec.Decode(
                RawImage, (int) nsoHeader.fileOffset_rodata, (int) nsoHeader.cmpSize_rodata,
                dcmpRodata, 0, (int) nsoHeader.dcmpSize_rodata);

            byte[] dcmpDataData = new byte[nsoHeader.dcmpSize_data];
            LZ4Codec.Decode(
                RawImage, (int) nsoHeader.fileOffset_data, (int) nsoHeader.cmpSize_data,
                dcmpDataData, 0, (int) nsoHeader.dcmpSize_data);

            var segText = MakeSeg(".text", nsoHeader.memoryOffset_text, dcmpText, AccessMode.ReadExecute);
            var segRo = MakeSeg(".rodata", nsoHeader.memoryOffset_rodata, dcmpRodata, AccessMode.Read);
            var segData = MakeSeg(".data", nsoHeader.memoryOffset_data, dcmpDataData, AccessMode.ReadWrite);
            
            var segmap = new SegmentMap(segText, segRo, segData);
            ReadMod0(dcmpText, segmap);

            var cfg = Services.RequireService<IConfigurationService>();
            var arch = cfg.GetArchitecture("arm");
            if (arch is null)
                throw new InvalidOperationException("Unable to load arm-thumb architecture.");
            var platform = Platform.Load(Services, "switch", sPlatformOverride, arch);
            return new Program(new ByteProgramMemory(segmap), arch, platform);
        }

        private ImageSegment MakeSeg(string segname, uint uAddr, byte[] bytes, AccessMode mode)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(uAddr), bytes);
            return new ImageSegment(segname, mem, mode);
        }

        private void ReadMod0(byte[] dcmpTextData, SegmentMap map)
        {
            var f = new LeImageReader(dcmpTextData);
            var mod0 = f.ReadStruct<Mod0>();
            if (!map.TryFindSegment(mod0.DynamicOffset, out var dynseg))
                return;

            var offset = mod0.DynamicOffset - dynseg.MemoryArea.BaseAddress.ToLinear();
            offset += mod0.MagicOffset;
            var rdr = dynseg.MemoryArea.CreateLeReader((int) offset);
            var elfHdr = new ElfHeader
            {
                Machine = ElfMachine.EM_ARM,
            };
            var bin = new ElfBinaryImage(ImageLocation, elfHdr, EndianServices.Little);
            var elfLoader = new ElfLoader32(Services, bin, RawImage);
            var (deps, entries) = elfLoader.LoadDynamicSegment(rdr);

            var dynEntries = entries.ToDictionary(e => e.Tag, e => e.UValue);
            var syms = LoadSymbols(map, dynEntries);
            LoadRelocations(map, dynEntries, syms);
        }

        private void LoadRelocations(SegmentMap map, Dictionary<int, ulong> dynEntries, List<ElfSymbol> syms)
        {
            Dump(dynEntries);
        }

        private void Dump(Dictionary<int, ulong> entries)
        {
            foreach (var entry in entries.OrderBy(e => e.Key))
            {
                var sTag = ElfDynamicEntry.TagInfos!.Get(entry.Key)?.Name ?? $"{entry.Key:X8}";

                Console.WriteLine("  {0,-20} {1:X20}", sTag, entry.Value);
            }
        }

        private List<ElfSymbol> LoadSymbols(SegmentMap map, Dictionary<int, ulong> dynEntries)
        {
            List<ElfSymbol> result = new List<ElfSymbol>();
            if (!dynEntries.TryGetValue(ElfDynamicEntry.DT_STRTAB, out var uAddrStrab) ||
                !dynEntries.TryGetValue(ElfDynamicEntry.DT_SYMTAB, out var uAddrSymtab) ||
                !dynEntries.TryGetValue(ElfDynamicEntry.DT_SYMENT, out var cbSymbol))
                return result;

            if (uAddrStrab == 0 || uAddrSymtab == 0 || cbSymbol == 0)
                return result;

            var addrSymtab = Address.Ptr32((uint) uAddrSymtab);
            var addrStrtab = Address.Ptr32((uint) uAddrStrab);
            if (!map.TryFindSegment(addrSymtab, out var symSeg))
                return result;
            var symrdr = symSeg.MemoryArea.CreateLeReader(addrSymtab);

            while (symrdr.Address < addrStrtab)
            {
                var symbol = ReadSymbol(symrdr, addrStrtab, map);
                if (symbol is null)
                    break;
                result.Add(symbol);
            }
            return result;
        }

        private ElfSymbol? ReadSymbol(EndianImageReader rdr, Address addrStrtab, SegmentMap map)
        {
            if (!Elf32_Sym.TryLoad(rdr, out var sym))
                return null;
            var name = ReadAsciiString(map, addrStrtab + sym.st_name);
            return new ElfSymbol(name)
            {
                Type = (ElfSymbolType) (sym.st_info & 0xF),
                Bind = (ElfSymbolBinding) (sym.st_info >> 4),
                SectionIndex = sym.st_shndx,
                Value = sym.st_value,
                Size = sym.st_size,
            };
        }

        private string ReadAsciiString(SegmentMap map, Address address)
        {
            if (!map.TryFindSegment(address, out var seg))
                return "???";
            var mem = (ByteMemoryArea) seg.MemoryArea;
            var offset = (int) (address - mem.BaseAddress);
            var bytes = mem.Bytes;
            int i = offset;
            for (; i < bytes.Length && bytes[i] != 0; ++i)
                ;
            return Encoding.UTF8.GetString(bytes, offset, i - offset);
        }
    }

    [Endian(Endianness.LittleEndian)]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct NSOHeader
    {
        public uint magic;
        public uint version;
        public uint pada;
        public uint flags;
        public uint fileOffset_text;
        public uint memoryOffset_text;
        public uint dcmpSize_text;
        public uint modOffset;
        public uint fileOffset_rodata;
        public uint memoryOffset_rodata;
        public uint dcmpSize_rodata;
        public uint modSize;
        public uint fileOffset_data;
        public uint memoryOffset_data;
        public uint dcmpSize_data;
        public uint bssSize;
        public fixed byte note[0x20];
        public uint cmpSize_text;
        public uint cmpSize_rodata;
        public uint cmpSize_data;
        public fixed byte padb[0x1C];
        public uint rdOffset_api;
        public uint rdSize_api;
        public uint rdOffset_dynstr;
        public uint rdSize_dynstr;
        public uint rdOffset_dynsym;
        public uint rdSize_dynsym;
    }

    [Endian(Endianness.LittleEndian)]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Mod0
    {
        public uint Reserved;
        public uint MagicOffset;        // (always 8, so it works when MOD is at image_base + 0
        public uint Magic;              // "MOD0"
        public uint DynamicOffset;      // .dynamic offset
        public uint BssStartOffset;     // .bss start offset
        public uint BssEndOffset;       // .bss end offset
        public uint EhFrameHdrStart;    // .eh_frame_hdr start offset
        public uint EhFrameHdrEnd;      // .eh_frame_hdr end offset
        public uint ModuleOffset;       // Offset to runtime-generated module object (typically equal to .bss base) */
    }
}
