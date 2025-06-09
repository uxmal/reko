#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.UnitTests
{
	public class FileUnitTester : IDisposable
	{
		private string testOutputFile;
		private string testExpectedFile;
		private TextWriter stm;

		public FileUnitTester(string testOutputFile)
		{
			Init(testOutputFile);
		}
		
		private void Init(string testOutputFile)
		{
			this.testOutputFile = MapTestPath(testOutputFile);
			this.testExpectedFile = string.Format("{0}/{1}.exp",
				Path.GetDirectoryName(this.testOutputFile),
				Path.GetFileNameWithoutExtension(this.testOutputFile));
			this.stm = new StreamWriter(this.testOutputFile, false, new UTF8Encoding(false));
		}
		
		public static string MapTestPath(string relativePath)
		{
			return string.Format("{0}/{1}", TestDirectory, relativePath);
		}

		public static void RunTest(string testOutputFile, Action<FileUnitTester> handler)
		{
			using (FileUnitTester fut = new FileUnitTester(testOutputFile))
			{
				handler(fut);
				fut.AssertFilesEqual();
			}
		}

		public static string TestDirectory
		{
			get 
			{
                string assemblyUri = typeof(FileUnitTester).Assembly.Location;
                string assemblyName = new Uri(assemblyUri).LocalPath;
                var iUnitTests = assemblyName.IndexOf("UnitTests");
                if (iUnitTests <= 0)
                    throw new NotSupportedException("Directory structure is expected to be '.../UnitTests/...'");
                return Path.Combine(assemblyName.Remove(iUnitTests), "tests");
			}
		}
		
		public TextWriter TextWriter
		{
			get { return stm; }
		}
		
		public void CloseTestStream()
		{
			if (stm is not null)
			{
				stm.Flush();
				stm.Close();
				stm = null;
			}
		}

		[DebuggerHidden]
		public void CompareFiles(StreamReader expected, StreamReader test)
		{
			int line = 1;
			for (;;)
			{
				string tstLine =  test.ReadLine();
				string expLine =  expected.ReadLine();

				if (tstLine is null && expLine is null)
					return;
				Assert.IsNotNull(expLine, string.Format("File should have ended before line {0}", line));
				Assert.IsNotNull(tstLine, string.Format("File ended unexpectedly at line {0}", line));
				Assert.AreEqual(expLine, tstLine, string.Format("File differs on line {0}", line));
				++line;
			}
		}

		public void Dispose()
		{
			CloseTestStream();
			System.GC.SuppressFinalize(this);
		}

		[DebuggerHidden]
		public void AssertFilesEqual()
		{
			CloseTestStream();
			using (StreamReader test = new StreamReader(testOutputFile),
				   expected = new StreamReader(testExpectedFile, new UTF8Encoding(false)))
			{
				CompareFiles(expected, test);
			}
		}
	}
}
