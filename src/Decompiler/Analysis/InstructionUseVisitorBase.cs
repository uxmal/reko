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

namespace Reko.Analysis
{
    public abstract class InstructionUseVisitorBase : InstructionVisitorBase
    {
        #region InstructionVisitor Members

        public override void VisitAssignment(Assignment a)
        {
            a.Src.Accept(this);
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            ci.Callee.Accept(this);
            foreach (var use in ci.Uses)
                use.Expression.Accept(this);
        }

        public override void VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
                decl.Expression.Accept(this);
        }

        public override void VisitDefInstruction(DefInstruction def)
        {
        }

        public override void VisitStore(Store store)
        {
            store.Src.Accept(this);

            // Do not count assignments to out identifiers as uses.
            if (store.Dst is Identifier idOut && idOut.Storage is OutArgumentStorage)
                return;
            // Do not add memory identifier to uses
            if (store.Dst is MemoryAccess access)
            {
                if (access is SegmentedAccess sa)
                    sa.BasePointer.Accept(this);
                access.EffectiveAddress.Accept(this);
                return;
            }
            store.Dst.Accept(this);
        }

        public override void VisitPhiAssignment(PhiAssignment phi)

        {
            phi.Src.Accept(this);
        }

        #endregion

        #region IExpressionVisitor Members

        public override void VisitIdentifier(Identifier id)
        {
            UseIdentifier(id);
        }

        public override void VisitOutArgument(OutArgument outArg)
        {
        }

        #endregion

        protected abstract void UseIdentifier(Identifier id);
    }
}
