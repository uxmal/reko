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

using Moq;
using NUnit.Framework;
using Reko.Arch.M68k;
using Reko.Arch.M68k.Assembler;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class Scanner_M68kTests
    {
        private M68kArchitecture arch;
        private Program program;
        private Scanner scanner;
        private ServiceContainer sc;
        private Mock<IDecompilerEventListener> listener;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            listener = new Mock<IDecompilerEventListener>();
            sc.AddService<IEventListener>(listener.Object);
            sc.AddService<IDecompilerEventListener>(listener.Object);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
        }

        private void BuildTest32(Action<M68kAssembler> asmProg)
        {
            arch = new M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            BuildTest(Address.Ptr32(0x00100000), new DefaultPlatform(sc, arch), asmProg);
        }

        private void BuildTest32(Address addrBase, params byte[] bytes)
        {
            arch = new M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var mem = new ByteMemoryArea(addrBase, bytes);
            var segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                new DefaultPlatform(null, arch));
            RunTest(addrBase);
        }

        private void BuildTest(Address addrBase, IPlatform platform, Action<M68kAssembler> asmProg)
        {
            var entryPoints = new List<ImageSymbol>();
            var asm = new M68kAssembler(arch, addrBase, entryPoints);
            asmProg(asm);

            program = asm.GetImage();

            RunTest(addrBase);
        }

        private void RunTest(Address addrBase)
        {
            var project = new Project { Programs = { program } };
            scanner = new Scanner(
                program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, new FakeDecompilerEventListener()),
                sc);
            scanner.EnqueueImageSymbol(ImageSymbol.Procedure(arch, addrBase), true);
            scanner.ScanImage();
        }

        [Test]
        public void ScanM68k_Simple()
        {
            BuildTest32(m =>
            {
                m.Move_l(m.d0, m.Pre(m.a7));
                m.Clr_l(m.d0);
                m.Move_l(m.Post(m.a7), m.d0);
                m.Rts();
            });
            var sw = new StringWriter();
            program.Procedures.Values[0].Write(true, sw);

            string sExp =
@"// fn00100000
// Return size: 4
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// a7:a7
// d0:d0
// ZN:ZN
// C:C
// V:V
// Z:Z
// N:N
// v10:v10
// return address size: 4
define fn00100000
fn00100000_entry:
	a7 = fp
	// succ:  l00100000
l00100000:
	a7 = a7 - 4<i32>
	Mem0[a7:word32] = d0
	ZN = cond(d0)
	C = false
	V = false
	d0 = 0<32>
	Z = true
	C = false
	N = false
	V = false
	v10 = Mem0[a7:word32]
	a7 = a7 + 4<i32>
	d0 = v10
	ZN = cond(d0)
	C = false
	V = false
	return
	// succ:  fn00100000_exit
fn00100000_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void ScanM68k_Zerofill()
        {
            BuildTest32(
                Address.Ptr32(0x01020),
                  0x41, 0xF9 , 0x00 , 0x00 , 0x3E , 0x94
                , 0x20, 0x3C , 0x00 , 0x00 , 0x00 , 0x30
                , 0x56, 0x80 
                , 0xE4, 0x88
                , 0x42, 0x98
                , 0x53, 0x80
                , 0x66, 0xFA
                , 0x4E, 0x75);
            var sw = new StringWriter();
            program.Procedures.Values[0].Write(true, sw);
            string sExp = @"// fn00001020
// Return size: 4
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// a7:a7
// a0:a0
// d0:d0
// ZN:ZN
// C:C
// V:V
// CVZNX:CVZNX
// Z:Z
// N:N
// return address size: 4
define fn00001020
fn00001020_entry:
	a7 = fp
	// succ:  l00001020
l00001020:
	a0 = 0x00003E94<p32>
	d0 = 0x30<32>
	ZN = cond(d0)
	C = false
	V = false
	d0 = d0 + 3<32>
	CVZNX = cond(d0)
	d0 = d0 >>u 2<32>
	CVZNX = cond(d0)
	// succ:  l00001030
l00001030:
	Mem0[a0:word32] = 0<32>
	a0 = a0 + 4<i32>
	Z = true
	C = false
	N = false
	V = false
	d0 = d0 - 1<32>
	CVZNX = cond(d0)
	branch Test(NE,Z) l00001030
	// succ:  l00001036 l00001030
l00001036:
	return
	// succ:  fn00001020_exit
fn00001020_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
