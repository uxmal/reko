/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Arch.Intel;
using Decompiler.Loading;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]
	public class BackWalkerTests
	{
		[Test]
		public void IbwSwitch16()
		{
			RunTest(
				"Fragments/switch.asm", 
				"Intel/IbwSwitch16.txt",
				new IntelArchitecture(ProcessorMode.Real),
				new Address(0xC00, 0), 
				new Address(0xC00, 0x000D), 
				new IbwSwitch16Helper()); 
		}

		[Test]
		public void IbwSwitch32()
		{
			RunTest(
				"Fragments/switch32.asm", 
				"Intel/IbwSwitch32.txt",
				new IntelArchitecture(ProcessorMode.ProtectedFlat),
				new Address(0, 0x10000000), 
				new Address(0, 0x10000013), 
				new IbwSwitch32Helper()); 
		}

		[Test]
		public void IbwInc()
		{
			IntelBackWalker ibw = new IntelBackWalker(new IntelArchitecture(ProcessorMode.ProtectedFlat), null);
			IntelInstruction instr = new IntelInstruction(Opcode.inc, null, null, new RegisterOperand(Registers.di));
			ArrayList instrs = new ArrayList();
			instrs.Add(instr);
			ArrayList ops = new ArrayList();
			MachineRegister r = ibw.BackwalkInstructions(Registers.di, instrs, 0, ops);
			Assert.AreSame(Registers.di, r);
			Assert.AreEqual("+ 1", ops[0].ToString());
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

		private void RunTest(string sourceFile, string outputFile, IntelArchitecture arch, Address addrBase, Address addrJump, IBackWalkHost host)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Program prog = new Program();
				Loader ld = new Loader(prog);
				ld.Assemble(FileUnitTester.MapTestPath(sourceFile), arch, addrBase);
				prog.Architecture = arch;
				IntelDumper dumper = new IntelDumper(arch);
				dumper.ShowAddresses = true;
				dumper.ShowCodeBytes = true;
				dumper.DumpAssembler(prog.Image, prog.Image.BaseAddress, prog.Image.BaseAddress + prog.Image.Bytes.Length, fut.TextWriter);
				fut.TextWriter.Flush();

				IntelBackWalker ibw = new IntelBackWalker(arch, prog.Image);
				ArrayList bws = ibw.BackWalk(addrJump, host);
				foreach (BackwalkOperation bwo in bws)
				{
					fut.TextWriter.WriteLine(bwo);
				}
				fut.TextWriter.WriteLine("Index register: {0}", ibw.IndexRegister);

				fut.AssertFilesEqual();
			}
		}

		private class IbwSwitch16Helper : IBackWalkHost
		{
			public Address GetBlockStartAddress(Address addr)
			{
				switch (addr.off)
				{
					case 0x000D: return new Address(addr.seg, 0x0009);
					default: throw new ArgumentException(string.Format("offset {0:X4} not handled", addr.off));
				}
			}

			public AddressRange GetSinglePredecessorAddressRange(Address addrBegin)
			{
				if (addrBegin.off == 0x0009)
					return new AddressRange(new Address(addrBegin.seg, 0), addrBegin);
				else
					return null;
			}
		}

		private class IbwSwitch32Helper : IBackWalkHost
		{
			public AddressRange GetSinglePredecessorAddressRange(Address addr)
			{
				switch (addr.off)
				{
					case 0x1000000B: return new AddressRange(new Address(0x10000000), addr);
					default: throw new ArgumentException(string.Format("offset {0:X8} not handled", addr.off));
				}
			}

			public Address GetBlockStartAddress(Address addr)
			{
				switch (addr.off)
				{
					case 0x10000013: return new Address(0x1000000B);
					default: throw new ArgumentException(string.Format("offset {0:X8} not handled", addr.off));
				}
			}
		}
	}
}
