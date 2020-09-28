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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class Win32PlatformTests
    {
        private Mock<ITypeLibraryLoaderService> tlSvc;
        private ServiceContainer sc;
        private Win32Platform win32;
        private Program program;
        private IntelArchitecture arch;
        private TypeLibrary environmentMetadata;
		private ExternalProcedure extProc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            arch = new X86ArchitectureFlat32("x86-protected-32");
        }

        private void When_Lookup_Procedure(string moduleName, string procName)
        {
            this.extProc = this.win32.LookupProcedureByName(moduleName, procName);
        }

        [Test]
        public void Win32_ProcedureLookup_ReadsConfiguration()
        {
            Given_Configuration_With_Win32_Element();
            Given_TypeLibraryLoaderService();
            Expect_TypeLibraryLoaderService_LoadLibrary("windows.xml");

            When_Creating_Win32_Platform();
            When_Lookup_Procedure("kernel32","foo");

            tlSvc.Verify();
        }

        private void Expect_TypeLibraryLoaderService_LoadLibrary(TypeLibraryDefinition expected, TypeLibrary dstLib)
        {
            environmentMetadata = dstLib;
            tlSvc.Setup(t => t.LoadMetadataIntoLibrary(
                It.IsNotNull<IPlatform>(),
                It.Is<TypeLibraryDefinition>(a => a.Name == expected.Name),
                It.IsNotNull<TypeLibrary>()))
                .Returns(dstLib)
                .Verifiable();
        }

        private void Expect_TypeLibraryLoaderService_LoadLibrary(string expected)
        {
            Expect_TypeLibraryLoaderService_LoadLibrary(
                new TypeLibraryDefinition
                {
                    Name = expected
                },
                new TypeLibrary());
        }

        private void Expect_TypeLibraryLoaderService_LoadLibrary(string expected, IDictionary<string, DataType> types)
        {
            var tl = new TypeLibrary(
                types, 
                new Dictionary<string, FunctionType>(),
                new Dictionary<string, DataType>());

            Expect_TypeLibraryLoaderService_LoadLibrary(
                new TypeLibraryDefinition
                {
                    Name = expected,
                },
                tl);
        }

        private void Given_Configuration_With_Win32_Element()
        {
            var dcSvc = new Mock<IConfigurationService>();
            var opEnv = new PlatformDefinition 
            {
                TypeLibraries =
                {
                    new TypeLibraryDefinition
                    {
                        Name = "windows.xml"
                    }
                }
            };
            dcSvc.Setup(d => d.GetEnvironment("win32")).Returns(opEnv);
            dcSvc.Setup(c => c.GetInstallationRelativePath(
                It.IsAny<string[]>()))
                .Returns((string []s) => string.Join("/", s));
            
            sc.AddService<IConfigurationService>(dcSvc.Object);
        }

        private void When_Creating_Win32_Platform()
        {
            win32 = new Win32Platform(sc, arch);
        }

        private void Given_Program()
        {
            program = new Program
            {
                Architecture = win32.Architecture,
                Platform = win32,
                EnvironmentMetadata = environmentMetadata,
            };
        }

        private void Given_TypeLibraryLoaderService()
        {
            tlSvc = new Mock<ITypeLibraryLoaderService>();
            sc.AddService(typeof(ITypeLibraryLoaderService), tlSvc.Object);
        }

        [Test]
        public void Win32_SignatureFromName_stdcall()
        {
            Given_TypeLibraryLoaderService();
            Given_Configuration_With_Win32_Element();

            var fnName = "_foo@4";
            When_Creating_Win32_Platform();

            var sProc = win32.SignatureFromName(fnName);
            var loader = new TypeLibraryDeserializer(
                win32,
                false,
                new TypeLibrary());
            var ep = loader.LoadExternalProcedure(sProc);

            var sigExp =
@"define foo
// stackDelta: 8; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sigExp, ep.Signature.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void Win32_Deserialize_PlatformTypes()
        {
            var types = new Dictionary<string, DataType>()
            {
                { "TESTTYPE1", PrimitiveType.Create( PrimitiveType.Byte.Domain, 8 ) },
                { "TESTTYPE2", PrimitiveType.Create( PrimitiveType.Int16.Domain, 16 ) },
                { "TESTTYPE3", PrimitiveType.Create( PrimitiveType.Int32.Domain, 32 ) },
            };
            Given_TypeLibraryLoaderService();
            Expect_TypeLibraryLoaderService_LoadLibrary("windows.xml", types);
            Given_Configuration_With_Win32_Element();

            When_Creating_Win32_Platform();
            Given_Program();

            var ser = program.CreateProcedureSerializer();
            var sSig = new SerializedSignature
            {
                Convention = "__cdecl",
                ReturnValue = new Argument_v1(null, new TypeReference_v1("TESTTYPE1"), null, false),
                Arguments = new Argument_v1[]
                {
                    new Argument_v1("a", new TypeReference_v1("TESTTYPE2"), null, false),
                    new Argument_v1("b", new TypeReference_v1("TESTTYPE3"), null, false)
                }
            };
            var sig = ser.Deserialize(sSig, win32.Architecture.CreateFrame());

            var sigExp =
@"Register TESTTYPE1 foo(Stack TESTTYPE2 a, Stack TESTTYPE3 b)
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sigExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual("byte", (sig.ReturnValue.DataType as TypeReference).Referent.ToString());
            Assert.AreEqual("int16", (sig.Parameters[0].DataType as TypeReference).Referent.ToString());
            Assert.AreEqual("int32", (sig.Parameters[1].DataType as TypeReference).Referent.ToString());
        }

        [Test]
        public void Win32_GetPrimitiveTypeNames_C()
        {
            When_Creating_Win32_Platform();

            Assert.AreEqual("char",  win32.GetPrimitiveTypeName(PrimitiveType.Char, "C"));
            Assert.AreEqual("short", win32.GetPrimitiveTypeName(PrimitiveType.Int16, "C"));
            Assert.AreEqual("unsigned short",  win32.GetPrimitiveTypeName(PrimitiveType.UInt16, "C"));
            Assert.AreEqual("int",  win32.GetPrimitiveTypeName(PrimitiveType.Int32, "C"));
            Assert.AreEqual("unsigned int",  win32.GetPrimitiveTypeName(PrimitiveType.UInt32, "C"));
            Assert.AreEqual("__int64",  win32.GetPrimitiveTypeName(PrimitiveType.Int64, "C"));
            Assert.AreEqual("unsigned __int64",  win32.GetPrimitiveTypeName(PrimitiveType.UInt64, "C"));
        }

        [Test]
        public void Win32_VtblFromMsMangledName()
        {
            Given_TypeLibraryLoaderService();
            Given_Configuration_With_Win32_Element();
            When_Creating_Win32_Platform();

            var type = win32.DataTypeFromImportName("??_7Scope@@6B@");

            Assert.IsNull(type.Item2);
        }
    }
}
