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
    public class BorlandMangledNamedParser
    {
        private string s;
        private int i;

        public BorlandMangledNamedParser(string fnName)
        {
            this.s = fnName;
            this.i = 0;
        }

        public string Modifier { get; set; }
        public string Scope { get; set; }

        public Tuple<string, SerializedType, SerializedType> Parse()
        {
            Debug.Print("$$$ {0}", s);
            var qname = ParseName();
            if (qname == null)
                return null;
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

        private Tuple<string, SerializedType, SerializedType> ParseArguments(List<string> qname)
        {
            if (!Expect('$'))
                return null;
            if (!Expect('q'))
                return null;
            var args = new List<Argument_v1>();
            while (i < s.Length)
            {
                var domain = Domain.SignedInt;
                var argType = ParseArgumentType(domain);
                if (argType == null)
                    return null;
                args.Add(new Argument_v1 { Type = argType, });
            }
            if (args.Count == 1 && args[0].Type is VoidType_v1)
                args.Clear();
            return new Tuple<string,SerializedType,SerializedType>(
                string.Join("::", qname),
                new SerializedSignature
                {
                    Arguments = args.ToArray()
                },
                null);
        }

        const char cVoid = 'v';
        const char cShort = 's';
        const char cInt= 'i';
        const char cLong = 'l';
        const char cFloat = 'f';
        const char cDouble = 'd';
        const char cLongDouble = 'g';
        const char cEllipsis = 'e';

        const char cFarPtr = 'n';

        private SerializedType ParseArgumentType(Domain domain)
        {
            Debug.Assert(i < s.Length);
            switch (s[i++])
            {
            case cVoid: return new VoidType_v1();
            case cInt: return new PrimitiveType_v1 { ByteSize = 2, Domain = domain };
            case cFarPtr:
                var type = ParseArgumentType(domain);
                if (type == null)
                    return null;
                return new PointerType_v1 { PointerSize = 4, DataType = type };
            default:
                --i;
                if (char.IsDigit(s[i]))
                    return ParseEnumClassName();
                throw new NotImplementedException(string.Format("Unexpected character '{0}'.", s[i]));
            }
        }

        private List<string> ParseName()
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
                    iStart = i + 1;
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

        private TypeReference_v1 ParseEnumClassName()
        {
            var len = ParseLength();
            if (!len.HasValue)
                return null;
            var tref = new TypeReference_v1 { TypeName = s.Substring(i, len.Value) };
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
