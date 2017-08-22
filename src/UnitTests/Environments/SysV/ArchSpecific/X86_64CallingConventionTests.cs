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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Environments.SysV;
using Reko.Core.Types;
using Reko.UnitTests.Core.Serialization;
using Reko.UnitTests.Mocks;
using System;
using System.Xml;
using System.Xml.Serialization;
using Rhino.Mocks;
using Reko.Environments.SysV.ArchSpecific;
using System.Collections.Generic;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class X86_64CallingConventionTests
    {
        private X86ArchitectureFlat64 arch;
        private X86_64CallingConvention cc;
        private PrimitiveType i8 = PrimitiveType.SByte;
        private PrimitiveType i16 = PrimitiveType.Int16;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType r64 = PrimitiveType.Real64;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat64();
        }

        private void Given_CallingConvention()
        {
            this.cc = new X86_64CallingConvention(arch);
        }

        [Test]
        public void SvAmdCc_DeserializeFpuArgument()
        {
            Given_CallingConvention();
            var ccr = cc.Generate(i32, null, new List<DataType> { r64 });
            Assert.AreEqual("Stk: 8 eax (xmm0)", ccr.ToString());
        }

        [Test]
        public void SvAmdCc_DeserializeFpuStackReturnValue()
        {
            Given_CallingConvention();
            var ccr = cc.Generate(r64, null, new List<DataType>());
            Assert.AreEqual("Stk: 8 xmm0 ()", ccr.ToString());
        }

        [Test]
        public void SvAmdCc_Load_cdecl()
        {
            Given_CallingConvention();
            var ccr = cc.Generate(null, null, new List<DataType> { i32 } );
            Assert.AreEqual("Stk: 8 void (rdi)", ccr.ToString());
        }

        [Test]
        public void SvAmdCc_Load_IntArgs()
        {
            Given_CallingConvention();
            var ccr = cc.Generate(null, null, new List<DataType> { i16, i8, i32, i16, i8, i32, i8, i32 });
            Assert.AreEqual(
                "Stk: 8 void (rdi, rsi, rdx, rcx, r8, r9, Stack +0008, Stack +0010)",
                ccr.ToString());
        }

        [Test]
        public void SvAmdCc_Return_Short()
        {
            Given_CallingConvention();
            var ccr = cc.Generate(PrimitiveType.Int16, null, new List<DataType>());
            Assert.AreEqual("Stk: 8 ax ()", ccr.ToString());
        }
    }
}
