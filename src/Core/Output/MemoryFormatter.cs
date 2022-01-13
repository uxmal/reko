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

using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Output
{
    public class MemoryFormatter
    {
        private readonly PrimitiveType dtUnit;
        private readonly int unitSize;
        private readonly uint unitsPerLine;
        private readonly string unitFormat;
        private readonly int bytesPerUnit;

        public MemoryFormatter(PrimitiveType dtUnit, int unitsPerLine, int bytesPerUnit)
        {
            this.dtUnit = dtUnit;
            this.unitSize = dtUnit.Size;
            this.unitsPerLine = (uint)unitsPerLine;
            this.unitFormat = "{0:" + $"X{unitSize * 2}" + "}";
            this.bytesPerUnit = bytesPerUnit;
        }

        public void RenderMemory(EndianImageReader rdr, Encoding enc, IMemoryFormatterOutput output)
        {
            while (RenderLine(rdr, enc, output))
                ;
        }

        public bool RenderLine(EndianImageReader rdr, Encoding enc, IMemoryFormatterOutput output)
        {
            output.BeginLine();
            var offStart = rdr.Offset;
            var addr = rdr.Address;
            output.RenderAddress(addr);

            var addrStart = Align(addr, unitsPerLine);
            var prePaddingUnits = (int) (addr - addrStart);
            var offsetEndLine = (rdr.Offset - prePaddingUnits) + unitsPerLine;

            output.RenderFillerSpan(PaddingCells(prePaddingUnits));

            bool moreData = true;
            int postPaddingUnits = 0;
            while (moreData && rdr.Offset < offsetEndLine)
            {
                moreData = rdr.TryRead(dtUnit, out var c);
                if (moreData)
                {
                    output.RenderUnit(string.Format(unitFormat, c.GetValue()));
                }
                else
                {
                    postPaddingUnits = (int) (offsetEndLine - rdr.Offset);
                }
            }

            output.RenderFillerSpan(PaddingCells(postPaddingUnits));

            var cb = rdr.Offset - offStart;
            rdr.Offset = offStart;
            var bytes = rdr.ReadBytes((int) cb);

            string sBytes = RenderAsText(enc, bytes);
            output.RenderUnitsAsText(prePaddingUnits * this.bytesPerUnit, sBytes, postPaddingUnits * this.bytesPerUnit);

            output.EndLine(bytes);

            return moreData && rdr.IsValid;
        }

        private int PaddingCells(int addressDifference)
        {
            var bytes = (addressDifference * bytesPerUnit);
            return (1 + 2 * unitSize) * (bytes / unitSize);
        }

        private string RenderAsText(Encoding enc, byte[] abCode)
        {
            var chars = enc.GetChars(abCode);
            for (int i = 0; i < chars.Length; ++i)
            {
                char ch = chars[i];
                if (char.IsControl(ch) || char.IsSurrogate(ch) || (0xE000 <= ch && ch <= 0xE0FF))
                    chars[i] = '.';
            }
            return new string(chars);
        }

        private static ulong Align(ulong ul, uint alignment)
        {
            return alignment * (ul / alignment);
        }

        private static Address Align(Address addr, uint alignment)
        {
            var lin = addr.ToLinear();
            var linAl = Align(lin, alignment);
            return addr - (int) (lin - linAl);
        }
    }
}
