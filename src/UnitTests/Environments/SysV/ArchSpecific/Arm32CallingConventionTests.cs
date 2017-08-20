#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Environments.SysV;
using Reko.Core.Types;
using Reko.UnitTests.Core.Serialization;
using Reko.UnitTests.Mocks;
using System;
using System.Xml;
using System.Xml.Serialization;
using Rhino.Mocks;
using Reko.Arch.Arm;

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class Arm32ProcedureSerializerTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private Arm32ProcessorArchitecture arch;
        private Arm32ProcedureSerializer ser;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            arch = new Arm32ProcessorArchitecture();
        }

        private void Given_ProcedureSerializer()
        {
            this.deserializer = mockFactory.CreateDeserializer(arch.PointerType.Size);
            this.ser = new Arm32ProcedureSerializer(arch, deserializer, "");
        }

        private void Verify(SerializedSignature ssig, string outputFilename)
        {
            using (FileUnitTester fut = new FileUnitTester(outputFilename))
            {
                XmlTextWriter x = new FilteringXmlWriter(fut.TextWriter);
                x.Formatting = Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(ssig.GetType());
                ser.Serialize(x, ssig);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void SvArm32Ps_Serialize()
        {
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = new FunctionType(
                new Identifier("r0", PrimitiveType.Word32, arch.GetRegister("r0")),
                new Identifier[] {
                    new Identifier("r0", PrimitiveType.Word32, arch.GetRegister("r0"))
                });

            SerializedSignature ssig = ser.Serialize(sig);
            Assert.IsNotNull(ssig.ReturnValue);
            Assert.AreEqual("r0", ((Register_v1)ssig.ReturnValue.Kind).Name);
        }

        [Test]
        public void SvArm32Ps_SerializeProcedure()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame());
            Address addr = Address.Ptr32(0x12345);
            Given_ProcedureSerializer();

            mr.ReplayAll();

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("foo", sproc.Name);
            Assert.AreEqual("00012345", sproc.Address);
        }

        [Test]
        public void SvArm32Ps_SerializeProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("", PrimitiveType.Word32, arch.GetRegister("r0")),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, arch.GetRegister("r0")),
                    })
            };

            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer();

            mr.ReplayAll();

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("r0", ((Register_v1)sproc.Signature.ReturnValue.Kind).Name);
        }

        private SerializedType Type(string typeName)
        {
            return new TypeReference_v1(typeName);
        }

        private Argument_v1 RegArg(SerializedType type, string regName)
        {
            return new Argument_v1
            {
                Type = type,
                Kind = new Register_v1 { Name = regName },
                Name = regName
            };
        }

        private Argument_v1 FpuArg(SerializedType type, string name)
        {
            return new Argument_v1(
                name,
                type,
                new Register_v1 { Name = name },
                false);
        }

        [Test]
        public void SvArm32Ps_DeserializeFpuReturnValue()
        {
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1
                {
                    Type = PrimitiveType_v1.Real64(),
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("Sequence r1:r0", sig.ReturnValue.Storage.ToString());
        }

        [Test]
        public void SvArm32Ps_Load_cdecl()
        {
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = PrimitiveType_v1.Int32(),
                    }
                }
            };
            Given_ProcedureSerializer();
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(0, sig.StackDelta);
        }

        [Test]
        public void SvArm32Ps_Load_IntArgs()
        {
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name="dc",
                        Type = PrimitiveType_v1.Int16(),
                    },
                    new Argument_v1
                    {
                        Name = "b1",
                        Type = PrimitiveType_v1.SChar8(),
                    },
                       new Argument_v1
                    {
                        Name = "w2",
                        Type = PrimitiveType_v1.Int32(),
                    },
                       new Argument_v1
                    {
                        Name = "h3",
                        Type = PrimitiveType_v1.Int16(),
                    },
                       new Argument_v1
                    {
                        Name = "b4",
                        Type = PrimitiveType_v1.UChar8(),
                    },
                    new Argument_v1
                    {
                        Name = "w5",
                        Type = PrimitiveType_v1.Int32(),
                    },
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = PrimitiveType_v1.Int32()
                    }
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var args = sig.Parameters;
            Assert.AreEqual("r0", args[0].Storage.ToString());
            Assert.AreEqual("r1", args[1].Storage.ToString());
            Assert.AreEqual("r2", args[2].Storage.ToString());
            Assert.AreEqual("r3", args[3].Storage.ToString());
            Assert.AreEqual("Stack +0000", args[4].Storage.ToString());
            Assert.AreEqual("Stack +0004", args[5].Storage.ToString());
            Assert.AreEqual("Stack +0008", args[6].Storage.ToString());
        }

        [Test]
        public void SvArm32Ps_mmap()
        {
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1
                {
                    Type = new PointerType_v1
                    {
                        PointerSize = 4,
                        DataType = new VoidType_v1(),
                    }
                },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "addr",
                        Type = new PointerType_v1
                        {
                            PointerSize = 4,
                            DataType = new VoidType_v1(),
                        }
                    },
                    new Argument_v1
                    {
                        Name = "length",
                        Type = PrimitiveType_v1.UInt32(),
                    },
                    new Argument_v1
                    {
                        Name = "prot",
                        Type = PrimitiveType_v1.Int32(),
                    },
                    new Argument_v1
                    {
                        Name = "flags",
                        Type = PrimitiveType_v1.Int32(),
                    },
                    new Argument_v1
                    {
                        Name = "fd",
                        Type = PrimitiveType_v1.Int32(),
                    },
                    new Argument_v1
                    {
                        Name = "offset",
                        Type = PrimitiveType_v1.Int32(),
                    }
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var args = sig.Parameters;

        }
    }
}
