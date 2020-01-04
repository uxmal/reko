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

namespace Reko.Arch.M6800.M6812
{
    using Decoder = Decoder<M6812Disassembler, Mnemonic, M6812Instruction>;

    public class M6812Disassembler : DisassemblerBase<M6812Instruction, Mnemonic>
    {
        private readonly static Decoder[] decoders;
        private readonly static Decoder[] decodersSecondByte;
        private readonly static Decoder[] decodersLoops;
        private readonly static Decoder[] decodersRegisters;
        private readonly static RegisterStorage[] BaseRegisters;

        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> operands;

        public M6812Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.operands = new List<MachineOperand>();
        }

        public override M6812Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out var bInstr))
                return null;
            operands.Clear();
            M6812Instruction instr = decoders[bInstr].Decode(bInstr, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override M6812Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new M6812Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.operands.ToArray()
            };
        }

        public override M6812Instruction CreateInvalidInstruction()
        {
            return new M6812Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public class NextByteDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public NextByteDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override M6812Instruction Decode(uint bInstr, M6812Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                return this.decoders[b].Decode(b, dasm);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<M6812Disassembler> [] mutators)
        {
            return new InstrDecoder<M6812Disassembler, Mnemonic, M6812Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<M6812Disassembler>[] mutators)
        {
            return new InstrDecoder<M6812Disassembler, Mnemonic, M6812Instruction>(iclass, mnemonic, mutators);
        }


        private static Decoder Nyi(string message)
        {
            return new InstrDecoder<M6812Disassembler, Mnemonic, M6812Instruction>(InstrClass.Invalid, Mnemonic.invalid, NotYetImplemented);
        }

        private static bool A(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.a));
            return true;
        }

        private static bool B(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.b));
            return true;
        }

        private static bool D(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.d));
            return true;
        }

        private static bool S(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.sp));
            return true;
        }

        private static bool X(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.x));
            return true;
        }

        private static bool Y(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.y));
            return true;
        }

        private static bool CCR(uint bInstr, M6812Disassembler dasm)
        {
            dasm.operands.Add(new RegisterOperand(Registers.ccr));
            return true;
        }

        private static bool I(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm))
                return false;
            dasm.operands.Add(ImmediateOperand.Byte(imm));
            return true;
        }

        private static bool JK(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var imm))
                return false;
            dasm.operands.Add(ImmediateOperand.Word16(imm));
            return true;
        }

        private static bool PG(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var imm))
                return false;
            dasm.operands.Add(ImmediateOperand.Word16(imm));
            return true;
        }

        private static bool Dir(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm))
                return false;
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Offset = imm
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool HL(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out var imm))
                return false;
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Offset = imm
            };
            dasm.operands.Add(mem);
            return true;
        }

        private readonly static short[] prePostIncrementOffset = new short[16]
        {
            1, 2, 3, 4, 5, 6, 7, 8, -8, -7, -6, -5, -4, -3, -2, -1,
        };
        private Address addr;

        public static bool XB(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var bExtraByte))
                return false;
            RegisterStorage baseReg;
            RegisterStorage idxReg;
            MemoryOperand mem;
            short? offset = null;
            switch ((bExtraByte >> 5) & 0x7)
            {
            case 0:
            case 2:
            case 4:
            case 6:
                // 5-bit constant offset.
                baseReg = BaseRegisters[(bExtraByte >> 6) & 3];
                offset = (short)Bits.SignExtend(bExtraByte, 5);
                mem = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = baseReg,
                    Offset = offset
                };
                dasm.operands.Add(mem);
                return true;
            case 1:
            case 3:
            case 5:
                // Auto (pre|post)(dec|inc)rement 
                baseReg = BaseRegisters[(bExtraByte >> 6) & 3];
                offset = prePostIncrementOffset[bExtraByte & 0xF];
                mem = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = baseReg,
                    Offset = offset,
                    PreIncrement = (bExtraByte & 0x10) == 0,
                    PostIncrement = (bExtraByte & 0x10) != 0,
                };
                break;
            default:
                baseReg = BaseRegisters[(bExtraByte >> 3) & 3];
                idxReg = null;
                bool indirect;
                switch (bExtraByte & 0b111)
                {
                case 0:
                case 1:
                    // 9-bit constant offset
                    indirect = false;
                    if (!dasm.rdr.TryReadByte(out var bOffset))
                        return false;
                    var q = (uint)((bExtraByte << 8) | bOffset);
                    offset = (short)Bits.SignExtend(q, 9);
                    break;
                case 2:
                    // 16-bit constant offset;
                    indirect = false;
                    if (!dasm.rdr.TryReadBeInt16(out var wOffset))
                        return false;
                    offset = wOffset;
                    break;
                case 3:
                    // 16-bit indexed indirect.
                    if (!dasm.rdr.TryReadBeInt16(out var o))
                        return false;
                    offset = o;
                    indirect = true;
                    break;
                case 4: idxReg = Registers.a; indirect = false; break;
                case 5: idxReg = Registers.b; indirect = false; break;
                case 6: idxReg = Registers.d; indirect = false; break;
                default:
                    idxReg = Registers.d;
                    indirect = true;
                    break;
                }
                mem = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = baseReg,
                    Index = idxReg,
                    Offset = offset,
                    Indirect = indirect
                };
                break;
            }
            dasm.operands.Add(mem);
            return true;
        }

        private static bool R(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var rel))
                return false;
            var addrDst = dasm.rdr.Address + (sbyte)rel;
            dasm.operands.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static bool RPlus(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var rel))
                return false;
            var addrDst = dasm.rdr.Address + (int)rel;
            dasm.operands.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static bool RMinus(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var rel))
                return false;
            var addrDst = dasm.rdr.Address - (int)rel;
            dasm.operands.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static bool QR(uint bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out var rel))
                return false;
            var addrDst = dasm.rdr.Address + rel;
            dasm.operands.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static bool NotYetImplemented(uint bInstr, M6812Disassembler dasm)
        {
            var instrHex = $"{bInstr:X2}";
            dasm.EmitUnitTest("M6812", instrHex, "", "M6812", dasm.addr, w =>
            {
                w.WriteLine("    Given_Code(\"{0:X2}\");", bInstr);
                w.WriteLine("    Expect_Instruction(\"@@@\");");
            });
            return true;
        }

        static M6812Disassembler()
        {
            BaseRegisters = new RegisterStorage[4]
            {
                Registers.x,
                Registers.y,
                Registers.sp,
                Registers.pc
            };

            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            decodersSecondByte = new Decoder[256]
            {
                // 00
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Instr(Mnemonic.aba),
                Instr(Mnemonic.daa),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Instr(Mnemonic.tab),
                Instr(Mnemonic.tba),
                // 10
                Instr(Mnemonic.idiv),
                Instr(Mnemonic.fdiv),
                Instr(Mnemonic.emacs, HL),
                Instr(Mnemonic.emuls),

                Instr(Mnemonic.edivs),
                Instr(Mnemonic.idivs),
                Instr(Mnemonic.sba),
                Instr(Mnemonic.cba),

                Instr(Mnemonic.maxa, XB),
                Instr(Mnemonic.mina, XB),
                Instr(Mnemonic.emaxd, XB),
                Instr(Mnemonic.emind, XB),

                Instr(Mnemonic.maxm, XB),
                Instr(Mnemonic.minm, XB),
                Instr(Mnemonic.emaxm, XB),
                Instr(Mnemonic.eminm, XB),
                // 20
                Instr(Mnemonic.lbra, InstrClass.Transfer, QR),
                Instr(Mnemonic.lbrn, QR),
                Instr(Mnemonic.lbhi, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbls, InstrClass.ConditionalTransfer, QR),

                Instr(Mnemonic.lbcc, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbcs, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbne, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbeq, InstrClass.ConditionalTransfer, QR),

                Instr(Mnemonic.lbvc, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbvs, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbpl, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbmi, InstrClass.ConditionalTransfer, QR),

                Instr(Mnemonic.lbge, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lblt, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lbgt, InstrClass.ConditionalTransfer, QR),
                Instr(Mnemonic.lble, InstrClass.ConditionalTransfer, QR),
                // 30
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.rev),
                Instr(Mnemonic.revw),

                Instr(Mnemonic.wav),
                Instr(Mnemonic.tbl, XB),
                Instr(Mnemonic.stop),
                Instr(Mnemonic.etbl, XB),
                // 40
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // 50
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // 60
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // 70
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // 80
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // 90
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // A0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // B0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // C0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // D0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // E0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // F0
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
            };

            // Bit 3 is don't care
            decodersLoops = new Decoder[256]
            {
                // 00 
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, D, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, X, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, Y, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, S, RPlus),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, D, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, X, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, Y, RPlus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, S, RPlus),
                // 10 
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, A, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, D, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, X, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, Y, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, S, RMinus),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, A, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, D, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, X, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, Y, RMinus),
                Instr(Mnemonic.dbeq, InstrClass.ConditionalTransfer, S, RMinus),
                // 20 
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, D, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, X, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, Y, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, S, RPlus),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, D, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, X, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, Y, RPlus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, S, RPlus),
                // 30 
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, A, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, D, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, X, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, Y, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, S, RMinus),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, A, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, D, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, X, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, Y, RMinus),
                Instr(Mnemonic.dbne, InstrClass.ConditionalTransfer, S, RMinus),
                // 40 
                Instr(Mnemonic.tbeq, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.tbeq, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbeq, D, RPlus),
                Instr(Mnemonic.tbeq, X, RPlus),
                Instr(Mnemonic.tbeq, Y, RPlus),
                Instr(Mnemonic.tbeq, S, RPlus),

                Instr(Mnemonic.tbeq, A, RPlus),
                Instr(Mnemonic.tbeq, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbeq, D, RPlus),
                Instr(Mnemonic.tbeq, X, RPlus),
                Instr(Mnemonic.tbeq, Y, RPlus),
                Instr(Mnemonic.tbeq, S, RPlus),
                // 50 
                Instr(Mnemonic.tbeq, A, RMinus),
                Instr(Mnemonic.tbeq, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbeq, D, RMinus),
                Instr(Mnemonic.tbeq, X, RMinus),
                Instr(Mnemonic.tbeq, Y, RMinus),
                Instr(Mnemonic.tbeq, S, RMinus),

                Instr(Mnemonic.tbeq, A, RMinus),
                Instr(Mnemonic.tbeq, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbeq, D, RMinus),
                Instr(Mnemonic.tbeq, X, RMinus),
                Instr(Mnemonic.tbeq, Y, RMinus),
                Instr(Mnemonic.tbeq, S, RMinus),
                // 60 
                Instr(Mnemonic.tbne, A, RPlus),
                Instr(Mnemonic.tbne, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbne, D, RPlus),
                Instr(Mnemonic.tbne, X, RPlus),
                Instr(Mnemonic.tbne, Y, RPlus),
                Instr(Mnemonic.tbne, S, RPlus),

                Instr(Mnemonic.tbne, A, RPlus),
                Instr(Mnemonic.tbne, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbne, D, RPlus),
                Instr(Mnemonic.tbne, X, RPlus),
                Instr(Mnemonic.tbne, Y, RPlus),
                Instr(Mnemonic.tbne, S, RPlus),
                // 70 
                Instr(Mnemonic.tbne, A, RMinus),
                Instr(Mnemonic.tbne, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbne, D, RMinus),
                Instr(Mnemonic.tbne, X, RMinus),
                Instr(Mnemonic.tbne, Y, RMinus),
                Instr(Mnemonic.tbne, S, RMinus),

                Instr(Mnemonic.tbne, A, RMinus),
                Instr(Mnemonic.tbne, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.tbne, D, RMinus),
                Instr(Mnemonic.tbne, X, RMinus),
                Instr(Mnemonic.tbne, Y, RMinus),
                Instr(Mnemonic.tbne, S, RMinus),
                // 80 
                Instr(Mnemonic.ibeq, InstrClass.ConditionalTransfer, A, RPlus),
                Instr(Mnemonic.ibeq, InstrClass.ConditionalTransfer, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibeq, D, RPlus),
                Instr(Mnemonic.ibeq, X, RPlus),
                Instr(Mnemonic.ibeq, Y, RPlus),
                Instr(Mnemonic.ibeq, S, RPlus),

                Instr(Mnemonic.ibeq, A, RPlus),
                Instr(Mnemonic.ibeq, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibeq, D, RPlus),
                Instr(Mnemonic.ibeq, X, RPlus),
                Instr(Mnemonic.ibeq, Y, RPlus),
                Instr(Mnemonic.ibeq, S, RPlus),
                // 90 
                Instr(Mnemonic.ibeq, A, RMinus),
                Instr(Mnemonic.ibeq, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibeq, D, RMinus),
                Instr(Mnemonic.ibeq, X, RMinus),
                Instr(Mnemonic.ibeq, Y, RMinus),
                Instr(Mnemonic.ibeq, S, RMinus),

                Instr(Mnemonic.ibeq, A, RMinus),
                Instr(Mnemonic.ibeq, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibeq, D, RMinus),
                Instr(Mnemonic.ibeq, X, RMinus),
                Instr(Mnemonic.ibeq, Y, RMinus),
                Instr(Mnemonic.ibeq, S, RMinus),
                // A0 
                Instr(Mnemonic.ibne, A, RPlus),
                Instr(Mnemonic.ibne, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibne, D, RPlus),
                Instr(Mnemonic.ibne, X, RPlus),
                Instr(Mnemonic.ibne, Y, RPlus),
                Instr(Mnemonic.ibne, S, RPlus),

                Instr(Mnemonic.ibne, A, RPlus),
                Instr(Mnemonic.ibne, B, RPlus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibne, D, RPlus),
                Instr(Mnemonic.ibne, X, RPlus),
                Instr(Mnemonic.ibne, Y, RPlus),
                Instr(Mnemonic.ibne, S, RPlus),
                // B0 
                Instr(Mnemonic.ibne, A, RMinus),
                Instr(Mnemonic.ibne, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibne, D, RMinus),
                Instr(Mnemonic.ibne, X, RMinus),
                Instr(Mnemonic.ibne, Y, RMinus),
                Instr(Mnemonic.ibne, S, RMinus),

                Instr(Mnemonic.ibne, A, RMinus),
                Instr(Mnemonic.ibne, B, RMinus),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.ibne, D, RMinus),
                Instr(Mnemonic.ibne, X, RMinus),
                Instr(Mnemonic.ibne, Y, RMinus),
                Instr(Mnemonic.ibne, S, RMinus),
                // C0 
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // D0 
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // E0 
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                // F0 
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),

                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
                Instr(Mnemonic.trap),
            };

            // Bits 3 and 7 are don't care
            decodersRegisters = new Decoder[256]
            {
                // 00
                Instr(Mnemonic.tfr, A,A),
                Instr(Mnemonic.tfr, A,B),
                Instr(Mnemonic.tfr, A,CCR),
                invalid,

                Instr(Mnemonic.sex, A,D),
                Instr(Mnemonic.sex, A,X),
                Instr(Mnemonic.sex, A,Y),
                Instr(Mnemonic.sex, A,S),

                Instr(Mnemonic.tfr, A,A),
                Instr(Mnemonic.tfr, A,B),
                Instr(Mnemonic.tfr, A,CCR),
                invalid,

                Instr(Mnemonic.sex, A,D),
                Instr(Mnemonic.sex, A,X),
                Instr(Mnemonic.sex, A,Y),
                Instr(Mnemonic.sex, A,S),
                // 10
                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.sex, B,D),
                Instr(Mnemonic.sex, B,X),
                Instr(Mnemonic.sex, B,Y),
                Instr(Mnemonic.sex, B,S),

                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.sex, B,D),
                Instr(Mnemonic.sex, B,X),
                Instr(Mnemonic.sex, B,Y),
                Instr(Mnemonic.sex, B,S),
                // 20
                Instr(Mnemonic.tfr, CCR,A),
                Instr(Mnemonic.tfr, CCR,B),
                Instr(Mnemonic.tfr, CCR,CCR),
                invalid,

                Instr(Mnemonic.sex, CCR,D),
                Instr(Mnemonic.sex, CCR,X),
                Instr(Mnemonic.sex, CCR,Y),
                Instr(Mnemonic.sex, CCR,S),

                Instr(Mnemonic.tfr, CCR,A),
                Instr(Mnemonic.tfr, CCR,B),
                Instr(Mnemonic.tfr, CCR,CCR),
                invalid,

                Instr(Mnemonic.sex, CCR,D),
                Instr(Mnemonic.sex, CCR,X),
                Instr(Mnemonic.sex, CCR,Y),
                Instr(Mnemonic.sex, CCR,S),
                // 30
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                // 40
                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.tfr, D,D),
                Instr(Mnemonic.tfr, D,X),
                Instr(Mnemonic.tfr, D,Y),
                Instr(Mnemonic.tfr, D,S),

                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.tfr, D,D),
                Instr(Mnemonic.tfr, D,X),
                Instr(Mnemonic.tfr, D,Y),
                Instr(Mnemonic.tfr, D,S),
                // 50
                Instr(Mnemonic.tfr, X,A),
                Instr(Mnemonic.tfr, X,B),
                Instr(Mnemonic.tfr, X,CCR),
                invalid,

                Instr(Mnemonic.tfr, X,D),
                Instr(Mnemonic.tfr, X,X),
                Instr(Mnemonic.tfr, X,Y),
                Instr(Mnemonic.tfr, X,S),

                Instr(Mnemonic.tfr, X,A),
                Instr(Mnemonic.tfr, X,B),
                Instr(Mnemonic.tfr, X,CCR),
                invalid,

                Instr(Mnemonic.tfr, X,D),
                Instr(Mnemonic.tfr, X,X),
                Instr(Mnemonic.tfr, X,Y),
                Instr(Mnemonic.tfr, X,S),
                // 60
                Instr(Mnemonic.tfr, Y,A),
                Instr(Mnemonic.tfr, Y,B),
                Instr(Mnemonic.tfr, Y,CCR),
                invalid,

                Instr(Mnemonic.tfr, Y,D),
                Instr(Mnemonic.tfr, Y,X),
                Instr(Mnemonic.tfr, Y,Y),
                Instr(Mnemonic.tfr, Y,S),

                Instr(Mnemonic.tfr, Y,A),
                Instr(Mnemonic.tfr, Y,B),
                Instr(Mnemonic.tfr, Y,CCR),
                invalid,

                Instr(Mnemonic.tfr, Y,D),
                Instr(Mnemonic.tfr, Y,X),
                Instr(Mnemonic.tfr, Y,Y),
                Instr(Mnemonic.tfr, Y,S),
                // 70
                Instr(Mnemonic.tfr, S,A),
                Instr(Mnemonic.tfr, S,B),
                Instr(Mnemonic.tfr, S,CCR),
                invalid,

                Instr(Mnemonic.tfr, S,D),
                Instr(Mnemonic.tfr, S,X),
                Instr(Mnemonic.tfr, S,Y),
                Instr(Mnemonic.tfr, S,S),

                Instr(Mnemonic.tfr, S,A),
                Instr(Mnemonic.tfr, S,B),
                Instr(Mnemonic.tfr, S,CCR),
                invalid,

                Instr(Mnemonic.tfr, S,D),
                Instr(Mnemonic.tfr, S,X),
                Instr(Mnemonic.tfr, S,Y),
                Instr(Mnemonic.tfr, S,S),
                // 80
                Instr(Mnemonic.tfr, A,A),
                Instr(Mnemonic.tfr, A,B),
                Instr(Mnemonic.tfr, A,CCR),
                invalid,

                Instr(Mnemonic.sex, A,D),
                Instr(Mnemonic.sex, A,X),
                Instr(Mnemonic.sex, A,Y),
                Instr(Mnemonic.sex, A,S),

                Instr(Mnemonic.tfr, A,A),
                Instr(Mnemonic.tfr, A,B),
                Instr(Mnemonic.tfr, A,CCR),
                invalid,

                Instr(Mnemonic.sex, A,D),
                Instr(Mnemonic.sex, A,X),
                Instr(Mnemonic.sex, A,Y),
                Instr(Mnemonic.sex, A,S),
                // 90
                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.sex, B,D),
                Instr(Mnemonic.sex, B,X),
                Instr(Mnemonic.sex, B,Y),
                Instr(Mnemonic.sex, B,S),

                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.sex, B,D),
                Instr(Mnemonic.sex, B,X),
                Instr(Mnemonic.sex, B,Y),
                Instr(Mnemonic.sex, B,S),
                // A0
                Instr(Mnemonic.tfr, CCR,A),
                Instr(Mnemonic.tfr, CCR,B),
                Instr(Mnemonic.tfr, CCR,CCR),
                invalid,

                Instr(Mnemonic.sex, CCR,D),
                Instr(Mnemonic.sex, CCR,X),
                Instr(Mnemonic.sex, CCR,Y),
                Instr(Mnemonic.sex, CCR,S),

                Instr(Mnemonic.tfr, CCR,A),
                Instr(Mnemonic.tfr, CCR,B),
                Instr(Mnemonic.tfr, CCR,CCR),
                invalid,

                Instr(Mnemonic.sex, CCR,D),
                Instr(Mnemonic.sex, CCR,X),
                Instr(Mnemonic.sex, CCR,Y),
                Instr(Mnemonic.sex, CCR,S),
                // B0
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                // C0
                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.tfr, D,D),
                Instr(Mnemonic.tfr, D,X),
                Instr(Mnemonic.tfr, D,Y),
                Instr(Mnemonic.tfr, D,S),

                Instr(Mnemonic.tfr, B,A),
                Instr(Mnemonic.tfr, B,B),
                Instr(Mnemonic.tfr, B,CCR),
                invalid,

                Instr(Mnemonic.tfr, D,D),
                Instr(Mnemonic.tfr, D,X),
                Instr(Mnemonic.tfr, D,Y),
                Instr(Mnemonic.tfr, D,S),
                // D0
                Instr(Mnemonic.tfr, X,A),
                Instr(Mnemonic.tfr, X,B),
                Instr(Mnemonic.tfr, X,CCR),
                invalid,

                Instr(Mnemonic.tfr, X,D),
                Instr(Mnemonic.tfr, X,X),
                Instr(Mnemonic.tfr, X,Y),
                Instr(Mnemonic.tfr, X,S),

                Instr(Mnemonic.tfr, X,A),
                Instr(Mnemonic.tfr, X,B),
                Instr(Mnemonic.tfr, X,CCR),
                invalid,

                Instr(Mnemonic.tfr, X,D),
                Instr(Mnemonic.tfr, X,X),
                Instr(Mnemonic.tfr, X,Y),
                Instr(Mnemonic.tfr, X,S),
                // E0
                Instr(Mnemonic.tfr, Y,A),
                Instr(Mnemonic.tfr, Y,B),
                Instr(Mnemonic.tfr, Y,CCR),
                invalid,

                Instr(Mnemonic.tfr, Y,D),
                Instr(Mnemonic.tfr, Y,X),
                Instr(Mnemonic.tfr, Y,Y),
                Instr(Mnemonic.tfr, Y,S),

                Instr(Mnemonic.tfr, Y,A),
                Instr(Mnemonic.tfr, Y,B),
                Instr(Mnemonic.tfr, Y,CCR),
                invalid,

                Instr(Mnemonic.tfr, Y,D),
                Instr(Mnemonic.tfr, Y,X),
                Instr(Mnemonic.tfr, Y,Y),
                Instr(Mnemonic.tfr, Y,S),
                // F0
                Instr(Mnemonic.tfr, S,A),
                Instr(Mnemonic.tfr, S,B),
                Instr(Mnemonic.tfr, S,CCR),
                invalid,

                Instr(Mnemonic.tfr, S,D),
                Instr(Mnemonic.tfr, S,X),
                Instr(Mnemonic.tfr, S,Y),
                Instr(Mnemonic.tfr, S,S),

                Instr(Mnemonic.tfr, S,A),
                Instr(Mnemonic.tfr, S,B),
                Instr(Mnemonic.tfr, S,CCR),
                invalid,

                Instr(Mnemonic.tfr, S,D),
                Instr(Mnemonic.tfr, S,X),
                Instr(Mnemonic.tfr, S,Y),
                Instr(Mnemonic.tfr, S,S),
            };

            decoders = new Decoder[256]
            {
                // 00
                Instr(Mnemonic.bgnd, InstrClass.Linear|InstrClass.Zero),
                Instr(Mnemonic.mem),
                Instr(Mnemonic.iny),
                Instr(Mnemonic.dey),

                new NextByteDecoder(decodersLoops),
                Instr(Mnemonic.jmp, InstrClass.Transfer, XB),
                Instr(Mnemonic.jmp, InstrClass.Transfer, HL),
                Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, R),

                Instr(Mnemonic.inx),
                Instr(Mnemonic.dex),
                Instr(Mnemonic.rtc, InstrClass.Transfer),
                Instr(Mnemonic.rti, InstrClass.Transfer),

                Instr(Mnemonic.bset, XB, I),
                Instr(Mnemonic.bclr, XB, I),
                Instr(Mnemonic.brset, XB, I, R),
                Instr(Mnemonic.brclr, XB, I, R),
                // 10
                Instr(Mnemonic.andcc, I),
                Instr(Mnemonic.ediv),
                Instr(Mnemonic.mul),
                Instr(Mnemonic.emul),

                Instr(Mnemonic.orcc, I),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, XB),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, HL),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Dir),

                new NextByteDecoder(decodersSecondByte),
                Instr(Mnemonic.leay, XB),
                Instr(Mnemonic.leax, XB),
                Instr(Mnemonic.leas, XB),

                Instr(Mnemonic.bset, HL,I),
                Instr(Mnemonic.bclr, HL,I),
                Instr(Mnemonic.brset, HL, I, R),
                Instr(Mnemonic.brclr, HL, I, R),
                // 20
                Instr(Mnemonic.bra, InstrClass.Transfer, R),
                Instr(Mnemonic.brn, R),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, R),

                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, R),

                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, R),

                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, R),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, R),
                // 30
                Instr(Mnemonic.pulx),
                Instr(Mnemonic.puly),
                Instr(Mnemonic.pula),
                Instr(Mnemonic.pulb),

                Instr(Mnemonic.pshx),
                Instr(Mnemonic.pshy),
                Instr(Mnemonic.psha),
                Instr(Mnemonic.pshb),

                Instr(Mnemonic.pulc),
                Instr(Mnemonic.pshc),
                Instr(Mnemonic.puld),
                Instr(Mnemonic.pshd),

                Instr(Mnemonic.wav),
                Instr(Mnemonic.rts),
                Instr(Mnemonic.wai),
                Instr(Mnemonic.swi),
                // 40
                Instr(Mnemonic.nega),
                Instr(Mnemonic.coma),
                Instr(Mnemonic.inca),
                Instr(Mnemonic.deca),

                Instr(Mnemonic.lsra),
                Instr(Mnemonic.rola),
                Instr(Mnemonic.rora),
                Instr(Mnemonic.asra),

                Instr(Mnemonic.lsla),
                Instr(Mnemonic.lsrd),
                Instr(Mnemonic.call, HL,PG),
                Instr(Mnemonic.call, HL,PG),

                Instr(Mnemonic.bset, Dir,I),
                Instr(Mnemonic.bclr, Dir,I),
                Instr(Mnemonic.brset, InstrClass.ConditionalTransfer, Dir, I, R),
                Instr(Mnemonic.brclr, InstrClass.ConditionalTransfer, Dir, I, R),
                // 50
                Instr(Mnemonic.negb),
                Instr(Mnemonic.comb),
                Instr(Mnemonic.incb),
                Instr(Mnemonic.decb),

                Instr(Mnemonic.lsrb),
                Instr(Mnemonic.rolb),
                Instr(Mnemonic.rorb),
                Instr(Mnemonic.asrb),

                Instr(Mnemonic.lslb),
                Instr(Mnemonic.lsld),
                Instr(Mnemonic.staa, Dir),
                Instr(Mnemonic.stab, Dir),

                Instr(Mnemonic.std, Dir),
                Instr(Mnemonic.sty, Dir),
                Instr(Mnemonic.stx, Dir),
                Instr(Mnemonic.sts, Dir),
                // 60
                Instr(Mnemonic.neg, XB),
                Instr(Mnemonic.com, XB),
                Instr(Mnemonic.inc, XB),
                Instr(Mnemonic.dec, XB),

                Instr(Mnemonic.lsr, XB),
                Instr(Mnemonic.rol, XB),
                Instr(Mnemonic.ror, XB),
                Instr(Mnemonic.asr, XB),

                Instr(Mnemonic.lsl, XB),
                Instr(Mnemonic.clr, XB),
                Instr(Mnemonic.staa, XB),
                Instr(Mnemonic.stab, XB),

                Instr(Mnemonic.std, XB),
                Instr(Mnemonic.sty, XB),
                Instr(Mnemonic.stx, XB),
                Instr(Mnemonic.sts, XB),
                // 70
                Instr(Mnemonic.neg, HL),
                Instr(Mnemonic.com, HL),
                Instr(Mnemonic.inc, HL),
                Instr(Mnemonic.dec, HL),

                Instr(Mnemonic.lsr, HL),
                Instr(Mnemonic.rol, HL),
                Instr(Mnemonic.ror, HL),
                Instr(Mnemonic.asr, HL),

                Instr(Mnemonic.lsl, HL),
                Instr(Mnemonic.clr, HL),
                Instr(Mnemonic.staa, HL),
                Instr(Mnemonic.stab, HL),

                Instr(Mnemonic.std, HL),
                Instr(Mnemonic.sty, HL),
                Instr(Mnemonic.stx, HL),
                Instr(Mnemonic.sts, HL),
                // 80
                Instr(Mnemonic.suba, I),
                Instr(Mnemonic.cmpa, I),
                Instr(Mnemonic.sbca, I),
                Instr(Mnemonic.subd, JK),

                Instr(Mnemonic.anda, I),
                Instr(Mnemonic.bita, I),
                Instr(Mnemonic.ldaa, I),
                Instr(Mnemonic.clra),

                Instr(Mnemonic.eora, I),
                Instr(Mnemonic.adca, I),
                Instr(Mnemonic.oraa, I),
                Instr(Mnemonic.adda, I),

                Instr(Mnemonic.cpd, JK),
                Instr(Mnemonic.cpy, JK),
                Instr(Mnemonic.cpx, JK),
                Instr(Mnemonic.cps, JK),
                // 90
                Instr(Mnemonic.suba, Dir),
                Instr(Mnemonic.cmpa, Dir),
                Instr(Mnemonic.sbca, Dir),
                Instr(Mnemonic.subd, Dir),

                Instr(Mnemonic.anda, Dir),
                Instr(Mnemonic.bita, Dir),
                Instr(Mnemonic.ldaa, Dir),
                Instr(Mnemonic.tsta),

                Instr(Mnemonic.eora, Dir),
                Instr(Mnemonic.adca, Dir),
                Instr(Mnemonic.oraa, Dir),
                Instr(Mnemonic.adda, Dir),

                Instr(Mnemonic.cpd, Dir),
                Instr(Mnemonic.cpy, Dir),
                Instr(Mnemonic.cpx, Dir),
                Instr(Mnemonic.cps, Dir),
                // A0
                Instr(Mnemonic.suba, XB),
                Instr(Mnemonic.cmpa, XB),
                Instr(Mnemonic.sbca, XB),
                Instr(Mnemonic.subd, XB),

                Instr(Mnemonic.anda, XB),
                Instr(Mnemonic.bita, XB),
                Instr(Mnemonic.ldaa, XB),
                Instr(Mnemonic.nop),

                Instr(Mnemonic.eora, XB),
                Instr(Mnemonic.adca, XB),
                Instr(Mnemonic.oraa, XB),
                Instr(Mnemonic.adda, XB),

                Instr(Mnemonic.cpd, XB),
                Instr(Mnemonic.cpy, XB),
                Instr(Mnemonic.cpx, XB),
                Instr(Mnemonic.cps, XB),
                // B0
                Instr(Mnemonic.suba, HL),
                Instr(Mnemonic.cmpa, HL),
                Instr(Mnemonic.sbca, HL),
                Instr(Mnemonic.subd, HL),

                Instr(Mnemonic.anda, HL),
                Instr(Mnemonic.bita, HL),
                Instr(Mnemonic.ldaa, HL),
                new NextByteDecoder(decodersRegisters),

                Instr(Mnemonic.eora, HL),
                Instr(Mnemonic.adca, HL),
                Instr(Mnemonic.oraa, HL),
                Instr(Mnemonic.adda, HL),

                Instr(Mnemonic.cpd, HL),
                Instr(Mnemonic.cpy, HL),
                Instr(Mnemonic.cpx, HL),
                Instr(Mnemonic.cps, HL),
                // C0
                Instr(Mnemonic.subb, I),
                Instr(Mnemonic.cmpb, I),
                Instr(Mnemonic.sbcb, I),
                Instr(Mnemonic.addd, JK),

                Instr(Mnemonic.andb, I),
                Instr(Mnemonic.bitb, I),
                Instr(Mnemonic.ldab, I),
                Instr(Mnemonic.clrb),

                Instr(Mnemonic.eorb, I),
                Instr(Mnemonic.adcb, I),
                Instr(Mnemonic.orab, I),
                Instr(Mnemonic.addb, I),

                Instr(Mnemonic.ldd, JK),
                Instr(Mnemonic.ldy, JK),
                Instr(Mnemonic.ldx, JK),
                Instr(Mnemonic.lds, JK),
                // D0
                Instr(Mnemonic.subb, Dir),
                Instr(Mnemonic.cmpb, Dir),
                Instr(Mnemonic.sbcb, Dir),
                Instr(Mnemonic.addd, Dir),

                Instr(Mnemonic.andb, Dir),
                Instr(Mnemonic.bitb, Dir),
                Instr(Mnemonic.ldab, Dir),
                Instr(Mnemonic.tstb),

                Instr(Mnemonic.eorb, Dir),
                Instr(Mnemonic.adcb, Dir),
                Instr(Mnemonic.orab, Dir),
                Instr(Mnemonic.addb, Dir),

                Instr(Mnemonic.ldd, Dir),
                Instr(Mnemonic.ldy, Dir),
                Instr(Mnemonic.ldx, Dir),
                Instr(Mnemonic.lds, Dir),
                // E0
                Instr(Mnemonic.subb, XB),
                Instr(Mnemonic.cmpb, XB),
                Instr(Mnemonic.sbcb, XB),
                Instr(Mnemonic.addb, XB),

                Instr(Mnemonic.andb, XB),
                Instr(Mnemonic.bitb, XB),
                Instr(Mnemonic.ldab, XB),
                Instr(Mnemonic.tst, XB),

                Instr(Mnemonic.eorb, XB),
                Instr(Mnemonic.adcb, XB),
                Instr(Mnemonic.orab, Dir),
                Instr(Mnemonic.addb, XB),

                Instr(Mnemonic.ldd, XB),
                Instr(Mnemonic.ldy, XB),
                Instr(Mnemonic.ldx, XB),
                Instr(Mnemonic.lds, XB),
                // F0
                Instr(Mnemonic.subb, HL),
                Instr(Mnemonic.cmpb, HL),
                Instr(Mnemonic.sbcb, HL),
                Instr(Mnemonic.addd, HL),

                Instr(Mnemonic.andb, HL),
                Instr(Mnemonic.bitb, HL),
                Instr(Mnemonic.ldab, HL),
                Instr(Mnemonic.tst, HL),

                Instr(Mnemonic.eorb, HL),
                Instr(Mnemonic.adcb, HL),
                Instr(Mnemonic.orab, HL),
                Instr(Mnemonic.addb, HL),

                Instr(Mnemonic.ldd, HL),
                Instr(Mnemonic.ldy, HL),
                Instr(Mnemonic.ldx, HL),
                Instr(Mnemonic.lds, HL),
            };
        }
    }
}
