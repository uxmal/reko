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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.MacOS.Classic;
using System.Collections.Generic;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class PascalCallingConventionTests
    {
        private M68kArchitecture arch;
        private CallingConventionEmitter emitter;

        public PascalCallingConventionTests()
        {
            this.arch = new M68kArchitecture("m68k");
        }

        [SetUp]
        public void Setup()
        {
            this.emitter = new CallingConventionEmitter();
        }

        [Test]
        public void Sbcc_VoidFn()
        {
            var sbcc = new PascalCallingConvention(arch);
            sbcc.Generate(emitter, VoidType.Instance, null, new List<DataType> { PrimitiveType.Word32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004)", emitter.ToString());
        }

        [Test]
        public void Sbcc_FnReturningValueOnStack()
        {
            var sbcc = new PascalCallingConvention(arch);
            sbcc.Generate(emitter, PrimitiveType.Int16, null, new List<DataType> { });
            Assert.AreEqual("Stk: 4 Stack +0004 ()", emitter.ToString());
        }

        [Test]
        public void Sbcc_FnTwoArgs()
        {
            var sbcc = new PascalCallingConvention(arch);
            sbcc.Generate(emitter, VoidType.Instance, null, new List<DataType> { PrimitiveType.Word32, PrimitiveType.Int16 });
            Assert.AreEqual("Stk: 10 void (Stack +0006, Stack +0004)", emitter.ToString());
        }

        [Test]
        public void Sbcc_FnTwoArgs_ReturningWord16()
        {
            var sbcc = new PascalCallingConvention(arch);
            sbcc.Generate(emitter, PrimitiveType.Word16, null, new List<DataType> { PrimitiveType.Word32, PrimitiveType.Int16 });
            Assert.AreEqual("Stk: 10 Stack +000A (Stack +0006, Stack +0004)", emitter.ToString());
        }

        [Test]
        public void Sbcc_FnTwoArgs_ReturningWord32()
        {
            var sbcc = new PascalCallingConvention(arch);
            sbcc.Generate(emitter, PrimitiveType.Word32, null, new List<DataType> { PrimitiveType.Word32, PrimitiveType.Int16 });
            Assert.AreEqual("Stk: 10 Stack +000A (Stack +0006, Stack +0004)", emitter.ToString());
        }
    }
}
