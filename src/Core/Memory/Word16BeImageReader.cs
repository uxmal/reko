#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// A big-endian image reader for a memory area organized in 16-bit words.
    /// </summary>
    public class Word16BeImageReader : Word16ImageReader, EndianImageReader
    {
        /// <summary>
        /// Constructs a reader on a memory area, starting at the given offset.
        /// </summary>
        /// <param name="mem">A <see cref="Word16MemoryArea"/> to read from.</param>
        /// <param name="offset">An offset within the words of the memory area.</param>
        public Word16BeImageReader(Word16MemoryArea mem, long offset)
            : base(mem, offset, mem.Words.Length)
        {
        }

        /// <summary>
        /// Constructs a reader on a memory area, starting at the given address.
        /// </summary>
        /// <param name="mem">A <see cref="Word16MemoryArea"/> to read from.</param>
        /// <param name="addr">An address within the words of the memory area.</param>
        public Word16BeImageReader(Word16MemoryArea mem, Address addr)
            : base(mem, addr - mem.BaseAddress)
        {
        }

        /// <summary>
        /// Constructs a reader on the memory area, starting at the given address,
        /// and reading at most <paramref name="cUnits"/> words.
        /// </summary>
        /// <param name="mem">A <see cref="Word16MemoryArea"/> to read from.</param>
        /// <param name="addr">An address within the words of the memory area.</param>
        /// <param name="cUnits">Maximum number of words to read before stopping.</param>
        public Word16BeImageReader(Word16MemoryArea mem, Address addr, long cUnits)
            : base(mem, addr - mem.BaseAddress, (addr - mem.BaseAddress) + cUnits)
        {
        }

        /// <summary>
        /// Constructs a reader on the memory area, restricted to reading words
        /// between the two given offsets.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="endOffset">Ending offset.</param>
        public Word16BeImageReader(Word16MemoryArea mem, long offset, long endOffset) 
            : base(mem, offset, endOffset)
        {
        }

        /// <summary>
        /// Clones this instance of <see cref="EndianImageReader"/>.
        /// </summary>
        /// <returns>A copy of this <see cref="Word16BeImageReader"/>.</returns>
        public EndianImageReader Clone()
        {
            return new Word16BeImageReader(this.mem, this.Offset);
        }

        /// <summary>
        /// Creates a new <see cref="Word16BeImageReader"/> on the given
        /// <see cref="MemoryArea"/> at the given address.
        /// </summary>
        /// <returns>A copy of this <see cref="Word16BeImageReader"/>.</returns>
        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new Word16BeImageReader((Word16MemoryArea) image, addr);
        }
        
        /// <inheritdoc />
        public string ReadNulTerminatedString(DataType charType, Encoding enc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public short ReadInt16() => ReadBeInt16();

        /// <inheritdoc/>
        public int ReadInt32() => ReadBeInt32();

        /// <inheritdoc/>
        public long ReadInt64() => ReadBeInt64();

        /// <inheritdoc/>
        public bool ReadNullCharTerminator(DataType dtChar)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort ReadUInt16() => ReadBeUInt16();

        /// <inheritdoc/>
        public uint ReadUInt32() => ReadBeUInt32();

        /// <inheritdoc/>
        public ulong ReadUInt64() => ReadBeUInt64();

        /// <inheritdoc/>
        public bool TryPeekUInt32(int offset, out uint value) => TryPeekBeUInt32(offset, out value);

        /// <inheritdoc/>
        public bool TryPeekUInt64(int offset, out ulong value) => TryPeekBeUInt64(offset, out value);

        /// <inheritdoc/>
        public bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value) => TryReadBe(dataType, out value);

        /// <inheritdoc/>
        public bool TryReadInt16(out short value) => TryReadBeInt16(out value);

        /// <inheritdoc/>
        public bool TryReadInt32(out int value) => TryReadBeInt32(out value);

        /// <inheritdoc/>
        public bool TryReadInt64(out long value) => TryReadBeInt64(out value);

        /// <inheritdoc/>
        public bool TryReadUInt16(out ushort value) => TryReadBeUInt16(out value);

        /// <inheritdoc/>
        public bool TryReadUInt32(out uint value) => TryReadBeUInt32(out value);

        /// <inheritdoc/>
        public bool TryReadUInt64(out ulong value) => TryReadBeUInt64(out value);
    }
}
