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

using System;
using System.Collections.Generic;
using Reko.Core.Types;
using System.Linq;

namespace Reko.Typing
{
    public class TypeStoreCycleFinder : IDataTypeVisitor<bool>
    {
        private DataType dtCandidate;
        private TypeStore store;
        private HashSet<DataType> visited;

        private TypeStoreCycleFinder(TypeStore store, DataType dtCandidate)
        {
            this.store = store;
            this.dtCandidate = dtCandidate;
            this.visited = new HashSet<DataType>();
        }

        public bool Find(DataType dt)
        {
            if (dt == dtCandidate)
                return true;
            return dt.Accept(this);
        }

        public static bool IsInCycle(TypeStore store, DataType dtCandidate)
        {
            var finder = new TypeStoreCycleFinder(store, dtCandidate);
            return dtCandidate.Accept(finder);
        }

        public bool VisitArray(ArrayType at)
        {
            return Find(at.ElementType);
        }

        public bool VisitClass(ClassType ct)
        {
            foreach (var field in ct.Fields)
            {
                if (Find(field.DataType))
                    return true;
            }
            foreach (var method in ct.Methods)
            {
                if (Find(method.Procedure.Signature))
                    return true;
            }
            return false;
        }

        public bool VisitCode(CodeType c)
        {
            return false;
        }

        public bool VisitEnum(EnumType e)
        {
            return false;
        }

        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (!visited.Contains(eq))
            {
                visited.Add(eq);
                return Find(eq.DataType);
            }
            return false;
        }

        public bool VisitFunctionType(FunctionType ft)
        {
            return ft.ParametersValid &&
                (Find(ft.ReturnValue.DataType) ||
                 ft.Parameters.Any(p => Find(p.DataType)));
        }

        public bool VisitMemberPointer(MemberPointer memptr)
        {
            return 
                Find(memptr.BasePointer) ||
                Find(memptr.Pointee);
        }

        public bool VisitPointer(Pointer ptr)
        {
            return Find(ptr.Pointee);
        }

        public bool VisitPrimitive(PrimitiveType pt)
        {
            return false;
        }

        public bool VisitReference(ReferenceTo refTo)
        {
            return Find(refTo.Referent);
        }

        public bool VisitString(StringType str)
        {
            return false;
        }

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

        public bool VisitTypeReference(TypeReference typeref)
        {
            return Find(typeref.Referent);
        }

        public bool VisitTypeVariable(TypeVariable tv)
        {
            return Find(tv.DataType);
        }

        public bool VisitUnion(UnionType ut)
        {
            return ut.Alternatives.Values.Any(a => Find(a.DataType));
        }

        public bool VisitUnknownType(UnknownType ut)
        {
            return false;
        }

        public bool VisitVoidType(VoidType voidType)
        {
            return false;
        }
    }
}