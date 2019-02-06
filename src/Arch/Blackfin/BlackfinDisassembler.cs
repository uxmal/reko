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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Blackfin
{
    public class BlackfinDisassembler : DisassemblerBase<BlackfinInstruction>
    {
        private delegate bool Mutator(uint uInstr, BlackfinDisassembler dasm);

        private static readonly Decoder rootDecoder;

        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;

        private Address addr;
        private BlackfinInstruction instr;
        private List<MachineOperand> ops;

        public BlackfinDisassembler(BlackfinArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override BlackfinInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            this.ops = new List<MachineOperand>();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Length = (int)(rdr.Address - this.addr);
            return instr;
        }

        private BlackfinInstruction Invalid()
        {
            return new BlackfinInstruction
            {
                IClass = InstrClass.Invalid,
                Opcode = Opcode.invalid,
                Address = addr,
                Length = (int) (rdr.Address - addr),
            };
        }

        private abstract class Decoder
        {
            public abstract BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm);

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, Bitfield[] bitfields, string debugString)
            {
                var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
                TraceDecoder(wInstr, shMask, debugString);
            }

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, Bitfield bitfield, string debugString)
            {
                var shMask = bitfield.Mask << bitfield.Position;
                TraceDecoder(wInstr, shMask, debugString);
            }

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, uint shMask, string debugString)
            {
                // return;
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

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            public InstrDecoder(InstrClass iclass, Opcode opcode, params Mutator[] mutators)
            {
                this.iclass = iclass;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm)
            {
                dasm.instr = new BlackfinInstruction();
                foreach (var mutator in mutators)
                {
                    if (!mutator(uInstr, dasm))
                        return dasm.Invalid();
                }
                dasm.instr.Address = dasm.addr;
                dasm.instr.IClass = iclass;
                dasm.instr.Opcode = opcode;
                dasm.instr.Operands = dasm.ops.ToArray();
                return dasm.instr;
            }
        }

        private class Mask32Decoder : MaskDecoder
        {
            public Mask32Decoder(Bitfield bitfield, params Decoder[] decoders) : base(bitfield, decoders)
            {
            }

            public override BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out ushort uInstrLo))
                    return dasm.Invalid();
                uInstr = (uInstr << 16) | uInstrLo;
                return base.Decode(uInstr, dasm);
            }
        }

        private class MaskDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public MaskDecoder(Bitfield bitfield, params Decoder[] decoders)
            {
                this.bitfield = bitfield;
                this.decoders = decoders;
            }

            public override BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm)
            {
                TraceDecoder(uInstr, bitfield, "");
                var decoder = decoders[bitfield.Read(uInstr)];
                return decoder.Decode(uInstr, dasm);
            }
        }

        private class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm)
            {
                string instrHexBytes;
                if (uInstr > 0xFFFF)
                {
                    instrHexBytes = $"{uInstr >> 16:X4}{ uInstr & 0xFFFF:X4}";
                }
                else
                {
                    instrHexBytes = $"{uInstr:X4}";
                }
                var rev = $"{Bits.Reverse(uInstr):X8}";
                message = (string.IsNullOrEmpty(message))
                    ? rev
                    : $"{rev} - {message}";
                dasm.EmitUnitTest("Blackfin", instrHexBytes, message, "BlackfinDasm", dasm.addr, w =>
                {
                    if (uInstr > 0xFFFF)
                    {
                        w.WriteLine($"    var instr = DisassembleHexBytes(\"{rev.Substring(4)}{rev.Substring(0,4)}\");");
                    }
                    else
                    {
                        w.WriteLine($"    var instr = DisassembleHexBytes(\"{rev.Substring(0, 4)}\");");
                    }
                    w.WriteLine("    Assert.AreEqual(\"@@@\", instr.ToString());");
                });
                return dasm.Invalid();
            }
        }

        private static InstrDecoder Instr(InstrClass iclass, Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(iclass, opcode, mutators);
        }

        private static InstrDecoder Instr( Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, opcode, mutators);
        }

        private static MaskDecoder Mask(int pos, int len, params Decoder[] decoders)
        {
            return new MaskDecoder(new Bitfield(pos, len), decoders);
        }

        private static MaskDecoder Mask(
            int pos, 
            int len, 
            Decoder defaultDecoder,
            params (int, Decoder)[] sparseDecoders)
        {
            var decoders = Enumerable.Range(0, 1 << len)
                .Select(i => defaultDecoder)
                .ToArray();
            foreach (var sd in sparseDecoders)
            {
                decoders[sd.Item1] = sd.Item2;
            }
            return new MaskDecoder(new Bitfield(pos, len), decoders);
        }

        private static MaskDecoder Mask32(
            int pos,
            int len,
            Decoder defaultDecoder,
            params (int, Decoder)[] sparseDecoders)
        {
            var decoders = Enumerable.Range(0, 1 << len)
                .Select(i => defaultDecoder)
                .ToArray();
            foreach (var sd in sparseDecoders)
            {
                decoders[sd.Item1] = sd.Item2;
            }
            return new Mask32Decoder(new Bitfield(pos, len), decoders);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        #region Mutators

        // RegisterGroup + register
        private static Mutator R(RegisterStorage[] regs, int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var iReg = (int) bitfield.Read(u);
                var reg = regs[iReg];
                if (reg == null)
                    return false;
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static readonly Mutator Rlo16 = R(Registers.RPIB_Lo, 16);
        private static readonly Mutator Rhi16 = R(Registers.RPI_Hi, 16);
        private static readonly Mutator Rpib16 = R(Registers.RPIB, 16);

        // Immediate quantity
        private static Mutator Imm(int pos, int len, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                var c = Constant.Create(dt, imm);
                d.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        // Indirect pointer
        private static Mutator IP(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                d.ops.Add(new MemoryOperand(PrimitiveType.Word32)
                {
                    Base = Registers.Pointers[bitfield.Read(u)]
                });
                return true;
            };
        }

        private static Mutator IP0 = IP(0);

        // PC-indexed
        private static Mutator PCIX(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                d.ops.Add(new MemoryOperand(PrimitiveType.Word32)
                {
                    Base = Registers.PC,
                    Index = Registers.Pointers[bitfield.Read(u)]
                });
                return true;
            };
        }

        private static Mutator PCIX0 = PCIX(0);

        // PC-relative jump destination
        private static Mutator J(int bitsize)
        {
            var bitfield = new Bitfield(0, bitsize);
            return (u, d) =>
            {
                var offset = bitfield.ReadSigned(u) << 1;
                d.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };
        }

        private static Mutator J12 = J(12);

        #endregion


        static BlackfinDisassembler()
        {
            var invalid = Instr(InstrClass.Invalid, Opcode.invalid);
            var instr32 = Mask32(24, 8,
                Nyi(""),
                (0xE1, Mask(5 + 16, 2,
                    Instr(Opcode.mov, Rlo16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Opcode.mov_x, Rpib16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Opcode.mov, Rhi16, Imm(0, 16, PrimitiveType.Word16)),
                    Nyi(""))));

            rootDecoder = Mask(12, 4, new Decoder[16]
            {
                Mask(4, 8, 
                    Nyi("0b0000............"),
                    (0x05, Mask(3, 1,
                        Instr(InstrClass.Transfer, Opcode.JUMP, IP0),
                        invalid)),
                    (0x08, Mask(3, 1,
                        Instr(InstrClass.Transfer, Opcode.JUMP, PCIX0),
                        invalid))
                    ),
                Nyi("0b0001............"),
                Instr(InstrClass.Transfer, Opcode.JUMP_S, J12),
                Nyi("0b0011............"),

                Nyi("0b0100............"),
                Nyi("0b0101............"),
                Nyi("0b0110............"),
                Nyi("0b0111............"),

                Nyi("0b1000............"),
                Nyi("0b1001............"),
                Nyi("0b1010............"),
                Nyi("0b1011............"),

                instr32,
                instr32,
                instr32,
                instr32,
            });
        }
    }
}