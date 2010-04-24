/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using DecompilerHost = Decompiler.DecompilerHost;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests
{
    [TestFixture]
    public class DecompilerTests
    {
        FakeLoader loader;
        TestDecompiler decompiler;

        [SetUp]
        public void Setup()
        {
            var config = new FakeDecompilerConfiguration();
            loader = new FakeLoader();
            var host = new FakeDecompilerHost();
            var sp = new ServiceContainer();
            sp.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            decompiler = new TestDecompiler(loader, host, sp);
        }


        [Test]
        public void LoadIncompleteProject()
        {
            loader.ImageBytes = Encoding.UTF8.GetBytes(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<project xmlns=""http://schemata.jklnet.org/Decompiler"">
</project>");
            loader.ThrowOnLoadImage(new FileNotFoundException("file.dcproject"), 1);

            try
            {
                decompiler.LoadProgram("file.dcproject");
                Assert.Fail("Should have thrown exception.");
            }
            catch
            {
                Assert.IsNotNull(decompiler.Project);
                Assert.IsTrue(string.IsNullOrEmpty(decompiler.Project.Input.Filename));
                Assert.IsNull(decompiler.Program);
            }
        }

        [Test]
        public void LoadProjectFileNoBom()
        {
            loader.ImageBytes = new UTF8Encoding(false).GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><project xmlns=\"http://schemata.jklnet.org/Decompiler\">" +
                "<input><filename>foo.bar</filename></input></project>");
            decompiler.LoadProgram("test.dcproject");
            Assert.AreEqual("foo.bar", decompiler.Project.Input.Filename);
        }

    }

    public class TestDecompiler : DecompilerDriver
    {
        public TestDecompiler(LoaderBase loader, DecompilerHost host, IServiceProvider sp)
            : base(loader, host, sp)
        {
        }

    }

}
