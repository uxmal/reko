#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    public class Tms7000Disassembler : DisassemblerBase<Tms7000Instruction>
    {
        private Tms7000Architecture arch;
        private EndianImageReader rdr;

        public Tms7000Disassembler(Tms7000Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
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
                instr = Decode(instrDecoder.name, instrDecoder.amode);
            }
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private Tms7000Instruction Decode(Opcode opcode, string format)
        {
            int iOp = 0;
            var instr = new Tms7000Instruction
            {
                Opcode = opcode,
            };
            for (int i = 0; i < format.Length; ++i)
            {
                MachineOperand op;
                byte b;
                ushort w;
                switch (format[i])
                {
                case ',': continue;
                case 'A':
                    op = new RegisterOperand(arch.a);
                    break;
                case 'B':
                    op = new RegisterOperand(arch.b);
                    break;
                case 'R':
                    if (!rdr.TryReadByte(out b))
                        return Invalid();
                    op = new RegisterOperand(arch.GpRegs[b]);
                    break;
                case 'P':
                    if (!rdr.TryReadByte(out b))
                        return Invalid();
                    op = new RegisterOperand(arch.Ports[b]);
                    break;
                case 'S':
;                   op = new RegisterOperand(arch.st);
                    break;
                case 'i':
                    if (!rdr.TryReadByte(out b))
                        return Invalid();
                    op = ImmediateOperand.Byte(b);
                    break;
                case 'I':
                    if (!rdr.TryReadBeUInt16(out w))
                        return Invalid();
                    op = ImmediateOperand.Word16(w);
                    break;
                case 'j':   // short PC-relative jump
                    if (!rdr.TryReadByte(out b))
                        return Invalid();
                    op = AddressOperand.Create(rdr.Address + (sbyte)b);
                    break;
                case 'D':  // direct, absolute address
                    if (!rdr.TryReadBeUInt16(out w))
                        return Invalid();
                    if (i < format.Length - 1 && format[i+1] == 'B')
                    {
                        ++i;
                        op = MemoryOperand.Indexed(Address.Ptr16(w), arch.b);
                    }
                    else
                    {
                        op = MemoryOperand.Direct(Address.Ptr16(w));
                    }
                    break;
                case '*': // indirect
                    if (!rdr.TryReadByte(out b))
                        return Invalid();
                    op = MemoryOperand.Indirect(arch.GpRegs[b]);
                    break;

                default: throw new NotImplementedException($"Addressing mode {format[i]}");
                }
                switch (iOp++)
                {
                case 0: instr.op1 = op; break;
                case 1: instr.op2 = op; break;
                case 2: instr.op3 = op; break;
                }
            }
            return instr;
        }

        private static Tms7000Instruction Invalid()
        {
            return new Tms7000Instruction
            {
                Opcode = Opcode.invalid,
            };
        }

        public class Decoder
        {
            public Opcode name;
            public string amode;

            public Decoder(Opcode name, string decoder)
            {
                this.name = name;
                this.amode = decoder;
            }
            public const string F_None = "";

            public const string F_A = "A";
            public const string F_B = "B";
            public const string F_Rn = "R";
            public const string F_Rn_A = "R,A";
            public const string F_Rn_B = "R,B";
            public const string F_A_Rn = "A,R";
            public const string F_B_Rn = "B,R";
            public const string F_ST = "S";

            public const string F_iop_A = "i,A";
            public const string F_iop_B = "i,B";
            public const string F_iop_Rn = "i,R";
            public const string F_iop16_Rn = "I,R";

            public const string F_A_B = "A,B";
            public const string F_B_A = "B,A";

            public const string F_Rn_Rn = "R,R";
            public const string F_A_Pn = "A,P";
            public const string F_B_Pn = "B,P";
            public const string F_Pn_A = "P,A";
            public const string F_Pn_B = "P,B";
            public const string F_iop_Pn = "i,P";

            public const string F_iop16 = "D";
            public const string F_iop16idxB = "DB";
            public const string F_iop16idxB_Rn = "DB,R";

            public const string F_starRn = "*";

            public const string F_offst = "j";
            public const string F_A_offst = "A,j";
            public const string F_B_offst = "B,j";
            public const string F_B_A_offst = "B,A,j";
            public const string F_Rn_offst = "R,j";
            public const string F_Rn_A_offst = "R,A,j";
            public const string F_Rn_B_offst = "R,B,j";
            public const string F_Rn_Rn_offst = "R,R,j";
            public const string F_iop_A_offst = "i,A,j";
            public const string F_iop_B_offst = "i,B,j";
            public const string F_iop_Rn_offst = "i,R,j";
            public const string F_A_Pn_offst = "A,P,j";
            public const string F_B_Pn_offst = "B,P,j";
            public const string F_iop_Pn_offst = "i,P,j";
        }

        static Dictionary<uint, Decoder> decoders = new Dictionary<uint, Decoder>
        {
            { 0x69, new Decoder(Opcode.adc, Decoder.F_B_A) },
            { 0x19, new Decoder(Opcode.adc, Decoder.F_Rn_A) },
            { 0x39, new Decoder(Opcode.adc, Decoder.F_Rn_B) },
            { 0x49, new Decoder(Opcode.adc, Decoder.F_Rn_Rn) },
            { 0x29, new Decoder(Opcode.adc, Decoder.F_iop_A) },
            { 0x59, new Decoder(Opcode.adc, Decoder.F_iop_B) },
            { 0x79, new Decoder(Opcode.adc, Decoder.F_iop_Rn) },

            { 0x68, new Decoder(Opcode.add, Decoder.F_B_A) },
            { 0x18, new Decoder(Opcode.add, Decoder.F_Rn_A) },
            { 0x38, new Decoder(Opcode.add, Decoder.F_Rn_B) },
            { 0x48, new Decoder(Opcode.add, Decoder.F_Rn_Rn) },
            { 0x28, new Decoder(Opcode.add, Decoder.F_iop_A) },
            { 0x58, new Decoder(Opcode.add, Decoder.F_iop_B) },
            { 0x78, new Decoder(Opcode.add, Decoder.F_iop_Rn) },

            { 0x63, new Decoder(Opcode.and, Decoder.F_B_A) },
            { 0x13, new Decoder(Opcode.and, Decoder.F_Rn_A) },
            { 0x33, new Decoder(Opcode.and, Decoder.F_Rn_B) },
            { 0x43, new Decoder(Opcode.and, Decoder.F_Rn_Rn) },
            { 0x23, new Decoder(Opcode.and, Decoder.F_iop_A) },
            { 0x53, new Decoder(Opcode.and, Decoder.F_iop_B) },
            { 0x73, new Decoder(Opcode.and, Decoder.F_iop_Rn) },

            { 0x83, new Decoder(Opcode.andp, Decoder.F_A_Pn) },
            { 0x93, new Decoder(Opcode.andp, Decoder.F_B_Pn) },
            { 0xA3, new Decoder(Opcode.andp, Decoder.F_iop_Pn) },

            { 0x8c, new Decoder(Opcode.br, Decoder.F_iop16) },
            { 0xac, new Decoder(Opcode.br, Decoder.F_iop16idxB) },
            { 0x9c, new Decoder(Opcode.br, Decoder.F_starRn) },

            { 0x66, new Decoder(Opcode.btjo, Decoder.F_B_A_offst) },
            { 0x16, new Decoder(Opcode.btjo, Decoder.F_Rn_A_offst) },
            { 0x36, new Decoder(Opcode.btjo, Decoder.F_Rn_B_offst) },
            { 0x46, new Decoder(Opcode.btjo, Decoder.F_Rn_Rn_offst) },
            { 0x26, new Decoder(Opcode.btjo, Decoder.F_iop_A_offst) },
            { 0x56, new Decoder(Opcode.btjo, Decoder.F_iop_B_offst) },
            { 0x76, new Decoder(Opcode.btjo, Decoder.F_iop_Rn_offst) },

            { 0x86, new Decoder(Opcode.btjop, Decoder.F_A_Pn_offst) },
            { 0x96, new Decoder(Opcode.btjop, Decoder.F_B_Pn_offst) },
            { 0xA6, new Decoder(Opcode.btjop, Decoder.F_iop_Pn_offst) },

            { 0x67, new Decoder(Opcode.btjz, Decoder.F_B_A_offst) },
            { 0x17, new Decoder(Opcode.btjz, Decoder.F_Rn_A_offst) },
            { 0x37, new Decoder(Opcode.btjz, Decoder.F_Rn_B_offst) },
            { 0x47, new Decoder(Opcode.btjz, Decoder.F_Rn_Rn_offst) },
            { 0x27, new Decoder(Opcode.btjz, Decoder.F_iop_A_offst) },
            { 0x57, new Decoder(Opcode.btjz, Decoder.F_iop_B_offst) },
            { 0x77, new Decoder(Opcode.btjz, Decoder.F_iop_Rn_offst) },

            { 0x87, new Decoder(Opcode.btjzp, Decoder.F_A_Pn_offst) },
            { 0x97, new Decoder(Opcode.btjzp, Decoder.F_B_Pn_offst) },
            { 0xA7, new Decoder(Opcode.btjzp, Decoder.F_iop_Pn_offst) },

            { 0x8e, new Decoder(Opcode.call, Decoder.F_iop16) },
            { 0xae, new Decoder(Opcode.call, Decoder.F_iop16idxB) },
            { 0x9e, new Decoder(Opcode.call, Decoder.F_starRn) },

            { 0xb5, new Decoder(Opcode.clr, Decoder.F_A) },
            { 0xc5, new Decoder(Opcode.clr, Decoder.F_B) },
            { 0xd5, new Decoder(Opcode.clr, Decoder.F_Rn) },

            { 0x6d, new Decoder(Opcode.cmp, Decoder.F_B_A) },
            { 0x1d, new Decoder(Opcode.cmp, Decoder.F_Rn_A) },
            { 0x3d, new Decoder(Opcode.cmp, Decoder.F_Rn_B) },
            { 0x4d, new Decoder(Opcode.cmp, Decoder.F_Rn_Rn) },
            { 0x2d, new Decoder(Opcode.cmp, Decoder.F_iop_A) },
            { 0x5d, new Decoder(Opcode.cmp, Decoder.F_iop_B) },
            { 0x7d, new Decoder(Opcode.cmp, Decoder.F_iop_Rn) },

            { 0x8d, new Decoder(Opcode.cmpa, Decoder.F_iop16) },
            { 0xad, new Decoder(Opcode.cmpa, Decoder.F_iop16idxB) },
            { 0x9d, new Decoder(Opcode.cmpa, Decoder.F_starRn) },

            { 0x6e, new Decoder(Opcode.dac, Decoder.F_A_B) },
            { 0x1e, new Decoder(Opcode.dac, Decoder.F_Rn_A) },
            { 0x3e, new Decoder(Opcode.dac, Decoder.F_Rn_B) },
            { 0x4e, new Decoder(Opcode.dac, Decoder.F_Rn_Rn) },
            { 0x2e, new Decoder(Opcode.dac, Decoder.F_iop_A) },
            { 0x5e, new Decoder(Opcode.dac, Decoder.F_iop_B) },
            { 0x7e, new Decoder(Opcode.dac, Decoder.F_iop_Rn) },

            { 0xb2, new Decoder(Opcode.dec, Decoder.F_A) },
            { 0xc2, new Decoder(Opcode.dec, Decoder.F_B) },
            { 0xd2, new Decoder(Opcode.dec, Decoder.F_Rn) },

            { 0xba, new Decoder(Opcode.djnz, Decoder.F_A_offst) },
            { 0xca, new Decoder(Opcode.djnz, Decoder.F_B_offst) },
            { 0xda, new Decoder(Opcode.djnz, Decoder.F_Rn_offst) },

            { 0x06, new Decoder(Opcode.dint, Decoder.F_None) },

            { 0xbb, new Decoder(Opcode.decd, Decoder.F_A) },
            { 0xcb, new Decoder(Opcode.decd, Decoder.F_B) },
            { 0xdb, new Decoder(Opcode.decd, Decoder.F_Rn) },

            { 0x6f, new Decoder(Opcode.dsb, Decoder.F_B_A) },
            { 0x1f, new Decoder(Opcode.dsb, Decoder.F_Rn_A) },
            { 0x3f, new Decoder(Opcode.dsb, Decoder.F_Rn_B) },
            { 0x4f, new Decoder(Opcode.dsb, Decoder.F_Rn_Rn) },
            { 0x2f, new Decoder(Opcode.dsb, Decoder.F_iop_A) },
            { 0x5f, new Decoder(Opcode.dsb, Decoder.F_iop_B) },
            { 0x7f, new Decoder(Opcode.dsb, Decoder.F_iop_Rn) },

            { 0x05, new Decoder(Opcode.eint, Decoder.F_None) },

            { 0x01, new Decoder(Opcode.idle, Decoder.F_None) },

            { 0xb3, new Decoder(Opcode.inc, Decoder.F_A) },
            { 0xc3, new Decoder(Opcode.inc, Decoder.F_B) },
            { 0xd3, new Decoder(Opcode.inc, Decoder.F_Rn) },

            { 0xb4, new Decoder(Opcode.inv, Decoder.F_A) },
            { 0xc4, new Decoder(Opcode.inv, Decoder.F_B) },
            { 0xd4, new Decoder(Opcode.inv, Decoder.F_Rn) },

            { 0xe0, new Decoder(Opcode.jmp, Decoder.F_offst) },
            { 0xe2, new Decoder(Opcode.jeq, Decoder.F_offst) },
            { 0xe5, new Decoder(Opcode.jge, Decoder.F_offst) },
            { 0xe4, new Decoder(Opcode.jgt, Decoder.F_offst) },
            { 0xe3, new Decoder(Opcode.jhs, Decoder.F_offst) },
            { 0xe7, new Decoder(Opcode.jl, Decoder.F_offst) },
            { 0xe6, new Decoder(Opcode.jne, Decoder.F_offst) },

            { 0x8a, new Decoder(Opcode.lda, Decoder.F_iop16) },
            { 0xaa, new Decoder(Opcode.lda, Decoder.F_iop16idxB) },
            { 0x9a, new Decoder(Opcode.lda, Decoder.F_starRn) },

            { 0x0d, new Decoder(Opcode.ldsp, Decoder.F_None) },

            { 0xc0, new Decoder(Opcode.mov, Decoder.F_A_B) },
            { 0xd0, new Decoder(Opcode.mov, Decoder.F_A_Rn) },
            { 0x62, new Decoder(Opcode.mov, Decoder.F_B_A) },
            { 0xd1, new Decoder(Opcode.mov, Decoder.F_B_Rn) },
            { 0x12, new Decoder(Opcode.mov, Decoder.F_Rn_A) },
            { 0x32, new Decoder(Opcode.mov, Decoder.F_Rn_B) },
            { 0x42, new Decoder(Opcode.mov, Decoder.F_Rn_Rn) },
            { 0x22, new Decoder(Opcode.mov, Decoder.F_iop_A) },
            { 0x52, new Decoder(Opcode.mov, Decoder.F_iop_B) },
            { 0x72, new Decoder(Opcode.mov, Decoder.F_iop_Rn) },

            { 0x88, new Decoder(Opcode.movd, Decoder.F_iop16_Rn) },
            { 0xa8, new Decoder(Opcode.movd, Decoder.F_iop16idxB_Rn) },
            { 0x98, new Decoder(Opcode.movd, Decoder.F_Rn_Rn) },

            { 0x82, new Decoder(Opcode.movp, Decoder.F_A_Pn) },
            { 0x92, new Decoder(Opcode.movp, Decoder.F_B_Pn) },
            { 0xA2, new Decoder(Opcode.movp, Decoder.F_iop_Pn) },
            { 0x80, new Decoder(Opcode.movp, Decoder.F_Pn_A) },
            { 0x91, new Decoder(Opcode.movp, Decoder.F_Pn_B) },

            { 0x6c, new Decoder(Opcode.mpy, Decoder.F_B_A) },
            { 0x1c, new Decoder(Opcode.mpy, Decoder.F_Rn_A) },
            { 0x3c, new Decoder(Opcode.mpy, Decoder.F_Rn_B) },
            { 0x4c, new Decoder(Opcode.mpy, Decoder.F_Rn_Rn) },
            { 0x2c, new Decoder(Opcode.mpy, Decoder.F_iop_A) },
            { 0x5c, new Decoder(Opcode.mpy, Decoder.F_iop_B) },
            { 0x7c, new Decoder(Opcode.mpy, Decoder.F_iop_Rn) },

            { 0x00, new Decoder(Opcode.nop, Decoder.F_None) },

            { 0x64, new Decoder(Opcode.or, Decoder.F_A_B) },
            { 0x14, new Decoder(Opcode.or, Decoder.F_Rn_A) },
            { 0x34, new Decoder(Opcode.or, Decoder.F_Rn_B) },
            { 0x44, new Decoder(Opcode.or, Decoder.F_Rn_Rn) },
            { 0x24, new Decoder(Opcode.or, Decoder.F_iop_A) },
            { 0x54, new Decoder(Opcode.or, Decoder.F_iop_B) },
            { 0x74, new Decoder(Opcode.or, Decoder.F_iop_Rn) },

            { 0x84, new Decoder(Opcode.orp, Decoder.F_A_Pn) },
            { 0x94, new Decoder(Opcode.orp, Decoder.F_B_Pn) },
            { 0xA4, new Decoder(Opcode.orp, Decoder.F_iop_Pn) },

            { 0xb9, new Decoder(Opcode.pop, Decoder.F_A) },
            { 0xc9, new Decoder(Opcode.pop, Decoder.F_B) },
            { 0xd9, new Decoder(Opcode.pop, Decoder.F_Rn) },
            { 0x08, new Decoder(Opcode.pop, Decoder.F_ST) },

            { 0xb8, new Decoder(Opcode.push, Decoder.F_A) },
            { 0xc8, new Decoder(Opcode.push, Decoder.F_B) },
            { 0xd8, new Decoder(Opcode.push, Decoder.F_Rn) },
            { 0x0e, new Decoder(Opcode.push, Decoder.F_ST) },

            { 0x0b, new Decoder(Opcode.reti, Decoder.F_None) },

            { 0x0a, new Decoder(Opcode.rets, Decoder.F_None) },

            { 0xbe, new Decoder(Opcode.rl, Decoder.F_A) },
            { 0xce, new Decoder(Opcode.rl, Decoder.F_B) },
            { 0xde, new Decoder(Opcode.rl, Decoder.F_Rn) },

            { 0xbf, new Decoder(Opcode.rlc, Decoder.F_A) },
            { 0xcf, new Decoder(Opcode.rlc, Decoder.F_B) },
            { 0xdf, new Decoder(Opcode.rlc, Decoder.F_Rn) },

            { 0xbc, new Decoder(Opcode.rr, Decoder.F_A) },
            { 0xcc, new Decoder(Opcode.rr, Decoder.F_B) },
            { 0xdc, new Decoder(Opcode.rr, Decoder.F_Rn) },

            { 0xbd, new Decoder(Opcode.rrc, Decoder.F_A) },
            { 0xcd, new Decoder(Opcode.rrc, Decoder.F_B) },
            { 0xdd, new Decoder(Opcode.rrc, Decoder.F_Rn) },

            { 0x6b, new Decoder(Opcode.sbb, Decoder.F_B_A) },
            { 0x1b, new Decoder(Opcode.sbb, Decoder.F_Rn_A) },
            { 0x3b, new Decoder(Opcode.sbb, Decoder.F_Rn_B) },
            { 0x4b, new Decoder(Opcode.sbb, Decoder.F_Rn_Rn) },
            { 0x2b, new Decoder(Opcode.sbb, Decoder.F_iop_A) },
            { 0x5b, new Decoder(Opcode.sbb, Decoder.F_iop_B) },
            { 0x7b, new Decoder(Opcode.sbb, Decoder.F_iop_Rn) },

            { 0x07, new Decoder(Opcode.setc, Decoder.F_None) },

            { 0x8b, new Decoder(Opcode.sta, Decoder.F_iop16) },
            { 0xab, new Decoder(Opcode.sta, Decoder.F_iop16idxB) },
            { 0x9b, new Decoder(Opcode.sta, Decoder.F_starRn) },

            { 0x09, new Decoder(Opcode.stsp, Decoder.F_None) },

            { 0x6a, new Decoder(Opcode.sub, Decoder.F_B_A) },
            { 0x1a, new Decoder(Opcode.sub, Decoder.F_Rn_A) },
            { 0x3a, new Decoder(Opcode.sub, Decoder.F_Rn_B) },
            { 0x4a, new Decoder(Opcode.sub, Decoder.F_Rn_Rn) },
            { 0x2a, new Decoder(Opcode.sub, Decoder.F_iop_A) },
            { 0x5a, new Decoder(Opcode.sub, Decoder.F_iop_B) },
            { 0x7a, new Decoder(Opcode.sub, Decoder.F_iop_Rn) },

            { 0xb7, new Decoder(Opcode.swap, Decoder.F_A) },
            { 0xc7, new Decoder(Opcode.swap, Decoder.F_B) },
            { 0xd7, new Decoder(Opcode.swap, Decoder.F_Rn) },

            { 0xe8, new Decoder(Opcode.trap_0, Decoder.F_None) },
            { 0xe9, new Decoder(Opcode.trap_1, Decoder.F_None) },
            { 0xea, new Decoder(Opcode.trap_2, Decoder.F_None) },
            { 0xeb, new Decoder(Opcode.trap_3, Decoder.F_None) },
            { 0xec, new Decoder(Opcode.trap_4, Decoder.F_None) },
            { 0xed, new Decoder(Opcode.trap_5, Decoder.F_None) },
            { 0xee, new Decoder(Opcode.trap_6, Decoder.F_None) },
            { 0xef, new Decoder(Opcode.trap_7, Decoder.F_None) },
            { 0xf0, new Decoder(Opcode.trap_8, Decoder.F_None) },
            { 0xf1, new Decoder(Opcode.trap_9, Decoder.F_None) },
            { 0xf2, new Decoder(Opcode.trap_10, Decoder.F_None) },
            { 0xf3, new Decoder(Opcode.trap_11, Decoder.F_None) },
            { 0xf4, new Decoder(Opcode.trap_12, Decoder.F_None) },
            { 0xf5, new Decoder(Opcode.trap_13, Decoder.F_None) },
            { 0xf6, new Decoder(Opcode.trap_14, Decoder.F_None) },
            { 0xf7, new Decoder(Opcode.trap_15, Decoder.F_None) },
            { 0xf8, new Decoder(Opcode.trap_16, Decoder.F_None) },
            { 0xf9, new Decoder(Opcode.trap_17, Decoder.F_None) },
            { 0xfa, new Decoder(Opcode.trap_18, Decoder.F_None) },
            { 0xfb, new Decoder(Opcode.trap_19, Decoder.F_None) },
            { 0xfc, new Decoder(Opcode.trap_20, Decoder.F_None) },
            { 0xfd, new Decoder(Opcode.trap_21, Decoder.F_None) },
            { 0xfe, new Decoder(Opcode.trap_22, Decoder.F_None) },
            { 0xff, new Decoder(Opcode.trap_23, Decoder.F_None) },

            { 0xb0, new Decoder(Opcode.tsta, Decoder.F_None) },

            { 0xc1, new Decoder(Opcode.tstb, Decoder.F_None) },

            { 0xb6, new Decoder(Opcode.xchb, Decoder.F_A) },
            { 0xc6, new Decoder(Opcode.xchb, Decoder.F_B) },
            { 0xd6, new Decoder(Opcode.xchb, Decoder.F_Rn) },

            { 0x65, new Decoder(Opcode.xor, Decoder.F_B_A) },
            { 0x15, new Decoder(Opcode.xor, Decoder.F_Rn_A) },
            { 0x35, new Decoder(Opcode.xor, Decoder.F_Rn_B) },
            { 0x45, new Decoder(Opcode.xor, Decoder.F_Rn_Rn) },
            { 0x25, new Decoder(Opcode.xor, Decoder.F_iop_A) },
            { 0x55, new Decoder(Opcode.xor, Decoder.F_iop_B) },
            { 0x75, new Decoder(Opcode.xor, Decoder.F_iop_Rn) },

            { 0x85, new Decoder(Opcode.xorp, Decoder.F_A_Pn) },
            { 0x95, new Decoder(Opcode.xorp, Decoder.F_B_Pn) },
            { 0xA5, new Decoder(Opcode.xorp, Decoder.F_iop_Pn) },
        };
    }
}
