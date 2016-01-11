﻿#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

#if DEBUG
namespace Reko.Tools.C2Xml.UnitTests
{
    [TestFixture]
    public class XmlConverterTests
    {
        public class FakeArchitecture : ProcessorArchitecture
        {
            public FakeArchitecture()
            {
                base.PointerType = PrimitiveType.Pointer32;
            }

            public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
            {
                throw new NotImplementedException();
            }

            public override ImageReader CreateImageReader(LoadedImage img, ulong off)
            {
                throw new NotImplementedException();
            }

            public override ImageReader CreateImageReader(LoadedImage img, Address addr)
            {
                throw new NotImplementedException();
            }

            public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
            {
                throw new NotImplementedException();
            }

            public override ProcessorState CreateProcessorState()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
            {
                throw new NotImplementedException();
            }

            public override Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
            {
                throw new NotImplementedException();
            }

            public override FlagGroupStorage GetFlagGroup(string name)
            {
                throw new NotImplementedException();
            }

            public override FlagGroupStorage GetFlagGroup(uint grf)
            {
                throw new NotImplementedException();
            }

            public override RegisterStorage GetRegister(string name)
            {
                throw new NotImplementedException();
            }

            public override RegisterStorage GetRegister(int i)
            {
                throw new NotImplementedException();
            }

            public override RegisterStorage[] GetRegisters()
            {
                throw new NotImplementedException();
            }

            public override string GrfToString(uint grf)
            {
                throw new NotImplementedException();
            }

            public override Address MakeAddressFromConstant(Constant c)
            {
                throw new NotImplementedException();
            }

            public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
            {
                throw new NotImplementedException();
            }

            public override bool TryGetRegister(string name, out RegisterStorage reg)
            {
                throw new NotImplementedException();
            }

            public override bool TryParseAddress(string txtAddr, out Address addr)
            {
                throw new NotImplementedException();
            }
        }

        void RunTest(string c_code, string expectedXml)
        {
            StringReader reader = null;
            StringWriter writer = null;
            try
            {
                reader = new StringReader(c_code);
                writer = new StringWriter();
                var xWriter = new XmlnsHidingWriter(writer)
                {
                    Formatting = Formatting.Indented
                };
                var arch = new FakeArchitecture();
                var platform = new DefaultPlatform(null, arch);
                var xc = new XmlConverter(reader, xWriter, platform);
                xc.Convert();
                writer.Flush();
                Assert.AreEqual(expectedXml, writer.ToString());
            }
            catch
            {
                Debug.WriteLine(writer.ToString());
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
                if (reader != null)
                    reader.Dispose();
            }
        }

        [Test]
        public void C2X_Typedef()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""INT"">
      <prim domain=""SignedInt"" size=""4"" />
    </typedef>
  </Types>
</library>";
            RunTest("typedef int INT;", sExp);
        }

        [Test]
        public void C2X_Struct()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <struct name=""tagPoint"">
      <field offset=""0"" name=""x"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
      <field offset=""4"" name=""y"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
  </Types>
</library>";
            RunTest("struct tagPoint { int x; int y; };", sExp);
        }

        [Test]
        public void C2X_Selfref()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <struct name=""link"">
      <field offset=""0"" name=""next"">
        <ptr size=""4"">
          <struct name=""link"" />
        </ptr>
      </field>
    </struct>
  </Types>
</library>";
            RunTest("struct link { struct link *next; }; ", sExp);
        }

        [Test]
        public void C2X_FunctionDecl()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
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
            RunTest("size_t __cdecl strlen(const char *);", sExp);
        }

        [Test]
        public void C2X_ForwardDeclaration()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""FOO"">
      <struct name=""foo"" />
    </typedef>
  </Types>
</library>";
            RunTest("typedef struct foo FOO;", sExp);
        }

        [Test]
        public void C2X_ForwardDeclaration_ThenDeclaration()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""FOO"">
      <struct name=""foo"" />
    </typedef>
    <struct name=""foo"">
      <field offset=""0"" name=""x"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
      <field offset=""4"" name=""y"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
      <field offset=""8"" name=""z"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
  </Types>
