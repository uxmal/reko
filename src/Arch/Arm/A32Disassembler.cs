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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;

namespace Reko.Arch.Arm
{
    public class A32Disassembler : DisassemblerBase<Arm32InstructionNew>
    {
        private static readonly Decoder rootDecoder;

        private Arm32Architecture arch;
        private EndianImageReader rdr;

        public A32Disassembler(Arm32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Arm32InstructionNew DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            var instrs = rootDecoder.Decode(wInstr, this);
            throw new NotImplementedException();
        }

        private static uint bitmask(uint u, int shift, uint mask)
        {
            return (u >> shift) & mask;
        }

        private abstract class Decoder
        {
            public abstract Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            private Opcode opcode;
            private string format;

            public  InstrDecoder(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class MaskDecoder : Decoder
        {
            private int shift;
            private uint mask;
            private Decoder[] decoders;

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
            {
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = (wInstr >> shift) & mask;
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                var shMask = this.mask << shift;
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
                Debug.Print(sb.ToString());
            }
        }

        // Special decoder for when a 4-bit field has the bit pattern 1111 or not.
        private class PcDecoder : Decoder
        {
            private int shift;
            private Decoder not1111;
            private Decoder is1111;

            public PcDecoder(int shift, Decoder not1111, Decoder is1111)
            {
                this.shift = shift;
                this.not1111 = not1111;
                this.is1111 = is1111;
            }

            public override Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> shift) & 0xF;
                if (op == 0xF)
                    return is1111.Decode(wInstr, dasm);
                else
                    return not1111.Decode(wInstr, dasm);
            }
        }

        class CustomDecoder : Decoder
        {
            private Func<uint, A32Disassembler, Decoder> decode;

            public CustomDecoder(Func<uint, A32Disassembler, Decoder> decode)
            {
                this.decode = decode;
            }

            public override Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm)
            {
                return decode(wInstr, dasm).Decode(wInstr, dasm);
            }
        }

