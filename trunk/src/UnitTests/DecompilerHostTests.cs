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

using Decompiler.Core;
using DecompilerHost = Decompiler.DecompilerHost;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests
{
	[TestFixture]
	public class DecompilerHostTests
	{
		private DecompilerHost host;
		private FakeDecompilerHost fake;

		[SetUp]
		public void Setup()
		{
			fake = new FakeDecompilerHost();
			host = fake;
		}

		[Test]
		public void DhWriteDiagnostic()
		{
			host.WriteDiagnostic(Diagnostic.Info, "{0}, world!", "Hello");

			Assert.AreEqual("Hello, world!", fake.LastDiagnostic);

		}

		[Test]
		public void DhGetDisassemblyWriter()
		{
			host.DisassemblyWriter.WriteLine("Hello");
			host.DisassemblyWriter.WriteLine("world");

			Assert.AreEqual("Hello" + Environment.NewLine + "world" + Environment.NewLine, fake.DisassemblyWriter.ToString());
		}

		[Test]
		public void DhProgress()
		{
			host.ShowProgress("zorking", 3, 10);

			Assert.AreEqual("zorking: 30%", fake.LastProgress);
		}

		[Test]
		public void DhTypes()
		{
			host.TypesWriter.WriteLine("Nils");
			Assert.AreEqual("Nils" + Environment.NewLine, fake.TypesWriter.ToString());
		}

	}

	public class FakeDecompilerHost : DecompilerHost
	{
		private string lastDiagnostic;
		private string lastProgress;
		private bool finishedCalled;
		private StringWriter disassembly = new StringWriter();
		private StringWriter decompiled = new StringWriter();
		private StringWriter typesWriter = new StringWriter();

		public void WriteDiagnostic(Diagnostic d, string format, params object[] args)
		{
			lastDiagnostic = string.Format(format, args);
			WriteDiagnostic(Console.Out, d, format, args);
			StringWriter sb = new StringWriter();
			WriteDiagnostic(sb, d, format, args);
			System.Diagnostics.Debug.Write(sb.ToString());
		}

		private void WriteDiagnostic(TextWriter sb, Diagnostic d, string format, params object[] args)
		{
			sb.Write(d.ToString());
			sb.Write(": ");
			sb.WriteLine(format, args);
		}

		public TextWriter DecompiledCodeWriter
		{
			get { return decompiled; }
		}

		public TextWriter DisassemblyWriter
		{
			get { return disassembly; }
		}

		public TextWriter IntermediateCodeWriter
		{
			get { return null; }
		}

		public TextWriter TypesWriter
		{
			get { return typesWriter; }
		}

		public void Finished()
		{
			finishedCalled = true;
		}

		public void ShowProgress(string caption, int numerator, int denominator)
		{
			lastProgress = string.Format("{0}: {1}%", caption, (numerator * 100) / denominator);
		}

		public void CodeStructuringComplete()
		{
		}

		public void DecompilationFinished()
		{
		}

		public void InterproceduralAnalysisComplete()
		{
		}

		public void MachineCodeRewritten()
		{
		}

		public void ProceduresTransformed()
		{
		}

		public void ProgramLoaded()
		{
		}

		public void ProgramScanned()
		{
		}

		public void TypeReconstructionComplete()
		{
		}

		// Diagnostic methods.

		public bool FinishedCalled
		{
			get { return finishedCalled; }
		}

		public string LastDiagnostic
		{
			get { return lastDiagnostic; }
		}

		public string LastProgress
		{
			get { return lastProgress; }
		}
	}
}
