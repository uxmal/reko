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
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class X86CallingConventionTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private IntelArchitecture arch;
        private X86CallingConvention cc;
        private Win32Platform platform;
        private ISerializedTypeVisitor<DataType> deserializer;
        private PrimitiveType i16 = PrimitiveType.Int16;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType r64 = PrimitiveType.Real64;

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
            X86CallingConvention cc;
            switch (cConvention)
            {
            case "stdapi":
                cc = new X86CallingConvention(4, 4, 4, false, false);

                break;
            default: throw new NotImplementedException();
            }
            this.cc = cc;
        }

        [Test]
        [Category(Categories.UnitTests)]
        [Ignore("This ia custom arg scenario")]
        public void X86ps_DeserializeFpuStackargument()
        {
            throw new NotImplementedException();
            /*
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

            var ccr = cc.Generate(ssig, arch.CreateFrame());
            Assert.AreEqual("Register int test(FpuStack real64 fpArg0)", ccr.ToString());
            */
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
        [Ignore("Wait a while")]
        public void X86ProcSer_Deserialize_thiscall()
        {
            throw new NotImplementedException();
            /*
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

            var sig = cc.Generate()
            Assert.AreEqual(2, sig.Parameters.Length);
            Assert.AreEqual("this", sig.Parameters[0].ToString());
            Assert.AreEqual("ecx", sig.Parameters[0].Storage.ToString());
            Assert.AreEqual(8, sig.StackDelta);*/
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
            Given_ProcedureSerializer("__cdecl");
            mr.ReplayAll();

            var ccr = cc.Generate(null, null, new List<DataType> { i32 });
            Assert.AreEqual(4, ccr.StackDelta);
            Assert.AreEqual(4, ((StackArgumentStorage)ccr.Parameters[0]).StackOffset);
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
            Given_ProcedureSerializer("stdapi");
            var ccr = cc.Generate(null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004", ccr.ToString());
            //Assert.AreEqual(8, sig.StackDelta);
            //Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
        }

        [Test]
        public void ProcSer_Load_stdcall()
        {
            Given_ProcedureSerializer("stdcall");
            var ccr = cc.Generate(null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004", ccr.ToString());
        }

        [Test]
        public void ProcSer_Load___stdcall()
        {
            Given_ProcedureSerializer("stdcall");
            var ccr = cc.Generate(null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004", ccr.ToString());
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
            Given_ProcedureSerializer("pascal");
            var ccr = cc.Generate(null, null, new List<DataType> { i16, i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004", ccr.ToString());
            //Assert.AreEqual(8, ((StackArgumentStorage)sig.Parameters[0].Storage).StackOffset);
            //Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[1].Storage).StackOffset);
        }

        [Test]
        [Ignore("Rethink __thiscall")]
        public void X86ProcSer_Load_thiscall()
        {
            throw new NotImplementedException();
            /*
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

            var sig = cc.Generate(ssig, arch.CreateFrame());
            var sExp =
@"void memfn(Register (ptr (struct ""CWindow"")) this, Stack int32 XX, Stack int16 arg1)
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, sig.ToString("memfn", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(4, ((StackArgumentStorage)sig.Parameters[1].Storage).StackOffset);
            Assert.AreEqual(8, ((StackArgumentStorage)sig.Parameters[2].Storage).StackOffset);
            */
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

            var ccr = cc.Generate(r64, null, new List<DataType> ());
            var sExp =
@"FpuStack real64 test()
// stackDelta: 4; fpuStackDelta: 1; fpuMaxParam: -1
";
            Assert.AreEqual(sExp, ccr.ToString());
        }

  
    }
}
