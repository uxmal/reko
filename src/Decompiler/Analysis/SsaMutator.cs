#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Contains various utility methods for modifying SSA state.
    /// </summary>
    public class SsaMutator
    {
        private readonly SsaState ssa;
        private readonly ExpressionEmitter m;

        public SsaMutator(SsaState ssa)
        {
            this.ssa = ssa;
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// Generates a "by-pass" of a register around a <see cref="CallInstruction"/>.
        /// </summary>
        /// <remarks>
        /// If we know that a particular register is incremented/decremented
        /// by a specific constant amount after calling a procedure, we can remove
        /// any definitions of that register from the signature of the procedure.
        /// 
        /// Example: suppose we know that the SP register is incremented by 2 
        /// after the <c>call</c> instruction:
        /// <code>
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: SP_94, [other registers]
        /// </code>
        ///     
        /// this function modifies it to look like this:
        /// 
        /// <code>
        /// call [something]
        ///     uses: SP_42, [other registers]
        ///     defs: [other registers]
        /// SP_94 = SP_42 + 2
        /// </code>
        /// </remarks>
        /// <param name="stm">The Statement containing the CallInstruction.</param>
        /// <param name="call">The CallInstruction.</param>
        /// <param name="register">The register whose post-call value is incremented.</param>
        /// <param name="delta">The amount by which to increment the register.</param>
        /// <returns>True if changes were made.</returns>
        public bool AdjustRegisterAfterCall(
            Statement stm,
            CallInstruction call,
            RegisterStorage register,
            int delta)
        {
            // Locate the post-call definition of the register, if any
            var defRegBinding = call.Definitions.Where(
                u => u.Storage == register)
                .FirstOrDefault();
            if (defRegBinding?.Expression is not Identifier defRegId)
                return false;
            var usedRegExp = call.Uses
                .Where(u => u.Storage == register)
                .Select(u => u.Expression)
                .FirstOrDefault();

            if (usedRegExp is null)
                return false;
            var src = m.AddSubSignedInt(usedRegExp, delta);
            // Generate a statement that adjusts the register according to
            // the specified delta.
            var ass = new Assignment(defRegId, src);
            var defSid = ssa.Identifiers[defRegId];
            var adjustRegStm = InsertStatementAfter(ass, stm);
            defSid.DefStatement = adjustRegStm;
            call.Definitions.Remove(defRegBinding);
            ssa.AddUses(adjustRegStm);
            return true;
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
            var linAddr = stmAfter.Address;
            return block.Statements.Insert(iPos + 1, linAddr, instr);
        }

        /// <summary>
        /// Inserts an assignment statement into a <see cref="Block" /> after
        /// the statement <paramref name="stmAfter"/>.
        /// </summary>
        /// <remarks>
        /// The method generates an <see cref="SsaIdentifier"/> from the the
        /// supplied identifier <paramref name="dst"/>, creates an <see cref="Assignment"/>
        /// from the source expression <paramref name="src"/> to the new SSA
        /// Identifier, and inserts the resulting Assignment after the
        /// statement <paramref name="stmAfter"/>.
        /// </remarks>
        /// <param name="instr"><see cref="Instruction"/> to insert.</param>
        /// <param name="stmAfter">The <see cref="Statement"/> after which to
        /// insert the instruction.</param>
        /// <returns>The <see cref="SsaIdentifier"/> of the newly inserted
        /// assignment.
        /// </returns>
        public SsaIdentifier InsertAssignmentAfter(Identifier dst, Expression src, Statement stmAfter)
        {
            var stmts = stmAfter.Block.Statements;
            var iPos = stmts.IndexOf(stmAfter);
            var stm = stmts.Insert(iPos + 1, stmAfter.Address, null!);
            var sid = ssa.Identifiers.Add(dst, stm, false);
            stm.Instruction = new Assignment(sid.Identifier, src);
            ssa.AddUses(stm);
            return sid;
        }

        /// <summary>
        /// Inserts an assignment <see cref="Block" /> before the statement
        /// <paramref name="stmBefore"/>.
        /// </summary>
        /// <remarks>
        /// The method generates an <see cref="SsaIdentifier"/> from the the
        /// supplied identifier <paramref name="dst"/>, creates an <see cref="Assignment"/>
        /// from the source expression<paramref name="src"/> to the new SSA
        /// Identifier, and inserts the resulting Assignment before the given
        /// statement <paramref name="stmBefore"/>.
        /// </remarks>
        /// <param name="instr"><see cref="Instruction"/> to insert.</param>
        /// <param name="stmBefore">The <see cref="Statement"/> before which
        /// to insert the instruction.</param>
        /// <returns>The <see cref="SsaIdentifier"/> of the newly inserted
        /// assignment.</returns>
        public SsaIdentifier InsertAssignmentBefore(Identifier dst, Expression src, Statement stmBefore)
        {
            var stmts = stmBefore.Block.Statements;
            var iPos = stmts.IndexOf(stmBefore);
            var stm =  stmts.Insert(iPos, stmBefore.Address, null!);
            var sid = ssa.Identifiers.Add(dst, stm, false);
            stm.Instruction = new Assignment(sid.Identifier, src);
            ssa.AddUses(stm);
            return sid;
        }

        /// <summary>
        /// After rewriting <see cref="CallInstruction"/>s to <see cref="Application"/>s,
        /// some identifiers defined in CallInstruction can become undefined.
        /// This method creates an '<id> = Constant.Invalid' instruction for
        /// each of undefined identifiers to avoid losing information about
        /// their place of definition. Normally these instruction should be
        /// eliminated as'dead' code. If they are not then it means that there
        /// are uses of <id> after the call. This implies the signanture of 
        /// the Application is incorrect. An assignment to an <see cref="InvalidConstant"/>
        /// is good way to draw attention of user and developer.
        /// </summary>
        public void DefineUninitializedIdentifiers(
            Statement stm,
            CallInstruction call)
        {
            var trashedSids = call.Definitions.Select(d => (Identifier) d.Expression)
                .Select(id => ssa.Identifiers[id])
                .Where(sid => sid.DefStatement is null);
            foreach (var sid in trashedSids)
            {
                DefineUninitializedIdentifier(stm, sid);
            }
        }

        private void DefineUninitializedIdentifier(
            Statement stm,
            SsaIdentifier sid)
        {
            var value = InvalidConstant.Create(sid.Identifier.DataType);
            var ass = new Assignment(sid.Identifier, value);
            var newStm = this.InsertStatementAfter(ass, stm);
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

        /// <summary>
        /// Given the SSA identifier <paramref name="sid"/>, replaces its 
        /// definition with the assignment <paramref name="ass"/>.
        /// </summary>
        /// <param name="sid">SSA identifier whose definition is to be
        /// replaced.</param>
        /// <param name="ass">The new definition.</param>
        public void ReplaceAssigment(SsaIdentifier sid, Assignment ass)
        {
            var stm = sid.DefStatement;
            ssa.RemoveUses(stm);
            sid.DefStatement.Instruction = ass;
            ssa.AddUses(stm);
        }
    }
}
