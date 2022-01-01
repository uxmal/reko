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
using Reko.Arch.Cray;
using Reko.Arch.Cray.Cray1;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Cray.Ymp
{
    [TestFixture]
    public class YmpDisassemblerTests : DisassemblerTestBase<CrayInstruction>
    {
        private CrayYmpArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new CrayYmpArchitecture(new ServiceContainer(), "crayYmp", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string octBytes)
        {
            var instr = DisassembleOctBytes(octBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        protected CrayInstruction DisassembleOctBytes(string octalBytes)
        {
            ushort[] words = ToUInt16s(OctalStringToBytes(octalBytes)).ToArray();
            var mem = new Word16MemoryArea(LoadAddress, words);
            return Disassemble(mem);
        }

        public static IEnumerable<ushort> ToUInt16s(IEnumerable<byte> bytes)
        {
            var e = bytes.GetEnumerator();
            while (e.MoveNext())
            {
                var hi = e.Current;
                if (!e.MoveNext())
                    break;
                var lo = e.Current;
                yield return (ushort) ((hi << 8) | lo);
            }
        }

        [Test]
        public void YmpDis_Ai_imm()
        {
            AssertCode("A7 200123456", "020700 123456 001000");
        }

        [Test]
        public void YmpDis_clz()
        {
            AssertCode("A2 ZS0", "027200");
        }
            [Test]
        public void YmpDis_err()
        {
            AssertCode("err", "000000");
        }

        [Test]
        public void YmpDis_ex()
        {
            AssertCode("ex", "004000");
        }

        [Test]
        public void YmpDis_A2_imul()
        {
            AssertCode("A2 A2*A6", "032226");
        }

        [Test]
        public void YmpDis_A_isub()
        {
            AssertCode("A7 A4-A6", "031746");
        }

        [Test]
        public void YmpDis_A_mv_neg1()
        {
            AssertCode("A7 37777777777", "031700");
        }

        [Test]
        public void YmpDis_A_isub1()
        {
            AssertCode("A7 A4-000001", "031740");
        }

        [Test]
        public void YmpDis_A_iadd()
        {
            AssertCode("A4 A3+A4", "030434");
        }

        [Test]
        public void YmpDis_A_iadd_1()
        {
            AssertCode("A4 A3+000001", "030430");
        }

        [Test]
        public void YmpDis_A_mov()
        {
            AssertCode("A4 A3", "030403");
        }

        [Test]
        public void YmpDis_Ai_nm()
        {
            AssertCode("A2 24713523456", "020200 123456 123456");
        }

        [Test]
        public void YmpDis_Bjk_Ai()
        {
            AssertCode("B57 A1", "025171");
        }


        [Test]
        public void YmpDis_Si_and()
        {
            AssertCode("S1 S2&S3", "044123");
        }

        [Test]
        public void YmpDis_Si_and_sb()
        {
            AssertCode("S1 S2&SB", "044120");
        }

        [Test]
        public void YmpDis_Si_andnot()
        {
            AssertCode("S2 #S2&S7", "045272");
        }

        [Test]
        public void YmpDis_Si_dshl()
        {
            AssertCode("S5 S5,S6<A1", "056561");
        }

        [Test]
        public void YmpDis_Si_load()
        {
            AssertCode("S7 24713523456,A1", "121700 123456 123456");
        }

        [Test]
        public void YmpDis_Si_load_direct()
        {
            AssertCode("S7 24713523456,", "120700 123456 123456");
        }

        [Test]
        public void YmpDis_Si_mov_1()
        {
            AssertCode("S7 000001", "042777");
        }

        [Test]
        public void YmpDis_Si_mov_imm32()
        {
            AssertCode("S0 24713523456", "040000123456123456");
        }

        [Test]
        public void YmpDis_Si_isub()
        {
            AssertCode("S3 S7-S3", "061373");
        }

        [Test]
        public void YmpDis_Si_movlo()
        {
            AssertCode("S0 S0:24713523456", "040020 123456123456");
        }

        [Test]
        public void YmpDis_Si_movhi()
        {
            AssertCode("S0 24713523456:S0", "040040 123456123456");
        }

        [Test]
        public void YmpDis_mov_Ai_Sj()
        {
            AssertCode("A7 S1", "023710");
        }

        [Test]
        public void YmpDis_Si_Ak()
        {
            AssertCode("S3 A6", "071306");
        }

        [Test]
        public void YmpDis_mov_Si_Vj_Ak()
        {
            AssertCode("S1 V2,A3", "076123");
        }

        [Test]
        public void YmpDis_fmul_Sj_Sk()
        {
            AssertCode("S1 S2*FS3", "064123");
        }

        [Test]
        public void YmpDis_j_Bjk()
        {
            AssertCode("j B63", "005077");
        }

        [Test]
        public void YmpDis_j()
        {
            AssertCode("j 000000123456", "006000 123456 123456");
        }

        [Test]
        public void YmpDis_jaz()
        {
            AssertCode("jaz 000000133445", "010000 133445");
        }

        [Test]
        public void YmpDis_jsn()
        {
            AssertCode("jsn 000000134455", "015000 134455");
        }

        [Test]
        public void YmpDis_pass()
        {
            AssertCode("pass", "001000");
        }

        [Test]
        public void YmpDis_R_001()
        {
            AssertCode("r 000000234567", "007001 234567");
        }

        [Test]
        public void YmpDis_R_000()
        {
            AssertCode("r 000000123456", "007000 123456");
        }

        [Test]
        public void YmpDis_load_inc()
        {
            AssertCode("V0 ,A0,000001", "176000");
        }

        [Test]
        public void YmpDis_store_inc()
        {
            AssertCode(",A0,000001 V2", "177020");
        }

        [Test]
        public void YmpDis_store_inc_Ai()
        {
            AssertCode(",A0,A6 V6", "177066");
        }

        [Test]
        public void YmpDis_store_Si()
        {
            AssertCode("24713523456,A6 S4", "136400 123456 123456");
        }

        [Test]
        public void YmpDis_Si_RT()
        {
            AssertCode("S1 RT", "072100");
        }

        [Test]
        public void YmpDis_SM_Si()
        {
            AssertCode("SM S2", "073202");
        }

        [Test]
        public void YmpDis_Tjk_Si()
        {
            AssertCode("T56 S1", "075170");
        }

        [Test]
        public void YmpDis_Vi_iadd()
        {
            AssertCode("V3 S1+V1", "154311");
        }

        [Test]
        public void YmpDis_Vi_vmov()
        {
            AssertCode("V1 V3", "142103");
        }

        [Test]
        public void YmpDis_Vi_or()
        {
            AssertCode("V2 S3!V1", "142231");
        }


        [Test]
        public void YmpDis_Vi_or_Sj()
        {
            AssertCode("V2 S2!V1", "142221");
        }

        [Test]
        public void YmpDis_VL()
        {
            AssertCode("VL A2", "002002");
        }

    }
}
