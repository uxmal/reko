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
	/// Common Representation of loops.
	/// </summary>
	public abstract class AbsynLoop : AbsynStatement
	{
		private Expression condition;
		private AbsynStatement body;

		protected AbsynLoop(Expression condition, AbsynStatement body)
		{
			this.condition = condition;
			this.body = body;
		}

		public AbsynStatement Body
		{
			get { return body; }
			set { body = value; }
		}

		public Expression Condition
		{
			get { return condition; }
		}

	}
}
