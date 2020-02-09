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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Assemblers.M68k
{
    class Lexer
    {
        private System.IO.TextReader rdr;
        private Token lookahead;
        private State st;
        private StringBuilder sb;
        private static Dictionary<string, Token> keywords = new Dictionary<string, Token>
        {
        { "cnop", new Token(TokenType.CNOP) },
        { "dc", new Token(TokenType.DC) },
        { "idnt", new Token(TokenType.IDNT) },
        { "opt", new Token(TokenType.OPT) },
        { "equ", new Token(TokenType.EQU) },
        { "public", new Token(TokenType.PUBLIC) },
        { "reg", new Token(TokenType.REG) },
        { "section", new Token(TokenType.SECTION) },

        { "add", new Token(TokenType.add) },
        { "addq", new Token(TokenType.addq) },
        { "cmp", new Token(TokenType.cmp) },
        { "clr", new Token(TokenType.clr) },
        { "beq", new Token(TokenType.beq) },	
        { "bge", new Token(TokenType.bge) },
        { "bne", new Token(TokenType.bne) },
        { "bra", new Token(TokenType.bra) },	
        { "jsr", new Token(TokenType.jsr) },
        { "lea", new Token(TokenType.lea) },	
        { "move", new Token(TokenType.move) },
        { "movem", new Token(TokenType.movem) },
        { "pea", new Token(TokenType.pea) },
        { "rts", new Token(TokenType.rts) },
        { "subq", new Token(TokenType.subq) },
        };

        public Lexer(System.IO.TextReader rdr)
        {
            this.rdr = rdr;
            st = State.StartOfLine;
            lookahead = new Token(TokenType.EOFile);
            sb = new StringBuilder();
        }

        public Token PeekToken()
        {
            if (lookahead.Type == TokenType.EOFile)
            {
                lookahead = ReadToken();
            }
            return lookahead;
        }

        public Token GetToken()
        {
            if (lookahead.Type != TokenType.EOFile)
            {
                var t = lookahead;
                lookahead = new Token(TokenType.EOFile);
                return t;
            }
            return ReadToken();
        }

        private enum State
        {
            StartOfLine,
            InitialWs,      // Whitespace before token.
            BetweenWs,      // Whitespace between token
            Id,
            String,
            Integer,
            Cr,
            Comment,
            HexNumber
        }

        private Token ReadToken()
        {
            for (; ; )
            {
                int ch = rdr.Peek();
                char c = (char) ch;
                switch (st)
                {
                case State.StartOfLine:
                    if (ch == -1)
                        return LightToken(TokenType.EOFile, State.StartOfLine);
                    switch (ch)
                    {
                    case ' ':
                    case '\t':
                        st = State.InitialWs;
                        rdr.Read();
                        break;
                    case '\r':
                        st = State.Cr;
                        rdr.Read();
                        break;
                    case '\n':
                        rdr.Read();
                        return LightToken(TokenType.NL, State.StartOfLine);
                    default:
                        st = State.BetweenWs;
                        break;
                    }
                    break;
                case State.Cr:
                    st = State.StartOfLine;
                    if (c == '\n')
                    {
                        rdr.Read();
                    }
                    return LightToken(TokenType.NL, State.StartOfLine);
                case State.InitialWs:
                    if (ch == -1)
                        return LightToken(TokenType.EOFile, State.InitialWs);
                    switch (ch)
                    {
                    case ' ':
                    case '\t':
                        rdr.Read();
                        break;
                    default:
                        return LightToken(TokenType.WS, State.BetweenWs);
                    }
                    break;
                case State.BetweenWs:
                    if (ch == -1)
                        return new Token(TokenType.EOFile);
                    switch (ch)
                    {
                    case ' ':
                    case '\t':
                        rdr.Read();
                        break;
                    case '\r':
                        st = State.Cr;
                        rdr.Read();
                        break;
                    case '\n':
                        rdr.Read();
                        return LightToken(TokenType.NL, State.StartOfLine);
                    case '"':
                        rdr.Read();
                        st = State.String;
                        break;
                    case ',':
                        rdr.Read();
                        return LightToken(TokenType.COMMA, State.BetweenWs);
                    case '.':
                        rdr.Read();
                        return LightToken(TokenType.DOT, State.BetweenWs);
                    case '#':
                        rdr.Read();
                        return LightToken(TokenType.HASH, State.BetweenWs);
                    case '+':
                        rdr.Read();
                        return LightToken(TokenType.PLUS, State.BetweenWs);
                    case '-':
                        rdr.Read();
                        return LightToken(TokenType.MINUS, State.BetweenWs);
                    case '(':
                        rdr.Read();
                        return LightToken(TokenType.LPAREN, State.BetweenWs);
                    case ')':
                        rdr.Read();
                        return LightToken(TokenType.RPAREN, State.BetweenWs);
                    case ';':
                        rdr.Read();
                        st = State.Comment;
                        break;
                    case '$':
                        rdr.Read();
                        st = State.HexNumber;
                        break;
                    default:
                        if (Char.IsLetter(c) || c == '_')
                        {
                            sb.Append(c);
                            rdr.Read();
                            st = State.Id;
                            break;
                        }
                        if (Char.IsDigit(c))
                        {
                            sb.Append(c);
                            rdr.Read();
                            st = State.Integer;
                            break;
                        }
                        throw Unexpected(ch, c);
                    }
                    break;
                case State.Id:
                    if (ch == -1 || !Char.IsLetterOrDigit(c) && c != '_')
                    {
                        st = State.BetweenWs;
                        return ClassifyTokenText();
                    }
                    rdr.Read();
                    sb.Append(c);
                    break;
                case State.String:
                    if (ch == -1)
                        throw new FormatException("Unterminated string constant.");
                    if (ch == '"')
                    {
                        rdr.Read();
                        return BuildToken(TokenType.STRING, State.BetweenWs);
                    }
                    else
                    {
                        sb.Append(c);
                        rdr.Read();
                    }
                    break;
                case State.Integer:
                    if (ch == -1 || !Char.IsDigit(c))
                    {
                        st = State.BetweenWs;
                        return BuildToken(TokenType.INTEGER, State.BetweenWs);
                    }
                    sb.Append(c);
                    rdr.Read();
                    break;
                case State.Comment:
                    if (ch == -1)
                        return LightToken(TokenType.EOFile, State.BetweenWs);
                    rdr.Read();
                    if (c == '\r')
                        st = State.Cr;
                    else if (c == '\n')
                        return LightToken(TokenType.NL, State.StartOfLine);
                    break;
                case State.HexNumber:
                    if (ch == -1)
                        return HexToken(State.StartOfLine);
                    if ('0' <= c && c <= '9' ||
                        'a' <= c && c <= 'f' ||
                        'A' <= c && c <= 'F')
                    {
                        rdr.Read();
                        sb.Append(c);
                        break;
                    }
                    return HexToken(State.StartOfLine);
                }
            }
            throw new NotImplementedException();
        }

        private static NotImplementedException Unexpected(int ch, char c)
        {
            return new NotImplementedException(string.Format("Unexpected {0} (U+{1:X4}).", c, ch));
        }

        private Token LightToken(TokenType t, State nextState)
        {
            Debug.Write(string.Format("{0} ", t));
            if (t == TokenType.NL)
                Debug.WriteLine("");
            st = nextState;
            return new Token(t);
        }

        private Token HexToken(State nextState)
        {
            string hex = sb.ToString();
            int h = Convert.ToInt32(hex, 16);
            Debug.Write(string.Format("{0} {1}", TokenType.INTEGER, h));
            sb = new StringBuilder();
            st = nextState;
            return new Token(TokenType.INTEGER, h.ToString(System.Globalization.CultureInfo.InvariantCulture)); 
        }

        private Token BuildToken(TokenType tokenType, State nextState)
        {
            var t = new Token(tokenType, sb.ToString());
            Debug.Write(string.Format("{0} {1} ", t.Type, t.Text));
            sb = new StringBuilder();
            st = nextState;
            return t;
        }

        private Token ClassifyTokenText()
        {
            var text = sb.ToString();
            sb = new StringBuilder();
            Token kw;
            if (keywords.TryGetValue(text, out kw))
            {
                Debug.Write(string.Format("{0} ", kw.Type));
                return kw;
            }
            Debug.Write(string.Format("{0} {1} ", TokenType.ID, text));
            return new Token(TokenType.ID, text);
        }
    }
}
