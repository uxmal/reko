#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Arch.X86.Rewriter;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    [TestFixture]
    public class RewriteFpuInstructionTests : Arch.RewriterTestBase
    {
        private X86ArchitectureFlat32 arch;
        private X86Assembler asm;
        private Program asmResult;
        private Address loadAddress = Address.Ptr32(0x0010000);

        [SetUp]
        public void Setup()
        {
            var services = new ServiceContainer();
            arch = new X86ArchitectureFlat32(services, "x86-protected-32", new Dictionary<string, object>());
            services.AddService<IFileSystemService>(new FileSystemService());
            asm = new X86Assembler(arch, loadAddress, new List<ImageSymbol>());
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => loadAddress;


        protected override IRewriterHost CreateHost()
        {
            return new FakeRewriterHost(null);
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea image, IStorageBinder binder, IRewriterHost host)
        {
            return new X86Rewriter(
                arch,
                host,
                new X86State(arch),
                asmResult.SegmentMap.Segments.Values.First().MemoryArea.CreateLeReader(0), binder);
        }


        private void BuildTest(Action<X86Assembler> m)
        {
            m(asm);
            asmResult = asm.GetImage();
        }
    }
}
