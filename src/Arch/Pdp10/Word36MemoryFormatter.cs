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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Output;
using System;
using System.Text;

namespace Reko.Arch.Pdp10
{
    /// <summary>
    /// Specialized class for rendering <see cref="Word36MemoryArea"/>s.
    /// </summary>
    public class Word36MemoryFormatter : MemoryFormatter
    {
        public Word36MemoryFormatter(int unitsPerLine, int charsPerHexChunk, int charsPerTextChunk)
            : base(Pdp10Architecture.Word36, 1, unitsPerLine, charsPerHexChunk, charsPerTextChunk)
        {
        }

        protected override void DoRenderUnit(Address addr, Constant c, IMemoryFormatterOutput output)
        {
            var sUnit = Convert.ToString((long)(ulong) c.GetValue(), 8).PadLeft(12, '0');
            output.RenderUnit(addr, sUnit);
        }

        protected override string DoRenderAsText(ImageReader reader, int cUnits, Encoding enc)
        {
            var rdr = (Word36ImageReader) reader;
            var sb = new StringBuilder();
            for (int i = 0; i < cUnits; ++i)
            {
                if (rdr.TryReadBeUInt36(out ulong value))
                {
                    int shift = 4 * 7;
                    for (int j = 0; j < 5; ++j)
                    {
                        var c = (char) ((value >> shift) & 0x7F);
                        if (char.IsControl(c))
                            c = '.';
                        sb.Append(c);
                        shift -= 7;
                    }
                }
            }
            return sb.ToString();
        }
    }
}