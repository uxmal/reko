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
        private readonly Formatter writer;
		private readonly Dictionary<DataType,object> visited;
		private string? name;
		private Mode mode;

		private readonly object Declared = 1;
		private readonly object Defined = 2;
        private int nesting;

        private enum Mode { Writing, Scanning }

        /// <summary>
        /// Constructs an type formatter.
        /// </summary>
        /// <param name="writer">Output sink.</param>
		public TypeFormatter(Formatter writer)
		{
            this.writer = writer;
			this.visited = new Dictionary<DataType,object>();
			this.mode = Mode.Writing;
		}

        /// <summary>
        /// Begins a new line of output.
        /// </summary>
		public void BeginLine()
		{
			writer.Indent();
		}

        /// <summary>
        /// Ends a line of output.
        /// </summary>
		public void EndLine()
		{
            writer.Terminate();
		}

        /// <summary>
        /// Ends a line of output with a terminator string.
        /// </summary>
        /// <param name="terminator">Final string on the line.</param>
		public void EndLine(string terminator)
		{
			writer.Terminate(terminator);
		}

        /// <summary>
        /// Ends a line of output with a terminator string and a line-end comment.
        /// </summary>
        /// <param name="terminator">Final string on the line.</param>
        /// <param name="comment">Optional line-end comment.</param>
		public void EndLine(string terminator, string? comment)
		{
			writer.Write(terminator);
			LineEndComment(comment);
			writer.Terminate();
		}

        /// <summary>
        /// Write a line end comment.
        /// </summary>
        /// <param name="comment"></param>
		public void LineEndComment(string? comment)
		{
			if (comment is not null)
			{
                writer.Write("\t");
				writer.WriteComment("// " + comment);
			}
		}

        /// <summary>
        /// Write an opening beace with an optional trailing comment.
        /// </summary>
		public void OpenBrace()
		{
			OpenBrace(null);
		}

        /// <summary>
        /// Write an opening beace with an optional trailing comment.
        /// </summary>
        /// <param name="trailingComment">Optional trailing comment.</param>
		public void OpenBrace(string? trailingComment)
		{
			EndLine(" {", trailingComment);
            writer.Indentation += writer.TabSize;
		}

        /// <summary>
        /// Write a closing brace.
        /// </summary>
		public void CloseBrace()
		{
            writer.Indentation -= writer.TabSize;
			BeginLine();
			writer.Write("}");
		}

		#region IDataTypeVisitor methods ///////////////////////////////////////

        /// <inheritdoc/>
		public Formatter VisitArray(ArrayType at)
		{
			string? oldName = name;
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

        /// <inheritdoc/>
        public Formatter VisitClass(ClassType ct)
        {
            var n = this.name;
            if (mode == Mode.Writing)
            {
                if (visited.TryGetValue(ct, out object? v) && (v == Defined || v == Declared))
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

                    WriteClassMembers(ct, AccessSpecifier.Public, "public");
                    WriteClassMembers(ct, AccessSpecifier.Protected, "protected");
                    WriteClassMembers(ct, AccessSpecifier.Private, "private");

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

        private void WriteClassMembers(ClassType ct, AccessSpecifier protection, string sectionName)
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

        /// <inheritdoc/>
        public Formatter VisitCode(CodeType c)
        {
            if (mode == Mode.Writing)
            {
                writer.Write("code");
                WriteName(true);
            }
            return writer;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
		public Formatter VisitEquivalenceClass(EquivalenceClass eq)
		{
            if (mode == Mode.Writing)
            {
                if (eq.DataType is null)
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

        /// <inheritdoc/>
		public Formatter VisitFunctionType(FunctionType ft)
		{
			string? oldName = name;
			name = null;
            if (ft.ParametersValid)
			    ft.ReturnValue!.DataType.Accept(this);
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
			if (ft.ParametersValid && ft.Parameters is not null && ft.Parameters.Length > 0)
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

        /// <inheritdoc/>
        public Formatter VisitString(StringType str)
        {
            return VisitArray(str);
        }

        /// <inheritdoc/>
		public Formatter VisitStructure(StructureType str)
		{
			string? n = name;
			if (mode == Mode.Writing)
			{
                if (visited.TryGetValue(str, out object? v) && (v == Defined))
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
                    if (str.Fields is not null)
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

		private void ScanFields(StructureType str)
		{
			Mode m = mode;
			mode = Mode.Scanning;

			foreach (StructureField f in str.Fields)
			{
				f.DataType.Accept(this);
			}
			mode = m;
		}

        private void ScanFields(ClassType ct)
        {
            Mode m = mode;
            mode = Mode.Scanning;
            foreach (var f in ct.Fields)
            {
                f.DataType.Accept(this);
            }
            mode = m;
        }

        private void ScanMethods(ClassType ct)
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

        /// <inheritdoc/>
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

            string? oldName = name;
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Writes a C style qualifier.
        /// </summary>
        /// <param name="q">Qualifier.</param>
        /// <param name="writer">Output sink.</param>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Formatter VisitPrimitive(PrimitiveType pt)
		{
			if (mode == Mode.Writing)
			{
                writer.WriteKeyword(pt.ToString());
				WriteName(true);
			}
            return writer;
		}

        /// <inheritdoc/>
        public Formatter VisitTypeReference(TypeReference typeref)
        {
            if (mode == Mode.Writing)
            {
                writer.Write(typeref.Name);
                WriteName(true);
            }
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitTypeVariable(TypeVariable t)
		{
            this.writer.WriteType(t.Name, t);
            return writer;
		}

        /// <inheritdoc/>
        public Formatter VisitUnion(UnionType ut)
		{
			string? n = name;

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

        /// <inheritdoc/>
		public Formatter VisitUnknownType(UnknownType ut)
		{
			if (mode == Mode.Writing)
			{
				writer.WriteKeyword("void");
			}
            return writer;
		}

        /// <inheritdoc/>
        public Formatter VisitVoidType(VoidType vt)
        {
            if (mode == Mode.Writing)
            {
                writer.WriteKeyword("void");
            }
            return writer;
        }
        #endregion

        /// <summary>
        /// Write a data type to the output sink.
        /// </summary>
        /// <param name="dt">Data time</param>
        /// <param name="name">Optional variable name</param>
        public void Write(DataType dt, string? name)
		{
			this.name = name;
			dt.Accept(this);
		}

        /// <summary>
        /// Write a collection of data types to the output sink.
        /// </summary>
        /// <param name="datatypes">Data types to write.</param>
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
				writer.Write(name!);
			}
		}
	}
}
