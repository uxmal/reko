﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Machine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Parses a stream of bytes into Commoder BASIC instructions.
    /// </summary>
    public class C64BasicReader : IEnumerable<C64BasicInstruction>
    {
        private LoadedImage image;
        private ushort lineOffset;

        public C64BasicReader(LoadedImage image, ushort lineOffset)
        {
            this.image = image;
            this.lineOffset = lineOffset;
        }

        public IEnumerator<C64BasicInstruction> GetEnumerator()
        {
            var instr = new C64BasicInstruction { NextAddress = lineOffset };
            for (;;)
            {
                instr = ReadLine(Address.Ptr16(instr.NextAddress));
                if (instr == null)
                    break;
                instr.Write(Console.Out);
                yield return instr;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public C64BasicInstruction ReadLine(Address addr)
        {
            var rdr = image.CreateLeReader(addr);
            ushort next;
            ushort line; 
            byte b;
            if (!rdr.TryReadLeUInt16(out next))
                return null;
            if (!rdr.TryReadLeUInt16(out line))
                return null;
            if (next < 0x0801)
                return null;
            var mem = new MemoryStream();
            while (rdr.TryReadByte(out b) && b != 0)
            {
                mem.WriteByte(b);
            }
            return new C64BasicInstruction
            {
                Address = Address.Ptr16(line),
                NextAddress = next,
                Line = mem.ToArray()
            };
        }
    }
}
