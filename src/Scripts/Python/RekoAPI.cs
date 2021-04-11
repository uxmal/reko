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
using System.Linq;

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

        //$TODO: Make it work with segmented addresses. The API should accept
        // Python ints and strs as parameters, and use
        // IProcessorArchitecture.TryParseAddress so that segmented addresses
        // are handled correctly.
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
            program.User.Annotations[addr] = NormalizeLineEndings(comment);
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

        public string[] GetProcedureAddresses()
        {
            return program.User.Procedures.Keys.Select(
                addr => addr.ToString()).ToArray();
        }

        public bool ContainsProcedureAddress(ulong linearAddress)
        {
            if (!TryConvertAddress(linearAddress, out var addr))
                return false;
            return program.User.Procedures.ContainsKey(addr);
        }

        public string GetProcedureName(ulong linearAddress)
        {
            return UserProcedure(linearAddress).Name;
        }

        public void SetUserProcedure(ulong linearAddress, string name)
        {
            var addr = Addr(linearAddress);
            program.User.Procedures[addr] = new UserProcedure(addr, name);
        }

        public bool GetProcedureDecompileFlag(ulong linearAddress)
        {
            return UserProcedure(linearAddress).Decompile;
        }

        public void SetUserProcedureDecompileFlag(
            ulong linearAddress, bool decompile)
        {
            UserProcedure(linearAddress).Decompile = decompile;
        }

        private bool TryConvertAddress(ulong linearAddress, out Address addr)
        {
            try
            {
                addr = Addr(linearAddress);
                return true;
            }
            catch
            {
                addr = null!;
                return false;
            }
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

        private string NormalizeLineEndings(string s)
        {
            // Python line endings differ from C#.
            // We should normalize them.
            return s.
                Replace(Environment.NewLine, "\n").
                Replace("\n", Environment.NewLine);
        }
    }
}
