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
using Reko.Core.Configuration;
using Reko.UnitTests.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Configuration
{
    [TestFixture]
    public class RekoConfigurationTests
    {
        [Test]
        public void RekoCon_Save()
        {
            var rekoCon = new RekoConfiguration_v1
            {
                Loaders = new[]
                {
                    new RekoLoader
                    {
                        MagicNumber = "4D5A",
                        Type = "mzexeloader,mzexeloader"
                    }
                },
                RawFiles = new []
                {
                    new RawFile_v1
                    {
                        Name="ms-dos-com",
                        Description="MS-DOS COM executable",
                        Architecture="x86-real-16",
                        Environment="ms-dos",
                        Base="0C00:0100",
                        Entry = new EntryPoint_v1
                        {
                            Address = "0C00:0100",
                            Name = "MsDosCom_Start",
                        }
                    }
                },
                SignatureFiles = new []
                {
                    new SignatureFile_v1
                    {
                        Filename="IMAGE_FILE_MACHINE_I386.xml",
                        Type ="Reko.Loading.UnpackerSignatureLoader,Reko"
                    }
                },
                Environments = new []
                {
                    new Environment_v1 {
                        Name="elf-neutral",
                        Description ="Unspecified ELF ABI",
                        Type ="Reko.Environments.SysV.SysVPlatform,Reko.Environments.SysV",
                        TypeLibraries = new []
                        {
                            new TypeLibraryReference_v1 { Name="libc.so.xml" },
                        },
                        Characteristics = new []
                        {
                            new TypeLibraryReference_v1 { Name="sysvcharacteristics.xml" }
                        },
                        Heuristics = new PlatformHeuristics_v1
                        {
                            ProcedurePrologs = new []
                            {
                                new BytePattern_v1 {
                                    Bytes = "55 8B EC",
                                    Mask = "FF FF FF"
                                }
                            }
                        }
                    }
                },
                Architectures = new[]
                {
                    new Architecture_v1
                    {
                        Name="x86-real-16",
                        Description = "x86 16-bit Real Mode",
                        Type ="Reko.Arch.X86.X86ArchitectureReal,Reko.Arch.X86"
                    }
                },
                Assemblers = new []
                {
                    new Assembler_v1
                    {
                        Name = "pdp11-mac",
                        Description = "PDP-11 MACRO assembler",
                        Type = "Reko.Assemblers.Pdp11.Pdp11TextAssembler,Reko.Assemblers.Pdp11" 
                    }
                },
                UiPreferences = new RekoUiPreferences
                {
                    Styles = new []
                    {
                        new StyleDefinition_v1
                        {
                          Name="mem", Font="Lucida Console, 9pt"
                        },
                        new StyleDefinition_v1
                        {
                            Name ="mem-code", ForeColor="#000000", BackColor="#FFC0C0"
                        }
                    }
                }
            };

            var ser = new XmlSerializer(rekoCon.GetType());
            var stm = new StringWriter();
            var xml = new FilteringXmlWriter(stm);
            xml.Formatting = Formatting.Indented;
            ser.Serialize(xml, rekoCon);
            var sExp =
            #region Expected
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Reko xmlns=""http://schemata.jklnet.org/Reko/Config/v1"">
  <Loaders>
    <Loader MagicNumber=""4D5A"" Type=""mzexeloader,mzexeloader"" />
  </Loaders>
  <RawFiles>
    <RawFile Name=""ms-dos-com"" Description=""MS-DOS COM executable"" Arch=""x86-real-16"" Env=""ms-dos"" Base=""0C00:0100"">
      <Entry Addr=""0C00:0100"" Name=""MsDosCom_Start"" />
    </RawFile>
  </RawFiles>
  <SignatureFiles>
    <SignatureFile Filename=""IMAGE_FILE_MACHINE_I386.xml"" Type=""Reko.Loading.UnpackerSignatureLoader,Reko"" />
  </SignatureFiles>
  <Environments>
    <Environment Name=""elf-neutral"" Description=""Unspecified ELF ABI"" Type=""Reko.Environments.SysV.SysVPlatform,Reko.Environments.SysV"">
      <TypeLibraries>
        <TypeLibrary Name=""libc.so.xml"" />
      </TypeLibraries>
      <Characteristics>
        <TypeLibrary Name=""sysvcharacteristics.xml"" />
      </Characteristics>
      <Heuristics>
        <ProcedurePrologs>
          <Pattern>
            <Bytes>55 8B EC</Bytes>
            <Mask>FF FF FF</Mask>
          </Pattern>
        </ProcedurePrologs>
      </Heuristics>
    </Environment>
  </Environments>
  <Architectures>
    <Architecture Name=""x86-real-16"" Description=""x86 16-bit Real Mode"" Type=""Reko.Arch.X86.X86ArchitectureReal,Reko.Arch.X86"" />
  </Architectures>
  <Assemblers>
    <Assembler Name=""pdp11-mac"" Description=""PDP-11 MACRO assembler"" Type=""Reko.Assemblers.Pdp11.Pdp11TextAssembler,Reko.Assemblers.Pdp11"" />
  </Assemblers>
  <UiPreferences>
    <Style Name=""mem"" Font=""Lucida Console, 9pt"" />
    <Style Name=""mem-code"" ForeColor=""#000000"" BackColor=""#FFC0C0"" />
  </UiPreferences>
</Reko>";
            #endregion
            Assert.AreEqual(sExp, stm.ToString());
        }
    }
}
