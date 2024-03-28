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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVRewriterTests : RewriterTestBase
    {
        private RiscVArchitecture arch = new RiscVArchitecture(CreateServiceContainer(), "riscV", new Dictionary<string, object>());
        private Address baseAddr = Address.Ptr64(0x0010000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        private void Given_32bitFloat()
        {
            arch = new RiscVArchitecture(
                CreateServiceContainer(),
                "riscV",
                new Dictionary<string, object>
                {
                    { "WordSize" , "32" },
                    { "FloatAbi", 32 },
                });
            baseAddr = Address.Ptr32(0x0010000);
        }

        private void Given_128bitFloat()
        {
            arch = new RiscVArchitecture(
                CreateServiceContainer(),
                "riscV",
                new Dictionary<string, object>
                {
                    { "WordSize", "64" },
                    { "FloatAbi", 128 }
                });
        }

        // No floating point support.
        private void Given_32bit()
        {
            arch = new RiscVArchitecture(
                CreateServiceContainer(),
                "riscV",
                new Dictionary<string, object>
                {
                    { "WordSize" , "32" },
                });
            baseAddr = Address.Ptr32(0x0010000);
        }

        private void Given_RiscVInstructions(params uint[] words)
        {
            byte[] bytes;
            if ((words[0] & 0b11) != 0b11)
            {
                bytes = new byte[] {
                    (byte) words[0],
                    (byte) (words[0] >> 8),
                };
            }
            else
            {
                bytes = words.SelectMany(w => new byte[]
                {
                    (byte) w,
                    (byte) (w >> 8),
                    (byte) (w >> 16),
                    (byte) (w >> 24)
                }).ToArray();
            }
            Given_MemoryArea(new ByteMemoryArea(LoadAddress, bytes));
        }

        [SetUp]
        public void Setup()
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize , "64" },
                { "FloatAbi", 64 },
            });
            baseAddr = Address.Ptr64(0x0010000);
        }


        [Test]
        public void RiscV_rw_add()
        {
            Given_RiscVInstructions(0x00E787B3u);    // add\ta5,a5,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = a5 + a4");
        }

        [Test]
        public void RiscV_rw_addi_zero()
        {
            Given_RiscVInstructions(0xFFF00413); // addi s0,zero,-00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = -1<i64>");
        }

        [Test]
        public void RiscV_rw_addiw()
        {
            Given_RiscVInstructions(0x0087879Bu);    // addiw\ta5,a5,+00000008
            AssertCode(
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v4 = SLICE(a5, word32, 0)",
                "2|L--|a5 = CONVERT(v4 + 8<i32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_addiw_sign_extend()
        {
            Given_RiscVInstructions(0x00002301);    // c.addiw\tt1,00000000
            AssertCode(
                "0|L--|0000000000010000(2): 2 instructions",
                "1|L--|v4 = SLICE(t1, word32, 0)",
                "2|L--|t1 = CONVERT(v4, word32, int64)");
        }

        [Test]
        public void RiscV_rw_addw()
        {
            Given_HexString("3B8F2000");
            AssertCode(     // addw	t5,ra,sp
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v4 = SLICE(ra, word32, 0)",
                "2|L--|v6 = SLICE(sp, word32, 0)",
                "3|L--|t5 = CONVERT(v4 + v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoadd_d()
        {
            Given_HexString("AFB36301");
            AssertCode(     // amoadd.d	t2,t2,s6
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t2 = __amo_add<word64>(s6, &Mem0[t2:word64])");
        }

        [Test]
        public void RiscV_rw_amoadd_w()
        {
            Given_HexString("AF21AF03");
            AssertCode(     // amoadd.w.rl	gp,s10,(t5)
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_add<word32>(s10, &Mem0[t5:word32])",
                "2|L--|gp = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoadd_w_rl()
        {
            //$TODO: RL?
            Given_HexString("AFAE6302");
            AssertCode(     // amoadd.w.rl	t4,t2,t1
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_add<word32>(t1, &Mem0[t2:word32])",
                "2|L--|t4 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoand_d()
        {
            Given_HexString("AFBF4263");
            AssertCode(     // amoand.d.rl	t6,t0,s4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t6 = __amo_and<word64>(s4, &Mem0[t0:word64])");
        }

        [Test]
        public void RiscV_rw_amoand_w()
        {
            Given_HexString("2FA4A660");
            AssertCode(     // amoand.w	s0,a3,a0
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_and<word32>(a0, &Mem0[a3:word32])",
                "2|L--|s0 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amomax_d()
        {
            Given_HexString("2FB3C7A1");
            AssertCode(     // amomax.d	t1,t3,(a5)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t1 = __amo_max<int64>(t3, &Mem0[a5:int64])");
        }

        [Test]
        public void RiscV_rw_amomax_d_aq_rl()
        {
            //$TODO: aq,rl
            Given_HexString("AFB197A7");
            AssertCode(     // amomax.d.aq.rl	gp,a5,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|gp = __amo_max<int64>(s9, &Mem0[a5:int64])");
        }

        [Test]
        public void RiscV_rw_amomax_w()
        {
            Given_HexString("AFA097A7");
            AssertCode(     // amomax.w.aq.rl	ra,a5,s9
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_max<int32>(s9, &Mem0[a5:int32])",
                "2|L--|ra = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void RiscV_rw_amomaxu_d()
        {
            Given_HexString("AFB670E0");
            AssertCode(     // amomaxu.d	a3,t2,(ra)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = __amo_max<uint64>(t2, &Mem0[ra:uint64])");
        }

        [Test]
        public void RiscV_rw_amomaxu_d_aq()
        {
            Given_HexString("AFBB08E4");
            AssertCode(     // amomaxu.d.aq	s7,a7,zero
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s7 = __amo_max<uint64>(0<64>, &Mem0[a7:uint64])");
        }

        [Test]
        public void RiscV_rw_amomaxu_w()
        {
            Given_HexString("AFAEEFE0");
            AssertCode(     // amomaxu.w	t4,t6,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_max<uint32>(a4, &Mem0[t6:uint32])",
                "2|L--|t4 = CONVERT(v6, uint32, int64)");
        }

        [Test]
        public void RiscV_rw_amomin_d()
        {
            //$TODO: amo_d_aql_rl
            Given_HexString("AFB7B387");
            AssertCode(     // amomin.d.aq.rl	a5,t2,s11
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = __amo_min<int64>(s11, &Mem0[t2:int64])");
        }

        [Test]
        public void RiscV_rw_amomin_w()
        {
            Given_HexString("AF276680");
            AssertCode(     // amomin.w	a5,t1,(a2)
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_min<int32>(t1, &Mem0[a2:int32])",
                "2|L--|a5 = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void RiscV_rw_amomin_w_aq()
        {
            Given_HexString("2FAF2A84");
            AssertCode(     // amomin.w.aq	t5,s5,sp
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_min<int32>(sp, &Mem0[s5:int32])",
                "2|L--|t5 = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void RiscV_rw_amominu_d()
        {
            Given_HexString("AF35DEC1");
            AssertCode(     // amominu.d	a1,t4,(t3)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a1 = __amo_min<uint64>(t4, &Mem0[t3:uint64])");
        }

        [Test]
        public void RiscV_rw_amominu_d_aq_rl()
        {
            Given_HexString("AFB797C7");
            AssertCode(     // amominu.d.aq.rl	a5,a5,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = __amo_min<uint64>(s9, &Mem0[a5:uint64])");
        }

        [Test]
        public void RiscV_rw_amominu_w()
        {
            Given_HexString("AFA9EFC0");
            AssertCode(     // amominu.w	s3,t6,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_min<uint32>(a4, &Mem0[t6:uint32])",
                "2|L--|s3 = CONVERT(v6, uint32, int64)");
        }

        [Test]
        public void RiscV_rw_amoor_d()
        {
            Given_HexString("2FB29547");
            AssertCode(     // amoor.d.aq.rl	tp,a1,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|tp = __amo_or<word64>(s9, &Mem0[a1:word64])");
        }

        [Test]
        public void RiscV_rw_amoor_w()
        {
            Given_HexString("AFA08947");
            AssertCode(     // amoor.w.aq.rl	ra,s3,s8
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_or<word32>(s8, &Mem0[s3:word32])",
                "2|L--|ra = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoswap_d()
        {
            Given_HexString("2FB26308");
            AssertCode(     // amoswap.d	tp,t2,t1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|tp = __amo_swap<word64>(t1, &Mem0[t2:word64])");
        }

        [Test]
        public void RiscV_rw_amoswap_w()
        {
            Given_HexString("AFA81309");
            AssertCode(     // amoswap.w	a7,t2,a7
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = __amo_swap<word32>(a7, &Mem0[t2:word32])",
                "2|L--|a7 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoxor_d()
        {
            Given_HexString("2FBD2320");
            AssertCode(     // amoxor.d	s10,t2,sp
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s10 = __amo_xor<word64>(sp, &Mem0[t2:word64])");
        }

        [Test]
        public void RiscV_rw_amoxor_w()
        {
            Given_HexString("AFA72320");
            AssertCode(     // amoxor.w	a5,t2,sp
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_xor<word32>(sp, &Mem0[t2:word32])",
                "2|L--|a5 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_and()
        {
            Given_RiscVInstructions(0x00F477B3u);    // and\ta5,s0,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = s0 & a5");
        }

        [Test]
        public void RiscV_rw_auipc()
        {
            Given_RiscVInstructions(0xFFFFF517u); // auipc\tgp,0x000FFFFD<32>
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = 000000000000F000");
        }

        [Test]
        public void RiscV_rw_beq()
        {
            Given_RiscVInstructions(0x00F58063u);    // beq\ta1,a5,0000000000010000
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a1 == a5) branch 0000000000010000");
        }

        [Test]
        public void RiscV_rw_beq_2()
        {
            Given_RiscVInstructions(0x12E50463u);    // beq\ta0,a4,0000000000100128
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a0 == a4) branch 0000000000010128");
        }

        [Test]
        public void RiscV_rw_beqz()
        {
            Given_RiscVInstructions(0xC3F1);    // c.beqz\ta5,00000000001000C4
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a5 == 0<64>) branch 00000000000100C4");
        }

        [Test]
        public void RiscV_rw_beqz_backward()
        {
            Given_RiscVInstructions(0xD399);    // c.beqz\ta5,00000000000FFF06
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a5 == 0<64>) branch 000000000000FF06");
        }

        [Test]
        public void RiscV_rw_c_addi()
        {
            Given_RiscVInstructions(0x00000785);    // c.addi\ta5,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 + 1<i64>");
        }

        [Test]
        public void RiscV_rw_c_addiw()
        {
            Given_RiscVInstructions(0x00002405);    // c.addiw\ts0,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 2 instructions",
                "1|L--|v4 = SLICE(s0, word32, 0)",
                "2|L--|s0 = CONVERT(v4 + 1<i32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_addi16sp()
        {
            Given_RiscVInstructions(0x6169);    // c.addi16sp\t000000D0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|sp = sp + 208<i64>");
        }

        [Test]
        public void RiscV_rw_c_addi4spn()
        {
            Given_RiscVInstructions(0x0000101C);    // c.addi4spn\ta5,00000020
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = sp + 32<i64>");
        }

        [Test]
        public void RiscV_rw_c_addiw_negative()
        {
            Given_RiscVInstructions(0x0000347D);    // c.addiw\ts0,FFFFFFFFFFFFFFFF
            AssertCode(
                "0|L--|0000000000010000(2): 2 instructions",
                "1|L--|v4 = SLICE(s0, word32, 0)",
                "2|L--|s0 = CONVERT(v4 - 1<i32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_addw()
        {
            Given_RiscVInstructions(0x00009FB5);    // c.addw\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 3 instructions",
                "1|L--|v4 = SLICE(a5, word32, 0)",
                "2|L--|v6 = SLICE(a3, word32, 0)",
                "3|L--|a5 = CONVERT(v4 + v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_and()
        {
            Given_RiscVInstructions(0x8FF5);    // c.and\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 & a3");
        }

        [Test]
        public void RiscV_rw_c_andi()
        {
            Given_RiscVInstructions(0x00008A61);    // c.andi\ta2,00000018
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 & 24<i64>");
        }

        [Test]
        public void RiscV_rw_c_beqz()
        {
            Given_RiscVInstructions(0x0000C121);    // c.beqz\ta0,0000000000100040
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a0 == 0<64>) branch 0000000000010040");
        }

        [Test]
        public void RiscV_rw_c_bnez()
        {
            Given_RiscVInstructions(0x0000EF09);    // c.bnez\ta4,000000000010001A
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0<64>) branch 000000000001001A");
        }

        [Test]
        public void RiscV_rw_c_bnez_backward()
        {
            Given_RiscVInstructions(0xFB05);    // c.bnez\ta4,00000000000FFF30
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0<64>) branch 000000000000FF30");
        }

        [Test]
        public void RiscV_rw_c_fld()
        {
            Given_RiscVInstructions(0x00002E64);    // c.fld\tfs1,216(a2)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fs1 = Mem0[a2 + 216<i64>:real64]");
        }

        [Test]
        public void RiscV_rw_c_fldsp()
        {
            Given_RiscVInstructions(0x00003436);    // c.fldsp\tfa0,sp,00000228
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fs0 = Mem0[sp + 360<i64>:real64]");
        }

        [Test]
        public void RiscV_rw_c_flw_32()
        {
            Given_32bitFloat();
            Given_HexString("C462");
            AssertCode(     // c.flw	fs1,4(a3)
                "0|L--|00010000(2): 1 instructions",
                "1|L--|fs1 = Mem0[a3 + 4<i32>:real32]");
        }

        [Test]
        public void RiscV_rw_c_fsd()
        {
            Given_RiscVInstructions(0x0000A604);    // c.fsd\tfs1,8(a2)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[a2 + 8<i64>:real64] = fs1");
        }

        [Test]
        public void RiscV_rw_c_fsdsp()
        {
            Given_RiscVInstructions(0xA7E6);        // c.fsdsp\tfs9,sp,000001C8
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 456<i64>:real64] = fs9");
        }

        [Test]
        public void RiscV_rw_c_fsw()
        {
            Given_32bitFloat();
            Given_HexString("00FC");
            AssertCode(     // c.fsw	s0,56(s0)
                "0|L--|00010000(2): 1 instructions",
                "1|L--|Mem0[s0 + 56<i32>:real32] = s0");
        }

        [Test]
        public void RiscV_rw_c_j()
        {
            Given_RiscVInstructions(0x0000B7D5);    // c.j\t00000000001003FC
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto 000000000000FFE4");
        }

        [Test]
        public void RiscV_rw_c_j_backward()
        {
            Given_RiscVInstructions(0x0000BF1D);    // c.j\t000000000000FF36
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto 000000000000FF36");
        }

        [Test]
        public void RiscV_rw_c_jal()
        {
            Given_32bitFloat();
            Given_HexString("912C");
            AssertCode(     // c.jal	00000000230822D4
                "0|T--|00010000(2): 1 instructions",
                "1|T--|call 00010254 (0)");
        }

        [Test]
        public void RiscV_rw_c_jr()
        {
            Given_RiscVInstructions(0x00008782);    // c.jr\ta5
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_c_ld()
        {
            Given_RiscVInstructions(0x00006568);    // c.ld\ta0,200(a0)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = Mem0[a0 + 200<i64>:word64]");
        }

        [Test]
        public void RiscV_rw_c_ld_64()
        {
            Given_HexString("C462");
            AssertCode(     // c.ld	s1,4(a3)
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s1 = Mem0[a3 + 128<i64>:word64]");
        }

        [Test]
        public void RiscV_rw_c_ldsp()
        {
            Given_RiscVInstructions(0x00006BA2);    // c.ldsp\ts7,00000008
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s7 = Mem0[sp + 8<i64>:word64]");
        }

        [Test]
        public void RiscV_rw_c_li()
        {
            Given_RiscVInstructions(0x00004521);    // c.li\ta0,00000008
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = 8<i64>");
        }

        [Test]
        public void RiscV_rw_c_li_minus3()
        {
            Given_RiscVInstructions(0x00005775);    // c.li\ta4,FFFFFFFFFFFFFFFD
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a4 = -3<i64>");
        }

        [Test]
        public void RiscV_rw_c_lui()
        {
            Given_RiscVInstructions(0x00006585);    // c.lui\ta1,00001000
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = 0x1000000<64>");
        }

        [Test]
        public void RiscV_rw_c_lw()
        {
            Given_RiscVInstructions(0x000043F4);    // c.lw\ta3,68(a5)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a3 = CONVERT(Mem0[a5 + 68<i64>:word32], word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_lwsp()
        {
            Given_RiscVInstructions(0x00004512);    // c.lwsp\ta0,sp,00000004
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = CONVERT(Mem0[sp + 4<i64>:word32], word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_mv()
        {
            Given_RiscVInstructions(0x844E);    // c.mv\ts0,s3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = s3");
        }

        [Test]
        public void RiscV_rw_c_nop()
        {
            Given_HexString("0100");
            AssertCode(     // c.nop
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void RiscV_rw_c_or()
        {
            Given_RiscVInstructions(0x8E55);    // c.or\ta2,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 | a3");
        }

        [Test]
        public void RiscV_rw_c_sdsp()
        {
            Given_RiscVInstructions(0xE4CE);    // c.sdsp\ts3,ps,00000048
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 72<i64>:word64] = s3");
        }

        [Test]
        public void RiscV_rw_c_slli()
        {
            Given_RiscVInstructions(0x0000040E);    // c.slli\ts0,03
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = s0 << 3<i32>");
        }

        [Test]
        public void RiscV_rw_c_slli64()
        {
            Given_RiscVInstructions(0x00000782);    // c.slli64\ta5
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void RiscV_rw_c_srai()
        {
            Given_RiscVInstructions(0x0000977D);    // c.srai\ta4,0000003F
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a4 = a4 >> 63<i32>");
        }

        [Test]
        public void RiscV_rw_c_srai64()
        {
            Given_RiscVInstructions(0x00008681);    // c.srai64\ta3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void RiscV_rw_c_srli()
        {
            Given_RiscVInstructions(0x000083A9);    // c.srli\ta5,0000000A
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 >>u 10<i32>");
        }

        [Test]
        public void RiscV_rw_c_srli64()
        {
            Given_RiscVInstructions(0x00008201);    // c.srli64\ta2
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void RiscV_rw_c_sub()
        {
            Given_RiscVInstructions(0x8D89);    // c.sub\ta1,a0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = a1 - a0");
        }

        [Test]
        public void RiscV_rw_c_subw()
        {
            Given_RiscVInstructions(0x00009D1D);    // c.subw\ta0,a5
            AssertCode(
                "0|L--|0000000000010000(2): 3 instructions",
                "1|L--|v4 = SLICE(a0, word32, 0)",
                "2|L--|v6 = SLICE(a5, word32, 0)",
                "3|L--|a0 = CONVERT(v4 - v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_sw()
        {
            Given_RiscVInstructions(0xC29C);    // c.sw\ta3,0(a5)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[a3:word32] = SLICE(a5, word32, 0)");
        }

        [Test]
        public void RiscV_rw_c_swsp()
        {
            Given_RiscVInstructions(0xC22A);    // c.swsp\ta0,00000080
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 4<i64>:word32] = SLICE(a0, word32, 0)");
        }

        [Test]
        public void RiscV_rw_csrrc()
        {
            Given_HexString("73B00230");
            AssertCode(     // csrrc	zero,mstatus,t0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrc<word64>(mstatus, t0)");
        }

        [Test]
        public void RiscV_rw_csrrci()
        {
            Given_HexString("F3770430");
            AssertCode(     // csrrci	a5,mstatus,00000008
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = __csrrc<word64>(mstatus, 8<64>)");
        }

        [Test]
        public void RiscV_rw_csrrs()
        {
            Given_HexString("73292000");
            AssertCode(     // csrrs	s2,frm,zero
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s2 = __csrrs<word64>(frm, 0<64>)");
        }

        [Test]
        public void RiscV_rw_csrrsi()
        {
            Given_HexString("73600430");
            AssertCode(     // csrrsi	zero,mstatus,00000008
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrs<word64>(mstatus, 8<64>)");
        }

        [Test]
        public void RiscV_rw_csrrw()
        {
            Given_HexString("73900930");
            AssertCode(     // csrrw	zero,mstatus,x19
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrw<word64>(mstatus, s3)");
        }

        [Test]
        public void RiscV_rw_csrrwi()
        {
            Given_HexString("73D02334");
            AssertCode(     // csrrwi	zero,mcause,00000007
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrw<word64>(mcause, 7<64>)");
        }

        [Test]
        public void RiscV_rw_csrw_unknown()
        {
            Given_HexString("7310C5BF");
            AssertCode(     // csrw\t0xbfc,a0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrw<word64>(0xBFC<u32>, a0)");
        }

        [Test]
        public void RiscV_rw_div()
        {
            Given_HexString("B3C5E502");
            AssertCode(     // div	a1,a1,a4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a1 = a1 / a4");
        }

        [Test]
        public void RiscV_rw_divu()
        {
            Given_RiscVInstructions(0x02B6D6B3);	// divu	a3,a3,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = a3 /u a1");
        }

        [Test]
        public void RiscV_rw_divuw()
        {
            Given_RiscVInstructions(0x02C857BB);    // divuw\ta5,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(a6 /u a2, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_divw()
        {
            Given_RiscVInstructions(0x02B4443B);    // divw\ts0,s0,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = CONVERT(SLICE(s0 / a1, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_ebreak()
        {
            Given_HexString("73001000");
            AssertCode(     // ebreak
                "0|H--|0000000000010000(4): 1 instructions",
                "1|L--|__ebreak()");
        }

        [Test]
        public void RiscV_rw_ecall()
        {
            Given_HexString("73000000");
            AssertCode(     // ecall
                "0|T--|0000000000010000(4): 1 instructions",
                "1|L--|__syscall()");
        }

        [Test]
        public void RiscV_rw_fabs_d()
        {
            Given_HexString("D3278422");
            AssertCode(     // fabs.d	fa5,fs0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = fabs(fs0)");
        }

        [Test]
        public void RiscV_rw_fadd_d()
        {
            Given_HexString("D3383102");
            AssertCode(     // fadd.d	fa7,ft2,ft3,rup
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa7 = ft2 + ft3");
        }

        [Test]
        public void RiscV_rw_fadd_h()
        {
            Given_HexString("53C60605");
            AssertCode(     // fadd.h	fa2,fa3,fa6,rmm
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa2 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(fa3, real16, 0) + SLICE(fa6, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fadd_q()
        {
            Given_128bitFloat();
            Given_HexString("D3832607");
            AssertCode(     // fadd.q	ft7,fa3,fs2,rne
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|ft7 = fa3 + fs2");
        }

        [Test]
        public void RiscV_rw_fadd_s()
        {
            Given_HexString("53495800");
            AssertCode(     // fadd.s	fs2,fa6,ft5,rmm
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs2 = SEQ(0xFFFFFFFF<32>, SLICE(fa6, real32, 0) + SLICE(ft5, real32, 0))");
        }

        [Test]
        public void RiscV_rw_fclass_d()
        {
            Given_HexString("539506E2");
            AssertCode(     // fclass.d	a0,fa3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = __fclass<real64,word64>(fa3)");
        }

        [Test]
        public void RiscV_rw_fclass_h()
        {
            Given_HexString("531607E4");
            AssertCode(     // fclass.h	a2,fa4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a2 = __fclass<real64,word64>(SLICE(fa4, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fclass_q()
        {
            Given_128bitFloat();
            Given_HexString("D31603E6");
            AssertCode(     // fclass.q	a3,ft6
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|a3 = __fclass<real64,word64>(ft6)");
        }

        [Test]
        public void RiscV_rw_fclass_s()
        {
            Given_HexString("D31606E0");
            AssertCode(     // fclass.s	a3,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = __fclass<real32,word64>(SLICE(fa2, real32, 0))");
        }

        [Test]
        public void RiscV_rw_fcvt_d_h()
        {
            Given_HexString("53872842");
            AssertCode(     // fcvt.d.h	fa4,fa7,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = CONVERT(SLICE(fa7, int16, 0), int16, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_d_l()
        {
            Given_HexString("D3F026D2");
            AssertCode(     // fcvt.d.l	ft1,a3,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft1 = CONVERT(a3, int64, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_d_lu()
        {
            Given_HexString("53F739D2");
            AssertCode(     // fcvt.d.lu	fa4,s3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = CONVERT(s3, uint64, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_d_q()
        {
            Given_128bitFloat();
            Given_HexString("D37B3B42");
            AssertCode(     // fcvt.d.q	fs7,fs6
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|fs7 = SEQ(0xFFFFFFFFFFFFFFFF<64>, CONVERT(fs6, real128, real64))");
        }

        [Test]
        public void RiscV_rw_fcvt_d_s()
        {
            Given_RiscVInstructions(0x42070753u);    // fcvt.d.s\tfa4,fa4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = CONVERT(SLICE(fa4, real32, 0), real32, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_d_w()
        {
            Given_HexString("538706D2");
            AssertCode(     // fcvt.d.w	fa4,a3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = CONVERT(SLICE(a3, int32, 0), int32, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_d_wu()
        {
            Given_HexString("538416D2");
            AssertCode(     // fcvt.d.wu	fs0,a3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs0 = CONVERT(SLICE(a3, uint32, 0), uint32, real64)");
        }

        [Test]
        public void RiscV_rw_fcvt_h_d()
        {
            Given_HexString("D3261744");
            AssertCode(     // fcvt.h.d	fa3,a4,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(fa4, real64, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_l()
        {
            Given_HexString("D32628D4");
            AssertCode(     // fcvt.h.l	fa3,a6,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(a6, int64, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_lu()
        {
            Given_HexString("53C638D4");
            AssertCode(     // fcvt.h.lu	fa2,a7,rmm
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa2 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(a7, uint64, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_q()
        {
            Given_128bitFloat();
            Given_HexString("D3283644");
            AssertCode(     // fcvt.h.q	fa7,a2,rdn
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|fa7 = SEQ(0x0FFFFFFFFFFFFFFFF<112>, CONVERT(fa2, real128, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_s()
        {
            Given_HexString("D3750744");
            AssertCode(     // fcvt.h.s	fa1,a4,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa1 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(SLICE(fa4, real32, 0), real32, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_w()
        {
            Given_HexString("D37607D4");
            AssertCode(     // fcvt.h.w	fa3,a4,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(SLICE(a4, int32, 0), int32, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_h_wu()
        {
            Given_HexString("53F815D4");
            AssertCode(     // fcvt.h.wu	fa6,a1,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa6 = SEQ(0xFFFFFFFFFFFF<48>, CONVERT(SLICE(a1, uint32, 0), uint32, real16))");
        }

        [Test]
        public void RiscV_rw_fcvt_l_d()
        {
            Given_HexString("D39020C2");
            AssertCode(     // fcvt.l.d	ra,ft1,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ra = CONVERT(ft1, real64, int64)");
        }

        [Test]
        public void RiscV_rw_fcvt_l_h()
        {
            Given_HexString("D31827C4");
            AssertCode(     // fcvt.l.h	a7,fa4,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = CONVERT(SLICE(fa4, real16, 0), real16, int64)");
        }

        [Test]
        public void RiscV_rw_fcvt_l_q()
        {
            Given_128bitFloat();
            Given_HexString("D32121C6");
            AssertCode(     // fcvt.l.q	gp,ft2,rdn
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|gp = CONVERT(ft2, real128, int64)");
        }

        [Test]
        public void RiscV_rw_fcvt_l_s()
        {
            Given_HexString("53952FC0");
            AssertCode(     // fcvt.l.s	a0,ft11,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(SLICE(ft11, real32, 0), real32, int64)");
        }

        [Test]
        public void RiscV_rw_fcvt_lu_d()
        {
            Given_HexString("531534C2");
            AssertCode(     // fcvt.lu.d	a0,fs0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(fs0, real64, uint64)");
        }

        [Test]
        public void RiscV_rw_fcvt_lu_h()
        {
            Given_HexString("D38537C4");
            AssertCode(     // fcvt.lu.h	a1,fa5,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a1 = CONVERT(SLICE(fa5, real16, 0), real16, uint64)");
        }

        [Test]
        public void RiscV_rw_fcvt_lu_s()
        {
            Given_HexString("D39437C0");
            AssertCode(     // fcvt.lu.s        s1,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s1 = CONVERT(SLICE(fa5, real32, 0), real32, uint64)");
        }

        [Test]
        public void RiscV_rw_fcvt_q_d()
        {
            Given_128bitFloat();
            Given_HexString("D3891A46");
            AssertCode(     // fcvt.q.d	fs3,fs5
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fs3 = CONVERT(SLICE(fs5, real64, 0), real64, real128)");
        }

        [Test]
        public void RiscV_rw_fcvt_q_h()
        {
            Given_128bitFloat();
            Given_HexString("D3082846");
            AssertCode(     // fcvt.q.h	fa7,fa6,rne
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fa7 = CONVERT(SLICE(fa6, real16, 0), real16, real128)");
        }

        [Test]
        public void RiscV_rw_fcvt_q_s()
        {
            Given_128bitFloat();
            Given_HexString("D30D0A46");
            AssertCode(     // fcvt.q.s	fs11,fs4
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fs11 = CONVERT(SLICE(fs4, int32, 0), int32, real128)");
        }

        [Test]
        public void RiscV_rw_fcvt_q_w()
        {
            Given_128bitFloat();
            Given_HexString("D30303D6");
            AssertCode(     // fcvt.q.w	ft7,t1,rne
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ft7 = CONVERT(SLICE(t1, int32, 0), int32, real128)");
        }

        [Test]
        public void RiscV_rw_fcvt_q_wu()
        {
            Given_128bitFloat();
            Given_HexString("D30212D6");
            AssertCode(     // fcvt.q.wu	ft5,tp,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft5 = CONVERT(SLICE(tp, uint32, 0), uint32, real128)");
        }

        [Test]
        public void RiscV_rw_fcvt_s_d()
        {
            Given_HexString("D3261C40");
            AssertCode(     // fcvt.s.d	fa3,fs8
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFF<32>, CONVERT(fs8, real64, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_s_h()
        {
            Given_HexString("53862740");
            AssertCode(     // fcvt.s.h	fa2,fa5,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa2 = SEQ(0xFFFFFFFF<32>, CONVERT(SLICE(fa5, real16, 0), real16, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_s_l()
        {
            Given_HexString("D3F328D0");
            AssertCode(     // fcvt.s.l	ft7,a7
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft7 = SEQ(0xFFFFFFFF<32>, CONVERT(a7, int64, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_s_lu()
        {
            Given_HexString("D3F734D0");
            AssertCode(     // fcvt.s.lu        fa5,s1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = SEQ(0xFFFFFFFF<32>, CONVERT(s1, uint64, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_s_q()
        {
            Given_128bitFloat();
            Given_HexString("53FD3C40");
            AssertCode(     // fcvt.s.q	fs10,fs9
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|fs10 = SEQ(0x0FFFFFFFFFFFFFFFF<96>, CONVERT(fs9, real128, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_s_w()
        {
            Given_32bitFloat();
            Given_HexString("537404D0");
            AssertCode(     // fcvt.s.w	s0,fs0
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs0 = CONVERT(s0, int32, real32)");
        }

        [Test]
        public void RiscV_rw_fcvt_s_wu()
        {
            Given_HexString("53F719D0");
            AssertCode(     // fcvt.s.wu	fs4,a3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFF<32>, CONVERT(SLICE(s3, uint32, 0), uint32, real32))");
        }

        [Test]
        public void RiscV_rw_fcvt_w_d()
        {
            Given_HexString("D31801C2");
            AssertCode(     // fcvt.w.d	a7,ft2,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = SEQ(0xFFFFFFFF<32>, CONVERT(ft2, real64, int32))");
        }

        [Test]
        public void RiscV_rw_fcvt_w_h()
        {
            Given_HexString("D3F805C4");
            AssertCode(     // fcvt.w.h	a7,fa1,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = SEQ(0xFFFFFFFF<32>, CONVERT(SLICE(fa1, real16, 0), real16, int32))");
        }

        [Test]
        public void RiscV_rw_fcvt_w_q()
        {
            Given_128bitFloat();
            Given_HexString("530001C6");
            AssertCode(     // fcvt.w.q	zero,ft2,rne
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|0<64> = SEQ(0xFFFFFFFF<32>, CONVERT(ft2, real128, int32))");
        }

        [Test]
        public void RiscV_rw_fcvt_w_s()
        {
            Given_32bitFloat();
            Given_HexString("D31704C0");
            AssertCode(     // fcvt.w.s	a5,fs0
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fs0, real32, int32)");
        }

        [Test]
        public void RiscV_rw_fcvt_wu_d()
        {
            Given_HexString("D39719C2");
            AssertCode(     // fcvt.wu.d	a5,fs3,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = SEQ(0xFFFFFFFF<32>, CONVERT(fs3, real64, uint32))");
        }

        [Test]
        public void RiscV_rw_fcvt_wu_h()
        {
            Given_HexString("53F716C4");
            AssertCode(     // fcvt.wu.h	a4,fa3,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = SEQ(0xFFFFFFFF<32>, CONVERT(SLICE(fa3, real16, 0), real16, uint32))");
        }

        [Test]
        public void RiscV_rw_fcvt_wu_q()
        {
            Given_128bitFloat();
            Given_HexString("D31013C6");
            AssertCode(     // fcvt.wu.q	ra,ft6,rtz
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|ra = SEQ(0xFFFFFFFF<32>, CONVERT(ft6, real128, uint32))");
        }

        [Test]
        public void RiscV_rw_fcvt_wu_s()
        {
            Given_32bitFloat();
            Given_HexString("539517C0");
            AssertCode(     // fcvt.wu.s	a0,fa5
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(fa5, real32, uint32)");
        }

        [Test]
        public void RiscV_rw_fdiv_d()
        {
            Given_HexString("D377F71A");
            AssertCode(     // fdiv.d	fa5,fa4,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = fa4 / fa5");
        }

        [Test]
        public void RiscV_rw_fdiv_h()
        {
            Given_HexString("5307181D");
            AssertCode(     // fdiv.h	fa4,fa6,fa7,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(fa6, real16, 0) / SLICE(fa7, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fdiv_q()
        {
            Given_128bitFloat();
            Given_HexString("53CDBC1F");
            AssertCode(     // fdiv.q	fs10,fs9,fs11,rmm
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|fs10 = fs9 / fs11");
        }

        [Test]
        public void RiscV_rw_fdiv_s()
        {
            Given_32bitFloat();
            Given_HexString("5374F418");
            AssertCode(     // fdiv.s	fs0,fs0,fa5
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs0 = fs0 / fa5");
        }

        [Test]
        public void RiscV_rw_fence()
        {
            Given_HexString("0F00F00F");
            AssertCode(     // fence	iorw,iorw
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__fence(iorw, iorw)");
        }

        [Test]
        public void RiscV_rw_fence_i()
        {
            Given_HexString("0F100000");
            AssertCode(     // fence.i
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__fence_i()");
        }

        [Test]
        public void RiscV_rw_fence_tso()
        {
            Given_HexString("0F003083");
            AssertCode(     // fence.tso
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__fence_tso()");
        }

        [Test]
        public void RiscV_rw_feq_d()
        {
            Given_HexString("D327D7A2");
            AssertCode(     // feq.d	a5,fa4,fa3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fa4 == fa3, bool, word64)");
        }

        [Test]
        public void RiscV_rw_feq_h()
        {
            Given_HexString("D3A7F5A4");
            AssertCode(     // feq.h	a5,fa1,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(fa1, real16, 0) == SLICE(fa5, real16, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_feq_q()
        {
            Given_128bitFloat();
            Given_HexString("D3A321A6");
            AssertCode(     // feq.q	t2,ft3,ft2
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|t2 = CONVERT(ft3 == ft2, bool, word64)");
        }

        [Test]
        public void RiscV_rw_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            Given_RiscVInstructions(0xA0F727D3u);    // feq.s\ta5,fa4,fa5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(fa4, real32, 0) == SLICE(fa5, real32, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_fld()
        {
            Given_HexString("07B6870F");
            AssertCode(     // fld	fa2,0xF8(a5)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa2 = Mem0[a5 + 248<i64>:real64]");
        }

        [Test]
        public void RiscV_rw_fle_d()
        {
            Given_HexString("D387E7A2");
            AssertCode(     // fle.d	a5,fa5,fa4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fa5 <= fa4, bool, word64)");
        }

        [Test]
        public void RiscV_rw_fle_h()
        {
            Given_HexString("D308E6A4");
            AssertCode(     // fle.h	a7,fa2,fa4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = CONVERT(SLICE(fa2, real16, 0) <= SLICE(fa4, real16, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_fle_q()
        {
            Given_128bitFloat();
            Given_HexString("530470A6");
            AssertCode(     // fle.q	s0,ft0,ft7
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|s0 = CONVERT(ft0 <= ft7, bool, word64)");
        }

        [Test]
        public void RiscV_rw_fle_s()
        {
            Given_HexString("D307F7A0");
            AssertCode(     // fle.s	a5,fa4,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(fa4, real32, 0) <= SLICE(fa5, real32, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_flh()
        {
            Given_HexString("0797C800");
            AssertCode(     // flh	fa4,0xC(a7)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFFFFFF<48>, Mem0[a7 + 12<i64>:real16])");
        }

        [Test]
        public void RiscV_rw_fli_d()
        {
            Given_RiscVInstructions(0xF21007D3u);   // fli.d\tfa5,-1.0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = -1.0F");
        }

        [Test]
        public void RiscV_rw_fli_h()
        {
            Given_HexString("D30F1CF4");
            AssertCode(     // fli.h	ft11,8.0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft11 = 8.0F");
        }

        [Test]
        public void RiscV_rw_fli_q()
        {
            Given_128bitFloat();
            Given_HexString("D30214F6");
            AssertCode(     // fli.q	ft5,0.25
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|ft5 = 0.25F");
        }

        [Test]
        public void RiscV_rw_fli_s()
        {
            Given_RiscVInstructions(0xF01587D3u);    // fli.s\tfa5,0.3125
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = 0.4375F");
        }

        [Test]
        public void RiscV_rw_flq()
        {
            Given_128bitFloat();
            Given_HexString("0740E391");
            AssertCode(     // flq	ft0,2334(t1)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft0 = Mem0[t1 + -1762<i64>:real128]");
        }

        [Test]
        public void RiscV_rw_flt_d()
        {
            Given_HexString("D317C7A2");
            AssertCode(     // flt.d	a5,fa4,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fa4 < fa2, bool, word64)");
        }

        [Test]
        public void RiscV_rw_flt_h()
        {
            Given_HexString("D39606A5");
            AssertCode(     // flt.h	a3,fa3,fa6
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = CONVERT(SLICE(fa3, real16, 0) < SLICE(fa6, real16, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_flt_s()
        {
            Given_32bitFloat();
            Given_HexString("D397E7A0");
            AssertCode(     // flt.s	a5,fa5,fa4
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fa5 < fa4, bool, word32)");
        }

        [Test]
        public void RiscV_rw_flw()
        {
            Given_RiscVInstructions(0x03492707u);    // flw\tfa4,s2,+00000034
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFF<32>, Mem0[s2 + 52<i64>:real32])");
        }

        [Test]
        public void RiscV_rw_fmadd_d()
        {
            Given_HexString("43F16303");
            AssertCode(     // fmadd.d	ft2,ft7,fs6,ft0,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft2 = ft7 * fs6 + ft0");
        }

        [Test]
        public void RiscV_rw_fmadd_h()
        {
            Given_HexString("C3250264");
            AssertCode(     // fmadd.h	fa1,ft4,ft0,fa2,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa1 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(ft4, real16, 0) * SLICE(ft0, real16, 0) + SLICE(fa2, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fmadd_q()
        {
            Given_128bitFloat();
            Given_HexString("432A8146");
            AssertCode(     // fmadd.q	fs4,ft2,fs0,fs0,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs4 = ft2 * fs0 + fs0");
        }

        [Test]
        public void RiscV_rw_fmadd_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x8093FD43);
            AssertCode(
                 "0|L--|00010000(4): 1 instructions",
                 "1|L--|fs10 = ft7 * fs1 + fa6");
        }

        [Test]
        public void RiscV_rw_fmax_d()
        {
            Given_HexString("5311402A");
            AssertCode(     // fmax.d	ft2,ft0,ft4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft2 = __fmax<real64>(ft0, ft4)");
        }

        [Test]
        public void RiscV_rw_fmax_h()
        {
            Given_HexString("D395C82C");
            AssertCode(     // fmax.h	fa1,fa7,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa1 = SEQ(0xFFFFFFFFFFFF<48>, __fmax<real16>(SLICE(fa7, real16, 0), SLICE(fa2, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fmax_s()
        {
            Given_HexString("D3122228");
            AssertCode(     // fmax.s	ft5,ft4,ft2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft5 = SEQ(0xFFFFFFFF<32>, __fmax<real32>(SLICE(ft4, real32, 0), SLICE(ft2, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fmaxm_d()
        {
            Given_HexString("5331402A");    // fmaxm.d\tft2,ft0,ft4"
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft2 = __fmaxm<real64>(ft0, ft4)");
        }

        [Test]
        public void RiscV_rw_fmaxm_h()
        {
            Given_HexString("D3B5C82C");
            AssertCode(     // fmaxm.h	fa1,fa7,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa1 = SEQ(0xFFFFFFFFFFFF<48>, __fmaxm<real16>(SLICE(fa7, real16, 0), SLICE(fa2, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fmaxm_s()
        {
            Given_HexString("D3322228");
            AssertCode(     // fmaxm.s	ft5,ft4,ft2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft5 = SEQ(0xFFFFFFFF<32>, __fmaxm<real32>(SLICE(ft4, real32, 0), SLICE(ft2, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fmin_d()
        {
            Given_HexString("D301102A");
            AssertCode(     // fmin.d	ft3,ft0,ft1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = __fmin<real64>(ft0, ft1)");
        }

        [Test]
        public void RiscV_rw_fmin_h()
        {
            Given_HexString("5387F62C");
            AssertCode(     // fmin.h	fa4,fa3,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFFFFFF<48>, __fmin<real16>(SLICE(fa3, real16, 0), SLICE(fa5, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fmin_s()
        {
            Given_HexString("53031028");
            AssertCode(     // fmin.s	ft6,ft0,ft1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft6 = SEQ(0xFFFFFFFF<32>, __fmin<real32>(SLICE(ft0, real32, 0), SLICE(ft1, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fminm_d()
        {
            Given_HexString("D321102A");
            AssertCode(     // fminm.d	ft3,ft0,ft1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = __fminm<real64>(ft0, ft1)");
        }

        [Test]
        public void RiscV_rw_fminm_h()
        {
            Given_HexString("53A7F62C");
            AssertCode(     // fminm.h	fa4,fa3,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFFFFFF<48>, __fminm<real16>(SLICE(fa3, real16, 0), SLICE(fa5, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fminm_s()
        {
            Given_HexString("53231028");        // fminm.s\tft6,ft0,ft1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft6 = SEQ(0xFFFFFFFF<32>, __fminm<real32>(SLICE(ft0, real32, 0), SLICE(ft1, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fmsub_d()
        {
            Given_HexString("47B29C63");
            AssertCode(     // fmsub.d	ft4,fs9,fs9,fa2,rup
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft4 = fs9 * fs9 - fa2");
        }

        [Test]
        public void RiscV_rw_fmsub_h()
        {
            Given_HexString("C7003355");
            AssertCode(     // fmsub.h	ft1,ft6,fs3,fa0,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft1 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(ft6, real16, 0) * SLICE(fs3, real16, 0) - SLICE(fa0, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fmsub_q()
        {
            Given_128bitFloat();
            Given_HexString("C706FD56");
            AssertCode(     // fmsub.q	fa3,fs10,fa5,fa0,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = fs10 * fa5 - fa0");
        }

        [Test]
        public void RiscV_rw_fmsub_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x6118B5C7);    // fmsub.s\tfa1,fa7,fa7,fa2
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fa1 = fa7 * fa7 - fa2");
        }

        [Test]
        public void RiscV_rw_fmul_d()
        {
            Given_HexString("5377D712");
            AssertCode(     // fmul.d	fa4,fa4,fa3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = fa4 * fa3");
        }

        [Test]
        public void RiscV_rw_fmul_h()
        {
            Given_HexString("53A6E714");
            AssertCode(     // fmul.h	fa2,fa5,fa4,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa2 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(fa5, real16, 0) * SLICE(fa4, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fmul_q()
        {
            Given_128bitFloat();
            Given_HexString("533e1417");
            AssertCode(     // fmul.q ft8,fs0,fa7,rup
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft8 = fs0 * fa7");
        }

        [Test]
        public void RiscV_rw_fmul_s()
        {
            Given_32bitFloat();
            Given_HexString("53749510");
            AssertCode(     // fmul.s	fs0,fa0,fs1
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs0 = fa0 * fs1");
        }

        [Test]
        public void RiscV_rw_fmv_d_x()
        {
            Given_RiscVInstructions(0xF2070753u);    // fmv.d.x\tfa4,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = a4");
        }

        [Test]
        public void RiscV_rw_fmv_h_x()
        {
            Given_HexString("530608F4");
            AssertCode(     // fmv.h.x	fa2,a6
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = SLICE(a6, int16, 0)",
                "2|L--|fa2 = SEQ(0xFFFFFFFFFFFF<48>, v5)");
        }

        [Test]
        public void RiscV_rw_fmv_s()
        {
            Given_HexString("53058420");
            AssertCode(     // fmv.s	fa0,fs0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa0 = fs0");
        }

        [Test]
        public void RiscV_rw_fmv_w_x()
        {
            Given_RiscVInstructions(0xF00007D3u);    // fmv.w.x\tfa5,zero
            AssertCode(
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v4 = SLICE(0<64>, int32, 0)",
                "2|L--|fa5 = SEQ(0xFFFFFFFF<32>, v4)");
        }

        [Test]
        public void RiscV_rw_fmv_x_d()
        {
            Given_RiscVInstructions(0xE2070753u);    // fmv.d.x\tfa4,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = fa4");
        }

        [Test]
        public void RiscV_rw_fmv_x_h()
        {
            Given_HexString("D38705E4");
            AssertCode(     // fmv.x.h	a5,fa1
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = SLICE(fa1, real16, 0)",
                "2|L--|a5 = SEQ(0xFFFFFFFFFFFF<48>, v5)");
        }

        [Test]
        public void RiscV_rw_fmv_x_w()
        {
            Given_32bitFloat();
            Given_HexString("538507E0");
            AssertCode(     // fmv.x.w	a0,fa5
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a0 = fa5");
        }

        [Test]
        public void RiscV_rw_fneg_d()
        {
            Given_HexString("5317E722");
            AssertCode(     // fneg.d	fa4,fa4,fa4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = -fa4");
        }

        [Test]
        public void RiscV_rw_fneg_s()
        {
            Given_32bitFloat();
            Given_HexString("D397F720");
            AssertCode(     // fsgnjn.s	fa5,fa5,fa5
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fa5 = -fa5");
        }

        [Test]
        public void RiscV_rw_fneg_s_64bit()
        {
            Given_HexString("D397F720");
            AssertCode(     // fsgnjn.s	fa5,fa5,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = SEQ(0xFFFFFFFF<32>, -SLICE(fa5, real32, 0))");
        }

        [Test]
        public void RiscV_rw_fnmadd_d()
        {
            Given_HexString("4FFF7D5A");
            AssertCode(     // fnmadd.d	ft10,fs11,ft7,fa1,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft10 = -(fs11 * ft7) - fa1");
        }

        [Test]
        public void RiscV_rw_fnmadd_h()
        {
            Given_HexString("4F81A285");
            AssertCode(     // fnmadd.h	ft2,ft5,fs10,fa6,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft2 = SEQ(0xFFFFFFFFFFFF<48>, -(SLICE(ft5, real16, 0) * SLICE(fs10, real16, 0)) - SLICE(fa6, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fnmadd_q()
        {
            Given_128bitFloat();
            Given_128bitFloat();
            Given_HexString("4FF39707");
            AssertCode(     // fnmadd.q	ft6,fa5,fs9,ft0,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft6 = -(fa5 * fs9) - ft0");
        }

        [Test]
        public void RiscV_rw_fnmadd_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x00B3FDCF);    // fnmadd.s\tfs11,ft7,fa1,ft0
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs11 = -(ft7 * fa1) - ft0");
        }

        [Test]
        public void RiscV_rw_fnmsub_d()
        {
            Given_HexString("4BA9AA8A");
            AssertCode(     // fnmsub.d	fs2,fs5,fa0,fa7,rdn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs2 = -(fs5 * fa0) + fa7");
        }

        [Test]
        public void RiscV_rw_fnmsub_h()
        {
            Given_HexString("4B00BE9D");
            AssertCode(     // fnmsub.h	ft0,ft8,fs11,fs3,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft0 = SEQ(0xFFFFFFFFFFFF<48>, -(SLICE(ft8, real16, 0) * SLICE(fs11, real16, 0)) + SLICE(fs3, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fnmsub_q()
        {
            Given_128bitFloat();
            Given_128bitFloat();
            Given_HexString("4B008947");
            AssertCode(     // fnmsub.q	ft0,fs2,fs8,fs0,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft0 = -(fs2 * fs8) + fs0");
        }

        [Test]
        public void RiscV_rw_fnmsub_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x4189004B);    // fnmsub.s\tft0,fs2,fs8,fs0
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|ft0 = -(fs2 * fs8) + fs0");
        }

        [Test]
        public void RiscV_rw_fround_d()
        {
            Given_HexString("D3014042");
            AssertCode(     // fround.d	ft3,ft0,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = __fround<real64>(ft0)");
        }

        [Test]
        public void RiscV_rw_fround_h()
        {
            Given_HexString("53434044");
            AssertCode(     // fround.h	ft6,ft0,rmm
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft6 = SEQ(0xFFFFFFFFFFFF<48>, __fround<real16>(SLICE(ft0, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fround_q()
        {
            Given_128bitFloat();
            Given_HexString("D3A34046");
            AssertCode(     // fround.q	ft7,ft1,rdn
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|ft7 = __fround<real128>(ft1)");
        }

        [Test]
        public void RiscV_rw_fround_s()
        {
            Given_HexString("D3924140");
            AssertCode(     // fround.s	ft5,ft3,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft5 = SEQ(0xFFFFFFFF<32>, __fround<real32>(SLICE(ft3, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_froundnx_d()
        {
            Given_HexString("D3115042");
            AssertCode(     // froundnx.d	ft3,ft0,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = __froundnx<real64>(ft0)");
        }

        [Test]
        public void RiscV_rw_froundnx_h()
        {
            Given_HexString("53735044");
            AssertCode(     // froundnx.h	ft6,ft0,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft6 = SEQ(0xFFFFFFFFFFFF<48>, __froundnx<real16>(SLICE(ft0, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_froundnx_q()
        {
            Given_128bitFloat();
            Given_HexString("D3C35046");
            AssertCode(     // froundnx.q	ft7,ft1,rmm
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|ft7 = __froundnx<real128>(ft1)");
        }

        [Test]
        public void RiscV_rw_froundnx_s()
        {
            Given_HexString("D3B25140");
            AssertCode(     // froundnx.s	ft5,ft3,rup
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft5 = SEQ(0xFFFFFFFF<32>, __froundnx<real32>(SLICE(ft3, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fsd()
        {
            Given_RiscVInstructions(0x639435A7);    // fsd	fs9,12632(s0)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[s0 + 1579<i64>:real64] = fs9");
        }

        [Test]
        public void RiscV_rw_fsgnj_d()
        {
            Given_HexString("53822523");
            AssertCode(     // fsgnj.d	ft4,fa1,fs2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft4 = __fsgnj<real64>(fa1, fs2)");
        }

        [Test]
        public void RiscV_rw_fsgnj_h()
        {
            Given_HexString("D307B624");
            AssertCode(     // fsgnj.h	fa5,fa2,fa1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = SEQ(0xFFFFFFFFFFFF<48>, __fsgnj<real16>(SLICE(fa2, real16, 0), SLICE(fa1, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fsgnj_s()
        {
            Given_HexString("53802020");
            AssertCode(     // fsgnj.s	ft0,ft1,ft2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft0 = SEQ(0xFFFFFFFF<32>, __fsgnj<real32>(SLICE(ft1, real32, 0), SLICE(ft2, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fsgnjn_d()
        {
            Given_HexString("D391C422");
            AssertCode(     // fsgnjn.d	ft3,fs1,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = __fsgnjn<real64>(fs1, fa2)");
        }

        [Test]
        public void RiscV_rw_fsgnjn_h()
        {
            Given_HexString("53971725");
            AssertCode(     // fsgnjn.h	fa4,fa5,fa7
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = SEQ(0xFFFFFFFFFFFF<48>, __fsgnjn<real16>(SLICE(fa5, real16, 0), SLICE(fa7, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fsgnjn_s()
        {
            Given_HexString("D3914320");
            AssertCode(     // fsgnjn.s	ft3,ft7,ft4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = SEQ(0xFFFFFFFF<32>, __fsgnjn<real32>(SLICE(ft7, real32, 0), SLICE(ft4, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fsgnjx_d()
        {
            Given_HexString("53A41123");
            AssertCode(     // fsgnjx.d	fs0,ft3,fa7
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs0 = __fsgnjx<real64>(ft3, fa7)");
        }

        [Test]
        public void RiscV_rw_fsgnjx_h()
        {
            Given_HexString("D326C724");
            AssertCode(     // fsgnjx.h	fa3,fa4,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFFFFFF<48>, __fsgnjx<real16>(SLICE(fa4, real16, 0), SLICE(fa2, real16, 0)))");
        }

        [Test]
        public void RiscV_rw_fsgnjx_s()
        {
            Given_HexString("532E5120");
            AssertCode(     // fsgnjx.s	ft8,ft2,ft5
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ft8 = SEQ(0xFFFFFFFF<32>, __fsgnjx<real32>(SLICE(ft2, real32, 0), SLICE(ft5, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fsh()
        {
            Given_HexString("A79AD700");
            AssertCode(     // fsh	fa3,0x15(a5)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[a5 + 21<i64>:real16] = SLICE(fa3, real16, 0)");
        }

        [Test]
        public void RiscV_rw_fsq()
        {
            Given_128bitFloat();
            Given_HexString("A7CF7955");
            AssertCode(     // fsq	fs7,0x55F(s3)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[s3 + 1375<i64>:real128] = fs7");
        }

        [Test]
        public void RiscV_rw_fsqrt_d()
        {
            Given_HexString("5377065A");
            AssertCode(     // fsqrt.d	fa4,fa2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = sqrt(fa2)");
        }

        [Test]
        public void RiscV_rw_fsqrt_h()
        {
            Given_HexString("D306085C");
            AssertCode(     // fsqrt.h	fa3,fa6,rne
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = sqrt(SLICE(fa6, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fsqrt_q()
        {
            Given_128bitFloat();
            Given_HexString("53FD0D5E");
            AssertCode(     // fsqrt.q	fs10,fs11
                    "0|L--|0000000000010000(4): 1 instructions",
                    "1|L--|fs10 = SEQ(0xFFFFFFFFFFFFFFFF<64>, sqrt(fs11))");
        }

        [Test]
        public void RiscV_rw_fsqrt_s()
        {
            Given_HexString("D3710058");
            AssertCode(     // fsqrt.s	ft3,ft0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = SEQ(0xFFFFFFFF<32>, sqrtf(SLICE(ft0, real32, 0)))");
        }

        [Test]
        public void RiscV_rw_fsub_d()
        {
            Given_HexString("D371100A");
            AssertCode(     // fsub.d	ft3,ft0,ft1,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft3 = ft0 - ft1");
        }

        [Test]
        public void RiscV_rw_fsub_h()
        {
            Given_HexString("D376070D");
            AssertCode(     // fsub.h	fa3,fa4,fa6,dyn
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa3 = SEQ(0xFFFFFFFFFFFF<48>, SLICE(fa4, real16, 0) - SLICE(fa6, real16, 0))");
        }

        [Test]
        public void RiscV_rw_fsub_q()
        {
            Given_128bitFloat();
            Given_HexString("531EEC0F");
            AssertCode(     // fsub.q	ft8,fs8,ft10,rtz
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft8 = fs8 - ft10");
        }

        [Test]
        public void RiscV_rw_fsub_s()
        {
            Given_32bitFloat();
            Given_HexString("5375A708");
            AssertCode(     // fsub.s	fa0,fa4,fa0
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fa0 = fa4 - fa0");
        }

        [Test]
        public void RiscV_rw_fsw()
        {
            Given_RiscVInstructions(0x8963A3A7);	// fsw	fs6,8732(a5)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[t2 + -1913<i64>:real32] = SLICE(fs6, real32, 0)");
        }

        [Test]
        public void RiscV_rw_hfence_gvma()
        {
            Given_HexString("73000062");
            AssertCode(     // hfence.gvma	zero,(zero)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hfence_gvma<word64,word64>(0<64>, 0<64>)");
        }

        [Test]
        public void RiscV_rw_hfence_vvma()
        {
            Given_HexString("73000022");
            AssertCode(     // hfence.vvma	zero,(zero)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hfence_vvma<word64,word64>(0<64>, 0<64>)");
        }

        [Test]
        public void RiscV_rw_hinval_gvma()
        {
            Given_HexString("7380B866");
            AssertCode(     // hinval.gvma a7,a1
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hinval_gvma<word64,word64>(a7, a1)");
        }

        [Test]
        public void RiscV_rw_hinval_vvma()
        {
            Given_HexString("7380C626");
            AssertCode(     // hinval.vvma	a2,(a3)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hinval_vvma<word64,word64>(a3, a2)");
        }

        [Test]
        public void RiscV_rw_hlv_b()
        {
            Given_HexString("F3C80760");
            AssertCode(     // hlv.b	a7,(a5)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = __hypervisor_load_from_VM<int8>(&Mem0[a5:int8])");
        }

        [Test]
        public void RiscV_rw_hlv_bu()
        {
            Given_HexString("73481660");
            AssertCode(     // hlv.bu	a6,(a2)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a6 = __hypervisor_load_from_VM<byte>(&Mem0[a2:uint8])");
        }

        [Test]
        public void RiscV_rw_hlv_d()
        {
            Given_HexString("F3C8036C");
            AssertCode(     // hlv.d	a7,(t2)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = __hypervisor_load_from_VM<word64>(&Mem0[t2:uint64])");
        }

        [Test]
        public void RiscV_rw_hlv_h()
        {
            Given_HexString("F3C60664");
            AssertCode(     // hlv.h	a3,(a3)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = __hypervisor_load_from_VM<int16>(&Mem0[a3:int16])");
        }

        [Test]
        public void RiscV_rw_hlv_hu()
        {
            Given_HexString("73411764");
            AssertCode(     // hlv.hu	sp,(a4)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|sp = __hypervisor_load_from_VM<word16>(&Mem0[a4:uint16])");
        }

        [Test]
        public void RiscV_rw_hlv_w()
        {
            Given_HexString("73C80468");
            AssertCode(     // hlv.w	a6,(s1)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a6 = __hypervisor_load_from_VM<int32>(&Mem0[s1:int32])");
        }

        [Test]
        public void RiscV_rw_hlv_wu()
        {
            Given_HexString("73CE1868");
            AssertCode(     // hlv.wu	t3,(a7)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|t3 = __hypervisor_load_from_VM<word32>(&Mem0[a7:uint32])");
        }

        [Test]
        public void RiscV_rw_hlvx_hu()
        {
            Given_HexString("F3483164");
            AssertCode(     // hlvx.hu	a7,(sp)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|a7 = __hypervisor_load_exe_from_VM<word16>(&Mem0[sp:uint16])");
        }

        [Test]
        public void RiscV_rw_hlvx_wu()
        {
            Given_HexString("F3C13E68");
            AssertCode(     // hlvx.wu	gp,(t4)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|gp = __hypervisor_load_exe_from_VM<word32>(&Mem0[t4:uint32])");
        }

        [Test]
        public void RiscV_rw_hsv_b()
        {
            Given_HexString("7340C663");
            AssertCode(     // hsv.b	(a2),t3
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hypervisor_store_in_VM<byte>(&t3, Mem0[a2:byte])");
        }

        [Test]
        public void RiscV_rw_hsv_d()
        {
            Given_HexString("73C0416E");
            AssertCode(     // hsv.d	(gp),tp
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hypervisor_store_in_VM<word64>(&tp, Mem0[gp:word64])");
        }

        [Test]
        public void RiscV_rw_hsv_h()
        {
            Given_HexString("73C0CF66");
            AssertCode(     // hsv.h	(t6),a2
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hypervisor_store_in_VM<word16>(&a2, Mem0[t6:word16])");
        }

        [Test]
        public void RiscV_rw_hsv_w()
        {
            Given_HexString("7340DE6B");
            AssertCode(     // hsv.w	(t3),t4
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__hypervisor_store_in_VM<word32>(&t4, Mem0[t3:word32])");
        }

        [Test]
        public void RiscV_rw_invalid()
        {
            Given_RiscVInstructions(0x00000000);    // invalid
            AssertCode(
                "0|---|0000000000010000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void RiscV_rw_jal()
        {
            Given_RiscVInstructions(0x9F4FF06F);    // jal\tzero,00000000000FF1F4
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_jal_not_zero()
        {
            Given_RiscVInstructions(0x9F4FF0EFu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000000F1F4 (0)");
        }

        [Test]
        public void RiscV_rw_jal_ra()
        {
            Given_RiscVInstructions(0x02C000EF);    // jal ra,0000B6A4
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000001002C (0)");
        }

        [Test]
        public void RiscV_rw_jal_zero()
        {
            Given_RiscVInstructions(0x9F4FF06Fu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_jalr()
        {
            Given_RiscVInstructions(0x00078067u);    // jalr\tzero,a5,+00000000
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_jalr_ra()
        {
            Given_RiscVInstructions(0x003780E7);    // jalr ra,a5,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call a5 + 3<i64> (0)");
        }

        [Test]
        public void RiscV_rw_jalr_zero()
        {
            Given_RiscVInstructions(0x00078067); // jalr zero, a5, 0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_jalr_zero_ra()
        {
            Given_RiscVInstructions(0x00008067); // jalr zero,ra,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_jr_ra()
        {
            Given_RiscVInstructions(0x8082);    // c.jr\tra
            AssertCode(
                "0|R--|0000000000010000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_lb()
        {
            Given_RiscVInstructions(0x87010183u);
            AssertCode(
               "0|L--|0000000000010000(4): 1 instructions",
               "1|L--|gp = CONVERT(Mem0[sp + -1936<i64>:int8], int8, int64)");
        }

        [Test]
        public void RiscV_rw_lbu()
        {
            Given_RiscVInstructions(0x00094703u);    // lbu\ta4,s2,+00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = CONVERT(Mem0[s2:byte], byte, word64)");
        }

        [Test]
        public void RiscV_rw_ld()
        {
            Given_HexString("833F850F");
            AssertCode(     // ld	t6,0xF8(a0)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t6 = Mem0[a0 + 248<i64>:word64]");
        }

        [Test]
        public void RiscV_rw_lh()
        {
            Given_RiscVInstructions(0x03131083u);   // lh
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ra = CONVERT(Mem0[t1 + 49<i64>:int16], int16, int64)");
        }

        [Test]
        public void RiscV_rw_lhu()
        {
            Given_HexString("835D4162");
            AssertCode(     // lhu	s11,0x624(sp)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = CONVERT(Mem0[sp + 1572<i64>:uint16], uint16, word64)");
        }

        [Test]
        public void RiscV_rw_li()
        {
            Given_RiscVInstructions(0x00004385);    // c.li\tt2,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|t2 = 1<i64>");
        }

        [Test]
        public void RiscV_rw_lr_d()
        {
            Given_HexString("2FB10114");
            AssertCode(     // lr.d.aq	sp,gp
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|sp = __load_reserved<word64>(&Mem0[gp:word64])");
        }

        [Test]
        public void RiscV_rw_lr_w()
        {
            Given_HexString("AFAB1715");
            AssertCode(     // lr.w.aq	s7,a5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s7 = __load_reserved<word64>(&Mem0[a5:word32])");
        }

        [Test]
        public void RiscV_rw_lui()
        {
            Given_RiscVInstructions(0x000114B7u);   // lui s1,0x00000011<32>
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|s1 = 0x11000<64>");
        }

        [Test]
        public void RiscV_rw_lw()
        {
            Given_HexString("03AA897E");
            AssertCode(     // lw	s4,0x7E8(s3)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s4 = CONVERT(Mem0[s3 + 2024<i64>:int32], int32, int64)");
        }

        [Test]
        public void RiscV_rw_lwu()
        {
            Given_RiscVInstructions(0x00446703u);    // lwu\ta4,s0,+00000004
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = CONVERT(Mem0[s0 + 4<i64>:uint32], uint32, word64)");
        }

        [Test]
        public void RiscV_rw_mret()
        {
            Given_HexString("73002030");
            AssertCode(     // mret
                "0|R--|0000000000010000(4): 2 instructions",
                "1|L--|__mret()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_mul()
        {
            Given_HexString("B387D702");
            AssertCode(     // mul	a5,a5,a3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = a5 * a3");
        }

        [Test]
        public void RiscV_rw_mulh()
        {
            Given_HexString("B397E702");
            AssertCode(     // mulh	a5,a5,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = a5 *s128 a4",
                "2|L--|a5 = SLICE(v5, word64, 64)");
        }

        [Test]
        public void RiscV_rw_mulhsu()
        {
            Given_HexString("332E0103");
            AssertCode(     // mulhsu	t3,sp,a6
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = sp *s128 a6",
                "2|L--|t3 = SLICE(v6, word64, 64)");
        }

        [Test]
        public void RiscV_rw_mulhu()
        {
            Given_HexString("33B7C502");
            AssertCode(     // mulhu	a4,a1,a2
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = a1 *u128 a2",
                "2|L--|a4 = SLICE(v6, word64, 64)");
        }

        [Test]
        public void RiscV_rw_mulw()
        {
            Given_HexString("BB8E2802");
            AssertCode(     // mulw	t4,a7,sp
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t4 = CONVERT(SLICE(a7 * sp, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_or()
        {
            Given_RiscVInstructions(0x01846433u);    // or\ts0,s0,s8
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = s0 | s8");
        }

        [Test]
        public void RiscV_rw_ori()
        {
            Given_HexString("13670B40");
            AssertCode(     // ori	a4,s6,0x400
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = s6 | 1024<i64>");
        }

        [Test]
        public void RiscV_rw_pause()
        {
            Given_HexString("0F000001");
            AssertCode(     // pause
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__pause()");
        }

        [Test]
        public void RiscV_rw_rem()
        {
            Given_HexString("33E7E402");
            AssertCode(     // rem	a4,s1,a4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = s1 %s a4");
        }

        [Test]
        public void RiscV_rw_remu()
        {
            Given_HexString("3377C702");
            AssertCode(     // remu	a4,a4,a2
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = a4 %u a2");
        }

        [Test]
        public void RiscV_rw_remuw()
        {
            Given_RiscVInstructions(0x02C8783B);    // remuw\ta6,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a6 = CONVERT(SLICE(a6 %u a2, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_remw()
        {
            Given_RiscVInstructions(0x02D7E6BB);    // remw\ta3,a5,a3
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = CONVERT(SLICE(a5 %s a3, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_sb()
        {
            Given_HexString("A30AF7D2");
            AssertCode(     // sb	a5,-0x2CB(a4)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[a4 + -715<i64>:byte] = SLICE(a5, byte, 0)");
        }

        [Test]
        public void RiscV_rw_sc_d()
        {
            Given_HexString("AFBA3C1B");
            AssertCode(     // sc.d.rl	s5,s9,s3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s5 = __store_conditional<word64>(s3, &Mem0[s9:word64])");
        }

        [Test]
        public void RiscV_rw_sc_w()
        {
            Given_HexString("AFADE31C");
            AssertCode(     // sc.w.aq	s11,t2,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __store_conditional<word32>(a4, &Mem0[t2:word32])",
                "2|L--|s11 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sd()
        {
            Given_RiscVInstructions(0x19513423u);    // sd\ts5,sp,392
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[sp + 392<i64>:word64] = s5");
        }

        [Test]
        public void RiscV_rw_sfence_inval_ir()
        {
            Given_HexString("73001018");
            AssertCode(     // sfence.inval.ir
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sfence_inval_ir()");
        }

        [Test]
        public void RiscV_rw_sfence_vm()
        {
            Given_HexString("73804610");
            AssertCode(     // sfence.vm	a3
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sfence_vm<word64>(a3)");
        }

        [Test]
        public void RiscV_rw_sfence_vm_zero()
        {
            Given_HexString("73004010");
            AssertCode( // sfence.vm	zero
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sfence_vm<word64>(0<64>)");
        }

        [Test]
        public void RiscV_rw_sfence_vma()
        {
            Given_HexString("73000012");
            AssertCode(     // sfence.vma	zero,(zero)
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sfence_vma<word64,word64>(0<64>, 0<64>)");
        }

        [Test]
        public void RiscV_rw_sfence_w_inval()
        {
            Given_HexString("73000018");
            AssertCode(     // sfence.w.inval
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sfence_w_inval()");
        }

        [Test]
        public void RiscV_rw_sh()
        {
            Given_HexString("2396A47E");
            AssertCode(     // sh	a0,0x7EC(s1)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[s1 + 2028<i64>:word16] = SLICE(a0, word16, 0)");
        }

        [Test]
        public void RiscV_rw_sinval_vma()
        {
            Given_HexString("73800717");
            AssertCode(     // sinval.vma	a5,a6
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__sinval_vma<word64,word64>(a5, a6)");
        }

        [Test]
        public void RiscV_rw_sll()
        {
            Given_HexString("B39D6B00");
            AssertCode(     // sll	s11,s7,t1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = s7 << t1");
        }

        [Test]
        public void RiscV_rw_slli()
        {
            Given_HexString("13150B01");
            AssertCode(     // slli	a0,s6,0x10
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = s6 << 0x10<u32>");
        }

        [Test]
        public void RiscV_rw_slliw()
        {
            Given_HexString("1B9FF001");
            AssertCode(     // slliw	t5,ra,0x1F
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t5 = CONVERT(SLICE(ra, word32, 0) << 0x1F<64>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sllw()
        {
            Given_HexString("3B9F2000");
            AssertCode(     // sllw	t5,ra,sp
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t5 = CONVERT(SLICE(ra, word32, 0) << sp, word32, int64)");
        }

        [Test]
        public void RiscV_rw_slt()
        {
            Given_RiscVInstructions(0x00A7A533);    // slt\ta0,a5,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(a5 < a0, bool, word64)");
        }

        [Test]
        public void RiscV_rw_slti()
        {
            Given_HexString("93A72700");
            AssertCode(     // slti	a5,a5,+00000002
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(a5 < 2<i64>, bool, word64)");
        }

        [Test]
        public void RiscV_rw_sltiu()
        {
            Given_RiscVInstructions(0x0014B493);	// sltiu	s1,s1,+00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s1 = CONVERT(s1 <u 1<i64>, bool, word64)");
        }

        [Test]
        public void RiscV_rw_sltu()
        {
            Given_RiscVInstructions(0x00A03533);    // sltu\ta0,zero,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(a0 != 0<64>, bool, word64)");
        }

        [Test]
        public void RiscV_rw_sra()
        {
            Given_HexString("3355B540");
            AssertCode(     // sra	a0,a0,a1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = a0 >> a1");
        }

        [Test]
        public void RiscV_rw_srai()
        {
            Given_HexString("13DCFD41");
            AssertCode(     // srai	s8,s11,0x1F
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s8 = s11 >> 0x1F<u32>");
        }

        [Test]
        public void RiscV_rw_sraiw()
        {
            Given_HexString("1BD0C041");
            AssertCode(     // sraiw	zero,ra,0x1C
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|0<64> = CONVERT(SLICE(ra, word32, 0) >> 0x1C<u32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sraw()
        {
            Given_HexString("BB 5B D5 41");
            AssertCode(     // sraw\ts7,a0,t4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s7 = CONVERT(SLICE(a0, word32, 0) >> t4, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sret()
        {
            Given_HexString("73002010");
            AssertCode(     // sret
                "0|R--|0000000000010000(4): 2 instructions",
                "1|L--|__sret()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_srl()
        {
            Given_RiscVInstructions(0x00B6D6B3);	// srl	a3,a3,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = a3 >>u a1");
        }

        [Test]
        public void RiscV_rw_srli()
        {
            Given_HexString("13DC7C01");
            AssertCode(     // srli	s8,s9,0x17
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s8 = s9 >>u 0x17<u32>");
        }

        [Test]
        public void RiscV_rw_srliw()
        {
            Given_RiscVInstructions(0x0017D71Bu);    // srliw\ta4,a5,00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = CONVERT(SLICE(a5, word32, 0) >>u 1<i32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_srlw()
        {
            Given_HexString("3BD6F600");
            AssertCode(     // srlw     a2,a3,a5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a2 = CONVERT(SLICE(a3, word32, 0) >>u a5, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sub()
        {
            Given_HexString("B38D6B40");
            AssertCode(     // sub	s11,s7,t1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = s7 - t1");
        }

        [Test]
        public void RiscV_rw_subw()
        {
            Given_RiscVInstructions(0x40F686BBu);    // subw\ta3,a3,a5
            AssertCode(
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v4 = SLICE(a3, word32, 0)",
                "2|L--|v6 = SLICE(a5, word32, 0)",
                "3|L--|a3 = CONVERT(v4 - v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_sw()
        {
            Given_HexString("2320217F");
            AssertCode(     // sw	s2,0x7E0(sp)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[sp + 2016<i64>:word32] = SLICE(s2, word32, 0)");
        }
        [Test]
        public void RiscV_rw_wfi()
        {
            Given_HexString("73005010");
            AssertCode(     // wfi
                "0|S--|0000000000010000(4): 1 instructions",
                "1|L--|__wait_for_interrupt()");
        }

        [Test]
        public void RiscV_rw_xor()
        {
            Given_HexString("33CFCF01");
            AssertCode(     // xor	t5,t6,t3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t5 = t6 ^ t3");
        }

        [Test]
        public void RiscV_rw_xori()
        {
            Given_HexString("1344D33F");
            AssertCode(     // xori	s0,t1,0x3FD
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = t1 ^ 1021<i64>");
        }



        [Test]
        public void RiscVRw_add_uw()
        {
            Given_HexString("BB894309");
            AssertCode(    // add.uw\ts3,t2,s4;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v6 = SLICE(s4, word32, 0)",
                "2|L--|v7 = CONVERT(v6, word32, word64)",
                "3|L--|s3 = t2 + v7");
        }

        [Test]
        public void RiscVRw_andn()
        {
            Given_RiscVInstructions(0b0100000_11011_10001_111_01110_0110011);
            AssertCode(    // andn\ta4,a7,s11;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = a7 & ~s11");
        }

        [Test]
        public void RiscVRw_bclr()
        {
            Given_RiscVInstructions(0b0100100_11011_10001_001_01110_0110011);
            AssertCode(    // bclr\ta4,a7,s11;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = __clear_bit<word64,word64>(a7, s11)");
        }

        [Test]
        public void RiscVRw_bclri32()
        {
            Given_RiscVInstructions(0b0100100_10110_10001_001_11011_0010011);
            AssertCode(    // bclri\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __clear_bit<word64,uint32>(a7, 0x16<u32>)");
        }

        [Test]
        public void RiscVRw_bclri64()
        {
            Given_RiscVInstructions(0b010010_110110_10001_001_11011_0010011);
            AssertCode(    // bclri\ts11,a7,0x36;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __clear_bit<word64,uint32>(a7, 0x36<u32>)");
        }

        [Test]
        public void RiscVRw_bext()
        {
            Given_RiscVInstructions(0b0100100_10010_10001_101_11011_0110011);
            AssertCode(    // bext\ts11,a7,s2;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __bit<word64,word64>(a7, s2)",
                "2|L--|s11 = CONVERT(v6, bool, word64)");
        }

        [Test]
        public void RiscVRw_bexti32()
        {
            Given_RiscVInstructions(0b0100100_10110_10001_101_11011_0010011);
            AssertCode(    // bexti\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = __bit<word64,uint32>(a7, 0x16<u32>)",
                "2|L--|s11 = CONVERT(v5, bool, word64)");
        }

        [Test]
        public void RiscVRw_bexti64()
        {
            Given_RiscVInstructions(0b010010_110110_10001_101_11011_0010011);
            AssertCode(    // bexti\ts11,a7,0x36;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = __bit<word64,uint32>(a7, 0x36<u32>)",
                "2|L--|s11 = CONVERT(v5, bool, word64)");
        }

        [Test]
        public void RiscVRw_binv()
        {
            Given_RiscVInstructions(0b0110100_10010_10001_001_11011_0110011);
            AssertCode(    // binv\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __invert_bit<word64,word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_binvi32()
        {
            Given_RiscVInstructions(0b0110100_10110_10001_001_11011_0010011);
            AssertCode(    // binvi\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __invert_bit<word64,uint32>(a7, 0x16<u32>)");
        }

        [Test]
        public void RiscVRw_binvi64()
        {
            Given_RiscVInstructions(0b011010_110110_10001_001_11011_0010011);
            AssertCode(    // binvi\ts11,a7,0x36;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __invert_bit<word64,uint32>(a7, 0x36<u32>)");
        }

        [Test]
        public void RiscVRw_bset()
        {
            Given_RiscVInstructions(0b0010100_10010_10001_001_11011_0110011);
            AssertCode(    // bset\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __set_bit<word64,word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_bseti32()
        {
            Given_RiscVInstructions(0b0010100_10110_10001_001_11011_0010011);
            AssertCode(    // bseti\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __set_bit<word64,uint32>(a7, 0x16<u32>)");
        }

        [Test]
        public void RiscVRw_bseti64()
        {
            Given_RiscVInstructions(0b001010_110110_10001_001_11011_0010011);
            AssertCode(    // bseti\ts11,a7,0x36;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __set_bit<word64,uint32>(a7, 0x36<u32>)");
        }

        [Test]
        public void RiscVRw_clmul()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_001_11011_0110011);
            AssertCode(    // clmul\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __clmul<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_clmulh()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_011_11011_0110011);
            AssertCode(    // clmulh\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __clmulh<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_clmulr()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_010_11011_0110011);
            AssertCode(    // clmulr\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __clmulr<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_clz()
        {
            Given_RiscVInstructions(0b011000000000_10001_001_11011_0010011);
            AssertCode(    // clz\ts11,a7;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __count_leading_zeros<word64>(a7)");
        }

        [Test]
        public void RiscVRw_clzw()
        {
            Given_RiscVInstructions(0b011000000000_10001_001_11011_0011011);
            AssertCode(    // clzw\ts11,a7;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v5 = __count_leading_zeros<word32>(v3)",
                "3|L--|s11 = CONVERT(v5, word64, word64)");
        }

        [Test]
        public void RiscVRw_cpop()
        {
            Given_RiscVInstructions(0b011000000010_10001_001_11011_0010011);
            AssertCode(    // cpop\ts11,a7;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __population_count<word64>(a7)");
        }

        [Test]
        public void RiscVRw_cpopw()
        {
            Given_RiscVInstructions(0b011000000010_10001_001_11011_0011011);
            AssertCode(    // cpopw\ts11,a7;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v5 = __count_leading_zeros<word32>(v3)",
                "3|L--|s11 = CONVERT(v5, word64, word64)");
        }

        [Test]
        public void RiscVRw_ctz()
        {
            Given_RiscVInstructions(0b011000000001_10001_001_11011_0010011);
            AssertCode(    // ctz\ts11,a7;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __count_trailing_zeros<word64>(a7)");
        }

        [Test]
        public void RiscVRw_ctzw()
        {
            Given_RiscVInstructions(0b011000000001_10001_001_11011_0011011);
            AssertCode(    // ctzw\ts11,a7;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v5 = __count_leading_zeros<word32>(v3)",
                "3|L--|s11 = CONVERT(v5, word64, word64)");
        }

        [Test]
        public void RiscVRw_max()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_110_11011_0110011);
            AssertCode(    // max\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = max<int64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_maxu()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_111_11011_0110011);
            AssertCode(    // maxu\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = max<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_min()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_100_11011_0110011);
            AssertCode(    // min\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = min<int64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_minu()
        {
            Given_RiscVInstructions(0b0000101_10010_10001_101_11011_0110011);
            AssertCode(    // minu\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = min<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_orc_b()
        {
            Given_RiscVInstructions(0b001010000111_10001_101_11011_0010011);
            AssertCode(    // orc.b\ts11,a7;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __bitwise_or_combine<word64>(a7)");
        }

        [Test]
        public void RiscVRw_orn()
        {
            Given_RiscVInstructions(0b0100000_10010_10001_110_11011_0110011);
            AssertCode(    // orn\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = a7 | ~s2");
        }

        [Test]
        public void RiscVRw_pack()
        {
            Given_RiscVInstructions(0b0000100_10010_10001_100_11011_0110011);
            AssertCode(    // pack\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __pack<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_packh()
        {
            Given_RiscVInstructions(0b0000100_10010_10001_111_11011_0110011);
            AssertCode(    // packh\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __packh<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_packw()
        {
            Given_RiscVInstructions(0b0000100_10010_10001_100_11011_0111011);
            AssertCode(    // packw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 4 instructions",
                "1|L--|v3 = SLICE(a7, word16, 0)",
                "2|L--|v5 = SLICE(a7, word16, 0)",
                "3|L--|v6 = __pack<word16>(v3, v5)",
                "4|L--|s11 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscVRw_rev8_32()
        {
            Given_32bit();
            Given_RiscVInstructions(0b011010011000_10001_101_11011_0010011);
            AssertCode(     // rev8\ts11,a7);
                "0|L--|00010000(4): 1 instructions",
                "1|L--|s11 = __rev8<word32>(a7)");
        }

        [Test]
        public void RiscVRw_rev8_64()
        {
            Given_RiscVInstructions(0b011010111000_10001_101_11011_0010011);
            //Given_64bit();
            AssertCode(     // rev8\ts11,a7
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __rev8<word64>(a7)");
        }

        [Test]
        public void RiscVRw_rol()
        {
            Given_RiscVInstructions(0b0110000_10010_10001_001_11011_0110011);
            AssertCode(    // rol\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __rol<word64,word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_rolw()
        {
            Given_RiscVInstructions(0b0110000_10010_10001_001_11011_0111011);
            AssertCode(    // rolw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v6 = __rol<word32,word64>(v3, s2)",
                "3|L--|s11 = CONVERT(v6, word32, word64)");
        }

        [Test]
        public void RiscVRw_ror()
        {
            Given_RiscVInstructions(0b0110000_10010_10001_101_11011_0110011);
            AssertCode(    // ror\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __ror<word64,word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_rori_32()
        {
            Given_RiscVInstructions(0b0110000_10110_10001_101_11011_0010011);
            AssertCode(    // rori\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __ror<word64,uint32>(a7, 0x16<u32>)");
        }

        [Test]
        public void RiscVRw_rori_64()
        {
            Given_RiscVInstructions(0b011000_110110_10001_101_11011_0010011);
            AssertCode(    // rori\ts11,a7,0x36;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __ror<word64,uint32>(a7, 0x36<u32>)");
        }

        [Test]
        public void RiscVRw_roriw()
        {
            Given_RiscVInstructions(0b0110000_10110_10001_101_11011_0011011);
            AssertCode(    // roriw\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v5 = __ror<word32,uint32>(v3, 0x16<u32>)",
                "3|L--|s11 = CONVERT(v5, word32, word64)");
        }

        [Test]
        public void RiscVRw_rorw()
        {
            Given_RiscVInstructions(0b0110000_10010_10001_101_11011_0111011);
            AssertCode(    // rorw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|v6 = __ror<word32,word64>(v3, s2)",
                "3|L--|s11 = CONVERT(v6, word32, word64)");
        }

        [Test]
        public void RiscVRw_sext_b()
        {
            Given_RiscVInstructions(0b011000000100_10001_001_11011_0010011);
            AssertCode(    // sext.b\ts11,a7;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v3 = SLICE(a7, byte, 0)",
                "2|L--|s11 = CONVERT(v3, byte, int64)");
        }

        [Test]
        public void RiscVRw_sext_h()
        {
            Given_RiscVInstructions(0b011000000101_10001_001_11011_0010011);
            AssertCode(    // sext.h\ts11,a7;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v3 = SLICE(a7, word16, 0)",
                "2|L--|s11 = CONVERT(v3, word16, int64)");
        }

        [Test]
        public void RiscVRw_sh1add()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_010_11011_0110011);
            AssertCode(    // sh1add\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = s2 + a7 * 2<64>");
        }

        [Test]
        public void RiscVRw_sh1add_uw()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_010_11011_0111011);
            AssertCode(    // sh1add.uw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v6 = SLICE(a7, word32, 0)",
                "2|L--|v7 = CONVERT(v6, word32, word64)",
                "3|L--|s11 = s2 + v7 * 2<64>");
        }

        [Test]
        public void RiscVRw_sh2add()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_100_11011_0110011);
            AssertCode(    // sh2add\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = s2 + a7 * 4<64>");
        }

        [Test]
        public void RiscVRw_sh2add_uw()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_100_11011_0111011);
            AssertCode(    // sh2add.uw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v6 = SLICE(a7, word32, 0)",
                "2|L--|v7 = CONVERT(v6, word32, word64)",
                "3|L--|s11 = s2 + v7 * 4<64>");
        }

        [Test]
        public void RiscVRw_sh3add()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_110_11011_0110011);
            AssertCode(    // sh3add\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = s2 + a7 * 8<64>");
        }

        [Test]
        public void RiscVRw_sh3add_uw()
        {
            Given_RiscVInstructions(0b0010000_10010_10001_110_11011_0111011);
            AssertCode(    // sh3add.uw\ts11,a7,s2;
                "0|L--|0000000000010000(4): 3 instructions",
                "1|L--|v6 = SLICE(a7, word32, 0)",
                "2|L--|v7 = CONVERT(v6, word32, word64)",
                "3|L--|s11 = s2 + v7 * 8<64>");
        }

        [Test]
        public void RiscVRw_slli_uw()
        {
            Given_RiscVInstructions(0b000010_110110_10001_001_11011_0011011);
            AssertCode(    // slli.uw\ts11,a7,0x16;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v3 = SLICE(a7, word32, 0)",
                "2|L--|s11 = CONVERT(v3, word32, word64) << 0x16<64>");
        }

        [Test]
        public void RiscVRw_unzip32()
        {
            Given_32bit();
            Given_RiscVInstructions(0b000010011111_10001_101_11011_0010011);
            AssertCode(     // unzip\ts11,a7);
                "0|L--|00010000(4): 1 instructions",
                "1|L--|s11 = __unzip<word32>(a7)");
        }

        [Test]
        public void RiscVRw_xnor()
        {
            Given_RiscVInstructions(0b0100000_10010_10001_100_11011_0110011);
            AssertCode(    // xnor\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = ~(a7 ^ s2)");
        }

        [Test]
        public void RiscVRw_xperm_b()
        {
            Given_RiscVInstructions(0b0010100_10010_10001_100_11011_0110011);
            AssertCode(    // xperm.b\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __xperm_b<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_xperm_n()
        {
            Given_RiscVInstructions(0b0010100_10010_10001_010_11011_0110011);
            AssertCode(    // xperm.n\ts11,a7,s2;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __xperm_n<word64>(a7, s2)");
        }

        [Test]
        public void RiscVRw_zext_h_32()
        {
            Given_32bit();
            Given_RiscVInstructions(0b000010000000_10001_100_11011_0110011);
            AssertCode(     // zext.h\ts11,a7
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v3 = SLICE(a7, word16, 0)",
                "2|L--|s11 = CONVERT(v3, word16, word32)");
        }

        [Test]
        public void RiscVRw_zext_h_64()
        {
            Given_RiscVInstructions(0b000010000000_10001_100_11011_0111011);
            AssertCode(    // zext.h\ts11,a7;
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v3 = SLICE(a7, word16, 0)",
                "2|L--|s11 = CONVERT(v3, word16, word64)");
        }

        [Test]
        public void RiscVRw_zip()
        {
            Given_RiscVInstructions(0b000010011110_10001_001_11011_0010011);
            AssertCode(    // zip\ts11,a7;
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s11 = __zip<word64>(a7)");
        }

    }
}
