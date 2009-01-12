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
using System.IO;

namespace Decompiler.Core.Types
{
	public class FunctionType : DataType
	{
		public DataType ReturnType;
		public DataType [] ArgumentTypes;
		public string [] ArgumentNames;

		public FunctionType(
			string name,
			DataType returnType,
			DataType [] argumentTypes,
			string [] argumentNames) :
			base(name)
		{
			if (returnType == null)
				returnType = PrimitiveType.Void;
			this.ReturnType = returnType; 
			this.ArgumentTypes = argumentTypes; 
			this.ArgumentNames = argumentNames;
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformFunctionType(this);
		}

		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitFunctionType(this);
		}

		public override DataType Clone()
		{
			DataType ret = (ReturnType != null) ? ReturnType.Clone() : null;
			DataType [] types = new DataType[ArgumentTypes.Length];
			string []   names = new string[ArgumentTypes.Length];
			for (int i = 0; i < types.Length; ++i)
			{
				types[i] = ArgumentTypes[i].Clone();
				if (ArgumentNames != null)
					names[i] = ArgumentNames[i];
			}
			return new FunctionType(Name, ret, types, names);
		}

		public override int Size
		{
			get { return 0; }
			set { ThrowBadSize(); }
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("(fn ");
			if (ReturnType != null)
				ReturnType.Write(writer);
			else 
				writer.Write("void");
			writer.Write(" (");

			string separator = "";
			for (int i = 0; i < ArgumentTypes.Length; ++i)
			{
				writer.Write(separator);
				separator = ", ";
				ArgumentTypes[i].Write(writer);
			}
			writer.Write("))");
		}

	}
}
