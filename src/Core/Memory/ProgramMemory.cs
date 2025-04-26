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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.Arm;

namespace Reko.Core.Memory
{
    public class ProgramMemory : IWriteableMemory
    {
        public ProgramMemory(SegmentMap segmentMap)
        {
            this.SegmentMap = segmentMap;
        }

        public SegmentMap SegmentMap { get; }

        /// <inheritdoc/>
        public ImageWriter? CreateBeWriter(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                return null;
            return segment.MemoryArea.CreateBeWriter(addr);
        }

        /// <inheritdoc/>
        public ImageWriter? CreateLeWriter(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                return null;
            return segment.MemoryArea.CreateBeWriter(addr);
        }

        /// <inheritdoc/>
        public bool IsExecutableAddress(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                return false;
            return segment.IsExecutable;
        }

        /// <inheritdoc/>
        public bool IsReadonly(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                return false;
            return segment.IsReadonly;
        }

        /// <inheritdoc/>
        public bool IsValidAddress(Address addr) => SegmentMap.IsValidAddress(addr);

        /// <inheritdoc/>
        public bool IsWriteable(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                return false;
            return segment.IsWriteable;
        }

        /// <inheritdoc/>
        public bool TryCreateBeReader(Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
            {
                rdr = null;
                return false;
            }
            rdr = segment.MemoryArea.CreateBeReader(addr);
            return true;
        }

        /// <inheritdoc/>
        public bool TryCreateBeReader(Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
            {
                rdr = null;
                return false;
            }
            rdr = segment.MemoryArea.CreateBeReader(addr, cUnits);
            return true;
        }

        /// <inheritdoc/>
        public bool TryCreateLeReader(Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
            {
                rdr = null;
                return false;
            }
            rdr = segment.MemoryArea.CreateLeReader(addr);
            return true;
        }

        /// <inheritdoc/>
        public bool TryCreateLeReader(Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
            {
                rdr = null;
                return false;
            }
            rdr = segment.MemoryArea.CreateLeReader(addr, cUnits);
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadBe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                c = null!;
                return false;
            }
            return segment.MemoryArea.TryReadBe(addr, dt, out c);
        }

        /// <inheritdoc/>
        public bool TryReadLe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out var segment))
            {
                c = null!;
                return false;
            }
            return segment.MemoryArea.TryReadLe(addr, dt, out c);
        }
    }
}
