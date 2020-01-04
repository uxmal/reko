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
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Arch.Intel
{
	[TestFixture]
	public class IntelInstructionTrieTests
	{
		public IntelInstructionTrieTests()
		{
		}

		[Test]
		public void Creation()
		{
//			IntelInstructionTrie trie = new IntelInstructionTrie();
		}

		[Test]
		public void AddInstruction()
		{
/*			IntelInstructionTrie trie = new IntelInstructionTrie();
			IntelInstruction inst = new IntelInstruction();
			inst.code = Opcode.push;
			inst.op1 = new RegisterOperand(Register.bp);
			inst.cOperands = 1;
			trie.Add(inst);
			Assertion.AssertEquals(trie.Instructions == 1);
*/
		}
	}
}
