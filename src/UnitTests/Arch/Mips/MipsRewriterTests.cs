#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        static MipsProcessorArchitecture arch = new MipsBe32Architecture();
        private MipsDisassembler dasm;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

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
            dasm = new MipsDisassembler(arch, image.CreateBeReader(LoadAddress), false);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new MipsRewriter(arch, dasm, frame, host);
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
                "1|L--|r13 = (word32) Mem0[r11 - 0x00000008:word16]");
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
            RunTest("110100 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __load_linked_64(Mem0[r10 - 0x00000038:word64])");
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
                "0|L--|00100000(4): 2 instructions",
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
        public void MipsRw_mul()
        {
            AssertCode(0x02F00018, // mult s7,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = r23 *s r16");
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
                "1|L--|Mem0[sp + 0x00000030:word32] = f12");
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
        public void MipsRw_sd()
        {
            AssertCode(0xFC444444,                              // sd
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r2 + 0x00004444:word64] = r4");
        }

        [Test]
        public void MipsRw_swl_swr()
        {
            AssertCode(0xABA8002B,                // swl r8, 002B(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__swl(Mem0[sp + 0x0000002B:word32], r8)");

            AssertCode(0xBBA80028,                // swr r8, 0028(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__swr(Mem0[sp + 0x00000028:word32], r8)");
        }
    }
}