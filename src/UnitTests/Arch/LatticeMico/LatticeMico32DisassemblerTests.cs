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
using Reko.Arch.LatticeMico;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.LatticeMico
{
    [TestFixture]
    public class LatticeMico32DisassemblerTests : DisassemblerTestBase<LatticeMico32Instruction>
    {
        private LatticeMico32Architecture arch;

        public LatticeMico32DisassemblerTests()
        {
            this.arch = new LatticeMico32Architecture("latticeMico32");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        private void Assert_HexBytes(string sExpected, string sHexBytes)
        {
            var instr = base.DisassembleHexBytes(sHexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void Lm32Dis_add()
        {
            Assert_HexBytes("add\tr15,r2,r22", "B4567800");
        }

        [Test]
        public void Lm32Dis_add_invalid()
        {
            Assert_HexBytes("Invalid", "B4567C00");
        }

        [Test]
        public void Lm32Dis_addi()
        {
            Assert_HexBytes("addi\tr22,r2,FFFFF800", "3456F800");
        }

        [Test]
        public void Lm32Dis_b_ra()
        {
            Assert_HexBytes("b\tra", "C3A00000");
        }

        [Test]
        public void Lm32Dis_be()
        {
            Assert_HexBytes("be\tr2,r13,000FFFFC", "45A2FFFF");
        }

        [Test]
        public void Lm32Dis_calli()
        {
            Assert_HexBytes("calli\t000FFFFC", "FBFFFFFF");
        }

        [Test]
        public void Lm32Dis_lb()
        {
            Assert_HexBytes("lb\tr2,(r9-1)", "1122FFFF");
        }

        [Test]
        public void Lm32Dis_sextb()
        {
            Assert_HexBytes("sextb\tr8,r3", "B0604000");
        }

        [Test]
        public void Lm32Dis_sh()
        {
            Assert_HexBytes("sh\t(r3-16),r2", "0C62FFF0");
        }
    }
}
