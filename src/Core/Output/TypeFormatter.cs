#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;

namespace Reko.Core.Output
{
	/// <summary>
	/// Formats type declarations using indentation settings specified by
    /// caller.
	/// </summary>
	public class TypeFormatter : IDataTypeVisitor<Formatter>
	{
        private Formatter writer;
		private string name;
		private Dictionary<DataType,object> visited;
		private Mode mode;

		private readonly object Declared = 1;
		private readonly object Defined = 2;
        private int nesting;

        public enum Mode { Writing, Scanning }

		public TypeFormatter(Formatter writer)
		{
            this.writer = writer;
			this.visited = new Dictionary<DataType,object>();
			this.mode = Mode.Writing;
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
            if (this.nesting > 90)
            {
                writer.Write("... ");
                WriteName(true);
                return writer;
            }
            ++this.nesting;

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
            --this.nesting;
            return writer;
		}

        public Formatter VisitClass(ClassType ct)
        {
            var n = this.name;
            if (mode == Mode.Writing)
            {
                if (visited.TryGetValue(ct, out object v) && (v == Defined || v == Declared))
                {
                    writer.WriteHyperlink(ct.Name, ct);
                }
                else if (v != Declared)
                {
                    visited[ct] = Declared;
                    ScanFields(ct);
                    ScanMethods(ct);
                    writer.WriteKeyword("class");
                    writer.Write(" ");
                    writer.WriteHyperlink(ct.Name, ct);
                    OpenBrace(ct.Size > 0 ? string.Format("size: {0} {0:X}", ct.Size) : null);

                    WriteClassMembers(ct, ClassProtection.Public, "public");
                    WriteClassMembers(ct, ClassProtection.Protected, "protected");
                    WriteClassMembers(ct, ClassProtection.Private, "private");

                    CloseBrace();
                    visited[ct] = Defined;
                }

                name = n;
                WriteName(true);
            }
            else
            {
                if (!visited.ContainsKey(ct))
                {
                    visited[ct] = Declared;
                    writer.WriteKeyword("class");
                    writer.Write(" ");
                    writer.WriteHyperlink(ct.Name, ct);
                    writer.Write(";");
                    writer.WriteLine();
                }
            }
            return writer;
        }

        private void WriteClassMembers(ClassType ct, ClassProtection protection, string sectionName)
        { 
            var methods = ct.Methods.Where(m => m.Protection == protection)
                .OrderBy(m => m.Offset).ThenBy(m => m.Name)
                .ToList();
            var fields = ct.Fields.Where(f => f.Protection == protection)
                .OrderBy(m => m.Offset)
                .ToList();
            if (methods.Count == 0 && fields.Count == 0)
                return;
            writer.Indentation -= writer.TabSize;
            BeginLine();
            writer.WriteKeyword(sectionName);
            writer.WriteLine(":");
            writer.Indentation += writer.TabSize;

            foreach (var m in methods)
            {
                //$TODO: finish this.
                BeginLine();
                writer.Write(m.Name);
                writer.Write("()");
                EndLine(";");
            }
            if (methods.Count > 0 && fields.Count > 0)
            {
                // separate methods from fields.
                writer.WriteLine();
            }
            foreach (var f in fields)
            {
                BeginLine();
                var trf = new TypeReferenceFormatter(writer);
                trf.WriteDeclaration(f.DataType, f.Name);
                EndLine(";", string.Format("{0:X}", f.Offset));
            }
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
            if (mode == Mode.Writing)
            {
                if (!visited.ContainsKey(e))
                {
                    writer.WriteKeyword("enum");
                    writer.Write(" ");
                    writer.WriteHyperlink(e.Name, e);
                    OpenBrace();

                    foreach (var member in e.Members.OrderBy(de => de.Value))
                    {
                        BeginLine();
                        writer.Write(member.Key);
                        writer.Write(" = ");
                        writer.Write("0x{0}", member.Value);
                        EndLine(",");
                    }

                    CloseBrace();
                    visited[e] = Defined;
                }
            }
            else
            {
                if (!visited.ContainsKey(e))
                {
                    visited[e] = Declared;
                    writer.WriteKeyword("enum");
                    writer.Write(" ");
                    writer.WriteHyperlink(e.Name, e);
                    EndLine(";");
                }
            }
            return writer;
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
			ft.ReturnValue.DataType.Accept(this);
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
			if (ft.Parameters != null && ft.Parameters.Length > 0)
			{
                name = ft.Parameters[0].Name;
				ft.Parameters[0].DataType.Accept(this);
				
				for (int i = 1; i < ft.Parameters.Length; ++i)
				{
                    if (mode == Mode.Writing)
                    {
                        writer.Write(", ");
                    }
                    name = ft.Parameters[i].Name;
					ft.Parameters[i].DataType.Accept(this);
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
                if (visited.TryGetValue(str, out object v) && (v == Defined))
                {
                    writer.WriteKeyword("struct");
                    writer.Write(" ");
                    writer.Write(str.Name);
                }
                else
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
                            var trf = new TypeReferenceFormatter(writer);
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

        public void ScanFields(ClassType ct)
        {
            Mode m = mode;
            mode = Mode.Scanning;
            foreach (var f in ct.Fields)
            {
                f.DataType.Accept(this);
            }
            mode = m;
        }

        public void ScanMethods(ClassType ct)
        {
            Mode m = mode;
            mode = Mode.Scanning;
            foreach (var method in ct.Methods)
            {
                //$TODO: it would be greate if FunctionType were a parent
                // of ProcedureSignature.
            }
            mode = m;
        }

		public Formatter VisitMemberPointer(MemberPointer memptr)
		{
            DataType baseType;
            if (memptr.BasePointer is Pointer p)
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
				if (string.IsNullOrEmpty(name))
					name = "*";
				else 
					name = "* " + name;
			}
			pt.Pointee.Accept(this);
            return writer;
		}

        public static Formatter WriteQualifier(Qualifier q, Formatter writer)
        {
            var sep = "";
            if ((q & Qualifier.Const) != 0)
            {
                sep = " ";
                writer.WriteKeyword("const");
            }
            if ((q & Qualifier.Volatile) != 0)
            {
                writer.Write(sep);
                sep = " ";
                writer.WriteKeyword("volatile");
            }
            if ((q & Qualifier.Restricted) != 0)
            {
                writer.Write(sep);
                sep = " ";
                writer.WriteKeyword("restricted");
            }
            writer.Write(sep);
            return writer;
        }

        public Formatter VisitReference(ReferenceTo refTo)
        {
            if (mode == Mode.Writing)
            {
                if (string.IsNullOrEmpty(name))
                    name = "&";
                else
                    name = "& " + name;
            }
            refTo.Referent.Accept(this);
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
            this.writer.WriteType(t.Name, t);
            return writer;
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
                var trf = new TypeReferenceFormatter(writer);
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
				EndLine(";");
				writer.WriteLine();
			}
		}

		private void WriteName(bool spacePrefix)
		{
			if (!string.IsNullOrEmpty(name))
			{
				if (spacePrefix)
					writer.Write(" ");
				writer.Write(name);
			}
		}
	}
}
