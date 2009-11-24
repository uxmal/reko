/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using System;
using System.Collections.Generic;

namespace Decompiler.ImageLoaders.MzExe
{
	/// <summary>
	/// Loads MS-DOS images.
	/// </summary>
	public class MsdosImageLoader : ImageLoader
	{
        private IProcessorArchitecture arch;
        private Platform platform;
		private ExeImageLoader exe;
		private ProgramImage imgLoaded;


		public MsdosImageLoader(IServiceProvider services, ExeImageLoader exe) : base(services, exe.RawImage)
		{
			this.exe = exe;
            this.arch = new IntelArchitecture(ProcessorMode.Real);
            this.platform = new MsdosPlatform(arch);
		}

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Platform Platform
        {
            get { return platform; }
        }

		public override Address PreferredBaseAddress
		{
			get { return new Address(0x800, 0); }
		}

		public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
		{
			ImageMap imageMap = imgLoaded.Map;
			ImageReader rdr = new ImageReader(exe.RawImage, (uint) exe.e_lfaRelocations);
			int i = exe.e_cRelocations;
			while (i != 0)
			{
				int offset = rdr.ReadLeUint16();
				ushort segOffset = rdr.ReadLeUint16();
				offset += segOffset * 0x0010;

				ushort seg = (ushort) (imgLoaded.ReadLeUint16(offset) + addrLoad.Selector);
				imgLoaded.WriteLeUint16(offset, seg);
				relocations.AddSegmentReference(offset, seg);

				imageMap.AddSegment(new Address(seg, 0), seg.ToString("X4"), AccessMode.ReadWrite);
				--i;
			}
		
			// Found the start address.

			Address addrStart = new Address((ushort)(exe.e_cs + addrLoad.Selector), exe.e_ip);
			imageMap.AddSegment(new Address(addrStart.Selector, 0), addrStart.Selector.ToString("X4"), AccessMode.ReadWrite);
			entryPoints.Add(new EntryPoint(addrStart, new IntelState()));
		}

        public override ProgramImage Load(Address addrLoad)
		{
			int iImageStart = (exe.e_cparHeader * 0x10);
			int cbImageSize = exe.e_cpImage * ExeImageLoader.CbPageSize - iImageStart;
			byte [] bytes = new byte[cbImageSize];
			int cbCopy = Math.Min(cbImageSize, RawImage.Length - iImageStart);
			Array.Copy(RawImage, iImageStart, bytes, 0, cbCopy);
			imgLoaded = new ProgramImage(addrLoad, bytes);
			return imgLoaded;
		}

	}
}
