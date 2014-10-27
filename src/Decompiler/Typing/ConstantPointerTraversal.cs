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
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Typing
{
    public class ConstantPointerTraversal : IDataTypeVisitor<IEnumerable<ConstantPointerTraversal.WorkItem>>
    {
        StructureType globalStr;
        LoadedImage image;
        HashSet<int> visited;
        Stack<IEnumerator<WorkItem>> stack;
        private int gOffset;

        public struct WorkItem
        {
            public int GlobalOffset;
            public DataType DataType;
        }

        public ConstantPointerTraversal(StructureType globalStr, LoadedImage image)
        {
            this.globalStr =  globalStr;
            this.image = image;
        }

        public void Traverse()
        {
            this.stack = new Stack<IEnumerator<WorkItem>>();
            this.visited = new HashSet<int>();
            stack.Push(Single(new WorkItem { GlobalOffset = 0, DataType = globalStr }).GetEnumerator());
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (!item.MoveNext())
                    continue;
                stack.Push(item);
                this.gOffset = item.Current.GlobalOffset;
                var children = item.Current.DataType.Accept(this);
                if (children != null)
                {
                    stack.Push(children.GetEnumerator());
                }
            }
        }

        private IEnumerable<T> Single<T>(T dt)
        {
            yield return dt;
        }

        public IEnumerable<WorkItem> VisitArray(ArrayType at)
        {
            int offset = gOffset;
            Debug.Print("Iterating array at {0:X}", gOffset);
            for (int i = 0; i < at.Length; ++i)
            {
                int off = i * at.ElementType.Size;
                if (visited.Contains(off))
                    break;
                yield return new WorkItem { GlobalOffset = offset + off, DataType = at.ElementType };
            }
        }

        public IEnumerable<WorkItem> VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        public IEnumerable<WorkItem> VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitPrimitive(PrimitiveType pt)
        {
            return null;
        }

        public IEnumerable<WorkItem> VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitPointer(Pointer ptr)
        {
            Debug.Print("Iterating pointer at {0:X}", gOffset);
            var rdr = image.CreateReader(gOffset - (int) image.BaseAddress.Linear);
            if (!rdr.IsValidOffset((uint) ptr.Size))
                return null;
            var c = rdr.ReadLe(PrimitiveType.Create(Domain.Pointer, ptr.Size));    //$REVIEW:Endianess?
            int offset = c.ToInt32();
            Debug.Print("  pointer value: {0:X}", offset);
            if (visited.Contains(offset))
                return null;
            if (image.IsValidLinearAddress((uint) offset))
                return null;
            return Single(new WorkItem { DataType = ptr.Pointee, GlobalOffset = c.ToInt32() });
        }

        public IEnumerable<WorkItem> VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitStructure(StructureType str)
        {
            int offset = gOffset;
            Debug.Print("Iterating structure at {0:X}", gOffset);
            foreach (var field in str.Fields)
            {
                int off = offset + field.Offset;
                if (visited.Contains(off))
                    continue;
                Debug.Print("   Field {0} at {1:X}", field.Name, off);
                yield return new WorkItem { DataType = field.DataType, GlobalOffset = off };
            }

        }

        public IEnumerable<WorkItem> VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}
