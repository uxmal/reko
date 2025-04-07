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

using Reko.Core.Loading;
using System;

namespace Reko.Core.Memory
{
    public class ByteProgramMemory : ProgramMemory, IByteWriteableMemory
    {
        public ByteProgramMemory(SegmentMap segmentMap)
            : base(segmentMap)
        {
        }

        public bool TryReadBeInt16(Address addr, out short s)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                s = 0;
                return false;
            }
            bool result = segment.MemoryArea.TryReadBeUInt16(addr, out var v);
            s = (short) v;
            return result;
        }

        public bool TryReadBeInt32(Address addr, out int value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            bool result = segment.MemoryArea.TryReadBeUInt32(addr, out var v);
            value = (int) v;
            return result;
        }

        public bool TryReadBeInt64(Address addr, out long value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            bool result = segment.MemoryArea.TryReadBeUInt64(addr, out var v);
            value = (long) v;
            return result;
        }

        public bool TryReadBeUInt16(Address addr, out ushort value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadBeUInt16(addr, out value);
        }

        public bool TryReadBeUInt32(Address addr, out uint value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadBeUInt32(addr, out value);
        }

        public bool TryReadBeUInt64(Address addr, out ulong value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadBeUInt64(addr, out value);
        }

        public bool TryReadInt8(Address addr, out sbyte value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            var offset = addr - segment.MemoryArea.BaseAddress;
            var retval = segment.MemoryArea.TryReadByte(offset, out byte v);
            value = (sbyte) v;
            return retval;
        }

        public bool TryReadLeInt16(Address addr, out short value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            var result = segment.MemoryArea.TryReadLeUInt16(addr, out var s);
            value = (short) s;
            return result;
        }

        public bool TryReadLeInt32(Address addr, out int value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            var result = segment.MemoryArea.TryReadLeUInt32(addr, out var u);
            value = (int) u;
            return result;
        }

        public bool TryReadLeInt64(Address addr, out long value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            var result = segment.MemoryArea.TryReadLeUInt64(addr, out var u);
            value = (long) u;
            return result;
        }

        public bool TryReadLeUInt16(Address addr, out ushort value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadLeUInt16(addr, out value);
        }

        public bool TryReadLeUInt32(Address addr, out uint value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadLeUInt32(addr, out value);
        }

        public bool TryReadLeUInt64(Address addr, out ulong value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            return segment.MemoryArea.TryReadLeUInt64(addr, out value);
        }

        public bool TryReadUInt8(Address addr, out byte value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                value = 0;
                return false;
            }
            var offset = addr - segment.MemoryArea.BaseAddress;
            return segment.MemoryArea.TryReadByte(offset, out value);
        }

        public void TryWriteInt8(Address addr, sbyte b)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteByte(addr - segment.MemoryArea.BaseAddress, (byte) b);
        }

        public void TryWriteUInt8(Address addr, byte b)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteByte(addr - segment.MemoryArea.BaseAddress, b);
        }

        public void WriteBeUInt16(Address addr, ushort value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteBeUInt16(addr - segment.MemoryArea.BaseAddress, value);
        }

        public void WriteBeUInt32(Address addr, uint value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteBeUInt32(addr - segment.MemoryArea.BaseAddress, value); 
        }

        public void WriteBeUInt64(Address addr, ulong value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteBeUInt64(addr - segment.MemoryArea.BaseAddress, value);
        }

        public void WriteLeUInt16(Address addr, ushort value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteLeUInt16(addr - segment.MemoryArea.BaseAddress, value);
        }

        public void WriteLeUInt32(Address addr, uint value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteLeUInt32(addr - segment.MemoryArea.BaseAddress, value);
        }

        public void WriteLeUInt64(Address addr, ulong value)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
                return;
            segment.MemoryArea.WriteLeUInt64(addr - segment.MemoryArea.BaseAddress, value);
        }

        private ImageSegment RequireSegment(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                throw new ArgumentException($"The address {addr} is invalid.");
            return segment;
        }
    }
}
