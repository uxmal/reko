#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe
{
    public class PharLapMpLoader : ProgramImageLoader
    {
        public PharLapMpLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
            : base(services, imageLocation, imgRaw)
        {
        }

        public override Address PreferredBaseAddress 
        {
            get => Address.Ptr32(0x0010_0000);
            set => throw new NotImplementedException();
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            addrLoad ??= PreferredBaseAddress;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("x86-protected-32")!;
            var platform = cfgSvc.GetEnvironment("ms-dos-386")
                .Load(Services, arch);
            var rdr = new LeImageReader(RawImage, 0);
            var fileHeader = rdr.ReadStruct<ExpHeader>();
            var filesize = FileSize(ref fileHeader);
            var image = new ByteMemoryArea(Address.Ptr32(0), new byte[filesize]);
            Array.Copy(
                RawImage, fileHeader.cpHeader * 0x10u,
                image.Bytes, 0,
                Math.Min(RawImage.Length, image.Bytes.Length));
            var loadseg = new ImageSegment("DOSX_PROG", image, AccessMode.ReadWriteExecute);
            var segmentMap = new SegmentMap(addrLoad);
            var seg = segmentMap.AddSegment(loadseg);
            var program = new Program(segmentMap, arch, platform);
            var ep = ImageSymbol.Procedure(
                arch,
                Address.Ptr32(fileHeader.initialEip),
                "_start");
            program.EntryPoints.Add(ep.Address, ep);
            return program;
        }

        private uint FileSize(ref ExpHeader hdr)
        {
            uint size = hdr.cPages * (uint) ExeImageLoader.CbPageSize;
            if (hdr.cbLastPage != 0)
            {
                size += hdr.cbLastPage - (uint) ExeImageLoader.CbPageSize;
            }
            return size - hdr.cpHeader * 0x10u;
        }

        // http://fd.lod.bz/rbil/interrup/dos_kernel/214b.html#table-01619
        [Endian(Endianness.LittleEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct ExpHeader
        {
            public ushort magic;            // "MP" (4Dh 50h) signature
            public ushort cbLastPage;       // remainder of image size / page size(page size = 512h)
            public ushort cPages;           // size of image in pages
            public ushort cRelocations;     // number of relocation items
            public ushort cpHeader;         // header size in paragraphs
            public ushort minExtra4kPages;  // minimum number of extra 4K pages to be allocated at the end
                                            // of program, when it is loaded
            public ushort maxExtra4kPage;   // maximum number of extra 4K pages to be allocated at the end
                                            // of program, when it is loaded
            public uint initialEsp;         // initial ESP
            public ushort checksum;         // word checksum of file
            public uint initialEip;         // initial EIP
            public ushort offRelocations;   // offset of first relocation item
            public ushort nOverlay;         // overlay number
            public ushort w001C;            // ??? (wants to be 1)
        }
    }
}
