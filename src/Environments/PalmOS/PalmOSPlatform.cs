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
using Reko.Core.Memory;
using System;

namespace Reko.Environments.PalmOS
{
    public class PalmOSPlatform : Platform
    {
        public PalmOSPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "palmOS")
        {
        }

        public override string DefaultCallingConvention => "";

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            if (vector != 0xF || state is null)
                return null;
            if (memory is not IByteAdressableMemory mem)
                return null;
            var addrTrapNo = state.InstructionPointer + 2;
            if (!mem.TryReadBeUInt16(addrTrapNo, out ushort trapNo))
                return null;

            return Traps.GetTrapSignature(trapNo);
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;  //$REVIEW: Does PalmOS support wchar_t?
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException($"C basic type {cb} not supported.");
            }
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            return null;
        }
    }
}
