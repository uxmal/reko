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
using Reko.Core.Analysis;
using Reko.Core.Hll.C;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Libraries.Libc
{ 
    /// <summary>
    /// Parses standard C printf format
    /// </summary>
    public class ScanfFormatParser : IVarargsFormatParser
    {
        protected Program program;
        protected Address addr;
        protected int i;
        protected string format;
        protected readonly int wordSize;
        protected readonly int longSize;
        protected readonly int floatSize;
        protected readonly int doubleSize;
        protected readonly int pointerSize;
        private readonly IServiceProvider services;

        public ScanfFormatParser(
            Program program,
            Address addrInstr,
            string format,
            IServiceProvider services)
        {
            this.ArgumentTypes = new List<DataType>();
            this.program = program;
            this.addr = addrInstr;
            this.format = format;
            var platform = program.Platform;

            this.wordSize = platform.Architecture.WordWidth.BitSize;
            this.longSize = platform.GetBitSizeFromCBasicType(CBasicType.Long);
            this.floatSize = platform.GetBitSizeFromCBasicType(CBasicType.Float);
            this.doubleSize = platform.GetBitSizeFromCBasicType(CBasicType.Double);
            this.pointerSize = platform.PointerType.BitSize;
            this.services = services;
        }

        public List<DataType> ArgumentTypes { get; private set; }

        public void Parse()
        {
            // %[flags] [width] [.precision] [{h | l | ll | w | I | I32 | I64}] type

            for (this.i = 0; i < format.Length; ++i)
            {
                if (format[i] != '%' || i == format.Length - 1)
                    continue;
                char ch = format[++i];
                if (ch == '%')
                    continue;
                bool ignoreArgument = false;
                if (ch == '*')
                {
                    ignoreArgument = true;
                    ++i;
                }
                // Possible flag?
                SkipFlag();
                SkipNumber();
                if (i < format.Length && format[i] == '.')
                {
                    SkipNumber();
                }

                var byteSize = CollectSize();

                char domain = CollectDataType();

                if (!ignoreArgument)
                {
                    DataType dt = MakeDataType(byteSize, domain);
                    if (dt is not null)
                        ArgumentTypes.Add(dt);
                }
            }
        }

        protected enum PrintfSize
        {
            Default,
            HalfHalf,
            Half,
            Long,
            LongLong,
            I32,
            I64,
        }

        protected virtual DataType MakeDataType(PrintfSize size, char cDomain)
        {
            Domain domain = Domain.None;
            int bitSize = this.wordSize;
            switch (cDomain)
            {
            case 'c':
            case 's':
                return program.TypeFactory.CreatePointer(
                    size == PrintfSize.Long ? PrimitiveType.WChar : PrimitiveType.Char,
                    pointerSize);
            case 'o':
            case 'u':
            case 'x':
                switch (size)
                {
                case PrintfSize.HalfHalf: bitSize = 8; break;
                case PrintfSize.Half: bitSize = 16; break;
                case PrintfSize.Long: bitSize = this.longSize; break;
                case PrintfSize.LongLong: bitSize = 64; break;
                case PrintfSize.I32: bitSize = 32; break;
                case PrintfSize.I64: bitSize = 64; break;
                }
                domain = Domain.UnsignedInt;
                break;
            case 'd':
            case 'i':
                switch (size)
                {
                case PrintfSize.HalfHalf: bitSize = 8; break;
                case PrintfSize.Half: bitSize = 16; break;
                case PrintfSize.Long: bitSize = this.longSize; break;
                case PrintfSize.LongLong: bitSize = 64; break;
                case PrintfSize.I32: bitSize = 32; break;
                case PrintfSize.I64: bitSize = 64; break;
                }
                domain = Domain.SignedInt;
                break;
            case 'a':
            case 'e':
            case 'f':
            case 'g':
                if (size == PrintfSize.Long)
                    bitSize = this.doubleSize;
                else 
                    bitSize = this.floatSize;
                domain = Domain.Real;
                break;
            case 'p':
                bitSize = this.pointerSize;
                domain = Domain.Pointer;
                break;
            default:
                var el = this.services.RequireService<IEventListener>();
                el.Warn(
                    el.CreateAddressNavigator(program, addr),
                    "The format specifier '%{0}' passed to *scanf is not known.", cDomain);
                return new UnknownType();
            }
            return program.TypeFactory.CreatePointer(
                PrimitiveType.Create(domain, bitSize),
                pointerSize);
        }

        private char CollectDataType()
        {
            if (i < format.Length)
                return format[i];
            else
                return '\0';
        }

        protected virtual PrintfSize CollectSize()
        {
            PrintfSize size = PrintfSize.Default;
            if (i < format.Length - 1)
            {
                switch (format[i])
                {
                case 'h':
                    ++i;
                    size = PrintfSize.Half;
                    if (i < format.Length - 1 && format[i] == 'h')
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
