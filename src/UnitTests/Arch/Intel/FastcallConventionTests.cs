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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Core.Serialization;
using Reko.UnitTests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Xml;
using System.Xml.Serialization;
using Reko.Environments.Windows;
using System.Collections.Generic;
using CommonMockFactory = Reko.UnitTests.Mocks.CommonMockFactory;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class FastcallConventionTests
    {
        private CommonMockFactory mockFactory;
        private IntelArchitecture arch;
        private FastcallConvention fcc;
        private ICallingConventionEmitter ccr;
        private Win32Platform platform;
        private ISerializedTypeVisitor<DataType> deserializer;
        private PrimitiveType i16 = PrimitiveType.Int16;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType u64 = PrimitiveType.UInt64;
        private PrimitiveType r64 = PrimitiveType.Real64;

        [SetUp]
        public void Setup()
        {
            mockFactory = new CommonMockFactory();
            arch = new X86ArchitectureFlat32("x86-protected-32");
            platform = new Win32Platform(null, arch);
        }

        private void Given_32bit_CallingConvention()
        {
            this.ccr = new CallingConventionEmitter();
            this.deserializer = new FakeTypeDeserializer(32);
            FastcallConvention fcc = new FastcallConvention(
                Registers.ecx, Registers.edx, 4, 4);
            this.fcc = fcc;
        }

        private void Given_16bit_CallingConvention(string cConvention)
        {
            this.ccr = new CallingConventionEmitter();
            this.deserializer = new FakeTypeDeserializer(32);
            FastcallConvention fcc = new FastcallConvention(
                Registers.cx, Registers.dx, 4, 4);
            this.fcc = fcc;
        }

        [Test]
        public void FastCc_Load()
        {
            Given_32bit_CallingConvention();
            fcc.Generate(ccr, i32, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 4 eax (ecx)", ccr.ToString());
        }

        [Test]
        public void FastCc_instance_method()
        {
            Given_32bit_CallingConvention();
            fcc.Generate(ccr, i32, i32, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 4 eax [this ecx] (edx)", ccr.ToString());
        }
    }
}
