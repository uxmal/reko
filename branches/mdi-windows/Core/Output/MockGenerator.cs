/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Output
{
    public class MockGenerator : InstructionVisitor, IExpressionVisitor, IDataTypeVisitor
    {
        private IndentingTextWriter writer;
        private Dictionary<Operator, string> mpopstr;

        public MockGenerator(TextWriter writer)
        {
            this.writer = new IndentingTextWriter(writer, false, 4);
            this.mpopstr = new Dictionary<Operator, string>();
            mpopstr.Add(Operator.add, "Add");
            mpopstr.Add(Operator.and, "And");
            mpopstr.Add(Operator.cand, "Cand");
            mpopstr.Add(Operator.cor, "Cor");

            mpopstr.Add(Operator.eq, "Eq");
            mpopstr.Add(Operator.ge, "Ge");
            mpopstr.Add(Operator.gt, "Gt");
            mpopstr.Add(Operator.le, "Le");
            mpopstr.Add(Operator.lt, "Lt");
            mpopstr.Add(Operator.mul, "Mul");
            mpopstr.Add(Operator.muls, "Muls");
            mpopstr.Add(Operator.mulu, "Mulu");
            mpopstr.Add(Operator.or, "Or");
            mpopstr.Add(Operator.sub, "Sub");
            mpopstr.Add(Operator.uge, "Uge");
            mpopstr.Add(Operator.ugt, "Ugt");
            mpopstr.Add(Operator.ule, "Ule");
            mpopstr.Add(Operator.ult, "Ult");
            mpopstr.Add(Operator.xor, "Xor");

        }

        private IEnumerable<Identifier> CollectLocalIdentifiers(Procedure proc)
        {
            SortedList<string, Identifier> identifiers = new SortedList<string, Identifier>();
            IdentifierCollector coll = new IdentifierCollector(identifiers);
            DirectedGraph<Block> graph = new BlockGraph(proc.RpoBlocks);
            foreach (Block block in new DfsIterator<Block>(graph).PreOrder(proc.EntryBlock))
            {
                foreach (Statement stm in block.Statements)
                {
                    stm.Instruction.Accept(coll);
                }
            }
            return identifiers.Values;
        }

        private class IdentifierCollector : InstructionVisitorBase
        {
            private SortedList<string, Identifier> identifiers;

            public IdentifierCollector(SortedList<string,Identifier> ids)
            {
                this.identifiers = ids;
            }

            public override void VisitIdentifier(Identifier id)
            {
                identifiers[id.Name] = id;
            }
        }


        public void Write(Procedure proc)
        {
            WritePrologue(proc);
            WriteIdentifierDeclarations(CollectLocalIdentifiers(proc));
            WriteCode(proc);
            WriteEpilogue();
        }

        private void WriteIdentifierDeclarations(IEnumerable<Identifier> identifiers)
        {
            foreach (Identifier id in identifiers)
            {
                if (id is MemoryIdentifier)
                    continue;
                writer.Write("Identifier {0} = Local(", id.Name);
                id.DataType.Accept(this);
                writer.WriteLine(", \"{0}\");", id.Name);
            }
        }

        private void WriteCode(Procedure proc)
        {
            BlockGraph graph = new BlockGraph(proc.RpoBlocks);

            DfsIterator<Block> iterator = new DfsIterator<Block>(graph);
            foreach (Block block in new DfsIterator<Block>(graph).PreOrder(proc.EntryBlock))
            {
                if (ShouldIgnoreBlock(proc, block))
                    continue;

                writer.WriteLine();
                EmitLabel(block);
                foreach (Statement stm in block.Statements)
                {
                    stm.Instruction.Accept(this);
                }
            }
        }

        private static bool ShouldIgnoreBlock(Procedure proc, Block block)
        {
            return (block == proc.EntryBlock || block == proc.ExitBlock) && block.Statements.Count == 0;
        }

        private void WriteEpilogue()
        {
            writer.Outdent();
            writer.WriteLine("}");
            writer.WriteLine();
        }

        private void WritePrologue(Procedure proc)
        {
            string procName = (proc.Name != "ProcedureMock")
                ? proc.Name
                : "MockProcedure";
            writer.WriteLine("public class {0} : ProcedureMock", procName);
            writer.WriteLine("{");
            writer.Indent();
        }

        private void EmitLabel(Block block)
        {
            writer.WriteLine("Label(\"{0}\");", block.Name);
        }

        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            writer.Write("Assign({0}, ", a.Dst.Name);
            a.Src.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            writer.Write("BranchIf(");
            b.Condition.Accept(this);
            writer.Write(", \"");
            writer.Write(b.Target.Name);
            writer.WriteLine("\");");
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            writer.Write("Declare(");
            writer.Write(decl.Identifier.Name);
            writer.Write(", ");
            decl.Expression.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            writer.Write("Return(");
            if (ret.Expression != null)
            {
                ret.Expression.Accept(this);
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            writer.Write("SideEffect(");
            side.Expression.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            writer.Write("Store(");
            MemoryAccess access = store.Dst as MemoryAccess;
            if (access != null)
            {
                access.EffectiveAddress.Accept(this);
            }
            else
                throw new NotSupportedException(store.ToString());
            writer.Write(", ");
            store.Src.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion



        #region IExpressionVisitor Members

        void IExpressionVisitor.VisitApplication(Application appl)
        {
            writer.Write("Fn(");
            appl.Procedure.Accept(this);
            foreach (Expression arg in appl.Arguments)
            {
                writer.Write(", ");
                arg.Accept(this);
            }
            writer.Write(")");
        }

        void IExpressionVisitor.VisitArrayAccess(ArrayAccess acc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitBinaryExpression(BinaryExpression binExp)
        {
            string str;
            if (!mpopstr.TryGetValue(binExp.op, out str))
                throw new NotImplementedException(binExp.op.ToString());
            writer.Write(str);
            writer.Write("(");
            binExp.Left.Accept(this);
            writer.Write(", ");
            binExp.Right.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitCast(Cast cast)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitConditionOf(ConditionOf cof)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitConstant(Constant c)
        {
            if (c.DataType == PrimitiveType.Word32)
            {
                writer.Write("Word32(");

            }
            else if (c.DataType==PrimitiveType.Int32)
            {
                writer.Write("new Constant(Primitive.Int32, ");
            } else if (c.DataType == PrimitiveType.Word16)
            {
                writer.Write("Word16(");
            }
            else
            {
                writer.Write("new Constant(");
                c.DataType.Accept(this);
                writer.Write(", ");
            }
            writer.Write("0x{0:X})", c.ToUInt64());
        }

        void IExpressionVisitor.VisitDepositBits(DepositBits d)
        {
            writer.Write("Dpb(");
            d.Source.Accept(this);
            writer.Write(", ");
            d.InsertedBits.Accept(this);
            writer.Write(", {0}, {1})", d.BitPosition, d.BitCount);
        }

        void IExpressionVisitor.VisitDereference(Dereference deref)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitFieldAccess(FieldAccess acc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitIdentifier(Identifier id)
        {
            writer.Write(id.Name);
        }

        void IExpressionVisitor.VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitMemoryAccess(MemoryAccess access)
        {
            writer.Write("Load(");
            access.DataType.Accept(this);
            writer.Write(", ");
            access.EffectiveAddress.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitMkSequence(MkSequence seq)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitPhiFunction(PhiFunction phi)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitPointerAddition(PointerAddition pa)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitProcedureConstant(ProcedureConstant pc)
        {
            writer.Write("\"");
            writer.Write(pc.Procedure.Name);
            writer.Write("\"");
        }

        void IExpressionVisitor.VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitSlice(Slice slice)
        {
            writer.Write("Slice(");
            slice.DataType.Accept(this);
            writer.Write(", ");
            slice.Expression.Accept(this);
            writer.Write(", {0})", slice.Offset);
        }

        void IExpressionVisitor.VisitTestCondition(TestCondition tc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IExpressionVisitor.VisitUnaryExpression(UnaryExpression unary)
        {
            if (unary.op == Operator.addrOf)
                writer.Write("AddrOf(");
            else
                throw new NotImplementedException(unary.ToString());
            unary.Expression.Accept(this);
            writer.Write(")");
        }

        #endregion

        #region IDataTypeVisitor Members

        public void VisitArray(ArrayType at)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitFunctionType(FunctionType ft)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitPrimitive(PrimitiveType pt)
        {
            writer.Write("PrimitiveType.");
            if (pt == PrimitiveType.Word32)
                writer.Write("Word32");
            else if (pt == PrimitiveType.Int32)
                writer.Write("Int32");
            else if (pt == PrimitiveType.Word16)
                writer.Write("Word16");
            else if (pt == PrimitiveType.Byte)
                writer.Write("Byte");
            else if (pt == PrimitiveType.SegmentSelector)
                writer.Write("SegmentSelector");
            else if (pt == PrimitiveType.Bool)
                writer.Write("Bool");
            else
                throw new NotSupportedException(pt.ToString());
        }

        public void VisitMemberPointer(MemberPointer memptr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitPointer(Pointer ptr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitStructure(StructureType str)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitTypeVar(TypeVariable tv)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitUnion(UnionType ut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void VisitUnknownType(UnknownType ut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
