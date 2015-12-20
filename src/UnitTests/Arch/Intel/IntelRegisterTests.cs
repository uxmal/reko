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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.Arch.X86;
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class IntelRegisterTests
	{
		private IntelArchitecture arch;

		public IntelRegisterTests()
		{
			arch = new IntelArchitecture(ProcessorMode.Protected32);
		}

		[Test]
		public void X86r_GetSubregisterOfAx()
		{
			Assert.AreSame(Registers.al, arch.GetSubregister(Registers.ax, 0, 8));
		}

		[Test]
		public void X86r_GetSubregisterOfEsi()
		{
			Assert.AreSame(Registers.esi, arch.GetSubregister(Registers.esi, 0, 32));
		}

		[Test]
		public void X86r_GetSubregisterOfEdx()
		{
			Assert.AreSame(Registers.edx, arch.GetSubregister(Registers.edx, 0, 32));
		}

		[Test]
		public void X86r_GetSubregisterOfAh()
		{
			Assert.AreSame(Registers.ah, arch.GetSubregister(Registers.ah, 8, 8));
		}

		[Test]
		public void X86r_GetSubregisterOfEax()
		{
			Assert.AreSame(Registers.ah, arch.GetSubregister(Registers.eax, 8, 8));
		}

		[Test]
		public void X86r_GetPartEsi()
		{
            Assert.AreSame(Registers.si, arch.GetSubregister(Registers.esi, 0, 16));
		}

		[Test]
		public void X86r_BitOffset32Acc()
		{
			Assert.AreEqual(0, Registers.eax.BitAddress);
		}

		[Test]
		public void X86r_BitOffsetHiByte()
		{
			Assert.AreEqual(8, Registers.ch.BitAddress);
		}

		[Test]
		public void X86r_IsAluRegister()
		{
			Assert.IsTrue(Registers.gs.IsAluRegister);
			Assert.IsFalse(Registers.Z.IsAluRegister);
		}

		[Test]
		public void X86r_RegisterPartsByteCount()
		{
            Assert.AreEqual(Registers.al, arch.GetSubregister(Registers.eax, 0, 8));
            Assert.AreEqual(Registers.bx, arch.GetSubregister(Registers.ebx, 0, 16));
            Assert.AreEqual(Registers.ecx, arch.GetSubregister(Registers.ecx, 0, 32));
            Assert.AreEqual(null, arch.GetSubregister(Registers.esi, 0, 8));
		}

		[Test]
		public void X86r_WidestSubregisterEcx()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(Registers.ecx.GetWidestSubregister(bits));
            bits.Add(Registers.cl);
			Assert.AreSame(Registers.cl, Registers.ecx.GetWidestSubregister(bits));
            bits.Clear();
			bits.Add(Registers.ch);
			Assert.AreSame(Registers.ch, Registers.ecx.GetWidestSubregister(bits));
            bits.Add(Registers.cx);
			Assert.AreSame(Registers.cx, Registers.ecx.GetWidestSubregister(bits));
			bits.Add(Registers.ecx);
			Assert.AreSame(Registers.ecx, Registers.ecx.GetWidestSubregister(bits));
		}

		[Test]
		public void X86r_WidestSubregisterChClTogether()
		{
            var bits = new HashSet<RegisterStorage>();
            bits.Add(Registers.cl);
            bits.Add(Registers.ch);
			Assert.AreSame(Registers.cx, Registers.ecx.GetWidestSubregister(bits));

		}

		[Test]
		public void X86r_WidestSubregisterEsi()
		{
            var bits = new HashSet<RegisterStorage>();
            Assert.IsNull(Registers.esi.GetWidestSubregister(bits));
			bits.Add(Registers.si);
			Assert.AreSame(Registers.si, Registers.esi.GetWidestSubregister(bits));
			bits.Add(Registers.esi);
			Assert.AreSame(Registers.esi, Registers.esi.GetWidestSubregister(bits));
		}

		[Test]
		public void X86r_WidestSubregisterDx()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(Registers.dx.GetWidestSubregister(bits));
            bits.Add(Registers.dl);
			Assert.AreSame(Registers.dl, Registers.dx.GetWidestSubregister(bits));
            bits.Clear();
			bits.Add(Registers.dh);
			Assert.AreSame(Registers.dh, Registers.dx.GetWidestSubregister(bits));
			bits.Add(Registers.dx);
			Assert.AreSame(Registers.dx, Registers.dx.GetWidestSubregister(bits));
			bits.Add(Registers.edx);
			Assert.AreSame(Registers.dx, Registers.dx.GetWidestSubregister(bits));
		}

		[Test]
		public void X86r_WidestSubregisterSp()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(Registers.sp.GetWidestSubregister(bits));
			bits.Add(Registers.sp);
			Assert.AreSame(Registers.sp, Registers.sp.GetWidestSubregister(bits));
			bits.Add(Registers.esp);
			Assert.AreSame(Registers.sp, Registers.sp.GetWidestSubregister(bits));
		}

		[Test]
		public void X86r_WidestSubregisterBh()
		{
            var bits = new HashSet<RegisterStorage>();
            Assert.IsNull(Registers.bh.GetWidestSubregister(bits));
			bits.Add(Registers.bh);
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
			bits.Add(Registers.bx);
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
			bits.Add(Registers.ebx);
			Assert.AreSame(Registers.bh, Registers.bh.GetWidestSubregister(bits));
		}

		[Test]
		public void X86r_SetAxAliasesTrue()
		{
            var bits = new HashSet<RegisterStorage>();
            var aliases = arch.GetAliases(Registers.ax);
			Assert.IsTrue(bits.Contains(Registers.ax), "Expected ax set");
			Assert.IsTrue(bits.Contains(Registers.ah), "Expected ah set");
			Assert.IsTrue(bits.Contains(Registers.al), "Expected al set");
		}

		[Test]
		public void X86r_SetAhRegisterFileValue()
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
		public void X86r_SetAhThenAl()
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
		public void X86r_SetBp()
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
		public void X86r_SetCx()
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
		public void X86r_SetEsi()
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
		public void X86r_SetEdx()
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

        [Test]
        public void X86reg_SetCxSymbolic_Invalid()
        {
            var ctx = new Dictionary<Storage, Expression> {
                { Registers.cl, Constant.Byte(0) }
            };
            Registers.cx.SetRegisterStateValues(Constant.Invalid, false, ctx);
            Assert.IsTrue(ctx[Registers.cx] == Constant.Invalid);
            Assert.IsTrue(ctx[Registers.cl] == Constant.Invalid);
            Assert.IsTrue(ctx[Registers.ch] == Constant.Invalid);
            Assert.IsTrue(ctx[Registers.ecx] == Constant.Invalid);
        }


        [Test]
        public void X86reg_SetDhSymbolic_Invalid()
        {
            var ctx = new Dictionary<Storage, Expression> {
                { Registers.dl, Constant.Byte(3) }
            };
            Registers.dh.SetRegisterStateValues(Constant.Invalid, false, ctx);
            Assert.IsTrue(ctx[Registers.dh] == Constant.Invalid);
            Assert.IsTrue(ctx[Registers.dx] == Constant.Invalid);
            Assert.IsTrue(ctx[Registers.edx] == Constant.Invalid);
            Assert.AreEqual("0x03", ctx[Registers.dl].ToString());
        }

        [Test]
        public void X86reg_GetSliceAh()
        {
            var c = Constant.Int32(0x1234);
            var ah = Registers.ah;
            var s = ah.GetSlice(c);
            Assert.AreEqual(0x12, ((Constant) s).ToInt16());
        }
	}
}
