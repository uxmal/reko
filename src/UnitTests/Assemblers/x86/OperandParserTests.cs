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
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Reko.UnitTests.Assemblers.x86
{
	[TestFixture]
	public class OperandParserTests
	{
		private SymbolTable symtab;

		[SetUp]
		public void Setup()
		{
			symtab = new SymbolTable();
		}

		[Test]
		public void ParseRegisterOperand()
		{
			OperandParser opp = Create16BitParser("eax");
			ParsedOperand po = opp.ParseOperand();
			Assert.AreEqual("RegisterOperand", po.Operand.GetType().Name);
			Assert.AreEqual("eax", po.Operand.ToString());
		}

		[Test]
		public void ParseMemoryOperandWithUnspecifiedWidth()
		{
			OperandParser opp = Create16BitParser("[bx]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(po.Operand.Width, "Operand width should have been null, was " + po.Operand.Width);
			Assert.AreSame(Registers.bx, mop.Base);
		}

		[Test]
		public void ParseMemoryOperandWithSpecifiedWidth()
		{
			OperandParser opp = Create16BitParser("byte ptr [bx]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.AreSame(PrimitiveType.Byte, mop.Width);
			Assert.AreSame(Registers.bx, mop.Base);
		}

		[Test]
		public void ParseNumericMemoryOperand()
		{
			OperandParser opp = Create16BitParser("[0x21E]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.AreSame(PrimitiveType.Word16, mop.Offset.DataType);
			Assert.AreEqual("[021E]", mop.ToString(MachineInstructionWriterOptions.None));
		}

		[Test]
		public void ParseSibMemoryOperand()
		{
			OperandParser opp = Create16BitParser("[eax+eax*4]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNull(mop.Offset, "Offset should be null, but is " + mop.Offset);
			Assert.AreEqual("[eax+eax*4]", mop.ToString(MachineInstructionWriterOptions.None));
		}

		[Test]
		public void ParseMemorySymbol()
		{
			OperandParser opp = Create16BitParser("[foo]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNotNull(po.Symbol, "Should have defined symbol foo");
			Assert.AreEqual("[0000]", mop.ToString(MachineInstructionWriterOptions.None));
		}

		private OperandParser Create16BitParser(string data)
		{
			return new OperandParser(new Lexer(new StringReader(data)), symtab, Address.SegPtr(0x0C00, 0x0000), PrimitiveType.Word16, PrimitiveType.Word16);
		}

		private OperandParser Create32BitParser(string data)
		{
            return new OperandParser(new Lexer(new StringReader(data)), symtab, Address.SegPtr(0x0C00, 0x0000), PrimitiveType.Word32, PrimitiveType.Word32);
		}
	}
}
