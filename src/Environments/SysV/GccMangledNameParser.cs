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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.SysV
{
    public class GccMangledNameParser
    {
        private string str;
        private int i;
        private int ptrSize;

        public GccMangledNameParser(string parse, int ptrSize)
        {
            this.str = parse;
            this.ptrSize = ptrSize;
        }

        public string Modifier { get; set; }
        public string Scope { get; set; }

        public StructField_v1 Parse()
        {
            this.i = 0;
            if (!PeekAndDiscard('_'))
                return null;
            if (!PeekAndDiscard('Z'))
                return null;

            if (char.IsDigit(str[i]))
            {
                var name = SimpleName();
                var args = Arguments();
                return new StructField_v1
                {
                    Name = name,
                    Type = new SerializedSignature
                    {
                        Arguments = args
                    }
                };
            }
            var qname = QualifiedName();
            return new StructField_v1
            {
                Name = string.Join("::", qname)
            };

        }

        private List<string> QualifiedName()
        {
            Expect('N');
            var items = new List<string>();
            for (;;)
            {
                var item = SimpleName();
                if (item == null)
                    break;
                items.Add(item);
            }
            Expect('E');
            return items;
        }

        private string SimpleName()
        {
            int n = NameLength();
            var name = str.Substring(i, n);
            i += n;
            return name;
        }

        private int NameLength()
        {
            int n = 0;
            for (;;)
            {
                var ch = str[i++];
                if ('0' <= ch && ch <= '9')
                {
                    n = n * 10 + (ch - '0');
                }
                else
                {
                    --i;
                    return n;
                }
            }
        }

        private void Error(string format, params object[] args)
        {
            throw new FormatException(string.Format(format, args));
        }

        private void Expect(char ch)
        {
            if (str[i++] != ch)
                Error("Expected '{0}' but found '{1}'.", ch, str[i - 1]);
        }

        private bool PeekAndDiscard(char ch)
        {
            if (str[i] == ch)
            {
                ++i;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Argument_v1[] Arguments()
        {
            var args = new List<Argument_v1>();
            while (i < str.Length)
            {
                var arg = Argument();
                if (arg == null)
                    break;
                args.Add(arg);
            }
            return args.ToArray();
        }

        private Argument_v1 Argument()
        {
            switch (str[i])
            {
            case 'z': ++i; return new Argument_v1 { Name = "..." };
            case 'v': ++i; return null;
            default: return new Argument_v1 { Type = Type() };
            }
        }

        private SerializedType Type()
        {
            switch (str[i++])
            {
            case 'b': return PrimitiveType_v1.Bool();
            case 'c': return PrimitiveType_v1.Char8();
            case 'h': return PrimitiveType_v1.UChar8();
            case 's': return PrimitiveType_v1.Int16();
            case 'r': return PrimitiveType_v1.UInt16();
            case 'i': return PrimitiveType_v1.Int32();
            case 'j': return PrimitiveType_v1.UInt32();
            case 'l': return PrimitiveType_v1.Int64();
            case 'm': return PrimitiveType_v1.UInt64();
            case 'w': return PrimitiveType_v1.WChar16();
            case 'f': return PrimitiveType_v1.Real32();
            case 'd': return PrimitiveType_v1.Real64();
            case 'P': return new PointerType_v1 { DataType = Type(), PointerSize = ptrSize };
            //case 'R': return new ReferenceType_v1 { DataType = Type() };
            default:
                --i;
                if (char.IsDigit(str[i]))
                {
                    return new TypeReference_v1
                    {
                        TypeName = SimpleName(),
                    };
                }
                throw new NotImplementedException(string.Format("Unknown GCC type code '{0}'.", str[i]));
            }
        }
    }
}
