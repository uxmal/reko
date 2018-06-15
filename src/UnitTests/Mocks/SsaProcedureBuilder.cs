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
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Supports the building of a procedure in Static Single Assignment form.
    /// </summary>
    /// <remarks>
    /// Some unit tests require procedure to be in Static Single Assignment
    /// form. This class gives possibility to build it without the overhead of
    /// using the SSATransform class.
    /// </remarks>
    public class SsaProcedureBuilder : ProcedureBuilder
    {
        public SsaState Ssa { get; private set; }

        public SsaProcedureBuilder() : base()
        {
            this.Ssa = new SsaState(Procedure);
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
            switch (instr)
            {
            case Assignment ass:
                Ssa.Identifiers[ass.Dst].DefStatement = stm;
                Ssa.Identifiers[ass.Dst].DefExpression = ass.Src;
                break;
            case PhiAssignment phiAss:
                Ssa.Identifiers[phiAss.Dst].DefStatement = stm;
                Ssa.Identifiers[phiAss.Dst].DefExpression = phiAss.Src;
                break;
            case Store store:
                if (store.Dst is MemoryAccess access)
                {
                    var memId = AddMemIdToSsa(access.MemoryId);
                    Ssa.Identifiers[memId].DefStatement = stm;
                    Ssa.Identifiers[memId].DefExpression = null;
                    var ea = access.EffectiveAddress;
                    var dt = access.DataType;
                    if (store.Dst is SegmentedAccess sa)
                    {
                        var basePtr = sa.BasePointer;
                        store.Dst = new SegmentedAccess(memId, basePtr, ea, dt);
                    }
                    else
                    {
                        store.Dst = new MemoryAccess(memId, ea, dt);
                    }
                }
                break;
            }
            Ssa.AddUses(stm);
            return stm;
        }

        private MemoryIdentifier AddMemIdToSsa(MemoryIdentifier idOld)
        {
            if (Ssa.Identifiers.Contains(idOld))
                return idOld;
            var idNew = new MemoryIdentifier(Ssa.Identifiers.Count, idOld.DataType);
            var sid = new SsaIdentifier(idNew, idOld, null, null, false);
            Ssa.Identifiers.Add(idNew, sid);
            return idNew;
        }

        public override MemoryAccess Mem32(Expression ea)
        {
            var access = base.Mem32(ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override SegmentedAccess SegMem(DataType dt, Expression basePtr, Expression ptr)
        {
            var access = base.SegMem(dt, basePtr, ptr);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new SegmentedAccess(
                memId,
                access.BasePointer,
                access.EffectiveAddress,
                access.DataType);
        }
    }
}
