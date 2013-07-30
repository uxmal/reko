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
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;

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
				proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, new Formatter(w));
				w.WriteLine();
				foreach (Block block in proc.SortBlocksByName())
				{
					block.Write(w);
					if (live != null)
					{
						dfa.ProgramDataFlow[block].Emit(prog.Architecture, w);
						w.WriteLine();
					}
				}
			}
		}


        protected Program BuildProgramMock(ProcedureBuilder mock)
        {
            var m = new ProgramBuilder();
            m.Add(mock);
            var prog = m.BuildProgram();
            prog.CallGraph.AddProcedure(mock.Procedure);
            DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
            dfa.UntangleProcedures();
            return prog;
        }


        protected Program RewriteFile(string relativePath)
        {
            return RewriteMsdosAssembler(relativePath, "");
        }

        protected static Program RewriteMsdosAssembler(string relativePath, string configFile)
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            Program prog = new Program
            {
                Architecture = arch,
                Platform = new MsdosPlatform(arch),
            };
            Assembler asm = new IntelTextAssembler();
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                asm.Assemble(new Address(0xC00, 0), rdr);
                prog.Image = asm.Image;
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
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Protected32);
            Assembler asm = new IntelTextAssembler();
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                asm.Assemble(new Address(0x10000000), rdr);
            }
            prog.Image = asm.Image;
            prog.Architecture = asm.Architecture;
            foreach (KeyValuePair<uint, PseudoProcedure> item in asm.ImportThunks)
            {
                prog.ImportThunks.Add(item.Key, item.Value);
            }
            Rewrite(prog, asm, configFile);
            return prog;
        }

        protected Program RewriteCodeFragment(string s)
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            Assembler asm = new IntelTextAssembler();
            asm.AssembleFragment(new Address(0xC00, 0), s);
            prog.Image = asm.Image;
            Rewrite(prog, asm, null);
            return prog;
        }

        private static void Rewrite(Program prog, Assembler asm, string configFile)
        {
            var scan = new Scanner(prog, 
                new Dictionary<Address, ProcedureSignature>(), new FakeDecompilerEventListener());
            var project = string.IsNullOrEmpty(configFile)
                ? new Project()
                : new ProjectSerializer().LoadProject(FileUnitTester.MapTestPath(configFile));
            
            scan.EnqueueEntryPoint(new EntryPoint(asm.StartAddress, prog.Architecture.CreateProcessorState()));
            foreach (var sp in project.UserProcedures.Values)
            {
                scan.EnqueueUserProcedure(sp);
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
