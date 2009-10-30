using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.ImageLoaders.BinHex
{
    public class BinHexDecoder
    {
        private TextReader input;
        // '!', '"', '#', '$', '%', '&', '\'', '(',  
        // ')', '*', '+', ',', '-', '0', '1', '2', 
        // '3', '4', '5', '6', '8', '9', '@', 'A', 
        // 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',

        // 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R',
        // 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '[', 
        // '`', 'a', 'b', 'c', 'd', 'e', 'f', 'h',
        // 'i', 'j', 'k', 'l', 'm', 'p', 'q', 'r' 
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

        private const string binhex4header = "(This file must be converted with BinHex 4.0)";

        public BinHexDecoder(TextReader input)
        {
            this.input = input;
            FindBinhexHeader();
            EatColon();
        }

        public IEnumerable<byte> GetBytes()
        {
            foreach (byte b in GetUnexpandedBytes())
            {
                yield return b;
            }
        }

        private IEnumerable<byte> GetUnexpandedBytes()
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
                    return mpchartobits[ch];
            }
        }

        private void EatColon()
        {
            Expect(':');
        }

        private void Expect(char ch)
        {
            if (input.Read() != ch)
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