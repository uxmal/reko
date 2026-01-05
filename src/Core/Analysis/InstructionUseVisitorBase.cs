#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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

using Reko.Core.Code;
using Reko.Core.Expressions;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// Base implementation class used for analyses interested in the usage of 
    /// <see cref="Identifier"/>s.
    /// </summary>
    public abstract class InstructionUseVisitorBase : InstructionVisitorBase
    {
        #region InstructionVisitor Members

        /// <inheritdoc/>
        public override void VisitAssignment(Assignment a)
        {
            a.Src.Accept(this);
        }

        /// <inheritdoc/>
        public override void VisitCallInstruction(CallInstruction ci)
        {
            ci.Callee.Accept(this);
            foreach (var use in ci.Uses)
                use.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public override void VisitDefInstruction(DefInstruction def)
        {
        }

        /// <inheritdoc/>
        public override void VisitStore(Store store)
        {
            store.Src.Accept(this);

            // Do not count assignments to out identifiers as uses.
            if (store.Dst is Identifier)
                return;
            // Do not add memory identifier to uses
            if (store.Dst is MemoryAccess access)
            {
                access.EffectiveAddress.Accept(this);
                return;
            }
            store.Dst.Accept(this);
        }

        /// <inheritdoc/>
        public override void VisitPhiAssignment(PhiAssignment phi)

        {
            phi.Src.Accept(this);
        }

        #endregion

        #region IExpressionVisitor Members

        /// <inheritdoc/>
        public override void VisitIdentifier(Identifier id)
        {
            UseIdentifier(id);
        }

        /// <inheritdoc/>
        public override void VisitOutArgument(OutArgument outArg)
        {
            if (outArg.Expression is Identifier)
                return;
            outArg.Expression.Accept(this);
        }

        #endregion

        /// <summary>
        /// Called when an <see cref="Identifier"/> is used in an instruction.
        /// </summary>
        protected abstract void UseIdentifier(Identifier id);
    }
}
