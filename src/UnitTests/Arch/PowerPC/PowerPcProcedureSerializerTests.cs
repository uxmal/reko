#region License
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
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Environments.SysV;
using Reko.Core.Types;
using Reko.UnitTests.Core.Serialization;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Xml;
using System.Xml.Serialization;
using PowerPcProcedureSerializer = Reko.Arch.PowerPC.PowerPcProcedureSerializer;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcProcedureSerializerTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private PowerPcArchitecture arch;
        private PowerPcProcedureSerializer ser;
        private SysVPlatform platform;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            arch = new PowerPcArchitecture32();
            platform = new SysVPlatform(null, arch);
        }

        private void Given_ProcedureSerializer()
        {
            this.deserializer = mockFactory.CreateDeserializer(arch.PointerType.Size);
            this.ser = new PowerPcProcedureSerializer(arch, deserializer, "");
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
        public void PpcPs_Serialize()
        {
            Given_ProcedureSerializer();
            mr.ReplayAll();

            FunctionType sig = new FunctionType(
                new Identifier("qax", PrimitiveType.Word32, arch.Registers[3]),
                new Identifier[] {
                    new Identifier("qbx", PrimitiveType.Word32, arch.Registers[3])
                });

            SerializedSignature ssig = ser.Serialize(sig);
            Assert.IsNotNull(ssig.ReturnValue);
            Assert.AreEqual("qax", ssig.ReturnValue.Name);
            Register_v1 sreg = (Register_v1)ssig.ReturnValue.Kind;
            Assert.AreEqual("r3", sreg.Name);
        }

        [Test]
        public void PpcPs_SsigSerializeAxBxCl()
        {
            Given_ProcedureSerializer();
            mr.ReplayAll();

            var ssig = ser.Serialize(SerializedSignatureTests.MkSigAxBxCl());
            Verify(ssig, "Core/SsigSerializeAxBxCl.txt");
        }

        [Test]
        public void PpcPs_SerializeProcedure()
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
        public void PpcPs_SerializeProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("eax", PrimitiveType.Word32, arch.Registers[3]),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, arch.Registers[3])
                    })
            };

            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer();
            mr.ReplayAll();

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("eax", sproc.Signature.ReturnValue.Name);
        }

        [Test]
        public void PpcPs_DeserializeFpuArgument()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                ReturnValue = RegArg(Type("int"), "r3"),
                Arguments = new Argument_v1[] {
                    FpuArg(Type("double"), "f1")
                }
            };
            Given_ProcedureSerializer();
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("Register int test(Register double f1)", sig.ToString("test"));
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
        public void PpcPs_DeserializeFpuStackReturnValue()
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
            Assert.AreEqual("f1", sig.ReturnValue.Storage.ToString());
        }

        [Test]
        public void PpcPs_Load_cdecl()
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
        public void PpcPs_Load_LongArg()
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
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 8 },
                    }
                }
            };
            Given_ProcedureSerializer();
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var arg = sig.Parameters[1].Storage;
            Assert.AreEqual("Sequence r5:r6", arg.ToString());
        }
    }
}
