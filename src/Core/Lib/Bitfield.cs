#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{
    public struct Bitfield
    {
        public readonly int Position;
        public readonly int Length;
        public readonly uint Mask;

        public Bitfield(int position, int length)
        {
            this.Position = position;
            this.Length = length;
            this.Mask = (1U << length) - 1U;
        }

        public Bitfield(int position, int length, uint mask)    
        {
            this.Position = position;
            this.Length = length;
            this.Mask = mask;
        }

        public uint Read(uint u)
        {
            return (u >> Position) & Mask;
        }

        public uint Read(ulong u)
        {
            return (uint)((u >> Position) & Mask);
        }

        public int ReadSigned(uint u)
        {
            var v = (u >> Position) & Mask;
            var m = 1u << (Length - 1);
            var s = (v ^ m) - m;
            return (int)s;
        }

        public static uint ReadFields(Bitfield[] bitfields, uint u)
        {
            uint n = 0;
            foreach (var bitfield in bitfields)
            {
                n = n << bitfield.Length | ((u >> bitfield.Position) & bitfield.Mask);
            }
            return n;
        }

        public static int ReadSignedFields(Bitfield[] fields, uint u)
        {
            int n = 0;
            int bitsTotal = 0;
            foreach (var bitfield in fields)
            {
                n = n << bitfield.Length | (int)((u >> bitfield.Position) & bitfield.Mask);
                bitsTotal += bitfield.Length;
            }
            n = n << (32 - bitsTotal);
            n = n >> (32 - bitsTotal);
            return n;
        }

        public static int ReadSignedFields(Bitfield[] fields, ulong ul)
        {
            int n = 0;
            int bitsTotal = 0;
            foreach (var bitfield in fields)
            {
                n = n << bitfield.Length | (int) ((ul >> bitfield.Position) & bitfield.Mask);
                bitsTotal += bitfield.Length;
            }
            n = n << (32 - bitsTotal);
            n = n >> (32 - bitsTotal);
            return n;
        }

        public override string ToString()
        {
            return $"[{Position}..{Position + Length})";
        }
    }
}
