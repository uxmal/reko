#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core;
using Reko.Environments.Gameboy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Environments.Gameboy
{
    [TestFixture]
    public class LR35902DisassemblerTests : Arch.DisassemblerTestBase<GameboyInstruction>
    {
        private readonly GameboyArchitecture arch;
        private readonly Address addr;

        public LR35902DisassemblerTests()
        {
            this.arch = new GameboyArchitecture(CreateServiceContainer(), "lr35902", new Dictionary<string, object>());
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void GameboyDis_ld_r_r()
        {
            AssertCode("ld\tb,c", "41");
        }

        [Test]
        public void GameboyDis_ld_a_Mem()
        {
            AssertCode("ld\ta,(de)", "1A");
        }

        [Test]
        public void GameboyDis_ld_a_Mem_inc()
        {
            AssertCode("ld\ta,(hl+)", "2A");
        }

        [Test]
        public void GameboyDis_ld_Mem_dec_a()
        {
            AssertCode("ld\t(hl-),a", "32");
        }

        [Test]
        public void GameboyDis_jr_nc()
        {
            AssertCode("jr\tnc,0100", "30FE");
        }

        [Test]
        public void GameboyDis_stop()
        {
            AssertCode("stop", "10");
        }

        [Test]
        public void GameboyDis_bit()
        {
            AssertCode("bit\t4h,(hl)", "CB66");
        }

        [Test]
        public void GameboyDis_ld_abs()
        {
            AssertCode("ld\t(1234h),a", "EA3412");
        }

        [Test]
        public void GameboyDis_ld_bc_imm()
        {
            AssertCode("ld\tbc,0FEDCh", "01DCFE");
        }

        [Test]
        public void GameboyDis_ldh()
        {
            AssertCode("ldh\ta,(0A7h)", "F0A7");
        }

        [Test]
        public void GameboyDis_ld_Mem_c()
        {
            AssertCode("ld\ta,(c)", "F2");
        }

        [Test]
        public void GameboyDis_jp_z()
        {
            AssertCode("jp\tz,BA98", "CA98BA");
        }

        [Test]
        public void GameboyDis_add_sp_signed_byte()
        {
            AssertCode("add\tsp,-4h", "E8FC");
        }

        [Test]
        public void GameboyDis_ld_hl_sp_signed_byte()
        {
            AssertCode("ld\thl,sp-4h", "F8FC");
        }

        [Test]
        public void GameboyDis_add_hl_sp()
        {
            AssertCode("add\thl,sp", "39");
        }
    }
}
