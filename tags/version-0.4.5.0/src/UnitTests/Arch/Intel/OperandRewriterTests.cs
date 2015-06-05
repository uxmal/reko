#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
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
		private X86State state;
		private Procedure proc;
        private Program prog;
        private IntelInstruction instr;

		[Test]
		public void X86Orw32_Creation()
		{
		}

		[TestFixtureSetUp]
		public void GlobalSetup()
		{
			arch = new IntelArchitecture(ProcessorMode.Protected32);
		}

		[SetUp]
		public void Setup()
		{
            prog = new Program();
            prog.Image = new LoadedImage(Address.Ptr32(0x10000), new byte[4]);
            var procAddress = Address.Ptr32(0x10000000);
            instr = new IntelInstruction(Opcode.nop, PrimitiveType.Word32, PrimitiveType.Word32)
            {
                Address = procAddress,
            };
            proc = Procedure.Create(procAddress, arch.CreateFrame());
			state = (X86State) arch.CreateProcessorState();
			orw = new OperandRewriter32(arch, proc.Frame, new FakeRewriterHost(prog));
		}

		[Test]
		public void X86Orw32_Register()
		{
			var r = new RegisterOperand(Registers.ebp);
			var id = (Identifier) orw.Transform(null, r, r.Width, state);
			Assert.AreEqual("ebp", id.Name);
			Assert.IsNotNull(proc.Frame.FramePointer);
		}

		[Test]
		public void X86Orw32_Immediate()
		{
			var imm = new ImmediateOperand(Constant.Word16(0x0003));
			var c = (Constant) orw.Transform(null, imm, imm.Width, state);
			Assert.AreEqual("0x0003", c.ToString());
		}

		[Test]
		public void X86Orw32_ImmediateExtend()
		{
			var imm = new ImmediateOperand(Constant.SByte(-1));
			var c = (Constant) orw.Transform(null, imm, PrimitiveType.Word16, state);
			Assert.AreEqual("0xFFFF", c.ToString());
		}

		[Test]
		public void X86Orw32_Fpu()
		{
			FpuOperand f = new FpuOperand(3);
			Identifier id = (Identifier) orw.Transform(instr, f, PrimitiveType.Real64,  state);
			Assert.AreEqual(PrimitiveType.Real64, id.DataType);
		}

		[Test]
        public void X86Orw32_MemAccess()
		{
			MemoryOperand mem = new MemoryOperand(PrimitiveType.Word32);
			mem.Base = Registers.ecx;
			mem.Offset = Constant.Word32(4);
			Expression expr = orw.Transform(instr, mem, PrimitiveType.Word32, state);
			Assert.AreEqual("Mem0[ecx + 0x00000004:word32]", expr.ToString());
		}	

		[Test]
		public void X86Orw32_IndexedAccess()
		{
			MemoryOperand mem = new MemoryOperand(PrimitiveType.Word32, Registers.eax, Registers.edx, 4, Constant.Word32(0x24));
			Expression expr = orw.Transform(instr, mem, PrimitiveType.Word32, state);
			Assert.AreEqual("Mem0[eax + 0x00000024 + edx * 0x00000004:word32]", expr.ToString());
		}

		[Test]
		public void X86Orw32_AddrOf()
		{
			Identifier eax = orw.AluRegister(Registers.eax);
			UnaryExpression ptr = orw.AddrOf(eax);
			Assert.AreEqual("&eax", ptr.ToString());
		}

		[Test]
		public void X86Orw32_AluRegister()
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

        public Expression PseudoProcedure(string name , DataType returnType, params Expression[] args)
        {
            var ppp = prog.EnsurePseudoProcedure(name, returnType, args.Length);
            return new Application(
                new ProcedureConstant(PrimitiveType.Pointer32, ppp),
                returnType,
                args);
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

		public ExternalProcedure GetImportedProcedure(Address addrTunk, Address addrInstruction)
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

		public LoadedImage Image
		{
			get { throw new NotImplementedException(); }
		}

		public SystemService SystemCallAt(Address addr)
		{
			// TODO:  Add FakeRewriterHost.SystemCallAt implementation
			return null;
		}

		public VectorUse VectorUseAt(Address addr)
		{
			// TODO:  Add FakeRewriterHost.VectorUseAt implementation
			return null;
		}

        public void AddDiagnostic(Address addr, Diagnostic d)
        {
            Console.Write(d.GetType().Name);
            Console.Write(" - ");
            Console.WriteLine(addr.ToString());
            Console.Write(": ");
            Console.WriteLine(d.Message);
        }

		#endregion



        public ExternalProcedure GetInterceptedCall(Address addrImportThunk)
        {
            throw new NotImplementedException();
        }


        public void Error(Address address, string message)
        {
            throw new NotImplementedException();
        }
    }

}
