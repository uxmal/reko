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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Code
{
    public class DelegatedInstructionVisitor : InstructionVisitor
    {
        private InstructionVisitor d;

        public DelegatedInstructionVisitor(InstructionVisitor d)
        {
            this.d = d;
        }

        #region InstructionVisitor Members

        public void VisitAssignment(Assignment a)
        {
            d.VisitAssignment(a);
        }

        public void VisitBranch(Branch b)
        {
            d.VisitBranch(b);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            d.VisitCallInstruction(ci);
        }

        public void VisitComment(CodeComment comment)
        {
            d.VisitComment(comment);
        }

        public void VisitDeclaration(Declaration decl)
        {
            d.VisitDeclaration(decl);
        }

        public void VisitDefInstruction(DefInstruction def)
        {
            d.VisitDefInstruction(def);
        }

        public void VisitGotoInstruction(GotoInstruction g)
        {
            d.VisitGotoInstruction(g);
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
            d.VisitPhiAssignment(phi);
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            d.VisitReturnInstruction(ret);
        }

        public void VisitSideEffect(SideEffect side)
        {
            d.VisitSideEffect(side);
        }

        public void VisitStore(Store store)
        {
            d.VisitStore(store);
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            d.VisitSwitchInstruction(si);
        }

        public void VisitUseInstruction(UseInstruction u)
        {
            d.VisitUseInstruction(u);
        }

        #endregion
    }
}
