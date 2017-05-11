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
                case TokenType.LocalId:
                    var ldef = ParseLocalDefinition();
                    entries.Add(ldef);
                    break;
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
                case TokenType.source_filename:
                    ParseSourceFilename();
                    break;
                case TokenType.target:
                    ParseTarget();
                    break;
                case TokenType.attributes:
                    ParseAttributes();
                    break;
                case TokenType.BANG:
                    ParseMetadata();
                    break;
                default:
                    Unexpected(peekTok);
                    break;
                }
            }
        }

        private TypeDefinition ParseLocalDefinition()
        {
            var name = ParseLocalId();
            Expect(TokenType.EQ);
            Expect(TokenType.type);
            var type = ParseType();
            return new TypeDefinition { Name = name, Type = type };
        }

        private GlobalDefinition ParseGlobalDefinition()
        {
            var name = Expect(TokenType.GlobalId);
            Expect(TokenType.EQ);
            bool unnamed_addr = false;
            string visibility = null;
            int align = 0;      // 0 = no aligmnent specified.
            if (PeekAndDiscard(TokenType.@private))
            {
                visibility = "private";
            } else if (PeekAndDiscard(TokenType.external))
            {
                visibility = "external";
            }
            if (PeekAndDiscard(TokenType.unnamed_addr))
            {
                unnamed_addr = true;
            }
            if (PeekAndDiscard(TokenType.global))
            {
                var type = ParseType();
                if (PeekAndDiscard(TokenType.COMMA))
                {
                    Expect(TokenType.align);
                    align = int.Parse(Expect(TokenType.Integer));
                };
                return new GlobalDefinition
                {
                    Name = name,
                    Type = type,
                    unnamed_addr = unnamed_addr,
                    Visibility = visibility,
                    Alignment = align,
                };
            }
            if (PeekAndDiscard(TokenType.constant))
            {
                var type = ParseType();
                var value = ParseConstant();
                if (PeekAndDiscard(TokenType.COMMA))
                {
                    Expect(TokenType.align);
                    align = int.Parse(Expect(TokenType.Integer));
                }
                return new GlobalDefinition
                {
                    Name = name,
                    Type = type,
                    constant = true,
                    unnamed_addr = unnamed_addr,
                    Visibility = visibility,
                    Initializer = value,
                    Alignment = align,
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

        public FunctionDefinition ParseFunctionDefinition()
        {
            Expect(TokenType.define);
            if (PeekAndDiscard(TokenType.dereferenceable))
            {
                //$REVIEW: no use for this.
                Expect(TokenType.LPAREN);
                Expect(TokenType.Integer);
                Expect(TokenType.RPAREN);
            }
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
                Value val = null; 
                if (Value_FIRST.Contains(Peek().Type))
                {
                    val = ParseValue();
                }
                args.Add(Tuple.Create(type, val));
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    type = ParseType();
                    if (Value_FIRST.Contains(Peek().Type))
                    {
                        val = ParseValue();
                    }
                    args.Add(Tuple.Create(type, val));
                }
            }
            Expect(TokenType.RPAREN);
            return args;
        }

        private LLVMArgument ParseParameter()
        {
            if (PeekAndDiscard(TokenType.ELLIPSIS))
            {
                return new LLVMArgument { name = "..." };
            }
            var type = ParseType();
            for (;;)
            {
                if (PeekAndDiscard(TokenType.nocapture))
                {
                    //$REVIEW: don't care
                } else if (PeekAndDiscard(TokenType.dereferenceable))
                {
                    Expect(TokenType.LPAREN);
                    Expect(TokenType.Integer);
                    Expect(TokenType.RPAREN);
                }
                else
                {
                    return new LLVMArgument { Type = type };
                }
            }
        }

        private void ParseFunctionAttributes()
        {
            //$REVIEW: currently we just discard these since they
            // seem mostly apply to transformations from IR => MC,
            // while Reko is all about MC => IR => source code.
            for (;;)
            {
                switch (Peek().Type)
                {
                case TokenType.nounwind:
                    Expect(TokenType.nounwind);
                    break;
                case TokenType.HASH:
                    Expect(TokenType.HASH);
                    Expect(TokenType.Integer);
                    break;
                default:
                    return;
                }
            }
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

        private void ParseSourceFilename()
        {
            Expect(TokenType.source_filename);
            Expect(TokenType.EQ);
            Expect(TokenType.String);
        }

        private void ParseTarget()
        {
            Expect(TokenType.target);
            PeekAndDiscard(TokenType.datalayout);
            PeekAndDiscard(TokenType.triple);
            Expect(TokenType.EQ);
            Expect(TokenType.String);
        }

        private void ParseAttributes()
        {
            //$REVIEW: we don't care about these attributes do we?
            Expect(TokenType.attributes);
            Expect(TokenType.HASH);
            Expect(TokenType.Integer);
            Expect(TokenType.EQ);
            Expect(TokenType.LBRACE);
            while (!PeekAndDiscard(TokenType.RBRACE))
            {
                Get();      // just throw everything away.
            }
        }

        // !0 = !{ ... }
        // !foo.bar = !{ ... }
        private void ParseMetadata()
        {
            Expect(TokenType.BANG);
            if (!PeekAndDiscard(TokenType.Integer))
            {
                if (PeekAndDiscard(TokenType.String))
                {
                    while (PeekAndDiscard(TokenType.DOT) &&
                           PeekAndDiscard(TokenType.String))
                    {
                    }
                }
            }
            Expect(TokenType.EQ);
            Expect(TokenType.BANG);
            Expect(TokenType.LBRACE);
            while (!PeekAndDiscard(TokenType.RBRACE))
            {
                Get();
            }
        }

        public Instruction ParseInstruction()
        {
            var tok = Peek();
            if (tok.LineNumber == 146)
                tok.ToString();
            switch(tok.Type)
            {
            case TokenType.br: return ParseBr();
            case TokenType.call: return ParseCall(null);
            case TokenType.ret: return ParseRet();
            case TokenType.store: return ParseStore();
            case TokenType.@switch: return ParseSwitch();
            case TokenType.LocalId: return ParseAssignment();
            default: Unexpected(tok); break;
            }
            throw new NotImplementedException();
        }

        private Instruction ParseAssignment()
        {
            var result = ParseLocalId();
            Expect(TokenType.EQ);

            var tok = Peek();
            switch (tok.Type)
            {
            case TokenType.alloca: return ParseAlloca(result);
            case TokenType.add: return ParseBinBitOp(result);
            case TokenType.and: return ParseBinBitOp(result);
            case TokenType.bitcast: return ParseBitcast(result);
            case TokenType.call: return ParseCall(result);
            case TokenType.extractvalue: return ParseExtractvalue(result);
            case TokenType.getelementptr: return ParseGetElementPtr(result);
            case TokenType.icmp: return ParseIcmp(result);
            case TokenType.inttoptr: return ParseInttoptr(result);
            case TokenType.load: return ParseLoad(result);
            case TokenType.phi: return ParsePhi(result);
            case TokenType.sub: return ParseBinOp(result);
            default: Unexpected(tok); return null;
            }
        }

        private Instruction ParseBr()
        {
            Expect(TokenType.br);
            if (!PeekAndDiscard(TokenType.label))
            {
                var type = ParseType();
                var cond = ParseValue();
                Expect(TokenType.COMMA);
                Expect(TokenType.label);
                var destTrue = ParseLocalId();
                Expect(TokenType.COMMA);
                Expect(TokenType.label);
                var destFalse = ParseLocalId();
                return new BrInstr
                {
                    Type = type,
                    Cond = cond,
                    IfTrue = destTrue,
                    IfFalse = destFalse,
                };
            }
            else 
            {
                var dest = ParseLocalId();
                return new BrInstr
                {
                    IfTrue = dest,
                };
            }
            throw new NotImplementedException();
        }

        private static HashSet<TokenType> Value_FIRST = new HashSet<TokenType>
        {
            TokenType.LocalId,
            TokenType.GlobalId,
            TokenType.Integer,
            TokenType.getelementptr
        };

        private Value ParseValue()
        {
            switch (Peek().Type)
            {
            case TokenType.LocalId: return ParseLocalId();
            case TokenType.GlobalId: return ParseGlobalId();
            case TokenType.Integer: return ParseInteger();
            case TokenType.@false: Expect(TokenType.@false); return new Constant(false);
            case TokenType.getelementptr: return ParseGetElementPtrExpr();
            case TokenType.@null: Expect(TokenType.@null); return new Constant(null);
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

        public LLVMType ParseType()
        {
            LLVMType type = ParseTypeCore();
            for (;;)
            {
                if (PeekAndDiscard(TokenType.STAR))
                {
                    type = new LLVMPointer(type);
                }
                else if (PeekAndDiscard(TokenType.LPAREN))
                {
                    var args = new List<LLVMArgument>();
                    if (Peek().Type != TokenType.RPAREN)
                    {
                        var argType = ParseType();
                        args.Add(new LLVMArgument { Type = argType });
                        while (PeekAndDiscard(TokenType.COMMA))
                        {
                            if (PeekAndDiscard(TokenType.ELLIPSIS))
                            {
                                args.Add(new LLVMArgument { name = "..." });
                                break;
                            }
                            argType = ParseType();
                            args.Add(new LLVMArgument { Type = argType });
                        }
                    }
                    Expect(TokenType.RPAREN);
                    var fnType = new LLVMFunctionType
                    {
                        ReturnType = type,
                        Arguments = args,
                    };
                    type = fnType;
                }
                else
                    return type;
            }
        }

        private static HashSet<TokenType> Type_FIRST = new HashSet<TokenType>
        {
            TokenType.IntType,
            TokenType.LocalId,
            TokenType.LBRACE,
            TokenType.LBRACKET,
        };

        private LLVMType ParseTypeCore()
        {
            var tok = Peek();
            switch (tok.Type)
            {
            case TokenType.@void:
                Expect(TokenType.@void);
                return LLVMType.Void;
            case TokenType.IntType:
                return LLVMType.GetBaseType(Get().Value);
            case TokenType.LocalId:
                var typeName = ParseLocalId();
                return new TypeReference(typeName);
            case TokenType.LBRACE:
                return ParseStruct();
            case TokenType.LBRACKET:
                return ParseArray();
            default:
                Unexpected(tok);
                return null;
            }
        }

        private LLVMType ParseStruct()
        {
            Expect(TokenType.LBRACE);
            var fields = new List<LLVMType>();
            fields.Add(ParseType());
            while (PeekAndDiscard(TokenType.COMMA))
            {
                var field = ParseType();
                fields.Add(field);
            }
            Expect(TokenType.RBRACE);
            return new StructureType
            {
                Fields = fields
            };
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
                string.Format("Unexpected token '{0}' {1} on line {2}.",
                tok.Type, tok.Value, tok.LineNumber));
        }
    }
}
