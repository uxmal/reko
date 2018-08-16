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
using static Reko.Arch.PowerPC.PowerPcDisassembler;

namespace Reko.Arch.PowerPC
{
    public class DecoderFactory
    {
        private readonly Decoder invalid;
        private readonly string model;

        public DecoderFactory(string model)
        {
            this.invalid = new InvalidDecoder();
            this.model = model ?? ""; 
        }

        public Decoder[] CreateDecoders()
        {
            var decoders = new Decoder[] {
                // 00
                invalid,
                invalid,
                new DOpRec(Opcode.tdi, "I1,r2,S"),
                new DOpRec(Opcode.twi, "I1,r2,S"),
                Ext4Decoder(),
                Ext5Decoder(),
                Ext6Decoder(),
                new DOpRec(Opcode.mulli, "r1,r2,S"),

                new DOpRec(Opcode.subfic, "r1,r2,S"),
                invalid,
                new CmpOpRec(Opcode.cmpli, "C1,r2,U"),
                new CmpOpRec(Opcode.cmpi, "C1,r2,S"),
                new DOpRec(Opcode.addic, "r1,r2,S"),
                new DOpRec(Opcode.addic, ":r1,r2,S"),
                new DOpRec(Opcode.addi, "r1,r2,S"),
                new DOpRec(Opcode.addis, "r1,r2,S"),
                // 10
                new BOpRec(),
                new DOpRec(Opcode.sc, ""),
                new IOpRec(),
                new XOpRec(new Dictionary<uint, Decoder>()
                {
                    { 0, new DOpRec(Opcode.mcrf, "C1,C2")},
                    { 16, new BclrOpRec() },
                    { 33, new DOpRec(Opcode.crnor, "I1,I2,I3") },
                    { 50, new DOpRec(Opcode.rfi, "") },
                    { 0x96, new DOpRec(Opcode.isync, "") },
                    { 449, new DOpRec(Opcode.cror, "I1,I2,I3") },
                    { 0x0C1, new DOpRec(Opcode.crxor, "I1,I2,I3") },
                    { 0x121, new DOpRec(Opcode.creqv, "I1,I2,I3") },
                    { 0x210, new XlOpRecAux(Opcode.bcctr, Opcode.bctrl, "I1,I2")}
                }),
                new DOpRec(Opcode.rlwimi, "r2,r1,I3,I4,I5"),
                new DOpRec(Opcode.rlwinm, ".r2,r1,I3,I4,I5"),
                invalid,
                new DOpRec(Opcode.rlwnm, "r2,r1,r3,I4,I5"),

                new DOpRec(Opcode.ori, "r2,r1,U"),
                new DOpRec(Opcode.oris, "r2,r1,U"),
                new DOpRec(Opcode.xori, "r2,r1,U"),
                new DOpRec(Opcode.xoris, "r2,r1,U"),
                new DOpRec(Opcode.andi, ":r2,r1,U"),
                new DOpRec(Opcode.andis, ":r2,r1,U"),
                new MDOpRec(),
                Ext1FDecoder(),
                // 20
                new DOpRec(Opcode.lwz, "r1,E2"),
                new DOpRec(Opcode.lwzu, "r1,E2"),
                new DOpRec(Opcode.lbz, "r1,E2"),
                new DOpRec(Opcode.lbzu, "r1,E2"),
                new DOpRec(Opcode.stw, "r1,E2"),
                new DOpRec(Opcode.stwu, "r1,E2"),
                new DOpRec(Opcode.stb, "r1,E2"),
                new DOpRec(Opcode.stbu, "r1,E2"),

                new DOpRec(Opcode.lhz, "r1,E2"),
                new DOpRec(Opcode.lhzu, "r1,E2"),
                new DOpRec(Opcode.lha, "r1,E2"),
                new DOpRec(Opcode.lhau, "r1,E2"),
                new DOpRec(Opcode.sth, "r1,E2"),
                new DOpRec(Opcode.sthu, "r1,E2"),
                new DOpRec(Opcode.lmw, "r1,E2"),
                new DOpRec(Opcode.stmw, "r1,E2"),
                // 30
                new DOpRec(Opcode.lfs, "f1,E2"),
                new DOpRec(Opcode.lfsu, "f1,E2"),
                new DOpRec(Opcode.lfd, "f1,E2"),
                new DOpRec(Opcode.lfdu, "f1,E2"),
                new DOpRec(Opcode.stfs, "f1,E2"),
                new DOpRec(Opcode.stfsu, "f1,E2"),
                new DOpRec(Opcode.stfd, "f1,E2"),
                new DOpRec(Opcode.stfdu, "f1,E2"),

                Ext38Decoder(),
                new DOpRec(Opcode.lfdp, "p1,E2:2"),             // 39
                new DSOpRec(Opcode.ld, Opcode.ldu, "r1,E2"),    // 3A
                Ext3BDecoder(),

                Ext3CDecoder(),
                new DOpRec(Opcode.stfdp, "p1,E2:2"),                    // 3D
                new DSOpRec(Opcode.std, Opcode.stdu, "r1,E2"),          // 3E
                new FpuOpRec(1, 0x1F, new Dictionary<uint, Decoder>()     // 3F
                {
                    { 0x00, new FpuOpRec(6, 0x1F, new Dictionary<uint, Decoder>
                        {
                            { 0, new FpuOpRecAux(Opcode.fcmpu, "C1,f2,f3") },
                            { 1, new FpuOpRecAux(Opcode.fcmpo, "C1,f2,f3") },
                            //{ 2, new FpuOpRecAux(Opcode.mcrfs)}
                        })
                    },
                    { 0x06, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            //{ 1, new FpuOpRecAux(Opcode.mtfsb1 },
                            //{ 2, new FpuOpRecAux(Opcode.mtfsb0 },
                            //{ 4, new FpuOpRecAux(Opcode.mtfsfi }
                        })
                    },
                    { 0x07, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            { 0x12, new FpuOpRecAux(Opcode.mffs, ".f1" )},
                            { 0x16, new FpuOpRecAux(Opcode.mtfsf, "u17:8,f3" )},
                        })
                    },
                    { 0x08, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            { 1, new FpuOpRecAux(Opcode.fneg, ".f1,f3") },
                            { 2, new FpuOpRecAux(Opcode.fmr, ".f1,f3" )},
                            { 4, new FpuOpRecAux(Opcode.fnabs, ".f1,f3") },
                            { 8, new FpuOpRecAux(Opcode.fabs, ".f1,f3") },
                        })
                    },
                    { 0x0C, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            { 0, new FpuOpRecAux(Opcode.frsp, ".f1,f3") },
                        })
                    },
                    { 0x0E, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            { 0x00, new FpuOpRecAux(Opcode.fctiw, ".f1,f3") },
                            { 0x19, new FpuOpRecAux(Opcode.fctid, ".f1,f3") },
                            { 0x1A, new FpuOpRecAux(Opcode.fcfid,  ".f1,f3") },
                        })
                    },
                    { 0x0F, new FpuOpRec(6, 0x1F, new Dictionary<uint,Decoder>
                        {
                            { 0x00, new FpuOpRecAux(Opcode.fctiwz,  ".f1,f3") },
                            { 0x19, new FpuOpRecAux(Opcode.fctidz,   ".f1,f3") },
                        })
                    },

                    { 18, new FpuOpRecAux(Opcode.fdiv, ".f1,f2,f3") },
                    { 20, new FpuOpRecAux(Opcode.fsub, ".f1,f2,f3") },
                    { 21, new FpuOpRecAux(Opcode.fadd, ".f1,f2,f3") },
                    { 0x16, new FpuOpRecAux(Opcode.fsqrt, ".f1,f3") },
                    { 0x17, new FpuOpRecAux(Opcode.fsel, ".f1,f2,f4,f3") },
                    { 0x19, new FpuOpRecAux(Opcode.fmul, ".f1,f2,f4") },
                    { 0x1A, new FpuOpRecAux(Opcode.frsqrte, ".f1,f3") },

                    { 0x1C, new FpuOpRecAux(Opcode.fmsub, ".f1,f2,f4,f3") },

                    { 0x1D, new FpuOpRecAux(Opcode.fmadd, ".f1,f2,f4,f3") },
                    { 0x1E, new FpuOpRecAux(Opcode.fnmsub, ".f1,f2,f4,f3") },
                    { 0x1F, new FpuOpRecAux(Opcode.fnmadd, ".f1,f2,f4,f3") },
                })

            };
            return decoders;
        }


        private Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        private Decoder Instr(Opcode opcode, string format)
        {
            return new DOpRec(opcode, format);
        }

        private Decoder Mask(int ppcBitPosition, int bits, params Decoder[] decoders)
        {
            return new MaskDecoder(ppcBitPosition, bits, decoders);
        }

        private Decoder Sparse(int ppcBitPosition, int bits, params (uint, Decoder)[] sparseDecoders)
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
                    decoders[i] = invalid;
            }
            return new MaskDecoder(ppcBitPosition, bits, decoders);
        }

        private Decoder Ext4Decoder()
        {
            if (string.Compare(this.model , "750") == 0)
            {
                return Ext4Decoder_PPC750CL();
            }
            else
            {
                return Ext4Decoder_VMX128();
            }
        }

        private Decoder Ext4Decoder_VMX128()
        {
            var decoder = new VXOpRec(
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
                { 0x403, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x407, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x40B, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x40F, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
   /*
=================================================================
   lvrx128                          Load Vector128 Right Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 1 0 0|VDh|1 1|
   lvrx128       vr(VD128), r(RA), r(RB) */
                { 0x443, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x447, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x44B, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x44F, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
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
                { 0x0C3, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0C7, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0CB, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0CF, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
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
                { 0x503, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x507, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x50B, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x50F, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },

/*=================================================================
   stvlxl128                    Store Vector128 Left Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 0 0 0|VDh|1 1|
   lvlxl128      vr(VS128), r(RA), r(RB)
=================================================================
   stvrx128                        Store Vector128 Right Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 1 0 0|VDh|1 1|
   stvrx128       vr(VS128), r(RA), r(RB)
*/
                { 0x543, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x547, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x54B, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x54F, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },

/*=================================================================
   stvrxl128                   Store Vector128 Right Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 1 0 0|VDh|1 1|
   stvrxl128     vr(VS128), r(RA), r(RB)
=================================================================
   stvx128                               Store Vector128 Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 0 1 1 1 0 0|VDh|1 1|
   stvx128       vr(VS128), r(RA), r(RB)*/
                { 0x1C3, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1C7, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1CB, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1CF, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },

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
                        { 0x000, new DOpRec(Opcode.vaddubm, "v1,v2,v3") },
                        { 0x002, new DOpRec(Opcode.vmaxub, "v1,v2,v3") },
                        { 0x00A, new DOpRec(Opcode.vaddfp, "v1,v2,v3") },
                       // { 0x010, new DOpRec(Opcode.mulhhwu, "r1,r2,r3") },
                        { 0x020, new DOpRec(Opcode.vmhaddshs, "v1,v2,v3,v4") },
                        { 0x022, new DOpRec(Opcode.vmladduhm, "v1,v2,v3,v4") },
                        { 0x042, new DOpRec(Opcode.vmaxuh, "v1,v2,v3") },
                        { 0x04A, new DOpRec(Opcode.vsubfp, "v1,v2,v3") },
                        { 0x080, new DOpRec(Opcode.vadduwm, "v1,v2,v3") },
                        { 0x086, new DOpRec(Opcode.vcmpequw, "v1,v2,v3") },
                        { 0x08C, new DOpRec(Opcode.vmrghw, "v1,v2,v3") },
                        { 0x0C6, new DOpRec(Opcode.vcmpeqfp, "v1,v2,v3") },
                        { 0x0E2, new DOpRec(Opcode.vmladduhm, "v1,v2,v3,v4") },
                        { 0x100, new DOpRec(Opcode.vadduqm, "v1,v2,v3") },
                        { 0x10A, new DOpRec(Opcode.vrefp, "v1,v3") },
                        { 0x14A, new DOpRec(Opcode.vrsqrtefp, "v1,v3") },
                        { 0x184, new DOpRec(Opcode.vslw, "v1,v2,v3") },
                        { 0x18C, new DOpRec(Opcode.vmrglw, "v1,v2,v3") },
                        { 0x200, new DOpRec(Opcode.vaddubs, "v1,v2,v3") },
                        { 0x28C, new DOpRec(Opcode.vspltw, "v1,v3,u16:2") },
                        { 0x2C6, new DOpRec(Opcode.vcmpgtfp, "v1,v2,v3") },
                        { 0x34A, new DOpRec(Opcode.vcfsx, "v1,v3,u16:5") },
                        { 0x38C, new DOpRec(Opcode.vspltisw, "v1,s16:5") },
                        { 0x3CA, new DOpRec(Opcode.vctsxs, "v1,v3,u16:5") },
                        { 0x401, new DOpRec(Opcode.bcdadd, ":v1,v2,v3,u9:1") },
                        { 0x404, new DOpRec(Opcode.vand, "v1,v2,v3") },
                        { 0x406, new DOpRec(Opcode.vcmpequb, ":v1,v2,v3") },
                        { 0x444, new DOpRec(Opcode.vandc, "v1,v2,v3") },
                        { 0x484, new DOpRec(Opcode.vor, "v1,v2,v3") },
                        { 0x4C4, new DOpRec(Opcode.vxor, "v1,v2,v3")},
                        { 0x4C6, new DOpRec(Opcode.vcmpeqfp, ":v1,v2,v3") },
                        { 0x4C7, new DOpRec(Opcode.vcmpequd, ":v1,v2,v3") },
                        // These aren't present for the VMX extension of PowerPC architecture
                        //{ 0x503, new DOpRec(Opcode.evmhessfaaw, "r1,r2,r3") },
                        //{ 0x50B, new DOpRec(Opcode.evmhesmfaaw, "r1,r2,r3") },
                        { 0x686, new DOpRec(Opcode.vcmpgtuw, ":v1,v2,v3") },
                        { 0x6C6, new DOpRec(Opcode.vcmpgtfp, ":v1,v2,v3") },
                 },
                 new Dictionary<uint, Decoder>
                 {
                        { 0x028, new DOpRec(Opcode.vmsumshm, "v1,v2,v3,v4") },
                        { 0x029, new DOpRec(Opcode.vmsumshs, "v1,v2,v3,v4") },
                        { 0x02A, new DOpRec(Opcode.vsel, "v1,v2,v3,v4") },
                        { 0x02B, new DOpRec(Opcode.vperm, "v1,v2,v3,v4") },
                        { 0x02C, new DOpRec(Opcode.vsldoi, "v1,v2,v3,u6:5") },
                        { 0x02E, new DOpRec(Opcode.vmaddfp, "v1,v2,v4,v3") },
                        { 0x02F, new DOpRec(Opcode.vnmsubfp, "v1,v2,v4,v3") }

                 });
            return decoder;
        }

        private Decoder Ext4Decoder_PPC750CL()
        {
            var decoder = Mask(26, 5,
                Sparse(21, 5,
                    (1, Instr(Opcode.ps_cmpo0, "c1,f2,f3"))
                    ),
                Nyi("0b00001"),
                Nyi("0b00010"),
                Nyi("0b00011"),

                Nyi("0b00100"),
                Nyi("0b00101"),
                Mask(25, 1,
                    Instr(Opcode.psq_lx, "f1,r2,r3,u21:1,u22:3"),
                    Instr(Opcode.psq_lux, "f1,r2,r3,u21:1,u22:3")),
                Mask(25, 1,
                    Instr(Opcode.psq_stx, "f1,r2,r3,u21:1,u22:3"),
                    Instr(Opcode.psq_stux, "f1,r2,r3,u21:1,u22:3")),

                Sparse(21, 5,
                    (1, new DOpRec(Opcode.ps_neg, ".f1,f3")),
                    (2, new DOpRec(Opcode.ps_mr, ".f1,f3")),
                    (4, new DOpRec(Opcode.ps_nabs, ".f1,f3")),
                    (8, new DOpRec(Opcode.ps_abs, ".f1,f3"))),
                Nyi("0b01001"),
                Instr(Opcode.ps_sum0, ".f1,f2,f3,f4"),
                Instr(Opcode.ps_sum1, ".f1,f2,f3,f4"),

                Instr(Opcode.ps_muls0, ".f1,f2,f4"),
                Instr(Opcode.ps_muls1, ".f1,f2,f4"),
                Instr(Opcode.ps_madds0, ".f1,f2,f4,f3"),
                Instr(Opcode.ps_madds1, ".f1,f2,f4,f3"),

                Sparse(21, 5,
                    (0b10000, Instr(Opcode.ps_merge00, ".f1,f2,f3")),
                    (0b10001, Instr(Opcode.ps_merge01, ".f1,f2,f3")),
                    (0b10010, Instr(Opcode.ps_merge10, ".f1,f2,f3")),
                    (0b10011, Instr(Opcode.ps_merge11, ".f1,f2,f3"))),
                Nyi("0b10001"),
                Instr(Opcode.ps_div, ".f1,f2,f3"),
                Nyi("0b10011"),

                Instr(Opcode.ps_sub, ".f1,f2,f3"),
                Instr(Opcode.ps_add, ".f1,f2,f3"),
                Nyi("0b10110"),
                Instr(Opcode.ps_sel, ".f1,f2,f3"),

                Instr(Opcode.ps_res, ".f1,f3"),
                Instr(Opcode.ps_mul, ".f1,f2,f4"),
                Instr(Opcode.ps_rsqrte, ".f1,f3"),
                Nyi("0b11011"),

                Instr(Opcode.ps_msub, ".f1,f2,f3,f4"),
                Instr(Opcode.ps_madd, ".f1,f2,f3,f4"),
                Instr(Opcode.ps_nmsub, ".f1,f2,f3,f4"),
                Instr(Opcode.ps_nmadd, ".f1,f2,f3,f4"));
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

                { 0x01, new DOpRec(Opcode.vaddfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x03, new DOpRec(Opcode.vaddfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x05, new DOpRec(Opcode.vsubfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|    vsubfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x07, new DOpRec(Opcode.vsubfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|    vsubfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x09, new DOpRec(Opcode.vmulfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x0B, new DOpRec(Opcode.vmulfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x11, new DOpRec(Opcode.vmaddcfp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|    vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)
                { 0x13, new DOpRec(Opcode.vmaddcfp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|    vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)

                { 0x19, new DOpRec(Opcode.vmsub3fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|    vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)
                { 0x1B, new DOpRec(Opcode.vmsub3fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|    vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)

                { 0x1D, new DOpRec(Opcode.vmsub4fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
                { 0x1F, new DOpRec(Opcode.vmsub4fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)

                { 0x21, new DOpRec(Opcode.vand128, "Wd,Wa,Wb") },           // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|    vand128       vr(VD128), vr(VA128), vr(VB128)
                { 0x23, new DOpRec(Opcode.vand128, "Wd,Wa,Wb") },           // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|    vand128       vr(VD128), vr(VA128), vr(VB128)



                { 0x2D, new DOpRec(Opcode.vor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)
                { 0x2F, new DOpRec(Opcode.vor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)

                { 0x31, new DOpRec(Opcode.vxor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)
                { 0x33, new DOpRec(Opcode.vxor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)

            });
        }

        private Decoder Ext6Decoder() {
            return new VMXDecoder(0x7F, new Dictionary<uint, Decoder>
            {
                { 0x00, new DOpRec(Opcode.vcmpeqfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x02, new DOpRec(Opcode.vcmpeqfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x04, new DOpRec(Opcode.vcmpeqfp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x06, new DOpRec(Opcode.vcmpeqfp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x40, new DOpRec(Opcode.vcmpeqfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x42, new DOpRec(Opcode.vcmpeqfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x44, new DOpRec(Opcode.vcmpeqfp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x46, new DOpRec(Opcode.vcmpeqfp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x08, new DOpRec(Opcode.vcmpgefp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0A, new DOpRec(Opcode.vcmpgefp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0C, new DOpRec(Opcode.vcmpgefp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0E, new DOpRec(Opcode.vcmpgefp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x48, new DOpRec(Opcode.vcmpgefp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4A, new DOpRec(Opcode.vcmpgefp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4C, new DOpRec(Opcode.vcmpgefp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4E, new DOpRec(Opcode.vcmpgefp128, ":Wd,Wa,Wb") },      // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)


                { 0x10, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x12, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x14, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x16, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x50, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x52, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x54, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x56, new DOpRec(Opcode.vcmpgtfp128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x18, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1A, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1C, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1E, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x58, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5A, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5C, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5E, new DOpRec(Opcode.vcmpbfp128, "Wd,Wa,Wb") },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x0D, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x0F, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4D, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4F, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x1D, new DOpRec(Opcode.vsrw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x1F, new DOpRec(Opcode.vsrw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5D, new DOpRec(Opcode.vsrw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5F, new DOpRec(Opcode.vsrw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x20, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x22, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x24, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x26, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x60, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x62, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x64, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x66, new DOpRec(Opcode.vcmpequw128, "Wd,Wa,Wb") },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x23, new DOpRec(Opcode.vcfpsxws128, "Wd,Wb,s16:5") },    // |0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|    vcfpsxws128   vr(VD128), vr(VB128), SIMM

                { 0x28, new DOpRec(Opcode.vmaxfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2A, new DOpRec(Opcode.vmaxfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x68, new DOpRec(Opcode.vmaxfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6A, new DOpRec(Opcode.vmaxfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x2B, new DOpRec(Opcode.vcsxwfp128, "Wd,Wb,u16:5") },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|    vcsxwfp128    vr(VD128), vr(VB128), SIMM

                { 0x2C, new DOpRec(Opcode.vminfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2E, new DOpRec(Opcode.vminfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6C, new DOpRec(Opcode.vminfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6E, new DOpRec(Opcode.vminfp128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x30, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x32, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x70, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x72, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x34, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x36, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x74, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x76, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x37, new DOpRec(Opcode.vrfin128, "Wd,Wb") },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|    vrfin128      vr(VD128), vr(VB128)

                { 0x3F, new DOpRec(Opcode.vrfiz128, "Wd,Wb") },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|    vrfiz128      vr(VD128), vr(VB128)

                { 0x61, new DOpRec(Opcode.vpkd3d128, "Wd,Wb,u18:3,u16:2,u6:2") },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x65, new DOpRec(Opcode.vpkd3d128, "Wd,Wb,u18:3,u16:2,u6:2") },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x69, new DOpRec(Opcode.vpkd3d128, "Wd,Wb,u18:3,u16:2,u6:2") },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x6D, new DOpRec(Opcode.vpkd3d128, "Wd,Wb,u18:3,u16:2,u6:2") },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z

                { 0x63, new DOpRec(Opcode.vrefp128, "Wd,Wb") },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|    vrefp128      vr(VD128), vr(VB128)

                { 0x67, new DOpRec(Opcode.vrsqrtefp128, "Wd,Wb") },         // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|    vrsqrtefp128  vr(VD128), vr(VB128)

                { 0x6B, new DOpRec(Opcode.vexptefp128, "Wd,Wb") },          // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|    vexptefp128   vr(VD128), vr(VB128)

                { 0x6F, new DOpRec(Opcode.vlogefp128, "Wd,Wb") },           // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|    vlogefp128    vr(VD128), vr(VB128)

                { 0x77, new DOpRec(Opcode.vspltisw128, "Wd,Wb,s16:5") },

                { 0x71, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x75, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x79, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x7D, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z

                { 0x73, new DOpRec(Opcode.vspltw128, "Wd,Wb,u16:5") },      // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|    vspltw128     vr(VD128), vr(VB128), UIMM

                { 0x7F, new DOpRec(Opcode.vupkd3d128, "Wd,Wb,u16:5") },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|    vupkd3d128    vr(VD128), vr(VB128), UIMM
            });
        }

        private Decoder Ext1FDecoder()
        {
            return new XOpRec(new Dictionary<uint, Decoder>      // 1F
            {
                { 0, new CmpOpRec(Opcode.cmp, "C1,r2,r3") },
                { 4, new DOpRec(Opcode.tw, "I1,r2,r3") },
                { 0x006, new DOpRec(Opcode.lvsl, "v1,r2,r3") },
                { 0x008, new DOpRec(Opcode.subfc, "r1,r2,r3")},
                { 0x00A, new DOpRec(Opcode.addc, "r1,r2,r3")},
                { 0x00B, new DOpRec(Opcode.mulhwu, ".r1,r2,r3")},
                { 0x013, new DOpRec(Opcode.mfcr, "r1") },
                { 0x015, new DOpRec(Opcode.ldx, "r1,r2,r3") },
                { 0x017, new DOpRec(Opcode.lwzx, "r1,r2,r3") },
                { 0x018, new DOpRec(Opcode.slw, ".r2,r1,r3") },
                { 0x01A, new DOpRec(Opcode.cntlzw, "r2,r1") },
                { 0x01B, new DOpRec(Opcode.sld, ".r2,r1,r3") },
                { 0x01C, new DOpRec(Opcode.and, ".r2,r1,r3")},
                { 0x020, new CmpOpRec(Opcode.cmpl, "C1,r2,r3") },
                { 0x014, new DOpRec(Opcode.lwarx, "r1,r2,r3") },
                { 0x028, new DOpRec(Opcode.subf, ".r1,r2,r3")},
                { 0x035, new DOpRec(Opcode.ldux, "r1,r2,r3")},
                { 0x036, new DOpRec(Opcode.dcbst, "r2,r3")},
                { 0x037, new DOpRec(Opcode.lwzux, "r1,r2,r3")},
                { 0x03A, new DOpRec(Opcode.cntlzd, "r2,r1") },
                { 0x03C, new DOpRec(Opcode.andc, ".r2,r1,r3") },
                { 0x047, new DOpRec(Opcode.lvewx, "v1,r2,r3") },
                { 0x04B, new DOpRec(Opcode.mulhw, ".r1,r2,r3") },
                { 0x053, new DOpRec(Opcode.mfmsr, "r1") },
                { 0x054, new DOpRec(Opcode.ldarx, "r1,r2,r3") },
                { 0x056, new DOpRec(Opcode.dcbf, "r2,r3") },
                { 0x057, new DOpRec(Opcode.lbzx, "r1,r2,r3") },
                { 0x067, new DOpRec(Opcode.lvx, "v1,r2,r3") },
                { 0x068, new DOpRec(Opcode.neg, "r1,r2") },
                { 0x077, new DOpRec(Opcode.lbzux, "r1,r2,r3") },
                { 124, new DOpRec(Opcode.nor, ".r2,r1,r3") },
                { 0x088, new DOpRec(Opcode.subfe, "r1,r2,r3") },
                { 0x08A, new DOpRec(Opcode.adde, ".r1,r2,r3") },
                { 0x090, new DOpRec(Opcode.mtcrf, "M,r1") },
                { 0x092, new DOpRec(Opcode.mtmsr, "r1,u16:1") },
                { 0x095, new DOpRec(Opcode.stdx, "r1,r2,r3") },
                { 0x096, new DOpRec(Opcode.stwcx, ".r1,r2,r3") },
                { 0x097, new DOpRec(Opcode.stwx, "r1,r2,r3") },
                { 0x0C7, new DOpRec(Opcode.stvewx, "v1,r2,r3")},
                { 0x0B2, new DOpRec(Opcode.mtmsrd, "r1,u16:1") },
                { 0x0B7, new DOpRec(Opcode.stwux, "r1,r2,r3") },
                { 0x0E7, new DOpRec(Opcode.stvx, "v1,r2,r3") },
                { 215, new DOpRec(Opcode.stbx, "r1,r2,r3") },
                { 235, new DOpRec(Opcode.mullw, ".r1,r2,r3") },
                { 0x0C8, new DOpRec(Opcode.subfze, ".r1,r2") },
                { 0x0CA, new DOpRec(Opcode.addze, ".r1,r2") },
                { 0x0D6, new DOpRec(Opcode.stdcx, ":r1,r2,r3") },
                { 0x0E9, new DOpRec(Opcode.mulld, ".r1,r2,r3")},
                { 0x0EA, new DOpRec(Opcode.addme, ".r1,r2")},
                { 0x0F6, new DOpRec(Opcode.dcbtst, "r2,r3") },
                { 247, new DOpRec(Opcode.stbux, "r1,r2,r3") },
                { 0x10A, new DOpRec(Opcode.add, ".r1,r2,r3") },
                { 279, new DOpRec(Opcode.lhzx, "r1,r2,r3") },
                { 0x116, new DOpRec(Opcode.dcbt, "r2,r3,u21:4") },
                { 0x11C, new DOpRec(Opcode.eqv, "r1,r2,r3") },
                { 316, new DOpRec(Opcode.xor, ".r2,r1,r3") },
                { 444, new DOpRec(Opcode.or, ".r2,r1,r3") },

                { 0x153, new SprOpRec(false) },
                { 0x155, new DOpRec(Opcode.lwax, "r1,r2,r3") },
                { 0x157, new DOpRec(Opcode.lhax, "r1,r2,r3") },
                { 0x173, new XfxOpRec(Opcode.mftb, "r1,X3") },
                { 0x177, new DOpRec(Opcode.lhaux, "r1,r2,r3") },
                { 0x197, new DOpRec(Opcode.sthx, "r1,r2,r3") },
                { 0x19C, new DOpRec(Opcode.orc, ".r2,r1,r3") },
                { 0x1C9, new DOpRec(Opcode.divdu, ".r1,r2,r3") },
                { 0x1CB, new DOpRec(Opcode.divwu, ".r1,r2,r3") },
                { 0x1DC, new DOpRec(Opcode.nand, ".r2,r1,r3") },
                { 0x1D3, new SprOpRec(true) },
                { 0x1D6, new DOpRec(Opcode.dcbi, "r2,r3")},
                { 0x1E9, new DOpRec(Opcode.divd, ".r1,r2,r3")},
                { 0x1EB, new DOpRec(Opcode.divw, ".r1,r2,r3")},
                { 0x207, new DOpRec(Opcode.lvlx, "r1,r2,r3") },
                { 0x216, new DOpRec(Opcode.lwbrx, "r1,r2,r3") },
                { 0x217, new DOpRec(Opcode.lfsx, "f1,r2,r3") },
                { 0x218, new DOpRec(Opcode.srw, ".r2,r1,r3") },
                { 0x21B, new DOpRec(Opcode.srd, ".r2,r1,r3") },
                { 0x255, new DOpRec(Opcode.lswi, "r1,r2,I3") },
                { 0x237, new DOpRec(Opcode.lfsux, "f1,r2,r3") },
                { 0x257, new DOpRec(Opcode.lfdx, "f1,r2,r3") },
                { 0x277, new DOpRec(Opcode.lfdux, "f1,r2,r3") },
                { 0x256, new DOpRec(Opcode.sync, "") },
                { 0x296, new DOpRec(Opcode.stwbrx, ".r2,r1,r3") },
                { 0x297, new DOpRec(Opcode.stfsx, "f1,r2,r3") },
                { 0x2B7, new DOpRec(Opcode.stfsux, "f1,r2,r3") },
                { 0x2D5, new DOpRec(Opcode.stswi, "r1,r2,I3") },
                { 0x2D7, new DOpRec(Opcode.stfdx, "f1,r2,r3") },
                { 0x316, new DOpRec(Opcode.lhbrx, "r1,r2,r3")},
                { 0x318, new DOpRec(Opcode.sraw, ".r2,r1,r2")},
                { 0x31A, new DOpRec(Opcode.srad, ".r2,r1,r2")},
                { 0x33A, new DOpRec(Opcode.sradi, ".r2,r1,I3") },
                { 0x338, new DOpRec(Opcode.srawi, "r2,r1,I3") },
                { 0x33B, new XSOpRec(Opcode.sradi, ".r2,r1,I3") },
                { 0x356, new DOpRec(Opcode.eieio, "") },
                { 0x39A, new DOpRec(Opcode.extsh, ".r2,r1")},
                { 0x3BA, new DOpRec(Opcode.extsb, ".r2,r1")},
                { 0x3D7, new DOpRec(Opcode.stfiwx, "f1,r2,r3")},
                { 0x3D6, new DOpRec(Opcode.icbi, "r2,r3")},
                { 0x3DA, new DOpRec(Opcode.extsw, ".r2,r1")},
                { 0x3F6, new DOpRec(Opcode.dcbz, "r2,r3") }
             });
        }

        private Decoder Ext38Decoder()
        {
            if (model == "750")
            {
                return Instr(Opcode.psq_l, "f1,r2,s0:12,u21:1,u22:3");
            }
            else
            {
                return Instr(Opcode.lq, "r1,E2");
            }
        }

        private Decoder Ext3BDecoder()
        {
            return new FpuOpRec(1, 0x1F, new Dictionary<uint, Decoder> // 3B
            {
                { 18, new FpuOpRecAux(Opcode.fdivs, ".f1,f2,f3") },
                { 20, new FpuOpRecAux(Opcode.fsubs, ".f1,f2,f3") },
                { 21, new FpuOpRecAux(Opcode.fadds, ".f1,f2,f3") },
                { 22, new FpuOpRecAux(Opcode.fsqrts, ".f1,f3") },
                { 24, new FpuOpRecAux(Opcode.fres, ".f1,f3") },
                { 25, new FpuOpRecAux(Opcode.fmuls, ".f1,f2,f4") },
                { 28, new FpuOpRecAux(Opcode.fmsubs, ".f1,f2,f4,f3") },
                { 29, new FpuOpRecAux(Opcode.fmadds, ".f1,f2,f4,f3") },
                { 30, new FpuOpRecAux(Opcode.fnmsubs, ".f1,f2,f3,f4") },
                { 31, new FpuOpRecAux(Opcode.fnmadds, ".f1,f2,f3,f4") },
            });
        }

        private Decoder Ext3CDecoder()
        {
            if (model == "750")
            {
                return Instr(Opcode.psq_st, "f1,r2,s0:12,u21:1,u22:3");
            }
            else
            {
                return new XX3OpRec(new Dictionary<uint, Decoder>                // 3C
                {
                    { 0x00, new DOpRec(Opcode.xsaddsp, "v1,v2,v3") },
                    { 0x01, new DOpRec(Opcode.xsmaddasp, "v1,v2,v3") },
                    //{ 0x02, new DOpRec(Opcode.xxsldwi, "v1,v2,v3") },       //$TODO need extra work.
                    { 0x09, new DOpRec(Opcode.xsmaddmsp, "v1,v2,v3") },
                });
            }
        }
    }
}
