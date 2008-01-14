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

using Decompiler.Core.Types;
using System;
using System.IO;
using System.Collections;

namespace Decompiler.Core.Output
{
	/// <summary>
	/// Formats types using indentation settings specified by caller.
	/// </summary>
	public class TypeFormatter : Formatter, IDataTypeVisitor
	{
		private string name;
		public int indentLevel;
		private Hashtable visited;
		private Mode mode;
		private readonly object Declared = 1;
		private readonly object Defined = 2;

		private enum Mode { Writing, Scanning }

		public TypeFormatter(TextWriter tw) : base(tw)
		{
			this.visited = new Hashtable();
			this.mode = Mode.Writing;
		}

		public void BeginLine()
		{
			BeginLine("");
		}

		public void BeginLine(string s)
		{
			for (int i = 0; i < indentLevel; ++i)
			{
				writer.Write('\t');
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
			at.ElementType.Accept(this);
			WriteName();
			writer.Write("[");
			if (at.Length != 0)
			{
				writer.Write(at.Length);
			}
			writer.Write("]");
		}

		public void VisitEquivalenceClass(EquivalenceClass eq)
		{
			throw new NotImplementedException("TypeFormatter.EquivalenceClass");
		}

		public void VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException("TypeFormatter.FunctionType");
		}

		public void VisitStructure(StructureType str)
		{
			string n = name;
			if (mode == Mode.Writing)
			{
				if (visited[str] == Defined || visited[str] == Declared)
				{
					writer.Write("struct {0}", str.Name);
				}
				else if (visited[str] != Declared)
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
				WriteName();
			}
			else
			{
				if (visited[str] == null)
				{
					visited[str] = Declared;
					writer.WriteLine("struct {0};", str.Name);
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
			throw new NotImplementedException();
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
				pt.Write(writer);
				WriteName();
			}
		}

		public void VisitTypeVar(TypeVariable t)
		{
			throw new NotImplementedException("TypeFormatter.TypeVariable");
		}

		public void VisitUnion(UnionType ut)
		{
			string n = name;

			writer.Write("union {0}", ut.Name);
			OpenBrace();
			int i = 0;
			foreach (UnionAlternative alt in ut.Alternatives)
			{
				BeginLine();
				name = alt.MakeName(i);
				alt.DataType.Accept(this);
				EndLine(";");
				++i;
			}
			CloseBrace();

			name = n;
			WriteName();
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

		public void WriteTypes(ICollection datatypes)
		{
			foreach (DataType dt in datatypes)
			{
				Write(dt, null);
				writer.WriteLine(";");
				writer.WriteLine();
			}
		}

		private void WriteName()
		{
			if (name != null)
			{
				writer.Write(" ");
				writer.Write(name);
			}
		}
	}
}
