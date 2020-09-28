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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class MipsCallingConventionTests
    {
        private PrimitiveType i32 = PrimitiveType.Int32;
        private VoidType v = VoidType.Instance;

        private MipsLe32Architecture arch;
        private Reko.Environments.Windows.MipsCallingConvention cc;
        private ICallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsLe32Architecture("mips-le-32");
        }

        private Pointer Ptr(DataType dt)
        {
            return new Pointer(dt, 32);
        }

        private StructureType LargeInt()
        {
            var largeInt = new StructureType
            {
                Size = 8,
                Fields =
                {
                    { 0x0000, PrimitiveType.Word32 },
                    { 0x0004, PrimitiveType.Word32 },
                }
            };
            Debug.Assert(largeInt.GetInferredSize() == 8);
            return largeInt;
        }

        private void Given_CallingConvention()
        {
            var cc = new MipsCallingConvention(arch);
            this.cc = cc;
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void MipsCc_ReturnRegister()
        {
            Given_CallingConvention();
            cc.Generate(ccr, i32, null, new List<DataType> { });
            Assert.AreEqual("Stk: 0 r2 ()", ccr.ToString());
        }

        [Test]
        public void MipsCc_CharArg()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null,new List<DataType> { Ptr(v) });
            Assert.AreEqual("Stk: 0 void (r4)", ccr.ToString());
        }

        [Test]
        public void MipsCc_ManyArgs()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null, new List<DataType> { Ptr(v), Ptr(v), Ptr(v), Ptr(v), Ptr(v) });
            Assert.AreEqual("Stk: 0 void (r4, r5, r6, r7, Stack +0010)", ccr.ToString());
        }

        [Test(Description = "The spec says that when an argument whose size is > 32 bits is straddling the 4 word boundary"  + 
            "it should spill the variable into a stack location.")]
        public void MipsCc_LargeIntegerStraddling4WordBoundary()
        {
            Given_CallingConvention();
            var largeInt = LargeInt();
            cc.Generate(ccr, null, null, new List<DataType> {
                PrimitiveType.Word32,
                largeInt,
                largeInt,
                largeInt                // this straddles the 4-word limit.
            });
            Assert.AreEqual("Stk: 0 void (r4, Sequence r5:r6, Stack +0010, Stack +0018)", ccr.ToString());
        }
    }
}