</library>";
            RunTest(
                "typedef struct foo FOO;" +
                "struct foo { int x, y, z; };"
                , sExp);
        }


        [Test]
        public void C2X_TypeReference()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <struct name=""foo"">
      <field offset=""0"" name=""x"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
    <typedef name=""FOO"">
      <struct name=""foo"" />
    </typedef>
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
            RunTest(
                "typedef struct foo { int x; } FOO;" +
                "int bar(FOO * pfoo);",
                sExp);
        }

        [Test]
        public void C2x_Variant()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <union name=""u"">
      <alt name=""i"">
        <prim domain=""SignedInt"" size=""4"" />
      </alt>
      <alt name=""s"">
        <ptr size=""4"">
          <prim domain=""Character"" size=""1"" />
        </ptr>
      </alt>
      <alt name=""f"">
        <prim domain=""Real"" size=""4"" />
      </alt>
    </union>
    <struct name=""tagVariant"">
      <field offset=""0"" name=""type"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
      <field offset=""4"">
        <union name=""u"" />
      </field>
    </struct>
    <typedef name=""Variant"">
      <struct name=""tagVariant"" />
    </typedef>
  </Types>
</library>";
            RunTest(
                "typedef struct tagVariant { " +
                    "int type;" +
                    "union u {" +
                        "int i;" +
                        "char * s;" +
                        "float f;" +
                    "};" +
                "} Variant;",
                sExp);
        }

        [Test]
        public void C2X_SizedArray()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""PunchCard"">
      <arr length=""80"">
        <prim domain=""Character"" size=""1"" />
      </arr>
    </typedef>
  </Types>
</library>";
            RunTest(
                "typedef char PunchCard[80];",
                sExp);
        }

        [Test]
        public void C2X_UnsizedArray()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""PunchCard"">
      <arr>
        <prim domain=""Character"" size=""1"" />
      </arr>
    </typedef>
  </Types>
</library>";
            RunTest(
                "typedef char PunchCard[];",
                sExp);
        }

        [Test]
        public void C2X_StructWithArray()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""byte"">
      <prim domain=""UnsignedInt"" size=""1"" />
    </typedef>
    <struct name=""header"">
      <field offset=""0"" name=""signature"">
        <arr length=""16"">
          <type>byte</type>
        </arr>
      </field>
      <field offset=""16"" name=""length"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
  </Types>
</library>";
            RunTest(
                "typedef unsigned char byte;" +
                "typedef struct header {" +
                    "byte signature[16];" +
                    "int length;" +
                "};",
                sExp);
        }

        [Test]
        public void C2X_TypedefEnum()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <enum name=""eFoo"">
      <member name=""Bar"" value=""1"" />
      <member name=""Fooie"" value=""2"" />
    </enum>
    <typedef name=""Foo"">
      <enum name=""eFoo"" />
    </typedef>
  </Types>
</library>";
            RunTest(
                "typedef enum eFoo {" +
                    "Bar = 1," +
                    "Fooie" +
                "} Foo;",
                sExp);
        }

        [Test]
        public void C2X_ConvertEnum_DerivedValues()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <enum name=""Foo"">
      <member name=""Bar"" value=""1"" />
      <member name=""Quux"" value=""1"" />
    </enum>
  </Types>
