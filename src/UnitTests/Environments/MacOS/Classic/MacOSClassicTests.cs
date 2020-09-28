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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Environments.MacOS.Classic;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class MacOSClassicTests
    {
        [Test(Description="Resolves a call to the jumptable pointed to by A5 as a direct call.")]
        public void MacOS_ResolveIndirectCall()
        {
            var macOS = new MacOSClassic(null, new M68kArchitecture("m68k"));
            var a5 = new Identifier(Registers.a5.Name, Registers.a5.DataType, Registers.a5);

            var a5world = new MemoryArea(Address.Ptr32(0x00100000), new byte[0x0300]);
            macOS.A5World = new ImageSegment("A5World", a5world, AccessMode.ReadWrite);
            macOS.A5Offset = 0x0100u;
            const int jumpTableOffset = 0x0032;
            const uint uAddrDirect = 0x00123400u;
            var w = new BeImageWriter(a5world, macOS.A5Offset + jumpTableOffset);
            w.WriteBeUInt16(0x4EF9);        // jmp... (not really necessary for the test but hey....);
            w.WriteBeUInt32(uAddrDirect);

            var m = new RtlEmitter(null);
            var instr = new RtlCall(m.IAdd(a5, jumpTableOffset), 4, InstrClass.Call);
            var addr =  macOS.ResolveIndirectCall(instr);

            Assert.AreEqual(uAddrDirect, addr.ToUInt32());
        }
    }
}
