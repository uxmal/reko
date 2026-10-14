#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Rca;
using Reko.Arch.Rca.Analysis;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Rca.Analysis;

public class XRegisterAnalysisTests
{
    private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder, [CallerMemberName] string name = "")
    {
        var sc = new ServiceContainer();
        var arch = new Rca1802Architecture(sc, "rca1802", []);
        var m = new SsaProcedureBuilder(name);
        builder(m);
        var ssa = m.Ssa;

        var context = new AnalysisContext(
            new Program(), m.Ssa.Procedure, null, sc, new FakeDecompilerEventListener());

        var xra = new XRegisterAnalysis(ssa, context);
        var (ssaNew, _) = xra.Transform(ssa);

        var sw = new StringWriter();
        sw.WriteLine();
        ssaNew.Procedure.Write(false, sw);
        var sActual = sw.ToString();
        if (sActual != sExpected)
        {
            Console.Out.Write(sActual);
            Assert.That(sActual, Is.EqualTo(sExpected));
        }
    }

    [Test]
    public void Xra_X_is_R2_on_entry()
    {
        string sExpected =
        #region
            @"
// Xra_X_is_R2_on_entry
// Return size: 0
define Xra_X_is_R2_on_entry
Xra_X_is_R2_on_entry_entry:
	def X_1
	def fp
	// succ:  l1
l1:
	R2_2 = fp
	X_1 = 2<4>
	D_3 = Mem4[__readX(X_1):byte]
	return
	// succ:  Xra_X_is_R2_on_entry_exit
Xra_X_is_R2_on_entry_exit:
";
        #endregion

        RunTest(sExpected, m =>
        {
            var fp = m.FramePointer();
            var X_1 = m.Reg("X_1", Registers.X);
            var R2_2 = m.Reg("R2_2", Registers.GpRegisters[2]);
            var D_3 = m.Reg("D_3", Registers.D);
            m.AddDefToEntryBlock(X_1);

            m.AddDefToEntryBlock(fp);
            m.Assign(R2_2, fp);
            m.Assign(X_1, 2);
            m.Assign(D_3, m.Mem8(m.Fn(Rca1802Rewriter.__readX_intrinsic, X_1)));
            m.Return();
        });

    }
}
