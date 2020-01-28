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
using Reko.Arch.Xtensa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaDisassemblerTests : DisassemblerTestBase<XtensaInstruction>
    {
        private XtensaArchitecture arch;

        public XtensaDisassemblerTests()
        {
            this.arch = new XtensaArchitecture("xtensa");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Xtdasm_l32r()
        {
            AssertCode("l32r\ta7,000FFFFC", 0xFFFF71);
            AssertCode("l32r\ta2,000FFFF8", 0xFFFE21);
        }

        [Test]
        public void Xtdasm_ret()
        {
            AssertCode("ret", 0x000080);
        }

        [Test]
        public void Xtdasm_ill()
        {
            AssertCode("ill", 0x000000);
        }

        [Test]
        public void Xtdasm_wsr()
        {
            AssertCode("wsr\ta2,VECBASE", 0x13E720);
        }

        [Test]
        public void Xtdasm_or()
        {
            AssertCode("or\ta1,a1,a1", 0x201110);
        }

        [Test]
        public void Xtdasm_call0()
        {
            AssertCode("call0\t00100B24", 0x00B205);
        }

        [Test]
        public void Xtdasm_reserved()
        {
            AssertCode("reserved", 0xFE9200);
            // 00 92 fe
        }

        [Test]
        public void Xtdasm_movi()
        {
            AssertCode("movi\ta9,000003A0", 0xA0A392);
        }

        [Test]
        public void Xtdasm_sub()
        {
            AssertCode("sub\ta1,a1,a9", 0xC01190);
        }

        [Test]
        public void Xtdasm_s32i()
        {
            AssertCode("s32i\ta13,a1,0394", 0xE561D2);
            AssertCode("s32i\ta0,a1,039C", 0xE76102);
        }

        [Test]
        public void Xtdasm_memw()
        {
            AssertCode("memw", 0x0020C0);
        }

        [Test]
        public void Xtdasm_l32i_n()
        {
            AssertCode("l32i.n\ta4,a13,1C", 0x7D48);
        }

        [Test]
        public void Xtdasm_movi_n()
        {
            AssertCode("movi.n\ta3,-20", 0x036C);
            AssertCode("movi.n\ta3,-01", 0xF37C);
            AssertCode("movi.n\ta3,+20", 0x032C);
            AssertCode("movi.n\ta3,+5F", 0xF35C);
        }

        [Test]
        public void Xtdasm_s32i_n()
        {
            AssertCode("s32i.n\ta3,a13,1C", 0x7d39);
        }

        [Test]
        public void Xtdasm_mov_n()
        {
            AssertCode("mov.n\ta3,a1", 0x013D);
        }

        [Test]
        public void Xtdasm_l8ui()
        {
            AssertCode("l8ui\ta2,a1,0003", 0x030122);
        }

        [Test]
        public void Xtdasm_srli()
        {
            AssertCode("srli\ta4,a2,04", 0x414420);
        }

        [Test]
        public void Xtdasm_addi_n()
        {
            AssertCode("addi.n\ta4,a4,-01", 0x440B);
        }

        [Test]
        public void Xtdasm_extui()
        {
            AssertCode("extui\ta2,a2,00,04", 0x342020);
        }

        [Test]
        public void Xtdasm_addx4()
        {
            AssertCode("addx4\ta2,a2,a3", 0xA02230);
        }

        [Test]
        public void Xtdasm_bgeui()
        {
            AssertCode("bgeui\ta4,00000004,00100016", 0x1244f6);
        }

        [Test]
        public void Xtdasm_slli()
        {
            AssertCode("slli\ta4,a12,14", 0x114c40);
        }

        [Test]
        public void Xtdasm_addmi()
        {
            AssertCode("addmi\ta2,a4,FFFFF000", 0xF0D422);
        }

        [Test]
        public void Xtdasm_j()
        {
            AssertCode("j\t0010000E", 0x000286);
        }

        [Test]
        public void Xtdasm_and()
        {
            AssertCode("and\ta4,a6,a4", 0x104640);
        }

        [Test]
        public void Xtdasm_movnez()
        {
            AssertCode("movnez\ta2,a5,a3", 0x932530);
        }

        [Test]
        public void Xtdasm_addi()
        {
            AssertCode("addi\ta4,a3,FFFFFFFD", 0xFDC342);
        }

        [Test]
        public void Xtdasm_add_n()
        {
            AssertCode("add.n\ta4,a4,a12", 0x44CA);
        }

        [Test]
        public void Xtdasm_l32i()
        {
            AssertCode("l32i\ta5,a1,0374", 0xDD2152);
        }

        [Test]
        public void Xtdasm_bltu()
        {
            AssertCode("bltu\ta4,a2,00100012", 0x0E3427);
        }

        [Test]
        public void Xtdasm_callx0()
        {
            AssertCode("callx0\ta0", 0x0000C0);
        }

        [Test]
        public void Xtdasm_bnei()
        {
            AssertCode("bnei\ta13,-00000001,00100023", 0x1f0d66);
        }

        [Test]
        public void Xtdasm_bne()
        {
            AssertCode("bne\ta4,a5,000FFFFC", 0xF89457);
        }

        [Test]
        public void Xtdasm_orbc()
        {
            AssertCode("orbc\tb0,b0,b0", 0x320000);
        }

        [Test]
        public void Xtdasm_ret_n()
        {
            AssertCode("ret.n", 0xF00D);
        }

        [Test]
        public void Xtdasm_mull()
        {
            AssertCode("mull\ta2,a4,a2", 0x822420);
        }

        [Test]
        public void Xtdasm_wsr_excsave2()
        {
            AssertCode("wsr\ta0,EXCSAVE2", 0x13D200);
        }

        [Test]
        public void Xtdasm_break()
        {
            AssertCode("break\t01,00", 0x004100);
        }

        [Test]
        public void Xtdasm_beqz_n()
        {
            AssertCode("beqz.n\ta14,00100013", 0xFE8C);
        }

        [Test]
        public void Xtdasm_andbc()
        {
            AssertCode("andbc\tb4,b0,b2", 0x124020);
        }

        [Test]
        public void Xtdasm_rfi()
        {
            AssertCode("rfi\t02", 0x003210);
        }

        [Test]
        public void Xtdasm_neg()
        {
            AssertCode("neg\ta2,a2", 0x602020);
        }

        [Test]
        public void Xtdasm_bbsi()
        {
            AssertCode("bbsi\ta4,00,000FFFEB", 0xE7E407);
        }

        [Test]
        public void Xtdasm_rsr()
        {
            AssertCode("rsr\ta7,EXCCAUSE", 0x03e870);
        }

        [Test]
        public void Xtdasm_ball()
        {
            AssertCode("ball\ta3,a6,00100029", 0x254367);
        }

        [Test]
        public void Xtdasm_s8i()
        {
            AssertCode("s8i\ta4,a2,0027", 0x274242);
            AssertCode("s8i\ta10,a0,0000", 0x0040a2);
        }

        [Test]
        public void Xtdasm_xor()
        {
            AssertCode("xor\ta5,a5,a4", 0x305540);
        }

        [Test]
        public void Xtdasm_bnez_n()
        {
            AssertCode("bnez.n\ta2,00100007", 0x32CC);
        }

        [Test]
        public void Xtdasm_add()
        {
            AssertCode("add\ta4,a2,a3", 0x804230);
        }

        [Test]
        public void Xtdasm_rfe()
        {
            AssertCode("rfe", 0x003000);
        }

        [Test]
        public void Xtdasm_srai()
        {
            AssertCode("srai\ta3,a10,10", 0x3130a0);
        }

        [Test]
        public void Xtdasm_bnez()
        {
            AssertCode("bnez\ta6,000FFFFA", 0xff6656);
        }

        [Test]
        public void Xtdasm_bany()
        {
            AssertCode("bany\ta12,a3,00100012", 0x0e8c37);
        }

        [Test]
        public void Xtdasm_beqz()
        {
            AssertCode("beqz\ta0,0010000C", 0x008016);
        }

        [Test]
        public void Xtdasm_rsr_epc1()
        {
            AssertCode("rsr\ta6,EPC1", 0x03b160);
        }

        [Test]
        public void Xtdasm_addx2()
        {
            AssertCode("addx2\ta6,a0,a0", 0x906000);
        }

        [Test]
        public void Xtdasm_bltui()
        {
            AssertCode("bltui\ta4,00000007,000FFFED", 0xe974b6);
        }

        [Test]
        public void Xtdasm_ssa8l()
        {
            AssertCode("ssa8l\ta3", 0x402300);
        }

        [Test]
        public void Xtdasm_ssl()
        {
            AssertCode("ssl\ta2", 0x401200);
        }

        [Test]
        public void Xtdasm_bnone()
        {
            AssertCode("bnone\ta11,a2,000FFFFA", 0xf60b27);
        }

        [Test]
        public void Xtdasm_movgez()
        {
            AssertCode("movgez\ta4,a5,a3", 0xb34530);
        }

        [Test]
        public void Xtdasm_ssai()
        {
            AssertCode("ssai\t08", 0x404800);
        }

        [Test]
        public void Xtdasm_ssr()
        {
            AssertCode("ssr\ta5", 0x400500);
        }

        [Test]
        public void Xtdasm_src()
        {
            AssertCode("src\ta6,a7,a6", 0x816760);
        }

        [Test]
        public void Xtdasm_sll()
        {
            AssertCode("sll\ta7,a6", 0xa17600);
        }

        [Test]
        public void Xtdasm_srl()
        {
            AssertCode("srl\ta2,a8", 0x912080);
        }

        [Test]
        public void Xtdasm_bnall()
        {
            AssertCode("bnall\ta5,a6,0010001E", 0x1ac567);
        }

        [Test]
        public void Xtdasm_ssi()
        {
            AssertCode("ssi\tf7,a6,0000", 0x004673);
        }

        [Test]
        public void Xtdasm_add_s()
        {
            AssertCode("add.s\tf4,f6,f0", 0x0a4600);
        }

        [Test]
        public void Xtdasm_lsiu()
        {
            AssertCode("lsiu\tf3,a1,0000", 0x008133);
        }

        [Test]
        public void Xtdasm_s32ri()
        {
            AssertCode("s32ri\ta0,a15,0300", 0xc0ff02);
        }

        [Test]
        public void Xtdasm_nsau()
        {
            AssertCode("nsau\ta3,a3", 0x40f330);
        }

        [Test]
        public void Xtdasm_mul16u()
        {
            AssertCode("mul16u\ta5,a6,a5", 0xc15650);
        }

        [Test]
        public void Xdasm_floor_s()
        {
            AssertCode("floor.s\ta11,f12,02", 0xBABC20);
        }

        [Test]
        public void Xtdasm_rems()
        {
            AssertCode("rems\ta3,a1,a0", 0xF23100);
        }

        [Test]
        public void Xtdasm_jx()
        {
            AssertCode("jx\ta9", 0x0009a0);
        }

        [Test]
        public void Xtdasm_subx2()
        {
            AssertCode("subx2\ta11,a11,a9", 0xD0BB90);
        }

        [Test]
        public void Xtdasm_subx4()
        {
            AssertCode("subx4\ta5,a5,a3", 0xE05530);
        }

        [Test]
        public void Xtdasm_subx8()
        {
            AssertCode("subx8\ta3,a13,a13", 0xF03DD0);
        }

        [Test]
        public void Xtdasm_isync()
        {
            AssertCode("isync", 0x002000);
        }

        [Test]
        public void Xtdasm_bbs()
        {
            AssertCode("bbs\ta3,a2,00100059", 0x55d327);
        }

        [Test]
        public void Xtdasm_bbc()
        {
            AssertCode("bbc\ta2,a4,00100008", 0x045247);
        }

        [Test]
        public void Xtdasm_moveqz_s()
        {
            AssertCode("moveqz.s\tf15,f12,a0", 0x8bfc00);
        }

        [Test]
        public void Xtdasm_rsil()
        {
            AssertCode("rsil\ta4,01", 0x006140);
        }

        [Test]
        public void Xtdasm_ldpte()
        {
            AssertCode("ldpte", 0xf1f810);
        }

        [Test]
        public void Xtdasm_s32e()
        {
            AssertCode("s32e\ta2,a0,-00000040", 0x490020);
        }

        [Test]
        public void Xtdasm_ueq_s()
        {
            AssertCode("ueq.s\tb11,f0,f2", 0x3bb020);
        }

        [Test]
        public void Xtdasm_l32e()
        {
            AssertCode("l32e\ta0,a4,-00000030", 0x094400);
        }

        [Test]
        public void Xtdasm_quou()
        {
            AssertCode("quou\ta1,a6,a0", 0xc21600);
        }

        [Test]
        public void Xtdasm_min()
        {
            AssertCode("min\ta2,a0,a1", 0x432010);
        }

        private uint ToHex(string value)
        {
            uint n = 0;
            uint f = 1u;
            for (int i = 0; i < value.Length; i += 2)
            {
                n = Convert.ToUInt32(value.Substring(i, 2), 16) * f + n;
                f = f << 8;
            }
            return n;
        }

        public void Xtdasm_swallow()
        {
            var rdr = new StreamReader("boot.txt");
            var re = new Regex("^[^:]+:\t([0-9a-f]+)[ \t]+([^;]+)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
            int c = 0;

            var ignore = new HashSet<uint>
            {
                0x00FF00, 0x3FFFC208, 0x4000DC3C, 0x3FFFD650,
                0x000024, 0x000020, 0x000018, 0x000014,
                0xF43347, 0x002E05, 0x00334B,
                0x000369, 0x00554B, 0xFFD411, 0x00000C, 0x38000000, 0x1FFFFF,
                0x080000, 0x7FF00000, 0xFFFFFF, 0x400000, 0x7f800000, 0xfefefeff,
                0x0041F0, 0x00BA05, 0xF83677, 0x00664B, 0x000609, 0x00090C, 0x2ED2F6,
            };

            for (;;)
            {
                string line = rdr.ReadLine();
                if (line == null || c >= 100)
                    break;

                // Chop the line into hex bytes and instruction.
                var m = re.Match(line);
                if (m.Groups.Count != 3)
                    continue;

                // Convert hex to word.

                uint uInstr = ToHex(m.Groups[1].Value);
                if (ignore.Contains(uInstr))
                    continue;
                var i = DisassembleWord(uInstr);
                if (!(m.Groups[2].Value.StartsWith(".long") ||
                      m.Groups[2].Value.StartsWith(".byte"))
                      &&
                    (i.ToString().StartsWith("inval") || 
                    i.ToString().StartsWith("rese")))
                /*
                if (i.ToString()
                    .Replace(" ", "") 
                    .Replace("\t", "") != m.Groups[2].Value
                    .Replace(" ", "")
                    .Replace("\t", ""))*/
                {
                    Debug.Print("        [Test]");
                    Debug.Print("        public void Xtdasm_{0}()", m.Groups[2].Value.Split(' ')[0]);
                    Debug.Print("        {");
                    Debug.Print("            AssertCode(\"{0}\", 0x{1:X6}); // {2}", m.Groups[2].Value, uInstr, i);
                    Debug.Print("        }");
                    Debug.Print("");
                    ++c;
                }
            }
            Assert.AreEqual(0, c, "C tests failed");
        }
    }
}