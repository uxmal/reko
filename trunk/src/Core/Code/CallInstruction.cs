#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Code
{
	public abstract class CallBase : Instruction
	{
		protected Expression expr;

		public CallBase(Expression expr, CallSite site)
		{
			this.expr = expr;
            this.CallSite = site;
		}

		public CallSite CallSite { get; private set; }

        public override bool IsControlFlow
        {
            get { return false; }
        }
	}

    //$TODO: the distinction between indirect call and call is just the type of the target expression. We should attempt to get rid of the two subclasses.
	public class IndirectCall : CallBase
	{
        public IndirectCall(Expression expr, CallSite site) : base(expr, site)
        {
        }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformIndirectCall(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitIndirectCall(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitIndirectCall(this);
		}

		public Expression Callee
		{
			get { return expr; }
			set { expr = value; }
		}
	}

	public class CallInstruction : CallBase
	{
        public CallInstruction(ProcedureConstant pc, CallSite site) : base(pc, site)
        {
            this.Callee = pc != null ? pc.Procedure : null;
        }

        public ProcedureBase Callee { get; set; }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformCallInstruction(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitCallInstruction(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitCallInstruction(this);
		}
	}
}
