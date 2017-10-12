#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]  
    public class PowerPcArchitectureTests
    {
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [Test]
        public void PPCArch_GetTrampoline()
        {
            var arch = new PowerPcBe32Architecture();
            var m = new InstructionBuilder(arch, Address.Ptr32(0x10030000));
            m.Lis(m.r11, 0x1006);
            m.Lwz(m.r11, 0x1234, m.r11);
            m.Mtctr(m.r11);
            m.Bctr();
            var host = mr.Stub<IRewriterHost>();
            host.Stub(h => h.GetImportedProcedure(
                Arg<Address>.Matches(a => a.ToLinear() == 0x10061234),
                Arg<Address>.Is.Anything)).Return(new ExternalProcedure("foo", null));
            mr.ReplayAll();

            ProcedureBase proc = arch.GetTrampolineDestination(m.Instructions, host);

            Assert.IsNotNull(proc);
            Assert.AreEqual("foo", proc.Name);
        }
    }
}
