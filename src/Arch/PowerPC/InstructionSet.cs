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

// "Power ISA(tm) Version 3.0 - November 30, 2015 - IBM
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Diagnostics;
using static Reko.Arch.PowerPC.PowerPcDisassembler;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;

    /// <summary>
    /// An instruction set builds the decoders for a particular PowerPC instruction set.
    /// </summary>
    public partial class InstructionSet
    {
        private const InstrClass LinPri = InstrClass.Linear | InstrClass.Privileged;

        protected readonly Decoder invalid;

        public static InstructionSet Create(string? model)
        {
            model ??= "";
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

                Select(0, 32, u => u == 0x6000_0000,
                    Instr(InstrClass.Linear|InstrClass.Padding, Mnemonic.nop),
                    Instr(Mnemonic.ori, r2,r1,U)),
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

                Mask(30, 1, "  Ext3A",
                    new DSDecoder(Mnemonic.ld, Mnemonic.ldu, r1,E2),    // 3A
                    new DSDecoder(Mnemonic.lwa, Mnemonic.lwa, Is64Bit, r1,E2)),
//111010 ..... ..... ..... ..... ....00 DS I 53 PPC ld Load Dword
//111010 ..... ..... ..... ..... ....01 DS I 53 PPC ldu Load Dword with Update
//111010 ..... ..... ..... ..... ....10 DS I 52 PPC lwa Load Word Algebraic
                Ext3BDecoder(),

                Ext3CDecoder(),
                Ext3DDecoder(),
                Mask(30, 1, "  Ext3E",
                    new DSDecoder(Mnemonic.std, Mnemonic.stdu, r1,E2),          // 3E
                    new DSDecoder(Mnemonic.stq, Mnemonic.stq, Is64Bit, rp1,E2)),
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

        protected Decoder InstrPPC(Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, mutators, InstrClass.Linear);
        }

        protected static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic,  mutators, iclass);
        }

        protected static Decoder Mask(int ppcBitPosition, int bits, params Decoder[] decoders)
        {
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(
                new Bitfield(32 - (ppcBitPosition + bits), bits),
                "",
                decoders);
        }

        protected static Decoder Mask(int ppcBitPosition, int bits, string diagnostic, params Decoder[] decoders)
        {
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(
                new Bitfield(32 - (ppcBitPosition + bits), bits),
                diagnostic,
                decoders);
        }

        protected Decoder Sparse(int ppcBitPosition, int bits, params (uint, Decoder)[] sparseDecoders)
        {
            return Sparse(ppcBitPosition, bits, this.invalid, sparseDecoders);
        }

        protected Decoder Sparse(int ppcBitPosition, int bits, Decoder defaultDecoder, params (uint, Decoder)[] sparseDecoders)
        {
            return Sparse(ppcBitPosition, bits, defaultDecoder, "", sparseDecoders);
        }

        protected Decoder Sparse(int ppcBitPosition, int bits, Decoder defaultDecoder, string tag, params (uint, Decoder)[] sparseDecoders)
        {
            var decoders = new Decoder[1 << bits];
            foreach (var (code, decoder) in sparseDecoders)
            {
                Debug.Assert(0 <= code && code < decoders.Length);
                Debug.Assert(decoders[code] is null, $"Duplicate decoder for {tag} at index 0x{code:X}.");
                decoders[code] = decoder;
            }
            for (int i = 0; i < decoders.Length; ++i)
            {
                if (decoders[i] is null)
                    decoders[i] = defaultDecoder;
            }
            var leBitPosition = 32 - (ppcBitPosition + bits);
            return new MaskDecoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>(new Bitfield(leBitPosition, bits), tag, decoders);
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
            // AltiVec instructions.
            var vxDecoders =
                Mask(27, 5, "  AltiVec",
                    Mask(21, 5, "  Subop=000000",

                        Instr(Mnemonic.vaddubm, v1, v2, v3),            // v2.03 Vector Add Unsigned Byte Modulo
                        Instr(Mnemonic.vadduhm, v1, v2, v3),            // v2.03 Vector Add Unsigned Hword Modulo
                        Instr(Mnemonic.vadduwm, v1, v2, v3),            // v2.03 Vector Add Unsigned Word Modulo
                        Instr(Mnemonic.vaddudm, v1, v2, v3),            // v2.07 Vector Add Unsigned Dword Modulo

                        Instr(Mnemonic.vadduqm, v1, v2, v3),            // v2.07 Vector Add Unsigned Qword Modulo
                        Instr(Mnemonic.vaddcuq, v1, v2, v3),            // v2.07 Vector Add & write Carry Unsigned Qword
                        Instr(Mnemonic.vaddcuw, v1,v2,v3),              // v2.03 Vector Add & Write Carry - Out Unsigned Word
                        invalid,

                        Instr(Mnemonic.vaddubs, v1, v2, v3),            // v2.03 Vector Add Unsigned Byte Saturate
                        Instr(Mnemonic.vadduhs, v1, v2, v3),            // v2.03 Vector Add Unsigned Hword Saturate
                        Instr(Mnemonic.vadduws, v1, v2, v3),            // v2.03 Vector Add Unsigned Word Saturate
                        invalid,

                        Instr(Mnemonic.vaddsbs, v1, v2, v3),            // v2.03 Vector Add Signed Byte Saturate
                        Instr(Mnemonic.vaddshs, v1, v2, v3),            // v2.03 Vector Add Signed Hword Saturate
                        Instr(Mnemonic.vaddsws, v1, v2, v3),            // v2.03 Vector Add Signed Word Saturate
                        invalid,

                        Instr(Mnemonic.vsububm, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Byte Modulo
                        Instr(Mnemonic.vsubuhm, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Hword Modulo
                        Instr(Mnemonic.vsubuwm, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Word Modulo
                        Instr(Mnemonic.vsubudm, v1, v2, v3),            // v2.07 Vector Subtract Unsigned Dword Modulo

                        Instr(Mnemonic.vsubuqm, v1, v2, v3),            // v2.07 Vector Subtract Unsigned Qword Modulo
                        Instr(Mnemonic.vsubcuq, v1, v2, v3),            // v2.07 Vector Subtract & write Carry Unsigned Qword
                        Instr(Mnemonic.vsubcuw, v1, v2, v3),            // v2.03 Vector Subtract & Write Carry - Out Unsigned Word
                        invalid,

                        Instr(Mnemonic.vsububs, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Byte Saturate
                        Instr(Mnemonic.vsubuhs, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Hword Saturate
                        Instr(Mnemonic.vsubuws, v1, v2, v3),            // v2.03 Vector Subtract Unsigned Word Saturate
                        invalid,

                        Instr(Mnemonic.vsubsbs, v1, v2, v3),            // v2.03 Vector Subtract Signed Byte Saturate
                        Instr(Mnemonic.vsubshs, v1, v2, v3),            // v2.03 Vector Subtract Signed Hword Saturate
                        Instr(Mnemonic.vsubsws, v1, v2, v3),            // v2.03 Vector Subtract Signed Word Saturate
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

                            Instr(Mnemonic.vmul10uq, v1, v2),           // v3.0 Vector Multiply-by-10 Unsigned Qword
                            Instr(Mnemonic.vmul10euq, v1, v2, v3),      // v3.0  Vector Multiply - by - 10 Extended Unsigned Qword
                            invalid,
                            invalid,

                            invalid,
                            Instr(Mnemonic.bcdcpsgn, CC, v1, v2, v3),   // v3.0 bcdcpsgn.Decimal CopySign &record
                            invalid,
                            invalid),
                        Mask(23, 3,
                            Instr(Mnemonic.bcdadd, CC, v1, v2, v3, u9_1),   // v2.07 bcdadd.Decimal Add Modulo & record
                            Instr(Mnemonic.bcdsub, CC, v1, v2, v3, u9_1),   // v2.07 bcdsub.Decimal Subtract Modulo & record
                            Instr(Mnemonic.bcdus, CC, v1, v2, v3),          // v3.0 bcdus.Decimal Unsigned Shift & record
                            Instr(Mnemonic.bcds, CC, v1, v2, v3),           // v3.0 bcds.Decimal Shift &record

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
                        Instr(Mnemonic.vmaxub, v1, v2, v3),             // v2.03 Vector Maximum Unsigned Byte
                        Instr(Mnemonic.vmaxuh, v1, v2, v3),             // v2.03 Vector Maximum Unsigned Hword
                        Instr(Mnemonic.vmaxuw, v1, v2, v3),             // v2.03 Vector Maximum Unsigned Word
                        Instr(Mnemonic.vmaxud, v1, v2, v3),             // v2.07 Vector Maximum Unsigned Dword
                        Instr(Mnemonic.vmaxsb, v1, v2, v3),             // v2.03 Vector Maximum Signed Byte
                        Instr(Mnemonic.vmaxsh, v1, v2, v3),             // v2.03 Vector Maximum Signed Hword
                        Instr(Mnemonic.vmaxsw, v1, v2, v3),             // v2.03 Vector Maximum Signed Word
                        Instr(Mnemonic.vmaxsd, v1, v2, v3),             // v2.07 Vector Maximum Signed Dword
                        Instr(Mnemonic.vminub, v1, v2, v3),             // v2.03 Vector Minimum Unsigned Byte
                        Instr(Mnemonic.vminuh, v1, v2, v3),             // v2.03 Vector Minimum Unsigned Hword
                        Instr(Mnemonic.vminuw, v1, v2, v3),             // v2.03 Vector Minimum Unsigned Word
                        Instr(Mnemonic.vminud, v1, v2, v3),             // v2.07 Vector Minimum Unsigned Dword
                        Instr(Mnemonic.vminsb, v1, v2, v3),             // v2.03 Vector Minimum Signed Byte
                        Instr(Mnemonic.vminsh, v1, v2, v3),             // v2.03 Vector Minimum Signed Hword
                        Instr(Mnemonic.vminsw, v1, v2, v3),             // v2.03 Vector Minimum Signed Word
                        Instr(Mnemonic.vminsd, v1, v2, v3),             // v2.07 Vector Minimum Signed Dword

                        Instr(Mnemonic.vavgub, v1, v2, v3),             // v2.03 Vector Average Unsigned Byte
                        Instr(Mnemonic.vavguh, v1, v2, v3),             // v2.03 Vector Average Unsigned Hword
                        Instr(Mnemonic.vavguw, v1, v2, v3),             // v2.03 Vector Average Unsigned Word
                        invalid,
                        Instr(Mnemonic.vavgsb, v1, v2, v3),             // v2.03 Vector Average Signed Byte
                        Instr(Mnemonic.vavgsh, v1, v2, v3),             // v2.03 Vector Average Signed Hword
                        Instr(Mnemonic.vavgsw, v1, v2, v3),             // v2.03 Vector Average Signed Word
                        invalid,

                        Sparse(11, 5, 
                            (0b00000, Instr(Mnemonic.vclzlsbb, v1, v3)),    // v3.0 vclzlsbb Vector Count Leading Zero Least-Significant Bits Byte
                            (0b00001, Instr(Mnemonic.vctzlsbb, v1, v3)),    // v3.0 vctzlsbb Vector Count Trailing Zero Least-Significant Bits Byte
                            (0b00110, Instr(Mnemonic.vnegw, v1, v3)),       // v3.0 vnegw Vector Negate Word
                            (0b00111, Instr(Mnemonic.vnegd, v1, v3)),       // v3.0 vnegd Vector Negate Dword
                            (0b01000, Instr(Mnemonic.vprtybw, v1, v3)),     // v3.0 vprtybw Vector Parity Byte Word
                            (0b01001, Instr(Mnemonic.vprtybd, v1, v3)),     // v3.0 vprtybd Vector Parity Byte Dword
                            (0b01010, Instr(Mnemonic.vprtybq, v1, v3)),     // v3.0 vprtybq Vector Parity Byte Qword
                            (0b10000, Instr(Mnemonic.vextsb2w, v1, v3)),    // v3.0 vextsb2w Vector Extend Sign Byte to Word
                            (0b10001, Instr(Mnemonic.vextsh2w, v1, v3)),    // v3.0 vextsh2w Vector Extend Sign Hword to Word
                            (0b11000, Instr(Mnemonic.vextsb2d, v1, v3)),    // v3.0 vextsb2d Vector Extend Sign Byte to Dword
                            (0b11001, Instr(Mnemonic.vextsh2d, v1, v3)),    // v3.0 vextsh2d Vector Extend Sign Hword to Dword
                            (0b11010, Instr(Mnemonic.vextsw2d, v1, v3)),    // v3.0 vextsw2d Vector Extend Sign Word to Dword
                            (0b11100, Instr(Mnemonic.vctzb, v1, v3)),   // v3.0 vctzb Vector Count Trailing Zeros Byte
                            (0b11101, Instr(Mnemonic.vctzh, v1, v3)),   // v3.0 vctzh Vector Count Trailing Zeros Hword
                            (0b11110, Instr(Mnemonic.vctzw, v1, v3)),   // v3.0 vctzw Vector Count Trailing Zeros Word
                            (0b11111, Instr(Mnemonic.vctzd, v1, v3))),  // v3.0 vctzd Vector Count Trailing Zeros Dword
                        invalid,
                        Nyi(Mnemonic.vshasigmaw),                       // v2.07Vector SHA - 256 Sigma Word
                        Nyi(Mnemonic.vshasigmad),                       // v2.07Vector SHA - 512 Sigma Dword

                        Instr(Mnemonic.vclzb, v1, v3),                  // v2.07Vector Count Leading Zeros Byte
                        Instr(Mnemonic.vclzh, v1, v3),                  // v2.07Vector Count Leading Zeros Hword
                        Instr(Mnemonic.vclzw, v1, v3),                  // v2.07Vector Count Leading Zeros Word
                        Instr(Mnemonic.vclzd, v1, v3)),                 // v2.07Vector Count Leading Zeros Dword
                    Sparse(21, 5, // "Opc=04, Subop=000011",
                        (0b10000, Instr(Mnemonic.vabsdub, v1, v2, v3)), // v3.0 vabsdub Vector Absolute Difference Unsigned Byte
                        (0b10001, Instr(Mnemonic.vabsduh, v1, v2, v3)), // v3.0 vabsduh Vector Absolute Difference Unsigned Hword
                        (0b10010, Instr(Mnemonic.vabsduw, v1, v2, v3)), // v3.0 vabsduw Vector Absolute Difference Unsigned Word
                        (0b11100, Instr(Mnemonic.vpopcntb, v1, v3)),    //  v2.07 vpopcntb Vector Population Count Byte
                        (0b11101, Instr(Mnemonic.vpopcnth, v1, v3)),    //  v2.07 vpopcnth Vector Population Count Hword
                        (0b11110, Instr(Mnemonic.vpopcntw, v1, v3)),    //  v2.07 vpopcntw Vector Population Count Word
                        (0b11111, Instr(Mnemonic.vpopcntd, v1, v3))),   //  v2.07 vpopcntd Vector Population Count Dword
                    Mask(21, 5, "Opc=04, Subop=000100",
                        Instr(Mnemonic.vrlb, v1, v2, v3),               // v2.03 Vector Rotate Left Byte
                        Instr(Mnemonic.vrlh, v1, v2, v3),               // v2.03 Vector Rotate Left Hword
                        Instr(Mnemonic.vrlw, v1, v2, v3),               // v2.03 Vector Rotate Left Word
                        Instr(Mnemonic.vrld, v1, v2, v3),               // v2.07 Vector Rotate Left Dword

                        Instr(Mnemonic.vslb, v1, v2, v3),               // v2.03 Vector Shift Left Byte
                        Instr(Mnemonic.vslh, v1, v2, v3),               // v2.03 Vector Shift Left Hword
                        Instr(Mnemonic.vslw, v1, v2, v3),               // v2.03 Vector Shift Left Word
                        Instr(Mnemonic.vsl, v1, v2, v3),                // v2.03 Vector Shift Left

                        Instr(Mnemonic.vsrb, v1, v2, v3),               // v2.03 Vector Shift Right Byte
                        Instr(Mnemonic.vsrh, v1, v2, v3),               // v2.03 Vector Shift Right Hword
                        Instr(Mnemonic.vsrw, v1, v2, v3),               // v2.03 Vector Shift Right Word
                        Instr(Mnemonic.vsr, v1, v2, v3),                // v2.03 Vector Shift Right

                        Instr(Mnemonic.vsrab, v1, v2, v3),              // v2.03 Vector Shift Right Algebraic Byte
                        Instr(Mnemonic.vsrah, v1, v2, v3),              // v2.03 Vector Shift Right Algebraic Hword
                        Instr(Mnemonic.vsraw, v1, v2, v3),              // v2.03 Vector Shift Right Algebraic Word
                        Instr(Mnemonic.vsrad, v1, v2, v3),              // v2.07 Vector Shift Right Algebraic Dword

                        Instr(Mnemonic.vand, v1, v2, v3),               // v2.03 Vector Logical AND
                        Instr(Mnemonic.vandc, v1, v2, v3),              // v2.03 Vector Logical AND with Complement
                        Instr(Mnemonic.vor, v1, v2, v3),                // v2.03 Vector Logical OR
                        Instr(Mnemonic.vxor, v1, v2, v3),               // v2.03 Vector Logical XOR

                        Instr(Mnemonic.vnor, v1,v2,v3),                 // v2.03 Vector Logical NOR
                        Instr(Mnemonic.vorc, v1, v2, v3),               // v2.07 Vector Logical OR with Complement
                        Instr(Mnemonic.vnand, v1, v2, v3),              // v2.07 Vector Logical NAND
                        Instr(Mnemonic.vsld, v1, v2, v3),               // v2.07 Vector Shift Left Dword

                        Instr(Mnemonic.mfvscr, v1),                     // v2.03 Move From VSCR
                        Instr(Mnemonic.mtvscr, v3),                     // v2.03 Move To VSCR
                        Instr(Mnemonic.veqv, v1, v2, v3),               // v2.07 Vector Logical Equivalence
                        Instr(Mnemonic.vsrd, v1, v2, v3),               // v2.07 Vector Shift Right Dword

                        Instr(Mnemonic.vsrv, v1, v2, v3),               // v3.0 Vector Shift Right Variable
                        Instr(Mnemonic.vslv, v1, v2, v3),               // v3.0 Vector Shift Left Variable
                        invalid,
                        invalid),
                    Sparse(21, 5,   // "Opc=04, Subop=000101",
                        (0b00010, Instr(Mnemonic.vrlwmi, v1, v2, v3)),  // v3.0 vrlwmi Vector Rotate Left Word then Mask Insert
                        (0b00011, Instr(Mnemonic.vrldmi, v1, v2, v3)),  // v3.0 vrldmi Vector Rotate Left Dword then Mask Insert
                        (0b00110, Instr(Mnemonic.vrlwnm, v1, v2, v3)),  // v3.0 vrlwnm Vector Rotate Left Word then AND with Mask
                        (0b00111, Instr(Mnemonic.vrldnm, v1, v2, v3))), // v3.0 vrldnm Vector Rotate Left Dword then AND with Mask
                    Mask(22, 4, "Opc=04, Subop=000110",
                        Instr(Mnemonic.vcmpequb, C10, v1, v2, v3),      // v2.03 vcmpequb[.] Vector Compare Equal Unsigned Byte
                        Instr(Mnemonic.vcmpequh, C10, v1, v2, v3),      // v2.03 vcmpequh[.] Vector Compare Equal Unsigned Hword
                        Instr(Mnemonic.vcmpequw, C10, v1, v2, v3),      // v2.03 vcmpequw[.] Vector Compare Equal Unsigned Word
                        Instr(Mnemonic.vcmpeqfp, C10, v1, v2, v3),      // v2.03 vcmpeqfp[.] Vector Compare Equal To Floating - Point

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgefp, C10, v1, v2, v3),      // v2.03 vcmpgefp[.] Vector Compare Greater Than or Equal To Floating-Point
                        Instr(Mnemonic.vcmpgtub, C10, v1, v2, v3),      // v2.03 vcmpgtub[.] Vector Compare Greater Than Unsigned Byte
                        Instr(Mnemonic.vcmpgtuh, C10, v1, v2, v3),      // v2.03 vcmpgtuh[.] Vector Compare Greater Than Unsigned Hword
                        Instr(Mnemonic.vcmpgtuw, C10, v1, v2, v3),      // v2.03 vcmpgtuw[.] Vector Compare Greater Than Unsigned Word
                        Instr(Mnemonic.vcmpgtfp, C10, v1, v2, v3),      // v2.03 vcmpgtfp[.] Vector Compare Greater Than Floating - Point
                        Instr(Mnemonic.vcmpgtsb, C10, v1, v2, v3),      // v2.03 vcmpgtsb[.] Vector Compare Greater Than Signed Byte
                        Instr(Mnemonic.vcmpgtsh, C10, v1, v2, v3),      // v2.03 vcmpgtsh[.] Vector Compare Greater Than Signed Hword
                        Instr(Mnemonic.vcmpgtsw, C10, v1, v2, v3),      // v2.03 vcmpgtsw[.] Vector Compare Greater Than Signed Word
                        Instr(Mnemonic.vcmpbfp, C10, v1, v2, v3)),      // v2.03 vcmpbfp[.] Vector Compare Bounds Floating-Point
                    Mask(22, 4, "Opc=04, Subop=000111",
                        Instr(Mnemonic.vcmpneb, C10, v1, v2, v3),       // v3.0 Vector Compare Not Equal Byte
                        Instr(Mnemonic.vcmpneh, C10, v1, v2, v3),       // v3.0 Vector Compare Not Equal Hword
                        Instr(Mnemonic.vcmpnew, C10, v1, v2, v3),       // v3.0 Vector Compare Not Equal Word
                        Instr(Mnemonic.vcmpequd, C10, v1, v2, v3),      // v2.07 Vector Compare Equal Unsigned Dword

                        Instr(Mnemonic.vcmpnezb, C10, v1, v2, v3),      // v3.0 Vector Compare Not Equal or Zero Byte
                        Instr(Mnemonic.vcmpnezh, C10, v1, v2, v3),      // v3.0 Vector Compare Not Equal or Zero Hword
                        Instr(Mnemonic.vcmpnezw, C10, v1, v2, v3),      // v3.0 Vector Compare Not Equal or Zero Word
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgtud, C10, v1, v2, v3),      // v2.07 Vector Compare Greater Than Unsigned Dword

                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcmpgtsd, C10, v1, v2, v3)),     // v2.07 Vector Compare Greater Than Signed Dword

                    Mask(21, 5, "Opc=04, Subop=001000",
                        Instr(Mnemonic.vmuloub, v1, v2, v3),            // v2.03 Vector Multiply Odd Unsigned Byte
                        Instr(Mnemonic.vmulouh, v1, v2, v3),            // v2.03 Vector Multiply Odd Unsigned Hword
                        Instr(Mnemonic.vmulouw, v1, v2, v3),            // v2.07 Vector Multiply Odd Unsigned Word
                        invalid,
                        Instr(Mnemonic.vmulosb, v1, v2, v3),            // v2.03 Vector Multiply Odd Signed Byte
                        Instr(Mnemonic.vmulosh, v1, v2, v3),            // v2.03 Vector Multiply Odd Signed Hword
                        Instr(Mnemonic.vmulosw, v1, v2, v3),            // v2.07 Vector Multiply Odd Signed Word
                        invalid,
                        Instr(Mnemonic.vmuleub, v1, v2, v3),            // v2.03 Vector Multiply Even Unsigned Byte
                        Instr(Mnemonic.vmuleuh, v1, v2, v3),            // v2.03 Vector Multiply Even Unsigned Hword
                        Instr(Mnemonic.vmuleuw, v1, v2, v3),            // v2.07 Vector Multiply Even Unsigned Word
                        invalid,
                        Instr(Mnemonic.vmulesb, v1, v2, v3),            // v2.03 Vector Multiply Even Signed Byte
                        Instr(Mnemonic.vmulesh, v1, v2, v3),            // v2.03 Vector Multiply Even Signed Hword
                        Instr(Mnemonic.vmulesw, v1, v2, v3),            // v2.07 Vector Multiply Even Signed Word
                        invalid,
                        Instr(Mnemonic.vpmsumb, v1, v2, v3),            // v2.07 Vector Polynomial Multiply-Sum Byte
                        Instr(Mnemonic.vpmsumh, v1, v2, v3),            // v2.07 Vector Polynomial Multiply-Sum Hword
                        Instr(Mnemonic.vpmsumw, v1, v2, v3),            // v2.07 Vector Polynomial Multiply-Sum Word
                        Instr(Mnemonic.vpmsumd, v1, v2, v3),            // v2.07 Vector Polynomial Multiply-Sum Dword
                        Instr(Mnemonic.vcipher, v1, v2, v3),            // v2.07 Vector AES Cipher
                        Instr(Mnemonic.vncipher, v1, v2, v3),           // v2.07 Vector AES Inverse Cipher
                        invalid,
                        Instr(Mnemonic.vsbox, v1, v2),                  // v2.07 Vector AES SubBytes
                        Instr(Mnemonic.vsum4ubs, v1, v2, v3),           // v2.03 Vector Sum across Quarter Unsigned Byte Saturate
                        Instr(Mnemonic.vsum4shs, v1, v2, v3),           // v2.03 Vector Sum across Quarter Signed Hword Saturate
                        Instr(Mnemonic.vsum2sws, v1, v2, v3),           // v2.03 Vector Sum across Half Signed Word Saturate
                        invalid,
                        Instr(Mnemonic.vsum4sbs, v1, v2, v3),           // v2.03 Vector Sum across Quarter Signed Byte Saturate
                        invalid,
                        Instr(Mnemonic.vsumsws, v1, v2, v3),            // v2.03 Vector Sum across Signed Word Saturate
                        invalid),
                    Sparse(21, 5, // "Opc=04, Subop=001001",

                        (0b00010, Instr(Mnemonic.vmuluwm, v1, v2, v3)),         // v2.07 vmuluwm Vector Multiply Unsigned Word Modulo
                        (0b10100, Instr(Mnemonic.vcipherlast, v1, v2, v3)),     // v2.07 vcipherlast Vector AES Cipher Last
                        (0b10101, Instr(Mnemonic.vncipherlast, v1, v2, v3))),   // v2.07 vncipherlast Vector AES Inverse Cipher Last

                    Sparse(21, 5, // "Opc=04, Subop=001010",
                        (0b00000, Instr(Mnemonic.vaddfp, v1, v2, v3)),      // v2.03 vaddfp Vector Add Floating-Point
                        (0b00001, Instr(Mnemonic.vsubfp, v1, v2, v3)),      // v2.03 vsubfp Vector Subtract Floating-Point
                        (0b00100, Instr(Mnemonic.vrefp, v1, v3)),           // v2.03 vrefp Vector Reciprocal Estimate Floating-Point
                        (0b00101, Instr(Mnemonic.vrsqrtefp, v1, v3)),       // v2.03 vrsqrtefp Vector Reciprocal Square Root Estimate Floating-Point
                        (0b00110, Instr(Mnemonic.vexptefp, v1, v3)),        // v2.03 vexptefp Vector 2 Raised to the Exponent Estimate Floating-Point
                        (0b00111, Instr(Mnemonic.vlogefp, v1, v3)),         // v2.03 vlogefp Vector Log Base 2 Estimate Floating-Point
                        (0b01000, Instr(Mnemonic.vrfin, v1, v3)),           // v2.03 vrfin Vector Round to Floating-Point Integral Nearest
                        (0b01001, Instr(Mnemonic.vrfiz, v1, v3)),           // v2.03 vrfiz Vector Round to Floating-Point Integral toward Zero
                        (0b01010, Instr(Mnemonic.vrfip, v1, v3)),           // v2.03 vrfip Vector Round to Floating-Point Integral toward +Infinity
                        (0b01011, Instr(Mnemonic.vrfim, v1, v3)),           // v2.03 vrfim Vector Round to Floating-Point Integral toward -Infinity
                        (0b01100, Instr(Mnemonic.vcfux, v1, v3, u16_5)),    // v2.03 vcfux Vector Convert From Unsigned Word
                        (0b01101, Instr(Mnemonic.vcfsx, v1, v3, u16_5)),    // v2.03 vcfsx Vector Convert From Signed Word
                        (0b01110, Instr(Mnemonic.vctuxs, v1, v3, u16_5)),   // v2.03 vctuxs Vector Convert To Unsigned Word Saturate
                        (0b01111, Instr(Mnemonic.vctsxs, v1, v3, u16_5)),   // v2.03 vctsxs Vector Convert To Signed Word Saturate
                        (0b10000, Instr(Mnemonic.vmaxfp, v1, v2, v3)),      // v2.03 vmaxfp Vector Maximum Floating-Point
                        (0b10001, Instr(Mnemonic.vminfp, v1, v2, v3))),     // v2.03 vminfp Vector Minimum Floating-Point

                    invalid,  // "Opc=04, Subop=001011"
                    Sparse(21, 5, // "Opc=04, Subop=001100"
                        (0b00000, Instr(Mnemonic.vmrghb, v1, v2, v3)),      // v2.03 vmrghb Vector Merge High Byte
                        (0b00001, Instr(Mnemonic.vmrghh, v1, v2, v3)),      // v2.03 vmrghh Vector Merge High Hword
                        (0b00010, Instr(Mnemonic.vmrghw, v1, v2, v3)),      // v2.03 vmrghw Vector Merge High Word
                        (0b00100, Instr(Mnemonic.vmrglb, v1, v2, v3)),      // v2.03 vmrglb Vector Merge Low Byte
                        (0b00101, Instr(Mnemonic.vmrglh, v1, v2, v3)),      // v2.03 vmrglh Vector Merge Low Hword
                        (0b00110, Instr(Mnemonic.vmrglw, v1, v2, v3)),      // v2.03 vmrglw Vector Merge Low Word
                        (0b01000, Instr(Mnemonic.vspltb, v1, v3, u16_4)),   // v2.03 vspltb Vector Splat Byte
                        (0b01001, Instr(Mnemonic.vsplth, v1, v3, u16_3)),   // v2.03 vsplth Vector Splat Hword
                        (0b01010, Instr(Mnemonic.vspltw, v1, v3, u16_2)),   // v2.03 vspltw Vector Splat Word
                        (0b01100, Instr(Mnemonic.vspltisb, v1, s16_5)), // v2.03 vspltisb Vector Splat Immediate Signed Byte
                        (0b01101, Instr(Mnemonic.vspltish, v1, s16_5)), // v2.03 vspltish Vector Splat Immediate Signed Hword
                        (0b01110, Instr(Mnemonic.vspltisw, v1, s16_5)), // v2.03 vspltisw Vector Splat Immediate Signed Word
                        (0b10000, Instr(Mnemonic.vslo, v1, v2, v3)),    // v2.03 vslo Vector Shift Left by Octet
                        (0b10001, Instr(Mnemonic.vsro, v1, v2, v3)),    // v2.03 vsro Vector Shift Right by Octet
                        (0b10100, Instr(Mnemonic.vgbbd, v1, v3)),       // v2.07 vgbbd Vector Gather Bits by Byte by Dword
                        (0b10101, Instr(Mnemonic.vbpermq, v1, v2, v3)), // v2.07 vbpermq Vector Bit Permute Qword
                        (0b10111, Instr(Mnemonic.vbpermd, v1, v2, v3)), // v3.0  vbpermd Vector Bit Permute Dword
                        (0b11010, Instr(Mnemonic.vmrgow, v1, v2, v3)),  // v2.07 vmrgow Vector Merge Odd Word
                        (0b11110, Instr(Mnemonic.vmrgew, v1, v2, v3))), // v2.07 vmrgew Vector Merge Even Word
                    Sparse(21, 5, // "Opc=04, Subop=001101"
                        (0b01000, Instr(Mnemonic.vextractub, v1, v3, u16_4)),   // v3.0 vextractub Vector Extract Unsigned Byte
                        (0b01001, Instr(Mnemonic.vextractuh, v1, v3, u16_4)),   // v3.0 vextractuh Vector Extract Unsigned Hword
                        (0b01010, Instr(Mnemonic.vextractuw, v1, v3, u16_4)),   // v3.0 vextractuw Vector Extract Unsigned Word
                        (0b01011, Instr(Mnemonic.vextractd, v1, v3, u16_4)),    // v3.0 vextractd Vector Extract Dword
                        (0b01100, Instr(Mnemonic.vinsertb, v1, v3, u16_4)), // v3.0 vinsertb Vector Insert Byte
                        (0b01101, Instr(Mnemonic.vinserth, v1, v3, u16_4)), // v3.0 vinserth Vector Insert Hword
                        (0b01110, Instr(Mnemonic.vinsertw, v1, v3, u16_4)), // v3.0 vinsertw Vector Insert Word
                        (0b01111, Instr(Mnemonic.vinsertd, v1, v3, u16_4)), // v3.0 vinsertd Vector Insert Dword
                        (0b11000, Instr(Mnemonic.vextublx, r1, r2, v3)),    // v3.0 vextublx Vector Extract Unsigned Byte Left-Indexed
                        (0b11001, Instr(Mnemonic.vextuhlx, r1, r2, v3)),    // v3.0 vextuhlx Vector Extract Unsigned Hword Left-Indexed
                        (0b11010, Instr(Mnemonic.vextuwlx, r1, r2, v3)),    // v3.0 vextuwlx Vector Extract Unsigned Word Left-Indexed
                        (0b11100, Instr(Mnemonic.vextubrx, r1, r2, v3)),    // v3.0 vextubrx Vector Extract Unsigned Byte Right-Indexed
                        (0b11101, Instr(Mnemonic.vextuhrx, r1, r2, v3)),    // v3.0 vextuhrx Vector Extract Unsigned Hword Right-Indexed
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
                        (0b01000, Instr(Mnemonic.vupkhsb, v1, v3)),       // v2.03 vupkhsb Vector Unpack High Signed Byte
                        (0b01001, Instr(Mnemonic.vupkhsh, v1, v3)),       // v2.03 vupkhsh Vector Unpack High Signed Hword
                        (0b01010, Instr(Mnemonic.vupklsb, v1, v3)),       // v2.03 vupklsb Vector Unpack Low Signed Byte
                        (0b01011, Instr(Mnemonic.vupklsh, v1, v3)),       // v2.03 vupklsh Vector Unpack Low Signed Hword
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
                Instr(Mnemonic.vmhaddshs, v1, v2, v3, v4),              // v2.03  Vector Multiply - High - Add Signed Hword Saturate
                Instr(Mnemonic.vmhraddshs, v1, v2, v3, v4),             // v2.03  Vector Multiply - High - Round - Add Signed Hword Saturate
                Instr(Mnemonic.vmladduhm, v1, v2, v3, v4),              // v2.03  Vector Multiply - Low - Add Unsigned Hword Modulo
                invalid,

                Instr(Mnemonic.vmsumubm, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Unsigned Byte Modulo
                Instr(Mnemonic.vmsummbm, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Mixed Byte Modulo
                Instr(Mnemonic.vmsumuhm, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Unsigned Hword Modulo
                Instr(Mnemonic.vmsumuhs, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Unsigned Hword Saturate
                
                Instr(Mnemonic.vmsumshm, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Signed Hword Modulo
                Instr(Mnemonic.vmsumshs, v1, v2, v3, v4),               // v2.03  Vector Multiply - Sum Signed Hword Saturate
                Instr(Mnemonic.vsel, v1,v2,v3,v4),                      // v2.03  Vector Select
                Instr(Mnemonic.vperm, v1,v2,v3,v4),                     // v2.03  Vector Permute
                
                Instr(Mnemonic.vsldoi, v1,v2,v3, u6_4),                 // v2.03  Vector Shift Left Double by Octet Immediate
                Instr(Mnemonic.vpermxor, v1, v2, v3, v4),               // v2.07  Vector Permute & Exclusive - OR
                Instr(Mnemonic.vmaddfp, v1, v2, v4, v3),                // v2.03  Vector Multiply - Add Floating - Point
                Instr(Mnemonic.vnmsubfp, v1, v2, v4, v3),               // v2.03  Vector Negative Multiply - Subtract Floating - Point
                        // 0x10 - 16
                Instr(Mnemonic.maddhd, r1,r2,r3,r4),                    // v3.0  Multiply - Add High Dword
                Instr(Mnemonic.maddhdu, r1,r2,r3,r4),                   // v3.0  Multiply - Add High Dword Unsigned
                invalid,
                Instr(Mnemonic.maddld, r1,r2,r3,r4),                    // v3.0  Multiply - Add Low Dword

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.vpermr, v1,v2,v3,v4),                    // v3.0  Vector Permute Right - indexed
                
                Instr(Mnemonic.vaddeuqm, v1, v2, v3, v4),               // v2.07  Vector Add Extended Unsigned Qword Modulo
                Instr(Mnemonic.vaddecuq, v1, v2, v3, v4),               // v2.07  Vector Add Extended & write Carry Unsigned Qword
                Instr(Mnemonic.vsubeuqm, v1, v2, v3, v4),               // v2.07  Vector Subtract Extended Unsigned Qword Modulo
                Instr(Mnemonic.vsubecuq, v1, v2, v3, v4));              // v2.07  Vector Subtract Extended & write Carry Unsigned Qword
            return Mask(26, 1,
                vxDecoders,
                vaDecoders);
        }

        
        public virtual Decoder Ext5Decoder()
        {
            return invalid;
        }

        public virtual Decoder Ext6Decoder()
        {
            return invalid;
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
                    (0b00000, new BclrDecoder()),                       // P1
                    (0b10000, Mask(31, 1,                               // P1
                        Instr(InstrClass.ConditionalTransfer, Mnemonic.bcctr, I1, I2),
                        Instr(InstrClass.ConditionalTransfer | InstrClass.Call, Mnemonic.bcctrl, I1, I2))),
                    (0b10001, Mask(31, 1, 
                        Instr(InstrClass.Conditional, Mnemonic.bctar, I1, I2),          // v2.07 bctar[l] Branch Conditional to BTAR [& Link]
                        Instr(InstrClass.Conditional, Mnemonic.bctarl, I1, I2))))),     // v2.07 bctar[l] Branch Conditional to BTAR [& Link]
                (0b10010, Sparse(21, 5,
                    (0b00000, Instr(InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged, Mnemonic.rfid)),   //  PPC  P  rfid Return from Interrupt Dword
                    (0b00010, Nyi(Mnemonic.rfscv)),                     // v3.0 P  rfscv Return From System Call Vectored
                    (0b00100, Instr(InstrClass.Transfer | InstrClass.Return, Mnemonic.rfebb, u11_1)),                   // v2.07   rfebb Return from Event Based Branch
                    (0b01000, Instr(InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged, Mnemonic.hrfid)),  // v2.02 H hrfid Return From Interrupt Dword Hypervisor
                    (0b01011, Instr(InstrClass.Terminates | InstrClass.Privileged, Mnemonic.stop)))),                   // v3.0 P  stop Stop
                (0b10110, Select(21, 5, u => u == 0b00100,
                    Instr(Mnemonic.isync),                              // P1 isync Instruction Synchronize
                    invalid)));
        }

        private Decoder Ext1FDecoder()
        {
            var decoder = Mask(26, 5, "  Ext31",
                Sparse(21, 5, invalid, "  0b00000",
                    (0b00000, new CmpDecoder(Mnemonic.cmp, C1, r2, r3)),    // P1 cmp Compare
                    (0b00001, new CmpDecoder(Mnemonic.cmpl, C1, r2, r3)),   // P1 cmpl Compare Logical
                    (0b00100, Instr(Mnemonic.setb, r1,C2)),                 // v3.0 setb Set Boolean
                    (0b00110, Nyi("cmprb")),                            // v3.0 cmprb Compare Ranged Byte
                    (0b00111, Nyi("cmpeqb")),                           // v3.0 cmpeqb Compare Equal Byte
                    (0b10010, Instr(Mnemonic.mcrxrx, u6_3))),               // v3.0 mcrxrx Move XER to CR Extended
                invalid,
                invalid,
                invalid,

                Sparse(21, 5, invalid, "  0b00100",
                    (0b00000, InstrPPC(Mnemonic.tw, I1, r2, r3)),       // PPC td Trap Word
                    (0b00010, Nyi("td"))),                              // PPC td Trap Dword
                invalid,
                Sparse(21, 5, invalid, "  0b00110",
                    (0b00000, Instr(Mnemonic.lvsl, v1, r2, r3)),        // v2.03 lvsl Load Vector for Shift Left
                    (0b00001, Instr(Mnemonic.lvsr, v1, r2, r3)),        // v2.03 lvsr Load Vector for Shift Right
                    (0b10010, Nyi("lwat")),                             // v3.0 lwat Load Word ATomic
                    (0b10011, Instr(Mnemonic.ldat,  r1,r2,fc_atomic_load)),     // v3.0 ldat Load Dword ATomic
                    (0b10110, Instr(Mnemonic.stwat, r1,r2,fc_atomic_store)),    // v3.0 stwat Store Word ATomic
                    (0b10111, Instr(Mnemonic.stdat, r1, r2, fc_atomic_store)),  // v3.0 stdat Store Dword ATomic
                    (0b11000, Instr(Mnemonic.copy, r2,r3,u21_1)),               // v3.0 copy Copy
                    (0b11010, Nyi("cp_abort")),                         // v3.0 cp_abort CP_Abort
                    (0b11100, Nyi("paste[.]"))),                        // v3.0 paste[.] Paste
                Sparse(21, 5, invalid, "  0b00111",
                    (0b00000, Instr(Mnemonic.lvewx, v1, r2, r3)),       // v2.03 lvebx Load Vector Element Byte Indexed
                    (0b00001, Instr(Mnemonic.lvehx, v1, r2, r3)),       // v2.03 lvehx Load Vector Element Hword Indexed
                    (0b00010, Instr(Mnemonic.lvewx, v1, r2, r3)),       // v2.03 lvewx Load Vector Element Word Indexed
                    (0b00011, Instr(Mnemonic.lvx, v1, r2, r3)),         // v2.03 lvx Load Vector Indexed
                    (0b00100, Instr(Mnemonic.stvebx, v1, r2, r3)),      // v2.03 stvebx Store Vector Element Byte Indexed
                    (0b00101, Instr(Mnemonic.stvehx, v1, r2, r3)),      // v2.03 stvehx Store Vector Element Hword Indexed
                    (0b00110, Instr(Mnemonic.stvewx, v1, r2, r3)),      // v2.03 stvewx Store Vector Element Word Indexed
                    (0b00111, Instr(Mnemonic.stvx, v1, r2, r3)),        // v2.03 stvx Store Vector Indexed
                    (0b01011, Instr(Mnemonic.lvxl, v1, r2, r3)),        // v2.03 lvxl Load Vector Indexed Last
                    (0b01111, Instr(Mnemonic.stvxl, r1, r2, r3)),       // v2.03 stvxl Store Vector Indexed Last
                    (0b10000, Instr(Mnemonic.lvlx, v1, r2, r3))),       // Cell lvx Load Vector Left Indexed


                Sparse(21, 5, invalid, "  0b01000",
                    (0b00000, Instr(Mnemonic.subfc, C, r1, r2, r3)),    // P1 SR subfc[o][.] Subtract From Carrying
                    (0b00001, InstrPPC(Mnemonic.subf, C, r1, r2, r3)),  // PPC SR subf[o][.] Subtract From
                    (0b00011, Instr(Mnemonic.neg, r1, r2)),             // P1 SR neg[o][.] Negate
                    (0b00100, Instr(Mnemonic.subfe, r1, r2, r3)),       // P1 SR subfe[o][.] Subtract From Extended
                    (0b00110, Instr(Mnemonic.subfze, C, r1, r2)),       // P1 SR subfze[o][.] Subtract From Zero Extended
                    (0b00111, Instr(Mnemonic.subfme, C, r1, r2)),       // P1 SR subfme[o][.] Subtract From Minus One Extended

                    (0b10000, Instr(Mnemonic.subfco, C, r1, r2, r3)),   // P1 SR subfc[o][.] Subtract From Carrying
                    (0b10001, InstrPPC(Mnemonic.subfo, C, r1, r2, r3)), // PPC SR subf[o][.] Subtract From
                    (0b10011, Instr(Mnemonic.nego, r1, r2)),            // P1 SR neg[o][.] Negate
                    (0b10100, Instr(Mnemonic.subfeo, r1, r2, r3)),      // P1 SR subfe[o][.] Subtract From Extended
                    (0b10110, Instr(Mnemonic.subfzeo, C, r1, r2)),      // P1 SR subfze[o][.] Subtract From Zero Extended
                    (0b10111, Instr(Mnemonic.subfmeo, C, r1, r2))),     // P1 SR subfme[o][.] Subtract From Minus One Extended

                Sparse(22, 4, invalid, "  0b01001",
                    (0b0000, Instr(Mnemonic.mulhdu, C, r1, r2, r3)),    // PPC SR mulhdu[.] Multiply High Dword Unsigned
                    (0b0010, Instr(Mnemonic.mulhd, C, r1, r2, r3)),     // PPC SR mulhd[.] Multiply High Dword
                    (0b0111, InstrPPC(Mnemonic.mulld, C, r1, r2, r3)),  // PPC SR mulld[o][.] Multiply Low Dword
                    (0b1100, Instr(Mnemonic.divdeu, C, r1, r2, r3)),    // v2.06 SR divdeu[o][.] Divide Dword Extended Unsigned
                    (0b1101, Instr(Mnemonic.divde, C, r1, r2, r3)),     // v2.06 SR divde[o][.] Divide Dword Extended
                    (0b1110, Instr(Mnemonic.divdu, C, r1, r2, r3)),     // PPC SR divdu[o][.] Divide Dword Unsigned
                    (0b1111, Instr(Mnemonic.divd, C, r1, r2, r3)),      // PPC SR divd[o][.] Divide Dword
                    (0b1000, Mask(21, 1, "  modud/sd",
                        Instr(Mnemonic.modud, r1, r2, r3),              // v3.0 modud Modulo Unsigned Dword
                        Instr(Mnemonic.modsd, r1, r2, r3)))),           // v3.0 modsd Modulo Signed Dword
                Sparse(21, 5, invalid, "  0b01010",
                    (0b00000, Instr(Mnemonic.addc, r1, r2, r3)),        // P1 SR addc[o][.] Add Carrying
                    (0b00010, Nyi("")),                                 // v2.06 addg6s Add & Generate Sixes
                    (0b00100, Instr(Mnemonic.adde, C, r1, r2, r3)),     // P1 SR adde[o][.] Add Extended
                    (0b00110, Instr(Mnemonic.addze, C, r1, r2)),        // P1 SR addze[o][.] Add to Zero Extended
                    (0b00111, Instr(Mnemonic.addme, C, r1, r2)),        // P1 SR addme[o][.] Add to Minus One Extended
                    (0b01000, Instr(Mnemonic.add, C, r1, r2, r3)),      // P1 SR add[o][.] Add

                    (0b10000, Instr(Mnemonic.addco, r1, r2, r3)),       // P1 SR addc[o][.] Add Carrying
                    (0b10100, Instr(Mnemonic.addeo, C, r1, r2, r3)),    // P1 SR adde[o][.] Add Extended
                    (0b10110, Instr(Mnemonic.addzeo, C, r1, r2)),       // P1 SR addze[o][.] Add to Zero Extended
                    (0b10111, Instr(Mnemonic.addmeo, C, r1, r2)),       // P1 SR addme[o][.] Add to Minus One Extended
                    (0b11000, Instr(Mnemonic.addo, C, r1, r2, r3))),    // P1 SR add[o][.] Add

                Sparse(21, 5, invalid, "  0b01011",
                    (0b0000, Instr(Mnemonic.mulhwu, C, r1, r2, r3)),    // PPC SR mulhwu[.] Multiply High Word Unsigned
                    (0b0010, Instr(Mnemonic.mulhw, C, r1, r2, r3)),     // PPC SR mulhw[.] Multiply High Word
                    (0b0111, Instr(Mnemonic.mullw, C, r1, r2, r3)),     // P1 SR mullw[o][.] Multiply Low Word
                    (0b1100, Instr(Mnemonic.divweu, C, r1, r2, r3)),    // v2.06 SR divweu[o][.] Divide Word Extended Unsigned
                    (0b1101, Instr(Mnemonic.divwe, C, r1, r2, r3)),     // v2.06 SR divwe[o][.] Divide Word Extended
                    (0b1110, Instr(Mnemonic.divwu, C, r1, r2, r3)),     // PPC SR divwu[o][.] Divide Word Unsigned
                    (0b1111, Instr(Mnemonic.divw, C, r1, r2, r3)),      // PPC SR divw[o][.] Divide Word
                    (0b1000, Mask(21, 1, "  moduw/sw",
                        Instr(Mnemonic.moduw, r1, r2, r3),              // v3.0 moduw Modulo Unsigned Word
                        Instr(Mnemonic.modsw, r1, r2, r3)))),           // v3.0 modsw Modulo Signed Word
                
                Sparse(21, 5, invalid, "  0b01100",
                    (0b00000, Instr(Mnemonic.lxsiwzx, xt31_6, r2,r3)),  // v2.07 lxsiwzx Load VSX Scalar as Integer Word & Zero Indexed
                    (0b00010, Nyi("lxsiwax")),                          // v2.07 lxsiwax Load VSX Scalar as Integer Word Algebraic Indexed
                    (0b00100, Instr(Mnemonic.stxsiwx, xt31_6,r2,r3)),   // v2.07 stxsiwx Store VSX Scalar as Integer Word Indexed
                    (0b01000, Nyi("lxvx")),                             // v3.0 lxvx Load VSX Vector Indexed
                    (0b01010, Nyi("lxvdsx")),                           // v2.06 lxvdsx Load VSX Vector Dword & Splat Indexed
                    (0b01011, Nyi("lxvwsx")),                           // v3.0 lxvwsx Load VSX Vector Word & Splat Indexed
                    (0b01100, Instr(Mnemonic.stxvx, xt31_6,r2,r3)),     // v3.0 stxvx Store VSX Vector Indexed
                    (0b10000, Nyi("lxsspx")),                           // v2.07 lxsspx Load VSX Scalar SP Indexed
                    (0b10010, Instr(Mnemonic.lxsdx, xt31_6,r2,r3)),                            // v2.06 lxsdx Load VSX Scalar Dword Indexed
                    (0b10100, Nyi("stxsspx")),                          // v2.07 stxsspx Store VSX Scalar SP Indexed
                    (0b10110, Instr(Mnemonic.stxsdx,xt31_6,r2,r3)),     // v2.06 stxsdx Store VSX Scalar Dword Indexed
                    (0b11000, Nyi("lxvw4x")),                           // v2.06 lxvw4x Load VSX Vector Word*4 Indexed
                    (0b11001, Nyi("lxvh8x")),                           // v3.0 lxvh8x Load VSX Vector Hword*8 Indexed
                    (0b11010, Instr(Mnemonic.lxvd2x,xt31_6,r2,r3)),     // v2.06 lxvd2x Load VSX Vector Dword*2 Indexed
                    (0b11011, Instr(Mnemonic.lxvb16x,xt31_6,r2,r3)),    // v3.0 lxvb16x Load VSX Vector Byte*16 Indexed
                    (0b11100, Instr(Mnemonic.stxvw4x, xt31_6, r2, r3)), // v2.06 stxvw4x Store VSX Vector Word*4 Indexed
                    (0b11101, Instr(Mnemonic.stxvh8x, xt31_6, r2, r3)), // v3.0 stxvh8x Store VSX Vector Hword*8 Indexed
                    (0b11110, Instr(Mnemonic.stxvd2x, xt31_6, r2, r3)), // v2.06 stxvd2x Store VSX Vector Dword*2 Indexed
                    (0b11111, Instr(Mnemonic.stxvb16x,xt31_6,r2,r3))),  // v3.0 stxvb16x Store VSX Vector Byte*16 Indexed
                Sparse(21, 5, invalid, "  0b01101",
                    (0b01000, Nyi("lxvl")),                             // v3.0 lxvl Load VSX Vector with Length
                    (0b01001, Instr(Mnemonic.lxvll, xt31_6, r2, r3)),   // v3.0 lxvll Load VSX Vector Left-justified with Length
                    (0b01100, Nyi("stxvl")),                            // v3.0 stxvl Store VSX Vector with Length
                    (0b01101, Nyi("stxvll")),                           // v3.0 stxvll Store VSX Vector Left-justified with Length
                    (0b11000, Nyi("lxsibzx")),                          // v3.0 lxsibzx Load VSX Scalar as Integer Byte & Zero Indexed
                    (0b11001, Instr(Mnemonic.lxsihzx, xt31_6, r2, r3)), // v3.0 lxsihzx Load VSX Scalar as Integer Hword & Zero Indexed
                    (0b11100, Nyi("stxsibx")),                          // v3.0 stxsibx Store VSX Scalar as Integer Byte Indexed
                    (0b11101, Instr(Mnemonic.stxsihx, xt31_6, r2, r3))), // v3.0 stxsihx Store VSX Scalar as Integer Hword Indexed
                Sparse(21, 5, invalid, "  0b01110",
                    (0b00100, Nyi("msgsndp")),                              // v2.07 P msgsndp Message Send Privileged
                    (0b00101, Instr(LinPri, Mnemonic.msgclrp, r3)),         // v2.07 P msgclrp Message Clear Privileged
                    (0b00110, Instr(LinPri, Mnemonic.msgsnd, r3)),          // v2.07 H msgsnd Message Send
                    (0b00111, Instr(LinPri, Mnemonic.msgclr, r3)),          // v2.07 H msgclr Message Clear
                    (0b01001, Instr(Mnemonic.mfbhrbe, r1, u11_10)),         // v2.07 mfbhrbe Move From BHRB
                    (0b01101, Instr(Mnemonic.clrbhrb)),                     // v2.07 clrbhrb Clear BHRB
                    (0b10101, Nyi("tend.")),                                // v2.07 tend. Transaction End & record
                    (0b10110, Nyi("tcheck")),                               // v2.07 tcheck Transaction Check & record
                    (0b10111, Instr(Mnemonic.tsr, u21_1, CC)),              // v2.07 tsr.       Transaction Suspend or Resume & record
                    (0b10100, Instr(Mnemonic.tbegin, u21_1, CC)),           // v2.07 tbegin.    Transaction Begin & record
                    (0b11000, Nyi("tabortwc.")),                            // v2.07 tabortwc.  Transaction Abort Word Conditional & record
                    (0b11001, Instr(Mnemonic.tabortdc, u21_5,r2,r3, CC)),   // v2.07 tabortdc.  Transaction Abort Dword Conditional & record
                    (0b11010, Nyi("tabortwci.")),                           // v2.07 tabortwci. Transaction Abort Word Conditional Immediate & record
                    (0b11011, Nyi("tabortdci.")),                           // v2.07 tabortdci. Transaction Abort Dword Conditional Immediate & record
                    (0b11100, Instr(Mnemonic.tabort, r2, CC)),              // v2.07 tabort.    Transaction Abort & record
                    (0b11101, Instr(LinPri, Mnemonic.treclaim, r2, CC)),    // v2.07 P treclaim.Transaction Reclaim & record
                    (0b11111, Instr(LinPri, Mnemonic.trechkpt, CC))),       // v2.07 P trechkpt.Transaction Recheckpoint & record
                Instr(Mnemonic.isel, r1,r2,r3,I4),                          // v2.03 isel Integer Select

                Sparse(21, 5, invalid, "  0b10000",
                    (0b00100, Mask(11, 1, "  mtcrf/mtocrf",
                        Instr(Mnemonic.mtcrf, M, r1),                   // P1 mtcrf Move To CR Fields
                        Instr(Mnemonic.mtocrf, u21_5, r1)))),           // v2.01 mtocrf Move To One CR Field
                invalid,
                Sparse(21, 5, invalid, "  0b10010",
                    (0b00100, Instr(LinPri, Mnemonic.mtmsr, r1, u16_1)),    // P1 P mtmsr Move To MSR
                    (0b00101, Instr(LinPri, Mnemonic.mtmsrd, r1, u16_1)),   // PPC P mtmsrd Move To MSR Dword
                    (0b01000, Instr(LinPri, Mnemonic.tlbiel, r3, r1, u18_2,u17_1,u16_1)),   // v2.03 P 64 tlbiel TLB Invalidate Entry Local
                    (0b01001, Instr(LinPri, Mnemonic.tlbie, r3)),       // P1 H 64 tlbie TLB Invalidate Entry
                    (0b01010, Instr(LinPri, Mnemonic.slbsync)),         // v3.0 P slbsync SLB Synchronize
                    (0b01100, Instr(LinPri, Mnemonic.slbmte, r1, r3)),  // v2.00 P slbmte SLB Move To Entry
                    (0b01101, Instr(LinPri, Mnemonic.slbie, r3)),       // PPC P slbie SLB Invalidate Entry
                    (0b01110, Instr(LinPri, Mnemonic.slbieg, r1, r3)),  // v3.0 P slbieg SLB Invalidate Entry Global
                    (0b01111, Nyi("P"))),                               // PPC P slbia SLB Invalidate All
                Sparse(21, 5, invalid, "  0b10011",
                    (0b00000, Mask(11, 1, "  mfcr,mfocrf",
                        Instr(Mnemonic.mfcr, r1),                       // P1 mfcr Move From CR
                        Instr(Mnemonic.mfocrf, r1,M))),                 // v2.01 mfocrf Move From One CR Field
                    (0b00001, Mask(31, 1, "  mfvsrd",
                        Instr(Mnemonic.mffprd, r2,f1),                 // v2.07 mfvsrd Move From VSR Dword
                        Instr(Mnemonic.mfvrd, r2,v1))),                 // v2.07 mfvsrd Move From VSR Dword
                    (0b00010, Instr(Mnemonic.mfmsr, r1)),               // P1 P mfmsr Move From MSR
                    (0b00011, Nyi("mfvsrwz")),                          // v2.07 mfvsrwz Move From VSR Word & Zero
                    (0b00101, Instr(Mnemonic.mtvsrd, vsr1, r2)),        // v2.07 mtvsrd Move To VSR Dword
                    (0b00110, Instr(Mnemonic.mtvsrwa, xt31_6, r2)),     // v2.07 mtvsrwa Move To VSR Word Algebraic
                    (0b00111, Nyi("mtvsrwz")),                          // v2.07 mtvsrwz Move To VSR Word & Zero
                    (0b01001, Instr(Mnemonic.mfvsrld, r2, xt31_6)),     // v3.0 mfvsrld Move From VSR Lower Dword
                    (0b01010, new SprDecoder(false)),                   // P1 O mfspr Move From SPR
                    (0b01011, new XfxDecoder(Mnemonic.mftb, r1, X3)),   // PPC mftb Move From Time Base   //$TODO don't need special decoder for SPR field
                    (0b01101, Instr(Mnemonic.mtvsrdd, xt31_6, r2, r3)), // v3.0 mtvsrdd Move To VSR Double Dword
                    (0b01100, Instr(Mnemonic.mtvsrws, xt31_6, r2)),     // v3.0 mtvsrws Move To VSR Word & Splat
                    (0b01110, new SprDecoder(true)),                    // P1 O mtspr Move To SPR
                    (0b10111, Instr(Mnemonic.darn, r1, u16_2)),         // v3.0 darn Deliver A Random Number
                    (0b11010, Nyi("slbmfev")),                          // v2.00 P slbmfev SLB Move From Entry VSID
                    (0b11100, Instr(LinPri, Mnemonic.slbmfee, r1, r3)), // v2.00 P slbmfee SLB Move From Entry ESID
                    (0b11110, Nyi("slbfee."))),                         // 1 X v2.05 P SR slbfee. SLB Find Entry ESID & record

                Sparse(21, 5, invalid, "  0b10100",
                    (0b00000, Instr(Mnemonic.lwarx, r1, r2, r3)),       // PPC lwarx Load Word & Reserve Indexed
                    (0b00001, Instr(Mnemonic.lbarx, r1, r2, r3)),       // v2.06 lbarx Load Byte And Reserve Indexed
                    (0b00010, Instr(Mnemonic.ldarx, r1, r2, r3)),       // PPC ldarx Load Dword And Reserve Indexed
                    (0b00011, Instr(Mnemonic.lharx, r1, r2, r3)),       // v2.06 lharx Load Hword And Reserve Indexed Xform
                    (0b01000, Instr(Mnemonic.lqarx, r1, r2, r3)),       // v2.07 lqarx Load Qword And Reserve Indexed
                    (0b10000, Instr(Mnemonic.ldbrx, r1, r2, r3)),       // v2.06 ldbrx Load Dword Byte-Reverse Indexed
                    (0b10100, Instr(Mnemonic.stdbrx, r1, r2, r3))),     // v2.06 stdbrx Store Dword Byte-Reverse Indexed
                Sparse(21, 5, invalid, "  0b10101",
                    (0b00000, Instr(Mnemonic.ldx, r1, r2, r3)),         // PPC ldx Load Dword Indexed
                    (0b00001, Instr(Mnemonic.ldux, r1, r2, r3)),        // PPC ldux Load Dword with Update Indexed
                    (0b00100, Instr(Mnemonic.stdx, r1, r2, r3)),        // PPC stdx Store Dword Indexed
                    (0b00101, Instr(Mnemonic.stdux, r1, r2, r3)),       // PPC stdux Store Dword with Update Indexed
                    (0b01001, Instr(Mnemonic.ldmx, r1, r2, r3)),        // v3.0 PI ldmx Load Dword Monitored Indexed
                    (0b01010, Instr(Mnemonic.lwax, r1, r2, r3)),        // PPC lwax Load Word Algebraic Indexed
                    (0b01011, Instr(Mnemonic.lwaux, r1, r2,r3)),        // PPC lwaux Load Word Algebraic with Update Indexed
                    (0b10000, Instr(Mnemonic.lswx, r1, r2, r3)),        // P1 lswx Load String Word Indexed
                    (0b10010, Instr(Mnemonic.lswi, r1, r2, I3)),        // P1 lswi Load String Word Immediate
                    (0b10100, Instr(Mnemonic.stswx, r1, r2, r3)),       // P1 stswx Store String Word Indexed
                    (0b10110, Instr(Mnemonic.stswi, r1, r2, I3)),       // P1 stswi Store String Word Immediate
                    (0b11000, Nyi("lwzcix")),                           // v2.05 H lwzcix Load Word & Zero Caching Inhibited Indexed
                    (0b11001, Nyi("lhzcix")),                           // v2.05 H lhzcix Load Hword & Zero Caching Inhibited Indexed
                    (0b11010, Instr(LinPri, Mnemonic.lbzcix, r1, r2, r3)),  // v2.05 H lbzcix Load Byte & Zero Caching Inhibited Indexed
                    (0b11011, Instr(LinPri, Mnemonic.ldcix, r1, r2, r3)),   // v2.05 H ldcix Load Dword Caching Inhibited Indexed
                    (0b11100, Instr(LinPri, Mnemonic.stwcix, r1, r2, r3)),  // v2.05 H stwcix Store Word Caching Inhibited Indexed
                    (0b11101, Instr(LinPri, Mnemonic.sthcix, r1, r2, r3)),  // v2.05 H sthcix Store Hword Caching Inhibited Indexed
                    (0b11110, Instr(LinPri, Mnemonic.stbcix, r1, r2, r3)),  // v2.05 H stbcix Store Byte Caching Inhibited Indexed
                    (0b11111, Instr(LinPri, Mnemonic.stdcix, r1, r2, r3))), // v2.05 H stdcix Store Dword Caching Inhibited Indexed
                Sparse(21, 5, invalid, "  0b10110",
                    (0b00000, Instr(Mnemonic.icbt, r2, r3)),            // v2.07 icbt Instruction Cache Block Touch
                    (0b00001, Instr(Mnemonic.dcbst, r2, r3)),           // PPC dcbst Data Cache Block Store
                    (0b00010, Instr(Mnemonic.dcbf, r2, r3)),            // PPC dcbf Data Cache Block Flush
                    (0b00111, Instr(Mnemonic.dcbtst, r2, r3)),          // PPC dcbtst Data Cache Block Touch for Store
                    (0b01000, Instr(Mnemonic.dcbt, r2, r3, u21_4)),     // PPC dcbt Data Cache Block Touch
                    (0b01110, Instr(LinPri, Mnemonic.dcbi, r2, r3)),    // PPC-ONLY dcbi P Data Cache Block iInvalidate
                    (0b10000, Instr(Mnemonic.lwbrx, r1, r2, r3)),       // P1 lwbrx Load Word Byte-Reverse Indexed
                    (0b10001, Instr(LinPri, Mnemonic.tlbsync)),         // PPC H tlbsync TLB Synchronize
                    (0b10010, Instr(Mnemonic.sync)),                    // P1 sync Synchronize
                    (0b10100, Instr(Mnemonic.stwbrx, C, r2, r1, r3)),   // P1 stwbrx Store Word Byte-Reverse Indexed
                    (0b11000, Instr(Mnemonic.lhbrx, r1, r2, r3)),       // P1 lhbrx Load Hword Byte-Reverse Indexed
                    (0b11010, Instr(Mnemonic.eieio)),                   // PPC eieio Enforce In-order Execution of I/O
                    (0b11011, Nyi("msgsync")),                          // v3.0 H msgsync Message Synchronize
                    (0b11100, Instr(Mnemonic.sthbrx, r2, r1, r3)),      // P1 sthbrx Store Hword Byte-Reverse Indexed
                    (0b11110, Instr(Mnemonic.icbi, r2, r3)),            // PPC icbi Instruction Cache Block Invalidate
                    (0b11111, Instr(Mnemonic.dcbz, r2, r3)),            // P1 dcbz Data Cache Block Zero
                    (0b00100, Instr(Mnemonic.stwcx, CC, r1, r2, r3)),   // PPC stwcx. Store Word Conditional Indexed & record
                    (0b00101, Instr(Mnemonic.stqcx, CC, r1, r2, r3)),   // v2.07 stqcx. Store Qword Conditional Indexed & record
                    (0b00110, Instr(Mnemonic.stdcx, CC, r1, r2, r3)),   // PPC stdcx. Store Dword Conditional Indexed & record
                    (0b10101, Instr(Mnemonic.stbcx, CC, r1, r2, r3)),   // v2.06 stbcx. Store Byte Conditional Indexed & record
                    (0b10110, Instr(Mnemonic.sthcx, CC, r1, r2, r3))),  // v2.06 sthcx. Store Hword Conditional Indexed & record
                Sparse(21, 5, invalid, "  0b10111",
                    (0b00000, Instr(Mnemonic.lwzx, r1, r2, r3)),        // P1 lwzx Load Word & Zero Indexed
                    (0b00001, Instr(Mnemonic.lwzux, r1, r2, r3)),       // P1 lwzux Load Word & Zero with Update Indexed
                    (0b00010, Instr(Mnemonic.lbzx, r1, r2, r3)),        // P1 lbzx Load Byte & Zero Indexed
                    (0b00011, Instr(Mnemonic.lbzux, r1, r2, r3)),       // P1 lbzux Load Byte & Zero with Update Indexed
                    (0b00100, Instr(Mnemonic.stwx, r1, r2, r3)),        // P1 stwx Store Word Indexed
                    (0b00101, Instr(Mnemonic.stwux, r1, r2, r3)),       // P1 stwux Store Word with Update Indexed
                    (0b00110, Instr(Mnemonic.stbx, r1, r2, r3)),        // P1 stbx Store Byte Indexed
                    (0b00111, Instr(Mnemonic.stbux, r1, r2, r3)),       // P1 stbux Store Byte with Update Indexed
                    (0b01000, Instr(Mnemonic.lhzx, r1, r2, r3)),        // P1 lhzx Load Hword & Zero Indexed
                    (0b01001, Instr(Mnemonic.lhzux, r1, r2, r3)),       // P1 lhzux Load Hword & Zero with Update Indexed
                    (0b01010, Instr(Mnemonic.lhax, r1, r2, r3)),        // P1 lhax Load Hword Algebraic Indexed
                    (0b01011, Instr(Mnemonic.lhaux, r1, r2, r3)),       // P1 lhaux Load Hword Algebraic with Update Indexed
                    (0b01100, Instr(Mnemonic.sthx, r1, r2, r3)),        // P1 sthx Store Hword Indexed
                    (0b01101, Instr(Mnemonic.sthux, r1, r2, r3)),       // P1 sthux Store Hword with Update Indexed
                    (0b10000, Instr(Mnemonic.lfsx, f1, r2, r3)),        // P1 lfsx Load Floating Single Indexed
                    (0b10001, Instr(Mnemonic.lfsux, f1, r2, r3)),       // P1 lfsux Load Floating Single with Update Indexed
                    (0b10010, Instr(Mnemonic.lfdx, f1, r2, r3)),        // P1 lfdx Load Floating Double Indexed
                    (0b10011, Instr(Mnemonic.lfdux, f1, r2, r3)),       // P1 lfdux Load Floating Double with Update Indexed
                    (0b10100, Instr(Mnemonic.stfsx, f1, r2, r3)),       // P1 stfsx Store Floating Single Indexed
                    (0b10101, Instr(Mnemonic.stfsux, f1, r2, r3)),      // P1 stfsux Store Floating Single with Update Indexed
                    (0b10110, Instr(Mnemonic.stfdx, f1, r2, r3)),       // P1 stfdx Store Floating Double Indexed
                    (0b10111, Instr(Mnemonic.stfdux, f1, r2, r3)),      // P1 stfdux Store Floating Double with Update Indexed
                    (0b11000, Instr(Mnemonic.lfdpx, f1, r2, r3)),       // v2.05 lfdpx Load Floating Double Pair Indexed
                    (0b11010, Instr(Mnemonic.lfiwax, f1, r2, r3)),      // v2.05 lfiwax Load Floating as Integer Word Algebraic Indexed
                    (0b11011, Instr(Mnemonic.lfiwzx, f1, r2, r3)),      // v2.06 lfiwzx Load Floating as Integer Word & Zero Indexed
                    (0b11100, Instr(Mnemonic.stfdpx, f1, r2, r3)),      // v2.05 stfdpx Store Floating Double Pair Indexed
                    (0b11110, Instr(Mnemonic.stfiwx, f1, r2, r3))),     // PPC stfiwx Store Floating as Integer Word Indexed
                Sparse(21, 5, invalid, "  0b11000",
                    (0b00000, Instr(Mnemonic.slw, C, r2, r1, r3)),      // P1 SR slw[.] Shift Left Word
                    (0b10000, Instr(Mnemonic.srw, C, r2, r1, r3)),      // P1 SR srw[.] Shift Right Word
                    (0b11000, Instr(Mnemonic.sraw, C, r2, r1, r2)),     // P1 SR sraw[.] Shift Right Algebraic Word
                    (0b11001, Instr(Mnemonic.srawi, C, r2, r1, I3))),   // P1 SR srawi[.] Shift Right Algebraic Word Immediate
                invalid,    // 0b11001,
                Sparse(21, 5, invalid, "  0b11010",
                    (0b00000, Instr(Mnemonic.cntlzw, r2, r1)),          // P1 SR cntlzw[.] Count Leading Zeros Word
                    (0b00001, Instr(Mnemonic.cntlzd, r2, r1)),          // PPC SR cntlzd[.] Count Leading Zeros Dword
                    (0b00011, Instr(Mnemonic.popcntb, r2, r1)),         // v2.02 popcntb Population Count Byte
                    (0b00100, Instr(Mnemonic.prtyw, r2, r1)),           // v2.05 prtyw Parity Word
                    (0b00101, Instr(Mnemonic.prtyd, r2, r1)),           // v2.05 prtyd Parity Dword
                    (0b01000, Nyi("cdtbcd")),                           // v2.06 cdtbcd Convert Declets To Binary Coded Decimal
                    (0b01001, Instr(Mnemonic.cbcdtd, r2, r1)),          // v2.06 cbcdtd Convert Binary Coded Decimal To Declets
                    (0b01011, Instr(Mnemonic.popcntw, r2, r1)),         // v2.06 popcntw Population Count Words
                    (0b01111, Instr(Mnemonic.popcntd, r2, r1)),         // v2.06 popcntd Population Count Dword
                    (0b10000, Instr(Mnemonic.cnttzw, C, r2, r1)),       // v3.0 cnttzw[.] Count Trailing Zeros Word
                    (0b10001, Instr(Mnemonic.cnttzd, C, r2, r1)),       // v3.0 cnttzd[.] Count Trailing Zeros Dword
                    (0b11000, Instr(Mnemonic.srad, C, r2, r1, r2)),     // PPC SR srad[.] Shift Right Algebraic Dword
                    (0b11001, Instr(Mnemonic.sradi, C, r2, r1, i(BeFields((30,1),(16,5))))),    // PPC SR sradi[.] Shift Right Algebraic Dword Immediate
                    (0b11011, Instr(Mnemonic.extswsli, C, r2, r1, u11_5)),  // v3.0 extswsli[.] Extend Sign Word & Shift Left Immediate
                    (0b11100, Instr(Mnemonic.extsh, C, r2, r1)),        // P1 SR extsh[.] Extend Sign Hword
                    (0b11101, Instr(Mnemonic.extsb, C, r2, r1)),        // PPC SR extsb[.] Extend Sign Byte
                    (0b11110, Instr(Mnemonic.extsw, C, r2, r1))),       // PPC SR extsw[.] Extend Sign Word
                Sparse(21, 5, invalid, "  0b11011",
                    (0b00000, Instr(Mnemonic.sld, C, r2, r1, r3)),      // PPC SR sld[.] Shift Left Dword
                    (0b10000, Instr(Mnemonic.srd, C, r2, r1, r3)),      // PPC SR srd[.] Shift Right Dword
                    (0b11001, Instr(Mnemonic.sradi, C, r2, r1, i(BeFields((30, 1), (16, 5))))),    // PPC SR sradi[.] Shift Right Algebraic Dword Immediate
                    (0b11011, Nyi("extswsli"))),   // v3.0 extswsli[.] Extend Sign Word & Shift Left Immediate

                Sparse(21, 5, invalid, "  0b11100",
                    (0b00000, Instr(Mnemonic.and, C, r2, r1, r3)),      // P1 SR and[.] AND
                    (0b00001, Instr(Mnemonic.andc, C, r2, r1, r3)),     // P1 SR andc[.] AND with Complement
                    (0b00011, Instr(Mnemonic.nor, C, r2, r1, r3)),      // P1 SR nor[.] NOR
                    (0b00111, Nyi("bpermd")), // v2.06 bpermd Bit Permute Dword
                    (0b01000, Instr(Mnemonic.eqv, r1,r2,r3)), // P1 SR eqv[.] Equivalent
                    (0b01001, Instr(Mnemonic.xor, C, r2, r1, r3)), // P1 SR xor[.] XOR
                    (0b01100, Instr(Mnemonic.orc, C, r2, r1, r3)), // P1 SR orc[.] OR with Complement
                    (0b01101, Instr(Mnemonic.or, C, r2, r1, r3)), // P1 SR or[.] OR
                    (0b01110, Instr(Mnemonic.nand, C, r2, r1, r3)), // P1 SR nand[.] NAND
                    (0b01111, Instr(Mnemonic.cmpb, r2, r1, r3))), // v2.05 cmpb Compare Bytes
                invalid,    // 0b11101
                Sparse(21, 5, invalid, "  0b11110",
                    (0b00000, Nyi("wait"))), // v3.0 wait Wait for Interrupt
                invalid);    // 0b11111
            return decoder;
        }

        public virtual Decoder Ext38Decoder()
        {
            return Instr(Mnemonic.lq, Is64Bit, rp1,E2);
        }

        public virtual Decoder Ext39Decoder()
        {
            return Instr(Mnemonic.lfdp, f1p, E2_2);
            //111001 ..... ..... ..... ..... ....00 DS I 150 v2.05 lfdp Load Floating Double Pair
            //111001 ..... ..... ..... ..... ....10 DS I 481 v3.0 lxsd Load VSX Scalar Dword
            //111001 ..... ..... ..... ..... ....11 DS I 486 v3.0 lxssp Load VSX Scalar Single

        }

        public virtual Decoder Ext3BDecoder()
        {
            var decoder = Sparse(26, 5, invalid, "  Ext3B",
                (0b00010, Mask(21, 5, "  00010",
                    Instr(Mnemonic.dadd, C, f1,f2,f3),      // 111011 ..... ..... ..... 00000 00010. X I 195 v2.05 dadd[.] DFP Add
                    Nyi("dmul[.] D"), // 111011 ..... ..... ..... 00001 00010. X I 197 v2.05 dmul[.] DFP Multiply
                    Nyi("dscli[.]"),  // 111011 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscli[.] DFP Shift Significand Left Immediate
                    Instr(Mnemonic.dscri, C,f1,f2,u10_6),   // Z22 I 222 v2.05 dscri[.] DFP Shift Significand Right Immediate

                    Nyi("dcmpo DFP"), // 111011 ...// ..... ..... 00100 00010/ X I 201 v2.05 dcmpo DFP Compare Ordered
                    Nyi("dtstex DF"), // 111011 ...// ..... ..... 00101 00010/ X I 203 v2.05 dtstex DFP Test Exponent
                    Nyi("dtstdc"),    // 111011 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdc DFP Test Data Class
                    Nyi("dtstdg"),    // 111011 ...// ..... ..... .0111 00010/ Z22 I 202 v2.05 dtstdg DFP Test Data Group

                    Nyi("dctdp[.] "), // 111011 ..... ///// ..... 01000 00010. X I 215 v2.05 dctdp[.] DFP Convert To DFP Long
                    Instr(Mnemonic.dctfix, C,f1,f3),        // X I 217 v2.05 dctfix[.] DFP Convert To Fixed
                    Nyi("ddedpd[.]"), // 111011 ..... ../// ..... 01010 00010. X I 219 v2.05 ddedpd[.] DFP Decode DPD To BCD
                    Nyi("dxex[.] D"), // 111011 ..... ///// ..... 01011 00010. X I 220 v2.05 dxex[.] DFP Extract Exponent

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.dsub, C,f1, f2, f3),     // 111011 ..... ..... ..... 10000 00010. X I 195 v2.05 dsub[.] DFP Subtract
                    Nyi("ddiv[.] D"), // 111011 ..... ..... ..... 10001 00010. X I 198 v2.05 ddiv[.] DFP Divide
                    Nyi("dscli[.]"),  // 111011 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscli[.] DFP Shift Significand Left Immediate
                    Nyi("dscri[.]"),  // 111011 ..... ..... ..... .0011 00010. Z22 I 222 v2.05 dscri[.] DFP Shift Significand Right Immediate

                    Instr(Mnemonic.dcmpu, C1,f1,f2),        // 111011 ...// ..... ..... 10100 00010/ X I 200 v2.05 dcmpu DFP Compare Unordered
                    Nyi("dtstsf DF"), // 111011 ...// ..... ..... 10101 00010/ X I 204 v2.05 dtstsf DFP Test Significance
                    Nyi("dtstdc"),    // 111011 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdc DFP Test Data Class
                    Nyi("dtstdg"),    // 111011 ...// ..... ..... .0111 00010/ Z22 I 202 v2.05 dtstdg DFP Test Data Group

                    Nyi("drsp[.] D"), // 111011 ..... ///// ..... 11000 00010. X I 216 v2.05 drsp[.] DFP Round To DFP Short
                    Nyi("dcffix[.]"), // 111011 ..... ///// ..... 11001 00010. X I 217 v2.06 dcffix[.] DFP Convert From Fixed
                    Instr(Mnemonic.denbcd, u20_1,f1,f3),    // 111011 ..... .//// ..... 11010 00010. X I 219 v2.05 denbcd[.] DFP Encode BCD To DPD
                    Nyi("diex[.] D"), // 111011 ..... ..... ..... 11011 00010. X I 220 v2.05 diex[.] DFP Insert Exponent

                    invalid,
                    invalid,
                    invalid,
                    invalid)),
                (0b00011, Mask(23, 3, "  00011",
                    Nyi("dqua[.] D"), // 111011 ..... ..... ..... ..000 00011. Z23 I 206 v2.05 dqua[.] DFP Quantize
                    Nyi("drrnd[.] "), // 111011 ..... ..... ..... ..001 00011. Z23 I 208 v2.05 drrnd[.] DFP Reround
                    Nyi("dquai[.] "), // 111011 ..... ..... ..... ..010 00011. Z23 I 205 v2.05 dquai[.] DFP Quantize Immediate
                    Nyi("drintx[.]"), // 111011 ..... ////. ..... ..011 00011. Z23 I 211 v2.05 drintx[.] DFP Round To FP Integer With Inexact
                    invalid,
                    Nyi("dtstsfi D"),  // 111011 ...// ..... ..... 10101 00011/ X I 204 v3.0 dtstsfi DFP Test Significance Immediate
                    invalid,
                    Instr(Mnemonic.drintn, C,u16_1,f1,f3,u9_3))),   // 111011 ..... ////. ..... ..111 00011. Z23 I 213 v2.05 drintn[.] DFP Round To FP Integer Without Inexact
                (0b01110, Sparse(21, 5, invalid, "  01110",
                    (0b11010, Instr(Mnemonic.fcfids, C, f1, f3)),                   // 111011 ..... ///// ..... 11010 01110. X I 165 v2.06 fcfids[.] Floating Convert From Integer Dword Single
                    (0b11110, Instr(Mnemonic.fcfidus, C, f1, f3)))),                 // 111011 ..... ///// ..... 11110 01110. X I 166 v2.06 fcfidus[.] Floating Convert From Integer Dword Unsigned Single
                (0b10010, Instr(Mnemonic.fdivs, C,f1,f2,f3)),       // 111011 ..... ..... ..... ///// 10010. A I 154 PPC fdivs[.] Floating Divide Single
                (0b10100, Instr(Mnemonic.fsubs, C, f1, f2, f3)),    // 111011 ..... ..... ..... ///// 10100. A I 153 PPC fsubs[.] Floating Subtract Single
                (0b10101, Instr(Mnemonic.fadds, C, f1, f2, f3)),    // 111011 ..... ..... ..... ///// 10101. A I 153 PPC fadds[.] Floating Add Single
                (0b10110, Instr(Mnemonic.fsqrts, C, f1, f3)),       // 111011 ..... ///// ..... ///// 10110. A I 155 PPC fsqrts[.] Floating Square Root Single
                (0b11000, Instr(Mnemonic.fres, C, f1, f3)),         // 111011 ..... ///// ..... ///// 11000. A I 155 PPC fres[.] Floating Reciprocal Estimate Single
                (0b11001, Instr(Mnemonic.fmuls, C, f1, f2, f4)),    // 111011 ..... ..... ///// ..... 11001. A I 154 PPC fmuls[.] Floating Multiply Single
                (0b11010, Instr(Mnemonic.frsqrtes, C, f1, f3)),     // 111011 ..... ///// ..... ///// 11010. A I 156 v2.02 frsqrtes[.] Floating Reciprocal Square Root Estimate Single
                (0b11100, Instr(Mnemonic.fmsubs, C,f1,f2,f4,f3)),   // 111011 ..... ..... ..... ..... 11100. A I 159 PPC fmsubs[.] Floating Multiply-Subtract Single
                (0b11101, Instr(Mnemonic.fmadds, C,f1,f2,f4,f3)),   // 111011 ..... ..... ..... ..... 11101. A I 158 PPC fmadds[.] Floating Multiply-Add Single
                (0b11110, Instr(Mnemonic.fnmsubs, C,f1,f2,f3,f4)),  // 111011 ..... ..... ..... ..... 11110. A I 159 PPC fnmsubs[.] Floating Negative Multiply-Subtract Single
                (0b11111, Instr(Mnemonic.fnmadds, C,f1,f2,f3,f4))); // 111011 ..... ..... ..... ..... 11111. A I 159 PPC fnmadds[.] Floating Negative Multiply-Add Single
            return decoder;
        }

        public virtual Decoder Ext3CDecoder()
        {
            var xxsldwi = Instr(Mnemonic.xxsldwi, v1, v2, v3);
            var xxpermdi = Instr(Mnemonic.xxpermdi, vsr1, vsr2, vsr3, u22_2);                            // 111100 ..... ..... ..... 0..01 010... XX3 I 777 v2.06 xxpermdi VSX Vector Dword Permute Immediate

            var decoder = Mask(26, 3, "Ext3C",
                Mask(21, 5, "  000",
                    Instr(Mnemonic.xsaddsp, vsr1, vsr2, vsr3),    // 111100 ..... ..... ..... 00000 000... XX3 I 519 v2.07 xsaddsp VSX Scalar Add SP
                    Instr(Mnemonic.xssubsp, vsr1, vsr2, vsr3),    // 111100 ..... ..... ..... 00001 000... XX3 I 651 v2.07 xssubsp VSX Scalar Subtract SP
                    Instr(Mnemonic.xsmulsp, vsr1, vsr2, vsr3),    // 111100 ..... ..... ..... 00010 000... XX3 I 606 v2.07 xsmulsp VSX Scalar Multiply SP
                    Instr(Mnemonic.xsdivsp, vsr1, vsr2, vsr3),    // 111100 ..... ..... ..... 00011 000... XX3 I 568 v2.07 xsdivsp VSX Scalar Divide SP
                    Instr(Mnemonic.xsadddp, vsr1, vsr2, vsr3),    // 111100 ..... ..... ..... 00100 000... XX3 I 514 v2.06 xsadddp VSX Scalar Add DP
                    Instr(Mnemonic.xssubdp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 00101 000... XX3 I 647 v2.06 xssubdp VSX Scalar Subtract DP
                    Instr(Mnemonic.xsmuldp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 00110 000... XX3 I 602 v2.06 xsmuldp VSX Scalar Multiply DP
                    Instr(Mnemonic.xsdivdp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 00111 000... XX3 I 564 v2.06 xsdivdp VSX Scalar Divide DP
                    Instr(Mnemonic.xvaddsp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 01000 000... XX3 I 665 v2.06 xvaddsp VSX Vector Add SP
                    Instr(Mnemonic.xvsubsp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 01001 000... XX3 I 759 v2.06 xvsubsp VSX Vector Subtract SP
                    Instr(Mnemonic.xvmulsp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 01010 000... XX3 I 727 v2.06 xvmulsp VSX Vector Multiply SP
                    Instr(Mnemonic.xvdivsp, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 01011 000... XX3 I 702 v2.06 xvdivsp VSX Vector Divide SP
                    Instr(Mnemonic.xvadddp, vsr1, vsr2, vsr3),      // XX3 I 661 v2.06 xvadddp VSX Vector Add DP
                    Instr(Mnemonic.xvsubdp, vsr1, vsr2, vsr3),      // XX3 I 757 v2.06 xvsubdp VSX Vector Subtract DP
                    Instr(Mnemonic.xvmuldp, vsr1, vsr2, vsr3),      // XX3 I 725 v2.06 xvmuldp VSX Vector Multiply DP
                    Instr(Mnemonic.xvdivdp, vsr1, vsr2, vsr3),      // XX3 I 700 v2.06 xvdivdp VSX Vector Divide DP
                    Instr(Mnemonic.xsmaxcdp, vsr1,vsr2,vsr3),       // XX3 I 583 v3.0 xsmaxcdp VSX Scalar Maximum Type-C Double-Precision
                    Nyi("xsmincdp"),   // 111100 ..... ..... ..... 10001 000... XX3 I 589 v3.0 xsmincdp VSX Scalar Minimum Type-C Double-Precision
                    Nyi("xsmaxjdp"),   // 111100 ..... ..... ..... 10010 000... XX3 I 585 v3.0 xsmaxjdp VSX Scalar Maximum Type-J Double-Precision
                    Nyi("xsminjdp"),   // 111100 ..... ..... ..... 10011 000... XX3 I 591 v3.0 xsminjdp VSX Scalar Minimum Type-J Double-Precision
                    Nyi("xsmaxdp"),    // 111100 ..... ..... ..... 10100 000... XX3 I 581 v2.06 xsmaxdp VSX Scalar Maximum DP
                    Nyi("xsmindp"),    // 111100 ..... ..... ..... 10101 000... XX3 I 587 v2.06 xsmindp VSX Scalar Minimum DP
                    Instr(Mnemonic.xscpsgndp, vsr1, vsr2, vsr3),    // I 535 v2.06 xscpsgndp VSX Scalar Copy Sign DP
                    invalid,
                    Nyi("xvmaxsp"),    // 111100 ..... ..... ..... 11000 000... XX3 I 713 v2.06 xvmaxsp VSX Vector Maximum SP
                    Nyi("xvminsp"),    // 111100 ..... ..... ..... 11001 000... XX3 I 717 v2.06 xvminsp VSX Vector Minimum SP
                    Nyi("xvcpsgnsp"),  // 111100 ..... ..... ..... 11010 000... XX3 I 675 v2.06 xvcpsgnsp VSX Vector Copy Sign SP
                    Nyi("xviexpsp"),   // 111100 ..... ..... ..... 11011 000... XX3 I 704 v3.0 xviexpsp VSX Vector Insert Exponent SP
                    Nyi("xvmaxdp"),    // 111100 ..... ..... ..... 11100 000... XX3 I 711 v2.06 xvmaxdp VSX Vector Maximum DP
                    Nyi("xvmindp"),    // 111100 ..... ..... ..... 11101 000... XX3 I 715 v2.06 xvmindp VSX Vector Minimum DP
                    Nyi("xvcpsgndp"),  // 111100 ..... ..... ..... 11110 000... XX3 I 675 v2.06 xvcpsgndp VSX Vector Copy Sign DP
                    Nyi("xviexpdp")),  // 111100 ..... ..... ..... 11111 000... XX3 I 704 v3.0 xviexpdp VSX Vector Insert Exponent DP
                Mask(21, 5, "  001",
                    Instr(Mnemonic.xsmaddasp, v1,v2,v3),            // 111100 ..... ..... ..... 00000 001... XX3 I 575 v2.07 xsmaddasp VSX Scalar Multiply-Add Type-A SP
                    Instr(Mnemonic.xsmaddmsp, v1,v2,v3),            // 111100 ..... ..... ..... 00001 001... XX3 I 575 v2.07 xsmaddmsp VSX Scalar Multiply-Add Type-M SP
                    Nyi("xsmsubasp"),    // 111100 ..... ..... ..... 00010 001... XX3 I 596 v2.07 xsmsubasp VSX Scalar Multiply-Subtract Type-A SP
                    Nyi("xsmsubmsp"),    // 111100 ..... ..... ..... 00011 001... XX3 I 596 v2.07 xsmsubmsp VSX Scalar Multiply-Subtract Type-M SP
                    
                    Nyi("xsmaddadp"),    // 111100 ..... ..... ..... 00100 001... XX3 I 572 v2.06 xsmaddadp VSX Scalar Multiply-Add Type-A DP
                    Instr(Mnemonic.xsmaddmdp, vsr1,vsr2,vsr3),      // XX3 I 572 v2.06 xsmaddmdp VSX Scalar Multiply-Add Type-M DP
                    Nyi("xsmsubadp"),    // 111100 ..... ..... ..... 00110 001... XX3 I 593 v2.06 xsmsubadp VSX Scalar Multiply-Subtract Type-A DP
                    Nyi("xsmsubmdp"),    // 111100 ..... ..... ..... 00111 001... XX3 I 593 v2.06 xsmsubmdp VSX Scalar Multiply-Subtract Type-M DP
                    
                    Nyi("xvmaddasp"),    // 111100 ..... ..... ..... 01000 001... XX3 I 708 v2.06 xvmaddasp VSX Vector Multiply-Add Type-A SP
                    Nyi("xvmaddmsp"),    // 111100 ..... ..... ..... 01001 001... XX3 I 708 v2.06 xvmaddmsp VSX Vector Multiply-Add Type-M SP
                    Nyi("xvmsubasp"),    // 111100 ..... ..... ..... 01010 001... XX3 I 722 v2.06 xvmsubasp VSX Vector Multiply-Subtract Type-A SP
                    Nyi("xvmsubmsp"),    // 111100 ..... ..... ..... 01011 001... XX3 I 722 v2.06 xvmsubmsp VSX Vector Multiply-Subtract Type-M SP
                    
                    Nyi("xvmaddadp"),    // 111100 ..... ..... ..... 01100 001... XX3 I 705 v2.06 xvmaddadp VSX Vector Multiply-Add Type-A DP
                    Nyi("xvmaddmdp"),    // 111100 ..... ..... ..... 01101 001... XX3 I 705 v2.06 xvmaddmdp VSX Vector Multiply-Add Type-M DP
                    Nyi("xvmsubadp"),    // 111100 ..... ..... ..... 01110 001... XX3 I 719 v2.06 xvmsubadp VSX Vector Multiply-Subtract Type-A DP
                    Nyi("xvmsubmdp"),    // 111100 ..... ..... ..... 01111 001... XX3 I 719 v2.06 xvmsubmdp VSX Vector Multiply-Subtract Type-M DP
                    
                    Nyi("xsnmaddasp"),   // 111100 ..... ..... ..... 10000 001... XX3 I 615 v2.07 xsnmaddasp VSX Scalar Negative Multiply-Add Type-A SP
                    Nyi("xsnmaddmsp"),   // 111100 ..... ..... ..... 10001 001... XX3 I 615 v2.07 xsnmaddmsp VSX Scalar Negative Multiply-Add Type-M SP
                    Nyi("xsnmsubasp"),   // 111100 ..... ..... ..... 10010 001... XX3 I 624 v2.07 xsnmsubasp VSX Scalar Negative Multiply-Subtract Type-A SP
                    Nyi("xsnmsubmsp"),   // 111100 ..... ..... ..... 10011 001... XX3 I 624 v2.07 xsnmsubmsp VSX Scalar Negative Multiply-Subtract Type-M SP
                    
                    Nyi("xsnmaddadp"),   // 111100 ..... ..... ..... 10100 001... XX3 I 610 v2.06 xsnmaddadp VSX Scalar Negative Multiply-Add Type-A DP
                    Nyi("xsnmaddmdp"),   // 111100 ..... ..... ..... 10101 001... XX3 I 610 v2.06 xsnmaddmdp VSX Scalar Negative Multiply-Add Type-M DP
                    Nyi("xsnmsubadp"),   // 111100 ..... ..... ..... 10110 001... XX3 I 621 v2.06 xsnmsubadp VSX Scalar Negative Multiply-Subtract Type-A DP
                    Instr(Mnemonic.xsnmsubmdp, vsr1, vsr2, vsr3),   // XX3 I 621 v2.06 xsnmsubmdp VSX Scalar Negative Multiply-Subtract Type-M DP
                    
                    Nyi("xvnmaddasp"),   // 111100 ..... ..... ..... 11000 001... XX3 I 736 v2.06 xvnmaddasp VSX Vector Negative Multiply-Add Type-A SP
                    Nyi("xvnmaddmsp"),   // 111100 ..... ..... ..... 11001 001... XX3 I 736 v2.06 xvnmaddmsp VSX Vector Negative Multiply-Add Type-M SP
                    Nyi("xvnmsubasp"),   // 111100 ..... ..... ..... 11010 001... XX3 I 742 v2.06 xvnmsubasp VSX Vector Negative Multiply-Subtract Type-A SP
                    Nyi("xvnmsubmsp"),   // 111100 ..... ..... ..... 11011 001... XX3 I 742 v2.06 xvnmsubmsp VSX Vector Negative Multiply-Subtract Type-M SP
                    
                    Nyi("xvnmaddadp"),   // 111100 ..... ..... ..... 11100 001... XX3 I 731 v2.06 xvnmaddadp VSX Vector Negative Multiply-Add Type-A DP
                    Nyi("xvnmaddmdp"),   // 111100 ..... ..... ..... 11101 001... XX3 I 731 v2.06 xvnmaddmdp VSX Vector Negative Multiply-Add Type-M DP
                    Nyi("xvnmsubadp"),   // 111100 ..... ..... ..... 11110 001... XX3 I 739 v2.06 xvnmsubadp VSX Vector Negative Multiply-Subtract Type-A DP
                    Nyi("xvnmsubmdp")),  // 111100 ..... ..... ..... 11111 001... XX3 I 739 v2.06 xvnmsubmdp VSX Vector Negative Multiply-Subtract Type-M DP
                Mask(21, 5, "  010",
                    xxsldwi,
                    xxpermdi,
                    Nyi("xxmrghw"),                             // 111100 ..... ..... ..... 00010 010... XX3 I 775 v2.06 xxmrghw VSX Vector Merge Word High
                    Nyi("xxperm"),                              // 111100 ..... ..... ..... 00011 010... XX3 I 776 v3.0 xxperm VSX Vector Permute

                    xxsldwi,
                    xxpermdi,
                    Nyi("xxmrglw"),                             // 111100 ..... ..... ..... 00110 010... XX3 I 775 v2.06 xxmrglw VSX Vector Merge Word Low
                    Nyi("xxpermr"),                             // 111100 ..... ..... ..... 00111 010... XX3 I 776 v3.0 xxpermr VSX Vector Permute Right-indexed

                    xxsldwi,
                    xxpermdi,
                    invalid,
                    invalid,

                    xxsldwi,
                    xxpermdi,
                    invalid,
                    invalid,

                    Instr(Mnemonic.xxland, vsr1, vsr2, vsr3),       // 111100 ..... ..... ..... 10000 010... XX3 I 771 v2.06 xxland VSX Vector Logical AND
                    Instr(Mnemonic.xxlandc, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 10001 010... XX3 I 771 v2.06 xxlandc VSX Vector Logical AND with Complement
                    Instr(Mnemonic.xxlor, vsr1, vsr2, vsr3),        // 111100 ..... ..... ..... 10010 010... XX3 I 774 v2.06 xxlor VSX Vector Logical OR
                    Instr(Mnemonic.xxlxor, vsr1, vsr2, vsr3),       // 111100 ..... ..... ..... 10011 010... XX3 I 774 v2.06 xxlxor VSX Vector Logical XOR

                    Instr(Mnemonic.xxlnor, vsr1, vsr2, vsr3),       // 111100 ..... ..... ..... 10100 010... XX3 I 773 v2.06 xxlnor VSX Vector Logical NOR
                    Instr(Mnemonic.xxlorc, vsr1, vsr2, vsr3),       // 111100 ..... ..... ..... 10101 010... XX3 I 773 v2.07 xxlorc VSX Vector Logical OR with Complement
                    Instr(Mnemonic.xxlnand, vsr1, vsr2, vsr3),      // 111100 ..... ..... ..... 10110 010... XX3 I 772 v2.07 xxlnand VSX Vector Logical NAND
                    Instr(Mnemonic.xxleqv, vsr1, vsr2, vsr3),       // 111100 ..... ..... ..... 10111 010... XX3 I 772 v2.07 xxleqv VSX Vector Logical Equivalence

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),

                /*
                Instr(Mnemonic.xxsldwi, v1,v2,v3),          // 111100 ..... ..... ..... 0..00 010... XX3 I 778 v2.06 xxsldwi VSX Vector Shift Left Double by Word Immediate
                Nyi("xxpermdi"),                            // 111100 ..... ..... ..... 0..01 010... XX3 I 777 v2.06 xxpermdi VSX Vector Dword Permute Immediate
                Nyi("xxmrghw"),                             // 111100 ..... ..... ..... 00010 010... XX3 I 775 v2.06 xxmrghw VSX Vector Merge Word High
                Nyi("xxperm"),                              // 111100 ..... ..... ..... 00011 010... XX3 I 776 v3.0 xxperm VSX Vector Permute
                Nyi("xxmrglw"),                             // 111100 ..... ..... ..... 00110 010... XX3 I 775 v2.06 xxmrglw VSX Vector Merge Word Low
                Nyi("xxpermr"),                             // 111100 ..... ..... ..... 00111 010... XX3 I 776 v3.0 xxpermr VSX Vector Permute Right-indexed
                Nyi("xxland"),                              // 111100 ..... ..... ..... 10000 010... XX3 I 771 v2.06 xxland VSX Vector Logical AND
                Nyi("xxlandc"),                             // 111100 ..... ..... ..... 10001 010... XX3 I 771 v2.06 xxlandc VSX Vector Logical AND with Complement
                Nyi("xxlor"),                               // 111100 ..... ..... ..... 10010 010... XX3 I 774 v2.06 xxlor VSX Vector Logical OR
                Nyi("xxlxor"),                              // 111100 ..... ..... ..... 10011 010... XX3 I 774 v2.06 xxlxor VSX Vector Logical XOR
                Nyi("xxlnor"),                              // 111100 ..... ..... ..... 10100 010... XX3 I 773 v2.06 xxlnor VSX Vector Logical NOR
                Nyi("xxlorc"),                              // 111100 ..... ..... ..... 10101 010... XX3 I 773 v2.07 xxlorc VSX Vector Logical OR with Complement
                Nyi("xxlnand"),                             // 111100 ..... ..... ..... 10110 010... XX3 I 772 v2.07 xxlnand VSX Vector Logical NAND
                Nyi("xxleqv"),                              // 111100 ..... ..... ..... 10111 010... XX3 I 772 v2.07 xxleqv VSX Vector Logical Equivalence
                Nyi("xxspltw"),                             // 111100 ..... ///.. ..... 01010 0100.. XX2 I 778 v2.06 xxspltw VSX Vector Splat Word
                Nyi("xxextractuw"),                         // 111100 ..... /.... ..... 01010 0101.. XX2 I 770 v3.0 xxextractuw VSX Vector Extract Unsigned Word
                Nyi("xxspltib"),                            // 111100 ..... 00... ..... 01011 01000. XX1 I 778 v3.0 xxspltib VSX Vector Splat Immediate Byte
                Nyi("xxinsertw"),                           // 111100 ..... /.... ..... 01011 0101.. XX2 I 770 v3.0 xxinsertw VSX Vector Insert Word
                */
                Nyi("Ext3C 011"),
                /*
                Nyi("xvcmpeqsp[.]"),    // 111100 ..... ..... ..... .1000 011... XX3 I 668 v2.06 xvcmpeqsp[.] VSX Vector Compare Equal SP
                Nyi("xvcmpgtsp[.]"),    // 111100 ..... ..... ..... .1001 011... XX3 I 672 v2.06 xvcmpgtsp[.] VSX Vector Compare Greater Than SP
                Nyi("xvcmpgesp[.]"),    // 111100 ..... ..... ..... .1010 011... XX3 I 670 v2.06 xvcmpgesp[.] VSX Vector Compare Greater Than or Equal SP
                Nyi("xvcmpnesp[.]"),    // 111100 ..... ..... ..... .1011 011... XX3 I 674 v3.0 xvcmpnesp[.] VSX Vector Compare Not Equal Single-Precision
                Nyi("xvcmpeqdp[.]"),    // 111100 ..... ..... ..... .1100 011... XX3 I 667 v2.06 xvcmpeqdp[.] VSX Vector Compare Equal DP
                Nyi("xvcmpgtdp[.]"),    // 111100 ..... ..... ..... .1101 011... XX3 I 671 v2.06 xvcmpgtdp[.] VSX Vector Compare Greater Than DP
                Nyi("xvcmpgedp[.]"),    // 111100 ..... ..... ..... .1110 011... XX3 I 669 v2.06 xvcmpgedp[.] VSX Vector Compare Greater Than or Equal DP
                Nyi("xvcmpnedp[.]"),    // 111100 ..... ..... ..... .1111 011... XX3 I 673 v3.0 xvcmpnedp[.] VSX Vector Compare Not Equal Double-Precision
                Nyi("xscmpeqdp"),       // 111100 ..... ..... ..... 00000 011... XX3 I 525 v3.0 xscmpeqdp VSX Scalar Compare Equal Double-Precision
                Nyi("xscmpgtdp"),       // 111100 ..... ..... ..... 00001 011... XX3 I 527 v3.0 xscmpgtdp VSX Scalar Compare Greater Than Double-Precision
                Nyi("xscmpgedp"),       // 111100 ..... ..... ..... 00010 011... XX3 I 526 v3.0 xscmpgedp VSX Scalar Compare Greater Than or Equal Double-Precision
                Nyi("xscmpnedp"),       // 111100 ..... ..... ..... 00011 011... XX3 I 528 v3.0 xscmpnedp VSX Scalar Compare Not Equal Double-Precision
                Nyi("xscmpudp"),        // 111100 ...// ..... ..... 00100 011../ XX3 I 532 v2.06 xscmpudp VSX Scalar Compare Unordered DP
                Nyi("xscmpodp"),        // 111100 ...// ..... ..... 00101 011../ XX3 I 529 v2.06 xscmpodp VSX Scalar Compare Ordered DP
                Nyi("xscmpexpdp"),      // 111100 ...// ..... ..... 00111 011../ XX3 I 523 v3.0 xscmpexpdp VSX Scalar Compare Exponents DP
                */
                Mask(29, 1, "Ext3C 100",
                    Mask(21, 5, "  Ext3C 1000",
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Instr(Mnemonic.xscvdpuxws, xt31_6, xt30_16),    // 111100 ..... ///// ..... 00100 1000.. XX2 I 546 v2.06 xscvdpuxws VSX Scalar Convert DP to Unsigned Word truncate
                        Instr(Mnemonic.xscvdpsxws, xt31_6, xt30_16),    // 111100 ..... ///// ..... 00101 1000.. XX2 I 542 v2.06 xscvdpsxws VSX Scalar Convert DP to Signed Word truncate
                        invalid,
                        invalid,

                        Nyi("xvcvspuxws"),      // 111100 ..... ///// ..... 01000 1000.. XX2 I 694 v2.06 xvcvspuxws VSX Vector Convert SP to Unsigned Word truncate
                        Nyi("xvcvspsxws"),      // 111100 ..... ///// ..... 01001 1000.. XX2 I 690 v2.06 xvcvspsxws VSX Vector Convert SP to Signed Word truncate
                        Nyi("xvcvuxwsp"),       // 111100 ..... ///// ..... 01010 1000.. XX2 I 699 v2.06 xvcvuxwsp VSX Vector Convert Unsigned Word to SP
                        Nyi("xvcvsxwsp"),       // 111100 ..... ///// ..... 01011 1000.. XX2 I 697 v2.06 xvcvsxwsp VSX Vector Convert Signed Word to SP

                        Nyi("xvcvdpuxws"),      // 111100 ..... ///// ..... 01100 1000.. XX2 I 683 v2.06 xvcvdpuxws VSX Vector Convert DP to Unsigned Word truncate
                        Instr(Mnemonic.xvcvdpsxws, xt31_6, xt30_16),      // 111100 ..... ///// ..... 01101 1000.. XX2 I 679 v2.06 xvcvdpsxws VSX Vector Convert DP to Signed Word truncate
                        Nyi("xvcvuxwdp"),       // 111100 ..... ///// ..... 01110 1000.. XX2 I 699 v2.06 xvcvuxwdp VSX Vector Convert Unsigned Word to DP
                        Nyi("xvcvsxwdp"),       // 111100 ..... ///// ..... 01111 1000.. XX2 I 697 v2.06 xvcvsxwdp VSX Vector Convert Signed Word to DP
                        
                        invalid,
                        invalid,
                        Nyi("xscvuxdsp"),       // 111100 ..... ///// ..... 10010 1000.. XX2 I 563 v2.07 xscvuxdsp VSX Scalar Convert Unsigned Dword to SP
                        Nyi("xscvsxdsp"),       // 111100 ..... ///// ..... 10011 1000.. XX2 I 561 v2.07 xscvsxdsp VSX Scalar Convert Signed Dword to SP
                       
                        Nyi("xscvdpuxds"),      // 111100 ..... ///// ..... 10100 1000.. XX2 I 544 v2.06 xscvdpuxds VSX Scalar Convert DP to Unsigned Dword truncate
                        Nyi("xscvdpsxds"),      // 111100 ..... ///// ..... 10101 1000.. XX2 I 539 v2.06 xscvdpsxds VSX Scalar Convert DP to Signed Dword truncate
                        Instr(Mnemonic.xscvuxddp, xt31_6, xt30_16),         // 111100 ..... ///// ..... 10110 1000.. XX2 I 563 v2.06 xscvuxddp VSX Scalar Convert Unsigned Dword to DP
                        Nyi("xscvsxddp"),       // 111100 ..... ///// ..... 10111 1000.. XX2 I 561 v2.06 xscvsxddp VSX Scalar Convert Signed Dword to DP
                        
                        Nyi("xvcvspuxds"),      // 111100 ..... ///// ..... 11000 1000.. XX2 I 692 v2.06 xvcvspuxds VSX Vector Convert SP to Unsigned Dword truncate
                        Nyi("xvcvspsxds"),      // 111100 ..... ///// ..... 11001 1000.. XX2 I 688 v2.06 xvcvspsxds VSX Vector Convert SP to Signed Dword truncate
                        Instr(Mnemonic.xvcvuxdsp, vsr1, vsr2),          // 111100 ..... ///// ..... 11010 1000.. XX2 I 698 v2.06 xvcvuxdsp VSX Vector Convert Unsigned Dword to SP
                        Nyi("xvcvsxdsp"),       // 111100 ..... ///// ..... 11011 1000.. XX2 I 696 v2.06 xvcvsxdsp VSX Vector Convert Signed Dword to SP
                        
                        Nyi("xvcvdpuxds"),      // 111100 ..... ///// ..... 11100 1000.. XX2 I 681 v2.06 xvcvdpuxds VSX Vector Convert DP to Unsigned Dword truncate
                        Nyi("xvcvdpsxds"),      // 111100 ..... ///// ..... 11101 1000.. XX2 I 677 v2.06 xvcvdpsxds VSX Vector Convert DP to Signed Dword truncate
                        Nyi("xvcvuxddp"),       // 111100 ..... ///// ..... 11110 1000.. XX2 I 698 v2.06 xvcvuxddp VSX Vector Convert Unsigned Dword to DP
                        Nyi("xvcvsxddp")),       // 111100 ..... ///// ..... 11111 1000.. XX2 I 696 v2.06 xvcvsxddp VSX Vector Convert Signed Dword to DP
                    Mask(21, 5, "  Ext3C 1000",
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Nyi("xsrdpi"),          // 111100 ..... ///// ..... 00100 1001.. XX2 I 630 v2.06 xsrdpi VSX Scalar Round DP to Integral to Nearest Away
                        Nyi("xsrdpiz"),         // 111100 ..... ///// ..... 00101 1001.. XX2 I 633 v2.06 xsrdpiz VSX Scalar Round DP to Integral toward Zero
                        Instr(Mnemonic.xsrdpip, vsr1, vsr3),    // 111100 ..... ///// ..... 00110 1001.. XX2 I 632 v2.06 xsrdpip VSX Scalar Round DP to Integral toward +Infinity
                        Nyi("xsrdpim"),         // 111100 ..... ///// ..... 00111 1001.. XX2 I 632 v2.06 xsrdpim VSX Scalar Round DP to Integral toward -Infinity

                        Nyi("xvrspi"),          // 111100 ..... ///// ..... 01000 1001.. XX2 I 750 v2.06 xvrspi VSX Vector Round SP to Integral to Nearest Away
                        Nyi("xvrspiz"),         // 111100 ..... ///// ..... 01001 1001.. XX2 I 752 v2.06 xvrspiz VSX Vector Round SP to Integral toward Zero
                        Nyi("xvrspip"),         // 111100 ..... ///// ..... 01010 1001.. XX2 I 751 v2.06 xvrspip VSX Vector Round SP to Integral toward +Infinity
                        Nyi("xvrspim"),         // 111100 ..... ///// ..... 01011 1001.. XX2 I 751 v2.06 xvrspim VSX Vector Round SP to Integral toward -Infinity

                        Nyi("xvrdpi"),          // 111100 ..... ///// ..... 01100 1001.. XX2 I 745 v2.06 xvrdpi VSX Vector Round DP to Integral to Nearest Away
                        Nyi("xvrdpiz"),         // 111100 ..... ///// ..... 01101 1001.. XX2 I 747 v2.06 xvrdpiz VSX Vector Round DP to Integral toward Zero
                        Nyi("xvrdpip"),         // 111100 ..... ///// ..... 01110 1001.. XX2 I 746 v2.06 xvrdpip VSX Vector Round DP to Integral toward +Infinity
                        Nyi("xvrdpim"),         // 111100 ..... ///// ..... 01111 1001.. XX2 I 746 v2.06 xvrdpim VSX Vector Round DP to Integral toward -Infinity

                        Nyi("xscvdpsp"),        // 111100 ..... ///// ..... 10000 1001.. XX2 I 538 v2.06 xscvdpsp VSX Scalar Convert DP to SP
                        Nyi("xsrsp"),           // 111100 ..... ///// ..... 10001 1001.. XX2 I 640 v2.07 xsrsp VSX Scalar Round DP to SP
                        invalid,
                        invalid,

                        Nyi("xscvspdp"),        // 111100 ..... ///// ..... 10100 1001.. XX2 I 559 v2.06 xscvspdp VSX Scalar Convert SP to DP
                        Nyi("xsabsdp"),         // 111100 ..... ///// ..... 10101 1001.. XX2 I 513 v2.06 xsabsdp VSX Scalar Absolute DP
                        Nyi("xsnabsdp"),        // 111100 ..... ///// ..... 10110 1001.. XX2 I 608 v2.06 xsnabsdp VSX Scalar Negative Absolute DP
                        Instr(Mnemonic.xsnegdp, xt31_6, xt30_16),   // 111100 ..... ///// ..... 10111 1001.. XX2 I 609 v2.06 xsnegdp VSX Scalar Negate DP

                        Nyi("xvcvdpsp"),        // 111100 ..... ///// ..... 11000 1001.. XX2 I 676 v2.06 xvcvdpsp VSX Vector Convert DP to SP
                        Nyi("xvabssp"),         // 111100 ..... ///// ..... 11001 1001.. XX2 I 660 v2.06 xvabssp VSX Vector Absolute SP
                        Nyi("xvnabssp"),        // 111100 ..... ///// ..... 11010 1001.. XX2 I 729 v2.06 xvnabssp VSX Vector Negative Absolute SP
                        Nyi("xvnegsp"),         // 111100 ..... ///// ..... 11011 1001.. XX2 I 730 v2.06 xvnegsp VSX Vector Negate SP

                        Nyi("xvcvspdp"),        // 111100 ..... ///// ..... 11100 1001.. XX2 I 686 v2.06 xvcvspdp VSX Vector Convert SP to DP
                        Nyi("xvabsdp"),         // 111100 ..... ///// ..... 11101 1001.. XX2 I 660 v2.06 xvabsdp VSX Vector Absolute DP
                        Nyi("xvnabsdp"),        // 111100 ..... ///// ..... 11110 1001.. XX2 I 729 v2.06 xvnabsdp VSX Vector Negative Absolute DP
                        Nyi("xvnegdp")          // 111100 ..... ///// ..... 11111 1001.. XX2 I 730 v2.06 xvnegdp VSX Vector Negate DP
                        )),
                Mask(29, 1, "  Ext3C 101",
                    Mask(21, 5, "  Ext3C 1010",
                        Nyi("xsrsqrtesp"),      // 111100 ..... ///// ..... 00000 1010.. XX2 I 642 v2.07 xsrsqrtesp VSX Scalar Reciprocal Square Root Estimate SP
                        Nyi("xsresp"),          // 111100 ..... ///// ..... 00001 1010.. XX2 I 635 v2.07 xsresp VSX Scalar Reciprocal Estimate SP
                        invalid,
                        invalid,

                        Nyi("xsrsqrtedp"),      // 111100 ..... ///// ..... 00100 1010.. XX2 I 641 v2.06 xsrsqrtedp VSX Scalar Reciprocal Square Root Estimate DP
                        Instr(Mnemonic.xsredp, xt31_6, xt30_16),            // 111100 ..... ///// ..... 00101 1010.. XX2 I 634 v2.06 xsredp VSX Scalar Reciprocal Estimate DP
                        Instr(Mnemonic.xstsqrtdp, u23_3, xt30_16),          // 111100 ...// ///// ..... 00110 1010./ XX2 I 654 v2.06 xstsqrtdp VSX Scalar Test for software Square Root DP
                        Instr(Mnemonic.xstdivdp, u23_3,xt29_11,xt30_16),    // 111100 ...// ..... ..... 00111 101../ XX3 I 653 v2.06 xstdivdp VSX Scalar Test for software Divide DP
                        
                        Nyi("xvrsqrtesp"),      // 111100 ..... ///// ..... 01000 1010.. XX2 I 754 v2.06 xvrsqrtesp VSX Vector Reciprocal Square Root Estimate SP
                        Nyi("xvresp"),          // 111100 ..... ///// ..... 01001 1010.. XX2 I 749 v2.06 xvresp VSX Vector Reciprocal Estimate SP
                        Nyi("xvtsqrtsp"),       // 111100 ...// ///// ..... 01010 1010./ XX2 I 763 v2.06 xvtsqrtsp VSX Vector Test for software Square Root SP
                        Nyi("xvtdivsp"),        // 111100 ...// ..... ..... 01011 101../ XX3 I 762 v2.06 xvtdivsp VSX Vector Test for software Divide SP
                        
                        Nyi("xvrsqrtedp"),      // 111100 ..... ///// ..... 01100 1010.. XX2 I 752 v2.06 xvrsqrtedp VSX Vector Reciprocal Square Root Estimate DP
                        Nyi("xvredp"),          // 111100 ..... ///// ..... 01101 1010.. XX2 I 748 v2.06 xvredp VSX Vector Reciprocal Estimate DP
                        Nyi("xvtsqrtdp"),       // 111100 ...// ///// ..... 01110 1010./ XX2 I 763 v2.06 xvtsqrtdp VSX Vector Test for software Square Root DP
                        Nyi("xvtdivdp"),        // 111100 ...// ..... ..... 01111 101../ XX3 I 761 v2.06 xvtdivdp VSX Vector Test for software Divide DP
                        
                        invalid,
                        invalid,
                        Nyi("xststdcsp"),       // 111100 ..... ..... ..... 10010 1010./ XX2 I 657 v3.0 xststdcsp VSX Scalar Test Data Class SP
                        invalid,

                        invalid,
                        invalid,
                        Nyi("xststdcdp"),       // 111100 ..... ..... ..... 10110 1010./ XX2 I 655 v3.0 xststdcdp VSX Scalar Test Data Class DP
                        invalid,
                        
                        invalid,
                        invalid,
                        Nyi("xvtstdcsp"),       // 111100 ..... ..... ..... 11010 101... XX2 I 765 v3.0 xvtstdcsp VSX Vector Test Data Class SP
                        Nyi("xvtstdcsp"),       // 111100 ..... ..... ..... 11011 101... XX2 I 765 v3.0 xvtstdcsp VSX Vector Test Data Class SP

                        invalid,
                        invalid,
                        Nyi("xvtstdcdp") ,      // 111100 ..... ..... ..... 11110 101... XX2 I 764 v3.0 xvtstdcdp VSX Vector Test Data Class DP
                        Nyi("xvtstdcdp")),      // 111100 ..... ..... ..... 11111 101... XX2 I 764 v3.0 xvtstdcdp VSX Vector Test Data Class DP
                    Mask(21, 5, "  Ext3C 1011",
                        Nyi("xssqrtsp"),        // 111100 ..... ///// ..... 00000 1011.. XX2 I 646 v2.07 xssqrtsp VSX Scalar Square Root SP
                        invalid,
                        invalid,
                        invalid,

                        Nyi("xssqrtdp"),        // 111100 ..... ///// ..... 00100 1011.. XX2 I 643 v2.06 xssqrtdp VSX Scalar Square Root DP
                        invalid,
                        Nyi("xsrdpic"),         // 111100 ..... ///// ..... 00110 1011.. XX2 I 631 v2.06 xsrdpic VSX Scalar Round DP to Integral using Current rounding mode
                        Nyi("xstdivdp"),        // 111100 ...// ..... ..... 00111 101../ XX3 I 653 v2.06 xstdivdp VSX Scalar Test for software Divide DP
                        
                        Nyi("xvsqrtsp"),        // 111100 ..... ///// ..... 01000 1011.. XX2 I 756 v2.06 xvsqrtsp VSX Vector Square Root SP
                        invalid,
                        Nyi("xvrspic"),         // 111100 ..... ///// ..... 01010 1011.. XX2 I 750 v2.06 xvrspic VSX Vector Round SP to Integral using Current rounding mode
                        Nyi("xvtdivsp"),        // 111100 ...// ..... ..... 01011 101../ XX3 I 762 v2.06 xvtdivsp VSX Vector Test for software Divide SP
                        
                        Nyi("xvsqrtdp"),        // 111100 ..... ///// ..... 01100 1011.. XX2 I 755 v2.06 xvsqrtdp VSX Vector Square Root DP
                        invalid,
                        Nyi("xvrdpic"),         // 111100 ..... ///// ..... 01110 1011.. XX2 I 745 v2.06 xvrdpic VSX Vector Round DP to Integral using Current rounding mode
                        Nyi("xvtdivdp"),        // 111100 ...// ..... ..... 01111 101../ XX3 I 761 v2.06 xvtdivdp VSX Vector Test for software Divide DP
                        
                        Nyi("xscvdpspn"),       // 111100 ..... ///// ..... 10000 1011.. XX2 I 539 v2.07 xscvdpspn VSX Scalar Convert DP to SP Non-signalling
                        invalid,
                        invalid,
                        invalid,
                        
                        Nyi("xscvspdpn"),       // 111100 ..... ///// ..... 10100 1011.. XX2 I 560 v2.07 xscvspdpn VSX Scalar Convert SP to DP Non-signalling
                        Sparse(11, 5, invalid, "  Ext 101 - 10101",
                            (0b00000,  Nyi("xsxexpdp")),        // 111100 ..... 00000 ..... 10101 1011./ XX2 I 658 v3.0 xsxexpdp VSX Scalar Extract Exponent DP
                            (0b00001,  Nyi("xsxsigdp")),        // 111100 ..... 00001 ..... 10101 1011./ XX2 I 659 v3.0 xsxsigdp VSX Scalar Extract Significand DP
                            (0b10000,  Nyi("xscvhpdp")),        // 111100 ..... 10000 ..... 10101 1011.. XX2 I 548 v3.0 xscvhpdp VSX Scalar Convert HP to DP
                            (0b10001,  Nyi("xscvdphp"))),       // 111100 ..... 10001 ..... 10101 1011.. XX2 I 536 v3.0 xscvdphp VSX Scalar Convert DP to HP
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        Nyi("xvtstdcsp"),       // 111100 ..... ..... ..... 11010 101... XX2 I 765 v3.0 xvtstdcsp VSX Vector Test Data Class SP
                        Nyi("xvtstdcsp"),       // 111100 ..... ..... ..... 11011 101... XX2 I 765 v3.0 xvtstdcsp VSX Vector Test Data Class SP
                        
                        Instr(Mnemonic.xsiexpdp, xt31_6,r2,r3),  // 111100 ..... ..... ..... 11100 10110. XX1 I 570 v3.0 xsiexpdp VSX Scalar Insert Exponent DP
                        Sparse(11, 5, invalid, "  Ext 101 - 11101",
                            (0b00000, Nyi("xvxexpdp")),        // 111100 ..... 00000 ..... 11101 1011.. XX2 I 766 v3.0 xvxexpdp VSX Vector Extract Exponent DP
                            (0b00001, Nyi("xvxsigdp")),        // 111100 ..... 00001 ..... 11101 1011.. XX2 I 767 v3.0 xvxsigdp VSX Vector Extract Significand DP
                            (0b00111, Nyi("xxbrh")),           // 111100 ..... 00111 ..... 11101 1011.. XX2 I 768 v3.0 xxbrh VSX Vector Byte-Reverse Hword
                            (0b01000, Nyi("xvxexpsp")),        // 111100 ..... 01000 ..... 11101 1011.. XX2 I 766 v3.0 xvxexpsp VSX Vector Extract Exponent SP
                            (0b01001, Nyi("xvxsigsp")),        // 111100 ..... 01001 ..... 11101 1011.. XX2 I 767 v3.0 xvxsigsp VSX Vector Extract Significand SP
                            (0b01111, Nyi("xxbrw")),           // 111100 ..... 01111 ..... 11101 1011.. XX2 I 769 v3.0 xxbrw VSX Vector Byte-Reverse Word
                            (0b10111, Nyi("xxbrd")),           // 111100 ..... 10111 ..... 11101 1011.. XX2 I 768 v3.0 xxbrd VSX Vector Byte-Reverse Dword
                            (0b11000, Nyi("xvcvhpsp")),        // 111100 ..... 11000 ..... 11101 1011.. XX2 I 685 v3.0 xvcvhpsp VSX Vector Convert HP to SP
                            (0b11001, Nyi("xvcvsphp")),        // 111100 ..... 11001 ..... 11101 1011.. XX2 I 687 v3.0 xvcvsphp VSX Vector Convert SP to HP
                            (0b11111, Nyi("xxbrq"))),          // 111100 ..... 11111 ..... 11101 1011.. XX2 I 769 v3.0 xxbrq VSX Vector Byte-Reverse Qword
                        Nyi("xvtstdcdp"),                      // 111100 ..... ..... ..... 11110 101... XX2 I 764 v3.0 xvtstdcdp VSX Vector Test Data Class DP
                        Nyi("xvtstdcdp"))),                     // 111100 ..... ..... ..... 11111 101... XX2 I 764 v3.0 xvtstdcdp VSX Vector Test Data Class DP
                Instr(Mnemonic.xxsel, vsr1,vsr2,vsr3,vsr4),     // 111100 ..... ..... ..... ..... 11.... XX4 I 777 v2.06 xxsel VSX Vector Select
                Instr(Mnemonic.xxsel, vsr1, vsr2, vsr3, vsr4)); // 111100 ..... ..... ..... ..... 11.... XX4 I 777 v2.06 xxsel VSX Vector Select
            return decoder;

        }

        public virtual Decoder Ext3DDecoder()
        {
            var decoder = Mask(29, 3, "  Ext3D",
                Instr(Mnemonic.stfdp, f1p, E2_2),        // 111101 ..... ..... ..... ..... ....00 DS I 150 v2.05 stfdp Store Floating Double Pair
                Instr(Mnemonic.lxv, vsr(21,3), DQ_4),   // 111101 ..... ..... ..... ..... ...001 DQ I 493 v3.0 lxv Load VSX Vector
                Instr(Mnemonic.stxsd, f1p, DS_2),        // 111101 ..... ..... ..... ..... ....10 DS I 499 v3.0 stxsd Store VSX Scalar Dword
                Instr(Mnemonic.stxssp, f1p, DS_2),       // 111101 ..... ..... ..... ..... ....11 DS I 502 v3.0 stxssp Store VSX Scalar SP
                Instr(Mnemonic.stfdp, f1p, E2_2),        // 111101 ..... ..... ..... ..... ....00 DS I 150 v2.05 stfdp Store Floating Double Pair
                Instr(Mnemonic.stxv, f1p, DS_2),         // 111101 ..... ..... ..... ..... ...101 DQ I 508 v3.0 stxv Store VSX Vector
                Instr(Mnemonic.stxsd, f1p, DS_2),        // 111101 ..... ..... ..... ..... ....10 DS I 499 v3.0 stxsd Store VSX Scalar Dword
                Instr(Mnemonic.stxsd, f1p, DS_2));       // 111101 ..... ..... ..... ..... ....11 DS I 502 v3.0 stxssp Store VSX Scalar SP
            return decoder;
        }

        public virtual Decoder Ext3FDecoder()
        {
            var decoder = Mask(26, 5, "  Ext3F",
                Sparse(21, 5, invalid, "  00000",
                    ( 0b00000, Instr(Mnemonic.fcmpu, C1,f2,f3)),        // 111111 ...// ..... ..... 00000 00000/ X I 168 P1 fcmpu Floating Compare Unordered
                    ( 0b00001, Instr(Mnemonic.fcmpo, C1,f2,f3)),        // 111111 ...// ..... ..... 00001 00000/ X I 168 P1 fcmpo Floating Compare Ordered
                    ( 0b00010, Instr(Mnemonic.mcrfs, u22_3, u18_3)),    // 111111 ...// ...// ///// 00010 00000/ X I 171 P1 mcrfs Move To CR from FPSCR
                    ( 0b00100, Instr(Mnemonic.ftdiv, C1,f2,f3)),        // 111111 ...// ..... ..... 00100 00000/ X I 157 v2.06 ftdiv Floating Test for software Divide
                    ( 0b00101, Instr(Mnemonic.ftsqrt, C1,f3))),         // 111111 ...// ///// ..... 00101 00000/ X I 157 v2.06 ftsqrt Floating Test for software Square Root
                invalid,
                Sparse(21, 5, invalid, "  00010",
                    ( 0b00000, Instr(Mnemonic.daddq, C,f1p,f2p,f3p)),          // X I 195 v2.05 daddq[.] DFP Add Quad
                    ( 0b00001, Instr(Mnemonic.dmulq, C,f1p,f2p,f3p)),          // X I 197 v2.05 dmulq[.] DFP Multiply Quad
                    ( 0b00010, Nyi("dscliq[.]")),   // 111111 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscliq[.] DFP Shift Significand Left Immediate Quad
                    ( 0b00011, Instr(Mnemonic.dscriq, C,f1p,f2p,u10_6)),      // Z22 I 222 v2.05 dscriq[.] DFP Shift Significand Right Immediate Quad
                        
                    ( 0b00100, Instr(Mnemonic.dcmpoq, C1,f2p,f3p)),           // X I 201 v2.05 dcmpoq DFP Compare Ordered Quad
                    ( 0b00101, Instr(Mnemonic.dtstexq, C1,f2p,f3p)),          // X I 203 v2.05 dtstexq DFP Test Exponent Quad
                    ( 0b00110, Nyi("dtstdcq")),     // 111111 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdcq DFP Test Data Class Quad
                    ( 0b00111, Instr(Mnemonic.dtstdgq, u23_3, f2p,u10_6)),     // 111111 ...// ..... ..... .0111 00010/ Z22 I 202 v2.05 dtstdgq DFP Test Data Group Quad

                    ( 0b01000, Instr(Mnemonic.dctqpq, C, f1p, f3p)),          // X I 215 v2.05 dctqpq[.] DFP Convert To DFP Extended
                    ( 0b01001, Instr(Mnemonic.dctfixq, C,f1p,f3p)),           // X I 217 v2.05 dctfixq[.] DFP Convert To Fixed Quad
                    ( 0b01010, Nyi("ddedpdq[.]")),  // 111111 ..... ../// ..... 01010 00010. X I 219 v2.05 ddedpdq[.] DFP Decode DPD To BCD Quad
                    ( 0b01011, Instr(Mnemonic.dxexq, C,f1p,f3p)),             // 111111 ..... ///// ..... 01011 00010. X I 220 v2.05 dxexq[.] DFP Extract Exponent Quad

                    ( 0b10000, Instr(Mnemonic.dsubq, C,f1p,f2p,f3p)),          // X I 195 v2.05 dsubq[.] DFP Subtract Quad
                    ( 0b10001, Nyi("ddivq[.]")),    // 111111 ..... ..... ..... 10001 00010. X I 198 v2.05 ddivq[.] DFP Divide Quad
                    ( 0b10010, Nyi("dscliq[.]")),   // 111111 ..... ..... ..... .0010 00010. Z22 I 222 v2.05 dscliq[.] DFP Shift Significand Left Immediate Quad
                    ( 0b10011, Instr(Mnemonic.dscriq, C,f1p,f2p,u10_6)),      // Z22 I 222 v2.05 dscriq[.] DFP Shift Significand Right Immediate Quad

                    ( 0b10100, Nyi("dcmpuq")),      // 111111 ...// ..... ..... 10100 00010/ X I 200 v2.05 dcmpuq DFP Compare Unordered Quad
                    ( 0b10101, Nyi("dtstsfq")),     // 111111 ...// ..... ..... 10101 00010/ X I 204 v2.05 dtstsfq DFP Test Significance Quad
                    ( 0b10110, Nyi("dtstdcq")),     // 111111 ...// ..... ..... .0110 00010/ Z22 I 202 v2.05 dtstdcq DFP Test Data Class Quad
                    ( 0b10111, Instr(Mnemonic.dtstdgq, u23_3, f2p, u10_6)),  // Z22 I 202 v2.05 dtstdgq DFP Test Data Group Quad

                    ( 0b11000, Nyi("drdpq[.]")),    // 111111 ..... ///// ..... 11000 00010. X I 216 v2.05 drdpq[.] DFP Round To DFP Long
                    ( 0b11001, Instr(Mnemonic.dcffixq, C,f1p,f3p)),           // 111111 ..... ///// ..... 11001 00010. X I 217 v2.05 dcffixq[.] DFP Convert From Fixed Quad
                    ( 0b11010, Nyi("denbcdq[.]")),  // 111111 ..... .//// ..... 11010 00010. X I 219 v2.05 denbcdq[.] DFP Encode BCD To DPD Quad
                    ( 0b11011, Nyi("diexq[.]"))),   // 111111 ..... ..... ..... 11011 00010. X I 220 v2.05 diexq[.] DFP Insert Exponent Quad
                Mask(23, 3, "  00011",
                    Instr(Mnemonic.dquaq, C, f1p,f2p,f3p,u9_2),                 // 111111 ..... ..... ..... ..000 00011. Z23 I 206 v2.05 dquaq[.] DFP Quantize Quad
                    Instr(Mnemonic.drrndq, C, f1p,f2p,f3p,u9_2),                // Z23 I 208 v2.05 drrndq[.] DFP Reround Quad
                    Instr(Mnemonic.dquaiq, C, u16_5,f1p,f3p,u9_2),              // Z23 I 205 v2.05 dquaiq[.] DFP Quantize Immediate Quad
                    Instr(Mnemonic.drintxq, C, u16_1,f1p,f3p,u9_2),             // Z23 I 211 v2.05 drintxq[.] DFP Round To FP Integer With Inexact Quad
                        
                    invalid,
                    Instr(Mnemonic.dtstsfiq, C1,u16_6,f3p),                     // 111111 ...// ..... ..... 10101 00011/ X I 204 v3.0 dtstsfiq DFP Test Significance Immediate Quad
                    invalid,
                    Instr(Mnemonic.drintnq,f1p,f3p,C)),                         // 111111 ..... ////. ..... ..111 00011. Z23 I 213 v2.05 drintnq[.] DFP Round To FP Integer Without Inexact Quad
                Mask(21, 5, "  00100",
                    Nyi("xsaddqp[o]"),      // 111111 ..... ..... ..... 00000 00100. X I 521 v3.0 xsaddqp[o] VSX Scalar Add QP
                    Nyi("xsmulqp[o]"),      // 111111 ..... ..... ..... 00001 00100. X I 604 v3.0 xsmulqp[o] VSX Scalar Multiply QP
                    invalid,
                    Instr(Mnemonic.xscpsgnqp, v1,v2,v3),            // X I 535 v3.0 xscpsgnqp VSX Scalar Copy Sign QP
            
                    Instr(Mnemonic.xscmpoqp, u23_3,v2,v3),          // X I 531 v3.0 xscmpoqp VSX Scalar Compare Ordered QP
                    Instr(Mnemonic.xscmpexpqp, u23_3,v2,v3),        // X I 524 v3.0 xscmpexpqp VSX Scalar Compare Exponents QP
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Mask(31, 1, "  xsmaddqp[o]",
                        Instr(Mnemonic.xsmaddqp, v1, v2, v3),       // X I 578 v3.0 xsmaddqp[o] VSX Scalar Multiply-Add QP
                        Instr(Mnemonic.xsmaddqpo, v1, v2, v3)),     // X I 578 v3.0 xsmaddqp[o] VSX Scalar Multiply-Add QP
                    Mask(31, 1, "  xsmsubqp[o]",
                        Instr(Mnemonic.xsmsubqp, v1, v2, v3),       // X I 599 v3.0 xsmsubqp[o] VSX Scalar Multiply-Subtract QP
                        Instr(Mnemonic.xsmsubqpo, v1, v2, v3)),     // X I 599 v3.0 xsmsubqp[o] VSX Scalar Multiply-Subtract QP
                    Nyi("xsnmaddqp[o]"),    // 111111 ..... ..... ..... 01110 00100. X I 618 v3.0 xsnmaddqp[o] VSX Scalar Negative Multiply-Add QP
                    Nyi("xsnmsubqp[o]"),    // 111111 ..... ..... ..... 01111 00100. X I 627 v3.0 xsnmsubqp[o] VSX Scalar Negative Multiply-Subtract QP
            
                    Nyi("xssubqp[o]"),      // 111111 ..... ..... ..... 10000 00100. X I 649 v3.0 xssubqp[o] VSX Scalar Subtract QP
                    Mask(31, 1, "  xsdivqp[o]",
                        Instr(Mnemonic.xsdivqp, v1, v2, v3),        // X I 566 v3.0 xsdivqp[o] VSX Scalar Divide QP
                        Instr(Mnemonic.xsdivqpo, v1, v2, v3)),      // X I 566 v3.0 xsdivqp[o] VSX Scalar Divide QP
                    invalid,
                    invalid,
            
                    Nyi("xscmpuqp"),        // 111111 ...// ..... ..... 10100 00100/ X I 534 v3.0 xscmpuqp VSX Scalar Compare Unordered QP
                    invalid,
                    Instr(Mnemonic.xststdcqp, u23_3,vsr3, u16_7),       // 111111 ..... ..... ..... 10110 00100/ X I 656 v3.0 xststdcqp VSX Scalar Test Data Class QP
                    invalid,
            
                    invalid,
                    Sparse(11, 5, invalid,  "  11001",
                        ( 0b00000, Nyi("xsabsqp")),         // 111111 ..... 00000 ..... 11001 00100/ X I 513 v3.0 xsabsqp VSX Scalar Absolute QP
                        ( 0b00010, Nyi("xsxexpqp")),        // 111111 ..... 00010 ..... 11001 00100/ X I 658 v3.0 xsxexpqp VSX Scalar Extract Exponent QP
                        ( 0b01000, Nyi("xsnabsqp")),        // 111111 ..... 01000 ..... 11001 00100/ X I 608 v3.0 xsnabsqp VSX Scalar Negative Absolute QP
                        ( 0b10000, Nyi("xsnegqp")),         // 111111 ..... 10000 ..... 11001 00100/ X I 609 v3.0 xsnegqp VSX Scalar Negate QP
                        ( 0b10010, Nyi("xsxsigqp")),        // 111111 ..... 10010 ..... 11001 00100/ X I 659 v3.0 xsxsigqp VSX Scalar Extract Significand QP
                        ( 0b11011, Nyi("xssqrtqp[o]"))),    // 111111 ..... 11011 ..... 11001 00100. X I 644 v3.0 xssqrtqp[o] VSX Scalar Square Root QP
                    Sparse(11, 5, invalid, "  11010",
                        ( 0b00001, Nyi("xscvqpuwz")),       // 111111 ..... 00001 ..... 11010 00100/ X I 556 v3.0 xscvqpuwz VSX Scalar Convert QP to Unsigned Word truncate
                        ( 0b00010, Nyi("xscvudqp")),        // 111111 ..... 00010 ..... 11010 00100/ X I 562 v3.0 xscvudqp VSX Scalar Convert Unsigned Dword to QP
                        ( 0b01001, Nyi("xscvqpswz")),       // 111111 ..... 01001 ..... 11010 00100/ X I 552 v3.0 xscvqpswz VSX Scalar Convert QP to Signed Word truncate
                        ( 0b01010, Nyi("xscvsdqp")),        // 111111 ..... 01010 ..... 11010 00100/ X I 558 v3.0 xscvsdqp VSX Scalar Convert Signed Dword to QP
                        ( 0b10001, Nyi("xscvqpudz")),       // 111111 ..... 10001 ..... 11010 00100/ X I 554 v3.0 xscvqpudz VSX Scalar Convert QP to Unsigned Dword truncate
                        ( 0b10100, Nyi("xscvqpdp[o]")),     // 111111 ..... 10100 ..... 11010 00100. X I 549 v3.0 xscvqpdp[o] VSX Scalar Convert QP to DP
                        ( 0b10110, Nyi("xscvdpqp")),        // 111111 ..... 10110 ..... 11010 00100/ X I 537 v3.0 xscvdpqp VSX Scalar Convert DP to QP
                        ( 0b11001, Nyi("xscvqpsdz"))),      // 111111 ..... 11001 ..... 11010 00100/ X I 550 v3.0 xscvqpsdz VSX Scalar Convert QP to Signed Dword truncate

                    Nyi("xsiexpqp"),                        // 111111 ..... ..... ..... 11011 00100/ X I 571 v3.0 xsiexpqp VSX Scalar Insert Exponent QP
            
                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(23, 3, "  00101",
                    Mask(0, 1, 
                        Instr(Mnemonic.xsrqpi, u16_1, v1,v3, u9_2),     // Z23 I 636 v3.0 xsrqpi[x] VSX Scalar Round QP to Integral
                        Instr(Mnemonic.xsrqpix, u16_1, v1,v3, u9_2)),   // Z23 I 636 v3.0 xsrqpi[x] VSX Scalar Round QP to Integral
                    Instr(Mnemonic.xsrqpxp, u16_1, v1,v3,u9_2),         // Z23 I 638 v3.0 xsrqpxp VSX Scalar Round QP to XP
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),

                Sparse(21, 5, invalid, "  00110",
                    ( 0b00001, Instr(Mnemonic.mtfsb1,C,u22_5)),             // X I 173 P1 mtfsb1[.] Move To FPSCR Bit 1
                    ( 0b00010, Instr(Mnemonic.mtfsb0,C,u22_5)),             // X I 173 P1 mtfsb0[.] Move To FPSCR Bit 0
                    ( 0b00100, Instr(Mnemonic.mtfsfi,C,u22_3,u12_4,u16_1)), // X I 172 P1 mtfsfi[.] Move To FPSCR Field Immediate
                    ( 0b11010, Nyi("fmrgow")),                              // X I 152 v2.07 fmrgow Floating Merge Odd Word
                    ( 0b11110, Instr(Mnemonic.fmrgew,f1,f2,f3))),           // X I 151 v2.07 fmrgew Floating Merge Even Word
                Sparse(21, 5, invalid, "  00111",
                    ( 0b10010, Instr(Mnemonic.mffs, C,f1)),                 // X I 171 P1 mffs[.] Move From FPSCR
                    ( 0b10110, Instr(Mnemonic.mtfsf, u17_8,f3 ))),          // XFL I 172 P1 mtfsf[.] Move To FPSCR Fields
                
                Sparse(21, 5, invalid, "  01000",
                    ( 0b00000, Nyi("fcpsgn[.]")), // 111111 ..... ..... ..... 00000 01000. X I 151 v2.05 fcpsgn[.] Floating Copy Sign
                    ( 0b00001, Instr(Mnemonic.fneg, C,f1,f3)),      // X I 151 P1 fneg[.] Floating Negate
                    ( 0b00010, Instr(Mnemonic.fmr, C,f1,f3)),       // X I 151 P1 fmr[.] Floating Move Register
                    ( 0b00100, Instr(Mnemonic.fnabs, C,f1,f3)),     // X I 151 P1 fnabs[.] Floating Negative Absolute Value
                    ( 0b01000, Instr(Mnemonic.fabs, C,f1,f3)),      // X I 151 P1 fabs[.] Floating Absolute
                    ( 0b01100, Instr(Mnemonic.frin, C,f1,f3)),      // X I 167 v2.02 frin[.] Floating Round To Integer Nearest
                    ( 0b01101, Instr(Mnemonic.friz, C, f1, f3)),    // X I 167 v2.02 friz[.] Floating Round To Integer Zero
                    ( 0b01110, Instr(Mnemonic.frip, C, f1, f3)),    // X I 167 v2.02 frip[.] Floating Round To Integer Plus
                    ( 0b01111, Instr(Mnemonic.frim, C,f1,f3))),     // X I 167 v2.02 frim[.] Floating Round To Integer Minus
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.frsp, C, f1, f3),                    // 111111 ..... ///// ..... 00000 01100. X I 160 P1 frsp[.] Floating Round to SP
                invalid,
                Sparse(21, 5, invalid, "  01110",
                    ( 0b00000, Instr(Mnemonic.fctiw, C,f1,f3)),     // X I 162 P2 fctiw[.] Floating Convert To Integer Word
                    ( 0b00100, Nyi("fctiwu[.]")),                   // X I 163 v2.06 fctiwu[.] Floating Convert To Integer Word Unsigned
                    ( 0b11001, Instr(Mnemonic.fctid, C,f1,f3)),     // X I 160 PPC fctid[.] Floating Convert To Integer Dword
                    ( 0b11010, Instr(Mnemonic.fcfid, C,f1,f3)),     // X I 164 PPC fcfid[.] Floating Convert From Integer Dword
                    ( 0b11101, Instr(Mnemonic.fctidu, C,f1,f3)),    // X I 161 v2.06 fctidu[.] Floating Convert To Integer Dword Unsigned
                    ( 0b11110, Instr(Mnemonic.fcfidu, C,f1,f3))),   // X I 165 v2.06 fcfidu[.] Floating Convert From Integer Dword Unsigned
                Sparse(21, 5, invalid, "  01111",
                    ( 0b00000, Instr(Mnemonic.fctiwz, C, f1, f3)),  // X I 163 P2 fctiwz[.] Floating Convert To Integer Word truncate
                    ( 0b00100, Instr(Mnemonic.fctiwuz, C, f1,f3)),  // X I 164 v2.06 fctiwuz[.] Floating Convert To Integer Word Unsigned truncate
                    ( 0b11001, Instr(Mnemonic.fctidz, C, f1, f3)),  // X I 161 PPC fctidz[.] Floating Convert To Integer Dword truncate
                    ( 0b11101, Instr(Mnemonic.fctiduz, C, f1, f3))),    // X I 162 v2.06 fctiduz[.] Floating Convert To Integer Dword Unsigned truncate

                invalid,    // 10000
                invalid,    // 10001
                Instr(Mnemonic.fdiv, C, f1, f2, f3),        // 111111 ..... ..... ..... ///// 10010. A I 154 P1 fdiv[.] Floating Divide
                invalid,    // 10011

                Instr(Mnemonic.fsub, C,f1,f2,f3),           // 111111 ..... ..... ..... ///// 10100. A I 153 P1 fsub[.] Floating Subtract
                Instr(Mnemonic.fadd, C,f1,f2,f3),           // 111111 ..... ..... ..... ///// 10101. A I 153 P1 fadd[.] Floating Add
                Instr(Mnemonic.fsqrt, C,f1,f3),             // 111111 ..... ///// ..... ///// 10110. A I 155 P2 fsqrt[.] Floating Square Root
                Instr(Mnemonic.fsel, C,f1,f2,f4,f3),        // 111111 ..... ..... ..... ..... 10111. A I 169 PPC fsel[.] Floating Select
                
                Instr(Mnemonic.fre, C,f2,f3),               // 111111 ..... ///// ..... ///// 11000. A I 155 v2.02 fre[.] Floating Reciprocal Estimate
                Instr(Mnemonic.fmul, C,f1,f2,f4),           // 111111 ..... ..... ///// ..... 11001. A I 154 P1 fmul[.] Floating Multiply
                Instr(Mnemonic.frsqrte, C,f1,f3),           // 111111 ..... ///// ..... ///// 11010. A I 156 PPC frsqrte[.] Floating Reciprocal Square Root Estimate
                invalid,

                Instr(Mnemonic.fmsub, C,f1,f2,f4,f3),       // 111111 ..... ..... ..... ..... 11100. A I 159 P1 fmsub[.] Floating Multiply-Subtract
                Instr(Mnemonic.fmadd, C,f1,f2,f4,f3),       // 111111 ..... ..... ..... ..... 11101. A I 158 P1 fmadd[.] Floating Multiply-Add
                Instr(Mnemonic.fnmsub, C,f1,f2,f4,f3),      // 111111 ..... ..... ..... ..... 11110. A I 159 P1 fnmsub[.] Floating Negative Multiply-Subtract
                Instr(Mnemonic.fnmadd, C,f1,f2,f4,f3));    // 111111 ..... ..... ..... ..... 11111. A I 159 P1 fnmadd[.] Floating Negative Multiply-Add
            return decoder;
        }
    }
}
