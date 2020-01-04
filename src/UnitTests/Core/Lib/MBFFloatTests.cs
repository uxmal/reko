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
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class MBFFloatTests
    {
        private void AssertEqual(float fExpected, string bytes)
        {
            var mem = BytePattern.FromHexBytes(bytes).ToArray();
            var uFloat = MemoryArea.ReadLeUInt32(mem, 0);
            var mbf = new MBFFloat32(uFloat);
            var fActual = mbf.ToSingle(null);
            Assert.AreEqual(fExpected, fActual, 1e-30);
        }

        [Test]
        public void Mbf_Zero()
        {
            AssertEqual(0.0F,"00000000");
        }

        [Test]
        public void Mbf_Zero_WithGarbage()
        {
            AssertEqual(0.0F, "00003200");
        }

        [Test]
        public void Mbf_NegativeZero_WithGarbage()
        {
            AssertEqual(0.0F, "0000B200");
        }

        [Test]
        public void Mbf_1_0()
        {
            AssertEqual(1.0F, "00000081");
        }

        [Test]
        public void Mbf_2_5()
        {
            AssertEqual(2.5F, "00002082");
        }

        [Test]
        public void Mbf_Neg_10()
        {
            AssertEqual(-10.0F, "0000A084");
        }

        [Test]
        public void Mbf_Min()
        {
            AssertEqual(2.93873588e-39F, "00000001");
        }

        [Test]
        public void Mbf_Max()
        {
            AssertEqual(1.70141173e+38F, "FFFF7FFF");
        }

        [Test]
        public void Mbf_NegMax()
        {
            AssertEqual(-1.70141173e+38F, "FFFFFFFF");
        }
    }
}
