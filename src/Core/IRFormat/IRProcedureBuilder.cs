using System;
using Reko.Core.Code;
using Reko.Core.Expressions;

namespace Reko.Core.IRFormat
{
    internal class IRProcedureBuilder : CodeEmitter
    {
        private object arch;
        private string name;

        public IRProcedureBuilder(string name, object arch)
        {
            this.name = name;
            this.arch = arch;
        }

        public override Frame Frame
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Statement Emit(Instruction instr)
        {
            throw new NotImplementedException();
        }
    }
}