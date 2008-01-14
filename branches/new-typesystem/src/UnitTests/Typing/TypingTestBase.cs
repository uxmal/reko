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
using Decompiler.Analysis;
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Loading;
using Decompiler.Scanning;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.UnitTests.Typing
{
	/// <summary>
	/// Base class for all typing tests.
	/// </summary>
	public class TypingTestBase
	{
		protected Program prog;
		protected InductionVariableCollection ivs;

		protected void RewriteFile(string relativePath)
		{
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Loader ldr = new Loader(prog);
			ldr.Assemble(FileUnitTester.MapTestPath(relativePath), prog.Architecture, new Address(0xC00, 0));
			EntryPoint ep = new EntryPoint(prog.Image.BaseAddress, new IntelState());
			prog.AddEntryPoint(ep);
			
			Scanner scan = new Scanner(prog, null);
			scan.EnqueueEntryPoint(ep);
			scan.ProcessQueues();
			RewriterHost rw = new RewriterHost(prog, null, scan.SystemCalls, scan.VectorUses);
			rw.RewriteProgram();

			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			dfa.AnalyzeProgram();
			ivs = dfa.InductionVariables;
		}

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
				if (id.id.TypeVariable != null)
					writer.WriteLine("{0}: {1}", id.id, id.id.TypeVariable);
			}
		}

		protected MemoryAccess MemLoad(Identifier id, int offset, DataType size)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, 
				new BinaryExpression(Operator.add, PrimitiveType.Word32, id, Constant.Word32(offset)),
				size);
		}
	}
}
