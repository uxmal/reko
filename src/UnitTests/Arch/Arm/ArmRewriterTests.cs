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

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class ArmRewriterTests : RewriterTestBase
    {
        private Arm32ProcessorArchitecture arch = new Arm32ProcessorArchitecture("arm32");
        private MemoryArea image;
        private Address baseAddress = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddress; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return new ArmRewriter(arch, new LeImageReader(image, 0), new ArmProcessorState(arch), binder, host);
        }

        private void BuildTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            image = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }

        private void BuildTest(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            image = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }

        [Test]
        public void ArmRw_mov_r1_r2()
        {
            BuildTest("1110 00 0 1101 0 0000 0001 00000000 0010");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2");
        }

        [Test]
        public void ArmRw_add_r1_r2_r3()
        {
            BuildTest("1110 00 0 0100 0 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 + r3");
        }

        [Test]
        public void ArmRw_adds_r1_r2_r3()
        {
            BuildTest("1110 00 0 0100 1 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r2 + r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_subgt_r1_r2_imm4()
        {
            BuildTest("1100 00 1 0010 0 0010 0001 0000 00000100");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,NZV)) branch 00100004",
                "2|L--|r1 = r2 - 0x00000004");
        }

        [Test]
        public void ArmRw_orr_r3_r4_r5_lsl_5()
        {
            BuildTest("1110 00 0 1100 0 1100 0001 00100 000 0100");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = ip | r4 << 0x04");
        }

        [Test]
        public void ArmRw_brgt()
        {
            BuildTest("1100 1010 000000000000000000000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GT,NZV)) branch 00100008");
        }

        [Test]
        public void ArmRw_lsl()
        {
            BuildTest(0xE1a00200);  // mov\tr0,r0,lsl #4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 << 0x04");
        }

        [Test]
        public void ArmRw_stmdb()
        {
            BuildTest(0xE92C003B);  // stmdb ip!,{r0,r1,r3-r5},lr,pc}
            AssertCode(
                "0|L--|00100000(4): 6 instructions",
                "1|L--|Mem0[ip:word32] = r0",
                "2|L--|Mem0[ip - 0x00000004:word32] = r1",
                "3|L--|Mem0[ip - 0x00000008:word32] = r3",
                "4|L--|Mem0[ip - 0x0000000C:word32] = r4",
                "5|L--|Mem0[ip - 0x00000010:word32] = r5",
                "6|L--|ip = ip - 0x00000014");
        }

        [Test]
        public void ArmRw_bllt()
        {
            BuildTest(0xBB000330);  // bllt
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,NV)) branch 00100004",
                "2|T--|call 00100CC8 (0)");
        }

        [Test]
        public void ArmRw_ldr()
        {
            BuildTest(0xE5940008);  // ldr r0,[r4,#8]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r4 + 8:word32]");
        }

        [Test]
        public void ArmRw_bne()
        {
            BuildTest(0x1A000004);  // bne
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100018");
        }

        [Test]
        public void ArmRw_bic()
        {
            BuildTest(0xE3CEB3FF);  // bic
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = lr & ~0xFC000003");
        }

        [Test]
        public void ArmRw_mov_pc_lr()
        {
            BuildTest(0xE1B0F00E);  // mov pc,lr
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void ArmRw_ldrsb()
        {
            BuildTest(0xE1F120D1);  // ldrsb r2,[r1,#1]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + 1",
                "2|L--|r2 = Mem0[r1:int8]");
        }

        [Test]
        public void ArmRw_mov_pc()
        {
            BuildTest(0xE59F0010);  // ldr\tr0,[pc,#&10]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[0x00100018:word32]");
        }

        [Test]
        public void ArmRw_cmp()
        {
            BuildTest(0xE3530000);  // cmp r3,#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 - 0x00000000)");
        }

        [Test]
        public void ArmRw_cmn()
        {
            BuildTest(0xE3730001); /// cmn r3,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 + 0x00000001)");
        }

        [Test]
        public void ArmRw_ldr_pc()
        {
            BuildTest(0xE59CF000); // ldr pc,[ip]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[ip:word32]");
        }

        [Test]
        public void ArmRw_ldr_post()
        {
            BuildTest(0xE4D43001);// ldrb r3,[r4],#1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = Mem0[r4:byte]",
                "2|L--|r4 = r4 + 0x00000001");
        }

        [Test]
        public void ArmRw_push()
        {
            BuildTest(0xE92D4010);
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|Mem0[sp:word32] = r4",
               "2|L--|Mem0[sp - 0x00000004:word32] = lr",
               "3|L--|sp = sp - 0x00000008");
        }

        [Test]
        public void ArmRw_movw()
        {
            BuildTest(0xE30F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = 0x0000FFFF");
        }

        [Test]
        public void ArmRw_uxtb()
        {
            BuildTest(0xE6EF2071);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (byte) r1");
            BuildTest(0xE6EF2471);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (byte) (r1 >>u 0x08)");
        }

        [Test]
        public void ArmRw_bxuge()
        {
            BuildTest(0x212FFF1E);
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULT,C)) branch 00100004",
                "2|T--|goto lr");
        }

        [Test]
        public void ArmRw_movt()
        {
            BuildTest(0xE34F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = DPB(r4, 0xFFFF, 16)");
        }

        [Test]
        public void ArmRw_pop()
        {
            BuildTest(0xE8BD000C);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r2 = Mem0[sp:word32]",
                "2|L--|r3 = Mem0[sp + 4:word32]",
                "3|L--|sp = sp + 8");
        }

        [Test]
        public void ArmRw_popne()
        {
            BuildTest(0x18BD000C);
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r2 = Mem0[sp:word32]",
                "3|L--|r3 = Mem0[sp + 4:word32]",
                "4|L--|sp = sp + 8");
        }

        [Test]
        public void ArmRw_clz()
        {
            BuildTest(0xE16F4F13);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __clz(r3)");
        }

        [Test]
        public void ArmRw_strd()
        {
            BuildTest(0xE04343F8);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r3:word64] = r5_r4",
                "2|L--|r3 = r3 + 0xFFFFFFC8");
        }

        [Test]
        public void ArmRw_muls()
        {
            BuildTest(0xE0120A94);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r4 * r10",
                "2|L--|NZCV = cond(r2)");
        }

        [Test]
        public void ArmRw_mlas()
        {
            BuildTest(0xE0314392);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r4 + r2 * r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_bfi()
        {
            BuildTest(0xE7CD1292);
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|v4 = SLICE(r2, ui9, 0)",
               "2|L--|r1 = DPB(r1, v4, 5)");
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
            BuildTest(0xE1C722D8);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3_r2 = Mem0[r7 + 40:word64]");
        }

        [Test]
        public void ArmRw_ubfx()
        {
            BuildTest(0xE7F01252);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (uint32) SLICE(r2, ui17, 4)");
        }

        [Test]
        public void ArmRw_sxtb()
        {
            BuildTest(0xE6AF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (int8) (r2 >>u 0x08)");
        }

        [Test]
        public void ArmRw_uxth()
        {
            BuildTest(0xE6FF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (uint16) (r2 >>u 0x08)");
        }

        [Test]
        public void ArmRw_umull()
        {
            BuildTest(0xE0912394);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1_r2 = r3 *u r4",
                "2|L--|NZCV = cond(r1_r2)");
        }

        [Test]
        public void ArmRw_mls()
        {
            BuildTest(0xE0612394);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 - r4 * r3");
        }

        [Test]
        public void ArmRw_vldmia()
        {
            //04 0B F2 EC
            //04 0B E3 EC
            BuildTest(0xECF20B04);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|d16 = Mem0[r2:word64]",
                "2|L--|d17 = Mem0[r2 + 8:word64]",
                "3|L--|r2 = r2 + 16");
        }

        [Test]
        public void ArmRw_ldmib()
        {
            BuildTest(0xE9950480); // ldmibr5, r7, r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = Mem0[r5 + 4:word32]",
                "2|L--|r10 = Mem0[r5 + 8:word32]");
        }

        [Test]
        public void ArmRw_rev()
        {
            BuildTest(0xE6BF2F32); // rev r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __rev(r2)");
        }

        [Test]
        public void ArmRw_vmov_i32()
        {
            //  51 00 C0 F2 vmov.i32 q8,#0
            BuildTest(0xF2C00051); // rev r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = __vmov_i32(0x00000001)");
        }

        [Test]
        public void ArmRw_adc()
        {
            BuildTest(0xE0A22002); // adc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|r2 = r2 + r2 + C");
        }

        [Test]
        public void ArmRw_sbc()
        {
            BuildTest(0xE0C22002); // sbc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|r2 = r2 - r2 - C");
        }

        [Test]
        public void ArmRw_vstmia()
        {
            BuildTest(0xECE30B04);  // vstmia r3, d16, d17
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|Mem0[r3:word64] = d16",
               "2|L--|Mem0[r3 + 8:word64] = d17",
               "3|L--|r3 = r3 + 16");
        }

        [Test]
        public void ArmRw_mrs()
        {
            BuildTest(0xE10F3000); // mrs r3, cpsr
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r3 = __mrs(cpsr)");
        }

        [Test]
        public void ArmRw_cpsid()
        {
            BuildTest(0xF10C0080); // cpsid
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__cps_id()");
        }

        [Test]
        public void ArmRw_smulbb()
        {
            BuildTest(0xE1600380); //  smulbb r0, r0, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (int16) r0 *s (int16) r3");
        }

        [Test]
        public void ArmRw_bfc()
        {
            BuildTest(0xE7C5901F);  // bfc r9, #0, #6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r9 & 0xFFFFFFC0");
        }

        [Test]
        public void ArmRw_sbfx()
        {
            BuildTest(0xE7A9C35C); // sbfx ip,ip,#6,#&A
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = (int32) SLICE(ip, ui10, 6)");
        }


        [Test]
        public void ArmRw_umlalne()
        {
            BuildTest(0x10A54A93); // umlalne r4,r5,r3,r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r5_r4 = r3 *u r10 + r5_r4");
        }

        [Test]
        public void ArmRw_msr()
        {
            BuildTest(0xE121F001); // msr cpsr, r1
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|__msr(cpsr, r1)");
        }

        [Test]
        public void ArmRw_uxtab()
        {
            BuildTest(0xE6E10070);  // uxtab r0, r1, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r1 + (byte) r0");
        }

        [Test]
        public void ArmRw_sxtab()
        {
            BuildTest(0xE6A55078);  // sxtab r5, r5, r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 + (int8) r8");
        }

        [Test]
        public void ArmRw_sxtah()
        {
            BuildTest(0xE6B6A07A);  // sxtah r10,r6,r10
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|r10 = r6 + (int16) r10");
        }

        [Test]
        public void ArmRw_sxthne()
        {
            BuildTest(0x16BF9077);  //  sxthne r9,r7
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004");
        }

        [Test]
        public void ArmRw_uxtah()
        {
            BuildTest(0xE6F30072);  // uxtah r0,r3,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r3 + (uint16) r2");
        }

        [Test]
        public void ArmRw_dmb()
        {
            BuildTest(0xF57FF05F);  // dmb
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|__dmb_sy()");
        }

        [Test]
        public void ArmRw_mrc()
        {
            BuildTest(0xEE123F10);  // mrc p15,#0,r3,c2
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|r3 = __mrc(0x0F, 0x00000000, 0x02, 0x00, 0x00000000)");
        }

        [Test]
        public void ArmRw_mcr()
        {
            BuildTest(0xEE070F58);  // mcr p15,#0,r0,c7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mcr(0x0F, 0x00000000, r0, 0x07, 0x08, 0x00000002)");
        }

        [Test]
        public void ArmRw_bl()
        {
            BuildTest(0xEB00166B);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 001059B4 (0)");
        }

        [Test]
        public void ArmRw_ldrls_pc()
        {
            BuildTest(0x979FF103);   // ldrls\tpc,[pc,r3,lsl #2]
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGT,ZC)) branch 00100004",
                "2|T--|goto Mem0[0x00100008 + r3 * 0x00000004:word32]");
        }

        [Test]
        public void ArmRw_svc()
        {
            BuildTest(0xEF001234); // svc 0x1234
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__syscall(0x00001234)");
        }
    }
}