        private class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override Arm32InstructionNew Decode(uint wInstr, A32Disassembler dasm)
            {
                throw new NotImplementedException($"A T32 decoder for the instruction {wInstr:X} ({message}) has not been implemented.");
            }
        }

        private static NyiDecoder nyi(string str)
        {
            return new NyiDecoder(str);
        }


        static A32Disassembler()
        {
            var invalid = new InstrDecoder(Opcode.Invalid, "");

            var LoadStoreExclusive = nyi("LoadStoreExclusive");

            var Stl = new InstrDecoder(Opcode.stl, "");
            var Stlex = new InstrDecoder(Opcode.stlex, "");
            var Strex = new InstrDecoder(Opcode.strex, "");
            var Lda = new InstrDecoder(Opcode.lda, "");
            var Ldaex = new InstrDecoder(Opcode.ldaex, "");
            var Ldrex = new InstrDecoder(Opcode.ldrex, "");
            var Stlexd = new InstrDecoder(Opcode.stlexd, "");
            var Strexd = new InstrDecoder(Opcode.strexd, "");
            var Ldaexd = new InstrDecoder(Opcode.ldaexd, "");
            var Ldrexd = new InstrDecoder(Opcode.ldrexd, "");
            var Stlb = new InstrDecoder(Opcode.stlb, "");
            var Stlexb = new InstrDecoder(Opcode.stlexb, "");
            var Strexb = new InstrDecoder(Opcode.strexb, "");
            var Ldab = new InstrDecoder(Opcode.ldab, "");
            var Ldaexb = new InstrDecoder(Opcode.ldrexb, "");
            var Ldrexb = new InstrDecoder(Opcode.ldaexb, "");
            var Stlh = new InstrDecoder(Opcode.stlh, "");
            var Stlexh = new InstrDecoder(Opcode.stlexh, "");
            var Strexh = new InstrDecoder(Opcode.strexh, "");
            var Ldah = new InstrDecoder(Opcode.ldah, "");
            var Ldaexh = new InstrDecoder(Opcode.ldrexh, "");
            var Ldrexh = new InstrDecoder(Opcode.ldaexh, "");

            var SynchronizationPrimitives = new MaskDecoder(23, 1,
            invalid,
            new MaskDecoder(20, 7,  // type || L
                new MaskDecoder(8, 3,   // ex ord
                    Stl,
                    invalid,
                    Stlex,
                    Strex),
                new MaskDecoder(8, 3,   // ex ord
                    Lda,
                    invalid,
                    Ldaex,
                    Ldrex),
                new MaskDecoder(8, 3,   // ex ord
                    invalid,
                    invalid,
                    Stlexd,
                    Strexd),
                new MaskDecoder(8, 3,   // ex ord
                    invalid,
                    invalid,
                    Ldaexd,
                    Ldrexd),

                new MaskDecoder(8, 3,   // ex ord
                    Stlb,
                    invalid,
                    Stlexb,
                    Strexb),
                new MaskDecoder(8, 3,   // ex ord
                    Ldab,
                    invalid,
                    Ldaexb,
                    Ldrexb),
                new MaskDecoder(8, 3,   // ex ord
                    Stlh,
                    invalid,
                    Stlexh,
                    Strexh),
                new MaskDecoder(8, 3,   // ex ord
                    Ldah,
                    invalid,
                    Ldaexh,
                    Ldrexh)),
                LoadStoreExclusive);


            var Mul = new InstrDecoder(Opcode.mul, "");
            var Mla = new InstrDecoder(Opcode.mla, "");
            var Umaal = new InstrDecoder(Opcode.umaal, "");
            var Mls = new InstrDecoder(Opcode.mls, "");
            var Umull = new InstrDecoder(Opcode.umull, "");
            var Umlal = new InstrDecoder(Opcode.umlal, "");
            var Smull = new InstrDecoder(Opcode.smull, "");
            var Smlal = new InstrDecoder(Opcode.smlal, "");

            var MultiplyAndAccumulate = new MaskDecoder(20, 4,
               Mul,
               Mul,
               Mla,
               Mla,

               Umaal,
               invalid,
               Mls,
               invalid,

               Umull,
               Umull,
               Umlal,
               Umlal,

               Smull,
               Smlal);

            // --
            var LdrdRegister = new InstrDecoder(Opcode.ldrd, "");
            var LdrhRegister = new InstrDecoder(Opcode.ldrh, "");
            var LdrsbRegister = new InstrDecoder(Opcode.ldrsb, "");
            var LdrshRegister = new InstrDecoder(Opcode.ldrsh, "");
            var Ldrht = new InstrDecoder(Opcode.ldrht, "");
            var Ldrsbt = new InstrDecoder(Opcode.ldrsbt, "");
            var Ldrsht = new InstrDecoder(Opcode.ldrsht, "");
            var StrdRegister = new InstrDecoder(Opcode.strd, "");
            var StrhRegister = new InstrDecoder(Opcode.strh, "");
            var Strht = new InstrDecoder(Opcode.strht, "");

            var LoadStoreDualHalfSbyteRegister = new MaskDecoder(24, 1,
                new MaskDecoder(20, 0x3,
                   new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid),
                    new MaskDecoder(5, 3,
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht)),
                new MaskDecoder(20, 1,
                    new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister)));

            var LdrdLiteral = new InstrDecoder(Opcode.ldrd, "");
            var LdrhLiteral = new InstrDecoder(Opcode.ldrh, "");
            var LdrsbLiteral = new InstrDecoder(Opcode.ldrsb, "");
            var LdrshLiteral = new InstrDecoder(Opcode.ldrsh, "");
            var StrhImmediate = new InstrDecoder(Opcode.strh, "");
            var LdrdImmediate = new InstrDecoder(Opcode.ldrd, "");
            var StrdImmediate = new InstrDecoder(Opcode.strd, "");
            var LdrhImmediate = new InstrDecoder(Opcode.ldrh, "");
            var LdrsbImmediate = new InstrDecoder(Opcode.ldrsb, "");
            var LdrshImmediate = new InstrDecoder(Opcode.ldrsh, "");

            var LoadStoreDualHalfSbyteImmediate = new CustomDecoder((wInstr, dasm) =>
            {
                var rn = bitmask(wInstr, 16, 0xF);
                var pw = bitmask(wInstr, 23, 0x10) | bitmask(wInstr, 21, 1);
                var o1 = bitmask(wInstr, 20, 1);
                var op2 = bitmask(wInstr, 5, 3);
                if (rn == 0xF)
                {
                    if (o1 == 0)
                    {
                        if (op2 == 2)
                            return LdrdLiteral;
                    }
                    else
                    {
                        if (pw != 1)
                        {
                            new MaskDecoder(5, 3,
                                invalid,
                                LdrhLiteral,
                                LdrsbLiteral,
                                LdrshLiteral);
                        }
                        else
                        {
                            return invalid;
                        }
                    }
                }
                switch ((pw << 1) | o1)
                {
                case 0:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 1:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 2:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 3:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 4:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 5:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 6:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 7:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                }
                throw new InvalidOperationException("Impossible");
            });

            var ExtraLoadStore = new MaskDecoder(22, 1,
                LoadStoreDualHalfSbyteRegister,
                LoadStoreDualHalfSbyteImmediate);

            var Mrs = new InstrDecoder(Opcode.mrs, "");
            var Msr = new InstrDecoder(Opcode.msr, "");
            var MrsBanked = new InstrDecoder(Opcode.mrs, "");
            var MsrBanked = new InstrDecoder(Opcode.msr, "");
            var MoveSpecialRegister = new MaskDecoder(21, 1,
                new MaskDecoder(9, 1,
                    Mrs,
                    MrsBanked),
                new MaskDecoder(9, 1,
                    Msr,
                    MsrBanked));

            var Crc32 = new InstrDecoder(Opcode.crc32w, "");
            var Crc32C = new InstrDecoder(Opcode.crc32cw, "");

            var CyclicRedundancyCheck = new MaskDecoder(21, 3,
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C));

            var Qadd = new InstrDecoder(Opcode.qadd, "");
            var Qsub = new InstrDecoder(Opcode.qsub, "");
            var Qdadd = new InstrDecoder(Opcode.qdadd, "");
            var Qdsub = new InstrDecoder(Opcode.qdsub, "");
            var IntegerSaturatingArithmetic = new MaskDecoder(21, 3,
                Qadd,
                Qsub,
                Qdadd,
                Qsub);


            var Hlt = new InstrDecoder(Opcode.hlt, "");
            var Bkpt = new InstrDecoder(Opcode.bkpt, "");
            var Hvc = new InstrDecoder(Opcode.hvc, "");
            var Smc = new InstrDecoder(Opcode.smc, "");
            var ExceptionGeneration = new MaskDecoder(21, 3,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = new InstrDecoder(Opcode.bx, "");
            var Bxj = new InstrDecoder(Opcode.bxj, "");
            var Blx = new InstrDecoder(Opcode.blx, "");
            var Clz = new InstrDecoder(Opcode.blx, "");
            var Eret = new InstrDecoder(Opcode.eret, "");
            var Miscellaneous = new MaskDecoder(21, 3,   // op0
                new MaskDecoder(4, 3, // op1
                    MoveSpecialRegister,
                    invalid,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 3, // op1
                    MoveSpecialRegister,
                    Bx,
                    Bxj,
                    Blx,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 3, // op1
                    MoveSpecialRegister,
                    invalid,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 3, // op1
                    MoveSpecialRegister,
                    Clz,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    Eret,
                    ExceptionGeneration));

            var HalfwordMultiplyAndAccumulate = new MaskDecoder(21, 0x3,
                nyi("SmlabbSmlabtSmlatbSmlatt"),
                new MaskDecoder(5, 2,
                    nyi("SmlawbSmlawt"),
                    nyi("SmulwbSmulwt"),
                    nyi("SmlawbSmlawt"),
                    nyi("SmulwbSmulwt")),
                nyi("SmlalbbSmlalbt"),
                nyi("SmulbbSmulbt"));

            var IntegerDataProcessingImmShift = new MaskDecoder(21, 7,
                new InstrDecoder(Opcode.and, ""),
                new InstrDecoder(Opcode.eor, ""),
                new InstrDecoder(Opcode.sub, ""),
                new InstrDecoder(Opcode.rsb, ""),
                new InstrDecoder(Opcode.add, ""),
                new InstrDecoder(Opcode.adc, ""),
                new InstrDecoder(Opcode.sbc, ""),
                new InstrDecoder(Opcode.rsc, ""));

            var IntegerTestAndCompareImmShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, ""),
                new InstrDecoder(Opcode.teq, ""),
                new InstrDecoder(Opcode.cmp, ""),
                new InstrDecoder(Opcode.cmn, ""));

            var LogicalArithmeticImmShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, ""),
                new InstrDecoder(Opcode.mov, ""),
                new InstrDecoder(Opcode.bic, ""),
                new InstrDecoder(Opcode.mvn, ""));

            var DataProcessingImmediateShift = new MaskDecoder(23, 2,
                IntegerDataProcessingImmShift, // 3 reg, imm shift
                IntegerDataProcessingImmShift,
                IntegerTestAndCompareImmShift,
                LogicalArithmeticImmShift);

            var IntegerDataProcessingRegShift = new MaskDecoder(21, 7,
               new InstrDecoder(Opcode.and, ""),
               new InstrDecoder(Opcode.eor, ""),
               new InstrDecoder(Opcode.sub, ""),
               new InstrDecoder(Opcode.rsb, ""),
               new InstrDecoder(Opcode.add, ""),
               new InstrDecoder(Opcode.adc, ""),
               new InstrDecoder(Opcode.sbc, ""),
               new InstrDecoder(Opcode.rsc, ""));

            var IntegerTestAndCompareRegShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, ""),
                new InstrDecoder(Opcode.teq, ""),
                new InstrDecoder(Opcode.cmp, ""),
                new InstrDecoder(Opcode.cmn, ""));

            var LogicalArithmeticRegShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, ""),
                new InstrDecoder(Opcode.mov, ""),
                new InstrDecoder(Opcode.bic, ""),
                new InstrDecoder(Opcode.mvn, ""));

            var DataProcessingRegisterShift = new MaskDecoder(23, 3,
                IntegerDataProcessingRegShift,
                IntegerDataProcessingRegShift,
                IntegerTestAndCompareRegShift,
                LogicalArithmeticRegShift);

            var IntegerDataProcessingTwoRegImm = new MaskDecoder(21, 7,
               new InstrDecoder(Opcode.and, ""),
               new InstrDecoder(Opcode.eor, ""),
               new InstrDecoder(Opcode.sub, ""),
               new InstrDecoder(Opcode.rsb, ""),
               new InstrDecoder(Opcode.add, ""),
               new InstrDecoder(Opcode.adc, ""),
               new InstrDecoder(Opcode.sbc, ""),
               new InstrDecoder(Opcode.rsc, ""));

            var MoveHalfwordImm = new MaskDecoder(22, 1,
               new InstrDecoder(Opcode.movs, ""),
               new InstrDecoder(Opcode.movt, ""));

            var IntegerTestAndCompareOneRegImm = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, ""),
                new InstrDecoder(Opcode.mov, ""),
                new InstrDecoder(Opcode.bic, ""),
                new InstrDecoder(Opcode.mvn, ""));

            var MsrImmediate = new InstrDecoder(Opcode.msr, "");
            var Nop = new InstrDecoder(Opcode.nop, "");
            var Yield = new InstrDecoder(Opcode.yield, "");
            var Wfe = new InstrDecoder(Opcode.wfe, "");
            var Wfi = new InstrDecoder(Opcode.wfi, "");
            var Sev = new InstrDecoder(Opcode.sevl, "");
            var Sevl = new InstrDecoder(Opcode.sevl, "");
            var ReservedNop = new InstrDecoder(Opcode.nop, "");
            var Esb = new InstrDecoder(Opcode.esb, "");
            var Dbg = new InstrDecoder(Opcode.dbg, "");

            var MoveSpecialRegisterAndHints = new CustomDecoder((wInstr, dasm) =>
            {
                var imm12 = bitmask(wInstr, 0, 0xFF);
                var imm4 = bitmask(wInstr, 16, 0xF);
                var r_iim4 = (bitmask(wInstr, 22, 1) << 4) | imm4;
                if (r_iim4 != 0)
                    return MsrImmediate;
                switch (imm12 >> 4)
                {
                case 0:
                    switch (imm12 & 0xF)
                    {
                    case 0: return Nop;
                    case 1: return Yield;
                    case 2: return Wfe;
                    case 3: return Wfi;
                    case 4: return Sev;
                    case 5: return Sevl;
                    default: return ReservedNop;
                    }
                case 1:
                    switch (imm12 & 0x0F)
                    {
                    case 0: return Esb;
                    default: return ReservedNop;
                    }
                case 0xF: return Dbg;
                default: return ReservedNop;
                }
            });

            var DataProcessingImmediate = new MaskDecoder(23, 3,
                IntegerDataProcessingTwoRegImm,
                IntegerDataProcessingTwoRegImm,
                new MaskDecoder(20, 3,
                    MoveHalfwordImm,
                    IntegerTestAndCompareOneRegImm,
                    MoveSpecialRegisterAndHints,
                    IntegerTestAndCompareOneRegImm),
                nyi("LogicalArithmeticTwoRegImm"));

            var DataProcessingAndMisc = new CustomDecoder((wInstr, dasm) =>
            {
                if (bitmask(wInstr, 25, 1) == 0)
                {
                    var op1 = bitmask(wInstr, 20, 0x1F);
                    var op2 = bitmask(wInstr, 7, 1);
                    var op3 = bitmask(wInstr, 5, 3);
                    var op4 = bitmask(wInstr, 6, 1);
                    if (op2 == 1 && op4 == 1)
                    {
                        if (op3 == 0)
                        {
                            if (op1 < 0x10)
                                return MultiplyAndAccumulate;
                            else
                                return SynchronizationPrimitives;
                        }
                        else
                        {
                            return ExtraLoadStore;
                        }
                    }
                    if ((op1 & 0x19) == 0x10)
                    {
                        if (op2 == 0)
                            return Miscellaneous;
                        else if (op2 == 1 && op4 == 0)
                            return HalfwordMultiplyAndAccumulate;
                        else
                            return nyi("DataProcessingAndMisc");
                    }
                    else
                    {
                        if (op4 == 0)
                            return DataProcessingImmediateShift;
                        else if (op2 == 0 && op4 == 1)
                            return DataProcessingRegisterShift;
                        else
                            return nyi("DataProcessingAndMisc");
                    }
                }
                else
                {
                    return DataProcessingImmediate;
                }
            });

            var LdrLiteral = new InstrDecoder(Opcode.ldr, "");
            var LdrbLiteral = new InstrDecoder(Opcode.ldrb, "");
            var StrImm = new InstrDecoder(Opcode.str, "");
            var LdrImm = new InstrDecoder(Opcode.ldr, "");
            var StrbImm = new InstrDecoder(Opcode.strb, "");
            var LdrbImm = new InstrDecoder(Opcode.ldrb, "");
            
            var Strt = new InstrDecoder(Opcode.strt, "");
            var Ldrt = new InstrDecoder(Opcode.ldrt, "");
            var Strbt = new InstrDecoder(Opcode.strbt, "");
            var Ldrbt = new InstrDecoder(Opcode.ldrbt, "");

            var LoadStoreWordUnsignedByteImmLit = new CustomDecoder((wInstr, dasm) => {
                var rn = bitmask(wInstr, 16, 0xF);
                var pw = bitmask(wInstr, 23, 0x10) | bitmask(wInstr, 21, 1);
                var o2_1 = bitmask(wInstr, 21, 2) | bitmask(wInstr, 20, 1);
                if (rn == 0xF)
                {
                    if (pw != 1)
                    {
                        if (o2_1 == 0x01)
                        {
                            return LdrLiteral;
                        }
                        else if (o2_1 == 0x03)
                        {
                            return LdrbLiteral;
                        }
                    }
                }
                switch ((pw << 2) | o2_1)
                {
                case 0: return StrImm;
                case 1: return LdrImm;
                case 2: return StrbImm;
                case 3: return LdrbImm;

                case 4: return Strt;
                case 5: return Ldrt;
                case 6: return Strbt;
                case 7: return Ldrbt;

                case 8: return StrImm;
                case 9: return StrImm;
                case 10: return StrImm;
                case 11: return StrImm;

                case 12: return StrImm;
                case 13: return LdrImm;
                case 14: return StrbImm;
                case 15: return LdrbImm;
                }
                throw new InvalidOperationException("impossible");
            });



            var StrReg = new InstrDecoder(Opcode.str, "");
            var LdrReg = new InstrDecoder(Opcode.ldr, "");
            var StrbReg = new InstrDecoder(Opcode.strb, "");
            var LdrbReg = new InstrDecoder(Opcode.ldrb, "");

            var LoadStoreWordUnsignedByteRegister = new CustomDecoder((wInstr, dasm) =>
            {
                var po2w01 = bitmask(wInstr, 21, 8) | bitmask(wInstr, 20, 7);
                switch (po2w01)
                {
                case 0: return StrReg;
                case 1: return LdrReg;
                case 2: return Strt;
                case 3: return Ldrt;

                case 4: return StrbReg;
                case 5: return LdrbReg;
                case 6: return Strbt;
                case 7: return Ldrbt;

                case 8: return StrReg;
                case 9: return LdrReg;
                case 10: return StrReg;
                case 11: return LdrReg;

                case 12: return StrbReg;
                case 13: return LdrbReg;
                case 14: return StrbReg;
                case 15: return LdrbReg;
                }
                throw new InvalidOperationException();
            });

            var Sadd16 = new InstrDecoder(Opcode.sadd16, "");
            var Sasx = new InstrDecoder(Opcode.sasx, "");
            var Ssax = new InstrDecoder(Opcode.ssax, "");
            var Ssub16 = new InstrDecoder(Opcode.ssub16, "");
            var Sadd8 = new InstrDecoder(Opcode.sadd8, "");
            var Ssub8 = new InstrDecoder(Opcode.ssub8, "");
            var Qadd16 = new InstrDecoder(Opcode.qadd16, "");
            var Qasx = new InstrDecoder(Opcode.qasx, "");
            var Qsax = new InstrDecoder(Opcode.qsax, "");
            var Qsub16 = new InstrDecoder(Opcode.qsub16, "");
            var Qadd8 = new InstrDecoder(Opcode.qadd8, "");
            var QSub8 = new InstrDecoder(Opcode.qsub8, "");
            var Shadd16 = new InstrDecoder(Opcode.shadd16, "");
            var Shasx = new InstrDecoder(Opcode.shasx, "");
            var Shsax = new InstrDecoder(Opcode.shsax, "");
            var Shsub16 = new InstrDecoder(Opcode.shsub16, "");
            var Shadd8 = new InstrDecoder(Opcode.shadd8, "");
            var Shsub8 = new InstrDecoder(Opcode.shsub8, "");
            var Uadd16 = new InstrDecoder(Opcode.uadd16, "");
            var Uasx = new InstrDecoder(Opcode.uasx, "");
            var Usax = new InstrDecoder(Opcode.usax, "");
            var Usub16 = new InstrDecoder(Opcode.usub16, "");
            var Uadd8 = new InstrDecoder(Opcode.uadd8, "");
            var Usub8 = new InstrDecoder(Opcode.usub8, "");
            var Uqadd16 = new InstrDecoder(Opcode.uqadd16, "");
            var Uqasx = new InstrDecoder(Opcode.uqasx, "");
            var Uqsax = new InstrDecoder(Opcode.uqsax, "");
            var Uqsub16 = new InstrDecoder(Opcode.uqsub16, "");
            var Uqadd8 = new InstrDecoder(Opcode.uqadd8, "");
            var Uqsub8 = new InstrDecoder(Opcode.uqsub8, "");
            var Uhadd16 = new InstrDecoder(Opcode.uhadd16, "");
            var Uhasx = new InstrDecoder(Opcode.uhasx, "");
            var Uhsax = new InstrDecoder(Opcode.uhsax, "");
            var Uhsub16 = new InstrDecoder(Opcode.uhsub16, "");
            var Uhadd8 = new InstrDecoder(Opcode.uhadd8, "");
            var Uhsub8 = new InstrDecoder(Opcode.uhsub8, "");

            var ParallelArithmetic = new MaskDecoder(20, 3,
                invalid,
                new MaskDecoder(5, 7,
                    Sadd16,
                    Sasx,
                    Ssax,
                    Ssub16,

                    Sadd8,
                    invalid,
                    invalid,
                    Ssub8),
                new MaskDecoder(5, 7,
                    Qadd16,
                    Qasx,
                    Qsax,
                    Qsub16,

                    Qadd8,
                    invalid,
                    invalid,
                    QSub8),
                new MaskDecoder(5, 7,
                    Shadd16,
                    Shasx,
                    Shsax,
                    Shsub16,

                    Shadd8,
                    invalid,
                    invalid,
                    Shsub8),
                invalid,
                new MaskDecoder(5, 7,
                    Uadd16,
                    Uasx,
                    Usax,
                    Usub16,

                    Uadd8,
                    invalid,
                    invalid,
                    Usub8),
                new MaskDecoder(5, 7,
                    Uqadd16,
                    Uqasx,
                    Uqsax,
                    Uqsub16,

                    Uqadd8,
                    invalid,
                    invalid,
                    Uqsub8),
                new MaskDecoder(5, 7,
                    Uhadd16,
                    Uhasx,
                    Uhsax,
                    Uhsub16,

                    Uhadd8,
                    invalid,
                    invalid,
                    Uhsub8));

            var Media = new MaskDecoder(23, 3,
                ParallelArithmetic,
                nyi("media1"),
                nyi("media2"),
                nyi("media3"));

