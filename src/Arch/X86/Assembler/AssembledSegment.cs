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

using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core.Types;

namespace Reko.Arch.X86.Assembler
{
    public class AssembledSegment
    {
        public IEmitter Emitter { get; } 
        public Symbol? Symbol { get; }
        public List<Relocation> Relocations { get; }
        public ushort Selector { get; set; }

        public AssembledSegment(IEmitter emitter, Symbol? sym)
        {
            this.Emitter = emitter;
            this.Symbol = sym;
            this.Relocations = new List<Relocation>();
        }

        public class Relocation
        {
            public Relocation(AssembledSegment seg, uint offset)
            {
                this.Segment = seg;
                this.Offset = offset;
            }

            public readonly AssembledSegment Segment;
            public readonly uint Offset;
        }

    }
}
