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
using Microsoft.Win32.SafeHandles;

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

        /// <summary>
        /// Constructs a mock generator.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        /// <param name="prefix">Prefix to use before prefix names.
        /// </param>
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
                { OperatorType.FNeg, "FNeg" },
                { OperatorType.Not, "Not" }

            };
            this.prefix = prefix;
        }

        /// <summary>
        /// Writes a procedure.
        /// </summary>
        /// <param name="proc">Procedure to write.</param>
        public static void DumpMethod(Procedure proc)
        {
            var sw = new StringWriter();
            sw.WriteLine("// {0} //////////", proc.Name);
            var mg = new MockGenerator(sw, "m.");
        }

        /// <summary>
        /// Writes a class method.
        /// </summary>
        /// <param name="proc">Class method.</param>
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

        /// <summary>
        /// Writes a class enclosing the given <paramref name="proc"/>.
        /// </summary>
        /// <param name="proc">Procedure to emit mock code for.
        /// </param>
        public void WriteClass(Procedure proc)
        {
            WriteClassPrologue(proc);
            WriteIdentifierDeclarations(proc, CollectLocalIdentifiers(proc));
            WriteCode(proc);
            WriteClassEpilogue();
        }

        /// <summary>
        /// Writes mock generation code for the given <paramref name="proc"/>.
        /// </summary>
        /// <param name="proc">Procedure to generate mock code for.</param>
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
            var idGroups = identifiers
                .GroupBy(id => id.Storage)
                .OrderBy(g => g.Key, StorageCollator.Instance)
                .ToArray();
            var stgVarNames = new Dictionary<Storage, string>();
            foreach (var group in idGroups)
            {
                var stg = group.Key;
                if (stg is MemoryStorage || stg is TemporaryStorage)
                    continue;
                if (stg == proc.Frame.FramePointer.Storage)
                    continue;
                if (stg == proc.Frame.Continuation.Storage)
                    continue;
                var stgName = WriteStorageDeclaration(group.Key, stgVarNames, this.writer);
            }
            foreach (var group in idGroups)
            {
                if (group.Key is MemoryStorage)
                    continue;
                foreach (var id in group)
                {
                    writer.Write("Identifier {0} = ", id.Name);
                    if (id == proc.Frame.FramePointer)
                    {
                        writer.Write("m.Frame.FramePointer");
                    }
                    else if (id.Storage == proc.Frame.Continuation.Storage)
                    {
                        writer.Write("m.Frame.Continuation");
                    }
                    else if (id.Storage is RegisterStorage reg)
                    {
                        writer.Write($"m.Frame.EnsureRegister({stgVarNames[reg]})");
                    }
                    else if (id.Storage is SequenceStorage seq)
                    {
                        writer.Write($"m.Frame.EnsureSequence(");
                        id.DataType.Accept(this);
                        writer.Write($", {stgVarNames[seq]})");
                    }
                    else if (id.Storage is FlagGroupStorage grf)
                    {
                        writer.Write($"m.Frame.EnsureFlagGroup({stgVarNames[grf]})");
                    }
                    else if (id.Storage is FpuStackStorage fpu)
                    {
                        writer.Write($"m.Frame.EnsureFpuStackVariable({fpu.FpuStackOffset}");
                        id.DataType.Accept(this);
                        writer.Write(")");
                    }
                    else if (id.Storage is StackStorage stk)
                    {
                        writer.Write("m.Frame.EnsureStackVariable(");
                        writer.Write($"{stk.StackOffset}, ");
                        id.DataType.Accept(this);
                        writer.Write($", \"{id.Name}\")");
                    }
                    else if (id.Storage is TemporaryStorage tmp)
                    {
                        writer.Write("m.Frame.CreateTemporary(");
                        writer.Write($"\"{tmp.Name}\", {tmp.Number}, ");
                        id.DataType.Accept(this);
                        writer.Write(")");
                    }
                    else
                    {
                        writer.WriteLine($"****** Storage {id.Storage.GetType().Name} for {id.Name} not implemented yet.");
                    }
                    writer.WriteLine(";");
                }
            }
        }

        private string WriteStorageDeclaration(
            Storage stg,
            Dictionary<Storage, string> stgVarNames,
            IndentingTextWriter writer)
        {
            if (stgVarNames.TryGetValue(stg, out var stgName))
                return stgName;
            switch (stg)
            {
            case RegisterStorage reg:
                stgName = $"reg_{reg.Name}";
                writer.Write("RegisterStorage {0} = new RegisterStorage(", stgName);
                writer.Write($"\"{reg.Name}\", {reg.Number}, {reg.BitAddress}, ");
                reg.DataType.Accept(this);
                writer.WriteLine(");");
                break;
            case StackStorage stk:
                stgName = "";
                break;
            case SequenceStorage seq:
                var stgNames = new List<string>();
                for (int i = 0; i < seq.Elements.Length; ++i)
                {
                    var stgElem = seq.Elements[i];
                    if (!stgVarNames.TryGetValue(stgElem, out var subName))
                    {
                        subName = WriteStorageDeclaration(stgElem, stgVarNames, writer);
                    }
                    stgNames.Add(subName);
                }
                stgName = $"seq_{string.Join('_', seq.Elements.Select(e => e.Name))}";
                writer.Write($"SequenceStorage {stgName} = new SequenceStorage(");
                seq.DataType.Accept(this);
                foreach (var s in stgNames)
                {
                    writer.Write(", {0}", s);
                }
                writer.WriteLine(");");
                break;
            case FlagGroupStorage grf:
                if (!stgVarNames.TryGetValue(grf.FlagRegister, out var flagRegName))
                {
                    flagRegName = WriteStorageDeclaration(grf.FlagRegister, stgVarNames, writer);
                }
                stgName = $"grf_{grf.Name}";
                writer.WriteLine($"FlagGroupStorage {stgName} = new FlagGroupStorage({flagRegName}, 0x{grf.FlagGroupBits:X}, \"{grf.Name}\");");
                break;
            case FpuStackStorage fpu:
                stgName = $"fpu_{fpu.Name}";
                writer.Write($"FpuStackStorage {stgName} = new FpuStackStorage({fpu.FpuStackOffset}, ");
                fpu.DataType.Accept(this);
                writer.WriteLine(");");
                break;
            default:
                writer.WriteLine($"***** Not implemented {stg.GetType().Name}");
                stgName = stg.Name;
                break;
            }
            stgVarNames.Add(stg, stgName);
            return stgName;
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

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            Method("Def");
            def.Identifier.Accept(this);
            writer.WriteLine(");");
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction g)
        {
            writer.Write("Jump");
            g.Target.Accept(this);
            writer.WriteLine(");");
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
            if (ret.Expression is not null)
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
            else if (store.Dst is Identifier || store.Dst is ArrayAccess)
            {
                Method("Store");
                store.Dst.Accept(this);
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
            int bitsize = addr.DataType.BitSize;
            if (bitsize == 16)
            {
                writer.Write("Address.Ptr16(0x{0:X})", addr.ToUInt16());
                return;
            }
            if (addr.Selector.HasValue)
            {
                writer.Write("Address.SegPtr(0x{0:X}, 0x{1:X})", addr.Selector, addr.Offset);
                return;
            }
            if (bitsize == 32)
            {
                writer.Write("Address.Ptr32(0x{0:X})", addr.ToUInt32());
                return;
            }
            if (bitsize == 64)
            {
                writer.Write("Address.Ptr64(0x{0:X})", addr.ToLinear());
                return;
            }
            writer.Write("Address.Ptr{0}(0x{0:X})", addr.DataType.BitSize, addr.ToLinear());
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
            Method("ARef");
            acc.Array.Accept(this);
            writer.Write(", ");
            acc.Index.Accept(this);
            writer.Write(")");
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
            Method("Conditional");
            cond.Condition.Accept(this);
            writer.Write(", ");
            cond.ThenExp.Accept(this);
            writer.Write(", ");
            cond.FalseExp.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitConditionOf(ConditionOf cof)
        {
            Method("Cond");
            cof.Expression.Accept(this);
            writer.Write(")");
        }

        void IExpressionVisitor.VisitConstant(Constant c)
        {
            if (!c.IsValid)
            {
                writer.Write("InvalidConstant.Create");

                c.DataType.Accept(this);
                writer.Write(")");
                return;
            }
            if (c.DataType.IsReal)
            {
                if (c.DataType.BitSize == 32)
                {
                    Method("Real32");
                    writer.Write("{0}F)", c.ToReal64());
                }
                else if (c.DataType.BitSize == 64)
                {
                    Method("Real64");
                    writer.Write("{0})", c.ToReal64());
                }
                return;
            }
            else if (c.DataType == PrimitiveType.Byte)
            {
                Method("Byte");
                writer.Write("0x{0:X})", c.ToByte());
                return;
            }
            if (c.DataType == PrimitiveType.Word32)
            {
                Method("Word32");
            }
            else if (c.DataType == PrimitiveType.Word64)
            {
                Method("Word64");
            }
            else if (c.DataType == PrimitiveType.Word16)
            {
                Method("Word16");
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
            else
            {
                writer.Write("Constant.Create(");
                c.DataType.Accept(this);
                writer.Write(", ");
            }
            try
            {
                writer.Write("0x{0:X})", c.ToUInt64());
            }
             catch (Exception ex)
            {
                throw new ApplicationException($"Failed to render: {c} ({c.DataType}).", ex);
            }
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
            Method("SegPtr");
            address.BasePointer.Accept(this);
            writer.Write(", ");
            address.Offset.Accept(this);
            writer.Write(")");
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
            writer.Write("new StringConstant(");
            str.DataType.Accept(this);
            writer.Write(", ");
            QuoteString(str.Literal);
            writer.Write(")");
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

        /// <inheritdoc />
        public int VisitArray(ArrayType at)
        {
            writer.Write("new ArrayType(");
            at.ElementType.Accept(this);
            writer.Write(", {0})", at.Length);
            return 0;
        }

        /// <inheritdoc />
        public int VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int VisitCode(CodeType c)
        {
            writer.Write("new CodeType()", c.Size);
            return 0;
        }

        /// <inheritdoc />
        public int VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
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
            if (ft.ReturnValue is null)
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

        /// <inheritdoc />
        public int VisitPrimitive(PrimitiveType pt)
        {
            writer.Write(StringFromPrimitive(pt));
            return 0;
        }

        private string StringFromPrimitive(PrimitiveType pt)
        {
            var sb = new StringBuilder();
            sb.Append("PrimitiveType.");
            if (pt == PrimitiveType.Byte)
                sb.Append("Byte");
            else if (pt.IsWord)
                sb.Append($"Word{pt.BitSize}");
            else if (pt.Domain == Domain.SignedInt)
                sb.Append($"Int{pt.BitSize}");
            else if (pt.Domain == Domain.UnsignedInt)
                sb.Append($"UInt{pt.BitSize}");
            else if (pt == PrimitiveType.SegmentSelector)
                sb.Append("SegmentSelector");
            else if (pt == PrimitiveType.Bool)
                sb.Append("Bool");
            else if (pt == PrimitiveType.Ptr32)
                sb.Append("Ptr32");
            else
            {
                sb.Append("Create(");
                sb.Append(string.Join("|",
                    pt.Domain.ToString()
                        .Split(',').Select(s => $"Domain.{s.Trim()}")));
                sb.Append($", {pt.BitSize}");
                sb.Append(")");
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public int VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int VisitPointer(Pointer ptr)
        {
            writer.Write("new Pointer(");
            ptr.Pointee.Accept(this);
            writer.Write(", {0})", ptr.BitSize);
            return 0;
        }

        /// <inheritdoc />
        public int VisitReference(ReferenceTo ptr)
        {
            writer.Write("new ReferenceTo(");
            ptr.Referent.Accept(this);
            writer.Write(", {0})", ptr.BitSize);
            return 0;
        }

        /// <inheritdoc />
        public int VisitString(StringType str)
        {
            writer.Write("new StringType(");
            str.ElementType.Accept(this);
            writer.Write(", ");
            if (str.LengthPrefixType is null)
                writer.Write("null");
            else
                str.LengthPrefixType.Accept(this);
            writer.Write(", ");
            writer.Write(str.PrefixOffset.ToString());
            writer.Write(")");
            return 0;
        }

        /// <inheritdoc />
        public int VisitStructure(StructureType str)
        {
            writer.Write("new StructureType({0})", str.Name ?? "<not-set>");
            return 0;
        }

        /// <inheritdoc />
        public int VisitTypeReference(TypeReference typeref)
        {
            writer.Write("new TypeReference(");
            typeref.Referent.Accept(this);
            writer.Write(")");
            return 0;
        }

        /// <inheritdoc />
        public int VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int VisitUnion(UnionType ut)
        {
            writer.Write("new UnionType({0})", ut.Name ?? "<not-set>");
            return 0;
        }

        /// <inheritdoc />
        public int VisitUnknownType(UnknownType ut)
        {
            writer.Write("new UnknownType(");
            if (ut.Size != 0)
                writer.Write(ut.Size.ToString());
            writer.Write(")");
            return 0;
        }

        /// <inheritdoc />
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

        private class StorageCollator : IComparer<Storage>
        {

            private static Dictionary<Type, int> precedences =
            new() {
                { typeof(RegisterStorage), 1 },
                { typeof(SequenceStorage), 2 },
                { typeof(StackStorage), 3 },
                { typeof(FlagGroupStorage), 4 },
                { typeof(FpuStackStorage), 5 },
                { typeof(TemporaryStorage), 6 },
                { typeof(GlobalStorage), 7 },
                { typeof(MemoryStorage), 8 },
            };

            public static StorageCollator Instance { get; } = new StorageCollator();

            public int Compare(Storage? x, Storage? y)
            {
                if (x is null)
                    return y is null ? 0 : -1;
                if (y is null)
                    return 1;
                if (x.GetType() != y.GetType())
                {
                    try
                    {
                        return precedences[x.GetType()].CompareTo(precedences[y.GetType()]);
                    }
                    catch
                    {
                        if (x.GetType().Name.Contains("PIC") ||
                            y.GetType().Name.Contains("PIC"))
                            return x.Domain.CompareTo(y.Domain);
                        throw;
                    }
                }
                if (x is StackStorage stgX && y is StackStorage stgY)
                {
                    if (stgX.StackOffset >= 0)
                    {
                        // X is an argument.
                        if (stgY.StackOffset >= 0)
                            return stgX.StackOffset.CompareTo(stgY.StackOffset);
                        return -1;
                    }
                    if (stgY.StackOffset >= 0)
                    {
                        // x is a local, y is an argument.
                        return 1;
                    }
                    return stgY.StackOffset.CompareTo(stgX.StackOffset);
                }
                else if (x is SequenceStorage seqX && x is SequenceStorage seqY)
                {
                    var d = seqX.Elements.Length.CompareTo(seqY.Elements.Length);
                    if (d != 0)
                        return d;
                    for (int i = 0; i < Math.Min(seqX.Elements.Length, seqY.Elements.Length); i++)
                    {
                        var cmp = this.Compare(seqX.Elements[i], seqY.Elements[i]);
                        if (cmp != 0)
                            return cmp;
                    }
                    return 0;
                }
                else if (x is FlagGroupStorage grfX && y is FlagGroupStorage grfY)
                {
                    var d = Compare(grfX.FlagRegister, grfY.FlagRegister);
                    if (d != 0)
                        return d;
                    return grfX.FlagGroupBits.CompareTo(grfY.FlagGroupBits);
                }
                else if (x is FpuStackStorage fpuX && y is FpuStackStorage fpuY)
                {
                    return fpuX.FpuStackOffset.CompareTo(fpuY.FpuStackOffset);
                }
                if (x.Domain != y.Domain)
                    return x.Domain.CompareTo(y.Domain);
                if (x.BitSize != y.BitSize)
                    return x.BitSize.CompareTo(y.BitSize);
                return 0;
            }
        }
    }
}
