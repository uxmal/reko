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

using Decompiler.Analysis.Simplification;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis.Simplification
{
	[TestFixture]
	public class SliceMem_RuleTest
	{
		[Test]
		public void SliceMem()
		{
			Slice s = new Slice(PrimitiveType.Byte,
				new MemoryAccess(MemoryIdentifier.GlobalMemory, 
				new Identifier("ptr", 0, PrimitiveType.Word32, null), PrimitiveType.Word32), 16);
			SliceMem_Rule r = new SliceMem_Rule();
			Assert.IsTrue(r.Match(s));
			Expression e = r.Transform(null);
			Assert.AreEqual("Mem0[ptr + 0x00000002:byte]", e.ToString());
		}

		[Test]
		public void SliceMem0()
		{
			Slice s = new Slice(PrimitiveType.Word16,
				new MemoryAccess(MemoryIdentifier.GlobalMemory,
				new Identifier("ptr", 0, PrimitiveType.Word32, null), PrimitiveType.Word32), 0);
			SliceMem_Rule r = new SliceMem_Rule();
			Assert.IsTrue(r.Match(s));
			Expression e = r.Transform(null);
			Assert.AreEqual("Mem0[ptr + 0x00000000:word16]", e.ToString());
		}

	}
}
