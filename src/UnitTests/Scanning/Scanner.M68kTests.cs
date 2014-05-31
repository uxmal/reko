#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler;
using Decompiler.Arch.M68k;
using Decompiler.Assemblers.M68k;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Scanning;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class Scanner_M68kTests
    {
        private MockRepository mr;
        private M68kArchitecture arch;
        private Program prog;
        private Scanner scanner;
        private DecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            listener = mr.Stub<DecompilerEventListener>();
        }

        private void BuildTest32(Action<M68kAssembler> asmProg)
        {
            arch = new M68kArchitecture();
            BuildTest(new Address(0x00100000), new DefaultPlatform(null, null), asmProg);
        }

        private void BuildTest(Address addrBase, Platform platform, Action<M68kAssembler> asmProg)
        {
            var entryPoints = new List<EntryPoint>();
            var asm = new M68kAssembler(arch, addrBase, entryPoints);
            asmProg(asm);

            var lr = asm.GetImage();
            prog = new Program
            {
                Architecture = arch,
                Image = lr.Image,
                ImageMap = lr.ImageMap,
                Platform = platform,
            };
            scanner = new Scanner(
                prog,
                new Dictionary<Address, ProcedureSignature>(),
                listener);
            scanner.EnqueueEntryPoint(new EntryPoint(addrBase, arch.CreateProcessorState()));
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
            scanner.Procedures.Values[0].Write(true, sw);

            string sExp =
@"// fn00100000
// Mem0:Global memory
// fp:fp
// a7:a7
// d0:d0
// v4:v4
// CVZN:Flags
// Z:Flags
// C:Flags
// N:Flags
// V:Flags
// v10:v10
// return address size: 4
void fn00100000()
fn00100000_entry:
	// succ:  l00100000
l00100000:
	a7 = a7 - 0x00000004
	v4 = d0
	Mem0[a7:word32] = v4
	CVZN = cond(v4)
	d0 = 0x00000000
	Z = true
	C = false
	N = false
	V = false
	v10 = Mem0[a7:word32]
	a7 = a7 + 0x00000004
	d0 = v10
	CVZN = cond(d0)
	return
	// succ:  fn00100000_exit
fn00100000_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
