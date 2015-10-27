#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using NUnit.Framework;
using System;
using System.Xml;
using System.Xml.Serialization;
using Reko.Environments.Windows;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86ProcedureSerializerTests
    {
        private IntelArchitecture arch;
        private X86ProcedureSerializer ser;
        private Win32Platform platform;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            platform = new Win32Platform(null, arch);
        }

        private void Given_ProcedureSerializer(string cConvention)
        {
            this.ser = new X86ProcedureSerializer(arch, new TypeLibraryLoader(platform, true), cConvention);
        }

        [Test]
        public void Test()
        {
            Given_ProcedureSerializer("stdapi");
            ProcedureSignature sig = new ProcedureSignature(
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
        public void SsigSerializeAxBxCl()
        {
            Given_ProcedureSerializer("stdapi");
            SerializedSignature ssig = ser.Serialize(SerializedSignatureTests.MkSigAxBxCl());
            Verify(ssig, "Core/SsigSerializeAxBxCl.txt");
        }

        [Test]
        public void SsigSerializeSequence()
        {
            Identifier seq = new Identifier("es_bx", PrimitiveType.Word32, new SequenceStorage(
                new Identifier(Registers.es.Name, Registers.es.DataType, Registers.es),
                new Identifier(Registers.bx.Name, Registers.bx.DataType, Registers.bx)));
            Given_ProcedureSerializer("stdapi");
            SerializedSignature ssig = ser.Serialize(new ProcedureSignature(seq, new Identifier[0]));
            Verify(ssig, "Core/SsigSerializeSequence.txt");

        }

        [Test]
        public void SerializeProcedure()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame());
            Address addr = Address.Ptr32(0x12345);
            Given_ProcedureSerializer("stdapi");
            Procedure_v1 sproc =  ser.Serialize(proc, addr);
            Assert.AreEqual("foo", sproc.Name);
            Assert.AreEqual("00012345", sproc.Address);
        }

        [Test]
        public void SerializeProcedureWithSignature()
        {
            Procedure proc = new Procedure("foo", arch.CreateFrame())
            {
                Signature = new ProcedureSignature(
                    new Identifier("eax", PrimitiveType.Word32, Registers.eax),
                    new Identifier[] {
                        new Identifier("arg00", PrimitiveType.Word32, new StackArgumentStorage(0, PrimitiveType.Word32))
                    })
            };
            
            Address addr = Address.Ptr32(0x567A0C);
            Given_ProcedureSerializer("stdapi");
            Procedure_v1 sproc = ser.Serialize(proc, addr);
            Assert.AreEqual("eax", sproc.Signature.ReturnValue.Name);
        }

        [Test]
        public void DeserializeFpuStackargument()
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
            return new SerializedTypeReference(typeName);
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
                    Type = new SerializedTypeReference("double"),
                    Kind = new FpuStackVariable_v1 { ByteSize = 8 },
                }
            };
            Given_ProcedureSerializer("stdapi");
            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(1, sig.FpuStackDelta);
        }

        [Test]
        public void ProcSer_Deserialize_thiscall()
        {
            var ssig = new SerializedSignature
            {
                Convention = "__thiscall",
                Arguments = new Argument_v1[] {
                    new Argument_v1 
                    {
                        Type = new SerializedTypeReference("int"),
                        Name = "this"
                    }
                }
            };

            Given_ProcedureSerializer("stdcall");
            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual("ecx", sig.Parameters[0].ToString());
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
            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(4, sig.StackDelta);
        }

        [Test]
        public void ProcSer_Load_stdcall()
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
            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(8, sig.StackDelta);
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
            var sig = ser.Deserialize(ssig, arch.CreateFrame());
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
            Assert.AreEqual(0, ((StackArgumentStorage)sig.Parameters[1].Storage).StackOffset);
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
    }
}
