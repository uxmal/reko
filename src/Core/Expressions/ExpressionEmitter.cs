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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Expressions
{
    /// <summary>
    /// Factory class that has the extra benefit of reducing the verbosity of the code.
    /// </summary>
    public class ExpressionEmitter
    {
        public ArrayAccess Array(DataType elemType, Expression arrayPtr, Expression index)
        {
            return new ArrayAccess(elemType, arrayPtr, index);
        }

        /// <summary>
        /// Integer addition.
        /// </summary>
        public BinaryExpression IAdd(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.IAdd, left.DataType, left, right);
        }

        public BinaryExpression IAdd(Expression left, int right)
        {
            return new BinaryExpression(Operator.IAdd, left.DataType, left, Constant.Create(left.DataType, right));
        }

        public UnaryExpression AddrOf(Expression e)
        {
            return new UnaryExpression(UnaryOperator.AddrOf, PrimitiveType.Pointer32, e);
        }

        public BinaryExpression And(Expression left, ulong right)
        {
            return And(left, Constant.Create(left.DataType, right));
        }

        public BinaryExpression And(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.And, left.DataType, left, right);
        }

        public Expression Cand(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cand, PrimitiveType.Bool, a, b);
        }

        public Cast Cast(DataType dataType, Expression expr)
        {
            return new Cast(dataType, expr);
        }

        public Expression Comp(Expression expr)
        {
            return new UnaryExpression(Operator.Comp, expr.DataType, expr); 
        }

        public ConditionOf Cond(Expression expr)
        {
            return new ConditionOf(expr);
        }

        public Expression Cor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cor, PrimitiveType.Bool, a, b);
        }

        public Dereference Deref(Expression a)
        {
            return new Dereference(a.DataType, a);
        }

        public DepositBits Dpb(Expression dst, Expression src, int offset, int bitCount)
        {
            return new DepositBits(dst, src, offset, bitCount);
        }

        public BinaryExpression Eq(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Eq(Expression a, int b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, Constant.Create(a.DataType, b));
        }

        public BinaryExpression Eq0(Expression exp)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, exp, Constant.Create(exp.DataType, 0));
        }

        public Expression FAdd(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.FAdd, PrimitiveType.Real64, a, b);
        }

        public Expression FDiv(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.FDiv, PrimitiveType.Real64, a, b);
        }

        public FieldAccess Field(DataType dt, Expression e, string name)
        {
            return new FieldAccess(dt, e, name);
        }

        public Expression FMul(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.FMul, PrimitiveType.Real64, a, b);
        }

        public Application Fn(Expression e, params Expression[] exps)
        {
            return Fn(e, PrimitiveType.Word32, exps);
        }

        public Application Fn(Expression fn, DataType retType, params Expression[] exps)
        {
            return new Application(fn, retType, exps);
        }

        public Application Fn(ExternalProcedure ep, params Expression[] args)
        {
            return new Application(
                new ProcedureConstant(PrimitiveType.Pointer32, ep), 
                ep.Signature.ReturnValue.DataType,
                args);
        }

        public Application Fn(PseudoProcedure ppp, params Expression[] args)
        {
            return new Application(new ProcedureConstant(PrimitiveType.Pointer32, ppp), ppp.ReturnType, args);
        }

        public Expression FSub(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.FSub, PrimitiveType.Real64, a, b);
        }

        public Expression Ge(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ge, PrimitiveType.Bool, a, b);
        }

        public Expression Ge(Expression a, int b)
        {
            return Ge(a, Constant.Create(a.DataType, b));
        }

        public Expression Gt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Gt, PrimitiveType.Bool, a, b);
        }

        public Expression Gt(Expression a, int b)
        {
            return Gt(a, Int32(b));
        }

        public Constant Int8(int n)
        {
            return Constant.SByte((sbyte)n);
        }

        public Constant Int16(short n)
        {
            return Constant.Word16((ushort)n);
        }

        public Constant Int16(uint n)
        {
            return Constant.Int16((short) n);
        }

        public Constant Int32(uint n)
        {
            return Constant.Word32(n);
        }

        public Constant Int32(int n)
        {
            return Constant.Word32(n);
        }

        public MemoryAccess Load(DataType dt, Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, dt);
        }

        public Expression LoadB(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Byte);
        }

        public MemoryAccess LoadDw(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word32);
        }

        public MemoryAccess LoadW(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word16);
        }

        public BinaryExpression Le(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Le, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Le(Expression a, int b)
        {
            return Le(a, Constant.Create(a.DataType, b));
        }

        public BinaryExpression Lt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Lt, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Lt(Expression a, int b)
        {
            return Lt(a, Constant.Create(a.DataType, b));
        }

        public MemberPointerSelector MembPtrW(Expression ptr, Expression membPtr)
        {
            return new MemberPointerSelector(PrimitiveType.Word16, new Dereference(PrimitiveType.Pointer32, ptr), membPtr);
        }

        public BinaryExpression Ne0(Expression expr)
        {
            return new BinaryExpression(
                Operator.Ne, PrimitiveType.Bool, expr, Constant.Create(expr.DataType, 0));
        }

        public UnaryExpression Neg(Expression expr)
        {
            return new UnaryExpression(
                Operator.Neg,
                expr.DataType,
                expr);
        }

        public OutArgument Out(DataType dt, Expression expr)
        {
            return new OutArgument(dt, expr);
        }

        public Expression Remainder(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.IMod,
                b.DataType,
                a, b);
        }

        public BinaryExpression SDiv(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.SDiv, b.DataType, a, b);
        }

        public MkSequence Seq(Expression head, Expression tail)
        {
            int totalSize = head.DataType.Size + tail.DataType.Size;
            Domain dom = (head.DataType == PrimitiveType.SegmentSelector)
                ? Domain.Pointer
                : ((PrimitiveType)head.DataType).Domain;
            return new MkSequence(PrimitiveType.Create(dom, totalSize), head, tail);
        }

        public SegmentedAccess SegMem(DataType dt, Expression basePtr, Expression ptr)
        {
            return new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ptr, dt);
        }

        public SegmentedAccess SegMemW(Expression basePtr, Expression ptr)
        {
            return new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ptr, PrimitiveType.Word16);
        }

        public Expression IMul(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.IMul, left.DataType, left, right);
        }

        public Expression IMul(Expression left, int c)
        {
            return new BinaryExpression(Operator.IMul, left.DataType, left, Constant.Create(left.DataType, c));
        }

        public Expression SMul(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.SMul, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, right);
        }

        public Expression SMul(Expression left, int c)
        {
            return new BinaryExpression(Operator.SMul, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, Constant.Create(left.DataType, c));
        }

        public Expression UMul(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.UMul, left.DataType, left, right);
        }

        public Expression UMul(Expression left, int c)
        {
            return new BinaryExpression(Operator.UMul, PrimitiveType.Create(Domain.UnsignedInt, left.DataType.Size), left, Constant.Create(left.DataType, c));
        }

        public BinaryExpression Ne(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Ne(Expression a, int n)
        {
            return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, Constant.Create(a.DataType, n));
        }

        /// <summary>
        /// Logical not operation
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public UnaryExpression Not(Expression exp)
        {
            return new UnaryExpression(Operator.Not, PrimitiveType.Bool, exp);
        }

        public Expression Or(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Or, a.DataType, a, b);
        }

        public BinaryExpression Sar(Expression e, byte sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, Constant.Byte(sh));
        }

        public BinaryExpression Sar(Expression e, Expression sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, sh);
        }

        public BinaryExpression Shl(Expression e, Expression sh)
        {
            return new BinaryExpression(Operator.Shl, e.DataType, e, sh);
        }

        public BinaryExpression Shl(Expression e, int sh)
        {
            return new BinaryExpression(Operator.Shl, e.DataType, e, Constant.Create(e.DataType, sh));
        }

        public BinaryExpression Shl(int c, Expression sh)
        {
            Constant cc = Constant.Word32(c);
            return new BinaryExpression(Operator.Shl, cc.DataType, cc, sh);
        }

        public BinaryExpression Shr(Expression exp, Expression sh)
        {
            return new BinaryExpression(Operator.Shr, exp.DataType, exp, sh);
        }

        public BinaryExpression Shr(Expression exp, byte c)
        {
            Constant cc = Constant.Byte(c);
            return new BinaryExpression(Operator.Shr, exp.DataType, exp, cc);
        }

        public BinaryExpression ISub(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.ISub, left.DataType, left, right);
        }

        public BinaryExpression ISub(Expression left, int right)
        {
            return ISub(left, Constant.Create(left.DataType, right));
        }

        public Slice Slice(PrimitiveType primitiveType, Identifier value, uint bitOffset)
        {
            return new Slice(primitiveType, value, bitOffset);
        }

        public TestCondition Test(ConditionCode cc, Expression expr)
        {
            return new TestCondition(cc, expr);
        }

        public BinaryExpression USub(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.USub, left.DataType, left, right);
        }

        public BinaryExpression UDiv(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.UDiv, b.DataType, a, b);
        }

        public BinaryExpression Ugt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ugt, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Uge(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Uge(Expression a, int n)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, Constant.Create(a.DataType, n));
        }

        public BinaryExpression Ule(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ule, PrimitiveType.Bool, a, b);
        }

        public Expression Ult(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, a, b);
        }

        public Expression Ult(Expression a, int b)
        {
            return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, a, Constant.Create(PrimitiveType.CreateWord(a.DataType.Size), b));
        }

        public Constant Byte(byte b)
        {
            return Constant.Byte(b);
        }

        public Constant Word16(ushort n)
        {
            return Constant.Word16(n);
        }

        public Constant Word16(uint n)
        {
            return Constant.Word16((ushort) n);
        }

        public Constant Word32(int n)
        {
            return Constant.Word32(n);
        }

        public Expression Xor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Xor, a.DataType, a, b);
        }
    }
}
