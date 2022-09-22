#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Hll.C;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Decompiling <see cref="Program"/> functionality which provided for
    /// using in user-defined scripts
    /// </summary>
    public class RekoProgramAPI
    {
        private readonly Program program;

        public RekoProgramAPI(Program program)
        {
            this.program = program;
        }

        public Address Address(ulong linearAddress)
        {
            return program.SegmentMap.MapLinearAddressToAddress(linearAddress);
        }

        public Address Address(string sAddr)
        {
            if (TryConvertAddress(sAddr, out var addr))
                return addr;
            throw new ArgumentException($"Failed to parse address: {sAddr}.");
        }

        public byte ReadByte(Address addr)
        {
            return CreateImageReader(addr).ReadByte();
        }

        public IEnumerable<byte> ReadBytes(Address startAddr, long length)
        {
            return ReadData(rdr => rdr.ReadByte(), startAddr, length);
        }

        public short ReadInt16(Address addr)
        {
            return CreateImageReader(addr).ReadInt16();
        }

        public IEnumerable<short> ReadInts16(Address startAddr, long length)
        {
            return ReadData(rdr => rdr.ReadInt16(), startAddr, length);
        }

        public int ReadInt32(Address addr)
        {
            return CreateImageReader(addr).ReadInt32();
        }

        public IEnumerable<int> ReadInts32(Address startAddr, long length)
        {
            return ReadData(rdr => rdr.ReadInt32(), startAddr, length);
        }

        public long ReadInt64(Address addr)
        {
            return CreateImageReader(addr).ReadInt64();
        }

        public IEnumerable<long> ReadInts64(Address startAddr, long length)
        {
            return ReadData(rdr => rdr.ReadInt64(), startAddr, length);
        }

        public string ReadCString(Address addr)
        {
            var rdr = CreateImageReader(addr);
            var c = rdr.ReadCString(PrimitiveType.Char, program.TextEncoding);
            return c.ToString();
        }

        public void SetUserComment(Address addr, string comment)
        {
            program.User.Annotations[addr] = NormalizeLineEndings(comment);
        }

        public void SetUserGlobal(Address addr, string decl)
        {
            var usb = new UserSignatureBuilder(program);
            var global = usb.ParseGlobalDeclaration(decl);
            var name = global?.Name;
            var dataType = global?.DataType;
            if (name is null || dataType is null)
                throw new ArgumentException(
                    $"Failed to parse global variable declaration: '{decl}'.");
            var arch = program.Architecture;
            program.ModifyUserGlobal(arch, addr, dataType, name);
        }

        public string[] GetProcedureAddresses()
        {
            return program.User.Procedures.Keys.
                Union(program.Procedures.Keys).
                Select(addr => addr.ToString()).ToArray();
        }

        public bool ContainsProcedureAddress(Address addr)
        {
            if (program.User.Procedures.ContainsKey(addr))
                return true;
            if (program.Procedures.ContainsKey(addr))
                return true;
            return false;
        }

        public string GetProcedureName(Address addr)
        {
            return UserProcedure(addr).Name;
        }

        public void SetUserProcedure(Address addr, string decl)
        {
            string name;
            string? CSignature;
            if (UserSignatureBuilder.IsValidCIdentifier(decl))
            {
                name = decl;
                CSignature = null;
            }
            else
            {
                var usb = new UserSignatureBuilder(program);
                var sProc = usb.ParseFunctionDeclaration(decl);
                if (sProc is null || sProc.Name is null)
                    throw new ArgumentException(
                        $"Failed to parse procedure declaration: '{decl}'.");
                name = sProc.Name;
                CSignature = decl;
            }
            program.User.Procedures[addr] = new UserProcedure(addr, name)
            {
                CSignature = CSignature,
            };
            if (program.Procedures.TryGetValue(addr, out var proc))
            {
                proc.Name = name;
            }
        }

        public bool GetProcedureDecompileFlag(Address addr)
        {
            return UserProcedure(addr).Decompile;
        }

        public void SetUserProcedureDecompileFlag(
            Address addr, bool decompile)
        {
            EnsureUserProcedure(addr).Decompile = decompile;
        }

        public string? GetProcedureOutputFile(Address addr)
        {
            return UserProcedure(addr).OutputFile;
        }

        public void SetUserProcedureOutputFile(
            Address addr, string outputFile)
        {
            EnsureUserProcedure(addr).OutputFile = outputFile;
        }

        private bool TryConvertAddress(
            string sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return program.Architecture.TryParseAddress(sAddress, out addr);
        }

        private EndianImageReader CreateImageReader(Address addr)
        {
            return program.CreateImageReader(program.Architecture, addr);
        }

        private IEnumerable<T> ReadData<T>(
            Func<EndianImageReader, T> reader,
            Address startAddr,
            long length)
        {
            var rdr = CreateImageReader(startAddr);
            var offStart = rdr.Offset;
            while (rdr.Offset - offStart < length)
            {
                yield return reader(rdr);
            }
        }

        private UserProcedure UserProcedure(Address addr)
        {
            if (!TryGetUserProcedure(addr, out var userProc))
            {
                if (!TryMakeDefaultUserProcedure(addr, out userProc))
                    throw new ArgumentException(
                        $"There is no procedure at address: {addr}.");
            }
            return userProc;
        }

        private UserProcedure EnsureUserProcedure(Address addr)
        {
            if (!TryGetUserProcedure(addr, out var userProc))
            {
                if (!TryMakeDefaultUserProcedure(addr, out userProc))
                    throw new ArgumentException(
                        $"There is no procedure at address: {addr}.");
                program.User.Procedures[addr] = userProc;
            }
            return userProc;
        }

        /// <summary>
        /// Gets the user procedure definition at specified address.
        /// Returns true if there is user procedure definition at specified
        /// address; otherwise, false
        /// </summary>
        private bool TryGetUserProcedure(
            Address addr, [MaybeNullWhen(false)] out UserProcedure userProc)
        {
            if (!program.User.Procedures.TryGetValue(addr, out userProc))
            {
                userProc = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets default user procedure based on data discovered by scanner.
        /// Returns true if there is procedure discovered by scanner at
        /// specified address; otherwise, false
        /// </summary>
        private bool TryMakeDefaultUserProcedure(
            Address addr, [MaybeNullWhen(false)] out UserProcedure userProc)
        {
            if (!program.Procedures.TryGetValue(addr, out var proc))
            {
                userProc = null;
                return false;
            }
            userProc = new UserProcedure(addr, proc.Name);
            return true;
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
