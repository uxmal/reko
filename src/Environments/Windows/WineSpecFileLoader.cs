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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Loads metadata info encoded as a WINE spec file
    /// (see https://www.winehq.org/docs/winelib-guide/spec-file for details)
    /// </summary>
    public class WineSpecFileLoader : MetadataLoader
    {
        private Lexer lexer;
        private Token? bufferedToken;
        private TypeLibraryDeserializer tlLoader;
        private IPlatform platform;
        private string moduleName;

        public WineSpecFileLoader(IServiceProvider services, ImageLocation imageUri, byte[] bytes)
            : base(services, imageUri, bytes)
        {
            var rdr = new StreamReader(new MemoryStream(bytes));
            this.lexer = new Lexer(rdr);
            this.tlLoader = null!;
            this.platform = null!;
            this.moduleName = null!;
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            return Load(platform, DefaultModuleName(base.Location), dstLib);
        }

        public override TypeLibrary Load(IPlatform platform, string? module, TypeLibrary dstLib)
        {
            this.platform = platform;
            module = module ?? DefaultModuleName(base.Location);
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
                var (ordinal, svc) = ParseLine();
                if (svc is not null)
                {
                    if (ordinal.HasValue)
                    {
                        tlLoader.LoadService(ordinal.Value, svc);
                    }
                    else
                    {
                        tlLoader.LoadService(svc.Name!, svc);
                    }
                }
            }
            return dstLib;
        }

        public (int?, SystemService?) ParseLine()
        {
            try
            {
                int? ordinal = ParseOrdinal();

                string callconv = ParseCallingConvention();

                var options = ParseOptions();

                var tok = Get();
                string fnName = tok.Value!;
                var ssig = new SerializedSignature
                {
                    Convention = callconv,
                };
                ssig.Arguments = ParseParameters(ssig);
                SkipToEndOfLine();

                var deser = new ProcedureSerializer(platform, tlLoader, ssig.Convention);
                var sig = deser.Deserialize(ssig, new Frame(platform.Architecture, platform.FramePointerType));
                var svc = new SystemService
                {
                    ModuleName = moduleName.ToUpper(),
                    Name = fnName,
                    Signature = sig
                };
                return (ordinal, svc);
            }
            catch
            {
                Services.RequireService<IEventListener>().Warn(
                    new NullCodeLocation(moduleName),
                    "Line {0} in the Wine spec file could not be read; skipping.",
                    lexer.lineNumber);
                SkipToEndOfLine();
                return (null, null);
            }
        }

        private void SkipToEndOfLine()
        {
            for (;;)
            {
                // Discard rest of line.
                var type = Get().Type;
                if (type == TokenType.EOF || type == TokenType.NL)
                    break;
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
            if (ssig.Convention == "varargs")
            {
                args.Add(new Argument_v1 { Name = "...", Type = new VoidType_v1() });
                ssig.Convention = "__cdecl";
            }
            return args.ToArray();
        }

        private Dictionary<string,string> ParseOptions()
        {
            var options = new Dictionary<string,string>();
            while (PeekAndDiscard(TokenType.MINUS))
            {
                var key = Expect(TokenType.ID).Value!;
                var value = "";
                if (PeekAndDiscard(TokenType.EQ))
                {
                    value = Get().Value!;
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
            return Expect(TokenType.ID).Value!;
        }

        private bool LoadParameter(SerializedSignature ssig, List<Argument_v1> args)
        {
            if (PeekAndDiscard(TokenType.NUMBER))
                return true;
            Token tok = Peek();
            if (tok.Type != TokenType.ID)
                return false;
            Get();
            SerializedType? type = null;
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
			case "uint16":
				type = PrimitiveType_v1.UInt16();
				break;
			case "uint32":
				type = PrimitiveType_v1.UInt32();
				break;
			case "uint64":
				type = PrimitiveType_v1.UInt64();
				break;
			case "int16":
				type = PrimitiveType_v1.Int16();
				break;
			case "int32":
				type = PrimitiveType_v1.Int32();
				break;
			case "int64":
				type = PrimitiveType_v1.Int64();
				break;
            default: throw new Exception("Unknown: " + tok.Value);
            }
            args.Add(new Argument_v1 { Type = type });
            return true;
        }

        private string DefaultModuleName(ImageLocation fileUri)
        {
            var filename = fileUri.GetFilename();
			string libName = Path.GetFileNameWithoutExtension(filename).ToUpper();

			if (Path.GetExtension(libName).Length > 0) {
				return libName;
			} else {
				return libName + ".DLL";
			}
        }

        private Token Peek()
        {
            if (bufferedToken is null)
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
            Token? t = bufferedToken;
            if (t is null)
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
            internal int lineNumber;
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
                StringBuilder sb = new StringBuilder();
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
            internal string? Value;
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
