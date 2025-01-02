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
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class BytePatternTests
    {
        private void AssertEqual(string sHexString, params byte[] expected)
        {
            var actual = BytePattern.FromHexBytes(sHexString);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], actual[i], $"Different at position {i}");
            }
        }

        [Test]
        public void Bp_ValidUppercaseHexDigits()
        {
            AssertEqual("09ACF2", 0x09, 0xAC, 0xF2);
        }

        [Test]
        public void Bp_ValidLowercaseHexDigits()
        {
            AssertEqual("09bdef", 0x09, 0xBD, 0xEF);
        }
    }
}
