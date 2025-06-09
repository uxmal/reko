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
using System.Linq;

namespace Reko.Arch.Blackfin
{
    using Decoder = Decoder<BlackfinDisassembler, Mnemonic, BlackfinInstruction>;
    using Mutator = Mutator<BlackfinDisassembler>;

    public class BlackfinDisassembler : DisassemblerBase<BlackfinInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public BlackfinDisassembler(BlackfinArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override BlackfinInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            this.ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = (int)(rdr.Address - this.addr);
            return instr;
        }

        public override BlackfinInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new BlackfinInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override BlackfinInstruction CreateInvalidInstruction()
        {
            return new BlackfinInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
            }

        public override BlackfinInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("BlackfinDasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
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
                return dasm.NotYetImplemented(message);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator[] mutators)
        {
            return new InstrDecoder<BlackfinDisassembler,Mnemonic,BlackfinInstruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator[] mutators)
        {
            return new InstrDecoder<BlackfinDisassembler, Mnemonic, BlackfinInstruction>(InstrClass.Linear, mnemonic, mutators);
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
                if (iReg >= regs.Length)
                    return false;
                var reg = regs[iReg];
                if (reg is null)
                    return false;
                d.ops.Add(reg);
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
                if (reg is null)
                    return false;
                d.ops.Add(reg);
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
                if (flag is null)
                    return false;
                d.ops.Add(flag);
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
            d.ops.Add(Registers.A0);
            return true;
        }

