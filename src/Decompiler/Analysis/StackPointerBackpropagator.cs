#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Operators;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Backpropagate stack pointer from procedure return.
    /// Assume that stack pointer at the end of procedure has the same
    /// value as at the start
    /// </summary>
    /// <example>
    /// If we have
    /// <code>
    ///     call eax; We do not know calling convention of this indirect call
    ///             ; So we do not know value of stack pointer after it
    /// cleanup:
    ///     pop esi
    ///     pop ebp
    ///     ret
    /// </code>
    /// then we could assume than stack pointer at "cleanup" label is
    /// "fp - 8"
    /// </example>
    // $REVIEW: It is highly unlikely that there is a procedure that
    // leaves the stack pointer at different values depending on what
    // path you took through it. Should we encounter such procedures in
    // a binary we might consider turning this analysis off with a user
    // switch.
    public class StackPointerBackpropagator
    {
        private readonly SsaState ssa;
        private readonly ExpressionEmitter m;

        public StackPointerBackpropagator(SsaState ssa)
        {
            this.ssa = ssa;
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// First find use of stack pointer at procedure exit block.
        /// Check if its definition is like 'sp_at_exit = sp_previous + offset'
        /// and 'sp_previous' is trashed (usually after indirect calls). We
        /// assume that stack pointer at the end ('sp_at_exit') is 'fp'. So
        /// 'sp_previous' is 'fp - offset'. So we replace definition of
        /// 'sp_previous' with 'fp - offset'
        /// </summary>
        public void BackpropagateStackPointer()
        {
            foreach (var spAtExit in FindStackUsesAtExit(ssa.Procedure))
            {
                BackpropagateStackPointer(spAtExit);
            }
        }

        private void BackpropagateStackPointer(Identifier spAtExit)
        {
            var spCur = spAtExit;
            var offset = 0;
            for (; ; )
            {
                var (spPrevious, delta) = MatchStackOffsetPattern(spCur);
                if (spPrevious == null)
                    break;
                ReplaceStackDefinition(spCur, spPrevious, offset);
                offset -= delta;
                spCur = spPrevious;
            }
            if (IsTrashed(spCur))
            {
                ReplaceStackDefinition(spCur, null, offset);
            }
        }

        private bool IsTrashed(Identifier sp)
        {
            var definition = ssa.Identifiers[sp].DefStatement;
            switch (definition?.Instruction)
            {
            case CallInstruction _:
            case PhiAssignment _:
                return true;
            default:
                return false;
            }
        }

        private (Identifier, int) MatchStackOffsetPattern(Identifier sp)
        {
            (Identifier, int) noMatch = (null, 0);
            var sid = ssa.Identifiers[sp];
            var def = sid.DefStatement;
            if (!(def.Instruction is Assignment ass))
                return noMatch;
            if (!(ass.Src is BinaryExpression bin))
                return noMatch;
            if ((bin.Operator != Operator.IAdd &&
                bin.Operator != Operator.ISub))
                return noMatch;
            if (!(bin.Left is Identifier id))
                return noMatch;
            if (id.Storage != sp.Storage)
                return noMatch;
            if (!(bin.Right is Constant c))
                return noMatch;
            var offset = c.ToInt32();
            if (bin.Operator == Operator.ISub)
                offset = -offset;
            return (id, offset);
        }

        /// <summary>
        /// Replace definition of '<paramref name="sp"/>' with
        /// 'fp - <paramref name="frameOffset"/>'
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="frameOffset"></param>
        private void ReplaceStackDefinition(Identifier sp, Identifier spPrev, int frameOffset)
        {
            var spDef = ssa.Identifiers[sp].DefStatement;
            if (spDef.Instruction is Assignment ass)
            {
                var fp = ssa.Procedure.Frame.FramePointer;
                ssa.Identifiers[spPrev].Uses.Remove(spDef);
                ssa.Identifiers[fp].Uses.Add(spDef);
                ass.Src = m.AddSubSignedInt(fp, frameOffset);
            }
            else
            {
	           // insert new stack definition
	            InsertStackDefinition(sp, frameOffset, spDef);
                // Remove old stack definition
                RemoveDefinition(sp, spDef);
            }
        }

        private void InsertStackDefinition(
            Identifier stack,
            int frameOffset,
            Statement stmAfter)
        {
            var fp = ssa.Procedure.Frame.FramePointer;
            var pos = stmAfter.Block.Statements.IndexOf(stmAfter);
            var src = m.AddSubSignedInt(fp, frameOffset);
            var newStm = stmAfter.Block.Statements.Insert(
                pos + 1,
                stmAfter.LinearAddress,
                new Assignment(stack, src));
            ssa.Identifiers[stack].DefStatement = newStm;
            ssa.AddUses(newStm);
        }

        private void RemoveDefinition(Identifier id, Statement defStatement)
        {
            switch (defStatement.Instruction)
            {
            case CallInstruction ci:
                ci.Definitions.RemoveWhere(cb => cb.Expression == id);
                break;
            case PhiAssignment phi:
                ssa.DeleteStatement(defStatement);
                break;
            }
        }

        private Identifier FindStackUseAtExit(Procedure proc)
        {
            return proc.ExitBlock.Statements
                .Select(s => s.Instruction)
                .OfType<UseInstruction>()
                .Select(u => u.Expression)
                .OfType<Identifier>()
                .Where(id => id.Storage == proc.Architecture.StackRegister)
                .SingleOrDefault();
        }

        private IEnumerable<Identifier> FindStackUsesAtExit(Procedure proc)
        {
            var sp = FindStackUseAtExit(proc);
            if (sp == null)
                return new Identifier[] { };
            var def = ssa.Identifiers[sp].DefStatement;
            if (def?.Instruction is PhiAssignment phi)
                return phi.Src.Arguments
                    .Select(de => de.Value)
                    .OfType<Identifier>().Distinct();
            return new Identifier[] { sp };
        }
    }
}
