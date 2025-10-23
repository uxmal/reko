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
using Reko.Arch.Motorola;
using Reko.Arch.Motorola.M68k;
using Reko.Arch.Motorola.M68k.Machine;
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Motorola.M68k
{
    [TestFixture]
    public class M68kStateTests
    {
        private M68kArchitecture arch;
        private M68kState state;

        [SetUp]
        public void Setup()
        {
            arch = new M68kArchitecture(new ServiceContainer(), "m68k", new Dictionary<string, object>());
            state = new M68kState(arch);
        }

        [Test]
        public void M68ks_SetIllegalValue()
        {
            var invalid = InvalidConstant.Create(Registers.a7.DataType);
            state.SetValue(Registers.a7, invalid);
            Assert.AreSame(invalid, state.GetValue(Registers.a7));
        }
    }
}
