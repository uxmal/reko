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
using X86ProcedureSerializer = Reko.Environments.SysV.X86ProcedureSerializer;

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    public class X86ProcedureSerializerTests
    {
        private static readonly string nl = Environment.NewLine;

        private MockRepository mr;
        private MockFactory mockFactory;
        private X86ArchitectureFlat32 arch;
        private X86ProcedureSerializer ser;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            arch = new X86ArchitectureFlat32();
        }

        private void Given_ProcedureSerializer()
        {
            this.deserializer = mockFactory.CreateDeserializer(arch.PointerType.Size);
            this.ser = new X86ProcedureSerializer(arch, deserializer, "");
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
        public void SvX86Ps_Serialize()
        {
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = new FunctionType(
                new Identifier("eax", PrimitiveType.Word32, arch.GetRegister("rbx")),
                new Identifier[] {
                    new Identifier("arg04", PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Int32))
                });

            SerializedSignature ssig = ser.Serialize(sig);
            Assert.IsNotNull(ssig.ReturnValue);
            Assert.AreEqual("eax", ssig.ReturnValue.Name);
            var sArg = (StackVariable_v1)ssig.Arguments[0].Kind;
        }

        [Test]
        public void SvX86Ps_SerializeProcedure()
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
        public void SvX86Ps_SerializeProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("eax", PrimitiveType.Word32, arch.GetRegister("eax")),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Int32))
                    })
            };

            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer();

            mr.ReplayAll();

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("eax", sproc.Signature.ReturnValue.Name);
        }

        [Test]
        public void SvX86Ps_DeserializeFpuArgument()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                ReturnValue = RegArg(PrimitiveType_v1.Int32(), "eax"),
                Arguments = new Argument_v1[] {
                    StackArg(PrimitiveType_v1.Real64(), "rArg04")
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("Register int32 test(Stack real64 rArg04)", sig.ToString("test"));
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

        private Argument_v1 StackArg(SerializedType type, string name)
        {
            return new Argument_v1(
                name,
                type,
                new StackVariable_v1(),
                false);
        }

        [Test]
        public void SvX86Ps_DeserializeFpuStackReturnValue()
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
            Assert.AreEqual(
                string.Format(
                    "FpuStack real64 foo(){0}// stackDelta: 0; fpuStackDelta: 1; fpuMaxParam: -1{0}",
                    nl),
                sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void SvX86Ps_Load_cdecl()
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
        public void SvX86Ps_Load_IntArgs()
        {
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name="hArg04",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize=2 },
                    },
                    new Argument_v1
                    {
                        Name = "wArg08",
                        Type = new PrimitiveType_v1 { Domain=Domain.SignedInt, ByteSize= 1 },
                    },
                       new Argument_v1
                    {
                        Name = "rArg0C",
                        Type = PrimitiveType_v1.Real32(),
                    },
                }
            };
            Given_ProcedureSerializer();

            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var args = sig.Parameters;
            Assert.AreEqual(
                string.Format(
                    "void foo(Stack int16 hArg04, Stack int8 wArg08, Stack real32 rArg0C){0}// stackDelta: 0; fpuStackDelta: 0; fpuMaxParam: -1{0}",
                    nl),
                sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }
    }
}
