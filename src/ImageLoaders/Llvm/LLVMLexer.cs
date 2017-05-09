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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMLexer
    {
        private TextReader rdr;
        private StringBuilder sb;

        public LLVMLexer(TextReader rdr)
        {
            this.rdr = rdr;
            this.LineNumber = 1;

        }

        public int LineNumber { get; private set; }

        private enum State
        {
            Start,
            Comment,
            Reserved,
            String,
            Id,
            c,
            CharArray,
            Zero,
            Decimal,
            Hex,
            Dot1,
            Dot2,
            i,
        }

        public Token GetToken()
        {
            sb = null;
            var st = State.Start;
            this.lineStart = LineNumber;
            EatWs();
            for (;;)
            {
                int c = rdr.Peek();
                char ch = (char)c;
                switch (st)
                {
                case State.Start:
                    switch (c)
                    {
                    case -1: return new Token(lineStart, TokenType.EOF);
                    case '"': rdr.Read(); sb = new StringBuilder(); st = State.String; break;
                    case ';': rdr.Read(); sb = new StringBuilder(); st = State.Comment; break;
                    case '@':
                    case '%': rdr.Read();  sb = new StringBuilder(); sb.Append(ch); st = State.Id; break;
                    case '=': rdr.Read(); return Tok(TokenType.EQ); 
                    case '(': rdr.Read(); return Tok(TokenType.LPAREN); 
                    case '[': rdr.Read(); return Tok(TokenType.LBRACKET);
                    case '{': rdr.Read(); return Tok(TokenType.LBRACE);
                    case ')': rdr.Read(); return Tok(TokenType.RPAREN); 
                    case ']': rdr.Read(); return Tok(TokenType.RBRACKET);
                    case '}': rdr.Read(); return Tok(TokenType.RBRACE);
                    case '*': rdr.Read(); return Tok(TokenType.STAR);
                    case ',': rdr.Read(); return Tok(TokenType.COMMA);
                    case '#': rdr.Read(); return Tok(TokenType.HASH);
                    case 'c': rdr.Read(); st = State.c; break;
                    case 'i': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.i; break;
                    case '0': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.Zero; break;
                    case '.': rdr.Read(); st = State.Dot1; break;
                    case '!': rdr.Read(); return Tok(TokenType.BANG);
                    default:
                        if (char.IsLetter(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb = new StringBuilder(); sb.Append(ch); st = State.Reserved; break;
                        }
                        if (char.IsDigit(ch))
                        {
                            rdr.Read();
                            sb = new StringBuilder(); sb.Append(ch); st = State.Decimal; break;
                        }
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
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    break;
                case State.Reserved:
                    switch (c)
                    {
                    case -1: return ReservedWord(sb);
                    default:
                        if (char.IsLetterOrDigit(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb.Append(ch);
                            break;
                        }
                        else
                        {
                            return ReservedWord(sb);
                        }
                    }
                    break;
                case State.String:
                    switch (c)
                    {
                    case -1: throw new FormatException("Unterminated string.");
                    case '"': rdr.Read(); return Tok(TokenType.String, sb);
                    default: rdr.Read(); sb.Append(ch); break;
                    }
                    break;
                case State.Id:
                    switch(c)
                    {
                    case -1: return Identifier(sb);
                    case '-':
                    case '_':
                    case '.':
                    case '$':
                        rdr.Read(); sb.Append(ch); break;
                    default:
                        if (char.IsLetterOrDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        return Identifier(sb);
                    }
                    break;
                case State.c:
                    switch (c)
                    {
                    case -1: return ReservedWord("c");
                    case '"': rdr.Read(); sb = new StringBuilder(); st = State.CharArray; break;
                    default:
                        rdr.Read();
                        sb = new StringBuilder();
                        sb.Append('c');
                        sb.Append(ch);
                        st = State.Reserved;
                        break;
                    }
                    break;
                case State.i:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.IntType, sb);
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read();
                            sb.Append(ch);
                        }
                        else if (char.IsLetter(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb.Append(ch);
                            st = State.Reserved;
                        }
                        else
                        {
                            return Tok(TokenType.IntType, sb);
                        }
                        break;
                    }
                    break;
                case State.CharArray:
                    switch (c)
                    {
                    case -1: throw new FormatException("Unterminated character array.");
                    case '"': rdr.Read(); return Tok(TokenType.CharArray, sb);
                    default: rdr.Read(); sb.Append(ch); break;
                    }
                    break;
                case State.Zero:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Integer, sb);
                    case 'x':
                    case 'X':
                        rdr.Read(); sb.Clear(); st = State.Hex; break;
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.Integer, sb);
                        }
                    }
                    break;
                case State.Decimal:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Integer, sb);
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.Integer, sb);
                        }
                    }
                    break;
                case State.Hex:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.HexInteger, sb);
                    default:
                        if ("0123456789abcdefABCDEF".IndexOf(ch) >= 0)
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.HexInteger, sb);
                        }
                    }
                    break;
                case State.Dot1:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.DOT);
                    case '.':rdr.Read(); st = State.Dot2; break;
                    default: return Tok(TokenType.DOT);
                    }
                    break;
                case State.Dot2:
                    switch (c)
                    {
                    case '.': rdr.Read(); return Tok(TokenType.ELLIPSIS);
                    default: Unexpected(".."); break;
                    }
                    break;
                }
            }
        }

        private void Unexpected(string un)
        {
            throw new FormatException(string.Format("Unexpected string '{0}' on line {1}.", un, LineNumber));
        }

        private Token Identifier(StringBuilder sb)
        {
            if (sb.Length < 2)
            {
                throw new FormatException("Invalid identifier.");
            }
            var type = sb[0] == '@'
                ? TokenType.GlobalId
                : TokenType.LocalId;
            return new Token(this.lineStart, type, sb.ToString(1, sb.Length - 1));
        }

        private Token ReservedWord(string s)
        {
            return new Token(this.lineStart, reservedWords[s]);
        }

        private Token ReservedWord(StringBuilder sb)
        {
            TokenType type;
            if (!reservedWords.TryGetValue(sb.ToString(), out type))
            {
                //throw new FormatException(string.Format("Unknown reserved word '{0}'.", sb.ToString()));
                Debug.Print("Unknown reserved word '{0}'.", sb.ToString());
                return new Token(this.lineStart,TokenType.String, sb.ToString());
            }
            return new Token(this.lineStart, reservedWords[sb.ToString()]);
        }

        private Token Tok(TokenType type, StringBuilder sb = null)
        {
            return new Token(lineStart, type, 
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

        static Dictionary<string, TokenType> reservedWords = new Dictionary<string, TokenType>
        {
            { "add", TokenType.add },
            { "align", TokenType.align },
            { "alloca", TokenType.alloca },
            { "and", TokenType.and },
            { "attributes", TokenType.attributes },
            { "bitcast", TokenType.bitcast },
            { "br", TokenType.br },
            { "call", TokenType.call },
            { "constant", TokenType.constant },
            { "datalayout", TokenType.datalayout },
            { "declare", TokenType.declare },
            { "define", TokenType.define },
            { "dereferenceable", TokenType.dereferenceable },
            { "external", TokenType.external },
            { "extractvalue", TokenType.extractvalue },
            { "false", TokenType.@false },
            { "getelementptr", TokenType.getelementptr },
            { "global", TokenType.global },
            { "i1", TokenType.i1 },
            { "i8", TokenType.i8 },
            { "i16", TokenType.i16 },
            { "i32", TokenType.i32 },
            { "i64", TokenType.i64 },
            { "icmp", TokenType.icmp },
            { "ident", TokenType.ident },
            { "inbounds", TokenType.inbounds },
            { "inrange", TokenType.inrange },
            { "inttoptr", TokenType.inttoptr },
            { "label", TokenType.label },
            { "load", TokenType.load },
            { "llvm", TokenType.llvm },
            { "ne", TokenType.ne },
            { "null", TokenType.@null },
            { "private", TokenType.@private },
            { "nocapture", TokenType.nocapture},
            { "noinline", TokenType.noinline },
            { "nounwind", TokenType.nounwind },
            { "nsw", TokenType.nsw },
            { "phi", TokenType.phi },
            { "ret", TokenType.ret },
            { "source_filename", TokenType.source_filename },
            { "store", TokenType.store },
            { "sub", TokenType.sub },
            { "switch", TokenType.@switch },
            { "target", TokenType.target },
            { "to", TokenType.to },
            { "triple", TokenType.triple },
            { "type", TokenType.type },
            { "unnamed_addr", TokenType.unnamed_addr },
            { "uwtable", TokenType.uwtable },
            { "void", TokenType.@void },
            { "x", TokenType.x },
        };
        private int lineStart;
    }
}
