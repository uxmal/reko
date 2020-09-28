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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Cpm
{
    public class CpmPlatform : Platform
    {
        private const ushort CPM_VECTOR = 5;

        private Dictionary<Address, DispatchProcedure> dispatchProcedures;

        public CpmPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "cpm")
        {
            this.dispatchProcedures = new Dictionary<Address, DispatchProcedure>();
            EnsureTypeLibraries(this.PlatformIdentifier);
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override MemoryMap_v1 MemoryMap
        {
            get { return base.MemoryMap; }
            set { base.MemoryMap = value; OnMemoryMapChanged(); }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 1;
            case CBasicType.Char: return 1;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 2;
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
            throw new NotImplementedException();
        }

        public override ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            var e = instrs.GetEnumerator();
            if (!e.MoveNext())
                return null;
            if (e.Current.Instructions[0] is RtlGoto g &&
                g.Target is Address addr &&
                this.dispatchProcedures.TryGetValue(addr, out var disp))
            {
                return disp;
            }
            return null;
        }

        private void OnMemoryMapChanged()
        {
            //var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
            var tser = new TypeLibraryDeserializer(this, true, Metadata);
            var sser = new ProcedureSerializer(this, tser, DefaultCallingConvention);
            var disps = new Dictionary<Address, DispatchProcedure>();
            foreach (var callable in this.MemoryMap.Segments.SelectMany(s => s.Procedures))
            {
                if (callable is DispatchProcedure_v1 sDisp)
                {
                    if (sDisp.Services == null)
                        continue;
                    var svcs = sDisp.Services
                        .Where(s => s.SyscallInfo != null)
                        .Select(s => (
                            s.SyscallInfo.Build(this),
                            new ExternalProcedure(
                                s.Name,
                                sser.Deserialize(s.Signature, Architecture.CreateFrame()))))
                        .ToList();
                    if (Architecture.TryParseAddress(sDisp.Address, out var addr))
                    {
                        var disp = new DispatchProcedure(
                            sDisp.Name,
                            svcs);
                        disps.Add(addr, disp);
                    }
                }
            }
            this.dispatchProcedures = disps;
        }
    }
}
