#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            sc = new ServiceContainer();
            arch = new IntelArchitecture(ProcessorMode.Protected32);
        }

        [Test]
        public void Win32_Create_OpensWin32_xml()
        {
            Given_Configuration_With_Win32_Element();
            Given_TypeLibraryLoaderService();
            Expect_CreateFileStream_PathEndsWith("win32.xml");
            repository.ReplayAll();

            When_Creating_Win32_Platform();

            repository.VerifyAll();
        }

        [Test]
        public void Win32_Create_ReadsConfiguration()
        {
            Given_Configuration_With_Win32_Element();
            Given_TypeLibraryLoaderService();
            Expect_TypeLibraryLoaderService_LoadLibrary("win32.xml");
            repository.ReplayAll();

            When_Creating_Win32_Platform();

            repository.VerifyAll();
        }

        private void Expect_TypeLibraryLoaderService_LoadLibrary(string expected)
        {
            tlSvc.Expect(t => t.LoadLibrary(
                Arg<IProcessorArchitecture>.Is.NotNull,
                Arg<string>.Is.Equal(expected))).
                Return(new SignatureLibrary(arch));
        }

        private void Given_Configuration_With_Win32_Element()
        {
            var envs = new ArrayList
            {
                
            };
            var dcSvc = repository.Stub<IDecompilerConfigurationService>();
            var opEnv = new OperatingEnvironmentElement();
            opEnv.TypeLibraries.Add(new TypeLibraryElement
            {
                Name = "win32.xml"
            });
            dcSvc.Expect(d => d.GetEnvironment("Win32")).Return(opEnv);
            sc.AddService<IDecompilerConfigurationService>(dcSvc);
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

        private SignatureLibrary CreateFakeLib()
        {
            return new SignatureLibrary(arch);
        }

        private void Given_TypeLibraryLoaderService()
        {
            tlSvc = repository.StrictMock<ITypeLibraryLoaderService>();
            sc.AddService(typeof(ITypeLibraryLoaderService), tlSvc);
        }
    }
}
