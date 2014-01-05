#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Environments.AmigaOS;
using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    /// <summary>
    /// This class knows how to load and how to relocate AmigaOS Hunk files.
    /// </summary>
    public class HunkLoader : ImageLoader
    {
        public HunkLoader(IServiceProvider services, byte[] imgRaw) : base(services, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override LoaderResults Load(Address addrLoad)
        {
            var imgReader = new ImageReader(RawImage, 0);
            var arch = new M68kArchitecture();
            var magic = imgReader.ReadBeInt32();
            if (magic != 0x000003F3)
                throw new FormatException("Not a valid AmigaOS Hunk header.");
            throw new NotImplementedException();
            return new LoaderResults(
                null, //$TODO: loaded  image should go here.
                new M68kArchitecture(),
                new AmigaOSPlatform(Services, arch));
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
            throw new NotImplementedException();
        }
    }
}
