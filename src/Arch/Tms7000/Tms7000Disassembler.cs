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
        public delegate Tms7000Instruction formatFunc(EndianImageReader rdr, uint pc, Tms7000Disassembler dasm);

        private Tms7000Architecture arch;
        private EndianImageReader rdr;
        private Tms7000Instruction instr;

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
            if (!decoders.TryGetValue(b, out var instrDecoder))
            {
                this.instr = Invalid();
            }
            else
            {
                this.instr = new Tms7000Instruction
                {
                    Opcode = instrDecoder.name,
                };
                instr = instrDecoder.amode.argFormat(rdr, addr.ToUInt32(), this);
            }
            instr.Address = addr;
            return instr;
        }

        private Tms7000Instruction Decode(string format, params object [] args)
        {
            int iOp = 0;
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
            public int args;
            public formatFunc argFormat;

            public Decoder(int args, formatFunc argFormat)
            {
                this.args = args;
                this.argFormat = argFormat;
            }

            public static formatFunc noArgs(string format)
            {
                return (a, b, dasm) => dasm.Decode(format);
            }

            public static formatFunc oneArg(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    return dasm.Decode(format);
                };
            }

            public static formatFunc twoArgs(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    return dasm.Decode(format);
                };
            }

            public static formatFunc label16bit(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    return dasm.Decode(format);
                };
            }

            public static formatFunc iop16bitOneReg(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    var bigval = args.ReadBeUInt16();
                    var reg = args.ReadByte();
                    return dasm.Decode(format, bigval, reg);
                };
            }

            public static formatFunc pcPlusOffset(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    return dasm.Decode(format);
                };
            }

            public static formatFunc argPcPlusOffset(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    var arg = args.ReadByte();
                    var address = pc + (uint)((short)args.ReadByte());
                    return dasm.Decode(format, arg, address);

                };
            }

            public static formatFunc argArgPcPlusOffset(string format)
            {
                return (EndianImageReader args, uint pc, Tms7000Disassembler dasm) =>
                {
                    var address = pc + (uint)(int)args.ReadByte();
                    var arg1 = args.ReadByte();
                    var arg2 = args.ReadByte();
                    return dasm.Decode(format, arg1, arg2, address);
                };
            }

            public static readonly Decoder F_None = new Decoder(0, noArgs(""));

            public static readonly Decoder F_A = new Decoder(0, noArgs("A"));
            public static readonly Decoder F_B = new Decoder(0, noArgs("B"));
            public static readonly Decoder F_Rn = new Decoder(1, oneArg("R"));
            public static readonly Decoder F_Rn_A = new Decoder(1, oneArg("R,A"));
            public static readonly Decoder F_Rn_B = new Decoder(1, oneArg("R,B"));
            public static readonly Decoder F_A_Rn = new Decoder(1, oneArg("A,R"));
            public static readonly Decoder F_B_Rn = new Decoder(1, oneArg("B,R"));
            public static readonly Decoder F_ST = new Decoder(0, noArgs("S"));

            public static readonly Decoder F_iop_A = new Decoder(1, oneArg("i,A"));
            public static readonly Decoder F_iop_B = new Decoder(1, oneArg("i,B"));
            public static readonly Decoder F_iop_Rn = new Decoder(2, twoArgs("i,R"));
            public static readonly Decoder F_iop16_Rn = new Decoder(3, iop16bitOneReg("I,R"));

            public static readonly Decoder F_A_B = new Decoder(0, noArgs("A,B"));
            public static readonly Decoder F_B_A = new Decoder(0, noArgs("B,A"));

            public static readonly Decoder F_Rn_Rn = new Decoder(2, twoArgs("R,R"));
            public static readonly Decoder F_A_Pn = new Decoder(1, oneArg("A,P"));
            public static readonly Decoder F_B_Pn = new Decoder(1, oneArg("B,P"));
            public static readonly Decoder F_Pn_A = new Decoder(1, oneArg("P,A"));
            public static readonly Decoder F_Pn_B = new Decoder(1, oneArg("P,B"));
            public static readonly Decoder F_iop_Pn = new Decoder(2, twoArgs("i,P"));

            public static readonly Decoder F_iop16 = new Decoder(2, label16bit("D"));
            public static readonly Decoder F_iop16idxB = new Decoder(2, label16bit("DB"));
            public static readonly Decoder F_iop16idxB_Rn = new Decoder(3, iop16bitOneReg("DB,R"));

            public static readonly Decoder F_starRn = new Decoder(1, oneArg("*"));

            public static readonly Decoder F_offst = new Decoder(1, pcPlusOffset("j"));
            public static readonly Decoder F_A_offst = new Decoder(1, pcPlusOffset("A,j"));
            public static readonly Decoder F_B_offst = new Decoder(1, pcPlusOffset("B,j"));
            public static readonly Decoder F_B_A_offst = new Decoder(1, pcPlusOffset("B,A,j"));
            public static readonly Decoder F_Rn_offst = new Decoder(2, argPcPlusOffset("R,j"));
            public static readonly Decoder F_Rn_A_offst = new Decoder(2, argPcPlusOffset("R,A,j"));
            public static readonly Decoder F_Rn_B_offst = new Decoder(2, argPcPlusOffset("R,B,j"));
            public static readonly Decoder F_Rn_Rn_offst = new Decoder(3, argArgPcPlusOffset("R,R,j"));
            public static readonly Decoder F_iop_A_offst = new Decoder(2, argPcPlusOffset("i,A,j"));
            public static readonly Decoder F_iop_B_offst = new Decoder(2, argPcPlusOffset("i,B,j"));
            public static readonly Decoder F_iop_Rn_offst = new Decoder(3, argArgPcPlusOffset("i,R,j"));
            public static readonly Decoder F_A_Pn_offst = new Decoder(2, argPcPlusOffset("A,P,j"));
            public static readonly Decoder F_B_Pn_offst = new Decoder(2, argPcPlusOffset("B,P,j"));
            public static readonly Decoder F_iop_Pn_offst = new Decoder(3, argArgPcPlusOffset("i,P,j"));
        }
        public class Instruction
        {
            public Opcode name;
            public string comment;
            public Decoder amode;

            public Instruction(Opcode name, string comment, Decoder decoder)
            {
                this.name = name;
                this.comment = comment;
                this.amode = decoder;
            }

            public int Args()
            {
                return this.amode.args;
            }
        }

        static Dictionary<uint, Instruction> decoders = new Dictionary<uint, Instruction>
{
    { 0x69, new Instruction(Opcode.adc, "", Decoder.F_B_A) },
    { 0x19, new Instruction(Opcode.adc, "", Decoder.F_Rn_A) },
    { 0x39, new Instruction(Opcode.adc, "", Decoder.F_Rn_B) },
    { 0x49, new Instruction(Opcode.adc, "", Decoder.F_Rn_Rn) },
    { 0x29, new Instruction(Opcode.adc, "", Decoder.F_iop_A) },
    { 0x59, new Instruction(Opcode.adc, "", Decoder.F_iop_B) },
    { 0x79, new Instruction(Opcode.adc, "", Decoder.F_iop_Rn) },

    { 0x68, new Instruction(Opcode.add, "", Decoder.F_B_A) },
    { 0x18, new Instruction(Opcode.add, "", Decoder.F_Rn_A) },
    { 0x38, new Instruction(Opcode.add, "", Decoder.F_Rn_B) },
    { 0x48, new Instruction(Opcode.add, "", Decoder.F_Rn_Rn) },
    { 0x28, new Instruction(Opcode.add, "", Decoder.F_iop_A) },
    { 0x58, new Instruction(Opcode.add, "", Decoder.F_iop_B) },
    { 0x78, new Instruction(Opcode.add, "", Decoder.F_iop_Rn) },

    { 0x63, new Instruction(Opcode.and, "", Decoder.F_B_A) },
    { 0x13, new Instruction(Opcode.and, "", Decoder.F_Rn_A) },
    { 0x33, new Instruction(Opcode.and, "", Decoder.F_Rn_B) },
    { 0x43, new Instruction(Opcode.and, "", Decoder.F_Rn_Rn) },
    { 0x23, new Instruction(Opcode.and, "", Decoder.F_iop_A) },
    { 0x53, new Instruction(Opcode.and, "", Decoder.F_iop_B) },
    { 0x73, new Instruction(Opcode.and, "", Decoder.F_iop_Rn) },

    { 0x83, new Instruction(Opcode.andp, "", Decoder.F_A_Pn) },
    { 0x93, new Instruction(Opcode.andp, "", Decoder.F_B_Pn) },
    { 0xA3, new Instruction(Opcode.andp, "", Decoder.F_iop_Pn) },

    { 0x8c, new Instruction(Opcode.br, "", Decoder.F_iop16) },
    { 0xac, new Instruction(Opcode.br, "", Decoder.F_iop16idxB) },
    { 0x9c, new Instruction(Opcode.br, "", Decoder.F_starRn) },

    { 0x66, new Instruction(Opcode.btjo, "", Decoder.F_B_A_offst) },
    { 0x16, new Instruction(Opcode.btjo, "", Decoder.F_Rn_A_offst) },
    { 0x36, new Instruction(Opcode.btjo, "", Decoder.F_Rn_B_offst) },
    { 0x46, new Instruction(Opcode.btjo, "", Decoder.F_Rn_Rn_offst) },
    { 0x26, new Instruction(Opcode.btjo, "", Decoder.F_iop_A_offst) },
    { 0x56, new Instruction(Opcode.btjo, "", Decoder.F_iop_B_offst) },
    { 0x76, new Instruction(Opcode.btjo, "", Decoder.F_iop_Rn_offst) },

    { 0x86, new Instruction(Opcode.btjop, "", Decoder.F_A_Pn_offst) },
    { 0x96, new Instruction(Opcode.btjop, "", Decoder.F_B_Pn_offst) },
    { 0xA6, new Instruction(Opcode.btjop, "", Decoder.F_iop_Pn_offst) },

    { 0x67, new Instruction(Opcode.btjz, "", Decoder.F_B_A_offst) },
    { 0x17, new Instruction(Opcode.btjz, "", Decoder.F_Rn_A_offst) },
    { 0x37, new Instruction(Opcode.btjz, "", Decoder.F_Rn_B_offst) },
    { 0x47, new Instruction(Opcode.btjz, "", Decoder.F_Rn_Rn_offst) },
    { 0x27, new Instruction(Opcode.btjz, "", Decoder.F_iop_A_offst) },
    { 0x57, new Instruction(Opcode.btjz, "", Decoder.F_iop_B_offst) },
    { 0x77, new Instruction(Opcode.btjz, "", Decoder.F_iop_Rn_offst) },

    { 0x87, new Instruction(Opcode.btjzp, "", Decoder.F_A_Pn_offst) },
    { 0x97, new Instruction(Opcode.btjzp, "", Decoder.F_B_Pn_offst) },
    { 0xA7, new Instruction(Opcode.btjzp, "", Decoder.F_iop_Pn_offst) },

    { 0x8e, new Instruction(Opcode.call, "", Decoder.F_iop16) },
    { 0xae, new Instruction(Opcode.call, "", Decoder.F_iop16idxB) },
    { 0x9e, new Instruction(Opcode.call, "", Decoder.F_starRn) },

    { 0xb5, new Instruction(Opcode.clr, "", Decoder.F_A) },
    { 0xc5, new Instruction(Opcode.clr, "", Decoder.F_B) },
    { 0xd5, new Instruction(Opcode.clr, "", Decoder.F_Rn) },

    { 0x6d, new Instruction(Opcode.cmp, "", Decoder.F_B_A) },
    { 0x1d, new Instruction(Opcode.cmp, "", Decoder.F_Rn_A) },
    { 0x3d, new Instruction(Opcode.cmp, "", Decoder.F_Rn_B) },
    { 0x4d, new Instruction(Opcode.cmp, "", Decoder.F_Rn_Rn) },
    { 0x2d, new Instruction(Opcode.cmp, "", Decoder.F_iop_A) },
    { 0x5d, new Instruction(Opcode.cmp, "", Decoder.F_iop_B) },
    { 0x7d, new Instruction(Opcode.cmp, "", Decoder.F_iop_Rn) },

    { 0x8d, new Instruction(Opcode.cmpa, "", Decoder.F_iop16) },
    { 0xad, new Instruction(Opcode.cmpa, "", Decoder.F_iop16idxB) },
    { 0x9d, new Instruction(Opcode.cmpa, "", Decoder.F_starRn) },

    { 0x6e, new Instruction(Opcode.dac, "", Decoder.F_A_B) },
    { 0x1e, new Instruction(Opcode.dac, "", Decoder.F_Rn_A) },
    { 0x3e, new Instruction(Opcode.dac, "", Decoder.F_Rn_B) },
    { 0x4e, new Instruction(Opcode.dac, "", Decoder.F_Rn_Rn) },
    { 0x2e, new Instruction(Opcode.dac, "", Decoder.F_iop_A) },
    { 0x5e, new Instruction(Opcode.dac, "", Decoder.F_iop_B) },
    { 0x7e, new Instruction(Opcode.dac, "", Decoder.F_iop_Rn) },

    { 0xb2, new Instruction(Opcode.dec, "", Decoder.F_A) },
    { 0xc2, new Instruction(Opcode.dec, "", Decoder.F_B) },
    { 0xd2, new Instruction(Opcode.dec, "", Decoder.F_Rn) },

    { 0xba, new Instruction(Opcode.djnz, "", Decoder.F_A_offst) },
    { 0xca, new Instruction(Opcode.djnz, "", Decoder.F_B_offst) },
    { 0xda, new Instruction(Opcode.djnz, "", Decoder.F_Rn_offst) },

    { 0x06, new Instruction(Opcode.dint, "Clear global interrupt enable bit", Decoder.F_None) },

    { 0xbb, new Instruction(Opcode.decd, "", Decoder.F_A) },
    { 0xcb, new Instruction(Opcode.decd, "", Decoder.F_B) },
    { 0xdb, new Instruction(Opcode.decd, "", Decoder.F_Rn) },

    { 0x6f, new Instruction(Opcode.dsb, "", Decoder.F_B_A) },
    { 0x1f, new Instruction(Opcode.dsb, "", Decoder.F_Rn_A) },
    { 0x3f, new Instruction(Opcode.dsb, "", Decoder.F_Rn_B) },
    { 0x4f, new Instruction(Opcode.dsb, "", Decoder.F_Rn_Rn) },
    { 0x2f, new Instruction(Opcode.dsb, "", Decoder.F_iop_A) },
    { 0x5f, new Instruction(Opcode.dsb, "", Decoder.F_iop_B) },
    { 0x7f, new Instruction(Opcode.dsb, "", Decoder.F_iop_Rn) },

    { 0x05, new Instruction(Opcode.eint, "Set global interrupt enable bit", Decoder.F_None) },

    { 0x01, new Instruction(Opcode.idle, "Sleep until interrupt", Decoder.F_None) },

    { 0xb3, new Instruction(Opcode.inc, "", Decoder.F_A) },
    { 0xc3, new Instruction(Opcode.inc, "", Decoder.F_B) },
    { 0xd3, new Instruction(Opcode.inc, "", Decoder.F_Rn) },

    { 0xb4, new Instruction(Opcode.inv, "", Decoder.F_A) },
    { 0xc4, new Instruction(Opcode.inv, "", Decoder.F_B) },
    { 0xd4, new Instruction(Opcode.inv, "", Decoder.F_Rn) },

    { 0xe0, new Instruction(Opcode.jmp, "", Decoder.F_offst) },
    { 0xe2, new Instruction(Opcode.jeq, "", Decoder.F_offst) },
    { 0xe5, new Instruction(Opcode.jge, "", Decoder.F_offst) },
    { 0xe4, new Instruction(Opcode.jgt, "", Decoder.F_offst) },
    { 0xe3, new Instruction(Opcode.jhs, "", Decoder.F_offst) },
    { 0xe7, new Instruction(Opcode.jl, "", Decoder.F_offst) },
    { 0xe6, new Instruction(Opcode.jne, "", Decoder.F_offst) },

    { 0x8a, new Instruction(Opcode.lda, "", Decoder.F_iop16) },
    { 0xaa, new Instruction(Opcode.lda, "", Decoder.F_iop16idxB) },
    { 0x9a, new Instruction(Opcode.lda, "", Decoder.F_starRn) },

    { 0x0d, new Instruction(Opcode.ldsp, "(B) -> (SP)", Decoder.F_None) },

    { 0xc0, new Instruction(Opcode.mov, "", Decoder.F_A_B) },
    { 0xd0, new Instruction(Opcode.mov, "", Decoder.F_A_Rn) },
    { 0x62, new Instruction(Opcode.mov, "", Decoder.F_B_A) },
    { 0xd1, new Instruction(Opcode.mov, "", Decoder.F_B_Rn) },
    { 0x12, new Instruction(Opcode.mov, "", Decoder.F_Rn_A) },
    { 0x32, new Instruction(Opcode.mov, "", Decoder.F_Rn_B) },
    { 0x42, new Instruction(Opcode.mov, "", Decoder.F_Rn_Rn) },
    { 0x22, new Instruction(Opcode.mov, "", Decoder.F_iop_A) },
    { 0x52, new Instruction(Opcode.mov, "", Decoder.F_iop_B) },
    { 0x72, new Instruction(Opcode.mov, "", Decoder.F_iop_Rn) },

    { 0x88, new Instruction(Opcode.movd, "", Decoder.F_iop16_Rn) },
    { 0xa8, new Instruction(Opcode.movd, "", Decoder.F_iop16idxB_Rn) },
    { 0x98, new Instruction(Opcode.movd, "", Decoder.F_Rn_Rn) },

    { 0x82, new Instruction(Opcode.movp, "", Decoder.F_A_Pn) },
    { 0x92, new Instruction(Opcode.movp, "", Decoder.F_B_Pn) },
    { 0xA2, new Instruction(Opcode.movp, "", Decoder.F_iop_Pn) },
    { 0x80, new Instruction(Opcode.movp, "", Decoder.F_Pn_A) },
    { 0x91, new Instruction(Opcode.movp, "", Decoder.F_Pn_B) },

    { 0x6c, new Instruction(Opcode.mov, "", Decoder.F_B_A) },
    { 0x1c, new Instruction(Opcode.mov, "", Decoder.F_Rn_A) },
    { 0x3c, new Instruction(Opcode.mov, "", Decoder.F_Rn_B) },
    { 0x4c, new Instruction(Opcode.mov, "", Decoder.F_Rn_Rn) },
    { 0x2c, new Instruction(Opcode.mov, "", Decoder.F_iop_A) },
    { 0x5c, new Instruction(Opcode.mov, "", Decoder.F_iop_B) },
    { 0x7c, new Instruction(Opcode.mov, "", Decoder.F_iop_Rn) },

    { 0x00, new Instruction(Opcode.nop, "", Decoder.F_None) },

    { 0x64, new Instruction(Opcode.or, "", Decoder.F_A_B) },
    { 0x14, new Instruction(Opcode.or, "", Decoder.F_Rn_A) },
    { 0x34, new Instruction(Opcode.or, "", Decoder.F_Rn_B) },
    { 0x44, new Instruction(Opcode.or, "", Decoder.F_Rn_Rn) },
    { 0x24, new Instruction(Opcode.or, "", Decoder.F_iop_A) },
    { 0x54, new Instruction(Opcode.or, "", Decoder.F_iop_B) },
    { 0x74, new Instruction(Opcode.or, "", Decoder.F_iop_Rn) },

    { 0x84, new Instruction(Opcode.orp, "", Decoder.F_A_Pn) },
    { 0x94, new Instruction(Opcode.orp, "", Decoder.F_B_Pn) },
    { 0xA4, new Instruction(Opcode.orp, "", Decoder.F_iop_Pn) },

    { 0xb9, new Instruction(Opcode.pop, "", Decoder.F_A) },
    { 0xc9, new Instruction(Opcode.pop, "", Decoder.F_B) },
    { 0xd9, new Instruction(Opcode.pop, "", Decoder.F_Rn) },
    { 0x08, new Instruction(Opcode.pop, "", Decoder.F_ST) },

    { 0xb8, new Instruction(Opcode.push, "", Decoder.F_A) },
    { 0xc8, new Instruction(Opcode.push, "", Decoder.F_B) },
    { 0xd8, new Instruction(Opcode.push, "", Decoder.F_Rn) },
    { 0x0e, new Instruction(Opcode.push, "", Decoder.F_ST) },

    { 0x0b, new Instruction(Opcode.reti, "return from interrupt", Decoder.F_None) },

    { 0x0a, new Instruction(Opcode.rets, "return from subroutine", Decoder.F_None) },

    { 0xbe, new Instruction(Opcode.rl, "", Decoder.F_A) },
    { 0xce, new Instruction(Opcode.rl, "", Decoder.F_B) },
    { 0xde, new Instruction(Opcode.rl, "", Decoder.F_Rn) },

    { 0xbf, new Instruction(Opcode.rlc, "", Decoder.F_A) },
    { 0xcf, new Instruction(Opcode.rlc, "", Decoder.F_B) },
    { 0xdf, new Instruction(Opcode.rlc, "", Decoder.F_Rn) },

    { 0xbc, new Instruction(Opcode.rr, "", Decoder.F_A) },
    { 0xcc, new Instruction(Opcode.rr, "", Decoder.F_B) },
    { 0xdc, new Instruction(Opcode.rr, "", Decoder.F_Rn) },

    { 0xbd, new Instruction(Opcode.rrc, "", Decoder.F_A) },
    { 0xcd, new Instruction(Opcode.rrc, "", Decoder.F_B) },
    { 0xdd, new Instruction(Opcode.rrc, "", Decoder.F_Rn) },

    { 0x6b, new Instruction(Opcode.sbb, "", Decoder.F_B_A) },
    { 0x1b, new Instruction(Opcode.sbb, "", Decoder.F_Rn_A) },
    { 0x3b, new Instruction(Opcode.sbb, "", Decoder.F_Rn_B) },
    { 0x4b, new Instruction(Opcode.sbb, "", Decoder.F_Rn_Rn) },
    { 0x2b, new Instruction(Opcode.sbb, "", Decoder.F_iop_A) },
    { 0x5b, new Instruction(Opcode.sbb, "", Decoder.F_iop_B) },
    { 0x7b, new Instruction(Opcode.sbb, "", Decoder.F_iop_Rn) },

    { 0x07, new Instruction(Opcode.setc, "", Decoder.F_None) },

    { 0x8b, new Instruction(Opcode.sta, "", Decoder.F_iop16) },
    { 0xab, new Instruction(Opcode.sta, "", Decoder.F_iop16idxB) },
    { 0x9b, new Instruction(Opcode.sta, "", Decoder.F_starRn) },

    { 0x09, new Instruction(Opcode.stsp, "(SP) -> (B)", Decoder.F_None) },

    { 0x6a, new Instruction(Opcode.sub, "", Decoder.F_B_A) },
    { 0x1a, new Instruction(Opcode.sub, "", Decoder.F_Rn_A) },
    { 0x3a, new Instruction(Opcode.sub, "", Decoder.F_Rn_B) },
    { 0x4a, new Instruction(Opcode.sub, "", Decoder.F_Rn_Rn) },
    { 0x2a, new Instruction(Opcode.sub, "", Decoder.F_iop_A) },
    { 0x5a, new Instruction(Opcode.sub, "", Decoder.F_iop_B) },
    { 0x7a, new Instruction(Opcode.sub, "", Decoder.F_iop_Rn) },

    { 0xb7, new Instruction(Opcode.swap, "", Decoder.F_A) },
    { 0xc7, new Instruction(Opcode.swap, "", Decoder.F_B) },
    { 0xd7, new Instruction(Opcode.swap, "", Decoder.F_Rn) },

    { 0xe8, new Instruction(Opcode.trap_0, "", Decoder.F_None) },
    { 0xe9, new Instruction(Opcode.trap_1, "", Decoder.F_None) },
    { 0xea, new Instruction(Opcode.trap_2, "", Decoder.F_None) },
    { 0xeb, new Instruction(Opcode.trap_3, "", Decoder.F_None) },
    { 0xec, new Instruction(Opcode.trap_4, "", Decoder.F_None) },
    { 0xed, new Instruction(Opcode.trap_5, "", Decoder.F_None) },
    { 0xee, new Instruction(Opcode.trap_6, "", Decoder.F_None) },
    { 0xef, new Instruction(Opcode.trap_7, "", Decoder.F_None) },
    { 0xf0, new Instruction(Opcode.trap_8, "", Decoder.F_None) },
    { 0xf1, new Instruction(Opcode.trap_9, "", Decoder.F_None) },
    { 0xf2, new Instruction(Opcode.trap_10, "", Decoder.F_None) },
    { 0xf3, new Instruction(Opcode.trap_11, "", Decoder.F_None) },
    { 0xf4, new Instruction(Opcode.trap_12, "", Decoder.F_None) },
    { 0xf5, new Instruction(Opcode.trap_13, "", Decoder.F_None) },
    { 0xf6, new Instruction(Opcode.trap_14, "", Decoder.F_None) },
    { 0xf7, new Instruction(Opcode.trap_15, "", Decoder.F_None) },
    { 0xf8, new Instruction(Opcode.trap_16, "", Decoder.F_None) },
    { 0xf9, new Instruction(Opcode.trap_17, "", Decoder.F_None) },
    { 0xfa, new Instruction(Opcode.trap_18, "", Decoder.F_None) },
    { 0xfb, new Instruction(Opcode.trap_19, "", Decoder.F_None) },
    { 0xfc, new Instruction(Opcode.trap_20, "", Decoder.F_None) },
    { 0xfd, new Instruction(Opcode.trap_21, "", Decoder.F_None) },
    { 0xfe, new Instruction(Opcode.trap_22, "", Decoder.F_None) },
    { 0xff, new Instruction(Opcode.trap_23, "", Decoder.F_None) },

    { 0xb0, new Instruction(Opcode.tsta, "", Decoder.F_None) },

    { 0xc1, new Instruction(Opcode.tstb, "", Decoder.F_None) },

    { 0xb6, new Instruction(Opcode.xchb, "", Decoder.F_A) },
    { 0xc6, new Instruction(Opcode.xchb, "", Decoder.F_B) },
    { 0xd6, new Instruction(Opcode.xchb, "", Decoder.F_Rn) },

    { 0x65, new Instruction(Opcode.xor, "", Decoder.F_B_A) },
    { 0x15, new Instruction(Opcode.xor, "", Decoder.F_Rn_A) },
    { 0x35, new Instruction(Opcode.xor, "", Decoder.F_Rn_B) },
    { 0x45, new Instruction(Opcode.xor, "", Decoder.F_Rn_Rn) },
    { 0x25, new Instruction(Opcode.xor, "", Decoder.F_iop_A) },
    { 0x55, new Instruction(Opcode.xor, "", Decoder.F_iop_B) },
    { 0x75, new Instruction(Opcode.xor, "", Decoder.F_iop_Rn) },

    { 0x85, new Instruction(Opcode.xorp, "", Decoder.F_A_Pn) },
    { 0x95, new Instruction(Opcode.xorp, "", Decoder.F_B_Pn) },
    { 0xA5, new Instruction(Opcode.xorp, "", Decoder.F_iop_Pn) },
};
    }
}
