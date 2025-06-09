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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Parses C++ names mangled by Borland compilers.
    /// </summary>
// https://github.com/mildred/Lysaac/blob/master/doc/boa.cp437.txt
// http://edn.embarcadero.com/article/27758

    public class BorlandMangledNamedParser
    {
        private string s;
        private int i;

        public BorlandMangledNamedParser(string fnName)
        {
            this.s = fnName;
            this.i = 0;
        }

        public string? Modifier { get; set; }
        public string? Scope { get; set; }

        public (string?, SerializedType?, SerializedType?) Parse()
        {
            //$HACK: names with two leading @@ are special, so we treat them specially.
            // There is no indication of what signature such a function has, 
            // so the signature is left undefined.
            if (s.StartsWith("@@"))
            {
                var sig = new SerializedSignature();
                return (s, sig, null);
            }
            var qname = ParseName();
            if (qname is null)
                return (null, null, null);
            if (i >= s.Length)
            {
                // Vtable.
                throw new NotImplementedException();
            }
            switch (s[i])
            {
            case '$': return ParseArguments(qname);
            }
            throw new NotImplementedException();
        }

        private (string?, SerializedType?, SerializedType?) ParseArguments(List<string> qname)
        {
            const char cBackReference = 't';

            if (!Expect('$'))
                return (null, null, null);
            if (s[i] == 'b')
            {
                ++i;
                // Special function
                var fnName = ParseSpecialFnName(qname.Last());
                if (fnName is null)
                    return (null, null, null);
                qname.Add(fnName);
            }
            if (!Expect('q'))
            {
                return (null, null, null);
            }

            var args = new List<Argument_v1>();
            while (i < s.Length)
            {
                var domain = Domain.SignedInt;
                if (s[i] == cBackReference)
                {
                    // Back reference to a previous arg.
                    if (i >= s.Length - 1)
                        return (null, null, null);
                    char c = s[i + 1];
                    if ('1' <= c && c <= '9')
                    {
                        int iArg = c - '1';
                        i += 2;
                        args.Add(new Argument_v1 { Type = args[iArg].Type });
                    }
                }
                else
                {
                    var argType = ParseArgumentType(domain);
                    if (argType is null)
                        return (null, null, null);
                    args.Add(new Argument_v1 { Type = argType, });
                }
            }
            if (args.Count == 1 && args[0].Type is VoidType_v1)
            {
                args.Clear();
            }
            return (
                string.Join("::", qname),
                new SerializedSignature
                {
                    Arguments = args.ToArray()
                },
                null);
        }

        private static Dictionary<string, string> specialFnNames = new Dictionary<string, string>
        {
            { "ctr", "{0}" },
            { "dtr", "~{0}" },
            { "add", "operator +" },
            { "adr", "operator &" },
            { "and", "operator &" },
            { "arow", "operator ->" },
            { "arwm", "operator ->*" },
            { "asg", "operator =" },
            { "call", "operator ()" },
            { "cmp", "operator ~" },
            { "coma", "operator ," },
            { "dec", "operator --" },
            { "dele", "operator delete" },
            { "div", "operator /" },
            { "eql", "operator ==" },
            { "geq", "operator >=" },
            { "gtr", "operator >" },
            { "inc", "operator ++" },
            { "ind", "operator *" },
            { "land", "operator &&" },
            { "lor", "operator ||" },
            { "leq", "operator <=" },
            { "lsh", "operator <<" },
            { "lss", "operator <" },
            { "mod", "operator %" },
            { "mul", "operator *" },
            { "neq", "operator !=" },
            { "new", "operator new" },
            { "not", "operator !" },
            { "or", "operator |" },
            { "rand", "operator &=" },
            { "rdiv", "operator /=" },
            { "rlsh", "operator <<=" },
            { "rmin", "operator -=" },
            { "rmod", "operator %=" },
            { "rmul", "operator *=" },
            { "ror", "operator |=" },
            { "rplu", "operator +=" },
            { "rrsh", "operator >>=" },
            { "rsh", "operator >>" },
            { "rxor", "operator ^=" },
            { "sub", "operator -" },
            { "subs", "operator []" },
            { "xor", "operator ^" },
            { "nwa", "operator new []" },
            { "dla", "operator delete []" },
        };

        private string? ParseSpecialFnName(string className)
        {
            int iStart = i;
            i = s.IndexOf('$', iStart);
            if (i < 0 || i == iStart)
                return null;
            if (!specialFnNames.TryGetValue(s.Substring(iStart, i - iStart), out string? specialName))
                return null;

            ++i;    // skip by terminating '$'.
            return string.Format(specialName, className);
        }

        const char cVoid = 'v';
        const char cChar = 'c';
        const char cShort = 's';
        const char cInt= 'i';
        const char cLong = 'l';
        const char cFloat = 'f';
        const char cDouble = 'd';
        const char cLongDouble = 'g';
        const char cEllipsis = 'e';

        const char cNearPtr = 'p';
        const char cNearRef = 'r';

        const char cFarPtr = 'n';
        const char cFarRef = 'm';

        const char cConst = 'x';
        const char cUnsigned = 'u';
        const char cSigned = 'z';

        private SerializedType? ParseArgumentType(Domain domain)
        {
            var charDomain = Domain.Character;
            while (i < s.Length)
            {
                SerializedType? type;
                switch (s[i++])
                {
                case cVoid: return new VoidType_v1();
                case cChar: return new PrimitiveType_v1 { ByteSize = 1, Domain = charDomain };
                case cShort: return new PrimitiveType_v1 { ByteSize = 2, Domain = domain };
                case cInt: return new PrimitiveType_v1 { ByteSize = 2, Domain = domain };
                case cLong: return new PrimitiveType_v1 { ByteSize = 4, Domain = domain };
                case cNearPtr:
                    type = ParseArgumentType(domain);
                    if (type is null)
                        return null;
                    return new PointerType_v1 { PointerSize = 2, DataType = type };
                case cNearRef:
                    type = ParseArgumentType(domain);
                    if (type is null)
                        return null;
                    return new ReferenceType_v1 { Size = 2, Referent = type };
                case cFarPtr:
                    type = ParseArgumentType(domain);
                    if (type is null)
                        return null;
                    return new PointerType_v1 { PointerSize = 4, DataType = type };
                case cFarRef:
                    type = ParseArgumentType(domain);
                    if (type is null)
                        return null;
                    return new ReferenceType_v1 { Size = 4, Referent = type };
                case cUnsigned:
                    domain = Domain.UnsignedInt;
                    continue;
                case cSigned:
                    charDomain = Domain.SignedInt;
                    continue;
                case cConst:
                    //$TODO: Reko doesn't have the concept of consts yet.
                    continue;
                default:
                    --i;
                    if (char.IsDigit(s[i]))
                        return ParseEnumClassName();
                    throw new NotImplementedException(string.Format("Unexpected character '{0}'.", s[i]));
                }
            }
            return null;
        }

        private List<string>? ParseName()
        {
            var names = new List<string>();
            if (!Expect('@'))
                return null;
            int iStart = i;
            while (i < s.Length)
            {
                char c = s[i];
                if (c == '@')
                {
                    names.Add(s.Substring(iStart, i - iStart));
                    ++i;
                    if (char.IsDigit(s[i]))
                    {
                        ++i;
                        if (s[i] == '$')
                        {
                            return names;
                        }
                    }
                    iStart = i;
                }
                else if (c == '$')
                {
                    names.Add(s.Substring(iStart, i - iStart));
                    return names;
                }
                ++i;
            }
            return names;
        }

        private TypeReference_v1? ParseEnumClassName()
        {
            var len = ParseLength();
            if (!len.HasValue)
                return null;
            var c = len.Value;
            string name;
            if (i + c > s.Length)
            {
                //$TODO: warn user about truncated symbol.
                name = s.Substring(i) + "_truncated";
            }
            else
            {
                name = s.Substring(i, c);
            }
            var tref = new TypeReference_v1 { TypeName = name };
            i += len.Value;
            return tref;
        }

        private int? ParseLength()
        {
            int iStart = i;
            int n = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (!char.IsDigit(c))
                {
                    if (iStart == i)
                        return null;
                    return n;
                }
                n = n * 10 + (c - '0');
                ++i;
            }
            return n;
        }

        private bool Expect(char c)
        {
            if (i >= s.Length || s[i] != c)
                return false;
            ++i;
            return true;
        }
    }
}
