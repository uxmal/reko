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
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class X86_64CallingConventionTests
    {
        private PrimitiveType i8 = PrimitiveType.Char;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType r32 = PrimitiveType.Real32;
        private PrimitiveType r64 = PrimitiveType.Real64;

        private X86_64CallingConvention cc;
        private ICallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            this.cc = new X86_64CallingConvention();
            this.ccr = new CallingConventionEmitter();
        }

        private void AssertEqual(string sExp, ICallingConventionEmitter ccr)
        {
            var sActual = ccr.ToString();
            if (sExp != sActual)
            {
                Debug.Print("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            } 
        }

        [Test]
        public void X86_64_cc_1_Param()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> { PrimitiveType.Char });
            var sExp = "Stk: 8 void (rcx)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        public void X86_64_cc_4_Params()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> {
                    PrimitiveType.Char,
                    new Pointer(PrimitiveType.Int32, 64),
                    PrimitiveType.UInt64,
                    PrimitiveType.Int32
                });
            var sExp = "Stk: 8 void (rcx, rdx, r8, r9)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        public void X86_64_cc_5_Params()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> {
                    PrimitiveType.Char,
                    new Pointer(PrimitiveType.Int32, 64),
                    PrimitiveType.UInt64,
                    PrimitiveType.Int32,
                    PrimitiveType.Int16,
                });
            var sExp = "Stk: 8 void (rcx, rdx, r8, r9, Stack +0028)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        public void X86_64_cc_5_Real_Params()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> { r32, r64, r32, r64, r32 });
            var sExp = "Stk: 8 void (xmm0, xmm1, xmm2, xmm3, Stack +0028)";
            AssertEqual(sExp, ccr);
        }

        private Pointer Ptr(DataType dt)
        {
            return new Pointer(dt, 64);
        }

        [Test]
        public void X86_64Cc_AllInts()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, i32, i32, i32, i32, Ptr(i32) });
            Assert.AreEqual("Stk: 8 eax (rcx, rdx, r8, r9, Stack +0028, Stack +0030)", ccr.ToString());
        }

        [Test]
        public void X86_64Cc_AllFloats()
        {
            cc.Generate(ccr, r32, null, new List<DataType> { r32, r64, r32, r64, r32 });
            Assert.AreEqual("Stk: 8 xmm0 (xmm0, xmm1, xmm2, xmm3, Stack +0028)", ccr.ToString());
        }

        [Test]
        public void X86_64Cc_MixedIntsFloats()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, r64, Ptr(i8), r64, r32 });
            Assert.AreEqual("Stk: 8 eax (rcx, xmm1, r8, xmm3, Stack +0028)", ccr.ToString());
        }


        [Test(Description = "Verifies that small stack arguments are properly aligned on stack")]
        public void X86_64Cc_SmallStackArguments()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, r64, Ptr(i8), r64, i8, i8, i8 });
            Assert.AreEqual("Stk: 8 eax (rcx, xmm1, r8, xmm3, Stack +0028, Stack +0030, Stack +0038)", ccr.ToString());
        }
    }
}
