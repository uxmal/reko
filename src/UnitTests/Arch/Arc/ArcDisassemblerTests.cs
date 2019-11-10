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

        //////////////////////////////////////////////////

        // Reko: a decoder for ARCompact instruction 1404341B at address 00000022 has not been implemented. (LD register + offset Delayed load 32-bit)

        // Reko: a decoder for ARCompact instruction C0D1 at address 00000026 has not been implemented. (POP_S)


        // Reko: a decoder for ARCompact instruction C6C1 at address 00000028 has not been implemented. (POP_S)

        // Reko: a decoder for ARCompact instruction 7EE0 at address 0000002C has not been implemented. (j_s [blink])


        // Reko: a decoder for ARCompact instruction 0000C5E1 at address 0000002E has not been implemented. (Branch Conditionally)

        // Reko: a decoder for ARCompact instruction 36C01404 at address 00000020 has not been implemented. (06 op  a,b,c ARC 32-bit extension instructions 32-bit)
        [Test]
        public void ARCompactDis_36C01404()
        {
            AssertCode("@@@", "36C01404");
        }

        // Reko: a decoder for ARCompact instruction 341BC0D1 at address 00000024 has not been implemented. (06 op  a,b,c ARC 32-bit extension instructions 32-bit)
        [Test]
        public void ARCompactDis_341BC0D1()
        {
            AssertCode("@@@", "341BC0D1");
        }

        // Reko: a decoder for ARCompact instruction C1A1 at address 0000003E has not been implemented. (ADD_S/SUB_S)

        // Reko: a decoder for ARCompact instruction 910B at address 0000004C has not been implemented. (LDW_S c,[b,u6] Delayed load (16-bit aligned offset) 16-bit)

        // Reko: a decoder for ARCompact instruction 20860DFF at address 0000004E has not been implemented. (REG_S12IMM)


        // Reko: a decoder for ARCompact instruction E080 at address 00000052 has not been implemented. (ADD_S / CMP_S b,u7 Add/compare immediate 16-bit)
        [Test]
        public void ARCompactDis_E080()
        {
            AssertCode("@@@", "E080");
        }

        // Reko: a decoder for ARCompact instruction F403 at address 00000054 has not been implemented. (Bcc_S s10/s7 Branch conditionally 16-bit)
        [Test]
        public void ARCompactDis_F403()
        {
            AssertCode("@@@", "F403");
        }

        // Reko: a decoder for ARCompact instruction D804 at address 00000056 has not been implemented. (MOV_S b,u8 Move immediate 16-bit)
        [Test]
        public void ARCompactDis_D804()
        {
            AssertCode("@@@", "D804");
        }

        // Reko: a decoder for ARCompact instruction F013 at address 00000058 has not been implemented. (Bcc_S s10/s7 Branch conditionally 16-bit)
        [Test]
        public void ARCompactDis_F013()
        {
            AssertCode("@@@", "F013");
        }

        // Reko: a decoder for ARCompact instruction 202A0F80 at address 0000005A has not been implemented. (REG_REG)
        [Test]
        public void ARCompactDis_202A0F80()
        {
            AssertCode("@@@", "202A0F80");
        }

        // Reko: a decoder for ARCompact instruction 00010043 at address 0000005E has not been implemented. (Branch Unconditional Far)
        [Test]
        public void ARCompactDis_00010043()
        {
            AssertCode("@@@", "00010043");
        }

        // Reko: a decoder for ARCompact instruction 13FCB000 at address 00000066 has not been implemented. (LD register + offset Delayed load 32-bit)
        [Test]
        public void ARCompactDis_13FCB000()
        {
            AssertCode("@@@", "13FCB000");
        }

        // Reko: a decoder for ARCompact instruction B8C0 at address 0000006A has not been implemented. (OP_S b,b,u5 Shift/subtract/bit ops 16-bit)
        [Test]
        public void ARCompactDis_B8C0()
        {
            AssertCode("@@@", "B8C0");
        }

        // Reko: a decoder for ARCompact instruction F404 at address 0000006E has not been implemented. (Bcc_S s10/s7 Branch conditionally 16-bit)
        [Test]
        public void ARCompactDis_F404()
        {
            AssertCode("@@@", "F404");
        }

        // Reko: a decoder for ARCompact instruction 00431BFC at address 00000060 has not been implemented. (Branch Unconditional Far)
        [Test]
        public void ARCompactDis_00431BFC()
        {
            AssertCode("@@@", "00431BFC");
        }

        // Reko: a decoder for ARCompact instruction F007 at address 00000072 has not been implemented. (Bcc_S s10/s7 Branch conditionally 16-bit)
        [Test]
        public void ARCompactDis_F007()
        {
            AssertCode("@@@", "F007");
        }
    }
}
