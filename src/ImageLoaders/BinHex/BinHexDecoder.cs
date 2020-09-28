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
using System.IO;

namespace Reko.ImageLoaders.BinHex
{
    // Classic Mac resource header format http://developer.apple.com/legacy/mac/library/documentation/mac/MoreToolbox/MoreToolbox-99.html
    public class BinHexDecoder
    {
        private TextReader input;

        private readonly sbyte [] mpchartobits = new sbyte[] 
        {
            -1, -1, -1, -1, -1, -1, -1, -1,  -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1,  -1, -1, -1, -1, -1, -1, -1, -1, 
            -1,  0,  1,  2,  3,  4,  5,  6,   7,  8,  9, 10, 11, 12, -1, -1,
            13, 14, 15, 16, 17, 18, 19, -1,  20, 21, -1, -1, -1, -1, -1, -1,
            22, 23, 24, 25, 26, 27, 28, 29,  30, 31, 32, 33, 34, 35, 36, -1,
            37, 38, 39, 40, 41, 42, 43, -1,  44, 45, 46, 47, -1, -1, -1, -1,
            48, 49, 50, 51, 52, 53, 54, -1,  55, 56, 57, 58, 59, 60, -1, -1,
            61, 62, 63, -1, -1, -1, -1, -1,  -1, -1, -1, -1, -1, -1, -1, -1, 
        };

        public const string binhex4header = "(This file must be converted with BinHex 4.0)";

        public BinHexDecoder(TextReader input)
        {
            this.input = input;
            FindBinhexHeader();
            EatNewline();
            EatColon();
        }

        private void EatNewline()
        {
            while (input.Peek() >= 0)
            {
                char ch = (char)input.Peek();
                if (ch != '\r' && ch != '\n')
                    return;
                input.Read();
            }
        }

        public IEnumerable<byte> GetBytes()
        {
            int lastByte = -1;
            var unexpanded = GetUnexpandedBytes().GetEnumerator();
            while (unexpanded.MoveNext())
            {
                byte b = unexpanded.Current;
                if (b == 0x90 && lastByte != -1)
                {
                    if (unexpanded.MoveNext())
                    {
                        int count = unexpanded.Current;
                        if (count > 0)
                        {
                            for (int i = 0; i < count - 1; ++i)
                            {
                                yield return (byte) lastByte;
                            }
                            continue;
                        }
                        else
                        {
                            b = 0x90;
                        }
                    }
                }
                yield return b;
                lastByte = b;
            }
        }

        protected virtual IEnumerable<byte> GetUnexpandedBytes()
        {
            byte[] buf = new byte[6];
            byte bits;
            int completeBytes;
            do
            {
                completeBytes = 0;
                sbyte b = MapNextCharToBits();
                if (b >= 0)
                {
                    bits = (byte) (b << 2);
                    b = MapNextCharToBits();
                    if (b >= 0)
                    {
                        bits |= (byte) (b >> 4);
                        ++completeBytes;
                        yield return bits;
                        bits = (byte) ((b & 0xF) << 4);

                        b = MapNextCharToBits();
                        if (b >= 0)
                        {
                            bits |= (byte) (b >> 2);
                            ++completeBytes;
                            yield return bits;
                            bits = (byte) ((b & 0x3) << 6);
                            b = MapNextCharToBits();
                            if (b >= 0)
                            {
                                bits |= (byte) b;
                                ++completeBytes;
                                yield return bits;
                            }
                        }
                    }
                }
            } while (completeBytes == 3);
        }

        // [      ][      ][      ]
        // 012345670123456701234567
        // 012345012345012345012345
        // [    ][    ][    ][    ]

        private sbyte MapNextCharToBits()
        {
            for (; ; )
            {
                int ch = input.Read();
                if (ch == -1)
                    return -1;
                if (ch < mpchartobits.Length)
                {
                    sbyte sb = mpchartobits[ch];
                    if (sb < 0)
                        continue;
                        return sb;
                }
            }
        }

        private void EatColon()
        {
            Expect(':');
        }

        private void Expect(char ch)
        {
            int c = input.Read();
            if (c != ch)
                throw new FormatException(string.Format("Expected '{0}'.", (char) ch));
        }

        private void FindBinhexHeader()
        {
            int ch;

            int state = 0;
            int i = 0;
            for (; ; )
            {
                ch = input.Peek();
                if (ch == -1)
                    throw new FormatException("File doesn't appear to be in BinHex format.");
                switch (state)
                {
                case 0:
                    if (ch == ':')
                        return;
                    input.Read();
                    if (ch == '(')
                    {
                        i = 1;
                        state = 1;
                    }
                    break;

                case 1:
                    input.Read();
                    if (ch == '(')
                    {
                        i = 0;
                        break;
                    }
                    if (binhex4header[i] != ch)
                    {
                        state = 0;
                    }
                    else
                    {
                        ++i;
                        if (i == binhex4header.Length)
                            return;
                    }
                    break;
                }
            }
        }
    }
}