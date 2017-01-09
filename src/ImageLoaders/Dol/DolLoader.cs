#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Dol
{
	/* Adapted from https://github.com/heinermann/ida-wii-loaders */
	/* Format Reference: http://wiibrew.org/wiki/DOL */
    /// <summary>
    /// Image loader for Nintendo DOL file format.
    /// </summary>
	public class DolLoader : ImageLoader
    {
		private FileHeader hdr;

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

        public override Program Load(Address addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc");
            var platform = new WiiPlatform(Services, arch);
            return Load(addrLoad, arch, platform);
        }

        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
			BeImageReader rdr = new BeImageReader(this.RawImage, 0);
			this.hdr = new FileHeader(rdr);


			List<ImageSegment> segments = new List<ImageSegment>();
			// add the code segment
			for (uint i=0, snum=1; i<7; i++, snum++) {
				if (hdr.addressText[i] == new Address32(0))
					continue;

				segments.Add(new ImageSegment(
					string.Format("Text{0}", snum),
					new MemoryArea(hdr.addressText[i], RawImage),
					AccessMode.ReadWriteExecute
				));
			}

			// create all data segments
			for (uint i = 0, snum = 1; i < 11; i++, snum++) {
				if (hdr.addressData[i] == new Address32(0))
					continue;
				segments.Add(new ImageSegment(
					string.Format("Data{0}", snum),
					new MemoryArea(hdr.addressText[i], RawImage),
					AccessMode.ReadWrite
				));
			}

			if (hdr.addressBSS != new Address32(0)) {
				segments.Add(new ImageSegment(
					".bss",
					new MemoryArea(hdr.addressBSS, RawImage),
					AccessMode.ReadWrite
				));
			}

			SegmentMap segmentMap = new SegmentMap(hdr.entrypoint, segments.ToArray());

			return new Program(
				segmentMap,
				arch,
                platform
			);
		}

		public override RelocationResults Relocate(Program program, Address addrLoad) {
			throw new NotImplementedException();
		}

		public class FileHeader {
			public uint[] offsetText = new uint[7];
			public uint[] offsetData = new uint[11];
			public Address[] addressText = new Address[7];
			public Address[] addressData = new Address[11];
			public uint[] sizeText = new uint[7];
			public uint[] sizeData = new uint[11];
			public Address addressBSS;
			public uint sizeBSS;
			public Address entrypoint;

			public FileHeader(BeImageReader rdr) {
				uint uAddress;

				bool result;
				for (uint i = 0; i < 7; i++) {
					result = rdr.TryReadUInt32(out offsetText[i]);
					if (!result)
						goto fail;
				}
				for (uint i = 0; i < 11; i++) {
					result = rdr.TryReadUInt32(out offsetData[i]);
					if (!result)
						goto fail;
				}
				for (uint i = 0; i < 7; i++) {
					result = rdr.TryReadUInt32(out uAddress);
					if (!result)
						goto fail;
					addressText[i] = Address.Ptr32(uAddress);
				}
				for (uint i = 0; i < 11; i++) {
					result = rdr.TryReadUInt32(out uAddress);
					if (!result)
						goto fail;
					addressData[i] = Address.Ptr32(uAddress);
				}
				for (uint i = 0; i < 7; i++) {
					result = rdr.TryReadUInt32(out sizeText[i]);
					if (!result)
						goto fail;
				}
				for (uint i = 0; i < 11; i++) {
					result = rdr.TryReadUInt32(out sizeData[i]);
					if (!result)
						goto fail;
				}

				result = rdr.TryReadUInt32(out uAddress);
				if (!result)
					goto fail;
				this.addressBSS = Address.Ptr32(uAddress);

				result = rdr.TryReadUInt32(out sizeBSS);
				if (!result)
					goto fail;

				result = rdr.TryReadUInt32(out uAddress);
				if (!result)
					goto fail;
				this.entrypoint = Address.Ptr32(uAddress);
                return;

				fail:
				throw new BadImageFormatException("Invalid DOL header.");
			}
		}
	}
}
