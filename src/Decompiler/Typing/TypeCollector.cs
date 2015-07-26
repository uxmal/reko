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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
    /// <summary>
    /// Performs a type analysis of the program.
    /// </summary>
    public class TypeCollector : InstructionVisitor<bool>
    {
        private TypeFactory factory;
        private TypeStore store;
        private Program program;
        private Procedure proc;
        private ExpressionTypeAscender asc;
        private ExpressionTypeDescender desc;

        public TypeCollector(TypeFactory factory, TypeStore store, Program program)
        {
            this.factory = factory;
            this.store = store;
            this.program = program;
            this.asc = new ExpressionTypeAscender(program.Platform, store, factory);
            this.desc = new ExpressionTypeDescender(program, store, factory);
        }

        public void CollectTypes()
        {
            desc.MeetDataType(program.Globals, factory.CreatePointer(
                factory.CreateStructureType(),
                program.Platform.PointerType.Size));
            foreach (Procedure p in program.Procedures.Values)
            {
                proc = p;
                CollectProcedureSignature(p);
                foreach (Statement stm in p.Statements)
                {
                    stm.Instruction.Accept(this);
                }
            }
        }

        /// <summary>
        /// Add the traits of the procedure's signature.
        /// </summary>
        private void CollectProcedureSignature(Procedure proc)
        {
            ProcedureSignature sig = proc.Signature;
            if (sig.ReturnValue != null)
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

        public bool VisitAssignment(Assignment ass)
        {
            var dtSrc = ass.Src.Accept(asc);
            desc.MeetDataType(ass.Src, dtSrc);
            ass.Src.Accept(desc, ass.Src.TypeVariable);

            var dtDst = ass.Dst.Accept(asc);
            desc.MeetDataType(ass.Dst, dtDst);
            desc.MeetDataType(ass.Dst, dtSrc);
            ass.Dst.Accept(desc, ass.Dst.TypeVariable);
            return false;
        }

        public bool VisitBranch(Branch branch)
        {
            var dt = branch.Condition.Accept(asc);
            desc.MeetDataType(branch.Condition, PrimitiveType.Bool);
            branch.Condition.Accept(desc, branch.Condition.TypeVariable);
            return false;
        }

        public bool VisitCallInstruction(CallInstruction call)
        {
            call.Callee.Accept(asc);
            desc.MeetDataType(
                          call.Callee,
                          new Pointer(
                              new CodeType(),
                              program.Platform.PointerType.Size));
            return call.Callee.Accept(desc, call.Callee.TypeVariable);
        }

        public bool VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
            {
                var dt = decl.Expression.Accept(asc);
                desc.MeetDataType(decl.Expression, dt);
                decl.Expression.Accept(desc, decl.Expression.TypeVariable);
            }
            return false;
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public bool VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
            {
                var dt = ret.Expression.Accept(asc);
                desc.MeetDataType(ret.Expression, dt);
                ret.Expression.Accept(desc, ret.Expression.TypeVariable);
            }
            return false;
        }

        public bool VisitSideEffect(SideEffect side)
        {
            var dt = side.Expression.Accept(asc);
            desc.MeetDataType(side.Expression, dt);
            side.Expression.Accept(desc, side.Expression.TypeVariable);
            return false;
        }

        public bool VisitStore(Store store)
        {
            var dt = store.Src.Accept(asc);
            desc.MeetDataType(store.Src, dt);
            store.Src.Accept(desc, store.Src.TypeVariable);

            dt = store.Dst.Accept(asc);
            desc.MeetDataType(store.Dst, dt);
            store.Dst.Accept(desc, store.Dst.TypeVariable);
            return false;
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            var dt = si.Expression.Accept(asc);
            desc.MeetDataType(si.Expression, dt);
            si.Expression.Accept(desc, si.Expression.TypeVariable);
            return false;
        }

        public bool VisitUseInstruction(UseInstruction use)
        {
            var dt = use.Expression.Accept(asc);
            desc.MeetDataType(use.Expression, dt);
            use.Expression.Accept(desc, use.Expression.TypeVariable);
            return false;
        }
    }
}
