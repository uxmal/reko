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

using System;
using System.IO;
using System.Linq;

namespace Decompiler.Core.Types
{
    /// <summary>
    /// Models a function type. Note the similarity to ProcedureSignature: it's likely we'll want to merge these two.
    /// </summary>
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
            if (argumentTypes == null)
                throw new ArgumentNullException("argumentTypes");
			if (returnType == null)
				returnType = VoidType.Instance;
			this.ReturnType = returnType; 
			this.ArgumentTypes = argumentTypes; 
			this.ArgumentNames = argumentNames;
		}

        public FunctionType(ProcedureSignature sig) : base()
        {
            this.ReturnType = sig.ReturnValue != null
                ? sig.ReturnValue.DataType
                : VoidType.Instance;
            this.ArgumentTypes = sig.Parameters.Select(a => a.DataType).ToArray();
            this.ArgumentNames = sig.Parameters.Select(a => a.Name).ToArray();
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitFunctionType(this);
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
	}
}
