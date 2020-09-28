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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Reko.Core.Serialization;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class OperandRewriterTests
	{
		private OperandRewriter orw;
		private IntelArchitecture arch;
		private X86State state;
		private Procedure proc;
        private Program program;
        private X86Instruction instr;

		[Test]
		public void X86Orw32_Creation()
		{
		}

        [OneTimeSetUp]
		public void GlobalSetup()
		{
			arch = new X86ArchitectureFlat32("x86-protected-32");
		}

		[SetUp]
		public void Setup()
		{
            var mem = new MemoryArea(Address.Ptr32(0x10000), new byte[4]);
            program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(".text", mem, AccessMode.ReadExecute))
            };
            var procAddress = Address.Ptr32(0x10000000);
            instr = new X86Instruction(Mnemonic.nop, InstrClass.Linear,  PrimitiveType.Word32, PrimitiveType.Word32)
            {
                Address = procAddress,
            };
            proc = Procedure.Create(arch, procAddress, arch.CreateFrame());
			state = (X86State) arch.CreateProcessorState();
			orw = new OperandRewriter32(arch, new ExpressionEmitter(), proc.Frame, new FakeRewriterHost(program));
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
			var id = orw.Transform(instr, f, PrimitiveType.Real64,  state);
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
        private Program program;
		private Dictionary<Address,FunctionType> callSignatures;
		private Dictionary<Address,Procedure> procedures;

		public FakeRewriterHost(Program program)
		{
            this.program = program;
			callSignatures = new Dictionary<Address,FunctionType>();
			procedures = new Dictionary<Address,Procedure>();
		}

		public void AddCallSignature(Address addr, FunctionType sig)
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

		public EndianImageReader CreateImageReader(Address addr)
		{
			throw new NotImplementedException();
		}

        public Expression CallIntrinsic(string name, FunctionType fnType, params Expression[] args)
        {
            var ppp = program.EnsurePseudoProcedure(name, fnType);
            return new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, ppp),
                fnType.ReturnValue.DataType,
                args);
        }

        public Expression PseudoProcedure(string name , DataType returnType, params Expression[] args)
        {
            var ppp = program.EnsurePseudoProcedure(name, returnType, args);
            return new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, ppp),
                returnType,
                args);
        }

        public Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
        {
            var ppp = program.EnsurePseudoProcedure(name, returnType, args);
            ppp.Characteristics = c;
            return new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, ppp),
                returnType,
                args);
        }

        public FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
		{
            FunctionType sig;
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

        public Expression GetImport(Address addrTunk, Address addrInstruction)
        {
            return null;
        }

        public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrTunk, Address addrInstruction)
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

		public MemoryArea Image
		{
			get { throw new NotImplementedException(); }
		}

		public SystemService SystemCallAt(Address addr)
		{
			// TODO:  Add FakeRewriterHost.SystemCallAt implementation
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

        public IProcessorArchitecture GetArchitecture(string archLabel)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
        {
            throw new NotImplementedException();
        }

        public void Error(Address address, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(Address address, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
        {
            throw new NotImplementedException();
        }
    }
}
