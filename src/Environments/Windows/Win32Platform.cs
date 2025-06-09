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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reko.Core.Loading;
using Reko.Core.Code;
using Reko.Core.Machine;
using Reko.Core.Memory;

namespace Reko.Environments.Windows
{
    public class Win32Platform : Platform
	{
        private static readonly HashSet<RegisterStorage> implicitRegs = new HashSet<RegisterStorage>()
        {
            Registers.cs,
            Registers.ss,
            Registers.sp,
            Registers.esp,
            Registers.fs,
            Registers.gs,
            Registers.Top,
        };

        private static readonly HashSet<RegisterStorage> possibleFastcallArgRegs = new()
        {
            Registers.ecx,
            Registers.edx,
        };

        private readonly Dictionary<int, SystemService> services;

        //$TODO: http://www.delorie.com/djgpp/doc/rbinter/ix/29.html int 29 for console apps!
        //$TODO: http://msdn.microsoft.com/en-us/data/dn774154(v=vs.99).aspx

        //$TODO: we need a Win32Base platform, possibly with a Windows base platform, and make this
        // x86-specific.
        public Win32Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "win32")
        {
            var frame = arch.CreateFrame();
            this.services = new Dictionary<int, SystemService>
            {
                {
                    3,
                    new SystemService
                    {
                        SyscallInfo = new SyscallInfo
                        {
                            Vector = 3,
                            RegisterValues = Array.Empty<RegValue>(),
                        },
                        Name = "int3",
                        Signature = FunctionType.Action(Array.Empty<Identifier>()),
                        Characteristics = new ProcedureCharacteristics(),
                    }
                },
                {
                    0x29,
                    new SystemService
                    {
                        SyscallInfo = new SyscallInfo
                        {
                            Vector = 0x29,
                            RegisterValues = Array.Empty<RegValue>()
                        },
                        Name = "__fastfail",
                        Signature = FunctionType.Action(
                            frame.EnsureRegister(Registers.ecx)),
                        Characteristics = new ProcedureCharacteristics
                        {
                            Terminates = true
                        }
                    }
                }
            };
            this.StructureMemberAlignment = 8;
            this.TrashedRegisters = CreateTrashedRegisters();
        }

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            return new Win32Emulator(segmentMap, this, importReferences);
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return implicitRegs.Contains(reg);
        }

        public override bool IsPossibleArgumentRegister(RegisterStorage reg)
        {
            return possibleFastcallArgRegs.Contains(reg);
        }

        public override Storage? PossibleReturnValue(IEnumerable<Storage> storages)
        {
            var eax = storages.FirstOrDefault(
                s => s is RegisterStorage r &&
                     r.Number == Registers.eax.Number);
            return eax;
        }

        private HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            // Win32 preserves, ebx, esi, edi.
            return new HashSet<RegisterStorage>
            {
                Registers.eax,
                Registers.ecx,
                Registers.edx,
                Registers.esp,
                Registers.Top,
            };
        }

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            ccName = ccName?.TrimStart('_') ?? string.Empty; // Default to cdecl (same as empty string)

            switch (ccName)
            {
            case "":
            case "cdecl":
                return new X86CallingConvention(
                    Architecture.WordWidth.Size,
                    Architecture.PointerType.Size,
                    true,
                    false);
            case "stdcall":
            case "stdapi":
                return new X86CallingConvention(
                    Architecture.WordWidth.Size,
                    Architecture.PointerType.Size,
                    false,
                    false);
            case "pascal":
                return new X86CallingConvention(
                    Architecture.WordWidth.Size,
                    Architecture.PointerType.Size,
                    false,
                    true);
            case "thiscall":
                return new ThisCallConvention(
                    Registers.ecx,
                    Architecture.WordWidth.Size);
            case "fastcall":
                return new FastcallConvention(
                    Registers.ecx,
                    Registers.edx,
                    Architecture.WordWidth.Size);
            }
            throw new ArgumentOutOfRangeException(string.Format("Unknown calling convention '{0}'.", ccName));
        }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            var sf = new X86StartFinder(program, addrStart);
            return sf.FindMainProcedure();
        }

        //$REFACTOR: should be loaded from config file.
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
            case CBasicType.WChar_t: return 2;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        //$REFACTOR: should fetch this from config file?
        public override string? GetPrimitiveTypeName(PrimitiveType pt, string language)
        {
            if (language != "C")
                return null;
            switch (pt.Domain)
            {
            case Domain.Character:
                switch (pt.Size)
                {
                case 1: return "char";
                case 2: return "wchar_t";
                }
                break;
            case Domain.SignedInt:
                switch (pt.Size)
                {
                case 1: return "signed char";
                case 2: return "short";
                case 4: return "int";
                case 8: return "__int64";
                }
                break;
            case Domain.UnsignedInt:
                switch (pt.Size)
                {
                case 1: return "unsigned char";
                case 2: return "unsigned short";
                case 4: return "unsigned int";
                case 8: return "unsigned __int64";
                }
                break;
            case Domain.Real:
                switch (pt.Size)
                {
                case 4: return "float";
                case 8: return "double";
                }
                break;
            }
            return null;
        }

        public override Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 1)
                return null;
            if (instrs[^1].Instructions[^1] is not RtlGoto jump)
                return null;
            if (jump.Target is ProcedureConstant pc)
                return new Trampoline(instrs[^1].Address, pc.Procedure);
            if (jump.Target is not MemoryAccess access)
                return null;
            Address? addrTarget;
            if (access.EffectiveAddress is not Address a)
            {
                if (access.EffectiveAddress is not Constant wAddr)
                {
                    return null;
                }
                addrTarget = MakeAddressFromConstant(wAddr, true);
                if (addrTarget is null)
                    return null;
            }
            else
            {
                addrTarget = a;
            }
            ProcedureBase? proc = host.GetImportedProcedure(this.Architecture, addrTarget.Value,  addrInstr);
            if (proc is null)
                proc = host.GetInterceptedCall(this.Architecture, addrTarget.Value);
            if (proc is null)
                return null;
            return new Trampoline(instrs[^1].Address, proc);
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rdr, IRewriterHost host)
        {
            var instr = rdr.FirstOrDefault();
            if (instr is null)
                return null;
            if (instr is not RtlGoto jump)
                return null;
            if (jump.Target is ProcedureConstant pc)
                return pc.Procedure;
            if (jump.Target is not MemoryAccess access)
                return null;
            Address? addrTarget;
            if (access.EffectiveAddress is not Address a)
            {
                if (access.EffectiveAddress is not Constant wAddr)
                {
                    return null;
                }
                addrTarget = MakeAddressFromConstant(wAddr, true);
                if (addrTarget is null)
                    return null;
            }
            else
            {
                addrTarget = a;
            }
            ProcedureBase? proc = host.GetImportedProcedure(this.Architecture, addrTarget.Value, addrInstr);
            if (proc is not null)
                return proc;
            return host.GetInterceptedCall(this.Architecture, addrTarget.Value);
        }

        public override void InjectProcedureEntryStatements(
            Procedure proc,
            Address addr,
            CodeEmitter m)
        {
            m.Assign(proc.Frame.EnsureRegister(Registers.Top), 0);
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            if (moduleName is not null && metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor? mod))
            {
                if (mod.ServicesByName.TryGetValue(procName, out SystemService? svc))
                {
                    var chr = LookupCharacteristicsByName(svc.Name!);
                    return new ExternalProcedure(svc.Name!, svc.Signature!, chr);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (!metadata.Signatures.TryGetValue(procName, out FunctionType? sig))
                    return null;
                var chr = LookupCharacteristicsByName(procName);
                return new ExternalProcedure(procName, sig, chr);
            }
        }

        public override ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            if (!metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor? mod))
                return null;
            if (mod.ServicesByOrdinal.TryGetValue(ordinal, out SystemService? svc))
            {
                var chr = LookupCharacteristicsByName(svc.Name!);
                return new ExternalProcedure(svc.Name!, svc.Signature!, chr);
            }
            else
                return null;
        }

		public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
		{
            if (!services.TryGetValue(vector, out SystemService? svc))
                return null;
            return svc;
		}

        public override string DefaultCallingConvention
        {
            get { return "__cdecl"; }
        }

        public override ProcedureBase_v1? SignatureFromName(string fnName)
        {
            EnsureTypeLibraries(PlatformIdentifier); 
            return SignatureGuesser.SignatureFromName(fnName, this);
        }

        public override (string, SerializedType, SerializedType)? DataTypeFromImportName(string importName)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            var (name, type, outerType) = SignatureGuesser.InferTypeFromName(importName);
            if (name is null)
                return null;
            return (name!, type!, outerType!);
        }
    }
}
