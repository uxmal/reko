#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Analysis;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using Decompiler.Environments.Msdos;
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Decompiler.UnitTests.Analysis
{
	public abstract class AnalysisTestBase
	{
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
            Program prog;
            Assembler asm = new IntelTextAssembler();
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                var lr = asm.Assemble(new Address(0xC00, 0), rdr);
                prog = new Program(lr.Image, lr.ImageMap, lr.Architecture, new MsdosPlatform(null, arch));
            }
            Rewrite(prog, asm, configFile);
            return prog;
        }

        protected Program RewriteFile32(string sourceFile)
        {
            return RewriteFile32(sourceFile, null);
        }

        private Program RewriteFile32(string relativePath, string configFile)
        {
            Program prog;
            Assembler asm = new IntelTextAssembler();
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                var lr = asm.Assemble(new Address(0x10000000), rdr);
                prog = new Program(lr.Image, lr.ImageMap, lr.Architecture, new Decompiler.Environments.Win32.Win32Platform(null, lr.Architecture));
            }
            foreach (var item in asm.ImportReferences)
            {
                prog.ImportReferences.Add(item.Key, item.Value);
            }
            Rewrite(prog, asm, configFile);
            return prog;
        }

        protected Program RewriteCodeFragment(string s)
        {
            Assembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(new Address(0xC00, 0), s);
            var prog = new Program(lr.Image, lr.ImageMap, lr.Architecture, new DefaultPlatform(null, lr.Architecture));
            Rewrite(prog, asm, null);
            return prog;
        }

        private static void Rewrite(Program prog, Assembler asm, string configFile)
        {
            var loader = new Loader(new ServiceContainer());
            var project = string.IsNullOrEmpty(configFile)
                ? new Project()
                : new ProjectLoader(new Loader(new ServiceContainer())).LoadProject(FileUnitTester.MapTestPath(configFile));
            var scan = new Scanner(
                prog,
                project,
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
	}
}
