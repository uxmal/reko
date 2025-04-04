#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core;
using Reko.Core.Memory;
using System.Linq;

namespace Reko.UnitTests.Arch.X86.Assembler
{
	[TestFixture]
	public class AssemblerData : AssemblerBase
	{
        private Program lr;
        private ByteMemoryArea bmem;

        private void AssembleFragment(string asmSrc)
        {
            lr = asm.AssembleFragment(Address.SegPtr(0x0C00, 0), asmSrc);
            bmem = (ByteMemoryArea) lr.SegmentMap.Segments.Values.First().MemoryArea;
        }

		[Test]
		public void StringTest()
		{
			AssembleFragment(
				@"	.i86
foo		proc
		mov	si,Offset data
		xor al,al
		rep scasb
		ret
foo		endp
data	db	'Hello',0
");
			Assert.AreEqual(new byte []
					{
					0xbe,0x08,0x00,0x32,0xc0,0xf3,0xae,0xc3,
					0x48,0x65,0x6c,0x6c,0x6f,0x0
                },
                bmem.Bytes);
		}

		[Test]
		public void SwitchStatement()
		{
			AssembleFragment(
				@"	.i86
foo		proc
		mov	bl,[si]
		xor bh,bh
		jmp	[bx+jmptable]

jmptable:
		dw	Offset one
		dw	Offset two
		dw	Offset three
one:
		mov	ax,1
		ret
two:
		mov ax,2
		ret
three:
		mov ax,3
		ret
foo		endp
");
            Assert.AreEqual(new byte[]
			{
				0x8a,0x1c,0x32,0xff,
				0xff,0xa7,0x08,0x00,
				0x0e,0x00,
				0x12,0x00,
				0x16,0x00,
				0xb8,0x01,0x00,
				0xc3,
				0xb8,0x02,0x00,
				0xc3,
				0xb8,0x03,0x00,
				0xc3, },
                bmem.Bytes);
		}

		[Test]
		public void MemOperandTest()
		{
			AssembleFragment(
				@"	.i86
		mov word ptr [bx+2],3
		mov byte ptr [bx+4],3
		mov dword ptr [bx+6],3
		add word ptr [bx+2],3
		add byte ptr [bx+4],3
");
			Assert.AreEqual(new byte []
			{
				0xC7, 0x47, 0x02, 0x03, 0x00,
				0xC6, 0x47, 0x04, 0x03,
				0x66, 0xC7, 0x47, 0x06, 0x03, 0x00, 0x00, 0x00,
				0x83, 0x47, 0x02, 0x03,
				0x80, 0x47, 0x04, 0x03
			},
            bmem.Bytes);
		}

		[Test]
		public void AssignPseudo()
		{
			AssembleFragment(
				@".i86
		f = 4
		mov byte ptr [bx + f],3
		f= 8
		mov byte ptr [bx + f],3
");
			Assert.AreEqual(new byte[]
			{
				0xC6, 0x47, 0x4, 0x3,
				0xC6, 0x47, 0x8, 0x3, 
			},
            bmem.Bytes);
		}

        [Test]
        public void AsIndirectCall()
        {

        }

		[Test]
		public void AsAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", "Arch/X86/AsAutoArray32.txt", Address.Ptr32(0x04000000));
		}

		[Test]
		public void AsFpuTest()
		{
			RunTest("Fragments/fpuops.asm", "Arch/X86/AsFpuTest.txt", Address.SegPtr(0xC00, 0));
		}

		[Test]
		public void AsFpuReversibles()
		{
			RunTest("Fragments/fpureversibles.asm", "Arch/X86/AsFpuReversibles.txt", Address.SegPtr(0xC00, 0));
		}

		[Test]
		public void AsFrame32()
		{
			RunTest("Fragments/multiple/frame32.asm", "Arch/X86/AsFrame32.txt", Address.Ptr32(0x10000000));
		}

		[Test]
		public void AsSwitch32()
		{
			RunTest("Fragments/switch32.asm", "Arch/X86/AsSwitch32.txt", Address.Ptr32(0x10000000));
		}

		[Test]
		public void AsMem32()
		{
			RunTest("Fragments/mem32operations.asm", "Arch/X86/AsMem32.txt", Address.Ptr32(0x20000000));
		}

		[Test]
		public void AsVoidFunctions()
		{
			RunTest("Fragments/multiple/voidfunctions.asm", "Arch/X86/AsVoidFunctions.txt", Address.SegPtr(0xC00, 0));
		}

		[Test]
		public void AsCallVector()
		{
			RunTest("Fragments/multiple/calltables.asm", "Arch/X86/AsCallVector.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsEnterLeave()
		{
			RunTest("Fragments/enterleave.asm", "Arch/X86/AsEnterLeave.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsFpuArgs()
		{
			RunTest("Fragments/multiple/fpuArgs.asm", "Arch/X86/AsFpuArgs.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsFpuComps()
		{
			RunTest("Fragments/fpucomps.asm", "Arch/X86/AsFpuComps.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsReg00003()
		{
			RunTest("Fragments/regressions/r00003.asm", "Arch/X86/AsReg00003.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsReg00004()
		{
			RunTest("Fragments/regressions/r00004.asm", "Arch/X86/AsReg00004.txt", Address.Ptr32(0x10000000));
		}

		[Test]
		public void AsReg00005()
		{
			RunTest("Fragments/regressions/r00005.asm", "Arch/X86/AsReg00005.txt", Address.SegPtr(0x0B00, 0x0000));
		}

		[Test]
		public void AsReg00006()
		{
			RunTest("Fragments/regressions/r00006.asm", "Arch/X86/AsReg00006.txt", Address.Ptr32(0x100048B0));
		}

		[Test]
		public void AsPopNoPop()
		{
			RunTest("Fragments/multiple/popnopop.asm", "Arch/X86/AsPopNoPop.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsPseudoprocs()
		{
			RunTest("Fragments/pseudoprocs.asm", "Arch/X86/AsPseudoprocs.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsMemPreserve()
		{
			RunTest("Fragments/multiple/mempreserve.asm", "Arch/X86/AsMemPreserve.txt", Address.SegPtr(0x0C00, 0));
		}

		[Test]
		public void AsStackPointerMessing()
		{
			RunTest("Fragments/multiple/stackpointermessing.asm", "Arch/X86/AsStackPointerMessing.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Arch/X86/AsStringInstructions.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsTestCondition()
		{
			RunTest("Fragments/setcc.asm", "Arch/X86/AsTestCondition.txt", Address.SegPtr(0xB00, 0));
		}

		[Test]
		public void AsLivenessAfterCall()
		{
			RunTest("Fragments/multiple/livenessaftercall.asm", "Arch/X86/AsLivenessAfterCall.txt", Address.SegPtr(0xB00, 0));
		}
	}
}
