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
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort opcode))
                return null;
            if (addr.ToLinear() == 0x277A)
                addr.ToString();
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

        abstract class Decoder
        {
            public abstract Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm);
        }

        class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator<Pdp11Disassembler>[] mutators;

            public InstrDecoder(Opcode op, InstrClass iclass, params Mutator<Pdp11Disassembler>[] mutators)
            {
                this.opcode = op;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(opcode, dasm))
                        return new Pdp11Instruction
                        {
                            Opcode = Opcode.illegal,
                            InstructionClass = InstrClass.Invalid,
                        };
                }
                var instr = new Pdp11Instruction
                {
                    Opcode = this.opcode,
                    InstructionClass = iclass,
                    DataWidth = dasm.dataWidth,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                };
                return instr;
            }
        }

        class FnDecoder : Decoder
        {
            private Func<ushort, Pdp11Disassembler, Pdp11Instruction> fn;

            public FnDecoder(Func<ushort, Pdp11Disassembler, Pdp11Instruction> fn)
            {
                this.fn = fn;
            }

            public override Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm)
            {
                return fn(opcode, dasm);
            }
        }

        private static InstrDecoder Instr(Opcode opcode, params Mutator<Pdp11Disassembler> [] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<Pdp11Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, mutators);
        }

        private static readonly Decoder[] decoders;
        private static readonly Decoder[] extraDecoders;
        private static readonly Decoder[] fpu2Decoders;

        static Pdp11Disassembler()
        {
            var illegal = Instr(Opcode.illegal, InstrClass.Invalid);
            
            decoders = new Decoder[] {
                null,
                Instr(Opcode.mov, w,e,E),
                Instr(Opcode.cmp, e,E),
                Instr(Opcode.bit, e,E),
                Instr(Opcode.bic, e,E),
                Instr(Opcode.bis, e,E),
                Instr(Opcode.add, w,e,E),
                null,

                null,
                Instr(Opcode.movb, b,e,E),
                Instr(Opcode.cmp, e,E),
                Instr(Opcode.bit, e,E),
                Instr(Opcode.bic, e,E),
                Instr(Opcode.bis, e,E),
                Instr(Opcode.sub, w,e,E),
                null,
            };

            extraDecoders = new Decoder[]
            {
                Instr(Opcode.mul, E,r),
                Instr(Opcode.div, E,r),
                Instr(Opcode.ash, E,r),
                Instr(Opcode.ashc, E,r),
                Instr(Opcode.xor, E,r),
                new FnDecoder(FpuArithmetic),
                illegal,
                Instr(Opcode.sob , r,I)
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
                Instr(Opcode.mulf, F,f),
                Instr(Opcode.modf, F,f),

                Instr(Opcode.addf, F,f),
                illegal,
                Instr(Opcode.subf, F,f),
                Instr(Opcode.cmpf, F,f),

                illegal,
                Instr(Opcode.divf, f,F),
                Instr(Opcode.stexp, f,E),
                Instr(Opcode.stcdi, f,F),

                Instr(Opcode.stcfd, f,F),
                Instr(Opcode.ldexp, F,f),
                Instr(Opcode.ldcid, F,f),
                Instr(Opcode.ldcfd, F,f),
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

        private static Pdp11Instruction FpuArithmetic(ushort opcode, Pdp11Disassembler dasm)
        {
            return fpu2Decoders[(opcode >> 8) & 0x0F].Decode(opcode, dasm);
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
            case 0x01: return dasm.BranchInstruction(opcode, Opcode.br, InstrClass.Transfer);
            case 0x02: return dasm.BranchInstruction(opcode, Opcode.bne);
            case 0x03: return dasm.BranchInstruction(opcode, Opcode.beq);
            case 0x04: return dasm.BranchInstruction(opcode, Opcode.bge);
            case 0x05: return dasm.BranchInstruction(opcode, Opcode.blt);
            case 0x06: return dasm.BranchInstruction(opcode, Opcode.bgt);
            case 0x07: return dasm.BranchInstruction(opcode, Opcode.ble);
            case 0x80: return dasm.BranchInstruction(opcode, Opcode.bpl);
            case 0x81: return dasm.BranchInstruction(opcode, Opcode.bmi);
            case 0x82: return dasm.BranchInstruction(opcode, Opcode.bhi);
            case 0x83: return dasm.BranchInstruction(opcode, Opcode.blos);
            case 0x84: return dasm.BranchInstruction(opcode, Opcode.bvc);
            case 0x85: return dasm.BranchInstruction(opcode, Opcode.bvs);
            case 0x86: return dasm.BranchInstruction(opcode, Opcode.bcc);
            case 0x87: return dasm.BranchInstruction(opcode, Opcode.bcs);
            }

            var dataWidth = dasm.DataWidthFromSizeBit(opcode & 0x8000u);
            int cop = 1;
            MachineOperand op1 = null;
            MachineOperand op2 = null;
            Opcode oc = Opcode.illegal;
            switch ((opcode >> 6) & 0x3FF)
            {
            case 0x000:
                switch (opcode & 0x3F)
                {
                case 0x00: cop = 0; oc = Opcode.halt; iclass = InstrClass.Terminates|InstrClass.Zero; break;
                case 0x01: cop = 0; oc = Opcode.wait; break;
                case 0x02: cop = 0; oc = Opcode.rti; iclass = InstrClass.Transfer; break;
                case 0x03: cop = 0; oc = Opcode.bpt; break;
                case 0x04: cop = 0; oc = Opcode.iot; break;
                case 0x05: cop = 0; oc = Opcode.reset; iclass = InstrClass.Transfer; break;
                case 0x06: cop = 0; oc = Opcode.rtt; iclass = InstrClass.Transfer; break;
                case 0x07: cop = 0; oc = Opcode.illegal; break;
                }
                break;
            case 0x001: op1 = dasm.DecodeOperand(opcode); oc = Opcode.jmp; iclass = InstrClass.Transfer; break;
            case 0x002:
                switch (opcode & 0x38)
                {
                case 0: op1 = dasm.DecodeOperand(opcode & 7u); oc = Opcode.rts; iclass = InstrClass.Transfer; break;
                case 3: op1 = dasm.DecodeOperand(opcode); oc = Opcode.spl; break;
                case 0x20:
                case 0x28:
                case 0x30:
                case 0x38:
                    return dasm.DecodeCondCode(opcode);
                }
                break;
            case 0x003:
                oc = Opcode.swab; op1 = dasm.DecodeOperand(opcode);
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
                oc = Opcode.jsr;
                iclass = InstrClass.Transfer | InstrClass.Call;
                cop = 2;
                op1 = Reg(opcode >> 6, dasm);
                op2 = dasm.DecodeOperand(opcode);
                dataWidth = PrimitiveType.Word16;
                break;
            case 0x220:
            case 0x221:
            case 0x222:
            case 0x223:
                oc = Opcode.emt;
                op1 = new ImmediateOperand(Constant.Byte((byte)opcode));
                break;
            case 0x224:
            case 0x225:
            case 0x226:
            case 0x227:
                oc = Opcode.trap;
                iclass = InstrClass.Transfer;
                op1 = new ImmediateOperand(Constant.Byte((byte)opcode));
                break;
            case 0x028:
            case 0x228:
                oc = dataWidth.Size == 1 ? Opcode.clrb : Opcode.clr;
                op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x029:
            case 0x229:
                oc = Opcode.com; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02A:
            case 0x22A:
                oc = Opcode.inc; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02B:
            case 0x22B:
                oc = Opcode.dec; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02C:
            case 0x22C:
                oc = Opcode.neg; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02D:
            case 0x22D:
                oc = Opcode.adc; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02E:
            case 0x22E:
                oc = Opcode.sbc; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x02F:
            case 0x22F:
                oc = Opcode.tst; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x030:
            case 0x230:
                oc = Opcode.ror; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x031:
            case 0x231:
                oc = Opcode.rol; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x032:
            case 0x232:
                oc = Opcode.asr; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x033:
            case 0x233:
                oc = Opcode.asl; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x034:
                oc = Opcode.mark;
                op1 = new ImmediateOperand(Constant.Byte((byte)opcode));
                break;
            case 0x234:
                oc = Opcode.mtps; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x035:
                oc = Opcode.mfpi; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x235:
                oc = Opcode.mfpd; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x036:
                oc = Opcode.mtpi; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x236:
                oc = Opcode.mtpd; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x037:
                oc = Opcode.sxt; op1 = dasm.DecodeOperand(opcode);
                break;
            case 0x237:
                oc = Opcode.mfps; op1 = dasm.DecodeOperand(opcode);
                break;
            }
            if (cop > 0 && op1 == null ||
                cop > 1 && op2 == null)
            {
                return new Pdp11Instruction
                {
                    Opcode = Opcode.illegal,
                    InstructionClass = InstrClass.Invalid
                };
            }
            return new Pdp11Instruction
            {
                Opcode = oc,
                InstructionClass = iclass,
                DataWidth = dataWidth,
                op1 = op1,
                op2 = op2,
            };
        }

        private Pdp11Instruction DecodeCondCode(ushort opcode)
        {
            if ((opcode & 0x1F) == 0)
            {
                return new Pdp11Instruction
                {
                    Opcode = Opcode.nop,
                    InstructionClass = InstrClass.Linear,
                };
            } 
            return new Pdp11Instruction
            {
                Opcode = ((opcode & 0x10) != 0) ? Opcode.setflags : Opcode.clrflags,
                InstructionClass = InstrClass.Linear,
                DataWidth = dataWidth,
                op1 = new ImmediateOperand(Constant.Byte((byte)(opcode&0xF))),
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

        private Pdp11Instruction BranchInstruction(ushort opcode, Opcode oc, InstrClass iclass = InstrClass.ConditionalTransfer)
        {
            return new Pdp11Instruction
            {
                Opcode = oc,
                InstructionClass = iclass,
                DataWidth = PrimitiveType.Word16,
                op1 = new AddressOperand(this.rdr.Address + 2 * (sbyte)(opcode & 0xFF)),
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
