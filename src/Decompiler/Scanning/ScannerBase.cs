#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public abstract class ScannerBase
    {
        private DecompilerEventListener eventListener;
        private Dictionary<Address, Procedure_v1> noDecompiledProcs;
 
        public ScannerBase(Program program, DecompilerEventListener eventListener)
        {
            this.Program = program;
            this.eventListener = eventListener;
            this.noDecompiledProcs = new Dictionary<Address, Procedure_v1>();
        }

        public Program Program { get; private set; }

        private bool TryGetNoDecompiledProcedure(Address addr, out Procedure_v1 sProc)
        {
            if (!Program.User.Procedures.TryGetValue(addr, out sProc) ||
                sProc.Decompile)
            {
                sProc = null;
                return false;
            }
            return true;
        }

        protected bool IsNoDecompiledProcedure(Address addr)
        {
            return TryGetNoDecompiledProcedure(addr, out Procedure_v1 sProc);
        }

        private bool TryGetNoDecompiledParsedProcedure(Address addr, out Procedure_v1 parsedProc)
        {
            if (!TryGetNoDecompiledProcedure(addr, out Procedure_v1 sProc))
            {
                parsedProc = null;
                return false;
            }
            if (noDecompiledProcs.TryGetValue(addr, out parsedProc))
                return true;
            parsedProc = new Procedure_v1()
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
            if (procDecl == null)
            {
                Warn(addr, "The user-defined procedure signature at address {0} could not be parsed.", addr);
                return true;
            }
            parsedProc.Signature = procDecl.Signature;
            return true;
        }

        protected bool TryGetNoDecompiledProcedure(Address addr, out ExternalProcedure ep)
        {
            if (!TryGetNoDecompiledParsedProcedure(addr, out Procedure_v1 sProc))
            {
                ep = null;
                return false;
            }
            var ser = Program.CreateProcedureSerializer();
            var sig = ser.Deserialize(
                sProc.Signature,
                Program.Architecture.CreateFrame());
            ep = new ExternalProcedure(sProc.Name, sig);
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
