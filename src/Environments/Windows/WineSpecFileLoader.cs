#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    // https://www.winehq.org/docs/winelib-guide/spec-file
    public class WineSpecFileLoader : MetadataLoader
    {
        private string filename;
        private Lexer lexer;
        private Token bufferedToken;
        private TypeLibraryDeserializer tlLoader;
        private IPlatform platform;
        private string moduleName;

        public WineSpecFileLoader(IServiceProvider services, string filename, byte[] bytes)
            : base(services, filename, bytes)
        {
            this.filename = filename;
            var rdr = new StreamReader(new MemoryStream(bytes));
            this.lexer = new Lexer(rdr);
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            return Load(platform, DefaultModuleName(filename), dstLib);
        }

        public TypeLibrary Load(IPlatform platform, string module, TypeLibrary dstLib)
        {
            this.platform = platform;
            this.tlLoader = new TypeLibraryDeserializer(platform, true, dstLib);
            this.moduleName = module;
            tlLoader.SetModuleName(module);
            for (;;)
            {
                var tok = Peek();
                if (tok.Type == TokenType.EOF)
                    break;
                if (PeekAndDiscard(TokenType.NL))
                    continue;
                ParseLine();
            }
            return dstLib;
        }

        public void ParseLine()
        {
            int? ordinal = ParseOrdinal();

            string callconv = ParseCallingConvention();

            var options = ParseOptions();

            var tok = Get();
            string fnName = tok.Value;
            var ssig = new SerializedSignature
            {
                Convention = callconv,
            };
            ssig.Arguments = ParseParameters(ssig);
            
            var deser = new X86ProcedureSerializer((IntelArchitecture)platform.Architecture, tlLoader, callconv);
            var sig = deser.Deserialize(ssig, new Frame(platform.FramePointerType));
            var svc = new SystemService
            {
                ModuleName = moduleName.ToUpper(),
                Name = fnName,
                Signature = sig
            };
            if (ordinal.HasValue)
            {
                tlLoader.LoadService(ordinal.Value, svc);
            }
            else
            {
                tlLoader.LoadService(fnName, svc);
            }

            for (;;)
            {
                // Discared entire line.
                var type = Get().Type;
                if (type == TokenType.EOF || type == TokenType.NL)
                    return;
            }
        }

        private Argument_v1[] ParseParameters(SerializedSignature ssig)
        {
            var args = new List<Argument_v1>();
            if (PeekAndDiscard(TokenType.LPAREN))
            {
                while (LoadParameter(ssig, args))
                    ;
                Expect(TokenType.RPAREN);
            }
            return args.ToArray();
        }

        private Dictionary<string,string> ParseOptions()
        {
            var options = new Dictionary<string,string>();
            while (PeekAndDiscard(TokenType.MINUS))
            {
                var key = Expect(TokenType.ID).Value;
                var value = "";
                if (PeekAndDiscard(TokenType.EQ))
                {
                    value = Get().Value;
                }
                options[key] = value;
            }
            return options;
        }

        private int? ParseOrdinal()
        {
            var tok = Peek();
            int? ordinal = null;
            if (tok.Type == TokenType.NUMBER)
            {
                tok = Get();
                ordinal = Convert.ToInt32(tok.Value);
            }
            else if (tok.Type == TokenType.AT)
            {
                Get();
            }
            return ordinal; 
        }

        private string ParseCallingConvention()
        {
            return Expect(TokenType.ID).Value;
        }

        private bool LoadParameter(SerializedSignature ssig, List<Argument_v1> args)
        {
            if (PeekAndDiscard(TokenType.NUMBER))
                return true;
            Token tok = Peek();
            if (tok.Type != TokenType.ID)
                return false;
            Get();
            SerializedType type = null;
            switch (tok.Value)
            {
            case "word":
            case "s_word":
                type = new PrimitiveType_v1(PrimitiveType.Word16.Domain, 2);
                break;
            case "long":
                type = new PrimitiveType_v1(PrimitiveType.Word32.Domain, 4);
                break;
            //$TODO: need SegmentedPointerType 
            case "segptr":
            case "segstr":
                type = new PrimitiveType_v1(Domain.SegPointer, 4);
                break;
            case "ptr":
                type = new PrimitiveType_v1(Domain.Pointer, 4);
                break;
            case "str":
                type = PointerType_v1.Create(PrimitiveType_v1.Char8(), 4);
                break;
            case "wstr":
                type = PointerType_v1.Create(PrimitiveType_v1.WChar16(), 4);
                break;
            default: throw new Exception("Unknown: " + tok.Value);
            }
            args.Add(new Argument_v1 { Type = type });
            return true;
        }

        private string DefaultModuleName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename).ToUpper() + ".DLL";
        }

        private Token Peek()
        {
            if (bufferedToken == null)
                bufferedToken = lexer.GetToken();
            return bufferedToken;
        }

        private bool PeekAndDiscard(TokenType type)
        {
            if (Peek().Type != type)
                return false;
            return Get().Type == type;
        }

        private Token Get()
        {
            Token t = bufferedToken;
            if (t == null)
                t = lexer.GetToken();
            bufferedToken = null;
            return t;
        }

        private Token Expect(TokenType type)
        {
            var tok = Get();
            if (tok.Type != type)
                throw new Exception("Expected " + type);
            return tok;
        }

        private class Lexer
        {
            private int lineNumber;
            private TextReader rdr;

            enum State
            {
                Initial,
                Number,
                Id,
                Comment
            }

            public Lexer(TextReader reader)
            {
                this.rdr = reader;
                this.lineNumber = 1;
            }

            public Token GetToken()
            {
                var st = State.Initial;
                StringBuilder sb = null;
                for (;;)
                {
                    int c = rdr.Peek();
                    char ch = (char)c;
                    switch (st)
                    {
                    case State.Initial:
                        switch (c)
                        {
                        case -1: return new Token(TokenType.EOF, lineNumber);
                        case ' ': rdr.Read(); break;
                        case '\t': rdr.Read(); break;
                        case '\r': rdr.Read(); break;
                        case '\n': rdr.Read(); ++lineNumber; return Tok(TokenType.NL);
                        case '#': rdr.Read(); st = State.Comment; break;
                        case '@': rdr.Read(); return Tok(TokenType.AT);
                        case '-': rdr.Read(); return Tok(TokenType.MINUS);
                        case '(': rdr.Read(); return Tok(TokenType.LPAREN);
                        case ')': rdr.Read(); return Tok(TokenType.RPAREN);
                        case '=': rdr.Read(); return Tok(TokenType.EQ);
                        default:
                            sb = new StringBuilder();
                            rdr.Read();
                            sb.Append(ch);
                            if (Char.IsDigit(ch))
                                st = State.Number;
                            else
                                st = State.Id;
                            break;
                        }
                        break;
                    case State.Comment:
                        if (c == -1)
                            return Tok(TokenType.EOF);
                        rdr.Read();
                        if (ch == '\n')
                        {
                            return Tok(TokenType.NL);
                        }
                        break;
                    case State.Number:
                        if (c == -1 || !Char.IsDigit(ch))
                            return Tok(TokenType.NUMBER, sb);
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    case State.Id:
                        if (Char.IsLetter(ch) || ch == '_' ||
                            Char.IsDigit(ch))
                        {
                            rdr.Read();
                            sb.Append(ch);
                            break;
                        }
                        return Tok(TokenType.ID, sb);
                    }
                }
            }

            private Token Tok(TokenType t)
            {
                return new Token(t, lineNumber);
            }

            private Token Tok(TokenType t, StringBuilder sb)
            {
                return new Token(t, sb.ToString(), lineNumber);
            }
        }

        public class Token
        {
            internal TokenType Type;
            internal string Value;
            internal int lineNumber;

            public Token(TokenType type, int lineNumber)
            {
                this.Type = type;
                this.lineNumber = lineNumber;
            }

            public Token(TokenType type, string value, int lineNumber)
            {
                this.Type = type;
                this.Value = value;
                this.lineNumber = lineNumber;
            }
        }

        public enum TokenType
        {
            EOF,
            NL,
            NUMBER,
            ID,
            MINUS,
            LPAREN,
            RPAREN,
            AT,
            EQ,
        }
    }
}
