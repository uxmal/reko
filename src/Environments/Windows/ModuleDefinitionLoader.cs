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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Reads Microsoft module definition (.def) files.
    /// http://msdn.microsoft.com/en-us/library/28d6s79h.aspx
    /// </summary>
    public class ModuleDefinitionLoader : MetadataLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ModuleDefinitionLoader), "Traces for ModuleDefinitionLoader") { Level = TraceLevel.Warning };

        private readonly Lexer lexer;
        private readonly ImageLocation fileUri;
        private Token? bufferedTok;
        private IPlatform? platform;
        private string? moduleName;

        public ModuleDefinitionLoader(IServiceProvider services, ImageLocation imageUri, byte[]  bytes) : base(services, imageUri, bytes)
        {
            this.fileUri = imageUri;
            var rdr = new StreamReader(new MemoryStream(bytes));
            this.lexer = new Lexer(rdr);
            this.bufferedTok = null;
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            this.platform = platform;
            var loader = new TypeLibraryDeserializer(platform, true, dstLib);
            this.moduleName = DefaultModuleName(fileUri.FilesystemPath);
            loader.SetModuleName(moduleName);
            for (;;)
            {
                var tok = Get();
                switch (tok.Type)
                {
                case TokenType.EOF: return dstLib;
                case TokenType.EXPORTS: ParseExports(loader); break;
                case TokenType.LIBRARY: ParseLibrary(loader); break;
                default: throw new NotImplementedException(
                    string.Format("Unknown token {0} ({1}) on line {2}.",
                    tok.Type,
                    tok.Text,
                    tok.LineNumber));
                }
            }
        }

        private string DefaultModuleName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename).ToUpper() + ".DLL";
        }

        private void ParseExports(TypeLibraryDeserializer deserializer)
        {
            while (Peek().Type == TokenType.Id)
            {
                string entryName = Expect(TokenType.Id)!;
                string? internalName = null;
                if (PeekAndDiscard(TokenType.Eq))
                {
                    internalName = Expect(TokenType.Id)!;
                }
                int ordinal = -1;
                if (PeekAndDiscard(TokenType.At))
                {
                    var num = Get();
                    switch (num.Type)
                    {
                    case TokenType.Number: ordinal = Convert.ToInt32(num.Text); break;
                    case TokenType.HexNumber: ordinal = Convert.ToInt32(num.Text, 16); break;
                    default: throw new FormatException("Expected a number after '@'.");
                    }
                    PeekAndDiscard(TokenType.NONAME);
                }
                PeekAndDiscard(TokenType.PRIVATE);
                PeekAndDiscard(TokenType.DATA);

                var ep = ParseSignature(entryName!, deserializer);
                var svc = new SystemService
                {
                    ModuleName = moduleName,
                    Name = ep is not null ? ep.Name : entryName,
                    Signature = ep?.Signature,
                };
                trace.Verbose("Loaded {0} @ {1}", entryName!, ordinal);
                if (ordinal != -1)
                {
                    svc.SyscallInfo = new SyscallInfo { Vector = ordinal };
                    deserializer.LoadService(ordinal, svc);
                }
                deserializer.LoadService(entryName, svc);
            }
        }

        private void ParseLibrary(TypeLibraryDeserializer lib)
        {
            if (Peek().Type == TokenType.Id)
            {
                moduleName = Get().Text!.ToUpper();
                lib.SetModuleName(moduleName);
            }
            if (PeekAndDiscard(TokenType.BASE))
            {
                throw new NotImplementedException();
            }
        }

        private ExternalProcedure? ParseSignature(string entryName, TypeLibraryDeserializer loader)
        {
            var sProc = SignatureGuesser.SignatureFromName(entryName, platform!);
            if (sProc is null)
                return null;
            return loader.LoadExternalProcedure(sProc);
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
            if (bufferedTok is null)
                bufferedTok = lexer.GetToken();
            return bufferedTok;
        }

        private Token Get()
        {
            Token? t = bufferedTok;
            if (t is null)
                t = lexer.GetToken();
            bufferedTok = null;
            return t;
        }

        private string? Expect(TokenType type)
        {
            var tok = Get();
            if (tok.Type != type)
                throw new FormatException($"Expected token type {type} but saw {tok.Type}.");
            return tok.Text;
        }

        public enum TokenType
        {
            EOF,

            BASE,
            DATA,
            EXPORTS,
            LIBRARY,
            NAME,
            NONAME,
            PRIVATE,
            SECTIONS,

            At,
            Colon,
            Eq,
            Id,
            Number,
            HexNumber,
        }

        public class Token
        {
            public TokenType Type;
            public string? Text;
            public int LineNumber;

            public Token(TokenType tokenType, int lineNumber)
            {
                this.Type = tokenType;
                this.LineNumber = lineNumber;
            }

            public Token(TokenType tokenType, string p, int lineNumber)
            {
                this.Type = tokenType;
                this.Text = p;
                this.LineNumber = lineNumber;
            }
        }

        private class Lexer
        {
            private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType> {
                { "BASE", TokenType.BASE},
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
                Zero,
                HexPrefix,
                HexNumber,
            }

            private readonly TextReader rdr;
            private int lineNumber;

            public Lexer(TextReader reader)
            {
                this.rdr = reader;
                this.lineNumber = 1;
            }

            public Token GetToken()
            {
                var st = State.Initial;
                StringBuilder? sb = null;
                for (; ; )
                {
                    int c = rdr.Peek();
                    char ch = (char) c;
                    switch (st)
                    {
                    case State.Initial:
                        switch (c)
                        {
                        case -1: return new Token(TokenType.EOF, lineNumber);
                        case '\"':
                            sb = new StringBuilder();
                            rdr.Read();
                            st = State.String;
                            break;
                        case '@': rdr.Read(); return new Token(TokenType.At, lineNumber);
                        case '=': rdr.Read(); return new Token(TokenType.Eq, lineNumber);
                        case ':': rdr.Read(); return new Token(TokenType.Colon, lineNumber);
                        case ';': rdr.Read(); st = State.Comment; break;
                        case '0': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.Zero; break;
                        case '\r': rdr.Read(); break;
                        case '\n': ++lineNumber; rdr.Read(); break;
                        default:
                            rdr.Read();
                            if (Char.IsWhiteSpace(ch))
                                break;
                            sb = new StringBuilder();
                            sb.Append(ch);
                            if (char.IsDigit(ch))
                                st = State.Number;
                            else 
                                st = State.Id;
                            break;
                        }
                        break;
                    case State.Comment:
                        if (c == -1)
                            return new Token(TokenType.EOF, lineNumber);
                        if (ch == '\r' || ch == '\n')
                        {
                            ++lineNumber;
                            st = State.Initial;
                        }
                        rdr.Read();
                        break;
                    case State.String:
                        switch (c)
                        {
                        case -1: throw new FormatException("Unterminated string.");
                        case '\r':
                        case '\n': throw new FormatException("Newline in string.");
                        case '\"': rdr.Read(); return new Token(TokenType.Id, sb!.ToString(), lineNumber);
                        default:
                            rdr.Read();
                            sb!.Append(ch);
                            break;
                        }
                        break;
                    case State.Id:
                        if (c == -1)
                            return IdOrKeyword(sb!);
                        if (Char.IsWhiteSpace(ch) || c == ';')
                            return IdOrKeyword(sb!);
                        sb!.Append(ch);
                        rdr.Read();
                        break;
                    case State.Number:
                        if (c == -1 || !Char.IsDigit(ch))
                            return Tok(TokenType.Number, sb!);
                        rdr.Read();
                        sb!.Append(ch);
                        break;
                    case State.Zero:
                        if (c == -1)
                            return Tok(TokenType.Number, sb!);
                        switch (ch)
                        {
                        case 'x': case 'X':
                            rdr.Read();
                            sb!.Clear();
                            st = State.HexPrefix;
                            break;
                        default:
                            return Tok(TokenType.Number, sb!);
                        }
                        break;
                    case State.HexPrefix:
                        if (c != -1 && BytePattern.IsHexDigit(ch))
                        { 
                            rdr.Read();
                            sb!.Append(ch);
                            st = State.HexNumber;
                            break;
                        }
                        throw new FormatException("Invalid hexadecimal constant.");
                    case State.HexNumber:
                        if (c != -1 && BytePattern.IsHexDigit(ch))
                        {
                            rdr.Read();
                            sb!.Append(ch);
                            st = State.HexNumber;
                            break;
                        }
                        return Tok(TokenType.HexNumber, sb!);
                    }
                }
            }

            private Token IdOrKeyword(StringBuilder sb)
            {
                var id = sb.ToString();
                if (keywords.TryGetValue(id, out TokenType t))
                    return new Token(t, lineNumber);
                else
                    return new Token(TokenType.Id, id, lineNumber);
            }

            private Token Tok(TokenType type, StringBuilder sb)
            {
                return new Token(type, sb.ToString(), lineNumber);
            }
        }
    }
}
