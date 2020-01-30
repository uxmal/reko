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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm.AArch32
{
    using Decoder = Reko.Core.Machine.Decoder<T32Disassembler, Mnemonic, AArch32Instruction>;

    public partial class T32Disassembler
    {
        #region Decoder classes

        /// <summary>
        /// Reads in the extra 16 bits needed for a 32-bit T32 instruction.
        /// </summary>
        private class LongDecoder : Decoder<T32Disassembler, Mnemonic, AArch32Instruction>
        {
            private readonly Decoder[] decoders;

            public LongDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out var wNext))
                    return null;
                wInstr = (wInstr << 16) | wNext;
                DumpMaskedInstruction(wInstr, 0xF << (9 + 16), "");
                return decoders[SBitfield(wInstr, 9 + 16, 4)].Decode(wInstr, dasm);
            }
        }

        /// <summary>
        /// This decoder hands control back to the disassembler, passing the 
        /// decoded mnemonic and an array of mutator functions that are called in
        /// order to build instruction operands and other ARM instruction
        /// fields.
        /// </summary>
        private class InstrDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly InstrClass iclass;
            private readonly ArmVectorData vec;
            private readonly Mutator<T32Disassembler>[] mutators;

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, ArmVectorData vec, params Mutator<T32Disassembler>[] mutators)
            {
                this.mnemonic = mnemonic;
                this.iclass = iclass;
                this.vec = vec;
                this.mutators = mutators;
            }

            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                DumpMaskedInstruction(wInstr, 0, this.mnemonic);
                dasm.state.mnemonic = this.mnemonic;
                dasm.state.iclass = this.iclass;
                dasm.state.vectorData = this.vec;
                for (int i = 0; i < mutators.Length; ++i)
                {
                    if (!mutators[i](wInstr, dasm))
                    {
                        return dasm.CreateInvalidInstruction();
                    }
                }
                var instr = dasm.state.MakeInstruction();
                return instr;
            }
        }

        // Decodes the T1 BL instruction
        private class BlDecoder : Decoder
        {
            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                var s = SBitfield(wInstr, 10 + 16, 1);
                var i1 = 1 & ~(SBitfield(wInstr, 13, 1) ^ s);
                var i2 = 1 & ~(SBitfield(wInstr, 11, 1) ^ s);
                var off = (int)Bits.SignExtend((uint)s, 1);
                off = (off << 2) | (i1 << 1) | i2;
                off = (off << 10) | SBitfield(wInstr, 16, 10);
                off = (off << 11) | SBitfield(wInstr, 0, 11);
                off <<= 1;
                return new T32Instruction
                {
                    Mnemonic = Mnemonic.bl,
                    Operands = new MachineOperand[] { AddressOperand.Create(dasm.addr + (off + 4)) }
                };
            }
        }

        // Decodes 16-bit LDM* STM* instructions
        private class LdmStmDecoder16 : Decoder
        {
            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 8, 3)];
                var registers = (byte)wInstr;
                var st = SBitfield(wInstr, 11, 1) == 0;
                var w = st || (registers & (1 << rn.Number)) == 0;
                return new T32Instruction
                {
                    Mnemonic = st
                        ? Mnemonic.stm
                        : Mnemonic.ldm,
                    InstructionClass = InstrClass.Linear,
                    Writeback = w,
                    Operands = new MachineOperand[] {
                            new RegisterOperand(rn),
                            new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, (registers)) }
                };
            }
        }

        // Decodes 32-bit LDM* STM* instructions
        private class LdmStmDecoder32 : Decoder
        {
            private readonly Mnemonic mnemonic;

            public LdmStmDecoder32(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 16, 4)];
                var registers = (ushort)wInstr;
                var w = SBitfield(wInstr, 16 + 5, 1) != 0;
                var l = SBitfield(wInstr, 16 + 4, 1);
                // writeback
                if (rn == Registers.sp)
                {
                    return new T32Instruction
                    {
                        Mnemonic = l != 0 ? Mnemonic.pop : Mnemonic.push,
                        InstructionClass = InstrClass.Linear,
                        Wide = true,
                        Writeback = w,
                        Operands = new MachineOperand[] { new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, registers) }
                    };
                }
                else
                {
                    return new T32Instruction
                    {
                        Mnemonic = mnemonic,
                        InstructionClass = InstrClass.Linear,
                        Writeback = w,
                        Operands = new MachineOperand[] {
                            new RegisterOperand(rn),
                            new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, registers)
                        }
                    };
                }
            }
        }

        // Decodes Mov/Movs instructions with optional shifts
        private class MovMovsDecoder : InstrDecoder
        {

            public MovMovsDecoder(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators) : base(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators)
            {
            }

            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                var instr = base.Decode(wInstr, dasm);
                if (instr.Operands[2] is ImmediateOperand imm && imm.Value.IsIntegerZero)
                {
                    instr.Mnemonic = Mnemonic.mov;
                }
                return instr;
            }
        }

        // Decodes IT instructions
        private class ItDecoder : Decoder
        {
            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                var instr = new T32Instruction
                {
                    Mnemonic = Mnemonic.it,
                    InstructionClass = InstrClass.Linear,
                    condition = (ArmCondition)SBitfield(wInstr, 4, 4),
                    itmask = (byte)SBitfield(wInstr, 0, 4),
                    Operands = MachineInstruction.NoOperands
                };
                // Add an extra bit for the 't' in 'it'.
                dasm.itState = instr.itmask | (SBitfield(wInstr, 4, 1) << 4);
                dasm.itCondition = instr.condition;
                return instr;
            }
        }

        // Decode BFC and BFI instructions, which display their immediate constants
        // differently from how they are repesented in the word.
        private class BfcBfiDecoder : InstrDecoder
        {
            public BfcBfiDecoder(Mnemonic mnemonic, Mutator<T32Disassembler> [] mutators) : base(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators)
            {
            }

            public override AArch32Instruction Decode(uint wInstr, T32Disassembler dasm)
            {
                // A hack -- we patch up the output of the regular decoder.
                var instr = base.Decode(wInstr, dasm);
                ImmediateOperand opLsb;
                ImmediateOperand opMsb;
                if (instr.Operands.Length > 3)
                {
                    opMsb = (ImmediateOperand)instr.Operands[3];
                    opLsb = (ImmediateOperand)instr.Operands[2];
                }
                else
                {
                    opMsb = (ImmediateOperand)instr.Operands[2];
                    opLsb = (ImmediateOperand)instr.Operands[1];
                }
                var opWidth = ImmediateOperand.Word32(opMsb.Value.ToInt32() - opLsb.Value.ToInt32() + 1);
                if (instr.Operands.Length > 3)
                    instr.Operands[3] = opWidth;
                else
                    instr.Operands[2] = opWidth;
                return instr;
            }
        }

        #endregion
    }
}
