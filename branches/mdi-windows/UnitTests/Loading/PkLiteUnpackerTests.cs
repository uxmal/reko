/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Loading;
using Decompiler.ImageLoaders.MzExe;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class PkLiteUnpackerTests
	{
		[Test]
        [Ignore("Don't rely on external files in unit tests.")]
		public void PkLiteLoad()
		{
            ServiceContainer sc = new ServiceContainer();
            sc.AddService(typeof (DecompilerEventListener), new FakeDecompilerEventListener());
			Loader l = new Loader(new FakeDecompilerConfiguration(), sc);
            Program prog = new Program();
			prog.Image = new ProgramImage(new Address(0xC00, 0), l.LoadImageBytes(FileUnitTester.MapTestPath("binaries/life.exe"), 0));
			ExeImageLoader exe = new ExeImageLoader(sc, prog.Image.Bytes);
			PkLiteUnpacker ldr = new PkLiteUnpacker(sc, exe, prog.Image.Bytes);
			ProgramImage img = ldr.Load(new Address(0xC00, 0));
			Assert.AreEqual(0x19EC0, img.Bytes.Length);
			ldr.Relocate(new Address(0xC00, 0), new List<EntryPoint>(), new RelocationDictionary());
		}

        [Test]
        public void ValidateImage()
        {
            Program prog = new Program();
            ProgramImage rawImage = new ProgramImage(new Address(0x0C00, 0), CreateMsdosHeader());
            ExeImageLoader exe = new ExeImageLoader(null, rawImage.Bytes);
            Assert.IsTrue(PkLiteUnpacker.IsCorrectUnpacker(exe, rawImage.Bytes));
        }


        private byte[] CreateMsdosHeader()
        {
            using (MemoryStream s = new MemoryStream())
            {
                ImageWriter stm = new ImageWriter(s);
                stm.WriteByte(0x4D);    // MZ
                stm.WriteByte(0x5A);
                stm.WriteBytes(0xCC, 4);
                stm.WriteLeUint16(0x0090);
                stm.WriteBytes(0xCC, 0x12);
                Console.WriteLine("{0:X}", s.Position);
                stm.WriteByte(0x00);
                stm.WriteByte(0x00);
                stm.WriteByte(0x05);
                stm.WriteByte(0x21);
                stm.WriteString("PKLITE", Encoding.ASCII);
                stm.WriteBytes(0xCC, 0x0C);
                s.Flush();
                Console.WriteLine("{0:X}", s.Position);
                return s.GetBuffer();
            }
        }


	}
}
