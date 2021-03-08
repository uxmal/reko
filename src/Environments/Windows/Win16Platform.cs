#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Environments.Windows
{
    public class Win16Platform : Platform
    {
        public Win16Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "win16")
        {
        }

        public override string DefaultCallingConvention
        {
            get { return "pascal"; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            // https://devblogs.microsoft.com/oldnewthing/20071203-00/?p=24323
/*
AX	zero (used to contain even geekier information in Windows 2)
BX	stack size
CX	heap size
DX	unused (reserved)
SI	previous instance handle
DI	instance handle
BP	zero (for stack walking)
DS	application data segment
ES	selector of program segment prefix
SS	application data segment (SS=DS)
SP	top of stack
*/
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.ss, Registers.ds, Registers.sp, Registers.Top,
            };
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.ax,
                Registers.cx,
                Registers.dx,
                Registers.bx,
                Registers.sp,
                Registers.Top,
            };
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            if (ccName == null)
                ccName = "";

            switch (ccName)
            {
            case "":
            case "__cdecl":
            case "cdecl":
                return new X86CallingConvention(4, 2, 4, true, false);
            case "__pascal":
            case "pascal":
                return new X86CallingConvention(4, 2, 4, false, true);
            case "__stdcall":
            case "stdcall":
                return new X86CallingConvention(4, 4, 4, false, false);
            }
            throw new NotSupportedException(string.Format("Calling convention '{0}' is not supported.", ccName));
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 16;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            m.Assign(proc.Frame.EnsureRegister(Registers.Top), 0);
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            return null;
        }

        public override ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            foreach (var tl in Metadata.Modules.Values.Where(t => string.Compare(t.ModuleName, moduleName, true) == 0))
            {
                if (tl.ServicesByOrdinal.TryGetValue(ordinal, out SystemService svc))
                {
                    return new ExternalProcedure(svc.Name!, svc.Signature!);
                }
            }
            return null;
        }

        public override ProcedureBase_v1? SignatureFromName(string fnName)
        {
            var sig = SignatureGuesser.SignatureFromName(fnName, this);
            return sig;
        }
    }
}
