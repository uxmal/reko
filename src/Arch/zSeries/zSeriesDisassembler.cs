#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
            return instr;
        }

        private MachineOperand CreateAccess(
            RegisterStorage baseReg,
            RegisterStorage idxReg,
            int offset)
        {
            if (baseReg == null || baseReg.Number == 0)
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

        private static HashSet<ulong> seen = new HashSet<ulong>();

        private void Nyi(ulong uInstr, string message)
        {
#if DEBUG
            if (true && !seen.Contains(uInstr))
            {
                seen.Add(uInstr);

                var rdr2 = rdr.Clone();
                int len = (int)(rdr.Address - this.addr);
                rdr2.Offset -= len;
                var bytes = rdr2.ReadBytes(len);
                var hexBytes = string.Join("", bytes
                        .Select(b => b.ToString("X2")));
                Console.WriteLine($"// A zSeries decoder for the instruction {uInstr:X8} ({message}) has not been implemented yet.");
                Console.WriteLine("[Test]");
                Console.WriteLine($"public void zSerDasm_{uInstr:X8}()");
                Console.WriteLine("{");
                Console.WriteLine("    AssertCode(\"@@@\", \"{0}\");", hexBytes);
                Console.WriteLine("}");
                Console.WriteLine();
            }
#endif
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

        public static ExtendDecoder32 Extend32(Opcode opcode, params Mutator[] mutators)
        {
            return new ExtendDecoder32(opcode, mutators);
        }

        public static ExtendDecoder48 Extend48(Opcode opcode, params Mutator[] mutators)
        {
            return new ExtendDecoder48(opcode, mutators);
        }

        public static MaskDecoder Mask(int pos, int len, params Decoder[] decoders)
        {
            return new MaskDecoder(pos, len, decoders);
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

            var e3_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Opcode.lg, RXYa)),
                (0x14, Instr(Opcode.lgf, RXYa)),
                (0x21, Instr(Opcode.clg, RXYa)),
                (0x24, Instr(Opcode.stg, RXYa)));
            var eb_decoders = ExtendMask48(0, 8,
                (0x04, Instr(Opcode.lmg, RSYa)),
                (0x0A, Instr(Opcode.srag, RSYa)),
                (0x0B, Instr(Opcode.slag, RSYa)),
                (0x0C, Instr(Opcode.srlg, RSYa)),
                (0x24, Instr(Opcode.stmg, RSYa)));

            decoders = new Decoder[256]
            {
                // 00
                invalid,
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
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
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.bassm, RR),
                Instr(Opcode.basr, RR),
                Nyi("*"),
                Nyi("*"),
                // 10
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.nr, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.lr, RR),
                Nyi("*"),
                Instr(Opcode.ar, RR),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 20
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
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Instr(Opcode.swr, RR),
                // 30
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
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 40
                Nyi("*"),
                Extend32(Opcode.la, RXa),
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
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 50
                Extend32(Opcode.st, RXa),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Extend32(Opcode.l, RXa),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 60
                Nyi("*"),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Nyi("*"),

                Nyi("*"),
                Instr(Opcode.cd, RXa),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 70
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
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 80
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
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 90
                Nyi("*"),
                Nyi("*"),
                Extend32(Opcode.mvi, SI),
                Nyi("*"),

                Nyi("*"),
                Extend32(Opcode.cli, SI),
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
                // A0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
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

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // B0
                invalid,
                Nyi("*"),
                Nyi("*"),
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
                Nyi("*"),
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
                // D0
                Extend48(Opcode.trtr, SSa),
                Extend48(Opcode.mvc, SSa),
                Extend48(Opcode.mvz, SSa),
                Extend48(Opcode.xc, SSa),

                Extend48(Opcode.nc, SSa),
                Extend48(Opcode.clc, SSa),
                Extend48(Opcode.oc, SSa),
                Extend48(Opcode.xc, SSa),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // E0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                e3_decoders,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                eb_decoders,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // F0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                invalid,
                invalid,
            };
        }
    }
}