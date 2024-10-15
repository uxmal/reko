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
using System.Security.Cryptography;

namespace Reko.Arch.Mips
{
    public class LongConstantFuser : IEnumerable<MachineInstruction>
    {
        private readonly IEnumerable<MipsInstruction> dasm;

        public LongConstantFuser(IEnumerable<MipsInstruction> dasm)
        {
            this.dasm = dasm;
        }

        public IEnumerator<MachineInstruction> GetEnumerator()
        {
            ImmediateOperand immLo;
            var e = new LookaheadEnumerator<MipsInstruction>(dasm);
            while (e.MoveNext())
            {
                var instrHi = e.Current;
                switch (instrHi.Mnemonic)
                {
                case Mnemonic.lui:
                    if (!e.TryPeek(1, out var instrLo))
                    {
                        break;
                    }
                    if (instrLo.Mnemonic == Mnemonic.addiu)
                    {
                        if (instrLo.Operands[0] == instrLo.Operands[1])
                        {
                            // Mutate the lui and addiu
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            immLo = (ImmediateOperand) instrLo.Operands[2];
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (uint)((immHi.Value.ToInt32() << 16) |
                                        immLo.Value.ToInt32())));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[2] = loOp;
                        }
                    }
                    else if (IsMemoryInstruction(instrLo.Mnemonic) &&
                        instrLo.Operands[1] is IndirectOperand memOp)
                    {
                        if (instrHi.Operands[0] == memOp.Base &&
                            memOp.Offset is ImmediateOperand imm)
                        {
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            immLo = imm;
                            // Mutate the addis/oris and the memory operand
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (immHi.Value.ToInt32() << 16) +
                                        immLo.Value.ToInt32()));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[1] = hiOp;
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
                Mnemonic.lb or
                Mnemonic.lbu or
                Mnemonic.ld or
                Mnemonic.lh or
                Mnemonic.lhu or
                Mnemonic.lui or
                Mnemonic.lw or
                Mnemonic.lwu or

                Mnemonic.sb or
                Mnemonic.sd or
                Mnemonic.sh or
                Mnemonic.sw => true,
                _ => false
            };
        }
    }
}