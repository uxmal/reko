#region License
/* 
 * Copyright (C) 1999-2017 John K�ll�n.
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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// Generates C# source code from Reko.Code that can be fed back into
    /// ProcedureBuilder.
    /// </summary>
    /// <remarks>
    /// Useful for generating unit tests.
    /// </remarks>
    public class MockGenerator : InstructionVisitor, IExpressionVisitor, IDataTypeVisitor<int>
    {
        private IndentingTextWriter writer;
        private Dictionary<Operator, string> mpopstr;

        public MockGenerator(TextWriter writer)
        {
            this.writer = new IndentingTextWriter(writer, false, 4);
            this.mpopstr = new Dictionary<Operator, string>();
            mpopstr.Add(Operator.IAdd, "IAdd");
            mpopstr.Add(Operator.And, "And");
            mpopstr.Add(Operator.Cand, "Cand");
            mpopstr.Add(Operator.Cor, "Cor");

            mpopstr.Add(Operator.Eq, "Eq");
            mpopstr.Add(Operator.Ge, "Ge");
            mpopstr.Add(Operator.Gt, "Gt");
            mpopstr.Add(Operator.Le, "Le");
            mpopstr.Add(Operator.Lt, "Lt");
            mpopstr.Add(Operator.IMul, "IMul");
            mpopstr.Add(Operator.SMul, "SMul");
            mpopstr.Add(Operator.UMul, "UMul");
            mpopstr.Add(Operator.Or, "Or");
            mpopstr.Add(Operator.ISub, "ISub");
            mpopstr.Add(Operator.USub, "USub");
            mpopstr.Add(Operator.Uge, "Uge");
            mpopstr.Add(Operator.Ugt, "Ugt");
            mpopstr.Add(Operator.Ule, "Ule");
            mpopstr.Add(Operator.Ult, "Ult");
            mpopstr.Add(Operator.Xor, "Xor");
        }

        private IEnumerable<Identifier> CollectLocalIdentifiers(Procedure proc)
        {
            SortedList<string, Identifier> identifiers = new SortedList<string, Identifier>();
            IdentifierCollector coll = new IdentifierCollector(identifiers);
            foreach (Block block in new DfsIterator<Block>(proc.ControlGraph).PreOrder(proc.EntryBlock))
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
            BlockGraph graph = proc.ControlGraph;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }


        void InstructionVisitor.VisitGotoInstruction(GotoInstruction def)
        {
            writer.Write("Jump");
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExpressionVisitor Members

        void IExpressionVisitor.VisitAddress(Address addr)
        {
            var addr16 = addr as Address16;
            if (addr16!= null)
                writer.Write("Address.Ptr16(0x{0:X}", addr16.ToUInt32());       //$REVIEW: need a ToUInt16
            if (addr.Selector.HasValue)
                writer.Write("Address.SegPtr(0x{0:X}, 0x{1:X}", addr.Selector, addr.Offset);
            var addr32 = addr as Address32;
            if (addr32 != null)
                writer.Write("Address.Ptr32(0x{0:X}", addr32.ToUInt32());
            var addr64 = addr as Address64;
            if (addr64 != null)
                writer.Write("Address.Ptr64(0x{0:X}", addr64.ToLinear());
            throw new NotSupportedException();
        }

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
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitBinaryExpression(BinaryExpression binExp)
        {
            string str;
            if (!mpopstr.TryGetValue(binExp.Operator, out str))
                throw new NotImplementedException(binExp.Operator.ToString());
            writer.Write(str);
            writer.Write("(");
            binExp.Left.Accept(this);
            writer.Write(", ");
            binExp.Right.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitConditionOf(ConditionOf cof)
        {
            throw new NotImplementedException();
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
            writer.Write(", {0})", d.BitPosition);
        }

        void IExpressionVisitor.VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitIdentifier(Identifier id)
        {
            writer.Write(id.Name);
        }

        void IExpressionVisitor.VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitOutArgument(OutArgument outArg)
        {
            writer.Write("Out(");
            outArg.DataType.Accept(this);
            writer.Write(", ");
            outArg.Expression.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitProcedureConstant(ProcedureConstant pc)
        {
            writer.Write("\"");
            writer.Write(pc.Procedure.Name);
            writer.Write("\"");
        }

        void IExpressionVisitor.VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitUnaryExpression(UnaryExpression unary)
        {
            if (unary.Operator == Operator.AddrOf)
                writer.Write("AddrOf(");
            else
                throw new NotImplementedException(unary.ToString());
            unary.Expression.Accept(this);
            writer.Write(")");
        }

        #endregion

        #region IDataTypeVisitor Members

        public int VisitArray(ArrayType at)
        {
            throw new NotImplementedException();
        }

        public int VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public int VisitCode(CodeType c)
        {
            writer.Write("new CodeType()", c.Size);
            return 0;
        }

        public int VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public int VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public int VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public int VisitPrimitive(PrimitiveType pt)
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
            return 0;
        }

        public int VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public int VisitPointer(Pointer ptr)
        {
            throw new NotImplementedException();
        }

        public int VisitReference(ReferenceTo ptr)
        {
            throw new NotImplementedException();
        }

        public int VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public int VisitStructure(StructureType str)
        {
            throw new NotImplementedException();
        }

        public int VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public int VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public int VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public int VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public int VisitVoidType(VoidType vt)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
