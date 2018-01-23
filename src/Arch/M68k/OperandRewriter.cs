#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
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
        private IStorageBinder binder;

        public OperandRewriter(M68kArchitecture arch, RtlEmitter emitter, IStorageBinder binder, PrimitiveType dataWidth)
        {
            this.arch = arch;
            this.m = emitter;
            this.binder = binder;
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
                Expression r = binder.EnsureRegister(reg.Register);
                if (DataWidth != null && DataWidth.Size != reg.Width.Size)
                    r = m.Cast(DataWidth, r);
                return r;
            }
            var imm = operand as M68kImmediateOperand;
            if (imm != null)
            {
                if (imm.Width.Domain == Domain.Real)
                    return imm.Constant.CloneExpression();
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
                var ea = binder.EnsureRegister(pre.Register);
                m.Assign(ea, m.ISub(ea, m.Int32(DataWidth.Size)));
                return m.Load(DataWidth, ea);
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = binder.EnsureRegister(post.Register);
                var tmp = binder.CreateTemporary(DataWidth);
                m.Assign(tmp, m.Load(DataWidth, r));
                m.Assign(r, m.IAdd(r, m.Int32(DataWidth.Size)));
                return tmp;
            }
            var indidx = operand as IndirectIndexedOperand;
            if (indidx != null)
            {
                Expression ea = RewriteIndirectBaseRegister(indidx, addrInstr);
                Expression ix = binder.EnsureRegister(indidx.XRegister);
                if (indidx.XWidth.Size != 4)
                    ix = m.Cast(PrimitiveType.Int32, m.Cast(PrimitiveType.Int16, ix));
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
                    if (indop.index_reg_width.BitSize != 32)
                        idx = m.Cast(PrimitiveType.Word32, m.Cast(PrimitiveType.Int16, idx));
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

        private Expression RewriteIndirectBaseRegister(IndirectIndexedOperand indidx, Address addrInstr)
        {
            if (indidx.ARegister == Registers.pc)
            {
                // pc-relative instruction.
                return addrInstr + (2 + indidx.Imm8);
            }
            Expression ea = binder.EnsureRegister(indidx.ARegister);
            if (indidx.Imm8 != 0)
                ea = m.IAdd(ea, Constant.Int32(indidx.Imm8));
            return ea;
        }

        public Expression Combine(Expression e, RegisterStorage reg)
        {
            if (reg == null)
                return e;
            var r = binder.EnsureRegister(reg);
            if (e == null)
                return r;
            return m.IAdd(e, r);
        }

        public Expression Combine(Expression e, Expression o)
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

        public Expression RewriteDst(
            MachineOperand operand,
            Address addrInstr,
            PrimitiveType dataWidth,
            Expression src,
            Func<Expression, Expression, Expression> opGen)
        {
            var reg = operand as RegisterOperand;
            if (reg != null)
            {
                Expression r = binder.EnsureRegister(reg.Register);
                Expression tmp = r;
                if (dataWidth != null && 
                    reg.Width.BitSize > dataWidth.BitSize &&
                    reg.Width.Domain != Domain.Real)
                {
                    Expression rSub = m.Cast(dataWidth, r);
                    var srcExp = opGen(src, rSub);
                    if (srcExp is Identifier || srcExp is Constant || srcExp is DepositBits)
                    {
                        tmp = srcExp;
                    }
                    else
                    {
                        tmp = binder.CreateTemporary(dataWidth);
                        m.Assign(tmp, srcExp);
                    }
                    src = m.Dpb(r, tmp, 0);
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
                Identifier h = binder.EnsureRegister(dbl.Register1);
                Identifier l = binder.EnsureRegister( dbl.Register2);
                var d = binder.EnsureSequence(h.Storage, l.Storage, PrimitiveType.Word64);
                var result = opGen(src, l);
                m.Assign(d, result);
                return d;
            }
            var addr = operand as M68kAddressOperand;
            if (addr != null)
            {
                var load = m.Load(dataWidth, addr.Address);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var load = RewriteMemoryAccess(mem, dataWidth, addrInstr);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = binder.EnsureRegister(post.Register);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, m.Load(dataWidth, r))); 
                m.Assign(m.Load(dataWidth, r), tmp);
                m.Assign(r, m.IAdd(r, m.Int32(dataWidth.Size)));
                return tmp;
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = binder.EnsureRegister(pre.Register);
                src = Spill(src, r);
                m.Assign(r, m.ISub(r, m.Int32(dataWidth.Size)));
                var load = m.Load(dataWidth, r);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(m.Load(dataWidth, r), tmp);
                return tmp;
            }
            var indidx = operand as IndirectIndexedOperand;
            if (indidx != null)
            {
                Expression ea = binder.EnsureRegister(indidx.ARegister);
                if (indidx.Imm8 != 0)
                    ea = m.IAdd(ea, Constant.Int32(indidx.Imm8));
                Expression ix = binder.EnsureRegister(indidx.XRegister);
                if (indidx.Scale > 1)
                    ix = m.IMul(ix, Constant.Int32(indidx.Scale));
                var load = m.Load(dataWidth, m.IAdd(ea, ix));
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(src, load));
                m.Assign(load, tmp);
                return tmp;
            }
            return null;
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
                var bReg = binder.EnsureRegister(mem.Base);
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
                Expression r = binder.EnsureRegister(reg.Register);
                if (r.DataType.Size > dataWidth.Size)
                {
                    var tmp = binder.CreateTemporary(dataWidth);
                    m.Assign(tmp, opGen(m.Cast(dataWidth, r)));
                    m.Assign(r, m.Dpb(r, tmp, 0));
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
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(load, tmp);
                return tmp;
            }
            var mem = operand as MemoryOperand;
            if (mem != null)
            {
                var load = RewriteMemoryAccess(mem, dataWidth, addrInstr);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(RewriteMemoryAccess(mem,dataWidth, addrInstr), tmp);
                return tmp;
            }
            var post = operand as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = binder.EnsureRegister(post.Register);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(m.Load(dataWidth, r)));
                m.Assign(m.Load(dataWidth, r), tmp);
                m.Assign(r, m.IAdd(r, m.Int32(dataWidth.Size)));
                return tmp;
            }
            var pre = operand as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = binder.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, m.Int32(dataWidth.Size)));
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(m.Load(dataWidth, r)));
                m.Assign(m.Load(dataWidth, r), tmp);
                return tmp;
            }
            var indidx = operand as IndirectIndexedOperand;
            if (indidx != null)
            {
                Expression ea = binder.EnsureRegister(indidx.ARegister);
                if (indidx.Imm8 != 0)
                    ea = m.IAdd(ea, Constant.Int32(indidx.Imm8));
                Expression ix = binder.EnsureRegister(indidx.XRegister);
                if (indidx.Scale > 1)
                    ix = m.IMul(ix, Constant.Int32(indidx.Scale));
                var load = m.Load(dataWidth, m.IAdd(ea, ix));

                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(load, tmp);
                return tmp;
            }
            var indop = operand as IndexedOperand;
            if (indop != null)
            {
                Expression ea = Combine(indop.Base, indop.base_reg);
                if (indop.postindex)
                {
                    ea = m.LoadDw(ea);
                }
                if (indop.index_reg != null)
                {
                    var idx = Combine(null, indop.index_reg);
                    if (indop.index_reg_width.BitSize != 32)
                        idx = m.Cast(PrimitiveType.Word32, m.Cast(PrimitiveType.Int16, idx));
                    if (indop.index_scale > 1)
                        idx = m.IMul(idx, m.Int32(indop.index_scale));
                    ea = Combine(ea, idx);
                }
                if (indop.preindex)
                {
                    ea = m.LoadDw(ea);
                }
                ea = Combine(ea, indop.outer);
                var load =  m.Load(DataWidth, ea);

                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(tmp, opGen(load));
                m.Assign(load, tmp);
                return tmp;
            }
            throw new AddressCorrelatedException(
                addrInstr,
                "Unimplemented RewriteUnary for operand {0} of type {1}.", 
                operand.ToString(),
                operand.GetType().Name);
        }

        public Expression RewriteMoveDst(MachineOperand opDst, Address addrInstr, PrimitiveType dataWidth, Expression src)
        {
            var reg = opDst as RegisterOperand;
            if (reg != null)
            {
                var r = binder.EnsureRegister(reg.Register);
                if (r.DataType.Size > dataWidth.Size)
                {
                    var tmp = binder.CreateTemporary(dataWidth);
                    m.Assign(r, m.Dpb(r, src, 0));
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
                src = Spill(src, binder.EnsureRegister(mem.Base));
                var load = RewriteMemoryAccess(mem, dataWidth,addrInstr);
                var tmp = binder.CreateTemporary(dataWidth);
                m.Assign(load, src);
                return tmp;
            }
            var post = opDst as PostIncrementMemoryOperand;
            if (post != null)
            {
                var r = binder.EnsureRegister(post.Register);
                var rExp = Spill(src, r);
                var load = m.Load(dataWidth, r);
                m.Assign(load, rExp);
                m.Assign(r, m.IAdd(r, m.Int32(dataWidth.Size)));
                return src;
            }
            var pre = opDst as PredecrementMemoryOperand;
            if (pre != null)
            {
                var r = binder.EnsureRegister(pre.Register);
                m.Assign(r, m.ISub(r, m.Int32(dataWidth.Size)));
                var rExp = Spill(src, r);
                var load = m.Load(dataWidth, rExp);
                m.Assign(load, src);
                return src;
            }
            var idxop = opDst as IndexedOperand;
            if (idxop != null)
            {
                var b = binder.EnsureRegister(idxop.base_reg);
                var i = binder.EnsureRegister(idxop.index_reg);
                Expression ea = b;
                if (i != null)
                {
                    var s = m.Const(i.DataType, idxop.index_scale);
                    if (idxop.index_scale > 1)
                    {
                        ea = m.IMul(i, s);
                    }
                    else
                    {
                        ea = i;
                    }
                }
                if (b != null)
                { 
                    if (ea != null)
                    {
                        ea = m.IAdd(b, ea);
                    }
                    else
                    {
                        ea = b;
                    }
                }
                if (idxop.Base != null)
                {
                    if (ea != null)
                    {
                        ea = m.IAdd(ea, idxop.Base);
                    }
                    else
                    {
                        ea = idxop.Base;
                    }
                }
                var load = m.Load(dataWidth, ea);
                m.Assign(load, src);
                return src;
            }
            var indidx = opDst as IndirectIndexedOperand;
            if (indidx != null)
            {
                var a = binder.EnsureRegister(indidx.ARegister);
                var x = binder.EnsureRegister(indidx.XRegister);
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
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = tmp;
            }
            return src;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(Registers.ccr, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }
    }
}
