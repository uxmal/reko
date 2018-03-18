#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.Microchip.Common;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Core.Machine;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Disasm
{
    public class PIC18DisassemblerTestsBase
    {
        protected PICProcessorMode picMode;
        protected static PICArchitecture arch;
        protected Address baseAddr = Address.Ptr32(0x200);

        private MachineInstruction _runTest(params ushort[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
            }).ToArray();
            var image = new MemoryArea(baseAddr, bytes);
            var rdr = new LeImageReader(image, 0);
            var dasm = picMode.CreateDisassembler(arch, rdr);
            return dasm.First();
        }

        private string _fmtBinary(string mesg, params ushort[] words)
        {
            string sPIC = $"{arch.PICDescriptor.Name}/{arch.ExecMode}";
            if (words.Length < 1) return $"{sPIC} {mesg}";
            return sPIC + "[" + string.Join("-", words.Select(w => w.ToString("X4"))) + "] " + mesg;
        }

        protected void VerifyDisasm(string sExpected, string sMesg, params ushort[] words)
        {
            var instr = _runTest(words);
            Assert.AreEqual(sExpected, instr.ToString(), _fmtBinary(sMesg, words));
        }

        protected void SetPICMode(string picName, PICExecMode mode)
        {
            picMode = PICProcessorMode.GetMode(picName);
            arch = picMode.CreateArchitecture();
            arch.ExecMode = mode;
        }

    }

}
