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
using Reko.Scanning;
using System.Collections.Generic;
using System.Threading;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ProcedurePrologFinderTests : AbstractBaseFinderTests
    {
        private List<MaskedPattern> prologPatterns;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.prologPatterns = new List<MaskedPattern>();
        }

        private void Given_LePrologPattern(string hexBytes, string hexMask)
        {
            prologPatterns.Add(new MaskedPattern
            {
                Bytes = BytePattern.FromHexBytes(hexBytes),
                Mask = BytePattern.FromHexBytes(hexMask),
                Endianness = EndianServices.Little
            });
        }

        [Test]
        public void Ppf_FindPrologs_Reversed()
        {
            Given_LePrologPattern("1230", "FFF0");
            Given_LePrologPattern("12302340", "FFF0FFF0");

            Given_Bytes(0x102, "3912 FEDA EDB0");
            Given_Bytes(0x12E, "3B12 FADE CAFE");
            Given_Bytes(0x142, "3F12 4423 CAFE");

            Given_Pointer(0x166, 0xFF0102);
            Given_Pointer(0x16A, 0xFF012E);
            Given_Pointer(0x16E, 0xFF0142);

            var ppf = new ProcedurePrologFinder(arch.Object, prologPatterns, mem);
            var results = ppf.Run(new CancellationTokenSource().Token);
            Assert.AreEqual(0xFF0000, results[0].Address);
        }
    }
}
