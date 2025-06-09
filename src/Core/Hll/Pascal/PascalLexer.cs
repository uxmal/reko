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
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reko.Core.Hll.Pascal
{
    /// <summary>
    /// Performs lexical analysis of Pascal source code.
    /// </summary>
    public class PascalLexer
    {
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            { "array",      TokenType.Array},
            { "boolean",    TokenType.Boolean },
            { "case",       TokenType.Case },
            { "char",       TokenType.Char },
            { "const",      TokenType.Const },
            { "end",        TokenType.End },
            { "extended",   TokenType.Extended },
            { "false",      TokenType.False },
            { "file",       TokenType.File },
            { "function",   TokenType.Function },
            { "interface",  TokenType.Interface },
            { "inline",     TokenType.Inline },
            { "integer",    TokenType.Integer },
            { "longint",    TokenType.Longint },
            { "of",         TokenType.Of },
            { "packed",     TokenType.Packed },
            { "procedure",  TokenType.Procedure },
            { "record",     TokenType.Record },
            { "set",        TokenType.Set },
            { "string",     TokenType.String },
            { "true",       TokenType.True },
            { "type",       TokenType.Type },
            { "unit",       TokenType.Unit},
            { "univ",       TokenType.Univ },
            { "var",        TokenType.Var},
        };

        private readonly TextReader rdr;
        private readonly List<(char,int)> nestedComments;
        private readonly bool allowNestedComments;
        private Token? token;
        private int sign;
        private int lineNumber;
        private State st;

        /// <summary>
        /// Constructs a Pascal lexer.
        /// </summary>
        /// <param name="rdr">Underlying text stream.</param>
        /// <param name="allowNestedComments">Allow nested comments.
        /// </param>
        public PascalLexer(TextReader rdr, bool allowNestedComments)
        {
            this.rdr = rdr;
            this.lineNumber = 1;
            this.st = State.Init;
            this.nestedComments = new List<(char,int)>();
            this.allowNestedComments = allowNestedComments;
        }

        /// <summary>
        /// Read a Pascal token.
        /// </summary>
        /// <returns>A <see cref="Token"/> instance.</returns>
        public Token Read()
        {
            if (this.token is not null)
            {
                var tok = this.token;
                this.token = null;
                return tok;
            }
            return Get();
        }

        /// <summary>
        /// Peek at the next token without consuming it.
        /// </summary>
        /// <returns>A <see cref="Token"/> instance.</returns>
        public Token Peek()
        {
            if (this.token is not null)
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
            CommentLparen,
            CommentStar,
            Identifier,
            Dot,
            Plus,
            Minus,
            Number,
            NumberDot,
            Cr,
            HexNumber,
            String,
            LParen,
            Real,
            DString,
            RealExponentStart,
            RealExponent,
        }

        private Token Get()
        {
            var sb = new StringBuilder();
            for (;;)
            {
                int c = rdr.Peek();
                char ch = (char) c;
                switch (st)
                {
                case State.Init:
                    if (c < 0)
                        return Tok(TokenType.EOF);
                    switch (ch)
                    {
                    case ' ':
                    case '\t': rdr.Read(); break;
                    case '\n': rdr.Read(); ++lineNumber; break;
                    case '\f': rdr.Read(); ++lineNumber; break;
                    case '\r': rdr.Read(); st = State.Cr; break;
                    case '\x1A': rdr.Read(); return Tok(TokenType.EOF);
                    case '{': rdr.Read(); NestComment('{'); st = State.Comment; break;
                    case '.': rdr.Read(); st = State.Dot; break;
                    case '(': rdr.Read(); st = State.LParen; break;
                    case '+': rdr.Read(); return Tok(TokenType.Plus);
                    case '-': rdr.Read(); st = State.Minus; break;
                    case '*': rdr.Read(); return Tok(TokenType.Star);
                    case '/': rdr.Read(); return Tok(TokenType.Slash);
                    case ':': rdr.Read(); return Tok(TokenType.Colon);
                    case ',': rdr.Read(); return Tok(TokenType.Comma);
                    case '=': rdr.Read(); return Tok(TokenType.Eq);
                    case ';': rdr.Read(); return Tok(TokenType.Semi);
                    case '[': rdr.Read(); return Tok(TokenType.LBracket);
                    case '^': rdr.Read(); return Tok(TokenType.Ptr);
                    case ']': rdr.Read(); return Tok(TokenType.RBracket);
                    case ')': rdr.Read(); return Tok(TokenType.RParen);
                    case '$': rdr.Read(); sb.Clear(); st = State.HexNumber; break;
                    case '\'': rdr.Read(); sb.Clear(); st = State.String; break;
                    case '\"': rdr.Read(); sb.Clear(); st = State.DString; break;
                    default:
                        if ('A' <= ch && ch <= 'Z' || 'a' <= ch && ch <= 'z' || ch == '_')
                        {
                            rdr.Read();
                            sb.Clear();
                            sb.Append(ch);
                            st = State.Identifier;
                            break;
                        }
                        if ('0' <= ch && ch <= '9')
                        {
                            rdr.Read();
                            sb.Clear();
                            sign = 1;
                            sb.Append(ch);
                            st = State.Number;
                            break;
                        }
                        Nyi(st, ch);
                        break;
                    }
                    break;
                case State.Cr:
                    if (c < 0)
                        return Tok(TokenType.EOF);
                    if (ch == '\n')
                    {
                        rdr.Read();
                    }
                    ++lineNumber;
                    st = State.Init;
                    break;
                case State.Comment:
                    if (c < 0)
                        return Tok(TokenType.EOF);
                    switch (ch)
                    {
                    case '\r':
                        rdr.Read();
                        c = rdr.Peek();
                        if (c == '\n')
                        {
                            rdr.Read();
                        }
                        ++lineNumber;
                        break;
                    case '\n': rdr.Read(); ++lineNumber; break;
                    case '(': rdr.Read(); st = State.CommentLparen; break;
                    case '{': rdr.Read(); NestComment('{'); break;
                    case '*': rdr.Read(); st = State.CommentStar; break;
                    case '}':
                        rdr.Read();
                        if (UnnestComment('{'))
                        {
                            if (nestedComments.Count == 0)
                                st = State.Init;
                        }
                        break;
                    default:
                        rdr.Read();
                        break;
                    }
                    break;
                case State.CommentLparen:
                    if (c < 0)
                        return Tok(TokenType.EOF);  //$TODO: should raise 'expected '}'
                    if (ch == '*')
                    {
                        rdr.Read();
                        NestComment('(');
                    }
                    st = State.Comment;
                    break;
                case State.CommentStar:
                    if (c < 0)
                        return Tok(TokenType.EOF);  //$TODO: should raise 'expected ')'
                    if (ch == ')')
                    {
                        rdr.Read();
                        if (UnnestComment('('))
                        {
                            if (nestedComments.Count == 0)
                            {
                                st = State.Init;
                                break;
                            }
                        }
                    }
                    st = State.Comment;
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
                case State.Dot:
                    if (c < 0 || ch != '.')
                        return Tok(TokenType.Dot);
                    rdr.Read();
                    if (rdr.Peek() == '.')
                    {
                        rdr.Read();
                        return Tok(TokenType.DotDotDot);    // MPW extension
                    }
                    return Tok(TokenType.DotDot);
                case State.Minus:
                    if (c < 0)
                        return Tok(TokenType.Minus);
                    if ('0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        this.sign = -1;
                        sb.Clear();
                        sb.Append(ch);
                        st = State.Number;
                        break;
                    }
                    return Tok(TokenType.Minus);
                case State.Number:
                    if (c < 0)
                        return Tok(TokenType.Number, sign * Int64.Parse(sb.ToString()));
                    if ('0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    if (ch == 'e' || ch == 'E')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        st = State.RealExponentStart;
                        break;
                    }
                    if (ch == '.')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        st = State.NumberDot;
                        break;
                    }
                    return Tok(TokenType.Number, sign * Int64.Parse(sb.ToString()));
                case State.NumberDot:
                    if (c < 0)
                        return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                    if ('0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        st = State.Real;
                        break;
                    }
                    if (ch == '.')
                    {
                        sb.Remove(sb.Length - 1, 1);
                        return Tok(TokenType.Number, State.Dot, Int64.Parse(sb.ToString()));
                    }
                    return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                case State.Real:
                    if (c < 0)
                        return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                    if (ch == 'e' || ch == 'E')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        st = State.RealExponentStart;
                        break;
                    }
                    else if ('0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                case State.RealExponentStart:
                    if (c < 0)
                        return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                    if (ch == '+' || ch == '-' || ('0' <= ch && ch <= '9'))
                    {
                        rdr.Read();
                        sb.Append(ch);
                        st = State.RealExponent;
                        break;
                    }
                    return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                case State.RealExponent:
                    if (c < 0)
                        return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                    if ('0' <= ch && ch <= '9')
                    {
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    return Tok(TokenType.RealLiteral, sign * ParseDouble(sb.ToString()));
                case State.HexNumber:
                    if (c < 0)
                        return Tok(TokenType.Number, Convert.ToInt64(sb.ToString(), 16));
                    if (IsHexDigit(ch))
                    {
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    return Tok(TokenType.Number, Convert.ToInt64(sb.ToString(), 16));
                case State.String:
                    if (c < 0)
                        return Tok(TokenType.StringLiteral, sb.ToString());
                    rdr.Read();
                    if (ch == '\'')
                    {
                        return Tok(TokenType.StringLiteral, sb.ToString());
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                    break;
                case State.DString:
                    if (c < 0)
                        return Tok(TokenType.StringLiteral, sb.ToString());
                    rdr.Read();
                    if (ch == '\"')
                    {
                        return Tok(TokenType.StringLiteral, sb.ToString());
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                    break;
                case State.LParen:
                    if (c < 0)
                        return Tok(TokenType.LParen);
                    if (ch == '*')
                    {
                        rdr.Read();
                        NestComment('(');
                        st = State.Comment;
                        break;
                    }
                    return Tok(TokenType.LParen);
                }
            }
        }

        // #1050: double parsing was failing in locales where '.' is not a decimal separator.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ParseDouble(string sDouble)
        {
            return double.Parse(sDouble, CultureInfo.InvariantCulture);
        }

        private bool UnnestComment(char commentChar)
        {
            if (nestedComments[^1].Item1 == commentChar)
            {
                nestedComments.RemoveAt(nestedComments.Count - 1);
                return true;
            }
            return false;
        }

        private void NestComment(char commentChar)
        {
            if (allowNestedComments || nestedComments.Count == 0)
            {
                nestedComments.Add((commentChar, lineNumber));
            }
        }

        private Token MaybeKeyword(string str)
        {
            st = State.Init;
            TokenType tok;
            if (keywords.TryGetValue(str, out tok))
                return new Token(tok);
            else
                return new Token(TokenType.Id, str);
        }

        private void Nyi(State state, char ch)
        {
            throw new NotImplementedException(string.Format("Line: {0}, State {1}, ch: {2} (U+{3:X4})", lineNumber, state, ch, (uint)ch));
        }

        private Token Tok(TokenType type, object? value = null)
        {
            st = State.Init;
            return new Token(type, value);
        }

        private Token Tok(TokenType type, State st, object? value = null)
        {
            this.st = st;
            return new Token(type, value);
        }

        private static bool IsHexDigit(char ch)
        {
            switch (ch)
            {
            case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8':  case '9':
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': 
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': 
                return true;
            default:
                return false;
            }
        }
    }
}
