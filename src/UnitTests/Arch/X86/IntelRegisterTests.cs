#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.X86
{
	[TestFixture]
	public class IntelRegisterTests
	{
		private IntelArchitecture arch;
        private readonly BitRange lowByte;
        private readonly BitRange highByte;
        private readonly BitRange word;
        private readonly BitRange dword;

        public IntelRegisterTests()
		{
			arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string, object>());
            lowByte = new BitRange(0, 8);
            highByte = new BitRange(8, 16);
            word = new BitRange(0, 16);
            dword = new BitRange(0, 32);
        }

        [Test]
		public void X86r_GetRegisterOfAx()
		{
			Assert.AreSame(Registers.al, arch.GetRegister(Registers.ax.Domain, lowByte));
		}

		[Test]
		public void X86r_GetRegisterOfEsi()
		{
			Assert.AreSame(Registers.esi, arch.GetRegister(Registers.esi.Domain, dword));
		}

		[Test]
		public void X86r_GetRegisterOfEdx()
		{
			Assert.AreSame(Registers.edx, arch.GetRegister(Registers.edx.Domain, dword));
		}

		[Test]
		public void X86r_GetRegisterOfAh()
		{
			Assert.AreSame(Registers.ah, arch.GetRegister(Registers.ah.Domain, highByte));
		}

		[Test]
		public void X86r_GetRegisterOfEax()
		{
			Assert.AreSame(Registers.ah, arch.GetRegister(Registers.eax.Domain, highByte));
		}

		[Test]
		public void X86r_GetPartEsi()
		{
            Assert.AreSame(Registers.si, arch.GetRegister(Registers.esi.Domain, word));
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
            Assert.AreEqual(Registers.al, arch.GetRegister(Registers.eax.Domain, lowByte));
            Assert.AreEqual(Registers.bx, arch.GetRegister(Registers.ebx.Domain, word));
            Assert.AreEqual(Registers.ecx, arch.GetRegister(Registers.ecx.Domain, dword));
            Assert.AreEqual(Registers.sil, arch.GetRegister(Registers.esi.Domain, lowByte));
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
