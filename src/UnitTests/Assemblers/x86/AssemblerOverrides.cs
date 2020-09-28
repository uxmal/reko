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
using Reko.Core;
using System.Linq;

namespace Reko.UnitTests.Assemblers.x86
{
	[TestFixture]
	public class AssemblerOverrides : AssemblerBase
	{
		[Test]
		public void DataOverrides()
		{
			var lr = asm.AssembleFragment(
				Address.SegPtr(0xC00, 0),
				@".86
		mov	eax,32
		mov si,0x2234
		mov ebx,0x2234
		add	eax,0x12345678
		add ebx,0x87654321
");
			Assert.IsTrue(Compare(lr.SegmentMap.Segments.Values.First().MemoryArea.Bytes, 
                new byte[]
				{	0x66,0xb8,0x20,0x00,0x00,0x00,0xbe,0x34,
					0x22,0x66,0xbb,0x34,0x22,0x00,0x00,0x66,
					0x05,0x78,0x56,0x34,0x12,0x66,0x81,0xC3,
					0x21,0x43,0x65,0x87
                }));
		}
	}
}
