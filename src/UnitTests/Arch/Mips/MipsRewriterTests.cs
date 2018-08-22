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

using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsRewriterTests : RewriterTestBase
    {
        private MipsProcessorArchitecture arch = new MipsBe32Architecture("mips-be-32");
        private BeImageReader rdr;
        private MipsDisassembler dasm;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture("mips-be-32");
        }

        private void RunTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte)(u >> 24), (byte)(u >> 16), (byte)(u >> 8), (byte)u })
                .ToArray();
            dasm = new MipsDisassembler(
                arch,
                new BeImageReader(new MemoryArea(Address.Ptr32(0x00100000), bytes), 0),
                false);
        }

        private void AssertCode(uint instr, params string[] sExp)
        {
            Rewrite(instr);
            AssertCode(sExp);
        }

        private void Given_Mips64_Architecture()
        {
            arch = new MipsBe64Architecture("mips-be-64");
        }

        protected override MemoryArea RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) (w >> 24),
                (byte) (w >> 16),
                (byte) (w >> 8),
                (byte) w
            }).ToArray();
            var image = new MemoryArea(LoadAddress, bytes);
            this.rdr = image.CreateBeReader(LoadAddress);
            dasm = new MipsDisassembler(arch, rdr, false);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return new MipsRewriter(arch, rdr, dasm, binder, host);
        }

        [Test]
        public void MipsRw_lh()
        {
            RunTest("100001 01001 00011 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = (word32) Mem0[r9 - 0x00000038:int16]");
        }

        [Test]
        public void MipsRw_lhu()
        {
            RunTest("100101 01011 01101 1111111111111000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (word32) Mem0[r11 - 0x00000008:uint16]");
        }

        [Test]
        public void MipsRw_lui()
        {
            RunTest("001111 00000 00011 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = 0xFFC80000");
        }
        [Test]
        public void MipsRw_ll()
        {
            RunTest("110000 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __load_linked_32(Mem0[r10 - 0x00000038:word32])");
        }
        [Test]
        public void MipsRw_lld()
        {
            Given_Mips64_Architecture();
            RunTest("110100 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __load_linked_64(Mem0[r10 - 0x0000000000000038:word64])");
        }

        [Test]
        public void MipsRw_sc()
        {
            RunTest("111000 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __store_conditional_32(Mem0[r10 - 0x00000038:word32], r21)");
        }
        [Test]
        public void MipsRw_scd()
        {
            Given_Mips64_Architecture();
            RunTest("111100 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __store_conditional_64(Mem0[r10 - 0x0000000000000038:word64], r21)");
        }

        [Test]
        public void MipsRw_ori_r0()
        {
            RunTest("001101 00000 00101 1111100000100111");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = 0x0000F827");
        }

        [Test]
        public void MipsRw_addi_r0()
        {
            RunTest("001000 00000 00010 1111111111111000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = -8");
        }

        [Test]
        public void MipsRw_add()
        {
            RunTest("000000 00001 00010 00011 00000 100000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r1 + r2");
        }

        [Test]
        public void MipsRw_andi_0()
        {
            RunTest("001100 00000 00101 0000000000000000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = 0x00000000");
        }

        [Test]
        public void MipsRw_bgtz()
        {
            RunTest("000111 00011 00000 1111111111111110");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|if (r3 > 0x00000000) branch 000FFFFC");
        }

        [Test]
        public void MipsRw_j()
        {
            RunTest("000010 11111111111111111111111111");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|goto 0FFFFFFC");
        }

        [Test]
        public void MipsRw_sw()
        {
            AssertCode(0xAFBF0020,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x00000020:word32] = ra");
        }

        [Test]
        public void MipsRw_sd_64()
        {
            Given_Mips64_Architecture();
            AssertCode(0xFFBF0020,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x0000000000000020:word64] = ra");
        }

        [Test]
        public void MipsRw_jal()
        {
            AssertCode(0x0C009B2C,
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|call 00026CB0 (0)");
        }

        [Test]
        public void MipsRw_srl()
        {
            AssertCode(0x00024c02,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r2 >>u 0x10");
        }

        [Test]
        public void MipsRw_nop()
        {
            AssertCode(0x00000000,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void MipsRw_lw()
        {
            AssertCode(0x8fb30010,   // lw s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[sp + 0x00000010:word32]");
            AssertCode(0x8fb3FFF0,   // lw s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[sp - 0x00000010:word32]");
        }

        [Test(Description = "On MIPS64, lw loads a signed integer")]
        public void MipsRw_lw_64bit()
        {
            Given_Mips64_Architecture();
            AssertCode(0x8fb3FFF0,   // lw s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = (word64) Mem0[sp - 0x0000000000000010:int32]");
        }

        [Test]
        public void MipsRw_ld()
        {
            Given_Mips64_Architecture();
            AssertCode(0xdfb30010,   // ld s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[sp + 0x0000000000000010:word64]");
        }

        [Test]
        public void MipsRw_lwu()
        {
            Given_Mips64_Architecture();
            AssertCode(0x9fb30010,   // ld s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = (word64) Mem0[sp + 0x0000000000000010:uint32]");
        }

        [Test]
        public void MipsRw_beq()
        {
            AssertCode(0x10300005,
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r1 == r16) branch 00100018");
        }

        [Test]
        public void MipsRw_Nor()
        {
            AssertCode(0x01004027, // nor t0,t0,zero	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = ~r8");
        }

        [Test]
        public void MipsRw_jr()
        {
            AssertCode(0x01000008,// jr t0	      
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|goto r8");
        }

        [Test]
        public void MipsRw_xor()
        {
            AssertCode(0x01285026, // xor t2,t1,t0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = r9 ^ r8");
        }

        [Test]
        public void MipsRw_jalr()
        {
            AssertCode(0x0120f809,  // jalr t1
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|call r9 (0)");
        }

        [Test]
        public void MipsRw_sltu()
        {
            AssertCode(0x0211402B,  // sltu t0,s0,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = (word32) (r16 <u r17)");
        }

        [Test]
        public void MipsRw_jr_returns()
        {
            AssertCode(0x03E00008, // jr ra
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void MipsRw_sb()
        {
            AssertCode(0xA128D958, // sb t0,-9896(t1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9 - 0x000026A8:byte] = (byte) r8");
        }

        [Test]
        public void MipsRw_tge()
        {
            AssertCode(0x00F000F0,  // tge a3,s0,0x3
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r7 < r16) branch 00100004",
                "2|L--|__trap(0x0003)");
        }

        [Test]
        public void MipsRw_br()
        {
            AssertCode(0x1000ffc2,  // b loc_00026e0
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto 000FFF0C");
        }

        [Test]
        public void MipsRw_slti()
        {
            AssertCode(0x29690070,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = (word32) (r11 < 112)");
        }

        [Test]
        public void MipsRw_lwl_lwr()
        {
            AssertCode(0x88c80003,  // lwl t0,3(a2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __lwl(r8, Mem0[r6 + 0x00000003:word32])");

            AssertCode(0x98c80000,   // lwr t0,0(a2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __lwr(r8, Mem0[r6:word32])");
        }

        [Test]
        public void MipsRw_mfhi()
        {
            AssertCode(0x00004010,  // mfhi t0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = hi");
        }

        [Test]
        public void MipsRw_mult()
        {
            AssertCode(0x02F00018, // mult s7,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = r23 *s r16");
        }

        [Test]
        public void MipsRw_mul()
        {
            AssertCode(0x70621002,  // mul r2, r3, r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r3 *s r2");
        }

        [Test]
        public void MipsRw_mtc1()
        {
            AssertCode(0x448C0800,  // mtc1 r12,f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = r12");
        }

        [Test]
        public void MipsRw_swc1()
        {
            AssertCode(0xE7AC0030, // swc1\tf12,0030(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x00000030:word32] = (word32) f12");
        }

        [Test]
        public void MipsRw_cle_d()
        {
            AssertCode(0x462C003E, // c.le.d\tf0,f12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f0_f1 <= f12_f13");
        }

        [Test]
        public void MipsRw_cfc1()
        {
            AssertCode(0x4443F800, // cfc1\tr3,FCSR
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = FCSR");
        }

        [Test]
        public void MipsRw_instrs1()
        {
            AssertCode(0x44C1F800, // "ctc1\tr1,FCSR"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|FCSR = r1");

            AssertCode(0x46206024, // "cvt.w.d\tf0,f12"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (int32) f12_f13");

            AssertCode(0x45000012, // "bc1f\tcc0,0010004C"
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (!cc0) branch 0010004C");

            AssertCode(0x46206000, // "add.d\tf0,f12,f0"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0_f1 = f12_f13 + f0_f1");
        }

        [Test]
        public void MipsRw_srav()
        {
            AssertCode(0x00C24c07,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r2 >> r6");
        }

        [Test]
        public void MipsRw_bltzal_r0()
        {
            AssertCode(0x0410FFFE,      // bltzal r0,000FFFFC
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ra = 00100008");
        }

        [Test]
        public void MipsRw_rdhwr()
        {
            // Test only the known ones, we'll have to see how this changes things later on with dynamic custom registers
            RunTest("011111 00000 00110 00000 00000 111011");   // CPU number
            AssertCode("0|L--|00100000(4): 1 instructions",
                        "1|L--|r6 = __read_hardware_register(0x00)");

            RunTest("011111 00000 01000 00001 00000 111011");   // SYNCI step size
            AssertCode("0|L--|00100000(4): 1 instructions",
                        "1|L--|r8 = __read_hardware_register(0x01)");

            RunTest("011111 00000 00001 00010 00000 111011");   // Cycle counter
            AssertCode("0|L--|00100000(4): 1 instructions",
                        "1|L--|r1 = __read_hardware_register(0x02)");

            RunTest("011111 00000 00011 00011 00000 111011");   // Cycle counter resolution
            AssertCode("0|L--|00100000(4): 1 instructions",
                        "1|L--|r3 = __read_hardware_register(0x03)");

            RunTest("011111 00000 00111 11101 00000 111011");   // OS-specific, thread local pointer on Linux
            AssertCode("0|L--|00100000(4): 1 instructions",
                        "1|L--|r7 = __read_hardware_register(0x1D)");
        }

        [Test]
        public void MipsRw_movz()
        {
            RunTest("000000 00011 01001 01010 00000 001010");    // movz
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r9 != 0x00000000) branch 00100004", 
                "2|L--|r10 = r3");
        }



        [Test]
        public void MipsRw_swl_swr()
        {
            AssertCode(0xABA8002B,                // swl r8, 002B(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x0000002B:word32] = __swl(Mem0[sp + 0x0000002B:word32], r8)");

            AssertCode(0xBBA80028,                // swr r8, 0028(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x00000028:word32] = __swr(Mem0[sp + 0x00000028:word32], r8)");
        }

        [Test(Description = "Oddly, we see production code that writes to the r0 register. We musn't allow that assignment result in invalid code")]
        public void MipsRw_WriteToR0()
        {
            AssertCode(0x03E00025,              // or r0,r0,r31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = ra");
        }

        [Test]
        public void MipsRw_trap()
        {
            AssertCode(0x00000034, // teq zero, zero	 
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__trap(0x0000)");
        }

        [Test]
        public void MipsRw_Shifts_64bit()
        {
            Given_Mips64_Architecture();
            AssertCode(0x001FE03E, // dsrl32 gp,ra,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = ra >>u 0x00 + 0x20");

            AssertCode(0x001ce03c, // dsll32 gp,gp,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = r28 << 0x00 + 0x20");

            AssertCode(0x0062182f, // dsubu v1, v1, v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r2");


            AssertCode(0x000208fa, // dsrl at,v0,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 >>u 0x03");

            AssertCode(0x000510f8, // dsll v0, a1,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r5 << 0x03");

            AssertCode(0x00410814, // dsllv at, at, v0	      
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r1 >>u r2");
        }

        [Test]
        public void MipsRw_Mult_64bit()
        {
            Given_Mips64_Architecture();
            AssertCode(0x00c2001d, // dmultu a2,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = r6 *u r2");

            AssertCode(0x0082001c, // dmult a0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = r4 *s r2");
        }

        [Test]
        public void MipsRw_Various_FPU_instructions()
        {
            AssertCode(0x46200832, // c.eq.d $f1,$f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f1_f2 == f0_f1");

            AssertCode(0x46A008E1, // cvt.d.l $f3,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f3 = (real64) f1_f2");

            AssertCode(0x46200820, // cvt.s.d $f0,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real32) f1_f2");
        }

        [Test]
        public void MipsRw_dmtc1()
        {
            Given_Mips64_Architecture();
            AssertCode(0x44A40800, // dmtc1 a0,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = r4");
        }

        [Test]
        public void MipsRw_sub_d()
        {
            AssertCode(0x463AD601, // sub.d $f24,$f26,$f26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f24_f25 = f26_f27 - f26_f27");
        }

        [Test]
        public void MipsRw_trunc_l_d()
        {
            AssertCode(0x46200049, // trunc.l.d $f1,$f0
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v2 = f0",
            "2|L--|f1 = (int64) trunc(v2)");
        }

        [Test]
        public void MipsRw_sdc1()
        {
            Given_Mips64_Architecture();
            AssertCode(0xf7a10018, // sdc1 $f1,24(sp)
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[sp + 0x0000000000000018:word64] = f1");
        }

        [Test]
        public void MipsRw_mov_d()
        {
            AssertCode(0x46200806, // mov.d $f0,$f1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f0 = f1");
        }

        [Test]
        public void MipsRw_div_d()
        {
            AssertCode(0x46220003, // div.d $f0,$f0,$f2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f0_f1 = f0_f1 / f2_f3");
        }

        [Test]
        public void MipsRw_dsrlv()
        {
            Given_Mips64_Architecture();
            AssertCode(0x00221016, // dsrlv v0, v0, at
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r2 >>u r1");
        }

        [Test]
        public void MipsRw_lwc1()
        {
            AssertCode(0xc4240000, // lwc1 $f4,0(at)
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f4 = (word32) Mem0[r1:real32]");
        }

        [Test]
        public void MipsRw_c_lt_d()
        {
            AssertCode(0x4620083c, // c.lt.d $f1,$f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f1_f2 < f0_f1");
        }

        [Test]
        public void MipsRw_dmfc1()
        {
            Given_Mips64_Architecture();
            AssertCode(0x44210800, // dmfc1 at,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = f1");
        }

        [Test]
        public void MipsRw_mul_s()
        {
            AssertCode(0x46020842, // mul.s $f1,$f1,$f2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f1 * f2");
        }

        [Test]
        public void MipsRw_c_eq_s()
        {
            AssertCode(0x46002032, // c.eq.s $f4,$f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f4_f5 == f0_f1");
        }

        [Test]
        public void MipsRw_c_lt_s()
        {
            AssertCode(0x4600083c, // c.lt.s $f1,$f0 
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f1_f2 < f0_f1");
        }

        [Test]
        public void MipsRw_c_le_s()
        {
            AssertCode(0x4600083e, // c.le.s $f1,$f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cc0 = f1_f2 <= f0_f1");
        }

        [Test]
        public void MipsRw_mov_s()
        {
            AssertCode(0x46001046, // mov.s $f1,$f2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f2");
        }

        [Test]
        public void MipsRw_mtc0()
        {
            AssertCode(0x40826000,   // mtc0	r2,r12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__cp12 = r2");
        }

        [Test]
        public void MipsRw_pref()
        {
            AssertCode(0xCC5E0000,   // pref	r30,0000(r2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__prefetch(r2)");
        }

        [Test]
        public void MipsRw_add_s()
        {
            AssertCode(0x46000000,   // add.s	f0,f0,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 + f0");
        }

        [Test]
        public void MipsRw_movt()
        {
            AssertCode(0x00212901,   // movt	r1,r1,fcc0
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (!fcc0) branch 00100004",
                "2|L--|r1 = r1");
        }

        [Test]
        public void MipsRw_tnei()
        {
            AssertCode(0x060E1701,   // tnei	r16,+00001701
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r16 == 5889) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void MipsRw_movf()
        {
            AssertCode(0x00000001,   // movf	r0,r0,fcc0
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (fcc0) branch 00100004",
                "2|L--|r0 = r0");
        }

        [Test]
        public void MipsRw_tlti()
        {
            AssertCode(0x06CA6351,   // tlti	r22,+00006351
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r22 >= 25425) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void MipsRw_tgeiu()
        {
            AssertCode(0x06096086,   // tgeiu	r16,+00006086
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r16 <u 24710) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void MipsRw_dmtc0()
        {
            Given_Mips64_Architecture();
            AssertCode(0x40A04800,   // dmtc0	r0,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__counter__ = 0x0000000000000000");
        }

        [Test]
        public void MipsRw_tlbp()
        {
            AssertCode(0x42000008,   // tlbp
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__tlbp()");
        }

        [Test]
        public void MipsRw_tlbwi()
        {
            AssertCode(0x42000002,   // tlbwi
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__tlbwi()");
        }

        [Test]
        public void MipsRw_dmfc0()
        {
            AssertCode(0x40224807,   // dmfc0	r2,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __counter__");
        }

        public void MipsRw_tlbwr()
        {
            AssertCode(0x43444546,   // tlbwr
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void MipsRw_eret()
        {
            AssertCode(0x43564D58,   // eret
                "0|L--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void MipsRw_wait()
        {
            AssertCode(0x43415320,   // wait
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__wait()");
        }

        [Test]
        public void MipsRw_tlbr()
        {
            AssertCode(0x43415041,   // tlbr
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__tlbr()");
        }

        [Test]
        public void MipsRw_bgezal_is_bal()
        {
            // The MIPS docs specify that bgezal r0,XXXX
            // is interpreted by the processor as bal XXXX.
            // It's a silicon hack, but we have to deal
            // with it....
            AssertCode(0x0411FF66,      // bgezal   r0,xxxx
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|call 000FFD9C (0)");
        }
    }
}
