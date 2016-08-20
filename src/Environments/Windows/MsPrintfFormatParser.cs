#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    public class MsPrintfFormatParser : IVarargsFormatParser
    {
        private int i;
        private string format;
        private bool wideChars;
        private int wordSize;
        private int longSize;
        private int doubleSize;
        private int pointerSize;

        public MsPrintfFormatParser(
            string format,
            bool wideChars,
            int wordSize,
            int longSize,
            int doubleSize,
            int pointerSize)
        {
            this.ArgumentTypes = new List<DataType>();
            this.wideChars = wideChars;
            this.format = format;
            this.wordSize = wordSize;
            this.longSize = longSize;
            this.doubleSize = doubleSize;
            this.pointerSize = pointerSize;
        }

        public List<DataType> ArgumentTypes { get; private set; }

        public void Parse()
        {
            // %[flags] [width] [.precision] [{h | l | ll | w | I | I32 | I64}] type

            for (this.i = 0; i < format.Length; ++i)
            {
                if (format[i] != '%' || i == format.Length-1)
                    continue;
                char ch = format[++i];
                if (ch == '%')
                    continue;
                // Possible flag?
                SkipFlag();
                SkipNumber();
                if (i < format.Length && format[i] == '.')
                {
                    SkipNumber();
                }

                var byteSize = CollectSize();

                char domain = CollectDataType();

                DataType dt = MakeDataType(byteSize, domain);
                if (dt != null)
                    ArgumentTypes.Add(dt);
            }
        }

        enum PrintfSize
        {
            Default,
            HalfHalf,
            Half,
            Long,
            LongLong,
            I32,
            I64,
        }

        private DataType MakeDataType(PrintfSize size, char cDomain)
        {
            Domain domain = Domain.None;
            int byteSize = this.wordSize;
            switch (cDomain)
            {
            case 'c':
                if (wideChars)
                    return PrimitiveType.WChar;
                else
                    return PrimitiveType.Char;
            case 'C':
                if (wideChars)
                    return PrimitiveType.Char;
                else
                    return PrimitiveType.WChar;
            case 'o':
            case 'u':
            case 'x':
            case 'X':
                switch (size)
                {
                case PrintfSize.HalfHalf: byteSize = 1; break;
                case PrintfSize.Half: byteSize = 2; break;
                case PrintfSize.Long: byteSize = this.longSize; break;
                case PrintfSize.LongLong: byteSize = 8; break;
                case PrintfSize.I32: byteSize = 4; break;
                case PrintfSize.I64: byteSize = 8; break;
                }
                domain = Domain.UnsignedInt;
                break;
            case 'd':
            case 'i':
                switch (size)
                {
                case PrintfSize.HalfHalf: byteSize = 1; break;
                case PrintfSize.Half: byteSize = 2; break;
                case PrintfSize.Long: byteSize = this.longSize; break;
                case PrintfSize.LongLong: byteSize = 8; break;
                case PrintfSize.I32: byteSize = 4; break;
                case PrintfSize.I64: byteSize = 8; break;
                }
                domain = Domain.SignedInt;
                break;
            case 'a':
            case 'A':
            case 'e':
            case 'E':
            case 'f':
            case 'F':
            case 'g':
            case 'G':
                byteSize = this.doubleSize;
                domain = Domain.Real;
                break;
            case 'p':
                byteSize = this.pointerSize;
                domain = Domain.Pointer;
                break;
            }
            return PrimitiveType.Create(domain, byteSize);
        }

        private char CollectDataType()
        {
            if (i < format.Length)
                return format[i];
            else
                return '\0';
        }

        private PrintfSize CollectSize()
        {
            PrintfSize size = PrintfSize.Default;
            if (i < format.Length-1)
            {
                switch (format[i])
                {
                case 'h':
                    ++i;
                    size = PrintfSize.Half;
                    if (i < format.Length-1 && format[i] == 'h')
                    {
                        ++i;
                        size = PrintfSize.HalfHalf;
                    }
                    break;
                case 'l':
                    ++i;
                    size = PrintfSize.Long;
                    if (i < format.Length - 1 && format[i] == 'l')
                    {
                        ++i;
                        size = PrintfSize.LongLong;
                    }
                    break;
                case 'I':
                    if (i < format.Length - 3)
                    {
                        if (format[i + 1] == '3' && format[i + 2] == '2')
                        {
                            i += 3;
                            return PrintfSize.I32;
                        }
                        if (format[i + 1] == '6' && format[i + 2] == '4')
                        {
                            i += 3;
                            return PrintfSize.I64;
                        }
                    }
                    break;
                default:
                    return PrintfSize.Default;
                }
            }
            return size;
        }

        private void SkipNumber()
        {
            while (i < format.Length && Char.IsDigit(format[i]))
                ++i;
        }

        private void SkipFlag()
        {
            while (i < format.Length)
            {
                switch (format[i])
                {
                case '-':
                case '+':
                case '0':
                case '#':
                case ' ':
                    ++i;
                    break;
                default:
                    return;
                }
            }
        }
    }
}
