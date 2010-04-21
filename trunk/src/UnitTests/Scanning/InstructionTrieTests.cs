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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using NUnit.Framework; 
using System;

namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class InstructionTrieTests
	{
		public InstructionTrieTests()
		{
		}

		[Test]
		public void Creation()
		{
			IntelInstructionComparer cmp = new IntelInstructionComparer();
			InstructionTrie trie = new InstructionTrie(cmp, cmp);
		}

		[Test]
		public void AddInstructions()
		{
			IntelInstructionComparer cmp = new IntelInstructionComparer();
			InstructionTrie trie = new InstructionTrie(cmp, cmp);
			IntelInstruction inst = CreatePush(Registers.bp);
			
			trie.AddInstructions(new object [] { inst });
			Assert.AreEqual(trie.Count, 1);

			trie.AddInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp) });
			Assert.AreEqual(trie.Count, 3);
		}

		/// <summary>
		/// Builds a trie with instrucion sequences, then generates subsequences of these and gets them scored.
		/// </summary>
		[Test]
		public void ScoreInstructions()
		{
			IntelInstructionComparer cmp = new IntelInstructionComparer();
			InstructionTrie trie = new InstructionTrie(cmp, cmp);
			trie.AddInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp) });
			trie.AddInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp),
				CreatePush(Registers.si),
				CreatePush(Registers.di) });

			long score = trie.ScoreInstructions(new object [] {
				CreatePush(Registers.bp) });
			Assert.AreEqual(2, score);
			score = trie.ScoreInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp) } );
			Assert.AreEqual(4, score);

			// This sequqnce matches one of the trie's strings exactly.
			score = trie.ScoreInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp),
				CreatePush(Registers.si),
				CreatePush(Registers.di) });
			Assert.AreEqual(6, score);

			// A longer sequence runs 'off' the trie, so it should have the same score
			// as the previous test
			score = trie.ScoreInstructions(new object [] {
				CreatePush(Registers.bp),
				CreateMov(Registers.bp, Registers.sp),
				CreatePush(Registers.si),
				CreatePush(Registers.di),
				CreatePush(Registers.bx)});
			Assert.AreEqual(6, score);

			// This doesn't exist in the trie at all, so it should score 0.

			score = trie.ScoreInstructions(new object [] {
				CreateMov(Registers.ax, Registers.bx) });
			Assert.AreEqual(0, score);
		}

		private IntelInstruction CreateMov(IntelRegister regDst, IntelRegister regSrc)
		{
            IntelInstruction inst = new IntelInstruction(
                Opcode.mov,
                PrimitiveType.Word16,
                PrimitiveType.Word16,
                new RegisterOperand(regDst),
                new RegisterOperand(regSrc));
			return inst;
		}

		private IntelInstruction CreatePush(IntelRegister reg)
		{
            IntelInstruction inst = new IntelInstruction(
                Opcode.push,
                reg.DataType,
                reg.DataType,
                new RegisterOperand(reg));
			return inst;
		}
	}
}
