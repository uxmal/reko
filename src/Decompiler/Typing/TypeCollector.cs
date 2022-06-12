#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Typing
{
    /// <summary>
    /// Collects the data types of all the expressions of the program.
    /// </summary>
    public class TypeCollector : InstructionVisitor
    {
        private readonly TypeFactory factory;
        private readonly TypeStore store;
        private readonly Program program;
        private readonly ExpressionTypeAscender asc;
        private readonly ExpressionTypeDescender desc;
        private readonly DecompilerEventListener eventListener;
        private Statement? stmCur;
        private bool seenPhi;

        public TypeCollector(
            TypeFactory factory, 
            TypeStore store,
            Program program,
            DecompilerEventListener eventListener)
        {
            this.factory = factory;
            this.store = store;
            this.program = program;
            this.eventListener = eventListener ?? throw new ArgumentNullException(nameof(eventListener));
            this.asc = new ExpressionTypeAscender(program, store, factory);
            this.desc = new ExpressionTypeDescender(program, store, factory);
        }

        public void CollectTypes()
        {
            CollectGlobalType();
            CollectUserGlobalVariableTypes();
            CollectImageSymbols();
            int cProc = program.Procedures.Count;
            int i = 0;
            foreach (Procedure proc in program.Procedures.Values)
            {
                eventListener.ShowProgress("Collecting data types.", i++, cProc);
                CollectProcedureSignature(proc);
                foreach (Statement stm in proc.Statements)
                {
                    if (eventListener.IsCanceled())
                        return;
                    try
                    {
                        this.stmCur = stm;
                        stm.Instruction.Accept(this);
                    }
                    catch (Exception ex)
                    {
                        eventListener.Error(
                            eventListener.CreateStatementNavigator(program, stm),
                            ex,
                            "An error occurred while processing the statement {0}.",
                            stm);
                    }
                }
            }
        }

        /// <summary>
        /// Collect the type of the structure containing the global variables.
        /// </summary>
        public void CollectGlobalType()
        {
            desc.MeetDataType(program.Globals, factory.CreatePointer(
                            factory.CreateStructureType(),
                            program.Platform.PointerType.BitSize));
        }

        /// <summary>
        /// Given a list of user-specified globals, make sure fields are present in
        /// the program
        /// </summary>
        /// <param name="segmentTypes"></param>
        public void CollectUserGlobalVariableTypes()
        {
            var deser = program.CreateTypeLibraryDeserializer();
            foreach (var ud in program.User.Globals)
            {
                if (ud.Value.DataType != null)
                {
                    var dt = ud.Value.DataType.Accept(deser);
                    AddGlobalField(dt, ud.Key, ud.Value.Name);
                }
            }
        }

        /// <subject>
        /// Make sure fields are present for all the image symbols.
        /// </subject>
        public void CollectImageSymbols()
        {
            foreach (var sym in program.ImageSymbols.Values)
            {
                if (sym != null && sym.DataType != null && sym.Address != null &&
                    sym.Type == SymbolType.Data && !(sym.DataType is UnknownType))
                {
                    DataType dtField;
                    if (sym.DataType.IsWord)
                    {
                        var tvField = store.CreateTypeVariable(factory);
                        tvField.OriginalDataType = sym.DataType;
                        tvField.DataType = sym.DataType;
                        dtField = tvField;
                    }
                    else
                    {
                        dtField = sym.DataType;
                    }

                    AddGlobalField(dtField, sym.Address, sym.Name);
                }
            }
        }

        private void AddGlobalField(DataType dtField, Address address, string? name)
        {
            if (address.Selector.HasValue)
            {
                if (program.SegmentMap.TryFindSegment(address, out var seg) &&
                    program.TypeStore.SegmentTypes.TryGetValue(seg, out var structureType))
                {
                    var f = new StructureField((int) address.Offset, dtField, name);
                    structureType.Fields.Add(f);
                }
            }
            else
            {
                var offset = (int) address.ToLinear();
                var f = new StructureField(offset, dtField, name);
                var ptrGlobals = (Pointer) program.Globals.TypeVariable!.OriginalDataType;
                ((StructureType)ptrGlobals.Pointee).Fields.Add(f);
            }
        }

        /// <summary>
        /// Add the traits of the procedure's signature.
        /// </summary>
        private void CollectProcedureSignature(Procedure proc)
        {
            FunctionType sig = proc.Signature;
            if (!sig.HasVoidReturn)
            {
                desc.MeetDataType(sig.ReturnValue, sig.ReturnValue.DataType);
            }
            if (sig.Parameters != null)
            {
                foreach (var p in sig.Parameters)
                {
                    desc.MeetDataType(p, p.DataType);
                }
            }
        }

        public void VisitAssignment(Assignment ass)
        {
            var dtSrc = ass.Src.Accept(asc);
            desc.MeetDataType(ass.Src, dtSrc);
            ass.Src.Accept(desc, ass.Src.TypeVariable!);

            var dtDst = ass.Dst.Accept(asc);
            desc.MeetDataType(ass.Dst, dtDst);
            desc.MeetDataType(ass.Dst, dtSrc);
            ass.Dst.Accept(desc, ass.Dst.TypeVariable!);
        }

        public void VisitBranch(Branch branch)
        {
            branch.Condition.Accept(asc);
            desc.MeetDataType(branch.Condition, PrimitiveType.Bool);
            branch.Condition.Accept(desc, branch.Condition.TypeVariable!);
        }

        public void VisitCallInstruction(CallInstruction call)
        {
            call.Callee.Accept(asc);
            desc.MeetDataType(
                          call.Callee,
                          new Pointer(
                              new CodeType(),
                              program.Platform.PointerType.BitSize));
            call.Callee.Accept(desc, call.Callee.TypeVariable!);
        }

        public void VisitComment(CodeComment comment)
        {
        }

        public void VisitDeclaration(Declaration decl)
        {
            // Pattern after VisitAssignment
            DataType? dtExp = null;
            if (decl.Expression != null)
            {
                dtExp = decl.Expression.Accept(asc);
                desc.MeetDataType(decl.Expression, dtExp);
                decl.Expression.Accept(desc, decl.Expression.TypeVariable!);
            }
            var dt = decl.Identifier.Accept(asc);
            desc.MeetDataType(decl.Identifier, dt);
            decl.Identifier.Accept(desc, decl.Identifier.TypeVariable!);
            if (dtExp != null)
            {
                // OK to do this as this is the _declaration_
                // of the variable. 
                desc.MeetDataType(decl.Identifier, dtExp);
            }
        }

        public void VisitDefInstruction(DefInstruction def)
        {
        }

        public void VisitGotoInstruction(GotoInstruction g)
        {
            var dt = g.Target.Accept(asc);
            desc.MeetDataType(g.Target, dt);
            g.Target.Accept(desc, g.Target.TypeVariable!);
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
            if (!seenPhi)
            {
                seenPhi = true;
                eventListener.Warn(
                    eventListener.CreateBlockNavigator(this.program, stmCur!.Block),
                    "Phi functions will be ignored by type analysis. " +
                    "This may be caused by a failure in a previous stage of the decompilation.");
            }
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
            {
                var dt = ret.Expression.Accept(asc);
                desc.MeetDataType(ret.Expression, dt);
                ret.Expression.Accept(desc, ret.Expression.TypeVariable!);
            }
        }

        public void VisitSideEffect(SideEffect side)
        {
            var dt = side.Expression.Accept(asc);
            desc.MeetDataType(side.Expression, dt);
            side.Expression.Accept(desc, side.Expression.TypeVariable!);
        }

        public void VisitStore(Store store)
        {
            var dt = store.Src.Accept(asc);
            desc.MeetDataType(store.Src, dt);
            store.Src.Accept(desc, store.Src.TypeVariable!);

            dt = store.Dst.Accept(asc);
            desc.MeetDataType(store.Dst, dt);
            store.Dst.Accept(desc, store.Dst.TypeVariable!);
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            var dt = si.Expression.Accept(asc);
            desc.MeetDataType(si.Expression, dt);
            si.Expression.Accept(desc, si.Expression.TypeVariable!);
        }

        public void VisitUseInstruction(UseInstruction use)
        {
            var dt = use.Expression.Accept(asc);
            desc.MeetDataType(use.Expression, dt);
            use.Expression.Accept(desc, use.Expression.TypeVariable!);
        }
    }
}
