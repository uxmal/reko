#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class BackWalkerTests
    {
        private ProgramImage image;
        private IProcessorArchitecture arch;
        private Scanner2 scanner;

        [Test]
        public void Create()
        {
            BackWalker2 bw = new BackWalker2(arch, image) ;
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
            scanner = new Scanner2(arch, image, new FakePlatform());
            scanner.EnqueueEntryPoint(new EntryPoint(startAddress, arch.CreateProcessorState()));
            scanner.ProcessQueue();
        }

        [Test]
		public void IbwSwitch16()
		{
			BuildTest_x86_16(delegate(IntelAssembler m)
            {
                m.Proc("foo");
                m.Push(m.cs);
                m.Pop(m.ds);
                m.Mov(m.bl, m.MemB(Registers.si, 0));
                m.Cmp(m.bl, 0x02);
                m.Ja("default");

                m.Label("test");
                m.Xor(m.bh, m.bh);
                m.Add(m.bx, m.bx);
                m.Jmp(m.MemW(Registers.bx, "jmptable"));
                
                m.Label("jmptable");
                m.Dw("one");
                m.Dw("two");
                m.Dw("three");

                m.Label("one");
                m.Mov(m.ax, 1);
                m.Ret();

                m.Label("two");
                m.Mov(m.ax, 2);
                m.Ret();

                m.Label("three");
                m.Mov(m.ax, 3);
                m.Ret();

                m.Label("default");
                m.Mov(m.ax, 0);
                m.Ret();
            });


            RunTest(new Address(0x2340), FindBlock("test"), "@@@");

                //"Fragments/switch.asm", 
                //"Intel/IbwSwitch16.txt",
                //new IntelArchitecture(ProcessorMode.Real),
                //new Address(0xC00, 0), 
                //new Address(0xC00, 0x000D), 
                //new IbwSwitch16Helper()); 
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
            var ibw = new BackWalker2(arch, image);
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

		[Test]
        [Ignore()]
		public void IbwSwitch32()
		{
            BuildTest_x86_32(delegate(IntelAssembler m)
            {
            });

            //RunTest(
            //    "Fragments/switch32.asm", 
            //    "Intel/IbwSwitch32.txt",
            //    new IntelArchitecture(ProcessorMode.ProtectedFlat),
            //    new Address(0, 0x10000000), 
            //    new Address(0, 0x10000013), 
            //    new IbwSwitch32Helper()); 
		}

		[Test]
        [Ignore()]

		public void IbwInc()
		{
            //IntelBackWalker ibw = new IntelBackWalker(new IntelArchitecture(ProcessorMode.ProtectedFlat), null);
            //IntelInstruction instr = new IntelInstruction(Opcode.inc, null, null, new RegisterOperand(Registers.di));
            //List<IntelInstruction> instrs = new List<IntelInstruction>();
            //instrs.Add(instr);
            //List<BackwalkOperation> ops = new List<BackwalkOperation>(); 
            //MachineRegister r = ibw.BackwalkInstructions(Registers.di, instrs, 0, ops);
            //Assert.AreSame(Registers.di, r);
            //Assert.AreEqual("+ 1", ops[0].ToString());
		}

		[Test]
        [Ignore()]

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

		private void RunTest(string sourceFile, string outputFile, IntelArchitecture arch, Address addrBase, Address addrJump, IBackWalkHost host)
		{
            throw new NotImplementedException();
            //using (FileUnitTester fut = new FileUnitTester(outputFile))
            //{
            //    AssemblerLoader ld = new AssemblerLoader(
            //        new IntelTextAssembler(),
            //        FileUnitTester.MapTestPath(sourceFile));
            //    Program prog = ld.Load(addrBase);
            //    Assert.IsTrue(prog.Architecture is IntelArchitecture);
            //    IntelDumper dumper = new IntelDumper(arch);
            //    dumper.ShowAddresses = true;
            //    dumper.ShowCodeBytes = true;
            //    dumper.DumpAssembler(prog.Image, prog.Image.BaseAddress, prog.Image.BaseAddress + prog.Image.Bytes.Length, fut.TextWriter);
            //    fut.TextWriter.Flush();

            //    IntelBackWalker ibw = new IntelBackWalker(arch, prog.Image);
            //    List<BackwalkOperation> bws = ibw.BackWalk(addrJump, host);
            //    foreach (BackwalkOperation bwo in bws)
            //    {
            //        fut.TextWriter.WriteLine(bwo);
            //    }
            //    fut.TextWriter.WriteLine("Index register: {0}", ibw.IndexRegister);

            //    fut.AssertFilesEqual();
            //}
		}

		private class IbwSwitch16Helper : IBackWalkHost
		{
			public Address GetBlockStartAddress(Address addr)
			{
				switch (addr.Offset)
				{
					case 0x000D: return new Address(addr.Selector, 0x0009);
					default: throw new ArgumentException(string.Format("offset {0:X4} not handled", addr.Offset));
				}
			}

			public AddressRange GetSinglePredecessorAddressRange(Address addrBegin)
			{
				if (addrBegin.Offset == 0x0009)
					return new AddressRange(new Address(addrBegin.Selector, 0), addrBegin);
				else
					return null;
			}
		}

		private class IbwSwitch32Helper : IBackWalkHost
		{
			public AddressRange GetSinglePredecessorAddressRange(Address addr)
			{
				switch (addr.Offset)
				{
					case 0x1000000B: return new AddressRange(new Address(0x10000000), addr);
					default: throw new ArgumentException(string.Format("offset {0:X8} not handled", addr.Offset));
				}
			}

			public Address GetBlockStartAddress(Address addr)
			{
				switch (addr.Offset)
				{
					case 0x10000013: return new Address(0x1000000B);
					default: throw new ArgumentException(string.Format("offset {0:X8} not handled", addr.Offset));
				}
			}
		}
	}
}

