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

using NUnit.Framework;
using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System.Collections.Generic;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class SparcCallingConventionTests
    {
        private PrimitiveType i8 = PrimitiveType.SByte;
        private PrimitiveType i16 = PrimitiveType.Int16;
        private PrimitiveType i32 = PrimitiveType.Int32;

        private SparcArchitecture32 arch;
        private SparcCallingConvention cc;
        private CallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            arch = new SparcArchitecture32("sparc");
        }

        private void Given_CallingConvention()
        {
            this.cc = new SparcCallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void SvSparcPs_DeserializeFpuStackReturnValue()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.Real64, null, new List<DataType>());
            Assert.AreEqual("Sequence f1:f0", ccr.Return.ToString());
        }

        [Test]
        public void SvSparcPs_Load_cdecl()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null, new List<DataType> { PrimitiveType.Int32 });
            Assert.AreEqual("Stk: 0 void (o0)", ccr.ToString());
        }

        [Test]
        public void SvSparcPs_Load_IntArgs()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null, new List<DataType> { i16, i8, i32, i16, i8, i32, i8 });
            Assert.AreEqual("Stk: 0 void (o0, o1, o2, o3, o4, o5, Stack +0018)", ccr.ToString());
        }
    }
}
