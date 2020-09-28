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
using Reko.Arch.Alpha;
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
    public class AlphaCallingConventionTests
    {
        private PrimitiveType i8 = PrimitiveType.Char;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType r32 = PrimitiveType.Real32;
        private PrimitiveType r64 = PrimitiveType.Real64;

        private AlphaCallingConvention cc;
        private ICallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            var arch = new AlphaArchitecture("alpha");
            this.cc = new AlphaCallingConvention(arch);
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
        public void WinAlphaCc_1_Param()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> { PrimitiveType.Char });
            var sExp = "Stk: 0 void (r16)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        public void WinAlphaCc_6_Params()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> {
                    PrimitiveType.Char,
                    new Pointer(PrimitiveType.Int32, 64),
                    PrimitiveType.UInt32,
                    PrimitiveType.Int32,
                    PrimitiveType.Int16,
                    PrimitiveType.WChar,
                });
            var sExp = "Stk: 0 void (r16, r17, r18, r19, r20, r21)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        public void WinAlphaCc_7_Params()
        {
            cc.Generate(
                ccr,
                null,
                null,
                new List<DataType> {
                    PrimitiveType.Char,
                    new Pointer(PrimitiveType.Int32, 64),
                    PrimitiveType.UInt32,
                    PrimitiveType.Int32,
                    PrimitiveType.Int16,
                    PrimitiveType.WChar,
                    new Pointer(PrimitiveType.Int32, 64),
                });
            var sExp = "Stk: 0 void (r16, r17, r18, r19, r20, r21, Stack +0000)";
            AssertEqual(sExp, ccr);
        }

        [Test]
        [Ignore("Not yet clear how Alpha deals with interleaved ints and floats")]
        public void WinAlphaCc_7_Real_Params()
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
        public void WinAlphaCc_AllInts()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, i32, i32, i32, i32, Ptr(i32) });
            Assert.AreEqual("Stk: 0 r0 (r16, r17, r18, r19, r20, r21)", ccr.ToString());
        }

        [Test]
        [Ignore("Need reliable documentation for the WinAlpha calling convention")]
        public void WinAlphaCc_AllFloats()
        {
            cc.Generate(ccr, r32, null, new List<DataType> { r32, r64, r32, r64, r32 });
            Assert.AreEqual("Stk: 0 xmm0 (xmm0, xmm1, xmm2, xmm3, Stack +0028)", ccr.ToString());
        }

        [Test]
        [Ignore("Need reliable documentation for the WinAlpha calling convention")]
        public void WinAlphaCc_MixedIntsFloats()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, r64, Ptr(i8), r64, r32 });
            Assert.AreEqual("Stk: 0 eax (rcx, xmm1, r8, xmm3, Stack +0028)", ccr.ToString());
        }


        [Test(Description = "Verifies that small stack arguments are properly aligned on stack")]
        [Ignore("Need reliable documentation for the WinAlpha calling convention")]
        public void WinAlphaCc_SmallStackArguments()
        {
            cc.Generate(ccr, i32, null, new List<DataType> { i32, r64, Ptr(i8), r64, i8, i8, i8 });
            Assert.AreEqual("Stk: 8 eax (rcx, xmm1, r8, xmm3, Stack +0028, Stack +0030, Stack +0038)", ccr.ToString());
        }

    }
}
