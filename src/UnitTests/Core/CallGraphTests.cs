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
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Loading;
using Reko.Scanning;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class CallGraphTests
	{
		[Test]
		public void Creation()
		{
			CallGraph g = new CallGraph();
            var arch = new Mock<IProcessorArchitecture>();
			Procedure p1 = new Procedure(arch.Object, "p1000", Address.Ptr32(0x1000), null);
			Procedure p2 = new Procedure(arch.Object, "p2000", Address.Ptr32(0x2000), null);
			Procedure p3 = new Procedure(arch.Object, "p3000", Address.Ptr32(0x3000), null);
			Procedure p4 = new Procedure(arch.Object, "p4000", Address.Ptr32(0x4000), null);

            var pc2 = new ProcedureConstant(PrimitiveType.Ptr32, p2);
            var pc3 = new ProcedureConstant(PrimitiveType.Ptr32, p3);
            var pc4 = new ProcedureConstant(PrimitiveType.Ptr32, p4);

            Statement s11 = new Statement(0, CreateCall(pc2), p1.EntryBlock);
            Statement s12 = new Statement(0, CreateCall(pc2), p1.EntryBlock);
            Statement s13 = new Statement(0, CreateCall(pc3), p1.EntryBlock);
			p1.EntryBlock.Statements.Add(s11);
			p1.EntryBlock.Statements.Add(s12);
			p1.EntryBlock.Statements.Add(s13);

            Statement s21 = new Statement(0, CreateCall(pc3), p2.EntryBlock);
            Statement s22 = new Statement(0, CreateCall(pc4), p2.EntryBlock);
			p2.EntryBlock.Statements.Add(s21);
			p2.EntryBlock.Statements.Add(s22);

            Statement s31 = new Statement(0, CreateCall(pc4), p3.EntryBlock);
			p3.EntryBlock.Statements.Add(s31);

            Statement s41 = new Statement(0, CreateCall(pc4), p4.EntryBlock);

			g.AddEntryPoint(p1);
			g.AddEdge(s11, p2);
			g.AddEdge(s12, p2);
			g.AddEdge(s13, p3);

			g.AddEdge(s21, p3);
			g.AddEdge(s22, p4);

			g.AddEdge(s31, p4);

			g.AddEdge(s41, p4);		// recursion!

            //$TODO: need Count
//			Assert.IsTrue(g.Callees(p1).Count == 3);
//			Assert.IsTrue(g.CallerStatements(p4).Count == 3);
		}

        private CallInstruction CreateCall(ProcedureConstant pc)
        {
            return new CallInstruction(pc, new CallSite(4, 0));
        }
	}
}