</library>";
            RunTest(
                "enum Foo {" +
                    "Bar = 1, " +
                    "Quux = Bar, " +
                "};",
                sExp);
        }

        [Test]
        public void C2X_AnonymousStruct()
        {
            var sExp =
                @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <struct name=""struct_0"">
      <field offset=""0"" name=""x"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
    <struct name=""Foo"">
      <field offset=""0"" name=""a"">
        <struct name=""struct_0"" />
      </field>
    </struct>
  </Types>
</library>";
            RunTest(
                "struct Foo {" +
                    "struct { " +
                        "int x;" +
                    "} a;" +
                "};",
                sExp);

        }

        [Test]
        public void C2X_FunctionWithStackParameters()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature convention=""__stdcall"">
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""bar"">
        <prim domain=""SignedInt"" size=""4"" />
      </arg>
      <arg name=""foo"">
        <ptr size=""4"">
          <prim domain=""Character"" size=""1"" />
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest("int __stdcall foo(int bar, char * foo);",
                sExp);
        }

        [Test]
        public void C2X_typedef_enum()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <enum name=""_foo"">
      <member name=""Bar"" value=""1"" />
    </enum>
    <typedef name=""Foo"">
      <enum name=""_foo"" />
    </typedef>
    <typedef name=""PFoo"">
      <ptr size=""4"">
        <enum name=""_foo"" />
      </ptr>
    </typedef>
  </Types>
</library>";
            RunTest("typedef enum _foo { Bar = 1 } Foo, *PFoo;", sExp);
        }

        [Test]
        public void C2x_typedef_anonymous_enum()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <enum name=""enum_0"">
      <member name=""Bar"" value=""1"" />
    </enum>
    <typedef name=""Foo"">
      <enum name=""enum_0"" />
    </typedef>
    <typedef name=""PFoo"">
      <ptr size=""4"">
        <enum name=""enum_0"" />
      </ptr>
    </typedef>
  </Types>
</library>";
            RunTest("typedef enum { Bar = 1 } Foo, *PFoo;", sExp);
        }

        [Test]
        public void C2X_typedef_anonymous_struct()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <struct name=""struct_0"">
      <field offset=""0"" name=""bar"">
        <prim domain=""SignedInt"" size=""4"" />
      </field>
    </struct>
    <typedef name=""Foo"">
      <struct name=""struct_0"" />
    </typedef>
    <typedef name=""PFoo"">
      <ptr size=""4"">
        <struct name=""struct_0"" />
      </ptr>
    </typedef>
  </Types>
</library>";
            RunTest("typedef struct { int bar; } Foo, *PFoo;", sExp);
        }

        [Test]
        public void C2X_typedef_stack_params()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""HANDLE"">
      <ptr size=""4"">
        <void />
      </ptr>
    </typedef>
  </Types>
  <procedure name=""foo"">
    <signature>
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""bar"">
        <type>HANDLE</type>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(
                "typedef void * HANDLE;" +
                "int foo(HANDLE bar);",
                sExp);
        }

        [Test]
        public void C2X_fn_with_typedef_short()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""SHORT"">
      <prim domain=""SignedInt"" size=""2"" />
    </typedef>
  </Types>
  <procedure name=""foo"">
    <signature>
      <return>
        <type>SHORT</type>
      </return>
      <arg name=""inp"">
        <type>SHORT</type>
      </arg>
      <arg name=""outp"">
        <ptr size=""4"">
          <type>SHORT</type>
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(
                "typedef short SHORT;" +
                "SHORT foo(SHORT inp, SHORT * outp);",
                sExp);
        }

        [Test]
        public void C2X_Void_fn()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature>
      <return>
        <void />
      </return>
    </signature>
  </procedure>
