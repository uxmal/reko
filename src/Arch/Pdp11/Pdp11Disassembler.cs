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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.Pdp11
{
    using Decoder = Decoder<Pdp11Disassembler, Mnemonic, Pdp11Instruction>;

    public class Pdp11Disassembler : DisassemblerBase<Pdp11Instruction>
    {
        private readonly Pdp11Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Pdp11Instruction instrCur;
        private PrimitiveType dataWidth;

        public Pdp11Disassembler(EndianImageReader rdr, Pdp11Architecture arch)
        {
            this.rdr = rdr;
            this.arch = arch;
            this.ops = new List<MachineOperand>(2);
        }

        public override Pdp11Instruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort opcode))
                return null;
            ops.Clear();
            dataWidth = PrimitiveType.Word16;
            var decoder = decoders[(opcode >> 0x0C) & 0x00F];
            if (decoder != null)
            {
                instrCur = decoder.Decode(opcode, this);
            }
            else
            {
                switch ((opcode >> 0x0C) & 0x007)
                {
                case 0: instrCur = NonDoubleOperandInstruction(opcode, this); break;
                case 7: instrCur = extraDecoders[(opcode >> 0x09) & 7].Decode(opcode, this); break;
                default: throw new NotImplementedException();
                }
            }
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        protected override Pdp11Instruction CreateInvalidInstruction()
        {
            return new Pdp11Instruction
            {
                Mnemonic = Mnemonic.illegal,
                InstructionClass = InstrClass.Invalid,
                Operands = new MachineOperand[0],
            };
        }

        #region Mutators
        private static bool b(uint uInstr, Pdp11Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Byte;
            return true;
        }
        private static bool w(uint uInstr, Pdp11Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word16;
            return true;
        }

        private static bool E(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.DecodeOperand(wOpcode);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool e(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.DecodeOperand(wOpcode >> 6);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool r(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = new RegisterOperand(dasm.arch.GetRegister(((int)wOpcode >> 6) & 7));
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool I(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.Imm6((ushort)wOpcode);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool F(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.DecodeOperand(wOpcode, true);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool f(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.FpuAccumulator(wOpcode);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }
        
        #endregion


        class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic opcode;
            private readonly Mutator<Pdp11Disassembler>[] mutators;

            public InstrDecoder(Mnemonic op, InstrClass iclass, params Mutator<Pdp11Disassembler>[] mutators)
            {
                this.opcode = op;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override Pdp11Instruction Decode(uint uInstr, Pdp11Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new Pdp11Instruction
                {
                    Mnemonic = this.opcode,
                    InstructionClass = iclass,
                    DataWidth = dasm.dataWidth,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic opcode, params Mutator<Pdp11Disassembler> [] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, mutators);
        }

        private static InstrDecoder Instr(Mnemonic opcode, InstrClass iclass, params Mutator<Pdp11Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, mutators);
        }

        private static readonly Decoder[] decoders;
        private static readonly Decoder[] extraDecoders;
        private static readonly Decoder[] fpu2Decoders;

        static Pdp11Disassembler()
        {
            var illegal = Instr(Mnemonic.illegal, InstrClass.Invalid);
            
            decoders = new Decoder[] {
                null,
                Instr(Mnemonic.mov, w,e,E),
                Instr(Mnemonic.cmp, e,E),
                Instr(Mnemonic.bit, e,E),
                Instr(Mnemonic.bic, e,E),
                Instr(Mnemonic.bis, e,E),
                Instr(Mnemonic.add, w,e,E),
                null,

                null,
                Instr(Mnemonic.movb, b,e,E),
                Instr(Mnemonic.cmp, e,E),
                Instr(Mnemonic.bit, e,E),
                Instr(Mnemonic.bic, e,E),
                Instr(Mnemonic.bis, e,E),
                Instr(Mnemonic.sub, w,e,E),
                null,
            };

            fpu2Decoders = new Decoder[16]
{
                illegal,
                // 00 cfcc
                // 01 setf
                // 02 seti
                // 09 setd
                // 0A setl

                illegal,
                // 01 - ldfps
                // 02 - stfps
                // 03 - stst
                // 4 clrf
                // 5 tstf
                // 6 absf
                //{  7, "F", Opcode.negf }, 
                Instr(Mnemonic.mulf, F,f),
                Instr(Mnemonic.modf, F,f),

                Instr(Mnemonic.addf, F,f),
                illegal,
                Instr(Mnemonic.subf, F,f),
                Instr(Mnemonic.cmpf, F,f),

                illegal,
                Instr(Mnemonic.divf, f,F),
                Instr(Mnemonic.stexp, f,E),
                Instr(Mnemonic.stcdi, f,F),

                Instr(Mnemonic.stcfd, f,F),
                Instr(Mnemonic.ldexp, F,f),
                Instr(Mnemonic.ldcid, F,f),
                Instr(Mnemonic.ldcfd, F,f),
            };

            extraDecoders = new Decoder[]
            {
                Instr(Mnemonic.mul, E,r),
                Instr(Mnemonic.div, E,r),
                Instr(Mnemonic.ash, E,r),
                Instr(Mnemonic.ashc, E,r),
                Instr(Mnemonic.xor, E,r),
                Mask(8, 4, fpu2Decoders),
                illegal,
                Instr(Mnemonic.sob , r,I)
            };

        }

        private MachineOperand Imm6(ushort opcode)
        {
            var offset = (opcode & 0x3F) << 1;
            return new AddressOperand(rdr.Address - offset);
        }

        private RegisterOperand FpuAccumulator(uint opcode)
        {
            var freg= arch.GetFpuRegister((int)opcode & 0x7);
            if (freg == null)
                return null;
            return new RegisterOperand(freg);
        }

        private PrimitiveType DataWidthFromSizeBit(uint p)
        {
            return p != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
        }

        private static Pdp11Instruction NonDoubleOperandInstruction(ushort opcode, Pdp11Disassembler dasm)
        {
            var iclass = InstrClass.Linear;
            switch ((opcode >> 8))
            {
            case 0x01: return dasm.BranchInstruction(opcode, Mnemonic.br, InstrClass.Transfer);
            case 0x02: return dasm.BranchInstruction(opcode, Mnemonic.bne);
            case 0x03: return dasm.BranchInstruction(opcode, Mnemonic.beq);
            case 0x04: return dasm.BranchInstruction(opcode, Mnemonic.bge);
            case 0x05: return dasm.BranchInstruction(opcode, Mnemonic.blt);
            case 0x06: return dasm.BranchInstruction(opcode, Mnemonic.bgt);
            case 0x07: return dasm.BranchInstruction(opcode, Mnemonic.ble);
            case 0x80: return dasm.BranchInstruction(opcode, Mnemonic.bpl);
            case 0x81: return dasm.BranchInstruction(opcode, Mnemonic.bmi);
            case 0x82: return dasm.BranchInstruction(opcode, Mnemonic.bhi);
            case 0x83: return dasm.BranchInstruction(opcode, Mnemonic.blos);
            case 0x84: return dasm.BranchInstruction(opcode, Mnemonic.bvc);
            case 0x85: return dasm.BranchInstruction(opcode, Mnemonic.bvs);
            case 0x86: return dasm.BranchInstruction(opcode, Mnemonic.bcc);
            case 0x87: return dasm.BranchInstruction(opcode, Mnemonic.bcs);
            }

            var dataWidth = dasm.DataWidthFromSizeBit(opcode & 0x8000u);
            var ops = new List<MachineOperand>();
            Mnemonic oc = Mnemonic.illegal;
            switch ((opcode >> 6) & 0x3FF)
            {
            case 0x000:
                switch (opcode & 0x3F)
                {
                case 0x00: oc = Mnemonic.halt; iclass = InstrClass.Terminates|InstrClass.Zero; break;
                case 0x01: oc = Mnemonic.wait; break;
                case 0x02: oc = Mnemonic.rti; iclass = InstrClass.Transfer; break;
                case 0x03: oc = Mnemonic.bpt; break;
                case 0x04: oc = Mnemonic.iot; break;
                case 0x05: oc = Mnemonic.reset; iclass = InstrClass.Transfer; break;
                case 0x06: oc = Mnemonic.rtt; iclass = InstrClass.Transfer; break;
                case 0x07: oc = Mnemonic.illegal; break;
                }
                break;
            case 0x001:
                var op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                oc = Mnemonic.jmp; iclass = InstrClass.Transfer; break;
            case 0x002:
                switch (opcode & 0x38)
                {
                case 0:
                    ops.Add(dasm.DecodeOperand(opcode & 7u));
                    oc = Mnemonic.rts;
                    iclass = InstrClass.Transfer; break;
                case 3:
                    op = dasm.DecodeOperand(opcode);
                    if (op == null)
                        return dasm.CreateInvalidInstruction();
                    ops.Add(op);
                    oc = Mnemonic.spl; break;
                case 0x20:
                case 0x28:
                case 0x30:
                case 0x38:
                    return dasm.DecodeCondCode(opcode);
                }
                break;
            case 0x003:
                oc = Mnemonic.swab;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                dataWidth = PrimitiveType.Byte;
                break;
            case 0x020:
            case 0x021:
            case 0x022:
            case 0x023:
            case 0x024:
            case 0x025:
            case 0x026:
            case 0x027:
                oc = Mnemonic.jsr;
                iclass = InstrClass.Transfer | InstrClass.Call;
                ops.Add(Reg(opcode >> 6, dasm));
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                dataWidth = PrimitiveType.Word16;
                break;
            case 0x220:
            case 0x221:
            case 0x222:
            case 0x223:
                oc = Mnemonic.emt;
                ops.Add(new ImmediateOperand(Constant.Byte((byte)opcode)));
                break;
            case 0x224:
            case 0x225:
            case 0x226:
            case 0x227:
                oc = Mnemonic.trap;
                iclass = InstrClass.Transfer;
                ops.Add(new ImmediateOperand(Constant.Byte((byte)opcode)));
                break;
            case 0x028:
            case 0x228:
                oc = dataWidth.Size == 1 ? Mnemonic.clrb : Mnemonic.clr;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x029:
            case 0x229:
                oc = Mnemonic.com;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02A:
            case 0x22A:
                oc = Mnemonic.inc;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02B:
            case 0x22B:
                oc = Mnemonic.dec;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02C:
            case 0x22C:
                oc = Mnemonic.neg;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02D:
            case 0x22D:
                oc = Mnemonic.adc;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02E:
            case 0x22E:
                oc = Mnemonic.sbc;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x02F:
            case 0x22F:
                oc = Mnemonic.tst;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x030:
            case 0x230:
                oc = Mnemonic.ror;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x031:
            case 0x231:
                oc = Mnemonic.rol;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x032:
            case 0x232:
                oc = Mnemonic.asr;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x033:
            case 0x233:
                oc = Mnemonic.asl;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x034:
                oc = Mnemonic.mark;
                ops.Add(new ImmediateOperand(Constant.Byte((byte)opcode)));
                break;
            case 0x234:
                oc = Mnemonic.mtps;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x035:
                oc = Mnemonic.mfpi;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x235:
                oc = Mnemonic.mfpd;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x036:
                oc = Mnemonic.mtpi;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x236:
                oc = Mnemonic.mtpd;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x037:
                oc = Mnemonic.sxt;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            case 0x237:
                oc = Mnemonic.mfps;
                op = dasm.DecodeOperand(opcode);
                if (op == null)
                    return dasm.CreateInvalidInstruction();
                ops.Add(op);
                break;
            }
            return new Pdp11Instruction
            {
                Mnemonic = oc,
                InstructionClass = iclass,
                DataWidth = dataWidth,
                Operands = ops.ToArray()
            };
        }

        private Pdp11Instruction DecodeCondCode(ushort opcode)
        {
            if ((opcode & 0x1F) == 0)
            {
                return new Pdp11Instruction
                {
                    Mnemonic = Mnemonic.nop,
                    InstructionClass = InstrClass.Linear|InstrClass.Padding,
                    Operands = new MachineOperand[0]
                };
            } 
            return new Pdp11Instruction
            {
                Mnemonic = ((opcode & 0x10) != 0) ? Mnemonic.setflags : Mnemonic.clrflags,
                InstructionClass = InstrClass.Linear,
                DataWidth = dataWidth,
                Operands = new MachineOperand[] { new ImmediateOperand(Constant.Byte((byte) (opcode & 0xF))) },
            };
        }

        private static MachineOperand Reg(int bits, Pdp11Disassembler dasm)
        {
            return new RegisterOperand(dasm.arch.GetRegister(bits & 7));
        }

        private MachineOperand Imm3(ushort opcode)
        {
            throw new NotImplementedException();
        }

        private Pdp11Instruction BranchInstruction(ushort opcode, Mnemonic oc, InstrClass iclass = InstrClass.ConditionalTransfer)
        {
            return new Pdp11Instruction
            {
                Mnemonic = oc,
                InstructionClass = iclass,
                DataWidth = PrimitiveType.Word16,
                Operands = new MachineOperand[] { new AddressOperand(this.rdr.Address + 2 * (sbyte) (opcode & 0xFF)) },
            };
        }

        /// <summary>
        /// Decodes an operand based on the 6-bit quantitity <paramref name="operandBits"/>.
        /// </summary>
        /// <param name="operandBits"></param>
        /// <returns>A decoded operand, or null if invalid.</returns>
        private MachineOperand DecodeOperand(uint operandBits, bool fpuReg = false)
        {
            ushort u;
            var reg = this.arch.GetRegister((int)operandBits & 7);
            //Debug.Print("operandBits {0:X} {1:X} ", (operandBits >> 3) & 7, operandBits & 7);
            if (reg == Registers.pc)
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0:
                    if (fpuReg)
                        return FpuAccumulator(operandBits & 7);
                    else
                        return new RegisterOperand(reg);
                case 1: return new MemoryOperand(AddressMode.RegDef, this.dataWidth, reg);
                case 2:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return ImmediateOperand.Word16(u);
                case 3: return new MemoryOperand(this.rdr.ReadLeUInt16(), this.dataWidth);
                case 6:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return new MemoryOperand(AddressMode.Indexed, this.dataWidth, reg)
                    {
                        EffectiveAddress = u,
                    };
                // PC relative
                case 7:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return new MemoryOperand(AddressMode.IndexedDef, this.dataWidth, reg)
                    {
                        EffectiveAddress =  u,
                    };
                }
                return null;
            }
            else
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);                                 //   Reg           Direct addressing of the register
                case 1: return new MemoryOperand(AddressMode.RegDef, this.dataWidth, reg);      //   Reg Def       Contents of Reg is the address
                case 2: return new MemoryOperand(AddressMode.AutoIncr, this.dataWidth, reg);   //   AutoIncr      Contents of Reg is the address, then Reg incremented
                case 3: return new MemoryOperand(AddressMode.AutoIncrDef, this.dataWidth, reg);    //   AutoIncrDef   Content of Reg is addr of addr, then Reg Incremented
                case 4: return new MemoryOperand(AddressMode.AutoDecr, this.dataWidth, reg);   //   AutoDecr      Reg incremented, then contents of Reg is the address
                case 5: return new MemoryOperand(AddressMode.AutoDecrDef, this.dataWidth, reg);    //   AutoDecrDef   Reg is decremented then contents is addr of addr
                case 6:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return new MemoryOperand(AddressMode.Indexed, this.dataWidth, reg)
                    {
                        EffectiveAddress = u
                    };
                case 7:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return new MemoryOperand(AddressMode.IndexedDef, this.dataWidth, reg)
                    {
                        EffectiveAddress = u
                    };
                default: return null;
                }
            }
        }
    }
}
