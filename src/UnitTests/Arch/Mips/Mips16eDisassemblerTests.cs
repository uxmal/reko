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
    public class Mips16eDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;

        public Mips16eDisassemblerTests()
        {
            this.arch = new MipsBe32Architecture("mips-be-32");
            arch.LoadUserOptions(new Dictionary<string, object> { { "decoder", "mips16e" } });
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        protected override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Mips16eDisassembler(this.arch, rdr);
        }

        private void AssertCode(string sExp, string sHex)
        {
            var instr = DisassembleHexBytes(sHex);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Mips16eDis_generate()
        {
            var rnd = new Random(4711);
            var buf = new byte[100_000];
            rnd.NextBytes(buf);
            var mem = new MemoryArea(LoadAddress, buf);
            var rdr = new BeImageReader(mem, 0);
            var dasm = new Mips16eDisassembler(arch, rdr);
            dasm.Take(400).ToArray();
        }

        [Test]
        public void Mips16eDis_addi_sp()
        {
            AssertCode("addi\tsp,-000001A0", "63CC");
        }

        [Test]
        public void Mips16eDis_addiu()
        {
            AssertCode("addiu\tr17,r3,00000006", "4166");
        }

        [Test]
        public void Mips16eDis_addiu_sp()
        {
            AssertCode("addiu\tr17,sp,00000344", "01D1");
        }

        [Test]
        public void Mips16eDis_addiu8()
        {
            AssertCode("addiu\tr6,0000007E", "4E7E");
        }

        [Test]
        public void Mips16eDis_addu()
        {
            AssertCode("addu\tr5,r2,r4", "E295");
        }

        [Test]
        public void Mips16eDis_b()
        {
            AssertCode("b\t00100250", "1127");
        }

        [Test]
        public void Mips16eDis_bnez()
        {
            AssertCode("bnez\tr5,000FFFD6", "2DEA");
        }

        [Test]
        public void Mips16eDis_beqz()
        {
            AssertCode("beqz\tr3,001000D8", "236B");
        }

        [Test]
        public void Mips16eDis_btnez()
        {
            AssertCode("btnez\t00100018", "610B");
        }

        [Test]
        public void Mips16eDis_cmpi()
        {
            AssertCode("cmpi\tr6,0000004B", "764B");
        }

        [Test]
        public void Mips16eDis_jal_0()
        {
            AssertCode("jal\t00000000", "18000000");
        }

        [Test]
        public void Mips16eDis_jal_1()
        {
            AssertCode("jal\t0FFFFFFC", "1BFFFFFF");
        }

        [Test]
        public void Mips16eDis_jalx_0()
        {
            AssertCode("jalx\t00000000", "1C000000");
        }

        [Test]
        public void Mips16eDis_lb()
        {
            AssertCode("lb\tr7,000E(r7)", "86EE");
        }

        [Test]
        public void Mips16eDis_lbu()
        {
            AssertCode("lbu\tr5,001E(r5)", "A1BE");
        }

        [Test]
        public void Mips16eDis_lh()
        {
            AssertCode("lh\tr4,0010(r4)", "8888");
        }

        [Test]
        public void Mips16eDis_lhu()
        {
            AssertCode("lhu\tr2,000E(r2)", "AE47");
        }

        [Test]
        public void Mips16eDis_lwpc()
        {
            AssertCode("lw\tr4,0364(pc)", "B4D9");
        }

        [Test]
        public void Mips16eDis_lwpc_2()
        {
            AssertCode("lw\tr17,01F4(pc)", "B17D");
        }

        [Test]
        public void Mips16eDis_lwsp()
        {
            AssertCode("lw\tr4,03F0(sp)", "94FC");
        }

        [Test]
        public void Mips16eDis_li()
        {
            AssertCode("li\tr4,000000B3", "6CB3");
        }

        [Test]
        public void Mips16eDis_mflo()
        {
            AssertCode("mflo\tr17", "E992");
        }

        [Test]
        public void Mips16eDis_move_from_wide()
        {
            AssertCode("move\tr3,r26", "677A");
        }

        [Test]
        public void Mips16eDis_move_to_wide()
        {
            AssertCode("move\tr27,r2", "657A");
        }

        [Test]
        public void Mips16eDis_neg()
        {
            AssertCode("neg\tr4,r7", "ECEB");
        }

        [Test]
        public void Mips16eDis_save()
        {
            AssertCode("save\tra,r16,r17,+00000080", "64F0");
        }

        [Test]
        public void Mips16eDis_sb()
        {
            AssertCode("sb\tr3,0015(r3)", "C275");
        }

        [Test]
        public void Mips16eDis_sh()
        {
            AssertCode("sh\tr2,001A(r2)", "C84D");
        }

        [Test]
        public void Mips16eDis_sll()
        {
            AssertCode("sll\tr17,r7,+00000001", "31E4");
        }

        [Test]
        public void Mips16eDis_slti()
        {
            AssertCode("slti\tr16,000000B8", "50B8");
        }

        [Test]
        public void Mips16eDis_sltiu()
        {
            AssertCode("sltiu\tr6,0000000A", "5E0A");
        }

        [Test]
        public void Mips16eDis_sra()
        {
            AssertCode("sra\tr4,r16,+00000007", "341F");
        }

        [Test]
        public void Mips16eDis_srav()
        {
            AssertCode("srav\tr4,r3", "EB87");
        }

        [Test]
        public void Mips16eDis_srl()
        {
            AssertCode("srl\tr3,r7,+00000004", "33F2");
        }

        [Test]
        public void Mips16eDis_subu()
        {
            AssertCode("subu\tr7,r17,r5", "E1BF");
        }

        [Test]
        public void Mips16eDis_swrasp()
        {
            AssertCode("sw\tra,03CC(sp)", "62F3");
        }

        [Test]
        public void Mips16eDis_xor()
        {
            AssertCode("xor\tr2,r4", "EA8E");
        }

        ////////////////////////////////////////////////////////////////////////////////
        ///
    }
}
