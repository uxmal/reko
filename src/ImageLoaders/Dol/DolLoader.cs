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
	public class DolLoader : ImageLoader {
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

		public override Program Load(Address addrLoad) {
			BeImageReader rdr = new BeImageReader(this.RawImage, 0);
			this.hdr = new FileHeader(rdr);

			var cfgSvc = Services.RequireService<IConfigurationService>();
			var arch = cfgSvc.GetArchitecture("ppc");

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
				new WiiPlatform(Services, arch, "Wii")
			);
		}

		public override RelocationResults Relocate(Program program, Address addrLoad) {
			throw new NotImplementedException();
		}

		public class FileHeader {
			public uint[] offsetText = new uint[7];
			public uint[] offsetData = new uint[11];
			public Address32[] addressText = new Address32[7];
			public Address32[] addressData = new Address32[11];
			public uint[] sizeText = new uint[7];
			public uint[] sizeData = new uint[11];
			public Address32 addressBSS;
			public uint sizeBSS;
			public Address32 entrypoint;


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
					addressText[i] = new Address32(uAddress);
				}
				for (uint i = 0; i < 11; i++) {
					result = rdr.TryReadUInt32(out uAddress);
					if (!result)
						goto fail;
					addressData[i] = new Address32(uAddress);
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
				this.addressBSS = new Address32(uAddress);

				result = rdr.TryReadUInt32(out sizeBSS);
				if (!result)
					goto fail;

				result = rdr.TryReadUInt32(out uAddress);
				if (!result)
					goto fail;
				this.entrypoint = new Address32(uAddress);

				fail:
				throw new BadImageFormatException("Invalid DOL header.");
			}
		}
	}
}
