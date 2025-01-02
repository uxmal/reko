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

using NUnit.Framework;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Types
{
    [TestFixture]
    public class StructureFieldCollectionTests
    {
        [Test]
        public void Sfc_Add()
        {
            var sfc = new StructureFieldCollection();
            sfc.Add(4, PrimitiveType.Word32);
            Assert.AreEqual(1, sfc.Count);
        }

        [Test]
        public void Sfc_AddDuplicate_SameType()
        {
            var sfc = new StructureFieldCollection();
            var f1 = sfc.Add(4, PrimitiveType.Word32);
            var f2 = sfc.Add(4, PrimitiveType.Word32);
            Assert.AreEqual(1, sfc.Count);
            Assert.AreSame(f1, f2);
        }

        [Test]
        public void Sfc_AddDuplicate_DifferentTypes()
        {
            var sfc = new StructureFieldCollection();
            var f1 = sfc.Add(4, PrimitiveType.Word32);
            var f2 = sfc.Add(4, PrimitiveType.Real32);
            Assert.AreEqual(2, sfc.Count);
            Assert.AreSame(PrimitiveType.Real32, f2.DataType);
        }

        [Test]
        public void Sfc_Add_DecreasingOffset()
        {
            var sfc = new StructureFieldCollection();
            var f1 = sfc.Add(10, PrimitiveType.Word32);
            var f2 = sfc.Add(2, PrimitiveType.Real32);
            Assert.AreEqual(2, sfc.Count);
            Assert.AreSame(PrimitiveType.Real32, sfc.AtOffset(2).DataType);
            Assert.AreSame(PrimitiveType.Word32, sfc.AtOffset(10).DataType);
        }

        [Test]
        public void Sfc_Add_ThreeFields()
        {
            var str = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Byte  },
                    { 2, PrimitiveType.Word16 },
                    { 8, PrimitiveType.Word32 }
                }
            };
            Assert.AreEqual(PrimitiveType.Byte, str.Fields.AtOffset(0).DataType);
            Assert.AreEqual(PrimitiveType.Word16, str.Fields.AtOffset(2).DataType);
            Assert.AreEqual(PrimitiveType.Word32, str.Fields.AtOffset(8).DataType);
        }
    }
}
