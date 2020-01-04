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

using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class PrimitiveTypeTests
	{
		[Test]
		public void PredefinedTypes()
		{
			Assert.AreEqual("bool", PrimitiveType.Bool.ToString());
			Assert.AreEqual("byte", PrimitiveType.Byte.ToString());
			Assert.AreEqual("char", PrimitiveType.Char.ToString());
			Assert.AreEqual("int8", PrimitiveType.SByte.ToString());
			Assert.AreEqual("uint8", PrimitiveType.UInt8.ToString());
			Assert.AreEqual("word16", PrimitiveType.Word16.ToString());
			Assert.AreEqual("int16", PrimitiveType.Int16.ToString());
			Assert.AreEqual("uint16", PrimitiveType.UInt16.ToString());
			Assert.AreEqual("word32", PrimitiveType.Word32.ToString());
			Assert.AreEqual("int32", PrimitiveType.Int32.ToString());
			Assert.AreEqual("uint32", PrimitiveType.UInt32.ToString());
			Assert.AreEqual("real32", PrimitiveType.Real32.ToString());
			Assert.AreEqual("real64", PrimitiveType.Real64.ToString());
			Assert.AreEqual("ptr32", PrimitiveType.Ptr32.ToString());
            Assert.AreEqual("segptr32", PrimitiveType.SegPtr32.ToString());
		}

		[Test]
		public void IsIntegral()
		{
			Assert.IsTrue(PrimitiveType.Int32.IsIntegral);
			Assert.IsFalse(PrimitiveType.Ptr64.IsIntegral);
			Assert.IsFalse(PrimitiveType.Real32.IsIntegral);
		}

		[Test]
		public void Hybrid32()
		{
			Assert.AreEqual("uip32", PrimitiveType.Create(Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer, 32).ToString());
        }

         [Test]
         public void CreateWord_NBitsTest()
         {
            var x1 = PrimitiveType.CreateWord(1);
            var x2 = PrimitiveType.CreateWord(1);

            for (int i = 64; i > 0; i--)
             {
                 try
                 {
                     var d1 = PrimitiveType.CreateWord(i);
                     var d2 = PrimitiveType.CreateWord(i);
                     Assert.AreSame(d1, d2, $"Not same for {i} bits");
                 }
                 catch (Exception ex)
                 {
                     Assert.IsTrue(false, $"CreateWord({i}) failed. Exception={ex.Message}");
                     throw;
                 }
             }
         }
	}
}
