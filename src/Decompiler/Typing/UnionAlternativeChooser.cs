#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static UnionAlternative? Choose(
            UnionType ut, DataType? dtResult, bool isEnclosingPtr, int offset)
        {
            return Choose(
                ut, dtResult, isEnclosingPtr, offset, new HashSet<DataType>());
        }

        public bool VisitArray(ArrayType at)
        {
            return IsValidState(at);
        }

        public bool VisitClass(ClassType ct)
        {
            return IsValidState(ct);
        }

        public bool VisitCode(CodeType c)
        {
            return IsValidState(c);
        }

        public bool VisitEnum(EnumType e)
        {
            return IsValidState(e);
        }

        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        public bool VisitFunctionType(FunctionType ft)
        {
            return IsValidState(ft);
        }

        public bool VisitMemberPointer(MemberPointer memptr)
        {
            return IsValidState(memptr);
        }

        public bool VisitPointer(Pointer ptr)
        {
            if (this.isEnclosingPtr)
                return IsValidState(ptr);
            this.isEnclosingPtr = true;
            return ptr.Pointee.Accept(this);
        }

        public bool VisitPrimitive(PrimitiveType ptr)
        {
            return IsValidState(ptr);
        }

        public bool VisitReference(ReferenceTo refTo)
        {
            return IsValidState(refTo);
        }

        public bool VisitString(StringType str)
        {
            return IsValidState(str);
        }

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

        public bool VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        public bool VisitTypeVariable(TypeVariable tv)
        {
            return IsValidState(tv);
        }

        public bool VisitUnion(UnionType ut)
        {
            if (visitedTypes.Contains(ut))
                return false;
            visitedTypes.Add(ut);
            return Choose(ut) is not null;
        }

        public bool VisitUnknownType(UnknownType ut)
        {
            return IsValidState(ut);
        }

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
