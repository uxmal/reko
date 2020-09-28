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

using Moq;
using NUnit.Framework;
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]  
    public class PowerPcArchitectureTests
    {
        [Test]
        public void PPCArch_GetTrampoline()
        {
            var arch = new PowerPcBe32Architecture("ppc-be-32");
            var m = new InstructionBuilder(arch, Address.Ptr32(0x10030000));
            m.Lis(m.r11, 0x1006);
            m.Lwz(m.r11, 0x1234, m.r11);
            m.Mtctr(m.r11);
            m.Bctr();
            var host = new Mock<IRewriterHost>();
            host.Setup(h => h.GetImportedProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                It.Is<Address>(a => a.ToLinear() == 0x10061234),
                It.IsAny<Address>()))
                .Returns(new ExternalProcedure("foo", new FunctionType()));

            ProcedureBase proc = arch.GetTrampolineDestination(m.Instructions, host.Object);

            Assert.IsNotNull(proc);
            Assert.AreEqual("foo", proc.Name);
        }
    }
}
