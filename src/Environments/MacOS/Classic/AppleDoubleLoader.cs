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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Reko.Environments.MacOS.Classic
{
    public class AppleDoubleLoader : ProgramImageLoader
    {
        private ResourceFork rsrcFork;
        private ByteMemoryArea mem;
        private SegmentMap segmentMap;

        public AppleDoubleLoader(IServiceProvider services, ImageLocation imageUri, byte[] rawImage)
            : base(services, imageUri, rawImage)
        {
            rsrcFork = null!;
            mem = null!;
            segmentMap = null!;
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x00100000); }
            set { throw new NotImplementedException(); }
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            addrLoad ??= PreferredBaseAddress;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var rdr = new BeImageReader(RawImage);
            var hdr = rdr.ReadStruct<Header>();
            var things = new Entry[hdr.cEntries];
            for (int i = 0; i < things.Length; ++i)
            {
                things[i] = rdr.ReadStruct<Entry>();
                Debug.Print("Type: {0,-16} Offset: {1:X8} Length: {2:X8}", things[i].type, things[i].offset, things[i].length);
            }

            var resourceEntry = things.FirstOrDefault(t => t.type == EntryType.ResourceFork);
            if (resourceEntry.type == EntryType.ResourceFork)
            {
                var bytes = new byte[resourceEntry.length];
                Array.Copy(RawImage, resourceEntry.offset, bytes, 0, bytes.Length);
                var arch = cfgSvc.GetArchitecture("m68k")!;
                var platform = (MacOSClassic) cfgSvc.GetEnvironment("macOs").Load(Services, arch);
                this.rsrcFork = new ResourceFork(platform, bytes);
                this.mem = new ByteMemoryArea(addrLoad, bytes);
                this.segmentMap = new SegmentMap(addrLoad);
                var program = new Program(new ProgramMemory(this.segmentMap), arch, platform);
                rsrcFork.Dump();
                rsrcFork.AddResourcesToImageMap(addrLoad, mem, program);
                return program;
            }
            else throw new BadImageFormatException("Unable to find a resource fork.");
        }


        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public uint magic;
            public uint version;

            public uint padding1;
            public uint padding2;
            public uint padding3;
            public uint padding4;

            public ushort cEntries;
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public EntryType type;
            public uint offset;
            public uint length;
        }

        public enum EntryType : uint
        {
            DataFork = 1,
            ResourceFork = 2,
            RealName = 3,
            Comment = 4,
            IconBw = 5,
            IconColor = 6,
            FileDatesInfo = 8,
            FinderInfo = 9,
            MacintoshFileInfo = 10,
            ProdosFileInfo = 11,
            MsdosFileInfo = 12,
            AfpShortName = 13,
            AfpFileInfo = 14,
            AfpDirectoryId = 15,
        }
    }
}
