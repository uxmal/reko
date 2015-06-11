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
using Decompiler.Core.Configuration;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Typing
{
	/// <summary>
	/// Base class for all typing tests.
	/// </summary>
	public abstract class TypingTestBase
	{
		protected Program RewriteFile16(string relativePath) { return RewriteFile(relativePath, Address.SegPtr(0xC00, 0)); }

		protected Program RewriteFile32(string relativePath) { return RewriteFile(relativePath, Address.Ptr32(0x00100000)); }

		protected Program RewriteFile(string relativePath, Address addrBase)
		{
            var services = new ServiceContainer();
            var config = new FakeDecompilerConfiguration();
            services.AddService<IConfigurationService>(config);
            ILoader ldr = new Loader(services);
            var program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(relativePath),
                new X86TextAssembler(new IntelArchitecture(ProcessorMode.Real)),
                addrBase);
            program.Platform = new DefaultPlatform(services, program.Architecture);
            var ep = new EntryPoint(program.Image.BaseAddress, program.Architecture.CreateProcessorState());
            var project = new Project { Programs = { program } };
			var scan = new Scanner(
                program,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
			scan.EnqueueEntryPoint(ep);
			scan.ScanImage();

			var dfa = new DataFlowAnalysis(program, new FakeDecompilerEventListener());
			dfa.AnalyzeProgram();
            return program;
		}

        protected void RunHexTest(string hexFile, string outputFile)
        {
            var svc = new ServiceContainer();
            var cfg = new FakeDecompilerConfiguration();
            svc.AddService<IConfigurationService>(cfg);
            ILoader ldr = new Loader(svc);
            var imgLoader = new DchexLoader(FileUnitTester.MapTestPath( hexFile), svc, null);
            var img = imgLoader.Load(null);
            var program = new Program(img.Image, img.Image.CreateImageMap(), img.Architecture, img.Platform);
            var project = new Project { Programs = { program } };
            var ep = new EntryPoint(program.Image.BaseAddress, program.Architecture.CreateProcessorState());
            var scan = new Scanner(program, new Dictionary<Address, ProcedureSignature>(), new ImportResolver(project), new FakeDecompilerEventListener());
            scan.EnqueueEntryPoint(ep);
            scan.ScanImage();

            var dfa = new DataFlowAnalysis(program, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram();
            RunTest(program, outputFile);
        }

        protected void RunTest16(string srcfile, string outputFile)
        {
            RunTest(RewriteFile16(srcfile), outputFile);
        }

        protected void RunTest32(string srcfile, string outputFile)
        {
            RunTest(RewriteFile32(srcfile), outputFile);
        }
        
        protected void RunTest(ProgramBuilder mock, string outputFile)
        {
            Program prog = mock.BuildProgram();
            DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
            dfa.DumpProgram();
            dfa.BuildExpressionTrees();
            RunTest(prog, outputFile);
        }

        protected void RunTest(Action<ProcedureBuilder> pg, string outputFile)
        {
            ProcedureBuilder m = new ProcedureBuilder();
            pg(m);
            ProgramBuilder prog = new ProgramBuilder();
            prog.Add(m);
            RunTest(prog, outputFile);
        }

        protected void RunTest(Program prog)
        {
            var outputFileName = string.Format("Typing/{0}.txt", new StackTrace().GetFrame(1).GetMethod().Name);
            RunTest(prog, outputFileName);
        }

        protected abstract void RunTest(Program prog, string outputFile);

		protected void DumpSsaInfo(Procedure proc, SsaState ssa, TextWriter writer)
		{
			writer.WriteLine("// {0} ////////////////////////////////", proc.Name);
			DumpSsaTypes(ssa, writer);
			proc.Write(false, writer);
			writer.WriteLine();
		}

		protected void DumpSsaTypes(SsaState ssa, TextWriter writer)
		{
			foreach (SsaIdentifier id in ssa.Identifiers)
			{
				if (id.Identifier.TypeVariable != null)
					writer.WriteLine("{0}: {1}", id.Identifier, id.Identifier.TypeVariable);
			}
		}

		protected MemoryAccess MemLoad(Identifier id, int offset, DataType size)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, 
				new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, id, Constant.Word32(offset)),
				size);
		}
	}
}
