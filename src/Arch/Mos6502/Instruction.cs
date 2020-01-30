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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public class Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic;

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            RenderOperands(writer, options);
        }

        // http://www.obelisk.demon.co.uk/6502/instructions.html
        public static FlagM DefCc(Mnemonic code)
        {
            switch (code)
            {
            case Mnemonic.clc:
            case Mnemonic.sec:
                return FlagM.CF;
            case Mnemonic.cld:
            case Mnemonic.sed:
                return FlagM.DF;
            case Mnemonic.cli:
            case Mnemonic.sei:
                return FlagM.IF;
            case Mnemonic.clv:
                return FlagM.VF;
            case Mnemonic.and:
            case Mnemonic.dec:
            case Mnemonic.dex:
            case Mnemonic.dey:
            case Mnemonic.eor:
            case Mnemonic.inc:
            case Mnemonic.inx:
            case Mnemonic.iny:
            case Mnemonic.lda:
            case Mnemonic.ldx:
            case Mnemonic.ldy:
            case Mnemonic.ora:
            case Mnemonic.tax:
            case Mnemonic.tay:
            case Mnemonic.txa:
            case Mnemonic.tya:
            case Mnemonic.tsx:
            case Mnemonic.pla:
                return FlagM.NF | FlagM.ZF;
            case Mnemonic.bit:
                return FlagM.NF | FlagM.VF | FlagM.ZF;
            case Mnemonic.adc:
            case Mnemonic.sbc:
                return FlagM.NF | FlagM.VF | FlagM.ZF | FlagM.CF;
            case Mnemonic.asl:
            case Mnemonic.cmp:
            case Mnemonic.cpx:
            case Mnemonic.cpy:
            case Mnemonic.lsr:
            case Mnemonic.rol:
            case Mnemonic.ror:
                return FlagM.NF | FlagM.ZF | FlagM.CF;

            case Mnemonic.txs:
            case Mnemonic.pha:
            case Mnemonic.php:
            case Mnemonic.jmp:
            case Mnemonic.jsr:
            case Mnemonic.rts:
            case Mnemonic.nop:
                return 0;
            case Mnemonic.plp:
            case Mnemonic.rti:
                return FlagM.CF |
                    FlagM.ZF |
                    FlagM.IF |
                    FlagM.DF |
                    FlagM.BF |
                    FlagM.VF |
                    FlagM.NF;
            case Mnemonic.brk:
                return FlagM.BF;
            }
            throw new NotImplementedException("DefCc for " + code);
        }

        public static FlagM UseCc(Mnemonic code)
        {
            switch (code)
            {
            case Mnemonic.adc:
            case Mnemonic.sbc:
                return FlagM.CF;
            }
            return 0;
        }

    }
}
