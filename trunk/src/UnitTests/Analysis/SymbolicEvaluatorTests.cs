#region License
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
#endregion

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class SymbolicEvaluatorTests
    {
        private Identifier esp ;
        private Identifier ebp;
        private Identifier eax;
        private SymbolicEvaluator se;
        private IProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
        }

        private static Identifier Reg32(string name)
        {
            return new Identifier(name, 1, PrimitiveType.Word32, new TemporaryStorage());
        }
        private static Identifier Reg8(string name)
        {
            return new Identifier(name, 1, PrimitiveType.Byte, new TemporaryStorage());
        }

        private void CreateSymbolicEvaluator()
        {
            se = new SymbolicEvaluator(arch);
        }

        private void RunBlockTest(Action<ProcedureBuilder> testBuilder)
        {
            var builder = new ProcedureBuilder(); 
            testBuilder(builder);
            var proc = builder.Procedure;
            CreateSymbolicEvaluator();
            proc.ControlGraph.Successors(proc.EntryBlock).First().Statements.ForEach(x => se.Evaluate(x.Instruction));
        }

        [Test]
        public void EvaluateConstantAssignment()
        {
            var name = "edx";
            var edx = Reg32(name);
            var ass = new Assignment(edx, Constant.Word32(3));
            var se = new SymbolicEvaluator(arch);
            se.Evaluate(ass);
            Assert.IsNotNull(se.RegisterState);
            Assert.IsInstanceOf(typeof(Constant), se.RegisterState[edx]);
        }


        [Test]
        public void IdentifierCopy()
        {
            var esp = Reg32("esp");
            var ebp = Reg32("ebp");
            CreateSymbolicEvaluator();
            var ass = new Assignment(ebp, esp);
            se.Evaluate(ass);
            Assert.AreSame(esp, se.RegisterState[esp]);
            Assert.AreSame(esp, se.RegisterState[ebp]);
        }

        [Test]
        public void AdjustValue()
        {
            var esp = Reg32("esp");
            CreateSymbolicEvaluator();
            var ass = new Assignment(esp, new BinaryExpression(BinaryOperator.Add, esp.DataType, esp, Constant.Word32(4)));
            se.Evaluate(ass);
            Assert.AreEqual("esp + 0x00000004", se.RegisterState[esp].ToString());
        }

        [Test]
        public void LoadFromMemoryTrashes()
        {
            var ebx = Reg32("ebx");
            var al = Reg8("al");
            CreateSymbolicEvaluator();
            var ass = new Assignment(al, new MemoryAccess(ebx, al.DataType));
            se.Evaluate(ass);
            Assert.AreEqual("<void>", se.RegisterState[al].ToString());
        }

        [Test]
        public void PushConstant()
        {
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                ebp = m.Frame.EnsureRegister(Registers.ebp );
                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
            });
            Assert.AreEqual("esp - 0x00000004", se.RegisterState[esp].ToString());
            Assert.AreEqual("esp - 0x00000004", se.RegisterState[ebp].ToString());
        }

        [Test]
        public void PushPop()
        {
            Identifier eax = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, eax);
                m.Assign(eax, m.Word32(1));
                m.Assign(eax, m.LoadDw(esp));
                m.Assign(esp, m.Add(esp, 4));
            });
            Assert.AreEqual("eax", se.RegisterState[eax].ToString());
        }

        [Test]
        public void FramePointer()
        {
            Identifier eax = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                ebp = m.Frame.EnsureRegister(Registers.ebp);
                eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(esp, m.Sub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(esp, m.Sub(esp, 20));

                m.Store(m.Add(ebp, 8), m.Word32(1));
                m.Assign(eax, m.LoadDw(m.Add(esp, 28)));

                m.Assign(esp, m.Add(esp, 20));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.Add(esp, 4));
            });
            Assert.AreEqual("0x00000001", se.RegisterState[eax].ToString());
            Assert.AreEqual("esp", se.RegisterState[ebp].ToString());
        }
    }
}

