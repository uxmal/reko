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
    public partial class T32Disassembler
    {
        #region Decoder classes

        /// <summary>
        /// A decoder is responsible for picking apart the bits of a machine language instruction
        /// and interpreting in the correct way.
        /// </summary>
        private abstract class Decoder
        {
            public abstract AArch32Instruction Decode(T32Disassembler dasm, uint wInstr);


            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, Bitfield[] bitfields, string debugString)
            {
                var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
                TraceDecoder(wInstr, shMask, debugString);
            }

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, int shift, uint mask, string debugString)
            {
                var shMask = mask << shift;
                TraceDecoder(wInstr, shMask, debugString);
            }

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, uint shMask, string debugString)
            {
                return;
                var hibit = 0x80000000u;
                var sb = new StringBuilder();
                for (int i = 0; i < 32; ++i)
                {
                    if ((shMask & hibit) != 0)
                    {
                        sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                    }
                    else
                    {
                        sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                    }
                    shMask <<= 1;
                    wInstr <<= 1;
                }
                if (!string.IsNullOrEmpty(debugString))
                {
                    sb.AppendFormat(" {0}", debugString);
                }
                Debug.Print(sb.ToString());
            }
        }

        /// <summary>
        /// Shifts the machine code instruction by a given shift amount and masks it
        /// with a bit mask. The result extracted bitfield is then used as an index into
        /// sub-decoders. 
        /// </summary>
        private class MaskDecoder : Decoder
        {
            private readonly Decoder[] decoders;
            private readonly int shift;
            private readonly uint mask;
            private readonly string debugString;

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
            {
                Debug.Assert(decoders == null || (int)(mask + 1) == decoders.Length, $"shift: {shift}, mask: {mask}, decoders: {decoders.Length}");
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
            }

            public MaskDecoder(int shift, uint mask, string debugString, params Decoder[] decoders) 
                : this(shift, mask, decoders)
            {
                this.debugString = debugString;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                TraceDecoder(wInstr, shift, mask, debugString);
                var op = (wInstr >> shift) & mask;
                return decoders[op].Decode(dasm, wInstr);
            }
        }

        private class BitFieldsDecoder : Decoder
        {
            private readonly (int, int, uint)[] bitfields;
            private readonly Decoder[] decoders;

            public BitFieldsDecoder(string fieldSpecifier, params Decoder[] decoders)
            {
                this.decoders = decoders;

                int i = 0;
                var list = new List<(int, int, uint)>();
                do
                {
                    int pos = ReadDecimal(fieldSpecifier, ref i);
                    Expect(':', fieldSpecifier, ref i); ;
                    int size = ReadDecimal(fieldSpecifier, ref i);
                    var subMask = (1u << size) - 1u;
                    list.Add((pos, size, subMask));
                } while (PeekAndDiscard(':', fieldSpecifier, ref i));
                this.bitfields = list.ToArray();
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                uint val = 0;
                foreach (var (pos, size, submask) in bitfields)
                {
                    val = (val << size) | ((wInstr >> pos) & submask);
                }
                return decoders[val].Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// This decoder selected one of two sub-decoders based on the outcome of the predicate.
        /// </summary>
        private class SelectDecoder : Decoder
        {
            private readonly Func<uint, bool> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectDecoder(Func<uint, bool> predicate,
                Decoder trueDecoder,
                Decoder falseDecoder)
            {
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                return (predicate(wInstr) ? trueDecoder : falseDecoder).Decode(dasm, wInstr);
            }
        }

        private class SelectFieldDecoder : Decoder
        {
            private readonly Bitfield[] fieldSpecifier;
            private readonly Func<uint, bool> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectFieldDecoder(
                Bitfield[] fieldSpecifier,
                Func<uint, bool> predicate,
                Decoder trueDecoder,
                Decoder falseDecoder)
            {
                this.fieldSpecifier = fieldSpecifier;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                TraceDecoder(wInstr, fieldSpecifier, null);
                var n = Bitfield.ReadFields(fieldSpecifier, wInstr);
                var decoder = (predicate(n) ? trueDecoder : falseDecoder);
                return decoder.Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// Reads in the extra 16 bits needed for a 32-bit T32 instruction.
        /// </summary>
        private class LongDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public LongDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                if (!dasm.rdr.TryReadLeUInt16(out var wNext))
                    return null;
                wInstr = (wInstr << 16) | wNext;
                TraceDecoder(wInstr, 9 + 16, 0xF, null);
                return decoders[SBitfield(wInstr, 9 + 16, 4)].Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// This decoder hands control back to the disassembler, passing the 
        /// deduced opcode and an array of mutator functions that are called in
        /// order to build instruction operands and other ARM instruction
        /// fields.
        /// </summary>
        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly ArmVectorData vec;
            private readonly Mutator<T32Disassembler>[] mutators;

            public InstrDecoder(Opcode opcode, InstrClass iclass, ArmVectorData vec, params Mutator<T32Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.vec = vec;
                this.mutators = mutators;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                dasm.state.opcode = this.opcode;
                dasm.state.iclass = this.iclass;
                dasm.state.vectorData = this.vec;
                for (int i = 0; i < mutators.Length; ++i)
                {
                    if (!mutators[i](wInstr, dasm))
                    {
                        return dasm.Invalid();
                    }
                }
                var instr = dasm.state.MakeInstruction();
                return instr;
            }
        }

        // Decodes the T1 BL instruction
        private class BlDecoder : Decoder
        {
            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
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
                    opcode = Opcode.bl,
                    ops = new MachineOperand[] { AddressOperand.Create(dasm.addr + (off + 4)) }
                };
            }
        }

        // Decodes 16-bit LDM* STM* instructions
        private class LdmStmDecoder16 : Decoder
        {
            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 8, 3)];
                var registers = (byte)wInstr;
                var st = SBitfield(wInstr, 11, 1) == 0;
                var w = st || (registers & (1 << rn.Number)) == 0;
                return new T32Instruction
                {
                    opcode = st
                        ? Opcode.stm
                        : Opcode.ldm,
                    InstructionClass = InstrClass.Linear,
                    Writeback = w,
                    ops = new MachineOperand[] {
                            new RegisterOperand(rn),
                            new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, (registers)) }
                };
            }
        }

        // Decodes 32-bit LDM* STM* instructions
        private class LdmStmDecoder32 : Decoder
        {
            private readonly Opcode opcode;

            public LdmStmDecoder32(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
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
                        opcode = l != 0 ? Opcode.pop : Opcode.push,
                        InstructionClass = InstrClass.Linear,
                        Wide = true,
                        Writeback = w,
                        ops = new MachineOperand[] { new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, registers) }
                    };
                }
                else
                {
                    return new T32Instruction
                    {
                        opcode = opcode,
                        InstructionClass = InstrClass.Linear,
                        Writeback = w,
                        ops = new MachineOperand[] {
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

            public MovMovsDecoder(Opcode opcode, params Mutator<T32Disassembler>[] mutators) : base(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators)
            {
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                var instr = base.Decode(dasm, wInstr);
                if (instr.ops[2] is ImmediateOperand imm && imm.Value.IsIntegerZero)
                {
                    instr.opcode = Opcode.mov;
                }
                return instr;
            }
        }

        // Decodes IT instructions
        private class ItDecoder : Decoder
        {
            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                var instr = new T32Instruction
                {
                    opcode = Opcode.it,
                    InstructionClass = InstrClass.Linear,
                    condition = (ArmCondition)SBitfield(wInstr, 4, 4),
                    itmask = (byte)SBitfield(wInstr, 0, 4)
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
            public BfcBfiDecoder(Opcode opcode, Mutator<T32Disassembler> [] mutators) : base(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators)
            {
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                // A hack -- we patch up the output of the regular decoder.
                var instr = base.Decode(dasm, wInstr);
                ImmediateOperand opLsb;
                ImmediateOperand opMsb;
                if (instr.ops.Length > 3)
                {
                    opMsb = (ImmediateOperand)instr.ops[3];
                    opLsb = (ImmediateOperand)instr.ops[2];
                }
                else
                {
                    opMsb = (ImmediateOperand)instr.ops[2];
                    opLsb = (ImmediateOperand)instr.ops[1];
                }
                var opWidth = ImmediateOperand.Word32(opMsb.Value.ToInt32() - opLsb.Value.ToInt32() + 1);
                if (instr.ops.Length > 3)
                    instr.ops[3] = opWidth;
                else
                    instr.ops[2] = opWidth;
                return instr;
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override AArch32Instruction Decode(T32Disassembler dasm, uint wInstr)
            {
                return dasm.NotYetImplemented(message, wInstr);
            }

        }

        #endregion
    }
}
