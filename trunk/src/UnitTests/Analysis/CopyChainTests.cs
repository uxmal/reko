/* 
 * Copyright (C) 1999-2007 John Källén.
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

using NUnit.Framework;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class CopyChainTests
	{
		private Procedure proc;
		private SsaState ssa;

		[Test]
		public void SimpleChain()
		{
			Build(new SimpleChainMock());

			CopyChain cch = new CopyChain(ssa);
			Block b = proc.ExitBlock.Pred[0];
			cch.Kill(b.Statements[3]);
			cch.Kill(b.Statements[2]);

			Assert.AreEqual("reg_5 = 0x00000000", b.Statements[b.Statements.Count - 2].ToString());
		}

		private class SimpleChainMock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier reg = Local32("reg");
				Identifier tmp = Local32("tmp");
				Assign(tmp, reg);
				Assign(reg, 0);
				Assign(reg, tmp);
				Use(reg);
				Use(tmp);
				Return();
			}
		}

		[Test]
		public void SavedAndUsedRegister()
		{
			Build(new SavedAndUsedRegisterMock());

			CopyChain cch = new CopyChain(ssa);
			Block b = proc.RpoBlocks[1];
			Statement stm = b.Statements[4];
			Assert.AreEqual("use reg_6", stm.ToString());			
			cch.Kill(b.Statements[4]);
			cch.Kill(b.Statements[4]);
			Dump();
			Assert.AreEqual("tmp_4 = reg", b.Statements[0].ToString());
		}

		private class SavedAndUsedRegisterMock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier reg = Local32("reg");
				Identifier tmp = Local32("tmp");
				Assign(tmp, reg);
				Assign(reg, 0);
				Assign(reg, tmp);
				Store(reg, Int16(0));
				Use(reg);
				Use(tmp);
				Return();
			}
		}	

		[Test]
		public void Fork()
		{
			Procedure proc = new ForkMock().Procedure;
			SsaTransform st = new SsaTransform(proc, new DominatorGraph(proc), false);
			SsaState ssa = st.SsaState;

			CopyChain cch = new CopyChain(ssa);
			Block b = proc.RpoBlocks[3];
			Assert.AreEqual("use tmp_7", b.Statements[2].ToString());
			cch.Kill(b.Statements[2]);

			Assert.AreEqual("use reg_8", b.Statements[1].ToString());
			cch.Kill(b.Statements[1]);
			Assert.AreEqual(1, b.Statements.Count);
		}

		private class ForkMock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier reg = Local32("reg");
				Identifier tmp = Local32("tmp");

				BranchIf(Load(reg.DataType, Int32(0x01000100)), "skip");

				Assign(tmp, reg);
				Assign(reg, 0);
				Assign(reg, tmp);

				Label("skip");
				Use(tmp);
				Use(reg);
				Return();
			}

		}

		public void Build(ProcedureMock mock)
		{
			proc = mock.Procedure;
			SsaTransform st = new SsaTransform(proc, new DominatorGraph(proc), false);
			ssa = st.SsaState;
		}

		public void Dump()
		{
			ssa.Write(Console.Out);
			proc.Write(false, Console.Out);
		}



	}
}
