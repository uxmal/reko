#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using Moq;

namespace Reko.UnitTests.Decompiler.Evaluation
{
	[TestFixture]
	public class SliceMem_RuleTest
	{
        private Mock<EvaluationContext> ctx;

        [SetUp]
        public void Setup()
        {
            this.ctx = new Mock<EvaluationContext>();
            this.ctx.Setup(c => c.MemoryGranularity).Returns(8);
        }

		[Test]
		public void SliceMem()
		{
			var s = new Slice(PrimitiveType.Byte,
				new MemoryAccess(MemoryIdentifier.GlobalMemory, 
				new Identifier("ptr", PrimitiveType.Word32, null), PrimitiveType.Word32), 16);
			var r = new SliceMem_Rule(ctx.Object);
			Assert.IsTrue(r.Match(s));
			var e = r.Transform();
			Assert.AreEqual("Mem0[ptr + 2<32>:byte]", e.ToString());
		}

		[Test]
		public void SliceMem0()
		{
			var s = new Slice(PrimitiveType.Word16,
				new MemoryAccess(MemoryIdentifier.GlobalMemory,
				new Identifier("ptr", PrimitiveType.Word32, null), PrimitiveType.Word32), 0);
			var r = new SliceMem_Rule(ctx.Object);
			Assert.IsTrue(r.Match(s));
			var e = r.Transform();
			Assert.AreEqual("Mem0[ptr + 0<32>:word16]", e.ToString());
		}
	}
}
