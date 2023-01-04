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
using Reko.Arch.Loongson;
using Reko.Core;

namespace Reko.UnitTests.Arch.Loongson
{
    [TestFixture]
    public class LoongArchDisassemblerTests : DisassemblerTestBase<LoongArchInstruction>
    {
        public LoongArchDisassemblerTests()
        {
            this.Architecture = new LoongArch(CreateServiceContainer(), "loongArch", new());
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void LoongarchDis_add_d()
        {
            AssertCode("add.d\t$r12,$r12,$r13", "8CB51000");
        }

        [Test]
        public void LoongarchDis_alsl_w()
        {
            AssertCode("alsl.w\t$r20,$r14,$r20,+00000001", "D4D10400");
        }

        [Test]
        public void LoongarchDis_alsl_wu()
        {
            AssertCode("alsl.wu\t$r25,$r1,$r19,+00000000", "394C0600");
        }

        [Test]
        public void LoongarchDis_andi()
        {
            AssertCode("andi\t$r13,$r12,00000003", "8D0D4003");
        }

        [Test]
        public void LoongarchDis_b()
        {
            AssertCode("b\tFC2C4F58", "075B4F50");
        }

        [Test]
        public void LoongarchDis_beqz()
        {
            AssertCode("beqz\t$r19,FFE56430", "75326441");
        }

        [Test]
        public void LoongarchDis_bl()
        {
            AssertCode("bl\t001009D4", "00D40954");
        }

        [Test]
        public void LoongarchDis_bne()
        {
            AssertCode("bne\t$r24,$r25,000FFDC4", "19C7FD5F");
        }

        [Test]
        public void LoongarchDis_bstrins_d()
        {
            AssertCode("bstrins.d\t$r30,$r16,00000007,0000000E", "1E3A8700");
        }

        [Test]
        public void LoongarchDis_bstrpick_d()
        {
            AssertCode("bstrpick.d\t$r20,$r4,0000003A,0000003B", "94ECFA00");
        }

        [Test]
        public void LoongarchDis_bstrpick_w()
        {
            AssertCode("bstrpick.w\t$r14,$r14,00000008,00000008", "CEA16800");
        }

        [Test]
        public void LoongarchDis_cacop()
        {
            AssertCode("cacop\t00000008,$r19,-000001CD", "68CE3806");
        }

        [Test]
        public void LoongarchDis_csrxchg()
        {
            AssertCode("csrxchg\t$r31,00000FE7", "FF9D3F04");
        }

        [Test]
        public void LoongarchDis_div_d()
        {
            AssertCode("div.d\t$r15,$r15,$r16", "EF412200");
        }

        [Test]
        public void LoongarchDis_idle()
        {
            AssertCode("idle\t00007C68", "68FC4806");
        }

        [Test]
        public void LoongarchDis_ld_d()
        {
            AssertCode("ld.d\t$r4,$r13,+00000000", "A401C028");
        }

        [Test]
        public void LoongarchDis_ld_w()
        {
            AssertCode("ld.w\t$r13,$r12,+00000004", "8D118028");
        }

        [Test]
        public void LoongarchDis_ldx_d()
        {
            AssertCode("ldx.d\t$r13,$r12,$r13", "8D350C38");
        }

        [Test]
        public void LoongarchDis_lu12i()
        {
            AssertCode("lu12i.w\t$r5,+0000FFFF", "E5FF1F14");
        }

        [Test]
        public void LoongarchDis_lu32i_d()
        {
            AssertCode("lu32i.d\t$r12,+00000001", "2C000016");
        }

        [Test]
        public void LoongarchDis_lu52i()
        {
            AssertCode("lu52i.d\t$r13,$r0,+00000330", "0DC00C03");
        }

        [Test]
        public void LoongarchDis_mul_d()
        {
            AssertCode("mul.d\t$r15,$r14,$r15", "CFBD1D00");
        }

        [Test]
        public void LoongarchDis_or()
        {
            AssertCode("or\t$r12,$r12,$r24", "8C611500");
        }

        [Test]
        public void LoongarchDis_ori()
        {
            AssertCode("ori\t$r4,$r4,000001D0", "84408703");
        }

        [Test]
        public void LoongarchDis_preld()
        {
            AssertCode("preld\t0000001D,$r8,+000005CF", "1D3DD72A");
        }

        [Test]
        public void LoongarchDis_rdtime_d()
        {
            AssertCode("rdtime.d\t$r16,$r11", "70690000");
        }

        [Test]
        public void LoongarchDis_sll_d()
        {
            AssertCode("sll.d\t$r23,$r12,$r23", "97DD1800");
        }

        [Test]
        public void LoongarchDis_slli_d()
        {
            AssertCode("slli.d\t$r15,$r15,0000002A", "EFA94100");
        }

        [Test]
        public void LoongarchDis_st_d()
        {
            AssertCode("st.d\t$r13,$r12,+00000020", "8D81C029");
        }

        [Test]
        public void LoongarchDis_st_w()
        {
            AssertCode("st.w\t$r13,$r12,+00000004", "8D118029");
        }

        [Test]
        public void LoongarchDis_xori()
        {
            AssertCode("xori\t$r13,$r0,00000FFF", "0DFCFF03");
        }
    }
}