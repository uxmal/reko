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
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class ProcedureFlowTests
    {
        [Test]
        public void Pflow_IntersectBinding()
        {
            var reg = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
            var id = new Identifier("r1", reg.DataType, reg);
            var cbs = new[]
            {
                new CallBinding(reg, id)
            };
            var uses = new Dictionary<Storage, BitRange>
            {
                {
                    reg,
                    new BitRange(0, 31)
                }
            };
            var bindings = ProcedureFlow.IntersectCallBindingsWithUses(cbs, uses)
                .ToArray();
            Assert.AreEqual(1, bindings.Length);
            Assert.AreEqual("r1:r1", bindings[0].ToString());
        }

        [Test]
        public void Pflow_IntersectBinding_WiderRegisterInCallBinding()
        {
            var regCaller = new RegisterStorage("ebx", 1, 0, PrimitiveType.Word32);
            var regCallee = new RegisterStorage("bx", 1, 0, PrimitiveType.Word16);
            var idCaller = new Identifier("ebx", regCaller.DataType, regCaller);
            var cbs = new[]
            {
                new CallBinding(regCaller, idCaller)
            };
            var uses = new Dictionary<Storage, BitRange>
            {
                {
                    regCallee,
                    new BitRange(0, 16)
                }
            };
            var bindings = ProcedureFlow.IntersectCallBindingsWithUses(cbs, uses)
                .ToArray();
            Assert.AreEqual(1, bindings.Length);
            Assert.AreEqual("bx:SLICE(ebx, word16, 0)", bindings[0].ToString());
        }

        [Test]
        public void Pflow_IntersectBinding_StackArgument_ExactMatch()
        {
            var stCallee = new StackArgumentStorage(4, PrimitiveType.Word32);
            var stCaller = new StackArgumentStorage(4, PrimitiveType.Word32);
            var idCaller = new Identifier("local", stCaller.DataType, stCaller);
            var cbs = new[]
            {
                new CallBinding(stCaller, idCaller)
            };
            var uses = new Dictionary<Storage, BitRange>
            {
                { stCallee, new BitRange(0, 32) }
            };
            var bindings = ProcedureFlow.IntersectCallBindingsWithUses(cbs, uses)
                .ToArray();
            Assert.AreEqual(1, bindings.Length);
            Assert.AreEqual("Stack +0004:local", bindings[0].ToString());
        }

        [Test]
        public void Pflow_IntersectBinding_NotFoundUses()
        {
            var reg = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
            var stCallee = new StackArgumentStorage(4, PrimitiveType.Word32);
            var id = new Identifier("r1", reg.DataType, reg);
            var cbs = new CallBinding[] { } ;
            var uses = new Dictionary<Storage, BitRange>
            {
                {
                    reg,
                    new BitRange(0, 31)
                },
                {
                    stCallee,
                    new BitRange(0, 31)
                },
            };

            var bindings = ProcedureFlow.IntersectCallBindingsWithUses(cbs, uses)
                .ToArray();

            Assert.AreEqual(0, bindings.Length);
        }
    }
} 
