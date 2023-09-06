#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Assemblers;
using Reko.Core.Hll.C;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public class AeonTextAssembler : IAssembler
    {
        private readonly AeonArchitecture arch;
        private CToken? token;

        public AeonTextAssembler(AeonArchitecture arch)
        {
            this.arch = arch;
        }

        public Address StartAddress => throw new NotImplementedException();

        public ICollection<ImageSymbol> EntryPoints => throw new NotImplementedException();

        public ICollection<ImageSymbol> ImageSymbols => throw new NotImplementedException();

        public Dictionary<Address, ImportReference> ImportReferences => throw new NotImplementedException();

        public Program Assemble(Address baseAddress, string filename, TextReader reader)
        {
            var lexer = new CLexer(reader, new Dictionary<string, CTokenType>());
            var parserState = new ParserState();
            var directiveLexer = new CDirectiveLexer(parserState, lexer);
            var entryPoints = new List<ImageSymbol>();
            var diagnostics = new AssemblerDiagnostics(filename);
            var asm = new AeonAssembler(arch, baseAddress, entryPoints, diagnostics);

            // Assemblers are strongly line-oriented.

            while (Peek(directiveLexer).Type != CTokenType.EOF)
            {
                diagnostics.LineNumber = lexer.LineNumber;
                ProcessLine(directiveLexer, asm);
            }

            asm.PerformRelocations();
            //addrStart = addrBase;
            if (diagnostics.Failed)
                return new Program();
            else 
                return asm.GetImage(baseAddress);
        }

        private void ProcessLine(CDirectiveLexer lex, AeonAssembler asm)
        {
            int curLine = lex.LineNumber;
            if (Peek(lex).Type == CTokenType.EOF || curLine != lex.LineNumber)
                return;

            // Eat any labels
            if (PeekAndDiscard(lex, CTokenType.Dot))
            {
                ParseDirective(lex, asm, "");
                return;
            }
            var t = Expect(lex, CTokenType.Id);
            var prefix = (string) t.Value!;
            while (PeekAndDiscard(lex, CTokenType.Colon))
            {
                asm.RegisterSymbol(prefix);
                if (Peek(lex).Type != CTokenType.Id)
                {
                    if (PeekAndDiscard(lex, CTokenType.Dot))
                    {
                        // .word and friends
                        ParseDirective(lex, asm, prefix);
                        return;
                    }
                    return;
                }
                t = Read(lex);
                prefix = (string) t.Value!;
            }

            Expect(lex, CTokenType.Dot);
            var tMnemonic = Expect(lex, CTokenType.Id);
            var mnemonic = (string) tMnemonic.Value!;
            switch (prefix)
            {
            case "bg":
                switch (mnemonic)
                {
                case "andi":
                    R_R_UImm(lex, asm, 0, AeonAssembler.RT_AEON_BG_LO16_0, asm.bg_andi);
                    break;
                case "beqi":
                    R_I_Disp(lex, asm, AeonAssembler.RT_AEON_BG_DISP13_3, asm.bg_beqi);
                    break;
                case "sb":
                    M_R(lex, asm, PrimitiveType.Byte, AeonAssembler.RT_AEON_BG_LO16_0, asm.bg_sb);
                    break;
                case "lbz":
                    R_M(lex, asm, PrimitiveType.Byte, AeonAssembler.RT_AEON_BG_LO16_0, asm.bg_lbz);
                    break;
                case "lwz":
                    R_M(lex, asm, PrimitiveType.Word32, AeonAssembler.RT_AEON_BG_LO14_2, asm.bg_lwz);
                    break;
                case "ori":
                    R_R_UImm(lex, asm, 0, AeonAssembler.RT_AEON_BG_LO16_0, asm.bg_ori);
                    break;
                case "movhi":
                    R_UImm(lex, asm, AeonAssembler.RT_AEON_BG_HI16_5, 0, asm.bg_movhi);
                    break;
                default:
                    throw new NotImplementedException(mnemonic);
                }
                break;
            case "bn":
                switch (mnemonic)
                {
                case "bnei":
                    R_I_Disp(lex, asm, AeonAssembler.RT_AEON_BN_DISP8_2, asm.bn_bnei);
                    break;
                case "j":
                    Disp(lex, asm, AeonAssembler.RT_AEON_BN_DISP18, asm.bn_j);
                    break;
                case "xor":
                    R_R_R(lex, asm, asm.bn_xor);
                    break;
                default:
                    throw new NotImplementedException($"{prefix}.{mnemonic}");
                }
                break;
            case "bt":
                switch (mnemonic)
                {
                case "addi":
                    R_SImm(lex, asm, 0, asm.bt_addi);
                    break;
                case "jr":
                    R(lex, asm, asm.bt_jr);
                    break;
                default:
                    throw new NotImplementedException($"{prefix}.{mnemonic}");
                }
                break;
            case "b":
                // Idea: try first with "bg", but if we can make it fit,  try again with smaller 
                // "bn" or "bt" instructions.
                goto case "bg";
            default:
                switch (mnemonic)
                {
                case "equ":
                    ParseEquate(lex, asm, prefix);
                    break;
                default:
                throw new NotImplementedException(prefix);
                }
                break;
            }
        }

        private void ParseDirective(CDirectiveLexer lex, AeonAssembler asm, string label)
        {
            // We just saw a '.' -- now get the directive name.
            var directive = Expect(lex, CTokenType.Id);
            switch ((string) directive.Value!)
            {
            case "word":
                var op = ParseUImmediateOperand(lex, asm, 0, 0);
                asm.EmitWord32(op);
                return;
            case "half":
                op = ParseUImmediateOperand(lex, asm, 0, 0);
                asm.EmitWord16(op);
                return;
            case "byte":
                do
                {
                    switch (Peek(lex).Type)
                    {
                    case CTokenType.StringLiteral:
                        var t = this.Read(lex);
                        var bytes = Encoding.UTF8.GetBytes((string) t.Value!);
                        asm.Emitter.EmitBytes(bytes);
                        break;
                    default:
                        op = ParseUImmediateOperand(lex, asm, 0, 0);
                        asm.EmitByte(op);
                        break;
                    }
                } while (PeekAndDiscard(lex, CTokenType.Comma));
                return;
            case "align":
                op = ParseUImmediateOperand(lex, asm, 0, 0);
                asm.Align(op);
                return;
            default:
                asm.Diagnostics.Error($"Unknown directive '.{directive}'.");
                return;
            }
        }

        private void ParseEquate(CDirectiveLexer lex, AeonAssembler asm, string label)
        {
            if (label.Length == 0)
            {
                ErrorToEol(lex, "Need a label for .equ");
                return;
            }
            var op = ParseUImmediateOperand(lex, asm, 0, 0);
            asm.Equate(label, op);
        }

        private void R(
            CDirectiveLexer lex,
            AeonAssembler asm,
            Action<RegisterStorage> assemble)
        {
            var r = ExpectRegister(lex);
            assemble(r);
        }

        private void R_SImm(
            CDirectiveLexer lex,
            AeonAssembler asm,
            int relocationType,
            Action<RegisterStorage, ParsedOperand> assemble)
        {
            var rdst = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var immop = this.ParseSImmediateOperand(lex);
            assemble(rdst, immop);
        }

        private void R_UImm(
            CDirectiveLexer lex,
            AeonAssembler asm,
            int relocationTypeHi,
            int relocationTypeLo,
            Action<RegisterStorage, ParsedOperand> assemble)
        {
            var rdst = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var immop = ParseUImmediateOperand(lex, asm, relocationTypeHi, relocationTypeLo);
            assemble(rdst, immop);
        }

        private void R_M(
            CDirectiveLexer lex,
            AeonAssembler asm,
            PrimitiveType dt, 
            int relocationType,
            Action<RegisterStorage, MemoryOperand> assemble)
        {
            var rdst = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var mop = ParseMemOperand(lex, asm, dt, relocationType);
            assemble(rdst, mop);
        }

        private void M_R(
            CDirectiveLexer lex,
            AeonAssembler asm,
            PrimitiveType dt,
            int relocationType,
            Action<MemoryOperand, RegisterStorage> assemble)
        {
            var mop = ParseMemOperand(lex, asm, dt, relocationType);
            Expect(lex, CTokenType.Comma);
            var rs = ExpectRegister(lex);
            assemble(mop, rs);
        }

        private void R_R_UImm(
            CDirectiveLexer lex, 
            AeonAssembler asm,
            int relocationTypeHi, 
            int relocationTypeLo,
            Action<RegisterStorage, RegisterStorage, ParsedOperand> assemble)
        {
            var rdst = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var rsrc1 = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var immop = ParseUImmediateOperand(lex, asm, relocationTypeHi, relocationTypeLo);
            assemble(rdst, rsrc1, immop);
        }

        private void R_R_R(
            CDirectiveLexer lex, 
            AeonAssembler asm, 
            Action<RegisterStorage, RegisterStorage, RegisterStorage> assemble)
        {
            var rdst = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var rsrc1 = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var rsrc2 = ExpectRegister(lex);
            assemble(rdst, rsrc1, rsrc2);
        }

        private void Disp(
            CDirectiveLexer lex,
            AeonAssembler asm, 
            int relocationType,
            Action<ParsedOperand> assemble)
        {
            var disp = ParseDisplacement(lex, asm, relocationType);
            assemble(disp);
        }

        private void R_I_Disp(
            CDirectiveLexer lex,
            AeonAssembler asm,
            int relocationType,
            Action<RegisterStorage, ParsedOperand, ParsedOperand> assemble)
        {
            var rsrc1 = ExpectRegister(lex);
            Expect(lex, CTokenType.Comma);
            var immop = ParseUImmediateOperand(lex, asm, 0, 0);
            Expect(lex, CTokenType.Comma);
            var disp = ParseDisplacement(lex, asm, relocationType);
            assemble(rsrc1, immop, disp);
        }

        private bool PeekAndDiscard(CDirectiveLexer lex, CTokenType tokenType)
        {
            var t = Peek(lex);
            if (t.Type != tokenType)
                return false;
            Read(lex);
            return true;
        }

        private ParsedOperand ParseDisplacement(
            CDirectiveLexer lex,
            AeonAssembler asm,
            int relocationType)
        {
            var tAddress = Read(lex);
            long displacement;
            if (tAddress.Type == CTokenType.Id)
            {
                // A symbol. Generate a relocation and leave a 0 displacement.
                var symbol = (string) tAddress.Value!;
                var reloc = new Relocation(asm.CurrentAddress, symbol, relocationType);
                asm.AddRelocation(in reloc);
                displacement = 0;
            }
            else if (tAddress.Type == CTokenType.NumericLiteral)
            {
                // Absolute address
                uint address = Convert.ToUInt32(tAddress.Value);
                var uAddrCur = asm.CurrentAddress;
                displacement = (long) address - uAddrCur.ToUInt32();
            }
            else
            {
                throw new ApplicationException($"Unexpected jump target {tAddress}.");
            }
            return ParsedOperand.Int32((int)displacement);
        }

        private ParsedOperand ParseUImmediateOperand(
            CDirectiveLexer lex,
            AeonAssembler asm,
            int hiRelocationType,
            int loRelocationType)
        {
            var t = Read(lex);
            if (t.Type == CTokenType.Id)
            {
                // Check for hi and low.
                var id = (string) t.Value!;
                uint value;
                if (id == "hi")
                {
                    value = ParseHiExpression(lex, asm, hiRelocationType);
                }
                else if (id == "lo")
                {
                    value = ParseLoExpression(lex, asm, loRelocationType);
                }
                else
                {
                    return ParsedOperand.Id(id);
                }
                return ParsedOperand.UInt32(value);
            }
            else if (t.Type == CTokenType.NumericLiteral)
            {
                uint offset = Convert.ToUInt32(t.Value);
                return ParsedOperand.UInt32(offset);
            }
            else
            {
                throw new NotImplementedException($"Unexpected token {t}.");
            }
        }

        private uint ParseHiExpression(CDirectiveLexer lex, AeonAssembler asm, int relocationType)
        {
            if (relocationType == 0)
                throw new ApplicationException("Unexpected hi or lo for this instruction.");
            Expect(lex, CTokenType.LParen);
            //$TODO expressions
            var tSymbol = Expect(lex, CTokenType.Id);
            Expect(lex, CTokenType.RParen);

            var symbol = (string) tSymbol.Value!;
            if (asm.Equates.TryGetValue(symbol, out var imm))
            {
                return imm.Value.ToUInt32() >> 16;
            }
            var reloc = new Relocation(asm.CurrentAddress, symbol, relocationType);
            asm.AddRelocation(reloc);
            return 0;
        }

        private uint ParseLoExpression(CDirectiveLexer lex, AeonAssembler asm, int relocationType)
        {
            if (relocationType == 0)
                throw new ApplicationException("Unexpected hi or lo for this instruction.");
            Expect(lex, CTokenType.LParen);
            //$TODO expressions
            var tSymbol = Expect(lex, CTokenType.Id);
            Expect(lex, CTokenType.RParen);

            var symbol = (string) tSymbol.Value!;
            if (asm.Equates.TryGetValue(symbol, out var imm))
            {
                return imm.Value.ToUInt32();
            }
            var reloc = new Relocation(asm.CurrentAddress, symbol, relocationType);
            asm.AddRelocation(reloc);
            return 0;
        }


        private ParsedOperand ParseSImmediateOperand(CDirectiveLexer lex)
        {
            var tOffset = Expect(lex, CTokenType.NumericLiteral);
            int offset = Convert.ToInt32(tOffset.Value);
            return ParsedOperand.Int32(offset);
        }

        private MemoryOperand ParseMemOperand(
            CDirectiveLexer lex,
            AeonAssembler asm,
            PrimitiveType dt,
            int relocationType)
        {
            int offset = 0;
            if (Peek(lex).Type != CTokenType.LParen)
            {
                var tOffset = Read(lex);
                switch (tOffset.Type)
                {
                case CTokenType.NumericLiteral:
                    offset = (int) tOffset.Value!;
                    break;
                case CTokenType.Id:
                    // Check for hi and low.
                    var id = (string) tOffset.Value!;
                    if (id == "hi")
                    {
                        throw new NotSupportedException("hi() is not supported in memory operands.");
                    }
                    else if (id == "lo")
                    {
                        // This relcation is incorrect for lbz, lhz
                        offset = (int)ParseLoExpression(lex, asm, relocationType);
                    }
                    else
                    {
                        throw new NotImplementedException("Constants NYI.");
                    }
                    break; 
                }
            }
            Expect(lex, CTokenType.LParen);
            var regBase = ExpectRegister(lex);
            Expect(lex, CTokenType.RParen);
            return new MemoryOperand(dt)
            {
                Offset = offset,
                Base = regBase
            };
        }

        private RegisterStorage ExpectRegister(CDirectiveLexer lex)
        {
            var tReg = Expect(lex, CTokenType.Id);
            var sReg = (string) tReg.Value!;
            if (!arch.TryGetRegister(sReg, out var reg))
                throw new ApplicationException($"Expected a register but found '{sReg}' on line {lex.LineNumber}.");
            return reg;
        }

        private CToken Expect(CDirectiveLexer directiveLexer, CTokenType expectedType)
        {
            var t = Read(directiveLexer);
            if (t.Type != expectedType)
            {
                throw new ApplicationException($"Expected {expectedType} but got {t.Type}.");
            }
            return t;
        }

        private CToken Read(CDirectiveLexer directiveLexer)
        {
            if (this.token.HasValue)
            {
                var t = this.token.Value;
                this.token = null;
                return t;
            }
            return directiveLexer.Read();
        }

        private CToken Peek(CDirectiveLexer directiveLexer)
        {
            if (!this.token.HasValue)
            {
                this.token = directiveLexer.Read();
            }
            return this.token.Value;
        }

        public int AssembleAt(Program program, Address address, TextReader reader)
        {
            throw new NotImplementedException();
        }

        public Program AssembleFragment(Address baseAddress, string asmFragment)
        {
            var rdr = new StringReader(asmFragment);
            return Assemble(baseAddress, "", rdr);
        }

        public int AssembleFragmentAt(Program program, Address address, string asmFragment)
        {
            throw new NotImplementedException();
        }

        private void ErrorToEol(CDirectiveLexer lex, string message)
        {

        }
    }
}
