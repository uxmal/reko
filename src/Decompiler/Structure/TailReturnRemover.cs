#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;

namespace Reko.Structure
{
    /// <summary>
    /// Removes redundant 'return' statements in procedures that return
    /// void. These return statements will always be in tail position.
    /// </summary>
    public class TailReturnRemover : IAbsynVisitor<bool>
    {
        private Procedure proc;

        /// <summary>
        /// Constructs a new instance of the <see cref="TailReturnRemover"/> class.
        /// </summary>
        /// <param name="proc">Procedore to be transformed.</param>
        public TailReturnRemover(Procedure proc)
        {
            this.proc = proc;
        }

        /// <summary>
        /// Transforms the procedure by removing redundant 'return' statements
        /// </summary>
        public void Transform()
        {
            if (!proc.Signature.HasVoidReturn)
                return;
            var stmts = proc.Body;
            RemoveRedundantReturn(stmts!);
        }

        /// <inheritdoc/>
        public bool VisitAssignment(AbsynAssignment ass)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitBreak(AbsynBreak brk)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitCase(AbsynCase absynCase)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitCompoundAssignment(AbsynCompoundAssignment compound)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitContinue(AbsynContinue cont)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitDeclaration(AbsynDeclaration decl)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitDefault(AbsynDefault decl)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitDoWhile(AbsynDoWhile loop)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitFor(AbsynFor forLoop)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitGoto(AbsynGoto gotoStm)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitIf(AbsynIf ifStm)
        {
            RemoveRedundantReturn(ifStm.Then);
            RemoveRedundantReturn(ifStm.Else);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitLabel(AbsynLabel lbl)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitLineComment(AbsynLineComment comment)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitReturn(AbsynReturn ret)
        {
            // Only remove returns that don't return anything.
            return ret.Value is null;
        }

        /// <inheritdoc/>
        public bool VisitSideEffect(AbsynSideEffect side)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitSwitch(AbsynSwitch absynSwitch)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitWhile(AbsynWhile loop)
        {
            return false;
        }

        private void RemoveRedundantReturn(List<AbsynStatement> stmts)
        {
            while (stmts.Count > 0)
            {
                int i = stmts.Count - 1;
                if (!stmts[i].Accept(this))
                    return;
                stmts.RemoveAt(i);
            }
        }
    }
}
