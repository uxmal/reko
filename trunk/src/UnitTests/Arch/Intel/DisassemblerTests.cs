#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using System;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    public class DisassemblerTests
    {
        private X86Disassembler dasm;

        public DisassemblerTests()
        {
        }

        private IntelInstruction Disassemble16(params byte[] bytes)
        {
            LoadedImage img = new LoadedImage(new Address(0xC00, 0), bytes);
            ImageReader rdr = img.CreateReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
            return dasm.Disassemble();
        }

        private IntelInstruction Disassemble32(params byte[] bytes)
        {
            var img = new LoadedImage(new Address(0x10000), bytes);
            var rdr = img.CreateReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            return dasm.Disassemble();
        }

        private IntelInstruction Disassemble64(params byte[] bytes)
        {
            var img = new LoadedImage(new Address(0x10000), bytes);
            var rdr = img.CreateReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word64, true);
            return dasm.Disassemble();
        }

        private void CreateDisassembler16(LoadedImage image)
        {
            dasm = new X86Disassembler(
                image.CreateReader(image.BaseAddress),
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
        }

        private void CreateDisassembler32(LoadedImage image)
        {
            dasm = new X86Disassembler(
                image.CreateReader(image.BaseAddress),
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
        }

        private void CreateDisassembler16(ImageReader rdr)
        {
            dasm = new X86Disassembler(
                rdr,
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
        }

        [Test]
        public void DisassembleSequence()
        {
            Program prog = new Program();
            IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(
                new Address(0xB96, 0),
                @"	mov	ax,0
	cwd
foo:
	lodsb	
	dec		cx
	jnz		foo
");

            CreateDisassembler16(lr.Image);
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
            IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(
                new Address(0xB96, 0),
                "foo	proc\r\n" +
                "		mov	bx,[bp+4]\r\n" +
                "		mov	ax,[bx+4]\r\n" +
                "		mov cx,cs:[si+4]\r\n");

            CreateDisassembler16(lr.Image);
            IntelInstruction[] instrs = new IntelInstruction[3];
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
            IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(
                new Address(0xB96, 0),
                "foo	proc\r\n" +
                "rol	ax,cl\r\n" +
                "ror	word ptr [bx+2],cl\r\n" +
                "rcr	word ptr [bp+4],4\r\n" +
                "rcl	ax,1\r\n");

            LoadedImage img = lr.Image;
            CreateDisassembler16(img.CreateReader(img.BaseAddress));
            IntelInstruction[] instrs = new IntelInstruction[4];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i != instrs.Length; ++i)
            {
                instrs[i] = dasm.Disassemble();
                sb.AppendFormat("{0}\r\n", instrs[i].ToString());
            }
            string s = sb.ToString();
            Assert.AreEqual(
                "rol\tax,cl\r\n" +
                "ror\tword ptr [bx+02],cl\r\n" +
                "rcr\tword ptr [bp+04],04\r\n" +
                "rcl\tax,01\r\n", s);

        }

        [Test]
        public void Extensions()
        {
            Program prog = new Program();
            IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(
                new Address(0xA14, 0),
@"		.i86
foo		proc
		movsx	ecx,word ptr [bp+8]
		movzx   edx,cl
		movsx	ebx,bx
		movzx	ax,byte ptr [bp+04]
");
            CreateDisassembler16(lr.Image);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i != 4; ++i)
            {
                IntelInstruction ii = dasm.Disassemble();
                sb.AppendLine(string.Format("{0}", ii.ToString()));
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
            Program prog = new Program(); IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(new Address(0x0B00, 0),
                @"	.i386
	mov ebx,[edi*2]
");
            CreateDisassembler32(lr.Image);
            IntelInstruction instr = dasm.Disassemble();
            MemoryOperand mem = (MemoryOperand) instr.op2;
            Assert.AreEqual(2, mem.Scale);
            Assert.AreEqual(RegisterStorage.None, mem.Base);
            Assert.AreEqual(Registers.edi, mem.Index);
        }

        [Test]
        public void DisFpuInstructions()
        {
            using (FileUnitTester fut = new FileUnitTester("Intel/DisFpuInstructions.txt"))
            {
                Program prog = new Program();
                IntelTextAssembler asm = new IntelTextAssembler();
                LoaderResults lr;
                using (var rdr = new StreamReader(FileUnitTester.MapTestPath("Fragments/fpuops.asm")))
                {
                    lr = asm.Assemble(new Address(0xC32, 0), rdr);
                }
                CreateDisassembler16(lr.Image);
                while (lr.Image.IsValidAddress(dasm.Address))
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
            var instr = Disassemble16(0x83, 0xC6, 0x1);  // add si,+01
            Assert.AreEqual("add\tsi,01", instr.ToString());
            Assert.AreEqual(PrimitiveType.Word16, instr.op1.Width);
            Assert.AreEqual(PrimitiveType.Byte, instr.op2.Width);
            Assert.AreEqual(PrimitiveType.Word16, instr.dataWidth);
        }

        [Test]
        public void DisLesBxStackArg()
        {
            var instr = Disassemble16(0xC4, 0x5E, 0x6);		// les bx,[bp+06]
            Assert.AreEqual("les\tbx,[bp+06]", instr.ToString());
            Assert.AreSame(PrimitiveType.Pointer32, instr.op2.Width);
        }

        [Test]
        public void SegFromBits()
        {
            Assert.AreSame(Registers.es, X86Disassembler.SegFromBits(0));
            Assert.AreSame(Registers.cs, X86Disassembler.SegFromBits(1));
            Assert.AreSame(Registers.ss, X86Disassembler.SegFromBits(2));
            Assert.AreSame(Registers.ds, X86Disassembler.SegFromBits(3));
            Assert.AreSame(Registers.fs, X86Disassembler.SegFromBits(4));
            Assert.AreSame(Registers.gs, X86Disassembler.SegFromBits(5));
        }

        [Test]
        public void Bswap()
        {
            var instr = Disassemble32(0x0F, 0xC8); ;		// bswap eax
            Assert.AreEqual("bswap\teax", instr.ToString());
            instr = Disassemble32(0x0F, 0xCF);       //  bswap edi
            Assert.AreEqual("bswap\tedi", instr.ToString());
        }

        [Test]
        public void DisasmRelocatedOperand()
        {
            byte[] image = new byte[] { 0xB8, 0x78, 0x56, 0x34, 0x12 };	// mov eax,0x12345678
            LoadedImage img = new LoadedImage(new Address(0x00100000), image);
            img.Relocations.AddPointerReference(0x00100001u - img.BaseAddress.Linear, 0x12345678);
            ImageReader rdr = img.CreateReader(img.BaseAddress);
            X86Disassembler dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            IntelInstruction instr = dasm.Disassemble();
            Assert.AreEqual("mov\teax,12345678", instr.ToString());
            Assert.AreEqual("ptr32", instr.op2.Width.ToString());
        }

        [Test]
        public void DisasmRelocatedSegment()
        {
            byte[] image = new byte[] { 0x2E, 0xC7, 0x06, 0x01, 0x00, 0x00, 0x08 }; // mov cs:[0001],0800
            LoadedImage img = new LoadedImage(new Address(0x900, 0), image);
            img.Relocations.AddSegmentReference(5, 0x0800);
            ImageReader rdr = img.CreateReader(img.BaseAddress);
            CreateDisassembler16(rdr);
            IntelInstruction instr = dasm.Disassemble();
            Assert.AreEqual("mov\tword ptr cs:[0001],0800", instr.ToString());
            Assert.AreEqual("selector", instr.op2.Width.ToString());
        }

        [Test]
        public void TestWithImmediateOperands()
        {
            var instr = Disassemble16(0xF6, 0x06, 0x26, 0x54, 0x01);     // test byte ptr [5426],01
            Assert.AreEqual("test\tbyte ptr [5426],01", instr.ToString());
            Assert.AreSame(PrimitiveType.Byte, instr.op1.Width);
            Assert.AreSame(PrimitiveType.Byte, instr.op2.Width);
            Assert.AreSame(PrimitiveType.Byte, instr.dataWidth, "Instruction data width should be byte");
        }

        [Test]
        public void RelativeCallTest()
        {
            var instr = Disassemble16(0xE8, 0x00, 0xF0);
            Assert.AreEqual("call\tF003", instr.ToString());
            Assert.AreSame(PrimitiveType.Word16, instr.op1.Width);
        }

        [Test]
        public void FarCall()
        {
            var instr = Disassemble16(0x9A, 0x78, 0x56, 0x34, 0x12, 0x90, 0x90);
            Assert.AreEqual("call\tfar 1234:5678", instr.ToString());
        }

        [Test]
        public void Xlat16()
        {
            var instr = Disassemble16(0xD7);
            Assert.AreEqual("xlat\t", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word16, instr.addrWidth);
        }


        [Test]
        public void Xlat32()
        {
            var instr = Disassemble32(0xD7);
            Assert.AreEqual("xlat\t", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word32, instr.addrWidth);
        }

        [Test]
        public void Hlt()
        {
            var instr = Disassemble16(0xF4);
            Assert.AreEqual("hlt\t", instr.ToString());
        }

        [Test]
        public void X86_64_rexW()
        {
            var instr = Disassemble64(0x48, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\trdx,[rsp+20]", instr.ToString());
        }

        [Test]
        public void Dis_X86_64_rexWR()
        {
            var instr = Disassemble64(0x4C, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\tr10,[rsp+20]", instr.ToString());
        }

        [Test]
        public void Dis_X86_64_rexWB()
        {
            var instr = Disassemble64(0x49, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\trdx,[r12+20]", instr.ToString());
        }

        [Test]
        public void Dis_X86_64_rexR()
        {
            var instr = Disassemble64(0x44, 0x33, 0xC0);
            Assert.AreEqual("xor\tr8d,eax", instr.ToString());
        }

        [Test]
        public void Dis_X86_64_rexRB()
        {
            var instr = Disassemble64(0x45, 0x33, 0xC0);
            Assert.AreEqual("xor\tr8d,r8d", instr.ToString());
        }

        [Test]
        public void Dis_x86_64_rexB_bytesize()
        {
            var instr = Disassemble64(0x41, 0x30, 0xC0);
            Assert.AreEqual("xor\tr8b,al", instr.ToString());
        }

        [Test]
        public void Dis_x86_64_rexR_bytesize_sil()
        {
            var instr = Disassemble64(0x44, 0x30, 0xC6);
            Assert.AreEqual("xor\tsil,r8b", instr.ToString());
        }

        [Test]
        public void Dis_x86_64_rexB_wordsize()
        {
            var instr = Disassemble64(0x66, 0x41, 0x33, 0xC6);
            Assert.AreEqual("xor\tax,r14w", instr.ToString());
        }

        [Test]
        public void Dis_x86_64_movaps()
        {
            var instr = Disassemble64(0x0f, 0x28, 0x44, 0x24, 0x20);
            Assert.AreEqual("movaps\txmm0,[rsp+20]", instr.ToString());
        }

        [Test]
        public void Dis_x86_64_movdqa()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7f, 0x44, 0x24, 0x20);
            Assert.AreEqual("movdqa\txmmword ptr [rsp+20],xmm0", instr.ToString());
        }

        [Test]
        public void Dis_x86_cmovnz()
        {
            var instr = Disassemble32(0x0F, 0x45, 0xC1);
            Assert.AreEqual("cmovnz\teax,ecx", instr.ToString());
        }
    }
}
