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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public partial class LLVMParser
    {
        private CommentFilter lex;
        private Token? peekTok;

        public LLVMParser(TextReader rdr)
        {
            this.lex = new CommentFilter(new LLVMLexer(rdr));
        }

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
            var targets = new List<TargetSpecification>();
            var entries = new List<ModuleEntry>();
            while (true)
            {
                switch (Peek().Type)
                {
                case TokenType.EOF:
                    return new Module(targets, entries);
                case TokenType.LocalId:
                    var ldef = ParseLocalDefinition();
                    entries.Add(ldef);
                    break;
                case TokenType.GlobalId:
                    var gdef = ParseGlobalDefinition();
                    entries.Add(gdef!);
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
                    var target = ParseTarget();
                    targets.Add(target);
                    break;
                case TokenType.attributes:
                    ParseAttributes();
                    break;
                case TokenType.BANG:
                    ParseMetadata();
                    break;
                default:
                    Unexpected(peekTok!);
                    break;
                }
            }
        }

        private TypeDefinition ParseLocalDefinition()
        {
            var name = ParseLocalId();
            Expect(TokenType.EQ);
            Expect(TokenType.type);
            LLVMType? type;
            bool opaque;
            if (PeekAndDiscard(TokenType.opaque))
            {
                opaque = true;
                type = null;
            }
            else
            {
                opaque = false;
                type = ParseType();
            }
            return new TypeDefinition { Name = name, Type = type, Opaque = opaque };
        }

        private GlobalDefinition? ParseGlobalDefinition()
        {
            var name = Expect(TokenType.GlobalId);
            Expect(TokenType.EQ);
            bool unnamed_addr = false;
            string? visibility = null;
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
            if (PeekAndDiscard(TokenType.common) || PeekAndDiscard(TokenType.@internal))
            {
                // Not used yet.
            }
            if (PeekAndDiscard(TokenType.global))
            {
                var type = ParseType();
                Value? value = null;
                if (Value_FIRST.Contains(Peek().Type))
                {
                    value = ParseValue();
                }
                if (PeekAndDiscard(TokenType.COMMA))
                {
                    Expect(TokenType.align);
                    align = int.Parse(Expect(TokenType.Integer));
                };
                return new GlobalDefinition
                {
                    Name = name,
                    Type = type,
                    Initializer = value,
                    unnamed_addr = unnamed_addr,
                    Visibility = visibility,
                    Alignment = align,
                };
            }
            else if (PeekAndDiscard(TokenType.constant))
            {
                var type = ParseType();
                var value = ParseValue();
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
            Unexpected(Peek());
            return null;
        }

        private ModuleEntry ParseDeclaration()
        {
            Expect(TokenType.declare);
            var attrs = ParseParameterAttributes();
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
                    ret_attrs = attrs!,
                    Parameters = args
                }
            };
        }

        public FunctionDefinition ParseFunctionDefinition()
        {
            Expect(TokenType.define);
            if (PeekAndDiscard(TokenType.@internal))
            {
            }
            if (PeekAndDiscard(TokenType.dereferenceable))
            {
                //$REVIEW: no use for this.
                Expect(TokenType.LPAREN);
                Expect(TokenType.Integer);
                Expect(TokenType.RPAREN);
            }
            var parAttrs = ParseParameterAttributes();
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
                ret_attrs = parAttrs,
                Parameters = args,
                Instructions = instrs,
            };
        }

        private List<LLVMParameter> ParseParameterList()
        {
            var @params = new List<LLVMParameter>();
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

        private List<Argument> ParseArgumentList()
        {
            var args = new List<Argument>();
            Expect(TokenType.LPAREN);
            if (Peek().Type != TokenType.RPAREN)
            {
                var arg = ParseArgument();
                args.Add(arg);
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    arg = ParseArgument();
                    args.Add(arg);
                }
            }
            Expect(TokenType.RPAREN);
            return args;
        }

        private Argument ParseArgument()
        {
            LLVMType type = ParseType();
            Value? val = null;
            var attrs = ParseParameterAttributes();
            if (Value_FIRST.Contains(Peek().Type))
            {
                val = ParseValue();
            }
            var arg = new Argument
            {
                Type = type,
                Value = val,
                Attributes = attrs
            };
            return arg;
        }

        private LLVMParameter ParseParameter()
        {
            if (PeekAndDiscard(TokenType.ELLIPSIS))
            {
                return new LLVMParameter { name = "..." };
            }
            var type = ParseType();
            var attrs = ParseParameterAttributes();
            return new LLVMParameter {
                Type = type,
                attrs = attrs,
            };
        }

        private ParameterAttributes? ParseParameterAttributes()
        {
            bool zeroext = false;
            bool signext = false;
            bool attrs = false;
            bool noalias = false;
            while (true)
            {
                switch (Peek().Type)
                {
                case TokenType.signext: Get(); attrs = true; signext = true; break;
                case TokenType.zeroext: Get(); attrs = true; zeroext = true; break;
                case TokenType.noalias: Get(); attrs = true; noalias = true; break;
                case TokenType.nocapture: Get(); break;
                case TokenType.@readonly: Get(); break;
                case TokenType.writeonly: Get(); break;
                case TokenType.dereferenceable:
                    Get();
                    Expect(TokenType.LPAREN);
                    Expect(TokenType.Integer);
                    Expect(TokenType.RPAREN);
                    break;
                default:
                    if (!attrs)
                        return null;
                    return new ParameterAttributes
                    {
                        signext = signext,
                        zeroext = zeroext,
                        noalias = noalias
                    };
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

        private AggregateValue ParseAggregateValue()
        {
            var values = new List<TypedValue>();
            Expect(TokenType.LBRACE);
            if (Peek().Type != TokenType.RBRACE)
            {
                var tv = ParseTypedValue();
                values.Add(tv);
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    tv = ParseTypedValue();
                    values.Add(tv);
                }
            }
            Expect(TokenType.RBRACE);
            return new AggregateValue { Values = values.ToArray() };
        }

        private ArrayValue ParseArrayValue()
        {
            var values = new List<(LLVMType,Value?)>();
            Expect(TokenType.LBRACKET);
            if (Peek().Type != TokenType.RBRACKET)
            {
                var type = ParseType();
                var value = ParseValue();
                values.Add((type, value));
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    type = ParseType();
                    value = ParseValue();
                    values.Add((type, value));
                }
            }
            Expect(TokenType.RBRACKET);
            return new ArrayValue { Values = values.ToArray() };
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

        private TargetSpecification ParseTarget()
        {
            Expect(TokenType.target);
            var tok = Get();
            Expect(TokenType.EQ);
            var spec = Expect(TokenType.String);
            return new TargetSpecification
            {
                Type = tok.Type,
                Specification = spec,
            };
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
            case TokenType.fence: return ParseFence();
            case TokenType.ret: return ParseRet();
            case TokenType.store: return ParseStore();
            case TokenType.@switch: return ParseSwitch();
            case TokenType.unreachable: return ParseUnreachable();
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
            case TokenType.add: return ParseBinOp(result);
            case TokenType.and: return ParseBinBitOp(result);
            case TokenType.ashr: return ParseBinBitOp(result);
            case TokenType.bitcast: return ParseConversion(result);
            case TokenType.call: return ParseCall(result);
            case TokenType.extractvalue: return ParseExtractvalue(result);
            case TokenType.fadd: return ParseBinOp(result);
            case TokenType.fcmp: return ParseFcmp(result);
            case TokenType.fdiv: return ParseBinOp(result);
            case TokenType.fmul: return ParseBinOp(result);
            case TokenType.fsub: return ParseBinOp(result);
            case TokenType.fpext: return ParseConversion(result);
            case TokenType.fptosi: return ParseConversion(result);
            case TokenType.fptoui: return ParseConversion(result);
            case TokenType.fptrunc: return ParseConversion(result);
            case TokenType.getelementptr: return ParseGetElementPtr(result);
            case TokenType.icmp: return ParseIcmp(result);
            case TokenType.inttoptr: return ParseConversion(result);
            case TokenType.load: return ParseLoad(result);
            case TokenType.lshr: return ParseBinBitOp(result);
            case TokenType.mul: return ParseBinOp(result);
            case TokenType.or: return ParseBinBitOp(result);
            case TokenType.phi: return ParsePhi(result);
            case TokenType.ptrtoint: return ParseConversion(result);
            case TokenType.sdiv: return ParseBinOp(result);
            case TokenType.sext: return ParseConversion(result);
            case TokenType.shl: return ParseBinBitOp(result);
            case TokenType.select: return ParseSelect(result);
            case TokenType.sitofp: return ParseConversion(result);
            case TokenType.srem: return ParseBinOp(result);
            case TokenType.sub: return ParseBinOp(result);
            case TokenType.trunc: return ParseConversion(result);
            case TokenType.udiv: return ParseBinOp(result);
            case TokenType.urem: return ParseBinOp(result);
            case TokenType.xor: return ParseBinBitOp(result);
            case TokenType.zext: return ParseConversion(result);
            default: Unexpected(tok); return null!;
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

        private TypedValue ParseTypedValue()
        {
            var type = ParseType();
            var val = ParseValue();
            return new TypedValue { Type = type, Value = val };
        }

        private static HashSet<TokenType> Value_FIRST = new HashSet<TokenType>
        {
            TokenType.LBRACE,
            TokenType.LBRACKET,
            TokenType.CharArray,
            TokenType.DoubleLiteral,
            TokenType.LocalId,
            TokenType.GlobalId,
            TokenType.HexInteger,
            TokenType.Integer,
            TokenType.bitcast,
            TokenType.@false,
            TokenType.getelementptr,
            TokenType.inttoptr,
            TokenType.@null,
            TokenType.@true,
            TokenType.zeroinitializer,
        };

        private Value? ParseValue()
        {
            switch (Peek().Type)
            {
            case TokenType.LBRACE: return ParseAggregateValue();
            case TokenType.LBRACKET: return ParseArrayValue();
            case TokenType.CharArray: return ParseCharArray();
            case TokenType.DoubleLiteral: return ParseLiteral();
            case TokenType.LocalId: return ParseLocalId();
            case TokenType.GlobalId: return ParseGlobalId();
            case TokenType.HexInteger: return ParseLiteral();
            case TokenType.Integer: return ParseInteger();
            case TokenType.X86_fp80_Literal: return ParseLiteral();
            case TokenType.@false: Expect(TokenType.@false); return new Constant(false);
            case TokenType.bitcast: return ParseConversionExpr();
            case TokenType.getelementptr: return ParseGetElementPtrExpr();
            case TokenType.inttoptr: return ParseConversionExpr();
            case TokenType.@null: Expect(TokenType.@null); return new Constant(null);
            case TokenType.@true: Expect(TokenType.@true); return new Constant(true);
            case TokenType.zeroinitializer: Expect(TokenType.zeroinitializer); return new Constant(TokenType.zeroinitializer);
            }
            Unexpected(Peek());
            return null;
        }

        private Literal ParseLiteral()
        {
            var literal = Get();
            return new Literal { Type = literal.Type, Value = literal.Value };
        }

        private Value ParseCharArray()
        {
            var cha = Get().Value!;
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
            return new Constant(long.Parse(n));
        }

        public LLVMType ParseType()
        {
            LLVMType? type = ParseTypeCore();
            for (;;)
            {
                if (PeekAndDiscard(TokenType.STAR))
                {
                    type = new LLVMPointer(type!);
                }
                else if (PeekAndDiscard(TokenType.LPAREN))
                {
                    var args = new List<LLVMParameter>();
                    if (Peek().Type != TokenType.RPAREN)
                    {
                        var argType = ParseType();
                        args.Add(new LLVMParameter { Type = argType });
                        while (PeekAndDiscard(TokenType.COMMA))
                        {
                            if (PeekAndDiscard(TokenType.ELLIPSIS))
                            {
                                args.Add(new LLVMParameter { name = "..." });
                                break;
                            }
                            argType = ParseType();
                            args.Add(new LLVMParameter { Type = argType });
                        }
                    }
                    Expect(TokenType.RPAREN);
                    var fnType = new LLVMFunctionType
                    {
                        ReturnType = type!,
                        Parameters = args,
                    };
                    type = fnType;
                }
                else
                    return type!;
            }
        }

        private static HashSet<TokenType> Type_FIRST = new HashSet<TokenType>
        {
            TokenType.@void,
            TokenType.IntType,
            TokenType.@double,
            TokenType.LocalId,
            TokenType.LBRACE,
            TokenType.LBRACKET,
        };

        private LLVMType? ParseTypeCore()
        {
            var tok = Peek();
            switch (tok.Type)
            {
            case TokenType.@void:
                Expect(TokenType.@void);
                return LLVMType.Void;
            case TokenType.IntType:
                return LLVMType.GetBaseType(Get().Value!);
            case TokenType.@double:
                Expect(TokenType.@double);
                return LLVMType.Double;
            case TokenType.x86_fp80:
                Expect(TokenType.x86_fp80);
                return LLVMType.X86_fp80;
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
            if (Peek().Type != TokenType.RBRACE)
            {
                fields.Add(ParseType());
                while (PeekAndDiscard(TokenType.COMMA))
                {
                    var field = ParseType();
                    fields.Add(field);
                }
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
            return tok.Value!;
        }

        private Token Peek()
        {
            if (this.peekTok is null)
            {
                this.peekTok = lex.GetToken();
            }
            return this.peekTok;
        }

        private Token Get()
        {
            Token t;
            if (this.peekTok is not null)
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
