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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class StructureReaderTests
    {
        public struct TestStruct
        {
            public ushort usField;
        }

        [Test]
        public void Sr_ReadLeUInt16_Field()
        {
            var rdr = new LeImageReader(new byte[] { 0x34, 0x12 });
			var test = rdr.ReadStruct<TestStruct>();
			Assert.AreEqual((ushort) 0x1234, test.usField);
        }

        public struct TestStruct2
        {
            public ushort usField;
            public ushort pad02;
            public int lField;
        }

        [Test]
        public void Sr_ReadLeInt32_Field()
        {
            var rdr = new LeImageReader(new byte[] { 0x34, 0x12, 0xAB, 0xCD, 0x78, 0x56, 0x34, 0x12 });
			var test = rdr.ReadStruct<TestStruct2>();
            Assert.AreEqual((int) 0x12345678, test.lField);
        }

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct TestStruct3
        {
            public ushort usField;
            public int lField;
        }

        [Test]
        public void Sr_ReadLeInt32_Padding()
        {
            var rdr = new LeImageReader(new byte[] { 
                0x34, 0x12,
                0xAB, 0xCD, 
                0x78, 0x56, 0x34, 0x12 });
			var test = rdr.ReadStruct<TestStruct3>();
            Assert.AreEqual((int) 0x12345678, test.lField);
        }

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TestStruct4
        {
            public uint usField;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string sField04;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
			public string sFieldnn;
        }

        [Test]
        public void Sr_ReadLeInt32_String()
        {
            var rdr = new ImageReader(new byte[] { 
                0x34, 0x12, 
                0xAB, 0xCD,
                0x48, 0x69, 0x00,
                0x42, 0x79, 0x65, 0x21, 0x00});

            var test = rdr.ReadStruct<TestStruct4>();
            Assert.AreEqual("Hi", test.sField04);
            Assert.AreEqual("Bye!", test.sFieldnn);
        }

#if false
		public class TestStruct5
        {
            public ushort sig;
            [PointerField(Size=4)]
            public Extra extra;

            public class Extra
            {
                public ushort sig;
            }
        }

        [Test]
        public void Sr_ReadStructure()
        {
            var rdr = new LeImageReader(new byte[] {
                0x4A, 0x4B,     // signature
                0x08, 0x00, 0x00, 0x00, // pointer to struct
                0xFF, 0xFF,      // padding
                0x34, 0x12,      // structure.
            });
            var test = new TestStruct5();
            var sr = new StructureReader(test);
            sr.Read(rdr);

            Assert.IsNotNull(test.extra);
            Assert.AreEqual((ushort) 0x1234, test.extra.sig);
        }

        public class TestStruct6
        {
            public short signature;
            [PointerField(Size = 2, Align=2)]
            public Directory directory;

            public class Directory
            {
                [ArrayField(Length = 4, PointerElementSize=2)]
                public Section[] sections;
            }

            public class Section
            {
                [StringField]
                public string name;
            }
        }

        [Test]
        public void Sr_ReadArray()
        {
            var rdr = new LeImageReader(new byte[] {
                0x4A, 0x4B,     // signature
                0x08, 0x00, 0x00, 0x00, // pointer to directory
                0xFF, 0xFF,      // padding
                0x10, 0,      // Directory slot 0
                0x13, 0,      // Directory slot 1
                0x16, 0,      // Directory slot 2
                0x19, 0,        // Directory slot 3
                0x61, 0x62, 0x00,
                0x63, 0x64, 0x00,
                0x65, 0x66, 0x00,
                0x65, 0x78, 0x00,
            });
            var test = new TestStruct6();
            var sr = new StructureReader(test);
            sr.Read(rdr);

            Assert.IsNotNull(test.directory);
            Assert.IsNotNull(test.directory.sections);
            Assert.AreEqual(4, test.directory.sections.Length);
            Assert.AreEqual("ab", test.directory.sections[0].name);
            Assert.AreEqual("cd", test.directory.sections[1].name);
            Assert.AreEqual("ef", test.directory.sections[2].name);
            Assert.AreEqual("ex", test.directory.sections[3].name);
        }
#endif
	}
}
