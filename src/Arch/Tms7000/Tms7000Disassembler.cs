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
    using Decoder = Decoder<Tms7000Disassembler, Mnemonic, Tms7000Instruction>;

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
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        private class InstrDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly InstrClass iclass;
            private readonly Mutator<Tms7000Disassembler>[] mutators;

            public InstrDecoder(Mnemonic opcode, params Mutator<Tms7000Disassembler>[] mutators) : this(opcode, InstrClass.Linear, mutators)
            {
            }

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, params Mutator<Tms7000Disassembler>[] mutators)
            {
                this.mnemonic = mnemonic;
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
                    Mnemonic = mnemonic,
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
            { 0x69, new InstrDecoder(Mnemonic.adc, F_B_A) },
            { 0x19, new InstrDecoder(Mnemonic.adc, F_Rn_A) },
            { 0x39, new InstrDecoder(Mnemonic.adc, F_Rn_B) },
            { 0x49, new InstrDecoder(Mnemonic.adc, F_Rn_Rn) },
            { 0x29, new InstrDecoder(Mnemonic.adc, F_iop_A) },
            { 0x59, new InstrDecoder(Mnemonic.adc, F_iop_B) },
            { 0x79, new InstrDecoder(Mnemonic.adc, F_iop_Rn) },

            { 0x68, new InstrDecoder(Mnemonic.add, F_B_A) },
            { 0x18, new InstrDecoder(Mnemonic.add, F_Rn_A) },
            { 0x38, new InstrDecoder(Mnemonic.add, F_Rn_B) },
            { 0x48, new InstrDecoder(Mnemonic.add, F_Rn_Rn) },
            { 0x28, new InstrDecoder(Mnemonic.add, F_iop_A) },
            { 0x58, new InstrDecoder(Mnemonic.add, F_iop_B) },
            { 0x78, new InstrDecoder(Mnemonic.add, F_iop_Rn) },

            { 0x63, new InstrDecoder(Mnemonic.and, F_B_A) },
            { 0x13, new InstrDecoder(Mnemonic.and, F_Rn_A) },
            { 0x33, new InstrDecoder(Mnemonic.and, F_Rn_B) },
            { 0x43, new InstrDecoder(Mnemonic.and, F_Rn_Rn) },
            { 0x23, new InstrDecoder(Mnemonic.and, F_iop_A) },
            { 0x53, new InstrDecoder(Mnemonic.and, F_iop_B) },
            { 0x73, new InstrDecoder(Mnemonic.and, F_iop_Rn) },

            { 0x83, new InstrDecoder(Mnemonic.andp, F_A_Pn) },
            { 0x93, new InstrDecoder(Mnemonic.andp, F_B_Pn) },
            { 0xA3, new InstrDecoder(Mnemonic.andp, F_iop_Pn) },

            { 0x8c, new InstrDecoder(Mnemonic.br, F_iop16) },
            { 0xac, new InstrDecoder(Mnemonic.br, F_iop16idxB) },
            { 0x9c, new InstrDecoder(Mnemonic.br, F_starRn) },

            { 0x66, new InstrDecoder(Mnemonic.btjo, F_B_A_offst) },
            { 0x16, new InstrDecoder(Mnemonic.btjo, F_Rn_A_offst) },
            { 0x36, new InstrDecoder(Mnemonic.btjo, F_Rn_B_offst) },
            { 0x46, new InstrDecoder(Mnemonic.btjo, F_Rn_Rn_offst) },
            { 0x26, new InstrDecoder(Mnemonic.btjo, F_iop_A_offst) },
            { 0x56, new InstrDecoder(Mnemonic.btjo, F_iop_B_offst) },
            { 0x76, new InstrDecoder(Mnemonic.btjo, F_iop_Rn_offst) },

            { 0x86, new InstrDecoder(Mnemonic.btjop, F_A_Pn_offst) },
            { 0x96, new InstrDecoder(Mnemonic.btjop, F_B_Pn_offst) },
            { 0xA6, new InstrDecoder(Mnemonic.btjop, F_iop_Pn_offst) },

            { 0x67, new InstrDecoder(Mnemonic.btjz, F_B_A_offst) },
            { 0x17, new InstrDecoder(Mnemonic.btjz, F_Rn_A_offst) },
            { 0x37, new InstrDecoder(Mnemonic.btjz, F_Rn_B_offst) },
            { 0x47, new InstrDecoder(Mnemonic.btjz, F_Rn_Rn_offst) },
            { 0x27, new InstrDecoder(Mnemonic.btjz, F_iop_A_offst) },
            { 0x57, new InstrDecoder(Mnemonic.btjz, F_iop_B_offst) },
            { 0x77, new InstrDecoder(Mnemonic.btjz, F_iop_Rn_offst) },

            { 0x87, new InstrDecoder(Mnemonic.btjzp, F_A_Pn_offst) },
            { 0x97, new InstrDecoder(Mnemonic.btjzp, F_B_Pn_offst) },
            { 0xA7, new InstrDecoder(Mnemonic.btjzp, F_iop_Pn_offst) },

            { 0x8e, new InstrDecoder(Mnemonic.call, F_iop16) },
            { 0xae, new InstrDecoder(Mnemonic.call, F_iop16idxB) },
            { 0x9e, new InstrDecoder(Mnemonic.call, F_starRn) },

            { 0xb5, new InstrDecoder(Mnemonic.clr, F_A) },
            { 0xc5, new InstrDecoder(Mnemonic.clr, F_B) },
            { 0xd5, new InstrDecoder(Mnemonic.clr, F_Rn) },

            { 0x6d, new InstrDecoder(Mnemonic.cmp, F_B_A) },
            { 0x1d, new InstrDecoder(Mnemonic.cmp, F_Rn_A) },
            { 0x3d, new InstrDecoder(Mnemonic.cmp, F_Rn_B) },
            { 0x4d, new InstrDecoder(Mnemonic.cmp, F_Rn_Rn) },
            { 0x2d, new InstrDecoder(Mnemonic.cmp, F_iop_A) },
            { 0x5d, new InstrDecoder(Mnemonic.cmp, F_iop_B) },
            { 0x7d, new InstrDecoder(Mnemonic.cmp, F_iop_Rn) },

            { 0x8d, new InstrDecoder(Mnemonic.cmpa, F_iop16) },
            { 0xad, new InstrDecoder(Mnemonic.cmpa, F_iop16idxB) },
            { 0x9d, new InstrDecoder(Mnemonic.cmpa, F_starRn) },

            { 0x6e, new InstrDecoder(Mnemonic.dac, F_A_B) },
            { 0x1e, new InstrDecoder(Mnemonic.dac, F_Rn_A) },
            { 0x3e, new InstrDecoder(Mnemonic.dac, F_Rn_B) },
            { 0x4e, new InstrDecoder(Mnemonic.dac, F_Rn_Rn) },
            { 0x2e, new InstrDecoder(Mnemonic.dac, F_iop_A) },
            { 0x5e, new InstrDecoder(Mnemonic.dac, F_iop_B) },
            { 0x7e, new InstrDecoder(Mnemonic.dac, F_iop_Rn) },

            { 0xb2, new InstrDecoder(Mnemonic.dec, F_A) },
            { 0xc2, new InstrDecoder(Mnemonic.dec, F_B) },
            { 0xd2, new InstrDecoder(Mnemonic.dec, F_Rn) },

            { 0xba, new InstrDecoder(Mnemonic.djnz, F_A_offst) },
            { 0xca, new InstrDecoder(Mnemonic.djnz, F_B_offst) },
            { 0xda, new InstrDecoder(Mnemonic.djnz, F_Rn_offst) },

            { 0x06, new InstrDecoder(Mnemonic.dint, F_None) },

            { 0xbb, new InstrDecoder(Mnemonic.decd, F_A) },
            { 0xcb, new InstrDecoder(Mnemonic.decd, F_B) },
            { 0xdb, new InstrDecoder(Mnemonic.decd, F_Rn) },

            { 0x6f, new InstrDecoder(Mnemonic.dsb, F_B_A) },
            { 0x1f, new InstrDecoder(Mnemonic.dsb, F_Rn_A) },
            { 0x3f, new InstrDecoder(Mnemonic.dsb, F_Rn_B) },
            { 0x4f, new InstrDecoder(Mnemonic.dsb, F_Rn_Rn) },
            { 0x2f, new InstrDecoder(Mnemonic.dsb, F_iop_A) },
            { 0x5f, new InstrDecoder(Mnemonic.dsb, F_iop_B) },
            { 0x7f, new InstrDecoder(Mnemonic.dsb, F_iop_Rn) },

            { 0x05, new InstrDecoder(Mnemonic.eint, F_None) },

            { 0x01, new InstrDecoder(Mnemonic.idle, F_None) },

            { 0xb3, new InstrDecoder(Mnemonic.inc, F_A) },
            { 0xc3, new InstrDecoder(Mnemonic.inc, F_B) },
            { 0xd3, new InstrDecoder(Mnemonic.inc, F_Rn) },

            { 0xb4, new InstrDecoder(Mnemonic.inv, F_A) },
            { 0xc4, new InstrDecoder(Mnemonic.inv, F_B) },
            { 0xd4, new InstrDecoder(Mnemonic.inv, F_Rn) },

            { 0xe0, new InstrDecoder(Mnemonic.jmp, F_offst) },
            { 0xe2, new InstrDecoder(Mnemonic.jeq, F_offst) },
            { 0xe5, new InstrDecoder(Mnemonic.jge, F_offst) },
            { 0xe4, new InstrDecoder(Mnemonic.jgt, F_offst) },
            { 0xe3, new InstrDecoder(Mnemonic.jhs, F_offst) },
            { 0xe7, new InstrDecoder(Mnemonic.jl, F_offst) },
            { 0xe6, new InstrDecoder(Mnemonic.jne, F_offst) },

            { 0x8a, new InstrDecoder(Mnemonic.lda, F_iop16) },
            { 0xaa, new InstrDecoder(Mnemonic.lda, F_iop16idxB) },
            { 0x9a, new InstrDecoder(Mnemonic.lda, F_starRn) },

            { 0x0d, new InstrDecoder(Mnemonic.ldsp, F_None) },

            { 0xc0, new InstrDecoder(Mnemonic.mov, F_A_B) },
            { 0xd0, new InstrDecoder(Mnemonic.mov, F_A_Rn) },
            { 0x62, new InstrDecoder(Mnemonic.mov, F_B_A) },
            { 0xd1, new InstrDecoder(Mnemonic.mov, F_B_Rn) },
            { 0x12, new InstrDecoder(Mnemonic.mov, F_Rn_A) },
            { 0x32, new InstrDecoder(Mnemonic.mov, F_Rn_B) },
            { 0x42, new InstrDecoder(Mnemonic.mov, F_Rn_Rn) },
            { 0x22, new InstrDecoder(Mnemonic.mov, F_iop_A) },
            { 0x52, new InstrDecoder(Mnemonic.mov, F_iop_B) },
            { 0x72, new InstrDecoder(Mnemonic.mov, F_iop_Rn) },

            { 0x88, new InstrDecoder(Mnemonic.movd, F_iop16_Rn) },
            { 0xa8, new InstrDecoder(Mnemonic.movd, F_iop16idxB_Rn) },
            { 0x98, new InstrDecoder(Mnemonic.movd, F_Rn_Rn) },

            { 0x82, new InstrDecoder(Mnemonic.movp, F_A_Pn) },
            { 0x92, new InstrDecoder(Mnemonic.movp, F_B_Pn) },
            { 0xA2, new InstrDecoder(Mnemonic.movp, F_iop_Pn) },
            { 0x80, new InstrDecoder(Mnemonic.movp, F_Pn_A) },
            { 0x91, new InstrDecoder(Mnemonic.movp, F_Pn_B) },

            { 0x6c, new InstrDecoder(Mnemonic.mpy, F_B_A) },
            { 0x1c, new InstrDecoder(Mnemonic.mpy, F_Rn_A) },
            { 0x3c, new InstrDecoder(Mnemonic.mpy, F_Rn_B) },
            { 0x4c, new InstrDecoder(Mnemonic.mpy, F_Rn_Rn) },
            { 0x2c, new InstrDecoder(Mnemonic.mpy, F_iop_A) },
            { 0x5c, new InstrDecoder(Mnemonic.mpy, F_iop_B) },
            { 0x7c, new InstrDecoder(Mnemonic.mpy, F_iop_Rn) },

            { 0x00, new InstrDecoder(Mnemonic.nop, InstrClass.Padding|InstrClass.Zero, F_None) },

            { 0x64, new InstrDecoder(Mnemonic.or, F_A_B) },
            { 0x14, new InstrDecoder(Mnemonic.or, F_Rn_A) },
            { 0x34, new InstrDecoder(Mnemonic.or, F_Rn_B) },
            { 0x44, new InstrDecoder(Mnemonic.or, F_Rn_Rn) },
            { 0x24, new InstrDecoder(Mnemonic.or, F_iop_A) },
            { 0x54, new InstrDecoder(Mnemonic.or, F_iop_B) },
            { 0x74, new InstrDecoder(Mnemonic.or, F_iop_Rn) },

            { 0x84, new InstrDecoder(Mnemonic.orp, F_A_Pn) },
            { 0x94, new InstrDecoder(Mnemonic.orp, F_B_Pn) },
            { 0xA4, new InstrDecoder(Mnemonic.orp, F_iop_Pn) },

            { 0xb9, new InstrDecoder(Mnemonic.pop, F_A) },
            { 0xc9, new InstrDecoder(Mnemonic.pop, F_B) },
            { 0xd9, new InstrDecoder(Mnemonic.pop, F_Rn) },
            { 0x08, new InstrDecoder(Mnemonic.pop, F_ST) },

            { 0xb8, new InstrDecoder(Mnemonic.push, F_A) },
            { 0xc8, new InstrDecoder(Mnemonic.push, F_B) },
            { 0xd8, new InstrDecoder(Mnemonic.push, F_Rn) },
            { 0x0e, new InstrDecoder(Mnemonic.push, F_ST) },

            { 0x0b, new InstrDecoder(Mnemonic.reti, F_None) },

            { 0x0a, new InstrDecoder(Mnemonic.rets, F_None) },

            { 0xbe, new InstrDecoder(Mnemonic.rl, F_A) },
            { 0xce, new InstrDecoder(Mnemonic.rl, F_B) },
            { 0xde, new InstrDecoder(Mnemonic.rl, F_Rn) },

            { 0xbf, new InstrDecoder(Mnemonic.rlc, F_A) },
            { 0xcf, new InstrDecoder(Mnemonic.rlc, F_B) },
            { 0xdf, new InstrDecoder(Mnemonic.rlc, F_Rn) },

            { 0xbc, new InstrDecoder(Mnemonic.rr, F_A) },
            { 0xcc, new InstrDecoder(Mnemonic.rr, F_B) },
            { 0xdc, new InstrDecoder(Mnemonic.rr, F_Rn) },

            { 0xbd, new InstrDecoder(Mnemonic.rrc, F_A) },
            { 0xcd, new InstrDecoder(Mnemonic.rrc, F_B) },
            { 0xdd, new InstrDecoder(Mnemonic.rrc, F_Rn) },

            { 0x6b, new InstrDecoder(Mnemonic.sbb, F_B_A) },
            { 0x1b, new InstrDecoder(Mnemonic.sbb, F_Rn_A) },
            { 0x3b, new InstrDecoder(Mnemonic.sbb, F_Rn_B) },
            { 0x4b, new InstrDecoder(Mnemonic.sbb, F_Rn_Rn) },
            { 0x2b, new InstrDecoder(Mnemonic.sbb, F_iop_A) },
            { 0x5b, new InstrDecoder(Mnemonic.sbb, F_iop_B) },
            { 0x7b, new InstrDecoder(Mnemonic.sbb, F_iop_Rn) },

            { 0x07, new InstrDecoder(Mnemonic.setc, F_None) },

            { 0x8b, new InstrDecoder(Mnemonic.sta, F_iop16) },
            { 0xab, new InstrDecoder(Mnemonic.sta, F_iop16idxB) },
            { 0x9b, new InstrDecoder(Mnemonic.sta, F_starRn) },

            { 0x09, new InstrDecoder(Mnemonic.stsp, F_None) },

            { 0x6a, new InstrDecoder(Mnemonic.sub, F_B_A) },
            { 0x1a, new InstrDecoder(Mnemonic.sub, F_Rn_A) },
            { 0x3a, new InstrDecoder(Mnemonic.sub, F_Rn_B) },
            { 0x4a, new InstrDecoder(Mnemonic.sub, F_Rn_Rn) },
            { 0x2a, new InstrDecoder(Mnemonic.sub, F_iop_A) },
            { 0x5a, new InstrDecoder(Mnemonic.sub, F_iop_B) },
            { 0x7a, new InstrDecoder(Mnemonic.sub, F_iop_Rn) },

            { 0xb7, new InstrDecoder(Mnemonic.swap, F_A) },
            { 0xc7, new InstrDecoder(Mnemonic.swap, F_B) },
            { 0xd7, new InstrDecoder(Mnemonic.swap, F_Rn) },

            { 0xe8, new InstrDecoder(Mnemonic.trap_0, F_None) },
            { 0xe9, new InstrDecoder(Mnemonic.trap_1, F_None) },
            { 0xea, new InstrDecoder(Mnemonic.trap_2, F_None) },
            { 0xeb, new InstrDecoder(Mnemonic.trap_3, F_None) },
            { 0xec, new InstrDecoder(Mnemonic.trap_4, F_None) },
            { 0xed, new InstrDecoder(Mnemonic.trap_5, F_None) },
            { 0xee, new InstrDecoder(Mnemonic.trap_6, F_None) },
            { 0xef, new InstrDecoder(Mnemonic.trap_7, F_None) },
            { 0xf0, new InstrDecoder(Mnemonic.trap_8, F_None) },
            { 0xf1, new InstrDecoder(Mnemonic.trap_9, F_None) },
            { 0xf2, new InstrDecoder(Mnemonic.trap_10, F_None) },
            { 0xf3, new InstrDecoder(Mnemonic.trap_11, F_None) },
            { 0xf4, new InstrDecoder(Mnemonic.trap_12, F_None) },
            { 0xf5, new InstrDecoder(Mnemonic.trap_13, F_None) },
            { 0xf6, new InstrDecoder(Mnemonic.trap_14, F_None) },
            { 0xf7, new InstrDecoder(Mnemonic.trap_15, F_None) },
            { 0xf8, new InstrDecoder(Mnemonic.trap_16, F_None) },
            { 0xf9, new InstrDecoder(Mnemonic.trap_17, F_None) },
            { 0xfa, new InstrDecoder(Mnemonic.trap_18, F_None) },
            { 0xfb, new InstrDecoder(Mnemonic.trap_19, F_None) },
            { 0xfc, new InstrDecoder(Mnemonic.trap_20, F_None) },
            { 0xfd, new InstrDecoder(Mnemonic.trap_21, F_None) },
            { 0xfe, new InstrDecoder(Mnemonic.trap_22, F_None) },
            { 0xff, new InstrDecoder(Mnemonic.trap_23, F_None) },

            { 0xb0, new InstrDecoder(Mnemonic.tsta, F_None) },

            { 0xc1, new InstrDecoder(Mnemonic.tstb, F_None) },

            { 0xb6, new InstrDecoder(Mnemonic.xchb, F_A) },
            { 0xc6, new InstrDecoder(Mnemonic.xchb, F_B) },
            { 0xd6, new InstrDecoder(Mnemonic.xchb, F_Rn) },

            { 0x65, new InstrDecoder(Mnemonic.xor, F_B_A) },
            { 0x15, new InstrDecoder(Mnemonic.xor, F_Rn_A) },
            { 0x35, new InstrDecoder(Mnemonic.xor, F_Rn_B) },
            { 0x45, new InstrDecoder(Mnemonic.xor, F_Rn_Rn) },
            { 0x25, new InstrDecoder(Mnemonic.xor, F_iop_A) },
            { 0x55, new InstrDecoder(Mnemonic.xor, F_iop_B) },
            { 0x75, new InstrDecoder(Mnemonic.xor, F_iop_Rn) },

            { 0x85, new InstrDecoder(Mnemonic.xorp, F_A_Pn) },
            { 0x95, new InstrDecoder(Mnemonic.xorp, F_B_Pn) },
            { 0xA5, new InstrDecoder(Mnemonic.xorp, F_iop_Pn) },
        };
    }
}
