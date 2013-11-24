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

using Decompiler.Core;
using Decompiler.Arch.Arm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Arm
{
    [TestFixture]
    public class AArch64DisassemblerTests : ArmTestBase
    {
        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new AArch64();
        }

        [Test]
        public void b_label()
        {
            var instr = DisassembleBits("00010111 11111111 11111111 00000000"); 
            Assert.AreEqual("b\t000FFC04",instr.ToString());
        }

        [Test]
        public void bl_label()
        {
            var instr = DisassembleBits("10010111 11111111 11111111 00000000");
            Assert.AreEqual("bl\t000FFC04", instr.ToString());
        }

        [Test]
        public void br_Xn()
        {
            var instr = DisassembleBits("11010110 00011111 00000011 11000000");
            Assert.AreEqual("br\tx30", instr.ToString());
        }
    }
}
