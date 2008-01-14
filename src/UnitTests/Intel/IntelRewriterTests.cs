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
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class IntelRewriterTests
	{
		private IntelArchitecture arch;
		private Procedure proc;
		private CodeEmitter emitter;
		private FakeRewriterHost host;
		private RewriterState state;
		private Program prog;

		public IntelRewriterTests()
		{
			 arch = new IntelArchitecture(ProcessorMode.Real);
		}

		[SetUp]
		public void Setup()
		{
			prog = new Program();
			proc = new Procedure("test", new Frame(PrimitiveType.Word16));
			emitter = new CodeEmitter(prog, proc);
			emitter.Block = new Block(proc, new Address(0xC10, 0x0030));
			host = new FakeRewriterHost();
			state = new RewriterState(proc.Frame);
		}

		[Test]
		public void RewriteIndirectCall()
		{
			IntelInstruction instr = new IntelInstruction();
			instr.code = Opcode.call;
			instr.op1 = new MemoryOperand(PrimitiveType.Word16, Registers.bx, new Value(PrimitiveType.Word16, 4));
			Address addr = new Address(0x0C00, 0x0100);

			FakeRewriterHost host = new FakeRewriterHost();
			host.AddCallSignature(addr, new ProcedureSignature(
				new Identifier(Registers.ax.Name, 0, PrimitiveType.Word16, new RegisterStorage(Registers.ax)),
				new Identifier[] {
					new Identifier(Registers.cx.Name, 0, PrimitiveType.Word16, new RegisterStorage(Registers.cx))
							   }));

			Procedure proc = new Procedure("test", new Frame(arch.WordWidth));
			CodeEmitter emitter = new CodeEmitter(null, proc);
			emitter.Block = new Block(proc, addr);
			IntelRewriter rw = new IntelRewriter(null, proc, host, arch, state, emitter);
			rw.ConvertInstructions(new IntelInstruction[] { instr }, new Address[] { addr }, new FlagM[] { FlagM.CF });
			Assert.AreEqual("ax = Mem0[ds:bx + 0x0004:word16](cx)", emitter.Block.Statements[0].Instruction.ToString());
		}

		[Test]
		public void RewriteLesBxStack()
		{
			IntelInstruction instr = new IntelInstruction();
			instr.code = Opcode.les;
			instr.op1 = new RegisterOperand(Registers.bx);
			instr.op2 = new MemoryOperand(PrimitiveType.Word32, Registers.bp, new Value(PrimitiveType.Word16, 6));

			IntelRewriter rw = new IntelRewriter(null, proc, host, arch, state, emitter);
			state.FrameRegister = Registers.bp;
			ConvertInstructions(rw, instr);
			Assert.AreEqual("es_bx = dwArg06", emitter.Block.Statements.Last.Instruction.ToString());
			Assignment ass = (Assignment) emitter.Block.Statements.Last.Instruction;
			Assert.AreSame(PrimitiveType.Pointer32, ass.Src.DataType);
		}

		[Test]
		public void RewriteBswap()
		{
			IntelInstruction instr = new IntelInstruction();
			instr.code = Opcode.bswap;
			instr.op1 = new RegisterOperand(Registers.ebx);

			IntelRewriter rw = new IntelRewriter(null, proc, host, arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual("ebx = __bswap(ebx)", emitter.Block.Statements.Last.Instruction.ToString());
		}

		[Test]
		public void RewriterNearReturn()
		{
			IntelInstruction instr = new IntelInstruction();
			instr.code = Opcode.ret;

			proc.Frame.ReturnAddressSize = 2;
			IntelRewriter rw = new IntelRewriter(null, proc, host, arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(2, proc.Frame.ReturnAddressSize);
		}


		[Test]
		public void RewriteFiadd()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.fiadd,
				PrimitiveType.Word16,
				PrimitiveType.Word16,
				new MemoryOperand(PrimitiveType.Word16, Registers.bx, Value.Invalid));

			CodeEmitter emitter = new CodeEmitter(null, proc);
			emitter.Block = new Block(proc, "foo");
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual("rArg0 = rArg0 + (real64) Mem0[ds:bx:word16]", emitter.Block.Statements[0].Instruction.ToString());
		}

		/// <summary>
		/// Captures the side effect of setting CF = 0
		/// </summary>
		[Test]
		public void RewriteAnd()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.and, PrimitiveType.Word16, PrimitiveType.Word16,
				new RegisterOperand(Registers.ax),
				new ImmediateOperand(PrimitiveType.Word16, 0x08));
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(3, emitter.Block.Statements.Count);
			Assert.AreEqual("ax = ax & 0x0008", emitter.Block.Statements[0].Instruction.ToString());
			Assert.AreEqual("SCZO = cond(ax)", emitter.Block.Statements[1].Instruction.ToString());
			Assert.AreEqual("C = false", emitter.Block.Statements[2].Instruction.ToString());
		}

		/// <summary>
		/// Captures the side effect of setting CF = 0
		/// </summary>
		[Test]
		public void RewriteTest()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.test, PrimitiveType.Word16, PrimitiveType.Word16,
				new RegisterOperand(Registers.ax),
				new ImmediateOperand(PrimitiveType.Word16, 0x08));
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(2, emitter.Block.Statements.Count);
			Assert.AreEqual("SCZO = cond(ax & 0x0008)", emitter.Block.Statements[0].Instruction.ToString());
			Assert.AreEqual("C = false", emitter.Block.Statements[1].Instruction.ToString());
		}

		[Test]
		public void RewritePushCsCallNear()
		{
			Address addrProc = new Address(0xC00, 0x1234);
			host.AddProcedureAtAddress(addrProc, new Procedure("test", new Frame(PrimitiveType.Word16)));
			state.InstructionAddress = new Address(addrProc.seg, 0);


			IntelInstruction push = new IntelInstruction(
				Opcode.push, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.cs));
			IntelInstruction call = new IntelInstruction(
				Opcode.call, PrimitiveType.Word16, PrimitiveType.Word16, new ImmediateOperand(new Value(PrimitiveType.Word16, addrProc.off)));
			IntelRewriter rw = new IntelRewriter(null, proc, host, arch, state, emitter);
			ConvertInstructions(rw, push, call);
			Assert.AreEqual(1, emitter.Block.Statements.Count);
			Assert.AreEqual("call test (depth: 2;)", emitter.Block.Statements[0].Instruction.ToString());
		}

		[Test]
		public void RewriteImul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.imul, PrimitiveType.Word32, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(2, emitter.Block.Statements.Count);
			Assignment ass = (Assignment) emitter.Block.Statements[0].Instruction;
			Assert.AreEqual("dx_ax = cx *s ax", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("int32", bin.DataType.ToString());
		}

		[Test]
		public void RewriteMul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.mul, PrimitiveType.Word32, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(2, emitter.Block.Statements.Count);
			Assignment ass = (Assignment) emitter.Block.Statements[0].Instruction;
			Assert.AreEqual("dx_ax = cx *u ax", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("uint32", bin.DataType.ToString());
		}

		[Test]
		public void RewriteFmul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.fmul, PrimitiveType.Real64, PrimitiveType.Word16, new FpuOperand(1));
			IntelRewriter rw = new IntelRewriter(null, proc, new FakeRewriterHost(), arch, state, emitter);
			ConvertInstructions(rw, instr);
			Assert.AreEqual(1, emitter.Block.Statements.Count);
			Assignment ass = (Assignment) emitter.Block.Statements[0].Instruction;
			Assert.AreEqual("rArg0 = rArg0 *s rArg1", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("real64", bin.DataType.ToString());
		}

		public void ConvertInstructions(IntelRewriter rw, params IntelInstruction [] instrs)
		{
			Address [] addrs = new Address[instrs.Length];
			for (uint i = 0; i < addrs.Length; ++i)
			{
				addrs[i] = new Address(0xC00, i * 3);
			}
			FlagM [] flags = new FlagM[instrs.Length];
			rw.ConvertInstructions(instrs, addrs, flags);
		}

	}
}
