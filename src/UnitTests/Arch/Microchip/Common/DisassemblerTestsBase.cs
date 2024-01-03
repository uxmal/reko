#region License
/* 
 * Copyright (C) 2017-2024 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.MicrochipPIC.Common;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Libraries.Microchip;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    public class DisassemblerTestsBase
    {
        protected IPICProcessorModel picModel;
        protected static PICArchitecture arch;
        protected Address baseAddr = Address.Ptr32(0x200);

        private MachineInstruction _runTest(params ushort[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
            }).ToArray();
            var image = new ByteMemoryArea(baseAddr, bytes);
            var rdr = new LeImageReader(image, 0);
            var dasm = picModel.CreateDisassembler(arch, rdr);
            return dasm.First();
        }

        private string _fmtBinary(string mesg, params ushort[] words)
        {
            string sPIC = $"{arch.PICDescriptor.PICName}/{PICMemoryDescriptor.ExecMode}";
            if (words.Length < 1) return $"{sPIC} {mesg}";
            return sPIC + "[" + string.Join("-", words.Select(w => w.ToString("X4"))) + "] " + mesg;
        }

        protected void VerifyDisasm(string sExpected, string sMesg, params ushort[] words)
        {
            var instr = _runTest(words);
            Assert.AreEqual(sExpected, instr.ToString(), _fmtBinary(sMesg, words));
        }

        protected void SetPICModel(string picName, PICExecMode mode = PICExecMode.Traditional)
        {
            arch = new PICArchitecture(new ServiceContainer(), "pic", new Dictionary<string, object>()) { Options = new PICArchitectureOptions(picName, mode) };
            picModel = arch.ProcessorModel;
            arch.CreatePICProcessorModel();
            PICMemoryDescriptor.ExecMode = mode;
        }

    }

}
