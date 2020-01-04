#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    public class SsaDefinitionsCollector : InstructionVisitorBase
    {
        private List<Identifier> definitions;

        public SsaDefinitionsCollector()
        {
            this.definitions = new List<Identifier>();
        }

        public ICollection<Identifier> CollectDefinitions(Statement stm)
        {
            definitions.Clear();
            stm.Instruction.Accept(this);
            return definitions;
        }

        #region InstructionVisitor Members

        public override void VisitAssignment(Assignment a)
        {
            base.VisitAssignment(a);
            definitions.Add(a.Dst);
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            base.VisitCallInstruction(ci);
            definitions.AddRange(ci.Definitions
                .Select(d => d.Expression as Identifier)
                .Where(i => i != null));
        }

        public override void VisitDefInstruction(DefInstruction def)
        {
            base.VisitDefInstruction(def);
            definitions.Add(def.Identifier);
        }

        public override void VisitPhiAssignment(PhiAssignment phi)
        {
            base.VisitPhiAssignment(phi);
            definitions.Add(phi.Dst);
        }

        public override void VisitStore(Store store)
        {
            base.VisitStore(store);
            if (store.Dst is MemoryAccess access)
            {
                definitions.Add(access.MemoryId);
            }
        }

        #endregion

        #region IExpressionVisitor Members

        public override void VisitOutArgument(OutArgument outArg)
        {
            if (outArg.Expression is Identifier outId)
            {
                definitions.Add(outId);
            }
        }

        #endregion
    }
}
