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

using Moq;
using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
    public class ProcedureSerializerTests
    {
        private CommonMockFactory mockFactory;
        private IntelArchitecture arch;
        private ProcedureSerializer ser;
        private Win32Platform platform;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mockFactory = new CommonMockFactory();
            arch = new X86ArchitectureFlat32("x86-protected-32");
            platform = new Win32Platform(null, arch);
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
        private void Given_ProcedureSerializer(string cConvention)
        {
            this.deserializer = new FakeTypeDeserializer(32);
            this.ser = new ProcedureSerializer(platform, deserializer, cConvention);
        }

        [Test]
        public void X86ps_Test()
        {
            Given_ProcedureSerializer("stdapi");
            FunctionType sig = new FunctionType(
                new Identifier("qax", PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                    new Identifier("qbx", PrimitiveType.Word32, Registers.ebx)
                });

            SerializedSignature ssig = ser.Serialize(sig);
            Assert.IsNotNull(ssig.ReturnValue);
            Assert.AreEqual("qax", ssig.ReturnValue.Name);
            Register_v1 sreg = (Register_v1)ssig.ReturnValue.Kind;
            Assert.AreEqual("eax", sreg.Name);
        }

        [Test]
        public void X86ps_SerializeAxBxCl()
        {
            Given_ProcedureSerializer("stdapi");
            SerializedSignature ssig = ser.Serialize(SerializedSignatureTests.MkSigAxBxCl());
            Verify(ssig, "Core/SsigSerializeAxBxCl.txt");
        }

        [Test]
        public void X86ps_SerializeSequence()
        {
            Identifier seq = new Identifier("es_bx", PrimitiveType.Word32,
                new SequenceStorage(PrimitiveType.SegPtr32, Registers.es, Registers.bx));
            Given_ProcedureSerializer("stdapi");

            SerializedSignature ssig = ser.Serialize(new FunctionType(seq, new Identifier[0]));
            Verify(ssig, "Core/SsigSerializeSequence.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86ps_SerializeProcedure()
        {
            Address addr = Address.Ptr32(0x12345);
            Procedure proc = new Procedure(arch, "foo",  addr, arch.CreateFrame());
            Given_ProcedureSerializer("stdapi");

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("foo", sproc.Name);
            Assert.AreEqual("00012345", sproc.Address);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86ps_ProcedureWithSignature()
        {
            Address addr = Address.Ptr32(0x567A0C);
            Procedure proc = new Procedure(arch, "foo", addr, arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("eax", PrimitiveType.Word32, Registers.eax),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, new StackArgumentStorage(0, PrimitiveType.Word32))
                    })
            };

            Given_ProcedureSerializer("stdapi");

            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("eax", sproc.Signature.ReturnValue.Name);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86ps_DeserializeFpuStackargument()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                ReturnValue = RegArg(Type("int"), "eax"),
                Arguments = new Argument_v1[] {
                    FpuArg(Type("double"),  null)
                }
            };
            Given_ProcedureSerializer("stdapi");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(-1, sig.FpuStackDelta);
            Assert.AreEqual(4, sig.StackDelta);
            Assert.AreEqual("Register int test(FpuStack real64 fpArg0)", sig.ToString("test"));
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
                Kind = new Register_v1 { Name = "eax" },
                Name = regName
            };
        }

        private Argument_v1 FpuArg(SerializedType type, string name)
        {
            return new Argument_v1(
                name,
                type,
                new FpuStackVariable_v1 { ByteSize = 8 },
                false);
        }

        [Test]
        public void ProcSer_Deserialize()
        {
            var ssig = new SerializedSignature
            {
            };
            Given_ProcedureSerializer("__cdecl");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("void test()", sig.ToString("test"));
        }

        [Test]
        public void ProcSer_DeserializeFpuStackReturnValue_function()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                ReturnValue = new Argument_v1
                {
                    Type = new TypeReference_v1("double"),
                    Kind = new FpuStackVariable_v1 { ByteSize = 8 },
                }
            };
            Given_ProcedureSerializer("stdapi");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(1, sig.FpuStackDelta);
        }

        [Test(Description = "Do not overrride user-defined stack delta")]
        public void ProcSer_Load_UserDefinedStackDelta()
        {
            var ssig = new SerializedSignature
            {
                StackDelta = 12,
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "arg",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    }
                }
            };
            Given_ProcedureSerializer("__cdecl");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void foo(Stack int32 arg)
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void ProcSer_Deserialize_thiscall_function()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__thiscall",
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "self",
                        Type = new PointerType_v1 { DataType = PrimitiveType_v1.Int32(), PointerSize = 4 }
                    },
                    new Argument_v1
                    {
                        Name = "arg1",
                        Type = new PointerType_v1 { DataType = PrimitiveType_v1.Int32(), PointerSize = 4 }
                    },
                }
            };
            Given_ProcedureSerializer("__thiscall");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void foo(Register (ptr32 int32) self, Stack (ptr32 int32) arg1)
// stackDelta: 8; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void ProcSer_Deserialize_thiscall_method()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__thiscall",
                EnclosingType = new TypeReference_v1 { TypeName = "bob" },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "arg0",
                        Type = new PointerType_v1 { DataType = PrimitiveType_v1.Int32(), PointerSize = 4 }
                    },
                    new Argument_v1
                    {
                        Name = "arg1",
                        Type = new PointerType_v1 { DataType = PrimitiveType_v1.Int32(), PointerSize = 4 }
                    },
                }
            };
            Given_ProcedureSerializer("__thiscall");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void foo(Register (ptr32 bob) this, Stack (ptr32 int32) arg0, Stack (ptr32 int32) arg1)
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void ProcSer_Deserialize_thiscall_method_noargs()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__thiscall",
                EnclosingType = new TypeReference_v1 { TypeName = "bob" },
                ReturnValue = new Argument_v1 { Type = new VoidType_v1() },
                Arguments = new Argument_v1[0]
            };
            Given_ProcedureSerializer("__thiscall");

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void foo(Register (ptr32 bob) this)
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }

    }
}
