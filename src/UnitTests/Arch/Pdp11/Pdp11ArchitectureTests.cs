#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Pdp11
{
    [TestFixture]
    public class Pdp11ArchitectureTests
    {
        private Pdp11Architecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new Pdp11Architecture("pdp11");
        }

        [Test]
        public void Pdp11Arch_CreateStackAccess()
        {
            var binder = MockRepository.GenerateStub<IStorageBinder>();
            var sp = Registers.sp;
            binder.Stub(b => b.EnsureRegister(sp)).Return(new Identifier(sp.Name, sp.DataType, sp));
            binder.Replay();
            
            var access = arch.CreateStackAccess(binder, -12, PrimitiveType.Word16);

            Assert.AreEqual("Mem0[sp + -12:word16]", access.ToString());
        }
    }
}
