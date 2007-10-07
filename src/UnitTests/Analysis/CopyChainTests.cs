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
using Decompiler.Arch.Intel;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class CopyChainTests
	{
		private ProcedureMock m;
		private string nl = Environment.NewLine;

		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
		}

		[Test]
		public void SingleCopy()
		{
			Identifier eax = m.Frame.EnsureRegister(Registers.eax);
			Identifier tmp = m.Frame.EnsureStackLocal(-4, eax.DataType);
			
			CopyChainFinder cch = new CopyChainFinder(null);
			cch.Process(m.Assign(tmp, eax));
			string exp = "dwLoc04 = eax" + nl;
			Assert.AreEqual(exp, Dump(cch.Chains[tmp]));
		}

		[Test]
		public void TrashCopy()
		{
			Identifier eax = m.Frame.EnsureRegister(Registers.eax);
			Identifier ebx = m.Frame.EnsureRegister(Registers.ebx);

			CopyChainFinder cch = new CopyChainFinder(null);
			cch.Process(m.Assign(ebx, eax));
			cch.Process(m.Assign(ebx, m.Int32(42)));	 // trashes ebx
			Assert.AreEqual("Trash", cch.Chains[ebx]);
		}

		[Test]
		public void RestoredCopy()
		{
			Identifier eax = m.Frame.EnsureRegister(Registers.eax);
			Identifier ebx = m.Frame.EnsureRegister(Registers.ebx);

			CopyChainFinder cch = new CopyChainFinder(null);
			cch.Process(m.Assign(ebx, eax));
			cch.Process(m.Assign(eax, m.Int32(42)));
			cch.Process(m.Assign(eax, ebx));				// eax now is restored.
			string exp = 
				"ebx = eax" + nl +
				"eax = ebx" + nl;
			Assert.AreEqual(exp, Dump(cch.Chains[ebx]));
		}

		[Test]
		public void OneBranchTrashes()
		{
			Identifier eax = m.Frame.EnsureRegister(Registers.eax);
			Identifier tmp = m.Frame.EnsureStackLocal(-4, eax.DataType);

			m.BranchIf(eax, "preserve");
			m.Assign(eax, m.Int32(42));
			m.Jump("join");
			m.Label("preserve");
			m.Assign(tmp, eax);
			m.Assign(eax, m.Int32(42));
			m.Assign(eax, tmp);
			m.Label("join");
			m.Return();
			CopyChainFinder cch = new CopyChainFinder(m.Procedure);
			cch.FindCopyChains();
			Assert.AreEqual("Trash", cch.Chains[eax]);
		}

		private string Dump(object a)
		{
			ArrayList aa = (ArrayList) a;
			StringWriter writer = new StringWriter();
			foreach (object o in aa)
			{
				writer.WriteLine(o);
			}
			return writer.ToString();
		}
	}

	[TestFixture]
	[Obsolete("Class is going away")]
	public class CopyChainTests2
	{
		private Procedure proc;
		private SsaState ssa;

		[Test]
		public void SimpleChain()
		{
			Build(new SimpleChainMock());

			CopyChain2 cch = new CopyChain2(ssa);
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

			CopyChain2 cch = new CopyChain2(ssa);
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

			CopyChain2 cch = new CopyChain2(ssa);
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
