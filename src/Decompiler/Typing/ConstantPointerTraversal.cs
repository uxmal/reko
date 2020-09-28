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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// This class follows constant pointers to discover structures in memory
    /// and possibly new pointers.
    /// </summary>
    public class ConstantPointerTraversal : IDataTypeVisitor<IEnumerable<ConstantPointerTraversal.WorkItem>>
    {
        private IProcessorArchitecture arch;
        private StructureType globalStr;
        private SegmentMap segmentMap;
        private HashSet<int> visited;
        private Stack<IEnumerator<WorkItem>> stack;
        private int gOffset;
        private IEnumerator<WorkItem> eCurrent;

        public struct WorkItem
        {
            public int GlobalOffset;
            public DataType DataType;
        }

        public ConstantPointerTraversal(IProcessorArchitecture arch, StructureType globalStr, SegmentMap segmentMap)
        {
            this.arch = arch;
            this.globalStr =  globalStr;
            this.segmentMap = segmentMap;
            this.Discoveries = new List<StructureField>();
        }

        public ConstantPointerTraversal(Program program) 
        {
            this.arch = program.Architecture;
            var ptr = (Pointer) program.Globals.TypeVariable.DataType;
            this.globalStr = (StructureType)((EquivalenceClass) ptr.Pointee).DataType;
            this.segmentMap = program.SegmentMap;
            this.Discoveries = new List<StructureField>();
        }

        public List<StructureField> Discoveries { get; private set; }

        public void Traverse()
        {
            this.stack = new Stack<IEnumerator<WorkItem>>();
            this.visited = new HashSet<int>();
            stack.Push(Single(new WorkItem { GlobalOffset = 0, DataType = globalStr }).GetEnumerator());
            while (stack.Count > 0)
            {
                this.eCurrent = stack.Pop();
                if (!eCurrent.MoveNext())
                    continue;
                stack.Push(eCurrent);
                this.gOffset = eCurrent.Current.GlobalOffset;
                var children = eCurrent.Current.DataType.Accept(this);
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
                yield return new WorkItem { GlobalOffset = offset, DataType = at.ElementType };
                offset += at.ElementType.Size;
            }
        }

        public IEnumerable<WorkItem> VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkItem> VisitEnum(EnumType e)
        {
            return null;
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
            ImageSegment segment;
            if (!segmentMap.TryFindSegment(segmentMap.MapLinearAddressToAddress((ulong) gOffset), out segment))
                return null;
            var rdr = arch.CreateImageReader(segment.MemoryArea,  (ulong) gOffset - segment.MemoryArea.BaseAddress.ToLinear());
            if (!rdr.IsValid)
                return null;
            var c = rdr.Read(PrimitiveType.Create(Domain.Pointer, ptr.BitSize));
            int offset = c.ToInt32();
            Debug.Print("  pointer value: {0:X}", offset);
            if (visited.Contains(offset) || !segment.MemoryArea.IsValidLinearAddress((uint) offset))
                return Enumerable.Empty<WorkItem>();

            // We've successfully traversed a pointer to a valid destination!
            // The address must therefore be of type ptr.Pointee.
            visited.Add(offset);
            if (globalStr.Fields.AtOffset(offset) == null)
            {
                Debug.Print("       Discovery: {0:X} {1}", offset, ptr.Pointee);
                Discoveries.Add(new StructureField(offset, ptr.Pointee));
            }
            return Single(new WorkItem { DataType = ptr.Pointee, GlobalOffset = c.ToInt32() });
        }

        public IEnumerable<WorkItem> VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
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
