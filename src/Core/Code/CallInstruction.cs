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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core.Code
{
    /// <summary>
    /// Models a low-level call instruction.
    /// </summary>
    /// <remarks>CallInstructions only exist right after scanning. Subsequent
    /// decompiler phases will replace them with <cref>Application</cref> instances.
    /// expressions.
    /// </remarks>
    public class CallInstruction : Instruction
    {
        public CallInstruction(Expression callee, CallSite site)
        {
            if (callee == null)
                throw new ArgumentNullException("callee");
            this.Callee = callee;
            this.CallSite = site;
            this.Definitions = new HashSet<DefInstruction>();
            this.Uses = new HashSet<UseInstruction>();
        }

        public Expression Callee { get; set; }
        public CallSite CallSite { get; private set; }

        // Set of variables that reach the call site. These need to be reconciled 
        // with the variables used by the callee, if these are known.
        public HashSet<UseInstruction> Uses { get; private set; }

        // Set of variables that the called function defines.
        public HashSet<DefInstruction> Definitions { get; private set; }

        public override bool IsControlFlow { get { return false; } }

        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformCallInstruction(this);
        }

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitCallInstruction(this);
        }

        public override void Accept(InstructionVisitor v)
        {
            v.VisitCallInstruction(this);
        }
    }
}
