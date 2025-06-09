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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// Writes out initialized global variables.
    /// </summary>
    public class GlobalDataWriter : IDataTypeVisitor<bool>
    {
        private readonly Program program;
        private readonly IServiceProvider services;
        private readonly DataTypeComparer cmp;
        private readonly CodeFormatter codeFormatter;
        private readonly Formatter formatter;
        private readonly TypeReferenceFormatter tw;
        private readonly bool chasePointers;
        private readonly bool showAddressInComment;
        private Queue<StructureField> queue;
        private StructureType globals;
        private EndianImageReader rdr;
        private int recursionGuard;     //$REVIEW: remove this once deep recursion bugs have been flushed out.

        /// <summary>
        /// Constructs an instance of <see cref="GlobalDataWriter"/>.
        /// </summary>
        /// <param name="program">Program whose globals are being rendered.</param>
        /// <param name="formatter">Output sink.</param>
        /// <param name="chasePointers">If true, follow any pointers encountered
        /// and render them also.</param>
        /// <param name="showAddressInComment">If true, show object addresses in a comment.</param>
        /// <param name="services"><see cref="IServiceProvider"/> interface.</param>
        public GlobalDataWriter(Program program, Formatter formatter, bool chasePointers, bool showAddressInComment, IServiceProvider services)
        {
            this.program = program;
            this.formatter = formatter;
            this.chasePointers = chasePointers;
            this.showAddressInComment = showAddressInComment;
            this.services = services;
            this.cmp = new DataTypeComparer();
            this.codeFormatter = new AbsynCodeFormatter(formatter);
            this.tw = new TypeReferenceFormatter(formatter);
            if (program.TypeStore.TryGetTypeVariable(program.Globals, out var tvGlobals))
            {
                var eqGlobalStruct = tvGlobals.Class;
                if (eqGlobalStruct is not null)
                {
                    var globals = eqGlobalStruct.ResolveAs<StructureType>();
                    if (globals is not null)
                        this.globals = globals;
                    else
                        this.globals = new StructureType();
                }
                else
                {
                    this.globals = new StructureType();
                }
            }
            else
            {
                this.globals = new StructureType();
            }
            this.queue = new Queue<StructureField>(globals.Fields);
            this.rdr = default!;
        }

        /// <summary>
        /// Writes all global variables.
        /// </summary>
        public void Write()
        {
            if (queue.Count == 0)
            {
                Debug.Print("No global variables found.");
                return;
            }
            while (queue.TryDequeue(out var field))
            {
                WriteGlobalVariable(field);
            }
        }

        private void WriteGlobalVariable(StructureField field)
        {
            var name = program.NamingPolicy.GlobalName(field);
            var addr = Address.Ptr32((uint)field.Offset);  //$BUG: this is completely wrong; field.Offsets should be as wide as the platform permits.
            var oneLineDeclaration = IsOneLineDeclaration(field.DataType);
            try
            {
                tw.WriteDeclaration(field.DataType, name);
                if (program.TryCreateImageReader(program.Architecture, addr, out this.rdr!))
                {
                    formatter.Write(" = ");
                    if (!oneLineDeclaration && showAddressInComment)
                    {
                        formatter.Write("// {0}", addr);
                    }
                    field.DataType.Accept(this);
                }
            }
            catch (Exception ex)
            {
                var dc = services.RequireService<IEventListener>();
                dc.Error(
                    dc.CreateAddressNavigator(program, addr),
                    ex,
                    "Failed to write global variable {0}.", name);
                formatter.Terminate(";");
                return;
            }
            if (oneLineDeclaration && showAddressInComment)
            {
                formatter.Write("; // {0}", addr);
                formatter.Terminate();
            }
            else
            {
                formatter.Terminate(";");
            }
        }

        /// <summary>
        /// Writes a global variable.
        /// </summary>
        /// <param name="address">Address of the global variable.</param>
        /// <param name="dataType">Data type of the global variable.</param>
        /// <param name="name">Name of the global variable.</param>
        public void WriteGlobalVariable(Address address, DataType dataType, string name)
        {
            this.globals = new StructureType();
            this.queue = new Queue<StructureField>(globals.Fields);
            var oneLineDeclaration = IsOneLineDeclaration(dataType);
            try
            {
                tw.WriteDeclaration(dataType, name);
                if (program.TryCreateImageReader(program.Architecture, address, out this.rdr!))
                {
                    formatter.Write(" = ");
                    if (!oneLineDeclaration && showAddressInComment)
                    {
                        formatter.Write("// {0}", address);
                    }
                    dataType.Accept(this);
                }
            }
            catch (Exception ex)
            {
                var dc = services.RequireService<IEventListener>();
                dc.Error(
                    dc.CreateAddressNavigator(program, address),
                    ex,
                    "Failed to write global variable {0}.",
                    name);
                formatter.Terminate(";");
                return;
            }
            if (oneLineDeclaration && showAddressInComment)
            {
                formatter.Write("; // {0}", address);
                formatter.Terminate();
            }
            else
            {
                formatter.Terminate(";");
            }
        }

        private bool IsOneLineDeclaration(DataType dataType)
        {
            var dt = dataType.ResolveAs<DataType>();
            return dt switch
            {
                PrimitiveType pt => !IsLargeBlob(pt),
                Pointer _ => true,
                MemberPointer _ => true,
                StringType _ => true,
                VoidType _ => true,
                EquivalenceClass eq => eq.DataType is null || IsOneLineDeclaration(eq.DataType),
                TypeReference tr => tr.Referent is null || IsOneLineDeclaration(tr.Referent),
                CodeType _ => true,
                FunctionType _ => true,
                _ => false,
            };
        }

        /// <inheritdoc/>
        public bool VisitArray(ArrayType at)
        {
            if (at.Length == 0)
            {
                var dc = services.RequireService<IEventListener>();
                dc.Warn(
                    dc.CreateAddressNavigator(program, rdr.Address),
                    "Expected sizes of arrays to have been determined by now");
            }
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            bool ok = true;
            for (int i = 0; i < at.Length; ++i)
            {
                fmt.Indent();
                ok = at.ElementType.Accept(this);
                fmt.Terminate(",");
            }

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return ok;
        }

        /// <inheritdoc/>
        public bool VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitCode(CodeType c)
        {
            codeFormatter.InnerFormatter.Write("<code>");
            return true;
        }

        /// <inheritdoc/>
        public bool VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (recursionGuard > 100)
            { codeFormatter.InnerFormatter.WriteComment("Recursion too deep"); return true; }
            else
            {
                if (eq.DataType is not null)
                {
                    //$TODO: this should go away once we figure out why type inference loops.
                    ++recursionGuard;
                    bool ok = eq.DataType.Accept(this);
                    --recursionGuard;
                    return ok;
                }
                else
                {
                    Debug.Print("WARNING: eq.DataType is null for {0}", eq.Name);
                    return true;
                }
            }
        }

        /// <inheritdoc/>
        public bool VisitFunctionType(FunctionType ft)
        {
            codeFormatter.InnerFormatter.Write("??");
            codeFormatter.InnerFormatter.Write("/* Unexpected function type {0} */ ", ft);
            return true;
        }

        /// <inheritdoc/>
        public bool VisitPrimitive(PrimitiveType pt)
        {
            if (IsLargeBlob(pt))
            {
                var bytes = rdr.ReadBytes(pt.Size);
                FormatRawBytes(bytes);
            }
            else
            {
                // #1279: when array sizes are miscalculated and Reko tries to read nonexistent memory,
                // the error spew is very verbose. Just don't emit anything if bad memory addresses
                // are being generated.
                if (!program.Platform.Architecture.TryRead(rdr, pt, out var cValue))
                    return false;

                cValue.Accept(codeFormatter);
            }
            return true;
        }

        private static bool IsLargeBlob(PrimitiveType pt)
        {
            return pt.Size > 8;
        }

        private void FormatRawBytes(byte[] bytes)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            for (int i = 0; i < bytes.Length;)
            {
                fmt.Indent();
                for (int j = 0; i < bytes.Length && j < 16; ++j, ++i)
                {
                    fmt.Write("0x{0:X2}, ", bytes[i]);
                }
                fmt.Terminate("");
            }
            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
        }

        /// <inheritdoc/>
        public bool VisitMemberPointer(MemberPointer memptr)
        {
            if (!rdr.TryRead(PrimitiveType.Create(Domain.Offset, memptr.BitSize), out var c))
                return false;
            c.Accept(codeFormatter);
            return true;
        }

        /// <inheritdoc/>
        public bool VisitPointer(Pointer ptr)
        {
            if (!rdr.TryRead(PrimitiveType.Create(Domain.Pointer, ptr.BitSize), out var c))
                return false;
            var addr = program.Platform.MakeAddressFromConstant(c, false);
            // Check if it is pointer to function
            if (addr is not null && program.Procedures.TryGetValue(addr.Value, out Procedure proc))
            {
                codeFormatter.InnerFormatter.WriteHyperlink(proc.Name, proc);
                return true;
            }
            int offset = c.ToInt32();
            if (offset == 0)
            {
                codeFormatter.WriteNull();
            }
            else
            {
                var field = globals.Fields.AtOffset(offset);
                if (field is null)
                {
                    // We've discovered a global variable! Create it!
                    //$REVIEW: what about collisions and the usual merge crap?
                    var dt = ptr.Pointee;
                    //$REVIEW: that this is a pointer to a C-style null 
                    // terminated string is a wild-assed guess of course.
                    // It could be a pascal string, or a raw area of bytes.
                    // Depend on user for this, or possibly platform.
                    if (dt.Domain == Domain.Character)
                    {
                        dt = StringType.NullTerminated(dt);
                    }
                    field = globals.Fields.Add(offset, dt);
                    if (chasePointers)
                    {
                        queue.Enqueue(field);
                    }
                }
                codeFormatter.InnerFormatter.Write("&{0}", program.NamingPolicy.GlobalName(field));
            }
            return true;
        }

        /// <inheritdoc/>
        public bool VisitReference(ReferenceTo refTo)
        {
            throw new NotSupportedException("Global variables cannot be references.");
        }

        /// <inheritdoc/>
        public bool VisitString(StringType str)
        {
            var offset = rdr.Offset;
            var s = rdr.ReadCString(str.ElementType, program.TextEncoding);
            //$TODO: appropriate prefix for UTF16-encoded strings.
            codeFormatter.VisitStringConstant(s);
            if (str.Length > 0)
            {
                rdr.Offset = offset + str.Length * str.ElementType.Size;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool VisitStructure(StructureType str)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            var structOffset = rdr.Offset;
            bool ok = true;
            for (int i = 0; ok && i < str.Fields.Count; ++i)
            {
                fmt.Indent();
                rdr.Offset = structOffset + str.Fields[i].Offset;
                ok = str.Fields[i].DataType.Accept(this);
                fmt.Terminate(",");
            }
            rdr.Offset = structOffset + str.GetInferredSize();

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return ok;
        }

        /// <inheritdoc/>
        public bool VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitUnion(UnionType ut)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            // According to http://en.cppreference.com/w/c/language/struct_initialization
            // union initializers use the first member of the union.
            // It may be unstable as the first member may vary from run to run.
            // Try tp pick the "best" alternative.
            var alt = ut.Alternatives.Values.OrderBy(v => v.DataType, cmp).First();

            fmt.Indent();
            bool ok = alt.DataType.Accept(this);
            fmt.Terminate();

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return ok;
        }

        /// <inheritdoc/>
        public bool VisitUnknownType(UnknownType ut)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool VisitVoidType(VoidType voidType)
        {
            // This "can't happen": data can't have void type.
            codeFormatter.InnerFormatter.Write("??void??");
            return true;
        }
    }
}
