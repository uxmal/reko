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

using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// Represents an access to a named "register" or "variable". 
	/// </summary>
    /// <remarks>
    /// Identifiers inherit a <see cref="DataType"/> from the <see cref="Expression"/> base
    /// class. They also have a <see cref="Storage"/> property, which identifies
    /// which "memory space" the identifier lives in.
    /// </remarks>
	public class Identifier : Expression
	{
		public Identifier(string name, DataType type, Storage stg) : base(type)
		{
			this.Name = name;
			this.Storage = stg;
		}

        public static Identifier Create(Storage reg)
        {
            return new Identifier(reg.Name, reg.DataType, reg);
        }

        public static Identifier CreateTemporary(string name, DataType dt)
        {
            var tmp = new TemporaryStorage(name, 0, dt);
            return new Identifier(name, dt, tmp);
        }

        public static Identifier Global(string name, DataType dt)
        {
            var globalStorage = new GlobalStorage(name, dt);
            return new Identifier(name, dt, globalStorage);
        }

        public string Name { get; }

        public override IEnumerable<Expression> Children
        {
            get { yield break; }
        }

        /// <summary>
        /// What storage area the identifier refers to.
        /// </summary>
        /// <remarks>
        /// Especially when considering register storage, the size of the 
        /// DataType property of an Identifier is allowed to be less than 
        /// or equal to the storage in which it is stored. This is in
        /// order to accomodate small values being passed in registers on
        /// architectures where there are no subregisters. For example, a 
        /// byte value on a PowerPC would be stored in a 32-bit register 
        /// 'r6'.
        /// </remarks>
        public Storage Storage { get; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitIdentifier(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitIdentifier(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitIdentifier(this);
		}

		public override Expression CloneExpression()
		{
			return this;
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}

		public void Write(bool writeStorage, TextWriter writer)
		{
			WriteType(writeStorage, writer);
			writer.Write(' ');
			writer.Write(Name);
		}

		public void WriteType(bool writeStorage, TextWriter writer)
		{
			if (writeStorage)
			{
				if (Storage is OutArgumentStorage os)
				{
					writer.Write(os.OriginalIdentifier.Storage.Kind);
					writer.Write(" out ");
				}	
				else
				{
					writer.Write(Storage.Kind);
					writer.Write(' ');
				}
			}
			writer.Write(DataType);
		}
    }
}
