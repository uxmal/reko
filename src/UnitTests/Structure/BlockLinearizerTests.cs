/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Absyn;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class BlockLinearizerTests
	{
		[Test]
		public void BlinEnsureLabel()
		{
			Procedure proc = new MockSnarl().Procedure;
			BlockLinearizer blin = new BlockLinearizer(null);
			Block block = proc.RpoBlocks[1];
			blin.EnsureLabel(block);
			Assert.IsTrue(block.Statements.Count > 1);
			Assert.IsNotNull((AbsynLabel) block.Statements[0].Instruction);
		}
		
		[Test]
		public void BlinInboundEdges()
		{
			Procedure proc = new MockSnarl().Procedure;
			BlockLinearizer blin = new BlockLinearizer(null);
			Block block = proc.RpoBlocks[1];
			blin.ConvertInboundEdgesToGotos(block);
			Assert.AreEqual(0, block.Pred.Count, "All predecessors converted to gotos");
			AbsynGoto g = (AbsynGoto) proc.RpoBlocks[0].Statements.Last.Instruction;
			Assert.AreEqual(block.Name, g.Label);
			Assert.IsNotNull((AbsynLabel) block.Statements[0].Instruction);
		}

		[Test]
		public void BlinInsertBreak()
		{
			Procedure proc = new MockSnarl().Procedure;
			BlockLinearizer blin = new BlockLinearizer(null);
			Block block = proc.RpoBlocks[4];
			blin.ConvertInboundEdges(block, new StructureExitCreator(MakeBreak));
			using (FileUnitTester fut = new FileUnitTester("Structure/BlinInsertBreak.txt"))
			{
				CodeFormatter fmt = new CodeFormatter(fut.TextWriter);
				fmt.Write(proc);
				proc.Write(false, fut.TextWriter);

				fut.AssertFilesEqual();
			}
			Assert.AreEqual("AbsynIf", proc.RpoBlocks[2].Statements.Last.Instruction.GetType().Name);
			Assert.AreEqual("AbsynBreak", proc.RpoBlocks[3].Statements.Last.Instruction.GetType().Name);
			Assert.AreEqual(1, proc.RpoBlocks[3].Pred.Count);
		}

		private AbsynStatement MakeBreak(Block b)
		{
			return new AbsynBreak();
		}

		[Test]
		public void BlinConvertBlock()
		{
			Procedure proc = new MockBlock().Procedure;
			BlockLinearizer blin = new BlockLinearizer(null);
			Block block = proc.RpoBlocks[1];
			Block.RemoveEdge(proc.RpoBlocks[0], proc.RpoBlocks[1]);
			AbsynCompoundStatement stms = (AbsynCompoundStatement) blin.ConvertBlock(block);
			Assert.AreEqual(2, stms.Statements.Count);
			Assert.IsNotNull((AbsynAssignment) stms.Statements[0]);
			Assert.IsNotNull((AbsynReturn) stms.Statements[1]);
		}


		private class MockBlock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier r1 = Local32("r1");
				Assign(r1, Int32(0));
				Return(Int32(3));
			}
		}
	}
}