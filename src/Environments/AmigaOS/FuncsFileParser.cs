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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// http://amiga.sourceforge.net/amigadevhelp/phpwebdev.php?keyword=FindTask&funcgroup=AmigaOS&action=Search
namespace Reko.Environments.AmigaOS
{
    /// <summary>
    /// Parses AmigaOS funcs file, with extended support for return registers and data types.
    /// </summary>
    public class FuncsFileParser
    {
        private readonly M68kArchitecture arch;
        private readonly TextReader streamReader;
        private Lexer lex;

        public FuncsFileParser(M68kArchitecture arch, TextReader streamReader)
        { 
            this.arch = arch;
            this.streamReader = streamReader;
            this.FunctionsByLibBaseOffset = new Dictionary<int, AmigaSystemFunction>();
            this.lex = null!;
        }

        public void Parse()
        {
            for (; ; )
            {
                var line = streamReader.ReadLine();
                if (line is null)
                    break;
                AmigaSystemFunction? func = ParseLine(line);
                if (func is not null)
                    FunctionsByLibBaseOffset.Add(func.Offset, func);
            }
        }

        private AmigaSystemFunction? ParseLine(string line)
        {
            lex = new Lexer(line);
            if (!PeekAndDiscard(TokenType.HASH))
                return null;
            Expect(TokenType.Number);   // Ordinal
            var decOffset = (int) Expect(TokenType.Number)!;   // offset (in decimal)
            var hexOffset = (int) Expect(TokenType.Number)!;   // offset (in hex)
            if (decOffset != hexOffset)
                throw new InvalidOperationException(string.Format(
                    "Decimal Offset {0} and hexadecimal Offset {1:X} do not match.", decOffset, hexOffset));
            var name = (string) Expect(TokenType.Id)!;         // fn name

            var parameters = new List<Argument_v1>();
            // GitHub #941. Assume that all functions return something in d0. If they don't,
            // calling code shouldn't be reading d0 after the call anyway, since d0 is a scratch
            // register on AmigaOS.
            Argument_v1 returnValue = new Argument_v1
            {
                Kind = new Register_v1 { Name = "d0" }
            };
            Expect(TokenType.LPAREN);
            if (!PeekAndDiscard(TokenType.RPAREN))
            {
                do {
                    var argName = (string) Expect(TokenType.Id)!;
                    Expect(TokenType.SLASH);
                    var regName = (string)Expect(TokenType.Id)!;
                    parameters.Add(new Argument_v1
                    {
                        Kind = new Register_v1 { Name = regName },
                        Name = argName,
                    });
                } while (PeekAndDiscard(TokenType.COMMA));
                if (PeekAndDiscard(TokenType.SEMI))
                {
                    var argName = (string) Expect(TokenType.Id)!;
                    Expect(TokenType.SLASH);
                    var regName = (string) Expect(TokenType.Id)!;
                    returnValue = new Argument_v1
                    {
                        Kind = new Register_v1 { Name = regName }
                    };
                }
                Expect(TokenType.RPAREN);
            }
            return new AmigaSystemFunction
            {
                Offset = -decOffset,
                Name = name,
                Signature = new SerializedSignature
                {
                    Arguments = parameters.ToArray(),
                    ReturnValue = returnValue,
                }
            };
        }

        private bool PeekAndDiscard(TokenType type)
        {
            var tok = lex.PeekToken();
            if (tok.Type == type)
            {
                lex.GetToken();
                return true;
            }
            else
                return false;
        }

        private object? Expect(TokenType type)
        {
            var tok = lex.GetToken();
            if (tok.Type != type)
                throw new InvalidOperationException(string.Format(
                    "Expected {0} but read {1} ({2}).", type, tok.Type, tok.Value));
            return tok.Value;
        }
        
        public Dictionary<int, AmigaSystemFunction> FunctionsByLibBaseOffset { get; private set; }

        private class Lexer
        {
            private readonly string line;
            private int pos;
            private Token? token;
            private StringBuilder sb;

            public Lexer(string line)
            {
                this.line = line;
                this.sb = new StringBuilder();
            }

            public Token PeekToken()
            {
                if (token is null)
                {
                    token = ReadToken();
                }
                return token;
            }

            public Token GetToken()
            {
                if (token is null)
                {
                    return ReadToken();
                }
                var t = token;
                token = null;
                return t;
            }

            private enum State
            {
                Initial,
                Id,
                Number,
                HexNumber,
            }

            private Token ReadToken()
            {
                State st = State.Initial;
                for (;;)
                {
                    int c = pos < line.Length ? line[pos++] : -1;
                    char ch = (char)c;
                    switch (st)
                    {
                    case State.Initial:
                        switch (c)
                        {
                        case -1: return new Token(TokenType.EOF);
                        case '#': return new Token(TokenType.HASH);
                        case '(': return new Token(TokenType.LPAREN);
                        case ')': return new Token(TokenType.RPAREN);
                        case '/': return new Token(TokenType.SLASH);
                        case ',': return new Token(TokenType.COMMA);
                        case ';': return new Token(TokenType.SEMI);
                        case ' ':
                        case '\t':
                            break;       // Ignore whitespace
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
                            sb.Clear();
                            sb.Append(ch);
                            st = State.Number;
                            break;
                        default:
                            if (Char.IsLetter(ch))
                            {
                                sb.Clear();
                                sb.Append(ch);
                                st = State.Id;
                            }
                            break;
                        }
                        break;
                    case State.Number:
                        switch (c)
                        {
                        case 'x':
                            sb.Clear();
                            st = State.HexNumber;
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
                            sb.Append(ch);
                            break;
                        case -1:
                            return new Token(TokenType.Number, Convert.ToInt32(sb.ToString()));
                        default:
                            --pos;
                            return new Token(TokenType.Number, Convert.ToInt32(sb.ToString()));
                        }
                        break;
                    case State.HexNumber:
                        switch (c)
                        {
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
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            sb.Append(ch);
                            break;
                        case -1:
                            return new Token(TokenType.Number, Convert.ToInt32(sb.ToString(), 16));
                        default:
                            --pos;
                            return new Token(TokenType.Number, Convert.ToInt32(sb.ToString(), 16));
                        }
                        break;
                    case State.Id:
                        if (c == -1)
                        {
                            return new Token(TokenType.Id, sb.ToString());
                        }
                        else if (Char.IsLetter(ch) || Char.IsDigit(ch))
                        {
                            sb.Append(ch);
                        }
                        else
                        {
                            --pos;
                            return new Token(TokenType.Id, sb.ToString());
                        }
                        break;
                    }
                }
            }
        }

        private enum TokenType
        {
            EOF,
            HASH,
            SLASH,
            Number,
            Id,
            LPAREN,
            RPAREN,
            COMMA,
            SEMI
        }

        private class Token
        {
            public TokenType Type;
            public object? Value;

            public Token(TokenType tokenType)
            {
                this.Type = tokenType;
            }

            public Token(TokenType tokenType, object value)
            {
                this.Type = tokenType;
                this.Value = value;
            }
        }
    }
}
