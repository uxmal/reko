#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.X86;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Arch.X86.Assembler
{
	[TestFixture]
	public class AssemblerAdvanced : AssemblerBase
	{
		[Test]
		public void AsAlloca()
		{
			RunTest("Fragments/multiple/alloca.asm", "Arch/X86/AsAlloca.txt", Address.SegPtr(0x0B00, 0x100)); 
		}

		[Test]
		public void AsImports()
		{
			RunTest("Fragments/import32/mallocfree.asm", "Arch/X86/AsImports.txt", Address.Ptr32(0x10000000)); 
		}

		[Test]
		public void AsLoopMalloc()
		{
			RunTest("Fragments/import32/loopmalloc.asm", "Arch/X86/AsLoopMalloc.txt", Address.Ptr32(0x10000000)); 
		}

		[Test]
		public void AsDuff()
		{
			RunTest("Fragments/duffs_device.asm", "Arch/X86/AsDuff.txt", Address.SegPtr(0x0C00, 0x0100)); 
		}

		[Test]
		public void AsMscOutput()
		{
			RunTest("Fragments/msc_output.asm", "Arch/X86/AsMscOutput.txt", Address.Ptr32(0x10000100)); 
		}
	}
}
