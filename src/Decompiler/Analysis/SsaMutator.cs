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
            var defRegBinding = call.Definitions.Where(
                u => u.Storage == register)
                .FirstOrDefault();
            if (defRegBinding == null)
                return;
            var defRegId = defRegBinding.Expression as Identifier;
            if (defRegId == null)
                return;
            var usedRegExp = call.Uses
                .Where(u => u.Storage == register)
                .Select(u => u.Expression)
                .FirstOrDefault();

            if (usedRegExp == null)
                return;
            var src = m.AddSubSignedInt(usedRegExp, delta);
            // Generate a statement that adjusts the register according to
            // the specified delta.
            var ass = new Assignment(defRegId, src);
            var defSid = ssa.Identifiers[defRegId];
            var adjustRegStm = InsertStatementAfter(ass, stm);
            defSid.DefExpression = src;
            defSid.DefStatement = adjustRegStm;
            call.Definitions.Remove(defRegBinding);
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

        /// <summary>
        /// After CallInstruction to Application rewriting some identifiers
        /// defined in CallInstruction can become undefined. Create
        /// '<id> = Constant.Invalid' instruction for each of undefined
        /// identifier to avoid losing information about their place of
        /// definition. Normally these instruction should be eliminated as
        /// 'dead' code. If they are not then it means that there are uses of
        /// <id> after call. So signanture of Application is incorrect.
        /// Assignment to 'Constant.Invalid' is good way to draw attention of
        /// user and developer.
        /// </summary>
        public void DefineUninitializedIdentifiers(
            Statement stm,
            CallInstruction call)
        {
            var trashedSids = call.Definitions.Select(d => (Identifier) d.Expression)
                .Select(id => ssa.Identifiers[id])
                .Where(sid => sid.DefStatement == null);
            foreach (var sid in trashedSids)
            {
                DefineUninitializedIdentifier(stm, sid);
            }
        }

        private void DefineUninitializedIdentifier(
            Statement stm,
            SsaIdentifier sid)
        {
            var value = Constant.Invalid;
            var ass = new Assignment(sid.Identifier, value);
            var newStm = InsertStatementAfter(ass, stm);
            sid.DefExpression = value;
            sid.DefStatement = newStm;
        }

        public void AdjustSsa(Statement stm, CallInstruction call)
        {
            ssa.ReplaceDefinitions(stm, null);
            ssa.RemoveUses(stm);
            ssa.AddDefinitions(stm);
            ssa.AddUses(stm);
            DefineUninitializedIdentifiers(stm, call);
        }
    }
}
