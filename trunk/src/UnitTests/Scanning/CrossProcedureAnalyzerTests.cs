#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;  
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class CrossProcedureAnalyzerTests
    {
        [Test]
        public void Crpa_Analyze_SimpleProc()
        {
            var prog = Given_Simple_Proc();

            var crpa = new CrossProcedureAnalyzer(prog);
            crpa.Analyze(prog);
            Assert.AreEqual(0, crpa.BlocksNeedingPromotion.Count);
        }

        [Test]
        public void Crpa_Analyze_CrossJump()
        {
            var prog = Given_CrossJump_Prog();
            var crpa = new CrossProcedureAnalyzer(prog);
            crpa.Analyze(prog);
            Assert.AreEqual(1, crpa.BlocksNeedingPromotion.Count);
        }

        [Test]
        public void Crpa_Analyze_CrossJumpToLinearReturn()
        {
            var prog = Given_CrossJumpLinearReturn_Prog();
            var crpa = new CrossProcedureAnalyzer(prog);
            crpa.Analyze(prog);
            Assert.AreEqual(1, crpa.BlocksNeedingCloning.Count);
            Assert.AreEqual(0, crpa.BlocksNeedingPromotion.Count);
        }

        [Test]
        public void Crpa_Promote_Block()
        {
            var prog = Given_CrossJump_Prog();
            var crpa = new CrossProcedureAnalyzer(prog);
            crpa.Analyze(prog);
            crpa.PromoteBlocksToProcedures(crpa.BlocksNeedingPromotion);
            Assert.AreEqual(3, prog.Procedures.Count);
            var proc = prog.Procedures.Values[1];
            Assert.AreEqual("fn00001001", proc.Name);
            var sExp =
#region Expected string
 @"// fn00001001
void fn00001001()
fn00001001_entry:
	goto Real_entry
	// succ:  Real_entry
l1:
	return
	// succ:  fn00001001_exit
Real_entry:
	Mem0[r2:word32] = r1
	branch r2 Real_entry
	goto l1
	// succ:  l1 Real_entry
fn00001001_exit:
";
#endregion
            var sw = new StringWriter();
            proc.Write(false, sw);
            Console.WriteLine(sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        private Program Given_Simple_Proc()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                m.Label("Zlon");
                m.Return();
            });
            return b.BuildProgram();
        }

        private Program Given_CrossJump_Prog()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                m.Label("bob_1");
                m.Assign(r1, 0);
                m.Label("Real_entry");
                m.Store(r2, r1);
                m.BranchIf(r2, "Real_entry");
                m.Return();
            });

            b.Add("ext", m=>
            {
                var r1 = m.Reg32("r1");
                m.Label("ext_1");
                m.Assign(r1, 4);
                m.Jump("Real_entry");
            });
            return b.BuildProgram();
        }

        private Program Given_CrossJumpLinearReturn_Prog()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                m.Label("bob_1");
                m.Assign(r1, 0);
                m.Label("Real_entry");
                m.Store(r2, r1);
                m.Return();
            });

            b.Add("ext", m =>
            {
                var r1 = m.Reg32("r1");
                m.Label("ext_1");
                m.Assign(r1, 4);
                m.Jump("Real_entry");
            });
            return b.BuildProgram();
        }
    }
}
