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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Structure
{
    /// <summary>
    /// Removes redundant 'return' statements in procedures that return
    /// void. These return statements will always be in tail position.
    /// </summary>
    public class TailReturnRemover : IAbsynVisitor<bool>
    {
        private Procedure proc;

        public TailReturnRemover(Procedure proc)
        {
            this.proc = proc;
        }

        public void Transform()
        {
            if (!proc.Signature.HasVoidReturn)
                return;
            var stmts = proc.Body;
            RemoveRedundantReturn(stmts!);
        }

        public bool VisitAssignment(AbsynAssignment ass)
        {
            return false;
        }

        public bool VisitBreak(AbsynBreak brk)
        {
            return false;
        }

        public bool VisitCase(AbsynCase absynCase)
        {
            return false;
        }

        public bool VisitCompoundAssignment(AbsynCompoundAssignment compound)
        {
            return false;
        }

        public bool VisitContinue(AbsynContinue cont)
        {
            return false;
        }

        public bool VisitDeclaration(AbsynDeclaration decl)
        {
            return false;
        }

        public bool VisitDefault(AbsynDefault decl)
        {
            return false;
        }

        public bool VisitDoWhile(AbsynDoWhile loop)
        {
            return false;
        }

        public bool VisitFor(AbsynFor forLoop)
        {
            return false;
        }

        public bool VisitGoto(AbsynGoto gotoStm)
        {
            return false;
        }

        public bool VisitIf(AbsynIf ifStm)
        {
            RemoveRedundantReturn(ifStm.Then);
            RemoveRedundantReturn(ifStm.Else);
            return false;
        }

        public bool VisitLabel(AbsynLabel lbl)
        {
            return false;
        }

        public bool VisitLineComment(AbsynLineComment comment)
        {
            return false;
        }

        public bool VisitReturn(AbsynReturn ret)
        {
            // Only remove returns that don't return anything.
            return ret.Value is null;
        }

        public bool VisitSideEffect(AbsynSideEffect side)
        {
            return false;
        }

        public bool VisitSwitch(AbsynSwitch absynSwitch)
        {
            return false;
        }

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
