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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core.IRFormat
{
    /// <summary>
    /// Lexer for the IR format.
    /// </summary>
    public class IRFormatLexer
    {
        private static readonly IReadOnlyDictionary<string, IRTokenType> keywords = new Dictionary<string, IRTokenType>()
        {
            { "if", IRTokenType.If },
            { "goto", IRTokenType.Goto },
        };

        private readonly TextReader rdr;
        private int lineNumber;

        /// <summary>
        /// Constructs a lexer for the IR format.
        /// </summary>
        /// <param name="rdr">Textreader of the source code.</param>
        public IRFormatLexer(TextReader rdr)
        {
            this.rdr = rdr;
            lineNumber = 1;
        }

        private enum State
        {
            Initial,
            ID,
            DecimalDigit,
            Zero,
            NumberSigilStart,
            NumberSigilBitsize,
            NumberSigilEnd,
            Asterisk,
            Slash,
            LineComment,
            Lt,
            Gt,
            Shr,
            Amp,
            Pipe,
            Caret,
            Equal,
            Bang,
        }

        /// <summary>
        /// Reads a token from the input stream.
        /// </summary>
        /// <returns>The read token.</returns>
        public Token Read()
        {
            var st = State.Initial;
            var sb = new StringBuilder();
            long constantValue = 0;
            DataType? dtSigil = null;

            for (;;)
            {
                int c = rdr.Peek();
                char ch = (char)c;
                switch (st)
                {
                case State.Initial:
                    switch (c)
                    {
                    case -1: return new Token(IRTokenType.EOF);
                    case ' ':
                    case '\t':
                    case '\r':
                        Advance(State.Initial);
                        break;
                    case '\n':
                        ++lineNumber;
                        Advance(State.Initial);
                        break;
                    case '0':
                        sb.Append('0');
                        st = Advance(State.Zero);
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        constantValue = c - '0';
                        st = Advance(State.DecimalDigit);
                        break;
                    case '+':
                        Advance(State.Initial);
                        return new Token(IRTokenType.PLUS);
                    case '-':
                        Advance(State.Initial);
                        return new Token(IRTokenType.MINUS);
                    case '*':
                        st = Advance(State.Asterisk);
                        break;
                    case '/':
                        st = Advance(State.Slash);
                        break;
                    case '=':
                        st = Advance(State.Equal);
                        break;
                    case '!':
                        st = Advance(State.Bang);
                        break;
                    case '[':
                        Advance(State.Initial);
                        return new Token(IRTokenType.LBRACKET);
                    case ']':
                        Advance(State.Initial);
                        return new Token(IRTokenType.RBRACKET);
                    case '(':
                        Advance(State.Initial);
                        return new Token(IRTokenType.LPAREN);
                    case ')':
                        Advance(State.Initial);
                        return new Token(IRTokenType.RPAREN);
                    case ':':
                        Advance(State.Initial);
                        return new Token(IRTokenType.COLON);
                    case '<': st = Advance(State.Lt); break;
                    case '>': st = Advance(State.Gt); break;
                    case '&': st = Advance(State.Amp); break;
                    case '|': st = Advance(State.Pipe); break;
                    case '^': st = Advance(State.Initial); return new Token(IRTokenType.XOR);
                    default:
                        if (char.IsLetter(ch) || ch == '_')
                        {
                            sb.Append(ch);
                            st = Advance(State.ID);
                            break;
                        }
                        return Unexpected(c);
                    }
                    break;
                case State.Zero:
                case State.DecimalDigit:
                    switch (c)
                    {
                    default:
                        return Expected("Expected a type sigil.");
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        constantValue = constantValue*10 + (c - '0');
                        st = Advance(State.DecimalDigit);
                        break;
                    case '<':
                        Advance(State.NumberSigilStart);
                        dtSigil = ReadSigil();
                        st = State.NumberSigilEnd;
                        break;
                    }
                    break;
                case State.NumberSigilEnd:
                    if (c == '>')
                    {
                        Advance(State.Initial);
                        Debug.Assert(dtSigil is not null);
                        return GenerateConstantToken(dtSigil, constantValue);
                    }
                    return Expected("Expected '>' in number sigil.");
                case State.ID:
                    if (c == -1 ||
                        !char.IsLetterOrDigit(ch) && ch != '_')
                    {
                        return MaybeKeyword(sb.ToString());
                    }
                    sb.Append(ch);
                    Advance(State.ID);
                    break;
                case State.Asterisk:
                    switch (c)
                    {
                    case 's':
                        dtSigil = ReadSigil();
                        return new Token(IRTokenType.SMUL, dtSigil);
                    case 'u':
                        dtSigil = ReadSigil();
                        return new Token(IRTokenType.UMUL, dtSigil);
                    case 'f':
                        dtSigil = ReadSigil();
                        return new Token(IRTokenType.FMUL, dtSigil);
                    default:
                        return new Token(IRTokenType.MUL);
                    }
                case State.Slash:
                    switch (c)
                    {
                    case 's':
                        Advance(State.Initial);
                        return new Token(IRTokenType.SDIV);
                    case 'u':
                        Advance(State.Initial);
                        return new Token(IRTokenType.UDIV);
                    case '/':
                        st = Advance(State.LineComment);
                        break;
                    }
                    Unexpected(c);
                    break;
                case State.LineComment:
                    switch (c)
                    {
                    case '\n':
                        ++lineNumber;
                        st = Advance(State.Initial);
                        break;
                    case -1:
                        return new Token(IRTokenType.EOF);
                    default:
                        Advance(State.LineComment);
                        break;
                    }
                    break;
                case State.Equal:
                    switch (c)
                    {
                    default: return new Token(IRTokenType.LogicalNot);
                    case '=':
                        Advance(State.Initial);
                        return new Token(IRTokenType.EQ);
                    }
                case State.Bang:
                    switch (c)
                    {
                    default: return new Token(IRTokenType.LogicalNot);
                    case '=':
                        Advance(State.Initial);
                        return new Token(IRTokenType.NE);
                    }
                case State.Lt:
                    switch (c)
                    {
                    default: return new Token(IRTokenType.LT);
                    case '<':
                        Advance(State.Initial);
                        return new Token(IRTokenType.SHL);
                    case '=':
                        Advance(State.Initial);
                        return new Token(IRTokenType.LE);
                    }
                case State.Gt:
                    switch (c)
                    {
                    default: return new Token(IRTokenType.GT);
                    case '>':
                        st = Advance(State.Shr);
                        break;
                    case '=':
                        Advance(State.Initial);
                        return new Token(IRTokenType.GE);
                    }
                    break;
                case State.Shr:
                    if (c == 'u')
                    {
                        Advance(State.Initial);
                        return new Token(IRTokenType.Goto);
                    }
                    return new Token(IRTokenType.SAR);
                case State.Amp:
                    if (c == '&')
                    {
                        st = Advance(State.Initial);
                        return new Token(IRTokenType.LogicalAnd);
                    }
                    return new Token(IRTokenType.BinaryAnd);
                case State.Pipe:
                    if (c == '|')
                    {
                        st = Advance(State.Initial);
                        return new Token(IRTokenType.LogicalOr);
                    }
                    return new Token(IRTokenType.BinaryOr);
                default:
                    Debug.Fail(st.ToString());
                    return Token.Error("Not implemented.");
                }
            }
        }

        private Token MaybeKeyword(string id)
        {
            if (keywords.TryGetValue(id, out var type))
                return new Token(type);
            return new Token(IRTokenType.ID, id);
        }

        private DataType? ReadSigil()
        {
            Domain sigilDomain = Domain.None;
            State st = State.NumberSigilStart;
            int bitsize = 0;
            for (; ; )
            {
                int c = rdr.Peek();
                char ch = (char) c;
                switch (st)
                {
                case State.NumberSigilStart:
                    switch (c)
                    {
                    default:
                        return null;
                    case 'i':
                    case 's':
                        sigilDomain = Domain.SignedInt;
                        st = Advance(State.NumberSigilBitsize);
                        break;
                    case 'u':
                        sigilDomain = Domain.UnsignedInt;
                        st = Advance(State.NumberSigilBitsize);
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        bitsize = ch - '0';
                        st = Advance(State.NumberSigilBitsize);
                        break;
                    }
                    break;
                case State.NumberSigilBitsize:
                    switch (c)
                    {
                    default:
                        if (bitsize == 0)
                            return null;
                        var dt = (sigilDomain == Domain.None)
                            ? PrimitiveType.CreateWord(bitsize)
                            : PrimitiveType.Create(sigilDomain, bitsize);
                        return dt;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        bitsize = bitsize * 10 + ch - '0';
                        Advance(State.NumberSigilBitsize);
                        break;
                    }
                    break;
                }
            }
        }

        private State Advance(State state)
        {
            rdr.Read();
            return state;
        }

        private static Token GenerateConstantToken(DataType dt, long value)
        {
            var c = Constant.Create(dt, value);
            return new Token(IRTokenType.CONST, c);
        }

        // 0x00
        // -123
        // +0.32p3
        // c"..."
        // w"...."
        // + - * /
        // +f -f *f /f
        // [


        private Token Unexpected(int c)
        {
            return new Token(IRTokenType.ERROR, $"Unexpected character '{(char) c} (U+{c:X4})");
        }

        private Token Expected(string errorMessage)
        {
            return Token.Error(errorMessage);
        }
    }
}