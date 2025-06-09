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
        private PrimitiveType? vecElemSize;
        private bool singleElement;
        private bool setCc;

        public zSeriesDisassembler(zSeriesArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override zSeriesInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out var opcode))
                return null;
            this.ops.Clear();
            this.vecElemSize = null;
            this.singleElement = false;
            this.setCc = false;
            var instr = decoders[opcode >> 8].Decode(opcode, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
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
                ElementSize = vecElemSize,
                SingleElement = singleElement,
                SetConditionCode = setCc,
            };
            return instr;
        }

        public override zSeriesInstruction CreateInvalidInstruction()
        {
            return new zSeriesInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        private MachineOperand CreateAccess(
            RegisterStorage baseReg,
            RegisterStorage idxReg,
            int offset)
        {
            if ((baseReg is null || baseReg.Number == 0) &&
                (idxReg is null || idxReg.Number == 0))
            {
                return Address.Ptr32((uint) offset);
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
            if (baseReg is null || baseReg.Number == 0)
            {
                return Address.Ptr32((uint) offset);
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
            dasm.ops.Add(Registers.FpRegisters[(uInstr >> 4) & 0xF]);
            dasm.ops.Add(Registers.FpRegisters[(uInstr) & 0xF]);
            return true;
        }

        public static bool FFE(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.FpRegisters[(uInstr >> 4) & 0xF]);
            dasm.ops.Add(Registers.FpRegisters[(uInstr) & 0xF]);
            return true;
        }

        public static bool FXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = Registers.FpRegisters[(uInstr >> 20) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(f1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool I(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i = Constant.Byte((byte) uInstr);
            dasm.ops.Add(i);
            return true;
        }

        public static bool IE(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i1 = Constant.Byte((byte) ((uInstr >> 4) & 0xF));
            var i2 = Constant.Byte((byte) (uInstr & 0xF));
            dasm.ops.Add(i1);
            dasm.ops.Add(i2);
            return true;
        }


        public static bool MII(ulong uInstr, zSeriesDisassembler dasm)
        {
            var m1 = (byte) ((uInstr >> 36) & 0xF);
            var r2 = (int) Bits.SignExtend((uint) (uInstr >> 24), 12);
            var r3 = (int) Bits.SignExtend((uint) uInstr, 24);
            dasm.ops.Add(Constant.Byte(m1));
            dasm.ops.Add(Constant.Int32(r2));
            dasm.ops.Add(Constant.Int32(r3));
            return true;
        }

        public static bool R(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[uInstr & 0xF]);
            return true;
        }

        public static bool RR(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 4) & 0xF]);
            dasm.ops.Add(Registers.GpRegisters[(uInstr) & 0xF]);
            return true;
        }

        public static bool RRE(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 4) & 0xF]);
            dasm.ops.Add(Registers.GpRegisters[(uInstr) & 0xF]);
            return true;
        }

        public static bool RRFa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 4) & 0xF];
            var r2 = Registers.GpRegisters[uInstr & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            dasm.ops.Add(r3);
            return true;
        }

        public static bool RRFb(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RRFb instr"); return false; }
        public static bool RRFc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 4) & 0xF];
            var r2 = Registers.GpRegisters[uInstr & 0xF];
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            return true;
        }

        public static bool RRFd(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RRFd instr"); return false; }
        public static bool RRFe(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RRFe instr"); return false; }


        public static bool RRS(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RRS instr"); return false; }

        public static bool RIa(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 20) & 0xF]);
            dasm.ops.Add(Constant.Int32((int)Bits.SignExtend(uInstr, 16)));
            return true;
        }

        public static bool RIb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2 * (int)Bits.SignExtend(uInstr, 16);
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 20) & 0xF]);
            dasm.ops.Add(addr);
            return true;
        }

        public static bool RIc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2*(int)Bits.SignExtend(uInstr, 16);
            dasm.ops.Add(addr);
            return true;
        }

        public static bool RIEa(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RIEa instr"); return false; }

        private static bool RIEb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var r2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var m3 = (byte) ((uInstr >> 12) & 0xF);
            var addr = dasm.addr + 2 * (short) Bits.SignExtend(uInstr >> 16, 16);
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            dasm.ops.Add(Constant.Byte(m3));
            dasm.ops.Add(addr);
            return true;
        }

        private static bool RIEc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var i2 = Constant.SByte((sbyte)Bits.SignExtend(uInstr >> 8, 8));
            var m3 = (byte) ((uInstr >> 32) & 0xF);
            var addr = dasm.addr + 2 * (short) Bits.SignExtend(uInstr >> 16, 16);
            dasm.ops.Add(r1);
            dasm.ops.Add(i2);
            dasm.ops.Add(Constant.Byte(m3));
            dasm.ops.Add(addr);
            return true;
        }

        private static bool RIEd(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var i2 = Constant.Int16((short)(uInstr >> 16));
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            dasm.ops.Add(r1);
            dasm.ops.Add(i2);
            dasm.ops.Add(r3);
            return true;
        }

        private static bool RIEe(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var ri2 = dasm.addr + 2 * (int) Bits.SignExtend(uInstr, 32);
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(ri2);
            return true;
        }

        private static bool RIEf(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var r2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var i3 = Constant.Byte((byte) (uInstr >> 24));
            var i4 = Constant.Byte((byte) (uInstr >> 16));
            var i5 = Constant.Byte((byte) (uInstr >> 8));
            dasm.ops.Add(r1);
            dasm.ops.Add(r2);
            dasm.ops.Add(i3);
            dasm.ops.Add(i4);
            dasm.ops.Add(i5);
            return true;
        }

        public static bool RIEg(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RIEg instr"); return false; }

        public static bool RILa(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var i2 = Constant.Word32((uint) Bits.SignExtend(uInstr, 32));
            dasm.ops.Add(i2);
            return true;
        }

        public static bool RILb(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.ops.Add(Registers.GpRegisters[(uInstr >> 36) & 0xF]);
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.ops.Add(dasm.addr + offset);
            return true;
        }

        public static bool RILc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.ops.Add(dasm.addr + offset);
            return true;
        }

        public static bool RIS(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RIS instr"); return false; }

        public static bool RSI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var offset = 2 * (short) uInstr;
            var ri2 = dasm.addr + offset;
            var r3 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(ri2);
            return true;
        }

        private static bool RSL(ulong uInstr, zSeriesDisassembler dasm) { dasm.NotYetImplemented("RSL instr"); return false; }
        private static bool RSLa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int) Bits.SignExtend(uInstr >> 16, 12);
            var l1 = (byte) ((uInstr >> 36) & 0xF);
            dasm.ops.Add(dasm.CreateAccessLength(b1, d1, l1 + 1));
            return true;
        }

        private static bool RSLb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr >> 16, 12);
            var l2 = (byte) (uInstr >> 32);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccessLength(b2, d2, l2 + 1));
            return true;
        }

        public static bool RXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
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
            dasm.ops.Add(f1);
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
            dasm.ops.Add(f1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            dasm.ops.Add(f3);
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
            var m1 = Constant.Byte((byte) (uInstr >> 36 & 0xF));
            var addr = dasm.addr + 2 * (int) Bits.SignExtend(uInstr, 16);
            var b3 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d3 = (int) Bitfield.ReadSignedFields(siyDisplacement, uInstr);
            dasm.ops.Add(m1);
            dasm.ops.Add(addr);
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
            dasm.ops.Add(Constant.Byte(i3));
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
            dasm.ops.Add(r3);
            return true;
        }

        public static bool SSe(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr >> 16, 12);
            var b4 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d4 = (int) Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.ops.Add(r3);
            dasm.ops.Add(dasm.CreateAccess(b4, d4));
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

        public static bool SSF(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int) Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr, 12);
            var r3 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            dasm.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.ops.Add(r3);
            return true;
        }

        public static bool RSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSar(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int) Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSa3(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var m3 = Constant.Byte((byte)((uInstr >> 16) & 0xF));
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(r1);
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
            dasm.ops.Add(r1);
            dasm.ops.Add(r3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static bool SIL(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (short) uInstr >> 16;
            var i2 = Constant.Word16((ushort) uInstr);
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
            var i2 = Constant.Byte((byte) (uInstr >> 32));
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
            dasm.ops.Add(r1);
            dasm.ops.Add(Constant.Byte(m3));
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
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rxya_offset, (uint)uInstr);
            dasm.ops.Add(r1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool RXYb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var m1 = Constant.Byte((byte) ((uInstr >> 36) & 0xF));
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rxya_offset, (uint) uInstr);
            dasm.ops.Add(m1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool FXYa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rxya_offset, (uint) uInstr);
            dasm.ops.Add(f1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool SI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i2 = Constant.Byte((byte)(uInstr >> 16));
            var b1 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.ops.Add(i2);
            return true;
        }

        private static readonly Bitfield[] bf_v36 = Bf((35, 1), (36, 4)); 
        private static readonly Bitfield[] bf_v32 = Bf((34, 1), (32, 4));
        private static readonly Bitfield[] bf_v28 = Bf((33, 1), (28, 4));
        private static readonly Bitfield[] bf_v12 = Bf((32, 1), (12, 4));

        private static bool VRIa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var i1 = (ushort) (uInstr >> 16);
            dasm.ops.Add(v1);
            dasm.ops.Add(Constant.Word16(i1));
            return true;
        }

        private static bool VRIb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var i1 = (byte) (uInstr >> 24);
            var i2 = (byte) (uInstr >> 16);
            dasm.ops.Add(v1);
            dasm.ops.Add(Constant.Byte(i1));
            dasm.ops.Add(Constant.Byte(i2));
            return true;
        }

        private static bool VRIc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var imm = (ushort) (uInstr >> 16);
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(Constant.Word16(imm));
            return true;
        }

        private static bool VRId(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v28, uInstr)];
            var imm = (byte) (uInstr >> 16);
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(v3);
            dasm.ops.Add(Constant.Byte(imm));
            return true;
        }

        private static bool VRIe(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var imm = (ushort) Bits.ZeroExtend((uInstr >> 20), 12);
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(Constant.Word16(imm));
            return true;
        }

        private static bool VRRa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            return true;
        }

        private static bool VRRb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v28, uInstr)];
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(v3);
            return true;
        }

        private static bool VRRc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v28, uInstr)];
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(v3);
            return true;
        }

        private static bool VRRd(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v28, uInstr)];
            var v4 = Registers.VecRegisters[Bitfield.ReadFields(bf_v12, uInstr)];
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(v3);
            dasm.ops.Add(v4);
            return true;
        }

        private static bool VRRe(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v28, uInstr)];
            var v4 = Registers.VecRegisters[Bitfield.ReadFields(bf_v12, uInstr)];
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(v3);
            dasm.ops.Add(v4);
            return true;
        }

        private static bool VRRf(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var r2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            dasm.ops.Add(v1);
            dasm.ops.Add(r2);
            dasm.ops.Add(r3);
            return true;
        }

        private static bool VRSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var d2 = (int) Bits.ZeroExtend(uInstr >> 16, 12);
            dasm.ops.Add(v1);
            dasm.ops.Add(v3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static bool VRSb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var d2 = (int) Bits.ZeroExtend(uInstr >> 16, 12);
            dasm.ops.Add(v1);
            dasm.ops.Add(r3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static bool VRSc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var v3 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var d2 = (int) Bits.ZeroExtend(uInstr >> 16, 12);
            dasm.ops.Add(r1);
            dasm.ops.Add(v3);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }
       
        private static bool VRV(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var v2 = Registers.VecRegisters[Bitfield.ReadFields(bf_v32, uInstr)];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = (int) Bits.ZeroExtend(uInstr >> 16, 12);
            dasm.ops.Add(v1);
            dasm.ops.Add(v2);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }
        private static bool VRX(ulong uInstr, zSeriesDisassembler dasm)
        {
            var v1 = Registers.VecRegisters[Bitfield.ReadFields(bf_v36, uInstr)];
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var x2 = Registers.GpRegisters[(uInstr >> 32) & 0xF];
            var d2 = (int) Bits.ZeroExtend(uInstr >> 16, 12);
            dasm.ops.Add(v1);
            dasm.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static readonly Bitfield vecelemField = new Bitfield(12, 4);
        /// <summary>
        /// Extract the vector element size.
        /// </summary>
        private static WideMutator<zSeriesDisassembler> vecelem(PrimitiveType?[] vectorElementTypes)
        {
            return (u, d) =>
            {
                var size = vecelemField.Read(u);
                d.vecElemSize = size < vectorElementTypes.Length
                    ? vectorElementTypes[size]
                    : null;
                return d.vecElemSize is not null;
            };
        }
        private static readonly WideMutator<zSeriesDisassembler> Sbhf = vecelem(new[] {
            PrimitiveType.SByte, PrimitiveType.Int16, PrimitiveType.Int32, PrimitiveType.Int64,
        });
        private static readonly WideMutator<zSeriesDisassembler> Sbhfg = vecelem(new[] {
            PrimitiveType.SByte, PrimitiveType.Int16, PrimitiveType.Int32, PrimitiveType.Int64,
        });
        private static readonly WideMutator<zSeriesDisassembler> S_hfg = vecelem(new[] {
            null, PrimitiveType.Int16, PrimitiveType.Int32, PrimitiveType.Int64,
        });

        private static readonly WideMutator<zSeriesDisassembler> Ubhf = vecelem(new[] {
            PrimitiveType.Byte,   PrimitiveType.Word16, PrimitiveType.Word32,
        });
        private static readonly WideMutator<zSeriesDisassembler> Ubhfg = vecelem(new PrimitiveType[] {
            PrimitiveType.Byte,   PrimitiveType.Word16, PrimitiveType.Word32, PrimitiveType.Word64,
        });
        private static readonly WideMutator<zSeriesDisassembler> U_hfg = vecelem(new[] {
            null, PrimitiveType.Word16, PrimitiveType.Word32, PrimitiveType.Word64,
        });

        private static readonly WideMutator<zSeriesDisassembler> Fg = vecelem(new PrimitiveType?[] {
            null, null, null, PrimitiveType.Real64,
        });


        /// <summary>
        /// Instruction checks the Single-Element bit.
        /// </summary>
        private static bool Se(ulong uInstr, zSeriesDisassembler dasm)
        {
            const int bit = 16 + 3;
            dasm.singleElement = Bits.IsBitSet(uInstr, bit);
            return true;
        }

        /// <summary>
        /// Sets the condition code
        /// </summary>
        private static bool Scc(ulong uInstr, zSeriesDisassembler dasm)
        {
            const int bit = 20;
            dasm.setCc = Bits.IsBitSet(uInstr, bit);
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
                    Debug.Assert(this.decoders[d.Item1] is null);
                    this.decoders[d.Item1] = d.Item2;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] is null)
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
                    Debug.Assert(this.decoders[d.Item1] is null);
                    this.decoders[d.Item1] = d.Item2;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] is null)
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
                        Debug.Assert(this.decoders[d.Item1] is null);
                        this.decoders[d.Item1] = d.Item2;
                    }
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] is null)
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

            var b2_decoders = ExtendMask32(16, 8,
                (0x02, Instr(Mnemonic.stidp, S)),
                (0x04, Instr(Mnemonic.sck, S)),
                (0x05, Instr(Mnemonic.stck, S)),
                (0x06, Instr(Mnemonic.sckc, S)),
                (0x07, Instr(Mnemonic.stckc, S)),
                (0x08, Instr(Mnemonic.spt, S)),
                (0x09, Instr(Mnemonic.stpt, S)),
                (0x0A, Instr(Mnemonic.spka, S)),
                (0x0B, Instr(Mnemonic.ipk, S)),
                (0x0D, Instr(Mnemonic.ptlb, S)),
                (0x10, Instr(Mnemonic.spx, S)),
                (0x11, Instr(Mnemonic.stpx, S)),
                (0x12, Instr(Mnemonic.stap, S)),
                (0x18, Instr(Mnemonic.pc, S)),
                (0x19, Instr(Mnemonic.sac, S)),
                (0x1A, Instr(Mnemonic.cfc, S)),
                (0x21, Instr(Mnemonic.ipte, RRFa)),
                (0x22, Instr(Mnemonic.ipm, RRE)),
                (0x23, Instr(Mnemonic.ivsk, RRE)),
                (0x24, Instr(Mnemonic.iac, RRE)),
                (0x25, Instr(Mnemonic.ssar, RRE)),
                (0x26, Instr(Mnemonic.epar, RRE)),
                (0x27, Instr(Mnemonic.esar, RRE)),
                (0x28, Instr(Mnemonic.pt, RRE)),
                (0x29, Instr(Mnemonic.iske, RRE)),
                (0x2A, Instr(Mnemonic.rrbe, RRE)),
                (0x2B, Instr(Mnemonic.sske, RRFc)),
                (0x2C, Instr(Mnemonic.tb, RRE)),
                (0x2D, Instr(Mnemonic.dxr, RRE)),
                (0x2E, Instr(Mnemonic.pgin, RRE)),
                (0x2F, Instr(Mnemonic.pgout, RRE)),
                (0x30, Instr(Mnemonic.csch, S)),
                (0x31, Instr(Mnemonic.hsch, S)),
                (0x32, Instr(Mnemonic.msch, S)),
                (0x33, Instr(Mnemonic.ssch, S)),
                (0x34, Instr(Mnemonic.stsch, S)),
                (0x35, Instr(Mnemonic.tsch, S)),
                (0x36, Instr(Mnemonic.tpi, S)),
                (0x37, Instr(Mnemonic.sal, S)),
                (0x38, Instr(Mnemonic.rsch, S)),
                (0x39, Instr(Mnemonic.stcrw, S)),
                (0x3A, Instr(Mnemonic.stcps, S)),
                (0x3B, Instr(Mnemonic.rchp, S)),
                (0x3C, Instr(Mnemonic.schm, S)),
                (0x40, Instr(Mnemonic.bakr, RRE)),
                (0x41, Instr(Mnemonic.cksm, RRE)),
                (0x44, Instr(Mnemonic.sqdr, RRE)),
                (0x45, Instr(Mnemonic.sqer, RRE)),
                (0x46, Instr(Mnemonic.stura, RRE)),
                (0x47, Instr(Mnemonic.msta, RRE)),
                (0x48, Instr(Mnemonic.palb, RRE)),
                (0x49, Instr(Mnemonic.ereg, RRE)),
                (0x4A, Instr(Mnemonic.esta, RRE)),
                (0x4B, Instr(Mnemonic.lura, RRE)),
                (0x4C, Instr(Mnemonic.tar, RRE)),
                (0x4D, Instr(Mnemonic.cpya, RRE)),
                (0x4E, Instr(Mnemonic.sar, RRE)),
                (0x4F, Instr(Mnemonic.ear, RRE)),
                (0x50, Instr(Mnemonic.csp, RRE)),
                (0x52, Instr(Mnemonic.msr, RRE)),
                (0x54, Instr(Mnemonic.mvpg, RRE)),
                (0x55, Instr(Mnemonic.mvst, RRE)),
                (0x57, Instr(Mnemonic.cuse, RRE)),
                (0x58, Instr(Mnemonic.bsg, RRE)),
                (0x5A, Instr(Mnemonic.bsa, RRE)),
                (0x5D, Instr(Mnemonic.clst, RRE)),
                (0x5E, Instr(Mnemonic.srst, RRE)),
                (0x63, Instr(Mnemonic.cmpsc, RRE)),
                (0x76, Instr(Mnemonic.xsch, S)),
                (0x77, Instr(Mnemonic.rp, S)),
                (0x78, Instr(Mnemonic.stcke, S)),
                (0x79, Instr(Mnemonic.sacf, S)),
                (0x7C, Instr(Mnemonic.stckf, S)),
                (0x7D, Instr(Mnemonic.stsi, S)),
                (0x99, Instr(Mnemonic.srnm, S)),
                (0x9C, Instr(Mnemonic.stfpc, S)),
                (0x9D, Instr(Mnemonic.lfpc, S)),
                (0xA5, Instr(Mnemonic.tre, RRE)),
                (0xA6, Instr(Mnemonic.cuutf, RRFc)),
                //(0xA6, Instr(Mnemonic.to, UTF8)),
                (0xA7, Instr(Mnemonic.cutfu, RRFc)),
                //(0xA7, Instr(Mnemonic.cu12, RRFc)),
                (0xB0, Instr(Mnemonic.stfle, S)),
                (0xB1, Instr(Mnemonic.stfl, S)),
                (0xB2, Instr(Mnemonic.lpswe, S)),
                (0xB8, Instr(Mnemonic.srnmb, S)),
                (0xB9, Instr(Mnemonic.srnmt, S)),
                (0xBD, Instr(Mnemonic.lfas, S)),
                (0xE8, Instr(Mnemonic.ppa, RRFc)),
                (0xEC, Instr(Mnemonic.etnd, RRE)),
                (0xF8, Instr(Mnemonic.tend, S)),
                (0xFA, Instr(Mnemonic.niai, IE)),
                (0xFC, Instr(Mnemonic.tabort, S)),
                (0xFF, Instr(Mnemonic.trap4, S)));

            var b3_decoders = ExtendMask32(16, 8,
                (0x37, Instr(Mnemonic.meer, FFE)),
                (0xC1, Instr(Mnemonic.ldgr, RRE)),
                (0xCD, Instr(Mnemonic.lgdr, RRE)));

            var b9_decoders = ExtendMask32(16, 8,
                (0x00, Instr(Mnemonic.lpgr, RRE)),
                (0x01, Instr(Mnemonic.lngr, RRE)),
                (0x02, Instr(Mnemonic.ltgr, RRE)),
                (0x03, Instr(Mnemonic.lcgr, RRE)),
                (0x04, Instr(Mnemonic.lgr, RRE)),
                (0x05, Instr(Mnemonic.lurag, RRE)),
                (0x06, Instr(Mnemonic.lgbr, RRE)),
                (0x07, Instr(Mnemonic.lghr, RRE)),
                (0x08, Instr(Mnemonic.agr, RRE)),
                (0x09, Instr(Mnemonic.sgr, RRE)),
                (0x0A, Instr(Mnemonic.algr, RRE)),
                (0x0B, Instr(Mnemonic.slgr, RRE)),
                (0x0C, Instr(Mnemonic.msgr, RRE)),
                (0x0D, Instr(Mnemonic.dsgr, RRE)),
                (0x0E, Instr(Mnemonic.eregg, RRE)),
                (0x0F, Instr(Mnemonic.lrvgr, RRE)),
                (0x10, Instr(Mnemonic.lpgfr, RRE)),
                (0x11, Instr(Mnemonic.lngfr, RRE)),
                (0x12, Instr(Mnemonic.ltgfr, RRE)),
                (0x13, Instr(Mnemonic.lcgfr, RRE)),
                (0x14, Instr(Mnemonic.lgfr, RRE)),
                (0x16, Instr(Mnemonic.llgfr, RRE)),
                (0x17, Instr(Mnemonic.llgtr, RRE)),
                (0x18, Instr(Mnemonic.agfr, RRE)),
                (0x19, Instr(Mnemonic.sgfr, RRE)),
                (0x1A, Instr(Mnemonic.algfr, RRE)),
                (0x1B, Instr(Mnemonic.slgfr, RRE)),
                (0x1C, Instr(Mnemonic.msgfr, RRE)),
                (0x1D, Instr(Mnemonic.dsgfr, RRE)),
                (0x1E, Instr(Mnemonic.kmac, RRE)),
                (0x1F, Instr(Mnemonic.lrvr, RRE)),
                (0x20, Instr(Mnemonic.cgr, RRE)),
                (0x21, Instr(Mnemonic.clgr, RRE)),
                (0x25, Instr(Mnemonic.sturg, RRE)),
                (0x26, Instr(Mnemonic.lbr, RRE)),
                (0x27, Instr(Mnemonic.lhr, RRE)),
                (0x28, Instr(Mnemonic.pckmo, RRE)),
                (0x2A, Instr(Mnemonic.kmf, RRE)),
                (0x2B, Instr(Mnemonic.kmo, RRE)),
                (0x2C, Instr(Mnemonic.pcc, RRE)),
                (0x2D, Instr(Mnemonic.kmctr, RRFb)),
                (0x2E, Instr(Mnemonic.km, RRE)),
                (0x2F, Instr(Mnemonic.kmc, RRE)),
                (0x30, Instr(Mnemonic.cgfr, RRE)),
                (0x31, Instr(Mnemonic.clgfr, RRE)),
                (0x3C, Instr(Mnemonic.ppno, RRE)),
                (0x3E, Instr(Mnemonic.kimd, RRE)),
                (0x3F, Instr(Mnemonic.klmd, RRE)),
                (0x41, Instr(Mnemonic.cfdtr, RRFe)),
                (0x42, Instr(Mnemonic.clgdtr, RRFe)),
                (0x43, Instr(Mnemonic.clfdtr, RRFe)),
                (0x46, Instr(Mnemonic.bctgr, RRE)),
                (0x49, Instr(Mnemonic.cfxtr, RRFe)),
                (0x4A, Instr(Mnemonic.clgxtr, RRFe)),
                (0x4B, Instr(Mnemonic.clfxtr, RRFe)),
                (0x51, Instr(Mnemonic.cdftr, RRE)),
                (0x52, Instr(Mnemonic.cdlgtr, RRFe)),
                (0x53, Instr(Mnemonic.cdlftr, RRFe)),
                (0x59, Instr(Mnemonic.cxftr, RRE)),
                (0x5A, Instr(Mnemonic.cxlgtr, RRFe)),
                (0x5B, Instr(Mnemonic.cxlftr, RRFe)),
                (0x60, Instr(Mnemonic.cgrt, RRFc)),
                (0x61, Instr(Mnemonic.clgrt, RRFc)),
                (0x72, Instr(Mnemonic.crt, RRFc)),
                (0x73, Instr(Mnemonic.clrt, RRFc)),
                (0x80, Instr(Mnemonic.ngr, RRE)),
                (0x81, Instr(Mnemonic.ogr, RRE)),
                (0x82, Instr(Mnemonic.xgr, RRE)),
                (0x83, Instr(Mnemonic.flogr, RRE)),
                (0x84, Instr(Mnemonic.llgcr, RRE)),
                (0x85, Instr(Mnemonic.llghr, RRE)),
                (0x86, Instr(Mnemonic.mlgr, RRE)),
                (0x87, Instr(Mnemonic.dlgr, RRE)),
                (0x88, Instr(Mnemonic.alcgr, RRE)),
                (0x89, Instr(Mnemonic.slbgr, RRE)),
                (0x8A, Instr(Mnemonic.cspg, RRE)),
                (0x8D, Instr(Mnemonic.epsw, RRE)),
                (0x8E, Instr(Mnemonic.idte, RRFb)),
                (0x8F, Instr(Mnemonic.crdte, RRFb)),
                (0x90, Instr(Mnemonic.trtt, RRFc)),
                (0x91, Instr(Mnemonic.trto, RRFc)),
                (0x92, Instr(Mnemonic.trot, RRFc)),
                (0x93, Instr(Mnemonic.troo, RRFc)),
                (0x94, Instr(Mnemonic.llcr, RRE)),
                (0x95, Instr(Mnemonic.llhr, RRE)),
                (0x96, Instr(Mnemonic.mlr, RRE)),
                (0x97, Instr(Mnemonic.dlr, RRE)),
                (0x98, Instr(Mnemonic.alcr, RRE)),
                (0x99, Instr(Mnemonic.slbr, RRE)),
                (0x9A, Instr(Mnemonic.epair, RRE)),
                (0x9B, Instr(Mnemonic.esair, RRE)),
                (0x9D, Instr(Mnemonic.esea, RRE)),
                (0x9E, Instr(Mnemonic.pti, RRE)),
                (0x9F, Instr(Mnemonic.ssair, RRE)),
                (0xA2, Instr(Mnemonic.ptf, RRE)),
                (0xAA, Instr(Mnemonic.lptea, RRFb)),
                (0xAE, Instr(Mnemonic.rrbm, RRE)),
                (0xAF, Instr(Mnemonic.pfmf, RRE)),
                (0xB0, Instr(Mnemonic.cu14, RRFc)),
                (0xB1, Instr(Mnemonic.cu24, RRFc)),
                (0xB2, Instr(Mnemonic.cu41, RRE)),
                (0xB3, Instr(Mnemonic.cu42, RRE)),
                (0xBD, Instr(Mnemonic.trtre, RRFc)),
                (0xBE, Instr(Mnemonic.srstu, RRE)),
                (0xBF, Instr(Mnemonic.trte, RRFc)),
                (0xC8, Instr(Mnemonic.ahhhr, RRFa)),
                (0xC9, Instr(Mnemonic.shhhr, RRFa)),
                (0xCA, Instr(Mnemonic.alhhhr, RRFa)),
                (0xCB, Instr(Mnemonic.slhhhr, RRFa)),
                (0xCD, Instr(Mnemonic.chhr, RRE)),
                (0xCF, Instr(Mnemonic.clhhr, RRE)),
                (0xD8, Instr(Mnemonic.ahhlr, RRFa)),
                (0xD9, Instr(Mnemonic.shhlr, RRFa)),
                (0xDA, Instr(Mnemonic.alhhlr, RRFa)),
                (0xDB, Instr(Mnemonic.slhhlr, RRFa)),
                (0xDD, Instr(Mnemonic.chlr, RRE)),
                (0xDF, Instr(Mnemonic.clhlr, RRE)),
                (0xE0, Instr(Mnemonic.locfhr, RRFc)),
                (0xE1, Instr(Mnemonic.popcnt, RRE)),
                (0xE2, Mask(12, 4,
                    Instr(Mnemonic.locgrnv, RRFc),
                    Instr(Mnemonic.locgro, RRFc),
                    Instr(Mnemonic.locgrh, RRFc),
                    Instr(Mnemonic.locgrnle, RRFc),
                    Instr(Mnemonic.locgrl, RRFc),
                    Instr(Mnemonic.locgrnhe, RRFc),
                    Instr(Mnemonic.locgrlh, RRFc),
                    Instr(Mnemonic.locgrne, RRFc),
                    Instr(Mnemonic.locgre, RRFc),
                    Instr(Mnemonic.locgrnlh, RRFc),
                    Instr(Mnemonic.locgrhe, RRFc),
                    Instr(Mnemonic.locgrnl, RRFc),
                    Instr(Mnemonic.locgrle, RRFc),
                    Instr(Mnemonic.locgrnh, RRFc),
                    Instr(Mnemonic.locgrno, RRFc),
                    Instr(Mnemonic.locgr, RRFc))),
                (0xE4, Instr(Mnemonic.ngrk, RRFa)),
                (0xE6, Instr(Mnemonic.ogrk, RRFa)),
                (0xE7, Instr(Mnemonic.xgrk, RRFa)),
                (0xE8, Instr(Mnemonic.agrk, RRFa)),
                (0xE9, Instr(Mnemonic.sgrk, RRFa)),
                (0xEA, Instr(Mnemonic.algrk, RRFa)),
                (0xEB, Instr(Mnemonic.slgrk, RRFa)),
                (0xF2, Mask(12, 4, 
                    Instr(Mnemonic.locr, RRFc),
                    Instr(Mnemonic.locro, RRFc),
                    Instr(Mnemonic.locrh, RRFc),
                    Instr(Mnemonic.locrnle, RRFc),
                    Instr(Mnemonic.locrl, RRFc),
                    Instr(Mnemonic.locrnhe, RRFc),
                    Instr(Mnemonic.locrlh, RRFc),
                    Instr(Mnemonic.locrne, RRFc),
                    Instr(Mnemonic.locre, RRFc),
                    Instr(Mnemonic.locrnlh, RRFc),
                    Instr(Mnemonic.locrhe, RRFc),
                    Instr(Mnemonic.locrnl, RRFc),
                    Instr(Mnemonic.locrle, RRFc),
                    Instr(Mnemonic.locrnh, RRFc),
                    Instr(Mnemonic.locrno, RRFc),
                    Instr(Mnemonic.locr, RRFc))),
                (0xF4, Instr(Mnemonic.nrk, RRFa)),
                (0xF6, Instr(Mnemonic.ork, RRFa)),
                (0xF7, Instr(Mnemonic.xrk, RRFa)),
                (0xF8, Instr(Mnemonic.ark, RRFa)),
                (0xF9, Instr(Mnemonic.srk, RRFa)),
                (0xFA, Instr(Mnemonic.alrk, RRFa)),
                (0xFB, Instr(Mnemonic.slrk, RRFa)));

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

            var cc_decoders = ExtendMask32(32, 4,
                (0x6, Instr(Mnemonic.brcth, InstrClass.ConditionalTransfer, RILb)),
                (0x8, Instr(Mnemonic.aih, RILa)),
                (0xA, Instr(Mnemonic.alsih, RILa)),
                (0xB, Instr(Mnemonic.alsihn, RILa)),
                (0xD, Instr(Mnemonic.cih, RILa)),
                (0xF, Instr(Mnemonic.clih, RILa)));

            var e3_decoders = ExtendMask48(0, 8,
                (0x02, Instr(Mnemonic.ltg, RXYa)),
                (0x03, Instr(Mnemonic.lrag, RXYa)),
                (0x04, Instr(Mnemonic.lg, RXYa)),
                (0x06, Instr(Mnemonic.cvby, RXYa)),
                (0x08, Instr(Mnemonic.ag, RXYa)),
                (0x09, Instr(Mnemonic.sg, RXYa)),
                (0x0A, Instr(Mnemonic.alg, RXYa)),
                (0x0B, Instr(Mnemonic.slg, RXYa)),
                (0x0C, Instr(Mnemonic.msg, RXYa)),
                (0x0D, Instr(Mnemonic.dsg, RXYa)),
                (0x0E, Instr(Mnemonic.cvbg, RXYa)),
                (0x0F, Instr(Mnemonic.lrvg, RXYa)),
                (0x12, Instr(Mnemonic.lt, RXYa)),
                (0x13, Instr(Mnemonic.lray, RXYa)),
                (0x14, Instr(Mnemonic.lgf, RXYa)),
                (0x15, Instr(Mnemonic.lgh, RXYa)),
                (0x16, Instr(Mnemonic.llgf, RXYa)),
                (0x17, Instr(Mnemonic.llgt, RXYa)),
                (0x18, Instr(Mnemonic.agf, RXYa)),
                (0x19, Instr(Mnemonic.sgf, RXYa)),
                (0x1A, Instr(Mnemonic.algf, RXYa)),
                (0x1B, Instr(Mnemonic.slgf, RXYa)),
                (0x1C, Instr(Mnemonic.msgf, RXYa)),
                (0x1D, Instr(Mnemonic.dsgf, RXYa)),
                (0x1E, Instr(Mnemonic.lrv, RXYa)),
                (0x1F, Instr(Mnemonic.lrvh, RXYa)),
                (0x20, Instr(Mnemonic.cg, RXYa)),
                (0x21, Instr(Mnemonic.clg, RXYa)),
                (0x24, Instr(Mnemonic.stg, RXYa)),
                (0x25, Instr(Mnemonic.ntstg, RXYa)),
                (0x26, Instr(Mnemonic.cvdy, RXYa)),
                (0x2A, Instr(Mnemonic.lzrg, RXYa)),
                (0x2E, Instr(Mnemonic.cvdg, RXYa)),
                (0x2F, Instr(Mnemonic.strvg, RXYa)),
                (0x30, Instr(Mnemonic.cgf, RXYa)),
                (0x31, Instr(Mnemonic.clgf, RXYa)),
                (0x32, Instr(Mnemonic.ltgf, RXYa)),
                (0x34, Instr(Mnemonic.cgh, RXYa)),
                (0x36, Instr(Mnemonic.pfd, RXYb)),
                (0x3A, Instr(Mnemonic.llzrgf, RXYa)),
                (0x3B, Instr(Mnemonic.lzrf, RXYa)),
                (0x3E, Instr(Mnemonic.strv, RXYa)),
                (0x3F, Instr(Mnemonic.strvh, RXYa)),
                (0x46, Instr(Mnemonic.bctg, RXYa)),
                (0x50, Instr(Mnemonic.sty, RXYa)),
                (0x51, Instr(Mnemonic.msy, RXYa)),
                (0x54, Instr(Mnemonic.ny, RXYa)),
                (0x55, Instr(Mnemonic.cly, RXYa)),
                (0x56, Instr(Mnemonic.oy, RXYa)),
                (0x57, Instr(Mnemonic.xy, RXYa)),
                (0x58, Instr(Mnemonic.ly, RXYa)),
                (0x59, Instr(Mnemonic.cy, RXYa)),
                (0x5A, Instr(Mnemonic.ay, RXYa)),
                (0x5B, Instr(Mnemonic.sy, RXYa)),
                (0x5C, Instr(Mnemonic.mfy, RXYa)),
                (0x5E, Instr(Mnemonic.aly, RXYa)),
                (0x5F, Instr(Mnemonic.sly, RXYa)),
                (0x70, Instr(Mnemonic.sthy, RXYa)),
                (0x71, Instr(Mnemonic.lay, RXYa)),
                (0x72, Instr(Mnemonic.stcy, RXYa)),
                (0x73, Instr(Mnemonic.icy, RXYa)),
                (0x75, Instr(Mnemonic.laey, RXYa)),
                (0x76, Instr(Mnemonic.lb, RXYa)),
                (0x77, Instr(Mnemonic.lgb, RXYa)),
                (0x78, Instr(Mnemonic.lhy, RXYa)),
                (0x79, Instr(Mnemonic.chy, RXYa)),
                (0x7A, Instr(Mnemonic.ahy, RXYa)),
                (0x7B, Instr(Mnemonic.shy, RXYa)),
                (0x7C, Instr(Mnemonic.mhy, RXYa)),
                (0x80, Instr(Mnemonic.ng, RXYa)),
                (0x81, Instr(Mnemonic.og, RXYa)),
                (0x82, Instr(Mnemonic.xg, RXYa)),
                (0x85, Instr(Mnemonic.lgat, RXYa)),
                (0x86, Instr(Mnemonic.mlg, RXYa)),
                (0x87, Instr(Mnemonic.dlg, RXYa)),
                (0x88, Instr(Mnemonic.alcg, RXYa)),
                (0x89, Instr(Mnemonic.slbg, RXYa)),
                (0x8E, Instr(Mnemonic.stpq, RXYa)),
                (0x8F, Instr(Mnemonic.lpq, RXYa)),
                (0x90, Instr(Mnemonic.llgc, RXYa)),
                (0x91, Instr(Mnemonic.llgh, RXYa)),
                (0x94, Instr(Mnemonic.llc, RXYa)),
                (0x95, Instr(Mnemonic.llh, RXYa)),
                (0x96, Instr(Mnemonic.ml, RXYa)),
                (0x97, Instr(Mnemonic.dl, RXYa)),
                (0x98, Instr(Mnemonic.alc, RXYa)),
                (0x99, Instr(Mnemonic.slb, RXYa)),
                (0x9C, Instr(Mnemonic.llgtat, RXYa)),
                (0x9D, Instr(Mnemonic.llgfat, RXYa)),
                (0x9F, Instr(Mnemonic.lat, RXYa)),
                (0xC0, Instr(Mnemonic.lbh, RXYa)),
                (0xC2, Instr(Mnemonic.llch, RXYa)),
                (0xC3, Instr(Mnemonic.stch, RXYa)),
                (0xC4, Instr(Mnemonic.lhh, RXYa)),
                (0xC6, Instr(Mnemonic.llhh, RXYa)),
                (0xC7, Instr(Mnemonic.sthh, RXYa)),
                (0xC8, Instr(Mnemonic.lfhat, RXYa)),
                (0xCA, Instr(Mnemonic.lfh, RXYa)),
                (0xCB, Instr(Mnemonic.stfh, RXYa)),
                (0xCD, Instr(Mnemonic.chf, RXYa)),
                (0xCF, Instr(Mnemonic.clhf, RXYa)));

                //(0x02, Instr(Mnemonic.ltg, RXYa)),
                //(0x04, Instr(Mnemonic.lg, RXYa)),
                //(0x12, Instr(Mnemonic.lt, RXYa)),
                //(0x14, Instr(Mnemonic.lgf, RXYa)),
                //(0x21, Instr(Mnemonic.clg, RXYa)),
                //(0x24, Instr(Mnemonic.stg, RXYa)),
                //(0x32, Instr(Mnemonic.ltgf, RXYa)),
                //(0x71, Instr(Mnemonic.lay, RXYa)),
                //(0x85, Instr(Mnemonic.lgat, RXYa)),
                //(0x9F, Instr(Mnemonic.lat, RXYa)));

            var e5_decoders = ExtendMask48(32, 8,
                (0x4C, Instr(Mnemonic.mvhi, SIL)));

            var e7_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x00, Instr(Mnemonic.vleb, VRX)),
                (0x01, Instr(Mnemonic.vleh, VRX)),
                (0x02, Instr(Mnemonic.vleg, VRX)),
                (0x03, Instr(Mnemonic.vlef, VRX)),
                (0x04, Instr(Mnemonic.vllez, VRX)),
                (0x05, Instr(Mnemonic.vlrep, VRX)),
                (0x06, Instr(Mnemonic.vl, VRX)),
                (0x07, Instr(Mnemonic.vlbb, VRX)),
                (0x08, Instr(Mnemonic.vsteb, VRX)),
                (0x09, Instr(Mnemonic.vsteh, VRX)),
                (0x0A, Instr(Mnemonic.vsteg, VRX)),
                (0x0B, Instr(Mnemonic.vstef, VRX)),
                (0x0E, Instr(Mnemonic.vst, VRX)),
                (0x12, Instr(Mnemonic.vgeg, VRV)),
                (0x13, Instr(Mnemonic.vgef, VRV)),
                (0x1A, Instr(Mnemonic.vsceg, VRV)),
                (0x1B, Instr(Mnemonic.vscef, VRV)),
                (0x21, Instr(Mnemonic.vlgv, VRSc)),
                (0x22, Instr(Mnemonic.vlvg, VRSb)),
                (0x27, Instr(Mnemonic.lcbb, RXE)),
                (0x30, Instr(Mnemonic.vesl, VRSa)),
                (0x33, Instr(Mnemonic.verll, VRSa)),
                (0x36, Instr(Mnemonic.vlm, VRSa)),
                (0x37, Instr(Mnemonic.vll, VRSb)),
                (0x38, Instr(Mnemonic.vesrl, VRSa)),
                (0x3A, Instr(Mnemonic.vesra, VRSa)),
                (0x3E, Instr(Mnemonic.vstm, VRSa)),
                (0x3F, Instr(Mnemonic.vstl, VRSb)),
                (0x40, Instr(Mnemonic.vleib, VRIa)),
                (0x41, Instr(Mnemonic.vleih, VRIa)),
                (0x42, Instr(Mnemonic.vleig, VRIa)),
                (0x43, Instr(Mnemonic.vleif, VRIa)),
                (0x44, Instr(Mnemonic.vgbm, VRIa)),
                (0x45, Instr(Mnemonic.vrepi, VRIa)),
                (0x46, Instr(Mnemonic.vgm, VRIb, Ubhfg)),
                (0x4A, Instr(Mnemonic.vftci, VRIe)),
                (0x4D, Instr(Mnemonic.vrep, VRIc)),
                (0x50, Instr(Mnemonic.vpopct, VRRa)),
                (0x52, Instr(Mnemonic.vctz, VRRa)),
                (0x53, Instr(Mnemonic.vclz, VRRa)),
                (0x56, Instr(Mnemonic.vlr, VRRa)),
                (0x5C, Instr(Mnemonic.vistr, VRRa)),
                (0x5F, Instr(Mnemonic.vseg, VRRa)),
                (0x60, Instr(Mnemonic.vmrl, VRRc, Ubhfg)),
                (0x61, Instr(Mnemonic.vmrh, VRRc, Ubhfg)),
                (0x62, Instr(Mnemonic.vlvgp, VRRf)),
                (0x64, Instr(Mnemonic.vsum, VRRc)),
                (0x65, Instr(Mnemonic.vsumg, VRRc)),
                (0x66, Instr(Mnemonic.vcksm, VRRc)),
                (0x67, Instr(Mnemonic.vsumq, VRRc)),
                (0x68, Instr(Mnemonic.vn, VRRc)),
                (0x69, Instr(Mnemonic.vnc, VRRc)),
                (0x6A, Instr(Mnemonic.vo, VRRc)),
                (0x6B, Instr(Mnemonic.vno, VRRc)),
                (0x6D, Instr(Mnemonic.vx, VRRc)),
                (0x70, Instr(Mnemonic.veslv, VRRc)),
                (0x72, Instr(Mnemonic.verim, VRId, Ubhfg)),
                (0x73, Instr(Mnemonic.verllv, VRRc)),
                (0x74, Instr(Mnemonic.vsl, VRRc)),
                (0x75, Instr(Mnemonic.vslb, VRRc)),
                (0x77, Instr(Mnemonic.vsldb, VRId)),
                (0x78, Instr(Mnemonic.vesrlv, VRRc)),
                (0x7A, Instr(Mnemonic.vesrav, VRRc)),
                (0x7C, Instr(Mnemonic.vsrl, VRRc)),
                (0x7D, Instr(Mnemonic.vsrlb, VRRc)),
                (0x7E, Instr(Mnemonic.vsra, VRRc)),
                (0x7F, Instr(Mnemonic.vsrab, VRRc)),
                (0x80, Instr(Mnemonic.vfee, VRRb)),
                (0x81, Instr(Mnemonic.vfene, VRRb)),
                (0x82, Instr(Mnemonic.vfae, VRRb)),
                (0x84, Instr(Mnemonic.vpdi, VRRc)),
                (0x8A, Instr(Mnemonic.vstrc, VRRd)),
                (0x8C, Instr(Mnemonic.vperm, VRRe)),
                (0x8D, Instr(Mnemonic.vsel, VRRe)),
                (0x8E, Instr(Mnemonic.vfms, VRRe)),
                (0x8F, Instr(Mnemonic.vfma, VRRe)),
                (0x94, Instr(Mnemonic.vpk, VRRc, S_hfg)),
                (0x95, Instr(Mnemonic.vpkls, VRRb, U_hfg, Scc)),
                (0x97, Instr(Mnemonic.vpks, VRRb, S_hfg, Scc)),
                (0xA1, Instr(Mnemonic.vmlh, VRRc)),
                (0xA2, Instr(Mnemonic.vml, VRRc)),
                (0xA3, Instr(Mnemonic.vmh, VRRc)),
                (0xA4, Instr(Mnemonic.vmle, VRRc)),
                (0xA5, Instr(Mnemonic.vmlo, VRRc)),
                (0xA6, Instr(Mnemonic.vme, VRRc)),
                (0xA7, Instr(Mnemonic.vmo, VRRc)),
                (0xA9, Instr(Mnemonic.vmalh, VRRd, Ubhf)),
                (0xAA, Instr(Mnemonic.vmal, VRRd, Ubhf)),
                (0xAB, Instr(Mnemonic.vmah, VRRd, Sbhf)),
                (0xAC, Instr(Mnemonic.vmale, VRRd)),
                (0xAD, Instr(Mnemonic.vmalo, VRRd)),
                (0xAE, Instr(Mnemonic.vmae, VRRd, Sbhf)),
                (0xAF, Instr(Mnemonic.vmao, VRRd, Sbhf)),
                (0xB4, Instr(Mnemonic.vgfm, VRRc)),
                (0xB9, Instr(Mnemonic.vaccc, VRRd)),
                (0xBB, Instr(Mnemonic.vac, VRRd)),
                (0xBC, Instr(Mnemonic.vgfma, VRRd)),
                (0xBD, Instr(Mnemonic.vsbcbi, VRRd)),
                (0xBF, Instr(Mnemonic.vsbi, VRRd)),
                (0xC0, Instr(Mnemonic.vclgd, VRRa)),
                (0xC1, Instr(Mnemonic.vcdlg, VRRa)),
                (0xC2, Instr(Mnemonic.vcgd, VRRa)),
                (0xC3, Instr(Mnemonic.vcdg, VRRa)),
                (0xC4, Instr(Mnemonic.vlde, VRRa)),
                (0xC5, Instr(Mnemonic.vled, VRRa)),
                (0xC7, Instr(Mnemonic.vfi, VRRa)),
                (0xCA, Instr(Mnemonic.wfk, VRRa)),
                (0xCB, Instr(Mnemonic.wfc, VRRa)),
                (0xCC, Instr(Mnemonic.vfpso, VRRa)),
                (0xCE, Instr(Mnemonic.vfsq, VRRa)),
                (0xD4, Instr(Mnemonic.vupll, VRRa)),
                (0xD5, Instr(Mnemonic.vuplh, VRRa)),
                (0xD6, Instr(Mnemonic.vupl, VRRa)),
                (0xD7, Instr(Mnemonic.vuph, VRRa)),
                (0xD8, Instr(Mnemonic.vtm, VRRa)),
                (0xD9, Instr(Mnemonic.vecl, VRRa)),
                (0xDB, Instr(Mnemonic.vec, VRRa)),
                (0xDE, Instr(Mnemonic.vlc, VRRa)),
                (0xDF, Instr(Mnemonic.vlp, VRRa)),
                (0xE2, Instr(Mnemonic.vfs, VRRc, Fg)),
                (0xE3, Instr(Mnemonic.vfa, VRRc)),
                (0xE5, Instr(Mnemonic.vfd, VRRc, Fg)),
                (0xE7, Instr(Mnemonic.vfm, VRRc)),
                (0xE8, Instr(Mnemonic.vfce, VRRc)),
                (0xEA, Instr(Mnemonic.vfche, VRRc)),
                (0xEB, Instr(Mnemonic.vfch, VRRc)),
                (0xF0, Instr(Mnemonic.vavgl, VRRc, Ubhfg)),
                (0xF1, Instr(Mnemonic.vacc, VRRc)),
                (0xF2, Instr(Mnemonic.vavg, VRRc, Sbhfg)),
                (0xF3, Instr(Mnemonic.va, VRRc)),
                (0xF5, Instr(Mnemonic.vscbi, VRRc)),
                (0xF7, Instr(Mnemonic.vs, VRRc)),
                (0xF8, Instr(Mnemonic.vceq, VRRb)),
                (0xF9, Instr(Mnemonic.vchl, VRRb)),
                (0xFB, Instr(Mnemonic.vch, VRRb)),
                (0xFC, Instr(Mnemonic.vmnl, VRRc, Ubhfg)),
                (0xFD, Instr(Mnemonic.vmxl, VRRc, Ubhfg)),
                (0xFE, Instr(Mnemonic.vmn, VRRc, Sbhfg)),
                (0xFF, Instr(Mnemonic.vmx, VRRc, Sbhfg)));

            var eb_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Mnemonic.lmg, RSYa)),
                (0x0A, Instr(Mnemonic.srag, RSYa)),
                (0x0B, Instr(Mnemonic.slag, RSYa)),
                (0x0C, Instr(Mnemonic.srlg, RSYa)),
                (0x0D, Instr(Mnemonic.sllg, RSYa)),
                (0x0F, Instr(Mnemonic.tracg, RSYa)),
                (0x14, Instr(Mnemonic.csy, RSYa)),
                (0x1C, Instr(Mnemonic.rllg, RSYa)),
                (0x1D, Instr(Mnemonic.rll, RSYa)),
                (0x20, Instr(Mnemonic.clmh, RSYb)),
                (0x21, Instr(Mnemonic.clmy, RSYb)),
                (0x23, Instr(Mnemonic.clt, RSYb)),
                (0x24, Instr(Mnemonic.stmg, RSYa)),
                (0x25, Instr(Mnemonic.stctg, RSYa)),
                (0x26, Instr(Mnemonic.stmh, RSYa)),
                (0x2B, Instr(Mnemonic.clgt, RSYb)),
                (0x2C, Instr(Mnemonic.stcmh, RSYb)),
                (0x2D, Instr(Mnemonic.stcmy, RSYb)),
                (0x2F, Instr(Mnemonic.lctlg, RSYa)),
                (0x30, Instr(Mnemonic.csg, RSYa)),
                (0x31, Instr(Mnemonic.cdsy, RSYa)),
                (0x3E, Instr(Mnemonic.cdsg, RSYa)),
                (0x44, Instr(Mnemonic.bxhg, RSYa)),
                (0x45, Instr(Mnemonic.bxleg, RSYa)),
                (0x4C, Instr(Mnemonic.ecag, RSYa)),
                (0x51, Instr(Mnemonic.tmy, SIY)),
                (0x52, Instr(Mnemonic.mviy, SIY)),
                (0x54, Instr(Mnemonic.niy, SIY)),
                (0x55, Instr(Mnemonic.cliy, SIY)),
                (0x56, Instr(Mnemonic.oiy, SIY)),
                (0x57, Instr(Mnemonic.xiy, SIY)),
                (0x6A, Instr(Mnemonic.asi, SIY)),
                (0x6E, Instr(Mnemonic.alsi, SIY)),
                (0x7A, Instr(Mnemonic.agsi, SIY)),
                (0x7E, Instr(Mnemonic.algsi, SIY)),
                (0x80, Instr(Mnemonic.icmh, RSYb)),
                (0x81, Instr(Mnemonic.icmy, RSYb)),
                (0x8E, Instr(Mnemonic.mvclu, RSYa)),
                (0x8F, Instr(Mnemonic.clclu, RSYa)),
                (0x90, Instr(Mnemonic.stmy, RSYa)),
                (0x96, Instr(Mnemonic.lmh, RSYa)),
                (0x98, Instr(Mnemonic.lmy, RSYa)),
                (0x9A, Instr(Mnemonic.lamy, RSYa)),
                (0x9B, Instr(Mnemonic.stamy, RSYa)),
                (0xC0, Instr(Mnemonic.tp, RSLa)),
                (0xDC, Instr(Mnemonic.srak, RSYa)),
                (0xDD, Instr(Mnemonic.slak, RSYa)),
                (0xDE, Instr(Mnemonic.srlk, RSYa)),
                (0xDF, Instr(Mnemonic.sllk, RSYa)),
                (0xE0, Instr(Mnemonic.locfh, RSYb)),
                (0xE1, Instr(Mnemonic.stocfh, RSYb)),
                (0xE2, Instr(Mnemonic.locg, InstrClass.Linear | InstrClass.Conditional, RSYb)),
                (0xE3, Instr(Mnemonic.stocg, InstrClass.Linear | InstrClass.Conditional, RSYb)),
                (0xE4, Instr(Mnemonic.lang, RSYa)),
                (0xE6, Instr(Mnemonic.laog, RSYa)),
                (0xE7, Instr(Mnemonic.laxg, RSYa)),
                (0xE8, Instr(Mnemonic.laag, RSYa)),
                (0xEA, Instr(Mnemonic.laalg, RSYa)),
                (0xF2, Instr(Mnemonic.loc, InstrClass.Linear | InstrClass.Conditional, RSYb)),
                (0xF3, Instr(Mnemonic.stoc, InstrClass.Linear | InstrClass.Conditional, RSYb)),
                (0xF4, Instr(Mnemonic.lan, RSYa)),
                (0xF6, Instr(Mnemonic.lao, RSYa)),
                (0xF7, Instr(Mnemonic.lax, RSYa)),
                (0xF8, Instr(Mnemonic.laa, RSYa)),
                (0xFA, Instr(Mnemonic.laal, RSYa)));

            var ec_decoders = ExtendMask48(0, 8,
                (0x42, Instr(Mnemonic.lochi, RIEg)),
                (0x44, Instr(Mnemonic.brxhg, InstrClass.ConditionalTransfer, RIEe)),
                (0x45, Instr(Mnemonic.brxlg, InstrClass.ConditionalTransfer, RIEe)),
                (0x46, Instr(Mnemonic.locghi, RIEg)),
                (0x4E, Instr(Mnemonic.lochhi, RIEg)),
                (0x51, Instr(Mnemonic.risblg, RIEf)),
                (0x54, Instr(Mnemonic.rnsbg, RIEf)),
                (0x55, Instr(Mnemonic.risbg, RIEf)),
                (0x56, Instr(Mnemonic.rosbg, RIEf)),
                (0x57, Instr(Mnemonic.rxsbg, RIEf)),
                (0x59, Instr(Mnemonic.risbgn, RIEf)),
                (0x5D, Instr(Mnemonic.risbhg, RIEf)),
                (0x64, Instr(Mnemonic.cgrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x65, Instr(Mnemonic.clgrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x70, Instr(Mnemonic.cgit, RIEa)),
                (0x71, Instr(Mnemonic.clgit, RIEa)),
                (0x72, Instr(Mnemonic.cit, RIEa)),
                (0x73, Instr(Mnemonic.clfit, RIEa)),
                (0x76, Instr(Mnemonic.crj, InstrClass.ConditionalTransfer, RIEb)),
                (0x77, Instr(Mnemonic.clrj, InstrClass.ConditionalTransfer, RIEb)),
                (0x7C, Instr(Mnemonic.cgij, InstrClass.ConditionalTransfer, RIEc)),
                (0x7D, Instr(Mnemonic.clgij, InstrClass.ConditionalTransfer, RIEc)),
                (0x7E, Instr(Mnemonic.cij, InstrClass.ConditionalTransfer, RIEc)),
                (0x7F, Instr(Mnemonic.clij, InstrClass.ConditionalTransfer, RIEc)),
                (0xD8, Instr(Mnemonic.ahik, RIEd)),
                (0xD9, Instr(Mnemonic.aghik, RIEd)),
                (0xDA, Instr(Mnemonic.alhsik, RIEd)),
                (0xDB, Instr(Mnemonic.alghsik, RIEd)),
                (0xE4, Instr(Mnemonic.cgrb, RRS)),
                (0xE5, Instr(Mnemonic.clgrb, RRS)),
                (0xF6, Instr(Mnemonic.crb, RRS)),
                (0xF7, Instr(Mnemonic.clrb, RRS)),
                (0xFC, Instr(Mnemonic.cgib, RIS)),
                (0xFD, Instr(Mnemonic.clgib, RIS)),
                (0xFE, Instr(Mnemonic.cib, RIS)),
                (0xFF, Instr(Mnemonic.clib, RIS)));

            var ed_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Mnemonic.ldeb, RXE)),
                (0x05, Instr(Mnemonic.lxdb, RXE)),
                (0x06, Instr(Mnemonic.lxeb, RXE)),
                (0x07, Instr(Mnemonic.mxdb, RXE)),
                (0x08, Instr(Mnemonic.keb, RXE)),
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
                (0x14, Instr(Mnemonic.sqeb, RXE)),
                (0x15, Instr(Mnemonic.sqdb, RXE)),
                (0x17, Instr(Mnemonic.meeb, RXE)),
                (0x18, Instr(Mnemonic.kdb, RXE)),
                (0x19, Instr(Mnemonic.cdb, RXE)),
                (0x1A, Instr(Mnemonic.adb, RXE)),
                (0x1B, Instr(Mnemonic.sdb, RXE)),
                (0x1C, Instr(Mnemonic.mdb, RXE)),
                (0x1D, Instr(Mnemonic.ddb, RXE)),
                (0x1E, Instr(Mnemonic.madb, RXF)),
                (0x1F, Instr(Mnemonic.msdb, RXF)),
                (0x24, Instr(Mnemonic.lde, RXE)),
                (0x25, Instr(Mnemonic.lxd, RXE)),
                (0x26, Instr(Mnemonic.lxe, RXE)),
                (0x2E, Instr(Mnemonic.mae, RXF)),
                (0x2F, Instr(Mnemonic.mse, RXF)),
                (0x34, Instr(Mnemonic.sqe, RXE)),
                (0x35, Instr(Mnemonic.sqd, RXE)),
                (0x37, Instr(Mnemonic.mee, RXE)),
                (0x38, Instr(Mnemonic.mayl, RXF)),
                (0x39, Instr(Mnemonic.myl, RXF)),
                (0x3A, Instr(Mnemonic.may, RXF)),
                (0x3B, Instr(Mnemonic.my, RXF)),
                (0x3C, Instr(Mnemonic.mayh, RXF)),
                (0x3D, Instr(Mnemonic.myh, RXF)),
                (0x3E, Instr(Mnemonic.mad, RXF)),
                (0x3F, Instr(Mnemonic.msd, RXF)),
                (0x40, Instr(Mnemonic.sldt, RXF)),
                (0x41, Instr(Mnemonic.srdt, RXF)),
                (0x48, Instr(Mnemonic.slxt, RXF)),
                (0x49, Instr(Mnemonic.srxt, RXF)),
                (0x50, Instr(Mnemonic.tdcet, RXE)),
                (0x51, Instr(Mnemonic.tdget, RXE)),
                (0x54, Instr(Mnemonic.tdcdt, RXE)),
                (0x55, Instr(Mnemonic.tdgdt, RXE)),
                (0x58, Instr(Mnemonic.tdcxt, RXE)),
                (0x59, Instr(Mnemonic.tdgxt, RXE)),
                (0x64, Instr(Mnemonic.ley, FXYa)),
                (0x65, Instr(Mnemonic.ldy, FXYa)),
                (0x66, Instr(Mnemonic.stey, FXYa)),
                (0x67, Instr(Mnemonic.stdy, FXYa)),
                (0xA8, Instr(Mnemonic.czdt, RSL)),
                (0xA9, Instr(Mnemonic.czxt, RSL)),
                (0xAA, Instr(Mnemonic.cdzt, RSL)),
                (0xAB, Instr(Mnemonic.cxzt, RSL)),
                (0xAC, Instr(Mnemonic.cpdt, RSLb)),
                (0xAD, Instr(Mnemonic.cpxt, RSLb)),
                (0xAE, Instr(Mnemonic.cdpt, RSLb)),
                (0xAF, Instr(Mnemonic.cxpt, RSLb)));



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

                Instr(Mnemonic.ler, FF),
                Instr(Mnemonic.cer, FF),
                Instr(Mnemonic.aer, FF),
                Instr(Mnemonic.ser, FF),

                Instr(Mnemonic.medr, FF),
                Instr(Mnemonic.der, FF),
                Instr(Mnemonic.aur, FF),
                Instr(Mnemonic.sur, FF),

                // 40
                Instr32(Mnemonic.sth, RXa),
                Instr32(Mnemonic.la, RXa),
                Instr32(Mnemonic.stc, RXa),
                Instr32(Mnemonic.ic, RXa),

                Instr32(Mnemonic.ex, RXa),
                Instr32(Mnemonic.bal, InstrClass.Call|InstrClass.Transfer, RXa),
                Instr32(Mnemonic.bct, RXa),
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
                    Instr(Mnemonic.b, InstrClass.Transfer, RXb)),

                Instr32(Mnemonic.lh, RXa),
                Instr32(Mnemonic.ch, RXa),
                Instr32(Mnemonic.ah, RXa),
                Instr32(Mnemonic.sh, RXa),

                Instr32(Mnemonic.mh, RXa),
                Instr32(Mnemonic.bas, InstrClass.Transfer|InstrClass.Call, RXa),
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
                Instr32(Mnemonic.aw, FXa),
                Instr32(Mnemonic.sw, FXa),

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
                Instr32(Mnemonic.ce, FXa),
                Instr32(Mnemonic.ae, FXa),
                Instr32(Mnemonic.se, FXa),

                Instr32(Mnemonic.mde, FXa),
                Instr32(Mnemonic.de, FXa),
                Instr32(Mnemonic.au, FXa),
                Instr32(Mnemonic.su, FXa),

                // 80
                Instr32(Mnemonic.ssm, S),
                invalid,
                Instr32(Mnemonic.lpsw, InstrClass.Privileged|InstrClass.Linear, S),
                Instr32(Mnemonic.diag, InstrClass.Privileged|InstrClass.Linear),

                Instr32(Mnemonic.brxh, InstrClass.ConditionalTransfer, RSI),
                Instr32(Mnemonic.brxle, InstrClass.ConditionalTransfer, RSI),
                Instr32(Mnemonic.bxh, InstrClass.ConditionalTransfer, RSar),
                Instr32(Mnemonic.bxle, InstrClass.ConditionalTransfer, RSar),

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
                    (0x04, Instr(Mnemonic.nihh, RIa)),
                    (0x05, Instr(Mnemonic.nihl, RIa)),
                    (0x06, Instr(Mnemonic.nilh, RIa)),
                    (0x07, Instr(Mnemonic.nill, RIa)),
                    (0x08, Instr(Mnemonic.oihh, RIa )),
                    (0x09, Instr(Mnemonic.oihl, RIa)),
                    (0x0A, Instr(Mnemonic.oilh, RIa)),
                    (0x0B, Instr(Mnemonic.oill, RIa)),
                    (0x0C, Instr(Mnemonic.llihh, RIa)),
                    (0x0D, Instr(Mnemonic.llihl, RIa)),
                    (0x0E, Instr(Mnemonic.llilh, RIa)),
                    (0x0F, Instr(Mnemonic. llill, RIa))),

                invalid,
                ExtendMask32(16, 4,
                    (0x0, Instr(Mnemonic.tmlh, RIa)),
                    (0x1, Instr(Mnemonic.tmll, RIa)),
                    (0x2, Instr(Mnemonic.tmhh, RIa)),
                    (0x3, Instr(Mnemonic.tmhl, RIa)),
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
                    (0xC, Instr(Mnemonic.mhi, RIa)),
                    (0xD, Instr(Mnemonic.mghi, RIa)),
                    (0xE, Instr(Mnemonic.chi, RIa)),
                    (0xF, Instr(Mnemonic.cghi, RIa))),

                Instr32(Mnemonic.mvcle, RSa3),
                Instr32(Mnemonic.clcle, RSa3),
                Instr32(Mnemonic.unpka, SSa),
                invalid,

                Instr32(Mnemonic.stnsm, SI),
                Instr32(Mnemonic.stosm, SI),
                Instr32(Mnemonic.sigp, InstrClass.Linear | InstrClass.Privileged, RSa),
                Instr32(Mnemonic.mc, SI),

                // B0
                invalid,
                Instr32(Mnemonic.lra, InstrClass.Linear | InstrClass.Privileged, RXa),
                b2_decoders,
                b3_decoders,

                invalid,
                invalid,
                Instr32(Mnemonic.stctl, InstrClass.Linear | InstrClass.Privileged,  RSa),
                Instr32(Mnemonic.lctl, InstrClass.Linear | InstrClass.Privileged, RSa),

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
                invalid,
                c2_decoders,
                invalid,

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

                ExtendMask48(32, 4,
                    (0x0, Instr(Mnemonic.mvcos, SSF)),
                    (0x1, Instr(Mnemonic.ectg, SSF)),
                    (0x2, Instr(Mnemonic.csst, SSF)),
                    (0x4, Instr(Mnemonic.lpd, SSF)),
                    (0x5, Instr(Mnemonic.lpdg, SSF))),
                invalid,
                invalid,
                invalid,

                cc_decoders,
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

                invalid,
                Instr48(Mnemonic.mvck, SSd),
                Instr48(Mnemonic.mvcp, InstrClass.Linear|InstrClass.Privileged, SSd),
                Instr48(Mnemonic.mvcs, InstrClass.Linear|InstrClass.Privileged, SSd),

                Instr48(Mnemonic.tr, SSa),
                Instr48(Mnemonic.trt, SSa),
                Instr48(Mnemonic.ed, SSa),
                Instr48(Mnemonic.edmk, SSa),

                // E0
                invalid,
                Instr48(Mnemonic.pku, SSf),
                Instr48(Mnemonic.unpku, SSa),
                e3_decoders,

                invalid,
                e5_decoders,
                invalid,
                e7_decoders,

                Instr48(Mnemonic.mvcin, SSa),
                Instr48(Mnemonic.pka, SSf),
                Instr48(Mnemonic.unpka, SSa),
                eb_decoders,

                ec_decoders,
                ed_decoders,
                Instr48(Mnemonic.plo, SSe),
                Instr48(Mnemonic.lmd, SSe),

                // F0
                Instr48(Mnemonic.srp, SSc),
                Instr48(Mnemonic.mvo, SSb),
                Instr48(Mnemonic.pack, SSb),
                Instr48(Mnemonic.unpk, SSb),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr48(Mnemonic.zap, SSb),
                Instr48(Mnemonic.cp, SSb),
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