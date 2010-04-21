/* 
 * Copyright (C) 1999-2010 John Källén.
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

using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests
{
	public delegate void FileUnitTestHandler(FileUnitTester fut);

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

		public static void RunTest(string testOutputFile, FileUnitTestHandler handler)
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
				string prefix = Environment.GetEnvironmentVariable("REVENGETESTDIR");
				Assert.IsNotNull(prefix, "Must define the environment variable REVENGETESTDIR");
				Assert.IsTrue(prefix.Length > 0, "Must define the environment variable REVENGETESTDIR");
				return prefix;
			}
		}
		
		public TextWriter TextWriter
		{
			get { return stm; }
		}
		
		public void CloseTestStream()
		{
			if (stm != null)
			{
				stm.Flush();
				stm.Close();
				stm = null;
			}
		}

		[System.Diagnostics.DebuggerHidden]
		public void CompareFiles(StreamReader expected, StreamReader test)
		{
			int line = 1;
			for (;;)
			{
				string tstLine =  test.ReadLine();
				string expLine =  expected.ReadLine();

				if (tstLine == null && expLine == null)
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


		[System.Diagnostics.DebuggerHidden]
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
