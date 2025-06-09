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

using Reko.Core.Hll.C;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Memory;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmPlatform : Platform
    {
        private WasmFile wasmFile;

        public WasmPlatform(
            IServiceProvider services,
            IProcessorArchitecture arch,
            WasmFile wasmFile)
            : base(services, arch, "wasmVM")
        {
            this.wasmFile = wasmFile;
            this.TypeLibraries = new List<TypeLibrary>();
            this.Description = "Web assembly VM";
            this.StructureMemberAlignment = 8;
            this.TrashedRegisters = new HashSet<RegisterStorage>();
        }

        public List<TypeLibrary> TypeLibraries { get; }

        public override string DefaultCallingConvention => "";

        public override IReadOnlySet<RegisterStorage> TrashedRegisters { get; protected set; }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;      // Assume 32-bit int.
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException($"C basic type {cb} not implemented yet.");
            }
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            return TypeLibraries
                .Select(t => t.Lookup(procName))
                .Where(sig => sig is not null)
                .Select(sig => new ExternalProcedure(procName, sig!))
                .FirstOrDefault();
        }

        public static string RenderValueType(int typeIdx)
        {
            switch(typeIdx)
            {
            case 0x7F: return "i32";
            case 0x7E: return "i64";
            case 0x7D: return "f32";
            case 0x7C: return "f64";
            case 0x7B: return "v128";
            }
            throw new NotImplementedException();
        }
    }
}
