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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    // https://ist.uwaterloo.ca/~schepers/formats/T64.TXT
    public class T64Loader : ImageLoader
    {
        private static readonly Address PreferredBaseAddress = Address.Ptr16(2048);

        public T64Loader(IServiceProvider services, ImageLocation imageUri, byte[] bytes)
            : base(services, imageUri, bytes)
        {
        }


        public override ILoadedImage Load(Address? addrLoad)
        {
            addrLoad ??= PreferredBaseAddress;
            return LoadTapeDirectory();
        }

        private IArchive LoadTapeDirectory()
        {
            var rdr = new ByteImageReader(RawImage);
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
                if (entry is not null)
                {
                    entries.Add(entry);
                }
            }
            return new T64Archive(base.ImageLocation, entries);
        }

        private T64FileEntry ReadDirectoryEntry(ImageReader rdr)
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
            var imgLocation = this.ImageLocation.AppendFragment(name);
            return new T64FileEntry(imgLocation, name, (FileType) (fileType & 7), uAddrStart, bytes);
        }


        public class T64Archive : IArchive
        {
            public T64Archive(ImageLocation archiveUri, List<ArchiveDirectoryEntry> entries)
            {
                this.Location = archiveUri;
                this.RootEntries = entries;
            }

            /// <summary>
            /// Retrieve the file whose name is <paramref name="path"/>.
            /// </summary>
            /// <remarks>
            /// T64 tape images have no tree structure, so the path has to be the actual
            /// file name.</remarks>
            /// <param name="path">Name of the file.</param>
            /// <returns></returns>
            public ArchiveDirectoryEntry? this[string path]
            {
                get => RootEntries.Where(e => e.Name == path).FirstOrDefault();
            }

            public ImageLocation Location { get; }

            public List<ArchiveDirectoryEntry> RootEntries { get; }

            public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
                => visitor.VisitArchive(this, context);

            public string GetRootPath(ArchiveDirectoryEntry? entry)
            {
                if (entry is null)
                    return "";
                if (entry is not T64FileEntry file)
                    throw new ArgumentException(string.Format(
                        "Invalid entry type {0} for {1}.",
                        entry.GetType().FullName,
                        nameof(T64Archive)));
                return file.Name;
            }
        }

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
            public T64FileEntry(ImageLocation location, string name, FileType fileType, ushort uAddrLoad, byte[] bytes)
            {
                this.ImageLocation = location;
                this.Name = name;
                this.FileType = fileType;
                this.LoadAddress = uAddrLoad;
                this.bytes = bytes;
            }

            public long Length => bytes.LongLength;

            public string Name { get; }
            public FileType FileType { get; }
            public ImageLocation ImageLocation { get; }
            public ushort LoadAddress { get; }

            public ArchiveDirectoryEntry? Parent => null;

            private readonly byte[] bytes;

            public byte[] GetBytes()
            {
                return bytes;
            }

            public ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred)
            {
                switch (this.FileType)
                {
                default:
                    throw new NotImplementedException();
                case FileType.PRG:
                    return LoadPrg(services);
                }
            }

            private Program LoadPrg(IServiceProvider services)
            {
                // Many of these programs use all of the 
                // C64 memory, so be generous.
                var image = new ByteMemoryArea(Address.Ptr16(0x800), new byte[0xF800 - 0x800]);
                var src = this.GetBytes();
                Array.Copy(src, 0, image.Bytes, (int) (this.LoadAddress - (ushort)image.BaseAddress.ToLinear()), src.Length);

                var rdr = new C64BasicReader(image, 0x0801);
                var lines = rdr.ToSortedList(line => (ushort) line.LineNumber, line => line);
                var cfgSvc = services.RequireService<IConfigurationService>();
                var arch6502 = cfgSvc.GetArchitecture("m6502")!;
                var arch = new C64Basic(services, lines);
                var platform = cfgSvc.GetEnvironment("c64").Load(services, arch);
                var segMap = platform.CreateAbsoluteMemoryMap()!;
                segMap.AddSegment(image, "code", AccessMode.ReadWriteExecute);
                var program = new Program(new ByteProgramMemory(segMap), arch, platform);
                program.Location = ImageLocation;
                program.Name = Name;

                program.Architectures.Add(arch6502.Name, arch6502);
                var addrBasic = Address.Ptr16(lines.Keys[0]);
                var sym = ImageSymbol.Procedure(arch, addrBasic, state: arch.CreateProcessorState());
                program.EntryPoints.Add(sym.Address, sym);
                return program;
            }
        }
    }
}
