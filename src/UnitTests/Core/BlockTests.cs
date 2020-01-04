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

using System;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class BlockTests
	{
		[Test]
		public void CreateStatements()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/BlockCreateStatements.txt"))
			{
				Block bl = new Block(null, "block0");
				bl.Statements.Add(0,
					new Assignment(
					new Identifier("id3", PrimitiveType.Word16, null),
					new Identifier("id4", PrimitiveType.Word16, null)));
				bl.Statements.Add(4,
					new Assignment(
					new Identifier("id4", PrimitiveType.Word16, null),
					Constant.Word16(3)));

				bl.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
