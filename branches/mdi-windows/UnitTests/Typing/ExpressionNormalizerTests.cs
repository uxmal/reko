/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class ExpressionNormalizerTests
	{
		private ProcedureMock m;
		private ExpressionNormalizer aen;

		[Test]
		public void SimpleArray()
		{
			Identifier globals = m.Local32("globals");
			Identifier i = m.Local32("idx");
			Expression ea = m.Add(globals, m.Add(m.Shl(i, 4), 0x30000));
			Expression e = m.Load(PrimitiveType.Int32, ea);
			e = e.Accept(aen);
			Assert.AreEqual("(globals + 0x00030000)[idx * 0x00000010]", e.ToString());
		}

		[Test]
		public void Test2()
		{
			Identifier p = m.Local32("p");
			Identifier i = m.Local32("i");
			Expression e = m.Load(PrimitiveType.Int32, 
				m.Add(p, m.Add(m.Muls(i, 8), 4)));
			e = e.Accept(aen);
			Assert.AreEqual("(p + 4)[i * 0x00000008]", e.ToString());
		}

        [Test]
        public void TransformIdentifierPointer()
        {
            Identifier p = m.Local32("p");
            Expression e = m.Load(PrimitiveType.Word16, p);
            e = e.Accept(aen);
            Assert.AreEqual("Mem0[p + 0x00000000:word16]", e.ToString());
        }

        [Test]
        public void TransformSegAccessMemPointer()
        {
            Identifier bx = m.Local16("bx");
            Identifier ds = m.Local16("ds");
            Expression e = m.SegMem(PrimitiveType.Byte, ds, bx);
            e = e.Accept(aen);
            Assert.AreEqual("Mem0[ds:bx + 0x0000:byte]", e.ToString());
        }

        [Test]
        public void SegAccessArray()
        {
            Identifier bx = m.Local16("bx");
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Expression e = m.SegMem(PrimitiveType.Int32, ds, m.Add(m.Mul(bx, 2), 0x42));
            e = e.Accept(aen);
            Assert.AreEqual("SEQ(ds, 0x0042)[bx * 0x0002]", e.ToString());
        }


		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
			aen = new ExpressionNormalizer(PrimitiveType.Pointer32);
		}
	}
}
