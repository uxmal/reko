#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core.Configuration;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Environments.Win32;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Xml;

namespace Decompiler.UnitTests.Environments.Win32
{
    [TestFixture]
    public class Win32PlatformTests
    {
        private MockRepository repository;
        private ITypeLibraryLoaderService tlSvc;
        private ServiceContainer sc;
        private Win32Platform win32;
        private IntelArchitecture arch;
        private ExternalProcedure extProc;
        private OperatingEnvironment opEnv;
        private IConfigurationService dcSvc;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            sc = new ServiceContainer();
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            dcSvc = repository.StrictMock<IConfigurationService>();
            opEnv = repository.StrictMock<OperatingEnvironment>();
        }

        private void When_Lookup_Procedure(string moduleName, string procName)
        {
            this.extProc = this.win32.LookupProcedureByName(moduleName, procName);
        }

        private void Given_Win32_TypeLibraries(string name)
        {
            var typelibs = new TypeLibraryElementCollection
            {
                new TypeLibraryElement
                {
                    Name = name
                }
            };
            opEnv.Expect(o => o.TypeLibraries).Return(typelibs);
        }

        private void Expect_GetEnvironment_Win32()
        {
            dcSvc.Expect(d => d.GetEnvironment("win32"))
                .Return(opEnv);
        }

        [Test]
        public void Win32_ProcedureLookup_ReadsConfiguration()
        {
            Given_Configuration_With_Win32_Element();
            Given_TypeLibraryLoaderService();
            Expect_TypeLibraryLoaderService_LoadLibrary("windows.xml");
            repository.ReplayAll();

            When_Creating_Win32_Platform();
            When_Lookup_Procedure("kernel32","foo");

            repository.VerifyAll();
        }

        private void Expect_TypeLibraryLoaderService_LoadLibrary(string expected)
        {
            tlSvc.Expect(t => t.LoadLibrary(
                Arg<IProcessorArchitecture>.Is.NotNull,
                Arg<string>.Is.Equal(expected))).
                Return(new TypeLibrary());
        }

        private void Given_Configuration_With_Win32_Element()
        {
            var dcSvc = repository.Stub<IConfigurationService>();
            var opEnv = new OperatingEnvironmentElement 
            {
                TypeLibraries =
                {
                    new TypeLibraryElement
                    {
                        Name = "windows.xml"
                    }
                }
            };
            dcSvc.Expect(d => d.GetEnvironment("win32")).Return(opEnv);
            dcSvc.Stub(c => c.GetPath(null)).IgnoreArguments()
                .Do(new Func<string, string>(s => s));
            
            sc.AddService<IConfigurationService>(dcSvc);
        }

        private void When_Creating_Win32_Platform()
        {
            win32 = new Win32Platform(sc, arch);
        }

        private void Expect_CreateFileStream_PathEndsWith(string filename)
        {
            tlSvc.Expect(f => f.LoadLibrary(
                Arg<IProcessorArchitecture>.Is.Anything,
                Arg<string>.Matches(s => filename.EndsWith(s))))
                .Return(CreateFakeLib());
        }

        private TypeLibrary CreateFakeLib()
        {
            return new TypeLibrary();
        }

        private void Given_TypeLibraryLoaderService()
        {
            tlSvc = repository.StrictMock<ITypeLibraryLoaderService>();
            sc.AddService(typeof(ITypeLibraryLoaderService), tlSvc);
        }

        [Test]
        public void Win32_SignatureFromName_stdcall()
        {
            var fnName = "_foo@4";
            When_Creating_Win32_Platform();

            var sig = win32.SignatureFromName(fnName);

            Assert.AreEqual("void ()()\r\n// stackDelta: 8; fpuStackDelta: 0; fpuMaxParam: -1\r\n", sig.ToString());
        }
    }
}
