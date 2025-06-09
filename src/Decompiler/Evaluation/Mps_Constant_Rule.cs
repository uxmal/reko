#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Operators;
using Reko.Core;
using System;

namespace Reko.Evaluation
{
	public class Mps_Constant_Rule
	{
		private MemberPointerSelector? mps;

		public Mps_Constant_Rule()
		{
		}

		public bool Match(MemberPointerSelector mps, EvaluationContext ctx)
		{
//			c = mps.MemberPtr as Constant;
			this.mps = mps;
			return false;
			//return (c is not null); 				//$REVIEW: disabled. Perhaps we don't want to do this transformation before we detect registerpairs.
		}

		public Expression Transform()
		{
			return new BinaryExpression(
				BinaryOperator.IAdd,
				mps!.DataType,
				null!, // mps.Ptr,
				null!); // c
		}
	}
}
