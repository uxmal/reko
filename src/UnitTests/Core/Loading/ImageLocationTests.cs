#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Loading
{
    [TestFixture]
    public class ImageLocationTests
    {
        [Test]
        public void ImLoc_CreateFromPath()
        {
            var imloc = ImageLocation.FromUri(@"c:\This is plus+hash#.so");
            Assert.AreEqual(@"c:\This is plus+hash#.so", imloc.FilesystemPath);
        }

        [Test]
        public void ImLoc_CreateFromFileUri()
        {
            var imloc = ImageLocation.FromUri(@"file:///c:/This is plus+hash#.so");
            Assert.AreEqual(@"c:/This is plus+hash#.so", imloc.FilesystemPath);
        }

        [Test]
        public void ImLoc_CreateFromArchiveUri()
        {
            var imloc = ImageLocation.FromUri(@"archive:c:\This+is#+plus%2b#hash%23.so");
            Assert.AreEqual(@"c:\This is", imloc.FilesystemPath);
            Assert.AreEqual(2, imloc.Fragments.Length);
            Assert.AreEqual(" plus+", imloc.Fragments[0]);
            Assert.AreEqual("hash#.so", imloc.Fragments[1]);
        }

        [Test]
        public void ImLoc_MakeRelative()
        {
            var imbase = ImageLocation.FromUri("/home/bob/test.proj");
            var imrel = ImageLocation.FromUri("/home/bob/test.exe");
            Assert.AreEqual("test.exe", imbase.MakeRelativeUri(imrel));
        }

        [Test]
        public void ImLoc_MakeRelative_WithArchive()
        {
            var imbase = ImageLocation.FromUri("/home/bob/test.proj");
            var imrel = ImageLocation.FromUri("archive:/home/bob/test.tar#test.exe");
            Assert.AreEqual("archive:test.tar#test.exe", imbase.MakeRelativeUri(imrel));
        }

        [Test]
        public void Imloc_Combine_SimplePaths()
        {
            var imbase = ImageLocation.FromUri(OsPath.Absolute("dir"));
            var combined = imbase.Combine("test.exe");
            Assert.AreEqual(OsPath.Absolute("dir", "test.exe"), combined.ToString());
        }

        [Test]
        public void Imloc_Combine_ArchivePath()
        {
            var imbase = ImageLocation.FromUri(OsPath.Absolute("dir"));
            var combined = imbase.Combine("archive:test.tar#test.exe");
            Assert.AreEqual(OsPath.Absolute("dir", "test.tar"), combined.FilesystemPath);
            Assert.AreEqual("test.exe", combined.Fragments[0]);
        }
    }
}
