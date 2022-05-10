#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public partial class X86Disassembler
    {
        public partial class InstructionSet
        {
            private void Create0F38Decoders(Decoder[] d)
            {
                // 0F 38 00
                d[0x00] = new PrefixedDecoder(
                        Instr(Mnemonic.pshufb, Pq,Qq),
                        VexInstr(Mnemonic.pshufb, Mnemonic.vpshufb, Vx,Hx,Wx));
                d[0x01] = new PrefixedDecoder(
                        Instr(Mnemonic.phaddw, Pq,Qq),
                        Instr(Mnemonic.vphaddw, Vx,Hx,Wx));
                d[0x02] = new PrefixedDecoder(
                        Instr(Mnemonic.phaddd, Pq,Qq),
                        Instr(Mnemonic.vphaddd, Vx,Hx,Wx));
                d[0x03] = new PrefixedDecoder(
                        Instr(Mnemonic.phaddsw, Pq,Qq),
                        Instr(Mnemonic.vphaddsw, Vx,Hx,Wx));

                d[0x04] = new PrefixedDecoder(
                        Instr(Mnemonic.pmaddubsw, Pq,Qq),
                        Instr(Mnemonic.vpmaddubsw, Vx,Hx,Wx));
                d[0x05] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubw, Pq,Qq),
                        Instr(Mnemonic.vphsubw, Vx,Hx,Wx));
                d[0x06] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubd, Pq,Qq),
                        Instr(Mnemonic.vphsubd, Vx,Hx,Wx));
                d[0x07] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubsw, Pq,Qq),
                        Instr(Mnemonic.vphsubsw, Vx,Hx,Wx));

                d[0x08] = new PrefixedDecoder(
                        Instr(Mnemonic.psignb, Pq,Qq),
                        Instr(Mnemonic.vpsignb, Vx,Hx,Wx));
                d[0x09] = new PrefixedDecoder(
                        Instr(Mnemonic.psignw, Pq,Qq),
                        Instr(Mnemonic.vpsignw, Vx,Hx,Wx));
                d[0x0A] = new PrefixedDecoder(
                        Instr(Mnemonic.psignd, Pq,Qq),
                        Instr(Mnemonic.vpsignd, Vx,Hx,Wx));
                d[0x0B] = new PrefixedDecoder(
                        Instr(Mnemonic.pmulhrsw, Pq,Qq),
                        Instr(Mnemonic.vpmulhrsw, Vx,Hx,Wx));

                d[0x0C] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vpermilps, Vx,Hx,Wx));
                d[0x0D] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vpermilpd, Vx,Hx,Wx));
                d[0x0E] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vtestps, Vx,Wx));
                d[0x0F] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vtestpd, Vx,Wx));

                    // 0F 38 10
                d[0x10] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.pblendvb, Vdq,Wdq));
                d[0x11] = s_invalid;
                d[0x12] = s_invalid;
                d[0x13] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vcvtph2ps, Vx,Wx,Ib));

                d[0x14] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.blendvps, Vdq,Wdq));
                d[0x15] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.blendvpd, Vdq,Wdq));
                d[0x16] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vpermps, Vqq,Hqq,Wqq));
                d[0x17] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vptest, Vx,Wx));

                d[0x18] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vbroadcastss, Vx,Wd));
                d[0x19] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vbroadcastsd, Vqq,Wq));
                d[0x1A] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vbroadcastf128, Vqq,Mdq));
                d[0x1B] = s_invalid;

                d[0x1C] = new PrefixedDecoder(
                        dec: Instr(Mnemonic.pabsb, Pq,Qq),
                        dec66: Instr(Mnemonic.vpabsb, Vx,Wx));
                d[0x1D] = new PrefixedDecoder(
                        dec: Instr(Mnemonic.pabsw, Pq,Qq),
                        dec66: Instr(Mnemonic.vpabsw, Vx,Wx));
                d[0x1E] = new PrefixedDecoder(
                        dec: Instr(Mnemonic.pabsd, Pq,Qq),
                        dec66: Instr(Mnemonic.vpabsd, Vx,Wx));
                d[0x1F] = s_invalid;

                    // 0F 38 20
                d[0x20] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovsxbw, Mnemonic.vpmovsxbw, Vx,Mq));
                d[0x21] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovsxbd, Mnemonic.vpmovsxbd, Vx,Md));
                d[0x22] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovsxbq, Mnemonic.vpmovsxbq, Vx,Mw));
                d[0x23] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovsxwd, Mnemonic.vpmovsxwd, Vx,Mq));

                d[0x24] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxwq, Vx,Mq));
                d[0x25] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxdq, Vx,Mq));
                d[0x26] = s_invalid;
                d[0x27] = s_invalid;

                d[0x28] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmuldq, Vx,Hx,Wx));
                d[0x29] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpcmpeqq, Vx,Hx,Wx));
                d[0x2A] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmovntdqa, Vx,Mx,Wx));
                d[0x2B] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpackusdw, Vx,Hx,Wx));

                d[0x2C] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Vx,Hx,Mx));
                d[0x2D] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Vx,Hx,Mx));
                d[0x2E] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Mx,Hx,Vx));
                d[0x2F] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Mx,Hx,Vx));

                    // 30
                d[0x30] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxbw, Vx,Mq));
                d[0x31] = new PrefixedDecoder(dec66: MemReg(
                        VexInstr(Mnemonic.pmovzxbd, Mnemonic.vpmovzxbd, Vx,Md),
                        VexInstr(Mnemonic.pmovzxbd, Mnemonic.vpmovzxbd, Vx,Ux)));
                d[0x32] = new PrefixedDecoder(dec66: MemReg(
                        VexInstr(Mnemonic.pmovzxbq, Mnemonic.vpmovzxbq, Vx,Mw),
                        VexInstr(Mnemonic.pmovzxbq, Mnemonic.vpmovzxbq, Vx,Ux)));
                d[0x33] = new PrefixedDecoder(dec66: MemReg(
                        VexInstr(Mnemonic.pmovzxwd, Mnemonic.vpmovzxwd, Vx,Mq),
                        VexInstr(Mnemonic.pmovzxwd, Mnemonic.vpmovzxwd, Vx,Ux)));

                d[0x34] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxwq, Vx,Mq));
                d[0x35] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxdq, Vx,Mq));
                d[0x36] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpermd, Vqq,Hqq,Wqq));
                d[0x37] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpcmpgtq, Vx,Hx,Wx));

                d[0x38] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpminsb, Vx,Hx,Wx));
                d[0x39] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpminsd, Vx,Hx,Wx));
                d[0x3A] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpminuw, Vx,Hx,Wx));
                d[0x3B] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpminud, Vx,Hx,Wx));

                d[0x3C] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxsb, Vx,Hx,Wx));
                d[0x3D] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxsd, Vx,Hx,Wx));
                d[0x3E] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxuw, Vx,Hx,Wx));
                d[0x3F] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxud, Vx,Hx,Wx));

                    // 40
                d[0x40] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmulld, Vx,Hx,Wx));
                d[0x41] = new PrefixedDecoder(dec66: Instr(Mnemonic.vphminposuw, Vdq,Wdq));
                d[0x42] = s_invalid;
                d[0x43] = s_invalid;
                d[0x44] = s_invalid;
                d[0x45] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vpsrlvd, Vx,Hx,Wx),
                        dec66Wide: Instr(Mnemonic.vpsrlvq, Vx,Hx,Wx));
                d[0x46] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpsravd, Vx,Hx,Wx));
                d[0x47] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vpsllvd, Vx,Hx,Wx),
                        dec66Wide: Instr(Mnemonic.vpsllvq, Vx,Hx,Wx));

                d[0x48] = s_invalid;
                d[0x49] = s_invalid;
                d[0x4A] = s_invalid;
                d[0x4B] = s_invalid;
                d[0x4C] = s_invalid;
                d[0x4D] = s_invalid;
                d[0x4E] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vrsqrt14ps, Vx, Wx),
                    dec66Wide: VexInstr(Mnemonic.vrsqrt14pd, Vx, Wx));
                d[0x4F] = s_invalid;

                    // 50
                d[0x50] = s_invalid;
                d[0x51] = s_invalid;
                d[0x52] = s_invalid;
                d[0x53] = s_invalid;
                d[0x54] = s_invalid;
                d[0x55] = s_invalid;
                d[0x56] = s_invalid;
                d[0x57] = s_invalid;

                d[0x58] = new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcastd, Vx,Wx));
                d[0x59] = new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcastq, Vx,Wx));
                d[0x5A] = new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcasti128, Vqq,Mdq));
                d[0x5B] = s_invalid;
                d[0x5C] = s_invalid;
                d[0x5D] = s_invalid;
                d[0x5E] = s_invalid;
                d[0x5F] = s_invalid;

                    // 60
                d[0x60] = s_invalid;
                d[0x61] = s_invalid;
                d[0x62] = s_invalid;
                d[0x63] = s_invalid;
                d[0x64] = s_invalid;
                d[0x65] = s_invalid;
                d[0x66] = s_invalid;
                d[0x67] = s_invalid;

                d[0x68] = s_invalid;
                d[0x69] = s_invalid;
                d[0x6A] = s_invalid;
                d[0x6B] = s_invalid;
                d[0x6C] = s_invalid;
                d[0x6D] = s_invalid;
                d[0x6E] = s_invalid;
                d[0x6F] = s_invalid;

                    // 70
                d[0x70] = s_invalid;
                d[0x71] = s_invalid;
                d[0x72] = s_invalid;
                d[0x73] = s_invalid;
                d[0x74] = s_invalid;
                d[0x75] = s_invalid;
                d[0x76] = s_invalid;
                d[0x77] = s_invalid;

                d[0x78] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpbroadcastb, Vx,Eb));
                d[0x79] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpbroadcastw, Vx,Wx));
                d[0x7A] = s_invalid;
                d[0x7B] = s_invalid;
                d[0x7C] = s_invalid;
                d[0x7D] = s_invalid;
                d[0x7E] = s_invalid;
                d[0x7F] = s_invalid;

                    // 0F 38 80
                d[0x80] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.invept, Gy,Mdq));
                d[0x81] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.invvpid, Gy,Mdq));
                d[0x82] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.invpcid, Gy,Mdq));
                d[0x83] = s_invalid;
                d[0x84] = s_invalid;
                d[0x85] = s_invalid;
                d[0x86] = s_invalid;
                d[0x87] = s_invalid;

                d[0x88] = s_invalid;
                d[0x89] = s_invalid;
                d[0x8A] = s_invalid;
                d[0x8B] = s_invalid;
                d[0X8C] = s_nyi;
                d[0x8D] = s_invalid;
                d[0x8E] = s_nyi;
                d[0x8F] = s_invalid;

                    // 90
                d[0x90] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherdd, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vgatherdq, Vx,Hx,Wx));
                d[0x91] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherqd, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vgatherqq, Vx,Hx,Wx));
                d[0x92] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherdps, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vgatherdpd, Vx,Hx,Wx));
                d[0x93] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherqps, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vgatherqpd, Vx,Hx,Wx));

                d[0x94] = s_invalid;
                d[0x95] = s_invalid;
                d[0x96] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vfmaddsub132ps, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vfmaddsub132pd, Vx,Hx,Wx));
                d[0x97] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vfmsubadd132ps, Vx,Hx,Wx),
                        dec66Wide: VexInstr(Mnemonic.vfmsubadd132pd, Vx,Hx,Wx));

                d[0x98] = s_nyi;
                d[0x99] = s_nyi;
                d[0x9A] = s_nyi;
                d[0x9B] = s_nyi;
                d[0x9C] = s_invalid;
                d[0x9D] = s_invalid;
                d[0x9E] = new PrefixedDecoder(dec66:nyi("vfmaddsub132ps/dv"));
                d[0x9F] = new PrefixedDecoder(dec66:nyi("vfmsubadd132ps/dv"));

                    // A0
                d[0xA0] = s_invalid;
                d[0xA1] = s_invalid;
                d[0xA2] = s_invalid;
                d[0xA3] = s_invalid;
                d[0xA4] = s_invalid;
                d[0xA5] = s_invalid;
                d[0xA6] = s_nyi;
                d[0xA7] = s_nyi;

                d[0xA8] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmadd213ps, Vx,Hx,Wx),
                        dec66Wide: Instr(Mnemonic.vfmadd213pd, Vx,Hx,Wx));
                d[0xA9] = s_nyi;
                d[0xAA] = s_nyi;
                d[0xAB] = s_nyi;

                d[0xAC] = s_nyi;
                d[0xAD] = s_nyi;
                d[0xAE] = s_nyi;
                d[0xAF] = s_nyi;


                    // B0
                d[0xB0] = s_invalid;
                d[0xB1] = s_invalid;
                d[0xB2] = s_invalid;
                d[0xB3] = s_invalid;
                d[0xB4] = s_invalid;
                d[0xB5] = s_invalid;
                d[0xB6] = s_nyi;
                d[0xB7] = s_nyi;

                d[0xB8] = s_nyi;
                d[0xB9] = s_nyi;
                d[0xBA] = s_nyi;
                d[0xBB] = s_nyi;

                d[0xBC] = s_nyi;
                d[0xBD] = s_nyi;
                d[0xBE] = new PrefixedDecoder(
                        s_invalid,
                        dec66:Instr(Mnemonic.vfnmsub231ps, Vx,Hx,Wx));
                d[0xBF] = new PrefixedDecoder(
                        s_invalid,
                        dec66:Instr(Mnemonic.vfnmsub231ss, Vx,Hx,Wx));

                    // 0F 38 C0
                d[0xC0] = s_invalid;
                d[0xC1] = s_invalid;
                d[0xC2] = s_invalid;
                d[0xC3] = s_invalid;
                d[0xC4] = s_invalid;
                d[0xC5] = s_invalid;
                d[0xC6] = s_invalid;
                d[0xC7] = s_invalid;

                d[0xC8] = new PrefixedDecoder(Instr(Mnemonic.sha1nexte, Vdq,Wdq));
                d[0xC9] = new PrefixedDecoder(Instr(Mnemonic.sha1msg1, Vdq,Wdq));
                d[0xCA] = new PrefixedDecoder(Instr(Mnemonic.sha1msg2, Vdq,Wdq));
                d[0xCB] = new PrefixedDecoder(Instr(Mnemonic.sha256mds2, Vdq,Wdq));
                d[0xCC] = new PrefixedDecoder(Instr(Mnemonic.sha256msg1, Vdq,Wdq));
                d[0xCD] = new PrefixedDecoder(Instr(Mnemonic.sha256msg2, Vdq,Wdq));
                d[0xCE] = s_invalid;
                d[0xCF] = s_invalid;

                    // 0F 38 D0
                d[0xD0] = s_invalid;
                d[0xD1] = s_invalid;
                d[0xD2] = s_invalid;
                d[0xD3] = s_invalid;
                d[0xD4] = s_invalid;
                d[0xD5] = s_invalid;
                d[0xD6] = s_invalid;
                d[0xD7] = s_invalid;

                d[0xD8] = s_invalid;
                d[0xD9] = s_invalid;
                d[0xDA] = s_invalid;
                d[0xDB] = new PrefixedDecoder(dec66: Instr(Mnemonic.aesimc, Vdq,Wdq));

                d[0xDC] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesenc, Mnemonic.vaesenc, Vdq,Hdq,Wdq));
                d[0xDD] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesenclast, Mnemonic.vaesenclast, Vdq,Hdq,Wdq));
                d[0xDE] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesdec, Mnemonic.vaesdec, Vdq,Hdq,Wdq));
                d[0xDF] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesdeclast, Mnemonic.vaesdeclast, Vdq,Hdq,Wdq));

                    // E0
                d[0xE0] = s_invalid;
                d[0xE1] = s_invalid;
                d[0xE2] = s_invalid;
                d[0xE3] = s_invalid;
                d[0xE4] = s_invalid;
                d[0xE5] = s_invalid;
                d[0xE6] = s_invalid;
                d[0xE7] = s_invalid;

                d[0xE8] = s_invalid;
                d[0xE9] = s_invalid;
                d[0xEA] = s_invalid;
                d[0xEB] = s_invalid;
                d[0xEC] = s_invalid;
                d[0xED] = s_invalid;
                d[0xEE] = s_invalid;
                d[0xEF] = s_invalid;

                    // F0
                d[0xF0] = Instr(Mnemonic.movbe, Gv,Ev);
                d[0xF1] = Instr(Mnemonic.movbe, Ev,Gv);
                d[0xF2] = new PrefixedDecoder(
                        dec:VexInstr(Mnemonic.andn, Gy,By,Ey),
                        dec66:s_nyi);
                d[0xF3] = new GroupDecoder(Grp17);

                d[0xF4] = s_invalid;
                d[0xF5] = VexInstr(s_invalid, new PrefixedDecoder(
                        dec:   Instr(Mnemonic.bzhi, Gy,Ey,By),
                        decF3: Instr(Mnemonic.pext, Gy,By,Ey),
                        decF2: Instr(Mnemonic.pdep, Gy,By,Ey)));
                d[0xF6] = new PrefixedDecoder(
                        dec66:Instr(Mnemonic.adcx, Gy,Ey),
                        decF3:Instr(Mnemonic.adox, Gy,Ey),
                        decF2:VexInstr(Mnemonic.mulx, Gy,By,rDX,Ey));
                d[0xF7] = VexInstr(s_invalid, new PrefixedDecoder(
                        dec:   Instr(Mnemonic.bextr, Gy,Ey,By),
                        dec66: Instr(Mnemonic.shlx, Gy,Ey,By),
                        decF3: Instr(Mnemonic.sarx, Gy,Ey,By),
                        decF2: Instr(Mnemonic.shrx, Gy,Ey,By)));

                d[0xF8] = s_invalid;
                d[0xF9] = s_invalid;
                d[0xFA] = s_invalid;
                d[0xFB] = s_invalid;
                d[0xFC] = s_invalid;
                d[0xFD] = s_invalid;
                d[0xFE] = s_invalid;
                d[0xFF] = s_invalid;
            }
        }
    }
}