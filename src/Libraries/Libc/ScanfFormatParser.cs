using Reko.Core.CLanguage;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Analysis
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
        protected readonly int doubleSize;
        protected readonly int pointerSize;
        private IServiceProvider services;

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

            this.wordSize = platform.Architecture.WordWidth.Size;
            this.longSize = platform.GetByteSizeFromCBasicType(CBasicType.Long);
            this.doubleSize = platform.GetByteSizeFromCBasicType(CBasicType.Double);
            this.pointerSize = platform.PointerType.Size;
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
            int byteSize = this.wordSize;
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
            case 'e':
            case 'f':
            case 'g':
                byteSize = this.doubleSize;
                domain = Domain.Real;
                break;
            case 'p':
                byteSize = this.pointerSize;
                domain = Domain.Pointer;
                break;
            default:
                var el = this.services.RequireService<DecompilerEventListener>();
                el.Warn(
                    el.CreateAddressNavigator(program, addr),
                    "The format specifier '%{0}' passed to *scanf is not known.", cDomain);
                return new UnknownType();
            }
            return program.TypeFactory.CreatePointer(
                PrimitiveType.Create(domain, byteSize),
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
