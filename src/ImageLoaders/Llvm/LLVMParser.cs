#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public partial class LLVMParser
    {
        private CommentFilter lex;
        private Token peekTok;

        public LLVMParser(LLVMLexer lexer)
        {
            this.lex = new CommentFilter(lexer);
        }

        private class CommentFilter
        {
            private LLVMLexer lexer;

            public CommentFilter(LLVMLexer lexer)
            {
                this.lexer = lexer;
            }

            public Token GetToken()
            {
                while (true)
                {
                    var tok = lexer.GetToken();
                    if (tok.Type == TokenType.EOF ||
                        tok.Type != TokenType.Comment)
                        return tok;
                }
            }
        }
        public Module ParseModule()
        {
            var entries = new List<ModuleEntry>();
            while (true)
            {
                switch (Peek().Type)
                {
                case TokenType.EOF:
                    return new Module
                    {
                        Entries = entries,
                    };
                case TokenType.GlobalId:
                    var gdef = ParseGlobalDefinition();
                    entries.Add(gdef);
                    break;
                case TokenType.declare:
                    var decl = ParseDeclaration();
                    entries.Add(decl);
                    break;
                case TokenType.define:
                    var def = ParseFunctionDefinition();
                    entries.Add(def);
                    break;
                default:
                    Unexpected(peekTok);
                    break;
                }
            }
        }

        private GlobalDefinition ParseGlobalDefinition()
        {
            var name = Expect(TokenType.GlobalId);
            Expect(TokenType.EQ);
            bool unnamed_addr = false;
            string visibility = null;
            if (PeekAndDiscard(TokenType.@private))
            {
                visibility = "private";
            }
            if (PeekAndDiscard(TokenType.unnamed_addr))
            {
                unnamed_addr = true;
            }
            if (PeekAndDiscard(TokenType.global))
            {
                global = true;
                var type = ParseType();
                return new GlobalDefinition
                {
                    Name = name,
                    Type = type,
                    unnamed_addr = unnamed_addr,
                    Visibility = visibility,
                };
            }
            if (PeekAndDiscard(TokenType.constant))
            {
                var type = ParseType();
                var value = ParseConstant();
                return new GlobalDefinition
                {
                    Name = name,
                    Type = type,
                    constant = true,
                    unnamed_addr = unnamed_addr,
                    Visibility = visibility,
                    Initializer = value,
                };
            }
            throw new NotImplementedException();
        }

        private ModuleEntry ParseDeclaration()
        {
            Expect(TokenType.declare);
            var retType = ParseType();
            var name = Expect(TokenType.GlobalId);
            var args = ParseParameterList();
            ParseFunctionAttributes();
            return new Declaration
            {
                Name = name,
                Type = new LLVMFunctionType
                {
                    ReturnType = retType,
                    Arguments = args
                }
            };
        }

        private ModuleEntry ParseFunctionDefinition()
        {
            Expect(TokenType.define);
            var retType = ParseType();
            var name = Expect(TokenType.GlobalId);
            var args = ParseParameterList();
            ParseFunctionAttributes();
            Expect(TokenType.LBRACE);
            var instrs = new List<Instruction>();
            while (!PeekAndDiscard(TokenType.RBRACE))
            {
                var instr = ParseInstruction();
                instrs.Add(instr);
            }
            return new FunctionDefinition
            {
                FunctionName = name,
                ResultType = retType,
                Parameters = args,
                Instructions = instrs,
            };
        }

        private List<LLVMArgument> ParseParameterList()
        {
            var @params = new List<LLVMArgument>();
            Expect(TokenType.LPAREN);
            if (Peek().Type != TokenType.RPAREN)
            {
                var arg = ParseParameter();
                @params.Add(arg);
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    arg = ParseParameter();
                    @params.Add(arg);
                }
            }
            Expect(TokenType.RPAREN);
            return @params;
        }

        private List<Tuple<LLVMType, Value>> ParseArgumentList()
        {
            var args = new List<Tuple<LLVMType, Value>>();
            Expect(TokenType.LPAREN);
            if (Peek().Type != TokenType.RPAREN)
            {
                var type = ParseType();
                var val = ParseValue();
                args.Add(Tuple.Create(type, val));
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    type = ParseType();
                    val = ParseValue();
                    args.Add(Tuple.Create(type, val));
                }
            }
            Expect(TokenType.RPAREN);
            return args;
        }

        private LLVMArgument ParseParameter()
        {
            var type = ParseType();
            PeekAndDiscard(TokenType.nocapture); //$REVIEW: don't care
            return new LLVMArgument
            {
                Type = type,
            };
        }

        private void ParseFunctionAttributes()
        {
            PeekAndDiscard(TokenType.nounwind); //$REVIEW: don't care
        }

        private Value ParseConstant()
        {
            switch (Peek().Type)
            {
            case TokenType.CharArray:
                var cha = Get().Value;
                var bytes = new List<byte>();
                for (int i = 0; i < cha.Length; ++i)
                {
                    if (cha[i] == '\\')
                    {
                        byte hi = 0, lo = 0;
                        if (i + 2 < cha.Length &&
                            TryParseHexDigit(cha[i + 1], out hi) &&
                            TryParseHexDigit(cha[i + 2], out lo))
                        {
                            bytes.Add((byte)((hi << 4) | lo));
                            i += 2;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        bytes.Add((byte)cha[i]);
                    }
                }
                return new Constant(bytes.ToArray());
            }
            throw new NotImplementedException();
        }

        private bool TryParseHexDigit(char c, out byte b)
        {
            int i = "0123456789ABCDEF".IndexOf(Char.ToUpper(c));
            if (i < 0)
            {
                b = 0;
                return false;
            }
            b = (byte)i;
            return true;
        }

        public Instruction ParseInstruction()
        {
            var tok = Peek();
            switch(tok.Type)
            {
            case TokenType.br: return ParseBr();
            case TokenType.call: return ParseCall();
            case TokenType.ret: return ParseRet();
            case TokenType.LocalId: return ParseAssignment();
            default: Unexpected(tok); break;
            }
            throw new NotImplementedException();
        }

        private Instruction ParseAssignment()
        {
            var local = Expect(TokenType.LocalId);
            Expect(TokenType.EQ);

            switch (Peek().Type)
            {
            case TokenType.getelementptr: return ParseGetElementPtr(local);
            default: throw new NotImplementedException();
            }
        }

        private Instruction ParseBr()
        {
            Expect(TokenType.br);
            if (PeekAndDiscard(TokenType.label))
            {
                var dest = ParseLocalId();
                return new BrInstr
                {
                    IfTrue = dest,
                };
            }
            throw new NotImplementedException();
        }

        private Value ParseValue()
        {
            switch (Peek().Type)
            {
            case TokenType.LocalId: return ParseLocalId();
            case TokenType.GlobalId: return ParseGlobalId();
            case TokenType.Integer: return ParseInteger();
            }
            Unexpected(Peek());
            return null;
        }

        private LocalId ParseLocalId()
        {
            var id = Expect(TokenType.LocalId);
            return new LocalId(id);
        }

        private GlobalId ParseGlobalId()
        {
            var id = Expect(TokenType.GlobalId);
            return new GlobalId(id);
        }

        private Constant ParseInteger()
        {
            var n = Expect(TokenType.Integer);
            return new Constant(int.Parse(n));
        }

        private Instruction ParseRet()
        {
            Expect(TokenType.ret);
            if (PeekAndDiscard(TokenType.@void))
            {
                return new RetInstr
                {
                    Type = LLVMType.Void,
                };
            }
            var type = ParseType();
            var val = ParseValue();
            return new RetInstr
            {
                Type = type,
                Value = val,
            };
        }

        public LLVMType ParseType()
        {
            LLVMType type = null;
            for (;;)
            {
                switch (Peek().Type)
                {
                case TokenType.IntType:
                    type = LLVMType.GetBaseType(Get().Value);
                    break;
                case TokenType.LBRACKET:
                    type = ParseArray();
                    break;
                default:
                    return type;
                }
                if (PeekAndDiscard(TokenType.STAR))
                {
                    type = new LLVMPointer(type);
                }
            }
        }

        private LLVMArrayType ParseArray()
        {
            Expect(TokenType.LBRACKET);
            var size = Expect(TokenType.Integer);
            Expect(TokenType.x);
            var type = ParseType();
            Expect(TokenType.RBRACKET);
            return new LLVMArrayType(Int32.Parse(size), type);
        }

        private bool PeekAndDiscard(TokenType type)
        {
            if (Peek().Type != type)
                return false;
            Get();
            return true;
        }

        private string Expect(TokenType type)
        {
            var tok = Get();
            if (tok.Type != type)
                Unexpected(tok);
            return tok.Value;
        }

        private Token Peek()
        {
            if (this.peekTok == null)
            {
                this.peekTok = lex.GetToken();
            }
            return this.peekTok;
        }

        private Token Get()
        {
            Token t;
            if (this.peekTok != null)
            {
                t = this.peekTok;
                this.peekTok = null;
            }
            else
            {
                t = lex.GetToken();
            }
            return t;
        }

        private void Unexpected(Token tok)
        {
            throw new FormatException(
                string.Format("Unexpected token '{0}' {1}.",
                tok.Type, tok.Value));
        }
    }
}
