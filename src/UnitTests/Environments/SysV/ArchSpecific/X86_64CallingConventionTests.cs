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
using Reko.Arch.X86;
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

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    public class X86_64ProcedureSerializerTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private X86ArchitectureFlat64 arch;
        private X86_64ProcedureSerializer ser;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            arch = new X86ArchitectureFlat64();
        }

        private void Given_ProcedureSerializer()
        {
            this.deserializer = mockFactory.CreateDeserializer(arch.PointerType.Size);
            this.ser = new X86_64ProcedureSerializer(arch, deserializer, "");
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
        public void SvAmdPs_Serialize()
        {
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = new FunctionType(
                new Identifier("rbx", PrimitiveType.Word32, arch.GetRegister("rbx")),
                new Identifier[] {
                    new Identifier("rbx", PrimitiveType.Word32, arch.GetRegister("rbx"))
                });

            SerializedSignature ssig = ser.Serialize(sig);
            Assert.IsNotNull(ssig.ReturnValue);
            Assert.AreEqual("rbx", ssig.ReturnValue.Name);
            Register_v1 sreg = (Register_v1)ssig.ReturnValue.Kind;
            Assert.AreEqual("rbx", sreg.Name);
        }

        [Test]
        public void SvAmdPs_SerializeProcedure()
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
        public void SvAmdPs_SerializeProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("rax", PrimitiveType.Word32, arch.GetRegister("rax")),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, arch.GetRegister("rdi")),
                    })
            };

            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer();

            mr.ReplayAll();

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("rax", sproc.Signature.ReturnValue.Name);
        }

        [Test]
        public void SvAmdPs_DeserializeFpuArgument()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                ReturnValue = RegArg(Type("int"), "rax"),
                Arguments = new Argument_v1[] {
                    FpuArg(Type("double"), "xmm0")
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("Register int test(Register double xmm0)", sig.ToString("test"));
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
        public void SvAmdPs_DeserializeFpuStackReturnValue()
        {
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1
                {
                    Type = new PrimitiveType_v1(Domain.Real, 8),
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("xmm0", sig.ReturnValue.Storage.ToString());
        }

        [Test]
        public void SvAmdPs_Load_cdecl()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__cdecl",
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    }
                }
            };
            Given_ProcedureSerializer();
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(0, sig.StackDelta);
        }

        [Test]
        public void SvAmdPs_Load_IntArgs()
        {
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name="dc",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize=2 },
                    },
                    new Argument_v1
                    {
                        Name = "b2",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 1 },
                    },
                       new Argument_v1
                    {
                        Name = "w3",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 4 },
                    },
                       new Argument_v1
                    {
                        Name = "h4",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize=2 },
                    },
                       new Argument_v1
                    {
                        Name = "b5",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 1 },
                    },
                    new Argument_v1
                    {
                        Name = "i6",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 4 },
                    },
                    new Argument_v1
                    {
                        Name = "b7",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 1 },
                    }
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var args = sig.Parameters;
            Assert.AreEqual("rdi", args[0].Storage.ToString());
            Assert.AreEqual("rsi", args[1].Storage.ToString());
            Assert.AreEqual("rdx", args[2].Storage.ToString());
            Assert.AreEqual("rcx", args[3].Storage.ToString());
            Assert.AreEqual("r8", args[4].Storage.ToString());
            Assert.AreEqual("r9", args[5].Storage.ToString());
            Assert.AreEqual("Stack +0008", args[6].Storage.ToString());
        }
    }
}
