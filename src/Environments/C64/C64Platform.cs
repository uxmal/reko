#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using Reko.Core.Loading;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Implementation of the C64 platform.
    /// </summary>
    public class C64Platform : Platform
    {
        public C64Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "c64")
        {
            this.StructureMemberAlignment = 1;
            this.TrashedRegisters = new HashSet<RegisterStorage>
            {
                Registers.a
            };
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            return new C64Emulator(segmentMap, importReferences);
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return reg == Registers.s;
        }

        public override SystemService FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
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

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }
    }
}
