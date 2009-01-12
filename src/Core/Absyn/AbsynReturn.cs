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

using Decompiler.Core.Code;
using System;

namespace Decompiler.Core.Absyn
{
	public class AbsynReturn : AbsynStatement
	{
		private Expression retval;

		public AbsynReturn(Expression retval)
		{
			this.retval = retval;
		}

		public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitReturn(this);
		}

		public Expression Value
		{
			get { return retval; }
		}
	}
}
