#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Output
{
    /// <summary>
    /// Writes out global initialized data.
    /// </summary>
    public class GlobalDataWriter : IDataTypeVisitor<CodeFormatter>
    {
        private Program program;
        private ImageReader rdr;
        private CodeFormatter codeFormatter;
        private StructureType globals;
        private IServiceProvider services;

        public GlobalDataWriter(Program program, IServiceProvider services)
        {
            this.program = program;
            this.services = services;
        }

        public void WriteGlobals(Formatter formatter)
        {
            this.codeFormatter = new CodeFormatter(formatter);
            var tw = new TypeReferenceFormatter(formatter, true);
            this.globals = (StructureType)((EquivalenceClass) ((Pointer)program.Globals.TypeVariable.DataType).Pointee).DataType;
            foreach (var field in globals.Fields)
            {
                var name = string.Format("g_{0:X}", field.Name);
                var addr = Address.Ptr32((uint) field.Offset);  //$BUG: this is completely wrong; offsets should be as wide as the platform permits.
                try
                {
                    tw.WriteDeclaration(field.DataType, name);
                    if (program.Image.IsValidAddress(addr))
                    {
                        formatter.Write(" = ");
                        this.rdr = program.CreateImageReader(addr);
                        field.DataType.Accept(this);
                    }
                }
                catch (Exception ex)
                {
                    var dc = services.RequireService<DecompilerEventListener>();
                    dc.Error(
                        dc.CreateAddressNavigator(program, addr),
                        ex,
                        string.Format("Failed to write global variable {0}.", name));
                }
                formatter.Terminate(";");
            }
        }

        public CodeFormatter VisitArray(ArrayType at)
        {
            Debug.Assert(at.Length != 0, "Expected sizes of arrays to have been determined by now");
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;
            
            for (int i = 0; i < at.Length; ++i)
            {
                var r = rdr.Clone();
                fmt.Indent();
                at.ElementType.Accept(this);
                fmt.Terminate(",");
                r.Offset += (uint) at.ElementType.Size;
                rdr = r;
            }

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return codeFormatter;
        }

        public CodeFormatter VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        private int guard;
        public CodeFormatter VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (guard > 100)
            { codeFormatter.InnerFormatter.WriteComment("Recursion too deep"); return codeFormatter; }
            else
            {
                if (eq.DataType != null)
                {
                    //$TODO: this should go away once we figure out why type inference loops.
                    ++guard;
                    var cf = eq.DataType.Accept(this);
                    --guard;
                    return cf;
                } else
                {
                    Debug.Print("WARNING: eq.DataType is null for {0}", eq.Name);
                    return codeFormatter;
                }
            }
        }

        public CodeFormatter VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitPrimitive(PrimitiveType pt)
        {
            rdr.Read(pt).Accept(codeFormatter);
            return codeFormatter;
        }

        public CodeFormatter VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitPointer(Pointer ptr)
        {
            var c = rdr.Read(PrimitiveType.Create(Domain.Pointer, ptr.Size));
            int offset = c.ToInt32();
            if (offset == 0)
            {
                codeFormatter.WriteNull();
            }
            else
            {
                var field = globals.Fields.AtOffset(offset);
                if (field == null)
                    throw new NotImplementedException("Drill into struct");
                codeFormatter.InnerFormatter.Write("&g_{0}", field.Name);
            }
            return codeFormatter;
        }

        public CodeFormatter VisitString(StringType str)
        {
            var s = rdr.ReadCString(str.ElementType, Encoding.UTF8);    //$BUG: should get this from platform / user-setting.
            var fmt = codeFormatter.InnerFormatter;
            fmt.Write('"');
            foreach (var ch in s.ToString())
            {
                if (Char.IsControl(ch))
                {
                    fmt.Write("\\x{0:X2}", (int) ch);
                }
                else
                {
                    if (ch == '\\' || ch == '"')
                        fmt.Write(ch);
                    fmt.Write(ch);
                }
            }
            fmt.Write('"');
            return codeFormatter;
        }

        public CodeFormatter VisitStructure(StructureType str)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            for (int i = 0; i < str.Fields.Count; ++i)
            {
                fmt.Indent();
                str.Fields[i].DataType.Accept(this);
                fmt.Terminate(",");
            }

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return codeFormatter;
        }

        public CodeFormatter VisitTypeReference(TypeReference typeref)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.WriteType(typeref.Name, typeref);
            return codeFormatter;
        }

        public CodeFormatter VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}
