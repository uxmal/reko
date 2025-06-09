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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.Trs80.Basic
{
    public class L2BasicReader : IEnumerable<L2BasicInstruction>
    {
        private readonly ByteMemoryArea bmem;
        private readonly ushort lineOffset;

        public L2BasicReader(ByteMemoryArea bmem, ushort lineOffset)
        {
            this.bmem = bmem;
            this.lineOffset = lineOffset;
        }

        public IEnumerator<L2BasicInstruction> GetEnumerator()
        {
            var instr = new L2BasicInstruction { NextAddress = lineOffset };
            for (;;)
            {
                instr = ReadLine(Address.Ptr16(instr.NextAddress));
                if (instr is null)
                    break;
                instr.Write(Console.Out);
                yield return instr;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public L2BasicInstruction? ReadLine(Address addr)
        {
            var rdr = bmem.CreateLeReader(addr);
            if (!rdr.TryReadLeUInt16(out ushort next))
                return null;
            if (!rdr.TryReadLeUInt16(out ushort line))
                return null;
            if (next < 0x0801)
                return null;
            var mem = new MemoryStream();
            while (rdr.TryReadByte(out byte b) && b != 0)
            {
                mem.WriteByte(b);
            }
            return new L2BasicInstruction
            {
                Address = Address.Ptr16(line),
                NextAddress = next,
                Line = mem.ToArray()
            };
        }
    }

}
