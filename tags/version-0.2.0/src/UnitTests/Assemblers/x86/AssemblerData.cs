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

using Decompiler;
using Decompiler.Core;
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Assemblers.x86
{
	[TestFixture]
	public class AssemblerData : AssemblerBase
	{
		[Test]
		public void StringTest()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"	.i86
foo		proc
		mov	si,offset data
		xor al,al
		rep scasb
		ret
foo		endp
data	db	'Hello',0
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte []
					{
						0xbe,0x08,0x00,0x32,0xc0,0xf3,0xae,0xc3,
						0x48,0x65,0x6c,0x6c,0x6f,0x0 }));
		}

		[Test]
		public void SwitchStatement()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"	.i86
foo		proc
		mov	bl,[si]
		xor bh,bh
		jmp	[bx+jmptable]

jmptable:
		dw	offset one
		dw	offset two
		dw	offset three
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
            Assert.IsTrue(Compare(asm.Image.Bytes, new byte[]
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
				0xc3, }));
		}

		[Test]
		public void MemOperandTest()
		{
			Program prog = new Program();
			asm.AssembleFragment(
				new Address(0xC00, 0),
				@"	.i86
		mov word ptr [bx+2],3
		mov byte ptr [bx+4],3
		mov dword ptr [bx+6],3
		add word ptr [bx+2],3
		add byte ptr [bx+4],3
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte []
			{
				0xC7, 0x47, 0x02, 0x03, 0x00,
				0xC6, 0x47, 0x04, 0x03,
				0x66, 0xC7, 0x47, 0x06, 0x03, 0x00, 0x00, 0x00,
				0x83, 0x47, 0x02, 0x03,
				0x80, 0x47, 0x04, 0x03
			}));
		}

		[Test]
		public void AssignPseudo()
		{
			Program prog = new Program();
			asm.AssembleFragment(
                new Address(0xC00, 0), 
				@".i86
		f = 4
		mov byte ptr [bx + f],3
		f= 8
		mov byte ptr [bx + f],3
");
			Assert.IsTrue(Compare(asm.Image.Bytes, new byte[]
			{
				0xC6, 0x47, 0x4, 0x3,
				0xC6, 0x47, 0x8, 0x3, 
			}));
		}

		[Test]
		public void AsAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", "Intel/AsAutoArray32.txt", new Address(0x04000000));
		}

		[Test]
		public void AsFpuTest()
		{
			RunTest("Fragments/fpuops.asm", "Intel/AsFpuTest.txt", new Address(0xC00, 0));
		}

		[Test]
		public void AsFpuReversibles()
		{
			RunTest("Fragments/fpureversibles.asm", "Intel/AsFpuReversibles.txt", new Address(0xC00, 0));
		}

		[Test]
		public void AsFrame32()
		{
			RunTest("fragments/multiple/frame32.asm", "Intel/AsFrame32.txt", new Address(0x10000000));
		}

		[Test]
		public void AsSwitch32()
		{
			RunTest("fragments/switch32.asm", "Intel/AsSwitch32.txt", new Address(0x10000000));
		}

		[Test]
		public void AsMem32()
		{
			RunTest("Fragments/mem32operations.asm", "Intel/AsMem32.txt", new Address(0x20000000));
		}

		[Test]
		public void AsVoidFunctions()
		{
			RunTest("Fragments/multiple/voidfunctions.asm", "Intel/AsVoidFunctions.txt", new Address(0xC00, 0));
		}

		[Test]
		public void AsCallVector()
		{
			RunTest("Fragments/multiple/calltables.asm", "Intel/AsCallVector.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsEnterLeave()
		{
			RunTest("Fragments/enterleave.asm", "Intel/AsEnterLeave.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsFpuArgs()
		{
			RunTest("Fragments/multiple/fpuargs.asm", "Intel/AsFpuArgs.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsFpuComps()
		{
			RunTest("Fragments/fpucomps.asm", "Intel/AsFpuComps.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsReg00003()
		{
			RunTest("Fragments/regressions/r00003.asm", "Intel/AsReg00003.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsReg00004()
		{
			RunTest("Fragments/regressions/r00004.asm", "Intel/AsReg00004.txt", new Address(0x10000000));
		}

		[Test]
		public void AsReg00005()
		{
			RunTest("Fragments/regressions/r00005.asm", "Intel/AsReg00005.txt", new Address(0x0B00, 0x0000));
		}

		[Test]
		public void AsReg00006()
		{
			RunTest("Fragments/regressions/r00006.asm", "Intel/AsReg00006.txt", new Address(0x100048B0));
		}

		[Test]
		public void AsPopNoPop()
		{
			RunTest("Fragments/multiple/popnopop.asm", "Intel/AsPopNoPop.txt", new Address(0xB00, 0));
		}


		[Test]
		public void AsPseudoprocs()
		{
			RunTest("Fragments/pseudoprocs.asm", "Intel/AsPseudoprocs.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsMemPreserve()
		{
			RunTest("Fragments/multiple/mempreserve.asm", "Intel/AsMemPreserve.txt", new Address(0x0C00, 0));
		}

		[Test]
		public void AsStackPointerMessing()
		{
			RunTest("Fragments/multiple/stackpointermessing.asm", "Intel/AsStackPointerMessing.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Intel/AsStringInstructions.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsTestCondition()
		{
			RunTest("Fragments/setcc.asm", "Intel/AsTestCondition.txt", new Address(0xB00, 0));
		}

		[Test]
		public void AsLivenessAfterCall()
		{
			RunTest("Fragments/multiple/livenessaftercall.asm", "Intel/AsLivenessAfterCall.txt", new Address(0xB00, 0));
		}

	}
}
