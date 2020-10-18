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
        public void MS1750Dis_aisp()
        {
            AssertCode("aisp\tgp1,#&1", "A210");
        }

        [Test]
        public void MS1750Dis_bez()
        {
            AssertCode("bez\t0104", "7505");
        }

        [Test]
        public void MS1750Dis_bge()
        {
            AssertCode("bge\t0170", "7B71");
        }

        [Test]
        public void MS1750Dis_blt()
        {
            AssertCode("blt\t011D", "761E");
        }

        [Test]
        public void MS1750Dis_br()
        {
            AssertCode("br\t00F9", "74FA");
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
        public void MS1750Dis_fa()
        {
            AssertCode("fa\tgp5,&AAAA,gp5", "A855AAAA");
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
        public void MS1750Dis_fm()
        {
            AssertCode("fm\tgp7,&1234,gp6", "C8761234");
        }

        [Test]
        public void MS1750Dis_fneg()
        {
            AssertCode("fneg\tgp0,gp8", "BC08");
        }

        [Test]
        public void MS1750Dis_l()
        {
            AssertCode("l\tgp0,&1234,gp1", "80011234");
        }

        [Test]
        public void MS1750Dis_lim()
        {
            AssertCode("lim\tgp0,#&FFF0", "8500FFF0");
        }

        [Test]
        public void MS1750Dis_llb()
        {
            AssertCode("llb\tgp8,&89AB", "8C8089AB");
        }

        [Test]
        public void MS1750Dis_lr()
        {
            AssertCode("lr\tgp1,gp0", "8110");
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
        public void MS1750Dis_po()
        {
            AssertCode("po\tgp6,#&321", "48640321");
        }

        [Test]
        public void MS1750Dis_sb()
        {
            AssertCode("sb\t#&3,&1234,gp9", "50391234");
        }

        [Test]
        public void MS1750Dis_sbi()
        {
            AssertCode("sbi\t#&D,&1234", "52D01234");
        }

        [Test]
        public void MS1750Dis_sjs()
        {
            AssertCode("sjs\tgp15,1234", "7EF01234");
        }

        [Test]
        public void MS1750Dis_stc()
        {
            AssertCode("stc\t#&4,&1234,gp4", "91441234");
        }

        [Test]
        public void MS1750Dis_stub()
        {
            AssertCode("stub\tgp14,&1234,gp7", "9BE71234");
        }

        [Test]
        public void MS1750Dis_urs()
        {
            AssertCode("urs\tgp15", "7FF0");
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
    }
}
