#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Pdp11
{
    using Decoder = Decoder<Pdp11Disassembler, Mnemonic, Pdp11Instruction>;

    public class Pdp11Disassembler : DisassemblerBase<Pdp11Instruction, Mnemonic>
    {
#pragma warning disable IDE1006 // Naming Styles

        private static readonly Decoder[] decoders;

        private readonly Pdp11Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private Pdp11Instruction instrCur;
        private PrimitiveType dataWidth;

        public Pdp11Disassembler(EndianImageReader rdr, Pdp11Architecture arch)
        {
            this.rdr = rdr;
            this.arch = arch;
            this.ops = new List<MachineOperand>(2);
            this.instrCur = null!;
            this.dataWidth = null!;
        }

        public override Pdp11Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort opcode))
                return null;
            ops.Clear();
            dataWidth = PrimitiveType.Word16;
            var decoder = decoders[(opcode >> 0x0C) & 0x00F];
            instrCur = decoder.Decode(opcode, this);
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        public override Pdp11Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Pdp11Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                DataWidth = this.dataWidth,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override Pdp11Instruction CreateInvalidInstruction()
        {
            return new Pdp11Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = System.Array.Empty<MachineOperand>()
            };
        }

        public override Pdp11Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Pdp11dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
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
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool e(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.DecodeOperand(wOpcode >> 6);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool R0(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.arch.GetRegister(((int) wOpcode) & 7)!;
            dasm.ops.Add(op);
            return true;
        }

        private static bool r(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.arch.GetRegister(((int)wOpcode >> 6) & 7)!;
            dasm.ops.Add(op);
            return true;
        }

        private static bool I(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.Imm6((ushort)wOpcode);
            dasm.ops.Add(op);
            return true;
        }

        private static bool Ib(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = Constant.Byte((byte)wOpcode);
            dasm.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Low order 3 bits of instruction word.
        /// </summary>
        private static bool I3(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = Constant.Byte((byte) (wOpcode & 0x07));
            dasm.ops.Add(op);
            return true;
        }
        // I4 - low order 4 bits.
        private static bool I4(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = Constant.Byte((byte)(wOpcode & 0x0F));
            dasm.ops.Add(op);
            return true;
        }

        private static bool F(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.DecodeOperand(wOpcode, true);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool f(uint wOpcode, Pdp11Disassembler dasm)
        {
            var op = dasm.FpuAccumulator(wOpcode);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }
        
        private static bool PcRel(uint wOpcode, Pdp11Disassembler dasm)
        {
            var uAddr = (int) dasm.rdr.Address.ToLinear() + 2 * (sbyte) (wOpcode & 0xFF);
            dasm.ops.Add(Address.Ptr16((ushort) uAddr));
            return true;
        }

        #endregion

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Pdp11Disassembler> [] mutators)
        {
            return new InstrDecoder<Pdp11Disassembler,Mnemonic,Pdp11Instruction>(InstrClass.Linear, mnemonic, mutators);
            }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Pdp11Disassembler>[] mutators)
            {
            return new InstrDecoder<Pdp11Disassembler, Mnemonic, Pdp11Instruction>(iclass, mnemonic, mutators);
                }


        static Pdp11Disassembler()
        {
            var illegal = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var jsr = Instr(Mnemonic.jsr, InstrClass.Transfer | InstrClass.Call, w, r, E);
            var emt = Instr(Mnemonic.emt, InstrClass.Transfer | InstrClass.Call, Ib);
            var trap = Instr(Mnemonic.trap, InstrClass.Transfer, Ib);

            var condCode = Select((0, 5), u => u == 0,
                Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding),
                Mask(4, 1, "  Condition code operators",
                    Instr(Mnemonic.clrflags, I4),
                    Instr(Mnemonic.setflags, I4)));

            var nondoubleFb = Sparse(6, 10, "  no double operand", illegal,
                (0x000, Sparse(0, 6, "  0x000", illegal,
                    (0x00, Instr(Mnemonic.halt, InstrClass.Terminates | InstrClass.Zero)),
                    (0x01, Instr(Mnemonic.wait)),
                    (0x02, Instr(Mnemonic.rti, InstrClass.Transfer | InstrClass.Return)),
                    (0x03, Instr(Mnemonic.bpt)),
                    (0x04, Instr(Mnemonic.iot)),
                    (0x05, Instr(Mnemonic.reset, InstrClass.Transfer)),
                    (0x06, Instr(Mnemonic.rtt, InstrClass.Transfer | InstrClass.Return)),
                    (0x07, Instr(Mnemonic.mfpt)))),
                (0x001, Instr(Mnemonic.jmp, InstrClass.Transfer, E)),
                (0x002, Mask(3, 3, "  002",
                    Instr(Mnemonic.rts, InstrClass.Transfer | InstrClass.Return, R0),
                    illegal,
                    illegal,
                    Instr(Mnemonic.spl, I3),
                    condCode,
                    condCode,
                    condCode,
                    condCode)),
                (0x003, Instr(Mnemonic.swab, E)),
                (0x020, jsr),
                (0x021, jsr),
                (0x022, jsr),
                (0x023, jsr),
                (0x024, jsr),
                (0x025, jsr),
                (0x026, jsr),
                (0x027, jsr),

               (0x220, emt),
               (0x221, emt),
               (0x222, emt),
               (0x223, emt),
               (0x224, trap),
               (0x225, trap),
               (0x226, trap),
               (0x227, trap),

               (0x028, Instr(Mnemonic.clr, w, E)),
               (0x228, Instr(Mnemonic.clrb, b, E)),
               (0x029, Instr(Mnemonic.com, w, E)),
               (0x229, Instr(Mnemonic.comb, b, E)),
               (0x02A, Instr(Mnemonic.inc, w, E)),
               (0x22A, Instr(Mnemonic.incb, b, E)),
               (0x02B, Instr(Mnemonic.dec, w, E)),
               (0x22B, Instr(Mnemonic.decb, b, E)),
               (0x02C, Instr(Mnemonic.neg, w, E)),
               (0x22C, Instr(Mnemonic.negb, b, E)),
               (0x02D, Instr(Mnemonic.adc, w, E)),
               (0x22D, Instr(Mnemonic.adcb, b, E)),
               (0x02E, Instr(Mnemonic.sbc, w, E)),
               (0x22E, Instr(Mnemonic.sbcb, b, E)),
               (0x02F, Instr(Mnemonic.tst, w, E)),
               (0x22F, Instr(Mnemonic.tstb, b, E)),
               (0x030, Instr(Mnemonic.ror, w, E)),
               (0x230, Instr(Mnemonic.rorb, b, E)),
               (0x031, Instr(Mnemonic.rol, w, E)),
               (0x231, Instr(Mnemonic.rolb, b, E)),
               (0x032, Instr(Mnemonic.asr, w, E)),
               (0x232, Instr(Mnemonic.asrb, b, E)),
               (0x033, Instr(Mnemonic.asl, w, E)),
               (0x233, Instr(Mnemonic.aslb, b, E)),
               (0x034, Instr(Mnemonic.mark, Ib)),
               (0x234, Instr(Mnemonic.mtps, b, E)),
               (0x035, Instr(Mnemonic.mfpi, w, E)),
               (0x235, Instr(Mnemonic.mfpd, b, E)),
               (0x036, Instr(Mnemonic.mtpi, w, E)),
               (0x236, Instr(Mnemonic.mtpd, b, E)),
               (0x037, Instr(Mnemonic.sxt, w, E)),
               (0x237, Instr(Mnemonic.mfps, b, E)));

            var nondouble = Sparse(8, 8, nondoubleFb,
                (0x01, Instr(Mnemonic.br, InstrClass.Transfer, PcRel)),
                (0x02, Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, PcRel)),
                (0x03, Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, PcRel)),
                (0x04, Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, PcRel)),
                (0x05, Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, PcRel)),
                (0x06, Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, PcRel)),
                (0x07, Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, PcRel)),
                (0x80, Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, PcRel)),
                (0x81, Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, PcRel)),
                (0x82, Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, PcRel)),
                (0x83, Instr(Mnemonic.blos,InstrClass.ConditionalTransfer, PcRel)),
                (0x84, Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, PcRel)),
                (0x85, Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, PcRel)),
                (0x86, Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, PcRel)),
                (0x87, Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, PcRel)));

            var fpu2Decoders = Mask(8, 4, "  fpu2Decoders",
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
                //{  7, "F", Mnemonic.negf }, 
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
                Instr(Mnemonic.ldcfd, F,f));

            var extra = Mask(9, 3, "  extra",
                Instr(Mnemonic.mul, E, r),
                Instr(Mnemonic.div, E, r),
                Instr(Mnemonic.ash, E, r),
                Instr(Mnemonic.ashc, E, r),
                Instr(Mnemonic.xor, E, r),
                fpu2Decoders,
                illegal,
                Instr(Mnemonic.sob, r, I));

            var extra2 = Mask(9, 3, "  extra",
               Instr(Mnemonic.mul, E, r),
               Instr(Mnemonic.div, E, r),
               Instr(Mnemonic.ash, E, r),
               Instr(Mnemonic.ashc, E, r),
               Instr(Mnemonic.xor, E, r),
               fpu2Decoders,
               illegal,
               illegal);

            decoders = new Decoder[] {
                nondouble,
                Instr(Mnemonic.mov, w,e,E),
                Instr(Mnemonic.cmp, w,e,E),
                Instr(Mnemonic.bit, w,e,E),
                Instr(Mnemonic.bic, w,e,E),
                Instr(Mnemonic.bis, w,e,E),
                Instr(Mnemonic.add, w,e,E),
                extra,

                nondouble,
                Instr(Mnemonic.movb, b,e,E),
                Instr(Mnemonic.cmpb, b,e,E),
                Instr(Mnemonic.bitb, b,e,E),
                Instr(Mnemonic.bicb, b,e,E),
                Instr(Mnemonic.bisb, b,e,E),
                Instr(Mnemonic.sub, w,e,E),
                extra2,
            };
        }

        private MachineOperand Imm6(ushort opcode)
        {
            var offset = (opcode & 0x3F) << 1;
            return rdr.Address - offset;
        }

        private RegisterStorage? FpuAccumulator(uint opcode)
        {
            var freg= arch.GetFpuRegister((int)opcode & 0x7);
            if (freg is null)
                return null;
            return freg;
        }

        /// <summary>
        /// Decodes an operand based on the 6-bit quantitity <paramref name="operandBits"/>.
        /// </summary>
        /// <param name="operandBits"></param>
        /// <returns>A decoded operand, or null if invalid.</returns>
        private MachineOperand? DecodeOperand(uint operandBits, bool fpuReg = false)
        {
            ushort u;
            var reg = this.arch.GetRegister((int)operandBits & 7)!;
            var mode = (operandBits >> 3) & 7;
            if (reg == Registers.pc)
            {
                switch (mode)
                {
                case 0:
                    if (fpuReg)
                        return FpuAccumulator(operandBits & 7);
                    else
                        return reg;
                case 1: return new MemoryOperand(AddressMode.RegDef, this.dataWidth, reg);
                case 2:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return Constant.Word16(u);
                case 3:
                    if (!this.rdr.TryReadLeUInt16(out u))
                        return null;
                    return new MemoryOperand(u, this.dataWidth);
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
                switch (mode)
                {
                case 0: return reg;                                 //   Reg           Direct addressing of the register
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
