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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Expressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kStateTests
    {
        private M68kArchitecture arch;
        private M68kState state;

        [SetUp]
        public void Setup()
        {
            arch = new M68kArchitecture("m68k");
            state = new M68kState(arch);
        }

        [Test]
        public void M68ks_SetIllegalValue()
        {
            state.SetValue(Registers.a7, Constant.Invalid);
            Assert.AreSame(Constant.Invalid, state.GetValue(Registers.a7));
        }
    }
}
