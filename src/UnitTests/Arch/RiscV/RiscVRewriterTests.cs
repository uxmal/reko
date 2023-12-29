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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
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
            Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize , "64" },
                { "FloatAbi", 64 },
            });
            baseAddr = Address.Ptr64(0x0010000);
        }

        [Test]
        public void RiscV_rw_amoadd_d()
        {
            Given_HexString("AFB36301");
            AssertCode(     // amoadd.d	t2,t2,s6
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t2 = __amo_add<word64>(t2, &Mem0[s6:word64])");
        }

        [Test]
        public void RiscV_rw_amoadd_w_rl()
        {
            //$TODO: RL?
            Given_HexString("AFAE6302");
            AssertCode(     // amoadd.w.rl	t4,t2,t1
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_add<word32>(t2, &Mem0[t1:word32])",
                "2|L--|t4 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoand_d()
        {
            Given_HexString("AFBF4263");
            AssertCode(     // amoand.d.rl	t6,t0,s4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|t6 = __amo_and<word64>(t0, &Mem0[s4:word64])");
        }

        [Test]
        public void RiscV_rw_amoand_w()
        {
            Given_HexString("2FA4A660");
            AssertCode(     // amoand.w	s0,a3,a0
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_and<word32>(a3, &Mem0[a0:word32])",
                "2|L--|s0 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amomax_w()
        {
            Given_HexString("AFA097A7");
            AssertCode(     // amomax.w.aq.rl	ra,a5,s9
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_max<int32>(a5, &Mem0[s9:int32])",
                "2|L--|ra = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void RiscV_rw_amomax_d_aq_rl()
        {
            //$TODO: aq,rl
            Given_HexString("AFB197A7");
            AssertCode(     // amomax.d.aq.rl	gp,a5,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|gp = __amo_max<int64>(a5, &Mem0[s9:int64])");
        }

        [Test]
        public void RiscV_rw_amomaxu_d_aq()
        {
            Given_HexString("AFBB08E4");
            AssertCode(     // amomaxu.d.aq	s7,a7,zero
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s7 = __amo_max<uint64>(a7, &Mem0[0<64>:uint64])");
        }

        [Test]
        public void RiscV_rw_amomaxu_w()
        {
            Given_HexString("AFAEEFE0");
            AssertCode(     // amomaxu.w	t4,t6,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_max<uint32>(t6, &Mem0[a4:uint32])",
                "2|L--|t4 = CONVERT(v6, uint32, int64)");
        }

        [Test]
        public void RiscV_rw_amomin_d()
        {
            //$TODO: amo_d_aql_rl
            Given_HexString("AFB7B387");
            AssertCode(     // amomin.d.aq.rl	a5,t2,s11
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = __amo_min<int64>(t2, &Mem0[s11:int64])");
        }

        [Test]
        public void RiscV_rw_amomin_w_aq()
        {
            Given_HexString("2FAF2A84");
            AssertCode(     // amomin.w.aq	t5,s5,sp
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_min<int32>(s5, &Mem0[sp:int32])",
                "2|L--|t5 = CONVERT(v6, int32, int64)");
        }


        [Test]
        public void RiscV_rw_amominu_d_aq_rl()
        {
            Given_HexString("AFB797C7");
            AssertCode(     // amominu.d.aq.rl	a5,a5,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = __amo_min<uint64>(a5, &Mem0[s9:uint64])");
        }

        [Test]
        public void RiscV_rw_amominu_w()
        {
            Given_HexString("AFA9EFC0");
            AssertCode(     // amominu.w	s3,t6,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_min<uint32>(t6, &Mem0[a4:uint32])",
                "2|L--|s3 = CONVERT(v6, uint32, int64)");
        }

        [Test]
        public void RiscV_rw_amoor_d()
        {
            Given_HexString("2FB29547");
            AssertCode(     // amoor.d.aq.rl	tp,a1,s9
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|tp = __amo_or<word64>(a1, &Mem0[s9:word64])");
        }

        [Test]
        public void RiscV_rw_amoor_w()
        {
            Given_HexString("AFA08947");
            AssertCode(     // amoor.w.aq.rl	ra,s3,s8
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_or<word32>(s3, &Mem0[s8:word32])",
                "2|L--|ra = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoswap_d()
        {
            Given_HexString("2FB26308");
            AssertCode(     // amoswap.d	tp,t2,t1
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|tp = __amo_swap<word64>(t2, &Mem0[t1:word64])");
        }

        [Test]
        public void RiscV_rw_amoswap_w()
        {
            Given_HexString("AFA81309");
            AssertCode(     // amoswap.w	a7,t2,a7
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v5 = __amo_swap<word32>(t2, &Mem0[a7:word32])",
                "2|L--|a7 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void RiscV_rw_amoxor_d()
        {
            Given_HexString("2FBD2320");
            AssertCode(     // amoxor.d	s10,t2,sp
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s10 = __amo_xor<word64>(t2, &Mem0[sp:word64])");
        }

        [Test]
        public void RiscV_rw_amoxor_w()
        {
            Given_HexString("AFA72320");
            AssertCode(     // amoxor.w	a5,t2,sp
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __amo_xor<word32>(t2, &Mem0[sp:word32])",
                "2|L--|a5 = CONVERT(v6, word32, int64)");
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
        public void RiscV_rw_jal_zero()
        {
            Given_RiscVInstructions(0x9F4FF06Fu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
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
        public void RiscV_rw_jal_not_zero()
        {
            Given_RiscVInstructions(0x9F4FF0EFu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000000F1F4 (0)");
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
        public void RiscV_rw_jalr_ra()
        {
            Given_RiscVInstructions(0x003780E7);    // jalr ra,a5,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call a5 + 3<i64> (0)");
        }

        [Test]
        public void RiscV_rw_sc_d()
        {
            Given_HexString("AFBA3C1B");
            AssertCode(     // sc.d.rl	s5,s9,s3
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s5 = __store_conditional<word64>(s9, &Mem0[s3:word64])");
        }

        [Test]
        public void RiscV_rw_sc_w()
        {
            Given_HexString("AFADE31C");
            AssertCode(     // sc.w.aq	s11,t2,a4
                "0|L--|0000000000010000(4): 2 instructions",
                "1|L--|v6 = __store_conditional<word32>(t2, &Mem0[a4:word32])",
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
        public void RiscV_rw_lui()
        {
            Given_RiscVInstructions(0x000114B7u);   // lui s1,0x00000011<32>
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|s1 = 0x11000<64>");
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
        public void RiscV_dasm_fence_i()
        {
            Given_HexString("0F10 0000");
            AssertCode(     // fence.i
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__fence_i()");
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
        public void RiscV_rw_fmadd()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x8093FD43);
            AssertCode(
                 "0|L--|00010000(4): 1 instructions",
                 "1|L--|fs10 = ft7 * fs1 + fa6");
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
        public void RiscV_rw_fnmadd_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x00B3FDCF);    // fnmadd.s\tfs11,ft7,fa1,ft0
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs11 = -(ft7 * fa1) - ft0");
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
        public void RiscV_rw_jal()
        {
            Given_RiscVInstructions(0x9F4FF06F);    // jal\tzero,00000000000FF1F4
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
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
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(CONVERT(a5, word64, word32) + 8<i32>, word32, int64)");
        }

        [Test]
        public void RiscV_rw_addiw_sign_extend()
        {
            Given_RiscVInstructions(0x00002301);    // c.addiw\tt1,00000000
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|t1 = CONVERT(SLICE(t1, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_addiw()
        {
            Given_RiscVInstructions(0x00002405);    // c.addiw\ts0,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = CONVERT(SLICE(s0 + 1<64>, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_addiw_negative()
        {
            Given_RiscVInstructions(0x0000347D);    // c.addiw\ts0,FFFFFFFFFFFFFFFF
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = CONVERT(SLICE(s0 + 0xFFFFFFFFFFFFFFFF<64>, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_fsdsp()
        {
            Given_RiscVInstructions(0xA7E6);        // c.fsdsp\tfs9,000001C8
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 456<i64>:real64] = fs9");
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
        public void RiscV_rw_c_fsw()
        {
            Given_32bitFloat();
            Given_HexString("00FC");
            AssertCode(     // c.fsw	s0,56(s0)
                "0|L--|00010000(2): 1 instructions",
                "1|L--|Mem0[s0 + 56<i32>:real32] = s0");
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
        public void RiscV_rw_c_nop()
        {
            Given_HexString("0100");
            AssertCode(     // c.nop
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|nop");
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
        public void RiscV_rw_fcvt_lu_d()
        {
            Given_HexString("531534C2");
            AssertCode(     // fcvt.lu.d	a0,fs0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(fs0, real64, uint64)");
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
        public void RiscV_rw_fcvt_w_s()
        {
            Given_32bitFloat();
            Given_HexString("D31704C0");
            AssertCode(     // fcvt.w.s	a5,fs0
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(fs0, real32, int32)");
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
        public void RiscV_rw_fdiv_s()
        {
            Given_32bitFloat();
            Given_HexString("5374F418");
            AssertCode(     // fdiv.s	fs0,fs0,fa5
                "0|L--|00010000(4): 1 instructions",
                "1|L--|fs0 = fs0 / fa5");
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
        public void RiscV_rw_c_flw_32()
        {
            Given_32bitFloat();
            Given_HexString("C462");
            AssertCode(     // c.flw	fs1,4(a3)
                "0|L--|00010000(2): 1 instructions",
                "1|L--|fs1 = Mem0[a3 + 4<i32>:real32]");
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
        public void RiscV_rw_x1()
        {
            Given_RiscVInstructions(0x12E50463u);    // beq\ta0,a4,0000000000100128
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a0 == a4) branch 0000000000010128");
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
        public void RiscV_rw_or()
        {
            Given_RiscVInstructions(0x01846433u);    // or\ts0,s0,s8
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = s0 | s8");
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
        public void RiscV_rw_and()
        {
            Given_RiscVInstructions(0x00F477B3u);    // and\ta5,s0,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = s0 & a5");
        }

        [Test]
        public void RiscV_rw_subw()
        {
            Given_RiscVInstructions(0x40F686BBu);    // subw\ta3,a3,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = CONVERT(SLICE(a3 - a5, word32, 0), word32, int64)");
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
        public void RiscV_rw_lbu()
        {
            Given_RiscVInstructions(0x00094703u);    // lbu\ta4,s2,+00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = CONVERT(Mem0[s2:byte], byte, word64)");
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
        public void RiscV_rw_fcvt_d_s()
        {
            Given_RiscVInstructions(0x42070753u);    // fcvt.d.s\tfa4,fa4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = CONVERT(SLICE(fa4, real32, 0), real32, real64)");
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
        public void RiscV_rw_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            Given_RiscVInstructions(0xA0F727D3u);    // feq.s\ta5,fa4,fa5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(fa4, real32, 0) == SLICE(fa5, real32, 0), bool, word64)");
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
        public void RiscV_rw_fle_s()
        {
            Given_HexString("D307F7A0");
            AssertCode(     // fle.s	a5,fa4,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(fa4, real32, 0) <= SLICE(fa5, real32, 0), bool, word64)");
        }

        [Test]
        public void RiscV_rw_flq()
        {
            Given_128bitFloat();
            Given_HexString("0740E391");
            AssertCode(     // flq	ft0,2334(t1)
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|ft0 = Mem0[t1 + 2334<i64>:real128]");
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
        public void RiscV_rw_fmv_d_x()
        {
            Given_RiscVInstructions(0xF2070753u);    // fmv.d.x\tfa4,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = a4");
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
                "1|L--|v4 = SLICE(0<64>, real32, 0)",
                "2|L--|fa5 = SEQ(0xFFFFFFFF<32>, v4)");
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
        public void RiscV_rw_fneg_s_64bit()
        {
            Given_HexString("D397F720");
            AssertCode(     // fsgnjn.s	fa5,fa5,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = SEQ(0xFFFFFFFF<32>, -SLICE(fa5, real32, 0))");
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
                "1|L--|s7 = __load_reserved<word64>(&Mem0[a5:word64])");
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
        public void RiscV_rw_c_sw()
        {
            Given_RiscVInstructions(0xC29C);    // c.sw\ta3,0(a5)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[a3:word32] = SLICE(a5, word32, 0)");
        }

        [Test]
        public void RiscV_rw_c_sdsp()
        {
            Given_RiscVInstructions(0xE4CE);    // c.sdsp\ts3,00000048
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 72<i64>:word64] = s3");
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
        public void RiscV_rw_c_lui()
        {
            Given_RiscVInstructions(0x00006585);    // c.lui\ta1,00001000
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = 0x1000000<64>");
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
        public void RiscV_rw_c_bnez()
        {
            Given_RiscVInstructions(0x0000EF09);    // c.bnez\ta4,000000000010001A
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0<64>) branch 000000000001001A");
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
        public void RiscV_rw_remuw()
        {
            Given_RiscVInstructions(0x02C8783B);    // remuw\ta6,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a6 = CONVERT(SLICE(a6 %u a2, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_li()
        {
            Given_RiscVInstructions(0x00004521);    // c.li\ta0,00000008
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = 8<64>");
        }


        [Test]
        public void RiscV_rw_c_li_minus3()
        {
            Given_RiscVInstructions(0x00005775);    // c.li\ta4,FFFFFFFFFFFFFFFD
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a4 = 0xFFFFFFFFFFFFFFFD<64>");
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
        public void RiscV_rw_c_lwsp()
        {
            Given_RiscVInstructions(0x00004512);    // c.lwsp\ta0,00000004
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
        public void RiscV_rw_c_lw()
        {
            Given_RiscVInstructions(0x000043F4);    // c.lw\ta3,68(a5)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a3 = CONVERT(Mem0[a5 + 68<i64>:word32], word32, int64)");
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
        public void RiscV_rw_csrw_unknown()
        {
            Given_HexString("7310C5BF");
            AssertCode(     // csrw\t0xbfc,a0
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__csrrw<word64>(0xBFC<u32>, a0)");
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
        public void RiscV_rw_ecall()
        {
            Given_HexString("73000000");
            AssertCode(     // ecall
                "0|T--|0000000000010000(4): 1 instructions",
                "1|L--|__syscall()");
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
        public void RiscV_rw_li()
        {
            Given_RiscVInstructions(0x00004385);    // c.li\tt2,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|t2 = 1<64>");
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
        public void RiscV_rw_c_bnez_backward()
        {
            Given_RiscVInstructions(0xFB05);    // c.bnez\ta4,00000000000FFF30
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0<64>) branch 000000000000FF30");
        }

        [Test]
        public void RiscV_rw_c_fldsp()
        {
            Given_RiscVInstructions(0x00003436);    // c.fldsp\tfa3,00000228
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fa3 = Mem0[sp + 552<i64>:real64]");
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
        public void RiscV_rw_jr_ra()
        {
            Given_RiscVInstructions(0x8082);    // c.jr\tra
            AssertCode(
                "0|R--|0000000000010000(2): 1 instructions",
                "1|R--|return (0,0)");
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
        public void RiscV_rw_c_and()
        {
            Given_RiscVInstructions(0x8FF5);    // c.and\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 & a3");
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
        public void RiscV_rw_c_sub()
        {
            Given_RiscVInstructions(0x8D89);    // c.sub\ta1,a0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = a1 - a0");
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
        public void RiscV_rw_c_addi4spn()
        {
            Given_RiscVInstructions(0x0000101C);    // c.addi4spn\ta5,00000020
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = sp + 32<i64>");
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
        public void RiscV_rw_c_subw()
        {
            Given_RiscVInstructions(0x00009D1D);    // c.subw\ta0,a5
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = CONVERT(SLICE(a0 - a5, word32, 0), word32, int64)");
        }

        [Test]
        public void RiscV_rw_c_addi()
        {
            Given_RiscVInstructions(0x00000785);    // c.addi\ta5,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 + 1<64>");
        }

        [Test]
        public void RiscV_rw_c_addw()
        {
            Given_RiscVInstructions(0x00009FB5);    // c.addw\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = CONVERT(SLICE(a5 + a3, word32, 0), word32, int64)");
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
        public void RiscV_rw_c_srli()
        {
            Given_RiscVInstructions(0x000083A9);    // c.srli\ta5,0000000A
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 >>u 10<i32>");
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
        public void RiscV_rw_c_andi()
        {
            Given_RiscVInstructions(0x00008A61);    // c.andi\ta2,00000018
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 & 0x18<64>");
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
        public void RiscV_rw_c_fld()
        {
            Given_RiscVInstructions(0x00002E64);    // c.fld\tfs1,216(a2)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fs1 = Mem0[a2 + 216<i64>:real64]");
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
        public void RiscV_rw_sltiu()
        {
            Given_RiscVInstructions(0x0014B493);	// sltiu	s1,s1,+00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s1 = CONVERT(s1 <u 1<i64>, bool, word64)");
        }

        [Test]
        public void RiscV_rw_fsw()
        {
            Given_RiscVInstructions(0x8963A3A7);	// fsw	fs6,8732(a5)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[t2 + 2183<i64>:real32] = SLICE(fs6, real32, 0)");
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
        public void RiscV_rw_srl()
        {
            Given_RiscVInstructions(0x00B6D6B3);	// srl	a3,a3,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = a3 >>u a1");
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
        public void RiscV_rw_fsd()
        {
            Given_RiscVInstructions(0x639435A7);    // fsd	fs9,12632(s0)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[s0 + 1579<i64>:real64] = fs9");
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
        public void RiscV_rw_slt()
        {
            Given_RiscVInstructions(0x00A7A533);    // slt\ta0,a5,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = CONVERT(a5 < a0, bool, word64)");
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
        public void RiscV_rw_sraw()
        {
            Given_HexString("BB 5B D5 41");
            AssertCode(     // sraw\ts7,a0,t4
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s7 = CONVERT(SLICE(a0, word32, 0) >> t4, word32, int64)");
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
        public void RiscV_rw_fmul_q()
        {
            Given_128bitFloat();
            Given_HexString("53046316");
            AssertCode(     // fmul.q	fs0,ft6,ft6
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fs0 = ft6 * ft6");
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
        public void RiscV_rw_fnmsub_s()
        {
            Given_32bitFloat();
            Given_RiscVInstructions(0x4189004B);    // fnmsub.s\tft0,fs2,fs8,fs0
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|ft0 = -(fs2 * fs8) + fs0");
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
        public void RiscV_rw_wfi()
        {
            Given_HexString("73005010");
            AssertCode(     // wfi
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|__wait_for_interrupt()");
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
        public void RiscV_rw_fcvt_lu_s()
        {
            Given_HexString("D39437C0");
            AssertCode(     // fcvt.lu.s        s1,fa5
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s1 = CONVERT(SLICE(fa5, real32, 0), real32, uint64)");
        }



    }
}
