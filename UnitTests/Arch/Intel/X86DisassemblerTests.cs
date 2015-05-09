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

using System;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86DisassemblerTests
    {
        private X86Disassembler dasm;

        public X86DisassemblerTests()
        {
        }

        private IntelInstruction Disassemble16(params byte[] bytes)
        {
            LoadedImage img = new LoadedImage(Address.SegPtr(0xC00, 0), bytes);
            ImageReader rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
            return dasm.First();
        }

        private IntelInstruction Disassemble32(params byte[] bytes)
        {
            var img = new LoadedImage(Address.Ptr32(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            return dasm.First();
        }

        private IntelInstruction Disassemble64(params byte[] bytes)
        {
            var img = new LoadedImage(Address.Ptr64(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word64, true);
            return dasm.First();
        }

        private void CreateDisassembler16(LoadedImage image)
        {
            dasm = new X86Disassembler(
                image.CreateLeReader(image.BaseAddress),
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
        }

        private void CreateDisassembler32(LoadedImage image)
        {
            dasm = new X86Disassembler(
                image.CreateLeReader(image.BaseAddress),
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

        private void AssertCode32(string sExp, params byte[] bytes)
        {
            var instr = Disassemble32(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void AssertCode64(string sExp, params byte[] bytes)
        {
            var instr = Disassemble64(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void X86Dis_Sequence()
        {
            IntelTextAssembler asm = new IntelTextAssembler();
            var program = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                @"	mov	ax,0
	cwd
foo:
	lodsb	
	dec		cx
	jnz		foo
");

            CreateDisassembler16(program.Image);
            StringBuilder sb = new StringBuilder();
            foreach (var instr in dasm.Take(5))
            {
                Address addr = instr.Address;
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
            IntelTextAssembler asm = new IntelTextAssembler();
            var program = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                "foo	proc\r\n" +
                "		mov	bx,[bp+4]\r\n" +
                "		mov	ax,[bx+4]\r\n" +
                "		mov cx,cs:[si+4]\r\n");

            CreateDisassembler16(program.Image);
            IntelInstruction[] instrs = dasm.Take(3).ToArray();
            Assert.AreEqual(Registers.ss, ((MemoryOperand)instrs[0].op2).DefaultSegment);
            Assert.AreEqual(Registers.ds, ((MemoryOperand)instrs[1].op2).DefaultSegment);
            Assert.AreEqual(Registers.cs, ((MemoryOperand)instrs[2].op2).DefaultSegment);
        }

        [Test]
        public void Rotations()
        {
            IntelTextAssembler asm = new IntelTextAssembler();
            var lr = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                "foo	proc\r\n" +
                "rol	ax,cl\r\n" +
                "ror	word ptr [bx+2],cl\r\n" +
                "rcr	word ptr [bp+4],4\r\n" +
                "rcl	ax,1\r\n");

            LoadedImage img = lr.Image;
            CreateDisassembler16(img.CreateLeReader(img.BaseAddress));
            StringBuilder sb = new StringBuilder();
            foreach (var instr in dasm.Take(4))
            {
                sb.AppendFormat("{0}\r\n", instr.ToString());
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
            IntelTextAssembler asm = new IntelTextAssembler();
            var program = asm.AssembleFragment(
                Address.SegPtr(0xA14, 0),
@"		.i86
foo		proc
		movsx	ecx,word ptr [bp+8]
		movzx   edx,cl
		movsx	ebx,bx
		movzx	ax,byte ptr [bp+04]
");
            CreateDisassembler16(program.Image);
            StringBuilder sb = new StringBuilder();
            foreach (var ii in dasm.Take(4))
            {
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
            IntelTextAssembler asm = new IntelTextAssembler();
            var program = asm.AssembleFragment(Address.SegPtr(0x0B00, 0),
                @"	.i386
	mov ebx,[edi*2]
");
            CreateDisassembler32(program.Image);
            var instr = dasm.First();
            MemoryOperand mem = (MemoryOperand)instr.op2;
            Assert.AreEqual(2, mem.Scale);
            Assert.AreEqual(RegisterStorage.None, mem.Base);
            Assert.AreEqual(Registers.edi, mem.Index);
        }

        [Test]
        public void DisFpuInstructions()
        {
            using (FileUnitTester fut = new FileUnitTester("Intel/DisFpuInstructions.txt"))
            {
                IntelTextAssembler asm = new IntelTextAssembler();
                Program lr;
                using (var rdr = new StreamReader(FileUnitTester.MapTestPath("Fragments/fpuops.asm")))
                {
                    lr = asm.Assemble(Address.SegPtr(0xC32, 0), rdr);
                }
                CreateDisassembler16(lr.Image);
                foreach (IntelInstruction instr in dasm)
                {
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
        public void X86Dis_Bswap()
        {
            var instr = Disassemble32(0x0F, 0xC8); ;		// bswap eax
            Assert.AreEqual("bswap\teax", instr.ToString());
            instr = Disassemble32(0x0F, 0xCF);       //  bswap edi
            Assert.AreEqual("bswap\tedi", instr.ToString());
        }

        [Test]
        public void X86Dis_RelocatedOperand()
        {
            byte[] image = new byte[] { 0xB8, 0x78, 0x56, 0x34, 0x12 };	// mov eax,0x12345678
            LoadedImage img = new LoadedImage(Address.Ptr32(0x00100000), image);
            img.Relocations.AddPointerReference(0x00100001ul - img.BaseAddress.ToLinear(), 0x12345678);
            ImageReader rdr = img.CreateLeReader(img.BaseAddress);
            X86Disassembler dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            IntelInstruction instr = dasm.First();
            Assert.AreEqual("mov\teax,12345678", instr.ToString());
            Assert.AreEqual("ptr32", instr.op2.Width.ToString());
        }

        [Test]
        public void X86Dis_RelocatedSegment()
        {
            byte[] image = new byte[] { 0x2E, 0xC7, 0x06, 0x01, 0x00, 0x00, 0x08 }; // mov cs:[0001],0800
            LoadedImage img = new LoadedImage(Address.SegPtr(0x900, 0), image);
            img.Relocations.AddSegmentReference(5, 0x0800);
            ImageReader rdr = img.CreateLeReader(img.BaseAddress);
            CreateDisassembler16(rdr);
            IntelInstruction instr = dasm.First();
            Assert.AreEqual("mov\tword ptr cs:[0001],0800", instr.ToString());
            Assert.AreEqual("selector", instr.op2.Width.ToString());
        }

        [Test]
        public void X86Dis_TestWithImmediateOperands()
        {
            var instr = Disassemble16(0xF6, 0x06, 0x26, 0x54, 0x01);     // test byte ptr [5426],01
            Assert.AreEqual("test\tbyte ptr [5426],01", instr.ToString());
            Assert.AreSame(PrimitiveType.Byte, instr.op1.Width);
            Assert.AreSame(PrimitiveType.Byte, instr.op2.Width);
            Assert.AreSame(PrimitiveType.Byte, instr.dataWidth, "Instruction data width should be byte");
        }

        [Test]
        public void X86Dis_RelativeCallTest()
        {
            var instr = Disassemble16(0xE8, 0x00, 0xF0);
            Assert.AreEqual("call\tF003", instr.ToString());
            Assert.AreSame(PrimitiveType.Word16, instr.op1.Width);
        }

        [Test]
        public void X86Dis_FarCall()
        {
            var instr = Disassemble16(0x9A, 0x78, 0x56, 0x34, 0x12, 0x90, 0x90);
            Assert.AreEqual("call\tfar 1234:5678", instr.ToString());
        }

        [Test]
        public void X86Dis_Xlat16()
        {
            var instr = Disassemble16(0xD7);
            Assert.AreEqual("xlat\t", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word16, instr.addrWidth);
        }

        [Test]
        public void X86Dis_Xlat32()
        {
            var instr = Disassemble32(0xD7);
            Assert.AreEqual("xlat\t", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word32, instr.addrWidth);
        }

        [Test]
        public void X86Dis_Hlt()
        {
            var instr = Disassemble16(0xF4);
            Assert.AreEqual("hlt\t", instr.ToString());
        }

        [Test]
        public void Dis_X86_64_rexW()
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
            Assert.AreEqual("movdqa\t[rsp+20],xmm0", instr.ToString());
        }

        [Test]
        public void Dis_x86_cmovnz()
        {
            var instr = Disassemble32(0x0F, 0x45, 0xC1);
            Assert.AreEqual("cmovnz\teax,ecx", instr.ToString());
        }

        [Test]
        public void Dis_x86_pxor()
        {
            var instr = Disassemble32(0x0F, 0xEF, 0xC1);
            Assert.AreEqual("pxor\tmm0,mm1", instr.ToString());
        }

        [Test]
        public void Dis_x86_Cmpxchg()
        {
            var instr = Disassemble32(0x0f, 0xb1, 0x0a, 0x85, 0xc0, 0x0f, 0x85, 0xdc);
            Assert.AreEqual("cmpxchg\t[edx],ecx", instr.ToString());
        }

        [Test]
        public void Dis_x86_Xadd()
        {
            var instr = Disassemble32(0x0f, 0xC1, 0xC2);
            Assert.AreEqual("xadd\tedx,eax", instr.ToString());
        }

        [Test]
        public void Dis_x86_cvttsd2si()
        {
            var instr = Disassemble32(0xF2, 0x0F, 0x2C, 0xC3);
            Assert.AreEqual("cvttsd2si\teax,xmm3", instr.ToString());
        }

        [Test]
        public void Dis_x86_fucompp()
        {
            var instr = Disassemble32(0xDA, 0xE9);
            Assert.AreEqual("fucompp\t", instr.ToString());
        }

        [Test]
        public void Dis_x86_Call32()
        {
            var instr = Disassemble32(0xE9, 0x78, 0x56, 0x34, 012);
            var addrOp = (AddressOperand) instr.op1;
        }

        [Test]
        public void Dis_x86_Call16()
        {
            var instr = Disassemble16(0xE9, 0x78, 0x56);
            var addrOp = (ImmediateOperand)instr.op1;
            Assert.AreEqual("567B", addrOp.ToString());
        }

        [Test]
        public void Dis_x86_DirectOperand32()
        {
            var instr = Disassemble32(0x8B, 0x15, 0x22, 0x33, 0x44, 0x55, 0x66);
            Assert.AreEqual("mov\tedx,[55443322]", instr.ToString());
            var memOp = (MemoryOperand)instr.op2;
            Assert.AreEqual("ptr32", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void Dis_x86_DirectOperand16()
        {
            var instr = Disassemble16(0x8B, 0x16, 0x22, 0x33, 0x44);
            Assert.AreEqual("mov\tdx,[3322]", instr.ToString());
            var memOp = (MemoryOperand)instr.op2;
            Assert.AreEqual("word16", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void Dis_x86_Movdqa()
        {
            var instr = Disassemble32(0x66, 0x0F, 0x6F, 0x06, 0x12, 0x34, 0x56);
            Assert.AreEqual("movdqa\txmm0,[esi]", instr.ToString());
        }
        //$TOD: copy only gives me n-1 bytes rathern than n. bytes
        //   0048D4A8 
        
        // 66 0F 6F 06                    f.o    
        [Test]
        public void Dis_x86_bts()
        {
            var instr = Disassemble32(0x0F, 0xAB, 0x04, 0x24, 0xEB);
            Assert.AreEqual("bts\t[esp],eax", instr.ToString());
        }

        [Test]
        public void Dis_x86_cpuid()
        {
            var instr = Disassemble32(0x0F, 0xA2);
            Assert.AreEqual("cpuid\t", instr.ToString());
        }

        [Test]
        public void Dis_x86_xgetbv()
        {
            var instr = Disassemble32(0x0F, 0x01, 0xD0);
            Assert.AreEqual("xgetbv\t", instr.ToString());
        }

        [Test]
        public void Dis_x86_rdtsc()
        {
            var instr = Disassemble32(0x0F, 0x31);
            Assert.AreEqual("rdtsc\t", instr.ToString());
        }

        [Test]
        public void Dis_x86_foo()
        {
            var instr = Disassemble32(0x0F, 0xBA, 0xF3, 0x00);
            Assert.AreEqual("btr\tebx,00", instr.ToString());
        }

        [Test]
        public void Dis_x86_movd_32()
        {
            AssertCode32("movd\tdword ptr [esi],mm1", 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tdword ptr [esi],xmm1", 0x66, 0x0F, 0x7E, 0x0E);
            AssertCode32("movq\txmm1,qword ptr [esi]", 0xF3, 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tesi,mm1", 0x0F, 0x7E, 0xCE);
            AssertCode32("movd\tesi,xmm1", 0x66, 0x0F, 0x7E, 0xCE);
            AssertCode32("movq\txmm1,xmm6", 0xF3, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void Dis_x86_movd_64()
        {
            AssertCode64("movd\tdword ptr [rsi],mm1", 0x0F, 0x7E, 0x0E);
            AssertCode64("movd\tdword ptr [rsi],xmm1", 0x66, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\txmm1,qword ptr [rsi]", 0xF3, 0x0F, 0x7E, 0x0E);
            AssertCode64("movd\tesi,mm1", 0x0F, 0x7E, 0xCE);
            AssertCode64("movd\tesi,xmm1", 0x66, 0x0F, 0x7E, 0xCE);
            AssertCode64("movq\txmm1,xmm6", 0xF3, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void Dis_x86_movd_64_rex()
        {
            AssertCode64("movq\t[rsi],mm1", 0x48, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\tqword ptr [rsi],xmm1", 0x66, 0x48, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\trsi,mm1", 0x48, 0x0F, 0x7E, 0xCE);
            AssertCode64("movq\trsi,xmm1", 0x66, 0x48, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void Dis_x86_punpcklbw()
        {
            AssertCode32("punpcklbw\txmm1,xmm3", 0x66, 0x0f, 0x60, 0xcb);
        }

        [Test]
        public void Dis_x86_more2()
        {
            AssertCode32("movlhps\txmm3,xmm3", 0x0f, 0x16, 0xdb);
            AssertCode32("pshuflw\txmm3,xmm3,00", 0xf2, 0x0f, 0x70, 0xdb, 0x00);
            AssertCode32("pcmpeqb\txmm0,[eax]", 0x66, 0x0f, 0x74, 0x00);
            AssertCode32("stmxcsr\tdword ptr [ebp-0C]", 0x0f, 0xae, 0x5d, 0xf4);
            AssertCode32("palignr\txmm3,xmm1,00", 0x66, 0x0f, 0x3a, 0x0f, 0xd9, 0x00);
            AssertCode32("movq\t[edi],xmm1", 0x66, 0x0f, 0xd6, 0x0f);
            AssertCode32("ldmxcsr\tdword ptr [ebp+08]", 0x0F, 0xAE, 0x55, 0x08);
            AssertCode32("pcmpistri\txmm0,[edi-10],40", 0x66, 0x0F, 0x3A, 0x63, 0x47, 0xF0, 0x40);
        }
    }
}
