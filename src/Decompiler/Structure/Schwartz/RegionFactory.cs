﻿#region License
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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Structure.Schwartz
{
    public class RegionFactory : InstructionVisitor<AbsynStatement>
    {
        private List<AbsynStatement> stms;
        private RegionType regType;
        private Expression exp;
        
        public Region Create(Block b)
        {
            this.stms = new List<AbsynStatement>();
            this.regType = b.Succ.Count != 0 ? RegionType.Linear : RegionType.Tail;
            this.exp = null;
            foreach (var stm in b.Statements)
            {
                var s = stm.Instruction.Accept(this);
                if (s != null)
                    stms.Add(s);
            }
            var region = new Region(b)
            {
                Type = regType,
                Statements = stms,
                Expression = exp,
            };
            return region;
        }

        public AbsynStatement VisitAssignment(Assignment ass)
        {
            return new AbsynAssignment(ass.Dst, ass.Src);
        }

        public AbsynStatement VisitBranch(Branch branch)
        {
            regType = RegionType.Condition;
            exp = branch.Condition;
            return null;
        }

        public AbsynStatement VisitCallInstruction(CallInstruction ci)
        {
            return new AbsynSideEffect(
                new Application(ci.Callee, VoidType.Instance));
        }

        public AbsynStatement VisitDeclaration(Declaration decl)
        {
            return new AbsynDeclaration(decl.Identifier, decl.Expression);
        }

        public AbsynStatement VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public AbsynStatement VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public AbsynStatement VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public AbsynStatement VisitReturnInstruction(ReturnInstruction ret)
        {
            regType = RegionType.Tail;
            return new AbsynReturn(ret.Expression);
        }

        public AbsynStatement VisitSideEffect(SideEffect side)
        {
            return new AbsynSideEffect(side.Expression);
        }

        public AbsynStatement VisitStore(Store store)
        {
            return new AbsynAssignment(store.Dst, store.Src);
        }

        public AbsynStatement VisitSwitchInstruction(SwitchInstruction si)
        {
            regType = RegionType.IncSwitch; 
            exp = si.Expression;
            return null;
        }

        public AbsynStatement VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }
    }
}
