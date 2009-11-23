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
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

//$REFACTOR: rename this file to StorageVisitorTests
namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class StorageVisitorTests : StorageVisitor
	{
		private string type;

		[Test]
		public void VisitRegister()
		{
			MachineRegister reg = new MachineRegister("r0", 0, PrimitiveType.Word16);
			Identifier r = new Identifier(reg.Name, 0, reg.DataType, new RegisterStorage(reg));
			r.Storage.Accept(this);
			Assert.AreEqual("reg", type);
		}

		[Test]
		public void VisitFlagGroup()
		{
			Identifier f = new Identifier("grf", 0, PrimitiveType.Word16, new FlagGroupStorage(0x11, "ZO"));
			f.Storage.Accept(this);
			Assert.AreEqual("grf", type);
		}

		[Test]
		public void VisitSequenceVariable()
		{
			Identifier ax = new Identifier(Registers.ax.Name, 0, Registers.ax.DataType, new RegisterStorage(Registers.ax));
			Identifier dx = new Identifier(Registers.dx.Name, 1, Registers.dx.DataType, new RegisterStorage(Registers.dx));
			Identifier seq = new Identifier("dx_ax", 2, PrimitiveType.Word32, new SequenceStorage(dx, ax));
			seq.Storage.Accept(this);
			Assert.AreEqual("seq", type);
		}

		[Test]
		public void VisitFpuStackVariable()
		{
			Identifier f = new Identifier("st(0)", 0, PrimitiveType.Real80, new FpuStackStorage(0, PrimitiveType.Real80));
			f.Storage.Accept(this);
			Assert.AreEqual("fpu", type);
		}

		public void VisitFlagGroupStorage(FlagGroupStorage reg)
		{
			type = "grf";
		}

		public void VisitFpuStackStorage(FpuStackStorage fpu)
		{
			type = "fpu";
		}

		public void VisitMemoryStorage(MemoryStorage global)
		{
			type = "global";
		}

		public void VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			type = "org";
		}

		public void VisitRegisterStorage(RegisterStorage reg)
		{
			type = "reg";
		}

		public void VisitSequenceStorage(SequenceStorage seq)
		{
			type = "seq";
		}

		public void VisitStackArgumentStorage(StackArgumentStorage stack)
		{
			type = "stack";
		}

		public void VisitStackLocalStorage(StackLocalStorage local)
		{
			type = "local";
		}

		public void VisitTemporaryStorage(TemporaryStorage temp)
		{
			type = "temp";
		}
	}
}