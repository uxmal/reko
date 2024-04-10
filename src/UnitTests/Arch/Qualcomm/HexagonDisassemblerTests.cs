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
using Reko.Arch.Qualcomm;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Qualcomm
{
    public class HexagonDisassemblerTests : DisassemblerTestBase<HexagonPacket>
    {
        private readonly HexagonArchitecture arch;
        private readonly Address addrLoad;

        public HexagonDisassemblerTests()
        {
            this.arch = new HexagonArchitecture(CreateServiceContainer(), "hexagon", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            //Assert.AreNotEqual("{  }", instr.ToString());
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Hexagon_dasm_allocframe()
        {
            AssertCode("{ allocframe(#0x18) }", "03C09DA0");
        }

        [Test]
        public void Hexagon_dasm_and_simm()
        {
            AssertCode("{ r7 = and(r7,#0x1F) }", "E7C30776");
        }
        [Test]
        public void Hexagon_dasm_and_rr()
        {
            AssertCode("{ r1:r0 = and(r1:r0,r5:r4) }", "00C4E0D3");
        }

        [Test]
        public void Hexagon_dasm_and_not_rr()
        {
            AssertCode("{ r1:r0 = and(r5:r4,~r1:r0) }", "20C4E0D3");
        }

        [Test]
        public void Hexagon_dasm_ANDEQ_lsl()
        {
            AssertCode("{ r26 &= lsl(r26,r7) }", "DAC75AC6");
        }

        [Test]
        public void Hexagon_dasm_add()
        {
            AssertCode("{ r28 = add(r28,r1) }", "1C411CF3");
        }

        [Test]
        public void Hexagon_dasm_add_pc_imm()
        {
            AssertCode("{ r14 = add(PC,#0x4) }", "0EC2496A");
        }

        [Test]
        public void Hexagon_asm_add_pc_imm_prefix()
        {
            AssertCode("{ immext(#0x21C40); r28 = add(PC,#0x21C50) }", "71480000 1CC8496A");
        }

        [Test]
        public void Hexagon_dasm_add_mpy()
        {
            AssertCode("{ r26 = add(r23,mpyi(r26,r19)) }", "17DA13E3");
        }

        [Test]
        public void Hexagon_dasm_add_uimm_mpy()
        {
            AssertCode("{ immext(#0x4000); r8 = add(#0x4000,mpyi(r8,r0)) }", "00410000084008D7");
        }

        [Test]
        public void Hexagon_dasm_add_lsl16()
        {
            AssertCode("{ r1 = add(r3.l,r1.l):<<16 }", "014341D5");
        }

        [Test]
        public void Hexagon_dasm_addasl()
        {
            AssertCode("{ r26 = addasl(r18,r23,#0x3) }", "7AD217C4");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_control_register()
        {
            AssertCode("{ r17 = UGP }", "11400A6A");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_predicate()
        {
            AssertCode("{ r0 = p0 }", "00C04089");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_predicate_expr()
        {
            AssertCode("{ p0 = cmp.eq(r1,#0x0) }", "00400175");
        }

        [Test]
        public void Hexagon_dasm_add_asl()
        {
            AssertCode("{ r1 = add(#0xF2,asl(r1,#0x4)) }", "1272E1DE");
        }

        [Test]
        public void Hexagon_dasm_add_conditional()
        {
            AssertCode("{ if (!p0.new) r0 = add(r16,#0x14) }", "80629074");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_or()
        {
            AssertCode("{ r1 = or(r1,r2) }", "01C221F1");
        }

        [Test]
        public void Hexagon_dasm_or_predicates()
        {
            AssertCode("{ p0 = or(p1,p2) }", "0041226B");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_h_imm()
        {
            AssertCode("{ r0.h = #0x0 }", "00C02072");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_l_uimm()
        {
            AssertCode("{ r1.l = #0xF0 }", "3CC02171");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_sysreg()
        {
            AssertCode("{ r1 = brkptcfg1 }", "01C0A76E");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_simm()
        {
            AssertCode("{ r0 = #0xFFFF8000 }", "00408078");
        }

        [Test]
        public void Hexagon_dasm_any8()
        {
            AssertCode("{ p0 = any8(vcmpb.eq(r5:r4,r7:r6)) }", "006604D2");
        }

        [Test]
        public void Hexagon_dasm_call()
        {
            AssertCode("{ call 00101924 }", "924C005A");
        }

        [Test]
        public void Hexagon_dasm_call_predicated()
        {
            AssertCode("{ if (p1) call 000F5D0C }", "875D975D");
        }

        [Test]
        public void Hexagon_dasm_callr()
        {
            AssertCode("{ callr r28 }", "00C0BC50");
        }

        [Test]
        public void Hexagon_dasm_cl0()
        {
            AssertCode("{ r6 = cl0(r1:r0) }", "46404088");
        }

        [Test]
        public void Hexagon_dasm_cmp_eq_r_r()
        {
            AssertCode("{ p0 = cmp.eq(r18,r20) }", "005412F2");
        }

        [Test]
        public void Hexagon_dasm_cmp_eq_rr_rr()
        {
            AssertCode("{ p0 = cmp.eq(r1:r0,r3:r2) }", "004280D2");
        }

        [Test]
        public void Hexagon_dasm_cmp_gt()
        {
            AssertCode("{ p0 = cmp.gt(r2,#0x0) }", "00404275");
        }

        [Test]
        public void Hexagon_dasm_cmbp_eq()
        {
            AssertCode("{ p0 = cmpb.eq(r0,#0x78) }", "004F00DD");
        }

        [Test]
        public void Hexagon_dasm_convert_sf2uwf()
        {
            AssertCode("{ r2 = convert_sf2uw(r2) }", "2240628B");
        }

        [Test]
        public void Hexagon_dasm_convert_uw2sf()
        {
            AssertCode("{ r21 = convert_uw2sf(r2) }", "1540228B");
        }

        [Test]
        public void Hexagon_dasm_convert_uw2sf_chop()
        {
            AssertCode("{ r21 = convert_uw2sf(r24):chop }", "3540988B");
        }

        [Test]
        public void Hexagon_dasm_convert_w2sf()
        {
            AssertCode("{ r2 = convert_w2sf(r20) }", "0240548B");
        }

        [Test]
        public void Hexagon_dasm_crswap()
        {
            AssertCode("{ crswap(r29,sgp0) }", "00C01D65");
        }

        [Test]
        public void Hexagon_dasm_combine_lowords()
        {
            AssertCode("{ r0 = combine(r0.l,r0.l) }", "00C0E0F3");
        }

        [Test]
        public void Hexagon_dasm_combine_reg_reg()
        {
            AssertCode("{ r19:r18 = combine(r3,r2) }", "12C203F5");
        }

        [Test]
        public void Hexagon_dasm_combine_reg_s8()
        {
            AssertCode("{ r17:r16 = combine(r0,#0x0) }", "10600073");
        }

        [Test]
        public void Hexagon_dasm_combine_s8_reg()
        {
            AssertCode("{ r3:r2 = combine(#0x2,r16) }", "42603073");
        }

        [Test]
        public void Hexagon_dasm_test_jump()
        {
            AssertCode("{ p0 = cmp.eq(r1,#0x0); if (p0.new) jump:nt 00100018 }", "0CC00110");
        }

        [Test]
        public void Hexagon_dasm_negated_test_jump()
        {
            AssertCode("{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:t 000FFFF4 }", "FAE07D10");
        }

        [Test]
        public void Hexagon_dasm_convert_chop()
        {
            AssertCode("{ r7:r6 = convert_df2ud(r23:r22):chop }", "E680F680");
        }

        [Test]
        public void Hexagon_dasm_dczeroa()
        {
            AssertCode("{ dczeroa(r6) }", "00C0C6A0");
        }

        [Test]
        public void Hexagon_dasm_dealloc_return()
        {
            AssertCode("{ dealloc_return }", "1EC01E96");
        }

        [Test]
        public void Hexagon_dasm_dealloc_return_conditional()
        {
            AssertCode("{ if (p0.new) dealloc_return:nt }", "1EC81E96");
        }

        [Test]
        public void Hexagon_dasm_dfclass()
        {
            AssertCode("{ p3 = dfclass(r1:r0,#0x2) }", "534080DC");
        }

        [Test]
        public void Hexagon_dasm_dfcmp()
        {
            AssertCode("{ p0 = dfcmp.eq(r5:r4,r3:r2) }", "0042E4D2");
        }

        [Test]
        public void Hexagon_dasm_duplex_add_sp()
        {
            AssertCode("{ r16 = add(r29,#0x0); memw(r29) = r0 }", "0028086C");
        }

        [Test]
        public void Hexagon_dasm_duplex_dealloc_return()
        {
            AssertCode("{ r0 = memw(r0); dealloc_return }", "403F0000");
        }

        [Test]
        public void Hexagon_dasm_duplex_ld_memd()
        {
            AssertCode("{ r3:r2 = memd(r29+8); r1:r0 = memd(r29) }", "001E093E");
        }

        [Test]
        public void Hexagon_dasm_ld_and_0xFF()
        {
            AssertCode("{ p1 = and(p1,p1); r22 = r4; r0 = add(r0,r15); r3 = #0x0; r7 = add(r7,#0xFF) }", "0141616B16406470004F00F316C70328");
        }

        [Test]
        public void Hexagon_dasm_duplex_combine()
        {
            AssertCode("{ r23:r22 = combine(#0x2,#0x1); r23:r22 = combine(#0x0,#0x1) }", "A73EB73E");
        }

        [Test]
        public void Hexagon_dasm_duplex_cmp_eq_allocframe()
        {
            AssertCode("{ p0 = cmp.eq(r1,#0x0); allocframe(#0x0) }", "003C1079");
        }

        [Test]
        public void Hexagon_dasm_EQ()
        {
            AssertCode("{ if (r3=#0x0) jump:nt 0010002C }", "16C08361");
        }

        [Test]
        public void Hexagon_dasm_extractu()
        {
            AssertCode("{ r7:r6 = extractu(r23:r22,#0x1,#0x21) }", "86819681");
        }

        [Test]
        public void Hexagon_dasm_icdtagr()
        {
            AssertCode("{ r7 = icdtagr(r23) }", "E755F755");
        }

        [Test]
        public void Hexagon_dasm_if_cmp_eq_new_imm()
        {
            AssertCode("{ p0 = cmp.eq(r18,#0x0); r11 = r9; r0 = memb(r0+32); if (cmp.eq(r0.new,#0x2)) jump:t 00100024 }", "004012750B4069700044009112E20224");
        }

        [Test]
        public void Hexagon_dasm_if_cmp_eq_new_reg()
        {
            AssertCode("{ p0 = cmp.eq(r21,#0x0); r25 = #0x0; r2 = memw(r18+24); if (!cmp.eq(r2.new,r4)) jump:t 00100034 }", "0040157519400078C24092911AE44220");
        }

        [Test]
        public void Hexagon_dasm_if_cmp_gt_new_imm()
        {
            AssertCode("{ r2 = memw(r22); if (cmp.gt(r2.new,#0x1)) jump:t 00100054 }", "024096912AE18224");
        }

        [Test]
        public void Hexagon_dasm_if_cmp_gtu_reg_regnew()
        {
            AssertCode("{ r0 = #0x0; r4 = memw(r2+16); if (cmp.gtu(r3,r4.new)) jump:t 00100048 }", "004000788440829124E30222");
        }

        [Test]
        public void Hexagon_dasm_insert()
        {
            AssertCode("{ r1 = insert(#0xB,#0x13) }", "814B498F");
        }

        [Test]
        public void Hexagon_dasm_jump()
        {
            AssertCode("{ jump 00100098 }", "4CC00058");
        }

        [Test]
        public void Hexagon_dasm_jumpr_31_conditional()
        {
            AssertCode("{ p0 = cmp.eq(r2,#0x0); if (p0.new) jumpr r31 }", "C63F2059");
        }

        [Test]
        public void Hexagon_dasm_jump_predicated()
        {
            AssertCode("{ if (!p1) jump:nt 000FFFF8 }", "FC61FF5C");
        }

        [Test]
        public void Hexagon_dasm_jump_conditional_application()
        {
            AssertCode("{ r4 = add(r2,r3); if (cmp.gtu(r4.new,r0)) jump:t 00100020 }", "044302F3 10E00221");
        }

        [Test]

        public void Hexagon_dasm_l2fetch_rr()
        {
            AssertCode("{ l2fetch(r2,r1:r0) }", "00C082A6");
        }

        [Test]
        public void Hexagon_dasm_ld_memd()
        {
            AssertCode("{ r17:r16 = memd(r30-8) }", "F07FDE97");
        }

        [Test]
        public void Hexagon_dasm_ld_postinc_predicated()
        {
            AssertCode("{ if (p2) r0 = memw(r10++#4) }", "20E48A9B");
        }

        [Test]
        public void Hexagon_dasm_ld_rdd_memub()
        {
            AssertCode("{ r21:r20 = memd(r11++m0) }", "14C0CB9D");
        }

        [Test]
        public void Hexagon_dasm_ld_memuh()
        {
            AssertCode("{ r1 = memuh(r0+6) }", "61C06091");
        }

        [Test]
        public void Hexagon_dasm_ld_memw_absset()
        {
            AssertCode("{ immext(#0x10040); r16 = memw(r0=00010068) }", "01440000105A809B");
        }

        [Test]
        public void Hexagon_dasm_ld_memw_locked()
        {
            AssertCode("{ r1 = memw_locked(r0) }", "01C00092");
        }

        [Test]
        public void Hexagon_dasm_ld_indexed()
        {
            AssertCode("{ r3 = memb(r20+r4) }", "0344313A");
        }

        [Test]
        public void Hexagon_dasm_max()
        {
            AssertCode("{ r18 = max(r2,r18) }", "1252C2D5");
        }

        [Test]
        public void Hexagon_dasm_memw_locked()
        {
            AssertCode("{ memw_locked(r16,p0) = r1 }", "00C1B0A0");
        }

        [Test]
        public void Hexagon_dasm_mux()
        {
            AssertCode("{ r7 = mux(p1,#0xFFFFFFD5,#0x6F) }", "A77AB77A");
        }

        [Test]
        public void Hexagon_dasm_mpy_l_l()
        {
            AssertCode("{ r6 = mpy(r8.l,r6.l) }", "064608EC");
        }

        [Test]
        public void Hexagon_dasm_mpyi()
        {
            AssertCode("{ r17 = mpyi(r1,r0) }", "114001ED");
        }

        [Test]
        public void Hexagon_dasm_mpyu()
        {
            AssertCode("{ r3 = mpyu(r4,r3) }", "234344ED");
        }

        [Test]
        public void Hexagon_dasm_mpyu_64()
        {
            AssertCode("{ r1:r0 = mpyu(r1,r4) }", "004441E5");
        }

        [Test]
        public void Hexagon_dasm_new_value()
        {
            AssertCode("{ r6 = or(r1,r7); r3 = or(r1,r7); memb(r4) = r3.new }", "064721F1 034721F1   00D2A4A1545412C4");
        }

        [Test]
        public void Hexagon_dasm_nop()
        {
            AssertCode("{ nop }", "0040007F");
        }

        [Test]
        public void Hexagon_dasm_or()
        {
            AssertCode("{ r1 = or(r1,r7) }", "014721F1");
        }

        [Test]
        public void Hexagon_dasm_regression1()
        {
            AssertCode("{ memd(r29-16) = r17:r16; allocframe(#0x8) }", "10 1c f4 eb");
        }

        [Test]
        public void Hexagon_dasm_regression2()
        {
          //  AssertCode("@@@", "15 75 82 97");
            AssertCode("{ immext(#0xFFFFFF80); r21 = memw(r2-96); immext(#0xFFFFFF80); r20 = memw(r2-80) }", "fe 7f ff 0f 15 75 82 97 fe 7f ff 0f 94 f5 82 97");
        }

        [Test]
        public void Hexagon_dasm_rte()
        {
            AssertCode("{ rte }", "00C0E057");
        }

        [Test]
        public void Hexagon_dasm_sfadd()
        {
            AssertCode("{ r0 = sfadd(r0,r4) }", "004400EB");
        }

        [Test]
        public void Hexagon_dasm_sfmax()
        {
            AssertCode("{ r3 = sfmax(r23,r3) }", "03C397EB");
        }

        [Test]
        public void Hexagon_dasm_sfmin()
        {
            AssertCode("{ r7 = sfmin(r6,r3) }", "274386EB");
        }

        [Test]
        public void Hexagon_dasm_sfmpy()
        {
            AssertCode("{ r0 = sfmpy(r2,r4) }", "004442EB");
        }

        [Test]
        public void Hexagon_dasm_sfsub()
        {
            AssertCode("{ r4 = sfsub(r7,r21) }", "249507EB");
        }

        [Test]
        public void Hexagon_dasm_st_addeq_imm()
        {
            AssertCode("{ memw(r17+44) += #0x1 }", "81C5513F");
        }

        [Test]
        public void Hexagon_dasm_st_addeq_reg()
        {
            AssertCode("{ memw(r16+64) += r0 }", "00C8503E");
        }

        [Test]
        public void Hexagon_dasm_st_indexed()
        {
            AssertCode("{ immext(#0xE700); memw(r17<<#2+0000E700) = r3 }", "9C43000080E391AD");
        }

        [Test]
        public void Hexagon_dasm_store_gp()
        {
            AssertCode("{ memw(gp+178576) = r12 }", "874C974C");
        }

        [Test]
        public void Hexagon_dasm_st_memw()
        {
            AssertCode("{ memw(r1) = r0 }", "00C081A1");
        }

        [Test]
        public void Hexagon_dasm_st_predicated_autoinc()
        {
            AssertCode("{ if (p0) memd(r0++#8) = r1:r0 }", "08E0C0AB");
        }

        [Test]
        public void Hexagon_dasm_st_subeq()
        {
            AssertCode("{ memw(r29+92) -= #0x1 }", "A1CB5D3F");
        }


        [Test]
        public void Hexagon_dasm_seq()
        {
            AssertCode("{ loop0(00100028,#0x18); p1 = cmp.gt(r1,#-0x1); if (!p1.new) jump:nt 001000B8 }", "D04201695C41C113");
        }

        [Test]
        public void Hexagon_dasm_seq_cmpeq_p0_set_if()
        {
            AssertCode("{ p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 000FFC40 }", "20C0A319");
        }

        [Test]
        public void Hexagon_dasm_seq_cmpeq_p1()
        {
            AssertCode("{ p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 000FFC00 }", "01CE201A");
        }

        [Test]
        public void Hexagon_dasm_seq_cmpeq_gtu_p1()
        {
            AssertCode("{ p1 = cmp.gtu(r7,#0x12); if (p1.new) jump:t 000FFE5C }", "2E72371B");
        }

        [Test]
        public void Hexagon_dasm_setbit()
        {
            AssertCode("{ r2 = setbit(r2,#0x10) }", "02D8C28C");
        }

        [Test]
        public void Hexagon_dasm_start()
        {
            AssertCode("{ start(r8) }", "20C06864");
        }

        [Test]
        public void Hexagon_dasm_stop()
        {
            AssertCode("{ stop(r0) }", "00C06064");
        }

        [Test]
        public void Hexagon_dasm_store_r64()
        {
            AssertCode("{ memd(r29+56) = r15:r14 }", "07CEDDA1");
        }

        [Test]
        public void Hexagon_dasm_tlbw()
        {
            AssertCode("{ tlbw(r3:r2,r0) }", "00C0026C");
        }

        [Test]
        public void Hexagon_dasm_tstbit()
        {
            AssertCode("{ p0 = tstbit(r0,#0x0) }", "00C00085");
        }

        [Test]
        public void Hexagon_dasm_valignb_rr()
        {
            AssertCode("{ r7:r6 = valignb(r11:r10,r13:r12,p2) }", "464A0CC2");
        }

        [Test]
        public void Hexagon_dasm_vraddub_addeq()
        {
            AssertCode("{ r15:r14 += vraddub(r13:r12,r11:r10) }", "2E4A4CEA");
        }

        [Test]
        public void Hexagon_dasm_vsplatb()
        {
            AssertCode("{ r7 = vsplatb(r1) }", "E740418C");
        }

        [Test]
        public void Hexagon_dasm_lsr_rr()
        {
            AssertCode("{ r1:r0 = lsr(r1:r0,#0x30) }", "20700080");
        }

        [Test]
        public void Hexagon_dasm_longnew()
        {
            AssertCode("{ r2 = or(r3,and(r2,#0xF)); immext(#0x10000); memb(gp+65540) = r2.new }", "E34142DA0044000028C4A048");
        }

        [Test]
        public void Hexagon_dasm_zxth()
        {
            AssertCode("{ r0 = zxth(r0) }", "0040C070");
        }

        [Test]
        public void Hexagon_dasm_long_offset()
        {
            AssertCode("{ immext(#0x1B40); r2 = memb(r2+00001B44) }", "6D40000002D1029D");
        }

        [Test]
        public void Hexagon_dasm_tlbp()
        {
            AssertCode("{ r5 = tlbp(r1) }", "05C0816C");
        }
   }
}