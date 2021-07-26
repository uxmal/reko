#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Arch.Tlcs;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.NeoGeo
{
    public class PocketRomLoader : ImageLoader
    {
        private const uint RomLoadAddress = 0x00200000;
        private Address entryPoint;

        public PocketRomLoader(IServiceProvider services, string filename, byte[] imgRaw)
            : base(services, filename, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0x00000000);
            entryPoint = null!;
        }

        public override Address PreferredBaseAddress
        {
            get;
            set;
        }

        public override Program Load(Address? addrLoad)
        {
            var romSegment = new ImageSegment(
                ".text",
                new ByteMemoryArea(
                    Address.Ptr32(RomLoadAddress),
                    RawImage),
                AccessMode.ReadExecute);

            romSegment.MemoryArea.TryReadLeUInt32(0x1C, out uint uAddrEntry);
            this.entryPoint = Address.Ptr32(uAddrEntry);
            var segmap = new SegmentMap(PreferredBaseAddress, romSegment);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("tlcs-900")!;
            var env = cfgSvc.GetEnvironment("neo-geo-pocket");
            var platform = env.Load(Services, arch);
            
            return new Program(segmap, arch, platform);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol> {
                    ImageSymbol.Procedure(program.Architecture, entryPoint)
                },
                new SortedList<Address, ImageSymbol>());
        }
    }
}
