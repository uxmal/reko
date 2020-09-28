#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Serialization.Json
{
    public class JsonProcedureSerializer : InstructionVisitor, IExpressionVisitor
    {
        private TextWriter w;
        private JsonWriter js;

        public string Serialize(Procedure proc)
        {
            var sw = new StringWriter();
            this.js = new JsonWriter(sw);
            Serialize(proc, sw);
            return sw.ToString();
        }

        public void Serialize(Procedure proc, TextWriter w)
        {
            this.w = w;
            js.BeginObject();
            js.WriteKeyValue("name", proc.Name);
            js.WriteKeyValue("signature", () => WriteSignature(proc.Signature));
            js.WriteKeyValue("ids", () => WriteIdentifiers(proc));
            js.WriteKeyValue("blocks", () => WriteBlocks(proc));
            js.EndObject();
        }

        private void WriteSignature(FunctionType sig)
        {
            if (sig.ParametersValid)
                throw new NotImplementedException();
            else
            {
                js.Write("");
            }
        }

        private void WriteIdentifiers(Procedure proc)
        {
            var idsByName = new SortedList<string, Identifier>();
            var idCollector = new IdentifierCollector(idsByName);
            if (proc.Signature.ParametersValid)
                throw new NotImplementedException();
            
            foreach (var stm in proc.Statements)
            {
                stm.Instruction.Accept(idCollector);
            }
            if (idsByName.Count > 0)
            {
                js.WriteList(idsByName.Values, WriteIdentifier);
            }
        }

        private void WriteIdentifier(Identifier id)
        {
            js.BeginObject();
            js.WriteKeyValue("name", id.Name);
            if (!(id.Storage is MemoryStorage))
            {
                js.WriteKeyValue("type", () => WriteType(id.DataType));
            }
            js.WriteKeyValue("stg", () => WriteStorage(id.Storage));
            js.EndObject();
        }

        private void WriteType(DataType dt)
        {
            if (JsonSymbols.PrimitiveNames.TryGetValue(dt, out var primitiveName))
            {
                js.Write(primitiveName);
            }
            else
                throw new NotImplementedException(dt.ToString());
        }

        private void WriteStorage(Storage stg)
        {
            js.BeginObject();
            switch (stg)
            {
            case RegisterStorage reg:
                js.WriteKeyValue("kind", "reg");
                js.WriteKeyValue("name", reg.Name);
                break;
            case MemoryStorage mem:
                js.WriteKeyValue("kind", "mem");
                break;
            case FlagGroupStorage flg:
                js.WriteKeyValue("kind", "flg");
                js.WriteKeyValue("grf", flg.FlagGroupBits);
                js.WriteKeyValue("reg", flg.FlagRegister.Name);
                break;
            default:
                throw new NotImplementedException(string.Format("Unimplemented storage: {0}.", stg));
            }
            js.EndObject();
        }

        private void WriteBlocks(Procedure proc)
        {
            js.WriteList(proc.ControlGraph.Blocks, b => WriteBlock(b, b == proc.ExitBlock));
        }

        private void WriteBlock(Block block, bool isExit)
        {
            js.BeginObject();
            js.WriteKeyValue("name", block.Name);
            if (isExit)
            {
                js.WriteKeyValue("exit", isExit);
            }
            if (block.Statements.Count > 0)
            {
                var linaddr = block.Statements[0].LinearAddress;
                js.WriteKeyValue("linaddr", linaddr);
                js.WriteKeyValue("stms", () => js.WriteList(block.Statements, stm => WriteStatement(stm, linaddr)));
            }
            if (block.Succ.Count > 0)
            {
                js.WriteKeyValue("succ", () => js.WriteList(block.Succ.Select(s => s.Name), js.Write));
            }
            js.EndObject();
        }

        private void WriteStatement(Statement stm, ulong linAddrBlock)
        {
            w.Write("[");
            js.Write(stm.LinearAddress - linAddrBlock);
            w.Write(',');
            stm.Instruction.Accept(this);
            w.Write("]");
        }

        public void VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public void VisitApplication(Application appl)
        {
            w.Write('[');
            appl.Procedure.Accept(this);
            w.Write(',');
            js.WriteListContents(appl.Arguments, e => e.Accept(this));
        }

        public void VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public void VisitAssignment(Assignment ass)
        {
            js.Write("=");
            w.Write(',');
            ass.Dst.Accept(this);
            w.Write(',');
            ass.Src.Accept(this);
        }

        public void VisitBinaryExpression(BinaryExpression binExp)
        {
            w.Write('[');
            js.Write(JsonSymbols.OpNames[binExp.Operator]);
            w.Write(',');
            binExp.Left.Accept(this);
            w.Write(',');
            binExp.Right.Accept(this);
            w.Write(']');
        }

        public void VisitBranch(Branch branch)
        {
            js.Write("bra");
            w.Write(',');
            branch.Condition.Accept(this);
            w.Write(',');
            js.Write(branch.Target.Name);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            js.Write("call");
            w.Write(',');
            ci.Callee.Accept(this);
            w.Write(',');
            js.Write(ci.CallSite.SizeOfReturnAddressOnStack);
            w.Write(',');
            js.Write(ci.CallSite.FpuStackDepthBefore);
            w.Write(',');
            js.WriteList(ci.Uses.Select(u => u.Expression), e => e.Accept(this));
            js.WriteList(ci.Definitions.Select(d => d.Expression.ToString()), id => js.Write(id.ToString()));
        }

        private void WriteUseInstruction(UseInstruction ui)
        {
        }

        public void VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        public void VisitComment(CodeComment comment)
        {
            js.Write("cmt");
            w.Write(",");
            w.Write(comment.Text);
        }

        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        public void VisitConditionOf(ConditionOf cof)
        {
            js.Write("cof");
            w.Write(',');
            cof.Expression.Accept(this);
        }

        public void VisitConstant(Constant c)
        {
            w.Write('[');
            js.Write(c.ToInt64());
            w.Write(',');
            WriteType(c.DataType);
            w.Write(']');
        }

        public void VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public void VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public void VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        public void VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public void VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public void VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public void VisitIdentifier(Identifier id)
        {
            js.Write(id.Name);
        }

        public void VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public void VisitMemoryAccess(MemoryAccess access)
        {
            w.Write('[');
            js.Write("m");
            w.Write(',');
            access.MemoryId.Accept(this);
            w.Write(',');
            access.EffectiveAddress.Accept(this);
            w.Write(',');
            WriteType(access.DataType);
            w.Write(']');
        }

        public void VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        public void VisitOutArgument(OutArgument outArgument)
        {
            throw new NotImplementedException();
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public void VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public void VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public void VisitProcedureConstant(ProcedureConstant pc)
        {
            js.Write(pc.Procedure.Name);
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            js.Write("ret");
            if (ret.Expression != null)
            {
                w.Write(',');
                ret.Expression.Accept(this);
            }
        }

        public void VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public void VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new NotImplementedException();
        }

        public void VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        public void VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        public void VisitStore(Store store)
        {
            w.Write('[');
            js.Write("st");
            w.Write(',');
            store.Dst.Accept(this);
            w.Write(',');
            store.Src.Accept(this);
            w.Write(']');
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public void VisitTestCondition(TestCondition tc)
        {
            w.Write('[');
            js.Write("test");
            w.Write(',');
            js.Write(tc.ConditionCode.ToString());
            w.Write(',');
            tc.Expression.Accept(this);
            w.Write(']');
        }

        public void VisitUnaryExpression(UnaryExpression unary)
        {
            w.Write('[');
            js.Write(JsonSymbols.OpNames[unary.Operator]);
            w.Write(',');
            unary.Expression.Accept(this);
            w.Write(']');
        }

        public void VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }
    }
}
