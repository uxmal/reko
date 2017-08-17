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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Core.Serialization;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Xml;
using System.Xml.Serialization;
using Reko.Environments.Windows;
using Rhino.Mocks;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class X86ProcedureSerializerTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private IntelArchitecture arch;
        private X86ProcedureSerializer ser;
        private Win32Platform platform;
        private ISerializedTypeVisitor<DataType> deserializer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            arch = new X86ArchitectureFlat32();
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
            this.deserializer = new FakeTypeDeserializer(4);
            this.ser = new X86ProcedureSerializer(arch, deserializer, cConvention);
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
            Register_v1 sreg = (Register_v1) ssig.ReturnValue.Kind;
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
                new SequenceStorage(Registers.es, Registers.bx));
            Given_ProcedureSerializer("stdapi");
            mr.ReplayAll();

            SerializedSignature ssig = ser.Serialize(new FunctionType(seq, new Identifier[0]));
            Verify(ssig, "Core/SsigSerializeSequence.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86ps_SerializeProcedure()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame());
            Address addr = Address.Ptr32(0x12345);
            Given_ProcedureSerializer("stdapi");
            mr.ReplayAll();

            Procedure_v1 sproc =  ser.Serialize(proc, addr);
            Assert.AreEqual("foo", sproc.Name);
            Assert.AreEqual("00012345", sproc.Address);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void X86ps_ProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new FunctionType(
                    new Identifier("eax", PrimitiveType.Word32, Registers.eax),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, new StackArgumentStorage(0, PrimitiveType.Word32))
                    })
            };
            
            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer("stdapi");
            mr.ReplayAll();

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
            mr.ReplayAll();

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
        public void ProcSer_DeserializeFpuStackReturnValue()
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
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(1, sig.FpuStackDelta);
        }

        [Test]
        public void X86ProcSer_Deserialize_thiscall()
        {
            var ssig = new SerializedSignature
            {
                EnclosingType = new StructType_v1 { Name = "CHandle" },
                Convention = "__thiscall",
                Arguments = new Argument_v1[] {
                    new Argument_v1 
                    {
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                        Name = "foo"
                    }
                }
            };

            Given_ProcedureSerializer("stdcall");
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(2, sig.Parameters.Length);
            Assert.AreEqual("this", sig.Parameters[0].ToString());
            Assert.AreEqual("ecx", sig.Parameters[0].Storage.ToString());
            Assert.AreEqual(8, sig.StackDelta);
        }

        [Test]
        public void ProcSer_Load_cdecl()
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
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(4, sig.StackDelta);
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
        }

        [Test]
        public void ProcSer_Load_stdapi()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdapi",
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    }
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(8, sig.StackDelta);
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
        }

        [Test]
        public void ProcSer_Load_stdcall()
        {
            var ssig = new SerializedSignature
            {
                Convention = "stdcall",
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    }
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(8, sig.StackDelta);
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
        }

        [Test]
        public void ProcSer_Load___stdcall()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__stdcall",
                Arguments = new Argument_v1[] {
                    new Argument_v1
                    {
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    }
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(8, sig.StackDelta);
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
        }

        [Test]
        public void ProcSer_Load_pascal()
        {
            var ssig = new SerializedSignature
            {
                Convention = "pascal",
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "arg1",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize= 2 },
                    },
                    new Argument_v1
                    {
                        Name = "arg2",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize=4  }
                    }
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(8, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[1].Storage).StackOffset);
        }

        [Test]
        public void X86ProcSer_Load_thiscall()
        {
            var ssig = new SerializedSignature
            {
                EnclosingType = new StructType_v1 { Name="CWindow" },
                Convention = "__thiscall",
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "XX",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize= 4 },
                    },
                    new Argument_v1
                    {
                        Name = "arg1",
                        Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize= 2 },
                    },
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void memfn(Register (ptr (struct ""CWindow"")) this, Stack int32 XX, Stack int16 arg1)
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("memfn", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[1].Storage).StackOffset);
            Assert.AreEqual(8, ((StackArgumentStorage)sig.Parameters[2].Storage).StackOffset);
        }

        [Test(Description = "Ensure FPU stack effects are accounted for when returning floats")]
        public void X86ProcSer_Load_FpuReturnValue()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__cdecl",
                ReturnValue = new Argument_v1
                {
                    Type = PrimitiveType_v1.Real64()
                }
            };
            Given_ProcedureSerializer(ssig.Convention);
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"FpuStack real64 test()
// stackDelta: 4; fpuStackDelta: 1; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("test", FunctionType.EmitFlags.AllDetails));
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
            mr.ReplayAll();

            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            var sExp =
@"void foo(Stack int32 arg)
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("foo", FunctionType.EmitFlags.AllDetails));
        }
    }
}
