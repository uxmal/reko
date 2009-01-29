/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.Text;

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
			host.WriteDiagnostic(Diagnostic.Info, new Address(42), "{0}, world!", "Hello");

			Assert.AreEqual("Info - 0000002A: Hello, world!", fake.LastDiagnostic);

		}

		[Test]
		public void DhGetDisassemblyWriter()
		{
			using (TextWriter w = host.CreateDisassemblyWriter())
			{
				w.WriteLine("Hello");
				w.WriteLine("world");
			}
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
			using (TextWriter w = host.CreateTypesWriter("Nils"))
			{
				w.WriteLine("Nils");
			}
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

		public void WriteDiagnostic(Diagnostic d, Address addr, string format, params object[] args)
		{
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}: ", d, addr);
            sb.AppendFormat(format, args);
            lastDiagnostic = sb.ToString();
            Console.Out.WriteLine(lastDiagnostic);
            System.Diagnostics.Debug.WriteLine(lastDiagnostic);
		}


		public TextWriter CreateDecompiledCodeWriter(string file)
		{
			return decompiled;
		}

		public TextWriter CreateDisassemblyWriter()
		{
			return disassembly;
		}

		public TextWriter GetIntermediateCodeWriter()
		{
			return null;
		}

		public TextWriter CreateTypesWriter(string fileName)
		{
			return typesWriter;
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

		public StringWriter DisassemblyWriter
		{
			get { return disassembly; }
		}

		public StringWriter TypesWriter
		{
			get { return typesWriter; }
		}
	}
}
