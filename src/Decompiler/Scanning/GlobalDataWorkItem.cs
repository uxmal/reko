#region License
/*
 * Copyright (C) 1999-2016 Pavel Tomin.
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

using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Serialization;
using System;
using System.Diagnostics;

namespace Reko.Scanning
{
    /// <summary>
    /// The purpose of this class is to discover interesting global variables. In particular,
    /// we want to discover pointers to procedures in global data.
    /// </summary>
    public class GlobalDataWorkItem : WorkItem, IDataTypeVisitor<bool>
    {
        private IScanner scanner;
        private Program program;
        private DataType dt;
        private ImageReader rdr;

        public GlobalDataWorkItem(IScanner scanner, Program program, Address addr, DataType dt) : base(addr)
        {
            this.scanner = scanner;
            this.program = program;
            this.dt = dt;
            this.rdr = program.CreateImageReader(addr);
        }

        public override void Process()
        {
            dt.Accept(this);
        }

        public bool VisitArray(ArrayType at)
        {
            if (at.Length == 0)
            {
                scanner.Warn(Address, "User-specified arrays must have a non-zero size.");
                return false;
            }
            for (int i = 0; i < at.Length; ++i)
            {
                at.ElementType.Accept(this);
            }
            return false;
        }

        public bool VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public bool VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public bool VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public bool VisitFunctionType(FunctionType ft)
        {
            var addr = rdr.Address;
            var up = new Procedure_v1
            {
                Address = addr.ToString(),
                Signature = ft.Signature
            };
            scanner.EnqueueUserProcedure(up);

            return false;
        }

        public bool VisitPrimitive(PrimitiveType pt)
        {
            rdr.Read(pt);
            return false;
        }

        public bool VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public bool VisitPointer(Pointer ptr)
        {
            var c = rdr.Read(PrimitiveType.Create(Domain.Pointer, ptr.Size));
            var addr = Address.FromConstant(c);

            if (!program.Image.IsValidAddress(addr))
                return false;

            scanner.EnqueueUserGlobalData(addr, ptr.Pointee);

            return false;
        }

        public bool VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public bool VisitStructure(StructureType str)
        {
            var structOffset = rdr.Offset;
            for (int i = 0; i < str.Fields.Count; ++i)
            {
                rdr.Offset = structOffset + str.Fields[i].Offset;
                str.Fields[i].DataType.Accept(this);
            }
            rdr.Offset = structOffset + str.GetInferredSize();
            return false;
        }

        public bool VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        public bool VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
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
