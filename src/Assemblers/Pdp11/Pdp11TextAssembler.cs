#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Assemblers.Pdp11
{
    public class Pdp11TextAssembler : Assembler
    {
        private Lexer lexer;
        private IEmitter emitter;
        private Pdp11Architecture arch;

        public Pdp11TextAssembler() : this(new Emitter())
        {
        }

        public Pdp11TextAssembler(IEmitter emitter)
        {
            this.emitter = emitter;
            this.ImageSymbols = new List<ImageSymbol>();
        }

        public Program Assemble(Address addrBase, TextReader reader)
        {
            arch = new Pdp11Architecture("pdp11");
            Assembler = new Pdp11Assembler(arch, addrBase, emitter);
            lexer = new Lexer(reader);

            // Assemblers are strongly line-oriented.

            while (lexer.Peek().Type != TokenType.EOF)
            {
                ProcessLine();
            }

            //asm.ReportUnresolvedSymbols();
            StartAddress = addrBase;
            return Assembler.GetImage();
        }

 
        public Program AssembleFragment(Address baseAddress, string fragment)
        {
            return Assemble(baseAddress, new StringReader(fragment));
        }

        private void ProcessLine()
        {
            if (lexer.PeekAndDiscard(TokenType.EOL))
                return;
            if (lexer.PeekAndDiscard(TokenType._Page))
                return;
            if (lexer.Peek().Type == TokenType.Id)
            {
                var id = (string) lexer.Expect(TokenType.Id);
                if (lexer.PeekAndDiscard(TokenType.Eq))
                {
                    var exp = ParseExpression();
                    Assembler.Equate(id, exp);
                    lexer.PeekAndDiscard(TokenType.EOL);
                    return;
                }
                else if (lexer.PeekAndDiscard(TokenType.Colon))
                {
                    Assembler.Label(id);
                }
                else
                {
                    lexer.Unexpected(lexer.Get());
                }
            }
            switch (lexer.Peek().Type)
            {
            case TokenType.EOL:
                lexer.Get();
                return;
            case TokenType._Word:
                lexer.Get();
                ProcessWords(PrimitiveType.Word16);
                break;
            case TokenType._Dot:
                lexer.Get();
                lexer.Expect(TokenType.Eq);
                lexer.Expect(TokenType._Dot);
                lexer.Expect(TokenType.Plus);
                var delta = (int) ParseExpression();
                emitter.Reserve(delta);
                break;
            case TokenType.ASR: ProcessSingleOperand(Assembler.Asr); break;
            case TokenType.CLR: ProcessSingleOperand(Assembler.Clr); break;
            case TokenType.CLRB: ProcessSingleOperand(Assembler.Clrb); break;
            case TokenType.BEQ: ProcessBranch(Assembler.Beq); break;
            case TokenType.DEC: ProcessSingleOperand(Assembler.Dec); break;
            case TokenType.INC: ProcessSingleOperand(Assembler.Inc); break;
            case TokenType.JSR: lexer.Get(); Assembler.Jsr(ParseOperands()); break;
            case TokenType.MOV: ProcessDoubleOperand(Assembler.Mov); break;
            case TokenType.MOVB: ProcessDoubleOperand(Assembler.Movb); break;
            case TokenType.RESET: lexer.Get(); Assembler.Reset(); break;
            case TokenType.SUB: ProcessDoubleOperand(Assembler.Sub);break;
            default:
                lexer.Unexpected(lexer.Get());
                break;
            }
        }

        private void ProcessBranch(Action<string> action)
        {
            lexer.Get();
            var dest = (string)lexer.Expect(TokenType.Id);
            lexer.PeekAndDiscard(TokenType.EOL);
            action(dest);
        }

        private void ProcessSingleOperand(Action<ParsedOperand> action)
        {
            lexer.Get();
            var ops = ParseOperands();
            if (ops.Length != 1)
                throw new ApplicationException();
            action(ops[0]);
        }

        private void ProcessDoubleOperand(Action<ParsedOperand, ParsedOperand> action)
        {
            lexer.Get();
            var ops = ParseOperands();
            if (ops.Length != 2)
                throw new ApplicationException();
            action(ops[0], ops[1]);
        }

        private ParsedOperand[] ParseOperands()
        {
            var ops = new List<ParsedOperand>();
            var tok = lexer.Peek();
            var opp = new OperandParser(arch, lexer, Assembler);
            if (tok.Type != TokenType.EOF && tok.Type != TokenType.EOL)
            {
                ops.Add(opp.ParseOperand());
                while (lexer.PeekAndDiscard(TokenType.Comma))
                {
                    ops.Add(opp.ParseOperand());
                }
            }
            lexer.PeekAndDiscard(TokenType.EOL);
            return ops.ToArray();
        }

        private void ProcessWords(PrimitiveType wordSize)
        {
            var token = lexer.Peek();
            if (token.Type == TokenType.EOL || token.Type == TokenType.EOF)
            {
                emitter.EmitLe(wordSize, 0);
                return;
            }
            var exp = ParseExpression();
            if (exp is string)
            {
                // A symbol!
                var sym = Assembler.Symtab.CreateSymbol((string)exp);
                Assembler.ReferToSymbol(sym, emitter.Position, wordSize);
                emitter.EmitLeUInt16(0);
                return;
            }
            emitter.EmitLe(wordSize, (int) exp);
        }

        private object ParseExpression()
        {
            var o = ParseTerm();
            while (lexer.PeekAndDiscard(TokenType.Plus))
            {
                var n = (int) o;
                o = ParseTerm();
                o = n + (int) o;
            }
            return o;
        }

        private object ParseTerm()
        {
            object exp;
            var token = lexer.Peek();
            if (token.Type == TokenType.Id)
            {
                return Evaluate((string) lexer.Expect(TokenType.Id));
            }
            if (token.Type == TokenType._Dot)
            {
                lexer.Get();
                return emitter.Position + (int) Assembler.BaseAddress.ToLinear();
            }
            if (token.Type == TokenType.Number)
            {
                exp = (int) lexer.Expect(TokenType.Number);
                return exp;
            }
            if (token.Type == TokenType.Register)
            {
                return lexer.Expect(TokenType.Register);
            }
            lexer.Unexpected(lexer.Get());
            return null;
        }

        private object Evaluate(string symbol)
        {
            object value;
            if (!Assembler.Equates.TryGetValue(symbol, out value))
                value = symbol;
            return value;
        }

        public Address StartAddress { get; private set; }

        public ICollection<ImageSymbol> EntryPoints
        {
            get { throw new NotImplementedException(); }
        }
        
        public ICollection<ImageSymbol> ImageSymbols { get; private set; }

        public Dictionary<Address, ImportReference> ImportReferences
        {
            get { throw new NotImplementedException(); }
        }

        public Pdp11Assembler Assembler { get; private set; }
    }
}
