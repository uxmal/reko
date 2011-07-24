#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using NUnit.Framework;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class ProcessorState
	{
		[Test]
		public void Simple()
		{
			X86State st = new X86State();
			st.SetRegister(Registers.cs, new Constant(PrimitiveType.Word16, 0xC00));
			st.SetRegister(Registers.ax, new Constant(PrimitiveType.Word16, 0x1234));
			Assert.IsTrue(!st.GetRegister(Registers.bx).IsValid);
			Assert.IsTrue(st.GetRegister(Registers.ax).IsValid);
			Assert.IsTrue(st.GetRegister(Registers.al).IsValid);
			Assert.AreEqual(0x34, st.GetRegister(Registers.al).ToUInt32());
			Assert.IsTrue(st.GetRegister(Registers.ah).IsValid);
			Assert.AreEqual(0x12, st.GetRegister(Registers.ah).ToUInt32());
		}

        [Test]
        public void AreEqual()
        {
            X86State st1 = new X86State();
            X86State st2 = new X86State();
            Assert.IsTrue(st1.HasSameValues(st2));
        }
	}
}
