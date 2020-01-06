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

using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakePlatform : Platform
    {
        public FakePlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "fake")
        {
            Metadata = new TypeLibrary();
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public Func<HashSet<RegisterStorage>> Test_CreateImplicitArgumentRegisters;
        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return Test_CreateImplicitArgumentRegisters();
        }

        public Func<HashSet<RegisterStorage>> Test_CreateTrashedRegisters;
        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            if (Test_CreateTrashedRegisters == null)
                return new HashSet<RegisterStorage>();
            return Test_CreateTrashedRegisters();
        }

        public Func<string, CallingConvention> Test_GetCallingConvention;
        public override CallingConvention GetCallingConvention(string ccName)
        {
            return Test_GetCallingConvention(ccName);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public string Test_DefaultCallingConvention = "";
        public override string DefaultCallingConvention
        {
            get { return Test_DefaultCallingConvention; }
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 1;
            case CBasicType.Char: return 1;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 4;
            case CBasicType.Long: return 4;
            case CBasicType.LongLong: return 8;
            case CBasicType.Float: return 4;
            case CBasicType.Double: return 8;
            case CBasicType.LongDouble: return 8;
            case CBasicType.Int64: return 8;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            return null;
        }
    }
}

