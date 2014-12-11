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
using Decompiler.Loading;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedProjectTests
	{
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

		[Test]
		public void SudWrite()
		{
            this.mr = new MockRepository();
			Project_v1 ud = new Project_v1();
			ud.Input.Address = "0x1000:0x0";
			ud.Output.DisassemblyFilename = "foo.asm";
			ud.Output.IntermediateFilename = "foo.cod";
			Procedure_v1 proc = new Procedure_v1();
			proc.Name = "foo";
			proc.Signature = new SerializedSignature
			{
				ReturnValue = new Argument_v1
				{
					Kind = new Register_v1("eax")
				},
				Arguments = new Argument_v1[]
				{
					new Argument_v1
					{
						Kind = new StackVariable_v1(),
						Type = new PrimitiveType_v1(Domain.SignedInt, 4)
					},
					new Argument_v1
					{
						Kind = new StackVariable_v1(),
						Type = new PrimitiveType_v1(Domain.SignedInt, 4)
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
                Programs = 
                {
                    new Program
                    {
                        Image = new LoadedImage(new Address(0x1000, 0), new byte[100]),
                        DisassemblyFilename = "foo.asm",
                        IntermediateFilename = "foo.cod",
                        UserProcedures = new SortedList<Address,Procedure_v1> 
                        {
                            { 
                                new Address(0x1000, 0x10), 
                                new Procedure_v1
                                {
                                    Name = "foo",
                                    Signature = new SerializedSignature
                                    {
                                        ReturnValue = new Argument_v1 { Kind = new Register_v1("eax") },
                                        Arguments = new Argument_v1[]
                                        {
                                            new Argument_v1
                                            {
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                            },
                                            new Argument_v1
                                            {
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        UserGlobalData =
                        {
                            { new Address(0x2000, 0) ,
                              new GlobalDataItem_v2 {
                                   Address = new Address(0x2000, 0).ToString(),
                                   DataType = new StringType_v2 { 
                                       Termination=StringType_v2.ZeroTermination, 
                                       CharType = new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 1 }
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
                Project_v2 ud = new ProjectSaver().Save(project);
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
			Procedure_v1 proc = (Procedure_v1) proj.UserProcedures[0];
			Assert.IsNull(proc.Signature.ReturnValue);
		}

		[Test]
		public void SudReadAlloca()
		{
            var loader = mr.Stub<ILoader>();
			Project proj;
            using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Fragments/multiple/alloca.xml"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                proj = new ProjectLoader(loader).LoadProject(stm);
            }
            var program = proj.Programs[0];
			var proc = (Procedure_v1) program.UserProcedures.Values[0];
			Assert.AreEqual("alloca", proc.Name);
			Assert.IsTrue(proc.Characteristics.IsAlloca);
		}

        [Test]
        public void Sud_LoadProject_Inputs_v2()
        {
            var loader = mr.Stub<ILoader>();
            var sProject = new Project_v2
            {
                Inputs = {
                    new DecompilerInput_v2 {
                        Filename = "foo.exe",
                        Address = "1000:0000",
                        Comment = "main file",
                        UserGlobalData = new List<GlobalDataItem_v2>
                        {
                            new GlobalDataItem_v2 { Address = "1000:0400", DataType = new StringType_v2 { 
                                Termination=StringType_v2.ZeroTermination,
                                CharType= new PrimitiveType_v1 { ByteSize = 1, Domain=Domain.Character} } 
                            }
                        }
                    },
                    new DecompilerInput_v2 {
                        Filename = "foo.bin",
                        Address = "1000:D000",
                        Comment = "overlay",
                        Processor = "x86-real-16",
                    }
                }
            };
            var ps = new ProjectLoader(loader);
            var project = ps.LoadProject(sProject);
            Assert.AreEqual(2, project.Programs.Count);
            var input0 = project.Programs[0];
            Assert.AreEqual(1, input0.UserGlobalData.Count);
            Assert.AreEqual("1000:0400", input0.UserGlobalData.Values[0].Address);
            var str_t = (StringType_v2)input0.UserGlobalData.Values[0].DataType;
            Assert.AreEqual("prim(Character,1)", str_t.CharType.ToString());
        }

        [Test]
        public void Sud_SaveMetadataReference()
        {
            var project = new Project
            {
                Programs =
                {
                    new Program
                    {
                        Filename = "c:\\test\\foo.exe",
                    }
                },
                MetadataFiles =
                {
                    new MetadataFile
                    {
                        Filename = "c:\\test\\foo.def",
                        LibraryName = "foo.def",
                    }
                }
            };
            var ps = new ProjectSaver();
            var sProject = ps.Save(project);
            Assert.AreEqual(1, project.MetadataFiles.Count);
            Assert.AreEqual("c:\\test\\foo.def", project.MetadataFiles[0].Filename);
            Assert.AreEqual("foo.def", project.MetadataFiles[0].LibraryName);
        }

        [Test]
        public void SudLoadMetadata()
        {
            var sProject = new Project_v2 {
                Inputs =
                {
                    new MetadataFile_v2
                    {
                        Filename = "c:\\tmp\\foo.def"
                    }
                }
            };
            var loader = mr.Stub<ILoader>();
            mr.ReplayAll();

            var ploader = new ProjectLoader(loader);
            var project = ploader.LoadProject(sProject);
            Assert.AreEqual(1, project.MetadataFiles.Count);
            Assert.AreEqual("c:\\tmp\\foo.def", project.MetadataFiles[0].Filename);
        }
	}
}
