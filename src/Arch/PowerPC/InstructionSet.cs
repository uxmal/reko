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

 // "Power ISA(tm) Version 3.0 - November 30, 2015 - IBM
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Reko.Arch.PowerPC.PowerPcDisassembler;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;

    /// <summary>
    /// An instruction set builds the decoders for a particular PowerPC instruction set.
    /// </summary>
    public class InstructionSet
    {
        protected readonly Decoder invalid;

        public static InstructionSet Create(string model)
        {
            model = model ?? "";
            switch (model.ToLowerInvariant())
            {
            case "750cl": return new PPC750clInstructionSet();
            case "xenon": return new XenonInstructionSet();
            default: return new InstructionSet();
            }
        }

        public InstructionSet()
        {
            this.invalid = new InvalidDecoder();
        }

        public Decoder[] CreateDecoders()
        {
            var decoders = new Decoder[] {
                // 00
                invalid,
                invalid,
                Instr(Mnemonic.tdi, I1,r2,S),
                Instr(Mnemonic.twi, I1,r2,S),
                Ext4Decoder(),
                Ext5Decoder(),
                Ext6Decoder(),
                Instr(Mnemonic.mulli, r1,r2,S),

                Instr(Mnemonic.subfic, r1,r2,S),
                invalid,
                new CmpDecoder(Mnemonic.cmpli, C1,r2,U),
                new CmpDecoder(Mnemonic.cmpi, C1,r2,S),
                Instr(Mnemonic.addic, r1,r2,S),
                Instr(Mnemonic.addic, CC,r1,r2,S),
                Instr(Mnemonic.addi, r1,r2,S),
                Instr(Mnemonic.addis, r1,r2,S),
                // 10
                new BDecoder(),
                Instr(Mnemonic.sc),
                new IDecoder(),
                Ext13Decoder(),
                Instr(Mnemonic.rlwimi, r2,r1,I3,I4,I5),
                Instr(Mnemonic.rlwinm, C,r2,r1,I3,I4,I5),
                invalid,
                Instr(Mnemonic.rlwnm, r2,r1,r3,I4,I5),

                Instr(Mnemonic.ori, r2,r1,U),
                Instr(Mnemonic.oris, r2,r1,U),
                Instr(Mnemonic.xori, r2,r1,U),
                Instr(Mnemonic.xoris, r2,r1,U),
                Instr(Mnemonic.andi, CC,r2,r1,U),
                Instr(Mnemonic.andis, CC,r2,r1,U),
                new MDDecoder(),
//011110 ..... ..... ..... ..... .000.. MD I 104 PPC SR rldicl[.] Rotate Left Dword Immediate then Clear Left
//011110 ..... ..... ..... ..... .001.. MD I 105 PPC SR rldicr[.] Rotate Left Dword Immediate then Clear Right
//011110 ..... ..... ..... ..... .010.. MD I 104 PPC SR rldic[.] Rotate Left Dword Immediate then Clear
//011110 ..... ..... ..... ..... .011.. MD I 105 PPC SR rldimi[.] Rotate Left Dword Immediate then Mask Insert
//011110 ..... ..... ..... ..... .1000. MDS I 103 PPC SR rldcl[.] Rotate Left Dword then Clear Left
//011110 ..... ..... ..... ..... .1001. MDS I 103 PPC SR rldcr[.] Rotate Left Dword then Clear Right
                Ext1FDecoder(),

                // 20
                Instr(Mnemonic.lwz, r1,E2),
                Instr(Mnemonic.lwzu, r1,E2),
                Instr(Mnemonic.lbz, r1,E2),
                Instr(Mnemonic.lbzu, r1,E2),
                Instr(Mnemonic.stw, r1,E2),
                Instr(Mnemonic.stwu, r1,E2),
                Instr(Mnemonic.stb, r1,E2),
                Instr(Mnemonic.stbu, r1,E2),

                Instr(Mnemonic.lhz, r1,E2),
                Instr(Mnemonic.lhzu, r1,E2),
                Instr(Mnemonic.lha, r1,E2),
                Instr(Mnemonic.lhau, r1,E2),
                Instr(Mnemonic.sth, r1,E2),
                Instr(Mnemonic.sthu, r1,E2),
                Instr(Mnemonic.lmw, r1,E2),
                Instr(Mnemonic.stmw, r1,E2),
                // 30
                Instr(Mnemonic.lfs, f1,E2),
                Instr(Mnemonic.lfsu, f1,E2),
                Instr(Mnemonic.lfd, f1,E2),
                Instr(Mnemonic.lfdu, f1,E2),
                Instr(Mnemonic.stfs, f1,E2),
                Instr(Mnemonic.stfsu, f1,E2),
                Instr(Mnemonic.stfd, f1,E2),
                Instr(Mnemonic.stfdu, f1,E2),

                Ext38Decoder(),
                Ext39Decoder(),
//111001 ..... ..... ..... ..... ....00 DS I 150 v2.05 lfdp Load Floating Double Pair
//111001 ..... ..... ..... ..... ....10 DS I 481 v3.0 lxsd Load VSX Scalar Dword
//111001 ..... ..... ..... ..... ....11 DS I 486 v3.0 lxssp Load VSX Scalar Single

                new DSDecoder(Mnemonic.ld, Mnemonic.ldu, r1,E2),    // 3A
//111010 ..... ..... ..... ..... ....00 DS I 53 PPC ld Load Dword
//111010 ..... ..... ..... ..... ....01 DS I 53 PPC ldu Load Dword with Update
//111010 ..... ..... ..... ..... ....10 DS I 52 PPC lwa Load Word Algebraic

                Ext3BDecoder(),

                Ext3CDecoder(),
                Ext3DDecoder(),
                new DSDecoder(Mnemonic.std, Mnemonic.stdu, r1,E2),          // 3E
// 111110 ..... ..... ..... ..... ....00 DS I 58 PPC std Store Dword
// 111110 ..... ..... ..... ..... ....01 DS I 58 PPC stdu Store Dword with Update
// 111110 ..... ..... ..... ..... ....10 DS I 60 v2.03 stq Store Qword
                Ext3FDecoder(),
            };
            return decoders;
        }


        protected static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        protected static Decoder Nyi(Mnemonic mnemonic)
        {
            return new NyiDecoder(mnemonic.ToString());
        }

        protected static Decoder Instr(Mnemonic mnemonic, params Mutator<PowerPcDisassembler> [] mutators)
        {
            return new InstrDecoder(mnemonic, mutators, InstrClass.Linear);
        }

        protected static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic,  mutators, iclass);
        }

        protected Decoder Mask(int ppcBitPosition, int bits, params Decoder[] decoders)
        {
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(
                32 - (ppcBitPosition + bits), bits, "", decoders);
        }

        protected Decoder Mask(int ppcBitPosition, int bits, string diagnostic, params Decoder[] decoders)
        {
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(
                32 - (ppcBitPosition + bits), bits, diagnostic, decoders);
        }

        protected Decoder Sparse(int ppcBitPosition, int bits, params (uint, Decoder)[] sparseDecoders)
        {
            return Sparse(ppcBitPosition, bits, this.invalid, sparseDecoders);
        }

        protected Decoder Sparse(int ppcBitPosition, int bits, Decoder defaultDecoder, params (uint, Decoder)[] sparseDecoders)
        {
            var decoders = new Decoder[1 << bits];
            foreach (var (code, decoder) in sparseDecoders)
            {
                Debug.Assert(0 <= code && code < decoders.Length);
                Debug.Assert(decoders[code] == null);
                decoders[code] = decoder;
            }
            for (int i = 0; i < decoders.Length; ++i)
            {
                if (decoders[i] == null)
                    decoders[i] = defaultDecoder;
            }
            var leBitPosition = 32 - (ppcBitPosition + bits);
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(leBitPosition, bits, "", decoders);
        }

        private static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        private static Bitfield[] BeFields(params (int bitPos, int bitLength)[] fieldDescs)
        {
            var bitfields = new Bitfield[fieldDescs.Length];
            for (int i = 0; i < fieldDescs.Length; ++i)
            {
                var (bitPos, bitLength) = fieldDescs[i];
                bitfields[i] = BeField(bitPos, bitLength);
            }
            return bitfields;
        }

        protected Decoder Select(int ppcBitPosition, int bits, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            var bitfield = BeField(ppcBitPosition, bits);
            return new ConditionalDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(
                new[] { bitfield },
                predicate, "", trueDecoder, falseDecoder);
        }

        public virtual Decoder Ext4Decoder()
        {
            var vxDecoders =
                Mask(27, 5,
                    Mask(21, 5, "Opc=04, Subop=000000",

                        Instr(Mnemonic.vaddubm, v1, v2, v3), // v2.03 Vector Add Unsigned Byte Modulo
                        Instr(Mnemonic.vadduhm, v1, v2, v3), // v2.03 Vector Add Unsigned Hword Modulo
                        Instr(Mnemonic.vadduwm, v1, v2, v3), // v2.03 Vector Add Unsigned Word Modulo
                        Instr(Mnemonic.vaddudm, v1, v2, v3), // v2.07 Vector Add Unsigned Dword Modulo

                        Instr(Mnemonic.vadduqm, v1, v2, v3), // v2.07 Vector Add Unsigned Qword Modulo
                        Instr(Mnemonic.vaddcuq, v1, v2, v3), // v2.07 Vector Add & write Carry Unsigned Qword
                        Instr(Mnemonic.vaddcuw, v1,v2,v3), // v2.03 Vector Add & Write Carry - Out Unsigned Word
                        invalid,

                        Instr(Mnemonic.vaddubs, v1, v2, v3), // v2.03 Vector Add Unsigned Byte Saturate
                        Instr(Mnemonic.vadduhs, v1, v2, v3), // v2.03 Vector Add Unsigned Hword Saturate
                        Instr(Mnemonic.vadduws, v1, v2, v3), // v2.03 Vector Add Unsigned Word Saturate
                        invalid,

                        Instr(Mnemonic.vaddsbs, v1, v2, v3), // v2.03 Vector Add Signed Byte Saturate
                        Instr(Mnemonic.vaddshs, v1, v2, v3), // v2.03 Vector Add Signed Hword Saturate
                        Instr(Mnemonic.vaddsws, v1, v2, v3), // v2.03 Vector Add Signed Word Saturate
                        invalid,

                        Instr(Mnemonic.vsububm, v1, v2, v3), // v2.03 Vector Subtract Unsigned Byte Modulo
                        Instr(Mnemonic.vsubuhm, v1, v2, v3), // v2.03 Vector Subtract Unsigned Hword Modulo
                        Instr(Mnemonic.vsubuwm, v1, v2, v3), // v2.03 Vector Subtract Unsigned Word Modulo
                        Instr(Mnemonic.vsubudm, v1, v2, v3), // v2.07 Vector Subtract Unsigned Dword Modulo

                        Instr(Mnemonic.vsubuqm, v1, v2, v3), // v2.07 Vector Subtract Unsigned Qword Modulo
                        Instr(Mnemonic.vsubcuq, v1, v2, v3), // v2.07 Vector Subtract & write Carry Unsigned Qword
                        Instr(Mnemonic.vsubcuw, v1, v2, v3), // v2.03 Vector Subtract & Write Carry - Out Unsigned Word
                        invalid,

                        Instr(Mnemonic.vsububs, v1, v2, v3), // v2.03 Vector Subtract Unsigned Byte Saturate
                        Instr(Mnemonic.vsubuhs, v1, v2, v3), // v2.03 Vector Subtract Unsigned Hword Saturate
                        Instr(Mnemonic.vsubuws, v1, v2, v3), // v2.03 Vector Subtract Unsigned Word Saturate
                        invalid,

                        Instr(Mnemonic.vsubsbs, v1, v2, v3), // v2.03 Vector Subtract Signed Byte Saturate
                        Instr(Mnemonic.vsubshs, v1, v2, v3), // v2.03 Vector Subtract Signed Hword Saturate
                        Instr(Mnemonic.vsubsws, v1, v2, v3), // v2.03 Vector Subtract Signed Word Saturate
                        invalid),
                    Mask(21, 1, "Opc=04, Subop=000001",
                        Mask(22, 4, "  Subop2=0?????",
                            Instr(Mnemonic.vmul10cuq, v1, v2),          // v3.0 Vector Multiply-by-10 & write Carry Unsigned Qword
                            Instr(Mnemonic.vmul10ecuq, v1, v2, v3),     // v3.0 Vector Multiply - by - 10 Extended & write Carry Unsigned Qword
                            invalid,
                            invalid,

                            invalid,
                            invalid,
                            invalid,
                            invalid,

                            Instr(Mnemonic.vmul10uq, v1, v2),  // v3.0 Vector Multiply-by-10 Unsigned Qword
                            Instr(Mnemonic.vmul10euq, v1, v2, v3), // v3.0  Vector Multiply - by - 10 Extended Unsigned Qword
                            invalid,
                            invalid,

                            invalid,
                            Instr(Mnemonic.bcdcpsgn, CC, v1, v2, v3), // v3.0 bcdcpsgn.Decimal CopySign &record
                            invalid,
                            invalid),
                        Mask(23, 3,
                            Instr(Mnemonic.bcdadd, CC, v1, v2, v3), // v2.07 bcdadd.Decimal Add Modulo & record
                            Instr(Mnemonic.bcdsub, CC, v1, v2, v3), // v2.07 bcdsub.Decimal Subtract Modulo & record
                            Instr(Mnemonic.bcdus, CC, v1, v2, v3), // v3.0 bcdus.Decimal Unsigned Shift & record
                            Instr(Mnemonic.bcds, CC, v1, v2, v3), // v3.0 bcds.Decimal Shift &record

                            Instr(Mnemonic.bcdtrunc, CC, v1, v2, v3),  // v3.0 bcdtrunc.Decimal Truncate &record
                            Instr(Mnemonic.bcdutrunc, CC, v1, v2, v3), // v3.0 bcdutrunc.Decimal Unsigned Truncate & record
                            Sparse(11, 5,
                                (0b00000, Instr(Mnemonic.bcdctsq, CC, v1, v3)),     // v3.0 bcdctsq.Decimal Convert To Signed Qword & record
                                (0b00010, Instr(Mnemonic.bcdcfsq, CC, v1, v3)),     // v3.0 bcdcfsq.Decimal Convert From Signed Qword & record
                                (0b00100, Instr(Mnemonic.bcdctz, CC, v1, v3)),      // v3.0 bcdctz.Decimal Convert To Zoned &record
                                (0b00101, Instr(Mnemonic.bcdctn, CC, v1, v3)),      // v3.0 bcdctn.Decimal Convert To National &record
                                (0b00110, Instr(Mnemonic.bcdcfz, CC, v1, v3)),      // v3.0 bcdcfz.Decimal Convert From Zoned &record
                                (0b00111, Instr(Mnemonic.bcdcfn, CC, v1, v3)),      // v3.0 bcdcfn.Decimal Convert From National &record
                                (0b11111, Instr(Mnemonic.bcdsetsgn, CC, v1, v3))),  // v3.0 bcdsetsgn.Decimal Set Sign & record
                            Instr(Mnemonic.bcdsr, CC, v1, v2, v3))),                // v3.0 Decimal Shift &Round & record
                    Mask(21, 5, "Opc=04, Subop=000010",
                        Instr(Mnemonic.vmaxub, v1, v2, v3),   // v2.03Vector Maximum Unsigned Byte
                        Instr(Mnemonic.vmaxuh, v1, v2, v3),   // v2.03Vector Maximum Unsigned Hword
                        Instr(Mnemonic.vmaxuw, v1, v2, v3),   // v2.03Vector Maximum Unsigned Word
                        Instr(Mnemonic.vmaxud, v1, v2, v3),   // v2.07Vector Maximum Unsigned Dword
                        Instr(Mnemonic.vmaxsb, v1, v2, v3),   // v2.03Vector Maximum Signed Byte
                        Instr(Mnemonic.vmaxsh, v1, v2, v3),   // v2.03Vector Maximum Signed Hword
                        Instr(Mnemonic.vmaxsw, v1, v2, v3),   // v2.03Vector Maximum Signed Word
                        Instr(Mnemonic.vmaxsd, v1, v2, v3),   // v2.07Vector Maximum Signed Dword
                        Instr(Mnemonic.vminub, v1, v2, v3),   // v2.03Vector Minimum Unsigned Byte
                        Instr(Mnemonic.vminuh, v1, v2, v3),   // v2.03Vector Minimum Unsigned Hword
                        Instr(Mnemonic.vminuw, v1, v2, v3),   // v2.03Vector Minimum Unsigned Word
                        Instr(Mnemonic.vminud, v1, v2, v3),   // v2.07Vector Minimum Unsigned Dword
                        Instr(Mnemonic.vminsb, v1, v2, v3),   // v2.03Vector Minimum Signed Byte
                        Instr(Mnemonic.vminsh, v1, v2, v3),   // v2.03Vector Minimum Signed Hword
                        Instr(Mnemonic.vminsw, v1, v2, v3),   // v2.03Vector Minimum Signed Word
                        Instr(Mnemonic.vminsd, v1, v2, v3),   // v2.07Vector Minimum Signed Dword

                        Instr(Mnemonic.vavgub, v1, v2, v3),   // v2.03Vector Average Unsigned Byte
                        Instr(Mnemonic.vavguh, v1, v2, v3),   // v2.03Vector Average Unsigned Hword
                        Instr(Mnemonic.vavguw, v1, v2, v3),   // v2.03Vector Average Unsigned Word
                        invalid,
                        Instr(Mnemonic.vavgsb, v1, v2, v3),   // v2.03Vector Average Signed Byte
                        Instr(Mnemonic.vavgsh, v1, v2, v3),   // v2.03Vector Average Signed Hword
                        Instr(Mnemonic.vavgsw, v1, v2, v3),   // v2.03Vector Average Signed Word
                        invalid,

                        Sparse(11, 5, 
                            (0b00000, Instr(Mnemonic.vclzlsbb, v1, v3)),     // v3.0 vclzlsbb Vector Count Leading Zero Least-Significant Bits Byte
                            (0b00001, Instr(Mnemonic.vctzlsbb, v1, v3)),     // v3.0 vctzlsbb Vector Count Trailing Zero Least-Significant Bits Byte
                            (0b00110, Instr(Mnemonic.vnegw, v1, v3)),     // v3.0 vnegw Vector Negate Word
                            (0b00111, Instr(Mnemonic.vnegd, v1, v3)),     // v3.0 vnegd Vector Negate Dword
                            (0b01000, Instr(Mnemonic.vprtybw, v1, v3)),     // v3.0 vprtybw Vector Parity Byte Word
                            (0b01001, Instr(Mnemonic.vprtybd, v1, v3)),     // v3.0 vprtybd Vector Parity Byte Dword
                            (0b01010, Instr(Mnemonic.vprtybq, v1, v3)),     // v3.0 vprtybq Vector Parity Byte Qword
                            (0b10000, Instr(Mnemonic.vextsb2w, v1, v3)),     // v3.0 vextsb2w Vector Extend Sign Byte to Word
                            (0b10001, Instr(Mnemonic.vextsh2w, v1, v3)),     // v3.0 vextsh2w Vector Extend Sign Hword to Word
                            (0b11000, Instr(Mnemonic.vextsb2d, v1, v3)),     // v3.0 vextsb2d Vector Extend Sign Byte to Dword
                            (0b11001, Instr(Mnemonic.vextsh2d, v1, v3)),     // v3.0 vextsh2d Vector Extend Sign Hword to Dword
                            (0b11010, Instr(Mnemonic.vextsw2d, v1, v3)),     // v3.0 vextsw2d Vector Extend Sign Word to Dword
                            (0b11100, Instr(Mnemonic.vctzb, v1, v3)),     // v3.0 vctzb Vector Count Trailing Zeros Byte
                            (0b11101, Instr(Mnemonic.vctzh, v1, v3)),     // v3.0 vctzh Vector Count Trailing Zeros Hword
                            (0b11110, Instr(Mnemonic.vctzw, v1, v3)),     // v3.0 vctzw Vector Count Trailing Zeros Word
                            (0b11111, Instr(Mnemonic.vctzd, v1, v3))),    // v3.0 vctzd Vector Count Trailing Zeros Dword
                        invalid,
                        Nyi(Mnemonic.vshasigmaw),   // v2.07Vector SHA - 256 Sigma Word
                        Nyi(Mnemonic.vshasigmad),   // v2.07Vector SHA - 512 Sigma Dword

                        Instr(Mnemonic.vclzb, v1, v3),   // v2.07Vector Count Leading Zeros Byte
                        Instr(Mnemonic.vclzh, v1, v3),   // v2.07Vector Count Leading Zeros Hword
                        Instr(Mnemonic.vclzw, v1, v3),   // v2.07Vector Count Leading Zeros Word
                        Instr(Mnemonic.vclzd, v1, v3)),   // v2.07Vector Count Leading Zeros Dword
                    Sparse(21, 5, // "Opc=04, Subop=000011",
                        (0b10000, Instr(Mnemonic.vabsdub, v1, v2, v3)),   // v3.0 vabsdub Vector Absolute Difference Unsigned Byte
                        (0b10001, Instr(Mnemonic.vabsduh, v1, v2, v3)),   // v3.0 vabsduh Vector Absolute Difference Unsigned Hword
                        (0b10010, Instr(Mnemonic.vabsduw, v1, v2, v3)),   // v3.0 vabsduw Vector Absolute Difference Unsigned Word
                        (0b11100, Instr(Mnemonic.vpopcntb, v1, v3)),   //  v2.07 vpopcntb Vector Population Count Byte
                        (0b11101, Instr(Mnemonic.vpopcnth, v1, v3)),   //  v2.07 vpopcnth Vector Population Count Hword
                        (0b11110, Instr(Mnemonic.vpopcntw, v1, v3)),   //  v2.07 vpopcntw Vector Population Count Word
                        (0b11111, Instr(Mnemonic.vpopcntd, v1, v3))),   //  v2.07 vpopcntd Vector Population Count Dword
                    Mask(21, 5, "Opc=04, Subop=000100",
                        Instr(Mnemonic.vrlb, v1, v2, v3),   // v2.03 Vector Rotate Left Byte
                        Instr(Mnemonic.vrlh, v1, v2, v3),   // v2.03 Vector Rotate Left Hword
                        Instr(Mnemonic.vrlw, v1, v2, v3),   // v2.03 Vector Rotate Left Word
                        Instr(Mnemonic.vrld, v1, v2, v3),   // v2.07 Vector Rotate Left Dword

                        Instr(Mnemonic.vslb, v1, v2, v3),   // v2.03 Vector Shift Left Byte
                        Instr(Mnemonic.vslh, v1, v2, v3),   // v2.03 Vector Shift Left Hword
                        Instr(Mnemonic.vslw, v1, v2, v3),   // v2.03 Vector Shift Left Word
                        Instr(Mnemonic.vsl, v1, v2, v3),   // v2.03 Vector Shift Left

                        Instr(Mnemonic.vsrb, v1, v2, v3),   // v2.03 Vector Shift Right Byte
                        Instr(Mnemonic.vsrh, v1, v2, v3),   // v2.03 Vector Shift Right Hword
                        Instr(Mnemonic.vsrw, v1, v2, v3),   // v2.03 Vector Shift Right Word
                        Instr(Mnemonic.vsr, v1, v2, v3),   // v2.03 Vector Shift Right

                        Instr(Mnemonic.vsrab, v1, v2, v3),   // v2.03 Vector Shift Right Algebraic Byte
                        Instr(Mnemonic.vsrah, v1, v2, v3),   // v2.03 Vector Shift Right Algebraic Hword
                        Instr(Mnemonic.vsraw, v1, v2, v3),   // v2.03 Vector Shift Right Algebraic Word
                        Instr(Mnemonic.vsrad, v1, v2, v3),   // v2.07 Vector Shift Right Algebraic Dword

                        Instr(Mnemonic.vand, v1, v2, v3),   // v2.03 Vector Logical AND
                        Instr(Mnemonic.vandc, v1, v2, v3),  // v2.03 Vector Logical AND with Complement
                        Instr(Mnemonic.vor, v1, v2, v3),    // v2.03 Vector Logical OR
                        Instr(Mnemonic.vxor, v1, v2, v3),   // v2.03 Vector Logical XOR

                        Instr(Mnemonic.vnor, v1,v2,v3),     // v2.03 Vector Logical NOR
                        Instr(Mnemonic.vorc, v1, v2, v3),   // v2.07 Vector Logical OR with Complement
                        Instr(Mnemonic.vnand, v1, v2, v3),  // v2.07 Vector Logical NAND
                        Instr(Mnemonic.vsld, v1, v2, v3),   // v2.07 Vector Shift Left Dword

                        Instr(Mnemonic.mfvscr, v1),         // v2.03 Move From VSCR
                        Instr(Mnemonic.mtvscr, v3),         // v2.03 Move To VSCR
                        Instr(Mnemonic.veqv, v1, v2, v3),   // v2.07 Vector Logical Equivalence
                        Instr(Mnemonic.vsrd, v1, v2, v3),   // v2.07 Vector Shift Right Dword

                        Instr(Mnemonic.vsrv, v1, v2, v3),   // v3.0 Vector Shift Right Variable
                        Instr(Mnemonic.vslv, v1, v2, v3),   // v3.0 Vector Shift Left Variable
                        invalid,
                        invalid),
                    Sparse(21, 5,   // "Opc=04, Subop=000101",
                        (0b00010, Instr(Mnemonic.vrlwmi, v1, v2, v3)),   // v3.0 vrlwmi Vector Rotate Left Word then Mask Insert
                        (0b00011, Instr(Mnemonic.vrldmi, v1, v2, v3)),   // v3.0 vrldmi Vector Rotate Left Dword then Mask Insert
                        (0b00110, Instr(Mnemonic.vrlwnm, v1, v2, v3)),   // v3.0 vrlwnm Vector Rotate Left Word then AND with Mask
                        (0b00111, Instr(Mnemonic.vrldnm, v1, v2, v3))),   // v3.0 vrldnm Vector Rotate Left Dword then AND with Mask
                    Mask(22, 4, "Opc=04, Subop=000110",
                        Instr(Mnemonic.vcmpequb, C10, v1, v2, v3),   // v2.03 vcmpequb[.] Vector Compare Equal Unsigned Byte
                        Instr(Mnemonic.vcmpequh, C10, v1, v2, v3),   // v2.03 vcmpequh[.] Vector Compare Equal Unsigned Hword
                        Instr(Mnemonic.vcmpequw, C10, v1, v2, v3),   // v2.03 vcmpequw[.] Vector Compare Equal Unsigned Word
                        Instr(Mnemonic.vcmpeqfp, C10, v1, v2, v3),   // v2.03 vcmpeqfp[.] Vector Compare Equal To Floating - Point

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgefp, C10, v1, v2, v3),   // v2.03 vcmpgefp[.] Vector Compare Greater Than or Equal To Floating-Point
                        Instr(Mnemonic.vcmpgtub, C10, v1, v2, v3),   // v2.03 vcmpgtub[.] Vector Compare Greater Than Unsigned Byte
                        Instr(Mnemonic.vcmpgtuh, C10, v1, v2, v3),   // v2.03 vcmpgtuh[.] Vector Compare Greater Than Unsigned Hword
                        Instr(Mnemonic.vcmpgtuw, C10, v1, v2, v3),   // v2.03 vcmpgtuw[.] Vector Compare Greater Than Unsigned Word
                        Instr(Mnemonic.vcmpgtfp, C10, v1, v2, v3),   // v2.03 vcmpgtfp[.] Vector Compare Greater Than Floating - Point
                        Instr(Mnemonic.vcmpgtsb, C10, v1, v2, v3),   // v2.03 vcmpgtsb[.] Vector Compare Greater Than Signed Byte
                        Instr(Mnemonic.vcmpgtsh, C10, v1, v2, v3),   // v2.03 vcmpgtsh[.] Vector Compare Greater Than Signed Hword
                        Instr(Mnemonic.vcmpgtsw, C10, v1, v2, v3),   // v2.03 vcmpgtsw[.] Vector Compare Greater Than Signed Word
                        Instr(Mnemonic.vcmpbfp, C10, v1, v2, v3)),   // v2.03 vcmpbfp[.] Vector Compare Bounds Floating-Point
                    Mask(22, 4, "Opc=04, Subop=000111",
                        Instr(Mnemonic.vcmpneb, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal Byte
                        Instr(Mnemonic.vcmpneh, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal Hword
                        Instr(Mnemonic.vcmpnew, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal Word
                        Instr(Mnemonic.vcmpequd, C10, v1, v2, v3),   // v2.07 Vector Compare Equal Unsigned Dword

                        Instr(Mnemonic.vcmpnezb, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal or Zero Byte
                        Instr(Mnemonic.vcmpnezh, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal or Zero Hword
                        Instr(Mnemonic.vcmpnezw, C10, v1, v2, v3),   // v3.0 Vector Compare Not Equal or Zero Word
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgtud, C10, v1, v2, v3),   // v2.07 Vector Compare Greater Than Unsigned Dword

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgtsd, C10, v1, v2, v3)),   // v2.07 Vector Compare Greater Than Signed Dword

                    Mask(21, 5, "Opc=04, Subop=001000",
                        Instr(Mnemonic.vmuloub, v1, v2, v3),   // v2.03 Vector Multiply Odd Unsigned Byte
                        Instr(Mnemonic.vmulouh, v1, v2, v3),   // v2.03 Vector Multiply Odd Unsigned Hword
                        Instr(Mnemonic.vmulouw, v1, v2, v3),   // v2.07 Vector Multiply Odd Unsigned Word
                        invalid,
                        Instr(Mnemonic.vmulosb, v1, v2, v3),   // v2.03 Vector Multiply Odd Signed Byte
                        Instr(Mnemonic.vmulosh, v1, v2, v3),   // v2.03 Vector Multiply Odd Signed Hword
                        Instr(Mnemonic.vmulosw, v1, v2, v3),   // v2.07 Vector Multiply Odd Signed Word
                        invalid,
                        Instr(Mnemonic.vmuleub, v1, v2, v3),   // v2.03 Vector Multiply Even Unsigned Byte
                        Instr(Mnemonic.vmuleuh, v1, v2, v3),   // v2.03 Vector Multiply Even Unsigned Hword
                        Instr(Mnemonic.vmuleuw, v1, v2, v3),   // v2.07 Vector Multiply Even Unsigned Word
                        invalid,
                        Instr(Mnemonic.vmulesb, v1, v2, v3),   // v2.03 Vector Multiply Even Signed Byte
                        Instr(Mnemonic.vmulesh, v1, v2, v3),   // v2.03 Vector Multiply Even Signed Hword
                        Instr(Mnemonic.vmulesw, v1, v2, v3),   // v2.07 Vector Multiply Even Signed Word
                        invalid,
                        Instr(Mnemonic.vpmsumb, v1, v2, v3),    // v2.07 Vector Polynomial Multiply-Sum Byte
                        Instr(Mnemonic.vpmsumh, v1, v2, v3),    // v2.07 Vector Polynomial Multiply-Sum Hword
                        Instr(Mnemonic.vpmsumw, v1, v2, v3),    // v2.07 Vector Polynomial Multiply-Sum Word
                        Instr(Mnemonic.vpmsumd, v1, v2, v3),    // v2.07 Vector Polynomial Multiply-Sum Dword
                        Instr(Mnemonic.vcipher, v1, v2, v3),    // v2.07 Vector AES Cipher
                        Instr(Mnemonic.vncipher, v1, v2, v3),   // v2.07 Vector AES Inverse Cipher
                        invalid,
                        Instr(Mnemonic.vsbox, v1, v2),   // v2.07 Vector AES SubBytes
                        Instr(Mnemonic.vsum4ubs, v1, v2, v3),   // v2.03 Vector Sum across Quarter Unsigned Byte Saturate
                        Instr(Mnemonic.vsum4shs, v1, v2, v3),   // v2.03 Vector Sum across Quarter Signed Hword Saturate
                        Instr(Mnemonic.vsum2sws, v1, v2, v3),   // v2.03 Vector Sum across Half Signed Word Saturate
                        invalid,
                        Instr(Mnemonic.vsum4sbs, v1, v2, v3),   // v2.03 Vector Sum across Quarter Signed Byte Saturate
                        invalid,
                        Instr(Mnemonic.vsumsws, v1, v2, v3),   // v2.03 Vector Sum across Signed Word Saturate
                        invalid),
                    Sparse(21, 5, // "Opc=04, Subop=001001",

                        (0b00010, Instr(Mnemonic.vmuluwm, v1, v2, v3)),   // v2.07 vmuluwm Vector Multiply Unsigned Word Modulo
                        (0b10100, Instr(Mnemonic.vcipherlast, v1, v2, v3)),   // v2.07 vcipherlast Vector AES Cipher Last
                        (0b10101, Instr(Mnemonic.vncipherlast, v1, v2, v3))),   // v2.07 vncipherlast Vector AES Inverse Cipher Last

                    Sparse(21, 5, // "Opc=04, Subop=001010",
                        (0b00000, Instr(Mnemonic.vaddfp, v1, v2, v3)),   // v2.03 vaddfp Vector Add Floating-Point
                        (0b00001, Instr(Mnemonic.vsubfp, v1, v2, v3)),   // v2.03 vsubfp Vector Subtract Floating-Point
                        (0b00100, Instr(Mnemonic.vrefp, v1, v3)),   // v2.03 vrefp Vector Reciprocal Estimate Floating-Point
                        (0b00101, Instr(Mnemonic.vrsqrtefp, v1, v3)),   // v2.03 vrsqrtefp Vector Reciprocal Square Root Estimate Floating-Point
                        (0b00110, Instr(Mnemonic.vexptefp, v1, v3)),   // v2.03 vexptefp Vector 2 Raised to the Exponent Estimate Floating-Point
                        (0b00111, Instr(Mnemonic.vlogefp, v1, v3)),   // v2.03 vlogefp Vector Log Base 2 Estimate Floating-Point
                        (0b01000, Instr(Mnemonic.vrfin, v1, v3)),   // v2.03 vrfin Vector Round to Floating-Point Integral Nearest
                        (0b01001, Instr(Mnemonic.vrfiz, v1, v3)),   // v2.03 vrfiz Vector Round to Floating-Point Integral toward Zero
                        (0b01010, Instr(Mnemonic.vrfip, v1, v3)),   // v2.03 vrfip Vector Round to Floating-Point Integral toward +Infinity
                        (0b01011, Instr(Mnemonic.vrfim, v1, v3)),   // v2.03 vrfim Vector Round to Floating-Point Integral toward -Infinity
                        (0b01100, Instr(Mnemonic.vcfux, v1, v3, u11_5)),   // v2.03 vcfux Vector Convert From Unsigned Word
                        (0b01101, Instr(Mnemonic.vcfsx, v1, v3, u11_5)),   // v2.03 vcfsx Vector Convert From Signed Word
                        (0b01110, Instr(Mnemonic.vctuxs, v1, v3, u11_5)),   // v2.03 vctuxs Vector Convert To Unsigned Word Saturate
                        (0b01111, Instr(Mnemonic.vctsxs, v1, v3, u11_5)),   // v2.03 vctsxs Vector Convert To Signed Word Saturate
                        (0b10000, Instr(Mnemonic.vmaxfp, v1, v2, v3)),   // v2.03 vmaxfp Vector Maximum Floating-Point
                        (0b10001, Instr(Mnemonic.vminfp, v1, v2, v3))),   // v2.03 vminfp Vector Minimum Floating-Point

                    invalid,  // "Opc=04, Subop=001011"
                    Sparse(21, 5, // "Opc=04, Subop=001100"
                        (0b00000, Instr(Mnemonic.vmrghb, v1, v2, v3)),   // v2.03 vmrghb Vector Merge High Byte
                        (0b00001, Instr(Mnemonic.vmrghh, v1, v2, v3)),   // v2.03 vmrghh Vector Merge High Hword
                        (0b00010, Instr(Mnemonic.vmrghw, v1, v2, v3)),   // v2.03 vmrghw Vector Merge High Word
                        (0b00100, Instr(Mnemonic.vmrglb, v1, v2, v3)),   // v2.03 vmrglb Vector Merge Low Byte
                        (0b00101, Instr(Mnemonic.vmrglh, v1, v2, v3)),   // v2.03 vmrglh Vector Merge Low Hword
                        (0b00110, Instr(Mnemonic.vmrglw, v1, v2, v3)),   // v2.03 vmrglw Vector Merge Low Word
                        (0b01000, Instr(Mnemonic.vspltb, v1, v3, u16_4)),   // v2.03 vspltb Vector Splat Byte
                        (0b01001, Instr(Mnemonic.vsplth, v1, v3, u16_3)),   // v2.03 vsplth Vector Splat Hword
                        (0b01010, Instr(Mnemonic.vspltw, v1, v3, u16_2)),   // v2.03 vspltw Vector Splat Word
                        (0b01100, Instr(Mnemonic.vspltisb, v1, s16_5)),   // v2.03 vspltisb Vector Splat Immediate Signed Byte
                        (0b01101, Instr(Mnemonic.vspltish, v1, s16_5)),   // v2.03 vspltish Vector Splat Immediate Signed Hword
                        (0b01110, Instr(Mnemonic.vspltisw, v1, s16_5)),   // v2.03 vspltisw Vector Splat Immediate Signed Word
                        (0b10000, Instr(Mnemonic.vslo, v1, v2, v3)),   // v2.03 vslo Vector Shift Left by Octet
                        (0b10001, Instr(Mnemonic.vsro, v1, v2, v3)),   // v2.03 vsro Vector Shift Right by Octet
                        (0b10100, Instr(Mnemonic.vgbbd, v1, v3)),   // v2.07 vgbbd Vector Gather Bits by Byte by Dword
                        (0b10101, Instr(Mnemonic.vbpermq, v1, v2, v3)),   // v2.07 vbpermq Vector Bit Permute Qword
                        (0b10111, Instr(Mnemonic.vbpermd, v1, v2, v3)),   // v3.0  vbpermd Vector Bit Permute Dword
                        (0b11010, Instr(Mnemonic.vmrgow, v1, v2, v3)),   // v2.07 vmrgow Vector Merge Odd Word
                        (0b11110, Instr(Mnemonic.vmrgew, v1, v2, v3))),  // v2.07 vmrgew Vector Merge Even Word
                    Sparse(21, 5, // "Opc=04, Subop=001101"
                        (0b01000, Instr(Mnemonic.vextractub, v1, v3, u16_4)),   // v3.0 vextractub Vector Extract Unsigned Byte
                        (0b01001, Instr(Mnemonic.vextractuh, v1, v3, u16_4)),   // v3.0 vextractuh Vector Extract Unsigned Hword
                        (0b01010, Instr(Mnemonic.vextractuw, v1, v3, u16_4)),   // v3.0 vextractuw Vector Extract Unsigned Word
                        (0b01011, Instr(Mnemonic.vextractd, v1, v3, u16_4)),   // v3.0 vextractd Vector Extract Dword
                        (0b01100, Instr(Mnemonic.vinsertb, v1, v3, u16_4)),   // v3.0 vinsertb Vector Insert Byte
                        (0b01101, Instr(Mnemonic.vinserth, v1, v3, u16_4)),   // v3.0 vinserth Vector Insert Hword
                        (0b01110, Instr(Mnemonic.vinsertw, v1, v3, u16_4)),   // v3.0 vinsertw Vector Insert Word
                        (0b01111, Instr(Mnemonic.vinsertd, v1, v3, u16_4)),   // v3.0 vinsertd Vector Insert Dword
                        (0b11000, Instr(Mnemonic.vextublx, r1, r2, v3)),   // v3.0 vextublx Vector Extract Unsigned Byte Left-Indexed
                        (0b11001, Instr(Mnemonic.vextuhlx, r1, r2, v3)),   // v3.0 vextuhlx Vector Extract Unsigned Hword Left-Indexed
                        (0b11010, Instr(Mnemonic.vextuwlx, r1, r2, v3)),   // v3.0 vextuwlx Vector Extract Unsigned Word Left-Indexed
                        (0b11100, Instr(Mnemonic.vextubrx, r1, r2, v3)),   // v3.0 vextubrx Vector Extract Unsigned Byte Right-Indexed
                        (0b11101, Instr(Mnemonic.vextuhrx, r1, r2, v3)),   // v3.0 vextuhrx Vector Extract Unsigned Hword Right-Indexed
                        (0b11110, Instr(Mnemonic.vextuwrx, r1, r2, v3))),   // v3.0 vextuwrx Vector Extract Unsigned Word Right-Indexed

                    Sparse(21, 5, // "Opc=04, Subop=001110"
                        (0b00000, Instr(Mnemonic.vpkuhum, v1, v2, v3)),   // v2.03 vpkuhum Vector Pack Unsigned Hword Unsigned Modulo
                        (0b00001, Instr(Mnemonic.vpkuwum, v1, v2, v3)),   // v2.03 vpkuwum Vector Pack Unsigned Word Unsigned Modulo
                        (0b00010, Instr(Mnemonic.vpkuhus, v1, v2, v3)),   // v2.03 vpkuhus Vector Pack Unsigned Hword Unsigned Saturate
                        (0b00011, Instr(Mnemonic.vpkuwus, v1, v2, v3)),   // v2.03 vpkuwus Vector Pack Unsigned Word Unsigned Saturate
                        (0b00100, Instr(Mnemonic.vpkshus, v1, v2, v3)),   // v2.03 vpkshus Vector Pack Signed Hword Unsigned Saturate
                        (0b00101, Instr(Mnemonic.vpkswus, v1, v2, v3)),   // v2.03 vpkswus Vector Pack Signed Word Unsigned Saturate
                        (0b00110, Instr(Mnemonic.vpkshss, v1, v2, v3)),   // v2.03 vpkshss Vector Pack Signed Hword Signed Saturate
                        (0b00111, Instr(Mnemonic.vpkswss, v1, v2, v3)),   // v2.03 vpkswss Vector Pack Signed Word Signed Saturate
                        (0b01000, Instr(Mnemonic.vupkhsb, v1, v2, v3)),   // v2.03 vupkhsb Vector Unpack High Signed Byte
                        (0b01001, Instr(Mnemonic.vupkhsh, v1, v2, v3)),   // v2.03 vupkhsh Vector Unpack High Signed Hword
                        (0b01010, Instr(Mnemonic.vupklsb, v1, v2, v3)),   // v2.03 vupklsb Vector Unpack Low Signed Byte
                        (0b01011, Instr(Mnemonic.vupklsh, v1, v2, v3)),   // v2.03 vupklsh Vector Unpack Low Signed Hword
                        (0b01100, Instr(Mnemonic.vpkpx, v1, v2, v3)),     // v2.03 vpkpx Vector Pack Pixel
                        (0b01101, Instr(Mnemonic.vupkhpx, v1, v2, v3)),   // v2.03 vupkhpx Vector Unpack High Pixel
                        (0b01111, Instr(Mnemonic.vupklpx, v1, v2, v3)),   // v2.03 vupklpx Vector Unpack Low Pixel
                        (0b10001, Instr(Mnemonic.vpkudum, v1, v2, v3)),   // v2.07 vpkudum Vector Pack Unsigned Dword Unsigned Modulo
                        (0b10011, Instr(Mnemonic.vpkudus, v1, v2, v3)),   // v2.07 vpkudus Vector Pack Unsigned Dword Unsigned Saturate
                        (0b10101, Instr(Mnemonic.vpksdus, v1, v2, v3)),   // v2.07 vpksdus Vector Pack Signed Dword Unsigned Saturate
                        (0b10111, Instr(Mnemonic.vpksdss, v1, v2, v3)),   // v2.07 vpksdss Vector Pack Signed Dword Signed Saturate
                        (0b11001, Instr(Mnemonic.vupkhsw, v1, v2, v3)),   // v2.07 vupkhsw Vector Unpack High Signed Word
                        (0b11011, Instr(Mnemonic.vupklsw, v1, v2, v3))),  // v2.07 vupklsw Vector Unpack Low Signed Word

                    invalid,

                    // 010000
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
                    invalid,
                    invalid);

            var vaDecoders = Mask(27, 5,
                Instr(Mnemonic.vmhaddshs, v1, v2, v3, v4),        // v2.03  Vector Multiply - High - Add Signed Hword Saturate
                Instr(Mnemonic.vmhraddshs, v1, v2, v3, v4),       // v2.03  Vector Multiply - High - Round - Add Signed Hword Saturate
                Instr(Mnemonic.vmladduhm, v1, v2, v3, v4),        // v2.03  Vector Multiply - Low - Add Unsigned Hword Modulo
                invalid,

                Instr(Mnemonic.vmsumubm, v1, v2, v3, v4),        // v2.03  Vector Multiply - Sum Unsigned Byte Modulo
                Instr(Mnemonic.vmsummbm, v1, v2, v3, v4),        // v2.03  Vector Multiply - Sum Mixed Byte Modulo
                Instr(Mnemonic.vmsumuhm, v1, v2, v3, v4),        // v2.03  Vector Multiply - Sum Unsigned Hword Modulo
                Instr(Mnemonic.vmsumuhs, v1, v2, v3, v4),       // v2.03  Vector Multiply - Sum Unsigned Hword Saturate
                
                Instr(Mnemonic.vmsumshm, v1, v2, v3, v4),       // v2.03  Vector Multiply - Sum Signed Hword Modulo
                Instr(Mnemonic.vmsumshs, v1, v2, v3, v4),       // v2.03  Vector Multiply - Sum Signed Hword Saturate
                Instr(Mnemonic.vsel, v1,v2,v3,v4),              // v2.03  Vector Select
                Instr(Mnemonic.vperm, v1,v2,v3,v4),             // v2.03  Vector Permute
                
                Instr(Mnemonic.vsldoi, v1,v2,v3, u6_4),         // v2.03  Vector Shift Left Double by Octet Immediate
                Instr(Mnemonic.vpermxor, v1, v2, v3, v4),       // v2.07  Vector Permute & Exclusive - OR
                Instr(Mnemonic.vmaddfp, v1, v2, v3, v4),        // v2.03  Vector Multiply - Add Floating - Point
                Instr(Mnemonic.vnmsubfp, v1, v2, v3, v4),       // v2.03  Vector Negative Multiply - Subtract Floating - Point
                        // 0x10 - 16
                Instr(Mnemonic.maddhd, r1,r2,r3,r4),            // v3.0  Multiply - Add High Dword
                Instr(Mnemonic.maddhdu, r1,r2,r3,r4),           // v3.0  Multiply - Add High Dword Unsigned
                invalid,
                Instr(Mnemonic.maddld, r1,r2,r3,r4),            // v3.0  Multiply - Add Low Dword

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.vpermr, v1,v2,v3,v4),        // v3.0  Vector Permute Right - indexed
                
                Instr(Mnemonic.vaddeuqm, v1, v2, v3, v4),   // v2.07  Vector Add Extended Unsigned Qword Modulo
                Instr(Mnemonic.vaddecuq, v1, v2, v3, v4),   // v2.07  Vector Add Extended & write Carry Unsigned Qword
                Instr(Mnemonic.vsubeuqm, v1, v2, v3, v4),        // v2.07  Vector Subtract Extended Unsigned Qword Modulo
                Instr(Mnemonic.vsubecuq, v1, v2, v3, v4));        // v2.07  Vector Subtract Extended & write Carry Unsigned Qword
            return Mask(26, 1,
                vxDecoders,
                vaDecoders);
        }

        protected Decoder Ext4Decoder_VMX128()
        {
            var decoder = new VXDecoder(
                new Dictionary<uint, Decoder>     // 4
                {
/*

Conventions:

   VD128, VS128:  5 lower bits of a VMX128 vector register 
                  number
   VDh:	          upper 2 bits of VD128
                  (so register number is (VDh << 5 | VD128))
   VA128:         same as VD128
   A:             bit 6 of VA128
   a:             bit 5 of VA128
                  (so register number is (A<<6 | a<<5 | VA128))
   VB128:         same as VD128
   VBh:           same as VDh
   VC128:         3 bits of a VMX128 vector register number
                  (you can only use vr0-vr7 here)
   RA, RB:        general purpose register number
   UIMM:          unsigned immediate value
   SIMM:          signed immediate value
   PERMh:         upper 3 bits of a permutation
   PERMl:         lower 5 bits of a permutation
   x, y, z:       unknown immediate values
=================================================================
   lvewx128                  Load Vector128 Element Word Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 0 0 0|VDh|1 1|
   lvewx128      vr(VD128), r(RA), r(RB)
=================================================================
   lvlx128                           Load Vector128 Left Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 0 0 0|VDh|1 1|
   lvlx128       vr(VD128), r(RA), r(RB)*/
                { 0x403, Instr(Mnemonic.lvlx128, Wd,r2,r3) },
                { 0x407, Instr(Mnemonic.lvlx128, Wd,r2,r3) },
                { 0x40B, Instr(Mnemonic.lvlx128, Wd,r2,r3) },
                { 0x40F, Instr(Mnemonic.lvlx128, Wd,r2,r3) },
   /*
=================================================================
   lvrx128                          Load Vector128 Right Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 1 0 0|VDh|1 1|
   lvrx128       vr(VD128), r(RA), r(RB) */
                { 0x443, Instr(Mnemonic.lvrx128, Wd,r2,r3) },
                { 0x447, Instr(Mnemonic.lvrx128, Wd,r2,r3) },
                { 0x44B, Instr(Mnemonic.lvrx128, Wd,r2,r3) },
                { 0x44F, Instr(Mnemonic.lvrx128, Wd,r2,r3) },
   /*
=================================================================
   lvlxl128                      Load Vector128 Left Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 0 0 0|VDh|1 1|
   lvlxl128      vr(VD128), r(RA), r(RB)
=================================================================
   lvrxl128                     Load Vector128 Right Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 1 0 0|VDh|1 1|
   lvrxl128      vr(VD128), r(RA), r(RB)
=================================================================
   lvsl128                         Load Vector128 for Shift Left
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 0 0 0|VDh|1 1|
   lvsl128       vr(VD128), r(RA), r(RB)
=================================================================
   lvsr128                        Load Vector128 for Shift Right
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 1 0 0|VDh|1 1|
   lvsr128       vr(VD128), r(RA), r(RB)
=================================================================
   lvx128                                 Load Vector128 Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 1 0 0|VDh|1 1|
   lvx128        vr(VD128), r(RA), r(RB)
=================================================================*/
                { 0x0C3, Instr(Mnemonic.lvx128, Wd,r2,r3) },
                { 0x0C7, Instr(Mnemonic.lvx128, Wd,r2,r3) },
                { 0x0CB, Instr(Mnemonic.lvx128, Wd,r2,r3) },
                { 0x0CF, Instr(Mnemonic.lvx128, Wd,r2,r3) },
/*
   lvxl128                            Load Vector128 Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 1 0 1 1 0 0|VDh|1 1|
   lvxl128       vr(VD128), r(RA), r(RB)
=================================================================
   stewx128                 Store Vector128 Element Word Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 0 0 0 0|VDh|1 1|
   stvewx128     vr(VS128), r(RA), r(RB)
=================================================================
   stvlx128                         Store Vector128 Left Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 0 0 0|VDh|1 1|
   stvlx128      vr(VS128), r(RA), r(RB)
*/
                { 0x503, Instr(Mnemonic.stvlx128, Wd,r2,r3) },
                { 0x507, Instr(Mnemonic.stvlx128, Wd,r2,r3) },
                { 0x50B, Instr(Mnemonic.stvlx128, Wd,r2,r3) },
                { 0x50F, Instr(Mnemonic.stvlx128, Wd,r2,r3) },

/*=================================================================
   stvlxl128                    Store Vector128 Left Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 0 0 0|VDh|1 1|
   lvlxl128      vr(VS128), r(RA), r(RB)
=================================================================
   stvrx128                        Store Vector128 Right Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 1 0 0|VDh|1 1|
   stvrx128       vr(VS128), r(RA), r(RB)
*/
                { 0x543, Instr(Mnemonic.stvrx128, Wd,r2,r3) },
                { 0x547, Instr(Mnemonic.stvrx128, Wd,r2,r3) },
                { 0x54B, Instr(Mnemonic.stvrx128, Wd,r2,r3) },
                { 0x54F, Instr(Mnemonic.stvrx128, Wd,r2,r3) },

/*=================================================================
   stvrxl128                   Store Vector128 Right Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 1 0 0|VDh|1 1|
   stvrxl128     vr(VS128), r(RA), r(RB)
=================================================================
   stvx128                               Store Vector128 Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 0 1 1 1 0 0|VDh|1 1|
   stvx128       vr(VS128), r(RA), r(RB)*/
                { 0x1C3, Instr(Mnemonic.stvx128, Wd,r2,r3) },
                { 0x1C7, Instr(Mnemonic.stvx128, Wd,r2,r3) },
                { 0x1CB, Instr(Mnemonic.stvx128, Wd,r2,r3) },
                { 0x1CF, Instr(Mnemonic.stvx128, Wd,r2,r3) },

/*=================================================================
   stvxl128                          Store Vector128 Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 1 1 0 0|VDh|1 1|
   stvxl128      vr(VS128), r(RA), r(RB)
=================================================================
   vsldoi128                         Vector128 Shift Left Double 
                                              by Octet Immediate
|0 0 0 1 0 0|  VD128  |  VA128  |  VB128  |A|  SHB  |a|1|VDh|VBh|
   vsldoi128     vr(VD128), vr(VA128), vr(VB128), SHB

=================================================================
   vaddfp128                        Vector128 Add Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|
   vaddfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vand128                                 Vector128 Logical AND
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|
   vand128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vandc128                                Vector128 Logical AND 
                                                 with Complement
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|1|VDh|VBh|
   vandc128      vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmaddcfp128                            Vector128 Multiply Add 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|
   vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)
=================================================================
   vmaddfp128                             Vector128 Multiply Add 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|
   vmaddfp128    vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vmsum3fp128                      Vector128 Multiply Sum 3-way 
                                                  Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|
   vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmsum4fp128                      Vector128 Multiply Sum 4-way 
                                                  Floating-Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|
   vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmulfp128                                  Vector128 Multiply
                                                  Floating-Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|
   vmulfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vnmsubfp128              Vector128 Negative Multiply-Subtract 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 1|a|1|VDh|VBh|
   vnmsubfp128   vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vnor128                                 Vector128 Logical NOR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|1|VDh|VBh|
   vnor128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vor128                                   Vector128 Logical OR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|
   vor128        vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vperm128                                Vector128 Permutation
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|
   vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
=================================================================
   vpkshss128                    Vector128 Pack Signed Half Word 
                                                 Signed Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|0|VDh|VBh|
   vpkshss128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkshus128                    Vector128 Pack Signed Half Word 
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 1|a|0|VDh|VBh|
   vpkshus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkswss128                         Vector128 Pack Signed Word
                                                 Signed Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|
   vpkswss128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkswus128                         Vector128 Pack Signed Word   
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|
   vpkswus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuhum128                  Vector128 Pack Unsigned Half Word 
                                                 Unsigned Modulo
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|
   vpkuhum128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuhus128                  Vector128 Pack Unsigned Half Word 
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|
   vpkuhus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuwum128                       Vector128 Pack Unsigned Word
                                                 Unsigned Modulo
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 0|a|0|VDh|VBh|
   vpkuwum128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuwus128                       Vector128 Pack Unsigned Word
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 1|a|0|VDh|VBh|
   vpkuwus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsel128                                      Vector128 Select
|0 0 0 1 0 1| VDS128  |  VA128  |  VB128  |A|1 1 0 1|a|1|VDh|VBh|
   vsel128       vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vslo128                            Vector128 Shift Left Octet
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 0|a|1|VDh|VBh|
   vslo128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsubfp128                   Vector128 Subtract Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|
   vsubfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vxor128                                 Vector128 Logical XOR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|
   vxor128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vrlw128                            Vector128 Rotate Left Word
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|
   vrlw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vcfpsxws128          Vector128 Convert From Floating-Point to 
                                Signed Fixed-Point Word Saturate
|0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|
   vcfpsxws128   vr(VD128), vr(VB128), SIMM
=================================================================
   vcfpuxws128          Vector128 Convert From Floating-Point to 
                              Unsigned Fixed-Point Word Saturate
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 0 1 1 1|VDh|VBh|
   vcfpuxws128   vr(VD128), vr(VB128), UIMM
=================================================================
   vcmpbfp128                           Vector128 Compare Bounds 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|
   vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpbfp128.   vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpeqfp128                        Vector128 Compare Equal-to
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|
   vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpeqfp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpequw128                        Vector128 Compare Equal-to 
                                                   Unsigned Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|
   vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpequw128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpgefp128                    Vector128 Compare Greater-Than-
                                      or-Equal-to Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|
   vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpgefp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpgtfp128                    Vector128 Compare Greater-Than 
                                                  Floating-Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|
   vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpgtfp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcsxwfp128          Vector128 Convert From Signed Fixed-Point 
                                          Word to Floating-Point
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|
   vcsxwfp128    vr(VD128), vr(VB128), SIMM
=================================================================
   vcuxwfp128        Vector128 Convert From Unsigned Fixed-Point
                                          Word to Floating-Point
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 1 1 1|VDh|VBh|
   vcuxwfp128    vr(VD128), vr(VB128), UIMM
=================================================================
   vexptefp128                Vector128 2 Raised to the Exponent 
                                         Estimate Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|
   vexptefp128   vr(VD128), vr(VB128)
=================================================================
   vlogefp128                            Vector128 Log2 Estimate 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|
   vlogefp128    vr(VD128), vr(VB128)
=================================================================
   vmaxfp128                                   Vector128 Maximum 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|
   vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vminfp128                                   Vector128 Minimum
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|
   vminfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmrghw128                           Vector128 Merge High Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|
   vmrghw128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmrglw128                            Vector128 Merge Low Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|
   vmrglw128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpermwi128                 Vector128 Permutate Word Immediate
|0 0 0 1 1 0|  VD128  |  PERMl  |  VB128  |0|1|PERMh|0|1|VDh|VBh|
   vpermwi128    vr(VD128), vr(VB128), (PERMh << 5 | PERMl)
=================================================================
   vpkd3d128                 Vector128 Pack D3Dtype, Rotate Left 
                                       Immediate and Mask Insert
|0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|
   vpkd3d128     vr(VD128), vr(VB128), x, y, z
=================================================================
   vrefp128                        Vector128 Reciprocal Estimate 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|
   vrefp128      vr(VD128), vr(VB128)
=================================================================
   vrfim128                    Vector128 Round to Floating-Point 
                                              Integer toward -oo
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 0 1 1|VDh|VBh|
   vrfim128      vr(VD128), vr(VB128)
=================================================================
   vrfin128                    Vector128 Round to Floating-Point 
                                          Integer toward Nearest
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|
   vrfin128      vr(VD128), vr(VB128)
=================================================================
   vrfip128                    Vector128 Round to Floating-Point 
                                              Integer toward +oo
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 0 1 1|VDh|VBh|
   vrfip128      vr(VD128), vr(VB128)
=================================================================
   vrfiz128                    Vector128 Round to Floating-Point 
                                             Integer toward Zero
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|
   vrfiz128      vr(VD128), vr(VB128)
=================================================================
   vrlimi128                     Vector128 Rotate Left Immediate 
                                                 and Mask Insert
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|
   vrlimi128     vr(VD128), vr(VB128), UIMM, z
=================================================================
   vrsqrtefp128                 Vector128 Reciprocal Square Root 
                                         Estimate Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|
   vrsqrtefp128  vr(VD128), vr(VB128)
=================================================================
   vslw128                             Vector128 Shift Left Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|
   vslw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vspltisw128                         Vector128 Splat Immediate 
                                                     Signed Word
|0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |1 1 1 0 1 1 1|VDh|VBh|
   vspltisw128   vr(VD128), vr(VB128), SIMM
=================================================================
   vspltw128                                Vector128 Splat Word
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|
   vspltw128     vr(VD128), vr(VB128), UIMM
=================================================================
   vsraw128                                Vector128 Shift Right 
                                                 Arithmetic Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0 1|a|1|VDh|VBh|
   vsraw128      vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsro128                           Vector128 Shift Right Octet
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 1 1|a|1|VDh|VBh|
   vsro128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsrw128                            Vector128 Shift Right Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|
   vsrw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vupkd3d128                           Vector128 Unpack D3Dtype
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|
   vupkd3d128    vr(VD128), vr(VB128), UIMM
=================================================================
   vupkhsb128                                   Vector128 Unpack 
                                                High Signed Byte
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 0 0 0|VDh|VBh|
   vupkhsb128    vr(VD128), vr(VB128)
=================================================================
   vupklsb128                                   Vector128 Unpack 
                                                 Low Signed Byte
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 0 0|VDh|VBh|
   vupkhsb128    vr(VD128), vr(VB128)
*/
                        { 0x000, Instr(Mnemonic.vaddubm, v1,v2,v3) },
                        { 0x002, Instr(Mnemonic.vmaxub, v1,v2,v3) },
                        { 0x00A, Instr(Mnemonic.vaddfp, v1,v2,v3) },
                       // { 0x010, Instr(Mnemonic.mulhhwu, r1,r2,r3) },
                        { 0x020, Instr(Mnemonic.vmhaddshs, v1,v2,v3,v4) },
                        { 0x022, Instr(Mnemonic.vmladduhm, v1,v2,v3,v4) },
                        { 0x042, Instr(Mnemonic.vmaxuh, v1,v2,v3) },
                        { 0x04A, Instr(Mnemonic.vsubfp, v1,v2,v3) },
                        { 0x080, Instr(Mnemonic.vadduwm, v1,v2,v3) },
                        { 0x086, Instr(Mnemonic.vcmpequw, v1,v2,v3) },
                        { 0x08C, Instr(Mnemonic.vmrghw, v1,v2,v3) },
                        { 0x0C6, Instr(Mnemonic.vcmpeqfp, v1,v2,v3) },
                        { 0x0E2, Instr(Mnemonic.vmladduhm, v1,v2,v3,v4) },
                        { 0x100, Instr(Mnemonic.vadduqm, v1,v2,v3) },
                        { 0x10A, Instr(Mnemonic.vrefp, v1,v3) },
                        { 0x14A, Instr(Mnemonic.vrsqrtefp, v1,v3) },
                        { 0x184, Instr(Mnemonic.vslw, v1,v2,v3) },
                        { 0x18C, Instr(Mnemonic.vmrglw, v1,v2,v3) },
                        { 0x200, Instr(Mnemonic.vaddubs, v1,v2,v3) },
                        { 0x28C, Instr(Mnemonic.vspltw, v1,v3,u16_2) },
                        { 0x2C6, Instr(Mnemonic.vcmpgtfp, v1,v2,v3) },
                        { 0x34A, Instr(Mnemonic.vcfsx, v1,v3,u16_5) },
                        { 0x38C, Instr(Mnemonic.vspltisw, v1,s16_5) },
                        { 0x3CA, Instr(Mnemonic.vctsxs, v1,v3,u16_5) },
                        { 0x401, Instr(Mnemonic.bcdadd, CC,v1,v2,v3,u9_1) },
                        { 0x404, Instr(Mnemonic.vand, v1,v2,v3) },
                        { 0x406, Instr(Mnemonic.vcmpequb, CC,v1,v2,v3) },
                        { 0x444, Instr(Mnemonic.vandc, v1,v2,v3) },
                        { 0x484, Instr(Mnemonic.vor, v1,v2,v3) },
                        { 0x4C4, Instr(Mnemonic.vxor, v1,v2,v3)},
                        { 0x4C6, Instr(Mnemonic.vcmpeqfp, CC,v1,v2,v3) },
                        { 0x4C7, Instr(Mnemonic.vcmpequd, CC,v1,v2,v3) },
                        // These aren't present for the VMX extension of PowerPC architecture
                        //{ 0x503, Instr(Mnemonic.evmhessfaaw, r1,r2,r3) },
                        //{ 0x50B, Instr(Mnemonic.evmhesmfaaw, r1,r2,r3) },
                        { 0x686, Instr(Mnemonic.vcmpgtuw, CC,v1,v2,v3) },
                        { 0x6C6, Instr(Mnemonic.vcmpgtfp, CC,v1,v2,v3) },
                },
                new Dictionary<uint, Decoder>
                {
                    { 0x028, Instr(Mnemonic.vmsumshm, v1,v2,v3,v4) },
                    { 0x029, Instr(Mnemonic.vmsumshs, v1,v2,v3,v4) },
                    { 0x02A, Instr(Mnemonic.vsel, v1,v2,v3,v4) },
                    { 0x02B, Instr(Mnemonic.vperm, v1,v2,v3,v4) },
                    { 0x02C, Instr(Mnemonic.vsldoi, v1,v2,v3,u6_5) },
                    { 0x02E, Instr(Mnemonic.vmaddfp, v1,v2,v4,v3) },
                    { 0x02F, Instr(Mnemonic.vnmsubfp, v1,v2,v4,v3) }

                });
            return decoder;
        }


        private Decoder Ext5Decoder()
        {
            return new VMXDecoder(0x3D, new Dictionary<uint, Decoder>
            {
                { 0x00, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x02, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x04, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x06, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)

                { 0x08, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x0A, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x0C, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x0E, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)

                { 0x10, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x12, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x14, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x16, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)

                { 0x18, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x1A, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x1C, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                { 0x1E, new FnDecoder(VMXDecoder.DecodeVperm128) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)

                { 0x01, Instr(Mnemonic.vaddfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x03, Instr(Mnemonic.vaddfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x05, Instr(Mnemonic.vsubfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|    vsubfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x07, Instr(Mnemonic.vsubfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|    vsubfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x09, Instr(Mnemonic.vmulfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x0B, Instr(Mnemonic.vmulfp128, Wd,Wa,Wb) },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x11, Instr(Mnemonic.vmaddcfp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|    vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)
                { 0x13, Instr(Mnemonic.vmaddcfp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|    vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)

                { 0x19, Instr(Mnemonic.vmsub3fp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|    vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)
                { 0x1B, Instr(Mnemonic.vmsub3fp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|    vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)

                { 0x1D, Instr(Mnemonic.vmsub4fp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
                { 0x1F, Instr(Mnemonic.vmsub4fp128, Wd,Wa,Wb) },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)

                { 0x21, Instr(Mnemonic.vand128, Wd,Wa,Wb) },           // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|    vand128       vr(VD128), vr(VA128), vr(VB128)
                { 0x23, Instr(Mnemonic.vand128, Wd,Wa,Wb) },           // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|    vand128       vr(VD128), vr(VA128), vr(VB128)



                { 0x2D, Instr(Mnemonic.vor128, Wd,Wa,Wb) },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)
                { 0x2F, Instr(Mnemonic.vor128, Wd,Wa,Wb) },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)

                { 0x31, Instr(Mnemonic.vxor128, Wd,Wa,Wb) },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)
                { 0x33, Instr(Mnemonic.vxor128, Wd,Wa,Wb) },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)

            });
        }

        private Decoder Ext6Decoder() {
            return new VMXDecoder(0x7F, new Dictionary<uint, Decoder>
            {
                { 0x00, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x02, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x04, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x06, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x40, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x42, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x44, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x46, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x08, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0A, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0C, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0E, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x48, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4A, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4C, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4E, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)


                { 0x10, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x12, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x14, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x16, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x50, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x52, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x54, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x56, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x18, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1A, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1C, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1E, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x58, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5A, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5C, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5E, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x0D, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x0F, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4D, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4F, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x1D, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x1F, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5D, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5F, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x20, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x22, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x24, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x26, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x60, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x62, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x64, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x66, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x23, Instr(Mnemonic.vcfpsxws128, Wd,Wb,s16_5) },    // |0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|    vcfpsxws128   vr(VD128), vr(VB128), SIMM

                { 0x28, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2A, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x68, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6A, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x2B, Instr(Mnemonic.vcsxwfp128, Wd,Wb,u16_5) },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|    vcsxwfp128    vr(VD128), vr(VB128), SIMM

                { 0x2C, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2E, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6C, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6E, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x30, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x32, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x70, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x72, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x34, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x36, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x74, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x76, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x37, Instr(Mnemonic.vrfin128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|    vrfin128      vr(VD128), vr(VB128)

                { 0x3F, Instr(Mnemonic.vrfiz128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|    vrfiz128      vr(VD128), vr(VB128)

                { 0x61, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x65, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x69, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x6D, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z

                { 0x63, Instr(Mnemonic.vrefp128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|    vrefp128      vr(VD128), vr(VB128)

                { 0x67, Instr(Mnemonic.vrsqrtefp128, Wd,Wb) },         // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|    vrsqrtefp128  vr(VD128), vr(VB128)

                { 0x6B, Instr(Mnemonic.vexptefp128, Wd,Wb) },          // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|    vexptefp128   vr(VD128), vr(VB128)

                { 0x6F, Instr(Mnemonic.vlogefp128, Wd,Wb) },           // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|    vlogefp128    vr(VD128), vr(VB128)

                { 0x77, Instr(Mnemonic.vspltisw128, Wd,Wb,s16_5) },

                { 0x71, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x75, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x79, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x7D, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z

                { 0x73, Instr(Mnemonic.vspltw128, Wd,Wb,u16_5) },      // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|    vspltw128     vr(VD128), vr(VB128), UIMM

                { 0x7F, Instr(Mnemonic.vupkd3d128, Wd,Wb,u16_5) },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|    vupkd3d128    vr(VD128), vr(VB128), UIMM
            });
        }

        private Decoder Ext13Decoder()
        {
            return Sparse(26, 5,
                (0b00000, Select(21, 5, u => u == 0,
                    Instr(Mnemonic.mcrf, C1, C2),
                    invalid)),
            //010011 ...// ...// ///// 00000 00000/ XL I 42 P1 mcrf Move CR Field
                (0b00001, Sparse(21, 5,
                    (0b00001, Instr(Mnemonic.crnor, I1, I2, I3)), // All P1
                    (0b00100, Instr(Mnemonic.crandc, I1, I2, I3)),
                    (0b00110, Instr(Mnemonic.crxor, I1, I2, I3)),
                    (0b00111, Instr(Mnemonic.crnand, I1, I2, I3)),
                    (0b01000, Instr(Mnemonic.crand, I1, I2, I3)),
                    (0b01001, Instr(Mnemonic.creqv, I1, I2, I3)),
                    (0b01101, Instr(Mnemonic.crorc, I1, I2, I3)),
                    (0b01110, Instr(Mnemonic.cror, I1, I2, I3)))),
                (0b00010, Instr(Mnemonic.addpcis, r1, s(BeFields((16, 10),(11, 5),(31,1))))),       // v3.0 addpcis Add PC Immediate Shifted
                (0b10000, Sparse(21, 5,
                    (0b00000, new BclrDecoder()),       // P1
                    (0b10000, Mask(31, 1,               // P1
                        Instr(InstrClass.ConditionalTransfer, Mnemonic.bcctr, I1, I2),
                        Instr(InstrClass.ConditionalTransfer | InstrClass.Call, Mnemonic.bcctrl, I1, I2))),
                    (0b10001, Nyi(Mnemonic.bctar)))), // v2.07 bctar[l] Branch Conditional to BTAR [& Link]
                (0b10010, Sparse(21, 5,
                    (0b00000, Instr(InstrClass.Transfer, Mnemonic.rfid)),   //  PPC  P  rfid Return from Interrupt Dword
                    (0b00010, Nyi(Mnemonic.rfscv)),     // v3.0 P  rfscv Return From System Call Vectored
                    (0b00100, Nyi(Mnemonic.rfebb)),     // v2.07   rfebb Return from Event Based Branch
                    (0b01000, Nyi(Mnemonic.hrfid)),     // v2.02 H hrfid Return From Interrupt Dword Hypervisor
                    (0b01011, Nyi(Mnemonic.stop)))),    // v3.0 P  stop Stop
                (0b10110, Select(21, 5, u => u == 0b00100,
                    Instr(Mnemonic.isync),          // P1 isync Instruction Synchronize
                    invalid)));
        }

        private Decoder Ext1FDecoder()
        {
            return new XDecoder(new Dictionary<uint, Decoder>      // 1F
            {
                { 0, new CmpDecoder(Mnemonic.cmp, C1,r2,r3) },
                { 4, Instr(Mnemonic.tw, I1,r2,r3) },
                { 0x006, Instr(Mnemonic.lvsl, v1,r2,r3) },
                { 0x008, Instr(Mnemonic.subfc, r1,r2,r3)},
                { 0x00A, Instr(Mnemonic.addc, r1,r2,r3)},
                { 0x00B, Instr(Mnemonic.mulhwu, C,r1,r2,r3)},
                { 0x013, Instr(Mnemonic.mfcr, r1) },
                { 0x015, Instr(Mnemonic.ldx, r1,r2,r3) },
                { 0x017, Instr(Mnemonic.lwzx, r1,r2,r3) },
                { 0x018, Instr(Mnemonic.slw, C,r2,r1,r3) },
                { 0x01A, Instr(Mnemonic.cntlzw, r2,r1) },
                { 0x01B, Instr(Mnemonic.sld, C,r2,r1,r3) },
                { 0x01C, Instr(Mnemonic.and, C,r2,r1,r3)},
                { 0x020, new CmpDecoder(Mnemonic.cmpl, C1,r2,r3) },
                { 0x014, Instr(Mnemonic.lwarx, r1,r2,r3) },
                { 0x028, Instr(Mnemonic.subf, C,r1,r2,r3)},
                { 0x035, Instr(Mnemonic.ldux, r1,r2,r3)},
                { 0x036, Instr(Mnemonic.dcbst, r2,r3)},
                { 0x037, Instr(Mnemonic.lwzux, r1,r2,r3)},
                { 0x03A, Instr(Mnemonic.cntlzd, r2,r1) },
                { 0x03C, Instr(Mnemonic.andc, C,r2,r1,r3) },
                { 0x047, Instr(Mnemonic.lvewx, v1,r2,r3) },
                { 0x04B, Instr(Mnemonic.mulhw, C,r1,r2,r3) },
                { 0x053, Instr(Mnemonic.mfmsr, r1) },
                { 0x054, Instr(Mnemonic.ldarx, r1,r2,r3) },
                { 0x056, Instr(Mnemonic.dcbf, r2,r3) },
                { 0x057, Instr(Mnemonic.lbzx, r1,r2,r3) },
                { 0x067, Instr(Mnemonic.lvx, v1,r2,r3) },
                { 0x068, Instr(Mnemonic.neg, r1,r2) },
                { 0x077, Instr(Mnemonic.lbzux, r1,r2,r3) },
                { 124, Instr(Mnemonic.nor, C,r2,r1,r3) },
                { 0x088, Instr(Mnemonic.subfe, r1,r2,r3) },
                { 0x08A, Instr(Mnemonic.adde, C,r1,r2,r3) },
                { 0x090, Instr(Mnemonic.mtcrf, M,r1) },
                { 0x092, Instr(Mnemonic.mtmsr, r1,u16_1) },
                { 0x095, Instr(Mnemonic.stdx, r1,r2,r3) },
                { 0x096, Instr(Mnemonic.stwcx, C,r1,r2,r3) },
                { 0x097, Instr(Mnemonic.stwx, r1,r2,r3) },
                { 0x0C7, Instr(Mnemonic.stvewx, v1,r2,r3)},
                { 0x0B2, Instr(Mnemonic.mtmsrd, r1,u16_1) },
                { 0x0B7, Instr(Mnemonic.stwux, r1,r2,r3) },
                { 0x0E7, Instr(Mnemonic.stvx, v1,r2,r3) },
                { 215, Instr(Mnemonic.stbx, r1,r2,r3) },
                { 235, Instr(Mnemonic.mullw, C,r1,r2,r3) },
                { 0x0C8, Instr(Mnemonic.subfze, C,r1,r2) },
                { 0x0CA, Instr(Mnemonic.addze, C,r1,r2) },
                { 0x0D6, Instr(Mnemonic.stdcx, CC,r1,r2,r3) },
                { 0x0E9, Instr(Mnemonic.mulld, C,r1,r2,r3)},
                { 0x0EA, Instr(Mnemonic.addme, C,r1,r2)},
                { 0x0F6, Instr(Mnemonic.dcbtst, r2,r3) },
                { 247, Instr(Mnemonic.stbux, r1,r2,r3) },
                { 0x10A, Instr(Mnemonic.add, C,r1,r2,r3) },
                { 279, Instr(Mnemonic.lhzx, r1,r2,r3) },
                { 0x116, Instr(Mnemonic.dcbt, r2,r3,u21_4) },
                { 0x11C, Instr(Mnemonic.eqv, r1,r2,r3) },
                { 306, Instr(Mnemonic.tlbie, r3) },
                { 311, Instr(Mnemonic.lhzux, r1,r2,r3) },
                { 316, Instr(Mnemonic.xor, C,r2,r1,r3) },
                { 444, Instr(Mnemonic.or, C,r2,r1,r3) },

                { 0x153, new SprDecoder(false) },
                { 0x155, Instr(Mnemonic.lwax, r1,r2,r3) },
                { 0x157, Instr(Mnemonic.lhax, r1,r2,r3) },
                { 0x173, new XfxDecoder(Mnemonic.mftb, r1,X3) },
                { 0x177, Instr(Mnemonic.lhaux, r1,r2,r3) },
                { 0x197, Instr(Mnemonic.sthx, r1,r2,r3) },
                { 0x19C, Instr(Mnemonic.orc, C,r2,r1,r3) },
                { 0x1C9, Instr(Mnemonic.divdu, C,r1,r2,r3) },
                { 0x1CB, Instr(Mnemonic.divwu, C,r1,r2,r3) },
                { 0x1DC, Instr(Mnemonic.nand, C,r2,r1,r3) },
                { 0x1D3, new SprDecoder(true) },
                { 0x1D6, Instr(Mnemonic.dcbi, r2,r3)},
                { 0x1E9, Instr(Mnemonic.divd, C,r1,r2,r3)},
                { 0x1EB, Instr(Mnemonic.divw, C,r1,r2,r3)},
                { 0x207, Instr(Mnemonic.lvlx, r1,r2,r3) },
                { 0x208, Instr(Mnemonic.subfco, C,r1,r2,r3)},
                { 0x20A, Instr(Mnemonic.addco, C,r1,r2,r3)},
                { 0x216, Instr(Mnemonic.lwbrx, r1,r2,r3) },
                { 0x217, Instr(Mnemonic.lfsx, f1,r2,r3) },
                { 0x218, Instr(Mnemonic.srw, C,r2,r1,r3) },
                { 0x21B, Instr(Mnemonic.srd, C,r2,r1,r3) },
                { 0x255, Instr(Mnemonic.lswi, r1,r2,I3) },
                { 566, Instr(Mnemonic.tlbsync) },
                { 0x237, Instr(Mnemonic.lfsux, f1,r2,r3) },
                { 0x257, Instr(Mnemonic.lfdx, f1,r2,r3) },
                { 0x277, Instr(Mnemonic.lfdux, f1,r2,r3) },
                { 0x256, Instr(Mnemonic.sync) },
                { 0x296, Instr(Mnemonic.stwbrx, C,r2,r1,r3) },
                { 0x297, Instr(Mnemonic.stfsx, f1,r2,r3) },
                { 0x2B7, Instr(Mnemonic.stfsux, f1,r2,r3) },
                { 0x2D5, Instr(Mnemonic.stswi, r1,r2,I3) },
                { 0x2D7, Instr(Mnemonic.stfdx, f1,r2,r3) },
                { 0x316, Instr(Mnemonic.lhbrx, r1,r2,r3)},
                { 0x318, Instr(Mnemonic.sraw, C,r2,r1,r2)},
                { 0x31A, Instr(Mnemonic.srad, C,r2,r1,r2)},
                { 0x33A, Instr(Mnemonic.sradi, C,r2,r1,I3) },
                { 0x338, Instr(Mnemonic.srawi, r2,r1,I3) },
                { 0x33B, new XSDecoder(Mnemonic.sradi, C,r2,r1,I3) },
                { 0x356, Instr(Mnemonic.eieio) },
                { 0x39A, Instr(Mnemonic.extsh, C,r2,r1)},
                { 0x3BA, Instr(Mnemonic.extsb, C,r2,r1)},
                { 0x3D7, Instr(Mnemonic.stfiwx, f1,r2,r3)},
                { 0x3D6, Instr(Mnemonic.icbi, r2,r3)},
                { 0x3DA, Instr(Mnemonic.extsw, C,r2,r1)},
                { 0x3F6, Instr(Mnemonic.dcbz, r2,r3) }
             });

/*
011111 .../. ..... ..... 00000 00000/ X I 85 P1 cmp Compare
011111 .../. ..... ..... 00001 00000/ X I 86 P1 cmpl Compare Logical
011111 ..... ...// ///// 00100 00000/ X I 121 v3.0 setb Set Boolean
011111 .../. ..... ..... 00110 00000/ X I 87 v3.0 cmprb Compare Ranged Byte
011111 ...// ..... ..... 00111 00000/ X I 88 v3.0 cmpeqb Compare Equal Byte
011111 ...// ///// ///// 10010 00000/ X I 119 v3.0 mcrxrx Move XER to CR Extended

011111 ..... ..... ..... 00000 00100/ X I 89 P1 tw Trap Word
011111 ..... ..... ..... 00010 00100/ X I 90 PPC td Trap Dword

011111 ..... ..... ..... 00000 00110/ X I 249 v2.03 lvsl Load Vector for Shift Left
011111 ..... ..... ..... 00001 00110/ X I 249 v2.03 lvsr Load Vector for Shift Right
011111 ..... ..... ..... 10010 00110/ X II 864 v3.0 lwat Load Word ATomic
011111 ..... ..... ..... 10011 00110/ X II 864 v3.0 ldat Load Dword ATomic
011111 ..... ..... ..... 10110 00110/ X II 866 v3.0 stwat Store Word ATomic
011111 ..... ..... ..... 10111 00110/ X II 866 v3.0 stdat Store Dword ATomic
011111 ////. ..... ..... 11000 00110/ X II 858 v3.0 copy Copy
011111 ///// ///// ///// 11010 00110/ X II 860 v3.0 cp_abort CP_Abort
011111 ////. ..... ..... 11100 00110. X II 859 v3.0 paste[.] Paste

011111 ..... ..... ..... 00000 00111/ X I 244 v2.03 lvebx Load Vector Element Byte Indexed
011111 ..... ..... ..... 00001 00111/ X I 244 v2.03 lvehx Load Vector Element Hword Indexed
011111 ..... ..... ..... 00010 00111/ X I 245 v2.03 lvewx Load Vector Element Word Indexed
011111 ..... ..... ..... 00011 00111/ X I 245 v2.03 lvx Load Vector Indexed
011111 ..... ..... ..... 00100 00111/ X I 247 v2.03 stvebx Store Vector Element Byte Indexed
011111 ..... ..... ..... 00101 00111/ X I 247 v2.03 stvehx Store Vector Element Hword Indexed
011111 ..... ..... ..... 00110 00111/ X I 248 v2.03 stvewx Store Vector Element Word Indexed
011111 ..... ..... ..... 00111 00111/ X I 248 v2.03 stvx Store Vector Indexed
011111 ..... ..... ..... 01011 00111/ X I 245 v2.03 lvxl Load Vector Indexed Last
011111 ..... ..... ..... 01111 00111/ X I 248 v2.03 stvxl Store Vector Indexed Last

011111 ..... ..... ..... .0000 01000. XO I 71 P1 SR subfc[o][.] Subtract From Carrying
011111 ..... ..... ..... .0001 01000. XO I 70 PPC SR subf[o][.] Subtract From
011111 ..... ..... ///// .0011 01000. XO I 73 P1 SR neg[o][.] Negate
011111 ..... ..... ..... .0100 01000. XO I 72 P1 SR subfe[o][.] Subtract From Extended
011111 ..... ..... ///// .0110 01000. XO I 73 P1 SR subfze[o][.] Subtract From Zero Extended
011111 ..... ..... ///// .0111 01000. XO I 72 P1 SR subfme[o][.] Subtract From Minus One Extended

011111 ..... ..... ..... /0000 01001. XO I 80 PPC SR mulhdu[.] Multiply High Dword Unsigned
011111 ..... ..... ..... /0010 01001. XO I 80 PPC SR mulhd[.] Multiply High Dword
011111 ..... ..... ..... .0111 01001. XO I 80 PPC SR mulld[o][.] Multiply Low Dword
011111 ..... ..... ..... .1100 01001. XO I 83 v2.06 SR divdeu[o][.] Divide Dword Extended Unsigned
011111 ..... ..... ..... .1101 01001. XO I 83 v2.06 SR divde[o][.] Divide Dword Extended
011111 ..... ..... ..... .1110 01001. XO I 82 PPC SR divdu[o][.] Divide Dword Unsigned
011111 ..... ..... ..... .1111 01001. XO I 82 PPC SR divd[o][.] Divide Dword
011111 ..... ..... ..... 01000 01001/ X I 84 v3.0 modud Modulo Unsigned Dword
011111 ..... ..... ..... 11000 01001/ X I 84 v3.0 modsd Modulo Signed Dword

011111 ..... ..... ..... .0000 01010. XO I 71 P1 SR addc[o][.] Add Carrying
011111 ..... ..... ..... /0010 01010/ XO I 110 v2.06 addg6s Add & Generate Sixes
011111 ..... ..... ..... .0100 01010. XO I 72 P1 SR adde[o][.] Add Extended
011111 ..... ..... ///// .0110 01010. XO I 73 P1 SR addze[o][.] Add to Zero Extended
011111 ..... ..... ///// .0111 01010. XO I 72 P1 SR addme[o][.] Add to Minus One Extended
011111 ..... ..... ..... .1000 01010. XO I 70 P1 SR add[o][.] Add

011111 ..... ..... ..... /0000 01011. XO I 74 PPC SR mulhwu[.] Multiply High Word Unsigned
011111 ..... ..... ..... /0010 01011. XO I 74 PPC SR mulhw[.] Multiply High Word
011111 ..... ..... ..... .0111 01011. XO I 74 P1 SR mullw[o][.] Multiply Low Word
011111 ..... ..... ..... .1100 01011. XO I 77 v2.06 SR divweu[o][.] Divide Word Extended Unsigned
011111 ..... ..... ..... .1101 01011. XO I 77 v2.06 SR divwe[o][.] Divide Word Extended
011111 ..... ..... ..... .1110 01011. XO I 75 PPC SR divwu[o][.] Divide Word Unsigned
011111 ..... ..... ..... .1111 01011. XO I 75 PPC SR divw[o][.] Divide Word
011111 ..... ..... ..... 01000 01011/ X I 76 v3.0 moduw Modulo Unsigned Word
011111 ..... ..... ..... 11000 01011/ X I 76 v3.0 modsw Modulo Signed Word

011111 ..... ..... ..... 00000 01100. XX1 I 485 v2.07 lxsiwzx Load VSX Scalar as Integer Word & Zero Indexed
011111 ..... ..... ..... 00010 01100. XX1 I 484 v2.07 lxsiwax Load VSX Scalar as Integer Word Algebraic Indexed
011111 ..... ..... ..... 00100 01100. XX1 I 501 v2.07 stxsiwx Store VSX Scalar as Integer Word Indexed
011111 ..... ..... ..... 01000 01100. XX1 I 493 v3.0 lxvx Load VSX Vector Indexed
011111 ..... ..... ..... 01010 01100. XX1 I 495 v2.06 lxvdsx Load VSX Vector Dword & Splat Indexed
011111 ..... ..... ..... 01011 01100. XX1 I 498 v3.0 lxvwsx Load VSX Vector Word & Splat Indexed
011111 ..... ..... ..... 01100 01100. XX1 I 511 v3.0 stxvx Store VSX Vector Indexed
011111 ..... ..... ..... 10000 01100. XX1 I 486 v2.07 lxsspx Load VSX Scalar SP Indexed
011111 ..... ..... ..... 10010 01100. XX1 I 481 v2.06 lxsdx Load VSX Scalar Dword Indexed
011111 ..... ..... ..... 10100 01100. XX1 I 503 v2.07 stxsspx Store VSX Scalar SP Indexed
011111 ..... ..... ..... 10110 01100. XX1 I 499 v2.06 stxsdx Store VSX Scalar Dword Indexed
011111 ..... ..... ..... 11000 01100. XX1 I 497 v2.06 lxvw4x Load VSX Vector Word*4 Indexed
011111 ..... ..... ..... 11001 01100. XX1 I 496 v3.0 lxvh8x Load VSX Vector Hword*8 Indexed
011111 ..... ..... ..... 11010 01100. XX1 I 489 v2.06 lxvd2x Load VSX Vector Dword*2 Indexed
011111 ..... ..... ..... 11011 01100. XX1 I 488 v3.0 lxvb16x Load VSX Vector Byte*16 Indexed
011111 ..... ..... ..... 11100 01100. XX1 I 507 v2.06 stxvw4x Store VSX Vector Word*4 Indexed
011111 ..... ..... ..... 11101 01100. XX1 I 506 v3.0 stxvh8x Store VSX Vector Hword*8 Indexed
011111 ..... ..... ..... 11110 01100. XX1 I 505 v2.06 stxvd2x Store VSX Vector Dword*2 Indexed
011111 ..... ..... ..... 11111 01100. XX1 I 504 v3.0 stxvb16x Store VSX Vector Byte*16 Indexed

011111 ..... ..... ..... 01000 01101. XX1 I 490 v3.0 lxvl Load VSX Vector with Length
011111 ..... ..... ..... 01001 01101. XX1 I 492 v3.0 lxvll Load VSX Vector Left-justified with Length
011111 ..... ..... ..... 01100 01101. XX1 I 508 v3.0 stxvl Store VSX Vector with Length
011111 ..... ..... ..... 01101 01101. XX1 I 510 v3.0 stxvll Store VSX Vector Left-justified with Length
011111 ..... ..... ..... 11000 01101. XX1 I 483 v3.0 lxsibzx Load VSX Scalar as Integer Byte & Zero Indexed
011111 ..... ..... ..... 11001 01101. XX1 I 483 v3.0 lxsihzx Load VSX Scalar as Integer Hword & Zero Indexed
011111 ..... ..... ..... 11100 01101. XX1 I 500 v3.0 stxsibx Store VSX Scalar as Integer Byte Indexed
011111 ..... ..... ..... 11101 01101. XX1 I 500 v3.0 stxsihx Store VSX Scalar as Integer Hword Indexed

011111 ///// ///// ..... 00100 01110/ X III 1125 v2.07 P msgsndp Message Send Privileged
011111 ///// ///// ..... 00101 01110/ X III 1126 v2.07 P msgclrp Message Clear Privileged
011111 ///// ///// ..... 00110 01110/ X III 1123 v2.07 H msgsnd Message Send
011111 ///// ///// ..... 00111 01110/ X III 1124 v2.07 H msgclr Message Clear
011111 ..... ..... ..... 01001 01110/ X I 44 v2.07 mfbhrbe Move From BHRB
011111 ///// ///// ///// 01101 01110/ X I 44 v2.07 clrbhrb Clear BHRB
011111 .//// ///// ///// 10101 01110/ X II 894 v2.07 tend. Transaction End & record
011111 ...// ///// ///// 10110 01110/ X II 898 v2.07 tcheck Transaction Check & record
011111 ////. ///// ///// 10111 01110/ X II 898 v2.07 tsr. Transaction Suspend or Resume & record
011111 .///. ///// ///// 10100 011101 X II 893 v2.07 tbegin. Transaction Begin & record
011111 ..... ..... ..... 11000 011101 X II 896 v2.07 tabortwc. Transaction Abort Word Conditional & record
011111 ..... ..... ..... 11001 011101 X II 897 v2.07 tabortdc. Transaction Abort Dword Conditional & record
011111 ..... ..... ..... 11010 011101 X II 896 v2.07 tabortwci. Transaction Abort Word Conditional Immediate & record
011111 ..... ..... ..... 11011 011101 X II 897 v2.07 tabortdci. Transaction Abort Dword Conditional Immediate & record
011111 ///// ..... ///// 11100 011101 X II 895 v2.07 tabort. Transaction Abort & record
011111 ///// ..... ///// 11101 011101 X III 969 v2.07 P treclaim. Transaction Reclaim & record
011111 ///// ///// ///// 11111 011101 X III 970 v2.07 P trechkpt. Transaction Recheckpoint & record

011111 ..... ..... ..... ..... 01111/ A I 90 v2.03 isel Integer Select

011111 ..... 0.... ..../ 00100 10000/ XFX I 120 P1 mtcrf Move To CR Fields
011111 ..... 1.... ..../ 00100 10000/ XFX I 120 v2.01 mtocrf Move To One CR Field

011111 ..... ////. ///// 00100 10010/ X III 977 P1 P mtmsr Move To MSR
011111 ..... ////. ///// 00101 10010/ X III 978 PPC P mtmsrd Move To MSR Dword
011111 ///// ///// ..... 01000 10010/ X III 1038 v2.03 P 64 tlbiel TLB Invalidate Entry Local
011111 ////. ///// ..... 01001 10010/ X III 1034 P1 H 64 tlbie TLB Invalidate Entry
011111 ///// ///// ///// 01010 10010/ X III 1031 v3.0 P slbsync SLB Synchronize
011111 ..... ///// ..... 01100 10010/ X III 1029 v2.00 P slbmte SLB Move To Entry
011111 ///// ///// ..... 01101 10010/ X III 1024 PPC P slbie SLB Invalidate Entry
011111 ..... ///// ..... 01110 10010/ X III 1025 v3.0 P slbieg SLB Invalidate Entry Global
011111 //... ///// ///// 01111 10010/ X III 1027 PPC P slbia SLB Invalidate All

011111 ..... 0//// ///// 00000 10011/ XFX I 121 P1 mfcr Move From CR
011111 ..... 1.... ..../ 00000 10011/ XFX I 121 v2.01 mfocrf Move From One CR Field
011111 ..... ..... ///// 00001 10011. XX1 I 111 v2.07 mfvsrd Move From VSR Dword
011111 ..... ///// ///// 00010 10011/ X III 979 P1 P mfmsr Move From MSR
011111 ..... ..... ///// 00011 10011. XX1 I 112 v2.07 mfvsrwz Move From VSR Word & Zero
011111 ..... ..... ///// 00101 10011. XX1 I 113 v2.07 mtvsrd Move To VSR Dword
011111 ..... ..... ///// 00110 10011. XX1 I 113 v2.07 mtvsrwa Move To VSR Word Algebraic
011111 ..... ..... ///// 00111 10011. XX1 I 114 v2.07 mtvsrwz Move To VSR Word & Zero
011111 ..... ..... ///// 01001 10011. XX1 I 111 v3.0 mfvsrld Move From VSR Lower Dword
011111 ..... ..... ..... 01010 10011/ X I
011111 ..... ..... ..... 01011 10011/ X II 902 PPC mftb Move From Time Base
011111 ..... ..... ///// 01100 10011. XX1 I 115 v3.0 mtvsrws Move To VSR Word & Splat
011111 ..... ..... ..... 01101 10011. XX1 I 114 v3.0 mtvsrdd Move To VSR Double Dword
011111 ..... ..... ..... 01110 10011/ X I
011111 ..... ///.. ///// 10111 10011/ X I 79 v3.0 darn Deliver A Random Number
011111 ..... ///// ..... 11010 10011/ X III 1030 v2.00 P slbmfev SLB Move From Entry VSID
011111 ..... ///// ..... 11100 10011/ X III 1030 v2.00 P slbmfee SLB Move From Entry ESID
011111 ..... ///// ..... 11110 100111 X III 1031 v2.05 P SR slbfee. SLB Find Entry ESID & record

011111 ..... ..... ..... 00000 10100/ X II 869 PPC lwarx Load Word & Reserve Indexed
011111 ..... ..... ..... 00001 10100. X II 868 v2.06 lbarx Load Byte And Reserve Indexed
011111 ..... ..... ..... 00010 10100/ X II 873 PPC ldarx Load Dword And Reserve Indexed
011111 ..... ..... ..... 00011 10100. X II 869 v2.06 lharx Load Hword And Reserve Indexed Xform
011111 ..... ..... ..... 01000 10100. X II 875 v2.07 lqarx Load Qword And Reserve Indexed
011111 ..... ..... ..... 10000 10100/ X I 62 v2.06 ldbrx Load Dword Byte-Reverse Indexed
011111 ..... ..... ..... 10100 10100/ X I 62 v2.06 stdbrx Store Dword Byte-Reverse Indexed

011111 ..... ..... ..... 00000 10101/ X I 53 PPC ldx Load Dword Indexed
011111 ..... ..... ..... 00001 10101/ X I 53 PPC ldux Load Dword with Update Indexed
011111 ..... ..... ..... 00100 10101/ X I 58 PPC stdx Store Dword Indexed
011111 ..... ..... ..... 00101 10101/ X I 58 PPC stdux Store Dword with Update Indexed
011111 ..... ..... ..... 01001 10101/ X I 54 v3.0 PI ldmx Load Dword Monitored Indexed
011111 ..... ..... ..... 01010 10101/ X I 52 PPC lwax Load Word Algebraic Indexed
011111 ..... ..... ..... 01011 10101/ X I 52 PPC lwaux Load Word Algebraic with Update Indexed
011111 ..... ..... ..... 10000 10101/ X I 65 P1 lswx Load String Word Indexed
011111 ..... ..... ..... 10010 10101/ X I 65 P1 lswi Load String Word Immediate
011111 ..... ..... ..... 10100 10101/ X I 66 P1 stswx Store String Word Indexed
011111 ..... ..... ..... 10110 10101/ X I 66 P1 stswi Store String Word Immediate
011111 ..... ..... ..... 11000 10101/ X III 966 v2.05 H lwzcix Load Word & Zero Caching Inhibited Indexed
011111 ..... ..... ..... 11001 10101/ X III 966 v2.05 H lhzcix Load Hword & Zero Caching Inhibited Indexed
011111 ..... ..... ..... 11010 10101/ X III 966 v2.05 H lbzcix Load Byte & Zero Caching Inhibited Indexed
011111 ..... ..... ..... 11011 10101/ X III 966 v2.05 H ldcix Load Dword Caching Inhibited Indexed
011111 ..... ..... ..... 11100 10101/ X III 967 v2.05 H stwcix Store Word Caching Inhibited Indexed
011111 ..... ..... ..... 11101 10101/ X III 967 v2.05 H sthcix Store Hword Caching Inhibited Indexed
011111 ..... ..... ..... 11110 10101/ X III 967 v2.05 H stbcix Store Byte Caching Inhibited Indexed
011111 ..... ..... ..... 11111 10101/ X III 967 v2.05 H stdcix Store Dword Caching Inhibited Indexed

011111 /.... ..... ..... 00000 10110/ X II 842 v2.07 icbt Instruction Cache Block Touch
011111 ///// ..... ..... 00001 10110/ X II 853 PPC dcbst Data Cache Block Store
011111 ///.. ..... ..... 00010 10110/ X II 854 PPC dcbf Data Cache Block Flush
011111 ..... ..... ..... 00111 10110/ X II 852 PPC dcbtst Data Cache Block Touch for Store
011111 ..... ..... ..... 01000 10110/ X II 851 PPC dcbt Data Cache Block Touch
011111 ..... ..... ..... 10000 10110/ X I 61 P1 lwbrx Load Word Byte-Reverse Indexed
011111 ///// ///// ///// 10001 10110/ X III 1042 PPC H tlbsync TLB Synchronize
011111 ///.. ///// ///// 10010 10110/ X II 877 P1 sync Synchronize
011111 ..... ..... ..... 10100 10110/ X I 61 P1 stwbrx Store Word Byte-Reverse Indexed
011111 ..... ..... ..... 11000 10110/ X I 61 P1 lhbrx Load Hword Byte-Reverse Indexed
011111 ///// ///// ///// 11010 10110/ X II 879 PPC eieio Enforce In-order Execution of I/O
011111 ///// ///// ///// 11011 10110/ X III 1126 v3.0 H msgsync Message Synchronize
011111 ..... ..... ..... 11100 10110/ X I 61 P1 sthbrx Store Hword Byte-Reverse Indexed
011111 ///// ..... ..... 11110 10110/ X II 842 PPC icbi Instruction Cache Block Invalidate
011111 ///// ..... ..... 11111 10110/ X II 853 P1 dcbz Data Cache Block Zero
011111 ..... ..... ..... 00100 101101 X II 872 PPC stwcx. Store Word Conditional Indexed & record
011111 ..... ..... ..... 00101 101101 X II 876 v2.07 stqcx. Store Qword Conditional Indexed & record
011111 ..... ..... ..... 00110 101101 X II 873 PPC stdcx. Store Dword Conditional Indexed & record
011111 ..... ..... ..... 10101 101101 X II 870 v2.06 stbcx. Store Byte Conditional Indexed & record
011111 ..... ..... ..... 10110 101101 X II 871 v2.06 sthcx. Store Hword Conditional Indexed & record

011111 ..... ..... ..... 00000 10111/ X I 51 P1 lwzx Load Word & Zero Indexed
011111 ..... ..... ..... 00001 10111/ X I 51 P1 lwzux Load Word & Zero with Update Indexed
011111 ..... ..... ..... 00010 10111/ X I 48 P1 lbzx Load Byte & Zero Indexed
011111 ..... ..... ..... 00011 10111/ X I 48 P1 lbzux Load Byte & Zero with Update Indexed
011111 ..... ..... ..... 00100 10111/ X I 57 P1 stwx Store Word Indexed
011111 ..... ..... ..... 00101 10111/ X I 57 P1 stwux Store Word with Update Indexed
011111 ..... ..... ..... 00110 10111/ X I 55 P1 stbx Store Byte Indexed
011111 ..... ..... ..... 00111 10111/ X I 55 P1 stbux Store Byte with Update Indexed
011111 ..... ..... ..... 01000 10111/ X I 49 P1 lhzx Load Hword & Zero Indexed
011111 ..... ..... ..... 01001 10111/ X I 49 P1 lhzux Load Hword & Zero with Update Indexed
011111 ..... ..... ..... 01010 10111/ X I 50 P1 lhax Load Hword Algebraic Indexed
011111 ..... ..... ..... 01011 10111/ X I 50 P1 lhaux Load Hword Algebraic with Update Indexed
011111 ..... ..... ..... 01100 10111/ X I 56 P1 sthx Store Hword Indexed
011111 ..... ..... ..... 01101 10111/ X I 56 P1 sthux Store Hword with Update Indexed
011111 ..... ..... ..... 10000 10111/ X I 142 P1 lfsx Load Floating Single Indexed
011111 ..... ..... ..... 10001 10111/ X I 142 P1 lfsux Load Floating Single with Update Indexed
011111 ..... ..... ..... 10010 10111/ X I 143 P1 lfdx Load Floating Double Indexed
011111 ..... ..... ..... 10011 10111/ X I 143 P1 lfdux Load Floating Double with Update Indexed
011111 ..... ..... ..... 10100 10111/ X I 146 P1 stfsx Store Floating Single Indexed
011111 ..... ..... ..... 10101 10111/ X I 146 P1 stfsux Store Floating Single with Update Indexed
011111 ..... ..... ..... 10110 10111/ X I 147 P1 stfdx Store Floating Double Indexed
011111 ..... ..... ..... 10111 10111/ X I 147 P1 stfdux Store Floating Double with Update Indexed
011111 ..... ..... ..... 11000 10111/ X I 150 v2.05 lfdpx Load Floating Double Pair Indexed
011111 ..... ..... ..... 11010 10111/ X I 144 v2.05 lfiwax Load Floating as Integer Word Algebraic Indexed
011111 ..... ..... ..... 11011 10111/ X I 144 v2.06 lfiwzx Load Floating as Integer Word & Zero Indexed
011111 ..... ..... ..... 11100 10111/ X I 150 v2.05 stfdpx Store Floating Double Pair Indexed
011111 ..... ..... ..... 11110 10111/ X I 148 PPC stfiwx Store Floating as Integer Word Indexed

011111 ..... ..... ..... 00000 11000. X I 106 P1 SR slw[.] Shift Left Word
011111 ..... ..... ..... 10000 11000. X I 106 P1 SR srw[.] Shift Right Word
011111 ..... ..... ..... 11000 11000. X I 107 P1 SR sraw[.] Shift Right Algebraic Word
011111 ..... ..... ..... 11001 11000. X I 107 P1 SR srawi[.] Shift Right Algebraic Word Immediate

011111 ..... ..... ..... 11001 1101.. XS I 109 PPC SR sradi[.] Shift Right Algebraic Dword Immediate
011111 ..... ..... ..... 11011 1101.. XS I 109 v3.0 extswsli[.] Extend Sign Word & Shift Left Immediate
011111 ..... ..... ///// 00000 11010. X I 95 P1 SR cntlzw[.] Count Leading Zeros Word
011111 ..... ..... ///// 00001 11010. X I 98 PPC SR cntlzd[.] Count Leading Zeros Dword
011111 ..... ..... ///// 00011 11010/ X I 96 v2.02 popcntb Population Count Byte
011111 ..... ..... ///// 00100 11010/ X I 97 v2.05 prtyw Parity Word
011111 ..... ..... ///// 00101 11010/ X I 97 v2.05 prtyd Parity Dword
011111 ..... ..... ///// 01000 11010/ X I 110 v2.06 cdtbcd Convert Declets To Binary Coded Decimal
011111 ..... ..... ///// 01001 11010/ X I 110 v2.06 cbcdtd Convert Binary Coded Decimal To Declets
011111 ..... ..... ///// 01011 11010/ X I 96 v2.06 popcntw Population Count Words
011111 ..... ..... ///// 01111 11010/ X I 98 v2.06 popcntd Population Count Dword
011111 ..... ..... ///// 10000 11010. X I 95 v3.0 cnttzw[.] Count Trailing Zeros Word
011111 ..... ..... ///// 10001 11010. X I 98 v3.0 cnttzd[.] Count Trailing Zeros Dword
011111 ..... ..... ..... 11000 11010. X I 109 PPC SR srad[.] Shift Right Algebraic Dword
011111 ..... ..... ///// 11100 11010. X I 94 P1 SR extsh[.] Extend Sign Hword
011111 ..... ..... ///// 11101 11010. X I 94 PPC SR extsb[.] Extend Sign Byte
011111 ..... ..... ///// 11110 11010. X I 98 PPC SR extsw[.] Extend Sign Word

011111 ..... ..... ..... 00000 11011. X I 108 PPC SR sld[.] Shift Left Dword
011111 ..... ..... ..... 10000 11011. X I 108 PPC SR srd[.] Shift Right Dword

011111 ..... ..... ..... 00000 11100. X I 93 P1 SR and[.] AND
011111 ..... ..... ..... 00001 11100. X I 94 P1 SR andc[.] AND with Complement
011111 ..... ..... ..... 00011 11100. X I 94 P1 SR nor[.] NOR
011111 ..... ..... ..... 00111 11100/ X I 99 v2.06 bpermd Bit Permute Dword
011111 ..... ..... ..... 01000 11100. X I 94 P1 SR eqv[.] Equivalent
011111 ..... ..... ..... 01001 11100. X I 93 P1 SR xor[.] XOR
011111 ..... ..... ..... 01100 11100. X I 94 P1 SR orc[.] OR with Complement
011111 ..... ..... ..... 01101 11100. X I 93 P1 SR or[.] OR
011111 ..... ..... ..... 01110 11100. X I 93 P1 SR nand[.] NAND
011111 ..... ..... ..... 01111 11100/ X I 96 v2.05 cmpb Compare Bytes

011111 ///.. ///// ///// 00000 11110/ X II 880 v3.0 wait Wait for Interrupt
 */
        }

        public virtual Decoder Ext38Decoder()
        {
            return Instr(Mnemonic.lq, Is64Bit, r1,E2);
        }

        public virtual Decoder Ext39Decoder()
        {
            return Instr(Mnemonic.lfdp, p1, E2_2);
        }

        private Decoder Ext3BDecoder()
        {
            return new FpuDecoder(1, 0x1F, new Dictionary<uint, Decoder> // 3B
            {
                { 18, Instr(Mnemonic.fdivs, C,f1,f2,f3) },
                { 20, Instr(Mnemonic.fsubs, C,f1,f2,f3) },
                { 21, Instr(Mnemonic.fadds, C,f1,f2,f3) },
                { 22, Instr(Mnemonic.fsqrts, C,f1,f3) },
                { 24, Instr(Mnemonic.fres, C,f1,f3) },
                { 25, Instr(Mnemonic.fmuls, C,f1,f2,f4) },
                { 28, Instr(Mnemonic.fmsubs, C,f1,f2,f4,f3) },
                { 29, Instr(Mnemonic.fmadds, C,f1,f2,f4,f3) },
                { 30, Instr(Mnemonic.fnmsubs, C,f1,f2,f3,f4) },
                { 31, Instr(Mnemonic.fnmadds, C,f1,f2,f3,f4) },
            });
/*
111011 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscli[.] DFP Shift Significand Left Immediate
111011 ..... ..... ..... .0011 00010. Z22 I 222 v2.05 dscri[.] DFP Shift Significand Right Immediate
111011 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdc DFP Test Data Class
111011 ...// ..... ..... .0111 00010/ Z22 I 202 v2.05 dtstdg DFP Test Data Group
111011 ..... ..... ..... 00000 00010. X I 195 v2.05 dadd[.] DFP Add
111011 ..... ..... ..... 00001 00010. X I 197 v2.05 dmul[.] DFP Multiply
111011 ...// ..... ..... 00100 00010/ X I 201 v2.05 dcmpo DFP Compare Ordered
111011 ...// ..... ..... 00101 00010/ X I 203 v2.05 dtstex DFP Test Exponent
111011 ..... ///// ..... 01000 00010. X I 215 v2.05 dctdp[.] DFP Convert To DFP Long
111011 ..... ///// ..... 01001 00010. X I 217 v2.05 dctfix[.] DFP Convert To Fixed
111011 ..... ../// ..... 01010 00010. X I 219 v2.05 ddedpd[.] DFP Decode DPD To BCD
111011 ..... ///// ..... 01011 00010. X I 220 v2.05 dxex[.] DFP Extract Exponent
111011 ..... ..... ..... 10000 00010. X I 195 v2.05 dsub[.] DFP Subtract
111011 ..... ..... ..... 10001 00010. X I 198 v2.05 ddiv[.] DFP Divide
111011 ...// ..... ..... 10100 00010/ X I 200 v2.05 dcmpu DFP Compare Unordered
111011 ...// ..... ..... 10101 00010/ X I 204 v2.05 dtstsf DFP Test Significance
111011 ..... ///// ..... 11000 00010. X I 216 v2.05 drsp[.] DFP Round To DFP Short
111011 ..... ///// ..... 11001 00010. X I 217 v2.06 dcffix[.] DFP Convert From Fixed
111011 ..... .//// ..... 11010 00010. X I 219 v2.05 denbcd[.] DFP Encode BCD To DPD
111011 ..... ..... ..... 11011 00010. X I 220 v2.05 diex[.] DFP Insert Exponent

111011 ..... ..... ..... ..000 00011. Z23 I 206 v2.05 dqua[.] DFP Quantize
111011 ..... ..... ..... ..001 00011. Z23 I 208 v2.05 drrnd[.] DFP Reround
111011 ..... ..... ..... ..010 00011. Z23 I 205 v2.05 dquai[.] DFP Quantize Immediate
111011 ..... ////. ..... ..011 00011. Z23 I 211 v2.05 drintx[.] DFP Round To FP Integer With Inexact
111011 ..... ////. ..... ..111 00011. Z23 I 213 v2.05 drintn[.] DFP Round To FP Integer Without Inexact
111011 ...// ..... ..... 10101 00011/ X I 204 v3.0 dtstsfi DFP Test Significance Immediate

111011 ..... ///// ..... 11010 01110. X I 165 v2.06 fcfids[.] Floating Convert From Integer Dword Single
111011 ..... ///// ..... 11110 01110. X I 166 v2.06 fcfidus[.] Floating Convert From Integer Dword Unsigned Single

111011 ..... ..... ..... ///// 10010. A I 154 PPC fdivs[.] Floating Divide Single

111011 ..... ..... ..... ///// 10100. A I 153 PPC fsubs[.] Floating Subtract Single

111011 ..... ..... ..... ///// 10101. A I 153 PPC fadds[.] Floating Add Single

111011 ..... ///// ..... ///// 10110. A I 155 PPC fsqrts[.] Floating Square Root Single

111011 ..... ///// ..... ///// 11000. A I 155 PPC fres[.] Floating Reciprocal Estimate Single

111011 ..... ..... ///// ..... 11001. A I 154 PPC fmuls[.] Floating Multiply Single

111011 ..... ///// ..... ///// 11010. A I 156 v2.02 frsqrtes[.] Floating Reciprocal Square Root Estimate Single
111011 ..... ..... ..... ..... 11100. A I 159 PPC fmsubs[.] Floating Multiply-Subtract Single
111011 ..... ..... ..... ..... 11101. A I 158 PPC fmadds[.] Floating Multiply-Add Single
111011 ..... ..... ..... ..... 11110. A I 159 PPC fnmsubs[.] Floating Negative Multiply-Subtract Single
111011 ..... ..... ..... ..... 11111. A I 159 PPC fnmadds[.] Floating Negative Multiply-Add Single
 */ 
        }

        public virtual Decoder Ext3CDecoder()
        {
            return new XX3Decoder(new Dictionary<uint, Decoder>                // 3C
            {
                { 0x00, Instr(Mnemonic.xsaddsp, v1,v2,v3) },
                { 0x01, Instr(Mnemonic.xsmaddasp, v1,v2,v3) },
                //{ 0x02, Instr(Mnemonic.xxsldwi, v1,v2,v3) },       //$TODO need extra work.
                { 0x09, Instr(Mnemonic.xsmaddmsp, v1,v2,v3) },
            });
/*
111100 ..... ..... ..... 00000 000... XX3 I 519 v2.07 xsaddsp VSX Scalar Add SP
111100 ..... ..... ..... 00001 000... XX3 I 651 v2.07 xssubsp VSX Scalar Subtract SP
111100 ..... ..... ..... 00010 000... XX3 I 606 v2.07 xsmulsp VSX Scalar Multiply SP
111100 ..... ..... ..... 00011 000... XX3 I 568 v2.07 xsdivsp VSX Scalar Divide SP
111100 ..... ..... ..... 00100 000... XX3 I 514 v2.06 xsadddp VSX Scalar Add DP
111100 ..... ..... ..... 00101 000... XX3 I 647 v2.06 xssubdp VSX Scalar Subtract DP
111100 ..... ..... ..... 00110 000... XX3 I 602 v2.06 xsmuldp VSX Scalar Multiply DP
111100 ..... ..... ..... 00111 000... XX3 I 564 v2.06 xsdivdp VSX Scalar Divide DP
111100 ..... ..... ..... 01000 000... XX3 I 665 v2.06 xvaddsp VSX Vector Add SP
111100 ..... ..... ..... 01001 000... XX3 I 759 v2.06 xvsubsp VSX Vector Subtract SP
111100 ..... ..... ..... 01010 000... XX3 I 727 v2.06 xvmulsp VSX Vector Multiply SP
111100 ..... ..... ..... 01011 000... XX3 I 702 v2.06 xvdivsp VSX Vector Divide SP
111100 ..... ..... ..... 01100 000... XX3 I 661 v2.06 xvadddp VSX Vector Add DP
111100 ..... ..... ..... 01101 000... XX3 I 757 v2.06 xvsubdp VSX Vector Subtract DP
111100 ..... ..... ..... 01110 000... XX3 I 725 v2.06 xvmuldp VSX Vector Multiply DP
111100 ..... ..... ..... 01111 000... XX3 I 700 v2.06 xvdivdp VSX Vector Divide DP
111100 ..... ..... ..... 10000 000... XX3 I 583 v3.0 xsmaxcdp VSX Scalar Maximum Type-C Double-Precision
111100 ..... ..... ..... 10001 000... XX3 I 589 v3.0 xsmincdp VSX Scalar Minimum Type-C Double-Precision
111100 ..... ..... ..... 10010 000... XX3 I 585 v3.0 xsmaxjdp VSX Scalar Maximum Type-J Double-Precision
111100 ..... ..... ..... 10011 000... XX3 I 591 v3.0 xsminjdp VSX Scalar Minimum Type-J Double-Precision
111100 ..... ..... ..... 10100 000... XX3 I 581 v2.06 xsmaxdp VSX Scalar Maximum DP
111100 ..... ..... ..... 10101 000... XX3 I 587 v2.06 xsmindp VSX Scalar Minimum DP
111100 ..... ..... ..... 10110 000... XX3 I 535 v2.06 xscpsgndp VSX Scalar Copy Sign DP
111100 ..... ..... ..... 11000 000... XX3 I 713 v2.06 xvmaxsp VSX Vector Maximum SP
111100 ..... ..... ..... 11001 000... XX3 I 717 v2.06 xvminsp VSX Vector Minimum SP
111100 ..... ..... ..... 11010 000... XX3 I 675 v2.06 xvcpsgnsp VSX Vector Copy Sign SP
111100 ..... ..... ..... 11011 000... XX3 I 704 v3.0 xviexpsp VSX Vector Insert Exponent SP
111100 ..... ..... ..... 11100 000... XX3 I 711 v2.06 xvmaxdp VSX Vector Maximum DP
111100 ..... ..... ..... 11101 000... XX3 I 715 v2.06 xvmindp VSX Vector Minimum DP
111100 ..... ..... ..... 11110 000... XX3 I 675 v2.06 xvcpsgndp VSX Vector Copy Sign DP
111100 ..... ..... ..... 11111 000... XX3 I 704 v3.0 xviexpdp VSX Vector Insert Exponent DP

111100 ..... ..... ..... 00000 001... XX3 I 575 v2.07 xsmaddasp VSX Scalar Multiply-Add Type-A SP
111100 ..... ..... ..... 00001 001... XX3 I 575 v2.07 xsmaddmsp VSX Scalar Multiply-Add Type-M SP
111100 ..... ..... ..... 00010 001... XX3 I 596 v2.07 xsmsubasp VSX Scalar Multiply-Subtract Type-A SP
111100 ..... ..... ..... 00011 001... XX3 I 596 v2.07 xsmsubmsp VSX Scalar Multiply-Subtract Type-M SP
111100 ..... ..... ..... 00100 001... XX3 I 572 v2.06 xsmaddadp VSX Scalar Multiply-Add Type-A DP
111100 ..... ..... ..... 00101 001... XX3 I 572 v2.06 xsmaddmdp VSX Scalar Multiply-Add Type-M DP
111100 ..... ..... ..... 00110 001... XX3 I 593 v2.06 xsmsubadp VSX Scalar Multiply-Subtract Type-A DP
111100 ..... ..... ..... 00111 001... XX3 I 593 v2.06 xsmsubmdp VSX Scalar Multiply-Subtract Type-M DP
111100 ..... ..... ..... 01000 001... XX3 I 708 v2.06 xvmaddasp VSX Vector Multiply-Add Type-A SP
111100 ..... ..... ..... 01001 001... XX3 I 708 v2.06 xvmaddmsp VSX Vector Multiply-Add Type-M SP
111100 ..... ..... ..... 01010 001... XX3 I 722 v2.06 xvmsubasp VSX Vector Multiply-Subtract Type-A SP
111100 ..... ..... ..... 01011 001... XX3 I 722 v2.06 xvmsubmsp VSX Vector Multiply-Subtract Type-M SP
111100 ..... ..... ..... 01100 001... XX3 I 705 v2.06 xvmaddadp VSX Vector Multiply-Add Type-A DP
111100 ..... ..... ..... 01101 001... XX3 I 705 v2.06 xvmaddmdp VSX Vector Multiply-Add Type-M DP
111100 ..... ..... ..... 01110 001... XX3 I 719 v2.06 xvmsubadp VSX Vector Multiply-Subtract Type-A DP
111100 ..... ..... ..... 01111 001... XX3 I 719 v2.06 xvmsubmdp VSX Vector Multiply-Subtract Type-M DP
111100 ..... ..... ..... 10000 001... XX3 I 615 v2.07 xsnmaddasp VSX Scalar Negative Multiply-Add Type-A SP
111100 ..... ..... ..... 10001 001... XX3 I 615 v2.07 xsnmaddmsp VSX Scalar Negative Multiply-Add Type-M SP
111100 ..... ..... ..... 10010 001... XX3 I 624 v2.07 xsnmsubasp VSX Scalar Negative Multiply-Subtract Type-A SP
111100 ..... ..... ..... 10011 001... XX3 I 624 v2.07 xsnmsubmsp VSX Scalar Negative Multiply-Subtract Type-M SP
111100 ..... ..... ..... 10100 001... XX3 I 610 v2.06 xsnmaddadp VSX Scalar Negative Multiply-Add Type-A DP
111100 ..... ..... ..... 10101 001... XX3 I 610 v2.06 xsnmaddmdp VSX Scalar Negative Multiply-Add Type-M DP
111100 ..... ..... ..... 10110 001... XX3 I 621 v2.06 xsnmsubadp VSX Scalar Negative Multiply-Subtract Type-A DP
111100 ..... ..... ..... 10111 001... XX3 I 621 v2.06 xsnmsubmdp VSX Scalar Negative Multiply-Subtract Type-M DP
111100 ..... ..... ..... 11000 001... XX3 I 736 v2.06 xvnmaddasp VSX Vector Negative Multiply-Add Type-A SP
111100 ..... ..... ..... 11001 001... XX3 I 736 v2.06 xvnmaddmsp VSX Vector Negative Multiply-Add Type-M SP
111100 ..... ..... ..... 11010 001... XX3 I 742 v2.06 xvnmsubasp VSX Vector Negative Multiply-Subtract Type-A SP
111100 ..... ..... ..... 11011 001... XX3 I 742 v2.06 xvnmsubmsp VSX Vector Negative Multiply-Subtract Type-M SP
111100 ..... ..... ..... 11100 001... XX3 I 731 v2.06 xvnmaddadp VSX Vector Negative Multiply-Add Type-A DP
111100 ..... ..... ..... 11101 001... XX3 I 731 v2.06 xvnmaddmdp VSX Vector Negative Multiply-Add Type-M DP
111100 ..... ..... ..... 11110 001... XX3 I 739 v2.06 xvnmsubadp VSX Vector Negative Multiply-Subtract Type-A DP
111100 ..... ..... ..... 11111 001... XX3 I 739 v2.06 xvnmsubmdp VSX Vector Negative Multiply-Subtract Type-M DP

111100 ..... ..... ..... 0..00 010... XX3 I 778 v2.06 xxsldwi VSX Vector Shift Left Double by Word Immediate
111100 ..... ..... ..... 0..01 010... XX3 I 777 v2.06 xxpermdi VSX Vector Dword Permute Immediate
111100 ..... ..... ..... 00010 010... XX3 I 775 v2.06 xxmrghw VSX Vector Merge Word High
111100 ..... ..... ..... 00011 010... XX3 I 776 v3.0 xxperm VSX Vector Permute
111100 ..... ..... ..... 00110 010... XX3 I 775 v2.06 xxmrglw VSX Vector Merge Word Low
111100 ..... ..... ..... 00111 010... XX3 I 776 v3.0 xxpermr VSX Vector Permute Right-indexed
111100 ..... ..... ..... 10000 010... XX3 I 771 v2.06 xxland VSX Vector Logical AND
111100 ..... ..... ..... 10001 010... XX3 I 771 v2.06 xxlandc VSX Vector Logical AND with Complement
111100 ..... ..... ..... 10010 010... XX3 I 774 v2.06 xxlor VSX Vector Logical OR
111100 ..... ..... ..... 10011 010... XX3 I 774 v2.06 xxlxor VSX Vector Logical XOR
111100 ..... ..... ..... 10100 010... XX3 I 773 v2.06 xxlnor VSX Vector Logical NOR
111100 ..... ..... ..... 10101 010... XX3 I 773 v2.07 xxlorc VSX Vector Logical OR with Complement
111100 ..... ..... ..... 10110 010... XX3 I 772 v2.07 xxlnand VSX Vector Logical NAND
111100 ..... ..... ..... 10111 010... XX3 I 772 v2.07 xxleqv VSX Vector Logical Equivalence
111100 ..... ///.. ..... 01010 0100.. XX2 I 778 v2.06 xxspltw VSX Vector Splat Word
111100 ..... 00... ..... 01011 01000. XX1 I 778 v3.0 xxspltib VSX Vector Splat Immediate Byte
111100 ..... /.... ..... 01010 0101.. XX2 I 770 v3.0 xxextractuw VSX Vector Extract Unsigned Word
111100 ..... /.... ..... 01011 0101.. XX2 I 770 v3.0 xxinsertw VSX Vector Insert Word

111100 ..... ..... ..... .1000 011... XX3 I 668 v2.06 xvcmpeqsp[.] VSX Vector Compare Equal SP
111100 ..... ..... ..... .1001 011... XX3 I 672 v2.06 xvcmpgtsp[.] VSX Vector Compare Greater Than SP
111100 ..... ..... ..... .1010 011... XX3 I 670 v2.06 xvcmpgesp[.] VSX Vector Compare Greater Than or Equal SP
111100 ..... ..... ..... .1011 011... XX3 I 674 v3.0 xvcmpnesp[.] VSX Vector Compare Not Equal Single-Precision
111100 ..... ..... ..... .1100 011... XX3 I 667 v2.06 xvcmpeqdp[.] VSX Vector Compare Equal DP
111100 ..... ..... ..... .1101 011... XX3 I 671 v2.06 xvcmpgtdp[.] VSX Vector Compare Greater Than DP
111100 ..... ..... ..... .1110 011... XX3 I 669 v2.06 xvcmpgedp[.] VSX Vector Compare Greater Than or Equal DP
111100 ..... ..... ..... .1111 011... XX3 I 673 v3.0 xvcmpnedp[.] VSX Vector Compare Not Equal Double-Precision
111100 ..... ..... ..... 00000 011... XX3 I 525 v3.0 xscmpeqdp VSX Scalar Compare Equal Double-Precision
111100 ..... ..... ..... 00001 011... XX3 I 527 v3.0 xscmpgtdp VSX Scalar Compare Greater Than Double-Precision
111100 ..... ..... ..... 00010 011... XX3 I 526 v3.0 xscmpgedp VSX Scalar Compare Greater Than or Equal Double-Precision
111100 ..... ..... ..... 00011 011... XX3 I 528 v3.0 xscmpnedp VSX Scalar Compare Not Equal Double-Precision
111100 ...// ..... ..... 00100 011../ XX3 I 532 v2.06 xscmpudp VSX Scalar Compare Unordered DP
111100 ...// ..... ..... 00101 011../ XX3 I 529 v2.06 xscmpodp VSX Scalar Compare Ordered DP
111100 ...// ..... ..... 00111 011../ XX3 I 523 v3.0 xscmpexpdp VSX Scalar Compare Exponents DP

111100 ..... ///// ..... 00100 1000.. XX2 I 546 v2.06 xscvdpuxws VSX Scalar Convert DP to Unsigned Word truncate
111100 ..... ///// ..... 00101 1000.. XX2 I 542 v2.06 xscvdpsxws VSX Scalar Convert DP to Signed Word truncate
111100 ..... ///// ..... 01000 1000.. XX2 I 694 v2.06 xvcvspuxws VSX Vector Convert SP to Unsigned Word truncate
111100 ..... ///// ..... 01001 1000.. XX2 I 690 v2.06 xvcvspsxws VSX Vector Convert SP to Signed Word truncate
111100 ..... ///// ..... 01010 1000.. XX2 I 699 v2.06 xvcvuxwsp VSX Vector Convert Unsigned Word to SP
111100 ..... ///// ..... 01011 1000.. XX2 I 697 v2.06 xvcvsxwsp VSX Vector Convert Signed Word to SP
111100 ..... ///// ..... 01100 1000.. XX2 I 683 v2.06 xvcvdpuxws VSX Vector Convert DP to Unsigned Word truncate
111100 ..... ///// ..... 01101 1000.. XX2 I 679 v2.06 xvcvdpsxws VSX Vector Convert DP to Signed Word truncate
111100 ..... ///// ..... 01110 1000.. XX2 I 699 v2.06 xvcvuxwdp VSX Vector Convert Unsigned Word to DP
111100 ..... ///// ..... 01111 1000.. XX2 I 697 v2.06 xvcvsxwdp VSX Vector Convert Signed Word to DP
111100 ..... ///// ..... 10010 1000.. XX2 I 563 v2.07 xscvuxdsp VSX Scalar Convert Unsigned Dword to SP
111100 ..... ///// ..... 10011 1000.. XX2 I 561 v2.07 xscvsxdsp VSX Scalar Convert Signed Dword to SP
111100 ..... ///// ..... 10100 1000.. XX2 I 544 v2.06 xscvdpuxds VSX Scalar Convert DP to Unsigned Dword truncate
111100 ..... ///// ..... 10101 1000.. XX2 I 539 v2.06 xscvdpsxds VSX Scalar Convert DP to Signed Dword truncate
111100 ..... ///// ..... 10110 1000.. XX2 I 563 v2.06 xscvuxddp VSX Scalar Convert Unsigned Dword to DP
111100 ..... ///// ..... 10111 1000.. XX2 I 561 v2.06 xscvsxddp VSX Scalar Convert Signed Dword to DP
111100 ..... ///// ..... 11000 1000.. XX2 I 692 v2.06 xvcvspuxds VSX Vector Convert SP to Unsigned Dword truncate
111100 ..... ///// ..... 11001 1000.. XX2 I 688 v2.06 xvcvspsxds VSX Vector Convert SP to Signed Dword truncate
111100 ..... ///// ..... 11010 1000.. XX2 I 698 v2.06 xvcvuxdsp VSX Vector Convert Unsigned Dword to SP
111100 ..... ///// ..... 11011 1000.. XX2 I 696 v2.06 xvcvsxdsp VSX Vector Convert Signed Dword to SP
111100 ..... ///// ..... 11100 1000.. XX2 I 681 v2.06 xvcvdpuxds VSX Vector Convert DP to Unsigned Dword truncate
111100 ..... ///// ..... 11101 1000.. XX2 I 677 v2.06 xvcvdpsxds VSX Vector Convert DP to Signed Dword truncate
111100 ..... ///// ..... 11110 1000.. XX2 I 698 v2.06 xvcvuxddp VSX Vector Convert Unsigned Dword to DP
111100 ..... ///// ..... 11111 1000.. XX2 I 696 v2.06 xvcvsxddp VSX Vector Convert Signed Dword to DP

111100 ..... ///// ..... 00100 1001.. XX2 I 630 v2.06 xsrdpi VSX Scalar Round DP to Integral to Nearest Away
111100 ..... ///// ..... 00101 1001.. XX2 I 633 v2.06 xsrdpiz VSX Scalar Round DP to Integral toward Zero
111100 ..... ///// ..... 00110 1001.. XX2 I 632 v2.06 xsrdpip VSX Scalar Round DP to Integral toward +Infinity
111100 ..... ///// ..... 00111 1001.. XX2 I 632 v2.06 xsrdpim VSX Scalar Round DP to Integral toward -Infinity
111100 ..... ///// ..... 01000 1001.. XX2 I 750 v2.06 xvrspi VSX Vector Round SP to Integral to Nearest Away
111100 ..... ///// ..... 01001 1001.. XX2 I 752 v2.06 xvrspiz VSX Vector Round SP to Integral toward Zero
111100 ..... ///// ..... 01010 1001.. XX2 I 751 v2.06 xvrspip VSX Vector Round SP to Integral toward +Infinity
111100 ..... ///// ..... 01011 1001.. XX2 I 751 v2.06 xvrspim VSX Vector Round SP to Integral toward -Infinity
111100 ..... ///// ..... 01100 1001.. XX2 I 745 v2.06 xvrdpi VSX Vector Round DP to Integral to Nearest Away
111100 ..... ///// ..... 01101 1001.. XX2 I 747 v2.06 xvrdpiz VSX Vector Round DP to Integral toward Zero
111100 ..... ///// ..... 01110 1001.. XX2 I 746 v2.06 xvrdpip VSX Vector Round DP to Integral toward +Infinity
111100 ..... ///// ..... 01111 1001.. XX2 I 746 v2.06 xvrdpim VSX Vector Round DP to Integral toward -Infinity
111100 ..... ///// ..... 10000 1001.. XX2 I 538 v2.06 xscvdpsp VSX Scalar Convert DP to SP
111100 ..... ///// ..... 10001 1001.. XX2 I 640 v2.07 xsrsp VSX Scalar Round DP to SP
111100 ..... ///// ..... 10100 1001.. XX2 I 559 v2.06 xscvspdp VSX Scalar Convert SP to DP
111100 ..... ///// ..... 10101 1001.. XX2 I 513 v2.06 xsabsdp VSX Scalar Absolute DP
111100 ..... ///// ..... 10110 1001.. XX2 I 608 v2.06 xsnabsdp VSX Scalar Negative Absolute DP
111100 ..... ///// ..... 10111 1001.. XX2 I 609 v2.06 xsnegdp VSX Scalar Negate DP
111100 ..... ///// ..... 11000 1001.. XX2 I 676 v2.06 xvcvdpsp VSX Vector Convert DP to SP
111100 ..... ///// ..... 11001 1001.. XX2 I 660 v2.06 xvabssp VSX Vector Absolute SP
111100 ..... ///// ..... 11010 1001.. XX2 I 729 v2.06 xvnabssp VSX Vector Negative Absolute SP
111100 ..... ///// ..... 11011 1001.. XX2 I 730 v2.06 xvnegsp VSX Vector Negate SP
111100 ..... ///// ..... 11100 1001.. XX2 I 686 v2.06 xvcvspdp VSX Vector Convert SP to DP
111100 ..... ///// ..... 11101 1001.. XX2 I 660 v2.06 xvabsdp VSX Vector Absolute DP
111100 ..... ///// ..... 11110 1001.. XX2 I 729 v2.06 xvnabsdp VSX Vector Negative Absolute DP
111100 ..... ///// ..... 11111 1001.. XX2 I 730 v2.06 xvnegdp VSX Vector Negate DP

111100 ...// ..... ..... 00111 101../ XX3 I 653 v2.06 xstdivdp VSX Scalar Test for software Divide DP
111100 ...// ..... ..... 01011 101../ XX3 I 762 v2.06 xvtdivsp VSX Vector Test for software Divide SP
111100 ...// ..... ..... 01111 101../ XX3 I 761 v2.06 xvtdivdp VSX Vector Test for software Divide DP
111100 ..... ..... ..... 1101. 101... XX2 I 765 v3.0 xvtstdcsp VSX Vector Test Data Class SP
111100 ..... ..... ..... 1111. 101... XX2 I 764 v3.0 xvtstdcdp VSX Vector Test Data Class DP
111100 ..... ///// ..... 00000 1010.. XX2 I 642 v2.07 xsrsqrtesp VSX Scalar Reciprocal Square Root Estimate SP
111100 ..... ///// ..... 00001 1010.. XX2 I 635 v2.07 xsresp VSX Scalar Reciprocal Estimate SP
111100 ..... ///// ..... 00100 1010.. XX2 I 641 v2.06 xsrsqrtedp VSX Scalar Reciprocal Square Root Estimate DP
111100 ..... ///// ..... 00101 1010.. XX2 I 634 v2.06 xsredp VSX Scalar Reciprocal Estimate DP
111100 ...// ///// ..... 00110 1010./ XX2 I 654 v2.06 xstsqrtdp VSX Scalar Test for software Square Root DP
111100 ..... ///// ..... 01000 1010.. XX2 I 754 v2.06 xvrsqrtesp VSX Vector Reciprocal Square Root Estimate SP
111100 ..... ///// ..... 01001 1010.. XX2 I 749 v2.06 xvresp VSX Vector Reciprocal Estimate SP
111100 ...// ///// ..... 01010 1010./ XX2 I 763 v2.06 xvtsqrtsp VSX Vector Test for software Square Root SP
111100 ..... ///// ..... 01100 1010.. XX2 I 752 v2.06 xvrsqrtedp VSX Vector Reciprocal Square Root Estimate DP
111100 ..... ///// ..... 01101 1010.. XX2 I 748 v2.06 xvredp VSX Vector Reciprocal Estimate DP
111100 ...// ///// ..... 01110 1010./ XX2 I 763 v2.06 xvtsqrtdp VSX Vector Test for software Square Root DP
111100 ..... ..... ..... 10010 1010./ XX2 I 657 v3.0 xststdcsp VSX Scalar Test Data Class SP
111100 ..... ..... ..... 10110 1010./ XX2 I 655 v3.0 xststdcdp VSX Scalar Test Data Class DP
111100 ..... ///// ..... 00000 1011.. XX2 I 646 v2.07 xssqrtsp VSX Scalar Square Root SP
111100 ..... ///// ..... 00100 1011.. XX2 I 643 v2.06 xssqrtdp VSX Scalar Square Root DP
111100 ..... ///// ..... 00110 1011.. XX2 I 631 v2.06 xsrdpic VSX Scalar Round DP to Integral using Current rounding mode
111100 ..... ///// ..... 01000 1011.. XX2 I 756 v2.06 xvsqrtsp VSX Vector Square Root SP
111100 ..... ///// ..... 01010 1011.. XX2 I 750 v2.06 xvrspic VSX Vector Round SP to Integral using Current rounding mode
111100 ..... ///// ..... 01100 1011.. XX2 I 755 v2.06 xvsqrtdp VSX Vector Square Root DP
111100 ..... ///// ..... 01110 1011.. XX2 I 745 v2.06 xvrdpic VSX Vector Round DP to Integral using Current rounding mode
111100 ..... ///// ..... 10000 1011.. XX2 I 539 v2.07 xscvdpspn VSX Scalar Convert DP to SP Non-signalling
111100 ..... ///// ..... 10100 1011.. XX2 I 560 v2.07 xscvspdpn VSX Scalar Convert SP to DP Non-signalling
111100 ..... 00000 ..... 10101 1011./ XX2 I 658 v3.0 xsxexpdp VSX Scalar Extract Exponent DP
111100 ..... 00001 ..... 10101 1011./ XX2 I 659 v3.0 xsxsigdp VSX Scalar Extract Significand DP
111100 ..... 10000 ..... 10101 1011.. XX2 I 548 v3.0 xscvhpdp VSX Scalar Convert HP to DP
111100 ..... 10001 ..... 10101 1011.. XX2 I 536 v3.0 xscvdphp VSX Scalar Convert DP to HP
111100 ..... 00000 ..... 11101 1011.. XX2 I 766 v3.0 xvxexpdp VSX Vector Extract Exponent DP
111100 ..... 00001 ..... 11101 1011.. XX2 I 767 v3.0 xvxsigdp VSX Vector Extract Significand DP
111100 ..... 00111 ..... 11101 1011.. XX2 I 768 v3.0 xxbrh VSX Vector Byte-Reverse Hword
111100 ..... 01000 ..... 11101 1011.. XX2 I 766 v3.0 xvxexpsp VSX Vector Extract Exponent SP
111100 ..... 01001 ..... 11101 1011.. XX2 I 767 v3.0 xvxsigsp VSX Vector Extract Significand SP
111100 ..... 01111 ..... 11101 1011.. XX2 I 769 v3.0 xxbrw VSX Vector Byte-Reverse Word
111100 ..... 10111 ..... 11101 1011.. XX2 I 768 v3.0 xxbrd VSX Vector Byte-Reverse Dword
111100 ..... 11000 ..... 11101 1011.. XX2 I 685 v3.0 xvcvhpsp VSX Vector Convert HP to SP
111100 ..... 11001 ..... 11101 1011.. XX2 I 687 v3.0 xvcvsphp VSX Vector Convert SP to HP
111100 ..... 11111 ..... 11101 1011.. XX2 I 769 v3.0 xxbrq VSX Vector Byte-Reverse Qword
111100 ..... ..... ..... 11100 10110. XX1 I 570 v3.0 xsiexpdp VSX Scalar Insert Exponent DP

111100 ..... ..... ..... ..... 11.... XX4 I 777 v2.06 xxsel VSX Vector Select
 */
        }

        public virtual Decoder Ext3DDecoder()
        {
            return Instr(Mnemonic.stfdp, p1, E2_2);
            /*
            111101 ..... ..... ..... ..... ....00 DS I 150 v2.05 stfdp Store Floating Double Pair
            111101 ..... ..... ..... ..... ....10 DS I 499 v3.0 stxsd Store VSX Scalar Dword
            111101 ..... ..... ..... ..... ....11 DS I 502 v3.0 stxssp Store VSX Scalar SP
            111101 ..... ..... ..... ..... ...001 DQ I 493 v3.0 lxv Load VSX Vector
            111101 ..... ..... ..... ..... ...101 DQ I 508 v3.0 stxv Store VSX Vector
            */

        }

        public Decoder Ext3FDecoder()
                    {
                        return new FpuDecoder(1, 0x1F, new Dictionary<uint, Decoder>()     // 3F
                            {
                                { 0x00, new FpuDecoder(6, 0x1F, new Dictionary<uint, Decoder>
                                    {
                                        { 0, Instr(Mnemonic.fcmpu, C1,f2,f3) },
                                        { 1, Instr(Mnemonic.fcmpo, C1,f2,f3) },
                                        //{ 2, Instr(Mnemonic.mcrfs)}
                                    })
                                },
                                { 0x06, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        //{ 1, Instr(Mnemonic.mtfsb1 },
                                        //{ 2, Instr(Mnemonic.mtfsb0 },
                                        //{ 4, Instr(Mnemonic.mtfsfi }
                                    })
                                },
                                { 0x07, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        { 0x12, Instr(Mnemonic.mffs, C,f1)},
                                        { 0x16, Instr(Mnemonic.mtfsf, u17_8,f3 )},
                                    })
                                },
                                { 0x08, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        { 1, Instr(Mnemonic.fneg, C,f1,f3) },
                                        { 2, Instr(Mnemonic.fmr, C,f1,f3)},
                                        { 4, Instr(Mnemonic.fnabs, C,f1,f3) },
                                        { 8, Instr(Mnemonic.fabs, C,f1,f3) },
                                    })
                                },
                                { 0x0C, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        { 0, Instr(Mnemonic.frsp, C,f1,f3) },
                                    })
                                },
                                { 0x0E, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        { 0x00, Instr(Mnemonic.fctiw, C,f1,f3) },
                                        { 0x19, Instr(Mnemonic.fctid, C,f1,f3) },
                                        { 0x1A, Instr(Mnemonic.fcfid,  C,f1,f3) },
                                    })
                                },
                                { 0x0F, new FpuDecoder(6, 0x1F, new Dictionary<uint,Decoder>
                                    {
                                        { 0x00, Instr(Mnemonic.fctiwz,  C,f1,f3) },
                                        { 0x19, Instr(Mnemonic.fctidz,   C,f1,f3) },
                                    })
                                },

                                { 18, Instr(Mnemonic.fdiv, C,f1,f2,f3) },
                                { 20, Instr(Mnemonic.fsub, C,f1,f2,f3) },
                                { 21, Instr(Mnemonic.fadd, C,f1,f2,f3) },
                                { 0x16, Instr(Mnemonic.fsqrt, C,f1,f3) },
                                { 0x17, Instr(Mnemonic.fsel, C,f1,f2,f4,f3) },
                                { 0x19, Instr(Mnemonic.fmul, C,f1,f2,f4) },
                                { 0x1A, Instr(Mnemonic.frsqrte, C,f1,f3) },

                                { 0x1C, Instr(Mnemonic.fmsub, C,f1,f2,f4,f3) },

                                { 0x1D, Instr(Mnemonic.fmadd, C,f1,f2,f4,f3) },
                                { 0x1E, Instr(Mnemonic.fnmsub, C,f1,f2,f4,f3) },
                                { 0x1F, Instr(Mnemonic.fnmadd, C,f1,f2,f4,f3) },
                            });
                        /*
                        111111 ...// ..... ..... 00000 00000/ X I 168 P1 fcmpu Floating Compare Unordered
                        111111 ...// ..... ..... 00001 00000/ X I 168 P1 fcmpo Floating Compare Ordered
                        111111 ...// ...// ///// 00010 00000/ X I 171 P1 mcrfs Move To CR from FPSCR
                        111111 ...// ..... ..... 00100 00000/ X I 157 v2.06 ftdiv Floating Test for software Divide
                        111111 ...// ///// ..... 00101 00000/ X I 157 v2.06 ftsqrt Floating Test for software Square Root
                        111111 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscliq[.] DFP Shift Significand Left Immediate Quad
                        111111 ..... ..... ..... .0011 00010. Z22 I 222 v2.05 dscriq[.] DFP Shift Significand Right Immediate Quad
                        111111 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdcq DFP Test Data Class Quad
                        111111 ...// ..... ..... .0111 00010/ Z22 I 202 v2.05 dtstdgq DFP Test Data Group Quad
                        111111 ..... ..... ..... 00000 00010. X I 195 v2.05 daddq[.] DFP Add Quad
                        111111 ..... ..... ..... 00001 00010. X I 197 v2.05 dmulq[.] DFP Multiply Quad
                        111111 ...// ..... ..... 00100 00010/ X I 201 v2.05 dcmpoq DFP Compare Ordered Quad
                        111111 ...// ..... ..... 00101 00010/ X I 203 v2.05 dtstexq DFP Test Exponent Quad
                        111111 ..... ///// ..... 01000 00010. X I 215 v2.05 dctqpq[.] DFP Convert To DFP Extended
                        111111 ..... ///// ..... 01001 00010. X I 217 v2.05 dctfixq[.] DFP Convert To Fixed Quad
                        111111 ..... ../// ..... 01010 00010. X I 219 v2.05 ddedpdq[.] DFP Decode DPD To BCD Quad
                        111111 ..... ///// ..... 01011 00010. X I 220 v2.05 dxexq[.] DFP Extract Exponent Quad
                        111111 ..... ..... ..... 10000 00010. X I 195 v2.05 dsubq[.] DFP Subtract Quad
                        111111 ..... ..... ..... 10001 00010. X I 198 v2.05 ddivq[.] DFP Divide Quad
                        111111 ...// ..... ..... 10100 00010/ X I 200 v2.05 dcmpuq DFP Compare Unordered Quad
                        111111 ...// ..... ..... 10101 00010/ X I 204 v2.05 dtstsfq DFP Test Significance Quad
                        111111 ..... ///// ..... 11000 00010. X I 216 v2.05 drdpq[.] DFP Round To DFP Long
                        111111 ..... ///// ..... 11001 00010. X I 217 v2.05 dcffixq[.] DFP Convert From Fixed Quad
                        111111 ..... .//// ..... 11010 00010. X I 219 v2.05 denbcdq[.] DFP Encode BCD To DPD Quad
                        111111 ..... ..... ..... 11011 00010. X I 220 v2.05 diexq[.] DFP Insert Exponent Quad
                        111111 ..... ..... ..... ..000 00011. Z23 I 206 v2.05 dquaq[.] DFP Quantize Quad
                        111111 ..... ..... ..... ..001 00011. Z23 I 208 v2.05 drrndq[.] DFP Reround Quad
                        111111 ..... ..... ..... ..010 00011. Z23 I 205 v2.05 dquaiq[.] DFP Quantize Immediate Quad
                        111111 ..... ////. ..... ..011 00011. Z23 I 211 v2.05 drintxq[.] DFP Round To FP Integer With Inexact Quad
                        111111 ..... ////. ..... ..111 00011. Z23 I 213 v2.05 drintnq[.] DFP Round To FP Integer Without Inexact Quad
                        111111 ...// ..... ..... 10101 00011/ X I 204 v3.0 dtstsfiq DFP Test Significance Immediate Quad
                        111111 ..... ..... ..... 00000 00100. X I 521 v3.0 xsaddqp[o] VSX Scalar Add QP
                        111111 ..... ..... ..... 00001 00100. X I 604 v3.0 xsmulqp[o] VSX Scalar Multiply QP
                        111111 ..... ..... ..... 00011 00100/ X I 535 v3.0 xscpsgnqp VSX Scalar Copy Sign QP
                        111111 ...// ..... ..... 00100 00100/ X I 531 v3.0 xscmpoqp VSX Scalar Compare Ordered QP
                        111111 ...// ..... ..... 00101 00100/ X I 524 v3.0 xscmpexpqp VSX Scalar Compare Exponents QP
                        111111 ..... ..... ..... 01100 00100. X I 578 v3.0 xsmaddqp[o] VSX Scalar Multiply-Add QP
                        111111 ..... ..... ..... 01101 00100. X I 599 v3.0 xsmsubqp[o] VSX Scalar Multiply-Subtract QP
                        111111 ..... ..... ..... 01110 00100. X I 618 v3.0 xsnmaddqp[o] VSX Scalar Negative Multiply-Add QP
                        111111 ..... ..... ..... 01111 00100. X I 627 v3.0 xsnmsubqp[o] VSX Scalar Negative Multiply-Subtract QP
                        111111 ..... ..... ..... 10000 00100. X I 649 v3.0 xssubqp[o] VSX Scalar Subtract QP
                        111111 ..... ..... ..... 10001 00100. X I 566 v3.0 xsdivqp[o] VSX Scalar Divide QP
                        111111 ...// ..... ..... 10100 00100/ X I 534 v3.0 xscmpuqp VSX Scalar Compare Unordered QP
                        111111 ..... ..... ..... 10110 00100/ X I 656 v3.0 xststdcqp VSX Scalar Test Data Class QP
                        111111 ..... 00000 ..... 11001 00100/ X I 513 v3.0 xsabsqp VSX Scalar Absolute QP
                        111111 ..... 00010 ..... 11001 00100/ X I 658 v3.0 xsxexpqp VSX Scalar Extract Exponent QP
                        111111 ..... 01000 ..... 11001 00100/ X I 608 v3.0 xsnabsqp VSX Scalar Negative Absolute QP
                        111111 ..... 10000 ..... 11001 00100/ X I 609 v3.0 xsnegqp VSX Scalar Negate QP
                        111111 ..... 10010 ..... 11001 00100/ X I 659 v3.0 xsxsigqp VSX Scalar Extract Significand QP
                        111111 ..... 11011 ..... 11001 00100. X I 644 v3.0 xssqrtqp[o] VSX Scalar Square Root QP
                        111111 ..... 00001 ..... 11010 00100/ X I 556 v3.0 xscvqpuwz VSX Scalar Convert QP to Unsigned Word truncate
                        111111 ..... 00010 ..... 11010 00100/ X I 562 v3.0 xscvudqp VSX Scalar Convert Unsigned Dword to QP
                        111111 ..... 01001 ..... 11010 00100/ X I 552 v3.0 xscvqpswz VSX Scalar Convert QP to Signed Word truncate
                        111111 ..... 01010 ..... 11010 00100/ X I 558 v3.0 xscvsdqp VSX Scalar Convert Signed Dword to QP
                        111111 ..... 10001 ..... 11010 00100/ X I 554 v3.0 xscvqpudz VSX Scalar Convert QP to Unsigned Dword truncate
                        111111 ..... 10100 ..... 11010 00100. X I 549 v3.0 xscvqpdp[o] VSX Scalar Convert QP to DP
                        111111 ..... 10110 ..... 11010 00100/ X I 537 v3.0 xscvdpqp VSX Scalar Convert DP to QP
                        111111 ..... 11001 ..... 11010 00100/ X I 550 v3.0 xscvqpsdz VSX Scalar Convert QP to Signed Dword truncate
                        111111 ..... ..... ..... 11011 00100/ X I 571 v3.0 xsiexpqp VSX Scalar Insert Exponent QP
                        111111 ..... ////. ..... ..000 00101. Z23 I 636 v3.0 xsrqpi[x] VSX Scalar Round QP to Integral
                        111111 ..... ////. ..... ..001 00101/ Z23 I 638 v3.0 xsrqpxp VSX Scalar Round QP to XP
                        111111 ..... ///// ///// 00001 00110. X I 173 P1 mtfsb1[.] Move To FPSCR Bit 1
                        111111 ..... ///// ///// 00010 00110. X I 173 P1 mtfsb0[.] Move To FPSCR Bit 0
                        111111 ...// ////. ..../ 00100 00110. X I 172 P1 mtfsfi[.] Move To FPSCR Field Immediate
                        111111 ..... ..... ..... 11010 00110/ X I 152 v2.07 fmrgow Floating Merge Odd Word
                        111111 ..... ..... ..... 11110 00110/ X I 151 v2.07 fmrgew Floating Merge Even Word
                        111111 ..... ///// ///// 10010 00111. X I 171 P1 mffs[.] Move From FPSCR
                        111111 ..... ..... ..... 10110 00111. XFL I 172 P1 mtfsf[.] Move To FPSCR Fields
                        111111 ..... ..... ..... 00000 01000. X I 151 v2.05 fcpsgn[.] Floating Copy Sign
                        111111 ..... ///// ..... 00001 01000. X I 151 P1 fneg[.] Floating Negate
                        111111 ..... ///// ..... 00010 01000. X I 151 P1 fmr[.] Floating Move Register
                        111111 ..... ///// ..... 00100 01000. X I 151 P1 fnabs[.] Floating Negative Absolute Value
                        111111 ..... ///// ..... 01000 01000. X I 151 P1 fabs[.] Floating Absolute
                        111111 ..... ///// ..... 01100 01000. X I 167 v2.02 frin[.] Floating Round To Integer Nearest
                        111111 ..... ///// ..... 01101 01000. X I 167 v2.02 friz[.] Floating Round To Integer Zero
                        111111 ..... ///// ..... 01110 01000. X I 167 v2.02 frip[.] Floating Round To Integer Plus
                        111111 ..... ///// ..... 01111 01000. X I 167 v2.02 frim[.] Floating Round To Integer Minus
                        111111 ..... ///// ..... 00000 01100. X I 160 P1 frsp[.] Floating Round to SP
                        111111 ..... ///// ..... 00000 01110. X I 162 P2 fctiw[.] Floating Convert To Integer Word
                        111111 ..... ///// ..... 00100 01110. X I 163 v2.06 fctiwu[.] Floating Convert To Integer Word Unsigned
                        111111 ..... ///// ..... 11001 01110. X I 160 PPC fctid[.] Floating Convert To Integer Dword
                        111111 ..... ///// ..... 11010 01110. X I 164 PPC fcfid[.] Floating Convert From Integer Dword
                        111111 ..... ///// ..... 11101 01110. X I 161 v2.06 fctidu[.] Floating Convert To Integer Dword Unsigned
                        111111 ..... ///// ..... 11110 01110. X I 165 v2.06 fcfidu[.] Floating Convert From Integer Dword Unsigned
                        111111 ..... ///// ..... 00000 01111. X I 163 P2 fctiwz[.] Floating Convert To Integer Word truncate
                        111111 ..... ///// ..... 00100 01111. X I 164 v2.06 fctiwuz[.] Floating Convert To Integer Word Unsigned truncate
                        111111 ..... ///// ..... 11001 01111. X I 161 PPC fctidz[.] Floating Convert To Integer Dword truncate
                        111111 ..... ///// ..... 11101 01111. X I 162 v2.06 fctiduz[.] Floating Convert To Integer Dword Unsigned truncate
                        111111 ..... ..... ..... ///// 10010. A I 154 P1 fdiv[.] Floating Divide
                        111111 ..... ..... ..... ///// 10100. A I 153 P1 fsub[.] Floating Subtract
                        111111 ..... ..... ..... ///// 10101. A I 153 P1 fadd[.] Floating Add
                        111111 ..... ///// ..... ///// 10110. A I 155 P2 fsqrt[.] Floating Square Root
                        111111 ..... ..... ..... ..... 10111. A I 169 PPC fsel[.] Floating Select
                        111111 ..... ///// ..... ///// 11000. A I 155 v2.02 fre[.] Floating Reciprocal Estimate
                        111111 ..... ..... ///// ..... 11001. A I 154 P1 fmul[.] Floating Multiply
                        111111 ..... ///// ..... ///// 11010. A I 156 PPC frsqrte[.] Floating Reciprocal Square Root Estimate
                        111111 ..... ..... ..... ..... 11100. A I 159 P1 fmsub[.] Floating Multiply-Subtract
                        111111 ..... ..... ..... ..... 11101. A I 158 P1 fmadd[.] Floating Multiply-Add
                        111111 ..... ..... ..... ..... 11110. A I 159 P1 fnmsub[.] Floating Negative Multiply-Subtract
                        111111 ..... ..... ..... ..... 11111. A I 159 P1 fnmadd[.] Floating Negative Multiply-Add
            */
        }
    }
}
