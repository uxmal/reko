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
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.Intel
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
            arch = new X86ArchitectureFlat32("x86-protected-32");
            var services = new ServiceContainer();
            services.AddService<IFileSystemService>(new FileSystemServiceImpl());
            asm = new X86Assembler(services, new DefaultPlatform(services, arch), loadAddress, new List<ImageSymbol>());
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

        [Test]
        public void X86Rw_fstsw_sahf_jp()
        {
            BuildTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Sahf();
                m.Jc("foo");
            });
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|SCZO = FPUF",              //$TODO: P flag as well
                "2|T--|00010004(2): 1 instructions",
                "3|T--|if (Test(ULT,C)) branch 00010000"
                );
        }

        [Test]
        public void X86Rw_fstsw_ax_40_jne()
        {
            BuildTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x40);
                m.Jnz("foo");
            });
            AssertCode(
                "0|T--|00010000(8): 2 instructions",
                "1|L--|SCZO = FPUF",
                "2|T--|if (Test(EQ,FPUF)) branch 00010000"
                );            
        }

        [Test]
        public void X86Rw_fstsw_ax_01_je()
        {
            BuildTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x01);
                m.Jz("foo");
            });
            AssertCode(
                "0|T--|00010000(8): 2 instructions",
                "1|L--|SCZO = FPUF",
                "2|T--|if (Test(GE,FPUF)) branch 00010000"
                );
        }
    }
}
