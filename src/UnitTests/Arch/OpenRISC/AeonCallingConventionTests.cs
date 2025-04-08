#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.OpenRISC;
using Reko.Arch.OpenRISC.Aeon;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.OpenRISC
{
    public class AeonCallingConventionTests
    {

        private static PrimitiveType i32 = PrimitiveType.Int32;
        private static PrimitiveType i64 = PrimitiveType.Int64;
        private static PrimitiveType r32 = PrimitiveType.Real32;
        private static PrimitiveType r64 = PrimitiveType.Real64;
        private static StructureType smallStruct = new StructureType(6);
        private static StructureType bigStruct = new StructureType(12);

        private readonly AeonArchitecture arch = new AeonArchitecture(new ServiceContainer(), "aeon", new());
        private AeonCallingConvention cc;
        private ICallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            this.cc = new AeonCallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        /*
         Functions receive their first 6 argument words in general-purpose function argument registers r3-r8. If there
are more than six argument words, the remaining argument words are passed on the stack. Small structure
and union arguments (not more than 64 bits in total) are passed in argument registers, other structure and
union arguments are passed as pointers to memory locations that contain the arguments itself. All 64-bit
arguments in a 32-bit system are passed using a pair of registers. 64-bit argument need not to be aligned on
32-bit boundaries.
For example long long arg1, long arg2, long long arg3, long long arg4 are to be passed in the following
way: arg1 in r3 and r4, arg2 in r5, arg3 in r6 and r7. Argument 4 has to be split between r8 and stack, and
the receiving function should reconstruct the passed value.
        */

        [Test]
        public void AeonCc_SingleArg()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i32 });
            Assert.AreEqual("Stk: 0 void (r3)", ccr.ToString());
        }

        [Test]
        public void AeonCc_ReturnInt()
        {
            cc.Generate(ccr, 0, i32, null, new List<DataType> { r64 });
            Assert.AreEqual("Stk: 0 r3 (Sequence r3:r4)", ccr.ToString());
        }

        [Test]
        public void AeonCc_ReturnInt64()
        {
            cc.Generate(ccr, 0, i64, null, new List<DataType> { r64 });
            Assert.AreEqual("Stk: 0 Sequence r3:r4 (Sequence r3:r4)", ccr.ToString());
        }


        [Test]
        public void AeonCc_ArgsOnStack()
        {
            cc.Generate(ccr, 0, null, null,
                new List<DataType> { i32, i32, i32, i32, i32, i32, i32 });

            Assert.AreEqual(
                "Stk: 0 void (r3, " +
                             "r4, " +
                             "r5, " +
                             "r6, " +
                             "r7, " +
                             "r8, " + 
                             "Stack +0018)",
                ccr.ToString());
        }

        [Test]
        public void AeonCc_regs_and_stack()
        {
            cc.Generate(ccr, 0, null, null, new List<DataType> { i64, i32, i64, i64 });
            Assert.AreEqual(
                "Stk: 0 void (Sequence r3:r4, r5, Sequence r6:r7, Sequence stack:r8)",
                ccr.ToString());

        }
    }
}
