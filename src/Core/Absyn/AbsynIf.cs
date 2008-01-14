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
	/// Abstract syntax for a traditional 'if' statement.
	/// </summary>
	public class AbsynIf : AbsynStatement
	{
		public Expression Condition;
		public AbsynStatement Then;
		public AbsynStatement Else;

		public AbsynIf(Expression e, AbsynStatement then)
		{
			this.Condition = e;
			this.Then = then;
			this.Else = null;
		}

		public AbsynIf(Expression e, AbsynStatement then, AbsynStatement els)
		{
			this.Condition = e;
			this.Then = then;
			this.Else = els;
		}

		public override void Accept(IAbsynVisitor v)
		{
			v.VisitIf(this);
		}
	}
}
