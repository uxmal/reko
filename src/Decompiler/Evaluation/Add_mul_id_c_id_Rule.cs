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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Analysis;
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// Rule that matches (+ (* id c) id) and yields (* id (+ c 1))
	/// </summary>
	public class Add_mul_id_c_id_Rule
	{
		private EvaluationContext ctx;
		private BinaryExpression bin;
		private Identifier id;
		private Constant cInner;

		public Add_mul_id_c_id_Rule(EvaluationContext ctx)
		{
			this.ctx = ctx;
		}

		public bool Match(BinaryExpression exp)
		{
			if (exp.Operator != Operator.IAdd)
				return false;
			id = exp.Left as Identifier;
			
			bin = exp.Right as BinaryExpression;
			if ((id == null || bin == null) && exp.Operator  == Operator.IAdd)
			{
				id = exp.Right as Identifier;
				bin = exp.Left as BinaryExpression;
			}
			if (id == null || bin == null)
				return false;

			if (bin.Operator != Operator.SMul && bin.Operator != Operator.UMul && bin.Operator != Operator.IMul)
				return false;

			Identifier idInner = bin.Left as Identifier;
			cInner = bin.Right as Constant;
			if (idInner == null ||cInner == null)
				return false;

			if (idInner != id)
				return false;

			return true;
		}

		public Expression Transform()
		{
            ctx.RemoveIdentifierUse(id);
			return new BinaryExpression(bin.Operator, id.DataType, id, Operator.IAdd.ApplyConstants(cInner, Constant.Create(cInner.DataType, 1)));
		}
	}
}
