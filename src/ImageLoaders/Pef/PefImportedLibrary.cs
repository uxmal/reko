#region License
/* 
 * Copyright (C) 2018-2023 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public struct PefImportedLibrary
    {
        public readonly PEFImportedLibrary Lib;
        public readonly string Name;

        public PefImportedLibrary(PEFImportedLibrary lib, string name)
        {
            Lib = lib;
            Name = name;
        }

        public static PefImportedLibrary Load(EndianByteImageReader rdr, PefLoaderStringTable loaderStringTable)
        {
            var lib = PEFImportedLibrary.Load(rdr);
            var name = loaderStringTable.ReadCString(lib.nameOffset);
            return new PefImportedLibrary(lib, name);
        }
    }
}
