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
using Reko.Core.Hll.C;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Scanning
{
    /// <summary>
    /// Abstract base class for scanners that scan a binary image.
    /// </summary>
    public abstract class ScannerBase
    {
        private readonly IEventListener eventListener;
        private readonly Dictionary<Address, UserProcedure> noDecompiledProcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerBase"/> class.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="eventListener"></param>
        public ScannerBase(Program program, IEventListener eventListener)
        {
            this.Program = program;
            this.eventListener = eventListener;
            this.noDecompiledProcs = new Dictionary<Address, UserProcedure>();
        }

        /// <summary>
        /// Program currently being analyzed by the scanner.
        /// </summary>
        public Program Program { get; }

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

        /// <summary>
        /// Returns true if the user has specified that the procedure
        /// should not be decompiled.
        /// </summary>
        /// <param name="addr">Address being probed.</param>
        /// <returns>True if the procedure at that address is not to be 
        /// decompiled.
        /// </returns>
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

        /// <summary>
        /// Attempts to get the user-defined procedure at the given address, if it
        /// is marked as "no-decompile".
        /// </summary>
        /// <param name="addr">Address to probe.</param>
        /// <param name="ep"><see cref="ExternalProcedure"/> representing the 
        /// no-decompile procedure.</param>
        /// <returns>True if a no-decompile procedure was found; false otherwise.
        /// </returns>
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

        /// <summary>
        /// Obsolete. Use <see cref="Warn(Address, string)"/> instead.
        /// </summary>
        public void Warn(Address addr, string message)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(Program, addr), message);
        }

        /// <summary>
        /// Obsolete. Use <see cref="Warn(Address, string, object[])"/> instead.
        /// </summary>
        public void Warn(Address addr, string message, params object[] args)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(Program, addr), message, args);
        }

        /// <summary>
        /// Obsolete. Use <see cref="Error(Address, string, object[])"/> instead.
        /// </summary>
        public void Error(Address addr, string message, params object[] args)
        {
            eventListener.Error(eventListener.CreateAddressNavigator(Program, addr), message, args);
        }
    }
}
