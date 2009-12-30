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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class SignatureLibraryTests
	{
		[Test]
		public void SlLoad()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/SlLoad.txt"))
			{
				foreach (string s in new string [] { "msvcrt.xml", "kernel32.xml", "user32.xml" })
				{
					fut.TextWriter.WriteLine("** {0} ****", s);
					SignatureLibrary lib = new SignatureLibrary(new IntelArchitecture(ProcessorMode.Real));
					lib.Load(FileUnitTester.MapTestPath("../Environments/Win32/" + s));
					lib.Write(fut.TextWriter);
				}
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void SlLookup()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/SlLookup.txt"))
			{
				IProcessorArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
				SignatureLibrary kernel = new SignatureLibrary(arch);
				kernel.Load(FileUnitTester.MapTestPath("../Environments/Win32/kernel32.xml"));

				SignatureLibrary crt = new SignatureLibrary(arch);
                crt.Load(FileUnitTester.MapTestPath("../Environments/Win32/msvcrt.xml"));

				kernel.Lookup("CreateFileA").Emit("CreateFileA", ProcedureSignature.EmitFlags.ArgumentKind, fut.TextWriter);
				fut.TextWriter.WriteLine();
				crt.Lookup("malloc").Emit("malloc", ProcedureSignature.EmitFlags.ArgumentKind, fut.TextWriter);
				fut.TextWriter.WriteLine();
                Assert.IsNull(crt.Lookup("MaLloc"));
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void SlUser32()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/SlUser32.txt"))
			{
				IProcessorArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
				SignatureLibrary user32 = new SignatureLibrary(arch);
                user32.Load(FileUnitTester.MapTestPath("../Environments/Win32/user32.xml"));
				SortedList<string,ProcedureSignature> sigs = new SortedList<string,ProcedureSignature>(user32.Signatures);
				foreach (KeyValuePair<string,ProcedureSignature> de in sigs)
				{
					de.Value.Emit(de.Key, ProcedureSignature.EmitFlags.ArgumentKind|ProcedureSignature.EmitFlags.LowLevelInfo, fut.TextWriter);
				}
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void SlKernel32()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/SlKernel32.txt"))
			{
				IProcessorArchitecture arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
				SignatureLibrary kernel32 = new SignatureLibrary(arch);
                kernel32.Load(FileUnitTester.MapTestPath("../Environments/Win32/kernel32.xml"));
				EmitSignature(kernel32, "GlobalHandle", fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void EmitSignature(SignatureLibrary lib, string fnName, System.IO.TextWriter tw)
		{
			lib.Lookup(fnName).Emit(fnName, ProcedureSignature.EmitFlags.ArgumentKind|ProcedureSignature.EmitFlags.LowLevelInfo, tw);
		}
	}
}
