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

using NUnit.Framework;
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System.Linq;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class MipsCallingConventionTests
    {
        private readonly PrimitiveType r64 = PrimitiveType.Real64;
        private readonly PrimitiveType r32 = PrimitiveType.Real32;
        private readonly PrimitiveType i32 = PrimitiveType.Int32;
        private readonly PrimitiveType i64 = PrimitiveType.Int64;

        private void AssertSignature(string sExp, params DataType[] args)
        {
            var arch = new MipsBe32Architecture("mips-be-32");
            var cc = new MipsCallingConvention(arch);
            var ccr = new CallingConventionEmitter();
            cc.Generate(ccr, null, null, args.ToList());
            Assert.AreEqual(sExp.Trim(), ccr.ToString());
        }

        private void AssertSignature64(string sExp, params DataType[] args)
        {
            var arch = new MipsBe64Architecture("mips-be-64");
            var cc = new MipsCallingConvention(arch);
            var ccr = new CallingConventionEmitter();
            cc.Generate(ccr, null, null, args.ToList());
            Assert.AreEqual(sExp.Trim(), ccr.ToString());
        }

        [Test]
        public void MipsCc_Regular()
        {
            //$TODO: there doesn't seem to be a consistent
            // piece of documentation for MIPS ABI....
            AssertSignature("Stk: 0 void (Sequence f12:f13, Sequence f14:f15) ", r64, r64);
            AssertSignature("Stk: 0 void (f12, f14)                           ", r32, r32);
            AssertSignature("Stk: 0 void (f12, Sequence f14:f15)              ", r32, r64);
            AssertSignature("Stk: 0 void (Sequence f12:f13, f14)              ", r64, r32);
            AssertSignature("Stk: 0 void (r4, r5, r6, r7)                     ", i32, i32, i32, i32);
            AssertSignature("Stk: 0 void (Sequence f12:f13, r6, Stack +0010)  ", r64, i32, r64);
            AssertSignature("Stk: 0 void (Sequence f12:f13, r6, r7)           ", r64, i32, i32);
            AssertSignature("Stk: 0 void (f12, r5, r6)                        ", r32, i32, i32);
            AssertSignature("Stk: 0 void (r4, r5, r6, Stack +0010)            ", i32, i32, i32, r64);
            AssertSignature("Stk: 0 void (r4, r5, r6, r7)                     ", i32, i32, i32, r32);
            AssertSignature("Stk: 0 void (r4, r5, Sequence r6:r7)             ", i32, i32, r64);
            AssertSignature("Stk: 0 void (r4, Sequence r6:r7)                 ", i32, r64);
            //AssertSignature("f12, f14, r6, r7                               ", r32, r32, r32, s4);
            //AssertSignature("f12, r5, r6, r7                                ", r32, i32, r32, i32);
            //AssertSignature("f12, f14, r6                                   ", r64, r32, r32);
            //AssertSignature("f12, f14, (r6, r7)                             ", r32, r32, r64);
            AssertSignature("Stk: 0 void (r4, r5, r6, r7)                     ", i32, r32, i32, r32);
            AssertSignature("Stk: 0 void (r4, r5, r6, r7)                     ", i32, r32, i32, i32);
            AssertSignature("Stk: 0 void (r4, r5, r6, r7)                     ", i32, i32, r32, i32);
            AssertSignature("Stk: 0 void (r4, Sequence r6:r7, Stack +0010)    ", i32, r64, r64);
            AssertSignature("Stk: 0 void (f12, r5)                            ", r32, i32);
            AssertSignature("Stk: 0 void (f12, r5, Sequence f14:f15)          ", r32, i32, r64);
            AssertSignature("Stk: 0 void (Sequence f12:f13, r6, Stack +0010)  ", r64, i32, r64);
        }

        [Test]
        public void MipsCc_64_bit()
        {
            AssertSignature64("Stk: 0 void (r4, r5)                 ", i64, r64);
        }
    }
}
