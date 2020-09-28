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
using Reko.Arch.PowerPC;
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
using PowerPcCallingConvention = Reko.Arch.PowerPC.PowerPcCallingConvention;
using System.ComponentModel.Design;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcCallingConventionTests
    {
        private PowerPcArchitecture arch;
        private PowerPcCallingConvention cc;
        private PrimitiveType i16 = PrimitiveType.Int16;
        private PrimitiveType i32 = PrimitiveType.Int32;
        private PrimitiveType i64 = PrimitiveType.Int64;
        private PrimitiveType r64 = PrimitiveType.Real64;
        private ICallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            arch = new PowerPcBe32Architecture("ppc-be-32");
        }

        private void Given_CallingConvention()
        {
            this.cc = new PowerPcCallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void PpcCc_DeserializeFpuArgument()
        {
            Given_CallingConvention();
            cc.Generate(ccr, i32, null, new List<DataType> { r64 });
            Assert.AreEqual("Stk: 0 r3 (f1)", ccr.ToString());
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
        public void PpcCc_DeserializeFpuStackReturnValue()
        {
            Given_CallingConvention();
            cc.Generate(ccr, r64, null, new List<DataType>());
            Assert.AreEqual("Stk: 0 f1 ()", ccr.ToString());
        }

        [Test]
        public void PpcCc_Load_cdecl()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (r3)", ccr.ToString());
        }

        [Test]
        public void PpcCc_Load_LongArg()
        {
            Given_CallingConvention();
            cc.Generate(ccr, null, null, new List<DataType> { i16, i64 });
            Assert.AreEqual("Stk: 0 void (r3, Sequence r5:r6)", ccr.ToString());
        }
    }
}
