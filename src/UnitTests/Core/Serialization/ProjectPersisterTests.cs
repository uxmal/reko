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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectPersisterTests
    {
        [Test]
        public void Prp_ToRelative1()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('/'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToProjectRelativePath("/home/bob/projects/foo.dcproj", "/home/bob/projects/reko/foo.c");
            Assert.AreEqual("reko/foo.c", s);
        }

        [Test]
        public void Prp_ToRelative2()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('/'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToProjectRelativePath("/home/bob/projects/foo/foo.dcproj", "/home/bob/projects/reko/foo.c");
            Assert.AreEqual("../reko/foo.c", s);
        }

        [Test]
        public void Prp_ToRelative3()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('/'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToProjectRelativePath("/home/bob/projects/foo/foo.dcproj", "/var/bob/reko/foo.c");
            Assert.AreEqual("/var/bob/reko/foo.c", s);
        }

        [Test]
        public void Prp_ToRelative_Msdos1()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('\\'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToProjectRelativePath(@"c:\Users\Bob\projects\foo.dcproj", @"c:\Users\Bob\reko\foo.c");
            Assert.AreEqual(@"..\reko\foo.c", s);
        }

        [Test]
        public void Prp_ToRelative_Msdos2()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('\\'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToProjectRelativePath(@"d:\Users\Bob\foo.dcproj", @"c:\Users\Bob\reko\foo.c");
            Assert.AreEqual(@"c:\Users\Bob\reko\foo.c", s);
        }

        [Test]
        public void Prp_ToAbsolute_Msdos1()
        {
            var sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('\\'));
            var pp = new ProjectPersister(sc);
            var s = pp.ConvertToAbsolutePath(
                OsPath.Absolute("Users", "Bob", "foo.dcproj"),
                OsPath.Relative("..", "reko", "foo.c"));
            Assert.AreEqual(OsPath.Absolute("Users", "reko", "foo.c"), s);
        }
    }
}

