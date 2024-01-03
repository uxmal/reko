#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core;
using Reko.Arch.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Reko.Core.Services;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.Cil
{
    [TestFixture]
    public class CilDisassemblerTests
    {
        private void RunTest(string sExp, params byte[] bytes)
        {
            var image = new ByteMemoryArea(Address.Ptr32(0x0100000), bytes);
            var sc = new ServiceContainer();
            sc.AddService<ITestGenerationService>(new UnitTestGenerationService(sc));
            var arch = new CilArchitecture {
                Services = sc,
            };
            var dasm = new CilDisassembler(arch, image.CreateLeReader(0)).GetEnumerator();
            Assert.IsTrue(dasm.MoveNext());
            var instr = dasm.Current;
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void CilDasm_Ldc0()
        {
            RunTest("ldc.i4.0", 0x16);
        }
    }
}
