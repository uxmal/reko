#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using System;
using System.IO;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Adapter that holds an Instruction.
	/// </summary>
	public class AbsynAssignment : AbsynStatement
	{
		public AbsynAssignment(Expression dst, Expression src)
		{
			this.Dst = dst;
			this.Src = src;
		}

		public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitAssignment(this);
		}

		public Expression Dst  {get; private set; }
		public Expression Src { get; private set; }
	}
}
