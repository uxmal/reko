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
using Reko.Core.IO;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public abstract class AbstractBaseAddressFinder
    {
        private uint stride;

        public AbstractBaseAddressFinder(EndianServices endianness, ByteMemoryArea mem)
        {
            Endianness = endianness;
            this.Memory = mem;
        }

        public abstract BaseAddressCandidate[] Run(CancellationToken ctoken);

        /// <summary>
        /// Endianness used to interpret pointer values.
        /// </summary>
        public EndianServices Endianness { get; set; }

        /// <summary>
        /// Memory area to scan.
        /// </summary>
        public ByteMemoryArea Memory { get; }


        /// <summary>
        /// Scan every N (power of 2) addresses. (default is 0x1000)'
        /// </summary>
        public uint Stride
        {
            get => stride;
            set
            {
                if (BitOperations.PopCount(value) != 1)
                    throw new ArgumentException("Value must be a power of 2.");
                stride = value;
            }
        }

        public HashSet<ulong> ReadPointers(ByteMemoryArea buffer, int alignment)
        {
            var pointers = new HashSet<ulong>();
            var rdr = Endianness.CreateImageReader(buffer, 0);
            var offset = rdr.Offset;
            while (rdr.TryReadUInt32(out uint v))
            {
                pointers.Add(v);
                offset = offset + alignment;
                rdr.Offset = offset;
            }
            return pointers;
        }

        protected static bool AddOverflow(ulong a, ulong b, ulong wordMask, out ulong result)
        {
            var s = (a + b) & wordMask;
            if (s < a)
            {
                result = 0;
                return true;
            }
            else
            {
                result = s;
                return false;
            }
        }


    }
    public struct BaseAddressCandidate
    {
        public BaseAddressCandidate(ulong uAddr, int confidence) : this()
        {
            Address = uAddr;
            Confidence = confidence;
        }

        public ulong Address { get; set; }
        public int Confidence { get; set; }
    }
}