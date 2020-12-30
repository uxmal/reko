#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.zSeries
{
    using Decoder = WideDecoder<zSeriesDisassembler, Mnemonic, zSeriesInstruction>;
    using WideInstrDecoder = WideInstrDecoder<zSeriesDisassembler, Mnemonic, zSeriesInstruction>;

#pragma warning disable IDE1006 // Naming Styles
    public class zSeriesDisassembler : DisassemblerBase<zSeriesInstruction, Mnemonic>
    {
        private readonly static Decoder[] decoders;
        private readonly static Decoder invalid;

        private readonly zSeriesArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public zSeriesDisassembler(zSeriesArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override zSeriesInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out var opcode))
                return null;
            this.ops.Clear();
            var instr = decoders[opcode >> 8].Decode(opcode, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            instr.InstructionClass |= opcode == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        public override zSeriesInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new zSeriesInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray(),
            };
            return instr;
        }

        public override zSeriesInstruction CreateInvalidInstruction()
        {
            return new zSeriesInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        private MachineOperand CreateAccess(
            RegisterStorage baseReg,
            RegisterStorage idxReg,
            int offset)
        {
            if ((baseReg == null || baseReg.Number == 0) && 
                (idxReg == null || idxReg.Number == 0))
            {
                return AddressOperand.Ptr32((uint)offset);
            }
            return new MemoryOperand(PrimitiveType.Word32)
            {
                Base = baseReg,
                Index = idxReg,
                Offset = offset
            };
        }

        private MachineOperand CreateAccess(
            RegisterStorage baseReg,
            int offset)
        {
            if (baseReg == null || baseReg.Number == 0)
            {
                return AddressOperand.Ptr32((uint)offset);
            }
            return new MemoryOperand(PrimitiveType.Word32)
            {
                Base = baseReg,
                Offset = offset
            };
        }

        private MachineOperand CreateAccessLength(
            RegisterStorage baseReg,
            int offset,
            int length)
        {
            return new MemoryOperand(PrimitiveType.Word32)
            {
                Base = baseReg,
                Offset = offset,
                Length = length
            };
        }

        public override zSeriesInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("zSerDasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators

        public static bool FF(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.FpRegisters[(uInstr >> 4) & 0xF]));
            dasm.ops.Add(new RegisterOperand(Registers.FpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool FXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = new RegisterOperand(Registers.FpRegisters[(uInstr >> 20) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(f1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool I(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i = ImmediateOperand.Byte((byte) uInstr);
            dasm.ops.Add(i);
            return true;
        }

        public static bool MII(ulong uInstr, zSeriesDisassembler dasm)
        {
            var m1 = (byte)((uInstr >> 36) & 0xF);
            var r2 = (int) Bits.SignExtend((uint)(uInstr >> 24), 12);
            var r3 = (int) Bits.SignExtend((uint)uInstr, 24);
            dasm.ops.Add(ImmediateOperand.Byte(m1));
            dasm.ops.Add(ImmediateOperand.Int32(r2));
            dasm.ops.Add(ImmediateOperand.Int32(r3));
            return true;
        }

        public static bool R(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[uInstr & 0xF]));
            return true;
        }

        public static bool RR(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 4) & 0xF]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool RRE(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 4) & 0xF]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool RIa(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]));
            dasm.ops.Add(ImmediateOperand.Int32((int)Bits.SignExtend(uInstr, 16)));
            return true;
        }

        public static bool RIb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2 * (int)Bits.SignExtend(uInstr, 16);
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]));
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        public static bool RIc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2*(int)Bits.SignExtend(uInstr, 16);
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        private static bool RIEb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var r2 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 32) & 0xF]);
            var m3 = (byte) ((uInstr >> 12) & 0xF);
            var addr = dasm.addr + 2 * (short) Bits.SignExtend(uInstr >> 16, 16);
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            dasm.ops.Add(ImmediateOperand.Byte(m3));
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        private static bool RIEc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var i2 = Constant.SByte((sbyte)Bits.SignExtend(uInstr >> 8, 8));
            var m3 = (byte) ((uInstr >> 32) & 0xF);
            var addr = dasm.addr + 2 * (short) Bits.SignExtend(uInstr >> 16, 16);
            dasm.ops.Add(r1);
            dasm.ops.Add(new ImmediateOperand(i2));
            dasm.ops.Add(ImmediateOperand.Byte(m3));
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        private static bool RIEd(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var i2 = ImmediateOperand.Int16((short)(uInstr >> 16));
            var r3 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 32) & 0xF]);
            dasm.ops.Add(r1);
            dasm.ops.Add(i2);
            dasm.ops.Add(r3);
            return true;
        }

        private static bool RIEe(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var ri2 = AddressOperand.Create(dasm.addr + 2 * (int) Bits.SignExtend(uInstr, 32));
            var r3 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 32) & 0xF]);
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(ri2);
            return true;
        }

        private static bool RIEf(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var r2 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 32) & 0xF]);
            var i3 = ImmediateOperand.Byte((byte) (uInstr >> 24));
            var i4 = ImmediateOperand.Byte((byte) (uInstr >> 16));
            var i5 = ImmediateOperand.Byte((byte) (uInstr >> 8));
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            dasm.ops.Add(i3);
            dasm.ops.Add(i4);
            dasm.ops.Add(i5);
            return true;
        }

        public static bool RILa(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]));
            var i2 = ImmediateOperand.Word32((uint) Bits.SignExtend(uInstr, 32));
            dasm.ops.Add(i2);
            return true;
        }

        public static bool RILb(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]));
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.ops.Add(AddressOperand.Create(dasm.addr + offset));
            return true;
        }

        public static bool RILc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.ops.Add(AddressOperand.Create(dasm.addr + offset));
            return true;
        }

        public static bool RSI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]);
            var offset = 2 * (short) uInstr;
            var ri2 = AddressOperand.Create(dasm.addr + offset);
            var r3 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 16) & 0xF]);
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(ri2);
            return true;
        }

        public static bool RXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool RXb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool RXE(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = Registers.FpRegisters[(uInstr >> 36) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr >> 16, 12);
            dasm.ops.Add(new RegisterOperand(f1));
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool RXF(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = Registers.FpRegisters[(uInstr >> 12) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr >> 16, 12);
            var f3 = Registers.FpRegisters[(uInstr >> 36) & 0xF];
            dasm.ops.Add(new RegisterOperand(f1));
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            dasm.ops.Add(new RegisterOperand(f3));
            return true;
        }

        public static bool S(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }
        
        public static bool SMI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var m1 = ImmediateOperand.Byte((byte) (uInstr >> 36 & 0xF));
            var addr = dasm.addr + 2 * (int) Bits.SignExtend(uInstr, 16);
            var b3 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d3 = (int) Bitfield.ReadSignedFields(siyDisplacement, uInstr);
            dasm.ops.Add(m1);
            dasm.ops.Add(AddressOperand.Create(addr));
            dasm.ops.Add(new MemoryOperand(dasm.arch.WordWidth)
            {
                Base = b3,
                Offset = d3,
            });

            return true;
        }

        public static bool SSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l = (byte)(uInstr >> 32);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccessLength(b1, d1, l + 1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool SSb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l1 = (byte)((uInstr >> 36) & 0xF);
            var l2 = (byte)((uInstr >> 32) & 0xF);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccessLength(b1, d1, l1+ 1));
            dasm.ops.Add(dasm.CreateAccessLength(b2, d2, l2+1));
            return true;
        }

        public static bool SSc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l = (byte)((uInstr >> 36) & 0xF);
            var i3 = (byte)((uInstr >> 32) & 0xF);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccessLength(b1, d1, l + 1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.ops.Add(ImmediateOperand.Byte(i3));
            return true;
        }

        public static bool SSd(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b1, r1, d1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.ops.Add(new RegisterOperand(r3));
            return true;
        }

        public static bool SSf(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l2 = (byte)(uInstr >> 32);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.ops.Add(dasm.CreateAccessLength(b2, d2, l2 + 1));
            return true;
        }

        public static bool RSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(new RegisterOperand(r1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSa3(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(new RegisterOperand(r1));
            dasm.ops.Add(new RegisterOperand(r3));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var m3 = ImmediateOperand.Byte((byte)((uInstr >> 16) & 0xF));
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(new RegisterOperand(r1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.ops.Add(m3);
            return true;
        }

        private static readonly Bitfield[] rsya_offset = new[]
        {
            new Bitfield(8, 8),
            new Bitfield(16, 12),
        };

        private static bool RSYa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rsya_offset, (uint)uInstr);
            dasm.ops.Add(new RegisterOperand(r1));
            dasm.ops.Add(new RegisterOperand(r3));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static bool SIL(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (short) uInstr >> 16;
            var i2 = ImmediateOperand.Word16((ushort) uInstr);
            dasm.ops.Add(new MemoryOperand(dasm.arch.WordWidth)
            {
                Base = b1,
                Offset = d1,
            });
            dasm.ops.Add(i2);
            return true;
        }

        private static readonly Bitfield[] siyDisplacement = Bf((8, 8), (16, 12));
        private static bool SIY(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int) Bitfield.ReadSignedFields(siyDisplacement, uInstr);
            var i2 = ImmediateOperand.Byte((byte) (uInstr >> 32));
            dasm.ops.Add(new MemoryOperand(dasm.arch.WordWidth)
            {
                Base = b1,
                Offset = d1,
            });
            dasm.ops.Add(i2);
            return true;
        }

        private static bool RSYb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var m3 = (byte)((uInstr >> 32) & 0xF);
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rsya_offset, (uint)uInstr);
            dasm.ops.Add(new RegisterOperand(r1));
            dasm.ops.Add(ImmediateOperand.Byte(m3));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static readonly Bitfield[] rxya_offset = new[]
        {
            new Bitfield(8, 8),
            new Bitfield(16, 12),
        };

        private static bool RXYa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rxya_offset, (uint)uInstr);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool FXYa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rxya_offset, (uint) uInstr);
            dasm.ops.Add(f1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool SI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i2 = ImmediateOperand.Byte((byte)(uInstr >> 16));
            var b1 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.ops.Add(i2);
            return true;
        }

        #endregion

        public class NyiDecoder : Decoder
        {
            private readonly string msg;

            public NyiDecoder(string msg = "")
            {
                this.msg = msg;
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                return dasm.NotYetImplemented(msg);
            }
        }

        public class ExtendDecoder32 : WideInstrDecoder
        {
            public ExtendDecoder32(InstrClass iclass, Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators)
                : base(iclass, mnemonic, mutators)
            {
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uLowWord))
                    return dasm.CreateInvalidInstruction();
                ulong uInstrExt = (uInstr << 16) | uLowWord;
                return base.Decode(uInstrExt, dasm);
            }
        }

        public class ExtendDecoder48 : WideInstrDecoder
        {
            public ExtendDecoder48(InstrClass iclass, Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators) : base(iclass, mnemonic, mutators)
            {
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt32(out uint uLowWord))
                    return dasm.CreateInvalidInstruction();
                ulong uInstrExt = (uInstr << 32) | uLowWord;
                return base.Decode(uInstrExt, dasm);
            }
        }

        public class MaskDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public MaskDecoder(int pos, int len, params Decoder[] decoders)
            {
                this.bitfield = new Bitfield(pos, len);
                this.decoders = decoders;
            }

            public MaskDecoder(int pos, int len, params (int, Decoder)[] decoders)
            {
                this.bitfield = new Bitfield(pos, len);
                this.decoders = new Decoder[1 << len];
                foreach (var d in decoders)
                {
                    Debug.Assert(this.decoders[d.Item1] == null);
                    this.decoders[d.Item1] = d.Item2;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] == null)
                        this.decoders[i] = invalid;
                }
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                var op = this.bitfield.Read(uInstr);
                return this.decoders[op].Decode(uInstr, dasm);
            }
        }

        public class ExtendMaskDecoder32: Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public ExtendMaskDecoder32(int pos, int len, params Decoder[] decoders)
            {
                this.bitfield = new Bitfield(pos, len);
                this.decoders = decoders;
            }

            public ExtendMaskDecoder32(int pos, int len, params (uint, Decoder)[] decoders)
            {
                this.bitfield = new Bitfield(pos, len);
                this.decoders = new Decoder[1 << len];
                foreach (var d in decoders)
                {
                    Debug.Assert(this.decoders[d.Item1] == null);
                    this.decoders[d.Item1] = d.Item2;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] == null)
                        this.decoders[i] = invalid;
                }
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uLowWord))
                    return dasm.CreateInvalidInstruction();
                ulong uInstrExt = (uInstr << 16) | uLowWord;
                var op = this.bitfield.Read((uint)uInstrExt);
                return this.decoders[op].Decode(uInstrExt, dasm);
            }
        }

        public class ExtendMaskDecoder48 : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public ExtendMaskDecoder48(int pos, int len, params (uint,Decoder)[] decoders)
            {
                this.bitfield = new Bitfield(pos, len);
                this.decoders = new Decoder[1 << len];
                Decoder defaultDecoder = invalid;
                foreach (var d in decoders)
                {
                    if (d.Item1 == ~0u)
                    {
                        defaultDecoder = d.Item2;
                    }
                    else
                    {
                        Debug.Assert(this.decoders[d.Item1] == null);
                        this.decoders[d.Item1] = d.Item2;
                    }
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] == null)
                        this.decoders[i] = defaultDecoder;
                }
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt32(out uint uLowWord))
                    return dasm.CreateInvalidInstruction();
                ulong uInstrExt = (uInstr << 32) | uLowWord;
                var op = this.bitfield.Read(uInstrExt);
                return this.decoders[op].Decode(uInstrExt, dasm);
            }
        }

        public static WideInstrDecoder Instr(Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new WideInstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        public static WideInstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new WideInstrDecoder(iclass, mnemonic, mutators);
        }

        public static ExtendDecoder32 Instr32(Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new ExtendDecoder32(InstrClass.Linear, mnemonic, mutators);
        }

        public static ExtendDecoder32 Instr32(Mnemonic mnemonic, InstrClass iclass, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new ExtendDecoder32(iclass, mnemonic, mutators);
        }


        public static ExtendDecoder48 Instr48(Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new ExtendDecoder48(InstrClass.Linear, mnemonic, mutators);
        }

        public static ExtendDecoder48 Instr48(Mnemonic mnemonic, InstrClass iclass, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new ExtendDecoder48(iclass, mnemonic, mutators);
        }

        public static MaskDecoder Mask(int pos, int len, params Decoder[] decoders)
        {
            return new MaskDecoder(pos, len, decoders);
        }

        public static MaskDecoder Mask(int pos, int len, params (int, Decoder) [] decoders)
        {
            return new MaskDecoder(pos, len, decoders);
        }

        public static ExtendMaskDecoder32 ExtendMask32(int pos, int len, params Decoder[] decoders)
        {
            return new ExtendMaskDecoder32(pos, len, decoders);
        }

        public static ExtendMaskDecoder32 ExtendMask32(int pos, int len, params (uint, Decoder)[] decoders)
        {
            return new ExtendMaskDecoder32(pos, len, decoders);
        }

        public static ExtendMaskDecoder48 ExtendMask48(int pos, int len, params (uint, Decoder)[] decoders)
        {
            return new ExtendMaskDecoder48(pos, len, decoders);
        }

        public static NyiDecoder Nyi(string msg)
        {
            return new NyiDecoder(msg);
        }

        static zSeriesDisassembler()
        {
            invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            var n01_decoders = Mask(0, 8,
                (0x01, Instr(Mnemonic.pr)));

            var b2_decoders = ExtendMask32(0, 8,
                (0x04, Instr(Mnemonic.sck, S))
                );

            var b3_decoders = ExtendMask32(16, 8,
                (0xC1, Instr(Mnemonic.ldgr, RRE)),
                (0xCD, Instr(Mnemonic.lgdr, RRE)));

            var b9_decoders = ExtendMask32(16, 8,
                (0x00, Instr(Mnemonic.lpgr, RRE)),
                (0x02, Instr(Mnemonic.ltgr, RRE)),
                (0x04, Instr(Mnemonic.lgr, RRE)),
                (0x08, Instr(Mnemonic.agr, RRE)),
                (0x09, Instr(Mnemonic.sgr, RRE)),
                (0x12, Instr(Mnemonic.ltgfr, RRE)),
                (0x14, Instr(Mnemonic.lgfr, RRE)),
                (0x20, Instr(Mnemonic.cgr, RRE)),
                (0x21, Instr(Mnemonic.clgr, RRE)),
                (0x80, Instr(Mnemonic.ngr, RRE)),
                (0xE2, Mask(12, 4,
                    Instr(Mnemonic.locgrnv, RR),
                    Instr(Mnemonic.locgro, RR),
                    Instr(Mnemonic.locgrh, RR),
                    Instr(Mnemonic.locgrnle, RR),
                    Instr(Mnemonic.locgrl, RR),
                    Instr(Mnemonic.locgrnhe, RR),
                    Instr(Mnemonic.locgrlh, RR),
                    Instr(Mnemonic.locgrne, RR),
                    Instr(Mnemonic.locgre, RR),
                    Instr(Mnemonic.locgrnlh, RR),
                    Instr(Mnemonic.locgrhe, RR),
                    Instr(Mnemonic.locgrnl, RR),
                    Instr(Mnemonic.locgrle, RR),
                    Instr(Mnemonic.locgrnh, RR),
                    Instr(Mnemonic.locgrno, RR),
                    Instr(Mnemonic.locgr, RR))));

            var c2_decoders = ExtendMask48(32, 4,
                (0x0, Instr(Mnemonic.msgfi, RILa)),
                (0x1, Instr(Mnemonic.msfi, RILa)),
                (0x2, invalid),
                (0x3, invalid),

                (0x4, Instr(Mnemonic.slgfi, RILa)),
                (0x5, Instr(Mnemonic.slfi, RILa)),
                (0x6, invalid),
                (0x7, invalid),

                (0x8, Instr(Mnemonic.agfi, RILa)),
                (0x9, Instr(Mnemonic.afi, RILa)),
                (0xA, Instr(Mnemonic.algfi, RILa)),
                (0xB, Instr(Mnemonic.alfi, RILa)),

                (0xC, Instr(Mnemonic.cgfi, RILa)),
                (0xD, Instr(Mnemonic.cfi, RILa)),
                (0xE, Instr(Mnemonic.clgfi, RILa)),
                (0xF, Instr(Mnemonic.clfi, RILa)));

            var c6_decoders = ExtendMask48(32, 4,
                (~0u, invalid),
                (0x0, Instr(Mnemonic.exrl, RILb)),
                (0x2, Instr(Mnemonic.pfdrl, RILb)),
                (0x4, Instr(Mnemonic.cghrl, RILb)),
                (0x5, Instr(Mnemonic.chrl, RILb)),
                (0x6, Instr(Mnemonic.clghrl, RILb)),
                (0x7, Instr(Mnemonic.clhrl, RILb)),
                (0x8, Instr(Mnemonic.cgrl, RILb)),
                (0xA, Instr(Mnemonic.clgrl, RILb)),
                (0xC, Instr(Mnemonic.cgfrl, RILb)),
                (0xD, Instr(Mnemonic.crl, RILb)),
                (0xE, Instr(Mnemonic.clgfrl, RILb)),
                (0xF, Instr(Mnemonic.clrl, RILb)));

            var e3_decoders = ExtendMask48(0, 8,
                (0x02, Instr(Mnemonic.ltg, RXYa)),
                (0x04, Instr(Mnemonic.lg, RXYa)),
                (0x12, Instr(Mnemonic.lt, RXYa)),
                (0x14, Instr(Mnemonic.lgf, RXYa)),
                (0x21, Instr(Mnemonic.clg, RXYa)),
                (0x24, Instr(Mnemonic.stg, RXYa)),
                (0x32, Instr(Mnemonic.ltgf, RXYa)),
                (0x71, Instr(Mnemonic.lay, RXYa)),
                (0x85, Instr(Mnemonic.lgat, RXYa)),
                (0x9F, Instr(Mnemonic.lat, RXYa)));

            var e5_decoders = ExtendMask48(32, 8,
                (0x4C, Instr(Mnemonic.mvhi, SIL)));

            var eb_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x04, Instr(Mnemonic.lmg, RSYa)),
                (0x0A, Instr(Mnemonic.srag, RSYa)),
                (0x0B, Instr(Mnemonic.slag, RSYa)),
                (0x0C, Instr(Mnemonic.srlg, RSYa)),
                (0x0D, Instr(Mnemonic.sllg, RSYa)),
                (0x1C, Instr(Mnemonic.rllg, RSYa)),
                (0x1D, Instr(Mnemonic.rll, RSYa)),
                (0x20, Instr(Mnemonic.clmh, RSYb)),
                (0x24, Instr(Mnemonic.stmg, RSYa)),
                (0x30, Instr(Mnemonic.csg, RSYa)),
                (0x4C, Instr(Mnemonic.ecag, RSYa)),
                (0x51, Instr(Mnemonic.tmy, SIY)),
                (0x52, Instr(Mnemonic.mviy, SIY)),
                (0x55, Instr(Mnemonic.cliy, SIY)),
                (0x6A, Instr(Mnemonic.asi, SIY)),
                (0x7A, Instr(Mnemonic.agsi, SIY)),
                (0xDC, Instr(Mnemonic.srak, RSYa)),
                (0xDE, Instr(Mnemonic.srlk, RSYa)),
                (0xDF, Instr(Mnemonic.sllk, RSYa)),
                (0xE2, Instr(Mnemonic.locg, InstrClass.Linear|InstrClass.Conditional, RSYb)),
                (0xE3, Instr(Mnemonic.stocg, InstrClass.Linear|InstrClass.Conditional, RSYb)),
                (0xE4, Instr(Mnemonic.lang, RSYa)),
                (0xE8, Instr(Mnemonic.laag, RSYa)),
                (0xF2, Instr(Mnemonic.loc, RSYb)),
                (0xF3, Instr(Mnemonic.stoc, RSYb)),
                (0xF4, Instr(Mnemonic.lan, RSYa)),
                (0xF8, Instr(Mnemonic.laa, RSYa)));

            var ec_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x44, Instr(Mnemonic.brxhg, InstrClass.ConditionalTransfer, RIEe)),
                (0x45, Instr(Mnemonic.brxlg, InstrClass.ConditionalTransfer, RIEe)),
                (0x55, Instr(Mnemonic.risbg, RIEf)),
                (0x56, Instr(Mnemonic.rosbg, RIEf)),
                (0x57, Instr(Mnemonic.rxsbg, RIEf)),
                (0x59, Instr(Mnemonic.risbgn, RIEf)),
                (0x64, Instr(Mnemonic.cgrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x65, Instr(Mnemonic.clgrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x76, Instr(Mnemonic.crj, InstrClass.ConditionalTransfer, RIEb)),
                (0x77, Instr(Mnemonic.clrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x7C, Instr(Mnemonic.cgij, InstrClass.ConditionalTransfer, RIEc)),
                (0x7D, Instr(Mnemonic.clgij, InstrClass.ConditionalTransfer, RIEc)),
                (0x7E, Instr(Mnemonic.cij,  InstrClass.ConditionalTransfer, RIEc)),
                (0x7F, Instr(Mnemonic.clij, InstrClass.ConditionalTransfer, RIEc)),
                (0xD8, Instr(Mnemonic.ahik, RIEd)),
                (0xD9, Instr(Mnemonic.aghik, RIEd)));

            var ed_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x04, Instr(Mnemonic.ldeb, RXE)),
                (0x05, Instr(Mnemonic.lxdb, RXE)),
                (0x06, Instr(Mnemonic.lxeb, RXE)),
                (0x07, Instr(Mnemonic.mxdb, RXE)),
                (0x09, Instr(Mnemonic.ceb, RXE)),
                (0x0A, Instr(Mnemonic.aeb, RXE)),
                (0x0B, Instr(Mnemonic.seb, RXE)),
                (0x0C, Instr(Mnemonic.mdeb, RXE)),
                (0x0D, Instr(Mnemonic.deb, RXE)),
                (0x0E, Instr(Mnemonic.maeb, RXF)),
                (0x0F, Instr(Mnemonic.mseb, RXF)),
                (0x10, Instr(Mnemonic.tceb, RXE)),
                (0x11, Instr(Mnemonic.tcdb, RXE)),
                (0x12, Instr(Mnemonic.tcxb, RXE)),
                (0x16, Instr(Mnemonic.meeb, RXE)),
                (0x17, Instr(Mnemonic.meeb, RXE)),
                (0x19, Instr(Mnemonic.cdb, RXE)),
                (0x1A, Instr(Mnemonic.adb, RXE)),
                (0x1B, Instr(Mnemonic.sdb, RXE)),
                (0x1C, Instr(Mnemonic.mdb, RXE)),
                (0x1D, Instr(Mnemonic.ddb, RXE)),
                (0x1E, Instr(Mnemonic.madb, RXE)),
                (0x57, Instr(Mnemonic.rxsbg, RIEf)),
                (0x65, Instr(Mnemonic.ldy, FXYa)),
                (0x67, Instr(Mnemonic.stdy, FXYa))
                );

            decoders = new Decoder[256]
            {
                // 00
                invalid,
                n01_decoders,
                invalid,
                invalid,

                Instr(Mnemonic.spm, RR),
                Instr(Mnemonic.balr, InstrClass.Transfer|InstrClass.Call, RR),
                Instr(Mnemonic.bctr, InstrClass.ConditionalTransfer, RR),
                Mask(4, 4,
                    Instr(Mnemonic.nopr, InstrClass.Linear|InstrClass.Padding, R),
                    Instr(Mnemonic.bor, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bhr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnler, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.blr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnher, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.blhr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bner, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.ber, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnlhr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bher, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnlr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bler, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnhr, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.bnor, InstrClass.ConditionalTransfer, R),
                    Instr(Mnemonic.br, InstrClass.Transfer, R)),
                invalid,
                invalid,
                Instr(Mnemonic.svc, InstrClass.Transfer|InstrClass.Call, I),
                Instr(Mnemonic.bsm, InstrClass.Transfer, RR),

                Instr(Mnemonic.bassm, InstrClass.Transfer|InstrClass.Call, RR),
                Instr(Mnemonic.basr, InstrClass.Transfer|InstrClass.Call, RR),
                Instr(Mnemonic.mvcl, RR),
                Instr(Mnemonic.clcl, RR),
                // 10
                Instr(Mnemonic.lpr, RR),
                Instr(Mnemonic.lnr, RR),
                Instr(Mnemonic.ltr, RR),
                Instr(Mnemonic.lcr, RR),

                Instr(Mnemonic.nr, RR),
                Instr(Mnemonic.clr, RR),
                Instr(Mnemonic.or, RR),
                Instr(Mnemonic.xr, RR),

                Instr(Mnemonic.lr, RR),
                Instr(Mnemonic.cr, RR),
                Instr(Mnemonic.ar, RR),
                Instr(Mnemonic.sr, RR),

                Instr(Mnemonic.mr, RR),
                Instr(Mnemonic.dr, RR),
                Instr(Mnemonic.alr, RR),
                Instr(Mnemonic.slr, RR),
                // 20
                Instr(Mnemonic.lpdr, FF),
                Instr(Mnemonic.lndr, FF),
                Instr(Mnemonic.ltdr, FF),
                Instr(Mnemonic.lcdr, FF),

                Instr(Mnemonic.hdr, FF),
                Instr(Mnemonic.ldxr, FF),
                Instr(Mnemonic.mxr, FF),
                Instr(Mnemonic.mxdr, FF),

                Instr(Mnemonic.ldr, FF),
                Instr(Mnemonic.cdr, FF),
                Instr(Mnemonic.adr, FF),
                Instr(Mnemonic.sdr, FF),

                Instr(Mnemonic.mdr, FF),
                Instr(Mnemonic.ddr, FF),
                Instr(Mnemonic.awr, FF),
                Instr(Mnemonic.swr, FF),
                // 30
                Instr(Mnemonic.lper, FF),
                Instr(Mnemonic.lner, FF),
                Instr(Mnemonic.lter, FF),
                Instr(Mnemonic.lcer, FF),

                Instr(Mnemonic.her, FF),
                Instr(Mnemonic.ledr, FF),
                Instr(Mnemonic.axr, FF),
                Instr(Mnemonic.sxr, FF),

                Instr(Mnemonic.ler, RR),
                Instr(Mnemonic.cer, FF),
                Instr(Mnemonic.aer, FF),
                Instr(Mnemonic.ser, FF),

                Instr(Mnemonic.mer, FF),
                Instr(Mnemonic.der, FF),
                Instr(Mnemonic.aur, FF),
                Instr(Mnemonic.sur, FF),

                // 40
                Instr32(Mnemonic.sth, RXa),
                Instr32(Mnemonic.la, RXa),
                Instr32(Mnemonic.stc, RXa),
                Instr32(Mnemonic.ic, RXa),

                Instr32(Mnemonic.ex, RXa),
                Nyi("*"),
                Nyi("*"),
                ExtendMask32(20, 4,
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    Instr(Mnemonic.bo, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bh, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bnle, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bl, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bnhe, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.blh, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.be, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bnlh, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bhe, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bnl, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bnh, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.bno, InstrClass.ConditionalTransfer, RXb),
                    Instr(Mnemonic.b, InstrClass.Transfer, RXb),
                    Nyi("*")),

                Instr32(Mnemonic.lh, RXa),
                Instr32(Mnemonic.ch, RXa),
                Instr32(Mnemonic.ah, RXa),
                Instr32(Mnemonic.sh, RXa),

                Instr32(Mnemonic.mh, RXa),
                Instr32(Mnemonic.bas, RXa),
                Instr32(Mnemonic.cvd, RXa),
                Instr32(Mnemonic.cvb, RXa),
                // 50
                Instr32(Mnemonic.st, RXa),
                Instr32(Mnemonic.lae, RXa),
                invalid,
                invalid,

                Instr32(Mnemonic.n, RXa),
                Instr32(Mnemonic.cl, RXa),
                Instr32(Mnemonic.o, RXa),
                Instr32(Mnemonic.x, RXa),

                Instr32(Mnemonic.l, RXa),
                Instr32(Mnemonic.c, RXa),
                Instr32(Mnemonic.a, RXa),
                Instr32(Mnemonic.s, RXa),

                Instr32(Mnemonic.m, RXa),
                Instr32(Mnemonic.d, RXa),
                Instr32(Mnemonic.al, RXa),
                Instr32(Mnemonic.sl, RXa),

                // 60
                Instr32(Mnemonic.std, FXa),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr32(Mnemonic.mxd, FXa),

                Instr32(Mnemonic.ld, FXa),
                Instr32(Mnemonic.cd, FXa),
                Instr32(Mnemonic.ad, FXa),
                Instr32(Mnemonic.sd, FXa),

                Instr32(Mnemonic.md, FXa),
                Instr32(Mnemonic.dd, FXa),
                Nyi("*"),
                Nyi("*"),
                // 70
                Instr32(Mnemonic.ste, FXa),
                Instr32(Mnemonic.ms, FXa),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr32(Mnemonic.le, FXa),
                Nyi("*"),
                Instr32(Mnemonic.ae, FXa),
                Instr32(Mnemonic.se, FXa),

                Instr32(Mnemonic.me, FXa),
                Instr32(Mnemonic.de, FXa),
                Instr32(Mnemonic.au, FXa),
                Instr32(Mnemonic.su, FXa),
                // 80
                Instr32(Mnemonic.ssm, S),
                Nyi("*"),
                Instr32(Mnemonic.lpsw, InstrClass.System|InstrClass.Linear, S),
                Nyi("*"),

                Instr32(Mnemonic.brxh, InstrClass.ConditionalTransfer, RSI),
                Instr32(Mnemonic.brxle, InstrClass.ConditionalTransfer, RSI),
                Instr32(Mnemonic.bxh, InstrClass.ConditionalTransfer, RSa),
                Instr32(Mnemonic.bxle, InstrClass.ConditionalTransfer, RSa),

                Instr32(Mnemonic.srl, RSa),
                Instr32(Mnemonic.sll, RSa),
                Instr32(Mnemonic.sra, RSa),
                Instr32(Mnemonic.sla, RSa),

                Instr32(Mnemonic.srdl, RSa),
                Instr32(Mnemonic.sldl, RSa),
                Instr32(Mnemonic.srda, RSa),
                Instr32(Mnemonic.slda, RSa),
                // 90
                Instr32(Mnemonic.stm, RSa3),
                Instr32(Mnemonic.tm, SI),
                Instr32(Mnemonic.mvi, SI),
                Instr32(Mnemonic.ts, S),

                Instr32(Mnemonic.ni, SI),
                Instr32(Mnemonic.cli, SI),
                Instr32(Mnemonic.oi, SI),
                Instr32(Mnemonic.xi, SI),

                Instr32(Mnemonic.lm, RSa3),
                Instr32(Mnemonic.trace, RSa3),
                Instr32(Mnemonic.lam, RSa3),
                Instr32(Mnemonic.stam, RSa3),

                invalid,
                invalid,
                invalid,
                invalid,

                // A0
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                ExtendMask32(16, 4,
                    (0x00, Instr(Mnemonic.iihh, RIa)),
                    (0x01, Instr(Mnemonic.iihl, RIa)),
                    (0x02, Instr(Mnemonic.iilh, RIa)),
                    (0x03, Instr(Mnemonic.iill, RIa)),
                    (0x08, Instr(Mnemonic.lhi, RIa)),
                    (0x09, Instr(Mnemonic.lghi, RIa)),
                    (0x0A, Instr(Mnemonic.ahi, RIa)),
                    (0x0F, Instr(Mnemonic.llill, RIa))),
                Nyi("*"),
                ExtendMask32(16, 4,
                    (0x04, Mask(20, 4,
                        Instr(Mnemonic.brc, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jo, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jh, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jnle, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jl, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jnhe, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jlh, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.je, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jnlh, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jhe, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jnl, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jle, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jnh, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.jno, InstrClass.ConditionalTransfer, RIc),
                        Instr(Mnemonic.j, InstrClass.Transfer, RIc))),
                    (0x7, Instr(Mnemonic.brctg, RIb)),
                    (0x8, Instr(Mnemonic.lhi, RIa)),
                    (0x9, Instr(Mnemonic.lghi, RIa)),
                    (0xA, Instr(Mnemonic.ahi, RIa)),
                    (0xB, Instr(Mnemonic.aghi, RIa)),
                    (0xE, Instr(Mnemonic.chi, RIa)),
                    (0xF, Instr(Mnemonic.cghi, RIa))),

                Instr32(Mnemonic.mvcle, RSa3),
                Nyi("*"),
                Instr32(Mnemonic.unpka, SSa),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr32(Mnemonic.sigp, InstrClass.System, RSa),
                Nyi("*"),

                // B0
                invalid,
                Instr32(Mnemonic.lra, RXa),
                b2_decoders,
                b3_decoders,

                invalid,
                invalid,
                Instr32(Mnemonic.stctl, RSa),
                Instr32(Mnemonic.lctl, RSa),

                invalid,
                b9_decoders,
                Instr32(Mnemonic.cs, RSa3),
                Instr32(Mnemonic.cds, RSa3),

                invalid,
                Instr32(Mnemonic.clm, RSb),
                Instr32(Mnemonic.stcm, RSb),
                Instr32(Mnemonic.icm, RSb),

                // C0
                ExtendMask48(32, 4,
                    (0x0, Instr(Mnemonic.larl, RILb)),
                    (0x4, Mask(36, 4,
                        Instr(Mnemonic.brcl, InstrClass.ConditionalTransfer,  RILc),
                        Instr(Mnemonic.jgo, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgh, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgnle, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgl, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgnhe, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jglh, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgne, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgnlh, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jghe, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgnl, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgle, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgnh, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jgno, InstrClass.ConditionalTransfer, RILc),
                        Instr(Mnemonic.jg, InstrClass.ConditionalTransfer, RILc))),
                    (0x5, Instr(Mnemonic.brasl, InstrClass.Transfer|InstrClass.Call,  RILb))),
                Nyi("*"),
                c2_decoders,
                Nyi("*"),

                ExtendMask48(32, 4,
                    (0x2, Instr(Mnemonic.llhrl, RILb)),
                    (0x4, Instr(Mnemonic.lghrl, RILb)),
                    (0x5, Instr(Mnemonic.lhrl, RILb)),
                    (0x6, Instr(Mnemonic.llghrl, RILb)),
                    (0x7, Instr(Mnemonic.sthrl, RILb)),
                    (0x8, Instr(Mnemonic.lgrl, RILb)),
                    (0xB, Instr(Mnemonic.stgrl, RILb)),
                    (0xC, Instr(Mnemonic.lgfrl, RILb)),
                    (0xD, Instr(Mnemonic.lrl, RILb)),
                    (0xE, Instr(Mnemonic.llgfrl, RILb)),
                    (0xF, Instr(Mnemonic.strl, RILb))),
                Instr48(Mnemonic.bprp, MII),
                c6_decoders,
                Instr48(Mnemonic.bpp, SMI),

                Nyi("*"),
                invalid,
                invalid,
                invalid,

                Nyi("*"),
                invalid,
                invalid,
                invalid,

                // D0
                Instr48(Mnemonic.trtr, SSa),
                Instr48(Mnemonic.mvc, SSa),
                Instr48(Mnemonic.mvz, SSa),
                Instr48(Mnemonic.xc, SSa),

                Instr48(Mnemonic.nc, SSa),
                Instr48(Mnemonic.clc, SSa),
                Instr48(Mnemonic.oc, SSa),
                Instr48(Mnemonic.xc, SSa),

                Nyi("*"),
                Nyi("*"),
                Instr48(Mnemonic.mvcp, InstrClass.Linear|InstrClass.System, SSd),
                Instr48(Mnemonic.mvcs, InstrClass.Linear|InstrClass.System, SSd),

                Nyi("*"),
                Instr48(Mnemonic.trt, SSa),
                Nyi("*"),
                Instr48(Mnemonic.edmk, SSa),

                // E0
                invalid,
                Instr48(Mnemonic.pku, SSf),
                Instr48(Mnemonic.unpku, SSa),
                e3_decoders,

                Nyi("*"),
                e5_decoders,
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr48(Mnemonic.unpka, SSa),
                eb_decoders,

                ec_decoders,
                ed_decoders,
                Nyi("*"),
                Nyi("*"),

                // F0
                Instr48(Mnemonic.srp, SSc),
                Instr48(Mnemonic.mvo, SSb),
                Instr48(Mnemonic.pack, SSb),
                Instr48(Mnemonic.unpk, SSb),

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi("*"),
                Nyi("*"),
                Instr48(Mnemonic.ap, SSb),
                Instr48(Mnemonic.sp, SSb),

                Instr48(Mnemonic.mp, SSb),
                Instr48(Mnemonic.dp, SSb),
                invalid,
                invalid,
            };
        }
    }
}