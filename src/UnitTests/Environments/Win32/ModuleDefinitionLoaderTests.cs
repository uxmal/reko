#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Environments.Win32;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Environments.Win32
{
    [TestFixture]
    public class ModuleDefinitionLoaderTests
    {
        private ModuleDefinitionLoader dfl;
        private readonly string nl = "\r\n";    // DEF files are from the Windows world, and have CR-LFs in them.

        private void CreateDefFileLoader(string str)
        {
            dfl = new ModuleDefinitionLoader(new StringReader(str));
        }

        [Test]
        public void DFL_Create()
        {
            CreateDefFileLoader("");
        }

        [Test]
        public void DFL_CommentLine()
        {
            CreateDefFileLoader("; hello\r\n");
            TypeLibrary lib = dfl.Load();
            Assert.AreEqual(0, lib.Types.Count);
            Assert.AreEqual(0, lib.Signatures.Count);
        }

        [Test]
        public void DFL_Read_StdapiExport()
        {
            CreateDefFileLoader(
                "EXPORTS" + nl +
                " _Foo@4 @4" + nl);
            var lib = dfl.Load();
            var svc = lib.ServicesByName["Foo"];
            Assert.AreEqual("Foo", svc.Name);
            Assert.IsFalse(svc.Signature.ArgumentsValid, "We don't know the arguments");
            Assert.AreEqual(8, svc.Signature.StackDelta, "StackDelta includes the return address, which stdapi calls pop.");
        }
    }
}
