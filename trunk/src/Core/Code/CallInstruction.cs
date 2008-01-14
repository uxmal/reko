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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Code
{
	public abstract class CallBase : Instruction
	{
		protected Expression expr;
		protected CallSite site;

		public CallBase(Expression expr, int stackDepth, int fpuStackDepth)
		{
			this.expr = expr;
			this.site = new CallSite(stackDepth, fpuStackDepth);
		}

		public CallSite CallSite
		{
			get { return site; }
		}
	}

	public class IndirectCall : CallBase
	{

		public IndirectCall(Expression expr, int stackDepth, int fpuStackDepth) : base(expr, stackDepth, fpuStackDepth)
		{
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformIndirectCall(this);
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

		public override bool IsControlFlow
		{
			get { return false; }
		}
	}

	public class CallInstruction : CallBase
	{
		private Procedure proc;

		public CallInstruction(Procedure proc, int stackDepth, int fpuStackDepth) :
			base(new ProcedureConstant(PrimitiveType.Pointer, proc), stackDepth, fpuStackDepth)
		{	
			this.proc = proc;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformCallInstruction(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitCallInstruction(this);
		}

		public Procedure Callee
		{
			get { return proc; }
			set { proc = value; }
		}

		public override bool IsControlFlow
		{
			get { return false; }
		}
	}
}
