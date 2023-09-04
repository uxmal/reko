#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;

namespace Reko.Environments.MacOS.OSX.ArchSpecific
{
    public abstract class ArchSpecificHandler
    {
        public static ArchSpecificHandler Create(IProcessorArchitecture arch)
        {
            switch (arch.Name)
            {
            case "arm-64":
                return new AArch64Handler(arch);
            case "x86-protected-64":
                return new X86_64Handler(arch);
            }
            throw new NotSupportedException($"A MacOS X handler for the {arch.Description} has not been implemented yet.");
        }

        public abstract Expression? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host);

        public abstract Expression? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host);

        public abstract CallingConvention? GetCallingConvention(string? ccName);
    }
}
