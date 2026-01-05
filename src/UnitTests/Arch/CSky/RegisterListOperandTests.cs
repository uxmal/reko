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
using Reko.Arch.CSky;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.CSky
{
    [TestFixture]
    public class RegisterListOperandTests
    {
        private void RunTest(string sExpected, RegisterListOperand rl)
        {
            var s = new StringRenderer();
            rl.Render(s, new());

            Assert.AreEqual(sExpected, s.ToString());
        }

        [Test]
        public void CSkyRl_Single()
        {
            RunTest("r2", new RegisterListOperand(4, Registers.GpRegs));
        }

        [Test]
        public void CSkyRl_Pair()
        {
            RunTest("r2-r3", new RegisterListOperand(0xC, Registers.GpRegs));
        }


        [Test]
        public void CSkyRl_r15()
        {
            RunTest("r15", new RegisterListOperand(0x8000, Registers.GpRegs));
        }

        [Test]
        public void CSkyRl_r14_r15()
        {
            RunTest("r14-r15", new RegisterListOperand(0xC000, Registers.GpRegs));
        }

        [Test]
        public void CSkyRl_r31()
        {
            RunTest("r31", new RegisterListOperand(0x8000_0000, Registers.GpRegs));
        }
    }
}
