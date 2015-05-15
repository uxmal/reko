#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using System;
using System.Collections.Generic;

namespace Decompiler.ImageLoaders.MzExe
{
	/// <summary>
	/// Loads MS-DOS binary executables.
	/// </summary>
	public class MsdosImageLoader : ImageLoader
	{
        private IProcessorArchitecture arch;
        private Platform platform;
		private ExeImageLoader exe;
		private LoadedImage imgLoaded;
        private ImageMap imgLoadedMap;

		public MsdosImageLoader(IServiceProvider services, string filename, ExeImageLoader exe) : base(services, filename, exe.RawImage)
		{
			this.exe = exe;
            this.arch = new IntelArchitecture(ProcessorMode.Real);
            this.platform = new MsdosPlatform(services, arch);
		}

		public override Address PreferredBaseAddress
		{
			get { return Address.SegPtr(0x0800, 0); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            int iImageStart = (exe.e_cparHeader * 0x10);
            int cbImageSize = exe.e_cpImage * ExeImageLoader.CbPageSize - iImageStart;
            byte[] bytes = new byte[cbImageSize];
            int cbCopy = Math.Min(cbImageSize, RawImage.Length - iImageStart);
            Array.Copy(RawImage, iImageStart, bytes, 0, cbCopy);
            imgLoaded = new LoadedImage(addrLoad, bytes);
            imgLoadedMap = imgLoaded.CreateImageMap();
            return new Program(imgLoaded, imgLoadedMap, arch, platform);
        }

		public override RelocationResults Relocate(Address addrLoad)
		{
			ImageMap imageMap = imgLoadedMap;
			ImageReader rdr = new LeImageReader(exe.RawImage, (uint) exe.e_lfaRelocations);
            var relocations = new RelocationDictionary();
			int i = exe.e_cRelocations;
			while (i != 0)
			{
				uint offset = rdr.ReadLeUInt16();
				ushort segOffset = rdr.ReadLeUInt16();
				offset += segOffset * 0x0010u;

				ushort seg = (ushort) (imgLoaded.ReadLeUInt16(offset) + addrLoad.Selector);
				imgLoaded.WriteLeUInt16(offset, seg);
				relocations.AddSegmentReference(offset, seg);

				imageMap.AddSegment(Address.SegPtr(seg, 0), seg.ToString("X4"), AccessMode.ReadWriteExecute);
				--i;
			}
		
			// Found the start address.

			Address addrStart = Address.SegPtr((ushort)(exe.e_cs + addrLoad.Selector), exe.e_ip);
			imageMap.AddSegment(Address.SegPtr(addrStart.Selector, 0), addrStart.Selector.ToString("X4"), AccessMode.ReadWriteExecute);
            return new RelocationResults(
                new List<EntryPoint> { new EntryPoint(addrStart, arch.CreateProcessorState()) },
                relocations);
		}
	}
}
