#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.CLanguage;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Libraries.Python
{
    public class PyBuildValueFormatParser : IVarargsFormatParser
    {
        private string format;
        private int pointerSize;

        private const int FLAG_SIZE_T = 1;

        private DataType dtInt;
        private DataType dtUInt;
        private DataType dtLong;
        private DataType dtULong;
        private DataType dtLongLong;
        private DataType dtULongLong;
        private DataType dtDouble;
        private DataType ptrChar;
        private DataType ptrVoid;

        private DataType dtPySize;
        private DataType ptrPyObject;
        private DataType ptrPyUnicode;
        private DataType ptrPyComplex;
        private DataType ptrPyConverter;

        public PyBuildValueFormatParser(
            Program program,
            Address addrInstr,
            string format,
            IServiceProvider services)
        {
            this.ArgumentTypes = new List<DataType>();
            this.format = format;
            var platform = program.Platform;
            this.pointerSize = platform.PointerType.BitSize;

            var wordSize = platform.Architecture.WordWidth.BitSize;
            var longSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Long) * DataType.BitsPerByte;
            var longLongSize = platform.GetByteSizeFromCBasicType(
                CBasicType.LongLong) * DataType.BitsPerByte;
            var doubleSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Double) * DataType.BitsPerByte;

            dtInt = Integer(wordSize);
            dtUInt = UInteger(wordSize);
            dtLong = Integer(longSize);
            dtULong = UInteger(longSize);
            dtLongLong = Integer(longLongSize);
            dtULongLong = UInteger(longLongSize);
            dtDouble = Real(doubleSize);
            ptrChar = Ptr(PrimitiveType.Char);
            ptrVoid = Ptr(VoidType.Instance);

            dtPySize = UInteger(pointerSize);
            ptrPyObject = Ptr(Ref("PyObject"));
            ptrPyUnicode = Ptr(Ref("Py_UNICODE"));
            ptrPyComplex = Ptr(Ref("Py_complex"));
            ptrPyConverter = Ptr(new CodeType());
        }

        public List<DataType> ArgumentTypes { get; private set; }

        private DataType Ptr(DataType pointee)
        {
            return new Pointer(pointee, pointerSize);
        }

        private DataType Ref(string name)
        {
            return new TypeReference(name, new UnknownType());
        }

        private DataType Integer(int size)
        {
            return PrimitiveType.Create(Domain.SignedInt, size);
        }

        private DataType UInteger(int size)
        {
            return PrimitiveType.Create(Domain.UnsignedInt, size);
        }

        private DataType Real(int size)
        {
            return PrimitiveType.Create(Domain.Real, size);
        }

        public void Parse()
        {
            ParseFormat(format + '\0', 0);
        }

        private void ParseFormat(string format, int flags)
        {
            int n = CountFormat(Enumerator(format), '\0');
            if (n == 0)
                return;
            if (n == 1)
                MakeValue(Enumerator(format), flags);
            else
                MakeList(Enumerator(format), '\0', n, flags);
        }

        private CharEnumerator Enumerator(string format)
        {
            var e = format.GetEnumerator();
            e.MoveNext();
            return e;
        }

        private char PopChar(CharEnumerator e)
        {
            var current = e.Current;
            e.MoveNext();
            return current;
        }

        private int CountFormat(CharEnumerator format, int endchar)
        {
            format = (CharEnumerator)format.Clone();
            int count = 0;
            int level = 0;
            while (level > 0 || format.Current != endchar)
            {
                switch (format.Current)
                {
                    case '\0':
                        /* Premature end */
                        throw new ApplicationException(
                            "Unmatched paren in format");
                    case '(':
                    case '[':
                    case '{':
                        if (level == 0)
                            count++;
                        level++;
                        break;
                    case ')':
                    case ']':
                    case '}':
                        level--;
                        break;
                    case '#':
                    case '&':
                    case ',':
                    case ':':
                    case ' ':
                    case '\t':
                        break;
                    default:
                        if (level == 0)
                            count++;
                        break;
                }
                format.MoveNext();
            }
            return count;
        }

        private void MakeList(
            CharEnumerator format,
            int endchar,
            int n,
            int flags)
        {
            int i;
            for (i = 0; i < n; i++)
                MakeValue(format, flags);

            if (format.Current != endchar)
                throw new ApplicationException("Unmatched paren in format");

            if (endchar != '\0')
                format.MoveNext();
        }

        private void MakeValue(CharEnumerator format, int flags)
        {
            for (;;)
            {
                int n;
                switch (PopChar(format))
                {
                    case '(':
                        n = CountFormat(format, ')');
                        MakeList(format, ')', n, flags);
                        return;

                    case '[':
                        n = CountFormat(format, ']');
                        MakeList(format, ']', n, flags);
                        return;

                    case '{':
                        n = CountFormat(format, '}');
                        MakeList(format, '}', n, flags);
                        return;
                    case 'b':
                    case 'B':
                    case 'h':
                    case 'i':
                        ArgumentTypes.Add(dtInt);
                        return;

                    case 'H':
                    case 'I':
                        ArgumentTypes.Add(dtUInt);
                        return;

                    case 'n':
                        ArgumentTypes.Add(dtPySize);
                        return;

                    case 'l':
                        ArgumentTypes.Add(dtLong);
                        return;

                    case 'k':
                        ArgumentTypes.Add(dtULong);
                        return;

                    case 'L':
                        ArgumentTypes.Add(dtLongLong);
                        return;

                    case 'K':
                        ArgumentTypes.Add(dtULongLong);
                        return;

                    case 'u':

                        ArgumentTypes.Add(ptrPyUnicode);
                        if (format.Current == '#')
                        {
                            format.MoveNext();
                            if ((flags & FLAG_SIZE_T) != 0)
                                ArgumentTypes.Add(dtPySize);
                            else
                                ArgumentTypes.Add(dtInt);
                        }
                        return;

                    case 'f':
                    case 'd':
                        ArgumentTypes.Add(dtDouble);
                        return;

                    case 'D':
                        ArgumentTypes.Add(ptrPyComplex);
                        return;

                    case 'c':
                        ArgumentTypes.Add(dtInt);
                        return;

                    case 's':
                    case 'z':
                        ArgumentTypes.Add(ptrChar);
                        if (format.Current == '#')
                        {
                            format.MoveNext();
                            if ((flags & FLAG_SIZE_T) != 0)
                                ArgumentTypes.Add(dtPySize);
                            else
                                ArgumentTypes.Add(dtInt);
                        }
                        return;

                    case 'N':
                    case 'S':
                    case 'O':
                        if (format.Current == '&')
                        {
                            ArgumentTypes.Add(ptrPyConverter);
                            ArgumentTypes.Add(ptrVoid);
                            format.MoveNext();
                        }
                        else
                            ArgumentTypes.Add(ptrPyObject);
                        return;

                    case ':':
                    case ',':
                    case ' ':
                    case '\t':
                        break;

                    default:
                        throw new ApplicationException(
                            "Bad format char passed to Py_BuildValue");

                }
            }
        }
    }
}
