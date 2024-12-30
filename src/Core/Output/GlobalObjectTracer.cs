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

using Reko.Core.Collections;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Output
{
    public class GlobalObjectTracer : IDataTypeVisitor<int>
    {
        private readonly Program program;
        private readonly WorkList<(StructureField, Address)> wl;
        private readonly HashSet<Address> visited;
        private readonly DataTypeComparer cmp;
        private readonly StructureFieldCollection globalFields;
        private readonly IEventListener eventListener;
        private int recursionGuard;
        private EndianImageReader rdr; 

        public GlobalObjectTracer(Program program, WorkList<(StructureField, Address)> wl, IEventListener eventListener)
        {
            this.program = program;
            this.wl = wl;
            this.eventListener = eventListener;
            this.globalFields = GlobalFields(program);
            this.visited = new HashSet<Address>();
            this.cmp = new DataTypeComparer();
            this.rdr = default!;
        }

        private StructureFieldCollection GlobalFields(Program program)
        {
            if (program.TypeStore.TryGetTypeVariable(program.Globals, out var tvGlobals) &&
                tvGlobals.DataType is Pointer ptr)
            {
                var str = ptr.Pointee.ResolveAs<StructureType>();
                if (str != null)
                {
                    return str.Fields;
                }
            }
            return program.GlobalFields.Fields;
        }

        public void TraceObject(DataType dataType, Address addr)
        {
            try
            {
                if (!program.Architecture.TryCreateImageReader(program.Memory, addr, out this.rdr!))
                    return;
                dataType.Accept(this);
            }
            catch (AddressCorrelatedException aex)
            {
                eventListener.Error(eventListener.CreateAddressNavigator(program, aex.Address), aex, "An error occurred while tracing the object at {0}.", addr);
            }
            catch (Exception ex)
            {
                eventListener.Error(eventListener.CreateAddressNavigator(program, addr), ex, "An error occurred while tracing the object at {0}.", addr);
            }
        }

        public int VisitArray(ArrayType at)
        {
            for (int i = 0; i < at.Length; ++i)
            {
                at.ElementType.Accept(this);
            }
            return 0;
        }

        public int VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public int VisitCode(CodeType c)
        {
            //$TODO: found fragment of code.
            return 0;
        }

        public int VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public int VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (eq.DataType is null || recursionGuard > 100)
                return 0;
            ++recursionGuard;
            eq.DataType.Accept(this);
            --recursionGuard;
            return 0;
        }

        public int VisitFunctionType(FunctionType ft)
        {
            return 0;
        }

        public int VisitMemberPointer(MemberPointer memptr)
        {
            return 0;
        }

        public int VisitPointer(Pointer ptr)
        {
            var addr = rdr.Address;
            try
            {
                if (!rdr!.TryRead(PrimitiveType.Create(Domain.Pointer, ptr.BitSize), out var c))
                    return 0;
                var a = program.Platform.MakeAddressFromConstant(c, false);
                if (a is null)
                    return 0;
                addr = a.Value;
                if (visited.Contains(addr))
                    return 0;
                // Don't chase unmapped pointers
                if (!program.Memory.IsValidAddress(addr))
                    return 0;
                // Don't chase decompiled procedures.
                if (program.Procedures.ContainsKey(addr))
                    return 0;
                visited.Add(addr);

                int offset = (int) addr.ToLinear(); //$BUG: could be 64-bit!
                var field = globalFields.AtOffset(offset);
                if (field is null)
                {
                    // Nothing there! Ensure a new global at that location. Later,
                    // when the data objects are rendered, we don't have to chase 
                    // the pointer.
                    var dt = ptr.Pointee;
                    //$REVIEW: that this is a pointer to a C-style null 
                    // terminated string is a wild-assed guess of course.
                    // It could be a Pascal string, or a raw area of bytes.
                    // Depend on user for this, or possibly platform.
                    if (dt.Domain == Domain.Character)
                    {
                        dt = StringType.NullTerminated(dt);
                    }
                    field = globalFields.Add(offset, dt);
                }
                wl.Add((field, addr));
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addr, ex, "An error occurred following a pointer at address {0}.", addr);
            }
            return 0;
        }

        public int VisitPrimitive(PrimitiveType pt)
        {
            return 0;
        }

        public int VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
        }

        public int VisitString(StringType str)
        {
            return 0;
        }

        public int VisitStructure(StructureType str)
        {
            var structOffset = rdr!.Offset;
            for (int i = 0; i < str.Fields.Count; ++i)
            {
                rdr.Offset = structOffset + str.Fields[i].Offset;
                str.Fields[i].DataType.Accept(this);
            }
            rdr.Offset = structOffset + str.GetInferredSize();
            return 0;
        }

        public int VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        public int VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public int VisitUnion(UnionType ut)
        {
            var alt = ut.Alternatives.Values.OrderBy(v => v.DataType, cmp).First();
            alt.DataType.Accept(this);
            return 0;
        }

        public int VisitUnknownType(UnknownType ut)
        {
            return 0;
        }

        public int VisitVoidType(VoidType voidType)
        {
            return 0;
        }
    }
}