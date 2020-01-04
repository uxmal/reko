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
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
	/// <summary>
	/// Looks at member pointer uses, to see if they always are associated
    /// with the same base pointer / segment selector. If so, they can be 
    /// treated as a pointer.
	/// </summary>
	public class SegmentedAccessClassifier : InstructionVisitorBase	
	{
		private readonly SsaState ssa;
		private readonly Dictionary<Identifier,Identifier> assocs;
		private readonly Dictionary<Identifier,Constant> consts;
		private readonly Identifier overAssociatedId = new Identifier("overAssociated", VoidType.Instance, null);
        private readonly Constant overAssociatedConst = Constant.Real64(0.0);

		public SegmentedAccessClassifier(SsaState  ssa)
		{
			this.ssa = ssa;
			assocs = new Dictionary<Identifier,Identifier>();
            consts = new Dictionary<Identifier,Constant>();
		}

		/// <summary>
		/// Associates a base pointer identifier <paramref name="basePtr"/> with an 
        /// offset identifier (think "es" and "bx").
		/// </summary>
		public void Associate(Identifier basePtr, Identifier membPtr)
		{
			if (consts.ContainsKey(basePtr))
			{
                // If basePtr is already associated with a constant,
                // it is over-associated.
				assocs[basePtr] = overAssociatedId;
                consts[basePtr] = overAssociatedConst;
				return;
			}
			
            if (!assocs.TryGetValue(basePtr, out Identifier a))
                assocs[basePtr] = membPtr;
            else if (a != membPtr)
                assocs[basePtr] = overAssociatedId;
            else
                assocs[basePtr] = membPtr;
		}

        /// <summary>
        /// Associates the segment selector <paramref name="basePtr"/> with a constant
        /// offset <paramref name="memberPtr"/>.
        /// </summary>
		public void Associate(Identifier basePtr, Constant memberPtr)
		{
			if (assocs.ContainsKey(basePtr))
			{
				assocs[basePtr] = overAssociatedId;
                consts[basePtr] = overAssociatedConst;
				return;
			}
			consts[basePtr] = memberPtr;
		}

		public Identifier AssociatedIdentifier(Identifier pointer)
		{
            if (assocs.TryGetValue(pointer, out Identifier id))
            {
                return (id != overAssociatedId) ? id : null;
            }
            else
            {
                return null;
            }
		}

        public void Classify()
        {
            foreach (Statement stm in ssa.Procedure.Statements)
            {
                stm.Instruction.Accept(this);
            }
        }

		public bool IsOnlyAssociatedWithConstants(Identifier pointer)
		{
            return (consts.TryGetValue(pointer, out Constant c) && 
                    c != overAssociatedConst);
		}


		#region InstructionVisitorMembers

		public override void VisitSegmentedAccess(SegmentedAccess access)
		{
            if (!(access.BasePointer is Identifier pointer))
                return;
            switch (access.EffectiveAddress)
            {
            case Identifier id:
                Associate(pointer, id);
                return;
            case BinaryExpression bin:
                if (bin.Operator == BinaryOperator.IAdd && bin.Left is Identifier mp)
                {
                    Associate(pointer, mp);
                }
                return;
            case Constant c:
                Associate(pointer, c);
                return;
            }
        }

		#endregion
	}
}
