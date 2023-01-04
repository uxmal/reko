#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Serialization;
using Reko.Core.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Scanning
{
    public abstract class ScannerBase
    {
        private readonly DecompilerEventListener eventListener;
        private readonly Dictionary<Address, UserProcedure> noDecompiledProcs;
 
        public ScannerBase(Program program, DecompilerEventListener eventListener)
        {
            this.Program = program;
            this.eventListener = eventListener;
            this.noDecompiledProcs = new Dictionary<Address, UserProcedure>();
        }

        public Program Program { get; private set; }

        private bool TryGetNoDecompiledProcedure(Address addr, [MaybeNullWhen(false)] out UserProcedure proc)
        {
            if (!Program.User.Procedures.TryGetValue(addr, out proc) ||
                proc.Decompile)
            {
                proc = null!;
                return false;
            }
            return true;
        }

        protected bool IsNoDecompiledProcedure(Address addr)
        {
            return TryGetNoDecompiledProcedure(addr, out UserProcedure? _);
        }

        private bool TryGetNoDecompiledParsedProcedure(Address addr, [MaybeNullWhen(false)] out UserProcedure parsedProc)
        {
            if (!TryGetNoDecompiledProcedure(addr, out UserProcedure? sProc))
            {
                parsedProc = null;
                return false;
            }
            if (noDecompiledProcs.TryGetValue(addr, out parsedProc))
                return true;
            parsedProc = new UserProcedure(addr, sProc.Name)
            {
                Name = sProc.Name,
                Signature = new SerializedSignature
                {
                    ParametersValid = false,
                },
            };
            noDecompiledProcs[addr] = parsedProc;
            if (string.IsNullOrEmpty(sProc.CSignature))
            {
                Warn(addr, "The user-defined procedure at address {0} did not have a signature.", addr);
                return true;
            }
            var usb = new UserSignatureBuilder(Program);
            var procDecl = usb.ParseFunctionDeclaration(sProc.CSignature);
            if (procDecl is null)
            {
                Warn(addr, "The user-defined procedure signature at address {0} could not be parsed.", addr);
                return true;
            }
            parsedProc.Signature = procDecl.Signature;
            return true;
        }

        protected bool TryGetNoDecompiledProcedure(Address addr, [MaybeNullWhen(false)] out ExternalProcedure ep)
        {
            if (!TryGetNoDecompiledParsedProcedure(addr, out UserProcedure? sProc))
            {
                ep = null;
                return false;
            }
            var ser = Program.CreateProcedureSerializer();
            var sig = ser.Deserialize(
                sProc.Signature,
                Program.Architecture.CreateFrame());
            ep = new ExternalProcedure(sProc.Name, sig!);
            return true;
        }

        public void Warn(Address addr, string message)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(Program, addr), message);
        }

        public void Warn(Address addr, string message, params object[] args)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(Program, addr), message, args);
        }

        public void Error(Address addr, string message, params object[] args)
        {
            eventListener.Error(eventListener.CreateAddressNavigator(Program, addr), message, args);
        }
    }
}
