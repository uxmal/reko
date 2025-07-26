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
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Reko.Scanning
{
    /// <summary>
    /// Classes deriving from this abstract class are used to 
    /// determine the base address of a binary program image
    /// when the image cannot provide that address.
    /// </summary>
    public abstract class AbstractBaseAddressFinder
    {
        private uint stride;

        /// <summary>
        /// Initializees a new instance of the <see cref="AbstractBaseAddressFinder"/> class.
        /// </summary>
        /// <param name="endianness">Endianness to use when discovering pointers.</param>
        /// <param name="mem"><see cref="ByteMemoryArea"/> containing the program
        /// image.</param>
        public AbstractBaseAddressFinder(EndianServices endianness, ByteMemoryArea mem)
        {
            Endianness = endianness;
            this.Memory = mem;
        }

        /// <summary>
        /// Executes the base address finder algorithm.
        /// </summary>
        /// <param name="ctoken"><see cref="CancellationToken"/> used to
        /// stop the algorithm.</param>
        /// <returns>Base address candidates found.
        /// </returns>
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
        /// Scan every N (power of 2) addresses (default is 0x1000).
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

        /// <summary>
        /// Reads 32-bit pointer (candidates) from the given buffer, aligned to the specified
        /// alignment.
        /// </summary>
        /// <param name="buffer">Buffer to read from.</param>
        /// <param name="alignment">Alignment to respect.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add two numbers, and return both the sum and 
        /// unsigned overflow.
        /// </summary>
        /// <param name="a">Augend.</param>
        /// <param name="b">Addend.</param>
        /// <param name="wordMask">
        /// Bitmask used to adjust the width of the summands.
        /// </param>
        /// <param name="result">
        /// The resulting sum if no unsigned overflow occurred, otherwise
        /// 0.</param>
        /// <returns>
        /// True if an overflow occurred.
        /// </returns>
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

    /// <summary>
    /// Represents a candidate for the base address of a program image.
    /// </summary>
    public struct BaseAddressCandidate
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="BaseAddressCandidate"/> struct.
        /// </summary>
        /// <param name="uAddr">The linear address value.</param>
        /// <param name="confidence">The confidence of this value being a pointer.
        /// </param>
        public BaseAddressCandidate(ulong uAddr, int confidence) : this()
        {
            Address = uAddr;
            Confidence = confidence;
        }

        /// <summary>
        /// Linear address of the candidate.
        /// </summary>
        public ulong Address { get; set; }

        /// <summary>
        /// Confidence level of this candidate being a base address.
        /// </summary>
        public int Confidence { get; set; }
    }
}