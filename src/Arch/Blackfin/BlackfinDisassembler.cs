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

        protected override BlackfinInstruction CreateInvalidInstruction()
        {
            return new BlackfinInstruction
            {
                InstructionClass = InstrClass.Invalid,
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
                        return dasm.CreateInvalidInstruction();
                }
                dasm.instr.Address = dasm.addr;
                dasm.instr.InstructionClass = iclass;
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
                    return dasm.CreateInvalidInstruction();
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

        private class ConditionalDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public ConditionalDecoder(Bitfield bitfield, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.bitfield = bitfield;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override BlackfinInstruction Decode(uint uInstr, BlackfinDisassembler dasm)
            {
                var val = bitfield.Read(uInstr);
                var decoder = (predicate(val)) ? trueDecoder : falseDecoder;
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
                return dasm.CreateInvalidInstruction();
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

        private static ConditionalDecoder Cond(
            int pos, int len,
            Predicate<uint> predicate,
            Decoder trueDecoder,
            Decoder falseDecoder)
        {
            return new ConditionalDecoder(
                new Bitfield(pos, len),
                predicate,
                trueDecoder, falseDecoder);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        #region Mutators

        // RegisterGroup + register
        private static Mutator R(RegisterStorage[] regs, int bitpos, int len)
        {
            var bitfield = new Bitfield(bitpos, len);
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

        private static Mutator R(RegisterStorage[] regs, int bitpos1, int len1, int bitpos2, int len2)
        {
            var bitfields = new[] {
                new Bitfield(bitpos1, len1),
                new Bitfield(bitpos2, len2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                var reg = regs[iReg];
                if (reg == null)
                    return false;
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static Mutator FlagGroup(int bitPos, int bitLen)
        {
            var field = new Bitfield(bitPos, bitLen);
            return (u, d) =>
            {
                var bitNo = (int)field.Read(u);
                var flag = d.arch.GetFlagGroup(Registers.ASTAT, 1u<<bitNo);
                d.ops.Add(new FlagGroupOperand(flag));
                return true;
            };
        }

        private static readonly Mutator Rlo16 = R(Registers.RPIB_Lo, 16, 5);
        private static readonly Mutator Rhi16 = R(Registers.RPI_Hi, 16, 5);
        private static readonly Mutator Rpib0 = R(Registers.RPIB, 0, 5);
        private static readonly Mutator Rpib16 = R(Registers.RPIB, 16, 5);
        private static readonly Mutator RallRegs0 = R(Registers.AllReg, 0, 6);
        private static readonly Mutator RmostRegs0 = R(Registers.MostReg, 0, 6);

        private static readonly Mutator D0 = R(Registers.Data, 0, 3);
        private static readonly Mutator D3 = R(Registers.Data, 3, 3);
        private static readonly Mutator D6 = R(Registers.Data, 6, 3);
        private static readonly Mutator D9 = R(Registers.Data, 9, 3);
        private static readonly Mutator D16 = R(Registers.Data, 16, 3);

        private static readonly Mutator P0 = R(Registers.Pointers, 0, 3);
        private static readonly Mutator P3 = R(Registers.Pointers, 3, 3);
        private static readonly Mutator P6 = R(Registers.Pointers, 6, 3);
        private static readonly Mutator P16 = R(Registers.Pointers, 16, 3);

        private static readonly Mutator R0 = R(Registers.DataPointers, 0, 4);

        private static bool A0(uint u, BlackfinDisassembler d)
        {
            d.ops.Add(new RegisterOperand(Registers.A0));
            return true;
        }

        private static bool A1(uint u, BlackfinDisassembler d)
        {
            d.ops.Add(new RegisterOperand(Registers.A0));
            return true;
        }

        // Pointer register indirect
        private static Mutator PtrInd(int pos, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, 3);
            return (u, d) =>
            {
                var ptrReg = Registers.Pointers[bitfield.Read(u)];
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = ptrReg,
                });
                return true;
            };
        }

        private static Mutator IdxInd(int pos, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, 2);
            return (u, d) =>
            {
                var ptrReg = Registers.Indices[bitfield.Read(u)];
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = ptrReg,
                });
                return true;
            };
        }

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

        // Scaled immediate
        private static Mutator ImmSh(int pos, int len, int sh, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                var c = Constant.Create(dt, imm << sh);
                d.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        private static Mutator I(int n, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var c = Constant.Create(dt, n);
                d.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        // Sign-extended immediate quantity.
        private static Mutator Imms(int pos, int len, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u);
                var c = Constant.Create(dt, imm);
                d.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        // Negated 2's complement
        private static Mutator NImms(int pos, int len, PrimitiveType dt)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u);
                var c = Constant.Create(dt, -imm);
                d.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        // Pointer + Offset
        private static Mutator MoffS(int ptrPos, int bitOff, int lenOff, int sh, PrimitiveType dt)
        {
            var bitfield = new Bitfield(ptrPos, 3);
            var offsetfield = new Bitfield(bitOff, lenOff);
            return (u, d) =>
            {
                var iPtr = bitfield.Read(u);
                var ptr = Registers.Pointers[iPtr];
                var off = offsetfield.ReadSigned(u) << sh;
                var mem = new MemoryOperand(dt)
                {
                    Base = ptr,
                    Offset = off
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator MoffU(int ptrPos, int bitOff, int lenOff, int sh, PrimitiveType dt)
        {
            var regField = new Bitfield(ptrPos, 3);
            var offsetfield = new Bitfield(bitOff, lenOff);
            return (u, d) =>
            {
                var iPtr = regField.Read(u);
                var ptr = Registers.Pointers[iPtr];
                var off = offsetfield.Read(u) << sh;
                var mem = new MemoryOperand(dt)
                {
                    Base = ptr,
                    Offset = (int)off
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator MfpOffU(int bitOff, int lenOff, int sh, PrimitiveType dt)
        {
            var offsetfield = new Bitfield(bitOff, lenOff);
            return (u, d) =>
            {
                var off = offsetfield.Read(u) << sh;
                var mem = new MemoryOperand(dt)
                {
                    Base = Registers.FP,
                    Offset = -(int) off
                };
                d.ops.Add(mem);
                return true;
            };
        }



        // Indirect pointer
        private static Mutator IP(int bitpos,PrimitiveType dt)
        {
            var bitfield = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.Pointers[bitfield.Read(u)]
                });
                return true;
            };
        }

        private static readonly Mutator IP0 = IP(0, PrimitiveType.Word32);
        private static readonly Mutator IP3 = IP(3, PrimitiveType.Word32);

        private static Mutator PostInc(RegisterStorage[] regs, int bitpos, int bitlen, PrimitiveType dt)
        {
            var bitfield = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = regs[bitfield.Read(u)],
                    PostIncrement = true
                });
                return true;
            };
        }

        private static Mutator PostDec(RegisterStorage[] regs, int bitpos, int bitlen, PrimitiveType dt)
        {
            var bitfield = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = Registers.Pointers[bitfield.Read(u)],
                    PostDecrement = true
                });
                return true;
            };
        }

        private static readonly Mutator Post0 = PostInc(Registers.Pointers, 0, 3, PrimitiveType.Word32);
        private static readonly Mutator PostInc3 = PostInc(Registers.Pointers, 3, 3, PrimitiveType.Word32);
        private static readonly Mutator PostI0 = PostInc(Registers.Indices, 0, 2, PrimitiveType.Word32);
        private static readonly Mutator PostI3 = PostInc(Registers.Indices, 3, 2, PrimitiveType.Word32);
        private static readonly Mutator PostDec3 = PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word32);

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

        private static readonly Mutator PCIX0 = PCIX(0);

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

        private static readonly Mutator J12 = J(12);

        private static bool SPpost(uint u, BlackfinDisassembler d)
        {
            var mem = new MemoryOperand(PrimitiveType.Word32)
            {
                Base = Registers.SP,
                PostIncrement = true
            };
            d.ops.Add(mem);
            return true;
        }

        private static bool SPpre(uint u, BlackfinDisassembler d)
        {
            var mem = new MemoryOperand(PrimitiveType.Word32)
            {
                Base = Registers.SP,
                PreDecrement = true
            };
            d.ops.Add(mem);
            return true;
        }

        private static Mutator Range0(int iregMax, RegisterStorage[] regs)
        {
            var bitfield = new Bitfield(0, 3);
            return (u, d) =>
            {
                var iregMin = bitfield.Read(u);
                if (iregMin > iregMax)
                    return false;
                d.ops.Add(new RegisterRange(Registers.Pointers[0].DataType, regs[iregMax], regs[iregMin]));
                return true;
            };
        }

        private static Mutator J(int pos, int len, int sh)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var pcOffset = bitfield.ReadSigned(u) << sh;
                d.ops.Add(AddressOperand.Create(d.addr + pcOffset));
                return true;
            };
        }
        #endregion

        private static bool IsZero(uint n) => n == 0;

        static BlackfinDisassembler()
        {
            var invalid = Instr(InstrClass.Invalid, Opcode.invalid);
            var instr32 = Mask32(24, 8,
                Nyi(""),
                (0xC4, Mask(16, 8,
                    invalid,
                    (0x02, Mask(12, 4,
                        invalid,
                        (0, Instr(Opcode.add3, 
                            R(Registers.RPIB_Lo, 9, 3), 
                            R(Registers.RPIB_Lo, 3, 3), 
                            R(Registers.RPIB_Lo, 3, 3))))),
                    (0x22, Mask(12, 4,
                        invalid,
                        (4, Instr(Opcode.add3, R(Registers.RPI_Hi,9,3), R(Registers.RPIB_Lo,3,3), R(Registers.RPI_Hi,3,3))))))),
                (0xC6, Mask(16, 8,
                    invalid,
                    (0x82, Mask(12, 4,
                        invalid,
                        (0, Cond(8, 1, n => n == 0,
                            invalid,
                            Instr(Opcode.asr3, D9, D0, NImms(3, 5, PrimitiveType.Byte)))),
                        (8, Cond(8, 1, n => n == 0,
                            Instr(Opcode.lsl3, D9, D0, Imms(3, 5, PrimitiveType.Byte)),
                            Instr(Opcode.lsr3, D9, D0, NImms(3, 5, PrimitiveType.Byte)))))))),
                (0xC8, Cond(16, 8, n => n == 3, 
                    Instr(InstrClass.Padding|InstrClass.Linear, Opcode.MNOP),
                    invalid)),
                (0xE1, Mask(5 + 16, 2,
                    Instr(Opcode.mov, Rlo16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Opcode.mov_x, Rpib16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Opcode.mov, Rhi16, Imm(0, 16, PrimitiveType.Word16)),
                    Nyi(""))),
                (0xE2, Instr(Opcode.JUMP_L, J(0, 24, 1))),
                (0xE3, Instr(Opcode.CALL, J(0, 24, 1))),

                (0xE4, Mask(6 + 16, 2,
                    Instr(Opcode.mov, D16, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32)),
                    Instr(Opcode.mov_z, D16, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16)),
                    Instr(Opcode.mov_z, D16, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte)),
                    invalid)),
                (0xE5, Mask(6 + 16, 2,
                    Instr(Opcode.mov, P16, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32)),
                    Instr(Opcode.mov_x, D16, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16)),
                    Instr(Opcode.mov_x, D16, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte)),
                    invalid)),
                (0xE6, Mask(6 + 16, 2,
                    Instr(Opcode.mov, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32), D16),
                    Instr(Opcode.mov, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16), D16),
                    Instr(Opcode.mov, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte), D16),
                    invalid)),
                (0xE7, Instr(Opcode.mov, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32), P16)),

                (0xE8, Mask(16, 8,
                    invalid,
                    (0, Instr(Opcode.LINK, ImmSh(0, 16, 4, PrimitiveType.Int32))),
                    (1, Instr(Opcode.UNLINK)))));

            rootDecoder = Mask(12, 4, new Decoder[16]
            {
                Mask(4, 8,
                    Nyi("0b0000????????...."),
                    (0x00, Cond(0, 4, IsZero,
                        Instr(InstrClass.Zero|InstrClass.Linear|InstrClass.Padding, Opcode.NOP),
                        invalid)),
                    (0x01, Mask(0, 4, invalid,
                        (0x0, Instr(InstrClass.Transfer, Opcode.RTS)),
                        (0x1, Instr(InstrClass.Transfer, Opcode.RTI)),
                        (0x2, Instr(InstrClass.Transfer, Opcode.RTX)),
                        (0x3, Instr(InstrClass.Transfer, Opcode.RTN)),
                        (0x4, Instr(InstrClass.Transfer, Opcode.RTE)))),
                    (0x02, Mask(0, 4, invalid,
                        (0x0, Instr(Opcode.IDLE)),
                        (0x3, Instr(Opcode.CSYNC)),
                        (0x4, Instr(Opcode.SSYNC)),
                        (0x5, Instr(Opcode.EMUEXCEPT)),
                        (0xF, Instr(Opcode.ABORT)))),
                    (0x03, Mask(3, 1,
                        Instr(InstrClass.System|InstrClass.Linear, Opcode.CLI, D0),
                        invalid)),
                    (0x04, Mask(3, 1,
                        Instr(Opcode.STI, D0),
                        invalid)),
                    (0x05, Mask(3, 1,
                        Instr(InstrClass.Transfer, Opcode.JUMP, IP0),
                        invalid)),
                    (0x06, Mask(3, 1,
                        Instr(InstrClass.Transfer, Opcode.CALL, IP0),
                        invalid)),
                    (0x08, Mask(3, 1,
                        Instr(InstrClass.Transfer, Opcode.JUMP, PCIX0),
                        invalid)),
                    (0x09, Instr(InstrClass.Linear, Opcode.RAISE, Imm(0, 4, PrimitiveType.Byte))),
                    (0x0A, Instr(InstrClass.Linear, Opcode.EXCPT, Imm(0, 4, PrimitiveType.Byte))),

                    (0x10, invalid),
                    (0x11, Instr(Opcode.mov, RmostRegs0, SPpost)),
                    (0x12, Instr(Opcode.mov, RmostRegs0, SPpost)),
                    (0x13, Instr(Opcode.mov, RmostRegs0, SPpost)),

                    (0x14, Instr(Opcode.mov, SPpre, RallRegs0)),
                    (0x15, Instr(Opcode.mov, SPpre, RallRegs0)),
                    (0x16, Instr(Opcode.mov, SPpre, RallRegs0)),
                    (0x17, Instr(Opcode.mov, SPpre, RallRegs0)),

                    (0x18, Instr(Opcode.if_ncc_mov, D3,D0)),
                    (0x19, Instr(Opcode.if_ncc_mov, D3,P0)),
                    (0x1A, Instr(Opcode.if_ncc_mov, P3,D0)),
                    (0x1B, Instr(Opcode.if_ncc_mov, P3,P0)),

                    (0x1C, Instr(Opcode.if_cc_mov, D3,D0)),
                    (0x1D, Instr(Opcode.if_cc_mov, D3,P0)),
                    (0x1E, Instr(Opcode.if_cc_mov, P3,D0)),
                    (0x1F, Instr(Opcode.if_cc_mov, P3,P0)),

                    (0x20, Instr(Opcode.mov_cc_eq, D0,D3)),
                    (0x21, Cond(0, 4, n => n == 8,
                        Instr(Opcode.neg_cc),
                        invalid)),
                    (0x25, Mask(3, 1,
                        Instr(Opcode.flush, PtrInd(0, PrimitiveType.Word32)),
                        Instr(Opcode.iflush, PtrInd(0, PrimitiveType.Word32)))),
                    (0x26, Mask(3, 1,
                        Instr(Opcode.prefetch, Post0),
                        Instr(Opcode.flushinv, Post0))),
                    (0x27, Mask(3, 1,
                        Instr(Opcode.flush, Post0),
                        Instr(Opcode.iflush, Post0))),

                    (0x30, Instr(Opcode.mov_cc_eq, D0,Imms(3, 3, PrimitiveType.Word32))),
                    (0x38, Instr(Opcode.mov_r_cc, FlagGroup(0, 4))),
                    (0x48, Instr(Opcode.mov, Range0(5, Registers.Pointers), SPpost)),
                    (0x4C, Instr(Opcode.mov, SPpre, Range0(5, Registers.Pointers))),

                    (0x50, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x51, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x52, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x53, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),

                    (0x54, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x55, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x56, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x57, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),

                    (0x58, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x59, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x5A, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x5B, Instr(Opcode.mov, Range0(7, Registers.Pointers), SPpost)),

                    (0x5C, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5D, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5E, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5F, Instr(Opcode.mov, SPpre, Range0(7, Registers.Pointers))),

                    (0x60, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x61, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x62, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x63, Instr(Opcode.if_ncc_mov, D3, D0)),

                    (0x64, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x65, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x66, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x67, Instr(Opcode.if_ncc_mov, D3, P0)),

                    (0x68, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x69, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x6A, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x6B, Instr(Opcode.if_ncc_mov, P3, D0)),

                    (0x6C, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x6D, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x6E, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x6F, Instr(Opcode.if_ncc_mov, P3, P0)),

                    (0x70, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x71, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x72, Instr(Opcode.if_ncc_mov, D3, D0)),
                    (0x73, Instr(Opcode.if_ncc_mov, D3, D0)),

                    (0x74, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x75, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x76, Instr(Opcode.if_ncc_mov, D3, P0)),
                    (0x77, Instr(Opcode.if_ncc_mov, D3, P0)),

                    (0x78, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x79, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x7A, Instr(Opcode.if_ncc_mov, P3, D0)),
                    (0x7B, Instr(Opcode.if_ncc_mov, P3, D0)),

                    (0x7C, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x7D, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x7E, Instr(Opcode.if_ncc_mov, P3, P0)),
                    (0x7F, Instr(Opcode.if_ncc_mov, P3, P0)),

                    (0x80, Instr(Opcode.mov_cc_eq, D3, D0)),
                    (0x81, Instr(Opcode.mov_cc_eq, D3, D0)),
                    (0x82, Instr(Opcode.mov_cc_eq, D3, D0)),
                    (0x83, Instr(Opcode.mov_cc_eq, D3, D0)),

                    (0x84, Instr(Opcode.mov_cc_eq, P3, P0)),
                    (0x85, Instr(Opcode.mov_cc_eq, P3, P0)),
                    (0x86, Instr(Opcode.mov_cc_eq, P3, P0)),
                    (0x87, Instr(Opcode.mov_cc_eq, P3, P0)),

                    (0x88, Instr(Opcode.mov_cc_lt, D3, D0)),
                    (0x89, Instr(Opcode.mov_cc_lt, D3, D0)),
                    (0x8A, Instr(Opcode.mov_cc_lt, D3, D0)),
                    (0x8B, Instr(Opcode.mov_cc_lt, D3, D0)),

                    (0x8C, Instr(Opcode.mov_cc_lt, P3, P0)),
                    (0x8D, Instr(Opcode.mov_cc_lt, P3, P0)),
                    (0x8E, Instr(Opcode.mov_cc_lt, P3, P0)),
                    (0x8F, Instr(Opcode.mov_cc_lt, P3, P0)),

                    (0x90, Instr(Opcode.mov_cc_le, D3, D0)),
                    (0x91, Instr(Opcode.mov_cc_le, D3, D0)),
                    (0x92, Instr(Opcode.mov_cc_le, D3, D0)),
                    (0x93, Instr(Opcode.mov_cc_le, D3, D0)),

                    (0x94, Instr(Opcode.mov_cc_le, P3, P0)),
                    (0x95, Instr(Opcode.mov_cc_le, P3, P0)),
                    (0x96, Instr(Opcode.mov_cc_le, P3, P0)),
                    (0x97, Instr(Opcode.mov_cc_le, P3, P0)),

                    (0x98, Instr(Opcode.mov_cc_ult, D3, D0)),
                    (0x99, Instr(Opcode.mov_cc_ult, D3, D0)),
                    (0x9A, Instr(Opcode.mov_cc_ult, D3, D0)),
                    (0x9B, Instr(Opcode.mov_cc_ult, D3, D0)),

                    (0x9C, Instr(Opcode.mov_cc_ult, P3, P0)),
                    (0x9D, Instr(Opcode.mov_cc_ult, P3, P0)),
                    (0x9E, Instr(Opcode.mov_cc_ult, P3, P0)),
                    (0x9F, Instr(Opcode.mov_cc_ult, P3, P0)),

                    (0xA0, Instr(Opcode.mov_cc_ule, D3, D0)),
                    (0xA1, Instr(Opcode.mov_cc_ule, D3, D0)),
                    (0xA2, Instr(Opcode.mov_cc_ule, D3, D0)),
                    (0xA3, Instr(Opcode.mov_cc_ule, D3, D0)),

                    (0xA4, Instr(Opcode.mov_cc_ule, P3, P0)),
                    (0xA5, Instr(Opcode.mov_cc_ule, P3, P0)),
                    (0xA6, Instr(Opcode.mov_cc_ule, P3, P0)),
                    (0xA7, Instr(Opcode.mov_cc_ule, P3, P0)),

                    (0xA8, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xA9, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xAA, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xAB, Instr(Opcode.mov_cc_eq, A0, A1)),

                    (0xAC, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xAD, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xAE, Instr(Opcode.mov_cc_eq, A0, A1)),
                    (0xAF, Instr(Opcode.mov_cc_eq, A0, A1)),

                    (0xB8, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xB9, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xBA, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xBB, Instr(Opcode.mov_cc_lt, A0, A1)),

                    (0xBC, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xBD, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xBE, Instr(Opcode.mov_cc_lt, A0, A1)),
                    (0xBF, Instr(Opcode.mov_cc_lt, A0, A1)),

                    (0xC0, Instr(Opcode.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC1, Instr(Opcode.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC2, Instr(Opcode.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC3, Instr(Opcode.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xC4, Instr(Opcode.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC5, Instr(Opcode.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC6, Instr(Opcode.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC7, Instr(Opcode.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xC8, Instr(Opcode.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC9, Instr(Opcode.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCA, Instr(Opcode.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCB, Instr(Opcode.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xCC, Instr(Opcode.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCD, Instr(Opcode.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCE, Instr(Opcode.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCF, Instr(Opcode.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD0, Instr(Opcode.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD1, Instr(Opcode.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD2, Instr(Opcode.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD3, Instr(Opcode.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD4, Instr(Opcode.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD5, Instr(Opcode.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD6, Instr(Opcode.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD7, Instr(Opcode.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD8, Instr(Opcode.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xD9, Instr(Opcode.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDA, Instr(Opcode.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDB, Instr(Opcode.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xDC, Instr(Opcode.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDD, Instr(Opcode.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDE, Instr(Opcode.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDF, Instr(Opcode.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xE0, Instr(Opcode.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE1, Instr(Opcode.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE2, Instr(Opcode.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE3, Instr(Opcode.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xE4, Instr(Opcode.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE5, Instr(Opcode.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE6, Instr(Opcode.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE7, Instr(Opcode.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32)))
                ),
                Mask(10, 2,
                    Instr(Opcode.if_ncc_jump, J(0,10,1)),
                    Instr(Opcode.if_ncc_jump_bp, J(0,10,1)),
                    Instr(Opcode.if_cc_jump, J(0,10,1)),
                    Instr(Opcode.if_cc_jump_bp, J(0,10,1))),
                Instr(InstrClass.Transfer, Opcode.JUMP_S, J12),
                Instr(Opcode.mov, R(Registers.AllReg, 9,3, 3,3), R(Registers.AllReg, 6,3,0,3)),
                
                // 4xxx
                Mask(6, 6,
                    Nyi("0b0100............"),
                    (0, Instr(Opcode.asr, D0,D3)),
                    (1, Instr(Opcode.lsr, D0,D3)),
                    (2, Instr(Opcode.lsl, D0,D3)),
                    (3, Instr(Opcode.mul, D0,D3)),

                    (4, Instr(Opcode.add_sh1, D0,D0,D3)),
                    (5, Instr(Opcode.add_sh2, D0,D0,D3)),

                    (8, Instr(Opcode.DIVQ, D0,D3)),
                    (9, Instr(Opcode.DIVS, D0,D3)),
                    (0b001010, Instr(Opcode.mov_xl, D0,D3)),
                    (0b001011, Instr(Opcode.mov_zl, D0,D3)),

                    (0b001100, Instr(Opcode.mov_xb, D0,D3)),
                    (0b001101, Instr(Opcode.mov_zb, D0,D3)),

                    (0b001110, Instr(Opcode.neg, D0,D3)),
                    (0b001111, Instr(Opcode.not, D0,D3)),

                    (0b010000, Instr(Opcode.sub, P0,P3)),
                    (0b010001, Instr(Opcode.lsl3, P0,P3,I(2,PrimitiveType.Byte))),

                    (0b010011, Instr(Opcode.lsr3, P0,P3,I(1,PrimitiveType.Byte))),

                    (0b010100, Instr(Opcode.lsr3, P0,P3,I(2,PrimitiveType.Byte))),

                    (0b100000, Instr(Opcode.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100001, Instr(Opcode.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100010, Instr(Opcode.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100011, Instr(Opcode.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b0101_10, Instr(Opcode.add_sh1, P0,P0,P3)),
                    (0b0101_11, Instr(Opcode.add_sh2, P0,P0,P3)),

                    (0b100100, Instr(Opcode.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100101, Instr(Opcode.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100110, Instr(Opcode.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100111, Instr(Opcode.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b101000, Instr(Opcode.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101001, Instr(Opcode.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101010, Instr(Opcode.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101011, Instr(Opcode.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b101100, Instr(Opcode.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101101, Instr(Opcode.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101110, Instr(Opcode.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101111, Instr(Opcode.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b110000, Instr(Opcode.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110001, Instr(Opcode.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110010, Instr(Opcode.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110011, Instr(Opcode.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b110100, Instr(Opcode.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110101, Instr(Opcode.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110110, Instr(Opcode.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110111, Instr(Opcode.asr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b111000, Instr(Opcode.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111001, Instr(Opcode.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111010, Instr(Opcode.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111011, Instr(Opcode.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b111100, Instr(Opcode.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111101, Instr(Opcode.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111110, Instr(Opcode.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111111, Instr(Opcode.lsl, D0,Imm(3, 5, PrimitiveType.Byte)))
                ),
                // 0x5...
                Mask(9, 3,
                    Instr(Opcode.add3, D6,D0,D3),
                    Instr(Opcode.sub3, D6,D0,D3),
                    Instr(Opcode.and3, D6,D0,D3),
                    Instr(Opcode.or3, D6,D0,D3),

                    Instr(Opcode.xor3, D6,D0,D3),
                    Instr(Opcode.add3, P6,P0,P3),
                    Instr(Opcode.shift1add, P6,P0,P3),
                    Instr(Opcode.shift2add, P6,P0,P3)),
                Mask(10, 2,
                    Instr(Opcode.mov, D0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Opcode.add, D0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Opcode.mov, P0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Opcode.add, P0,Imms(3,7,PrimitiveType.Word32))),
                Nyi("0b0111............"),

                // 8xxx
                Nyi("0b1000............"),
                // 9xxx
                Mask(6, 6,
                    Nyi("0b1001............"),
                    (0x00, Instr(Opcode.mov, D0,PostInc3)),
                    (0x01, Instr(Opcode.mov, P0,PostInc3)),
                    (0x02, Instr(Opcode.mov, D0,PostDec3)),
                    (0x03, Instr(Opcode.mov, P0,PostDec3)),

                    (0x04, Instr(Opcode.mov, D0,IP3)),
                    (0x05, Instr(Opcode.mov, P0,IP3)),

                    (0x08, Instr(Opcode.mov, PostInc3,D0)),
                    (0x09, Instr(Opcode.mov, PostInc3,P0)),
                    (0x0A, Instr(Opcode.mov, PostDec3,D0)),
                    (0x0B, Instr(Opcode.mov, PostDec3,P0)),

                    (0x0C, Instr(Opcode.mov, IP3,D0)),
                    (0x0D, Instr(Opcode.mov, IP3,P0)),

                    (0x10, Instr(Opcode.mov_z, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x11, Instr(Opcode.mov_x, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x12, Instr(Opcode.mov_z, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x13, Instr(Opcode.mov_x, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),

                    (0x14, Instr(Opcode.mov_z, D0,PtrInd(3, PrimitiveType.Word16))),
                    (0x15, Instr(Opcode.mov_x, D0,PtrInd(3, PrimitiveType.Word16))),

                    (0x18, Instr(Opcode.mov, PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte), D0)),
                    (0x1A, Instr(Opcode.mov, PostDec(Registers.Pointers, 3, 3, PrimitiveType.Byte), D0)),

                    (0x1C, Instr(Opcode.mov, PtrInd(3, PrimitiveType.Word16), D0)),

                    (0x20, Instr(Opcode.mov_z, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte))),
                    (0x21, Instr(Opcode.mov_x, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte))),

                    (0x22, Instr(Opcode.mov_zb, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x23, Instr(Opcode.mov_xb, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),

                    (0x24, Instr(Opcode.mov_z, D0,PtrInd(3, PrimitiveType.Byte))),
                    (0x25, Instr(Opcode.mov_x, D0,PtrInd(3, PrimitiveType.Byte))),

                    (0x28, Instr(Opcode.mov, PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte),D0)),

                    (0x2A, Instr(Opcode.mov, PostDec(Registers.Pointers, 3, 3, PrimitiveType.Byte),D0)),

                    (0x2C, Instr(Opcode.mov, IP3,D0)),

                    (0x30, Mask(5, 1,
                        Instr(Opcode.mov, D0,PostInc(Registers.Indices, 3, 2, PrimitiveType.Word32)),
                        Instr(Opcode.mov, R(Registers.RPIB_Lo,0,3),PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16)))),
                    (0x31, Mask(5, 1,
                        Instr(Opcode.mov, R(Registers.RPI_Hi,0,3),PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16)),
                        invalid)),
                    (0x32, Mask(5, 1,
                        Instr(Opcode.mov, D0,PostDec(Registers.Indices, 3, 2, PrimitiveType.Word32)),
                        Instr(Opcode.mov, R(Registers.RPIB_Lo,0,3),PostDec(Registers.Indices, 3, 2,PrimitiveType.Word16)))),
                    (0x33, Mask(5, 1,
                        Instr(Opcode.mov, R(Registers.RPI_Hi,0,3),PostDec(Registers.Indices, 3, 2, PrimitiveType.Word16)),
                        invalid)),
                    (0x38, Mask(5, 1,
                        Instr(Opcode.mov, D0, IdxInd(3, PrimitiveType.Word32)),
                        Instr(Opcode.mov, R(Registers.RPIB_Lo,0,3),IdxInd(3, PrimitiveType.Word16)))),
                    (0x3D, Mask(5, 1,
                        Instr(Opcode.mov, PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16), R(Registers.RPI_Hi,0,3)),
                        Instr(Opcode.add, R(Registers.Indices, 0, 2), R(Registers.Ms, 2, 2))))),
                // A...
                Mask(10, 2,
                    Instr(Opcode.mov, D0,MoffU(3, 6, 4, 2, PrimitiveType.Word32)),
                    Instr(Opcode.mov_z, D0,MoffU(3, 6, 4, 1, PrimitiveType.Word16)),
                    Instr(Opcode.mov_x, D0,MoffU(3, 6, 4, 1, PrimitiveType.Word16)),
                    Instr(Opcode.mov, P0,MoffU(3, 6, 4, 2, PrimitiveType.Word32))),
                // 0xB...
                Mask(9, 3,
                    Instr(Opcode.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),D0),
                    Instr(Opcode.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),D0),
                    Instr(Opcode.mov, MoffU(3, 6, 4, 1, PrimitiveType.Word16),D0),
                    Instr(Opcode.mov, MoffU(3, 6, 4, 1, PrimitiveType.Word16),D0),

                    Instr(Opcode.mov, R0,MfpOffU(4, 6, 2, PrimitiveType.Word32)),
                    Instr(Opcode.mov, MfpOffU(4, 6, 1, PrimitiveType.Word32),R0),
                    Instr(Opcode.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),P0),
                    Instr(Opcode.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),P0)),

                instr32,
                instr32,
                instr32,
                invalid,
            });
        }
    }
}