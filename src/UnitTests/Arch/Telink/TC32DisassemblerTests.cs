#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Telink;
using Reko.Core;

namespace Reko.UnitTests.Arch.Telink
{
    [TestFixture]
    public class TC32DisassemblerTests : DisassemblerTestBase<TC32Instruction>
    {
        private readonly TC32Architecture arch;
        private readonly Address addr = Address.Ptr32(0x10_0000);
        
        public TC32DisassemblerTests()
        {
            this.arch = new TC32Architecture(CreateServiceContainer(), "tc32", new());
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            var sActual = instr.ToString();
            Assert.AreEqual(sExpected, sActual);
        }

        [Test]
        public void TC32Dasm_tcmp()
        {
            AssertCode("tcmp\tr3,#0", "00AB");
        }

        [Test]
        public void TC32Dasm_tjcs()
        {
            AssertCode("tjcs\t00100010", "08C2");
        }

        [Test]
        public void TC32Dasm_tjne()
        {
            AssertCode("tjne\t00100032", "19C1");
        }

        [Test]
        public void TC32Dasm_tloadr_pc()
        {
            AssertCode("tloadr\tr2,[pc+8]", "020A");
        }

        [Test]
        public void TC32Dasm_tloadrb()
        {
            AssertCode("tloadrb\tr3,[r7]", "3B48");
        }

        [Test]
        public void TC32Dasm_tpush_lr()
        {
            AssertCode("tpush\tlr", "F065");
        }


        /*
// ................0110010:::::.... TC32
// ................0000111:....:::: TC32
// ................0100100...:::.:: TC32
// ................1010101:........ TC32
// ................1100000:...::..: TC32
// ................0000110.....:::. TC32
// ................0101100...:...:: TC32
// ................0000111.....:::. TC32
// ................0000110:....:::. TC32
// ................1110101::.:.::.: TC32
// ................1110000.:.:.::.: TC32
// ................1011110:.......: TC32
// ................0000001.:.:.:.:: TC32
// ................1100001.....:... TC32
// ................1011001:.......: TC32
// ................0101000...:...:: TC32
// ................1111000.:..::.:: TC32
// ................0110010:::::.... TC32
// ................0000111:....:::: TC32
// ................0100100...:::.:: TC32
// ................1010101:........ TC32
// ................1100000:...::..: TC32
// ................0000110.....:::. TC32
// ................0101100...:...:: TC32
// ................0000111.....:::. TC32
// ................0000110:....:::. TC32
// ................1110101::.:.::.: TC32
// ................1110000.:.:.::.: TC32
// ................1011110:.......: TC32
// ................0000001.:.:.:.:: TC32
// ................1100001.....:... TC32
// ................1011001:.......: TC32
// ................0101000...:...:: TC32
// ................1111000.:..::.:: TC32
         */
    }
}