</library>";
            RunTest(
                "void foo(void);",
                sExp);

        }

        [Test]
        public void C2X_pfn_arg()
        {
            var cCode =
                "typedef int (__stdcall *PRTL_RUN_ONCE_INIT_FN) (" +
                     "int RunOnce" +
                ");" +
                "void foo(PRTL_RUN_ONCE_INIT_FN q);";
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""PRTL_RUN_ONCE_INIT_FN"">
      <ptr size=""4"">
        <fn>
          <return>
            <prim domain=""SignedInt"" size=""4"" />
          </return>
          <arg name=""RunOnce"">
            <prim domain=""SignedInt"" size=""4"" />
          </arg>
        </fn>
      </ptr>
    </typedef>
  </Types>
  <procedure name=""foo"">
    <signature>
      <return>
        <void />
      </return>
      <arg name=""q"">
        <type>PRTL_RUN_ONCE_INIT_FN</type>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_fn_with_array_parameter()
        {
            var cCode = "int foo(char arr[]);";
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature>
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""arr"">
        <ptr>
          <prim domain=""Character"" size=""1"" />
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_typedef_2d_array()
        {
            var cCode = "typedef float matrix[3][4];";
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""matrix"">
      <arr length=""3"">
        <arr length=""4"">
          <prim domain=""Real"" size=""4"" />
        </arr>
      </arr>
    </typedef>
  </Types>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_fn_with_2d_array_parameter()
        {
            var cCode = "int foo(char matrix[3][4]);";
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature>
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""matrix"">
        <ptr>
          <arr length=""4"">
            <prim domain=""Character"" size=""1"" />
          </arr>
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_fn_with_structref()
        {
            var cCode =
                "struct foo { int x; };" +
                "int bar(struct foo * pfoo);";

            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
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
          <struct name=""foo"" />
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_fn_with_pfn_args()
        {
            var cCode =
                "int __libc_start_main(int (*main) (int, char **, char **), int argc, char ** ubp_av, void (*init) (void), void (*fini) (void), void (*rtld_fini) (void), void (* stack_end));";
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""__libc_start_main"">
    <signature>
      <return>
        <prim domain=""SignedInt"" size=""4"" />
      </return>
      <arg name=""main"">
        <ptr size=""4"">
          <fn>
            <return>
              <prim domain=""SignedInt"" size=""4"" />
            </return>
            <arg>
              <prim domain=""SignedInt"" size=""4"" />
            </arg>
            <arg>
              <ptr size=""4"">
                <ptr size=""4"">
                  <prim domain=""Character"" size=""1"" />
                </ptr>
              </ptr>
            </arg>
            <arg>
              <ptr size=""4"">
                <ptr size=""4"">
                  <prim domain=""Character"" size=""1"" />
                </ptr>
              </ptr>
            </arg>
          </fn>
        </ptr>
      </arg>
      <arg name=""argc"">
        <prim domain=""SignedInt"" size=""4"" />
      </arg>
      <arg name=""ubp_av"">
        <ptr size=""4"">
          <ptr size=""4"">
            <prim domain=""Character"" size=""1"" />
          </ptr>
        </ptr>
      </arg>
      <arg name=""init"">
        <ptr size=""4"">
          <fn>
            <return>
              <void />
            </return>
          </fn>
        </ptr>
      </arg>
      <arg name=""fini"">
        <ptr size=""4"">
          <fn>
            <return>
              <void />
            </return>
          </fn>
        </ptr>
      </arg>
      <arg name=""rtld_fini"">
        <ptr size=""4"">
          <fn>
            <return>
              <void />
            </return>
          </fn>
        </ptr>
      </arg>
      <arg name=""stack_end"">
        <ptr size=""4"">
          <void />
        </ptr>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(cCode, sExp);
        }

        [Test]
        public void C2X_Parameter_Attribute()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature>
      <return>
        <void />
      </return>
      <arg name=""parm"">
        <prim domain=""SignedInt"" size=""4"" />
        <reg>D0</reg>
      </arg>
    </signature>
  </procedure>
</library>";
            RunTest(
                "void foo([[reko::reg(\"D0\")]]int parm);",
                sExp);

        }

        [Test]
        public void C2X_Return_Attribute()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types />
  <procedure name=""foo"">
    <signature>
      <return>
        <prim domain=""Character"" size=""1"" />
        <reg>D0</reg>
      </return>
    </signature>
  </procedure>
</library>";
            RunTest(
                "[[reko::reg(\"D0\")]] char foo();",
                sExp);

        }

        [Test]
        public void C2X_NearPtr()
        {
            var sExp = @"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""PVOID"">
      <ptr size=""2"">
        <void />
      </ptr>
    </typedef>
  </Types>
</library>";
            RunTest("typedef void _near * PVOID;", sExp);
        }
    }
}
#endif