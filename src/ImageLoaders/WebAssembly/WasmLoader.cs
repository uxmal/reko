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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmLoader : ImageLoader
    {
        public WasmLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            LoadHeader();
            LoadSections();
            throw new NotImplementedException();
        }

        private void LoadSections()
        {
            throw new NotImplementedException();
        }

        public LeImageReader LoadHeader()
        {
            var rdr = new LeImageReader(RawImage);
            uint magic;
            if (!rdr.TryReadLeUInt32(out magic))
                throw new BadImageFormatException();
            uint version;
            if (!rdr.TryReadLeUInt32(out version))
                throw new BadImageFormatException();
            return rdr;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
