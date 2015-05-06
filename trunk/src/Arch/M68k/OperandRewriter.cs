#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        private M68kArchitecture arch;
        private RtlEmitter m;
        private Frame frame;

        public OperandRewriter(M68kArchitecture arch, RtlEmitter emitter, Frame frame, PrimitiveType dataWidth)
        {
            this.arch = arch;
            this.m = emitter;
            this.frame = frame;
            this.DataWidth = dataWidth;
        }

        public PrimitiveType DataWidth { get; set; }     // the data width of the current instruction being rewritten.
        
        /// <summary>
        /// Rewrite operands being used as sources.
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="addrInstr">Address of the current instruction</param>
        /// <returns></returns>
        public Expression RewriteSrc(MachineOperand operand, Address addrInstr, bool addressAsAddress = false)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
            {
                Expression r = frame.EnsureRegister(reg.Register);
                if (DataWidth != null && DataWidth.Size != reg.Width.Size)
                    r = m.Cast(DataWidth, r);
                return r;
            }
            var imm = operand as M68kImmediateOperand;
            if (imm != null)
            {
               if (DataWidth != null && DataWidth.BitSize > imm.Width.BitSize)
                    return Constant.Create(DataWidth, imm.Constant.ToInt64());
                else
                    return Constant.Create(imm.Width, imm.Constant.ToUInt32());
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                return RewriteMemoryAccess(mem, DataWidth, addrInstr);
            }
            var addr = operand as M68kAddressOperand;
            if (addr != null)
            {
                if (addressAsAddress)
                    return addr.Address;
                else 
                    return m.Load(DataWidth, addr.Address);
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var ea = frame.EnsureRegister(pre.Register);
                m.Assign(ea, m.ISub(ea, DataWidth.Size));
                return m.Load(DataWidth, ea);
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = frame.EnsureRegister(post.Register);
                var tmp = frame.CreateTemporary(DataWidth);
                m.Assign(tmp, m.Load(DataWidth, r));
                m.Assign(r, m.IAdd(r, DataWidth.Size));
                return tmp;
            }
            var indidx = operand as IndirectIndexedOperand;
            if (indidx != null)
            {
                Expression ea = frame.EnsureRegister(indidx.ARegister);
                if (indidx.Imm8 != 0)
                    ea = m.IAdd(ea, Constant.Int32(indidx.Imm8));
                Expression ix = frame.EnsureRegister(indidx.XRegister);
                if (indidx.Scale > 1)
                    ix = m.IMul(ix, Constant.Int32(indidx.Scale));
                return m.Load(DataWidth, m.IAdd(ea, ix));
            }
            var indop = operand as IndexedOperand;
            if (indop!=null)
            {
                Expression ea = Combine(indop.Base, indop.base_reg);
                if (indop.postindex)
                {
                    ea = m.LoadDw(ea);
                }
                if (indop.index_reg != null)
                {
                    var idx = Combine(null, indop.index_reg);
                    if (indop.index_scale > 1)
                        idx = m.IMul(idx, indop.index_scale);
                    ea = Combine(ea, idx);
                }
                if (indop.preindex)
                {
                    ea = m.LoadDw(ea);
                }
                ea = Combine(ea, indop.outer);
                return m.Load(DataWidth, ea);
            }
            throw new NotImplementedException("Unimplemented RewriteSrc for operand type " + operand.GetType().Name);
        }

        Expression Combine(Expression e, RegisterStorage reg)
        {
            if (reg == null)
                return e;
            var r = frame.EnsureRegister(reg);
            if (e == null)
                return r;
            return m.IAdd(e, r);
        }

        Expression Combine(Expression e, Expression o)
        {
            if (o == null)
                return e;
            if (e == null)
                return o;
            return m.IAdd(e, o);
        }

        public Expression RewriteDst(MachineOperand operand, Address addrInstr, Expression src, Func<Expression, Expression, Expression> opGen)
        {
            return RewriteDst(operand, addrInstr, this.DataWidth, src, opGen);
        }

        public Expression RewriteDst(MachineOperand operand, Address addrInstr, PrimitiveType dataWidth, Expression src, Func<Expression ,Expression, Expression> opGen)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
            {
                Expression r = frame.EnsureRegister(reg.Register);
                Expression tmp = r;
                if (dataWidth != null && reg.Width.BitSize > dataWidth.BitSize)
                {
                    Expression rSub = m.Cast(dataWidth, r);
                    var srcExp = opGen(src, rSub);
                    if (srcExp is Identifier || srcExp is Constant || srcExp is DepositBits)
                    {
                        tmp = srcExp;
                    }
                    else
                    {
                        tmp = frame.CreateTemporary(dataWidth);
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
            var addr = operand as M68kAddressOperand;
            if (addr != null)
            {
                var load = m.Load(dataWidth, addr.Address);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var load = RewriteMemoryAccess(mem, dataWidth, addrInstr);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = frame.EnsureRegister(post.Register);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, m.Load(post.Width, r))); 
                m.Assign(m.Load(dataWidth, r), tmp);
                m.Assign(r, m.IAdd(r, dataWidth.Size));
                return tmp;
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = frame.EnsureRegister(pre.Register);
                src = Spill(src, r);
                m.Assign(r, m.ISub(r, dataWidth.Size));
                var load = m.Load(dataWidth, r);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(m.Load(dataWidth, r), tmp);
                return tmp;
            }
            var indidx = operand as IndirectIndexedOperand;
            if (indidx != null)
            {
                Expression ea = frame.EnsureRegister(indidx.ARegister);
                if (indidx.Imm8 != 0)
                    ea = m.IAdd(ea, Constant.Int32(indidx.Imm8));
                Expression ix = frame.EnsureRegister(indidx.XRegister);
                if (indidx.Scale > 1)
                    ix = m.IMul(ix, Constant.Int32(indidx.Scale));
                var load = m.Load(dataWidth, m.IAdd(ea, ix));
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            throw new NotImplementedException("Unimplemented RewriteDst for operand type " + operand.ToString());
        }

        private MemoryAccess RewriteMemoryAccess(MemoryOperand mem, PrimitiveType dataWidth, Address addrInstr)
        {
            Expression ea;
            if (mem.Base == Registers.pc)
            {
                ea = addrInstr + mem.Offset.ToInt32();
            }
            else
            {
                var bReg = frame.EnsureRegister(mem.Base);
                ea = bReg;
                if (mem.Offset != null)
                {
                    ea = m.IAdd(bReg, Constant.Int32(mem.Offset.ToInt32()));
                }
            }
            return m.Load(dataWidth, ea);
        }

        public Expression RewriteUnary(
            MachineOperand operand, 
            Address addrInstr,
            PrimitiveType dataWidth,
            Func<Expression, Expression> opGen)
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
            var addr = operand as M68kAddressOperand;
            if (addr != null)
            {
                var load = m.Load(dataWidth, addr.Address);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(load, tmp);
                return tmp;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var load = RewriteMemoryAccess(mem, dataWidth, addrInstr);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(RewriteMemoryAccess(mem,dataWidth, addrInstr), tmp);
                return tmp;
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = frame.EnsureRegister(post.Register);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(m.Load(dataWidth, r)));
                m.Assign(m.Load(dataWidth, r), tmp);
                m.Assign(r, m.IAdd(r, dataWidth.Size));
                return tmp;
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

        public Expression RewriteMoveDst(MachineOperand opDst, Address addrInstr, PrimitiveType dataWidth, Expression src)
        {
            var reg = opDst as RegisterOperand;
            if (reg != null)
            {
                var r = frame.EnsureRegister(reg.Register);
                if (r.DataType.Size > dataWidth.Size)
                {
                    var tmp = frame.CreateTemporary(dataWidth);
                    m.Assign(r, m.Dpb(r, src, 0, dataWidth.BitSize));
                    return tmp;
                }
                else
                {
                    m.Assign(r, src);
                    return r;
                }
            }
            var mem = opDst as MemoryOperand;
            if (mem != null)
            {
                src = Spill(src, frame.EnsureRegister(mem.Base));
                var load = RewriteMemoryAccess(mem, dataWidth,addrInstr);
                var tmp = frame.CreateTemporary(dataWidth);
                m.Assign(load, src);
                return tmp;
            }
            var post = opDst as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = frame.EnsureRegister(post.Register);
                var rExp = Spill(src, r);
                var load = m.Load(dataWidth, r);
                m.Assign(load, rExp);
                m.Assign(r, m.IAdd(r, dataWidth.Size));
                return src;
            }
            var pre = opDst as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = frame.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, dataWidth.Size));
                var rExp = Spill(src, r);
                var load = m.Load(dataWidth, rExp);
                m.Assign(load, src);
                return src;
            }
            var indidx = opDst as IndirectIndexedOperand;
            if (indidx != null)
            {
                var a = frame.EnsureRegister(indidx.ARegister);
                var x = frame.EnsureRegister(indidx.XRegister);
                var load = m.Load(dataWidth, m.IAdd(a, x));
                m.Assign(load, src);
                return src;
            }
            var mAddr = opDst as M68kAddressOperand;
            if (mAddr != null)
            {
                m.Assign(
                    m.Load(
                        dataWidth, 
                        Constant.Word32(mAddr.Address.ToUInt32())),
                    src);
                return src;
            }
            throw new NotImplementedException("Unimplemented RewriteMoveDst for operand type " + opDst.GetType().Name);
        }

        private Expression Spill(Expression src, Identifier r)
        {
            if (src is MemoryAccess || src == r)
            {
                var tmp = frame.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = tmp;
            }
            return src;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return frame.EnsureFlagGroup((uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }
    }
}
