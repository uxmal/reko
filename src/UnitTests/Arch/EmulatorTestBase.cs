#region License
/* 
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
using Reko.Arch.PowerPC;
using Reko.Core.Emulation;
using Reko.Core.Memory;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Loading;

namespace Reko.UnitTests.Arch
{
    public abstract class EmulatorTestBase
    {
        private readonly IProcessorArchitecture arch;
        private readonly Address addrBase;

        public EmulatorTestBase(IProcessorArchitecture arch, Address addrBase)
        {
            this.arch = arch;
            this.addrBase = addrBase;
        }

        protected IProcessorEmulator Emulator { get; private set; }

        protected void Given_Code(params string[] hexStrings)
        {
            var bytes = BytePattern.FromHexBytes(string.Join("", hexStrings));

            var mem = new ByteMemoryArea(addrBase, bytes);
            var seg = new ImageSegment("code", mem, AccessMode.ReadWriteExecute);
            var segmap = new SegmentMap(mem.BaseAddress, seg);
            var program = new Program(new ByteProgramMemory(segmap), arch, new DefaultPlatform(new ServiceContainer(), arch));

            var envEmu = new DefaultPlatformEmulator();

            Emulator = arch.CreateEmulator(segmap, envEmu);
            Emulator.InstructionPointer = program.ImageMap.BaseAddress;
            Emulator.ExceptionRaised += (sender, e) => { throw e.Exception; };
        }
    }
}
