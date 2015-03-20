#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    /// <summary>
    /// These tests are making sure that we can re-run SsaTransform
    /// on a procedure that already has been transformed once
    /// </summary>
    [TestFixture]
    public class SsaTestsRedo
    {
        private ProgramBuilder pb;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder();
            builder(pb);
            var proc = pb.Procedure;
            var dg = new DominatorGraph<Block>(proc.ControlGraph, proc.EntryBlock);
            var ssax = new SsaTransform(proc, dg);

            proc.Dump(true, false);
            Debug.Print("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            var vp = new ValuePropagator(ssax.SsaState.Identifiers, proc);
            vp.Transform();

            ssax.StackVariables = true;
            ssax.Transform();

            var writer = new StringWriter();
            proc.Write(false, writer);
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        [Test]
        public void SsarSimple()
        {
            var sExp = 
                @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	def fp
	def r1
	def r2
	// succ:  l1
l1:
	r1_4 = r1 + r2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });
        }

        [Test]
        public void SsarStackLocals()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	def fp
	def r63
	def r1
	def r2
	// succ:  l1
l1:
	r63_5 = fp
	r63_6 = fp - 0x00000004
	Mem7[fp - 0x00000004:word32] = r1
	r63_8 = fp - 0x00000008
	Mem9[fp - 0x00000008:word32] = r2
	r1_10 = Mem9[fp - 0x00000004:word32]
	r2_11 = Mem9[fp - 0x00000008:word32]
	r1_12 = r1_10 + r2_11
	Mem13[0x00010008:word32] = r1_12
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r2);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.Assign(r2, m.LoadDw(sp));
                m.Assign(r1, m.IAdd(r1, r2));
                m.Store(m.Word32(0x010008), r1);
                m.Return();
            });
        }
    }
}
