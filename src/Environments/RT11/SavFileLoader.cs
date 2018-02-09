#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using Reko.Core;
using Reko.Arch.Pdp11;
using System.Collections.Generic;

namespace Reko.Environments.RT11
{
    public class SavFileLoader : ImageLoader
    {
        public SavFileLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
            this.PreferredBaseAddress = Address.Ptr16(0);
        }

        public override Address PreferredBaseAddress
        {
            get; set;
        }

        public override Program Load(Address addrLoad)
        {
            var arch = new Pdp11Architecture("pdp11");

            return new Program(
                new SegmentMap(addrLoad,
                new ImageSegment(".text",
                        new MemoryArea(addrLoad, RawImage),
                        AccessMode.ReadWriteExecute)),
                arch,
                new RT11Platform(Services, arch));
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var uaddrEntry = MemoryArea.ReadLeUInt16(RawImage, 0x20);
            var entry = new ImageSymbol(Address.Ptr16(uaddrEntry));
            return new RelocationResults(
                new List<ImageSymbol> { entry },
                new SortedList<Address, ImageSymbol>())
            {
            };
        }
    }
}