#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System.Linq;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class EndianServicesTests
    {
        private static void AssertHexBytes(string sExp, byte[] actual)
        {
            var sActual = string.Join("", actual.Select(b => $"{b:X2}"));
            Assert.AreEqual(sExp, sActual);
        }

        [Test]
        public void EndSvc_SliceStackIdentifier()
        {
            var stk = new StackStorage(-16, PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceStackStorage(stk, new BitRange(0, 8), 8);
            var beSlice = EndianServices.Big.SliceStackStorage(stk, new BitRange(0, 8), 8);
            Assert.AreEqual("Stack -0010", leSlice.ToString());
            Assert.AreEqual("Stack -000D", beSlice.ToString());
        }


        [Test]
        public void EndSvc_ReverseWords()
        {
            var bytes = new byte[] { 0x04, 0x03, 0x02, 0x01, 0x0F, 0xE, 0x0D, 0x0C };
            var newBytes = EndianServices.SwapByGroups(bytes, 4);
            AssertHexBytes("010203040C0D0E0F", newBytes);
        }
    }
}
