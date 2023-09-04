#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Coff.eCoff
{
    public class BeLoader : ProgramImageLoader
    {
        private aouthdr? opthdr;

        public BeLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawImage)
            : base(services, imageLocation, rawImage)
        {
            this.PreferredBaseAddress = Address.Ptr32(0x0010_0000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var rdr = new BeImageReader(RawImage);
            var header = rdr.ReadStruct<filehdr>();
            if (header.f_opthdr != 0)
            {
                var sectionOffset = rdr.Offset + header.f_opthdr;
                opthdr = rdr.ReadStruct<aouthdr>();
                rdr.Offset = sectionOffset;
            }
            var sections = new scnhdr[header.f_nscns];
            for (int i = 0; i < sections.Length; ++i)
            {
                sections[i] = rdr.ReadStruct<scnhdr>();
            }
            var imgSegments = new ImageSegment[header.f_nscns];
            for (int i = 0; i < sections.Length; ++i)
            {
                imgSegments[i] = LoadImageSegment(sections[i]);
            }

            var arch = LoadArchitecture(header.f_magic);
            var platform = new DefaultPlatform(Services, arch);
            var segmap = new SegmentMap(imgSegments);
            var program = new Program(segmap, arch, platform);
            if (this.opthdr.HasValue)
            {
                var addrEntry = Address.Ptr32(opthdr.Value.entry);   //$64
                var ep = ImageSymbol.Procedure(program.Architecture, addrEntry, "_start");
                program.EntryPoints.Add(ep.Address, ep);
            }
            return program;
        }

        private ImageSegment LoadImageSegment(in scnhdr scnhdr)
        {
            var bytes = new byte[scnhdr.s_size];
            var availableBytes = RawImage.Length - scnhdr.s_scnptr;
            if (availableBytes > 0)
            {
                Array.Copy(RawImage, scnhdr.s_scnptr, bytes, 0, Math.Min(bytes.Length, availableBytes));
            }
            var addr = Address.Ptr32(scnhdr.s_vaddr); //$64-bit
            var mem = new ByteMemoryArea(addr, bytes);
            var name = Utf8StringFromFixedArray(scnhdr.s_name);
            var access = ComputeSectionAccess(scnhdr.s_flags);
            var imgseg = new ImageSegment(name, mem, access);
            return imgseg;
        }

        private AccessMode ComputeSectionAccess(eCoffSectionFlags flags)
        {
            switch (flags)
            {
            case eCoffSectionFlags.STYP_TEXT:
            case eCoffSectionFlags.STYP_INIT:
            case eCoffSectionFlags.STYP_FINI:
                return AccessMode.ReadExecute;
            case eCoffSectionFlags.STYP_DATA:
            case eCoffSectionFlags.STYP_BSS: 
            case eCoffSectionFlags.STYP_SDATA:
            case eCoffSectionFlags.STYP_SBSS:
                return AccessMode.ReadWrite;
            case eCoffSectionFlags.STYP_RDATA:
            case eCoffSectionFlags.STYP_ECOFF_LIB:
                return AccessMode.Read;
            case (eCoffSectionFlags) 0x0020_0000:
                return AccessMode.Read;
            }
            throw new NotImplementedException();
        }

        private static string Utf8StringFromFixedArray(byte[] abName)
        {
            int i = Array.IndexOf<byte>(abName, 0);
            if (i < 0)
                i = abName.Length;
            return Encoding.UTF8.GetString(abName, 0, i);
        }

        private IProcessorArchitecture LoadArchitecture(ushort f_magic)
        {
            string sArch;
            string endianness;
            int wordsize;
            switch (f_magic)
            {
            case 0x160:
                (sArch, endianness, wordsize) = ("mips-be-32", "be", 32); break;
            default:
                throw new NotSupportedException($"eCoff architecture {0:X4} is not supported yet.");
            }
            var options = new Dictionary<string, object>
            {
                { ProcessorOption.Endianness, endianness },
                { ProcessorOption.WordSize, wordsize},
            };
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture(sArch, options);
            if (arch is null)
                throw new InvalidOperationException($"Unable to load the '{sArch}' architecture.");
            return arch;
        }
    }
}
