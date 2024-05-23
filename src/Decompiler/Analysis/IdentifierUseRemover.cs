using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using System;

namespace Reko.Analysis
{
    internal class IdentifierUseRemover : InstructionUseVisitorBase
    {
        private readonly Statement stm;
        private readonly SsaIdentifierCollection identifiers;

        public IdentifierUseRemover(Statement stm, SsaIdentifierCollection identifiers)
        {
            this.stm = stm;
            this.identifiers = identifiers;
        }

        public static void Remove(Statement stm, SsaIdentifierCollection identifiers)
        {
            var iur = new IdentifierUseRemover(stm, identifiers);
            stm.Instruction.Accept(iur);
        }

        protected override void UseIdentifier(Identifier id)
        {
            if (identifiers.TryGetValue(id, out SsaIdentifier? sid))
                sid.Uses.RemoveAll(u => u == this.stm);
        }
    }
}