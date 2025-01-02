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

using Reko.Core.Collections;
using Reko.Core.Machine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.RiscV
{
    public class LongConstantFuser : IEnumerable<MachineInstruction>
    {
        private RiscVDisassembler dasm;

        public LongConstantFuser(RiscVDisassembler dasm)
        {
            this.dasm = dasm;
        }

        public IEnumerator<MachineInstruction> GetEnumerator()
        {
            ImmediateOperand immLo;
            var e = new LookaheadEnumerator<RiscVInstruction>(dasm);
            while (e.MoveNext())
            {
                var instrHi = e.Current;
                switch (instrHi.Mnemonic)
                {
                case Mnemonic.auipc:
                    if (!e.TryPeek(1, out var instrLo))
                    {
                        yield return e.Current;
                        break;
                    }
                    if (instrLo.Mnemonic == Mnemonic.addi)
                    {
                        if (instrHi.Operands[0] == instrLo.Operands[0] &&
                            instrLo.Operands[0] == instrLo.Operands[1])
                        {
                            // Mutate the auipc
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            immLo = (ImmediateOperand) instrLo.Operands[2];
                            var fullAddr = instrHi.Address +
                                ((immHi.Value.ToInt32() << 12) +
                                immLo.Value.ToInt32());
                            var hiOp = new SliceOperand(SliceType.PcRelHi, immHi, fullAddr);
                            var loOp = new SliceOperand(SliceType.PcRelLo, immLo, fullAddr);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[2] = loOp;
                        }
                    }
                    else if (IsMemoryInstruction(instrLo.Mnemonic))
                    {
                        var memOp = (MemoryOperand) instrLo.Operands[1];
                        if (instrHi.Operands[0] == memOp.Base &&
                            memOp.Offset is ImmediateOperand imm)
                        {
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            immLo = imm;
                            // Mutate the auipc and the memory operand
                            var fullAddr = instrHi.Address +
                                ((immHi.Value.ToInt32() << 12) +
                                immLo.Value.ToInt32());
                            var hiOp = new SliceOperand(SliceType.PcRelHi, immHi, fullAddr);
                            var loOp = new SliceOperand(SliceType.PcRelLo, immLo, fullAddr);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[1] = loOp;
                        }
                    }
                    yield return e.Current;
                    break;
                case Mnemonic.lui:
                    if (!e.TryPeek(1, out instrLo))
                    {
                        yield return e.Current;
                        break;
                    }
                    if (instrLo.Mnemonic == Mnemonic.addi)
                    {
                        if (instrHi.Operands[0] == instrLo.Operands[0] &&
                            instrLo.Operands[0] == instrLo.Operands[1])
                        {
                            // Mutate the auipc and the addi
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            immLo = (ImmediateOperand) instrLo.Operands[2];
                            var fullAddr = ImmediateOperand.Word32(
                                (immHi.Value.ToInt32() << 12) +
                                immLo.Value.ToInt32());
                            var hiOp = new SliceOperand(SliceType.PcRelHi, immHi, fullAddr);
                            var loOp = new SliceOperand(SliceType.PcRelLo, immLo, fullAddr);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[2] = loOp;
                        }
                    }
                    else if (IsMemoryInstruction(instrLo.Mnemonic))
                    {
                        var memOp = (MemoryOperand) instrLo.Operands[1];
                        if (instrHi.Operands[0] == memOp.Base &&
                            memOp.Offset is ImmediateOperand imm)
                        {
                            immLo = imm;
                            // Mutate the auipc and the memory operand
                            var immHi = (ImmediateOperand) instrHi.Operands[1];
                            var fullAddr = ImmediateOperand.Word32(
                                (immHi.Value.ToInt32() << 12) +
                                immLo.Value.ToInt32());
                            var hiOp = new SliceOperand(SliceType.PcRelHi, immHi, fullAddr);
                            var loOp = new SliceOperand(SliceType.PcRelLo, immLo, fullAddr);
                            instrHi.Operands[1] = hiOp;
                            instrLo.Operands[1] = loOp;
                        }
                    }
                    yield return e.Current;
                    break;
                default:
                    yield return e.Current;
                    break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static bool IsMemoryInstruction(Mnemonic mnemonic)
        {
            return mnemonic switch
            {
                Mnemonic.c_ld or
                Mnemonic.c_lw or
                Mnemonic.c_sd or
                Mnemonic.c_sw or
                Mnemonic.lb or
                Mnemonic.lbu or
                Mnemonic.lh or
                Mnemonic.lhu or
                Mnemonic.lw or
                Mnemonic.lwu or
                Mnemonic.ld or
                Mnemonic.sb or
                Mnemonic.sh or
                Mnemonic.sw or
                Mnemonic.sd or
                Mnemonic.fsd or
                Mnemonic.fsw => true,
                _ => false,
            };
        }
    }
}
