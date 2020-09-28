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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Arch.Rl78
{
    using Decoder = Decoder<Rl78Disassembler, Mnemonic, Rl78Instruction>;

    public class Rl78Disassembler : DisassemblerBase<Rl78Instruction, Mnemonic>
    {
        private static readonly Decoder[] s_decoders;
        private static readonly Decoder[] s_ext2;
        private static readonly Decoder[] s_ext3;
        private static readonly Decoder[] s_ext4;
        private static readonly Decoder s_invalid;

        private readonly Rl78Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private RegisterStorage prefix;

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
            prefix = null;
            var instr = s_decoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Rl78Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Rl78Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Prefix = this.prefix,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override Rl78Instruction CreateInvalidInstruction()
        {
            return new Rl78Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
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


        private static Mutator<Rl78Disassembler> Flag(FlagGroupStorage grf)
        {
            return (u, d) =>
            {
                d.ops.Add(new FlagGroupOperand(grf));
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

        private readonly static Mutator<Rl78Disassembler> PSW = Reg(Registers.psw);

        private readonly static Mutator<Rl78Disassembler> CY = Flag(Registers.cy);


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
        private static readonly Mutator<Rl78Disassembler> M_de_b = Mem(PrimitiveType.Byte, Registers.de);
        private static readonly Mutator<Rl78Disassembler> M_de_w = Mem(PrimitiveType.Word16, Registers.de);
        private static readonly Mutator<Rl78Disassembler> M_hl_b = Mem(PrimitiveType.Byte, Registers.hl);
        private static readonly Mutator<Rl78Disassembler> M_hl_w = Mem(PrimitiveType.Word16, Registers.hl);


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
        private static readonly Mutator<Rl78Disassembler> M_de_off_w = MemOff(PrimitiveType.Word16, Registers.de);

        private static readonly Mutator<Rl78Disassembler> M_hl_off_b = MemOff(PrimitiveType.Byte, Registers.hl);
        private static readonly Mutator<Rl78Disassembler> M_hl_off_w = MemOff(PrimitiveType.Word16, Registers.hl);

        private static readonly Mutator<Rl78Disassembler> M_sp_off_b = MemOff(PrimitiveType.Byte, Registers.sp);
        private static readonly Mutator<Rl78Disassembler> M_sp_off_w = MemOff(PrimitiveType.Word16, Registers.sp);


        private static Mutator<Rl78Disassembler> M_idx(PrimitiveType dt, RegisterStorage baseReg, RegisterStorage idx)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Index = idx
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> M_hl_c_b = M_idx(PrimitiveType.Byte, Registers.hl, Registers.c);
        private static readonly Mutator<Rl78Disassembler> M_hl_b_b = M_idx(PrimitiveType.Byte, Registers.hl, Registers.b);

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

        private static Mutator<Rl78Disassembler> Mabs(ushort uAddr)
        {
            var mem = new MemoryOperand(PrimitiveType.Word16)
            {
                Offset = uAddr
            };
            return (u, d) =>
            {
                d.ops.Add(mem);
                return true;
            };
        }

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

        // Modify previous operand to refer to a specific bit.
        private static Mutator<Rl78Disassembler> Bit(int nBit)
        {
            return (u, d) =>
            {
                var iOp = d.ops.Count - 1;
                var bit = new BitOperand(d.ops[iOp], nBit);
                d.ops[iOp] = bit;
                return true;
            };
        }

        private static Mutator<Rl78Disassembler> n(int value)
        {
            return (u, d) =>
            {
                d.ops.Add(ImmediateOperand.Byte((byte) value));
                return true;
            };
        }

        private static Mutator<Rl78Disassembler> RegisterBank(int n)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterBankOperand(n));
                return true;
            };
        }
        private static readonly Mutator<Rl78Disassembler> RB0 = RegisterBank(0);
        private static readonly Mutator<Rl78Disassembler> RB1 = RegisterBank(1);
        private static readonly Mutator<Rl78Disassembler> RB2 = RegisterBank(2);
        private static readonly Mutator<Rl78Disassembler> RB3 = RegisterBank(3);

        #endregion

        private class TwoByteDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public TwoByteDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte op))
                    return dasm.CreateInvalidInstruction();
                return decoders[op].Decode(op, dasm);
            }
        }

        private class PrefixDecoder : Decoder
        {
            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte op))
                    return dasm.CreateInvalidInstruction();
                dasm.prefix = Registers.es;
                return s_decoders[op].Decode(op, dasm);
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
                return dasm.CreateInvalidInstruction();
            }
        }

        private static Decoder Instr(Mnemonic mnem, InstrClass iclass, params Mutator<Rl78Disassembler>[] mutators)
        {
            return new InstrDecoder<Rl78Disassembler,Mnemonic,Rl78Instruction>(iclass, mnem, mutators);
        }

        private static Decoder Instr(Mnemonic mnem, params Mutator<Rl78Disassembler>[] mutators)
        {
            return new InstrDecoder<Rl78Disassembler, Mnemonic, Rl78Instruction>(InstrClass.Linear, mnem, mutators);
        }

        private static Decoder TwoByte(Decoder[] decoders)
        {
            return new TwoByteDecoder(decoders);
        }


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        static Rl78Disassembler()
        {
            s_invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            s_ext2 = new Decoder[256] {
                Instr(Mnemonic.add, X,A),
                Instr(Mnemonic.add, A,A),
                Instr(Mnemonic.add, C,A),
                Instr(Mnemonic.add, B,A),

                Instr(Mnemonic.add, E,A),
                Instr(Mnemonic.add, D,A),
                Instr(Mnemonic.add, L,A),
                Instr(Mnemonic.add, H,A),

                Instr(Mnemonic.add, A,X),
                Instr(Mnemonic.addw, AX,M_hl_off_b),
                Instr(Mnemonic.add, A,C),
                Instr(Mnemonic.add, A,B),

                Instr(Mnemonic.add, A,E),
                Instr(Mnemonic.add, A,D),
                Instr(Mnemonic.add, A,L),
                Instr(Mnemonic.add, A,H), 
                // 0x01
                Instr(Mnemonic.addc, X,A),
                Instr(Mnemonic.addc, A,A),
                Instr(Mnemonic.addc, C,A),
                Instr(Mnemonic.addc, B,A),

                Instr(Mnemonic.addc, E,A),
                Instr(Mnemonic.addc, D,A),
                Instr(Mnemonic.addc, L,A),
                Instr(Mnemonic.addc, H,A),

                Instr(Mnemonic.addc, A,X),
                s_invalid,
                Instr(Mnemonic.addc, A,C),
                Instr(Mnemonic.addc, A,B),

                Instr(Mnemonic.addc, A,E),
                Instr(Mnemonic.addc, A,D),
                Instr(Mnemonic.addc, A,L),
                Instr(Mnemonic.addc, A,H),
                // 0x20
                Instr(Mnemonic.sub, X,A),
                Instr(Mnemonic.sub, A,A),
                Instr(Mnemonic.sub, C,A),
                Instr(Mnemonic.sub, B,A),

                Instr(Mnemonic.sub, E,A),
                Instr(Mnemonic.sub, D,A),
                Instr(Mnemonic.sub, L,A),
                Instr(Mnemonic.sub, H,A),

                Instr(Mnemonic.sub, A,X),
                Instr(Mnemonic.subw, AX,M_hl_off_w),
                Instr(Mnemonic.sub, A,C),
                Instr(Mnemonic.sub, A,B),

                Instr(Mnemonic.sub, A,E),
                Instr(Mnemonic.sub, A,D),
                Instr(Mnemonic.sub, A,L),
                Instr(Mnemonic.sub, A,H),
                // 0x30
                Instr(Mnemonic.subc, X,A),
                Instr(Mnemonic.subc, A,A),
                Instr(Mnemonic.subc, C,A),
                Instr(Mnemonic.subc, B,A),

                Instr(Mnemonic.subc, E,A),
                Instr(Mnemonic.subc, D,A),
                Instr(Mnemonic.subc, L,A),
                Instr(Mnemonic.subc, H,A),

                Instr(Mnemonic.subc, A,X),
                s_invalid,
                Instr(Mnemonic.subc, A,C),
                Instr(Mnemonic.subc, A,B),

                Instr(Mnemonic.subc, A,E),
                Instr(Mnemonic.subc, A,D),
                Instr(Mnemonic.subc, A,L),
                Instr(Mnemonic.subc, A,H),
                
                // 0x40
                Instr(Mnemonic.cmp, X,A),
                Instr(Mnemonic.cmp, A,A),
                Instr(Mnemonic.cmp, C,A),
                Instr(Mnemonic.cmp, B,A),

                Instr(Mnemonic.cmp, E,A),
                Instr(Mnemonic.cmp, D,A),
                Instr(Mnemonic.cmp, L,A),
                Instr(Mnemonic.cmp, H,A),

                Instr(Mnemonic.cmp, A,X),
                Instr(Mnemonic.cmpw, AX,M_hl_off_w),
                Instr(Mnemonic.cmp, A,C),
                Instr(Mnemonic.cmp, A,B),

                Instr(Mnemonic.cmp, A,E),
                Instr(Mnemonic.cmp, A,D),
                Instr(Mnemonic.cmp, A,L),
                Instr(Mnemonic.cmp, A,H),
                // 0x50
                Instr(Mnemonic.and, X,A),
                Instr(Mnemonic.and, A,A),
                Instr(Mnemonic.and, C,A),
                Instr(Mnemonic.and, B,A),

                Instr(Mnemonic.and, E,A),
                Instr(Mnemonic.and, D,A),
                Instr(Mnemonic.and, L,A),
                Instr(Mnemonic.and, H,A),

                Instr(Mnemonic.and, A,X),
                Instr(Mnemonic.inc, M_hl_off_b),
                Instr(Mnemonic.and, A,C),
                Instr(Mnemonic.and, A,B),

                Instr(Mnemonic.and, A,E),
                Instr(Mnemonic.and, A,D),
                Instr(Mnemonic.and, A,L),
                Instr(Mnemonic.and, A,H),
                // 0x60
                Instr(Mnemonic.or, X,A),
                Instr(Mnemonic.or, A,A),
                Instr(Mnemonic.or, C,A),
                Instr(Mnemonic.or, B,A),

                Instr(Mnemonic.or, E,A),
                Instr(Mnemonic.or, D,A),
                Instr(Mnemonic.or, L,A),
                Instr(Mnemonic.or, H,A),

                Instr(Mnemonic.or, A,X),
                Instr(Mnemonic.dec, M_hl_off_b),
                Instr(Mnemonic.or, A,C),
                Instr(Mnemonic.or, A,B),

                Instr(Mnemonic.or, A,E),
                Instr(Mnemonic.or, A,D),
                Instr(Mnemonic.or, A,L),
                Instr(Mnemonic.or, A,H),
                // 0x70
                Instr(Mnemonic.xor, X,A),
                Instr(Mnemonic.xor, A,A),
                Instr(Mnemonic.xor, C,A),
                Instr(Mnemonic.xor, B,A),

                Instr(Mnemonic.xor, E,A),
                Instr(Mnemonic.xor, D,A),
                Instr(Mnemonic.xor, L,A),
                Instr(Mnemonic.xor, H,A),

                Instr(Mnemonic.xor, A,X),
                Instr(Mnemonic.incw, M_hl_off_w),
                Instr(Mnemonic.xor, A,C),
                Instr(Mnemonic.xor, A,B),

                Instr(Mnemonic.xor, A,E),
                Instr(Mnemonic.xor, A,D),
                Instr(Mnemonic.xor, A,L),
                Instr(Mnemonic.xor, A,H),

                // 0x80
                Instr(Mnemonic.add, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.add, A,M_hl_c_b),
                s_invalid,

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0080)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0090)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00A0)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00B0)),

                s_invalid,
                Instr(Mnemonic.decw, M_hl_off_w),
                Instr(Mnemonic.xch, A,C),
                Instr(Mnemonic.xch, A,B),

                Instr(Mnemonic.xch, A,E),
                Instr(Mnemonic.xch, A,D),
                Instr(Mnemonic.xch, A,L),
                Instr(Mnemonic.xch, A,H),
                
                // 0x90
                Instr(Mnemonic.addc, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.addc, A,M_hl_c_b),
                s_invalid,

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0082)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0092)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00A2)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00B2)),

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 0xA0,
                Instr(Mnemonic.sub, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.sub, A,M_hl_c_b),
                s_invalid,

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0084)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0094)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00A4)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00B4)),

                Instr(Mnemonic.xch, A,saddr),
                Instr(Mnemonic.xch, A,M_hl_c_b),
                Instr(Mnemonic.xch, A,Maddr16b),
                Instr(Mnemonic.xch, A,sfr),

                Instr(Mnemonic.xch, A,M_hl_b),
                Instr(Mnemonic.xch, A,M_hl_off_b),
                Instr(Mnemonic.xch, A,M_de_b),
                Instr(Mnemonic.xch, A,M_de_off_b),

                //  0xB0
                Instr(Mnemonic.subc, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.subc, A,M_hl_c_b),
                s_invalid,

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0086)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0096)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00A6)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00B6)),

                Instr(Mnemonic.mov, ES,saddr),
                Instr(Mnemonic.xch, A,M_hl_b_b),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 0xC0
                Instr(Mnemonic.cmp, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.cmp, A,M_hl_c_b),
                Instr(Mnemonic.bh, InstrClass.ConditionalTransfer, PcRelative8),

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0088)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x0098)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00A8)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00B8)),

                Instr(Mnemonic.skc, InstrClass.ConditionalTransfer),
                Instr(Mnemonic.mov, A,M_hl_b_b),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, AX),
                Instr(Mnemonic.br, InstrClass.Transfer, AX),

                Instr(Mnemonic.brk),
                Instr(Mnemonic.pop, PSW),
                Instr(Mnemonic.movs, M_hl_off_b,X),
                Instr(Mnemonic.sel, RB0),

                // 0xD0
                Instr(Mnemonic.and, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.and, A,M_hl_c_b),
                Instr(Mnemonic.bnh, InstrClass.ConditionalTransfer, PcRelative8),

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x008A)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x009A)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00AA)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00BA)),

                Instr(Mnemonic.sknc, InstrClass.ConditionalTransfer),
                Instr(Mnemonic.mov, M_hl_b_b,A),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, BC),
                Instr(Mnemonic.ror, A,n(1)),

                Instr(Mnemonic.rolc, A,n(1)),
                Instr(Mnemonic.push, PSW),
                Instr(Mnemonic.cmps, X,M_hl_off_b),
                Instr(Mnemonic.sel, RB1), // Note
    
                // 0xE0
                Instr(Mnemonic.or, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.or, A,M_hl_c_b),
                Instr(Mnemonic.skh, InstrClass.ConditionalTransfer),

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x008C)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x009C)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00AC)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00BC)),

                Instr(Mnemonic.skz, InstrClass.ConditionalTransfer),
                Instr(Mnemonic.mov, A,M_hl_c_b),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, DE),
                Instr(Mnemonic.rol, A,n(1)),

                Instr(Mnemonic.retb, InstrClass.Transfer),
                Instr(Mnemonic.halt, InstrClass.Terminates),
                Instr(Mnemonic.rolwc, AX,n(1)),
                Instr(Mnemonic.sel, RB2), // Note
                // 0xF0
                Instr(Mnemonic.xor, A,M_hl_b_b),
                s_invalid,
                Instr(Mnemonic.xor, A,M_hl_c_b),
                Instr(Mnemonic.sknh, InstrClass.ConditionalTransfer),

                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x008E)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x009E)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00AE)),
                Instr(Mnemonic.callt, InstrClass.Transfer|InstrClass.Call, Mabs(0x00BE)),

                Instr(Mnemonic.sknz, InstrClass.ConditionalTransfer),
                Instr(Mnemonic.mov, M_hl_c_b,A),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, HL),
                Instr(Mnemonic.rorc, A,n(1)),

                Instr(Mnemonic.reti, InstrClass.Transfer),
                Instr(Mnemonic.stop, InstrClass.Terminates),
                Instr(Mnemonic.rolwc, BC,n(1)),
                Instr(Mnemonic.sel, RB3)
             };

            s_ext3 = new Decoder[256] {
                // 0x00
                Instr(Mnemonic.set1, Maddr16b,Bit(0)),
                Instr(Mnemonic.mov1, saddr,Bit(0),CY),
                Instr(Mnemonic.set1, saddr,Bit(0)),
                Instr(Mnemonic.clr1, saddr,Bit(0)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(0)),
                Instr(Mnemonic.and1, CY,saddr,Bit(0)),
                Instr(Mnemonic.or1, CY,saddr,Bit(0)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(0)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(0)),
                Instr(Mnemonic.mov1, sfr,Bit(0),CY),
                Instr(Mnemonic.set1, sfr,Bit(0)),
                Instr(Mnemonic.clr1, sfr,Bit(0)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(0)),
                Instr(Mnemonic.and1, CY,sfr,Bit(0)),
                Instr(Mnemonic.or1, CY,sfr,Bit(0)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(0)),
                
                // 0x10
                Instr(Mnemonic.set1, Maddr16b,Bit(1)),
                Instr(Mnemonic.mov1, saddr,Bit(1),CY),
                Instr(Mnemonic.set1, saddr,Bit(1)),
                Instr(Mnemonic.clr1, saddr,Bit(1)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(1)),
                Instr(Mnemonic.and1, CY,saddr,Bit(1)),
                Instr(Mnemonic.or1, CY,saddr,Bit(1)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(1)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(1)),
                Instr(Mnemonic.mov1, sfr,Bit(1),CY),
                Instr(Mnemonic.set1, sfr,Bit(1)),
                Instr(Mnemonic.clr1, sfr,Bit(1)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(1)),
                Instr(Mnemonic.and1, CY,sfr,Bit(1)),
                Instr(Mnemonic.or1, CY,sfr,Bit(1)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(1)),
    
                // 0x20
                Instr(Mnemonic.set1, Maddr16b,Bit(2)),
                Instr(Mnemonic.mov1, saddr,Bit(2),CY),
                Instr(Mnemonic.set1, saddr,Bit(2)),
                Instr(Mnemonic.clr1, saddr,Bit(2)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(2)),
                Instr(Mnemonic.and1, CY,saddr,Bit(2)),
                Instr(Mnemonic.or1, CY,saddr,Bit(2)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(2)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(2)),
                Instr(Mnemonic.mov1, sfr,Bit(2),CY),
                Instr(Mnemonic.set1, sfr,Bit(2)),
                Instr(Mnemonic.clr1, sfr,Bit(2)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(2)),
                Instr(Mnemonic.and1, CY,sfr,Bit(2)),
                Instr(Mnemonic.or1, CY,sfr,Bit(2)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(2)),
                
                // 0x30
                Instr(Mnemonic.set1, Maddr16b,Bit(3)),
                Instr(Mnemonic.mov1, saddr,Bit(3),CY),
                Instr(Mnemonic.set1, saddr,Bit(3)),
                Instr(Mnemonic.clr1, saddr,Bit(3)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(3)),
                Instr(Mnemonic.and1, CY,saddr,Bit(3)),
                Instr(Mnemonic.or1, CY,saddr,Bit(3)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(3)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(3)),
                Instr(Mnemonic.mov1, sfr,Bit(3),CY),
                Instr(Mnemonic.set1, sfr,Bit(3)),
                Instr(Mnemonic.clr1, sfr,Bit(3)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(3)),
                Instr(Mnemonic.and1, CY,sfr,Bit(3)),
                Instr(Mnemonic.or1, CY,sfr,Bit(3)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(3)),
                
                // 0x40
                Instr(Mnemonic.set1, Maddr16b,Bit(4)),
                Instr(Mnemonic.mov1, saddr,Bit(4),CY),
                Instr(Mnemonic.set1, saddr,Bit(4)),
                Instr(Mnemonic.clr1, saddr,Bit(4)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(4)),
                Instr(Mnemonic.and1, CY,saddr,Bit(4)),
                Instr(Mnemonic.or1, CY,saddr,Bit(4)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(4)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(4)),
                Instr(Mnemonic.mov1, sfr,Bit(4),CY),
                Instr(Mnemonic.set1, sfr,Bit(4)),
                Instr(Mnemonic.clr1, sfr,Bit(4)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(4)),
                Instr(Mnemonic.and1, CY,sfr,Bit(4)),
                Instr(Mnemonic.or1, CY,sfr,Bit(4)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(4)),
    
                // 0x50
                Instr(Mnemonic.set1, Maddr16b,Bit(5)),
                Instr(Mnemonic.mov1, saddr,Bit(5),CY),
                Instr(Mnemonic.set1, saddr,Bit(5)),
                Instr(Mnemonic.clr1, saddr,Bit(5)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(5)),
                Instr(Mnemonic.and1, CY,saddr,Bit(5)),
                Instr(Mnemonic.or1, CY,saddr,Bit(5)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(5)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(5)),
                Instr(Mnemonic.mov1, sfr,Bit(5),CY),
                Instr(Mnemonic.set1, sfr,Bit(5)),
                Instr(Mnemonic.clr1, sfr,Bit(5)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(5)),
                Instr(Mnemonic.and1, CY,sfr,Bit(5)),
                Instr(Mnemonic.or1, CY,sfr,Bit(5)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(5)),
    
                // 0x60
                Instr(Mnemonic.set1, Maddr16b,Bit(6)),
                Instr(Mnemonic.mov1, saddr,Bit(6),CY),
                Instr(Mnemonic.set1, saddr,Bit(6)),
                Instr(Mnemonic.clr1, saddr,Bit(6)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(6)),
                Instr(Mnemonic.and1, CY,saddr,Bit(6)),
                Instr(Mnemonic.or1, CY,saddr,Bit(6)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(6)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(6)),
                Instr(Mnemonic.mov1, sfr,Bit(6),CY),
                Instr(Mnemonic.set1, sfr,Bit(6)),
                Instr(Mnemonic.clr1, sfr,Bit(6)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(6)),
                Instr(Mnemonic.and1, CY,sfr,Bit(6)),
                Instr(Mnemonic.or1, CY,sfr,Bit(6)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(6)),
    
                // 0x70
                Instr(Mnemonic.set1, Maddr16b,Bit(7)),
                Instr(Mnemonic.mov1, saddr,Bit(7),CY),
                Instr(Mnemonic.set1, saddr,Bit(7)),
                Instr(Mnemonic.clr1, saddr,Bit(7)),

                Instr(Mnemonic.mov1, CY,saddr,Bit(7)),
                Instr(Mnemonic.and1, CY,saddr,Bit(7)),
                Instr(Mnemonic.or1, CY,saddr,Bit(7)),
                Instr(Mnemonic.xor1, CY,saddr,Bit(7)),

                Instr(Mnemonic.clr1, Maddr16b,Bit(7)),
                Instr(Mnemonic.mov1, sfr,Bit(7),CY),
                Instr(Mnemonic.set1, sfr,Bit(7)),
                Instr(Mnemonic.clr1, sfr,Bit(7)),

                Instr(Mnemonic.mov1, CY,sfr,Bit(7)),
                Instr(Mnemonic.and1, CY,sfr,Bit(7)),
                Instr(Mnemonic.or1, CY,sfr,Bit(7)),
                Instr(Mnemonic.xor1, CY,sfr,Bit(7)),
    
                // 0x80
                Instr(Mnemonic.set1, CY),
                Instr(Mnemonic.mov1, M_hl_b,Bit(0),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(0)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(0)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(0)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(0)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(0)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(0)),

                Instr(Mnemonic.clr1, CY),
                Instr(Mnemonic.mov1, A,Bit(0),CY),
                Instr(Mnemonic.set1, A,Bit(0)),
                Instr(Mnemonic.clr1, A,Bit(0)),

                Instr(Mnemonic.mov1, CY,A,Bit(0)),
                Instr(Mnemonic.and1, CY,A,Bit(0)),
                Instr(Mnemonic.or1, CY,A,Bit(0)),
                Instr(Mnemonic.xor1, CY,A,Bit(0)),
   
                // 0x90
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(1),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(1)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(1)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(1)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(1)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(1)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(1)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(1),CY),
                Instr(Mnemonic.set1, A,Bit(1)),
                Instr(Mnemonic.clr1, A,Bit(1)),

                Instr(Mnemonic.mov1, CY,A,Bit(1)),
                Instr(Mnemonic.and1, CY,A,Bit(1)),
                Instr(Mnemonic.or1, CY,A,Bit(1)),
                Instr(Mnemonic.xor1, CY,A,Bit(1)),
    
                // 0xA0
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(2),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(2)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(2)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(2)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(2)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(2)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(2)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(2),CY),
                Instr(Mnemonic.set1, A,Bit(2)),
                Instr(Mnemonic.clr1, A,Bit(2)),

                Instr(Mnemonic.mov1, CY,A,Bit(2)),
                Instr(Mnemonic.and1, CY,A,Bit(2)),
                Instr(Mnemonic.or1, CY,A,Bit(2)),
                Instr(Mnemonic.xor1, CY,A,Bit(2)),
    
                // 0xB0
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(3),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(3)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(3)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(3)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(3)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(3)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(3)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(3),CY),
                Instr(Mnemonic.set1, A,Bit(3)),
                Instr(Mnemonic.clr1, A,Bit(3)),

                Instr(Mnemonic.mov1, CY,A,Bit(3)),
                Instr(Mnemonic.and1, CY,A,Bit(3)),
                Instr(Mnemonic.or1, CY,A,Bit(3)),
                Instr(Mnemonic.xor1, CY,A,Bit(3)),
    
                // 0xC0
                Instr(Mnemonic.not1, CY),
                Instr(Mnemonic.mov1, M_hl_b,Bit(4),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(4)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(4)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(4)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(4)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(4)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(4)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(4),CY),
                Instr(Mnemonic.set1, A,Bit(4)),
                Instr(Mnemonic.clr1, A,Bit(4)),

                Instr(Mnemonic.mov1, CY,A,Bit(4)),
                Instr(Mnemonic.and1, CY,A,Bit(4)),
                Instr(Mnemonic.or1, CY,A,Bit(4)),
                Instr(Mnemonic.xor1, CY,A,Bit(4)),

                // 0xD0
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(5),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(5)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(5)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(5)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(5)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(5)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(5)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(5),CY),
                Instr(Mnemonic.set1, A,Bit(5)),
                Instr(Mnemonic.clr1, A,Bit(5)),

                Instr(Mnemonic.mov1, CY,A,Bit(5)),
                Instr(Mnemonic.and1, CY,A,Bit(5)),
                Instr(Mnemonic.or1, CY,A,Bit(5)),
                Instr(Mnemonic.xor1, CY,A,Bit(5)),
    
                // 0xE0
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(6),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(6)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(6)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(6)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(6)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(6)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(6)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(6),CY),
                Instr(Mnemonic.set1, A,Bit(6)),
                Instr(Mnemonic.clr1, A,Bit(6)),

                Instr(Mnemonic.mov1, CY,A,Bit(6)),
                Instr(Mnemonic.and1, CY,A,Bit(6)),
                Instr(Mnemonic.or1, CY,A,Bit(6)),
                Instr(Mnemonic.xor1, CY,A,Bit(6)),
    
                // 0xF0
                s_invalid,
                Instr(Mnemonic.mov1, M_hl_b,Bit(7),CY),
                Instr(Mnemonic.set1, M_hl_b,Bit(7)),
                Instr(Mnemonic.clr1, M_hl_b,Bit(7)),

                Instr(Mnemonic.mov1, CY,M_hl_b,Bit(7)),
                Instr(Mnemonic.and1, CY,M_hl_b,Bit(7)),
                Instr(Mnemonic.or1, CY,M_hl_b,Bit(7)),
                Instr(Mnemonic.xor1, CY,M_hl_b,Bit(7)),

                s_invalid,
                Instr(Mnemonic.mov1, A,Bit(7),CY),
                Instr(Mnemonic.set1, A,Bit(7)),
                Instr(Mnemonic.clr1, A,Bit(7)),

                Instr(Mnemonic.mov1, CY,A,Bit(7)),
                Instr(Mnemonic.and1, CY,A,Bit(7)),
                Instr(Mnemonic.or1, CY,A,Bit(7)),
                Instr(Mnemonic.xor1, CY,A,Bit(7))
            };

            s_ext4 = new Decoder[256] {
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(0),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(0),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(0),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(0),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(0),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(0),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 0x10
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer,saddr,Bit(1),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(1),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer,  saddr,Bit(1),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(1),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(1),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(1),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(1)),

                Instr(Mnemonic.shl, B,n(1)),
                Instr(Mnemonic.shl, A,n(1)),
                Instr(Mnemonic.shr, A,n(1)),
                Instr(Mnemonic.sar, A,n(1)),

                Instr(Mnemonic.shlw, BC,n(1)),
                Instr(Mnemonic.shlw, AX,n(1)),
                Instr(Mnemonic.shrw, AX,n(1)),
                Instr(Mnemonic.sarw, AX,n(1)),

                // 0x20
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(2),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(2),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(2),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(2),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(2),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(2),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(2)),

                Instr(Mnemonic.shl, B,n(2)),
                Instr(Mnemonic.shl, A,n(2)),
                Instr(Mnemonic.shr, A,n(2)),
                Instr(Mnemonic.sar, A,n(2)),

                Instr(Mnemonic.shlw, BC,n(2)),
                Instr(Mnemonic.shlw, AX,n(2)),
                Instr(Mnemonic.shrw, AX,n(2)),
                Instr(Mnemonic.sarw, AX,n(2)),
    
                // 0x30
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(3),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(3),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(3),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(3),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(3),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(3),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(3)),

                Instr(Mnemonic.shl, B,n(3)),
                Instr(Mnemonic.shl, A,n(3)),
                Instr(Mnemonic.shr, A,n(3)),
                Instr(Mnemonic.sar, A,n(3)),

                Instr(Mnemonic.shlw, BC,n(3)),
                Instr(Mnemonic.shlw, AX,n(3)),
                Instr(Mnemonic.shrw, AX,n(3)),
                Instr(Mnemonic.sarw, AX,n(3)),
    
                // 0x40
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(4),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(4),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(4),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(4),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(4),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(4),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(4)),

                Instr(Mnemonic.shl, B,n(4)),
                Instr(Mnemonic.shl, A,n(4)),
                Instr(Mnemonic.shr, A,n(4)),
                Instr(Mnemonic.sar, A,n(4)),

                Instr(Mnemonic.shlw, BC,n(4)),
                Instr(Mnemonic.shlw, AX,n(4)),
                Instr(Mnemonic.shrw, AX,n(4)),
                Instr(Mnemonic.sarw, AX,n(4)),
    
                // 0x50
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(5),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(5),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(5),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(5),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(5),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(5),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(5)),

                Instr(Mnemonic.shl, B,n(5)),
                Instr(Mnemonic.shl, A,n(5)),
                Instr(Mnemonic.shr, A,n(5)),
                Instr(Mnemonic.sar, A,n(5)),

                Instr(Mnemonic.shlw, BC,n(5)),
                Instr(Mnemonic.shlw, AX,n(5)),
                Instr(Mnemonic.shrw, AX,n(5)),
                Instr(Mnemonic.sarw, AX,n(5)),
                
                // 0x60
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(6),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(6),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(6),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(6),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(6),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(6),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(6)),

                Instr(Mnemonic.shl, B,n(6)),
                Instr(Mnemonic.shl, A,n(6)),
                Instr(Mnemonic.shr, A,n(6)),
                Instr(Mnemonic.sar, A,n(6)),

                Instr(Mnemonic.shlw, BC,n(6)),
                Instr(Mnemonic.shlw, AX,n(6)),
                Instr(Mnemonic.shrw, AX,n(6)),
                Instr(Mnemonic.sarw, AX,n(6)),
                
                // 0x70
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, saddr,Bit(7),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, A,Bit(7),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, saddr,Bit(7),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, A,Bit(7),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, saddr,Bit(7),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, A,Bit(7),PcRelative8),
                s_invalid,
                Instr(Mnemonic.shl, C,n(7)),

                Instr(Mnemonic.shl, B,n(7)),
                Instr(Mnemonic.shl, A,n(7)),
                Instr(Mnemonic.shr, A,n(7)),
                Instr(Mnemonic.sar, A,n(7)),

                Instr(Mnemonic.shlw, BC,n(7)),
                Instr(Mnemonic.shlw, AX,n(7)),
                Instr(Mnemonic.shrw, AX,n(7)),
                Instr(Mnemonic.sarw, AX,n(7)),
                
                // 0x80
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(0),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(0),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(0),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(0),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(0),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(0),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(8)),
                Instr(Mnemonic.shlw, AX,n(8)),
                Instr(Mnemonic.shrw, AX,n(8)),
                Instr(Mnemonic.sarw, AX,n(8)),

                // 0x90
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(1),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(1),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(1),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(1),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(1),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(1),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(9)),
                Instr(Mnemonic.shlw, AX,n(9)),
                Instr(Mnemonic.shrw, AX,n(9)),
                Instr(Mnemonic.sarw, AX,n(9)),

                // 0xA0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(2),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(2),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(2),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(2),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(2),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(2),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(10)),
                Instr(Mnemonic.shlw, AX,n(10)),
                Instr(Mnemonic.shrw, AX,n(10)),
                Instr(Mnemonic.sarw, AX,n(10)),
                
                // 0xB0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(3),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(3),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(3),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(3),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(3),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(3),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(11)),
                Instr(Mnemonic.shlw, AX,n(11)),
                Instr(Mnemonic.shrw, AX,n(11)),
                Instr(Mnemonic.sarw, AX,n(11)),
                
                // 0xC0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(4),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(4),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(4),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(4),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(4),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(4),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(12)),
                Instr(Mnemonic.shlw, AX,n(12)),
                Instr(Mnemonic.shrw, AX,n(12)),
                Instr(Mnemonic.sarw, AX,n(12)),
                
                // 0xD0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(5),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(5),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(5),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(5),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(5),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(5),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(13)),
                Instr(Mnemonic.shlw, AX,n(13)),
                Instr(Mnemonic.shrw, AX,n(13)),
                Instr(Mnemonic.sarw, AX,n(13)),
                
                // 0xE0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(6),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(6),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(6),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(6),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(6),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(6),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(14)),
                Instr(Mnemonic.shlw, AX,n(14)),
                Instr(Mnemonic.shrw, AX,n(14)),
                Instr(Mnemonic.sarw, AX,n(14)),
                
                // 0xF0
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, sfr,Bit(7),PcRelative8),
                Instr(Mnemonic.btclr, InstrClass.ConditionalTransfer, M_hl_b,Bit(7),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, sfr,Bit(7),PcRelative8),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, M_hl_b,Bit(7),PcRelative8),

                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, sfr,Bit(7),PcRelative8),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, M_hl_b,Bit(7),PcRelative8),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                Instr(Mnemonic.shlw, BC,n(15)),
                Instr(Mnemonic.shlw, AX,n(15)),
                Instr(Mnemonic.shrw, AX,n(15)),
                Instr(Mnemonic.sarw, AX,n(15))
            };

            s_decoders = new Decoder[256]
            {
                // 0x00
                Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding | InstrClass.Zero),
                Instr(Mnemonic.addw, AX, AX),
                Instr(Mnemonic.addw, AX,Maddr16w),
                Instr(Mnemonic.addw, AX, BC),

                Instr(Mnemonic.addw, AX,Iw ),
                Instr(Mnemonic.addw, AX,DE),
                Instr(Mnemonic.addw, AX,saddrp),
                Instr(Mnemonic.addw, AX,HL),

                Instr(Mnemonic.xch, A,X),
                Instr(Mnemonic.mov, A,Mword_b_b),
                Instr(Mnemonic.add, saddr,Ib),
                Instr(Mnemonic.add, A,saddr),

                Instr(Mnemonic.add, A,Ib ),
                Instr(Mnemonic.add, A,M_hl_b),
                Instr(Mnemonic.add, A,M_hl_off_b),
                Instr(Mnemonic.add, A,Maddr16b),

                // 0x10
                Instr(Mnemonic.addw, SP,Ib ),
                new PrefixDecoder(),
                Instr(Mnemonic.movw, BC,AX),
                Instr(Mnemonic.movw, AX,BC),

                Instr(Mnemonic.movw, DE,AX),
                Instr(Mnemonic.movw, AX,DE),
                Instr(Mnemonic.movw, HL,AX),
                Instr(Mnemonic.movw, AX,HL),

                Instr(Mnemonic.mov, Mword_b_b,A),
                Instr(Mnemonic.mov, Mword_b_b,Ib ),
                Instr(Mnemonic.addc, saddr,Ib ),
                Instr(Mnemonic.addc, A,saddr),

                Instr(Mnemonic.addc, A,Ib ),
                Instr(Mnemonic.addc, A,M_hl_b),
                Instr(Mnemonic.addc, A,M_hl_off_b),
                Instr(Mnemonic.addc, A,Maddr16b),

                // 0x20
                Instr(Mnemonic.subw, SP,Ib ),
                Nyi("0x21"),
                Instr(Mnemonic.subw, AX,Maddr16w),
                Instr(Mnemonic.subw, AX, BC),


                Instr(Mnemonic.subw, AX,Iw ),
                Instr(Mnemonic.subw, AX,DE),
                Instr(Mnemonic.subw, AX,saddrp),
                Instr(Mnemonic.subw, AX,HL),

                Instr(Mnemonic.mov, Mword_c_b,A),
                Instr(Mnemonic.mov, A,Mword_c_b),
                Instr(Mnemonic.sub, saddr,Ib ),
                Instr(Mnemonic.sub, A,saddr),

                Instr(Mnemonic.sub, A,Ib ),
                Instr(Mnemonic.sub, A,M_hl_b),
                Instr(Mnemonic.sub, A,M_hl_off_b),
                Instr(Mnemonic.sub, A,Maddr16b),

                // 0x30
                Instr(Mnemonic.movw, AX,Iw),
                TwoByte(s_ext4),
                Instr(Mnemonic.movw, BC,Iw ),
                Instr(Mnemonic.xchw, AX,BC),

                Instr(Mnemonic.movw, DE,Iw ),
                Instr(Mnemonic.xchw, AX, DE),
                Instr(Mnemonic.movw, HL,Iw ),
                Instr(Mnemonic.xchw, AX,HL),

                Instr(Mnemonic.mov, Mword_c_b,Ib ),
                Instr(Mnemonic.mov, Mword_bc_b,Ib ),
                Instr(Mnemonic.subc, saddr,Ib ),
                Instr(Mnemonic.subc, A, saddr),


                Instr(Mnemonic.subc, A,Ib ),
                Instr(Mnemonic.subc, A,M_hl_b),
                Instr(Mnemonic.subc, A,M_hl_off_b),
                Instr(Mnemonic.subc, A,Maddr16b),

                // 0x40
                Instr(Mnemonic.cmp, Maddr16b,Ib ),
                Instr(Mnemonic.mov, ES,Ib ),
                Instr(Mnemonic.cmpw, AX,Maddr16w),
                Instr(Mnemonic.cmpw, AX,BC),

                Instr(Mnemonic.cmpw, AX,Iw ),
                Instr(Mnemonic.cmpw, AX, DE),
                Instr(Mnemonic.cmpw, AX, saddrp),
                Instr(Mnemonic.cmpw, AX, HL),

                Instr(Mnemonic.mov, Mword_bc_b, A),
                Instr(Mnemonic.mov, A, Mword_bc_b),
                Instr(Mnemonic.cmp, saddr,Ib ),
                Instr(Mnemonic.cmp, A,saddr),

                Instr(Mnemonic.cmp, A,Ib ),
                Instr(Mnemonic.cmp, A,M_hl_b),
                Instr(Mnemonic.cmp, A,M_hl_off_b),
                Instr(Mnemonic.cmp, A,Maddr16b),

                // 0x50
                Instr(Mnemonic.mov, X,Ib ),
                Instr(Mnemonic.mov, A,Ib ),
                Instr(Mnemonic.mov, C,Ib ),
                Instr(Mnemonic.mov, B,Ib ),

                Instr(Mnemonic.mov, E,Ib ),
                Instr(Mnemonic.mov, D,Ib ),
                Instr(Mnemonic.mov, L,Ib ),
                Instr(Mnemonic.mov, H,Ib ),

                Instr(Mnemonic.movw, Mword_b_w,AX),
                Instr(Mnemonic.movw, AX,Mword_b_w),
                Instr(Mnemonic.and, saddr,Ib ),
                Instr(Mnemonic.and, A, saddr),


                Instr(Mnemonic.and, A,Ib ),
                Instr(Mnemonic.and, A,M_hl_b),
                Instr(Mnemonic.and, A,M_hl_off_b),
                Instr(Mnemonic.and, A,Maddr16b),

                // 0x60
                Instr(Mnemonic.mov, A,X),
                TwoByte(s_ext2),
                Instr(Mnemonic.mov, A, C),
                Instr(Mnemonic.mov, A, B),


                Instr(Mnemonic.mov, A, E),
                Instr(Mnemonic.mov, A, D),
                Instr(Mnemonic.mov, A, L),
                Instr(Mnemonic.mov, A, H),


                Instr(Mnemonic.movw, Mword_c_w, AX),
                Instr(Mnemonic.movw, AX, Mword_c_w),
                Instr(Mnemonic.or, saddr,Ib ),
                Instr(Mnemonic.or, A,saddr),

                Instr(Mnemonic.or, A,Ib ),
                Instr(Mnemonic.or, A,M_hl_b),
                Instr(Mnemonic.or, A,M_hl_off_b),
                Instr(Mnemonic.or, A,Maddr16b),

                // 0x70
                Instr(Mnemonic.mov, X,A),
                TwoByte(s_ext3),
                Instr(Mnemonic.mov, C, A),
                Instr(Mnemonic.mov, B, A),

                Instr(Mnemonic.mov, E, A),
                Instr(Mnemonic.mov, D, A),
                Instr(Mnemonic.mov, L, A),
                Instr(Mnemonic.mov, H, A),


                Instr(Mnemonic.movw, Mword_bc_w, AX),
                Instr(Mnemonic.movw, AX, Mword_bc_w),
                Instr(Mnemonic.xor, saddr,Ib ),
                Instr(Mnemonic.xor, A,saddr),

                Instr(Mnemonic.xor, A,Ib ),
                Instr(Mnemonic.xor, A,M_hl_b),
                Instr(Mnemonic.xor, A,M_hl_off_b),
                Instr(Mnemonic.xor, A,Maddr16b),

                // 0x80
                Instr(Mnemonic.inc, X),
                Instr(Mnemonic.inc, A),
                Instr(Mnemonic.inc, C),
                Instr(Mnemonic.inc, B),

                Instr(Mnemonic.inc, E),
                Instr(Mnemonic.inc, D),
                Instr(Mnemonic.inc, L),
                Instr(Mnemonic.inc, H),

                Instr(Mnemonic.mov, A,M_sp_off_b ),
                Instr(Mnemonic.mov, A,M_de_b),
                Instr(Mnemonic.mov, A,M_de_off_b),
                Instr(Mnemonic.mov, A,M_hl_b),

                Instr(Mnemonic.mov, A,M_hl_off_b),
                Instr(Mnemonic.mov, A, saddr),
                Instr(Mnemonic.mov, A, sfr),
                Instr(Mnemonic.mov, A,Maddr16b),

                // 0x90 
                Instr(Mnemonic.dec, X),
                Instr(Mnemonic.dec, A),
                Instr(Mnemonic.dec, C),
                Instr(Mnemonic.dec, B),

                Instr(Mnemonic.dec, E),
                Instr(Mnemonic.dec, D),
                Instr(Mnemonic.dec, L),
                Instr(Mnemonic.dec, H),

                Instr(Mnemonic.mov,M_sp_off_b, A),
                Instr(Mnemonic.mov,M_de_b,A),
                Instr(Mnemonic.mov,M_de_off_b, A),
                Instr(Mnemonic.mov,M_hl_b,A),

                Instr(Mnemonic.mov,M_hl_off_b, A),
                Instr(Mnemonic.mov, saddr, A),
                Instr(Mnemonic.mov, sfr, A),
                Instr(Mnemonic.mov, Maddr16b,A),

                // 0xA0
                Instr(Mnemonic.inc, Maddr16b),
                Instr(Mnemonic.incw, AX),
                Instr(Mnemonic.incw, Maddr16w),
                Instr(Mnemonic.incw, BC),

                Instr(Mnemonic.inc, saddr),
                Instr(Mnemonic.incw, DE),
                Instr(Mnemonic.incw, saddrp),
                Instr(Mnemonic.incw, HL),

                Instr(Mnemonic.movw, AX,M_sp_off_w ),
                Instr(Mnemonic.movw, AX,M_de_w),
                Instr(Mnemonic.movw, AX,M_de_off_w),
                Instr(Mnemonic.movw, AX,M_hl_w),


                Instr(Mnemonic.movw, AX,M_hl_off_w),
                Instr(Mnemonic.movw, AX, saddrp),
                Instr(Mnemonic.movw, AX, sfrp),
                Instr(Mnemonic.movw, AX,Maddr16w),

                // 0xB0
                Instr(Mnemonic.dec, Maddr16b),
                Instr(Mnemonic.decw, AX),
                Instr(Mnemonic.decw, Maddr16w),
                Instr(Mnemonic.decw, BC),

                Instr(Mnemonic.dec, saddr),
                Instr(Mnemonic.decw, DE),
                Instr(Mnemonic.decw, saddrp),
                Instr(Mnemonic.decw, HL),

                Instr(Mnemonic.movw,M_sp_off_w, AX),
                Instr(Mnemonic.movw,M_de_w,AX),
                Instr(Mnemonic.movw,M_de_off_b, AX),
                Instr(Mnemonic.movw,M_hl_w,AX),

                Instr(Mnemonic.movw,M_hl_off_w, AX),
                Instr(Mnemonic.movw, saddrp, AX),
                Instr(Mnemonic.movw, sfrp, AX),
                Instr(Mnemonic.movw, Maddr16w,AX),

                // 0xC0
                Instr(Mnemonic.pop, AX),
                Instr(Mnemonic.push, AX),
                Instr(Mnemonic.pop, BC),
                Instr(Mnemonic.push, BC),

                Instr(Mnemonic.pop, DE),
                Instr(Mnemonic.push, DE),
                Instr(Mnemonic.pop, HL),
                Instr(Mnemonic.push, HL),

                Instr(Mnemonic.mov,M_sp_off_b,Ib ),
                Instr(Mnemonic.movw, saddrp,Iw ),
                Instr(Mnemonic.mov,M_de_off_b,Ib ),
                Instr(Mnemonic.movw, sfrp,Iw ),

                Instr(Mnemonic.mov,M_hl_off_b,Ib ),
                Instr(Mnemonic.mov, saddr,Ib ),
                Instr(Mnemonic.mov, sfr,Ib ),
                Instr(Mnemonic.mov, Maddr16b,Ib ),

                // 0xD0
                Instr(Mnemonic.cmp0, X),
                Instr(Mnemonic.cmp0, A),
                Instr(Mnemonic.cmp0, C),
                Instr(Mnemonic.cmp0, B),

                Instr(Mnemonic.cmp0, saddr),
                Instr(Mnemonic.cmp0, Maddr16b),
                Instr(Mnemonic.mulu, X),
                Instr(Mnemonic.ret, InstrClass.Transfer),

                Instr(Mnemonic.mov, X, saddr),
                Instr(Mnemonic.mov, X,Maddr16b),
                Instr(Mnemonic.movw, BC,saddrp),
                Instr(Mnemonic.movw, BC,Maddr16b),

                Instr(Mnemonic.bc, InstrClass.ConditionalTransfer, PcRelative8),
                Instr(Mnemonic.bz, InstrClass.ConditionalTransfer, PcRelative8),
                Instr(Mnemonic.bnc, InstrClass.ConditionalTransfer, PcRelative8),
                Instr(Mnemonic.bnz, InstrClass.ConditionalTransfer, PcRelative8),

                // 0xInstr(E0),
                Instr(Mnemonic.oneb, X),
                Instr(Mnemonic.oneb, A),
                Instr(Mnemonic.oneb, C),
                Instr(Mnemonic.oneb, B),

                Instr(Mnemonic.oneb, saddr),
                Instr(Mnemonic.oneb, Maddr16b),
                Instr(Mnemonic.onew, AX),
                Instr(Mnemonic.onew, BC),

                Instr(Mnemonic.mov, B,saddr),
                Instr(Mnemonic.mov, B,Maddr16b),
                Instr(Mnemonic.movw, DE,saddrp),
                Instr(Mnemonic.movw, DE,Maddr16w),

                Instr(Mnemonic.br, InstrClass.Transfer, ImmAddr20),
                Instr(Mnemonic.br, InstrClass.Transfer, ImmAddr16),
                Instr(Mnemonic.br, InstrClass.Transfer, PcRelative16),
                Instr(Mnemonic.br, InstrClass.Transfer, PcRelative8),

                // 0xInstr(F0),
                Instr(Mnemonic.clrb, X),
                Instr(Mnemonic.clrb, A),
                Instr(Mnemonic.clrb, C),
                Instr(Mnemonic.clrb, B),

                Instr(Mnemonic.clrb, saddr),
                Instr(Mnemonic.clrb, Maddr16b),
                Instr(Mnemonic.clrw, AX),
                Instr(Mnemonic.clrw, BC),

                Instr(Mnemonic.mov, C,saddr),
                Instr(Mnemonic.mov, C,Maddr16b),
                Instr(Mnemonic.movw, HL,saddrp),
                Instr(Mnemonic.movw, HL,Maddr16w),

                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, ImmAddr20),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Maddr16w),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, PcRelative16),
                s_invalid
            };
        }
    }
}
