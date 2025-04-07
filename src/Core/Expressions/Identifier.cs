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
	public class Identifier : AbstractExpression
	{
        /// <summary>
        /// Creates an instance of the <see cref="Identifier"/> class.
        /// </summary>
        /// <param name="name">The name of the identifier.</param>
        /// <param name="type">The data type of the identifier.</param>
        /// <param name="stg">The backing <see cref="Storage"/> of the identifier.
        /// </param>
		public Identifier(string name, DataType type, Storage stg) : base(type)
		{
			this.Name = name;
			this.Storage = stg;
		}

        /// <summary>
        /// Create an identifier whose backing <see cref="Reko.Core.Storage"/> is <paramref name="stg"/>.
        /// </summary>
        /// <param name="stg">Backing storage.</param>
        /// <returns>A new <see cref="Identifier"/> with the same name and 
        /// data type as the storage.</returns>
        public static Identifier Create(Storage stg)
        {
            return new Identifier(stg.Name, stg.DataType, stg);
        }

        /// <summary>
        /// Creates a temporary and unique identifier named <paramref name="name"/> and with
        /// the data type <paramref name="dt"/>.
        /// </summary>
        /// <param name="name">Name to give the identifier.</param>
        /// <param name="dt">Date type for the identifier.</param>
        /// <returns>A new identifier.</returns>
        public static Identifier CreateTemporary(string name, DataType dt)
        {
            var tmp = new TemporaryStorage(name, 0, dt);
            return new Identifier(name, dt, tmp);
        }

        /// <summary>
        /// Creates a identifier for a global variable named <paramref name="name" />
        /// and with the data type <paramref name="dt"/>.
        /// </summary>
        /// <param name="name">Name to give the identifier.</param>
        /// <param name="dt">Date type for the identifier.</param>
        /// <returns>A new identifier.</returns>
        public static Identifier Global(string name, DataType dt)
        {
            var globalStorage = new GlobalStorage(name, dt);
            return new Identifier(name, dt, globalStorage);
        }

        /// <summary>
        /// The name of this identifier.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitIdentifier(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitIdentifier(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor v)
		{
			v.VisitIdentifier(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
        {
			return this;
		}

        /// <inheritdoc/>
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
