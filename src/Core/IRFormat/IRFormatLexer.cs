#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core.IRFormat
{
    public class IRFormatLexer
    {
        private readonly TextReader rdr;
        private Token tok;
        private int lineNumber;

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
        }

        public Token Read()
        {
            var st = State.Initial;
            var sb = new StringBuilder();
            int sigilBitsize = 0;
            long constantValue = 0;
            DataType? dtSigil = null;
            var sigilDomain = Domain.None;

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
                        Advance(State.Initial);
                        return new Token(IRTokenType.ASSIGN);
                    default:
                        if (Char.IsLetter(ch) || ch == '_')
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
                        Debug.Assert(dtSigil is not null);
                        return GenerateConstantToken(dtSigil, constantValue);
                    }
                    return Expected("Expected '>' in number sigil.");
                case State.ID:
                    if (c == -1 ||
                        !char.IsLetterOrDigit(ch) && ch != '_')
                    {
                        return new Token(IRTokenType.ID, sb.ToString());
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
                default:
                    Debug.Fail(st.ToString());
                    return Token.Error("Not implemented.");
                }
            }
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