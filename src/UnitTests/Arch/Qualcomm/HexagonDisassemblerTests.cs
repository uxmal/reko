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
using Reko.Arch.Qualcomm;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Qualcomm
{
    public class HexagonDisassemblerTests : DisassemblerTestBase<HexagonPacket>
    {
        private HexagonArchitecture arch;
        private Address addrLoad;

        public HexagonDisassemblerTests()
        {
            this.arch = new HexagonArchitecture(CreateServiceContainer(), "hexagon");
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        // Reko: a decoder for the instruction 62000000 at address 01E08000 has not been implemented.
        [Test]
        public void Hexagon_dasm_62000000()
        {
            AssertCode("@@@", "62000000");
        }

        // Reko: a decoder for the instruction 00040000 at address 01E08004 has not been implemented.
        [Test]
        public void Hexagon_dasm_00040000()
        {
            AssertCode("@@@", "00040000");
        }
        // Reko: a decoder for the instruction 00004004 at address 01E08008 has not been implemented.
        [Test]
        public void Hexagon_dasm_00004004()
        {
            AssertCode("@@@", "00004004");
        }
        // Reko: a decoder for the instruction 003C0000 at address 01E0800C has not been implemented.
        [Test]
        public void Hexagon_dasm_003C0000()
        {
            AssertCode("@@@", "003C0000");
        }
        // Reko: a decoder for the instruction 0000805F at address 01E08010 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000805F()
        {
            AssertCode("@@@", "0000805F");
        }
        // Reko: a decoder for the instruction 0000C0F8 at address 01E08018 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000C0F8()
        {
            AssertCode("@@@", "0000C0F8");
        }
        // Reko: a decoder for the instruction 0000C011 at address 01E08020 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000C011()
        {
            AssertCode("@@@", "0000C011");
        }
        // Reko: a decoder for the instruction 013C0000 at address 01E08024 has not been implemented.
        [Test]
        public void Hexagon_dasm_013C0000()
        {
            AssertCode("@@@", "013C0000");
        }
        // Reko: a decoder for the instruction 0000801B at address 01E08028 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000801B()
        {
            AssertCode("@@@", "0000801B");
        }
        // Reko: a decoder for the instruction 0000C0B2 at address 01E08030 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000C0B2()
        {
            AssertCode("@@@", "0000C0B2");
        }
        // Reko: a decoder for the instruction 00004008 at address 01E08038 has not been implemented.
        [Test]
        public void Hexagon_dasm_00004008()
        {
            AssertCode("@@@", "00004008");
        }
        // Reko: a decoder for the instruction 00000085 at address 01E08040 has not been implemented.
        [Test]
        public void Hexagon_dasm_00000085()
        {
            AssertCode("@@@", "00000085");
        }
        // Reko: a decoder for the instruction 00008041 at address 01E08048 has not been implemented.
        [Test]
        public void Hexagon_dasm_00008041()
        {
            AssertCode("@@@", "00008041");
        }
        // Reko: a decoder for the instruction 0000C001 at address 01E08050 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000C001()
        {
            AssertCode("@@@", "0000C001");
        }
        // Reko: a decoder for the instruction 0000C06C at address 01E08058 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000C06C()
        {
            AssertCode("@@@", "0000C06C");
        }
        // Reko: a decoder for the instruction 0000001A at address 01E08060 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000001A()
        {
            AssertCode("@@@", "0000001A");
        }
        // Reko: a decoder for the instruction 0000800C at address 01E08068 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000800C()
        {
            AssertCode("@@@", "0000800C");
        }
        // Reko: a decoder for the instruction 00000000 at address 01E08088 has not been implemented.
        [Test]
        public void Hexagon_dasm_00000000()
        {
            AssertCode("@@@", "00000000");
        }
        // Reko: a decoder for the instruction 73160000 at address 01E0808C has not been implemented.
        [Test]
        public void Hexagon_dasm_73160000()
        {
            AssertCode("@@@", "73160000");
        }
        // Reko: a decoder for the instruction 00010040 at address 01E08090 has not been implemented.
        [Test]
        public void Hexagon_dasm_00010040()
        {
            AssertCode("@@@", "00010040");
        }
        // Reko: a decoder for the instruction C0050000 at address 01E08094 has not been implemented.
        [Test]
        public void Hexagon_dasm_C0050000()
        {
            AssertCode("@@@", "C0050000");
        }
        // Reko: a decoder for the instruction FF000000 at address 01E08098 has not been implemented.
        [Test]
        public void Hexagon_dasm_FF000000()
        {
            AssertCode("@@@", "FF000000");
        }
        // Reko: a decoder for the instruction C1050000 at address 01E0809C has not been implemented.
        [Test]
        public void Hexagon_dasm_C1050000()
        {
            AssertCode("@@@", "C1050000");
        }
        // Reko: a decoder for the instruction 0F000280 at address 01E080A0 has not been implemented.
        [Test]
        public void Hexagon_dasm_0F000280()
        {
            AssertCode("@@@", "0F000280");
        }
        // Reko: a decoder for the instruction A2050000 at address 01E080A4 has not been implemented.
        [Test]
        public void Hexagon_dasm_A2050000()
        {
            AssertCode("@@@", "A2050000");
        }
        // Reko: a decoder for the instruction 00008008 at address 01E080A8 has not been implemented.
        [Test]
        public void Hexagon_dasm_00008008()
        {
            AssertCode("@@@", "00008008");
        }
        // Reko: a decoder for the instruction 40880000 at address 01E080AC has not been implemented.
        [Test]
        public void Hexagon_dasm_40880000()
        {
            AssertCode("@@@", "40880000");
        }
        // Reko: a decoder for the instruction 0100C007 at address 01E080B0 has not been implemented.
        [Test]
        public void Hexagon_dasm_0100C007()
        {
            AssertCode("@@@", "0100C007");
        }
        // Reko: a decoder for the instruction 00380000 at address 01E080B4 has not been implemented.
        [Test]
        public void Hexagon_dasm_00380000()
        {
            AssertCode("@@@", "00380000");
        }
        // Reko: a decoder for the instruction 0C004045 at address 01E080B8 has not been implemented.
        [Test]
        public void Hexagon_dasm_0C004045()
        {
            AssertCode("@@@", "0C004045");
        }
        // Reko: a decoder for the instruction 20380000 at address 01E080BC has not been implemented.
        [Test]
        public void Hexagon_dasm_20380000()
        {
            AssertCode("@@@", "20380000");
        }
        // Reko: a decoder for the instruction 0D008051 at address 01E080C0 has not been implemented.
        [Test]
        public void Hexagon_dasm_0D008051()
        {
            AssertCode("@@@", "0D008051");
        }
        // Reko: a decoder for the instruction 0000000D at address 01E080C8 has not been implemented.
        [Test]
        public void Hexagon_dasm_0000000D()
        {
            AssertCode("@@@", "0000000D");
        }
        // Reko: a decoder for the instruction 01004019 at address 01E080D0 has not been implemented.
        [Test]
        public void Hexagon_dasm_01004019()
        {
            AssertCode("@@@", "01004019");
        }
        // Reko: a decoder for the instruction 0200C0D6 at address 01E080D8 has not been implemented.
        [Test]
        public void Hexagon_dasm_0200C0D6()
        {
            AssertCode("@@@", "0200C0D6");
        }
        // Reko: a decoder for the instruction 0900803C at address 01E080E0 has not been implemented.
        [Test]
        public void Hexagon_dasm_0900803C()
        {
            AssertCode("@@@", "0900803C");
        }
        // Reko: a decoder for the instruction 0A00C011 at address 01E080E8 has not been implemented.
        [Test]
        public void Hexagon_dasm_0A00C011()
        {
            AssertCode("@@@", "0A00C011");
        }
        // Reko: a decoder for the instruction 0B0000D3 at address 01E080F0 has not been implemented.
        [Test]
        public void Hexagon_dasm_0B0000D3()
        {
            AssertCode("@@@", "0B0000D3");
        }
        // Reko: a decoder for the instruction 00004014 at address 01E080F8 has not been implemented.
        [Test]
        public void Hexagon_dasm_00004014()
        {
            AssertCode("@@@", "00004014");
        }
        // Reko: a decoder for the instruction 09008043 at address 01E08100 has not been implemented.
        [Test]
        public void Hexagon_dasm_09008043()
        {
            AssertCode("@@@", "09008043");
        }
        // Reko: a decoder for the instruction 0A00C016 at address 01E08108 has not been implemented.
        [Test]
        public void Hexagon_dasm_0A00C016()
        {
            AssertCode("@@@", "0A00C016");
        }
        // Reko: a decoder for the instruction 000F00A0 at address 01E08110 has not been implemented.
        [Test]
        public void Hexagon_dasm_000F00A0()
        {
            AssertCode("@@@", "000F00A0");
        }
        // Reko: a decoder for the instruction A3090000 at address 01E08114 has not been implemented.
        [Test]
        public void Hexagon_dasm_A3090000()
        {
            AssertCode("@@@", "A3090000");
        }
        // Reko: a decoder for the instruction 1F000018 at address 01E08118 has not been implemented.
        [Test]
        public void Hexagon_dasm_1F000018()
        {
            AssertCode("@@@", "1F000018");
        }
        // Reko: a decoder for the instruction A4090000 at address 01E0811C has not been implemented.
        [Test]
        public void Hexagon_dasm_A4090000()
        {
            AssertCode("@@@", "A4090000");
        }
        // Reko: a decoder for the instruction 00000018 at address 01E08120 has not been implemented.
        [Test]
        public void Hexagon_dasm_00000018()
        {
            AssertCode("@@@", "00000018");
        }
        // Reko: a decoder for the instruction 84300000 at address 01E08124 has not been implemented.
        [Test]
        public void Hexagon_dasm_84300000()
        {
            AssertCode("@@@", "84300000");
        }
        // Reko: a decoder for the instruction 000000A0 at address 01E08128 has not been implemented.
        [Test]
        public void Hexagon_dasm_000000A0()
        {
            AssertCode("@@@", "000000A0");
        }
        // Reko: a decoder for the instruction 63200000 at address 01E0812C has not been implemented.
        [Test]
        public void Hexagon_dasm_63200000()
        {
            AssertCode("@@@", "63200000");
        }
        // Reko: a decoder for the instruction 00000078 at address 01E08138 has not been implemented.
        [Test]
        public void Hexagon_dasm_00000078()
        {
            AssertCode("@@@", "00000078");
        }
        // Reko: a decoder for the instruction 00240000 at address 01E0813C has not been implemented.
        [Test]
        public void Hexagon_dasm_00240000()
        {
            AssertCode("@@@", "00240000");
        }
        // Reko: a decoder for the instruction 03100000 at address 01E08148 has not been implemented.
        [Test]
        public void Hexagon_dasm_03100000()
        {
            AssertCode("@@@", "03100000");
        }
        // Reko: a decoder for the instruction 84180000 at address 01E0814C has not been implemented.
        [Test]
        public void Hexagon_dasm_84180000()
        {
            AssertCode("@@@", "84180000");
        }
        // Reko: a decoder for the instruction 01000200 at address 01E08150 has not been implemented.
        [Test]
        public void Hexagon_dasm_01000200()
        {
            AssertCode("@@@", "01000200");
        }
        // Reko: a decoder for the instruction A5140000 at address 01E0815C has not been implemented.
        [Test]
        public void Hexagon_dasm_A5140000()
        {
            AssertCode("@@@", "A5140000");
        }
        // Reko: a decoder for the instruction 0100C00B at address 01E08160 has not been implemented.
        [Test]
        public void Hexagon_dasm_0100C00B()
        {
            AssertCode("@@@", "0100C00B");
        }
        // Reko: a decoder for the instruction 40380000 at address 01E08164 has not been implemented.
        [Test]
        public void Hexagon_dasm_40380000()
        {
            AssertCode("@@@", "40380000");
        }
        // Reko: a decoder for the instruction 0200400C at address 01E08168 has not been implemented.
        [Test]
        public void Hexagon_dasm_0200400C()
        {
            AssertCode("@@@", "0200400C");
        }
        // Reko: a decoder for the instruction 00004010 at address 01E08170 has not been implemented.
        [Test]
        public void Hexagon_dasm_00004010()
        {
            AssertCode("@@@", "00004010");
        }
        // Reko: a decoder for the instruction 00300000 at address 01E08178 has not been implemented.
        [Test]
        public void Hexagon_dasm_00300000()
        {
            AssertCode("@@@", "00300000");
        }
        // Reko: a decoder for the instruction A5180000 at address 01E0817C has not been implemented.
        [Test]
        public void Hexagon_dasm_A5180000()
        {
            AssertCode("@@@", "A5180000");
        }
        // Reko: a decoder for the instruction 00400000 at address 01E08188 has not been implemented.
        [Test]
        public void Hexagon_dasm_00400000()
        {
            AssertCode("@@@", "00400000");
        }
        // Reko: a decoder for the instruction 00000008 at address 01E08190 has not been implemented.
        [Test]
        public void Hexagon_dasm_00000008()
        {
            AssertCode("@@@", "00000008");
        }
        // Reko: a decoder for the instruction A5300000 at address 01E08194 has not been implemented.
        [Test]
        public void Hexagon_dasm_A5300000()
        {
            AssertCode("@@@", "A5300000");
        }
        // Reko: a decoder for the instruction A1090000 at address 01E081A4 has not been implemented.
        [Test]
        public void Hexagon_dasm_A1090000()
        {
            AssertCode("@@@", "A1090000");
        }
        // Reko: a decoder for the instruction 1F000000 at address 01E081A8 has not been implemented.
        [Test]
        public void Hexagon_dasm_1F000000()
        {
            AssertCode("@@@", "1F000000");
        }
        // Reko: a decoder for the instruction 44240000 at address 01E081B4 has not been implemented.
        [Test]
        public void Hexagon_dasm_44240000()
        {
            AssertCode("@@@", "44240000");
        }
        // Reko: a decoder for the instruction 63140000 at address 01E081C4 has not been implemented.
        [Test]
        public void Hexagon_dasm_63140000()
        {
            AssertCode("@@@", "63140000");
        }
        // Reko: a decoder for the instruction 00004020 at address 01E081C8 has not been implemented.
        [Test]
        public void Hexagon_dasm_00004020()
        {
            AssertCode("@@@", "00004020");
        }
        // Reko: a decoder for the instruction 435C0000 at address 01E081CC has not been implemented.
        [Test]
        public void Hexagon_dasm_435C0000()
        {
            AssertCode("@@@", "435C0000");
        }
        // Reko: a decoder for the instruction 01004020 at address 01E081D0 has not been implemented.
        [Test]
        public void Hexagon_dasm_01004020()
        {
            AssertCode("@@@", "01004020");
        }
        // Reko: a decoder for the instruction 00008020 at address 01E081D8 has not been implemented.
        [Test]
        public void Hexagon_dasm_00008020()
        {
            AssertCode("@@@", "00008020");
        }
        // Reko: a decoder for the instruction 01008020 at address 01E081E0 has not been implemented.
        [Test]
        public void Hexagon_dasm_01008020()
        {
            AssertCode("@@@", "01008020");
        }

    }
}
