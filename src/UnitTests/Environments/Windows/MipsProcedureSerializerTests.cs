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

        private MockRepository mr;
        private ProcedureSignature sig;
        private SerializedSignature ssig;
        private MipsLe32Architecture arch;
        private ISerializedTypeVisitor<DataType> typeLoader;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = new MipsLe32Architecture();
            this.typeLoader = mr.Stub<ISerializedTypeVisitor<DataType>>();
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
            When_DeserializeSignature();
            Assert.AreEqual("r2", sig.ReturnValue.Storage.ToString());
        }
    }
}
