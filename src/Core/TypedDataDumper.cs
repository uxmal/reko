#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Output;
using Reko.Core.Types;

namespace Reko.Core
{
    public class TypedDataDumper : IDataTypeVisitor
    {
        private ImageReader rdr;
        private Formatter stm;

        public TypedDataDumper(ImageReader rdr, Formatter stm) 
        {
            this.rdr = rdr;
            this.stm = stm;
        }

        public void VisitArray(ArrayType at)
        {
            throw new NotImplementedException();
        }

        public void VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public void VisitCode(CodeType c)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public void VisitPointer(Pointer ptr)
        {
            switch (ptr.Size)
            {
            case 2:
                stm.WriteKeyword("dw");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X4}", rdr.ReadByte()));
                stm.WriteLine();
                return;
            case 4:
                stm.WriteKeyword("dd");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X8}", rdr.ReadUInt32()));
                stm.WriteLine();
                return;
            case 8:
                stm.WriteKeyword("dq");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X16}", rdr.ReadUInt32()));
                stm.WriteLine();
                return;
            }
        }

        public void VisitPrimitive(PrimitiveType pt)
        {
            switch (pt.Size)
            {
            case 1:
                stm.WriteKeyword("db");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X2}", rdr.ReadByte()));
                stm.WriteLine();
                return;
            case 2:
                stm.WriteKeyword("dw");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X4}", rdr.ReadByte()));
                stm.WriteLine();
                return;
            case 4:
                stm.WriteKeyword("dd");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X8}", rdr.ReadUInt32()));
                stm.WriteLine();
                return;
            case 8:
                stm.WriteKeyword("dq");
                stm.Write("\t");
                stm.Write(string.Format("0x{0:X16}", rdr.ReadUInt32()));
                stm.WriteLine();
                return;
            }
            throw new NotImplementedException();
        }

        public void VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
        }

        public void VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public void VisitStructure(StructureType str)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}