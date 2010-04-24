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
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class RegisterLivenessUnitTests
	{
		private Program prog;
		private Procedure proc;
		private Frame f;
		private ProgramDataFlow mpprocflow;
		private RegisterLiveness rl;
		private ProcedureMock m;

		[SetUp]
		public void Setup()
		{
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
			m = new ProcedureMock();
			proc = m.Procedure;
			f = proc.Frame;
			mpprocflow = new ProgramDataFlow();
			rl = new RegisterLiveness(prog, mpprocflow, null);
			rl.Procedure = proc;
			rl.IdentifierLiveness.BitSet = prog.Architecture.CreateRegisterBitset();

		}

		/// <summary>
		/// Tests using only part of a register.
		/// </summary>
		[Test]
		public void SliceRegister()
		{
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);

			m.Store(m.Int32(0x01F3004), ax).Instruction.Accept(rl);
			Assert.AreEqual(" ax", Dump(rl.IdentifierLiveness));
			m.Assign(eax, ecx).Instruction.Accept(rl);
			Assert.AreEqual(16, rl.IdentifierLiveness.DefBitSize);
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		/// <summary>
		/// In this test, the assignment of eax = ecx is followed by uses of al, ah. This implies 
		/// that cx, not ecx should be live.
		/// </summary>
		[Test]
		public void AlAhUses()
		{
			Identifier al = f.EnsureRegister(Registers.al);
			Identifier ah = f.EnsureRegister(Registers.ah);
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);

			m.Store(m.Int32(0x01F3004), al).Instruction.Accept(rl);
			Assert.AreEqual(" al", Dump(rl.IdentifierLiveness));
			m.Store(m.Int32(0x01F3008), ah).Instruction.Accept(rl);	
			Assert.AreEqual(" al ah", Dump(rl.IdentifierLiveness));
			m.Assign(eax, ecx).Instruction.Accept(rl);		
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		/// <summary>
		/// The assignment of ah = ah + 0x7 is followed by uses of al and ah. the result should still be
		/// that ah and al are live.
		/// </summary>
		[Test]
		public void AlAhUses2()
		{
			Identifier al = f.EnsureRegister(Registers.al);
			Identifier ah = f.EnsureRegister(Registers.ah);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int32(0x01F3004), ax).Instruction.Accept(rl);	// use al and ah
			Assert.AreEqual(" ax", Dump(rl.IdentifierLiveness));
			m.Assign(ah, m.Add(ah, 3)).Instruction.Accept(rl);
			Assert.AreEqual(" al ah", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void MemLoad()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier eax = f.EnsureRegister(Registers.eax);

			m.Store(m.Int32(0x01F0300), ax).Instruction.Accept(rl);			// force ax to be live.
			m.Assign(ax, m.Load(ax.DataType, eax)).Instruction.Accept(rl);	// eax should be live in here.
			Assert.AreEqual(" eax", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Shift()
		{
			Identifier cl = f.EnsureRegister(Registers.cl);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int16(0x01F0300), ax).Instruction.Accept(rl);			// force ax to be live.
			m.Assign(ax, m.Shl(ax, cl)).Instruction.Accept(rl);				// ax, cl should be live in.
			Assert.AreEqual(" ax cl", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void CopyDeadRegister()
		{
			Identifier bx = f.EnsureRegister(Registers.bx);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Assign(ax, bx).Instruction.Accept(rl);
			Assert.AreEqual("", Dump(rl.IdentifierLiveness), "No identifiers should be live since ax is dead");
		}

		[Test]
		public void CopyLiveRegister()
		{
			Identifier bx = f.EnsureRegister(Registers.bx);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int32(0x12341234), ax).Instruction.Accept(rl);
			m.Assign(ax, bx).Instruction.Accept(rl);
			Assert.AreEqual(" bx", Dump(rl.IdentifierLiveness), "bx should be live since ax was stored");
		}

		[Test]
		public void Locals()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);
			Identifier loc = f.EnsureStackLocal(-8, PrimitiveType.Word32);

			m.Store(m.Int32(0x01DFDF), ax).Instruction.Accept(rl);
			m.Assign(ax, loc).Instruction.Accept(rl);
			Assert.AreEqual(" Local -0008", Dump(rl.IdentifierLiveness));
			m.Assign(loc, ecx).Instruction.Accept(rl);
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void NarrowedStackParameters()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier arg = f.EnsureStackArgument(4, PrimitiveType.Word32);

			m.Store(m.Int32(0x102343), ax).Instruction.Accept(rl);
			m.Assign(eax, arg).Instruction.Accept(rl);
			Assert.AreEqual(16, rl.IdentifierLiveness.LiveStorages[arg.Storage]);
		}

		[Test]
		public void PushPop()
		{
			Identifier bp = f.EnsureRegister(Registers.bp);
			Identifier loc = f.EnsureStackLocal(-4, PrimitiveType.Word16);

			m.Assign(bp, loc).Instruction.Accept(rl);
			m.Assign(loc, bp).Instruction.Accept(rl);
			Assert.AreEqual("", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void PushPopLiveBp()
		{
			Identifier bp = f.EnsureRegister(Registers.bp);
			Identifier loc = f.EnsureStackLocal(-4, PrimitiveType.Word16);

			m.Assign(bp, loc).Instruction.Accept(rl);
			m.Store(m.Int32(0x12345678), bp).Instruction.Accept(rl);
			m.Assign(loc, bp).Instruction.Accept(rl);
			Assert.AreEqual(" bp", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void CallToProcedureWithValidSignature()
		{
			Procedure callee = new Procedure("callee", null);
			callee.Signature = new ProcedureSignature(
				new Identifier("eax", -1, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
				new Identifier[] {
					new Identifier("ebx", -1, PrimitiveType.Word32, new RegisterStorage(Registers.ebx)),
					new Identifier("ecx", -1, PrimitiveType.Word32, new RegisterStorage(Registers.ecx)),
					new Identifier("edi", -1, PrimitiveType.Word32, new OutArgumentStorage(
						new Identifier("edi", -1, PrimitiveType.Word32, new RegisterStorage(Registers.edi))))
								 });
			
			rl.IdentifierLiveness.BitSet[Registers.eax.Number] = true;
			rl.IdentifierLiveness.BitSet[Registers.esi.Number] = true;
			rl.IdentifierLiveness.BitSet[Registers.edi.Number] = true;
			CallInstruction ci = new CallInstruction(new ProcedureConstant(PrimitiveType.Pointer32, callee), new CallSite(0, 0));
			rl.VisitCallInstruction(ci);
			Assert.AreEqual(" ecx ebx esi", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void CallToProcedureWithStackArgs()
		{
			Procedure callee = new Procedure("callee", null);
			BitSet trash = prog.Architecture.CreateRegisterBitset();
			callee.Signature = new ProcedureSignature(
				new Identifier("eax", -1, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
				new Identifier[] { new Identifier("arg04", -1, PrimitiveType.Word16, new StackArgumentStorage(4, PrimitiveType.Word16)),
								   new Identifier("arg08", -1, PrimitiveType.Byte, new StackArgumentStorage(8, PrimitiveType.Byte)) });

			Identifier b04 = m.Frame.EnsureStackLocal(-4, PrimitiveType.Word32);
			Identifier w08 = m.Frame.EnsureStackLocal(-8, PrimitiveType.Word32);
			new CallInstruction(new ProcedureConstant(PrimitiveType.Pointer32, callee), new CallSite(0x0C, 0)).Accept(rl);
			Assert.AreEqual(2, rl.IdentifierLiveness.LiveStorages.Count, "Should have two accesses");
			foreach (object o in rl.IdentifierLiveness.LiveStorages.Keys)
			{
				Console.WriteLine("{0} {1} {2}",o, Object.Equals(o, b04.Storage), Object.Equals(o, b04.Storage));
			}

			Assert.IsTrue(rl.IdentifierLiveness.LiveStorages.ContainsKey(b04.Storage), "Should have storage for b04");
			Assert.IsTrue(rl.IdentifierLiveness.LiveStorages.ContainsKey(w08.Storage), "Should have storage for w08");

		}

		[Test]
		public void MarkLiveStackParameters()
		{
            Procedure callee = new Procedure("callee", prog.Architecture.CreateFrame());
			callee.Frame.ReturnAddressSize = 4;
			callee.Frame.EnsureStackArgument(0, PrimitiveType.Word32);
			callee.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
			Assert.AreEqual(8, callee.Frame.GetStackArgumentSpace());
			ProcedureFlow pf = new ProcedureFlow(callee, prog.Architecture);
			mpprocflow[callee] = pf;

			Identifier loc08 = m.Frame.EnsureStackLocal(-8, PrimitiveType.Word32);
			Identifier loc0C = m.Frame.EnsureStackLocal(-12, PrimitiveType.Word32);
			Identifier loc10 = m.Frame.EnsureStackLocal(-16, PrimitiveType.Word32);
			rl.CurrentState = new RegisterLiveness.ByPassState();
			CallInstruction ci = new CallInstruction(new ProcedureConstant(PrimitiveType.Pointer32, callee), new CallSite(16, 0));
			rl.Procedure = m.Procedure;
			rl.MarkLiveStackParameters(ci);
			Assert.AreEqual(" Local -000C Local -0010", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void PredefinedSignature()
		{
			Procedure callee = new Procedure("callee", null);
			Identifier edx = new Identifier("edx", -1, PrimitiveType.Word32, new RegisterStorage(Registers.edx));
			callee.Signature = new ProcedureSignature(
				new Identifier("eax", -1, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
				new Identifier[] { new Identifier("ecx", -1, PrimitiveType.Word32, new RegisterStorage(Registers.ecx)),
								   new Identifier("arg04", -1, PrimitiveType.Word16, new StackArgumentStorage(4, PrimitiveType.Word16)),
								   new Identifier("edxOut", -1, PrimitiveType.Word32, new OutArgumentStorage(edx))});

			RegisterLiveness.State st = new RegisterLiveness.ByPassState();
			BlockFlow bf = new BlockFlow(callee.ExitBlock, prog.Architecture.CreateRegisterBitset());
			mpprocflow[callee.ExitBlock] = bf;
			st.InitializeBlockFlow(callee.ExitBlock, mpprocflow, true);
			Assert.IsTrue(bf.DataOut[Registers.eax.Number],"eax is a return register");
			Assert.IsTrue(bf.DataOut[Registers.edx.Number],"edx is an out register");
			Assert.IsTrue(bf.DataOut[Registers.ax.Number], "ax is aliased by eax");
			Assert.IsFalse(bf.DataOut[Registers.ecx.Number], "ecx is an in register");
			Assert.IsFalse(bf.DataOut[Registers.esi.Number], "esi is not present in signature");
		}

		/// <summary>
		/// We assume preserved registers are _never_ live out.
		/// </summary>
		[Test]
		public void ProcedureWithTrashedAndPreservedRegisters()
		{
            Procedure proc = new Procedure("test", prog.Architecture.CreateFrame());
			ProcedureFlow pf = new ProcedureFlow(proc, prog.Architecture);
			mpprocflow[proc] = pf;
			pf.TrashedRegisters[Registers.eax.Number] = true;
			pf.TrashedRegisters[Registers.ebx.Number] = true;
			pf.PreservedRegisters[Registers.ebp.Number] = true;
			pf.PreservedRegisters[Registers.bp.Number] = true;

			
			RegisterLiveness.State st = new RegisterLiveness.ByPassState();
			BlockFlow bf = new BlockFlow(proc.ExitBlock, prog.Architecture.CreateRegisterBitset());
			mpprocflow[proc.ExitBlock] = bf;
			st.InitializeBlockFlow(proc.ExitBlock, mpprocflow, true);
			Assert.IsFalse(bf.DataOut[Registers.ebp.Number], "preserved registers cannot be live out");
			Assert.IsFalse(bf.DataOut[Registers.bp.Number], "preserved registers cannot be live out");
			Assert.IsTrue(bf.DataOut[Registers.eax.Number], "trashed registers may be live out");
			Assert.IsTrue(bf.DataOut[Registers.esi.Number], "Unmentioned registers may be live out");
		}

		[Ignore("Decide how to implement this")]
		[Test]
		public void TerminatingProcedure()
		{
			Procedure terminator = new Procedure("terminator", null);
			mpprocflow[terminator.ExitBlock] = new BlockFlow(terminator.ExitBlock, prog.Architecture.CreateRegisterBitset());
			terminator.Signature = new ProcedureSignature(
				null,
				new Identifier[] {
					new Identifier("eax", -1, PrimitiveType.Word32, new RegisterStorage(Registers.eax)) });
			terminator.Characteristics = new ProcedureCharacteristics();
			terminator.Characteristics.Terminates = true;
			rl.IdentifierLiveness.BitSet[Registers.eax.Number] = true;
			rl.IdentifierLiveness.BitSet[Registers.ebx.Number] = true;
			rl.MergeBlockInfo(terminator.ExitBlock);
			Assert.IsFalse(rl.IdentifierLiveness.BitSet[Registers.eax.Number]);
			Assert.IsFalse(rl.IdentifierLiveness.BitSet[Registers.ebx.Number]);

		}

		private string Dump(IdentifierLiveness vl)
		{
			StringWriter sw = new StringWriter();
			vl.Write(sw, "");
			SortedList sl = new SortedList();
			foreach (object o in vl.LiveStorages.Keys)
			{
				string s = o.ToString();
				sl.Add(s, s);
			}
			foreach (string s in sl.Keys)
			{
				sw.Write(" ");
				sw.Write(s);
			}
			return sw.ToString();
		}	
	}
}
