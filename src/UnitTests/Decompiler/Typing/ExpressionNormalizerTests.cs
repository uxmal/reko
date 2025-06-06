#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Decompiler.Typing
{
    [TestFixture]
    public class ExpressionNormalizerTests
    {
        private ProcedureBuilder m;
        private ExpressionNormalizer aen;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            aen = new ExpressionNormalizer(PrimitiveType.Ptr32);
        }

        [Test]
        public void EnSimpleArray()
        {
            Identifier globals = m.Local32("globals");
            Identifier i = m.Local32("idx");
            Expression ea = m.IAdd(globals, m.IAdd(m.Shl(i, 4), 0x30000));
            Expression e = m.Mem(PrimitiveType.Int32, ea);
            e = e.Accept(aen);
            Assert.AreEqual("(globals + 0x30000<32>)[idx * 0x10<32>]", e.ToString());
        }

        [Test]
        public void EnTest2()
        {
            Identifier p = m.Local32("p");
            Identifier i = m.Local32("i");
            Expression e = m.Mem(PrimitiveType.Int32,
            m.IAdd(p, m.IAddS(m.SMul(i, 8), 4)));
            e = e.Accept(aen);
            Assert.AreEqual("(p + 4<i32>)[i * 8<32>]", e.ToString());
        }

        [Test]
        public void EnIdentifierPointer()
        {
            Identifier p = m.Local32("p");
            Expression e = m.Mem(PrimitiveType.Word16, p);
            e = e.Accept(aen);
            Assert.AreEqual("Mem0[p + 0<32>:word16]", e.ToString());
        }

        [Test]
        public void EnSegAccessMemPointer()
        {
            Identifier bx = m.Local16("bx");
            Identifier ds = m.Local16("ds");
            Expression e = m.SegMem(PrimitiveType.Byte, ds, bx);
            e = e.Accept(aen);
            Assert.AreEqual("Mem0[ds:bx + 0<16>:byte]", e.ToString());
        }

        [Test]
        public void EnSegAccessArray()
        {
            Identifier bx = m.Local16("bx");
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Expression e = m.SegMem(PrimitiveType.Int16, ds, m.IAdd(m.IMul(bx, 2), 0x42));
            Assert.AreEqual("Mem0[ds:bx * 2<16> + 0x42<16>:int16]", e.ToString());
            e = e.Accept(aen);
            Assert.AreEqual("ds:0x42<16>[bx * 2<16>]", e.ToString());
        }

        [Test]
        public void EnSegAccessZeroBasedArray()
        {
            Identifier bx = m.Local16("bx");
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Expression e = m.SegMem(PrimitiveType.Int32, ds, m.Shl(bx, 2));
            Assert.AreEqual("Mem0[ds:bx << 2<8>:int32]", e.ToString());
            e = e.Accept(aen);
            Assert.AreEqual("ds:0<16>[bx * 4<16>]", e.ToString());
        }

        // Seems unlikely, but sometimes important arrays are located at address 0 in memory.
        [Test]
        public void EnAccessZeroBasedArray()
        {
            Identifier a1 = m.Local32("a1");
            Expression e = m.Mem(PrimitiveType.Int32, m.Shl(a1, 2));
            Assert.AreEqual("Mem0[a1 << 2<8>:int32]", e.ToString());
            e = e.Accept(aen);
            Assert.AreEqual("0<32>[a1 * 4<32>]", e.ToString()); //$LIT: 0<p32>?
        }
    }
}
