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
using Reko.Arch.Mips;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Rhino.Mocks;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class MipsProcedureSerializerTests
    {
        protected readonly PrimitiveType_v1 Int32 = new PrimitiveType_v1(Domain.Integer, 4);
        protected readonly VoidType_v1 Void = new VoidType_v1();

        private MockRepository mr;
        private FunctionType sig;
        private SerializedSignature ssig;
        private MipsLe32Architecture arch;
        private ISerializedTypeVisitor<DataType> typeLoader;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = new MipsLe32Architecture();
            this.typeLoader = mr.Stub<ISerializedTypeVisitor<DataType>>();
            this.ssig = null;
            this.sig = null;
        }

        private void Given_Sig(SerializedType ret, params Argument_v1 [] args)
        {
            ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = ret }
            };
        }

        private void Given_Sig(params Argument_v1 [] args)
        {
            ssig = new SerializedSignature
            {
                Arguments = args
            };
        }

        private Argument_v1 Arg(string argName, SerializedType sType)
        {
            return new Argument_v1(argName, sType, null, false);
        }

        private PointerType_v1 Ptr(SerializedType sType)
        {
            return new PointerType_v1 { DataType = sType, PointerSize = 4 };
        }

        private void When_DeserializeSignature()
        {
            var mps = new MipsProcedureSerializer(arch, typeLoader, "");
            this.sig = mps.Deserialize(ssig, arch.CreateFrame());
        }

        [Test]
        public void MipsProcSer_ReturnRegister()
        {
            Given_Sig(Int32);
            typeLoader.Stub(t => t.VisitPrimitive(null)).IgnoreArguments().Return(PrimitiveType.Int32);
            mr.ReplayAll();

            When_DeserializeSignature();

            Assert.AreEqual("r2", sig.ReturnValue.Storage.ToString());
        }

        [Test]
        public void MipsProcSet_CharArg()
        {
            Given_Sig(Arg("mem", Ptr(Void)));
            typeLoader.Stub(t => t.VisitPointer(null)).IgnoreArguments().Return(new Pointer(VoidType.Instance, 4));
            mr.ReplayAll();

            When_DeserializeSignature();

            Assert.AreEqual("r4", sig.Parameters[0].Storage.ToString());
            Assert.AreEqual("(ptr void)", sig.Parameters[0].DataType.ToString());
        }

        [Test]
        public void MipsProcSet_ManyArgs()
        {
            Given_Sig(
                Arg("reg1", Ptr(Void)),
                Arg("reg2", Ptr(Void)),
                Arg("reg3", Ptr(Void)),
                Arg("reg4", Ptr(Void)),
                Arg("stk5", Ptr(Void)));
            typeLoader.Stub(t => t.VisitPointer(null)).IgnoreArguments().Return(new Pointer(VoidType.Instance, 4));
            mr.ReplayAll();

            When_DeserializeSignature();

            Assert.AreEqual("r4", sig.Parameters[0].Storage.ToString());
            Assert.AreEqual("r5", sig.Parameters[1].Storage.ToString());
            Assert.AreEqual("r6", sig.Parameters[2].Storage.ToString());
            Assert.AreEqual("r7", sig.Parameters[3].Storage.ToString());
            Assert.AreEqual("Stack +0000", sig.Parameters[4].Storage.ToString());
        }
    }
}
