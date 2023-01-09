#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This class represents a memory area where the contents are accessed by 16-bit words.
    /// </summary>
    public class Word16MemoryArea : MemoryArea
    {
        public Word16MemoryArea(Address addrBase, ushort[] words)
            : base(addrBase, words.Length, 16, new MemoryFormatter(PrimitiveType.Word16, 1, 8, 4, 2))
        {
            this.Words = words;
        }

        public static MemoryArea CreateFromBeBytes(Address address, byte[] bytes)
        {
            static ushort[] CreateFromBeBytes(byte[] bytes)
            {
                var words = new ushort[bytes.Length / 2];
                for (int i = 0; i < words.Length; ++i)
                {
                    words[i] = (ushort) ((bytes[i * 2] << 8) | bytes[i * 2 + 1]);
                }
                return words;
            }
            var words = CreateFromBeBytes(bytes);
            return new Word16MemoryArea(address, words);
        }

        public static MemoryArea CreateFromLeBytes(Address address, byte[] bytes)
        {
            static ushort[] CreateFromLeBytes(byte[] bytes)
            {
                var words = new ushort[bytes.Length / 2];
                for (int i = 0; i < words.Length; ++i)
                {
                    words[i] = (ushort) ((bytes[i * 2 + 1] << 8) | bytes[i * 2]);
                }
                return words;
            }
            var words = CreateFromLeBytes(bytes);
            return new Word16MemoryArea(address, words);
        }

        /// <summary>
        /// The words of this memory area.
        /// </summary>
        public ushort[] Words { get; }

        public override EndianImageReader CreateBeReader(Address addr) => new Word16BeImageReader(this, addr);
        public override EndianImageReader CreateBeReader(Address addr, long cUnits) => new Word16BeImageReader(this, addr, cUnits);
        public override EndianImageReader CreateBeReader(long offset) => new Word16BeImageReader(this, offset);
        public override EndianImageReader CreateBeReader(long beginOffset, long endOffset) => new Word16BeImageReader(this, beginOffset, endOffset);
        public override BeImageWriter CreateBeWriter(Address addr) => throw new NotImplementedException();
        public override BeImageWriter CreateBeWriter(long offset) => throw new NotImplementedException();

        public override EndianImageReader CreateLeReader(Address addr) => new Word16LeImageReader(this, addr);
        public override EndianImageReader CreateLeReader(Address addr, long cUnits) => new Word16LeImageReader(this, addr, cUnits);
        public override EndianImageReader CreateLeReader(long offset) => new Word16LeImageReader(this, offset);
        public override EndianImageReader CreateLeReader(long beginOffset, long endOffset) => new Word16LeImageReader(this, beginOffset, endOffset);
        public override LeImageWriter CreateLeWriter(Address addr) => throw new NotImplementedException();
        public override LeImageWriter CreateLeWriter(long offset) => throw new NotImplementedException();

        public uint ReadBeUInt32(long offset)
        {
            var hi = (uint)Words[offset];
            var lo = (uint)Words[offset + 1];
            return (hi << 16) | lo;
        }

        public ulong ReadBeUInt64(long offset)
        {
            var w0 = (ulong) Words[offset];
            var w1 = (ulong) Words[offset + 1];
            var w2 = (ulong) Words[offset + 2];
            var w3 = (ulong) Words[offset + 3];
            return (w0 << 48) | (w1 << 32) | (w2 << 16) | w3;
        }


        public override string ToString()
        {
            return string.Format("Image {0}{1} - length {2} 16-bit words {3}", "{", BaseAddress, this.Length, "}");
        }


        public override bool TryReadByte(long off, out byte b)
        {
            throw new NotSupportedException("Byte reads are not supported.");
        }

        public override bool TryReadBe(long imageOffset, DataType type, out Constant c)
        {
            throw new NotImplementedException();
        }

        public override bool TryReadBeUInt16(long off, out ushort retvalue)
        {
            throw new NotImplementedException();
        }

        public override bool TryReadBeUInt32(long off, out uint retvalue)
        {
            throw new NotImplementedException();
        }

        public override bool TryReadBeUInt64(long off, out ulong retvalue)
        {
            throw new NotImplementedException();
        }

        public override bool TryReadLe(long imageOffset, DataType type, [MaybeNullWhen(false)] out Constant c)
        {
            switch (type.BitSize)
            {
            case 8:
                if (!TryReadLeUInt16(imageOffset, out var us))
                {
                    c = default;
                    return false;
                }
                c = Constant.Create(type, us);
                return true;
            case 16:
                if (!TryReadLeUInt16(imageOffset, out us))
                {
                    c = default;
                    return false;
                }
                c = Constant.Create(type, us);
                return true;
            }
            throw new NotImplementedException();
        }

        public override bool TryReadLeInt32(long off, out int retvalue)
        {
            throw new NotImplementedException();
        }

        public override bool TryReadLeUInt16(long off, out ushort retvalue)
        {
            if (off >= Words.Length)
            {
                retvalue = 0;
                return false;
            }
            retvalue = Words[off];
            return true;
        }

        public override bool TryReadLeUInt32(long off, out uint retvalue)
        {
            if (off + 1 >= Words.Length)
            {
                retvalue = 0;
                return false;
            }
            var hi = (uint) Words[off];
            var lo = (uint) Words[off + 1];
            retvalue = (hi << 16) | lo;
            return true;
        }

        public override bool TryReadLeUInt64(long off, out ulong retvalue)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(long off, byte value)
        {
            throw new NotSupportedException("Writing individual bytes is not supported.");
        }

        public override void WriteBeUInt16(long off, ushort value)
        {
            throw new NotImplementedException();
        }

        public override void WriteBeUInt32(long off, uint value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLeUInt16(long off, ushort value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLeUInt32(long off, uint value)
        {
            throw new NotImplementedException();
        }
    }
}
