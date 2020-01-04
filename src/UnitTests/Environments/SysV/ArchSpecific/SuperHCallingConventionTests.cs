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
using Reko.Arch.SuperH;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class SuperHCallingConventionTests
    {
        private SuperHArchitecture arch;
        private SuperHCallingConvention cc;
        private CallingConventionEmitter ccr;
        private PrimitiveType c = PrimitiveType.Char;
        private PrimitiveType s = PrimitiveType.Int16;
        private PrimitiveType i = PrimitiveType.Int32;
        private PrimitiveType l = PrimitiveType.Int64;
        private PrimitiveType f = PrimitiveType.Real32;
        private PrimitiveType d = PrimitiveType.Real64;
        private StructureType str = new StructureType
        {
            Size = 8,
            Fields =
            {
                { 0, PrimitiveType.Int32 },
                { 4, PrimitiveType.Int32 }
            }
        };


        public SuperHCallingConventionTests()
        {
            this.arch = new SuperHLeArchitecture("superH-le");
        }

        private void Given_CallingConvention()
        {
            this.cc = new SuperHCallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test]
        public void SHCC_csii()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.Int32, null, new List<DataType> { c, s, i, i });
            Assert.AreEqual("Stk: 0 r0 (r4, r5, r6, r7)", ccr.ToString());
        }


        [Test]
        public void SHCC_csiic()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.Real32, null, new List<DataType> { c, s, i, i, c });
            Assert.AreEqual("Stk: 0 fr0 (r4, r5, r6, r7, Stack +0014)", ccr.ToString());
        }

        [Test]
        public void SHCC_struct()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.Int32, null, new List<DataType> { i, str, i });
            Assert.AreEqual("Stk: 0 r0 (r4, Stack +0014, r5)", ccr.ToString());
        }

        [Test]
        public void SHCC_cfsfd()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.Int32, null, new List<DataType> { c, f, s, f, d });
            Assert.AreEqual("Stk: 0 r0 (r4, fr4, r5, fr5, dr6)", ccr.ToString());
        }
    }
}
