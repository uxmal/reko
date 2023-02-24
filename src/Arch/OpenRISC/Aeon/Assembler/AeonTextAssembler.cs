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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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

        public Program Assemble(Address baseAddress, TextReader reader)
        {
            var lexer = new CLexer(reader, new Dictionary<string, CTokenType>());
            var parserState = new ParserState();
            var directiveLexer = new CDirectiveLexer(parserState, lexer);
            var entryPoints = new List<ImageSymbol>();
            var asm = new AeonAssembler(arch, baseAddress, entryPoints);

            // Assemblers are strongly line-oriented.

            while (Peek(directiveLexer).Type != CTokenType.EOF)
            {
                ProcessLine(directiveLexer, asm);
            }

            asm.PerformRelocations();
            //addrStart = addrBase;
            return asm.GetImage(baseAddress);
        }

        private void ProcessLine(CDirectiveLexer lex, AeonAssembler asm)
        {
            int curLine = lex.LineNumber;
            if (Peek(lex).Type == CTokenType.EOF || curLine != lex.LineNumber)
                return;

            // Eat any labels
            var tPrefix = Expect(lex, CTokenType.Id);
            var prefix = (string) tPrefix.Value!;
            while (PeekAndDiscard(lex, CTokenType.Colon))
            {
                asm.RegisterSymbol(prefix);
                if (Peek(lex).Type != CTokenType.Id)
                    return;
                tPrefix = Read(lex);
                prefix = (string) tPrefix.Value!;
            }

            Expect(lex, CTokenType.Dot);
            var tMnemonic = Expect(lex, CTokenType.Id);
            var mnemonic = (string) tMnemonic.Value!;
            RegisterStorage rsrc1;
            RegisterStorage rsrc2;
            RegisterStorage rdst;
            MemoryOperand mop;
            ImmediateOperand immop;
            ImmediateOperand disp;
            switch (prefix)
            {
            case "bg":
                switch (mnemonic)
                {
                case "andi":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    rsrc1 = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = ParseUImmediateOperand(lex);
                    asm.bg_andi(rdst, rsrc1, immop);
                    break;
                case "beqi":
                    rsrc1 = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = ParseUImmediateOperand(lex);
                    Expect(lex, CTokenType.Comma);
                    disp = ParseDisplacement(lex, asm, AeonAssembler.RT_AEON_BG_DISP13_3);
                    asm.bg_beqi(rsrc1, immop, disp);
                    break;
                case "sb":
                    mop = ParseMemOperand(lex, PrimitiveType.Byte);
                    Expect(lex, CTokenType.Comma);
                    rsrc1 = ExpectRegister(lex);
                    asm.bg_sb(mop, rsrc1);
                    break;
                case "lbz":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    mop = ParseMemOperand(lex, PrimitiveType.Byte);
                    asm.bg_lbz(rdst, mop);
                    break;
                case "ori":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    rsrc1 = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = ParseUImmediateOperand(lex);
                    asm.bg_ori(rdst, rsrc1, immop);
                    break;
                case "movhi":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = ParseUImmediateOperand(lex);
                    asm.bg_movhi(rdst, immop);
                    break;
                default:
                    throw new NotImplementedException(mnemonic);
                }
                break;
            case "bn":
                switch (mnemonic)
                {
                case "bnei":
                    rsrc1 = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = ParseUImmediateOperand(lex);
                    Expect(lex, CTokenType.Comma);
                    disp = ParseDisplacement(lex, asm, AeonAssembler.RT_AEON_BN_DISP8_2);
                    asm.bn_bnei(rsrc1, immop, disp);
                    break;
                case "j":
                    disp = ParseDisplacement(lex, asm, AeonAssembler.RT_AEON_BN_DISP18);
                    asm.bn_j(disp);
                    break;
                case "xor":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    rsrc1 = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    rsrc2 = ExpectRegister(lex);
                    asm.bn_xor(rdst, rsrc1, rsrc2);
                    break;
                default:
                throw new NotImplementedException($"{prefix}.{mnemonic}");
                }
                break;
            case "bt":
                switch (mnemonic)
                {
                case "addi":
                    rdst = ExpectRegister(lex);
                    Expect(lex, CTokenType.Comma);
                    immop = this.ParseSImmediateOperand(lex);
                    asm.bt_addi(rdst, immop);
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
                throw new NotImplementedException(prefix);
            }
        }

        private bool PeekAndDiscard(CDirectiveLexer lex, CTokenType tokenType)
        {
            var t = Peek(lex);
            if (t.Type != tokenType)
                return false;
            Read(lex);
            return true;
        }

        private ImmediateOperand ParseDisplacement(
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
            return ImmediateOperand.Int32((int)displacement);
        }

        private ImmediateOperand ParseUImmediateOperand(CDirectiveLexer lex)
        {
            var tOffset = Expect(lex, CTokenType.NumericLiteral);
            uint offset = Convert.ToUInt32(tOffset.Value);
            return ImmediateOperand.UInt32(offset);
        }

        private ImmediateOperand ParseSImmediateOperand(CDirectiveLexer lex)
        {
            var tOffset = Expect(lex, CTokenType.NumericLiteral);
            int offset = Convert.ToInt32(tOffset.Value);
            return ImmediateOperand.Int32(offset);
        }

        private MemoryOperand ParseMemOperand(CDirectiveLexer lex, PrimitiveType dt)
        {
            int offset = 0;
            if (Peek(lex).Type != CTokenType.LParen)
            {
                var tOffset = Expect(lex, CTokenType.NumericLiteral);
                offset = (int) tOffset.Value!;
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
                throw new ApplicationException($"Expected a register but found '{sReg}'.");
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
            throw new NotImplementedException();
        }

        public int AssembleAt(Program program, Address address, TextReader reader)
        {
            throw new NotImplementedException();
        }

        public Program AssembleFragment(Address baseAddress, string asmFragment)
        {
            var rdr = new StringReader(asmFragment);
            return Assemble(baseAddress, rdr);
        }

        public int AssembleFragmentAt(Program program, Address address, string asmFragment)
        {
            throw new NotImplementedException();
        }
    }
}
