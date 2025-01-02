#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Altera;
using Reko.Arch.Altera.Nios2;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Altera
{
    [TestFixture]
    public class Nios2DisassemblerTests : DisassemblerTestBase<Nios2Instruction>
    {
        private readonly Nios2Architecture arch;
        private readonly Address addr;

        public Nios2DisassemblerTests()
        {
            this.arch = new Nios2Architecture(CreateServiceContainer(), "nios2", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;
        
        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string hexString)
        {
            var instr = DisassembleHexBytes(hexString);
            Assert.AreEqual(sExp, instr.ToString());
        }

        //$TODO: once the whole instruction set is complete,
        // this will no longer be needed.
        public void Nios2Dis_Gen()
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x10_0000), new byte[10000]);
            var rnd = new Random(9999);
            rnd.NextBytes(mem.Bytes);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            dasm.ToArray();
        }

        [Test]
        public void Nios2Dasm_andi()
        {
            AssertCode("andi\tr25,r17,0xFF08", "0CC27F8E");
        }

        [Test]
        public void Nios2Dasm_andhi()
        {
            AssertCode("andhi\tr29,r11,0xFC4C", "2C137F5F");
        }

        [Test]
        public void Nios2Dasm_addi()
        {
            AssertCode("addi\tr30,r23,0xFFFFFFFE", "84FF BFBF");
        }

        [Test]
        public void Nios2Dasm_bgeu()
        {
            AssertCode("bgeu\tr30,ra,000FF873", "EE1BFEF7");
        }

        [Test]
        public void Nios2Dasm_bne()
        {
            AssertCode("bne\tr12,r9,00100004", "1E004062");
        }

        [Test]
        public void Nios2Dasm_br()
        {
            AssertCode("br\t0010000C", "06020000");
        }

        [Test]
        public void Nios2Dasm_call()
        {
            AssertCode("call\t000FFCDC", "80CDFFFF");
        }

        [Test]
        public void Nios2Dasm_callr()
        {
            AssertCode("callr\tr18", "3AE8F494");
        }

        [Test]
        public void Nios2Dasm_jmp()
        {
            AssertCode("jmp\tr18", "3A68F494");
        }

        [Test]
        public void Nios2Dasm_cmpeqi()
        {
            AssertCode("cmpeqi\tr16,r11,0x2C4D", "60130B5C");
        }

        [Test]
        public void Nios2Dasm_cmplti()
        {
            AssertCode("cmplti\tr8,sp,0xFFFF9C38", "100E27DA");
        }

        [Test]
        public void Nios2Dasm_cmpltui()
        {
            AssertCode("cmpltui\tr13,r19,0xC3C4", "30F1709B");
        }

        [Test]
        public void Nios2Dasm_cmpnei()
        {
            AssertCode("cmpnei\tr10,r26,0xFFFFBB87", "D8E1AED2");
        }

        [Test]
        public void Nios2Dasm_flushi()
        {
            AssertCode("flushi\t384(r1)", "3A600008");
        }

        [Test]
        public void Nios2Dasm_flushp()
        {
            AssertCode("flushp", "3A200000");
        }

        [Test]
        public void Nios2Dasm_initi()
        {
            AssertCode("initi\tr1", "3A480108");
        }

        [Test]
        public void Nios2Dasm_jmpi()
        {
            AssertCode("jmpi\t00100000", "C1FFFFFF");
        }

        [Test]
        public void Nios2Dasm_ldbio()
        {
            AssertCode("ldbio\tr3,-4955(zero)", "6729FB00");
        }

        [Test]
        public void Nios2Dasm_ldbu()
        {
            AssertCode("ldbu\tr23,1446(r12)", "8369C165");
        }

        [Test]
        public void Nios2Dasm_ldbuio()
        {
            AssertCode("ldbuio\tr21,-11992(r16)", "234A7485");
        }

        [Test]
        public void Nios2Dasm_ldh()
        {
            AssertCode("ldh\tr14,-4065(r18)", "CF07BC93");
        }

        [Test]
        public void Nios2Dasm_ldhio()
        {
            AssertCode("ldhio\tr2,-25717(r17)", "EFE2A688");
        }

        [Test]
        public void Nios2Dasm_ldhu()
        {
            AssertCode("ldhu\tr12,-19412(zero)", "0B0B2D03");
        }

        [Test]
        public void Nios2Dasm_ldhuio()
        {
            AssertCode("ldhuio\tr2,19672(r7)", "2B369338");
        }

        [Test]
        public void Nios2Dasm_ldwio()
        {
            AssertCode("ldwio\tr2,-24736(r22)", "37D8A7B0");
        }

        [Test]
        public void Nios2Dasm_muli()
        {
            AssertCode("muli\tr21,r5,0xFFFFFFF8", "24FE7F2D");
        }

        [Test]
        public void Nios2Dis_ori()
        {
            AssertCode("ori\tr10,zero,0x8850", "14 14 A2 02");
        }

        [Test]
        public void Nios2Dasm_sll()
        {
            AssertCode("sll\tr22,r14,r17", "3A986C74");
        }

        [Test]
        public void Nios2Dasm_slli()
        {
            AssertCode("slli\tr7,r7,0x8", "3A920E38");
        }

        [Test]
        public void Nios2Dasm_sth()
        {
            AssertCode("sth\tr4,-4(r9)", "0DFF3F49");
        }

        [Test]
        public void Nios2Dasm_stw()
        {
            AssertCode("stw\tr3,-25780(ra)", "15D3E6F8");
        }

        [Test]
        public void Nios2Dasm_sync()
        {
            AssertCode("sync", "3AB00100");
        }

        [Test]
        public void Nios2Dasm_xorhi()
        {
            AssertCode("xorhi\tr15,r21,0x11EC", "3C7BC4AB");
        }

        [Test]
        public void Nios2Dasm_xori()
        {
            AssertCode("xori\tr9,r2,0x63E5", "5CF95812");
        }
    }
}
