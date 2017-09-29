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

        public string Serialize(Procedure proc)
        {
            var sw = new StringWriter();
            Serialize(proc, sw);
            return sw.ToString();
        }

        public void Serialize(Procedure proc, TextWriter w)
        {
            this.w = w;
            w.Write('{');
            Write("name", proc.Name);
            w.Write(",");
            Write("signature", () => WriteSignature(proc.Signature));
            w.Write(",");
            Write("ids", () => WriteIdentifiers(proc));
            w.Write(",");
            Write("blocks", () => WriteBlocks(proc));
            w.Write('}');
        }

        private void WriteSignature(FunctionType sig)
        {
            if (sig.ParametersValid)
                throw new NotImplementedException();
            else
            {
                Write("");
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
                WriteList(idsByName.Values, WriteIdentifier);
            }
        }

        private void WriteIdentifier(Identifier id)
        {
            w.Write('{');
            Write("name", id.Name);
            if (!(id.Storage is MemoryStorage))
            {
                w.Write(',');
                Write("type", () => WriteType(id.DataType));
            }
            w.Write(',');
            Write("stg", () => WriteStorage(id.Storage));
            w.Write('}');
        }

        private void WriteType(DataType dt)
        {
            string primitiveName;
            if (JsonSymbols.PrimitiveNames.TryGetValue(dt, out primitiveName))
            {
                Write(primitiveName);
            }
            else
                throw new NotImplementedException(dt.ToString());
        }

        private void WriteStorage(Storage stg)
        {
            w.Write('{');
            RegisterStorage reg;
            MemoryStorage mem;
            FlagGroupStorage flg;
            if (stg.As(out reg))
            {
                Write("kind", "reg");
                w.Write(',');
                Write("name", reg.Name);
            }
            else if (stg.As(out mem))
            {
                Write("kind", "mem");
            }
            else if (stg.As(out flg))
            {
                Write("kind", "flg");
                w.Write(",");
                Write("grf", flg.FlagGroupBits);
                w.Write(",");
                Write("reg", flg.FlagRegister.Name);
            }
            else
                throw new NotImplementedException(string.Format("Unimplemented storage: {0}.", stg));
            w.Write('}');
        }

        private void WriteBlocks(Procedure proc)
        {
            WriteList(proc.ControlGraph.Blocks, b => WriteBlock(b, b == proc.ExitBlock));
        }

        private void WriteBlock(Block block, bool isExit)
        {
            w.Write('{');
            Write("name", block.Name);
            if (isExit)
            {
                w.Write(',');
                Write("exit", isExit);
            }
            if (block.Statements.Count > 0)
            {
                w.Write(',');
                var linaddr = block.Statements[0].LinearAddress;
                Write("linaddr", linaddr);
                w.Write(',');
                Write("stms", () => WriteList(block.Statements, stm => WriteStatement(stm, linaddr)));
            }
            if (block.Succ.Count > 0)
            {
                w.Write(',');
                Write("succ", () => WriteList(block.Succ.Select(s => s.Name), Write));
            }
            w.Write('}');
        }

        private void WriteStatement(Statement stm, ulong linAddrBlock)
        {
            w.Write("[");
            Write(stm.LinearAddress - linAddrBlock);
            w.Write(',');
            stm.Instruction.Accept(this);
            w.Write("]");
        }

        private void WriteList<T>(IEnumerable<T> items, Action<T> itemWriter)
        {
            w.Write('[');
            WriteListContents(items, itemWriter);
            w.Write(']');
        }

        private void WriteListContents<T>(IEnumerable<T> items, Action<T> itemWriter)
        {
            bool sep = false;
            foreach (var item in items)
            {
                if (sep)
                    w.Write(',');
                sep = true;
                itemWriter(item);
            }
        }

        private void Write(string key, ulong u64)
        {
            Write(key);
            w.Write(':');
            Write(u64);
        }

        private void Write(string key, bool value)
        {
            Write(key);
            w.Write(':');
            Write(value);
        }

        private void Write(bool value)
        {
            w.Write(value ? "true" : "false");
        }

        private void Write(ulong u64)
        {
            // Max integer representable with a IEEE 64-bit double
            const ulong MaxExactUInt = (1ul << 53) - 1ul;
            if (u64 > MaxExactUInt)
                Write(u64.ToString());
            else
                w.Write(u64);
        }

        private void Write(long i64)
        {
            // Max integer representable with a IEEE 64-bit double
            const long MaxExactInt = (1L << 53) - 1L;
            if (Math.Abs(i64) > MaxExactInt)
                Write(i64.ToString());
            else
                w.Write(i64);
        }

        private void Write(string s)
        {
            w.Write('"');
            foreach (char c in s)
            {
                switch (c)
                {
                case '\b': w.Write(@"\"); break;
                case '\t': w.Write(@"\"); break;
                case '\n': w.Write(@"\"); break;
                case '\f': w.Write(@"\"); break;
                case '\r': w.Write(@"\"); break;
                case '\"': w.Write(@"\"""); break;
                case '\\': w.Write(@"\\"); break;
                default:
                    if (0 <= c && c < ' ' || 0x7F <= c)
                    {
                        w.Write(@"\u{0:X4}", (int)c);
                    }
                    else
                    {
                        w.Write(c);
                    }
                    break;
                }
            }
            w.Write('"');
        }

        private void Write(string key, Action writer)
        {
            Write(key);
            w.Write(':');
            writer();
        }

        private void Write(string key, string value)
        {
            Write(key);
            w.Write(':');
            Write(value);
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
            WriteListContents(appl.Arguments, e => e.Accept(this));
        }

        public void VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public void VisitAssignment(Assignment ass)
        {
            Write("=");
            w.Write(',');
            ass.Dst.Accept(this);
            w.Write(',');
            ass.Src.Accept(this);
        }

        public void VisitBinaryExpression(BinaryExpression binExp)
        {
            w.Write('[');
            Write(JsonSymbols.OpNames[binExp.Operator]);
            w.Write(',');
            binExp.Left.Accept(this);
            w.Write(',');
            binExp.Right.Accept(this);
            w.Write(']');
        }

        public void VisitBranch(Branch branch)
        {
            Write("bra");
            w.Write(',');
            branch.Condition.Accept(this);
            w.Write(',');
            Write(branch.Target.Name);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            Write("call");
            w.Write(',');
            ci.Callee.Accept(this);
            w.Write(',');
            Write(ci.CallSite.SizeOfReturnAddressOnStack);
            w.Write(',');
            Write(ci.CallSite.FpuStackDepthBefore);
            w.Write(',');
            WriteList(ci.Uses.Select(u => u.Expression), e => e.Accept(this));
            WriteList(ci.Definitions.Select(d => d.Identifier), id => Write(id.Name));
        }

        private void WriteUseInstruction(UseInstruction ui)
        {
        }

        public void VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        public void VisitConditionOf(ConditionOf cof)
        {
            Write("cof");
            w.Write(',');
            cof.Expression.Accept(this);
        }

        public void VisitConstant(Constant c)
        {
            w.Write('[');
            Write(c.ToInt64());
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
            Write(id.Name);
        }

        public void VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public void VisitMemoryAccess(MemoryAccess access)
        {
            w.Write('[');
            Write("m");
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
            Write(pc.Procedure.Name);
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            Write("ret");
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
            Write("st");
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
            Write("test");
            w.Write(',');
            Write(tc.ConditionCode.ToString());
            w.Write(',');
            tc.Expression.Accept(this);
            w.Write(']');
        }

        public void VisitUnaryExpression(UnaryExpression unary)
        {
            w.Write('[');
            Write(JsonSymbols.OpNames[unary.Operator]);
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
