#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.ImageLoaders.MzExe;
using Reko.ImageLoaders.MzExe.Borland;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.MzExe.Borland
{
    [TestFixture]
    public class SymbolLoaderTests
    {
        private LeImageWriter writer;
        private ExeImageLoader exeLoader;

        [SetUp]
        public void Setup()
        {
            this.writer = null;
            this.exeLoader = null;
        }

        private void Given_MzExeProgram(uint size)
        {
            this.writer = new LeImageWriter();
            writer.WriteBytes(0, size);
            var pos = writer.Position;
            writer.Position = 0;
            writer.WriteString("MZ", Encoding.ASCII);
            uint cPages = size / ExeImageLoader.CbPageSize;
            uint cbLastPage = size % ExeImageLoader.CbPageSize;
            if (cbLastPage > 0)
                ++cPages;
            writer.WriteLeUInt16((ushort)cbLastPage);
            writer.WriteLeUInt16((ushort)cPages);
            writer.Position = pos;
        }

        private void Given_ExeLoader()
        {
            this.exeLoader = new ExeImageLoader(null, "FOO.EXE", writer.ToArray());
            this.exeLoader.ReadCommonExeFields();
        }

        [Test(Description = "This binary has no Borland symbols.")]
        public void BorSymLdr_Fail()
        {
            Given_MzExeProgram(0x0430);

            Given_ExeLoader();

            var borsymldr = new SymbolLoader(exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsFalse(borsymldr.LoadDebugHeader());
        }

        [Test]
        public void BorSymLdr_HasHeader()
        {
            Given_MzExeProgram(0x0430);

            Given_ExeLoader();

            var borsymldr = new SymbolLoader(exeLoader, writer.ToArray(), Address.SegPtr(0x800, 0));
            Assert.IsFalse(borsymldr.LoadDebugHeader());
        }
    }
}
