#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class OutStorageTests
	{
		[Test]
		public void CreateOutArgumentRegister()
		{
			RegisterStorage mr = RegisterStorage.Reg32("r1", 1);
			Identifier oarg = new Identifier("r1Out", PrimitiveType.Word32, new OutArgumentStorage(
				new Identifier(mr.Name, PrimitiveType.Word32, mr)));
			StringWriter w = new StringWriter();
			oarg.Write(true, w);
			Assert.AreEqual("Register out word32 r1Out", w.ToString());
		}
	}

	[TestFixture]
	public class StackArgumentTests
	{
		private Frame f;
		private Identifier argOff;
		private Identifier argSeg;
		private Identifier arg_alias;

		[SetUp]
		public void Setup()
		{
            var arch = new Mocks.FakeArchitecture(new ServiceContainer());
			f = new Frame(arch, PrimitiveType.Word16);
			argOff = f.EnsureStackArgument(4, PrimitiveType.Word16);
			argSeg = f.EnsureStackArgument(6, PrimitiveType.SegmentSelector);
			arg_alias = f.EnsureStackArgument(4, PrimitiveType.Ptr32);
		}

		[Test]
		public void OffsetOfStackArgumentOffset()
		{
//			Assert.AreNotSame(argOff, arg_alias);
			Assert.AreEqual(0, arg_alias.Storage.OffsetOf(argOff.Storage));
		}

		[Test]
		public void OffsetOfStackArgumentSegment()
		{
//			Assert.AreNotSame(argOff, arg_alias);
			Assert.AreEqual(16, arg_alias.Storage.OffsetOf(argSeg.Storage));
		}
	}

	[TestFixture]
	public class StackLocalTests
	{
		private Frame f;
		private StackStorage varOff;
		private StackStorage varSeg;
		private StackStorage varPointer;

		[SetUp]
		public void Setup()
		{
			f = new Frame(new Mocks.FakeArchitecture(), PrimitiveType.Word16);
			varOff = (StackStorage) f.EnsureStackLocal(-4, PrimitiveType.Word16).Storage;
			varSeg = (StackStorage) f.EnsureStackLocal(-2, PrimitiveType.SegmentSelector).Storage;
			varPointer = (StackStorage) f.EnsureStackLocal(-4, PrimitiveType.Ptr32).Storage;
		}

		[Test]
		public void OffsetOfLocalVariableOffset()
		{
//			Assert.AreNotSame(varOff, varPointer);
			Assert.AreEqual(0, varPointer.OffsetOf(varOff));
		}

	}
}
