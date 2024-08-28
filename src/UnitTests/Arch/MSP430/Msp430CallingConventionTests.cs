#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Msp430;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Policy;

#if NYI
namespace Reko.UnitTests.Arch.MSP430
{
    [TestFixture]
    public class Msp430CallingConventionTests
    {
        private static readonly PrimitiveType i16 = PrimitiveType.Int16;
        private static readonly PrimitiveType i32 = PrimitiveType.Int32;

        private Msp430Architecture arch;
        private Msp430CallingConvention cc;
        private CallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            Given_Arch();
        }

        private void Given_Arch()
        {
            this.arch = new Msp430Architecture(
                new ServiceContainer(),
                "msp430",
                new Dictionary<string, object>());
            this.cc = new Msp430CallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void Msp430Cc_SingleArg()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i16 });
            Assert.AreEqual("Stk: 0 void (r12)", ccr.ToString());
        }

        [Test]
        public void Msp430Cc_Single_int32_Arg()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (Sequence r13:r12)", ccr.ToString());
        }

        // 32-bit argument may be split between the stack and memory.If an argument
        // would be passed in a register pair, but only one register is available (always R15),
        // the compiler will split the
        // argument between R15 and one register-sized spot on the stack.
        [Test]
        public void Msp430Cc_Split_pair()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i16, i32, i32 });
            Assert.AreEqual("Stk: 0 void (r12, Sequence r14:r13, Sequence stack:r15)", ccr.ToString());
        }
    }
}
#endif