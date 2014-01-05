#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        public Opcodes Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override uint DefCc()
        {
            throw new NotImplementedException();
        }

        public override uint UseCc()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}\t", Opcode);
            if (op1 != null)
            {
                sb.Append(op1);
                if (op2 != null)
                    sb.AppendFormat(",{0}", op2);
            }
            return sb.ToString();
        }
    }

    public enum Opcodes
    {
        illegal = -1,

        adc,
        add,
        asl,
        asr,
        bic,
        bis,
        bit,
        cmp,
        dec,
        clr,
        com,
        div,
        inc,
        mark,
        mfpd,
        mfpi,
        mfps,
        mtps,
        mtpi,
        mtpd,
        mov,
        mul,
        neg,
        rol,
        ror,
        sbc,
        sub,
        swab,
        sxt,
        tst,
        br,
        bne,
        bcs,
        bcc,
        bvs,
        bvc,
        blos,
        bhi,
        bmi,
        bgt,
        blt,
        bge,
        beq,
        ble,
        bpl,
        xor,
    }
}
