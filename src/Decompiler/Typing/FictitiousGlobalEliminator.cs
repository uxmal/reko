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
using Reko.Core.Types;

namespace Reko.Typing
{
    /// <summary>
    /// Eliminate fictitious "global" identifiers. For instance, transform
    /// "global->ptr0001" to "g_ptr0001" or "global->userVariable" to
    /// "userVariable"
    /// </summary>
    public class FictitiousGlobalEliminator : InstructionTransformer
    {
        private readonly Program program;

        public FictitiousGlobalEliminator(Program program)
        {
            this.program = program;
        }

        public void Transform(Procedure proc)
        {
            foreach (var stm in proc.Statements)
            {
                stm.Instruction = Transform(stm.Instruction);
            }
        }

        public override Expression VisitFieldAccess(FieldAccess acc)
        {
            var str = acc.Structure.Accept(this);
            acc = new FieldAccess(acc.DataType, str, acc.Field);
            if (!(acc.Structure is Dereference deref))
                return acc;
            if (deref.Expression != program.Globals)
                return acc;
            if (!(acc.Field is StructureField field))
                return acc;
            var name = program.NamingPolicy.GlobalName(field);
            return Identifier.Global(name, acc.DataType);
        }
    }
}
