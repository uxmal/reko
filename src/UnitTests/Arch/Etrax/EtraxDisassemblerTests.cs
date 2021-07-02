#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Etrax;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch.Etrax
{
    [TestFixture]
    public class EtraxDisassemblerTests : DisassemblerTestBase<EtraxInstruction>
    {
        private readonly EtraxArchitecture arch;
        private readonly Address addr;

        public EtraxDisassemblerTests()
        {
            this.arch = new EtraxArchitecture(CreateServiceContainer(), "etrax", null);
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void EtraxDis_gen()
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x0010_0000), new byte[10000]);
            var rnd = new Random(1147);
            rnd.NextBytes(mem.Bytes);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr).ToArray();
        }

        [Test]
        public void EtraxDis_nop()
        {
            AssertCode("nop", "0F05");
        }

        [Test]
        public void EtraxDis_not()
        {
            AssertCode("not\tr0", "7087");
        }

        [Test]
        public void EtraxDis_setf()
        {
            AssertCode("setf\tMBINZC", "0011BDE5");
        }

        [Test]
        public void EtraxDis_swap_nwbr()
        {
            AssertCode("swapnwbr\tr0", "70F7");
        }

        [Test]
        public void EtraxDis_and_b_disp()
        {
            AssertCode("and.b\t[sp+0x78563412],r2", "5FED 1234 5678 032F");
        }

        [Test]
        public void EtraxDis_and_b_index()
        {
            AssertCode("and.b\t[sp+r0:b],r2", "4E05 032F");
        }

        [Test]
        public void EtraxDis_and_b_postinc()
        {
            AssertCode("and.b\t[r3+],r2", "032F");
        }

        [Test]
        public void EtraxDis_add_b_r_r()
        {
            AssertCode("add.b\tr3,r1", "0316");
        }

        [Test]
        public void EtraxDis_add_d_r_r()
        {
            AssertCode("add.d\tr3,r1", "2316");
        }

        [Test]
        public void EtraxDis_bge()
        {
            AssertCode("bge\t000FFFFE", "FDA0");
        }

        [Test]
        public void EtraxDis_bgt_w()
        {
            AssertCode("bgt\t00100000", "F0CD FCFF");
        }

        [Test]
        public void EtraxDis_ble()
        {
            AssertCode("ble\t00100000", "FFD0");
        }

        [Test]
        public void EtraxDis_bne()
        {
            AssertCode("bne\t00100100", "FE20");
        }

        [Test]
        public void EtraxDis_btst()
        {
            AssertCode("btst\tr10,r9", "00B1FA94");
        }

        [Test]
        public void EtraxDis_bvs()
        {
            AssertCode("bvs\t00100002", "0050");
        }

        [Test]
        public void EtraxDis_clearf()
        {
            AssertCode("clearf\tBIXNZVC", "FFF5");
        }

        [Test]
        public void EtraxDis_jir_indirect()
        {
            AssertCode("jir\t[r9+0x4]", "04913FAD");
        }

        [Test]
        public void EtraxDis_jsr()
        {
            AssertCode("jsr\t00100044", "3FBD42000000");
        }

        [Test]
        public void EtraxDis_jsr_reg()
        {
            AssertCode("jsr\tr1", "0191B1B9");
        }

        [Test]
        public void EtraxDis_jsr_rs()
        {
            AssertCode("jsr\tr0", "B0B9");
        }

        [Test]
        public void EtraxDis_jump_rs()
        {
            AssertCode("jump\tr7", "B709");
        }

        [Test]
        public void EtraxDis_jump()
        {
            AssertCode("jump\t0010000C", "3F 0D 0A 00 00 00 ");
        }

        [Test]
        public void EtraxDis_jump_indirect()
        {
            AssertCode("jump\t[r0]", "30 09 ");
        }

        [Test]
        public void EtraxDis_lz()
        {
            AssertCode("lz\tr12,r5", "3C57");
        }

        [Test]
        public void EtraxDis_move_b_double_indirect()
        {
            AssertCode("move.b\t[[r7]],r4", "7769424A");
        }

        [Test]
        public void EtraxDis_move_d_abs()
        {
            AssertCode("move.d\t[0000001E],r0", "7F 0D 1E 00 00 00 60 0a");
        }

        [Test]
        public void EtraxDis_move_ps()
        {
            AssertCode("move\tsrp,[sp-0x4]", "FCE17EBE");
        }

        [Test]
        public void EtraxDis_movem_store()
        {
            AssertCode("movem\tr8,[sp]", "FE8B");
        }

        [Test]
        public void EtraxDis_or_d_imm()
        {
            AssertCode("or.d\t0xC0000400,r2", "6F2F 0004 00C0");
        }

        [Test]
        public void EtraxDis_pop_sp()
        {
            AssertCode("move\t[sp+],srp", "3EBE");
        }

        [Test]
        public void EtraxDis_rbf()
        {
            AssertCode("rbf\t[r11]", "00B13B3F");
        }

        [Test]
        public void EtraxDis_ret()
        {
            AssertCode("ret", "7FB6");
        }

        [Test]
        public void EtraxDis_sbfs()
        {
            AssertCode("sbfs\t[r3+0x12]", "1231723B");
        }

        [Test]
        public void EtraxDis_sub_w_mem_indirect()
        {
            AssertCode("sub.w\t[r4],r1", "941A");
        }
    }
}
