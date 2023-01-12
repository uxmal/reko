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
using Reko.Arch.H8;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.H8
{
    [TestFixture]
    public class H8DisassemblerTests : DisassemblerTestBase<H8Instruction>
    {
        private readonly H8Architecture arch;
        private readonly Address addrLoad;

        public H8DisassemblerTests()
        {
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
            this.arch = new H8Architecture(CreateServiceContainer(), "h8", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr16(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void H8Dis_add_reg_reg()
        {
            AssertCode("add.w\tr3,r4", "0934");
        }

        [Test]
        public void H8Dis_add_b_imm()
        {
            AssertCode("add.b\t#0x0C,r3l", "8B0C");
        }

        [Test]
        public void H8Dis_add_b_reg()
        {
            AssertCode("add.b\tr2l,r0h", "08A0");
        }

        [Test]
        public void H8Dis_add_i16()
        {
            AssertCode("add.w\t#0x1234,r3", "79131234");
        }

        [Test]
        public void H8Dis_add_i32()
        {
            AssertCode("add.l\t#0x12345678,er3", "7A1312345678");
        }

        [Test]
        public void H8Dis_adds_1()
        {
            AssertCode("adds\t#0x00000001,er5", "0B05");
        }

        [Test]
        public void H8Dis_adds_2()
        {
            AssertCode("adds\t#0x00000002,er3", "0B83");
        }

        [Test]
        public void H8Dis_addx_imm()
        {
            AssertCode("addx.b\t#0xF8,r2l", "9AF8");
        }

        [Test]
        public void H8Dis_addx_reg()
        {
            AssertCode("addx.b\tr0h,r1h", "0E01");
        }

        [Test]
        public void H8Dis_and_l()
        {
            AssertCode("and.l\ter4,er2", "01F06642");
        }

        [Test]
        public void H8Dis_bcc()
        {
            AssertCode("bcc\t8070", "446E");
        }

        [Test]
        public void H8Dis_bcc_long()
        {
            AssertCode("bcc\t8000", "5840FFFC");
        }

        [Test]
        public void H8Dis_bist()
        {
            AssertCode("bist\t#0x00,r2l", "678A");
        }

        [Test]
        public void H8Dis_bld()
        {
            AssertCode("bld\t#0x07,r1h", "7771");
        }

        [Test]
        public void H8Dis_bset_imm()
        {
            AssertCode("bset\t#0x00,r4h", "7004");
        }

        [Test]
        public void H8Dis_bset_reg_reg()
        {
            AssertCode("bset\tr3l,r1h", "60B1");
        }

        [Test]
        public void H8Dis_bsr_disp8()
        {
            AssertCode("bsr\t7FFE", "55FE");
        }

        [Test]
        public void H8Dis_bsr_disp16()
        {
            AssertCode("bsr\t7FFC", "5C00FFFC");
        }

        [Test]
        public void H8Dis_btst()
        {
            AssertCode("btst\t#0x00,r4h", "7304");
        }

        [Test]
        public void H8Dis_clrmac()
        {
            AssertCode("clrmac", "01A0");
        }

        [Test]
        public void H8Dis_cmp_w_reg_reg()
        {
            AssertCode("cmp.w\tr6,r1", "1D61");
        }

        [Test]
        public void H8Dis_cmp_l_reg()
        {
            AssertCode("cmp.l\tsp,er2", "1FF2");
        }

        [Test]
        public void H8Dis_daa()
        {
            AssertCode("daa\tr2l", "0F0A");
        }

        [Test]
        public void H8Dis_das()
        {
            AssertCode("das\tr7h", "1F07");
        }

        [Test]
        public void H8Dis_dec_b()
        {
            AssertCode("dec.b\tr6l", "1A0E");
        }

        [Test]
        public void H8Dis_divxs_b()
        {
            AssertCode("divxs.b\tr7l,e3", "01D051FB");
        }

        [Test]
        public void H8Dis_divxs_w()
        {
            AssertCode("divxs.w\te0,er5", "01D05385");
        }

        [Test]
        public void H8Dis_divxu_b()
        {
            AssertCode("divxu.b\tr7l,e3", "51FB");
        }

        [Test]
        public void H8Dis_divxu_w()
        {
            AssertCode("divxu.w\te0,er5", "5385");
        }

        [Test]
        public void H8Dis_jmp_aa24()
        {
            AssertCode("jmp\t@0x123456:24", "5A123456");
        }

        [Test]
        public void H8Dis_jsr_aa24()
        {
            AssertCode("jsr\t@0x123456:24", "5E123456");
        }

        [Test]
        public void H8Dis_jsr_ind_aa8()
        {
            AssertCode("jsr\t@@0x54:8", "5F54");
        }

        [Test]
        public void H8Dis_jsr_indirect()
        {
            AssertCode("jsr\t@er2", "5D20");
        }

        [Test]
        public void H8Dis_ldc_reg()
        {
            AssertCode("ldc.b\tr4l,ccr", "030C");
        }

        [Test]
        public void H8Dis_ldc_disp16()
        {
            AssertCode("ldc.w\t@(-2:16,er3),ccr", "01406F30FFFE");
        }

        [Test]
        public void H8Dis_ldm_l()
        {
            AssertCode("ldm.l\t@sp+,(er4-er5)", "01106D75");
            AssertCode("ldm.l\t@sp+,(er4-er6)", "01206D76");
            AssertCode("ldm.l\t@sp+,(er4-sp)", "01306D77");
        }

        [Test]
        public void H8Dis_ldmac()
        {
            AssertCode("ldmac\ter1,mach", "0321");
            AssertCode("ldmac\ter1,macl", "0331");
        }


        [Test]
        public void H8Dis_mac()
        {
            AssertCode("mac\t@er3+,@er4+", "01606D34");
        }

        [Test]
        public void H8Dis_mov_predec()
        {
            AssertCode("mov.w\tr1,@-sp", "6DF1");
        }

        [Test]
        public void H8Dis_mov_b_abs8()
        {
            AssertCode("mov.b\t@0x79:8,r0h", "207942");
        }

        [Test]
        public void H8Dis_mov_b_base_offset()
        {
            AssertCode("mov.b\t@(4660:16,er2),r0h", "6E201234");
        }

        [Test]
        public void H8Dis_mov_b_disp32()
        {
            AssertCode("mov.b\tr2h,@(0x12345678:32,er4)", "78406AA212345678");
        }

        [Test]
        public void H8Dis_mov_b_disp32b()
        {
            AssertCode("mov.b\tr2l,@(0x12345678:32,er4)", "78406AAA12345678");
        }

        [Test]
        public void H8Dis_mov_b_disp32c()
        {
            AssertCode("mov.b\tr2l,@(0x12345678:32,er4)", "78406AAA12345678");
        }

        [Test]
        public void H8Dis_mov_b_imm()
        {
            AssertCode("mov.b\t#0xFF,r7l", "FFFF");
        }

        [Test]
        public void H8Dis_mov_b_indirect()
        {
            AssertCode("mov.b\t@er6,r5h", "6865");
        }

        [Test]
        public void H8Dis_mov_l_abs16_to_reg()
        {
            AssertCode("mov.l\t@0xFEDC:16,er0", "01 00 6B 00 fe dc");
        }

        [Test]
        public void H8Dis_mov_l_abs32to_reg()
        {
            AssertCode("mov.l\t@0x12345678:32,er0", "01 00 6B 20 12 34 56 78");
        }

        [Test]
        public void H8Dis_mov_l_abs16_to_mem()
        {
            AssertCode("mov.l\t@0xFEDC:16,er0", "01 00 6b 00 fe dc");
        }

        [Test]
        public void H8Dis_mov_l_abs32_to_mem()
        {
            AssertCode("mov.l\t@0x12345678:32,er0", "01 00 6b 20 12 34 56 78");
        }

        [Test]
        public void H8Dis_mov_l_indirect_to_reg()
        {
            AssertCode("mov.l\t@er0,er3", "01006903");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp16_to_reg()
        {
            AssertCode("mov.l\ter3,@(-2:16,er0)", "01006F83FFFE");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp32_to_reg()
        {
            AssertCode("mov.l\t@(0x23512345:32,er3),er1", "010078306AA123512345");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp32_to_mem()
        {
            AssertCode("mov.l\ter1,@(0x23512345:32,er3)", "010078306BA123512345");
        }

        [Test]
        public void H8Dis_mov_l_indirect_to_mem()
        {
            AssertCode("mov.l\ter3,@er0", "01006983");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp_to_mem()
        {
            AssertCode("mov.l\ter3,@(-2:16,er0)", "01006F83FFFE");
        }

        [Test]
        public void H8Dis_mov_l_postinc()
        {
            AssertCode("mov.l\t@er0+,er3", "01006D03");
        }

        [Test]
        public void H8Dis_mov_l_preinc()
        {
            AssertCode("mov.l\ter3,@-er0", "01006D83");
        }

        [Test]
        public void H8Dis_mov_postinc()
        {
            AssertCode("mov.w\t@sp+,r2", "6D72");
        }

        [Test]
        public void H8Dis_mov_w_aa16()
        {
            AssertCode("mov.w\t@0x1234:16,e6", "6B0E 1234");
        }

        [Test]
        public void H8Dis_mov_w_aa24()
        {
            AssertCode("mov.w\t@0x123456:24,e6", "6B2E 123456");
        }

        [Test]
        public void H8Dis_mov_w_base_offset()
        {
            AssertCode("mov.w\t@(4660:16,sp),r5", "6F751234");
        }

        [Test]
        public void H8Dis_mov_w_imm()
        {
            AssertCode("mov.w\t#0x1234,r3", "79031234");
        }

        [Test]
        public void H8Dis_mov_w_reg_reg()
        {
            AssertCode("mov.w\tr5,r3", "0D53");
        }

        [Test]
        public void H8Dis_movfpe()
        {
            AssertCode("movfpe\t@0x123:16,r3h", "6A430123");
        }

        [Test]
        public void H8Dis_movtpe()
        {
            AssertCode("movtpe\tr3h,@0x123:16", "6AC30123");
        }

        [Test]
        public void H8Dis_mulxs_b()
        {
            AssertCode("mulxs.b\tr3h,r4", "01C05034");
        }

        [Test]
        public void H8Dis_mulxs_w()
        {
            AssertCode("mulxs.w\tr3,er4", "01C05234");
        }

        [Test]
        public void H8Dis_mulxu_b()
        {
            AssertCode("mulxu.b\tr1h,r2h", "5012");
        }

        [Test]
        public void H8Dis_not_b()
        {
            AssertCode("not.b\tr2l", "170A");
        }

        [Test]
        public void H8Dis_or_b()
        {
            AssertCode("or.b\tr1l,r0l", "1498");
        }

        [Test]
        public void H8Dis_or_b_imm()
        {
            AssertCode("or.b\t#0x10,r0h", "C010");
        }

        [Test]
        public void H8Dis_or()
        {
            AssertCode("or.l\ter4,er2", "01F06442");
        }

        [Test]
        public void H8Dis_rotxl_b()
        {
            AssertCode("rotxl.b\tr2h", "1202");
        }

        [Test]
        public void H8Dis_rotxr_b()
        {
            AssertCode("rotxr.b\tr2l", "130A");
        }

        [Test]
        public void H8Dis_rts()
        {
            AssertCode("rts", "5470");
        }

        [Test]
        public void H8Dis_shar_b()
        {
            AssertCode("shar.b\tr2h", "1182");
        }

        [Test]
        public void H8Dis_shll()
        {
            AssertCode("shll.b\tr2l", "100A");
        }

        [Test]
        public void H8Dis_sleep()
        {
            AssertCode("sleep", "0180");
        }

        [Test]
        public void H8Dis_sub()
        {
            AssertCode("sub.l\ter0,er0", "1A80");
        }

        [Test]
        public void H8Dis_stc_ind()
        {
            AssertCode("stc.w\tccr,@sp", "014069F0");
        }

        [Test]
        public void H8Dis_stm_l()
        {
            AssertCode("stm.l\t(er4-er5),@-sp", "01106DF4");
            AssertCode("stm.l\t(er4-er6),@-sp", "01206DF4");
            AssertCode("stm.l\t(er4-sp),@-sp", "01306DF4");
        }

        [Test]
        public void H8Dis_stmac()
        {
            AssertCode("stmac\tmach,er1", "0221");
            AssertCode("stmac\tmacl,er1", "0231");
        }

        [Test]
        public void H8Dis_sub_w_regs()
        {
            AssertCode("sub.w\tr1,r2", "1912");
        }

        [Test]
        public void H8Dis_subs_1()
        {
            AssertCode("subs\t#0x00000002,sp", "1B87");
        }

        [Test]
        public void H8Dis_tas()
        {
            AssertCode("tas\t@er2", "01E07B2C");
        }

        [Test]
        public void H8Dis_trapa()
        {
            AssertCode("trapa\t#0x03", "5730");
        }

        [Test]
        public void H8Dis_xor_l()
        {
            AssertCode("xor.l\ter4,er2", "01F06542");
        }

        [Test]
        public void H8Dis_xor_w()
        {
            AssertCode("xor.w\tr2,e4", "652C");
        }

        [Test]
        public void H8Dis_xorc()
        {
            AssertCode("xorc\t#0xDC,ccr", "05DC");
        }

#if BORED
        [Test]
        public void H8Dis_7C1C()
        {
            AssertCode("@@@", "7C1C6000000");
        }

        [Test]
        public void H8Dis_7C1C7()
        {
            AssertCode("@@@", "7C1C7000000");
        }

        [Test]
        public void H8Dis_7D1C()
        {
            AssertCode("@@@", "7D1C6000000");
        }

        [Test]
        public void H8Dis_7D1C7()
        {
            AssertCode("@@@", "7D1C7000000");
        }
        [Test]
        public void H8Dis_7E1C6()
        {
            AssertCode("@@@", "7E1C6000000");
        }

        [Test]
        public void H8Dis_7E1C7()
        {
            AssertCode("@@@", "7E1C7000000");
        }
        [Test]
        public void H8Dis_7F1C6()
        {
            AssertCode("@@@", "7F1C6000000");
        }

        [Test]
        public void H8Dis_7F1C7()
        {
            AssertCode("@@@", "7F1C7000000");
        }
#endif
    }
}