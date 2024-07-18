using Reko.Core.Code;
using System;
using System.Collections.Generic;

namespace Reko.Core.IRFormat
{
    internal class IRBlock
    {
        private Address addrCur;
        private string name;
        private readonly List<(Address, Instruction)> stmts;

        public IRBlock(Address addrCur, string name)
        {
            this.addrCur = addrCur;
            this.name = name;
            this.stmts = new List<(Address, Instruction)>(); 
        }

        public IReadOnlyList<(Address, Instruction)> Statements => stmts;

        public Instruction AddStatement(Address addrCur, Instruction instr)
        {
            stmts.Add((addrCur, instr));
            return instr;
        }
    }
}