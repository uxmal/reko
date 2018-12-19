
#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class ProjectionPropagatorTests
    {
        private void RunTest(string sExp, IProcessorArchitecture arch, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);
            var prpr = new ProjectionPropagator(arch, m.Ssa);
            prpr.Transform();
            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(false, sw);
            sw.Flush();
            m.Ssa.Dump(true);
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                Assert.AreEqual(sExp, sActual);
            }
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        /// <summary>
        /// Fuse together subregisters that compose into a larger register.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	h = SLICE(hl, byte, 8)
	l = SLICE(hl, byte, 0)
	de_1 = hl
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var h = m.Reg("h", Registers.h); 
                var l = m.Reg("l", Registers.l);
                var de_1 = m.Reg("de_1", Registers.de);

                m.Def(h);
                m.Def(l);
                m.Assign(de_1, m.Seq(h, l));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together subregisters that compose into a larger register.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters_slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def hl
	l = (byte) hl
	h = SLICE(hl, ui8, 8)
	de_1 = hl
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var hl = m.Reg("hl", Registers.hl);
                var h = m.Reg("h", Registers.h);
                var l = m.Reg("l", Registers.l);
                var de_1 = m.Reg("de_1", Registers.de);

                m.Def(hl);
                m.Assign(l, m.Cast(PrimitiveType.Byte, hl));
                m.Assign(h, m.Slice(hl, 8, 8));
                m.Assign(de_1, m.Seq(h, l));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together subregisters that compose into a larger register,
        /// used in two branches.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters_TwoBranches()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	h = SLICE(hl, byte, 8)
	l = SLICE(hl, byte, 0)
	branch a == 0x00 m2azero
m1anotzero:
	de_1 = hl
	goto m3done
m2azero:
	de_2 = hl
m3done:
	de_3 = PHI((de_1, m1anotzero), (de_2, m2azero))
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var a = m.Reg("a", Registers.a);
                var h = m.Reg("h", Registers.h);
                var l = m.Reg("l", Registers.l);
                var de_1 = m.Reg("de_1", Registers.de);
                var de_2 = m.Reg("de_2", Registers.de);
                var de_3 = m.Reg("de_3", Registers.de);

                m.Def(h);
                m.Def(l);
                m.BranchIf(m.Eq0(a), "m2azero");

                m.Label("m1anotzero");
                m.Assign(de_1, m.Seq(h, l));
                m.Goto("m3done");

                m.Label("m2azero");
                m.Assign(de_2, m.Seq(h, l));

                m.Label("m3done");
                m.Phi(de_3, (de_1, "m1anotzero"), (de_2, "m2azero"));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together a register pair.
        /// </summary>
        [Test]
        public void Prjpr_Simple_RegisterPair()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl_de
l1:
	hl = SLICE(hl_de, word16, 16)
	de = SLICE(hl_de, word16, 0)
	hl_de_1 = hl_de
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var hl = m.Reg("hl", Registers.hl);
                var de = m.Reg("de", Registers.de);
                var hl_de = m.Frame.EnsureSequence(hl.Storage, de.Storage, PrimitiveType.Word32);
                var hl_de_1 = m.Ssa.Identifiers.Add(new Identifier("hl_de_1", hl_de.DataType, hl_de.Storage), null, null, false).Identifier;

                m.Def(hl);
                m.Def(de);
                m.Assign(hl_de_1, m.Seq(hl, de));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Phi_Subregisters()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def h
	def l
	hl_1 = SEQ(h, l)
loop:
	hl_3 = PHI((hl_1, l1), (hl_2, loop))
	h_4 = PHI((h, l1), (h_2, loop))
	l_5 = PHI((l, l1), (l_3, loop))
	hl_1 = hl_3 << 0x01
	h_2 = SLICE(hl_1, byte, 8)
	l_3 = (byte) hl_1
	hl_2 = SEQ(h_2, l_3)
	goto loop
xit:
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var h = m.Reg("h", Registers.h);
                var l = m.Reg("l", Registers.l);
                var hl_1 = m.Reg("hl_1", Registers.de);
                var h_2 = m.Reg("h_2", Registers.h);
                var l_3 = m.Reg("l_3", Registers.l);
                var h_4 = m.Reg("h_4", Registers.h);
                var l_5 = m.Reg("l_5", Registers.l);

                m.Def(h);
                m.Def(l);

                m.Label("loop");
                m.Phi(h_4, (h, "l1"), (h_2, "loop"));
                m.Phi(l_5, (l, "l1"), (l_3, "loop"));
                m.Assign(hl_1, m.Shl(m.Seq(h_4, l_5), 1));
                m.Assign(h_2, m.Slice(PrimitiveType.Byte, hl_1, 8));
                m.Assign(l_3, m.Cast(PrimitiveType.Byte, hl_1));
                m.Goto("loop");

                m.Label("xit");
                m.Return();
            });
        }
    }
}
