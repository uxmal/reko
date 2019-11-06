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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    using Decoder = Decoder<Tms7000Disassembler, Opcode, Tms7000Instruction>;

    public class Tms7000Disassembler : DisassemblerBase<Tms7000Instruction>
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

        protected override Tms7000Instruction CreateInvalidInstruction()
        {
            return new Tms7000Instruction
            {
                Mnemonic = Opcode.invalid,
            };
        }

        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly Mutator<Tms7000Disassembler>[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator<Tms7000Disassembler>[] mutators) : this(opcode, InstrClass.Linear, mutators)
            {
            }

            public InstrDecoder(Opcode opcode, InstrClass iclass, params Mutator<Tms7000Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override Tms7000Instruction Decode(uint b, Tms7000Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new Tms7000Instruction
                {
                    Mnemonic = opcode,
                    InstructionClass = iclass,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
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
            { 0x69, new InstrDecoder(Opcode.adc, F_B_A) },
            { 0x19, new InstrDecoder(Opcode.adc, F_Rn_A) },
            { 0x39, new InstrDecoder(Opcode.adc, F_Rn_B) },
            { 0x49, new InstrDecoder(Opcode.adc, F_Rn_Rn) },
            { 0x29, new InstrDecoder(Opcode.adc, F_iop_A) },
            { 0x59, new InstrDecoder(Opcode.adc, F_iop_B) },
            { 0x79, new InstrDecoder(Opcode.adc, F_iop_Rn) },

            { 0x68, new InstrDecoder(Opcode.add, F_B_A) },
            { 0x18, new InstrDecoder(Opcode.add, F_Rn_A) },
            { 0x38, new InstrDecoder(Opcode.add, F_Rn_B) },
            { 0x48, new InstrDecoder(Opcode.add, F_Rn_Rn) },
            { 0x28, new InstrDecoder(Opcode.add, F_iop_A) },
            { 0x58, new InstrDecoder(Opcode.add, F_iop_B) },
            { 0x78, new InstrDecoder(Opcode.add, F_iop_Rn) },

            { 0x63, new InstrDecoder(Opcode.and, F_B_A) },
            { 0x13, new InstrDecoder(Opcode.and, F_Rn_A) },
            { 0x33, new InstrDecoder(Opcode.and, F_Rn_B) },
            { 0x43, new InstrDecoder(Opcode.and, F_Rn_Rn) },
            { 0x23, new InstrDecoder(Opcode.and, F_iop_A) },
            { 0x53, new InstrDecoder(Opcode.and, F_iop_B) },
            { 0x73, new InstrDecoder(Opcode.and, F_iop_Rn) },

            { 0x83, new InstrDecoder(Opcode.andp, F_A_Pn) },
            { 0x93, new InstrDecoder(Opcode.andp, F_B_Pn) },
            { 0xA3, new InstrDecoder(Opcode.andp, F_iop_Pn) },

            { 0x8c, new InstrDecoder(Opcode.br, F_iop16) },
            { 0xac, new InstrDecoder(Opcode.br, F_iop16idxB) },
            { 0x9c, new InstrDecoder(Opcode.br, F_starRn) },

            { 0x66, new InstrDecoder(Opcode.btjo, F_B_A_offst) },
            { 0x16, new InstrDecoder(Opcode.btjo, F_Rn_A_offst) },
            { 0x36, new InstrDecoder(Opcode.btjo, F_Rn_B_offst) },
            { 0x46, new InstrDecoder(Opcode.btjo, F_Rn_Rn_offst) },
            { 0x26, new InstrDecoder(Opcode.btjo, F_iop_A_offst) },
            { 0x56, new InstrDecoder(Opcode.btjo, F_iop_B_offst) },
            { 0x76, new InstrDecoder(Opcode.btjo, F_iop_Rn_offst) },

            { 0x86, new InstrDecoder(Opcode.btjop, F_A_Pn_offst) },
            { 0x96, new InstrDecoder(Opcode.btjop, F_B_Pn_offst) },
            { 0xA6, new InstrDecoder(Opcode.btjop, F_iop_Pn_offst) },

            { 0x67, new InstrDecoder(Opcode.btjz, F_B_A_offst) },
            { 0x17, new InstrDecoder(Opcode.btjz, F_Rn_A_offst) },
            { 0x37, new InstrDecoder(Opcode.btjz, F_Rn_B_offst) },
            { 0x47, new InstrDecoder(Opcode.btjz, F_Rn_Rn_offst) },
            { 0x27, new InstrDecoder(Opcode.btjz, F_iop_A_offst) },
            { 0x57, new InstrDecoder(Opcode.btjz, F_iop_B_offst) },
            { 0x77, new InstrDecoder(Opcode.btjz, F_iop_Rn_offst) },

            { 0x87, new InstrDecoder(Opcode.btjzp, F_A_Pn_offst) },
            { 0x97, new InstrDecoder(Opcode.btjzp, F_B_Pn_offst) },
            { 0xA7, new InstrDecoder(Opcode.btjzp, F_iop_Pn_offst) },

            { 0x8e, new InstrDecoder(Opcode.call, F_iop16) },
            { 0xae, new InstrDecoder(Opcode.call, F_iop16idxB) },
            { 0x9e, new InstrDecoder(Opcode.call, F_starRn) },

            { 0xb5, new InstrDecoder(Opcode.clr, F_A) },
            { 0xc5, new InstrDecoder(Opcode.clr, F_B) },
            { 0xd5, new InstrDecoder(Opcode.clr, F_Rn) },

            { 0x6d, new InstrDecoder(Opcode.cmp, F_B_A) },
            { 0x1d, new InstrDecoder(Opcode.cmp, F_Rn_A) },
            { 0x3d, new InstrDecoder(Opcode.cmp, F_Rn_B) },
            { 0x4d, new InstrDecoder(Opcode.cmp, F_Rn_Rn) },
            { 0x2d, new InstrDecoder(Opcode.cmp, F_iop_A) },
            { 0x5d, new InstrDecoder(Opcode.cmp, F_iop_B) },
            { 0x7d, new InstrDecoder(Opcode.cmp, F_iop_Rn) },

            { 0x8d, new InstrDecoder(Opcode.cmpa, F_iop16) },
            { 0xad, new InstrDecoder(Opcode.cmpa, F_iop16idxB) },
            { 0x9d, new InstrDecoder(Opcode.cmpa, F_starRn) },

            { 0x6e, new InstrDecoder(Opcode.dac, F_A_B) },
            { 0x1e, new InstrDecoder(Opcode.dac, F_Rn_A) },
            { 0x3e, new InstrDecoder(Opcode.dac, F_Rn_B) },
            { 0x4e, new InstrDecoder(Opcode.dac, F_Rn_Rn) },
            { 0x2e, new InstrDecoder(Opcode.dac, F_iop_A) },
            { 0x5e, new InstrDecoder(Opcode.dac, F_iop_B) },
            { 0x7e, new InstrDecoder(Opcode.dac, F_iop_Rn) },

            { 0xb2, new InstrDecoder(Opcode.dec, F_A) },
            { 0xc2, new InstrDecoder(Opcode.dec, F_B) },
            { 0xd2, new InstrDecoder(Opcode.dec, F_Rn) },

            { 0xba, new InstrDecoder(Opcode.djnz, F_A_offst) },
            { 0xca, new InstrDecoder(Opcode.djnz, F_B_offst) },
            { 0xda, new InstrDecoder(Opcode.djnz, F_Rn_offst) },

            { 0x06, new InstrDecoder(Opcode.dint, F_None) },

            { 0xbb, new InstrDecoder(Opcode.decd, F_A) },
            { 0xcb, new InstrDecoder(Opcode.decd, F_B) },
            { 0xdb, new InstrDecoder(Opcode.decd, F_Rn) },

            { 0x6f, new InstrDecoder(Opcode.dsb, F_B_A) },
            { 0x1f, new InstrDecoder(Opcode.dsb, F_Rn_A) },
            { 0x3f, new InstrDecoder(Opcode.dsb, F_Rn_B) },
            { 0x4f, new InstrDecoder(Opcode.dsb, F_Rn_Rn) },
            { 0x2f, new InstrDecoder(Opcode.dsb, F_iop_A) },
            { 0x5f, new InstrDecoder(Opcode.dsb, F_iop_B) },
            { 0x7f, new InstrDecoder(Opcode.dsb, F_iop_Rn) },

            { 0x05, new InstrDecoder(Opcode.eint, F_None) },

            { 0x01, new InstrDecoder(Opcode.idle, F_None) },

            { 0xb3, new InstrDecoder(Opcode.inc, F_A) },
            { 0xc3, new InstrDecoder(Opcode.inc, F_B) },
            { 0xd3, new InstrDecoder(Opcode.inc, F_Rn) },

            { 0xb4, new InstrDecoder(Opcode.inv, F_A) },
            { 0xc4, new InstrDecoder(Opcode.inv, F_B) },
            { 0xd4, new InstrDecoder(Opcode.inv, F_Rn) },

            { 0xe0, new InstrDecoder(Opcode.jmp, F_offst) },
            { 0xe2, new InstrDecoder(Opcode.jeq, F_offst) },
            { 0xe5, new InstrDecoder(Opcode.jge, F_offst) },
            { 0xe4, new InstrDecoder(Opcode.jgt, F_offst) },
            { 0xe3, new InstrDecoder(Opcode.jhs, F_offst) },
            { 0xe7, new InstrDecoder(Opcode.jl, F_offst) },
            { 0xe6, new InstrDecoder(Opcode.jne, F_offst) },

            { 0x8a, new InstrDecoder(Opcode.lda, F_iop16) },
            { 0xaa, new InstrDecoder(Opcode.lda, F_iop16idxB) },
            { 0x9a, new InstrDecoder(Opcode.lda, F_starRn) },

            { 0x0d, new InstrDecoder(Opcode.ldsp, F_None) },

            { 0xc0, new InstrDecoder(Opcode.mov, F_A_B) },
            { 0xd0, new InstrDecoder(Opcode.mov, F_A_Rn) },
            { 0x62, new InstrDecoder(Opcode.mov, F_B_A) },
            { 0xd1, new InstrDecoder(Opcode.mov, F_B_Rn) },
            { 0x12, new InstrDecoder(Opcode.mov, F_Rn_A) },
            { 0x32, new InstrDecoder(Opcode.mov, F_Rn_B) },
            { 0x42, new InstrDecoder(Opcode.mov, F_Rn_Rn) },
            { 0x22, new InstrDecoder(Opcode.mov, F_iop_A) },
            { 0x52, new InstrDecoder(Opcode.mov, F_iop_B) },
            { 0x72, new InstrDecoder(Opcode.mov, F_iop_Rn) },

            { 0x88, new InstrDecoder(Opcode.movd, F_iop16_Rn) },
            { 0xa8, new InstrDecoder(Opcode.movd, F_iop16idxB_Rn) },
            { 0x98, new InstrDecoder(Opcode.movd, F_Rn_Rn) },

            { 0x82, new InstrDecoder(Opcode.movp, F_A_Pn) },
            { 0x92, new InstrDecoder(Opcode.movp, F_B_Pn) },
            { 0xA2, new InstrDecoder(Opcode.movp, F_iop_Pn) },
            { 0x80, new InstrDecoder(Opcode.movp, F_Pn_A) },
            { 0x91, new InstrDecoder(Opcode.movp, F_Pn_B) },

            { 0x6c, new InstrDecoder(Opcode.mpy, F_B_A) },
            { 0x1c, new InstrDecoder(Opcode.mpy, F_Rn_A) },
            { 0x3c, new InstrDecoder(Opcode.mpy, F_Rn_B) },
            { 0x4c, new InstrDecoder(Opcode.mpy, F_Rn_Rn) },
            { 0x2c, new InstrDecoder(Opcode.mpy, F_iop_A) },
            { 0x5c, new InstrDecoder(Opcode.mpy, F_iop_B) },
            { 0x7c, new InstrDecoder(Opcode.mpy, F_iop_Rn) },

            { 0x00, new InstrDecoder(Opcode.nop, InstrClass.Padding|InstrClass.Zero, F_None) },

            { 0x64, new InstrDecoder(Opcode.or, F_A_B) },
            { 0x14, new InstrDecoder(Opcode.or, F_Rn_A) },
            { 0x34, new InstrDecoder(Opcode.or, F_Rn_B) },
            { 0x44, new InstrDecoder(Opcode.or, F_Rn_Rn) },
            { 0x24, new InstrDecoder(Opcode.or, F_iop_A) },
            { 0x54, new InstrDecoder(Opcode.or, F_iop_B) },
            { 0x74, new InstrDecoder(Opcode.or, F_iop_Rn) },

            { 0x84, new InstrDecoder(Opcode.orp, F_A_Pn) },
            { 0x94, new InstrDecoder(Opcode.orp, F_B_Pn) },
            { 0xA4, new InstrDecoder(Opcode.orp, F_iop_Pn) },

            { 0xb9, new InstrDecoder(Opcode.pop, F_A) },
            { 0xc9, new InstrDecoder(Opcode.pop, F_B) },
            { 0xd9, new InstrDecoder(Opcode.pop, F_Rn) },
            { 0x08, new InstrDecoder(Opcode.pop, F_ST) },

            { 0xb8, new InstrDecoder(Opcode.push, F_A) },
            { 0xc8, new InstrDecoder(Opcode.push, F_B) },
            { 0xd8, new InstrDecoder(Opcode.push, F_Rn) },
            { 0x0e, new InstrDecoder(Opcode.push, F_ST) },

            { 0x0b, new InstrDecoder(Opcode.reti, F_None) },

            { 0x0a, new InstrDecoder(Opcode.rets, F_None) },

            { 0xbe, new InstrDecoder(Opcode.rl, F_A) },
            { 0xce, new InstrDecoder(Opcode.rl, F_B) },
            { 0xde, new InstrDecoder(Opcode.rl, F_Rn) },

            { 0xbf, new InstrDecoder(Opcode.rlc, F_A) },
            { 0xcf, new InstrDecoder(Opcode.rlc, F_B) },
            { 0xdf, new InstrDecoder(Opcode.rlc, F_Rn) },

            { 0xbc, new InstrDecoder(Opcode.rr, F_A) },
            { 0xcc, new InstrDecoder(Opcode.rr, F_B) },
            { 0xdc, new InstrDecoder(Opcode.rr, F_Rn) },

            { 0xbd, new InstrDecoder(Opcode.rrc, F_A) },
            { 0xcd, new InstrDecoder(Opcode.rrc, F_B) },
            { 0xdd, new InstrDecoder(Opcode.rrc, F_Rn) },

            { 0x6b, new InstrDecoder(Opcode.sbb, F_B_A) },
            { 0x1b, new InstrDecoder(Opcode.sbb, F_Rn_A) },
            { 0x3b, new InstrDecoder(Opcode.sbb, F_Rn_B) },
            { 0x4b, new InstrDecoder(Opcode.sbb, F_Rn_Rn) },
            { 0x2b, new InstrDecoder(Opcode.sbb, F_iop_A) },
            { 0x5b, new InstrDecoder(Opcode.sbb, F_iop_B) },
            { 0x7b, new InstrDecoder(Opcode.sbb, F_iop_Rn) },

            { 0x07, new InstrDecoder(Opcode.setc, F_None) },

            { 0x8b, new InstrDecoder(Opcode.sta, F_iop16) },
            { 0xab, new InstrDecoder(Opcode.sta, F_iop16idxB) },
            { 0x9b, new InstrDecoder(Opcode.sta, F_starRn) },

            { 0x09, new InstrDecoder(Opcode.stsp, F_None) },

            { 0x6a, new InstrDecoder(Opcode.sub, F_B_A) },
            { 0x1a, new InstrDecoder(Opcode.sub, F_Rn_A) },
            { 0x3a, new InstrDecoder(Opcode.sub, F_Rn_B) },
            { 0x4a, new InstrDecoder(Opcode.sub, F_Rn_Rn) },
            { 0x2a, new InstrDecoder(Opcode.sub, F_iop_A) },
            { 0x5a, new InstrDecoder(Opcode.sub, F_iop_B) },
            { 0x7a, new InstrDecoder(Opcode.sub, F_iop_Rn) },

            { 0xb7, new InstrDecoder(Opcode.swap, F_A) },
            { 0xc7, new InstrDecoder(Opcode.swap, F_B) },
            { 0xd7, new InstrDecoder(Opcode.swap, F_Rn) },

            { 0xe8, new InstrDecoder(Opcode.trap_0, F_None) },
            { 0xe9, new InstrDecoder(Opcode.trap_1, F_None) },
            { 0xea, new InstrDecoder(Opcode.trap_2, F_None) },
            { 0xeb, new InstrDecoder(Opcode.trap_3, F_None) },
            { 0xec, new InstrDecoder(Opcode.trap_4, F_None) },
            { 0xed, new InstrDecoder(Opcode.trap_5, F_None) },
            { 0xee, new InstrDecoder(Opcode.trap_6, F_None) },
            { 0xef, new InstrDecoder(Opcode.trap_7, F_None) },
            { 0xf0, new InstrDecoder(Opcode.trap_8, F_None) },
            { 0xf1, new InstrDecoder(Opcode.trap_9, F_None) },
            { 0xf2, new InstrDecoder(Opcode.trap_10, F_None) },
            { 0xf3, new InstrDecoder(Opcode.trap_11, F_None) },
            { 0xf4, new InstrDecoder(Opcode.trap_12, F_None) },
            { 0xf5, new InstrDecoder(Opcode.trap_13, F_None) },
            { 0xf6, new InstrDecoder(Opcode.trap_14, F_None) },
            { 0xf7, new InstrDecoder(Opcode.trap_15, F_None) },
            { 0xf8, new InstrDecoder(Opcode.trap_16, F_None) },
            { 0xf9, new InstrDecoder(Opcode.trap_17, F_None) },
            { 0xfa, new InstrDecoder(Opcode.trap_18, F_None) },
            { 0xfb, new InstrDecoder(Opcode.trap_19, F_None) },
            { 0xfc, new InstrDecoder(Opcode.trap_20, F_None) },
            { 0xfd, new InstrDecoder(Opcode.trap_21, F_None) },
            { 0xfe, new InstrDecoder(Opcode.trap_22, F_None) },
            { 0xff, new InstrDecoder(Opcode.trap_23, F_None) },

            { 0xb0, new InstrDecoder(Opcode.tsta, F_None) },

            { 0xc1, new InstrDecoder(Opcode.tstb, F_None) },

            { 0xb6, new InstrDecoder(Opcode.xchb, F_A) },
            { 0xc6, new InstrDecoder(Opcode.xchb, F_B) },
            { 0xd6, new InstrDecoder(Opcode.xchb, F_Rn) },

            { 0x65, new InstrDecoder(Opcode.xor, F_B_A) },
            { 0x15, new InstrDecoder(Opcode.xor, F_Rn_A) },
            { 0x35, new InstrDecoder(Opcode.xor, F_Rn_B) },
            { 0x45, new InstrDecoder(Opcode.xor, F_Rn_Rn) },
            { 0x25, new InstrDecoder(Opcode.xor, F_iop_A) },
            { 0x55, new InstrDecoder(Opcode.xor, F_iop_B) },
            { 0x75, new InstrDecoder(Opcode.xor, F_iop_Rn) },

            { 0x85, new InstrDecoder(Opcode.xorp, F_A_Pn) },
            { 0x95, new InstrDecoder(Opcode.xorp, F_B_Pn) },
            { 0xA5, new InstrDecoder(Opcode.xorp, F_iop_Pn) },
        };
    }
}
