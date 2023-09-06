using Reko.Core.Machine;
using System;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public class ParsedOperand
    {
        public ImmediateOperand? Immediate;
        public string? Identifier;

        private ParsedOperand() { }

        public static ParsedOperand UInt32(uint value)
        {
            return new ParsedOperand
            {
                Immediate = ImmediateOperand.UInt32(value)
            };
        }

        public static ParsedOperand Int32(int value)
        {
            return new ParsedOperand
            {
                Immediate = ImmediateOperand.Int32(value)
            };
        }

        public static ParsedOperand Id(string id)
        {
            return new ParsedOperand
            {
                Identifier = id
            };
        }
    }
}