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

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class ThumbDisassemblerTests : ArmTestBase
    {
        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new ThumbProcessorArchitecture();
        }

        protected MachineInstruction Disassemble16(params ushort[] instrs)
        {
            var image = new LoadedImage(Address.Ptr32(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            foreach (var instr in instrs)
            {
                w.WriteLeUInt16(instr);
            }
            var arch = CreateArchitecture();
            var dasm = CreateDisassembler(arch, image.CreateLeReader(0));
            Assert.IsTrue(dasm.MoveNext());
            var armInstr = dasm.Current;
            dasm.Dispose();
            return armInstr;
        }

        protected override IEnumerator<MachineInstruction> CreateDisassembler(IProcessorArchitecture arch, ImageReader rdr)
        {
            return new ThumbDisassembler(rdr).GetEnumerator();
        }

        /*
  00402704: 46EB      mov         r11,sp
  00402706: B082      sub         sp,sp,#8
  00402708: F000 FA06 bl          00402B18
  0040270C: F7FF FE58 bl          004023C0
  00402710: 9000      str         r0,[sp]
  00402712: 9B00      ldr         r3,[sp]
  00402714: 9301      str         r3,[sp,#4]
  00402716: 9801      ldr         r0,[sp,#4]
  00402718: B002      add         sp,sp,#8
  0040271A: E8BD 8800 pop         {r11,pc}
  0040271E: 0000      movs        r0,r0
  00402720: 0000      movs        r0,r0         */
        [Test]
        public void ThumbDis_push()
        {
            var instr = Disassemble16(0xE92D, 0x4800);
            Assert.AreEqual("push.w\t{fp,lr}", instr.ToString());
        }
    }
}
