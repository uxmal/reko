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
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Environments.SysV
{
    //$TODO: rename to Elf-Neutral? Or Posix?
    public class SysVPlatform : Platform
    {
        private RegisterStorage[] trashedRegs;
        private CallingConvention ccX86;
        private CallingConvention ccRiscV;

        public SysVPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "elf-neutral")
        {
            LoadTrashedRegisters();
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            switch (Architecture.Name)
            {
            case "mips-be-32":
            case "mips-le-32":
            case "mips-be-64":
            case "mips-le-64":
                return new MipsCallingConvention(Architecture); //$ ccName?
            case "ppc-be-32":
            case "ppc-le-32":
                return new PowerPcCallingConvention(Architecture);
            case "sparc32":
                return new SparcCallingConvention(Architecture);
            case "x86-protected-32":
                if (this.ccX86 == null)
                {
                    var t = Type.GetType("Reko.Arch.X86.X86CallingConvention,Reko.Arch.X86", true);
                    this.ccX86 = (CallingConvention)Activator.CreateInstance(
                        t,
                        4,      // retAddressOnStack,
                        4,      // stackAlignment,
                        4,      // pointerSize,
                        true,   // callerCleanup,
                        false); // reverseArguments)
                }
                return this.ccX86;
            case "x86-protected-64":
                return new X86_64CallingConvention(Architecture);
            case "xtensa":
                return new XtensaCallingConvention(Architecture);
            case "arm":
                return new Arm32CallingConvention(Architecture);
            case "arm-64":
                return new Arm64CallingConvention(Architecture);
            case "m68k":
                return new M68kCallingConvention(Architecture);
            case "avr8":
                return new Avr8CallingConvention(Architecture);
            case "msp430":
                return new Msp430CallingConvention(Architecture);
            case "risc-v":
                if (this.ccRiscV == null)
                {
                    var t = Type.GetType("Reko.Arch.RiscV.RiscVCallingConvention,Reko.Arch.RiscV", true);
                    this.ccRiscV = (CallingConvention)Activator.CreateInstance(t, Architecture);
                }
                return this.ccRiscV;
            case "superH-le":
            case "superH-be":
                return new SuperHCallingConvention(Architecture);
            case "alpha":
                return new AlphaCallingConvention(Architecture);
            case "zSeries":
                return new zSeriesCallingConvention(Architecture);
            case "blackfin":
                return new BlackfinCallingConvention(Architecture);
            default:
                throw new NotImplementedException(string.Format("ELF calling convention for {0} not implemented yet.", Architecture.Description));
            }
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>(this.trashedRegs);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            return null;
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 1;
            case CBasicType.Char: return 1;
            case CBasicType.WChar_t: return 2;
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

        public override ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> rw, IRewriterHost host)
        {
            var rtlc = rw.FirstOrDefault();
            if (rtlc == null || rtlc.Instructions.Length == 0)
                return null;

            // Match x86 pattern.
            // jmp [destination]
            Address addrTarget = null;
            if (rtlc.Instructions[0] is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return pc.Procedure;
                if (!(jump.Target is MemoryAccess access))
                    return null;
                addrTarget = access.EffectiveAddress as Address;
                if (addrTarget == null)
                {
                    if (!(access.EffectiveAddress is Constant wAddr))
                    {
                        return null;
                    }
                    addrTarget = MakeAddressFromConstant(wAddr, true);
                }
            }
            if (addrTarget == null)
                return null;
            var arch = this.Architecture;
            ProcedureBase proc = host.GetImportedProcedure(arch, addrTarget, rtlc.Address);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(arch, addrTarget);
        }

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            switch (Architecture.Name)
            {
            case "mips-be-32":
            case "mips-le-32":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")), Constant.Word32((uint)addr.ToLinear()));
                break;
            case "mips-be-64":
            case "mips-le-64":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")), Constant.Word64((uint) addr.ToLinear()));
                break;
            case "x86-protected-32":
            case "x86-protected-64":
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.FpuStackRegister),
                    0);
                break;
            case "zSeries":
                // Stack parameters are passed in starting at offset +160 from the 
                // stack; everything at lower addresses is local to the called procedure's
                // frame.
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.GetRegister("r15")),
                    m.ISub(
                        proc.Frame.FramePointer,
                        Constant.Int(proc.Frame.FramePointer.DataType, 160)));
                break;
            }
        }

        private void LoadTrashedRegisters()
        {
            if (Services != null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var pa = cfgSvc.GetEnvironment(this.PlatformIdentifier).Architectures.SingleOrDefault(a => a.Name == Architecture.Name);
                if (pa != null)
                {
                    this.trashedRegs = pa.TrashedRegisters
                        .Select(r => Architecture.GetRegister(r))
                        .Where(r => r != null)
                        .ToArray();
                    return;
                }
            }
            this.trashedRegs = new RegisterStorage[0];
        } 

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$REVIEW: looks a lot like Win32library, perhaps push to parent class?
            EnsureTypeLibraries(PlatformIdentifier);
            var sig = Metadata.Lookup(procName);
            if (sig == null)
                return null;
            var proc = new ExternalProcedure(procName, sig);
            var characteristics = CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c != null)
                .FirstOrDefault();
            if (characteristics != null)
                proc.Characteristics = characteristics;
            return proc;
        }

        public override ProcedureBase_v1 SignatureFromName(string fnName)
        {
            StructField_v1 field = null;
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
            if (field == null)
                return null;
            if (field.Type is SerializedSignature sproc)
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
