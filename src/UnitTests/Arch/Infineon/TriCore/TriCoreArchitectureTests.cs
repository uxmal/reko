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
using Reko.Arch.Infineon;
using Reko.Arch.Infineon.TriCore;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Infineon.TriCore;

[TestFixture]
public class TriCoreArchitectureTests
{
    private TriCoreArchitecture arch;

    [SetUp]
    public void Setup()
    {
        arch = new TriCoreArchitecture(new ServiceContainer(), "tricore", []);
    }

    [Test]
    public void TriCoreArch_GetRegister_Low()
    {
        Assert.AreEqual(Registers.DataRegisters[2], arch.GetRegister(Registers.ExtendedDRegisters[1].Domain, new(0, 32)));
        Assert.AreEqual(Registers.DataRegisters[3], arch.GetRegister(Registers.ExtendedDRegisters[1].Domain, new(32, 64)));
    }

}
