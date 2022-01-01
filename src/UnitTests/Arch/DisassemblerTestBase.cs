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
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Reko.Core.Services;
using NUnit.Framework.Interfaces;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch
{
    public abstract class DisassemblerTestBase<TInstruction> : ArchTestBase
        where TInstruction : MachineInstruction
    {
        protected ServiceContainer CreateServiceContainer()
        {
            var sc = new ServiceContainer();
            sc.AddService<ITestGenerationService>(new UnitTestGenerationService(sc));
            return sc;
        }

        public TInstruction DisassembleBytes(params byte[] a)
        {
            ByteMemoryArea img = new ByteMemoryArea(LoadAddress, a);
            return Disassemble(img);
        }

        public TInstruction DisassembleWord(uint instr)
        {
            var img = new ByteMemoryArea(LoadAddress, new byte[256]);
            Architecture.CreateImageWriter(img, img.BaseAddress).WriteUInt32(0, instr);
            return Disassemble(img);
        }

        protected TInstruction DisassembleBits(string bitPattern)
        {
            var mem = new ByteMemoryArea(LoadAddress, new byte[256]);
            uint instr = BitStringToUInt32(bitPattern);
            Architecture.CreateImageWriter(mem, mem.BaseAddress).WriteUInt32(0, instr);
            return Disassemble(mem);
        }

        protected virtual TInstruction DisassembleHexBytes(string hexBytes)
        {
            byte[] instr = BytePattern.FromHexBytes(hexBytes);
            return DisassembleBytes(instr);
        }

        public TInstruction Disassemble(MemoryArea mem)
        {
            var dasm = this.CreateDisassembler(Architecture.CreateImageReader(mem, 0U));
            return (TInstruction) dasm.First();
        }

        protected virtual IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return Architecture.CreateDisassembler(rdr);
        }
    }
}
