#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core.IRFormat
{
    public class IRFormatLexer
    {
        private TextReader rdr;
        private Token tok;

        public IRFormatLexer(TextReader rdr)
        {
            this.rdr = rdr;
        }

        internal bool Peek(IRTokenType tok)
        {
            throw new NotImplementedException();
        }

        internal Token Get()
        {
            if (tok.Type != IRTokenType.None)
            {
                var t = this.tok;
                tok = Token.None;
                return tok;
            }
            throw new NotImplementedException();
        }

        private enum State
        {
            Initial,
            ID,
        }

        internal Token Read()
        {
            var st = State.Initial;
            var sb = new StringBuilder();
            for (;;)
            {
                int c = rdr.Read();
                char ch = (char)c;
                switch (st)
                {
                case State.Initial:
                    switch (c)
                    {
                    case -1: return new Token(IRTokenType.EOF);
                    default:
                        if (Char.IsLetter(ch))
                        {
                            sb.Append(ch);
                            st = State.ID;
                        }
                        return Unexpected(c);
                    }
                default:
                    return Unexpected(c);
                }
            }
        }

        // 0x00
        // -123
        // +0.32p3
        // c"..."
        // w"...."
        // 


        private Token Unexpected(int c)
        {
            throw new NotImplementedException();
        }
    }
}