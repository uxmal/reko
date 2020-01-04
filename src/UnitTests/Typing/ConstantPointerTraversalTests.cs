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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using System;

namespace Reko.UnitTests.Typing
{
    [TestFixture]
    public class ConstantPointerTraversalTests
    {
        private MemoryArea mem;
        private StructureType globalStruct;
        private EquivalenceClass eqLink;
        private Mock<IProcessorArchitecture> arch;
        private SegmentMap imageMap;

        [SetUp]
        public void Setup()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.CreateImageReader(It.IsAny<MemoryArea>(), It.IsAny<ulong>()))
                .Returns((MemoryArea i, ulong o) => i.CreateLeReader(o));
            globalStruct = new StructureType
            {
            };
            var globals_t = new TypeVariable("globals_t", 1) { DataType = globalStruct };
            var globals = new Identifier("globals", PrimitiveType.Ptr32, null);

            eqLink = new EquivalenceClass(new TypeVariable(2));
            StructureType str = new StructureType
            {
                Fields = {
                    { 0, new Pointer(eqLink, 32)},
                    { 4, PrimitiveType.Int32 }
                }
            };
            eqLink.DataType = str;
        }

        private ImageWriter Memory(uint address)
        {
            mem = new MemoryArea(Address.Ptr32(address), new byte[1024]);
            imageMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(".data", mem, AccessMode.ReadWrite));
            var writer = new LeImageWriter(mem.Bytes);
            return writer;
        }

        private void Root(uint address, DataType dt)
        {
            globalStruct.Fields.Add((int) address, dt);
        }

        [Test]
        public void CptPointerChain()
        {
            Memory(0x10000000)
                .WriteLeUInt32(0x10000008)
                .WriteLeUInt32(0x00000001)
                .WriteLeUInt32(0x10000010)
                .WriteLeUInt32(0x00000002)
                .WriteLeUInt32(0)
                .WriteLeUInt32(0);

            Root(0x10000000, eqLink);

            var cpt = new ConstantPointerTraversal(arch.Object, globalStruct, imageMap);
            cpt.Traverse();
            Assert.AreEqual(2, cpt.Discoveries.Count);
            Assert.AreEqual("10000008: t10000008: Eq_2", cpt.Discoveries[0].ToString());
            Assert.AreEqual("10000010: t10000010: Eq_2", cpt.Discoveries[1].ToString());
        }

        [Test]
        public void CptArrayOfPointers()
        {
            Memory(0x1000)
                .WriteLeUInt32(0x01010)
                .WriteLeUInt32(0x01014)
                .WriteLeUInt32(0x01018)
                .WriteLeUInt32(0x0101C)
                .WriteLeUInt32('a')
                .WriteLeUInt32('b')
                .WriteLeUInt32('c')
                .WriteLeUInt32('d');
            Root(0x01000, new ArrayType(new Pointer(PrimitiveType.Char, 32), 4));
            var cpt = new ConstantPointerTraversal(arch.Object, globalStruct, imageMap);
            cpt.Traverse();

            Assert.AreEqual(4, cpt.Discoveries.Count);

            Assert.AreEqual("1010: b1010: char", cpt.Discoveries[0].ToString());
            Assert.AreEqual("1014: b1014: char", cpt.Discoveries[1].ToString());
            Assert.AreEqual("1018: b1018: char", cpt.Discoveries[2].ToString());
            Assert.AreEqual("101C: b101C: char", cpt.Discoveries[3].ToString());
        }

        [Test]
        public void CptCycle()
        {
            Memory(0x1000)
                .WriteLeUInt32(0x1000)
                .WriteLeUInt32(42);
            Root(0x1000, eqLink);
            var cpt = new ConstantPointerTraversal(arch.Object, globalStruct, imageMap);
            cpt.Traverse();

            Assert.AreEqual(0, cpt.Discoveries.Count);
        }

        [Test]
        public void CptTree()
        {
            var eqTreeNode = new EquivalenceClass(new TypeVariable(2));
            var str = new StructureType
            {
                Fields = {
                    { 0, PrimitiveType.UInt32 },
                    { 4, new Pointer(eqTreeNode, 32) },  // Left
                    { 8, new Pointer(eqTreeNode, 32) },  // Right
                }
            };
            eqTreeNode.DataType = str;

            Memory(0x10000)
                .WriteLeUInt32(0)               // 00: Padding
                .WriteLeUInt32(0)               // 04: Padding
                .WriteLeUInt32(0x746F6F52)      // 08: Ascii 'Root'
                .WriteLeUInt32(0x00010014)      // 0C: Ptr to Left node
                .WriteLeUInt32(0x00010020)      // 10: Right node
                .WriteLeUInt32(0x7466654C)      // 14: Ascii 'Left'
                .WriteLeUInt32(0x00000000)      // 18: no left node
                .WriteLeUInt32(0x00000000)      // 1C: no right node
                .WriteLeUInt32(0x65746952)      // 20: Ascii 'Rite'
                .WriteLeUInt32(0x00000000)      // 24: no left node
                .WriteLeUInt32(0x00000000);     // 28: no right node
            Root(0x10008, eqTreeNode);
            var cpt = new ConstantPointerTraversal(arch.Object, globalStruct, imageMap);
            cpt.Traverse();

            Assert.AreEqual(2, cpt.Discoveries.Count);
            Assert.AreEqual("10014: t10014: Eq_2", cpt.Discoveries[0].ToString());
            Assert.AreEqual("10020: t10020: Eq_2", cpt.Discoveries[1].ToString());
            Assert.AreEqual("10020: t10020: Eq_2", cpt.Discoveries[1].ToString());
        }
    }
}
