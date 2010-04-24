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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class OperandRewriterTests
	{
		private OperandRewriter orw;
		private IntelArchitecture arch;
		private IntelRewriterState state;
		private Procedure proc;
        private Program prog;

		[Test]
		public void OrwCreation()
		{
		}

		[TestFixtureSetUp]
		public void GlobalSetup()
		{
			arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
		}

		[SetUp]
		public void Setup()
		{
            prog = new Program();
            prog.Image = new ProgramImage(new Address(0x10000), new byte[4]);
            Address procAddress = new Address(0x10000000);
            proc = Procedure.Create(procAddress, arch.CreateFrame());
			state = new IntelRewriterState(proc.Frame);
			orw = new OperandRewriter(new FakeRewriterHost(prog), arch, proc.Frame);
		}

		[Test]
		public void OrwRegister()
		{
			RegisterOperand r = new RegisterOperand(Registers.ebp);
			Identifier id = (Identifier) orw.Transform(r, r.Width, r.Width, state);
			Assert.AreEqual("ebp", id.Name);
			Assert.IsNotNull(proc.Frame.FramePointer);
		}

		[Test]
		public void OrwImmediate()
		{
			ImmediateOperand imm = new ImmediateOperand(Constant.Word16(0x0003));
			Constant c = (Constant) orw.Transform(imm, imm.Width, imm.Width, state);
			Assert.AreEqual("0x0003", c.ToString());
		}

		[Test]
		public void OrwImmediateExtend()
		{
			ImmediateOperand imm = new ImmediateOperand(Constant.SByte(-1));
			Constant c = (Constant) orw.Transform(imm, PrimitiveType.Word16, PrimitiveType.Word16, state);
			Assert.AreEqual("0xFFFF", c.ToString());
		}

		[Test]
		public void OrwOperandAsCodeAddress()
		{
            ImmediateOperand imm = new ImmediateOperand(Constant.Word32(0x100F0000));
			Address addr = orw.OperandAsCodeAddress(imm, null);
			Assert.AreEqual(imm.Value.ToUInt32(), addr.Offset);
		}

		[Test]
		public void OrwFpu()
		{
			FpuOperand f = new FpuOperand(3);
			Identifier id = (Identifier) orw.Transform(f, PrimitiveType.Real64, PrimitiveType.Word32, state);
			Assert.AreEqual(PrimitiveType.Real64, id.DataType);
		}

		[Test]
		public void OrwFrameEscapes()
		{
			Assert.IsFalse(proc.Frame.Escapes);
			MemoryOperand mem = new MemoryOperand(PrimitiveType.Word32);
			mem.Base = Registers.esp;
			mem.Index = Registers.eax;
			mem.Scale = 4;
			mem.Offset = new Constant(PrimitiveType.Byte, 0x02);
			Expression expr = orw.Transform(mem, PrimitiveType.Word32, PrimitiveType.Word32, state);
			Assert.IsTrue(proc.Frame.Escapes);
		}

		[Test]
        [Ignore("Escaping frame registers are ignored for now.")]
		public void OrwEbpEscapes()
		{
			state.FrameRegister = MachineRegister.None;
			proc.Frame.FrameOffset = 0;
			Assert.AreEqual(false, proc.Frame.Escapes);

			RegisterOperand r = new RegisterOperand(Registers.ebp);
			orw.Transform(r, PrimitiveType.Word32, PrimitiveType.Word32, state);
			Assert.AreEqual(false, proc.Frame.Escapes);

			state.GrowStack(4);
			state.EnterFrame(Registers.ebp);
			Assert.AreEqual("fp", proc.Frame.FramePointer.Name);

			MemoryOperand m = new MemoryOperand(PrimitiveType.Word32);
			m.Base = Registers.ebp;
			m.Offset = new Constant(PrimitiveType.Byte, 0x08);
			Expression expr = orw.Transform(m, m.Width, PrimitiveType.Word32, state);
			Assert.AreEqual(false, proc.Frame.Escapes);

			expr = orw.Transform(r, null, null, state);
			Assert.AreEqual(true, proc.Frame.Escapes);
		}

		[Test]
		public void OrwMemAccess()
		{
			MemoryOperand mem = new MemoryOperand(PrimitiveType.Word32);
			mem.Base = Registers.ecx;
			mem.Offset = Constant.Word32(4);
			Expression expr = orw.Transform(mem, PrimitiveType.Word32, PrimitiveType.Word32, state);
			Assert.AreEqual("Mem0[ecx + 0x00000004:word32]", expr.ToString());
		}	

		[Test]
		public void OrwIndexedAccess()
		{
			MemoryOperand mem = new MemoryOperand(PrimitiveType.Word32, Registers.eax, Registers.edx, 4, new Constant(PrimitiveType.Word32, 0x24));
			Expression expr = orw.Transform(mem, PrimitiveType.Word32, PrimitiveType.Word32, state);
			Assert.AreEqual("Mem0[eax + 0x00000024 + edx * 0x00000004:word32]", expr.ToString());
		}

		[Test]
		public void OrwAddrOf()
		{
			Identifier eax = orw.AluRegister(Registers.eax);
			UnaryExpression ptr = orw.AddrOf(eax);
			Assert.AreEqual("&eax", ptr.ToString());
		}

		[Test]
		public void OrwAluRegister()
		{
			Assert.AreEqual("si", orw.AluRegister(Registers.esi, PrimitiveType.Word16).ToString());
		}
	}

	public class FakeRewriterHost : IRewriterHost
	{
        private Program prog;
		private Dictionary<Address,ProcedureSignature> callSignatures;
		private Dictionary<Address,Procedure> procedures;

		public FakeRewriterHost(Program prog)
		{
            this.prog = prog;
			callSignatures = new Dictionary<Address,ProcedureSignature>();
			procedures = new Dictionary<Address,Procedure>();
		}


		public void AddCallSignature(Address addr, ProcedureSignature sig)
		{
			callSignatures[addr] = sig;
		}

		public void AddProcedureAtAddress(Address addr, Procedure proc)
		{
			procedures[addr] = proc;
		}

		#region IRewriterHost Members

		public void AddCallEdge(Procedure caller, Statement stm, Procedure callee)
		{
		}

		public ImageReader CreateImageReader(Address addr)
		{
			throw new NotImplementedException();
		}

		public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int args)
		{
            if (prog == null)
			    throw new NotImplementedException();
            return prog.EnsurePseudoProcedure(name, returnType, args);
		}

		public ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction)
		{
            ProcedureSignature sig;
            if (callSignatures.TryGetValue(addrCallInstruction, out sig))
                return sig;
            else
                return null;
		}


		public Procedure[] GetAddressesFromVector(Address addrCaller, int cbReturnAddress)
		{
			return null;
		}

		public Procedure[] GetProceduresFromVector(Address addrCaller, int cbReturnAddress)
		{
			return new Procedure[0];
		}

		public PseudoProcedure GetImportThunkAtAddress(Address addr)
		{
			return null;
		}

		public Procedure GetProcedureAtAddress(Address addr, int cbStackDepth)
		{
            Procedure proc;
            return procedures.TryGetValue(addr, out proc) ? proc : null;
		}

		public Procedure [] GetProceduresFromVector(Address vectorAddress)
		{
			return new Procedure[0];
		}

		public ProgramImage Image
		{
			get { throw new NotImplementedException(); }
		}

		public SystemService SystemCallAt(Address addr)
		{
			// TODO:  Add FakeRewriterHost.SystemCallAt implementation
			return null;
		}

		public PseudoProcedure TrampolineAt(Address addr)
		{
			return null;
		}

		public VectorUse VectorUseAt(Address addr)
		{
			// TODO:  Add FakeRewriterHost.VectorUseAt implementation
			return null;
		}

		public void AddDiagnostic(Diagnostic d)
		{
			Console.Write(d.GetType().Name);
			Console.Write(" - ");
            Console.WriteLine(d.Address.ToString());
            Console.Write(": ");
			Console.WriteLine(d.Message);
		}
		#endregion

	}

}
