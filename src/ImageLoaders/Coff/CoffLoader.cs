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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    public class CoffLoader : ImageLoader
    {
        private IProcessorArchitecture arch;

        public CoffLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
            this.header = LoadHeader();
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        private FileHeader LoadHeader()
        {
            var rdr = new LeImageReader(RawImage, 0);
            var magic = rdr.ReadLeUInt16();
            var cfgSvc = Services.RequireService<IConfigurationService>();
            switch (magic)
            {
            case 0x014C: arch = cfgSvc.GetArchitecture("x86-real-16"); break;
            default: throw new NotSupportedException();
            }
            return new FileHeader
            {
                f_magic = magic,
                f_nscns = rdr.ReadUInt16(),
                f_timdat = rdr.ReadUInt32(),
                f_symptr = rdr.ReadUInt32(),
                f_nsyms = rdr.ReadUInt32(),
                f_opthdr = rdr.ReadUInt16(),
                f_flags = rdr.ReadUInt16(),
            };
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public FileHeader header { get; set; }
    }
}
