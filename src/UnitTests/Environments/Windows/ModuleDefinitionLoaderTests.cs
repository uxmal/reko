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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Environments.Windows;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class ModuleDefinitionLoaderTests
    {
        private ModuleDefinitionLoader dfl;
        private Win32Platform platform;
        private readonly string nl = "\r\n";    // DEF files are from the Windows world, and have CR-LFs in them.

        private void CreateDefFileLoader(string absPath, string contents)
        {
            this.platform = new Win32Platform(null, new X86ArchitectureFlat32("x86-protected-32"));
            dfl = new ModuleDefinitionLoader(null, absPath, Encoding.ASCII.GetBytes(contents));
        }

        [Test]
        public void DFL_Create()
        {
            CreateDefFileLoader(OsPath.Absolute("bar", "foo.def"), "");
        }

        [Test]
        public void DFL_CommentLine()
        {
            CreateDefFileLoader(OsPath.Absolute("bar", "foo.def"), "; hello\r\n");
            TypeLibrary lib = dfl.Load(platform, new TypeLibrary());
            Assert.AreEqual(0, lib.Types.Count);
            Assert.AreEqual(0, lib.Signatures.Count);
        }

        [Test]
        public void DFL_Read_StdapiExport()
        {
            CreateDefFileLoader(
                OsPath.Absolute( "bar", "foo.def"),
                "EXPORTS" + nl +
                " _Foo@4 @4" + nl);
            var lib = dfl.Load(platform, new TypeLibrary());
            Assert.IsTrue(lib.Modules.ContainsKey("FOO.DLL"));
            var svc = lib.Modules["FOO.DLL"].ServicesByName["_Foo@4"];
            Assert.AreEqual("Foo", svc.Name);
            Assert.IsFalse(svc.Signature.ParametersValid, "We don't know the arguments");
            Assert.AreEqual(8, svc.Signature.StackDelta, "StackDelta includes the return address, which stdapi calls pop.");
        }

        [Test]
        public void DFL_Read_StdapiExport_With_Extra_Spaces()
        {
            CreateDefFileLoader(
                OsPath.Absolute("bar", "foo.def"),
                "EXPORTS" + nl +
                " _Foo@4 @ 4" + nl);
            var lib = dfl.Load(platform, new TypeLibrary());
            var svc = lib.Modules["FOO.DLL"].ServicesByName["_Foo@4"];
            Assert.AreEqual("Foo", svc.Name);
            Assert.AreEqual(4, svc.SyscallInfo.Vector);
        }

        [Test]
        public void DFL_LibraryStatement()
        {
            CreateDefFileLoader(
                OsPath.Absolute("bar", "foo.def"),
                " LIBRARY bar" + nl +
                "EXPORTS" + nl +
                " _foo@12 @ 1" + nl);
            var lib = dfl.Load(platform, new TypeLibrary());
            Assert.IsTrue(lib.Modules.ContainsKey("BAR"));
        }
    }
}
