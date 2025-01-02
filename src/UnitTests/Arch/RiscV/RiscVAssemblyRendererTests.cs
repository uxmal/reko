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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Machine;


namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVAssemblyRendererTests
    {
        private readonly RegisterStorage zero = RegisterStorage.Reg32("zero", 0);
        private readonly RegisterStorage x1 = RegisterStorage.Reg32("x1", 1);
        private MachineInstructionRendererFlags flags;

        [SetUp]
        public void Setup()
        {
            flags = MachineInstructionRendererFlags.None;
        }

        private void AssertCode(string sExpected, RiscVInstruction instr)
        {
            var sr = new StringRenderer();
            var options = new MachineInstructionRendererOptions(
                flags: flags
            );
            instr.Render(sr, options);
            var sActual = sr.ToString();
            Assert.AreEqual(sExpected, sActual);
        }

        [Test]
        public void RiscVAr_addi_aliased_to_li()
        {
            var instr = new RiscVInstruction
            {
                Mnemonic = Mnemonic.addi,
                Operands = new MachineOperand[] { x1, zero, ImmediateOperand.Int32(-1)},
            };
            AssertCode("li\tx1,-0x1", instr);
        }

        [Test]
        public void RiscVAr_addi_not_aliased_to_li()
        {
            var instr = new RiscVInstruction
            {
                Mnemonic = Mnemonic.addi,
                Operands = new MachineOperand[] { x1, zero, ImmediateOperand.Int32(-1)},
            };
            flags |= MachineInstructionRendererFlags.RenderInstructionsCanonically;
            AssertCode("addi\tx1,zero,-0x1", instr);
        }
    }
}
