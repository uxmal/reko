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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.X86.Disassembler
{
    [TestFixture]
    public class X86Disassembler_8086_Tests : DisassemblerTestBase<X86Instruction>
    {
        public X86Disassembler_8086_Tests()
        {
            Architecture = new X86ArchitectureReal(
                CreateServiceContainer(),
                "x86-real-16",
                new Dictionary<string, object>
                {
                    { ProcessorOption.InstructionSet, "8086" }
                });
            LoadAddress = Address.SegPtr(0x800, 0);
        }
        public override IProcessorArchitecture Architecture { get; }
        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void X86_8086_Arpl_invalid()
        {
            AssertCode("illegal", "63 C3");
        }

        [Test]
        public void X86_8086_fs_prefix_invalid()
        {
            AssertCode("illegal", "64 01 00");
        }

        [Test]
        public void X86_8086_fadd_DC_C3()
        {
            AssertCode("fadd\tst(3),st(0)", "DC C3");
        }
    }
}
