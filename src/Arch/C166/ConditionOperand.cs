#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.C166
{
    public class ConditionalOperand : MachineOperand
    {
        public ConditionalOperand(CondCode cc) : base(PrimitiveType.Bool)
        {
            this.CondCode = cc;
        }

        public CondCode CondCode { get; }
        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(CondCode.ToString());
        }
    }

    public enum CondCode
    {
        cc_UC,
        cc_NET,
        cc_Z,
        cc_NZ,
        cc_V,
        cc_NV,
        cc_N,
        cc_NN,
        cc_C,
        cc_NC,
        //cc_ULT C = 1 Unsigned less than 8H
        //cc_UGE C = 0 Unsigned greater than or equal 9H
        cc_SGT,
        cc_SLE,
        cc_SLT,
        cc_SGE,
        cc_UGT,
        cc_ULE,
    }
}
