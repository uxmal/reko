#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public class Instruction : MachineInstruction
    {
        public Opcode Code;
        public Operand Operand;

        public override int OpcodeAsInteger => (int) Code;

        public override MachineOperand GetOperand(int i)
        {
            return i == 0 ? Operand : null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Code.ToString());
            if (Operand != null)
            {
                writer.Tab();
                Operand.Write(writer, options);
            }
        }

        // http://www.obelisk.demon.co.uk/6502/instructions.html
        public static FlagM DefCc(Opcode code)
        {
            switch (code)
            {
            case Opcode.clc:
            case Opcode.sec:
                return FlagM.CF;
            case Opcode.cld:
            case Opcode.sed:
                return FlagM.DF;
            case Opcode.cli:
            case Opcode.sei:
                return FlagM.IF;
            case Opcode.clv:
                return FlagM.VF;
            case Opcode.and:
            case Opcode.dec:
            case Opcode.dex:
            case Opcode.dey:
            case Opcode.eor:
            case Opcode.inc:
            case Opcode.inx:
            case Opcode.iny:
            case Opcode.lda:
            case Opcode.ldx:
            case Opcode.ldy:
            case Opcode.ora:
            case Opcode.tax:
            case Opcode.tay:
            case Opcode.txa:
            case Opcode.tya:
            case Opcode.tsx:
            case Opcode.pla:
                return FlagM.NF | FlagM.ZF;
            case Opcode.bit:
                return FlagM.NF | FlagM.VF | FlagM.ZF;
            case Opcode.adc:
            case Opcode.sbc:
                return FlagM.NF | FlagM.VF | FlagM.ZF | FlagM.CF;
            case Opcode.asl:
            case Opcode.cmp:
            case Opcode.cpx:
            case Opcode.cpy:
            case Opcode.lsr:
            case Opcode.rol:
            case Opcode.ror:
                return FlagM.NF | FlagM.ZF | FlagM.CF;

            case Opcode.txs:
            case Opcode.pha:
            case Opcode.php:
            case Opcode.jmp:
            case Opcode.jsr:
            case Opcode.rts:
            case Opcode.nop:
                return 0;
            case Opcode.plp:
            case Opcode.rti:
                return FlagM.CF |
                    FlagM.ZF |
                    FlagM.IF |
                    FlagM.DF |
                    FlagM.BF |
                    FlagM.VF |
                    FlagM.NF;
            case Opcode.brk:
                return FlagM.BF;
            }
            throw new NotImplementedException("DefCc for " + code);
        }

        public static FlagM UseCc(Opcode code)
        {
            switch (code)
            {
            case Opcode.adc:
            case Opcode.sbc:
                return FlagM.CF;
            }
            return 0;
        }

    }
}
