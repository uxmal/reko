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

using Decompiler;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Services;
using Decompiler.Environments.Win32;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class Rewrite32
	{
        private MockRepository mr;
        private Win32Platform win32;
        private IntelArchitecture arch;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            var services = mr.Stub<IServiceProvider>();
            var tlSvc = mr.Stub<ITypeLibraryLoaderService>();
            var configSvc = mr.StrictMock<IConfigurationService>();
            var win32env = new OperatingEnvironmentElement
            {
                TypeLibraries = 
                {
                    new TypeLibraryElement {  Name= "msvcrt.xml" },
                    new TypeLibraryElement {  Name= "windows32.xml" },
                }
            };
            configSvc.Stub(c => c.GetEnvironment("win32")).Return(win32env);
            configSvc.Stub(c => c.GetPath(null)).IgnoreArguments()
                .Do(new Func<string, string>(s => s));
            services.Stub(s => s.GetService(typeof(ITypeLibraryLoaderService))).Return(tlSvc);
            services.Stub(s => s.GetService(typeof(IConfigurationService))).Return(configSvc);
            tlSvc.Stub(t => t.LoadLibrary(null, null)).IgnoreArguments()
                .Do(new Func<IProcessorArchitecture, string, TypeLibrary>((a, n) =>
                {
                    var lib = TypeLibrary.Load(a, Path.ChangeExtension(n, ".xml"));
                    return lib;
                }));
            services.Replay();
            tlSvc.Replay();
            configSvc.Replay();
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            win32 = new Decompiler.Environments.Win32.Win32Platform(services, arch);
        }

		[Test]
		public void RwAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", "Intel/RwAutoArray32.txt");
		}

		[Test]
		public void RwMallocFree()
		{
			RunTest("Fragments/import32/mallocfree.asm", "Intel/RwMallocFree.txt");
		}

		[Test]
		public void RwFrame32()
		{
			RunTest("Fragments/multiple/frame32.asm", "Intel/RwFrame32.txt");
		}

		[Test]
		public void RwFtol()
		{
			RunTest("Fragments/import32/ftol.asm", "Intel/RwFtol.txt");
		}

		[Test]
		public void RwGlobalHandle()
		{
			RunTest("Fragments/import32/GlobalHandle.asm", "Intel/RwGlobalHandle.txt");
		}

		[Test]
		public void RwLoopMalloc()
		{
			RunTest("Fragments/import32/loopmalloc.asm", "Intel/RwLoopMalloc.txt");
		}

		[Test]
		public void RwLoopGetDC()
		{
			RunTest("Fragments/import32/loop_GetDC.asm", "Intel/RwLoopGetDC.txt");
		}

		[Test]
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

		private void RunTest(string sourceFile, string outputFile)
		{
			Program program;
            var asm = new X86TextAssembler(new X86ArchitectureFlat32());
            using (StreamReader rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                program = asm.Assemble(Address.Ptr32(0x10000000), rdr);
                program.Platform = win32;
            }
            foreach (var item in asm.ImportReferences)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }
            var project = new Project { Programs = { program } };
            Scanner scan = new Scanner(
                program,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            foreach (var ep in asm.EntryPoints)
            {
                scan.EnqueueEntryPoint(ep);
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