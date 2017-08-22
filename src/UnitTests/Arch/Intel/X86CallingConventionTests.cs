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
        private ICallingConventionEmitter ccr;
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

        private void Given_32bit_CallingConvention(string cConvention)
        {
            this.ccr = new CallingConventionEmitter();
            this.deserializer = new FakeTypeDeserializer(4);
            X86CallingConvention cc;
            switch (cConvention)
            {
            case "__cdecl":
                cc = new X86CallingConvention(4, 4, 4, true, false);
                break;
            case "stdapi":
            case "stdcall":
            case "__stdcall":
                cc = new X86CallingConvention(4, 4, 4, false, false);
                break;
            case "pascal":
                cc = new X86CallingConvention(4, 4, 4, false, true);
                break;
            default: throw new NotImplementedException(cConvention + " not supported.");
            }
            this.cc = cc;
        }

        private void Given_16bit_CallingConvention(string cConvention)
        {
            this.deserializer = new FakeTypeDeserializer(4);
            X86CallingConvention cc;
            switch (cConvention)
            {
            case "__cdecl":
                cc = new X86CallingConvention(4, 2, 4, true, false);
                break;
            case "stdapi":
            case "stdcall":
            case "__stdcall":
                cc = new X86CallingConvention(4, 2, 4, false, false);
                break;
            case "pascal":
                cc = new X86CallingConvention(4, 2, 4, false, true);
                break;
            default: throw new NotImplementedException(cConvention + " not supported.");
            }
            this.cc = cc;
        }

        [Test]
        [Ignore("Wait a while with __thiscall")]
        public void X86Cc_Deserialize_thiscall()
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
        public void X86Cc_Load_cdecl()
        {
            Given_32bit_CallingConvention("__cdecl");
            cc.Generate(ccr, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 4 void (Stack +0004)", ccr.ToString());
        }

        [Test]
        public void X86Cc_Load_stdapi()
        {
            Given_32bit_CallingConvention("stdapi");
            cc.Generate(ccr, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004)", ccr.ToString());
        }

        [Test]
        public void X86Cc_Load_stdcall()
        {
            Given_32bit_CallingConvention("stdcall");
            cc.Generate(ccr, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004)", ccr.ToString());
        }

        [Test]
        public void X86Cc_Load___stdcall()
        {
            Given_32bit_CallingConvention("stdcall");
            cc.Generate(ccr, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 8 void (Stack +0004)", ccr.ToString());
        }

        [Test]
        public void X86Cc_Load_pascal()
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
            Given_32bit_CallingConvention("pascal");
            cc.Generate(ccr, null, null, new List<DataType> { i16, i32 });
            Assert.AreEqual("Stk: 12 void (Stack +0008, Stack +0004)", ccr.ToString());
        }

        [Test]
        [Ignore("Rethink __thiscall")]
        public void X86Cc_Load_thiscall()
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
        public void X86Cc_Load_FpuReturnValue()
        {
            Given_32bit_CallingConvention("__cdecl");
            cc.Generate(ccr, r64, null, new List<DataType> ());
            Assert.AreEqual("Stk: 4 Fpu: 1 FPU stack ()", ccr.ToString());
        }

        [Test]
        public void X86Cc_Return_bool()
        {
            var stg = X86CallingConvention.GetReturnStorage(PrimitiveType.Bool, 2);
            Assert.AreEqual("al", stg.ToString());
        }

        [Test]
        public void X86Cc_Return_16bit_long()
        {
            var stg = X86CallingConvention.GetReturnStorage(PrimitiveType.Int32, 2);
            Assert.AreEqual("Sequence dx:ax", stg.ToString());
        }

        [Test]
        public void X86Cc_Return_32bit_long()
        {
            var stg = X86CallingConvention.GetReturnStorage(PrimitiveType.Int32, 4);
            Assert.AreEqual("eax", stg.ToString());
        }

        [Test]
        public void X86Cc_Return_64bit_long()
        {
            var stg = X86CallingConvention.GetReturnStorage(PrimitiveType.UInt64, 4);
            Assert.AreEqual("Sequence edx:eax", stg.ToString());
        }
    }
}

