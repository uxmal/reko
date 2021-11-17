#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    public class X86DisassemblerTests
    {
        private ServiceContainer sc;
        private X86Disassembler dasm;
        private Dictionary<string,object> options;

        public X86DisassemblerTests()
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        private X86Instruction Disassemble16(params byte[] bytes)
        {
            ByteMemoryArea mem = new ByteMemoryArea(Address.SegPtr(0xC00, 0), bytes);
            EndianImageReader rdr = mem.CreateLeReader(mem.BaseAddress);
            var decoders = ProcessorMode.Real.CreateRootDecoders(options);
            var dasm = ProcessorMode.Real.CreateDisassembler(sc, decoders, rdr, options);
            if (options.ContainsKey("Emulate8087"))
            {
                dasm.Emulate8087 = true;
            }
            return dasm.First();
        }

        private X86Instruction Disassemble32(params byte[] bytes)
        {
            var img = new ByteMemoryArea(Address.Ptr32(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var decoders = ProcessorMode.Protected32.CreateRootDecoders(options);
            var dasm = new X86Disassembler(sc, decoders, ProcessorMode.Protected32, rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            return dasm.First();
        }

        private X86Instruction Disassemble64(params byte[] bytes)
        {
            var img = new ByteMemoryArea(Address.Ptr64(0x10000), bytes);
            var rdr = img.CreateLeReader(img.BaseAddress);
            var decoders = ProcessorMode.Protected64.CreateRootDecoders(options);
            var dasm = new X86Disassembler(
                sc,
                decoders,
                ProcessorMode.Protected64,
                rdr,
                PrimitiveType.Word32,
                PrimitiveType.Word64,
                true);
            return dasm.First();
        }

        private void CreateDisassembler16(params byte[] bytes)
        {
            var mem = new ByteMemoryArea(Address.SegPtr(0x0C00, 0), bytes);
            CreateDisassembler16(mem);
        }

        private void CreateDisassembler16(MemoryArea mem)
        {
            var decoders = ProcessorMode.Real.CreateRootDecoders(options);
            dasm = new X86Disassembler(
                sc,
                decoders,
                ProcessorMode.Real,
                mem.CreateLeReader(mem.BaseAddress),
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                false);
            if (options!= null && options.ContainsKey("Emulate8087"))
            {
                dasm.Emulate8087 = true;
            }
        }

        private void CreateDisassembler32(MemoryArea mem)
        {
            var decoders = ProcessorMode.Protected32.CreateRootDecoders(options);
            dasm = new X86Disassembler(
                sc,
                decoders,
                ProcessorMode.Protected32,
                mem.CreateLeReader(mem.BaseAddress),
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
        }

        private void CreateDisassembler16(EndianImageReader rdr)
        {
            var decoders = ProcessorMode.Real.CreateRootDecoders(options);
            dasm = new X86Disassembler(
                sc,
                decoders,
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

        private void AssertCode16(string sExp, string hexBytes)
        {
            var instr = Disassemble16(BytePattern.FromHexBytes(hexBytes));
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void AssertCode32(string sExp, params byte[] bytes)
        {
            var instr = Disassemble32(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void AssertCode32(string sExp, string hexBytes)
        {
            var instr = Disassemble32(BytePattern.FromHexBytes(hexBytes));
            Assert.AreEqual(sExp, instr.ToString());
        }

        private X86Instruction AssertCode64(string sExp, params byte[] bytes)
        {
            var instr = Disassemble64(bytes);
            Assert.AreEqual(sExp, instr.ToString());
            return instr;
        }

        private void AssertCode64(string sExp, string hexBytes)
        {
            var instr = Disassemble64(BytePattern.FromHexBytes(hexBytes));
            Assert.AreEqual(sExp, instr.ToString());
        }

        [SetUp]
        public void Setup()
        {
            options = new Dictionary<string, object>();
        }

        [Test]
        public void X86dis_Sequence()
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            X86TextAssembler asm = new X86TextAssembler(arch);
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
                "0B96:0000\tmov\tax,0h\r\n" +
                "0B96:0003\tcwd\r\n" +
                "0B96:0004\tlodsb\r\n" +
                "0B96:0005\tdec\tcx\r\n" +
                "0B96:0006\tjnz\t0004h\r\n",
                s);
        }

        [Test]
        public void SegmentOverrides()
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            X86TextAssembler asm = new X86TextAssembler(arch);
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
        public void X86Dis_Rotations()
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            X86TextAssembler asm = new X86TextAssembler(arch);
            var lr = asm.AssembleFragment(
                Address.SegPtr(0xB96, 0),
                "foo	proc\r\n" +
                "rol	ax,cl\r\n" +
                "ror	word ptr [bx+2],cl\r\n" +
                "rcr	word ptr [bp+4],4\r\n" +
                "rcl	ax,1\r\n");

            var bmem = (ByteMemoryArea) lr.SegmentMap.Segments.Values.First().MemoryArea;
            CreateDisassembler16(bmem.CreateLeReader(bmem.BaseAddress));
            StringBuilder sb = new StringBuilder();
            foreach (var instr in dasm.Take(4))
            {
                sb.AppendFormat("{0}\r\n", instr.ToString());
            }
            string s = sb.ToString();
            Assert.AreEqual(
                "rol\tax,cl\r\n" +
                "ror\tword ptr [bx+2h],cl\r\n" +
                "rcr\tword ptr [bp+4h],4h\r\n" +
                "rcl\tax,1h\r\n", s);
        }

        [Test]
        public void X86Dis_rorx()
        {
            AssertCode64("rorx\trax,r10,2h", "C4C3FBF0C202");
        }

        [Test]
        public void X86Dis_Extensions()
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            IAssembler asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(
                Address.SegPtr(0xA14, 0),
@"		.i86
foo		proc
		movsx	ecx,word ptr [bp+8]
		movzx   edx,cl
		movsx	ebx,bx
		movzx	ax,byte ptr [bp+04]
");
            program.Platform = new DefaultPlatform(sc, arch);
            CreateDisassembler16(program.SegmentMap.Segments.Values.First().MemoryArea);
            StringBuilder sb = new StringBuilder();
            foreach (var ii in dasm.Take(4))
            {
                sb.AppendLine(string.Format("{0}", ii.ToString()));
            }
            string s = sb.ToString();
            Assert.AreEqual(
@"movsx	ecx,word ptr [bp+8h]
movzx	edx,cl
movsx	ebx,bx
movzx	ax,byte ptr [bp+4h]
", s);
        }

        private X86Instruction DisEnumerator_TakeNext(System.Collections.Generic.IEnumerator<X86Instruction> e)
        {
            e.MoveNext();
            return e.Current;
        }

        [Test]
        public void X86Dis_InvalidKeptStateRegression()
        {
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            X86TextAssembler asm = new X86TextAssembler(arch);
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

            var bmem = (ByteMemoryArea) lr.SegmentMap.Segments.Values.First().MemoryArea;
            CreateDisassembler32(bmem);
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
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            X86TextAssembler asm = new X86TextAssembler(arch);
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
        public void X86Dis_FpuInstructions()
        {
            using (FileUnitTester fut = new FileUnitTester("Intel/DisFpuInstructions.txt"))
            {
                var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
                X86TextAssembler asm = new X86TextAssembler(arch);
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
        public void X86Dis_SignedByte()
        {
            var instr = Disassemble16(0x83, 0xC6, 0x1);  // add si,+01
            Assert.AreEqual("add\tsi,1h", instr.ToString());
            Assert.AreEqual(PrimitiveType.Word16, instr.Operands[0].Width);
            Assert.AreEqual(PrimitiveType.Byte, instr.Operands[1].Width);
            Assert.AreEqual(PrimitiveType.Word16, instr.dataWidth);
        }

        [Test]
        public void X86Dis_LesBxStackArg()
        {
            var instr = Disassemble16(0xC4, 0x5E, 0x6);		// les bx,[bp+06]
            Assert.AreEqual("les\tbx,[bp+6h]", instr.ToString());
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
            byte[] image = new byte[] { 0xB8, 0x78, 0x56, 0x34, 0x12 };	// mov eax,0x12345678<32>
            ByteMemoryArea img = new ByteMemoryArea(Address.Ptr32(0x00100000), image);
            img.Relocations.AddPointerReference(0x00100001ul, 0x12345678);
            EndianImageReader rdr = img.CreateLeReader(img.BaseAddress);
            var decoders = ProcessorMode.Protected32.CreateRootDecoders(new Dictionary<string, object>());
            X86Disassembler dasm = new X86Disassembler(
                sc,
                decoders,
                ProcessorMode.Protected32,
                rdr,
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
            X86Instruction instr = dasm.First();
            Assert.AreEqual("mov\teax,12345678h", instr.ToString());
            Assert.AreEqual("ptr32", instr.Operands[1].Width.ToString());
        }

        [Test]
        public void X86Dis_RelocatedSegment()
        {
            byte[] image = new byte[] { 0x2E, 0xC7, 0x06, 0x01, 0x00, 0x00, 0x08 }; // mov cs:[0001],0800
            ByteMemoryArea img = new ByteMemoryArea(Address.SegPtr(0x900, 0), image);
            var relAddr = Address.SegPtr(0x900, 5);
            img.Relocations.AddSegmentReference(relAddr.ToLinear(), 0x0800);
            EndianImageReader rdr = img.CreateLeReader(img.BaseAddress);
            CreateDisassembler16(rdr);
            X86Instruction instr = dasm.First();
            Assert.AreEqual("mov\tword ptr cs:[0001h],800h", instr.ToString());
            Assert.AreEqual("selector", instr.Operands[1].Width.ToString());
        }

        [Test]
        public void X86dis_TestWithImmediateOperands()
        {
            var instr = Disassemble16(0xF6, 0x06, 0x26, 0x54, 0x01);     // test byte ptr [5426],01
            Assert.AreEqual("test\tbyte ptr [5426h],1h", instr.ToString());
            Assert.AreSame(PrimitiveType.Byte, instr.Operands[0].Width);
            Assert.AreSame(PrimitiveType.Byte, instr.Operands[1].Width);
            Assert.AreSame(PrimitiveType.Byte, instr.dataWidth, "Instruction data width should be byte");
        }

        [Test]
        public void X86dis_RelativeCallTest()
        {
            var instr = Disassemble16(0xE8, 0x00, 0xF0);
            Assert.AreEqual("call\t0F003h", instr.ToString());
            Assert.AreSame(PrimitiveType.Offset16, instr.Operands[0].Width);
        }

        [Test]
        public void X86dis_farCall()
        {
            var instr = Disassemble16(0x9A, 0x78, 0x56, 0x34, 0x01, 0x90, 0x90);
            Assert.AreEqual("call\tfar 0134h:5678h", instr.ToString());
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
        public void X86Dis_64_rexW()
        {
            var instr = Disassemble64(0x48, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\trdx,[rsp+20h]", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexWR()
        {
            var instr = Disassemble64(0x4C, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\tr10,[rsp+20h]", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexWB()
        {
            var instr = Disassemble64(0x49, 0x8D, 0x54, 0x24, 0x20);
            Assert.AreEqual("lea\trdx,[r12+20h]", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexR()
        {
            var instr = Disassemble64(0x44, 0x33, 0xC0);
            Assert.AreEqual("xor\tr8d,eax", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexRB()
        {
            var instr = Disassemble64(0x45, 0x33, 0xC0);
            Assert.AreEqual("xor\tr8d,r8d", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexB_bytesize()
        {
            var instr = Disassemble64(0x41, 0x30, 0xC0);
            Assert.AreEqual("xor\tr8b,al", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexR_bytesize_sil()
        {
            var instr = Disassemble64(0x44, 0x30, 0xC6);
            Assert.AreEqual("xor\tsil,r8b", instr.ToString());
        }

        [Test]
        public void X86Dis_64_rexB_wordsize()
        {
            var instr = Disassemble64(0x66, 0x41, 0x33, 0xC6);
            Assert.AreEqual("xor\tax,r14w", instr.ToString());
        }

        [Test]
        public void X86Dis_64_movaps()
        {
            var instr = Disassemble64(0x0f, 0x28, 0x44, 0x24, 0x20);
            Assert.AreEqual("movaps\txmm0,[rsp+20h]", instr.ToString());
        }

        [Test]
        public void X86Dis_64_movdqa()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7f, 0x44, 0x24, 0x20);
            Assert.AreEqual("movdqa\t[rsp+20h],xmm0", instr.ToString());
        }

        [Test]
        public void X86Dis_cmovnz()
        {
            var instr = Disassemble32(0x0F, 0x45, 0xC1);
            Assert.AreEqual("cmovnz\teax,ecx", instr.ToString());
        }

        [Test]
        public void X86Dis_Cmpxchg()
        {
            var instr = Disassemble32(0x0f, 0xb1, 0x0a, 0x85, 0xc0, 0x0f, 0x85, 0xdc);
            Assert.AreEqual("cmpxchg\t[edx],ecx", instr.ToString());
        }

        [Test]
        public void X86Dis_Xadd()
        {
            var instr = Disassemble32(0x0f, 0xC1, 0xC2);
            Assert.AreEqual("xadd\tedx,eax", instr.ToString());
        }

        [Test]
        public void X86Dis_cvttsd2si()
        {
            var instr = Disassemble32(0xF2, 0x0F, 0x2C, 0xC3);
            Assert.AreEqual("cvttsd2si\teax,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_fucompp()
        {
            var instr = Disassemble32(0xDA, 0xE9);
            Assert.AreEqual("fucompp", instr.ToString());
        }

        [Test]
        public void X86Dis_Call32()
        {
            var instr = Disassemble32(0xE9, 0x78, 0x56, 0x34, 012);
            var addrOp = (AddressOperand) instr.Operands[0];
            Assert.AreEqual("0C35567D", addrOp.ToString());
        }

        [Test]
        public void X86Dis_Call16()
        {
            var instr = Disassemble16(0xE9, 0x78, 0x56);
            var addrOp = (ImmediateOperand) instr.Operands[0];
            Assert.AreEqual("567B", addrOp.ToString());
        }

        [Test]
        public void X86Dis_DirectOperand32()
        {
            var instr = Disassemble32(0x8B, 0x15, 0x22, 0x33, 0x44, 0x55, 0x66);
            Assert.AreEqual("mov\tedx,[55443322h]", instr.ToString());
            var memOp = (MemoryOperand) instr.Operands[1];
            Assert.AreEqual("ptr32", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void X86Dis_DirectOperand16()
        {
            var instr = Disassemble16(0x8B, 0x16, 0x22, 0x33, 0x44);
            Assert.AreEqual("mov\tdx,[3322h]", instr.ToString());
            var memOp = (MemoryOperand) instr.Operands[1];
            Assert.AreEqual("word16", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void X86Dis_Movdqa()
        {
            var instr = Disassemble32(0x66, 0x0F, 0x6F, 0x06, 0x12, 0x34, 0x56);
            Assert.AreEqual("movdqa\txmm0,[esi]", instr.ToString());
        }
       
        [Test]
        public void X86Dis_bts()
        {
            var instr = Disassemble32(0x0F, 0xAB, 0x04, 0x24, 0xEB);
            Assert.AreEqual("bts\t[esp],eax", instr.ToString());
        }

        [Test]
        public void X86Dis_bzhi()
        {
            AssertCode64("bzhi\trax,rdx,r9", "C4E2B0F5C2");
        }

        [Test]
        public void X86Dis_cpuid()
        {
            var instr = Disassemble32(0x0F, 0xA2);
            Assert.AreEqual("cpuid", instr.ToString());
        }

        [Test]
        public void X86Dis_xgetbv()
        {
            var instr = Disassemble32(0x0F, 0x01, 0xD0);
            Assert.AreEqual("xgetbv", instr.ToString());
        }

        [Test]
        public void X86Dis_rdtsc()
        {
            var instr = Disassemble32(0x0F, 0x31);
            Assert.AreEqual("rdtsc", instr.ToString());
        }

        [Test]
        public void X86Dis_foo()
        {
            var instr = Disassemble32(0x0F, 0xBA, 0xF3, 0x00);
            Assert.AreEqual("btr\tebx,0h", instr.ToString());
        }

        [Test]
        public void X86Dis_movd_32()
        {
            AssertCode32("movd\tdword ptr [esi],mm1", 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tdword ptr [esi],xmm1", 0x66, 0x0F, 0x7E, 0x0E);
            AssertCode32("movq\txmm1,qword ptr [esi]", 0xF3, 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tesi,mm1", 0x0F, 0x7E, 0xCE);
            AssertCode32("movd\tesi,xmm1", 0x66, 0x0F, 0x7E, 0xCE);
            AssertCode32("movq\txmm1,xmm6", 0xF3, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void X86Dis_movd_64()
        {
            AssertCode64("movd\tdword ptr [rsi],mm1", 0x0F, 0x7E, 0x0E);
            AssertCode64("movd\tdword ptr [rsi],xmm1", 0x66, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\txmm1,qword ptr [rsi]", 0xF3, 0x0F, 0x7E, 0x0E);
            AssertCode64("movd\tesi,mm1", 0x0F, 0x7E, 0xCE);
            AssertCode64("movd\tesi,xmm1", 0x66, 0x0F, 0x7E, 0xCE);
            AssertCode64("movq\txmm1,xmm6", 0xF3, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void X86Dis_movd_64_rex()
        {
            AssertCode64("movq\t[rsi],mm1", 0x48, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\tqword ptr [rsi],xmm1", 0x66, 0x48, 0x0F, 0x7E, 0x0E);
            AssertCode64("movq\trsi,mm1", 0x48, 0x0F, 0x7E, 0xCE);
            AssertCode64("movq\trsi,xmm1", 0x66, 0x48, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void X86Dis_punpcklbw()
        {
            AssertCode32("punpcklbw\txmm1,xmm3", 0x66, 0x0f, 0x60, 0xcb);
        }

        [Test]
        public void X86Dis_more2()
        {
            AssertCode32("movlhps\txmm3,xmm3", 0x0f, 0x16, 0xdb);
            AssertCode32("pshuflw\txmm3,xmm3,0h", 0xf2, 0x0f, 0x70, 0xdb, 0x00);
        }

        [Test]
        public void X86dis_pcmpeqb()
        {
            AssertCode32("pcmpeqb\txmm0,[eax]", 0x66, 0x0f, 0x74, 0x00);
        }

        [Test]
        public void X86Dis_more3()
        {
            AssertCode32("stmxcsr\tdword ptr [ebp-0Ch]", 0x0f, 0xae, 0x5d, 0xf4);
            AssertCode32("palignr\txmm3,xmm1,0h", 0x66, 0x0f, 0x3a, 0x0f, 0xd9, 0x00);
            AssertCode32("movq\t[edi],xmm1", 0x66, 0x0f, 0xd6, 0x0f);
            AssertCode32("ldmxcsr\tdword ptr [ebp+8h]", 0x0F, 0xAE, 0x55, 0x08);
            AssertCode32("pcmpistri\txmm0,[edi-10h],40h", 0x66, 0x0F, 0x3A, 0x63, 0x47, 0xF0, 0x40);
        }

        [Test]
        public void X86Dis_64_movsxd()
        {
            AssertCode64("movsxd\trcx,dword ptr [rax+3Ch]", 0x48, 0x63, 0x48, 0x3c);
        }

        [Test]
        public void X86Dis_64_rip_relative()
        {
            AssertCode64("mov\trax,[rip+100000h]", 0x49, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00);
            AssertCode64("mov\trax,[rip+100000h]", 0x48, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00);
        }

        [Test]
        public void X86Dis_64_sub_immediate_dword()
        {
            AssertCode64("sub\trsp,+508h", 0x48, 0x81, 0xEC, 0x08, 0x05, 0x00, 0x00);
        }

        [Test]
        public void X86Dis_nops()
        {
            AssertCode64("nop", 0x90);
            AssertCode64("nop", 0x66, 0x90);
            AssertCode64("nop\tdword ptr [rax]", 0x0F, 0x1F, 0x00);
            AssertCode64("nop\tdword ptr [rax+0h]", 0x0F, 0x1F, 0x40, 0x00);
            AssertCode64("nop\tdword ptr [rax+rax+0h]", 0x0F, 0x1F, 0x44, 0x00, 0x00);
            AssertCode64("nop\tword ptr [rax+rax+0h]", 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00);
            AssertCode64("nop\tdword ptr [rax+0h]", 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00);
            AssertCode64("nop\tdword ptr [rax+rax+0h]", 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00);
            AssertCode64("nop\tword ptr [rax+rax+0h]", 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00);
        }

        [Test]
        public void X86Dis_rex_prefix_40()
        {
            AssertCode64("push\trbp", 0x40, 0x55);
        }

        [Test]
        public void X86Dis_repz_ret()
        {
            AssertCode64("ret", 0xF3, 0xC3);
        }

        [Test]
        public void X86Dis_emulate_x87_int_39()
        {
            options = new Dictionary<string, object> { { "Emulate8087", "true" } };
            CreateDisassembler16(0xCD, 0x39, 0x5E, 0xEA);
            var instrs = dasm.Take(2)
                .Select(i => i.ToString())
                .ToArray();
            Assert.AreEqual("nop", instrs[0]);
            Assert.AreEqual("fstp\tdouble ptr [bp-16h]", instrs[1]);
        }

        [Test]
        public void X86Dis_emulate_x87_int_3C()
        {
            options = new Dictionary<string, object> { { "Emulate8087", "true" } };
            CreateDisassembler16(0xCD, 0x3C, 0xDD, 0x06, 0x8B, 0x04);
            var instrs = dasm.Take(2)
                .Select(i => i.ToString())
                .ToArray();
            Assert.AreEqual("nop", instrs[0]);
            Assert.AreEqual("fld\tdouble ptr es:[048Bh]", instrs[1]);
        }

        [Test(Description = "Very large 32-bit offsets can be treated as negative offsets")]
        public void X86Dis_LargeNegativeOffset()
        {
            AssertCode32("mov\tesi,[eax-0FFF0h]", 0x8B, 0xB0, 0x10, 0x00, 0xFF, 0xFF);
            AssertCode32("mov\tesi,[eax+0FFFF0000h]", 0x8B, 0xB0, 0x00, 0x00, 0xFF, 0xFF);
        }

        [Test]
        public void X86Dis_StringOps()
        {
            X86TextAssembler asm = new X86TextAssembler(new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>()));
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

            MemoryArea mem = lr.SegmentMap.Segments.Values.First().MemoryArea;
            CreateDisassembler32(mem);
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
            AssertCode64("movups\t[rsp+20h],xmm0", 0x0F, 0x11, 0x44, 0x24, 0x20);
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
            AssertCode64("movups\txmm0,[rbp-20h]", 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movupd\txmm0,[rbp-20h]", 0x66, 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movss\txmm0,dword ptr [rbp-20h]", 0xF3, 0x0F, 0x10, 0x45, 0xE0);
            AssertCode64("movsd\txmm0,double ptr [rbp-20h]", 0xF2, 0x0F, 0x10, 0x45, 0xE0);
        }

        [Test]
        public void X86dis_movups_16bit()
        {
            AssertCode16("movups\txmm7,xmm3", 0x0F, 0x10, 0xFB);
        }

        [Test]
        public void X86dis_ucomiss()
        {
            AssertCode64("ucomiss\txmm0,dword ptr [rip+0B12Dh]", 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
        }

        [Test]
        public void X86dis_ucomisd()
        {
            AssertCode64("ucomisd\txmm0,double ptr [rip+0B12Dh]", 0x66, 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
        }

        [Test]
        public void X86dis_addss()
        {
            AssertCode64("addss\txmm1,dword ptr [rip+0B0FBh]", 0xF3, 0x0F, 0x58, 0x0D, 0xFB, 0xB0, 0x00, 0x00);
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
            AssertCode64("push\t42h", 0x6A, 0x42);
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
            AssertCode64("mfence", 0x0F, 0xAE, 0xF0);
        }

        [Test]
        public void X86dis_lfence()
        {
            AssertCode64("lfence", 0x0F, 0xAE, 0xE8);
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
            AssertCode64("vmovss\txmm0,dword ptr [rip+351h]", 0xC5, 0xFA, 0x10, 0x05, 0x51, 0x03, 0x00, 0x00);
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
            AssertCode64("vmovaps\t[rcx+4Dh],xmm4", 0xC5, 0xF8, 0x29, 0x61, 0x4D);
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
            AssertCode64("vaddpd\tymm0,ymm0,[rbp-90h]", 0xC5, 0xFD, 0x58, 0x85, 0x70, 0xFF, 0xFF, 0xFF);
        }

        [Test]
        public void X86dis_64_lea()
        {
            AssertCode64("lea\trdi,[rip+0DAh]", "488D3DDA000000");
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
            AssertCode32("lar\teax,word ptr [edx+42h]", 0x0F, 0x02, 0x42, 0x42);
        }


        [Test]
        public void X86dis_lsl()
        {
            AssertCode32("lsl\teax,word ptr [edx+42h]", 0x0F, 0x03, 0x42, 0x42);
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
        public void X86Dis_64_vzeroall()
        {
            AssertCode64("vzeroall", "C5FC77");
        }

        [Test]
        public void X86Dis_64_vzeroupper()
        {
            AssertCode64("vzeroupper", "C5F877");
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
            AssertCode32("prefetchw\tdword ptr [edx+42h]", 0x0F, 0x0D, 0x42, 0x42);
        }

        // /1
        [Test]
        public void X86dis_movlps()
        {
            AssertCode32("movlps\txmm0,qword ptr [edx+42h]", 0x0F, 0x12, 0x42, 0x42);
        }


        [Test]
        public void X86dis_unpcklpd()
        {
            AssertCode32("unpcklps\txmm0,[edx+42h]", 0x0F, 0x14, 0x42, 0x42);
            AssertCode32("unpcklpd\txmm0,[edx+42h]", 0x66, 0x0F, 0x14, 0x42, 0x42);
        }


        [Test]
        public void X86dis_movhpd()
        {
            AssertCode32("movhps\tqword ptr [edx+42h],xmm0", 0x0F, 0x17, 0x42, 0x42);
            AssertCode32("movhpd\tqword ptr [edx+42h],xmm0", 0x66, 0x0F, 0x17, 0x42, 0x42);
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
            AssertCode32("movntps\t[edx+42h],xmm0", 0x0F, 0x2B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvttps2pi()
        {
            AssertCode32("cvtps2pi\tmm0,xmmword ptr [edx+42h]", 0x0F, 0x2D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_comiss()
        {
            AssertCode32("comiss\txmm0,dword ptr [edx+42h]", 0x0F, 0x2F, 0x42, 0x42);
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
            AssertCode32("vpmovzxbw\txmm0,qword ptr [edx+42h]", 0x66, 0x0F, 0x38, 0x30, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_pdep()
        {
            AssertCode64("pdep\trax,r10,r10", "C4C2ABF5C2");
        }

        [Test]
        public void X86dis_permq()
        {
            AssertCode64("vpermq\tymm0,[rdx+42h],6h", "C4 E3 01 00 42 42 06");
            AssertCode32("illegal",    "0F 3A 00 42 42 06");
            AssertCode32("illegal", "66 0F 3A 00 42 42 06");
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
            AssertCode32("sqrtps\txmm0,[edx+42h]", 0x0F, 0x51, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rsqrtps()
        {
            AssertCode32("rsqrtps\txmm0,[edx+42h]", 0x0F, 0x52, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rcpps()
        {
            AssertCode32("rcpps\txmm0,[edx+42h]", 0x0F, 0x53, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_andn()
        {
            AssertCode64("andn\teax,eax,ebx", "C4E278F2C3");
        }

        [Test]
        public void X86dis_andps()
        {
            AssertCode32("andps\txmm0,[edx+42h]", 0x0F, 0x54, 0x42, 0x42);
        }

        [Test]
        public void X86dis_andnps()
        {
            AssertCode32("andnps\txmm0,[edx+42h]", 0x0F, 0x55, 0x42, 0x42);
        }

        [Test]
        public void X86dis_orps()
        {
            AssertCode32("orps\txmm0,[edx+42h]", 0x0F, 0x56, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_cvtss2si()
        { 
            AssertCode32("cvtss2si\teax,xmm3", "F30F2DC3");
        }
        
        [Test]
        public void X86dis_cvtps2pd()
        {
            AssertCode32("cvtps2pd\txmm0,[edx+42h]", 0x0F, 0x5A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvtdq2ps()
        {
            AssertCode32("cvtdq2ps\txmm0,[edx+42h]", 0x0F, 0x5B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_minps()
        {
            AssertCode32("minps\txmm0,[edx+42h]", 0x0F, 0x5D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckldq()
        {
            AssertCode32("punpckldq\tmm0,dword ptr [edx+42h]", 0x0F, 0x62, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtb()
        {
            AssertCode32("pcmpgtb\tmm0,dword ptr [edx+42h]", 0x0F, 0x64, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtw()
        {
            AssertCode32("pcmpgtw\tmm0,dword ptr [edx+42h]", 0x0F, 0x65, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtd()
        {
            AssertCode32("pcmpgtd\tmm0,dword ptr [edx+42h]", 0x0F, 0x66, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packuswb()
        {
            AssertCode32("packuswb\tmm0,dword ptr [edx+42h]", 0x0F, 0x67, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhbw()
        {
            AssertCode32("punpckhbw\tmm0,dword ptr [edx+42h]", 0x0F, 0x68, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhwd()
        {
            AssertCode32("punpckhwd\tmm0,dword ptr [edx+42h]", 0x0F, 0x69, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhdq()
        {
            AssertCode32("punpckhdq\tmm0,dword ptr [edx+42h]", 0x0F, 0x6A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packssdw()
        {
            AssertCode32("packssdw\tmm0,dword ptr [edx+42h]", 0x0F, 0x6B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqw()
        {
            AssertCode32("pcmpeqw\tmm0,[edx+42h]", 0x0F, 0x75, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqd()
        {
            AssertCode32("pcmpeqd\tmm0,[edx+42h]", 0x0F, 0x76, 0x42, 0x42);
        }

        [Test]
        public void X86dis_emms()
        {
            AssertCode32("emms", 0x0F, 0x77);
        }

        [Test]
        public void X86dis_vmread()
        {
            AssertCode32("vmread\t[edx+42h],eax", 0x0F, 0x78, 0x42, 0x42);
        }

        [Test]
        public void X86dis_vmwrite()
        {
            AssertCode32("vmwrite\teax,[edx+42h]", 0x0F, 0x79, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_bextr()
        {
            AssertCode64("bextr\teax,r11d,r10d", "C4C228F7C3");
        }

        [Test]
        public void X86dis_btc()
        {
            AssertCode32("btc\teax,[edx+42h]", 0x0F, 0xBB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_bsf()
        {
            AssertCode32("bsf\teax,[edx+42h]", 0x0F, 0xBC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_bsf_w16()
        {
            AssertCode64("bsf\tr11w,r15w", "66450FBCDF");
        }

        [Test]
        public void X86dis_cmpps()
        {
            AssertCode32("cmpps\txmm0,[edx+42h],8h", 0x0F, 0xC2, 0x42, 0x42, 0x08);
        }

        [Test]
        public void X86dis_movnti()
        {
            AssertCode32("movnti\t[edx+42h],eax", 0x0F, 0xC3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pinsrw()
        {
            //$TODO check encoding; look in the Intel spec.
            AssertCode32("pinsrw\tmm0,edx", 0x0F, 0xC4, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_pext()
        {
            AssertCode64("pext\tr8,r10,r10", "C442AAF5C2");
        }

        [Test]
        public void X86dis_pextrw()
        {
            AssertCode32("pextrw\teax,mm2,42h", 0x0F, 0xC5, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_shrx()
        {
            AssertCode64("shrx\teax,r11d,r8d", "C4C23BF7C3");
        }

        [Test]
        public void X86dis_shufps()
        {
            AssertCode32("shufps\txmm0,[edx+42h],7h", "0FC6424207");
        }

        [Test]
        public void X86dis_psrlq()
        {
            AssertCode32("psrlq\tmm0,[edx+42h]", 0x0F, 0xD3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmullw()
        {
            AssertCode32("pmullw\tmm0,[edx+42h]", 0x0F, 0xD5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmovmskb()
        {
            AssertCode32("pmovmskb\teax,mm2", 0x0F, 0xD7, 0x42);
        }

        [Test]
        public void X86dis_psubusb()
        {
            AssertCode32("psubusb\tmm0,[edx+42h]", 0x0F, 0xD8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminub()
        {
            AssertCode32("pminub\tmm0,[edx+42h]", 0x0F, 0xDA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddusb()
        {
            AssertCode32("paddusb\tmm0,[edx+42h]", 0x0F, 0xDC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxub()
        {
            AssertCode32("pmaxub\tmm0,[edx+42h]", 0x0F, 0xDE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pavgb()
        {
            AssertCode32("pavgb\tmm0,[edx+42h]", 0x0F, 0xE0, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psraw()
        {
            AssertCode32("psraw\tmm0,[edx+42h]", 0x0F, 0xE1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psrad()
        {
            AssertCode32("psrad\tmm0,[edx+42h]", 0x0F, 0xE2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmulhuw()
        {
            AssertCode32("pmulhuw\tmm0,[edx+42h]", 0x0F, 0xE4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmulhw()
        {
            AssertCode32("pmulhw\tmm0,[edx+42h]", 0x0F, 0xE5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_movntq()
        {
            AssertCode32("movntq\t[edx+42h],mm0", 0x0F, 0xE7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsb()
        {
            AssertCode32("psubsb\tmm0,[edx+42h]", 0x0F, 0xE8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsw()
        {
            AssertCode32("psubsw\tmm0,[edx+42h]", 0x0F, 0xE9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminsw()
        {
            AssertCode32("pminsw\tmm0,[edx+42h]", 0x0F, 0xEA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_por()
        {
            AssertCode32("por\tmm0,[edx+42h]", 0x0F, 0xEB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsb()
        {
            AssertCode32("paddsb\tmm0,[edx+42h]", 0x0F, 0xEC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsw()
        {
            AssertCode32("paddsw\tmm0,[edx+42h]", 0x0F, 0xED, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxsw()
        {
            AssertCode32("pmaxsw\tmm0,[edx+42h]", 0x0F, 0xEE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pxor()
        {
            AssertCode32("pxor\tmm0,[edx+42h]", 0x0F, 0xEF, 0x42, 0x42);
            AssertCode32("pxor\tmm0,mm1", 0x0F, 0xEF, 0xC1);
        }

        [Test]
        public void X86dis_psllw()
        {
            AssertCode32("psllw\tmm0,[edx+42h]", 0x0F, 0xF1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pslld()
        {
            AssertCode32("pslld\tmm0,[edx+42h]", 0x0F, 0xF2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psllq()
        {
            AssertCode32("psllq\tmm0,[edx+42h]", 0x0F, 0xF3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmuludq()
        {
            AssertCode32("pmuludq\tmm0,[edx+42h]", 0x0F, 0xF4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaddwd()
        {
            AssertCode32("pmaddwd\tmm0,[edx+42h]", 0x0F, 0xF5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psadbw()
        {
            AssertCode32("psadbw\tmm0,[edx+42h]", 0x0F, 0xF6, 0x42, 0x42);
        }

        [Test]
        public void X86dis_maskmovq()
        {
            AssertCode32("maskmovq\tmm0,[edx+42h]", 0x0F, 0xF7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubb()
        {
            AssertCode32("psubb\tmm0,[edx+42h]", 0x0F, 0xF8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubw()
        {
            AssertCode32("psubw\tmm0,[edx+42h]", 0x0F, 0xF9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubd()
        {
            AssertCode32("psubd\tmm0,[edx+42h]", 0x0F, 0xFA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubq()
        {
            AssertCode32("psubq\tmm0,[edx+42h]", 0x0F, 0xFB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddb()
        {
            AssertCode32("paddb\tmm0,[edx+42h]", 0x0F, 0xFC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddw()
        {
            AssertCode32("paddw\tmm0,[edx+42h]", 0x0F, 0xFD, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddd()
        {
            AssertCode32("paddd\tmm0,[edx+42h]", 0x0F, 0xFE, 0x42, 0x42);
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
            AssertCode32("fisttp\tqword ptr [eax+42h]", 0xDD, 0x48, 0x42);
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
            AssertCode32("fisttp\tword ptr [eax+42h]", 0xDF, 0x48, 0x42);
        }

        [Test]
        public void X86dis_fild_i16()
        {
            AssertCode32("fild\tword ptr [eax+42h]", 0xDF, 0x40, 0x42);
        }

        [Test]
        public void X86dis_fcomip()
        {
            AssertCode32("fcomip\tst(0),st(2)", 0xDF, 0xF2);
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
            Assert.AreEqual("sidt\t[rip+10AD7D6h]", instr.ToString());
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
        public void X86Dis_mulx()
        {
            AssertCode64("mulx\tr12,rax,rdx,r10", "C442FBF6E2");
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
            Assert.AreEqual("punpcklqdq\txmm0,xmm3", instr.ToString());
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
            Assert.AreEqual("bt\tebx,4h", instr.ToString());
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
        public void X86Dis_vblendvpd()
        {
            AssertCode64("vblendvpd\txmm0,xmm7,xmm2,xmm4", "C4E341 4BC242");
            AssertCode64("illegal", "0F3A4BC242");
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
            Assert.AreEqual("smsw\tword ptr [rcx+40h]", instr.ToString());
        }

        [Test]
        public void X86Dis_lmsw()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x74, 0x45, 0x48);
            Assert.AreEqual("lmsw\tword ptr [rbp+rax*2+48h]", instr.ToString());
        }

        [Test]
        public void X86Dis_invlpg()
        {
            var instr = Disassemble64(0x0f, 0x01, 0x78, 0x16);
            Assert.AreEqual("invlpg\tbyte ptr [rax+16h]", instr.ToString());
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
            Assert.AreEqual("pshufb\tmm0,[rcx+54h]", instr.ToString());
        }

        [Test]
        public void X86Dis_sha1msg2()
        {
            var instr = Disassemble64(0x0f, 0x38, 0xca, 0x74, 0x0b, 0x48);
            Assert.AreEqual("sha1msg2\txmm6,[rbx+rcx+48h]", instr.ToString());
        }

        [Test]
        public void X86Dis_packsswb_0f63dd()
        {
            var instr = Disassemble64(0x0f, 0x63, 0xdd);
            Assert.AreEqual("packsswb\tmm3,mm5", instr.ToString());
        }


        [Test]
        public void X86Dis_psrlq()
        {
            AssertCode64("psrlq\tmm3,2h", 0x0f, 0x73, 0xD3, 0x02);
        }

        [Test]
        public void X86Dis_psrld()
        {
            AssertCode64("psrld\tmm3,0Bh", "0F72D30B");
            AssertCode64("psrld\tmm4,[rdi+8E42A09h]", "0FD2A7092AE408"); 
            AssertCode64("vpsrld\txmm12,xmm6,[rdi+8E42A09h]", "C549D2A7092AE408");
        }

        [Test]
        public void X86Dis_haddpd()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7c, 0xC3);
            Assert.AreEqual("haddpd\txmm0,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_haddps()
        {
            var instr = Disassemble64(0xF2, 0x0f, 0x7c, 0xC3);
            Assert.AreEqual("haddps\txmm0,xmm3", instr.ToString());
        }

        [Test]
        public void X86Dis_hsubpd()
        {
            var instr = Disassemble64(0x66, 0x0f, 0x7d, 0xD4);
            Assert.AreEqual("hsubpd\txmm2,xmm4", instr.ToString());
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
            Assert.AreEqual("btr\t[rsp+30h],eax", instr.ToString());
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
            Assert.AreEqual("ud1\tesp,[rax+0F000001h]", instr.ToString());
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
            Assert.AreEqual("sldt\tword ptr es:[rcx-7Dh]", instr.ToString());
        }

        [Test]
        public void X86Dis_str()
        {
            var instr = Disassemble64(0x2e, 0x0f, 0x00, 0x48, 0x85);
            Assert.AreEqual("str\tword ptr cs:[rax-7Bh]", instr.ToString());
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
            Assert.AreEqual("btr\t[rsp+30h],rax", instr.ToString());
        }

        [Test]
        public void X86Dis_cmpxchg16b()
        {
            var instr = Disassemble64(0x0f, 0xc7, 0x4c, 0x24, 0x20);
            Assert.AreEqual("cmpxchg16b\txmmword ptr [rsp+20h]", instr.ToString());
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
            Assert.AreEqual("vpacksswb\tymm8,ymm1,[r9-7Dh]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpckhqdq()
        {
            var instr = Disassemble64(0xc4, 0x01, 0x75, 0x6d, 0x48, 0x89);
            Assert.AreEqual("vpunpckhqdq\tymm9,ymm1,[r8-77h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpshufb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x41, 0x00, 0x48, 0x8b);
            Assert.AreEqual("vpshufb\txmm9,xmm7,[r8-75h]", instr.ToString());
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
            Assert.AreEqual("vtestpd\tymm14,[r14+2434h]", instr.ToString());
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
            Assert.AreEqual("vphsubw\tymm11,ymm1,[r11+5Dh]", instr.ToString());
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
            Assert.AreEqual("vpsignb\tymm8,ymm1,[r9-0Ah]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmulhrsw_c402750b4883()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x0b, 0x48, 0x83);
            Assert.AreEqual("vpmulhrsw\tymm9,ymm1,[r8-7Dh]", instr.ToString());
        }


        [Test]
        public void X86Dis_vpermps()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x16, 0x48, 0x89);
            Assert.AreEqual("vpermps\tymm9,ymm1,[r8-77h]", instr.ToString());
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
            Assert.AreEqual("vpabsd\tymm9,[r8-75h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmovsxbw()
        {
            AssertCode64("vpmovsxbw\tymm9,qword ptr [r9-7Dh]", "C4 02 75 20 49 83");
        }

        [Test]
        public void X86Dis_vpmovsxbd()
        {
            AssertCode64("vpmovsxbd\tymm12,dword ptr [r13-75h]", "C4027521658B");
        }

        [Test]
        public void X86Dis_pmovsxbq()
        {
            AssertCode64("illegal", "0F3822418B");
            AssertCode64("pmovsxbq\txmm0,word ptr [rcx-75h]", "660F3822418B");
            AssertCode64("vpmovsxbq\tymm8,word ptr [r9-75h]", "C4027522418B");
        }

        [Test]
        public void X86Dis_vpmovsxwq()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x24, 0x48, 0x39);
            Assert.AreEqual("vpmovsxwq\tymm9,qword ptr [r8+39h]", instr.ToString());
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
            Assert.AreEqual("vpcmpeqq\tymm9,ymm1,[r11+15h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovpd_toreg()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2d, 0x5b, 0x48);
            Assert.AreEqual("vmaskmovpd\tymm11,ymm1,[r11+48h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovps_tomem()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2e, 0x80, 0x00, 0x34, 0x12, 0x00);
            Assert.AreEqual("vmaskmovps\t[r8+123400h],ymm1,ymm8", instr.ToString());
        }

        [Test]
        public void X86Dis_vmaskmovps_2()
        {
            AssertCode64("vmaskmovps\t[r8-0F734h],ymm1,ymm8", "C402752E80CC08FFFF");
        }

        [Test]
        public void X86Dis_vmaskmovpd_tomem()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2f, 0x4b, 0x43);
            Assert.AreEqual("vmaskmovpd\t[r11+43h],ymm1,ymm9", instr.ToString());
        }

        [Test]
        public void X86Dis_vpmovzxbd()
        {
            AssertCode64("vpmovzxbd\tymm8,dword ptr [r9-7Dh]", "C4 02 75 31 41 83");
        }

        [Test]
        public void X86Dis_vpmovzxbq()
        {
            AssertCode64("vpmovzxbq\tymm9,word ptr [r11+7Bh]", "C4 02 75 32 4B 7B");
        }

        [Test]
        [Ignore("Intel opcode map _appears_ to imply that 0x66 prefix is required, but none seen.")]
        public void X86Dis_vpmovzxbq_2()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x32, 0x8b, 0x7b, 0x10);
            Assert.AreEqual("vpmovzxbq\tymm9,DWORD PTR [r11-0x12ceef85<32>]", instr.ToString());
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
            Assert.AreEqual("vpmaxuw\tymm8,ymm1,[r8+70h]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpmaxud()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3f, 0x49, 0x8b);
            Assert.AreEqual("vpmaxud\tymm9,ymm1,[r9-75h]", instr.ToString());
        }
        [Test]
        public void X86Dis_vpsllvd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x47, 0x0f);
            Assert.AreEqual("vpsllvd\tymm9,ymm1,[r15]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpbroadcastb()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x78, 0x4c, 0x89, 0xe7);
            Assert.AreEqual("vpbroadcastb\tymm9,byte ptr [r9+r9*4-19h]", instr.ToString());
        }

        [Test]
        public void X86Dis_verr_sp()
        {
            var instr = Disassemble64(0xc4, 0x61, 0x29, 0x00, 0xE4);
            Assert.AreEqual("verr\tsp", instr.ToString());
        }

        [Test]
        public void X86Dis_vfmadd213ps()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xa8, 0x48, 0x63);
            Assert.AreEqual("vfmadd213ps\tymm9,ymm1,[r8+63h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vaesenc()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0xdc, 0x43, 0xe0, 0xf7, 0x00);
            Assert.AreEqual("vaesenc\txmm8,xmm1,[r11-20h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpgatherqq()
        {
            AssertCode64("vgatherqq\txmm8,xmm2,[r8]", "C402E99100");
        }

        [Test]
        public void X86Dis_vpshufb_ymm()
        {
            var instr = Disassemble64(0xc4, 0x22, 0x2d, 0x00, 0x48, 0x8b);
            Assert.AreEqual("vpshufb\tymm9,ymm10,[rax-75h]", instr.ToString());
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
            Assert.AreEqual("vpunpckhqdq\tymm6,ymm8,[r13+r9+1010101h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpermpd()
        {
            var instr = Disassemble64(0xc4, 0x83, 0xc5, 0x01, 0xeb, 0xae);
            Assert.AreEqual("vpermpd\tymm5,ymm11,0AEh", instr.ToString());
        }

        [Test]
        public void X86Dis_femms()
        {
            var instr = Disassemble64(0xc4, 0xc1, 0xc0, 0x0e);
            Assert.AreEqual("femms", instr.ToString());
        }

        [Test]
        public void X86Dis_addsubpd()
        {
            var instr = Disassemble64(0xc5, 0xe9, 0xd0, 0xfe);
            Assert.AreEqual("vaddsubpd\txmm7,xmm2,xmm6", instr.ToString());
        }

        [Test]
        public void X86Dis_vaddsubpd()
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
            Assert.AreEqual("vcvtdq2pd\txmm8,[rcx+5C415D5Bh]", instr.ToString());
        }



        [Test]
        public void X86Dis_vpacksswb()
        {
            var instr = Disassemble64(0xc5, 0x49, 0x63, 0x44, 0x24, 0x04);
            Assert.AreEqual("vpacksswb\txmm8,xmm6,[rsp+4h]", instr.ToString());
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
            Assert.AreEqual("vunpckhps\tymm9,ymm1,[rbx+50h]", instr.ToString());
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
            Assert.AreEqual("vhaddpd\tymm9,ymm1,[rax-75h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vhsubpd_ymm()
        {
            var instr = Disassemble64(0xc5, 0x75, 0x7d, 0x4c, 0x8d, 0xa3);
            Assert.AreEqual("vhsubpd\tymm9,ymm1,[rbp+rcx*4-5Dh]", instr.ToString());
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
            Assert.AreEqual("vaddsubps\tymm9,ymm1,[rax-77h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpunpcklqdq_ymm()
        {
            var instr = Disassemble64(0xc5, 0x7d, 0x6c, 0x45, 0x31);
            Assert.AreEqual("vpunpcklqdq\tymm8,ymm0,[rbp+31h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vmovlps_memdst()
        {
            var instr = Disassemble64(0xc5, 0x80, 0x13, 0x02);
            Assert.AreEqual("vmovlps\tqword ptr [rdx],xmm0", instr.ToString());
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
            Assert.AreEqual("vpunpcklqdq\tymm5,ymm14,[rip+4CE889FFh]", instr.ToString());
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
            Assert.AreEqual("vpslld\txmm2,xmm2,0FFh", instr.ToString());
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

        [Test]
        public void X86Dis_lldt()
        {
            var instr = Disassemble32(0x0F, 0x00, 0xD0);
            Assert.AreEqual("lldt\tax", instr.ToString());
        }

        [Test]
        public void X86Dis_vmovdqa()
        {
            AssertCode64("vmovdqa\txmm1,[rbp-40h]", 0xC5, 0xF9, 0x6F, 0x4D, 0xC0);
        }

        [Test]
        public void X86Dis_vmaskmovpd()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x2d, 0x5b, 0x48);
            Assert.AreEqual("vmaskmovpd\tymm11,ymm1,[r11+48h]", instr.ToString());
        }

        [Test]
        public void X86Dis_vpminuw_c402753af6()
        {
            var instr = Disassemble64(0xc4, 0x02, 0x75, 0x3a, 0xf6);
            Assert.AreEqual("vpminuw\tymm14,ymm1,ymm14", instr.ToString());
        }

        [Test]
        public void X86Dis_vunpckhpd()
        {
            var instr = Disassemble64(0xc5, 0x75, 0x15, 0x41, 0x83);
            Assert.AreEqual("vunpckhpd\tymm8,ymm1,[rcx-7Dh]", instr.ToString());
        }

        [Test]
        public void X86Dis_ffreep()
        {
            var instr = Disassemble64(0xdf, 0xc0);
            Assert.AreEqual("ffreep\tst(0)", instr.ToString());
        }

        [Test]
        public void X86Dis_expected_bad_group11_instruction()
        {
            var instr = Disassemble64(0xC6, 0xAE, 0x53, 0x87, 0x7A, 0x9B, 0xC3);
            Assert.AreEqual("illegal", instr.ToString());
        }

        [Test]
        public void X86Dis_mov_Ea_imm_group11()
        {
            var instr = Disassemble64(0xC6, 0x86, 0x53, 0x87, 0x7A, 0x9B, 0xC3);
            Assert.AreEqual("mov\tbyte ptr [rsi+9B7A8753h],0C3h", instr.ToString());
        }

        [Test]
        public void X86Dis_move_ea_group11_16bit()
        {
            var instr = Disassemble16(0xC6, 0xAE, 0x53, 0x87, 0x7A);
            Assert.AreEqual("mov\tbyte ptr [bp+8753h],7Ah", instr.ToString());
        }

        [Test]
        public void X86Dis_pop_group_1A_16bit()
        {
            AssertCode16("pop\tbx", "8FC3");
            AssertCode16("pop\tbx", "8FCB");
        }

        [Test]
        public void X86Dis_pop_group_1A_64bit()
        {
            AssertCode64("pop\tebx", "8FC3");
            AssertCode64("illegal", "8FCF");
        }

        // X86-64
        [Test]
        public void X86Dis_out_64()
        {
            AssertCode64("out\tdx,eax", "EF");
            AssertCode64("out\t50h,eax", "E7 50");
        }

        [Test]
        public void X86Dis_cwd16()
        {
            AssertCode16("cwd", "99");
            AssertCode16("cdq", "66 99");
        }

        [Test]
        public void X86Dis_cwd32()
        {
            AssertCode32("cdq", "99");
            AssertCode32("cwd", "66 99");
        }

        [Test]
        public void X86Dis_cdq64()
        {
            AssertCode64("cdq", "99");
            AssertCode32("cwd", "66 99");
            AssertCode64("cqo", "48 99");
        }

        [Test]
        public void X86Dis_cbw16()
        {
            AssertCode16("cbw", "98");
            AssertCode16("cwde", "66 98");
        }

        [Test]
        public void X86Dis_cwde32()
        {
            AssertCode32("cwde", "98");
            AssertCode32("cbw", "66 98");
        }

        [Test]
        public void X86Dis_cdqe64()
        {
            AssertCode64("cwde", "98");
            AssertCode32("cbw",  "66 98");
            AssertCode64("cdqe", "48 98");
        }

        [Test]
        public void X86Dis_mov_ds_64()
        {
            AssertCode64("mov\tds,di", "8E DF");
        }

        [Test]
        public void X86Dis_movsxd_64()
        {
            AssertCode64("movsxd\tesp,dword ptr [rcx]", "63 21");
            AssertCode64("movsxd\trsp,dword ptr [rcx]", "48 63 21");
        }

        [Test]
        public void X86Dis_jcxz()
        {
            AssertCode16("jcxz\t0006h", "E3 04");
            AssertCode32("jecxz\t10006h", "E3 04");
            AssertCode64("jrcxz\t10006h", "E3 04");
        }

        [Test]
        public void X86Dis_adc_64()
        {
            AssertCode64("adc\tdword ptr [edx-27h],6E13A5B4h", "67 81 52 D9 B4 A5 13 6E");
        }   

        [Test]
        public void X86Dis_fisub()
        {
            AssertCode64("fisub\tword ptr [ebx]", "67 DE 23");
        }

        [Test]
        public void X86Dis_addps()
        {
            AssertCode64("addps\txmm1,[rdi-12h]", "0F584FEE");
            AssertCode64("vaddps\txmm1,xmm10,[rdi-12h]", "C5A8584FEE7F");
        }

        [Test]
        public void X86Dis_xabort()
        {
            AssertCode64("xabort\t42h", "C6F842");
        }

        [Test]
        public void X86Dis_shufps()
        {
            AssertCode64("shufps\txmm5,xmm6,0DEh", "0FC6EEDE");
        }

        [Test]
        public void X86Dis_vshufps_VEX()
        { 
            AssertCode64("vshufps\txmm13,xmm5,xmm6,0DEh", "C550C6EEDE"); 
            AssertCode64("vshufps\tymm13,ymm5,ymm6,0DEh", "C554C6EEDE"); 
        }

        [Test]
        public void X86Dis_pmulhw()
        {
            AssertCode64("pmulhw\tmm5,[rbx]", "0FE52B");
            AssertCode64("vpmulhw\tymm5,ymm4,[rbx]", "C5DDE52B");
        }

        [Test]
        public void X86Dis_andps()
        {
            AssertCode64("andps\txmm6,[rdi-1Dh]", "0F5477E3");
        }

        [Test]
        public void X86Dis_vandps()
        {
            AssertCode64("vandps\txmm6,xmm9,[rdi-1Dh]", "C5B05477E3");
        }

        [Test]
        public void X86Dis_punpckhwd()
        {
            AssertCode64("punpckhwd\tmm6,dword ptr [rcx+7h]", "0F697107");
            AssertCode64("vpunpckhwd\tymm14,ymm12,[rcx+7h]", "C51D697107");
        }

        [Test]
        public void X86Dis_cmpss()
        {
            AssertCode64("cmpss\txmm2,dword ptr [rdi+27h],69h", "F30FC2572769");
            AssertCode64("vcmpss\txmm10,xmm10,[rdi+27h],69h", "C52EC2572769");
        }

        [Test]
        public void X86Dis_pandn()
        {
            AssertCode64("pandn\tmm7,[rcx+0C148E7D4h]", "0FDFB9D4E748C1");
            AssertCode64("vpandn\txmm15,xmm6,[rcx+0C148E7D4h]", "C549DFB9D4E748C1");
        }

        [Test]
        public void X86Dis_psubd()
        {
            AssertCode64("psubd\tmm6,[rdi]", "0FFA37");
            AssertCode64("psubd\txmm6,[rdi]", "660FFA37");
            AssertCode64("vpsubd\tymm6,ymm1,[rdi]", "C5F5FA37"); //  833563B6A4BC9ADAF9C455");
        }

        [Test]
        public void X86Dis_pxor()
        {
            AssertCode64("pxor\tmm1,mm2", "0FEFCA"); //  BFCFF487E40067119259D0");
            AssertCode64("vpxor\txmm9,xmm10,xmm2", "C529EFCA"); // BFCFF487E40067119259D0");
        }

        [Test]
        public void X86Dis_andnps()
        {
            AssertCode64("andnps\txmm3,[rbx]", "0F551B"); 
        }

        [Test]
        public void X86Dis_andnpd()
        { 
            AssertCode64("andnpd\txmm3,[rbx]", "660F551B");
            AssertCode64("vandnpd\tymm3,ymm4,[rbx]", "C5DD551B");
        }

        [Test]
        public void X86Dis_vcmpneqpd()
        {
            AssertCode64("cmpps\txmm4,[rsi],4h", "0FC22604");
            AssertCode64("vcmppd\txmm12,xmm3,[rsi],4h", "C561C22604");
        }

        [Test]
        [Ignore("EVEX prefix is horrendously complex.")]
        public void X86Dis_vpermt2pd()
        {
            AssertCode64("vpermt2pd\tymm12{k7},ymm25,QWORD PTR [r11]{1to4}", "6252B5377F2312423CAC2746CA518E");
        }

        [Test]
        public void X86Dis_and_64_rex()
        {
            AssertCode64("and\trax,-3BDB14BCh", "48 25 44 EB 24 C4");
        }


        [Test]
        [Ignore("Discovered by RekoSifter tool")]
        public void X86Dis_pushw_64()
        {
            AssertCode64("pushw\t0x33f1", "66 68 F1 33");
        }

        [Test]
        public void X86Dis_vpaddsb()
        {
            AssertCode64("vpaddsb\txmm8,xmm10,[rdi]", "C529EC07");
        }

        [Test]
        public void X86Dis_ReservedNop_0F18()
        {
            AssertCode64("nop", "0F1826");
        }

        [Test]
        public void X86Dis_xchg()
        {
            AssertCode64("xchg\tr13,rax", "49 95");
        }

        [Test]
        public void X86Dis_cvtpi2ps()
        {
            AssertCode64("cvtpi2ps\txmm7,[rdx+0B24DEEE0h]", "0F 2A BA E0 EE 4D B2");
        }

        [Test]
        [Ignore("Think about this, some more -- Intel manual allows it")]
        public void X86Dis_enterw()
        {
            AssertCode64("enterw\t0x854,0xD5", "66 C8 54 08 D5");
        }

        [Test]
        public void X86Dis_Redundant_SIB()
        {
            AssertCode64("add\t[rax],ebp", "012c20");
        }

        [Test]
        public void X86Dis_mov_dr0_rax()
        {
            AssertCode64("mov\tdr0,rax", "0F23C0");
        }

        [Test]
        public void X86Dis_out_with_rex_prefix()
        {
            // REX prefix should have no effect.
            AssertCode64("out\tdx,al", "4F EE");
        }

        [Test]
        public void X86Dis_rw_mov_seg()
        {
            AssertCode64("mov\t[rax+76h],cs", "8C 48 76    ");
            AssertCode64("mov\t[rax+76h],cs", "48 8C 48 76 ");
            AssertCode64("mov\teax,cs",      "8c c8       ");
            AssertCode64("mov\tax,cs",       "66 8c c8    ");
            AssertCode64("mov\trax,cs",      "66 48 8c c8 ");       // REX prefix wins
            AssertCode64("mov\tax,cs",       "48 66 8c c8 ");        // data size override prefix wins
        }

        [Test]
        public void X86Dis_push_64_size_override()
        {
            AssertCode64("push\tcx", "66 51");
        }

        [Test]
        public void X86Dis_endbr()
        {
            AssertCode32("endbr32", "F3 0F 1E FB");
            AssertCode32("endbr64", "F3 0F 1E FA");
        }

        public void X86Dis_adcx()
        {
            AssertCode64("adcx\teax,ebx", "660F38F6C3");
        }

        [Test]
        public void X86Dis_adox()
        {
            AssertCode64("adox\teax,esp", "F30F38F6C4");
        }

        [Test(Description = "Tests customizing operator separator.")]
        public void X86Dis_OperandRendering()
        {
            var options = new MachineInstructionRendererOptions(
                operandSeparator: ", ");
            var instr = Disassemble16(0x33, 0xc0);
            Assert.AreEqual("xor\tax, ax", instr.ToString(options));
        }

        [Test]
        public void X86Dis_pextrX()
        {
            AssertCode64("pextrb\tebx,xmm0,4h",             "66 0f 3a 14 C3 04");
            AssertCode64("pextrb\tbyte ptr [rbx],xmm0,4h",  "66 0F 3A 14 03 04");
            AssertCode64("pextrd\tebx,xmm0,4h",             "66 0F 3A 16 C3 04");
            AssertCode64("pextrd\tdword ptr [rbx],xmm0,4h", "66 0F 3A 16 03 04");
            AssertCode64("pextrq\trbx,xmm0,4h",             "66 48 0f 3a 16 c3 04");
            AssertCode64("pextrq\tqword ptr [rbx],xmm0,4h", "66 48 0f 3a 16 03 04");
        }

        [Test]
        public void X86Dis_instr_tool_long_prefixes()
        {
            AssertCode32("xor\teax,fs:[eax]", "64 64 64 64  64 64 64 64  64 64 64 64  64 33 00");
            AssertCode32("illegal",           "64 64 64 64  64 64 64 64  64 64 64 64  64 64 33 00");
        }
    }
}
