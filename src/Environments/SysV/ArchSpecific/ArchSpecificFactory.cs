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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class ArchSpecificFactory
    {
        private readonly IServiceProvider services;
        private readonly IProcessorArchitecture arch;
        private ICallingConvention ccX86;
        private ICallingConvention ccRiscV;
        private ICallingConvention cci386KernelAbi;

        public ArchSpecificFactory(IServiceProvider services, IProcessorArchitecture arch)
        {
            this.services = services;
            this.arch = arch;
            this.ccX86 = null!;
            this.ccRiscV = null!;
            this.cci386KernelAbi = default!;
        }

        public Func<IProcessorArchitecture, Address, List<RtlInstructionCluster>, IRewriterHost, (Expression?, Address)> CreateTrampolineDestinationFinder(IProcessorArchitecture arch)
        {
            switch (arch.Name)
            {
            case "arm":
                return TrampolineFinder.Arm32;
            case "arm-64":
                return TrampolineFinder.AArch64;
            case "mips-be-32":
                return TrampolineFinder.Mips32;
            case "ppc-le-64":
            case "ppc-be-64":
                return TrampolineFinder.PowerPC64;
            case "risc-v":
                return TrampolineFinder.RiscV;
            case "x86-protected-32":
                return TrampolineFinder.X86;
            case "x86-protected-64":
                return TrampolineFinder.X86_64;
            default:
                return delegate { return (null,default); };
            }
        } 

        public Func<IProcessorArchitecture, Address, IEnumerable<RtlInstruction>, IRewriterHost, Expression?> CreateTrampolineDestinationFinderOld(IProcessorArchitecture arch)
        {
            switch (arch.Name)
            {
            case "arm":
                return TrampolineFinder.Arm32_Old;
            case "arm-64":
                return TrampolineFinder.AArch64_Old;
            case "ppc-le-64":
            case "ppc-be-64":
                return TrampolineFinder.PowerPC64_Old;
            case "risc-v":
                return TrampolineFinder.RiscV_Old;
            case "x86-protected-32":
                return TrampolineFinder.X86_Old;
            case "x86-protected-64":
                return TrampolineFinder.X86_64_Old;
            default:
                return delegate { return null; };
            }
        } 
 
        public ICallingConvention? CreateCallingConvention(IProcessorArchitecture arch, string? name)
        {
            switch (arch.Name)
            {
            case "mips-be-32":
            case "mips-le-32":
            case "mips-be-64":
            case "mips-le-64":
                return new MipsCallingConvention(arch); //$ ccName?
            case "ppc-be-32":
            case "ppc-le-32":
                return new PowerPcCallingConvention(arch);
            case "ppc-be-64":
            case "ppc-le-64":
                return new PowerPc64CallingConvention(arch);
            case "sparc32":
            case "sparc64":
                return new SparcCallingConvention(arch);
            case "x86-protected-32":
                if (name == "i386kernel")
                {
                    if (this.cci386KernelAbi is null)
                    {
                        cci386KernelAbi = new i386KernelCallingConvention(arch);
                    }
                    return cci386KernelAbi;
                }

                if (this.ccX86 is null)
                {
                    if (services is null)
                        return null;
                    var svc = services.RequireService<IPluginLoaderService>();
                    var t = svc.GetType("Reko.Arch.X86.X86CallingConvention,Reko.Arch.X86");
                    this.ccX86 = (ICallingConvention)Activator.CreateInstance(
                        t,
                        4,      // stackAlignment,
                        4,      // pointerSize,
                        true,   // callerCleanup,
                        false)!; // reverseArguments)
                }
                return this.ccX86;
            case "x86-protected-64":
                return new X86_64CallingConvention(arch);
            case "xtensa":
                return new XtensaCallingConvention(arch);
            case "arm":
                return new Arm32CallingConvention(arch);
            case "arm-64":
                // Documentation states to use the architecture-defined ABI.
                return arch.GetCallingConvention("")!;
            case "m68k":
                return new M68kCallingConvention(arch);
            case "avr8":
                return new Avr8CallingConvention(arch);
            case "avr32":
                return new Avr32CallingConvention(arch);
            case "msp430":
                return arch.GetCallingConvention("");
            case "risc-v":
                if (this.ccRiscV is null)
                {
                    var t = Type.GetType("Reko.Arch.RiscV.RiscVCallingConvention,Reko.Arch.RiscV", true)!;
                    this.ccRiscV = (ICallingConvention) Activator.CreateInstance(t, arch)!;
                }
                return this.ccRiscV;
            case "superH":
                return new SuperHCallingConvention(arch);
            case "alpha":
                return new AlphaCallingConvention(arch);
            case "zSeries":
                return new zSeriesCallingConvention(arch);
            case "blackfin":
                return new BlackfinCallingConvention(arch);
            case "hexagon":
                return new HexagonCallingConvention(arch);
            case "ia64":
                return new Ia64CallingConvention(arch);
            case "vax":
                return new VaxCallingConvention(arch);
            case "nios2":
                return new Nios2CallingConvention(arch);
            case "aeon":
                return new AeonCallingConvention(arch);
            case "csky":
                return new CSkyCallingConvention(arch);
            case "beyond":
                return new BeyondCallingConvention(arch);
            default:
                return null;
            }
        }

        public Func<IEnumerable<RtlInstructionCluster>, Constant?>? CreateGlobalPointerFinder(IProcessorArchitecture arch)
        {
            switch (arch.Name)
            {
            case "risc-v":
                return GlobalPointerFinder.RiscV;
            default:
                return null;
            }
        }

    }
}
