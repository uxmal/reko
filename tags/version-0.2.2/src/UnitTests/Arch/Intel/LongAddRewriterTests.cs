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

using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class LongAddRewriterTests
	{
		private Frame frame;
		private OperandRewriter orw;
		private LongAddRewriter rw;

		private IntelArchitecture arch;
		private RegisterOperand ax;
		private RegisterOperand dx;
		private RegisterOperand bx;
		private PrimitiveType w16 = PrimitiveType.Word16;
		private IntelInstruction addAxMem;
		private IntelInstruction adcDxMem;
        private CodeEmitter emitter;
        private Block block;

		public LongAddRewriterTests()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
			ax = new RegisterOperand(Registers.ax);
			bx = new RegisterOperand(Registers.bx);
			dx = new RegisterOperand(Registers.dx);
			addAxMem = new IntelInstruction(
				Opcode.add, w16, w16, ax, new MemoryOperand(
				w16, Registers.bx, new Constant(w16, 0x300)));
			adcDxMem = new IntelInstruction(
				Opcode.adc, w16, w16, dx, new MemoryOperand(
				w16, Registers.bx, new Constant(w16, 0x302)));
		}

		[SetUp]
		public void Setup()
		{
            frame = arch.CreateFrame();
			orw = new OperandRewriter(null, arch, frame);
			rw = new LongAddRewriter(frame, orw, null);
            Procedure proc = new Procedure("test", frame);
            block = new Block(proc, "bloke");
            emitter = new CodeEmitter(arch, null, proc, block); 
		}

		[Test]
		public void MatchAddRegMem()
		{
			Assert.IsTrue(rw.Match(addAxMem, adcDxMem));
			Assert.AreEqual("dx_ax", rw.Dst.ToString());
			Assert.AreEqual("Mem0[ds:bx + 0x0300:ui32]", rw.Src.ToString());
		}

		[Test]
		public void MatchAddRecConst()
		{
            IntelInstruction i1 = new IntelInstruction(Opcode.add, w16, w16,
                ax,
                new ImmediateOperand(Constant.Word16(0x5678)));
			IntelInstruction i2 = new IntelInstruction(Opcode.adc, w16, w16,
				dx,
				new ImmediateOperand(Constant.Word16(0x1234)));
			Assert.IsTrue(rw.Match(i1, i2));
			Assert.AreEqual("0x12345678", rw.Src.ToString());
		}


		[Test]
		public void Adc1()
		{
			IntelInstruction in1 = new IntelInstruction(Opcode.add, w16, w16,
				ax, new ImmediateOperand(Constant.Word16(0x0001)));
			IntelInstruction in2 = new IntelInstruction(Opcode.adc, w16, w16,
				dx, new ImmediateOperand(Constant.Byte(0)));
			Assert.IsTrue(rw.Match(in1, in2));
			Assert.AreEqual("dx_ax", rw.Dst.ToString());
			Assert.AreEqual("0x00000001", rw.Src.ToString());
		}

		[Test]
		public void CreateInstruction()
		{
			rw.Match(addAxMem, adcDxMem);
            rw.EmitInstruction(Operator.Add, emitter);
			Assert.AreEqual("dx_ax = dx_ax + Mem0[ds:bx + 0x0300:ui32]", block.Statements[0].ToString());
		}
	}
}
