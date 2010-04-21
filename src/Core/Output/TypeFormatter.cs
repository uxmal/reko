/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Core.Output
{
	/// <summary>
	/// Formats types using indentation settings specified by caller.
	/// </summary>
	public class TypeFormatter : IDataTypeVisitor
	{
        private Formatter writer;

		private string name;
		public int indentLevel;
		private Dictionary<DataType,object> visited;
		private Mode mode;
        private bool typeReference;

		private readonly object Declared = 1;
		private readonly object Defined = 2;

		private enum Mode { Writing, Scanning }

		public TypeFormatter(Formatter writer, bool typeReference)
		{
            this.writer = writer;
			this.visited = new Dictionary<DataType,object>();
			this.mode = Mode.Writing;
            this.typeReference = typeReference;
		}

		public void BeginLine()
		{
			BeginLine("");
		}

		public void BeginLine(string s)
		{
			for (int i = 0; i < indentLevel; ++i)
			{
				writer.Write("\t");
			}
			writer.Write(s);
		}


		public void EndLine()
		{
			EndLine("", null);
		}

		public void EndLine(string terminator)
		{
			EndLine(terminator, null);
		}

		public void EndLine(string terminator, string comment)
		{
			writer.Write(terminator);
			LineEndComment(comment);
			writer.WriteLine();
		}

		public void LineEndComment(string comment)
		{
			if (comment != null)
			{
				writer.Write("\t// ");
				writer.Write(comment);
			}
		}

		public void OpenBrace()
		{
			OpenBrace(null);
		}

		public void OpenBrace(string trailingComment)
		{
			EndLine(" {", trailingComment);
			++indentLevel;
		}

		public void CloseBrace()
		{
			--indentLevel;
			BeginLine();
			writer.Write("}");
		}

		#region IDataTypeVisitor methods ///////////////////////////////////////

		public void VisitArray(ArrayType at)
		{
			string oldName = name;
			name = null;
			at.ElementType.Accept(this);
			name = oldName;
			WriteName(true);
			name = null;
			writer.Write("[");
			if (at.Length != 0)
			{
				writer.Write(at.Length.ToString());
			}
			writer.Write("]");
		}

		public void VisitEquivalenceClass(EquivalenceClass eq)
		{
            if (eq.DataType == null)
            {
                writer.Write("ERROR: EQ_{0}.DataType is Null", eq.Number);      //$DEBUG
            }
            else
            {
                writer.Write(eq.DataType.Name);
            }
            WriteName(true);
		}

		public void VisitFunctionType(FunctionType ft)
		{
			string oldName = name;
			name = null;
			ft.ReturnType.Accept(this);
			writer.Write(" (");
			name = oldName;
			WriteName(false);
			writer.Write(")(");
			if (ft.ArgumentTypes.Length > 0)
			{
				name = ft.ArgumentNames != null ? ft.ArgumentNames[0] : null;
				ft.ArgumentTypes[0].Accept(this);
				
				for (int i = 1; i < ft.ArgumentTypes.Length; ++i)
				{
					writer.Write(", ");
					name = ft.ArgumentNames != null ? ft.ArgumentNames[i] : null;
					ft.ArgumentTypes[i].Accept(this);
				}
				name = oldName;
			}

			writer.Write(")");
		}

		public void VisitStructure(StructureType str)
		{
			string n = name;
			if (mode == Mode.Writing)
			{
                object v;
                if (visited.TryGetValue(str, out v) && (v == Defined || v == Declared))
				{
                    writer.WriteKeyword("struct");
                    writer.Write(" ");
                    writer.Write(str.Name);
				}
				else if (v != Declared)
				{
					visited[str] = Declared;
					ScanFields(str);
					writer.Write("struct {0}", str.Name);
					OpenBrace(str.Size > 0 ? string.Format("size: {0} {0:X}", str.Size) : null);
					if (str.Fields != null)
					{
						foreach (StructureField f in str.Fields)
						{
							BeginLine();
							name = f.Name;
							f.DataType.Accept(this);
							EndLine(";", string.Format("{0:X}", f.Offset));
						}
					}
					CloseBrace();
					visited[str] = Defined;
				}

				name = n;
				WriteName(true);
			}
			else
			{
				if (!visited.ContainsKey(str))
				{
					visited[str] = Declared;
					writer.Write("struct ");
                    writer.Write(str.Name);
                    writer.Write(";");
					writer.WriteLine();
				}
			}
		}

		public void ScanFields(StructureType str)
		{
			Mode m = mode;
			mode = Mode.Scanning;

			foreach (StructureField f in str.Fields)
			{
				f.DataType.Accept(this);
			}
			mode = m;
		}

		public void VisitMemberPointer(MemberPointer memptr)
		{
			Pointer p = memptr.BasePointer as Pointer;
			DataType baseType;
			if (p != null)
			{
				baseType = p.Pointee;
			}
			else
			{
				baseType = memptr.BasePointer;
			}

			string oldName = name;
			name = null;
			memptr.Pointee.Accept(this);
            if (mode == Mode.Writing)
            {
                writer.Write(" ");
                writer.Write(baseType.Name);
                writer.Write("::*");
            }
			name = oldName;
            if (mode == Mode.Writing)
            {
                WriteName(false);
            }
		}

		public void VisitPointer(Pointer pt)
		{
			if (mode == Mode.Writing)
			{
				if (name == null)
					name = "*";
				else 
					name = "* " + name;
			}
			pt.Pointee.Accept(this);
		}

		public void VisitPrimitive(PrimitiveType pt)
		{
			if (mode == Mode.Writing)
			{
                writer.Write(pt.ToString());
				WriteName(true);
			}
		}

		public void VisitTypeVar(TypeVariable t)
		{
			throw new NotImplementedException("TypeFormatter.TypeVariable");
		}

		public void VisitUnion(UnionType ut)
		{
			string n = name;

			writer.WriteKeyword("union");
            writer.Write(" ");
            writer.Write(ut.Name);
			OpenBrace();
			int i = 0;
			foreach (UnionAlternative alt in ut.Alternatives.Values)
			{
				BeginLine();
				name = alt.MakeName(i);
				alt.DataType.Accept(this);
				EndLine(";");
				++i;
			}
			CloseBrace();

			name = n;
			WriteName(true);
		}

		public void VisitUnknownType(UnknownType ut)
		{
			if (mode == Mode.Writing)
			{
				writer.Write("void");
			}
		}

		#endregion

		public void Write(DataType dt, string name)
		{
			this.name = name;
			dt.Accept(this);
		}

		public void WriteTypes(ICollection<DataType> datatypes)
		{
			foreach (DataType dt in datatypes)
			{
				Write(dt, null);
				writer.Write(";");
                writer.WriteLine();
				writer.WriteLine();
			}
		}

		private void WriteName(bool spacePrefix)
		{
			if (name != null)
			{
				if (spacePrefix)
					writer.Write(" ");
				writer.Write(name);
			}
		}
	}
}
