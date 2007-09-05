/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Diagnostic = Decompiler.Diagnostic;
using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Decompiler.Tests
{
	[TestFixture]
	public class DecompilerTests
	{
		private DecompilerDriver deco;
		private int phases;
		private int segsExpected;

		public DecompilerTests()
		{
		}

		[SetUp]
		public void Setup()
		{
			phases = 0;
			deco = new DecompilerDriver();
			deco.Loaded += new ProgramLoadedEventHandler(deco_Loaded);
			deco.Scanned += new EventHandler(deco_Scanned);
			deco.Rewritten += new EventHandler(deco_Rewritten);
			deco.DataAnalyzed += new EventHandler(deco_DataAnalyzed);
			deco.ProgramStructured += new EventHandler(deco_ProgramStructured);
			deco.Finished += new EventHandler(deco_Finished);
		}

		[Test]
		public void DecompileSpacesim()
		{
			segsExpected = 0x1A;
			DecompilerProject proj = DecompilerProject.Load(FileUnitTester.MapTestPath("binaries/spacesim.xml"));
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/spacesim.exe");
			deco.Decompile(proj, new TestDecompilerHost(proj));
			Assert.AreEqual(6, phases);
		}

		[Test]
		public void DecompileItp()
		{
			segsExpected = 0x1F;
			DecompilerProject proj = new DecompilerProject();
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/itp.exe");
			deco.Decompile(proj, new TestDecompilerHost(proj));
			Assert.IsTrue(phases == 6);
		}

		[Test]
		public void DecompileKKrieger()
		{
			segsExpected = 2;
			DecompilerProject proj = new DecompilerProject();
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/kkrieger.exe");
			deco.Decompile(proj, new TestDecompilerHost(proj));
			Assert.AreSame(-1, phases);
		}

		[Test]
		public void DecompileLunar()
		{
			segsExpected = 5;
			DecompilerProject proj = DecompilerProject.Load(FileUnitTester.MapTestPath("binaries/lunarcell-150.xml"));
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/lunarcell-150.8bf");
			deco.Decompile(proj, new TestDecompilerHost(proj));
		}

		[Test]
		public void DecompileTrailerP()
		{
			segsExpected = 4;
			DecompilerProject proj = DecompilerProject.Load(FileUnitTester.MapTestPath("trailer.xml"));
			proj.Input.Filename = FileUnitTester.MapTestPath("trailer.p");
			proj.Input.BaseAddress = new Address(0x10000000);
			deco.Decompile(proj, new TestDecompilerHost(proj));
		}

		[Test]
		public void DecompileLife()
		{
			segsExpected = 12;
			DecompilerProject proj = new DecompilerProject();
			proj.Input.Filename = FileUnitTester.MapTestPath("life.exe");
			deco.Decompile(proj, new TestDecompilerHost(proj));
		}

		[Test]
		public void DecompileOmni()
		{
			segsExpected = 1;
			DecompilerProject proj = new DecompilerProject();
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/omni.com");
			proj.Input.FileFormat = InputFormat.COM;
			proj.Input.BaseAddress = new Address(0x800, 0);
			proj.Output.OutputFilename = FileUnitTester.MapTestPath("binaries/omni.dis");
			deco.Decompile(proj, new TestDecompilerHost(proj));
		}

		[Test]
		public void DecompileColony()
		{
			segsExpected = 59;
			DecompilerProject proj = new DecompilerProject();
			proj.Input.Filename = FileUnitTester.MapTestPath("binaries/col.exe");
			proj.Input.FileFormat= InputFormat.Exe;
			proj.Input.BaseAddress = new Address(0x800, 0);
			proj.Output.DisassemblyFilename = "";
			deco.Decompile(proj, new TestDecompilerHost(proj));
		}

		public void deco_Loaded(object o, ProgramLoadedEventArgs e)
		{
			int segs = e.Loader.ImageMap.Segments.Count;
			Assert.AreEqual(segsExpected, segs);
			++phases;
		}

		public void deco_Scanned(object o, EventArgs e)
		{
			++phases;
		}

		public void deco_Rewritten(object o, EventArgs e)
		{
			++phases;
		}

		public void deco_DataAnalyzed(object o, EventArgs e)
		{
			++phases;
		}

		public void deco_ProgramStructured(object o, EventArgs e)
		{
			++phases;
		}

		public void deco_Finished(object o, EventArgs e)
		{
			++phases; 
		}

		[Test]
		public void WalkSpacesim()
		{
			Program pgm = new Program();
			Loader ld = new Loader(pgm);
			ld.LoadExecutable(FileUnitTester.MapTestPath("spacesim.exe"));
			Scanner sc = new Scanner(pgm, ld.ImageMap, null);
			sc.Parse(ld.EntryPoints);
		}

		[Test]
		public void WalkItp()
		{
			Program pgm = new Program();
			Loader ld = new Loader(pgm);
			ld.LoadExecutable(FileUnitTester.MapTestPath("itp.exe"), new Address(0x0C00, 0));

			Scanner sc = new Scanner(pgm, ld.ImageMap, null);
			sc.Parse(ld.EntryPoints);
		}

		[Test]
		public void WalkLunar()
		{
			Program pgm = new Program();
			Loader ld = new Loader(pgm);
			ld.LoadExecutable(FileUnitTester.MapTestPath(@"lunarcell-150.8bf"), new Address(0x10000000));

			Scanner sc = new Scanner(pgm, ld.ImageMap, null);
			sc.Parse(ld.EntryPoints);
		}

		private class TestDecompilerHost : DecompilerHost
		{
			private FileStream stm;
			private TextWriter output;

			public TestDecompilerHost(DecompilerProject project)
			{
				string s = Path.GetFullPath(project.Input.Filename);
				string t = Path.GetDirectoryName(s);
				string outFilename = Path.Combine(t, Path.GetFileNameWithoutExtension(project.Input.Filename)) + ".dis";
				stm = new FileStream(outFilename, FileMode.Create);
				output = new StreamWriter(stm, new UTF8Encoding(false, false));
			}

			#region DecompilerHost Members

			public TextWriter DecompiledCodeWriter
			{
				get { return output; }
			}

			public TextWriter DisassemblyWriter
			{
				get { return output; }
			}

			public TextWriter IntermediateCodeWriter
			{
				get { return output; }
			}

			public TextWriter TypesWriter
			{
				get { return output; }
			}

			public void Finished()
			{
				output.Close();
			}

			public void WriteDiagnostic(Diagnostic d, string format, params object[] args)
			{
				output.Write("// {0}: ", d);
				output.WriteLine(format, args);
			}

			public void ShowProgress(string caption, int numerator, int denominator)
			{
				// TODO:  Add TestDecompilerHost.ShowProgress implementation
			}

			#endregion
		}
	}
}
