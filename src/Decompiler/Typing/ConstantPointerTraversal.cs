#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decompiler.Typing
{
    public class ConstantPointerTraversal : IDataTypeVisitor<DataType>
    {
        StructureType globalStr;
        LoadedImage image;
        HashSet<int> visited;
        private int gOffset;

        public ConstantPointerTraversal(StructureType globalStr, LoadedImage image)
        {
            this. globalStr =  globalStr;
            this.image = image;
        }

        public void Traverse()
        {
            this.visited = new HashSet<int>();
            this.gOffset = 0;
            globalStr.Accept(this);
        }

        public DataType VisitArray(ArrayType at)
        {
            int offset = gOffset;
            for (int i = 0; i < at.Length; ++i)
            {
                int off = i * at.ElementType.Size;
                if (visited.Contains(off))
                    break;
                this.gOffset = offset + off;
                at.ElementType.Accept(this);
            }
            return at;
        }

        public DataType VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public DataType VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        public DataType VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public DataType VisitPrimitive(PrimitiveType pt)
        {
            return pt;
        }

        public DataType VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public DataType VisitPointer(Pointer ptr)
        {
            var rdr = image.CreateReader(gOffset - (int) image.BaseAddress.Linear);
            if (!rdr.IsValidOffset((uint)ptr.Size))
                return ptr;
            var c = rdr.ReadLe(PrimitiveType.Create(Domain.Pointer, ptr.Size));    //$REVIEW:Endianess?
            this.gOffset = c.ToInt32(); 
            if (visited.Contains(gOffset))
                return ptr;
            if (image.IsValidLinearAddress((uint) this.gOffset))
                return ptr;
            return ptr.Pointee.Accept(this);
        }

        public DataType VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public DataType VisitStructure(StructureType str)
        {
            int[] offsets = str.Fields.Select(f => f.Offset).ToArray();
            int offset = gOffset;
            foreach (int off in offsets)
            {
                var field = str.Fields.AtOffset(off);
                this.gOffset = offset + field.Offset;
                if (!visited.Contains(gOffset))
                    field.DataType.Accept(this);
            }
            return str;
        }

        public DataType VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public DataType VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public DataType VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public DataType VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public DataType VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}
