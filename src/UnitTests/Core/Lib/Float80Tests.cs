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
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class Float80Tests
    {
        private string F80(ushort expSign, ulong significand)
        {
            var f80 = new Float80(expSign, significand);
            return f80.ToString("G", CultureInfo.InvariantCulture);
        }

        [Test]
        public void F80_Zero()
        {
            Assert.AreEqual("0", F80(0, 0));
        }

        [Test]
        public void F80_One()
        {
            Assert.AreEqual("1", F80(0x3FFF, 0x8000000000000000UL));
        }

        [Test]
        public void F80_1_5()
        {
            Assert.AreEqual("1.5", F80(0x3FFF, 0xC000000000000000UL));
        }

        [Test]
        public void F80_1_75()
        {
            Assert.AreEqual("1.75", F80(0x3FFF, 0xE000000000000000UL));
        }

        [Test]
        public void F80_1_875()
        {
            Assert.AreEqual("1.875", F80(0x3FFF, 0xF000000000000000UL));
        }

        [Test]
        public void F80_NegInfinity()
        {
            Assert.AreEqual("-Infinity", F80(0xFFFF, 0x0000000000000000UL));
        }

        [Test]
        public void F80_NaN()
        {
            Assert.AreEqual("NaN", F80(0xFFFF, 0x4E614E4E614E4E61UL));
        }
    }
}
