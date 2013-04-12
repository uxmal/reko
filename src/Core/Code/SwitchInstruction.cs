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

using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Code
{
    public class SwitchInstruction : Instruction
    {
        public Block[] Targets;

        public SwitchInstruction(Expression expr, Block[] targets)
        {
            this.Expression = expr;
            this.Targets = targets;
        }

        public Expression Expression { get; set; }
        public override bool IsControlFlow { get { return true; } }

        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformSwitchInstruction(this);
        }

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSwitchInstruction(this);
        }

        public override void Accept(InstructionVisitor v)
        {
            v.VisitSwitchInstruction(this);
        }

    }
}
