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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Windows
{
    public class Win_x86_64_Platform : Platform
    {
        private SystemService int29svc;
        private SystemService int3svc;

        public Win_x86_64_Platform(IServiceProvider sp, IProcessorArchitecture arch)
            : base(sp, arch, "win64")
        {
            int3svc = new SystemService
            {
                SyscallInfo = new SyscallInfo
                {
                    Vector = 3,
                    RegisterValues = new RegValue[0],
                },
                Name = "int3",
                Signature = FunctionType.Action(new Identifier[0]),
                Characteristics = new ProcedureCharacteristics(),
            };
            int29svc = new SystemService
            {
                SyscallInfo = new SyscallInfo
                {
                    Vector = 0x29,
                    RegisterValues = new RegValue[0]
                },
                Name = "__fastfail",
                Signature = FunctionType.Action(
                            new Identifier("ecx", PrimitiveType.Word32, Registers.ecx)),
                Characteristics = new ProcedureCharacteristics
                {
                    Terminates = true
                }
            };
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.rsp,
                Registers.Top,
            };
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            // https://msdn.microsoft.com/en-us/library/9z1stfyw.aspx
            return new HashSet<RegisterStorage>
            {
                Registers.rax,
                Registers.rcx,
                Registers.rdx,
                Registers.r8,
                Registers.r9,
                Registers.r10,
                Registers.r11
            };
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            return new X86_64CallingConvention();
        }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
                           "Win32 X86-64 main procedure finder not implemented yet.");
            return null;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            if (vector == 3)
                return int3svc;
            if (vector == 0x29)
                return int29svc;
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

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rw, IRewriterHost host)
        {
            var instr = rw.FirstOrDefault();
            if (instr == null)
                return null;
            if (!(instr is RtlGoto jump))
                return null;
            if (jump.Target is ProcedureConstant pc)
                return pc.Procedure;
            if (!(jump.Target is MemoryAccess access))
                return null;
            var addrTarget = access.EffectiveAddress as Address;
            if (addrTarget == null)
            {
                if (!(access.EffectiveAddress is Constant wAddr))
                {
                    return null;
                }
                addrTarget = MakeAddressFromConstant(wAddr, false);
            }
            ProcedureBase? proc = host.GetImportedProcedure(this.Architecture, addrTarget, addrInstr);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(this.Architecture, addrTarget);
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            ModuleDescriptor mod;
            if (moduleName == null || !Metadata.Modules.TryGetValue(moduleName.ToUpper(), out mod))
                return null;
            SystemService svc;
            if (mod.ServicesByName.TryGetValue(procName, out svc))
            {
                return new ExternalProcedure(svc.Name!, svc.Signature!);
            }
            else
                return null;
        }

        public override ProcedureBase_v1? SignatureFromName(string fnName)
        {
            return SignatureGuesser.SignatureFromName(fnName, this);
        }
    }
}