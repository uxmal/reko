#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class TypeLibraryLoaderTests
    {
        private TypeLibraryLoader tlldr;
        private IPlatform platform;

        private void CreateTypeLibraryLoader(string filename, string contents)
        {
            this.platform = new Win32Platform(null, new X86ArchitectureFlat32("x86-protected-32"));
            tlldr = new TypeLibraryLoader(null, filename, Encoding.ASCII.GetBytes(contents));
        }

        [Test]
        public void TLLDR_Typedef()
        {
            var contents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""INT"">
      <prim domain=""SignedInt"" size=""4"" />
    </typedef>
  </Types>
</library>";
            CreateTypeLibraryLoader("c:\\bar\\foo.xml", contents);
            var lib = tlldr.Load(platform, new TypeLibrary());
            Assert.AreEqual(1, lib.Types.Count);
            Assert.AreEqual("int32", lib.Types["INT"].ToString());
            Assert.AreEqual(0, lib.Signatures.Count);
        }

        [Test]
        public void TLLDR_FunctionDecl()
        {
            var contents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""size_t"">
      <prim domain=""UnsignedInt"" size=""4"" />
    </typedef>
  </Types>
  <procedure name=""strlen"">
    <signature convention=""__cdecl"">
      <return>
        <type>size_t</type>
      </return>
      <arg>
        <ptr size=""4"">
          <prim domain=""Character"" size=""1"" />
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            CreateTypeLibraryLoader("c:\\bar\\foo.xml", contents);
            var lib = tlldr.Load(platform, new TypeLibrary());
            Assert.AreEqual(1, lib.Types.Count);
            Assert.AreEqual(1, lib.Signatures.Count);
            var sExp =
@"Register size_t strlen(Stack (ptr32 char) ptrArg04)
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, lib.Signatures["strlen"].ToString("strlen", FunctionType.EmitFlags.AllDetails)
            );
        }

        [Test]
        public void TLLDR_TypeReference()
        {
            var contents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""FOO"">
      <struct name=""foo"" />
    </typedef>
    <struct name=""foo"">
      <field offset=""0"" name=""x"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
  </Types>
  <procedure name=""bar"">
    <signature>
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""pfoo"">
        <ptr size=""4"">
          <type>FOO</type>
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            CreateTypeLibraryLoader("c:\\bar\\foo.xml", contents);
            var lib = tlldr.Load(platform, new TypeLibrary());
            Assert.AreEqual(1, lib.Types.Count);
            Assert.AreEqual(1, lib.Signatures.Count);
            Assert.AreEqual("(struct \"foo\" (0 int32 x))", lib.Types["FOO"].ToString());
            var sExp =
@"Register int32 bar(Stack (ptr32 FOO) pfoo)
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, lib.Signatures["bar"].ToString("bar", FunctionType.EmitFlags.AllDetails)
            );
        }

        [Test]
        public void TLLDR_Global()
        {
            var contents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""foo"">
      <struct name=""foo_t"" />
    </typedef>
  </Types>
  <global name=""g_foo"">
    <type>foo</type>
  </global>
</library>";
            CreateTypeLibraryLoader("c:\\bar\\foo.xml", contents);
            var lib = tlldr.Load(platform, new TypeLibrary());
            Assert.AreEqual(1, lib.Types.Count);
            Assert.AreEqual(1, lib.Globals.Count);
            var sExp = @"foo";
            Assert.AreEqual(sExp, lib.Globals["g_foo"].ToString());
        }
    }
}

