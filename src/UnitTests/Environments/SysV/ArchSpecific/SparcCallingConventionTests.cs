#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class SparcCallingConventionTests
    {
        private readonly PrimitiveType i8 = PrimitiveType.SByte;
        private readonly PrimitiveType i16 = PrimitiveType.Int16;
        private readonly PrimitiveType i32 = PrimitiveType.Int32;
        private readonly PrimitiveType i64 = PrimitiveType.Int64;
        private readonly TypeReference off_t = new TypeReference("off_t", PrimitiveType.Int64);

        private SparcArchitecture32 arch;
        private SparcCallingConvention cc;
        private CallingConventionBuilder ccr;

        [SetUp]
        public void Setup()
        {
            arch = new SparcArchitecture32(new ServiceContainer(), "sparc", new Dictionary<string, object>());
        }

        private void Given_CallingConvention()
        {
            this.cc = new SparcCallingConvention(arch);
            this.ccr = new CallingConventionBuilder();
        }

        [Test]
        public void SvSparcPs_DeserializeFpuStackReturnValue()
        {
            Given_CallingConvention();
            cc.Generate(ccr, 0, PrimitiveType.Real64, null, new List<DataType>());
            Assert.AreEqual("Sequence f1:f0", ccr.Return.ToString());
        }

        [Test]
        public void SvSparcPs_Load_cdecl()
        {
            Given_CallingConvention();
            cc.Generate(ccr, 0, null, null, new List<DataType> { PrimitiveType.Int32 });
            Assert.AreEqual("Stk: 0 void (o0)", ccr.ToString());
        }

        [Test]
        public void SvSparcPs_Load_IntArgs()
        {
            Given_CallingConvention();
            cc.Generate(ccr, 0, null, null, new List<DataType> { i16, i8, i32, i16, i8, i32, i8 });
            Assert.AreEqual("Stk: 0 void (o0, o1, o2, o3, o4, o5, Stack +0018)", ccr.ToString());
        }

        [Test]
        public void SvSparcPs_longlongArgs()
        {
            Given_CallingConvention();
            cc.Generate(ccr, 0, i64, null, new List<DataType> { i32, off_t, i64 });
            Assert.AreEqual("Stk: 0 Sequence o0:o1 (o0, Sequence o1:o2, Sequence o3:o4)", ccr.ToString());
        }

        [Test]
        public void SvSparcPs_longlongArgs_exceeding_max_register_args()
        {
            Given_CallingConvention();
            cc.Generate(ccr, 0, i64, null, new List<DataType> { i32, off_t, i64, off_t });
            Assert.AreEqual("Stk: 0 Sequence o0:o1 (o0, Sequence o1:o2, Sequence o3:o4, Sequence o5:stack)", ccr.ToString());
        }
    }
}
