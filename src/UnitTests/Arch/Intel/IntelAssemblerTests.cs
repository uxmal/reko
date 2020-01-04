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

using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class IntelAssemblerTests
	{
		[Test]
		public void IntegralConstant32()
		{
			Constant c;
			c = X86Assembler.IntegralConstant(-2, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.SByte, c.DataType);
			c = X86Assembler.IntegralConstant(-128, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.SByte, c.DataType);
			c = X86Assembler.IntegralConstant(-129, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.Word32, c.DataType);
			c = X86Assembler.IntegralConstant(-129, PrimitiveType.Word16);
			Assert.AreSame(PrimitiveType.Word16, c.DataType);
		}
	}
}
