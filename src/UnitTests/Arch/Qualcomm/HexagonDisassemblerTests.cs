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
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Hexagon_dasm_allocframe()
        {
            AssertCode("{ allocframe(+00000018) }", "03C09DA0");
        }

        [Test]
        public void Hexagon_dasm_and_simm()
        {
            AssertCode("{ r7 = and(r7,0000001F) }", "E7C30776");
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
        public void Hexagon_dasm_add_mpy()
        {
            AssertCode("{ r26 = add(r23,mpyi(r26,r19)) }", "17DA13E3");
        }

        [Test]
        public void Hexagon_dasm_add_lsl16()
        {
            AssertCode("{ r1 = add(r3.l,r1.l):<<16 }", "014341D5");
        }

        [Test]
        public void Hexagon_dasm_addasl()
        {
            AssertCode("{ r26 = addasl(r18,r23,00000003) }", "7AD217C4");
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
            AssertCode("{ p0 = cmp.eq(r1,00000000) }", "00400175");
        }

        [Test]
        public void Hexagon_dasm_add_asl()
        {
            AssertCode("{ r1 = add(000000F2,asl(r1,00000004)) }", "1272E1DE");
        }

        [Test]
        public void Hexagon_dasm_add_conditional()
        {
            AssertCode("{ if (!p0.new) r0 = add(r16,00000014) }", "80629074");
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
            AssertCode("{ r0.h = 0000 }", "00C02072");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_l_uimm()
        {
            AssertCode("{ r1.l = 00F0 }", "3CC02171");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_r_sysreg()
        {
            AssertCode("{ r1 = brkptcfg1 }", "01C0A76E");
        }

        [Test]
        public void Hexagon_dasm_ASSIGN_simm()
        {
            AssertCode("{ r0 = FFFF8000 }", "00408078");
        }

        [Test]
        public void Hexagon_dasm_any8()
        {
            AssertCode("{ p0 = any8(vcmpb.eq(r5:r4,r7:r6)) }", "006604D2");
        }

        [Test]
        public void Hexagon_dasm_call()
        {
            AssertCode("{ call\t00101924 }", "924C005A");
        }

        [Test]
        public void Hexagon_dasm_call_predicated()
        {
            AssertCode("{ if (p1) call\t000F5D0C }", "875D975D");
        }

        [Test]
        public void Hexagon_dasm_callr()
        {
            AssertCode("{ callr\tr28 }", "00C0BC50");
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
            AssertCode("{ p0 = cmp.gt(r2,00000000) }", "00404275");
        }

        [Test]
        public void Hexagon_dasm_cmbp_eq()
        {
            AssertCode("{ p0 = cmpb.eq(r0,78) }", "004F00DD");
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
        public void Hexagon_dasm_combined_test_jump()
        {
            AssertCode("{ if (p0.new) jump:nt\t00100018; p0 = cmp.eq(r1,00000000) }", "0CC00110");
        }

        [Test]
        public void Hexagon_dasm_combined_negated_test_jump()
        {
            AssertCode("{ if (!p0.new) jump:t\t000FFFF4; p0 = cmp.eq(r29,00000001) }", "FAE07D10");
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
            AssertCode("{ p3 = dfclass(r1:r0,00000002) }", "534080DC");
        }

        [Test]
        public void Hexagon_dasm_dfcmp()
        {
            AssertCode("{ p0 = dfcmp.eq(r5:r4,r3:r2) }", "0042E4D2");
        }

        [Test]
        public void Hexagon_dasm_duplex_add_sp()
        {
            AssertCode("{ memw(r29) = r0; r16 = add(r29,00000000) }", "0028086C");
        }

        [Test]
        public void Hexagon_dasm_duplex_dealloc_return()
        {
            AssertCode("{ dealloc_return; r0 = memw(r0) }", "403F0000");
        }

        [Test]
        public void Hexagon_dasm_duplex_ld_memd()
        {
            AssertCode("{ r1:r0 = memd(r29); r3:r2 = memd(r29+8) }", "001E093E");
        }

        [Test]
        public void Hexagon_dasm_duplex_combine()
        {
            AssertCode("{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000002,00000001) }", "A73EB73E");
        }

        [Test]
        public void Hexagon_dasm_duplex_cmp_eq_allocframe()
        {
            AssertCode("{ allocframe(00000000); p0 = cmp.eq(r1,00000000) }", "003C1079");
        }

        [Test]
        public void Hexagon_dasm_EQ()
        {
            AssertCode("{ if (r3=00000000) jump:nt\t0010002C }", "16C08361");
        }

        [Test]
        public void Hexagon_dasm_extractu()
        {
            AssertCode("{ r7:r6 = extractu(r23:r22,00000001,00000021) }", "86819681");
        }

        [Test]
        public void Hexagon_dasm_icdtagr()
        {
            AssertCode("{ r7 = icdtagr(r23) }", "E755F755");
        }

        [Test]
        public void Hexagon_dasm_insert()
        {
            AssertCode("{ r1 = insert(0000000B,00000013) }", "814B498F");
        }

        [Test]
        public void Hexagon_dasm_jump()
        {
            AssertCode("{ jump\t00100098 }", "4CC00058");
        }

        [Test]
        public void Hexagon_dasm_jumpr_31_conditional()
        {
            AssertCode("{ if (p0.new) jumpr\tr31; p0 = cmp.eq(r2,00000000) }", "C63F2059");
        }

        [Test]
        public void Hexagon_dasm_jump_predicated()
        {
            AssertCode("{ if (!p1) jump:nt\t000FFFF8 }", "FC61FF5C");
        }

        [Test]
        public void Hexagon_dasm_jump_conditional_application()
        {
            AssertCode("{ if (cmp.gtu(r4.new,r0)) jump:t\t00100024; r4 = add(r2,r3) }", "044302F3 10E00221");
        }

        [Test]
        public void Hexagon_dasm_ld_memd()
        {
            AssertCode("{ r17:r16 = memd(r30-8) }", "F07FDE97");
        }

        [Test]
        public void Hexagon_dasm_ld_memuh()
        {
            AssertCode("{ r1 = memuh(r0+6) }", "61C06091");
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
            AssertCode("{ r7 = mux(p1,FFFFFFD5,0000006F) }", "A77AB77A");
        }

        [Test]
        public void Hexagon_dasm_mpyi()
        {
            AssertCode("{ r17 = mpyi(r1,r0) }", "114001ED");
        }

        [Test]
        public void Hexagon_dasm_mpyu_64()
        {
            AssertCode("{ r1:r0 = mpyu(r1,r4) }", "004441E5");
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
        public void Hexagon_dasm_rte()
        {
            AssertCode("{ rte }", "00C0E057");
        }

        [Test]
        public void Hexagon_dasm_st_indexed()
        {
            AssertCode("{ memw(r17<<#2+0000E700) = r3 }", "9C43000080E391AD");
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
        public void Hexagon_dasm_setbit()
        {
            AssertCode("{ r2 = setbit(r2,00000010) }", "02D8C28C");
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
            AssertCode("{ p0 = tstbit(r0,00000000) }", "00C00085");
        }

        [Test]
        public void Hexagon_dasm_valignb_rr()
        {
            AssertCode("{ r7:r6 = valignb(r11:r10,r13:r12,p2) }", "464A0CC2");
        }

        [Test]
        public void Hexagon_dasm_vsplatb()
        {
            AssertCode("{ r7 = vsplatb(r1) }", "E740418C");
        }

        [Test]
        public void Hexagon_dasm_lsr_rr()
        {
            AssertCode("{ r1:r0 = lsr(r1:r0,00000030) }", "20700080");
        }

        [Test]
        public void Hexagon_dasm_longnew()
        {
            AssertCode("{ memb(gp+132) = r2.new; r2 = or(r3,and(r2,0000000F)) }", "E34142DA0044000028C4A048");
        }

        [Test]
        public void Hexagon_dasm_zxth()
        {
            AssertCode("{ r0 = zxth(r0) }", "0040C070");
        }

        [Test]
        public void Hexagon_dasm_long_offset()
        {
            AssertCode("{ r2 = memb(r2+00001B44) }", "6D40000002D1029D");
        }

        [Test]
        public void Hexagon_dasm_tlbp()
        {
            AssertCode("{ r5 = tlbp(r1) }", "05C0816C");
        }

        // 2032F0F6 E0004140 

    }
}
