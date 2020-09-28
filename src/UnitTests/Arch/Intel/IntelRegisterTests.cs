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
			arch = new X86ArchitectureFlat32("x86-protected-32");
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
			Assert.AreSame(Registers.ah, arch.GetSubregister(Registers.ah, 8, 16));
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
		public void X86r_RegisterPartsByteCount()
		{
            Assert.AreEqual(Registers.al, arch.GetSubregister(Registers.eax, 0, 8));
            Assert.AreEqual(Registers.bx, arch.GetSubregister(Registers.ebx, 0, 16));
            Assert.AreEqual(Registers.ecx, arch.GetSubregister(Registers.ecx, 0, 32));
            Assert.AreEqual(Registers.sil, arch.GetSubregister(Registers.esi, 0, 8));
		}

		[Test]
		public void X86r_WidestSubregisterEcx()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(arch.GetWidestSubregister(Registers.ecx, bits));
            bits.Add(Registers.cl);
			Assert.AreSame(Registers.cl, arch.GetWidestSubregister(Registers.ecx, bits));
            bits.Clear();
			bits.Add(Registers.ch);
			Assert.AreSame(Registers.ch, arch.GetWidestSubregister(Registers.ecx, bits));
            bits.Add(Registers.cx);
			Assert.AreSame(Registers.cx, arch.GetWidestSubregister(Registers.ecx, bits));
			bits.Add(Registers.ecx);
			Assert.AreSame(Registers.ecx, arch.GetWidestSubregister(Registers.ecx, bits));
		}

		[Test]
		public void X86r_WidestSubregisterChClTogether()
		{
            var bits = new HashSet<RegisterStorage>();
            bits.Add(Registers.cl);
            bits.Add(Registers.ch);
			Assert.AreSame(Registers.cx, arch.GetWidestSubregister(Registers.ecx, bits));
		}

		[Test]
		public void X86r_WidestSubregisterEsi()
		{
            var bits = new HashSet<RegisterStorage>();
            Assert.IsNull(arch.GetWidestSubregister(Registers.esi, bits));
			bits.Add(Registers.si);
			Assert.AreSame(Registers.si, arch.GetWidestSubregister(Registers.esi, bits));
			bits.Add(Registers.esi);
			Assert.AreSame(Registers.esi, arch.GetWidestSubregister(Registers.esi, bits));
		}

		[Test]
		public void X86r_WidestSubregisterDx()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(arch.GetWidestSubregister(Registers.dx, bits));
            bits.Add(Registers.dl);
			Assert.AreSame(Registers.dl, arch.GetWidestSubregister(Registers.dx, bits));
            bits.Clear();
			bits.Add(Registers.dh);
			Assert.AreSame(Registers.dh, arch.GetWidestSubregister(Registers.dx, bits));
			bits.Add(Registers.dx);
			Assert.AreSame(Registers.dx, arch.GetWidestSubregister(Registers.dx, bits));
			bits.Add(Registers.edx);
			Assert.AreSame(Registers.dx, arch.GetWidestSubregister(Registers.dx, bits));
		}

		[Test]
		public void X86r_WidestSubregisterSp()
		{
            var bits = new HashSet<RegisterStorage>();
			Assert.IsNull(arch.GetWidestSubregister(Registers.sp, bits));
			bits.Add(Registers.sp);
			Assert.AreSame(Registers.sp, arch.GetWidestSubregister(Registers.sp, bits));
			bits.Add(Registers.esp);
			Assert.AreSame(Registers.sp, arch.GetWidestSubregister(Registers.sp, bits));
		}

		[Test]
		public void X86r_WidestSubregisterBh()
		{
            var bits = new HashSet<RegisterStorage>();
            Assert.IsNull(arch.GetWidestSubregister(Registers.bh, bits));
			bits.Add(Registers.bh);
			Assert.AreSame(Registers.bh, arch.GetWidestSubregister(Registers.bh, bits));
			bits.Add(Registers.bx);
 			Assert.AreSame(Registers.bh, arch.GetWidestSubregister(Registers.bh, bits));
			bits.Add(Registers.ebx);
			Assert.AreSame(Registers.bh, arch.GetWidestSubregister(Registers.bh, bits));
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
