#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.Avr;
using Reko.Arch.Avr.Avr8;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr8ArchitectureTests
    {
        [Test]
        public void Avr8_arch_AliasZRegister()
        {
            var arch = new Avr8Architecture(new ServiceContainer(), "avr8", new Dictionary<string, object>());
            var r30 = arch.GetRegister(30);
            var r31 = arch.GetRegister(31);
            var z = Registers.z;

            Assert.AreSame(r30, arch.GetRegister(z.Domain, new BitRange(0, 8)));
            Assert.AreSame(r31, arch.GetRegister(z.Domain, new BitRange(8, 16)));
        }
    }
}
