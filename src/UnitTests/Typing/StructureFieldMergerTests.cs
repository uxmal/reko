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

using Reko.Core.Types;
using Reko.Typing;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Typing
{
    [TestFixture]
    public class StructureFieldMergerTests
    {
        private TypeFactory factory;
        private StructureFieldMerger sfm;

        [SetUp]
        public void Setup()
        {
            factory = new TypeFactory();
            sfm = new StructureFieldMerger();
        }
        
        [Test]
        public void StrFldMerger_EmptyStruct()
        {
            StructureType str = new StructureType("foo", 0);
            DataType dt = sfm.Merge(str);
            Assert.AreEqual("(struct \"foo\")", dt.ToString());
        }

        [Test]
        public void StrFldMerger_SingleMember()
        {
            StructureType str = new StructureType("foo", 0);
            str.Fields.Add(4, PrimitiveType.Word16);
            DataType dt = sfm.Merge(str);
            Assert.AreEqual("(struct \"foo\" (4 word16 w0004))", dt.ToString());
        }

        [Test]
        public void StrFldMerger_TwoDisjointMembers()
        {
            StructureType str = new StructureType("foo", 0);
            str.Fields.Add(4, PrimitiveType.Word16);
            str.Fields.Add(6, PrimitiveType.Word16);
            DataType dt = sfm.Merge(str);
            Assert.AreEqual("(struct \"foo\" (4 word16 w0004) (6 word16 w0006))", dt.ToString());
        }

        [Test]
        public void StrFldMerger_FindOverlappingCluster()
        {
            StructureType str = new StructureType("foo", 0);
            str.Fields.Add(2, PrimitiveType.Word32);
            str.Fields.Add(4, PrimitiveType.Word32);
            foreach (List<StructureField> cluster in sfm.GetOverlappingClusters(str.Fields))
            {
                Assert.AreEqual(2, cluster.Count);
            }
        }

        [Test]
        public void StrFldMerger_FindOverLappingClusterWithEqv()
        {
            StructureType str = new StructureType("foo", 0);
            UnionType u = new UnionType(null, null, PrimitiveType.Ptr32, PrimitiveType.Word16);
            EquivalenceClass eq = Eqv(u);
            str.Fields.Add(2, eq);
            str.Fields.Add(4, PrimitiveType.SegmentSelector);

            IEnumerable<List<StructureField>> eb = sfm.GetOverlappingClusters(str.Fields);
            IEnumerator<List<StructureField>> e = eb.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(3, e.Current.Count);
            List<StructureField> cluster = e.Current;
        }

        [Test]
        public void StrFldMerger_BuildOverlappedStructure()
        {
            List<StructureField> fields = new List<StructureField>();
            fields.Add(new StructureField(0, PrimitiveType.Ptr32));
            fields.Add(new StructureField(0, PrimitiveType.Word16));
            fields.Add(new StructureField(2, PrimitiveType.SegmentSelector));
            
            DataType dt = sfm.BuildOverlappedStructure(fields);
            Assert.AreEqual("(union (ptr32 u0) ((struct (0 word16 w0000) (2 selector pseg0002)) u1))", dt.ToString());
        }

        [Test]
        public void StrFldMerger_Merge()
        {
            StructureType str = new StructureType("foo", 0);
            UnionType u = new UnionType(null, null, PrimitiveType.Ptr32, PrimitiveType.Word16);
            EquivalenceClass eq = Eqv(u);
            str.Fields.Add(2, eq);
            str.Fields.Add(4, PrimitiveType.SegmentSelector);
            StructureType strNew = sfm.Merge(str);
            Assert.AreEqual("(struct \"foo\" (2 (union (ptr32 u0) ((struct (0 word16 w0000) (2 selector pseg0002)) u1)) u0002))", strNew.ToString());

        }
        private EquivalenceClass Eqv(DataType dt)
        {
            return new EquivalenceClass(
                factory.CreateTypeVariable(),
                dt);
        }
    }
}
