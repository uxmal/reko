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
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Environments.Windows;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class Rewrite32
	{
        private Win32Platform win32;
        private IntelArchitecture arch;
        private ServiceContainer services;
        private FakeDecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            eventListener = new FakeDecompilerEventListener();
            this.services = new ServiceContainer();
            var tlSvc = new TypeLibraryLoaderServiceImpl(services);
            var configSvc = new Mock<IConfigurationService>();
            var win32env = new PlatformDefinition
            {
                TypeLibraries = 
                {
                    new TypeLibraryDefinition {  Name= "msvcrt.xml" },
                    new TypeLibraryDefinition {  Name= "windows32.xml" },
                }
            };
            configSvc.Setup(c => c.GetEnvironment("win32")).Returns(win32env);
            configSvc.Setup(c => c.GetInstallationRelativePath(It.IsAny<string>()))
                .Returns((string[] s) => RekoConfigurationService.MakeInstallationRelativePath(s));
            services.AddService(typeof(ITypeLibraryLoaderService), tlSvc);
            services.AddService(typeof(IConfigurationService), configSvc.Object);
            services.AddService(typeof(DecompilerEventListener), eventListener);
            services.AddService(typeof(CancellationTokenSource), new CancellationTokenSource());
            services.AddService(typeof(IFileSystemService),new FileSystemServiceImpl());
            services.AddService(typeof(IDiagnosticsService),new FakeDiagnosticsService());
            arch = new X86ArchitectureFlat32("x86-protected-32");
            win32 = new Reko.Environments.Windows.Win32Platform(services, arch);
        }

		[Test]
		public void RwAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", "Intel/RwAutoArray32.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RwMallocFree()
		{
			RunTest("Fragments/import32/mallocfree.asm", "Intel/RwMallocFree.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RwFrame32()
		{
			RunTest("Fragments/multiple/frame32.asm", "Intel/RwFrame32.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RwFtol()
		{
			RunTest("Fragments/import32/ftol.asm", "Intel/RwFtol.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RwGlobalHandle()
		{
			RunTest("Fragments/import32/GlobalHandle.asm", "Intel/RwGlobalHandle.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RwLoopMalloc()
		{
			RunTest("Fragments/import32/loopmalloc.asm", "Intel/RwLoopMalloc.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RwLoopGetDC()
		{
			RunTest("Fragments/import32/loop_GetDC.asm", "Intel/RwLoopGetDC.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RwReg00004()
		{
			RunTest("Fragments/regressions/r00004.asm", "Intel/RwReg00004.txt");
		}

		[Test]
		public void RwReg00006()
		{
			RunTest("Fragments/regressions/r00006.asm", "Intel/RwReg00006.txt");
		}

		[Test]
		public void RwSwitch32()
		{
			RunTest("Fragments/switch32.asm", "Intel/RwSwitch32.txt");
		}

        [Test]
        public void RwSwitchReg00001()
        {
            RunTest("Fragments/switch_reg00001.asm", "Intel/RwSwitchReg00001.txt");
        }

        [Test]
        public void RwNotFoundImport()
        {
            RunTest("Fragments/import32/not_found_import.asm", "Intel/RwNotFoundImport.txt");
            Assert.AreEqual(
                "WarningDiagnostic - 10000014 - Unable to guess parameters of msvcrt!_not_found_import.",
                eventListener.LastDiagnostic);
        }

        private void RunTest(string sourceFile, string outputFile)
		{
			Program program;
            var asm = new X86TextAssembler(services, new X86ArchitectureFlat32("x86-protected-32"));
            using (StreamReader rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                program = asm.Assemble(Address.Ptr32(0x10000000), rdr);
                program.Platform = win32;
                program.User.Heuristics.Add("shingle");
            }
            foreach (var item in asm.ImportReferences)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }
            var project = new Project { Programs = { program } };
            Scanner scan = new Scanner(
                program,
                new DynamicLinker(project, program, eventListener),
                services);
            foreach (var ep in asm.EntryPoints)
            {
                scan.EnqueueImageSymbol(ep, true);
            }
            scan.ScanImage();

			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				foreach (Procedure proc in program.Procedures.Values)
				{
					proc.Write(true, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
