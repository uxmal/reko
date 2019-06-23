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
                instr = Invalid();
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

        private static Tms7000Instruction Invalid()
        {
            return new Tms7000Instruction
            {
                Opcode = Opcode.invalid,
            };
        }

        private class Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly Mutator<Tms7000Disassembler>[] mutators;

            public Decoder(Opcode opcode, params Mutator<Tms7000Disassembler>[] mutators) : this(opcode, InstrClass.Linear, mutators)
            {
            }

            public Decoder(Opcode opcode, InstrClass iclass, params Mutator<Tms7000Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public Tms7000Instruction Decode(byte b, Tms7000Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return Invalid();
                }
                var instr = new Tms7000Instruction
                {
                    Opcode = opcode,
                    InstructionClass = iclass,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
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
            { 0x69, new Decoder(Opcode.adc, F_B_A) },
            { 0x19, new Decoder(Opcode.adc, F_Rn_A) },
            { 0x39, new Decoder(Opcode.adc, F_Rn_B) },
            { 0x49, new Decoder(Opcode.adc, F_Rn_Rn) },
            { 0x29, new Decoder(Opcode.adc, F_iop_A) },
            { 0x59, new Decoder(Opcode.adc, F_iop_B) },
            { 0x79, new Decoder(Opcode.adc, F_iop_Rn) },

            { 0x68, new Decoder(Opcode.add, F_B_A) },
            { 0x18, new Decoder(Opcode.add, F_Rn_A) },
            { 0x38, new Decoder(Opcode.add, F_Rn_B) },
            { 0x48, new Decoder(Opcode.add, F_Rn_Rn) },
            { 0x28, new Decoder(Opcode.add, F_iop_A) },
            { 0x58, new Decoder(Opcode.add, F_iop_B) },
            { 0x78, new Decoder(Opcode.add, F_iop_Rn) },

            { 0x63, new Decoder(Opcode.and, F_B_A) },
            { 0x13, new Decoder(Opcode.and, F_Rn_A) },
            { 0x33, new Decoder(Opcode.and, F_Rn_B) },
            { 0x43, new Decoder(Opcode.and, F_Rn_Rn) },
            { 0x23, new Decoder(Opcode.and, F_iop_A) },
            { 0x53, new Decoder(Opcode.and, F_iop_B) },
            { 0x73, new Decoder(Opcode.and, F_iop_Rn) },

            { 0x83, new Decoder(Opcode.andp, F_A_Pn) },
            { 0x93, new Decoder(Opcode.andp, F_B_Pn) },
            { 0xA3, new Decoder(Opcode.andp, F_iop_Pn) },

            { 0x8c, new Decoder(Opcode.br, F_iop16) },
            { 0xac, new Decoder(Opcode.br, F_iop16idxB) },
            { 0x9c, new Decoder(Opcode.br, F_starRn) },

            { 0x66, new Decoder(Opcode.btjo, F_B_A_offst) },
            { 0x16, new Decoder(Opcode.btjo, F_Rn_A_offst) },
            { 0x36, new Decoder(Opcode.btjo, F_Rn_B_offst) },
            { 0x46, new Decoder(Opcode.btjo, F_Rn_Rn_offst) },
            { 0x26, new Decoder(Opcode.btjo, F_iop_A_offst) },
            { 0x56, new Decoder(Opcode.btjo, F_iop_B_offst) },
            { 0x76, new Decoder(Opcode.btjo, F_iop_Rn_offst) },

            { 0x86, new Decoder(Opcode.btjop, F_A_Pn_offst) },
            { 0x96, new Decoder(Opcode.btjop, F_B_Pn_offst) },
            { 0xA6, new Decoder(Opcode.btjop, F_iop_Pn_offst) },

            { 0x67, new Decoder(Opcode.btjz, F_B_A_offst) },
            { 0x17, new Decoder(Opcode.btjz, F_Rn_A_offst) },
            { 0x37, new Decoder(Opcode.btjz, F_Rn_B_offst) },
            { 0x47, new Decoder(Opcode.btjz, F_Rn_Rn_offst) },
            { 0x27, new Decoder(Opcode.btjz, F_iop_A_offst) },
            { 0x57, new Decoder(Opcode.btjz, F_iop_B_offst) },
            { 0x77, new Decoder(Opcode.btjz, F_iop_Rn_offst) },

            { 0x87, new Decoder(Opcode.btjzp, F_A_Pn_offst) },
            { 0x97, new Decoder(Opcode.btjzp, F_B_Pn_offst) },
            { 0xA7, new Decoder(Opcode.btjzp, F_iop_Pn_offst) },

            { 0x8e, new Decoder(Opcode.call, F_iop16) },
            { 0xae, new Decoder(Opcode.call, F_iop16idxB) },
            { 0x9e, new Decoder(Opcode.call, F_starRn) },

            { 0xb5, new Decoder(Opcode.clr, F_A) },
            { 0xc5, new Decoder(Opcode.clr, F_B) },
            { 0xd5, new Decoder(Opcode.clr, F_Rn) },

            { 0x6d, new Decoder(Opcode.cmp, F_B_A) },
            { 0x1d, new Decoder(Opcode.cmp, F_Rn_A) },
            { 0x3d, new Decoder(Opcode.cmp, F_Rn_B) },
            { 0x4d, new Decoder(Opcode.cmp, F_Rn_Rn) },
            { 0x2d, new Decoder(Opcode.cmp, F_iop_A) },
            { 0x5d, new Decoder(Opcode.cmp, F_iop_B) },
            { 0x7d, new Decoder(Opcode.cmp, F_iop_Rn) },

            { 0x8d, new Decoder(Opcode.cmpa, F_iop16) },
            { 0xad, new Decoder(Opcode.cmpa, F_iop16idxB) },
            { 0x9d, new Decoder(Opcode.cmpa, F_starRn) },

            { 0x6e, new Decoder(Opcode.dac, F_A_B) },
            { 0x1e, new Decoder(Opcode.dac, F_Rn_A) },
            { 0x3e, new Decoder(Opcode.dac, F_Rn_B) },
            { 0x4e, new Decoder(Opcode.dac, F_Rn_Rn) },
            { 0x2e, new Decoder(Opcode.dac, F_iop_A) },
            { 0x5e, new Decoder(Opcode.dac, F_iop_B) },
            { 0x7e, new Decoder(Opcode.dac, F_iop_Rn) },

            { 0xb2, new Decoder(Opcode.dec, F_A) },
            { 0xc2, new Decoder(Opcode.dec, F_B) },
            { 0xd2, new Decoder(Opcode.dec, F_Rn) },

            { 0xba, new Decoder(Opcode.djnz, F_A_offst) },
            { 0xca, new Decoder(Opcode.djnz, F_B_offst) },
            { 0xda, new Decoder(Opcode.djnz, F_Rn_offst) },

            { 0x06, new Decoder(Opcode.dint, F_None) },

            { 0xbb, new Decoder(Opcode.decd, F_A) },
            { 0xcb, new Decoder(Opcode.decd, F_B) },
            { 0xdb, new Decoder(Opcode.decd, F_Rn) },

            { 0x6f, new Decoder(Opcode.dsb, F_B_A) },
            { 0x1f, new Decoder(Opcode.dsb, F_Rn_A) },
            { 0x3f, new Decoder(Opcode.dsb, F_Rn_B) },
            { 0x4f, new Decoder(Opcode.dsb, F_Rn_Rn) },
            { 0x2f, new Decoder(Opcode.dsb, F_iop_A) },
            { 0x5f, new Decoder(Opcode.dsb, F_iop_B) },
            { 0x7f, new Decoder(Opcode.dsb, F_iop_Rn) },

            { 0x05, new Decoder(Opcode.eint, F_None) },

            { 0x01, new Decoder(Opcode.idle, F_None) },

            { 0xb3, new Decoder(Opcode.inc, F_A) },
            { 0xc3, new Decoder(Opcode.inc, F_B) },
            { 0xd3, new Decoder(Opcode.inc, F_Rn) },

            { 0xb4, new Decoder(Opcode.inv, F_A) },
            { 0xc4, new Decoder(Opcode.inv, F_B) },
            { 0xd4, new Decoder(Opcode.inv, F_Rn) },

            { 0xe0, new Decoder(Opcode.jmp, F_offst) },
            { 0xe2, new Decoder(Opcode.jeq, F_offst) },
            { 0xe5, new Decoder(Opcode.jge, F_offst) },
            { 0xe4, new Decoder(Opcode.jgt, F_offst) },
            { 0xe3, new Decoder(Opcode.jhs, F_offst) },
            { 0xe7, new Decoder(Opcode.jl, F_offst) },
            { 0xe6, new Decoder(Opcode.jne, F_offst) },

            { 0x8a, new Decoder(Opcode.lda, F_iop16) },
            { 0xaa, new Decoder(Opcode.lda, F_iop16idxB) },
            { 0x9a, new Decoder(Opcode.lda, F_starRn) },

            { 0x0d, new Decoder(Opcode.ldsp, F_None) },

            { 0xc0, new Decoder(Opcode.mov, F_A_B) },
            { 0xd0, new Decoder(Opcode.mov, F_A_Rn) },
            { 0x62, new Decoder(Opcode.mov, F_B_A) },
            { 0xd1, new Decoder(Opcode.mov, F_B_Rn) },
            { 0x12, new Decoder(Opcode.mov, F_Rn_A) },
            { 0x32, new Decoder(Opcode.mov, F_Rn_B) },
            { 0x42, new Decoder(Opcode.mov, F_Rn_Rn) },
            { 0x22, new Decoder(Opcode.mov, F_iop_A) },
            { 0x52, new Decoder(Opcode.mov, F_iop_B) },
            { 0x72, new Decoder(Opcode.mov, F_iop_Rn) },

            { 0x88, new Decoder(Opcode.movd, F_iop16_Rn) },
            { 0xa8, new Decoder(Opcode.movd, F_iop16idxB_Rn) },
            { 0x98, new Decoder(Opcode.movd, F_Rn_Rn) },

            { 0x82, new Decoder(Opcode.movp, F_A_Pn) },
            { 0x92, new Decoder(Opcode.movp, F_B_Pn) },
            { 0xA2, new Decoder(Opcode.movp, F_iop_Pn) },
            { 0x80, new Decoder(Opcode.movp, F_Pn_A) },
            { 0x91, new Decoder(Opcode.movp, F_Pn_B) },

            { 0x6c, new Decoder(Opcode.mpy, F_B_A) },
            { 0x1c, new Decoder(Opcode.mpy, F_Rn_A) },
            { 0x3c, new Decoder(Opcode.mpy, F_Rn_B) },
            { 0x4c, new Decoder(Opcode.mpy, F_Rn_Rn) },
            { 0x2c, new Decoder(Opcode.mpy, F_iop_A) },
            { 0x5c, new Decoder(Opcode.mpy, F_iop_B) },
            { 0x7c, new Decoder(Opcode.mpy, F_iop_Rn) },

            { 0x00, new Decoder(Opcode.nop, InstrClass.Padding|InstrClass.Zero, F_None) },

            { 0x64, new Decoder(Opcode.or, F_A_B) },
            { 0x14, new Decoder(Opcode.or, F_Rn_A) },
            { 0x34, new Decoder(Opcode.or, F_Rn_B) },
            { 0x44, new Decoder(Opcode.or, F_Rn_Rn) },
            { 0x24, new Decoder(Opcode.or, F_iop_A) },
            { 0x54, new Decoder(Opcode.or, F_iop_B) },
            { 0x74, new Decoder(Opcode.or, F_iop_Rn) },

            { 0x84, new Decoder(Opcode.orp, F_A_Pn) },
            { 0x94, new Decoder(Opcode.orp, F_B_Pn) },
            { 0xA4, new Decoder(Opcode.orp, F_iop_Pn) },

            { 0xb9, new Decoder(Opcode.pop, F_A) },
            { 0xc9, new Decoder(Opcode.pop, F_B) },
            { 0xd9, new Decoder(Opcode.pop, F_Rn) },
            { 0x08, new Decoder(Opcode.pop, F_ST) },

            { 0xb8, new Decoder(Opcode.push, F_A) },
            { 0xc8, new Decoder(Opcode.push, F_B) },
            { 0xd8, new Decoder(Opcode.push, F_Rn) },
            { 0x0e, new Decoder(Opcode.push, F_ST) },

            { 0x0b, new Decoder(Opcode.reti, F_None) },

            { 0x0a, new Decoder(Opcode.rets, F_None) },

            { 0xbe, new Decoder(Opcode.rl, F_A) },
            { 0xce, new Decoder(Opcode.rl, F_B) },
            { 0xde, new Decoder(Opcode.rl, F_Rn) },

            { 0xbf, new Decoder(Opcode.rlc, F_A) },
            { 0xcf, new Decoder(Opcode.rlc, F_B) },
            { 0xdf, new Decoder(Opcode.rlc, F_Rn) },

            { 0xbc, new Decoder(Opcode.rr, F_A) },
            { 0xcc, new Decoder(Opcode.rr, F_B) },
            { 0xdc, new Decoder(Opcode.rr, F_Rn) },

            { 0xbd, new Decoder(Opcode.rrc, F_A) },
            { 0xcd, new Decoder(Opcode.rrc, F_B) },
            { 0xdd, new Decoder(Opcode.rrc, F_Rn) },

            { 0x6b, new Decoder(Opcode.sbb, F_B_A) },
            { 0x1b, new Decoder(Opcode.sbb, F_Rn_A) },
            { 0x3b, new Decoder(Opcode.sbb, F_Rn_B) },
            { 0x4b, new Decoder(Opcode.sbb, F_Rn_Rn) },
            { 0x2b, new Decoder(Opcode.sbb, F_iop_A) },
            { 0x5b, new Decoder(Opcode.sbb, F_iop_B) },
            { 0x7b, new Decoder(Opcode.sbb, F_iop_Rn) },

            { 0x07, new Decoder(Opcode.setc, F_None) },

            { 0x8b, new Decoder(Opcode.sta, F_iop16) },
            { 0xab, new Decoder(Opcode.sta, F_iop16idxB) },
            { 0x9b, new Decoder(Opcode.sta, F_starRn) },

            { 0x09, new Decoder(Opcode.stsp, F_None) },

            { 0x6a, new Decoder(Opcode.sub, F_B_A) },
            { 0x1a, new Decoder(Opcode.sub, F_Rn_A) },
            { 0x3a, new Decoder(Opcode.sub, F_Rn_B) },
            { 0x4a, new Decoder(Opcode.sub, F_Rn_Rn) },
            { 0x2a, new Decoder(Opcode.sub, F_iop_A) },
            { 0x5a, new Decoder(Opcode.sub, F_iop_B) },
            { 0x7a, new Decoder(Opcode.sub, F_iop_Rn) },

            { 0xb7, new Decoder(Opcode.swap, F_A) },
            { 0xc7, new Decoder(Opcode.swap, F_B) },
            { 0xd7, new Decoder(Opcode.swap, F_Rn) },

            { 0xe8, new Decoder(Opcode.trap_0, F_None) },
            { 0xe9, new Decoder(Opcode.trap_1, F_None) },
            { 0xea, new Decoder(Opcode.trap_2, F_None) },
            { 0xeb, new Decoder(Opcode.trap_3, F_None) },
            { 0xec, new Decoder(Opcode.trap_4, F_None) },
            { 0xed, new Decoder(Opcode.trap_5, F_None) },
            { 0xee, new Decoder(Opcode.trap_6, F_None) },
            { 0xef, new Decoder(Opcode.trap_7, F_None) },
            { 0xf0, new Decoder(Opcode.trap_8, F_None) },
            { 0xf1, new Decoder(Opcode.trap_9, F_None) },
            { 0xf2, new Decoder(Opcode.trap_10, F_None) },
            { 0xf3, new Decoder(Opcode.trap_11, F_None) },
            { 0xf4, new Decoder(Opcode.trap_12, F_None) },
            { 0xf5, new Decoder(Opcode.trap_13, F_None) },
            { 0xf6, new Decoder(Opcode.trap_14, F_None) },
            { 0xf7, new Decoder(Opcode.trap_15, F_None) },
            { 0xf8, new Decoder(Opcode.trap_16, F_None) },
            { 0xf9, new Decoder(Opcode.trap_17, F_None) },
            { 0xfa, new Decoder(Opcode.trap_18, F_None) },
            { 0xfb, new Decoder(Opcode.trap_19, F_None) },
            { 0xfc, new Decoder(Opcode.trap_20, F_None) },
            { 0xfd, new Decoder(Opcode.trap_21, F_None) },
            { 0xfe, new Decoder(Opcode.trap_22, F_None) },
            { 0xff, new Decoder(Opcode.trap_23, F_None) },

            { 0xb0, new Decoder(Opcode.tsta, F_None) },

            { 0xc1, new Decoder(Opcode.tstb, F_None) },

            { 0xb6, new Decoder(Opcode.xchb, F_A) },
            { 0xc6, new Decoder(Opcode.xchb, F_B) },
            { 0xd6, new Decoder(Opcode.xchb, F_Rn) },

            { 0x65, new Decoder(Opcode.xor, F_B_A) },
            { 0x15, new Decoder(Opcode.xor, F_Rn_A) },
            { 0x35, new Decoder(Opcode.xor, F_Rn_B) },
            { 0x45, new Decoder(Opcode.xor, F_Rn_Rn) },
            { 0x25, new Decoder(Opcode.xor, F_iop_A) },
            { 0x55, new Decoder(Opcode.xor, F_iop_B) },
            { 0x75, new Decoder(Opcode.xor, F_iop_Rn) },

            { 0x85, new Decoder(Opcode.xorp, F_A_Pn) },
            { 0x95, new Decoder(Opcode.xorp, F_B_Pn) },
            { 0xA5, new Decoder(Opcode.xorp, F_iop_Pn) },
        };
    }
}
