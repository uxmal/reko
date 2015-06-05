#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

//$REFACTOR: rename this file to StorageVisitorTests
namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class StorageVisitorTests : StorageVisitor<string>
	{
		[Test]
		public void VisitRegister()
		{
			var reg = new RegisterStorage("r0", 0, PrimitiveType.Word16);
			var r = new Identifier(reg.Name, reg.DataType, reg);
			var type = r.Storage.Accept(this);
			Assert.AreEqual("reg", type);
		}

		[Test]
		public void VisitFlagGroup()
		{
			var f = new Identifier("grf", PrimitiveType.Word16, new FlagGroupStorage(0x11, "ZO", PrimitiveType.Byte));
			var type = f.Storage.Accept(this);
			Assert.AreEqual("grf", type);
		}

		[Test]
		public void VisitSequenceVariable()
		{
			var ax = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
			var dx = new Identifier(Registers.dx.Name, Registers.dx.DataType, Registers.dx);
			var seq = new Identifier("dx_ax", PrimitiveType.Word32, new SequenceStorage(dx, ax));
			var type = seq.Storage.Accept(this);
			Assert.AreEqual("seq", type);
		}

		[Test]
		public void VisitFpuStackVariable()
		{
			Identifier f = new Identifier("st(0)", PrimitiveType.Real80, new FpuStackStorage(0, PrimitiveType.Real80));
			var type = f.Storage.Accept(this);
			Assert.AreEqual("fpu", type);
		}

		public string VisitFlagGroupStorage(FlagGroupStorage reg)
		{
			return "grf";
		}

		public string VisitFpuStackStorage(FpuStackStorage fpu)
		{
			return "fpu";
		}

		public string VisitMemoryStorage(MemoryStorage global)
		{
			return "global";
		}

		public string VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			return "org";
		}

		public string VisitRegisterStorage(RegisterStorage reg)
		{
			return "reg";
		}

		public string VisitSequenceStorage(SequenceStorage seq)
		{
			return "seq";
		}

		public string VisitStackArgumentStorage(StackArgumentStorage stack)
		{
			return "stack";
		}

		public string VisitStackLocalStorage(StackLocalStorage local)
		{
			return "local";
		}

		public string VisitTemporaryStorage(TemporaryStorage temp)
		{
			return "temp";
		}
	}
}