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
using Decompiler.ImageLoaders.Hunk;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.ImageLoaders.Hunk
{
    [TestFixture]
    public class HunkLoaderTests
    {
        private HunkMaker mh;

        [SetUp]
        public void Setup()
        {
            mh = new HunkMaker();
        }


        [Test]
        public void Hunk_LoadFile()
        {
            var bytes = File.ReadAllBytes(
                FileUnitTester.MapTestPath("../UnitTests/Arch/M68k/images/FIBO"));
            var ldr = new HunkLoader(null, "FIBO", bytes);
            ldr.Load(Address.Ptr32(0x10000)); 
        }

        [Test]
        public void Hunk_LoadEmpty()
        {
            var bytes = mh.MakeBytes(
                HunkType.HUNK_HEADER,
                "",
                0,
                0,
                0,
                0);
            var ldr = new HunkLoader(null, "foo.bar", bytes);
            var ldImg = ldr.Load(Address.Ptr32(0x00010000));
            Assert.AreEqual(1, ldImg.ImageMap.Segments.Count);
            Assert.AreEqual(Address.Ptr32(0x00010000), ldImg.ImageMap.Segments.Values[0].Address);
        }

        [Test]
        public void Hunk_LoadCode()
        {
            var bytes = mh.MakeBytes(
                HunkType.HUNK_HEADER,
                "CODE",
                "",
                1,
                0,
                0,
                0x40,
                HunkType.HUNK_CODE,
                1,
                (ushort) 0x4E75,
                (ushort) 0,
                HunkType.HUNK_END);
            var ldr = new HunkLoader(null, "foo.bar", bytes);
            var ldImg = ldr.Load(Address.Ptr32(0x00010000));
            var rlImg = ldr.Relocate(Address.Ptr32(0x00010000));
            Assert.AreEqual(1, rlImg.EntryPoints.Count);
            Assert.AreEqual(0x00010000ul, rlImg.EntryPoints[0].Address.ToLinear());

        }
    }
}
