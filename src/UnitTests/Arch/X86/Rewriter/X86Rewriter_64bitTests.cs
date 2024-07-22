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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    [TestFixture]
    public class X86Rewriter_64bitTests : Arch.RewriterTestBase
    {
        private readonly X86ArchitectureFlat64 arch;
        private readonly Address addrBase;

        public X86Rewriter_64bitTests()
        {
            var sc = CreateServiceContainer();
            arch = new X86ArchitectureFlat64(sc, "x86-protected-64", new Dictionary<string, object>());
            addrBase = Address.Ptr64(0x140000000ul);

        }
        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrBase;


        [Test(Description = "Intel and AMD state that if you set the low 32-bits of a register in 64-bit mode, they are zero extended.")]
        public void X86rw_64bit_clearHighBits()
        {
            Given_HexString("33C0");
            AssertCode(
               "0|L--|0000000140000000(2): 5 instructions",
               "1|L--|eax = eax ^ eax",
               "2|L--|rax = CONVERT(eax, word32, uint64)",
               "3|L--|SZ = cond(eax)",
               "4|L--|O = false",
               "5|L--|C = false");
        }

        [Test]
        public void X86Rw_add_64_rex()
        {
            Given_HexString("48 05 44 EB 24 C4"); // add rax,0xffffffffc424eb44<64>
            AssertCode(
                "0|L--|0000000140000000(6): 2 instructions",
                "1|L--|rax = rax + 0xFFFFFFFFC424EB44<64>",
                "2|L--|SCZO = cond(rax)");
        }

        [Test]
        public void X86Rw_vaddpd()
        {
            Given_HexString("C5FD588570FFFFFF");
            AssertCode(     // vaddpd   ymm0,ymm0,[rbp-90h]
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|ymm0 = __simd_fadd<real64[4]>(ymm0, Mem0[rbp - 144<i64>:(arr real64 4)])");
        }

        [Test]
        public void X86rw_addps()
        {
            Given_HexString("0F 58 0D FB B0 00 00");
            AssertCode( // addps xmm1,[0000000000415EF4]
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm1 = __simd_fadd<real32[4]>(xmm1, Mem0[0x000000014000B102<p64>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_vaddsd()
        {
            Given_HexString("C5FB58C0");   // vaddsd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = SLICE(xmm0, real64, 0) + SLICE(xmm0, real64, 0)",
                "2|L--|xmm0 = SEQ(0<64>, v4)");
        }

        [Test]
        public void X86rw_addss()
        {
            Given_HexString("F30F580DFBB00000");
            AssertCode( //addss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v4 = SLICE(xmm1, real32, 0) + Mem0[0x000000014000B103<p64>:real32]",
               "2|L--|v5 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v5, v4)");
        }

        [Test]
        public void X86Rw_aesenc()
        {
            Given_HexString("660F38DCC0");
            AssertCode(     // vaesenc	xmm0,xmm0
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm0 = __aesenc(xmm0, xmm0)");
        }

        [Test]
        public void X86Rw_aesenclast()
        {
            Given_HexString("660F38DD04C2");
            AssertCode(     // aesenclast	xmm0,[rdx+rax*8]
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm0 = __aesenclast(xmm0, Mem0[rdx + rax * 8<64>:word128])");
        }

        [Test]
        public void X86Rw_aeskeygen()
        {
            Given_HexString("660F3ADFC600");
            AssertCode(     // aeskeygen	xmm0,xmm6,0h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm0 = __aeskeygen(xmm6, 0<8>)");
        }

        [Test]
        public void X86Rw_and_64_rex()
        {
            Given_HexString("48 25 44 EB 24 C4"); // and rax,0xffffffffc424eb44<64>
            //$REVIEW: and's should be unsigned masks no?
            AssertCode(
                "0|L--|0000000140000000(6): 4 instructions",
                "1|L--|rax = rax & 0xFFFFFFFFC424EB44<64>",
                "2|L--|SZ = cond(rax)",
                "3|L--|O = false",
                "4|L--|C = false");
        }

        [Test]
        public void X86rw_andn()
        {
            Given_HexString("C4E278F2CB"); //andn\tecx,eax,ebx
            AssertCode(
                "0|L--|0000000140000000(5): 5 instructions",
                "1|L--|ecx = ebx & ~eax",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)",
                "3|L--|SZ = cond(ecx)",
                "4|L--|O = false",
                "5|L--|C = false");
        }

        [Test]
        public void X86Rw_andnpd()
        {
            Given_HexString("660F55D9");
            AssertCode(     // andnpd	xmm3,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm3 = __andnpd<word128>(xmm3, xmm1)");
        }

        [Test]
        public void X86Rw_bextr()
        {
            Given_HexString("C4C228F7F9");
            AssertCode(     // bextr	edi,r9d,r10d
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|edi = __bextr<word32>(r9d, r10d)",
                "2|L--|Z = edi == 0<32>");
        }

        [Test]
        public void X86Rw_vblendw()
        {
            Given_HexString("66440F3A0ED8F0");
            AssertCode(     // vblendw	xmm11,xmm0,0F0h
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm11 = __packed_blend<word16[8]>(xmm11, xmm0, 0xF0<8>)");
        }

        [Test]
        public void X86Rw_blsi()
        {
            Given_HexString("C4C298F3DB");
            AssertCode(     // blsi	r12,r11
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r12 = __blsi<word64>(r11)",
                "2|L--|Z = r12 == 0<64>",
                "3|L--|S = r12 <= 0<64>",
                "4|L--|C = r11 == 0<64>");
        }

        [Test]
        public void X86Rw_blsmsk()
        {
            Given_HexString("C4C290F3D2");
            AssertCode(     // blsmsk	r13,r10
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r13 = __blsmsk<word64>(r10)",
                "2|L--|Z = r13 == 0<64>",
                "3|L--|S = r13 <= 0<64>",
                "4|L--|C = r10 == 0<64>");
        }

        [Test]
        public void X86Rw_blsr()
        {
            Given_HexString("C4E2A8F3CA");
            AssertCode(     // blsr r10,rdx
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r10 = __blsr<word64>(rdx)",
                "2|L--|Z = r10 == 0<64>",
                "3|L--|S = r10 <= 0<64>",
                "4|L--|C = rdx == 0<64>");
        }

        [Test]
        public void X86Rw_bzhi()
        {
            Given_HexString("C46290F5EB");
            AssertCode(     // bzhi	r13,rbx,r13
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r13 = __bzhi<word64>(rbx, r13)");
        }

        [Test]
        public void X86Rw_cdqe()
        {
            Given_HexString("4898"); // cdqe
            AssertCode(
                "0|L--|0000000140000000(2): 1 instructions",
                "1|L--|rax = CONVERT(eax, int32, int64)");
        }

        [Test]
        public void X86Rw_cldemote()
        {
            Given_HexString("0F1C78CB");
            AssertCode(     // cldemote	byte ptr [rax-35h]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|__cache_line_demote(&Mem0[rax - 53<i64>:byte])");
        }

        [Test]
        public void X86Rw_cmpeqsd()
        {
            Given_HexString("F20FC244241800");
            AssertCode(     // cmpeqsd	xmm0,[esp+18]
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = SLICE(xmm0, real64, 0) == Mem0[rsp + 24<i64>:real64] ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cmplesd()
        {
            Given_HexString("F20FC2E806");
            AssertCode(     // cmplesd	xmm5,6h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm5 = SLICE(xmm5, real64, 0) > SLICE(xmm0, real64, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_vcmpnltsd()
        {
            Given_HexString("C5FBC21DF616020005");
            AssertCode(     // vcmpnltsd	xmm3,xmm0,double ptr [rip+216F6h]
                "0|L--|0000000140000000(9): 1 instructions",
                "1|L--|xmm3 = SLICE(xmm0, real64, 0) < Mem0[0x00000001400216FF<p64>:real64] ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cmpnltsd()
        {
            Given_HexString("F20FC21D1619020005");
            AssertCode(     // cmpnltsd	xmm3,double ptr [rip+21916h]
                "0|L--|0000000140000000(9): 1 instructions",
                "1|L--|xmm3 = SLICE(xmm3, real64, 0) < Mem0[0x000000014002191F<p64>:real64] ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cmpss()
        {
            Given_HexString("F30FC2C805");
            AssertCode(     // cmpnltss	xmm1,xmm0,5h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = SLICE(xmm1, real32, 0) >= SLICE(xmm0, real32, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cqo()
        {
            Given_HexString("4899"); // cqo
            AssertCode(
                "0|L--|0000000140000000(2): 1 instructions",
                "1|L--|rdx_rax = CONVERT(rax, int64, int128)");
        }

        [Test]
        public void X86Rw_cvtdq2pd()
        {
            Given_HexString("F30FE6C9");
            AssertCode(     // cvtdq2pd	xmm1,xmm1
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|xmm1 = __cvtdq2pd<int32[4],real64[4]>(v4)");
        }

        [Test]
        public void X86Rw_cvtpd2ps()
        {
            Given_HexString("660F5AC0");
            AssertCode(     // cvtpd2ps	xmm0,xmm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = __cvtpd2ps<real64[2],real32[2]>(v4)",
                "3|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v5)");
        }

        [Test]
        public void X86Rw_cvtpd2dq()
        {
            Given_HexString("F20FE6E7");
            AssertCode(     // cvtpd2dq	xmm4,xmm7
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm7",
                "2|L--|v6 = __cvtpd2dq<real64[2],int32[2]>(v4)",
                "3|L--|xmm4 = SEQ(SLICE(xmm4, word64, 64), v6)");
        }

        [Test]
        public void X86rw_cvtpi2ps()
        {
            Given_HexString("49 0F 2A C5");
            AssertCode( // cvtpi2ps xmm0, mm5
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = CONVERT(SLICE(mm5, int32, 0), int32, real32)",
               "2|L--|v5 = CONVERT(SLICE(mm5, int32, 32), int32, real32)",
               "3|L--|xmm0 = SEQ(v5, v4)");
        }

        [Test]
        public void X86Rw_cvtps2dq()
        {
            Given_HexString("660F5BC0");
            AssertCode(     // cvtps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __cvtps2dq<real32[4],int32[4]>(v4)");
        }

        [Test]
        public void X86Rw_cvtsd2ss()
        {
            Given_HexString("F2480F5AC0");	// cvtsd2ss	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = CONVERT(SLICE(xmm0, real64, 0), real64, real32)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v4)");
        }

        [Test]
        public void X86rw_cvtsi2ss()
        {
            Given_HexString("F3480F2AC0");
            AssertCode(     // "cvtsi2ss\txmm0,rax", 
               "0|L--|0000000140000000(5): 2 instructions",
               "1|L--|v5 = CONVERT(rax, int64, real32)",
               "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v5)");
        }

        [Test]
        public void X86rw_vcvtsi2sd()
        {
            Given_HexString("C4E1FB2AC2"); // vcvtsi2sd\txmm0,xmm0,rdx
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v5 = CONVERT(rdx, int64, real64)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v5)");
        }

        [Test]
        public void X86rw_vcvtsi2ss()
        {
            Given_HexString("C4E1FA2AC0");     // vcvtsi2ss\txmm0,xmm0,rax
            AssertCode(
             "0|L--|0000000140000000(5): 2 instructions",
             "1|L--|v5 = CONVERT(rax, int64, real32)",
             "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v5)");
        }


        [Test]
        public void X86Rw_cvtss2sd()
        {
            Given_HexString("F3480F5A0DB5473200");	// cvtss2sd	xmm1,dword ptr [rip+003247B5]
            AssertCode(
                "0|L--|0000000140000000(9): 2 instructions",
                "1|L--|v4 = CONVERT(Mem0[0x00000001403247BE<p64>:real32], real32, real64)",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word64, 64), v4)");
        }

        [Test]
        public void X86Rw_cvtss2si()
        {
            Given_HexString("F3480F2D5010");	// cvtss2si	rdx,dword ptr [rax+10]
            AssertCode(
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|rdx = CONVERT(Mem0[rax + 16<i64>:real32], real32, int64)");
        }

        [Test]
        public void X86Rw_cvttpd2dq()
        {
            Given_HexString("66450FE6C9");
            AssertCode(     // cvttpd2dq	xmm9,xmm9
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v4 = xmm9", //$REVIEW: unnecessary copy
                "2|L--|v5 = __cvttpd2dq<real64[2],int32[2]>(v4)",
                "3|L--|xmm9 = SEQ(SLICE(xmm9, word64, 64), v5)");
        }

        [Test]
        public void X86Rw_cvttps2dq()
        {
            Given_HexString("F30F5BC0");
            AssertCode(     // cvttps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __cvttps2dq<real32[4],int32[4]>(v4)");
        }



        [Test]
        public void X86rw_cvttss2si()
        {
            Given_HexString("F3 4C 0F 2C F8");
            AssertCode(     // cvttss2si r15, xmm0
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|r15 = CONVERT(SLICE(xmm0, real32, 0), real32, int64)");
        }

        [Test]
        public void X86Rw_divpd()
        {
            Given_HexString("660F5EF1");
            AssertCode(     // divpd	xmm6,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm6 = __divp<real64[2]>(xmm6, xmm1)");
        }

        [Test]
        public void X86rw_divsd()
        {
            Given_HexString("F2 0F 5E C1");
            AssertCode( // divsd xmm0,xmm1
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = SLICE(xmm0, real64, 0) / SLICE(xmm1, real64, 0)",
               "2|L--|v6 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_divss()
        {
            Given_HexString("F3 0F 5E C1");
            AssertCode(     // divss xmm0, xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = SLICE(xmm0, real32, 0) / SLICE(xmm1, real32, 0)",
                "2|L--|v6 = SLICE(xmm0, word96, 32)",
                "3|L--|xmm0 = SEQ(v6, v4)");
        }

        [Test]
        public void X86Rw_vdivss()
        {
            Given_HexString("C5 FB 5E 45 E8");
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = SLICE(xmm0, real64, 0) / Mem0[rbp - 24<i64>:real64]",
                "2|L--|xmm0 = SEQ(0<64>, v4)");
        }

        [Test]
        public void X86Rw_enterw()
        {
            Given_HexString("66C8A50AE2");
            AssertCode(     // enterw	0AA5h,0E2h
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|rsp = rsp - 2<i64>",
                "2|L--|Mem0[rsp:word16] = bp",
                "3|L--|bp = SLICE(rsp, word16, 0)",
                "4|L--|rsp = rsp - 2665<i64>");     //$TODO: odd numbers are suspicious
        }

        [Test]
        public void X86rw_fmul_load()
        {
            Given_HexString("D80D899F0000");
            AssertCode(     // fmul
              "0|L--|0000000140000000(6): 1 instructions",
              "1|L--|ST[Top:real64] = ST[Top:real64] * CONVERT(Mem0[0x0000000140009F8F<p64>:real32], real32, real64)");
        }

        [Test]
        public void X86Rw_fxrstor()
        {
            Given_HexString("0FAE0B");
            AssertCode(     // fxrstor
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__fxrstor()");
        }

        [Test]
        public void X86Rw_fxsave()
        {
            Given_HexString("0FAE00");
            AssertCode(     // fxsave
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__fxsave()");
        }


        [Test(Description = "RET n instructions with an odd n are unlikely to be valid.")]
        public void X86rw_invalid_ret_n()
        {
            Given_HexString("C2 01 00");
            AssertCode(     // ret 0001
                "0|---|0000000140000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_jkz()
        {
            Given_HexString("C5C584F9FFFFFF");
            AssertCode(     // jkz\tk7 10000h
                "0|T--|0000000140000000(7): 1 instructions",
                "1|T--|if (k7 == 0<64>) branch 0000000140000000");
        }

        [Test]
        public void X86Rw_jknz()
        {
            Given_HexString("C5C585F9FFFFFF");
            AssertCode(     // jknz\tk7 10000h
                "0|T--|0000000140000000(7): 1 instructions",
                "1|T--|if (k7 != 0<64>) branch 0000000140000000");
        }

        [Test]
        public void X86Rw_lea_short_dst()
        {
            Given_HexString("8D 4C 09 01"); // lea ecx,[rcx+rcx+01]
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = SLICE(rcx + 1<i64> + rcx, word32, 0)",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_16bit_dst()
        {
            Given_HexString("668D03"); // lea\tax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|ax = SLICE(rbx, word16, 0)");
        }

        [Test]
        public void X86Rw_lea_32bit_dst_64bit_src()
        {
            Given_HexString("8D03"); // lea\teax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|eax = SLICE(rbx, word32, 0)",
                "2|L--|rax = CONVERT(eax, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_32bit_dst_32bit_src()
        {
            Given_HexString("678D03"); // lea\teax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(3): 2 instructions",
                "1|L--|eax = ebx",
                "2|L--|rax = CONVERT(eax, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_64bit_dst_64bit_src()
        {
            Given_HexString("488D03"); // lea\teax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|rax = rbx");
        }

        [Test]
        public void X86Rw_lea_64bit_dst_32bit_src()
        {
            Given_HexString("67488D03"); // lea\trax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rax = CONVERT(ebx, word32, word64)");
        }


        [Test]
        public void X86Rw_lea_16bit_dst_32bit_src()
        {
            Given_HexString("67668D03"); // lea\tax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ax = SLICE(ebx, word16, 0)");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg()
        {
            Given_HexString("8D 4C 09 01");    // lea ecx,[rcx + rcx + 1]
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = SLICE(rcx + 1<i64> + rcx, word32, 0)",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg_negative_offset()
        {
            Given_HexString("8D 14 7D F8 FF FF FF ");
            AssertCode(
                "0|L--|0000000140000000(7): 2 instructions",
                "1|L--|edx = SLICE(-8<i64> + rdi * 2<64>, word32, 0)",
                "2|L--|rdx = CONVERT(edx, word32, uint64)");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg_2()
        {
            Given_HexString("8D 14 BD 00 00 00 00");
            AssertCode(
                "0|L--|0000000140000000(7): 2 instructions",
                "1|L--|edx = SLICE(0<i64> + rdi * 4<64>, word32, 0)",
                "2|L--|rdx = CONVERT(edx, word32, uint64)");
        }

        [Test]
        public void X86rw_lss_64bit()
        {
            Given_HexString("480FB206");
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ss_rax = Mem0[rsi + 0<64>:segptr80]");
        }

        [Test]
        public void X86Rw_lzcnt()
        {
            Given_HexString("F30FBDCE");
            AssertCode(     // lzcnt	ecx,esi
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = __lzcnt<word32>(esi)",
                "2|L--|Z = ecx == 0<32>");
        }

        [Test]
        public void X86Rw_maxsd_legacy()
        {
            Given_HexString("F20F5FD0");
            AssertCode(     // maxsd	xmm2,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v5 = max(SLICE(xmm2, real64, 0), SLICE(xmm0, real64, 0))",
                "2|L--|xmm2 = SEQ(SLICE(xmm2, word64, 64), v5)");
        }

        [Test]
        public void X86Rw_minpd()
        {
            Given_HexString("660F5D4242"); // minpd\txmm0,[rdx+42]
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm0 = __simd_min<real64[2]>(xmm0, Mem0[rdx + 66<i64>:(arr real64 2)])");
        }

        [Test]
        public void X86Rw_vminpd()
        {
            Given_HexString("C401E95DFB");
            AssertCode(     // vminpd	xmm15,xmm2,xmm11
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm15 = __simd_min<real64[2]>(xmm2, xmm11)");
        }

        [Test]
        public void X86Rw_mov_imm_64()
        {
            Given_HexString("48B88A7A6A5A4A3A2A1A");
            AssertCode( // mov rax,1A2A3A4A5A6A7A8Ah
                "0|L--|0000000140000000(10): 1 instructions",
                "1|L--|rax = 0x1A2A3A4A5A6A7A8A<64>");
        }


        [Test]
        public void X86rw_mov_rip_relative()
        {
            Given_HexString("498b0500001000"); // "mov\trax,qword ptr [rip+10000000]",
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|rax = Mem0[0x0000000140100007<p64>:word64]");
        }

        [Test]
        public void X86rw_movaps()
        {
            Given_HexString("0F280540120000"); //  movaps xmm0,[rip+00001240]
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = Mem0[0x0000000140001247<p64>:word128]");
        }

        [Test]
        public void X86rw_vmovaps_xmm()
        {
            Given_HexString("C5F92800"); // vmovaps xmm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = Mem0[rax:word128]");
        }

        [Test]
        public void X86rw_vmovaps_ymm()
        {
            Given_HexString("C5FD2800"); // vmovaps ymm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm0 = Mem0[rax:word256]");
        }

        [Test]
        public void X86Rw_vmovd()
        {
            Given_HexString("C4C1F96ECE");
            AssertCode(     // vmovd	xmm1,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = CONVERT(r14, word64, word128)");
        }

        [Test]
        public void X86Rw_vmovddup()
        {
            Given_HexString("C5FF12D5");
            AssertCode(     // vmovddup	ymm2,ymm5
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm2 = __movddup<word256>(ymm5)");
        }

        [Test]
        public void X86rw_vmovd_slicing()
        {
            Given_HexString("C4C1F97EC0");
            AssertCode(     // vmovd	r8,xmm0
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|r8 = SLICE(xmm0, word64, 0)");
        }

        [Test]
        public void X86Rw_movdqa()
        {
            Given_HexString("660F6F4DC0");
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 64<i64>:word128]");
        }

        [Test]
        public void X86Rw_vmovdqa()
        {
            Given_HexString("C5F96F4DC0");
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 64<i64>:word128]");
        }

        [Test]
        public void X86Rw_movdqu()
        {
            Given_HexString("F30F7F01");
            AssertCode(     // movdqu	[rcx],xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|Mem0[rcx:word128] = xmm0");
        }

        [Test]
        public void X86rw_movsd()
        {
            Given_HexString("F20F1045E0");   // movsd xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<64>, Mem0[rbp - 32<i64>:real64])");
            Given_HexString("F20F1145E0");   // movsd dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 32<i64>:real64] = SLICE(xmm0, real64, 0)");
            Given_HexString("F20F10C3");   // movsd xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), SLICE(xmm3, real64, 0))");
        }


        [Test]
        public void X86rw_vmovsd()
        {
            Given_HexString("C5FB1101"); // vmovsd double ptr[rcx], xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|Mem0[rcx:real64] = SLICE(xmm0, real64, 0)");
        }

        [Test]
        public void X86rw_movss()
        {
            Given_HexString("F30F1045E0");   // movss xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<96>, Mem0[rbp - 32<i64>:real32])");
            Given_HexString("F30F1145E0");   // movss dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 32<i64>:real32] = SLICE(xmm0, real32, 0)");
            Given_HexString("F30F10C3");         // movss xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), SLICE(xmm3, real32, 0))");
        }

        [Test]
        public void X86rw_vmovss_load()
        {
            Given_HexString("C5FA100551030000"); // vmovss txmm0,dword ptr [rip+00000351]
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm0 = SEQ(0<96>, Mem0[0x0000000140000359<p64>:real32])");
        }

        [Test]
        public void X86rw_vmovss_store()
        {
            Given_HexString("C5FA11852CFFFFFF"); // vmovss dword ptr [rbp-0xd4], xmm0
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|Mem0[rbp - 212<i64>:real32] = SLICE(xmm0, real32, 0)");
        }

        [Test]
        public void X86rw_movsxd()
        {
            Given_HexString("4863483c"); // "movsxd\trcx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rcx = CONVERT(Mem0[rax + 60<i64>:word32], word32, int64)");
        }

        [Test]
        public void X86rw_movsxd_dword()
        {
            Given_HexString("63483c"); // "movsxd\tecx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|ecx = Mem0[rax + 60<i64>:word32]");
        }


        [Test]
        public void X86rw_movups()
        {
            Given_HexString("0F1045E0");   // movups xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 32<i64>:word128]");

            Given_HexString("660F1045E0");   // movupd xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 32<i64>:word128]");

            Given_HexString("0F11442420"); // movups\t[rsp+20],xmm0, 
            AssertCode(
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|Mem0[rsp + 32<i64>:word128] = xmm0");
        }

        [Test]
        public void X86Rw_vmovups()
        {
            Given_HexString("C4C17C10049B");
            AssertCode(     // vmovups	ymm0,[r11+rbx*4]
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|ymm0 = Mem0[r11 + rbx * 4<64>:word256]");
        }

        [Test]
        public void X86Rw_vmptrld()
        {
            Given_HexString("0FC732");
            AssertCode(     // vmptrld	qword ptr [edx]
                "0|S--|0000000140000000(3): 1 instructions",
                "1|L--|__vmptrld(Mem0[rdx:word64])");
        }

        [Test]
        public void X86rw_mulps()
        {
            Given_HexString("0F 59 4A 08");
            AssertCode(     // mulps xmm1,[rdx+08]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm1 = __mulp<real32[4]>(xmm1, Mem0[rdx + 8<i64>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_mulsd()
        {
            Given_HexString("F2 0F 59 05 92 AD 00 00 ");
            AssertCode(     // mulsd xmm0,qword ptr[rip + ad92]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v4 = SLICE(xmm0, real64, 0) * Mem0[0x000000014000AD9A<p64>:real64]",
               "2|L--|v5 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v5, v4)");
        }

        [Test]
        public void X86rw_mulss()
        {
            Given_HexString("F3 0F 59 D8");
            AssertCode(     // mulss xmm3, xmm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = SLICE(xmm3, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|v6 = SLICE(xmm3, word96, 32)",
                "3|L--|xmm3 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_vmulss()
        {
            Given_HexString("C5 BE 59 D8");
            AssertCode(     // mulss xmm3, xmm8, xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = SLICE(xmm8, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|xmm3 = SEQ(0<96>, v4)");
        }

        [Test]
        public void X86Rw_mulx()
        {
            Given_HexString("C442FBF6E2");
            AssertCode(     // mulx	r12,rax,rdx,r10
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r12_rax = rdx *u128 r10");
        }

        [Test]
        public void X86Rw_vorpd()
        {
            Given_HexString("C5F956442450");
            AssertCode(     // vorpd	xmm0,xmm0,[rsp+50h]
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm0 = __orp<real64[2]>(xmm0, Mem0[rsp + 80<i64>:(arr real64 2)])");
        }

        [Test]
        public void X86Rw_packsswb()
        {
            Given_HexString("660F63C1");
            AssertCode(     // packsswb	xmm0,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __packss<int16[8],int8[16]>(xmm0, xmm1)");
        }

        [Test]
        public void X86Rw_vpaddb()
        {
            Given_HexString("C591FCC1");
            AssertCode(     // vpaddb	xmm0,xmm13,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __simd_add<byte[16]>(xmm13, xmm1)");
        }

        [Test]
        public void X86Rw_vpbroadcastb()
        {
            Given_HexString("C4E27D78C0");
            AssertCode(     // vpbroadcastb	ymm0,al
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|ymm0 = __pbroadcast<byte,byte[32]>(SLICE(ymm0, byte, 0))");
        }

        [Test]
        public void X86Rw_vpcmpeqb()
        {
            Given_HexString("C5ED74D8");
            AssertCode(     // vpcmpeqb	ymm3,ymm2,ymm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm3 = __pcmpeq<byte[32]>(ymm2, ymm0)");
        }

        [Test]
        public void X86Rw_vpcmpeqw()
        {
            Given_HexString("C5ED750A");
            AssertCode(     // vpcmpeqw	ymm1,ymm2,[rdx]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm1 = __pcmpeq<word16[16]>(ymm2, Mem0[rdx:(arr word16 16)])");
        }

        [Test]
        [Ignore("whoa...")]
        public void X86Rw_pcmpistri()
        {
            Given_HexString("66410F3A630140");
            AssertCode(     // pcmpistri	xmm0,[r9],40h
                "0|L--|000000013F179336(7): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void X86Rw_pdep()
        {
            Given_HexString("C4E2E3F5F2");
            AssertCode(     // pdep	rsi,rbx,rdx
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|rsi = __pdep<word64>(rbx, rdx)");
        }

        [Test]
        public void X86Rw_pext()
        {
            Given_HexString("C4C282F5D6");
            AssertCode(     // pext	rdx,r15,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|rdx = __pext<word64>(r15, r14)");
        }

        [Test]
        public void X86Rw_pextrb()
        {
            Given_HexString("660F3A14C303");
            AssertCode(     // pextrb	dword ptr [rcx+10h],xmm4,3h
                "0|L--|0000000140000000(6): 2 instructions",
                "1|L--|ebx = SLICE(xmm0, byte, 24)",
                "2|L--|rbx = CONVERT(ebx, word32, uint64)");
        }

        [Test]
        public void X86Rw_pextrd()
        {
            Given_HexString("660F3A16611003");
            AssertCode(     // pextrd	dword ptr [rcx+10h],xmm4,3h
                "0|L--|0000000140000000(7): 2 instructions",
                "1|L--|v5 = SLICE(xmm4, word32, 24)",
                "2|L--|Mem0[rcx + 16<i64>:word32] = v5");
        }

        [Test]
        public void X86Rw_vpinsrq()
        {
            Given_HexString("C4E3F122C101");
            AssertCode(     // vpinsrq	xmm0,xmm1,rcx,1h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,word64>(xmm1, rcx, 1<8>)");
        }

        [Test]
        public void X86Rw_pmaddubsw()
        {
            Given_HexString("660F3804D6");
            AssertCode(     // pmaddubsw	xmm2,xmm6
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v5 = xmm2",
                "2|L--|v6 = xmm6",
                "3|L--|xmm2 = __pmaddubsw<uint8[16],int8[16],int16[8]>(v5, v6)");
        }

        [Test]
        public void X86Rw_vpmaxub()
        {
            Given_HexString("C5C9DEC9");
            AssertCode(     // vpmaxub	xmm1,xmm6,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm1 = __pmaxu<uint8[16]>(xmm6, xmm1)");
        }

        [Test]
        public void X86Rw_vpmovmskb()
        {
            Given_HexString("C5FDD7CA");
            AssertCode(     // vpmovmskb	ecx,ymm2
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v5 = __pmovmskb<word256>(ymm2)",
                "2|L--|ecx = SEQ(SLICE(ecx, word24, 8), v5)");
        }
        [Test]
        public void X86Rw_popcnt()
        {
            Given_HexString("F3480FB8C7");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|rax = __popcnt<word64,int64>(rdi)",
                "2|L--|Z = rdi == 0<64>");
        }

        [Test]
        public void X86Rw_popcnt_mem()
        {
            Given_HexString("F3480FB800");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v4 = Mem0[rax:word64]",
                "2|L--|rax = __popcnt<word64,int64>(v4)",
                "3|L--|Z = v4 == 0<64>");
        }


        [Test]
        public void X86Rw_vpor()
        {
            Given_HexString("C501EB8BE809E800");
            AssertCode(     // vpor	xmm9,xmm15,[rbx+0E809E8h]
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm9 = xmm15 | Mem0[rbx + 15206888<i64>:word128]");
        }

        [Test]
        public void X86rw_prefetch()
        {
            Given_HexString("410F1808"); // prefetch
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|__prefetcht0<ptr64>(Mem0[r8:byte])");
        }

        [Test]
        public void X86Rw_vpshufd()
        {
            Given_HexString("C5D570F081");
            AssertCode(     // vpshufd	ymm6,ymm0,81h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|ymm6 = __pshuf<word32[8]>(ymm6, ymm0, 0x81<8>)");
        }

        [Test]
        public void X86Rw_pslldq()
        {
            Given_HexString("660F73FB01");
            AssertCode(     // pslldq	xmm3,1h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm3",  //$TODO unnecessary copy
                "2|L--|xmm3 = __psll<word128[1]>(v4, 1<8>)");
        }

        [Test]
        public void X86Rw_vpsllw()
        {
            Given_HexString("660F71F508");
            AssertCode(     // vpsllw	xmm5,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm5",
                "2|L--|xmm5 = __psll<word16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_vpsrad()
        {
            Given_HexString("660F72E203");
            AssertCode(     // vpsrad	xmm2,3h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm2",
                "2|L--|xmm2 = __psra<int32[4]>(v4, 3<8>)");
        }

        [Test]
        public void X86Rw_vpsraw()
        {
            Given_HexString("660F71E008");
            AssertCode(     // vpsraw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __psra<int16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_psrldq()
        {
            Given_HexString("660F73DA08");
            AssertCode(     // psrldq	xmm2,8h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm2 = __psrldq<word128[1]>(xmm2, 64<i32>)");
        }

        [Test]
        public void X86Rw_vpsrlw()
        {
            Given_HexString("660F71D008");
            AssertCode(     // vpsrlw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __psrl<word16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_vpsubsw()
        {
            Given_HexString("C501E92F");
            AssertCode(     // vpsubsw	xmm13,xmm15,[rdi]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm13 = __psubs<int16[8]>(xmm15, Mem0[rdi:(arr int16 8)])");
        }

        [Test]
        public void X86Rw_punpcklqdq()
        {
            Given_HexString("660F6CC0");
            AssertCode(     // punpcklqdq	xmm0,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklqdq<word128>(xmm0, xmm0)");
        }

        [Test]
        public void X86Rw_punpckhqdq()
        {
            Given_HexString("660F6DC1");
            AssertCode(     // punpckhqdq	xmm0,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __punpckhqdq<word128>(xmm0, xmm1)");
        }

        [Test]
        public void X86Rw_vpunpckhdq()
        {
            Given_HexString("C5396A700F");
            AssertCode(     // vpunpckhdq       xmm14,xmm8,[rax+0Fh]
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm14 = __punpckhdq<word128>(xmm8, Mem0[rax + 15<i64>:word128])");
        }

        [Test]
        public void X86Rw_push_64()
        {
            Given_HexString("66 51");
            AssertCode(
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 2<i64>",
                "2|L--|Mem0[rsp:word16] = cx");
        }

        [Test]
        public void X86rw_push_immediate_32bit()
        {
            Given_HexString("6AC2");
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 4<i64>",
                "2|L--|Mem0[rsp:word32] = 0xFFFFFFC2<32>");
        }

        [Test]
        public void X86rw_push_immediate_64bit()
        {
            Given_HexString("486AC2");
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(3): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = 0xFFFFFFFFFFFFFFC2<64>");
        }

        [Test]
        public void X86rw_push_register()
        {
            Given_HexString("53");
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(1): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = rbx");
        }

        [Test]
        public void X86rw_push_memoryload()
        {
            Given_HexString("FF75E0");
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v5 = Mem0[rbp - 32<i64>:word64]",
                "2|L--|rsp = rsp - 8<i64>",
                "3|L--|Mem0[rsp:word64] = v5");
        }

        [Test]
        public void X86Rw_pushw()
        {
            Given_HexString("66683412");
            AssertCode(     // pushw	1234h
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|rsp = rsp - 2<i64>",
                "2|L--|Mem0[rsp:word16] = 0x1234<16>");
        }

        [Test]
        public void X86Rw_vpxor()
        {
            Given_HexString("C501EFC0");
            AssertCode(     // vpxor xmm8,xmm15,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm8 = __pxor<word128>(xmm15, xmm0)");
        }

        [Test]
        public void X86Rw_vpxor_same_register()
        {
            Given_HexString("C579EFC0");
            AssertCode(     // vpxor xmm8,xmm0,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm8 = 0<128>");
        }

        [Test]
        public void X86rw_vxorpd()
        {
            Given_HexString("C5F957C0");   // vxorpd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86rw_64_vxorpd_mem_256()
        {
            Given_HexString("C5FD5709");   // vxorpd\tymm1,ymm0,[rcx]
            AssertCode(
             "0|L--|0000000140000000(4): 1 instructions",
             "1|L--|ymm1 = __xorp<word64[4]>(ymm0, Mem0[rcx:(arr word64 4)])");
        }

        [Test]
        public void X86Rw_64_vrcpps()
        {
            Given_HexString("C5E8532D0100488B");
            AssertCode(     // vrcpps   xmm5,[rip-74B7FFFFh]
                "0|L--|0000000140000000(8): 2 instructions",
                "1|L--|v4 = Mem0[0x00000000CB480009<p64>:word128]",
                "2|L--|xmm5 = __rcpp<real32[4]>(v4)");
        }

        [Test]
        public void X86rw_64_repne()
        {
            Given_HexString("F348A5");   // "rep\tmovsd"
            AssertCode(
                 "0|L--|0000000140000000(3): 5 instructions",
                 "1|L--|size = rcx *u 8<64>",
                 "2|L--|memcpy(rdi, rsi, size)",
                 "3|L--|rcx = 0<64>",
                 "4|L--|rsi = rsi + size",
                 "5|L--|rdi = rdi + size");
        }

        [Test]
        public void X86Rw_roundss()
        {
            Given_HexString("66 0F 3A 0A C3 03");
            AssertCode(     // roundss xmm0,xmm3,3h
                "0|L--|0000000140000000(6): 2 instructions",
                "1|L--|v5 = truncf(SLICE(xmm3, real32, 0))",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v5)");
        }

        [Test]
        public void X86Rw_rdpid()
        {
            Given_Bytes(0xF3, 0x0F, 0xC7, 0xF9);
            AssertCode(     // rdpid\tecx",
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rcx = __rdpid<word64>()");
        }

        [Test]
        public void X86Rw_rorx()
        {
            Given_HexString("C443FBF0D602");
            AssertCode(     // rorx	r10,r14,2h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|r10 = __ror<word64,byte>(r14, 2<8>)");
        }

        [Test]
        public void X86Rw_sarx()
        {
            Given_HexString("C462D2F7C1");
            AssertCode(     // sarx	r8,rcx,rbp
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r8 = rcx >> rbp");
        }

        [Test]
        public void X86Rw_sha1rnds4()
        {
            Given_HexString("440F3ACCD000");
            AssertCode(     // sha1rnds4	xmm10,xmm0,0h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm10 = __sha1rnds4(xmm0, 0<8>)");
        }

        [Test]
        public void X86Rw_sha256mds2()
        {
            Given_HexString("0F38CB00");
            AssertCode(     // sha256mds2	xmm0,[rax]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __sha256mds2(xmm0, Mem0[rax:word128])");
        }

        [Test]
        public void X86Rw_sha256msg1()
        {
            Given_HexString("0F38CCCA");
            AssertCode(     // sha256msg1	xmm1,xmm2
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm1 = __sha256msg1(xmm1, xmm2)");
        }

        [Test]
        public void X86Rw_shlx()
        {
            Given_HexString("C44289F7C6");
            AssertCode(     // shlx	r8,r14,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r8 = r14 << r14");
        }

        [Test]
        public void X86Rw_shrx()
        {
            Given_HexString("C4C23BF7ED");
            AssertCode(     // shrx	ebp,r13d,r8d
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|ebp = r13d >>u r8d",
                "2|L--|rbp = CONVERT(ebp, word32, uint64)");
        }

        [Test]
        public void X86Rw_sqrtpd()
        {
            Given_HexString("660F51E4");
            AssertCode(     // sqrtpd	xmm4,xmm4
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm4",
                "2|L--|xmm4 = __simd_sqrt<real64[2]>(v4)");
        }

        [Test]
        public void X86Rw_sqrtsd()
        {
            Given_HexString("F20F51C0");	// sqrtsd	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = sqrt(SLICE(xmm0, real64, 0))",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v4)");
        }

        [Test]
        public void X86Rw_sqrtss()
        {
            Given_HexString("F30F51C8");
            AssertCode(     // sqrtss	xmm1,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v5 = fsqrt(SLICE(xmm0, real32, 0))",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word96, 32), v5)");
        }

        [Test]
        public void X86rw_sub_immediate_dword()
        {
            Given_HexString("4881EC08050000"); // "sub\trsp,+00000508", 
            AssertCode(
                 "0|L--|0000000140000000(7): 2 instructions",
                 "1|L--|rsp = rsp - 0x508<64>",
                 "2|L--|SCZO = cond(rsp)");
        }

        [Test]
        public void X86rw_subps()
        {
            Given_HexString("0F 5C 05 61 AA 00 00");
            AssertCode( // subps xmm0,[0000000000415F0C]
               "0|L--|0000000140000000(7): 1 instructions",
               "1|L--|xmm0 = __simd_fsub<real32[4]>(xmm0, Mem0[0x000000014000AA68<p64>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_subss()
        {
            Given_HexString("F30F5CCD");
            AssertCode(     // subss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = SLICE(xmm1, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|v6 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_syscall()
        {
            Given_HexString("0F05");    // syscall
            AssertCode(
                "0|T--|0000000140000000(2): 1 instructions",
                "1|L--|__syscall()");
        }


        [Test]
        public void X86rw_sysret()
        {
            Given_HexString("0F07");    // sysret
            AssertCode(
                "0|R--|0000000140000000(2): 2 instructions",
                "1|L--|__sysret()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void X86rw_vsubss()
        {
            Given_HexString("C5BE5CCD");
            AssertCode(     // vsubss xmm1,xmm8,xmm5
               "0|L--|0000000140000000(4): 2 instructions",
               "1|L--|v4 = SLICE(xmm8, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|xmm1 = SEQ(0<96>, v4)");
        }

        [Test]
        public void X86Rw_vucomisd()
        {
            Given_HexString("C4C1C12E9D94557D01");
            AssertCode(     // vucomisd	xmm3,double ptr [r13d+17D5594h]
                "0|L--|0000000140000000(9): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm3, real64, 0) - Mem0[r13 + 24991124<i64>:real64])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_ucomisd()
        {
            Given_HexString("660F2E052DB10000");
            AssertCode( // ucomisd\txmm0,qword ptr [rip+0000B12D]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real64, 0) - Mem0[0x000000014000B135<p64>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_ucomiss()
        {
            Given_HexString("0F2E052DB10000");
            AssertCode( // ucomiss\txmm0,dword ptr [rip+0000B12D]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[0x000000014000B134<p64>:real32])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86Rw_tzcnt()
        {
            Given_HexString("F30FBCCA");
            AssertCode(     // tzcnt	ecx,edx
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = __tzcnt<word32>(edx)",
                "2|L--|Z = ecx == 0<32>");
        }

        [Test]
        public void X86Rw_wrpkru()
        {
            Given_HexString("0F01EF");
            AssertCode(     // wrpkru
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__wrpkru(eax)");
        }

        [Test]
        public void X86rw_xabort()
        {
            Given_HexString("44C6F842");
            AssertCode(     // xabort
                "0|H--|0000000140000000(4): 1 instructions",
                "1|H--|__xabort(0x42<8>)");
        }

        [Test]
        public void X86rw_xorps()
        {
            Given_HexString("0F57C3"); // xorps\txmm0,xmm3
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|xmm0 = __xorp<word32[4]>(xmm0, xmm3)");
        }

        [Test]
        public void X86rw_xorps_same_register()
        {
            Given_HexString("0F57C0"); // xorps\txmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86Rw_vzeroupper()
        {
            Given_HexString("C5F877");
            AssertCode(     // vzeroupper
                "0|L--|0000000140000000(3): 16 instructions",
                "1|L--|ymm0 = CONVERT(xmm0, word128, word256)",
                "2|L--|ymm1 = CONVERT(xmm1, word128, word256)",
                "3|L--|ymm2 = CONVERT(xmm2, word128, word256)",
                "4|L--|ymm3 = CONVERT(xmm3, word128, word256)",
                "5|L--|ymm4 = CONVERT(xmm4, word128, word256)",
                "6|L--|ymm5 = CONVERT(xmm5, word128, word256)",
                "7|L--|ymm6 = CONVERT(xmm6, word128, word256)",
                "8|L--|ymm7 = CONVERT(xmm7, word128, word256)",
                "9|L--|ymm8 = CONVERT(xmm8, word128, word256)",
                "10|L--|ymm9 = CONVERT(xmm9, word128, word256)",
                "11|L--|ymm10 = CONVERT(xmm10, word128, word256)",
                "12|L--|ymm11 = CONVERT(xmm11, word128, word256)",
                "13|L--|ymm12 = CONVERT(xmm12, word128, word256)",
                "14|L--|ymm13 = CONVERT(xmm13, word128, word256)",
                "15|L--|ymm14 = CONVERT(xmm14, word128, word256)",
                "16|L--|ymm15 = CONVERT(xmm15, word128, word256)");
        }





    }
}
