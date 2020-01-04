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
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
    public class InstructionUseCollector : InstructionUseVisitorBase
    {
        private IDictionary<Identifier, int> idMap;

        public InstructionUseCollector()
        {
            this.idMap = new Dictionary<Identifier, int>();
        }

        public IDictionary<Identifier, int> CollectUses(Statement stm)
        {
            idMap.Clear();
            stm.Instruction.Accept(this);
            return idMap;
        }

        protected override void UseIdentifier(Identifier id)
        {
            if (idMap.ContainsKey(id))
                idMap[id]++;
            else
                idMap.Add(id, 1);
        }
    }
}
