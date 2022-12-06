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
using Reko.Arch.OpenRISC;
using Reko.Arch.OpenRISC.Aeon;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonDisassemblerTests : DisassemblerTestBase<AeonInstruction>
    {
        private readonly AeonArchitecture arch;
        private readonly Address addrLoad;

        public AeonDisassemblerTests()
        {
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
            this.arch = new AeonArchitecture(CreateServiceContainer(), "aeon", new());
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void AeonDis_l_addi()
        {
            AssertCode("l.addi\tr3,r13,0x0", "fc 6d 00 00");
        }

        [Test]
        public void AeonDis_l_addi_two_operand()
        {
            AssertCode("l.addi?\tr1,-0x4", "9C 3C");
        }

        [Test]
        public void AeonDis_l_blti__()
        {
            AssertCode("l.blti?\tr4,0x8,000FFFE7", "D0 88 FF 3E");
        }

        [Test]
        public void AeonDis_l_jal()
        {
            AssertCode("l.jal\t000F17F9", "E7 FE 2F F2");
        }

        [Test]
        public void AeonDis_l_movi()
        {
            AssertCode("l.movi?\tr6,-0x1", "98 DF");
        }

        [Test]
        public void AeonDis_l_movhi()
        {
            AssertCode("l.movhi\tr7,0x52", "C0E00A41");
        }

        [Test]
        public void AeonDis_l_nop()
        {
            AssertCode("l.nop", "000000");
        }

        [Test]
        public void AeonDis_l_sh()
        {
            AssertCode("l.sh?\t0x345A(r7),r3", "EC 67 34 5B");
        }

        [Test]
        public void AeonDis_l_srai()
        {
            AssertCode("l.srai?\tr5,r7,0x1", "4C A7 0A");
        }
    }
}
