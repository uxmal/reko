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
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using Reko.Environments.Windows;
using Reko.Loading;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Typing
{
	/// <summary>
	/// Base class for all typing tests.
	/// </summary>
	public abstract class TypingTestBase
	{
        protected ServiceContainer sc;

        protected Program RewriteFile16(string relativePath) { return RewriteFile(relativePath, Address.SegPtr(0xC00, 0), (s, a) => new MsdosPlatform(s,a)); }

		protected Program RewriteFile32(string relativePath) { return RewriteFile(relativePath, Address.Ptr32(0x00100000), (s, a) => new Win32Platform(s, a)); }

		protected Program RewriteFile(
            string relativePath,
            Address addrBase,
            Func<IServiceProvider, IProcessorArchitecture, IPlatform> mkPlatform)
		{
            sc = new ServiceContainer();
            var config = new FakeDecompilerConfiguration();
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IConfigurationService>(config);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            var arch = new X86ArchitectureReal("x86-real-16");
            ILoader ldr = new Loader(sc);
            var program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(relativePath),
                new X86TextAssembler(sc, arch),
                addrBase);
            program.Platform = mkPlatform(sc, program.Architecture);
            var ep = ImageSymbol.Procedure(arch, program.SegmentMap.BaseAddress);
            var project = new Project { Programs = { program } };
            var scan = new Scanner(
                program,
                new DynamicLinker(project, program, eventListener),
                sc);
			scan.EnqueueImageSymbol(ep, true);
			scan.ScanImage();

            var dynamicLinker = new DynamicLinker(project, program, eventListener);
            var dfa = new DataFlowAnalysis(program, dynamicLinker, eventListener);
			dfa.AnalyzeProgram();
            return program;
		}

        protected void RunHexTest(string hexFile, string outputFile)
        {
            var svc = new ServiceContainer();
            var cfg = new FakeDecompilerConfiguration();
            var eventListener = new FakeDecompilerEventListener();
            svc.AddService<IConfigurationService>(cfg);
            svc.AddService<DecompilerEventListener>(eventListener);
            svc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            ILoader ldr = new Loader(svc);
            var imgLoader = new DchexLoader(FileUnitTester.MapTestPath( hexFile), svc, null);
            var program = imgLoader.Load(null);
            var project = new Project { Programs = { program } };
            var ep = ImageSymbol.Procedure(program.Architecture, program.ImageMap.BaseAddress);
            var dynamicLinker = new DynamicLinker(project, program, eventListener);
            var scan = new Scanner(program, dynamicLinker, svc);
            scan.EnqueueImageSymbol(ep, true);
            scan.ScanImage();

            var dfa = new DataFlowAnalysis(program, null, eventListener);
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
            Program program = mock.BuildProgram();
            var dynamicLinker = new Mock<IDynamicLinker>();
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, dynamicLinker.Object, new FakeDecompilerEventListener());
            dfa.UntangleProcedures();
            dfa.BuildExpressionTrees();
            RunTest(program, outputFile);
        }

        protected virtual void RunTest(Action<ProcedureBuilder> pg, string outputFile)
        {
            ProcedureBuilder m = new ProcedureBuilder();
            pg(m);
            ProgramBuilder program = new ProgramBuilder();
            program.Add(m);
            RunTest(program, outputFile);
        }

        protected abstract void RunTest(Program program, string outputFile);

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
