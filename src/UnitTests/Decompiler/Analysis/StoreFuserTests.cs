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
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Hll.Pascal;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class StoreFuserTests
    {
        private FakeArchitecture arch;

        public StoreFuserTests()
        {
            this.arch = new FakeArchitecture(new ServiceContainer());
        }

        private void RunTest(string sExp, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);

            var stfu = new StoreFuser(m.Ssa, new FakeDecompilerEventListener());
            stfu.Transform();

            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(false, sw);
            sw.Flush();
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                m.Ssa.Dump(true);
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        [Test]
        public void Stfu_LittleEndian()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	id = Mem3[0x123400<32>:word32]
	idLo = SLICE(id, word16, 0) (alias)
	ilHi = SLICE(id, word16, 16) (alias)
	Mem5[0x00123404<p32>:word32] = id
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = m.Reg32("id");
                var idLo = m.Reg16("idLo");
                var idHi = m.Reg16("ilHi");

                m.Assign(id, m.Mem32(m.Word32(0x00123400)));
                m.Alias(idLo, m.Slice(id, idLo.DataType, 0));
                m.Alias(idHi, m.Slice(id, idHi.DataType, 16));
                m.MStore(m.Word32(0x00123404), idLo);
                m.MStore(m.Word32(0x00123406), idHi);
            });
        }


        [Test]
        public void Stfu_LittleEndian_4_slices()
        {
            var sExp =
        #region Expected
@"SsaProcedureBuilder_entry:
l1:
	id = Mem5[0x123400<32>:word32]
	s0 = SLICE(id, byte, 0) (alias)
	s1 = SLICE(id, byte, 8) (alias)
	s2 = SLICE(id, byte, 16) (alias)
	s3 = SLICE(id, byte, 24) (alias)
	Mem9[0x00123404<p32>:word32] = id
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = m.Reg32("id");
                var s0 = m.Reg8("s0");
                var s1 = m.Reg8("s1");
                var s2 = m.Reg8("s2");
                var s3 = m.Reg8("s3");

                m.Assign(id, m.Mem32(m.Word32(0x00123400)));
                m.Alias(s0, m.Slice(id, s0.DataType, 0));
                m.Alias(s1, m.Slice(id, s1.DataType, 8));
                m.Alias(s2, m.Slice(id, s2.DataType, 16));
                m.Alias(s3, m.Slice(id, s3.DataType, 24));
                m.MStore(m.Word32(0x00123404), s0);
                m.MStore(m.Word32(0x00123405), s1);
                m.MStore(m.Word32(0x00123406), s2);
                m.MStore(m.Word32(0x00123407), s3);
            });
        }

        [Test(Description = "The slice(s) don't cover the whole variable.")]
        public void Stfu_NotEnoughSlices()
        {
            var sExp =
            #region Expected
               @"SsaProcedureBuilder_entry:
l1:
	id = Mem2[0x123400<32>:word32]
	idLo = SLICE(id, word16, 0) (alias)
	Mem3[0x123404<32>:word16] = idLo
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var id = m.Reg32("id");
                var idLo = m.Reg16("idLo");

                m.Assign(id, m.Mem32(m.Word32(0x00123400)));
                m.Alias(idLo, m.Slice(id, idLo.DataType, 0));
                m.MStore(m.Word32(0x00123404), idLo);
            });
        }


        [Test]
        public void Stfu_Gap_in_slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	id = Mem4[0x123400<32>:word32]
	s0 = SLICE(id, byte, 0) (alias)
	s1 = SLICE(id, byte, 8) (alias)
	s2 = SLICE(id, byte, 24) (alias)
	Mem5[0x123404<32>:byte] = s0
	Mem6[0x123405<32>:byte] = s1
	Mem7[0x123406<32>:byte] = s2
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = m.Reg32("id");
                var s0 = m.Reg8("s0");
                var s1 = m.Reg8("s1");
                var s2 = m.Reg8("s2");

                m.Assign(id, m.Mem32(m.Word32(0x00123400)));
                m.Alias(s0, m.Slice(id, s0.DataType, 0));
                m.Alias(s1, m.Slice(id, s1.DataType, 8));
                m.Alias(s2, m.Slice(id, s2.DataType, 24));
                m.MStore(m.Word32(0x00123404), s0);
                m.MStore(m.Word32(0x00123405), s1);
                m.MStore(m.Word32(0x00123406), s2);
            });
        }

    }
}
