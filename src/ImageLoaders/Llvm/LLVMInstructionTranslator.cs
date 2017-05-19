#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrConstant = Reko.Core.Expressions.Constant;
using IrDomain = Reko.Core.Types.Domain;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMInstructionTranslator : InstructionVisitor<int>
    {
        private ProgramBuilder builder;
        private ProcedureBuilder m;

        public LLVMInstructionTranslator(ProgramBuilder builder, ProcedureBuilder m)
        {
            this.builder = builder;
            this.m = m;
        }

        public int VisitAlloca(Alloca alloca)
        {
            var type = builder.TranslateType(alloca.Type);
            int count = 1;
            if (alloca.ElementCount != null)
            {
                throw new NotImplementedException();
            }
            var stk = m.AllocateStackVariable(type, count);
            var dst = m.CreateLocalId("loc", type);
            m.Assign(dst, m.AddrOf(stk));
            return 0;
        }

        public int VisitBinary(Binary bin)
        {
            var type = builder.TranslateType(bin.Type);
            var left = MakeValueExpression(bin.Left, m, type);
            var right = MakeValueExpression(bin.Right, m, type);
            var dst = m.CreateLocalId("loc", type);
            Func<Expression, Expression, Expression> fn;
            switch (bin.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("TranslateInstruction({0})", bin.Operator));
            case TokenType.add: fn = m.IAdd; break;
            }
            m.Assign(dst, fn(left, right));
            return 0;
        }

        public int VisitBitwiseBinary(BitwiseBinary bit)
        {
            throw new NotImplementedException();
        }

        public int VisitBr(BrInstr br)
        {
            if (br.Cond == null)
            {
                m.Goto(br.IfTrue.Name);
                return 0;
            }
            var cond = MakeValueExpression(br.Cond, m, builder.TranslateType(br.Type));
            m.Branch(cond, br.IfTrue.Name, br.IfFalse.Name);
            return 0;
        }

        public int VisitCall(LLVMCall call)
        {
            var args = new List<Expression>();
            foreach (var arg in call.Arguments)
            {
                var type = builder.TranslateType(arg.Type);
                var irArg = MakeValueExpression(arg.Value, m, type);
                args.Add(irArg);
            }
            var retType = builder.TranslateType(call.FnType);
            var fn = MakeValueExpression(call.FnPtr, m, null);
            var app = m.Fn(fn, retType, args.ToArray());
            if (call.Result != null)
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
            var srcType = builder.TranslateType(cmp.Type);
            var op1 = MakeValueExpression(cmp.Op1, m, srcType);
            var op2 = MakeValueExpression(cmp.Op2, m, srcType);
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
            var src = MakeValueExpression(conv.Value, m, srcType);
            var dst = m.CreateLocalId("loc", dstType);
            Expression e;
            switch (conv.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("TranslateConversion({0})", conv.Operator));
            case TokenType.sext:
                dstType = PrimitiveType.Create(IrDomain.SignedInt, dstType.Size);
                e = m.Cast(dstType, src);
                break;
            case TokenType.trunc:
                e = m.Cast(dstType, src);
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
            var type = builder.TranslateType(get.PtrType);
            Expression e = MakeValueExpression(get.PtrValue, m, type);
            foreach (var index in get.Indices)
            {
                var idx = Convert.ToInt64(((Constant)index.Item2).Value);
                var ptr = type as Pointer;
                if(ptr != null)
                {
                    if (idx == 0)
                    {
                        e = m.Deref(e);
                    }
                    else
                    {
                        e = m.Array(ptr.Pointee, e, IrConstant.Int32((int)idx));
                    }
                    e.DataType = ptr.Pointee;
                    type = ptr.Pointee;
                    continue;
                }
                var a = type as ArrayType;
                if (a != null)
                {
                    e = m.Array(a.ElementType, e, IrConstant.Int32((int)idx));
                    e.DataType = a.ElementType;
                    type = a.ElementType;
                    continue;
                }
            }
            var dst = m.CreateLocalId("loc", VoidType.Instance);
            m.Assign(dst, m.AddrOf(e));
            return 0;
        }

        public int VisitLoad(Load load)
        {
            var dstType = builder.TranslateType(load.DstType);
            var srcType = builder.TranslateType(load.SrcType);
            var ea = MakeValueExpression(load.Src, m, srcType);
            var dst = m.CreateLocalId("loc", dstType);
            m.Assign(dst, m.Load(dstType, ea));
            return 0;
        }

        public int VisitPhi(PhiInstruction phi)
        {
            throw new NotImplementedException();
        }

        public int VisitRet(RetInstr ret)
        {
            if (ret.Value == null)
            {
                m.Return();
            }
            else
            {
                var e = MakeValueExpression(ret.Value, m, builder.TranslateType(ret.Type));
                m.Return(e);
            }
            return 0;
        }

        public int VisitSelect(Select select)
        {
            throw new NotImplementedException();
        }

        public int VisitStore(Store store)
        {
            var dstType = builder.TranslateType(store.DstType);
            var srcType = builder.TranslateType(store.SrcType);
            var src = MakeValueExpression(store.Src, m, srcType);
            var ea = MakeValueExpression(store.Dst, m, dstType);
            m.Store(ea, src);
            return 0;
        }

        public int VisitSwitch(Switch sw)
        {
            throw new NotImplementedException();
        }

        public int VisitUnreachable(Unreachable unreachable)
        {
            throw new NotImplementedException();
        }

        private Expression MakeValueExpression(Value value, ProcedureBuilder m, DataType dt)
        {
            var c = value as Constant;
            if (c != null)
            {
                return IrConstant.Create(dt, Convert.ToInt64(c.Value));
            }
            var local = value as LocalId;
            if (local != null)
            {
                return m.GetLocalId(local.Name);
            }
            var global = value as GlobalId;
            if (global != null)
            {
                return builder.Globals[global.Name];
            }
            throw new NotImplementedException(string.Format("MakeValueExpression: {0}", value.ToString() ?? "(null)"));
        }

    }
}
