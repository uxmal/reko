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
using Reko.Arch.Zilog;
using Reko.Arch.Zilog.Z80;
using Reko.Core.Lib;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Zilog.Z80
{
    [TestFixture]
    public class Z80ArchitectureTests
    {
        private Z80Architecture arch;

        [SetUp]
        public void Setup()
        {
            arch = new Z80Architecture(new ServiceContainer(), "z80", []);
        }

        [Test]
        public void Z80Arch_GetRegister_Low()
        {
            Assert.AreEqual(Registers.l, arch.GetRegister(Registers.hl.Domain, new BitRange(0, 8)));
        }

        [Test]
        public void Z80Arch_GetRegister_High()
        {
            Assert.AreEqual(Registers.d, arch.GetRegister(Registers.de.Domain, new BitRange(8, 16)));
        }

        [Test]
        public void Z80Arch_GetRegister_word()
        {
            Assert.AreEqual(Registers.bc, arch.GetRegister(Registers.bc.Domain, new BitRange(0, 16)));
        }

        [Test]
        public void Z80Arch_GetSubRegister_hl()
        {
            Assert.AreSame(Registers.h, arch.GetRegister(Registers.hl.Domain, new BitRange(8, 16)));
        }
    }
}
