using Decompiler.Arch.Intel;
using Decompiler.Arch.Intel.Assembler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Decompiler.UnitTests.Intel.Assembler
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
			Assert.AreEqual("[021E]", mop.ToString(false));
		}

		[Test]
		public void ParseSibMemoryOperand()
		{
			OperandParser opp = Create16BitParser("[eax+eax*4]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNull(mop.Offset, "Offset should be null, but is " + mop.Offset);
			Assert.AreEqual("[eax+eax*4]", mop.ToString(false));
		}

		[Test]
		public void ParseMemorySymbol()
		{
			OperandParser opp = Create16BitParser("[foo]");
			ParsedOperand po = opp.ParseOperand();
			MemoryOperand mop = (MemoryOperand) po.Operand;
			Assert.IsNull(mop.Width, "Width should be undefined, but is " + mop.Width);
			Assert.IsNotNull(po.Symbol, "Should have defined symbol foo");
			Assert.AreEqual("[0000]", mop.ToString(false));
		}
		private OperandParser Create16BitParser(string data)
		{
			return new OperandParser(new Lexer(new StringReader(data)), symtab, new Address(0x0C00, 0x0000), PrimitiveType.Word16, PrimitiveType.Word16);
		}

		private OperandParser Create32BitParser(string data)
		{
			return new OperandParser(new Lexer(new StringReader(data)), symtab, new Address(0x0C00, 0x0000), PrimitiveType.Word32, PrimitiveType.Word32);
		}
	}
}
