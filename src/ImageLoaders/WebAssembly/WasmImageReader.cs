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

using Reko.Core.Memory;

namespace Reko.ImageLoaders.WebAssembly
{
    /// <summary>
    /// Extends the LeImageReader class to provide functions for reading WASM
    /// formatted integers.
    /// </summary>
    public class WasmImageReader : LeImageReader
    {
        public WasmImageReader(byte[] bytes) : base(bytes)
        {
        }

        public WasmImageReader(ByteMemoryArea mem) : base(mem, 0)
        {
        }

        public bool TryReadVarUInt32(out uint u)
        {
            u = 0;
            int sh = 0;
            byte b;
            do
            {
                if (!TryReadByte(out b))
                    return false;
                u = ((b & 0x7Fu) << sh) | u;
                sh += 7;
                //$TODO: overflow.
            } while ((b & 0x80) != 0);
            return true;
        }

        public bool TryReadVarUInt64(out ulong u)
        {
            u = 0;
            int sh = 0;
            byte b;
            do
            {
                if (!TryReadByte(out b))
                    return false;
                u = ((b & 0x7Fu) << sh) | u;
                sh += 7;
                //$TODO: overflow.
            } while ((b & 0x80) != 0);
            return true;
        }

        public bool TryReadVarUInt7(out byte b)
        {
            if (!TryReadByte(out b))
                return false;
            if ((b & 0x80) != 0)
                return false;
            return true;
        }

        public bool TryReadVarInt7(out sbyte sb)
        {
            sb = 0;
            if (!TryReadByte(out byte b))
                return false;
            if ((b & 0x80) != 0)
                return false;
            if ((b & 40) != 0)
            {
                sb = (sbyte)(0xC0 | b);
            }
            else
            {
                sb = (sbyte)b;
            }
            return true;
        }

        public bool TryReadVarInt64(out long result)
        {
            int shift = 0;
            long u = 0;
            int size = 64;
            byte b;
            while (TryReadByte(out b))
            {
                u |= ((b & 0x7Fu) << shift);
                shift += 7;
                if ((b & 0x80) == 0)
                {
                    if (shift < size && (b & 0x40) != 0)
                        result = u | (~0L << shift);
                    else
                        result = u;
                    return true;
                }
            }
            result = 0;
            return false;
        }
    }
}
