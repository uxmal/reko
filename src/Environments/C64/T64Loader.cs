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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Archives;
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.C64
{
    // https://ist.uwaterloo.ca/~schepers/formats/T64.TXT
    public class T64Loader : ImageLoader
    {
        private Program program;

        public T64Loader(IServiceProvider services, string filename, byte[] bytes)
            : base(services, filename, bytes)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr16(2048); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            List<ArchiveDirectoryEntry> entries = LoadTapeDirectory();
            IArchiveBrowserService abSvc = Services.GetService<IArchiveBrowserService>();
            if (abSvc != null)
            {
                if (abSvc.UserSelectFileFromArchive(entries) is T64FileEntry selectedFile)
                {
                    this.program = LoadImage(addrLoad, selectedFile);
                    return program;
                }
            }
            var arch = new Mos6502ProcessorArchitecture("mos6502");
            var mem = new MemoryArea(Address.Ptr16(0), RawImage);
            var segmentMap = new SegmentMap(mem.BaseAddress);
            segmentMap.AddSegment(mem, "code", AccessMode.ReadWriteExecute);
            return new Program
            {
                SegmentMap = segmentMap,
                Architecture = arch,
                Platform = new DefaultPlatform(Services, arch)
            };
        }

        private Program LoadImage(Address addrLoad, T64FileEntry selectedFile)
        {
            switch (selectedFile.FileType)
            {
            default:
                throw new NotImplementedException();
            case FileType.PRG:
                return LoadPrg(selectedFile);
            }
        }

        private Program LoadPrg(T64FileEntry selectedFile)
        {
            var image = new MemoryArea(
                Address.Ptr16(selectedFile.LoadAddress),
                selectedFile.GetBytes());
            var rdr = new C64BasicReader(image, 0x0801);
            var lines = rdr.ToSortedList(line => (ushort) line.Address.ToLinear(), line => line);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch6502 = cfgSvc.GetArchitecture("m6502");
            var arch = new C64Basic(lines);
            var platform = cfgSvc.GetEnvironment("c64").Load(Services, arch);
            var segMap = platform.CreateAbsoluteMemoryMap();
            segMap.AddSegment(image, "code", AccessMode.ReadWriteExecute);
            var program = new Program(segMap, arch, platform);
            program.Architectures.Add(arch6502.Name, arch6502);
            var addrBasic = Address.Ptr16(lines.Keys[0]);
            var sym = ImageSymbol.Procedure(arch, addrBasic, state: arch.CreateProcessorState());
            program.EntryPoints.Add(sym.Address, sym);
            return program;
        }

        private List<ArchiveDirectoryEntry> LoadTapeDirectory()
        {
            var rdr = new LeImageReader(RawImage);
            var sig = Encoding.ASCII.GetString(rdr.ReadBytes(0x20));
            if (!sig.StartsWith("C64"))
                throw new BadImageFormatException("Expected T64 file to begin with C64 signature.");

            if (!rdr.TryReadLeUInt16(out ushort version) ||
                (version != 0x0100 && version != 0x0101))
                throw new BadImageFormatException("Unsupported T64 version.");

            if (!rdr.TryReadLeUInt16(out ushort cTotalEntries) ||
                !rdr.TryReadLeUInt16(out ushort cEntriesInUse) ||
                !rdr.TryReadLeUInt16(out ushort padding))
            {
                throw new BadImageFormatException("Unable to read T64 header.");
            }
            var containerName = Encoding.ASCII.GetString(rdr.ReadBytes(0x18)).TrimEnd();

            // Now read the directory entries.

            var entries = new List<ArchiveDirectoryEntry>();
            for (int i = 0; i < cTotalEntries; ++i)
            {
                var entry = ReadDirectoryEntry(rdr);
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }
            return entries;
        }

        private T64FileEntry ReadDirectoryEntry(LeImageReader rdr)
        {
            if (!rdr.TryReadByte(out var c64Type) ||
                !rdr.TryReadByte(out var fileType) ||
                !rdr.TryReadLeUInt16(out var uAddrStart) ||
                !rdr.TryReadLeUInt16(out var uAddrEnd) ||
                !rdr.TryReadLeUInt16(out var dummy) ||
                !rdr.TryReadLeUInt32(out var fileOffset) ||
                !rdr.TryReadLeUInt32(out var dummy32))
                throw new BadImageFormatException();
            var name = Encoding.ASCII.GetString(rdr.ReadBytes(0x10));

            var bytes = new byte[uAddrEnd - uAddrStart];
            Array.Copy(RawImage, fileOffset, bytes, 0, bytes.Length);
            return new T64FileEntry(name, (FileType) (fileType & 7), uAddrStart, bytes);

        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                 new List<ImageSymbol>(),
                 new SortedList<Address, ImageSymbol>());
        }

        public class T64Header
        {
            /*  
             The first 32 bytes ($000000-00001F) represent the signature of the  file,
telling us it is a tape container for C64S. Note that it is padded with $00
to make the signature 32 bytes long.

  It is important that the string "C64" be at the  beginning  of  the  file
because it is the string which is common enough to be used to identify  the
file type. There are several variations of  this  string  like  "C64S  tape
file" or "C64 tape image file". The string is stored in ASCII.
*/
          //  The next 32 bytes contain all the info about the directory  size,  number
          //of used entries, tape container name, tape version#, etc.


//Bytes:$20-21: Tape version number of either $0100 or $0101. I am  not sure
//              what differences exist between versions.
//       22-23: Maximum number  of entries  in the directory, stored  in
//              low/high byte order (in this case $0190 = 400 total)
//       24-25: Total number of used entries, once again  in  low/high byte.
//              Used = $0005 = 5 entries.
//       26-27: Not used
//       28-3F: Tape container name, 24 characters, padded with $20 (space)
        }

        /*Bytes   $40: C64s filetype
                  0 = free (usually)
                  1 = Normal tape file
                  3 = Memory Snapshot, v .9, uncompressed
              2-255 = Reserved (for memory snapshots)
         41: 1541 file type (0x82 for PRG, 0x81 for  SEQ,  etc).  You  will
             find it can vary  between  0x01,  0x44,  and  the  normal  D64
             values. In reality any value that is not a $00 is  seen  as  a
             PRG file. When this value is a $00 (and the previous  byte  at
             $40 is >1), then the file is a special T64 "FRZ" (frozen) C64s
             session snapshot.
      42-43: Start address (or Load address). This is the first  two  bytes
             of the C64 file which is usually the load  address  (typically
             $01 $08). If the file is a snapshot, the address will be 0.
      44-45: End address (actual end address in memory,  if  the  file  was
             loaded into a C64). If  the  file  is  a  snapshot,  then  the
             address will be a 0.
      46-47: Not used
      48-4B: Offset into the conatiner file (from the beginning)  of  where
             the C64 file starts (stored as low/high byte)
      4C-4F: Not used
      50-5F: C64 filename (in PETASCII, padded with $20, not $A0)

  Typically, an empty entry will have no contents at all, and not just have
the first byte set to $00. If you only set the C64s filetype  byte  to  $00
and then use the file in C64S, you will see  the  entry  is  still  in  the
directory.
*/
        public class T64FileEntry : ArchivedFile
        {
            public T64FileEntry(string name, FileType fileType, ushort uAddrLoad, byte[] bytes)
            {
                this.Name = name;
                this.FileType = fileType;
                this.LoadAddress = uAddrLoad;
                this.bytes = bytes;
            }

            public string Name { get; }
            public FileType FileType { get; }
            public ushort LoadAddress { get; }

            private byte[] bytes;

            public byte[] GetBytes()
            {
                return bytes;
            }

        }
    }
}
