/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Assemblers.x86
{
	public class AssemblerBase
	{
		protected IntelArchitecture arch;
		protected IntelTextAssembler asm;

		[SetUp]
		public void Setup()
		{
			asm = new IntelTextAssembler();
		}

        protected void AssertEqualBytes(string expected, byte[] actual)
        {
            Assert.AreEqual(expected, Hexize(actual));
        }

        private string Hexize(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                sb.AppendFormat("{0:X2}", (uint) bytes[i]);
            }
            return sb.ToString();
        }

        protected bool Compare(byte[] expected, byte[] actual)
		{
			if (expected.Length != actual.Length)
				return false;
			for (int i = 0; i != expected.Length; ++i)
			{
				if (expected[i] != actual[i])
					return false;
			}
			return true;
		}

		protected void RunTest(string sourceFile, string outputFile, Address addrBase)
		{
			Program prog = new Program();
			asm.Assemble(addrBase, FileUnitTester.MapTestPath(sourceFile));
            prog.Image = asm.Image;
            foreach (KeyValuePair<uint, PseudoProcedure> item in asm.ImportThunks)
            {
                prog.ImportThunks.Add(item.Key, item.Value);
            }

			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Dumper dumper = asm.Architecture.CreateDumper();
				dumper.ShowAddresses = true;
				dumper.ShowCodeBytes = true;
				dumper.DumpData(prog.Image, prog.Image.BaseAddress, prog.Image.Bytes.Length, fut.TextWriter);
				fut.TextWriter.WriteLine();
				dumper.DumpAssembler(prog.Image, prog.Image.BaseAddress, prog.Image.BaseAddress + prog.Image.Bytes.Length, fut.TextWriter);
				if (prog.ImportThunks.Count > 0)
				{
					SortedList<uint, PseudoProcedure> list = new SortedList<uint, PseudoProcedure>(prog.ImportThunks);
					foreach (KeyValuePair<uint, PseudoProcedure> de in list)
					{
						fut.TextWriter.WriteLine("{0:X8}: {1}", de.Key, de.Value);
					}
				}
				fut.AssertFilesEqual();
			}
		}
	}

	[TestFixture]
	public class AssemblerBasic : AssemblerBase
	{

		[Test]
		public void AsFragment()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
@"		.i86
hello	proc
		mov	ax,0x30
		mov	bx,0x40
hello	endp
");
            ProgramImage img = asm.Image;
			using (FileUnitTester fut = new FileUnitTester("Intel/AsFragment.txt"))
			{
				prog.Image = img;
				IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
				prog.Architecture = arch;
				Dumper d = new IntelDumper(arch);
				d.DumpData(prog.Image, img.BaseAddress, img.Bytes.Length, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void AssembleLoopFragment()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),

				@"		.i86
hello	proc
		xor		ax,ax
		mov		cx,10

l:		add		ax,cx
		loop	l

		ret
hello	endp
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte [] 
				{ 0x33, 0xC0, 0xB9, 0x0a, 0x00, 0x03, 0xC1, 0xE2, 0xFC, 0xC3 }));

		}

		[Test]
		public void Extensions()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"		.i86
hello	proc
		mov cl,0x3
		movzx eax,cl
		movsx ecx,cx
		movsx cx, byte ptr [bp+43]
		ret
hello   endp
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte[]
				{
						0xB1, 0x03,
					0x66, 0x0F, 0xB6, 0xC1,
					0x66, 0x0F, 0xBF, 0xC9,
					0x0F, 0xBE, 0x4E, 0x2B,
					0xC3,
			}));
		}
		[Test]
		public void Rotations()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"	.i86
foo		proc
		rol	ax,cl
		rol	byte ptr [bx+2],1
		rcr	word ptr [bp+4],4
		ret
foo		endp
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte []
					{ 0xD3, 0xC0, 0xD0, 0x47, 0x02, 0xC1, 0x5E, 0x4, 0x4, 0xC3}));
		}
		
		[Test]
		public void Shifts()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0x0C00, 0),
				@"	.i86
foo		proc
		shl eax,cl
		shr byte ptr [si+3],1
		sar word ptr [si+4],6
		ret
foo		endp
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte []
				{ 0x66, 0xD3, 0xE0, 0xD0, 0x6C, 0x03, 0xC1, 0x7C, 0x06, 0x04, 0xC3 }));
		}

		[Test]
		public void StringInstruction()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"	.i86
foo		proc
		mov	si,0x1234
		mov	di,0x3241
		mov	cx,0x32
		rep movsb
		ret
