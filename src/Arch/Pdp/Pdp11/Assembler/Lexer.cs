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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Arch.Pdp.Pdp11.Assembler
{
    public class Lexer
    {
        private TextReader rdr;
        private Token? tok;
        private StringBuilder sb;
        private int lineNumber;

        private static Dictionary<string, TokenType> mpstrtotype = new Dictionary<string, TokenType>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".", TokenType._Dot },
            { ".page", TokenType._Page } ,
            { ".word", TokenType._Word } ,
            { "r0", TokenType.Register },
            { "r1", TokenType.Register },
            { "r2", TokenType.Register },
            { "r3", TokenType.Register },
            { "r4", TokenType.Register },
            { "r5", TokenType.Register },
            { "r6", TokenType.Register },
            { "r7", TokenType.Register },
            { "sp", TokenType.Register },
            { "pc", TokenType.Register },

            { "asr", TokenType.ASR },
            { "clr", TokenType.CLR },
            { "clrb", TokenType.CLRB },
            { "beq", TokenType.BEQ },
            { "bgt", TokenType.BGT },
            { "dec", TokenType.DEC },
            { "inc", TokenType.INC },
            { "jsr", TokenType.JSR },
            { "mov", TokenType.MOV },
            { "movb", TokenType.MOVB },
            { "reset", TokenType.RESET }, 
            { "sub", TokenType.SUB }, 

        };

        public Lexer(TextReader rdr)
        {
            this.rdr = rdr;
            this.lineNumber = 1;
            this.sb = new StringBuilder();
        }

        private enum State
        {
            Initial,
            Comment,
            Id,
            Number,
        }

        public Token Peek()
        {
            if (tok is not null)
                return tok;
            tok = Get();
            return tok;
        }

        public bool PeekAndDiscard(TokenType type)
        {
            if (Peek().Type == type)
            {
                Get();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Token Get()
        {
            if (tok is not null)
            {
                var t = tok;
                tok = null;
                return t;
            }
            var st = State.Initial;
            for (; ; )
            {
                int c = rdr.Peek();
                char ch = (char) c;
                switch (st)
                {
                case State.Initial:
                    switch (c)
                    {
                    case -1:
                        return new Token(TokenType.EOF, lineNumber);
                    case ' ':
                    case '\t':
                    case '\r':
                        st = Transition(State.Initial); break;
                    case '\n':
                        ++lineNumber;
                        return EatTokenize(TokenType.EOL);
                    case ';': st = Transition(State.Comment); break;
                    case '=': return EatTokenize(TokenType.Eq);
                    case '+': return EatTokenize(TokenType.Plus);
                    case ':': return EatTokenize(TokenType.Colon);
                    case ',': return EatTokenize(TokenType.Comma);
                    case '#': return EatTokenize(TokenType.Hash);
                    case '@': return EatTokenize(TokenType.At);
                    case '-': return EatTokenize(TokenType.Minus);
                    case '(': return EatTokenize(TokenType.LParen);
                    case ')': return EatTokenize(TokenType.RParen);
                    case '.': st = StartToken(ch, State.Id); break;
                    default:
                        if (Char.IsLetter(ch))
                        {
                           st =  StartToken(ch, State.Id); break;
                        }
                        else if (Char.IsDigit(ch))
                        {
                            st = StartToken(ch, State.Number); break;
                        }
                        Unexpected(c);
                        break;
                    }
                    break;
                case State.Comment:
                    switch (c)
                    {
                    case -1: return new Token(TokenType.EOF, lineNumber);
                    case '\n': 
                        ++lineNumber;
                        return EatTokenize(TokenType.EOL);
                    default: st = Transition(State.Comment); break;
                    }
                    break;
                case State.Id:
                    switch (c)
                    {
                    case -1: return TokenizeId();
                    default:
                        if (char.IsLetterOrDigit(ch) || ch == '_')
                        {
                            st = Accumulate(ch, State.Id);
                        }
                        else
                        {
                            return TokenizeId();
                        }
                        break;
                    }
                    break;
                case State.Number:
                    switch (c)
                    {
                    case -1: return new Token(TokenType.Number, Convert.ToInt32(sb.ToString(), 8), lineNumber);
                    case '.': return EatTokenize(TokenType.Number, Convert.ToInt32(sb.ToString(), 10));
                    default:
                        if (char.IsDigit(ch))
                            st = Accumulate(ch, State.Number);
                        else 
                            return new Token(TokenType.Number, Convert.ToInt32(sb.ToString(), 8), lineNumber);
                        break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
        }

        private Token TokenizeId(char ch)
        {
            sb.Append(ch);
            rdr.Read();
            return TokenizeId();
        }

        private Token TokenizeId()
        {
            var str = sb.ToString();
            TokenType type;
            if (!mpstrtotype.TryGetValue(str, out type))
            {
                type = TokenType.Id;
            }
            return new Token(type, str, this.lineNumber);
        }

        private State Accumulate(char ch,State state)
        {
            sb.Append(ch);
            rdr.Read();
            return state;
        }

        private State StartToken(State state)
        {
            this.sb.Clear();
            return state;
        }

        private State StartToken(char ch, State state)
        {
            this.sb.Clear();
            rdr.Read();
            sb.Append(ch);
            return state;
        }

        private void Unexpected(int c)
        {
            throw new FormatException(string.Format("Unexpected character '{0}' (U+{1:X4}).", (char)c, c));
        }

        private Token EatTokenize(TokenType tokenType)
        {
            rdr.Read();
            return new Token(tokenType, lineNumber);
        }

        private Token EatTokenize(TokenType tokenType, object value)
        {
            rdr.Read();
            return new Token(tokenType, value, this.lineNumber);
        }

        private State Transition(State newState)
        {
            rdr.Read();
            return newState;
        }

        public void Unexpected(Token unexpected)
        {
            throw new FormatException(string.Format("Unpexected token '{0}'.", unexpected));
        }

        public object? Expect(TokenType expected)
        {
            var token = Get();
            if (token.Type != expected)
                throw new FormatException(string.Format("Unpexected token '{0}', expected '{1}'.", token.Type, expected));
            return token.Value;
        }
    }

    public class Token
    {
        public TokenType Type { get; private set; }
        public object? Value { get; }
        public int Linenumber { get; private set; }
        public Token(TokenType type, int lineNumber)
        {
            this.Type = type;
            this.Linenumber = lineNumber;
        }

        public Token(TokenType type, object value, int linenumber)
        {
            this.Type = type;
            this.Value = value;
            this.Linenumber = linenumber;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Type);
            if (Value is not null)
            {
                sb.AppendFormat(" ({0})", Value);
            }
            sb.AppendFormat(" on line {0}", Linenumber);
            return sb.ToString();
        }
    }

    public enum TokenType
    {
        EOF,
        EOL,
        Id,
        Eq,
        Number,
        Plus,
        _Page,
        Colon,
        _Word,
        _Dot,
        RESET,
        MOV,
        Comma,
        Register,
        Hash,
        JSR,
        SUB,
        At,
        ASR,
        MOVB,
        Minus,
        LParen,
        RParen,
        CLR,
        DEC,
        INC,
        CLRB,
        BEQ,
        BGT,
    }
}