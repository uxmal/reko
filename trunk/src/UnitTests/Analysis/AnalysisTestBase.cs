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

using Decompiler.Analysis;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Configuration;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Environments.Msdos;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	public abstract class AnalysisTestBase
	{
        private Platform platform;

		protected void DumpProcedureFlows(Program prog, DataFlowAnalysis dfa, RegisterLiveness live, TextWriter w)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				w.WriteLine("// {0} /////////////////////", proc.Name);
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				DataFlow.EmitRegisters(prog.Architecture, "\tLiveOut:  ", flow.grfLiveOut, flow.LiveOut, w);
				w.WriteLine();
				DataFlow.EmitRegisters(prog.Architecture, "\tMayUseIn: ", flow.grfMayUse, flow.MayUse, w);
				w.WriteLine();
				DataFlow.EmitRegisters(prog.Architecture, "\tBypassIn: ", flow.grfMayUse, flow.ByPass, w);
				w.WriteLine();
				DataFlow.EmitRegisters(prog.Architecture, "\tTrashed:  ", flow.grfTrashed, flow.TrashedRegisters, w);
				w.WriteLine();
				DataFlow.EmitRegisters(prog.Architecture, "\tPreserved:", flow.grfPreserved, flow.PreservedRegisters, w);
				w.WriteLine();

				w.WriteLine("// {0}", proc.Name);
				proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, new TextFormatter(w));
				w.WriteLine();
				foreach (Block block in proc.SortBlocksByName())
				{
                    if (live != null)
                    {
                        var bFlow = dfa.ProgramDataFlow[block];
                        bFlow.WriteBefore(prog.Architecture, w);
                        block.Write(w);
                        bFlow.WriteAfter(prog.Architecture, w);
                        w.WriteLine();
                    }
                    else
                    {
                        block.Write(w);
                    }
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

        protected Program BuildProgramMock(ProcedureBuilder mock)
        {
            var m = new ProgramBuilder();
            m.Add(mock);
            var prog = m.BuildProgram();
            prog.CallGraph.AddProcedure(mock.Procedure);
            return prog;
        }

        protected Program RewriteFile(string relativePath)
        {
            return RewriteMsdosAssembler(relativePath, "");
        }

        protected static Program RewriteMsdosAssembler(string relativePath, string configFile)
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            Program program;
            Assembler asm = new X86TextAssembler(arch);
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                program = asm.Assemble(Address.SegPtr(0xC00, 0), rdr);
                program.Platform = new MsdosPlatform(null, arch);
            }
            Rewrite(program, asm, configFile);
            return program;
        }

        protected Program RewriteFile32(string sourceFile)
        {
            return RewriteFile32(sourceFile, null);
        }

        private Program RewriteFile32(string relativePath, string configFile)
        {
            Program program;
            var asm = new X86TextAssembler(new X86ArchitectureReal());
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                if (this.platform == null)
                {
                    this.platform = new Decompiler.Environments.Win32.Win32Platform(null, new X86ArchitectureFlat32());
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
            Assembler asm = new X86TextAssembler(new X86ArchitectureReal());
            var program = asm.AssembleFragment(Address.SegPtr(0xC00, 0), s);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            Rewrite(program, asm, null);
            return program;
        }

        private static void Rewrite(Program prog, Assembler asm, string configFile)
        {
            var fakeDiagnosticsService = new FakeDiagnosticsService();
            var fakeConfigService = new FakeDecompilerConfiguration();
            var sc = new ServiceContainer();
            sc.AddService(typeof(IDiagnosticsService), fakeDiagnosticsService);
            sc.AddService(typeof(IConfigurationService), fakeConfigService);
            var loader = new Loader(sc);
            var project = string.IsNullOrEmpty(configFile)
                ? new Project()
                : new ProjectLoader(loader).LoadProject(FileUnitTester.MapTestPath(configFile));
            var scan = new Scanner(
                prog,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            
            scan.EnqueueEntryPoint(new EntryPoint(asm.StartAddress, prog.Architecture.CreateProcessorState()));
            foreach (var f in project.Programs)
            {
                foreach (var sp in f.UserProcedures.Values)
                {
                    scan.EnqueueUserProcedure(sp);
                }
            }
            scan.ScanImage();
        }

        public static void RunTest(string sourceFile, Action<Program, TextWriter> test, string outputFile)
        {
            Program prog = RewriteMsdosAssembler(sourceFile, null);
            SaveRunOutput(prog, test, outputFile);
        }

		protected void RunTest(string sourceFile, string outputFile)
		{
			Program prog = RewriteMsdosAssembler(sourceFile, null);
            SaveRunOutput(prog, RunTest, outputFile);
		}

		protected void RunTest(ProcedureBuilder mock, string outputFile)
		{
			Program prog = BuildProgramMock(mock);
            SaveRunOutput(prog, RunTest, outputFile);
		}

		protected void RunTest(string sourceFile, string configFile, string outputFile)
		{
			Program prog = RewriteMsdosAssembler(sourceFile, configFile);
            SaveRunOutput(prog, RunTest, outputFile);
		}

        protected void RunTest(Program prog, string outputFile)
        {
            SaveRunOutput(prog, RunTest, outputFile);
        }

		protected void RunTest32(string sourceFile, string outputFile)
		{
			Program prog = RewriteFile32(sourceFile);
            SaveRunOutput(prog, RunTest, outputFile);
		}

		protected void RunTest32(string sourceFile, string configFile, string outputFile)
		{
			Program prog = RewriteFile32(sourceFile, configFile);
			SaveRunOutput(prog, RunTest, outputFile);
		}

        protected virtual void RunTest(Program prog, TextWriter writer)
        {
        }

		protected static void SaveRunOutput(Program prog, Action<Program, TextWriter> test, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				test(prog, fut.TextWriter);
                fut.AssertFilesEqual();
			}
		}

        protected void Given_Platform(Platform platform)
        {
            this.platform = platform;
        }

        protected void Given_FakeWin32Platform(MockRepository mr)
        {
            var platform = mr.StrictMock<Platform>(null, null);
            var tHglobal = new TypeReference("HGLOBAL", PrimitiveType.Pointer32);
            var tLpvoid = new TypeReference("LPVOID", PrimitiveType.Pointer32);
            var tBool = new TypeReference("BOOL", PrimitiveType.Int32);
            platform.Stub(p => p.LookupProcedureByName(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal("GlobalHandle")))
                .Return(
                    new ExternalProcedure(
                        "GlobalHandle",
                        new ProcedureSignature(
                            new Identifier("eax", tHglobal, Decompiler.Arch.X86.Registers.eax),
                            new Identifier("pv",  tLpvoid, new StackArgumentStorage(0, PrimitiveType.Word32)))
                        {
                            StackDelta = 4,
                        }));
            platform.Stub(p => p.LookupProcedureByName(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal("GlobalUnlock")))
                .Return(new ExternalProcedure(
                    "GlobalUnlock",
                    new ProcedureSignature(
                        new Identifier("eax",  tBool, Decompiler.Arch.X86.Registers.eax),
                        new Identifier("hMem", tHglobal, new StackArgumentStorage(0, PrimitiveType.Word32)))
                    {
                        StackDelta = 4,
                    }));

            platform.Stub(p => p.LookupProcedureByName(
             Arg<string>.Is.Anything,
             Arg<string>.Is.Equal("GlobalFree")))
             .Return(new ExternalProcedure(
                 "GlobalFree",
                 new ProcedureSignature(
                     new Identifier("eax",  tBool, Decompiler.Arch.X86.Registers.eax),
                     new Identifier("hMem", tHglobal, new StackArgumentStorage(0, PrimitiveType.Word32)))
                 {
                     StackDelta = 4,
                 }));
            platform.Stub(p => p.GetTrampolineDestination(
                Arg<ImageReader>.Is.NotNull,
                Arg<IRewriterHost>.Is.NotNull))
                .Return(null);

            platform.Stub(p => p.PointerType).Return(PrimitiveType.Pointer32);
            platform.Stub(p => p.CreateImplicitArgumentRegisters()).Return(
                new Decompiler.Arch.X86.X86ArchitectureFlat32().CreateRegisterBitset());
            Given_Platform(platform);
        }
	}
}
