#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Memory
{
    public class ProgramMemory : IMemory, IWriteableMemory
    {
        public ProgramMemory(SegmentMap segmentMap)
        {
            this.SegmentMap = segmentMap;
        }

        public SegmentMap SegmentMap { get; }

        public EndianImageReader CreateBeReader(Address addr)
        {
            throw new NotImplementedException();
        }

        public EndianImageReader CreateBeReader(Address addr, long cUnits)
        {
            throw new NotImplementedException();
        }

        public ImageWriter CreateBeWriter(Address addr)
        {
            throw new NotImplementedException();
        }

        public EndianImageReader CreateLeReader(Address addr)
        {
            throw new NotImplementedException();
        }

        public EndianImageReader CreateLeReader(Address addr, long cUnits)
        {
            throw new NotImplementedException();
        }

        public ImageWriter CreateLeWriter(Address addr)
        {
            throw new NotImplementedException();
        }

        public bool IsExecutable(Address addr)
        {
            throw new NotImplementedException();
        }

        public bool IsReadonly(Address addr)
        {
            throw new NotImplementedException();
        }

        public bool IsWriteable(Address addr)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c)
        {
            throw new NotImplementedException();
        }
    }
}
