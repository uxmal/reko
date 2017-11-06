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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Pascal
{
    public class PascalLexer
    {
        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            { "const", TokenType.Const }
        };

        private TextReader rdr;
        private Token token;
        private int nestedComments;

        public PascalLexer(TextReader rdr)
        {
            this.rdr = rdr;
        }

        public Token Read()
        {
            if (this.token != null)
            {
                var tok = this.token;
                this.token = null;
                return tok;
            }
            return Get();
        }

        public Token Peek()
        {
            if (this.token != null)
            {
                return token;
            }
            this.token = Get();
            return this.token;
        }

        private enum State
        {
            Init,
            Comment,
            Identifier,
        }

        private Token Get()
        {
            var sb = new StringBuilder();
            var st = State.Init;
            for (;;)
            {
                int c = rdr.Peek();
                char ch = (char)c;
                switch (st)
                {
                case State.Init:
                    if (c < 0)
                        return Tok(TokenType.EOF);
                    switch (ch)
                    {
                    case ' ':
                    case '\t':  rdr.Read(); break;
                    case '{': rdr.Read(); ++nestedComments; st = State.Comment; break;
                    default:
                        if ('A' <= ch && ch <= 'Z' || 'a' <= ch && ch <= 'z' || ch == '_')
                        {
                            rdr.Read();
                            sb.Clear();
                            sb.Append(ch);
                            st = State.Identifier;
                            break;
                        }
                        Nyi(st, ch);
                        break;
                    }
                    break;
                case State.Comment:
                    if (c < 0)
                        return Tok(TokenType.EOF);
                    switch (ch)
                    {
                    case '{': rdr.Read(); ++nestedComments; break;
                    case '}':
                        rdr.Read();
                        --nestedComments;
                        if (nestedComments == 0)
                            st = State.Init;
                        break;
                    default:
                        rdr.Read();
                        break;
                    }
                    break;
                case State.Identifier:
                    if (c < 0)
                        return MaybeKeyword(sb.ToString());
                    if ('A' <= ch && ch <= 'Z' || 'a' <= ch && ch <= 'z' || ch == '_' ||
                        '0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        sb.Append(ch);
                    }
                    else
                    {
                        return MaybeKeyword(sb.ToString());
                    }
                    break;
                }
            }
        }

        private Token MaybeKeyword(string str)
        {
            TokenType tok;
            if (keywords.TryGetValue(str, out tok))
                return new Token { Type = tok };
            else
                return new Token { Type = TokenType.Id, Value = str };
        }

        private void Nyi(State state, char ch)
        {
            throw new NotImplementedException(string.Format("State {0}, ch: {1} (U+{2:X4})", state, ch, (uint)ch));
        }


        private Token Tok(TokenType type)
        {
            return new Token { Type = type };
        }
    }
}
