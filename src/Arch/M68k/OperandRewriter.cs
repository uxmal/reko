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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class OperandRewriter
    {
        private Rewriter rewriter;
        private RtlEmitter m;
        private Frame frame;

        public OperandRewriter(Rewriter rewriter)
        {
            this.rewriter = rewriter;
            this.m = rewriter.emitter;
            this.frame = rewriter.frame;
        }

        public Expression Rewrite(MachineOperand operand)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
                return rewriter.frame.EnsureRegister(reg.Register);
            var imm = operand as ImmediateOperand;
            if (imm != null)
                return imm.Value;
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = rewriter.frame.EnsureRegister(pre.Register);
                m.Assign(r, m.Sub(r, rewriter.di.Instruction.dataWidth.Size));
                return m.Load(rewriter.di.Instruction.dataWidth, r);
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
                return m.Load(rewriter.di.Instruction.dataWidth, frame.EnsureRegister(post.Register));
            throw new NotImplementedException("Unimplemented rewrite for operand type " + operand.ToString());
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return rewriter.frame.EnsureFlagGroup((uint)flags, rewriter.arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }
    }
}
