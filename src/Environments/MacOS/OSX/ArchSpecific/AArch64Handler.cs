#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.MacOS.OSX.ArchSpecific
{
    internal class AArch64Handler : ArchSpecificHandler
    {
        private readonly IProcessorArchitecture arch;

        public AArch64Handler(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConvention? GetCallingConvention(string? ccName)
        {
            return arch.GetCallingConvention(ccName);
        }

        public override Expression? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            // adrp x16,#&100004000
            // ldr x16,[x16+8]
            // br x16
            if (instrs.Count < 3)
                return null;

            if (instrs[^3].Instructions[0] is RtlAssignment ass &&
                ass.Dst is Identifier idPage &&
                ass.Src is Address addrPage &&
                idPage.Name == "x16")
            {
            }
            else return null;

            Address addr;
            if (instrs[^2].Instructions[0] is RtlAssignment load &&
                load.Dst is Identifier ptrGotSlot &&
                load.Src is MemoryAccess gotslot)
            {
                if (gotslot.EffectiveAddress is BinaryExpression bin &&
                    bin.Operator == Operator.IAdd &&
                    bin.Left == idPage &&
                    bin.Right is Constant offset)
                {
                    addr = addrPage + offset.ToInt64();
                }
                else if (gotslot.EffectiveAddress is Identifier idEa &&
                     idEa == idPage)
                {
                    addr = addrPage;
                }
                else
                    return null;
            }
            else
                return null;

            if (instrs[^1].Instructions[0] is RtlGoto g &&
                g.Target == ptrGotSlot)
            {
                return addr;
            }
            return null;
        }

        public override Expression? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            // adrp x16,#&100004000
            // ldr x16,[x16+8]
            // br x16
            var stubInstrs = instrs.Take(3).ToArray();
            if (stubInstrs.Length != 3)
                return null;

            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Dst is Identifier idPage &&
                ass.Src is Address addrPage &&
                idPage.Name == "x16")
            {
            }
            else return null;

            Address addr;
            if (stubInstrs[1] is RtlAssignment load &&
                load.Dst is Identifier ptrGotSlot &&
                load.Src is MemoryAccess gotslot)
            {
                if (gotslot.EffectiveAddress is BinaryExpression bin &&
                    bin.Operator.Type == OperatorType.IAdd &&
                    bin.Left == idPage &&
                    bin.Right is Constant offset)
                {
                    addr = addrPage + offset.ToInt64();
                }
                else if (gotslot.EffectiveAddress is Identifier idEa &&
                     idEa == idPage)
                {
                    addr = addrPage;
                }
                else
                    return null;
            }
            else
                return null;

            if (stubInstrs[2] is RtlGoto g &&
                g.Target == ptrGotSlot)
            {
                return addr;
            }
            return null;
        }

    }
}