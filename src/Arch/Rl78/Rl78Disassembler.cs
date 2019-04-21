#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Arch.Rl78
{
    public class Rl78Disassembler : DisassemblerBase<Rl78Instruction>
    {
        private static readonly Decoder[] s_decoders;
        private static readonly Decoder s_invalid;

        private readonly Rl78Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Rl78Disassembler(Rl78Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Rl78Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            ops.Clear();
            var instr = s_decoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        private Rl78Instruction Invalid()
        {
            return new Rl78Instruction
            {
                IClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = new MachineOperand[0]
            };
        }

        private void EmitUnitTest(string message)
        {

        }

        #region Mutators

        private static Mutator<Rl78Disassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private readonly static Mutator<Rl78Disassembler> X = Reg(Registers.x);
        private readonly static Mutator<Rl78Disassembler> A = Reg(Registers.a);
        private readonly static Mutator<Rl78Disassembler> C = Reg(Registers.c);
        private readonly static Mutator<Rl78Disassembler> B = Reg(Registers.b);
        private readonly static Mutator<Rl78Disassembler> E = Reg(Registers.e);
        private readonly static Mutator<Rl78Disassembler> D = Reg(Registers.d);
        private readonly static Mutator<Rl78Disassembler> L = Reg(Registers.l);
        private readonly static Mutator<Rl78Disassembler> H = Reg(Registers.h);

        private readonly static Mutator<Rl78Disassembler> AX = Reg(Registers.ax);
        private readonly static Mutator<Rl78Disassembler> BC = Reg(Registers.bc);
        private readonly static Mutator<Rl78Disassembler> DE = Reg(Registers.de);
        private readonly static Mutator<Rl78Disassembler> HL = Reg(Registers.hl);
        private readonly static Mutator<Rl78Disassembler> SP = Reg(Registers.sp);

        private readonly static Mutator<Rl78Disassembler> ES = Reg(Registers.es);

        private static bool Ib(uint uInstr, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(imm));
            return true;
        }

        private static bool Iw(uint uInstr, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(imm));
            return true;
        }


        private static Mutator<Rl78Disassembler> Mem(PrimitiveType dt, RegisterStorage baseReg)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> M_de_b = MemOff(PrimitiveType.Byte, Registers.de);
        private static readonly Mutator<Rl78Disassembler> M_de_w = MemOff(PrimitiveType.Word16, Registers.de);
        private static readonly Mutator<Rl78Disassembler> M_hl_b = MemOff(PrimitiveType.Byte, Registers.hl);
        private static readonly Mutator<Rl78Disassembler> M_hl_w = MemOff(PrimitiveType.Word16, Registers.hl);


        // [HL+byte]
        private static Mutator<Rl78Disassembler> MemOff(PrimitiveType dt, RegisterStorage baseReg)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte offset))
                    return false;
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = offset
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> M_de_off_b = MemOff(PrimitiveType.Byte, Registers.de);
        private static readonly Mutator<Rl78Disassembler> M_de_off_w = MemOff(PrimitiveType.UInt16, Registers.de);

        private static readonly Mutator<Rl78Disassembler> M_hl_off_b = MemOff(PrimitiveType.Byte, Registers.hl);
        private static readonly Mutator<Rl78Disassembler> M_hl_off_w = MemOff(PrimitiveType.UInt16, Registers.hl);

        private static readonly Mutator<Rl78Disassembler> M_sp_off_b = MemOff(PrimitiveType.Byte, Registers.sp);
        private static readonly Mutator<Rl78Disassembler> M_sp_off_w = MemOff(PrimitiveType.UInt16, Registers.sp);

        private static Mutator<Rl78Disassembler> MwordReg(PrimitiveType dt, RegisterStorage indexReg)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadUInt16(out ushort offset))
                    return false;
                var mem = new MemoryOperand(dt)
                {
                    Offset = offset,
                    Index = indexReg,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> Mword_b_b = MwordReg(PrimitiveType.Byte, Registers.b);
        private static readonly Mutator<Rl78Disassembler> Mword_b_w = MwordReg(PrimitiveType.Word16, Registers.b);
        private static readonly Mutator<Rl78Disassembler> Mword_c_b = MwordReg(PrimitiveType.Byte, Registers.c);
        private static readonly Mutator<Rl78Disassembler> Mword_c_w = MwordReg(PrimitiveType.Word16, Registers.c);
        private static readonly Mutator<Rl78Disassembler> Mword_bc_b = MwordReg(PrimitiveType.Byte, Registers.bc);
        private static readonly Mutator<Rl78Disassembler> Mword_bc_w = MwordReg(PrimitiveType.Word16, Registers.bc);

        private static Mutator<Rl78Disassembler> Maddr16(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeUInt16(out ushort uAddr))
                    return false;
                var mem = new MemoryOperand(dt)
                {
                    Offset = uAddr
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> Maddr16b = Maddr16(PrimitiveType.Byte);
        private static readonly Mutator<Rl78Disassembler> Maddr16w = Maddr16(PrimitiveType.Word16);

        private static Mutator<Rl78Disassembler> saddress(PrimitiveType dt, int uAddrBase)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte off))
                    return false;
                var mem = new MemoryOperand(dt)
                {
                    Offset = uAddrBase + off
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> saddr = saddress(PrimitiveType.Byte, 0xFFE20);
        private static readonly Mutator<Rl78Disassembler> saddrp = saddress(PrimitiveType.Word16, 0xFFE20);
        private static readonly Mutator<Rl78Disassembler> sfr = saddress(PrimitiveType.Byte, 0xFFF00);
        private static readonly Mutator<Rl78Disassembler> sfrp = saddress(PrimitiveType.Word16, 0xFFF00);

        private static bool PcRelative8(uint op, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte upcRelativeOffset))
                return false;
            sbyte pcRelativeOffset = (sbyte) upcRelativeOffset;
            var addrOp = AddressOperand.Create(dasm.rdr.Address + pcRelativeOffset);
            dasm.ops.Add(addrOp);
            return true;
        }

        private static bool PcRelative16(uint op, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort upcRelativeOffset))
                return false;
            short pcRelativeOffset = (short) upcRelativeOffset;
            var addrOp = AddressOperand.Create(dasm.rdr.Address + pcRelativeOffset);
            dasm.ops.Add(addrOp);
            return true;
        }

        private static bool ImmAddr20(uint op, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort upcRelativeOffset))
                return false;
            if (!dasm.rdr.TryReadByte(out byte seg))
                return false;
            uint uAbsAddress = ((uint) seg << 16) | (uint) upcRelativeOffset;
            var addrOp = AddressOperand.Ptr32(uAbsAddress);
            dasm.ops.Add(addrOp);
            return true;
        }

        private static bool ImmAddr16(uint op, Rl78Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort upcRelativeOffset))
                return false;
            uint uAbsAddress = (0xFu << 16) | (uint) upcRelativeOffset;
            var addrOp = AddressOperand.Ptr32(uAbsAddress);
            dasm.ops.Add(addrOp);
            return true;
        }

        #endregion
        private abstract class Decoder
        {
            public abstract Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnem;
            private readonly Mutator<Rl78Disassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnem, Mutator<Rl78Disassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnem = mnem;
                this.mutators = mutators;
            }

            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.Invalid();
                }
                var instr = new Rl78Instruction
                {
                    IClass = iclass,
                    Mnemonic = mnem,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                dasm.EmitUnitTest(message);
                return dasm.Invalid();
            }
        }

        private static Decoder Instr(Mnemonic mnem, InstrClass iclass, params Mutator<Rl78Disassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnem, mutators);
        }

        private static Decoder Instr(Mnemonic mnem, params Mutator<Rl78Disassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnem, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        static Rl78Disassembler()
        {
            s_invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            /*
             * ext-1
             *  ADD X,A ADD A,A ADD C,A ADD B,A ADD E,A ADD D,A ADD L,A ADD H,A ADD A,X ADDW AX,[HL+byte] ADD A,C ADD A,B ADD A,E ADD A,D ADD A,L ADD A,H 1 ADDC X,A ADDC A,A ADDC C,A ADDC B,A ADDC E,A ADDC D,A ADDC L,A ADDC H,A ADDC A,X ADDC A,C ADDC A,B ADDC A,E ADDC A,D ADDC A,L ADDC A,H 2 SUB X,A SUB A,A SUB C,A SUB B,A SUB E,A SUB D,A SUB L,A SUB H,A SUB A,X SUBW AX,[HL+byte] SUB A,C SUB A,B SUB A,E SUB A,D SUB A,L SUB A,H 3 SUBC X,A SUBC A,A SUBC C,A SUBC B,A SUBC E,A SUBC D,A SUBC L,A SUBC H,A SUBC A,X SUBC A,C SUBC A,B SUBC A,E SUBC A,D SUBC A,L SUBC A,H 4 CMP X,A CMP A,A CMP C,A CMP B,A CMP E,A CMP D,A CMP L,A CMP H,A CMP A,X CMPW AX,[HL+byte] CMP A,C CMP A,B CMP A,E CMP A,D CMP A,L CMP A,H 5 AND X,A AND A,A AND C,A AND B,A AND E,A AND D,A AND L,A AND H,A AND A,X INC [HL+byte] AND A,C AND A,B AND A,E AND A,D AND A,L AND A,H 6 OR X,A OR A,A OR C,A OR B,A OR E,A OR D,A OR L,A OR H,A OR A,X DEC [HL+byte] OR A,C OR A,B OR A,E OR A,D OR A,L OR A,H 7 XOR X,A XOR A,A XOR C,A XOR B,A XOR E,A XOR D,A XOR L,A XOR H,A XOR A,X INCW [HL+byte] XOR A,C XOR A,B XOR A,E XOR A,D XOR A,L XOR A,H 8 ADD A,[HL+B] ADD A,[HL+C] CALLT [0080h] CALLT [0090h] CALLT [00A0h] CALLT [00B0h] DECW [HL+byte] XCH A,C XCH A,B XCH A,E XCH A,D XCH A,L XCH A,H 9 ADDC A,[HL+B] ADDC A,[HL+C] CALLT [0082h] CALLT [0092h] CALLT [00A2h] CALLT [00B2h] a SUB A,[HL+B] SUB A,[HL+C] CALLT [0084h] CALLT [0094h] CALLT [00A4h] CALLT [00B4h] XCH A,saddr XCH A,[HL+C] XCH A,!addr16 XCH A,sfr XCH A,M_hl XCH A,[HL+byte] XCH A,[DE] XCH A,M_de_off_b b SUBC A,[HL+B] SUBC A,[HL+C] CALLT [0086h] CALLT [0096h] CALLT [00A6h] CALLT [00B6h] MOV ES,saddr XCH A,[HL+B] c CMP A,[HL+B] CMP A,[HL+C] BH PcRelative8 CALLT [0088h] CALLT [0098h] CALLT [00A8h] CALLT [00B8h] SKC MOV A,[HL+B] CALL AX BR AX BRK POP PSW MOVS [HL+byte],X SEL RB0Note d AND A,[HL+B] AND A,[HL+C] BNH PcRelative8 CALLT [008Ah] CALLT [009Ah] CALLT [00AAh] CALLT [00BAh] SKNC MOV [HL+B],A CALL BC ROR A,1 ROLC A,1 PUSH PSW CMPS X,[HL+byte] SEL RB1Note e OR A,[HL+B] OR A,[HL+C] SKH CALLT [008Ch] CALLT [009Ch] CALLT [00ACh] CALLT [00BCh] SKZ MOV A,[HL+C] CALL DE ROL A,1 RETB HALT ROLWC AX,1 SEL RB2Note f XOR A,[HL+B] XOR A,[HL+C] SKNH CALLT [008Eh] CALLT [009Eh] CALLT [00AEh] CALLT [00BEh] SKNZ MOV [HL+C],A CALL HL RORC A,1 RETI STOP ROLWC BC,1 SEL RB3Note Note Not mounted
             */


            /*
             * ext-3
             *  0 SET1 !addr16.0 MOV1 saddr.0,CY SET1 saddr.0 CLR1 saddr.0 MOV1 CY,saddr.0 AND1 CY,saddr.0 OR1 CY,saddr.0 XOR1 CY,saddr.0 CLR1 !addr16.0 MOV1 sfr.0,CY SET1 sfr.0 CLR1 sfr.0 MOV1 CY,sfr.0 AND1 CY,sfr.0 OR1 CY,sfr.0 XOR1 CY,sfr.0 1 SET1 !addr16.1 MOV1 saddr.1,CY SET1 saddr.1 CLR1 saddr.1 MOV1 CY,saddr.1 AND1 CY,saddr.1 OR1 CY,saddr.1 XOR1 CY,saddr.1 CLR1 !addr16.1 MOV1 sfr.1,CY SET1 sfr.1 CLR1 sfr.1 MOV1 CY,sfr.1 AND1 CY,sfr.1 OR1 CY,sfr.1 XOR1 CY,sfr.1 2 SET1 !addr16.2 MOV1 saddr.2,CY SET1 saddr.2 CLR1 saddr.2 MOV1 CY,saddr.2 AND1 CY,saddr.2 OR1 CY,saddr.2 XOR1 CY,saddr.2 CLR1 !addr16.2 MOV1 sfr.2,CY SET1 sfr.2 CLR1 sfr.2 MOV1 CY,sfr.2 AND1 CY,sfr.2 OR1 CY,sfr.2 XOR1 CY,sfr.2 3 SET1 !addr16.3 MOV1 saddr.3,CY SET1 saddr.3 CLR1 saddr.3 MOV1 CY,saddr.3 AND1 CY,saddr.3 OR1 CY,saddr.3 XOR1 CY,saddr.3 CLR1 !addr16.3 MOV1 sfr.3,CY SET1 sfr.3 CLR1 sfr.3 MOV1 CY,sfr.3 AND1 CY,sfr.3 OR1 CY,sfr.3 XOR1 CY,sfr.3 4 SET1 !addr16.4 MOV1 saddr.4,CY SET1 saddr.4 CLR1 saddr.4 MOV1 CY,saddr.4 AND1 CY,saddr.4 OR1 CY,saddr.4 XOR1 CY,saddr.4 CLR1 !addr16.4 MOV1 sfr.4,CY SET1 sfr.4 CLR1 sfr.4 MOV1 CY,sfr.4 AND1 CY,sfr.4 OR1 CY,sfr.4 XOR1 CY,sfr.4 5 SET1 !addr16.5 MOV1 saddr.5,CY SET1 saddr.5 CLR1 saddr.5 MOV1 CY,saddr.5 AND1 CY,saddr.5 OR1 CY,saddr.5 XOR1 CY,saddr.5 CLR1 !addr16.5 MOV1 sfr.5,CY SET1 sfr.5 CLR1 sfr.5 MOV1 CY,sfr.5 AND1 CY,sfr.5 OR1 CY,sfr.5 XOR1 CY,sfr.5 6 SET1 !addr16.6 MOV1 saddr.6,CY SET1 saddr.6 CLR1 saddr.6 MOV1 CY,saddr.6 AND1 CY,saddr.6 OR1 CY,saddr.6 XOR1 CY,saddr.6 CLR1 !addr16.6 MOV1 sfr.6,CY SET1 sfr.6 CLR1 sfr.6 MOV1 CY,sfr.6 AND1 CY,sfr.6 OR1 CY,sfr.6 XOR1 CY,sfr.6 7 SET1 !addr16.7 MOV1 saddr.7,CY SET1 saddr.7 CLR1 saddr.7 MOV1 CY,saddr.7 AND1 CY,saddr.7 OR1 CY,saddr.7 XOR1 CY,saddr.7 CLR1 !addr16.7 MOV1 sfr.7,CY SET1 sfr.7 CLR1 sfr.7 MOV1 CY,sfr.7 AND1 CY,sfr.7 OR1 CY,sfr.7 XOR1 CY,sfr.7 8 SET1 CY MOV1 M_hl.0,CY SET1 M_hl.0 CLR1 M_hl.0 MOV1 CY,M_hl.0 AND1 CY,M_hl.0 OR1 CY,M_hl.0 XOR1 CY,M_hl.0 CLR1 CY MOV1 A.0,CY SET1 A.0 CLR1 A.0 MOV1 CY,A.0 AND1 CY,A.0 OR1 CY,A.0 XOR1 CY,A.0 9 MOV1 M_hl.1,CY SET1 M_hl.1 CLR1 M_hl.1 MOV1 CY,M_hl.1 AND1 CY,M_hl.1 OR1 CY,M_hl.1 XOR1 CY,M_hl.1 MOV1 A.1,CY SET1 A.1 CLR1 A.1 MOV1 CY,A.1 AND1 CY,A.1 OR1 CY,A.1 XOR1 CY,A.1 a MOV1 M_hl.2,CY SET1 M_hl.2 CLR1 M_hl.2 MOV1 CY,M_hl.2 AND1 CY,M_hl.2 OR1 CY,M_hl.2 XOR1 CY,M_hl.2 MOV1 A.2,CY SET1 A.2 CLR1 A.2 MOV1 CY,A.2 AND1 CY,A.2 OR1 CY,A.2 XOR1 CY,A.2 b MOV1 M_hl.3,CY SET1 M_hl.3 CLR1 M_hl.3 MOV1 CY,M_hl.3 AND1 CY,M_hl.3 OR1 CY,M_hl.3 XOR1 CY,M_hl.3 MOV1 A.3,CY SET1 A.3 CLR1 A.3 MOV1 CY,A.3 AND1 CY,A.3 OR1 CY,A.3 XOR1 CY,A.3 c NOT1 CY MOV1 M_hl.4,CY SET1 M_hl.4 CLR1 M_hl.4 MOV1 CY,M_hl.4 AND1 CY,M_hl.4 OR1 CY,M_hl.4 XOR1 CY,M_hl.4 MOV1 A.4,CY SET1 A.4 CLR1 A.4 MOV1 CY,A.4 AND1 CY,A.4 OR1 CY,A.4 XOR1 CY,A.4 d MOV1 M_hl.5,CY SET1 M_hl.5 CLR1 M_hl.5 MOV1 CY,M_hl.5 AND1 CY,M_hl.5 OR1 CY,M_hl.5 XOR1 CY,M_hl.5 MOV1 A.5,CY SET1 A.5 CLR1 A.5 MOV1 CY,A.5 AND1 CY,A.5 OR1 CY,A.5 XOR1 CY,A.5 e MOV1 M_hl.6,CY SET1 M_hl.6 CLR1 M_hl.6 MOV1 CY,M_hl.6 AND1 CY,M_hl.6 OR1 CY,M_hl.6 XOR1 CY,M_hl.6 MOV1 A.6,CY SET1 A.6 CLR1 A.6 MOV1 CY,A.6 AND1 CY,A.6 OR1 CY,A.6 XOR1 CY,A.6 f MOV1 M_hl.7,CY SET1 M_hl.7 CLR1 M_hl.7 MOV1 CY,M_hl.7 AND1 CY,M_hl.7 OR1 CY,M_hl.7 XOR1 CY,M_hl.7 MOV1 A.7,CY SET1 A.7 CLR1 A.7 MOV1 CY,A.7 AND1 CY,A.7 OR1 CY,A.7 XOR1 CY,A.7 
             */

            /*
             * ext-4
             *  BTCLR saddr.0,PcRelative8 BTCLR A.0,PcRelative8 BT saddr.0,PcRelative8 BT A.0,PcRelative8 BF saddr.0,PcRelative8 BF A.0,PcRelative8 1 BTCLR saddr.1,PcRelative8 BTCLR A.1,PcRelative8 BT saddr.1,PcRelative8 BT A.1,PcRelative8 BF saddr.1,PcRelative8 BF A.1,PcRelative8 SHL C,1 SHL B,1 SHL A,1 SHR A,1 SAR A,1 SHLW BC,1 SHLW AX,1 SHRW AX,1 SARW AX,1 2 BTCLR saddr.2,PcRelative8 BTCLR A.2,PcRelative8 BT saddr.2,PcRelative8 BT A.2,PcRelative8 BF saddr.2,PcRelative8 BF A.2,PcRelative8 SHL C,2 SHL B,2 SHL A,2 SHR A,2 SAR A,2 SHLW BC,2 SHLW AX,2 SHRW AX,2 SARW AX,2 3 BTCLR saddr.3,PcRelative8 BTCLR A.3,PcRelative8 BT saddr.3,PcRelative8 BT A.3,PcRelative8 BF saddr.3,PcRelative8 BF A.3,PcRelative8 SHL C,3 SHL B,3 SHL A,3 SHR A,3 SAR A,3 SHLW BC,3 SHLW AX,3 SHRW AX,3 SARW AX,3 4 BTCLR saddr.4,PcRelative8 BTCLR A.4,PcRelative8 BT saddr.4,PcRelative8 BT A.4,PcRelative8 BF saddr.4,PcRelative8 BF A.4,PcRelative8 SHL C,4 SHL B,4 SHL A,4 SHR A,4 SAR A,4 SHLW BC,4 SHLW AX,4 SHRW AX,4 SARW AX,4 5 BTCLR saddr.5,PcRelative8 BTCLR A.5,PcRelative8 BT saddr.5,PcRelative8 BT A.5,PcRelative8 BF saddr.5,PcRelative8 BF A.5,PcRelative8 SHL C,5 SHL B,5 SHL A,5 SHR A,5 SAR A,5 SHLW BC,5 SHLW AX,5 SHRW AX,5 SARW AX,5 6 BTCLR saddr.6,PcRelative8 BTCLR A.6,PcRelative8 BT saddr.6,PcRelative8 BT A.6,PcRelative8 BF saddr.6,PcRelative8 BF A.6,PcRelative8 SHL C,6 SHL B,6 SHL A,6 SHR A,6 SAR A,6 SHLW BC,6 SHLW AX,6 SHRW AX,6 SARW AX,6 7 BTCLR saddr.7,PcRelative8 BTCLR A.7,PcRelative8 BT saddr.7,PcRelative8 BT A.7,PcRelative8 BF saddr.7,PcRelative8 BF A.7,PcRelative8 SHL C,7 SHL B,7 SHL A,7 SHR A,7 SAR A,7 SHLW BC,7 SHLW AX,7 SHRW AX,7 SARW AX,7 8 BTCLR sfr.0,PcRelative8 BTCLR M_hl.0,PcRelative8 BT sfr.0,PcRelative8 BT M_hl.0,PcRelative8 BF sfr.0,PcRelative8 BF M_hl.0,PcRelative8 SHLW BC,8 SHLW AX,8 SHRW AX,8 SARW AX,8 9 BTCLR sfr.1,PcRelative8 BTCLR M_hl.1,PcRelative8 BT sfr.1,PcRelative8 BT M_hl.1,PcRelative8 BF sfr.1,PcRelative8 BF M_hl.1,PcRelative8 SHLW BC,9 SHLW AX,9 SHRW AX,9 SARW AX,9 a BTCLR sfr.2,PcRelative8 BTCLR M_hl.2,PcRelative8 BT sfr.2,PcRelative8 BT M_hl.2,PcRelative8 BF sfr.2,PcRelative8 BF M_hl.2,PcRelative8 SHLW BC,10 SHLW AX,10 SHRW AX,10 SARW AX,10 b BTCLR sfr.3,PcRelative8 BTCLR M_hl.3,PcRelative8 BT sfr.3,PcRelative8 BT M_hl.3,PcRelative8 BF sfr.3,PcRelative8 BF M_hl.3,PcRelative8 SHLW BC,11 SHLW AX,11 SHRW AX,11 SARW AX,11 c BTCLR sfr.4,PcRelative8 BTCLR M_hl.4,PcRelative8 BT sfr.4,PcRelative8 BT M_hl.4,PcRelative8 BF sfr.4,PcRelative8 BF M_hl.4,PcRelative8 SHLW BC,12 SHLW AX,12 SHRW AX,12 SARW AX,12 d BTCLR sfr.5,PcRelative8 BTCLR M_hl.5,PcRelative8 BT sfr.5,PcRelative8 BT M_hl.5,PcRelative8 BF sfr.5,PcRelative8 BF M_hl.5,PcRelative8 SHLW BC,13 SHLW AX,13 SHRW AX,13 SARW AX,13 e BTCLR sfr.6,PcRelative8 BTCLR M_hl.6,PcRelative8 BT sfr.6,PcRelative8 BT M_hl.6,PcRelative8 BF sfr.6,PcRelative8 BF M_hl.6,PcRelative8 SHLW BC,14 SHLW AX,14 SHRW AX,14 SARW AX,14 f BTCLR sfr.7,PcRelative8 BTCLR M_hl.7,PcRelative8 BT sfr.7,PcRelative8 BT M_hl.7,PcRelative8 BF sfr.7,PcRelative8 BF M_hl.7,PcRelative8 SHLW BC,15 SHLW AX,15 SHRW AX,15 SARW AX,15 
             */

            s_decoders = new Decoder[]
            {
// 0x00
Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding | InstrClass.Zero),
Instr(Mnemonic.ADDW, AX, AX),                Instr(Mnemonic.ADDW, AX,Maddr16w),
Instr(Mnemonic.ADDW, AX, BC),

Instr(Mnemonic.ADDW, AX,Iw ),
Instr(Mnemonic.ADDW, AX,DE),
Instr(Mnemonic.ADDW, AX,saddrp),
Instr(Mnemonic.ADDW, AX,HL),

Instr(Mnemonic.XCH, A,X),
Instr(Mnemonic.MOV, A,Mword_b_b),
Instr(Mnemonic.ADD, saddr,Ib),
Instr(Mnemonic.ADD, A,saddr),

Instr(Mnemonic.ADD, A,Ib ),
Instr(Mnemonic.ADD, A,M_hl_b),
Instr(Mnemonic.ADD, A,M_hl_off_b),
Instr(Mnemonic.ADD, A,Maddr16b),

// 0x10
Instr(Mnemonic.ADDW, SP,Ib ),
Nyi("PREFIX "),
Instr(Mnemonic.MOVW, BC,AX),
Instr(Mnemonic.MOVW, AX,BC),

Instr(Mnemonic.MOVW, DE,AX),
Instr(Mnemonic.MOVW, AX,DE),
Instr(Mnemonic.MOVW, HL,AX),
Instr(Mnemonic.MOVW, AX,HL),

Instr(Mnemonic.MOV, Mword_b_b,A),
Instr(Mnemonic.MOV, Mword_b_b,Ib ),
Instr(Mnemonic.ADDC, saddr,Ib ),
Instr(Mnemonic.ADDC, A,saddr),

Instr(Mnemonic.ADDC, A,Ib ),
Instr(Mnemonic.ADDC, A,M_hl_b),
Instr(Mnemonic.ADDC, A,M_hl_off_b),
Instr(Mnemonic.ADDC, A,Maddr16b),

// 0x20
Instr(Mnemonic.SUBW, SP,Ib ),
Nyi("0x21"),
Instr(Mnemonic.SUBW, AX,Maddr16w),
Instr(Mnemonic.SUBW, AX, BC),


Instr(Mnemonic.SUBW, AX,Iw ),
Instr(Mnemonic.SUBW, AX,DE),
Instr(Mnemonic.SUBW, AX,saddrp),
Instr(Mnemonic.SUBW, AX,HL),

Instr(Mnemonic.MOV, Mword_c_b,A),
Instr(Mnemonic.MOV, A,Mword_c_b),
Instr(Mnemonic.SUB, saddr,Ib ),
Instr(Mnemonic.SUB, A,saddr),

Instr(Mnemonic.SUB, A,Ib ),
Instr(Mnemonic.SUB, A,M_hl_b),
Instr(Mnemonic.SUB, A,M_hl_off_b),
Instr(Mnemonic.SUB, A,Maddr16b),

// 0x30
Instr(Mnemonic.MOVW, AX,Iw),
Nyi("4th MAP"),
Instr(Mnemonic.MOVW, BC,Iw ),
Instr(Mnemonic.XCHW, AX,BC),

Instr(Mnemonic.MOVW, DE,Iw ),
Instr(Mnemonic.XCHW, AX, DE),
Instr(Mnemonic.MOVW, HL,Iw ),
Instr(Mnemonic.XCHW, AX,HL),

Instr(Mnemonic.MOV, Mword_c_b,Ib ),
Instr(Mnemonic.MOV, Mword_bc_b,Ib ),
Instr(Mnemonic.SUBC, saddr,Ib ),
Instr(Mnemonic.SUBC, A, saddr),


Instr(Mnemonic.SUBC, A,Ib ),
Instr(Mnemonic.SUBC, A,M_hl_b),
Instr(Mnemonic.SUBC, A,M_hl_off_b),
Instr(Mnemonic.SUBC, A,Maddr16b),

// 0x40
Instr(Mnemonic.CMP, Maddr16b,Ib ),
Instr(Mnemonic.MOV, ES,Ib ),
Instr(Mnemonic.CMPW, AX,Maddr16w),
Instr(Mnemonic.CMPW, AX,BC),

Instr(Mnemonic.CMPW, AX,Iw ),
Instr(Mnemonic.CMPW, AX, DE),
Instr(Mnemonic.CMPW, AX, saddrp),
Instr(Mnemonic.CMPW, AX, HL),


Instr(Mnemonic.MOV, Mword_bc_b, A),
Instr(Mnemonic.MOV, A, Mword_bc_b),
Instr(Mnemonic.CMP, saddr,Ib ),
Instr(Mnemonic.CMP, A,saddr),

Instr(Mnemonic.CMP, A,Ib ),
Instr(Mnemonic.CMP, A,M_hl_b),
Instr(Mnemonic.CMP, A,M_hl_off_b),
Instr(Mnemonic.CMP, A,Maddr16b),

// 0x50
Instr(Mnemonic.MOV, X,Ib ),
Instr(Mnemonic.MOV, A,Ib ),
Instr(Mnemonic.MOV, C,Ib ),
Instr(Mnemonic.MOV, B,Ib ),

Instr(Mnemonic.MOV, E,Ib ),
Instr(Mnemonic.MOV, D,Ib ),
Instr(Mnemonic.MOV, L,Ib ),
Instr(Mnemonic.MOV, H,Ib ),

Instr(Mnemonic.MOVW, Mword_b_w,AX),
Instr(Mnemonic.MOVW, AX,Mword_b_w),
Instr(Mnemonic.AND, saddr,Ib ),
Instr(Mnemonic.AND, A, saddr),


Instr(Mnemonic.AND, A,Ib ),
Instr(Mnemonic.AND, A,M_hl_b),
Instr(Mnemonic.AND, A,M_hl_off_b),
Instr(Mnemonic.AND, A,Maddr16b),

// 0x60
Instr(Mnemonic.MOV, A,X),
Nyi("2nd MAP"),
Instr(Mnemonic.MOV, A, C),
Instr(Mnemonic.MOV, A, B),


Instr(Mnemonic.MOV, A, E),
Instr(Mnemonic.MOV, A, D),
Instr(Mnemonic.MOV, A, L),
Instr(Mnemonic.MOV, A, H),


Instr(Mnemonic.MOVW, Mword_c_w, AX),
Instr(Mnemonic.MOVW, AX, Mword_c_w),
Instr(Mnemonic.OR, saddr,Ib ),
Instr(Mnemonic.OR, A,saddr),

Instr(Mnemonic.OR, A,Ib ),
Instr(Mnemonic.OR, A,M_hl_b),
Instr(Mnemonic.OR, A,M_hl_off_b),
Instr(Mnemonic.OR, A,Maddr16b),

// 0x70
Instr(Mnemonic.MOV, X,A),
Nyi("3rd MAP"),
Instr(Mnemonic.MOV, C, A),
Instr(Mnemonic.MOV, B, A),


Instr(Mnemonic.MOV, E, A),
Instr(Mnemonic.MOV, D, A),
Instr(Mnemonic.MOV, L, A),
Instr(Mnemonic.MOV, H, A),


Instr(Mnemonic.MOVW, Mword_bc_w, AX),
Instr(Mnemonic.MOVW, AX, Mword_bc_w),
Instr(Mnemonic.XOR, saddr,Ib ),
Instr(Mnemonic.XOR, A,saddr),

Instr(Mnemonic.XOR, A,Ib ),
Instr(Mnemonic.XOR, A,M_hl_b),
Instr(Mnemonic.XOR, A,M_hl_off_b),
Instr(Mnemonic.XOR, A,Maddr16b),

// 0x80
Instr(Mnemonic.INC, X),
Instr(Mnemonic.INC, A),
Instr(Mnemonic.INC, C),
Instr(Mnemonic.INC, B),

Instr(Mnemonic.INC, E),
Instr(Mnemonic.INC, D),
Instr(Mnemonic.INC, L),
Instr(Mnemonic.INC, H),

Instr(Mnemonic.MOV, A,M_sp_off_b ),
Instr(Mnemonic.MOV, A,M_de_b),
Instr(Mnemonic.MOV, A,M_de_off_b),
Instr(Mnemonic.MOV, A,M_hl_b),


Instr(Mnemonic.MOV, A,M_hl_off_b),
Instr(Mnemonic.MOV, A, saddr),
Instr(Mnemonic.MOV, A, sfr),
Instr(Mnemonic.MOV, A,Maddr16b),

// 0x90 
Instr(Mnemonic.DEC, X),
Instr(Mnemonic.DEC, A),
Instr(Mnemonic.DEC, C),
Instr(Mnemonic.DEC, B),

Instr(Mnemonic.DEC, E),
Instr(Mnemonic.DEC, D),
Instr(Mnemonic.DEC, L),
Instr(Mnemonic.DEC, H),

Instr(Mnemonic.MOV,M_sp_off_b, A),
Instr(Mnemonic.MOV,M_de_b,A),
Instr(Mnemonic.MOV,M_de_off_b, A),
Instr(Mnemonic.MOV,M_hl_b,A),

Instr(Mnemonic.MOV,M_hl_off_b, A),
Instr(Mnemonic.MOV, saddr, A),
Instr(Mnemonic.MOV, sfr, A),
Instr(Mnemonic.MOV, Maddr16b,A),

// 0xInstr(Mnemonic.A,0),
Instr(Mnemonic.INC, Maddr16b),
Instr(Mnemonic.INCW, AX),
Instr(Mnemonic.INCW, Maddr16w),
Instr(Mnemonic.INCW, BC),

Instr(Mnemonic.INC, saddr),
Instr(Mnemonic.INCW, DE),
Instr(Mnemonic.INCW, saddrp),
Instr(Mnemonic.INCW, HL),

Instr(Mnemonic.MOVW, AX,M_sp_off_w ),
Instr(Mnemonic.MOVW, AX,M_de_w),
Instr(Mnemonic.MOVW, AX,M_de_off_w),
Instr(Mnemonic.MOVW, AX,M_hl_w),


Instr(Mnemonic.MOVW, AX,M_hl_off_w),
Instr(Mnemonic.MOVW, AX, saddrp),
Instr(Mnemonic.MOVW, AX, sfrp),
Instr(Mnemonic.MOVW, AX,Maddr16w),

// 0xInstr(B0),
Instr(Mnemonic.DEC, Maddr16b),
Instr(Mnemonic.DECW, AX),
Instr(Mnemonic.DECW, Maddr16w),
Instr(Mnemonic.DECW, BC),

Instr(Mnemonic.DEC, saddr),
Instr(Mnemonic.DECW, DE),
Instr(Mnemonic.DECW, saddrp),
Instr(Mnemonic.DECW, HL),

Instr(Mnemonic.MOVW,M_sp_off_w, AX),
Instr(Mnemonic.MOVW,M_de_w,AX),
Instr(Mnemonic.MOVW,M_de_off_b, AX),
Instr(Mnemonic.MOVW,M_hl_w,AX),

Instr(Mnemonic.MOVW,M_hl_off_w, AX),
Instr(Mnemonic.MOVW, saddrp, AX),
Instr(Mnemonic.MOVW, sfrp, AX),
Instr(Mnemonic.MOVW, Maddr16w,AX),

// 0xInstr(C0),
Instr(Mnemonic.POP, AX),
Instr(Mnemonic.PUSH, AX),
Instr(Mnemonic.POP, BC),
Instr(Mnemonic.PUSH, BC),

Instr(Mnemonic.POP, DE),
Instr(Mnemonic.PUSH, DE),
Instr(Mnemonic.POP, HL),
Instr(Mnemonic.PUSH, HL),

Instr(Mnemonic.MOV,M_sp_off_b,Ib ),
Instr(Mnemonic.MOVW, saddrp,Iw ),
Instr(Mnemonic.MOV,M_de_off_b,Ib ),
Instr(Mnemonic.MOVW, sfrp,Iw ),

Instr(Mnemonic.MOV,M_hl_off_b,Ib ),
Instr(Mnemonic.MOV, saddr,Ib ),
Instr(Mnemonic.MOV, sfr,Ib ),
Instr(Mnemonic.MOV, Maddr16b,Ib ),

// 0xInstr(D0),
Instr(Mnemonic.CMP0, X),
Instr(Mnemonic.CMP0, A),
Instr(Mnemonic.CMP0, C),
Instr(Mnemonic.CMP0, B),


Instr(Mnemonic.CMP0, saddr),
Instr(Mnemonic.CMP0, Maddr16b),
Instr(Mnemonic.MULU, X),
Instr(Mnemonic.RET),


Instr(Mnemonic.MOV, X, saddr),
Instr(Mnemonic.MOV, X,Maddr16b),
Instr(Mnemonic.MOVW, BC,saddrp),
Instr(Mnemonic.MOVW, BC,Maddr16b),

Instr(Mnemonic.BC, PcRelative8),
Instr(Mnemonic.BZ, PcRelative8),
Instr(Mnemonic.BNC, PcRelative8),
Instr(Mnemonic.BNZ, PcRelative8),

// 0xInstr(E0),
Instr(Mnemonic.ONEB, X),
Instr(Mnemonic.ONEB, A),
Instr(Mnemonic.ONEB, C),
Instr(Mnemonic.ONEB, B),

Instr(Mnemonic.ONEB, saddr),
Instr(Mnemonic.ONEB, Maddr16b),
Instr(Mnemonic.ONEW, AX),
Instr(Mnemonic.ONEW, BC),

Instr(Mnemonic.MOV, B,saddr),
Instr(Mnemonic.MOV, B,Maddr16b),
Instr(Mnemonic.MOVW, DE,saddrp),
Instr(Mnemonic.MOVW, DE,Maddr16w),

Instr(Mnemonic.BR, ImmAddr20),
Instr(Mnemonic.BR, ImmAddr16),
Instr(Mnemonic.BR, PcRelative16),
Instr(Mnemonic.BR, PcRelative8),

// 0xInstr(F0),
Instr(Mnemonic.CLRB, X),
Instr(Mnemonic.CLRB, A),
Instr(Mnemonic.CLRB, C),
Instr(Mnemonic.CLRB, B),

Instr(Mnemonic.CLRB, saddr),
Instr(Mnemonic.CLRB, Maddr16b),
Instr(Mnemonic.CLRW, AX),
Instr(Mnemonic.CLRW, BC),

Instr(Mnemonic.MOV, C,saddr),
Instr(Mnemonic.MOV, C,Maddr16b),
Instr(Mnemonic.MOVW, HL,saddrp),
Instr(Mnemonic.MOVW, HL,Maddr16w),

Instr(Mnemonic.CALL, ImmAddr20),
Instr(Mnemonic.CALL, Maddr16w),
Instr(Mnemonic.CALL, PcRelative16),
s_invalid
            };
        }
    }
}
