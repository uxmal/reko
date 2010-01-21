/* 
 * Copyright (C) 1999-2010 John Källén.
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

namespace Decompiler.Analysis.Simplification
{
	/// <summary>
	/// Rule that matches (+ id1 id1) and generates (* id1 2)
	/// </summary>
	public class AddTwoIdsRule
	{
		private SsaIdentifierCollection ssaIds;
		private Identifier idLeft;
		private Identifier idRight;

		public AddTwoIdsRule(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		public bool Match(BinaryExpression binExp)
		{
			idLeft = binExp.Left as Identifier;
			if (idLeft == null)
				return false;
			idRight = binExp.Right as Identifier;
			if (idRight == null)
				return false;
			return (idLeft == idRight && binExp.op == Operator.add);
		}

		public Expression Transform(Statement stm)
		{
			ssaIds[idLeft].Uses.Remove(stm);
			return new BinaryExpression(Operator.mul, idLeft.DataType, idLeft, new Constant(idLeft.DataType, 2));
		}
	}
}
