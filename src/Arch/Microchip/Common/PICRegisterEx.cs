#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Types;
using System;

namespace Reko.Arch.Microchip.Common
{
    public static class PICRegisterEx
    {

        /// <summary>
        /// Converts a bit size (bitfield width) to the most appropriate Reko primitive word type.
        /// </summary>
        /// <param name="bitSize">The bit size.</param>
        /// <returns>
        /// A PrimitiveType value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when bit size is outside the required range.</exception>
        public static PrimitiveType Size2PrimitiveType(this uint bitSize)
        {
            if (bitSize == 0)
                throw new ArgumentOutOfRangeException(nameof(bitSize), $"Value = {bitSize}");
            if (bitSize == 1)
                return PrimitiveType.Bool;
            if (bitSize <= 8)
                return PrimitiveType.Byte;
            if (bitSize <= 16)
                return PrimitiveType.Word16;
            if (bitSize <= 32)
                return PrimitiveType.Word32;
            if (bitSize <= 64)
                return PrimitiveType.Word64;
            if (bitSize <= 128)
                return PrimitiveType.Word128;
            if (bitSize <= 256)
                return PrimitiveType.Word256;
            throw new ArgumentOutOfRangeException(nameof(bitSize), $"Value = {bitSize}");
        }

    }

}
