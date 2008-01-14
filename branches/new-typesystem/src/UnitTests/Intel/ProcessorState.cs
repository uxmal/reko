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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using NUnit.Framework;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class ProcessorState
	{
		[Test]
		public void Simple()
		{
			IntelState st = new IntelState();
			st.Set(Registers.cs, new Value(PrimitiveType.Word16, 0xC00));
			st.Set(Registers.ax, new Value(PrimitiveType.Word16, 0x1234));
			Assert.IsTrue(!st.Get(Registers.bx).IsValid);
			Assert.IsTrue(st.Get(Registers.ax).IsValid);
			Assert.IsTrue(st.Get(Registers.al).IsValid);
			Assert.IsTrue(st.Get(Registers.al).Unsigned == 0x34);
			Assert.IsTrue(st.Get(Registers.ah).IsValid);
			Assert.IsTrue(st.Get(Registers.ah).Unsigned == 0x12);
		}
	}
}
