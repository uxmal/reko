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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Hll.C;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Environments.SysV
{
    //$TODO: rename to Elf-Neutral? Or Posix?
    public class SysVPlatform : Platform
    {
        private readonly ArchSpecificFactory archSpecificFactory;
        private readonly ICallingConvention? defaultCc;

        public SysVPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "elf-neutral")
        {
            archSpecificFactory = new ArchSpecificFactory(services, arch);
            this.defaultCc = archSpecificFactory.CreateCallingConvention(arch, "");
            //$REVIEW: examine this carefully! It may well be arch-dependent
            this.StructureMemberAlignment = 8;
        }

        public override string DefaultCallingConvention => "";

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.GccKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override ICallingConvention? DetermineCallingConvention(FunctionType signature, IProcessorArchitecture? arch)
        {
            return base.DetermineCallingConvention(signature, arch);
        }

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            var cc = archSpecificFactory.CreateCallingConvention(this.Architecture, ccName);
            if (cc is null)
                throw new NotImplementedException($"ELF calling convention for {Architecture.Description} not implemented yet.");
            return cc;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            foreach (var module in metadata.Modules.Values)
            {
                if (module.ServicesByVector.TryGetValue(vector, out var svcs))
                {
                    return svcs.FirstOrDefault(s => 
                        s.SyscallInfo is not null && 
                        s.SyscallInfo.Matches(vector, state));
                }
            }
            return null;
        }

        public override Constant? FindGlobalPointerValue(Program program, Address addrStart)
        {
            var arch = program.Architecture;
            var finder = archSpecificFactory.CreateGlobalPointerFinder(arch);
            if (finder is null)
                return null;
            if (!program.TryCreateImageReader(addrStart, out var rdr))
                return null;
            var rw = program.Architecture.CreateRewriter(
                rdr,
                arch.CreateProcessorState(),
                new StorageBinder(),
                new NullRewriterHost());
            return finder(rw);
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
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble:
                if (Architecture is Reko.Arch.X86.IntelArchitecture)
                {
                    // According to section 3.1.2 "Data representation" in 
                    // System V Application Binary Interface, AMD64 Architecture
                    // Processor Supplement. __float128 is a different beast.
                    return 80;
                }
                else
                {
                    //$TODO: determine what this is for each ELF platform.
                    return 64;
                }
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> rw, IRewriterHost host)
        {
            var finder = archSpecificFactory.CreateTrampolineDestinationFinder(this.Architecture);
            var (target, addrStub) = finder(Architecture, addrInstr, rw, host);
            if (target is ProcedureConstant pc)
            {
                return new Trampoline(addrStub!, pc.Procedure);
            }
            else if (target is Address addrTarget)
            {
                var arch = this.Architecture;
                ProcedureBase? proc = host.GetImportedProcedure(arch, addrTarget, addrInstr);
                if (proc is null)
                    proc = host.GetInterceptedCall(arch, addrTarget);
                if (proc is null)
                    return null;
                return new Trampoline(addrStub!, proc);
            }
            else
                return null;
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rw, IRewriterHost host)
        {
            var finder = archSpecificFactory.CreateTrampolineDestinationFinderOld(this.Architecture);
            var target = finder(Architecture, addrInstr, rw, host);
            if (target is ProcedureConstant pc)
            {
                return pc.Procedure;
            }
            else if (target is Address addrTarget)
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

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            switch (Architecture.Name)
            {
            case "arm":
            case "arm-thumb":
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.GetRegister("lr")!),
                    proc.Frame.Continuation);
                break;
            case "mips-be-32":
            case "mips-le-32":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")!), Constant.Word32((uint)addr.ToLinear()));
                break;
            case "mips-be-64":
            case "mips-le-64":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")!), Constant.Word64((uint) addr.ToLinear()));
                break;
            case "x86-protected-32":
            case "x86-protected-64":
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.FpuStackRegister!),
                    0);
                break;
            case "zSeries":
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.GetRegister("r14")!),
                    proc.Frame.Continuation);

                // Stack parameters are passed in starting at offset +160 from the 
                // stack; everything at lower addresses is local to the called procedure's
                // frame.
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.GetRegister("r15")!),
                    m.ISub(
                        proc.Frame.FramePointer,
                        Constant.Int(proc.Frame.FramePointer.DataType, 160)));
                break;
            }
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            if (base.IsImplicitArgumentRegister(reg))
                return true;
            return reg.IsSystemRegister;
        }

        public override bool IsPossibleArgumentRegister(RegisterStorage reg)
        {
            if (defaultCc is null)
                return false;
            return defaultCc.IsArgument(reg);
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            if (options.TryGetValue("osabi", out var oOsAbi) &&
                oOsAbi is string osAbi &&
                string.Compare(osAbi, "linux", StringComparison.OrdinalIgnoreCase) == 0)
            {
            }
            base.LoadUserOptions(options);
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

        public override Storage? PossibleReturnValue(IEnumerable<Storage> storages)
        {
            if (defaultCc is null)
                return null;
            var retStorage = storages.FirstOrDefault(
                s => defaultCc.IsOutArgument(s));
            return retStorage;
        }

        public override ProcedureBase_v1? SignatureFromName(string fnName)
        {
            StructField_v1? field;
            try
            {
                var gcc = new GccMangledNameParser(fnName, this.PointerType.Size);
                field = gcc.Parse();
            }
            catch (Exception ex)
            {
                Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                return null;
            }
            if (field is not null && field.Type is SerializedSignature sproc)
            {
                return new Procedure_v1
                {
                    Name = field.Name,
                    Signature = sproc,
                };
            }
            return null;
        }
    }
}
