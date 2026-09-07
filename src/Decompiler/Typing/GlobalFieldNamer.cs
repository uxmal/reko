#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Text;

namespace Reko.Typing;

/// <summary>
/// Names global fields that are pointers to strings by "mangling"
/// </summary>
public class GlobalFieldNamer
{
    private readonly TypeStore store;
    private readonly IReadOnlyProgram program;
    private readonly IProcessorArchitecture arch;
    private readonly int cIdentifierMax;

    /// <summary>
    /// Crerates an instance of the <see cref="GlobalFieldNamer"/>.
    /// </summary>
    /// <param name="store"><see cref="TypeStore"/> containing structure types.</param>
    /// <param name="program">Program being analyzed.</param>
    /// <param name="cIdentifierMax">Maximum number of characters extracted for strings.
    /// </param>
    public GlobalFieldNamer(TypeStore store, IReadOnlyProgram program, int cIdentifierMax)
    {
        this.store = store;
        this.program = program;
        this.cIdentifierMax = cIdentifierMax;
        this.arch = program.Platform.Architecture;

    }

    /// <summary>
    /// Name global fields that are pointers to strings by "mangling"
    /// their names.
    /// </summary>
    public void NameGlobalFields()
    {
        var globals = this.GlobalVars(program.Globals, store);
        if (globals is null)
            return;
        foreach (var field in globals.Fields)
        {
            if (field.IsNameSet)
                continue;
            string? name = NameField(field);
            if (name is not null)
            {
                field.Name = name;
            }
        }
    }

    private string? NameField(StructureField field)
    {
        var addr = Address.Create(arch.PointerType, (ulong) field.Offset);

        var ptr = field.DataType.ResolveAs<PointerType>();
        if (ptr is not null)
        {
            if (ptr.Pointee is PrimitiveType ptChar &&
                ptChar.Domain == Domain.Character)
            {
                if (!arch.TryReadDataAddress(program.Memory, addr, out var addrStringData))
                    return null;
                return MakeFieldName(field, addrStringData, ptChar);
            }
            return null;
        }
        var str = field.DataType.ResolveAs<StringType>();
        if (str is not null)
        {
            return MakeFieldName(field, addr, str.ElementType);
        }
        var array = field.DataType.ResolveAs<ArrayType>();
        if (array is not null && array.ElementType.Domain == Domain.Character)
        {
            return MakeFieldName(field, addr, array.ElementType);
        }
        return null;
    }

    private string? MakeFieldName(
        StructureField field,
        Address addrStringData,
        DataType ptChar)
    {
        if (!program.TryCreateImageReader(arch, addrStringData, out var rdr))
            return null;
        var strData = rdr.ReadCString(ptChar, program.Platform.DefaultTextEncoding);
        var prefix = program.NamingPolicy.GlobalName(field);
        var fieldName = MakeFieldNameFromString(prefix, strData, program);
        return fieldName;
    }

    /// <summary>
    /// Copy of <see cref="TypedConstantRewriter.GlobalVars"/>
    /// </summary>
    private StructureType? GlobalVars(Identifier globals, TypeStore store)
    {
        if (globals is not null)
        {
            if (store.TryGetTypeVariable(globals, out var tvGlobals))
            {
                if (tvGlobals.DataType is PointerType pGlob)
                {
                    return pGlob.Pointee.ResolveAs<StructureType>();
                }
            }
            if (globals.DataType is PointerType pGlob2)
            {
                return pGlob2.Pointee.ResolveAs<StructureType>();
            }
        }
        return null;
    }


    private string? MakeFieldNameFromString(
        string prefix,
        StringConstant strData,
        IReadOnlyProgram program)
    {
        var sb = new StringBuilder(prefix);
        sb.Append('_');
        int cNonAscii = 0;
        int cAdded = 0;
        foreach (var ch in strData.Literal)
        {
            char c = ch;
            if (c != '_' && !char.IsAsciiLetter(c)
                && (sb.Length == 0 || !char.IsAsciiDigit(c)))
            {
                c = '_';
                ++cNonAscii;
                if (cAdded == 0)
                    continue;
            }
            sb.Append(c);
            ++cAdded;
            if (cAdded > this.cIdentifierMax)        //$REVIEW: add a user toggle for this?
                break;
        }
        if (cAdded == 0 || cNonAscii > strData.Literal.Length / 3)
            return null;
        return sb.ToString();
    }
}
