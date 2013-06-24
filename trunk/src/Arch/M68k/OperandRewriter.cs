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
    /// <summary>
    /// Rewrites M68k operands into sequences of RTL expressions and possibly instructions.
    /// </summary>
    /// <remarks>
    /// Some of the operands, like (A6)+ and -(A5), have side effects that need to be expressed
    /// as separate instructions. We must therefore insert RTL instructions into the stream as these
    /// operands are rewritten. Because of these side effects, it is critical that we don't call
    /// Rewrite twice on the same operand, as this will cause two side effect instructions to be
    /// generated.
    /// </remarks>
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

        /// <summary>
        /// Rewrite operands being used as sources.
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public Expression RewriteSrc(MachineOperand operand)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
                return rewriter.frame.EnsureRegister(reg.Register);
            var imm = operand as ImmediateOperand;
            if (imm != null)
                return imm.Value;
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var ea = rewriter.frame.EnsureRegister(mem.Base);
                var tmp = rewriter.frame.CreateTemporary(rewriter.di.Instruction.dataWidth);
                m.Assign(tmp, m.Load(rewriter.di.Instruction.dataWidth, ea));
                return tmp;
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = rewriter.frame.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, rewriter.di.Instruction.dataWidth.Size));
                return m.Load(rewriter.di.Instruction.dataWidth, r);
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
                return m.Load(rewriter.di.Instruction.dataWidth, frame.EnsureRegister(post.Register));
            throw new NotImplementedException("Unimplemented RewriteSrc for operand type " + operand.ToString());
        }

        public Expression RewriteDst(MachineOperand operand)
        {
            throw new NotImplementedException("Unimplemented RewriteDst for operand type " + operand.ToString());

        }
        public Identifier FlagGroup(FlagM flags)
        {
            return rewriter.frame.EnsureFlagGroup((uint)flags, rewriter.arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }
    }
}
