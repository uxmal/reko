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
using Reko.Arch.XCore;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.XCore
{
    [TestFixture]
    public class XCore200DisassemblerTests : DisassemblerTestBase<XCoreInstruction>
    {
        private readonly XCore200Architecture arch;
        private readonly Address addr;

        public XCore200DisassemblerTests()
        {
            this.arch = new XCore200Architecture(CreateServiceContainer(), "xcore", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        // Avoid spamming the CI builds [Test]
        public void XCore200Dis_GenerateRandom()
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[65536]);
            var rnd = new Random(4711);
            rnd.NextBytes(mem.Bytes);
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(mem, 0));
            var instrs = dasm.ToArray();
        }

        private void AssertCode(string sExpected, string hexbytes)
        {
            var instr = base.DisassembleHexBytes(hexbytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }


        [Test]
        public void XCore200Dis_add()
        {
            AssertCode("add\tr4,r11,r3", "CF13");
        }

        [Test]
        public void XCore200Dis_add_r3_r3_r3()
        {
            AssertCode("add\tr3,r3,r3", "3F10");
        }

        [Test]
        public void XCore200Dis_add_r3_r7_r3()
        {
            AssertCode("add\tr3,r7,r3", "FF10");
        }

        [Test]
        public void XCore200Dis_add_r7_r3_r3()
        {
            AssertCode("add\tr7,r3,r3", "7F12");
        }

        [Test]
        public void XCore200Dis_addi()
        {
            AssertCode("addi\tr4,r5,00000001", "0593");
        }

        [Test]
        public void XCore200Dis_and()
        {
            AssertCode("and\tr0,r7,r6", "CE3F");
        }

        [Test]
        public void XCore200Dis_andnot()
        {
            AssertCode("andnot\tr11,r1", "2D2F");
        }

        [Test]
        public void XCore200Dis_bitrev()
        {
            AssertCode("bitrev\tr11,r7", "6F17");
        }


        [Test]
        public void XCore200Dis_blrb()
        {
            AssertCode("blrb\t000FF858", "D4D7");
        }

        [Test]
        public void XCore200Dis_blrf()
        {
            AssertCode("blrf\t0010000C", "06D0");
        }

        [Test]
        public void XCore200Dis_brbt()
        {
            AssertCode("brbt\tr10,000FFFDA", "A676");
        }

        [Test]
        public void XCore200Dis_brff()
        {
            AssertCode("brff\tr5,00100054", "6A79");
        }

        [Test]
        public void XCore200Dis_brft()
        {
            AssertCode("brft\tr7,0010005E", "EF71");
        }

        [Test]
        public void XCore200Dis_brfu()
        {
            AssertCode("brfu\t0010007C", "3E73");
        }


        [Test]
        public void XCore200Dis_byterev()
        {
            AssertCode("byterev\tr1,r3", "C706");
        }

        [Test]
        public void XCore200Dis_clz()
        {
            AssertCode("clz\tr4,r8", "E00E");
        }

        [Test]
        public void XCore200Dis_eef()
        {
            AssertCode("eef\tr3,r2", "DE2E");
        }

        [Test]
        public void XCore200Dis_eet()
        {
            AssertCode("eet\tr0,r5", "1127");
        }

        [Test]
        public void XCore200Dis_endin()
        {
            AssertCode("endin\tr6,r7", "DB97");
        }

        [Test]
        public void XCore200Dis_eq()
        {
            AssertCode("eq\tr2,r9,r7", "E731");
        }

        [Test]
        public void XCore200Dis_getst()
        {
            AssertCode("getst\tr1,r0", "D406");
        }

        [Test]
        public void XCore200Dis_ld16s()
        {
            AssertCode("ld16s\tr1,r6,r11", "5B81");
        }

        [Test]
        public void XCore200Dis_ld8u()
        {
            AssertCode("ld8u\tr1,r6,r1", "D988");
        }

        [Test]
        public void XCore200Dis_ldapb()
        {
            AssertCode("ldapb\t00100304", "82D9");
        }

        [Test]
        public void XCore200Dis_ldawdp()
        {
            AssertCode("ldawdp\tr1,00000001", "4160");
        }

        [Test]
        public void XCore200Dis_ldawsp()
        {
            AssertCode("ldawsp\tr2,0000002E", "AE64");
        }

        [Test]
        public void XCore200Dis_ldc()
        {
            AssertCode("ldc\tr0,0000002B", "2B68");
        }

        [Test]
        public void XCore200Dis_ldwcp()
        {
            AssertCode("ldwcp\tr2,00000011", "916C");
        }

        [Test]
        public void XCore200Dis_ldwdp()
        {
            AssertCode("ldwdp\tr3,00000004", "C458");
        }

        [Test]
        public void XCore200Dis_ldwi()
        {
            AssertCode("ldwi\tr0,r7,00000005", "0D09");
        }

        [Test]
        public void XCore200Dis_ldwsp()
        {
            AssertCode("ldwsp\tr6,00000036", "B65D");
        }

        [Test]
        public void XCore200Dis_lss()
        {
            AssertCode("lss\tr4,r5,r1", "05C3");
        }

        [Test]
        public void XCore200Dis_lsu()
        {
            AssertCode("lsu\tr2,r3,r4", "6CC8");
        }

        [Test]
        public void XCore200Dis_mkmsk()
        {
            AssertCode("mkmsk\tr0,r3", "C3A6");
        }

        [Test]
        public void XCore200Dis_neg()
        {
            AssertCode("neg\tr1,r4", "0497");
        }

        [Test]
        public void XCore200Dis_or()
        {
            AssertCode("or\tr3,r5,r5", "3541");
        }

        [Test]
        public void XCore200Dis_outcti()
        {
            AssertCode("outcti\tr6,r2", "9A4F");
        }

        [Test]
        public void XCore200Dis_outt()
        {
            AssertCode("outt\tr2,r4", "920F");
        }

        [Test]
        public void XCore200Dis_setd()
        {
            AssertCode("setd\tr1,r2", "D916");
        }

        [Test]
        public void XCore200Dis_shli()
        {
            AssertCode("shli\tr2,r3,00000020", "2CA0");
        }

        [Test]
        public void XCore200Dis_stwdp()
        {
            AssertCode("stwdp\tr6,00000018", "9851");
        }

        [Test]
        public void XCore200Dis_stwi_x()
        {
            AssertCode("stwi\tr7,r5,00000003", "3703");
        }

        [Test]
        public void XCore200Dis_stwsp()
        {
            AssertCode("stwsp\tr10,00000030", "B056");
        }

        [Test]
        public void XCore200Dis_stwi()
        {
            AssertCode("stwi\tr2,r4,00000002", "E200");
        }

        [Test]
        public void XCore200Dis_subi()
        {
            AssertCode("subi\tr3,r3,00000001", "3D98");
        }
   }
}