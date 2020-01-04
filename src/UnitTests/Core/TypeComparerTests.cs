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
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class TypeComparerTests
    {
        [Test]
        public void CmpPtrIntPtrInt()
        {
            Pointer p1 = new Pointer(PrimitiveType.Int32, 32);
            Pointer p2 = new Pointer(PrimitiveType.Int32, 32);
            DataTypeComparer c = new DataTypeComparer();
            Assert.AreEqual(0, c.Compare(p1, p2));
        }

        [Test]
        public void CmpUnionToUnion()
        {
            //Warning : inner types were selected in such a way as to force UnionAlternativeCollection
            // to place them in provided order.
            DataType inner1 = PrimitiveType.UInt32;
            DataType inner2 = PrimitiveType.UInt16;
            DataType inner3 = PrimitiveType.UInt8;
            UnionType u1 = new UnionType("Union1", inner1, inner3, inner1);
            UnionType u2 = new UnionType("Union1", inner1, inner3, inner2);

            DataTypeComparer c = new DataTypeComparer();
            Assert.AreNotEqual(0, c.Compare(u1, u2));
        }

        [Test]
        public void CmpRecursiveEqualStructs()
        {
            // struct str1{struct *str2} == struct str2{struct *str1}
            StructureType s1 = new StructureType();
            StructureType s2 = new StructureType();
            s1.Fields.Add(0, new Pointer(s2, 32));
            s2.Fields.Add(0, new Pointer(s1, 32));
            DataTypeComparer c = new DataTypeComparer();
            Assert.AreEqual(0, c.Compare(s1, s2));
        }

        [Test]
        public void CmpRecursiveNotEqualStructs()
        {
            // struct str1{struct *str2; int f1} != struct str2{struct *str1; float f2}
            DataType u32 = PrimitiveType.UInt32;
            DataType r32 = PrimitiveType.Real32;
            StructureType s1 = new StructureType();
            StructureType s2 = new StructureType();
            s1.Fields.Add(0, new Pointer(s2, 32));
            s1.Fields.Add(4, u32);
            s2.Fields.Add(0, new Pointer(s1, 32));
            s2.Fields.Add(4, r32);
            DataTypeComparer c = new DataTypeComparer();
            Assert.AreNotEqual(0, c.Compare(s1, s2));
            Assert.AreNotEqual(0, c.Compare(r32, u32));
        }
    }
}
