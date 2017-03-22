#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Rewriter support for "extended" instructions of the x86 architecture.
    /// Basically, anything SSE or post-Pentium goes here.
    /// </summary>
    public partial class X86Rewriter
    {
        public void RewritePxor()
        {
            var rdst = instrCur.op1 as RegisterOperand;
            var rsrc = instrCur.op2 as RegisterOperand;
            if (rdst != null && rsrc != null && rdst.Register.Number == rsrc.Register.Number)
            { // selfie!
                m.Assign(orw.AluRegister(rdst), m.Cast(rdst.Width, Constant.Int32(0)));
                return;
            }
            var dst = this.SrcOp(instrCur.op1);
            var src = this.SrcOp(instrCur.op2);
            m.Assign(dst, host.PseudoProcedure("__pxor", dst.DataType, dst, src));
        }
    }
}
