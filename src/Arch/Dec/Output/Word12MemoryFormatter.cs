#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Dec.Linc;
using Reko.Arch.Dec.Memory;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Output;
using System;
using System.Text;

namespace Reko.Arch.Dec.Output;

/// <summary>
/// Formats PDP-12 memory words for display as fixed-width octal values and as decoded text.
/// </summary>
/// <remarks>Represents each 12-bit word as a 4-character octal unit. Text rendering uses LINC encoding and
/// substitutes control characters, surrogate code points, and private-use characters in the U+E000 to U+E0FF range with
/// '.'.</remarks>
public class Word12MemoryFormatter : MemoryFormatter
{
    public Word12MemoryFormatter(int chunksPerLine, int charsPerNumericChunk, int charsPerTextChunk) : 
        base(PdpTypes.Word12, 1, chunksPerLine, charsPerNumericChunk, charsPerTextChunk)
    {
    }

    /// <inheritdoc/>
    protected override void DoRenderUnit(Address addr, Constant c, IMemoryFormatterOutput output)
    {
        var sUnit = Convert.ToString((long) c.ToUInt16(), 8).PadLeft(4, '0');
        output.RenderUnit(addr, sUnit);
    }


    /// <inheritdoc/>
    protected override void DoRenderAsText(ImageReader rdr, int cUnits, Encoding enc, IMemoryFormatterOutput output)
    {
        var r = (Word12BeImageReader) rdr;
        ushort [] words = r.ReadWords(cUnits);

        var s = LincEncoding.Instance.ConvertToUnicode(words, 0, words.Length);
        var sb = new StringBuilder();
        foreach (var c in s)
        {
            char ch = c;
            if (char.IsControl(ch) || char.IsSurrogate(ch) || (0xE000 <= ch && ch <= 0xE0FF))
                ch = '.';
            sb.Append(ch);
        }
        output.RenderUnitAsText(rdr.Address, sb.ToString());
    }
}
