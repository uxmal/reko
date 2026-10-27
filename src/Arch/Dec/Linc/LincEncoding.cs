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

using System.Text;

namespace Reko.Arch.Dec.Linc;

/// <summary>
/// Supports the LINC text encoding.
/// </summary>
public class LincEncoding
{
    public static LincEncoding Instance { get; } = new LincEncoding();

    public const string Ch77 = "ʸ";

    private static readonly string[] encoding =
    {
        "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
        "8",  "9",  "\n", "\b", " ",  "i",  "p",  "-",
        "+",  "|",  "#",  "\x0E","A", "B", "C",  "D",
        "E",  "F",  "G",  "H",  "I",  "J",  "K",  "L",
        "M",  "N",  "O",  "P",  "Q",  "R",  "S",  "T",
        "U",  "V",  "W",  "X",  "Y",  "Z",  "\x1B","→",
        "?",  "=",  "u",  ",",  ".",  "⊟",  "[",  "_",
        "\"", "„",  "<",  ">",  "]",  "ˣ",  ":",  Ch77
    };

    public LincEncoding()
    {
    }

    public string ConvertToUnicode(ushort[] words, int iWord, int cWord)
    {
        var sb = new StringBuilder();
        ushort w = 0;
        for (int i = 0; i < cWord * 2; ++i)
        {
            int b;
            if ((i & 1) == 0)
            {
                w = words[iWord + (i >> 1)];
                b = (w >> 6);
            }
            else
            {
                b = w;
            }
            b &= 0x3F;
            var s = b == 0x3F ? " " : encoding[b];
            sb.Append(s);
        }
        return sb.ToString();
    }


}
