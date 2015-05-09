#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Core.Output;
using NUnit.Framework;
using Rhino.Mocks;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Core.Output
{
    [TestFixture]
    public class GlobalDataWriterTests
    {
        private LoadedImage image;
        private Program prog;
        private ServiceContainer sc;

        private ImageWriter Memory(uint address)
        {
            image = new LoadedImage(Address.Ptr32(address), new byte[1024]);
            var mem = new LeImageWriter(image.Bytes);
            return mem;
        }

        private void Globals(params StructureField[] fields)
        {
            var arch = new Mocks.FakeArchitecture();
            this.prog = new Program(
                image,
                image.CreateImageMap(),
                arch,
                new DefaultPlatform(null, arch));
            var globalStruct = new StructureType();
            globalStruct.Fields.AddRange(fields);
            prog.Globals.TypeVariable = new TypeVariable("globals_t", 1) { DataType = globalStruct };
            var eq = new EquivalenceClass(prog.Globals.TypeVariable);
            eq.DataType = globalStruct;
            var ptr = new Pointer(eq, 4);
            prog.Globals.TypeVariable.DataType = ptr;
        }

        private void RunTest(string sExp)
        {
            var sw = new StringWriter();
            var gdw = new GlobalDataWriter(prog, sc);
            gdw.WriteGlobals(new TextFormatter(sw)
            {
                Indentation = 0,
                UseTabs = false,
            });
            Assert.AreEqual(sExp, sw.ToString());
        }

        private StructureField Field(int offset, DataType dt)
        {
            return new StructureField(offset, dt);
        }

        [SetUp]
        public void SEtup()
        {
            this.sc = new ServiceContainer();
        }

        [Test]
        public void GdwInt32()
        {
            Memory(0x1000)
                .WriteLeUInt32(0xFFFFFFFF);
            Globals(
                new StructureField(0x1000, PrimitiveType.Int32));

            RunTest("int32 g_dw1000 = -1;\r\n");
        }

        [Test]
        public void GdwReal32()
        {
            Memory(0x1000)
                .WriteLeUInt32(0x3F800000); // 1.0F
            Globals(
                Field(0x1000, PrimitiveType.Real32));

            RunTest("real32 g_r1000 = 1F;\r\n");
        }

        [Test]
        public void GdwTwoFields()
        {
            Memory(0x1000)
                .WriteLeUInt32(0xC0800000)  // -4.0F
                .WriteLeUInt32(0x48);       // 'H'
            Globals(
                Field(0x1000, PrimitiveType.Real32),
                Field(0x1004, PrimitiveType.Char));

            RunTest(
@"real32 g_r1000 = -4F;
char g_b1004 = 'H';
");
        }

        [Test]
        public void GdwFixedArray()
        {
            Memory(0x1000)
                .WriteLeUInt32(1)
                .WriteLeUInt32(10)
                .WriteLeUInt32(100);
            Globals(
                Field(0x1000, new ArrayType(PrimitiveType.UInt32, 3)));
            RunTest(
@"uint32 g_a1000[3] = 
{
    0x00000001,
    0x0000000A,
    0x00000064,
};
");
        }

        [Test]
        public void GdwPointer()
        {
            Memory(0x1000)
                .WriteLeUInt32(0x1008)
                .WriteLeUInt32(0)
                .WriteLeUInt32(1234);
            Globals(
                Field(0x1000, new Pointer(PrimitiveType.Int32, 4)),
                Field(0x1008, PrimitiveType.Int32));

            RunTest(
@"int32* g_ptr1000 = &g_dw1008;
int32 g_dw1008 = 1234;
");
        }

        [Test]
        public void GdwStructure()
        {
            Memory(0x1000)
                .WriteLeUInt16(4)
                .WriteLeUInt16(unchecked((ushort) -104));
            var eqStr = new EquivalenceClass(new TypeVariable(2));
            var str = new StructureType
            {
                Name = "Eq_2",
                Fields = {
                    { 0, PrimitiveType.Int16 },
                    { 2, PrimitiveType.Int16 },
                }
            };
            eqStr.DataType = str;
            Globals(
                Field(0x1000, eqStr));
            RunTest(
@"Eq_2 g_t1000 = 
{
    4,
    -104,
};
");
        }

        [Test]
        public void GdwVisitLinkedList()
        {
            Memory(0x1000)
                .WriteLeUInt32(1)
                .WriteLeUInt32(0x1008)
                .WriteLeUInt32(2)
                .WriteLeUInt32(0x0000)
                .WriteLeUInt32(0x1000);
            var eqLink = new EquivalenceClass(new TypeVariable(2));
            var link = new StructureType
            {
                Name = "Eq_2",
                Fields = {
                    { 0, PrimitiveType.Int32 },
                    { 4, new Pointer(eqLink, 4) }
                }
            };
            eqLink.DataType = link;
            Globals(
                Field(0x1000, eqLink),
                Field(0x1008, eqLink),
                Field(0x1010, new Pointer(eqLink, 4)));
            RunTest(
@"Eq_2 g_t1000 = 
{
    1,
    &g_t1008,
};
Eq_2 g_t1008 = 
{
    2,
    null,
};
Eq_2* g_ptr1010 = &g_t1000;
");
        }

        [Test]
        public void GdwNullTerminatedString()
        {
            Memory(0x1000)
                .WriteString("Hello, world!", Encoding.UTF8)
                .WriteByte(0);
            Globals(
                Field(0x1000, StringType.NullTerminated(PrimitiveType.Char)));
            RunTest(
@"char g_str1000[] = ""Hello, world!"";
");
        }

        [Test]
        public void GdwArrayStrings()
        {
            Memory(0x1000)
                .WriteString("Low", Encoding.UTF8)
                .WriteBytes(0, 5)
                .WriteString("High", Encoding.UTF8)
                .WriteBytes(0, 4)
                .WriteString("Medium", Encoding.UTF8)
                .WriteBytes(0, 2);
            Globals(
                Field(0x1000, new ArrayType(
                    new StringType(
                        PrimitiveType.Char,
                        null,
                        0) { Length = 8 },
                    3)));
            RunTest(
@"char g_a1000[3][8] = 
{
    ""Low"",
    ""High"",
    ""Medium"",
};
");
        }
    }
}