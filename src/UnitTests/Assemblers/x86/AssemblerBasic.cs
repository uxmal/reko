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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Core.Services;
using Reko.Core.Output;

namespace Reko.UnitTests.Assemblers.x86
{
	public class AssemblerBase
	{
		protected IntelArchitecture arch;
		protected X86TextAssembler asm;
        protected ServiceContainer sc;

        [SetUp]
		public void Setup()
		{
            this.sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
            arch = new X86ArchitectureReal("x86-real-16");
            asm = new X86TextAssembler(sc, arch);
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
			Program program;
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                program = asm.Assemble(addrBase, rdr);
            }
            foreach (var item in asm.ImportReferences)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }

			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Dumper dumper = new Dumper(program);
				dumper.ShowAddresses = true;
				dumper.ShowCodeBytes = true;
                foreach (var segment in program.SegmentMap.Segments.Values)
                {
                    var mem = segment.MemoryArea;
                    var formatter = new TextFormatter(fut.TextWriter);
                    dumper.DumpData(program.SegmentMap, program.Architecture, mem.BaseAddress, mem.Length, formatter);
                    fut.TextWriter.WriteLine();
                    dumper.DumpAssembler(program.SegmentMap, program.Architecture, mem.BaseAddress, mem.EndAddress, formatter);
                    if (program.ImportReferences.Count > 0)
                    {
                        foreach (var de in program.ImportReferences.OrderBy(d => d.Key))
                        {
                            fut.TextWriter.WriteLine("{0:X8}: {1}", de.Key, de.Value);
                        }
                    }
                }
				fut.AssertFilesEqual();
			}
		}
	}

	[TestFixture]
	public class AssemblerBasic : AssemblerBase
	{
        private MemoryArea mem;
        private Program program;

        private void AssembleFragment(string asmSrc)
        {
            program = asm.AssembleFragment(Address.SegPtr(0x0C00, 0), asmSrc);
            mem = program.SegmentMap.Segments.Values.First().MemoryArea;
        }

		[Test]
		public void AsFragment()
		{
			 AssembleFragment(
@"		.i86
hello	proc
		mov	ax,0x30
		mov	bx,0x40
hello	endp
");
            var segment = program.SegmentMap.Segments.Values.First();
			using (FileUnitTester fut = new FileUnitTester("Intel/AsFragment.txt"))
			{
				var arch = new X86ArchitectureReal("x86-real-16");
				var d = new Dumper(program);
				d.DumpData(program.SegmentMap, arch, segment.Address, segment.ContentSize, new TextFormatter(fut.TextWriter));
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void AssembleLoopFragment()
		{
			AssembleFragment(
				@"		.i86
hello	proc
		xor		ax,ax
		mov		cx,10

l:		add		ax,cx
		loop	l

		ret
hello	endp
");
			Assert.IsTrue(Compare(mem.Bytes, new byte [] 
				{ 0x33, 0xC0, 0xB9, 0x0a, 0x00, 0x03, 0xC1, 0xE2, 0xFC, 0xC3 }));

		}

		[Test]
		public void Extensions()
		{
			AssembleFragment(
				@"		.i86
hello	proc
		mov cl,0x3
		movzx eax,cl
		movsx ecx,cx
		movsx cx, byte ptr [bp+43]
		ret
hello   endp
");
			Assert.IsTrue(Compare(mem.Bytes, new byte[]
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
			AssembleFragment(
				@"	.i86
foo		proc
		rol	ax,cl
		rol	byte ptr [bx+2],1
		rcr	word ptr [bp+4],4
		ret
foo		endp
");
			Assert.IsTrue(Compare(mem.Bytes, new byte []
					{ 0xD3, 0xC0, 0xD0, 0x47, 0x02, 0xC1, 0x5E, 0x4, 0x4, 0xC3}));
		}
		
		[Test]
		public void Shifts()
		{
			AssembleFragment(
				@"	.i86
foo		proc
		shl eax,cl
		shr byte ptr [si+3],1
		sar word ptr [si+4],6
		ret
foo		endp
");
			Assert.IsTrue(Compare(mem.Bytes, new byte []
				{ 0x66, 0xD3, 0xE0, 0xD0, 0x6C, 0x03, 0xC1, 0x7C, 0x06, 0x04, 0xC3 }));
		}

		[Test]
		public void StringInstruction()
		{
			AssembleFragment(
				@"	.i86
foo		proc
		mov	si,0x1234
		mov	di,0x3241
		mov	cx,0x32
		rep movsb
		ret
foo		endp
");
			Assert.IsTrue(Compare(mem.Bytes, new byte []
				{ 0xBE, 0x34, 0x12, 0xBF, 0x41, 0x32, 0xB9, 0x32, 0x00, 0xF3, 0xA4, 0xC3 }));
		}

		[Test]
		public void AsCarryInstructions()
		{
            Program program;
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath("Fragments/carryinsts.asm")))
            {
			    program = asm.Assemble(Address.SegPtr(0xBAC, 0), rdr);
            }
			using (FileUnitTester fut = new FileUnitTester("Intel/AsCarryInstructions.txt"))
			{
				Dumper dump = new Dumper(program);
                var arch = program.Architecture;
                var mem = program.SegmentMap.Segments.Values.First().MemoryArea;
				dump.DumpData(program.SegmentMap, arch, mem.BaseAddress, mem.Length, new TextFormatter(fut.TextWriter));
				fut.AssertFilesEqual();
			}
		}

        [Test]
        public void MovMemoryToSegmentRegister()
        {
            AssembleFragment("mov es,[0x4080]\r\n");
            Assert.IsTrue(Compare(mem.Bytes, new byte[] { 0x8E, 0x06, 0x80, 0x40 }));
        }

        [Test]
        public void XchgMem()
        {
            AssembleFragment("xchg word ptr [0x1234],bx\r\n");
            Assert.IsTrue(Compare(mem.Bytes, new byte[] { 0x87, 0x1E, 0x34, 0x12 }));
        }

        [Test]
        public void Fcompp()
        {
            AssembleFragment("fcompp\r\n");
            Assert.AreEqual(new byte[] { 0xDE, 0xD9 }, mem.Bytes);
        }

        [Test]
        public void AsJpo()
        {
            AssembleFragment(
                "jpo label\r\n" +
                "label: xor ax,ax\r\n");
            Assert.AreEqual(new byte[] { 0x7B, 0x00, 0x33, 0xC0 },  mem.Bytes); 
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
            Address addr = Address.SegPtr(0x0C00, 0);
            var program = asm.AssembleFragment(addr, "mov [0x400],0x1234\n");
            var mem = program.SegmentMap.Segments.Values.First().MemoryArea;
            var dasm = new X86Disassembler(
                ProcessorMode.Real,
                mem.CreateLeReader(addr),
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
            Assert.AreEqual("mov\tword ptr [0400],1234", dasm.First().ToString());
        }

        [Test]
        public void AsIntraSegmentFarCall()
        {
            RunTest("Fragments/multiple/intrasegmentfarcall.asm", "Intel/AsIntraSegmentFarCall.txt");
        }

		private void RunTest(string sourceFile, string outputFile)
		{
            Program program;
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(sourceFile)))
            {
                program = asm.Assemble(Address.SegPtr(0x0C00, 0), rdr);
            }
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Dumper dump = new Dumper(program);
                var mem = program.SegmentMap.Segments.Values.First().MemoryArea;
                var formatter = new TextFormatter(fut.TextWriter);
				dump.DumpData(program.SegmentMap, program.Architecture, mem.BaseAddress, mem.Bytes.Length, formatter);
				fut.TextWriter.WriteLine();
				dump.ShowAddresses = true;
				dump.ShowCodeBytes = true;
				dump.DumpAssembler(program.SegmentMap, program.Architecture, mem.BaseAddress, mem.EndAddress, formatter);

				fut.AssertFilesEqual();
			}	
		}
	}
}
