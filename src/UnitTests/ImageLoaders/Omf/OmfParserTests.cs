#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Memory;
using Reko.ImageLoaders.Omf;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Omf
{
    [TestFixture]
    public class OmfParserTests
    {
        private OmfParser parser;

        [SetUp]
        public void Setup()
        {
            this.parser = null!;
        }

        private string Format(byte[] bytes)
        {
            return string.Join(" ", bytes.Select(b => b.ToString("X2")));
        }

        private void Expect_Record<T>(out T record)
            where T : OmfRecord
        {
            Assert.IsTrue(parser.TryReadRecord(out OmfRecord r));
            record = (T) r;
        }

        private void Given_Data(string sBytes)
        {
            var bytes = BytePattern.FromHexBytes(sBytes);
            var rdr = new LeImageReader(bytes);
            this.parser = new OmfParser(rdr);
        }


        [Test]
        public void Omfp_Lidata()
        {
            Given_Data("A2 0B 00 01 12 34 03 00 00 00 02 40 41 CC");

            Expect_Record<DataRecord>(out var dataRecord);

            Assert.AreEqual("40 41 40 41 40 41", Format(dataRecord.Data));
        }

        [Test]
        public void Omfp_Lidata_Recursive()
        {
            Given_Data(@"A2 16 00
                01 34 12
                02 00 02 00 
                   03 00 00 00 02 40 41
                   02 00 00 00 02 50 51 CC");

            Expect_Record<DataRecord>(out var dataRecord);

            Assert.AreEqual("40 41 40 41 40 41 50 51 50 51 40 41 40 41 40 41 50 51 50 51", Format(dataRecord.Data));
        }

        [Test]
        public void Omfp_Lidata_AlphaBeta()
        {
            Given_Data(@"A2 1B 00 
                01 00 00 
                04 00 02 00 
                   01 00 00 00 05 41 4C 50 48 41
                   01 00 00 00 04 42 45 54 41 A9");

            Expect_Record<DataRecord>(out var record);

            Assert.AreEqual("ALPHABETAALPHABETAALPHABETAALPHABETA", Encoding.ASCII.GetString(record.Data));
        }
    }
}
