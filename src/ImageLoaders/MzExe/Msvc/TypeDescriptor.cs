#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Msvc
{
    public record TypeDescriptor(
        Address Address,
        Address VFTableAddress,
        string Name)
    {
        public static TypeDescriptor? Read(EndianImageReader rdr, Func<uint, Address> addrGen)
        {
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out uint pVFTable))
                return null;
            if (!rdr.TryReadLeUInt32(out _))
                return null;
            var str = rdr.ReadCString(PrimitiveType.Char, Encoding.UTF8).ToString();
            return new TypeDescriptor(addr, addrGen(pVFTable), str);
        }
    }
}
