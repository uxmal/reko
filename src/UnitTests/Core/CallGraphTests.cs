/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Loading;
using Decompiler.Scanning;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class CallGraphTests
	{
		[Test]
		public void Creation()
		{
			CallGraph g = new CallGraph();
			Procedure p1 = new Procedure("p1000", null);
			Procedure p2 = new Procedure("p2000", null);
			Procedure p3 = new Procedure("p3000", null);
			Procedure p4 = new Procedure("p4000", null);

            ProcedureConstant pc1 = new ProcedureConstant(PrimitiveType.Pointer32, p1);
            ProcedureConstant pc2 = new ProcedureConstant(PrimitiveType.Pointer32, p2);
            ProcedureConstant pc3 = new ProcedureConstant(PrimitiveType.Pointer32, p3);
            ProcedureConstant pc4 = new ProcedureConstant(PrimitiveType.Pointer32, p4);

            Statement s11 = new Statement(new CallInstruction(pc2, new CallSite(0, 0)), p1.EntryBlock);
            Statement s12 = new Statement(new CallInstruction(pc2, new CallSite(0, 0)), p1.EntryBlock);
            Statement s13 = new Statement(new CallInstruction(pc3, new CallSite(0, 0)), p1.EntryBlock);
			p1.EntryBlock.Statements.Add(s11);
			p1.EntryBlock.Statements.Add(s12);
			p1.EntryBlock.Statements.Add(s13);

            Statement s21 = new Statement(new CallInstruction(pc3, new CallSite(0, 0)), p2.EntryBlock);
			Statement s22 = new Statement(new CallInstruction(pc4, new CallSite(0, 0)), p2.EntryBlock);
			p2.EntryBlock.Statements.Add(s21);
			p2.EntryBlock.Statements.Add(s22);

            Statement s31 = new Statement(new CallInstruction(pc4, new CallSite(0, 0)), p3.EntryBlock);
			p3.EntryBlock.Statements.Add(s31);

            Statement s41 = new Statement(new CallInstruction(pc4, new CallSite(0, 0)), p4.EntryBlock);

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
	}
}
