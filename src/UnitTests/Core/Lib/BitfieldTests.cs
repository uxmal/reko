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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class BitfieldTests
    {
        private Bitfield[] fields;
        private uint uValue;
        private int sValue;

        [SetUp]
        public void Setup()
        {
            fields = null;
            uValue = 0;
            sValue = 0;
        }

        private void  Given_Fields(params (int, int) [] fields)
        {
            this.fields = fields.Select(f => new Bitfield(f.Item1, f.Item2)).ToArray();
        }

        private void When_ReadUnsignedFields(uint u)
        {
            this.uValue = Bitfield.ReadFields(fields, u);
        }

        private void When_ReadSignedFields(uint u)
        {
            this.sValue = Bitfield.ReadSignedFields(fields, u);
        }

        [Test]
        public void Bif_SingleBit()
        {
            Given_Fields((0, 1));
            When_ReadUnsignedFields(0x1);
            Assert.AreEqual(1, uValue);
        }

        [Test]
        public void Bif_SplitField()
        {
            Given_Fields((28, 4),(0, 4));
            When_ReadUnsignedFields(0x71234567u);
            Assert.AreEqual(0x77, uValue);
        }

        [Test]
        public void Bif_Signed_OneBit()
        {
            Given_Fields((2, 1));
            When_ReadSignedFields(0x44444444);
            Assert.AreEqual(-1, sValue);
        }
    }
}
