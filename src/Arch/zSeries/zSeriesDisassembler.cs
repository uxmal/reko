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

        public override zSeriesInstruction NotYetImplemented(uint wInstr, string message)
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

        public static bool S(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.ops.Add(dasm.CreateAccess(b2, d2));
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
                return dasm.NotYetImplemented(0, msg);
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

        public static ExtendDecoder48 Instr48(Mnemonic mnemonic, params WideMutator<zSeriesDisassembler>[] mutators)
        {
            return new ExtendDecoder48(InstrClass.Linear, mnemonic, mutators);
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

            var e3_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Mnemonic.lg, RXYa)),
                (0x14, Instr(Mnemonic.lgf, RXYa)),
                (0x21, Instr(Mnemonic.clg, RXYa)),
                (0x24, Instr(Mnemonic.stg, RXYa)));

            var e5_decoders = ExtendMask48(0, 8);

            var eb_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x04, Instr(Mnemonic.lmg, RSYa)),
                (0x0A, Instr(Mnemonic.srag, RSYa)),
                (0x0B, Instr(Mnemonic.slag, RSYa)),
                (0x0C, Instr(Mnemonic.srlg, RSYa)),
                (0x0D, Instr(Mnemonic.sllg, RSYa)),
                (0x20, Instr(Mnemonic.clmh, RSYb)),
                (0x24, Instr(Mnemonic.stmg, RSYa)));

            decoders = new Decoder[256]
            {
                // 00
                invalid,
                n01_decoders,
                invalid,
                invalid,

                Instr(Mnemonic.spm, RR),
                Instr(Mnemonic.balr, RR),
                Instr(Mnemonic.bctr, RR),
                Mask(4, 4,
                    Instr(Mnemonic.nopr, R),
                    Instr(Mnemonic.bor, R),
                    Instr(Mnemonic.bhr, R),
                    Instr(Mnemonic.bnler, R),
                    Instr(Mnemonic.blr, R),
                    Instr(Mnemonic.bnher, R),
                    Instr(Mnemonic.blhr, R),
                    Instr(Mnemonic.bner, R),
                    Instr(Mnemonic.ber, R),
                    Instr(Mnemonic.bnlhr, R),
                    Instr(Mnemonic.bher, R),
                    Instr(Mnemonic.bnlr, R),
                    Instr(Mnemonic.bler, R),
                    Instr(Mnemonic.bnhr, R),
                    Instr(Mnemonic.bnor, R),
                    Instr(Mnemonic.br, R)),
                invalid,
                invalid,
                Nyi("*"),
                Instr(Mnemonic.bsm, RR),

                Instr(Mnemonic.bassm, RR),
                Instr(Mnemonic.basr, RR),
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
                Instr(Mnemonic.lpdr, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Mnemonic.ldr, FF),
                Instr(Mnemonic.cdr, FF),
                Instr(Mnemonic.adr, FF),
                Instr(Mnemonic.sdr, FF),

                Instr(Mnemonic.mdr, FF),
                Instr(Mnemonic.ddr, FF),
                Instr(Mnemonic.awr, FF),
                Instr(Mnemonic.swr, FF),
                // 30
                Instr(Mnemonic.lper, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Mnemonic.her, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 40
                Instr32(Mnemonic.sth, RXa),
                Instr32(Mnemonic.la, RXa),
                Instr32(Mnemonic.stc, RXa),
                Instr32(Mnemonic.ic, RXa),

                Instr32(Mnemonic.ex, RXa),
                Nyi("*"),
                Nyi("*"),
                ExtendMask32(20, 4,
                    Instr(Mnemonic.nop),
                    Instr(Mnemonic.bo, RXb),
                    Instr(Mnemonic.bh, RXb),
                    Instr(Mnemonic.bnle, RXb),
                    Instr(Mnemonic.bl, RXb),
                    Instr(Mnemonic.bnhe, RXb),
                    Instr(Mnemonic.blh, RXb),
                    Instr(Mnemonic.bne, RXb),
                    Instr(Mnemonic.be, RXb),
                    Instr(Mnemonic.bnlh, RXb),
                    Instr(Mnemonic.bhe, RXb),
                    Instr(Mnemonic.bnl, RXb),
                    Instr(Mnemonic.ble, RXb),
                    Instr(Mnemonic.bnh, RXb),
                    Instr(Mnemonic.bno, RXb),
                    Instr(Mnemonic.b, RXb),
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
                Nyi("*"),
                Nyi("*"),
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
                Instr32(Mnemonic.ms, RXa),
                invalid,
                invalid,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr32(Mnemonic.ae, FXa),
                Instr32(Mnemonic.se, FXa),

                Nyi("*"),
                Nyi("*"),
                Instr32(Mnemonic.au, FXa),
                Instr32(Mnemonic.su, FXa),
                // 80
                Instr32(Mnemonic.ssm, S),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr32(Mnemonic.sra, RSa),
                Nyi("*"),

                Instr32(Mnemonic.srl, RSa),
                Instr32(Mnemonic.sll, RSa),
                Instr32(Mnemonic.sra, RSa),
                Instr32(Mnemonic.sla, RSa),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 90
                Instr32(Mnemonic.stm, RSa3),
                Nyi("*"),
                Instr32(Mnemonic.mvi, SI),
                Nyi("*"),

                Nyi("*"),
                Instr32(Mnemonic.cli, SI),
                Instr32(Mnemonic.oi, SI),
                Instr32(Mnemonic.xi, SI),

                Instr32(Mnemonic.lm, RSa3),
                Nyi("*"),
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
                        Instr(Mnemonic.brc, RIc),
                        Instr(Mnemonic.jo, RIc),
                        Instr(Mnemonic.jh, RIc),
                        Instr(Mnemonic.jnle, RIc),
                        Instr(Mnemonic.jl, RIc),
                        Instr(Mnemonic.jnhe, RIc),
                        Instr(Mnemonic.jlh, RIc),
                        Instr(Mnemonic.jne, RIc),
                        Instr(Mnemonic.je, RIc),
                        Instr(Mnemonic.jnlh, RIc),
                        Instr(Mnemonic.jhe, RIc),
                        Instr(Mnemonic.jnl, RIc),
                        Instr(Mnemonic.jle, RIc),
                        Instr(Mnemonic.jnh, RIc),
                        Instr(Mnemonic.jno, RIc),
                        Instr(Mnemonic.j, RIc))),
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
                Nyi("*"),
                Nyi("*"),
                // B0
                invalid,
                Instr32(Mnemonic.lra, RXa),
                b2_decoders,
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                ExtendMask32(16, 8,
                    (0x02, Instr(Mnemonic.ltgr, RRE)),
                    (0x04, Instr(Mnemonic.lgr, RRE)),
                    (0x08, Instr(Mnemonic.agr, RRE)),
                    (0x09, Instr(Mnemonic.sgr, RRE)),
                    (0x12, Instr(Mnemonic.ltgfr, RRE)),
                    (0x14, Instr(Mnemonic.lgfr, RRE)),
                    (0x80, Instr(Mnemonic.ngr, RRE))),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Instr32(Mnemonic.icm, RSb),
                // C0
                ExtendMask48(32, 4,
                    (0x0, Instr(Mnemonic.larl, RILb)),
                    (0x4, Mask(36, 4,
                        Instr(Mnemonic.brcl, RILc),
                        Instr(Mnemonic.jgo, RILc),
                        Instr(Mnemonic.jgh, RILc),
                        Instr(Mnemonic.jgnle, RILc),
                        Instr(Mnemonic.jgl, RILc),
                        Instr(Mnemonic.jgnhe, RILc),
                        Instr(Mnemonic.jglh, RILc),
                        Instr(Mnemonic.jgne, RILc),
                        Instr(Mnemonic.jge, RILc),
                        Instr(Mnemonic.jgnlh, RILc),
                        Instr(Mnemonic.jghe, RILc),
                        Instr(Mnemonic.jgnl, RILc),
                        Instr(Mnemonic.jgle, RILc),
                        Instr(Mnemonic.jgnh, RILc),
                        Instr(Mnemonic.jgno, RILc),
                        Instr(Mnemonic.jg, RILc))),
                    (0x5, Instr(Mnemonic.brasl, RILb))),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Instr48(Mnemonic.bprp, MII),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                invalid,
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
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
                Nyi("*"),
                Instr48(Mnemonic.mvcs, SSd),

                Nyi("*"),
                Nyi("*"),
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

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // F0
                Instr48(Mnemonic.srp, SSc),
                Instr48(Mnemonic.mvo, SSb),
                Nyi("*"),
                Nyi("*"),

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