#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using System.Collections.Generic;

namespace Reko.Typing
{
    /// <summary>
    /// Chooses apropriate union alternative. It test each alternative by
    /// doing recursive search at types graph to find any data type at
    /// specified offset which is compatible with specified result data type.
    /// </summary>
    public class UnionAlternativeChooser : IDataTypeVisitor<bool>
    {
        private int offset;
        private bool isEnclosingPtr;
        private readonly HashSet<DataType> visitedTypes;
        private readonly DataType? dtResult;
        private readonly Unifier unifier;

        private UnionAlternativeChooser(
            DataType? dtResult, int offset, bool isEnclosingPtr,
            HashSet<DataType> visitedTypes)
        {
            this.dtResult = dtResult;
            this.offset = offset;
            this.isEnclosingPtr = isEnclosingPtr;
            this.visitedTypes = visitedTypes;
            this.unifier = new Unifier();
        }

        /// <summary>
        /// Given a union type, a result data type and an offset,
        /// choose an alternative from the union that matches the result data type
        /// </summary>
        /// <param name="ut"></param>
        /// <param name="dtResult"></param>
        /// <param name="isEnclosingPtr"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UnionAlternative? Choose(
            UnionType ut,
            DataType? dtResult,
            bool isEnclosingPtr,
            int offset)
        {
            return Choose(
                ut, dtResult, isEnclosingPtr, offset, new HashSet<DataType>());
        }

        /// <inheritdoc/>
        public bool VisitArray(ArrayType at)
        {
            return IsValidState(at);
        }

        /// <inheritdoc/>
        public bool VisitClass(ClassType ct)
        {
            return IsValidState(ct);
        }

        /// <inheritdoc/>
        public bool VisitCode(CodeType c)
        {
            return IsValidState(c);
        }

        /// <inheritdoc/>
        public bool VisitEnum(EnumType e)
        {
            return IsValidState(e);
        }

        /// <inheritdoc/>
        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitFunctionType(FunctionType ft)
        {
            return IsValidState(ft);
        }

        /// <inheritdoc/>
        public bool VisitMemberPointer(MemberPointer memptr)
        {
            return IsValidState(memptr);
        }

        /// <inheritdoc/>
        public bool VisitPointer(Pointer ptr)
        {
            if (this.isEnclosingPtr)
                return IsValidState(ptr);
            this.isEnclosingPtr = true;
            return ptr.Pointee.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitPrimitive(PrimitiveType ptr)
        {
            return IsValidState(ptr);
        }

        /// <inheritdoc/>
        public bool VisitReference(ReferenceTo refTo)
        {
            return IsValidState(refTo);
        }

        /// <inheritdoc/>
        public bool VisitString(StringType str)
        {
            return IsValidState(str);
        }

        /// <inheritdoc/>
        public bool VisitStructure(StructureType str)
        {
            if (visitedTypes.Contains(str))
                return false;
            visitedTypes.Add(str);
            if (!this.isEnclosingPtr)
                return false;
            var field = str.Fields.LowerBound(this.offset);
            if (field is null)
                return false;
            this.offset -= field.Offset;
            return field.DataType.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitTypeVariable(TypeVariable tv)
        {
            return IsValidState(tv);
        }

        /// <inheritdoc/>
        public bool VisitUnion(UnionType ut)
        {
            if (!visitedTypes.Add(ut))
                return false;
            return Choose(ut) is not null;
        }

        /// <inheritdoc/>
        public bool VisitUnknownType(UnknownType ut)
        {
            return IsValidState(ut);
        }

        /// <inheritdoc/>
        public bool VisitVoidType(VoidType voidType)
        {
            return IsValidState(voidType);
        }

        private bool IsValidState(DataType dt)
        {
            return
                offset == 0 && this.isEnclosingPtr &&
                (dtResult is null || unifier.AreCompatible(dt, dtResult));
        }

        private UnionAlternative? Choose(UnionType ut)
        {
            return Choose(ut, dtResult, isEnclosingPtr, offset, visitedTypes);
        }

        private static UnionAlternative? Choose(
            UnionType ut,
            DataType? dtResult,
            bool isEnclosingPtr,
            int offset,
            HashSet<DataType> visitedTypes)
        {
            foreach (var alt in ut.Alternatives.Values)
            {
                var chooser = new UnionAlternativeChooser(
                    dtResult, offset, isEnclosingPtr, visitedTypes);
                if (alt.DataType.Accept(chooser))
                    return alt;
            }
            return null;
        }
    }
}
