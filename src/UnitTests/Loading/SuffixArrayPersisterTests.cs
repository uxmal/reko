#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Lib;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class SuffixArrayPersisterTests
    {
        private ServiceContainer sc;
        private MemoryStream stream;
        private Dictionary<MemoryArea, SuffixArray<byte>> sufas;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
        }

        private void Given_MemoryStream()
        {
            this.stream = new MemoryStream();
        }

        private void Given_SuffixArrays(params string [] sMemChunks)
        {
            const int AlignSize = 16;
            const int BaseAddress = 0x010000;

            var aByteChunks = sMemChunks
                .Select(s => HexStringsToByteArray(s))
                .ToList();
            var addr = Address.Ptr32(BaseAddress);
            var mems = new List<MemoryArea>();
            foreach (var ab in aByteChunks)
            {
                mems.Add(new MemoryArea(addr, ab));
                addr = (addr + ab.Length).Align(AlignSize);
            }
            this.sufas = mems
                .ToDictionary(k => k, v => SuffixArray.Create(v.Bytes));
        }


        private void Expect_Output(params string[] expectedHex)
        {
            var abExpected = HexStringsToByteArray(expectedHex);
            Assert.AreEqual(abExpected, stream.ToArray());
        }

        private static byte[] HexStringsToByteArray(params string[] expectedHex)
        {
            return expectedHex
                .SelectMany(hex => hex.Split(' '))
                .Select(hex => (byte)int.Parse(hex, NumberStyles.HexNumber))
                .ToArray();
        }

        [Test]
        public void Sufa_Save_Empty()
        {
            Given_MemoryStream();
            Given_SuffixArrays();

            var sufaPersister = new SuffixArrayPersister(sc);
            sufaPersister.Save(sufas, stream);

            Expect_Output(
                "52 65 6B 6F 53 66 78 1A",
                "00 00 00 00");
        }

        [Test]
        public void Sufa_Save_SingleSuffixArray()
        {
            Given_MemoryStream();
            Given_SuffixArrays("00 01 01 00 01");

            var sufaPersister = new SuffixArrayPersister(sc);
            sufaPersister.Save(sufas, stream);

            Expect_Output(
                "52 65 6B 6F 53 66 78 1A",
                "00 00 00 01",
                "00 01 00 00",  // linaddr
                "00 01 00 00",  // offset
                "00 00 00 14"  // length (inbytes)
                );
        }

    }
}
