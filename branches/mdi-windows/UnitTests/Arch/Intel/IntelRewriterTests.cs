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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class IntelRewriterTests
	{
		private IntelArchitecture arch;
		private Procedure proc;
        private FakeRewriterHost host;
        private IntelRewriterState state;
		private Program prog;
		private TestRewriter rw;

		public IntelRewriterTests()
		{
			 arch = new IntelArchitecture(ProcessorMode.Real);
		}

		[SetUp]
		public void Setup()
		{
			prog = new Program();
            prog.Architecture = arch;
            proc = new Procedure("test", arch.CreateFrame());
            host = new FakeRewriterHost(prog);
			state = new IntelRewriterState(proc.Frame);

			rw = new TestRewriter(new FakeProcedureRewriter(arch, host, proc), proc, host, arch, state);
		}

		[Test]
		public void RewriteIndirectCall()
		{
            IntelInstruction instr = new IntelInstruction(
                Opcode.call,
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                new MemoryOperand(PrimitiveType.Word16, Registers.bx, new Constant(PrimitiveType.Word16, 4)));
			Address addr = new Address(0x0C00, 0x0100);

			host.AddCallSignature(addr, new ProcedureSignature(
                Reg(Registers.ax),
				new Identifier[] { Reg(Registers.cx) }));

            Procedure proc = new Procedure("test", arch.CreateFrame());
            rw.ConvertInstructions(instr);
			Assert.AreEqual("ax = SEQ(cs, Mem0[ds:bx + 0x0004:word16])(cx)", rw.Block.Statements[0].Instruction.ToString());
		}

        private Identifier Reg(IntelRegister r)
        {
            return new Identifier(r.Name, 0, r.DataType, new RegisterStorage(r));
        }

		[Test]
		public void RewriteLesBxStack()
		{
            IntelInstruction instr = new IntelInstruction(
                Opcode.les,
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                new RegisterOperand(Registers.bx),
                new MemoryOperand(PrimitiveType.Word32, Registers.bp, new Constant(PrimitiveType.Word16, 6)));

			state.FrameRegister = Registers.bp;
			rw.ConvertInstructions(instr);
			Assert.AreEqual("es_bx = dwArg06", rw.Block.Statements.Last.Instruction.ToString());
			Assignment ass = (Assignment) rw.Block.Statements.Last.Instruction;
			Assert.AreSame(PrimitiveType.Pointer32, ass.Src.DataType);
		}

		[Test]
		public void RewriteBswap()
		{
            IntelInstruction instr = new IntelInstruction(
                Opcode.bswap,
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                new RegisterOperand(Registers.ebx));

			rw.ConvertInstructions(instr);
			Assert.AreEqual("ebx = __bswap(ebx)", rw.Block.Statements.Last.Instruction.ToString());
		}

		[Test]
		public void RewriterNearReturn()
		{
            IntelInstruction instr = new IntelInstruction(
                Opcode.ret,
                PrimitiveType.Word16,
                PrimitiveType.Word16);

			proc.Frame.ReturnAddressSize = 2;
			rw.ConvertInstructions(instr);
			Assert.AreEqual(2, proc.Frame.ReturnAddressSize);
		}


		[Test]
		public void RewriteFiadd()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.fiadd,
				PrimitiveType.Word16,
				PrimitiveType.Word16,
				new MemoryOperand(PrimitiveType.Word16, Registers.bx, Constant.Invalid));

			rw.ConvertInstructions(instr);
			Assert.AreEqual("rArg0 = rArg0 + (real64) Mem0[ds:bx:word16]", rw.Block.Statements[0].Instruction.ToString());
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
                new ImmediateOperand(Constant.Word16(0x08)));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(3, rw.Block.Statements.Count);
			Assert.AreEqual("ax = ax & 0x0008", rw.Block.Statements[0].Instruction.ToString());
			Assert.AreEqual("SCZO = cond(ax)", rw.Block.Statements[1].Instruction.ToString());
			Assert.AreEqual("C = false", rw.Block.Statements[2].Instruction.ToString());
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
				new ImmediateOperand(Constant.Word16(0x08)));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(2, rw.Block.Statements.Count);
			Assert.AreEqual("SCZO = cond(ax & 0x0008)", rw.Block.Statements[0].Instruction.ToString());
			Assert.AreEqual("C = false", rw.Block.Statements[1].Instruction.ToString());
		}

		[Test]
		public void RewritePushCsCallNear()
		{
			Address addrProc = new Address(0xC00, 0x1234);
            host.AddProcedureAtAddress(addrProc, new Procedure("test", arch.CreateFrame()));
			state.InstructionAddress = new Address(addrProc.Selector, 0);

			IntelInstruction push = new IntelInstruction(
				Opcode.push, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.cs));
			IntelInstruction call = new IntelInstruction(
				Opcode.call, PrimitiveType.Word16, PrimitiveType.Word16, new ImmediateOperand(new Constant(PrimitiveType.Word16, addrProc.Offset)));
			rw.ConvertInstructions(push, call);
			Assert.AreEqual(1, rw.Block.Statements.Count);
			Assert.AreEqual("call test (depth: 2;)", rw.Block.Statements[0].Instruction.ToString());
		}

		[Test]
		public void RewriteImul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.imul, PrimitiveType.Word32, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(2, rw.Block.Statements.Count);
			Assignment ass = (Assignment) rw.Block.Statements[0].Instruction;
			Assert.AreEqual("dx_ax = cx *s ax", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("int32", bin.DataType.ToString());
		}

		[Test]
		public void RewriteMul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.mul, PrimitiveType.Word32, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(2, rw.Block.Statements.Count);
			Assignment ass = (Assignment) rw.Block.Statements[0].Instruction;
			Assert.AreEqual("dx_ax = cx *u ax", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("uint32", bin.DataType.ToString());
		}

		[Test]
		public void RewriteFmul()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.fmul, PrimitiveType.Real64, PrimitiveType.Word16, new FpuOperand(1));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(1, rw.Block.Statements.Count);
			Assignment ass = (Assignment) rw.Block.Statements[0].Instruction;
			Assert.AreEqual("rArg0 = rArg0 *s rArg1", ass.ToString());
			BinaryExpression bin = (BinaryExpression) ass.Src;
			Assert.AreEqual("real64", bin.DataType.ToString());
		}

		[Test]
		public void RewriteDivWithRemainder()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.div, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(3, rw.Block.Statements.Count);
			Assignment a2 = (Assignment) rw.Block.Statements[0].Instruction;
			Assert.AreEqual("dx = dx_ax % cx", a2.ToString());
			BinaryExpression mod = (BinaryExpression) a2.Src;
			Assert.AreEqual("uint16", mod.DataType.ToString());
		}

		[Test]
		public void RewriteIdivWithRemainder()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.idiv, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.cx));
			rw.ConvertInstructions(instr);
			Assert.AreEqual(3, rw.Block.Statements.Count);
			Assignment a2 = (Assignment) rw.Block.Statements[0].Instruction;
			Assert.AreEqual("dx = dx_ax % cx", a2.ToString());
			BinaryExpression mod = (BinaryExpression) a2.Src;
			Assert.AreEqual("int16", mod.DataType.ToString());

		}

		[Test]
		public void RewriteBsr()
		{
			IntelInstruction instr = new IntelInstruction(
				Opcode.bsr, PrimitiveType.Word32, PrimitiveType.Word32, new RegisterOperand(Registers.ecx), new RegisterOperand(Registers.eax));
			rw.ConvertInstructions(instr);
//			Assert.AreEqual(1, rw.Block.Statements.Count);
			Assert.AreEqual("Z = eax == 0x00000000", rw.Block.Statements[0].Instruction.ToString());
			Assert.AreEqual("ecx = __bsr(eax)", rw.Block.Statements[1].Instruction.ToString());
		}

        [Test]
        public void RewriteIndirectCalls()
        {
            RegisterOperand bx = new RegisterOperand(Registers.bx);
            rw.ConvertInstructions(new IntelInstruction[] {
                new IntelInstruction(
                    Opcode.call, PrimitiveType.Word16, PrimitiveType.Word16, bx),
                new IntelInstruction(
                    Opcode.call, PrimitiveType.Word16, PrimitiveType.Word16, new MemoryOperand(PrimitiveType.Word16, Registers.bx, new Constant(PrimitiveType.Word16, 0))),
                new IntelInstruction(
                    Opcode.call, PrimitiveType.Word16, PrimitiveType.Word16, new MemoryOperand(PrimitiveType.Pointer32, Registers.bx, new Constant(PrimitiveType.Word16, 0)))
            });
        
            Assert.AreEqual("icall SEQ(cs, bx)", rw.Block.Statements[0].Instruction.ToString());
            Assert.AreEqual("icall SEQ(cs, Mem0[ds:bx + 0x0000:word16])", rw.Block.Statements[1].Instruction.ToString());
            Assert.AreEqual("icall Mem0[ds:bx + 0x0000:ptr32]", rw.Block.Statements[2].Instruction.ToString());
        }

        [Test]
        public void AssignSpToBxShouldMakeBxFramePointer()
        {
            RegisterOperand bx = new RegisterOperand(Registers.bx);
            RegisterOperand sp = new RegisterOperand(Registers.sp);
            rw.ConvertInstructions(
                new IntelInstruction(
                    Opcode.mov, PrimitiveType.Word16, PrimitiveType.Word16, bx, sp));
            Assert.AreSame(Registers.bx, state.FrameRegister);
        }

        [Test]
        public void RewriteJp()
        {
            rw.ConvertInstructions(
                new IntelInstruction(Opcode.jpe, PrimitiveType.Word16, PrimitiveType.Word16, new ImmediateOperand(new Constant(PrimitiveType.Word32, 0x100)))
                );
            Assert.AreEqual("branch Test(PE,P) l0C00_0100", rw.Block.Statements[0].Instruction.ToString());
        }





        public class FakeProcedureRewriter : IProcedureRewriter
        {
            private IProcessorArchitecture arch;
            private IRewriterHost host;
            private Procedure proc;

            public FakeProcedureRewriter(IProcessorArchitecture arch, IRewriterHost host, Procedure proc)
            {
                this.arch = arch;
                this.host = host;
                this.proc = proc;
            }

            #region IProcedureRewriter Members

            public Block RewriteBlock(Address addr, Block prev, Rewriter rewriter)
            {
                return new Block(proc, addr.GenerateName("l", ""));
            }

            public CodeEmitter CreateEmitter(Block block)
            {
                return new CodeEmitter(arch, host, proc, block);
            }

            #endregion
        }


	}

    public class TestRewriter : IntelRewriter
    {
        private FakeRewriterHost host;
        private Block block;
        private CodeEmitter emitter;

        public TestRewriter(IProcedureRewriter prw, Procedure proc, FakeRewriterHost host, IntelArchitecture arch, IntelRewriterState state)
            : base(prw, proc, host, arch, state)
        {
            this.host = host;
            block = proc.AddBlock(new Address(0x0C00, 0x0030).ToString());
            emitter = prw.CreateEmitter(block);
        }

        public Block Block
        {
            get { return block; }
        }

        public void ConvertInstructions(params IntelInstruction[] instrs)
        {
            Address addrStart = new Address(0x0C00, 0x0100);
            List<Address> addrs = new List<Address>();
            for (int i = 0; i < instrs.Length; i++)
            {
                addrs.Add(addrStart + i * 3);
            }
            base.ConvertInstructions(instrs, addrs.ToArray(), new uint[instrs.Length], addrStart + instrs.Length * 3, emitter);
        }

        public FakeRewriterHost Host
        {
            get { return host; }
        }
    }

}
