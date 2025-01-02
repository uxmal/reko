#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.MN103;
using Reko.Core;
using Reko.Core.Memory;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MN103
{
    [TestFixture]
    public class MN103DisassemblerTests : DisassemblerTestBase<MN103Instruction>
    {
        private readonly MN103Architecture arch;
        private readonly Address addr;

        public MN103DisassemblerTests()
        {
            this.arch = new MN103Architecture(CreateServiceContainer(), "mn103", new(), new(), new());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void MN103Dis_Generate()
        {
            var rnd = new Random(0x103);
            var buf = new byte[65536];
            rnd.NextBytes(buf);
            var mem = new ByteMemoryArea(addr, buf);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            var instrs = dasm.ToArray();
        }
    }
}
