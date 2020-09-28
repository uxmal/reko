#region License
/*
 * Copyright (C) 1999-2020 Pavel Tomin.
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
    /// The purpose of this class is to discover interesting global variables.
    /// In particular, we want to discover pointers to procedures in global
    /// data.
    /// </summary>
    public class GlobalDataWorkItem : WorkItem, IDataTypeVisitor
    {
        private readonly IScannerQueue scanner;
        private readonly Program program;
        private readonly DataType dt;
        private readonly EndianImageReader rdr;
        private readonly string name;

        public GlobalDataWorkItem(IScannerQueue scanner, Program program, Address addr, DataType dt, string name) : base(addr)
        {
            this.scanner = scanner;
            this.program = program;
            this.dt = dt;
            this.name = name;
            var arch = program.Architecture;
            this.rdr = program.CreateImageReader(arch, addr);
        }

        public override void Process()
        {
            dt.Accept(this);
        }

        public void VisitArray(ArrayType at)
        {
            if (at.Length <= 0)
            {
                scanner.Warn(Address, "User-specified arrays must have a positive size.");
                return;
            }
            for (int i = 0; i < at.Length; ++i)
            {
                at.ElementType.Accept(this);
            }
        }

        public void VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public void VisitCode(CodeType c)
        {
            program.ImageMap.TerminateItem(rdr.Address);
        }

        public void VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public void VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public void VisitFunctionType(FunctionType ft)
        {
            var addr = rdr.Address;
            //$TODO: if address is odd and we're dealing with an ARM binary,
            // the arch should be arm-thumb.
            scanner.EnqueueUserProcedure(program.Architecture, addr, ft, null);
        }

        public void VisitPrimitive(PrimitiveType pt)
        {
            rdr.Offset += pt.Size;
        }

        public void VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public void VisitPointer(Pointer ptr)
        {
            var c = rdr.Read(PrimitiveType.Create(Domain.Pointer, ptr.BitSize));
            var addr = Address.FromConstant(c);

            if (!program.SegmentMap.IsValidAddress(addr))
                return;

            scanner.EnqueueUserGlobalData(addr, ptr.Pointee, null);
        }

        public void VisitReference(ReferenceTo refTo)
        {
            throw new NotSupportedException("Global variables cannot be references.");
        }

        public void VisitString(StringType str)
        {
            var offsetStart = rdr.Offset;
            var addr = rdr.Address;
            if (str.LengthPrefixType == null)
            {
                // Null terminated string
                if (str.ElementType.Size == 1)
                {
                    // Byte-sized characters
                    while (rdr.TryReadByte(out byte b))
                    {
                        if (b == 0)
                            break;
                    }

                    var len = rdr.Offset - offsetStart;
                    program.ImageMap.AddItemWithSize(
                        addr, new ImageMapItem((uint) len)
                        {
                            Address = addr,
                            DataType = str
                        });
                    return;
                }
            }
            throw new NotImplementedException();
        }

        public void VisitStructure(StructureType str)
        {
            var structOffset = rdr.Offset;
            for (int i = 0; i < str.Fields.Count; ++i)
            {
                rdr.Offset = structOffset + str.Fields[i].Offset;
                str.Fields[i].DataType.Accept(this);
            }
            rdr.Offset = structOffset + str.GetInferredSize();
        }

        public void VisitTypeReference(TypeReference typeref)
        {
            typeref.Referent.Accept(this);
        }

        public void VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public void VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public void VisitUnknownType(UnknownType ut)
        {
        }

        public void VisitVoidType(VoidType voidType)
        {
        }
    }
}
