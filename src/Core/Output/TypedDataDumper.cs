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

using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Core.Output
{
    /// <summary>
    /// Formats typed data as pseudo-assembly language.
    /// </summary>
    public class TypedDataDumper : IDataTypeVisitor
    {
        private readonly EndianImageReader rdr;
        private readonly uint cbSize;
        private readonly Formatter fmt;

        /// <summary>
        /// Creates an instance of the <see cref="TypedDataDumper"/> class.
        /// </summary>
        /// <param name="rdr">Image readed to read from.</param>
        /// <param name="cbSize">Size of the object being dumped.</param>
        /// <param name="formatter">Output sink.</param>
        public TypedDataDumper(EndianImageReader rdr, uint cbSize, Formatter formatter) 
        {
            this.rdr = rdr;
            this.cbSize = cbSize;
            this.fmt = formatter;
        }

        /// <inheritdoc/>
        public void VisitArray(ArrayType at)
        {
            var addrEnd = rdr.Address + cbSize;
            for (int i = 0; at.IsUnbounded || i < at.Length; ++i)
            {
                if (!rdr.IsValid || addrEnd <= rdr.Address)
                    return;
                at.ElementType.Accept(this);
            }
        }

        /// <inheritdoc/>
        public void VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitString(StringType str)
        {
            if (str.LengthPrefixType is null)
            {
                if (str.ElementType.Size == 1)
                {
                    fmt.WriteKeyword("db");
                    fmt.Write("\t");
                    bool inStringLiteral = false;
                    string sep = "";
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
                            }
                            fmt.Write(sep);
                            sep = ",";
                            fmt.Write(string.Format("0x{0:X2}", b));
                            if (b == 0)
                                break;
                        }
                    }
                    fmt.WriteLine();
                    return;
                }
            }
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void VisitTypeReference(TypeReference typeref)
        {
            typeref.Referent.Accept(this);
        }

        /// <inheritdoc/>
        public void VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitUnknownType(UnknownType ut)
        {
            if (ut.Size > 0)
            {
                DumpBytes(ut.Size);
            }
        }

        /// <inheritdoc/>
        public void VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}