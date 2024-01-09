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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class BlockConstantPropagatorTests
    {
        private IReadOnlySegmentMap segmentMap;
        private List<RtlInstruction> instrs;
        private List<RtlInstruction> result;
        private readonly Identifier r2;
        private readonly Identifier r3;
        private readonly Identifier C;
        private readonly Identifier CZ;

        public BlockConstantPropagatorTests()
        {
            var sr = RegisterStorage.Reg16("sr", 0x42);
            r2 = Identifier.Create(RegisterStorage.Reg16("r2", 2));
            r3 = Identifier.Create(RegisterStorage.Reg16("r3", 2));

            C = Identifier.Create(new FlagGroupStorage(sr, 0x1, "C", PrimitiveType.Bool));
            CZ = Identifier.Create(new FlagGroupStorage(sr, 0x3, "CZ", PrimitiveType.Byte));
        }

        [SetUp]
        public void Setup()
        {
            this.instrs = null;
            this.result = null;
            var mem = new ByteMemoryArea(Address.Ptr16(0x1000), new byte[0x100]);
            this.segmentMap = new SegmentMap(
                new ImageSegment(".text", mem, AccessMode.ReadExecute));
        }

        private void When_ConstantsPropagated()
        {
            var bcp = new BlockConstantPropagator(segmentMap, new FakeDecompilerEventListener());
            result = instrs.Select(i => i.Accept(bcp)).ToList();
        }

        private void Given_Block(Action<RtlEmitter> builder)
        {
            instrs = new List<RtlInstruction>();
            var rtlEmitter = new RtlEmitter(instrs);
            builder(rtlEmitter);
        }

        [Test]
        public void Bcp_Copy()
        {
            Given_Block(m =>
            {
                m.Assign(r2, 0x42);
                m.Assign(r3, r2);
            });

            When_ConstantsPropagated();

            Assert.AreEqual("r3 = 0x42<16>", result[1].ToString());
        }

        [Test]
        public void Bcp_BranchCondition()
        {
            Given_Block(m =>
            {
                m.Assign(r2, 0x42);
                m.Branch(m.Uge(r3, r2), Address.Ptr16(0x1234));
            });

            When_ConstantsPropagated();

            Assert.AreEqual("if (r3 >=u 0x42<16>) branch 1234", result[1].ToString());
        }

        [Test]
        public void Bcp_BranchCondition_Mirror()
        {
            Given_Block(m =>
            {
                m.Assign(r2, 0x42);
                m.Branch(m.Uge(r2, r3), Address.Ptr16(0x1234));
            });

            When_ConstantsPropagated();

            Assert.AreEqual("if (r3 <=u 0x42<16>) branch 1234", result[1].ToString());
        }

        [Test]
        public void Bcp_Addition()
        {
            Given_Block(m =>
            {
                m.Assign(r2, 0x1000);
                m.Assign(r3, m.IAdd(r3, r2));
            });

            When_ConstantsPropagated();

            Assert.AreEqual("r3 = r3 + 0x1000<16>", result[1].ToString());
        }

        [Test]
        public void Bcp_Flags()
        {
            Given_Block(m =>
            {
                m.Assign(C, Constant.False());
                m.Assign(CZ, m.Cond(r2));
                m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr16(0x1234));
            });

            When_ConstantsPropagated();

            Assert.AreEqual("if (Test(ULT,C)) branch 1234", result[2].ToString());
        }
    }
}
