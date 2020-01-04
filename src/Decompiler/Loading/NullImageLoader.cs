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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Loading
{
    /// <summary>
    /// The NullLoader is used when Reko is unable to determine what image
    /// loader to use. It doesn't support disassembly.
    /// </summary>
    public class NullImageLoader : ImageLoader
    {
        private Address baseAddr;
        private byte[] imageBytes;

        public NullImageLoader(IServiceProvider services, string filename, byte[] image) : base(services, filename, image)
        {
            this.imageBytes = image;
            this.baseAddr = Address.Ptr32(0);
            this.EntryPoints = new List<ImageSymbol>();
        }

        public IProcessorArchitecture Architecture { get; set; }
        public List<ImageSymbol> EntryPoints { get; private set; }
        public IPlatform Platform { get; set; }
        public override Address PreferredBaseAddress
        {
            get { return this.baseAddr; }
            set { this.baseAddr = value; }
        }

        public override Program Load(Address addrLoad)
        {
            if (addrLoad == null)
                addrLoad = PreferredBaseAddress;
            var platform = Platform ?? new DefaultPlatform(Services, Architecture);
            return Load(addrLoad, Architecture, platform);
        }

        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            var segmentMap = CreatePlatformSegmentMap(platform, addrLoad, imageBytes);
            var program = new Program(
                segmentMap,
                arch,
                platform);
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(EntryPoints, new SortedList<Address, ImageSymbol>());
        }

        public SegmentMap CreatePlatformSegmentMap(IPlatform platform, Address loadAddr, byte[] rawBytes)
        {
            var segmentMap = platform.CreateAbsoluteMemoryMap();
            if (segmentMap == null)
            {
                segmentMap = new SegmentMap(loadAddr);
            }
            var mem = new MemoryArea(loadAddr, rawBytes);
            segmentMap.AddSegment(mem, "code", AccessMode.ReadWriteExecute);
            return segmentMap;
        }

    }
}
