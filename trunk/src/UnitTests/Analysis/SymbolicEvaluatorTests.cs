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
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class SymbolicEvaluatorTests
    {
        private static Identifier Reg32(string name)
        {
            return new Identifier(name, 1, PrimitiveType.Word32, new TemporaryStorage());
        }

        [Test]
        public void EvaluateConstantAssignment()
        {
            var name = "edx";
            var edx = Reg32(name);
            var ass = new Assignment(edx, Constant.Word32(3));
            var se = new SymbolicEvaluator();
            se.Evaluate(ass);
            Assert.IsNotNull(se.State);
            Assert.IsInstanceOf(typeof(Constant), se.State[edx]);
        }


        [Test]
        public void IdentifierCopy()
        {
            var esp = Reg32("esp");
            var ebp = Reg32("ebp");
            var se = new SymbolicEvaluator();
            var ass = new Assignment(ebp, esp);
            se.Evaluate(ass);
            Assert.AreSame(esp, se.State[esp]);
            Assert.AreSame(esp, se.State[ebp]);
        }

        [Test]
        public void AdjustValue()
        {
            var esp = Reg32("esp");
            var se = new SymbolicEvaluator();
            var ass = new Assignment(esp, new BinaryExpression(BinaryOperator.Add, esp.DataType, esp, Constant.Word32(4)));
            se.Evaluate(ass);
            Assert.AreEqual("esp + 0x00000004", se.State[esp].ToString());
        }
    }
}

