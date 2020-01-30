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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86DisassemblerTests
    {
        private ServiceContainer sc;
        private X86Disassembler dasm;
        private X86Options options;

        public X86DisassemblerTests()
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        private X86Instruction Disassemble16(params byte[] bytes)
        {
            MemoryArea img = new MemoryArea(Address.SegPtr(0xC00, 0), bytes);
            EndianImageReader rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(ProcessorMode.Real, rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
            if (options != null)
            {
                dasm.Emulate8087 = options.Emulate8087;
            }
            return dasm.First();
        }

        private X86Instruction Disassemble32(params byte[] bytes)
        {
            var img = new MemoryArea(Address.Ptr32(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(ProcessorMode.Protected32, rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            return dasm.First();
        }

        private X86Instruction Disassemble64(params byte[] bytes)
        {
            var img = new MemoryArea(Address.Ptr64(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var dasm = new X86Disassembler(
                ProcessorMode.Protected64,
                rdr,
                PrimitiveType.Word32,
                PrimitiveType.Word64,
                true);
            return dasm.First();
        }

        private void CreateDisassembler16(params byte[] bytes)
        {
            var mem = new MemoryArea(Address.SegPtr(0x0C00, 0), bytes);
            CreateDisassembler16(mem);
        }

        private void CreateDisassembler16(MemoryArea mem)
        {
            dasm = new X86Disassembler(
                ProcessorMode.Real,
                mem.CreateLeReader(mem.BaseAddress),
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
            if (options != null)
            {
                dasm.Emulate8087 = options.Emulate8087;
            }
        }

        private void CreateDisassembler32(MemoryArea image)
        {
            dasm = new X86Disassembler(
                ProcessorMode.Protected32,
                image.CreateLeReader(image.BaseAddress),
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
        }

        private void CreateDisassembler16(EndianImageReader rdr)
        {
            dasm = new X86Disassembler(
                ProcessorMode.Real,
                rdr,
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
        }

        private void AssertCode16(string sExp, params byte[] bytes)
        {
            var instr = Disassemble16(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void AssertCode32(string sExp, params byte[] bytes)
        {
            var instr = Disassemble32(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private X86Instruction AssertCode64(string sExp, params byte[] bytes)
        {
            var instr = Disassemble64(bytes);
            Assert.AreEqual(sExp, instr.ToString());
            return instr;
        }

        [SetUp]
        public void Setup()
        {
            options = null;
        }

        [Test]
        public void X86dis_Sequence()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
            var program = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                @"	mov	ax,0
	cwd
foo:
	lodsb	
	dec		cx
	jnz		foo
");

            CreateDisassembler16(program.SegmentMap.Segments.Values.First().MemoryArea);
            StringBuilder sb = new StringBuilder();
            foreach (var instr in dasm.Take(5))
            {
                Address addr = instr.Address;
                sb.AppendFormat("{0}\t{1}\r\n", addr, instr.ToString());
            }

            string s = sb.ToString();
            Assert.AreEqual(
                "0B96:0000\tmov\tax,0000\r\n" +
                "0B96:0003\tcwd\r\n" +
                "0B96:0004\tlodsb\r\n" +
                "0B96:0005\tdec\tcx\r\n" +
                "0B96:0006\tjnz\t0004\r\n",
                s);
        }

        [Test]
        public void SegmentOverrides()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
            var program = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                "foo	proc\r\n" +
                "		mov	bx,[bp+4]\r\n" +
                "		mov	ax,[bx+4]\r\n" +
                "		mov cx,cs:[si+4]\r\n");

            CreateDisassembler16(program.SegmentMap.Segments.Values.First().MemoryArea);
            X86Instruction[] instrs = dasm.Take(3).ToArray();
            Assert.AreEqual(Registers.ss, ((MemoryOperand) instrs[0].Operands[1]).DefaultSegment);
            Assert.AreEqual(Registers.ds, ((MemoryOperand) instrs[1].Operands[1]).DefaultSegment);
            Assert.AreEqual(Registers.cs, ((MemoryOperand) instrs[2].Operands[1]).DefaultSegment);
        }

        [Test]
        public void Rotations()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
            var lr = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                "foo	proc\r\n" +
                "rol	ax,cl\r\n" +
                "ror	word ptr [bx+2],cl\r\n" +
                "rcr	word ptr [bp+4],4\r\n" +
                "rcl	ax,1\r\n");

            MemoryArea img = lr.SegmentMap.Segments.Values.First().MemoryArea;
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
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
            var program = asm.AssembleFragment(
                Address.SegPtr(0xA14, 0),
@"		.i86
foo		proc
		movsx	ecx,word ptr [bp+8]
		movzx   edx,cl
		movsx	ebx,bx
		movzx	ax,byte ptr [bp+04]
");
            CreateDisassembler16(program.SegmentMap.Segments.Values.First().MemoryArea);
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

        private X86Instruction DisEnumerator_TakeNext(System.Collections.Generic.IEnumerator<X86Instruction> e)
        {
            e.MoveNext();
            return e.Current;
        }

        [Test]
        public void Dis_x86_InvalidKeptStateRegression()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureFlat32("x86-protected-32"));
            var lr = asm.AssembleFragment(
                Address.Ptr32(0x01001000),

                "db 0xf2, 0x0f, 0x70, 0x00, 0x00\r\n" +
                "db 0xf3, 0x0f, 0x70, 0x00, 0x00\r\n");

            /* Before (incorrect):
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  
             *  
             * After (correct):
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  pshufhw xmm0, dqword ptr ds:[eax], 0
             */

            MemoryArea img = lr.SegmentMap.Segments.Values.First().MemoryArea;
            CreateDisassembler32(img);
            var instructions = dasm.GetEnumerator();

            X86Instruction one = DisEnumerator_TakeNext(instructions);
            X86Instruction two = DisEnumerator_TakeNext(instructions);

            Assert.AreEqual(Mnemonic.pshuflw, one.Mnemonic);
            Assert.AreEqual("xmm0", one.Operands[0].ToString());
            Assert.AreEqual("[eax]", one.Operands[1].ToString());

            Assert.AreEqual(Mnemonic.pshufhw, two.Mnemonic);
            Assert.AreEqual("xmm0", two.Operands[0].ToString());
            Assert.AreEqual("[eax]", two.Operands[1].ToString());
        }

        [Test]
        public void DisEdiTimes2()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureFlat32("x86-protected-32"));
            var program = asm.AssembleFragment(Address.SegPtr(0x0B00, 0),
                @"	.i386
	mov ebx,[edi*2]
");
            CreateDisassembler32(program.SegmentMap.Segments.Values.First().MemoryArea);
            var instr = dasm.First();
            MemoryOperand mem = (MemoryOperand) instr.Operands[1];
            Assert.AreEqual(2, mem.Scale);
            Assert.AreEqual(RegisterStorage.None, mem.Base);
            Assert.AreEqual(Registers.edi, mem.Index);
        }

        [Test]
        public void DisFpuInstructions()
        {
            using (FileUnitTester fut = new FileUnitTester("Intel/DisFpuInstructions.txt"))
            {
                X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureReal("x86-real-16"));
                Program lr;
                using (var rdr = new StreamReader(FileUnitTester.MapTestPath("Fragments/fpuops.asm")))
                {
                    lr = asm.Assemble(Address.SegPtr(0xC32, 0), rdr);
                }
                CreateDisassembler16(lr.SegmentMap.Segments.Values.First().MemoryArea);
                foreach (X86Instruction instr in dasm)
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
            Assert.AreEqual(PrimitiveType.Word16, instr.Operands[0].Width);
            Assert.AreEqual(PrimitiveType.Byte, instr.Operands[1].Width);
            Assert.AreEqual(PrimitiveType.Word16, instr.dataWidth);
        }

        [Test]
        public void DisLesBxStackArg()
        {
            var instr = Disassemble16(0xC4, 0x5E, 0x6);		// les bx,[bp+06]
            Assert.AreEqual("les\tbx,[bp+06]", instr.ToString());
            Assert.AreSame(PrimitiveType.Ptr32, instr.Operands[1].Width);
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
        public void X86dis_bswap()
        {
            var instr = Disassemble32(0x0F, 0xC8); ;		// bswap eax
            Assert.AreEqual("bswap\teax", instr.ToString());
            instr = Disassemble32(0x0F, 0xCF);       //  bswap edi
            Assert.AreEqual("bswap\tedi", instr.ToString());
        }

        [Test]
        public void X86dis_RelocatedOperand()
        {
            byte[] image = new byte[] { 0xB8, 0x78, 0x56, 0x34, 0x12 };	// mov eax,0x12345678
            MemoryArea img = new MemoryArea(Address.Ptr32(0x00100000), image);
            img.Relocations.AddPointerReference(0x00100001ul, 0x12345678);
            EndianImageReader rdr = img.CreateLeReader(img.BaseAddress);
            X86Disassembler dasm = new X86Disassembler(
                ProcessorMode.Protected32,
                rdr,
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
            X86Instruction instr = dasm.First();
            Assert.AreEqual("mov\teax,12345678", instr.ToString());
            Assert.AreEqual("ptr32", instr.Operands[1].Width.ToString());
        }

        [Test]
        public void X86Dis_RelocatedSegment()
        {
            byte[] image = new byte[] { 0x2E, 0xC7, 0x06, 0x01, 0x00, 0x00, 0x08 }; // mov cs:[0001],0800
            MemoryArea img = new MemoryArea(Address.SegPtr(0x900, 0), image);
            var relAddr = Address.SegPtr(0x900, 5);
            img.Relocations.AddSegmentReference(relAddr.ToLinear(), 0x0800);
            EndianImageReader rdr = img.CreateLeReader(img.BaseAddress);
            CreateDisassembler16(rdr);
            X86Instruction instr = dasm.First();
            Assert.AreEqual("mov\tword ptr cs:[0001],0800", instr.ToString());
            Assert.AreEqual("selector", instr.Operands[1].Width.ToString());
        }

        [Test]
        public void X86dis_TestWithImmediateOperands()
        {
            var instr = Disassemble16(0xF6, 0x06, 0x26, 0x54, 0x01);     // test byte ptr [5426],01
            Assert.AreEqual("test\tbyte ptr [5426],01", instr.ToString());
            Assert.AreSame(PrimitiveType.Byte, instr.Operands[0].Width);
            Assert.AreSame(PrimitiveType.Byte, instr.Operands[1].Width);
            Assert.AreSame(PrimitiveType.Byte, instr.dataWidth, "Instruction data width should be byte");
        }

        [Test]
        public void X86dis_RelativeCallTest()
        {
            var instr = Disassemble16(0xE8, 0x00, 0xF0);
            Assert.AreEqual("call\tF003", instr.ToString());
            Assert.AreSame(PrimitiveType.Word16, instr.Operands[0].Width);
        }

        [Test]
        public void X86dis_farCall()
        {
            var instr = Disassemble16(0x9A, 0x78, 0x56, 0x34, 0x12, 0x90, 0x90);
            Assert.AreEqual("call\tfar 1234:5678", instr.ToString());
        }

        [Test]
        public void X86dis_Xlat16()
        {
            var instr = Disassemble16(0xD7);
            Assert.AreEqual("xlat", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word16, instr.addrWidth);
        }

        [Test]
        public void X86Dis_xlat32()
        {
            var instr = Disassemble32(0xD7);
            Assert.AreEqual("xlat", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word32, instr.addrWidth);
        }

        [Test]
        public void X86dis_hlt()
        {
            var instr = Disassemble16(0xF4);
            Assert.AreEqual("hlt", instr.ToString());
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
            Assert.AreEqual("fucompp", instr.ToString());
        }

        [Test]
        public void Dis_x86_Call32()
        {
            var instr = Disassemble32(0xE9, 0x78, 0x56, 0x34, 012);
            var addrOp = (AddressOperand) instr.Operands[0];
            Assert.AreEqual("0C35567D", addrOp.ToString());
        }

        [Test]
        public void Dis_x86_Call16()
        {
            var instr = Disassemble16(0xE9, 0x78, 0x56);
            var addrOp = (ImmediateOperand) instr.Operands[0];
            Assert.AreEqual("567B", addrOp.ToString());
        }

        [Test]
        public void Dis_x86_DirectOperand32()
        {
            var instr = Disassemble32(0x8B, 0x15, 0x22, 0x33, 0x44, 0x55, 0x66);
            Assert.AreEqual("mov\tedx,[55443322]", instr.ToString());
            var memOp = (MemoryOperand) instr.Operands[1];
            Assert.AreEqual("ptr32", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void Dis_x86_DirectOperand16()
        {
            var instr = Disassemble16(0x8B, 0x16, 0x22, 0x33, 0x44);
            Assert.AreEqual("mov\tdx,[3322]", instr.ToString());
            var memOp = (MemoryOperand) instr.Operands[1];
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
            Assert.AreEqual("cpuid", instr.ToString());
        }

        [Test]
        public void Dis_x86_xgetbv()
        {
            var instr = Disassemble32(0x0F, 0x01, 0xD0);
            Assert.AreEqual("xgetbv", instr.ToString());
        }

        [Test]
        public void Dis_x86_rdtsc()
        {
            var instr = Disassemble32(0x0F, 0x31);
            Assert.AreEqual("rdtsc", instr.ToString());
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
        }

        [Test]
        public void X86dis_pcmpeqb()
        {
            AssertCode32("pcmpeqb\txmm0,[eax]", 0x66, 0x0f, 0x74, 0x00);
        }

        [Test]
        public void Dis_x86_more3()
        {
            AssertCode32("stmxcsr\tdword ptr [ebp-0C]", 0x0f, 0xae, 0x5d, 0xf4);
            AssertCode32("palignr\txmm3,xmm1,00", 0x66, 0x0f, 0x3a, 0x0f, 0xd9, 0x00);
            AssertCode32("movq\t[edi],xmm1", 0x66, 0x0f, 0xd6, 0x0f);
            AssertCode32("ldmxcsr\tdword ptr [ebp+08]", 0x0F, 0xAE, 0x55, 0x08);
            AssertCode32("pcmpistri\txmm0,[edi-10],40", 0x66, 0x0F, 0x3A, 0x63, 0x47, 0xF0, 0x40);
        }

        [Test]
        public void Dis_x86_64_movsxd()
        {
            AssertCode64("movsx\trcx,dword ptr [rax+3C]", 0x48, 0x63, 0x48, 0x3c);
        }

        [Test]
        public void Dis_x86_64_rip_relative()
        {
            AssertCode64("mov\trax,[rip+00100000]", 0x49, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00);
            AssertCode64("mov\trax,[rip+00100000]", 0x48, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00);
        }

        [Test]
        public void Dis_x86_64_sub_immediate_dword()
        {
            AssertCode64("sub\trsp,+00000508", 0x48, 0x81, 0xEC, 0x08, 0x05, 0x00, 0x00);
        }

        [Test]
        public void Dis_x86_nops()
        {
            AssertCode64("nop", 0x90);
            AssertCode64("nop", 0x66, 0x90);
            AssertCode64("nop\tdword ptr [rax]", 0x0F, 0x1F, 0x00);
            AssertCode64("nop\tdword ptr [rax+00]", 0x0F, 0x1F, 0x40, 0x00);
            AssertCode64("nop\tdword ptr [rax+rax+00]", 0x0F, 0x1F, 0x44, 0x00, 0x00);
            AssertCode64("nop\tword ptr [rax+rax+00]", 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00);
            AssertCode64("nop\tdword ptr [rax+00000000]", 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00);
            AssertCode64("nop\tdword ptr [rax+rax+00000000]", 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00);
            AssertCode64("nop\tword ptr [rax+rax+00000000]", 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00);
        }

        [Test]
        public void Dis_x86_rex_prefix_40()
        {
            AssertCode64("push\trbp", 0x40, 0x55);
        }

        [Test]
        public void Dis_x86_repz_ret()
        {
            AssertCode64("ret", 0xF3, 0xC3);
        }

        [Test]
        public void Dis_x86_emulate_x87_int_39()
        {
            options = new X86Options { Emulate8087 = true };
            CreateDisassembler16(0xCD, 0x39, 0x5E, 0xEA);
            var instrs = dasm.Take(2)
                .Select(i => i.ToString())
                .ToArray();
            Assert.AreEqual("nop", instrs[0]);
            Assert.AreEqual("fstp\tdouble ptr [bp-16]", instrs[1]);
        }

        [Test]
        public void Dis_x86_emulate_x87_int_3C()
        {
            options = new X86Options { Emulate8087 = true };
            CreateDisassembler16(0xCD, 0x3C, 0xDD, 0x06, 0x8B, 0x04);
            var instrs = dasm.Take(2)
                .Select(i => i.ToString())
                .ToArray();
            Assert.AreEqual("nop", instrs[0]);
            Assert.AreEqual("fld\tdouble ptr es:[048B]", instrs[1]);
        }

        [Test(Description = "Very large 32-bit offsets can be treated as negative offsets")]
        public void Dis_x86_LargeNegativeOffset()
        {
            AssertCode32("mov\tesi,[eax-0000FFF0]", 0x8B, 0xB0, 0x10, 0x00, 0xFF, 0xFF);
            AssertCode32("mov\tesi,[eax+FFFF0000]", 0x8B, 0xB0, 0x00, 0x00, 0xFF, 0xFF);
        }

        [Test]
        public void Dis_x86_StringOps()
        {
            X86TextAssembler asm = new X86TextAssembler(sc, new X86ArchitectureFlat32("x86-protected-32"));
            var lr = asm.AssembleFragment(
                Address.Ptr32(0x01001000),

                "movsb\r\n" +
                "movsw\r\n" +
                "movsd\r\n" +

                "scasb\r\n" +
                "scasw\r\n" +
                "scasd\r\n" +

                "cmpsb\r\n" +
                "cmpsw\r\n" +
                "cmpsd\r\n" +

                "lodsb\r\n" +
                "lodsw\r\n" +
                "lodsd\r\n" +

                "stosb\r\n" +
                "stosw\r\n" +
                "stosd\r\n");

            MemoryArea img = lr.SegmentMap.Segments.Values.First().MemoryArea;
            CreateDisassembler32(img);
            var instructions = dasm.GetEnumerator();

            List<X86Instruction> instr = new List<X86Instruction>();
            for (int i = 0; i < 5 * 3; i++)
            {
                instr.Add(DisEnumerator_TakeNext(instructions));
            }
            Assert.AreEqual(Mnemonic.movsb, instr[0].Mnemonic);
            Assert.AreEqual(Mnemonic.movs, instr[1].Mnemonic);
            Assert.AreEqual("word16", instr[1].dataWidth.Name);
            Assert.AreEqual(Mnemonic.movs, instr[2].Mnemonic);
            Assert.AreEqual("word32", instr[2].dataWidth.Name);

            Assert.AreEqual(Mnemonic.scasb, instr[3].Mnemonic);
            Assert.AreEqual(Mnemonic.scas, instr[4].Mnemonic);
            Assert.AreEqual("word16", instr[4].dataWidth.Name);
            Assert.AreEqual(Mnemonic.scas, instr[5].Mnemonic);
            Assert.AreEqual("word32", instr[5].dataWidth.Name);

            Assert.AreEqual(Mnemonic.cmpsb, instr[6].Mnemonic);
            Assert.AreEqual(Mnemonic.cmps, instr[7].Mnemonic);
            Assert.AreEqual("word16", instr[7].dataWidth.Name);
            Assert.AreEqual(Mnemonic.cmps, instr[8].Mnemonic);
            Assert.AreEqual("word32", instr[8].dataWidth.Name);

            Assert.AreEqual(Mnemonic.lodsb, instr[9].Mnemonic);
            Assert.AreEqual(Mnemonic.lods, instr[10].Mnemonic);
            Assert.AreEqual("word16", instr[10].dataWidth.Name);
            Assert.AreEqual(Mnemonic.lods, instr[11].Mnemonic);
            Assert.AreEqual("word32", instr[11].dataWidth.Name);

            Assert.AreEqual(Mnemonic.stosb, instr[12].Mnemonic);
            Assert.AreEqual(Mnemonic.stos, instr[13].Mnemonic);
            Assert.AreEqual("word16", instr[13].dataWidth.Name);
            Assert.AreEqual(Mnemonic.stos, instr[14].Mnemonic);
            Assert.AreEqual("word32", instr[14].dataWidth.Name);
        }

        [Test]
        public void X86dis_regression()
        {
            AssertCode64("movups\t[rsp+20],xmm0", 0x0F, 0x11, 0x44, 0x24, 0x20);
        }

        [Test]
        public void X86dis_fucomi()
        {
            AssertCode32("fucomi\tst(0),st(3)", 0xDB, 0xEB);
        }

        [Test]
        public void X86dis_fucomip()
        {
            AssertCode32("fucomip\tst(0),st(1)", 0xDF, 0xE9);
        }

        [Test]
        public void X86dis_movups()
        {
            AssertCode64("movups\txmm0,[rbp-20]", 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movupd\txmm0,[rbp-20]", 0x66, 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movss\txmm0,dword ptr [rbp-20]", 0xF3, 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movsd\txmm0,double ptr [rbp-20]", 0xF2, 0x0F, 0x10, 0x45, 0xE0);
        }

        [Test]
        public void X86dis_movups_16bit()
        {
            AssertCode16("movups\txmm7,xmm3", 0x0F, 0x10, 0xFB);
        }

        [Test]
        public void X86dis_ucomiss()
        {
            AssertCode64("ucomiss\txmm0,dword ptr [rip+0000B12D]", 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
        }

        [Test]
        public void X86dis_ucomisd()
        {
            AssertCode64("ucomisd\txmm0,double ptr [rip+0000B12D]", 0x66, 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
        }

        [Test]
        public void X86dis_addss()
        {
            AssertCode64("addss\txmm1,dword ptr [rip+0000B0FB]", 0xF3, 0x0F, 0x58, 0x0D, 0xFB, 0xB0, 0x00, 0x00);
        }

        [Test]
        public void X86dis_cvtsi2ss()
        {
            AssertCode64("cvtsi2ss\txmm0,rax", 0xF3, 0x48, 0x0F, 0x2A, 0xC0);
        }

        [Test]
        public void X86dis_out_dx()
        {
            AssertCode16("out\tdx,al", 0xEE);
            AssertCode16("out\tdx,ax", 0xEF);
        }

        [Test]
        public void X86dis_x64_push()
        {
            AssertCode64("push\tr15", 0x41, 0x57);
        }

        [Test]
        public void X86dis_x64_push_immediate()
        {
            AssertCode64("push\t42", 0x6A, 0x42);
        }

        [Test]
        public void X86dis_rep_prefix_to_ucomiss()
        {
            AssertCode32("illegal", 0xF2, 0x0F, 0x2E, 0x00);
        }

        [Test]
        public void X86dis_pause()
        {
            AssertCode32("pause", 0xF3, 0x90);
        }

        [Test]
        public void X86dis_mfence()
        {
            AssertCode32("mfence", 0x0F, 0xAE, 0xF0);
        }

        [Test]
        public void X86dis_lfence()
        {
            AssertCode32("lfence", 0x0F, 0xAE, 0xE8);
        }

        [Test]
        public void X86dis_xorps()
        {
            AssertCode32("xorps\txmm0,xmm0", 0x0F, 0x57, 0xC0);
        }

        [Test]
        public void X86dis_xorpd()
        {
            AssertCode32("xorpd\txmm0,xmm0", 0x66, 0x0F, 0x57, 0xC0);
        }

        [Test]
        public void X86dis_aesimc()
        {
            AssertCode64("aesimc\txmm0,xmm0", 0x66, 0x0F, 0x38, 0xDB, 0xC0);
        }

        [Test]
        public void X86dis_vmovss()
        {
            AssertCode64("vmovss\txmm0,dword ptr [rip+00000351]", 0xC5, 0xFA, 0x10, 0x05, 0x51, 0x03, 0x00, 0x00);
        }

        [Test]
        public void X86dis_vcvtsi2ss()
        {
            AssertCode64("vcvtsi2ss\txmm0,xmm0,rax", 0xC4, 0xE1, 0xFA, 0x2A, 0xC0);
        }

        [Test]
        public void X86dis_vcvtsi2sd()
        {
            AssertCode64("vcvtsi2sd\txmm0,xmm0,rdx", 0xC4, 0xE1, 0xFB, 0x2A, 0xC2);
        }

        [Test]
        public void X86dis_call_Ev()
        {
            AssertCode64("call\trax", 0xFF, 0xD0);
        }

        [Test]
        public void X86dis_vaddsd()
        {
            AssertCode64("vaddsd\txmm0,xmm0,xmm0", 0xC5, 0xFB, 0x58, 0xC0);
        }

        [Test]
        public void X86dis_vmovapd()
        {
            AssertCode64("vmovapd\tymm1,[rax]", 0xC5, 0xFD, 0x28, 0x08);
        }

        [Test]
        public void X86dis_vmovaps()
        {
            AssertCode64("vmovaps\t[rcx+4D],xmm4", 0xC5, 0xF8, 0x29, 0x61, 0x4D);
        }

        [Test]
        public void X86dis_vmovsd()
        {
            AssertCode64("vmovsd\tdouble ptr [rcx],xmm0", 0xC5, 0xFB, 0x11, 0x01);
        }

        [Test]
        public void X86dis_vxorps()
        {
            AssertCode64("vxorpd\txmm0,xmm0,xmm0", 0xC5, 0xF9, 0x57, 0xC0);
        }

        [Test]
        public void X86dis_vaddpd()
        {
            AssertCode64("vaddpd\tymm0,ymm0,[rbp-00000090]", 0xC5, 0xFD, 0x58, 0x85, 0x70, 0xFF, 0xFF, 0xFF);
        }

        [Test]
        public void X86dis_64_lea()
        {
            AssertCode64("lea\trdi,[rip+000000DA]", 0x48, 0x8D, 0x3D, 0xDA, 0x00, 0x00, 0x00);
        }

        [Test]
        public void X86dis_vxorpd_256()
        {
            AssertCode64("vxorpd\tymm0,ymm0,ymm0", 0xC5, 0xFD, 0x57, 0xC0);
        }

        [Test]
        public void X86dis_vxorpd_mem_256()
        {
            AssertCode64("vxorpd\tymm1,ymm0,[rcx]", 0xC5, 0xFD, 0x57, 0x09);
        }

        [Test]
        public void X86dis_lar()
        {
            AssertCode32("lar\teax,word ptr [edx+42]", 0x0F, 0x02, 0x42, 0x42);
        }


        [Test]
        public void X86dis_lsl()
        {
            AssertCode32("lsl\teax,word ptr [edx+42]", 0x0F, 0x03, 0x42, 0x42);
        }


        [Test]
        public void X86dis_syscall()
        {
            AssertCode64("syscall", 0x0F, 0x05, 0x42, 0x42);
            AssertCode32("illegal", 0x0F, 0x05, 0x42, 0x42);
        }

        // o64
        [Test]
        public void X86dis_clts()
        {
            AssertCode32("clts", 0x0F, 0x06);
        }


        [Test]
        public void X86dis_sysret()
        {
            AssertCode64("sysret", 0x0F, 0x07);
            AssertCode32("illegal", 0x0F, 0x07);
        }

        // o64
        [Test]
        public void X86dis_invd()
        {
            AssertCode32("invd", 0x0F, 0x08);
        }

        [Test]
        public void X86dis_wbinvd()
        {
            AssertCode32("wbinvd", 0x0F, 0x09);
        }


        [Test]
        public void X86dis_ud2()
        {
            AssertCode32("ud2", 0x0F, 0x0B);
        }


        [Test]
        public void X86dis_prefetch()
        {
            AssertCode32("prefetchw\tdword ptr [edx+42]", 0x0F, 0x0D, 0x42, 0x42);
        }

        // /1
        [Test]
        public void X86dis_movlps()
        {
            AssertCode32("movlps\txmm0,qword ptr [edx+42]", 0x0F, 0x12, 0x42, 0x42);
        }


        [Test]
        public void X86dis_unpcklpd()
        {
            AssertCode32("unpcklps\txmm0,[edx+42]", 0x0F, 0x14, 0x42, 0x42);
            AssertCode32("unpcklpd\txmm0,[edx+42]", 0x66, 0x0F, 0x14, 0x42, 0x42);
        }


        [Test]
        public void X86dis_movhpd()
        {
            AssertCode32("movhps\tqword ptr [edx+42],xmm0", 0x0F, 0x17, 0x42, 0x42);
            AssertCode32("movhpd\tqword ptr [edx+42],xmm0", 0x66, 0x0F, 0x17, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_from_control_Reg()
        {
            AssertCode32("mov\tedx,cr0", 0x0F, 0x20, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_debug_reg()
        {
            AssertCode32("mov\tedx,dr0", 0x0F, 0x21, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_control_reg()
        {
            AssertCode32("mov\tcr0,edx", 0x0F, 0x22, 0x42);
        }

        [Test]
        public void X86dis_mov_to_debug_reg()
        {
            AssertCode32("mov\tdr0,edx", 0x0F, 0x23, 0x42);
        }


        [Test]
        public void X86dis_movntps()
        {
            AssertCode32("movntps\t[edx+42],xmm0", 0x0F, 0x2B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvttps2pi()
        {
            AssertCode32("cvtps2pi\tmm0,xmmword ptr [edx+42]", 0x0F, 0x2D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_comiss()
        {
            AssertCode32("comiss\txmm0,dword ptr [edx+42]", 0x0F, 0x2F, 0x42, 0x42);
        }

        [Test]
        public void X86dis_wrmsr()
        {
            AssertCode32("wrmsr", 0x0F, 0x30, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rdtsc()
        {
            AssertCode32("rdtsc", 0x0F, 0x31, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rdmsr()
        {
            AssertCode32("rdmsr", 0x0F, 0x32, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rdpmc()
        {
            AssertCode32("rdpmc", 0x0F, 0x33, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sysenter()
        {
            AssertCode32("sysenter", 0x0F, 0x34, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sysexit()
        {
            AssertCode32("sysexit", 0x0F, 0x35, 0x42, 0x42);
        }

        [Test]
        public void X86dis_getsec()
        {
            AssertCode32("getsec", 0x0F, 0x37, 0x42, 0x42);
        }

        [Test]
        public void X86dis_vpmovsxbw()
        {
            AssertCode32("illegal", 0x0F, 0x38, 0x30, 0x42, 0x42);
            AssertCode32("vpmovzxbw\txmm0,qword ptr [edx+42]", 0x66, 0x0F, 0x38, 0x30, 0x42, 0x42);
        }


        [Test]
        public void X86dis_permq()
        {
            AssertCode32("illegal", 0x0F, 0x3A, 0x00, 0x42, 0x42);
            AssertCode32("vpermq\tymm0,[edx+42],06", 0x66, 0x0F, 0x3A, 0x00, 0x42, 0x42, 0x6);
        }

        // v
        [Test]
        public void X86dis_movmskps()
        {
            AssertCode32("movmskps\teax,xmm2", 0x0F, 0x50, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sqrtps()
        {
            AssertCode32("sqrtps\txmm0,[edx+42]", 0x0F, 0x51, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rsqrtps()
        {
            AssertCode32("rsqrtps\txmm0,[edx+42]", 0x0F, 0x52, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rcpps()
        {
            AssertCode32("rcpps\txmm0,[edx+42]", 0x0F, 0x53, 0x42, 0x42);
        }

        [Test]
        public void X86dis_andps()
        {
            AssertCode32("andps\txmm0,[edx+42]", 0x0F, 0x54, 0x42, 0x42);
        }

        [Test]
        public void X86dis_andnps()
        {
            AssertCode32("andnps\txmm0,[edx+42]", 0x0F, 0x55, 0x42, 0x42);
        }

        [Test]
        public void X86dis_orps()
        {
            AssertCode32("orps\txmm0,[edx+42]", 0x0F, 0x56, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvtps2pd()
        {
            AssertCode32("cvtps2pd\txmm0,[edx+42]", 0x0F, 0x5A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvtdq2ps()
        {
            AssertCode32("cvtdq2ps\txmm0,[edx+42]", 0x0F, 0x5B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_minps()
        {
            AssertCode32("minps\txmm0,[edx+42]", 0x0F, 0x5D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckldq()
        {
            AssertCode32("punpckldq\tmm0,dword ptr [edx+42]", 0x0F, 0x62, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtb()
        {
            AssertCode32("pcmpgtb\tmm0,dword ptr [edx+42]", 0x0F, 0x64, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtw()
        {
            AssertCode32("pcmpgtw\tmm0,dword ptr [edx+42]", 0x0F, 0x65, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtd()
        {
            AssertCode32("pcmpgtd\tmm0,dword ptr [edx+42]", 0x0F, 0x66, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packuswb()
        {
            AssertCode32("packuswb\tmm0,dword ptr [edx+42]", 0x0F, 0x67, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhbw()
        {
            AssertCode32("punpckhbw\tmm0,dword ptr [edx+42]", 0x0F, 0x68, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhwd()
        {
            AssertCode32("punpckhwd\tmm0,dword ptr [edx+42]", 0x0F, 0x69, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhdq()
        {
            AssertCode32("punpckhdq\tmm0,dword ptr [edx+42]", 0x0F, 0x6A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packssdw()
        {
            AssertCode32("packssdw\tmm0,dword ptr [edx+42]", 0x0F, 0x6B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqw()
        {
            AssertCode32("pcmpeqw\tmm0,[edx+42]", 0x0F, 0x75, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqd()
        {
            AssertCode32("pcmpeqd\tmm0,[edx+42]", 0x0F, 0x76, 0x42, 0x42);
        }

        [Test]
        public void X86dis_emms()
        {
            AssertCode32("emms", 0x0F, 0x77);
        }

        [Test]
        public void X86dis_vmread()
        {
            AssertCode32("vmread\t[edx+42],eax", 0x0F, 0x78, 0x42, 0x42);
        }


        [Test]
        public void X86dis_vmwrite()
        {
            AssertCode32("vmwrite\teax,[edx+42]", 0x0F, 0x79, 0x42, 0x42);
        }

        [Test]
        public void X86dis_btc()
        {
            AssertCode32("btc\teax,[edx+42]", 0x0F, 0xBB, 0x42, 0x42);
        }


        [Test]
        public void X86dis_bsf()
        {
            AssertCode32("bsf\teax,[edx+42]", 0x0F, 0xBC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cmpps()
        {
            AssertCode32("cmpps\txmm0,[edx+42],08", 0x0F, 0xC2, 0x42, 0x42, 0x08);
        }

        [Test]
        public void X86dis_movnti()
        {
            AssertCode32("movnti\t[edx+42],eax", 0x0F, 0xC3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pinsrw()
        {
            //$TODO check encoding; look in the Intel spec.
            AssertCode32("pinsrw\tmm0,edx", 0x0F, 0xC4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pextrw()
        {
            AssertCode32("pextrw\teax,mm2,42", 0x0F, 0xC5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_vshufps()
        {
            AssertCode32("vshufps\txmm0,[edx+42],07", 0x0F, 0xC6, 0x42, 0x42, 0x7);
        }

        [Test]
        public void X86dis_psrlq()
        {
            AssertCode32("psrlq\tmm0,[edx+42]", 0x0F, 0xD3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmullw()
        {
            AssertCode32("pmullw\tmm0,[edx+42]", 0x0F, 0xD5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmovmskb()
        {
            AssertCode32("pmovmskb\teax,mm2", 0x0F, 0xD7, 0x42);
        }

        [Test]
        public void X86dis_psubusb()
        {
            AssertCode32("psubusb\tmm0,[edx+42]", 0x0F, 0xD8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminub()
        {
            AssertCode32("pminub\tmm0,[edx+42]", 0x0F, 0xDA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddusb()
        {
            AssertCode32("paddusb\tmm0,[edx+42]", 0x0F, 0xDC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxub()
        {
            AssertCode32("pmaxub\tmm0,[edx+42]", 0x0F, 0xDE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pavgb()
        {
            AssertCode32("pavgb\tmm0,[edx+42]", 0x0F, 0xE0, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psraw()
        {
            AssertCode32("psraw\tmm0,[edx+42]", 0x0F, 0xE1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psrad()
        {
            AssertCode32("psrad\tmm0,[edx+42]", 0x0F, 0xE2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmulhuw()
        {
            AssertCode32("pmulhuw\tmm0,[edx+42]", 0x0F, 0xE4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmulhw()
        {
            AssertCode32("pmulhw\tmm0,[edx+42]", 0x0F, 0xE5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_movntq()
        {
            AssertCode32("movntq\t[edx+42],mm0", 0x0F, 0xE7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsb()
        {
            AssertCode32("psubsb\tmm0,[edx+42]", 0x0F, 0xE8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsw()
        {
            AssertCode32("psubsw\tmm0,[edx+42]", 0x0F, 0xE9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminsw()
        {
            AssertCode32("pminsw\tmm0,[edx+42]", 0x0F, 0xEA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_por()
        {
            AssertCode32("por\tmm0,[edx+42]", 0x0F, 0xEB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsb()
        {
            AssertCode32("paddsb\tmm0,[edx+42]", 0x0F, 0xEC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsw()
        {
            AssertCode32("paddsw\tmm0,[edx+42]", 0x0F, 0xED, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxsw()
        {
            AssertCode32("pmaxsw\tmm0,[edx+42]", 0x0F, 0xEE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pxor()
        {
            AssertCode32("pxor\tmm0,[edx+42]", 0x0F, 0xEF, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psllw()
        {
            AssertCode32("psllw\tmm0,[edx+42]", 0x0F, 0xF1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pslld()
        {
            AssertCode32("pslld\tmm0,[edx+42]", 0x0F, 0xF2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psllq()
        {
            AssertCode32("psllq\tmm0,[edx+42]", 0x0F, 0xF3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmuludq()
        {
            AssertCode32("pmuludq\tmm0,[edx+42]", 0x0F, 0xF4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaddwd()
        {
            AssertCode32("pmaddwd\tmm0,[edx+42]", 0x0F, 0xF5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psadbw()
        {
            AssertCode32("psadbw\tmm0,[edx+42]", 0x0F, 0xF6, 0x42, 0x42);
        }

        [Test]
        public void X86dis_maskmovq()
        {
            AssertCode32("maskmovq\tmm0,[edx+42]", 0x0F, 0xF7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubb()
        {
            AssertCode32("psubb\tmm0,[edx+42]", 0x0F, 0xF8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubw()
        {
            AssertCode32("psubw\tmm0,[edx+42]", 0x0F, 0xF9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubd()
        {
            AssertCode32("psubd\tmm0,[edx+42]", 0x0F, 0xFA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubq()
        {
            AssertCode32("psubq\tmm0,[edx+42]", 0x0F, 0xFB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddb()
        {
            AssertCode32("paddb\tmm0,[edx+42]", 0x0F, 0xFC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddw()
        {
            AssertCode32("paddw\tmm0,[edx+42]", 0x0F, 0xFD, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddd()
        {
            AssertCode32("paddd\tmm0,[edx+42]", 0x0F, 0xFE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_fcmovb()
        {
            AssertCode32("fcmovb\tst(0),st(1)", 0xDA, 0xC1);
        }

        [Test]
        public void X86dis_fcmove()
        {
            AssertCode32("fcmove\tst(0),st(1)", 0xDA, 0xC9);
        }

        [Test]
        public void X86dis_fcmovbe()
        {
            AssertCode32("fcmovbe\tst(0),st(1)", 0xDA, 0xD1);
        }

        [Test]
        public void X86dis_fcmovu()
        {
            AssertCode32("fcmovu\tst(0),st(1)", 0xDA, 0xD9);
        }

        [Test]
        public void X86dis_fucompp()
        {
            AssertCode32("fucompp", 0xDA, 0xE9);
        }

        [Test]
        public void X86dis_fisttp()
        {
            AssertCode32("fisttp\tdword ptr [eax]", 0xDB, 0x08);
        }

        [Test]
        public void X86dis_fld_real80()
        {
            AssertCode32("fld\ttword ptr [eax]", 0xDB, 0x28);
        }

        [Test]
        public void X86dis_fcmovne()
        {
            AssertCode32("fcmovne\tst(0),st(1)", 0xDB, 0xC9);
        }

        [Test]
        public void X86dis_fcmovnbe()
        {
            AssertCode32("fcmovnbe\tst(0),st(1)", 0xDB, 0xD1);
        }

        [Test]
        public void X86dis_fcmovnu()
        {
            AssertCode32("fcmovnu\tst(0),st(1)", 0xDB, 0xD9);
        }

        [Test]
        public void X86dis_fclex()
        {
            AssertCode32("fclex", 0xDB, 0xE2);
        }

        [Test]
        public void X86dis_finit()
        {
            AssertCode32("fninit", 0xDB, 0xE3);
        }


        [Test]
        public void X86dis_fisttp_i64()
        {
            AssertCode32("fisttp\tqword ptr [eax+42]", 0xDD, 0x48, 0x42);
        }


        [Test]
        public void X86dis_ffree()
        {
            AssertCode32("ffree\tst(2)", 0xDD, 0xC2);
        }


        [Test]
        public void X86dis_fucom()
        {
            AssertCode32("fucom\tst(5),st(0)", 0xDD, 0xE5);
        }

        [Test]
        public void X86dis_fucomp()
        {
            AssertCode32("fucomp\tst(2)", 0xDD, 0xEA);
        }

        [Test]
        public void X86dis_fisttp_int16()
        {
            AssertCode32("fisttp\tword ptr [eax+42]", 0xDF, 0x48, 0x42);
        }

        [Test]
        public void X86dis_fild_i16()
        {
            AssertCode32("fild\tword ptr [eax+42]", 0xDF, 0x40, 0x42);
        }

        [Test]
        public void X86dis_fcomip()
        {
            AssertCode32("fcomip\tst(0),st(2)", 0xDF, 0xF2);
        }


        [Test]
        public void X86Dis_vaddsubpd_c531d04401c8()
        {
            var instr = Disassemble64(0xc5, 0x31, 0xd0, 0x44, 0x01, 0xc8);
            Assert.AreEqual("vaddsubpd\txmm8,xmm9,[rcx+rax-38]", instr.ToString());
        }

        [Test]
        public void X86Dis_sldt_0f00c1()
        {
            var instr = Disassemble64(0x0f, 0x00, 0xc1);
            Assert.AreEqual("sldt\tecx", instr.ToString());
        }

        [Test]
        public void X86Dis_sgdt()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x00);
            Assert.AreEqual("sgdt\t[rax]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmcall()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xC1);
            Assert.AreEqual("vmcall", instr.ToString());
        }

        [Test]
        public void X86Dis_sidt()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x0d, 0xd6, 0xd7, 0x0a, 0x01);
            Assert.AreEqual("sidt\t[rip+010AD7D6]", instr.ToString());
        }

        [Test]
        public void X86Dis_lgdt()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x14, 0x0f);
            Assert.AreEqual("lgdt\t[rdi+rcx]", instr.ToString());
        }

        [Test]
        public void X86Dis_mwait()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xC9);
            Assert.AreEqual("mwait", instr.ToString());
        }

        [Test]
        public void X86Dis_invalid_0f6c()
        {
            var instr = Disassemble64(0x0f, 0x6c);
            Assert.AreEqual("illegal", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpcklqdq()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x6c, 0xC3);
            Assert.AreEqual("vpunpcklqdq\txmm0,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_invalid_0f6d()
        {
            var instr = Disassemble64(0x0f, 0x6d);
            Assert.AreEqual("illegal", instr.ToString());
        }

        [Test]
        public void X86Dis_ud0_0fffff()
        {
            var instr = Disassemble64(0x0f, 0xff, 0xff);
            Assert.AreEqual("ud0\tedi,edi", instr.ToString());
        }

        [Test]
        public void X86Dis_bt_imm()
        {
            var instr = Disassemble64(0x0F, 0xBA, 0xE3, 0x04);
            Assert.AreEqual("bt\tebx,04", instr.ToString());
        }

        [Test]
        public void X86Dis_rdrand()
        {
            var instr = Disassemble64(0x0f, 0xc7, 0xF3);
            Assert.AreEqual("rdrand\tebx", instr.ToString());
        }

        [Test]
        public void X86Dis_fcomi()
        {
            var instr = Disassemble64(0xdb, 0xf1);
            Assert.AreEqual("fcomi\tst(0),st(1)", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubpd()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0xd0, 0xfe);
            Assert.AreEqual("vaddsubpd\txmm7,xmm2,xmm6", instr.ToString());
        }

        [Test]
        public void X86Dis_vblendvpdv()
        {
            AssertCode32("vblendvpdv\txmm0,xmm2,xmm4", 0x0F, 0x3A, 0x4B, 0xC2, 0x42);
        }

        [Test]
        public void X86Dis_phsubsw()
        {
            AssertCode32("phsubsw\tmm0,mm2", 0x0F, 0x38, 0x07, 0xC2);
        }


        [Test]
        public void X86Dis_vcvttpd2dq()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0xe6, 0xf5);
            Assert.AreEqual("vcvttpd2dq\txmm6,xmm5", instr.ToString());
        }

        [Test]
        public void X86Dis_nop_0f19c0()
        {
            var instr = Disassemble64(0x0f, 0x19, 0xc0);
            Assert.AreEqual("nop\teax", instr.ToString());
        }

        [Test]
        public void X86Dis_nop_0f19c1()
        {
            var instr = Disassemble64(0x0f, 0x19, 0xc0);
            Assert.AreEqual("nop\teax", instr.ToString());
        }

        [Test]
        public void X86Dis_lidt()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x1f);
            Assert.AreEqual("lidt\t[rdi]", instr.ToString());
        }

        [Test]
        public void X86Dis_smsw()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x61, 0x40);
            Assert.AreEqual("smsw\tword ptr [rcx+40]", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x74, 0x45, 0x48);
            Assert.AreEqual("lmsw\tword ptr [rbp+rax*2+48]", instr.ToString());
        }

        [Test]
        public void X86Dis_invlpg()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x78, 0x16);
            Assert.AreEqual("invlpg\tbyte ptr [rax+16]", instr.ToString());
        }


        [Test]
        public void X86Dis_vmresume_0f01c3()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xc3);
            Assert.AreEqual("vmresume", instr.ToString());
        }

        [Test]
        public void X86Dis_vmxoff_0f01c4()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xc4);
            Assert.AreEqual("vmxoff", instr.ToString());
        }

        [Test]
        public void X86Dis_monitor()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xc8);
            Assert.AreEqual("monitor", instr.ToString());
        }

        [Test]
        public void X86Dis_smsw_0f01e6()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xe6);
            Assert.AreEqual("smsw\tesi", instr.ToString());
        }

        [Test]
        public void X86Dis_rdpkru_0f01ee()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xee);
            Assert.AreEqual("rdpkru", instr.ToString());
        }

        [Test]
        public void X86Dis_wrpkru()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xef);
            Assert.AreEqual("wrpkru", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw_0f01f0()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xf0);
            Assert.AreEqual("lmsw\tax", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw_0f01f3()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xf3);
            Assert.AreEqual("lmsw\tbx", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw_0f01f6()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xf6);
            Assert.AreEqual("lmsw\tsi", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw_0f01f7()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xf7);
            Assert.AreEqual("lmsw\tdi", instr.ToString());
        }

        [Test]
        public void X86Dis_swapgs()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xf8);
            Assert.AreEqual("swapgs", instr.ToString());
        }

        [Test]
        public void X86Dis_monitorx()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xfa);
            Assert.AreEqual("monitorx", instr.ToString());
        }

        [Test]
        public void X86Dis_mwaitx()
        {
            var instr = Disassemble64(0x0f, 0x01, 0xfb);
            Assert.AreEqual("mwaitx", instr.ToString());
        }

        [Test]
        public void X86Dis_unpckhps_0f1500()
        {
            var instr = Disassemble64(0x0f, 0x15, 0x00);
            Assert.AreEqual("unpckhps\txmm0,[rax]", instr.ToString());
        }

        [Test]
        public void X86Dis_cldemote_0f1c00()
        {
            var instr = Disassemble64(0x0f, 0x1c, 0x00);
            Assert.AreEqual("cldemote\tbyte ptr [rax]", instr.ToString());
        }

        [Test]
        public void X86Dis_pshufb_0f38004154()
        {
            var instr = Disassemble64(0x0f, 0x38, 0x00, 0x41, 0x54);
            Assert.AreEqual("pshufb\tmm0,[rcx+54]", instr.ToString());
        }

        [Test]
        public void X86Dis_sha1msg2()
        {
            var instr = Disassemble64(0x0f, 0x38, 0xca, 0x74, 0x0b, 0x48);
            Assert.AreEqual("sha1msg2\txmm6,[rbx+rcx+48]", instr.ToString());
        }

        [Test]
        public void X86Dis_packsswb_0f63dd()
        {
            var instr = Disassemble64(0x0f, 0x63, 0xdd);
            Assert.AreEqual("packsswb\tmm3,mm5", instr.ToString());
        }


        [Test]
        public void X86Dis_psrld()
        {
            var instr = Disassemble64(0x0f, 0x72, 0xD3, 0x0B);
            Assert.AreEqual("psrld\tmm3,0B", instr.ToString());
        }

        [Test]
        public void X86Dis_psrlq()
        {
            var instr = Disassemble64(0x0f, 0x73, 0xD3, 0x02);
            Assert.AreEqual("psrlq\tmm3,02", instr.ToString());
        }

        [Test]
        public void X86Dis_vhaddpd()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7c, 0xC3);
            Assert.AreEqual("vhaddpd\txmm0,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_vhaddps()
        {
            var instr = Disassemble64(0xF2, 0x0f, 0x7c, 0xC3);
            Assert.AreEqual("vhaddps\txmm0,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_vhsubpd()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7d, 0xD4);
            Assert.AreEqual("vhsubpd\txmm2,xmm4", instr.ToString());
        }

        [Test]
        public void X86Dis_rsm()
        {
            var instr = Disassemble64(0x0f, 0xaa);
            Assert.AreEqual("rsm", instr.ToString());
        }

        [Test]
        public void X86Dis_xsave()
        {
            var instr = Disassemble32(0x0f, 0xae, 0x27);
            Assert.AreEqual("xsave\tbyte ptr [edi]", instr.ToString());
        }

        [Test]
        public void X86Dis_btr()
        {
            var instr = Disassemble64(0x0f, 0xb3, 0x44, 0x24, 0x30);
            Assert.AreEqual("btr\t[rsp+30],eax", instr.ToString());
        }

        [Test]
        public void X86Dis_jmpe()
        {
            var instr = Disassemble64(0x0f, 0xb8);
            Assert.AreEqual("jmpe", instr.ToString());
        }

        [Test]
        public void X86Dis_ud1()
        {
            var instr = Disassemble64(0x0f, 0xb9, 0xa0, 0x01, 0x00, 0x00, 0x0f);
            Assert.AreEqual("ud1\tesp,[rax+0F000001]", instr.ToString());
        }

        [Test]
        public void X86Dis_addsubpd()
        {
            var instr = Disassemble64(0x66, 0x0f, 0xd0, 0xD3);
            Assert.AreEqual("addsubpd\txmm2,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_cvtdq2pd()
        {
            var instr = Disassemble64(0xF3, 0x0F, 0xE6, 0xC4);
            Assert.AreEqual("cvtdq2pd\txmm0,xmm4", instr.ToString());
        }

        [Test]
        public void X86Dis_sldt()
        {
            var instr = Disassemble64(0x26, 0x0f, 0x00, 0x41, 0x83);
            Assert.AreEqual("sldt\tword ptr es:[rcx-7D]", instr.ToString());
        }

        [Test]
        public void X86Dis_str()
        {
            var instr = Disassemble64(0x2e, 0x0f, 0x00, 0x48, 0x85);
            Assert.AreEqual("str\tword ptr cs:[rax-7B]", instr.ToString());
        }

        [Test]
        public void X86Dis_xsave64_480fae27()
        {
            var instr = Disassemble64(0x48, 0x0f, 0xae, 0x27);
            Assert.AreEqual("xsave64\tbyte ptr [rdi]", instr.ToString());
        }

        [Test]
        public void X86Dis_btr_rax()
        {
            var instr = Disassemble64(0x48, 0x0f, 0xb3, 0x44, 0x24, 0x30);
            Assert.AreEqual("btr\t[rsp+30],rax", instr.ToString());
        }

        [Test]
        public void X86Dis_cmpxchg16b()
        {
            var instr = Disassemble64(0x0f, 0xc7, 0x4c, 0x24, 0x20);
            Assert.AreEqual("cmpxchg16b\txmmword ptr [rsp+20]", instr.ToString());
        }

        [Test]
        public void X86Dis_invpcid()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x38, 0x82, 0x01);
            Assert.AreEqual("invpcid\teax,xmmword ptr [rcx]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb_c4014963cc()
        {
            var instr = Disassemble64(0xc4, 0x01, 0x49, 0x63, 0xcc);
            Assert.AreEqual("vpacksswb\txmm9,xmm6,xmm12", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb_c40175634183()
        {
            var instr = Disassemble64(0xc4, 0x01, 0x75, 0x63, 0x41, 0x83);
            Assert.AreEqual("vpacksswb\tymm8,ymm1,[r9-7D]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq()
        {
            var instr = Disassemble64(0xc4, 0x01, 0x75, 0x6d, 0x48, 0x89);
            Assert.AreEqual("vpunpckhqdq\tymm9,ymm1,[r8-77]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpshufb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x41, 0x00, 0x48, 0x8b);
            Assert.AreEqual("vpshufb\txmm9,xmm7,[r8-75]", instr.ToString());
        }

        [Test]
        public void X86Dis_vfnmsub231ps_c40241beff()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x41, 0xbe, 0xff);
            Assert.AreEqual("vfnmsub231ps\txmm15,xmm7,xmm15", instr.ToString());
        }

        [Test]
        public void X86Dis_vtestpd_c402450fb63424()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x45, 0x0f, 0xb6, 0x34, 0x24, 0x00, 0x00);
            Assert.AreEqual("vtestpd\tymm14,[r14+00002434]", instr.ToString());
        }

        [Test]
        public void X86Dis_phaddsw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x74, 0x03, 0xC3);
            Assert.AreEqual("phaddsw\tmm0,mm3", instr.ToString());
        }

        [Test]
        public void X86Dis_phsubw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x74, 0x05, 0xC3);
            Assert.AreEqual("phsubw\tmm0,mm3", instr.ToString());
        }

        [Test]
        public void X86Dis_psignb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x74, 0x08, 0xC3);
            Assert.AreEqual("psignb\tmm0,mm3", instr.ToString());
        }

        [Test]
        public void X86Dis_vphaddd_c4027502f3()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x02, 0xf3);
            Assert.AreEqual("vphaddd\tymm14,ymm1,ymm11", instr.ToString());
        }
        [Test]
        public void X86Dis_vphaddsw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x03, 0x31);
            Assert.AreEqual("vphaddsw\tymm14,ymm1,[r9]", instr.ToString());
        }
        [Test]
        public void X86Dis_vphsubw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x05, 0x5b, 0x5d);
            Assert.AreEqual("vphsubw\tymm11,ymm1,[r11+5D]", instr.ToString());
        }

        [Test]
        public void X86Dis_vphsubsw_c4027507f0()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x07, 0xf0);
            Assert.AreEqual("vphsubsw\tymm14,ymm1,ymm8", instr.ToString());
        }

        [Test]
        public void X86Dis_vpsignb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x08, 0x41, 0xf6);
            Assert.AreEqual("vpsignb\tymm8,ymm1,[r9-0A]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmulhrsw_c402750b4883()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x0b, 0x48, 0x83);
            Assert.AreEqual("vpmulhrsw\tymm9,ymm1,[r8-7D]", instr.ToString());
        }


        [Test]
        public void X86Dis_vpermps()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x16, 0x48, 0x89);
            Assert.AreEqual("vpermps\tymm9,ymm1,[r8-77]", instr.ToString());
        }

        [Test]
        public void X86Dis_vptest()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x17, 0x31);
            Assert.AreEqual("vptest\tymm14,[r9]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpabsd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x1e, 0x48, 0x8b);
            Assert.AreEqual("vpabsd\tymm9,[r8-75]", instr.ToString());
        }
        [Test]
        [Ignore("addressing mode is off")]
        public void X86Dis_vpmovsxbw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x20, 0x49, 0x83);
            Assert.AreEqual("vpmovsxbw\tymm9,qword ptr [r9-7d]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmovsxbd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x21, 0x65, 0x8b);
            Assert.AreEqual("vpmovsxbd\tymm12,qword ptr [r13-75]", instr.ToString());
        }

        [Test]
        [Ignore("addressing mode is off")]
        public void X86Dis_vpmovsxbq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x22, 0x41, 0x8b);
            Assert.AreEqual("vpmovsxbq\tymm8,[r9-75]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmovsxwq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x24, 0x48, 0x39);
            Assert.AreEqual("vpmovsxwq\tymm9,qword ptr [r8+39]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmuldq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x28, 0x31);
            Assert.AreEqual("vpmuldq\tymm14,ymm1,[r9]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpcmpeqq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x29, 0x4b, 0x15);
            Assert.AreEqual("vpcmpeqq\tymm9,ymm1,[r11+15]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovpd_toreg()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2d, 0x5b, 0x48);
            Assert.AreEqual("vmaskmovpd\tymm11,ymm1,[r11+48]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovps_tomem()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2e, 0x80, 0x00, 0x34, 0x12, 0x00);
            Assert.AreEqual("vmaskmovps\t[r8+00123400],ymm1,ymm8", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovpd_tomem()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2f, 0x4b, 0x43);
            Assert.AreEqual("vmaskmovpd\t[r11+43],ymm1,ymm9", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmovzxbd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x31, 0x41, 0x83);
            Assert.AreEqual("vpmovzxbd\tymm8,qword ptr [r9-7D]", instr.ToString());
        }

        [Test]
        [Ignore("addressing mode is off")]
        public void X86Dis_vpmovzxbq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x32, 0x4b, 0x7b);
            Assert.AreEqual("vpmovzxbq\tymm9,DWORD PTR [r11+7B]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpminuw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3a, 0xf6);
            Assert.AreEqual("vpminuw\tymm14,ymm1,ymm14", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmaxuw()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3e, 0x40, 0x70);
            Assert.AreEqual("vpmaxuw\tymm8,ymm1,[r8+70]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmaxud()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3f, 0x49, 0x8b);
            Assert.AreEqual("vpmaxud\tymm9,ymm1,[r9-75]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpsllvd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x47, 0x0f);
            Assert.AreEqual("vpsllvd\tymm9,ymm1,[r15]", instr.ToString());
        }

        [Test]
        public void X86Dis_vbroadcastb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x78, 0x4c, 0x89, 0xe7);
            Assert.AreEqual("vbroadcastb\tymm9,byte ptr [r9+r9*4-19]", instr.ToString());
        }

        [Test]
        public void X86Dis_vfmadd213ps()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xa8, 0x48, 0x63);
            Assert.AreEqual("vfmadd213ps\tymm9,ymm1,[r8+63]", instr.ToString());
        }

        [Test]
        public void X86Dis_vaesenc()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xdc, 0x43, 0xe0, 0xf7, 0x00);
            Assert.AreEqual("vaesenc\txmm8,xmm1,[r11-20]", instr.ToString());
        }

        [Test]
        [Ignore("addressing mode is off")]
        public void X86Dis_vpgatherqq_c402e99100()
        {
            var instr = Disassemble64(0xc4, 0x02, 0xe9, 0x91, 0x00);
            Assert.AreEqual("vpgatherqq\txmm8,QWORD PTR [r8],xmm2", instr.ToString());
        }
        [Test]
        public void X86Dis_vpshufb_ymm()
        {
            var instr = Disassemble64(0xc4, 0x22, 0x2d, 0x00, 0x48, 0x8b);
            Assert.AreEqual("vpshufb\tymm9,ymm10,[rax-75]", instr.ToString());
        }
        [Test]
        public void X86Dis_vphaddw()
        {
            var instr = Disassemble64(0xc4, 0x62, 0x05, 0x01, 0x0f);
            Assert.AreEqual("vphaddw\tymm9,ymm15,[rdi]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq_sib()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x3d, 0x6d, 0xb4, 0x0d, 0x01, 0x1, 0x01, 0x01);
            Assert.AreEqual("vpunpckhqdq\tymm6,ymm8,[r13+r9+01010101]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpermpd()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc5, 0x01, 0xeb, 0xae);
            Assert.AreEqual("vpermpd\tymm5,ymm11,AE", instr.ToString());
        }

        [Test]
        public void X86Dis_femms()
        {
            var instr = Disassemble64(0xc4, 0xc1, 0xc0, 0x0e);
            Assert.AreEqual("femms", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubpd_c509d0e8()
        {
            var instr = Disassemble64(0xc5, 0x09, 0xd0, 0xe8);
            Assert.AreEqual("vaddsubpd\txmm13,xmm14,xmm0", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubps_c537d0ff()
        {
            var instr = Disassemble64(0xc5, 0x37, 0xd0, 0xff);
            Assert.AreEqual("vaddsubps\tymm15,ymm9,ymm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vcvtdq2pd()
        {
            var instr = Disassemble64(0xc5, 0x3a, 0xe6, 0x81, 0x5b, 0x5d, 0x41, 0x5C);
            Assert.AreEqual("vcvtdq2pd\txmm8,[rcx+5C415D5B]", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubpd_c541d0ed()
        {
            var instr = Disassemble64(0xc5, 0x41, 0xd0, 0xed);
            Assert.AreEqual("vaddsubpd\txmm13,xmm7,xmm5", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb()
        {
            var instr = Disassemble64(0xc5, 0x49, 0x63, 0x44, 0x24, 0x04);
            Assert.AreEqual("vpacksswb\txmm8,xmm6,[rsp+04]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb_c54d63f5()
        {
            var instr = Disassemble64(0xc5, 0x4d, 0x63, 0xf5);
            Assert.AreEqual("vpacksswb\tymm14,ymm6,ymm5", instr.ToString());
        }

        [Test]
        public void X86Dis_vmovlps()
        {
            var instr = Disassemble64(0xc5, 0x50, 0x13, 0x00);
            Assert.AreEqual("vmovlps\tqword ptr [rax],xmm8", instr.ToString());
        }

        [Test]
        public void X86Dis_vcvttpd2dq_c551e6ff()
        {
            var instr = Disassemble64(0xc5, 0x51, 0xe6, 0xff);
            Assert.AreEqual("vcvttpd2dq\txmm15,xmm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_c56015ca()
        {
            var instr = Disassemble64(0xc5, 0x60, 0x15, 0xca);
            Assert.AreEqual("vunpckhps\txmm9,xmm3,xmm2", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps()
        {
            var instr = Disassemble64(0xc5, 0x74, 0x15, 0x4b, 0x50);
            Assert.AreEqual("vunpckhps\tymm9,ymm1,[rbx+50]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq_ymm()
        {
            var instr = Disassemble64(0xc5, 0x75, 0x6d, 0x31);
            Assert.AreEqual("vpunpckhqdq\tymm14,ymm1,[rcx]", instr.ToString());
        }

        [Test]
        public void X86Dis_vhaddpd_ymm()
        {
            var instr = Disassemble64(0xc5, 0x75, 0x7c, 0x48, 0x8b);
            Assert.AreEqual("vhaddpd\tymm9,ymm1,[rax-75]", instr.ToString());
        }

        [Test]
        public void X86Dis_vhsubpd_ymm()
        {
            var instr = Disassemble64(0xc5, 0x75, 0x7d, 0x4c, 0x8d, 0xa3);
            Assert.AreEqual("vhsubpd\tymm9,ymm1,[rbp+rcx*4-5D]", instr.ToString());
        }

        [Test]
        public void X86Dis_vcvttpd2dq_mem()
        {
            var instr = Disassemble64(0xc5, 0x75, 0xe6, 0x31);
            Assert.AreEqual("vcvttpd2dq\txmm14,ymmword ptr [rcx]", instr.ToString());
        }

        [Test]
        public void X86Dis_vhaddps_c5777cf7()
        {
            var instr = Disassemble64(0xc5, 0x77, 0x7c, 0xf7);
            Assert.AreEqual("vhaddps\tymm14,ymm1,ymm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubps()
        {
            var instr = Disassemble64(0xc5, 0x77, 0xd0, 0x48, 0x89);
            Assert.AreEqual("vaddsubps\tymm9,ymm1,[rax-77]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpcklqdq_ymm()
        {
            var instr = Disassemble64(0xc5, 0x7d, 0x6c, 0x45, 0x31);
            Assert.AreEqual("vpunpcklqdq\tymm8,ymm0,[rbp+31]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmovlps_memdst()
        {
            var instr = Disassemble64(0xc5, 0x80, 0x13, 0x02);
            Assert.AreEqual("vmovlps\tqword ptr [rdx],xmm0", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_c58015c4()
        {
            var instr = Disassemble64(0xc5, 0x80, 0x15, 0xc4);
            Assert.AreEqual("vunpckhps\txmm0,xmm15,xmm4", instr.ToString());
        }

        [Test]
        public void X86Dis_vcvtpd2dq()
        {
            var instr = Disassemble64(0xc5, 0x83, 0xe6, 0x04, 0x74);
            Assert.AreEqual("vcvtpd2dq\txmm0,[rsp+rsi*2]", instr.ToString());
        }

        [Test]
        public void X86Dis_vlddqu_c583f001()
        {
            var instr = Disassemble64(0xc5, 0x83, 0xf0, 0x01);
            Assert.AreEqual("vlddqu\txmm0,[rcx]", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_ymm()
        {
            var instr = Disassemble64(0xc5, 0x8c, 0x15, 0x00);
            Assert.AreEqual("vunpckhps\tymm0,ymm14,[rax]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpcklqdq_rip()
        {
            var instr = Disassemble64(0xc5, 0x8d, 0x6c, 0x2d, 0xff, 0x89, 0xe8, 0x4C);
            Assert.AreEqual("vpunpcklqdq\tymm5,ymm14,[rip+4CE889FF]", instr.ToString());
        }

        [Test]
        public void X86Dis_vcvtpd2dq_c59fe6ff()
        {
            var instr = Disassemble64(0xc5, 0x9f, 0xe6, 0xff);
            Assert.AreEqual("vcvtpd2dq\txmm7,ymm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq_c5b96deb()
        {
            var instr = Disassemble64(0xc5, 0xb9, 0x6d, 0xeb);
            Assert.AreEqual("vpunpckhqdq\txmm5,xmm8,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_c5c015c0()
        {
            var instr = Disassemble64(0xc5, 0xc0, 0x15, 0xc0);
            Assert.AreEqual("vunpckhps\txmm0,xmm7,xmm0", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_c5e015c4()
        {
            var instr = Disassemble64(0xc5, 0xe0, 0x15, 0xc4);
            Assert.AreEqual("vunpckhps\txmm0,xmm3,xmm4", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb_xmm()
        {
            var instr = Disassemble64(0xc5, 0xe1, 0x63, 0x00);
            Assert.AreEqual("vpacksswb\txmm0,xmm3,[rax]", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhps_xmm()
        {
            var instr = Disassemble64(0xc5, 0xe8, 0x15, 0x17);
            Assert.AreEqual("vunpckhps\txmm2,xmm2,[rdi]", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhpd_c5e915ff()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x15, 0xff);
            Assert.AreEqual("vunpckhpd\txmm7,xmm2,xmm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vpacksswb_c5e963ff()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x63, 0xff);
            Assert.AreEqual("vpacksswb\txmm7,xmm2,xmm7", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpcklqdq_c5e96cfb()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x6c, 0xfb);
            Assert.AreEqual("vpunpcklqdq\txmm7,xmm2,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq_c5e96dfe()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x6d, 0xfe);
            Assert.AreEqual("vpunpckhqdq\txmm7,xmm2,xmm6", instr.ToString());
        }

        [Test]
        public void X86Dis_vpslld_c5e972f2ff()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x72, 0xf2, 0xff);
            Assert.AreEqual("vpslld\txmm2,xmm2,FF", instr.ToString());
        }

        [Test]
        public void X86Dis_vhaddpd_c5e97cfd()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0x7c, 0xfd);
            Assert.AreEqual("vhaddpd\txmm7,xmm2,xmm5", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubps_c5efd0ff()
        {
            var instr = Disassemble64(0xc5, 0xef, 0xd0, 0xff);
            Assert.AreEqual("vaddsubps\tymm7,ymm2,ymm7", instr.ToString());
        }

        [Test]
        public void X86Dis_fneni()
        {
            var instr = Disassemble64(0xdb, 0xe0);
            Assert.AreEqual("fneni", instr.ToString());
        }

        [Test]
        public void X86Dis_fndisi()
        {
            var instr = Disassemble64(0xdb, 0xe1);
            Assert.AreEqual("fndisi", instr.ToString());
        }

        [Test]
        public void X86Dis_fnsetpm_287_dbe4()
        {
            var instr = Disassemble64(0xdb, 0xe4);
            Assert.AreEqual("fnsetpm", instr.ToString());
        }

        [Test]
        public void X86Dis_frstpm_287_dbe5()
        {
            var instr = Disassemble64(0xdb, 0xe5);
            Assert.AreEqual("frstpm", instr.ToString());
        }

        [Test]
        public void X86Dis_ffreep_dfc1()
        {
            var instr = Disassemble64(0xdf, 0xc1);
            Assert.AreEqual("ffreep\tst(1)", instr.ToString());
        }

        // Sad tests below

#if false

        [Test]
        public void X86Dis__bad__c4027514()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x14);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpermps_c40275164889()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x16, 0x48, 0x89);
            Assert.AreEqual("vpermps\tymm9,ymm1,YMMWORD PTR [r8-0x77]", instr.ToString());
        }
        [Test]
        public void X86Dis_vptest_c402751731()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x17, 0x31);
            Assert.AreEqual("vptest\tymm14,YMMWORD PTR [r9]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402751b()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x1b);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpabsd_c402751e488b()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x1e, 0x48, 0x8b);
            Assert.AreEqual("vpabsd\tymm9,YMMWORD PTR [r8-0x75]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovsxbw_c40275204983()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x20, 0x49, 0x83);
            Assert.AreEqual("vpmovsxbw\tymm9,XMMWORD PTR [r9-0x7d]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovsxbd_c4027521658b()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x21, 0x65, 0x8b);
            Assert.AreEqual("vpmovsxbd\tymm12,QWORD PTR [r13-0x75]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovsxbq_c4027522418b()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x22, 0x41, 0x8b);
            Assert.AreEqual("vpmovsxbq\tymm8,DWORD PTR [r9-0x75]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovsxwq_c40275244839()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x24, 0x48, 0x39);
            Assert.AreEqual("vpmovsxwq\tymm9,QWORD PTR [r8+0x39]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmuldq_c402752831()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x28, 0x31);
            Assert.AreEqual("vpmuldq\tymm14,ymm1,YMMWORD PTR [r9]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpcmpeqq_c40275298b15f8()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x29, 0x8b, 0x15, 0xf8);
            Assert.AreEqual("vpcmpeqq\tymm9,ymm1,YMMWORD PTR [r11+0x56b5f815]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402752a()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2a);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vmaskmovpd_c402752d5b48()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2d, 0x5b, 0x48);
            Assert.AreEqual("vmaskmovpd\tymm11,ymm1,YMMWORD PTR [r11+0x48]", instr.ToString());
        }
        [Test]
        public void X86Dis_vmaskmovps_c402752e80cc02()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2e, 0x80, 0xcc, 0x02);
            Assert.AreEqual("vmaskmovps\tYMMWORD PTR [r8-0x76befd34],ymm1,ymm8", instr.ToString());
        }
        [Test]
        public void X86Dis_vmaskmovpd_c402752f8b433c()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2f, 0x8b, 0x43, 0x3c);
            Assert.AreEqual("vmaskmovpd\tYMMWORD PTR [r11-0x9cec3bd],ymm1,ymm9", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovzxbd_c40275314183()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x31, 0x41, 0x83);
            Assert.AreEqual("vpmovzxbd\tymm8,QWORD PTR [r9-0x7d]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmovzxbq_c40275328b7b10()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x32, 0x8b, 0x7b, 0x10);
            Assert.AreEqual("vpmovzxbq\tymm9,DWORD PTR [r11-0x12ceef85]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpminuw_c402753af6()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3a, 0xf6);
            Assert.AreEqual("vpminuw\tymm14,ymm1,ymm14", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmaxuw_c402753e807f4c()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3e, 0x80, 0x7f, 0x4c);
            Assert.AreEqual("vpmaxuw\tymm8,ymm1,YMMWORD PTR [r8+0x74004c7f]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmaxud_c402753f498b()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3f, 0x49, 0x8b);
            Assert.AreEqual("vpmaxud\tymm9,ymm1,YMMWORD PTR [r9-0x75]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027543()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x43);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027544()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x44);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpsllvd_c40275470f()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x47, 0x0f);
            Assert.AreEqual("vpsllvd\tymm9,ymm1,YMMWORD PTR [r15]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027548()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x48);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402754c()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x4c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027551()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x51);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027554()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x54);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027555()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x55);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027556()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x56);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027566()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x66);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027569()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x69);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402756d()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x6d);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4027571()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x71);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpbroadcastb_c40275784c89e7()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x78, 0x4c, 0x89, 0xe7);
            Assert.AreEqual("vpbroadcastb\tymm9,BYTE PTR [r9+r9*4-0x19]", instr.ToString());
        }
        [Test]
        public void X86Dis_vfmadd213ps_c40275a84863()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xa8, 0x48, 0x63);
            Assert.AreEqual("vfmadd213ps\tymm9,ymm1,YMMWORD PTR [r8+0x63]", instr.ToString());
        }
        [Test]
        public void X86Dis_vaesenc_c40275dc83e0f7()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xdc, 0x83, 0xe0, 0xf7, 0x00);
            Assert.AreEqual("vaesenc\tymm8,ymm1,YMMWORD PTR [r11-0x76bb0820]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c40275f6()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xf6);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4028803()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x88, 0x03);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4028817()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x88, 0x17);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4029ce4()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x9c, 0xe4);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402e86f()
        {
            var instr = Disassemble64(0xc4, 0x02, 0xe8, 0x6f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402e90d()
        {
            var instr = Disassemble64(0xc4, 0x02, 0xe9, 0x0d);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpgatherqq_c402e99100()
        {
            var instr = Disassemble64(0xc4, 0x02, 0xe9, 0x91, 0x00);
            Assert.AreEqual("vpgatherqq\txmm8,QWORD PTR [r8],xmm2", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c402ff00()
        {
            var instr = Disassemble64(0xc4, 0x02, 0xff, 0x00);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4030083()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x00, 0x83);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4034c8b()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x4c, 0x8b);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4036689()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x66, 0x89);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4037402()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x74, 0x02);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4037421()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x74, 0x21);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4037430()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x74, 0x30);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c40374df()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x74, 0xdf);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c40375cb()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x75, 0xcb);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c40384d2()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x84, 0xd2);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4038853()
        {
            var instr = Disassemble64(0xc4, 0x03, 0x88, 0x53);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c403c744()
        {
            var instr = Disassemble64(0xc4, 0x03, 0xc7, 0x44);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c403e860()
        {
            var instr = Disassemble64(0xc4, 0x03, 0xe8, 0x60);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c403f6c2()
        {
            var instr = Disassemble64(0xc4, 0x03, 0xf6, 0xc2);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4210100()
        {
            var instr = Disassemble64(0xc4, 0x21, 0x01, 0x00);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpshufb_c4222d00488b()
        {
            var instr = Disassemble64(0xc4, 0x22, 0x2d, 0x00, 0x48, 0x8b);
            Assert.AreEqual("vpshufb\tymm9,ymm10,YMMWORD PTR [rax-0x75]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4230041()
        {
            var instr = Disassemble64(0xc4, 0x23, 0x00, 0x41);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c42370e2()
        {
            var instr = Disassemble64(0xc4, 0x23, 0x70, 0xe2);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c441807c()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x80, 0x7c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c44183c7()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x83, 0xc7);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c44183e6()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x83, 0xe6);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c44189c7()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x89, 0xc7);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4418b01()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x8b, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4418b04()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x8b, 0x04);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4418b7d()
        {
            var instr = Disassemble64(0xc4, 0x41, 0x8b, 0x7d);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c441f6c7()
        {
            var instr = Disassemble64(0xc4, 0x41, 0xf6, 0xc7);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c441ffc7()
        {
            var instr = Disassemble64(0xc4, 0x41, 0xff, 0xc7);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c442803c()
        {
            var instr = Disassemble64(0xc4, 0x42, 0x80, 0x3c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4428d1c()
        {
            var instr = Disassemble64(0xc4, 0x42, 0x8d, 0x1c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4428d3c()
        {
            var instr = Disassemble64(0xc4, 0x42, 0x8d, 0x3c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4612900()
        {
            var instr = Disassemble64(0xc4, 0x61, 0x29, 0x00, 0xE4);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4620401()
        {
            var instr = Disassemble64(0xc4, 0x62, 0x04, 0x01, 0xE4);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vphaddw_c46205010f()
        {
            var instr = Disassemble64(0xc4, 0x62, 0x05, 0x01, 0x0f);
            Assert.AreEqual("vphaddw\tymm9,ymm15,YMMWORD PTR [rdi]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c463e67e()
        {
            var instr = Disassemble64(0xc4, 0x63, 0xe6, 0x7e);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpunpckhqdq_c4813d6db40d01()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x3d, 0x6d, 0xb4, 0x0d, 0x01);
            Assert.AreEqual("vpunpckhqdq\tymm6,ymm8,YMMWORD PTR [r13+r9*1+0xffff01]", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c481420f()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x42, 0x0f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c481430f()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x43, 0x0f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c48148c7()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x48, 0xc7);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c481740c()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x74, 0x0c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c481753f()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x75, 0x3f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4817c24()
        {
            var instr = Disassemble64(0xc4, 0x81, 0x7c, 0x24);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c481e91b()
        {
            var instr = Disassemble64(0xc4, 0x81, 0xe9, 0x1b);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4825601()
        {
            var instr = Disassemble64(0xc4, 0x82, 0x56, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c482cfc9()
        {
            var instr = Disassemble64(0xc4, 0x82, 0xcf, 0xc9);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4834424()
        {
            var instr = Disassemble64(0xc4, 0x83, 0x44, 0x24);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4837b4c()
        {
            var instr = Disassemble64(0xc4, 0x83, 0x7b, 0x4c);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4838104()
        {
            var instr = Disassemble64(0xc4, 0x83, 0x81, 0x04);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483ad40()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xad, 0x40);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483bb48()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xbb, 0x48);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c001()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc0, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c101()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc1, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c301()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc3, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c302()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc3, 0x02);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis_vpermpd_c483c501ebae()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc5, 0x01, 0xeb, 0xae);
            Assert.AreEqual("vpermpd\tymm5,ymm11,0xae", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c601()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc6, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c801()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc8, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483c820()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc8, 0x20);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e001()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e003()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0x03);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e005()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0x05);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e007()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0x07);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e0f3()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0xf3);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e0f8()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0xf8);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e0fb()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe0, 0xfb);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e11f()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe1, 0x1f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e204()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe2, 0x04);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e3f6()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe3, 0xf6);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e621()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe6, 0x21);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483e901()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xe9, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483ee01()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xee, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483f801()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xf8, 0x01);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483f80b()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xf8, 0x0b);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483f86b()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xf8, 0x6b);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483f8a1()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xf8, 0xa1);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483fd08()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xfd, 0x08);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c483fd40()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xfd, 0x40);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4c1c00e()
        {
            var instr = Disassemble64(0xc4, 0xc1, 0xc0, 0x0e);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4c28148()
        {
            var instr = Disassemble64(0xc4, 0xc2, 0x81, 0x48);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4c28181()
        {
            var instr = Disassemble64(0xc4, 0xc2, 0x81, 0x81);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4c38141()
        {
            var instr = Disassemble64(0xc4, 0xc3, 0x81, 0x41);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4e28131()
        {
            var instr = Disassemble64(0xc4, 0xe2, 0x81, 0x31);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }
        [Test]
        public void X86Dis__bad__c4e2ff0f()
        {
            var instr = Disassemble64(0xc4, 0xe2, 0xff, 0x0f);
            Assert.AreEqual("_bad_\t", instr.ToString());
        }

		[Test]
		public void X86Dis__bad__c4e2ff84() {
			var instr = Disassemble64(0xc4, 0xe2, 0xff, 0x84);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpermil2ps_c4e381488b4308() {
			var instr = Disassemble64(0xc4, 0xe3, 0x81, 0x48, 0x8b, 0x43, 0x08);
			Assert.AreEqual("vpermil2ps\txmm1,xmm15,xmm13,XMMWORD PTR [rbx+0x63480843],0x5", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c4e38189() {
			var instr = Disassemble64(0xc4, 0xe3, 0x81, 0x89);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c4e381c6() {
			var instr = Disassemble64(0xc4, 0xe3, 0x81, 0xc6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c4e381e8() {
			var instr = Disassemble64(0xc4, 0xe3, 0x81, 0xe8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50001() {
			var instr = Disassemble64(0xc5, 0x00, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50004() {
			var instr = Disassemble64(0xc5, 0x00, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5001502() {
			var instr = Disassemble64(0xc5, 0x00, 0x15, 0x02);
			Assert.AreEqual("vunpckhps\txmm8,xmm15,XMMWORD PTR [rdx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50024() {
			var instr = Disassemble64(0xc5, 0x00, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50039() {
			var instr = Disassemble64(0xc5, 0x00, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5003c() {
			var instr = Disassemble64(0xc5, 0x00, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50072() {
			var instr = Disassemble64(0xc5, 0x00, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5007d() {
			var instr = Disassemble64(0xc5, 0x00, 0x7d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c500aa() {
			var instr = Disassemble64(0xc5, 0x00, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c500ba() {
			var instr = Disassemble64(0xc5, 0x00, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c500c7() {
			var instr = Disassemble64(0xc5, 0x00, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c500e6() {
			var instr = Disassemble64(0xc5, 0x00, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c500f0() {
			var instr = Disassemble64(0xc5, 0x00, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50101() {
			var instr = Disassemble64(0xc5, 0x01, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5010f() {
			var instr = Disassemble64(0xc5, 0x01, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50125() {
			var instr = Disassemble64(0xc5, 0x01, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50139() {
			var instr = Disassemble64(0xc5, 0x01, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5013b() {
			var instr = Disassemble64(0xc5, 0x01, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5013d() {
			var instr = Disassemble64(0xc5, 0x01, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c501b8() {
			var instr = Disassemble64(0xc5, 0x01, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c501b9() {
			var instr = Disassemble64(0xc5, 0x01, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c501c7() {
			var instr = Disassemble64(0xc5, 0x01, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c501d00f() {
			var instr = Disassemble64(0xc5, 0x01, 0xd0, 0x0f);
			Assert.AreEqual("vaddsubpd\txmm9,xmm15,XMMWORD PTR [rdi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c501f0() {
			var instr = Disassemble64(0xc5, 0x01, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50201() {
			var instr = Disassemble64(0xc5, 0x02, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5020f() {
			var instr = Disassemble64(0xc5, 0x02, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c502c7() {
			var instr = Disassemble64(0xc5, 0x02, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50301() {
			var instr = Disassemble64(0xc5, 0x03, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5030f() {
			var instr = Disassemble64(0xc5, 0x03, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50401() {
			var instr = Disassemble64(0xc5, 0x04, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5040f() {
			var instr = Disassemble64(0xc5, 0x04, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c504ba() {
			var instr = Disassemble64(0xc5, 0x04, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c504f0() {
			var instr = Disassemble64(0xc5, 0x04, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50701() {
			var instr = Disassemble64(0xc5, 0x07, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5070f() {
			var instr = Disassemble64(0xc5, 0x07, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50801() {
			var instr = Disassemble64(0xc5, 0x08, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5080f() {
			var instr = Disassemble64(0xc5, 0x08, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50824() {
			var instr = Disassemble64(0xc5, 0x08, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50838() {
			var instr = Disassemble64(0xc5, 0x08, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50839() {
			var instr = Disassemble64(0xc5, 0x08, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c508aa() {
			var instr = Disassemble64(0xc5, 0x08, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c508b9() {
			var instr = Disassemble64(0xc5, 0x08, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c508c7() {
			var instr = Disassemble64(0xc5, 0x08, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c509d0e8() {
			var instr = Disassemble64(0xc5, 0x09, 0xd0, 0xe8);
			Assert.AreEqual("vaddsubpd\txmm13,xmm14,xmm0", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50a01() {
			var instr = Disassemble64(0xc5, 0x0a, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50b01() {
			var instr = Disassemble64(0xc5, 0x0b, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50c0f() {
			var instr = Disassemble64(0xc5, 0x0c, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50c39() {
			var instr = Disassemble64(0xc5, 0x0c, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50d01() {
			var instr = Disassemble64(0xc5, 0x0d, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50d1e() {
			var instr = Disassemble64(0xc5, 0x0d, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50f01() {
			var instr = Disassemble64(0xc5, 0x0f, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c50ff0() {
			var instr = Disassemble64(0xc5, 0x0f, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51001() {
			var instr = Disassemble64(0xc5, 0x10, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5100f() {
			var instr = Disassemble64(0xc5, 0x10, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5101b() {
			var instr = Disassemble64(0xc5, 0x10, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5101e() {
			var instr = Disassemble64(0xc5, 0x10, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51024() {
			var instr = Disassemble64(0xc5, 0x10, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51027() {
			var instr = Disassemble64(0xc5, 0x10, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51039() {
			var instr = Disassemble64(0xc5, 0x10, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c510c7() {
			var instr = Disassemble64(0xc5, 0x10, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51101() {
			var instr = Disassemble64(0xc5, 0x11, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5120f() {
			var instr = Disassemble64(0xc5, 0x12, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51339() {
			var instr = Disassemble64(0xc5, 0x13, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51404() {
			var instr = Disassemble64(0xc5, 0x14, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51413() {
			var instr = Disassemble64(0xc5, 0x14, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51471() {
			var instr = Disassemble64(0xc5, 0x14, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51527() {
			var instr = Disassemble64(0xc5, 0x15, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c515b8() {
			var instr = Disassemble64(0xc5, 0x15, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51601() {
			var instr = Disassemble64(0xc5, 0x16, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5160f() {
			var instr = Disassemble64(0xc5, 0x16, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c516f0() {
			var instr = Disassemble64(0xc5, 0x16, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5171501() {
			var instr = Disassemble64(0xc5, 0x17, 0x15, 0x01);
			Assert.AreEqual("vunpckhps\tymm8,ymm13,YMMWORD PTR [rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51801() {
			var instr = Disassemble64(0xc5, 0x18, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5180e() {
			var instr = Disassemble64(0xc5, 0x18, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5180f() {
			var instr = Disassemble64(0xc5, 0x18, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5181d() {
			var instr = Disassemble64(0xc5, 0x18, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c518b9() {
			var instr = Disassemble64(0xc5, 0x18, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c518ba() {
			var instr = Disassemble64(0xc5, 0x18, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c518e6() {
			var instr = Disassemble64(0xc5, 0x18, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvtpd2dq_c51be6814889df() {
			var instr = Disassemble64(0xc5, 0x1b, 0xe6, 0x81, 0x48, 0x89, 0xdf);
			Assert.AreEqual("vcvtpd2dq\txmm8,XMMWORD PTR [rcx+0x49df8948]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51c01() {
			var instr = Disassemble64(0xc5, 0x1c, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51c04() {
			var instr = Disassemble64(0xc5, 0x1c, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51c0f() {
			var instr = Disassemble64(0xc5, 0x1c, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c51ce6() {
			var instr = Disassemble64(0xc5, 0x1c, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52013() {
			var instr = Disassemble64(0xc5, 0x20, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52027() {
			var instr = Disassemble64(0xc5, 0x20, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52036() {
			var instr = Disassemble64(0xc5, 0x20, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52039() {
			var instr = Disassemble64(0xc5, 0x20, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52063() {
			var instr = Disassemble64(0xc5, 0x20, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5206d() {
			var instr = Disassemble64(0xc5, 0x20, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52071() {
			var instr = Disassemble64(0xc5, 0x20, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5207b() {
			var instr = Disassemble64(0xc5, 0x20, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c520b8() {
			var instr = Disassemble64(0xc5, 0x20, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52101() {
			var instr = Disassemble64(0xc5, 0x21, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c52239() {
			var instr = Disassemble64(0xc5, 0x22, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c531f0() {
			var instr = Disassemble64(0xc5, 0x31, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53238() {
			var instr = Disassemble64(0xc5, 0x32, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5323d() {
			var instr = Disassemble64(0xc5, 0x32, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53301() {
			var instr = Disassemble64(0xc5, 0x33, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53701() {
			var instr = Disassemble64(0xc5, 0x37, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubps_c537d0ff() {
			var instr = Disassemble64(0xc5, 0x37, 0xd0, 0xff);
			Assert.AreEqual("vaddsubps\tymm15,ymm9,ymm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53824() {
			var instr = Disassemble64(0xc5, 0x38, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c539d07325() {
			var instr = Disassemble64(0xc5, 0x39, 0xd0, 0x73, 0x25);
			Assert.AreEqual("vaddsubpd\txmm14,xmm8,[rbx+0x25]", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvtdq2pd_c53ae6815b5d41() {
			var instr = Disassemble64(0xc5, 0x3a, 0xe6, 0x81, 0x5b, 0x5d, 0x41);
			Assert.AreEqual("vcvtdq2pd\txmm8,QWORD PTR [rcx+0x5c415d5b]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53b1c() {
			var instr = Disassemble64(0xc5, 0x3b, 0x1c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c53f0f() {
			var instr = Disassemble64(0xc5, 0x3f, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54001() {
			var instr = Disassemble64(0xc5, 0x40, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5400e() {
			var instr = Disassemble64(0xc5, 0x40, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5400f() {
			var instr = Disassemble64(0xc5, 0x40, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54013() {
			var instr = Disassemble64(0xc5, 0x40, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5401a() {
			var instr = Disassemble64(0xc5, 0x40, 0x1a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5401d() {
			var instr = Disassemble64(0xc5, 0x40, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54025() {
			var instr = Disassemble64(0xc5, 0x40, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54026() {
			var instr = Disassemble64(0xc5, 0x40, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54027() {
			var instr = Disassemble64(0xc5, 0x40, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54038() {
			var instr = Disassemble64(0xc5, 0x40, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54039() {
			var instr = Disassemble64(0xc5, 0x40, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5403e() {
			var instr = Disassemble64(0xc5, 0x40, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5403f() {
			var instr = Disassemble64(0xc5, 0x40, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54073() {
			var instr = Disassemble64(0xc5, 0x40, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5407b() {
			var instr = Disassemble64(0xc5, 0x40, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c540a6() {
			var instr = Disassemble64(0xc5, 0x40, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c540b3() {
			var instr = Disassemble64(0xc5, 0x40, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c540f0() {
			var instr = Disassemble64(0xc5, 0x40, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54101() {
			var instr = Disassemble64(0xc5, 0x41, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5410f() {
			var instr = Disassemble64(0xc5, 0x41, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54139() {
			var instr = Disassemble64(0xc5, 0x41, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5413b() {
			var instr = Disassemble64(0xc5, 0x41, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c541b8() {
			var instr = Disassemble64(0xc5, 0x41, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c541b9() {
			var instr = Disassemble64(0xc5, 0x41, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c541c7() {
			var instr = Disassemble64(0xc5, 0x41, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c541d0ed() {
			var instr = Disassemble64(0xc5, 0x41, 0xd0, 0xed);
			Assert.AreEqual("vaddsubpd\txmm13,xmm7,xmm5", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54204() {
			var instr = Disassemble64(0xc5, 0x42, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5420f() {
			var instr = Disassemble64(0xc5, 0x42, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54213() {
			var instr = Disassemble64(0xc5, 0x42, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54401() {
			var instr = Disassemble64(0xc5, 0x44, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5440f() {
			var instr = Disassemble64(0xc5, 0x44, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54438() {
			var instr = Disassemble64(0xc5, 0x44, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54439() {
			var instr = Disassemble64(0xc5, 0x44, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54501() {
			var instr = Disassemble64(0xc5, 0x45, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5450f() {
			var instr = Disassemble64(0xc5, 0x45, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54539() {
			var instr = Disassemble64(0xc5, 0x45, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5453b() {
			var instr = Disassemble64(0xc5, 0x45, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54801() {
			var instr = Disassemble64(0xc5, 0x48, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5480f() {
			var instr = Disassemble64(0xc5, 0x48, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54819() {
			var instr = Disassemble64(0xc5, 0x48, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54839() {
			var instr = Disassemble64(0xc5, 0x48, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5483b() {
			var instr = Disassemble64(0xc5, 0x48, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5483d() {
			var instr = Disassemble64(0xc5, 0x48, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54863() {
			var instr = Disassemble64(0xc5, 0x48, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54873() {
			var instr = Disassemble64(0xc5, 0x48, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c548b8() {
			var instr = Disassemble64(0xc5, 0x48, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c548b9() {
			var instr = Disassemble64(0xc5, 0x48, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c548ba() {
			var instr = Disassemble64(0xc5, 0x48, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c548c7() {
			var instr = Disassemble64(0xc5, 0x48, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54901() {
			var instr = Disassemble64(0xc5, 0x49, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5490f() {
			var instr = Disassemble64(0xc5, 0x49, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54939() {
			var instr = Disassemble64(0xc5, 0x49, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5493b() {
			var instr = Disassemble64(0xc5, 0x49, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpacksswb_c54963442404() {
			var instr = Disassemble64(0xc5, 0x49, 0x63, 0x44, 0x24, 0x04);
			Assert.AreEqual("vpacksswb\txmm8,xmm6,XMMWORD PTR [rsp+0x4]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c549c7() {
			var instr = Disassemble64(0xc5, 0x49, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54a01() {
			var instr = Disassemble64(0xc5, 0x4a, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54a1d() {
			var instr = Disassemble64(0xc5, 0x4a, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54a3f() {
			var instr = Disassemble64(0xc5, 0x4a, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54b01() {
			var instr = Disassemble64(0xc5, 0x4b, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54c01() {
			var instr = Disassemble64(0xc5, 0x4c, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54c0f() {
			var instr = Disassemble64(0xc5, 0x4c, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54c39() {
			var instr = Disassemble64(0xc5, 0x4c, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54c3b() {
			var instr = Disassemble64(0xc5, 0x4c, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54c63() {
			var instr = Disassemble64(0xc5, 0x4c, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54d01() {
			var instr = Disassemble64(0xc5, 0x4d, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54d0f() {
			var instr = Disassemble64(0xc5, 0x4d, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54d39() {
			var instr = Disassemble64(0xc5, 0x4d, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpacksswb_c54d63f5() {
			var instr = Disassemble64(0xc5, 0x4d, 0x63, 0xf5);
			Assert.AreEqual("vpacksswb\tymm14,ymm6,ymm5", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54f01() {
			var instr = Disassemble64(0xc5, 0x4f, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c54f25() {
			var instr = Disassemble64(0xc5, 0x4f, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55001() {
			var instr = Disassemble64(0xc5, 0x50, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c5501300() {
			var instr = Disassemble64(0xc5, 0x50, 0x13, 0x00);
			Assert.AreEqual("vmovlps\tQWORD PTR [rax],xmm8", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5503a() {
			var instr = Disassemble64(0xc5, 0x50, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5506d() {
			var instr = Disassemble64(0xc5, 0x50, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvttpd2dq_c551e6ff() {
			var instr = Disassemble64(0xc5, 0x51, 0xe6, 0xff);
			Assert.AreEqual("vcvttpd2dq\txmm15,xmm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55238() {
			var instr = Disassemble64(0xc5, 0x52, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5530f() {
			var instr = Disassemble64(0xc5, 0x53, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55539() {
			var instr = Disassemble64(0xc5, 0x55, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55601() {
			var instr = Disassemble64(0xc5, 0x56, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5576d() {
			var instr = Disassemble64(0xc5, 0x57, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55801() {
			var instr = Disassemble64(0xc5, 0x58, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55819() {
			var instr = Disassemble64(0xc5, 0x58, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55824() {
			var instr = Disassemble64(0xc5, 0x58, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55836() {
			var instr = Disassemble64(0xc5, 0x58, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55a6d() {
			var instr = Disassemble64(0xc5, 0x5a, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55c19() {
			var instr = Disassemble64(0xc5, 0x5c, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55c1d() {
			var instr = Disassemble64(0xc5, 0x5c, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55ef0() {
			var instr = Disassemble64(0xc5, 0x5e, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c55f1b() {
			var instr = Disassemble64(0xc5, 0x5f, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56013() {
			var instr = Disassemble64(0xc5, 0x60, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c56015ca() {
			var instr = Disassemble64(0xc5, 0x60, 0x15, 0xca);
			Assert.AreEqual("vunpckhps\txmm9,xmm3,xmm2", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5601a() {
			var instr = Disassemble64(0xc5, 0x60, 0x1a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56024() {
			var instr = Disassemble64(0xc5, 0x60, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56025() {
			var instr = Disassemble64(0xc5, 0x60, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56038() {
			var instr = Disassemble64(0xc5, 0x60, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56039() {
			var instr = Disassemble64(0xc5, 0x60, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5603b() {
			var instr = Disassemble64(0xc5, 0x60, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5603d() {
			var instr = Disassemble64(0xc5, 0x60, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5606c() {
			var instr = Disassemble64(0xc5, 0x60, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5606d() {
			var instr = Disassemble64(0xc5, 0x60, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5607a() {
			var instr = Disassemble64(0xc5, 0x60, 0x7a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5607b() {
			var instr = Disassemble64(0xc5, 0x60, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5607d() {
			var instr = Disassemble64(0xc5, 0x60, 0x7d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c560aa() {
			var instr = Disassemble64(0xc5, 0x60, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5621a() {
			var instr = Disassemble64(0xc5, 0x62, 0x1a, 0xC0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56326() {
			var instr = Disassemble64(0xc5, 0x63, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56801() {
			var instr = Disassemble64(0xc5, 0x68, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56824() {
			var instr = Disassemble64(0xc5, 0x68, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56c04() {
			var instr = Disassemble64(0xc5, 0x6c, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56e01() {
			var instr = Disassemble64(0xc5, 0x6e, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56e0e() {
			var instr = Disassemble64(0xc5, 0x6e, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c56f1d() {
			var instr = Disassemble64(0xc5, 0x6f, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57001() {
			var instr = Disassemble64(0xc5, 0x70, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5700e() {
			var instr = Disassemble64(0xc5, 0x70, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5700f() {
			var instr = Disassemble64(0xc5, 0x70, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57027() {
			var instr = Disassemble64(0xc5, 0x70, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57204() {
			var instr = Disassemble64(0xc5, 0x72, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5720f() {
			var instr = Disassemble64(0xc5, 0x72, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c572154889() {
			var instr = Disassemble64(0xc5, 0x72, 0x15, 0x48, 0x89);
			Assert.AreEqual("vunpckhps\txmm9,xmm1,XMMWORD PTR [rax-0x77]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5721d() {
			var instr = Disassemble64(0xc5, 0x72, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5721e() {
			var instr = Disassemble64(0xc5, 0x72, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57224() {
			var instr = Disassemble64(0xc5, 0x72, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57225() {
			var instr = Disassemble64(0xc5, 0x72, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57227() {
			var instr = Disassemble64(0xc5, 0x72, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57238() {
			var instr = Disassemble64(0xc5, 0x72, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5723a() {
			var instr = Disassemble64(0xc5, 0x72, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5723b() {
			var instr = Disassemble64(0xc5, 0x72, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c572c7() {
			var instr = Disassemble64(0xc5, 0x72, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c5731342c7() {
			var instr = Disassemble64(0xc5, 0x73, 0x13, 0x42, 0xc7);
			Assert.AreEqual("vmovlps\tQWORD PTR [rdx-0x39],xmm8", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57324() {
			var instr = Disassemble64(0xc5, 0x73, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57327() {
			var instr = Disassemble64(0xc5, 0x73, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57338() {
			var instr = Disassemble64(0xc5, 0x73, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5733b() {
			var instr = Disassemble64(0xc5, 0x73, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5733c() {
			var instr = Disassemble64(0xc5, 0x73, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5733f() {
			var instr = Disassemble64(0xc5, 0x73, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57373() {
			var instr = Disassemble64(0xc5, 0x73, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5737a() {
			var instr = Disassemble64(0xc5, 0x73, 0x7a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c573a7() {
			var instr = Disassemble64(0xc5, 0x73, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5740e() {
			var instr = Disassemble64(0xc5, 0x74, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5740f() {
			var instr = Disassemble64(0xc5, 0x74, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57413() {
			var instr = Disassemble64(0xc5, 0x74, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c574158b500c85() {
			var instr = Disassemble64(0xc5, 0x74, 0x15, 0x8b, 0x50, 0x0c, 0x85);
			Assert.AreEqual("vunpckhps\tymm9,ymm1,YMMWORD PTR [rbx-0x2d7af3b0]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57424() {
			var instr = Disassemble64(0xc5, 0x74, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57425() {
			var instr = Disassemble64(0xc5, 0x74, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57426() {
			var instr = Disassemble64(0xc5, 0x74, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57427() {
			var instr = Disassemble64(0xc5, 0x74, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57436() {
			var instr = Disassemble64(0xc5, 0x74, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57438() {
			var instr = Disassemble64(0xc5, 0x74, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57439() {
			var instr = Disassemble64(0xc5, 0x74, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743a() {
			var instr = Disassemble64(0xc5, 0x74, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743b() {
			var instr = Disassemble64(0xc5, 0x74, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743c() {
			var instr = Disassemble64(0xc5, 0x74, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743d() {
			var instr = Disassemble64(0xc5, 0x74, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743e() {
			var instr = Disassemble64(0xc5, 0x74, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5743f() {
			var instr = Disassemble64(0xc5, 0x74, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57463() {
			var instr = Disassemble64(0xc5, 0x74, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5746c() {
			var instr = Disassemble64(0xc5, 0x74, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5746d() {
			var instr = Disassemble64(0xc5, 0x74, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57471() {
			var instr = Disassemble64(0xc5, 0x74, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57472() {
			var instr = Disassemble64(0xc5, 0x74, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57473() {
			var instr = Disassemble64(0xc5, 0x74, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5747a() {
			var instr = Disassemble64(0xc5, 0x74, 0x7a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5747b() {
			var instr = Disassemble64(0xc5, 0x74, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5747c() {
			var instr = Disassemble64(0xc5, 0x74, 0x7c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5747d() {
			var instr = Disassemble64(0xc5, 0x74, 0x7d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574a7() {
			var instr = Disassemble64(0xc5, 0x74, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574b9() {
			var instr = Disassemble64(0xc5, 0x74, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574ba() {
			var instr = Disassemble64(0xc5, 0x74, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574c7() {
			var instr = Disassemble64(0xc5, 0x74, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574d0() {
			var instr = Disassemble64(0xc5, 0x74, 0xd0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c574e6() {
			var instr = Disassemble64(0xc5, 0x74, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5750f() {
			var instr = Disassemble64(0xc5, 0x75, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57513() {
			var instr = Disassemble64(0xc5, 0x75, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhpd_c575154183() {
			var instr = Disassemble64(0xc5, 0x75, 0x15, 0x41, 0x83);
			Assert.AreEqual("vunpckhpd\tymm8,ymm1,YMMWORD PTR [rcx-0x7d]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57524() {
			var instr = Disassemble64(0xc5, 0x75, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57525() {
			var instr = Disassemble64(0xc5, 0x75, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57526() {
			var instr = Disassemble64(0xc5, 0x75, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57527() {
			var instr = Disassemble64(0xc5, 0x75, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57536() {
			var instr = Disassemble64(0xc5, 0x75, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57538() {
			var instr = Disassemble64(0xc5, 0x75, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57539() {
			var instr = Disassemble64(0xc5, 0x75, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5753b() {
			var instr = Disassemble64(0xc5, 0x75, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5753c() {
			var instr = Disassemble64(0xc5, 0x75, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5753d() {
			var instr = Disassemble64(0xc5, 0x75, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5753e() {
			var instr = Disassemble64(0xc5, 0x75, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5753f() {
			var instr = Disassemble64(0xc5, 0x75, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpcklqdq_c5756c488b() {
			var instr = Disassemble64(0xc5, 0x75, 0x6c, 0x48, 0x8b);
			Assert.AreEqual("vpunpcklqdq\tymm9,ymm1,YMMWORD PTR [rax-0x75]", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpckhqdq_c5756d31() {
			var instr = Disassemble64(0xc5, 0x75, 0x6d, 0x31);
			Assert.AreEqual("vpunpckhqdq\tymm14,ymm1,YMMWORD PTR [rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57571() {
			var instr = Disassemble64(0xc5, 0x75, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57572() {
			var instr = Disassemble64(0xc5, 0x75, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57573() {
			var instr = Disassemble64(0xc5, 0x75, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhaddpd_c5757c488b() {
			var instr = Disassemble64(0xc5, 0x75, 0x7c, 0x48, 0x8b);
			Assert.AreEqual("vhaddpd\tymm9,ymm1,YMMWORD PTR [rax-0x75]", instr.ToString());
		}
		[Test]
		public void X86Dis_vhsubpd_c5757d4c8da3() {
			var instr = Disassemble64(0xc5, 0x75, 0x7d, 0x4c, 0x8d, 0xa3);
			Assert.AreEqual("vhsubpd\tymm9,ymm1,YMMWORD PTR [rbp+rcx*4-0x5d]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575a6() {
			var instr = Disassemble64(0xc5, 0x75, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575a7() {
			var instr = Disassemble64(0xc5, 0x75, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575b3() {
			var instr = Disassemble64(0xc5, 0x75, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575b8() {
			var instr = Disassemble64(0xc5, 0x75, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575b9() {
			var instr = Disassemble64(0xc5, 0x75, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c575d05b44() {
			var instr = Disassemble64(0xc5, 0x75, 0xd0, 0x5b, 0x44);
			Assert.AreEqual("vaddsubpd\tymm11,ymm1,YMMWORD PTR [rbx+0x44]", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvttpd2dq_c575e631() {
			var instr = Disassemble64(0xc5, 0x75, 0xe6, 0x31);
			Assert.AreEqual("vcvttpd2dq\txmm14,YMMWORD PTR [rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c575f0() {
			var instr = Disassemble64(0xc5, 0x75, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c576158b83cc00() {
			var instr = Disassemble64(0xc5, 0x76, 0x15, 0x8b, 0x83, 0xcc, 0x00);
			Assert.AreEqual("vunpckhps\tymm9,ymm1,YMMWORD PTR [rbx+0xcc83]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57638() {
			var instr = Disassemble64(0xc5, 0x76, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5763a() {
			var instr = Disassemble64(0xc5, 0x76, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5763b() {
			var instr = Disassemble64(0xc5, 0x76, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5763e() {
			var instr = Disassemble64(0xc5, 0x76, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5763f() {
			var instr = Disassemble64(0xc5, 0x76, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57673() {
			var instr = Disassemble64(0xc5, 0x76, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5770e() {
			var instr = Disassemble64(0xc5, 0x77, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57724() {
			var instr = Disassemble64(0xc5, 0x77, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57726() {
			var instr = Disassemble64(0xc5, 0x77, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57727() {
			var instr = Disassemble64(0xc5, 0x77, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57738() {
			var instr = Disassemble64(0xc5, 0x77, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57739() {
			var instr = Disassemble64(0xc5, 0x77, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5773b() {
			var instr = Disassemble64(0xc5, 0x77, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5773e() {
			var instr = Disassemble64(0xc5, 0x77, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5773f() {
			var instr = Disassemble64(0xc5, 0x77, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5776c() {
			var instr = Disassemble64(0xc5, 0x77, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5776d() {
			var instr = Disassemble64(0xc5, 0x77, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57773() {
			var instr = Disassemble64(0xc5, 0x77, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5777b() {
			var instr = Disassemble64(0xc5, 0x77, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhaddps_c5777cf7() {
			var instr = Disassemble64(0xc5, 0x77, 0x7c, 0xf7);
			Assert.AreEqual("vhaddps\tymm14,ymm1,ymm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c577a7() {
			var instr = Disassemble64(0xc5, 0x77, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c577c7() {
			var instr = Disassemble64(0xc5, 0x77, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubps_c577d04889() {
			var instr = Disassemble64(0xc5, 0x77, 0xd0, 0x48, 0x89);
			Assert.AreEqual("vaddsubps\tymm9,ymm1,YMMWORD PTR [rax-0x77]", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c578138b839c01() {
			var instr = Disassemble64(0xc5, 0x78, 0x13, 0x8b, 0x83, 0x9c, 0x01);
			Assert.AreEqual("vmovlps\tQWORD PTR [rbx+0x19c83],xmm9", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5781e() {
			var instr = Disassemble64(0xc5, 0x78, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57826() {
			var instr = Disassemble64(0xc5, 0x78, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57836() {
			var instr = Disassemble64(0xc5, 0x78, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57838() {
			var instr = Disassemble64(0xc5, 0x78, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57839() {
			var instr = Disassemble64(0xc5, 0x78, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5783f() {
			var instr = Disassemble64(0xc5, 0x78, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5786c() {
			var instr = Disassemble64(0xc5, 0x78, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5786d() {
			var instr = Disassemble64(0xc5, 0x78, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5787c() {
			var instr = Disassemble64(0xc5, 0x78, 0x7c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c578b3() {
			var instr = Disassemble64(0xc5, 0x78, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c578b8() {
			var instr = Disassemble64(0xc5, 0x78, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c578b9() {
			var instr = Disassemble64(0xc5, 0x78, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57901() {
			var instr = Disassemble64(0xc5, 0x79, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57925() {
			var instr = Disassemble64(0xc5, 0x79, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57926() {
			var instr = Disassemble64(0xc5, 0x79, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5793c() {
			var instr = Disassemble64(0xc5, 0x79, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhsubpd_c5797d00() {
			var instr = Disassemble64(0xc5, 0x79, 0x7d, 0x00);
			Assert.AreEqual("vhsubpd\txmm8,xmm0,XMMWORD PTR [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c579b3() {
			var instr = Disassemble64(0xc5, 0x79, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c579d04889() {
			var instr = Disassemble64(0xc5, 0x79, 0xd0, 0x48, 0x89);
			Assert.AreEqual("vaddsubpd\txmm9,xmm0,XMMWORD PTR [rax-0x77]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57b1b() {
			var instr = Disassemble64(0xc5, 0x7b, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c57c154889() {
			var instr = Disassemble64(0xc5, 0x7c, 0x15, 0x48, 0x89);
			Assert.AreEqual("vunpckhps\tymm9,ymm0,YMMWORD PTR [rax-0x77]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57c3e() {
			var instr = Disassemble64(0xc5, 0x7c, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57d3b() {
			var instr = Disassemble64(0xc5, 0x7d, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57d3d() {
			var instr = Disassemble64(0xc5, 0x7d, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpcklqdq_c57d6c4531() {
			var instr = Disassemble64(0xc5, 0x7d, 0x6c, 0x45, 0x31);
			Assert.AreEqual("vpunpcklqdq\tymm8,ymm0,YMMWORD PTR [rbp+0x31]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57d71() {
			var instr = Disassemble64(0xc5, 0x7d, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57eb8() {
			var instr = Disassemble64(0xc5, 0x7e, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57f0f() {
			var instr = Disassemble64(0xc5, 0x7f, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57f27() {
			var instr = Disassemble64(0xc5, 0x7f, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c57fa7() {
			var instr = Disassemble64(0xc5, 0x7f, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c5801302() {
			var instr = Disassemble64(0xc5, 0x80, 0x13, 0x02);
			Assert.AreEqual("vmovlps\tQWORD PTR [rdx],xmm0", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c58015c4() {
			var instr = Disassemble64(0xc5, 0x80, 0x15, 0xc4);
			Assert.AreEqual("vunpckhps\txmm0,xmm15,xmm4", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58025() {
			var instr = Disassemble64(0xc5, 0x80, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58027() {
			var instr = Disassemble64(0xc5, 0x80, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58036() {
			var instr = Disassemble64(0xc5, 0x80, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58039() {
			var instr = Disassemble64(0xc5, 0x80, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5803d() {
			var instr = Disassemble64(0xc5, 0x80, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5803f() {
			var instr = Disassemble64(0xc5, 0x80, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58063() {
			var instr = Disassemble64(0xc5, 0x80, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5806c() {
			var instr = Disassemble64(0xc5, 0x80, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5806d() {
			var instr = Disassemble64(0xc5, 0x80, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58073() {
			var instr = Disassemble64(0xc5, 0x80, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5807b() {
			var instr = Disassemble64(0xc5, 0x80, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5807c() {
			var instr = Disassemble64(0xc5, 0x80, 0x7c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c580a6() {
			var instr = Disassemble64(0xc5, 0x80, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c580a7() {
			var instr = Disassemble64(0xc5, 0x80, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c580b8() {
			var instr = Disassemble64(0xc5, 0x80, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c580d0() {
			var instr = Disassemble64(0xc5, 0x80, 0xd0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58101() {
			var instr = Disassemble64(0xc5, 0x81, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5833c() {
			var instr = Disassemble64(0xc5, 0x83, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhaddps_c5837c2440() {
			var instr = Disassemble64(0xc5, 0x83, 0x7c, 0x24, 0x40);
			Assert.AreEqual("vhaddps\txmm4,xmm15,XMMWORD PTR [rax+rax*2]", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvtpd2dq_c583e60474() {
			var instr = Disassemble64(0xc5, 0x83, 0xe6, 0x04, 0x74);
			Assert.AreEqual("vcvtpd2dq\txmm0,XMMWORD PTR [rsp+rsi*2]", instr.ToString());
		}
		[Test]
		public void X86Dis_vlddqu_c583f001() {
			var instr = Disassemble64(0xc5, 0x83, 0xf0, 0x01);
			Assert.AreEqual("vlddqu\txmm0,[rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58801() {
			var instr = Disassemble64(0xc5, 0x88, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58804() {
			var instr = Disassemble64(0xc5, 0x88, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5881500() {
			var instr = Disassemble64(0xc5, 0x88, 0x15, 0x00);
			Assert.AreEqual("vunpckhps\txmm0,xmm14,XMMWORD PTR [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58819() {
			var instr = Disassemble64(0xc5, 0x88, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58904() {
			var instr = Disassemble64(0xc5, 0x89, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhsubpd_c5897d00() {
			var instr = Disassemble64(0xc5, 0x89, 0x7d, 0x00);
			Assert.AreEqual("vhsubpd\txmm0,xmm14,XMMWORD PTR [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c589c7() {
			var instr = Disassemble64(0xc5, 0x89, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubpd_c589d099f7f948() {
			var instr = Disassemble64(0xc5, 0x89, 0xd0, 0x99, 0xf7, 0xf9, 0x48);
			Assert.AreEqual("vaddsubpd\txmm3,xmm14,XMMWORD PTR [rcx-0x76b70609]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c589f0() {
			var instr = Disassemble64(0xc5, 0x89, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58a1b() {
			var instr = Disassemble64(0xc5, 0x8a, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58a3d() {
			var instr = Disassemble64(0xc5, 0x8a, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58b0f() {
			var instr = Disassemble64(0xc5, 0x8b, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58b1d() {
			var instr = Disassemble64(0xc5, 0x8b, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58b72() {
			var instr = Disassemble64(0xc5, 0x8b, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58b73() {
			var instr = Disassemble64(0xc5, 0x8b, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58baa() {
			var instr = Disassemble64(0xc5, 0x8b, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58bb3() {
			var instr = Disassemble64(0xc5, 0x8b, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58c01() {
			var instr = Disassemble64(0xc5, 0x8c, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c58c1500() {
			var instr = Disassemble64(0xc5, 0x8c, 0x15, 0x00);
			Assert.AreEqual("vunpckhps\tymm0,ymm14,YMMWORD PTR [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58ce6() {
			var instr = Disassemble64(0xc5, 0x8c, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpcklqdq_c58d6c2dff89e8() {
			var instr = Disassemble64(0xc5, 0x8d, 0x6c, 0x2d, 0xff, 0x89, 0xe8);
			Assert.AreEqual("vpunpcklqdq\tymm5,ymm14,YMMWORD PTR [rip+0x4ce889ff]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58e13() {
			var instr = Disassemble64(0xc5, 0x8e, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c58f01() {
			var instr = Disassemble64(0xc5, 0x8f, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5903f() {
			var instr = Disassemble64(0xc5, 0x90, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c590a6() {
			var instr = Disassemble64(0xc5, 0x90, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c590a7() {
			var instr = Disassemble64(0xc5, 0x90, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c590ae() {
			var instr = Disassemble64(0xc5, 0x90, 0xae);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c590f0() {
			var instr = Disassemble64(0xc5, 0x90, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c59304() {
			var instr = Disassemble64(0xc5, 0x93, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5931e() {
			var instr = Disassemble64(0xc5, 0x93, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c59327() {
			var instr = Disassemble64(0xc5, 0x93, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5980f() {
			var instr = Disassemble64(0xc5, 0x98, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c59824() {
			var instr = Disassemble64(0xc5, 0x98, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c59a39() {
			var instr = Disassemble64(0xc5, 0x9a, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c59ea6() {
			var instr = Disassemble64(0xc5, 0x9e, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvtpd2dq_c59fe6ff() {
			var instr = Disassemble64(0xc5, 0x9f, 0xe6, 0xff);
			Assert.AreEqual("vcvtpd2dq\txmm7,ymm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a001() {
			var instr = Disassemble64(0xc5, 0xa0, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a026() {
			var instr = Disassemble64(0xc5, 0xa0, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a027() {
			var instr = Disassemble64(0xc5, 0xa0, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a03a() {
			var instr = Disassemble64(0xc5, 0xa0, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a063() {
			var instr = Disassemble64(0xc5, 0xa0, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a06c() {
			var instr = Disassemble64(0xc5, 0xa0, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a0f0() {
			var instr = Disassemble64(0xc5, 0xa0, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a4c7() {
			var instr = Disassemble64(0xc5, 0xa4, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a5f0() {
			var instr = Disassemble64(0xc5, 0xa5, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a639() {
			var instr = Disassemble64(0xc5, 0xa6, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a827() {
			var instr = Disassemble64(0xc5, 0xa8, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a83b() {
			var instr = Disassemble64(0xc5, 0xa8, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a86d() {
			var instr = Disassemble64(0xc5, 0xa8, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5a8aa() {
			var instr = Disassemble64(0xc5, 0xa8, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ad04() {
			var instr = Disassemble64(0xc5, 0xad, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b024() {
			var instr = Disassemble64(0xc5, 0xb0, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b03c() {
			var instr = Disassemble64(0xc5, 0xb0, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b063() {
			var instr = Disassemble64(0xc5, 0xb0, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b06c() {
			var instr = Disassemble64(0xc5, 0xb0, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b06d() {
			var instr = Disassemble64(0xc5, 0xb0, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b07b() {
			var instr = Disassemble64(0xc5, 0xb0, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b327() {
			var instr = Disassemble64(0xc5, 0xb3, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b83a() {
			var instr = Disassemble64(0xc5, 0xb8, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5b8f0() {
			var instr = Disassemble64(0xc5, 0xb8, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpckhqdq_c5b96deb() {
			var instr = Disassemble64(0xc5, 0xb9, 0x6d, 0xeb);
			Assert.AreEqual("vpunpckhqdq\txmm5,xmm8,xmm3", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5bb27() {
			var instr = Disassemble64(0xc5, 0xbb, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5be19() {
			var instr = Disassemble64(0xc5, 0xbe, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5beb3() {
			var instr = Disassemble64(0xc5, 0xbe, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c5c0136382() {
			var instr = Disassemble64(0xc5, 0xc0, 0x13, 0x63, 0x82);
			Assert.AreEqual("vmovlps\tQWORD PTR [rbx-0x7e],xmm4", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5c015c0() {
			var instr = Disassemble64(0xc5, 0xc0, 0x15, 0xc0);
			Assert.AreEqual("vunpckhps\txmm0,xmm7,xmm0", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c019() {
			var instr = Disassemble64(0xc5, 0xc0, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c024() {
			var instr = Disassemble64(0xc5, 0xc0, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c026() {
			var instr = Disassemble64(0xc5, 0xc0, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c027() {
			var instr = Disassemble64(0xc5, 0xc0, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c036() {
			var instr = Disassemble64(0xc5, 0xc0, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c039() {
			var instr = Disassemble64(0xc5, 0xc0, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c03a() {
			var instr = Disassemble64(0xc5, 0xc0, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c03b() {
			var instr = Disassemble64(0xc5, 0xc0, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c03c() {
			var instr = Disassemble64(0xc5, 0xc0, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c03d() {
			var instr = Disassemble64(0xc5, 0xc0, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c03e() {
			var instr = Disassemble64(0xc5, 0xc0, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c06d() {
			var instr = Disassemble64(0xc5, 0xc0, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c0a7() {
			var instr = Disassemble64(0xc5, 0xc0, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c0ba() {
			var instr = Disassemble64(0xc5, 0xc0, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c0c7() {
			var instr = Disassemble64(0xc5, 0xc0, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c0d0() {
			var instr = Disassemble64(0xc5, 0xc0, 0xd0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c0f0() {
			var instr = Disassemble64(0xc5, 0xc0, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c824() {
			var instr = Disassemble64(0xc5, 0xc8, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c873() {
			var instr = Disassemble64(0xc5, 0xc8, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5c8a7() {
			var instr = Disassemble64(0xc5, 0xc8, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ceb8() {
			var instr = Disassemble64(0xc5, 0xce, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5cec7() {
			var instr = Disassemble64(0xc5, 0xce, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5d024() {
			var instr = Disassemble64(0xc5, 0xd0, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5d03e() {
			var instr = Disassemble64(0xc5, 0xd0, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5d10e() {
			var instr = Disassemble64(0xc5, 0xd1, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5d20e() {
			var instr = Disassemble64(0xc5, 0xd2, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5d63d() {
			var instr = Disassemble64(0xc5, 0xd6, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5db1c() {
			var instr = Disassemble64(0xc5, 0xdb, 0x1c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5de6d() {
			var instr = Disassemble64(0xc5, 0xde, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5e015c4() {
			var instr = Disassemble64(0xc5, 0xe0, 0x15, 0xc4);
			Assert.AreEqual("vunpckhps\txmm0,xmm3,xmm4", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e01c() {
			var instr = Disassemble64(0xc5, 0xe0, 0x1c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e01d() {
			var instr = Disassemble64(0xc5, 0xe0, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e03a() {
			var instr = Disassemble64(0xc5, 0xe0, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e063() {
			var instr = Disassemble64(0xc5, 0xe0, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e06c() {
			var instr = Disassemble64(0xc5, 0xe0, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e072() {
			var instr = Disassemble64(0xc5, 0xe0, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e073() {
			var instr = Disassemble64(0xc5, 0xe0, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e0f0() {
			var instr = Disassemble64(0xc5, 0xe0, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpacksswb_c5e16300() {
			var instr = Disassemble64(0xc5, 0xe1, 0x63, 0x00);
			Assert.AreEqual("vpacksswb\txmm0,xmm3,XMMWORD PTR [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e172() {
			var instr = Disassemble64(0xc5, 0xe1, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e3a7() {
			var instr = Disassemble64(0xc5, 0xe3, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e801() {
			var instr = Disassemble64(0xc5, 0xe8, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e804() {
			var instr = Disassemble64(0xc5, 0xe8, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e80e() {
			var instr = Disassemble64(0xc5, 0xe8, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e80f() {
			var instr = Disassemble64(0xc5, 0xe8, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vmovlps_c5e813b6ffff4c() {
			var instr = Disassemble64(0xc5, 0xe8, 0x13, 0xb6, 0xff, 0xff, 0x4c);
			Assert.AreEqual("vmovlps\tQWORD PTR [rsi-0x76b30001],xmm6", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5e81517() {
			var instr = Disassemble64(0xc5, 0xe8, 0x15, 0x17);
			Assert.AreEqual("vunpckhps\txmm2,xmm2,XMMWORD PTR [rdi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e819() {
			var instr = Disassemble64(0xc5, 0xe8, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e81a() {
			var instr = Disassemble64(0xc5, 0xe8, 0x1a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e81b() {
			var instr = Disassemble64(0xc5, 0xe8, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e81d() {
			var instr = Disassemble64(0xc5, 0xe8, 0x1d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e81e() {
			var instr = Disassemble64(0xc5, 0xe8, 0x1e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e824() {
			var instr = Disassemble64(0xc5, 0xe8, 0x24);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e826() {
			var instr = Disassemble64(0xc5, 0xe8, 0x26);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e827() {
			var instr = Disassemble64(0xc5, 0xe8, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e836() {
			var instr = Disassemble64(0xc5, 0xe8, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e838() {
			var instr = Disassemble64(0xc5, 0xe8, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e839() {
			var instr = Disassemble64(0xc5, 0xe8, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83a() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83b() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83c() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83d() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83e() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e83f() {
			var instr = Disassemble64(0xc5, 0xe8, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e863() {
			var instr = Disassemble64(0xc5, 0xe8, 0x63);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e86c() {
			var instr = Disassemble64(0xc5, 0xe8, 0x6c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e86d() {
			var instr = Disassemble64(0xc5, 0xe8, 0x6d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e871() {
			var instr = Disassemble64(0xc5, 0xe8, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e872() {
			var instr = Disassemble64(0xc5, 0xe8, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e873() {
			var instr = Disassemble64(0xc5, 0xe8, 0x73);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e87a() {
			var instr = Disassemble64(0xc5, 0xe8, 0x7a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e87b() {
			var instr = Disassemble64(0xc5, 0xe8, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e87c() {
			var instr = Disassemble64(0xc5, 0xe8, 0x7c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e87d() {
			var instr = Disassemble64(0xc5, 0xe8, 0x7d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8a6() {
			var instr = Disassemble64(0xc5, 0xe8, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8a7() {
			var instr = Disassemble64(0xc5, 0xe8, 0xa7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8aa() {
			var instr = Disassemble64(0xc5, 0xe8, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8b3() {
			var instr = Disassemble64(0xc5, 0xe8, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8b8() {
			var instr = Disassemble64(0xc5, 0xe8, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8b9() {
			var instr = Disassemble64(0xc5, 0xe8, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8ba() {
			var instr = Disassemble64(0xc5, 0xe8, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8c7() {
			var instr = Disassemble64(0xc5, 0xe8, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8d0() {
			var instr = Disassemble64(0xc5, 0xe8, 0xd0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8e6() {
			var instr = Disassemble64(0xc5, 0xe8, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e8f0() {
			var instr = Disassemble64(0xc5, 0xe8, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e904() {
			var instr = Disassemble64(0xc5, 0xe9, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e90e() {
			var instr = Disassemble64(0xc5, 0xe9, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e90f() {
			var instr = Disassemble64(0xc5, 0xe9, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhpd_c5e915ff() {
			var instr = Disassemble64(0xc5, 0xe9, 0x15, 0xff);
			Assert.AreEqual("vunpckhpd\txmm7,xmm2,xmm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e918() {
			var instr = Disassemble64(0xc5, 0xe9, 0x18);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e919() {
			var instr = Disassemble64(0xc5, 0xe9, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e91b() {
			var instr = Disassemble64(0xc5, 0xe9, 0x1b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e91c() {
			var instr = Disassemble64(0xc5, 0xe9, 0x1c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e925() {
			var instr = Disassemble64(0xc5, 0xe9, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e927() {
			var instr = Disassemble64(0xc5, 0xe9, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e936() {
			var instr = Disassemble64(0xc5, 0xe9, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e938() {
			var instr = Disassemble64(0xc5, 0xe9, 0x38);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e939() {
			var instr = Disassemble64(0xc5, 0xe9, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e93b() {
			var instr = Disassemble64(0xc5, 0xe9, 0x3b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e93c() {
			var instr = Disassemble64(0xc5, 0xe9, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e93e() {
			var instr = Disassemble64(0xc5, 0xe9, 0x3e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e93f() {
			var instr = Disassemble64(0xc5, 0xe9, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpacksswb_c5e963ff() {
			var instr = Disassemble64(0xc5, 0xe9, 0x63, 0xff);
			Assert.AreEqual("vpacksswb\txmm7,xmm2,xmm7", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpcklqdq_c5e96cfb() {
			var instr = Disassemble64(0xc5, 0xe9, 0x6c, 0xfb);
			Assert.AreEqual("vpunpcklqdq\txmm7,xmm2,xmm3", instr.ToString());
		}
		[Test]
		public void X86Dis_vpunpckhqdq_c5e96dfe() {
			var instr = Disassemble64(0xc5, 0xe9, 0x6d, 0xfe);
			Assert.AreEqual("vpunpckhqdq\txmm7,xmm2,xmm6", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e971() {
			var instr = Disassemble64(0xc5, 0xe9, 0x71);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vpslld_c5e972f2ff() {
			var instr = Disassemble64(0xc5, 0xe9, 0x72, 0xf2, 0xff);
			Assert.AreEqual("vpslld\txmm2,xmm2,0xff", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e97a() {
			var instr = Disassemble64(0xc5, 0xe9, 0x7a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e97b() {
			var instr = Disassemble64(0xc5, 0xe9, 0x7b);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vhaddpd_c5e97cfd() {
			var instr = Disassemble64(0xc5, 0xe9, 0x7c, 0xfd);
			Assert.AreEqual("vhaddpd\txmm7,xmm2,xmm5", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9a6() {
			var instr = Disassemble64(0xc5, 0xe9, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9aa() {
			var instr = Disassemble64(0xc5, 0xe9, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9b3() {
			var instr = Disassemble64(0xc5, 0xe9, 0xb3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9b8() {
			var instr = Disassemble64(0xc5, 0xe9, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9b9() {
			var instr = Disassemble64(0xc5, 0xe9, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9ba() {
			var instr = Disassemble64(0xc5, 0xe9, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5e9c7() {
			var instr = Disassemble64(0xc5, 0xe9, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}

		[Test]
		public void X86Dis__bad__c5e9f0() {
			var instr = Disassemble64(0xc5, 0xe9, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ead0() {
			var instr = Disassemble64(0xc5, 0xea, 0xd0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vunpckhps_c5eb154885() {
			var instr = Disassemble64(0xc5, 0xeb, 0x15, 0x48, 0x85);
			Assert.AreEqual("vunpckhps\txmm1,xmm2,XMMWORD PTR [rax-0x7b]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb19() {
			var instr = Disassemble64(0xc5, 0xeb, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb1a() {
			var instr = Disassemble64(0xc5, 0xeb, 0x1a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb1c() {
			var instr = Disassemble64(0xc5, 0xeb, 0x1c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb25() {
			var instr = Disassemble64(0xc5, 0xeb, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb36() {
			var instr = Disassemble64(0xc5, 0xeb, 0x36);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb3a() {
			var instr = Disassemble64(0xc5, 0xeb, 0x3a);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb3c() {
			var instr = Disassemble64(0xc5, 0xeb, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb3d() {
			var instr = Disassemble64(0xc5, 0xeb, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5eb72() {
			var instr = Disassemble64(0xc5, 0xeb, 0x72);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ebb8() {
			var instr = Disassemble64(0xc5, 0xeb, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ebb9() {
			var instr = Disassemble64(0xc5, 0xeb, 0xb9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ebba() {
			var instr = Disassemble64(0xc5, 0xeb, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vcvtpd2dq_c5ebe6803d7f06() {
			var instr = Disassemble64(0xc5, 0xeb, 0xe6, 0x80, 0x3d, 0x7f, 0x06);
			Assert.AreEqual("vcvtpd2dq\txmm0,XMMWORD PTR [rax-0x58f980c3]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ec0f() {
			var instr = Disassemble64(0xc5, 0xec, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ef0f() {
			var instr = Disassemble64(0xc5, 0xef, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ef25() {
			var instr = Disassemble64(0xc5, 0xef, 0x25);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ef7e() {
			var instr = Disassemble64(0xc5, 0xef, 0x7e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubps_c5efd0ff() {
			var instr = Disassemble64(0xc5, 0xef, 0xd0, 0xff);
			Assert.AreEqual("vaddsubps\tymm7,ymm2,ymm7", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f001() {
			var instr = Disassemble64(0xc5, 0xf0, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f00e() {
			var instr = Disassemble64(0xc5, 0xf0, 0x0e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f03c() {
			var instr = Disassemble64(0xc5, 0xf0, 0x3c);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f03d() {
			var instr = Disassemble64(0xc5, 0xf0, 0x3d);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f513() {
			var instr = Disassemble64(0xc5, 0xf5, 0x13);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f6ba() {
			var instr = Disassemble64(0xc5, 0xf6, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f701() {
			var instr = Disassemble64(0xc5, 0xf7, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f70f() {
			var instr = Disassemble64(0xc5, 0xf7, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_vaddsubps_c5f7d06621() {
			var instr = Disassemble64(0xc5, 0xf7, 0xd0, 0x66, 0x21);
			Assert.AreEqual("vaddsubps\tymm4,ymm1,YMMWORD PTR [rsi+0x21]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f801() {
			var instr = Disassemble64(0xc5, 0xf8, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f804() {
			var instr = Disassemble64(0xc5, 0xf8, 0x04);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f819() {
			var instr = Disassemble64(0xc5, 0xf8, 0x19);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f839() {
			var instr = Disassemble64(0xc5, 0xf8, 0x39);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f8a6() {
			var instr = Disassemble64(0xc5, 0xf8, 0xa6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f8aa() {
			var instr = Disassemble64(0xc5, 0xf8, 0xaa);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5f9f0() {
			var instr = Disassemble64(0xc5, 0xf9, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5fa27() {
			var instr = Disassemble64(0xc5, 0xfa, 0x27);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5fb7e() {
			var instr = Disassemble64(0xc5, 0xfb, 0x7e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5fd01() {
			var instr = Disassemble64(0xc5, 0xfd, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5fe01() {
			var instr = Disassemble64(0xc5, 0xfe, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ff01() {
			var instr = Disassemble64(0xc5, 0xff, 0x01);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ff0f() {
			var instr = Disassemble64(0xc5, 0xff, 0x0f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ff3f() {
			var instr = Disassemble64(0xc5, 0xff, 0x3f);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ff7e() {
			var instr = Disassemble64(0xc5, 0xff, 0x7e);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ffb8() {
			var instr = Disassemble64(0xc5, 0xff, 0xb8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ffba() {
			var instr = Disassemble64(0xc5, 0xff, 0xba);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__c5ffc7() {
			var instr = Disassemble64(0xc5, 0xff, 0xc7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db22() {
			var instr = Disassemble64(0xdb, 0x22);
			Assert.AreEqual("_bad_\t[rdx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db23() {
			var instr = Disassemble64(0xdb, 0x23);
			Assert.AreEqual("_bad_\t[rbx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db247500483977() {
			var instr = Disassemble64(0xdb, 0x24, 0x75, 0x00, 0x48, 0x39, 0x77);
			Assert.AreEqual("_bad_\t[rsi*2+0x77394800]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db257700488b() {
			var instr = Disassemble64(0xdb, 0x25, 0x77, 0x00, 0x48, 0x8b);
			Assert.AreEqual("_bad_\t[rip+0xffffffff8b480077]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db26() {
			var instr = Disassemble64(0xdb, 0x26);
			Assert.AreEqual("_bad_\t[rsi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db27() {
			var instr = Disassemble64(0xdb, 0x27);
			Assert.AreEqual("_bad_\t[rdi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db30() {
			var instr = Disassemble64(0xdb, 0x30);
			Assert.AreEqual("_bad_\t[rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db31() {
			var instr = Disassemble64(0xdb, 0x31);
			Assert.AreEqual("_bad_\t[rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db32() {
			var instr = Disassemble64(0xdb, 0x32);
			Assert.AreEqual("_bad_\t[rdx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db33() {
			var instr = Disassemble64(0xdb, 0x33);
			Assert.AreEqual("_bad_\t[rbx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db34b6() {
			var instr = Disassemble64(0xdb, 0x34, 0xb6);
			Assert.AreEqual("_bad_\t[rsi+rsi*4]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db3579004154() {
			var instr = Disassemble64(0xdb, 0x35, 0x79, 0x00, 0x41, 0x54);
			Assert.AreEqual("_bad_\t[rip+0x54410079]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db36() {
			var instr = Disassemble64(0xdb, 0x36);
			Assert.AreEqual("_bad_\t[rsi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db37() {
			var instr = Disassemble64(0xdb, 0x37);
			Assert.AreEqual("_bad_\t[rdi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db607e() {
			var instr = Disassemble64(0xdb, 0x60, 0x7e);
			Assert.AreEqual("_bad_\t[rax+0x7e]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db6100() {
			var instr = Disassemble64(0xdb, 0x61, 0x00);
			Assert.AreEqual("_bad_\t[rcx+0x0]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db6210() {
			var instr = Disassemble64(0xdb, 0x62, 0x10);
			Assert.AreEqual("_bad_\t[rdx+0x10]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db637d() {
			var instr = Disassemble64(0xdb, 0x63, 0x7d);
			Assert.AreEqual("_bad_\t[rbx+0x7d]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db647d00() {
			var instr = Disassemble64(0xdb, 0x64, 0x7d, 0x00);
			Assert.AreEqual("_bad_\t[rbp+rdi*2+0x0]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db6548() {
			var instr = Disassemble64(0xdb, 0x65, 0x48);
			Assert.AreEqual("_bad_\t[rbp+0x48]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db667f() {
			var instr = Disassemble64(0xdb, 0x66, 0x7f);
			Assert.AreEqual("_bad_\t[rsi+0x7f]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db677e() {
			var instr = Disassemble64(0xdb, 0x67, 0x7e);
			Assert.AreEqual("_bad_\t[rdi+0x7e]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db706c() {
			var instr = Disassemble64(0xdb, 0x70, 0x6c);
			Assert.AreEqual("_bad_\t[rax+0x6c]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db717a() {
			var instr = Disassemble64(0xdb, 0x71, 0x7a);
			Assert.AreEqual("_bad_\t[rcx+0x7a]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db7265() {
			var instr = Disassemble64(0xdb, 0x72, 0x65);
			Assert.AreEqual("_bad_\t[rdx+0x65]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db737e() {
			var instr = Disassemble64(0xdb, 0x73, 0x7e);
			Assert.AreEqual("_bad_\t[rbx+0x7e]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db741c48() {
			var instr = Disassemble64(0xdb, 0x74, 0x1c, 0x48);
			Assert.AreEqual("_bad_\t[rsp+rbx*1+0x48]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db7580() {
			var instr = Disassemble64(0xdb, 0x75, 0x80);
			Assert.AreEqual("_bad_\t[rbp-0x80]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db767c() {
			var instr = Disassemble64(0xdb, 0x76, 0x7c);
			Assert.AreEqual("_bad_\t[rsi+0x7c]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__db7778() {
			var instr = Disassemble64(0xdb, 0x77, 0x78);
			Assert.AreEqual("_bad_\t[rdi+0x78]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba00d006666() {
			var instr = Disassemble64(0xdb, 0xa0, 0x0d, 0x00, 0x66, 0x66);
			Assert.AreEqual("_bad_\t[rax+0x6666000d]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba178004154() {
			var instr = Disassemble64(0xdb, 0xa1, 0x78, 0x00, 0x41, 0x54);
			Assert.AreEqual("_bad_\t[rcx+0x54410078]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba263004889() {
			var instr = Disassemble64(0xdb, 0xa2, 0x63, 0x00, 0x48, 0x89);
			Assert.AreEqual("_bad_\t[rdx-0x76b7ff9d]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba33d014585() {
			var instr = Disassemble64(0xdb, 0xa3, 0x3d, 0x01, 0x45, 0x85);
			Assert.AreEqual("_bad_\t[rbx-0x7abafec3]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba47700555389() {
			var instr = Disassemble64(0xdb, 0xa4, 0x77, 0x00, 0x55, 0x53, 0x89);
			Assert.AreEqual("_bad_\t[rdi+rsi*2-0x76acab00]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba57900482b() {
			var instr = Disassemble64(0xdb, 0xa5, 0x79, 0x00, 0x48, 0x2b);
			Assert.AreEqual("_bad_\t[rbp+0x2b480079]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dba67900ba01() {
			var instr = Disassemble64(0xdb, 0xa6, 0x79, 0x00, 0xba, 0x01);
			Assert.AreEqual("_bad_\t[rsi+0x1ba0079]", instr.ToString());
		}
		[Test]
		public void X86Dis_fneni_8087_dbe0() {
			var instr = Disassemble64(0xdb, 0xe0);
			Assert.AreEqual("fneni_8087\tonly)", instr.ToString());
		}
		[Test]
		public void X86Dis_fndisi_8087_dbe1() {
			var instr = Disassemble64(0xdb, 0xe1);
			Assert.AreEqual("fndisi_8087\tonly)", instr.ToString());
		}
		[Test]
		public void X86Dis_fnsetpm_287_dbe4() {
			var instr = Disassemble64(0xdb, 0xe4);
			Assert.AreEqual("fnsetpm_287\tonly)", instr.ToString());
		}
		[Test]
		public void X86Dis_frstpm_287_dbe5() {
			var instr = Disassemble64(0xdb, 0xe5);
			Assert.AreEqual("frstpm_287\tonly)", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dbe6() {
			var instr = Disassemble64(0xdb, 0xe6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dbe7() {
			var instr = Disassemble64(0xdb, 0xe7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}

		[Test]
		public void X86Dis__bad__dd28() {
			var instr = Disassemble64(0xdd, 0x28);
			Assert.AreEqual("_bad_\t[rax]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd29() {
			var instr = Disassemble64(0xdd, 0x29);
			Assert.AreEqual("_bad_\t[rcx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2a() {
			var instr = Disassemble64(0xdd, 0x2a);
			Assert.AreEqual("_bad_\t[rdx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2b() {
			var instr = Disassemble64(0xdd, 0x2b);
			Assert.AreEqual("_bad_\t[rbx]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2c00() {
			var instr = Disassemble64(0xdd, 0x2c, 0x00);
			Assert.AreEqual("_bad_\t[rax+rax*1]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2dfaff85c0() {
			var instr = Disassemble64(0xdd, 0x2d, 0xfa, 0xff, 0x85, 0xc0);
			Assert.AreEqual("_bad_\t[rip+0xffffffffc085fffa]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2e() {
			var instr = Disassemble64(0xdd, 0x2e);
			Assert.AreEqual("_bad_\t[rsi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd2f() {
			var instr = Disassemble64(0xdd, 0x2f);
			Assert.AreEqual("_bad_\t[rdi]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd688c() {
			var instr = Disassemble64(0xdd, 0x68, 0x8c);
			Assert.AreEqual("_bad_\t[rax-0x74]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6903() {
			var instr = Disassemble64(0xdd, 0x69, 0x03);
			Assert.AreEqual("_bad_\t[rcx+0x3]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6a00() {
			var instr = Disassemble64(0xdd, 0x6a, 0x00);
			Assert.AreEqual("_bad_\t[rdx+0x0]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6b0e() {
			var instr = Disassemble64(0xdd, 0x6b, 0x0e);
			Assert.AreEqual("_bad_\t[rbx+0xe]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6c0041() {
			var instr = Disassemble64(0xdd, 0x6c, 0x00, 0x41);
			Assert.AreEqual("_bad_\t[rax+rax*1+0x41]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6d00() {
			var instr = Disassemble64(0xdd, 0x6d, 0x00);
			Assert.AreEqual("_bad_\t[rbp+0x0]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6efe() {
			var instr = Disassemble64(0xdd, 0x6e, 0xfe);
			Assert.AreEqual("_bad_\t[rsi-0x2]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dd6fff() {
			var instr = Disassemble64(0xdd, 0x6f, 0xff);
			Assert.AreEqual("_bad_\t[rdi-0x1]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dda8027557a8() {
			var instr = Disassemble64(0xdd, 0xa8, 0x02, 0x75, 0x57, 0xa8);
			Assert.AreEqual("_bad_\t[rax-0x57a88afe]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dda9ffff4885() {
			var instr = Disassemble64(0xdd, 0xa9, 0xff, 0xff, 0x48, 0x85);
			Assert.AreEqual("_bad_\t[rcx-0x7ab70001]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddaae9ff84c0() {
			var instr = Disassemble64(0xdd, 0xaa, 0xe9, 0xff, 0x84, 0xc0);
			Assert.AreEqual("_bad_\t[rdx-0x3f7b0017]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddab28004889() {
			var instr = Disassemble64(0xdd, 0xab, 0x28, 0x00, 0x48, 0x89);
			Assert.AreEqual("_bad_\t[rbx-0x76b7ffd8]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddac6200b90100() {
			var instr = Disassemble64(0xdd, 0xac, 0x62, 0x00, 0xb9, 0x01, 0x00);
			Assert.AreEqual("_bad_\t[rdx+riz*2+0x1b900]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddad56004883() {
			var instr = Disassemble64(0xdd, 0xad, 0x56, 0x00, 0x48, 0x83);
			Assert.AreEqual("_bad_\t[rbp-0x7cb7ffaa]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddaed2ffebe7() {
			var instr = Disassemble64(0xdd, 0xae, 0xd2, 0xff, 0xeb, 0xe7);
			Assert.AreEqual("_bad_\t[rsi-0x1814002e]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddaffdff4885() {
			var instr = Disassemble64(0xdd, 0xaf, 0xfd, 0xff, 0x48, 0x85);
			Assert.AreEqual("_bad_\t[rdi-0x7ab70003]", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddc8() {
			var instr = Disassemble64(0xdd, 0xc8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddc9() {
			var instr = Disassemble64(0xdd, 0xc9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddca() {
			var instr = Disassemble64(0xdd, 0xca);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddcb() {
			var instr = Disassemble64(0xdd, 0xcb);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddcc() {
			var instr = Disassemble64(0xdd, 0xcc);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddcd() {
			var instr = Disassemble64(0xdd, 0xcd);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddce() {
			var instr = Disassemble64(0xdd, 0xce);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddcf() {
			var instr = Disassemble64(0xdd, 0xcf);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf0() {
			var instr = Disassemble64(0xdd, 0xf0);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf1() {
			var instr = Disassemble64(0xdd, 0xf1);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf2() {
			var instr = Disassemble64(0xdd, 0xf2);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf3() {
			var instr = Disassemble64(0xdd, 0xf3);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf4() {
			var instr = Disassemble64(0xdd, 0xf4);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf5() {
			var instr = Disassemble64(0xdd, 0xf5);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf6() {
			var instr = Disassemble64(0xdd, 0xf6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__ddf7() {
			var instr = Disassemble64(0xdd, 0xf7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc0() {
			var instr = Disassemble64(0xdf, 0xc0);
			Assert.AreEqual("ffreep\tst(0)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc1() {
			var instr = Disassemble64(0xdf, 0xc1);
			Assert.AreEqual("ffreep\tst(1)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc2() {
			var instr = Disassemble64(0xdf, 0xc2);
			Assert.AreEqual("ffreep\tst(2)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc3() {
			var instr = Disassemble64(0xdf, 0xc3);
			Assert.AreEqual("ffreep\tst(3)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc4() {
			var instr = Disassemble64(0xdf, 0xc4);
			Assert.AreEqual("ffreep\tst(4)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc5() {
			var instr = Disassemble64(0xdf, 0xc5);
			Assert.AreEqual("ffreep\tst(5)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc6() {
			var instr = Disassemble64(0xdf, 0xc6);
			Assert.AreEqual("ffreep\tst(6)", instr.ToString());
		}
		[Test]
		public void X86Dis_ffreep_dfc7() {
			var instr = Disassemble64(0xdf, 0xc7);
			Assert.AreEqual("ffreep\tst(7)", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfc8() {
			var instr = Disassemble64(0xdf, 0xc8);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfc9() {
			var instr = Disassemble64(0xdf, 0xc9);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfca() {
			var instr = Disassemble64(0xdf, 0xca);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfcb() {
			var instr = Disassemble64(0xdf, 0xcb);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfcc() {
			var instr = Disassemble64(0xdf, 0xcc);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfcd() {
			var instr = Disassemble64(0xdf, 0xcd);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfce() {
			var instr = Disassemble64(0xdf, 0xce);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfcf() {
			var instr = Disassemble64(0xdf, 0xcf);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfd4() {
			var instr = Disassemble64(0xdf, 0xd4);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfd5() {
			var instr = Disassemble64(0xdf, 0xd5);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfd6() {
			var instr = Disassemble64(0xdf, 0xd6);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfd7() {
			var instr = Disassemble64(0xdf, 0xd7);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfda() {
			var instr = Disassemble64(0xdf, 0xda);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis__bad__dfdb() {
			var instr = Disassemble64(0xdf, 0xdb);
			Assert.AreEqual("_bad_\t", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f20f0085c075dc() {
			var instr = Disassemble64(0xf2, 0x0f, 0x00, 0x85, 0xc0, 0x75, 0xdc);
			Assert.AreEqual("repnz\tsldt WORD PTR [rbp-0x7c238a40]", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f20f13() {
			var instr = Disassemble64(0xf2, 0x0f, 0x13);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f22eff() {
			var instr = Disassemble64(0xf2, 0x2e, 0xff);
			Assert.AreEqual("repnz\tcs (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f241ff() {
			var instr = Disassemble64(0xf2, 0x41, 0xff);
			Assert.AreEqual("repnz\trex.B (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2c50800() {
			var instr = Disassemble64(0xf2, 0xc5, 0x08, 0x00);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2c51200() {
			var instr = Disassemble64(0xf2, 0xc5, 0x12, 0x00);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2c5f600() {
			var instr = Disassemble64(0xf2, 0xc5, 0xf6, 0x00);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2c5ffc7() {
			var instr = Disassemble64(0xf2, 0xc5, 0xff, 0xc7);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2dae1() {
			var instr = Disassemble64(0xf2, 0xda, 0xe1);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2daed() {
			var instr = Disassemble64(0xf2, 0xda, 0xed);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2daff() {
			var instr = Disassemble64(0xf2, 0xda, 0xff);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2dbfe() {
			var instr = Disassemble64(0xf2, 0xdb, 0xfe);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repnz_f2ded2() {
			var instr = Disassemble64(0xf2, 0xde, 0xd2);
			Assert.AreEqual("repnz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f30f0048c7() {
			var instr = Disassemble64(0xf3, 0x0f, 0x00, 0x48, 0xc7);
			Assert.AreEqual("repz\tstr WORD PTR [rax-0x39]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f30f0100() {
			var instr = Disassemble64(0xf3, 0x0f, 0x01, 0x00);
			Assert.AreEqual("repz\tsgdt [rax]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f30f0173b5() {
			var instr = Disassemble64(0xf3, 0x0f, 0x01, 0x73, 0xb5);
			Assert.AreEqual("repz\tlmsw WORD PTR [rbx-0x4b]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f340() {
			var instr = Disassemble64(0xf3, 0x40);
			Assert.AreEqual("repz\trex", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3400f004881() {
			var instr = Disassemble64(0xf3, 0x40, 0x0f, 0x00, 0x48, 0x81);
			Assert.AreEqual("repz\trex str WORD PTR [rax-0x7f]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f342ff() {
			var instr = Disassemble64(0xf3, 0x42, 0xff);
			Assert.AreEqual("repz\trex.X (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3650f0185c00f() {
			var instr = Disassemble64(0xf3, 0x65, 0x0f, 0x01, 0x85, 0xc0, 0x0f);
			Assert.AreEqual("repz\tsgdt gs:[rbp-0x1e7bf040]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f365fe() {
			var instr = Disassemble64(0xf3, 0x65, 0xfe);
			Assert.AreEqual("repz\tgs (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3c4030083() {
			var instr = Disassemble64(0xf3, 0xc4, 0x03, 0x00, 0x83);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3daff() {
			var instr = Disassemble64(0xf3, 0xda, 0xff);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3dbb6ff4885c0() {
			var instr = Disassemble64(0xf3, 0xdb, 0xb6, 0xff, 0x48, 0x85, 0xc0);
			Assert.AreEqual("repz\t(bad) [rsi-0x3f7ab701]", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3dbff() {
			var instr = Disassemble64(0xf3, 0xdb, 0xff);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3ddff() {
			var instr = Disassemble64(0xf3, 0xdd, 0xff);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3dfff() {
			var instr = Disassemble64(0xf3, 0xdf, 0xff);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3f2ff() {
			var instr = Disassemble64(0xf3, 0xf2, 0xff);
			Assert.AreEqual("repz\trepnz (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3f3ff() {
			var instr = Disassemble64(0xf3, 0xf3, 0xff);
			Assert.AreEqual("repz\trepz (bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3fe() {
			var instr = Disassemble64(0xf3, 0xfe);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
		[Test]
		public void X86Dis_repz_f3ff() {
			var instr = Disassemble64(0xf3, 0xff);
			Assert.AreEqual("repz\t(bad)", instr.ToString());
		}
#endif
    }
}