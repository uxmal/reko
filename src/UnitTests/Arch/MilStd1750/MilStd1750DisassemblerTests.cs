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
using Reko.Arch.MilStd1750;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MilStd1750
{
    [TestFixture]
    public class MilStd1750DisassemblerTests : DisassemblerTestBase<Instruction>
    {
        private readonly MilStd1750Architecture arch;
        private readonly Address addrLoad;

        public MilStd1750DisassemblerTests()
        {
            this.arch = new MilStd1750Architecture(CreateServiceContainer(), "mil-std-1750a");
            this.addrLoad = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override Instruction DisassembleHexBytes(string hexBytes)
        {
            var mem = HexStringToMemoryArea(hexBytes);
            return Disassemble(mem);
        }

        private MemoryArea HexStringToMemoryArea(string sBytes)
        {
            int shift = 12;
            int bb = 0;
            var words = new List<ushort>();
            for (int i = 0; i < sBytes.Length; ++i)
            {
                char c = sBytes[i];
                if (BytePattern.TryParseHexDigit(c, out byte b))
                {
                    bb |= (b << shift);
                    shift -= 4;
                    if (shift < 0)
                    {
                        words.Add((ushort) bb);
                        shift = 12;
                        bb = 0;
                    }
                }
            }
            return new Word16MemoryArea(this.LoadAddress, words.ToArray());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void MS1750Dis_Generate()
        {
            var rnd = new Random(0x3421);
            var words = Enumerable.Range(0, 200)
                .Select(_ => (ushort) rnd.Next())
                .ToArray();
            var mem = new Word16MemoryArea(LoadAddress, words);
            var dasm = new MilStd1750Disassembler(arch, mem.CreateBeReader(0));
            foreach (var instr in dasm)
            {

            }
        }

        [Test]
        public void MS1750Dis_blt()
        {
            AssertCode("@@@", "761E");
        }

        [Test]
        public void MS1750Dis_dc()
        {
            AssertCode("dc\tgp0,&1234,gp2", "F6021234");
        }

        [Test]
        public void MS1750Dis_dsti()
        {
            AssertCode("dsti\tgp1,&1234,gp11", "981B1234");
        }

        [Test]
        public void MS1750Dis_efdr()
        {
            AssertCode("efdr\tgp14,gp1", "DBE1");
        }

        [Test]
        public void MS1750Dis_fcr()
        {
            AssertCode("fcr\tgp13,gp5", "F9D5");
        }

        [Test]
        public void MS1750Dis_flt()
        {
            AssertCode("flt\tgp8,gp0", "E980");
        }

        [Test]
        public void MS1750Dis_fneg()
        {
            AssertCode("fneg\tgp0,gp8", "BC08");
        }

        [Test]
        public void MS1750Dis_llb()
        {
            AssertCode("llb\tgp8,&89AB", "8C8089AB");
        }

        [Test]
        public void MS1750Dis_msr()
        {
            AssertCode("msr\tgp15,gp8", "C1F8");
        }

        [Test]
        public void MS1750Dis_nop()
        {
            AssertCode("nop", "FF00");
        }

        [Test]
        public void MS1750Dis_or()
        {
            AssertCode("or\tgp8,&1234,gp3", "E0831234");
        }

        [Test]
        public void MS1750Dis_sbi()
        {
            AssertCode("sbi\t#&D,&1234", "52D01234");
        }

        [Test]
        public void MS1750Dis_stc()
        {
            AssertCode("stc\t#&4,&1234,gp4", "91441234");
        }

        [Test]
        public void MS1750Dis_stub()
        {
            AssertCode("stubt\tgp14,&1234,gp7", "9BE71234");
        }

        [Test]
        public void MS1750Dis_xbr()
        {
            AssertCode("xbr\tgp11", "ECB0");
        }

        [Test]
        public void MS1750Dis_xor()
        {
            AssertCode("xor\tgp2,&FEDC,gp9", "E429FEDC");
        }

        // Reko: a decoder for the instruction DBE1 at address 0100 has not been implemented. (efdr)


        // Reko: a decoder for the instruction F9D5 at address 0102 has not been implemented. (fcr)


        // Reko: a decoder for the instruction F602 at address 0103 has not been implemented. (dc)


        // Reko: a decoder for the instruction E429 at address 0104 has not been implemented. (xor)


        // Reko: a decoder for the instruction 9144 at address 0105 has not been implemented. (stc )


        // Reko: a decoder for the instruction 9BE7 at address 0108 has not been implemented. (stub)


        // Reko: a decoder for the instruction BC08 at address 0109 has not been implemented. (fneg)


        // Reko: a decoder for the instruction 8C85 at address 010A has not been implemented. (llb)
  

        // Reko: a decoder for the instruction C1F8 at address 010B has not been implemented. (msr)


        // Reko: a decoder for the instruction 8C9E at address 010C has not been implemented. (llb)


        // Reko: a decoder for the instruction 981B at address 010D has not been implemented. (dsti )


        // Reko: a decoder for the instruction 52D0 at address 010E has not been implemented. (sbi)


        // Reko: a decoder for the instruction C12A at address 0110 has not been implemented. (msr)


        // Reko: a decoder for the instruction 761E at address 0111 has not been implemented. (dc)


        // Reko: a decoder for the instruction F6F9 at address 0112 has not been implemented. (dc)

        // Reko: a decoder for the instruction E083 at address 0113 has not been implemented. (or)


        // Reko: a decoder for the instruction E980 at address 0114 has not been implemented. (flt)


        // Reko: a decoder for the instruction 4864 at address 0115 has not been implemented. (xio)
        [Test]
        public void MS1750Dis_4864()
        {
            AssertCode("@@@", "4864");
        }

        // Reko: a decoder for the instruction 5039 at address 0117 has not been implemented. (sb)
        [Test]
        public void MS1750Dis_5039()
        {
            AssertCode("@@@", "5039");
        }

        // Reko: a decoder for the instruction C876 at address 011A has not been implemented. (fm)
        [Test]
        public void MS1750Dis_C876()
        {
            AssertCode("@@@", "C876");
        }

        // Reko: a decoder for the instruction A855 at address 011D has not been implemented. (fa)
        [Test]
        public void MS1750Dis_A855()
        {
            AssertCode("@@@", "A855");
        }

        // Reko: a decoder for the instruction ECBB at address 011F has not been implemented. (xbr)

        // Reko: a decoder for the instruction 7B71 at address 0121 has not been implemented. (efcr)
        [Test]
        public void MS1750Dis_7B71()
        {
            AssertCode("@@@", "7B71");
        }
    }
}
