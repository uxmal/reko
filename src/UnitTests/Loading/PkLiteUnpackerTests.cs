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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Loading;
using Reko.ImageLoaders.MzExe;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Loading
{
	[TestFixture]
	public class PkLiteUnpackerTests
	{
        [Test]
        public void ValidateImage()
        {
            MemoryArea rawImage = new MemoryArea(Address.SegPtr(0x0C00, 0), CreateMsdosHeader());
            ExeImageLoader exe = new ExeImageLoader(null, "foo.exe", rawImage.Bytes);
            Assert.IsTrue(PkLiteUnpacker.IsCorrectUnpacker(exe, rawImage.Bytes));
        }

        private byte[] CreateMsdosHeader()
        {
            ImageWriter stm = new LeImageWriter(new byte[16]);
            stm.WriteByte(0x4D);    // MZ
            stm.WriteByte(0x5A);
            stm.WriteBytes(0xCC, 4);
            stm.WriteLeUInt16(0x0090);
            stm.WriteBytes(0xCC, 0x12);
            stm.WriteByte(0x00);
            stm.WriteByte(0x00);
            stm.WriteByte(0x05);
            stm.WriteByte(0x21);
            stm.WriteString("PKLITE", Encoding.ASCII);
            stm.WriteBytes(0xCC, 0x0C);
            return stm.Bytes;
        }
	}
}
