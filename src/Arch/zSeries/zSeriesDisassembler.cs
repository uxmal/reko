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

namespace Reko.Arch.zSeries
{
    using Mutator = Func<ulong, zSeriesDisassembler, bool>;

    public class zSeriesDisassembler : DisassemblerBase<zSeriesInstruction>
    {
        private readonly static Decoder[] decoders;
        private readonly static Decoder invalid;

        private readonly zSeriesArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly State state;
        private Address addr;

        public zSeriesDisassembler(zSeriesArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = new State();
        }

        public override zSeriesInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out var opcode))
                return null;
            var instr = decoders[opcode >> 8].Decode(opcode, this);
            state.Reset();
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            instr.InstructionClass |= opcode == 0 ? InstrClass.Zero : 0;
            return instr;
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

        private zSeriesInstruction Invalid()
        {
            return new zSeriesInstruction
            {
                Opcode = Opcode.invalid,
                Ops = new MachineOperand[0]
            };
        }

        private void Nyi(ulong uInstr, string message)
        {
            var rdr2 = rdr.Clone();
            int len = (int) (rdr.Address - this.addr);
            rdr2.Offset -= len;
            var bytes = rdr2.ReadBytes(len);
            var leHexBytes = string.Join("", bytes
                    .Select(b => b.ToString("X2")));
            var instrHexBytes = $"{uInstr:X8}";
            base.EmitUnitTest("zSeries", instrHexBytes, message, "zSerDasm", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"{0}\");", leHexBytes);
            });
        }

        private class State
        {
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Opcode opcode;

            public void Reset()
            {
                this.opcode = Opcode.invalid;
                this.ops.Clear();
            }

            public zSeriesInstruction MakeInstruction()
            {
                var instr = new zSeriesInstruction
                {
                    Opcode = this.opcode,
                    Ops = this.ops.ToArray(),
                };
                return instr;
            }
        }

        #region Mutators

        public static bool FF(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.FpRegisters[(uInstr >> 4) & 0xF]));
            dasm.state.ops.Add(new RegisterOperand(Registers.FpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool FXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var f1 = new RegisterOperand(Registers.FpRegisters[(uInstr >> 20) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(f1);
            dasm.state.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool MII(ulong uInstr, zSeriesDisassembler dasm)
        {
            var m1 = (byte)((uInstr >> 36) & 0xF);
            var r2 = (int) Bits.SignExtend((uint)(uInstr >> 24), 12);
            var r3 = (int) Bits.SignExtend((uint)uInstr, 24);
            dasm.state.ops.Add(ImmediateOperand.Byte(m1));
            dasm.state.ops.Add(ImmediateOperand.Int32(r2));
            dasm.state.ops.Add(ImmediateOperand.Int32(r3));
            return true;
        }

        public static bool R(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[uInstr & 0xF]));
            return true;
        }

        public static bool RR(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 4) & 0xF]));
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool RRE(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 4) & 0xF]));
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr) & 0xF]));
            return true;
        }

        public static bool RIa(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]));
            dasm.state.ops.Add(ImmediateOperand.Int32((int)Bits.SignExtend(uInstr, 16)));
            return true;
        }

        public static bool RIb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2 * (int)Bits.SignExtend(uInstr, 16);
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]));
            dasm.state.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        public static bool RIc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var addr = dasm.addr + 2*(int)Bits.SignExtend(uInstr, 16);
            dasm.state.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        public static bool RILb(ulong uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 36) & 0xF]));
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.state.ops.Add(AddressOperand.Create(dasm.addr + offset));
            return true;
        }

        public static bool RILc(ulong uInstr, zSeriesDisassembler dasm)
        {
            var offset = 2 * (int)Bits.SignExtend(uInstr, 32);
            dasm.state.ops.Add(AddressOperand.Create(dasm.addr + offset));
            return true;
        }

        public static bool RXa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = new RegisterOperand(Registers.GpRegisters[(uInstr >> 20) & 0xF]);
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(r1);
            dasm.state.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool RXb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var x2 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        public static bool S(ulong uInstr, zSeriesDisassembler dasm)
        {
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool SSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l = (byte)(uInstr >> 32);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(dasm.CreateAccessLength(b1, d1, l + 1));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
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
            dasm.state.ops.Add(dasm.CreateAccessLength(b1, d1, l1+ 1));
            dasm.state.ops.Add(dasm.CreateAccessLength(b2, d2, l2+1));
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
            dasm.state.ops.Add(dasm.CreateAccessLength(b1, d1, l + 1));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.state.ops.Add(ImmediateOperand.Byte(i3));
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
            dasm.state.ops.Add(dasm.CreateAccess(b1, r1, d1));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.state.ops.Add(new RegisterOperand(r3));
            return true;
        }

        public static bool SSf(ulong uInstr, zSeriesDisassembler dasm)
        {
            var l2 = (byte)(uInstr >> 32);
            var b1 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr >> 16, 12);
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.state.ops.Add(dasm.CreateAccessLength(b2, d2, l2 + 1));
            return true;
        }

        public static bool RSa(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(new RegisterOperand(r1));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSa3(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var r3 = Registers.GpRegisters[(uInstr >> 16) & 0xF];
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(new RegisterOperand(r1));
            dasm.state.ops.Add(new RegisterOperand(r3));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        public static bool RSb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 20) & 0xF];
            var m3 = ImmediateOperand.Byte((byte)((uInstr >> 16) & 0xF));
            var b2 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d2 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(new RegisterOperand(r1));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            dasm.state.ops.Add(m3);
            return true;
        }

        private static Bitfield[] rsya_offset = new[]
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
            dasm.state.ops.Add(new RegisterOperand(r1));
            dasm.state.ops.Add(new RegisterOperand(r3));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static bool RSYb(ulong uInstr, zSeriesDisassembler dasm)
        {
            var r1 = Registers.GpRegisters[(uInstr >> 36) & 0xF];
            var m3 = (byte)((uInstr >> 32) & 0xF);
            var b2 = Registers.GpRegisters[(uInstr >> 28) & 0xF];
            var d2 = Bitfield.ReadSignedFields(rsya_offset, (uint)uInstr);
            dasm.state.ops.Add(new RegisterOperand(r1));
            dasm.state.ops.Add(ImmediateOperand.Byte(m3));
            dasm.state.ops.Add(dasm.CreateAccess(b2, d2));
            return true;
        }

        private static Bitfield[] rxya_offset = new[]
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
            dasm.state.ops.Add(r1);
            dasm.state.ops.Add(dasm.CreateAccess(b2, x2, d2));
            return true;
        }

        private static bool SI(ulong uInstr, zSeriesDisassembler dasm)
        {
            var i2 = ImmediateOperand.Byte((byte)(uInstr >> 16));
            var b1 = Registers.GpRegisters[(uInstr >> 12) & 0xF];
            var d1 = (int)Bits.SignExtend(uInstr, 12);
            dasm.state.ops.Add(dasm.CreateAccess(b1, d1));
            dasm.state.ops.Add(i2);
            return true;
        }

        #endregion

        public abstract class Decoder
        {
            public abstract zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator[] mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                dasm.state.opcode = opcode;
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.Invalid();
                }
                return dasm.state.MakeInstruction();
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string msg;

            public NyiDecoder(string msg = "")
            {
                this.msg = msg;
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                dasm.Nyi(uInstr, msg);
                return dasm.Invalid();
            }
        }

        public class ExtendDecoder32 : InstrDecoder
        {
            public ExtendDecoder32(Opcode opcode, params Mutator [] mutators) : base(opcode, mutators)
            {
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uLowWord))
                    return dasm.Invalid();
                ulong uInstrExt = (uInstr << 16) | uLowWord;
                return base.Decode(uInstrExt, dasm);
            }
        }

        public class ExtendDecoder48 : InstrDecoder
        {
            public ExtendDecoder48(Opcode opcode, params Mutator[] mutators) : base(opcode, mutators)
            {
            }

            public override zSeriesInstruction Decode(ulong uInstr, zSeriesDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt32(out uint uLowWord))
                    return dasm.Invalid();
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
                    return dasm.Invalid();
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
                    return dasm.Invalid();
                ulong uInstrExt = (uInstr << 32) | uLowWord;
                var op = this.bitfield.Read(uInstrExt);
                return this.decoders[op].Decode(uInstrExt, dasm);
            }
        }

        public static InstrDecoder Instr(Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(opcode, mutators);
        }

        public static ExtendDecoder32 Instr32(Opcode opcode, params Mutator[] mutators)
        {
            return new ExtendDecoder32(opcode, mutators);
        }

        public static ExtendDecoder48 Instr48(Opcode opcode, params Mutator[] mutators)
        {
            return new ExtendDecoder48(opcode, mutators);
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
            invalid = Instr(Opcode.invalid);

            var n01_decoders = Mask(0, 8,
                (0x01, Instr(Opcode.pr)));

            var b2_decoders = ExtendMask32(0, 8,
                (0x04, Instr(Opcode.sck, S))
                );

            var e3_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Opcode.lg, RXYa)),
                (0x14, Instr(Opcode.lgf, RXYa)),
                (0x21, Instr(Opcode.clg, RXYa)),
                (0x24, Instr(Opcode.stg, RXYa)));

            var e5_decoders = ExtendMask48(0, 8);

            var eb_decoders = ExtendMask48(0, 8,
                (~0u, Nyi("*")),
                (0x04, Instr(Opcode.lmg, RSYa)),
                (0x0A, Instr(Opcode.srag, RSYa)),
                (0x0B, Instr(Opcode.slag, RSYa)),
                (0x0C, Instr(Opcode.srlg, RSYa)),
                (0x0D, Instr(Opcode.sllg, RSYa)),
                (0x20, Instr(Opcode.clmh, RSYb)),
                (0x24, Instr(Opcode.stmg, RSYa)));

            decoders = new Decoder[256]
            {
                // 00
                invalid,
                n01_decoders,
                invalid,
                invalid,

                Instr(Opcode.spm, RR),
                Instr(Opcode.balr, RR),
                Instr(Opcode.bctr, RR),
                Mask(4, 4,
                    Instr(Opcode.nopr, R),
                    Instr(Opcode.bor, R),
                    Instr(Opcode.bhr, R),
                    Instr(Opcode.bnler, R),
                    Instr(Opcode.blr, R),
                    Instr(Opcode.bnher, R),
                    Instr(Opcode.blhr, R),
                    Instr(Opcode.bner, R),
                    Instr(Opcode.ber, R),
                    Instr(Opcode.bnlhr, R),
                    Instr(Opcode.bher, R),
                    Instr(Opcode.bnlr, R),
                    Instr(Opcode.bler, R),
                    Instr(Opcode.bnhr, R),
                    Instr(Opcode.bnor, R),
                    Instr(Opcode.br, R)),
                invalid,
                invalid,
                Nyi("*"),
                Instr(Opcode.bsm, RR),

                Instr(Opcode.bassm, RR),
                Instr(Opcode.basr, RR),
                Instr(Opcode.mvcl, RR),
                Instr(Opcode.clcl, RR),
                // 10
                Instr(Opcode.lpr, RR),
                Instr(Opcode.lnr, RR),
                Instr(Opcode.ltr, RR),
                Instr(Opcode.lcr, RR),

                Instr(Opcode.nr, RR),
                Instr(Opcode.clr, RR),
                Instr(Opcode.or, RR),
                Instr(Opcode.xr, RR),

                Instr(Opcode.lr, RR),
                Instr(Opcode.cr, RR),
                Instr(Opcode.ar, RR),
                Instr(Opcode.sr, RR),

                Instr(Opcode.mr, RR),
                Instr(Opcode.dr, RR),
                Instr(Opcode.alr, RR),
                Instr(Opcode.slr, RR),
                // 20
                Instr(Opcode.lpdr, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.ldr, FF),
                Instr(Opcode.cdr, FF),
                Instr(Opcode.adr, FF),
                Instr(Opcode.sdr, FF),

                Instr(Opcode.mdr, FF),
                Instr(Opcode.ddr, FF),
                Instr(Opcode.awr, FF),
                Instr(Opcode.swr, FF),
                // 30
                Instr(Opcode.lper, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.her, RR),
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
                Instr32(Opcode.sth, RXa),
                Instr32(Opcode.la, RXa),
                Instr32(Opcode.stc, RXa),
                Instr32(Opcode.ic, RXa),

                Instr32(Opcode.ex, RXa),
                Nyi("*"),
                Nyi("*"),
                ExtendMask32(20, 4,
                    Instr(Opcode.nop),
                    Instr(Opcode.bo, RXb),
                    Instr(Opcode.bh, RXb),
                    Instr(Opcode.bnle, RXb),
                    Instr(Opcode.bl, RXb),
                    Instr(Opcode.bnhe, RXb),
                    Instr(Opcode.blh, RXb),
                    Instr(Opcode.bne, RXb),
                    Instr(Opcode.be, RXb),
                    Instr(Opcode.bnlh, RXb),
                    Instr(Opcode.bhe, RXb),
                    Instr(Opcode.bnl, RXb),
                    Instr(Opcode.ble, RXb),
                    Instr(Opcode.bnh, RXb),
                    Instr(Opcode.bno, RXb),
                    Instr(Opcode.b, RXb),
                    Nyi("*")),

                Instr32(Opcode.lh, RXa),
                Instr32(Opcode.ch, RXa),
                Instr32(Opcode.ah, RXa),
                Instr32(Opcode.sh, RXa),

                Instr32(Opcode.mh, RXa),
                Instr32(Opcode.bas, RXa),
                Instr32(Opcode.cvd, RXa),
                Instr32(Opcode.cvb, RXa),
                // 50
                Instr32(Opcode.st, RXa),
                Instr32(Opcode.lae, RXa),
                invalid,
                invalid,

                Instr32(Opcode.n, RXa),
                Instr32(Opcode.cl, RXa),
                Instr32(Opcode.o, RXa),
                Instr32(Opcode.x, RXa),

                Instr32(Opcode.l, RXa),
                Instr32(Opcode.c, RXa),
                Instr32(Opcode.a, RXa),
                Instr32(Opcode.s, RXa),

                Instr32(Opcode.m, RXa),
                Instr32(Opcode.d, RXa),
                Nyi("*"),
                Nyi("*"),
                // 60
                Instr32(Opcode.std, FXa),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr32(Opcode.mxd, FXa),

                Instr32(Opcode.ld, FXa),
                Instr32(Opcode.cd, FXa),
                Instr32(Opcode.ad, FXa),
                Instr32(Opcode.sd, FXa),

                Instr32(Opcode.md, FXa),
                Instr32(Opcode.dd, FXa),
                Nyi("*"),
                Nyi("*"),
                // 70
                Instr32(Opcode.ste, FXa),
                Instr32(Opcode.ms, RXa),
                invalid,
                invalid,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr32(Opcode.ae, FXa),
                Instr32(Opcode.se, FXa),

                Nyi("*"),
                Nyi("*"),
                Instr32(Opcode.au, FXa),
                Instr32(Opcode.su, FXa),
                // 80
                Instr32(Opcode.ssm, S),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr32(Opcode.sra, RSa),
                Nyi("*"),

                Instr32(Opcode.srl, RSa),
                Instr32(Opcode.sll, RSa),
                Instr32(Opcode.sra, RSa),
                Instr32(Opcode.sla, RSa),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 90
                Instr32(Opcode.stm, RSa3),
                Nyi("*"),
                Instr32(Opcode.mvi, SI),
                Nyi("*"),

                Nyi("*"),
                Instr32(Opcode.cli, SI),
                Instr32(Opcode.oi, SI),
                Instr32(Opcode.xi, SI),

                Instr32(Opcode.lm, RSa3),
                Nyi("*"),
                Instr32(Opcode.lam, RSa3),
                Instr32(Opcode.stam, RSa3),

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
                    (0x00, Instr(Opcode.iihh, RIa)),
                    (0x01, Instr(Opcode.iihl, RIa)),
                    (0x02, Instr(Opcode.iilh, RIa)),
                    (0x03, Instr(Opcode.iill, RIa)),
                    (0x08, Instr(Opcode.lhi, RIa)),
                    (0x09, Instr(Opcode.lghi, RIa)),
                    (0x0A, Instr(Opcode.ahi, RIa)),
                    (0x0F, Instr(Opcode.llill, RIa))),
                Nyi("*"),
                ExtendMask32(16, 4,
                    (0x04, Mask(20, 4,
                        Instr(Opcode.brc, RIc),
                        Instr(Opcode.jo, RIc),
                        Instr(Opcode.jh, RIc),
                        Instr(Opcode.jnle, RIc),
                        Instr(Opcode.jl, RIc),
                        Instr(Opcode.jnhe, RIc),
                        Instr(Opcode.jlh, RIc),
                        Instr(Opcode.jne, RIc),
                        Instr(Opcode.je, RIc),
                        Instr(Opcode.jnlh, RIc),
                        Instr(Opcode.jhe, RIc),
                        Instr(Opcode.jnl, RIc),
                        Instr(Opcode.jle, RIc),
                        Instr(Opcode.jnh, RIc),
                        Instr(Opcode.jno, RIc),
                        Instr(Opcode.j, RIc))),
                    (0x7, Instr(Opcode.brctg, RIb)),
                    (0x8, Instr(Opcode.lhi, RIa)),
                    (0x9, Instr(Opcode.lghi, RIa)),
                    (0xA, Instr(Opcode.ahi, RIa)),
                    (0xB, Instr(Opcode.aghi, RIa)),
                    (0xE, Instr(Opcode.chi, RIa)),
                    (0xF, Instr(Opcode.cghi, RIa))),

                Instr32(Opcode.mvcle, RSa3),
                Nyi("*"),
                Instr32(Opcode.unpka, SSa),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // B0
                invalid,
                Instr32(Opcode.lra, RXa),
                b2_decoders,
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                ExtendMask32(16, 8,
                    (0x02, Instr(Opcode.ltgr, RRE)),
                    (0x04, Instr(Opcode.lgr, RRE)),
                    (0x08, Instr(Opcode.agr, RRE)),
                    (0x09, Instr(Opcode.sgr, RRE)),
                    (0x12, Instr(Opcode.ltgfr, RRE)),
                    (0x14, Instr(Opcode.lgfr, RRE)),
                    (0x80, Instr(Opcode.ngr, RRE))),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Instr32(Opcode.icm, RSb),
                // C0
                ExtendMask48(32, 4,
                    (0x0, Instr(Opcode.larl, RILb)),
                    (0x4, Mask(36, 4,
                        Instr(Opcode.brcl, RILc),
                        Instr(Opcode.jgo, RILc),
                        Instr(Opcode.jgh, RILc),
                        Instr(Opcode.jgnle, RILc),
                        Instr(Opcode.jgl, RILc),
                        Instr(Opcode.jgnhe, RILc),
                        Instr(Opcode.jglh, RILc),
                        Instr(Opcode.jgne, RILc),
                        Instr(Opcode.jge, RILc),
                        Instr(Opcode.jgnlh, RILc),
                        Instr(Opcode.jghe, RILc),
                        Instr(Opcode.jgnl, RILc),
                        Instr(Opcode.jgle, RILc),
                        Instr(Opcode.jgnh, RILc),
                        Instr(Opcode.jgno, RILc),
                        Instr(Opcode.jg, RILc))),
                    (0x5, Instr(Opcode.brasl, RILb))),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Instr48(Opcode.bprp, MII),
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
                Instr48(Opcode.trtr, SSa),
                Instr48(Opcode.mvc, SSa),
                Instr48(Opcode.mvz, SSa),
                Instr48(Opcode.xc, SSa),

                Instr48(Opcode.nc, SSa),
                Instr48(Opcode.clc, SSa),
                Instr48(Opcode.oc, SSa),
                Instr48(Opcode.xc, SSa),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Instr48(Opcode.mvcs, SSd),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Instr48(Opcode.edmk, SSa),
                // E0
                invalid,
                Instr48(Opcode.pku, SSf),
                Instr48(Opcode.unpku, SSa),
                e3_decoders,

                Nyi("*"),
                e5_decoders,
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr48(Opcode.unpka, SSa),
                eb_decoders,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // F0
                Instr48(Opcode.srp, SSc),
                Instr48(Opcode.mvo, SSb),
                Nyi("*"),
                Nyi("*"),

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi("*"),
                Nyi("*"),
                Instr48(Opcode.ap, SSb),
                Instr48(Opcode.sp, SSb),

                Instr48(Opcode.mp, SSb),
                Instr48(Opcode.dp, SSb),
                invalid,
                invalid,
            };
        }
    }
}