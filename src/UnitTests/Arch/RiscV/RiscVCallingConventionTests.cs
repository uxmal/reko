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
using Reko.Arch.RiscV;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVCallingConventionTests
    {
        private RiscVArchitecture arch;
        private RiscVCallingConvention cc;
        private static PrimitiveType i32 = PrimitiveType.Int32;
        private static PrimitiveType i64 = PrimitiveType.Int64;
        private static PrimitiveType w128 = PrimitiveType.Word128;
        private static PrimitiveType r32 = PrimitiveType.Real32;
        private static PrimitiveType r64 = PrimitiveType.Real64;

        [SetUp]
        public void Setup()
        {
            this.arch = new RiscVArchitecture();
            this.cc = new RiscVCallingConvention(arch);
        }

        [Test]
        public void RiscVCc_SingleArg()
        {
            var ccr = cc.Generate(null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (a0)", ccr.ToString());
        }

        [Test]
        public void RiscVCc_ReturnInt()
        {
            var ccr = cc.Generate(i64, null, new List<DataType> { r64 });
            Assert.AreEqual("Stk: 0 a0 (fa0)", ccr.ToString());
        }

        [Test]
        public void RiscVCc_ReturnLongLong()
        {
            var ccr = cc.Generate(r64, null, new List<DataType> { i32, w128 });
            Assert.AreEqual("Stk: 0 fa0 (a0, Sequence a3:a2)", ccr.ToString());
        }

        [Test]
        public void RiscVcc_ArgsOnStack()
        {
            var ccr = cc.Generate(null, null,
                new List<DataType> { w128, w128, w128, i32, w128, r64 });

            Assert.AreEqual(
                "Stk: 0 void (Sequence a1:a0, " +
                             "Sequence a3:a2, " +
                             "Sequence a5:a4, " +
                             "a6, " +
                             "Stack +0000, " +
                             "Stack +0010)",
                ccr.ToString());
        }

        [Test]
        public void RiscVcc_MixedRegs()
        {
            var ccr = cc.Generate(null, null,
                new List<DataType> { r64, i32, r32, i64, r64, i32, r32, i64, r32, i32 });
            Assert.AreEqual(
                "Stk: 0 void (fa0, a1, fa2, a3, fa4, a5, fa6, a7, Stack +0000, Stack +0008)",
                ccr.ToString());
        }
    }
}
