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

namespace Reko.Core.Code
{
	/// <summary>
	/// Determines whether an instruction is critical or not. Non-critical
    /// instructions are candidates for dead code elimination, if the
    /// decompiler can prove they are not used.
	/// </summary>
	public class CriticalInstruction : InstructionVisitorBase
	{
		private bool isCritical;

		public bool IsCritical(Instruction instr)
		{
			isCritical = false;
			instr.Accept(this);
			return isCritical;
		}

		public bool IsCritical(Expression expr)
		{
			expr.Accept(this);
			return isCritical;
		}

		#region InstructionVisitor //////////////////////////////////

		public override void VisitBranch(Branch b)
		{
			isCritical = true;
		}

		public override void VisitCallInstruction(CallInstruction ci)
		{
			isCritical = true;
		}

        public override void VisitComment(CodeComment comment)
        {
            isCritical = true;
        }

        public override void VisitReturnInstruction(ReturnInstruction ret)
		{
			isCritical = true;
		}

		public override void VisitSideEffect(SideEffect side)
		{
			isCritical = true;
		}

		public override void VisitStore(Store store)
		{
			isCritical = true;
		}

		public override void VisitSwitchInstruction(SwitchInstruction si)
		{
			isCritical = true;
		}

		public override void VisitUseInstruction(UseInstruction u)
		{
			isCritical = true;
		}
		#endregion 

		#region ExpressionVisitor /////////////////////////////

		public override void VisitApplication(Application appl)
		{
			isCritical = true;
		}

		public override void VisitMemoryAccess(MemoryAccess access)
		{
			isCritical = IsCritical(access.EffectiveAddress);
		}

		public override void VisitDereference(Dereference deref)
		{
			isCritical = true;
		}
		#endregion
	}
}
