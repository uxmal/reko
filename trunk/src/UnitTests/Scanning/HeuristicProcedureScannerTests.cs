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

using Decompiler.Scanning;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class HeuristicProcedureScannerTests : HeuristicTestBase
    {
        private HeuristicProcedureScanner subject;
        private HeuristicProcedure proc;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        private void AssertConflicts(string sExp, IEnumerable<Tuple<HeuristicBlock, HeuristicBlock>> conflicts)
        {
            var sActual = conflicts
                .OrderBy(c => c.Item1.Address.ToLinear())
                .ThenBy(c => c.Item2.Address.ToLinear())
                .Aggregate(
                    new StringBuilder(),
                    (s, c) => s.AppendFormat("({0}-{1})", c.Item1.Name, c.Item2.Name).AppendLine())
                .ToString();
            if (sExp != sActual)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void When_DisassembleProcedure()
        {
            var hsc = new HeuristicScanner(prog, host);
            this.proc = hsc.DisassembleProcedure(
                prog.Image.BaseAddress,
                prog.Image.BaseAddress + prog.Image.Length);
        }

        [Test]
        public void HPSC_Conflicts_1()
        {
            Given_Image32(
                0x0010000,
                "55 89 e5 c3");
            Given_x86_32();
            Given_RewriterHost();
            mr.ReplayAll();

            When_DisassembleProcedure();
            var conflicts = HeuristicProcedureScanner.BuildConflictGraph(proc.Cfg.Nodes);
            var sExp = @"(l00010001-l00010002)
(l00010002-l00010003)
";
            AssertConflicts(sExp, conflicts);
        }

        [Test]
        public void HPSC_Conflicts_2()
        {
            Given_Image32(
                0x0010000,
                "55 E8 00 00 00 38 c3");
            Given_x86_32();
            Given_RewriterHost();
            mr.ReplayAll();

            When_DisassembleProcedure();
            var conflicts = HeuristicProcedureScanner.BuildConflictGraph(proc.Cfg.Nodes);
            var sExp =
@"(l00010001-l00010002)
(l00010001-l00010003)
(l00010001-l00010004)
(l00010001-l00010005)
(l00010002-l00010003)
(l00010003-l00010004)
(l00010004-l00010005)
(l00010005-l00010006)
";
            AssertConflicts(sExp, conflicts);
        }

        [Test]
        public void HPSC_DiscardNodes()
        {
            Given_Image32(
                0x0010000,
                "55 8B EC  a1 32 12 1a 12 5D c3");
            Given_x86_32();
            Given_RewriterHost();
            Given_NoImportedProcedures();
            mr.ReplayAll();

            When_DisassembleProcedure();
            var hps = new HeuristicProcedureScanner(prog, proc);
            hps.BlockConflictResolution();

            var sExp =
            #region Expected
@"l00010000:  // pred:
    push ebp
l00010001:  // pred: l00010000
    mov ebp,esp
l00010003:  // pred: l00010001
    mov eax,[121A1232]
l00010008:  // pred: l00010003
    pop ebp
l00010009:  // pred: l00010008
    ret 
";
            #endregion

            AssertBlocks(sExp, proc.Cfg);
        }

        private void Given_NoImportedProcedures()
        {
            host.Stub(h => h.GetImportedProcedure(null, null))
                .IgnoreArguments()
                .Return(null);
        }

        [Test]
        public void HPSC_TrickyProc()
        {
            Given_Image32(0x0010000, TrickyProc);
            Given_x86_32();
            Given_RewriterHost();
            Given_NoImportedProcedures();
            mr.ReplayAll();

            When_DisassembleProcedure();
            var hps = new HeuristicProcedureScanner(prog, proc);
            hps.BlockConflictResolution();

            var sExp =
            #region Expected
@"l00010000:  // pred:
    push ebp
l00010001:  // pred: l00010000
    mov ebp,esp
l00010003:  // pred: l00010001
    call 11750008
l0001000A:  // pred:
    cmp al,00
l0001000C:  // pred: l0001000A
    jnz 00010014
l0001000E:  // pred: l0001000C
    mov al,00
l00010010:  // pred: l0001000E
    jmp 00010019
l00010014:  // pred: l0001000C
    mov eax,[01740000]
l00010019:  // pred: l00010010 l00010014
    mov esp,ebp
l0001001B:  // pred: l00010019
    pop ebp
l0001001C:  // pred: l0001001B
    ret 
";
#endregion
            AssertBlocks(sExp, proc.Cfg);
        }
    }
}