foo		endp
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte []
				{ 0xBE, 0x34, 0x12, 0xBF, 0x41, 0x32, 0xB9, 0x32, 0x00, 0xF3, 0xA4, 0xC3 }));
		}

		[Test]
		public void AsCarryInstructions()
		{
			Program prog = new Program();
			asm.Assemble(new Address(0xBAC, 0), FileUnitTester.MapTestPath("Fragments/carryinsts.asm"));
			using (FileUnitTester fut = new FileUnitTester("Intel/AsCarryInstructions.txt"))
			{
				IntelDumper dump = new IntelDumper(arch);
				dump.DumpData(asm.Image, asm.Image.BaseAddress, asm.Image.Bytes.Length, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

        [Test]
        public void MovMemoryToSegmentRegister()
        {
            asm.AssembleFragment(new Address(0x0C00, 0),
                "    mov es,[0x4080]\r\n");
            Assert.IsTrue(Compare(asm.Image.Bytes, new byte[] { 0x8E, 0x06, 0x80, 0x40 }));
        }

        [Test]
        public void XchgMem()
        {
            asm.AssembleFragment(new Address(0x0C00, 0), "xchg word ptr [0x1234],bx\r\n");
            Assert.IsTrue(Compare(asm.Image.Bytes, new byte[] { 0x87, 0x1E, 0x34, 0x12 }));
        }

		[Test]
		public void AsJa()
		{
			RunTest("Fragments/switch.asm", "Intel/AsJa.txt");
		}

		[Test]
		public void AsLargeOffset()
		{
			RunTest("Fragments/largeoffset.asm", "Intel/AsLargeOffset.txt");
		}

		[Test]
		public void AsMultiplication()
		{
			RunTest("Fragments/multiplication.asm", "Intel/AsMultiplication.txt");
		}

		[Test]
		public void AsLoop()
		{
			RunTest("Fragments/loopne.asm", "Intel/AsLoop.txt");
		}

		[Test]
		public void AsLogical()
		{
			RunTest("Fragments/logical.asm", "Intel/AsLogical.txt");
		}

		[Test]
		public void AsTest()
		{
			RunTest("Fragments/test.asm", "Intel/AsTest.txt");
		}

		[Test]
		public void AsEquate()
		{
			RunTest("Fragments/equate.asm", "Intel/AsEquate.txt");
		}

		[Test]
		public void ModRm16Memop()
		{
			MemoryOperand m;
			PrimitiveType w = PrimitiveType.Word16;
			ModRmBuilder mrm = new ModRmBuilder(w, null);
			m = new MemoryOperand(w, Registers.bx, Registers.si, 1, Constant.Invalid);
			Assert.AreEqual(0, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.bx, Registers.di, 1, Constant.Invalid);
			Assert.AreEqual(1, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.bp, Registers.si, 1, Constant.Invalid);
			Assert.AreEqual(2, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.bp, Registers.di, 1, Constant.Invalid);
			Assert.AreEqual(3, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.si, Constant.Invalid);
			Assert.AreEqual(4, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.di, Constant.Invalid);
			Assert.AreEqual(5, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.bp, Constant.Invalid);
			Assert.AreEqual(0x46, mrm.Get16AddressingModeMask(m));
			m = new MemoryOperand(w, Registers.bx, Constant.Invalid);
			Assert.AreEqual(7, mrm.Get16AddressingModeMask(m));
		}

        [Test]
        public void AsConstantStore()
        {
            Address addr = new Address(0x0C00, 0);
            asm.AssembleFragment(addr, "mov [0x400],0x1234\n");
            Disassembler dasm = new IntelDisassembler(asm.Image.CreateReader(addr), PrimitiveType.Word16);
            Assert.AreEqual("mov\tword ptr [0400],1234", dasm.DisassembleInstruction().ToString());
        }

		private void RunTest(string sourceFile, string outputFile)
		{
			Program prog = new Program();
			asm.Assemble(new Address(0x0C00, 0), FileUnitTester.MapTestPath(sourceFile));
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Dumper dump = asm.Architecture.CreateDumper();
				dump.DumpData(asm.Image, asm.Image.BaseAddress, asm.Image.Bytes.Length, fut.TextWriter);
				fut.TextWriter.WriteLine();
				dump.ShowAddresses = true;
				dump.ShowCodeBytes = true;
				dump.DumpAssembler(asm.Image, asm.Image.BaseAddress, asm.Image.BaseAddress + asm.Image.Bytes.Length, fut.TextWriter);

				fut.AssertFilesEqual();
			}	
		}
	}
}
