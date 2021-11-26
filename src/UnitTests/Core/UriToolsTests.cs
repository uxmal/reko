#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.UnitTests.Core
{
    [TestFixture]
    [Obsolete]
    [Ignore("Obsolete")]
    public class UriToolsTests
    {
        [Test]
        public void Ut_FilenameFromUri_NoFilePrefix()
        {
            Assert.AreEqual("hash#file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("hash#file.exe")));
            Assert.AreEqual("c:\\hash%20file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("c:\\hash%20file.exe")));
        }

        [Test]
        public void Ut_FilenameFromUri_FilePrefix_NoHash()
        {
            Assert.AreEqual("hash#file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("file:hash%23file.exe")));
            Assert.AreEqual("/home/user/Bob's file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("file:///home/user/Bob%27s%20file.exe")));
        }

        [Test]
        public void Ut_FilenameFromUri_FilePrefix_ShortName()
        {
            Assert.AreEqual("h", UriTools.FilePathFromUri(ImageLocation.FromUri("file:h")));
        }

        [Test]
        public void Ut_FilenameFromUri_FilePrefix_Hash()
        {
            Assert.AreEqual("a hash#file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("file:a+hash%23file.exe#archive/path")));
            Assert.AreEqual("/home/user/Bob's file.exe", UriTools.FilePathFromUri(ImageLocation.FromUri("file:///home/user/Bob%27s%20file.exe#archive/path")));
        }

        [Test]
        public void Ut_UriFromFilename_UnixPath_Relative()
        {
            Assert.AreEqual("file:local+path/with%23hash.so", UriTools.UriFromFilePath("local path/with#hash.so").FilesystemPath);
        }

        [Test]
        public void Ut_UriFromFilename_UnixPath_Absolute()
        {
            Assert.AreEqual("file:///abs+path/with%23hash.so", UriTools.UriFromFilePath("/abs path/with#hash.so").FilesystemPath);
        }

        [Test]
        public void Ut_UriFromFilename_WindowsPath_Relative()
        {
            Assert.AreEqual("file:local+path/with%23hash.so", UriTools.UriFromFilePath("local path\\with#hash.so").FilesystemPath);
        }

        [Test]
        public void Ut_UriFromFilename_WindowsPath_Absolute()
        {
            Assert.AreEqual("file:///c:/abs+path/with%23hash.dll", UriTools.UriFromFilePath("c:\\abs path\\with#hash.dll").FilesystemPath);
        }

        [Test]
        public void Ut_ParseFragments_One_frament()
        {
            var fragments = UriTools.ParseUriIntoFragments("file:///c:/dir/archive.zip#i+like+spaces.exe");
            Assert.AreEqual(2, fragments.Length);
            Assert.AreEqual("c:/dir/archive.zip", fragments[0]);
            Assert.AreEqual("i like spaces.exe", fragments[1]);
        }

        [Test]
        public void Ut_ParseFragments_Two_fragments()
        {
            var fragments = UriTools.ParseUriIntoFragments("file:///home/archive.zip#i+like+spaces.tar#ilike%23hash.exe");
            Assert.AreEqual(3, fragments.Length);
            Assert.AreEqual("/home/archive.zip", fragments[0]);
            Assert.AreEqual("i like spaces.tar", fragments[1]);
        }
    }
}
