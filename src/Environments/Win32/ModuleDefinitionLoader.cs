#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Decompiler.Environments.Win32
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/28d6s79h.aspx
    /// </summary>
    public class ModuleDefinitionLoader
    {
        Lexer lexer;
        Token bufferedTok;

        public ModuleDefinitionLoader(TextReader rdr)
        {
            this.lexer = new Lexer(rdr);
            this.bufferedTok = null;
        }

        public TypeLibrary Load()
        {
            TypeLibrary lib = new TypeLibrary();
            for (; ; )
            {
                var tok = Get();
                switch (tok.Type)
                {
                case TokenType.EOF: return lib;
                case TokenType.EXPORTS: ParseExports(lib); break;
                default: throw new NotImplementedException(string.Format("Unknown token {0}.", tok.Type));
                }
            }
        }

        private void ParseExports(TypeLibrary lib)
        {
            string entryName = Expect(TokenType.Id);
            string internalName = null;
            if (PeekAndDiscard(TokenType.Eq))
            {
                internalName = Expect(TokenType.Id);
            }
            int ordinal = -1;
            var tok = Peek();
            if (tok.Type == TokenType.Number)
            {
                ordinal = Convert.ToInt32(tok.Text);
                PeekAndDiscard(TokenType.NONAME);
            }
            PeekAndDiscard(TokenType.PRIVATE);
            PeekAndDiscard(TokenType.DATA);

            var svc = new SystemService
            {
                Name = entryName,
                Signature = ParseSignature(entryName),
                SyscallInfo = new SyscallInfo
                {
                    Vector = ordinal,
                },
            };
        }

        private ProcedureSignature ParseSignature(string entryName)
        {
            throw new NotImplementedException();
            if (string.IsNullOrEmpty(entryName))
                throw new ArgumentException("entryName", "Function name was blank.");
        }

        private bool PeekAndDiscard(TokenType type)
        {
            if (Peek().Type != type)
                return false;

            Get();
            return true;
        }

        private Token Peek()
        {
            if (bufferedTok == null)
                bufferedTok = lexer.GetToken();
            return bufferedTok;
        }

        private Token Get()
        {
            Token t = bufferedTok;
            if (t == null)
                t = lexer.GetToken();
            bufferedTok = null;
            return t;
        }

        private string Expect(TokenType type)
        {
            var tok = Get();
            if (tok.Type != type)
                throw new FormatException(string.Format("Expected token type {0} but saw {1}.", type, tok.Type));
            return tok.Text;
        }

        public enum TokenType
        {
            EOF,

            EXPORTS,
            LIBRARY,
            NAME,
            NONAME,
            SECTIONS,

            Colon,
            Eq,
            Id,
            Number,
            PRIVATE,
            DATA,
        }

        public class Token
        {
            public TokenType Type;
            public string Text;

            public Token(TokenType tokenType)
            {
                this.Type = tokenType;
            }

            public Token(TokenType tokenType, string p)
            {
                this.Type = tokenType;
                this.Text = p;
            }
        }

        private class Lexer
        {
            private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType> {
                { "DATA", TokenType.DATA },
                { "EXPORTS", TokenType.EXPORTS },
                { "LIBRARY" ,TokenType.LIBRARY },
                { "NAME", TokenType.NAME },
                { "NONAME", TokenType.NONAME },
                { "PRIVATE", TokenType.PRIVATE },
                { "SECTIONS", TokenType.SECTIONS },
            };

            private enum State
            {
                Initial,
                Comment,
                Id,
                String,
                Number,
            }

            private TextReader rdr;

            public Lexer(TextReader reader)
            {
                this.rdr = reader;
            }

            public Token GetToken()
            {
                var st = State.Initial;
                StringBuilder sb = null;
                for (; ; )
                {
                    int c = rdr.Peek();
                    char ch = (char) c;
                    switch (st)
                    {
                    case State.Initial:
                        switch (c)
                        {
                        case -1: return new Token(TokenType.EOF);
                        case '\"':
                            sb = new StringBuilder();
                            rdr.Read();
                            st = State.String;
                            break;
                        case '@': rdr.Read(); sb = new StringBuilder(); st = State.Number; break;
                        case '=': rdr.Read(); return new Token(TokenType.Eq);
                        case ':': rdr.Read(); return new Token(TokenType.Colon);
                        case ';': rdr.Read(); st = State.Comment; break;
                        default:
                            rdr.Read();
                            if (Char.IsWhiteSpace(ch))
                                break;
                            sb = new StringBuilder();
                            st = State.Id;
                            break;
                        }
                        break;
                    case State.Comment:
                        if (c == -1)
                            return new Token(TokenType.EOF);
                        if (ch == '\r' || ch == '\n')
                            st = State.Initial;
                        rdr.Read();
                        break;
                    case State.String:
                        switch (c)
                        {
                        case -1: throw new FormatException("Unterminated string.");
                        case '\r':
                        case '\n': throw new FormatException("Newline in string.");
                        case '\"': rdr.Read(); return new Token(TokenType.Id, sb.ToString());
                        default:
                            rdr.Read();
                            sb.Append(ch);
                            break;
                        }
                        break;
                    case State.Id:
                        if (c == -1)
                            return IdOrKeyword(sb);
                        if (Char.IsWhiteSpace(ch) || c == ';')
                            return IdOrKeyword(sb);
                        sb.Append(ch);
                        rdr.Read();
                        break;
                    case State.Number:
                        if (c == -1 || !Char.IsDigit(ch))
                            return Tok(TokenType.Number, sb);
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                }
            }

            private Token IdOrKeyword(StringBuilder sb)
            {
                var id = sb.ToString();
                TokenType t;
                if (keywords.TryGetValue(id, out t))
                    return new Token(t);
                else
                    return new Token(TokenType.Id, id);
            }

            private Token Tok(TokenType type, StringBuilder sb)
            {
                return new Token(type, sb.ToString());
            }
        }
    }
}
