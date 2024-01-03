#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

#pragma warning disable IDE1006

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.C166
{
    // C166 Family User’s Manual 29 V2.0, 2001-03
    using Decoder = Reko.Core.Machine.Decoder<C166Disassembler, Mnemonic, C166Instruction>;

    public class C166Disassembler : DisassemblerBase<C166Instruction, Mnemonic>
    {
        private readonly C166Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public C166Disassembler(C166Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override C166Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            if (!rdr.TryReadLeUInt16(out ushort op))
                return null;
            var instr = decoders[(byte) op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override C166Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new C166Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = ops.ToArray()
            };
        }

        public override C166Instruction CreateInvalidInstruction()
        {
            return new C166Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override C166Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("C166Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators
        /// <summary>
        /// Register.
        /// </summary>
        private static Mutator<C166Disassembler> R(int bitPos, RegisterStorage [] regs)
        {
            var regField = new Bitfield(bitPos, 4);
            return (u, d) =>
            {
                var reg = regs[regField.Read(u)];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> R8b = R(8, Registers.ByteRegs);
        private static readonly Mutator<C166Disassembler> R8w = R(8, Registers.GpRegs);
        private static readonly Mutator<C166Disassembler> R12w = R(12, Registers.GpRegs);
        private static readonly Mutator<C166Disassembler> R12b = R(12, Registers.ByteRegs);

        /// <summary>
        /// Two registers encoded in the high nybbles of the opcode.
        /// </summary>
        private static Mutator<C166Disassembler> nm(RegisterStorage[] regs)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var nReg = Bits.ZeroExtend(uInstr >> 12, 4);
                var mReg = Bits.ZeroExtend(uInstr >> 8, 4);
                dasm.ops.Add(regs[nReg]);
                dasm.ops.Add(regs[mReg]);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> nm8 = nm(Registers.ByteRegs);
        private static readonly Mutator<C166Disassembler> nm16 = nm(Registers.GpRegs);

        /// <summary>
        /// Register referred to twice in the high byte of the opcode.
        /// </summary>
        private static bool nn(uint uInstr, C166Disassembler dasm)
        {
            var regs = Registers.GpRegs;
            var nReg = Bits.ZeroExtend(uInstr >> 12, 4);
            var mReg = Bits.ZeroExtend(uInstr >> 8, 4);
            if (nReg != mReg)
                return false;
            dasm.ops.Add(regs[nReg]);
            return true;
        }

        /// <summary>
        /// Register deferred encoded a nybble of the opcode.
        /// </summary>
        private static Mutator<C166Disassembler> Rd(PrimitiveType dt, int bitPos)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var nReg = Bits.ZeroExtend(uInstr >> bitPos, 4);
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.GpRegs[nReg]
                });
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> Rdnb = Rd(PrimitiveType.Byte, 12);
        private static readonly Mutator<C166Disassembler> Rdnw = Rd(PrimitiveType.Word16, 12);
        private static readonly Mutator<C166Disassembler> Rdmb = Rd(PrimitiveType.Byte, 8);
        private static readonly Mutator<C166Disassembler> Rdmw = Rd(PrimitiveType.Word16, 8);

        /// <summary>
        /// Register deferred encoded using only two bits of the opcode.
        /// </summary>
        private static Mutator<C166Disassembler> Rd_twobits(PrimitiveType dt, bool postInc)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var nReg = Bits.ZeroExtend(uInstr >> 8, 2);
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.GpRegs[nReg],
                    Postincrement = postInc,
                });
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> Rdi_b = Rd_twobits(PrimitiveType.Byte, false);
        private static readonly Mutator<C166Disassembler> Rdi_w = Rd_twobits(PrimitiveType.Word16, false);
        private static readonly Mutator<C166Disassembler> Rdi_post_b = Rd_twobits(PrimitiveType.Byte, false);
        private static readonly Mutator<C166Disassembler> Rdi_post_w = Rd_twobits(PrimitiveType.Word16, false);

        /// <summary>
        /// Register encoded in the low nybble of the high byte of the opcode. The high
        /// nybble must have the value <paramref name="highNybble"/>.
        /// </summary>
        private static Mutator<C166Disassembler> Rlo(RegisterStorage[] regs, uint highNybble)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var n = (uInstr >> 12);
                if (n != highNybble)
                    return false;
                var nReg = Bits.ZeroExtend(uInstr >> 8, 4);
                dasm.ops.Add(regs[nReg]);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> R0nb = Rlo(Registers.ByteRegs, 0x0);
        private static readonly Mutator<C166Disassembler> R0nw = Rlo(Registers.GpRegs, 0x0);
        private static readonly Mutator<C166Disassembler> RFnw = Rlo(Registers.GpRegs, 0xF);

        /// <summary>
        /// Register encoded in the high nybble of the high byte of the opcode. The high
        /// nybble must have the value <paramref name="highNybble"/>.
        /// </summary>
        private static Mutator<C166Disassembler> Rhi(RegisterStorage[] regs, uint highNybble)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var n = (uInstr >> 8) & 0xF;
                if (n != highNybble)
                    return false;
                var nReg = Bits.ZeroExtend(uInstr >> 12, 4);
                dasm.ops.Add(regs[nReg]);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> Rn0w = Rhi(Registers.GpRegs, 0x0);
        private static readonly Mutator<C166Disassembler> RnFw = Rhi(Registers.GpRegs, 0xF);


        /// <summary>
        /// Register deferred encoded in the low nybble of the high byte of the opcode. The high
        /// nybble must have the value <paramref name="highNybble"/>.
        /// </summary>
        private static Mutator<C166Disassembler> Rdn(PrimitiveType dt, uint highNybble)
        {
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var n = (uInstr >> 12);
                if (n != highNybble)
                    return false;
                var nReg = Bits.ZeroExtend(uInstr >> 8, 4);
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.GpRegs[nReg]
                });
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> Rdn0w = Rdn(PrimitiveType.Word16, 0x0);

        private static Mutator<C166Disassembler> PreDec(PrimitiveType dt, int bitPos)
        {
            var regField = new Bitfield(bitPos, 4);
            return (u, d) =>
            {
                var reg = Registers.GpRegs[regField.Read(u)];
                var mem = new MemoryOperand(dt)
                {
                    Base = reg,
                    Predecrement = true,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> PreDec8b = PreDec(PrimitiveType.Byte, 8);
        private static readonly Mutator<C166Disassembler> PreDec8w = PreDec(PrimitiveType.Word16, 8);
        private static readonly Mutator<C166Disassembler> PreDec12w = PreDec(PrimitiveType.Word16, 12);


        private static Mutator<C166Disassembler> PostInc(PrimitiveType dt, int bitPos)
        {
            var regField = new Bitfield(bitPos, 4);
            return (u, d) =>
            {
                var reg = Registers.GpRegs[regField.Read(u)];
                var mem = new MemoryOperand(dt)
                {
                    Base = reg,
                    Postincrement = true,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> PostInc8b = PostInc(PrimitiveType.Byte, 8);
        private static readonly Mutator<C166Disassembler> PostInc8w = PostInc(PrimitiveType.Word16, 8);
        private static readonly Mutator<C166Disassembler> PostInc12b = PostInc(PrimitiveType.Byte, 12);
        private static readonly Mutator<C166Disassembler> PostInc12w = PostInc(PrimitiveType.Word16, 12);


        /// <summary>
        /// Register encoded in the high byte of the opcode.
        /// </summary>
        private static Mutator<C166Disassembler> reg(PrimitiveType dt, RegisterStorage[] regs)
        {
            return (u, d) =>
            {
                var reg = (int) Bits.ZeroExtend(u >> 8, 8);
                var op = Decode8bitReg(dt, regs, reg);
                d.ops.Add(op);
                return true;
            };
        }

        private static MachineOperand Decode8bitReg(PrimitiveType dt, RegisterStorage[] regs, int reg)
        {
            if (reg < 0xF0)
            {
                var uAddr = 0xFE00 + 2 * reg;
                if (Registers.SpecialFunctionRegs.TryGetValue(uAddr, out var sreg))
                    return sreg;
                return new MemoryOperand(dt)
                {
                    Offset = uAddr,
                };
            }
            else
            {
                return regs[reg & 0xF];
            }
        }

        private static readonly Mutator<C166Disassembler> reg8 = reg(PrimitiveType.Byte, Registers.ByteRegs);
        private static readonly Mutator<C166Disassembler> reg16 = reg(PrimitiveType.Word16, Registers.GpRegs);

        private static Mutator<C166Disassembler> q_QQ()
        {
            var qqField = new Bitfield(8, 8);
            var qField = new Bitfield(4, 4);
            return (u, d) =>
            {
                var qq = Decode8bitReg(PrimitiveType.Word16, Registers.GpRegs, (int) qqField.Read(u));
                var q = (int)qField.Read(u);
                d.ops.Add(new BitOfOperand(q, qq));
                return true;
            };
        }

        private static Mutator<C166Disassembler> QQZZqzFn()
        {
            var qqField = new Bitfield(8, 8);
            var zzField = new Bitfield(16, 8);
            var qField = new Bitfield(28, 4);
            var zField = new Bitfield(24, 4);
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeUInt16(out ushort hiword))
                    return false;
                u |= ((uint) hiword) << 16;
                var qq = Decode8bitReg(PrimitiveType.Word16, Registers.GpRegs, (int) qqField.Read(u));
                var zz = Decode8bitReg(PrimitiveType.Word16, Registers.GpRegs, (int) zzField.Read(u));
                var q = (int)qField.Read(u);
                var z = (int) zField.Read(u);
                d.ops.Add(new BitOfOperand(z, zz));
                d.ops.Add(new BitOfOperand(q, qq));
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> QQZZqz = QQZZqzFn();

        private static Mutator<C166Disassembler> QQrrqFn()
        {
            var qqField = new Bitfield(8, 8);
            var qField = new Bitfield(28, 4);
            var rrField = new Bitfield(16, 8);
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeUInt16(out ushort hiword))
                    return false;
                u |= ((uint) hiword) << 16;
                var qq = Decode8bitReg(PrimitiveType.Word16, Registers.GpRegs, (int) qqField.Read(u));
                var q = (int) qField.Read(u);
                d.ops.Add(new BitOfOperand(q, qq));
                var offset = rrField.ReadSigned(u) << 1;
                var addr = d.rdr.Address + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> QQrrq = QQrrqFn();


        private static Mutator<C166Disassembler> QQaaddFn()
        {
            var qqField = new Bitfield(8, 8);
            var aaField = new Bitfield(16, 8);
            var ddField = new Bitfield(24, 8);
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeUInt16(out ushort hiword))
                    return false;
                u |= ((uint) hiword) << 16;
                var qq = Decode8bitReg(PrimitiveType.Word16, Registers.GpRegs, (int) qqField.Read(u));
                var aa = (byte) aaField.Read(u);
                var dd = (byte) ddField.Read(u);
                d.ops.Add(qq);
                d.ops.Add(ImmediateOperand.Byte(aa));
                d.ops.Add(ImmediateOperand.Byte(dd));
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> QQaadd = QQaaddFn();

        /// <summary>
        /// Conditional operand in the high nybble.
        /// </summary>
        private static bool c(uint uInstr, C166Disassembler dasm)
        {
            var c = (CondCode) ((uInstr >> 12) & 0xF);
            dasm.ops.Add(new ConditionalOperand(c));
            return true;
        }


        /// <summary>
        /// Conditional operand in the next-to-lowest nybble.
        /// </summary>
        private static bool c4(uint uInstr, C166Disassembler dasm)
        {
            var c = (CondCode) ((uInstr >> 4) & 0xF);
            dasm.ops.Add(new ConditionalOperand(c));
            return true;
        }

        private static Mutator<C166Disassembler> data4(PrimitiveType dt)
        {
            const int bitPos = 12;
            return (uint uInstr, C166Disassembler dasm) =>
            {
                var u = (uInstr >> bitPos) & 0xF;
                dasm.ops.Add(new ImmediateOperand(Constant.Create(dt, u)));
                return true;
            };
        }

        private static readonly Mutator<C166Disassembler> data4_8 = data4(PrimitiveType.Byte);
        private static readonly Mutator<C166Disassembler> data4_16 = data4(PrimitiveType.Word16);

        private static bool data3(uint uInstr, C166Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) ((uInstr >> 8) & 0b111)));
            return true;
        }

        private static bool data2_12(uint uInstr, C166Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) ((uInstr >> 12) & 0b11)));
            return true;
        }

        private static bool data8(uint uInstr, C166Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort data))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte((byte)data));
            return true;
        }

        private static bool data16(uint uInstr, C166Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort data))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(data));
            return true;
        }

        /// <summary>
        /// Segment descriptor encoded in high byte of instruction.
        /// </summary>
        private static bool seg(uint uInstr, C166Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (uInstr >> 8)));
            return true;
        }

        /// <summary>
        /// Direct 16-bit jump address.
        /// </summary>
        private static bool caddr(uint uInstr, C166Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort uAddr))
                return false;
            dasm.ops.Add(AddressOperand.Ptr16(uAddr));
            return true;
        }

        private static bool trapNo(uint uInstr, C166Disassembler dasm)
        {
            if ((uInstr & 0x100) != 0)
                return false;
            dasm.ops.Add(ImmediateOperand.Byte((byte) (uInstr >> 9)));
            return true;
        }

        private static bool pageNo(uint uInstr, C166Disassembler dasm)
        {
            if ((uInstr & 0x100) != 0)
                return false;
            var page = ImmediateOperand.Word16((ushort)Bits.ZeroExtend(uInstr >> 16, 10));
            dasm.ops.Add(page);
            return true;
        }

        /// <summary>
        /// Absolute memory address.
        /// </summary>
        private static Mutator<C166Disassembler> mem(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeUInt16(out ushort data))
                    return false;
                d.ops.Add(new MemoryOperand(dt)
                {
                    Offset = data
                });
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> mem8 = mem(PrimitiveType.Byte);
        private static readonly Mutator<C166Disassembler> mem16 = mem(PrimitiveType.Word16);

        /// <summary>
        /// Memory access with displacement.
        /// </summary>
        private static Mutator<C166Disassembler> Mdisp(PrimitiveType dt, int bitposBasereg)
        {
            var baseRegField = new Bitfield(bitposBasereg, 4);
            return (u, d) =>
            {
                if (!d.rdr.TryReadUInt16(out ushort offset))
                    return false;
                var iReg = baseRegField.Read(u);
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.GpRegs[iReg],
                    Offset = offset
                });
                return true;
            };
        }
        private static readonly Mutator<C166Disassembler> Md8b = Mdisp(PrimitiveType.Byte, 8);
        private static readonly Mutator<C166Disassembler> Md8w = Mdisp(PrimitiveType.Word16, 8);


        private static readonly Bitfield relField = new Bitfield(8, 8);
        private static bool rel(uint uInstr, C166Disassembler dasm)
        {
            var offset = relField.ReadSigned(uInstr) << 1;
            var addr = dasm.rdr.Address + offset;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        #endregion

        #region Predicates

        private static bool Is0(uint u) => u == 0;

        #endregion

        private class Read16Decoder : Decoder
        {
            private readonly Decoder nextDecoder;

            public Read16Decoder(Decoder nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override C166Instruction Decode(uint wInstr, C166Disassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out ushort uHigh))
                    return dasm.CreateInvalidInstruction();
                wInstr |= ((uint) uHigh) << 16;
                return nextDecoder.Decode(wInstr, dasm);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<C166Disassembler>[] mutators)
        {
            return new InstrDecoder<C166Disassembler, Mnemonic, C166Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<C166Disassembler>[] mutators)
        {
            return new InstrDecoder<C166Disassembler, Mnemonic, C166Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(Mnemonic mnemonic, InstrClass iclass) =>
            new NyiDecoder<C166Disassembler, Mnemonic, C166Instruction>($"{mnemonic} {iclass}");

        private static Decoder Nyi(Mnemonic mnemonic) => 
            new NyiDecoder<C166Disassembler, Mnemonic, C166Instruction>(mnemonic.ToString());

        private static Decoder Nyi(string message) =>
            new NyiDecoder<C166Disassembler, Mnemonic, C166Instruction>(message);

        private static Decoder Read16(Decoder nextDecoder)
        {
            return new Read16Decoder(nextDecoder);
        }

        private static readonly Decoder invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        private static readonly Decoder[] decoders = new Decoder[256] {
            // 00
            Instr(Mnemonic.add, nm16),
            Instr(Mnemonic.addb, nm8),
            Instr(Mnemonic.add, reg16, mem16),
            Instr(Mnemonic.addb, reg8, mem8),
            Instr(Mnemonic.add, mem16, reg16),
            Instr(Mnemonic.addb, mem8, reg8),
            Instr(Mnemonic.add, reg16, data16),
            Instr(Mnemonic.addb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.add, R12w, data3),
                Instr(Mnemonic.add, R12w, data3),
                Instr(Mnemonic.add, R12w, Rdi_w),
                Instr(Mnemonic.add, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.addb, R12b, data3),
                Instr(Mnemonic.addb, R12b, data3),
                Instr(Mnemonic.addb, R12b, Rdi_b),
                Instr(Mnemonic.addb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bfldl, QQaadd),
            Instr(Mnemonic.mul, nm16),
            Instr(Mnemonic.rol, nm16),
            Instr(Mnemonic.jmpr, InstrClass.Transfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 10
            Instr(Mnemonic.addc, nm16),
            Instr(Mnemonic.addcb, nm8),
            Instr(Mnemonic.addc, reg16, mem16),
            Instr(Mnemonic.addcb, reg8, mem8),
            Instr(Mnemonic.addc, mem16, reg16),
            Instr(Mnemonic.addcb, mem8, reg8),
            Instr(Mnemonic.addc, reg16, data16),
            Instr(Mnemonic.addcb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.addc, R12w, data3),
                Instr(Mnemonic.addc, R12w, data3),
                Instr(Mnemonic.addc, R12w, Rdi_w),
                Instr(Mnemonic.addc, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.addcb, R12b, data3),
                Instr(Mnemonic.addcb, R12b, data3),
                Instr(Mnemonic.addcb, R12b, Rdi_b),
                Instr(Mnemonic.addcb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bfldh, QQaadd),
            Instr(Mnemonic.mulu, nm16),

            Instr(Mnemonic.rol, R8w, data4_16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 20
            Instr(Mnemonic.sub, nm16),
            Instr(Mnemonic.subb, nm8),
            Instr(Mnemonic.sub, reg16, mem16),
            Instr(Mnemonic.subb, reg8, mem8),
            Instr(Mnemonic.sub, mem16, reg16),
            Instr(Mnemonic.subb, mem8, reg8),
            Instr(Mnemonic.sub, reg16, data16),
            Instr(Mnemonic.subb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.sub, R12w, data3),
                Instr(Mnemonic.sub, R12w, data3),
                Instr(Mnemonic.sub, R12w, Rdi_w),
                Instr(Mnemonic.sub, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.subb, R12b, data3),
                Instr(Mnemonic.subb, R12b, data3),
                Instr(Mnemonic.subb, R12b, Rdi_b),
                Instr(Mnemonic.subb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bcmp, QQZZqz),
            Instr(Mnemonic.prior, nm16),
            Instr(Mnemonic.ror, nm16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 30
            Instr(Mnemonic.subc, nm16),
            Instr(Mnemonic.subcb, nm8),
            Instr(Mnemonic.subc, reg16, mem16),
            Instr(Mnemonic.subcb, reg8, mem8),

            Instr(Mnemonic.subc, mem16, reg16),
            Instr(Mnemonic.subcb, mem8, reg8),
            Instr(Mnemonic.subc, reg16, data16),
            Instr(Mnemonic.subcb, reg8, data8),

            // 30
            Mask(10, 2,
                Instr(Mnemonic.subc, R12w, data3),
                Instr(Mnemonic.subc, R12w, data3),
                Instr(Mnemonic.subc, R12w, Rdi_w),
                Instr(Mnemonic.subc, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.subcb, R12b, data3),
                Instr(Mnemonic.subcb, R12b, data3),
                Instr(Mnemonic.subcb, R12b, Rdi_b),
                Instr(Mnemonic.subcb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bmovn, QQZZqz),
            invalid,

            Instr(Mnemonic.ror, R8w, data4_8),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 40
            Instr(Mnemonic.cmp, nm16),
            Instr(Mnemonic.cmpb, nm8),
            Instr(Mnemonic.cmp, reg16, mem16),
            Instr(Mnemonic.cmpb, reg8, reg8),

            invalid,
            invalid,
            Instr(Mnemonic.cmp, reg16, data16),
            Instr(Mnemonic.cmpb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.cmp, R12w, data3),
                Instr(Mnemonic.cmp, R12w, data3),
                Instr(Mnemonic.cmp, R12w, Rdi_w),
                Instr(Mnemonic.cmp, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.cmpb, R12b, data3),
                Instr(Mnemonic.cmpb, R12b, data3),
                Instr(Mnemonic.cmpb, R12b, Rdi_b),
                Instr(Mnemonic.cmpb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bmov, QQZZqz),
            Instr(Mnemonic.div, nn),

            Instr(Mnemonic.shl, nm16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 50
            Instr(Mnemonic.xor, nm16),
            Instr(Mnemonic.xorb, nm8),
            Instr(Mnemonic.xor, reg16, mem16),
            Instr(Mnemonic.xorb, reg8, mem8),
            Instr(Mnemonic.xor, mem16, reg16),
            Instr(Mnemonic.xorb, mem8, reg8),
            Instr(Mnemonic.xor, reg16, data16),
            Instr(Mnemonic.xorb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.xor, R12w, data3),
                Instr(Mnemonic.xor, R12w, data3),
                Instr(Mnemonic.xor, R12w, Rdi_w),
                Instr(Mnemonic.xor, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.xorb, R12b, data3),
                Instr(Mnemonic.xorb, R12b, data3),
                Instr(Mnemonic.xorb, R12b, Rdi_b),
                Instr(Mnemonic.xorb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bor, QQZZqz),
            Instr(Mnemonic.divu, nn),

            Instr(Mnemonic.shl, R8w, data4_8),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 60
            Instr(Mnemonic.and, nm16),
            Instr(Mnemonic.andb, nm8),
            Instr(Mnemonic.and, reg16, mem16),
            Instr(Mnemonic.andb, reg8, mem8),
            Instr(Mnemonic.and, mem16, reg16),
            Instr(Mnemonic.andb, mem8, reg8),
            Instr(Mnemonic.and, reg16, data16),
            Instr(Mnemonic.andb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.and, R12w, data3),
                Instr(Mnemonic.and, R12w, data3),
                Instr(Mnemonic.and, R12w, Rdi_w),
                Instr(Mnemonic.and, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.andb, R12b, data3),
                Instr(Mnemonic.andb, R12b, data3),
                Instr(Mnemonic.andb, R12b, Rdi_b),
                Instr(Mnemonic.andb, R12b, Rdi_post_b)),

            Instr(Mnemonic.band, QQZZqz),
            Instr(Mnemonic.divl, nn),

            Instr(Mnemonic.shr, nm16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 70
            Instr(Mnemonic.or, nm16),
            Instr(Mnemonic.orb, nm8),
            Instr(Mnemonic.or, reg16, mem16),
            Instr(Mnemonic.orb, reg8, mem8),
            Instr(Mnemonic.or, mem16,reg16),
            Instr(Mnemonic.orb, mem8,reg8),
            Instr(Mnemonic.or, reg16, data16),
            Instr(Mnemonic.orb, reg8, data8),

            Mask(10, 2,
                Instr(Mnemonic.or, R12w, data3),
                Instr(Mnemonic.or, R12w, data3),
                Instr(Mnemonic.or, R12w, Rdi_w),
                Instr(Mnemonic.or, R12w, Rdi_post_w)),
            Mask(10, 2,
                Instr(Mnemonic.orb, R12b, data3),
                Instr(Mnemonic.orb, R12b, data3),
                Instr(Mnemonic.orb, R12b, Rdi_b),
                Instr(Mnemonic.orb, R12b, Rdi_post_b)),
            Instr(Mnemonic.bxor, QQZZqz),
            Instr(Mnemonic.divlu, nn),

            Instr(Mnemonic.shr, R8w, data4_8),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 80
            Instr(Mnemonic.cmpi1, R8w, data4_16),
            Select((8, 4), Is0, "  81",
                Instr(Mnemonic.neg, R12w),
                invalid),
            Instr(Mnemonic.cmpi1, RFnw, mem16),
            invalid,
            Instr(Mnemonic.mov, Rdn0w, mem16),
            invalid,
            Instr(Mnemonic.cmpi1, RFnw, data16),
            Read16(
                Select(u => u == 0x87877887,
                    Instr(Mnemonic.idle, InstrClass.Linear|InstrClass.Privileged),
                    invalid)),
            Instr(Mnemonic.mov, PreDec8w,R12w),
            Instr(Mnemonic.movb, PreDec8b,R12b),
            Instr(Mnemonic.jb, InstrClass.ConditionalTransfer, QQrrq),
            invalid,
            
            invalid,
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 90
            Instr(Mnemonic.cmpi2, R8w, data4_16),
            Select((8, 4), Is0,
                Instr(Mnemonic.cpl, R12w),
                invalid),
            Instr(Mnemonic.cmpi2, RFnw, mem16),
            invalid,

            Instr(Mnemonic.mov, mem16, Rdn0w),
            invalid,
            Instr(Mnemonic.cmpi2, RFnw, data16),
            Read16(
                Select(u => u == 0x97976897,
                    Instr(Mnemonic.pwrdn, InstrClass.Linear|InstrClass.Privileged),
                    invalid)),
            
            Instr(Mnemonic.mov, R12w, PostInc8w),
            Instr(Mnemonic.movb, R12b, PostInc8b),
            Instr(Mnemonic.jnb, InstrClass.ConditionalTransfer, QQrrq),
            Instr(Mnemonic.trap, InstrClass.Transfer|InstrClass.Call, trapNo),
            
            Instr(Mnemonic.jmpi, InstrClass.Transfer, c, Rdmw),  // 2 - cc, [Rw] 
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),
            
            Instr(Mnemonic.cmpd1, R8w, data4_16),
            Select((8, 4), Is0, 
                Instr(Mnemonic.negb, R12b),
                invalid),
            Instr(Mnemonic.cmpd1, RFnw, mem16),
            invalid,
            Select((12, 4), Is0, 
                Instr(Mnemonic.movb, Rdmb, mem8),
                invalid),
            Read16(
                Select(u => u == 0xA5A55AA5,
                    Instr(Mnemonic.diswdt, InstrClass.Linear|InstrClass.Privileged),
                    invalid)),
            Instr(Mnemonic.cmpd1, RFnw, data16),
            Read16(
                Select(u => u== 0xA7A758A7,
                    Instr(Mnemonic.srvwdt, InstrClass.Linear|InstrClass.Privileged),
                    invalid)),

            Instr(Mnemonic.mov, R12w, Rdmw),
            Instr(Mnemonic.movb, R12b, Rdmb),
            Instr(Mnemonic.jbc, InstrClass.ConditionalTransfer, QQrrq),
            Instr(Mnemonic.calli, InstrClass.Call|InstrClass.Transfer, c, Rdmw),

            Instr(Mnemonic.ashr, nm16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // B0
            Instr(Mnemonic.cmpd2, R8w, data4_16),
            Select((8, 4), Is0,
                Instr(Mnemonic.cplb, R12b),
                invalid),
            Instr(Mnemonic.cmpd2, RFnw, mem16),
            invalid,

            Instr(Mnemonic.movb, mem8, R0nb),
            Read16(
                Select(u => u == 0xB5B54AB5,
                    Instr(Mnemonic.einit, InstrClass.Linear|InstrClass.Privileged),
                    invalid)),
            Instr(Mnemonic.cmpd2, RFnw, data16),
            Read16(
                Select(u => u == 0xB7B748B7,
                    Instr(Mnemonic.srst, InstrClass.Terminates|InstrClass.Privileged),
                    invalid)),
            Instr(Mnemonic.mov, Rdmw, R12w),
            Instr(Mnemonic.movb, Rdmb, R12b),
            Instr(Mnemonic.jnbs, InstrClass.ConditionalTransfer, QQrrq),
            Instr(Mnemonic.callr, InstrClass.Call|InstrClass.ConditionalTransfer, rel),
            Instr(Mnemonic.ashr, R8w, data4_16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // C0
            Instr(Mnemonic.movbz, R8w,R12b),
            invalid,
            Instr(Mnemonic.movbz, reg16, mem8),
            invalid,

            Instr(Mnemonic.mov, Md8w, R12w),
            Instr(Mnemonic.movbz, mem16, reg8),
            Instr(Mnemonic.scxt, reg16, data16),
            invalid,

            Instr(Mnemonic.mov, Rdnw, Rdmw),
            Instr(Mnemonic.movb, Rdnb, Rdmb),
            Select((8,4), Is0,
                Instr(Mnemonic.calla, InstrClass.Call|InstrClass.ConditionalTransfer, c, caddr),
                invalid),
            Select((8,8), Is0,
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),
                invalid),
            
            Select((8,8), Is0,
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                invalid),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // D0
            Instr(Mnemonic.movbs, R8w,R12b),
            Mask(14, 2,
                Instr(Mnemonic.atomic2, data2_12),
                Nyi("d1 0b01"),
                Instr(Mnemonic.extr, data2_12),
                Nyi("d1 0b11")),
            Instr(Mnemonic.movbs, reg16, mem8),
            invalid,

            Instr(Mnemonic.mov, R12w, Md8w),
            Instr(Mnemonic.movbs, mem16, reg8),
            Instr(Mnemonic.scxt, reg16, mem16),  // 4 - reg, mem 
            Read16(
                Select(u => (u & 0x4F0000FC) == 0x4F000000,
                    Instr(Mnemonic.extp, R8w, pageNo),
                    invalid)),

            Instr(Mnemonic.mov, PostInc12w, Rdmw),
            Instr(Mnemonic.movb, PostInc12b, Rdmb),
            Instr(Mnemonic.calls, InstrClass.Call|InstrClass.Transfer, seg, caddr),
            Select((8,8), Is0,
                Instr(Mnemonic.rets, InstrClass.Transfer| InstrClass.Return),
                invalid),

            Select((14,2), u => u == 0b01,
                Instr(Mnemonic.extp, R8w, data2_12),
                invalid),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),  // 2 - cc_SGE, rel 
            Instr(Mnemonic.bclr, q_QQ()),  // 2 - bitoff.13 
            Instr(Mnemonic.bset, q_QQ()),  // 2 - bitoff.13 

            // E0
            Instr(Mnemonic.mov, R8w, data4_16),
            Instr(Mnemonic.movb, R8b, data4_8),
            Instr(Mnemonic.pcall, reg16, caddr),
            invalid,

            Instr(Mnemonic.movb, Md8b,R12b),
            invalid,
            Instr(Mnemonic.mov, reg16, data16),
            Instr(Mnemonic.movb, reg8, data8),

            Instr(Mnemonic.mov, Rdnw,PostInc8w),
            Instr(Mnemonic.movb, Rdnb,PostInc8b),
            Select((8,4), Is0,
                Instr(Mnemonic.jmpa, InstrClass.ConditionalTransfer, c, caddr),
                invalid),
            Instr(Mnemonic.retp, InstrClass.Transfer| InstrClass.Return, reg16),
            
            Instr(Mnemonic.push, reg16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),
            Instr(Mnemonic.bclr, q_QQ()),
            Instr(Mnemonic.bset, q_QQ()),

            // 0xF0
            Instr(Mnemonic.mov, nm16),
            Instr(Mnemonic.movb, nm8),
            Instr(Mnemonic.mov, reg16, mem16),
            Instr(Mnemonic.movb, reg8, mem8),

            Instr(Mnemonic.movb, R12b, Md8b),
            invalid,
            Instr(Mnemonic.mov, mem16, reg16),
            Instr(Mnemonic.movb, mem8, reg8),

            invalid,
            invalid,
            Instr(Mnemonic.jmps, InstrClass.Transfer, seg, caddr),
            Select((8, 8), u => u == 0x88u, "  FB",
                Instr(Mnemonic.reti, InstrClass.Transfer| InstrClass.Return),
                invalid),  // 2 - – 

            Instr(Mnemonic.pop, reg16),
            Instr(Mnemonic.jmpr, InstrClass.ConditionalTransfer, c4, rel),  // 2 - cc_ULE, rel 
            Instr(Mnemonic.bclr, q_QQ()),  // 2 - bitoff.15 
            Instr(Mnemonic.bset, q_QQ())  // 2 - bitoff.15 
        };
    }
}
