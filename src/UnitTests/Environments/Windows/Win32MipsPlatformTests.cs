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

 using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class Win32MipsPlatformTests
    {
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [Test]
        public void W32Mips_Trampoline()
        {
            var instrs = new List<RtlInstructionCluster>();
            var frame = new Frame(PrimitiveType.Pointer32);
            var r9 = frame.EnsureRegister(new RegisterStorage("r9", 9, 0, PrimitiveType.Word32));
            var rtl = new RtlTrace(0x123460)
            {
                m => m.Assign(r9, 0x00030000),
                m => m.Assign(r9, m.LoadDw(m.IAdd(r9, 0x1234))),
                m => m.Goto(r9)
            };

            var host = mr.Stub<IRewriterHost>();
            var services = mr.Stub<IServiceProvider>();
            var arch = mr.Stub<IProcessorArchitecture>();
            var addr = Address.Ptr32(0x00031234);
            arch.Stub(a => a.MakeAddressFromConstant(Arg<Constant>.Is.NotNull)).Return(addr);
            host.Stub(h => h.GetImportedProcedure(
                Arg<Address>.Is.Equal(addr),
                Arg<Address>.Is.NotNull)).Return(new ExternalProcedure("foo", new FunctionType()));
            mr.ReplayAll();

            var platform = new Win32MipsPlatform(services, arch);
            var result = platform.GetTrampolineDestination(rtl, host);
            Assert.IsNotNull(result);
        }
    }
}
