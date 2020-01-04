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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Environments.Wii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
		public Address32[] addressText = new Address32[7];
		public Address32[] addressData = new Address32[11];
		public UInt32[] sizeText = new UInt32[7];
		public UInt32[] sizeData = new UInt32[11];
		public Address32 addressBSS;
		public UInt32 sizeBSS;
		public Address32 entrypoint;

		public DolHeader(DolStructure hdr) {
			this.offsetText = hdr.offsetText;
			this.offsetData = hdr.offsetData;
			this.sizeText = hdr.sizeText;
			this.sizeBSS = hdr.sizeBSS;

			for (int i = 0; i < 7; i++) {
				this.addressText[i] = new Address32(hdr.addressText[i]);
			}
			for (int i = 0; i < 11; i++) {
				this.addressData[i] = new Address32(hdr.addressData[i]);
			}
			this.addressBSS = new Address32(hdr.addressBSS);
			this.entrypoint = new Address32(hdr.entrypoint);
		}
	}

	/* Adapted from https://github.com/heinermann/ida-wii-loaders */
	/* Format Reference: http://wiibrew.org/wiki/DOL */
	/// <summary>
	/// Image loader for Nintendo DOL file format.
	/// </summary>
	public class DolLoader : ImageLoader
    {
		private DolHeader hdr;

		public DolLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw) {
		}

		public override Address PreferredBaseAddress {
			get {
				return this.hdr.entrypoint;
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override Program Load(Address addrLoad) {
			var cfgSvc = Services.RequireService<IConfigurationService>();
			var arch = cfgSvc.GetArchitecture("ppc-32-be");
			var platform = cfgSvc.GetEnvironment("wii").Load(Services, arch);
			return Load(addrLoad, arch, platform);
		}

        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform) {
            BeImageReader rdr = new BeImageReader(this.RawImage, 0);
            DolStructure? str = rdr.ReadStruct<DolStructure>();
            if (!str.HasValue)
                throw new BadImageFormatException("Invalid DOL header.");
            this.hdr = new DolHeader(str.Value);
            var segments = new List<ImageSegment>();

            // Create code segments
            for (uint i = 0, snum = 1; i < 7; i++, snum++)
            {
                if (hdr.addressText[i] == Address32.NULL)
                    continue;
                var bytes = new byte[hdr.sizeText[i]];
                Array.Copy(RawImage, hdr.offsetText[i], bytes, 0, bytes.Length);
                var mem = new MemoryArea(hdr.addressText[i], bytes); 
                segments.Add(new ImageSegment(
                    string.Format("Text{0}", snum),
                    mem,
                    AccessMode.ReadExecute));
            }

            // Create all data segments
            for (uint i = 0, snum = 1; i < 11; i++, snum++)
            {
                if (hdr.addressData[i] == Address32.NULL ||
                    hdr.sizeData[i] == 0)
                    continue;
                var bytes = new byte[hdr.sizeData[i]];
                Array.Copy(RawImage, hdr.offsetData[i], bytes, 0, bytes.Length);
                var mem = new MemoryArea(hdr.addressText[i], bytes);

                segments.Add(new ImageSegment(
                    string.Format("Data{0}", snum),
                    mem,
                    AccessMode.ReadWrite));
            }

            if (hdr.addressBSS != Address32.NULL)
            {
                segments.Add(new ImageSegment(
                    ".bss",
                    new MemoryArea(hdr.addressBSS, new byte[hdr.sizeBSS]),
                    AccessMode.ReadWrite));
            }

			var segmentMap = new SegmentMap(addrLoad, segments.ToArray());

            var entryPoint = ImageSymbol.Procedure(arch, this.hdr.entrypoint);
			var program = new Program(
				segmentMap,
				arch,
				platform) {
				ImageSymbols = { { this.hdr.entrypoint, entryPoint } },
				EntryPoints = { { this.hdr.entrypoint, entryPoint } }
			};
			return program;
		}

		public override RelocationResults Relocate(Program program, Address addrLoad) {
			throw new NotImplementedException();
		}
	}
}
