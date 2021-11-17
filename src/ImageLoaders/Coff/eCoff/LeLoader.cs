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

using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Coff.eCoff
{
    using coff_ushort = System.UInt16;
    using coff_int = System.Int32;
    using coff_off = System.UInt32;

    public class LeLoader : ProgramImageLoader
    {
        public LeLoader(IServiceProvider services, string filename, byte[] rawImage)
            : base(services, filename, rawImage)
        {
            this.PreferredBaseAddress = Address.Ptr32(0x0010_0000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