var StmdaStmed = nyi("StmdaStmed");
var LdmdaLdmfa = nyi("LdmdaLdmfa");
var Ldm =        nyi("ldm");
var StmStmia =   nyi("StmStmia");
var LdmLdmia =   nyi("LdmLdmia");
var StmdbStmfd = nyi("StmdbStmfd");
var LdmdbLDmea = nyi("LdmdbLDmea");

            var LoadStoreMultiple = new MaskDecoder(22, 7, // PUop
                new MaskDecoder(20, 1, // L
                    StmdaStmed,
                    LdmdaLdmfa),
                new MaskDecoder(20, 1, // L
                    invalid,
                    Ldm),
                new MaskDecoder(20, 1, // L
                    StmStmia,
                    LdmLdmia),
                new MaskDecoder(20, 1, // L
                    invalid,
                    Ldm),

                new MaskDecoder(20, 1, // L
                    StmdbStmfd,
                    LdmdbLDmea),
                new MaskDecoder(20, 1, // L
                    invalid,
                    Ldm),
                new MaskDecoder(20, 1, // L
                    StmdbStmfd,
                    LdmdbLDmea),
                new MaskDecoder(20, 1, // L
                    invalid,
                    Ldm));

            var RfeRfeda = nyi("RfeRefda");
            var SrcSrsda = nyi("SrcSrsda");
            var ExceptionSaveRestore = new MaskDecoder(22, 7, // PUS
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),

                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid));

            var B_Imm = new InstrDecoder(Opcode.b, "");
            var BlBlx_A1 = nyi("BlBlx_A1");
            var BlBlx_A2 = nyi("BlBlx_A2");
            var BranchImmediate = new PcDecoder(28,
                new MaskDecoder(24, 1,
                    B_Imm,
                    BlBlx_A1),
                BlBlx_A2);

            var Branch_BranchLink_BlockDataTransfer = new MaskDecoder(25, 1,
                new PcDecoder(28,
                    LoadStoreMultiple,
                    ExceptionSaveRestore),
                BranchImmediate);

            var SystemRegister_AdvancedSimd_FloatingPoint = nyi("SystemRegister_AdvancedSimd_FloatingPoint");


            var ConditionalDecoder = new MaskDecoder(25, 0x7,
                DataProcessingAndMisc,
                DataProcessingAndMisc,
                LoadStoreWordUnsignedByteImmLit,
                new MaskDecoder(4, 1,
                    LoadStoreWordUnsignedByteRegister,
                    Media),
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                SystemRegister_AdvancedSimd_FloatingPoint,
                SystemRegister_AdvancedSimd_FloatingPoint);


            var AdvancedSimd = nyi("AdvancedSimd");
            var AdvancedSimdElementLoadStore = nyi("AdvancedSimdElementLoadStore");
            var MemoryHintsAndBarriers = nyi("MemoryHintsAndBarries");

            var unconditionalDecoder = new MaskDecoder(25, 7,
                Miscellaneous,
                AdvancedSimd,
                new MaskDecoder(20, 1,
                    AdvancedSimdElementLoadStore,
                    MemoryHintsAndBarriers),
                new MaskDecoder(20, 1,
                    MemoryHintsAndBarriers,
                    invalid),
                
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                SystemRegister_AdvancedSimd_FloatingPoint,
                SystemRegister_AdvancedSimd_FloatingPoint);


            rootDecoder = new MaskDecoder(28, 0x0F,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                unconditionalDecoder);






















            /*

            Saturate16Bit = new MaskDecoder(22, 1,
                Ssat16,
                Usat16);

            ReverseBitByte = new MaskDecoder(22, 1,
                new MaskDecoder(7, 1,
                    Rev,
                    Rev16),
                new MaskDecoder(7, 1,
                    Rbit,
                    Revsh));

            Saturate32Bit = new MaskDecoder(22, 1)
                Ssat,
                Usat);

            ExtendAndAdd = new PcDecoder(16, 0xF,
                // not pc 
                MaskDecoder(20, 7,
                    Sxtab16,
                    invalid,
                    Sxtab,
                    Sxtah,

                    Uxtab16,
                    invalid,
                    Uxtab,
                    Uxtah),
                MaskDecoder(20, 7,
                    Sxtb16,
                    invalid,
                    Sxtb,
                    Sxth,

                    Uxtb16,
                    invalid,
                    Uxtb,
                    Uxth));
            SignedMultiplyDivide = nyi,

            UnsignedSumOfAbsDifferences = new PcDecoder(12, 0xF,
                Usada8,
                Usad8);

            BitfieldInsert = new PcDecoder(0, 0x0F,
                Bfi,
                Bfc);

            PermanentlyUndefined = MaskDecoder(28, 0xF,
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
                Udf,
                invalid);

            BitfieldExtract = MaskDecoder(22, 1,
                Sbfx,
                Ubfx);







            var SystemRegister32bitMove = new PcDecoder(28,
                MaskDecoder(20, 1,
                    Mcr,
                    Mrc)),
                invalid);
                
                    */


        }
    }
}
