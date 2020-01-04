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

using Reko.Arch.M68k;
using Reko.Assemblers.M68k;
using Reko.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Assemblers.M68k
{
    [TestFixture]
    public class M68kAssemblerTests
    {
        private M68kArchitecture arch;
        private M68kAssembler asm;
        private M68kDisassembler dasm;

        [SetUp]
        public void Setup()
        {
            arch = new M68kArchitecture("m68k");
            asm = new M68kAssembler(arch, Address.Ptr32(0x00010000), new List<ImageSymbol>());
        }

        private void BuildTest(Action<M68kAssembler> builder)
        {
            builder(asm);
            dasm = M68kDisassembler.Create68020(
                asm.GetImage().SegmentMap.Segments.Values.First().MemoryArea.CreateBeReader(asm.BaseAddress));
        }

        private void Expect(string expectedInstr)
        {
            var instr = dasm.First();
            Assert.AreEqual(expectedInstr, instr.ToString());
        }

        [Test]
        public void M68kasm_nop()
        {
            BuildTest(m =>
            {
                m.Nop();
            });
            Expect("nop");
        }

        [Test]
        public void M68kasm_addq_d0()
        {
            BuildTest(m =>
            {
                m.Addq_l(4, m.d0);
            });
            Expect("addq.l\t#$04,d0");
        }

        [Test]
        public void M68kasm_adda_l_d0_a0()
        {
            BuildTest(m =>
            {
                m.Adda_l(m.d0, m.a0);
            });
            Expect("adda.l\td0,a0");
        }

        [Test]
        public void M68kasm_predec()
        {
            BuildTest(m =>
            {
                m.Move_b(m.Pre(m.a2), m.d0);
            });
            Expect("move.b\t-(a2),d0");
        }

        [Test]
        public void M68kasm_addi_w_mem()
        {
            BuildTest(m =>
            {
                m.Addi_w(0x300, m.Mem(m.a6));
            });
            Expect("addi.w\t#$0300,(a6)");
        }

        [Test]
        public void M68kasm_lsl_l_mem_off()
        {
            BuildTest(m =>
            {
                m.Lsl_l(0x04, m.d1);
            });
            Expect("lsl.l\t#$04,d1");
        }

        [Test]
        public void M68kasm_symbolic_labels()
        {
            BuildTest(m =>
            {
                m.Clr_l(m.d0);
                m.Label("lupe");
                m.Cmp_l(m.Post(m.a3), m.d0);
                m.Bne("lupe");
            });
            Expect("clr.l\td0");
            Expect("cmp.l\t(a3)+,d0");
            Expect("bne\t$00010002");
        }

        [Test]
        public void M68kasm_lea()
        {
            BuildTest(m =>
            {
                m.Lea(m.Mem(-8,  m.a2), m.a4);
            });
            Expect("lea\t-$0008(a2),a4");
        }
    }
}
