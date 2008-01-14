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
using Decompiler.Loading;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class LoaderTests
	{
		[Test]
		public void LoadSpacesim()
		{
			Program prog = new Program();
			Loader loader = new Loader(prog);
			loader.LoadExecutable(FileUnitTester.MapTestPath("binaries/itp.exe"));
		}

		[Test]
		public void LoadOmni()
		{
			Program prog = new Program();
			Loader loader = new Loader(prog);
			loader.LoadBinary(FileUnitTester.MapTestPath("binaries/omni.com"), new Address(0xC000, 0));
		}

		[Test]
		public void LoadItp()
		{
			Program prog = new Program();
			Loader loader = new Loader(prog);
			loader.LoadExecutable(FileUnitTester.MapTestPath("binaries/itp.exe"));
		}

		[Test]
		public void LoadLunar()
		{
			Program prog = new Program();
			Loader loader = new Loader(prog);
			loader.LoadExecutable(FileUnitTester.MapTestPath("binaries/lunarcell-150.8bf"));
			Scanner scan = new Scanner(prog, null);
			foreach (EntryPoint ep in loader.EntryPoints)
			{
				scan.EnqueueEntryPoint(ep);
			}
			scan.ProcessQueues();
			using (FileUnitTester fut = new FileUnitTester("Loading/LoadLunar.txt"))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					fut.TextWriter.WriteLine(proc.Name);
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
