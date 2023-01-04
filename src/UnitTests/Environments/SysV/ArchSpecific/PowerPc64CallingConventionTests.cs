#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    class PowerPc64CallingConventionTests
    {
        private readonly PrimitiveType r64 = PrimitiveType.Real64;
        private readonly PrimitiveType r32 = PrimitiveType.Real32;
        private readonly PrimitiveType i32 = PrimitiveType.Int32;
        private readonly PrimitiveType i64 = PrimitiveType.Int64;
        private readonly DataType Void = null;

        private void AssertSignature(string sExp, DataType retType, params DataType[] args)
        {
            var arch = new PowerPcBe64Architecture(new ServiceContainer(), "ppc-be-64", new Dictionary<string, object>());
            var cc = new PowerPc64CallingConvention(arch);
            var ccr = new CallingConventionEmitter();
            cc.Generate(ccr, 0, retType, null, args.ToList());
            Assert.AreEqual(sExp.Trim(), ccr.ToString());
        }

        [Test]
        public void Ppc64cc_SimpleFns()
        {
            AssertSignature("Stk: 0 void ()", Void);
            AssertSignature("Stk: 0 void (r3)", Void, i32);
            AssertSignature("Stk: 0 void (r3, f1)", Void, i64, r32);
            AssertSignature("Stk: 0 r3 (r3, f1)", i32, i64, r32);
            AssertSignature("Stk: 0 f1 (r3)", r32, i64);
        }

        [Test]
        public void Ppc64cc_LongArgumentList()
        {
            AssertSignature("Stk: 0 r3 (r3, r4, r5, r6, r7, r8, r9, r10, Stack +0048)", i32, i64, i64, i64, i64, i64, i64, i64, i64, new Pointer(i32, 64));
        }
    }
}
