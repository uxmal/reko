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
using Decompiler.Arch.Intel.Assembler;
using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class DisassemblerTests
	{
		public DisassemblerTests()
		{
		}

		[Test]
		public void DisassembleSequence()
		{
			Program prog = new Program();
			IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(
				prog,
				new Address(0xB96, 0),
				@"	mov	ax,0
	cwd
foo:
	lodsb	
	dec		cx
	jnz		foo
");
			
			IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word16);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i != 5; ++i)
			{
				Address addr = dasm.Address;
				IntelInstruction instr = dasm.Disassemble();
				sb.AppendFormat("{0}\t{1}\r\n", addr, instr.ToString());
			}

			string s = sb.ToString();
			Assert.AreEqual(
				"0B96:0000\tmov\tax,0000\r\n" +
				"0B96:0003\tcwd\t\r\n" +
				"0B96:0004\tlodsb\t\r\n" +
				"0B96:0005\tdec\tcx\r\n" +
				"0B96:0006\tjnz\t0004\r\n",
				s);
		}

		[Test]
		public void SegmentOverrides()
		{
			Program prog = new Program();
			IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(
				prog,
				new Address(0xB96, 0),
				"foo	proc\r\n" +
				"		mov	bx,[bp+4]\r\n" +
				"		mov	ax,[bx+4]\r\n" +
				"		mov cx,cs:[si+4]\r\n");

			IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word16);
			IntelInstruction [] instrs = new IntelInstruction[3];
			for (int i = 0; i != instrs.Length; ++i)
			{
				instrs[i] = dasm.Disassemble();
			}

			Assert.AreEqual(Registers.ss, ((MemoryOperand) instrs[0].op2).DefaultSegment);
			Assert.AreEqual(Registers.ds, ((MemoryOperand) instrs[1].op2).DefaultSegment);
			Assert.AreEqual(Registers.cs, ((MemoryOperand) instrs[2].op2).DefaultSegment);
		}

		[Test]
		public void Rotations()
		{
			Program prog = new Program();
			IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(
				prog,
				new Address(0xB96, 0),
				"foo	proc\r\n" +
				"rol	ax,cl\r\n" +
				"ror	word ptr [bx+2],cl\r\n" +
				"rcr	word ptr [bp+4],4\r\n" +
				"rcl	ax,1\r\n");

			IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word16);
			IntelInstruction [] instrs = new IntelInstruction[4];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i != instrs.Length; ++i)
			{
				instrs[i] = dasm.Disassemble();
				sb.AppendFormat("{0}\r\n", instrs[i].ToString());
			}
			string s = sb.ToString();
			Assert.AreEqual(
				"rol\tax,cl\r\n"+
				"ror\tword ptr [bx+02],cl\r\n"+
				"rcr\tword ptr [bp+04],04\r\n"+
				"rcl\tax,01\r\n", s);

		}

		[Test]
		public void Extensions()
		{
			Program prog = new Program();
			IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(
				prog,
				new Address(0xA14, 0),
@"		.i86
foo		proc
		movsx	ecx,word ptr [bp+8]
		movzx   edx,cl
		movsx	ebx,bx
		movzx	ax,byte ptr [bp+04]
");
			IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word16);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i != 4; ++i)
			{
				IntelInstruction ii = dasm.Disassemble();
				sb.AppendFormat("{0}\r\n", ii.ToString());
			}
			string s = sb.ToString();
			Assert.AreEqual(
@"movsx	ecx,word ptr [bp+08]
movzx	edx,cl
movsx	ebx,bx
movzx	ax,byte ptr [bp+04]
", s);
		}

		[Test]
		public void DisEdiTimes2()
		{
			Program prog = new Program(); IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(prog, new Address(0x0B00, 0),
				@"	.i386
	mov ebx,[edi*2]
");

			IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word32);
			IntelInstruction instr = dasm.Disassemble();
			MemoryOperand mem = (MemoryOperand) instr.op2;
			Assert.AreEqual(2, mem.Scale);
			Assert.AreEqual(Registers.None, mem.Base);
			Assert.AreEqual(Registers.edi,  mem.Index);
		}

		[Test]
		public void DisFpuInstructions()
		{
			using (FileUnitTester fut = new FileUnitTester("Intel/DisFpuInstructions.txt"))
			{
				Program prog = new Program();
				IntelAssembler asm = new IntelAssembler();
				ProgramImage img = asm.Assemble(prog, new Address(0xC32, 0), FileUnitTester.MapTestPath("Fragments/fpuops.asm"), null);
				IntelDisassembler dasm = new IntelDisassembler(img.CreateReader(img.BaseAddress), PrimitiveType.Word16);
				while (img.IsValidAddress(dasm.Address))
				{
					IntelInstruction instr = dasm.Disassemble();
					fut.TextWriter.WriteLine("{0}", instr.ToString());
				}

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void DisSignedByte()
		{
			byte [] image = new byte [] { 0x83, 0xC6, 0x1 };   // add si,+01
			ProgramImage img = new ProgramImage(new Address(0xC00, 0), image);
			ImageReader rdr = img.CreateReader(img.BaseAddress);
			IntelDisassembler dasm = new IntelDisassembler(rdr, PrimitiveType.Word16);
			IntelInstruction instr = dasm.Disassemble();
			Assert.AreEqual("add\tsi,01", instr.ToString());
			Assert.AreEqual(PrimitiveType.Word16, instr.op1.Width);
			Assert.AreEqual(PrimitiveType.Byte, instr.op2.Width);
			Assert.AreEqual(PrimitiveType.Word16, instr.dataWidth);
		}

		[Test]
		public void DisLesBxStackArg()
		{
			byte [] image = new byte [] { 0xC4, 0x5E, 0x6 };		// les bx,[bp+06]
			ProgramImage img = new ProgramImage(new Address(0x0C00, 0), image);
			ImageReader rdr = img.CreateReader(img.BaseAddress);
			IntelDisassembler dasm = new IntelDisassembler(rdr, PrimitiveType.Word16);
			IntelInstruction instr = dasm.Disassemble();
			Assert.AreEqual("les\tbx,[bp+06]", instr.ToString());
			Assert.AreSame(PrimitiveType.Pointer32, instr.op2.Width);
		}

		[Test]
		public void SegFromBits()
		{
			Assert.AreSame(Registers.es, IntelDisassembler.SegFromBits(0));
			Assert.AreSame(Registers.cs, IntelDisassembler.SegFromBits(1));
			Assert.AreSame(Registers.ss, IntelDisassembler.SegFromBits(2));
			Assert.AreSame(Registers.ds, IntelDisassembler.SegFromBits(3));
			Assert.AreSame(Registers.fs, IntelDisassembler.SegFromBits(4));
			Assert.AreSame(Registers.gs, IntelDisassembler.SegFromBits(5));
		}

		[Test]
		public void Bswap()
		{
			byte [] image = new byte[] { 0x0F, 0xC8, 0x0F, 0xCF };		// bswap eax, bswap edi
			ProgramImage img = new ProgramImage(new Address(0x01000000), image);
			ImageReader rdr = img.CreateReader(img.BaseAddress);
			IntelDisassembler dasm = new IntelDisassembler(rdr, PrimitiveType.Word32);
			IntelInstruction instr = dasm.Disassemble();
			Assert.AreEqual("bswap\teax", instr.ToString());
			instr = dasm.Disassemble();
			Assert.AreEqual("bswap\tedi", instr.ToString());
		}
	}
}
