/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Replaces the bits in the specified range with the new expression.
	/// </summary>
	public class DepositBits : Expression
	{
		private Expression src;
		private Expression bits;
		private int bitPos;
		private int bitCount;

		public DepositBits(Expression src, Expression bits, int bitPos, int bitCount) : base(src.DataType)
		{
			this.src = src;
			this.bits = bits;
			this.bitPos = bitPos;
			this.bitCount = bitCount;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformDepositBits(this);
		}

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitDepositBits(this);
		}

		public int BitCount
		{
			get { return bitCount; }
		}

		public int BitPosition
		{
			get { return bitPos; }
		}

		public override Expression CloneExpression()
		{
			return new DepositBits(src.CloneExpression(), bits.CloneExpression(), bitPos, bitCount);
		}


		public Expression Source
		{
			get { return src; }
			set { src = value; }
		}

		public Expression InsertedBits
		{
			get { return bits; }
			set { bits = value; }
		}
	}
}
