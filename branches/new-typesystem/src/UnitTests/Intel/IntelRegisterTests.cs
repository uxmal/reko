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

using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;
using System.Text;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class IntelRegisterTests
	{
		private IntelArchitecture arch;

		public IntelRegisterTests()
		{
			arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
		}

		[Test]
		public void GetSubregisterOfAx()
		{
			Assert.AreSame(Registers.al, Registers.ax.GetSubregister(0, 8));
		}

		[Test]
		public void GetSubregisterOfEsi()
		{
			Assert.AreSame(Registers.esi, Registers.esi.GetSubregister(0, 32));
		}

		[Test]
		public void GetSubregisterOfEdx()
		{
			Assert.AreSame(Registers.edx, Registers.edx.GetSubregister(0, 32));
		}

		[Test]
		public void GetSubregisterOfAh()
		{
			Assert.AreSame(Registers.ah, Registers.ah.GetSubregister(8, 8));
		}

		[Test]
		public void GetSubregisterOfEax()
		{
			Assert.AreSame(Registers.ah, Registers.eax.GetSubregister(8, 8));
		}

		[Test]
		public void GetPartEsi()
		{
			Assert.AreSame(Registers.si, Registers.esi.GetPart(PrimitiveType.Word16));
		}

		[Test]
		public void AliasOffset32Acc()
		{
			Assert.AreEqual(0, Registers.eax.AliasOffset);
		}

		[Test]
		public void AliasOffsetHiByte()
		{
			Assert.AreEqual(8, Registers.ch.AliasOffset);
		}

		[Test]
		public void IsAluRegister()
		{
			Assert.IsTrue(Registers.gs.IsAluRegister);
			Assert.IsFalse(Registers.Z.IsAluRegister);
		}

		[Test]
		public void RegisterPartsByteCount()
		{
			Assert.AreEqual(Registers.al, Registers.eax.GetPart(PrimitiveType.Byte));
			Assert.AreEqual(Registers.bx, Registers.ebx.GetPart(PrimitiveType.Word16));
			Assert.AreEqual(Registers.ecx, Registers.ecx.GetPart(PrimitiveType.Word32));
			Assert.AreEqual(null, Registers.esi.GetPart(PrimitiveType.Byte));
		}

		[Test]
		public void WidestSubregisterEcx()
		{
			BitSet bits = new BitSet(64);
			Assert.IsNull(Registers.ecx.GetWidestSubregister(bits));
			bits[Registers.cl.Number] = true;
			Assert.AreSame(Registers.cl, Registers.ecx.GetWidestSubregister(bits));
			bits.SetAll(false);
			bits[Registers.ch.Number] = true;
			Assert.AreSame(Registers.ch, Registers.ecx.GetWidestSubregister(bits));
			bits[Registers.cx.Number] = true;
			Assert.AreSame(Registers.cx, Registers.ecx.GetWidestSubregister(bits));
			bits[Registers.ecx.Number] = true;
			Assert.AreSame(Registers.ecx, Registers.ecx.GetWidestSubregister(bits));
		}

		[Test]
		public void WidestSubregisterChClTogether()
		{
			BitSet bits = new BitSet(64);
			bits[Registers.cl.Number] = true;
			bits[Registers.ch.Number] = true;
			Assert.AreSame(Registers.cx, Registers.ecx.GetWidestSubregister(bits));

		}

		[Test]
		public void WidestSubregisterEsi()
		{
			BitSet bits = new BitSet(64);
			Assert.IsNull(Registers.esi.GetWidestSubregister(bits));
			bits[Registers.si.Number] = true;
			Assert.AreSame(Registers.si, Registers.esi.GetWidestSubregister(bits));
			bits[Registers.esi.Number] = true;
			Assert.AreSame(Registers.esi, Registers.esi.GetWidestSubregister(bits));
		}

		[Test]
		public void WidestSubregisterDx()
		{
			BitSet bits = new BitSet(64);
			Assert.IsNull(Registers.dx.GetWidestSubregister(bits));
			bits[Registers.dl.Number] = true;
			Assert.AreSame(Registers.dl, Registers.dx.GetWidestSubregister(bits));
			bits.SetAll(false);
			bits[Registers.dh.Number] = true;
			Assert.AreSame(Registers.dh, Registers.dx.GetWidestSubregister(bits));
			bits[Registers.dx.Number] = true;
			Assert.AreSame(Registers.dx, Registers.dx.GetWidestSubregister(bits));
			bits[Registers.edx.Number] = true;
			Assert.AreSame(Registers.dx, Registers.dx.GetWidestSubregister(bits));
		}

		[Test]
		public void WidestSubregisterSp()
		{
			BitSet bits = new BitSet(64);
			Assert.IsNull(Registers.sp.GetWidestSubregister(bits));
			bits[Registers.sp.Number] = true;
			Assert.AreSame(Registers.sp, Registers.sp.GetWidestSubregister(bits));
			bits[Registers.esp.Number] = true;
			Assert.AreSame(Registers.sp, Registers.sp.GetWidestSubregister(bits));
		}

		[Test]
		public void WidestSubregisterBh()
		{
			BitSet bits = new BitSet(64);
			Assert.IsNull(Registers.bh.GetWidestSubregister(bits));
			bits[Registers.bh.Number] = true;
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
			bits[Registers.bx.Number] = true;
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
			bits[Registers.ebx.Number] = true;
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
		}

		[Test]
		public void SetAxAliasesTrue()
		{
			BitSet bits = arch.CreateRegisterBitset();
			Registers.ax.SetAliases(bits, true);
			Assert.IsTrue(bits[Registers.ax.Number], "Expected ax set");
			Assert.IsTrue(bits[Registers.ah.Number], "Expected ah set");
			Assert.IsTrue(bits[Registers.al.Number], "Expected al set");
		}

		[Test]
		public void SetAhAliasesFalse()
		{
			BitSet bits = arch.CreateRegisterBitset();
			bits.SetAll(true);
			Registers.ah.SetAliases(bits, false);
			Assert.IsFalse(bits[Registers.ax.Number], "Expected ax not set");
			Assert.IsFalse(bits[Registers.ah.Number], "Expected ah not set");
			Assert.IsTrue(bits[Registers.al.Number], "Expected al set");
			Assert.IsTrue(bits[Registers.eax.Number], "Expected eax set");
		}

		[Test]
		public void SetAhRegisterFileValue()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.ah.SetRegisterFileValues(regFile, 0x3A, valid);
			Assert.AreEqual(0x3A00, regFile[Registers.ax.Number]);
			Assert.AreEqual(0x3A, regFile[Registers.ah.Number]);
			Assert.AreEqual(0x3A00, regFile[Registers.eax.Number]);
			Assert.IsFalse(valid[Registers.ax.Number]);
			Assert.IsTrue(valid[Registers.ah.Number]);
		}

		[Test]
		public void SetAhThenAl()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.ah.SetRegisterFileValues(regFile, 0x12, valid);
			Registers.al.SetRegisterFileValues(regFile, 0x34, valid);
			Assert.AreEqual(0x1234, regFile[Registers.ax.Number]);
			Assert.AreEqual(0x12, regFile[Registers.ah.Number]);
			Assert.IsTrue(valid[Registers.ax.Number]);
			Assert.IsTrue(valid[Registers.al.Number]);
		}

		[Test]
		public void SetBp()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.bp.SetRegisterFileValues(regFile, 0x1234, valid);
			Assert.AreEqual(0x1234, regFile[Registers.bp.Number]);
			Assert.AreEqual(0x1234, regFile[Registers.ebp.Number]);
			Assert.IsFalse(valid[Registers.ebp.Number]);
			Assert.IsTrue(valid[Registers.bp.Number]);
		}

		[Test]
		public void SetCx()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.cx.SetRegisterFileValues(regFile, 0x1234, valid);
			Assert.AreEqual(0x1234, regFile[Registers.cx.Number]);
			Assert.AreEqual(0x34, regFile[Registers.cl.Number]);
			Assert.AreEqual(0x12, regFile[Registers.ch.Number]);
			Assert.IsTrue(valid[Registers.cx.Number]);
			Assert.IsTrue(valid[Registers.cl.Number]);
			Assert.IsTrue(valid[Registers.ch.Number]);
		}

		[Test]
		public void SetEsi()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.esi.SetRegisterFileValues(regFile, 0x12345678, valid);
			Assert.AreEqual(0x12345678, regFile[Registers.esi.Number]);
			Assert.AreEqual(0x5678, regFile[Registers.si.Number]);
			Assert.IsTrue(valid[Registers.esi.Number]);
			Assert.IsTrue(valid[Registers.si.Number]);
		}

		[Test]
		public void SetEdx()
		{
			ulong [] regFile = new ulong[32];
			bool [] valid = new bool[32];
			Registers.edx.SetRegisterFileValues(regFile, 0x12345678, valid);
			Assert.AreEqual(0x12345678, regFile[Registers.edx.Number]);
			Assert.AreEqual(0x5678, regFile[Registers.dx.Number]);
			Assert.AreEqual(0x78, regFile[Registers.dl.Number]);
			Assert.AreEqual(0x56, regFile[Registers.dh.Number]);
			Assert.IsTrue(valid[Registers.edx.Number]);
			Assert.IsTrue(valid[Registers.dx.Number]);
			Assert.IsTrue(valid[Registers.dl.Number]);
			Assert.IsTrue(valid[Registers.dh.Number]);
		}

	}
}
