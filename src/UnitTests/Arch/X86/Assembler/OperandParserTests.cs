#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Reko.UnitTests.Arch.X86.Assembler
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
			Assert.AreEqual("RegisterStorage", po.Operand.GetType().Name);
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
			Assert.AreEqual("[021E]", mop.ToString(MachineInstructionRendererOptions.Default));
		}

		[Test]
		public void ParseSibMemoryOperand()
		{
			OperandParser opp = Create16BitParser("[eax+eax*4]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNull(mop.Offset, "Offset should be null, but is " + mop.Offset);
			Assert.AreEqual("[eax+eax*4]", mop.ToString(MachineInstructionRendererOptions.Default));
		}

		[Test]
		public void ParseMemorySymbol()
		{
			OperandParser opp = Create16BitParser("[foo]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNotNull(po.Symbol, "Should have defined symbol foo");
			Assert.AreEqual("[0000]", mop.ToString(MachineInstructionRendererOptions.Default));
		}

		private OperandParser Create16BitParser(string data)
		{
			return new OperandParser(new Lexer(new StringReader(data)), symtab, Address.SegPtr(0x0C00, 0x0000), PrimitiveType.Word16, PrimitiveType.Word16);
		}

		private OperandParser Create32BitParser(string data)
		{
            return new OperandParser(new Lexer(new StringReader(data)), symtab, Address.SegPtr(0x0C00, 0x0000), PrimitiveType.Word32, PrimitiveType.Word32);
		}

        [Test]
        public void ParseImmediate_IntelSyntax()
        {
            OperandParser opp = Create16BitParser("21Eh");
            ParsedOperand po = opp.ParseOperand();
            ImmediateOperand mop = (ImmediateOperand) po.Operand;
            Assert.AreEqual("021E", mop.ToString(MachineInstructionRendererOptions.Default));
        }

        [Test]
        public void ParseMemoryAccess_IntelSyntax()
        {
            OperandParser opp = Create16BitParser("[ebp+0Ch]");
            ParsedOperand po = opp.ParseOperand();
            var mop = (MemoryOperand) po.Operand;
            Assert.AreEqual("[ebp+0C]", mop.ToString(MachineInstructionRendererOptions.Default));
        }

        [Test]
        public void ParseMemoryAccess_IntelSyntax_NegativeOffset()
        {
            OperandParser opp = Create16BitParser("[ebp-70h]");
            ParsedOperand po = opp.ParseOperand();
            var mop = (MemoryOperand) po.Operand;
            Assert.AreEqual("[ebp-70]", mop.ToString(MachineInstructionRendererOptions.Default));
        }
    }
}
