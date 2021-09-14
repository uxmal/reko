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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class TrampolineFinder
    {
        /// <summary>
        /// Find the destination of a ARM PLT stub.
        /// </summary>
        public static Expression? Arm32(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(3).ToArray();
            if (stubInstrs.Length != 3)
                return null;

            Constant offset;
            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Src is MemoryAccess mem &&
                mem.EffectiveAddress is Address addrOffset &&
                mem.DataType is PrimitiveType dt &&
                dt.BitSize == 32)
            {
                if (!host.TryRead(arch, addrOffset, dt, out offset))
                    return null;
            }
            else return null;

            if (stubInstrs[1] is RtlAssignment ass2 &&
                ass2.Src is BinaryExpression bin &&
                bin.Operator is IAddOperator &&
                bin.Left is Address addr &&
                bin.Right == ass.Dst)
            {
                addr = addr + offset.ToInt32();
            }
            else return null;

            if (stubInstrs[2] is RtlGoto g &&
                g.Target is MemoryAccess mem2 &&
                mem2.EffectiveAddress == ass2.Dst)
            {
                return addr;
            }
            else return null;
        }

        public static Expression? AArch64(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            // adrp x16,#&13000
            //ldr x17,[x16]
            //add x16, x16,#0
            //br x17

            var stubInstrs = instrs.Take(4).ToArray();
            if (stubInstrs.Length != 4)
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
                    bin.Operator == Operator.IAdd &&
                    bin.Left == idPage &&
                    bin.Right is Constant offset)
                {
                    addr = addrPage + offset.ToInt32();
                }
                else if (gotslot.EffectiveAddress is Identifier idEa &&
                  idEa == idPage)
                {
                    addr = addrPage;
                }
                else
                    return null;
            }
            else return null;

            if (stubInstrs[2] is RtlAssignment addrOf &&
                addrOf.Dst != ptrGotSlot)
            {
            }
            else return null;

            if (stubInstrs[3] is RtlGoto g && 
                g.Target == ptrGotSlot)
            {
                return addr;
            }
            return null;
        }

        /// <summary>
        /// Find the destination of a Risc-V PLT stub.
        /// </summary>
        /// <code>
        /// auipc   t3,00000005
        /// ld      t3,t3,+000000F0
        /// jalr    t1,t3,+00000000
        /// </code>
        /// <param name="addrInstr">Address of the beginning of the stub.</param>
        public static Expression? RiscV(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(4).ToArray();
            if (stubInstrs.Length != 4)
                return null;
            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Src is Address addr)
            {

            }
            else return null;

            if (stubInstrs[1] is RtlAssignment ld &&
                ld.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Operator == Operator.IAdd &&
                bin.Left == ass.Dst &&
                bin.Right is Constant offset)
            {
                addr = addr + offset.ToInt64();
            }
            else return null;

            if (stubInstrs[2] is RtlAssignment &&
                stubInstrs[3] is RtlGoto g &&
                g.Target is Identifier target &&
                target == ld.Dst)
            {
                return addr;
            }
            else return null;
        }

        public static Expression? X86(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var instr = instrs.FirstOrDefault();
            if (instr == null)
                return null;
            // Match x86 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return pc;
                if (!(jump.Target is MemoryAccess access))
                    return null;
                addrTarget = access.EffectiveAddress as Address;
                if (addrTarget == null)
                {
                    if (!(access.EffectiveAddress is Constant wAddr))
                    {
                        return null;
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
            }
            return addrTarget;
        }

        public static Expression? X86_64(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var instr = instrs.FirstOrDefault();
            if (instr == null)
                return null;
            // Match x86-64 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return pc;
                if (!(jump.Target is MemoryAccess access))
                    return null;
                addrTarget = access.EffectiveAddress as Address;
                if (addrTarget == null)
                {
                    if (!(access.EffectiveAddress is Constant wAddr))
                    {
                        return null;
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
            }
            return addrTarget;
        }
    }
}
