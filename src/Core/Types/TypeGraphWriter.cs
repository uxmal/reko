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

using Reko.Core.Output;
using System.Collections.Generic;

namespace Reko.Core.Types
{
    /// <summary>
    /// Generates a more compact and easily parsed string version
    /// of a datatype. For final output, use <see cref="Reko.Core.Output.TypeFormatter"/>.
    /// </summary>
    public class TypeGraphWriter : IDataTypeVisitor<Formatter>
    {
        // Structs with more than this number of fields are considered 'large'
        // for the purpose of rendering.
        private const int SmallCompositeType = 6;
        private const int NestingLimit = 90;

        private HashSet<DataType>? visited;
        private Formatter writer;
        private bool reference;
        private int nesting;

        /// <summary>
        /// Constructs a type graph writer.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public TypeGraphWriter(Formatter writer)
        {
            this.writer = writer;
        }

        /// <inheritdoc/>
        public Formatter VisitArray(ArrayType at)
        {
            writer.Write("(arr ");
            if (this.nesting < NestingLimit)
            {
                ++this.nesting;
                at.ElementType.Accept(this);
                if (at.Length != 0)
                {
                    writer.Write(" {0}", at.Length);
                }
                --this.nesting;
            }
            else
            {
                writer.Write("...");
            }
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitClass(ClassType ct)
        {
            if (this.visited is null)
                visited = new HashSet<DataType>();

            writer.Write("(class");
            if (ct.Name is not null)
            {
                writer.Write(" \"{0}\"", ct.Name);
            }
            if (ct.Size != 0)
            {
                writer.Write(" {0:X4}", ct.Size);
            }

            if (!visited.Contains(ct) && (!reference || ct.Name is null))
            {
                visited.Add(ct);
                foreach (ClassField f in ct.Fields)
                {
                    writer.Write(" ({0:X} ", f.Offset);
                    f.DataType.Accept(this);
                    writer.Write(" {0} {1})", f.Name, f.Protection);
                }
            }
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitCode(CodeType c)
        {
            writer.Write("code");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitEnum(EnumType e)
        {
            writer.Write("(enum");
            if (!string.IsNullOrEmpty(e.Name))
            {
                writer.Write(" {0},", e.Name);
            }
            writer.Write("(");
            var sep = "";
            foreach (var item in e.Members)
            {
                writer.Write(sep);
                sep = ",";
                writer.Write("({0},{1})");
            }
            writer.Write(")");
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitEquivalenceClass(EquivalenceClass eq)
        {
            writer.Write(eq.Name);
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitFunctionType(FunctionType ft)
        {
            writer.Write("(fn ");
            if (ft.ReturnValue is not null)
                ft.ReturnValue.DataType.Accept(this);
            else
                writer.Write("void");
            writer.Write(" (");

            string separator = "";
            if (ft.Parameters is not null)
            {
                for (int i = 0; i < ft.Parameters.Length; ++i)
                {
                    writer.Write(separator);
                    separator = ", ";
                    ft.Parameters[i].DataType.Accept(this);
                }
            }
            writer.Write("))");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitPrimitive(PrimitiveType pt)
        {
            writer.Write(pt.Name);
            WriteQualifier(pt.Qualifier);
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitMemberPointer(MemberPointer memptr)
        {
            writer.Write("(memptr ");
            memptr.BasePointer.Accept(this);
            writer.Write(" ");
            memptr.Pointee.Accept(this);
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitPointer(Pointer ptr)
        {
			writer.Write($"(ptr{ptr.BitSize} ");
            WriteQualifier(ptr.Qualifier);
            if (this.nesting < NestingLimit)
            {
                ++nesting;
                WriteReference(ptr.Pointee);
                --nesting;
            }
            else
            {
                writer.Write("...");
            }
			writer.Write(")");
            return writer;
		}

        /// <inheritdoc/>
        public Formatter WriteQualifier(Qualifier q)
        {
            var sep = "";
            if ((q & Qualifier.Const) != 0)
            {
                sep = " ";
                writer.Write("const");
            }
            if ((q & Qualifier.Volatile) != 0)
            {
                writer.Write(sep);
                sep = " ";
                writer.Write("volatile");
            }
            if ((q & Qualifier.Restricted) != 0)
            {
                writer.Write(sep);
                sep = " ";
                writer.Write("restricted");
            }
            writer.Write(sep);
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitReference(ReferenceTo refTo)
        {
            writer.Write("(ref ");
            WriteReference(refTo.Referent);
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitString(StringType str)
        {
            writer.Write("(str");
            WriteQualifier(str.Qualifier);
            if (str.LengthPrefixType is not null)
            {
                writer.Write(" length-");
                str.LengthPrefixType.Accept(this);
                if (str.PrefixOffset != 0)
                    writer.Write(" {0}", str.PrefixOffset);
            }
            writer.Write(" ");
            str.ElementType.Accept(this);
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitStructure(StructureType str)
        {
            if (this.visited is null)
                visited = new HashSet<DataType>();

            writer.Write("({0}", str.IsSegment ? "segment" : "struct");
            if (str.Name is not null)
            {
                writer.Write(" \"{0}\"", str.Name);
            }
            if (str.Size != 0)
            {
                writer.Write(" {0:X4}", str.Size);
            }

            if (!visited.Contains(str) && (!reference || str.Name is null))
            {
                visited.Add(str);
                if (str.Fields.Count > SmallCompositeType)
                {
                    writer.Indentation += 4;
                    foreach (StructureField f in str.Fields)
                    {
                        writer.WriteLine();
                        writer.Indent();
                        writer.Write("({0:X} ", f.Offset);
                        f.DataType.Accept(this);
                        writer.Write(" {0})", f.Name);
                    }
                    writer.Indentation -= 4;
                }
                else
                {
                    foreach (StructureField f in str.Fields)
                    {
                        writer.Write(" ({0:X} ", f.Offset);
                        f.DataType.Accept(this);
                        writer.Write(" {0})", f.Name);
                    }
                }
            }
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitTypeReference(TypeReference typeref)
        {
            if (string.IsNullOrEmpty(typeref.Name))
            {
                typeref.Referent.Accept(this);
            }
            else
            {
                writer.Write(typeref.Name);
            }
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitTypeVariable(TypeVariable tv)
        {
            writer.Write(tv.Name);
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitUnion(UnionType ut)
        {
            if (visited is null)
                visited = new HashSet<DataType>(); 

            writer.Write("(union");
            if (ut.Name is not null)
            {
                writer.Write(" \"{0}\"", ut.Name);
            }
            if (!visited.Contains(ut) && (!reference || ut.Name is null))
            {
                visited.Add(ut);
                if (ut.Alternatives.Count > SmallCompositeType)
                {
                    writer.Indentation += 4;
                    foreach (UnionAlternative alt in ut.Alternatives.Values)
                    {
                        writer.WriteLine();
                        writer.Indent();
                        writer.Write("(");
                        alt.DataType.Accept(this);
                        writer.Write(" {0})", alt.Name);
                    }
                    writer.Indentation -= 4;
                }
                else
                {
                    foreach (UnionAlternative alt in ut.Alternatives.Values)
                    {
                        writer.Write(" (");
                        alt.DataType.Accept(this);
                        writer.Write(" {0})", alt.Name);
                    }
                }
            }
            writer.Write(")");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitUnknownType(UnknownType ut)
        {
            if (ut.BitSize == 0)
                writer.Write("<unknown>");
            else
                writer.Write($"<unknown{ut.BitSize}>");
            return writer;
        }

        /// <inheritdoc/>
        public Formatter VisitVoidType(VoidType vt)
        {
            writer.Write("void");
            return writer;
        }

        internal void WriteReference(DataType dataType)
        {
            var old = reference;
            reference = true;
            try
            {
                dataType.Accept(this);
            }
            finally
            {
                reference = old;
            }
        }
    }
}
