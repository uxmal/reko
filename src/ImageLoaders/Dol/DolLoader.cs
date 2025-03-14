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
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reko.ImageLoaders.Dol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Endian(Endianness.BigEndian)]
	public struct DolStructure {
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public UInt32[] offsetText;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public UInt32[] offsetData;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public UInt32[] addressText;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public UInt32[] addressData;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public UInt32[] sizeText;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public UInt32[] sizeData;
		public UInt32 addressBSS;
		public UInt32 sizeBSS;
		public UInt32 entrypoint;
	}

	public class DolHeader {
		public UInt32[] offsetText = new UInt32[7];
		public UInt32[] offsetData = new UInt32[11];
		public Address[] addressText = new Address[7];
		public Address[] addressData = new Address[11];
		public UInt32[] sizeText = new UInt32[7];
		public UInt32[] sizeData = new UInt32[11];
		public Address addressBSS;
		public UInt32 sizeBSS;
		public Address entrypoint;

		public DolHeader(DolStructure hdr) {
			this.offsetText = hdr.offsetText;
			this.offsetData = hdr.offsetData;
			this.sizeText = hdr.sizeText;
			this.sizeBSS = hdr.sizeBSS;

			for (int i = 0; i < 7; i++) {
				this.addressText[i] = Address.Ptr32(hdr.addressText[i]);
			}
			for (int i = 0; i < 11; i++) {
				this.addressData[i] = Address.Ptr32(hdr.addressData[i]);
			}
			this.addressBSS = Address.Ptr32(hdr.addressBSS);
			this.entrypoint = Address.Ptr32(hdr.entrypoint);
		}
	}

	/* Adapted from https://github.com/heinermann/ida-wii-loaders */
	/* Format Reference: http://wiibrew.org/wiki/DOL */
	/// <summary>
	/// Image loader for Nintendo DOL file format.
	/// </summary>
	public class DolLoader : ProgramImageLoader
    {
		private DolHeader hdr;

		public DolLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
            : base(services, imageLocation, imgRaw)
        {
            this.hdr = null!;
		}

		public override Address PreferredBaseAddress {
			get {
				return this.hdr.entrypoint;
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override Program LoadProgram(Address? addrLoad) {
			var cfgSvc = Services.RequireService<IConfigurationService>();
			var arch = cfgSvc.GetArchitecture("ppc-32-be")!;
			var platform = cfgSvc.GetEnvironment("wii").Load(Services, arch);
			return base.LoadProgram(addrLoad ?? PreferredBaseAddress, arch, platform, new());
		}

        public override Program LoadProgram(
            Address addrLoad,
            IProcessorArchitecture arch,
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            BeImageReader rdr = new BeImageReader(this.RawImage, 0);
            DolStructure? str = rdr.ReadStruct<DolStructure>();
            if (!str.HasValue)
                throw new BadImageFormatException("Invalid DOL header.");
            this.hdr = new DolHeader(str.Value);
            var segments = new List<ImageSegment>();

            // Create code segments
            for (uint i = 0, snum = 1; i < 7; i++, snum++)
            {
                if (hdr.addressText[i].IsNull)
                    continue;
                var bytes = new byte[hdr.sizeText[i]];
                Array.Copy(RawImage, hdr.offsetText[i], bytes, 0, bytes.Length);
                var mem = new ByteMemoryArea(hdr.addressText[i], bytes); 
                segments.Add(new ImageSegment(
                    string.Format("Text{0}", snum),
                    mem,
                    AccessMode.ReadExecute));
            }

            // Create all data segments
            for (uint i = 0, snum = 1; i < 11; i++, snum++)
            {
                if (hdr.addressData[i].IsNull ||
                    hdr.sizeData[i] == 0)
                    continue;
                var bytes = new byte[hdr.sizeData[i]];
                Array.Copy(RawImage, hdr.offsetData[i], bytes, 0, bytes.Length);
                var mem = new ByteMemoryArea(hdr.addressText[i], bytes);

                segments.Add(new ImageSegment(
                    string.Format("Data{0}", snum),
                    mem,
                    AccessMode.ReadWrite));
            }

            if (!hdr.addressBSS.IsNull)
            {
                segments.Add(new ImageSegment(
                    ".bss",
                    new ByteMemoryArea(hdr.addressBSS, new byte[hdr.sizeBSS]),
                    AccessMode.ReadWrite));
            }

			var segmentMap = new SegmentMap(addrLoad, segments.ToArray());

            var entryPoint = ImageSymbol.Procedure(arch, this.hdr.entrypoint);
            var program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform,
                new() { { this.hdr.entrypoint, entryPoint } },
                new() { { this.hdr.entrypoint, entryPoint } });
			return program;
		}
	}
}
