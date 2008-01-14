/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using System;
using System.Collections;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Looks at member pointer uses, to see if they always are associated with the same pointer. 
	/// </summary>
	public class SegmentedAccessClassifier : InstructionVisitorBase	
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private Hashtable assocs;
		private Hashtable consts;
		private object overAssociated = new object();

		private int sequencePoint;

		public SegmentedAccessClassifier(Procedure proc, SsaIdentifierCollection ssaIds)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			assocs = new Hashtable();
			consts = new Hashtable();
		}

		/// <summary>
		/// Associates a base pointer identifier with an offset identifier (think "es" and "bx").
		/// </summary>
		/// <param name="basePtr"></param>
		/// <param name="membPtr"></param>
		public void Associate(Identifier basePtr, Identifier membPtr)
		{
			if (consts.Contains(basePtr))
			{
				assocs[basePtr] = overAssociated;
				consts[basePtr] = overAssociated;
				return;
			}
			
			object a = assocs[basePtr];
			if (a == null)
				assocs[basePtr] = membPtr;
			else if (a != membPtr)
				assocs[basePtr] = overAssociated;
			else
				assocs[basePtr] = membPtr;
		}

		public void Associate(Identifier basePtr, Constant memberPtr)
		{
			if (assocs.Contains(basePtr))
			{
				assocs[basePtr] = overAssociated;
				consts[basePtr] = overAssociated;
				return;
			}
			consts[basePtr] = memberPtr;
		}

		public Identifier AssociatedIdentifier(Identifier pointer)
		{
			return assocs[pointer] as Identifier;
		}

		public void Classify()
		{
			sequencePoint = 0;
			foreach (Block b in proc.RpoBlocks)
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
			return (consts[pointer] as Constant) != null;
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
				if (bin.op == BinaryOperator.add && mp != null)
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
