#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Graphs;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;

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
        private readonly IndentingTextWriter writer;
        private readonly Dictionary<OperatorType, string> mpopstr;
        private readonly string prefix;

        public MockGenerator(TextWriter writer, string prefix)
        {
            this.writer = new IndentingTextWriter(writer, false, 4);
            this.mpopstr = new Dictionary<OperatorType, string> {
                { OperatorType.IAdd, "IAdd" },
                { OperatorType.And, "And" },
                { OperatorType.Cand, "Cand" },
                { OperatorType.Cor, "Cor" },

                { OperatorType.Eq, "Eq" },
                { OperatorType.Ne, "Ne" },
                { OperatorType.Ge, "Ge" },
                { OperatorType.Gt, "Gt" },
                { OperatorType.Le, "Le" },
                { OperatorType.Lt, "Lt" },
                { OperatorType.IMul, "IMul" },
                { OperatorType.SMul, "SMul" },
                { OperatorType.UMul, "UMul" },
                { OperatorType.IMod, "Mod" },
                { OperatorType.SMod, "SMod" },
                { OperatorType.UMod, "UMod" },
                { OperatorType.Or, "Or" },
                { OperatorType.ISub, "ISub" },
                { OperatorType.USub, "USub" },
                { OperatorType.SDiv, "SDiv" },
                { OperatorType.UDiv, "UDiv" },

                { OperatorType.Shl, "Shl" },
                { OperatorType.Shr, "Shr" },
                { OperatorType.Sar, "Sar" },
                { OperatorType.Uge, "Uge" },
                { OperatorType.Ugt, "Ugt" },
                { OperatorType.Ule, "Ule" },
                { OperatorType.Ult, "Ult" },
                { OperatorType.Xor, "Xor" },

                { OperatorType.FAdd, "FAdd" },
                { OperatorType.FSub, "FSub" },
                { OperatorType.FMul, "FMul" },
                { OperatorType.FDiv, "FDiv" },
                { OperatorType.Feq, "FEq" },
                { OperatorType.Fne, "FNe" },
                { OperatorType.Flt, "FLt" },
                { OperatorType.Fle, "FLe" },
                { OperatorType.Fge, "FGe" },
                { OperatorType.Fgt, "FGt" },

                { OperatorType.AddrOf, "AddrOf" },
                { OperatorType.Comp, "Comp" },
                { OperatorType.Neg, "Neg" },
                { OperatorType.Not, "Not" }

            };
            this.prefix = prefix;
        }

        public static void DumpMethod(Procedure proc)
        {
            var sw = new StringWriter();
            sw.WriteLine("// {0} //////////", proc.Name);
            var mg = new MockGenerator(sw, "m.");
            mg.WriteMethod(proc);
            Debug.Print(sw.ToString());
        }

        public static void DumpClass(Procedure proc)
        {
            var sw = new StringWriter();
            var mg = new MockGenerator(sw, "");
            mg.WriteClass(proc);
            Debug.Print(sw.ToString());
        }

        private IEnumerable<Identifier> CollectLocalIdentifiers(Procedure proc)
        {
            var identifiers = new Dictionary<string, Identifier>();
            var coll = new IdentifierCollector(identifiers);
            foreach (Block block in new DfsIterator<Block>(proc.ControlGraph).PreOrder(proc.EntryBlock))
            {
                foreach (Statement stm in block.Statements)
                {
                    stm.Instruction.Accept(coll);
                }
            }
            return identifiers.Values;
        }


        public void WriteClass(Procedure proc)
        {
            WriteClassPrologue(proc);
            WriteIdentifierDeclarations(proc, CollectLocalIdentifiers(proc));
            WriteCode(proc);
            WriteClassEpilogue();
        }

        public void WriteMethod(Procedure proc)
        {
            WriteMethodPrologue(proc);
            WriteIdentifierDeclarations(proc, CollectLocalIdentifiers(proc));
            WriteCode(proc);
            WriteMethodEpilogue();
        }

        private void WriteExpressions(IEnumerable<Expression> exprs)
        {
            var sep = "";
            foreach (var e in exprs)
            {
                writer.Write(sep);
                sep = ", ";
                e.Accept(this);
            }
        }

        private void WriteIdentifierDeclarations(Procedure proc, IEnumerable<Identifier> identifiers)
        {
            foreach (Identifier id in identifiers)
            {
                if (id.Storage is MemoryStorage)
                    continue;
                writer.Write("Identifier {0} = ", id.Name);
                if (id == proc.Frame.FramePointer)
                {
                    writer.Write("m.Frame.FramePointer");
                }
                else
                {
                    Method("Local");
                    id.DataType.Accept(this);
                    writer.Write(", \"{0}\")", id.Name);
                }
                writer.WriteLine(";");
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

        private void WriteClassEpilogue()
        {
            writer.Outdent();
            writer.WriteLine("}");
            writer.WriteLine();
        }

        private void WriteMethodEpilogue()
        {
            writer.Outdent();
            writer.WriteLine("});");
            writer.WriteLine();
        }

        private void WriteClassPrologue(Procedure proc)
        {
            string procName = (proc.Name != "ProcedureMock")
                ? proc.Name
                : "MockProcedure";
            writer.WriteLine("public class {0} : ProcedureMock", procName);
            writer.WriteLine("{");
            writer.Indent();
        }

        private void WriteMethodPrologue(Procedure proc)
        {
            writer.WriteLine("RunTest(m =>");
            writer.WriteLine("{");
            writer.Indent();
        }

        private void EmitLabel(Block block)
        {
            Method("Label");
            writer.WriteLine("\"{0}\");", block.DisplayName);
        }

        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            Method("Assign");
            writer.Write("{0}, ", a.Dst.Name);
            a.Src.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            Method("BranchIf");
            b.Condition.Accept(this);
            writer.Write(", \"");
            writer.Write(b.Target.DisplayName);
            writer.WriteLine("\");");
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            Method("Call");
            ci.Callee.Accept(this);
            writer.Write(", {0}", ci.CallSite.SizeOfReturnAddressOnStack);
            if (ci.Uses.Count > 0)
            {
                writer.WriteLine(",");
                writer.Indent();
                writer.Write("new [] {");
                WriteExpressions(ci.Uses.Select(u => u.Expression));
                writer.Write("}");
                writer.Outdent();
            }
            if (ci.Definitions.Count > 0)
            {
                writer.WriteLine(",");
                writer.Indent();
                writer.Write("new [] {");
                WriteExpressions(ci.Definitions.Select(d => d.Expression));
                writer.Write("}");
                writer.Outdent();
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitComment(CodeComment comment)
        {
            Method("Comment");
            QuoteString(comment.Text);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            Method("Declare");
            writer.Write(decl.Identifier.Name);
            if (decl.Expression != null)
            {
                writer.Write(", ");
                decl.Expression.Accept(this);
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            Method("Def");
            def.Identifier.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction def)
        {
            writer.Write("Jump");
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            Method("Phi");
            phi.Dst.Accept(this);
            foreach (var arg in phi.Src.Arguments)
            {
                writer.Write(", ");
                writer.Write("(");
                arg.Value.Accept(this);
                writer.Write(",");
                writer.Write("\"{0}\"", arg.Block.DisplayName);
                writer.Write(")");
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            Method("Return");
            if (ret.Expression != null)
            {
                ret.Expression.Accept(this);
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            Method("SideEffect");
            side.Expression.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            if (store.Dst is MemoryAccess access)
            {
                Method("MStore");
                access.EffectiveAddress.Accept(this);
            }
            else if (store.Dst is Identifier id)
            {
                Method("Store");
                id.Accept(this);
            }
            else
                throw new NotSupportedException(store.ToString());
            writer.Write(", ");
            store.Src.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            Method("Switch");
            si.Expression.Accept(this);
            if (si.Targets.Length > 0)
            {
                writer.Write(", ");
                writer.Write(string.Join(",", si.Targets.Select(t => $"\"{t}\"")));
            }
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            Method("Use");
            u.Expression.Accept(this);
            writer.WriteLine(");");
        }

        #endregion

        #region IExpressionVisitor Members

        void IExpressionVisitor.VisitAddress(Address addr)
        {
            if (addr is Address16 addr16)
            {
                writer.Write("Address.Ptr16(0x{0:X})", addr16.ToUInt16());
                return;
            }
            if (addr.Selector.HasValue)
            {
                writer.Write("Address.SegPtr(0x{0:X}, 0x{1:X})", addr.Selector, addr.Offset);
                return;
            }
            if (addr is Address32 addr32)
            {
                writer.Write("Address.Ptr32(0x{0:X})", addr32.ToUInt32());
                return;
            }
            if (addr is Address64 addr64)
            {
                writer.Write("Address.Ptr64(0x{0:X})", addr64.ToLinear());
                return;
            }
            throw new NotSupportedException();
        }

        void IExpressionVisitor.VisitApplication(Application appl)
        {
            Method("Fn");
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
            if (!mpopstr.TryGetValue(binExp.Operator.Type, out string? str))
                throw new NotImplementedException(binExp.Operator.ToString());
            Method(str);
            binExp.Left.Accept(this);
            writer.Write(", ");
            binExp.Right.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitCast(Cast cast)
        {
            Method("Cast");
            cast.DataType.Accept(this);
            writer.Write(", ");
            cast.Expression.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitConversion(Conversion conversion)
        {
            Method("Convert");
            conversion.Expression.Accept(this);
            writer.Write(", ");
            conversion.SourceDataType.Accept(this);
            writer.Write(", ");
            conversion.DataType.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitConditionOf(ConditionOf cof)
        {
            Method("Cond");
            cof.Expression.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitConstant(Constant c)
        {
            if (c.DataType == PrimitiveType.Word32)
            {
                Method("Word32");
            }
            else if (c.DataType==PrimitiveType.Int32)
            {
                Method("Int32");
                var v = c.ToInt32();
                if (v < 0)
                    writer.Write("-{0})", -v);
                else
                    writer.Write("{0})", v);
                return;
            }

            else if (c.DataType == PrimitiveType.Word16)
            {
                Method("Word16");
            }
            else if (c.DataType == PrimitiveType.Byte)
            {
                Method("Byte");
            }
            else
            {
                writer.Write("Constant.Create(");
                c.DataType.Accept(this);
                writer.Write(", ");
            }
            writer.Write("0x{0:X})", c.ToUInt64());
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
            if (access.EffectiveAddress is SegmentedPointer segptr)
            {
                Method("SegMem");
                access.DataType.Accept(this);
                writer.Write(", ");
                segptr.BasePointer.Accept(this);
                writer.Write(", ");
                segptr.Offset.Accept(this);
                writer.Write(")");
            }
            else
            {
                Method("Mem");
                access.DataType.Accept(this);
                writer.Write(", ");
                access.EffectiveAddress.Accept(this);
                writer.Write(")");
            }
        }

        void IExpressionVisitor.VisitMkSequence(MkSequence seq)
        {
            Method("Seq");
            WriteExpressions(seq.Expressions);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitOutArgument(OutArgument outArg)
        {
            Method("Out");
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

        void IExpressionVisitor.VisitSegmentedAddress(SegmentedPointer address)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitSlice(Slice slice)
        {
            Method("Slice");
            slice.DataType.Accept(this);
            writer.Write(", ");
            slice.Expression.Accept(this);
            writer.Write(", {0})", slice.Offset);
        }

        void IExpressionVisitor.VisitStringConstant(StringConstant str)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitTestCondition(TestCondition tc)
        {
            Method("Test");
            writer.Write($"ConditionCode.{tc.ConditionCode}");
            writer.Write(", ");
            tc.Expression.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitUnaryExpression(UnaryExpression unary)
        {
            if (!mpopstr.TryGetValue(unary.Operator.Type, out var methodName))
                throw new NotImplementedException(unary.ToString());
            Method(methodName);
            unary.Expression.Accept(this);
            writer.Write(")");
        }

        #endregion

        #region IDataTypeVisitor Members

        public int VisitArray(ArrayType at)
        {
            writer.Write("new ArrayType(");
            at.ElementType.Accept(this);
            writer.Write(", {0})", at.Length);
            return 0;
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
            void writeArg(Identifier arg)
            {
                writer.Write("new Identifier(\"{0}\", ", arg.Name);
                arg.DataType.Accept(this);
                //$TODO: storage
                writer.Write(")");
            }

            writer.Write("new FunctionType(");
            if (ft.ReturnValue == null)
            {
                writer.Write("null");
            }
            else
            {
                writeArg(ft.ReturnValue);
            }
            WriteList(ft.Parameters!, ", ", writeArg);
            writer.Write(")");
            return 0;
        }

        public int VisitPrimitive(PrimitiveType pt)
        {
            writer.Write(StringFromPrimitive(pt));
            return 0;
        }

        private string StringFromPrimitive(PrimitiveType pt)
        {
            var sb = new StringBuilder();
            sb.Append("PrimitiveType.");
            if (pt == PrimitiveType.Word32)
                sb.Append("Word32");
            else if (pt == PrimitiveType.Int32)
                sb.Append("Int32");
            else if (pt == PrimitiveType.Word16)
                sb.Append("Word16");
            else if (pt == PrimitiveType.Byte)
                sb.Append("Byte");
            else if (pt == PrimitiveType.SegmentSelector)
                sb.Append("SegmentSelector");
            else if (pt == PrimitiveType.Bool)
                sb.Append("Bool");
            else if (pt == PrimitiveType.Ptr32)
                sb.Append("Ptr32");
            else
            {
                if (pt.Domain == Domain.SignedInt)
                {
                    sb.Append($"Int{pt.BitSize}");
                }
                else
                {
                    sb.Append("Create(");
                    sb.Append(string.Join("|",
                        pt.Domain.ToString()
                            .Split(',').Select(s => $"Domain.{s.Trim()}")));
                    sb.Append($", {pt.BitSize}");
                    sb.Append(")");
                }
            }
            return sb.ToString();
        }

        public int VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public int VisitPointer(Pointer ptr)
        {
            writer.Write("new Pointer(");
            ptr.Pointee.Accept(this);
            writer.Write(", {0})", ptr.BitSize);
            return 0;
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
            writer.Write("new StructureType({0})", str.Name ?? "<not-set>");
            return 0;
        }

        public int VisitTypeReference(TypeReference typeref)
        {
            writer.Write("new TypeReference(");
            typeref.Referent.Accept(this);
            writer.Write(")");
            return 0;
        }

        public int VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public int VisitUnion(UnionType ut)
        {
            writer.Write("new UnionType({0})", ut.Name ?? "<not-set>");
            return 0;
        }

        public int VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public int VisitVoidType(VoidType vt)
        {
            writer.Write("VoidType.Instance");
            return 0;
        }
        #endregion

        private void QuoteString(string str)
        {
            var sb = new StringBuilder();
            sb.Append("\"");
            foreach (var ch in str)
            {
                switch (ch)
                {
                case '\r': sb.Append(@"\r"); break;
                case '\n': sb.Append(@"\n"); break;
                case '\t': sb.Append(@"\t"); break;
                case '\\': sb.Append(@"\\"); break;
                default:
                    sb.Append(ch);
                    break;
                }
            }
            sb.Append("\"");
            writer.Write(sb.ToString());
        }

        private void Method(string methodName)
        {
            writer.Write("{0}{1}(", prefix, methodName);
        }

        private void WriteList<T>(IEnumerable<T> items, string separator, Action<T> w)
        {
            var s = "";
            foreach (var item in items)
            {
                writer.Write(s);
                s = separator;
                w(item);
            }
        }
    }
}
