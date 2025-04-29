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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// A big-endian image reader for a memory area organized in 64-bit words.
    /// </summary>
    public class Word64BeReader : Word64ImageReader, EndianImageReader
    {
        /// <summary>
        /// Constructs a reader on a memory area, starting at the given offset.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="offset">An offset within the words of the memory area.</param>
        public Word64BeReader(Word64MemoryArea mem, long offset = 0) : base(mem, offset)
        {
        }

        /// <summary>
        /// Constructs a reader on a memory area, starting at the given offset.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="addr">An address within the words of the memory area.</param>
        public Word64BeReader(Word64MemoryArea mem, Address addr) : base(mem, addr)
        {
        }

        /// <summary>
        /// Constructs a reader on the memory area, starting at the given offset,
        /// and reading at most <paramref name="cUnits"/> words.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="addr">An address within the words of the memory area.</param>
        /// <param name="cUnits">Maximum number of words to read before stopping.</param>
        public Word64BeReader(Word64MemoryArea mem, Address addr, long cUnits) : base(mem, addr, cUnits)
        {
        }
        
        /// <inheritdoc/>
        public EndianImageReader Clone()
        {
            return new Word64BeReader(mem, Offset);
        }

        /// <inheritdoc/>
        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public string ReadNulTerminatedString(DataType charType, Encoding enc)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public short ReadInt16()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public int ReadInt32()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public long ReadInt64()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ReadNullCharTerminator(DataType dtChar)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort ReadUInt16()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public uint ReadUInt32()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong ReadUInt64()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekUInt32(int offset, out uint value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekUInt64(int offset, out ulong value) => TryPeekBeUInt64(offset, out value);

        /// <inheritdoc/>
        public bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            value = default!;
            return false;
        }

        /// <inheritdoc/>
        public bool TryReadInt16(out short _) => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public bool TryReadInt32(out int value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadInt64(out long value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadUInt16(out ushort value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadUInt32(out uint value)
        {
            if (!mem.TryReadBeUInt32(Offset, out value))
                return false;
            Offset += 1;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadUInt64(out ulong value)
        {
            throw new System.NotImplementedException();
        }
    }
}