        private static bool A1(uint u, BlackfinDisassembler d)
        {
            d.ops.Add(Registers.A1);
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
                d.ops.Add(c);
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
                d.ops.Add(c);
                return true;
            };
        }

        private static Mutator I(int n, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var c = Constant.Create(dt, n);
                d.ops.Add(c);
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
                d.ops.Add(c);
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
                d.ops.Add(c);
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
                d.ops.Add(d.addr + offset);
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
                var iregMin = (int)bitfield.Read(u);
                if (iregMin > iregMax)
                    return false;
                var bitsize = (iregMax - iregMin + 1) * Registers.Pointers[0].DataType.BitSize;
                var dt = PrimitiveType.CreateWord(bitsize);
                d.ops.Add(new RegisterRange(dt, regs, iregMax, iregMin));
                return true;
            };
        }

        private static Mutator J(int pos, int len, int sh)
        {
            var bitfield = new Bitfield(pos, len);
            return (u, d) =>
            {
                var pcOffset = bitfield.ReadSigned(u) << sh;
                d.ops.Add(d.addr + pcOffset);
                return true;
            };
        }
        #endregion

        private static bool IsZero(uint n) => n == 0;

        static BlackfinDisassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);
            var instr32 = Mask32(24, 8,
                Nyi(""),
                (0xC4, Mask(16, 8,
                    invalid,
                    (0x02, Mask(12, 4,
                        invalid,
                        (0, Instr(Mnemonic.add3, 
                            R(Registers.RPIB_Lo, 9, 3), 
                            R(Registers.RPIB_Lo, 3, 3), 
                            R(Registers.RPIB_Lo, 3, 3))))),
                    (0x22, Mask(12, 4,
                        invalid,
                        (4, Instr(Mnemonic.add3, R(Registers.RPI_Hi,9,3), R(Registers.RPIB_Lo,3,3), R(Registers.RPI_Hi,3,3))))))),
                (0xC6, Mask(16, 8,
                    invalid,
                    (0x82, Mask(12, 4,
                        invalid,
                        (0, Cond(8, 1, n => n == 0,
                            invalid,
                            Instr(Mnemonic.asr3, D9, D0, NImms(3, 5, PrimitiveType.Byte)))),
                        (8, Cond(8, 1, n => n == 0,
                            Instr(Mnemonic.lsl3, D9, D0, Imms(3, 5, PrimitiveType.Byte)),
                            Instr(Mnemonic.lsr3, D9, D0, NImms(3, 5, PrimitiveType.Byte)))))))),
                (0xC8, Cond(16, 8, n => n == 3, 
                    Instr(Mnemonic.MNOP, InstrClass.Padding | InstrClass.Linear),
                    invalid)),
                (0xE1, Mask(5 + 16, 2,
                    Instr(Mnemonic.mov, Rlo16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov_x, Rpib16, Imm(0, 16, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov, Rhi16, Imm(0, 16, PrimitiveType.Word16)),
                    Nyi(""))),
                (0xE2, Instr(Mnemonic.JUMP_L, InstrClass.Transfer, J(0, 24, 1))),
                (0xE3, Instr(Mnemonic.CALL, InstrClass.Transfer | InstrClass.Call, J(0, 24, 1))),

                (0xE4, Mask(6 + 16, 2,
                    Instr(Mnemonic.mov, D16, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32)),
                    Instr(Mnemonic.mov_z, D16, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov_z, D16, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte)),
                    invalid)),
                (0xE5, Mask(6 + 16, 2,
                    Instr(Mnemonic.mov, P16, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32)),
                    Instr(Mnemonic.mov_x, D16, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov_x, D16, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte)),
                    invalid)),
                (0xE6, Mask(6 + 16, 2,
                    Instr(Mnemonic.mov, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32), D16),
                    Instr(Mnemonic.mov, MoffS(3 + 16, 0, 16, 1, PrimitiveType.Word16), D16),
                    Instr(Mnemonic.mov, MoffS(3 + 16, 0, 16, 0, PrimitiveType.Byte), D16),
                    invalid)),
                (0xE7, Instr(Mnemonic.mov, MoffS(3 + 16, 0, 16, 2, PrimitiveType.Word32), P16)),

                (0xE8, Mask(16, 8,
                    invalid,
                    (0, Instr(Mnemonic.LINK, ImmSh(0, 16, 4, PrimitiveType.Int32))),
                    (1, Instr(Mnemonic.UNLINK)))));

            rootDecoder = Mask(12, 4, new Decoder[16]
            {
                Mask(4, 8,
                    Nyi("0b0000????????...."),
                    (0x00, Cond(0, 4, IsZero,
                        Instr(Mnemonic.NOP, InstrClass.Zero|InstrClass.Linear|InstrClass.Padding),
                        invalid)),
                    (0x01, Mask(0, 4, invalid,
                        (0x0, Instr(Mnemonic.RTS, InstrClass.Transfer|InstrClass.Return)),
                        (0x1, Instr(Mnemonic.RTI, InstrClass.Transfer|InstrClass.Return)),
                        (0x2, Instr(Mnemonic.RTX, InstrClass.Transfer|InstrClass.Return)),
                        (0x3, Instr(Mnemonic.RTN, InstrClass.Transfer|InstrClass.Return)),
                        (0x4, Instr(Mnemonic.RTE, InstrClass.Transfer|InstrClass.Return)))),
                    (0x02, Mask(0, 4, invalid,
                        (0x0, Instr(Mnemonic.IDLE)),
                        (0x3, Instr(Mnemonic.CSYNC)),
                        (0x4, Instr(Mnemonic.SSYNC)),
                        (0x5, Instr(Mnemonic.EMUEXCEPT)),
                        (0xF, Instr(Mnemonic.ABORT)))),
                    (0x03, Mask(3, 1,
                        Instr(Mnemonic.CLI, InstrClass.Privileged|InstrClass.Linear, D0),
                        invalid)),
                    (0x04, Mask(3, 1,
                        Instr(Mnemonic.STI, D0),
                        invalid)),
                    (0x05, Mask(3, 1,
                        Instr(Mnemonic.JUMP, InstrClass.Transfer, IP0),
                        invalid)),
                    (0x06, Mask(3, 1,
                        Instr(Mnemonic.CALL, InstrClass.Transfer, IP0),
                        invalid)),
                    (0x07, Mask(3, 1,
                        Instr(Mnemonic.CALL, InstrClass.Transfer, PCIX0),
                        invalid)),
                    (0x08, Mask(3, 1,
                        Instr(Mnemonic.JUMP, InstrClass.Transfer, PCIX0),
                        invalid)),
                    (0x09, Instr(Mnemonic.RAISE, InstrClass.Linear, Imm(0, 4, PrimitiveType.Byte))),
                    (0x0A, Instr(Mnemonic.EXCPT, InstrClass.Linear, Imm(0, 4, PrimitiveType.Byte))),
                    (0x0B, Nyi("TESTSET (Preg)")),

                    (0x10, invalid),
                    (0x11, Instr(Mnemonic.mov, RmostRegs0, SPpost)),
                    (0x12, Instr(Mnemonic.mov, RmostRegs0, SPpost)),
                    (0x13, Instr(Mnemonic.mov, RmostRegs0, SPpost)),

                    (0x14, Instr(Mnemonic.mov, SPpre, RallRegs0)),
                    (0x15, Instr(Mnemonic.mov, SPpre, RallRegs0)),
                    (0x16, Instr(Mnemonic.mov, SPpre, RallRegs0)),
                    (0x17, Instr(Mnemonic.mov, SPpre, RallRegs0)),

                    (0x18, Instr(Mnemonic.if_ncc_mov, D3,D0)),
                    (0x19, Instr(Mnemonic.if_ncc_mov, D3,P0)),
                    (0x1A, Instr(Mnemonic.if_ncc_mov, P3,D0)),
                    (0x1B, Instr(Mnemonic.if_ncc_mov, P3,P0)),

                    (0x1C, Instr(Mnemonic.if_cc_mov, D3,D0)),
                    (0x1D, Instr(Mnemonic.if_cc_mov, D3,P0)),
                    (0x1E, Instr(Mnemonic.if_cc_mov, P3,D0)),
                    (0x1F, Instr(Mnemonic.if_cc_mov, P3,P0)),

                    (0x20, Instr(Mnemonic.mov_cc_eq, D0,D3)),
                    (0x21, Cond(0, 4, n => n == 8,
                        Instr(Mnemonic.neg_cc),
                        invalid)),
                    (0x25, Mask(3, 1,
                        Instr(Mnemonic.flush, PtrInd(0, PrimitiveType.Word32)),
                        Instr(Mnemonic.iflush, PtrInd(0, PrimitiveType.Word32)))),
                    (0x26, Mask(3, 1,
                        Instr(Mnemonic.prefetch, Post0),
                        Instr(Mnemonic.flushinv, Post0))),
                    (0x27, Mask(3, 1,
                        Instr(Mnemonic.flush, Post0),
                        Instr(Mnemonic.iflush, Post0))),

                    (0x30, Instr(Mnemonic.mov_cc_eq, D0,Imms(3, 3, PrimitiveType.Word32))),
                    (0x38, Instr(Mnemonic.mov_r_cc, FlagGroup(0, 4))),
                    (0x48, Instr(Mnemonic.mov, Range0(5, Registers.Pointers), SPpost)),
                    (0x4C, Instr(Mnemonic.mov, SPpre, Range0(5, Registers.Pointers))),

                    (0x50, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x51, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x52, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x53, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),

                    (0x54, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x55, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x56, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x57, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),

                    (0x58, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x59, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x5A, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),
                    (0x5B, Instr(Mnemonic.mov, Range0(7, Registers.Pointers), SPpost)),

                    (0x5C, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5D, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5E, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),
                    (0x5F, Instr(Mnemonic.mov, SPpre, Range0(7, Registers.Pointers))),

                    (0x60, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x61, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x62, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x63, Instr(Mnemonic.if_ncc_mov, D3, D0)),

                    (0x64, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x65, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x66, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x67, Instr(Mnemonic.if_ncc_mov, D3, P0)),

                    (0x68, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x69, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x6A, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x6B, Instr(Mnemonic.if_ncc_mov, P3, D0)),

                    (0x6C, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x6D, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x6E, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x6F, Instr(Mnemonic.if_ncc_mov, P3, P0)),

                    (0x70, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x71, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x72, Instr(Mnemonic.if_ncc_mov, D3, D0)),
                    (0x73, Instr(Mnemonic.if_ncc_mov, D3, D0)),

                    (0x74, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x75, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x76, Instr(Mnemonic.if_ncc_mov, D3, P0)),
                    (0x77, Instr(Mnemonic.if_ncc_mov, D3, P0)),

                    (0x78, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x79, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x7A, Instr(Mnemonic.if_ncc_mov, P3, D0)),
                    (0x7B, Instr(Mnemonic.if_ncc_mov, P3, D0)),

                    (0x7C, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x7D, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x7E, Instr(Mnemonic.if_ncc_mov, P3, P0)),
                    (0x7F, Instr(Mnemonic.if_ncc_mov, P3, P0)),

                    (0x80, Instr(Mnemonic.mov_cc_eq, D3, D0)),
                    (0x81, Instr(Mnemonic.mov_cc_eq, D3, D0)),
                    (0x82, Instr(Mnemonic.mov_cc_eq, D3, D0)),
                    (0x83, Instr(Mnemonic.mov_cc_eq, D3, D0)),

                    (0x84, Instr(Mnemonic.mov_cc_eq, P3, P0)),
                    (0x85, Instr(Mnemonic.mov_cc_eq, P3, P0)),
                    (0x86, Instr(Mnemonic.mov_cc_eq, P3, P0)),
                    (0x87, Instr(Mnemonic.mov_cc_eq, P3, P0)),

                    (0x88, Instr(Mnemonic.mov_cc_lt, D3, D0)),
                    (0x89, Instr(Mnemonic.mov_cc_lt, D3, D0)),
                    (0x8A, Instr(Mnemonic.mov_cc_lt, D3, D0)),
                    (0x8B, Instr(Mnemonic.mov_cc_lt, D3, D0)),

                    (0x8C, Instr(Mnemonic.mov_cc_lt, P3, P0)),
                    (0x8D, Instr(Mnemonic.mov_cc_lt, P3, P0)),
                    (0x8E, Instr(Mnemonic.mov_cc_lt, P3, P0)),
                    (0x8F, Instr(Mnemonic.mov_cc_lt, P3, P0)),

                    (0x90, Instr(Mnemonic.mov_cc_le, D3, D0)),
                    (0x91, Instr(Mnemonic.mov_cc_le, D3, D0)),
                    (0x92, Instr(Mnemonic.mov_cc_le, D3, D0)),
                    (0x93, Instr(Mnemonic.mov_cc_le, D3, D0)),

                    (0x94, Instr(Mnemonic.mov_cc_le, P3, P0)),
                    (0x95, Instr(Mnemonic.mov_cc_le, P3, P0)),
                    (0x96, Instr(Mnemonic.mov_cc_le, P3, P0)),
                    (0x97, Instr(Mnemonic.mov_cc_le, P3, P0)),

                    (0x98, Instr(Mnemonic.mov_cc_ult, D3, D0)),
                    (0x99, Instr(Mnemonic.mov_cc_ult, D3, D0)),
                    (0x9A, Instr(Mnemonic.mov_cc_ult, D3, D0)),
                    (0x9B, Instr(Mnemonic.mov_cc_ult, D3, D0)),

                    (0x9C, Instr(Mnemonic.mov_cc_ult, P3, P0)),
                    (0x9D, Instr(Mnemonic.mov_cc_ult, P3, P0)),
                    (0x9E, Instr(Mnemonic.mov_cc_ult, P3, P0)),
                    (0x9F, Instr(Mnemonic.mov_cc_ult, P3, P0)),

                    (0xA0, Instr(Mnemonic.mov_cc_ule, D3, D0)),
                    (0xA1, Instr(Mnemonic.mov_cc_ule, D3, D0)),
                    (0xA2, Instr(Mnemonic.mov_cc_ule, D3, D0)),
                    (0xA3, Instr(Mnemonic.mov_cc_ule, D3, D0)),

                    (0xA4, Instr(Mnemonic.mov_cc_ule, P3, P0)),
                    (0xA5, Instr(Mnemonic.mov_cc_ule, P3, P0)),
                    (0xA6, Instr(Mnemonic.mov_cc_ule, P3, P0)),
                    (0xA7, Instr(Mnemonic.mov_cc_ule, P3, P0)),

                    (0xA8, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xA9, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xAA, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xAB, Instr(Mnemonic.mov_cc_eq, A0, A1)),

                    (0xAC, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xAD, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xAE, Instr(Mnemonic.mov_cc_eq, A0, A1)),
                    (0xAF, Instr(Mnemonic.mov_cc_eq, A0, A1)),

                    (0xB8, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xB9, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xBA, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xBB, Instr(Mnemonic.mov_cc_lt, A0, A1)),

                    (0xBC, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xBD, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xBE, Instr(Mnemonic.mov_cc_lt, A0, A1)),
                    (0xBF, Instr(Mnemonic.mov_cc_lt, A0, A1)),

                    (0xC0, Instr(Mnemonic.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC1, Instr(Mnemonic.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC2, Instr(Mnemonic.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC3, Instr(Mnemonic.mov_cc_eq, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xC4, Instr(Mnemonic.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC5, Instr(Mnemonic.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC6, Instr(Mnemonic.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC7, Instr(Mnemonic.mov_cc_eq, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xC8, Instr(Mnemonic.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xC9, Instr(Mnemonic.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCA, Instr(Mnemonic.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCB, Instr(Mnemonic.mov_cc_lt, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xCC, Instr(Mnemonic.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCD, Instr(Mnemonic.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCE, Instr(Mnemonic.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xCF, Instr(Mnemonic.mov_cc_lt, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD0, Instr(Mnemonic.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD1, Instr(Mnemonic.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD2, Instr(Mnemonic.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD3, Instr(Mnemonic.mov_cc_le, D0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD4, Instr(Mnemonic.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD5, Instr(Mnemonic.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD6, Instr(Mnemonic.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),
                    (0xD7, Instr(Mnemonic.mov_cc_le, P0, Imms(0, 3, PrimitiveType.Word32))),

                    (0xD8, Instr(Mnemonic.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xD9, Instr(Mnemonic.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDA, Instr(Mnemonic.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDB, Instr(Mnemonic.mov_cc_ult, D0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xDC, Instr(Mnemonic.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDD, Instr(Mnemonic.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDE, Instr(Mnemonic.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xDF, Instr(Mnemonic.mov_cc_ult, P0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xE0, Instr(Mnemonic.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE1, Instr(Mnemonic.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE2, Instr(Mnemonic.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE3, Instr(Mnemonic.mov_cc_ule, D0, Imm(0, 3, PrimitiveType.Word32))),

                    (0xE4, Instr(Mnemonic.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE5, Instr(Mnemonic.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE6, Instr(Mnemonic.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32))),
                    (0xE7, Instr(Mnemonic.mov_cc_ule, P0, Imm(0, 3, PrimitiveType.Word32)))
                ),
                Mask(10, 2,
                    Instr(Mnemonic.if_ncc_jump, J(0,10,1)),
                    Instr(Mnemonic.if_ncc_jump_bp, J(0,10,1)),
                    Instr(Mnemonic.if_cc_jump, J(0,10,1)),
                    Instr(Mnemonic.if_cc_jump_bp, J(0,10,1))),
                Instr(Mnemonic.JUMP_S, InstrClass.Transfer, J12),
                Instr(Mnemonic.mov, R(Registers.AllReg, 9,3, 3,3), R(Registers.AllReg, 6,3,0,3)),
                
                // 4xxx
                Mask(6, 6,
                    Nyi("0b0100............"),
                    (0, Instr(Mnemonic.asr, D0,D3)),
                    (1, Instr(Mnemonic.lsr, D0,D3)),
                    (2, Instr(Mnemonic.lsl, D0,D3)),
                    (3, Instr(Mnemonic.mul, D0,D3)),

                    (4, Instr(Mnemonic.add_sh1, D0,D0,D3)),
                    (5, Instr(Mnemonic.add_sh2, D0,D0,D3)),

                    (8, Instr(Mnemonic.DIVQ, D0,D3)),
                    (9, Instr(Mnemonic.DIVS, D0,D3)),
                    (0b001010, Instr(Mnemonic.mov_xl, D0,D3)),
                    (0b001011, Instr(Mnemonic.mov_zl, D0,D3)),

                    (0b001100, Instr(Mnemonic.mov_xb, D0,D3)),
                    (0b001101, Instr(Mnemonic.mov_zb, D0,D3)),

                    (0b001110, Instr(Mnemonic.neg, D0,D3)),
                    (0b001111, Instr(Mnemonic.not, D0,D3)),

                    (0b010000, Instr(Mnemonic.sub, P0,P3)),
                    (0b010001, Instr(Mnemonic.lsl3, P0,P3,I(2,PrimitiveType.Byte))),

                    (0b010011, Instr(Mnemonic.lsr3, P0,P3,I(1,PrimitiveType.Byte))),

                    (0b010100, Instr(Mnemonic.lsr3, P0,P3,I(2,PrimitiveType.Byte))),

                    (0b100000, Instr(Mnemonic.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100001, Instr(Mnemonic.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100010, Instr(Mnemonic.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100011, Instr(Mnemonic.mov_cc_n_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b0101_10, Instr(Mnemonic.add_sh1, P0,P0,P3)),
                    (0b0101_11, Instr(Mnemonic.add_sh2, P0,P0,P3)),

                    (0b100100, Instr(Mnemonic.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100101, Instr(Mnemonic.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100110, Instr(Mnemonic.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b100111, Instr(Mnemonic.mov_cc_bittest, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b101000, Instr(Mnemonic.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101001, Instr(Mnemonic.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101010, Instr(Mnemonic.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101011, Instr(Mnemonic.bitset, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b101100, Instr(Mnemonic.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101101, Instr(Mnemonic.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101110, Instr(Mnemonic.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b101111, Instr(Mnemonic.bittgl, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b110000, Instr(Mnemonic.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110001, Instr(Mnemonic.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110010, Instr(Mnemonic.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110011, Instr(Mnemonic.bitclr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b110100, Instr(Mnemonic.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110101, Instr(Mnemonic.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110110, Instr(Mnemonic.asr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b110111, Instr(Mnemonic.asr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b111000, Instr(Mnemonic.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111001, Instr(Mnemonic.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111010, Instr(Mnemonic.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111011, Instr(Mnemonic.lsr, D0,Imm(3, 5, PrimitiveType.Byte))),

                    (0b111100, Instr(Mnemonic.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111101, Instr(Mnemonic.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111110, Instr(Mnemonic.lsl, D0,Imm(3, 5, PrimitiveType.Byte))),
                    (0b111111, Instr(Mnemonic.lsl, D0,Imm(3, 5, PrimitiveType.Byte)))
                ),
                // 0x5...
                Mask(9, 3,
                    Instr(Mnemonic.add3, D6,D0,D3),
                    Instr(Mnemonic.sub3, D6,D0,D3),
                    Instr(Mnemonic.and3, D6,D0,D3),
                    Instr(Mnemonic.or3, D6,D0,D3),

                    Instr(Mnemonic.xor3, D6,D0,D3),
                    Instr(Mnemonic.add3, P6,P0,P3),
                    Instr(Mnemonic.shift1add, P6,P0,P3),
                    Instr(Mnemonic.shift2add, P6,P0,P3)),
                Mask(10, 2,
                    Instr(Mnemonic.mov, D0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Mnemonic.add, D0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Mnemonic.mov, P0,Imms(3,7,PrimitiveType.Word32)),
                    Instr(Mnemonic.add, P0,Imms(3,7,PrimitiveType.Word32))),
                Nyi("0b0111............"),

                // 8xxx
                Nyi("0b1000............"),
                // 9xxx
                Mask(6, 6,
                    Nyi("0b1001............"),
                    (0x00, Instr(Mnemonic.mov, D0,PostInc3)),
                    (0x01, Instr(Mnemonic.mov, P0,PostInc3)),
                    (0x02, Instr(Mnemonic.mov, D0,PostDec3)),
                    (0x03, Instr(Mnemonic.mov, P0,PostDec3)),

                    (0x04, Instr(Mnemonic.mov, D0,IP3)),
                    (0x05, Instr(Mnemonic.mov, P0,IP3)),

                    (0x08, Instr(Mnemonic.mov, PostInc3,D0)),
                    (0x09, Instr(Mnemonic.mov, PostInc3,P0)),
                    (0x0A, Instr(Mnemonic.mov, PostDec3,D0)),
                    (0x0B, Instr(Mnemonic.mov, PostDec3,P0)),

                    (0x0C, Instr(Mnemonic.mov, IP3,D0)),
                    (0x0D, Instr(Mnemonic.mov, IP3,P0)),

                    (0x10, Instr(Mnemonic.mov_z, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x11, Instr(Mnemonic.mov_x, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x12, Instr(Mnemonic.mov_z, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x13, Instr(Mnemonic.mov_x, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),

                    (0x14, Instr(Mnemonic.mov_z, D0,PtrInd(3, PrimitiveType.Word16))),
                    (0x15, Instr(Mnemonic.mov_x, D0,PtrInd(3, PrimitiveType.Word16))),

                    (0x18, Instr(Mnemonic.mov, PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte), D0)),
                    (0x1A, Instr(Mnemonic.mov, PostDec(Registers.Pointers, 3, 3, PrimitiveType.Byte), D0)),

                    (0x1C, Instr(Mnemonic.mov, PtrInd(3, PrimitiveType.Word16), D0)),

                    (0x20, Instr(Mnemonic.mov_z, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte))),
                    (0x21, Instr(Mnemonic.mov_x, D0,PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte))),

                    (0x22, Instr(Mnemonic.mov_zb, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),
                    (0x23, Instr(Mnemonic.mov_xb, D0,PostDec(Registers.Pointers, 3, 3, PrimitiveType.Word16))),

                    (0x24, Instr(Mnemonic.mov_z, D0,PtrInd(3, PrimitiveType.Byte))),
                    (0x25, Instr(Mnemonic.mov_x, D0,PtrInd(3, PrimitiveType.Byte))),

                    (0x28, Instr(Mnemonic.mov, PostInc(Registers.Pointers, 3, 3, PrimitiveType.Byte),D0)),

                    (0x2A, Instr(Mnemonic.mov, PostDec(Registers.Pointers, 3, 3, PrimitiveType.Byte),D0)),

                    (0x2C, Instr(Mnemonic.mov, IP3,D0)),

                    (0x30, Mask(5, 1,
                        Instr(Mnemonic.mov, D0,PostInc(Registers.Indices, 3, 2, PrimitiveType.Word32)),
                        Instr(Mnemonic.mov, R(Registers.RPIB_Lo,0,3),PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16)))),
                    (0x31, Mask(5, 1,
                        Instr(Mnemonic.mov, R(Registers.RPI_Hi,0,3),PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16)),
                        invalid)),
                    (0x32, Mask(5, 1,
                        Instr(Mnemonic.mov, D0,PostDec(Registers.Indices, 3, 2, PrimitiveType.Word32)),
                        Instr(Mnemonic.mov, R(Registers.RPIB_Lo,0,3),PostDec(Registers.Indices, 3, 2,PrimitiveType.Word16)))),
                    (0x33, Mask(5, 1,
                        Instr(Mnemonic.mov, R(Registers.RPI_Hi,0,3),PostDec(Registers.Indices, 3, 2, PrimitiveType.Word16)),
                        invalid)),
                    (0x38, Mask(5, 1,
                        Instr(Mnemonic.mov, D0, IdxInd(3, PrimitiveType.Word32)),
                        Instr(Mnemonic.mov, R(Registers.RPIB_Lo,0,3),IdxInd(3, PrimitiveType.Word16)))),
                    (0x3D, Mask(5, 1,
                        Instr(Mnemonic.mov, PostInc(Registers.Indices, 3, 2, PrimitiveType.Word16), R(Registers.RPI_Hi,0,3)),
                        Instr(Mnemonic.add, R(Registers.Indices, 0, 2), R(Registers.Ms, 2, 2))))),
                // A...
                Mask(10, 2,
                    Instr(Mnemonic.mov, D0,MoffU(3, 6, 4, 2, PrimitiveType.Word32)),
                    Instr(Mnemonic.mov_z, D0,MoffU(3, 6, 4, 1, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov_x, D0,MoffU(3, 6, 4, 1, PrimitiveType.Word16)),
                    Instr(Mnemonic.mov, P0,MoffU(3, 6, 4, 2, PrimitiveType.Word32))),
                // 0xB...
                Mask(9, 3,
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),D0),
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),D0),
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 1, PrimitiveType.Word16),D0),
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 1, PrimitiveType.Word16),D0),

                    Instr(Mnemonic.mov, R0,MfpOffU(4, 6, 2, PrimitiveType.Word32)),
                    Instr(Mnemonic.mov, MfpOffU(4, 6, 1, PrimitiveType.Word32),R0),
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),P0),
                    Instr(Mnemonic.mov, MoffU(3, 6, 4, 2, PrimitiveType.Word32),P0)),

                instr32,
                instr32,
                instr32,
                invalid,
            });
        }
    }
}