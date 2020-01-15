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

using NUnit.Framework;
using Reko.Core;
using Reko.ImageLoaders.MzExe.CodeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CvStructure = Reko.ImageLoaders.MzExe.CodeView.Structure;

namespace Reko.UnitTests.ImageLoaders.MzExe.CodeView
{
    [TestFixture]
    public class CodeViewTypeLoaderTests
    {
        private byte[] typeSection;

        [SetUp]
        public void Setup()
        {
            typeSection = new byte[0];
        }

        private void Given_TypeSection(string hexbytes)
        {
            this.typeSection = BytePattern.FromHexBytes(hexbytes).ToArray();
        }

        private object ReadLeaf()
        {
            return CodeViewTypeLoader.ReadLeaf(new LeImageReader(typeSection));
        }

        [Test]
        public void Cvtl_ReadStructLeaf()
        {
            Given_TypeSection("79 40 05 83 0F02 83 1002 82 065F696F627566 69");
            var str = (CvStructure) ReadLeaf();

            Assert.AreEqual("Structure _iobuf: bitsize 64 fields: 5 types: 020F names: 0210", str.ToString());
        }

        [Test]
        public void Cvtl_ReadList()
        {
            Given_TypeSection(
@"7F
    82 04 5F 70-74 72 
    88 00 
    82 04 5F 63 6E 74 
    88 02
    82 05 5F 62 61 73 65 
    88 04 
    82 05 5F 66 6C 61 67
    88 06
    82 05 5F 66 69 6C 65
    88 07");
            var list = (object[]) ReadLeaf();
            Assert.AreEqual("_ptr,0,_cnt,2,_base,4,_flag,6,_file,7", string.Join(",", list));
        }

        [Test]
        public void Cvtl_ReadPointer()
        {
            Given_TypeSection("7A 74 83 0E02 82 06 5F696F627566");
            var x = ReadLeaf();
            Assert.AreEqual("Pointer _iobuf NEAR type: 020E", x.ToString());
        }

        [Test]
        public void Cvtl_ReadArray()
        {
            Given_TypeSection("78 85 C800 83 8000");
            var x = ReadLeaf();
            Assert.AreEqual("Array  BitSize: 200 Elem:128 Idx:-1", x.ToString());
        }

        [Test]
        public void Cvtl_ReadProcedure()
        {
            Given_TypeSection("75 80 81 63 02 83 C1 03");
            var proc = ReadLeaf();
            Assert.AreEqual("Procedure C_NEAR return:129 (2: #03C1)", proc.ToString());
        }

        [Test]
        public void Cvtl_ReadLabel()
        {
            Given_TypeSection("72 80 74");
            var proc = ReadLeaf();
            Assert.AreEqual("Label NEAR", proc.ToString());
        }
    }
}
