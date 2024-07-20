using Reko.Core.Code;
using System;
using System.Collections.Generic;

namespace Reko.Core.IRFormat
{
    public class IRBlock
    {
        private Address addrCur;
        private string id;
        private string? name;
        private readonly List<(Address, Instruction)> stmts;

        public IRBlock(Address addrCur, string id, string? name)
        {
            this.addrCur = addrCur;
            this.id = id;
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