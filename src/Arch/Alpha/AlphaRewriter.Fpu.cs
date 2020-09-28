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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Alpha
{
    public partial class AlphaRewriter
    {
        private const int FpuZeroRegister = 63;

        private void RewriteCpys(string intrinsic)
        {
            var r1 = ((RegisterOperand)instr.Operands[0]).Register.Number;
            var r2 = ((RegisterOperand)instr.Operands[1]).Register.Number;
            var r3 = ((RegisterOperand)instr.Operands[2]).Register.Number;
            if (r1 == r2 && r2 == r3 && r1 == FpuZeroRegister)
            {
                m.Nop();
            }
            else if (r1 == FpuZeroRegister && r2 == FpuZeroRegister)
            {
                m.Assign(Rewrite(instr.Operands[2]), Rewrite(instr.Operands[0]));
            }
            else
            {
                RewriteInstrinsic(intrinsic);
            }
        }

        private void RewriteFpuOp(Func<Expression,Expression,Expression> fn)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            m.Assign(dst, fn(op1, op2));
            //$TODO: overflow? Chopped?
        }


        private void RewriteCmpt(Func<Expression, Expression, Expression> fn)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            m.Assign(dst, m.Conditional(op1.DataType, fn(op1, op2), Constant.Real64(2.0),  Constant.Real64(0.0)));
        }

        private void RewriteCvt(DataType dtFrom, DataType dtTo)
        {
            var rSrc = ((RegisterOperand)instr.Operands[0]).Register;
            Expression src;
            if (rSrc.Number == ZeroRegister || rSrc.Number == FpuZeroRegister)
            {
                if (dtTo == PrimitiveType.Real64)
                {
                    src = Constant.Real64(0);
                }
                else if (dtTo == PrimitiveType.Real32)
                {
                    src = Constant.Real32(0);
                }
                else
                {
                    src = Constant.Create(dtTo, 0);
                }
            }
            else
            {
                src = m.Cast(dtTo, Rewrite(instr.Operands[0]));
            }
            var dst = Rewrite(instr.Operands[1]);
            m.Assign(dst, src);
        }
    }
}
