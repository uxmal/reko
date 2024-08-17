#region License
/*
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Core.Output
{
    public class TypedDataDumper : IDataTypeVisitor
    {
        private readonly EndianImageReader rdr;
        private readonly uint cbSize;
        private readonly Formatter fmt;

        public TypedDataDumper(EndianImageReader rdr, uint cbSize, Formatter stm)
        {
            this.rdr = rdr;
            this.cbSize = cbSize;
            this.fmt = stm;
        }

        public void VisitArray(ArrayType at)
        {
            var addrEnd = rdr.Address + cbSize;

            if (at.ElementType.Domain < Domain.Composite) 
            {
                int offset = 0;
                bool clean = true;
                for (int i = 0; at.IsUnbounded || i < at.Length; ++i)
                {
                    if (!rdr.IsValid || addrEnd <= rdr.Address) 
                    {
                        return;
                    }
                    if (offset % 16 == 0) 
                    {
                        if (!clean) 
                        {
                            fmt.WriteLine();
                            fmt.Write("        ");
                            clean = true;
                        }
                        switch (at.ElementType.Size)
                        {
                        default:
                        case 1:
                            fmt.WriteKeyword("db");
                            fmt.Write("\t");
                            break;
                        case 2:
                            fmt.WriteKeyword("dw");
                            fmt.Write("\t");
                            break;
                        case 4:
                            fmt.WriteKeyword("dd");
                            fmt.Write("\t");
                            break;
                        case 8:
                            fmt.WriteKeyword("dq");
                            fmt.Write("\t");
                            break;
                        }
                    }
                    if (!clean) 
                    {
                        fmt.Write(",");
                    }
                    switch (at.ElementType.Size)
                    {
                    default:
                    case 1:
                        byte b = rdr.ReadByte();
                        if (at.ElementType.Domain == Domain.Character && 0x20 <= b && b < 0x7F) 
                        {
                            fmt.Write(string.Format("'{0}'", (char)b));
                        }
                        else
                        {
                            fmt.Write(string.Format("0x{0:X2}", b));
                        }
                        offset += 1;
                        break;
                    case 2:
                        fmt.Write(string.Format("0x{0:X4}", rdr.ReadUInt16()));
                        offset += 2;
                        break;
                    case 4:
                        fmt.Write(string.Format("0x{0:X8}", rdr.ReadUInt32()));
                        offset += 4;
                        break;
                    case 8:
                        fmt.Write(string.Format("0x{0:X16}", rdr.ReadUInt64()));
                        offset += 8;
                        break;
                    }
                    clean = false;
                }
                if (!clean) {
                    fmt.WriteLine();
                    clean = true;
                }
            }
            else {
                bool clean = true;
                for (int i = 0; at.IsUnbounded || i < at.Length; ++i)
                {
                    if (!rdr.IsValid || addrEnd <= rdr.Address)
                        return;
                    
                    if (!clean) {
                        fmt.Write("        ");
                        clean = true;
                    }
                    at.ElementType.Accept(this);
                    clean = false;
                }
            }
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
                fmt.WriteKeyword("dw");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X4}", rdr.ReadByte()));
                fmt.WriteLine();
                return;
            case 4:
                fmt.WriteKeyword("dd");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X8}", rdr.ReadUInt32()));
                fmt.WriteLine();
                return;
            case 8:
                fmt.WriteKeyword("dq");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X16}", rdr.ReadUInt64()));
                fmt.WriteLine();
                return;
            }
        }

        public void VisitPrimitive(PrimitiveType pt)
        {
            switch (pt.Size)
            {
            case 1:
                fmt.WriteKeyword("db");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X2}", rdr.ReadByte()));
                fmt.WriteLine();
                return;
            case 2:
                fmt.WriteKeyword("dw");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X4}", rdr.ReadUInt16()));
                fmt.WriteLine();
                return;
            case 4:
                fmt.WriteKeyword("dd");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X8}", rdr.ReadUInt32()));
                fmt.WriteLine();
                return;
            case 8:
                fmt.WriteKeyword("dq");
                fmt.Write("\t");
                fmt.Write(string.Format("0x{0:X16}", rdr.ReadUInt64()));
                fmt.WriteLine();
                return;
            default:
                DumpBytes(pt.Size);
                break;
            }
        }

        private void DumpBytes(int size)
        {
            bool newLine = false;
            fmt.WriteKeyword("db");
            fmt.Write("\t");
            fmt.Write(string.Format("0x{0:X2}", rdr.ReadByte()));
            for (int i = 1; i < size; ++i)
            {
                if (newLine)
                {
                    fmt.WriteLine();
                    fmt.Write("\t");
                    fmt.WriteKeyword("db");
                    fmt.Write("\t");
                    fmt.Write(string.Format("0x{0:X2}", rdr.ReadByte()));
                }
                else
                {
                    fmt.Write(", ");
                    fmt.Write(string.Format("0x{0:X2}", rdr.ReadByte()));
                }
                newLine = (rdr.Address.ToLinear() & 0xF) == 0;
            }
            fmt.WriteLine();
        }

        public void VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
        }

        public void VisitString(StringType str)
        {
            if (str.LengthPrefixType == null)
            {
                if (str.ElementType.Size == 1)
                {
                    fmt.WriteKeyword("db");
                    fmt.Write("\t");
                    bool inStringLiteral = false;
                    string sep = "";
                    int size = str.Length;
                    int i = 0;
                    while (rdr.TryReadByte(out byte b))
                    {
                        //$REVIEW: assumes ASCII.
                        if (0x20 <= b && b < 0x7F)
                        {
                            if (!inStringLiteral)
                            {
                                fmt.Write(sep);
                                sep = ",";
                                fmt.Write('\'');
                                inStringLiteral = true;
                            }
                            fmt.Write((char) b);
                        }
                        else
                        {
                            if (inStringLiteral)
                            {
                                fmt.Write('\'');
                                inStringLiteral = false;
                            }
                            fmt.Write(sep);
                            sep = ",";
                            fmt.Write(string.Format("0x{0:X2}", b));
                            if (b == 0)
                                break;
                        }
                        i++;
                        Debug.Assert(i < size);
                    }
                    if (inStringLiteral)
                    {
                        fmt.Write('\'');
                    }
                    fmt.WriteLine();
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
                long fieldOffset = structOffset + str.Fields[i].Offset;
                WritePadBytes(fieldOffset);
                Debug.Assert(rdr.Offset == fieldOffset);
                fmt.Indent();
                str.Fields[i].DataType.Accept(this);
            }
            WritePadBytes(structOffset + str.MeasureSize());
        }

        private void WritePadBytes(long fieldOffset)
        {
            if (rdr.Offset < fieldOffset)
            {
                // Need padding.
                fmt.Indent();
                fmt.WriteKeyword("db\t");
                var sep = "";
                while (rdr.Offset < fieldOffset)
                {
                    var b = rdr.ReadByte();
                    fmt.Write("{0}0x{1:X2}", sep, b);
                    sep = ",";
                }
                fmt.Write("\t");
                fmt.WriteComment("; padding");
                fmt.WriteLine();
            }
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
            if (ut.Size > 0)
            {
                DumpBytes(ut.Size);
            }
        }

        public void VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}