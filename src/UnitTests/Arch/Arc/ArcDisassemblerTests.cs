#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.Arc;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arc
{
    [TestFixture]
    public class ArcDisassemblerTests : DisassemblerTestBase<ArcInstruction>
    {
        private ARCompactArchitecture arch;

        public ArcDisassemblerTests()
        {
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new ARCompactArchitecture("arc");
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { "Endianness", "be" }
            });
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        [Test]
        public void ArcDasm_unimp_s()
        {
            AssertCode("unimp_s", "79E0");
        }

        [Test]
        public void ARCompactDis_push_s_reg()
        {
            AssertCode("push_s\tr13", "C5E1");
        }

        [Test]
        public void ARCompactDis_push_blink()
        {
            AssertCode("push_s\tblink", "C0F1");
        }

        [Test]
        public void ARCompactDis_st_aw()
        {
            AssertCode("st.aw\tfp,[sp,-4]", "1CFCB6C8");
        }

        [Test]
        public void ARCompactDis_mov_reg_reg()
        {
            AssertCode("mov\tfp,sp", "230A3700");
        }

        [Test]
        public void ARCompactDis_mov_s_reg_reg()
        {
            AssertCode("mov_s\tr14,r0", "7608");
        }

        [Test]
        public void ARCompactDis_bl()
        {
            AssertCode("bl\t00100308", "0B0A0000");
        }

        [Test]
        public void ARCompactDis_ld_ab()
        {
            AssertCode("ld.ab\tfp,[sp,4]", "1404341B");
        }

        [Test]
        public void ARCompactDis_ldb_x_ab()
        {
            AssertCode("ldb.x.ab\tfp,[sp,4]", "140434DB");
        }

        [Test]
        public void ARCompactDis_pop_s()
        {
            AssertCode("pop_s\tblink", "C0D1");
        }

        [Test]
        public void ARCompactDis_j_s_blink()
        {
            AssertCode("j_s\t[blink]", "7EE0");
        }

        [Test]
        public void ARCompactDis_sub_s_sp_sp_imm()
        {
            AssertCode("sub_s\tsp,sp,00000004", "C1A1");
        }

        [Test]
        public void ARCompactDis_ldw_s()
        {
            AssertCode("ldw_s\tr0,[r1,22]", "910B");
        }

        [Test]
        public void ARCompactDis_bic()
        {
            AssertCode("bic\tr0,r0,-00000009", "20860DFF");
        }

        [Test]
        public void ARCompactDis_cmp_s_imm()
        {
            AssertCode("cmp_s\tr0,00000000", "E080");
        }

        [Test]
        public void ARCompactDis_b_s()
        {
            AssertCode("b_s\t00100026", "F013");
        }

        [Test]
        public void ARCompactDis_bne_s()
        {
            AssertCode("bne_s\t00100006", "F403");
        }

        [Test]
        public void ARCompactDis_bmsk_s()
        {
            AssertCode("bmsk_s\tr0,r0,00000000", "B8C0");
        }

        [Test]
        public void ARCompactDis_mov_s()
        {
            AssertCode("mov_s\tr0,00000004", "D804");
        }

        [Test]
        public void ARCompactDis_b_far()
        {
            AssertCode("b\t00700800", "00010043");
        }

        [Test]
        public void ARCompactDis_b_d_far()
        {
            AssertCode("b.d\t00700800", "00010063");
        }

        [Test]
        public void ARCompactDis_lr_limm()
        {
            AssertCode("lr\tr0,[19088743]", "202A0F8001234567");
        }

        [Test]
        public void ARCompactDis_ld()
        {
            AssertCode("ld\tr0,[fp,-4]", "13FCB000");
        }

        [Test]
        public void ARCompactDis_ld_s()
        {
            AssertCode("ld_s\tr0,[r15,4]", "8702");
        }

        [Test]
        public void ARCompactDis_extb_s()
        {
            AssertCode("extb_s\tr0,r0", "780F");
        }

        [Test]
        public void ARCompactDis_sub_s()
        {
            AssertCode("sub_s\tr0,r0,r1", "7822");
        }

        [Test]
        public void ARCompactDis_lsr_s_1()
        {
            AssertCode("lsr_s\tr0,r0,+00000001", "781D");
        }

        [Test]
        public void ARCompactDis_asl_s_imm3()
        {
            AssertCode("asl_s\tr0,r14,00000002", "6E12");
        }

        [Test]
        public void ARCompactDis_bmsk_imm6()
        {
            AssertCode("bmsk\tr1,r0,00000000", "20530001");
        }

        [Test]
        public void ARCompactDis_bss()
        {
            AssertCode("bss\t001E2802", "00027150");
        }

        [Test]
        public void ARCompactDis_bhi()
        {
            AssertCode("bhi\t0010004C", "004C000D");
        }

        [Test]
        public void ARCompactDis_ld_aw()
        {
            AssertCode("ld.aw\tr1,[r0,0]", "20700401");
        }

        [Test]
        public void ARCompactDis_and()
        {
            AssertCode("and\tr1,r0,00000010", "20440401");
        }

        [Test]
        public void ARCompactDis_bl_s()
        {
            AssertCode("bl_s\t000FFFFC", "FFFF");
        }

        [Test]
        public void ARCompactDis_jl_d_MemAbs()
        {
            AssertCode("jl.d\t[1450709556]", "20220F80 5678 1234");
        }

        [Test]
        public void ARCompactDis_brhs()
        {
            AssertCode("brhs\tr23,r0,001000FE", "0FFF2005");
        }

        [Test]
        public void ARCompactDis_bleq()
        {
            AssertCode("bleq\t00100780", "0F800001");
        }

        [Test]
        public void ARCompactDis_j_blink()
        {
            AssertCode("j.d\t[blink]", "202007C0");
        }

        [Test]
        public void ARCompactDis_flag()
        {
            AssertCode("flag\tr1", "20690040");
        }

        [Test]
        public void ARCompactDis_j_d_blink()
        {
            AssertCode("j.d\t[blink]", "202107C0");
        }

        [Test]
        public void ARCompactDis_or()
        {
            AssertCode("or\tr0,r0,00000002", "20450080");
        }

        [Test]
        public void ARCompactRw_add_invalid()
        {
            AssertCode("Invalid", "20C07814"); // add.20  r56,r56,r32
        } 

        [Test]
        public void ARCompactDis_add_s()
        {
            AssertCode("add_s\tr0,r15,r13", "67B8");
        }

        [Test]
        public void ARCompactDis_asl_imm()
        {
            AssertCode("asl\tr3,0000000F,r1", "2E0070430000000F");
        }

        [Test]
        public void ARCompactDis_lsr_1op()
        {
            AssertCode("lsr.f\tlp_count,r4", "242FF102");
        }

        [Test]
        public void ARCompactDis_LPcc()
        {
            AssertCode("lpne\t00100024", "20E804A2");
        }

        [Test]
        public void ARCompactDis_jeq_d()
        {
            AssertCode("jeq.d\t[blink]", "20E007C1");
        }

  

        [Test]
        public void ARCompactDis_st_s()
        {
            AssertCode("st_s\tr0,[sp,8]", "C042");
        }

        [Test]
        public void ARCompactDis_ld_s_sp()
        {
            AssertCode("ld_s\tr1,[sp,8]", "C102");
        }

        [Test]
        public void ARCompactDis_ld_s_pcl()
        {
            AssertCode("ld_s\tr0,[pcl,76]", "D013");
        }

        [Test]
        public void ARCompactDis_add_reg_sp_imm()
        {
            AssertCode("add_s\tr1,sp,00000004", "C181");
        }

        [Test]
        public void ARCompactDis_cmp_ne()
        {
            // 21cc 8f82 0000 0201 	cmp.ne	r1,0x201
            AssertCode("cmp.ne\tr1,00000201", "21CC8F82 0000 0201");
        }

        [Test]
        public void ARCompactDis_mov_hi()
        {
            AssertCode("mov.hi\tr2,r15", "22CA03CD");
        }

        [Test]
        public void ARCompactDis_bset_ne()
        {
            AssertCode("bset.ne\tr14,r14,00000005", "26CF1162");
        }

        [Test]
        public void ARCompactDis_ldw_s_gp()
        {
            AssertCode("ldw_s\tr0,[gp,56]", "CC1C");
        }

        [Test]
        public void ARCompactDis_jl_s_reg()
        {
            AssertCode("jl_s\t[r2]", "7A40");
        }

        [Test]
        public void ARCompactDis_trap0()
        {
            AssertCode("trap0", "226F003F");
        }

        [Test]
        public void ARCompactDis_brne_s()
        {
            AssertCode("brne_s\tr0,+00000000,00100048", "E8A4");
        }

        [Test]
        public void ARCompactDis_breq_s()
        {
            AssertCode("breq_s\tr2,+00000000,000FFF90", "EA48");
        }

        [Test]
        public void ARCompactDis_sr()
        {
            AssertCode("sr\tr1,[]", "212B0000");
        }


        //////////////////////////////////////////////////
         
#if BORED




#endif
    }
}
