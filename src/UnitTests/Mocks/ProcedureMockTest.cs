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

using Reko.Core;
using Reko.Core.Expressions;
using System;
using NUnit.Framework;

namespace Reko.UnitTests.Mocks
{
	[TestFixture]
	public class MockProcedureTest
	{
		[Test]
		public void MockFactorial()
		{
			RunTest(new FactorialMock(), "Mock/MockProcedureTest.txt");
		}

		[Test]
		public void MockStore()
		{
			RunTest(new StoreMock(), "Mock/MockStore.txt");
		}

		[Test]
		public void MockNot()
		{
			RunTest(new NotMock(), "Mock/MockNot.txt");
		}

		private class NotMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				var a = Local32("a");
				Assign(a, Not(a));
				Return();
			}
		}

		[Test]
		public void MockFn()
		{
			RunTest(new FnMock(), "Mock/MockFn.txt");
		}

		private class FnMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				Identifier r1 = Local32("r1");
				Identifier r2 = Local32("r2");
				Assign(r1, Fn("foo"));
				Assign(r1, Fn("bar", r1));
				SideEffect(Fn("baz", r1, r2));
				Return();
			}
		}

		[Test]
		public void MockReturn()
		{
			RunTest(new ReturnMock(), "Mock/MockReturn.txt");
		}

		private class ReturnMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				Identifier r1 = Local32("r1");
				BranchIf(Eq(r1, Int32(0)), "zero");
				Return(Int32(0));
				Label("zero");
				Return(Int32(1));
			}
		}

		private void RunTest(ProcedureBuilder mock, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				mock.Procedure.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private class StoreMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				Identifier r = Local32("r0");

				LoadId(r, Word32(0x20000000));
				MStore(Word32(0x20000000), r);
				Return();
			}
		}
	}
}
