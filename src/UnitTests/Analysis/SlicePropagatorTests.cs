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
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class SlicePropagatorTests
    {
        private FakeArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new FakeArchitecture();
        }

        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);
            var slp = new SlicePropagator(m.Ssa, new FakeDecompilerEventListener());
            slp.Transform();
            var sw = new StringWriter();
            m.Ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sActual != sExpected)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            m.Ssa.Validate(s => { m.Ssa.Dump(true); Assert.Fail(s); });
        }

        /// <summary>
        /// Models the pattern seen on RISC architectures, where byte loads are always
        /// extended to a full word, which is then stored in a register.
        /// </summary>
        [Test]
        public void Slp_LinearSlice()
        {
            var sExp =
            #region Expected 
@"// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_4 = Mem1[0x123400<32>:byte]
	Mem2[0x123404<32>:byte] = r1_4
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1 = m.Register(arch.GetRegister("r1"));
                m.Assign(r1, m.Cast(PrimitiveType.UInt32, m.Mem8(m.Word32(0x00123400))));
                m.MStore(m.Word32(0x00123404), m.Slice(PrimitiveType.Byte, r1, 0));
            });
        }

        [Test]
        public void Slp_SliceDef()
        {
            var sExp =
            #region Expected
@"// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	def r1_3
	// succ:  l1
l1:
	Mem1[0x123400<32>:word16] = r1_3
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1 = m.Register(arch.GetRegister("r1"));
                m.AddDefToEntryBlock(r1);
                m.MStore(m.Word32(0x00123400), m.Slice(PrimitiveType.Word16, r1, 0));
            });
        }

        [Test]
        public void Slp_SliceSum()
        {
            var sExp =
            #region Expected
@"// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_4 = Mem1[0x123400<32>:word16]
	Mem2[0x123400<32>:word16] = r1_4 + 4<16>
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1 = m.Register(arch.GetRegister("r1"));
                m.Assign(r1, m.Cast(PrimitiveType.Word32, m.Mem16(m.Word32(0x00123400))));
                m.MStore(m.Word32(0x00123400), m.Slice(PrimitiveType.Word16, m.IAdd(r1, 4), 0));
            });
        }
        }
    }
