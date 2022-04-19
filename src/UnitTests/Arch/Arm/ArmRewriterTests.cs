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

using NUnit.Framework;
using Reko.Arch.Arm;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public partial class ArmRewriterTests : RewriterTestBase
    {
        private readonly Arm32Architecture arch = new Arm32Architecture(CreateServiceContainer(), "arm32", new Dictionary<string, object>());
        private readonly Address baseAddress = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddress;

        public ArmRewriterTests()
        {
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
        }

        [Test]
        public void ArmRw_mov_r1_r2()
        {
            Given_BitStrings("1110 00 0 1101 0 0000 0001 00000000 0010");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2");
        }

        [Test]
        public void ArmRw_adc()
        {
            Given_UInt32s(0xE0A22002); // adc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r2 = r2 + r2 + C");
        }

        [Test]
        public void ArmRw_add_r1_r2_r3()
        {
            Given_BitStrings("1110 00 0 0100 0 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 + r3");
        }

        [Test]
        public void ArmRw_adds_r1_r2_r3()
        {
            Given_BitStrings("1110 00 0 0100 1 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r2 + r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_subgt_r1_r2_imm4()
        {
            Given_BitStrings("1100 00 1 0010 0 0010 0001 0000 00000100");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,NZV)) branch 00100004",
                "2|L--|r1 = r2 - 4<32>");
        }

        [Test]
        public void ArmRw_orr_r3_r4_r5_lsl_5()
        {
            Given_BitStrings("1110 00 0 1100 0 1100 0001 00100 000 0100");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = ip | r4 << 4<i32>");
        }

        [Test]
        public void ArmRw_bkpt()
        {
            Given_UInt32s(0xE1262B70);        // bkpt\t#&62B0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__breakpoint(0x62B0<32>)");
        }

        [Test]
        public void ArmRw_brgt()
        {
            Given_BitStrings("1100 1010 000000000000000000000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GT,NZV)) branch 00100008");
        }

        [Test]
        public void ArmRw_lsl()
        {
            Given_UInt32s(0xE1A00200);  // mov\tr0,r0,lsl #4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 << 4<i32>");
        }


        [Test]
        public void ArmRw_bllt()
        {
            Given_UInt32s(0xBB000330);  // bllt
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,NZV)) branch 00100004",
                "2|T--|call 00100CC8 (0)");
        }

        [Test]
        public void ArmRw_bne()
        {
            Given_UInt32s(0x1A000004);  // bne
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100018");
        }

        [Test]
        public void ArmRw_bic()
        {
            Given_UInt32s(0xE3CEB3FF);  // bic
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = lr & ~0xFC000003<32>");
        }

        [Test]
        public void ArmRw_mov_pc_lr()
        {
            Given_UInt32s(0xE1B0F00E);  // mov pc,lr
            AssertCode(
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_ldr()
        {
            Given_UInt32s(0xE5940008);  // ldr r0,[r4,#8]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r4 + 8<i32>:word32]");
        }

        [Test]
        public void ArmRw_ldr_pcindexed()
        {
            Given_UInt32s(0xE79F3003); // ldr r3,[pc, r3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[0x00100008<p32> + r3:word32]");
        }

        [Test]
        public void ArmRw_ldrexb()
        {
            Given_HexString("9622D9E1");
            AssertCode(     // ldrexb	r2,[r9]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __ldrex<byte>(Mem0[r9:void])");
        }

        [Test]
        public void ArmRw_ldrexd()
        {
            Given_HexString("9F07B6E1");
            AssertCode(     // ldrexd	r0,r1,[r6]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1_r0 = __ldrex<word64>(Mem0[r6:void])");
        }

        [Test]
        public void ArmRw_ldrexh()
        {
            Given_HexString("90BEF0E1");
            AssertCode(     // ldrexh	fp,[r0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = __ldrex<word16>(Mem0[r0:void])");
        }

        [Test]
        public void ArmRw_ldrh()
        {
            Given_UInt32s(0xE1D041BC);        // ldrh\tr4,[r0,#&1C]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r0 + 28<i32>:word16], uint16, word32)");
        }

        [Test]
        public void ArmRw_ldrsb()
        {
            Given_UInt32s(0xE1F120D1);  // ldrsb r2,[r1,#1]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + 1<i32>",
                "2|L--|r2 = CONVERT(Mem0[r1:int8], int8, word32)");
        }

        [Test]
        public void ArmRw_ldrsht()
        {
            Given_UInt32s(0xE0fe50fc);	// ldrsht r5, [lr], #0xc
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = CONVERT(Mem0[lr:int16], int16, word32)",
                "2|L--|lr = lr + 12<i32>",
                "3|L--|r5 = v4");
        }

        [Test]
        public void ArmRw_mov_pc()
        {
            Given_UInt32s(0xE59F0010);  // ldr\tr0,[pc,#&10]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[0x00100018<p32>:word32]");
        }

        [Test]
        public void ArmRw_clrex()
        {
            Given_HexString("1FF07FF5");
            AssertCode(     // clrex
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__clrex()");
        }

        [Test]
        public void ArmRw_cmp()
        {
            Given_UInt32s(0xE3530000);  // cmp r3,#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 - 0<32>)");
        }

        [Test]
        public void ArmRw_cmn()
        {
            Given_UInt32s(0xE3730001); /// cmn r3,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 + 1<32>)");
        }

        [Test]
        public void ArmRw_crc32cb()
        {
            Given_UInt32s(0xE10C4648);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r8, uint8, 0)",
                "2|L--|r4 = __crc32cb(ip, v4)");
        }

        [Test]
        public void ArmRw_crc32cw()
        {
            Given_UInt32s(0xE1408245);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __crc32cw(r0, r5)");
        }

        [Test]
        public void ArmRw_crc32h()
        {
            Given_UInt32s(0xE1248D4B);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(fp, uint16, 0)",
                "2|L--|r8 = __crc32h(r4, v4)");
        }

        [Test]
        public void ArmRw_crc32w()
        {
            Given_UInt32s(0xE14A8040);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __crc32w(r10, r0)");
        }

        [Test]
        public void ArmRw_hvc()
        {
            Given_UInt32s(0xE14C7472);        // hvc\t#&C742
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__hypervisor(0xC742<32>)");
        }

        [Test]
        public void ArmRw_lda()
        {
            Given_HexString("9F6C91E1");
            AssertCode(     // lda	r6,[r1]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __load_acquire<word32>(r1)");
        }

        [Test]
        public void ArmRw_ldab()
        {
            Given_HexString("9F0CD0E1");
            AssertCode(     // ldab	r0,[r0]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = __load_acquire<byte>(r0)",
                "2|L--|r0 = CONVERT(v3, byte, word32)");
        }

        [Test]
        public void ArmRw_ldaex()
        {
            Given_HexString("9F3E91E1");
            AssertCode(     // ldaex	r3,[r1]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __load_acquire_exclusive<word32>(r1)");
        }

        [Test]
        public void ArmRw_ldaexd()
        {
            Given_HexString("9F2EB0E1");
            AssertCode(     // ldaexd	r2,r3,[r0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2_r3 = __load_acquire_exclusive<word64>(r0)");
        }

        [Test]
        public void ArmRw_ldaexh()
        {
            Given_HexString("90DFF0E1");
            AssertCode(     // ldaexh	sp,[r0]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = __load_acquire_exclusive<word16>(r0)",
                "2|L--|sp = CONVERT(v4, word16, word32)");
        }

        [Test]
        public void ArmRw_ldr_pc()
        {
            Given_UInt32s(0xE59CF000); // ldr pc,[ip]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[ip:word32]");
        }

        [Test]
        public void ArmRw_pkhtb()
        {
            Given_UInt32s(0xB6847751);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,NZV)) branch 00100004",
                "2|L--|r7 = __pkhtb(r4, r1 >> 14<i32>)");
        }

        [Test]
        public void ArmRw_ldr_post()
        {
            Given_UInt32s(0xE4D43001);// ldrb r3,[r4],#1
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = CONVERT(Mem0[r4:byte], byte, word32)",
                "2|L--|r4 = r4 + 1<i32>",
                "3|L--|r3 = v4");
        }

        [Test]
        public void ArmRw_push()
        {
            Given_UInt32s(0xE92D4010);
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|Mem0[sp + -8<i32>:word32] = r4",
               "2|L--|Mem0[sp + -4<i32>:word32] = lr",
               "3|L--|sp = sp - 8<i32>");
        }

        [Test]
        public void ArmRw_movw()
        {
            Given_UInt32s(0xE30F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = 0xFFFF<32>");
        }

        [Test]
        public void ArmRw_uxtb()
        {
            Given_UInt32s(0xE6EF2071);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = CONVERT(SLICE(r1, byte, 0), byte, uint32)");
            Given_UInt32s(0xE6EF2471);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = CONVERT(SLICE(__ror<word32,int32>(r1, 8<i32>), byte, 0), byte, uint32)");
        }

        [Test]
        public void ArmRw_bxuge()
        {
            Given_UInt32s(0x212FFF1E);
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULT,C)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_bxj()
        {
            Given_UInt32s(0xE1204620);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r0");
        }


        [Test]
        public void ArmRw_movt()
        {
            Given_UInt32s(0xE34F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = SEQ(0xFFFF<16>, SLICE(r4, word16, 0))");
        }

        [Test]
        public void ArmRw_pop()
        {
            Given_UInt32s(0xE8BD000C);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r2 = Mem0[sp:word32]",
                "2|L--|r3 = Mem0[sp + 4<i32>:word32]",
                "3|L--|sp = sp + 8<i32>");
        }

        [Test]
        public void ArmRw_popne()
        {
            Given_UInt32s(0x18BD000C);
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r2 = Mem0[sp:word32]",
                "3|L--|r3 = Mem0[sp + 4<i32>:word32]",
                "4|L--|sp = sp + 8<i32>");
        }

        [Test]
        public void ArmRw_pop_pc()
        {
            Given_UInt32s(0xE49DF004);  //  pop pc
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_pop_many_including_pc()
        {
            Given_UInt32s(0xE8BD80F0);
            AssertCode(     // pop { r4 - r7,pc}
                "0|T--|00100000(4): 7 instructions",
                "1|L--|r4 = Mem0[sp:word32]",
                "2|L--|r5 = Mem0[sp + 4<i32>:word32]",
                "3|L--|r6 = Mem0[sp + 8<i32>:word32]",
                "4|L--|r7 = Mem0[sp + 12<i32>:word32]",
                "5|L--|v7 = Mem0[sp + 16<i32>:word32]",
                "6|L--|sp = sp + 20<i32>",
                "7|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_clz()
        {
            Given_UInt32s(0xE16F4F13);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __clz(r3)");
        }

        [Test]
        public void ArmRw_strd()
        {
            Given_UInt32s(0xE04343F8);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r3:word64] = r5_r4",
                "2|L--|r3 = r3 - 56<i32>");
        }

        [Test]
        public void ArmRw_muls()
        {
            Given_UInt32s(0xE0120A94);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r4 * r10",
                "2|L--|NZCV = cond(r2)");
        }

        [Test]
        public void ArmRw_mlas()
        {
            Given_UInt32s(0xE0314392);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r4 + r2 * r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_bfi()
        {
            Given_UInt32s(0xE7CD1292);
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|v4 = SLICE(r2, ui9, 0)",
               "2|L--|r1 = SEQ(SLICE(r1, word18, 14), v4, SLICE(r1, word5, 0))");
        }

        /*
        MOV             R2, R0,LSR#8
        MOV             R3, #0
        AND             R0, R0, #0xF
        BFI             R3, R2, #0, #8
        SUB             SP, SP, #8
        BFI             R3, R0, #8, #8
        MOV             R0, R3
        ADD             SP, SP, #8
        BX              LR
        int __fastcall AUDIO_ConvertVolumeValue(unsigned __int16 a1)
        {
          return (a1 >> 8) | ((a1 & 0xF) << 8);
        }

        MOV             R3, #0
MOV             R1, #0xD
MOV             R0, R3
BFI             R0, R1, #0, #8
BFI             R0, R3, #8, #8
BFI             R0, R2, #0x10, #8
ADD             SP, SP, #8
BX              LR
means
  return (v3 << 16) | 0xD;
         */

        [Test]
        public void ArmRw_ldrd()
        {
            Given_UInt32s(0xE1C722D8);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3_r2 = Mem0[r7 + 40<i32>:word64]");
        }

        [Test]
        public void ArmRw_ubfx()
        {
            Given_UInt32s(0xE7F01252);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = CONVERT(SLICE(r2, ui17, 4), ui17, uint32)");
        }

        [Test]
        public void ArmRw_sxtb()
        {
            Given_UInt32s(0xE6AF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = CONVERT(SLICE(__ror<word32,int32>(r2, 8<i32>), int8, 0), int8, int32)");
        }

        [Test]
        public void ArmRw_uxth()
        {
            Given_UInt32s(0xE6FF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = CONVERT(SLICE(__ror<word32,int32>(r2, 8<i32>), uint16, 0), uint16, uint32)");
        }

        [Test]
        public void ArmRw_umull()
        {
            Given_UInt32s(0xE0912394);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1_r2 = r3 *u64 r4",
                "2|L--|NZCV = cond(r1_r2)");
        }

        [Test]
        public void ArmRw_mls()
        {
            Given_UInt32s(0xE0612394);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 - r4 * r3");
        }


        [Test]
        public void ArmRw_vbit()
        {
            Given_UInt32s(0xF324F19E);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vbit(d20, d14)");
        }

        [Test]
        public void ArmRw_vld1()
        {
            Given_HexString("8F0A63F4");
            AssertCode(     // vld1.i32	{d16,d17},[r3]
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = __vld1_multiple<int32>(r3)",
                "2|L--|d16 = SLICE(v2, word64, 64)",
                "3|L--|d17 = SLICE(v2, word64, 0)");
        }

        [Test]
        public void ArmRw_vld2()
        {
            Given_HexString("4D436BF4");
            AssertCode(     // vld2.i8	{d20,d22},[fp]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v2 = __vld2_multiple<int8>(fp)",
                "2|L--|d20 = SLICE(v2, word64, 64)",
                "3|L--|d22 = SLICE(v2, word64, 0)",
                "4|L--|fp = fp + 16<i32>");
        }

        [Test]
        public void ArmRw_vld3_multi_postinc()
        {
            Given_HexString("0D4522F4");
            AssertCode(     // vld3.i8 {d4,d6,d8},[r2]!
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v2 = __vld3_multiple<int8>(r2)",
                "2|L--|d4 = SLICE(v2, word64, 128)",
                "3|L--|d6 = SLICE(v2, word64, 64)",
                "4|L--|d8 = SLICE(v2, word64, 0)",
                "5|L--|r2 = r2 + 24<i32>");
        }

        [Test]
        public void ArmRw_vld4_multiple()
        {
            Given_HexString("8DA128F4");
            AssertCode(     // vld4.i32	{d10-d13},[r8]
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v2 = __vld4_multiple<int32>(r8)",
                "2|L--|d10 = SLICE(v2, word64, 192)",
                "3|L--|d11 = SLICE(v2, word64, 128)",
                "4|L--|d12 = SLICE(v2, word64, 64)",
                "5|L--|d13 = SLICE(v2, word64, 0)",
                "6|L--|r8 = r8 + 32<i32>");
        }

        [Test]
        public void ArmRw_vld4_multiple_postinc()
        {
            Given_HexString("0D8167F4");
            AssertCode(     // vld4.i8	{d24-d27},[r7]!
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v2 = __vld4_multiple<int8>(r7)",
                "2|L--|d24 = SLICE(v2, word64, 192)",
                "3|L--|d25 = SLICE(v2, word64, 128)",
                "4|L--|d26 = SLICE(v2, word64, 64)",
                "5|L--|d27 = SLICE(v2, word64, 0)",
                "6|L--|r7 = r7 + 32<i32>");
        }

        [Test]
        public void ArmRw_vmin_f32()
        {
            Given_HexString("011F20F2");
            AssertCode(     // vmin.f32\td1,d0,d1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = __vmin_f32(d0, d1)");
        }


        [Test]
        public void ArmRw_vldmia_update()
        {
            //04 0B F2 EC
            //04 0B E3 EC
            Given_UInt32s(0xECF20B04);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|d16 = Mem0[r2:word64]",
                "2|L--|d17 = Mem0[r2 + 8<i32>:word64]",
                "3|L--|r2 = r2 + 16<i32>");
        }

        [Test]
        public void ArmRw_ldmib()
        {
            Given_UInt32s(0xE9950480); // ldmibr5, r7, r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = Mem0[r5 + 4<i32>:word32]",
                "2|L--|r10 = Mem0[r5 + 8<i32>:word32]");
        }

        [Test]
        public void ArmRw_ldr_literal()
        {
            Given_UInt32s(0xE59F5254);        // ldr\tr5,[pc,#&254]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = Mem0[0x0010025C<p32>:word32]");
        }



        [Test]
        public void ArmRw_sbc()
        {
            Given_UInt32s(0xE0C22002); // sbc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r2 = r2 - r2 - C");
        }

        [Test]
        public void ArmRw_mrs()
        {
            Given_UInt32s(0xE10F3000); // mrs r3, cpsr
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r3 = __mrs(cpsr)");
        }

        [Test]
        public void ArmRw_cpsid()
        {
            Given_UInt32s(0xF10C0080); // cpsid
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__cps_id()");
        }

        [Test]
        public void ArmRw_smull()
        {
            Given_UInt32s(0xE0CE3C9E);  // smull r3,lr,lr,ip
            AssertCode(
           "0|L--|00100000(4): 1 instructions",
           "1|L--|lr_r3 = ip *s64 lr");
        }

        [Test]
        public void ArmRw_smulbb()
        {
            Given_UInt32s(0xE1600380); //  smulbb r0, r0, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = CONVERT(r0, word32, int16) *s CONVERT(r3, word32, int16)");
        }

        [Test]
        public void ArmRw_bfc()
        {
            Given_UInt32s(0xE7C5901F);  // bfc r9, #0, #6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r9 & 0xFFFFFFC0<u32>");
        }

        [Test]
        public void ArmRw_sbfx()
        {
            Given_UInt32s(0xE7A9C35C); // sbfx ip,ip,#6,#&A
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = CONVERT(SLICE(ip, ui10, 6), ui10, int32)");
        }

        [Test]
        public void ArmRw_umlalne()
        {
            Given_UInt32s(0x10A54A93); // umlalne r4,r5,r3,r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r5_r4 = r3 *u r10 + r5_r4");
        }

        [Test]
        public void ArmRw_msr()
        {
            Given_UInt32s(0xE121F001); // msr cpsr, r1
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|__msr(cpsr, r1)");
        }

        [Test]
        public void ArmRw_uxtab()
        {
            Given_UInt32s(0xE6E10070);  // uxtab r0, r1, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r1 + CONVERT(SLICE(r0, byte, 0), byte, word32)");
        }

        [Test]
        public void ArmRw_sxtab()
        {
            Given_UInt32s(0xE6A55078);  // sxtab r5, r5, r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 + CONVERT(SLICE(r8, int8, 0), int8, word32)");
        }

        [Test]
        public void ArmRw_sxtah()
        {
            Given_UInt32s(0xE6B6A07A);  // sxtah r10,r6,r10
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|r10 = r6 + CONVERT(SLICE(r10, int16, 0), int16, word32)");
        }

        [Test]
        public void ArmRw_sxthne()
        {
            Given_UInt32s(0x16BF9077);  //  sxthne r9,r7
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004");
        }

        [Test]
        public void ArmRw_uxtah()
        {
            Given_UInt32s(0xE6F30072);  // uxtah r0,r3,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r3 + CONVERT(SLICE(r2, uint16, 0), uint16, word32)");
        }

        [Test]
        public void ArmRw_dmb()
        {
            Given_UInt32s(0xF57FF05F);  // dmb
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|__dmb_sy()");
        }

        [Test]
        public void ArmRw_mrc()
        {
            Given_UInt32s(0xEE123F10);  // mrc p15,#0,r3,c2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __mrc(p15, 0<32>, cr2, cr0, 0<32>)");
        }

        // Only present in old ARM models
        public void ArmRw_mrrc()
        {
            Given_UInt32s(0xEC565554);        // mrrc\tp5,#5,r5,r6,c4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5_r6 = __mrrc(p5, 5<32>, cr4)");
        }

        [Test]
        public void ArmRw_mcr()
        {
            Given_UInt32s(0xEE070F58);  // mcr p15,#0,r0,c7,c8,#2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mcr(p15, 0<32>, r0, cr7, cr8, 2<32>)");
        }

        [Test]
        public void ArmRw_bl()
        {
            Given_UInt32s(0xEB00166B);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 001059B4 (0)");
        }

        [Test]
        public void ArmRw_rsc()
        {
            // Capstone incorrectly disassembles this as setting the S flag.
            Given_UInt32s(0x00E050CC); // rsceq asr r5,r0,ip, asr #1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r5 = (ip >> 1<i32>) - r0 - C");
        }

        [Test]
        public void ArmRw_smlawt()
        {
            Given_UInt32s(0x012D06CC); // smlawteq sp,ip,r6,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|sp = (ip *s32 CONVERT(r6 >> 16<i32>, word32, int16) >> 16<i32>) + r0");
            //74 EC 0A 01 ????
            //﻿90 41 E0 00 smlaleqr4,r0,r0,r1
            // B0 44 E0 00 strhteqr4,[r0],#&40
            //﻿ A8 5B 2E 01 smulwbeqlr,r8,fp
        }

        [Test]
        public void ArmRw_smlal()
        {
            Given_UInt32s(0xE0e04190);	// smlal r4, r0, r0, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_r0 = r0 *s r1 + r4_r0");
        }

        [Test]
        public void ArmRw_strht()
        {
            Given_UInt32s(0xE0e051b0);	// strht r5, [r0], #0x10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r0:word16] = SLICE(r5, uint16, 0)");
        }

        [Test]
        public void ArmRw_swpeq()
        {
            Given_UInt32s(0xE10ea598);	// swp sl, r8, [lr]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = std::atomic_exchange<int32_t>(r8, Mem0[lr:word32])");
        }

        [Test]
        public void ArmRw_smulwb()
        {
            //$REVIEW: shoudln't the CONVERT be a SLICE?
            Given_UInt32s(0xE12e5ba8);	// smulwb lr, r8, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = r8 *s32 CONVERT(fp, word32, int16) >> 16<i32>");
        }

        [Test]
        public void ArmRw_smulbt()
        {
            Given_UInt32s(0xE168dbcc);	// smulbt r8, ip, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = CONVERT(ip, word32, int16) *s CONVERT(fp >> 16<i32>, word32, int16)");
        }

        [Test]
        public void ArmRw_qdsub()
        {
            Given_UInt32s(0xE168da50);	// qdsub sp, r0, r8
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|sp = __signed_sat_32(r0 - __signed_sat_32(r8 *s 2<i32>))",
                "2|L--|Q = cond(sp)");
        }



        [Test]
        public void ArmRw_smultt()
        {
            Given_UInt32s(0xE168dbe0);	// smultt r8, r0, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = CONVERT(r0 >> 16<i32>, word32, int16) *s CONVERT(fp >> 16<i32>, word32, int16)");
        }

        [Test]
        public void ArmRw_qadd()
        {
            Given_UInt32s(0xE10fb85c);	// qadd fp, ip, pc
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|fp = __signed_sat_32(ip + 0x00100008<p32>)",
                "2|L--|Q = cond(fp)");
        }

        [Test]
        public void ArmRw_qsax()
        {
            Given_UInt32s(0xE6208D59);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __qsax(r0, r9)");
        }

        [Test]
        public void ArmRw_qsub()
        {
            Given_UInt32s(0xE12d6650);	// qsube r6, r0, sp
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = __signed_sat_32(r0 - sp)",
                "2|L--|Q = cond(r6)");
        }

        [Test]
        public void ArmRw_smlatb()
        {
            Given_UInt32s(0xE10c6ca0);	// smlatb ip, r0, ip, r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|ip = CONVERT(r0 >> 16<i32>, word32, int16) *s CONVERT(ip, word32, int16) + r6",
                "2|L--|Q = cond(ip)");
        }

        [Test]
        public void ArmRw_ldrht()
        {
            Given_UInt32s(0xE0fd52b4);	// ldrht r5, [sp], #0x24
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = CONVERT(Mem0[sp:word16], uint16, word32)",
                "2|L--|sp = sp + 36<i32>",
                "3|L--|r5 = v4");
        }

        [Test]
        public void ArmRw_smulwt()
        {
            Given_UInt32s(0xE1206aec);	// smulwt r0, ip, sl
            AssertCode("0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = ip *s32 CONVERT(r10 >> 16<i32>, word32, int16) >> 16<i32>");
        }

        [Test]
        public void ArmRw_smlawb()
        {
            Given_UInt32s(0xE12d5980);	// smlawb sp, r0, sb, r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = (r0 *s32 CONVERT(r9, word32, int16) >> 16<i32>) + r5");
        }

        [Test]
        public void ArmRw_ldrsbteq()
        {
            Given_UInt32s(0x00F707D0);	// ldrsbteq r0, [r7], #0x70
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|v5 = CONVERT(Mem0[r7:int8], int8, word32)",
                "3|L--|r7 = r7 + 112<i32>",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_smultb()
        {
            Given_UInt32s(0xE16c69ac);	// smultb ip, ip, sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = CONVERT(ip >> 16<i32>, word32, int16) *s CONVERT(r9, word32, int16)");
        }

        [Test]
        public void ArmRw_vst1_multi()
        {
            Given_HexString("8F0744F4");
            AssertCode(     // vst1.i32	{d16},[r4]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__vst1_multiple<int32,word64>(d16, r4)");
        }

        [Test]
        public void ArmRw_vst2_multi_postinc()
        {
            Given_HexString("0DC343F4");
            AssertCode(     // vst2.i8	{d28,d29},[r3]!
                "0|L--|00100000(4): 2 instructions",
                "1|L--|__vst2_multiple<int8,word128>(d28_d29, r3)",
                "2|L--|r3 = r3 + 16<i32>");
        }

        [Test]
        public void ArmRw_vst3()
        {
            Given_HexString("0D4502F4");
            AssertCode(     // vst3.i8	{d4,d6,d8,d10},[r2]!
                "0|L--|00100000(4): 2 instructions",
                "1|L--|__vst3_multiple<int16,word256>(d4_d6_d8_d10, r2)",
                "2|L--|r2 = r2 + 32<i32>");
        }

        [Test]
        public void ArmRw_vstr()
        {
            Given_UInt32s(0xedcd0b29);	// vstr d16, [sp, #0xa4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 164<i32>:word64] = d16");
        }

        [Test]
        public void ArmRw_vldr()
        {
            Given_UInt32s(0xedd20b04);	// vldr d16, [r2, #0x10]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = Mem0[r2 + 16<i32>:word64]");
        }

        [Test]
        public void ArmRw_veor()
        {
            Given_UInt32s(0xf34001f4);	// veor q8, q8, q10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q8 ^ q10");
        }

        [Test]
        public void ArmRw_vmov_32()
        {
            Given_UInt32s(0xee102b90);	// vmov.32 r2, d16[0]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = d16[0<i32>]");
        }

        [Test]
        public void ArmRw_vmov_i32_128bit()
        {
            Given_UInt32s(0xF2C00051); // vmov.i32 q8,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = SEQ(0x100000001<64>, 0x100000001<64>)");    //$TODO: support for 128-bit and larger constants LIT: maybe a <128>?
        }

        [Test]
        public void ArmRw_vmov_i32_64bit()
        {
            Given_UInt32s(0xF2C00011); // vmov.i32 d16,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = 0x100000001<64>");
        }

        [Test]
        public void ArmRw_smlabt()
        {
            Given_UInt32s(0xE10f54cc);  // smlabt pc, ip, r4, r5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|pc = CONVERT(ip, word32, int16) *s CONVERT(r4 >> 16<i32>, word32, int16) + r5",
                "2|L--|Q = cond(pc)");
        }

        [Test]
        public void ArmRw_vcvt()
        {
            Given_HexString("C77BB7EE");
            AssertCode(     // vcvt.f32.f64	s14,d7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s14 = CONVERT(d7, real64, real32)");
        }

        [Test]
        public void ArmRw_vcvt_f64_s32()
        {
            Given_UInt32s(0xeef80be7);  // vcvt.f64.s32 d16, s15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = CONVERT(s15, int32, real64)");
        }

        [Test]
        public void ArmRw_vcvt_f32_f64()
        {
            Given_HexString("E28BB7EE");
            AssertCode(     // vcvt.f32.f64	s16,d18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s16 = CONVERT(d18, real64, real32)");
        }

        [Test]
        public void ArmRw_vpush()
        {
            Given_UInt32s(0xed2d8b04);  // vpush {d8, d9}
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|Mem0[sp + -16<i32>:word64] = d8",
                "2|L--|Mem0[sp + -8<i32>:word64] = d9",
                "3|L--|sp = sp - 16<i32>");
        }

        [Test]
        public void ArmRw_vpop()
        {
            Given_UInt32s(0xecbd8b04);  // vpop {d8, d9}
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|d8 = Mem0[sp:word64]",
                "2|L--|d9 = Mem0[sp + 8<i32>:word64]",
                "3|L--|sp = sp + 16<i32>");
        }

        [Test]
        public void ArmRw_vsub_f64()
        {
            Given_UInt32s(0xee711be0);  // vsub.f64 d17, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d17 = __vsub_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_vmul_f64()
        {
            Given_UInt32s(0xee611ba0);  // vmul.f64 d17, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d17 = __vmul_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_vdiv_f64()
        {
            Given_UInt32s(0xee817ba0);  // vdiv.f64 d7, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = d17 / d16");
        }

        [Test]
        public void ArmRw_vcmpe_f32()
        {
            Given_UInt32s(0xeeb49ae7);  // vcmpe.f32 s18, s15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s18 - s15)");
        }

        [Test]
        public void ArmRw_vmrs()
        {
            Given_UInt32s(0xeef1fa10);  // vmrs apsr_nzcv, fpscr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = fpscr");
        }

        [Test]
        public void ArmRw_vnmls_f32()
        {
            Given_UInt32s(0xee567a87);  // vnmls.f32 s15, s13, s14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s15 = __vnmls_f32(s13, s14)");
        }

        [Test]
        public void ArmRw_vmla_f32()
        {
            Given_UInt32s(0xee476a86);  // vmla.f32 s13, s15, s12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s13 = __vmla_f32(s15, s12)");
        }

        [Test]
        public void ArmRw_ldrbtgt()
        {
            Given_UInt32s(0xc47a0000);  // ldrbtgt r0, [sl], #-0
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(LE,NZV)) branch 00100004",
                "2|L--|v5 = CONVERT(Mem0[r10:byte], byte, word32)",
                "3|L--|r10 = r10 + 0<i32>",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_vmax_s32()
        {
            Given_UInt32s(0xf26006e2);  // vmax.s32 q8, q8, q9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = __vmax_s32(q8, q9)");
        }

        [Test]
        public void ArmRw_vmin_s32()
        {
            Given_UInt32s(0xf26446f0);  // vmin.s32 q10, q10, q8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmin_s32(q10, q8)");
        }

        [Test]
        public void ArmRw_vorr()
        {
            Given_UInt32s(0xf26021b0);  // vorr d18, d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d18 = d16 | d16");
        }

        [Test]
        public void ArmRw_vpmax_s32()
        {
            Given_UInt32s(0xf2600aa0);  // vpmax.s32 d16, d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vpmax_s32(d16, d16)");
        }

        [Test]
        public void ArmRw_vpmin_s32()
        {
            Given_UInt32s(0xf2644ab4);  // vpmin.s32 d20, d20, d20
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vpmin_s32(d20, d20)");
        }

        [Test]
        public void ArmRw_smlabb()
        {
            Given_UInt32s(0xE10e3b88);  // smlabb lr, r8, fp, r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|lr = CONVERT(r8, word32, int16) *s CONVERT(fp, word32, int16) + r3");
        }

        [Test]
        public void ArmRw_stmda()
        {
            Given_UInt32s(0xE80230fd);  // stmda r2, {r0, r2, r3, r4, r5, r6, r7, ip, sp} ^
            AssertCode(
                "0|L--|00100000(4): 9 instructions",
                "1|L--|Mem0[r2 + -32<i32>:word32] = r0",
                "2|L--|Mem0[r2 + -28<i32>:word32] = r2",
                "3|L--|Mem0[r2 + -24<i32>:word32] = r3",
                "4|L--|Mem0[r2 + -20<i32>:word32] = r4",
                "5|L--|Mem0[r2 + -16<i32>:word32] = r5",
                "6|L--|Mem0[r2 + -12<i32>:word32] = r6",
                "7|L--|Mem0[r2 + -8<i32>:word32] = r7",
                "8|L--|Mem0[r2 + -4<i32>:word32] = ip",
                "9|L--|Mem0[r2:word32] = sp");
        }

        [Test]
        public void ArmRw_stmda_writeback()
        {
            Given_UInt32s(0xE82230fd);  // stmda r2!, {r0, r2, r3, r4, r5, r6, r7, ip, sp} ^
            AssertCode(
                "0|L--|00100000(4): 10 instructions",
                "1|L--|Mem0[r2 + -32<i32>:word32] = r0",
                "2|L--|Mem0[r2 + -28<i32>:word32] = r2",
                "3|L--|Mem0[r2 + -24<i32>:word32] = r3",
                "4|L--|Mem0[r2 + -20<i32>:word32] = r4",
                "5|L--|Mem0[r2 + -16<i32>:word32] = r5",
                "6|L--|Mem0[r2 + -12<i32>:word32] = r6",
                "7|L--|Mem0[r2 + -8<i32>:word32] = r7",
                "8|L--|Mem0[r2 + -4<i32>:word32] = ip",
                "9|L--|Mem0[r2:word32] = sp",
                "10|L--|r2 = r2 - 36<i32>");
        }

        [Test]
        public void ArmRw_stmdb()
        {
            Given_UInt32s(0xE92C003B);  // stmdb ip!,{r0,r1,r3-r5},lr,pc}
            AssertCode(
                "0|L--|00100000(4): 6 instructions",
                "1|L--|Mem0[ip + -20<i32>:word32] = r0",
                "2|L--|Mem0[ip + -16<i32>:word32] = r1",
                "3|L--|Mem0[ip + -12<i32>:word32] = r3",
                "4|L--|Mem0[ip + -8<i32>:word32] = r4",
                "5|L--|Mem0[ip + -4<i32>:word32] = r5",
                "6|L--|ip = ip - 20<i32>");
        }


        [Test]
        public void ArmRw_vneg_f64()
        {
            Given_UInt32s(0xeef10b60);  // vneg.f64 d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vneg_f64(d16)");
        }

        [Test]
        public void ArmRw_vnmul_f64()
        {
            Given_UInt32s(0xee680b60);  // vnmul.f64 d16, d8, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vnmul_f64(d8, d16)");
        }

        // Only present in old ARM models
        public void ArmRw_cdplo()
        {
            Given_UInt32s(0x3e200000);  // cdplo p0, #2, c0, c0, c0, #0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|__cdp(p0, 2<32>, cr0, cr0, cr0, 0<32>)");
        }

        [Test]
        public void ArmRw_vpadd_i32()
        {
            Given_UInt32s(0xf2622bb2);  // vpadd.i32 d18, d18, d18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d18 = __vpadd_i32(d18, d18)");
        }

        [Test]
        public void ArmRw_strbt()
        {
            Given_UInt32s(0xE6666666);  // strbt r6, [r6], -r6, ror #12
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r6:byte] = SLICE(r6, byte, 0)",
                "2|L--|r6 = r6 - __ror<word32,int32>(r6, 12<i32>)");
        }

        [Test]
        public void ArmRw_rrx()
        {
            Given_UInt32s(0xE1B00061); // rrxs r0, r1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = __rcr<word32,int32>(r1, 1<i32>, C)",
                "2|L--|NZC = cond(r0)");
        }

        // Only present in old ARM models
        public void ArmRw_stc()
        {
            Given_UInt32s(0xECCC5CED);  // stc p12, c12, [ip], {0xcd}
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stc(p12, cr5, Mem0[ip:word32])");
        }

        // Only present in old ARM models
        public void ArmRw_ldc()
        {
            Given_UInt32s(0xECDC5CED);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = Mem0[ip:word32]",
                "2|L--|p12 = __ldc(cr5, v3)");
        }

        [Test]
        public void ArmRw_vdup_32()
        {
            Given_UInt32s(0xeea02b90);	// vdup.32 q8, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = __vdup_32(r2)");
        }

        [Test]
        public void ArmRw_vmvn_i32()
        {
            Given_UInt32s(0xf2c04077);  // vmvn.i32 q10, #7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmvn_imm_i32(0x700000007<64>)");
        }

        [Test]
        public void ArmRw_vshl_u32()
        {
            Given_UInt32s(0xF36424E2);  // vshl.u32 q9, q9, q10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = __vshl_u32(q9, q10)");
        }

        [Test]
        public void ArmRw_vmls_f64()
        {
            Given_UInt32s(0xee017be0);  // vmls.f64 d7, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = __vmls_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_ldmdalt()
        {
            Given_UInt32s(0xB811EB85);  // ldmdalt r1, {r0, r2, r7, r8, sb, fp, sp, lr, pc} ^
            AssertCode(
                "0|T--|00100000(4): 11 instructions",
                "1|T--|if (Test(GE,NZV)) branch 00100004",
                "2|L--|r0 = Mem0[r1:word32]",
                "3|L--|r2 = Mem0[r1 - 4<i32>:word32]",
                "4|L--|r7 = Mem0[r1 - 8<i32>:word32]",
                "5|L--|r8 = Mem0[r1 - 12<i32>:word32]",
                "6|L--|r9 = Mem0[r1 - 16<i32>:word32]",
                "7|L--|fp = Mem0[r1 - 20<i32>:word32]",
                "8|L--|sp = Mem0[r1 - 24<i32>:word32]",
                "9|L--|lr = Mem0[r1 - 28<i32>:word32]",
                "10|L--|v12 = Mem0[r1 - 32<i32>:word32]",
                "11|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_vabs_f64()
        {
            Given_UInt32s(0xeeb09bc9);  // vabs.f64 d9, d9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d9 = __vabs_f64(d9)");
        }

        [Test]
        public void ArmRw_vadd_f64()
        {
            Given_UInt32s(0xee377b20);  // vadd.f64 d7, d7, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = __vadd_f64(d7, d16)");
        }

        [Test]
        public void ArmRw_vand()
        {
            Given_UInt32s(0xf24001f2);  // vand q8, q8, q9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q8 & q9");
        }

        [Test]
        public void ArmRw_vcmp_f32()
        {
            Given_UInt32s(0xeef47a47);  // vcmp.f32 s15, s14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s15 - s14)");
        }

        [Test]
        public void ArmRw_vsqrt_f64()
        {
            Given_UInt32s(0xeeb1cbe0);  // vsqrt.f64 d12, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d12 = sqrt(d16)");
        }

        [Test]
        public void ArmRw_umaal()
        {
            Given_UInt32s(0xE040a590);  // umaal sl, r0, r0, r5
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = r0 *u r5",
                "2|L--|v2 = v2 + CONVERT(r0, word32, uint64)",
                "3|L--|r0_r10 = v2 + CONVERT(r10, word32, uint64)");
        }

        [Test]
        public void ArmRw_smlatteq()
        {
            Given_UInt32s(0x010bdae4);  // smlatteq fp, r4, sl, sp
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|fp = CONVERT(r4 >> 16<i32>, word32, int16) *s CONVERT(r10 >> 16<i32>, word32, int16) + sp",
                "3|L--|Q = cond(fp)");
        }

        [Test]
        public void ArmRw_qdaddeq()
        {
            Given_UInt32s(0x01408e50);  // qdaddeq r8, r0, r0
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r8 = __signed_sat_32(r0 + __signed_sat_32(r0 *s 2<i32>))");
        }

        [Test]
        public void ArmRw_smlalbt()
        {
            Given_UInt32s(0xE14090c0);  // smlalbt sb, r0, r0, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = CONVERT(r0, word32, int16) *s CONVERT(r0 >> 16<i32>, word32, int16) + r9_r0");
        }

        [Test]
        public void ArmRw_swpb()
        {
            Given_UInt32s(0xE1409190);  // swpb sb, r0, [r0]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = std::atomic_exchange<byte>(r0, Mem0[r0:byte])");
        }

        [Test]
        public void ArmRw_smlaltb()
        {
            Given_UInt32s(0xE14091a0);  // smlaltb sb, r0, r0, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = CONVERT(r0 >> 16<i32>, word32, int16) *s CONVERT(r1, word32, int16) + r9_r0");
        }

        [Test]
        public void ArmRw_strt()
        {
            Given_UInt32s(0xE6247800);  // strt r7, [r4], -r0, lsl #16
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r4:word32] = r7",
                "2|L--|r4 = r4 - (r0 << 16<i32>)");
        }

        [Test]
        public void ArmRw_ldrtmi()
        {
            Given_UInt32s(0x44340000);  // ldrtmi r0, [r4], #-0
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(GE,N)) branch 00100004",
                "2|L--|v5 = Mem0[r4:word32]",
                "3|L--|r4 = r4 + 0<i32>",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_smlalbb()
        {
            Given_UInt32s(0xE1409280);  // smlalbb sb, r0, r0, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = CONVERT(r0, word32, int16) *s CONVERT(r2, word32, int16) + r9_r0");
        }

        [Test]
        public void ArmRw_smlaltt()
        {
            Given_UInt32s(0xE140abec);  // smlaltt sl, r0, ip, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10_r0 = CONVERT(ip >> 16<i32>, word32, int16) *s CONVERT(fp >> 16<i32>, word32, int16) + r10_r0");
        }

        [Test]
        public void ArmRw_ldrls_pc()
        {
            Given_UInt32s(0x979FF103);   // ldrls\tpc,[pc,r3,lsl #2]
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGT,ZC)) branch 00100004",
                "2|T--|goto Mem0[0x00100008<p32> + r3 * 4<i32>:word32]");
        }

        [Test]
        public void ArmRw_svc()
        {
            Given_UInt32s(0xEF001234); // svc 0x1234<16>
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__syscall(0x1234<32>)");
        }

        [Test]
        public void ArmRw_ReadPC()
        {
            Given_UInt32s(0xE08FE00E);  // add lr, pc, lr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = 0x00100008<p32> + lr");
        }

        [Test]
        public void ArmRw_ldrsb_positive_indexed()
        {
            Given_UInt32s(0xE19120D3);  // ldrsb\tr2,[r1,-r3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[r1 + r3:int8], int8, word32)");
        }

        [Test]
        public void ArmRw_ldrsb_negative_indexed()
        {
            Given_UInt32s(0xE11120D3); // ldrsb\tr2,[r1, r3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[r1 - r3:int8], int8, word32)");
        }

        [Test]
        public void ArmRw_setend()
        {
            Given_UInt32s(0xF1010200);      // setend be
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__set_bigendian(true)");
        }


        [Test]
        public void ArmRw_rev()
        {
            Given_UInt32s(0xE6BF2F32); // rev r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __rev<byte>(r2)");
        }

        [Test]
        public void ArmRw_rev16()
        {
            Given_HexString("B00FBFE6");
            AssertCode(     // rev16	r0,r0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __rev<word16>(r0)");
        }

        [Test]
        public void ArmRw_revsh()
        {
            Given_HexString("B9 86 FA 86");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULE,ZC)) branch 00100004",
                "2|L--|r8 = CONVERT(__rev<word16>(SLICE(r9, word16, 0)), word16, int32)");
        }

        [Test]
        public void ArmRw_shasx()
        {
            Given_HexString("36323036");
            AssertCode(     // shasxlo	r3,r0,r6
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|v5 = SLICE(r0, int16, 0) - SLICE(r6, int16, 16)",
                "3|L--|v6 = SLICE(r0, int16, 16) - SLICE(r6, int16, 0)",
                "4|L--|r3 = SEQ(v6, v5)");
        }

        [Test]
        public void ArmRw_stl()
        {
            Given_HexString("91FC80E1");
            AssertCode(     // stl	r1,[r0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_release<word32>(r0, r1)");
        }

        [Test]
        public void ArmRw_stlex()
        {
            Given_HexString("948A8301");
            AssertCode(     // stlexeq	r8,r4,[r3]
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r8 = __store_release_exclusive<word32>(r3, r4)");
        }

        [Test]
        public void ArmRw_stlexd()
        {
            Given_HexString("961EA0E1");
            AssertCode(     // stlexd	r1,r6,r7,[r0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __store_release_exclusive<word64>(r0, r6_r7)");
        }

        [Test]
        public void ArmRw_stlexh()
        {
            Given_HexString("94AAEB01");
            AssertCode(     // stlexheq	r10,r4,[fp]
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|v4 = SLICE(r4, word16, 0)",
                "3|L--|r10 = __store_release_exclusive<word16>(fp, v4)");
        }

        [Test]
        public void ArmRw_stlh()
        {
            Given_HexString("9460EA01");
            AssertCode(     // stlheq	r4,[r10]
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|v4 = SLICE(r4, word16, 0)",
                "3|L--|__store_release<word16>(r10, v4)");
        }

        [Test]
        public void ArmRw_strexd()
        {
            Given_HexString("9097A9E1");
            AssertCode(     // strexdeq	r9,r0,r1,[r9]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = __store_exclusive<word64>(r9, r0_r1)");
        }

        [Test]
        public void ArmRw_strexh()
        {
            Given_HexString("98D3EA01");
            AssertCode(     // strexheq	sp,r8,[r10]
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|v4 = SLICE(r8, word16, 0)",
                "3|L--|sp = __store_exclusive<word16>(r10, v4)");
        }

        [Test]
        public void ArmRw_strh()
        {
            Given_UInt32s(0xE1C320B0);        // strh\tr2,[r3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r3:word16] = SLICE(r2, uint16, 0)");
        }

        [Test]
        public void ArmRw_strh_pre_imm()
        {
            Given_UInt32s(0xE16230B2);        // strh\tr3,[r2,-#&2]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 - 2<i32>",
                "2|L--|Mem0[r2:word16] = SLICE(r3, uint16, 0)");
        }

        [Test]
        public void ArmRw_stm_user()
        {
            Given_UInt32s(0xE9435246);        // stm r3,{r1-r2,r6,r9,ip,lr}^
            AssertCode("" +
                "0|L--|00100000(4): 6 instructions",
                "1|L--|Mem0[r3:word32] = r1",
                "2|L--|Mem0[r3 + 4<i32>:word32] = r2",
                "3|L--|Mem0[r3 + 8<i32>:word32] = r6",
                "4|L--|Mem0[r3 + 12<i32>:word32] = r9",
                "5|L--|Mem0[r3 + 16<i32>:word32] = ip",
                "6|L--|Mem0[r3 + 20<i32>:word32] = lr");
        }

        [Test]
        public void ArmRw_switch()
        {
            Given_UInt32s(0xE796F104);    // ldr pc,[r6,r4, lsl #2]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[r6 + (r4 << 2<i32>):word32]");
        }

        [Test]
        public void ArmRw_ldm_user()
        {
            Given_UInt32s(0xE958414D);        // ldm\tr8,{r0,r2-r3,r6,r8,lr}^
            AssertCode(
                "0|L--|00100000(4): 6 instructions",
                "1|L--|r0 = Mem0[r8:word32]",
                "2|L--|r2 = Mem0[r8 + 4<i32>:word32]",
                "3|L--|r3 = Mem0[r8 + 8<i32>:word32]",
                "4|L--|r6 = Mem0[r8 + 12<i32>:word32]",
                "5|L--|r8 = Mem0[r8 + 16<i32>:word32]",
                "6|L--|lr = Mem0[r8 + 20<i32>:word32]");
        }

        [Test]
        public void ArmRw_usax()
        {
            Given_UInt32s(0xE6535054);        // usax\tr5,r3,r4
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = SLICE(r3, ui16, 0) + SLICE(r4, ui16, 16)",
                "2|L--|v3 = SLICE(r3, ui16, 16) - SLICE(r4, ui16, 0)",
                "3|L--|r5 = SEQ(v3, v2)");
        }

        [Test]
        public void ArmRw_eret()
        {
            Given_UInt32s(0xE167B760);        // eret
            AssertCode(
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void ArmRw_smc()
        {
            Given_UInt32s(0xE167C970);        // smc\t#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__smc(0<32>)");
        }

        [Test]
        public void ArmRw_smlsldx()
        {
            Given_UInt32s(0xE7434774);        // smlsldx\tr3,r4,r7,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_r3 = r4_r3 + (CONVERT(r7, word32, int16) *s16 (r4 >> 16<i32>) - (r7 >> 16<i32>) *s32 CONVERT(r4, word32, int16))");
        }

        [Test]
        public void ArmRw_ldrsh_imm_pre()
        {
            Given_UInt32s(0xE167C9F0);        // ldrsh\tip,[r7,-#&90]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = r7 - 144<i32>",
                "2|L--|ip = CONVERT(Mem0[r7:int16], int16, word32)");
        }






        [Test]
        public void ArmRw_ldrh_imm()
        {
            Given_UInt32s(0xE0DA85B8);        // ldrheq\tr8,[r10],#&58
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = CONVERT(Mem0[r10:word16], uint16, word32)",
                "2|L--|r10 = r10 + 88<i32>",
                "3|L--|r8 = v4");
        }

        [Test]
        public void ArmRw_ldrsh_imm()
        {
            Given_UInt32s(0xE0DAC7F0);        // ldrsh\tip,[r10],#&70
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = CONVERT(Mem0[r10:int16], int16, word32)",
                "2|L--|r10 = r10 + 112<i32>",
                "3|L--|ip = v4");
        }





        [Test]
        public void ArmRw_msr_banked_register()
        {
            Given_UInt32s(0xE167B70C);        // msr\tr11_usr,ip
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__msr(r11_usr, ip)");
        }

        [Test]
        public void ArmRw_pld()
        {
            Given_UInt32s(0xF5D0F020);
            AssertCode(// pld\t[r0,#&20]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__pld(r0 + 32<i32>)");
        }

        [Test]
        public void ArmRw_pldw()
        {
            Given_UInt32s(0xF59AF393);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__pldw(r10 + 915<i32>)");
        }

        [Test]
        public void ArmRw_uqasx()
        {
            Given_UInt32s(0xE6664F3A);        // uqasx\tr4,r6,r10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __uqasx_u16(r6, r10)");
        }

        [Test]
        public void ArmRw_uqsub16()
        {
            Given_UInt32s(0xE6694478);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __uqsub_u16(r9, r8)");
        }

        [Test]
        public void ArmRw_sasx()
        {
            Given_UInt32s(0xE615A034);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __sasx(r5, r4)");
        }

        [Test]
        public void ArmRw_vrecps()
        {
            Given_HexString("F48F46F2");
            AssertCode(     // vrecps.f32	q12,q11,q10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q12 = __vrecps_f32(q11)");
        }
        
        [Test]
        public void ArmRw_qasx()
        {
            Given_UInt32s(0xE6294630);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __qasx(r9, r0)");
        }

        [Test]
        public void ArmRw_qsub16()
        {
            Given_UInt32s(0xE6206174);        // qsub16\tr6,r0,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __qsub_s16(r0, r4)");
        }

        [Test]
        public void ArmRw_uqsax()
        {
            Given_UInt32s(0xE668985B);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = __uqsax_u16(r8, fp)");
        }






        [Test]
        public void ArmRw_uhsax()
        {
            Given_HexString("523A7D36");
            AssertCode(     // uhsaxlo	r3,sp,r2
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|v5 = SLICE(sp, uint16, 0) - SLICE(r2, uint16, 16)",
                "3|L--|v6 = SLICE(sp, uint16, 16) - SLICE(r2, uint16, 0)",
                "4|L--|r3 = SEQ(v6, v5)");
        }

        [Test]
        public void ArmRw_uqsub8()
        {
            Given_UInt32s(0xE6689EFD);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = __uqsub_u8(r8, sp)");
        }

        [Test]
        public void ArmRw_uqadd8()
        {
            Given_UInt32s(0xE6688E91);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __uqadd_u8(r8, r1)");
        }














        [Test]
        public void ArmRw_vaddl()
        {
            Given_UInt32s(0xF3AF8000);    // vaddl.u32\tq4,d15,d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q4 = __vaddl_u32(d15, d0)");
        }

        [Test]
        public void ArmRw_vbic()
        {
            Given_UInt32s(0xF2C72B3F); // vbic.i16\td18,0x7F00<16>
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d18 = __vbic_i16(d18, 0x7F007F007F007F00<64>)");
        }

        [Test]
        public void ArmRw_vbsl()
        {
            Given_HexString("F8C15AF3");
            AssertCode(     // vbsl	q14,q13,q12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q14 = __vbsl<word128>(q13, q12)");
        }

        [Test]
        public void ArmRw_vcvt_f64_f32()
        {
            Given_UInt32s(0xEEF70AC7);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = CONVERT(s14, real32, real64)");
        }

        [Test]
        public void ArmRw_vcvt_f32_u32()
        {
            Given_HexString("400AB8EE");
            AssertCode(     // vcvt.f32.u32	s0,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = CONVERT(s0, uint32, real32)");
        }

        [Test]
        public void ArmRw_vcvt_s32_f32()
        {
            Given_HexString("6DCABD3E");
            AssertCode(     // vcvtlo.s32.f32	s24,s27
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|s24 = CONVERT(s27, real32, int32)");
        }

        [Test]
        public void ArmRw_vcvtr()
        {
            Given_UInt32s(0xEEFD9AE9);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s19 = CONVERT(trunc(s19), real32, int32)");
        }

        [Test]
        public void ArmRw_vcvtr_u32_f64()
        {
            Given_UInt32s(0xEEFC6BC7);  // vcvtr.u32.f64	s13,d7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s13 = CONVERT(trunc(d7), real64, uint32)");
        }

        [Test]
        public void ArmRw_vcvtr_s32_f64()
        {
            Given_HexString("C00BBDEE");
            AssertCode(     // vcvtr.s32.f64	s0,d0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = CONVERT(trunc(d0), real64, int32)");
        }

        [Test]
        public void ArmRw_vcvtr_u32_f32()
        {
            Given_HexString("C52ABCEE");
            AssertCode(     // vcvtr.u32.f32	s4,s10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s4 = CONVERT(trunc(s10), real32, uint32)");
        }

        [Test]
        public void ArmRw_vcvt_scalar()
        {
            Given_UInt32s(0xEEB86B45);  // vcvt.f64.u32	d6,s10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d6 = CONVERT(s10, uint32, real64)");
        }

        [Test]
        public void ArmRw_vext_64()
        {
            Given_UInt32s(0xF2F068E2);	// vext.64 q11, q8, q9, #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q11 = __vext(q8, q9, 8<32>)");
        }

        [Test]
        public void ArmRw_vfma()
        {
            Given_HexString("ABAAAA3E");
            AssertCode(     // vfmalo.f32	s20,s21,s23
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|s20 = s20 + s21 * s23");
        }

        [Test]
        public void ArmRw_vmla()
        {
            Given_UInt32s(0xF2DEF14C);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d31 = __vmla_f16(d31, d14)");
        }

        [Test]
        public void ArmRw_vmlsl_scalar()
        {
            Given_UInt32s(0xF2EF4665);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmlsl_s32(d15, d5[1<i32>])");
        }

        [Test]
        public void ArmRw_vrhadd()
        {
            Given_UInt32s(0xF30DF1A5);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vrhadd_u8(d29, d21)");
        }

        [Test]
        public void ArmRw_vrsra()
        {
            Given_UInt32s(0xF3B2F393);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vrsra_i64(d3, 14<i32>)");
        }

        [Test]
        public void ArmRw_vrsubhn()
        {
            Given_UInt32s(0xF3934620);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vrsubhn_i32(q1, q8)");
        }

        [Test]
        public void ArmRw_vstmia()
        {
            Given_UInt32s(0xECE30B04);  // vstmia r3, d16, d17
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|Mem0[r3:word64] = d16",
               "2|L--|Mem0[r3 + 8<i32>:word64] = d17",
               "3|L--|r3 = r3 + 16<i32>");
        }

        [Test]
        public void ArmRw_vtbl()
        {
            Given_HexString("A209F0F3");
            AssertCode(     // vtbl.i8	d16,{d16-d17},d18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vtbl<word128>(d16_d17, d18)");
        }

#if NYI
        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

                [Test]
        [Ignore("Read up on the specs")]
        public void ArmRw_vld4()
        {
            Given_UInt32s(0xF468F191);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("Read up on the specs")]
        public void ArmRw_vst3()
        {
            Given_UInt32s(0xF44F249F);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vst3_thumb()
        {
            Given_HexString("4AF96B35");
            AssertCode(     // vst3.i16	{d19,d21,d23},[r10:128],fp
                "0|L--|001A218E(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void ArmRw_vtbl()
        {
            Given_UInt32s(0xF3F36800);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void ArmRw_vtbl_2()
        {
            Given_HexString("2029F2F3");
            AssertCode(     // vtbl.i8	d18,{d2-d3},d16
                "0|L--|008E222C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void ArmRw_vzip()
        {
            Given_HexString("E401FAF3");
            AssertCode(     // vzip.i32	q8,q10
                "0|L--|000377F8(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_Nyi()
        {
            Given_HexString("8CCCBDBE");
            AssertCode(     // Nyilt
                "0|L--|001811C4(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_mcr2()
        {
            Given_HexString("7B672EFE");
            AssertCode(     // mcr2	p7,#1,r6,cr14,cr11,#3
                "0|L--|001A186C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uxtab16()
        {
            Given_HexString("745BC456");
            AssertCode(     // uxtab16pl	r5,r4,r4,ror #&10
                "0|L--|001637F0(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ArmRw_uhasx()
        {
            Given_HexString("36CA7896");
            AssertCode(     // uhasxls	ip,r8,r6
                "0|L--|001C3268(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uhadd16()
        {
            Given_HexString("1EB073D6");
            AssertCode(     // uhadd16le	fp,r3,lr
                "0|L--|001A4FC0(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_mrc2()
        {
            Given_HexString("5B15FFFE");
            AssertCode(     // mrc2	p5,#7,r1,cr15,cr11,#2
                "0|L--|00184234(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vmsr()
        {
            Given_HexString("7470EFCE");
            AssertCode(     // vmsrgt	#&F,r7
                "0|L--|001653F8(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vfnma()
        {
            Given_HexString("618A9B8E");
            AssertCode(     // vfnmahi.f32	s16,s22,s3
                "0|L--|0013468C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vsubhn()
        {
            Given_HexString("AF66A5F2");
            AssertCode(     // vsubhn.i64	d6,q10,q15
                "0|L--|001ED5B8(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ArmRw_sel()
        {
            Given_HexString("B3CF8706");
            AssertCode(     // seleq
                "0|L--|001CA1B8(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uhsub16()
        {
            Given_HexString("73B27066");
            AssertCode(     // uhsub16vs	fp,r0,r3
                "0|L--|001BE974(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_shadd16()
        {
            Given_HexString("1A4A3C96");
            AssertCode(     // shadd16ls	r4,ip,r10
                "0|L--|00175504(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uhadd8()
        {
            Given_HexString("932D7446");
            AssertCode(     // uhadd8mi	r2,r4,r3
                "0|L--|00175554(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_shsax()
        {
            Given_HexString("58253B36");
            AssertCode(     // shsaxlo	r2,fp,r8
                "0|L--|0019A94C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vcvtt()
        {
            Given_HexString("C322B22E");
            AssertCode(     // vcvtths.f32.f16	s4,s6
                "0|L--|001D72AC(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_smmlsr()
        {
            Given_HexString("F6E35E77");
            AssertCode(     // smmlsrvc	lr,r6,r3,lr
                "0|L--|001585F4(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_shadd8()
        {
            Given_HexString("97BF3476");
            AssertCode(     // shadd8vc	fp,r4,r7
                "0|L--|0017C908(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vfnms()
        {
            Given_HexString("0A0A98BE");
            AssertCode(     // vfnmslt.f32	s0,s16,s20
                "0|L--|0014085C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_qsub8()
        {
            Given_HexString("FDC52E36");
            AssertCode(     // qsub8lo	ip,lr,sp
                "0|L--|001A0DD0(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vacge()
        {
            Given_HexString("12CE42F3");
            AssertCode(     // vacge.f32	d28,d2,d2
                "0|L--|001A3AB0(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uhsub8()
        {
            Given_HexString("F8E17236");
            AssertCode(     // uhsub8lo	lr,r2,r8
                "0|L--|00123608(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vst2()
        {
            Given_HexString("8AB30DF4");
            AssertCode(     // vst2.i32	{d11,d12},[sp],r10
                "0|L--|0012384C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_ldrexh()
        {
            Given_HexString("965EF261");
            AssertCode(     // ldrexhvs	r5,[r2]
                "0|L--|00141C54(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vld1()
        {
            Given_HexString("AC3A6CF4");
            AssertCode(     // vld1.i32	{d19,d20},[ip:128],ip
                "0|L--|00145D18(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_smmlar()
        {
            Given_HexString("3A375D07");
            AssertCode(     // smmlareq	sp,r10,r7,r3
                "0|L--|001853FC(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vqshlu()
        {
            Given_HexString("F8C79CF3");
            AssertCode(     // vqshlu.i64	q6,q12,#&1C
                "0|L--|0015E100(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_rbit()
        {
            Given_HexString("32EDFB56");
            AssertCode(     // rbitpl	lr,r2
                "0|L--|00143D48(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_ssax()
        {
            Given_HexString("538A1CB6");
            AssertCode(     // ssaxlt	r8,ip,r3
                "0|L--|00148510(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_sxtab16()
        {
            Given_HexString("7E008526");
            AssertCode(     // sxtab16hs	r0,r5,lr
                "0|L--|00148548(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void ArmRw_vrsqrts()
        {
            Given_HexString("157F71F2");
            AssertCode(     // vrsqrts.f16	d23,d1,d5
                "0|L--|0010E008(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_shasx()
        {
            Given_HexString("36423616");
            AssertCode(     // shasxne	r4,r6,r6
                "0|L--|001C3A14(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vabal()
        {
            Given_HexString("85458EF3");
            AssertCode(     // vabal.u8	q2,d30,d5
                "0|L--|00182180(4): 1 instructions",
                "1|L--|@@@");
        }




        [Test]
        public void ArmRw_vqsub()
        {
            Given_HexString("11A211F3");
            AssertCode(     // vqsub.u16	d10,d1,d1
                "0|L--|001955E0(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vldmdb()
        {
            Given_HexString("101331BD");
            AssertCode(     // vldmdblt	r1!,{d1-d8}
                "0|L--|001C5884(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vst1()
        {
            Given_HexString("776206F4");
            AssertCode(     // vst1.i16	{d6-d9},[r6:256],r7
                "0|L--|0011013C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_setpan()
        {
            Given_HexString("15B6");
            AssertCode(     // setpan	#0
                "0|L--|00124418(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_rfeia()
        {
            Given_HexString("B6E97784");
            AssertCode(     // rfeia	r6
                "0|L--|001643DC(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_srsdb()
        {
            Given_HexString("26E805E9");
            AssertCode(     // srsdb	sp,#5
                "0|L--|0018232C(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ArmRw_vqrshl()
        {
            Given_HexString("54FF9E45");
            AssertCode(     // vqrshl.u16	d20,d20,d14
                "0|L--|0012220A(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_srsia()
        {
            Given_HexString("89E97F8D");
            AssertCode(     // srsia	sp,#&1F
                "0|L--|001A223C(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vtbx()
        {
            Given_HexString("F3FF4129");
            AssertCode(     // vtbx.i8	d18,{d3-d4},d1
                "0|L--|001C008A(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_rfedb()
        {
            Given_HexString("3EE8F3FF");
            AssertCode(     // rfedb	lr
                "0|L--|001405FE(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vmaxnm()
        {
            Given_HexString("CEFEAA69");
            AssertCode(     // vmaxnm.f16	s13,s29,s21
                "0|L--|00120D5E(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vld2()
        {
            Given_HexString("65F96598");
            AssertCode(     // vld2.i16	{d25-d26},[r5:128],r5
                "0|L--|0012B1F6(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vld3()
        {
            Given_HexString("A0F91866");
            AssertCode(     // vld3.i16	{d6[0],d7[0],d8[0]},[r0],r8
                "0|L--|0010329E(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ArmRw_vqshrn()
        {
            Given_HexString("9FEF3829");
            AssertCode(     // vqshrn.u32	d2,q12,#1
                "0|L--|0013C382(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_sev()
        {
            Given_HexString("40BF");
            AssertCode(     // sev
                "0|L--|001E83B6(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vabdl()
        {
            Given_HexString("C3EFA967");
            AssertCode(     // vabdl.s8	q11,d19,d25
                "0|L--|0013C6C4(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_uxtb16()
        {
            Given_HexString("3FFAECE3");
            AssertCode(     // uxtb16	r3,ip,ror #&10
                "0|L--|001C8468(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vminnm()
        {
            Given_HexString("8DFE69CA");
            AssertCode(     // vminnm.f32	s24,s26,s19
                "0|L--|001EDF12(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ArmRw_vst4()
        {
            Given_HexString("48F98180");
            AssertCode(     // vst4.i32	{d24-d27},[r8],r1
                "0|L--|001CD3AC(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_sxtb16()
        {
            Given_HexString("2FFAA3DD");
            AssertCode(     // sxtb16	sp,r3,ror #&10
                "0|L--|0018B79E(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vraddhn()
        {
            Given_HexString("EEFFA194");
            AssertCode(     // vraddhn.i64	d25,q15,q8
                "0|L--|00144306(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vaddhn()
        {
            Given_HexString("9AEF2AF4");
            AssertCode(     // vaddhn.i32	d15,q5,q13
                "0|L--|001F541A(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_strexd()
        {
            Given_HexString("CFE8764A");
            AssertCode(     // strexd	r6,r4,r10,[pc]
                "0|L--|001D23F0(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_wfe()
        {
            Given_HexString("20BF");
            AssertCode(     // wfe
                "0|L--|001D7A54(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vrintm()
        {
            Given_HexString("FBFEC80B");
            AssertCode(     // vrintm.f64	d16,d8
                "0|L--|0016CFEA(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ArmRw_vcvtm()
        {
            Given_HexString("BFFEC4DB");
            AssertCode(     // vcvtm.s32.f64	d13,d4
                "0|L--|00173444(4): 1 instructions",
                "1|L--|@@@");
        }
#endif
    }
}