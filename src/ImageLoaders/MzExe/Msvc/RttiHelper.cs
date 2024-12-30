#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Msvc
{
    /// <summary>
    /// This interface abstracts away the difference between 32- and 
    /// 64-bit representations of RTTI elements.
    /// </summary>
    public interface IRttiHelper
    {
        /// <summary>
        /// The signature used in the first word Complete Object Locators.
        /// </summary>
        uint ColSignature { get; }


        /// <summary>
        /// Reads a 32-bit uint and interprets it either an absolute address
        /// or an offset from the image base address.
        /// </summary>
        /// <param name="rdr"><see cref="EndianImageReader"/> to read the uint from.
        /// </param>
        /// <returns>An address of the appropriate bit size or null if the provided <paramref name="rdr" />
        /// is not in a valid state.
        /// </returns>
        Address? ReadOffsetPointer(EndianImageReader rdr);

        /// <summary>
        /// Reads an absolute address of the appropriate bit size.
        /// </summary>
        /// <param name="rdr"><see cref="EndianImageReader"/> to read the address from.
        /// </param>
        /// <returns>An address of the appropriate bit size or null if the provided <paramref name="rdr" />
        /// is not in a valid state.
        /// </returns>
        bool TryReadPointer(EndianImageReader rdr, [MaybeNullWhen(false)] out Address addr);
    }

    public class RttiHelper32 : IRttiHelper
    {
        public uint ColSignature => 0x0;

        public Address? ReadOffsetPointer(EndianImageReader rdr)
        {
            if (!rdr.TryReadLeUInt32(out uint ptr))
                return null;
            return Address.Ptr32(ptr);
        }

        public bool TryReadPointer(EndianImageReader rdr, [MaybeNullWhen(false)] out Address addr)
        {
            if (rdr.TryReadLeUInt32(out uint u))
            {
                addr = Address.Ptr32(u);
                return true;
            }
            else
            {
                addr = default;
                return false;
            }
        }
    }

    public class RttiHelper64 : IRttiHelper
    {
        private readonly Address addrBase;

        public RttiHelper64(Address addrBase)
        {
            this.addrBase = addrBase;
        }

        public uint ColSignature => 0x1;

        public Address? ReadOffsetPointer(EndianImageReader rdr)
        {
            if (!rdr.TryReadLeUInt32(out uint offset))
                return null;
            return addrBase + offset;
        }

        public bool TryReadPointer(EndianImageReader rdr, [MaybeNullWhen(false)] out Address addr)
        {
            if (rdr.TryReadLeUInt64(out ulong u))
            {
                addr = Address.Ptr64(u);
                return true;
            }
            else
            {
                addr = default;
                return false;
            }
        }
    }
}
