#region License
/* 
 * Copyright (C) 1999-2016 John K�ll�n.
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
using Rhino.Mocks;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using System;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ArgumentDeserializerTests
    {
        private IntelArchitecture arch;
        private ProcedureSerializer sigser;
        private ArgumentDeserializer argser;
        private MsdosPlatform platform;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            arch = new X86ArchitectureReal();
            platform = new MsdosPlatform(sc, arch);
            sigser = mr.Stub<ProcedureSerializer>(
                arch,
                new TypeLibraryDeserializer(platform, true, new TypeLibrary()),
                "stdapi");
            argser = new ArgumentDeserializer(
                sigser,
                arch,
                arch.CreateFrame(),
                // It's possible that old, hand-written assembler passes
                // arguments on unaligned offsets
                13);
            mr.ReplayAll();
        }


        [Test]
        public void ArgSer_DeserializeRegister()
        {
            Register_v1 reg = new Register_v1("eax");
            Argument_v1 arg = new Argument_v1
            {
                Name = "eax",
                Kind = reg,
            };
            Identifier id = argser.Deserialize(arg);
            Assert.AreEqual("eax", id.Name);
            Assert.AreEqual(32, id.DataType.BitSize);
        }

        [Test]
        public void ArgSer_DeserializeReturnRegisterWithType()
        {
            var arg = new Argument_v1
            {
                Kind = new Register_v1("eax"),
                Type = new PointerType_v1 { DataType = new PrimitiveType_v1 { ByteSize = 1, Domain = Domain.Character } }
            };
            var id = argser.DeserializeReturnValue(arg);
            Assert.AreEqual("(ptr char)", id.DataType.ToString());
        }

        [Test]
        public void ArgSer_DeserializeRegisterWithType()
        {
            var arg = new Argument_v1
            {
                Kind = new Register_v1("eax"),
                Type = new PointerType_v1 { DataType = new PrimitiveType_v1 { ByteSize = 1, Domain = Domain.Character } }
            };
            var id = argser.Deserialize(arg);
            Assert.AreEqual("eax", id.Name);
            Assert.AreEqual("(ptr char)", id.DataType.ToString());
        }

        [Test]
        [Category(Categories.FailedTests)]
        public void ArgSer_DeserializeStackVariable()
        {
            var arg = new Argument_v1
            {
                Kind = new StackVariable_v1(),
                Type = new PointerType_v1 { DataType = new PrimitiveType_v1 { ByteSize = 1, Domain = Domain.Character } }
            };
            var id = argser.Deserialize(arg);
            Assert.AreEqual("ptrArg0D", id.Name);
            Assert.AreEqual("(ptr char)", id.DataType.ToString());
            Assert.AreEqual("Stack +000D", id.Storage.ToString());
        }

        [Test]
        [Category(Categories.FailedTests)]
        public void ArgSer_DeserializeTwoNamedStackVariables()
        {
            var argA = new Argument_v1
            {
                Name = "a",
                Kind = new StackVariable_v1(),
                Type = new PrimitiveType_v1 { ByteSize = 4, Domain = Domain.SignedInt }
            };
            var argB = new Argument_v1
            {
                Name = "b",
                Kind = new StackVariable_v1(),
                Type = new PrimitiveType_v1 { ByteSize = 4, Domain = Domain.Real }
            };
            var a = argser.Deserialize(argA);
            var b = argser.Deserialize(argB);
            Assert.AreEqual("a", a.Name);
            Assert.AreEqual("int32", a.DataType.ToString());
            Assert.AreEqual("Stack +000D", a.Storage.ToString());
            Assert.AreEqual("b", b.Name);
            Assert.AreEqual("real32", b.DataType.ToString());
            Assert.AreEqual("Stack +0011", b.Storage.ToString());
        }
    }
}
