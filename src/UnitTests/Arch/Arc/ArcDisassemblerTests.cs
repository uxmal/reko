#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
            AssertCode("cmp_s\tr0,r0,00000000", "E080");
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

        //////////////////////////////////////////////////

#if BORED
#endif
    }
}
