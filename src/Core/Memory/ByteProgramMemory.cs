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
            throw new NotImplementedException();
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

        public bool TryReadInt8(Address addr, out byte b)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeInt16(Address addr, out short s)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeInt32(Address addr, out int i)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeInt64(Address addr, out long l)
        {
            throw new NotImplementedException();
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

        public bool TryReadUInt8(Address addr, out sbyte b)
        {
            throw new NotImplementedException();
        }

        public void TryWriteInt8(Address addr, sbyte b)
        {
            throw new NotImplementedException();
        }

        public void TryWriteUInt8(Address addr, byte b)
        {
            throw new NotImplementedException();
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
