#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Expressions
{
	public class ExpressionFactory
	{
		private Constant trueValue;
		private Constant falseValue;

		public ExpressionFactory()
		{
            this.trueValue = Decompiler.Core.Expressions.Constant.True();
            this.falseValue = Decompiler.Core.Expressions.Constant.False();
        }

		public Identifier Identifier(DataType type, int id, string name, Storage storage) 
		{
			return new Identifier(name, id, type, storage);
		}

		public Constant Const<T>(PrimitiveType type, T value)
		{
			return new Constant<T>(type, value);
		}

		public Constant False
		{
			get { return falseValue; }
		}

		public Constant True
		{
			get { return trueValue; }
		}
	}
}
