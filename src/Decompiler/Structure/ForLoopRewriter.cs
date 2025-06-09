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
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Structure
{
    /// <summary>
    /// Detects and rewrites loop constructs.
    /// </summary>
    public class ForLoopRewriter
    {
        private readonly Procedure proc;
        private readonly ExpressionValueComparer cmp;

        public ForLoopRewriter(Procedure proc)
        {
            this.proc = proc;
            this.cmp = new ExpressionValueComparer();
        }

        public void Transform()
        {
            RewriteForLoops(proc.Body!);
        }

        /// <summary>
        /// Locate loops in a list of statements and if they are for-loops,
        /// rewrite them as such.
        /// </summary>
        public void RewriteForLoops(List<AbsynStatement> stmts)
        {
            for (int i = 0; i < stmts.Count; ++i)
            {
                ForLoopCandidate? candidate;
                AbsynFor? forLoop;
                switch (stmts[i])
                {
                case AbsynWhile whi:
                    forLoop = TryRewriteWhileLoop(whi, stmts, i);
                    if (forLoop is not null)
                    {
                        stmts[i] = forLoop;
                    }
                    break;
                case AbsynDoWhile dow:
                    RewriteForLoops(dow.Body);
                    candidate = TryMakeLoopCandidate(dow.Condition, dow.Body, stmts, i);
                    if (candidate is not null && LoopsAtLeastOnce(candidate))
                    {
                        stmts[i] = MakeForLoop(candidate, stmts);
                    }
                    break;
                case AbsynIf ifStm:
                    RewriteForLoops(ifStm.Then);
                    RewriteForLoops(ifStm.Else);
                    var whileLoop = TryRewriteGuardedDoWhile(ifStm);
                    if (whileLoop is not null)
                    {
                        forLoop = TryRewriteWhileLoop(whileLoop, stmts, i);
                        stmts[i] = forLoop ?? (AbsynStatement) whileLoop;
                    }
                    break;
                case AbsynSwitch sw:
                    RewriteForLoops(sw.Statements);
                    break;
                default:
                    break;
                }
            }
            stmts.RemoveAll(s => s is null);
        }

        private AbsynFor? TryRewriteWhileLoop(AbsynWhile whi, List<AbsynStatement> stmts, int i)
        {
            RewriteForLoops(whi.Body);
            var candidate = TryMakeLoopCandidate(whi.Condition, whi.Body, stmts, i);
            if (candidate is not null)
            {
                return MakeForLoop(candidate, stmts);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Attempt to undo the if / do-while pattern that many compilers emit when
        /// compiling while loops.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private AbsynWhile? TryRewriteGuardedDoWhile(AbsynIf ifStm)
        {
            if (ifStm.Then.Count == 1 && ifStm.Then[0] is AbsynDoWhile doWhile)
            {
                var ifCond = ifStm.Condition;
                var doCond = doWhile.Condition;
                if (cmp.Equals(ifCond, doCond))
                {
                    // Pattern 1
                    return new AbsynWhile(doCond, doWhile.Body);
                }
                if (ifStm.Condition is not BinaryExpression ifCondBin ||
                    doWhile.Condition is not BinaryExpression doCondBin)
                    return null;

                if (ifCondBin.Operator.Type == OperatorType.Ne && 
                    ifCondBin.Right is Constant ifConst && ifConst.IsZero &&
                    doCondBin.Operator.Type == OperatorType.Gt)
                {
                    if (cmp.Equals(ifCondBin.Left, doCondBin.Left))
                    {
                        // Flip the condition.
                        var condNew = new BinaryExpression(Operator.Lt, PrimitiveType.Bool, doCondBin.Right, doCondBin.Left);
                        return new AbsynWhile(condNew, doWhile.Body);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Attempts to identify the variable controlling a for loop,
        /// and the statement that update the variable within the loop body.
        /// </summary>
        /// <param name="cond">Possible loop control condition</param>
        /// <param name="loopBody">The body of the loop which may contain an update variable</param>
        /// <param name="container">List of statements in which the loop is located.</param>
        /// <param name="i">Position of the candidate loop within the container.</param>
        /// <returns>A for loop candidate if a loop has been recognized, otherwise null.</returns>
        public ForLoopCandidate? TryMakeLoopCandidate(
            Expression cond,
            List<AbsynStatement> loopBody,
            List<AbsynStatement> container, 
            int i)
        {
            if (cond is not BinaryExpression cmp ||
                cmp.Operator is not ConditionalOperator)
                return null;

            var (loopVariable, update) = IdentifyLoopVariable(loopBody, cmp);
            if (loopVariable is null)
                return null;

            var (initializer, deadDecl) = FindInitializer(loopVariable, container, i);
            var candidate = new ForLoopCandidate
            {
                LoopBody = loopBody,
                Container = container,
                LoopVariable = loopVariable,
                Initializer = initializer,
                DeadDeclaration = deadDecl,
                Condition = cmp,
                Update = update,
            };
            return candidate;
        }

        private (Identifier?,AbsynAssignment?) IdentifyLoopVariable(
            List<AbsynStatement> loopBody,
            BinaryExpression cmp)
        {
            List<AbsynAssignment> updates;
            Identifier? loopVariable;
            var idLeft = FindLoopExpressionUse(cmp.Left);
            var idRight = FindLoopExpressionUse(cmp.Right);
            var leftUpdates = FindUpdateAssignments(idLeft, loopBody);
            var rightUpdates = FindUpdateAssignments(idRight, loopBody);
            if (leftUpdates.Count == 1)
            {
                if (rightUpdates.Count == 1)
                    return (null, null);
                loopVariable = idLeft;
                updates = leftUpdates;
            }
            else if (rightUpdates.Count == 1)
            {
                loopVariable = idRight;
                updates = rightUpdates;
            }
            else
                return (null, null);
            return (loopVariable, updates[0]);
        }

        private Identifier? FindLoopExpressionUse(Expression exp)
        {
            if (exp is Identifier id)
                return id;
            if (exp is Cast c && c.Expression is Identifier castId)
                return castId;
            if (exp is Conversion conv && conv.Expression is Identifier convId)
                return convId;
            return null;
        }

        /// <summary>
        /// Attempt to find the statement that initalizes the for loop variable. 
        /// </summary>
        /// <param name="loopVariable">The index variable of the potential loop.</param>
        /// <param name="stmts">The statements in which the loop is located.</param>
        /// <param name="i">The location of the loop within those statements.</param>
        /// <returns>If an initializing statement is found, a pair consisting of a possibly
        /// freshly created initializing statement and a possible old declaration that needs
        /// to be deleted.
        /// </returns>
        private (AbsynAssignment?, AbsynDeclaration?) FindInitializer(Identifier loopVariable, List<AbsynStatement> stmts, int i)
        {
            for (i = i - 1; i >= 0; --i)
            {
                if (stmts[i] is AbsynAssignment ass)
                {
                    if (ass.Dst == loopVariable)
                        return (ass, null);
                    if (UsedIdentifierFinder.Contains(ass.Dst, loopVariable))
                        return (null, null);
                    if (UsedIdentifierFinder.Contains(ass.Src, loopVariable))
                        return (null, null);
                }
                else if (stmts[i] is AbsynDeclaration decl)
                {
                    if (decl.Identifier == loopVariable)
                    {
                        if (decl.Expression is null)
                            return (null, null);
                        var init = new AbsynAssignment(decl.Identifier, decl.Expression);
                        return (init, decl);
                    }
                    else
                    {
                        if (UsedIdentifierFinder.Contains(decl.Expression!, loopVariable))
                            return (null, null);
                    }
                }
                else
                    return (null, null);
            }
            return (null, null);
        }

        private List<AbsynAssignment> FindUpdateAssignments(Identifier? id, List<AbsynStatement> stmts)
        {
            var updates = new List<AbsynAssignment>();
            if (id is null)
                return updates;
            for (int i = stmts.Count - 1; i >= 0; --i)
            {
                var stm = stmts[i];
                if (!(stm is AbsynAssignment ass))
                    return updates;
                if (id != ass.Dst)
                {
                    if (UsedIdentifierFinder.Contains(ass.Src, id) ||
                        UsedIdentifierFinder.Contains(ass.Dst, id))
                    {
                        return updates;
                    }
                    continue;
                }
                if (ass.Src is BinaryExpression bin)
                {
                    if (id == bin.Left)
                        updates.Add(ass);
                }
                else if (ass.Src is FieldAccess fa)
                {
                    if (fa.Structure is Dereference de)
                    {
                        if (de.Expression == id)
                            updates.Add(ass);
                    }
                }
            }
            return updates;
        }

        /// <summary>
        /// Attempt to prove that the condition of the for-loop candidate is 
        /// true at least once. If it isn't, the rewriting of do-while to
        /// for-loop can't be done safely.
        /// </summary>
        private bool LoopsAtLeastOnce(ForLoopCandidate candidate)
        {
            //$REVIEW: enabling this logic makes the majority of the 
            // do-while loops that match the for-loop pattern fail, which
            // is regrettable since we know that the great majority of such
            // loops actually do loop at least once. For now, I'm leaving
            // this in, pending the outcome of https://github.com/uxmal/reko/issues/689
            if (!(candidate.Condition is BinaryExpression cond))
                return false;
            if (!(cond.Right is Constant cRight))
                return false;
            if (candidate.Initializer is null)
                return false;
            if (!(candidate.Initializer.Src is Constant cInit))
                return false;
            return cond.Operator.ApplyConstants(cInit.DataType, cInit, cRight).ToBoolean();
        }

        private AbsynFor MakeForLoop(ForLoopCandidate candidate, List<AbsynStatement> stmts)
        {
            var iInit = candidate.Initializer is null
                ? -1 
                : stmts.IndexOf(candidate.Initializer);
            if (iInit >= 0)
            {
                // Putting in nulls is bad but we will remove them later.
                stmts[iInit] = null!;
            }
            if (candidate.DeadDeclaration is not null)
            {
                candidate.DeadDeclaration.Expression = null;
            }
            candidate.LoopBody!.Remove(candidate.Update!);
            var forLoop = new AbsynFor(
                candidate.Initializer!,
                candidate.Condition!,
                candidate.Update!,
                candidate.LoopBody);
            return forLoop;
        }

        private class UsedIdentifierFinder : ExpressionVisitorBase
        {
            private Identifier? id;
            private bool found;

            public static bool Contains(Expression expr, Identifier id)
            {
                if (expr is null)
                    return false;
                var uif = new UsedIdentifierFinder { id = id };
                expr.Accept(uif);
                return uif.found;
            }

            public override void VisitIdentifier(Identifier id)
            {
                this.found |= (id == this.id);
            }
        }
    }

    public class ForLoopCandidate
    {
        public List<AbsynStatement>? Container;
        public List<AbsynStatement>? LoopBody;
        public Identifier? LoopVariable;
        public AbsynDeclaration? DeadDeclaration;
        public AbsynAssignment? Initializer;
        public Expression? Condition;
        public AbsynAssignment? Update;
    }
}
