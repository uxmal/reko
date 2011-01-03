#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Assemblers.x86;
using Decompiler.Arch.Intel;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using Decompiler.UnitTests.TestCode;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class RtlBackWalkerTests
    {
        private ProgramImage image;
        private IProcessorArchitecture arch;
        private Scanner scanner;
        private readonly string nl = Environment.NewLine;

        [Test]
        public void Create()
        {
            RtlBackWalker bw = new RtlBackWalker(arch, image) ;
        }

        private Block FindBlock(string name)
        {
            foreach (var b in scanner.Procedures.Values[0].ControlGraph.Nodes)
            {
                if (b.Name == name)
                    return b;
            }
            Assert.Fail(string.Format("No block with name {0}.", name));
            return null;
        }

        private void RunTest(Address addrJump, Block block, string sExp)
        {
            var ibw = new RtlBackWalker(arch, image);
            List<BackwalkOperation> bws = ibw.Backwalk(addrJump, block);
            var sw = new StringWriter();
            foreach (BackwalkOperation bwo in bws)
            {
                sw.WriteLine(bwo);
            }
            sw.WriteLine("Index register: {0}", ibw.IndexRegister);
            if (sExp != sw.ToString())
            {
                Console.WriteLine(sw);
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

        void BuildTest_x86_16(Action<IntelAssembler> builder)
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            var emitter = new IntelEmitter ();
            IntelAssembler asm = new IntelAssembler(arch, new Address(0xC00, 0x0000), emitter, new List<EntryPoint>());
            builder(asm);
            this.image = asm.GetImage();
            this.arch = arch;
            BuildTest(image.BaseAddress);
        }

        void BuildTest_x86_32(Action<IntelAssembler> builder)
        {
            var arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            var emitter = new IntelEmitter();
            IntelAssembler asm = new IntelAssembler(arch, new Address(0x10000), emitter, new List<EntryPoint>());
            builder(asm);
            this.image = asm.GetImage();
            this.arch = arch;
            BuildTest(image.BaseAddress);
        }

        private void BuildTest(Address startAddress)
        {
            scanner = new Scanner(arch, image, new FakePlatform(), null, new FakeDecompilerEventListener());
            scanner.EnqueueEntryPoint(new EntryPoint(startAddress, arch.CreateProcessorState()));
            scanner.ProcessQueue();
        }

        [Test]
		public void IbwSwitch16()
		{
			BuildTest_x86_16(X86.Switch);
            var sExp = 
                "cmp 2" + nl +
                "branch UGT" + nl +
                "& 255" + nl +
                "* 2" + nl + 
                "Index register: bl" + nl;
            RunTest(new Address(0x2340), FindBlock("l0C00_000A"), sExp);
		}

		[Test]
		public void IbwSwitch32()
		{
            BuildTest_x86_32(X86.Switch32);
            scanner.Procedures.Values[0].Write(false, Console.Out);

            var sExp =
                "cmp 3" + nl +
                "branch UGT" + nl +
                "deref 0001001A 1" + nl +
                "* 4" + nl +
                "Index register: eax" + nl;
            RunTest(new Address(0x10000), FindBlock("l0001000B"), sExp);
		}

		[Test]
		public void IbwPowersOfTwo()
		{
			Assert.IsTrue(IntelBackWalker.IsEvenPowerOfTwo(2), "2 is power of two");
			Assert.IsTrue(IntelBackWalker.IsEvenPowerOfTwo(4), "4 is power of two");
			Assert.IsTrue(IntelBackWalker.IsEvenPowerOfTwo(8), "8 is power of two");
			Assert.IsTrue(IntelBackWalker.IsEvenPowerOfTwo(16), "16 is power of two");
			Assert.IsTrue(IntelBackWalker.IsEvenPowerOfTwo(256), "256 is power of two");
			Assert.IsFalse(IntelBackWalker.IsEvenPowerOfTwo(3), "3 isn't power of two");
			Assert.IsFalse(IntelBackWalker.IsEvenPowerOfTwo(7), "7 isn't power of two");
			Assert.IsFalse(IntelBackWalker.IsEvenPowerOfTwo(127), "127 isn't power of two");
		}

	}
}

