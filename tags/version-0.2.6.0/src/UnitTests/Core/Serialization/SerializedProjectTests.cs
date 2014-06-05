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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedProjectTests
	{
		[Test]
		public void SudWrite()
		{
			Project_v1 ud = new Project_v1();
			ud.Input.Address = "0x1000:0x0";
			ud.Output.DisassemblyFilename = "foo.asm";
			ud.Output.IntermediateFilename = "foo.cod";
			SerializedProcedure proc = new SerializedProcedure();
			proc.Name = "foo";
			proc.Signature = new SerializedSignature
			{
				ReturnValue = new Argument_v1
				{
					Kind = new SerializedRegister("eax")
				},
				Arguments = new Argument_v1[]
				{
					new Argument_v1
					{
						Kind = new SerializedStackVariable(),
						Type = new SerializedPrimitiveType(Domain.SignedInt, 4)
					},
					new Argument_v1
					{
						Kind = new SerializedStackVariable(),
						Type = new SerializedPrimitiveType(Domain.SignedInt, 4)
					}
				}
			};
			ud.UserProcedures.Add(proc);

			using (FileUnitTester fut = new FileUnitTester("Core/SudWrite.txt"))
			{
			    var writer = new FilteringXmlWriter(fut.TextWriter);
				writer.Formatting = System.Xml.Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v1));
				ser.Serialize(writer, ud);
				fut.AssertFilesEqual();
			}
		}

        [Test]
        public void SudSaveProject()
        {
            Project project = new Project
            {
                InputFiles = 
                {
                    new InputFile
                    {
                        BaseAddress = new Address(0x1000, 0),
                        DisassemblyFilename = "foo.asm",
                        IntermediateFilename = "foo.cod",
                        UserProcedures = new SortedList<Address,SerializedProcedure> 
                        {
                            { 
                                new Address(0x1000, 0x10), 
                                new SerializedProcedure
                                {
                                    Name = "foo",
                                    Signature = new SerializedSignature
                                    {
                                        ReturnValue = new Argument_v1 { Kind = new SerializedRegister("eax") },
                                        Arguments = new Argument_v1[]
                                        {
                                            new Argument_v1
                                            {
                                                Kind = new SerializedStackVariable(),
                                                Type = new SerializedPrimitiveType(Domain.SignedInt, 4)
                                            },
                                            new Argument_v1
                                            {
                                                Kind = new SerializedStackVariable(),
                                                Type = new SerializedPrimitiveType(Domain.SignedInt, 4)
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            using (FileUnitTester fut = new FileUnitTester("Core/SudSaveProject.txt"))
            {
                FilteringXmlWriter writer = new FilteringXmlWriter(fut.TextWriter);
                writer.Formatting = System.Xml.Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v2(typeof(Project_v2));
                Project_v2 ud = project.Save();
                ser.Serialize(writer, ud);
                fut.AssertFilesEqual();
            }
        }
		
        [Test]
		public void SudRead_v1()
		{
			Project_v1 proj = null;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Core/SudRead.xml"), FileMode.Open))
			{
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v1));
				proj = (Project_v1) ser.Deserialize(stm);
			}
			Assert.AreEqual("10003330", proj.Input.Address);
			Assert.AreEqual(2, proj.UserProcedures.Count);
			SerializedProcedure proc = (SerializedProcedure) proj.UserProcedures[0];
			Assert.IsNull(proc.Signature.ReturnValue);
		}

        [Test]
        public void SudLoadProject()
        {
            string input = @"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Decompiler"">
  <input>
    <address>10003330</address>
  </input>
  <output>
    <disassembly>foo.asm</disassembly>
    <intermediate-code>foo.cod</intermediate-code>
  </output>
  <procedure name=""foo"">
    <address>10004000</address>
    <signature>
      <arg>
        <stack size=""4"" />
      </arg>
      <arg>
        <stack size=""4"" />
      </arg>
    </signature>
  </procedure>
  <procedure name=""bar"">
    <address>10005000</address>
    <signature>
      <return><reg>eax</reg></return>
      <arg><reg>ecx</reg></arg>
    </signature>
  </procedure>
</project>";
            Project_v1 proj = null;
            using (StringReader rdr = new StringReader(input))
            {
                var ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v1));
                proj = (Project_v1)ser.Deserialize(rdr);
            }
            var project = new ProjectSerializer().LoadProject(proj);
            Assert.AreEqual(0x10003330, project.InputFiles[0].BaseAddress.Linear);
            Assert.AreEqual("foo.cod", project.InputFiles[0].IntermediateFilename);
            Assert.AreEqual(2, project.InputFiles[0].UserProcedures.Count);
            Assert.AreEqual("foo.asm", project.InputFiles[0].DisassemblyFilename);
            Assert.AreEqual(0x10004000, project.InputFiles[0].UserProcedures.Keys[0].Linear);
            SerializedProcedure proc = project.InputFiles[0].UserProcedures.Values[0];
            Assert.IsNull(proc.Signature.ReturnValue);
        }

		[Test]
		public void SudReadAlloca()
		{
			Project proj;
            using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Fragments/multiple/alloca.xml"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                proj = new ProjectSerializer().LoadProject(stm);
            }
			var proc = (SerializedProcedure) proj.InputFiles[0].UserProcedures.Values[0];
			Assert.AreEqual("alloca", proc.Name);
			Assert.IsTrue(proc.Characteristics.IsAlloca);
		}

        [Test]
        public void LoadProject_Inputs_v2()
        {
            var sProject = new Project_v2
            {
                Inputs = {
                    new DecompilerInput_v1 {
                        Filename = "foo.exe",
                        Address = "1000:0000",
                        Comment = "main file" 
                    },
                    new DecompilerInput_v1 {
                        Filename = "foo.bin",
                        Address = "1000:D000",
                        Comment = "overlay",
                        Processor = "x86-real-16",
                    }
                }
            };
            var ser = new ProjectSerializer();
            var project = ser.LoadProject(sProject);
            Assert.AreEqual(2, project.InputFiles.Count);
        }
	}
}
