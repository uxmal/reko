#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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
using Reko.Core.CLanguage;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Reko functionality which provided for using in user-defined scripts
    /// </summary>
    public class RekoAPI
    {
        private readonly Program program;

        public RekoAPI(Program program)
        {
            this.program = program;
        }

        public int ReadInt32(ulong linearAddress)
        {
            return CreateImageReader(linearAddress).ReadInt32();
        }

        public string ReadCString(ulong linearAddress)
        {
            var rdr = CreateImageReader(linearAddress);
            var c = rdr.ReadCString(PrimitiveType.Char, program.TextEncoding);
            return c.ToString();
        }

        public void SetUserComment(ulong linearAddress, string comment)
        {
            var addr = Addr(linearAddress);
            program.User.Annotations[addr] = comment;
        }

        public void SetUserGlobal(ulong linearAddress, string decl)
        {
            var addr = Addr(linearAddress);
            var usb = new UserSignatureBuilder(program);
            var global = usb.ParseGlobalDeclaration(decl);
            var name = global?.Name;
            var dataType = global?.DataType;
            if (name is null || dataType is null)
                throw new ArgumentException(
                    $"Failed to parse global variable declaration: '{decl}'");
            program.User.Globals[addr] = new UserGlobal(addr, name, dataType);
        }

        public void SetUserProcedure(ulong linearAddress, string name)
        {
            var addr = Addr(linearAddress);
            program.User.Procedures[addr] = new UserProcedure(addr, name);
        }

        public void SetUserProcedureDecompileFlag(
            ulong linearAddress, bool decompile)
        {
            UserProcedure(linearAddress).Decompile = decompile;
        }

        private Address Addr(ulong linearAddress)
        {
            return program.SegmentMap.MapLinearAddressToAddress(linearAddress);
        }

        private EndianImageReader CreateImageReader(ulong linearAddress)
        {
            var addr = Addr(linearAddress);
            return program.CreateImageReader(program.Architecture, addr);
        }

        private UserProcedure UserProcedure(ulong linearAddress)
        {
            var addr = Addr(linearAddress);
            return program.User.Procedures[addr];
        }
    }
}
