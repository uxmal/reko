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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.CLanguage;
using Reko.Core.Types;

namespace Reko.Libraries.Python
{
    /// <summary>
    /// Parses format of PyArg_Parse and PyArg_ParseTuple.
    /// </summary>
    public class PyArgFormatParser : IVarargsFormatParser
    {
        private string format;
        private int pointerSize;

        private const int FLAG_SIZE_T = 1;

        private DataType dtInt;
        private DataType dtUInt;
        private DataType dtShort;
        private DataType dtUShort;
        private DataType dtLong;
        private DataType dtULong;
        private DataType dtLongLong;
        private DataType dtULongLong;
        private DataType dtFloat;
        private DataType dtDouble;
        private DataType ptrChar;
        private DataType ptrVoid;

        private DataType dtPySize;
        private DataType ptrPyObject;
        private DataType ptrPyTypeObject;
        private DataType ptrPyBuffer;
        private DataType ptrPyUnicode;
        private DataType ptrPyComplex;
        private DataType ptrPyConverter;
        private DataType ptrPyInquiry;

        public PyArgFormatParser(
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
            var shortSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Short) * DataType.BitsPerByte;
            var longSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Long) * DataType.BitsPerByte;
            var longLongSize = platform.GetByteSizeFromCBasicType(
                CBasicType.LongLong) * DataType.BitsPerByte;
            var floatSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Float) * DataType.BitsPerByte;
            var doubleSize = platform.GetByteSizeFromCBasicType(
                CBasicType.Double) * DataType.BitsPerByte;

            dtInt = Integer(wordSize);
            dtUInt = UInteger(wordSize);
            dtShort = Integer(shortSize);
            dtUShort = UInteger(shortSize);
            dtLong = Integer(longSize);
            dtULong = UInteger(longSize);
            dtLongLong = Integer(longLongSize);
            dtULongLong = UInteger(longLongSize);
            dtFloat = Real(floatSize);
            dtDouble = Real(doubleSize);
            ptrChar = Ptr(PrimitiveType.Char);
            ptrVoid = Ptr(VoidType.Instance);

            dtPySize = UInteger(pointerSize);
            ptrPyObject = Ptr(Ref("PyObject"));
            ptrPyTypeObject = Ptr(Ref("PyTypeObject"));
            ptrPyBuffer = Ptr(Ref("Py_buffer"));
            ptrPyUnicode = Ptr(Ref("Py_UNICODE"));
            ptrPyComplex = Ptr(Ref("Py_complex"));
            ptrPyConverter = Ptr(new CodeType());
            ptrPyInquiry = Ptr(new CodeType());
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

        private DataType Integer(int bitSize)
        {
            return PrimitiveType.Create(Domain.SignedInt, bitSize);
        }

        private DataType UInteger(int bitSize)
        {
            return PrimitiveType.Create(Domain.UnsignedInt, bitSize);
        }

        private DataType Real(int bitSize)
        {
            return PrimitiveType.Create(Domain.Real, bitSize);
        }

        public void Parse()
        {
            ParseFormat(format + '\0', 0);
        }

        private void ParseFormat(string format, int flags)
        {
            var len = CountFormat(Enumerator(format));

            var e = Enumerator(format);
            for (int i = 0; i < len; i++)
            {
                if (e.Current == '|')
                    e.MoveNext();
                ConvertItem(e, flags);
            }

            if (e.Current != '\0' && !char.IsLetter(e.Current) &&
                e.Current != '(' &&
                e.Current != '|' && e.Current != ':' && e.Current != ';')
            {
                throw new ApplicationException("Bad format string");
            }
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

        private int CountFormat(CharEnumerator format)
        {
            int count = 0;
            int level = 0;
            int endfmt = 0;
            while (endfmt == 0)
            {
                var c = PopChar(format);
                switch (c)
                {
                    case '(':
                        if (level == 0)
                            count++;
                        level++;
                        break;
                    case ')':
                        if (level == 0)
                            throw new ApplicationException(
                                "Excess ')' in getargs format");
                        else
                            level--;
                        break;
                    case '\0':
                        endfmt = 1;
                        break;
                    case ':':
                        endfmt = 1;
                        break;
                    case ';':
                        endfmt = 1;
                        break;
                    default:
                        if (level == 0)
                        {
                            if (char.IsLetter(c))
                            {
                                if (c != 'e') /* skip encoded */
                                    count++;
                            }
                        }
                        break;
                }
            }

            if (level != 0)
                throw new ApplicationException(
                    "Missing ')' in getargs format");
            return count;
        }

        private void ConvertItem(CharEnumerator format, int flags)
        {
            if (format.Current == '(')
            {
                format.MoveNext();
                ConvertTuple(format, flags);
                format.MoveNext();
            }
            else
            {
                ConvertSimple(format, flags);
            }
        }

        private void ConvertTuple(CharEnumerator format, int flags)
        {
            int level = 0;
            int n = 0;
            var formatsave = format;
            format = (CharEnumerator)format.Clone();

            for (;;)
            {
                var c = PopChar(format);
                if (c == '(')
                {
                    if (level == 0)
                        n++;
                    level++;
                }
                else if (c == ')')
                {
                    if (level == 0)
                        break;
                    level--;
                }
                else if (c == ':' || c == ';' || c == '\0')
                    break;
                else if (level == 0 && char.IsLetter(c))
                    n++;
            }

            format = formatsave;
            for (int i = 0; i < n; i++)
            {
                ConvertItem(format, flags);
            }
        }

        private void ConvertSimple(CharEnumerator format, int flags)
        {
            switch (PopChar(format))
            {
                case 'b': /* unsigned byte -- very short int */
                case 'B': /* byte sized bitfield - both signed and unsigned
                             values allowed */
                    ArgumentTypes.Add(ptrChar);
                    break;

                case 'h': /* signed short int */
                    ArgumentTypes.Add(Ptr(dtShort));
                    break;

                case 'H': /* short int sized bitfield, both signed and
                             unsigned allowed */
                    ArgumentTypes.Add(Ptr(dtUShort));
                    break;

                case 'i': /* signed int */
                    ArgumentTypes.Add(Ptr(dtInt));
                    break;

                case 'I': /* int sized bitfield, both signed and
                             unsigned allowed */
                    ArgumentTypes.Add(Ptr(dtUInt));
                    break;

                case 'n': /* Py_ssize_t */
                    ArgumentTypes.Add(Ptr(dtPySize));
                    break;

                case 'l': /* long int */
                    ArgumentTypes.Add(Ptr(dtLong));
                    break;

                case 'k': /* long sized bitfield */
                    ArgumentTypes.Add(Ptr(dtULong));
                    break;

                case 'L': /* PY_LONG_LONG */
                    ArgumentTypes.Add(Ptr(dtLongLong));
                    break;

                case 'K': /* long long sized bitfield */
                    ArgumentTypes.Add(Ptr(dtULongLong));
                    break;

                case 'f': /* float */
                    ArgumentTypes.Add(Ptr(dtFloat));
                    break;

                case 'd': /* double */
                    ArgumentTypes.Add(Ptr(dtDouble));
                    break;

                case 'D': /* complex double */
                    ArgumentTypes.Add(ptrPyComplex);
                    break;

                case 'c': /* char */
                    ArgumentTypes.Add(ptrChar);
                    break;

                case 's': /* string */
                case 'z': /* string, may be NULL (None) */
                    if (format.Current == '*')
                    {
                        ArgumentTypes.Add(ptrPyBuffer);
                        format.MoveNext();
                    } else if (format.Current == '#')
                    {
                        ArgumentTypes.Add(Ptr(ptrChar));
                        if ((flags & FLAG_SIZE_T) != 0)
                            ArgumentTypes.Add(Ptr(dtPySize));
                        else
                            ArgumentTypes.Add(Ptr(dtInt));

                        format.MoveNext();
                    } else
                    {
                        ArgumentTypes.Add(Ptr(ptrChar));
                    }
                    break;

                case 'e': /* encoded string */
                    ArgumentTypes.Add(ptrChar);

                    /* Get output buffer parameter:
                       's' (recode all objects via Unicode) or
                       't' (only recode non-string objects)
                    */
                    if (format.Current != 's' && format.Current == 't')
                        throw new ApplicationException(
                            "Unknown parser marker combination");
                    ArgumentTypes.Add(Ptr(ptrChar));
                    format.MoveNext();

                    if (format.Current == '#')
                    {
                        /* Using buffer length parameter '#' */
                        if ((flags & FLAG_SIZE_T) != 0)
                            ArgumentTypes.Add(Ptr(dtPySize));
                        else
                            ArgumentTypes.Add(Ptr(dtInt));

                        format.MoveNext();
                    }
                    break;

                case 'u': /* raw unicode buffer (Py_UNICODE *) */
                    if (format.Current == '#')
                    { 
                        ArgumentTypes.Add(Ptr(ptrChar));
                        if ((flags & FLAG_SIZE_T) != 0)
                            ArgumentTypes.Add(Ptr(dtPySize));
                        else
                            ArgumentTypes.Add(Ptr(dtInt));
                        format.MoveNext();
                    }
                    else
                    {
                        ArgumentTypes.Add(Ptr(ptrPyUnicode));
                    }
                    break;

                case 'S': /* string object */
                case 'U': /* Unicode object */
                    ArgumentTypes.Add(Ptr(ptrPyObject));
                    break;

                case 'O': /* object */
                    if (format.Current == '!')
                    {
                        ArgumentTypes.Add(ptrPyTypeObject);
                        ArgumentTypes.Add(Ptr(ptrPyObject));
                        format.MoveNext();
                    }
                    else if (format.Current == '?')
                    {
                        ArgumentTypes.Add(ptrPyInquiry);
                        ArgumentTypes.Add(Ptr(ptrPyObject));
                        format.MoveNext();
                    }
                    else if (format.Current == '&')
                    {
                        ArgumentTypes.Add(ptrPyConverter);
                        ArgumentTypes.Add(ptrVoid);
                        format.MoveNext();
                    }
                    else
                    {
                        ArgumentTypes.Add(Ptr(ptrPyObject));
                    }
                    break;

                case 'w': /* memory buffer, read-write access */
                    ArgumentTypes.Add(Ptr(ptrVoid));

                    if (format.Current == '*')
                    {
                        format.MoveNext();
                    }
                    else if (format.Current == '#')
                    {
                        if ((flags & FLAG_SIZE_T) != 0)
                            ArgumentTypes.Add(Ptr(dtPySize));
                        else
                            ArgumentTypes.Add(Ptr(dtInt));
                        format.MoveNext();
                    }
                    break;

                case 't': /* 8-bit character buffer, read-only access */
                    ArgumentTypes.Add(Ptr(ptrChar));

                    if (format.Current != '#')
                        throw new ApplicationException(
                            "Invalid use of 't' format character");

                    format.MoveNext();

                    if ((flags & FLAG_SIZE_T) != 0)
                        ArgumentTypes.Add(Ptr(dtPySize));
                    else
                        ArgumentTypes.Add(Ptr(dtInt));

                    break;

                default:
                    throw new ApplicationException("Bad format char");
            }
        }
    }
}
