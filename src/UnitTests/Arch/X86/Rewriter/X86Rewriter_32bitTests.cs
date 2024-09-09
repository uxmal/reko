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
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    [TestFixture]
    partial class X86Rewriter_32bitTests : Arch.RewriterTestBase
    {
        private readonly IntelArchitecture arch32;
        private readonly Address baseAddr32;
        private IntelArchitecture arch;
        private Address baseAddr;
        private ServiceContainer sc;

        public X86Rewriter_32bitTests()
        {
            var sc = CreateServiceContainer();
            arch32 = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            baseAddr32 = Address.Ptr32(0x10000000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddr;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemService());
        }

        private X86Assembler Create32bitAssembler()
        {
            arch = arch32;
            baseAddr = baseAddr32;
            var asm = new X86Assembler(arch, baseAddr32, new List<ImageSymbol>());
            return asm;
        }

        private void Run32bitTest(Action<X86Assembler> fn)
        {
            var m = Create32bitAssembler();
            fn(m);
            Given_MemoryArea(m.GetImage().SegmentMap.Segments.Values.First().MemoryArea);
        }

        private void Run32bitTest(string hexBytes)
        {
            arch = arch32;
            Given_MemoryArea(new ByteMemoryArea(baseAddr32, BytePattern.FromHexBytes(hexBytes)));
        }


        [Test]
        public void X86rw_Call32Bit()
        {
            Run32bitTest(m =>
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|T--|10000000(5): 1 instructions",
                "1|T--|call 10000000 (4)");
        }

        [Test]
        public void X86rw_bswap()
        {
            Run32bitTest(m =>
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap<word32>(ebx)");
        }

        [Test]
        public void X86Rw_crc32()
        {
            Run32bitTest("F20F38F1FD");
            AssertCode(     // crc32	edi,ebp
                "0|L--|10000000(5): 1 instructions",
                "1|L--|edi = __crc32<word32,word32>(edi, ebp)");
        }

        [Test]
        public void X86rw_Neg_mem()
        {
            Run32bitTest("F719");   // neg dword ptr [ecx]
            AssertCode(
                "0|L--|10000000(2): 5 instructions",
                "1|L--|v4 = Mem0[ecx:word32]",
                "2|L--|C = v4 != 0<32>",
                "3|L--|v6 = -v4",
                "4|L--|Mem0[ecx:word32] = v6",
                "5|L--|SZO = cond(v6)");
        }

        [Test]
        public void X86rw_Fild()
        {
            Run32bitTest(m =>
            {
                m.Fild(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ebx + 4<i32>:int32], int32, real64)");
        }

        [Test]
        public void X86rw_fstp()
        {
            Run32bitTest(m =>
            {
                m.Fstp(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[ebx + 4<i32>:real32] = CONVERT(ST[Top:real64], real64, real32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_bsr()
        {
            Run32bitTest(m =>
            {
                m.Bsr(m.ecx, m.eax);
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Z = eax == 0<32>",
                "2|L--|ecx = __bsr<word32>(eax)");
        }

        [Test]
        public void X86rw_Xlat32()
        {
            Run32bitTest(m =>
            {
                m.Xlat();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|al = Mem0[ebx + CONVERT(al, uint8, uint32):byte]");
        }

        [Test]
        public void X86Rw_xsaveopt()
        {
            Run32bitTest("0FAE74D8BE");
            AssertCode(     // xsaveopt dword ptr [eax+ebx*8-42h]
                "0|L--|10000000(5): 1 instructions",
                "1|L--|__xsaveopt(edx_eax, &Mem0[eax - 66<i32> + ebx * 8<32>:word32])");
        }

        [Test]
        public void X86rw_Aaa()
        {
            Run32bitTest(m =>
            {
                m.Aaa();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|C = __aaa(al, ah, &al, &ah)");
        }

        [Test]
        public void X86rw_Aam()
        {
            Run32bitTest(m =>
            {
                m.Aam();
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ax = __aam(al)");
        }

        [Test]
        public void X86Rw_cmpneqsd()
        {
            Run32bitTest("F20FC244241804");
            AssertCode(     // cmpneqsd	xmm0,double ptr [esp+18h]
                "0|L--|10000000(7): 1 instructions",
                "1|L--|xmm0 = SLICE(xmm0, real64, 0) != Mem0[esp + 24<i32>:real64] ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86rw_Cmpsb()
        {
            Run32bitTest(m =>
            {
                m.Cmpsb();
            });
            AssertCode(
                "0|L--|10000000(1): 3 instructions",
                "1|L--|SCZO = cond(Mem0[esi:byte] - Mem0[edi:byte])",
                "2|L--|esi = esi + 1<i32>",
                "3|L--|edi = edi + 1<i32>");
        }

        [Test]
        public void X86Rw_getsec()
        {
            Run32bitTest("0F37");	// getsec
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_ebx = __getsec(eax)");
        }

        [Test]
        public void X86rw_Hlt()
        {
            Run32bitTest(m =>
            {
                m.Hlt();
            });
            AssertCode(
                "0|H--|10000000(1): 1 instructions",
                "1|H--|__halt()");
        }

        [Test]
        public void X86rw_std_Lodsw()
        {
            Run32bitTest(m =>
            {
                m.Std();
                m.Lodsw();
                m.Cld();
                m.Lodsw();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|D = 8<32>",
                "2|L--|10000001(2): 2 instructions",
                "3|L--|ax = Mem0[esi:word16]",
                "4|L--|esi = esi - 2<i32>",
                "5|L--|10000003(1): 1 instructions",
                "6|L--|D = 0<32>",
                "7|L--|10000004(2): 2 instructions",
                "8|L--|ax = Mem0[esi:word16]",
                "9|L--|esi = esi + 2<i32>");
        }

        [Test]
        public void X86rw_cmovz()
        {
            Run32bitTest("0F44C8");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000003",
                "2|L--|ecx = eax");
        }

        [Test]
        public void X86rw_cmp_Ev_Ib()
        {
            Run32bitTest("833FFF");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF<32>)");
        }

        [Test]
        public void X86Rw_rcl_1()
        {
            Run32bitTest("D0 D8");      // ror al,1h
            AssertCode(
                "0|L--|10000000(2): 3 instructions",
                "1|L--|v4 = C",
                "2|L--|C = (al & 1<8> << 1<8> - 1<8>) != 0<8>",
                "3|L--|al = __rcr<byte,byte>(al, 1<8>, v4)");
        }

        [Test]
        public void X86Rw_rol_1()
        {
            Run32bitTest("D0 C0");      // ror al,1h
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|C = (al & 1<8> << 8<8> - 1<8>) != 0<8>",
                "2|L--|al = __rol<byte,byte>(al, 1<8>)");
        }

        [Test]
        public void X86rw_rol_Eb()
        {
            Run32bitTest("C0C0C0");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|C = (al & 1<8> << 8<8> - 0xC0<8>) != 0<8>",
                "2|L--|al = __rol<byte,byte>(al, 0xC0<8>)");
        }

        [Test]
        public void X86Rw_ror_1()
        {
            Run32bitTest("D0 C8");      // ror al,1h
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|C = (al & 1<8> << 1<8> - 1<8>) != 0<8>",
                "2|L--|al = __ror<byte,byte>(al, 1<8>)");
        }

        [Test]
        public void X86rw_pause()
        {
            Run32bitTest("F390");
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__pause()");
        }

        [Test]
        public void X86Rw_vpunpckhwd()
        {
            Run32bitTest("C5E4694A2A");
            AssertCode(     // vpunpckhwd	mm1,dword ptr [edx+2Ah]
                "0|L--|10000000(5): 1 instructions",
                "1|L--|mm1 = __punpckhwd<word64>(mm1, Mem0[edx + 42<i32>:word32])");
        }

        [Test]
        public void X86Rw_vpunpckldq()
        {
            Run32bitTest("C5F162C8");
            AssertCode(     // vpunpckldq	xmm1,xmm1,xmm0
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm1 = __punpckldq<word128>(xmm1, xmm0)");
        }

        [Test]
        public void X86Rw_vpunpcklqdq()
        {
            Run32bitTest("C5E16CC3");
            AssertCode(     // vpunpcklqdq	xmm0,xmm3,xmm3
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklqdq<word128>(xmm3, xmm3)");
        }

        [Test]
        public void X86Rw_vpunpcklwd()
        {
            Run32bitTest("C5E96102");
            AssertCode(     // vpunpcklwd	xmm0,xmm2,[edx]
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklwd<word128>(xmm2, Mem0[edx:word128])");
        }

        [Test]
        public void X86rw_pxor_self()
        {
            Run32bitTest("0FEFC9");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm1 = 0<64>");
        }

        [Test]
        public void X86rw_lock()
        {
            Run32bitTest("F0");
            AssertCode(
                  "0|L--|10000000(1): 1 instructions",
                  "1|L--|__lock()");
        }

        [Test]
        public void X86rw_cmpxchg()
        {
            Run32bitTest("0FB10A");
            AssertCode(
              "0|L--|10000000(3): 1 instructions",
              "1|L--|Z = __cmpxchg<word32>(Mem0[edx:word32], ecx, eax, out eax)");
        }

        [Test]
        public void X86rw_Xadd()
        {
            Run32bitTest("0fC1C2");
            AssertCode(
               "0|L--|10000000(3): 2 instructions",
               "1|L--|edx = __xadd<word32>(edx, eax)",
               "2|L--|SCZO = cond(edx)");
        }

        [Test]
        public void X86Rw_vcvttpd2dq()
        {
            Run32bitTest("C5C1E602");
            AssertCode(     // vcvttpd2dq	xmm0,[edx]
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = Mem0[edx:word128]",
                "2|L--|v6 = __cvttpd2dq<real64[2],int32[2]>(v4)",
                "3|L--|xmm0 = CONVERT(v6, int32[2], uint128)");
        }

        [Test]
        public void X86Rw_vcvtps2pi()
        {
            Run32bitTest("C5F42D27");
            AssertCode(     // vcvtps2pi	mm4,ymmword ptr [edi]
                "0|L--|10000000(4): 2 instructions",    //$REVIEW: encoding looks weird
                "1|L--|v4 = Mem0[edi:word256]",
                "2|L--|mm4 = __cvtps2pi<real32[8],int32[8]>(v4)");
        }

        [Test]
        public void X86rw_cvttsd2si()
        {
            Run32bitTest("F20F2CC3");
            AssertCode(
              "0|L--|10000000(4): 1 instructions",
              "1|L--|eax = CONVERT(SLICE(xmm3, real64, 0), real64, int32)");
        }

        [Test]
        public void X86rw_fucompp()
        {
            Run32bitTest("DAE9");
            AssertCode(
              "0|L--|10000000(2): 2 instructions",
              "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
              "2|L--|Top = Top + 2<i8>");
        }

        [Test]
        public void X86rw_fs_prefix()
        {
            Run32bitTest("648B0A");
            AssertCode(
           "0|L--|10000000(3): 1 instructions",
           "1|L--|ecx = Mem0[fs:edx:word32]");
        }

        [Test]
        public void X86rw_seto()
        {
            Run32bitTest("0f90c1");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = CONVERT(Test(OV,O), bool, byte)");
        }

        [Test]
        public void X86rw_cpuid()
        {
            Run32bitTest("0FA2");
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__cpuid(eax, ecx, &eax, &ebx, &ecx, &edx)");
        }

        [Test]
        public void X86rw_xgetbv()
        {
            Run32bitTest("0F01D0");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx_eax = __xgetbv(ecx)");
        }

        [Test]
        public void X86rw_setc()
        {
            Run32bitTest("0F92C1");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = CONVERT(Test(ULT,C), bool, byte)");
        }

        [Test]
        public void X86rw_movd_xmm()
        {
            Run32bitTest("660f6ec0");
            AssertCode(  // movd
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = CONVERT(eax, word32, word128)");
        }

        [Test]
        public void X86rw_more_xmm()
        {
            Run32bitTest("660F7E01");
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = SLICE(xmm0, word32, 0)",
                "2|L--|Mem0[ecx:word32] = v5");
            Run32bitTest("660f60c0");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklbw<word128>(xmm0, xmm0)");
            Run32bitTest("660f61c0");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklwd<word128>(xmm0, xmm0)");
            Run32bitTest("660f70c000");
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __pshuf<word32[4]>(xmm0, xmm0, 0<8>)");
        }



        [Test]
        public void X86rw_PIC_idiom()
        {
            Run32bitTest("E80000000059");        // call $+5, pop ecx
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ecx = 10000005");
        }

        [Test]
        public void X86rw_invalid_les()
        {
            Run32bitTest("C4C0");
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_fstp_real32()
        {
            Run32bitTest("d91c24");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[esp:real32] = CONVERT(ST[Top:real64], real64, real32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_cmpxchg_byte()
        {
            Run32bitTest("F00FB023"); // lock cmpxchg[ebx], ah
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|__lock()",
                "2|L--|10000001(3): 1 instructions",
                "3|L--|Z = __cmpxchg<byte>(Mem0[ebx:byte], ah, al, out al)");
        }

        [Test]
        public void X86rw_idiv()
        {
            Run32bitTest("F77C2404");       // idiv [esp+04]
            AssertCode(
                  "0|L--|10000000(4): 4 instructions",
                  "1|L--|v6 = edx_eax",
                  "2|L--|edx = v6 %s Mem0[esp + 4<i32>:word32]",
                  "3|L--|eax = v6 /32 Mem0[esp + 4<i32>:word32]",
                  "4|L--|SCZO = cond(eax)");
        }

        [Test]
        public void X86rw_long_nop()
        {
            Run32bitTest("660f1f440000"); // nop WORD PTR[eax + eax*1 + 0x0]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_movlhps()
        {
            Run32bitTest("0F16F3");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|xmm6[2<i32>] = xmm3[0<i32>]",
                "2|L--|xmm6[3<i32>] = xmm3[1<i32>]");
        }

        [Test]
        public void X86rw_fucomi()
        {
            Run32bitTest("DBEB");  // fucomi\tst(0),st(3)
            AssertCode(
               "0|L--|10000000(2): 3 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 3<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_fucomip()
        {
            Run32bitTest("DFE9");   // fucomip\tst(0),st(1)
            AssertCode(
               "0|L--|10000000(2): 4 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false",
               "4|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86Rw_movzx()
        {
            Run32bitTest("0F B7 C0");   // movzx eax, ax
            AssertCode(
               "0|L--|10000000(3): 1 instructions",
               "1|L--|eax = CONVERT(ax, word16, word32)");
        }

        [Test(Description = "Regression reported by @mewmew")]
        public void X86rw_regression1()
        {
            Run32bitTest("DB7C4783");       // fstp [esi-0x7D + eax*2]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Mem0[edi - 125<i32> + eax * 2<32>:real80] = CONVERT(ST[Top:real64], real64, real80)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fdivr()
        {
            Run32bitTest("DC3D78563412"); // fdivr [12345678]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = Mem0[0x12345678<p32>:real64] / ST[Top:real64]");
        }

        [Test]
        public void X86Rw_fneni()
        {
            Run32bitTest("DBE0");
            AssertCode(     // fneni
                "0|L--|10000000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_fninit()
        {
            Run32bitTest("DBE3");
            AssertCode(     // fninit
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fninit()");
        }

        /// <summary>
        /// This appears to be an obsolete 286 whose net effect is negligible.
        /// </summary>
        [Test]
        public void X86Rw_fnsetpm()
        {
            Run32bitTest("DBE4");	// fnsetpm
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_push_segreg()
        {
            Run32bitTest("06");
            AssertCode(     // "push es", 
                "0|L--|10000000(1): 2 instructions",
                "1|L--|esp = esp - 2<i32>",
                "2|L--|Mem0[esp:word16] = es");
        }

        [Test]
        public void X86rw_mfence()
        {
            Run32bitTest("0FAEF0");   // mfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__mfence()");
        }

        [Test]
        public void X86rw_lfence()
        {
            Run32bitTest("0FAEE8"); // lfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__lfence()");
        }

        [Test]
        public void X86rw_aesimc()
        {
            Run32bitTest("660F38DBC0"); // aesimc\txmm0,xmm0
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __aesimc(xmm0)");
        }

        [Test]
        public void X86Rw_andpd()
        {
            Run32bitTest("660F540550595700");	// andpd	xmm0,[00575950]
            AssertCode(
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm0 = __andp<word64[2]>(xmm0, Mem0[0x00575950<p32>:(arr word64 2)])");
        }

        [Test]
        public void X86rw_andnps()
        {
            Run32bitTest("0F554242");    // andnps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __andnps<word128>(xmm0, Mem0[edx + 66<i32>:word128])");
        }

        [Test]
        public void X86rw_andps()
        {
            Run32bitTest("0F544242");    // andps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 1 instructions",
               "1|L--|xmm0 = __andp<word32[4]>(xmm0, Mem0[edx + 66<i32>:(arr word32 4)])");
        }

        [Test]
        public void X86rw_bsf()
        {
            Run32bitTest("0FBC4242");    // bsf\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Z = Mem0[edx + 66<i32>:word32] == 0<32>",
                "2|L--|eax = __bsf<word32>(Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_btc()
        {
            Run32bitTest("0FBB4242");    // btc\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btc<word32>(Mem0[edx + 66<i32>:word32], eax, out Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_btr()
        {
            Run32bitTest("0FBAF300");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btr<word32>(ebx, 0<8>, out ebx)");
        }

        [Test]
        public void X86rw_bts()
        {
            Run32bitTest("0FAB0424");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __bts<word32>(Mem0[esp:word32], eax, out Mem0[esp:word32])");
        }

        [Test]
        public void X86Rw_clflush()
        {
            Run32bitTest("0FAE7D11");
            AssertCode(     // clflush	byte ptr [ebp+11h]
                "0|L--|10000000(4): 1 instructions",
                "1|L--|__cache_line_flush(&Mem0[ebp + 17<i32>:byte])");
        }

        [Test]
        public void X86rw_clts()
        {
            Run32bitTest("0F06");    // clts
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|cr0 = __clts<word32>(cr0)");
        }

        [Test]
        public void X86rw_cmpltps()
        {
            Run32bitTest("0FC2424201");    // cmpltps\txmm0,[edx+42],01
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __cmpltp<real32[4],word32[4]>(xmm0, Mem0[edx + 66<i32>:(arr real32 4)])");
        }

        [Test]
        public void X86Rw_vcomisd()
        {
            Run32bitTest("C5CD2F5D33");
            AssertCode(     // vcomisd	xmm3,double ptr [ebp+33h]
                "0|L--|10000000(5): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm3, real64, 0) - Mem0[ebp + 51<i32>:real64])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_comiss()
        {
            Run32bitTest("0F2F4242");    // comiss\txmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[edx + 66<i32>:real32])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_comiss_reg()
        {
            Run32bitTest("0F2FCF");
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm1, real32, 0) - SLICE(xmm7, real32, 0))",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86Rw_vcvtdq2pd()
        {
            Run32bitTest("C5FAE6F3");
            AssertCode(     // vcvtdq2pd	xmm6,xmm3
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = xmm3",  //$TODO check sizes?
                "2|L--|xmm6 = __cvtdq2pd<int32[4],real64[4]>(v4)");
        }

        [Test]
        public void X86rw_cvtdq2ps()
        {
            Run32bitTest("0F5B4242");    // cvtdq2ps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|v6 = __cvtdq2ps<int64[2],real32[2]>(v4)",
                "3|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v6)");
        }

        [Test]
        public void X86rw_cvtps2pd()
        {
            Run32bitTest("0F5A4242");    // cvtps2pd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __cvtps2pd<real32[4],real64[4]>(v4)");
        }

        [Test]
        public void X86rw_cvtps2pi()
        {
            Run32bitTest("0F2D4242");    // cvtps2pi\tmm0,xmmword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|mm0 = __cvtps2pi<real32[4],int32[4]>(v4)");
        }

        [Test]
        public void X86rw_div()
        {
            Run32bitTest("F7742404");       // idiv [esp+04]
            AssertCode(
                  "0|L--|10000000(4): 4 instructions",
                  "1|L--|v6 = edx_eax",
                  "2|L--|edx = v6 %u Mem0[esp + 4<i32>:word32]",
                  "3|L--|eax = v6 /u Mem0[esp + 4<i32>:word32]",
                  "4|L--|SCZO = cond(eax)");
        }

        [Test]
        public void X86rw_emms()
        {
            Run32bitTest("0F77");    // emms
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__emms()");
        }

        [Test]
        public void X86rw_fclex()
        {
            Run32bitTest("DBE2");    // fclex
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fclex()");
        }

        [Test]
        public void X86rw_fcmovb()
        {
            Run32bitTest("DAC1");    // fcmovb\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GE,C)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovbe()
        {
            Run32bitTest("DAD1");    // fcmovbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GT,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmove()
        {
            Run32bitTest("DAC9");    // fcmove\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnbe()
        {
            Run32bitTest("DBD1");    // fcmovnbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(LE,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovne()
        {
            Run32bitTest("DBC9");    // fcmovne\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnu()
        {
            Run32bitTest("DBD9");    // fcmovnu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovu()
        {
            Run32bitTest("DAD9");    // fcmovu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcomip()
        {
            Run32bitTest("DFF2");    // fcomip\tst(0),st(2)
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])",
                "2|L--|O = false",
                "3|L--|S = false",
                "4|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_ffree()
        {
            Run32bitTest("DDC2");    // ffree\tst(2)
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__ffree(ST[Top + 2<i8>:real64])");
        }

        [Test]
        public void X86rw_fild_i16()
        {
            Run32bitTest("DF4042");    // fild\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax + 66<i32>:int16], int16, real64)");
        }

        [Test]
        public void X86rw_fisttp()
        {
            Run32bitTest("DB08");    // fisttp\tdword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Mem0[eax:int32] = CONVERT(trunc(ST[Top:real64]), real64, int32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int16()
        {
            Run32bitTest("DF4842");    // fisttp\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 66<i32>:int16] = CONVERT(trunc(ST[Top:real64]), real64, int16)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int64()
        {
            Run32bitTest("DD4842");    // fisttp\tqword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 66<i32>:int64] = CONVERT(trunc(ST[Top:real64]), real64, int64)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fld_real80()
        {
            Run32bitTest("DB28");    // fld\ttword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax:real80], real80, real64)");
        }

        [Test]
        public void X86rw_fucom()
        {
            Run32bitTest("DDE5");    // fucom\tst(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top + 5<i8>:real64] - ST[Top:real64])",
                "2|L--|Top = Top + 0<i8>");
        }

        [Test]
        public void X86rw_fcomp()
        {
            Run32bitTest("D8 D9");               // fcomp st(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fucomp()
        {
            Run32bitTest("DDEA");    // fucomp\tst(2)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_invd()
        {
            Run32bitTest("0F08");    // invd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__invd()");
        }

        [Test]
        public void X86rw_lar()
        {
            Run32bitTest("0F024242");    // lar\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 2 instructions",
                "1|L--|eax = __lar<word32>(&Mem0[edx + 66<i32>:word16])",
                "2|L--|Z = true");
        }

        [Test]
        public void X86rw_lsl()
        {
            Run32bitTest("0F034242");    // lsl\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|eax = __load_segment_limit<word32>(Mem0[edx + 66<i32>:word16])");
        }

        [Test]
        public void X86Rw_maskmovdqu()
        {
            Run32bitTest("660FF7E2");
            AssertCode(     // maskmovdqu	xmm4,xmm2
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm4 = __maskmovdqu<word128>(xmm4, xmm2)");
        }

        [Test]
        public void X86rw_maskmovq()
        {
            Run32bitTest("0FF74242");    // maskmovq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __maskmovq<word64>(mm0, Mem0[edx + 66<i32>:word64])");
        }

        [Test]
        public void X86Rw_minps()
        {
            Run32bitTest("0F5D4242");    // minps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __simd_min<real32[4]>(xmm0, Mem0[edx + 66<i32>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_syscall()
        {
            Run32bitTest("0F05");    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_sysenter()
        {
            Run32bitTest("0F34");    // sysenter
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__sysenter()");
        }

        [Test]
        public void X86rw_sysexit()
        {
            Run32bitTest("0F35");    // sysexit
            AssertCode(
                "0|R--|10000000(2): 2 instructions",
                "1|L--|__sysexit()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void X86rw_sysret()
        {
            Run32bitTest("0F07");    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_ud2()
        {
            Run32bitTest("0F0B");    // ud2
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_unpcklpd()
        {
            Run32bitTest("660F144242");    // unpcklpd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __unpcklp<real64[2]>(xmm0, Mem0[edx + 66<i32>:(arr real64 2)])");
        }

        [Test]
        public void X86rw_unpcklps()
        {
            Run32bitTest("0F144242");    // unpcklps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __unpcklp<real32[4]>(xmm0, Mem0[edx + 66<i32>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_wbinvd()
        {
            Run32bitTest("0F09");    // wbinvd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wbinvd()");
        }

        [Test]
        public void X86rw_prefetchw()
        {
            Run32bitTest("0F0D4242");    // prefetchw\tdword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|__prefetchw<ptr32>(Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_mov_from_control_Reg()
        {
            Run32bitTest("0F2042");    // mov\tedx,cr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = cr0");
        }

        [Test]
        public void X86rw_mov_debug_reg()
        {
            Run32bitTest("0F2142");    // mov\tedx,dr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = dr0");
        }

        [Test]
        public void X86rw_mov_sib_eiz()
        {
            Run32bitTest("8B4C61F8");   // mov\tecx,[ecx+eiz*2-8h]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ecx = Mem0[ecx - 8<i32>:word32]");
        }

        [Test]
        public void X86rw_mov_control_reg()
        {
            Run32bitTest("0F2242");    // mov\tcr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cr0 = edx");
        }

        [Test]
        public void X86rw_mov_to_debug_reg()
        {
            Run32bitTest("0F2342");    // mov\tdr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|dr0 = edx");
        }

        [Test]
        public void X86rw_movhpd()
        {
            Run32bitTest("0F174242");    // movhps\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|Mem0[edx + 66<i32>:word64] = __movhp<real32[4],real64[1]>(v5)");
            Run32bitTest("660F174242");    // movhpd\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(5): 2 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|Mem0[edx + 66<i32>:word64] = __movhp<real64[2],real64[1]>(v5)");
        }

        [Test]
        public void X86rw_movlps()
        {
            Run32bitTest("0F124242");    // movlps\txmm0,qword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word64]",
                "2|L--|xmm0 = __movlp<real32[2],real64[2]>(v5)");
        }

        [Test]
        public void X86rw_movmskps()
        {
            Run32bitTest("0F5042");    // movmskps\teax,xmm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = __movmskps<word128>(xmm2)",
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v5)");
        }

        [Test]
        public void X86rw_movnti()
        {
            //$TODO: should use intrisic here.
            Run32bitTest("0FC34242");    // movnti\t[edx+42],eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word32] = eax");
        }

        [Test]
        public void X86rw_movntps()
        {
            Run32bitTest("0F2B4242");    // movntps\t[edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word128] = xmm0");
        }

        [Test]
        public void X86rw_movntq()
        {
            Run32bitTest("0FE74242");    // movntq\t[edx+42],mm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word64] = mm0");
        }

        [Test]
        public void X86rw_orps()
        {
            Run32bitTest("0F564242");    // orps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 1 instructions",
               "1|L--|xmm0 = __orp<real32[4]>(xmm0, Mem0[edx + 66<i32>:(arr real32 4)])");
        }

        [Test]
        public void X86rw_packssdw()
        {
            Run32bitTest("0F6B4242");    // packssdw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __packss<int32[2],int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int32 2)])");
        }

        [Test]
        public void X86rw_packuswb()
        {
            Run32bitTest("0F674242");    // packuswb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __packus<uint16[4],uint8[8]>(mm0, Mem0[edx + 66<i32>:(arr uint16 4)])");
        }

        [Test]
        public void X86rw_paddb()
        {
            Run32bitTest("0FFC4242");    // paddb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_add<byte[8]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_paddd()
        {
            Run32bitTest("0FFE4242");    // paddd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_add<word32[2]>(mm0, Mem0[edx + 66<i32>:(arr word32 2)])");
        }

        [Test]
        public void X86rw_paddsw()
        {
            Run32bitTest("0FED4242");    // paddsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __padds<int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }

        [Test]
        public void X86rw_paddusb()
        {
            Run32bitTest("0FDC4242");    // paddusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __paddus<byte[8]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_paddw()
        {
            Run32bitTest("0FFD4242");    // paddw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_add<word16[4]>(mm0, Mem0[edx + 66<i32>:(arr word16 4)])");
        }

        [Test]
        public void X86rw_pextrw()
        {
            Run32bitTest("0FC54242");    // pextrw\teax,mm2,42
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|eax = SLICE(mm2, word16, 16)");
        }

        [Test]
        public void X86Rw_pinsrb()
        {
            Run32bitTest("660F3A20C00F");
            AssertCode(     // pinsrb	xmm0,eax,0Fh
                "0|L--|10000000(6): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,byte>(xmm0, eax, 0xF<8>)");
        }

        [Test]
        public void X86rw_pinsrw()
        {
            Run32bitTest("0FC44204");    // pinsrw\tmm0,edx,04
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pinsr<word64,word16>(mm0, edx, 4<8>)");
        }

        [Test]
        public void X86Rw_pinsrd()
        {
            Run32bitTest("660F3A2244240802");
            AssertCode(     // pinsrd	xmm0,dword ptr [esp+8h],2h
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,word32>(xmm0, Mem0[esp + 8<i32>:word32], 2<8>)");
        }

        [Test]
        public void X86rw_pxor()
        {
            Run32bitTest("0FEF4242");    // pxor\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pxor<word64>(mm0, Mem0[edx + 66<i32>:word64])");
        }

        [Test]
        public void X86rw_rcpps()
        {
            Run32bitTest("0F534242");    // rcpps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __rcpp<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_rdmsr()
        {
            Run32bitTest("0F32");    // rdmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdmsr(ecx)");
        }

        [Test]
        public void X86Rw_rdpid()
        {
            Run32bitTest("F30FC7F9");
            AssertCode(     // rdpid\tecx",
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ecx = __rdpid<word32>()");
        }

        [Test]
        public void X86rw_rdpmc()
        {
            Run32bitTest("0F33");    // rdpmc
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdpmc(ecx)");
        }

        [Test]
        public void X86Rw_rdrand()
        {
            Run32bitTest("0FC7F3");
            AssertCode(     // rdrand	ebx
                "0|L--|10000000(3): 5 instructions",
                "1|L--|C = __rdrand(out ebx)",
                "2|L--|S = 0<32>",
                "3|L--|Z = 0<32>",
                "4|L--|O = 0<32>",
                "5|L--|P = 0<32>");
        }

        [Test]
        public void X86rw_rdtsc()
        {
            Run32bitTest("0F31");
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdtsc()");
        }

        [Test]
        public void X86Rw_rdtscp()
        {
            Run32bitTest("0F01F9");
            AssertCode(     // rdtscp
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx_eax = __rdtscp(out ecx)");
        }

        [Test]
        public void X86rw_rsqrtps()
        {
            Run32bitTest("0F524242");    // rsqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __rsqrtp<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_sqrtps()
        {
            Run32bitTest("0F514242");    // sqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __simd_sqrt<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_pcmpgtb()
        {
            Run32bitTest("0F644242");    // pcmpgtb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr byte 8)]",
                "3|L--|mm0 = __pcmpgt<byte[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_pcmpgtw()
        {
            Run32bitTest("0F654242");    // pcmpgtw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word16 4)]",
                "3|L--|mm0 = __pcmpgt<word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_pcmpgtd()
        {
            Run32bitTest("0F664242");    // pcmpgtd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word32 2)]",
                "3|L--|mm0 = __pcmpgt<word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_punpckhbw()
        {
            Run32bitTest("0F684242");    // punpckhbw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhbw<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckhwd()
        {
            Run32bitTest("0F694242");    // punpckhwd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhwd<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckhdq()
        {
            Run32bitTest("0F6A4242");    // punpckhdq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhdq<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckldq()
        {
            Run32bitTest("0F624242");    // punpckldq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckldq<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86Rw_vpcmpeqd()
        {
            Run32bitTest("C5F576BA30406AF1");
            AssertCode(     // vpcmpeqd	ymm7,ymm1,[edx+0F16A4030h]
                "0|L--|10000000(8): 1 instructions",
                "1|L--|ymm7 = __pcmpeq<word32[8]>(ymm1, Mem0[edx + 0xF16A4030<32>:(arr word32 8)])");
        }

        [Test]
        public void X86rw_pcmpeqd()
        {
            Run32bitTest("0F764242");    // pcmpeqd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pcmpeq<word32[2]>(mm0, Mem0[edx + 66<i32>:(arr word32 2)])");
        }

        [Test]
        public void X86rw_pcmpeqw()
        {
            Run32bitTest("0F754242");    // pcmpeqw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pcmpeq<word16[4]>(mm0, Mem0[edx + 66<i32>:(arr word16 4)])");
        }

        [Test]
        public void X86rw_vmread()
        {
            Run32bitTest("0F784242");    // vmread\t[edx+42],eax
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word32] = __vmread<word32,word32>(eax)");
        }

        [Test]
        public void X86rw_vmwrite()
        {
            Run32bitTest("0F794242");    // vmwrite\teax,[edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__vmwrite<word32,word32>(eax, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86Rw_vmxon()
        {
            Run32bitTest("C5FEC7772B");
            AssertCode(     // vmxon	qword ptr [edi+2Bh]
                "0|S--|10000000(5): 1 instructions",
                "1|L--|__vmxon(&Mem0[edi + 43<i32>:word64])");
        }

        [Test]
        public void X86rw_vshufps()
        {
            Run32bitTest("0FC6424207");    // vshufps\txmm0,[edx+42],07
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __shufp<real32[4]>(v5, v6, 7<8>)");
        }

        [Test]
        public void X86rw_pminub()
        {
            Run32bitTest("0FDA4242");    // pminub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pminu<uint8[8]>(mm0, Mem0[edx + 66<i32>:(arr uint8 8)])");
        }

        [Test]
        public void X86rw_pmulhrsw()
        {
            Run32bitTest("0F 38 0B 0B");    // pmulhrsw mm1,[ebx]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm1 = __pmulhrs<int16[4]>(mm1, Mem0[ebx:(arr int16 4)])");
        }

        [Test]
        public void X86rw_pmullw()
        {
            Run32bitTest("0FD54242");    // pmullw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmull<int16[4],int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }
        [Test]
        public void X86rw_pmovmskb()
        {
            Run32bitTest("0FD742");    // pmovmskb\teax,mm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = __pmovmskb<word64>(mm2)",
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v5)");
        }

        [Test]
        public void X86rw_psrad()
        {
            Run32bitTest("0FE24242");    // psrad\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psra<int32[2]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psrlq()
        {
            Run32bitTest("0FD34242");    // psrlq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psrl<word64[1]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86Rw_vpsubsb()
        {
            Run32bitTest("C5C1E810");
            AssertCode(     // vpsubsb	xmm2,xmm7,[eax]
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm2 = __psubs<int8[16]>(xmm7, Mem0[eax:(arr int8 16)])");
        }

        [Test]
        public void X86rw_psubusb()
        {
            Run32bitTest("0FD84242");    // psubusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __psubus<uint8[8]>(mm0, Mem0[edx + 66<i32>:(arr uint8 8)])");
        }

        [Test]
        public void X86rw_pmaxub()
        {
            Run32bitTest("0FDE4242");    // pmaxub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmaxu<uint8[8]>(mm0, Mem0[edx + 66<i32>:(arr uint8 8)])");
        }

        [Test]
        public void X86rw_pavgb()
        {
            Run32bitTest("0FE04242");    // pavgb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pavg<byte[8]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_psraw()
        {
            Run32bitTest("0FE14242");    // psraw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psra<int16[4]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_pmulhuw()
        {
            Run32bitTest("0FE44242");    // pmulhuw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmulhu<uint16[4],uint16[4]>(mm0, Mem0[edx + 66<i32>:(arr uint16 4)])");
        }

        [Test]
        public void X86rw_pmulhw()
        {
            Run32bitTest("0FE54242");    // pmulhw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmulh<int16[4],int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }

        [Test]
        public void X86rw_psubb()
        {
            Run32bitTest("0FF84242");    // psubb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_sub<byte[8]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_psubd()
        {
            Run32bitTest("0FFA4242");    // psubd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_sub<word32[2]>(mm0, Mem0[edx + 66<i32>:(arr word32 2)])");
        }

        [Test]
        public void X86rw_psubq()
        {
            //$TODO: consider reducing this to a simple subtraction once SIMD slicing is implemented.
            Run32bitTest("0FFB4242");    // psubq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_sub<word64[1]>(mm0, Mem0[edx + 66<i32>:(arr word64 1)])");
        }

        [Test]
        public void X86rw_psubsw()
        {
            Run32bitTest("0FE94242");    // psubsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __psubs<int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }

        [Test]
        public void X86rw_psubw()
        {
            Run32bitTest("0FF94242");    // psubw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __simd_sub<word16[4]>(mm0, Mem0[edx + 66<i32>:(arr word16 4)])");
        }

        [Test]
        public void X86rw_psubsb()
        {
            Run32bitTest("0FE84242");    // psubsb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __psubs<int8[8]>(mm0, Mem0[edx + 66<i32>:(arr int8 8)])");
        }

        [Test]
        public void X86rw_pmaxsw()
        {
            Run32bitTest("0FEE4242");    // pmaxsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmaxs<int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }

        [Test]
        public void X86rw_pminsw()
        {
            Run32bitTest("0FEA4242");    // pminsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmins<int16[4]>(mm0, Mem0[edx + 66<i32>:(arr int16 4)])");
        }

        [Test]
        public void X86Rw_popfw()
        {
            Run32bitTest("669D");
            AssertCode(     // popfw
                "0|L--|10000000(2): 2 instructions",
                "1|L--|SCZDOP = SEQ(SLICE(SCZDOP, word16, 16), Mem0[esp:word16])",
                "2|L--|esp = esp + 2<i32>");
        }

        [Test]
        public void X86rw_por()
        {
            Run32bitTest("0FEB4242");    // por\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = mm0 | Mem0[edx + 66<i32>:word64]");
        }

        [Test]
        public void X86rw_pslld()
        {
            Run32bitTest("0FF24242");    // pslld\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word32[2]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psllq()
        {
            Run32bitTest("0FF34242");    // psllq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word64[1]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psllw()
        {
            Run32bitTest("0FF14242");    // psllw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word16[4]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_pmaddwd()
        {
            Run32bitTest("0FF54242");    // pmaddwd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmaddwd<word16[4],word32[2]>(mm0, Mem0[edx + 66<i32>:(arr word16 4)])");
        }

        [Test]
        public void X86rw_pmuludq()
        {
            Run32bitTest("0FF44242");    // pmuludq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pmulu<uint32[2],uint64[1]>(mm0, Mem0[edx + 66<i32>:(arr uint32 2)])");
        }

        [Test]
        public void X86rw_psadbw()
        {
            Run32bitTest("0FF64242");    // psadbw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __psadbw<byte[8],word16[4]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_wrmsr()
        {
            Run32bitTest("0F30");    // wrmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wrmsr(ecx, edx_eax)");
        }

        [Test]
        public void X86Rw_pxor()
        {
            Run32bitTest("660FEFC0");	// pxor	xmm0,xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86Rw_stmxcsr()
        {
            Run32bitTest("0FAE5DF0");	// stmxcsr	dword ptr [ebp-10]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[ebp - 16<i32>:word32] = mxcsr");
        }

        [Test]
        public void X86Rw_fcmovu()
        {
            Run32bitTest("DADD");	// fcmovu	st(0),st(5)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 5<i8>:real64]");
        }

        [Test]
        public void X86Rw_psrlw()
        {
            Run32bitTest("0FD1E8");	// psrlw	mm5,mm0
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = mm5",
                "2|L--|mm5 = __psrl<word16[4]>(v5, SLICE(mm0, byte, 0))");
        }

        [Test]
        public void X86Rw_psrld()
        {
            Run32bitTest("0FD2F9");	// psrld	mm7,mm1
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = mm7",
                "2|L--|mm7 = __psrl<word32[2]>(v5, SLICE(mm1, byte, 0))");
        }

        [Test]
        public void X86Rw_fcmovnu()
        {
            Run32bitTest("DBD9");	// fcmovnu	st(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86Rw_paddq()
        {
            Run32bitTest("0FD408");	// paddq	mm1,[eax]
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm1 = __simd_add<word64[1]>(mm1, Mem0[eax:(arr word64 1)])");
        }

        [Test]
        public void X86Rw_psubusw()
        {
            Run32bitTest("0FD9450C");	// psubusw	mm0,[ebp+0C]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __psubus<uint16[4]>(mm0, Mem0[ebp + 12<i32>:(arr uint16 4)])");
        }

        [Test]
        public void X86Rw_pshufw()
        {
            Run32bitTest("0F700200");	// pshufw	mm0,[edx],00
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pshuf<word16[4]>(mm0, Mem0[edx:word64], 0<8>)");
        }

        [Test]
        public void X86Rw_fxtract()
        {
            Run32bitTest("D9F4");	// fxtract
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|v4 = ST[Top:real64]",
                "2|L--|Top = Top - 1<i8>",
                "3|L--|ST[Top + 1<i8>:real64] = __exponent(v4)",
                "4|L--|ST[Top:real64] = __significand(v4)");
        }

        [Test]
        public void X86Rw_fprem()
        {
            Run32bitTest("D9F8");	// fprem
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = __fprem_x87(ST[Top:real64], ST[Top + 1<i8>:real64])",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_fprem1()
        {
            Run32bitTest("D9F5");	// fprem1	st(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] % ST[Top + 1<i8>:real64]",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_vpsubd()
        {
            Run32bitTest("660FFAD0");	// vpsubd	xmm2,xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm2 = __simd_sub<word32[4]>(xmm2, xmm0)");
        }

        [Test]
        public void X86Rw_vpsrlq()
        {
            Run32bitTest("660FD3CA");	// vpsrlq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|xmm1 = __psrl<word64[2]>(v5, SLICE(xmm2, byte, 0))");
        }

        [Test]
        public void X86Rw_pshufb()
        {
            Run32bitTest("660F38000CDD20220F08");
            AssertCode(     // pshufb	xmm1,[80F2220h+ebx*8]
                "0|L--|10000000(10): 3 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|v6 = xmm1",
                "3|L--|xmm1 = __pshufb<byte[16]>(v5, v6, Mem0[0x80F2220<32> + ebx * 8<32>:word128])");
        }

        [Test]
        public void X86Rw_pshuflw()
        {
            Run32bitTest("F20F70C0E0");
            AssertCode(     // pshuflw	xmm0,xmm0,0E0h
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = xmm0",
                "3|L--|xmm0 = __pshuflw<byte[16]>(v4, v5, 0xE0<8>)");
        }

        [Test]
        public void X86Rw_vpsllq()
        {
            Run32bitTest("660FF3CA");	// psllq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|xmm1 = __psll<word64[2]>(v5, SLICE(xmm2, byte, 0))");
        }

        [Test]
        public void X86Rw_orpd()
        {
            Run32bitTest("660F561DA0595700");	// orpd	xmm3,[005759A0]
            AssertCode(
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm3 = __orp<real64[2]>(xmm3, Mem0[0x005759A0<p32>:(arr real64 2)])");
        }

        [Test]
        public void X86Rw_movlpd()
        {
            Run32bitTest("660F12442404");	// movlpd	xmm0,qword ptr [esp+04]
            AssertCode(
                "0|L--|10000000(6): 2 instructions",
                "1|L--|v5 = Mem0[esp + 4<i32>:word64]",
                "2|L--|xmm0 = __movlp<real64[1],real64[2]>(v5)");
        }

        [Test]
        public void X86Rw_vextrw()
        {
            Run32bitTest("660FC5C003");	// vextrw	eax,xmm0,03
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|eax = SLICE(xmm0, word16, 24)");
        }

        [Test]
        public void X86Rw_cvtsd2si()
        {
            Run32bitTest("F20F2DD1");	// cvtsd2si	edx,xmm1
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|edx = CONVERT(SLICE(xmm1, real64, 0), real64, int32)");
        }

        [Test]
        public void X86Rw_vpand()
        {
            Run32bitTest("660FDBFE");	// vpand	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm7 = __pand<word128>(xmm7, xmm6)");
        }

        [Test]
        public void X86Rw_pand()
        {
            Run32bitTest("0FDBFE");	// pand	mm7,mm6
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm7 = __pand<word64>(mm7, mm6)");
        }

        [Test]
        public void X86Rw_vpaddq()
        {
            Run32bitTest("660FD4FE");	// vpaddq	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm7 = __simd_add<word64[2]>(xmm7, xmm6)");
        }

        [Test]
        public void X86Rw_vpandn()
        {
            Run32bitTest("660FDFF2");	// vpandn	xmm6,xmm2
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm6 = __pandn<word128>(xmm6, xmm2)");
        }

        [Test]
        public void X86Rw_pandn()
        {
            Run32bitTest("0FDFF2");	// pandn	mm6,mm2
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm6 = __pandn<word64>(mm6, mm2)");
        }

        [Test]
        public void X86Rw_vpinsrw()
        {
            Run32bitTest("660FC4C002");	// vpinsrw	xmm0,eax,02
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,word16>(xmm0, eax, 2<8>)");
        }

        [Test]
        public void X86Rw_ldmxcsr()
        {
            Run32bitTest("0FAE5508");	// ldmxcsr	dword ptr [ebp+08]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mxcsr = Mem0[ebp + 8<i32>:word32]");
        }

        [Test]
        public void X86Rw_vlddqu()
        {
            Run32bitTest("C5E3F08BD780E190");
            AssertCode(     // vlddqu	xmm1,[ebx+90E180D7h]
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm1 = Mem0[ebx + 0x90E180D7<32>:word128]");
        }

        [Test]
        public void X86Rw_paddsb()
        {
            Run32bitTest("0FECFF");	// paddsb	mm7,mm7
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm7 = __padds<int8[8]>(mm7, mm7)");
        }

        [Test(Description = "We cannot make 16-bit calls in 32- or 64-bit mode")]
        public void X86Rw_invalid_call()
        {
            Run32bitTest("66FF51CC"); // call word ptr[ecx - 34]
            AssertCode(
                "0|---|10000000(4): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_sldt()
        {
            Run32bitTest("0F0001");  // sldt	word ptr [ecx]
            AssertCode(
                 "0|S--|10000000(3): 1 instructions",
                 "1|L--|Mem0[ecx:word16] = __sldt<word16>()");
        }

        [Test]
        public void X86Rw_sgdt()
        {
            Run32bitTest("0F0100");	// sgdt	[eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|Mem0[eax:word48] = __sgdt<word48>()");
        }

        [Test]
        public void X86Rw_sidt()
        {
            Run32bitTest("0F018A86040500");	// sidt	[edx+00050486]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[edx + 0x50486<32>:word48] = __sidt<word48>()");
        }

        [Test]
        public void X86Rw_lldt()
        {
            Run32bitTest("0F00558D");	// lldt	word ptr [ebp-73]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__lldt<word16>(Mem0[ebp - 115<i32>:word16])");
        }

        [Test]
        public void X86Rw_ud0()
        {
            Run32bitTest("0FFFFF");	// ud0	edi,edi
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_ud1()
        {
            Run32bitTest("0FB900");	// ud1	eax,[eax]
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_lidt()
        {
            Run32bitTest("0F011B");	// lidt	[ebx]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__lidt<word48>(Mem0[ebx:word48])");
        }

        [Test]
        public void X86Rw_movbe()
        {
            Run32bitTest("0F38F1C3");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ebx = __movbe<word32>(eax)");
        }

        [Test]
        public void X86Rw_sha1msg2()
        {
            Run32bitTest("0F38CA75E8");	// sha1msg2	xmm6,[ebp-18]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = Mem0[ebp - 24<i32>:word128]",
                "2|L--|v6 = xmm6",
                "3|L--|xmm6 = __sha1msg2(v6, v5)");
        }

        [Test]
        public void X86Rw_smsw()
        {
            Run32bitTest("0F01E0");	// smsw	ax
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|eax = __smsw<word32>()");
        }

        [Test]
        public void X86Rw_lmsw()
        {
            Run32bitTest("0F01F0");	// lmsw	ax
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__load_machine_status_word(ax)");
        }

        [Test]
        public void X86Rw_cmpxchg8b()
        {
            Run32bitTest("0FC70F");	// cmpxchg8b	qword ptr [edi]
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __cmpxchg<word64>(edx_eax, Mem0[edi:word64], ecx_ebx, out edx_eax)");
        }

        [Test]
        public void X86Rw_unpckhps()
        {
            Run32bitTest("0F1510");	// unpckhps	xmm2,[eax]
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|xmm2 = __unpckhp<real32[4]>(xmm2, Mem0[eax:(arr real32 4)])");
        }

        [Test]
        public void X86Rw_ltr()
        {
            Run32bitTest("0F00983F051019");	// ltr	word ptr [eax+1910053F]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|__load_task_register(Mem0[eax + 0x1910053F<32>:word16])");
        }

        [Test]
        public void X86Rw_ffreep()
        {
            Run32bitTest("DFC7");	// ffreep	st(7)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|__ffree(ST[Top + 7<i8>:real64])",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86Rw_verr()
        {
            Run32bitTest("0F00A5640F00A5");	// verr	word ptr [ebp+A5000F64]
            AssertCode(
                "0|L--|10000000(7): 1 instructions",
                "1|L--|Z = __verify_readable(Mem0[ebp + 0xA5000F64<32>:word16])");
        }

        [Test]
        public void X86Rw_verw()
        {
            Run32bitTest("0F00EB");	// verw	bx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __verify_writeable(bx)");
        }

        [Test]
        public void X86Rw_str()
        {
            Run32bitTest("0F008BF38B4DFC");	// str	word ptr [ebx+FC4D8BF3]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[ebx + 0xFC4D8BF3<32>:word16] = __store_task_register()");
        }

        [Test]
        public void X86Rw_jmpe()
        {
            Run32bitTest("0FB8");	// jmpe
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__jmpe()");
        }


        [Test]
        public void X86Rw_femms()
        {
            Run32bitTest("0F0E");	// femms
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__femms()");
        }

        [Test]
        public void X86Rw_invlpg()
        {
            Run32bitTest("0F0138");	// invlpg	byte ptr [eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__invldpg<ptr32>(Mem0[eax:byte])");
        }

        [Test]
        public void X86Rw_rsm()
        {
            Run32bitTest("0FAA");	// rsm
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        /*
        R:push   0x8f865955<32>                        68 55 59 86 8F
        O:push   0xffffffff8f865955<64>                68 55 59 86 8F
        */

        /*
         * R:imul   eax,esi,0xea                      6B C6 EA
        O:imul   eax,esi,0xffffffea<32>                6B C6 EA
        */

        [Test]
        public void X86Rw_cdq()
        {
            Run32bitTest("99"); // cdq
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|edx_eax = CONVERT(eax, int32, int64)");
        }

        [Test]
        public void X86Rw_cwde()
        {
            Run32bitTest("98"); // cwde
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|eax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86Rw_icebp()
        {
            Run32bitTest("F1");
            AssertCode(
                "0|---|10000000(1): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_adc_imm8()
        {
            Run32bitTest("83 56 FE FF");
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = Mem0[esi - 2<i32>:word32] + 0xFFFFFFFF<32> + C",
                "2|L--|Mem0[esi - 2<i32>:word32] = v5",
                "3|L--|SCZO = cond(v5)");
        }

        [Test]
        public void X86rw_short_push()
        {
            Run32bitTest(
                "66 68 34 12"   // push word 1234h
                );
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|esp = esp - 2<i32>",
                "2|L--|Mem0[esp:word16] = 0x1234<16>");
        }

        [Test]
        public void X86Rw_fld_st0()
        {
            // This is a 'duplicate top of stack' instruction.
            Run32bitTest("D9 C0"); // fld st(0)");
            AssertCode(
              "0|L--|10000000(2): 3 instructions",
              "1|L--|v4 = ST[Top:real64]",
              "2|L--|Top = Top - 1<i8>",
              "3|L--|ST[Top:real64] = v4");
        }

        [Test]
        public void X86Rw_fsubr()
        {
            Run32bitTest("D8 AD 78 FF FF FF"); // fsubr dword ptr [ebp-00000088]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = CONVERT(Mem0[ebp - 0x88<32>:real32], real32, real64) - ST[Top:real64]");
        }





        [Test]
        public void X86Rw_adcx()
        {
            Run32bitTest("660F38F6C3");
            AssertCode(     // adcx eax,ebx
                "0|L--|10000000(5): 2 instructions",
                "1|L--|eax = eax + ebx + C",
                "2|L--|C = cond(eax)");
        }

        [Test]
        public void X86Rw_adox()
        {
            Run32bitTest("F30F38F6C4");
            AssertCode(     // adox eax,esp
                "0|L--|10000000(5): 2 instructions",
                "1|L--|eax = eax + esp + O",
                "2|L--|O = cond(eax)");
        }

        [Test]
        [Ignore("This isn't expressing the idea of 'eq'")]
        public void X86Rw_cmpeqps()
        {
            Run32bitTest("0FC20800");
            AssertCode(     // cmpeqps	xmm1,[eax]
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|v6 = xmm1",
                "3|L--|xmm1 = __cmpp<real32[4],word32[4]>(v5, v6, Mem0[eax:word128])");
        }

        [Test]
        public void X86Rw_fdiv_mem()
        {
            Run32bitTest("D8 B6 3C 01 00 00");  // fdiv dword ptr [esi+0000013C]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] / CONVERT(Mem0[esi + 0x13C<32>:real32], real32, real64)");
        }

        [Test]
        public void X86Rw_vzeroall()
        {
            Run32bitTest("C5F577");
            AssertCode(     // vzeroall
                "0|L--|10000000(3): 8 instructions",
                "1|L--|ymm0 = 0<256>",
                "2|L--|ymm1 = 0<256>",
                "3|L--|ymm2 = 0<256>",
                "4|L--|ymm3 = 0<256>",
                "5|L--|ymm4 = 0<256>",
                "6|L--|ymm5 = 0<256>",
                "7|L--|ymm6 = 0<256>",
                "8|L--|ymm7 = 0<256>");
        }
    }
}