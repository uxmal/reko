/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class TerminationAnalysisTests
    {
        ProcedureBase exit;
        ProgramMock progMock;

        [SetUp]
        public void Setup()
        {
            exit = new ExternalProcedure("exit", 
                new ProcedureSignature(null, new Identifier("retCode", 0, PrimitiveType.Int32, new StackArgumentStorage(0, PrimitiveType.Int32))));
            exit.Characteristics = new ProcedureCharacteristics();
            exit.Characteristics.Terminates = true;

            progMock = new ProgramMock();
        }
        [Test]
        public void BlockTerminates()
        {
            ProcedureMock m = new ProcedureMock();
            m.Call(exit);
            var b = m.CurrentBlock;
            m.Return();

            var a = new TerminationAnalysis();
            a.Analyze(b);
            Assert.IsTrue(a.Terminates(b));
        }

        [Test]
        public void BlockDoesntTerminate()
        {
            var m = new ProcedureMock();
            m.Store(m.Word32(0x1231), m.Byte(0));
            var b = m.Block;
            m.Return();
            var a = new TerminationAnalysis();
            a.Analyze(b);
            Assert.IsFalse(a.Terminates(b));
        }

        [Test]
        public void ProcedureTerminatesIfBlockTerminates()
        {
            var m = new ProcedureMock();
            m.Call(exit);
            m.Return();
            var proc = m.Procedure;

            var a = new TerminationAnalysis();
            a.Analyze(proc);
            Assert.IsTrue(a.Terminates(proc));
        }

        [Test]
        public void ProcedureDoesntTerminatesIfOneBranchDoesnt()
        {
            var m = new ProcedureMock();
            m.BranchIf(m.Eq(m.Local32("foo"), m.Word32(0)), "bye");
            m.Call(exit);
            m.Label("bye");
            m.Return();

            var proc = m.Procedure;
            var a = new TerminationAnalysis();
            a.Analyze(proc);
            Assert.IsFalse(a.Terminates(proc));
        }

        [Test]
        public void ProcedureTerminatesIfAllBranchesDo()
        {
            var m = new ProcedureMock();
            m.BranchIf(m.Eq(m.Local32("foo"), m.Word32(0)), "whee");
            m.Call(exit);
            m.TerminateProcedure();
            m.Label("whee");
            m.Call(exit);
            m.TerminateProcedure();

            var proc = m.Procedure;
            var a = new TerminationAnalysis();
            a.Analyze(proc);
            Assert.IsTrue(a.Terminates(proc));
        }

        [Test]
        public void TerminatingSubProcedure()
        {
            var sub = CompileProcedure("sub", delegate(ProcedureMock m)
            {
                m.Call(exit);
                m.TerminateProcedure();
            });

            Procedure caller = CompileProcedure("caller", delegate(ProcedureMock m)
            {
                m.Call(sub);
                m.Return();
            });

            var a = new TerminationAnalysis();
            a.Analyze(progMock.BuildProgram());
            Assert.IsTrue(a.Terminates(sub));
            Assert.IsTrue(a.Terminates(caller));


        }

        private Procedure CompileProcedure(string procName, Action<ProcedureMock> builder)
        {
            var m = new ProcedureMock(procName);
            builder(m);
            progMock.Add(m);
            return m.Procedure;
        }
    }
}
