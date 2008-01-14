/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Analysis;
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using System.IO;
using System.Collections;

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
				proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, w);
				w.WriteLine();
				foreach (Block block in proc.RpoBlocks)
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


		protected Program BuildProgramMock(ProcedureMock mock)
		{
			ProgramMock m = new ProgramMock();
			m.Add(mock);
			Program prog = m.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			dfa.UntangleProcedures();
			return prog;
		}

		private Program RewriteFile(string relativePath, string configFile)
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			prog.Platform = new Arch.Intel.MsDos.MsdosPlatform(prog.Architecture);
			Assembler asm = prog.Architecture.CreateAssembler();
			prog.Image = asm.Assemble(prog, new Address(0xC00, 0), FileUnitTester.MapTestPath(relativePath), null);
			Rewrite(prog, asm, configFile);
			return prog;
		}

		protected Program RewriteFile(string relativePath)
		{
			return RewriteFile(relativePath, null);
		}

		protected Program RewriteFile32(string sourceFile)
		{
			return RewriteFile32(sourceFile, null);
		}

		private Program RewriteFile32(string relativePath, string configFile)
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
			Assembler asm = prog.Architecture.CreateAssembler();
			prog.Image = asm.Assemble(prog, new Address(0x10000000), FileUnitTester.MapTestPath(relativePath), null);
			Rewrite(prog, asm, configFile);
			return prog;
		}

		protected Program RewriteCode(string s)
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = prog.Architecture.CreateAssembler();
			prog.Image = asm.AssembleFragment(prog, new Address(0xC00, 0), s);
			Rewrite(prog, asm, null);
			return prog;
		}

		private void Rewrite(Program prog, Assembler asm, string configFile)
		{
			Scanner scan = new Scanner(prog, null);
			DecompilerProject project = new DecompilerProject();
			if (configFile != null)
			{
				project = DecompilerProject.Load(FileUnitTester.MapTestPath(configFile));
			}
			EntryPoint ep = new EntryPoint(asm.StartAddress, new IntelState());
			prog.AddEntryPoint(ep);
			scan.EnqueueEntryPoint(ep);
			foreach (SerializedProcedure sp in project.UserProcedures)
			{
				scan.EnqueueUserProcedure(sp);
			}
			scan.ProcessQueues();
			RewriterHost rw = new RewriterHost(prog, null, scan.SystemCalls, scan.VectorUses);
			rw.RewriteProgram();
		}


		protected void RunTest(string sourceFile, string outputFile)
		{
			Program prog = RewriteFile(sourceFile);
			SaveRunOutput(prog, outputFile);
		}

		protected void RunTest(ProcedureMock mock, string outputFile)
		{
			Program prog = BuildProgramMock(mock);
			SaveRunOutput(prog, outputFile);
		}

		protected void RunTest(string sourceFile, string configFile, string outputFile)
		{
			Program prog = RewriteFile(sourceFile, configFile);
			SaveRunOutput(prog, outputFile);
		}

		protected void RunTest32(string sourceFile, string outputFile)
		{
			Program prog = RewriteFile32(sourceFile);
			SaveRunOutput(prog, outputFile);
		}

		protected void RunTest32(string sourceFile, string configFile, string outputFile)
		{
			Program prog = RewriteFile32(sourceFile, configFile);
			SaveRunOutput(prog, outputFile);
		}

		protected virtual void RunTest(Program prog, FileUnitTester fut)
		{
		}

		private void SaveRunOutput(Program prog, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				RunTest(prog, fut);
				fut.AssertFilesEqual();
			}
		}
	}
}
