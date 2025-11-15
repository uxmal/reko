#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using Reko.Core.Operators;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// Backpropagate stack pointer from procedure return.
/// Assumes that stack pointer at the end of procedure has the same
/// value as at the start.
/// <para>
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
/// </para>
/// </summary>
// $REVIEW: It is highly unlikely that there is a procedure that
// leaves the stack pointer at different values depending on what
// path you took through it. Should we encounter such procedures in
// a binary we might consider turning this analysis off with a user
// switch.
public class StackPointerBackpropagator : IAnalysis<SsaState>
{
    private readonly AnalysisContext context;

    /// <summary>
    /// Constructs a new instance of the <see cref="StackPointerBackpropagator"/> class.
    /// </summary>
    /// <param name="context"><see cref="AnalysisContext"/> for this analysis.
    /// </param>
    public StackPointerBackpropagator(AnalysisContext context)
    {
        this.context = context;
    }


    /// <inheritdoc/>
    public string Id => "spbp";

    /// <inheritdoc/>
    public string Description => "Propagates the stack pointer backwards from the exit block";

    /// <inheritdoc/>
    public (SsaState, bool) Transform(SsaState ssa)
    {
        var w = new Worker(ssa, context.EventListener);
        w.BackpropagateStackPointer();
        return (ssa, true); //$TODO: compute whether change occured. 
    }

    private class Worker
    {
        private readonly SsaState ssa;
        private readonly ExpressionEmitter m;
        private readonly IEventListener listener;

        public Worker(SsaState ssa, IEventListener listener)
        {
            this.ssa = ssa;
            this.m = new ExpressionEmitter();
            this.listener = listener;
        }

        /// <summary>
        /// First find use of stack pointer at procedure exit block.
        /// Check if its definition is like <c>sp_at_exit = sp_previous + offset</c>
        /// and <c>sp_previous</c> is trashed (usually after indirect calls). We
        /// assume that stack pointer at the end (<c>sp_at_exit</c>) is <c>fp</c>. So
        /// <c>sp_previous</c> is <c>fp - offset</c>. So we replace definition of
        /// <c>sp_previous</c> with <c>fp - offset</c>.
        /// </summary>
        public void BackpropagateStackPointer()
        {
            foreach (var spAtExit in FindStackUsesAtExit(ssa.Procedure))
            {
                if (listener.IsCanceled())
                    return;
                BackpropagateStackPointer(spAtExit);
            }
        }

        private void BackpropagateStackPointer(Identifier spAtExit)
        {
            var spCur = spAtExit;
            var offset = 0;
            for (; ; )
            {
                if (listener.IsCanceled())
                    return;
                var (spPrevious, delta) = MatchStackOffsetPattern(spCur);
                if (spPrevious is null)
                    break;
                ReplaceStackDefinition(spCur, spPrevious, offset);
                offset -= delta;
                spCur = spPrevious;
            }
            if (IsTrashed(spCur))
            {
                ReplaceStackDefinition(spCur, null!, offset);   //$BUG: should not be null!
            }
        }

        private bool IsTrashed(Identifier sp)
        {
            var definition = ssa.Identifiers[sp].DefStatement;
            return (definition?.Instruction) switch
            {
                CallInstruction _ or PhiAssignment _ => true,
                _ => false,
            };
        }

        private (Identifier?, int) MatchStackOffsetPattern(Identifier sp)
        {
            (Identifier?, int) noMatch = (null, 0);
            var sid = ssa.Identifiers[sp];
            var def = sid.DefStatement;
            if (def is null || def.Instruction is not Assignment ass)
                return noMatch;
            if (ass.Src is not BinaryExpression bin)
                return noMatch;
            if (!bin.Operator.Type.IsAddOrSub())
                return noMatch;
            if (bin.Left is not Identifier id)
                return noMatch;
            if (id.Storage != sp.Storage)
                return noMatch;
            if (bin.Right is not Constant c)
                return noMatch;
            var offset = c.ToInt32();
            if (bin.Operator.Type == OperatorType.ISub)
                offset = -offset;
            return (id, offset);
        }

        /// <summary>
        /// Replace definition of <c><paramref name="sp"/></c> with
        /// <c>fp - <paramref name="frameOffset"/></c>.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="spPrev"></param>
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
                stmAfter.Address,
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

        /// <summary>
        /// Finds a use of the architecture stack pointer in the synthetic exit
        /// block of the procedure
        /// </summary>
        private static Identifier? FindStackUseAtExit(Procedure proc)
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
            if (sp is null)
                return Array.Empty<Identifier>();
            var def = ssa.Identifiers[sp].DefStatement;
            if (def?.Instruction is PhiAssignment phi)
                return phi.Src.Arguments
                    .Select(de => de.Value)
                    .OfType<Identifier>().Distinct();
            return new Identifier[] { sp };
        }
    }
}
