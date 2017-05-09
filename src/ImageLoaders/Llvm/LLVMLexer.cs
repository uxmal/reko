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

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMLexer
    {
        private TextReader rdr;

        public LLVMLexer(TextReader rdr)
        {
            this.rdr = rdr;
            this.LineNumber = 1;
        }

        public int LineNumber { get; private set; }

        private enum State
        {
            Start,
            Comment
        }

        public Token GetToken()
        {
            var st = State.Start;
            StringBuilder sb = null;
            EatWs();
            for (;;)
            {
                int c = rdr.Read();
                char ch = (char)c;
                switch (st)
                {
                case State.Start:
                    switch (c)
                    {
                    case -1: return new Token(TokenType.EOF);
                    case ';': sb = new StringBuilder(); st = State.Comment; break;
                    default:
                        throw new FormatException(
                            string.Format("Unexpected character '{0}' (U+{1:X4}) on line {2}.",
                                ch, c, LineNumber));
                    }
                    break;
                case State.Comment:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Comment, sb);
                    case '\r':
                    case '\n':
                        EatWs();
                        return Tok(TokenType.Comment, sb);
                    default:
                        sb.Append(ch);
                        break;
                    }
                    break;
                }
            }
        }

        private Token Tok(TokenType type, StringBuilder sb)
        {
            return new Token(type, 
                sb != null ? sb.ToString() : null);
        }

        private bool EatWs()
        {
            if (rdr == null)
                return false;
            int ch = rdr.Peek();
            while (ch >= 0 && Char.IsWhiteSpace((char)ch))
            {
                if (ch == '\n')
                    ++LineNumber;
                rdr.Read();
                ch = rdr.Peek();
            }
            return true;
        }
    }
}
