#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 or or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful or
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not or write to
 * the Free Software Foundation or 675 Mass Ave or Cambridge or MA 02139 or USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.PowerPC
{
    public class LongConstantFuser : IEnumerable<MachineInstruction>
    {
        private PowerPcDisassembler dasm;

        public LongConstantFuser(PowerPcDisassembler dasm)
        {
            this.dasm = dasm;
        }

        public IEnumerator<MachineInstruction> GetEnumerator()
        {
            ImmediateOperand immLo;
            var e = new LookaheadEnumerator<PowerPcInstruction>(dasm);
            while (e.MoveNext())
            {
                var instrHi = e.Current;
                switch (instrHi.Mnemonic)
                {
                case Mnemonic.addis:
                case Mnemonic.oris:
                    if (((RegisterStorage) instrHi.Operands[1]).Number != 0 ||
                        !e.TryPeek(1, out var instrLo))
                    {
                        break;
                    }
                    if (instrLo.Mnemonic == Mnemonic.ori)
                    {
                        if (instrHi.Operands[0] == instrLo.Operands[0] &&
                            instrLo.Operands[0] == instrLo.Operands[1])
                        {
                            // Mutate the addis/oris and ori
                            var immHi = (ImmediateOperand) instrHi.Operands[2];
                            immLo = (ImmediateOperand) instrLo.Operands[2];
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (immHi.Value.ToInt32() << 16) |
                                        immLo.Value.ToInt32()));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[2] = loOp;
                        }
                    }
                    if (instrLo.Mnemonic == Mnemonic.addi)
                    {
                        if (instrHi.Operands[0] == instrLo.Operands[0] &&
                            instrLo.Operands[0] == instrLo.Operands[1])
                        {
                            // Mutate the addis/oris and addi
                            var immHi = (ImmediateOperand) instrHi.Operands[2];
                            immLo = (ImmediateOperand) instrLo.Operands[2];
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (immHi.Value.ToInt32() << 16) +
                                        immLo.Value.ToInt32()));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[2] = loOp;
                        }
                    }
                    else if (IsMemoryInstruction(instrLo.Mnemonic) &&
                        instrLo.Operands[1] is MemoryOperand memOp)
                    {
                        if (instrHi.Operands[0] == memOp.BaseRegister &&
                            memOp.Offset is ImmediateOperand imm)
                        {
                            var immHi = (ImmediateOperand) instrHi.Operands[2];
                            immLo = imm;
                            // Mutate the addis/oris and the memory operand
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (immHi.Value.ToInt32() << 16) +
                                        immLo.Value.ToInt32()));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[2] = hiOp;
                            memOp.Offset = loOp;
                        }
                    }
                    break;
                default:
                    break;
                }
                yield return e.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static bool IsMemoryInstruction(Mnemonic mnemonic)
        {
            return mnemonic switch
            {
                Mnemonic.lbz or
                Mnemonic.lbzcix or
                Mnemonic.lbzu or
                Mnemonic.lbzux or
                Mnemonic.lbzx or
                Mnemonic.ld or
                Mnemonic.ldat or
                Mnemonic.ldarx or
                Mnemonic.ldcix or
                Mnemonic.ldmx or
                Mnemonic.ldu or
                Mnemonic.ldux or
                Mnemonic.ldx or
                Mnemonic.lfd or
                Mnemonic.lfdp or
                Mnemonic.lfdu or
                Mnemonic.lfdux or
                Mnemonic.lfdx or
                Mnemonic.lfs or
                Mnemonic.lfsu or
                Mnemonic.lfsux or
                Mnemonic.lfsx or
                Mnemonic.lha or
                Mnemonic.lhau or
                Mnemonic.lhaux or
                Mnemonic.lhax or
                Mnemonic.lhbrx or
                Mnemonic.lhz or
                Mnemonic.lhzu or
                Mnemonic.lhzux or
                Mnemonic.lhzx or
                Mnemonic.lq or
                Mnemonic.lwa or
                Mnemonic.lwarx or
                Mnemonic.lwax or
                Mnemonic.lwbrx or
                Mnemonic.lwz or
                Mnemonic.lwzu or
                Mnemonic.lwzux or
                Mnemonic.lwzx or

                Mnemonic.stb or
                Mnemonic.stbcix or
                Mnemonic.stbcx or
                Mnemonic.stbu or
                Mnemonic.stbux or
                Mnemonic.stbx or
                Mnemonic.std or
                Mnemonic.stdat or
                Mnemonic.stdbrx or
                Mnemonic.stdcx or
                Mnemonic.stdcix or
                Mnemonic.stdu or
                Mnemonic.stdx or
                Mnemonic.stfd or
                Mnemonic.stfdp or
                Mnemonic.stfdu or
                Mnemonic.stfdux or
                Mnemonic.stfdx or
                Mnemonic.stfiwx or
                Mnemonic.stfs or
                Mnemonic.stfsu or
                Mnemonic.stfsux or
                Mnemonic.stfsx or
                Mnemonic.sth or
                Mnemonic.sthbrx or
                Mnemonic.sthcix or
                Mnemonic.sthcx or
                Mnemonic.sthu or
                Mnemonic.sthx or
                Mnemonic.stmw or
                Mnemonic.stop or
                Mnemonic.stq or
                Mnemonic.stqcx or
                Mnemonic.stqdx or
                Mnemonic.stswi or
                Mnemonic.stswx or
                Mnemonic.stvebx or
                Mnemonic.stvehx or
                Mnemonic.stvewx or
                Mnemonic.stvx or
                Mnemonic.stvxl or
                Mnemonic.stw or
                Mnemonic.stwat or
                Mnemonic.stwbrx or
                Mnemonic.stwcix or
                Mnemonic.stwcx or
                Mnemonic.stwu or
                Mnemonic.stwux or
                Mnemonic.stwx or
                Mnemonic.stxsd or
                Mnemonic.stxsdx or
                Mnemonic.stxsihx or
                Mnemonic.stxsiwx or
                Mnemonic.stxsix or
                Mnemonic.stxssp => true,
                _ => false
            };
        }
    }
}
