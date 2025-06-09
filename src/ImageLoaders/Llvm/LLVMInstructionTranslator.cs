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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrConstant = Reko.Core.Expressions.Constant;
using IrDomain = Reko.Core.Types.Domain;
using PhiAssignment = Reko.Core.Code.PhiAssignment;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMInstructionTranslator : InstructionVisitor<int>
    {
        private ProgramBuilder builder;
        private ProcedureBuilder m;
        private Dictionary<Statement, PhiInstruction> unresolvedPhis;

        public LLVMInstructionTranslator(ProgramBuilder builder, ProcedureBuilder m)
        {
            this.builder = builder;
            this.m = m;
            this.unresolvedPhis = new Dictionary<Statement, PhiInstruction>();
        }

        public int VisitAlloca(Alloca alloca)
        {
            var type = builder.TranslateType(alloca.Type);
            int count = 1;
            if (alloca.ElementCount is not null)
            {
                throw new NotImplementedException();
            }
            var stk = m.AllocateStackVariable(type, count);
            var dst = m.CreateLocalId("loc", type);
            m.Assign(dst, m.AddrOf(m.Procedure.Architecture.PointerType, stk));
            return 0;
        }

        public int VisitBinary(Binary bin)
        {
            var type = builder.TranslateType(bin.Type);
            var left = MakeValueExpression(bin.Left, type);
            var right = MakeValueExpression(bin.Right, type);
            var dst = m.CreateLocalId("loc", type);
            Func<Expression, Expression, Expression> fn;
            switch (bin.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("TranslateInstruction({0})", bin.Operator));
            case TokenType.add: fn = m.IAdd; break;
            case TokenType.sub: fn = m.ISub; break;
            case TokenType.mul: fn = m.IMul; break;
            case TokenType.sdiv: fn = m.SDiv; break;
            case TokenType.srem: fn = m.SMod; break;
            }
            m.Assign(dst, fn(left, right));
            return 0;
        }

        public int VisitBitwiseBinary(BitwiseBinary bit)
        {
            var type = builder.TranslateType(bit.Type);
            var left = MakeValueExpression(bit.Left, type);
            var right = MakeValueExpression(bit.Right, type);
            var dst = m.CreateLocalId("loc", type);
            Func<Expression, Expression, Expression> fn;
            switch (bit.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("VisitBitwiseBinary({0})", bit.Operator));
            case TokenType.and: fn = m.And; break;
            case TokenType.ashr: fn = m.Sar; break;
            case TokenType.shl: fn = m.Shl; break;
            case TokenType.or: fn = m.Or; break;
            case TokenType.xor: fn = m.Xor; break;
            }
            m.Assign(dst, fn(left, right));
            return 0;
        }

        public int VisitBr(BrInstr br)
        {
            if (br.Cond is null)
            {
                m.Goto(br.IfTrue.Name);
                return 0;
            }
            var cond = MakeValueExpression(br.Cond, builder.TranslateType(br.Type));
            m.Branch(cond, br.IfTrue.Name, br.IfFalse.Name);
            return 0;
        }

        public int VisitCall(LLVMCall call)
        {
            var args = new List<Expression>();
            foreach (var arg in call.Arguments)
            {
                var type = builder.TranslateType(arg.Type!);
                var irArg = MakeValueExpression(arg.Value!, type);
                args.Add(irArg);
            }
            var retType = builder.TranslateType(call.FnType);
            var fn = MakeValueExpression(call.FnPtr, null!);
            var app = m.Fn(fn, retType, args.ToArray());
            if (call.Result is not null)
            {
                var dst = m.CreateLocalId("loc", retType);
                m.Assign(dst, app);
            }
            else
            {
                m.SideEffect(app);
            }
            return 0;
        }

        public int VisitCmp(CmpInstruction cmp)
        {
            var srcType = builder.TranslateType(cmp.Type!);
            var op1 = MakeValueExpression(cmp.Op1!, srcType);
            var op2 = MakeValueExpression(cmp.Op2!, srcType);
            var dst = m.CreateLocalId("loc", PrimitiveType.Bool);
            Func<Expression, Expression, Expression> fn;
            if (cmp.Operator == TokenType.icmp)
            {
                switch (cmp.ConditionCode)
                {
                default:
                    throw new NotImplementedException(string.Format("TranslateCmp({0})", cmp.ConditionCode));
                case TokenType.eq: fn = m.Eq; break;
                case TokenType.ne: fn = m.Ne; break;
                case TokenType.sge: fn = m.Ge; break;
                case TokenType.sgt: fn = m.Gt; break;
                case TokenType.sle: fn = m.Le; break;
                case TokenType.slt: fn = m.Lt; break;
                case TokenType.uge: fn = m.Uge; break;
                case TokenType.ugt: fn = m.Ugt; break;
                case TokenType.ule: fn = m.Ule; break;
                case TokenType.ult: fn = m.Ult; break;
                }
            }
            else if (cmp.Operator == TokenType.fcmp)
            {
                switch (cmp.ConditionCode)
                {
                default:
                    throw new NotImplementedException(string.Format("TranslateCmp({0})", cmp.ConditionCode));
                }
            }
            else
                throw new NotImplementedException(string.Format("TranslateCmp({0})", cmp.Operator));

            m.Assign(dst, fn(op1, op2));
            return 0;
        }

        public int VisitConversion(Conversion conv)
        {
            var dstType = builder.TranslateType(conv.TypeTo);
            var srcType = builder.TranslateType(conv.TypeFrom);
            var src = MakeValueExpression(conv.Value, srcType);
            var dst = m.CreateLocalId("loc", dstType);
            Expression e;
            switch (conv.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("TranslateConversion({0},{1},{2})", conv.Operator, srcType, dstType));
            case TokenType.bitcast:     // reinterpret_cast requires no convert
                e = src;
                break;
            case TokenType.inttoptr:
                dstType = PrimitiveType.Create(IrDomain.Pointer, srcType.BitSize);
                e = m.Convert(src, src.DataType, dstType);
                break;
            case TokenType.sext:
                dstType = PrimitiveType.Create(IrDomain.SignedInt, dstType.BitSize);
                e = m.Convert(src, src.DataType, dstType);
                break;
            case TokenType.zext:
                dstType = PrimitiveType.Create(IrDomain.UnsignedInt, dstType.BitSize);
                e = m.Convert(src, src.DataType, dstType);
                break;
            case TokenType.ptrtoint:
            case TokenType.trunc:
                e = m.Convert(src, src.DataType, dstType);
                break;
            }
            m.Assign(dst, e);
            return 0;
        }

        public int VisitExtractvalue(Extractvalue ext)
        {
            throw new NotImplementedException();
        }

        public int VisitFence(Fence fence)
        {
            throw new NotImplementedException();
        }

        public int VisitGetelementptr(GetElementPtr get)
        {
            var e = GetElementPtr(get.PtrType, get.PtrValue, get.Indices);
            var dst = m.CreateLocalId("loc", VoidType.Instance);
            m.Assign(dst, m.AddrOf(m.Procedure.Architecture.PointerType, e));
            return 0;
        }

        private Expression GetElementPtr(
            LLVMType ptrType, 
            Value ptrValue, 
            List<(LLVMType, Value?)> indices)
        {
            var type = builder.TranslateType(ptrType);
            var e = MakeValueExpression(ptrValue, type);
            foreach (var index in indices)
            {
                long? idx = null;
                var dt = builder.TranslateType(index.Item1);
                var i = MakeValueExpression(index.Item2!, dt);
                if (i is IrConstant con)
                {
                    idx = Convert.ToInt64(con.ToInt64());
                }
                if (type is Pointer ptr)
                {
                    if (idx.HasValue && idx.Value == 0)
                    {
                        e = m.Deref(e);
                    }
                    else
                    {
                        e = m.Array(ptr.Pointee, e, i);
                    }
                    e.DataType = ptr.Pointee;
                    type = ptr.Pointee;
                    continue;
                }
                if (type is ArrayType a)
                {
                    e = m.Array(a.ElementType, e, i);
                    e.DataType = a.ElementType;
                    type = a.ElementType;
                    continue;
                }
                throw new NotImplementedException(index.Item1.ToString());
            }
            return e;
        }

        public int VisitLoad(Load load)
        {
            m.EnsureBlock(null!);
            var dstType = builder.TranslateType(load.DstType);
            var srcType = builder.TranslateType(load.SrcType);
            var ea = MakeValueExpression(load.Src, srcType);
            var dst = m.CreateLocalId("loc", dstType);
            m.Assign(dst, m.Deref(ea));
            return 0;
        }

        public int VisitPhi(PhiInstruction phi)
        {
            m.EnsureBlock(null!);
            var dstType = builder.TranslateType(phi.Type);
            var dst = m.CreateLocalId("loc", dstType);
            var stm = m.Emit(new PhiAssignment(dst));
            unresolvedPhis.Add(stm, phi);
            return 0;
        }

        public void ResolvePhis()
        {
            foreach (var de in unresolvedPhis)
            {
                var args = new List<PhiArgument>(de.Value.Arguments.Count);
                var type = builder.TranslateType(de.Value.Type);
                foreach (var arg in de.Value.Arguments)
                {
                    var val = MakeValueExpression(arg.Item1, type);
                    args.Add(new PhiArgument(
                        m.BlockOf(arg.Item2.Name, false),
                        val));
                }
                var phi = (PhiAssignment)de.Key.Instruction;
                phi.Src = new PhiFunction(type, args.ToArray());
            }
        }

        public int VisitRet(RetInstr ret)
        {
            if (ret.Value is null)
            {
                m.Return();
            }
            else
            {
                var e = MakeValueExpression(ret.Value, builder.TranslateType(ret.Type));
                m.Return(e);
            }
            return 0;
        }

        public int VisitSelect(Select select)
        {
            var condType = builder.TranslateType(select.CondType);
            var condExp = MakeValueExpression(select.Cond, condType);

            var trueType = builder.TranslateType(select.TrueType);
            var trueExp = MakeValueExpression(select.TrueValue, trueType);

            var falseType = builder.TranslateType(select.FalseType);
            var falseExp = MakeValueExpression(select.FalseValue, falseType);

            var dst = m.CreateLocalId("loc", trueType);
            m.Assign(dst, m.Conditional(trueType, condExp, trueExp, falseExp));

            return 0;
        }

        public int VisitStore(Store store)
        {
            var dstType = builder.TranslateType(store.DstType);
            var srcType = builder.TranslateType(store.SrcType);
            var src = MakeValueExpression(store.Src, srcType);
            var ea = MakeValueExpression(store.Dst, dstType);
            m.Emit(new Core.Code.Store(m.Deref(ea), src));
            return 0;
        }

        public int VisitSwitch(Switch sw)
        {
            throw new NotImplementedException();
        }

        public int VisitUnreachable(Unreachable unreachable)
        {
            //$REVIEW: no equivalent in Reko. Does it matter?
            m.TerminateBlock();
            return 0;
        }

        private Expression MakeValueExpression(Value value, DataType dt)
        {
            switch (value)
            {
            case Constant c:
                if (c.Value is null)
                {
                    var w = PrimitiveType.CreateWord(dt.BitSize);
                    var v = IrConstant.Create(w, 0);
                    v.DataType = w;
                    return v;
                }
                else
                {
                    return IrConstant.Create(dt, Convert.ToInt64(c.Value));
                }
            case Literal l:
                if (l.Type == TokenType.HexInteger)
                {
                    var val = Convert.ToInt64(l.Value, 16);
                    if (dt is Pointer ptr)
                    {
                        return Address.Create(ptr, (ulong)val);
                    }
                    return IrConstant.Create(dt, val);
                }
                throw new NotImplementedException();
            case LocalId local:
                return m.GetLocalId(local.Name);
            case GlobalId global:
                return builder.Globals[global.Name];
            case GetElementPtrExpr get:
                return GetElementPtr(get.PointerType!, get.Pointer!, get.Indices!);
            }
            throw new NotImplementedException(string.Format("MakeValueExpression: {0} {1}", value.GetType().Name, value.ToString() ?? "(null)"));
        }
    }
}
