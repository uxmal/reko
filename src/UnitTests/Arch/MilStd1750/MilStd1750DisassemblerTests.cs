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
using Reko.Arch.MilStd1750;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.MilStd1750
{
    [TestFixture]
    public class MilStd1750DisassemblerTests : DisassemblerTestBase<Instruction>
    {
        private readonly MilStd1750Architecture arch;
        private readonly Address addrLoad;

        public MilStd1750DisassemblerTests()
        {
            this.arch = new MilStd1750Architecture(CreateServiceContainer(), "mil-std-1750a", new Dictionary<string, object>());
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

        // This spams the CI builds [Test]
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
        public void MS1750Dis_aim()
        {
            AssertCode("aim\tgp0,#0x30", "4A010030");
        }

        [Test]
        public void MS1750Dis_aisp()
        {
            AssertCode("aisp\tgp1,#1", "A210");
        }

        [Test]
        public void MS1750Dis_andm()
        {
            AssertCode("andm\tgp0,#0xF", "4A07000F");
        }

        [Test]
        public void MS1750Dis_andx()
        {
            AssertCode("andx\tgp12,gp11", "40EB");
        }

        [Test]
        public void MS1750Dis_bez()
        {
            AssertCode("bez\t0105", "7505");
        }

        [Test]
        public void MS1750Dis_bge()
        {
            AssertCode("bge\t0171", "7B71");
        }

        [Test]
        public void MS1750Dis_blt()
        {
            AssertCode("blt\t011E", "761E");
        }

        [Test]
        public void MS1750Dis_bpt()
        {
            AssertCode("bpt", "FFFF");
        }

        [Test]
        public void MS1750Dis_br()
        {
            AssertCode("br\t00FA", "74FA");
        }

        [Test]
        public void MS1750Dis_c()
        {
            AssertCode("c\tgp7,0x1234,gp14", "F07E1234");
        }

        [Test]
        public void MS1750Dis_cbl()
        {
            AssertCode("cbl\tgp15,0x1234,gp5", "F4F51234");
        }

        [Test]
        public void MS1750Dis_cim()
        {
            AssertCode("cim\tgp1,#0x8000", "4A1A8000");
        }

        [Test]
        public void MS1750Dis_cisp()
        {
            AssertCode("cisp\tgp1,#3", "F212");
        }

        [Test]
        public void MS1750Dis_co()
        {
            AssertCode("co\tgp0", "48004000");
        }

        [Test]
        public void MS1750Dis_da()
        {
            AssertCode("da\tgp0,0x1234", "A6001234");
        }

        [Test]
        public void MS1750Dis_dc()
        {
            AssertCode("dc\tgp0,0x1234,gp2", "F6021234");
        }

        [Test]
        public void MS1750Dis_ddr()
        {
            AssertCode("ddr\tgp12,gp7", "D7C7");
        }

        [Test]
        public void MS1750Dis_disp()
        {
            AssertCode("disp\tgp1,#0xA", "D219");
        }

        [Test]
        public void MS1750Dis_dlr()
        {
            AssertCode("dlr\tgp0,gp3", "8703");
        }

        [Test]
        public void MS1750Dis_dsra()
        {
            AssertCode("dsra\tgp2,#0x10", "67F2");
        }

        [Test]
        public void MS1750Dis_dst()
        {
            AssertCode("dst\tgp8,0x1234,gp14", "968E1234");
        }

        [Test]
        public void MS1750Dis_dsti()
        {
            AssertCode("dsti\tgp1,0x1234,gp11", "981B1234");
        }

        [Test]
        public void MS1750Dis_efa()
        {
            AssertCode("efa\tgp8,0x1234,gp12", "AA8C1234");
        }

        [Test]
        public void MS1750Dis_efc()
        {
            AssertCode("efc\tgp2,0x1234", "FA201234");
        }

        [Test]
        public void MS1750Dis_efd()
        {
            AssertCode("efd\tgp3,0x1234,gp14", "DA3E1234");
        }


        [Test]
        public void MS1750Dis_efdr()
        {
            AssertCode("efdr\tgp14,gp1", "DBE1");
        }

        [Test]
        public void MS1750Dis_efl()
        {
            AssertCode("efl\tgp5,0x1234,gp8", "8A581234");
        }

        [Test]
        public void MS1750Dis_eflt()
        {
            AssertCode("eflt\tgp6,gp2", "EB62");
        }

        [Test]
        public void MS1750Dis_efm()
        {
            AssertCode("efm\tgp3,0x1234", "CA301234");
        }

        [Test]
        public void MS1750Dis_efmr()
        {
            AssertCode("efmr\tgp5,gp5", "CB55");
        }

        [Test]
        public void MS1750Dis_efsr()
        {
            AssertCode("efsr\tgp2,gp8", "BB28");
        }

        [Test]
        public void MS1750Dis_efst()
        {
            AssertCode("efst\tgp0,0x1234,gp8", "9A081234");
        }

        [Test]
        public void MS1750Dis_efix()
        {
            AssertCode("efix\tgp0,gp5", "EA05");
        }

        [Test]
        public void MS1750Dis_fa()
        {
            AssertCode("fa\tgp5,0xAAAA,gp5", "A855AAAA");
        }

        [Test]
        public void MS1750Dis_fabs()
        {
            AssertCode("fabs\tgp6,gp0", "AC60");
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
            AssertCode("fm\tgp7,0x1234,gp6", "C8761234");
        }

        [Test]
        public void MS1750Dis_fneg()
        {
            AssertCode("fneg\tgp0,gp8", "BC08");
        }

        [Test]
        public void MS1750Dis_incm()
        {
            AssertCode("incm\t#1,0x1234,gp14", "A30E1234");
        }

        [Test]
        public void MS1750Dis_jc()
        {
            AssertCode("jc\t#2,0120", "70200120");
        }

        [Test]
        public void MS1750Dis_l()
        {
            AssertCode("l\tgp0,0x1234,gp1", "80011234");
        }

        [Test]
        public void MS1750Dis_lbx()
        {
            AssertCode("lbx\tgp12,gp0", "4000");
        }

        [Test]
        public void MS1750Dis_lim()
        {
            AssertCode("lim\tgp0,#0xFFF0", "8500FFF0");
        }

        [Test]
        public void MS1750Dis_llb()
        {
            AssertCode("llb\tgp8,0x89AB", "8C8089AB");
        }

        [Test]
        public void MS1750Dis_lr()
        {
            AssertCode("lr\tgp1,gp0", "8110");
        }

        [Test]
        public void MS1750Dis_lub()
        {
            AssertCode("lub\tgp0,0x1234,gp11", "8B0B1234");
        }

        [Test]
        public void MS1750Dis_mov()
        {
            AssertCode("mov\tgp0,gp2", "9302");
        }

        [Test]
        public void MS1750Dis_msr()
        {
            AssertCode("msr\tgp15,gp8", "C1F8");
        }

        [Test]
        public void MS1750Dis_neg()
        {
            AssertCode("neg\tgp1,gp1", "B411");
        }

        [Test]
        public void MS1750Dis_nop()
        {
            AssertCode("nop", "FF00");
        }

        [Test]
        public void MS1750Dis_or()
        {
            AssertCode("or\tgp8,0x1234,gp3", "E0831234");
        }

        [Test]
        public void MS1750Dis_orim()
        {
            AssertCode("orim\tgp1,#0xFF", "4A1800FF");
        }

        [Test]
        public void MS1750Dis_po()
        {
            AssertCode("po\tgp6,#0x321", "48640321");
        }

        [Test]
        public void MS1750Dis_popm()
        {
            AssertCode("popm\tgp14,gp14", "8FEE");
        }
        
        [Test]
        public void MS1750Dis_pshm()
        {
            AssertCode("pshm\tgp14,gp14", "9FEE");
        }

        [Test]
        public void MS1750Dis_rvbr()
        {
            AssertCode("rvbr\tgp0,gp4", "5C04");
        }

        [Test]
        public void MS1750Dis_sar()
        {
            AssertCode("sar\tgp2,gp1", "6B21");
        }

        [Test]
        public void MS1750Dis_sb()
        {
            AssertCode("sb\t#3,0x1234,gp9", "50391234");
        }

        [Test]
        public void MS1750Dis_sbi()
        {
            AssertCode("sbi\t#0xD,0x1234", "52D01234");
        }

        [Test]
        public void MS1750Dis_sisp()
        {
            AssertCode("sisp\tgp15,#1", "B2F0");
        }

        [Test]
        public void MS1750Dis_sjs()
        {
            AssertCode("sjs\tgp15,1234", "7EF01234");
        }

        [Test]
        public void MS1750Dis_sll()
        {
            AssertCode("sll\tgp8,#1", "6008");
        }

        [Test]
        public void MS1750Dis_slr()
        {
            AssertCode("slr\tgp5,gp2", "6A52");
        }

        [Test]
        public void MS1750Dis_soj()
        {
            AssertCode("soj\tgp9,1234", "73901234");
        }

        [Test]
        public void MS1750Dis_sr()
        {
            AssertCode("sr\tgp2,gp1", "B121");
        }

        [Test]
        public void MS1750Dis_srl()
        {
            AssertCode("srl\tgp4,#1", "6104");
        }

        [Test]
        public void MS1750Dis_stc()
        {
            AssertCode("stc\t#4,0x1234,gp4", "91441234");
        }

        [Test]
        public void MS1750Dis_stlb()
        {
            AssertCode("stlb\tgp10,0x1234,gp11", "9CAB1234");
        }

        [Test]
        public void MS1750Dis_stm()
        {
            AssertCode("stm\t#0xF,0x1234,gp14", "99FE 1234");
        }

        [Test]
        public void MS1750Dis_stub()
        {
            AssertCode("stub\tgp14,0x1234,gp7", "9BE71234");
        }

        [Test]
        public void MS1750Dis_svbr()
        {
            AssertCode("svbr\tgp8,gp2", "5A82");
        }

        [Test]
        public void MS1750Dis_tbr()
        {
            AssertCode("tbr\t#8,gp4", "5784");
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
            AssertCode("xor\tgp2,0xFEDC,gp9", "E429FEDC");
        }

        [Test]
        public void MS1750Dis_xwr()
        {
            AssertCode("xwr\tgp15,gp11", "EDFB");
        }

        [Test]
        public void MS1750Dis_4F76()
        {
            AssertCode("bif\t#0x76", "4F76");
        }
        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        // Reko: a decoder for the instruction 5A82 at address 000005B4 has not been implemented. (svbr)
  
        // Reko: a decoder for the instruction 99FE at address 000005B6 has not been implemented. (stm )



    }
}
