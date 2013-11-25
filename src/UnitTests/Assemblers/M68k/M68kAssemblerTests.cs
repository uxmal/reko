#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Assemblers.M68k;
using Decompiler.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Assemblers.M68k
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
            arch = new M68kArchitecture();
            asm = new M68kAssembler(arch, new Address(0x00010000), new List<EntryPoint>());
        }

        private void BuildTest(Action<M68kAssembler> builder)
        {
            builder(asm);
            dasm = new M68kDisassembler(new ImageReader(asm.GetImage(), asm.BaseAddress));
        }

        private void Expect(string expectedInstr)
        {
            var instr = dasm.DisassembleInstruction();
            Assert.AreEqual(expectedInstr, instr.ToString());
        }

        [Test]
        public void M68kasm_nop()
        {
            BuildTest(m =>
            {
                m.Nop();
            });
            Expect("nop\t");
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
    }
}
