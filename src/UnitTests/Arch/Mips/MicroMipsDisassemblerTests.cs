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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MicroMipsDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture("mips-be-micro");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new MicroMipsDisassembler(this.arch, rdr);
        }

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        private void Given_Mips64Architecture()
        {
            this.arch = new MipsBe64Architecture("mips-be-micro");
        }

        [Test]
        public void uMipsDis_Generate()
        {
            var ab = new byte[1000];
            var rnd = new Random(0x4711);
            rnd.NextBytes(ab);
            var mem = new MemoryArea(Address.Ptr32(0x00100000), ab);
            var rdr = new BeImageReader(mem, 0);
            var dasm = new MicroMipsDisassembler(arch, rdr);
            foreach (var instr in dasm)
            {
            }
        }

        [Test]
        public void uMipsDis_addiur1sp()
        {
            AssertCode("addiur1sp\tr16,00000004", "6C03");
        }

        [Test]
        public void uMipsDis_andi32()
        {
            AssertCode("andi32\tr6,r28,0000AAAA", "D0DCAAAA");
        }

        [Test]
        public void uMipsDis_aui()
        {
            AssertCode("aui\tr18,r12,FFFFFFFE", "124CFFFE");
        }

        [Test]
        public void uMipsDis_bc()
        {
            AssertCode("bc\t00100000", "97FFFFFE");
        }

        [Test]
        public void uMipsDis_bc16()
        {
            AssertCode("bc16\t000FFF08", "CF83");
        }

        [Test]
        public void uMipsDis_bnezc16()
        {
            AssertCode("bnezc16\tr3,000FFFCC", "A1E5");
        }

        [Test]
        public void uMipsDis_lbu16()
        {
            AssertCode("lbu16\tr4,000A(r3)", "0A3A");
        }

        [Test]
        public void uMipsDis_lbu32()
        {
            AssertCode("lbu32\tr25,-5556(r9)", "1729AAAA");
        }

        [Test]
        public void uMipsDis_ldc132()
        {
            AssertCode("ldc132\tf10,-5556(r8)", "BD48AAAA");
        }

        [Test]
        public void uMipsDis_ori32()
        {
            AssertCode("ori32\tr6,r11,0000AAAA", "50CBAAAA");
        }

        [Test]
        public void uMipsDis_sb32()
        {
            AssertCode("sb32\tr18,4242(r1)", "1A414242");
        }

        [Test]
        public void uMipsDis_sd32()
        {
            Given_Mips64Architecture();
            AssertCode("sd32\tr15,-5556(r27)", "D9FBAAAA");
        }

        [Test]
        public void uMipsDis_sdc132()
        {
            AssertCode("sdc132\tf22,-5556(r25)", "BAD9AAAA");
        }

        [Test]
        public void uMipsDis_sh16()
        {
            AssertCode("sh16\tr7,-A80C(r0)", "ABFA");
        }

        [Test]
        public void uMipsDis_sw16()
        {
            AssertCode("sw16\tr3,0004(r4)", "E9C1");
        }

        [Test]
        public void uMipsDis_sw32()
        {
            AssertCode("sw32\tr20,-5556(r8)", "FA88AAAA");
        }

        [Test]
        public void uMipsDis_xori32()
        {
            AssertCode("xori32\tr30,r5,0000AAAA", "73C5AAAA");
        }
    }
}
