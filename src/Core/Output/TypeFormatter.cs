#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Output
{
	/// <summary>
	/// Formats type declarations using indentation settings specified by caller.
	/// </summary>
	public class TypeFormatter : IDataTypeVisitor<Formatter>
	{
        private Formatter writer;
		private string name;
		private Dictionary<DataType,object> visited;
		private Mode mode;
        private bool typeReference;

		private readonly object Declared = 1;
		private readonly object Defined = 2;

		public enum Mode { Writing, Scanning }

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
			writer.Indent();
		}

		public void EndLine()
		{
            writer.Terminate();
		}

		public void EndLine(string terminator)
		{
			writer.Terminate(terminator);
		}

		public void EndLine(string terminator, string comment)
		{
			writer.Write(terminator);
			LineEndComment(comment);
			writer.Terminate();
		}

		public void LineEndComment(string comment)
		{
			if (comment != null)
			{
                writer.Write("\t");
				writer.WriteComment("// " + comment);
			}
		}

		public void OpenBrace()
		{
			OpenBrace(null);
		}

		public void OpenBrace(string trailingComment)
		{
			EndLine(" {", trailingComment);
            writer.Indentation += writer.TabSize;
		}

		public void CloseBrace()
		{
            writer.Indentation -= writer.TabSize;
			BeginLine();
			writer.Write("}");
		}

		#region IDataTypeVisitor methods ///////////////////////////////////////

		public Formatter VisitArray(ArrayType at)
		{
			string oldName = name;
			name = null;
			at.ElementType.Accept(this);
            if (mode == Mode.Writing)
            {
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
            return writer;
		}

        public Formatter VisitCode(CodeType c)
        {
            if (mode == Mode.Writing)
            {
                writer.Write("code");
                WriteName(true);
            }
            return writer;
        }

        public Formatter VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

		public Formatter VisitEquivalenceClass(EquivalenceClass eq)
		{
            if (mode == Mode.Writing)
            {
                if (eq.DataType == null)
                {
                    //$BUG: we should never have Eq.classes with null DataType properties.
                    writer.Write("ERROR: EQ_{0}.DataType is Null", eq.Number);
                }
                else
                {
                    writer.WriteType(eq.DataType.Name, eq.DataType);
                }
                WriteName(true);
            }
            return writer;
		}

		public Formatter VisitFunctionType(FunctionType ft)
		{
			string oldName = name;
			name = null;
			ft.ReturnType.Accept(this);
            if (mode == Mode.Writing)
            {
                writer.Write(" (");
            }
			name = oldName;
			WriteName(false);
            if (mode == Mode.Writing)
            {
                writer.Write(")(");
            }
			if (ft.ArgumentTypes.Length > 0)
			{
				name = ft.ArgumentNames != null ? ft.ArgumentNames[0] : null;
				ft.ArgumentTypes[0].Accept(this);
				
				for (int i = 1; i < ft.ArgumentTypes.Length; ++i)
				{
                    if (mode == Mode.Writing)
                    {
                        writer.Write(", ");
                    }
					name = ft.ArgumentNames != null ? ft.ArgumentNames[i] : null;
					ft.ArgumentTypes[i].Accept(this);
				}
				name = oldName;
			}

            if (mode == Mode.Writing)
            {
                writer.Write(")");
            }
            return writer;
		}

        public Formatter VisitString(StringType str)
        {
            return VisitArray(str);
        }

		public Formatter VisitStructure(StructureType str)
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
                    writer.WriteKeyword("struct");
                    writer.Write(" ");
                    writer.WriteHyperlink(str.Name, str);
					OpenBrace(str.Size > 0 ? string.Format("size: {0} {0:X}", str.Size) : null);
					if (str.Fields != null)
					{
						foreach (StructureField f in str.Fields)
						{
							BeginLine();
                            var trf = new TypeReferenceFormatter(writer, true);
                            trf.WriteDeclaration(f.DataType, f.Name);
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
                    writer.WriteHyperlink(str.Name, str);
                    writer.Write(";");
					writer.WriteLine();
				}
			}
            return writer;
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

		public Formatter VisitMemberPointer(MemberPointer memptr)
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
                writer.WriteType(baseType.Name, baseType);
                writer.Write("::*");
            }
			name = oldName;
            if (mode == Mode.Writing)
            {
                WriteName(false);
            }
            return writer;
		}

		public Formatter VisitPointer(Pointer pt)
		{
			if (mode == Mode.Writing)
			{
				if (name == null)
					name = "*";
				else 
					name = "* " + name;
			}
			pt.Pointee.Accept(this);
            return writer;
		}

		public Formatter VisitPrimitive(PrimitiveType pt)
		{
			if (mode == Mode.Writing)
			{
                writer.WriteKeyword(pt.ToString());
				WriteName(true);
			}
            return writer;
		}

        public Formatter VisitTypeReference(TypeReference typeref)
        {
            if (mode == Mode.Writing)
            {
                writer.Write(typeref.Name);
                WriteName(true);
            }
            return writer;
        }

        public Formatter VisitTypeVariable(TypeVariable t)
		{
			throw new NotImplementedException("TypeFormatter.TypeVariable");
		}

        public Formatter VisitUnion(UnionType ut)
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
                var trf = new TypeReferenceFormatter(writer, true);
                trf.WriteDeclaration(alt.DataType, alt.Name);
				EndLine(";");
				++i;
			}
			CloseBrace();

			name = n;
			WriteName(true);
            return writer;
		}

		public Formatter VisitUnknownType(UnknownType ut)
		{
			if (mode == Mode.Writing)
			{
				writer.WriteKeyword("void");
			}
            return writer;
		}

        public Formatter VisitVoidType(VoidType vt)
        {
            if (mode == Mode.Writing)
            {
                writer.WriteKeyword("void");
            }
            return writer;
        }
		#endregion

		public void Write(DataType dt, string name)
		{
			this.name = name;
			dt.Accept(this);
		}

		public void WriteTypes(IEnumerable<DataType> datatypes)
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
