#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Assemblers
{
    [TestFixture]
    public class EmitterTests
    {
        private void AssertResult(string sExpected, Emitter m)
        {
            sExpected = sExpected.Replace(" ", "");
            var bytes = m.GetBytes();
            var sActual = string.Join("", bytes.Select(b => $"{b:X2}"));
            Assert.AreEqual(sExpected, sActual);
        }

        [Test]
        public void Em_EmitBeUInt32()
        {
            var m = new Emitter();
            m.EmitBeUInt32(0x12345678);
            AssertResult("12345678", m);
        }

        [Test]
        public void Em_EmitWrite_ThenRead()
        {
            var m = new Emitter();
            m.EmitBeUInt32(0x12345678);
            var b = m.ReadByte(2);
            m.WriteByte(2, b ^ 0xFF);
            m.EmitByte(0x9A);
            AssertResult("1234A9789A", m);
        }
    }
}
