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

using Decompiler.Core;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Represents an access to a named "register" or "variable".
	/// </summary>
	public class Identifier : Expression
	{
		private int idx;
		private string name;
		private Storage storage;

		public Identifier(string name, int number, DataType type, Storage stg) : base(type)
		{
			this.name = name;
			this.idx = number;
			this.storage = stg;
		}
		 
		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformIdentifier(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitIdentifier(this);
		}

		public override Expression CloneExpression()
		{
			return this;
		}

		public override bool Equals(object o)
		{
			Identifier id = o as Identifier;
			return id != null && idx == id.idx;
		}

		public override int GetHashCode()
		{
			return idx.GetHashCode();
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.not, PrimitiveType.Bool, this);
		}

		public string Name
		{
			get { return name; } 
		}

		public int Number
		{
			get { return idx; }
		}

		public Storage Storage
		{
			get { return storage; }
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
				OutArgumentStorage os = storage as OutArgumentStorage;
				if (os != null)
				{
					writer.Write(os.OriginalIdentifier.Storage.Kind);
					writer.Write(" out ");
				}	
				else
				{
					writer.Write(storage.Kind);
					writer.Write(' ');
				}
			}
			writer.Write(DataType);
		}
	}

	/// <summary>
	/// A special class that represents memory locations. Initially,
	/// all memory accesses can be considered to be made from one global
	/// variable, MEM0. Later, SSA analysis will break apart MEM references 
	/// after each store operation, giving rise to MEM1, MEM2 &c. 
	/// If ambitious, memory alias analysis can be done. In this case,
	/// we will have several MEMx variables before SSA, each MEMx variable
	/// will be an alias class. 
	/// </summary>
	public class MemoryIdentifier : Identifier
	{
		private static MemoryIdentifier g;

		public MemoryIdentifier(int i, DataType dt) : base("Mem" + i, i, dt, new MemoryStorage())
		{
		}


		static MemoryIdentifier()
		{
			g = new MemoryIdentifier(0, new UnknownType());
		}

		public static MemoryIdentifier GlobalMemory
		{
			get { return g; }
		}
	}
}
