#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Decompiler.Typing;

public class GlobalFieldNamerTests
{
    [Test]
    public void Gfn_NameGlobalString()
    {
        var program = new Program();
        var rodata = new ByteMemoryArea(Address.Ptr32(0x10_0000), new byte[0x1000]);
        rodata.WriteBytes(Encoding.UTF8.GetBytes("Hello"), 0x42, 5);
        var addrPstr = rodata.BaseAddress + 0x30;
        rodata.WriteLeUInt32(addrPstr, 0x10_0042);
        var map = new SegmentMap(new ImageSegment(".rodata", rodata, AccessMode.Read));
        var memory = new ByteProgramMemory(map);
        program.Memory = memory;
        program.Architecture = new FakeArchitecture();
        program.Platform = new DefaultPlatform(new ServiceContainer(), program.Architecture);
        program.AddGlobalField(addrPstr, new PointerType(PrimitiveType.Char, 32), null!);

        var gfn = new GlobalFieldNamer(program.TypeStore, program, 20);
        gfn.NameGlobalFields();

        Assert.That(program.GlobalFields.Fields.First().Name, Is.EqualTo("g_ptr100030_Hello"));
    }

}
