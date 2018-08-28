#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteScvtf()
        {
            var srcReg = ((RegisterOperand)instr.ops[1]).Register;
            var dstReg = ((RegisterOperand)instr.ops[0]).Register;
            var src = binder.EnsureRegister(srcReg);
            var dst = binder.EnsureRegister(dstReg);
            var realType = PrimitiveType.Create(Domain.Real, (int)dstReg.BitSize);
            if (Registers.IsIntegerRegister(srcReg))
            {
                var intType = PrimitiveType.Create(Domain.SignedInt, (int)srcReg.BitSize);
                m.Assign(dst, m.Cast(realType, m.Cast(intType, src)));
            }
            else if (instr.ops.Length == 3)
            {
                // fixed point conversion.
                throw new NotImplementedException();
            }
            else
            {
                //$BUG: what is this supposed to do?
                m.Assign(dst, m.Cast(realType, src));
            }
        }
    }
}
