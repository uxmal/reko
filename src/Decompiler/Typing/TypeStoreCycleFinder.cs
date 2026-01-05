#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;
using Reko.Core.Types;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Finds data types that are involved in self-referential cycles that are not
    /// broken by pointers or references.
    /// </summary>
    public class TypeStoreCycleFinder : IDataTypeVisitor<bool>
    {
        private readonly DataType dtCandidate;
        private readonly TypeStore store;
        private readonly HashSet<DataType> visited;

        private TypeStoreCycleFinder(TypeStore store, DataType dtCandidate)
        {
            this.store = store;
            this.dtCandidate = dtCandidate;
            this.visited = [];
        }

        /// <summary>
        /// Determine if <paramref name="dt"/> is in a type cycle.
        /// </summary>
        /// <param name="dt">Data type to test.</param>
        /// <returns>True if <paramref name="dt"/> is in a type cycle.</returns>
        public bool Find(DataType dt)
        {
            if (dt is null) 
                return false;
            if (dt == dtCandidate)
                return true;
            return dt.Accept(this);
        }

        /// <summary>
        /// Determines whether the data type <paramref name="dtCandidate"/> is involved
        /// in a type cycle.
        /// </summary>
        /// <param name="store">Type store to analyze.</param>
        /// <param name="dtCandidate">Type to analyze.</param>
        /// <returns>True if the data type is in a type cycle; otherwise false.
        /// </returns>
        public static bool IsInCycle(TypeStore store, DataType dtCandidate)
        {
            var finder = new TypeStoreCycleFinder(store, dtCandidate);
            return dtCandidate.Accept(finder);
        }

        /// <inheritdoc/>
        public bool VisitArray(ArrayType at)
        {
            return Find(at.ElementType);
        }

        /// <inheritdoc/>
        public bool VisitClass(ClassType ct)
        {
            foreach (var field in ct.Fields)
            {
                if (Find(field.DataType))
                    return true;
            }
            foreach (var method in ct.Methods)
            {
                if (Find(method.Procedure!.Signature))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool VisitCode(CodeType c)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitEnum(EnumType e)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (!visited.Contains(eq))
            {
                visited.Add(eq);
                return Find(eq.DataType);
            }
            return false;
        }

        /// <inheritdoc/>
        public bool VisitFunctionType(FunctionType ft)
        {
            return ft.ParametersValid &&
                (Find(ft.Outputs[0].DataType) ||
                 ft.Parameters!.Any(p => Find(p.DataType)));
        }

        /// <inheritdoc/>
        public bool VisitMemberPointer(MemberPointer memptr)
        {
            return 
                Find(memptr.BasePointer) ||
                Find(memptr.Pointee);
        }

        /// <inheritdoc/>
        public bool VisitPointer(Pointer ptr)
        {
            return Find(ptr.Pointee);
        }

        /// <inheritdoc/>
        public bool VisitPrimitive(PrimitiveType pt)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitReference(ReferenceTo refTo)
        {
            return Find(refTo.Referent);
        }

        /// <inheritdoc/>
        public bool VisitString(StringType str)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitStructure(StructureType str)
        {
            if (!visited.Contains(str))
            {
                visited.Add(str);
                foreach (var field in str.Fields)
                {
                    if (Find(field.DataType))
                        return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public bool VisitTypeReference(TypeReference typeref)
        {
            return Find(typeref.Referent);
        }

        /// <inheritdoc/>
        public bool VisitTypeVariable(TypeVariable tv)
        {
            return Find(tv.DataType);
        }

        /// <inheritdoc/>
        public bool VisitUnion(UnionType ut)
        {
            return ut.Alternatives.Values.Any(a => Find(a.DataType));
        }

        /// <inheritdoc/>
        public bool VisitUnknownType(UnknownType ut)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitVoidType(VoidType voidType)
        {
            return false;
        }
    }
}