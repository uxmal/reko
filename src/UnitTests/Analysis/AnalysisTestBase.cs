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
using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using Reko.Loading;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Analysis
{
	public abstract class AnalysisTestBase
	{
        protected IPlatform platform;
        protected Mock<IPlatform> platformMock;
        private ServiceContainer sc;

        public AnalysisTestBase()
        {
            //$TODO: this is a hard dependency on the file system.
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        protected void DumpProcedureFlows(Program program, DataFlowAnalysis dfa, TextWriter w)
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
				w.WriteLine("// {0} /////////////////////", proc.Name);
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				DataFlow.EmitRegisters(program.Architecture, "\tLiveOut:  ", flow.grfLiveOut, flow.BitsLiveOut, w);
				w.WriteLine();
				DataFlow.EmitRegisterValues("\tBitsUsed: ", flow.BitsUsed, w);
				w.WriteLine();
				DataFlow.EmitRegisters(program.Architecture, "\tTrashed:  ", flow.grfTrashed, flow.Trashed, w);
				w.WriteLine();
				DataFlow.EmitRegisters(program.Architecture, "\tPreserved:", flow.grfPreserved, flow.Preserved, w);
				w.WriteLine();

				w.WriteLine("// {0}", proc.Name);
				proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, new TextFormatter(w));
				w.WriteLine();
                foreach (Block block in proc.SortBlocksByName())
                {
                    block.Write(w);
                }
			}
		}

        private class FlowDecorator : BlockDecorator
        {
            public override void BeforeBlock(Block block, List<string> lines)
            {
                base.BeforeBlock(block, lines);
            }

            public override void AfterBlock(Block block, List<string> lines)
            {
                base.AfterBlock(block, lines);
            }
        }

        protected Program BuildProgram(ProcedureBuilder mock)
        {
            var m = new ProgramBuilder();
            m.Add(mock);
            var program = m.BuildProgram();
            program.CallGraph.AddProcedure(mock.Procedure);
            return program;
        }

        protected Program BuildProgram(Action<ProcedureBuilder> buildProc)
        {
            var m = new ProgramBuilder();
            var pb = new ProcedureBuilder();
            pb.ProgramBuilder = m;
            buildProc(pb);
            m.Add(pb);
            var program = m.BuildProgram();
            program.CallGraph.AddProcedure(pb.Procedure);
            return program;
        }

        protected Program BuildProgram(IProcessorArchitecture arch, Action<ProcedureBuilder> buildProc)
        {
            var m = new ProgramBuilder(arch);
            var pb = new ProcedureBuilder(arch);
            pb.ProgramBuilder = m;
            buildProc(pb);
            m.Add(pb);
            var program = m.BuildProgram();
            program.CallGraph.AddProcedure(pb.Procedure);
            return program;
        }

        protected Program RewriteFile(string relativePath)
        {
            return RewriteMsdosAssembler(relativePath, "");
        }

        protected static Program RewriteMsdosAssembler(string relativePath, string configFile)
        {
            var arch = new X86ArchitectureReal("x86-real-16");
            var sc = new ServiceContainer();
            var cfgSvcMock = new Mock<IConfigurationService>();
            var envMock = new Mock<PlatformDefinition>();
            var tlSvcMock = new Mock<ITypeLibraryLoaderService>();
            cfgSvcMock.Setup(c => c.GetEnvironment("ms-dos")).Returns(envMock.Object);
            envMock.Setup(e => e.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            envMock.Setup(e => e.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            sc.AddService<IConfigurationService>(cfgSvcMock.Object);
            sc.AddService<ITypeLibraryLoaderService>(tlSvcMock.Object);
            Program program;
            Assembler asm = new X86TextAssembler(sc, arch);
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                program = asm.Assemble(Address.SegPtr(0xC00, 0), rdr);
                program.Platform = new MsdosPlatform(sc, program.Architecture);
            }
            Rewrite(program, asm, configFile);
            return program;
        }

        protected static Program RewriteMsdosAssembler(string relativePath, Action<Program> postLoad)
        {
            var arch = new X86ArchitectureReal("x86-real-16");
            var sc = new ServiceContainer();
            var cfgSvc = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            cfgSvc.Setup(c => c.GetEnvironment("ms-dos")).Returns(env.Object);
            env.Setup(e => e.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(e => e.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<ITypeLibraryLoaderService>(tlSvc.Object);
            Program program;
            Assembler asm = new X86TextAssembler(sc, arch);
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                program = asm.Assemble(Address.SegPtr(0xC00, 0), rdr);
                program.Platform = new MsdosPlatform(sc, program.Architecture);
            }
            Rewrite(program, asm, postLoad);
            return program;
        }

        protected Program RewriteFile32(string sourceFile)
        {
            return RewriteFile32(sourceFile, null);
        }

        private Program RewriteFile32(string relativePath, string configFile)
        {
            Program program;
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var asm = new X86TextAssembler(sc, arch);
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                if (this.platform == null)
                {
                    this.platform = new Reko.Environments.Windows.Win32Platform(sc, arch);
                }
                asm.Platform = this.platform;
                program = asm.Assemble(Address.Ptr32(0x10000000), rdr);
            }
            foreach (var item in asm.ImportReferences)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }
            Rewrite(program, asm, configFile);
            return program;
        }

        protected Program RewriteCodeFragment(string s)
        {
            Assembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
            var program = asm.AssembleFragment(Address.SegPtr(0xC00, 0), s);
            program.Platform = new MsdosPlatform(null, program.Architecture);
            Rewrite(program, asm, (string)null);
            return program;
        }

        protected Program RewriteCodeFragment32(string s)
        {
            Assembler asm = new X86TextAssembler(sc, new X86ArchitectureFlat32("x86-protected-32"));
            var program = asm.AssembleFragment(Address.Ptr32(0x00400000), s);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            Rewrite(program, asm, (string)null);
            return program;
        }

        private static void Rewrite(Program program, Assembler asm, string configFile)
        {
            var fakeDiagnosticsService = new FakeDiagnosticsService();
            var fakeConfigService = new FakeDecompilerConfiguration();
            var eventListener = new FakeDecompilerEventListener();
            var sc = new ServiceContainer();
            sc.AddService<IDiagnosticsService>(fakeDiagnosticsService);
            sc.AddService<IConfigurationService>(fakeConfigService);
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            var loader = new Loader(sc);
            var project = string.IsNullOrEmpty(configFile)
                ? new Project()
                : new ProjectLoader(sc, loader, eventListener).LoadProject(FileUnitTester.MapTestPath(configFile));
            var scan = new Scanner(
                program,
                new DynamicLinker(project, program, eventListener),
                sc);

            scan.EnqueueImageSymbol(ImageSymbol.Procedure(program.Architecture, asm.StartAddress), true);
            foreach (var f in project.Programs)
            {
                foreach (var sp in f.User.Procedures.Values)
                {
                    scan.EnqueueUserProcedure(program.Architecture, sp);
                }
            }
            scan.ScanImage();
        }

        private static void Rewrite(Program program, Assembler asm, Action<Program> postLoad)
        {
            var fakeDiagnosticsService = new FakeDiagnosticsService();
            var fakeConfigService = new FakeDecompilerConfiguration();
            var eventListener = new FakeDecompilerEventListener();
            var sc = new ServiceContainer();
            sc.AddService<IDiagnosticsService>(fakeDiagnosticsService);
            sc.AddService<IConfigurationService>(fakeConfigService);
            sc.AddService<DecompilerEventListener>(eventListener);
            //sc.AddService<IDecompiledFileService>(new FakeDecompilerHost());
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            var loader = new Loader(sc);
            var project = new Project
            {
                Programs = { program }
            };
            postLoad(program);
            var scan = new Scanner(
                program,
                new DynamicLinker(project, program, eventListener),
                sc);

            scan.EnqueueImageSymbol(ImageSymbol.Location(program.Architecture, asm.StartAddress), true);
            foreach (var f in project.Programs)
            {
                foreach (var sp in f.User.Procedures.Values)
                {
                    scan.EnqueueUserProcedure(program.Architecture, sp);
                }
            }
            scan.ScanImage();
        }

        #region X86-specific
        // Run x86-specific test (deprecated for unit testing as they require a specific architecture --
        // try not to use these)
        public static void RunTest_x86_real(string sourceFile, Action<Program, TextWriter> test, string outputFile)
        {
            Program program = RewriteMsdosAssembler(sourceFile, (string)null);
            SaveRunOutput(program, test, outputFile);
        }

		protected void RunFileTest_x86_real(string sourceFile, string outputFile)
		{
			Program program = RewriteMsdosAssembler(sourceFile, (string)null);
            SaveRunOutput(program, RunTest, outputFile);
		}

        protected void RunFileTest_x86_real(string sourceFile, string configFile, string outputFile)
        {
            Program program = RewriteMsdosAssembler(sourceFile, configFile);
            SaveRunOutput(program, RunTest, outputFile);
        }

        protected void RunFileTest_x86_32(string sourceFile, string outputFile)
        {
            Program program = RewriteFile32(sourceFile);
            SaveRunOutput(program, RunTest, outputFile);
        }

        protected void RunFileTest_x86_32(string sourceFile, string configFile, string outputFile)
        {
            Program program = RewriteFile32(sourceFile, configFile);
            SaveRunOutput(program, RunTest, outputFile);
        }

        #endregion

        protected void RunFileTest(ProcedureBuilder mock, string outputFile)
        {
			Program program = BuildProgram(mock);
            SaveRunOutput(program, RunTest, outputFile);
		}

        protected void RunStringTest(string sExp, Action<ProcedureBuilder> m)
        {
            var program = BuildProgram(m);
            AssertRunOutput(program, RunTest, sExp);
        }

        protected void RunStringTest(string sExp, IProcessorArchitecture arch, Action<ProcedureBuilder> m)
        {
            var program = BuildProgram(arch, m);
            AssertRunOutput(program, RunTest, sExp);
        }


        protected void RunStringTest(string sExp, ProcedureBuilder pb)
        {
            var program = BuildProgram(pb);
            AssertRunOutput(program, RunTest, sExp);
        }

        protected void RunFileTest(string sourceFile, string configFile, string outputFile)
		{
			Program prog = RewriteMsdosAssembler(sourceFile, configFile);
            SaveRunOutput(prog, RunTest, outputFile);
		}

        protected void RunStringTest(string sExp, Program program)
        {
            AssertRunOutput(program, RunTest, sExp);
        }

        protected void RunFileTest(string outputFile, Action<ProcedureBuilder> m)
        {
            var program = BuildProgram(m);
            SaveRunOutput(program, RunTest, outputFile);
        }

        protected void RunFileTest(Program program, string outputFile)
        {
            SaveRunOutput(program, RunTest, outputFile);
        }

        // Override this to do the analysis.
        protected virtual void RunTest(Program program, TextWriter writer)
        {
        }

		protected static void SaveRunOutput(Program program, Action<Program, TextWriter> test, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				test(program, fut.TextWriter);
                fut.AssertFilesEqual();
			}
		}

        protected static void AssertRunOutput(Program program, Action<Program, TextWriter> test, string sExp)
        {
            var sw = new StringWriter();
            test(program, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        protected void Given_Platform(IPlatform platform)
        {
            this.platform = platform;
        }

        protected void Given_FakeWin32Platform()
        {
            this.platformMock = new Mock<IPlatform>();
            var tHglobal = new TypeReference("HGLOBAL", PrimitiveType.Ptr32);
            var tLpvoid = new TypeReference("LPVOID", PrimitiveType.Ptr32);
            var tBool = new TypeReference("BOOL", PrimitiveType.Int32);
            //platformMock.Setup(p => p.Architecture).Returns(arch.Object);
            platformMock.Setup(p => p.LookupProcedureByName(
                It.IsAny<string>(),
                "GlobalHandle"))
                .Returns(
                    new ExternalProcedure(
                        "GlobalHandle",
                        new FunctionType(
                            new Identifier("eax", tHglobal, Reko.Arch.X86.Registers.eax),
                            new Identifier[] {
                                new Identifier("pv",  tLpvoid, new StackArgumentStorage(4, PrimitiveType.Word32))
                            })
                        {
                            StackDelta = 4,
                        }));
            platformMock.Setup(p => p.LookupProcedureByName(
                It.IsAny<string>(),
                "GlobalUnlock"))
                .Returns(new ExternalProcedure(
                    "GlobalUnlock",
                    new FunctionType(
                        new Identifier("eax",  tBool, Reko.Arch.X86.Registers.eax),
                        new Identifier[] {
                            new Identifier("hMem", tHglobal, new StackArgumentStorage(4, PrimitiveType.Word32))
                        })
                    {
                        StackDelta = 4,
                    }));

            platformMock.Setup(p => p.LookupProcedureByName(
                It.IsAny<string>(),
                "GlobalFree"))
             .Returns(new ExternalProcedure(
                 "GlobalFree",
                 new FunctionType(
                     new Identifier("eax",  tBool, Reko.Arch.X86.Registers.eax),
                     new[] {
                        new Identifier("hMem", tHglobal, new StackArgumentStorage(4, PrimitiveType.Word32))
                     })
                     {
                         StackDelta = 4,
                     }));
            platformMock.Setup(p => p.GetTrampolineDestination(
                It.IsNotNull<IEnumerable<RtlInstructionCluster>>(),
                It.IsNotNull<IRewriterHost>()))
                .Returns((ProcedureBase)null);

            platformMock.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            platformMock.Setup(p => p.CreateImplicitArgumentRegisters()).Returns(
                new HashSet<RegisterStorage>());
            platformMock.Setup(p => p.MakeAddressFromLinear(It.IsAny<ulong>(), It.IsAny<bool>()))
                .Returns((ulong ul, bool b) => Address.Ptr32((uint) ul));
            platformMock.Setup(p => p.CreateTrashedRegisters())
                .Returns(new HashSet<RegisterStorage>());
            Given_Platform(platformMock.Object);
        }
	}
}
