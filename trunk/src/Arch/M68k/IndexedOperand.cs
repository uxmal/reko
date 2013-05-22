#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class IndexedOperand : MachineOperand, M68kOperand
    {
        private RegisterStorage base_reg;
        private RegisterStorage index_reg;
        private PrimitiveType index_reg_width;
        private int index_scale;
        private bool preindex;
        private bool postindex;

        public IndexedOperand(PrimitiveType width, Core.RegisterStorage base_reg, Core.RegisterStorage index_reg, Core.Types.PrimitiveType index_reg_width, int index_scale, bool preindex, bool postindex)
            : base(width)
        {
            // TODO: Complete member initialization
            this.base_reg = base_reg;
            this.index_reg = index_reg;
            this.index_reg_width = index_reg_width;
            this.index_scale = index_scale;
            this.preindex = preindex;
            this.postindex = postindex;
        }

        public T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
