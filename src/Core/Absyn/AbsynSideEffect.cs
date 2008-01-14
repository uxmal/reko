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

using Decompiler.Core.Code;
using System;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Models a statement that isn't an assignment, typically a function that does IO.
	/// </summary>
	public class AbsynSideEffect : AbsynStatement
	{
		private Expression side;

		public AbsynSideEffect(Expression expr)
		{
			side = expr;
		}

		public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitSideEffect(this);
		}

		public Expression Expression
		{
			get { return side; }
		}
	}
}
