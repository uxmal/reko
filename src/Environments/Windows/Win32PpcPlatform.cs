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

using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    public class Win32PpcPlatform : Platform
    {
        public Win32PpcPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch, "winPpc")
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>
            {
            };
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            return new PowerPcCallingConvention(this.Architecture);
        }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
                "Win32 PowerPC main procedure finder not supported.");
            return null;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rtls, IRewriterHost host)
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
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor mod))
                return null;
            if (mod.ServicesByOrdinal.TryGetValue(ordinal, out SystemService svc))
            {
                return new ExternalProcedure(svc.Name!, svc.Signature!);
            }
            else
                return null;
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            if (moduleName is null)
                return null;
            EnsureTypeLibraries(PlatformIdentifier);
            if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor mod))
                return null;
            if (mod.ServicesByName.TryGetValue(moduleName, out SystemService svc))
            {
                return new ExternalProcedure(svc.Name!, svc.Signature!);
            }
            else
                throw new NotImplementedException();
        }
    }
}
