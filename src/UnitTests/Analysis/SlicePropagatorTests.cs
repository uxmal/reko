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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;
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
            this.arch = new FakeArchitecture(new ServiceContainer());
        }

        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);
            var slp = new SlicePropagator(m.Ssa, new FakeDecompilerEventListener());
            slp.Transform();
            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(true, sw);
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
        public void Slp_Linear()
        {
            var sExp =
            #region Expected 
@"SsaProcedureBuilder_entry:
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
                m.Assign(r1, m.Convert(m.Mem8(m.Word32(0x00123400)), PrimitiveType.Byte, PrimitiveType.UInt32));
                m.MStore(m.Word32(0x00123404), m.Slice(PrimitiveType.Byte, r1, 0));
            });
        }

        [Test]
        public void Slp_Def()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
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
        public void Slp_Sum()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
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
                m.Assign(r1, m.Convert(m.Mem16(m.Word32(0x00123400)), PrimitiveType.Word16, PrimitiveType.Word32));
                m.MStore(m.Word32(0x00123400), m.Slice(PrimitiveType.Word16, m.IAdd(r1, 4), 0));
            });
        }

        [Test]
        public void Slp_Sum_dont_propagate_highbits()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	id_1 = CONVERT(Mem1[0x123400<32>:word16], word16, word32)
	Mem2[0x123400<32>:word16] = SLICE(id_1 + 4<32>, word16, 8)
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1 = m.Register(arch.GetRegister("r1"));
                m.Assign(r1, m.Convert(m.Mem16(m.Word32(0x00123400)), PrimitiveType.Word16, PrimitiveType.Word32));
                m.MStore(m.Word32(0x00123400), m.Slice(PrimitiveType.Word16, m.IAdd(r1, 4), 8));
            });
        }

        [Test]
        public void Slp_Phi()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  m1
m1:
	branch Mem4[0x123400<32>:word32] >= 0<32> m3
	// succ:  m2 m3
m2:
	r1_7 = 0x3404<16>
	goto m4
	// succ:  m4
m3:
	r1_8 = 0x3408<16>
	// succ:  m4
m4:
	r1_9 = PHI((r1_7, m2), (r1_8, m3))
	r2_10 = r1_9
	Mem5[0x12340C<32>:word16] = r2_10
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1_1 = m.Register(arch.GetRegister("r1"));
                var r1_2 = m.Register(arch.GetRegister("r1"));
                var r1_3 = m.Register(arch.GetRegister("r1"));
                var r2_4 = m.Register(arch.GetRegister("r2"));


                m.Label("m1");
                m.BranchIf(m.Ge0(m.Mem32(m.Word32(0x00123400))), "m3");

                m.Label("m2");
                m.Assign(r1_1, m.Word32(0x00123404));
                m.Goto("m4");

                m.Label("m3");
                m.Assign(r1_2, m.Word32(0x00123408));
                m.Goto("m4");

                m.Label("m4");
                m.Phi(r1_3, (r1_1, "m2"), (r1_2, "m3"));
                m.Assign(r2_4, r1_3);
                m.MStore(m.Word32(0x0012340C), m.Slice(PrimitiveType.Word16, r2_4, 0));
                m.Return();
            });
        }

        [Test]
        public void Slp_Seq()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r3_5 = Mem2[0x123400<32>:byte]
	Mem3[0x123404<32>:byte] = r3_5
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var tmpHi = m.Temp(PrimitiveType.CreateWord(24), "tmp");
                var r3 = m.Register(arch.GetRegister("r3"));

                m.Assign(r3, m.Seq(tmpHi, m.Mem8(m.Word32(0x00123400))));
                m.MStore(m.Word32(0x00123404), m.Slice(PrimitiveType.Byte, r3, 0));
            });
        }

        [Test]
        public void Slp_NecessarySlice()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	id_1 = CONVERT(Mem1[0x123400<32>:byte], byte, word32)
	Mem2[0x123408<32>:word16] = SLICE(id_1, word16, 0)
	Mem3[0x12340C<32>:word32] = id_1
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r3 = m.Register(arch.GetRegister("r3"));
                m.Assign(r3, m.Convert(m.Mem8(m.Word32(0x00123400)), PrimitiveType.Byte, PrimitiveType.Word32));
                m.MStore(m.Word32(0x00123408), m.Slice(PrimitiveType.Word16, r3, 0));
                m.MStore(m.Word32(0x0012340C), r3);
            });
        }

        [Test]
        public void Slp_Call()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	call <invalid> (retsize: 0;)
		uses: r2:id_1
		defs: r3:id_2
	Mem2[0x123400<32>:byte] = SLICE(id_2, byte, 0)
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r2 = m.Register(arch.GetRegister("r2"));
                var r3 = m.Register(arch.GetRegister("r3"));
                var external = new ProcedureConstant(PrimitiveType.Ptr32, new ExternalProcedure("external", new FunctionType()));
                m.Call("external", 0,
                    new[] { (r2.Storage, (Expression)r2) },
                    new[] { (r3.Storage, r3) });
                m.MStore(m.Word32(0x00123400), m.Slice(PrimitiveType.Byte, r3, 0));
            });
        }

        [Test]
        public void Slp_Dont_slice_memory_loads()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	bb = SLICE(Mem2[0x123400<32>:word16], byte, 0)
	Mem3[0x123408<32>:byte] = bb
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var ww = m.Temp(PrimitiveType.Word16, "ww");
                var bb = m.Temp(PrimitiveType.Byte, "bb");
                m.Assign(ww, m.Mem16(m.Word32(0x00123400)));
                m.Assign(bb, m.Slice(bb.DataType, ww, 0));
                m.Store(m.Mem8(m.Word32(0x00123408)), bb);
            });
        }

        [Test]
        public void Slp_Slice_Sequence()
        {
            var sExp =
@"SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	v2_4 = SLICE(Mem1[0x123402<32>:word64], word32, 16)
	Mem2[0x123410<32>:word32] = v2_4
SsaProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var reg_r2 = arch.GetRegister("r2");
                var reg_r3 = arch.GetRegister("r3");
                var r2_r3 = m.SeqId("r2_r3", PrimitiveType.Word64, reg_r2, reg_r3);

                m.Assign(r2_r3, m.Mem(r2_r3.DataType, m.Word32(0x00123400)));
                m.MStore(m.Word32(0x00123410), m.Slice(reg_r2.DataType, r2_r3, 16));
            });
        }
    }
}
