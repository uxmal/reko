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
        private PrimitiveType dataWidth;

        public OperandRewriter(Rewriter rewriter, PrimitiveType dataWidth)
        {
            this.rewriter = rewriter;
            this.m = rewriter.emitter;
            this.frame = rewriter.frame;
            this.dataWidth = dataWidth;
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
            {
                Expression r = rewriter.frame.EnsureRegister(reg.Register);
                if (dataWidth.Size != reg.Width.Size)
                    r = m.Cast(dataWidth, r);
                return r;
            }
            var imm = operand as ImmediateOperand;
            if (imm != null)
            {
                return imm.Value;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var ea = rewriter.frame.EnsureRegister(mem.Base);
                return m.Load(rewriter.di.Instruction.dataWidth, ea);
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var ea = rewriter.frame.EnsureRegister(pre.Register);
                m.Assign(ea, m.ISub(ea, rewriter.di.Instruction.dataWidth.Size));
                return m.Load(rewriter.di.Instruction.dataWidth, ea);
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = frame.EnsureRegister(post.Register);
                var tmp = rewriter.frame.CreateTemporary(dataWidth);
                m.Assign(tmp, m.Load(dataWidth, r));
                m.Assign(r, m.IAdd(r, dataWidth.Size));
                return tmp;
            }
            throw new NotImplementedException("Unimplemented RewriteSrc for operand type " + operand.ToString());
        }

        public Expression RewriteDst(MachineOperand operand, Expression src, Func<Expression, Expression, Expression> opGen)
        {
            return RewriteDst(operand, this.dataWidth, src, opGen);
        }

        public Expression RewriteDst(MachineOperand operand, PrimitiveType dataWidth, Expression src, Func<Expression ,Expression, Expression> opGen)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
            {
                Expression r = frame.EnsureRegister(reg.Register);
                Expression tmp = r;
                if (reg.Width.BitSize > dataWidth.BitSize)
                {
                    Expression rSub = m.Cast(dataWidth, r);
                    var srcExp = opGen(src, rSub);
                    if (srcExp is Identifier || srcExp is Constant)
                    {
                        tmp = srcExp;
                    }
                    else
                    {
                        tmp = rewriter.frame.CreateTemporary(dataWidth);
                        m.Assign(tmp, srcExp);
                    }
                    src = m.Dpb(r, tmp, 0, dataWidth.BitSize);
                }
                else
                {
                    src = opGen(src, r);
                }
                m.Assign(r, src);
                return tmp;
            }
            var dbl = operand as DoubleRegisterOperand;
            if (dbl != null)
            {
                Identifier h = frame.EnsureRegister(dbl.Register1);
                Identifier l = frame.EnsureRegister( dbl.Register2);
                var d = frame.EnsureSequence(h, l, PrimitiveType.Word64);
                var result = opGen(src, l);
                m.Assign(d, result);
                return d;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var bReg = rewriter.frame.EnsureRegister(mem.Base);
                var load = m.Load(mem.Width, m.IAdd(bReg, mem.Offset));
                var tmp = rewriter.frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load, src));
                m.Assign(load, tmp);
                return tmp;
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                src = Spill(src);
                var r = frame.EnsureRegister(post.Register);
                var load = opGen(src, m.Load(post.Width, r));
                m.Assign(m.Load(dataWidth, r), load);
                m.Assign(r, m.IAdd(r, dataWidth.Size));
                return load;
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                src = Spill(src);
                var r = frame.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, dataWidth.Size));
                var load = m.Load(dataWidth, r);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load, src));
                m.Assign(m.Load(dataWidth, r), tmp);
                return tmp;
            }
            throw new NotImplementedException("Unimplemented RewriteDst for operand type " + operand.ToString());

        }

        public Expression RewriteUnary(MachineOperand operand, PrimitiveType dataWidth, Func<Expression ,Expression> opGen)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
            {
                Expression r = frame.EnsureRegister(reg.Register);
                if (r.DataType.Size > dataWidth.Size)
                {
                    var tmp = frame.CreateTemporary(dataWidth);
                    m.Assign(tmp, opGen(m.Cast(dataWidth, r)));
                    m.Assign(r, m.Dpb(r, tmp, 0, dataWidth.BitSize));
                    return tmp;
                }
                else 
                {
                    m.Assign(r, opGen(r));
                    return r;
                }
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = frame.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, dataWidth.Size));
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(m.Load(dataWidth, r)));
                m.Assign(m.Load(dataWidth, r), tmp);
                return tmp;
            }
            throw new NotImplementedException("Unimplemented RewriteUnary for operand type " + operand.ToString());

        }

        private Expression Spill(Expression src)
        {
            if (src is MemoryAccess)
            {
                var tmp = frame.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = tmp;
            }
            return src;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return rewriter.frame.EnsureFlagGroup((uint)flags, rewriter.arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }
    }
}
