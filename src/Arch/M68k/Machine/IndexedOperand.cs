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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k.Machine
{
    /// <summary>
    /// The Godzilla of address operands, supporting Indirect pre- and post-indexed operation.
    /// </summary>
    public class IndexedOperand : AbstractMachineOperand
    {
        public Constant? BaseDisplacement;
        public Constant? OuterDisplacement;
        public RegisterStorage? Base;
        public RegisterStorage? Index;
        public PrimitiveType? index_reg_width;
        public int IndexScale;
        public bool preindex;
        public bool postindex;

        public IndexedOperand(
            PrimitiveType width,
            Constant? baseReg,
            Constant? outer,
            RegisterStorage? base_reg,
            RegisterStorage? index_reg,
            PrimitiveType? index_reg_width,
            int index_scale,
            bool preindex,
            bool postindex)
            : base(width)
        {
            BaseDisplacement = baseReg;
            OuterDisplacement = outer;
            Base = base_reg;
            Index = index_reg;
            this.index_reg_width = index_reg_width;
            IndexScale = index_scale;
            this.preindex = preindex;
            this.postindex = postindex;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            //@base = EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
            //outer = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
            //if (EXT_BASE_REGISTER_PRESENT(extension))
            //    base_reg = string.Format("A{0}", instruction & 7);
            //else
            //    base_reg = "";
            //if (EXT_INDEX_REGISTER_PRESENT(extension))
            //{
            //    index_reg = string.Format("{0}{1}.{2}", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
            //    if (EXT_INDEX_SCALE(extension) != 0)
            //        index_reg += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
            //}
            //else
            //    index_reg = "";
            //preindex = (extension & 7) > 0 && (extension & 7) < 4;
            //postindex = (extension & 7) > 4;

            renderer.WriteString("(");
            var sep = preindex || postindex ? "[" : "";
            if (BaseDisplacement is not null)
            {
                renderer.WriteString(sep);
                renderer.WriteString(FormatValue(BaseDisplacement));
                sep = ",";
            }
            if (Base is not null)
            {
                renderer.WriteString(sep);
                renderer.WriteString(Base.ToString());
                sep = ",";
            }
            if (postindex)
            {
                if (BaseDisplacement is not null || Base is not null)
                {
                    renderer.WriteString("]");
                    sep = ",";
                }
                else
                {
                    sep = "";
                }
            }
            if (Index is not null)
            {
                renderer.WriteString(sep);
                renderer.WriteString(Index.Name);
                if (index_reg_width!.BitSize == 16)
                    renderer.WriteString(".w");
                if (IndexScale > 1)
                    renderer.WriteFormat("*{0}", IndexScale);
                sep = ",";
            }
            if (preindex)
            {
                renderer.WriteString("]");
                sep = ",";
            }
            if (OuterDisplacement is not null)
            {
                renderer.WriteString(sep);
                renderer.WriteString(FormatSignedValue(OuterDisplacement));
            }
            renderer.WriteString(")");
        }
    }
}
