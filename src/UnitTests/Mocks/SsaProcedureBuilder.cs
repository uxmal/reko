#region License
/* 
 * Copyright (C) 1999-2018 Pavel Tomin.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Supports the building of a procedure in Static Single Assignment form.
    /// </summary>
    /// <remarks>
    /// Some unit tests require procedure to be in Static Single Assignment
    /// form. This class gives possibility to build it without ssa transforming
    /// </remarks>
    public class SsaProcedureBuilder : ProcedureBuilder
    {
        public SsaState Ssa { get; private set; }

        public SsaProcedureBuilder() : base()
        {
            this.Ssa = new SsaState(Procedure, null);
        }

        public RegisterStorage RegisterStorage(string name, PrimitiveType pt)
        {
            return new RegisterStorage(name, Ssa.Identifiers.Count, 0, pt);
        }

        public Identifier Reg(string name, RegisterStorage r)
        {
            var id = new Identifier(name, r.DataType, r);
            var sid = new SsaIdentifier(id, id, null, null, false);
            Ssa.Identifiers.Add(id, sid);
            return sid.Identifier;
        }

        private Identifier Reg(string name, PrimitiveType pt)
        {
            return Reg(name, RegisterStorage(name, pt));
        }

        public override Identifier Local32(string name, int offset)
        {
            var local = base.Local32(name, offset);
            var sid = new SsaIdentifier(local, local, null, null, false);
            Ssa.Identifiers.Add(local, sid);
            return sid.Identifier;
        }

        public Identifier Reg32(string name)
        {
            return Reg(name, PrimitiveType.Word32);
        }

        public Identifier Reg16(string name)
        {
            return Reg(name, PrimitiveType.Word16);
        }


        public Identifier Reg8(string name)
        {
            return Reg(name, PrimitiveType.Byte);
        }

        public override Statement Emit(Instruction instr)
        {
            var stm = base.Emit(instr);
            var ass = instr as Assignment;
            if (ass != null)
            {
                Ssa.Identifiers[ass.Dst].DefStatement = stm;
                Ssa.Identifiers[ass.Dst].DefExpression = ass.Src;
            }
            var phiAss = instr as PhiAssignment;
            if (phiAss != null)
            {
                Ssa.Identifiers[phiAss.Dst].DefStatement = stm;
                Ssa.Identifiers[phiAss.Dst].DefExpression = phiAss.Src;
            }
            Ssa.AddUses(stm);
            return stm;
        }

        private void AddMemIdToSsa(MemoryAccess access)
        {
            var idOld = access.MemoryId;
            var idNew = new MemoryIdentifier(Ssa.Identifiers.Count, idOld.DataType);
            access.MemoryId = idNew;
            var sid = new SsaIdentifier(idNew, idOld, null, null, false);
            Ssa.Identifiers.Add(idNew, sid);
        }

        public new MemoryAccess Mem32(Expression ea)
        {
            var access = base.Mem32(ea);
            AddMemIdToSsa(access);
            return access;
        }

        public new SegmentedAccess SegMem(DataType dt, Expression basePtr, Expression ptr)
        {
            var access = base.SegMem(dt, basePtr, ptr);
            AddMemIdToSsa(access);
            return access;
        }
    }
}
