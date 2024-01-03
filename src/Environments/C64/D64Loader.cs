#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Loads sub-images from a Commodore C64 disk image.
    /// </summary>
    public class D64Loader : ImageLoader
    {
        // http://petlibrary.tripod.com/D64.HTM
        // PRG files have 2 bytes load address and data.
        // https://hkn.eecs.berkeley.edu/~mcmartin/ophis/manual/x51.html
        //
        
        /*
 Track => offset map
 Track #Sect #SectorsIn D64 Offset   Track #Sect #SectorsIn D64 Offset
  ----- ----- ---------- ----------   ----- ----- ---------- ----------
    1     21       0       $00000      21     19     414       $19E00
    2     21      21       $01500      22     19     433       $1B100
    3     21      42       $02A00      23     19     452       $1C400
    4     21      63       $03F00      24     19     471       $1D700
    5     21      84       $05400      25     18     490       $1EA00
    6     21     105       $06900      26     18     508       $1FC00
    7     21     126       $07E00      27     18     526       $20E00
    8     21     147       $09300      28     18     544       $22000
    9     21     168       $0A800      29     18     562       $23200
   10     21     189       $0BD00      30     18     580       $24400
   11     21     210       $0D200      31     17     598       $25600
   12     21     231       $0E700      32     17     615       $26700
   13     21     252       $0FC00      33     17     632       $27800
   14     21     273       $11100      34     17     649       $28900
   15     21     294       $12600      35     17     666       $29A00
   16     21     315       $13B00      36*    17     683       $2AB00
   17     21     336       $15000      37*    17     700       $2BC00
   18     19     357       $16500      38*    17     717       $2CD00
   19     19     376       $17800      39*    17     734       $2DE00
   20     19     395       $18B00      40*    17     751       $2EF00
*/
        private static readonly int[] sectorCount = new int[] {
            -1,
            21, 21, 21, 21, 21,  21, 21, 21, 21, 21, 
            21, 21, 21, 21, 21,  21, 21, 19, 19, 19,
            19, 19, 19, 19, 18,  18, 18, 18, 18, 18, 
            17, 17, 17, 17, 17,  17, 17, 17, 17, 17, 
        };

        private static readonly int [] sectorsIn = new int[] {
            -1,
            0  ,21 ,42 ,63 ,84 , 105,126,147,168,189,
            210,231,252,273,294, 315,336,357,376,395,
            414,433,452,471,490, 508,526,544,562,580,
            598,615,632,649,666, 683,700,717,734,751,
        };

        private static readonly int[] trackOffset = new int[] {
            -1,
            0x00000,0x01500,0x02A00,0x03F00,0x05400,  0x06900,0x07E00,0x09300,0x0A800,0x0BD00,
            0x0D200,0x0E700,0x0FC00,0x11100,0x12600,  0x13B00,0x15000,0x16500,0x17800,0x18B00,
            0x19E00,0x1B100,0x1C400,0x1D700,0x1EA00,  0x1FC00,0x20E00,0x22000,0x23200,0x24400,
            0x25600,0x26700,0x27800,0x28900,0x29A00,  0x2AB00,0x2BC00,0x2CD00,0x2DE00,0x2EF00,
        };

        public static readonly Address PreferredBaseAddress = Address.Ptr16(2048);


        public D64Loader(IServiceProvider services, ImageLocation imageUri, byte[] rawImage)
            : base(services, imageUri, rawImage)
        {
        }


        /*
        public class C64ImageHeader : ImageHeader
        {
            private readonly D64FileEntry dirEntry;

            public C64ImageHeader(D64FileEntry dirEntry)
            {
                this.dirEntry = dirEntry;
                this.PreferredBaseAddress = Address.Ptr16(2048);
            }

            public override Address PreferredBaseAddress { get; set; }
        }*/

        public override ILoadedImage Load(Address? addrLoad)
        {
            var archive = LoadDiskDirectory();
            return archive;
        }

        public IArchive LoadDiskDirectory()
        {
            var entries = new List<ArchiveDirectoryEntry>();
            var rdr = new ByteImageReader(RawImage, (uint)SectorOffset(18, 0));
            byte track = rdr.ReadByte();
            var archive = new D64Archive(Services, ImageLocation, entries);
            if (track != 0)
            {
                byte sector = rdr.ReadByte();
                rdr.Offset = (uint) D64Loader.SectorOffset(track, sector);
                while (ReadDirectorySector(rdr, archive, entries))
                    ;
            }
            return archive;
        }

        public bool ReadDirectorySector(ImageReader rdr, D64Archive archive, List<ArchiveDirectoryEntry> entries)
        {
            byte nextDirTrack = 0;
            byte nextDirSector = 0;
            for (int i = 0; i < 8; ++i)
            {
                if (i == 0)
                {
                    nextDirTrack = rdr.ReadByte();
                    nextDirSector = rdr.ReadByte();
                }
                else
                {
                    rdr.Seek(2);
                }
                var fileType = (FileType) rdr.ReadByte();
                var fileTrack = rdr.ReadByte();
                var fileSector = rdr.ReadByte();
                var sName = Encoding.ASCII.GetString(rdr.ReadBytes(16))
                    .TrimEnd((char) 0xA0);
                var relTrack = rdr.ReadByte();
                var relSector = rdr.ReadByte();
                var rel = rdr.ReadByte();
                rdr.Seek(6);
                var sectorSize = rdr.ReadLeInt16();
                if ((fileType & FileType.FileTypeMask) != FileType.DEL)
                {
                    entries.Add(new D64FileEntry(
                        archive,
                        sName,
                        RawImage, 
                        SectorOffset(fileTrack, fileSector), 
                        fileType));
                }
            }
            if (nextDirTrack != 0)
            {
                rdr.Offset = (uint) SectorOffset(nextDirTrack, nextDirSector);
                return true;
            }
            else
            {
                return false;
            }
        }

        public class D64FileEntry : ArchivedFile
        {
            private readonly D64Archive archive;
            private readonly byte[] image;
            private readonly int offset;

            public D64FileEntry(D64Archive archive, string name, byte[] diskImage, int offset, FileType fileType)
            {
                this.archive = archive;
                this.Name = name;
                this.image = diskImage;
                this.offset = offset;
                this.FileType = fileType;
            }

            public string Name { get; private set; }

            public FileType FileType { get; private set; }

            public long Length => throw new NotImplementedException();

            public ArchiveDirectoryEntry? Parent => null;

            public byte[] GetBytes()
            {
                byte[] data;
                var stm = new MemoryStream();
                var rdr = new ByteImageReader(image, (uint) offset);
                byte trackNext = rdr.ReadByte();
                while (trackNext != 0)
                {
                    byte sectorNext = rdr.ReadByte();
                    data = rdr.ReadBytes(0xFE);
                    stm.Write(data, 0, data.Length);

                    rdr.Offset = (uint) SectorOffset(trackNext, sectorNext);
                    trackNext = rdr.ReadByte();
                }
                byte lastUsed = rdr.ReadByte();
                data = rdr.ReadBytes(lastUsed - 2);
                stm.Write(data, 0, data.Length);
                return stm.ToArray();
            }

            public ILoadedImage LoadImage(IServiceProvider Services, Address? addrPreferred)
            {
                byte[] imageBytes = this.GetBytes();
                return (this.FileType & FileType.FileTypeMask) switch
                {
                    FileType.PRG => LoadPrg(Services, imageBytes),
                    FileType.SEQ => LoadSeq(Services, addrPreferred ?? Address.Ptr16(0x0800), imageBytes),
                    _ => throw new NotImplementedException(),
                };
            }

            /// <summary>
            /// Load a Basic PRG.
            /// </summary>
            /// <param name="imageBytes"></param>
            /// <returns></returns>
            private Program LoadPrg(IServiceProvider services, byte[] imageBytes)
            {
                var prgUri = archive.Location.AppendFragment(this.Name);
                var prgLoader = new PrgLoader(services, prgUri, imageBytes);
                var program = prgLoader.LoadProgram(null);
                program.Name = this.Name;
                program.Location = prgUri;
                return program;
            }

            public Program LoadSeq(IServiceProvider services, Address addrPreferred, byte[] imageBytes)
            {
                var seqUri = archive.Location.AppendFragment(this.Name);
                var mem = new ByteMemoryArea(addrPreferred, imageBytes);
                var arch = new Mos6502Architecture(services, "mos6502", new Dictionary<string, object>());
                var program = new Program(
                    new SegmentMap(
                        mem.BaseAddress,
                        new ImageSegment("c64", mem, AccessMode.ReadWriteExecute)),
                    arch,
                    new DefaultPlatform(services, arch))
                {
                    Name = this.Name,
                    Location = archive.Location.AppendFragment(this.Name)
                };
                return program;
            }
        }

        public static int SectorOffset(byte track, byte sector)
        {
            return trackOffset[track] + sector * 256;
        }
    }
}
