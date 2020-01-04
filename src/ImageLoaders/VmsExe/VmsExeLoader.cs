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

using Reko.Arch.Vax;
using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.VmsExe
{
    /// <summary>
    /// Documentation for the VAX VMS .EXE format is hard to obtain and understand(!)
    /// </summary>
    // https://mail.encompasserve.org/anon/htnotes/note?f1=DEC_SOFTWARE&f2=444.6
    // http://fossies.org/linux/freevms/sys/src/sysimgact.c
    public class VmsExeLoader : ImageLoader
    {
        public VmsExeLoader(IServiceProvider services, string filename, byte[] imgRaw) 
            : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var rdr = new LeImageReader(RawImage);
            var hdr = LoadHeader(rdr);
            var isds = LoadImageSectionDescriptors(hdr.HdrSize);

            var addr = Address.Ptr32(0x1000);   //$REVIEW: what should this really be?
            var arch = (VaxArchitecture)Services.RequireService<IConfigurationService>()
                .GetArchitecture("vax");
            return new Program(
                new SegmentMap(
                    addr,
                    new ImageSegment(
                        "", 
                        new MemoryArea(addr, RawImage),
                        AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(Services, arch, "VAX VMS"));   //$TODO: VaxVms platform
        }

        private Header LoadHeader(LeImageReader rdr)
        {
            var header = new Header
            {
                HdrSize = rdr.ReadLeUInt16(),
                RvaTaa = rdr.ReadLeUInt16(),
                RvaSymbols = rdr.ReadLeUInt16(),
                RvaIdent = rdr.ReadLeUInt16(),
                RvaPatchData = rdr.ReadLeUInt16(),
                Spare0A = rdr.ReadLeUInt16(),
                IdMajor = rdr.ReadLeUInt16(),
                IdMinor = rdr.ReadLeUInt16(),

                HeaderBlocks = rdr.ReadByte(),
                ImageType = rdr.ReadByte(),
                Spare12 = rdr.ReadLeUInt16(),

                RequestedPrivilegeMask = rdr.ReadLeUInt64(),
                IoChannels = rdr.ReadLeUInt16(),
                IoSegPages = rdr.ReadLeUInt16(),
                ImageFlags = rdr.ReadLeUInt32(),
                GlobalSectionID = rdr.ReadLeUInt32(),
                SystemVersionNumber = rdr.ReadLeUInt32(),
            };
            return header;
        }

        private List<ImageSectionDescriptor> LoadImageSectionDescriptors(ushort rvaIsds)
        {
            var sections = new List<ImageSectionDescriptor>();
            var rdr = new LeImageReader(RawImage, rvaIsds);
            Debug.WriteLine("Isd: Size Pges Start    Flags    Rva      GsId     Name");

            for (;;)
            {
                var isd = new ImageSectionDescriptor();
                isd.Size = rdr.ReadLeUInt16();
                if (isd.Size == 0)
                    break;
                isd.NumPages = rdr.ReadLeUInt16();
                isd.StartVPage = rdr.ReadLeUInt32();
                isd.Flags = rdr.ReadLeUInt32();
                if (isd.Size > 0x0C)
                {
                    isd.RvaFile = rdr.ReadLeUInt32();
                    if (isd.Size > 0x010)
                    {
                        isd.GlobalSectionIdent = rdr.ReadLeUInt32();
                        var count = rdr.ReadByte();
                        var sectionName = rdr.ReadBytes(count);
                        isd.SectionName = Encoding.ASCII.GetString(sectionName);
                    }
                }
                sections.Add(isd);
                Debug.WriteLine("{0}", isd);
            }
            return sections;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(),
                new SortedList<Address, ImageSymbol>());
        }
    }
}
