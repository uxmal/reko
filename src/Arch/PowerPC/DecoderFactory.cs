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
using Reko.Core.Machine;
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
                new XDecoder(new Dictionary<uint, Decoder>()
                {
                    { 0, Instr(Mnemonic.mcrf, C1,C2)},
                    { 16, new BclrDecoder() },
                    { 33, Instr(Mnemonic.crnor, I1,I2,I3) },
                    { 50, Instr( InstrClass.Transfer, Mnemonic.rfi) },
                    { 0x96, Instr(Mnemonic.isync) },
                    { 449, Instr(Mnemonic.cror, I1,I2,I3) },
                    { 0x0C1, Instr(Mnemonic.crxor, I1,I2,I3) },
                    { 0x121, Instr(Mnemonic.creqv, I1,I2,I3) },
                    { 0x210, new XlDecoderAux(Mnemonic.bcctr, Mnemonic.bctrl, I1,I2)}
                }),
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
                new DSDecoder(Mnemonic.ld, Mnemonic.ldu, r1,E2),    // 3A
                Ext3BDecoder(),

                Ext3CDecoder(),
                Ext3DDecoder(),
                new DSDecoder(Mnemonic.std, Mnemonic.stdu, r1,E2),          // 3E
                new FpuDecoder(1, 0x1F, new Dictionary<uint, Decoder>()     // 3F
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
                })

            };
            return decoders;
        }


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<PowerPcDisassembler> [] mutators)
        {
            return new InstrDecoder(mnemonic, mutators, InstrClass.Linear);
        }

        private static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic,  mutators, iclass);
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
            if (string.Compare(this.model, "750") == 0)
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

        private Decoder Ext4Decoder_PPC750CL()
        {
            var decoder = Mask(26, 5,
                Sparse(21, 5,
                    (1, Instr(Mnemonic.ps_cmpo0, c1,f2,f3))
                    ),
                Nyi("0b00001"),
                Nyi("0b00010"),
                Nyi("0b00011"),

                Nyi("0b00100"),
                Nyi("0b00101"),
                Mask(25, 1,
                    Instr(Mnemonic.psq_lx, f1,r2,r3,u21_1,u22_3),
                    Instr(Mnemonic.psq_lux, f1,r2,r3,u21_1,u22_3)),
                Mask(25, 1,
                    Instr(Mnemonic.psq_stx, f1,r2,r3,u21_1,u22_3),
                    Instr(Mnemonic.psq_stux, f1,r2,r3,u21_1,u22_3)),

                Sparse(21, 5,
                    (1, Instr(Mnemonic.ps_neg, C,f1,f3)),
                    (2, Instr(Mnemonic.ps_mr, C,f1,f3)),
                    (4, Instr(Mnemonic.ps_nabs, C,f1,f3)),
                    (8, Instr(Mnemonic.ps_abs, C,f1,f3))),
                Nyi("0b01001"),
                Instr(Mnemonic.ps_sum0, C,f1,f2,f4,f3),
                Instr(Mnemonic.ps_sum1, C,f1,f2,f4,f3),

                Instr(Mnemonic.ps_muls0, C,f1,f2,f4),
                Instr(Mnemonic.ps_muls1, C,f1,f2,f4),
                Instr(Mnemonic.ps_madds0, C,f1,f2,f4,f3),
                Instr(Mnemonic.ps_madds1, C,f1,f2,f4,f3),

                Sparse(21, 5,
                    (0b10000, Instr(Mnemonic.ps_merge00, C,f1,f2,f3)),
                    (0b10001, Instr(Mnemonic.ps_merge01, C,f1,f2,f3)),
                    (0b10010, Instr(Mnemonic.ps_merge10, C,f1,f2,f3)),
                    (0b10011, Instr(Mnemonic.ps_merge11, C,f1,f2,f3))),
                Nyi("0b10001"),
                Instr(Mnemonic.ps_div, C,f1,f2,f3),
                Nyi("0b10011"),

                Instr(Mnemonic.ps_sub, C,f1,f2,f3),
                Instr(Mnemonic.ps_add, C,f1,f2,f3),
                Nyi("0b10110"),
                Instr(Mnemonic.ps_sel, C,f1,f2,f4,f3),

                Instr(Mnemonic.ps_res, C,f1,f3),
                Instr(Mnemonic.ps_mul, C,f1,f2,f4),
                Instr(Mnemonic.ps_rsqrte, C,f1,f3),
                Nyi("0b11011"),

                Instr(Mnemonic.ps_msub, C,f1,f2,f4,f3),
                Instr(Mnemonic.ps_madd, C,f1,f2,f4,f3),
                Instr(Mnemonic.ps_nmsub, C,f1,f2,f4,f3),
                Instr(Mnemonic.ps_nmadd, C,f1,f2,f4,f3));
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
                { 0x216, Instr(Mnemonic.lwbrx, r1,r2,r3) },
                { 0x217, Instr(Mnemonic.lfsx, f1,r2,r3) },
                { 0x218, Instr(Mnemonic.srw, C,r2,r1,r3) },
                { 0x21B, Instr(Mnemonic.srd, C,r2,r1,r3) },
                { 0x255, Instr(Mnemonic.lswi, r1,r2,I3) },
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
        }

        private Decoder Ext38Decoder()
        {
            if (model == "750")
            {
                return Instr(Mnemonic.psq_l, f1,r2,s0_12,u21_1,u22_3);
            }
            else
            {
                return Instr(Mnemonic.lq, Is64Bit, r1,E2);
            }
        }

        private Decoder Ext39Decoder()
        {
            if (model == "750")
            {
                return Instr(Mnemonic.psq_lu, f1,r2,s0_12,u21_1,u22_3);
            }
            else
            {
                return Instr(Mnemonic.lfdp, p1,E2_2);
            }
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
        }

        private Decoder Ext3CDecoder()
        {
            if (model == "750")
            {
                return Instr(Mnemonic.psq_st, f1,r2,s0_12,u21_1,u22_3);
            }
            else
            {
                return new XX3Decoder(new Dictionary<uint, Decoder>                // 3C
                {
                    { 0x00, Instr(Mnemonic.xsaddsp, v1,v2,v3) },
                    { 0x01, Instr(Mnemonic.xsmaddasp, v1,v2,v3) },
                    //{ 0x02, Instr(Mnemonic.xxsldwi, v1,v2,v3) },       //$TODO need extra work.
                    { 0x09, Instr(Mnemonic.xsmaddmsp, v1,v2,v3) },
                });
            }
        }

        private Decoder Ext3DDecoder()
        {
            if (model == "750")
            {
                return Instr(Mnemonic.psq_stu, f1,r2,s0_12,u21_1,u22_3);
            }
            else
            {
                return Instr(Mnemonic.stfdp, p1,E2_2);
            }
        }
    }
}
