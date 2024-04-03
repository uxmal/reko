#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
                        VexInstr(Mnemonic.phaddw, Mnemonic.vphaddw, Vx, Hx,Wx));
                d[0x02] = new PrefixedDecoder(
                        Instr(Mnemonic.phaddd, Pq,Qq),
                        VexInstr(Mnemonic.phaddd, Mnemonic.vphaddd, Vx, Hx,Wx));
                d[0x03] = new PrefixedDecoder(
                        Instr(Mnemonic.phaddsw, Pq,Qq),
                        VexInstr(Mnemonic.phaddsw, Mnemonic.vphaddsw, Vx, Hx,Wx));

                d[0x04] = new PrefixedDecoder(
                        Instr(Mnemonic.pmaddubsw, Pq,Qq),
                        VexInstr(Mnemonic.pmaddubsw, Mnemonic.vpmaddubsw, Vx,Hx,Wx));
                d[0x05] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubw, Pq,Qq),
                        VexInstr(Mnemonic.phsubw, Mnemonic.vphsubw, Vx, Hx,Wx));
                d[0x06] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubd, Pq,Qq),
                        VexInstr(Mnemonic.phsubd, Mnemonic.vphsubd, Vx, Hx,Wx));
                d[0x07] = new PrefixedDecoder(
                        Instr(Mnemonic.phsubsw, Pq,Qq),
                        VexInstr(Mnemonic.phsubsw, Mnemonic.vphsubsw, Vx, Hx,Wx));

                d[0x08] = new PrefixedDecoder(
                        Instr(Mnemonic.psignb, Pq,Qq),
                        VexInstr(Mnemonic.psignb, Mnemonic.vpsignb, Vx,Hx,Wx));
                d[0x09] = new PrefixedDecoder(
                        Instr(Mnemonic.psignw, Pq,Qq),
                        VexInstr(Mnemonic.psignw, Mnemonic.vpsignw, Vx,Hx,Wx));
                d[0x0A] = new PrefixedDecoder(
                        Instr(Mnemonic.psignd, Pq,Qq),
                        VexInstr(Mnemonic.psignd, Mnemonic.vpsignd, Vx,Hx,Wx));
                d[0x0B] = new PrefixedDecoder(
                        Instr(Mnemonic.pmulhrsw, Pq,Qq),
                        VexInstr(Mnemonic.pmulhrsw, Mnemonic.vpmulhrsw, Vx, Hx,Wx));

                d[0x0C] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vpermilps, Vx,Hx,WBx_d));
                d[0x0D] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.vpermilpd, Vx,Hx,WBx_q));
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
                    EvexInstr(
                        s_invalid,
                        VexLong(
                            Instr(Mnemonic.vcvtph2ps, Vx,Wq),
                            Instr(Mnemonic.vcvtph2ps, Vqq,Wdq)),
                        VexLong(
                            Instr(Mnemonic.vcvtph2ps, Vx,Wq),
                            Instr(Mnemonic.vcvtph2ps, Vx,Wdq),
                            Instr(Mnemonic.vcvtph2ps, Vx,Wqq))));

                d[0x14] = new PrefixedDecoder(
                    s_invalid,
                    dec66:EvexInstr(
                        Instr(Mnemonic.blendvps, Vdq, Wdq, Reg(Registers.xmm0)),
                        Instr(Mnemonic.blendvps, Vdq, Wdq),
                        Instr(Mnemonic.vprorvd, Vx, Hx, Wx)),
                    dec66Wide:EvexInstr(
                        s_nyi,
                        s_nyi,
                        Instr(Mnemonic.vprorvq, Vx, Hx, Wx)));
                d[0x15] = new PrefixedDecoder(
                        s_invalid,
                        Instr(Mnemonic.blendvpd, Vdq,Wdq, Reg(Registers.xmm0)));
                d[0x16] = new PrefixedDecoder(
                        s_invalid,
                        dec66: VexLong(
                            Instr(Mnemonic.vpermps, Vx,Hx,WBx_d),
                            Instr(Mnemonic.vpermps, Vx,Hx,WBx_d),
                            Instr(Mnemonic.vpermps, Vx,Hx,WBx_d)),
                        dec66Wide: VexLong(
                            Instr(Mnemonic.vpermpd, Vx, Hx, WBx_d),
                            Instr(Mnemonic.vpermpd, Vx, Hx, WBx_d),
                            Instr(Mnemonic.vpermpd, Vx, Hx, WBx_d)));
                d[0x17] = new PrefixedDecoder(
                        s_invalid,
                        VexInstr(Mnemonic.ptest, Mnemonic.vptest, Vx,Wx));

                d[0x18] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vbroadcastss, Vx,Wd));
                d[0x19] = new PrefixedDecoder(
                        dec66: EvexInstr(
                            s_invalid,
                            Instr(Mnemonic.vbroadcastsd, Vqq, Wq),
                            Instr(Mnemonic.vbroadcastsd, Vx, Wq)));
                d[0x1A] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vbroadcastf128, Vqq,Mdq));
                d[0x1B] = s_invalid;

                d[0x1C] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsb, Pq,Qq),
                    dec66: VexInstr(Mnemonic.pabsb, Mnemonic.vpabsb, Vx, Wx));
                d[0x1D] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsw, Pq,Qq),
                    dec66: VexInstr(Mnemonic.pabsw, Mnemonic.vpabsw, Vx,Wx));
                d[0x1E] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsd, Pq,Qq),
                    dec66: VexInstr(Mnemonic.pabsd, Mnemonic.vpabsd, Vx, WBx_d),
                    dec66Wide: EvexInstr(
                        Instr(Mnemonic.pabsd, Vx,Wx),
                        Instr(Mnemonic.vpabsd, Vx,Wx),
                        Instr(Mnemonic.vpabsq, Vx, WBx_q)));
                d[0x1F] = s_invalid;

                    // 0F 38 20
                d[0x20] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.pmovsxbw, Mnemonic.vpmovsxbw, Vx,Md),
                    dec66Wide: VexInstr(Mnemonic.vpmovsxbw, Vx,Mdq));
                d[0x21] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovsxbd, Mnemonic.vpmovsxbd, Vx,Md));
                d[0x22] = new PrefixedDecoder(dec66: VexInstr(
                    Instr(Mnemonic.pmovsxbq, Vx,Mw),
                    VexLong(
                        Instr(Mnemonic.vpmovsxbq, Vx, Mw),
                        Instr(Mnemonic.vpmovsxbq, Vx, Md),
                        Instr(Mnemonic.vpmovsxbq, Vx, Md))));
                d[0x23] = new PrefixedDecoder(dec66: EvexInstr(
                    Instr(Mnemonic.pmovsxwd, Vx,Mq),
                    Instr(Mnemonic.vpmovsxwd, Vx,Mq),
                    VexLong(
                        Instr(Mnemonic.vpmovsxwd, Vqq,Mq),
                        Instr(Mnemonic.vpmovsxwd, Vqq,Mq),
                        Instr(Mnemonic.vpmovsxwd, Vx,Mqq))));

                d[0x24] = new PrefixedDecoder(dec66: VexInstr(
                    Instr(Mnemonic.pmovsxwq, Vx, Md),
                    VexLong(
                        Instr(Mnemonic.vpmovsxwq, Vx, Md),
                        Instr(Mnemonic.vpmovsxwq, Vx, Mq),
                        Instr(Mnemonic.vpmovsxwq, Vx, Md))));
                d[0x25] = new PrefixedDecoder(dec66:
                    EvexInstr(
                        Instr(Mnemonic.pmovsxdq,  Vx, Mq),
                        Instr(Mnemonic.vpmovsxdq, Vx, Mq),
                        Instr(Mnemonic.vpmovsxdq, Vx, Mq)));
                d[0x26] = s_invalid;
                d[0x27] = s_invalid;

                d[0x28] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmuldq, Mnemonic.vpmuldq, Vx, Hx,Wx));
                d[0x29] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpcmpeqq, Vkx,Hx,WBx_q));
                d[0x2A] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmovntdqa, Vx,Mx));
                d[0x2B] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpackusdw, Vx,Hx,Wx));

                d[0x2C] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Vx,Hx,Mx));
                d[0x2D] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Vx,Hx,Mx));
                d[0x2E] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Mx,Hx,Vx));
                d[0x2F] = new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Mx,Hx,Vx));

                    // 30
                d[0x30] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovzxbw, Mnemonic.vpmovzxbw, Vx, Mq));
                d[0x31] = new PrefixedDecoder(dec66:
                    EvexInstr(
                        Instr(Mnemonic.pmovzxbd, Vx, Wd),
                        VexLong(
                            Instr(Mnemonic.vpmovzxbd, Vx, Wd),
                            Instr(Mnemonic.vpmovzxbd, Vx, Wq)),
                        VexLong(
                            Instr(Mnemonic.vpmovzxbd, Vx, Wd),
                            MemReg(
                                Instr(Mnemonic.vpmovzxbd, Vx, Md),
                                Instr(Mnemonic.vpmovzxbd, Vx, Wdq)),
                            Instr(Mnemonic.vpmovzxbd, Vx, Wdq))));
                d[0x32] = new PrefixedDecoder(dec66:
                    EvexInstr(
                        Instr(Mnemonic.pmovzxbq, Vx, Ww),
                        VexLong(
                            Instr(Mnemonic.vpmovzxbq, Vx, Ww),
                            MemReg(
                                Instr(Mnemonic.vpmovzxbq, Vx, Mw),
                                Instr(Mnemonic.vpmovzxbq, Vx, Wdq))),
                        VexLong(
                            Instr(Mnemonic.vpmovzxbq, Vx, Ww),
                            MemReg(
                                Instr(Mnemonic.vpmovzxbq, Vx, Md),
                                Instr(Mnemonic.vpmovzxbq, Vx, Wdq)),
                            MemReg(
                                Instr(Mnemonic.vpmovzxbq, Vx, Mq),
                                Instr(Mnemonic.vpmovzxbq, Vx, Wdq)))));
                d[0x33] = new PrefixedDecoder(dec66: MemReg(
                        VexInstr(Mnemonic.pmovzxwd, Mnemonic.vpmovzxwd, Vx,Mq),
                        VexInstr(Mnemonic.pmovzxwd, Mnemonic.vpmovzxwd, Vx,Ux)));

                d[0x34] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxwq, Vx,Mq));
                d[0x35] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmovzxdq, Mnemonic.vpmovzxdq, Vx, Mq));
                d[0x36] = new PrefixedDecoder(dec66: Instr(Mnemonic.vpermd, Vqq,Hqq,Wqq));
                d[0x37] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpgtq, Mnemonic.vpcmpgtq, Vkx,Hx,WBx_q));

                d[0x38] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pminsb, Mnemonic.vpminsb, Vx,Hx,WBx_b));
                d[0x39] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.pminsd, Mnemonic.vpminsd, Vx, Hx, WBx_d),
                    dec66Wide: VexInstr(Mnemonic.vpminsq, Mnemonic.vpminsq, Vx, Hx, WBx_q));
                d[0x3A] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pminuw, Mnemonic.vpminuw, Vx,Hx,WBx_w));
                d[0x3B] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pminud, Mnemonic.vpminud, Vx,Hx,WBx_d));

                d[0x3C] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmaxsb, Mnemonic.vpmaxsb, Vx, Hx,WBx_b));
                d[0x3D] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.pmaxsd, Mnemonic.vpmaxsd, Vx,Hx,WBx_d),
                    dec66Wide: Instr(Mnemonic.vpmaxsq, Vx,Hx,WBx_q));
                d[0x3E] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pmaxuw, Mnemonic.vpmaxuw, Vx, Hx,WBx_w));
                d[0x3F] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.pmaxud, Mnemonic.vpmaxud, Vx, Hx,WBx_d),
                    dec66Wide: VexInstr(Mnemonic.pmaxuq, Mnemonic.vpmaxuq, Vx, Hx, WBx_q));

                // 40
                d[0x40] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.pmulld, Mnemonic.vpmulld, Vx, Hx, WBx_d),
                    dec66Wide: Instr(Mnemonic.vpmullq, Vx, Hx, WBx_q));
                d[0x41] = new PrefixedDecoder(dec66: Instr(Mnemonic.vphminposuw, Vdq,Wdq));
                d[0x42] = s_invalid;
                d[0x43] = s_invalid;
                d[0x44] = s_invalid;
                d[0x45] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vpsrlvd, Vx,Hx,WBx_d),
                        dec66Wide: Instr(Mnemonic.vpsrlvq, Vx,Hx,WBx_q));
                d[0x46] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vpsravd, Vx, Hx, WBx_d),
                    dec66Wide: Instr(Mnemonic.vpsravq, Vx, Hx, WBx_q));
                d[0x47] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vpsllvd, Vx,Hx,WBx_d),
                        dec66Wide: Instr(Mnemonic.vpsllvq, Vx,Hx,WBx_q));

                d[0x48] = s_invalid;
                d[0x49] = s_invalid;
                d[0x4A] = s_invalid;
                d[0x4B] = s_invalid;
                d[0x4C] = s_invalid;
                d[0x4D] = s_invalid;
                d[0x4E] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vrsqrt14ps, Vx, WBx_d),
                    dec66Wide: VexInstr(Mnemonic.vrsqrt14pd, Vx, WBx_q));
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

                d[0x58] = new PrefixedDecoder(dec66:
                    EvexInstr(
                        s_invalid,
                        VexLong(
                            Instr(Mnemonic.vpbroadcastd, Vx, Wd),
                            Instr(Mnemonic.vpbroadcastd, Vx, Wd)),
                        VexLong(
                            Instr(Mnemonic.vpbroadcastd, Vx, Wd),
                            Instr(Mnemonic.vpbroadcastd, Vx, Wd),
                            Instr(Mnemonic.vpbroadcastd, Vx, Wd))));
                d[0x59] = new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcastq, Vx,Wq));
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

                d[0x78] = new PrefixedDecoder(dec66: MemReg(
                    Instr(Mnemonic.vpbroadcastb, Vx,Mb),
                    Instr(Mnemonic.vpbroadcastb, Vx,Wx)));
                d[0x79] = new PrefixedDecoder(dec66: MemReg(
                    Instr(Mnemonic.vpbroadcastw, Vx,Mw),
                    Instr(Mnemonic.vpbroadcastw, Vx,Wx)));
                d[0x7A] = s_invalid;
                d[0x7B] = s_invalid;
                d[0x7C] = s_invalid;
                d[0x7D] = s_invalid;
                d[0x7E] = s_invalid;
                d[0x7F] = new PrefixedDecoder(
                    dec66: EvexInstr(
                        s_invalid,
                        s_invalid,
                        Instr(Mnemonic.vpermt2ps, Vx,Hx,WBx_d)),
                    dec66Wide: EvexInstr(
                        s_invalid,
                        s_invalid,
                        Instr(Mnemonic.vpermt2pd, Vx,Hx,WBx_q)));

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
                d[0X8C] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vpmaskmovd, Vx, Hx, Mx),
                    dec66Wide: VexInstr(Mnemonic.vpmaskmovq, Vx, Hx, Mx));

                d[0x8D] = s_invalid;
                d[0x8E] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vpmaskmovd, Mx, Vx, Hx),
                    dec66Wide: VexInstr(Mnemonic.vpmaskmovq, Mx, Vx, Hx));
                d[0x8F] = s_invalid;

                    // 90
                d[0x90] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherdd, Vx,Hx,WBx_d, RnSae),
                        dec66Wide: VexInstr(Mnemonic.vgatherdq, Vx,Hx,WBx_q, RnSae));
                d[0x91] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherqd, Vx,Hx,WBx_d, RnSae),
                        dec66Wide: VexInstr(Mnemonic.vgatherqq, Vx,Hx,WBx_q, RnSae));
                d[0x92] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherdps, Vx,WBx_d, RnSae),
                        dec66Wide: VexInstr(Mnemonic.vgatherdpd, Vx,WBx_q, RnSae));
                d[0x93] = new PrefixedDecoder(
                        dec66: VexInstr(Mnemonic.vgatherqps, Vx,Hx,WBx_d, RnSae),
                        dec66Wide: VexInstr(Mnemonic.vgatherqpd, Vx,Hx,WBx_q, RnSae));

                d[0x94] = s_invalid;
                d[0x95] = s_invalid;
                d[0x96] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmaddsub132ps, Vx,Hx,WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmaddsub132pd, Vx,Hx,WBx_q, RnSae));
                d[0x97] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmsubadd132ps, Vx,Hx,WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmsubadd132pd, Vx,Hx,WBx_q, RnSae));

                d[0x98] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmadd132ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmadd132pd, Vx, Hx, WBx_q, RnSae));
                d[0x99] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmadd132ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmadd132ss, Vx, Hx, Wsd, RnSae));
                d[0x9A] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmsub132ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmsub132pd, Vx, Hx, WBx_q, RnSae));
                d[0x9B] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmsub132ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmsub132sd, Vx, Hx, Wsd, RnSae));
                d[0x9C] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmadd132ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmadd132pd, Vx, Hx, WBx_q, RnSae));
                d[0x9D] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmadd132ss, Vss, Hss, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmadd132sd, Vsd, Hsd, Wsd, RnSae));
                d[0x9E] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmsub132ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmsub132pd, Vx, Hx, WBx_q, RnSae));
                d[0x9F] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmsub132ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmsub132sd, Vx, Hx, Wsd, RnSae));

                // A0
                d[0xA0] = s_invalid;
                d[0xA1] = s_invalid;
                d[0xA2] = s_invalid;
                d[0xA3] = s_invalid;
                d[0xA4] = s_invalid;
                d[0xA5] = s_invalid;
                d[0xA6] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmaddsub213ps, Vx, Hx, WBx_d, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmaddsub213pd, Vx, Hx, WBx_q, RnSae));
                d[0xA7] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmsubadd213ps, Vx, Hx, Wss, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmsubadd213pd, Vx, Hx, Wsd, RnSae));

                d[0xA8] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmadd213ps, Vx,Hx,WBx_d, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmadd213pd, Vx,Hx,WBx_q, RnSae));
                d[0xA9] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmadd213ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmadd213sd, Vx, Hx, Wsd, RnSae));
                d[0xAA] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmsub213ps, Vx, Hx, WBx_d, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmsub213pd, Vx, Hx, WBx_q, RnSae));
                d[0xAB] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmsub213ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmsub213sd, Vx, Hx, Wsd, RnSae));
                d[0xAC] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfnmadd213ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: Instr(Mnemonic.vfnmadd213pd, Vx, Hx, WBx_q, RnSae));
                d[0xAD] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmadd213ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmadd213sd, Vx, Hx, Wsd, RnSae));
                d[0xAE] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfnmsub213ps, Vx, Hx, WBx_d, RnSae),
                    dec66Wide: Instr(Mnemonic.vfnmsub213pd, Vx, Hx, WBx_q, RnSae));
                d[0xAF] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfnmsub213ss, Vx, Hx, Wss, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfnmsub213sd, Vx, Hx, Wsd, RnSae));

                    // B0
                d[0xB0] = s_invalid;
                d[0xB1] = s_invalid;
                d[0xB2] = s_invalid;
                d[0xB3] = s_invalid;
                d[0xB4] = s_invalid;
                d[0xB5] = s_invalid;
                d[0xB6] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmaddsub231ps, Vx, Hx, Wx, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmaddsub231pd, Vx, Hx, Wx, RnSae));
                d[0xB7] = new PrefixedDecoder(
                        dec66: Instr(Mnemonic.vfmsubadd231ps, Vx, Hx, Wx, RnSae),
                        dec66Wide: Instr(Mnemonic.vfmsubadd231pd, Vx, Hx, Wx, RnSae));

                d[0xB8] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfmadd231ps, Vx, Hx, Wx, RnSae),
                    dec66Wide: Instr(Mnemonic.vfmadd231pd, Vx, Hx, Wx, RnSae));
                d[0xB9] = new PrefixedDecoder(
                    dec66: VexInstr(Mnemonic.vfmadd231ss, Vx, Hx, Wx, RnSae),
                    dec66Wide: VexInstr(Mnemonic.vfmadd231sd, Vx, Hx, WBx_d,RnSae));
                d[0xBA] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfmsub231ps, Vx, Hx, Wx, RnSae),
                    dec66Wide: Instr(Mnemonic.vfmsub231pd, Vx, Hx, Wx, RnSae));
                d[0xBB] = new PrefixedDecoder(
                    dec66: VexInstr(
                        s_invalid,
                        MemReg(
                            Instr(Mnemonic.vfmsub231ss, Vdq, Hdq, Wd, RnSae),
                            Instr(Mnemonic.vfmsub231ss, Vdq, Hdq, Wdq, RnSae))),
                    dec66Wide: VexInstr(
                        s_invalid,
                        MemReg(
                            Instr(Mnemonic.vfmsub231sd, Vdq, Hdq, Wq, RnSae),
                            Instr(Mnemonic.vfmsub231sd, Vdq, Hdq, Wdq, RnSae))));
                d[0xBC] = new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfnmadd231ps, Vx, Hx, Wx, RnSae),
                    dec66Wide: Instr(Mnemonic.vfnmadd231pd, Vx, Hx, Wx, RnSae));
                d[0xBD] = new PrefixedDecoder(
                    dec66: VexInstr(
                        s_invalid,
                        MemReg(
                            Instr(Mnemonic.vfnmadd231ss, Vdq, Hdq, Wd),
                            Instr(Mnemonic.vfnmadd231ss, Vdq, Hdq, Wdq))),
                    dec66Wide: VexInstr(
                        s_invalid,
                        MemReg(
                            Instr(Mnemonic.vfnmadd231sd, Vdq, Hdq, Wq),
                            Instr(Mnemonic.vfnmadd231sd, Vdq, Hdq, Wqq))));
                d[0xBE] = new PrefixedDecoder(
                    dec: s_invalid,
                    dec66:EvexInstr(
                        s_invalid,
                        Instr(Mnemonic.vfnmsub231ps, Vx,Hx,Wx),
                        Instr(Mnemonic.vfnmsub231ps, Vx,Hx,WBx_d, RnSae)),
                    dec66Wide: EvexInstr(
                        s_invalid,
                        Instr(Mnemonic.vfnmsub231pd, Vx, Hx, Wx),
                        Instr(Mnemonic.vfnmsub231pd, Vx, Hx, WBx_q, RnSae)));
                d[0xBF] = new PrefixedDecoder(
                        s_invalid,
                        dec66:Instr(Mnemonic.vfnmsub231ss, Vx,Hx,Wss, RnSae));

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
                d[0xDB] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesimc, Mnemonic.vaesimc, Vdq,Wdq));

                d[0xDC] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesenc, Mnemonic.vaesenc, Vdq,Hdq,Wdq));
                d[0xDD] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aesenclast, Mnemonic.vaesenclast, Vx,Hx,Wx));
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
                d[0xF0] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.movbe, Gv, Ev),
                    decF2: Instr(Mnemonic.crc32, Gy, Eb));
                d[0xF1] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.movbe, Ev, Gv),
                    decF2: Instr(Mnemonic.crc32, Gy, Ev));
                d[0xF2] = new PrefixedDecoder(
                        dec:VexInstr(Mnemonic.andn, Gy,By,Ey));
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