#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.V850;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.UnitTests.Arch.V850
{
    [TestFixture]
    public class V850DisassemblerTests : DisassemblerTestBase<V850Instruction>
    {
        private readonly V850Architecture arch;
        private readonly Address addr;

        public V850DisassemblerTests()
        {
            this.arch = new V850Architecture(base.CreateServiceContainer(), "v850", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexInstruction)
        {
            var instr = base.DisassembleHexBytes(hexInstruction);
            if (sExpected != instr.ToString())
            {
                Debug.Print("AssertCode(\"{0}\", \"{1}\");", instr, hexInstruction);
                Assert.AreEqual(sExpected, instr.ToString());
            }
        }

        [Test]
        public void V850Dis_Generate()
        {
            var buf = new byte[10000];
            var rnd = new Random(0x4711);
            rnd.NextBytes(buf);
            var mem = new ByteMemoryArea(addr, buf);
            var rdr = mem.CreateLeReader(mem.BaseAddress);
            var dasm = arch.CreateDisassembler(rdr);
            foreach (var instr in dasm)
            {
                dasm.ToString();
            }
        }

  

        [Test]
        public void V850Dis_add_imm5()
        {
            AssertCode("add\tFFFFFFFE,r19", "5E9A FEFF");
        }

        [Test]
        public void V850Dis_add_reg()
        {
            AssertCode("add\tr10,r8", "CA41");
        }


        [Test]
        public void V850Dis_addi_imm16()
        {
            AssertCode("addi\t-00000002,r0,r16", "0086 FEFF");
        }

        [Test]
        public void V850Dis_and()
        {
            AssertCode("and\tr2,r10", "4251");
        }

        [Test]
        public void V850Dis_andi()
        {
            AssertCode("andi\t-00000002,ep,r29", "DEEE FEFF");
        }

        [Test]
        public void V850Dis_bge()
        {
            AssertCode("bge\t001000F8", "CE7D");
        }

        [Test]
        public void V850Dis_bgt()
        {
            AssertCode("bgt\t001000F8", "CF7D");
        }

        [Test]
        public void V850Dis_bl()
        {
            AssertCode("bl\t000FFF8A", "D1C5");
        }


        [Test]
        public void V850Dis_bn()
        {
            AssertCode("bn\t000FFFFE", "F4FD");
        }

        [Test]
        public void V850Dis_bnh()
        {
            AssertCode("bnh\t00100042", "9325");
        }

        [Test]
        public void V850Dis_bnl()
        {
            AssertCode("bnl\t000FFFFC", "E9FD");
        }

        [Test]
        public void V850Dis_bnz()
        {
            AssertCode("bnz\t000FFFBC", "EADD");
        }


        [Test]
        public void V850Dis_bv()
        {
            AssertCode("bv\t001000D0", "806D");
        }

        [Test]
        public void V850Dis_C9B7()
        {
            AssertCode("clr1\t00000006,-2[r9]", "C9B7 FEFF");
        }

        [Test]
        public void V850Dis_cmp_imm5()
        {
            AssertCode("cmp\tFFFFFFFE,r15", "7E7A");
        }

        [Test]
        [Ignore("Nyi")]
        public void V850Dis_dispose()
        {
            AssertCode("@@@", "4D06 FEFF");
        }

        [Test]
        public void V850Dis_jarl_disp22()
        {
            AssertCode("jarl\t000EFFFE,r31", "8667 FEFF");
        }

        [Test]
        public void V850Dis_jmp()
        {
            AssertCode("jmp\t-2[r2]", "E206 FEFF FFFF");
        }

        [Test]
        public void V850Dis_ld_b()
        {
            AssertCode("ld.b\t-2[r0],r4", "0027 FEFF");
        }

        [Test]
        [Ignore("NYI")]
        public void V850Dis_ld_bu_disp16()
        {
            AssertCode("@@@", "B53F 05FF 34F2");
        }

        [Test]
        public void V850Dis_ld_h()
        {
            AssertCode("ld.h\t-2[ep],r31", "2477 FEFF");
        }

        [Test]
        [Ignore("Crazy encoding")]
        public void V850Dis_ld_h_disp23()
        {
            AssertCode("@@@", "B53F 07FF 34F2");
        }


        [Test]
        public void V850Dis_mov()
        {
            AssertCode("mov\tr31,r25", "1FC8");
        }

        [Test]
        public void V850Dis_mov_neg_imm()
        {
            AssertCode("mov\tFFFFFFF0,r17", "108A");
        }

        [Test]
        public void V850Dis_mov_imm32()
        {
            AssertCode("mov\t12345678,r2", "2206 7856 3412");
        }

        [Test]
        public void V850Dis_movea()
        {
            AssertCode("movea\t-00000002,r0,r4", "2026 FEFF");
        }

        [Test]
        public void V850Dis_movhi()
        {
            AssertCode("movhi\t-00000002,r3,r25", "43CE FEFF");
        }

        [Test]
        public void V850Dis_mulh()
        {
            AssertCode("mulh\tr10,r15", "EA78 FEFF 34FB");
        }

        [Test]
        public void V850Dis_mulhi()
        {
            AssertCode("mulhi\t-00000002,r2,r29", "E2EE FEFF");
        }

        [Test]
        public void V850Dis_nop()
        {
            AssertCode("nop", "0000");
        }

        [Test]
        public void V850Dis_not1()
        {
            AssertCode("not1\t00000001,-2[r16]", "D04F FEFF 34FB");
        }

        [Test]
        public void V850Dis_or()
        {
            AssertCode("or\tr2,r24", "02C1");
        }

        [Test]
        public void V850Dis_ori()
        {
            AssertCode("ori\t-00000002,r14,r21", "8EAE FEFF");
        }

        [Test]
        [Ignore("Unclear how the disassembly should look like")]
        public void V850Dis_prepare()
        {
            AssertCode("@@@", "B53F 01FF");
        }

        [Test]
        [Ignore("Unclear how the disassembly should look like")]
        public void V850Dis_prepare_3()
        {
            AssertCode("@@@", "B53F 01FF");
        }

        [Test]
        public void V850Dis_sar()
        {
            AssertCode("sar\t0000000C,r5", "AC2A");
        }

        [Test]
        public void V850Dis_satadd_reg()
        {
            AssertCode("satadd\tr21,r3", "D518 FEFF");
        }

        [Test]
        public void V850Dis_satadd_simm5()
        {
            AssertCode("satadd\tFFFFFFFE,r5", "3E2A");
        }

        [Test]
        public void V850Dis_satsub_reg()
        {
            AssertCode("satsub\tr14,r9", "AE48");
        }

        [Test]
        public void V850Dis_satsubi()
        {
            AssertCode("satsubi\t-00000002,r0,r5", "602E FEFF");
        }

        [Test]
        public void V850Dis_set1()
        {
            AssertCode("set1\t00000007,-2[r16]", "D03F FEFF");
        }

        [Test]
        public void V850Dis_shl()
        {
            AssertCode("shl\t0000000F,ep", "CFF2");
        }

        [Test]
        public void V850Dis_shr()
        {
            AssertCode("shr\t00000003,r8", "8342");
        }

        [Test]
        public void V850Dis_sld_b()
        {
            AssertCode("sld.b\t3[ep],r14", "0373");
        }

        [Test]
        public void V850Dis_sld_h()
        {
            AssertCode("sld.h\t6[ep],r24", "06C4");
        }

        [Test]
        public void V850Dis_sld_hu()
        {
            AssertCode("sld.hu\t12[ep],r10", "7750");
        }

        [Test]
        public void V850Dis_sld_w()
        {
            AssertCode("sld.w\t0[ep],r8", "0045");
        }

        [Test]
        public void V850Dis_sst_b()
        {
            AssertCode("sst.b\tr15,108[ep]", "EC7B");
        }

        [Test]
        public void V850Dis_sst_h()
        {
            AssertCode("sst.h\tr24,90[ep]", "DAC4");
        }

        [Test]
        public void V850Dis_sst_w()
        {
            AssertCode("sst.w\tr25,44[ep]", "5BCD");
        }

        [Test]
        public void V850Dis_st_b()
        {
            AssertCode("st.b\tr12,-2[r13]", "4D67 FEFF");
        }

        [Test]
        public void V850Dis_st_h()
        {
            AssertCode("st.h\tr31,-2[ep]", "7E17 FEFF 34FB");
        }

        [Test]
        public void V850Dis_sub_reg()
        {
            AssertCode("sub\tr29,r10", "BD51 FEFF 34FB");
        }

        [Test]
        public void V850Dis_sxh()
        {
            AssertCode("sxh\tr20", "D400");
        }

        [Test]
        public void V850Dis_tst()
        {
            AssertCode("tst\tr23,r19", "7799");
        }

        [Test]
        public void V850Dis_tst1()
        {
            AssertCode("tst1\t00000006,-2[r7]", "C7F7 FEFF");
        }

        [Test]
        public void V850Dis_xor()
        {
            AssertCode("xor\tr3,r5", "2329");
        }

        [Test]
        public void V850Dis_xori()
        {
            AssertCode("xori\t-00000002,r22,r31", "B6FE FEFF");
        }

        [Test]
        public void V850Dis_zxb()
        {
            AssertCode("zxb\tr14", "AE00");
        }
    }
}
