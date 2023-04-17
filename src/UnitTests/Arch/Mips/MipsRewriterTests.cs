#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsRewriterTests : RewriterTestBase
    {
        private MipsProcessorArchitecture arch = new MipsBe32Architecture(CreateServiceContainer(), "mips-be-32", new Dictionary<string, object>());
        private Func<EndianImageReader, IEnumerable<MachineInstruction>> mkDasm;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture(new ServiceContainer(), "mips-be-32", new Dictionary<string, object>());
            this.mkDasm = rdr => arch.CreateDisassembler(rdr);
        }

        private void AssertCode(uint instr, params string[] sExp)
        {
            Given_UInt32s(instr);
            AssertCode(sExp);
        }

        private void Given_Mips64_Architecture()
        {
            arch = new MipsBe64Architecture(new ServiceContainer(), "mips-be-64", new Dictionary<string, object>());
            mkDasm = rdr => arch.CreateDisassembler(rdr);
        }

        private void Given_NanoDecoder()
        {
            mkDasm = rdr => new NanoMipsDisassembler(arch, rdr);
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var dasm = mkDasm(arch.CreateImageReader(mem, 0)).Cast<MipsInstruction>();
            return new MipsRewriter(arch, arch.Intrinsics, null, dasm, binder, host);
        }

        [Test]
        public void MipsRw_lh()
        {
            Given_BitStrings("100001 01001 00011 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = CONVERT(Mem0[r9 - 0x38<32>:int16], int16, word32)");
        }

        [Test]
        public void MipsRw_lhu()
        {
            Given_BitStrings("100101 01011 01101 1111111111111000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = CONVERT(Mem0[r11 - 8<32>:uint16], uint16, word32)");
        }

        [Test]
        public void MipsRw_lui()
        {
            Given_BitStrings("001111 00000 00011 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = 0xFFC80000<32>");
        }
        [Test]
        public void MipsRw_ll()
        {
            Given_BitStrings("110000 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __load_linked<word32>(&Mem0[r10 - 0x38<32>:word32])");
        }
        [Test]
        public void MipsRw_lld()
        {
            Given_Mips64_Architecture();
            Given_BitStrings("110100 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __load_linked<word64>(&Mem0[r10 - 0x38<64>:word64])");
        }

        [Test]
        public void MipsRw_sc()
        {
            Given_BitStrings("111000 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __store_conditional<word32>(&Mem0[r10 - 0x38<32>:word32], r21)");
        }
        [Test]
        public void MipsRw_scd()
        {
            Given_Mips64_Architecture();
            Given_BitStrings("111100 01010 10101 1111111111001000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __store_conditional<word64>(&Mem0[r10 - 0x38<64>:word64], r21)");
        }

        [Test]
        public void MipsRw_ori_r0()
        {
            Given_BitStrings("001101 00000 00101 1111100000100111");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = 0xF827<32>");
        }

        [Test]
        public void MipsRw_addi_r0()
        {
            Given_BitStrings("001000 00000 00010 1111111111111000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = -8<i32>");
        }

        [Test]
        public void MipsRw_add()
        {
            Given_BitStrings("000000 00001 00010 00011 00000 100000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r1 + r2");
        }

        [Test]
        public void MipsRw_andi_0()
        {
            Given_BitStrings("001100 00000 00101 0000000000000000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = 0<32>");
        }

        [Test]
        public void MipsRw_bgtz()
        {
            Given_BitStrings("000111 00011 00000 1111111111111110");
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r3 > 0<32>) branch 000FFFFC");
        }

        [Test]
        public void MipsRw_j()
        {
            Given_BitStrings("000010 11111111111111111111111111");
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto 0FFFFFFC");
        }

        [Test]
        public void MipsRw_sw()
        {
            AssertCode(0xAFBF0020,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x20<32>:word32] = ra");
        }

        [Test]
        public void MipsRw_sd_64()
        {
            Given_Mips64_Architecture();
            AssertCode(0xFFBF0020,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x20<64>:word64] = ra");
        }

        [Test]
        public void MipsRw_jal()
        {
            AssertCode(0x0C009B2C,
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 00026CB0 (0)");
        }

        [Test]
        public void MipsRw_srl()
        {
            AssertCode(0x24c02,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r2 >>u 0x10<8>");
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
                "1|L--|r19 = Mem0[sp + 0x10<32>:word32]");
            AssertCode(0x8fb3FFF0,   // lw s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[sp - 0x10<32>:word32]");
        }

        [Test(Description = "On MIPS64, lw loads a signed integer")]
        public void MipsRw_lw_64bit()
        {
            Given_Mips64_Architecture();
            AssertCode(0x8fb3FFF0,   // lw s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = CONVERT(Mem0[sp - 0x10<64>:int32], int32, word64)");
        }

        [Test]
        public void MipsRw_ld()
        {
            Given_Mips64_Architecture();
            AssertCode(0xdfb30010,   // ld s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[sp + 0x10<64>:word64]");
        }

        [Test]
        public void MipsRw_lwu()
        {
            Given_Mips64_Architecture();
            AssertCode(0x9fb30010,   // ld s3,16(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = CONVERT(Mem0[sp + 0x10<64>:uint32], uint32, word64)");
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
                "0|TD-|00100000(4): 1 instructions",
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
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call r9 (0)");
        }

        [Test]
        public void MipsRw_sltu()
        {
            AssertCode(0x0211402B,  // sltu t0,s0,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = CONVERT(r16 <u r17, bool, word32)");
        }

        [Test]
        public void MipsRw_jr_returns()
        {
            AssertCode(0x03E00008, // jr ra
                "0|RD-|00100000(4): 1 instructions",
                "1|RD-|return (0,0)");
        }

        [Test]
        public void MipsRw_sb()
        {
            AssertCode(0xA128D958, // sb t0,-9896(t1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9 - 0x26A8<32>:byte] = SLICE(r8, byte, 0)");
        }

        [Test]
        public void MipsRw_tge()
        {
            AssertCode(0x00F000F0,  // tge a3,s0,0x3
                "0|TD-|00100000(4): 2 instructions",
                "1|T--|if (r7 < r16) branch 00100004",
                "2|L--|__syscall<word16>(3<16>)");
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
                "1|L--|r9 = CONVERT(r11 < 112<i32>, bool, word32)");
        }

        [Test]
        public void MipsRw_lwl_lwr()
        {
            AssertCode(0x88c80003,  // lwl t0,3(a2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __lwl(r8, Mem0[r6 + 3<32>:word32])");

            AssertCode(0x98c80000,   // lwr t0,0(a2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __lwr(r8, Mem0[r6:word32])");
        }

        [Test]
        public void MipsRw_mfhi()
        {
            AssertCode(0x4010,  // mfhi t0
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
                "1|L--|Mem0[sp + 0x30<32>:word32] = f12");
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
                "1|L--|f0 = CONVERT(f12_f13, real64, int32)");

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
            Given_BitStrings("011111 00000 00110 00000 00000 111011");   // CPU number
            AssertCode("0|L--|00100000(4): 1 instructions",
                       "1|L--|r6 = __read_cpu_number()");

            Given_BitStrings("011111 00000 01000 00001 00000 111011");   // SYNCI step size
            AssertCode("0|L--|00100000(4): 1 instructions",
                       "1|L--|r8 = __read_hardware_register(1<8>)");

            Given_BitStrings("011111 00000 00001 00010 00000 111011");   // Cycle counter
            AssertCode("0|L--|00100000(4): 1 instructions",
                       "1|L--|r1 = __read_hardware_register(2<8>)");

            Given_BitStrings("011111 00000 00011 00011 00000 111011");   // Cycle counter resolution
            AssertCode("0|L--|00100000(4): 1 instructions",
                       "1|L--|r3 = __read_hardware_register(3<8>)");

            Given_BitStrings("011111 00000 00111 11101 00000 111011");   // OS-specific, thread local pointer on Linux
            AssertCode("0|L--|00100000(4): 1 instructions",
                       "1|L--|r7 = __read_user_local()");
        }

        [Test]
        public void MipsRw_movz()
        {
            Given_BitStrings("000000 00011 01001 01010 00000 001010");    // movz
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r9 != 0<32>) branch 00100004",
                "2|L--|r10 = r3");
        }



        [Test]
        public void MipsRw_swl_swr()
        {
            AssertCode(0xABA8002B,                // swl r8, 002B(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x2B<32>:word32] = __swl(Mem0[sp + 0x2B<32>:word32], r8)");

            AssertCode(0xBBA80028,                // swr r8, 0028(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x28<32>:word32] = __swr(Mem0[sp + 0x28<32>:word32], r8)");
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
            AssertCode(0x34, // teq zero, zero	 
                "0|TD-|00100000(4): 1 instructions",
                "1|L--|__syscall<word16>(0<16>)");
        }

        [Test]
        public void MipsRw_Shifts_64bit()
        {
            Given_Mips64_Architecture();
            AssertCode(0x001FE03E, // dsrl32 gp,ra,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = ra >>u 0<8> + 0x20<8>");

            AssertCode(0x001ce03c, // dsll32 gp,gp,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = r28 << 0<8> + 0x20<8>");

            AssertCode(0x0062182f, // dsubu v1, v1, v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r2");


            AssertCode(0x208fa, // dsrl at,v0,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 >>u 3<8>");

            AssertCode(0x510f8, // dsll v0, a1,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r5 << 3<8>");

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
        }

        [Test]
        public void MipsRw_cvt_d_l()
        {
            AssertCode(0x46A008E1, // cvt.d.l $f3,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f3_f4 = CONVERT(f1, int32, real64)");
        }

        [Test]
        public void MipsRw_cvt_s_d()
        {
            AssertCode(0x46200820, // cvt.s.d $f0,$f1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(f1_f2, real64, real32)");
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
            "1|L--|v3 = f0",
            "2|L--|f1 = CONVERT(trunc<real64>(v3), real64, int64)");
        }

        [Test]
        public void MipsRw_sdc1()
        {
            Given_Mips64_Architecture();
            AssertCode(0xf7a10018, // sdc1 $f1,24(sp)
            "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 0x18<64>:word64] = f1");
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
            "1|L--|f4 = Mem0[r1:word32]");
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
                "1|L--|__prefetch<ptr32>(r2)");
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
                "1|T--|if (r16 == 5889<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_movf()
        {
            AssertCode(0x1,   // movf	r0,r0,fcc0
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (fcc0) branch 00100004",
                "2|L--|r0 = r0");
        }

        [Test]
        public void MipsRw_tlti()
        {
            AssertCode(0x06CA6351,   // tlti	r22,+00006351
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r22 >= 25425<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_tgeiu()
        {
            AssertCode(0x06096086,   // tgeiu	r16,+00006086
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r16 <u 24710<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_dmtc0()
        {
            Given_Mips64_Architecture();
            AssertCode(0x40A04800,   // dmtc0	r0,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__counter__ = 0<64>");
        }

        [Test]
        public void MipsRw_tlbp()
        {
            AssertCode(0x42000008,   // tlbp
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__tlbp()");
        }

        [Test]
        public void MipsRw_tlbwi()
        {
            AssertCode(0x42000002,   // tlbwi
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__tlbwi()");
        }

        [Test]
        public void MipsRw_dmfc0()
        {
            AssertCode(0x40224807,   // dmfc0	r2,r9
                "0|S--|00100000(4): 1 instructions",
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
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void MipsRw_wait()
        {
            AssertCode(0x43415320,   // wait
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__wait()");
        }

        [Test]
        public void MipsRw_tlbr()
        {
            AssertCode(0x43415041,   // tlbr
                "0|S--|00100000(4): 1 instructions",
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
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 000FFD9C (0)");
        }

        [Test]
        public void MipsRw_bgezal_idiom()
        {
            // This idiom is used to capture the address of the 
            // called destination in the la register.
            AssertCode(0x04110001,      // skip the delay slot.
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ra = 00100008");
        }

        [Test]
        public void MipsRw_seh()
        {
            AssertCode(0x7C021620, // seh\tr2,r2"
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r2, word16, 0)",
                "2|L--|r2 = CONVERT(v4, word16, int32)");
        }

        [Test]
        public void MipsRw_save16()
        {
            Given_NanoDecoder();
            Given_HexString("1CAA");
            AssertCode(
                "0|L--|00100000(2): 11 instructions",
                "1|L--|Mem0[sp + -4<i32>:word32] = r30",
                "2|L--|Mem0[sp + -8<i32>:word32] = ra",
                "3|L--|Mem0[sp + -12<i32>:word32] = r16",
                "4|L--|Mem0[sp + -16<i32>:word32] = r17",
                "5|L--|Mem0[sp + -20<i32>:word32] = r18",
                "6|L--|Mem0[sp + -24<i32>:word32] = r19",
                "7|L--|Mem0[sp + -28<i32>:word32] = r20",
                "8|L--|Mem0[sp + -32<i32>:word32] = r21",
                "9|L--|Mem0[sp + -36<i32>:word32] = r22",
                "10|L--|Mem0[sp + -40<i32>:word32] = r23",
                "11|L--|sp = sp - 160<i32>");
        }

        [Test]
        public void MipsRw_restore16()
        {
            Given_NanoDecoder();
            Given_HexString("1DAA");
            AssertCode(
                "0|R--|00100000(2): 12 instructions",
                "1|L--|r30 = Mem0[sp + 156<i32>:word32]",
                "2|L--|ra = Mem0[sp + 152<i32>:word32]",
                "3|L--|r16 = Mem0[sp + 148<i32>:word32]",
                "4|L--|r17 = Mem0[sp + 144<i32>:word32]",
                "5|L--|r18 = Mem0[sp + 140<i32>:word32]",
                "6|L--|r19 = Mem0[sp + 136<i32>:word32]",
                "7|L--|r20 = Mem0[sp + 132<i32>:word32]",
                "8|L--|r21 = Mem0[sp + 128<i32>:word32]",
                "9|L--|r22 = Mem0[sp + 124<i32>:word32]",
                "10|L--|r23 = Mem0[sp + 120<i32>:word32]",
                "11|L--|sp = sp + 160<i32>",
                "12|R--|return (0,0)");
        }

        [Test]
        public void MipsRw_lwxs()
        {
            Given_NanoDecoder();
            Given_HexString("50CB");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = Mem0[r17 + r4 * 4<32>:word32]");
        }

        [Test]
        public void MipsRw_sw_4x4()
        {
            Given_NanoDecoder();
            Given_HexString("F4C0");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r8:word32] = r6");
        }

        [Test]
        public void MipsRw_move()
        {
            Given_NanoDecoder();
            Given_HexString("106B");   // move	r3,r11
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r11");
        }

        [Test]
        public void MipsRw_bnezc()
        {
            Given_NanoDecoder();
            Given_HexString("BBA4");   // bnezc	r7,08048340
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r7 != 0<32>) branch 00100026");
        }

        [Test]
        public void MipsRw_beqc()
        {
            Given_NanoDecoder();
            Given_HexString("88E03D3D");   // beqc	r0,r7,08048052
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (0<32> == r7) branch 000FFD40");
        }

        [Test]

        public void MipsRw_bbeqzc()
        {
            Given_NanoDecoder();
            Given_HexString("C904B812");   // bbeqzc	r8,00000017,00100016
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (__bit<word32,word32>(r8, 0x17<32>)) branch 00100016");
        }

        [Test]
        public void MipsRw_bc()
        {
            Given_NanoDecoder();
            Given_HexString("1BB1");   // bc	000FFFB2
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 000FFFB2");
        }

        [Test]
        public void MipsRw_bgeic()
        {
            Given_NanoDecoder();
            Given_HexString("C8E9C86C");   // bgeic	r7,00000039,00100070
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 >= 0x39<32>) branch 00100070");
        }

        [Test]
        public void MipsRw_beqic()
        {
            Given_NanoDecoder();
            Given_HexString("C8E025DB");   // beqic	r7,00000004,08048064
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 == 4<32>) branch 000FFDDE");
        }

        [Test]
        public void MipsRw_ins()
        {
            Given_NanoDecoder();
            Given_HexString("8100E5D7");   // ins	r8,r0,00000007,00000001
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __ins<word32,word32>(r8, 0<32>, 7<32>, 1<32>)");
        }

        [Test]
        public void MipsRw_clz()
        {
            Given_NanoDecoder();
            Given_HexString("20E45B3F");   // clz	r7,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = __count_leading_zeros<word32>(r4)");
        }

        [Test]
        public void MipsRw_li()
        {
            Given_NanoDecoder();
            Given_HexString("D3A0");   // li	r7,00000020
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = 0x20<32>");
        }

        [Test]
        public void MipsRw_beqzc()
        {
            Given_NanoDecoder();
            Given_HexString("9BB6");   // beqzc	r7,080484D2
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r7 == 0<32>) branch 00100038");
        }

        [Test]
        public void MipsRw_bbnezc()
        {
            Given_NanoDecoder();
            Given_HexString("C9349820");   // bbnezc	r9,00000013,0804851A
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (!__bit<word32,word32>(r9, 0x13<32>)) branch 00100024");
        }

        [Test]
        public void MipsRw_movep()
        {
            Given_NanoDecoder();
            Given_HexString("FFFE");   // movep	r22,r23,r7,r8
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r22 = r7",
                "2|L--|r23 = r8");
        }

        [Test]
        public void MipsRw_not()
        {
            Given_NanoDecoder();
            Given_HexString("5050");   // not	r16,r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r16 = ~r5");
        }

        [Test]
        public void MipsRw_balc()
        {
            Given_NanoDecoder();
            Given_HexString("3810");
            AssertCode(   // balc	080485E2
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call 00100012 (0)");
        }


        [Test]
        public void MipsRw_bgeiuc()
        {
            Given_NanoDecoder();
            Given_HexString("C8ED080A");   // bgeiuc	r7,00000021,08048096
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 >=u 0x21<32>) branch 0010000E");
        }

        [Test]
        public void MipsRw_sigrie()
        {
            Given_NanoDecoder();
            Given_HexString("00000000");   // sigrie	00000000
            AssertCode(
                "0|H--|00100000(4): 1 instructions",
                "1|H--|__reserved_instruction(0<32>)");
        }

        [Test]
        public void MipsRw_addiupc()
        {
            Given_NanoDecoder();
            Given_HexString("04C00000");   // addiupc	r6,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = 00100004");
        }

        [Test]
        public void MipsRw_jalrc()
        {
            Given_NanoDecoder();
            Given_HexString("DA30");   // jalrc	ra,r17
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r17 (0)");
        }

        [Test]
        public void MipsRw_restore()
        {
            Given_NanoDecoder();
            Given_HexString("83CA37E2");   // restore	000007E0,r30,0000000A
            AssertCode(
                "0|L--|00100000(4): 11 instructions",
                "1|L--|r30 = Mem0[sp + 2012<i32>:word32]",
                "2|L--|ra = Mem0[sp + 2008<i32>:word32]",
                "3|L--|r16 = Mem0[sp + 2004<i32>:word32]",
                "4|L--|r17 = Mem0[sp + 2000<i32>:word32]",
                "5|L--|r18 = Mem0[sp + 1996<i32>:word32]",
                "6|L--|r19 = Mem0[sp + 1992<i32>:word32]",
                "7|L--|r20 = Mem0[sp + 1988<i32>:word32]",
                "8|L--|r21 = Mem0[sp + 1984<i32>:word32]",
                "9|L--|r22 = Mem0[sp + 1980<i32>:word32]",
                "10|L--|r23 = Mem0[sp + 1976<i32>:word32]",
                "11|L--|sp = sp + 2016<i32>");
        }

        [Test]
        public void MipsRw_move_balc()
        {
            Given_NanoDecoder();
            Given_HexString("09E00000");   // move_balc	r5,r7,08049546
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r5 = r7",
                "2|T--|call 00100004 (0)");
        }

        [Test]
        public void MipsRw_lwm()
        {
            Given_NanoDecoder();
            Given_HexString("A4D02400");   // lwm	r6,0000(r16),00000002
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = Mem0[r16 + 0<i32>:word32]",
                "2|L--|r7 = Mem0[r16 + 4<i32>:word32]");
        }

        [Test]
        public void MipsRw_swxs()
        {
            Given_NanoDecoder();
            Given_HexString("208534C7");   // swxs	r6,r5(r4)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r4 + r5 * 4<32>:word32] = r6");
        }

        [Test]
        public void MipsRw_lsa()
        {
            Given_NanoDecoder();
            Given_HexString("2205860F");   // lsa	r16,r5,r16,00000003
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = r16 + (r5 << 3<8>)");
        }

        [Test]
        public void MipsRw_ualwm()
        {
            Given_NanoDecoder();
            Given_HexString("A5441501");   // ualwm	r10,0001(r4),00000001
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = Mem0[r4 + 1<i32>:word32]");
        }

        [Test]
        public void MipsRw_bltuc()
        {
            Given_NanoDecoder();
            Given_HexString("A8E5C000");   // bltuc	r5,r7,08048186
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r5 <u r7) branch 00100004");
        }

        [Test]
        public void MipsRw_aluipc()
        {
            Given_NanoDecoder();
            Given_HexString("E0C00002");   // aluipc	r6,00008048
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = 0x100<32>");
        }

        [Test]
        public void MipsRw_bltiuc()
        {
            Given_NanoDecoder();
            Given_HexString("C97C9000");  // bltiuc	r11,00000012,08048770
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r11 <u 0x12<32>) branch 00100004");
        }

        [Test]
        public void MipsRw_bltic()
        {
            Given_NanoDecoder();
            Given_HexString("CA581800");  // bltic	r18,00000003,0804879E
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r18 < 3<32>) branch 00100004");
        }

        [Test]
        public void MipsRw_lbux()
        {
            Given_NanoDecoder();
            Given_HexString("20C73107");  // lbux	r6,r7(r6)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = CONVERT(Mem0[r6 + r7:byte], byte, word32)");
        }

        [Test]
        public void MipsRw_lwx()
        {
            Given_NanoDecoder();
            Given_HexString("22472407");  // lwx	r4,r7(r18)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r18 + r7:word32]");
        }

        [Test]
        public void MipsRw_cache()
        {
            AssertCode(0xBCC80000,   // cache	08,0000(r6)
                "0|S--|00100000(4): 1 instructions",
                "1|S--|__cache(8<8>, &Mem0[r6:word32])");
        }

        [Test]
        public void MipsRw_madd_s()
        {
            AssertCode(0x4C804F60,   // madd.s	f29,f4,f9,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f29 = f4 + f9 * f0");
        }

        [Test]
        public void MipsRw_madd()
        {
            AssertCode(0x71A60000,   // madd	r13,r6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = hi_lo + r13 *64 r6");
        }

        [Test]
        public void MipsRw_msub()
        {
            AssertCode(0x71670004,   // msub	r11,r7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi_lo = hi_lo - r11 *64 r7");
        }

        [Test]
        public void MipsRw_tgei()
        {
            AssertCode(0x04080420,   // tgei	r0,+00000420
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r0 < 1056<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_nmadd_s()
        {
            AssertCode(0x4C11DB70,   // nmadd_s	f13,f0,f27,f17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f13 = -(f0 + f27 * f17)");
        }

        [Test]
        public void MipsRw_ldc2()
        {
            AssertCode(0xD9714B49,   // ldc2	r17,4B49(r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__write_cpr2(0x11<8>, Mem0[r11 + 0x4B49<32>:word64])");
        }

        [Test]
        public void MipsRw_lwc2()
        {
            AssertCode(0xCA753D95,   // lwc2	r21,3D95(r19)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__write_cpr2(0x15<8>, Mem0[r19 + 0x3D95<32>:word64])");
        }

        [Test]
        public void MipsRw_sdc2()
        {
            AssertCode(0xFBB8BB46,   // sdc2	r24,-44BA(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp - 0x44BA<32>:word64] = __read_cpr2<word64>(0x18<8>)");
        }

        [Test]
        public void MipsRw_swc2()
        {
            AssertCode(0xE8BCCD9A,   // swc2	r28,-3266(r5)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r5 - 0x3266<32>:word32] = __read_cpr2<word32>(0x1C<8>)");
        }

        [Test]
        public void MipsRw_tltiu()
        {
            AssertCode(0x054BF6A4,   // tltiu	r10,-0000095C
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (r10 >=u -2396<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_luxc1()
        {
            AssertCode(0x4E8EE645,   // luxc1	f25,r14(r20)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f25 = Mem0[r20 + r14:word32]");
        }

        [Test]
        public void MipsRw_msub_s()
        {
            AssertCode(0x4D453A28,   // msub.s	f8,f10,f7,f5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f8 = f10 - f7 * f5");
        }

        [Test]
        public void MipsRw_teqi()
        {
            AssertCode(0x058CF428,   // teqi	r12,-00000BD8
                "0|TD-|00100000(4): 2 instructions",
                "1|T--|if (r12 != -3032<i32>) branch 00100004",
                "2|L--|__syscall()");
        }

        [Test]
        public void MipsRw_ldxc1()
        {
            AssertCode(0x4D455441,   // ldxc1	f10,r5(r10)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f10 = Mem0[r10 + r5:word64]");
        }

        [Test]
        public void MipsRw_lwxc1()
        {
            AssertCode(0x4F000000,   // lwxc1	f0,r0(r24)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r24:word32]");
        }

        [Test]
        public void MipsRw_clo()
        {
            AssertCode(0x73E0C9A1,   // clo	r25,ra
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r25 = __count_leading_ones<word32>(ra)");
        }

        [Test]
        public void MipsRw_nmsub_d()
        {
            AssertCode(0x4E989AF9,   // nmsub_d	f11,f20,f19,f24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f11 = -(f20 - f19 *64 f24)");
        }

        [Test]
        public void MipsRw_nmsub_ps()
        {
            AssertCode(0x4DF99A7E,   // nmsub_ps	f9,f15,f19,f25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f9 = -(f15 - f19 * f25)");
        }

        [Test]
        public void MipsRw_madd_ps()
        {
            AssertCode(0x4D3199E6,   // madd_ps	f7,f9,f19,f17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f7 = f9 + f19 * f17");
        }

        [Test]
        public void MipsRw_swxc1()
        {
            AssertCode(0x4D0999C8,   // swxc1	f19,r9(r8)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r8 + r9:word32] = f19");
        }

        [Test]
        public void MipsRw_wsbh()
        {
            AssertCode(0x7C1490A0, // wsbh r18,r20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = __word_swap_bytes_in_halfwords<word32>(r20)");
        }
    }
}
