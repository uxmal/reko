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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Loading
{
    public class NullLoader : ImageLoader
    {
        private byte[] imageBytes;

        public NullLoader(byte[] image) : base(image)
        {
            this.imageBytes = image;
        }

        public override ProgramImage Load(Address addrLoad)
        {
            return new ProgramImage(addrLoad, imageBytes);
        }

        public override ProgramImage LoadAtPreferredAddress()
        {
            return new ProgramImage(PreferredBaseAddress, imageBytes);
        }

        public override Address PreferredBaseAddress
        {
            get { return new Address(0); }
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
        }
    }
}
