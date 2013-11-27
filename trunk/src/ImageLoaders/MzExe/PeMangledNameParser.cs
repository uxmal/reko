#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.MzExe
{
    // http://www.kegel.com/mangle.html
    // http://www.agner.org/optimize/calling_conventions.pdf
    public class PeMangledNameParser
    {
        private int idx;
        private string str;
        private List<SerializedTypeReference> namesSeen;
        private Stack<string> scopes;

        public SerializedProcedure Parse(string mangled)
        {
            this.idx = 0;
            this.str = mangled;
            this.namesSeen = new List<SerializedTypeReference>();
            this.scopes = new Stack<string>();

            if (idx >= str.Length)
                throw new InvalidOperationException();
            if (str[idx++] == '?')
            {
                // C++ identifier
                string fnName = null;
                if (str[idx] == '_' || Char.IsLetter(str[idx]))
                {
                    fnName = ParseName();
                    Expect('@');
                }
                else if (str[idx] == '?')
                {
                    fnName = ParseSpecialName();
                }
                else
                    throw new NotImplementedException();
                var tyRef = this.ParseFullyQualifiedTypeName();

                return ParseTypeInformation(tyRef.Scope != null ? string.Join(":", tyRef.Scope) : "", fnName);
            }
            return null;
        }

        public class TemplateArg
        {
        }
        private string EatScope()
        {
            if (str[idx] == '?' && str[idx + 1] == '$')           // template!
            {
                idx += 2;
                string tName = ParseName();
                Expect('@');
                var tPars = new List<TemplateArg>();
                for (; ; )
                {
                    var t = GetEncodedType();
                    if (t == null)
                        break;
                }
                scopes.Push(tName + "<>");
            }
            return scopes.Peek();
        }
        private string ParseSpecialName()
        {
            if (str[idx++] != '?') throw new InvalidOperationException();
            switch (str[idx++])
            {
            case '0': return "__ctor";
            case '1': return "__dtor";
            default: throw new NotImplementedException();
            }
        }

        private string ParseName()
        {
            var sb = new StringBuilder();
            while (idx < str.Length)
            {
                char ch = str[idx];
                if (!Char.IsLetter(ch) && !Char.IsNumber(ch) && ch != '_')
                    break;
                sb.Append(ch);
                ++idx;
            }
            var name = sb.ToString();
            return name;
        }

        private SerializedProcedure ParseTypeInformation(string clName, string name)
        {
            switch (str[idx++])
            {
            case '@':
                string callConvention;
                SerializedType retType ;
                SerializedArgument[] args;
                switch (str[idx++])
                {
                case 'Y':  // near global fn
                    callConvention = GetCallConvention();
                    retType = GetEncodedType();
                    args = GetArgs();
                    Expect('Z');
                    break;
                case 'Q': // public
                    name = "public: " + clName + "::" + name;
                    MaybeMemberModifier();
                    callConvention = GetCallConvention();
                    retType = GetEncodedType();
                    args = GetArgs();
                    Expect('Z');
                    break;
                case 'U': // public virtual
                    name = "public: virtual " + clName + "::" + name;
                    MaybeMemberModifier();
                    callConvention = GetCallConvention();
                    retType = GetEncodedType();
                    args = GetArgs();
                    Expect('Z');
                    break;
                default:
                    throw new FormatException(string.Format("Unpexected '{0}'", str[--idx]));
                }
                return new SerializedProcedure
                    {
                        Name = name,
                        Decompile = false,
                        Signature = new SerializedSignature
                        {
                            Convention = callConvention,
                            ReturnValue = new SerializedArgument { Type = retType },
                            Arguments = args,
                        }
                    };

            }
            throw new NotImplementedException();
        }

        private void MaybeMemberModifier()
        {
            switch (str[idx++])
            {
            case 'A': // default
            case 'B': // const
            case 'C': // volatile
            case 'D': // const volatile
                return;
            }
            throw new NotImplementedException();
        }

        private void Expect(char expected)
        {
            if (str[idx++] != expected)
                throw new FormatException(string.Format("Expected '{0}' but got '{1}'.", expected, str[idx-1]));
        }

        private SerializedArgument[] GetArgs()
        {
            var args = new List<SerializedArgument> ();
            while (idx < str.Length)
            {
                char ch = str[idx++];
                if (ch == '@' || ch == 'X')
                    break;
                switch (ch)
                {
                case 'A':
                    switch (str[idx++])
                    {
                    case 'B': // const &
                        var type = GetEncodedType();
                        args.Add(new SerializedArgument { Type = new SerializedPointerType { DataType=type } });     //$TODO: is_const, is_reference
                        break;
                    default: throw new NotSupportedException(string.Format("Unsupported type specifier 'A{0}'.", ch));
                    }
                    break;
                case 'D': args.Add(new SerializedArgument { Type = new SerializedPrimitiveType(Domain.Character, 1) }); break;
                default: throw new NotSupportedException(string.Format("Unsupported type specifier {0}.", ch));
               }
            }
            return args.ToArray();
        }

        private SerializedType GetEncodedType()
        {
            switch(str[idx++])
            {
            case 'H': return new SerializedPrimitiveType(Domain.SignedInt, 4);
            case '@': return null;  // No return type at all (dtors)
            case 'X': return new SerializedVoidType();
            case 'D': return new SerializedPrimitiveType(Domain.Character, 1);
            case 'U': return ParseFullyQualifiedTypeName(); // struct
            case 'V': return ParseFullyQualifiedTypeName(); // class
            }
            throw new NotSupportedException(string.Format("Unknown type {0}", str[idx - 1]));
        }

        private SerializedTypeReference ParseFullyQualifiedTypeName()
        {
            string name;
            char ch = str[idx];
            if (ch == '?' && str[idx + 1] == '$')
            {
                idx += 2;
                name = this.ParseName();
                Expect('@');
                var tempTypes = new List<SerializedType>();
                for (; ; )
                {
                    var t = this.GetEncodedType();
                    if (t == null)
                        break;
                    tempTypes.Add(t);
                }
                scopes = ParseNamespaces();
                var tr = new SerializedTypeReference(scopes.ToArray(), name, tempTypes.ToArray());
                namesSeen.Add(tr);
                return tr;
            }
            else if ('0' <= ch && ch <= '9')
            {
                int n = ch - '0';
                ++idx;
                var t = namesSeen[n];
                Expect('@');
                return t;
            }
            else
            {
                name = this.ParseName();
                Expect('@');
                scopes = ParseNamespaces();
                var tr = new SerializedTypeReference(scopes.ToArray(), name);
                namesSeen.Add(tr);
                return tr;
            }
        }

        public Stack<string> ParseNamespaces()
        {
            var stack = new Stack<string>();
            while (str[idx] != '@')
            {
                stack.Push(ParseName());
                Expect('@');
            }
            Expect('@');
            return stack;
        }

        private int ParseDecimalDigit()
        {
            switch (str[idx++])
            {
            case '0': return 0;
            default: throw new NotImplementedException();
            }
        }

        private string GetCallConvention()
        {
            switch (str[idx++])
            {
            case 'G': return "__stdcall";
            case 'A': return "__cdecl";
            case 'I': return "__fastcall";
            case 'E': return "__thiscall";
            }
            throw new NotSupportedException(string.Format("Unknown call convertion code '{0}'.", str[idx-1]));
        }
    }
}
