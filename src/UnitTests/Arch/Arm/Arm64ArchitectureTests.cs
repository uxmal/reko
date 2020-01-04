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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class Arm64ArchitectureTests
    {
        [Test]
        public void Arm64Arch_Registers()
        {
            for (int i = 0; i < Registers.SubRegisters.Length; ++i)
            {
                Assert.AreEqual(i, Registers.SubRegisters[i][0].Number);
                Assert.AreEqual(i, (int) Registers.SubRegisters[i][0].Domain);
            }
        }

        [Test]
        public void Arm64Arch_GetRegister()
        {
            var arch = new Arm64Architecture("aarch64");
            Assert.AreSame(Registers.GpRegs32[3], arch.GetRegister((StorageDomain) 3, new BitRange(16, 32)));
        }
    }
}
