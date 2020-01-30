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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Arch.Intel
{
#if BORKED      //$ Move to Decompiler.UnitTests.Scanning.Rewriter
	[TestFixture]
	public class DeadConditionFlagsFinderTests
	{
		private DeadConditionFlagsFinder dcff;

		[OneTimeSetUp]
		public void Setup()
		{
			dcff = new DeadConditionFlagsFinder();
		}

		/// <summary>
		/// At the end of a basic block we can't prove that the flags are dead,
		/// since we don't look at the successor blocks.
		/// </summary>
		[Test]
		public void DcffLiveAtEnd()
		{
			IntelInstruction [] instrs =
				new IntelInstruction[] {
										   new IntelInstruction(Mnemonic.cmp, PrimitiveType.Word32, PrimitiveType.Word32, new RegisterOperand(Registers.eax), new RegisterOperand(Registers.eax)),
										   new IntelInstruction(Mnemonic.jnz, PrimitiveType.Word32, PrimitiveType.Word32, new ImmediateOperand(Constant.Word32(0x10001010)))
									   };
			FlagM [] deadOut = dcff.DeadOutFlags(instrs);
			Assert.AreEqual(0, (int) deadOut[0]);
		}

		/// <summary>
		/// Flag bits can't be live between two adds.
		/// </summary>
		[Test]
		public void DcffConsecutiveAdds()
		{
			IntelInstruction [] instrs = 
				new IntelInstruction[] {
										   new IntelInstruction(Mnemonic.add, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.bx), new ImmediateOperand(Constant.Word16(0x10))),
										   new IntelInstruction(Mnemonic.mov, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.si), new RegisterOperand(Registers.bx)),
										   new IntelInstruction(Mnemonic.add, PrimitiveType.Word16, PrimitiveType.Word16, new RegisterOperand(Registers.cx), new ImmediateOperand(Constant.Word16(1)))
									   };
			FlagM [] deadOut = dcff.DeadOutFlags(instrs);
			Assert.AreEqual(FlagM.SF|FlagM.CF|FlagM.ZF|FlagM.OF, deadOut[0], "Item 0");
			Assert.AreEqual(FlagM.SF|FlagM.CF|FlagM.ZF|FlagM.OF, deadOut[1], "Item 1");
			Assert.AreEqual(0, (int) deadOut[2], "Item 2");
		}
	}
#endif
}