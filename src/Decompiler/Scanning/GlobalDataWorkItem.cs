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
    public class GlobalDataWorkItem : WorkItem, IDataTypeVisitor<bool>
    {
        private IScanner scanner;
        private Program program;
        private Address addr;
        private DataType dt;
        private ImageReader rdr;

        public GlobalDataWorkItem(IScanner scanner, Program program, Address addr, DataType dt)
        {
            this.scanner = scanner;
            this.program = program;
            this.addr = addr;
            this.dt = dt;
            this.rdr = program.CreateImageReader(addr);
        }

        public override void Process()
        {
            try
            {
                dt.Accept(this);
            }
            catch (AddressCorrelatedException aex)
            {
                scanner.Error(aex.Address, aex.Message);
            }
            catch (Exception ex)
            {
                scanner.Error(addr, ex.Message);
            }
        }

        public bool VisitArray(ArrayType at)
        {
            if (at.Length == 0)
            {
                scanner.Warn(addr, "User-specified arrays must have a non-zero size.");
                return false;
            }
            for (int i = 0; i < at.Length; ++i)
            {
                at.ElementType.Accept(this);
            }

            return false;
        }

        public bool VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public bool VisitCode(CodeType c)
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
            for (int i = 0; i < str.Fields.Count; ++i)
            {
                //$BUG: should be paying attention to StructureField.Offset here.
                str.Fields[i].DataType.Accept(this);
            }
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
            throw new NotImplementedException();
        }

        public bool VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}
