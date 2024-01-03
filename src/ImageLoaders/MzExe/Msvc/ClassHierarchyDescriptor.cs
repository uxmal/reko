#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Msvc
{
    public record ClassHierarchyDescriptor(
        Address Address,
        uint Attributes,
        uint BaseClasses,
        Address BaseClassArray)
    {
        public static ClassHierarchyDescriptor? Read(EndianImageReader rdr, Func<uint, Address> addrGen)
        {
            var addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint signature))
                return null;
            if (signature != 0)
                return null;
            if (!rdr.TryReadUInt32(out uint attributes))
                return null;
            if (!rdr.TryReadUInt32(out uint baseClasses))
                return null;
            if (!rdr.TryReadUInt32(out uint pbaseClasses))
                return null;
            return new ClassHierarchyDescriptor(addr, attributes, baseClasses, addrGen(pbaseClasses));
        }
    }
}
