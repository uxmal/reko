#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Core
{
    public class ImageReaderTests
    {
        [Test]
        public void ReadCString()
        {
            var img = new LeImageReader(new byte[] {
                0x12, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0x00, 0x12 },
                1);
            StringConstant str = img.ReadCString(PrimitiveType.Char);
            Assert.AreEqual("Hello world!", str.ToString());

        }

        [Test]
        public void ReadLengthPrefixedString()
        {
            var img =
                new LeImageReader(
                    new LoadedImage(
                        Address.Ptr32(0x10000),
                        new byte[] { 0x12, 0x34, 0x03, 0x00, 0x00, 0x00, 0x46, 0x00, 0x6f, 0x00, 0x6f, 0x00, 0x02, 0x02}),
                    2);
            StringConstant str = img.ReadLengthPrefixedString(PrimitiveType.Int32, PrimitiveType.WChar);
            Assert.AreEqual("Foo", str.ToString());
        }
    }
}
