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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class TrashedRegisterFinderTests
	{
		private ProcedureMock m;
		private Program prog;

		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
		}

		[Test]
		public void TrashRegister()
		{
			Identifier r1 =  m.Register(1);
			Statement stm = m.Assign(r1, m.Int32(0));

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);

			stm.Instruction.Accept(trf);
			Assert.IsTrue(trf.TrashedRegisters.Contains(r1.Storage));
//			Assert.AreNotSame(trf.RegisterSet[r1], r1);
		}

		[Test]
		public void TrashFlag()
		{
			IntelArchitecture arch = new IntelArchitecture(null);
			Identifier scz = m.Frame.EnsureFlagGroup(0x7, arch.GrfToString(0x7), PrimitiveType.Byte);
			Statement stm = m.Assign(scz, m.Int32(3));

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);
			stm.Instruction.Accept(trf);
			Assert.AreEqual(0x7, trf.TrashedFlags);
		}

		[Test]
		public void TrashCompoundRegister()
		{
			Identifier ax = m.Frame.EnsureRegister(Registers.ax);
			Statement stm = m.Assign(ax, 1);

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);
			stm.Instruction.Accept(trf);
			Assert.AreEqual("(ax:TRASH)", DumpRegs(trf.TrashedRegisters));
		}

		[Test]
		public void Copy()
		{
			Identifier r1 = m.Register(1);
			Identifier r2 = m.Register(2);
			Statement stm = m.Assign(r2, r1);

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);

			stm.Instruction.Accept(trf);
			Assert.AreEqual(r1.Storage, trf.TrashedRegisters[r2.Storage]);
		}

		[Test]
		public void CopyBack()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier tmp = m.Local32("tmp");
			Identifier r2 = m.Register(2);
			Statement stm1 = m.Assign(tmp, r2);
			Statement stm2 = m.Assign(r2, m.Int32(0));
			Statement stm3 = m.Assign(r2, tmp);

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);

			stm1.Instruction.Accept(trf);
			stm2.Instruction.Accept(trf);
			stm3.Instruction.Accept(trf);

			Assert.AreEqual(r2.Storage, trf.TrashedRegisters[r2.Storage]);
		}

		[Test]
		public void OutParameters()
		{
			Identifier r2 = m.Register(2);
			Statement stm = m.SideEffect(m.Fn("Hello", m.AddrOf(r2)));

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, null);

			stm.Instruction.Accept(trf);
			Assert.AreEqual("TRASH", trf.TrashedRegisters[r2.Storage].ToString());
		}

		[Test]
		public void CallInstruction()
		{
			Procedure callee = new Procedure("Callee", new Frame(PrimitiveType.Word32));
			CallInstruction ci = new CallInstruction(callee, 0, 0);
			ProgramDataFlow flow = new ProgramDataFlow();
			ProcedureFlow pf = new ProcedureFlow(callee, prog.Architecture);
			pf.TrashedRegisters[Registers.ebx.Number] = true;
			flow[callee] = pf;
			
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);

			trf.CurrentFrame = new Frame(null);
			ci.Accept(trf);
			Assert.AreEqual("(ebx:TRASH)", DumpRegs(trf.TrashedRegisters));
		}

		[Test]
		public void PropagateToSuccessorBlocks()
		{
			Identifier r1 = m.Register(1);
			Identifier r2 = m.Register(2);
			Identifier r3 = m.Register(3);
			Block b = new Block(null, "b");
			Block t = new Block(null, "t");
			Block e = new Block(null, "e");
			Block.AddEdge(b, e);
			Block.AddEdge(b, t);
			ProgramDataFlow pdf = new ProgramDataFlow();
			pdf[t] = new BlockFlow(t, null);
			pdf[e] = new BlockFlow(e, null);

			TrashedRegisterFinder trf = new TrashedRegisterFinder(null, pdf);
			trf.TrashedRegisters[r1.Storage] = "trash";
			trf.TrashedRegisters[r2.Storage] = r3.Storage;

			pdf[e].TrashedIn[r1.Storage] = r2.Storage;
			pdf[e].TrashedIn[r2.Storage] = r3.Storage;

			pdf[t].TrashedIn[r1.Storage] = "TrAsH";
			pdf[t].TrashedIn[r2.Storage] = r2.Storage;

			trf.PropagateToSuccessorBlocks(b);
			Assert.AreEqual("TRASH", pdf[e].TrashedIn[r1.Storage].ToString(), "trash & r2 => trash");
			Assert.AreEqual("r3", TrashToString(pdf[e].TrashedIn[r2.Storage]), "r3 & r3 => r3");


			Assert.AreEqual("TRASH", pdf[t].TrashedIn[r1.Storage].ToString(), "trash & trash => trash");
			Assert.AreEqual("TRASH", pdf[t].TrashedIn[r2.Storage].ToString(), "r3 & r2 => trash");
		}

		[Test]
		public void PropagateToProcedureSummary()
		{
			Procedure proc = new Procedure("proc", new Frame(PrimitiveType.Word32));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier ebx = proc.Frame.EnsureRegister(Registers.ebx);
			Identifier ecx = proc.Frame.EnsureRegister(Registers.ecx);
			Identifier esi = proc.Frame.EnsureRegister(Registers.esi);
			ProgramDataFlow flow = new ProgramDataFlow();
			flow[proc] = new ProcedureFlow(proc, prog.Architecture);
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			trf.TrashedRegisters[eax.Storage] = eax.Storage;			// preserved
			trf.TrashedRegisters[ebx.Storage] = ecx.Storage;			// trashed
			trf.TrashedRegisters[esi.Storage] = "@#$@";				// trashed
			trf.PropagateToProcedureSummary(proc);
			ProcedureFlow pf = flow[proc];
			Assert.AreEqual(" ebx esi", pf.EmitRegisters(prog.Architecture, "", pf.TrashedRegisters));
			Assert.AreEqual(" eax", pf.EmitRegisters(prog.Architecture, "", pf.PreservedRegisters));
		}

		[Test]
		public void PropagateFlagsToProcedureSummary()
		{
			Procedure proc = new Procedure("proc", new Frame(PrimitiveType.Word32));
			MachineFlags flags = prog.Architecture.GetFlagGroup("SZ");
			Identifier sz = m.Frame.EnsureFlagGroup(flags.FlagGroupBits, flags.Name, flags.DataType);
			ProgramDataFlow flow = new ProgramDataFlow();
			flow[proc] = new ProcedureFlow(proc, prog.Architecture);
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			Statement stm = m.Assign(sz, m.Int32(3));
			stm.Instruction.Accept(trf);
			trf.PropagateToProcedureSummary(proc);
			Assert.AreEqual(" SZ" + Environment.NewLine, flow[proc].EmitFlagGroup(prog.Architecture, "", flow[proc].grfTrashed));

		}

		[Test]
		public void PreserveEbp()
		{
			Identifier ebp = m.Frame.EnsureRegister(Registers.ebp);
			Identifier loc = m.Frame.EnsureStackLocal(-1, PrimitiveType.Word32);
			m.Assign(loc, ebp);
			m.Assign(ebp, m.LoadDw(m.Int32(0x12345678)));
			m.Assign(ebp, loc);
			m.Return();

			Procedure proc = m.Procedure;
			proc.RenumberBlocks();
			prog.Procedures.Add(new Address(0x10000),  proc);
			ProgramDataFlow flow = new ProgramDataFlow(prog);
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			trf.Compute();
			ProcedureFlow pf = flow[proc];
			Assert.AreEqual(" ebp", pf.EmitRegisters(prog.Architecture, "", pf.PreservedRegisters));
		}

		[Test]
		public void ProcessBlock()
		{
			Identifier eax = m.Procedure.Frame.EnsureRegister(Registers.eax);
			Identifier tmp = m.Local32("tmp");
			m.Assign(tmp, eax);
			m.Assign(eax, m.Int32(3));
			m.Assign(eax, tmp);
			
			ProgramDataFlow flow = new ProgramDataFlow(prog);
			flow[m.Block] = new BlockFlow(m.Block, prog.Architecture.CreateRegisterBitset());
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			trf.ProcessBlock(m.Block);
			Assert.AreEqual("(Local -0004:eax), (eax:eax)", DumpRegs(trf.TrashedRegisters));
		}

		[Ignore("Disabled until Identifier/Variable story works out")]
		[Test]
		public void TwoProcedures()
		{
			ProgramMock p = new ProgramMock();
		
			Identifier eax = m.Frame.EnsureRegister(Registers.eax);
			Identifier tmp = m.Local32("tmp");
			m.Assign(tmp, eax);
			m.Call("TrashEaxEbx");
			m.Assign(eax, tmp);
			m.Return();
			m.Procedure.RenumberBlocks();
			p.Add(m);

		
			ProcedureMock TrashEaxEbx = new ProcedureMock("TrashEaxEbx");
			eax = TrashEaxEbx.Frame.EnsureRegister(Registers.eax);
			Identifier ebx = TrashEaxEbx.Frame.EnsureRegister(Registers.ebx);
			TrashEaxEbx.Assign(ebx, m.Int32(0x1231313));
			TrashEaxEbx.Assign(eax, m.LoadDw(ebx));
			TrashEaxEbx.Return();
			TrashEaxEbx.Procedure.RenumberBlocks();
			p.Add(TrashEaxEbx);

			IProcessorArchitecture arch = prog.Architecture;
			prog = p.BuildProgram();
			prog.Architecture = arch;
			ProgramDataFlow flow = new ProgramDataFlow(prog);
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			trf.Compute();

			StringBuilder sb = new StringBuilder();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				sb.Append(flow[proc].EmitRegisters(prog.Architecture, proc.Name, flow[proc].TrashedRegisters));
				sb.Append(Environment.NewLine);
			}
			string exp = @"ProcedureMock ebx bx bl bh
TrashEaxEbx eax ebx ax bx al bl ah bh
";
			Assert.AreEqual(exp, sb.ToString());

		}

		private string DumpRegs(Hashtable tbl)
		{
			SortedList l = new SortedList(tbl, new HashComparer());
			StringBuilder sb = new StringBuilder();
			string sep = "";
			foreach (DictionaryEntry de in l)
			{
				sb.Append(sep);
				sb.AppendFormat("({0}:{1})", TrashToString(de.Key), TrashToString(de.Value));
				sep = ", ";
			}
			return sb.ToString();
		}

		private string TrashToString(object o)
		{
			RegisterStorage rst = o as RegisterStorage;
			if (rst != null)
				return rst.Register.Name;
			return o.ToString();
		}

		private class HashComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				return String.Compare(x.ToString(), y.ToString());
			}

			#endregion
		}

	}
}
