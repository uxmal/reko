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
    public class UnionAlternativeChooser : IDataTypeVisitor<bool>
    {
        private int offset;
        private bool isEnclosingPtr;
        private HashSet<DataType> visitedTypes;

        private UnionAlternativeChooser(
            int offset, bool isEnclosingPtr, HashSet<DataType> visitedTypes)
        {
            this.offset = offset;
            this.isEnclosingPtr = isEnclosingPtr;
            this.visitedTypes = visitedTypes;
        }

        public static UnionAlternative? Choose(
            UnionType ut, bool isEnclosingPtr, int offset)
        {
            return Choose(ut, isEnclosingPtr, offset, new HashSet<DataType>());
        }

        public bool VisitArray(ArrayType at)
        {
            return IsValidState();
        }

        public bool VisitClass(ClassType ct)
        {
            return IsValidState();
        }

        public bool VisitCode(CodeType c)
        {
            return IsValidState();
        }

        public bool VisitEnum(EnumType e)
        {
            return IsValidState();
        }

        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        public bool VisitFunctionType(FunctionType ft)
        {
            return IsValidState();
        }

        public bool VisitMemberPointer(MemberPointer memptr)
        {
            return IsValidState();
        }

        public bool VisitPointer(Pointer ptr)
        {
            if (this.isEnclosingPtr)
                return IsValidState();
            this.isEnclosingPtr = true;
            return ptr.Pointee.Accept(this);
        }

        public bool VisitPrimitive(PrimitiveType pt)
        {
            return IsValidState();
        }

        public bool VisitReference(ReferenceTo refTo)
        {
            return IsValidState();
        }

        public bool VisitString(StringType str)
        {
            return IsValidState();
        }

        public bool VisitStructure(StructureType str)
        {
            if (visitedTypes.Contains(str))
                return false;
            visitedTypes.Add(str);
            if (!this.isEnclosingPtr)
                return false;
            var field = str.Fields.LowerBound(this.offset);
            if (field == null)
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
            return IsValidState();
        }

        public bool VisitUnion(UnionType ut)
        {
            if (visitedTypes.Contains(ut))
                return false;
            visitedTypes.Add(ut);
            return Choose(ut) != null;
        }

        public bool VisitUnknownType(UnknownType ut)
        {
            return IsValidState();
        }

        public bool VisitVoidType(VoidType voidType)
        {
            return IsValidState();
        }

        private bool IsValidState()
        {
            return offset == 0 && this.isEnclosingPtr;
        }

        private UnionAlternative? Choose(UnionType ut)
        {
            return Choose(ut, isEnclosingPtr, offset, visitedTypes);
        }

        private static UnionAlternative? Choose(
            UnionType ut,
            bool isEnclosingPtr,
            int offset,
            HashSet<DataType> visitedTypes)
        {
            foreach (var alt in ut.Alternatives.Values)
            {
                var chooser = new UnionAlternativeChooser(
                    offset, isEnclosingPtr, visitedTypes);
                if (alt.DataType.Accept(chooser))
                    return alt;
            }
            return null;
        }
    }
}
