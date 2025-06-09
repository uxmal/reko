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

using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Environments.MacOS.OSX.ArchSpecific;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.MacOS.OSX
{
    public class MacOSXPlatform : Platform
    {
        private readonly ArchSpecificHandler archHandler;

        public MacOSXPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "macOsX")
        {
            archHandler = ArchSpecificHandler.Create(arch);
            this.StructureMemberAlignment = arch.WordWidth.Size;    //$REVIEW: correct?
            this.TrashedRegisters = GenerateTrashedRegisters(arch);
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override SystemService FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            //$REVIEW: it seems this sort of data should be in the reko.config file.
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 64;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble:
                if (Architecture.Name.StartsWith("x86") &&
                    Architecture.WordWidth.BitSize == 32)
                    return 80;
                else
                    return 64;      //$REVIEW: should this be 128?
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ICallingConvention? GetCallingConvention(string? ccName)
        {
            return this.archHandler.GetCallingConvention(ccName);
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            //$REVIEW: looks a lot like Win32library, perhaps push to parent class?
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            var sig = metadata.Lookup(procName);
            if (sig is null)
                return null;
            var proc = new ExternalProcedure(procName, sig);
            var characteristics = CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c is not null)
                .FirstOrDefault();
            if (characteristics is not null)
                proc.Characteristics = characteristics;
            return proc;
        }

        public override Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            var target = archHandler.GetTrampolineDestination(addrInstr, instrs, host);
            if (target is Address addrTarget)
            {
                var arch = this.Architecture;
                ProcedureBase? proc = host.GetImportedProcedure(arch, addrTarget, addrInstr);
                if (proc is null)
                    proc = host.GetInterceptedCall(arch, addrTarget);
                if (proc is null)
                    return null;
                return new Trampoline(addrInstr, proc);
            }
            else
                return null;
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var target = archHandler.GetTrampolineDestination(addrInstr, instrs, host);
            if (target is Address addrTarget)
            {
                var arch = this.Architecture;
                ProcedureBase? proc = host.GetImportedProcedure(arch, addrTarget, addrInstr);
                if (proc is not null)
                    return proc;
                return host.GetInterceptedCall(arch, addrTarget);
            }
            else
                return null;
        }

        private HashSet<RegisterStorage> GenerateTrashedRegisters(IProcessorArchitecture arch)
        {
            switch (arch.Name)
            {
            case "arm-64":
                // ARM64 ABI defines registers r19-r29 and SP as callee-save.
                return Enumerable.Range(0, 32)
                    .Where(n => n < 19 || n == 30)
                    .Select(n => arch.GetRegister(n + StorageDomain.Register, new BitRange(0, 64))!)
                    .ToHashSet();
            case "x86-protected-64":
                return new[] {
                    "rax","rcx","rdx","rsp","rsi","rdi","r8","r9","r10","r11"
                }
                .Select(s => arch.GetRegister(s)!)
                .ToHashSet();

            }
            throw new NotImplementedException();
        }

    }
}
