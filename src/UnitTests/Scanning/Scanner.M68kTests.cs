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

using Moq;
using NUnit.Framework;
using Reko.Arch.M68k;
using Reko.Assemblers.M68k;
using Reko.Core;
using Reko.Core.Services;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class Scanner_M68kTests
    {
        private M68kArchitecture arch;
        private Program program;
        private Scanner scanner;
        private ServiceContainer sc;
        private Mock<DecompilerEventListener> listener;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            listener = new Mock<DecompilerEventListener>();
            sc.AddService<DecompilerEventListener>(listener.Object);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
        }

        private void BuildTest32(Action<M68kAssembler> asmProg)
        {
            arch = new M68kArchitecture("m68k");
            BuildTest(Address.Ptr32(0x00100000), new DefaultPlatform(null, arch), asmProg);
        }

        private void BuildTest32(Address addrBase, params byte[] bytes)
        {
            arch = new M68kArchitecture("m68k");
            var mem = new MemoryArea(addrBase, bytes);
            program = new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute)),
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
// a7:a7
// d0:d0
// v4:v4
// CVZN:CVZN
// Z:Z
// C:C
// N:N
// V:V
// v10:v10
// return address size: 4
define fn00100000
fn00100000_entry:
	a7 = fp
	// succ:  l00100000
l00100000:
	a7 = a7 - 4
	v4 = d0
	Mem0[a7:word32] = v4
	CVZN = cond(v4)
	d0 = 0x00000000
	Z = true
	C = false
	N = false
	V = false
	v10 = Mem0[a7:word32]
	a7 = a7 + 4
	d0 = v10
	CVZN = cond(d0)
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
// a7:a7
// a0:a0
// d0:d0
// CVZN:CVZN
// CVZNX:CVZNX
// Z:Z
// C:C
// N:N
// V:V
// return address size: 4
define fn00001020
fn00001020_entry:
	a7 = fp
	// succ:  l00001020
l00001020:
	a0 = 0x00003E94
	d0 = 0x00000030
	CVZN = cond(d0)
	d0 = d0 + 0x00000003
	CVZNX = cond(d0)
	d0 = d0 >>u 0x00000002
	CVZNX = cond(d0)
	// succ:  l00001030
l00001030:
	Mem0[a0:word32] = 0x00000000
	a0 = a0 + 4
	Z = true
	C = false
	N = false
	V = false
	d0 = d0 - 0x00000001
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
