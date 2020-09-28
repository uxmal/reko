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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    using Decoder = Decoder<Tms7000Disassembler, Mnemonic, Tms7000Instruction>;

    public class Tms7000Disassembler : DisassemblerBase<Tms7000Instruction, Mnemonic>
    {
        private readonly Tms7000Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;

        public Tms7000Disassembler(Tms7000Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Tms7000Instruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
                return null;
            Tms7000Instruction instr;
            if (!decoders.TryGetValue(b, out var instrDecoder))
            {
                instr = CreateInvalidInstruction();
            }
            else
            {
                ops.Clear();
                instr = instrDecoder.Decode(b, this);
            }
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        #region Mutators

        private static bool A(uint b, Tms7000Disassembler dasm) {
            dasm.ops.Add(new RegisterOperand(dasm.arch.a));
            return true;
        }

        private static bool B(uint b, Tms7000Disassembler dasm) {
            dasm.ops.Add(new RegisterOperand(dasm.arch.b));
            return true;
        }

        private static bool R(uint u, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.arch.GpRegs[b]));
            return true;
        }

        private static bool P(uint u, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.arch.Ports[b]));
            return true;
        }

        private static bool S(uint b, Tms7000Disassembler dasm) {
            dasm.ops.Add(new RegisterOperand(dasm.arch.st));
            return true;
        }

        private static bool i(uint u, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(b));
            return true;
        }

        private static bool I(uint b, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadBeUInt16(out ushort w))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(w));
            return true;
        }

        // short PC-relative jump
        private static bool j(uint u, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + (sbyte) b));
            return true;
        }

        // direct, absolute address
        private static bool D(uint b, Tms7000Disassembler dasm) {
            if (!dasm.rdr.TryReadBeUInt16(out ushort w))
                return false;
            dasm.ops.Add(MemoryOperand.Direct(Address.Ptr16(w)));
            return true;
        }

        private static bool DB(uint b, Tms7000Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort w))
                return false;
            dasm.ops.Add(MemoryOperand.Indexed(Address.Ptr16(w), dasm.arch.b));
            return true;
        }


        // indirect
        private static bool Indirect(uint u, Tms7000Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;

            dasm.ops.Add(MemoryOperand.Indirect(dasm.arch.GpRegs[b]));
            return true;
        }

        #endregion

        public override Tms7000Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Tms7000Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override Tms7000Instruction CreateInvalidInstruction()
        {
            return new Tms7000Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Tms7000Disassembler> [] mutators)
        {
            return new InstrDecoder<Tms7000Disassembler, Mnemonic, Tms7000Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Tms7000Disassembler>[] mutators)
        {
            return new InstrDecoder<Tms7000Disassembler, Mnemonic, Tms7000Instruction>(iclass, mnemonic, mutators);
        }

        private static readonly Mutator<Tms7000Disassembler>[]  F_None = {};

            private static readonly Mutator<Tms7000Disassembler>[]  F_A = {A};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B = {B};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn = {R};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_A = {R,A};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_B = {R,B};
            private static readonly Mutator<Tms7000Disassembler>[]  F_A_Rn = {A,R};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_Rn = {B,R};
            private static readonly Mutator<Tms7000Disassembler>[]  F_ST = {S};

            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_A = {i,A};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_B = {i,B};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_Rn = {i,R};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop16_Rn = {I,R};

            private static readonly Mutator<Tms7000Disassembler>[]  F_A_B = {A,B};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_A = {B,A};

            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_Rn = {R,R};
            private static readonly Mutator<Tms7000Disassembler>[]  F_A_Pn = {A,P};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_Pn = {B,P};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Pn_A = {P,A};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Pn_B = {P,B};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_Pn = {i,P};

            private static readonly Mutator<Tms7000Disassembler>[]  F_iop16 = {D};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop16idxB = {DB};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop16idxB_Rn = {DB,R};

            private static readonly Mutator<Tms7000Disassembler>[]  F_starRn = { Indirect};

            private static readonly Mutator<Tms7000Disassembler>[]  F_offst = {j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_A_offst = {A,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_offst = {B, j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_A_offst = {B,A,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_offst = {R,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_A_offst = {R,A,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_B_offst = {R,B,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_Rn_Rn_offst = {R,R,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_A_offst = {i,A,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_B_offst = {i,B,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_Rn_offst = {i,R,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_A_Pn_offst = {A,P,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_B_Pn_offst = {B,P,j};
            private static readonly Mutator<Tms7000Disassembler>[]  F_iop_Pn_offst = {i,P,j};

        static Dictionary<uint, Decoder> decoders = new Dictionary<uint, Decoder>
        {
            { 0x69, Instr(Mnemonic.adc, F_B_A) },
            { 0x19, Instr(Mnemonic.adc, F_Rn_A) },
            { 0x39, Instr(Mnemonic.adc, F_Rn_B) },
            { 0x49, Instr(Mnemonic.adc, F_Rn_Rn) },
            { 0x29, Instr(Mnemonic.adc, F_iop_A) },
            { 0x59, Instr(Mnemonic.adc, F_iop_B) },
            { 0x79, Instr(Mnemonic.adc, F_iop_Rn) },

            { 0x68, Instr(Mnemonic.add, F_B_A) },
            { 0x18, Instr(Mnemonic.add, F_Rn_A) },
            { 0x38, Instr(Mnemonic.add, F_Rn_B) },
            { 0x48, Instr(Mnemonic.add, F_Rn_Rn) },
            { 0x28, Instr(Mnemonic.add, F_iop_A) },
            { 0x58, Instr(Mnemonic.add, F_iop_B) },
            { 0x78, Instr(Mnemonic.add, F_iop_Rn) },

            { 0x63, Instr(Mnemonic.and, F_B_A) },
            { 0x13, Instr(Mnemonic.and, F_Rn_A) },
            { 0x33, Instr(Mnemonic.and, F_Rn_B) },
            { 0x43, Instr(Mnemonic.and, F_Rn_Rn) },
            { 0x23, Instr(Mnemonic.and, F_iop_A) },
            { 0x53, Instr(Mnemonic.and, F_iop_B) },
            { 0x73, Instr(Mnemonic.and, F_iop_Rn) },

            { 0x83, Instr(Mnemonic.andp, F_A_Pn) },
            { 0x93, Instr(Mnemonic.andp, F_B_Pn) },
            { 0xA3, Instr(Mnemonic.andp, F_iop_Pn) },

            { 0x8c, Instr(Mnemonic.br, F_iop16) },
            { 0xac, Instr(Mnemonic.br, F_iop16idxB) },
            { 0x9c, Instr(Mnemonic.br, F_starRn) },

            { 0x66, Instr(Mnemonic.btjo, F_B_A_offst) },
            { 0x16, Instr(Mnemonic.btjo, F_Rn_A_offst) },
            { 0x36, Instr(Mnemonic.btjo, F_Rn_B_offst) },
            { 0x46, Instr(Mnemonic.btjo, F_Rn_Rn_offst) },
            { 0x26, Instr(Mnemonic.btjo, F_iop_A_offst) },
            { 0x56, Instr(Mnemonic.btjo, F_iop_B_offst) },
            { 0x76, Instr(Mnemonic.btjo, F_iop_Rn_offst) },

            { 0x86, Instr(Mnemonic.btjop, F_A_Pn_offst) },
            { 0x96, Instr(Mnemonic.btjop, F_B_Pn_offst) },
            { 0xA6, Instr(Mnemonic.btjop, F_iop_Pn_offst) },

            { 0x67, Instr(Mnemonic.btjz, F_B_A_offst) },
            { 0x17, Instr(Mnemonic.btjz, F_Rn_A_offst) },
            { 0x37, Instr(Mnemonic.btjz, F_Rn_B_offst) },
            { 0x47, Instr(Mnemonic.btjz, F_Rn_Rn_offst) },
            { 0x27, Instr(Mnemonic.btjz, F_iop_A_offst) },
            { 0x57, Instr(Mnemonic.btjz, F_iop_B_offst) },
            { 0x77, Instr(Mnemonic.btjz, F_iop_Rn_offst) },

            { 0x87, Instr(Mnemonic.btjzp, F_A_Pn_offst) },
            { 0x97, Instr(Mnemonic.btjzp, F_B_Pn_offst) },
            { 0xA7, Instr(Mnemonic.btjzp, F_iop_Pn_offst) },

            { 0x8e, Instr(Mnemonic.call, F_iop16) },
            { 0xae, Instr(Mnemonic.call, F_iop16idxB) },
            { 0x9e, Instr(Mnemonic.call, F_starRn) },

            { 0xb5, Instr(Mnemonic.clr, F_A) },
            { 0xc5, Instr(Mnemonic.clr, F_B) },
            { 0xd5, Instr(Mnemonic.clr, F_Rn) },

            { 0x6d, Instr(Mnemonic.cmp, F_B_A) },
            { 0x1d, Instr(Mnemonic.cmp, F_Rn_A) },
            { 0x3d, Instr(Mnemonic.cmp, F_Rn_B) },
            { 0x4d, Instr(Mnemonic.cmp, F_Rn_Rn) },
            { 0x2d, Instr(Mnemonic.cmp, F_iop_A) },
            { 0x5d, Instr(Mnemonic.cmp, F_iop_B) },
            { 0x7d, Instr(Mnemonic.cmp, F_iop_Rn) },

            { 0x8d, Instr(Mnemonic.cmpa, F_iop16) },
            { 0xad, Instr(Mnemonic.cmpa, F_iop16idxB) },
            { 0x9d, Instr(Mnemonic.cmpa, F_starRn) },

            { 0x6e, Instr(Mnemonic.dac, F_A_B) },
            { 0x1e, Instr(Mnemonic.dac, F_Rn_A) },
            { 0x3e, Instr(Mnemonic.dac, F_Rn_B) },
            { 0x4e, Instr(Mnemonic.dac, F_Rn_Rn) },
            { 0x2e, Instr(Mnemonic.dac, F_iop_A) },
            { 0x5e, Instr(Mnemonic.dac, F_iop_B) },
            { 0x7e, Instr(Mnemonic.dac, F_iop_Rn) },

            { 0xb2, Instr(Mnemonic.dec, F_A) },
            { 0xc2, Instr(Mnemonic.dec, F_B) },
            { 0xd2, Instr(Mnemonic.dec, F_Rn) },

            { 0xba, Instr(Mnemonic.djnz, F_A_offst) },
            { 0xca, Instr(Mnemonic.djnz, F_B_offst) },
            { 0xda, Instr(Mnemonic.djnz, F_Rn_offst) },

            { 0x06, Instr(Mnemonic.dint, F_None) },

            { 0xbb, Instr(Mnemonic.decd, F_A) },
            { 0xcb, Instr(Mnemonic.decd, F_B) },
            { 0xdb, Instr(Mnemonic.decd, F_Rn) },

            { 0x6f, Instr(Mnemonic.dsb, F_B_A) },
            { 0x1f, Instr(Mnemonic.dsb, F_Rn_A) },
            { 0x3f, Instr(Mnemonic.dsb, F_Rn_B) },
            { 0x4f, Instr(Mnemonic.dsb, F_Rn_Rn) },
            { 0x2f, Instr(Mnemonic.dsb, F_iop_A) },
            { 0x5f, Instr(Mnemonic.dsb, F_iop_B) },
            { 0x7f, Instr(Mnemonic.dsb, F_iop_Rn) },

            { 0x05, Instr(Mnemonic.eint, F_None) },

            { 0x01, Instr(Mnemonic.idle, F_None) },

            { 0xb3, Instr(Mnemonic.inc, F_A) },
            { 0xc3, Instr(Mnemonic.inc, F_B) },
            { 0xd3, Instr(Mnemonic.inc, F_Rn) },

            { 0xb4, Instr(Mnemonic.inv, F_A) },
            { 0xc4, Instr(Mnemonic.inv, F_B) },
            { 0xd4, Instr(Mnemonic.inv, F_Rn) },

            { 0xe0, Instr(Mnemonic.jmp, F_offst) },
            { 0xe2, Instr(Mnemonic.jeq, F_offst) },
            { 0xe5, Instr(Mnemonic.jge, F_offst) },
            { 0xe4, Instr(Mnemonic.jgt, F_offst) },
            { 0xe3, Instr(Mnemonic.jhs, F_offst) },
            { 0xe7, Instr(Mnemonic.jl, F_offst) },
            { 0xe6, Instr(Mnemonic.jne, F_offst) },

            { 0x8a, Instr(Mnemonic.lda, F_iop16) },
            { 0xaa, Instr(Mnemonic.lda, F_iop16idxB) },
            { 0x9a, Instr(Mnemonic.lda, F_starRn) },

            { 0x0d, Instr(Mnemonic.ldsp, F_None) },

            { 0xc0, Instr(Mnemonic.mov, F_A_B) },
            { 0xd0, Instr(Mnemonic.mov, F_A_Rn) },
            { 0x62, Instr(Mnemonic.mov, F_B_A) },
            { 0xd1, Instr(Mnemonic.mov, F_B_Rn) },
            { 0x12, Instr(Mnemonic.mov, F_Rn_A) },
            { 0x32, Instr(Mnemonic.mov, F_Rn_B) },
            { 0x42, Instr(Mnemonic.mov, F_Rn_Rn) },
            { 0x22, Instr(Mnemonic.mov, F_iop_A) },
            { 0x52, Instr(Mnemonic.mov, F_iop_B) },
            { 0x72, Instr(Mnemonic.mov, F_iop_Rn) },

            { 0x88, Instr(Mnemonic.movd, F_iop16_Rn) },
            { 0xa8, Instr(Mnemonic.movd, F_iop16idxB_Rn) },
            { 0x98, Instr(Mnemonic.movd, F_Rn_Rn) },

            { 0x82, Instr(Mnemonic.movp, F_A_Pn) },
            { 0x92, Instr(Mnemonic.movp, F_B_Pn) },
            { 0xA2, Instr(Mnemonic.movp, F_iop_Pn) },
            { 0x80, Instr(Mnemonic.movp, F_Pn_A) },
            { 0x91, Instr(Mnemonic.movp, F_Pn_B) },

            { 0x6c, Instr(Mnemonic.mpy, F_B_A) },
            { 0x1c, Instr(Mnemonic.mpy, F_Rn_A) },
            { 0x3c, Instr(Mnemonic.mpy, F_Rn_B) },
            { 0x4c, Instr(Mnemonic.mpy, F_Rn_Rn) },
            { 0x2c, Instr(Mnemonic.mpy, F_iop_A) },
            { 0x5c, Instr(Mnemonic.mpy, F_iop_B) },
            { 0x7c, Instr(Mnemonic.mpy, F_iop_Rn) },

            { 0x00, Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Zero, F_None) },

            { 0x64, Instr(Mnemonic.or, F_A_B) },
            { 0x14, Instr(Mnemonic.or, F_Rn_A) },
            { 0x34, Instr(Mnemonic.or, F_Rn_B) },
            { 0x44, Instr(Mnemonic.or, F_Rn_Rn) },
            { 0x24, Instr(Mnemonic.or, F_iop_A) },
            { 0x54, Instr(Mnemonic.or, F_iop_B) },
            { 0x74, Instr(Mnemonic.or, F_iop_Rn) },

            { 0x84, Instr(Mnemonic.orp, F_A_Pn) },
            { 0x94, Instr(Mnemonic.orp, F_B_Pn) },
            { 0xA4, Instr(Mnemonic.orp, F_iop_Pn) },

            { 0xb9, Instr(Mnemonic.pop, F_A) },
            { 0xc9, Instr(Mnemonic.pop, F_B) },
            { 0xd9, Instr(Mnemonic.pop, F_Rn) },
            { 0x08, Instr(Mnemonic.pop, F_ST) },

            { 0xb8, Instr(Mnemonic.push, F_A) },
            { 0xc8, Instr(Mnemonic.push, F_B) },
            { 0xd8, Instr(Mnemonic.push, F_Rn) },
            { 0x0e, Instr(Mnemonic.push, F_ST) },

            { 0x0b, Instr(Mnemonic.reti, F_None) },

            { 0x0a, Instr(Mnemonic.rets, F_None) },

            { 0xbe, Instr(Mnemonic.rl, F_A) },
            { 0xce, Instr(Mnemonic.rl, F_B) },
            { 0xde, Instr(Mnemonic.rl, F_Rn) },

            { 0xbf, Instr(Mnemonic.rlc, F_A) },
            { 0xcf, Instr(Mnemonic.rlc, F_B) },
            { 0xdf, Instr(Mnemonic.rlc, F_Rn) },

            { 0xbc, Instr(Mnemonic.rr, F_A) },
            { 0xcc, Instr(Mnemonic.rr, F_B) },
            { 0xdc, Instr(Mnemonic.rr, F_Rn) },

            { 0xbd, Instr(Mnemonic.rrc, F_A) },
            { 0xcd, Instr(Mnemonic.rrc, F_B) },
            { 0xdd, Instr(Mnemonic.rrc, F_Rn) },

            { 0x6b, Instr(Mnemonic.sbb, F_B_A) },
            { 0x1b, Instr(Mnemonic.sbb, F_Rn_A) },
            { 0x3b, Instr(Mnemonic.sbb, F_Rn_B) },
            { 0x4b, Instr(Mnemonic.sbb, F_Rn_Rn) },
            { 0x2b, Instr(Mnemonic.sbb, F_iop_A) },
            { 0x5b, Instr(Mnemonic.sbb, F_iop_B) },
            { 0x7b, Instr(Mnemonic.sbb, F_iop_Rn) },

            { 0x07, Instr(Mnemonic.setc, F_None) },

            { 0x8b, Instr(Mnemonic.sta, F_iop16) },
            { 0xab, Instr(Mnemonic.sta, F_iop16idxB) },
            { 0x9b, Instr(Mnemonic.sta, F_starRn) },

            { 0x09, Instr(Mnemonic.stsp, F_None) },

            { 0x6a, Instr(Mnemonic.sub, F_B_A) },
            { 0x1a, Instr(Mnemonic.sub, F_Rn_A) },
            { 0x3a, Instr(Mnemonic.sub, F_Rn_B) },
            { 0x4a, Instr(Mnemonic.sub, F_Rn_Rn) },
            { 0x2a, Instr(Mnemonic.sub, F_iop_A) },
            { 0x5a, Instr(Mnemonic.sub, F_iop_B) },
            { 0x7a, Instr(Mnemonic.sub, F_iop_Rn) },

            { 0xb7, Instr(Mnemonic.swap, F_A) },
            { 0xc7, Instr(Mnemonic.swap, F_B) },
            { 0xd7, Instr(Mnemonic.swap, F_Rn) },

            { 0xe8, Instr(Mnemonic.trap_0, F_None) },
            { 0xe9, Instr(Mnemonic.trap_1, F_None) },
            { 0xea, Instr(Mnemonic.trap_2, F_None) },
            { 0xeb, Instr(Mnemonic.trap_3, F_None) },
            { 0xec, Instr(Mnemonic.trap_4, F_None) },
            { 0xed, Instr(Mnemonic.trap_5, F_None) },
            { 0xee, Instr(Mnemonic.trap_6, F_None) },
            { 0xef, Instr(Mnemonic.trap_7, F_None) },
            { 0xf0, Instr(Mnemonic.trap_8, F_None) },
            { 0xf1, Instr(Mnemonic.trap_9, F_None) },
            { 0xf2, Instr(Mnemonic.trap_10, F_None) },
            { 0xf3, Instr(Mnemonic.trap_11, F_None) },
            { 0xf4, Instr(Mnemonic.trap_12, F_None) },
            { 0xf5, Instr(Mnemonic.trap_13, F_None) },
            { 0xf6, Instr(Mnemonic.trap_14, F_None) },
            { 0xf7, Instr(Mnemonic.trap_15, F_None) },
            { 0xf8, Instr(Mnemonic.trap_16, F_None) },
            { 0xf9, Instr(Mnemonic.trap_17, F_None) },
            { 0xfa, Instr(Mnemonic.trap_18, F_None) },
            { 0xfb, Instr(Mnemonic.trap_19, F_None) },
            { 0xfc, Instr(Mnemonic.trap_20, F_None) },
            { 0xfd, Instr(Mnemonic.trap_21, F_None) },
            { 0xfe, Instr(Mnemonic.trap_22, F_None) },
            { 0xff, Instr(Mnemonic.trap_23, F_None) },

            { 0xb0, Instr(Mnemonic.tsta, F_None) },

            { 0xc1, Instr(Mnemonic.tstb, F_None) },

            { 0xb6, Instr(Mnemonic.xchb, F_A) },
            { 0xc6, Instr(Mnemonic.xchb, F_B) },
            { 0xd6, Instr(Mnemonic.xchb, F_Rn) },

            { 0x65, Instr(Mnemonic.xor, F_B_A) },
            { 0x15, Instr(Mnemonic.xor, F_Rn_A) },
            { 0x35, Instr(Mnemonic.xor, F_Rn_B) },
            { 0x45, Instr(Mnemonic.xor, F_Rn_Rn) },
            { 0x25, Instr(Mnemonic.xor, F_iop_A) },
            { 0x55, Instr(Mnemonic.xor, F_iop_B) },
            { 0x75, Instr(Mnemonic.xor, F_iop_Rn) },

            { 0x85, Instr(Mnemonic.xorp, F_A_Pn) },
            { 0x95, Instr(Mnemonic.xorp, F_B_Pn) },
            { 0xA5, Instr(Mnemonic.xorp, F_iop_Pn) },
        };
    }
}
