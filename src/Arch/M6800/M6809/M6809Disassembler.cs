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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.M6800.M6809
{
    using Decoder = Decoder<M6809Disassembler, Mnemonic, M6809Instruction>;

    public class M6809Disassembler : DisassemblerBase<M6809Instruction, Mnemonic>
    {
        private readonly static Decoder<M6809Disassembler, Mnemonic, M6809Instruction>[] rootDecoders;

        private readonly M6809Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private InstrClass iclass;

        public M6809Disassembler(M6809Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override M6809Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
                return null;
            this.ops.Clear();
            this.iclass = InstrClass.None;
            var instr = rootDecoders[b].Decode(b, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override M6809Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new M6809Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
            return instr;
        }

        public override M6809Instruction CreateInvalidInstruction()
        {
            return new M6809Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        public override M6809Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("M6809Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators

        private static Mutator<M6809Disassembler> Direct(PrimitiveType dt)
        {
            return (uint wInstr, M6809Disassembler dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out var b))
                    return false;
                dasm.ops.Add(new MemoryOperand(PrimitiveType.Byte)
                {
                    AccessMode = MemoryOperand.Mode.Direct,
                    Offset = b,
                });
                return true;
            };
        }

        private static readonly Mutator<M6809Disassembler> Dir = Direct(PrimitiveType.Byte);
        private static readonly Mutator<M6809Disassembler> Dirw = Direct(PrimitiveType.Word16);

        private static bool A(uint wInstr, M6809Disassembler dasm)
        {
            dasm.ops.Add(Registers.A);
            return true;
        }

        private static bool B(uint wInstr, M6809Disassembler dasm)
        {
            dasm.ops.Add(Registers.B);
            return true;
        }

        private static Mutator<M6809Disassembler> Indexed(PrimitiveType dt)
        {
            return (wInstr, dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return false;
                var iReg = (b >> 5) & 3;
                var reg = Registers.AddrRegs[iReg];
                int offset = 0;
                RegisterStorage? idx = null;
                MemoryOperand.Mode mode;
                bool indirect;
                if ((b & 0x80) == 0)
                {
                    indirect = false;
                    mode = MemoryOperand.Mode.ConstantOffset;
                    offset = (int) Bits.SignExtend(b, 5);
                }
                else
                {
                    indirect = (b & 0x10) != 0;
                    switch (b & 0xF)
                    {
                    case 0b0000:
                        if (indirect)
                            return false;
                        mode = MemoryOperand.Mode.PostInc1;
                        break;
                    case 0b0001:
                        mode = MemoryOperand.Mode.PostInc2;
                        break;
                    case 0b0010:
                        if (indirect)
                            return false;
                        mode = MemoryOperand.Mode.PreDec1;
                        break;
                    case 0b0011:
                        mode = MemoryOperand.Mode.PreDec2;
                        break;
                    case 0b0100:
                        mode = MemoryOperand.Mode.ConstantOffset;
                        break;
                    case 0b0101:
                        mode = MemoryOperand.Mode.AccumulatorOffset;
                        idx = Registers.B;
                        break;
                    case 0b0110:
                        mode = MemoryOperand.Mode.AccumulatorOffset;
                        idx = Registers.A;
                        break;
                    case 0b1000:
                        if (!dasm.rdr.TryReadByte(out byte bOffset))
                            return false;
                        mode = MemoryOperand.Mode.ConstantOffset;
                        offset = (sbyte) bOffset;
                        break;
                    case 0b1001:
                        if (!dasm.rdr.TryReadBeUInt16(out ushort usOffset))
                            return false;
                        mode = MemoryOperand.Mode.ConstantOffset;
                        offset = usOffset;
                        break;
                    case 0b1011:
                        mode = MemoryOperand.Mode.AccumulatorOffset;
                        idx = Registers.D;
                        break;
                    case 0b1100:
                        if (!dasm.rdr.TryReadByte(out bOffset))
                            return false;
                        mode = MemoryOperand.Mode.ConstantOffset;
                        reg = Registers.PCR;
                        offset = (sbyte) bOffset;
                        break;
                    case 0b1101:
                        if (!dasm.rdr.TryReadBeInt16(out short sOffset))
                            return false;
                        mode = MemoryOperand.Mode.ConstantOffset;
                        reg = Registers.PCR;
                        offset = sOffset;
                        break;
                    case 0b1111:
                        if (b != 0x9F || !indirect)
                            return false;
                        if (!dasm.rdr.TryReadBeUInt16(out usOffset))
                            return false;
                        mode = MemoryOperand.Mode.ConstantOffset;
                        reg = null;
                        offset = usOffset;
                        break;
                    default:
                        return false;
                    }
                }
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    AccessMode = mode,
                    Base = reg,
                    Index = idx,
                    Offset = offset,
                    Indirect = indirect,
                });
                return true;
            };
        }

        private static readonly Mutator<M6809Disassembler> Idx = Indexed(PrimitiveType.Byte);
        private static readonly Mutator<M6809Disassembler> IdxW = Indexed(PrimitiveType.Word16);


        private static Mutator<M6809Disassembler> Extended(PrimitiveType dt)
        {
            return (uint wInstr, M6809Disassembler dasm) =>
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort usOffset))
                    return false;
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    AccessMode = MemoryOperand.Mode.ConstantOffset,
                    Offset = usOffset,
                });
                return true;
            };
        }

        private static readonly Mutator<M6809Disassembler> Ext = Extended(PrimitiveType.Byte);
        private static readonly Mutator<M6809Disassembler> ExtW = Extended(PrimitiveType.Word16);

        private static bool Ib(uint wInstr, M6809Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(Constant.Byte(b));
            return true;
        }

        private static bool Iw(uint wInstr, M6809Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort u))
                return false;
            dasm.ops.Add(Constant.Word16(u));
            return true;
        }

        private static bool PCb(uint wInstr, M6809Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var addr = dasm.rdr.Address + (sbyte)b;
            dasm.ops.Add(addr);
            return true;
        }

        private static bool PCw(uint wInstr, M6809Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short d))
                return false;
            var addr = dasm.rdr.Address + d;
            dasm.ops.Add(addr);
            return true;
        }


        private static readonly RegisterStorage[] regsS =
        {
            Registers.PCR,
            Registers.S,
            Registers.Y,
            Registers.X,
            Registers.DP,
            Registers.B,
            Registers.A,
            Registers.CC
        };

        private static readonly RegisterStorage[] regsU =
        {
            Registers.PCR,
            Registers.U,
            Registers.Y,
            Registers.X,
            Registers.DP,
            Registers.B,
            Registers.A,
            Registers.CC
        };
        private static Mutator<M6809Disassembler> Mregs(bool writeRegs, RegisterStorage[] regs)
        {
            return (uint wInstr, M6809Disassembler dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return false;
                var addr = dasm.rdr.Address + (sbyte) b;
                dasm.ops.Add(new MultipleRegisterOperand(regs, b));
                if (writeRegs && (b & 0x80) != 0)
                {
                    // PCR register was written.
                    dasm.iclass = InstrClass.Transfer | InstrClass.Return;
                }
                return true;
            };
        }

        private static readonly RegisterStorage?[] exgRegs = new RegisterStorage?[16]
        {
            Registers.D,
            Registers.X,
            Registers.Y,
            Registers.U,

            Registers.S,
            Registers.PCR,
            null,
            null,

            Registers.A,
            Registers.B,
            Registers.CC,
            Registers.DP,

            null,
            null,
            null,
            null,
        };
        private static bool Exg(uint wInstr, M6809Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var r0 = exgRegs[(uint) b >> 4];
            var r1 = exgRegs[b & 0xF];
            if (r0 is null || r1 is null)
                return false;
            dasm.ops.Add(r0);
            dasm.ops.Add(r1);
            return true;
        }

        #endregion

        #region Decoders

        private class InstructionDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<M6809Disassembler>[] mutators;

            public InstructionDecoder(InstrClass iclass, Mnemonic mnemonic, params Mutator<M6809Disassembler> [] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override M6809Instruction Decode(uint wInstr, M6809Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var iclass = dasm.iclass != InstrClass.None
                    ? dasm.iclass
                    : this.iclass;
                return dasm.MakeInstruction(iclass, this.mnemonic);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<M6809Disassembler>[] mutators)
        {
            return new InstructionDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<M6809Disassembler>[] mutators)
        {
            return new InstructionDecoder(iclass, mnemonic, mutators);
        }

        private class PrefixDecoder : Decoder
        {
            private readonly Decoder[] subDecoders;

            public PrefixDecoder(Decoder [] subDecoders)
            {
                this.subDecoders = subDecoders;
            }

            public override M6809Instruction Decode(uint wInstr, M6809Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte opcode))
                    return dasm.CreateInvalidInstruction();
                return subDecoders[opcode].Decode(opcode, dasm);
            }
        }

        #endregion

        static M6809Disassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            var page2 = new Decoder[256]
            {
                // 00
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

                // 10
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

                // 20
                invalid,
                Instr(Mnemonic.lbrn, PCw),
                Instr(Mnemonic.lbhi, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbls, InstrClass.ConditionalTransfer, PCw),

                Instr(Mnemonic.lbhs, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lblo, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbne, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbeq, InstrClass.ConditionalTransfer, PCw),

                Instr(Mnemonic.lbvc, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbvs, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbpl, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbmi, InstrClass.ConditionalTransfer, PCw),

                Instr(Mnemonic.lbge, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lblt, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lbgt, InstrClass.ConditionalTransfer, PCw),
                Instr(Mnemonic.lble, InstrClass.ConditionalTransfer, PCw),

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
                Instr(Mnemonic.swi2),

                // 40
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

                // 50
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

                // 60
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

                // 70
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

                // 80
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpd, Iw),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmpy, Iw),
                invalid,
                Instr(Mnemonic.ldy, Iw),
                invalid,

                // 90
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpd, Dirw),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmpy, Dirw),
                invalid,
                Instr(Mnemonic.ldy, Dirw),
                Instr(Mnemonic.sty, Dirw),

                // A0
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpd, IdxW),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmpy, IdxW),
                invalid,
                Instr(Mnemonic.ldy, IdxW),
                Instr(Mnemonic.sty, IdxW),

                // B0
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpd, ExtW),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmpy, ExtW),
                invalid,
                Instr(Mnemonic.ldy, ExtW),
                Instr(Mnemonic.sty, ExtW),

                // C0
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
                Instr(Mnemonic.lds, Iw),
                invalid,

                // D0
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
                Instr(Mnemonic.lds, Dirw),
                Instr(Mnemonic.sts, Dirw),

                // E0
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
                Instr(Mnemonic.lds, IdxW),
                Instr(Mnemonic.sts, IdxW),

                // F0
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
                Instr(Mnemonic.lds, ExtW),
                Instr(Mnemonic.sts, ExtW),
            };

            var page3 = new Decoder[256]
            {
                // 00
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

                // 10
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

                // 20
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
                Instr(Mnemonic.swi3),

                // 40
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

                // 50
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

                // 60
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

                // 70
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

                // 80
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpu, Iw),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmps, Iw),
                invalid,
                invalid,
                invalid,

                // 90
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpu, Dirw),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmps, Dirw),
                invalid,
                invalid,
                invalid,

                // A0
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpu, IdxW),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmps, IdxW),
                invalid,
                invalid,
                invalid,

                // B0
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cmpu, ExtW),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.cmps, ExtW),
                invalid,
                invalid,
                invalid,

                // C0
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

                // D0
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

                // E0
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

                // F0
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
};

            rootDecoders = new Decoder[256]
            {
                // 00
                Instr(Mnemonic.neg, Dir),
                invalid,
                invalid,
                Instr(Mnemonic.com, Dir),

                Instr(Mnemonic.lsr, Dir),
                invalid,
                Instr(Mnemonic.ror, Dir),
                Instr(Mnemonic.asr, Dir),

                Instr(Mnemonic.lsl, Dir),
                Instr(Mnemonic.rol, Dir),
                Instr(Mnemonic.dec, Dir),
                invalid,

                Instr(Mnemonic.inc, Dir),
                Instr(Mnemonic.tst, Dir),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Dir),
                Instr(Mnemonic.clr, Dir),

                // 10
                new PrefixDecoder(page2),
                new PrefixDecoder(page3),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.sync),

                invalid,
                invalid,
                Instr(Mnemonic.lbra, InstrClass.Transfer, PCw),
                Instr(Mnemonic.lbsr, InstrClass.Transfer|InstrClass.Call, PCw),

                invalid,
                Instr(Mnemonic.daa),
                Instr(Mnemonic.orcc, Ib),
                invalid,

                Instr(Mnemonic.andcc, Ib),
                Instr(Mnemonic.sex),
                Instr(Mnemonic.exg, Exg),
                Instr(Mnemonic.tfr, Exg),

                // 20
                Instr(Mnemonic.bra, InstrClass.Transfer, PCb),
                Instr(Mnemonic.brn, InstrClass.Linear, PCb),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, PCb),

                Instr(Mnemonic.bhs, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.blo, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, PCb),

                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, PCb),

                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, PCb),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, PCb),

                // 30
                Instr(Mnemonic.leax, Idx),
                Instr(Mnemonic.leay, Idx),
                Instr(Mnemonic.leas, Idx),
                Instr(Mnemonic.leau, Idx),

                Instr(Mnemonic.pshs, Mregs(false, regsU)),
                Instr(Mnemonic.puls, Mregs(true, regsU)),
                Instr(Mnemonic.pshu, Mregs(false, regsS)),
                Instr(Mnemonic.pulu, Mregs(true, regsS)),

                invalid,
                Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.abx),
                Instr(Mnemonic.rti, InstrClass.Transfer|InstrClass.Return),

                Instr(Mnemonic.cwai, Ib),
                Instr(Mnemonic.mul),
                invalid,
                Instr(Mnemonic.swi),

                // 40
                Instr(Mnemonic.neg, A),
                invalid,
                invalid,
                Instr(Mnemonic.com, A),

                Instr(Mnemonic.lsr, A),
                invalid,
                Instr(Mnemonic.ror, A),
                Instr(Mnemonic.asr, A),

                Instr(Mnemonic.lsl, A),
                Instr(Mnemonic.rol, A),
                Instr(Mnemonic.dec, A),
                invalid,

                Instr(Mnemonic.inc, A),
                Instr(Mnemonic.tst, A),
                invalid,
                Instr(Mnemonic.clr, A),

                // 50
                Instr(Mnemonic.neg, B),
                invalid,
                invalid,
                Instr(Mnemonic.com, B),

                Instr(Mnemonic.lsr, B),
                invalid,
                Instr(Mnemonic.ror, B),
                Instr(Mnemonic.asr, B),

                Instr(Mnemonic.lsl, B),
                Instr(Mnemonic.rol, B),
                Instr(Mnemonic.dec, B),
                invalid,

                Instr(Mnemonic.inc, B),
                Instr(Mnemonic.tst, B),
                invalid,
                Instr(Mnemonic.clr, B),

                // 60
                Instr(Mnemonic.neg, Idx),
                invalid,
                invalid,
                Instr(Mnemonic.com, Idx),

                Instr(Mnemonic.lsr, Idx),
                invalid,
                Instr(Mnemonic.ror, Idx),
                Instr(Mnemonic.asr, Idx),

                Instr(Mnemonic.lsl, Idx),
                Instr(Mnemonic.rol, Idx),
                Instr(Mnemonic.dec, Idx),
                invalid,

                Instr(Mnemonic.inc, Idx),
                Instr(Mnemonic.tst, Idx),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Idx),
                Instr(Mnemonic.clr, Idx),

                // 70
                Instr(Mnemonic.neg, Ext),
                invalid,
                invalid,
                Instr(Mnemonic.com, Ext),

                Instr(Mnemonic.lsr, Ext),
                invalid,
                Instr(Mnemonic.ror, Ext),
                Instr(Mnemonic.asr, Ext),

                Instr(Mnemonic.lsl, Ext),
                Instr(Mnemonic.rol, Ext),
                Instr(Mnemonic.dec, Ext),
                invalid,

                Instr(Mnemonic.inc, Ext),
                Instr(Mnemonic.tst, Ext),
                Instr(Mnemonic.tst, Ext),
                Instr(Mnemonic.clr, Ext),

                // 80
                Instr(Mnemonic.suba, Ib),
                Instr(Mnemonic.cmpa, Ib),
                Instr(Mnemonic.sbca, Ib),
                Instr(Mnemonic.subd, Iw),

                Instr(Mnemonic.anda, Ib),
                Instr(Mnemonic.bita, Ib),
                Instr(Mnemonic.lda, Ib),
                invalid, 

                Instr(Mnemonic.eora, Ib),
                Instr(Mnemonic.adca, Ib),
                Instr(Mnemonic.ora, Ib),
                Instr(Mnemonic.adda, Ib),

                Instr(Mnemonic.cmpx, Iw),
                Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, PCb),
                Instr(Mnemonic.ldx, Iw),
                invalid,

                // 90
                Instr(Mnemonic.suba, Dir),
                Instr(Mnemonic.cmpa, Dir),
                Instr(Mnemonic.sbca, Dir),
                Instr(Mnemonic.subd, Dirw),

                Instr(Mnemonic.anda, Dir),
                Instr(Mnemonic.bita, Dir),
                Instr(Mnemonic.lda, Dir),
                Instr(Mnemonic.sta, Dir),

                Instr(Mnemonic.eora, Dir),
                Instr(Mnemonic.adca, Dir),
                Instr(Mnemonic.ora, Dir),
                Instr(Mnemonic.adda, Dir),

                Instr(Mnemonic.cmpx, Dirw),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Dirw),
                Instr(Mnemonic.ldx, Dirw),
                Instr(Mnemonic.stx, Dirw),

                // A0
                Instr(Mnemonic.suba, Idx),
                Instr(Mnemonic.cmpa, Idx),
                Instr(Mnemonic.sbca, Idx),
                Instr(Mnemonic.subd, IdxW),

                Instr(Mnemonic.anda, Idx),
                Instr(Mnemonic.bita, Idx),
                Instr(Mnemonic.lda, Idx),
                Instr(Mnemonic.sta, Idx),

                Instr(Mnemonic.eora, Idx),
                Instr(Mnemonic.adca, Idx),
                Instr(Mnemonic.ora, Idx),
                Instr(Mnemonic.adda, Idx),

                Instr(Mnemonic.cmpx, IdxW),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, IdxW),
                Instr(Mnemonic.ldx, IdxW),
                Instr(Mnemonic.stx, IdxW),

                // B0
                Instr(Mnemonic.suba, Ext),
                Instr(Mnemonic.cmpa, Ext),
                Instr(Mnemonic.sbca, Ext),
                Instr(Mnemonic.subd, ExtW),

                Instr(Mnemonic.anda, Ext),
                Instr(Mnemonic.bita, Ext),
                Instr(Mnemonic.lda, Ext),
                Instr(Mnemonic.sta, Ext),

                Instr(Mnemonic.eora, Ext),
                Instr(Mnemonic.adca, Ext),
                Instr(Mnemonic.ora, Ext),
                Instr(Mnemonic.adda, Ext),

                Instr(Mnemonic.cmpx, ExtW),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call,ExtW),
                Instr(Mnemonic.ldx, ExtW),
                Instr(Mnemonic.stx, ExtW),

                // C0
                Instr(Mnemonic.subb, Ib),
                Instr(Mnemonic.cmpb, Ib),
                Instr(Mnemonic.sbcb, Ib),
                Instr(Mnemonic.addd, Iw),

                Instr(Mnemonic.andb, Ib),
                Instr(Mnemonic.bitb, Ib),
                Instr(Mnemonic.ldb, Ib),
                Instr(Mnemonic.stb, Ib),

                Instr(Mnemonic.eorb, Ib),
                Instr(Mnemonic.adcb, Ib),
                Instr(Mnemonic.orb, Ib),
                Instr(Mnemonic.addb, Ib),

                Instr(Mnemonic.ldd, Iw),
                invalid,
                Instr(Mnemonic.ldu, Iw),
                invalid,

                // D0
                Instr(Mnemonic.subb, Dir),
                Instr(Mnemonic.cmpb, Dir),
                Instr(Mnemonic.sbcb, Dir),
                Instr(Mnemonic.addd, Dirw),

                Instr(Mnemonic.andb, Dir),
                Instr(Mnemonic.bitb, Dir),
                Instr(Mnemonic.ldb, Dir),
                Instr(Mnemonic.stb, Dir),

                Instr(Mnemonic.eorb, Dir),
                Instr(Mnemonic.adcb, Dir),
                Instr(Mnemonic.orb, Dir),
                Instr(Mnemonic.addb, Dir),

                Instr(Mnemonic.ldd, Dirw),
                Instr(Mnemonic.std, Dirw),
                Instr(Mnemonic.ldu, Dirw),
                Instr(Mnemonic.stu, Dirw),

                // E0
                Instr(Mnemonic.subb, Idx),
                Instr(Mnemonic.cmpb, Idx),
                Instr(Mnemonic.sbcb, Idx),
                Instr(Mnemonic.addd, IdxW),

                Instr(Mnemonic.andb, Idx),
                Instr(Mnemonic.bitb, Idx),
                Instr(Mnemonic.ldb, Idx),
                Instr(Mnemonic.stb, Idx),

                Instr(Mnemonic.eorb, Idx),
                Instr(Mnemonic.adcb, Idx),
                Instr(Mnemonic.orb, Idx),
                Instr(Mnemonic.addb, Idx),

                Instr(Mnemonic.ldd, IdxW),
                Instr(Mnemonic.std, IdxW),
                Instr(Mnemonic.ldu, IdxW),
                Instr(Mnemonic.stu, IdxW),

                // F0
                Instr(Mnemonic.subb, Ext),
                Instr(Mnemonic.cmpb, Ext),
                Instr(Mnemonic.sbcb, Ext),
                Instr(Mnemonic.addd, ExtW),

                Instr(Mnemonic.andb, Ext),
                Instr(Mnemonic.bitb, Ext),
                Instr(Mnemonic.ldb, Ext),
                Instr(Mnemonic.stb, Ext),

                Instr(Mnemonic.eorb, Ext),
                Instr(Mnemonic.adcb, Ext),
                Instr(Mnemonic.orb, Ext),
                Instr(Mnemonic.addb, Ext),

                Instr(Mnemonic.ldd, ExtW),
                Instr(Mnemonic.std, ExtW),
                Instr(Mnemonic.ldu, ExtW),
                Instr(Mnemonic.stu, ExtW),
            };
        }
    }
}