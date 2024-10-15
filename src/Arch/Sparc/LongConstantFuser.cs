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

namespace Reko.Arch.Sparc
{
    public class LongConstantFuser : IEnumerable<MachineInstruction>
    {
        private readonly IEnumerable<SparcInstruction> dasm;

        public LongConstantFuser(IEnumerable<SparcInstruction> dasm)
        {
            this.dasm = dasm;
        }

        public IEnumerator<MachineInstruction> GetEnumerator()
        {
            ImmediateOperand immLo;
            var e = new LookaheadEnumerator<SparcInstruction>(dasm);
            while (e.MoveNext())
            {
                var instrHi = e.Current;
                switch (instrHi.Mnemonic)
                {
                case Mnemonic.sethi:
                    if (((RegisterStorage) instrHi.Operands[1]).Number == 0 ||
                        !e.TryPeek(1, out var instrLo))
                    {
                        break;
                    }
                    if (instrLo.Mnemonic == Mnemonic.add)
                    {
                        if (instrHi.Operands[1] == instrLo.Operands[0])
                        {
                            // Mutate the sethi and add
                            var immHi = (ImmediateOperand) instrHi.Operands[0];
                            immLo = (ImmediateOperand) instrLo.Operands[1];
                            var longConst = new ImmediateOperand(
                                Constant.Create(
                                    instrHi.Operands[0].Width,
                                    (uint) ((immHi.Value.ToInt32() << 16) |
                                        immLo.Value.ToInt32())));
                            var hiOp = new SliceOperand(SliceType.Hi, immHi, longConst);
                            var loOp = new SliceOperand(SliceType.Lo, immLo, longConst);
                            instrHi.Operands[0] = hiOp;
                            instrLo.Operands[1] = loOp;
                        }
                    }
                    else if (IsMemoryLoadInstruction(instrLo.Mnemonic))
                    {
                        if (instrLo.Operands[0] is MemoryOperand memOp &&
                            instrHi.Operands[1] == memOp.Base &&
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
                            instrHi.Operands[0] = hiOp;
                            memOp.Offset = loOp;
                        }
                    }
                    else if (IsMemoryStoreInstruction(instrLo.Mnemonic) &&
                        instrLo.Operands[1] is MemoryOperand memOp)
                    {
                        if (instrHi.Operands[1] == memOp.Base &&
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

        private static bool IsMemoryLoadInstruction(Mnemonic mnemonic)
        {
            return mnemonic switch
            {
                Mnemonic.ld or
                Mnemonic.ldd or
                Mnemonic.ldf or
                Mnemonic.lddf or
                Mnemonic.ldsb or
                Mnemonic.ldub or
                Mnemonic.ldsh or
                Mnemonic.lduh or
                Mnemonic.ldsw or
                Mnemonic.lduw or
                _ => false
            };
        }

        private static bool IsMemoryStoreInstruction(Mnemonic mnemonic)
        {
            return mnemonic switch
            {
                Mnemonic.st or
                Mnemonic.std or
                Mnemonic.stb or
                Mnemonic.sth or
                Mnemonic.stf or
                Mnemonic.stdf or
                _ => false
            };
        }

    }
}
