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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.ImageLoaders.OdbgScript;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    [Ignore("Overly destructive")]
    public class SsaEvaluationContextTests
    {
        private readonly IMemory memory;

        public SsaEvaluationContextTests()
        {
            var segmentMap = new SegmentMap(
                new ImageSegment(
                    ".readonly",
                    new ByteMemoryArea(Address.Ptr32(0x0012_3000), new byte[] { 0x78, 0x56, 0x34, 0x12 }),
                    AccessMode.ReadExecute));
            this.memory = new ProgramMemory(segmentMap);
        }

        [Test]
        public void SsaCtx_ReadonlyMemoryAccess_Const()
        {
            var dynLink = new Mock<IDynamicLinker>();
            var m = new SsaProcedureBuilder(nameof(SsaCtx_ReadonlyMemoryAccess_Const));
            
            var ssaCtx = new SsaEvaluationContext(m.Architecture, m.Ssa.Identifiers, dynLink.Object);

            var result = ssaCtx.GetValue(m.Mem32(m.Word32(0x0012_3000)), memory);

            Assert.AreEqual("0x12345678<32>", result.ToString());
        }

        [Test]
        public void SsaCtx_ReadonlyMemoryAccess_Address()
        {
            var dynLink = new Mock<IDynamicLinker>();
            var m = new SsaProcedureBuilder(nameof(SsaCtx_ReadonlyMemoryAccess_Address));

            var ssaCtx = new SsaEvaluationContext(m.Architecture, m.Ssa.Identifiers, dynLink.Object);

            var result = ssaCtx.GetValue(m.Mem32(Address.Ptr32(0x0012_3000)), memory);

            Assert.AreEqual("0x12345678<32>", result.ToString());
        }
    }
}
