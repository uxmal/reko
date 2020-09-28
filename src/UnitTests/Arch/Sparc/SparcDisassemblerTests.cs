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

using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class SparcDisassemblerTests
    {
        private static SparcInstruction DisassembleWord(byte[] a)
        {
            MemoryArea img = new MemoryArea(Address.Ptr32(0x00100000), a);
            return Disassemble(img);
        }

        private static SparcInstruction DisassembleWord(uint instr)
        {
            var bytes = new byte[4];
            new BeImageWriter(bytes).WriteBeUInt32(0, instr);
            var img = new MemoryArea(Address.Ptr32(0x00100000), bytes);
            return Disassemble(img);
        }

        private static SparcInstruction Disassemble(MemoryArea img)
        {
            var arch = new SparcArchitecture("sparc", PrimitiveType.Word32);
            var dasm = new SparcDisassembler(arch, img.CreateBeReader(0U));
            return dasm.First();
        }

        private void AssertInstruction(uint word, string expected)
        {
            var instr = DisassembleWord(word);
            Assert.AreEqual(expected, instr.ToString());
        }

        [Test]
        public void SparcDis_call()
        {
            AssertInstruction(0x7FFFFFFF, "call\t000FFFFC");
        }

        [Test]
        public void SparcDis_addcc()
        {
            AssertInstruction(0x8A800004, "addcc\t%g0,%g4,%g5");
        }

        [Test]
        public void SparcDis_subcc()
        {
            AssertInstruction(0x986060FF, "subx\t%g1,0x000000FF,%o4");
        }

        [Test]
        public void SparcDis_or_imm()
        {
            AssertInstruction(0xBE10E004, "or\t%g3,0x00000004,%i7");
        }

        [Test]
        public void SparcDis_and_neg()
        {
            AssertInstruction(0x86087FFE, "and\t%g1,0xFFFFFFFE,%g3");
        }

        [Test]
        public void SparcDis_sll_imm()
        {
            AssertInstruction(0xAB2EA01F, "sll\t%i2,0x0000001F,%l5");
        }

        [Test]
        public void SparcDis_sethi()
        {
            AssertInstruction(0x0B00AAAA, "sethi\t0x0000AAAA,%g5");
        }

        [Test]
        public void SparcDis_taddcc()
        {
            AssertInstruction(0x8B006001, "taddcc\t%g1,0x00000001,%g5");
        }

        [Test]
        public void SparcDis_mulscc()
        {
            AssertInstruction(0x8B204009, "mulscc\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_umul()
        {
            AssertInstruction(0x8A504009, "umul\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_smul()
        {
            AssertInstruction(0x8A584009, "smul\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_udivcc()
        {
            AssertInstruction(0x8AF04009, "udivcc\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_sdiv()
        {
            AssertInstruction(0x8A784009, "sdiv\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_save()
        {
            AssertInstruction(0x8BE04009, "save\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcDis_be()
        {
            AssertInstruction(0x02800001, "be\t00100004");
        }

        [Test]
        public void SparcDis_fbne()
        {
            AssertInstruction(0x03800001, "fbne\t00100004");
        }

        [Test]
        public void SparcDis_jmpl()
        {
            AssertInstruction(0x8FC07FFF, "jmpl\t%g1,-1,%g7");
        }

        [Test]
        public void SparcDis_rett()
        {
            AssertInstruction(0x81C86009, "rett\t%g1,9");
        }

        [Test]
        public void SparcDis_ta()
        {
            AssertInstruction(0x91D06999, "ta\t%g1,0x00000019");
        }

        [Test]
        public void SparcDis_fitos()
        {
            AssertInstruction(0x8BA0188A, "fitos\t%f10,%f5");
        }

        [Test]
        public void SparcDis_ldsb()
        {
            AssertInstruction(0xC248A044, "ldsb\t[%g2+68],%g1"); 
        }

        [Test]
        public void SparcDis_sth()
        {
            AssertInstruction(0xC230BFF0, "sth\t%g1,[%g2-16]"); 
        }

        [Test]
        public void SparcDis_bg_a()
        {
            AssertInstruction(0x34800024, "bg,a\t00100090");
        }

        [Test]
        public void SparcDis_rdy()
        {
            AssertInstruction(0xA3400000, "rd\t%y,%l1");
        }

        [Test]
        public void SparcDis_fcmpes()
        {
            AssertInstruction(0x81a80aa2, "fcmpes\t%f0,%f2");
        }

        [Test]
        public void SparcDis_ldd()
        {
            AssertInstruction(0xd01be000, "ldd\t[%o7+0],%o0");
        }

        [Test]
        public void SparcDis_sss()
        {
            AssertInstruction(0xC12D0000, "stfsr\t%fsr,[%l4+%g0]");
        }

        [Test]
        public void SparcDis_fcmpd()
        {
            AssertInstruction(0x81A90A47, "fcmpd\t%f4,%f38");
        }
    }
}
