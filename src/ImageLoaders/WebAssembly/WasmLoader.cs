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

        public Section LoadSection(LeImageReader rdr)
        {
            byte type;
            uint payload_len;
            uint name_len;
            if (!TryReadVarUInt7(rdr, out type))
                throw new BadImageFormatException();
            if (!TryReadVarUInt32(rdr, out payload_len))
                throw new BadImageFormatException();
            if (!TryReadVarUInt32(rdr, out name_len))
                throw new NotImplementedException();
            var name = "";
            if (name_len > 0)
            {
                name = Encoding.UTF8.GetString(rdr.ReadBytes(name_len));
            }
            byte[] bytes;
            if (payload_len > 0)
            {
                bytes = rdr.ReadBytes(payload_len);
            }
            else
            {
                bytes = new byte[0];
            }
            return new Section
            {
                Type = (WasmSection)type,
                Name = name,
                Bytes = bytes,
            };
        }

        private bool TryReadVarUInt32(LeImageReader rdr, out uint u)
        {
            u = 0;
            int sh = 0;
            for (;;)
            {
                byte b;
                if (!rdr.TryReadByte(out b))
                    return false;
                u = ((b & 0x7Fu) << sh) | u;
                if ((b & 0x80) == 0)
                {
                    // found the msb.
                    return true;
                }
                //$TODO: overflow.
            }
        }

        private bool TryReadVarUInt7(ImageReader rdr, out byte b)
        {
            if (!rdr.TryReadByte(out b))
                return false;
            if ((b & 0x80) != 0)
                return false;
            return true;
        }
    }

    public enum WasmSection
    {
        Type = 1,       // Function signature declarations
        Import = 2,     // Import declarations
        Function = 3,   // Function declarations
        Table = 4,      // Indirect function table and other tables
        Memory = 5,     // Memory attributes
        Global = 6,     // Global declarations
        Export = 7,     // Exports
        Start = 8,      // Start function declaration
        Element = 9,    // Elements section
        Code = 10,      // Function bodies (code)
        Data = 11,      // Data segments
    }

    public class Section
    {
        public byte[] Bytes { get; internal set; }
        public string Name { get; internal set; }
        public WasmSection Type { get; internal set; }
    }
}
