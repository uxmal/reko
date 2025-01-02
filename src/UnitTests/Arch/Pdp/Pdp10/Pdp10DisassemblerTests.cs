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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp10;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp10
{
    [TestFixture]
    public class Pdp10DisassemblerTests : DisassemblerTestBase<Pdp10Instruction>
    {
        public Pdp10DisassemblerTests()
        {
            var options = new Dictionary<string, object>();
            Architecture = new Pdp10Architecture(CreateServiceContainer(), "pdp10", options);
            LoadAddress = new Address18(0x001000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string octalWord)
        {
            ulong word = Pdp10Architecture.OctalStringToWord(octalWord);
            var mem = new Word36MemoryArea(LoadAddress, new ulong[] { word });
            var i = Disassemble(mem);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Pdp10Dis_addi()
        {
            AssertCode("addi\t12,60", "271500000060");
        }

        [Test]
        public void Pdp10Dis_andi()
        {
            AssertCode("andi\t1,37777", "405040037777");
        }

        [Test]
        public void Pdp10Dis_aos()
        {
            AssertCode("aos\t43477", "350000043477");
        }

        [Test]
        public void Pdp10Dis_aos_with_ac()
        {
            AssertCode("aos\t16,11(1)", "350701000011");
        }

        [Test]
        public void Pdp10Dis_caige()
        {
            AssertCode("caige\t4,@100000(1)", "305221100000");
        }

        [Test]
        public void Pdp10Dis_dmove()
        {
            AssertCode("dmove\t1,170506(17)", "120057170506");
        }

        [Test]
        public void Pdp10Dis_exch()
        {
            AssertCode("exch\t6,@225124(1)", "250321225124");
        }

        [Test]
        public void Pdp10Dis_fad()
        {
            AssertCode("fad\t10,3", "140400000003");
        }

        [Test]
        public void Pdp10Dis_fadrm()
        {
            AssertCode("fadrm\t12,@461671(7)", "146527461671");
        }

        [Test]
        public void Pdp10Dis_fmpr()
        {
            AssertCode("fmpr\t2,@46046(4)", "164124046046");
        }

        [Test]
        public void Pdp10Dis_fsb()
        {
            AssertCode("fsb\t12,6", "150500000006");
        }

        [Test]
        public void Pdp10Dis_fsbr()
        {
            AssertCode("fsbr\t6,@200000(1)", "154321200000");
        }

        [Test]
        public void Pdp10Dis_idivi()
        {
            AssertCode("idivi\t12,5", "231500000005");
        }

        [Test]
        public void Pdp10Dis_idpb()
        {
            AssertCode("idpb\t1,5(2)", "136042000005");
        }

        [Test]
        public void Pdp10Dis_imul()
        {
            AssertCode("imul\t14,3473(3)", "220603003473");
        }

        [Test]
        public void Pdp10Dis_jfcl()
        {
            AssertCode("jfcl", "255000000000");
        }

        [Test]
        public void Pdp10Dis_jra()
        {
            AssertCode("jra\t4,1(4)", "267204000001");
        }

        [Test]
        public void Pdp10Dis_jrst()
        {
            AssertCode("jrst\t007423", "254000007423");
        }

        [Test]
        public void Pdp10Dis_jrst_indirect()
        {
            AssertCode("jrst\t@176(3)", "254023000176");
        }

        [Test]
        public void Pdp10Dis_jsp()
        {
            AssertCode("jsp\t12,004673", "265500004673");
        }

        [Test]
        public void Pdp10Dis_jsr()
        {
            AssertCode("jsr\t017527", "264040017527");
        }

        [Test]
        public void Pdp10Dis_lsh()
        {
            AssertCode("lsh\t5,777775", "242240777775");
        }

        [Test]
        public void Pdp10Dis_lshc()
        {
            AssertCode("lshc\t17,@0", "246760000000");
        }

        [Test]
        public void Pdp10Dis_moves()
        {
            AssertCode("moves\t1,@167116(14)", "203074167116");
        }
      
        [Test]
        public void Pdp10Dis_movei()
        {
            AssertCode("movei\t7,1", "201340000001");
        }

        [Test]
        public void Pdp10Dis_or()
        {
            AssertCode("or\t7,3143(7)", "434347003143");
        }

        [Test]
        public void Pdp10Dis_push()
        {
            AssertCode("push\t15,116347(6)", "261646116347");
        }

        [Test]
        public void Pdp10Dis_pushj()
        {
            AssertCode("pushj\t17,004736", "260740004736");
        }

        [Test]
        public void Pdp10Dis_pushj_indirect()
        {
            AssertCode("pushj\t17,@777775(17)", "260777777775");
        }

        [Test]
        public void Pdp10Dis_setcmm()
        {
            AssertCode("setcmm\t7,@351602(12)", "462372351602");
        }

        [Test]
        public void Pdp10Dis_setzb_two_operands()
        {
            AssertCode("setzb\t16,4375", "403700004375");
        }

        [Test]
        public void Pdp10Dis_setzm()
        {
            AssertCode("setzm\t3051(7)", "402007003051");
        }

        [Test]
        public void Pdp10Dis_skiple()
        {
            AssertCode("skiple\t5,67671", "333240067671");
        }

        [Test]
        public void Pdp10Dis_sojge()
        {
            AssertCode("sojge\t4,000147", "365200000147");
        }

        [Test]
        public void Pdp10Dis_trnn()
        {
            AssertCode("trnn\t5,566400(11)", "606251566400");
        }
        
        [Test]
        public void Pdp10Dis_tro()
        {
            AssertCode("tro\t6,40", "660300000040");
        }

        [Test]
        public void Pdp10Dis_ujen()
        {
            AssertCode("ujen\t5,777776(17)", "100257777776");
        }
    }
}
