#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
    // https://mentorembedded.github.io/cxx-abi/abi.html#mangling-type
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

            if (PeekAndDiscard('L'))
            {
                //$TODO: local name => in C, this is 'static'
            }
            else if (Peek('N'))
            {
                var qname = NestedName();
                SerializedType type = null;
                if (i < str.Length)
                {
                    type = new SerializedSignature
                    {
                        Arguments = Arguments()
                    };
                }
                return new StructField_v1
                {
                    Name = string.Join("::", qname),
                    Type = type,
                };
            }
            var name = UnscopedName();
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

        private List<string> NestedName()
        {
            Expect('N');
            var items = Prefix();
            var name = UnqualifiedName();
            name = string.Format(name, items != null ? items.LastOrDefault() : "");
            items.Add(name);
            Expect('E');
            return items;
        }

        private List<string> Prefix()
        {
            if (Peek('S'))
                return Substitution();
            throw new NotImplementedException();
        }

        private List<string> Substitution()
        {
            Expect('S');
            switch (str[i])
            {
            case 'o':
                ++i;
                return new List<string>
                {
                    "std", "ostream"
                };
            case 's':
                ++i;
                return new List<string>
                {
                    "std", "string"
                };
            }
            throw new NotImplementedException();
        }

        private string UnscopedName()
        {
            if (i < str.Length - 1 && str[i] == 'S' && str[i+1] == 't')
            {
                i += 2;
                return "std::" + UnqualifiedName();
            }
            return UnqualifiedName();
        }

        private string UnqualifiedName()
        {
            switch (str[i])
            {
            case 'l':
                if (str[i + 1] == 's')
                {
                    i += 2;
                    return "operator<<";
                }
                throw new NotImplementedException();
            case 'D':
                // D0	# deleting destructor
                // D1	# complete object destructor
                // D2	# base object destructor
                i += 2;
                return "~{0}";
            }
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

        private bool Peek(char ch)
        {
            return str[i] == ch;
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
            case 'z': ++i; return new Argument_v1 { Name = "...", Type = new VoidType_v1() };
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
            case 'R':
                //$TODO: Reko doesn't have a concept of 'const' or 'volatile'. 
                // Needs to be implemented for completeness, but should not affect
                // quality of decompilation.
                var qual = CvQualifier();
                return new ReferenceType_v1 { Referent = Type(), Size = ptrSize };
            case 'S':
                switch (str[i++])
                {
                case 't':
                    return new TypeReference_v1
                    {
                        Scope = new[] { "std" },
                        TypeName = Type().ToString(),
                    };
                case 's':
                    return new TypeReference_v1
                    {
                        Scope = new[] { "std" },
                        TypeName = "string"
                    };
                }
                throw new NotImplementedException();
            default:
                --i;
                if (char.IsDigit(str[i]))
                {
                    return new TypeReference_v1
                    {
                        TypeName = UnqualifiedName(),
                    };
                }
                throw new NotImplementedException(string.Format("Unknown GCC type code '{0}'.", str[i]));
            }
        }

        private char CvQualifier()
        {
            char c = str[i];
            if (c == 'K' ||  // const
                c == 'V' ||  // volatile
                c == 'r')    // restrict
            {
                ++i;
                return c;
            }
            return '\0';
        }
    }
}
