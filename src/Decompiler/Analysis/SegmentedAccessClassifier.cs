#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
	/// Looks at member pointer uses, to see if they always are associated with the same base pointer / segment.
    /// If so, they can be treated as a pointer.
	/// </summary>
	public class SegmentedAccessClassifier : InstructionVisitorBase	
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private Dictionary<Identifier,Identifier> assocs;
		private Dictionary<Identifier,Constant> consts;
		private Identifier overAssociatedId = new Identifier("overAssociated", VoidType.Instance, null);
        private Constant overAssociatedConst = Constant.Real64(0.0);

		private int sequencePoint;

		public SegmentedAccessClassifier(Procedure proc, SsaIdentifierCollection ssaIds)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			assocs = new Dictionary<Identifier,Identifier>();
            consts = new Dictionary<Identifier, Constant>();
		}

		/// <summary>
		/// Associates a base pointer identifier with an offset identifier (think "es" and "bx").
		/// </summary>
		/// <param name="basePtr"></param>
		/// <param name="membPtr"></param>
		public void Associate(Identifier basePtr, Identifier membPtr)
		{
			if (consts.ContainsKey(basePtr))
			{
				assocs[basePtr] = overAssociatedId;
                consts[basePtr] = overAssociatedConst;
				return;
			}
			
			Identifier a;
            if (!assocs.TryGetValue(basePtr, out a))
                assocs[basePtr] = membPtr;
            else if (a != membPtr)
                assocs[basePtr] = overAssociatedId;
            else
                assocs[basePtr] = membPtr;
		}

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
            Identifier id;
            if (assocs.TryGetValue(pointer, out id))
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
			sequencePoint = 0;
			foreach (Block b in proc.ControlGraph.Blocks)
			{
				foreach (Statement stm in b.Statements)
				{
					stm.Instruction.Accept(this);
				}
				++sequencePoint;
			}
		}

		public bool IsOnlyAssociatedWithConstants(Identifier pointer)
		{
            Constant c;
            return (consts.TryGetValue(pointer, out c) &&  c != overAssociatedConst);
		}


		#region InstructionVisitorMembers

		public override void VisitSegmentedAccess(SegmentedAccess access)
		{
			Identifier pointer = access.BasePointer as Identifier;
			if (pointer == null)
				return;
			BinaryExpression bin = access.EffectiveAddress as BinaryExpression;
			if (bin != null)
			{
				Identifier mp = bin.Left as Identifier;
				if (bin.Operator == BinaryOperator.IAdd && mp != null)
				{
					Associate(pointer, mp);
				}
				return;
			}
			Constant c = access.EffectiveAddress as Constant;
			if (c != null)
			{
				Associate(pointer, c);
				return;
			}
		}

		#endregion
	}
}
