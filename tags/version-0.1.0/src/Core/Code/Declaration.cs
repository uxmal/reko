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
	/// Represents a declaration of an identifier, with an optional initial value assignment.
	/// </summary>
	public class Declaration : Instruction
	{
		private Identifier id;
		private Expression init;

		public Declaration(Identifier id, Expression init)
		{
			this.id = id;
			this.init = init;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformDeclaration(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitDeclaration(this);
		}

		public Expression Expression
		{
			get { return init; }
			set { init = value; }
		}

		public override bool IsControlFlow
		{
			get { return false; }
		}

		public Identifier Identifier
		{
			get { return id; }
			set { id = value; }
		}
	}
}
