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

using NUnit.Framework;
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class Arm64CallingConventionTests
    {
        private static readonly PrimitiveType i32 = PrimitiveType.Int32;

        private Arm64Architecture arch;
        private AArch64CallingConvention cc;
        private CallingConventionEmitter ccr;

        public Arm64CallingConventionTests()
        {
            this.arch = new Arm64Architecture(new ServiceContainer(), "aarch64", new Dictionary<string, object>());
        }

        [SetUp]
        public void Setup()
        {
            this.cc = new AArch64CallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void SysVArm64cc_int_arg()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (x0)", ccr.ToString());
        }

        [Test]
        public void SysVArm64cc_float_arg()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (x0)", ccr.ToString());
        }
    }
}
