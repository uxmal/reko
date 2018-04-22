#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// Contains various utility methods for modifying SSA state.
    /// </summary>
    public class SsaMutator
    {
        private SsaState ssa;
        private ExpressionEmitter m;

        public SsaMutator(SsaState ssa)
        {
            this.ssa = ssa;
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// Generates a "by-pass" of a register around a Call instruction.
        /// </summary>
        /// <remarks>
        /// If we know that a particular register is incremented/decremented
        /// by a specific constant amount after calling a procedure, we can remove
        /// any definitions of that register from the signature of the procedure.
        /// 
        /// Example: suppose we know that the SP register is incremented by 2 
        /// after the Call instruction:
        /// 
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: SP_94, [other registers]
        ///     
        /// this function modifies it to look like this:
        /// 
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: [other registers]
        /// SP_94 = SP_42 + 2
        /// //$TODO: this really belongs in a CallRewriter type class,
        /// but because the `master` and the `analysis-development` branches
        /// are out of sync, we put it in this class instead.
        /// </remarks>
        /// <param name="stm">The Statement containing the CallInstruction.</param>
        /// <param name="call">The CallInstruction.</param>
        /// <param name="register">The register whose post-call value is incremented.</param>
        /// <param name="delta">The amount by which to increment the register.</param>
        public void AdjustRegisterAfterCall(
            Statement stm,
            CallInstruction call,
            RegisterStorage register,
            int delta)
        {
            // Locate the post-call definition of the register, if any
            var defRegBinding = call.Definitions
                .Where(d => d.Identifier is Identifier idDef &&
                            idDef.Storage == register)
                .FirstOrDefault();
            if (defRegBinding == null)
                return;
            var defRegId = defRegBinding.Identifier as Identifier;
            if (defRegId == null)
                return;
            var usedRegExp = call.Uses
                .Select(u => u.Expression)
                .OfType<Identifier>()
                .Where(u => u.Storage == register)
                .FirstOrDefault();
            if (usedRegExp == null)
                return;

            // Generate an instruction that adjusts the register according to
            // the specified delta.
            var src = m.AddConstantWord(usedRegExp, register.DataType, delta);
            var ass = new Assignment(defRegId, src);
            var defSid = ssa.Identifiers[defRegId];

            // Insert the instruction after the call statement.
            var adjustRegStm = InsertStatementAfter(ass, stm);

            // Remove the bypassed register definition from
            // the call instructions.
            call.Definitions.Remove(defRegBinding);
            defSid.DefExpression = src;
            defSid.DefStatement = adjustRegStm;
            ssa.AddUses(adjustRegStm);
        }

        /// <summary>
        /// Inserts the instruction <paramref name="instr"/> into a 
        /// Block after the statement <paramref name="stmAfter"/>.
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="stmAfter"></param>
        /// <returns></returns>
        public Statement InsertStatementAfter(Instruction instr, Statement stmAfter)
        {
            var block = stmAfter.Block;
            var iPos = block.Statements.IndexOf(stmAfter);
            var linAddr = stmAfter.LinearAddress;
            return block.Statements.Insert(iPos + 1, linAddr, instr);
        }
    }
}
